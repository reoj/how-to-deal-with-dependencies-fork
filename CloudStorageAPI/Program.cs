using CloudStorage.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.RegisterCloudStorageCore(String.IsNullOrEmpty(builder.Configuration.GetConnectionString("Db")) ?
                                            "": builder.Configuration.GetConnectionString("Db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();
