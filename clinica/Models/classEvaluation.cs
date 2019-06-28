using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace clinica.Models
{
    public class classEvaluation
    {
        #region properties
        public int id { get; set; }
        public int cd_cliente { get; set; }
        public DateTime dataInicio { get; set; }
        public string diagnosticoMedico { get; set; }
        public string nomeMedico { get; set; }
        public string CRMmedico { get; set; }
        public string anamnese { get; set; }
        public string medicamentos { get; set; }
        public string doencasAssociadas { get; set; }
        public bool etilista { get; set; }
        public bool tabagista { get; set; }
        public string hobbies { get; set; }
        public string planoTratamento { get; set; }
        public int cd_tipoTratamento { get; set; }
        public Int16 EVA { get; set; }
        public string examesComplementares { get; set; }
        public string QueixaPrincipal { get; set; }
        public bool fechada { get; set; }
        public classErro objErro { get; set; }
        #endregion

        #region Methods
        public classEvaluation()
        {
            objErro = new classErro();
        }

        public bool insertAlter()
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spAvaliacoesInserirAlterar", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@cd_cliente", this.cd_cliente);
            cmd.Parameters.AddWithValue("@diagnosticoMedico", this.diagnosticoMedico);
            cmd.Parameters.AddWithValue("@nomeMedico", this.nomeMedico);
            cmd.Parameters.AddWithValue("@CRMmedico", this.CRMmedico);
            cmd.Parameters.AddWithValue("@anamnese", this.anamnese);
            cmd.Parameters.AddWithValue("@doencasAssociadas", this.doencasAssociadas);
            cmd.Parameters.AddWithValue("@etilista", this.@etilista);
            cmd.Parameters.AddWithValue("@tabagista", this.tabagista);
            cmd.Parameters.AddWithValue("@hobbies", this.hobbies);
            cmd.Parameters.AddWithValue("@planoTratamento", this.planoTratamento);
            cmd.Parameters.AddWithValue("@cd_tipoTratamento", this.cd_tipoTratamento);
            cmd.Parameters.AddWithValue("@EVA", this.EVA);
            cmd.Parameters.AddWithValue("@examesComplementares", this.examesComplementares);
            cmd.Parameters.AddWithValue("@QueixaPrincipal", this.QueixaPrincipal);
            cmd.Parameters.AddWithValue("@fechada", this.fechada);
            cmd.Parameters.AddWithValue("@medicamentos", this.@medicamentos);
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

        public bool deleteByID()
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spAvaliacoesDeletarPorID", conn);
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

        public bool getById(ref classEvaluation objEvaluation, int id)
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spAvaliacoesObterPorID", conn);
            cmd.Parameters.AddWithValue("@id", id);
           
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    objEvaluation = (fillRow(dt.Rows[0]));
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

        public bool listEvaluationByCliente(ref List<classEvaluation> lstEvaluation, int cd_cliente)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = classModulo.strConn;
            SqlCommand cmd = new SqlCommand("spAvaliacoesListar", conn);

            cmd.Parameters.AddWithValue("@cd_cliente", cd_cliente);

            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow obj in dt.Rows)
                        lstEvaluation.Add(fillRow(obj));
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

        private classEvaluation fillRow(DataRow obj)
        {
            classEvaluation objEvaluation = new classEvaluation();

            objEvaluation.id = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(0).ToString());
            objEvaluation.cd_cliente = Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(1).ToString());
            objEvaluation.dataInicio = Convert.ToDateTime(((DataRow)obj).ItemArray.GetValue(2).ToString());
            objEvaluation.diagnosticoMedico = (((DataRow)obj).ItemArray.GetValue(3).ToString());
            objEvaluation.nomeMedico = ((DataRow)obj).ItemArray.GetValue(4).ToString();
            objEvaluation.CRMmedico = ((DataRow)obj).ItemArray.GetValue(5).ToString();
            objEvaluation.anamnese = ((DataRow)obj).ItemArray.GetValue(6).ToString();
            objEvaluation.medicamentos = ((DataRow)obj).ItemArray.GetValue(7).ToString();
            objEvaluation.doencasAssociadas = ((DataRow)obj).ItemArray.GetValue(8).ToString();
            objEvaluation.etilista = Convert.ToBoolean(((DataRow)obj).ItemArray.GetValue(9).ToString());
            objEvaluation.tabagista = Convert.ToBoolean(((DataRow)obj).ItemArray.GetValue(10).ToString());
            objEvaluation.hobbies = ((DataRow)obj).ItemArray.GetValue(11).ToString();
            objEvaluation.planoTratamento = (((DataRow)obj).ItemArray.GetValue(12).ToString());
            objEvaluation.cd_tipoTratamento =  Convert.ToInt32(((DataRow)obj).ItemArray.GetValue(13).ToString());
            objEvaluation.EVA = Convert.ToInt16(((DataRow)obj).ItemArray.GetValue(14).ToString());
            objEvaluation.examesComplementares = (((DataRow)obj).ItemArray.GetValue(15).ToString());
            objEvaluation.QueixaPrincipal = (((DataRow)obj).ItemArray.GetValue(16).ToString());
            objEvaluation.fechada = Convert.ToBoolean(((DataRow)obj).ItemArray.GetValue(17).ToString());

            return objEvaluation;
        }
        #endregion

    }

}
