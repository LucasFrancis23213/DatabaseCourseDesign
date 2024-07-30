using DatabaseProject.ServiceLayer.ConmmunityFeature;
using SQLOperation.PublicAccess.Templates.SQLManager;
using DatabaseProject.APILayer.CommunityFeatureAPI;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// 配置 Kestrel 只使用 HTTP
builder.WebHost.ConfigureKestrel(serverOptions =>
{
serverOptions.Listen(IPAddress.Any, 5000);  // 使用端口 5000，您可以根据需要更改
});
// Learn more about configuring Swagger/OpenAPI at
https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 添加connection服务
builder.Services.AddSingleton<Connection>(new Connection("BAMBOO", "123456", "localhost:1521/ORCL"));

builder.Services.AddSignalR();


// 添加CORS服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();


// 配置 SignalR 路由
app.MapHub<ChatHub>("/chathub");
app.UseCors("AllowSpecificOrigin"); // Use the CORS policy

app.Run();