using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using PianoWPFClient.PianoWSClient;

namespace PianoWPFClient
{
    public class PianoWSDuplexClient : IServicioCallback
    {
        protected InstanceContext _context;
        protected ServicioClient _proxy;
        protected Action<string> _ejecutarNotaAction;

        public int Id {get; set;}


        public PianoWSDuplexClient(int id)
        {
            Id = id;
            _context = new InstanceContext(this);
            _proxy = new ServicioClient(_context);

            _proxy.IniciarSesionCompleted += new EventHandler<IniciarSesionCompletedEventArgs>(_proxy_IniciarSesionCompleted);
            _proxy.FinalizarSesionCompleted += new EventHandler<FinalizarSesionCompletedEventArgs>(_proxy_FinalizarSesionCompleted);
            _proxy.PublicarNotaCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(_proxy_PublicarNotaCompleted);
        }

        public void SetEjecutarNotaAction(Action<string> ejecutarNotaAction) {
            _ejecutarNotaAction = ejecutarNotaAction;
        }

        public void IniciarSesionAsync() {
            _proxy.IniciarSesionAsync();
        }

        public void FinalizarSesionAsync() { 
            _proxy.FinalizarSesionAsync();
        }

        void _proxy_IniciarSesionCompleted(object sender, IniciarSesionCompletedEventArgs e)
        {
            Console.WriteLine("Servicio iniciado ...");
        }

        void _proxy_FinalizarSesionCompleted(object sender, FinalizarSesionCompletedEventArgs e)
        {
            Console.WriteLine("Servicio finalizado ...");
        }

        public void EjecutarNota(string nota) {
            if (_ejecutarNotaAction != null) {
                _ejecutarNotaAction.Invoke(nota);
            }
            
        }

        public void PublicarNota(string nota)
        {
            _proxy.PublicarNotaAsync(nota);
        }

        void _proxy_PublicarNotaCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //  throw new NotImplementedException();
        }

        public IAsyncResult BeginEjecutarNota(string nota, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndEjecutarNota(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
