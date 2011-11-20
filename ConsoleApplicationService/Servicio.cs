using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Negocio;
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

                //Parallel.ForEach(Usuarios, usuario =>
                foreach (IServicioCallBack usuario in Usuarios)
                {
                    if (((ICommunicationObject)usuario).State == CommunicationState.Opened)
                    {
                        Negocio.Log.EscribirLog(nota);
                        
                        usuario.EjecutarNota(hostName, nota);
                        //Negocio.Logs.EscribirLogParticipante(mensajeParticipante, publicador);
                    }
                    else
                        Usuarios.Remove(usuario);
                }
                //});

            }
            catch (AggregateException aex)
            {
              //  Negocio.Logs.EscribirLogErrores(aex);
            }
            catch (Exception ex)
            {
              //  Negocio.Logs.EscribirLogErrores(ex);
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

                    //para evitar el error de timeout, falta probarlo....
                    ((IContextChannel)callback).OperationTimeout = new TimeSpan(0, 0, 240);

                    //para evitar que los mensajes se queden en el buffer y mejore la salida inmeditamente
                    ((IContextChannel)callback).AllowOutputBatching = false;
                    Negocio.Log.EscribirLog("Sesion Iniciada");

                  
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
                    Negocio.Log.EscribirLog("Sesion Finalizada");
                   
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
