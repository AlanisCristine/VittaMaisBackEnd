using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Newtonsoft.Json;
using VittaMais.API.Models;
using VittaMais.API.Models.DTOs;

namespace VittaMais.API.Services
{
    public class UsuarioService
    {
        private readonly FirebaseClient _firebase;
        private readonly Cloudinary _cloudinary;
        private readonly EmailService _emailService;
        private readonly List<TokenRecuperacao> _tokens = new();


        // Construtor que recebe o FirebaseService
        public UsuarioService(FirebaseService firebaseService, EmailService emailService)
        {
            Account account = new Account(
           "du4uvbmzy",         // Cloud Name
           "925321576856996",   // API Key
           "fDAJ9k_6GBrSc7zXnf4V050HjWU" // API Secret
       );

            _cloudinary = new Cloudinary(account);
            _firebase = firebaseService.GetDatabase();
            _emailService = emailService;
        }

        public async Task<string> CadastrarUsuario(Usuario usuario)
        {
            if (usuario.Tipo == TipoUsuario.Medico && string.IsNullOrEmpty(usuario.EspecialidadeId))
            {
                throw new Exception("Médicos precisam de uma especialidade.");
            }

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            // Passo 1: Posta o objeto (sem ID ainda)
            var usuarioRef = await _firebase
                .Child("usuarios")
                .PostAsync(JsonConvert.SerializeObject(usuario));

            // Passo 2: Pega o ID gerado pelo Firebase
            var usuarioId = usuarioRef.Key;

            // Passo 3: Atualiza o objeto com o ID correto
            usuario.Id = usuarioId;

            // Passo 4: Salva de novo com o ID incluído
            await _firebase
                .Child("usuarios")
                .Child(usuarioId)
                .PutAsync(JsonConvert.SerializeObject(usuario));

            return usuarioId;
        }

        public async Task<string> UploadFotoAsync(IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
                return null;

            using var stream = imagem.OpenReadStream();

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(imagem.FileName, stream),
                Folder = "usuarios"  // opcional: pasta no Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();  // URL pública da imagem
            }
            else
            {
                throw new Exception("Falha no upload da imagem para Cloudinary: " + uploadResult.Error.Message);
            }
        }


        public async Task<string> CadastrarUsuarioComFoto(IFormFile? imagem, Usuario usuario)
        {
            if (usuario.Tipo == TipoUsuario.Medico && string.IsNullOrEmpty(usuario.EspecialidadeId))
                throw new Exception("Médicos precisam de uma especialidade.");

            if (imagem != null && imagem.Length > 0)
            {
                var urlFoto = await UploadFotoAsync(imagem);
                usuario.FotoUrl = urlFoto;  // agora é uma URL, não base64
            }

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            var usuarioRef = await _firebase
                .Child("usuarios")
                .PostAsync(JsonConvert.SerializeObject(usuario));

            var usuarioId = usuarioRef.Key;

            usuario.Id = usuarioId;

            await _firebase
                .Child("usuarios")
                .Child(usuarioId)
                .PutAsync(JsonConvert.SerializeObject(usuario));

            return usuarioId;
        }


        public async Task<Usuario?> Login(string email, string senha)
        {
            var usuarioRaw = await _firebase
                .Child("usuarios")
                .OrderBy("email")
                .EqualTo(email)
                .OnceAsync<Usuario>();

            // Verifique os dados brutos retornados pelo Firebase
            Console.WriteLine(JsonConvert.SerializeObject(usuarioRaw, Formatting.Indented));

            // Agora, tente acessar o usuário
            var usuario = usuarioRaw?.FirstOrDefault()?.Object;

            if (usuario == null)
            {
                Console.WriteLine("Nenhum usuário encontrado.");
                return null;
            }

            if (usuario.Senha == null)
            {
                Console.WriteLine("Senha do usuário não encontrada.");
                return null;
            }

            // Verifica se a senha digitada bate com o hash salvo
            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                Console.WriteLine("Senha incorreta.");
                return null;
            }

            return usuario;
        }

        public async Task<List<Usuario>> ListarUsuarios()
        {
            var usuarios = await _firebase
                .Child("usuarios")
                .OnceAsync<Usuario>();

            return usuarios.Select(x => x.Object).ToList();
        }

        public async Task<List<Usuario>> ListarMedicos()
        {
            // Obtém todos os usuários da coleção "usuarios" no Firebase
            var usuarios = await _firebase
                .Child("usuarios")
                .OnceAsync<Usuario>();

            // Filtra apenas os usuários do tipo "Medico"
            return usuarios
                .Select(x => x.Object)
                .Where(u => u.Tipo == TipoUsuario.Medico)  // Filtra médicos
                .ToList();
        }

        public async Task<List<Usuario>> ListarPacientes()
        {
            // Obtém todos os usuários da coleção "usuarios" no Firebase
            var usuarios = await _firebase
                .Child("usuarios")
                .OnceAsync<Usuario>();

            // Filtra apenas os usuários do tipo "Medico"
            return usuarios
                .Select(x => x.Object)
                .Where(u => u.Tipo == TipoUsuario.Paciente)  // Filtra médicos
                .ToList();
        }

        public async Task<Usuario> BuscarPorIdAsync(string id)
        {
            var usuario = await _firebase
                .Child("usuarios")
                .Child(id)
                .OnceSingleAsync<Usuario>();

            return usuario != null ? usuario : null;
        }
       

        public async Task AtualizarDadosBasicosAsync(string id, AtualizarDadosPacienteDto dto)
        {
            var usuarioExistente = await _firebase
                .Child("usuarios")
                .Child(id)
                .OnceSingleAsync<Usuario>();

            if (usuarioExistente == null)
                throw new Exception("Usuário não encontrado.");

            // Atualiza apenas os campos recebidos
            usuarioExistente.Nome = dto.Nome;
            usuarioExistente.Email = dto.Email;
            usuarioExistente.Endereco = dto.Endereco;
            usuarioExistente.Cpf = dto.Cpf;
            usuarioExistente.Telefone = dto.Telefone;
            usuarioExistente.DataNascimento = dto.DataNascimento;
            // os campos como Id, Senha, FotoUrl, etc. são preservados

            await _firebase
                .Child("usuarios")
                .Child(id)
                .PutAsync(usuarioExistente);
        }

        public async Task AtualizarDadosBasicosMedicoAsync(string id, AtualizarDadosMedicoDTO dto)
        {
            var usuarioExistente = await _firebase
                .Child("usuarios")
                .Child(id)
                .OnceSingleAsync<Usuario>();

            if (usuarioExistente == null)
                throw new Exception("Usuário não encontrado.");

            // Atualiza apenas os campos recebidos
            usuarioExistente.Nome = dto.Nome;
            usuarioExistente.Email = dto.Email;
            usuarioExistente.Endereco = dto.Endereco;
            usuarioExistente.Cpf = dto.Cpf;
            usuarioExistente.Telefone = dto.Telefone;
            usuarioExistente.DataNascimento = dto.DataNascimento.Value;
            // os campos como Id, Senha, FotoUrl, etc. são preservados

            await _firebase
                .Child("usuarios")
                .Child(id)
                .PutAsync(usuarioExistente);
        }

        public async Task AtualizarDadosBasicosDiretorAsync(string id, AtualizarDadosDiretorDTO dto)
        {
            var usuarioExistente = await _firebase
                .Child("usuarios")
                .Child(id)
                .OnceSingleAsync<Usuario>();

            if (usuarioExistente == null)
                throw new Exception("Usuário não encontrado.");

            // Atualiza apenas os campos recebidos
            usuarioExistente.Nome = dto.Nome;
            usuarioExistente.Email = dto.Email;
            usuarioExistente.Endereco = dto.Endereco;
            usuarioExistente.Cpf = dto.Cpf;
            usuarioExistente.Telefone = dto.Telefone;
            usuarioExistente.DataNascimento = dto.DataNascimento.Value;
            // os campos como Id, Senha, FotoUrl, etc. são preservados

            await _firebase
                .Child("usuarios")
                .Child(id)
                .PutAsync(usuarioExistente);
        }


        public async Task AtualizarFotoPerfilAsync(string id, IFormFile foto)
        {
            // Buscar usuário no Firebase
            var usuario = await _firebase
                .Child("usuarios")
                .Child(id)
                .OnceSingleAsync<Usuario>();

            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            // Upload para Cloudinary
            using var stream = foto.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(foto.FileName, stream),
                PublicId = $"usuarios/{id}_foto", // usa o ID do usuário para manter a imagem única
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            // Atualiza a URL da foto no objeto do Firebase
            usuario.FotoUrl = uploadResult.SecureUrl.ToString();

            // Atualiza no Firebase
            await _firebase
                .Child("usuarios")
                .Child(id)
                .PutAsync(usuario);
        }

        public async Task AtualizarAsync(string id, Usuario usuario)
        {
               await _firebase
                .Child("usuarios")
                .Child(id)
                .PutAsync(usuario);
        }

        public async Task SolicitarRecuperacaoSenhaAsync(string email)
        {
            // 1. Buscar usuário
            var usuarios = await _firebase.Child("usuarios").OnceAsync<Usuario>();
            var usuario = usuarios.FirstOrDefault(u => u.Object.Email == email);

            if (usuario == null)
                throw new Exception("E-mail não encontrado.");

            // 2. Criar token
            var token = Guid.NewGuid().ToString();
            var tokenInfo = new TokenRecuperacaoSenha
            {
                Token = token,
                UsuarioId = usuario.Key,
                CriadoEm = DateTime.UtcNow,
                Usado = false
            };

            await _firebase.Child("tokens_recuperacao").Child(token).PutAsync(tokenInfo);

            // 3. Montar link com token
            var link = $"https://seusite.com/recuperar-senha?token={token}";

            // 4. E-mail HTML estilizado
            string emailHtml = $@"
        <html>
            <body style='font-family: Arial; background-color: #f6f6f6; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background: white; padding: 30px; border-radius: 10px;'>
                    <h2 style='color: #2b7bff;'>Recuperação de Senha - Vitta+</h2>
                    <p>Olá <strong>{usuario.Object.Nome}</strong>,</p>
                    <p>Você solicitou a recuperação de senha. Clique no botão abaixo para redefinir:</p>
                    <a href='{link}' style='background-color: #2b7bff; color: white; padding: 12px 20px; text-decoration: none; border-radius: 6px; display: inline-block;'>Redefinir senha</a>
                    <p style='margin-top: 20px;'>Se você não solicitou isso, ignore este e-mail.</p>
                    <p style='color: gray; font-size: 12px;'>Válido por 30 minutos.</p>
                </div>
            </body>
        </html>
    ";

            // 5. Enviar com Brevo
            //await _emailService.EnviarEmailAsync(email, "Recuperação de Senha - Vitta+", emailHtml, true);  true = isHtml
        }

        //public async Task<bool> EnviarTokenRecuperacaoAsync(string email)
        //{
        //    try
        //    {
        //        // 1. Verificar se usuário existe
        //        var usuario = await BuscarUsuarioPorEmail(email);

        //        // 2. Gerar token
        //        var token = Guid.NewGuid().ToString();

        //        // 3. Salvar token
        //        _tokens.Add(new TokenRecuperacao
        //        {
        //            Email = email,
        //            Token = token,
        //            Expiracao = DateTime.UtcNow.AddHours(1),
        //            Usado = false
        //        });

        //        // 4. Criar link e corpo do email
        //        var link = $"http://localhost:5173/redefinir-senha?email={email}&token={token}";

        //        var corpoHtml = $"<p>Para redefinir sua senha, clique no link abaixo:</p><a href='{link}'>Redefinir senha</a>";

        //        // 5. Enviar email
        //        return await _emailService.EnviarEmailConsultaAsync(email, "Recuperação de senha", corpoHtml);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro ao recuperar senha: {ex.Message}");
        //        return false;
        //    }
        //}

        // Implemente o BuscarUsuarioPorEmail conforme seu Firebase
        public async Task<Usuario> BuscarUsuarioPorEmail(string email)
        {
            var usuariosFirebase = await _firebase
                .Child("usuarios")
                .OnceAsync<Usuario>();

            var usuarioEncontrado = usuariosFirebase
                .Select(x => x.Object)
                .FirstOrDefault(u => u.Email.Trim().ToLower() == email.Trim().ToLower());

            if (usuarioEncontrado == null)
                throw new Exception("Email não encontrado.");

            return usuarioEncontrado;
        }


    }

}


