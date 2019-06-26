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
    public class customerEvaluationPageModel : PageModel
    {
        #region properties
        public classErro objErro { get; set; }
        public List<classEvaluation> lstEvaluation { get; set; }
        public classClientes objCustomer { get; set; }
        #endregion

        public IActionResult OnGet(string strParams)
        {
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
                        bool redirect = false;
                        fillProperties(ObjValidation, ref redirect);

                        if (redirect)
                        {
                            classValidationQueryString objValidationReturn = new classValidationQueryString();
                            objValidationReturn.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");
                            objValidationReturn.insertQueryParam("inpSearch", TempData["inpSearch"].ToString());

                            TempData["alterSaveCustomerOK"] = "Cliente deletado com sucesso";

                            var strObjValidationReturn = JsonConvert.SerializeObject(objValidationReturn);
                            var strObjValidationReturnEncrypt = classModulo.Encrypt(strObjValidationReturn);

                            return RedirectToPage("customerPage", new { strParams = strObjValidationReturnEncrypt });
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

        public string truncateCustomerName()
        {
            var length = 15;
            return objCustomer.nome.truncate(length) + (objCustomer.nome.Length <= length ? string.Empty : ".");
        }

        private void fillProperties(classValidationQueryString objValidation, ref bool redirect)
        {
            objErro = new classErro();
            lstEvaluation = new List<classEvaluation>();

            if (objValidation.lstQueryStrings.Count != 0)
            {
                //string valueInput = string.Empty;
                string strCustomerID = string.Empty;

                objValidation.getQueryParam("customerID", ref strCustomerID);

                if (!string.IsNullOrEmpty(strCustomerID))
                {
                    classClientes objCliente = new classClientes();
                    classClientes objClienteTemp = new classClientes();

                    if (objCliente.getCustomerByID(ref objClienteTemp, Convert.ToInt32( strCustomerID)))
                    {
                        this.objCustomer = objClienteTemp;

                        List<classEvaluation> lstEvaluationTemp = new List<classEvaluation>();
                        classEvaluation objvaluation = new classEvaluation();

                        if (objvaluation.listEvaluationByCliente(ref lstEvaluationTemp, objCustomer.id))
                        {
                            if (lstEvaluationTemp.Count == 0)
                            {
                                objvaluation.objErro.erro = true;
                                objvaluation.objErro.strErroAmigavel = "Nenhuma avaliacao encontrada para o cliente: " + objCustomer.nome.Trim();
                                objErro = objvaluation.objErro;
                            }
                            else
                            {
                                lstEvaluation = lstEvaluationTemp;
                            }
                        }
                        else
                        {
                            objErro = objvaluation.objErro;
                        }
                    }
                    else
                    {
                        objErro = objCliente.objErro;
                    }
                }
                else
                {
                    objErro.erro = true;
                    objErro.strErro = "Erro ao receber parametros";
                }

                //else if (string.IsNullOrEmpty(valueInput) && !string.IsNullOrEmpty(valueID))
                //{
                //    classClientes objCliente = new classClientes();
                //    objCliente.id = Convert.ToInt32(valueID);

                //    if (objCliente.deleteCustomerByID())
                //    {
                //        redirect = true;
                //    }
                //    else
                //    {
                //        objErro = objCliente.objErro;
                //    }

                //}
            }
            else
            {
                objErro.erro = true;
                objErro.strErro = "Erro ao receber parametros";
            }
        }
    }
}
