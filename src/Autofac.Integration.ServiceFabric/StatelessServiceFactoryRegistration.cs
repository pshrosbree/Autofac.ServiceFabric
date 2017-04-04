﻿// This software is part of the Autofac IoC container
// Copyright © 2017 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Reflection;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Autofac.Integration.ServiceFabric
{
    internal sealed class StatelessServiceFactoryRegistration : IStatelessServiceFactoryRegistration
    {
        public void RegisterStatelessServiceFactory(ILifetimeScope lifetimeScope, Type serviceType, string serviceTypeName)
        {
            var factoryMethod = typeof(StatelessServiceFactoryRegistration)
                .GetMethod(nameof(RegisterFactoryWithServiceRuntime), BindingFlags.NonPublic | BindingFlags.Static);

            var genericFactoryMethod = factoryMethod.MakeGenericMethod(serviceType);

            genericFactoryMethod.Invoke(null, new object[] {lifetimeScope, serviceTypeName});
        }

        private static void RegisterFactoryWithServiceRuntime<TService>(ILifetimeScope container, string serviceTypeName)
            where TService : StatelessService
        {
            ServiceRuntime.RegisterServiceAsync(serviceTypeName, context =>
            {
                var lifetimeScope = container.BeginLifetimeScope();
                var service = lifetimeScope.Resolve<TService>(TypedParameter.From(context));
                return service;
            }).GetAwaiter().GetResult();
        }
    }
}
