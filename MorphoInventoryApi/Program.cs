using MorphoInventoryApi.Data;
using MorphoInventoryApi.Repositories;
using MorphoInventoryApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new DatabaseContext(connectionString));

// Register repositories
builder.Services.AddScoped<DeviceRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<AssignmentRepository>();
builder.Services.AddScoped<ReturnRepository>();

// Register services
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<ReturnService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

