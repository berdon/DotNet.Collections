using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace DotNet.Extensions.DependencyInjection
{
    public class MutableService<TInterface, TService> : IMutableService<TInterface>, IDisposable
        where TService : TInterface
    {
        public TInterface CurrentValue { get; set; }

        private event Action<TInterface> _onChange;
        private readonly Func<TInterface> _factory;
        private readonly IDisposable _disposer;

        public MutableService(Func<TInterface> factory, Func<TInterface, IChangeToken> tokenProvider)
        {
            CurrentValue = factory.Invoke();
            _factory = factory;
            _disposer = ChangeToken.OnChange(() => tokenProvider(CurrentValue), InvokeChanged);
        }

        private void InvokeChanged()
        {
            _onChange.Invoke(_factory.Invoke());
        }

        public IDisposable OnChange(Action<TInterface> listener)
        {
            var disposable = new ChangeTrackerDisposable(this, listener);
            _onChange += disposable.OnChange;
            return disposable;
        }

        public void Dispose()
        {
            _disposer?.Dispose();
            foreach(var handler in _onChange.GetInvocationList())
            {
                _onChange -= (Action<TInterface>) handler;
            }
        }

        private class ChangeTrackerDisposable : IDisposable
        {
            private readonly Action<TInterface> _listener;
            private readonly MutableService<TInterface, TService> _monitor;

            public ChangeTrackerDisposable(MutableService<TInterface, TService> monitor, Action<TInterface> listener)
            {
                _listener = listener;
                _monitor = monitor;
            }

            public void OnChange(TInterface options) => _listener.Invoke(options);

            public void Dispose() => _monitor._onChange -= OnChange;
        }
    }
}
