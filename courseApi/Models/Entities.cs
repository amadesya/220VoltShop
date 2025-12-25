using System;
using System.Collections.Generic;

namespace courseApi.Models
{
	public class Role
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class User
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string Name { get; set; }
		// Контактный телефон пользователя
		public string? Phone { get; set; }
		// Ссылка на фотографию профиля
		public string? PhotoUrl { get; set; }
		public int RoleId { get; set; }
		public Role Role { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? ParentId { get; set; }
	}

	public class Product
	{
		public int Id { get; set; }
		public string Sku { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }
		public int? CategoryId { get; set; }
		public Category Category { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
        public string ImageUrl { get; set; }
    }

	public class Order
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public decimal Total { get; set; }
		public string Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public ICollection<OrderItem> Items { get; set; }
	}

	public class OrderItem
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public Order Order { get; set; }
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
	}

	public class Report
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public int? CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Payload { get; set; } // JSON
	}

	public class ImportExport
	{
		public int Id { get; set; }
		public string Filename { get; set; }
		public string Action { get; set; }
		public int? PerformedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public string Status { get; set; }
		public string Details { get; set; }
	}
}