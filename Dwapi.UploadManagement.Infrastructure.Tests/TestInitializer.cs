using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Dapper;
using Dwapi.ExtractsManagement.Core.Interfaces.Repository;
using Dwapi.ExtractsManagement.Core.Interfaces.Services;
using Dwapi.ExtractsManagement.Core.Services;
using Dwapi.ExtractsManagement.Infrastructure;
using Dwapi.ExtractsManagement.Infrastructure.Repository;
using Dwapi.SettingsManagement.Core.Interfaces.Repositories;
using Dwapi.SettingsManagement.Infrastructure;
using Dwapi.SettingsManagement.Infrastructure.Repository;
using Dwapi.SharedKernel.Infrastructure.Tests.TestHelpers;
using Dwapi.SharedKernel.Utility;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Cbs;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Packager.Hts;
using Dwapi.UploadManagement.Core.Interfaces.Reader;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Cbs;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Reader.Hts;
using Dwapi.UploadManagement.Core.Interfaces.Services.Cbs;
using Dwapi.UploadManagement.Core.Interfaces.Services.Dwh;
using Dwapi.UploadManagement.Core.Interfaces.Services.Hts;
using Dwapi.UploadManagement.Core.Packager.Cbs;
using Dwapi.UploadManagement.Core.Packager.Dwh;
using Dwapi.UploadManagement.Core.Packager.Hts;
using Dwapi.UploadManagement.Core.Services.Cbs;
using Dwapi.UploadManagement.Core.Services.Dwh;
using Dwapi.UploadManagement.Core.Services.Hts;
using Dwapi.UploadManagement.Infrastructure.Data;
using Dwapi.UploadManagement.Infrastructure.Reader;
using Dwapi.UploadManagement.Infrastructure.Reader.Cbs;
using Dwapi.UploadManagement.Infrastructure.Reader.Dwh;
using Dwapi.UploadManagement.Infrastructure.Reader.Hts;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog;

namespace Dwapi.UploadManagement.Core.Tests
{
    [SetUpFixture]
    public class TestInitializer
    {
        public static IServiceProvider ServiceProvider;
        public static IServiceCollection Services;
        public static string ConnectionString;
        public static IConfigurationRoot Configuration;

        [OneTimeSetUp]
        public void Init()
        {
            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(new NumericTypeHandler());
            RemoveTestsFilesDbs();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var dir = $"{TestContext.CurrentContext.TestDirectory.HasToEndsWith(@"/")}";

            var config = Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DwapiConnection");
            // ConnectionString = connectionString.ToOsStyle();
            //var connection = new SqliteConnection(connectionString.Replace(".db", $"{DateTime.Now.Ticks}.db"));
            //connection.Open();

            var services = new ServiceCollection();

            services.
            AddDbContext<SettingsContext>(
                o => {
                    o.UseSqlServer(connectionString,
                        x => x.MigrationsAssembly(typeof(SettingsContext).GetTypeInfo().Assembly.GetName().Name));
                    o.EnableSensitiveDataLogging();


                }

            );
            services.AddDbContext<ExtractsContext>(o =>
            {
                o.UseSqlServer(connectionString,
                    x => x.MigrationsAssembly(typeof(ExtractsContext).GetTypeInfo().Assembly.GetName().Name));
                o.EnableSensitiveDataLogging();
            });
            services.AddDbContext<UploadContext>(o =>
                {
                    o.UseSqlServer(connectionString,
                        x => x.MigrationsAssembly(typeof(UploadContext).GetTypeInfo().Assembly.GetName().Name));
                    o.EnableSensitiveDataLogging();
                })


                .AddTransient<IDwhExtractReader, DwhExtractReader>()
                .AddTransient<IDwhPackager, DwhPackager>()
                .AddTransient<ICTPackager, CTPackager>()
                .AddTransient<ICTSendService, CTSendService>()
                .AddTransient<ICbsSendService,CbsSendService>()
                .AddTransient<ICbsPackager, CbsPackager>()
                .AddTransient<ICbsExtractReader, CbsExtractReader>()
                .AddTransient<IHtsSendService,HtsSendService>()
                .AddTransient<IHtsPackager, HtsPackager>()
                .AddTransient<IEmrMetricReader, EmrMetricReader>()
                .AddTransient<IHtsExtractReader, HtsExtractReader>()
                .AddTransient<IExtractStatusService,ExtractStatusService>()
                .AddTransient<IExtractHistoryRepository,ExtractHistoryRepository>()
                .AddTransient<IEmrSystemRepository,EmrSystemRepository>()
                .AddTransient<IDwhExtractReader, DwhExtractReader>();


            Services = services;

            ServiceProvider = Services.BuildServiceProvider();

         }

        public static void ClearDb()
        {
            var context = ServiceProvider.GetService<SettingsContext>();
           // context.Database.EnsureCreated();
            context.Database.Migrate();
            context.EnsureSeeded();

            var contextA = ServiceProvider.GetService<ExtractsContext>();
           // contextA.Database.EnsureCreated();
            contextA.Database.Migrate();
            contextA.EnsureSeeded();
        }


        public static void SeedData(params IEnumerable<object>[] entities)
        {
            var context = ServiceProvider.GetService<ExtractsContext>();

            foreach (IEnumerable<object> t in entities)
            {
                context.AddRange(t);
            }

            context.SaveChanges();
        }

        private void RemoveTestsFilesDbs()
        {
            string[] keyFiles ={ "dwapitest.db", "dwapitestdata.db"};
            string[] keyDirs = { @"Database".ToOsStyle()};

            foreach (var keyDir in keyDirs)
            {
                DirectoryInfo di = new DirectoryInfo(keyDir);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (!keyFiles.Contains(file.Name))
                        file.Delete();
                }
            }
        }
    }
}
