using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI_Lab2.Data;
using WebAPI_Lab2.Repository;
using WebAPI_Lab2.UnitOfWork;

namespace WebAPI_Lab2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(Program));

            // Allow CORS => Cross Origin Resource Sharing to consume my API
            builder.Services.AddCors();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "xmlfile.xml");
                options.IncludeXmlComments(filePath);

                // Add Definition for APIs
                options.SwaggerDoc("StudentEndpoints", new OpenApiInfo
                {
                    Title = "Student",
                    Version = "v1",
                    Description = "Student API Endpoint",
                    Contact = new OpenApiContact
                    {
                        Name = "Islam Ismail",
                        Email = "islam.ismail.ali@icloud.com",
                        Url = new Uri("https://www.linkedin.com/in/islam-ismail-ali/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "My License",
                        Url = new Uri("https://www.linkedin.com/in/islam-ismail-ali/")
                    }
                });

                options.SwaggerDoc("DepartmentEndpoints", new OpenApiInfo
                {
                    Title = "Department",
                    Version = "v1",
                    Description = "Department API Endpoint",
                    Contact = new OpenApiContact
                    {
                        Name = "Islam Ismail",
                        Email = "islam.ismail.ali@icloud.com",
                        Url = new Uri("https://www.linkedin.com/in/islam-ismail-ali/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "My License",
                        Url = new Uri("https://www.linkedin.com/in/islam-ismail-ali/")
                    }
                });

                // For Authorize the API with JWT Bearer Tokens

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT API KEY"
                });

                // For Authorize the End Points such as GET,POST 

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.EnableAnnotations();
            });


            // Configure the connection string
            builder.Services.AddDbContext<ItiContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add services UnitOfWork
            builder.Services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            builder.Services.AddScoped(typeof(IHelperRepository), typeof(HelperRepository));

            // Add Authentication for JwtBearer Json Web Token
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    //    options.InjectStylesheet("/swagger-ui/custom.css");
                    options.SwaggerEndpoint("/swagger/StudentEndpoints/swagger.json", "StudentAPI");
                    options.SwaggerEndpoint("/swagger/DepartmentEndpoints/swagger.json", "DepartmentAPI");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
