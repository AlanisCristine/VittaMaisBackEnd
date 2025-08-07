using CloudinaryDotNet;
using VittaMais.API;
using VittaMais.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar Cloudinary
builder.Services.AddSingleton(_ =>
{
    Account account = new Account(
        "du4uvbmzy", // Cloud Name
        "925321576856996", // API Key
        "fDAJ9k_6GBrSc7zXnf4V050HjWU" // API Secret
    );
    return new Cloudinary(account);
});

// Serviços
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ConsultaService>();
builder.Services.AddScoped<EspecialidadeService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<TokenService>();

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🔽 Ordem do middleware importa muito! 🔽

// Swagger deve vir primeiro
app.UseSwagger();
app.UseSwaggerUI();

// CORS deve vir antes de Authorization e MapControllers
app.UseCors("PermitirTudo");

// Se você for usar autenticação, adicione antes do Authorization:
// app.UseAuthentication(); 

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
