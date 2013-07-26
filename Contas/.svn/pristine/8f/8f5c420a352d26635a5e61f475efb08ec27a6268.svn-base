using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contas
{
    public partial class frmPrincipal : Form
    {        
        private enum Tipo        
        {
            insert,
            update,
            delete,
            reload
        }

        private Tipo _tipo;

        private List<Parcelas> listaParcelas;

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {            
            DBSqlite.CriarBanco();            
            tabControl1.TabPages.Remove(tabControl1.TabPages[1]);
            _tipo = Tipo.reload;            
            dgvDados.Focus();
            Carregar();
            HabilitarDesabilitar();
        }

        private void Carregar()
        {
            DateTime dataini    = Convert.ToDateTime("01/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString());
            DateTime datafim    = dataini.AddMonths(1).AddDays(-1);
            var dados           = DBSqlite.GetAll();
            dgvDados.DataSource = dados;
            lblParcial.Text     = String.Format("Valor vencido: {0}", dados.Where(c => c.vencimento <= DateTime.Now).Sum(c => c.valor).ToString("C2"));
            lblTotal.Text       = String.Format("Valor total: {0}", dados.Sum(c => c.valor).ToString("C2"));
            lblVenceMes.Text    = String.Format("Valor mês: {0}", dados.Where(c => c.vencimento >= dataini && c.vencimento <= datafim).Sum(c => c.valor).ToString("C2"));
        }

        private void HabilitarDesabilitar()
        {
            btnCadastrar.Enabled = (_tipo == Tipo.reload);
            btnAlterar.Enabled   = (_tipo == Tipo.reload && dgvDados.RowCount > 0);
            btnExcluir.Enabled   = btnAlterar.Enabled;
            btnSalvar.Enabled    = (_tipo == Tipo.insert || _tipo == Tipo.update); ;
            btnCancelar.Enabled  = btnSalvar.Enabled;
            btnAtualizar.Enabled = (_tipo == Tipo.reload);
            btnFinalizar.Enabled = btnAlterar.Enabled;
        }

        private void Limpar()
        {
            txtDescricao.Text     = string.Empty;
            txtDataEmissao.Text   = DateTime.Now.ToShortDateString();
            txtQtdeParcelas.Value = 0;
            txtValor.Value        = 0;
            if (listaParcelas != null)
            {
                listaParcelas.Clear();
                dataGridView1.DataSource = listaParcelas;                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtDescricao.Text == "")
            {
                errorProvider1.SetError(txtDescricao,"Informe a descrição.");
                return;
            }
            if (txtQtdeParcelas.Value == 0)
            {
                errorProvider1.SetError(txtQtdeParcelas, "Informe um número maior que zero.");
                return;
            }
            if (txtValor.Text == "")
            {
                errorProvider1.SetError(txtValor, "Informe o valor.");
                return;
            }
            if (Truncar(Convert.ToDecimal(txtValor.Text)) == 0)
            {
                errorProvider1.SetError(txtValor, "Informe o valor maior que zero.");
                return;
            }
            try
            {
                listaParcelas        = new List<Parcelas>();
                decimal valorParcela = (Convert.ToDecimal(txtValor.Text)) / Convert.ToInt32(txtQtdeParcelas.Text);
                DateTime dataEmissao = Convert.ToDateTime(txtDataEmissao.Text);

                for (int i = 0; i < Convert.ToInt32(txtQtdeParcelas.Text); i++)
                {
                    Parcelas parcela     = new Parcelas();
                    parcela.dtcompra     = dataEmissao;
                    parcela.dtvencimento = dataEmissao.AddMonths(i);
                    parcela.nparcela     = i + 1;
                    parcela.vlrparcela   = Truncar(valorParcela);
                    parcela.descricao    = txtDescricao.Text;
                    parcela.vlrtotal     = Convert.ToDecimal(txtValor.Text);
                    parcela.qtdeparcelas = Convert.ToInt32(txtQtdeParcelas.Text);                     
                    listaParcelas.Add(parcela);
                }
                dataGridView1.DataSource = listaParcelas;
                MessageBox.Show("Parcela(s) gerada(s) com sucesso!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gerar parcelas: "+ex.Message);
            }
        }

        public static decimal Truncar(decimal valor)
        {
            valor *= 100;
            valor = Math.Truncate(valor);
            valor /= 100;
            return valor;
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            _tipo = Tipo.insert;
            HabilitarDesabilitar();            
            tabControl1.TabPages.Remove(tabPage1);
            tabControl1.TabPages.Add(tabPage2);
            txtDescricao.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            _tipo = Tipo.reload;
            HabilitarDesabilitar();            
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Add(tabPage1);
            Limpar();
        }

        private void frmPrincipal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in listaParcelas)
                {
                    Documentos doc    = new Documentos();
                    doc.compra        = item.dtcompra;
                    doc.descricao     = item.descricao;
                    doc.parcela       = item.nparcela;
                    doc.qtde_parcelas = item.qtdeparcelas;
                    doc.status        = "A";
                    doc.total         = item.vlrtotal;
                    doc.valor         = item.vlrparcela;
                    doc.vencimento    = item.dtvencimento;
                    DBSqlite.Insert(doc);
                }
                MessageBox.Show("Documento cadastrado com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnCancelar_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Erro",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.waib.com.br");
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            Carregar();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente excluir o documento?", "Pergunta", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    DBSqlite.Delete(Convert.ToInt32(dgvDados.CurrentRow.Cells[0].Value));
                    MessageBox.Show("Documento excluído com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Carregar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Deseja realmente finalizar o documento?","Pergunta",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    DBSqlite.Finalizar(Convert.ToInt32(dgvDados.CurrentRow.Cells[0].Value));
                    MessageBox.Show("Documento finalizado com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Carregar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }                
    }
}
