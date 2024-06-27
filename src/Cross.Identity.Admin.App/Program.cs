using Juice.Modular;

var builder = WebApplication.CreateBuilder(args);

builder.AddDiscoveredModules();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowKnownOrigins");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.ConfigureDiscoverdModules(app.Environment);

await app.RunAsync();

