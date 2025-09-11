using Client.Storage;
using Client.Service;

var builder = WebApplication.CreateBuilder(args);

// 🔹 String de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 🔹 Registrar Repository e Service
builder.Services.AddSingleton(new ClientRepository(connectionString));
builder.Services.AddSingleton<ClientService>();

// 🔹 Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Swagger sempre ativo no Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
