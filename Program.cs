//Olvera Morales Miguel Angel
using System;
namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lenguaje lenguaje = new Lenguaje())
                {
                    lenguaje.Programa();
                }
                //Lenguaje a = new Lenguaje("C:\\Users\\wachi\\OneDrive\\Escritorio\\AUTOMATAS\\Semantica\\prueba.cpp");
                //a.Programa();
                //a.cerrar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
