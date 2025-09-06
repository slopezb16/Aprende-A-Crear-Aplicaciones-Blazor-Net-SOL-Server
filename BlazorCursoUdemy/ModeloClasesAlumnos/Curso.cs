using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloClasesAlumnos
{
    public class Curso
    {
        public int Id {  get; set; }
        public string NombreCurso { get; set; }
        public List<Precio> ListaPrecios {  get; set; }
    }
}
