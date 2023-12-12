using System;
using System.IO.Ports;
using System.Windows.Forms;
using System.Management;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Runtime.InteropServices;



namespace ComputerToArduino
{
    public partial class Form1 : Form

    {

        char[] relay = new char[4];
        string relays;


        bool isConnected = false;
        String[] ports;
        SerialPort port;
        string valorTextBox;
        //private int contadors = 6;
        bool selecionado_de_contador;

        bool selecionado_de_boton;
        bool estado_de_boton1=false;
        bool estado_de_boton2 = false;

        private Timer myTimer;
        private int contador = 0;

       



        public Form1()
        {
            InitializeComponent();
            disableControls();
            getAvailableComPorts();
            relay[0] = (char)0;
            relay[1] = (char)0;
            relay[2] = (char)0;
            relay[3] = (char)0;
            relays = new string(relay);
            
            foreach (string portss in ports)
            {

                string puertoCOM = portss; // Reemplaza "COM1" con el puerto COM que desees

                // Consulta para obtener información del puerto COM
                string query = $"SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%' AND Manufacturer = 'wch.cn' AND Name LIKE '%CH340%'";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject port in searcher.Get())
                    {
                        // Imprime todas las propiedades del objeto ManagementObject
                        foreach (PropertyData property in port.Properties)
                        {
                            Console.WriteLine($"{property.Name}: {property.Value}");
                            connectToArduino(portss);
                            break;

                        }
                    }
                }
            }


            port.WriteLine("1-32");

            button2.Enabled = false;
            button3.Enabled = true;
            button11.Enabled = false;
            button10.Enabled = true;


            port.DataReceived += PuertoSerie_DataReceived;
            // ActualizarLabel();

            myTimer = new Timer();
            myTimer.Interval = 1000; // Establecer el intervalo en milisegundos (en este caso, 1 segundo)
            myTimer.Tick += MyTimer_Tick;


        }


        private void button6_Click(object sender, EventArgs e)
        {
            valorTextBox = textBox3.Text;
            contador = int.Parse(valorTextBox);
            selecionado_de_contador = true;
            if (estado_de_boton1)
            {
                port.WriteLine("1-31"); //debuger
                port.WriteLine("1-30");
            }
            else
            {
                port.WriteLine("0-31"); //poder
                port.WriteLine("0-30");
            }



            myTimer.Start();
        }
        private void button1_Click_2(object sender, EventArgs e)
        {
            valorTextBox = textBox4.Text;
            contador = int.Parse(valorTextBox);
            selecionado_de_contador = false;

            if (estado_de_boton2)
            {
                port.WriteLine("1-29");// debuger
                port.WriteLine("1-28");
            }
            else
            {
                port.WriteLine("0-29"); //debuger
                port.WriteLine("0-28");
            }



            myTimer.Start();

        }


        private void MyTimer_Tick(object sender, EventArgs e)
        {
            // Este método se ejecutará cada vez que el temporizador alcance su intervalo

            if (selecionado_de_contador)
            {
                if (contador > 0)
                {
                    contador--;
                    label3.Text = "Time: " + contador;
                }
                else
                {
                    if (estado_de_boton1)
                    {
                        port.WriteLine("0-31"); //debuger
                        port.WriteLine("0-30");
                    }
                    else
                    {
                        port.WriteLine("1-31"); //poder
                        port.WriteLine("1-30");
                    }


                    myTimer.Stop();
                    label3.Text = "Time: ";
                }
            }
            else
            {
                if (contador > 0)
                {
                    contador--;
                    label4.Text = "Time: " + contador;
                }
                else
                {
                    if (estado_de_boton2)
                    {
                        port.WriteLine("0-29"); //debuger
                        port.WriteLine("0-28");
                    }
                    else
                    {
                        port.WriteLine("1-29"); //poder
                        port.WriteLine("1-28");
                    }



                    myTimer.Stop();
                    label4.Text = "Time: ";
                }
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Iniciar el temporizador
            myTimer.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Detener el temporizador
            myTimer.Stop();
        }






        private void PuertoSerie_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Leer datos del puerto serie
                string[] mensaje = port.ReadLine().Split(' ');
                MostrarMensajeEnLabel(mensaje);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al recibir datos: {ex.Message}");
            }
        }

        private void MostrarMensaje(string mensaje)
        {
            // Muestra el mensaje en la consola (puedes cambiar esto según tus necesidades)
            Console.WriteLine(mensaje);
        }

        private void MostrarMensajeEnLabel(string[] mensaje)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.Invoke((MethodInvoker)(() => textBox1.Text = mensaje[0]));
                textBox2.Invoke((MethodInvoker)(() => textBox2.Text = mensaje[1]));

            }
            else
            {
                // Si ya estamos en el hilo de la interfaz gráfica, podemos actualizar directamente
                textBox1.Text = mensaje[0];
                textBox2.Text = mensaje[1];
            }
        }





       










        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (contador > 0)
            {
                contador--;
                ActualizarLabel();
            }
            else
            {
                timer1.Stop(); // Detiene el temporizador cuando el contador llega a 0.
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                //connectToArduino();
            }
            else
            {
                disconnectFromArduino();
            }
        }

        void getAvailableComPorts()
        {
            ports = SerialPort.GetPortNames();
        }

        private void connectToArduino(string supremo)
        {
            isConnected = true;
            string selectedPort = supremo;
            port = new SerialPort(selectedPort, 115200, Parity.None, 8, StopBits.One);
            port.Open();
            //port.Write("#STAR\n");
            //button1.Text = "Disconnect";
            enableControls();
        }
        private void btnDecrement_Click(object sender, EventArgs e)
        {
            if (contador > 0)
            {
                contador--;
                ActualizarLabel();
            }
        }
        private void ActualizarLabel()
        {
            //  label1.Text = contador.ToString();
        }





        private void disconnectFromArduino()
        {
            isConnected = false;
            port.Write("/\n");
            port.Close();

            disableControls();
            resetDefaults();
        }



        private void enableControls()
        {



            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;


        }

        private void disableControls()
        {
            // checkBox1.Enabled = false;
            //checkBox2.Enabled = false;
            //checkBox3.Enabled = false;

            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;

        }

        private void resetDefaults()
        {
            checkBox0.Checked = false;
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;
            checkBox5.Checked = false;
            checkBox6.Checked = false;
            checkBox7.Checked = false;
            checkBox8.Checked = false;
            checkBox9.Checked = false;
            checkBox10.Checked = false;
            checkBox11.Checked = false;
            checkBox12.Checked = false;
            checkBox13.Checked = false;
            checkBox14.Checked = false;
            checkBox15.Checked = false;
            checkBox16.Checked = false;
            checkBox17.Checked = false;
            checkBox18.Checked = false;
            checkBox19.Checked = false;
            checkBox20.Checked = false;
            checkBox21.Checked = false;
            checkBox22.Checked = false;
            checkBox23.Checked = false;
            checkBox24.Checked = false;
            checkBox25.Checked = false;
            checkBox26.Checked = false;
            checkBox27.Checked = false;
       


        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = true;
            port.WriteLine("0-31");
            port.WriteLine("0-30");
            //if (isConnected)
            //{
            //    port.Write("R\n");
            //}

        }

        private void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            button2.Enabled = true;
            //port.Write("P\n");
            //resetDefaults();

            port.WriteLine("1-31");
            port.WriteLine("1-30");


        }
        private void button10_Click(object sender, EventArgs e)
        {

            button10.Enabled = false;
            button11.Enabled = true;
            port.WriteLine("1-29");
            port.WriteLine("1-28");

        }

        private void button11_Click(object sender, EventArgs e)
        {

            button11.Enabled = false;
            button10.Enabled = true;
            port.WriteLine("0-29");
            port.WriteLine("0-28");

        }









        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }










        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }



        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                port.WriteLine("1-33");
                resetDefaults();
            }
           
        }








        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void checkBox32_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }


























        private void button8_Click_1(object sender, EventArgs e)
        {
            button10.Enabled = false;
            button11.Enabled = true;





            port.WriteLine("1-29");
            port.WriteLine("1-28");

            Timer delayTimer = new Timer();
            delayTimer.Interval = 6000;
            delayTimer.Tick += (timerSender, timerEvent) =>
            {

                button2.Enabled = true;
                button3.Enabled = false;
                port.WriteLine("1-31");
                port.WriteLine("1-30");

                delayTimer.Stop();
                delayTimer.Dispose();  // Liberar recursos del temporizador
            };

            // Iniciar el temporizador
            delayTimer.Start();

        }


        private void button9_Click_1(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = true;


            port.WriteLine("0-31");
            port.WriteLine("0-30");
  

            // Configurar un temporizador con un intervalo de 6 segundos (6000 milisegundos)
            Timer delayTimer = new Timer();
            delayTimer.Interval = 6000;
            delayTimer.Tick += (timerSender, timerEvent) =>
            {
                // Acción después del retraso



                button10.Enabled = true;
                button11.Enabled = false;

                port.WriteLine("0-29");
                port.WriteLine("0-28");

                // Detener el temporizador después de la acción
                delayTimer.Stop();
                delayTimer.Dispose();  // Liberar recursos del temporizador
            };

            // Iniciar el temporizador
            delayTimer.Start();

        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }






        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }






        private void checkBox0_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox0.Checked) { port.WriteLine("1-0"); }
                else { port.WriteLine("0-0"); }
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox1.Checked) { port.WriteLine("1-1"); }
                else { port.WriteLine("0-1"); }
            }
        }
        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox2.Checked) { port.WriteLine("1-2"); }
                else { port.WriteLine("0-2"); }
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox3.Checked) { port.WriteLine("1-3"); }
                else { port.WriteLine("0-3"); }
            }

        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox4.Checked) { port.WriteLine("1-4"); }
                else { port.WriteLine("0-4"); }
            }
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox5.Checked) { port.WriteLine("1-5"); }
                else { port.WriteLine("0-5"); }
            }
        }
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox6.Checked) { port.WriteLine("1-6"); }
                else { port.WriteLine("0-6"); }
            }
        }
        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox7.Checked) { port.WriteLine("1-7"); }
                else { port.WriteLine("0-7"); }
            }
        }
        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox8.Checked) { port.WriteLine("1-8"); }
                else { port.WriteLine("0-8"); }
            }
        }
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox9.Checked) { port.WriteLine("1-9"); }
                else { port.WriteLine("0-9"); }
            }
        }
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox10.Checked) { port.WriteLine("1-10"); }
                else { port.WriteLine("0-10"); }
            }
        }
        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox11.Checked) { port.WriteLine("1-11"); }
                else { port.WriteLine("0-11"); }
            }
        }
        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox12.Checked) { port.WriteLine("1-12"); }
                else { port.WriteLine("0-12"); }
            }
        }
        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox13.Checked) { port.WriteLine("1-13"); }
                else { port.WriteLine("0-13"); }
            }
        }



        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox14.Checked) { port.WriteLine("1-14"); }
                else { port.WriteLine("0-14"); }
            }
        }
        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox15.Checked) { port.WriteLine("1-15"); }
                else { port.WriteLine("0-15"); }
            }
        }
        private void checkBox16_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox16.Checked) { port.WriteLine("1-16"); }
                else { port.WriteLine("0-16"); }
            }
        }
        private void checkBox17_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox17.Checked) { port.WriteLine("1-17"); }
                else { port.WriteLine("0-17"); }
            }
        }
        private void checkBox18_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox18.Checked) { port.WriteLine("1-18"); }
                else { port.WriteLine("0-18"); }
            }
        }
        private void checkBox19_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox19.Checked) { port.WriteLine("1-19"); }
                else { port.WriteLine("0-19"); }
            }
        }
        private void checkBox20_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox20.Checked) { port.WriteLine("1-20"); }
                else { port.WriteLine("0-20"); }
            }
        }
        private void checkBox21_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox21.Checked) { port.WriteLine("1-21"); }
                else { port.WriteLine("0-21"); }
            }
        }
        private void checkBox22_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox22.Checked) { port.WriteLine("1-22"); }
                else { port.WriteLine("0-22"); }
            }
        }
        private void checkBox23_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox23.Checked) { port.WriteLine("1-23"); }
                else { port.WriteLine("0-23"); }
            }
        }
        private void checkBox24_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox24.Checked) { port.WriteLine("1-24"); }
                else { port.WriteLine("0-24"); }
            }
        }
        private void checkBox25_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox25.Checked) { port.WriteLine("1-25"); }
                else { port.WriteLine("0-25"); }
            }
        }
        private void checkBox26_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox26.Checked) { port.WriteLine("1-26"); }
                else { port.WriteLine("0-26"); }
            }
        }
        private void checkBox27_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox27.Checked) { port.WriteLine("1-27"); }
                else { port.WriteLine("0-27"); }
            }
        }/*
        private void checkBox28_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox28.Checked) { port.WriteLine("1-28"); }
                else { port.WriteLine("0-28"); }
            }
        }
        private void checkBox29_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox29.Checked) { port.WriteLine("1-29"); }
                else { port.WriteLine("0-29"); }
            }
        }
        private void checkBox30_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox30.Checked) { port.WriteLine("1-30"); }
                else { port.WriteLine("0-30"); }
            }
        }
        private void checkBox31_CheckedChanged_1(object sender, EventArgs e)
        {
            if (isConnected)
            {
                if (checkBox31.Checked) { port.WriteLine("1-31"); }
                else { port.WriteLine("0-31"); }
            }
        }

        */

   
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            port.WriteLine("0-32");

            button2.Enabled = false;
            button3.Enabled = true;
            button11.Enabled = false;
            button10.Enabled = true;

            resetDefaults();
        }

        private void checkBox30_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox30.Checked) {
                estado_de_boton1=true;
            }
            else {
                estado_de_boton1 =false;
            }


        }

        private void checkBox31_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox31.Checked)
            {
                estado_de_boton2 = true;
            }
            else
            {
                estado_de_boton2 = false;
            }
        }
    }
}
