using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Categories;

namespace DotNet.Extensions.DependencyInjection.Tests
{
    [Category("DependencyInjection")]
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddedNonMutableIsRetrievableFromServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<CustomNonMutableService>>();

            Assert.NotNull(mutableService);
        }

        [Fact]
        public void AddedNonMutableAsInterfaceIsRetrievableFromServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomNonMutableService, CustomNonMutableService>(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<ICustomNonMutableService>>();

            Assert.NotNull(mutableService);
        }

        [Fact]
        public void AddedMutablesServiceMutatorIsRetrievableFromServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<CustomNonMutableService>>();

            Assert.NotNull(serviceMutator);
        }

        [Fact]
        public void AddedMutablesServiceMutatorAsInterfaceIsRetrievableFromServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomNonMutableService, CustomNonMutableService>(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<ICustomNonMutableService>>();

            Assert.NotNull(serviceMutator);
        }

        [Fact]
        public void AddedNonMutableOnChangeHandlerIsCalledOnUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<CustomNonMutableService>>();
            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<CustomNonMutableService>>();

            var mockCallback = new Mock<ICallbackWrapper<CustomNonMutableService>>(MockBehavior.Default);
            mutableService.OnChange(mockCallback.Object.OnChange);

            for (var i = 1; i <= 10; i++)
            {
                serviceMutator.Update(old => { });
                mockCallback.Verify(x => x.OnChange(It.IsAny<CustomNonMutableService>()), Times.Exactly(i));
            }
        }

        [Fact]
        public void AddedNonMutableAsInterfaceOnChangeHandlerIsCalledOnUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomNonMutableService, CustomNonMutableService>(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<ICustomNonMutableService>>();
            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<ICustomNonMutableService>>();

            var mockCallback = new Mock<ICallbackWrapper<ICustomNonMutableService>>(MockBehavior.Default);
            mutableService.OnChange(mockCallback.Object.OnChange);

            for (var i = 1; i <= 10; i++)
            {
                serviceMutator.Update(old => { });
                mockCallback.Verify(x => x.OnChange(It.IsAny<ICustomNonMutableService>()), Times.Exactly(i));
            }
        }

        [Fact]
        public void MultipleNonMutableSubscribersReceiveUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceBag = new List<(Mock<ICallbackWrapper<CustomNonMutableService>>, IDisposable)>();
            for(var i = 0; i < 10; i++)
            {
                var mockCallback = new Mock<ICallbackWrapper<CustomNonMutableService>>(MockBehavior.Default);
                serviceBag.Add((mockCallback, serviceProvider.GetRequiredService<IMutableService<CustomNonMutableService>>().OnChange(mockCallback.Object.OnChange)));
            }

            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<CustomNonMutableService>>();

            serviceMutator.Update(old => { });

            foreach(var tuple in serviceBag)
            {
                tuple.Item1.Verify(x => x.OnChange(It.IsAny<CustomNonMutableService>()), Times.Once());
            }
        }

        [Fact]
        public void MultipleNonMutableAsInterfaceSubscribersReceiveUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomNonMutableService, CustomNonMutableService>(new CustomNonMutableService());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceBag = new List<(Mock<ICallbackWrapper<ICustomNonMutableService>>, IDisposable)>();
            for (var i = 0; i < 10; i++)
            {
                var mockCallback = new Mock<ICallbackWrapper<ICustomNonMutableService>>(MockBehavior.Default);
                serviceBag.Add((mockCallback, serviceProvider.GetRequiredService<IMutableService<ICustomNonMutableService>>().OnChange(mockCallback.Object.OnChange)));
            }

            var serviceMutator = serviceProvider.GetRequiredService<IServiceMutator<ICustomNonMutableService>>();

            serviceMutator.Update(old => { });

            foreach (var tuple in serviceBag)
            {
                tuple.Item1.Verify(x => x.OnChange(It.IsAny<ICustomNonMutableService>()), Times.Once());
            }
        }

        [Fact]
        public void AddedMutableIsRetrievableFromServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(() => new CustomMutableService(), service => service.GetChangeToken());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<CustomMutableService>>();

            Assert.NotNull(mutableService);
        }

        [Fact]
        public void AddedMutableAsInterfaceIsRetrievableFromServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomMutableService, CustomMutableService>(() => new CustomMutableService(), service => service.GetChangeToken());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<ICustomMutableService>>();

            Assert.NotNull(mutableService);
        }

        [Fact]
        public void AddedMutableOnChangeHandlerIsCalledOnUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(() => new CustomMutableService(), service => service.GetChangeToken());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<CustomMutableService>>();

            var mockCallback = new Mock<ICallbackWrapper<CustomMutableService>>(MockBehavior.Default);
            mutableService.OnChange(mockCallback.Object.OnChange);

            for (var i = 1; i <= 10; i++)
            {
                mutableService.CurrentValue.Update();
                mockCallback.Verify(x => x.OnChange(It.IsAny<CustomMutableService>()), Times.Exactly(i));
            }
        }

        [Fact]
        public void AddedMutableAsInterfaceOnChangeHandlerIsCalledOnUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomMutableService, CustomMutableService>(() => new CustomMutableService(), service => service.GetChangeToken());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mutableService = serviceProvider.GetRequiredService<IMutableService<ICustomMutableService>>();

            var mockCallback = new Mock<ICallbackWrapper<ICustomMutableService>>(MockBehavior.Default);
            mutableService.OnChange(mockCallback.Object.OnChange);

            for (var i = 1; i <= 10; i++)
            {
                mutableService.CurrentValue.Update();
                mockCallback.Verify(x => x.OnChange(It.IsAny<ICustomMutableService>()), Times.Exactly(i));
            }
        }

        [Fact]
        public void MultipleMutableSubscribersReceiveUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable(() => new CustomMutableService(), service => service.GetChangeToken());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceBag = new List<(Mock<ICallbackWrapper<CustomMutableService>>, IDisposable)>();
            for(var i = 0; i < 10; i++)
            {
                var mockCallback = new Mock<ICallbackWrapper<CustomMutableService>>(MockBehavior.Default);
                serviceBag.Add((mockCallback, serviceProvider.GetRequiredService<IMutableService<CustomMutableService>>().OnChange(mockCallback.Object.OnChange)));
            }

            var mutableService = serviceProvider.GetRequiredService<IMutableService<CustomMutableService>>();
            mutableService.CurrentValue.Update();

            foreach(var tuple in serviceBag)
            {
                tuple.Item1.Verify(x => x.OnChange(It.IsAny<CustomMutableService>()), Times.Once());
            }
        }

        [Fact]
        public void MultipleMutableAsInterfaceSubscribersReceiveUpdate()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMutable<ICustomMutableService, CustomMutableService>(() => new CustomMutableService(), service => service.GetChangeToken());
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var serviceBag = new List<(Mock<ICallbackWrapper<ICustomMutableService>>, IDisposable)>();
            for (var i = 0; i < 10; i++)
            {
                var mockCallback = new Mock<ICallbackWrapper<ICustomMutableService>>(MockBehavior.Default);
                serviceBag.Add((mockCallback, serviceProvider.GetRequiredService<IMutableService<ICustomMutableService>>().OnChange(mockCallback.Object.OnChange)));
            }

            var mutableService = serviceProvider.GetRequiredService<IMutableService<ICustomMutableService>>();
            mutableService.CurrentValue.Update();

            foreach (var tuple in serviceBag)
            {
                tuple.Item1.Verify(x => x.OnChange(It.IsAny<ICustomMutableService>()), Times.Once());
            }
        }

        public interface ICallbackWrapper<TService>
        {
            void OnChange(TService service);
        }

        public interface ICustomNonMutableService { }

        public class CustomNonMutableService : ICustomNonMutableService
        {
        }

        public interface ICustomMutableService
        {
            void Update();
            IChangeToken GetChangeToken();
        }

        public class CustomMutableService : ICustomMutableService
        {
            private ServiceChangeToken _changeToken = new ServiceChangeToken();

            public IChangeToken GetChangeToken() => _changeToken;

            public void Update()
            {
                var previousToken = Interlocked.Exchange(ref _changeToken, new ServiceChangeToken());
                previousToken.OnReload();
            }
        }
    }
}
