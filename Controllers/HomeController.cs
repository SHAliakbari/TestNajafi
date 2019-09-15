using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using com.google.i18n.phonenumbers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Najafi_Test.Models;
using Najafi_Test.ViewModels;
using static com.google.i18n.phonenumbers.Phonenumber;

namespace Najafi_Test.Controllers
{
    public class HomeController : Controller
    {
        DB db;
        #region Constructor

        public HomeController(DB db)
        {
            this.db = db;
        }
        #endregion


        public async Task<IActionResult> List()
        {
            var items = await db.RegisterForms.ToListAsync();
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromQuery]int id )
        {
            var item = await db.RegisterForms.Where(x => x.Id == id).FirstOrDefaultAsync();
            if(item != null)
            {
                db.RegisterForms.Remove(item);
                await db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Not Found");
        }

        public async Task< IActionResult> Index([FromQuery]int id = 0)
        {
            var item = await db.RegisterForms.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (item == null)
                item = new RegisterForm();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody]RegisterForm registerForm)
        {
            ResultViewModel resultViewModel = new ResultViewModel() { IsSuccess = true };

            if (registerForm != null)
            {
                List<string> errors = new List<string>();
                if(!registerForm.Validate(ref errors))
                {
                    resultViewModel.ErrorMessage = string.Join("\r\n", errors);
                    resultViewModel.IsSuccess = false;
                }
                if (resultViewModel.IsSuccess)
                {
                    PhoneNumberUtil phoneUtil = PhoneNumberUtil.getInstance();
                    PhoneNumber NumberProto = phoneUtil.parse(registerForm.PhoneNumber, registerForm.Country.ToUpper());
                    registerForm.PhoneNumber = $"+{NumberProto.getCountryCode()}{NumberProto.getNationalNumber()}";
                    if (registerForm.Id == 0)
                    {
                        registerForm.Guid = Guid.NewGuid();
                        db.RegisterForms.Add(registerForm);
                    }
                    else
                    {
                        db.RegisterForms.Attach(registerForm);
                        db.Entry(registerForm).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    }

                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        if(ex.InnerException != null )
                        {
                            if (ex.InnerException.Message.Contains("'IX_RegisterForm'"))
                            {
                                resultViewModel.IsSuccess = false;
                                resultViewModel.ErrorMessage = "Duplicate Email not permitted";
                            }
                            if (ex.InnerException.Message.Contains("'IX_RegisterForm_1'"))
                            {
                                resultViewModel.IsSuccess = false;
                                resultViewModel.ErrorMessage = "Duplicate FirstName,LastName and DatOfBirth not permitted";
                            }
                        }
                        
                    }
                    
                }
            }
            else
            {
                resultViewModel.IsSuccess = false;
                resultViewModel.ErrorMessage = "Data is not presented";
            }

            if (resultViewModel.IsSuccess)
                return Ok();

            return BadRequest(resultViewModel.ErrorMessage);
        }

        
    }
}
