using Microsoft.Extensions.FileProviders;
using TalentBridge.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
var app = builder.Build();

var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var uploadsPath = Path.Combine(wwwrootPath, "uploads");
Directory.CreateDirectory(Path.Combine(uploadsPath, "cv"));
Directory.CreateDirectory(Path.Combine(uploadsPath, "avatars"));
Directory.CreateDirectory(Path.Combine(uploadsPath, "organizations"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");  
app.UseHttpsRedirection();

// Serve static files from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(wwwrootPath),
    RequestPath = ""
});

app.UseAuthentication();           
app.UseAuthorization();
app.MapControllers();

app.Run();