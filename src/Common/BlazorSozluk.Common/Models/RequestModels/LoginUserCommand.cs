using BlazorSozluk.Common.Models.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Models.RequestModels
{
    //kullanıcı tarafından dışarıdan alınan veriler neticesinde döneceğim ViewModel  = LoginUserViewModel 

    //Buradaki login sınıfında ise dışarıdan ne alacağımızı tanımlamıs olduk 
    public class LoginUserCommand :IRequest<LoginUserViewModel>
    {

        public  string EmailAddress { get;  set; }
        public  string Password { get;  set; }

        public LoginUserCommand(string emailAddress, string password)
        {
           EmailAddress = emailAddress;
           Password = password;
        }

        public LoginUserCommand()
        {
            
        }
    }
}
