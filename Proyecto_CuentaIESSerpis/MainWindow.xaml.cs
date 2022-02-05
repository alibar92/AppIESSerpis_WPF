using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Principal;

namespace Proyecto_CuentaIESSerpis
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            if (!CallLogin())
            {
                Application.Current.Shutdown(-1);
            }
            InitializeComponent();
            //LogedUser.Content = System.Threading.Thread.CurrentPrincipal.Identity.Name;

        }

        private bool CallLogin()
        {
            Login login = new Login();
            login.ShowDialog();

            if (login.UserOK)
            {
                //Para trabajar con usuarios que no son de Windows, sino de la BBDD
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.UnauthenticatedPrincipal);
                //Crear una identidad a partir del usuario
                IIdentity identity = new GenericIdentity(login.usuarioTxtBox.Text, "Database");
                string[] roles = { "Docente", "Alumno" };
                GenericPrincipal credencial = new GenericPrincipal(identity, roles);
                //Asignar esta credencial a la aplicación
                System.Threading.Thread.CurrentPrincipal = credencial;

                if(roles.Equals("Docente")) { //si es docente
                    GridAlumno.Visibility = Visibility.Hidden; //se esconde el Grid del alumno
                }
                else //si el alumno
                {
                    GridDocente.Visibility = Visibility.Hidden; //se esconde el Grid docente
                }

            }
            return true;
        }
    }
}
