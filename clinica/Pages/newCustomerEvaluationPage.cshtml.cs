using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace clinica.Pages
{
    public class newCustomerEvaluationPageModel : PageModel
    {
        #region properties
        [BindProperty]
        public string inpID { get; set; }
        [BindProperty]
        public string inpCdCliente { get; set; }
        [BindProperty]
        public string inpCustomerCode { get; set; }
        [BindProperty]
        public string inpMedicDiagnosis { get; set; }
        [BindProperty]
        [MaxLength(30, ErrorMessage = "O nome do medico deve ter no maximo 30 letras")]
        public string inpMedicName{ get; set; }
        [BindProperty]
        [MaxLength(30, ErrorMessage = "O CRM do medico deve ter no maximo 30 digitos")]
        public string inpMedicCRM { get; set; }
        [BindProperty]
        public string inpAnamnesis { get; set; }
        [BindProperty]
        public string inpMedicines { get; set; }
        [BindProperty]
        public string inpAssociatedDiseases { get; set; }
        [BindProperty]
        public bool inpAlcoholist { get; set; }
        [BindProperty]
        public bool inpSmoker { get; set; }
        [BindProperty]
        public string inpHobbies { get; set; }
        [BindProperty]
        public string inpTreatmentPlan { get; set; }
        [BindProperty]
        public List<SelectListItem> lstTypesTreatment { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Selecione um tipo de tratamento"), Display(Name = "Tipo de tratamento")]
        public int slcTypesTreatment { get; set; }
        [BindProperty, Required(ErrorMessage = "Selecione um EVA"), Display(Name = "EVA")]
        public string slcEVA { get; set; }
        [BindProperty]
        public string inpComplementaryExams { get; set; }
        [BindProperty]
        public string inpMainComplaint { get; set; }
        [BindProperty]
        public bool close { get; set; }

        public classErro objErro { get; set; }
        public classClientes objCustomer { get; set; }
        public string strMsgOK { get; set; }
        public string strTitle { get; set; } = "Novo Cliente";

        #endregion
        public IActionResult OnGet(string strParams)
        {
            objErro = new classErro();
            var strErrorAuth = classModulo.strErrorAuth;

            try
            {
                if (!string.IsNullOrEmpty(strParams))
                {
                    var strObjValidation = classModulo.Decrypt(strParams);

                    var ObjValidation = JsonConvert.DeserializeObject<classValidationQueryString>(strObjValidation);

                    classErro objErroValidationUser = new classErro();

                    if (classModulo.userValidation(ObjValidation, ref objErroValidationUser, HttpContext.Session.GetObjectFromJson<classUsuario>("user")))
                    {
                        //execucao da logica da pagina
                        bool redirect = false;

                        fillProperties(ObjValidation, ref redirect);

                        if (redirect)
                        {
                            classValidationQueryString objValidationReturn = new classValidationQueryString();
                            objValidationReturn.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");
                            //objValidationReturn.insertQueryParam("inpSearch", TempData["inpSearch"].ToString());
                            objValidationReturn.insertQueryParam("customerID", inpCdCliente);
                            var strObjValidationReturn = JsonConvert.SerializeObject(objValidationReturn);
                            var strObjValidationReturnEncrypt = classModulo.Encrypt(strObjValidationReturn);

                            return RedirectToPage("customerEvaluationPage", new { strParams = strObjValidationReturnEncrypt });
                        }
                        return Page();
                    }
                    else
                        return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(objErroValidationUser) });
                }
                else
                    return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(new classErro { erro = true, strErro = strErrorAuth }) });
            }
            catch (Exception e)
            {
                string a = e.Message;
                return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(new classErro { erro = true, strErro = strErrorAuth }) });
            }
        }


        private void fillProperties(classValidationQueryString ObjValidation, ref bool redirect)
        {

            var cd_cliente = Convert.ToInt32( TempData["cdCliente"].ToString());
            inpCdCliente = cd_cliente.ToString();

            TempData["cdCliente"] = cd_cliente;

            classClientes objCustomerTemp = new classClientes();
            classClientes objCustomer = new classClientes();

            classTreatmentTypes objTypes = new classTreatmentTypes();
            List<classTreatmentTypes> lstTypes = new List<classTreatmentTypes>();

            if (objTypes.list(ref lstTypes))
            {
                this.lstTypesTreatment = lstTypes.Select(c => new SelectListItem() { Text = c.descricao, Value = c.codigo.ToString() }).ToList();

                if (objCustomer.getCustomerByID(ref objCustomerTemp, cd_cliente))
                {
                    this.objCustomer = objCustomerTemp;
                    

                    string valueObjEvaluation = string.Empty;
                    string valueCdEvaluation = string.Empty;

                    ObjValidation.getQueryParam("objEvaluation", ref valueObjEvaluation);
                    ObjValidation.getQueryParam("cdEvaluation", ref valueCdEvaluation);

                    if (!string.IsNullOrEmpty(valueObjEvaluation))
                    {
                        classEvaluation objEvaluation = JsonConvert.DeserializeObject<classEvaluation>(valueObjEvaluation);

                        if (objEvaluation.insertAlter())
                        {
                            redirect = true;

                            if (objEvaluation.id == 0)
                                TempData["alterSaveEvaluationOK"] = "Avaliacao salva com sucesso!";
                            else
                                TempData["alterSaveEvaluationOK"] = "Avaliacao alterada com sucesso!";
                        }
                        else
                        {
                            objErro = objEvaluation.objErro;
                        }
                    }
                    else if (string.IsNullOrEmpty(valueObjEvaluation) && !string.IsNullOrEmpty(valueCdEvaluation))
                    {
                        classEvaluation objEvaluation = new classEvaluation();
                        classEvaluation objEvaluationTemp = new classEvaluation();

                        if (objEvaluation.getById(ref objEvaluationTemp, Convert.ToInt32(valueCdEvaluation)))
                        {
                            fillforms(objEvaluationTemp);
                        }
                        else
                        {
                            objErro = objEvaluation.objErro;
                        }
                    }
                }
                else
                {
                    objErro = objCustomer.objErro;
                }
            }
            else
            {
                objErro = objTypes.objErro;
            }
        }

        private void fillforms(classEvaluation objEvaluationTemp)
        {
            inpID = objEvaluationTemp.id.ToString();
            inpCdCliente = objEvaluationTemp.cd_cliente.ToString();
            inpMedicDiagnosis = objEvaluationTemp.diagnosticoMedico.ToString();
            inpMedicName = objEvaluationTemp.nomeMedico.ToString();
            inpMedicCRM  = objEvaluationTemp.CRMmedico.ToString();
            inpAnamnesis = objEvaluationTemp.anamnese.ToString();
            inpMedicines = objEvaluationTemp.medicamentos.ToString();
            inpAlcoholist = objEvaluationTemp.etilista;
            inpSmoker = objEvaluationTemp.tabagista;
            inpHobbies = objEvaluationTemp.hobbies.ToString();
            inpAssociatedDiseases = objEvaluationTemp.doencasAssociadas.ToString();
            inpTreatmentPlan = objEvaluationTemp.planoTratamento.ToString();
            slcTypesTreatment = Convert.ToInt32( objEvaluationTemp.cd_tipoTratamento.ToString());
            slcEVA = objEvaluationTemp.EVA.ToString();
            inpComplementaryExams = objEvaluationTemp.examesComplementares.ToString();
            inpMainComplaint = objEvaluationTemp.QueixaPrincipal.ToString();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                classValidationQueryString objValidation = new classValidationQueryString();
                objErro = new classErro();

                classEvaluation objClassEvaluation = new classEvaluation();

                if (!evaluationValidation(ref objClassEvaluation))
                {
                    objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                    objValidation.insertQueryParam("objErro", JsonConvert.SerializeObject(objErro));
                    objValidation.insertQueryParam("objEvaluation", JsonConvert.SerializeObject(objClassEvaluation));

                    var strObjValidation = JsonConvert.SerializeObject(objValidation);

                    var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                    return RedirectToPage("newCustomerPage", new { strParams = strObjValidationEncrypt });
                }
                else
                {

                    objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                    objValidation.insertQueryParam("objEvaluation", JsonConvert.SerializeObject(objClassEvaluation));

                    var strObjValidation = JsonConvert.SerializeObject(objValidation);

                    var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                    return RedirectToPage("newCustomerEvaluationPage", new { strParams = strObjValidationEncrypt });
                }
            }

            return Page();
        }

        private bool evaluationValidation(ref classEvaluation objClassEvaluation)
        {
            bool valid = false;
            string strValid = string.Empty;

            objClassEvaluation.id = Convert.ToInt32(inpID);
            objClassEvaluation.cd_cliente = Convert.ToInt32(inpCdCliente);
            objClassEvaluation.diagnosticoMedico = inpMedicDiagnosis;
            objClassEvaluation.nomeMedico = inpMedicName;
            objClassEvaluation.CRMmedico = inpMedicCRM;
            objClassEvaluation.anamnese = inpAnamnesis;
            objClassEvaluation.medicamentos = inpMedicines;
            objClassEvaluation.doencasAssociadas = inpAssociatedDiseases;
            objClassEvaluation.etilista = Convert.ToBoolean(inpAlcoholist);
            objClassEvaluation.tabagista = Convert.ToBoolean(inpSmoker);
            objClassEvaluation.hobbies = inpHobbies;
            objClassEvaluation.planoTratamento = inpTreatmentPlan;
            objClassEvaluation.cd_tipoTratamento = Convert.ToInt32(slcTypesTreatment) == 0 ? 1 : Convert.ToInt32(slcTypesTreatment);
            objClassEvaluation.EVA = Convert.ToInt16(slcEVA);
            objClassEvaluation.examesComplementares = inpComplementaryExams;
            objClassEvaluation.QueixaPrincipal = inpMainComplaint;

            valid = true;
            return valid;
        }
    }
}
