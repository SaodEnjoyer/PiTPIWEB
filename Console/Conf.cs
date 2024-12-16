using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Conf : IOptions<Conf>
    {
        public Conf Value => this;

        private const string Address = "https://localhost:7291/api/";
        public static string GetAdress()
        {
            return Address;
        }

    }
}
