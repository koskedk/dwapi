using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using AutoMapper;
using Dapper;
using FluentValidation;
using Linda.Registry.Core.Application.Identification.Commands;
using Linda.Registry.Core.Application.Identification.Validators;
using Linda.Registry.Core.Interfaces.Repositories;
using Linda.Registry.Core.Model.Profiles.Dtos;
using Linda.Registry.Infrastructure;
using Linda.Registry.Infrastructure.Repository;
using Linda.SharedKernel.Application.Behaviors;
using Linda.SharedKernel.Infrastructure.Tests;
using Linda.SharedKernel.Utility;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog;

namespace Linda.Registry.Core.Tests
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

            var dir = $"{TestContext.CurrentContext.TestDirectory.HasToEndWith(@"/")}";

            var config = Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("liveConnection")
                .Replace("#dir#", dir);
            ConnectionString = connectionString.ToOsStyle();
            var connection = new SqliteConnection(connectionString.Replace(".db", $"{DateTime.Now.Ticks}.db"));
            connection.Open();

            var services = new ServiceCollection().AddDbContext<RegistryContext>(x => x.UseSqlite(connection));

            services
                .AddTransient<RegistryContext>()
                .AddTransient<IIdentifierRepository, IdentifierRepository>()
                .AddTransient<IPersonRepository, PersonRepository>()
                .AddTransient<IPersonIdentifierRepository, PersonIdentifierRepository>()
                .AddTransient<IPersonRelationRepository, PersonRelationRepository>()
                .AddTransient<IChildRepository, ChildRepository>()
                .AddTransient<IGuardianRepository, GuardianRepository>()
                .AddTransient<IParentRepository, ParentRepository>()
                .AddTransient<IContactProfileRepository, ContactProfileRepository>()
                .AddTransient<IEducationProfileRepository, EducationProfileRepository>()
                .AddTransient<IRelationRepository, RelationRepository>()
                .AddTransient<IPersonStateHistoryRepository, PersonStateHistoryRepository>()
                .AddMediatR(typeof(SaveIdentifierCommand).Assembly)
                .AddValidatorsFromAssemblyContaining<SaveIdentifierCommandValidator>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            Services = services;

            ServiceProvider = Services.BuildServiceProvider();

           Mapper.Initialize(cfg => { cfg.AddProfile<PersonProfileMap>(); });
        }

        public static void ClearDb()
        {
            var context = ServiceProvider.GetService<RegistryContext>();
            context.Database.EnsureCreated();
            context.EnsureSeeded();
        }


        public static void SeedData(params IEnumerable<object>[] entities)
        {
            var context = ServiceProvider.GetService<RegistryContext>();

            foreach (IEnumerable<object> t in entities)
            {
                context.AddRange(t);
            }

            context.SaveChanges();
        }

        private void RemoveTestsFilesDbs()
        {
            string[] keyFiles ={ "lindatest.db", "lindatestdata.db"};
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
