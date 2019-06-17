using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using clinica.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;
namespace clinica.Pages
{
    public class IndexModel : PageModel
    {
        #region properties

        [BindProperty, Required(ErrorMessage = "Digite um usuario"), Display(Name = "Usuario")]
        public string inpUser { get; set; }
        [BindProperty, Required(ErrorMessage = "Digite uma senha"), Display(Name = "Senha")]
        public string inpPassword { get; set; }
        public classErro objErro { get; set; }
        public string strMsg { get; set; }

        #endregion

        public void OnGet(string objErroParam)
        {
            //classLog.writeLog2();

            if (!string.IsNullOrEmpty(objErroParam))
            {
                objErro = JsonConvert.DeserializeObject<classErro>(objErroParam);
            }
                 

        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                classUsuario objUsuario = new classUsuario();

                if (objUsuario.getUserByName(inpUser))
                {

                    if (objUsuario.id == 0)
                    {
                        objUsuario.objErro.strErroAmigavel = "Usuario nao cadastrado";
                        objUsuario.objErro.erro = true;
                        return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(objUsuario.objErro) });
                    }

                    else if (objUsuario.senha != inpPassword.Trim())
                    {
                        objUsuario.objErro.strErroAmigavel = "Senha incorreta!";
                        objUsuario.objErro.erro = true;
                        return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(objUsuario.objErro) });
                    }
                    else
                    {
                        classValidationQueryString objValidation = new classValidationQueryString();

                        HttpContext.Session.SetObjectAsJson("user", objUsuario);
                        objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                        var strObjValidation = JsonConvert.SerializeObject(objValidation);

                        var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                        return RedirectToPage("customerPage", new { strParams = strObjValidationEncrypt });
                    }
                        
                }
                else
                {
                    return RedirectToPage("index", new { objErroParam = JsonConvert.SerializeObject(objUsuario.objErro) });
                }

            }

            return Page();
        }
    }
}
