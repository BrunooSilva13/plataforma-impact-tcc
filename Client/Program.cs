using Client.Middleware;
using Client.Repository;
using Client.Service;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar Kestrel para ouvir em qualquer IP na porta 5000 (mesma do Docker)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// 🔹 Adicionar serviços ao container
builder.Services.AddControllers();

// 🔹 Registro da injeção de dependência
builder.Services.AddScoped<ClientRepository>();  // Acesso ao banco
builder.Services.AddScoped<ClientService>();     // Regras de negócio

// 🔹 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Middleware global de erros
app.UseMiddleware<ExceptionMiddleware>();

// 🔹 Swagger sempre habilitado (mesmo em Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Client API V1");
    c.RoutePrefix = string.Empty; // permite acessar direto em http://localhost:5000/
});

// 🔹 Redirecionamento HTTPS (opcional dentro do Docker)
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
