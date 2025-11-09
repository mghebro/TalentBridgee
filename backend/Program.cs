using TalentBridge.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");  
app.UseHttpsRedirection();
app.UseAuthentication();           
app.UseAuthorization();
app.MapControllers();

app.Run();