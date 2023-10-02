using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace projetoMvc.Models
{
    public static class TasksRoutes
    {
        public static void ConfigTaskRoutes(this WebApplication app)
        {
            app.MapPost("/tarefas", (TaskRequest taskRequest, ApplicationDbContext context) =>
            {
                var tarefa = new Task
                {
                    Title = taskRequest.Title,
                    Description = taskRequest.Description
                };
                context.Tasks.Add(tarefa);
                context.SaveChanges();

                var response = new
                {
                    message = "Tarefa criada com sucesso",
                    tarefa
                };

                return Results.Created($"/tarefas/{tarefa.Id}", response);
            }).WithMetadata(new HttpMethodMetadata(new[] { "POST" }), new RouteNameMetadata("CriarTarefa"), new SwaggerOperationAttribute("CriarTarefa"));

            app.MapGet("/tarefas", (ApplicationDbContext context) =>
            {
                var tarefas = context.Tasks.ToList();
                return Results.Ok(tarefas);
            }).WithMetadata(new HttpMethodMetadata(new[] { "GET" }), new RouteNameMetadata("ObterTodasTarefas"), new SwaggerOperationAttribute("ObterTodasTarefas"));

            app.MapGet("/tarefas/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
            {
                var tarefa = context.Tasks.Where(t => t.Id == id).FirstOrDefault();
                if (tarefa != null)
                {
                    return Results.Ok(tarefa);
                }
                return Results.NotFound(new { message = "Tarefa não encontrada" });
            }).WithMetadata(new HttpMethodMetadata(new[] { "GET" }), new RouteNameMetadata("ObterTarefa"), new SwaggerOperationAttribute("ObterTarefa"));

            app.MapPut("/tarefas/{id}", ([FromRoute] int id, TaskRequest taskRequest, ApplicationDbContext context) =>
            {
                var tarefa = context.Tasks.Where(t => t.Id == id).FirstOrDefault();

                if (tarefa == null)
                {
                    return Results.NotFound(new { message = "Tarefa não encontrada" });
                }

                tarefa.Title = taskRequest.Title;
                tarefa.Description = taskRequest.Description;

                context.SaveChanges();

                var response = new
                {
                    message = "Tarefa atualizada com sucesso",
                    tarefa
                };

                return Results.Ok(response);
            }).WithMetadata(new HttpMethodMetadata(new[] { "PUT" }), new RouteNameMetadata("AtualizarTarefa"), new SwaggerOperationAttribute("AtualizarTarefa"));

            app.MapDelete("/tarefas/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
            {
                var tarefa = context.Tasks.Where(t => t.Id == id).FirstOrDefault();
                if (tarefa != null)
                {
                    context.Tasks.Remove(tarefa);
                    context.SaveChanges();
                    return Results.Ok(new { message = "Tarefa excluída com sucesso" });
                }
                return Results.NotFound(new { message = "Tarefa não encontrada" });
            }).WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }), new RouteNameMetadata("ExcluirTarefa"), new SwaggerOperationAttribute("ExcluirTarefa"));
        }
    }
}
