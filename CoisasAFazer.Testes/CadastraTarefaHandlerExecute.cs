using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        [Fact]
        public void DadaTarefaComInfoValidadasDeveIncluirNoBD()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var contexto = new DbTarefasContext(options);
            var repo = new RepositorioTarefa(contexto);

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();

            var handler = new CadastraTarefaHandler(repo, mock.Object);

            //act
            handler.Execute(comando); //SUT >> CadastraTarefaHandlerExecute

            //assert
            var tarefa = repo.ObtemTarefas(t => t.Titulo == "Estudar Xunit").FirstOrDefault();
            Assert.NotNull(tarefa);
        }

        [Fact]
        public void QuandoExceptionForLancadaResultadoIsSucessDeveSerFalse()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var mock = new Mock<IRepositorioTarefas>();
            var mockLog = new Mock<ILogger<CadastraTarefaHandler>>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));
            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLog.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            Assert.False(resultado.isSucesss);
        }

        delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<object, Exception, string> func);

        [Fact]
        public void DadaTarefaComInfoValidasDeveLogar()
        {
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));

            var mockLog = new Mock<ILogger<CadastraTarefaHandler>>();

            LogLevel levelCaptrado = LogLevel.Error;
            string mensagemCaptura = string.Empty;

            CapturaMensagemLog captura = (level, eventId, state, exception, func) =>
            {
                levelCaptrado = level;
                mensagemCaptura = func(state,exception);
            };

            mockLog.Setup(l =>
                l.Log(
                     It.IsAny<LogLevel>(),
                     It.IsAny<EventId>(),
                     It.IsAny<object>(),
                     It.IsAny<Exception>(),
                     It.IsAny<Func<object, Exception, string>>()
                )).Callback(captura);

            var mock = new Mock<IRepositorioTarefas>();



            var handler = new CadastraTarefaHandler(mock.Object, mockLog.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            Assert.Equal(LogLevel.Debug, levelCaptrado);
            Assert.Contains("Persistindo a tarefa...", mensagemCaptura);
        }


        [Fact]
        public void QuandoExceptionFrLancadaDeveLogarAMensagemDaExcecao()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar Xunit", new Categoria("Estudo"), new DateTime(2019, 12, 31));
            var mock = new Mock<IRepositorioTarefas>();
            var mockLog = new Mock<ILogger<CadastraTarefaHandler>>();
            string msgErro = "Houve um erro na inclusão de tarefas";
            var ex = new Exception(msgErro);
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>()))
                .Throws(ex);
            var repo = mock.Object;

            var handler = new CadastraTarefaHandler(repo, mockLog.Object);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            mockLog.Verify(l => 
                l.Log(
                     LogLevel.Error,
                     It.IsAny<EventId>(),
                     It.IsAny<object>(),
                     ex, 
                     It.IsAny<Func<object,Exception, string>>()
                ), Times.Once());
        }
    }
}
