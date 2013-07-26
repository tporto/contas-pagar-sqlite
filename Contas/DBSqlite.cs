using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace Contas
{
    public class DBSqlite
    {
        private static string conexao = "Data Source=Banco.db3";

        public static void CriarBanco()
        {
            if (!File.Exists("Banco.db3"))
            {
                SQLiteConnection.CreateFile("Banco.db3");                
                SQLiteConnection conn = new SQLiteConnection(conexao);
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }

                //Criar tabela de contas
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("CREATE TABLE PAGAR([ID] INTEGER PRIMARY KEY,[DESCRICAO] NVARCHAR(200),");
                sql.AppendLine("[COMPRA] DATE, [VENCIMENTO] DATE, [PARCELA] INTEGER, [QTDE_PARCELAS] INTEGER, [VALOR] DECIMAL(18,2), [TOTAL] DECIMAL(18,2), [STATUS] CHAR(1))");                
                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(),conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao criar: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static List<Documentos> GetAll()
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT * FROM PAGAR WHERE STATUS = 'A' ORDER BY VENCIMENTO ASC");                               
                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);

                SQLiteDataReader dr = cmd.ExecuteReader();
                Documentos doc;
                List<Documentos> lista = new List<Documentos>();
                while (dr.Read())
                {
                    doc               = new Documentos();
                    doc.id            = Convert.ToInt32(dr["ID"]);
                    doc.parcela       = Convert.ToInt32(dr["PARCELA"]);
                    doc.qtde_parcelas = Convert.ToInt32(dr["QTDE_PARCELAS"]);
                    doc.status        = dr["STATUS"].ToString();
                    doc.total         = Convert.ToDecimal(dr["TOTAL"]);
                    doc.valor         = Convert.ToDecimal(dr["VALOR"]);
                    doc.vencimento    = Convert.ToDateTime(dr["VENCIMENTO"]);
                    doc.descricao     = dr["DESCRICAO"].ToString();
                    doc.compra        = Convert.ToDateTime(dr["COMPRA"]);
                    lista.Add(doc);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao retornar registros: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }            
        }

        public static void Insert(Documentos doc)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("INSERT INTO PAGAR (DESCRICAO,COMPRA,VENCIMENTO, PARCELA, QTDE_PARCELAS,");
                sql.AppendLine("VALOR, TOTAL, STATUS) VALUES (@DESCRICAO,@COMPRA,@VENCIMENTO, @PARCELA,");
                sql.AppendLine("@QTDE_PARCELAS, @VALOR, @TOTAL, @STATUS)");

                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(),conn);
                cmd.Parameters.AddWithValue("DESCRICAO",doc.descricao);
                cmd.Parameters.AddWithValue("COMPRA",doc.compra);
                cmd.Parameters.AddWithValue("VENCIMENTO",doc.vencimento);
                cmd.Parameters.AddWithValue("PARCELA",doc.parcela);
                cmd.Parameters.AddWithValue("QTDE_PARCELAS",doc.qtde_parcelas);
                cmd.Parameters.AddWithValue("VALOR",doc.valor);
                cmd.Parameters.AddWithValue("TOTAL",doc.total);
                cmd.Parameters.AddWithValue("STATUS",doc.status);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao cadastrar documento: " + ex.Message); 
            }
            finally
            {
                conn.Close(); 
            }
        }

        public static void Delete(int id)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("UPDATE PAGAR SET STATUS = 'I' WHERE ID = @ID");                
                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("ID", id);                
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir documento: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public static void Finalizar(int id)
        {
            SQLiteConnection conn = new SQLiteConnection(conexao);
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("UPDATE PAGAR SET STATUS = 'F' WHERE ID = @ID");
                SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("ID", id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao finalizar documento: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
