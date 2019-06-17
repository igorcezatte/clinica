using System;
using System.Collections.Generic;
using clinica.Models;

namespace clinica.Models
{
    public class classValidationQueryString
    {
        #region properties
        public classUsuario objUsuario { get; set; }
        public List<string> lstQueryStrings { get; set; }

        #endregion

        #region methods
        public classValidationQueryString()
        {
            lstQueryStrings = new List<string>();
        }

        public void insertQueryParam(string key, string value)
        {
            var param = key + "=" + value;
            lstQueryStrings.Add(param);
        }

        public bool getQueryParam(string key, ref string value)
        {
            try
            {
                foreach (var obj in lstQueryStrings)
                {
                    if( key == (new List<string>(obj.Split("=")))[0] ? true : false)
                    {
                        value = (new List<string>(obj.Split("=")))[1];
                        return true;
                    }
                }

                return false;
                    
            }
            catch(Exception EX)
            {
                string a = EX.Message;
                value = string.Empty;
                return false;
            }
        }
        #endregion

    }
}
