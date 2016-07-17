using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Net;
using System.IO;
using System.Xml;
using System.Data;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Timers;
using System.Collections;
using Pokebot;
using GLaDOSbot;

namespace PokebotStats
{
    class Program
    {
        /*
            GLOBAL SET STRING VALUES
        */
        string[] bannedRoles = new string[]
                     {
                         "Admin", "Mod", "DevGod", "Owner", "SubOwner", "BotGuy" , "AdminBot",
                         "@Admin", "@Mod", "@DevGod", "@Owner", "@SubOwner", "@BotGuy", "@AdminBot"
                     };
        const string sql =
            "Server=ultrarust.com;" +
            "Database=singularbeing;" +
            "Uid=singularbeing;" +
            "Pwd=jXKcjyThJtBisZwW;";

        string[] starterPokemon = new string[]
        {
            "Squirtle", "Charmander", "Bulbasaur"
        };

        /*
            GLOBAL SET ULONG VALUES
        */
        ulong tempServer = 203806251988025344;

        System.Timers.Timer timer = new System.Timers.Timer();

        static Program instance;

        static void Main(string[] args)
            => new Program().Start();

        private DiscordClient _client;

        public class UserValues
        {
            public string name,nameId,money,wins,losses,pokemon1,pokemon2,pokemon3,pokemon4,pokemon5,pokemon6,starterpokemon;
            public UserValues(string n, string n1, int n2, int n3, int n4, string n5, string n6, string n7, string n8, string n9, string n10, int n11)
            {
                name = n;
                nameId = n1;
                money = n2.ToString();
                wins = n3.ToString();
                losses = n4.ToString();
                pokemon1 = n5;
                pokemon2 = n6;
                pokemon3 = n7;
                pokemon4 = n8;
                pokemon5 = n9;
                pokemon6 = n10;
                starterpokemon = n11.ToString();
            }
        }

        public void Start()
        {
            instance = this;

            timer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            timer.Interval = 1000;
            timer.Start();

            _client = new DiscordClient();

            _client.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.HelpMode = HelpMode.Public;
            });
            #region Commands
            //pokemon stats
            _client.GetService<CommandService>().CreateGroup("stats", x =>
            {
                _client.GetService<CommandService>().CreateCommand("all")
               .Description("Shows the stats of all of a user's pokemon.")
               .Do(async e =>
               {
                   //check the database for this user
                   UserValues userVa = null;
                   string param = "SELECT * FROM users";
                   MySqlConnection con = null;
                   MySqlCommand cmd = null;
                   MySqlDataReader reader = null;
                   con = new MySqlConnection(sql);
                   cmd = new MySqlCommand(param, con);
                   new GLaDOSbot.Message("Starting connection.");

                   con.Open();

                   reader = cmd.ExecuteReader();

                   while (reader.Read())
                   {
                       string aName = reader.GetString("name");
                       if (aName != null)
                       {
                           //check this for a name
                           if (aName == e.User.Name)
                           {
                               userVa = new UserValues(
                                   reader.GetString("name"),
                                   reader.GetString("nameId"),
                                   reader.GetInt32("money"),
                                   reader.GetInt32("wins"),
                                   reader.GetInt32("losses"),
                                   reader.GetString("pokemon1"),
                                   reader.GetString("pokemon2"),
                                   reader.GetString("pokemon3"),
                                   reader.GetString("pokemon4"),
                                   reader.GetString("pokemon5"),
                                   reader.GetString("pokemon6"),
                                   reader.GetInt32("starterpokemon")
                                   );
                               new GLaDOSbot.Message($"Got the user {userVa.name}.");
                           }
                       }
                   }

                   con.Close();
                   con = null;

                   //we have the trainer, now we need to get the pokemon
                   string param2 = "SELECT * FROM userpokemon";
                   con = new MySqlConnection(sql);
                   cmd = new MySqlCommand(param2, con);

                   con.Open();

                   reader = cmd.ExecuteReader();

                   List<Pokemon> pokemon = new List<Pokemon>();

                   while (reader.Read())
                   {
                        pokemon.Add(new Pokemon(reader.GetString("name"), int.Parse(reader.GetString("userId")), reader.GetString("move1"), reader.GetString("move2"),
                            reader.GetString("move3"), reader.GetString("move4"), int.Parse(reader.GetString("exp")), reader.GetString("statusEffect"),
                            reader.GetString("heldItem"), int.Parse(reader.GetString("hp")), int.Parse(reader.GetString("attack")), int.Parse(reader.GetString("defense")),
                            int.Parse(reader.GetString("spatk")), int.Parse(reader.GetString("spdef")), int.Parse(reader.GetString("speed")), int.Parse(reader.GetString("level")), int.Parse(reader.GetString("nextlevel"))));
                        new GLaDOSbot.Message($"Adding pokemon {reader.GetString("name")}.");
                   }

                   con.Close();
                   con = null;
                   reader = null;

                   string finalString = string.Empty;

                   new GLaDOSbot.Message("Done with pokemon. Length: " + pokemon.Count);

                   bool hasPokemon = false;
                   bool pokemonExists = false;

                   foreach (Pokemon po in pokemon)
                   {
                       pokemonExists = true;
                       new GLaDOSbot.Message(po.name + "....yep");
                       new GLaDOSbot.Message("ID:" + userVa.nameId + ",  " + po.userId);
                       if (po.userId == (int)e.User.Id && po.name == userVa.pokemon1)
                       {
                           hasPokemon = true;
                           //exists
                           await e.Channel.SendMessage(
                               $"__**{po.name}:**__" + '\n' +
                               $"**HP:** {po.hp}, **Attack:** {po.attack}" + '\n' +
                               $"**Defense:** {po.def}, **SpAtk:** {po.spatk}" + '\n' +
                               $"**SpDef:** {po.spdef}, **Speed:** {po.speed}" + '\n' +
                               $"**EXP:** {po.exp}, **Level:** {po.level}" + '\n' +
                               $"**EXP for Next Level:** {po.nextLevel}" + '\n' +
                               $"**Moves: {(string.IsNullOrEmpty(po.move1) ? "Unassigned" : po.move1)}, {(string.IsNullOrEmpty(po.move2) ? "Unassigned" : po.move2)}" + '\n' +
                               $"{(string.IsNullOrEmpty(po.move3) ? "Unassigned" : po.move3)}, {(string.IsNullOrEmpty(po.move4) ? "Unassigned" : po.move4)}");
                       }
                       else
                       {
                           new GLaDOSbot.Message("Pokemon does not exist. Adding.");
                           break;
                       }
                   }

                   if (!pokemonExists)
                   {
                       if (userVa.pokemon1 != null)
                       {
                           List<MySqlParameter> paramList = new List<MySqlParameter>();
                           paramList.Add(new MySqlParameter("name", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = userVa.pokemon1;
                           paramList.Add(new MySqlParameter("userId", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = (int)e.User.Id;
                           paramList.Add(new MySqlParameter("move1", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           paramList.Add(new MySqlParameter("move2", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           paramList.Add(new MySqlParameter("move3", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           paramList.Add(new MySqlParameter("move4", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           paramList.Add(new MySqlParameter("exp", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           paramList.Add(new MySqlParameter("statusEffect", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           paramList.Add(new MySqlParameter("heldItem", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           paramList.Add(new MySqlParameter("hp", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 15;
                           paramList.Add(new MySqlParameter("attack", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           paramList.Add(new MySqlParameter("defense", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           paramList.Add(new MySqlParameter("spatk", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           paramList.Add(new MySqlParameter("spdef", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           paramList.Add(new MySqlParameter("speed", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           paramList.Add(new MySqlParameter("level", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 5;
                           paramList.Add(new MySqlParameter("nextlevel", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 85;

                           new GLaDOSbot.Message($"Adding pokemon {userVa.pokemon1}.");
                           SetValueInTable("userpokemon", "SELECT * FROM userpokemon", "name,userId,move1,move2,move3,move4,exp,statusEffect,heldItem,hp,attack,defense,spatk,spdef,speed,level,nextlevel", "?name,?userId,?move1,?move2,?move3,?move4,?exp,?statusEffect,?heldItem,?hp,?attack,?defense,?spatk,?spdef,?speed,?level,?nextlevel", paramList);
                       }
                   }

                   if (!hasPokemon)
                   {
                       if (userVa.pokemon1 != null)
                       {
                           new GLaDOSbot.Message("Adding pokemon 1.");
                           List<MySqlParameter> paramList = new List<MySqlParameter>();
                           paramList.Add(new MySqlParameter("name", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = userVa.pokemon1;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("userId", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = (int)e.User.Id;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("move1", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("move2", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("move3", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("move4", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("exp", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("statusEffect", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("heldItem", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = "";
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("hp", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 15;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("attack", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("defense", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("spatk", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("spdef", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("speed", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 0;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("level", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 5;
                           new GLaDOSbot.Message("0");
                           paramList.Add(new MySqlParameter("nextlevel", MySqlDbType.VarString));
                           paramList[paramList.Count - 1].Value = 85;
                           new GLaDOSbot.Message("0");

                           new GLaDOSbot.Message($"Adding pokemon {userVa.pokemon1}.");
                           SetValueInTable("userpokemon", "SELECT * FROM userpokemon", "name,userId,move1,move2,move3,move4,exp,statusEffect,heldItem,hp,attack,defense,spatk,spdef,speed,level,nextlevel", "?name,?userId,?move1,?move2,?move3,?move4,?exp,?statusEffect,?heldItem,?hp,?attack,?defense,?spatk,?spdef,?speed,?level,?nextlevel", paramList);
                       }
                   }

                   new GLaDOSbot.Message("After check.");

                   con.Close();

                   if (con != null) con = null;
                   if (reader != null) reader = null;
               });
            });
                   #endregion

                   _client.ExecuteAndWait(async () =>
            {
                new GLaDOSbot.Message("Pokebot-Stats v0.1 is now active and connected.");
                await _client.Connect("MjAzOTY1MDIxOTc2Mzk1Nzc2.Cmwkug.U5-UNa-H7JirE2kFC0rPcMi6C1Y");
            });
        }

        public static void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            //updates every second

        }

        bool CheckUserInDatabase(string name, int id)
        {
            string param = "SELECT * FROM users";
            MySqlConnection con = null;
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            List<PokeUser> users = new List<PokeUser>();
            bool userExists = false;
            bool exists = true;

            try
            {
                con = new MySqlConnection(sql);
                cmd = new MySqlCommand(param, con);
                new GLaDOSbot.Message("Con is null: " + (con == null ? true : false).ToString());

                con.Open();

                reader = cmd.ExecuteReader();
                new GLaDOSbot.Message("Reader is null: " + (reader == null ? true : false).ToString());

                while (reader.Read())
                {
                    new GLaDOSbot.Message("Reader is null: " + (reader == null ? true : false).ToString());
                    //users.Add(new PokeUser(reader.GetString("name"), reader.GetString("money")));
                    string aName = reader.GetString("name");
                    if (aName != null)
                    {
                        //check this for a name
                        if (aName == name)
                        {
                            userExists = true;
                        }
                    }
                }

                con.Close();
                con = null;
                con = new MySqlConnection(sql);
                cmd = new MySqlCommand(param, con);
                con.Open();
                new GLaDOSbot.Message("Opened");

                if (userExists)
                {
                    new GLaDOSbot.Message($"User {name} exists!");
                }
                else
                {
                    exists = false;
                    new GLaDOSbot.Message($"Setting new user {name}.");
                    //add to the database
                    cmd.CommandText = "INSERT INTO users(name,nameId,money,wins,losses,pokemon1,pokemon2,pokemon3,pokemon4,pokemon5,pokemon6,starterpokemon) VALUES(?name,?nameid,?money,?wins,?losses,?pokemon1,?pokemon2,?pokemon3,?pokemon4,?pokemon5,?pokemon6,?starterpokemon)";
                    cmd.Parameters.Add("?name", MySqlDbType.VarString).Value = name;
                    new GLaDOSbot.Message(id.ToString() + "   = ID");
                    cmd.Parameters.Add("?nameId", MySqlDbType.VarString).Value = id.ToString();
                    cmd.Parameters.Add("?money", MySqlDbType.VarString).Value = "500";
                    cmd.Parameters.Add("?wins", MySqlDbType.VarString).Value = "0";
                    cmd.Parameters.Add("?losses", MySqlDbType.VarString).Value = "0";
                    cmd.Parameters.Add("?pokemon1", MySqlDbType.VarString).Value = "";
                    cmd.Parameters.Add("?pokemon2", MySqlDbType.VarString).Value = "";
                    cmd.Parameters.Add("?pokemon3", MySqlDbType.VarString).Value = "";
                    cmd.Parameters.Add("?pokemon4", MySqlDbType.VarString).Value = "";
                    cmd.Parameters.Add("?pokemon5", MySqlDbType.VarString).Value = "";
                    cmd.Parameters.Add("?pokemon6", MySqlDbType.VarString).Value = "";
                    cmd.Parameters.Add("?starterpokemon", MySqlDbType.VarString).Value = 0;
                    cmd.ExecuteNonQuery();
                    new GLaDOSbot.Message("Execute");
                    con.Close();
                    con = null;
                    con = new MySqlConnection(sql);
                    cmd = new MySqlCommand(param, con);
                }

                con.Open();

                reader = cmd.ExecuteReader();
                new GLaDOSbot.Message("Reader is null: " + (reader == null ? true : false).ToString());

                while (reader.Read())
                {
                    new GLaDOSbot.Message("Reader is null: " + (reader == null ? true : false).ToString());
                    //users.Add(new PokeUser(reader.GetString("name"), reader.GetString("money")));
                    string aName = reader.GetString("name");
                    if (aName != null)
                    {
                        //check this for a name
                        if (aName == name)
                        {
                            userExists = true;
                        }
                    }
                }

                if (userExists)
                {
                    new GLaDOSbot.Message($"User {name} exists!");
                }
                else
                {
                    new GLaDOSbot.Message($"User {name} does not exist.......wtf");
                }

            }
            catch (Exception ex)
            {
                new GLaDOSbot.Message(string.Format("An error occurred {0}", ex.Message));
            }
            finally
            {
                if (reader != null) reader.Close();
                if (con != null) con.Close();

                new GLaDOSbot.Message("Done");
            }

            return exists;
        }

        private void SetValueInTable(string tableName, string param, string param1, string param2, List<MySqlParameter> parameters)
        {
            new GLaDOSbot.Message("Query" + '\n' + $"INSERT INTO {tableName}({param1}) VALUES({param2})");
            new GLaDOSbot.Message("started");
            MySqlConnection con = new MySqlConnection(sql);
            MySqlCommand cmd = new MySqlCommand(param, con);
            con.Open();
            new GLaDOSbot.Message("Open");

            cmd.CommandText = $"INSERT INTO {tableName}({param1}) VALUES({param2})";
            foreach (MySqlParameter p in parameters)
            {
                new GLaDOSbot.Message("p=" + p.Value);
                cmd.Parameters.Add(p);
            }
            new GLaDOSbot.Message(cmd.CommandText);
            cmd.ExecuteNonQuery();
            new GLaDOSbot.Message("execute");

            con.Close();
            new GLaDOSbot.Message("close");
            con = null;
        }

        private struct InviteInfo
        {
            public string userId;
            public string senderId;
            public string server;
            public string channel;
            public InviteInfo(string p1, string p2, string p3, string p4)
            {
                userId = p1;
                senderId = p2;
                server = p3;
                channel = p4;
            }
        }
    }
}