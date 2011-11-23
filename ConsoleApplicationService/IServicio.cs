using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ConsoleApplicationService
{

    [ServiceContract(CallbackContract = typeof(IServicioCallBack))]
    public interface IServicio
    {

         //método para iniciar las sesiones
        [OperationContract]
        bool IniciarSesion();


        //método para expulsar a los usuarios
        [OperationContract]
        bool FinalizarSesion();


        [OperationContract(IsOneWay=true)]
        void PublicarNota(string hostName, string nota);
    }

    [ServiceContract]
    public interface IServicioCallBack
    {
        [OperationContract(IsOneWay=true)]
        void EjecutarNota(string hostName, string nota);
    }

}
