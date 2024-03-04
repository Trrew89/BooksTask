using Books_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add BookService to the DI container
builder.Services.AddSingleton<BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseCors(options => options.WithOrigins("http://127.0.0.1:5500/") 
                              .AllowAnyMethod() 
                              .AllowAnyHeader()
                              .AllowAnyOrigin());

app.MapControllers();

app.Run();


