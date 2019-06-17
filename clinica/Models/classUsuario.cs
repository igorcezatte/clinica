using System;
using System.Data;
using System.Data.SqlClient;

namespace clinica.Models
{
    public class classUsuario
    {
        #region properties
        public int id { get; set; }
        public string usuario { get; set; }
        public string senha { get; set; }
        public bool adm  {get; set; }
        public classErro objErro { get; set; }
        #endregion


    #region methods
    public classUsuario()
        {
            objErro = new classErro();
        }

        public bool getUserByName(string name)
        {
           
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spUsuariosObter", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

               
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    fillRow(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                objErro.strErro = ex.Message;
                objErro.erro = false;
                objErro.cd_erro = ex.HResult;
                return false;
            }
            finally
            {
                conn.Close();
            }
            return true;
        }

        private void fillRow(DataRow obj)
        {
            this.id = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(0).ToString());
            this.usuario = ((DataRow)obj).ItemArray.GetValue(1).ToString();
            this.senha = ((DataRow)obj).ItemArray.GetValue(2).ToString();
            this.adm = Convert.ToBoolean( ((DataRow)obj).ItemArray.GetValue(3).ToString());
        }
        #endregion
    }
}
