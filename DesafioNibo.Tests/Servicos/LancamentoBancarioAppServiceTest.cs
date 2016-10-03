using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DesafioNibo.Dominio;
using DesafioNibo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DesafioNibo.Tests.Servicos
{
    [TestClass]
    public class LancamentoBancarioAppServiceTest
    {
        [TestMethod]        
        public void ImportacaoDeTodosOsArquivosTest()
        {
            var lancamentoBancarioAppService = new LancamentoBancarioAppService();
            var arquivos = Directory.GetFiles(Directory.GetParent(@"../../").FullName + @"\Arquivos\");
            var lancamentosBancarios = new List<LancamentoBancario>();

            Parallel.ForEach(arquivos, (arquivo) =>
            {
                string[] linhas;
                using (var streamReader = new StreamReader(arquivo))
                {
                    var resultado = streamReader.ReadToEndAsync().Result;
                    linhas = resultado.Split(new[] { "\r\n" }, StringSplitOptions.None);
                }

                lancamentosBancarios.AddRange(lancamentoBancarioAppService.PopulaLancamentosBancarios(linhas));
            });

            Assert.IsTrue(lancamentosBancarios.Any());
            Assert.AreEqual(lancamentosBancarios.Count, 99);
        }

        [TestMethod]
        public void SelecionaChaveTest()
        {
            var lancamentoBancarioAppService = new LancamentoBancarioAppService();
            var resultado = lancamentoBancarioAppService.SelecionaChave("<CURDEF>");

            Assert.AreEqual(resultado, "CURDEF");
        }

        [TestMethod]
        public void SelecionaValorTest()
        {
            var lancamentoBancarioAppService = new LancamentoBancarioAppService();
            var resultado = lancamentoBancarioAppService.SelecionaValor("<CURDEF>BRL");

            Assert.AreEqual(resultado, "BRL");
        }

        [TestMethod]
        public void SelecionaValorDataTest()
        {
            var lancamentoBancarioAppService = new LancamentoBancarioAppService();
            var resultado = lancamentoBancarioAppService.SelecionarValorData("<DTSTART>20140605100000[-03:GTM]");

            Assert.AreEqual(resultado, new DateTime(2014, 06, 05, 10, 0, 0));
        }

        [TestMethod]
        public void BuscaEnumPelaDescricaoTest()
        {
            var lancamentoBancarioAppService = new LancamentoBancarioAppService();
            var resultado = (TipoDeMoeda) lancamentoBancarioAppService.BuscaEnumPelaDescricao(
                lancamentoBancarioAppService.SelecionaValor("<CURDEF>BRL"), typeof(TipoDeMoeda));

            Assert.AreEqual(resultado, TipoDeMoeda.Real);
        }
    }
}
