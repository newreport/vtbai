
#region init path/file
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static Model.ConfigModel;

Console.Title = "live_tts_chatgpt";
FileHelper.CreatePath("data/models");
FileHelper.CreatePath("data/output");
#endregion

#region api conifg
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // 允许跨域
                          policy.AllowAnyOrigin();
                          //policy.WithOrigins("http://0.0.0.0:*");
                          //policy.WithOrigins("http://0.0.0.0:4000");

                      });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseExceptionHandler("/Error");
app.UseCors(MyAllowSpecificOrigins);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//监听至 3939 端口
app.Urls.Add("http://0.0.0.0:3939");
app.UseAuthorization();

app.MapControllers();
#endregion

app.MapGet("/test", () => "ok::"+DateTime.Now);


app.Run();
Console.WriteLine("运行");

