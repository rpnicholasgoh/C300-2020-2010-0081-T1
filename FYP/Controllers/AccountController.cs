using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Security.Claims;
using FYPDraft.Models;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FYPDraft.Controllers
{
    public class AccountController : Controller
    {

        private const string LOGIN_SQL =
           @"SELECT * FROM Users 
            WHERE Username = '{0}' 
              AND Password = HASHBYTES('SHA1', '{1}')";

        private const string LASTLOGIN_SQL =
           @"UPDATE Users SET LastLogin=GETDATE() WHERE Username='{0}'";

        private const string ROLE_COL = "UserRole";
        private const string NAME_COL = "FullName";

        private const string REDIRECT_CNTR = "Portal";
        private const string REDIRECT_ACTN = "Home";

        private const string LOGIN_VIEW = "Login";

        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(LOGIN_VIEW);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(UserLogin user)
        {
            if (!AuthenticateUser(user.Username, user.Password, out ClaimsPrincipal principal))
            {
                ViewData["Message"] = "Incorrect User ID or Password";
                ViewData["MsgType"] = "warning";
                return View(LOGIN_VIEW);
            }
            else
            {
                HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   principal,
               new AuthenticationProperties
               {
                   IsPersistent = user.RememberMe
               });

                // Update the Last Login Timestamp of the User
                DBUtl.ExecSQL(LASTLOGIN_SQL, user.Username);

                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
            }
        }

        [Authorize]
        public IActionResult Logoff(string returnUrl = null)
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }

        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            List<User> list = DBUtl.GetList<User>("SELECT * FROM Users WHERE UserRole='Alumni' AND UserRole='Startup'");
            return View(list);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string name)
        {
            string delete = "DELETE FROM Users WHERE Username='{0}'";
            int res = DBUtl.ExecSQL(delete, name);
            if (res == 1)
            {
                TempData["Message"] = "User Record Deleted";
                TempData["MsgType"] = "success";
            }
            else
            {
                TempData["Message"] = DBUtl.DB_Message;
                TempData["MsgType"] = "danger";
            }

            return RedirectToAction("Users");
        }

        [AllowAnonymous]
        public IActionResult ForgotPwd()
        {
            return View("ForgotPwd");
        }

        [HttpPost]
        public IActionResult ForgotPwd(ForgotPwd forgotPwd)
        {
            string email = forgotPwd.Email.ToString();
            string select = "SELECT * FROM Users WHERE Email='{0}'";
            DataTable dt = DBUtl.GetTable(select, email);

            foreach (DataRow row in dt.Rows)
            {
                string fname = row.Field<string>("FullName");
                string Email = row.Field<string>("Email");
                string template = @"Hi {0}, <br></br>
                                  Request to change password";
                string title = "Reset Password";
                string message = String.Format(template, fname);
                string result;

                if (EmailUtl.SendEmail(email, title, message, out result))
                {
                    ViewData["Message"] = "Email Successfully Sent";
                    ViewData["MsgType"] = "success";
                }
                else
                {
                    ViewData["Message"] = result;
                    ViewData["MsgType"] = "warning";
                }

                return View("ForgotPwdCfm");
            }

            //Create necessary database to store user info if required
            /*To code for forgot password to send user email for the password reset link*/

            return View(); //TO remove or edit this line of code//
        }

        public IActionResult ForgotPwdCfm()
        {
            return View("ForgotPwdCfm");
        }

        public IActionResult ResetPwd()
        {
            return View("ResetPassword");
        }

        [HttpPost]
        public IActionResult ResetPwd(ResetPwd reset)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Message"] = "Invalid Input";
                ViewData["MsgType"] = "warning";
                return View("ResetPassword");
            }
            else
            {
                string Username = reset.Username.ToString();
                string currentpwd = reset.currentPwd.ToString();
                string password = reset.UserPw.ToString();
                string cfmpassword = reset.UserPw2.ToString();
                string sql = @"SELECT * FROM Users WHERE Username='{0}'";
                Console.WriteLine(sql);
                string select = String.Format(sql, Username);
                Console.WriteLine("select");
                DataTable dt = DBUtl.GetTable(select);
                foreach (DataRow row in dt.Rows)
                {
                    string uname = row.Field<string>("Username");



                    if (password.Equals(cfmpassword))
                    {
                        string update = @"UPDATE Users SET Password=HASHBYTES('SHA1','{2}') WHERE Username = '{0}'";
                        Console.WriteLine(update);
                        int res = DBUtl.ExecSQL(update, uname, currentpwd, cfmpassword);
                        Console.WriteLine(res);
                        if (res == 1)
                        {
                            ViewData["Message"] = "Password has been reset successfully.";
                            ViewData["MsgType"] = "success";
                            return View("ResetPassword");
                        }
                        else
                        {
                            ViewData["Message"] = "Password reset unsuccessful.";
                            ViewData["MsgType"] = "warning";
                            return View("ResetPassword");
                        }
                    }
                    else
                    {
                        ViewData["Message"] = "Username cannot be verified.";
                        ViewData["MsgType"] = "danger";
                        return View("ResetPassword");
                    }
                }
            }
            return View("ResetPassword");
        }



        [Authorize(Roles = "Admin")]
public IActionResult Register()
{
    return View("RegisterUser");
}

[HttpPost]
[Authorize(Roles = "Admin")]
public IActionResult Register(User usr)
{
    if (!ModelState.IsValid)
    {
        ViewData["Message"] = "Invalid Input";
        ViewData["MsgType"] = "warning";
        return View("RegisterUser");
    }
    else
    {
        string insert = @"INSERT INTO User(Username, Password, FullName, Email, UserRole) VALUES('{0}', HASHBYTES('SHA1', '{1}', '{2}', '{3}', 'Startup')";
        if (DBUtl.ExecSQL(insert, usr.Username, usr.Password, usr.FullName, usr.Email) == 1)
        {
            string template = @"Hi {0}, <br/><br/>
                                      Welcome to StartUp Accelerator!
                                      Your username is <b>{1}</b> and password is <b>{2}</b>.
                                      <br/><br/>Admin";
            string title = "Registration Successful - Welcome";
            string message = String.Format(template, usr.FullName, usr.Username, usr.Password);
            string result = "";

            bool outcome = false;

            outcome = EmailUtl.SendEmail(usr.Email, title, message, out result);

            if (outcome)
            {
                ViewData["Message"] = "User Successfully Registered";
                ViewData["MsgType"] = "success";
            }
            else
            {
                ViewData["Message"] = result;
                ViewData["MsgType"] = "warning";
            }
        }
        else
        {
            ViewData["Message"] = DBUtl.DB_Message;
            ViewData["MsgType"] = "danger";
        }
        return View("RegisterUser");
    }
}

[AllowAnonymous]
public IActionResult VerifyUserID(string username)
{
    string select = $"SELECT * FROM Users WHERE Username='{username}'";
    if (DBUtl.GetTable(select).Rows.Count > 0)
    {
        return Json($"[{username}] already in use");
    }
    return Json(true);
}

private bool AuthenticateUser(string uname, string pw, out ClaimsPrincipal principal)
{
    principal = null;

    DataTable ds = DBUtl.GetTable(LOGIN_SQL, uname, pw);
    if (ds.Rows.Count == 1)
    {
        principal =
           new ClaimsPrincipal(
              new ClaimsIdentity(
                 new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, uname),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                 }, "Basic"
              )
           );
        return true;
    }
    return false;
}

public IActionResult ListUsers()
{
    List<User> list = DBUtl.GetList<User>(
        @"SELECT * FROM Users");
    return View(list);
}

[Authorize(Roles = "Admin")]
public IActionResult Create()
{
    return View();
}

[HttpPost]
public IActionResult Create(User newUser)
{
    if (!ModelState.IsValid)
    {
        ViewData["Message"] = "Invalid Input";
        ViewData["MsgType"] = "warning";
        return View("Create");
    }
    else
    {
        string insert =
            @"INSERT INTO Users(Username, FullName, Email, Password, UserRole, CompanyName, ContactNo) 
                VALUES('{0}', '{1}', '{2}', HASHBYTES('SHA1', '{3}'), '{4}', '{5}', '{6}')";

        int result = DBUtl.ExecSQL(insert, newUser.Username, newUser.FullName, newUser.Email, newUser.Password, newUser.UserRole, newUser.CompanyName, newUser.ContactNo);

        if (result == 1)
        {
            TempData["Message"] = "User Created";
            TempData["MsgType"] = "success";
        }
        else
        {
            TempData["Message"] = DBUtl.DB_Message;
            TempData["MsgType"] = "danger";
        }
        return RedirectToAction("ListUsers");
    }
}

[Authorize(Roles = "Admin")]
[HttpGet]
public IActionResult EditUser(string id)
{
    string userSQL = @"SELECT * FROM Users WHERE Username = '{0}'";
    List<User> lstUser = DBUtl.GetList<User>(userSQL, id);

    if (lstUser.Count == 1)
    {
        return View(lstUser[0]);
    }
    else
    {
        TempData["Message"] = "User not found.";
        TempData["MsgType"] = "warning";
        return RedirectToAction("ListUsers");
    }
}

[Authorize(Roles = "Admin")]
[HttpPost]
public IActionResult EditUser(User user)
{
    if (!ModelState.IsValid)
    {
        ViewData["Message"] = "Invalid Input";
        ViewData["MsgType"] = "warning";
        return View("EditUser");
    }
    else
    {
        string update = @"UPDATE Users SET FullName='{1}', Email='{2}', Password=HASHBYTES('SHA1','{3}'), UserRole='{4}', Batch='{5:yyyy-MM-dd HH:mm}', CompanyName='{6}', ContactPerson='{7}', ContactNo='{8}' WHERE Username='{0}'";

        Console.WriteLine(update);

        int res = DBUtl.ExecSQL(update, user.Username, user.FullName, user.Email, user.Password, user.UserRole, user.Batch, user.CompanyName, user.ContactPerson, user.ContactNo);

        Console.WriteLine(res);

        if (res == 1)
        {
            TempData["Message"] = "User Updated";
            TempData["MsgType"] = "success";
        }
        else
        {
            TempData["Message"] = DBUtl.DB_Message;
            TempData["MsgType"] = "danger";
        }
        return RedirectToAction("ListUsers");
    }
}

[Authorize(Roles = "Admin")]
public IActionResult DeleteUser(string id)
{
    string select = @"SELECT * FROM Users WHERE Username='{0}'";

    Console.WriteLine(select);
    Console.WriteLine(id);

    DataTable ds = DBUtl.GetTable(select, id);
    if (ds.Rows.Count != 1)
    {
        TempData["Message"] = "User record no longer exists.";
        TempData["MsgType"] = "warning";
    }
    else
    {
        string delete = "DELETE FROM Users WHERE Username='{0}'";
        int res = DBUtl.ExecSQL(delete, id);
        if (res == 1)
        {
            TempData["Message"] = "User Deleted";
            TempData["MsgType"] = "success";
        }
        else
        {
            TempData["Message"] = DBUtl.DB_Message;
            TempData["MsgType"] = "danger";
        }
    }

    return RedirectToAction("ListUsers");

}


private List<User> GetListUsers()
{
    string userSql = @"SELECT Username, FullName From Users";

    List<User> lstuser = DBUtl.GetList<User>(userSql);
    return lstuser;
}

    }

}
