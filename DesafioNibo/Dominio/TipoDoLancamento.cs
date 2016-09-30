using System.ComponentModel;

namespace DesafioNibo.Dominio
{
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