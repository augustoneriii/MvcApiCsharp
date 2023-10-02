using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace projetoMvc.Models
{
    public static class RouteConfig
    {
        public static void ConfigureRoutes(this WebApplication app)
        {
            app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) =>
            {
                var category = context.Categories.FirstOrDefault(c => c.Id == productRequest.CategoryId);

                if (category != null)
                {
                    var product = new Product
                    {
                        Code = productRequest.Code,
                        Name = productRequest.Name,
                        Description = productRequest.Description,
                        Category = category
                    };

                    if (productRequest.Tags != null)
                    {
                        product.Tags = new List<Tag>();
                        foreach (var item in productRequest.Tags)
                        {
                            product.Tags.Add(new Tag { Name = item });
                        }
                    }

                    context.Products.Add(product);
                    context.SaveChanges();
                    return Results.Created($"/products/{product.Id}", product);
                }
                else
                {
                    // Lógica para lidar com o caso em que a categoria não foi encontrada
                    return Results.BadRequest("Categoria não encontrada");
                }
            }).WithMetadata(new HttpMethodMetadata(new[] { "POST" }), new RouteNameMetadata("CreateProduct"), new SwaggerOperationAttribute("CreateProduct"));

            app.MapGet("/products", (ApplicationDbContext context) =>
            {
                var products = context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Tags)
                    .ToList();

                return Results.Ok(products);
            }).WithMetadata(new HttpMethodMetadata(new[] { "GET" }), new RouteNameMetadata("GetAllProducts"), new SwaggerOperationAttribute("GetAllProducts"));

            app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
            {
                var product = context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Tags)
                    .Where(p => p.Id == id).FirstOrDefault();
                if (product != null)
                {
                    return Results.Ok(product);
                }
                return Results.NotFound();
            }).WithMetadata(new HttpMethodMetadata(new[] { "GET" }), new RouteNameMetadata("GetProduct"), new SwaggerOperationAttribute("GetProduct"));

            app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) =>
            {
                var product = context.Products
                    .Include(p => p.Tags)
                    .Where(p => p.Id == id).FirstOrDefault();
                var category = context.Categories.Where(c => c.Id == productRequest.CategoryId).First();

                product.Code = productRequest.Code;
                product.Name = productRequest.Name;
                product.Description = productRequest.Description;
                product.Category = category;
                product.Tags = new List<Tag>();
                if (productRequest.Tags != null)
                {
                    product.Tags = new List<Tag>();
                    foreach (var item in productRequest.Tags)
                    {
                        product.Tags.Add(new Tag { Name = item });
                    }
                }
                context.SaveChanges();
                return Results.Ok();
            }).WithMetadata(new HttpMethodMetadata(new[] { "PUT" }), new RouteNameMetadata("UpdateProduct"), new SwaggerOperationAttribute("UpdateProduct"));

            app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) =>
            {
                var product = context.Products.Where(p => p.Id == id).FirstOrDefault();
                if (product != null)
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                    return Results.Ok();
                }
                return Results.NotFound();
            }).WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }), new RouteNameMetadata("DeleteProduct"), new SwaggerOperationAttribute("DeleteProduct"));
        }
    }
}