using HRApplication;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(opt => { opt.ResourcesPath = "Recourses"; });
builder.Services.AddMvc()
.AddDataAnnotationsLocalization().AddViewLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultres = new List<CultureInfo> {
        new CultureInfo("en-US"),
        new CultureInfo("ar-EG")
    };
    options.DefaultRequestCulture = new RequestCulture("ar-EG");
    options.SupportedCultures = supportedCultres;
    options.SupportedUICultures = supportedCultres;
});

// Add services to the container.
builder.Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization(options => {
    var type = typeof(SharedResource);
    var assemplyName = new AssemblyName(type.Assembly.FullName);
    var factory = builder.Services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
    var localizer = factory.Create("SharedResource", assemplyName.Name);
    options.DataAnnotationLocalizerProvider = (t, f) => localizer;
}); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

var supportedCulture = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("ar-EG")
};
var options = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();

app.UseRequestLocalization(options.Value);


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
