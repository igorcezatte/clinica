using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace clinica.Models
{
    public static class classModulo
    {

        #region properties

        public static string strConn { get; set; } =
            "Data Source=clinica.brazilsouth.cloudapp.azure.com;" +
            "Initial Catalog=desenvolvimento;" +
            "User id=sa;" +
            "Password=Clinica123456;";

        public static string strErrorAuth = "Erro de autenticacao";
        public static string strAuthNotValid = "Autenticacao nao autorizada";
        public static string strAuthExpired = "Sessao Expirada";

        #endregion

        #region methods

        // ----------- encrypt ---------------
        public static string Encrypt(string textToEncrypt)
        {

            try
            {
                textToEncrypt = textToEncrypt.Trim();

                Byte[] b = System.Text.ASCIIEncoding.UTF8.GetBytes(textToEncrypt);
                return Convert.ToBase64String(b);
            }
            catch
            {
                return string.Empty;
            }
        }

        // ----------- decrypt --------------- 
        public static string Decrypt(string textToDecrypt)
        {

            try
            {
                if (textToDecrypt == null)
                    return string.Empty;

                Byte[] b = Convert.FromBase64String(textToDecrypt);
                return System.Text.ASCIIEncoding.UTF8.GetString(b);
            }
            catch (Exception ex)
            {
                //return string.Empty;
                return ex.Message;

            }
           
        }

        // ----------- cpf validation --------------- 

        public static bool cpfValidation(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCpf = tempCpf + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);

        }



            // ----------- date validation --------------- 
            public static bool dateValidation(string date)
        {
            DateTime temp;
           
            var dateFormats = new[] { "dd/MM/yyyy" };
            bool validDate = DateTime.TryParseExact(
            date,
            dateFormats,
            DateTimeFormatInfo.InvariantInfo,
            DateTimeStyles.None,
            out temp);

            if (temp.Month > 12 || temp.Day > 31 || temp.Year < 1901 || temp.Year > DateTime.Now.Year)
                validDate = false;

            return validDate;

        }

        public static bool dateValidation(string date, ref DateTime dateOut)
        {
            if (dateValidation(date))
            {
                dateOut = Convert.ToDateTime(date, new CultureInfo("pt-BR").DateTimeFormat);
                return true;
            }
            else
            {
                return false;
            }
        }
        // ----------- auth Validation --------------- 
        public static bool userValidation(classValidationQueryString ObjValidation, ref classErro objErro, classUsuario objUserSession)
        {
            var erro = string.Empty;

            try
            {
                if ( objUserSession != null)
                {
                    if (ObjValidation != null)
                    {
                        if (ObjValidation.objUsuario.id == objUserSession.id)
                        {
                            return true;
                            // continuar verificacoes se precisar
                        }
                        else
                        {
                            objErro.erro = true;
                            objErro.strErroAmigavel = strAuthNotValid;
                            return false;
                        }
                    }
                    else
                    {
                        objErro.erro = true;
                        objErro.strErroAmigavel = strAuthNotValid;
                        return false;
                    }
                }
                else
                {
                    objErro.erro = true;
                    objErro.strErroAmigavel = strAuthExpired;
                    return false;
                }

            }
            catch (Exception ex)
            {
                objErro.erro = true;
                objErro.strErro = ex.Message;
                return false;
            }

        }
        #endregion



    }
}
