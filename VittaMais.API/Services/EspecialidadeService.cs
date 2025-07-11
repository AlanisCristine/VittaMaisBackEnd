
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using VittaMais.API.Models;

namespace VittaMais.API.Services
{
    public class EspecialidadeService
    {
        private readonly FirebaseService _firebaseService;
        private readonly Cloudinary _cloudinary;

        public EspecialidadeService(FirebaseService firebaseService, Cloudinary cloudinary)
        {
            _firebaseService = firebaseService;
            _cloudinary = cloudinary;
        }

        public async Task<string> AdicionarEspecialidade(Especialidade especialidade)
        {
            // 1. Salva a especialidade (ainda com Id vazio ou "string")
            var refEspecialidade = await _firebaseService
                .GetDatabase()
                .Child("especialidades")
                .PostAsync(JsonConvert.SerializeObject(especialidade));

            // 2. Pega o ID gerado automaticamente
            var firebaseId = refEspecialidade.Key;

            // 3. Preenche o objeto com o ID correto
            especialidade.Id = firebaseId;

            // 4. Atualiza no Firebase com o ID corrigido
            await _firebaseService
                .GetDatabase()
                .Child("especialidades")
                .Child(firebaseId)
                .PutAsync(JsonConvert.SerializeObject(especialidade));

            return firebaseId;
        }

        public async Task<string?> UploadImagemAsync(IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
                return null;

            using var stream = imagem.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imagem.FileName, stream),
                Folder = "vittamais/especialidades" // opcional, para organizar no Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                return uploadResult.SecureUrl.ToString();

            throw new Exception("Erro ao fazer upload no Cloudinary: " + uploadResult.Error.Message);
        }

        public async Task<string> AdicionarEspecialidadeComImagemAsync(IFormFile? imagem, Especialidade especialidade)
        {
            if (imagem != null && imagem.Length > 0)
            {
                var imagemUrl = await UploadImagemAsync(imagem);
                especialidade.ImagemUrl = imagemUrl;
            }

            // 1. Salva a especialidade sem ID
            var refEspecialidade = await _firebaseService
                 .GetDatabase()
                .Child("especialidades")
                .PostAsync(JsonConvert.SerializeObject(especialidade));

            // 2. Pega o ID gerado automaticamente
            var firebaseId = refEspecialidade.Key;

            // 3. Preenche o objeto com o ID correto
            especialidade.Id = firebaseId;

            // 4. Atualiza no Firebase com o ID corrigido
            await _firebaseService
                 .GetDatabase()
                .Child("especialidades")
                .Child(firebaseId)
                .PutAsync(JsonConvert.SerializeObject(especialidade));

            return firebaseId;
        }


        public async Task<List<Especialidade>> ListarEspecialidades()
        {
            var especialidades = new List<Especialidade>();

            var snapshot = await _firebaseService
                .GetDatabase()
                .Child("especialidades")
                .OnceAsync<Especialidade>();

            foreach (var item in snapshot)
            {
                especialidades.Add(item.Object);
            }

            return especialidades;
        }

        public async Task<Especialidade?> ObterPorId(string id)
        {
            var resultado = await _firebaseService
                .GetDatabase()
                .Child("especialidades")
                .Child(id)
                .OnceSingleAsync<Especialidade>();

            return resultado;
        }


        public async Task EditarEspecialidadeComImagemAsync(string id, IFormFile? imagem, Especialidade especialidadeAtualizada)
        {
            // Busca a especialidade atual
            var especialidadeExistente = await ObterPorId(id);

            if (especialidadeExistente == null)
                throw new Exception("Especialidade não encontrada.");

            // Se imagem enviada, faz upload e atualiza a URL
            if (imagem != null && imagem.Length > 0)
            {
                var imagemUrl = await UploadImagemAsync(imagem);
                especialidadeExistente.ImagemUrl = imagemUrl;
            }

            // Atualiza dados
            especialidadeExistente.Nome = especialidadeAtualizada.Nome;
            especialidadeExistente.Descricao = especialidadeAtualizada.Descricao;
            especialidadeExistente.Valor = especialidadeAtualizada.Valor;

            // Atualiza no Firebase
            await _firebaseService
                .GetDatabase()
                .Child("especialidades")
                .Child(id)
                .PutAsync(JsonConvert.SerializeObject(especialidadeExistente));
        }


    }

}