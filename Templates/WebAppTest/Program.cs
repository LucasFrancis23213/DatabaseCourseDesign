
using SQLOperation.PublicAccess.Templates.SQLManager;
using DatabaseProject.APILayer.CommunityFeatureAPI;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// 将服务添加到容器中
builder.Services.AddControllers();

// 配置 Kestrel 只使用 HTTP
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5174);  // 使用端口 5174
});

// 了解更多关于配置 Swagger/OpenAPI 的信息
// https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加数据库连接服务
builder.Services.AddSingleton<Connection>(new Connection("ADMIN", "123456", "121.36.200.128:1521/ORCL"));



// 添加 CORS 服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()  // 允许任何来源
                  .AllowAnyHeader()  // 允许任何头部
                  .AllowAnyMethod(); // 允许任何方法
        });
});

// 注册 WebSocketService
builder.Services.AddSingleton<WebSocketService>();

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

// 添加 WebSocket 支持
app.UseWebSockets();

// 自定义中间件处理 WebSocket 连接
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        var webSocketService = context.RequestServices.GetRequiredService<WebSocketService>();
        await webSocketService.HandleConnection(context);
    }
    else
    {
        await next();
    }
});

app.UseAuthorization();
app.MapControllers();


app.UseCors("AllowAllOrigins"); // 使用 CORS 策略

app.Run();
