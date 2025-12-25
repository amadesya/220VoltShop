using System;
using System.Threading.Tasks;

namespace courseProd.Services
{
    public class ToastService
    {
        public event Func<string, string, Task>? OnShow;

        public Task ShowAsync(string message, string type = "success")
        {
            return OnShow?.Invoke(message, type) ?? Task.CompletedTask;
        }
    }
}
