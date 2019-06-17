using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using clinica.Models;
using Newtonsoft.Json;

namespace clinica.Pages
{
    public class customerPageModel : PageModel
    {
        #region properties

        [BindProperty]
        public string inpSearch { get; set; }
        public classErro objErro  { get; set; }
        public List<classClientes> lstClientes { get; set; }
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
            catch(Exception e)
            {
                string a = e.Message;
                return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(new classErro { erro = true, strErro = strErrorAuth}) });
            }

        }

        private void fillProperties(classValidationQueryString ObjValidation, ref bool redirect)
        {
            objErro = new classErro();
            lstClientes = new List<classClientes>();

            if (ObjValidation.lstQueryStrings.Count != 0)
            {
                string valueInput = string.Empty;
                string valueID = string.Empty;

                ObjValidation.getQueryParam("inpSearch", ref valueInput);
                ObjValidation.getQueryParam("id", ref valueID);

                if (!string.IsNullOrEmpty(valueInput))
                {
                    inpSearch = valueInput;

                    long cpf;
                    bool isCpf = long.TryParse(valueInput, out cpf);

                    if (!isCpf)
                    {
                        List<classClientes> lstClientesTemp = new List<classClientes>();
                        classClientes objCliente = new classClientes();

                        if (objCliente.listCustomerByName(ref lstClientesTemp, valueInput))
                        {
                            if (lstClientesTemp.Count == 0)
                            {
                                objCliente.objErro.erro = true;
                                objCliente.objErro.strErroAmigavel = "Nenhum cliente encontrado com as palavras chaves: " + valueInput;
                                objErro = objCliente.objErro;
                            }
                            else
                            {
                                lstClientes = lstClientesTemp;
                            }
                        }
                        else
                        {
                            objErro = objCliente.objErro;
                        }
                    }
                    else
                    {
                        List<classClientes> lstClientesTemp = new List<classClientes>();
                        classClientes objCliente = new classClientes();

                        if (objCliente.listCustomerByCPF(ref lstClientesTemp, cpf.ToString()))
                        {
                            if (lstClientesTemp.Count == 0)
                            {
                                objCliente.objErro.erro = true;
                                objCliente.objErro.strErroAmigavel = "Nenhum cliente encontrado com CPF: " + cpf;
                                objErro = objCliente.objErro;
                                inpSearch = string.Empty;
                            }
                            else
                            {
                                lstClientes = lstClientesTemp;
                            }
                        }
                        else
                        {
                            objErro = objCliente.objErro;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(valueInput) && !string.IsNullOrEmpty(valueID))
                {
                    classClientes objCliente = new classClientes();
                    objCliente.id = Convert.ToInt32(valueID);

                    if (objCliente.deleteCustomerByID())
                    {
                        redirect = true;
                    }
                    else
                    {
                        objErro = objCliente.objErro;
                    }

                }
            }

        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrEmpty(inpSearch))
            {
                classValidationQueryString objValidation = new classValidationQueryString();

                objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                objValidation.insertQueryParam("inpSearch", inpSearch);

                var strObjValidation = JsonConvert.SerializeObject(objValidation);

                var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                TempData["inpSearch"] = inpSearch;

                return RedirectToPage("customerPage", new { strParams = strObjValidationEncrypt });
            }

            return Page();
        }

        public IActionResult OnPostEdit(int id)
        {
            classValidationQueryString objValidation = new classValidationQueryString();

            objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

            objValidation.insertQueryParam("id", id.ToString());

            var strObjValidation = JsonConvert.SerializeObject(objValidation);

            var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

            return RedirectToPage("newCustomerPage", new
            {
                strParams = strObjValidationEncrypt
            });
        }

        public IActionResult OnPostDelete(int id)
        {
            classValidationQueryString objValidation = new classValidationQueryString();

            objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

            objValidation.insertQueryParam("id", id.ToString());
           
            var strObjValidation = JsonConvert.SerializeObject(objValidation);

            var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

            TempData["inpSearch"] = inpSearch;

            return RedirectToPage("customerPage", new
            {
                strParams = strObjValidationEncrypt
            });
        }
    }
}
