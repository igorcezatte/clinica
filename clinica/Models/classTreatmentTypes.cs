using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace clinica.Models
{
    public class classTreatmentTypes
    {

        #region properties
       
        public int codigo { get; set; }
        public string descricao { get; set; }
        public classErro objErro { get; set; }
        #endregion

        #region Methods
        public classTreatmentTypes()
        {
            objErro = new classErro();
        }

        public bool list(ref List<classTreatmentTypes> lstTreatment)
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spTipoTratamentoListar", conn);
          
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow obj in dt.Rows)
                        lstTreatment.Add(fillRow(obj));
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

        private classTreatmentTypes fillRow(DataRow obj)
        {
            classTreatmentTypes objTreatment= new classTreatmentTypes();
            objTreatment.codigo = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(0).ToString());
            objTreatment.descricao = (((DataRow)obj).ItemArray.GetValue(1).ToString());

            return objTreatment;
        }
        #endregion
       
    }
}
