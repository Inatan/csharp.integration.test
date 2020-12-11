using System;
using Xunit;
using Moq;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Core.Models;

namespace CoisasAFazer.Testes
{
    public class TarefasControllerEndpointCadastraTarefa
    {
        [Fact]
        public void Test()
        {
            //arrange

            var mockLog = new Mock<ILogger<CadastraTarefaHandler>>();


            var option = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;

            var contexto = new DbTarefasContext(option);
            contexto.Categorias.Add(new Categoria(20, "estudo"));
            var repo = new RepositorioTarefa(contexto);

            

            var controlador = new TarefasController(repo, mockLog.Object);

            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar Xunit";
            model.Prazo = new DateTime(2019, 12, 31);
            //act

            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<OkResult>(retorno);
        }

        [Fact]
        public void Teste()
        {
            //arrange

            var mockLog = new Mock<ILogger<CadastraTarefaHandler>>();


            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20,"Estudo"));
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));
            
            var repo = mock.Object;



            var controlador = new TarefasController(repo, mockLog.Object);

            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar Xunit";
            model.Prazo = new DateTime(2019, 12, 31);
            //act

            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<StatusCodeResult>(retorno);
            var statusCode = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCode);

        }
    }
}
