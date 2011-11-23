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
           Console.WriteLine(DateTime.Now + " : " + mensaje);
       }
    }
}
