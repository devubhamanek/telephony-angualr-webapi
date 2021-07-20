//using Revoco.Models;
using Revoco_Model.Models;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using Revoco.DAL.Manager;
using Revoco.Common.Helpers;

namespace Revoco.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController"/>
    public class UserController : ApiController
    {

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/forgotpassword/{username}/{dnsname}")]
        public IHttpActionResult ForgotPassword(string username,string dnsname)
        {
            User user = new User();
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    using (UserManager _repo = new UserManager())
                    {
                        user = _repo.GetUserByUserName(username);
                        if (user != null)
                        {
                            Mailer objMailer = new Mailer();
                           
                            var isSendMail=objMailer.SendForgotPasswordLink(user.UserName, user.DisplayName, dnsname);
                            if(!isSendMail)
                            {
                                return Ok(Response.GenerateResponce("400", "Error while sending email.", null));
                            }
                            

                        }
                        else
                        {
                            return Ok(Response.GenerateResponce("400", "Email address is incorrect.", null));
                        }
                    }
                }
                else
                {
                    return Ok(Response.GenerateResponce("400", "Please fill up emai address.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(Response.GenerateResponce("400", "User not exists", null));
            }

            return Ok(Response.GenerateResponce("200", "Email password successfully sent to your email address.", null));
        }
        /// <summary>
        /// Save User Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/User/savepassword")]
        public bool SavePassword([FromBody] ResetPassword resetPassword)
        {
            bool savePassword = false;
            if (!string.IsNullOrEmpty(resetPassword.UserName) && !string.IsNullOrEmpty(resetPassword.Password))
            {
                using (UserManager _repo = new UserManager())
                {
                    try
                    {
                        _repo.SavePassword(resetPassword.UserName, resetPassword.Password);
                        savePassword = true;
                    }catch(Exception ex)
                    {
                        savePassword = false;
                    }
                    
                    
                }
            }
            return savePassword;
        }


        /// <summary>
        /// Check User Exists Or Not 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        ///
        [HttpGet]
        [Route("api/User/userexists/{userName}")]
        public User CheckUserExists(string userName)
        {
            User objUser = new User();

            if (!string.IsNullOrEmpty(userName))
                {
                    using (UserManager _repo = new UserManager())
                    {
                        try
                        {
                            objUser= _repo.GetUserByUserName(userName);
                            
                        }
                        catch (Exception ex)
                        {
                            return objUser;
                            
                        }


                    }
                }
          return objUser;
          

        }

        /// <summary>
        /// Get User Detail
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        ///
        [HttpGet]
        [Route("api/User/UserDetailByKey")]
        public IHttpActionResult GetUserByKey(string userName)
        {
            User objUser = new User();

            if (!string.IsNullOrEmpty(userName))
            {

                using (UserManager _repo = new UserManager())
                {
                    try
                    {
                        objUser = _repo.GetUserByUserName(Crypto.Decrypt(userName));

                    }
                    catch (Exception ex)
                    {
                        //return objUser;
                        return Ok(Response.GenerateResponce("400", "Error in find user", null));
                    }


                }
            }
            else
            {
                return Ok(Response.GenerateResponce("400", "User not exists.", objUser));
            }
            //return objUser;
            return Ok(Response.GenerateResponce("200", "Successfully get result.", objUser));

        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/User/ListAccounts/{userId}")]
        public IHttpActionResult GetListAccounts(string userId)
        {
            List<AccountViewModel> objAccountViewModel = new List<AccountViewModel>();
            try
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    using (UserManager _repo = new UserManager())
                    {
                        objAccountViewModel = _repo.GetListAccounts(userId);
                        if (objAccountViewModel != null)
                        {
                            return Ok(Response.GenerateResponce("200", "Success",objAccountViewModel));
                        }
                        else
                        {
                            return Ok(Response.GenerateResponce("400", "Account List Not Found.", null));
                        }
                    }
                }
                else
                {
                    return Ok(Response.GenerateResponce("400", "Userid in not valid.", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(Response.GenerateResponce("400",ex.ToString(), null));
            }

            
        }
    }
}
