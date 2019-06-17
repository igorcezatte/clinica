using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace clinica.Models
{
    public class classClientes
    {
        #region properties
        public int id { get; set; }
        public string nome { get; set; }
        public int cd_planoSaude { get; set; }
        public DateTime dataNascimento { get; set; }
        public string telefone { get; set; }
        public string cpf { get; set; }
        public string planoSaude { get; set; }
        public classErro objErro { get; set; }
        #endregion



        #region methods
        public classClientes()
        {
            objErro = new classErro();
        }

        public bool insertAlterCliente()
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spClientesInserir", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@cd_planoSaude", cd_planoSaude);
            cmd.Parameters.AddWithValue("@telefone", telefone);
            cmd.Parameters.AddWithValue("@cpf", cpf.Replace("-",string.Empty).Replace(".", string.Empty));
            cmd.Parameters.AddWithValue("@dataNascimento", dataNascimento);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                objErro.strErro = ex.Message;
                objErro.erro = true;
                objErro.cd_erro = ex.HResult;
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }

        public bool listCustomerByName(ref List<classClientes> lstClientes, string value)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spClientesListarPorNome", conn);

            if(!string.IsNullOrEmpty(value))
                cmd.Parameters.AddWithValue("@value", value);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    foreach(DataRow obj in dt.Rows)
                        lstClientes.Add(fillRow(obj));
                }
            }
            catch (Exception ex)
            {
                objErro.strErro = ex.Message;
                objErro.erro = true;
                objErro.cd_erro = ex.HResult;
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }

        public bool getCustomerByID(ref classClientes objCliente , int id   )
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spClientesObterPorID", conn);

            cmd.Parameters.AddWithValue("@id", id);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow obj in dt.Rows)
                        objCliente = fillRow(obj);
                }
            }
            catch (Exception ex)
            {
                objErro.strErro = ex.Message;
                objErro.erro = true;
                objErro.cd_erro = ex.HResult;
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }

        private classClientes fillRow(DataRow obj)
        {
            classClientes objCliente = new classClientes();

            objCliente.id = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(0).ToString());
            objCliente.nome = ((DataRow)obj).ItemArray.GetValue(1).ToString();
            objCliente.cd_planoSaude = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(2).ToString());
            objCliente.dataNascimento = Convert.ToDateTime(((DataRow)obj).ItemArray.GetValue(3).ToString());
            objCliente.cpf = ((DataRow)obj).ItemArray.GetValue(4).ToString();
            objCliente.telefone = ((DataRow)obj).ItemArray.GetValue(5).ToString();
            objCliente.planoSaude = ((DataRow)obj).ItemArray.GetValue(6).ToString();

            return objCliente;
        }

        internal bool listCustomerByCPF(ref List<classClientes> lstClientesTemp, string cpf)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spClientesListarPorCPF", conn);

            if (!string.IsNullOrEmpty(cpf))
                cmd.Parameters.AddWithValue("@cpf", cpf);

            //cmd.Parameters.AddWithValue("@Endereco", txtEndereco.Text);
            //cmd.Parameters.AddWithValue("@Cep", mskCep.Text);
            //cmd.Parameters.AddWithValue("@Bairro", txtBairro.Text);
            //cmd.Parameters.AddWithValue("@Cidade", txtCidade.Text);
            //cmd.Parameters.AddWithValue("@Uf", txtUf.Text);
            //cmd.Parameters.AddWithValue("@Telefone", mskTelefone.Text);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow obj in dt.Rows)
                        lstClientesTemp.Add(fillRow(obj));
                }
            }
            catch (Exception ex)
            {
                objErro.strErro = ex.Message;
                objErro.erro = true;
                objErro.cd_erro = ex.HResult;
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }

        internal bool deleteCustomerByID()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spClientesDeletar", conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();
                cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                objErro.strErro = ex.Message;
                objErro.erro = true;
                objErro.cd_erro = ex.HResult;
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }
        #endregion
    }
}
