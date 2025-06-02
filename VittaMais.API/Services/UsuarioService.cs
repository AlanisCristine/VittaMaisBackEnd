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
        // Construtor que recebe o FirebaseService
        public UsuarioService(FirebaseService firebaseService)
        {
            Account account = new Account(
           "du4uvbmzy",         // Cloud Name
           "925321576856996",   // API Key
           "fDAJ9k_6GBrSc7zXnf4V050HjWU" // API Secret
       );

            _cloudinary = new Cloudinary(account);
            _firebase = firebaseService.GetDatabase();
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

        public async Task<bool> EditarPaciente(string id, PacienteDTO pacienteDto)
        {
            var usuarioRef = await _firebase
                .Child("usuarios")
                .Child(id)
                .OnceSingleAsync<Usuario>();

            if (usuarioRef == null || usuarioRef.Tipo != TipoUsuario.Paciente)
                return false;

            // Atualiza os dados permitidos
            usuarioRef.Nome = pacienteDto.Nome;
            usuarioRef.Cpf = pacienteDto.Cpf;
            usuarioRef.DataNascimento = pacienteDto.DataNascimento;
            usuarioRef.Telefone = pacienteDto.Telefone;
            usuarioRef.Endereco = pacienteDto.Endereco;

            // Atualiza senha apenas se vier preenchida
            if (!string.IsNullOrWhiteSpace(pacienteDto.Senha))
            {
                usuarioRef.Senha = BCrypt.Net.BCrypt.HashPassword(pacienteDto.Senha);
            }

            await _firebase
                .Child("usuarios")
                .Child(id)
                .PutAsync(JsonConvert.SerializeObject(usuarioRef));

            return true;
        }
    }
}

