using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokebot
{
    class PokeUser
    {
        public string name;
        public string money;
        public PokeUser(string name, string money)
        {
            this.name = name;
            this.money = money;
        }
    }
}
