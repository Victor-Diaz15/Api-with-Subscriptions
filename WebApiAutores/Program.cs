using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebApiAutores;
using WebApiAutores.Entiities;
using WebApiAutores.Filters;
using WebApiAutores.Middlewares;
using WebApiAutores.Services;
using WebApiAutores.Utilidades;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Services

//limpiando el mapeo automatico de los claims.
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddControllers(options =>
{
    //filtro general para cuando se lance una excepcion en cualquier accion de mis controladores.
    options.Filters.Add(typeof(FilterOfException));

    options.Conventions.Add(new SwaggerAgrupaPorVersion());
})
    //Esta configuracion sirve, para especificar que se ingnoren los ciclos en las peticiones
    //cuando una entidad tiene data realacionada.
    .AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddNewtonsoftJson();

//Esto es para agregar la conexion a la base de datos.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("defaultConnection")
    ));

//configuracion de los servicios para la inyeccion de dependencias
//builder.Services.AddTransient<IServicio, ServicioA>();

//builder.Services.AddTransient<ServicioTransient>();
//builder.Services.AddScoped<ServicioScoped>();
//builder.Services.AddSingleton<ServicioSingleton>();

//configurando la inyeccion de mi filtro de accion personalizado
builder.Services.AddTransient<MyFilterOfAction>();

//agregando automapper
builder.Services.AddAutoMapper(typeof(Program));

//inyeccion del servicio de escribir en archivos
//El cual es un servicio que se puede ejecutar concurrentemente utilizando HostedService
//builder.Services.AddHostedService<WriteInFile>();

//servicio para trabajar con cache.
builder.Services.AddResponseCaching();

//configurando autenticacion con jtwbearer token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["firmToken"])),
        ClockSkew = TimeSpan.Zero
    });

//Esta es la configuracion para swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiAutores", Version = "v1" });

    c.OperationFilter<AgregarParametroHATEOAS>();
    c.OperationFilter<AgregarParametroXVersion>();

    //configurando swagger para la autenticacion con token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });

    //var archivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXml);

    //c.IncludeXmlComments(rutaXML);

});

//Configurando Identity para el manejo de usuarios y roles de nuestro sistema
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//Configurando la politica de roles en mi aplicacion
builder.Services.AddAuthorization(opciones => {
    opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("EsAdmin"));
});

//Configurando para poder usar los  servicios de data protector
builder.Services.AddDataProtection();

//Registrando el servicio del hashing
builder.Services.AddTransient<HashService>();

//configurando CORS
builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(opciones =>
    {
        opciones.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
    });
});

builder.Services.AddTransient<GeneradorEnlaces>();
builder.Services.AddTransient<HATEOASAutorFilterAttribute>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddScoped<LlaveApiService>();

builder.Services.AddHostedService<FacturasHostedService>();
//builder.Services.AddApplicationInsightsTelemetry();


#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.

#region Environment

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c => {

    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");

});

#endregion

#region Middlewares

//middleware personalizado.
app.UseLoguerResponseHTTP();

app.UseHttpsRedirection();

app.UseRouting();

//CORS
app.UseCors();

//Api Key Middelware
app.UseLimitarPeticiones();

//middleware para trabajar con cache en nuestro sistema.
app.UseResponseCaching();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

});

#endregion

app.Run();
