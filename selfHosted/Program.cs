using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace selfHosted
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://10.16.144.7:9095")) //<--comenzar con WebApp.Start. Esto es Owin
            {
                Console.WriteLine("El web server se está ejecutando en el puerto 9095.");
                Console.WriteLine("Oprima una tecla para finalizar...");
                Console.ReadLine();
            }
        }
    }
}
