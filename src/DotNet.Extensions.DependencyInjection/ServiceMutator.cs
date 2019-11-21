using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace DotNet.Extensions.DependencyInjection
{
    public class ServiceMutator<TInterface, TService> : IServiceMutator<TInterface>, IMutableService<TInterface>, IDisposable
        where TService : TInterface
    {
        private MutableService<ServiceMutator<TInterface, TService>, ServiceMutator<TInterface, TService>> _wrappedMutableService;

        private TInterface _value;
        public TInterface Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaiseChanged();
            }
        }

        private ServiceChangeToken _changeToken = new ServiceChangeToken();

        public IChangeToken GetChangeToken() => _changeToken;

        public ServiceMutator(TInterface value)
        {
            _value = value;
            _wrappedMutableService = new MutableService<ServiceMutator<TInterface, TService>, ServiceMutator<TInterface, TService>>(() => this, _ => GetChangeToken());
        }

        public void Update(Action<TInterface> action)
        {
            action.Invoke(_value);
            RaiseChanged();
        }

        public void Update(Func<TInterface, TInterface> func)
        {
            Value = func.Invoke(_value);
        }

        private void RaiseChanged()
        {
            var previousToken = Interlocked.Exchange(ref _changeToken, new ServiceChangeToken());
            previousToken.OnReload();
            previousToken.Dispose();
        }

        TInterface IMutableService<TInterface>.CurrentValue => Value;

        IDisposable IMutableService<TInterface>.OnChange(Action<TInterface> listener) => _wrappedMutableService.OnChange(sm => listener(sm.Value));

        public void Dispose()
        {
            _wrappedMutableService?.Dispose();
            _changeToken.Dispose();
        }
    }
}
