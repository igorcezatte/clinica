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
        public int inpMedicines { get; set; }
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
        public int slcTypesTreatment { get; set; }
        [BindProperty]
        public int slcEVA { get; set; }
        [BindProperty]
        public string inpComplementaryExams { get; set; }
        [BindProperty]
        public string inpMainComplaint { get; set; }
        [BindProperty]
        public bool close { get; set; }

        public classErro objErro { get; set; }
        public string strMsgOK { get; set; }
        public string strTitle { get; set; } = "Novo Cliente";

        #endregion
        public void OnGet()
        {
        }
    }
}
