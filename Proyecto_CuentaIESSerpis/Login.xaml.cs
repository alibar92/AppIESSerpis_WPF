using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Security.Cryptography;
using System.Data.Entity.Validation;

namespace Proyecto_CuentaIESSerpis
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public bool UserOK { get; set; }
        public char PasswordChar { get; set; }
        public Usuarios UsuarioLogueado { get; set; }


        public Login()
        {
            InitializeComponent();
            GridRegistro.Visibility = Visibility.Hidden; //se oculta el grid de registro
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

        private void click_login(object sender, RoutedEventArgs e)
        {
            UserOK = false;


            //creo la instancia de la BD
            BDSerpisEntities1 basededatos = new BDSerpisEntities1();

            //creo la variable usuario y recorrendo el campo User de la tabla Usuarios busco un campo igual al introducido 
            var usuario = basededatos.Usuarios.SingleOrDefault(
                us => us.User.Equals(
                    usuarioTxtBox.Text, StringComparison.InvariantCultureIgnoreCase)  //no caseSensitive
                );

            //si el usuario existe como User en mi tabla Usuarios y la Password corresponde con la de HashPassword 
            if (usuario != null && (usuario.HashPassword).Equals(CalcHash(passwordBox.Password.ToUpper())))
            {
                if (usuario.Role == null) //si no hay rol asignado, es decir que el usuario no esta dado de alta en el sistema
                {
                    UserOK = false;  //no se puede loguear
                    usuarioTxtBox.Text = null; //se limpian el TextBox y la Password
                    passwordBox.Password = null;
                    MessageBox.Show("Este usuario aún no está dado de alta en el sistema.");
                    return;
                }
                MessageBox.Show("Usuario y contraseña correctos.");

                UserOK = true;  //UserOK 
                UsuarioLogueado = usuario; //se asigna el valor al usuario logueado
            }
            else
            {
                MessageBox.Show("Usuario o contraseña no válidos");
                usuarioTxtBox.Text = null; //se limpian el TextBox y la Password
                passwordBox.Password = null;

                return;
            }
            DialogResult = true;
        }

        private void click_registrarse(object sender, RoutedEventArgs e)
        {
            mostrarRegistro();
        }

        //metodo inverso al anterior 
        private void click_iniciarSesion(object sender, RoutedEventArgs e)
        {
            mostrarLogin();
        }

        private void mostrarRegistro()
        {
            GridLogin.Visibility = Visibility.Hidden; //se desactiva la pantalla de Login
            GridRegistro.Visibility = Visibility.Visible; //y se activa la de Registro
            usuarioTxtBox.Text = null; //se limpian el TextBox y la Password de login
            passwordBox.Password = null;
            MessageBox.Show("Bienvenid@ a la pantalla de registro!");
        }
        private void mostrarLogin()
        {
            GridLogin.Visibility = Visibility.Visible;
            GridRegistro.Visibility = Visibility.Hidden;
            usuarioTxtBoxReg.Text = null; //se limpian el TextBox y la Password de registro
            passwordBoxReg.Password = null;
            MessageBox.Show("Bienvenid@ a la pantalla de Login!\nRecuerda que, si acabas de registrarte," +
               " necesitas esperar\na que el sistema te de de alta.");
        }
        private void click_registro(object sender, RoutedEventArgs e)
        {
            if(usuarioTxtBoxReg.Text.Equals("") || passwordBoxReg.Password.Equals(""))
            {
                MessageBox.Show("ERROR! Tienes que introducir un NI y una contraseña.");
                usuarioTxtBoxReg.Text = null; //se limpian el TextBox y la Password de registro
                passwordBoxReg.Password = null;
            }
            else
            {
                try  //NO LOGRO GUARDAR LOS CAMBIOS. ME SALTAN EXC SOBRE LAS ENTIDADES DE MI BD 
                {
                    BDSerpisEntities1 db = new BDSerpisEntities1();
                        var sysUser = new Usuarios
                        {
                            User = usuarioTxtBoxReg.Text,
                            HashPassword = CalcHash(passwordBox.Password.ToUpper())
                        };

                        db.Usuarios.Add(sysUser); //se anyade el nuevo usuario en la tabla Usuarios
                        db.SaveChanges();

                    MessageBox.Show("Usario registrado correctamente.");
                    //volvemos a mostrar el Login
                    mostrarLogin();
                }
                catch (DbEntityValidationException ex) //exc si por ejemplo ya existe un usuario con esta PK
                {
                    MessageBox.Show(ex.Message);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                usuarioTxtBoxReg.Clear(); //se limpian los campos
                passwordBoxReg.Clear();
            }
        }
    }
}
