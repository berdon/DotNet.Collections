using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Extensions.DependencyInjection
{
    public interface IMutableService<out TService>
    {
        /// <summary>
        /// The current value of the mutable service.
        /// </summary>
        TService CurrentValue { get; }

        /// <summary>
        /// Registers a callback to be invoked when the service
        /// changes.
        /// </summary>
        IDisposable OnChange(Action<TService> handler);
    }
}
