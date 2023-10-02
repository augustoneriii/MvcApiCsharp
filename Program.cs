using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using projetoMvc.Models; // Certifique-se de adicionar este using.
using Swashbuckle.AspNetCore.Annotations;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMySql<ApplicationDbContext>(
    builder.Configuration["Database:MySql"],
    new MySqlServerVersion(new Version(8, 0, 21)),
    mySqlOptions => { });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.MapType<IEndpointConventionBuilder>(() => new OpenApiSchema { Type = "object" });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.ConfigProductRoutes(); 
app.ConfigTaskRoutes(); 
app.MapControllers();
app.Run();
