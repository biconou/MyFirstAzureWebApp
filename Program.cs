var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Ajouter HttpClient pour effectuer les appels HTTP
builder.Services.AddHttpClient("ProxyApi", client =>
{
    client.BaseAddress = new Uri("https://api.exemple.com"); // Remplacez par l'URL de l'API cible
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
