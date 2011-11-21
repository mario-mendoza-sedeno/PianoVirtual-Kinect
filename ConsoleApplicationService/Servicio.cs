using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ConsoleApplicationService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Servicio" in both code and config file together.
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
                foreach (IServicioCallBack usuario in Usuarios)
                {
                    if (((ICommunicationObject)usuario).State == CommunicationState.Opened)
                    {
                        Log.EscribirLog(nota);
                        usuario.EjecutarNota(hostName, nota);
                    }
                    else
                        Usuarios.Remove(usuario);
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
                if (Usuarios.Contains(callback) == false)
                {
                    Usuarios.Add(callback);

                    //para evitar el error de timeout
                    ((IContextChannel)callback).OperationTimeout = new TimeSpan(0, 0, 240);

                    //para evitar que los mensajes se queden en el buffer y mejore la salida inmeditamente
                    ((IContextChannel)callback).AllowOutputBatching = false;
                    Log.EscribirLog("Sesion Iniciada");

                  
                }
                return true;

            }
            catch (Exception ex)
            {
                //Guardar en un log, la causa por la cual no pudo inicar sesión
               
                return false;
            }

        }

        public bool FinalizarSesion()
        {
            try
            {
                IServicioCallBack callback = OperationContext.Current.GetCallbackChannel<IServicioCallBack>();
                if (Usuarios.Contains(callback) == true)
                {
                    Usuarios.Remove(callback);
                    Log.EscribirLog("Sesion Finalizada");
                   
                }
                return true;
            }
            catch (Exception ex)
            {
                //TODO: Escribir un log, con la causa por la cual no pudo finalizar sesión
            
                return false;
            }
        }
    }
}
