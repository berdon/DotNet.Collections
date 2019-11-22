using DotNet.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleUsage
{
    public class InjectedClass : IDisposable
    {
        private IDisposable _disposable;
        private MyService _service;

        public InjectedClass(IMutableService<MyService> mutableService)
        {
            _service = mutableService.CurrentValue;
            _disposable = mutableService.OnChange(service => _service = service);
        }

        public void DisplayMessage()
        {
            _service.Foo();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
