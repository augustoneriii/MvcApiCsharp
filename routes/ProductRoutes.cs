using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace projetoMvc.Models
{
    public static class ProductRoutes
    {
        public static void ConfigProductRoutes(this WebApplication app)
        {
            app.MapPost("/produtos", (ProductRequest productRequest, ApplicationDbContext context) =>
            {
                var categoria = context.Categories.FirstOrDefault(c => c.Id == productRequest.CategoryId);

                if (categoria != null)
                {
                    var produto = new Product
                    {
                        Code = productRequest.Code,
                        Name = productRequest.Name,
                        Description = productRequest.Description,
                        Category = categoria
                    };

                    if (productRequest.Tags != null)
                    {
                        produto.Tags = new List<Tag>();
                        foreach (var item in productRequest.Tags)
                        {
                            produto.Tags.Add(new Tag { Name = item });
                        }
                    }

                    context.Products.Add(produto);
                    context.SaveChanges();
                    return Results.Created($"/produtos/{produto.Id}", produto);
                }
                else
                {
                    // Lógica para lidar com o caso em que a categoria não foi encontrada
                    return Results.BadRequest(new { message = "Categoria não encontrada" });
                }
            }).WithMetadata(new HttpMethodMetadata(new[] { "POST" }), new RouteNameMetadata("CriarProduto"), new SwaggerOperationAttribute("CriarProduto"));

            app.MapGet("/produtos", (ApplicationDbContext context) =>
            {
                var produtos = context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Tags)
                    .ToList();

                return Results.Ok(produtos);
            }).WithMetadata(new HttpMethodMetadata(new[] { "GET" }), new RouteNameMetadata("ObterTodosProdutos"), new SwaggerOperationAttribute("ObterTodosProdutos"));

            app.MapGet("/produtos/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
            {
                var produto = context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Tags)
                    .Where(p => p.Id == id).FirstOrDefault();
                if (produto != null)
                {
                    return Results.Ok(produto);
                }
                return Results.NotFound(new { message = "Produto não encontrado" });
            }).WithMetadata(new HttpMethodMetadata(new[] { "GET" }), new RouteNameMetadata("ObterProduto"), new SwaggerOperationAttribute("ObterProduto"));

            app.MapPut("/produtos/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) =>
            {
                var produto = context.Products
                    .Include(p => p.Tags)
                    .Where(p => p.Id == id).FirstOrDefault();
                var categoria = context.Categories.Where(c => c.Id == productRequest.CategoryId).FirstOrDefault();

                if (produto != null && categoria != null)
                {
                    produto.Code = productRequest.Code;
                    produto.Name = productRequest.Name;
                    produto.Description = productRequest.Description;
                    produto.Category = categoria;
                    produto.Tags = new List<Tag>();
                    if (productRequest.Tags != null)
                    {
                        produto.Tags = new List<Tag>();
                        foreach (var item in productRequest.Tags)
                        {
                            produto.Tags.Add(new Tag { Name = item });
                        }
                    }
                    context.SaveChanges();
                    return Results.Ok(new { message = "Produto atualizado com sucesso" });
                }
                else
                {
                    // Lógica para lidar com o caso em que o produto ou categoria não foi encontrado
                    return Results.NotFound(new { message = "Produto ou categoria não encontrados" });
                }
            }).WithMetadata(new HttpMethodMetadata(new[] { "PUT" }), new RouteNameMetadata("AtualizarProduto"), new SwaggerOperationAttribute("AtualizarProduto"));

            app.MapDelete("/produtos/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
            {
                var produto = context.Products.Where(p => p.Id == id).FirstOrDefault();
                if (produto != null)
                {
                    context.Products.Remove(produto);
                    context.SaveChanges();
                    return Results.Ok(new { message = "Produto excluído com sucesso" });
                }
                return Results.NotFound(new { message = "Produto não encontrado" });
            }).WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }), new RouteNameMetadata("ExcluirProduto"), new SwaggerOperationAttribute("ExcluirProduto"));
        }
    }
}
