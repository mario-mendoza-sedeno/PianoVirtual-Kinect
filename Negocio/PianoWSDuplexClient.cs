using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Negocio.PianoWSClient;
using System.Net;

namespace Negocio
{
    public class PianoWSDuplexClient : IServicioCallback
    {
        protected InstanceContext _context;
        protected ServicioClient _proxy;
        protected Action<string> _ejecutarNotaAction;
        protected Action _iniciarSesionCompletedAction;
        protected Action _finalizarSesionCompletedAction;

        public string HostName { get; private set;}

        public bool Connected { get; set; }

        public PianoWSDuplexClient()
        {
            HostName = Dns.GetHostName();
            _context = new InstanceContext(this);
            _proxy = new ServicioClient(_context);

            _proxy.IniciarSesionCompleted += new EventHandler<IniciarSesionCompletedEventArgs>(_proxy_IniciarSesionCompleted);
            _proxy.FinalizarSesionCompleted += new EventHandler<FinalizarSesionCompletedEventArgs>(_proxy_FinalizarSesionCompleted);
            _proxy.PublicarNotaCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(_proxy_PublicarNotaCompleted);
           
        }

        public void SetEjecutarNotaAction(Action<string> ejecutarNotaAction) {
            _ejecutarNotaAction = ejecutarNotaAction;
        }

        public void SetIniciarSesionCompleted(Action iniciarSesionCompletedAction)
        {
            _iniciarSesionCompletedAction = iniciarSesionCompletedAction;
        }

        public void SetFinalizarSesionCompleted(Action finalizarSesionCompletedAction)
        {
            _finalizarSesionCompletedAction = finalizarSesionCompletedAction;
        }

        public void IniciarSesionAsync() {
            _proxy.IniciarSesionAsync();
        }

        public void FinalizarSesionAsync() { 
            _proxy.FinalizarSesionAsync();
        }

        void _proxy_IniciarSesionCompleted(object sender, IniciarSesionCompletedEventArgs e)
        {
            Connected = true;
            Console.WriteLine("Servicio iniciado ...");
            if (_iniciarSesionCompletedAction != null) {
                _iniciarSesionCompletedAction.Invoke();
            }
        }

        void _proxy_FinalizarSesionCompleted(object sender, FinalizarSesionCompletedEventArgs e)
        {
            Connected = false;
            Console.WriteLine("Servicio finalizado ...");
            if (_finalizarSesionCompletedAction != null)
            {
                _finalizarSesionCompletedAction.Invoke();
            }
        }

        public void EjecutarNota(string hostName, string nota) {
            if (_ejecutarNotaAction != null ) {
                _ejecutarNotaAction.Invoke(nota);
            }
        }

        public void PublicarNota(string nota)
        {
            _proxy.PublicarNotaAsync(HostName, nota);
        }

        void _proxy_PublicarNotaCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //  throw new NotImplementedException();
        }

        public IAsyncResult BeginEjecutarNota(string hostName, string nota, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndEjecutarNota(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
