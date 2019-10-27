namespace Novellus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Novellus.Views;
    using Xamarin.Forms;
    using Xamarin.Forms.Internals;

    public partial class App : Application
    {
        private static readonly IServiceCollection Services = new ServiceCollection();

        public App()
        {
            this.InitializeComponent();

            this.MainPage = new MainPage();
        }

        public static void BuildSeriveProvider()
        {
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            DependencyResolver.ResolveUsing(type =>
            {
                return serviceProvider.GetService(type);
            });
        }

        public static void ClearRegisteredTypes()
        {
            Services.Clear();
        }

        public static void RegisterType<T>(ServiceLifetime lifetime = ServiceLifetime.Transient) where T : class
        {
            RegisterTypeImpl<T>(lifetime, null);
        }

        public static void RegisterType<T>(SortedDictionary<Type, object> parameters, ServiceLifetime lifetime = ServiceLifetime.Transient) where T : class
        {
            T implementationFactory(IServiceProvider provider)
            {
                var argTypes = parameters.Keys.ToArray();
                var ctorInfo = typeof(T).GetConstructor(argTypes);
                if (ctorInfo is null)
                {
                    throw new ArgumentException($"no {typeof(T).FullName}.ctor({string.Join(", ", argTypes.Select(t => t.FullName))}) found", nameof(parameters));
                }

                return (T)ctorInfo.Invoke(parameters.Values.ToArray());
            }

            RegisterTypeImpl<T>(lifetime, implementationFactory);
        }

        public static void RegisterType<TIService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient) where TIService : class where TImplementation : class, TIService
        {
            RegisterTypeImpl<TIService, TImplementation>(lifetime, null);
        }

        public static void RegisterType<TIService, TImplementation>(SortedDictionary<Type, object> parameters, ServiceLifetime lifetime = ServiceLifetime.Transient) where TIService : class where TImplementation : class, TIService
        {
            TImplementation implementationFactory(IServiceProvider provider)
            {
                var argTypes = parameters.Keys.ToArray();
                var ctorInfo = typeof(TImplementation).GetConstructor(argTypes);
                if (ctorInfo is null)
                {
                    throw new ArgumentException($"no {typeof(TImplementation).FullName}.ctor({string.Join(", ", argTypes.Select(t => t.FullName))}) found", nameof(parameters));
                }

                return (TImplementation)ctorInfo.Invoke(parameters.Values.ToArray());
            }

            RegisterTypeImpl<TIService, TImplementation>(lifetime, implementationFactory);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private static void RegisterTypeImpl<T>(ServiceLifetime lifetime, Func<IServiceProvider, T> implementationFactory) where T : class
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    _ = implementationFactory is null ? Services.AddSingleton<T>() : Services.AddSingleton<T>(implementationFactory);
                    break;
                case ServiceLifetime.Scoped:
                    _ = implementationFactory is null ? Services.AddScoped<T>() : Services.AddScoped<T>(implementationFactory);
                    break;
                case ServiceLifetime.Transient:
                default:
                    _ = implementationFactory is null ? Services.AddTransient<T>() : Services.AddTransient<T>(implementationFactory);
                    break;
            }
        }

        private static void RegisterTypeImpl<TIService, TImplementation>(ServiceLifetime lifetime, Func<IServiceProvider, TImplementation> implementationFactory) where TIService : class where TImplementation : class, TIService
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    _ = implementationFactory is null ? Services.AddSingleton<TIService, TImplementation>() : Services.AddSingleton<TIService, TImplementation>(implementationFactory);
                    break;
                case ServiceLifetime.Scoped:
                    _ = implementationFactory is null ? Services.AddScoped<TIService, TImplementation>() : Services.AddScoped<TIService, TImplementation>(implementationFactory);
                    break;
                case ServiceLifetime.Transient:
                default:
                    _ = implementationFactory is null ? Services.AddTransient<TIService, TImplementation>() : Services.AddTransient<TIService, TImplementation>(implementationFactory);
                    break;
            }
        }
    }
}
