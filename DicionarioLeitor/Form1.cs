using Microsoft.VisualBasic.Devices;
using System.Drawing;
using System.Text.RegularExpressions;

namespace DicionarioLeitor
{
    public partial class Form1 : Form
    {

        private List<string> listaPalavrasDigitadas;
        string textoDigitado = string.Empty;
        string textoSalvo = string.Empty;
        Keys teclaPressionada;
        Dictionary<int, string> mapaDePalavras;

        public Form1()
        {
            listaPalavrasDigitadas = new List<string>();
            mapaDePalavras = new Dictionary<int, string>();
            this.criaTabelaHash();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        private void sobreOProjetoToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            //MessageBox.Show("No map: " + map.TryGetValue("casa".GetHashCode(), out var palavra));

            listaPalavrasDigitadas.ForEach(a => { MessageBox.Show("Palavra na lista: " + a); });
            //foreach (var item in mapaDePalavras)
            //{
                //MessageBox.Show("key: " + item.Key);
            //}
            //this.richTextBox1.Text = this.richTextBox1.Text.Insert(0, "viva");

            //pinta palavra de vermelho
            //Color cor = Color.Red;
            //this.richTextBox1.SelectionColor = cor;
            //this.richTextBox1.AppendText(palavra);
            //this.richTextBox1.SelectionColor = this.richTextBox1.ForeColor;


        }



        //metodo que vai pegando qualquer mudanca no texto e inserindo na lista
        private void pegaAlteracaoDoTexto(object sender, EventArgs e)
        {

            textoDigitado = this.richTextBox1.Text;

            if (Keys.Enter == teclaPressionada || Keys.Space == teclaPressionada)
            {
                int tamanhoTexto = textoSalvo.Length;
                string palavra = string.Empty;
                if (tamanhoTexto == 0)
                {
                    palavra = textoDigitado.Trim();

                    if (palavra.Length != 0)
                    {
                        if (!palavraExisteNoDicionario(palavra.GetHashCode()))
                        {

                            int inicioPalavra = this.richTextBox1.Find(palavra, 0, RichTextBoxFinds.MatchCase);
                            this.richTextBox1.SelectionStart = inicioPalavra;
                            this.richTextBox1.SelectionLength = palavra.Length;
                            this.richTextBox1.SelectionColor = Color.Red;
                            
                            int size = this.richTextBox1.Text.Length;
                            this.richTextBox1.Select(size, size);

                            listaPalavrasDigitadas.Add(palavra);
                        }
                        textoSalvo = textoDigitado;
                        return;
                    }
                } else
                {
                    
                    int tamanhoTextoSalvo = textoSalvo.Length;
                    palavra = textoDigitado.Substring(tamanhoTextoSalvo - 1);
                    //MessageBox.Show("palavra: " + palavra.Trim().GetHashCode());

                    if (!palavraExisteNoDicionario(palavra.Trim().GetHashCode()))
                    {
                        //MessageBox.Show("Entrou aqui");
                        int inicioPalavra = this.richTextBox1.Find(palavra, (tamanhoTextoSalvo-1), RichTextBoxFinds.MatchCase);
                        this.richTextBox1.SelectionStart = inicioPalavra;
                        this.richTextBox1.SelectionLength = palavra.Length;
                        this.richTextBox1.SelectionColor = Color.Red;

                        int size = this.richTextBox1.Text.Length;
                        this.richTextBox1.Select(size, size);
                        this.richTextBox1.SelectionColor = Color.Black;

                        listaPalavrasDigitadas.Add(palavra);
                    }

                    textoSalvo = textoDigitado;
                }

                return;
            }
        }

        
        private void pegaEventoDeTecla(object sender, KeyEventArgs e)
        {
            teclaPressionada = e.KeyCode;
        }

        
        private string leArquivo()
        {
            return File.ReadAllText("C:\\Users\\usuario\\Downloads\\dicionarioSimplificado.txt", System.Text.Encoding.UTF8);
        }

        
        //metodo que separa as palavras por virgula, criando um array de palavras
        private string[] separaPalavras()
        {
            string file = this.leArquivo();
            return file.Split(",");

        // Separar as palavras por linhas e remover espaços em branco
           //return file.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(palavra => palavra.Trim())
              // .ToArray();
        }

        
        //metodo que cria a tabela hash e insere palavra por palavra dentro da tabela
        private void criaTabelaHash()
        {
            string[] palavras = separaPalavras();
            foreach (var palavra in palavras)
            {
                int palavraTransformada = palavra.GetHashCode();
                if (!mapaDePalavras.ContainsKey(palavraTransformada))
                {
                    this.mapaDePalavras.Add(palavraTransformada, palavra);
                }
            }
        }

        
        //busca na hashtable se a palavra existe
        private bool palavraExisteNoDicionario(int hash)
        {
            return mapaDePalavras.TryGetValue(hash, out var palavra);
        }

    }

}
