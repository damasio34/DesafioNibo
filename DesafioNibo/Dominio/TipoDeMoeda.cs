using System.ComponentModel;

namespace DesafioNibo.Dominio
{
    public enum TipoDeMoeda
    {
        [Description("BLR")]
        Real = 1,
        [Description("EUR")]
        Euro = 2,
        [Description("USD")]
        Dolar = 3
    }
}