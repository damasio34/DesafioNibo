using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using DesafioNibo.Dominio;
using DesafioNibo.Models;

namespace DesafioNibo.Services
{
    public class LancamentoBancarioAppService
    {
        /// <summary>
        /// Seleciona a chave de uma tag
        /// </summary>
        /// <param name="tag">tag</param>
        /// <returns>Chave da tag</returns>
        public string SelecionaChave(string tag)
        {
            tag = tag.Trim();
            if (tag.IndexOf('<').Equals(-1)) return tag;
            return tag.Substring(tag.IndexOf('<') + 1, tag.IndexOf('>') - 1);
        }
        /// <summary>
        /// Seleciona o valor de uma tag
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <returns>Valor da tag</returns>
        public string SelecionaValor(string tag)
        {
            tag = tag.Trim();
            return tag.Substring(tag.IndexOf('>') + 1).Trim();
        }
        /// <summary>
        /// Seleciona o valor de uma tag e converte pra DateTime
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <returns>Valor da tag no formato DateTime</returns>
        public DateTime SelecionarValorData(string tag)
        {
            tag = tag.Trim();
            TimeZoneInfo localZone;

            switch (tag.Substring(tag.IndexOf(':') + 1, 3))
            {
                case "GMT":
                    localZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                    break;
                case "EST":
                    localZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    break;
                default:
                    localZone = null;
                    break;
            }

            var valorTratado = SelecionaValor(tag).Substring(0, 14);
            var data = DateTime.ParseExact(valorTratado, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return localZone != null ? TimeZoneInfo.ConvertTime(data, TimeZoneInfo.Local, localZone) : data;
        }
        /// <summary>
        /// Converte uma string em enum
        /// </summary>
        /// <param name="descricao">Descrição do enum</param>
        /// <param name="tipoDoEnum">Tipo do enum</param>
        /// <returns>Enum do tipo informado</returns>
        public int BuscaEnumPelaDescricao(string descricao, Type tipoDoEnum)
        {
            foreach (var field in tipoDoEnum.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute == null) continue;
                if (attribute.Description == descricao) return (int)field.GetValue(null);
            }
            return 0;
        }
        /// <summary>
        /// Converte um enumerador de string em Lançamento
        /// </summary>
        /// <param name="bloco">Enumerador de strings</param>
        /// <returns>Um novo lançamento</returns>
        private LancamentoBancario ConverteEmLancamento(string numeroDoBanco, string numeroDaAgencia,
            TipoDeMoeda tipoDeMoeda, IEnumerable<string> bloco)
        {
            var lancamento = new LancamentoBancario(numeroDoBanco, numeroDaAgencia, tipoDeMoeda);

            foreach (var linha in bloco)
            {
                switch (SelecionaChave(linha))
                {
                    case "TRNTYPE":
                        lancamento.TipoDoLancamento =
                            (TipoDoLancamento)BuscaEnumPelaDescricao(SelecionaValor(linha),
                                typeof(TipoDoLancamento));
                        break;
                    case "DTPOSTED":
                        lancamento.DataDePostagem = SelecionarValorData(linha);
                        break;
                    case "TRNAMT":
                        lancamento.Valor = decimal.Parse(SelecionaValor(linha),
                            new NumberFormatInfo { NumberDecimalSeparator = "." });
                        break;
                    case "CHECKNUM":
                        lancamento.Checknum = SelecionaValor(linha);
                        break;
                    case "MEMO":
                        lancamento.Descricao = SelecionaValor(linha);
                        break;
                    default:
                        break;
                }
            }

            return lancamento;
        }
        /// <summary>
        /// Adiciona lançamentos na transação bancária
        /// </summary>
        /// <param name="linhas">Dados dos lançamentos</param>
        /// <param name="lancamentoBancario">Transação bancária</param>
        public List<LancamentoBancario> PopulaLancamentosBancarios(string[] linhas)
        {
            if (linhas == null) throw new ArgumentNullException(nameof(linhas));

            var lancamentosBancarios = new List<LancamentoBancario>();
            string numeroDoBanco = null;
            string numeroDaAgencia = null;
            TipoDeMoeda tipoDeMoeda = 0;

            for (var i = 0; i < linhas.Length; i++)
            {
                var linha = linhas[i];

                switch (SelecionaChave(linha))
                {
                    case "STMTTRN":
                        {
                            var bloco = new List<string>();
                            linha = linhas[i++];

                            while (!linha.Trim().Equals("</STMTTRN>"))
                            {
                                bloco.Add(linha);
                                linha = linhas[i++];
                            }
                            var lancamento = ConverteEmLancamento(numeroDoBanco, numeroDaAgencia, tipoDeMoeda, bloco);
                            lancamentosBancarios.Add(lancamento);
                            break;
                        }
                    case "BANKID":
                        numeroDoBanco = SelecionaValor(linha);
                        break;
                    case "ACCTID":
                        numeroDaAgencia = SelecionaValor(linha);
                        break;
                    case "CURDEF":
                        tipoDeMoeda = (TipoDeMoeda)BuscaEnumPelaDescricao(SelecionaValor(linha), typeof(TipoDeMoeda));
                        break;
                    default:
                        break;
                }
            }

            return lancamentosBancarios;
        }

        public IEnumerable<LancamentoBancario> EnviaLancamentos(LancamentoBacarioViewModel lancamentoBacarioViewModel)
        {
            var tasks = new List<Task>();
            var lancamentosBancarios = new List<LancamentoBancario>();

            Parallel.ForEach(lancamentoBacarioViewModel.Arquivos, (arquivo) =>
            {
                string[] linhas;                
                var task = Task.Run(() =>
                {
                    using (var streamReader = new StreamReader(arquivo.InputStream))
                    {
                        var resultado = streamReader.ReadToEndAsync().Result;
                        linhas = resultado.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    }
                    lancamentosBancarios.AddRange(PopulaLancamentosBancarios(linhas));
                });

                tasks.Add(task);
            });

            Task.WaitAll(tasks.ToArray());
            return lancamentosBancarios;
        }        
    }
}