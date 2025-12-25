using courseApi.Models;

namespace courseApi.Services
{
	public class InMemoryProductStore
	{
		private readonly List<Product> _products = new();
		private int _id = 1;

		public List<Product> GetAll()
		{
			return _products;
		}

		public Product? Get(int id)
		{
			return _products.FirstOrDefault(p => p.Id == id);
		}

		public Product Add(Product product)
		{
			product.Id = _id++;
			_products.Add(product);
			return product;
		}

		public bool Update(int id, Product model)
		{
			var p = Get(id);
			if (p == null) return false;

			p.Sku = model.Sku;
			p.Name = model.Name;
			p.Description = model.Description;
			p.Price = model.Price;
			p.Stock = model.Stock;
			p.CategoryId = model.CategoryId;

			return true;
		}

		public bool Delete(int id)
		{
			var p = Get(id);
			if (p == null) return false;

			_products.Remove(p);
			return true;
		}
	}
}
