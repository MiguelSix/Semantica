//Olvera Morales Miguel Angel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Semantica
{
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log): base(mensaje)
        {
            /*
            Console.WriteLine();
            Console.WriteLine(mensaje);
            log.WriteLine();
            */
            log.WriteLine("\n" + mensaje);
        }
    }
}