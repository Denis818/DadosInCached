using DadosInCached.Configurations.Middleware;
using DadosInCached.Configurations.Extensions;
using Data.Configurations.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiDependencyServices();
builder.Services.AddAppDbContext(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<MiddlewareException>();
app.MapControllers();
app.Run();