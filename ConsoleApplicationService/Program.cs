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
            using (ServiceHost sv = new ServiceHost(typeof(ConsoleApplicationService.Servicio))) // typeof  -> como service host acepta un object, utiliza el constructor que maneja el type(cualquier tipo)
            {
                sv.Open();

                Console.WriteLine("El servicio está listo");
                Console.WriteLine("Presione <Enter> para terminar");
                Console.ReadLine();

                // ya no es necesario utilizar el sv.close, por que se está utilizando el dispose implicito en el using
                sv.Close();
            }
        }
    }
}
