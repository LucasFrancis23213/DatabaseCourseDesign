using DatabaseProject.ServiceLayer.ConmmunityFeature;
using SQLOperation.PublicAccess.Templates.SQLManager;
using DatabaseProject.APILayer.CommunityFeatureAPI;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at
https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 添加connection服务
builder.Services.AddSingleton<Connection>(new Connection("ADMIN", "123456", "121.36.200.128:1521/ORCL"));

builder.Services.AddSignalR();


// 添加CORS服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
    builder =>
    {
        builder.WithOrigins("http://localhost:5173")
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
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();


// 配置 SignalR 路由
app.MapHub<ChatHub>("/chathub");
app.UseCors("AllowSpecificOrigin"); // Use the CORS policy

app.Run();

