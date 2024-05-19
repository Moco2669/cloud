﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminToolsConsoleApp
{
    /// <summary>
    /// Represents a generic service connector for connecting to a service.
    /// </summary>
    /// <typeparam name="TService">The service interface type.</typeparam>
    public interface IServiceConnector<TService>
    {
        /// <summary>
        /// Connects to the specified service.
        /// </summary>
        /// <param name="serviceAddress">The address of the service.</param>
        void Connect(string serviceAddress);

        /// <summary>
        /// Gets the proxy instance for the connected service.
        /// </summary>
        /// <returns>The proxy instance for the service.</returns>
        TService GetProxy();
    }
}
