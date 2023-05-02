using BlazorSozluk.Api.Application.Extensions;
using BlazorSozluk.Infrastructure.Persistence.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    //Bu kod, JSON verilerini serialize ederken veya deserialize ederken, �zellik isimlerinin nas�l bi�imlendirilece�ini ayarlar PropertyNamingPolicy, JSON'da bir �zellik ad� olarak kullan�lan C# nesne �zelliklerinin adland�r�lmas�n� ayarlamak i�in kullan�l�r. Burada PropertyNamingPolicy null olarak ayarlanarak, nesne �zellik isimleri do�rudan JSON'daki �zellik adlar�na e�le�tirilir, yani hi�bir bi�imlendirme yap�lmaz.
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    })
    .AddFluentValidation();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApplicationRegistration();
builder.Services.AddInfrastructureRegistration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
