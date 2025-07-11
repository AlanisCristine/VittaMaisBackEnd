using CloudinaryDotNet;
using VittaMais.API;
using VittaMais.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ =>
{
    Account account = new Account(
        "du4uvbmzy", // Cloud Name
        "925321576856996", // API Key
        "fDAJ9k_6GBrSc7zXnf4V050HjWU" // API Secret
    );
    return new Cloudinary(account);
});

// Registrar depend�ncias antes de chamar builder.Build()
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<EspecialidadeService>();
builder.Services.AddScoped<EmailService>();





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
