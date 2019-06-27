using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using clinica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace clinica.Pages
{
    public class redirectPageModel : PageModel
    {
        enum enumRedirect { customerPage = 1, newCustomerPage = 2, newCustomerEvaluationPage = 3 };

        public IActionResult OnGet(int id, int cdCliente)
        {
            // Clientes
            if (id == (int)enumRedirect.customerPage)
            {
                classValidationQueryString objValidation = new classValidationQueryString();
                objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");
                var strObjValidation = JsonConvert.SerializeObject(objValidation);
                var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                return RedirectToPage("customerPage", new { strParams = strObjValidationEncrypt });
            }

            if (id == (int)enumRedirect.newCustomerPage)
            {
                classValidationQueryString objValidation = new classValidationQueryString();

                objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                //objValidation.insertQueryParam("inpSearch", inpSearch);

                var strObjValidation = JsonConvert.SerializeObject(objValidation);

                var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                return RedirectToPage("newCustomerPage", new { strParams = strObjValidationEncrypt });
            }

            if (id == (int)enumRedirect.newCustomerEvaluationPage)
            {
                classValidationQueryString objValidation = new classValidationQueryString();

                objValidation.objUsuario = HttpContext.Session.GetObjectFromJson<classUsuario>("user");

                TempData["cdCliente"] = cdCliente;
                //objValidation.insertQueryParam("inpSearch", inpSearch);

                var strObjValidation = JsonConvert.SerializeObject(objValidation);

                var strObjValidationEncrypt = classModulo.Encrypt(strObjValidation);

                return RedirectToPage("newCustomerEvaluationPage", new { strParams = strObjValidationEncrypt });
            }

            return Page();
        }

        public void OnPost()
        {

        }

    }
}
