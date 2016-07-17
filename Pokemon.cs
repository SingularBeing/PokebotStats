using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokebotStats
{
    class Pokemon
    {
        public string name;
        public int userId;
        public string move1, move2, move3, move4;
        public int exp;
        public string statusEffect;
        public string heldItem;
        public int hp, attack, def, spatk, spdef, speed, level, nextLevel;

        public Pokemon(string name, int userId, string move1, string move2, string move3, string move4, int exp, string statusEffect,
            string heldItem, int hp, int attack, int def, int spatk, int spdef, int speed, int level, int nextLevel)
        {
            this.name = name;
            this.userId = userId;
            this.move1 = move1;
            this.move2 = move2;
            this.move3 = move3;
            this.move4 = move4;
            this.exp = exp;
            this.statusEffect = statusEffect;
            this.heldItem = heldItem;
            this.hp = hp;
            this.attack = attack;
            this.def = def;
            this.spatk = spatk;
            this.spdef = spdef;
            this.speed = speed;
            this.level = level;
            this.nextLevel = nextLevel;
        }
    }
}
