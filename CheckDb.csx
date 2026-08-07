using System;
using System.Linq;
using Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseNpgsql(""Host=localhost;Database=MesaFacil;Username=postgres;Password=postgres"")
    .Options;

using var db = new ApplicationDbContext(options);
var estados = db.Set<Domain.Entities.CatEstadoMesa>().ToList();
foreach(var e in estados) {
    Console.WriteLine($""ID: {e.Id} - {e.Descripcion}"");
}
