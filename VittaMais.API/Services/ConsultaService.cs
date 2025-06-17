using Firebase.Database.Query;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VittaMais.API.Models;
using VittaMais.API.Models.DTOs;

namespace VittaMais.API.Services
{
    public class ConsultaService
    {
        private readonly FirebaseService _firebaseService;

        public ConsultaService(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<string> AgendarConsulta(Consulta consulta)
        {
            // Verifica se o médico existe
            var medico = await _firebaseService
                .GetDatabase()
                .Child("usuarios")
                .Child(consulta.MedicoId)
                .OnceSingleAsync<Usuario>();

            if (medico == null || medico.Tipo != TipoUsuario.Medico)
                throw new Exception("Médico inválido.");

            // Verifica se a especialidade existe
            var especialidade = await _firebaseService
                .GetDatabase()
                .Child("especialidades")
                .Child(medico.EspecialidadeId)
                .OnceSingleAsync<Especialidade>();

            if (especialidade == null)
                throw new Exception("Especialidade não encontrada.");

            // Agora associamos a especialidade na consulta
            consulta.EspecialidadeId = medico.EspecialidadeId;

            // Cria a consulta no Firebase
            var consultaRef = await _firebaseService
                .GetDatabase()
                .Child("consultas")
                .PostAsync(JsonConvert.SerializeObject(consulta));

            // Seta o ID da consulta com o do Firebase
            consulta.Id = consultaRef.Key;

            // Atualiza o objeto com o ID
            await _firebaseService
                .GetDatabase()
                .Child("consultas")
                .Child(consulta.Id)
                .PutAsync(JsonConvert.SerializeObject(consulta));

            return consulta.Id;
        }

        public async Task AtualizarConsulta(AtualizarConsultaDto dto)
        {
            // Busca consulta existente
            var consultaRef = _firebaseService.GetDatabase()
                .Child("consultas")
                .Child(dto.Id);

            var consultaExistente = await consultaRef.OnceSingleAsync<Consulta>();

            if (consultaExistente == null)
                throw new Exception("Consulta não encontrada.");

            // Atualiza apenas os campos clínicos
            consultaExistente.Diagnostico = dto.Diagnostico;
            consultaExistente.Observacoes = dto.Observacoes;
            consultaExistente.Remedios = dto.Remedios;

            // Salva novamente
            await consultaRef.PutAsync(JsonConvert.SerializeObject(consultaExistente));
        }

        public async Task<List<Consulta>> ListarConsultas()
        {
            var consultas = new List<Consulta>();
            var consultaSnapshot = await _firebaseService.GetDatabase().Child("consultas").OnceAsync<Consulta>();

            foreach (var consulta in consultaSnapshot)
            {
                consultas.Add(consulta.Object);
            }

            return consultas;
        }
        public async Task<List<Consulta>> ListarConsultasPorEspecialidade(string especialidadeId)
        {
            var db = _firebaseService.GetDatabase();

            // Passo 1: Buscar médicos com a especialidadeId
            var medicos = await db.Child("usuarios")
                .OnceAsync<Usuario>();

            var medicosComEspecialidade = medicos
                .Where(m => m.Object.Tipo == TipoUsuario.Medico && m.Object.EspecialidadeId == especialidadeId)
                .Select(m => m.Key) // Pega apenas os IDs dos médicos
                .ToList();

            // Passo 2: Buscar todas as consultas
            var consultas = await db.Child("consultas")
                .OnceAsync<Consulta>();

            // Passo 3: Filtrar consultas com os médicos da especialidade
            var resultado = consultas
                .Where(c => medicosComEspecialidade.Contains(c.Object.MedicoId))
                .Select(c => c.Object)
                .ToList();

            return resultado;
        }
        public async Task<List<Consulta>> ListarConsultasPorMedico(string medicoId)
        {
            var db = _firebaseService.GetDatabase();

            // Buscar todas as consultas
            var consultasSnap = await db.Child("consultas").OnceAsync<Consulta>();
            var consultas = consultasSnap
                .Where(c => c.Object.MedicoId == medicoId)
                .ToList();

            // Buscar todos os usuários (pacientes) para mapear o nome
            var usuariosSnap = await db.Child("usuarios").OnceAsync<Usuario>();
            var usuarios = usuariosSnap.ToDictionary(u => u.Key, u => u.Object);

            var resultado = new List<Consulta>();

            foreach (var consultaSnap in consultas)
            {
                var consulta = consultaSnap.Object;

                // Verifica se o paciente existe e adiciona o nome
                if (usuarios.TryGetValue(consulta.PacienteId, out var paciente))
                {
                    consulta.NomePaciente = paciente.Nome; // Certifique-se de que Consulta tem essa propriedade
                }

                resultado.Add(consulta);
            }

            return resultado;
        }
        public async Task<List<ConsultaDetalhada>> ListarConsultasDetalhadasPorEspecialidade(string especialidadeId)
        {
            var db = _firebaseService.GetDatabase();

            // Médicos com a especialidade
            var usuariosSnapshot = await db.Child("usuarios").OnceAsync<Usuario>();
            var medicos = usuariosSnapshot
                .Where(u => u.Object.Tipo == TipoUsuario.Medico && u.Object.EspecialidadeId == especialidadeId)
                .ToList();

            var medicoIds = medicos.Select(m => m.Key).ToList();

            // Consultas feitas com esses médicos
            var consultasSnapshot = await db.Child("consultas").OnceAsync<Consulta>();
            var consultasFiltradas = consultasSnapshot
                .Where(c => medicoIds.Contains(c.Object.MedicoId))
                .ToList();

            // Pacientes
            var pacientes = usuariosSnapshot
                .Where(u => u.Object.Tipo == TipoUsuario.Paciente)
                .ToDictionary(p => p.Key, p => p.Object.Nome);

            // Especialidade
            var especialidadesSnapshot = await db.Child("especialidades").OnceAsync<Especialidade>();
            var especialidadeNome = especialidadesSnapshot
                .FirstOrDefault(e => e.Key == especialidadeId)?.Object?.Nome ?? "Desconhecida";

            // Montar resultado detalhado
            var resultado = new List<ConsultaDetalhada>();
            foreach (var c in consultasFiltradas)
            {
                var consulta = c.Object;
                var medicoNome = medicos.FirstOrDefault(m => m.Key == consulta.MedicoId)?.Object?.Nome ?? "Desconhecido";
                var pacienteNome = pacientes.ContainsKey(consulta.PacienteId) ? pacientes[consulta.PacienteId] : "Desconhecido";

                resultado.Add(new ConsultaDetalhada
                {
                    Id = c.Key,
                    MedicoId = consulta.MedicoId,
                    MedicoNome = medicoNome,
                    PacienteId = consulta.PacienteId,
                    PacienteNome = pacienteNome,
                    EspecialidadeId = especialidadeId,
                    EspecialidadeNome = especialidadeNome,
                    Data = consulta.Data,
                    Status = consulta.Status
                });
            }

            return resultado;
        }
        public async Task<object> ObterConsultasPorUsuario(string usuarioId)
        {
            var db = _firebaseService.GetDatabase();

            // Pega o nome do paciente
            var paciente = await db.Child("usuarios").Child(usuarioId).OnceSingleAsync<Usuario>();
            if (paciente == null)
                return new { Mensagem = "Usuário não encontrado." };

            // Pega todas as consultas
            var consultasSnapshot = await db.Child("consultas").OnceAsync<Consulta>();

            // Monta a lista com todos os dados
            var consultasDoUsuario = new List<object>();

            foreach (var consulta in consultasSnapshot)
            {
                var c = consulta.Object;

                if (c.PacienteId != usuarioId)
                    continue;

                // Pega o nome do médico
                var medico = await db.Child("usuarios").Child(c.MedicoId).OnceSingleAsync<Usuario>();
                var nomeMedico = medico?.Nome ?? "Médico não encontrado";

                // Pega a especialidade do médico
                string nomeEspecialidade = "Especialidade não encontrada";
                if (!string.IsNullOrEmpty(medico?.EspecialidadeId))
                {
                    var especialidade = await db.Child("especialidades").Child(medico.EspecialidadeId).OnceSingleAsync<Especialidade>();
                    nomeEspecialidade = especialidade?.Nome ?? nomeEspecialidade;
                }

                // Adiciona à lista
                consultasDoUsuario.Add(new
                {
                    NomePaciente = paciente.Nome,
                    Data = c.Data,
                    NomeMedico = nomeMedico,
                    Especialidade = nomeEspecialidade,
                    Status = c.Status.ToString()
                });
            }

            return consultasDoUsuario;
        }
        public async Task<Consulta> ObterConsultaPorId(string id)
        {
            var consulta = await _firebaseService
                .GetDatabase()
                .Child("consultas")
                .Child(id)
                .OnceSingleAsync<Consulta>();

            return consulta;
        }

        public async Task<List<Consulta>> ListarConsultasPorData(DateTime data)
        {
            var db = _firebaseService.GetDatabase();
            var consultaSnapshot = await db.Child("consultas").OnceAsync<Consulta>();

            var consultasDoDia = consultaSnapshot
                .Where(c => c.Object.Data.Date == data.Date)
                .Select(c => c.Object)
                .OrderBy(c => c.Data) // opcional: ordena por horário
                .ToList();

            return consultasDoDia;
        }





    }

}
