using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CustomMiddlewareExtension
{
    public class HelloMiddleware
    {
        //Middleware için gerekli olan RequestDelegate inject edildi
        private readonly RequestDelegate _next;
        public HelloMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Invoke() metotu yazıldı
        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("Hello World!");
            await _next.Invoke(context);
            Console.WriteLine("Bye World!");
        }
    }

    public static class MiddlewareExtension
    {
        //Middleware startup.cs de app.UseHello() olarak çağırabilmek için Extension Metot haline getirdik.
        public static IApplicationBuilder UseHello(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HelloMiddleware>();
        }
    }
}