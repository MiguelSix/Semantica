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
                Lenguaje a = new Lenguaje("C:\\Users\\wachi\\OneDrive\\Escritorio\\AUTOMATAS\\Semantica\\prueba.cpp");

                a.Programa();
                
                /*while(!a.FinArchivo())
                {
                    a.NextToken();
                }*/
                a.cerrar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine("Fin de compilacion");
                //Console.WriteLine("Error de compilacion");
            }
        }
    }
}
