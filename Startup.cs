using System;
using CustomMiddlewareExtension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace dotnet_middleware
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "dotnet_middleware", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dotnet_middleware v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            //app.Run() Kısa devre yaptıran Middleware. app.Run() 'dan sonra gelen middleware'ler çalışmaz.
            // app.Run(async context => Console.WriteLine("1. Middleware çalışıyor"));
            // app.Run(async context => Console.WriteLine("1. Middleware çalışıyor"));


            //app.Use() Parametreden HttpContext ve bir metot alır.
            // 1,2 ve 3. Middleware çalışır. await ettiğim için sonlandırma yazısını göremeyiz.
            // Ardından sondan başa dönerek bekleyen işlemleri yapar. 3,2 ve 1. Middleeware'ler sırasıyla sonlanır.

            // app.Use(async (context, next) =>
            // {
            //     Console.WriteLine("Middleware 1 çalıştı");
            //     await next.Invoke();
            //     Console.WriteLine("Middleware 1 sonlandı");
            // });

            // app.Use(async (context, next) =>
            // {
            //     Console.WriteLine("Middleware 2 çalıştı");
            //     await next.Invoke();
            //     Console.WriteLine("Middleware 2 sonlandı");
            // });
            // app.Use(async (context, next) =>
            // {
            //     Console.WriteLine("Middleware 3 çalıştı");
            //     await next.Invoke();
            //     Console.WriteLine("Middleware 3 sonlandı");
            // });

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Use Middleware tetilklendi");
                await next.Invoke();
                System.Console.WriteLine("Use Middleware sonlandı");
            });

            //app.Map()
            // Verilen route göre hareket eder. Eğer https://localhost:5001/example path'ine request atılırsa bu middleware çalışır.  
            app.Map("/example", internalApp =>
            internalApp.Run(async context =>
            {
                Console.WriteLine("/example Map middleware tetiklendi");
                await context.Response.WriteAsync("/example Map middleware tetiklendi response goruntuleniyor.");
            }));


            // custom middleware
            app.UseHello();



            //app.MapWhen()
            // MapWhen() middleware ile Request , Response , Session gibi yapılara müdahale edilebilir.

            app.MapWhen(x => x.Request.Method == "GET", internalApp =>
               {
                   internalApp.Run(async context =>
                   {
                       Console.WriteLine("Request GET metotu olduğu için MapWhen Middleware'i tetiklendi");
                       await context.Response.WriteAsync("Request GET metotu olduğu için MapWhen Middleware'i tetiklendi");
                   });

               });


            


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            
        }
    }
}
