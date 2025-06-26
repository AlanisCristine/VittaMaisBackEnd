using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using VittaMais.API.Models;

namespace VittaMais.API.Services
{
    public class EspecialidadeService
    {
        private readonly FirebaseService _firebaseService;

        public EspecialidadeService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
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

        public async Task<Especialidade> ObterPorId(string id)
        {
            var resultado = await _firebaseService
                .GetDatabase()                    
                .Child("especialidades")
                .Child(id)
                .OnceSingleAsync<Especialidade>();

            return resultado;
        }


    }

}
