using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InterComm.Helpers
{
    class EmailAddress
    {
        private string strRegex =   @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        private string _address;
        private string _username;
        private string _domain;
        private bool _valid;

        public EmailAddress(string address)
        {
            if (address.Length == 0)
            {
                _address = "null";
            }
            else
            {
                _address = address;
            }

            string[] splitAddress = _address.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            _username = splitAddress.Length >= 1 ? splitAddress[0] : "null";
            _domain = splitAddress.Length >= 2 ? splitAddress[1] : "null";

            Regex re = new Regex(strRegex);
            _valid = re.IsMatch(_address);
        }

        public bool Valid
        {
            get
            {
                return _valid;
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
        }

        public string Domain
        {
            get
            {
                return _domain;
            }
        }

        public string Address
        {
            get
            {
                return _address;
            }
        }

    }
}
