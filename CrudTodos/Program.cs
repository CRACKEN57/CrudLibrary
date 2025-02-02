using CrudTodos.Business.Interfaces;
using CrudTodos.Business.Repositories;
using CrudTodos.Data;
using CrudTodos.Presentations.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// DbContext para conectarnos a SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar Repositorios (Inyección de dependencias)
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => options.DetailedErrors = true);




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
