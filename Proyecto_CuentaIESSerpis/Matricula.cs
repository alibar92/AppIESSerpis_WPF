//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_CuentaIESSerpis
{
    using System;
    using System.Collections.Generic;
    
    public partial class Matricula
    {
        public int Codigo { get; set; }
        public string NIA { get; set; }
        public string NID { get; set; }
        public string Asignatura { get; set; }
        public Nullable<int> Nota1 { get; set; }
        public Nullable<int> Nota2 { get; set; }
        public Nullable<int> Nota3 { get; set; }
    
        public virtual Alumno Alumno { get; set; }
        public virtual Docente Docente { get; set; }
    }
}