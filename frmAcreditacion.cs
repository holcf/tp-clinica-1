﻿using Clinica.Datos;
using Clinica.Entidades;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace clinica
{
    public partial class frmAcreditacion : Form
    {
        private Form formOrigen;
        public frmAcreditacion(Form formOrigen)
        {
            InitializeComponent();
            this.formOrigen = formOrigen;
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
            formOrigen.Show();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmAcreditacion_Load(object sender, EventArgs e)
        {
            cbxPaciente.SelectedIndex = -1;

             rbtEfectivo.Enabled = false;
            rbtTarjeta.Enabled = false;
            rbtAdeudado.Enabled = false;
            rbtPagado.Enabled = false;
            lblCoberturaPaciente.Enabled = false;
            lblMonto.Enabled = false;

            CargarPacientes();
            CargarListaEstudios();
            CargarTurnos(0);
        }

        public void CargarPacientes()
        {
            cbxPaciente.DataSource = null;
            cbxPaciente.Items.Clear();
            cbxPaciente.Text = "";
            // Asignar la lista de coberturas al ComboBox
            cbxPaciente.DataSource = Clinica.Clinica.ObtenerPacientes();
            // Especificar qué propiedad del KeyValuePair se debe mostrar en el ComboBox (en este caso, el nombre)
            cbxPaciente.DisplayMember = "Value";
            cbxPaciente.SelectedIndex = -1;
        }

        private void CargarListaEstudios()
        {
            cbxEstudios.Items.Clear();
            cbxEstudios.Text = "";

            MySqlConnection sqlCon = new MySqlConnection();
            try
            {
                string query;
                sqlCon = Conexion.getInstancia().CrearConexion();
                query = "select id, Descripcion from Estudio;";
                MySqlCommand comando = new(query, sqlCon)
                {
                    CommandType = CommandType.Text
                };
                sqlCon.Open();

                MySqlDataReader reader;
                reader = comando.ExecuteReader();

                if (reader.HasRows)
                {
                    List<KeyValuePair<int, string>> estudios = new();

                    while (reader.Read())
                    {
                        // Obtener el ID y el nombre de la cobertura
                        int id = reader.GetInt32(0);
                        string descripcion = reader.GetString(1);

                        // Crear un objeto de KeyValuePair con el ID y el nombre de la cobertura
                        KeyValuePair<int, string> estudio = new(id, descripcion);
                        estudios.Add(estudio);

                    }
                    // Asignar la lista de coberturas al ComboBox
                    cbxEstudios.DataSource = estudios;
                    // Especificar qué propiedad del KeyValuePair se debe mostrar en el ComboBox (en este caso, el nombre)
                    cbxEstudios.DisplayMember = "Value";
                    cbxEstudios.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("No hay datos de Estudios");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
        }

        private void CargarTurnos(int idPaciente)
        {
            lbxTurnos.DataSource = null;
            lbxTurnos.Items.Clear();

            //int filtroEstudioId = 0;

            /*if (cbxEstudios.SelectedIndex != -1)
            {
                filtroEstudioId = ((KeyValuePair<int, string>)cbxEstudios.SelectedItem!).Key;
            }*/


            MySqlConnection sqlCon = new MySqlConnection();
            try
            {


                string query = "select Turno.id, Turno.Fecha, Turno.Hora, Estudio.Descripcion, " +
                    "LugarDeAtencion.Descripcion,  TurnoStatus " +
                    "from Turno inner join lugardeatencion on Turno.LugarDeAtencion_id = Lugardeatencion.id " +
                    $"inner join estudio on Turno.Estudio_id = estudio.id where Turno.Paciente_id = {idPaciente}";


                //query += $" where Turno.Fecha >= '{dtpFechaDesde.Value:yyyy-MM-dd}'";
                //query += $" and Turno.Fecha <= '{dtpFechaHasta.Value:yyyy-MM-dd}'";

                /*if (cbxHoraDesde.SelectedIndex != -1)
                {
                    query += $" and Turno.Hora >= '{cbxHoraDesde.Text}'";
                }
                if (cbxHoraHasta.SelectedIndex != -1)
                {
                    query += $" and Turno.Hora <= '{cbxHoraHasta.Text}'";
                }

                if (filtroEstudioId > 0)
                {
                    query += $" and estudio.id = {filtroEstudioId}";
                }*/




                query += " ORDER BY Turno.Fecha, Turno.Hora;";

                sqlCon = Conexion.getInstancia().CrearConexion();

                MySqlCommand comando = new(query, sqlCon)
                {
                    CommandType = CommandType.Text
                };
                sqlCon.Open();

                MySqlDataReader reader;
                reader = comando.ExecuteReader();

                if (reader.HasRows)
                {
                    List<KeyValuePair<int, string>> turnos = new();

                    while (reader.Read())
                    {
                        // Obtener el ID y el nombre de la cobertura
                        int id = reader.GetInt32(0);
                        string fecha = reader.GetDateTime(1).ToString("dd/MM/yyyy");
                        string hora = reader.GetTimeSpan(2).ToString();
                        string estudioDescripcion = reader.GetString(3);
                        string lugarDescripcion = reader.GetString(4);
                        int turnoStatus = reader.GetInt32(5);

                        string turnoStatusDescripcion = turnoStatus == (int)TurnoStatus.Disponible ? "Disponible" : "Ocupado";

                        string descripcionTurno = $"{fecha} - {hora} - {lugarDescripcion} -  {estudioDescripcion} - {turnoStatusDescripcion}";

                        // Crear un objeto de KeyValuePair con el ID y el nombre de la cobertura
                        KeyValuePair<int, string> turno = new(id, descripcionTurno);
                        turnos.Add(turno);

                    }

                    // Asignar la lista de coberturas al ComboBox
                    lbxTurnos.DataSource = turnos;
                    // Especificar qué propiedad del KeyValuePair se debe mostrar en el ComboBox (en este caso, el nombre)
                    lbxTurnos.DisplayMember = "Value";
                    lbxTurnos.SelectedIndex = 0;
                }
                else
                {
                    //MessageBox.Show("No hay datos de Turnos");
                    lbxTurnos.DataSource = null;
                    lbxTurnos.Items.Clear();
                    lbxTurnos.Items.Add("No hay turnos disponibles con los criterios seleccionados.");
                    // TODO: ojo, si se deja eso hay que evitar que se pueda elegir el turno
                    // Otra opción es dejar el ListBox vacío y oculto, y mostrar un mensaje en un Label
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sqlCon.State == ConnectionState.Open)
                {
                    sqlCon.Close();
                }
            }
        }

        private void cbxPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPaciente.SelectedIndex != -1)
            {

                CargarTurnos(((KeyValuePair<int, string>)cbxPaciente.SelectedItem!).Key);
                lblCoberturaPaciente.Text = Clinica.Clinica.ObtenerCobertura(((KeyValuePair<int, string>)cbxPaciente.SelectedItem!).Key);
                lblCoberturaPaciente.Enabled = true;
            }
            else
            {
                lblCoberturaPaciente.Text = "";
            }
        }

        private void cbxEstudios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPaciente.SelectedIndex != -1 && cbxEstudios.SelectedIndex != -1)
            {
                int idPaciente = ((KeyValuePair<int, string>)cbxPaciente.SelectedItem!).Key;
                int idEstudio = ((KeyValuePair<int, string>)cbxEstudios.SelectedItem!).Key;
                int? monto = Clinica.Clinica.ObtenerMonto(idEstudio, idPaciente);

                if (monto != null)
                {
                    lblMonto.Text = monto.ToString();
                    lblMonto.Enabled = true;
                    if (monto > 0)
                    {
                        rbtEfectivo.Enabled = true;
                        rbtTarjeta.Enabled = true;
                        rbtAdeudado.Enabled = true;
                        rbtPagado.Enabled = true;
                    }
                }
                else
                {
                    lblMonto.Text = "Sin datos";
                }

            }

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }
}