using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ConsoleApplicationService
{
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
    public class Servicio : IServicio
    {

        #region Campos
        //Coleccion para almacenar a los usuarios conectados
        SynchronizedCollection<IServicioCallBack> Usuarios = new SynchronizedCollection<IServicioCallBack>();
        #endregion


        public void PublicarNota(string hostName, string nota)
        {
            try
            {
                IServicioCallBack callback = OperationContext.Current.GetCallbackChannel<IServicioCallBack>();
                string callBackSessionId = ((IContextChannel)callback).SessionId;
                Log.EscribirLog(hostName + " -> PublicarNota() " + nota);
                foreach (IServicioCallBack usuario in Usuarios.Where(user => (!((IContextChannel)user).SessionId.Equals(callBackSessionId))))
                {
                    if (((ICommunicationObject)usuario).State == CommunicationState.Opened && Usuarios.Contains(usuario))
                    {
                        Log.EscribirLog("-> " + ((IContextChannel)usuario).SessionId + ".EjecutarNota() ");
                        usuario.EjecutarNota(hostName, nota);
                    }
                    else
                    {
                        Usuarios.Remove(usuario);
                    }
                }
            }
            catch (AggregateException aex)
            {
                Console.WriteLine(aex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

        }

        public bool IniciarSesion()
        {
            try
            {
                IServicioCallBack callback = OperationContext.Current.GetCallbackChannel<IServicioCallBack>();
                IContextChannel contextChannel = (IContextChannel)callback;
                if (!Usuarios.Contains(callback))
                {
                    Usuarios.Add(callback);

                    //para evitar el error de timeout
                    contextChannel.OperationTimeout = new TimeSpan(0, 0, 240);

                    //para evitar que los mensajes se queden en el buffer y mejore la salida inmeditamente
                    contextChannel.AllowOutputBatching = false;
                    Log.EscribirLog("Sesion Iniciada");

                }
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }

        }

        public bool FinalizarSesion()
        {
            try
            {
                IServicioCallBack callback = OperationContext.Current.GetCallbackChannel<IServicioCallBack>();
                if (Usuarios.Contains(callback))
                {
                    Usuarios.Remove(callback);
                    Log.EscribirLog("Sesion Finalizada");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
