using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;


namespace ConsoleApplicationService
{
    class Program
    {
        static void Main(string[] args)
        {
            // como arrancar el servicio
            // typeof  -> como service host acepta un object, utiliza el constructor que maneja el type(cualquier tipo)
            using (ServiceHost sv = new ServiceHost(typeof(ConsoleApplicationService.Servicio))) 
            {
                sv.Open();
                Console.WriteLine("El servicio está listo");
                Console.WriteLine("Presione <Enter> para terminar");
                Console.ReadLine();
                sv.Close();
            }
        }
    }
}
