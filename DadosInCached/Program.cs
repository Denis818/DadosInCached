using DadosInCached.Context;
using DadosInCached.Interfaces;
using DadosInCached.Models;
using DadosInCached.Models.Dtos;
using DadosInCached.Repository;
using Microsoft.EntityFrameworkCore;
using ProEventos.API.Configuration.Middleware;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryBase<Produto>, RepositoryBase<Produto>>();

builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<MiddlewareException>();
app.MapControllers();
app.Run();