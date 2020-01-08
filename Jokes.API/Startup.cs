
using AutoMapper;
using Jokes.Data.Memory;
using Jokes.Entities;
using Jokes.Interfaces;
using Jokes.Models;
using Jokes.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace Jokes.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddDbContext<JokesContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                //options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                //options.ConfigureWarnings(x => x.Ignore(RelationalEventId.));
            });

            services.AddSingleton(_ => Configuration);

            services.AddScoped<IRepoFactory, RepoFactory>((_) => new RepoFactory());
            //services.AddLogging();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Jokes Web API",
                    Description = "BorrowWorks Jokes Web API",
                    TermsOfService = "This is serious stuff",
                    Contact = new Contact
                    {
                        Name = "Chris Noblett",
                        Email = "cknobman@gmail.com",
                        Url = ""
                    }
                });

                //c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                //{
                //    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                //    Name = "Authorization",
                //    In = "header",
                //    Type = "apiKey"
                //});

                //c.DocumentFilter<Helpers.SecurityRequirementsDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);

                c.CustomSchemaIds(i => i.FullName);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        //.AllowAnyOrigin()
                        .WithOrigins("http://localhost:8080",
                            "http://localhost:3000",
                            "https://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            var mappingConfiguration = new AutoMapper.MapperConfiguration(mc =>
            {
                mc.CreateMap<Joke, JokeModel>().ReverseMap();
                mc.CreateMap<JokeType, JokeTypeModel>().ReverseMap();
            });

            IMapper mapper = mappingConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddMvcOptions(o =>
            {
                //o.EnableEndpointRouting = false;
            });

            services.AddApiVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BorrowWorks Jokes API");
            });

            //app.UseRouting();

            //app.UseAuthorization();

            app.UseAuthentication();
            app.UseCors("AllowAll");

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseMvc();
        }
    }
}
