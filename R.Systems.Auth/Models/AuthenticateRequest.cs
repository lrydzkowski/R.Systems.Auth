namespace R.Systems.Auth.Models
{
    public class AuthenticateRequest
    {
        private string _email = "";
        public string Email 
        { 
            get
            {
                return _email.Trim();
            }
            init
            {
                _email = value;
            }
        }

        private string _password = "";
        public string Password 
        { 
            get
            {
                return _password.Trim();
            }
            init
            {
                _password = value;
            }
        }
    }
}
