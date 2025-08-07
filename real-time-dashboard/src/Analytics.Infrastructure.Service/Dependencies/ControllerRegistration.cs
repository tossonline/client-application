// Copyright (c) DigiOutsource. All rights reserved.

using System.Reflection;
using Affiliate.Platform.Extensions.Service.Configuration;
using Analytics.Application.BackgroundJobs.Builder;
using Analytics.Domain.Models.Configuration;

namespace Analytics.Infrastructure.Service.Dependencies
{
    public static class ControllerRegistration
    {
        public static void RegisterDependencies(IMvcBuilder builder, IServiceConfiguration configuration)
        {
            Assembly assembly = Assembly.GetExecutingAssembly() ?? throw new Exception("Assembly cannot be null.");
            builder.AddApplicationPart(assembly);
            
            BackgroundJobBuilder.RegisterJobs((IAnalyticsConfiguration)configuration);
        }
    }
}