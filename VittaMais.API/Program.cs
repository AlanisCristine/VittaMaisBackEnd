using VittaMais.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Registrar dependências antes de chamar builder.Build()
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<EspecialidadeService>();


// Registrar outros serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do CORS
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

// Permite requisições de qualquer origem (CORS)
app.UseCors("PermitirTudo");


// Configuração de autorização e mapeamento de controladores
app.UseAuthorization();
app.MapControllers();

app.Run();
