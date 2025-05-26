using VittaMais.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar depend�ncias antes de chamar builder.Build()
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<EspecialidadeService>();


// Registrar outros servi�os
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Usar o Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Permite requisi��es de qualquer origem (CORS)
app.UseCors("PermitirTudo");


// Configura��o de autoriza��o e mapeamento de controladores
app.UseAuthorization();
app.MapControllers();

app.Run();
