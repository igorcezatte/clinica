using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace clinica.Models
{
    public class classPlanoSaude
    {
        #region properties
        public int codigo { get; set; }
        public string descricao { get; set; }
        public classErro objErro { get; set; }
        #endregion



        #region methods
        public classPlanoSaude()
        {
            objErro = new classErro();
        }

        public bool listPlanoSaude(ref List<classPlanoSaude> lstPlanosSaude)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spPlanoSaudeListar", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow obj in dt.Rows)
                        lstPlanosSaude.Add(fillRow(obj));
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

        private classPlanoSaude fillRow(DataRow obj)
        {
            classPlanoSaude objCliente = new classPlanoSaude();

            objCliente.codigo = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(0).ToString());
            objCliente.descricao = ((DataRow)obj).ItemArray.GetValue(2).ToString();
           
            return objCliente;
        }
        #endregion
    }
}
