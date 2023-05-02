using BlazorSozluk.Api.Application.Extensions;
using BlazorSozluk.Infrastructure.Persistence.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    //Bu kod, JSON verilerini serialize ederken veya deserialize ederken, özellik isimlerinin nasýl biçimlendirileceðini ayarlar PropertyNamingPolicy, JSON'da bir özellik adý olarak kullanýlan C# nesne özelliklerinin adlandýrýlmasýný ayarlamak için kullanýlýr. Burada PropertyNamingPolicy null olarak ayarlanarak, nesne özellik isimleri doðrudan JSON'daki özellik adlarýna eþleþtirilir, yani hiçbir biçimlendirme yapýlmaz.
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
