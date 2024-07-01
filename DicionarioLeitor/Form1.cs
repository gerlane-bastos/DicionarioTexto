using Microsoft.VisualBasic.Devices;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

namespace DicionarioLeitor
{
    public partial class Form1 : Form
    {
        private const string caminhoArquivoDicionario = "C:\\Users\\usuario\\Downloads\\dicionarioSimplificado.txt";
        private string caminhoAquivoCarregado = string.Empty;

        private List<string> listaPalavrasDigitadas;
        private string textoDigitado = string.Empty;
        private string textoSalvo = string.Empty;
        private Keys teclaPressionada;
        private Dictionary<int, string> mapaDePalavras;

        //CONSTRUTOR SEM PARAMAETROS QUE INICIALIZA ALGUMAS VARIAVEIS
        public Form1()
        {
            listaPalavrasDigitadas = new List<string>();
            mapaDePalavras = new Dictionary<int, string>();
            this.criaTabelaHash();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        //METODO DE ADICIONAR PALAVRAS NO DICIONARO, PEGA A LISTA DE PALAVRAS
        //INSERIDAS NO TEXTO GUARDA NUMA LISTA E QUANDO O BOTAO É ACIONADO
        //CADA PALAVRA É INCLUIDO NO DICIONARIO
        private void adicionarPalavraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            salvaPalavrasNoDicionario();
        }

        //metodo que vai pegando qualquer mudanca no texto e inserindo na lista
        private void pegaAlteracaoDoTexto(object sender, EventArgs e)
        {
            textoDigitado = this.richTextBox1.Text;

            if (Keys.Back == teclaPressionada)
            {
                textoSalvo = textoDigitado;
            }

            if (Keys.Enter == teclaPressionada || Keys.Space == teclaPressionada)
            {
                int tamanhoTexto = textoSalvo.Length;
                string palavra = string.Empty;
                if (tamanhoTexto == 0)
                {
                    palavra = textoDigitado;
                    if (palavra.Trim().Length != 0)
                    {
                        if (!palavraExisteNoDicionario(palavra.Trim().GetHashCode()))
                        {

                            int inicioPalavra = this.richTextBox1.Find(palavra, 0, RichTextBoxFinds.MatchCase);
                            this.richTextBox1.SelectionStart = inicioPalavra;
                            this.richTextBox1.SelectionLength = palavra.Length;
                            this.richTextBox1.SelectionColor = Color.Red;

                            int size = this.richTextBox1.Text.Length;
                            this.richTextBox1.Select(size, size);
                            this.richTextBox1.SelectionColor = Color.Black;

                            listaPalavrasDigitadas.Add(palavra);
                        }
                        textoSalvo = textoDigitado;
                        return;
                    }
                }
                else
                {

                    int tamanhoTextoSalvo = textoSalvo.Length;
                    palavra = textoDigitado.Substring(tamanhoTextoSalvo - 1);

                    if (!palavraExisteNoDicionario(palavra.Trim().GetHashCode()))
                    {

                        int inicioPalavra = this.richTextBox1.Find(palavra, tamanhoTextoSalvo > 0 ? (tamanhoTextoSalvo - 1) : 0, RichTextBoxFinds.MatchCase);
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

        //METODO QUE FICA OUVINDO UMA ACAO DE PRESSIONAR A TECLA E CAPTURA QUAL
        //TECLA FOI PRESSIONADA
        private void pegaEventoDeTecla(object sender, KeyEventArgs e)
        {
            teclaPressionada = e.KeyCode;
        }

        //METODO QUE CARREGA O DICIONARIO
        private string leDicionario()
        {
            return File.ReadAllText(caminhoArquivoDicionario, System.Text.Encoding.UTF8);
        }


        //metodo que separa palavra por palavra do texto que esteja entre espaco em branco ou uma nova linha
        //e adiciona cada palavra em um array
        private string[] separaPalavras(string texto)
        {
            // Separa palavra por palavra e remove espacos em branco
            return texto.Split(new[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }




        //metodo que cria a tabela hash e insere palavra por palavra dentro da tabela (quando inicia a aplicacao)
        private void criaTabelaHash()
        {
            string textoDoArquivo = leDicionario();
            string[] palavras = separaPalavras(textoDoArquivo);
            foreach (var palavra in palavras)
            {
                int palavraTransformada = palavra.GetHashCode();
                if (!this.mapaDePalavras.ContainsKey(palavraTransformada))
                {
                    this.mapaDePalavras.Add(palavraTransformada, palavra);
                }
            }
        }

        //metodo que pega a tabela hash com as palavras carregadas pelo dicionario,
        //e insere novas palavras na tabela
        private void adicionaPalavraNaTabelaHash(string palavra)
        {
            int palavraHasheada = palavra.GetHashCode();
            if (!this.mapaDePalavras.ContainsKey(palavraHasheada))
            {
                this.mapaDePalavras.Add(palavraHasheada, palavra);
            }
        }

        //busca na tabela hash se a palavra existe
        private bool palavraExisteNoDicionario(int hash)
        {
            return mapaDePalavras.TryGetValue(hash, out var palavra);
        }

        //METODO QUE GUARDA ACAO DE AO CLICAR NO BOTAO ABRIR DO EDITOR, ABRI UM
        //ARQUIVO SELECIONADO
        private void abrirArquivo(object sender, EventArgs e)
        {
            this.textoDigitado = string.Empty;
            this.textoSalvo = string.Empty;
            this.richTextBox1.Text = "";

            var abrirArquivo = new OpenFileDialog
            {
                Filter = "(*.txt)|*.txt",
                Title = "Abrir arquivo"
            };

            if (abrirArquivo.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    caminhoAquivoCarregado = abrirArquivo.FileName;
                    using (StreamReader reader = new StreamReader(caminhoAquivoCarregado))
                    {
                        this.richTextBox1.Text = reader.ReadToEnd();

                        textoDigitado = this.richTextBox1.Text;
                        string[] arrayPalavras = separaPalavras(textoDigitado);
                        int inicioPalavra = 0;
                        foreach (var item in arrayPalavras)
                        {
                            if (!palavraExisteNoDicionario(item.GetHashCode()))
                            {
                                listaPalavrasDigitadas.Add(item);
                                inicioPalavra = this.richTextBox1.Find(item, inicioPalavra, RichTextBoxFinds.MatchCase);

                                this.richTextBox1.SelectionStart = inicioPalavra;
                                this.richTextBox1.SelectionLength = item.Length;
                                this.richTextBox1.SelectionColor = Color.Red;

                                inicioPalavra += item.Length;

                                int size = this.richTextBox1.Text.Length;
                                this.richTextBox1.Select(size, size);
                                this.richTextBox1.SelectionColor = Color.Black;
                            }
                        }

                        int tamanhoTexto = this.richTextBox1.Text.Length;
                        this.richTextBox1.Select(tamanhoTexto, tamanhoTexto);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar arquivo: " + ex.Message);
                }
            }
        }

        //METODO QUE AO CLICAR NO BOTAO SALVAR NO EDITOR DE TEXTO, SALVA O TEXTO
        //DIGITADO NO ARQUIVO ABERTO, SE O ARQUIVO NAO EXISTIR, PEDE PARA CRIAR UM NOVO
        private void salvarArquivo(object sender, EventArgs e)
        {
            if (File.Exists(caminhoAquivoCarregado))
            {
                var textoArquivo = File.ReadAllText(caminhoAquivoCarregado);
                textoArquivo = textoArquivo.Replace(textoArquivo, this.richTextBox1.Text);
                File.WriteAllText(caminhoAquivoCarregado, textoArquivo);
            }
            else
            {
                novoArquivo(sender, e);
            }
        }

        //METODO UE AO CLICAR NO BOTAO NOVO DO EDITOR DE TEXTO ABRE DIALOGO PARA
        //SALVAR UM NOVO ARQUIVO
        private void novoArquivo(object sender, EventArgs e)
        {
            var arquivoParaSalvar = new SaveFileDialog()
            {
                Filter = "(*.txt)|*.txt",
                Title = "Novo arquivo"
            };

            if (arquivoParaSalvar.ShowDialog() == DialogResult.OK)
            {
                if (arquivoParaSalvar.FileName != "")
                {
                    File.WriteAllText(arquivoParaSalvar.FileName, this.richTextBox1.Text);
                    MessageBox.Show("Arquivo Salvo");
                }
            }
        }

        //METODO QUE AO CLICAR NO BOTAO DE INCLUIR PALAVRA, SALVA AS PALAVRAS DIGITADAS
        //NO DICIONARIO, ALÉM DE ADICIONAR A PALAVRA NA TABELA HASH
        private void salvaPalavrasNoDicionario()
        {
            string textoOriginal = File.ReadAllText(caminhoArquivoDicionario);
            string textoModificado = textoOriginal.Substring(0, textoOriginal.Length) +
                                     textoOriginal.Substring(textoOriginal.Length);

            foreach (var palavra in listaPalavrasDigitadas)
            {
                var palavraFormatada = palavra.Trim();

                if (!palavraExisteNoDicionario(palavraFormatada.GetHashCode()))
                {
                    textoModificado += "\n" + palavraFormatada;
                    adicionaPalavraNaTabelaHash(palavraFormatada);
                }
            }

            File.WriteAllText(caminhoArquivoDicionario, textoModificado);

            this.richTextBox1.SelectAll();
            this.richTextBox1.SelectionColor = this.richTextBox1.ForeColor;
            var tamanho = this.richTextBox1.Text.Length;
            this.richTextBox1.Select(tamanho, tamanho);

            MessageBox.Show("Palavras incluídas no dicionário.");
        }

    }

}
