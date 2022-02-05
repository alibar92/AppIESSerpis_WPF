using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Proyecto_CuentaIESSerpis
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public bool UserOK { get; set; }
        public char PasswordChar { get; set; }

        public Login()
        {
            InitializeComponent();
            GridRegistro.Visibility = Visibility.Hidden;
            CalcHash(passwordBox.Password);
        }
        private string CalcHash(string contra)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] data = enc.GetBytes(contra);
            byte[] result;
            SHA256CryptoServiceProvider sha = new SHA256CryptoServiceProvider();
            result = sha.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                // Convertimos los valores en hexadecimal
                // cuando tiene una cifra hay que rellenarlo con cero
                // para que siempre ocupen dos dígitos.
                if (result[i] < 16)
                {
                    sb.Append("0");
                }
                sb.Append(result[i].ToString("x"));
            }
            return sb.ToString().ToUpper();

        }

    }
}
