namespace DojoFSharpWebApi
#nowarn "20"

open Microsoft.AspNetCore.Builder
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.OpenApi.Models

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddDbContext<DatabaseContext>(fun options -> 
            options.UseSqlite("Filename=ToDoList.db") |> ignore)

        builder.Services.AddScoped<ToDoListRepository>()
        
        // AuthN & AuthZ
        builder.Services.AddAuthentication().AddJwtBearer()
        builder.Services.AddAuthorization()

        builder.Services
            .AddAuthorizationBuilder()
            .AddPolicy("RequireAdminFromZDI", fun policy ->
                policy
                    .RequireRole("admin")
                    .RequireClaim("company", "ZDI") |> ignore)

        // Swagger
        let tokenId = "TokenAuthNZ"

        let securityScheme = new OpenApiSecurityScheme (
            Name = "Authorization",
            Description = "Token-based authentication and authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        )

        let securityRequirement = new OpenApiSecurityRequirement()
        securityRequirement.Add(
            new OpenApiSecurityScheme(
                Reference = new OpenApiReference(
                    Type = ReferenceType.SecurityScheme,
                    Id = tokenId
                )
            ), [||]
        )

        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen(fun options ->
            options.SwaggerDoc("v1", new OpenApiInfo(Title = "Dojo F# APIs", Version = "v1"))
            options.AddSecurityDefinition(tokenId, securityScheme)
            options.AddSecurityRequirement(securityRequirement)
        )

        builder.Services.AddControllers()

        let app = builder.Build()

        app.UseHttpsRedirection()

        app.UseSwagger()
        app.UseSwaggerUI()

        app.MapControllers()

        app.Run()

        exitCode
