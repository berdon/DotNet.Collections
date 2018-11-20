using System;
using System.Collections.Generic;
using System.Text;

namespace DotNet.Extensions.DependencyInjection
{
    public interface IServiceMutator<TService>
    {
        /// <summary>
        /// Provides a means of updating the existing tracked service reference
        /// and notifies registered subscribers of the update.
        /// </summary>
        void Update(Action<TService> action);

        /// <summary>
        /// Updates the underlying, tracked mutable service with the new reference
        /// and notifies registered subscribers of the update.
        /// </summary>
        void Update(Func<TService, TService> func);
    }
}
