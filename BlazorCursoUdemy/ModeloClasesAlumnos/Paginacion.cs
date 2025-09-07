using System;
using System.Collections.Generic;
using System.Text;

namespace ModeloClasesAlumnos
{
    public class Paginacion
    {
        public int pagina { get; set; } = 1;
        public int registros { get; set; } = 4;
        public int totalPaginas { get; set; }

    }
}
