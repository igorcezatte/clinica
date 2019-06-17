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
    public class newCustomerPageModel : PageModel
    {
        #region properties
        [BindProperty]
        public string inpID { get; set; }
        [BindProperty, Required(ErrorMessage = "Digite um nome"), Display(Name = "Nome completo")]
        [MaxLength(40, ErrorMessage = "O nome deve ter no maximo 40 letras")]
        public string inpName { get; set; }
        [BindProperty, Required(ErrorMessage = "Digite um telefone"), Display(Name = "Telefone")]
        public string inpTelephone { get; set; }
        [BindProperty, Required(ErrorMessage = "Digite um CPF"), Display(Name = "CPF")]
        //[RegularExpression("([0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2})", ErrorMessage = "CPF invalido")]
        public string inpCPF { get; set; }
        [BindProperty, 
            Required(ErrorMessage = "Digite uma data de nascimento"), Display(Name = "Data nascimento"),
            //RegularExpression("([0-9]{2}/[0-9]{2}/[0-9]{4})", ErrorMessage = "Data invalida")
        ]
        public string inpDate { get; set; }

        [BindProperty, Required(ErrorMessage ="Selecione um plano de saude"), Display(Name = "Plano")]
        public int selectedPlanoSaude { get; set; }

        public List<SelectListItem> lstPlanoSaude  { get; set; }
        public classErro objErro { get; set; }
        public string strMsgOK { get; set; }
        public string strTitulo { get; set; } = "Novo Cliente";

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
                            objValidationReturn.insertQueryParam("inpSearch", TempData["inpSearch"].ToString());

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
                return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(new classErro { erro = true, strErro = strErrorAuth }) });
            }


        }

        private void fillProperties(classValidationQueryString ObjValidation, ref bool redirect)
        {
            classPlanoSaude objPlanoSaude = new classPlanoSaude();
            List<classPlanoSaude> lstPlanoSaudeTemp = new List<classPlanoSaude>();

            if (objPlanoSaude.listPlanoSaude(ref lstPlanoSaudeTemp))
            {
                lstPlanoSaude = lstPlanoSaudeTemp.Select(c => new SelectListItem(){ Text = c.descricao, Value = c.codigo.ToString() }).ToList();

                if (ObjValidation.lstQueryStrings.Count != 0)
                {
               
                string valueObjErro = string.Empty;
                string valueObjCliente = string.Empty;
                string valueID = string.Empty;

                ObjValidation.getQueryParam("objErro", ref valueObjErro);
                ObjValidation.getQueryParam("objCliente", ref valueObjCliente);
                ObjValidation.getQueryParam("id", ref valueID);


                if (!string.IsNullOrEmpty(valueObjErro))
                {
                    objErro = JsonConvert.DeserializeObject<classErro>(valueObjErro);
                    classClientes objCliente = JsonConvert.DeserializeObject<classClientes>(valueObjCliente);

                    if (objCliente == null)
                        return;

                    fillForms(objCliente);

                }
                else if (!string.IsNullOrEmpty(valueObjCliente) && string.IsNullOrEmpty(valueObjErro))
                {
                    classClientes objCliente = JsonConvert.DeserializeObject<classClientes>(valueObjCliente);

                    if (objCliente.insertAlterCliente())
                    {
                        if (objCliente.id == 0)
                        {
                            redirect = true;
                            TempData["alterSaveCustomerOK"] = "Cliente salvo com sucesso!";
                            TempData["inpSearch"] = objCliente.nome;
                        }
                        else
                        {
                            redirect = true;
                            TempData["alterSaveCustomerOK"] = "Cliente alterado com sucesso!";
                        }
                    }
                    else
                    {
                        fillForms(objCliente);
                        objErro = objCliente.objErro;
                    }

                }
                else if (!string.IsNullOrEmpty(valueID) && string.IsNullOrEmpty(valueObjCliente) && string.IsNullOrEmpty(valueObjErro))
                {
                    classClientes objCliente = new classClientes();

                    if (objCliente.getCustomerByID(ref objCliente, Convert.ToInt32(valueID)))
                    {
                        fillForms(objCliente);
                    }
                    else
                    {
                        objErro = objCliente.objErro;
                    }
                }

                }
            }
            else
            {
                objErro = objPlanoSaude.objErro;
            }

        }

        private void fillForms(classClientes objCliente)
        {
            strTitulo = objCliente.id == 0 ? strTitulo : "Alterar dados";
            selectedPlanoSaude = objCliente.cd_planoSaude;
            inpID = objCliente.id.ToString();
            inpName = objCliente.nome;
            inpCPF = string.IsNullOrEmpty(objCliente.cpf) ? string.Empty : objCliente.cpf;
            inpTelephone = string.IsNullOrEmpty(objCliente.telefone) ? string.Empty : objCliente.telefone;
            inpDate = objCliente.dataNascimento == DateTime.MinValue ? string.Empty : objCliente.dataNascimento.ToString("dd/MM/yyyy");
        }

        private bool userValidation(ref classClientes objCliente)
        {
            bool valid = false;
            string strValid = string.Empty;

            objCliente.id = Convert.ToInt32(inpID);
            objCliente.nome = inpName;
            objCliente.cpf = inpCPF;
            //fazer isso
            objCliente.cd_planoSaude = selectedPlanoSaude;
            objCliente.telefone = inpTelephone;

            // --------   validar data --------------
           
            if (classModulo.dateValidation(inpDate))
            {
                objCliente.dataNascimento = Convert.ToDateTime(inpDate, new CultureInfo("pt-BR").DateTimeFormat);
                valid = true;
            }
            else
            {
                valid = false;
                strValid = "A data fornecida, "+ inpDate + ", nao eh valida;";
                objCliente.dataNascimento = DateTime.MinValue;
            }

            // -------------- validar cpf --------------

            if (classModulo.cpfValidation(inpCPF))
            {
                objCliente.cpf = inpCPF;
            }
            else
            {
                valid = false;
                var strCPFinvalid = "O cpf fornecido, " + inpCPF + ", nao eh valido;";
                strValid = string.IsNullOrEmpty(strValid) ? strCPFinvalid : strValid + Environment.NewLine + strCPFinvalid;
                objCliente.cpf = string.Empty;
            }

            if (valid) 
                return true;
            else
            {
                objErro.erro = true;
                objErro.strErroAmigavel = strValid;
                return false;
            }
        }

        //public IActionResult OnPostBackPage()
        //{

        //    classValidationQueryString objValidation = new classValidationQueryString();

        //    objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

        //    //objValidation.insertQueryParam("inpSearch", inpSearch);

        //    var strObjValidation = JsonConvert.SerializeObject(objValidation);

        //    var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

        //    return RedirectToPage("newCustomerPage", new { strParams = strObjValidationEncrypt });

        //}
      
        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                classValidationQueryString objValidation = new classValidationQueryString();
                objErro = new classErro();

                classClientes objCliente = new classClientes();

                if (!userValidation(ref objCliente))
                {
                    objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                    objValidation.insertQueryParam("objErro", JsonConvert.SerializeObject(objErro));
                    objValidation.insertQueryParam("objCliente", JsonConvert.SerializeObject(objCliente));

                    var strObjValidation = JsonConvert.SerializeObject(objValidation);

                    var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                    return RedirectToPage("newCustomerPage", new { strParams = strObjValidationEncrypt });
                }
                else
                {
                  
                    objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                    objValidation.insertQueryParam("objCliente", JsonConvert.SerializeObject(objCliente));

                    var strObjValidation = JsonConvert.SerializeObject(objValidation);

                    var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                    return RedirectToPage("newCustomerPage", new { strParams = strObjValidationEncrypt });
                }
            }

            return Page();
        }
    }
}
