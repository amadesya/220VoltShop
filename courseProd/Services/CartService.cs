using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace courseProd.Services
{
    public class CartService
    {
        private readonly IJSRuntime _js;
        public CartService(IJSRuntime js) { _js = js; }

        const string OrderKey = "courseProdOrder";

        public async Task<List<JsonElement>> GetItemsAsync()
        {
            var items = await _js.InvokeAsync<object>("courseProdCart.get");
            try {
                var json = JsonSerializer.Serialize(items);
                var doc = JsonDocument.Parse(json);
                var list = new List<JsonElement>();
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in doc.RootElement.EnumerateArray()) list.Add(e);
                }
                return list;
            } catch { return new List<JsonElement>(); }
        }

        public async Task<List<JsonElement>> AddItemAsync(object item)
        {
            var res = await _js.InvokeAsync<object>("courseProdCart.add", item);
            try {
                var json = JsonSerializer.Serialize(res);
                var doc = JsonDocument.Parse(json);
                var list = new List<JsonElement>();
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var e in doc.RootElement.EnumerateArray()) list.Add(e);
                }
                return list;
            } catch { return new List<JsonElement>(); }
        }

        public async Task ClearAsync()
        {
            await _js.InvokeVoidAsync("courseProdCart.clear");
        }

        public async Task<System.Collections.Generic.List<System.Text.Json.JsonElement>> UpdateQuantityAsync(string sku, int newQuantity)
        {
            var items = await GetItemsAsync();
            var list = new System.Collections.Generic.List<object>();
            bool changed = false;
            foreach (var e in items)
            {
                var obj = new System.Collections.Generic.Dictionary<string, object>();
                foreach (var prop in e.EnumerateObject())
                {
                    if (prop.Value.ValueKind == System.Text.Json.JsonValueKind.Number)
                    {
                        if (prop.Value.TryGetInt32(out var ival)) obj[prop.Name] = ival;
                        else obj[prop.Name] = prop.Value.GetDecimal();
                    }
                    else
                    {
                        obj[prop.Name] = prop.Value.GetString() ?? string.Empty;
                    }
                }
                var s = e.TryGetProperty("sku", out var sp) ? (sp.GetString() ?? string.Empty) : string.Empty;
                if (s == sku)
                {
                    obj["quantity"] = newQuantity;
                    changed = true;
                }
                list.Add(obj);
            }
            if (changed)
            {
                await _js.InvokeVoidAsync("courseProdCart.set", list);
                // return fresh list
                return await GetItemsAsync();
            }
            return items;
        }

        public async Task<System.Collections.Generic.List<System.Text.Json.JsonElement>> RemoveItemAsync(string sku)
        {
            try
            {
                var res = await _js.InvokeAsync<object>("courseProdCart.removeFromCart", sku);
                var json = System.Text.Json.JsonSerializer.Serialize(res);
                var doc = System.Text.Json.JsonDocument.Parse(json);
                var list = new System.Collections.Generic.List<System.Text.Json.JsonElement>();
                if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var e in doc.RootElement.EnumerateArray()) list.Add(e);
                }
                return list;
            }
            catch
            {
                return await GetItemsAsync();
            }
        }

        public async Task<decimal> GetTotalAsync()
        {
            var items = await GetItemsAsync();
            decimal total = 0m;
            foreach (var e in items)
            {
                if (e.TryGetProperty("price", out var p) && p.ValueKind == JsonValueKind.Number)
                {
                    var price = p.GetDecimal();
                    var qty = 1;
                    if (e.TryGetProperty("quantity", out var q) && q.ValueKind == JsonValueKind.Number) qty = q.GetInt32();
                    total += price * qty;
                }
            }
            return total;
        }

        public async Task SaveOrderDetailsAsync(object order)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", OrderKey, System.Text.Json.JsonSerializer.Serialize(order));
        }

        public async Task<string> GetOrderDetailsJsonAsync()
        {
            try
            {
                var res = await _js.InvokeAsync<string>("localStorage.getItem", OrderKey);
                return res ?? string.Empty;
            }
            catch { return string.Empty; }
        }
    }
}
