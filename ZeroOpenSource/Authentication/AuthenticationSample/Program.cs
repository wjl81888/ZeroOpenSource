using GitHub;
using Microsoft;
using Tencent;
using WeChat;
using Weibo;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
//使用示例
builder.Services.AddAuthentication()
                .AddMicrosoftAccount(microsoftOptions =>
                    {
                        microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                        microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                        microsoftOptions.RemoteAuthenticationTimeout = TimeSpan.FromDays(15);
                        microsoftOptions.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    })
                .AddQQ(qqOptions =>
                    {
                        qqOptions.ClientId = Configuration["Authentication:TencentQQ:AppID"];
                        qqOptions.ClientSecret = Configuration["Authentication:TencentQQ:AppKey"];
                        qqOptions.RemoteAuthenticationTimeout = TimeSpan.FromDays(15);
                        qqOptions.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    })
                .AddGitHub(gitHubOptions =>
                    {
                        gitHubOptions.ClientId = Configuration["Authentication:GitHub:ClientID"];
                        gitHubOptions.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                        gitHubOptions.RemoteAuthenticationTimeout = TimeSpan.FromDays(15);
                        gitHubOptions.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    })
                .AddWeibo("Weibo", "微博", weiboOptions =>
                    {
                        weiboOptions.ClientId = Configuration["Authentication:Weibo:AppKey"];
                        weiboOptions.ClientSecret = Configuration["Authentication:Weibo:AppSecret"];
                        weiboOptions.UserEmailsEndpoint = string.Empty;
                        weiboOptions.RemoteAuthenticationTimeout = TimeSpan.FromDays(15);
                        weiboOptions.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    })
                .AddWeixin("WeChat", "微信", weChatOptions =>
                    {
                        weChatOptions.ClientId = Configuration["Authentication:WeChat:AppID"];
                        weChatOptions.ClientSecret = Configuration["Authentication:WeChat:AppSecret"];
                        weChatOptions.RemoteAuthenticationTimeout = TimeSpan.FromDays(15);
                        weChatOptions.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
