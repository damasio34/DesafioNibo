using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioNibo.Dominio
{
    public class LancamentoBancario
    {
        #region [ Construtores ]
        
        protected LancamentoBancario() { }
        public LancamentoBancario(string numeroDoBanco, string numeroDaAgencia, TipoDeMoeda tipoDeMoeda)
        {
            NumeroDoBanco = numeroDoBanco;
            NumeroDaAgencia = numeroDaAgencia;
            TipoDeMoeda = tipoDeMoeda;

            this.Id = Guid.NewGuid();
        }

        #endregion

        #region [ Propriedades ]        

        [Key]
        public Guid Id { get; protected set; }
        [Index("LancamentoUnico", 1, IsUnique = true)]
        [MaxLength(4)]
        public string NumeroDoBanco { get; protected set; }
        [Index("LancamentoUnico", 2, IsUnique = true)]
        [MaxLength(13)]
        public string NumeroDaAgencia { get; protected set; }
        [Index("LancamentoUnico", 3, IsUnique = true)]
        [MaxLength(11)]
        public string Checknum { get; set; }
        public TipoDeMoeda TipoDeMoeda { get; protected set; }
        public TipoDoLancamento TipoDoLancamento { get; set; }
        public decimal Valor { get; set; }        
        public string Descricao { get; set; }
        public DateTime? DataDePostagem { get; set; }

        #endregion
    }
}