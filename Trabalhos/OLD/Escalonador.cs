using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Escalonador
{
    public partial class Form1 : Form
    {
        string[,] matriz;
        int ultimaColuna = 1;
        int qntProcessos = 0;
        int qntColunas = 0;
        int processosTerminados = 0;
        public Form1()
        {
            InitializeComponent();
        }

        void validaTeclas(TextBox txt, NumericUpDown nUD, Label lbl)
        {
            int restant = calculaCaracterRestante(txt.Text.Length);
            lbl.Text = restant.ToString();

            if (restant < 255)
            {
                if (restant == 254 && !validaEntrada(txt.Text.Substring(txt.Text.Length - 1).ToUpper()))
                    txt.Text = "";
                else if (restant == 0 || !validaEntrada(txt.Text.Substring(txt.Text.Length - 1).ToUpper()))
                    txt.Text = txt.Text.Substring(0, txt.Text.Length - 1);
            }

            if (txt.Text.Length > 0)
                nUD.Enabled = true;
            else
                nUD.Enabled = false;
        }


        void controlesBloquear()
        {
            ChangeMyEnabled(textBox1,false);
            ChangeMyEnabled(textBox2,false);
            ChangeMyEnabled(textBox3,false);
            ChangeMyEnabled(textBox4,false);
            ChangeMyEnabled(textBox8,false);
            ChangeMyEnabled(textBox9,false);
            ChangeMyEnabled(textBox10,false);
            ChangeMyEnabled(button1,false);
            ChangeMyEnabled(button2,false);
        }

        void controlesDesbloquear()
        {
            ChangeMyEnabled(textBox1, true);
            ChangeMyEnabled(textBox2, true);
            ChangeMyEnabled(textBox3, true);
            ChangeMyEnabled(textBox4, true);
            ChangeMyEnabled(textBox8, true);
            ChangeMyEnabled(textBox9, true);
            ChangeMyEnabled(textBox10, true);
            ChangeMyEnabled(button1, true);
            ChangeMyEnabled(button2, true);
        }

        int calculaCaracterRestante(int length)
        {
            int max = 255;
            return max - length;
        }

        int calculaMaiorTamanho(List<Processo> processos)
        {
            int maior = 0;
            for (int i = 0; i < processos.Count; i++)
            {
                int tamanhoProcesso = processos[i].getTamanho();
                if (tamanhoProcesso > maior)
                    maior = tamanhoProcesso;
            }
            return maior;
        }

        public int calculaTamanhoTotal(List<Processo> processos)
        {
            int total = 0;
            for (int i = 0; i < processos.Count; i++)
            {
                int tamanhoProcesso = processos[i].getTamanho();
                total += tamanhoProcesso;
            }
            return total;
        }

        public StringBuilder escreveLog()
        {
            StringBuilder sb = new StringBuilder();
            //string log = "";
            for (int i = 0; i < qntProcessos; i++)
            {
                matriz[i, 0] = (i+1).ToString();
            }
            matriz[qntProcessos-1, 0] = " ";

            for (int i = 1; i < qntColunas; i++)
            {
                matriz[qntProcessos - 1, i] = ((i > 9)?(i%10).ToString():i.ToString());
            }

            for (int i = 0; i < qntProcessos; i++)
            {
                string a = "";
                for (int j = 0; j < qntColunas; j++)
                {
                    a += (matriz[i, j] == "") ? matriz[i, j] : matriz[i, j] + " | ";
                    // log += (matriz[i, j] == "") ? matriz[i, j] : matriz[i, j] + " | ";
                }
                //log += "\n\r";
                sb.AppendLine(a);
            }
            sb.AppendLine("");
            sb.AppendLine("Legenda:");
            sb.AppendLine("# - Processo Apto");
            sb.AppendLine("T - Processo Terminado");

            //return log;
            return sb;
        }

        public void gravaMatriz(int linha, string caracter)
        {
            for (int i = 0; i < qntProcessos - 1; i++)
            {
                if (i == linha)
                    matriz[i, ultimaColuna] = caracter;
                else if(ultimaColuna > 1 && (matriz[i, ultimaColuna - 1] == " " || matriz[i, ultimaColuna - 1] == "T" ))
                    matriz[i, ultimaColuna] = " ";
                else
                    matriz[i, ultimaColuna] = "#";
            }
            ultimaColuna++;
        }

        delegate void ChangeMyTextDelegate(Control ctrl, string text);
        public static void ChangeMyText(Control ctrl, string text)
        {
            if (ctrl.InvokeRequired)
            {
                ChangeMyTextDelegate del = new ChangeMyTextDelegate(ChangeMyText);
                ctrl.Invoke(del, ctrl, text);
            }
            else
            {
                ctrl.Text = text;
            }
        }

        delegate void ChangeMyColorDelegate(Control ctrl, Color color);
        public static void ChangeMyColor(Control ctrl, Color Color)
        {
            if (ctrl.InvokeRequired)
            {
                ChangeMyColorDelegate del = new ChangeMyColorDelegate(ChangeMyColor);
                ctrl.Invoke(del, ctrl, Color);
            }
            else
            {
                ctrl.BackColor = Color;
            }
        }

        delegate void ChangeMyEnabledDelegate(Control ctrl, bool a);
        public static void ChangeMyEnabled(Control ctrl, bool a)
        {
            if (ctrl.InvokeRequired)
            {
                ChangeMyEnabledDelegate del = new ChangeMyEnabledDelegate(ChangeMyEnabled);
                ctrl.Invoke(del, ctrl, a);
            }
            else
            {
                ctrl.Enabled = a;
            }
        }

        private void escalonaProcessos(List<Processo> processos, int timeslice, int velocidade, List<TextBox> textboxes)
        {
            //5 - Percorre todos os processos de acordo com o seu timeslice e tempo de "pausa" setando como finalizado o processo após terminar o total de caracteres+ciclos
            while (true)
            {
                for (int i = 0; i < processos.Count(); i++)
                {
                    if (!processos[i].finalizado)
                        ChangeMyColor(textboxes[i],Color.Yellow);
                    
                    for (int j = 0; j < timeslice; j++)
                    {
                        if (!processos[i].finalizado)
                        {
                            int fracao = Convert.ToInt32(1000 * Decimal.Divide(1, Convert.ToInt32(numericUpDown1.Value)));
                            string a = "";
                            System.Threading.Thread.Sleep(fracao);
                            int posicaoString = processos[i].incrementaUltimoAdicionado();
                            if (posicaoString > -1)
                            {
                                a = processos[i].getInput().ElementAt(posicaoString).ToString();
                                ChangeMyText(textboxes[i], textboxes[i].Text + a);
                            }

                            if (processos[i].finalizado)
                            {
                                processosTerminados++;
                                //6 - Joga os valores para a matriz de saída do TXT
                                gravaMatriz(i, "T");
                                ChangeMyColor(textboxes[i], Color.Green);
                                break;
                            }
                            else
                                //6 - Joga os valores para a matriz de saída do TXT
                                gravaMatriz(i, a);
                        }
                        else
                            break;
                    }
                    if(!processos[i].finalizado)
                        ChangeMyColor(textboxes[i], Color.White);
                }

                if (processosTerminados == processos.Count())
                    break;

                
            }

            string caminho = "saida.txt";
            if (System.IO.File.Exists(caminho))
                System.IO.File.Delete(caminho);

            System.IO.File.Create(caminho).Close();
            System.IO.TextWriter txtWr = System.IO.File.AppendText(caminho);

            string log = escreveLog().ToString();
            txtWr.WriteLine(log);
            txtWr.Close();

            //Grava no TXT
            Process.Start("notepad.exe", caminho);

            //return true;
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //1 - Seta as variaveis
            ultimaColuna = 1;
            qntProcessos = 0;
            qntColunas = 0;
            processosTerminados = 0;

            //2 - Cria a Thread de execução.
            System.Threading.ThreadStart ts = new System.Threading.ThreadStart(executaThread);
            System.Threading.Thread t =
                new System.Threading.Thread(ts);
            t.IsBackground = true;
            t.Start();

            controlesBloquear();
        }

        void finalizado(bool a)
        {
            if (a)
                processosTerminados++;
        }

        void executaThread()
        {
            //3 - Cria os processos
            List<Processo> processos = new List<Processo>();
            List<TextBox> textboxes = new List<TextBox>();
            List<Label> labels = new List<Label>();

            Processo processo1 = new Processo();
            processo1.setInput(textBox2.Text);
            processos.Add(processo1);
            textboxes.Add(textBox5);
            labels.Add(label4);
            processo1.ciclos = Convert.ToInt32(textBox8.Text);
            finalizado(processos[0].finalizado);

            Processo processo2 = new Processo();
            processo2.setInput(textBox3.Text);
            processos.Add(processo2);
            textboxes.Add(textBox6);
            labels.Add(label5);
            processo2.ciclos = Convert.ToInt32(textBox9.Text);
            finalizado(processos[1].finalizado);

            Processo processo3 = new Processo();
            processo3.setInput(textBox4.Text);
            processos.Add(processo3);
            textboxes.Add(textBox7);
            labels.Add(label6);
            processo3.ciclos = Convert.ToInt32(textBox10.Text);
            finalizado(processos[2].finalizado);

            //4 - Calcula a matriz de saída para o txt
            qntProcessos = processos.Count() + 1;
            qntColunas = calculaTamanhoTotal(processos)+1;
            matriz = new string[qntProcessos, qntColunas];


            escalonaProcessos(processos, Convert.ToInt32(textBox1.Text), 0, textboxes);
            controlesDesbloquear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            validaTeclas(textBox2, textBox8, label10);
        }

        private bool validaEntrada(string p)
        {
            string permitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (permitidos.Contains(p))
                return true;

            return false;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.Text = textBox2.Text.ToUpper();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            validaTeclas(textBox3, textBox9, label11);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            textBox3.Text = textBox3.Text.ToUpper();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            validaTeclas(textBox4, textBox10, label12);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            textBox4.Text = textBox4.Text.ToUpper();
        }

        private void Parâmetros_Enter(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Value = 1;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Value = 1;
            textBox9.Value = 1;
            textBox10.Value = 1;

            textBox7.BackColor = Color.White;
            textBox5.BackColor = Color.White;
            textBox6.BackColor = Color.White;

            textBox8 .Enabled = false;
            textBox9 .Enabled = false;
            textBox10.Enabled = false;
        }
    }
}
