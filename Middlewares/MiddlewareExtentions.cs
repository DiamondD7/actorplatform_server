namespace UserAPI.Middlewares
{
    public static class MiddlewareExtentions
    {
        public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TokenMiddleware>();
        }
    }
}
