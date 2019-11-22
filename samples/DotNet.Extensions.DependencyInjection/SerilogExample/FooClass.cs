using DotNet.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerilogExample
{
    public class FooClass : IDisposable
    {
        private IDisposable _disposable;
        private ILogger _logger;

        public FooClass(IMutableService<ILogger> logger)
        {
            _logger = logger.CurrentValue;
            _disposable = logger.OnChange(newLogger => _logger = newLogger);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Verbose(string message, params object[] args)
        {
            _logger.Verbose(message, args);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
