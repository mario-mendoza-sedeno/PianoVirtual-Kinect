using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negocio
{
   public class Log
    {
       public static void EscribirLog(string mensaje)
       {
           Console.WriteLine("Hora:             " + DateTime.Now);
           Console.WriteLine("------------------" + DateTime.Now);
           Console.WriteLine("Nota:             " + mensaje);
           Console.WriteLine("");
           Console.WriteLine("");
           Console.WriteLine("");
           

       }

    }
}
