//Olvera Morales Miguel Angel
using System;
namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (Lenguaje lenguaje = new Lenguaje())
            {
                try
                {
                    lenguaje.Programa();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            //Lenguaje a = new Lenguaje("C:\\Users\\wachi\\OneDrive\\Escritorio\\AUTOMATAS\\Semantica\\prueba.cpp");
            //a.Programa();
            //a.cerrar();
        }

    }
}
