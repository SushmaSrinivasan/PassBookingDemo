using PassbokningsDemo.Data;
using PassBookingDemo.Data;

namespace PassBookingDemo.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<ApplicationBuilder> SeedDataAsync(this ApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope()) 
            {
                var services= scope.ServiceProvider;
                var context=services.GetRequiredService<ApplicationDbContext>();

                try
                {
                    await SeedData.InitAsync(context, services);
                }
                catch(Exception )
                {
                    throw;
                }
            }
            return app;
        }
    }
}
