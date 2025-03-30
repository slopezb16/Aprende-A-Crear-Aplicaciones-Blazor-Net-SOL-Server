namespace ApiAlumnos.Datos
{
    public class AccesoDatos
    {
        private string _CadenaConexionSql;
        public string CadenaConexionSQL {  get => _CadenaConexionSql; }

        public AccesoDatos(string conexionSql)
        {
            if (string.IsNullOrWhiteSpace(conexionSql))
                throw new ArgumentException("La cadena de conexión no puede estar vacía.", nameof(conexionSql));

            _CadenaConexionSql = conexionSql;
        }


    }
}
