using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Threading;

namespace Proyecto_CuentaIESSerpis
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BDSerpisEntities1 basededatos = new BDSerpisEntities1(); //creo la instancia de la BD
        Login login = new Login(); //instancio la clase Login para acceder a sus metodos y atributos

        public MainWindow()
        {
            CallLogin();
        }

        private bool CallLogin()
        {
            login.ShowDialog();

            if (login.UserOK)
            {
                //el usuario logueado lo obtenemos de la clase Login
                Usuarios usuarioLogueado = login.UsuarioLogueado;

                //TODO Crear rol de Administrador para gestionar las altas y matriculas de profesorado y alumnado del centro

                if (usuarioLogueado.Role == "Alumno") //si el rol del usuario logueado es alumno
                {    
                    InitializeComponent();  //inicializo los componentes
                    GridDocente.Visibility = Visibility.Hidden;  //se esconde el Grid del docente
                    notasAlumnoDataGrid.Visibility = Visibility.Hidden;

                    //busco el alumno que tenga NIA igual al User del usuario logueado
                    Alumno alumno = basededatos.Alumno.SingleOrDefault(
                       alu => alu.NIA.Equals(
                        usuarioLogueado.User, StringComparison.InvariantCultureIgnoreCase)  //no caseSensitive
                    );

                    //configuro la label de saludo inicial para que aparezca el nombre del alumno logueado
                    saludoAlumno.Content = "Hola " + alumno.Nombre;
                }
                else    //si es docente
                {
                    InitializeComponent(); //inicializo los componentes
                    GridAlumno.Visibility = Visibility.Hidden; //se esconde el Grid alumno

                    cambioNotasStackPanel.Visibility = Visibility.Hidden; //el stackpanel está oculto

                    //busco el docente que tenga NID igual al User del usuario logueado
                    Docente docente = basededatos.Docente.SingleOrDefault(
                        doc => doc.NID.Equals(
                            usuarioLogueado.User, StringComparison.InvariantCultureIgnoreCase));

                    //configuro la label de saludo inicial para que aparezca el nombre del docente logueado
                    saludoDocente.Content = "Hola " + docente.Nombre_;

                    //busco la primera fila de la tabla Matricula donde el campo NID corresponda con el de Docente
                    Matricula matricula = (from matri in basededatos.Matricula where 
                        matri.NID == docente.NID select matri).FirstOrDefault();
                    
                    //asigno a la label la asignatura del docente para que aparezca en mi Grid
                    asignaturaDocente.Content = matricula.Asignatura;
                    actualizarTabla(docente);
                }
            } else //si el Login no es OK
            {
                Application.Current.Shutdown(-1);
            }
            return login.UserOK;
        }

        //metodo de consulta de las notas del alumno
        private void click_consultarNotas(object sender, RoutedEventArgs e)
        {
            //cojo el item del combobox y lo convierto a String (no me lo cogia bien seleccionando el content directamente
            ComboBoxItem typeItem = (ComboBoxItem)asignaturaAlumno.SelectedItem;
            string asignaturaElegida = typeItem.Content.ToString();

            //en la tabla Matricula se busca el alumno con NIA logueado y se muestra la asignatura según el item del combobox
            var matriculaAsignatura = from m in basededatos.Matricula
                                      where m.NIA == login.UsuarioLogueado.User &&
                                      m.Asignatura == asignaturaElegida
                                      select new
                                      {
                                          m.NIA,
                                          m.Alumno.Nombre,
                                          m.Alumno.Apellido,
                                          m.Alumno.Curso,
                                          m.Nota1,
                                          m.Nota2,
                                          m.Nota3
                                      };

            //Matricula matri = (Matricula)matriculaAsignatura;
            notasAlumnoDataGrid.Visibility = Visibility.Visible;
            notasAlumnoDataGrid.ItemsSource = matriculaAsignatura.ToList();

            if (notasAlumnoDataGrid.Items.IsEmpty) //si despues de mostrar la matricula, los campos estan vacios
            {
                MessageBox.Show("Aún no estás matriculado en esta asignatura.");
            }
        }

        private void click_EditarNotas(object sender, RoutedEventArgs e)
        {
            Usuarios usuarioLogueado = login.UsuarioLogueado;
            cambioNotasStackPanel.Visibility = Visibility.Visible; //el stackpanel se muestra cuando pulsamos 'editar notas'

            if (listaAlumnos ==  null) //si no hay datos en la tabla
            {
                MessageBox.Show("ERROR. No hay datos a editar.");
                cambioNotasStackPanel.Visibility = Visibility.Hidden; //el stackpanel se oculta
                editarNotasBtn.Content = "Editar Notas"; //el boton vuelve a ser el de antes
            } 
            else if (editarNotasBtn.Content.Equals("Editar Notas")) //si el boton esta en modalidad Editar
            {
                //cambioNotasStackPanel.Visibility = Visibility.Visible; //el stackpanel se muestra
                MessageBox.Show("Para editar las notas, rellena los campos de NIA y de las \n" +
                    "notas que quieres actualizar.\nPulsa 'Guardar Cambios' una vez actualizado.");

                editarNotasBtn.Content = "Guardar Cambios"; //el boton cambia a modalidad Guardar
            }
            else //el boton ya tiene el contenido "Guardar Cambios" 
            {
                if (niaTextBox.Text.Equals("")) //si el nia no se ha introducido, salta el mensaje de error 
                {
                    MessageBox.Show("ERROR. El campo NIA no puede estar vacío.");
                    clearTextBoxes(); //se limpian todos los textboxes
                }
                else
                {
                    var alumno = buscarAlumnoPorNIA();
                    if (alumno != null) //si se ha encontrado el alumno buscado
                    {
                        try //para el control del formato introducido en los textBoxes de las notas (int)
                        {
                            //se settean las notas con los eventuales valores modificados (los campos ya nulos pueden seguir siendo nulos)
                            alumno.Nota1 = Convert.ToInt32(nota1TextBox.Text);
                            alumno.Nota2 = Convert.ToInt32(nota2TextBox.Text);
                            alumno.Nota3 = Convert.ToInt32(nota2TextBox.Text);
                            Matricula matricula = (from matri in basededatos.Matricula
                                                       where matri.NIA == alumno.NIA && matri.NID == usuarioLogueado.User
                                                   select matri).SingleOrDefault();
                            
                            //Actualizo las notas del alumno
                            matricula.Nota1 = alumno.Nota1;
                            matricula.Nota2 = alumno.Nota2;
                            matricula.Nota3 = alumno.Nota3;

                            basededatos.SaveChanges();
                            Docente docente = basededatos.Docente.SingleOrDefault(
                            doc => doc.NID.Equals(
                            usuarioLogueado.User, StringComparison.InvariantCultureIgnoreCase));

                            //Llamo al metodo para actualizar la tabla
                            actualizarTabla(docente);

                            editarNotasBtn.Content = "Editar Notas"; //el boton vuelve a ser el de antes

                            MessageBox.Show("Cambios realizados correctamente");
                            cambioNotasStackPanel.Visibility = Visibility.Hidden; //el stackpanel se esconde
                        }
                        catch (FormatException ex)
                        {
                            MessageBox.Show(ex.Message);
                            editarNotasBtn.Content = "Editar Notas"; //el boton vuelve a ser el de antes 
                            clearTextBoxes(); //se limpian todos los textboxes
                            cambioNotasStackPanel.Visibility = Visibility.Hidden; //el stackpanel se esconde
                        }
                    }
                    else //el metodo buscarAlumnoPorNia nos ha devuelto null
                    {
                        MessageBox.Show("El alumno buscado no existe. Vuelve a escribir su NIA.");
                        //cambioNotasStackPanel.Visibility = Visibility.Visible;
                        clearTextBoxes(); //se limpian todos los textboxes
                    }
                }
                clearTextBoxes();
            }
        }

        private void clearTextBoxes()
        {
            niaTextBox.Clear();
            nota1TextBox.Clear();
            nota2TextBox.Clear();
            nota3TextBox.Clear();
        }

        //metodo que devuelve el alumno buscado
        public Matricula buscarAlumnoPorNIA()
        {
            BDSerpisEntities1 basededatos = new BDSerpisEntities1();

            Matricula aluBuscado = basededatos.Matricula.FirstOrDefault(
            m => m.NIA.Equals(niaTextBox.Text, StringComparison.InvariantCultureIgnoreCase));

            return aluBuscado;
        }

        private void actualizarTabla (Docente docente)
        {

            Matricula matricula = (from matri in basededatos.Matricula
                                   where matri.NID == docente.NID
                                   select matri).FirstOrDefault();

            //LO QUE QUIERO ES JUNTAR LAS DOS TABLAS (Alumno y Matricula ya están relacionadas por el NIA) 
            //Y Mostrar en el DataGrid nombre, apellido, curso y notas del alumno.
            //Creo un nuevo obj que contenga todos los campos que quiero mostrar
            var tablaNotasAlumno = from m in basededatos.Matricula
                                   where m.NID == docente.NID
                                   select new
                                   {
                                       m.Alumno.NIA,
                                       m.Alumno.Nombre, //puedo acceder a Alumno desde Matricula porque las dos tablas están relacionadas
                                       m.Alumno.Apellido,
                                       m.Alumno.Curso,
                                       m.Nota1,
                                       m.Nota2,
                                       m.Nota3
                                   };
            //asigno este obj a mi DataGrid
            listaAlumnos.ItemsSource = tablaNotasAlumno.ToList();
        }

        private void click_cerrarSesion(object sender, RoutedEventArgs e)
        {
            //se cierra y reinicia la app 
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}