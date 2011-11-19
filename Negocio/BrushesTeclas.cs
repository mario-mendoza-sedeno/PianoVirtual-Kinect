using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace PianoWPFClient.Utils
{
    public sealed class BrushesTeclas
    {
        // borde
        public static SolidColorBrush BorderBrush { get; set; }

        public static SolidColorBrush Negro { get; set; }

        public static SolidColorBrush Blanco { get; set; }

        static BrushesTeclas()
        {
             BorderBrush = new SolidColorBrush(Colors.Black);
             Negro = new SolidColorBrush(Colors.Black);
             Blanco = new SolidColorBrush(Colors.WhiteSmoke);

        }
    }
}
