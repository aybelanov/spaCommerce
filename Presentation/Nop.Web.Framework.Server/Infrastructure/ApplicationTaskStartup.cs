using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Web.Framework.Server.Infrastructure
{
    //TODO added
    /// <summary>
    /// Starting of the task's executing. It has to be done in the latest step after all steps of the application's configuration.
    /// </summary>
    public static class ApplicationTaskStartup //: INopStartup
    {

        public static void Run()
        {
            if (DataSettingsManager.DatabaseIsInstalled)
            {
                //implement schedule tasks
                //database is already installed, so start scheduled tasks
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();

                //log application start
                EngineContext.Current.Resolve<ILogger>().Information("Application started", null, null);
            }
        }

        /// <summary>
        /// The latest startup
        /// </summary>
        //public int Order => int.MaxValue;

        //public void Configure(IApplicationBuilder application)
        //{
            
        //}

        //public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        //{
            
        //}
    }
}
