using Microsoft.EntityFrameworkCore;
using courseApi.Data;
using courseApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Установите пакеты:
// dotnet add package Microsoft.EntityFrameworkCore
// dotnet add package Pomelo.EntityFrameworkCore.MySql
// dotnet add package ClosedXML
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
// dotnet add package BCrypt.Net-Next  (для хеширования паролей)

builder.Services.AddControllers();
builder.Services.AddSingleton<courseApi.Services.InMemoryProductStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CourseStoreContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 29))
    ));

// JwtService через DI
builder.Services.AddSingleton<JwtService>();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "please_replace_with_a_long_secure_random_secret_in_production";
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
		ClockSkew = TimeSpan.Zero
	};
});

// add CORS policy for local frontend
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowLocalhostFrontend", policy =>
		policy.WithOrigins("http://localhost:5004", "https://localhost:5005", "http://localhost:5002", "https://localhost:5003")
		      .AllowAnyHeader()
		      .AllowAnyMethod()
		      .AllowCredentials());
});

builder.Services.AddAuthorization(); // роли будут использоваться в контроллерах

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed demo data in development environment (idempotent)
using (var scope = app.Services.CreateScope())
{
	try
	{
		var db = scope.ServiceProvider.GetRequiredService<CourseStoreContext>();
		// apply pending migrations automatically (dev convenience)
		try { db.Database.Migrate(); } catch (Exception migEx) { Console.WriteLine($"Migration error: {migEx.Message}"); }
		DbSeeder.Seed(db);
		// Ensure new columns exist in case DB was created before migration files were updated
		try
		{
			// use INFORMATION_SCHEMA to check column existence (works on older MySQL versions)
			var conn = db.Database.GetDbConnection();
			try
			{
				if (conn.State != System.Data.ConnectionState.Open) conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Users' AND COLUMN_NAME = 'Phone'";
					var res = cmd.ExecuteScalar();
					var hasPhone = Convert.ToInt32(res) > 0;
					if (!hasPhone)
					{
						try { db.Database.ExecuteSqlRaw("ALTER TABLE `Users` ADD COLUMN `Phone` longtext NULL;"); }
						catch (Exception ex) { Console.WriteLine("AddPhone exec error: " + ex.Message); }
					}
				}

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'Users' AND COLUMN_NAME = 'PhotoUrl'";
					var res = cmd.ExecuteScalar();
					var hasPhoto = Convert.ToInt32(res) > 0;
					if (!hasPhoto)
					{
						try { db.Database.ExecuteSqlRaw("ALTER TABLE `Users` ADD COLUMN `PhotoUrl` longtext NULL;"); }
						catch (Exception ex) { Console.WriteLine("AddPhoto exec error: " + ex.Message); }
					}
				}

				// If a shadow foreign key column OrderId1 was previously created by EF, drop its FK and the column
				using (var cmd2 = conn.CreateCommand())
				{
					try
					{
						cmd2.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'OrderItems' AND COLUMN_NAME = 'OrderId1'";
						var colRes = cmd2.ExecuteScalar();
						var hasOrderId1 = Convert.ToInt32(colRes) > 0;
						if (hasOrderId1)
						{
							// try to find a foreign key constraint name for OrderId1
							cmd2.CommandText = "SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'OrderItems' AND COLUMN_NAME = 'OrderId1' AND REFERENCED_TABLE_NAME = 'Orders' LIMIT 1";
							var fkObj = cmd2.ExecuteScalar();
							if (fkObj != null && fkObj != DBNull.Value)
							{
								var fkName = fkObj.ToString();
								try { db.Database.ExecuteSqlRaw($"ALTER TABLE `OrderItems` DROP FOREIGN KEY `{fkName}`;"); } catch (Exception fx) { Console.WriteLine("Drop FK OrderId1 error: " + fx.Message); }
							}

							// drop the column if it still exists
							try { db.Database.ExecuteSqlRaw("ALTER TABLE `OrderItems` DROP COLUMN `OrderId1`;"); } catch (Exception cx) { Console.WriteLine("Drop column OrderId1 error: " + cx.Message); }
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine("OrderId1 cleanup check error: " + ex.Message);
					}
				}
			}
			finally { try { conn.Close(); } catch { } }
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Ensure-columns error: {ex.Message}");
		}
	}
	catch (Exception ex)
	{
		// do not fail startup for seeding errors in dev
		Console.WriteLine($"DB seeding error: {ex.Message}");
	}
}

app.UseCors("AllowLocalhostFrontend");

app.UseStaticFiles(); // serve frontend build from wwwroot if present

app.MapControllers();

// simple root response or fallback to index.html if SPA placed in wwwroot
app.MapGet("/", () => Results.Text("Course API is running"));
app.MapFallbackToFile("index.html");

app.Run();