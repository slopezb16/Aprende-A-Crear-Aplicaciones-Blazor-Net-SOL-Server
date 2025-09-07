using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServer.Shared
{

    public class PaginasAux
    {

        public string Literal { get; set; }
        public int Pagina { get; set; }
        public bool Enabled { get; set; } = true;
        public bool Activa { get; set; } = false;

        public PaginasAux(int pagina, bool enabled, string literal)
        {
            Pagina = pagina;
            Enabled = enabled;
            Literal = literal;
        }

    }
}
