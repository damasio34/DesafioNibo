using System;
using System.ComponentModel;

namespace DesafioNibo.Dominio
{
    public class LancamentoBancario
    {
        public Guid Id { get; protected set; }
        protected LancamentoBancario() { }
        public LancamentoBancario(string numeroDoBanco, string numeroDaAgencia, TipoDeMoeda tipoDeMoeda)
        {
            NumeroDoBanco = numeroDoBanco;
            NumeroDaAgencia = numeroDaAgencia;
            TipoDeMoeda = tipoDeMoeda;

            this.Id = Guid.NewGuid();
        }

        public string NumeroDoBanco { get; protected set; }
        public string NumeroDaAgencia { get; protected set; }
        public TipoDeMoeda TipoDeMoeda { get; protected set; }
        public TipoDoLancamento TipoDoLancamento { get; set; }
        public decimal Valor { get; set; }
        public string Checknum { get; set; }
        public string Descricao { get; set; }
        public DateTime? DataDePostagem { get; set; }
    }

    public enum TipoDeMoeda
    {
        [Description("BLR")]
        Real = 1,
        [Description("EUR")]
        Euro = 2,
        [Description("USD")]
        Dolar = 3
    }

    public enum TipoDoLancamento
    {
        [Description("DEBIT")]
        Debito = 1,
        [Description("CREDIT")]
        Credito = 2,
        [Description("OTHER")]
        Outro = 3
    }
}