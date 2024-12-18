var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddHttpClient("TEACoreProxy", client =>
{
    //client.BaseAddress = new Uri("https://glatech-myfirstazurewebapp.azurewebsites.net/");
    client.BaseAddress = new Uri("http://localhost:5016/");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Ajouter Application Insights
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Utiliser le routage basé sur les contrôleurs
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RequestLoggerMiddleware>();

app.Run();
