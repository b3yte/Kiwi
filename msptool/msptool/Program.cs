﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using StarService.Enum;
using StarService.Utility;
using static StarService.Utility.Amf;
using static StarService.Utility.ChecksumCalculator;
using static StarService.Utility.ConfigManager;
using static StarService.Utility.ProtobufData;
using static StarService.Utility.Ws;
using static msptool.localisation;
using Rule = Spectre.Console.Rule;
using WebClient = System.Net.WebClient;

namespace msptool
{
    internal class Program
    {
        private static readonly string vloc1 = "2024.4.1.1";

        private static readonly string vloc3 =
            "https://raw.githubusercontent.com/r-h-y/star/main/msptool/version.txt";

        static async Task Main(string[] args)
        {
            InitConfig();
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var spt1 = @"
                                                                                   
                                                   .=+**+-.                        
                                                  =*######*-                       
                                                 :###*--*###.                      
                                           :--===*##*-  =###*===--.                
                                         :*#########+   .+#########+:              
                                        :*###======-.    .-=====+###*.             
                                      .:=###*-.                :=###*.             
                                 :-+**########*+:.          .-+####*:              
                             .-+*################*         .*####+:                
                           :+####################=          =###+                  
                         -*#####################+.  .-**-.  .*###-                 
                       :*#######################=:-+#####*+-.+###=                 
                     .=###################*+=########*++*#######*.                 
                    -*###################+:  .=+*#*+-.  .-+*#*+-.                  
                  .=####################-                                          
                 .+###################*:                                           
                .+###################*:                                            
               .*###################*:                                             
              .+###################*-                                              
              =####################=                                               
             .*#*+-=##############*.                                               
              ::. .+##############=                                                
                  :*######*=#####*.                                                
                  :######*: =####=                                                 
                  -#####+.  .+###-                                                 
                  -####=     .+**.                                                 
                  :*#*:        ::                                                  
                  .=+:                                                             
                                                                                   
                                                                                   
";

            AnsiConsole.Clear();
            AnsiConsole.WriteLine(spt1);
            AnsiConsole.MarkupLine("[#71d5fb]Star Project by ham & 6c0[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Star)
                .SpinnerStyle(Style.Parse("#71d5fb"))
                .Start("Loading...", ctx =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(50);
                        ctx.Refresh();
                    }
                });

            Console.Clear();

            if (!vloc2())
            {
                HttpClient loc1 = new HttpClient();
                string vloc4 = loc1.GetStringAsync(vloc3).Result;
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Update").LeftJustified());
                Console.Write("\n");
                while (true)
                {
                    Console.WriteLine("[\x1b[95m!\u001b[39m] \u001b[93mAn update was found !\n");
                    Console.WriteLine("\u001b[94m1\u001b[39m > Install new update");
                    Console.WriteLine("\u001b[94m2\u001b[39m > Update manually\n");
                    Console.Write("[\u001b[95mUPDATE\u001b[39m] Pick an option: ");
                    string options = Console.ReadLine();
                    switch (options)
                    {
                        case "1":
                            await InstallUpdate(vloc4);
                            return;
                        case "2":
                            Console.WriteLine(
                                "\n\x1b[95mUPDATE\u001b[39m > \x1b[93mGo on https://github.com/r-h-y/star [Click any key to close]");
                            Console.ReadKey();
                            return;
                        default:
                            Console.WriteLine("\n\u001b[91mERROR\u001b[39m > \u001b[93mChoose a option which exists !");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            break;
                    }
                }
            }

            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Choose").LeftJustified());
            Console.Write("\n");

            var loc2 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[[[#71d5fb]+[/]]] Select which MSP you want to use")
                    .PageSize(3)
                    .AddChoices(new[] { "MovieStarPlanet", "MovieStarPlanet 2" })
            );

            if (loc2 == "MovieStarPlanet")
                MSP1_Login();
            else
                MSP2_Login();

        }

        static void MSP1_Login()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Choose Language").LeftJustified());
            Console.Write("\n");

            var loc1 = AnsiConsole.Prompt(
                new Spectre.Console.SelectionPrompt<string>()
                    .Title("[[[#71d5fb]+[/]]] Select your language")
                    .PageSize(11)
                    .AddChoices(new[]
                    {
                        "English", "French", "Turkish", "German", "Polish", "Swedish", "Dutch", "Finnish", "Norwegian",
                        "Danish", "Spanish"
                    })
            );
            Console.Clear();
            bool loc2 = false;
            var l2msp = login2();
            //AnsiConsole.WriteLine($"Accounts found: {l2msp.Count}");

            if (l2msp.Count > 0)
            {
                var l2loc2 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[[[#71d5fb]+[/]]] Select an account to login with:")
                        .PageSize(10)
                        .AddChoices(l2msp.Keys)
                );

                var l2loc1 = l2msp[l2loc2];
            }
            else
            {
                while (!loc2)
                {

                    AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Login MSP").LeftJustified());
                    Console.Write("\n");
                    var loc3 = AnsiConsole.Prompt(
                        new Spectre.Console.TextPrompt<string>("[[[#71d5fb]+[/]]] Enter username: ")
                            .PromptStyle("#71d5fb"));

                    var loc4 = AnsiConsole.Prompt(
                        new Spectre.Console.TextPrompt<string>("[[[#71d5fb]+[/]]] Enter password: ")
                            .PromptStyle("#71d5fb")
                            .Secret());

                    var loc5 = Enum.GetValues(typeof(WebServer))
                        .Cast<WebServer>()
                        .Select(ws => (ws.loc3().Item1, ws.loc3().Item2))
                        .ToArray();

                    var loc6 = AnsiConsole.Prompt(
                        new Spectre.Console.SelectionPrompt<string>()
                            .Title("[[[#71d5fb]+[/]]] Select a server: ")
                            .PageSize(15)
                            .MoreChoicesText("[grey](Move up and down to reveal more servers)[/]")
                            .AddChoices(loc5.Select(loc7 => loc7.Item1))
                    );

                    var loc8 = loc5.First(loc7 => loc7.Item1 == loc6);
                    dynamic loc9 = null;
                    string server = loc8.Item2;
                    if (Msptoolhome.TryGetValue(loc1, out var loc11))
                        AnsiConsole.Status()
                            .SpinnerStyle(Spectre.Console.Style.Parse("#71d5fb"))
                            .Start("Login...", ctx =>
                            {
                                ctx.Refresh();
                                ctx.Spinner(Spinner.Known.Circle);
                                loc9 = AmfCall(server, "MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login",
                                    new object[6]
                                    {
                                        loc3, loc4, new object[] { }, null, null, "MSP1-Standalone:XXXXXX"
                                    });
                                Thread.Sleep(1000);
                            });

                    if (loc9 == null)
                    {
                        Console.WriteLine(
                            "\n\x1b[91mFAILED\u001b[39m > \x1b[93mUnknown [Click any key to return to login]");
                        Console.ReadKey();
                        Console.Clear();
                    }

                    if (loc9["loginStatus"]["status"] != "Success")
                    {
                        Console.WriteLine(
                            "\n\x1b[91mFAILED\u001b[39m > \x1b[93mLogin failed [Click any key to return to login]");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        loc2 = true;
                        int actorId = loc9["loginStatus"]["actor"]["ActorId"];
                        string name = loc9["loginStatus"]["actor"]["Name"];
                        string ticket = loc9["loginStatus"]["ticket"];
                        string accessToken = loc9["loginStatus"]["nebulaLoginStatus"]["accessToken"];
                        string profileId = loc9["loginStatus"]["nebulaLoginStatus"]["profileId"];
                        var th = new JwtSecurityTokenHandler();
                        var jtoken = th.ReadJwtToken(accessToken);
                        var loginId = jtoken.Payload["loginId"].ToString();
                        string loc15 = loc9["loginStatus"]["actorLocale"][0];
                        string culture = loc15.Replace('_', '-');

                        Console.Write(culture);

                        Console.Clear();

                        while (true)
                        {
                            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home").LeftJustified().RoundedBorder());
                            Console.Write("\n");
                            foreach (var loc12 in loc11)
                            {
                                AnsiConsole.Markup($"[#71d5fb]{loc12.Key}[/]  > {loc12.Value}\n");
                            }

                            AnsiConsole.Write(
                                new Rule(
                                        "[slowblink][#71d5fb]ham & 6c0[/][/]")
                                    .RightJustified().RoundedBorder());
                            var loc13 = AnsiConsole.Prompt(
                                new TextPrompt<string>("\n[[[#71d5fb]+[/]]] Pick an option: ")
                                    .PromptStyle("#71d5fb"));

                            switch (loc13)
                            {
                                case "1":
                                    recycleNoneRareClothes(server, actorId, ticket);
                                    break;
                                case "2":
                                    buyBoonie(server, actorId, ticket);
                                    break;
                                case "3":
                                    buyAnimation(server, actorId, ticket);
                                    break;
                                case "4":
                                    buyClothes(server, actorId, ticket);
                                    break;
                                case "5":
                                    buyEyes(server, actorId, ticket);
                                    break;
                                case "6":
                                    buyNose(server, actorId, ticket);
                                    break;
                                case "7":
                                    buyLips(server, actorId, ticket);
                                    break;
                                case "8":
                                    wearRareSkin(server, actorId, ticket);
                                    break;
                                case "9":
                                    addToWishlist(server, ticket);
                                    break;
                                case "10":
                                    customStatus(server, name, actorId, ticket);
                                    break;
                                case "11":
                                    addSponsors(server, ticket);
                                    break;
                                case "12":
                                    blockDefaults(server, actorId, ticket);
                                    break;
                                case "13":
                                    recycleitems(server, actorId, ticket);
                                    break;
                                case "14":
                                    wheelspins(server, actorId, ticket);
                                    break;
                                case "15":
                                    lisaHack(server, actorId, ticket);
                                    break;
                                case "16":
                                    automatedPixeller(server, ticket);
                                    break;
                                case "17":
                                    mspquery(server, actorId, ticket);
                                    break;
                                case "18":
                                    usernameChecker();
                                    break;
                                case "19":
                                    clothesExtractor(server, ticket);
                                    break;
                                case "20":
                                    usernameToActorid(server);
                                    break;
                                case "21":
                                    actorIdToUsername(server);
                                    break;
                                case "22":
                                    itemTracker(server);
                                    break;
                                case "23":
                                    roomChanger(server, actorId, ticket);
                                    break;
                                case "24":
                                    animationsExtractor(server, ticket);
                                    break;
                                case "25":
                                    iconChanger(server, actorId, ticket);
                                    break;
                                case "26":
                                    botGenerator(server, culture);
                                    break;
                                case "27":
                                    itemGlitcher(server, ticket, actorId, accessToken, profileId);
                                    break;
                                case "28":
                                    automatedAutographer(server, ticket, actorId, accessToken, profileId);
                                    break;
                                case "29":
                                    sfAutomatedFarmer();
                                    break;
                                case "30":
                                    passwordChanger(server, ticket, actorId, loc4);
                                    break;
                                case "31":
                                    friendRequester(server, ticket, actorId);
                                    break;
                                case "32":
                                    Console.WriteLine("\n\x1b[97mBYE\u001b[39m > \u001b[93mLogging out...");
                                    Console.Clear();
                                    loc2 = false;
                                    break;
                                default:
                                    Console.WriteLine(
                                        "\n\u001b[91mERROR\u001b[39m > \u001b[93mChoose a option which exists !");
                                    Thread.Sleep(2000);
                                    Console.Clear();
                                    break;
                            }

                            if (!loc2)
                                break;
                        }
                    }
                }
            }

            static void recycleNoneRareClothes(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Recycle None-Rare Clothes")
                    .LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                dynamic loc1 = AmfCall(server,
                    "MovieStarPlanet.WebService.ActorClothes.AMFActorClothes.GetActorClothesRelMinimals",
                    new object[2]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId
                    });

                foreach (dynamic loc2 in loc1)
                {
                    object loc3 = loc2["ActorClothesRelId"];

                    dynamic loc4 = AmfCall(server,
                        "MovieStarPlanet.WebService.MovieStar.AMFMovieStarService.GetActorClothesRel",
                        new object[1] { loc3 });

                    string loc5 = loc4["Cloth"]["ShopId"].ToString();
                    string loc6 = loc4["Cloth"]["Name"] ?? "Unknown";

                    if (loc5 != "-100")
                    {
                        dynamic recycler = AmfCall(server,
                            "MovieStarPlanet.WebService.Profile.AMFProfileService.RecycleItem",
                            new object[4]
                            {
                                new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                                actorId,
                                loc3,
                                0
                            });
                        AnsiConsole.Markup($"[[[#71d5fb]![/]]] Recycled {loc6}");
                    }
                }

                AnsiConsole.Markup(
                    "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Finished recycling[/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();
            }

            static void buyBoonie(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Buy Boonie").LeftJustified()
                    .RoundedBorder());

                var loc1 = new (string Name, int Value)[]
                {
                    ("Light Side Boonie", 1),
                    ("Dark Side Boonie", 2),
                    ("VIP Boonie", 3),
                    ("FOX", 4),
                    ("DOG", 5),
                    ("PLANT", 6),
                    ("DRAGON", 7),
                    ("Metat Eater", 8),
                    ("Xmas Boonie", 9),
                    ("Valentine Boonie", 10),
                    ("Diamond Boonie", 11),
                    ("Easter Bunny", 12),
                    ("Diamond Squirrel", 13),
                    ("Poodle", 14),
                    ("Summer Boonie", 15),
                    ("Gamer Bunny", 16),
                    ("Brad Pet", 17),
                    ("Magazine Pet", 18),
                    ("Puppy", 19),
                    ("Halloween Boonie", 20),
                    ("Space Boonie", 21),
                    ("Xmax Boonie 2012", 22),
                    ("New Years Boonie 2012", 23),
                    ("Elements 2013 Boonie", 24),
                    ("Valentines 2013 Boonie", 25),
                    ("Australia 2013 Boonie", 26),
                    ("EgmontMagazine1Boonie", 27),
                    ("Easter 2013 Boonie", 29),
                    ("Tutti Frutti 2013 Boonie", 30),
                    ("Birthday 2013 Boonie", 31),
                    ("Mexican 2013 Boonie", 32),
                    ("Fastfood 2013 Boonie", 33),
                    ("Rio 2013 Boonie", 34),
                    ("Night Sky 2013 Boonie", 35),
                    ("Wonderland Boonie", 37),
                    ("Robots 2013", 38),
                    ("Halloween 2013", 39),
                    ("Winter Wonderland 2013", 40),
                    ("Christmas 2013", 41),
                    ("New Year 2013", 42),
                    ("Egmont Mag 10", 43),
                    ("Egmont Mag 2014", 46)
                };

                var loc2 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[[[#71d5fb]+[/]]] Select a boonie: ")
                        .PageSize(10)
                        .AddChoices(loc1.Select(loc3 => loc3.Name))
                );

                var loc4 = loc1.First(loc3 => loc3.Name == loc2);


                dynamic loc5 = AmfCall(server,
                    "MovieStarPlanet.WebService.Pets.AMFPetService.BuyClickItem",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        loc4.Value
                    });
                if (loc5["SkinSWF"] != "femaleskin" && loc5["SkinSWF"] != "maleskin")
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Boonie bought![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void buyAnimation(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Buy Animations").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(
                    new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter AnimationId: ")
                        .PromptStyle("#71d5fb"));

                dynamic loc2 = AmfCall(server,
                    "MovieStarPlanet.WebService.Spending.AMFSpendingService.BuyAnimation",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        loc1
                    });

                if (loc2["Code"] != 0)
                {
                    AnsiConsole.Markup("\n[#fa1414]FAILED[/] > [#f7b136][underline]"
                                       + (loc2["Description"] ?? "Unknown") +
                                       "[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Animation bought![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void buyClothes(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Buy Clothes").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter ClothesId: ")
                    .PromptStyle("#71d5fb"));
                string loc2 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter Color: ")
                        .PromptStyle("#71d5fb"));
                try
                {
                    dynamic loc3 = AmfCall(server, "MovieStarPlanet.WebService.AMFSpendingService.BuyClothes",
                        new object[4]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            actorId,
                            new object[]
                            {
                                new
                                {
                                    Color = loc2,
                                    y = 0,
                                    ActorClothesRelId = 0,
                                    ActorId = actorId,
                                    ClothesId = loc1,
                                    IsWearing = 1,
                                    x = 0
                                },
                            },
                            0
                        });

                    if (loc3["Code"] != 0)
                    {
                        AnsiConsole.Markup("\n[#fa1414]FAILED[/] > [#f7b136][underline]"
                                           + (loc3["Description"] ?? "Unknown") +
                                           "[/] [[Click any key to return to Home]][/]");
                        Console.ReadKey();
                        Console.Clear();
                    }
                    else
                    {
                        AnsiConsole.Markup(
                            "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Clothing bought![/] [[Click any key to return to Home]][/]");
                    }
                }
                catch (Exception)
                {
                    AnsiConsole.Markup("\n[#fa1414]FAILED[/] > Hidden or Deleted [#f7b136][underline]"
                                       +
                                       "[/] [[Click any key to return to Home]][/]");
                }

                Console.ReadKey();
                Console.Clear();
            }

            static void buyNose(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Buy Nose").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter NoseId: ")
                    .PromptStyle("#71d5fb"));

                dynamic loc2 = AmfCall(server,
                    "MovieStarPlanet.WebService.BeautyClinic.AMFBeautyClinicService.BuyManyBeautyClinicItems",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        new object[]
                        {
                            new
                            {
                                IsOwned = false,
                                Type = 4,
                                IsWearing = true,
                                InventoryId = 0,
                                ItemId = loc1,
                                Colors = "",

                            }
                        }
                    });
                if (loc2[0]["InventoryId"] == 0)
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Nose bought![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void buyLips(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Buy Lips").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter LipsId: ")
                    .PromptStyle("#71d5fb"));
                string loc2 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter Color: ")
                        .PromptStyle("#71d5fb"));

                dynamic loc3 = AmfCall(server,
                    "MovieStarPlanet.WebService.BeautyClinic.AMFBeautyClinicService.BuyManyBeautyClinicItems",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        new object[]
                        {
                            new
                            {
                                IsOwned = false,
                                Type = 3,
                                IsWearing = true,
                                InventoryId = 0,
                                ItemId = loc1,
                                Colors = loc2,

                            }
                        }
                    });
                if (loc3[0]["InventoryId"] == 0)
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Lips bought![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void buyEyes(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Buy Eyes").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter EyeId: ")
                    .PromptStyle("#71d5fb"));
                string loc2 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter Color: ")
                        .PromptStyle("#71d5fb"));

                dynamic loc3 = AmfCall(server,
                    "MovieStarPlanet.WebService.BeautyClinic.AMFBeautyClinicService.BuyManyBeautyClinicItems",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        new object[]
                        {
                            new
                            {
                                InventoryId = 0,
                                IsOwned = false,
                                ItemId = loc1,
                                Colors = loc2,
                                Type = 1,
                                IsWearing = true

                            }
                        }
                    });
                if (loc3[0]["InventoryId"] == 0)
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Eye bought![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void wearRareSkin(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ RareSkin").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                string loc1 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter Color: ")
                        .PromptStyle("#71d5fb"));

                dynamic loc2 = AmfCall(server,
                    "MovieStarPlanet.WebService.BeautyClinic.AMFBeautyClinicService.BuyManyBeautyClinicItems",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        new object[]
                        {
                            new
                            {
                                InventoryId = 0,
                                Type = 5,
                                ItemId = -1,
                                Colors = loc1,
                                IsWearing = true

                            }
                        }
                    });
                if (loc2[0]["InventoryId"] == 0)
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Skin bought![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void customStatus(string server, string name, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Status").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                string loc1 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter Status: ")
                        .PromptStyle("#71d5fb"));
                var loc2 = new (string Name, int Value)[]
                {
                    ("Black", 0),
                    ("Red", 13369344),
                    ("Purple", 6684774),
                    ("Light Purple", 6710988),
                    ("Pink", 13369446),
                    ("Green", 3368448),
                    ("Orange", 16737792),
                    ("Blue", 39372),
                    ("Gray", 11187123)
                };

                var loc3 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[[[#71d5fb]+[/]]] Select a color: ")
                        .PageSize(10)
                        .AddChoices(loc2.Select(loc4 => loc4.Name))
                );

                var loc5 = loc2.First(loc4 => loc4.Name == loc3);

                dynamic loc6 = AmfCall(server,
                    "MovieStarPlanet.WebService.ActorService.AMFActorServiceForWeb.SetMoodWithModerationCall",
                    new object[5]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        new
                        {
                            Likes = 0,
                            TextLine = loc1,
                            TextLineLastFiltered = (object)null,
                            ActorId = actorId,
                            WallPostId = 0,
                            TextLineBlacklisted = "",
                            WallPostLinks = (object)null,
                            FigureAnimation = "Girl Pose",
                            FaceAnimation = "neutral",
                            MouthAnimation = "none",
                            SpeechLine = false,
                            IsBrag = false,
                            TextLineWhitelisted = ""
                        },
                        name,
                        loc5.Value,
                        false
                    });
                if (loc6["FilterTextResult"]["IsMessageOk"])
                {
                    if (loc6["FilterTextResult"]["UnrestrictedPolicy"]["HasFilteredParts"])
                    {
                        AnsiConsole.Markup(
                            "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Mood Set But Censored![/] [[Click any key to return to Home]][/]");
                        Console.ReadKey();
                        Console.Clear();
                        return;
                    }

                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Mood Set![/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void addSponsors(string server, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Add Sponsors").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");

                List<int> loc1 = new List<int>
                    { 273, 276, 277, 341, 418, 419, 420, 421, 83417, 83423, 83427, 83424 };

                foreach (int loc2 in loc1)
                {
                    dynamic loc3 = AmfCall(server,
                        "MovieStarPlanet.WebService.AnchorCharacter.AMFAnchorCharacterService.RequestFriendship",
                        new object[2]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            loc2
                        });
                    if (loc3["Code"] != 0)
                    {
                        AnsiConsole.Markup(
                            $"\n[#fa1414]FAILED[/] > [#f7b136][underline]{loc3["Description"] ?? "Unknown"}[/][/]"
                        );
                    }
                    else
                    {
                        AnsiConsole.Markup(
                            $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Anchor {loc2} has been added!![/][/]"
                        );
                    }
                }

                AnsiConsole.Markup(
                    "\n[#71d5fb][/] > [#f7b136][underline]Click any key to return to Home[/][/]"
                );
                Console.ReadKey();
                Console.Clear();
            }

            static void blockDefaults(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Block Defaults").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");

                List<int> loc1 = new List<int>
                    { 3, 4, 414 };

                foreach (int loc2 in loc1)
                {
                    dynamic mspdefaults = AmfCall(server,
                        "MovieStarPlanet.WebService.ActorService.AMFActorServiceForWeb.BlockActor",
                        new object[3]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            actorId,
                            loc2
                        });
                    Console.WriteLine($"Blocked: {loc2}");

                }
            }

            static void recycleitems(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Status").LeftJustified()
                    .RoundedBorder());

                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(
                    new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter Item relid: ")
                        .PromptStyle("#71d5fb"));

                dynamic loc2 = AmfCall(server,
                    "MovieStarPlanet.WebService.Profile.AMFProfileService.RecycleItem",
                    new object[4]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        loc1,
                        0
                    });
            }

            static void wheelspins(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Wheelspins").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");

                loc222(server, ticket, "wheel", 120, actorId, 4);
                loc222(server, ticket, "starwheel", 120, actorId, 4);
                loc222(server, ticket, "starVipWheel", 200, actorId, 4);
                loc222(server, ticket, "advertWheelDwl", 240, actorId, 2);
                loc222(server, ticket, "advertWheelVipDwl", 400, actorId, 2);
            }

            static void loc222(string server, string ticket, string awardType, int awardVal,
                int actorId,
                int count)
            {
                for (int i = 0; i < count; i++)
                {
                    dynamic loc2 = AmfCall(server,
                        "MovieStarPlanet.WebService.Awarding.AMFAwardingService.claimDailyAward",
                        new object[4]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            awardType,
                            awardVal,
                            actorId
                        });
                    Console.WriteLine("Spinning Wheels...");
                    Console.Clear();
                }
            }


            static void addToWishlist(string server, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ WishList").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] Enter ClothesId: ")
                    .PromptStyle("#71d5fb"));
                string loc2 = AnsiConsole.Prompt(
                    new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter Color: ")
                        .PromptStyle("#71d5fb"));

                dynamic loc3 = AmfCall(server,
                    "MovieStarPlanet.WebService.Gifts.AMFGiftsService+Version2.AddItemToWishlist",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        new object[]
                        {
                            loc1
                        },
                        new object[]
                        {
                            loc2
                        }
                    });
                if (loc3 != 0)
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]An cloth have been added in your wishlist[/] [[Click any key to return to Home]][/]");
                }
            }


            static void lisaHack(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Lisa Hack").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                AnsiConsole.Markup(
                    "[slowblink][[[#c70000]?![/]]] Use it at your own risk, we are not responsible for your misdeeds.[/]\n");

                wheelspins(server, actorId, ticket);
                dynamic loc5 = AmfCall(server,
                    "MovieStarPlanet.WebService.Awarding.AMFAwardingService.RequestIntroductionAward",
                    new object[2]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId
                    });
                dynamic loc6 = AmfCall(server,
                    "MovieStarPlanet.WebService.AMFAwardService.RequestIntroductionAward",
                    new object[2]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId
                    });
                dynamic loc7 = AmfCall(server,
                    "MovieStarPlanet.WebService.AMFAwardService.claimDailyAward",
                    new object[4]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        "firstRetentionSC",
                        300,
                        actorId
                    });

                bool loc1 = false;

                for (int i = 0; i < 100; i++)
                {
                    dynamic loc2 = AmfCall(server,
                        "MovieStarPlanet.WebService.AMFAwardService.claimDailyAward",
                        new object[4]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            "twoPlayerFame",
                            50,
                            actorId
                        });
                    Console.WriteLine("Generated 50 fame");

                    dynamic loc3 = AmfCall(server,
                        "MovieStarPlanet.WebService.AMFAwardService.claimDailyAward",
                        new object[4]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            "twoPlayerMoney",
                            50,
                            actorId
                        });
                    Console.WriteLine("Generated 50 starcoins");

                    if (i == 99)
                    {
                        loc1 = true;
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    dynamic loc8 = AmfCall(server,
                        "MovieStarPlanet.WebService.Achievement.AMFAchievementWebService.ClaimReward",
                        new object[3]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            "LUCKY_YOU",
                            actorId
                        });

                    if (loc1)
                    {
                        AnsiConsole.Markup(
                            "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Stars are out your account has been levelled and has starcoins : )[/] [[Auto redirect in 2 seconds]][/]");
                        Thread.Sleep(500);
                        Console.Clear();
                    }
                }
            }

            static void mspquery(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Msp Query").LeftJustified()
                    .RoundedBorder());
                var loc1 = AnsiConsole.Prompt(
                    new Spectre.Console.TextPrompt<string>("[[[#71d5fb]+[/]]] Enter username: ")
                        .PromptStyle("#71d5fb"));


                dynamic loc2 = AmfCall(server,
                    "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                    new object[1] { loc1 });

                if (loc2 == -1)
                {
                    Console.WriteLine(
                        "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    double loc3 = loc2;


                    dynamic loc4 = AmfCall(server,
                        "MovieStarPlanet.WebService.Profile.AMFProfileService.LoadProfileSummary",
                        new object[3]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            loc3,
                            actorId
                        });

                    DateTime loc5 = loc4["Created"];

                    dynamic loc6 = AmfCall(server,
                        "MovieStarPlanet.WebService.AMFActorService.BulkLoadActors",
                        new object[2]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            new object[]
                            {
                                loc3
                            }
                        });

                    string loc7 = loc6[0]["NebulaProfileId"];
                    double loc8 = loc6[0]["ActorId"];
                    string loc9 = loc6[0]["Name"];
                    int loc10 = loc6[0]["Level"];
                    double loc11 = loc6[0]["Fame"];
                    int loc12 = loc6[0]["Money"];
                    int loc13 = loc6[0]["Diamonds"];
                    string loc14 = loc6[0]["SkinColor"];
                    int loc15 = loc6[0]["EyeId"];
                    string loc16 = loc6[0]["EyeColors"];
                    int loc17 = loc6[0]["NoseId"];
                    int loc18 = loc6[0]["MouthId"];
                    string loc19 = loc6[0]["MouthColors"];
                    DateTime loc20 = loc6[0]["MembershipTimeoutDate"];
                    DateTime loc21 = loc6[0]["LastLogin"];

                    AnsiConsole.MarkupLine("[bold white]Profile Information[/]");
                    AnsiConsole.MarkupLine($"[bold blue]NebulaProfileId:[/] {loc7}");
                    AnsiConsole.MarkupLine($"[bold blue]ActorId:[/] {loc8}");
                    AnsiConsole.MarkupLine($"[bold blue]Name:[/] {loc9}");
                    AnsiConsole.MarkupLine($"[bold blue]Level:[/] {loc10}");
                    AnsiConsole.MarkupLine($"[bold blue]Fame:[/] {loc11}");
                    AnsiConsole.MarkupLine($"[bold blue]Money:[/] {loc12}");
                    AnsiConsole.MarkupLine($"[bold blue]Diamonds:[/] {loc13}");
                    AnsiConsole.MarkupLine($"[bold blue]SkinColor:[/] {loc14}");
                    AnsiConsole.MarkupLine($"[bold blue]EyeId:[/] {loc15}");
                    AnsiConsole.MarkupLine($"[bold blue]EyeColors:[/] {loc16}");
                    AnsiConsole.MarkupLine($"[bold blue]NoseId:[/] {loc17}");
                    AnsiConsole.MarkupLine($"[bold blue]MouthId:[/] {loc18}");
                    AnsiConsole.MarkupLine($"[bold blue]MouthColors:[/] {loc19}");
                    AnsiConsole.MarkupLine($"[bold blue]Created:[/] {loc5}");
                    AnsiConsole.MarkupLine($"[bold blue]MembershipTimeoutDate:[/] {loc20}");
                    AnsiConsole.MarkupLine($"[bold blue]LastLogin:[/] {loc21}");

                    AnsiConsole.MarkupLine(
                        $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Queried {loc9} :)[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void automatedPixeller(string server, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Automated Pixeller").LeftJustified()
                    .RoundedBorder());
                AnsiConsole.MarkupLine($"[#71d5fb]Login with second account : )[/]");
                var username = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                    .PromptStyle("#71d5fb"));
                AnsiConsole.MarkupLine($"[#71d5fb]Coming next update : )[/]");

                Console.ReadKey();
                Console.Clear();
            }

            static void usernameChecker()
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Username Checker").LeftJustified()
                    .RoundedBorder());
                var loc5 = AnsiConsole.Prompt(new Spectre.Console.TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                    .PromptStyle("#71d5fb"));
                var loc1 = new string[][]
                {
                    new string[] { "US", "CA", "AU", "NZ" },
                    new string[] { "GB", "DE", "FR", "TR", "SE", "DK", "FI", "PL", "IE", "ES", "NL", "NO" }
                };

                foreach (var loc2 in loc1)
                {
                    foreach (var server in loc2)
                    {
                        var loc4 = AmfCall(server,
                            "MovieStarPlanet.WebService.AMFActorService.IsActorNameUsed",
                            new object[] { loc5 });

                        bool loc3 = Convert.ToBoolean(loc4);

                        if (loc3)
                        {
                            AnsiConsole.MarkupLine(
                                $"[#FF0000]{server} | {loc5} | Not available[/]");
                        }
                        else

                        {
                            AnsiConsole.MarkupLine(
                                $"[#00FF00]{server} | {loc5} | available[/]");
                        }
                    }
                }

                AnsiConsole.MarkupLine(
                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]checked all servers for username :)[/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();
            }

            static void clothesExtractor(string server, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Clothes Extractor").LeftJustified()
                    .RoundedBorder());
                var loc5 = AnsiConsole.Prompt(new Spectre.Console.TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                    .PromptStyle("#71d5fb"));

                dynamic loc1 = AmfCall(server,
                    "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                    new object[1] { loc5 });

                if (loc1 == -1)
                {
                    Console.WriteLine(
                        "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    double loc6 = loc1;

                    dynamic loc2 = AmfCall(server,
                        "MovieStarPlanet.WebService.ActorClothes.AMFActorClothes.GetActorClothesRelMinimals",
                        new object[2]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            loc6,
                        });

                    foreach (var loc3 in loc2)
                    {
                        int loc7 = Convert.ToInt32(loc3["ActorClothesRelId"]);

                        dynamic loc4 = AmfCall(server,
                            "MovieStarPlanet.WebService.MovieStar.AMFMovieStarService.GetActorClothesRel",
                            new object[1]
                                { loc7 });

                        string loc8 = loc4["Cloth"]["Name"] ?? "Unknown";
                        int loc9 = loc4["ClothesId"];
                        string loc10 = loc4["Color"].ToString();
                        string loc11 = loc4["Cloth"]["ShopId"].ToString();
                        int loc12 = loc4["Cloth"]["Vip"];
                        int loc13 = loc4["Cloth"]["DiamondsPrice"];

                        string loc15 = loc13 != 0 ? "Yes" : "No";
                        string loc14 = loc12 != 0 ? "Yes" : "No";
                        string loc16 = loc11 != "-100" ? "Yes" : "No";

                        AnsiConsole.MarkupLine($"[#71d5fb]ActorClothesRelId:[/] {loc7}");
                        AnsiConsole.MarkupLine($"[#71d5fb]ClothesName:[/] {loc8}");
                        AnsiConsole.MarkupLine($"[#71d5fb]ClothesId:[/] {loc9}");
                        if (!string.IsNullOrEmpty(loc10))
                        {
                            AnsiConsole.MarkupLine($"[#71d5fb]Colors:[/] {loc10}");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("[#71d5fb]Colors:[/] None");
                        }

                        AnsiConsole.MarkupLine($"[#71d5fb]IsRareItem:[/] {loc16}");
                        AnsiConsole.MarkupLine($"[#71d5fb]IsVipItem:[/] {loc14}");
                        AnsiConsole.MarkupLine($"[#71d5fb]IsDiamondItem:[/] {loc15}");
                        AnsiConsole.MarkupLine("");
                        string loc17 = $"{server}-{loc5}-clothes.txt";
                        string loc18 = $"ActorClothesRelId: {loc7}{Environment.NewLine}" +
                                       $"ClothesName: {loc8}{Environment.NewLine}" +
                                       $"ClothesId: {loc9}{Environment.NewLine}" +
                                       $"Colors: {(string.IsNullOrEmpty(loc10) ? "None" : loc10)}{Environment.NewLine}" +
                                       $"IsRareItem: {loc16}{Environment.NewLine}" +
                                       $"IsVipItem: {loc14}{Environment.NewLine}" +
                                       $"IsDiamondItem: {loc15}{Environment.NewLine}{Environment.NewLine}";

                        File.AppendAllText(loc17, loc18);
                    }
                }

                AnsiConsole.MarkupLine(
                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]checked all clothes and extracted to {server}-{loc5}-clothes.txt  :)[/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();
            }

            static void usernameToActorid(string server)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Username To ActorId").LeftJustified()
                    .RoundedBorder());
                var loc2 = AnsiConsole.Prompt(new Spectre.Console.TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                    .PromptStyle("#71d5fb"));

                dynamic loc1 = AmfCall(server,
                    "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                    new object[1] { loc2 });

                if (loc1 == -1)
                {
                    Console.WriteLine(
                        "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    double loc3 = loc1;

                    AnsiConsole.MarkupLine(
                        $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]ActorId: {loc3} | Username: {loc2} :)[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void actorIdToUsername(string server)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ ActorId to Username").LeftJustified()
                    .RoundedBorder());
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] actorid: ")
                    .PromptStyle("#71d5fb"));

                dynamic loc5 = AmfCall(server,
                    "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorNameFromId",
                    new object[1]
                        { loc1 });

                string loc2 = loc5;
                AnsiConsole.MarkupLine(
                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Username: {loc2} | ActorId: {loc1}  :)[/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();
            }

            static void itemTracker(string server)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Item Tracker").LeftJustified()
                    .RoundedBorder());
                int loc1 = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] ActorClothesRelId: ")
                    .PromptStyle("#71d5fb"));

                dynamic loc4 = AmfCall(server,
                    "MovieStarPlanet.WebService.MovieStar.AMFMovieStarService.GetActorClothesRel",
                    new object[1]
                        { loc1 });

                int loc2 = loc4["ActorId"];

                dynamic loc5 = AmfCall(server,
                    "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorNameFromId",
                    new object[1]
                        { loc2 });

                string loc3 = loc5;

                string loc6 = loc4["Cloth"]["Name"] ?? "Unknown";
                int loc7 = loc4["ClothesId"];
                string loc8 = loc4["Color"].ToString();
                string loc9 = loc4["Cloth"]["ShopId"].ToString();
                int loc10 = loc4["Cloth"]["Vip"];
                int loc11 = loc4["Cloth"]["DiamondsPrice"];

                string loc12 = loc11 != 0 ? "Yes" : "No";
                string loc13 = loc10 != 0 ? "Yes" : "No";
                string loc14 = loc9 != "-100" ? "Yes" : "No";

                AnsiConsole.MarkupLine($"[#71d5fb]ActorClothesRelId:[/] {loc1}");
                AnsiConsole.MarkupLine($"[#71d5fb]ClothesName:[/] {loc6}");
                AnsiConsole.MarkupLine($"[#71d5fb]ClothesId:[/] {loc7}");
                if (!string.IsNullOrEmpty(loc8))
                {
                    AnsiConsole.MarkupLine($"[#71d5fb]Colors:[/] {loc8}");
                }
                else
                {
                    AnsiConsole.MarkupLine("[#71d5fb]Colors:[/] None");
                }

                AnsiConsole.MarkupLine($"[#71d5fb]IsRareItem:[/] {loc14}");
                AnsiConsole.MarkupLine($"[#71d5fb]IsVipItem:[/] {loc13}");
                AnsiConsole.MarkupLine($"[#71d5fb]IsDiamondItem:[/] {loc12}");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(
                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Item tracked to {loc3}  :)[/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();

            }

            static void roomChanger(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ RoomChanger").LeftJustified().RoundedBorder());
                Console.Write("\n");
                AnsiConsole.Markup(
                    "[slowblink][[[#c70000]?![/]]] Use it at your own risk, we are not responsible for your misdeeds.[/]\n");
                string loc1 = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter image url: ")
                    .PromptStyle("#71d5fb"));
                WebClient loc2 = new WebClient();
                byte[] loc3 = loc2.DownloadData(loc1);

                dynamic loc4 = AmfCall(server,
                    "MovieStarPlanet.WebService.Snapshots.AMFGenericSnapshotService.CreateSnapshot",
                    new object[5]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        "room",
                        loc3,
                        "jpg"
                    });

                dynamic loc5 = AmfCall(server,
                    "MovieStarPlanet.WebService.Snapshots.AMFGenericSnapshotService.CreateSnapshot",
                    new object[5]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        "roomProfile",
                        loc3,
                        "jpg"
                    });
                dynamic loc6 = AmfCall(server,
                    "MovieStarPlanet.WebService.Snapshots.AMFGenericSnapshotService.CreateSnapshot",
                    new object[5]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        "roomMedium",
                        loc3,
                        "jpg"
                    });
                if (loc4 && loc5 && loc6)
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Room changed[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void animationsExtractor(string server, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Animation Extractor").LeftJustified()
                    .RoundedBorder());
                var loc4 = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                    .PromptStyle("#71d5fb"));

                dynamic loc1 = AmfCall(server,
                    "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                    new object[1] { loc4 });

                if (loc1 == -1)
                {
                    Console.WriteLine(
                        "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    double loc12 = loc1;

                    dynamic loc2 = AmfCall(server,
                        "MovieStarPlanet.WebService.Media.AMFMediaService.GetMyAnimations",
                        new object[2]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            loc12,
                        });

                    foreach (var loc3 in loc2)
                    {
                        string loc5 = loc3["Animation"]["Name"] ?? "Unknown";
                        int loc6 = loc3["Animation"]["AnimationId"] ?? -1;
                        int loc7 = loc3["ActorAnimationRelId"] ?? -1;
                        int loc8 = loc3["Animation"]["Deleted"];
                        int? loc9 = loc3["Animation"]["Vip"] as int?;

                        string loc10 = loc9.HasValue ? loc9.Value != 0 ? "Yes" : "No" : "None";
                        string loc11 = loc8 != 0 ? "Yes" : "No";

                        AnsiConsole.MarkupLine($"[#71d5fb]Name:[/] {loc5}");
                        AnsiConsole.MarkupLine($"[#71d5fb]AnimationId:[/] {loc6}");
                        AnsiConsole.MarkupLine($"[#71d5fb]ActorAnimationRelId:[/] {loc7}");
                        AnsiConsole.MarkupLine($"[#71d5fb]IsRareItem:[/] {loc11}");
                        AnsiConsole.MarkupLine($"[#71d5fb]IsVipItem:[/] {loc10}");
                        AnsiConsole.MarkupLine("");
                        string loc13 = $"{server}-{loc4}-animations.txt";
                        string loc14 = $"Name: {loc5}{Environment.NewLine}" +
                                       $"AnimationId: {loc6}{Environment.NewLine}" +
                                       $"ActorAnimationRelId: {loc7}{Environment.NewLine}" +
                                       $"IsRareItem: {loc11}{Environment.NewLine}" +
                                       $"IsVipItem: {loc10}{Environment.NewLine}{Environment.NewLine}";

                        File.AppendAllText(loc13, loc14);
                    }
                }

                AnsiConsole.MarkupLine(
                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]checked all animations and extracted to {server}-{loc4}-animations.txt  :)[/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();
            }

            static void iconChanger(string server, int actorId, string ticket)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Icon Changer").LeftJustified()
                    .RoundedBorder());
                Console.Write("\n");
                AnsiConsole.Markup(
                    "[slowblink][[[#c70000]?![/]]] Use it at your own risk, we are not responsible for your misdeeds.[/]\n");
                string loc1 = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter image url: ")
                    .PromptStyle("#71d5fb"));
                WebClient loc2 = new WebClient();
                byte[] loc3 = loc2.DownloadData(loc1);

                dynamic loc4 = AmfCall(server,
                    "MovieStarPlanet.WebService.Snapshots.AMFGenericSnapshotService.CreateSnapshot",
                    new object[5]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        "moviestar",
                        loc3,
                        "jpg"
                    });

                dynamic loc5 = AmfCall(server,
                    "MovieStarPlanet.WebService.Snapshots.AMFGenericSnapshotService.CreateSnapshot",
                    new object[5]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        "fullSizeMoviestar",
                        loc3,
                        "jpg"
                    });
                if (loc4 && loc5)
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Icon changed[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            static void botGenerator(string server, string culture)
            {
                Console.Clear();
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Bot Generator").LeftJustified()
                    .RoundedBorder());

                int loc13 = AnsiConsole.Prompt(
                    new Spectre.Console.TextPrompt<int>("[[[#71d5fb]+[/]]] Amount of bots: ").PromptStyle("#71d5fb"));

                string loc29 = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] Pass for bots: ")
                    .PromptStyle("#71d5fb"));

                for (int i = 0; i < loc13; i++)
                {
                    string loc1 = CaptchaV3();
                    WebClient loc5 = new WebClient();
                    string loc6 = loc5.DownloadString("https://" + ((server.ToUpper() == "US") ? "us" : "eu") +
                                                      $".mspapis.com/profileidentity/v1/profiles/names/suggestions/?&gameId=5ooi&culture={culture}");
                    System.Collections.Generic.List<string> loc7 = JsonConvert.DeserializeObject<List<string>>(loc6);
                    string loc2 = string.Join("", loc7);
                    string loc3 = loc29;
                    string loc4 = BitConverter
                        .ToString(new HMACSHA256(Encoding.UTF8.GetBytes("7jA7^kAZSHtjxDAa")).ComputeHash(
                            Encoding.UTF8.GetBytes("5ooi" + server + loc3 + loc2 + "false"))).Replace("-", "")
                        .ToLower();
                    WebClient loc8 = new WebClient();
                    loc8.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string loc12 = loc8.UploadString(
                        "https://" + ((server.ToUpper() == "US") ? "us" : "eu") +
                        ".mspapis.com/edgelogins/graphql/graphql",
                        JsonConvert.SerializeObject(new
                        {
                            query =
                                "mutation create ($loginName: String!, $password: String!, $gameId: String!, $isGuest: Boolean!, $countryCode: Region!, $checksum: String!, $recaptchaV3Token: String ){createLoginProfile(input: { name: $loginName, password: $password, gameId: $gameId, region: $countryCode, isGuest: $isGuest }, verify: {checksum: $checksum, recaptchaV3Token: $recaptchaV3Token } ) {success,loginProfile {loginId,loginName,profileId,profileName,isGuest},error}}",
                            variables = "{\"checksum\": \"" + loc4 + "\", \"loginName\": \"" + loc2 +
                                        "\", \"password\": \"" + loc3 +
                                        "\", \"gameId\": \"5ooi\", \"isGuest\": false, \"countryCode\": \"" +
                                        server.ToUpper() + "\", \"recaptchaV3Token\": \"" + loc1 + "\"}",
                            operationName = ""
                        }));
                    JObject loc9 = JObject.Parse(loc12);
                    bool loc10 = (bool)loc9["data"]["createLoginProfile"]["success"];
                    dynamic loc15 = AmfCall(server, "MovieStarPlanet.WebService.User.AMFUserServiceWeb.Login",
                        new object[6] { loc2, loc3, new object[] { }, null, null, "MSP1-Standalone:XXXXXX" });
                    if (loc15["loginStatus"]["status"] != "ThirdPartyCreated")
                        Console.WriteLine("\n" + $"not validated with msp1 {loc2}");
                    else
                    {
                        int loc27 = loc15["loginStatus"]["actor"]["ActorId"];
                        string loc28 = loc15["loginStatus"]["ticket"];
                        string loc25 = loc15["loginStatus"]["nebulaLoginStatus"]["accessToken"];
                        string loc26 = loc15["loginStatus"]["nebulaLoginStatus"]["profileId"];
                        Console.WriteLine("\n" + $"attempting to validate with msp1 {loc2}");

                        WebClient loc18 = new WebClient();
                        var loc16 =
                            loc18.DownloadData(
                                $"https://snapshots.mspcdns.com/v1/MSP/GB/snapshot/fullSizeMoviestar/1.jpg");
                        var loc17 =
                            loc18.DownloadData($"https://snapshots.mspcdns.com/v1/MSP/GB/snapshot/moviestar/4.jpg");
                        dynamic loc19 = AmfCall(server,
                            "MovieStarPlanet.WebService.AMFActorService.ThirdPartySaveAvatar",
                            new object[]
                            {
                                new
                                {
                                    Clothes = new object[]
                                    {
                                        new
                                        {
                                            ActorClothesRelId = -10, ActorId = -1, ClothesId = 18671, IsWearing = 1,
                                            y = 0, Color = 0, x = 0
                                        },
                                        new
                                        {
                                            ActorClothesRelId = -14, ActorId = -1, ClothesId = 22252, IsWearing = 1,
                                            y = 0, Color = "0,0,0", x = 0
                                        },
                                        new
                                        {
                                            ActorClothesRelId = -15, ActorId = -1, ClothesId = 19121, IsWearing = 1,
                                            y = 0, Color = "0x000000,0xffffff,0x000000", x = 0
                                        },
                                        new
                                        {
                                            ActorClothesRelId = -16, ActorId = -1, ClothesId = 18931, IsWearing = 1,
                                            y = 0, Color = "0xFFFCF7,0xF5F6FF", x = 0
                                        }
                                    },
                                    MouthColors = "skincolor,0xB67676", MouthId = 35, NoseId = 28,
                                    InvitedByActorId = -1, ChosenActorName = loc2, ChosenPassword = loc3,
                                    SkinIsMale = true, EyeColors = "0x0,0x000000", EyeId = 6, SkinColor = "14195824"
                                },
                                loc17, loc16, loc2, loc3
                            });
                        
                        WSConn(server, loc26,loc25);

                        dynamic loc62 = AmfCall(server,
                            "MovieStarPlanet.WebService.ActorService.AMFActorServiceForWeb.GetPostLoginBundleStandalone",
                            new object[2] { new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, loc27 });

                        dynamic loc55 = AmfCall(server,
                            "MovieStarPlanet.WebService.Awarding.AMFAwardingService.claimDailyAward",
                            new object[4]
                            {
                                new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, "wheel", 120, loc27
                            });

                        dynamic loc63 = AmfCall(server,
                            "MovieStarPlanet.WebService.Awarding.AMFAwardingService.claimDailyAward",
                            new object[4]
                            {
                                new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, "starwheel", 120, loc27
                            });

                        for (int y = 0; i < 13; i++)
                        {
                            dynamic loc64 = AmfCall(server,
                                "MovieStarPlanet.WebService.AMFAwardService.claimDailyAward",
                                new object[4]
                                {
                                    new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, "twoPlayerFame",
                                    50, loc27
                                });
                        }

                        for (int z = 0; i < 25; i++)
                        {
                            dynamic loc56 = AmfCall(server,
                                "MovieStarPlanet.WebService.AMFSpendingService.BuyClothes",
                                new object[4]
                                {
                                    new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, loc27,
                                    new object[]
                                    {
                                        new
                                        {
                                            Color = 0xFC67CC, y = 0, ActorClothesRelId = 0, ActorId = loc27,
                                            ClothesId = 30217, IsWearing = 0, x = 0
                                        },
                                    },
                                    0
                                });
                        }

                        dynamic loc67 = AmfCall(server,
                            "MovieStarPlanet.WebService.Achievement.AMFAchievementWebService.ClaimReward",
                            new object[3]
                            {
                                new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, "LUCKY_YOU", loc27
                            });

                        for (int d = 0; i < 1; i++)
                        {
                            dynamic loc68 = AmfCall(server,
                                "MovieStarPlanet.WebService.Achievement.AMFAchievementWebService.ClaimReward",
                                new object[3]
                                {
                                    new TicketHeader { anyAttribute = null, Ticket = actor(loc28) }, "CRAZY_COLLECTOR",
                                    loc27
                                });

                            if (loc10)
                            {
                                AnsiConsole.Markup(
                                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]{loc2}:{loc3}[/][/]");
                                string loc11 = $"bots-{server}.txt";
                                File.AppendAllText(loc11, loc2 + ":" + loc3 + Environment.NewLine);
                            }
                            else
                            {
                                AnsiConsole.Markup(
                                    $"\n[#fa1414]FAILED[/] > [#f7b136][underline]{loc2}[/][/]");
                            }
                        }
                    }

                    AnsiConsole.MarkupLine(
                        "\n[#71d5fb][/] > [#f7b136][underline]created all bots :)[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }


        static void itemGlitcher(string server, string ticket, int actorId, string accessToken, string profileId)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Item Glitcher").LeftJustified()
                .RoundedBorder());
            var username = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                .PromptStyle("#71d5fb"));
            var relid = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] relid: ")
                .PromptStyle("#71d5fb"));

            dynamic loc1 = AmfCall(server,
                "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                new object[1] { username });

            if (loc1 == -1)
            {
                Console.WriteLine(
                    "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                double friendActorId = loc1;

                WSConn(server, profileId, accessToken);

                dynamic loc4 = AmfCall(server,
                    "MovieStarPlanet.WebService.Gifts.AMFGiftsService+Version2.GiveGiftOfCategory",
                    new object[7]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        friendActorId,
                        relid,
                        0,
                        1,
                        "Gift_item_5.swf"
                    });

                if (loc4["Code"] != 0)
                {
                    AnsiConsole.Markup("\n[#fa1414]FAILED[/] > [#f7b136][underline]"
                                       + (loc4["Description"] ?? "Unknown") +
                                       "[/] [[Click any key to return to Home]][/]");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    AnsiConsole.Markup(
                        $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Gifted item to {username}![/] [[Click any key to return to Home]][/]");
                }
            }

            Console.ReadKey();
            Console.Clear();
        }

        static void automatedAutographer(string server, string ticket, int actorId, string accessToken,
            string profileId)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Automated Autographer (VIP)").LeftJustified()
                .RoundedBorder());
            var username = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                .PromptStyle("#71d5fb"));

            var amountofautos = AnsiConsole.Prompt(new TextPrompt<int>("[[[#71d5fb]+[/]]] enter amount of autos: ")
                .PromptStyle("#71d5fb"));

            dynamic loc1 = AmfCall(server,
                "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                new object[1] { username });

            if (loc1 == -1)
            {
                Console.WriteLine(
                    "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                double friendActorId = loc1;
                WSConn(server, profileId, accessToken);

                for (int i = 0; i < amountofautos; i++)
                {
                    dynamic loc4 = AmfCall(server,
                        "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GiveAutographAndCalculateTimestamp",
                        new object[3]
                        {
                            new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                            actorId,
                            friendActorId
                        });

                    if (loc4["Fame"] == 0)
                    {
                        AnsiConsole.Markup("\n[#fa1414]FAILED[/] > [#f7b136][underline]"
                                           + ("Unknown") +
                                           "[/][/]");
                    }
                    else
                    {
                        AnsiConsole.Markup(
                            $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Sent Autograph to {username}![/][/]");
                        if (i < amountofautos - 1)
                        {
                            Thread.Sleep(TimeSpan.FromMinutes(2));
                        }
                    }
                }
            }

            AnsiConsole.Markup(
                $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Sent Autograph  {amountofautos} to {username}![/] [[Click any key to return to Home]][/]");
            Console.ReadKey();
            Console.Clear();
        }

        static void sfAutomatedFarmer()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ starFame - Fame Farmer").LeftJustified()
                .RoundedBorder());
            AnsiConsole.Markup(
                $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Coming soon![/] [[Click any key to return to Home]][/]");
            Console.ReadKey();
            Console.Clear();
        }

        static void passwordChanger(string server, string ticket, int actorId, string password)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Password Changer").LeftJustified()
                .RoundedBorder());

            var newPassword = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] new password: ")
                .PromptStyle("#71d5fb"));

            dynamic loc4 = AmfCall(server,
                "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.ChangePasswordNew",
                new object[4]
                {
                    new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                    actorId,
                    password,
                    newPassword
                });

            AnsiConsole.Markup(
                $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]password changed![/] [[Click any key to return to Home]][/]");
            Console.ReadKey();
            Console.Clear();
        }

        static void friendRequester(string server, string ticket, int actorId)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Friend Requester").LeftJustified()
                .RoundedBorder());

            var username = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] username: ")
                .PromptStyle("#71d5fb"));

            dynamic loc1 = AmfCall(server,
                "MovieStarPlanet.WebService.UserSession.AMFUserSessionService.GetActorIdFromName",
                new object[1] { username });

            if (loc1 == -1)
            {
                Console.WriteLine(
                    "\n\x1b[91mFAILED\u001b[39m > \x1b[93mThe account doesn't exist or has been deleted [Click any key to return to login]");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                double friendActorId = loc1;

                dynamic loc4 = AmfCall(server,
                    "MovieStarPlanet.WebService.AMFMobileFriendshipService.RequestFriendship",
                    new object[3]
                    {
                        new TicketHeader { anyAttribute = null, Ticket = actor(ticket) },
                        actorId,
                        friendActorId
                    });

                AnsiConsole.Markup(
                    $"\n[#06c70c]SUCCESS[/] > [#f7b136][underline]friend request sent to {username}![/] [[Click any key to return to Home]][/]");
                Console.ReadKey();
                Console.Clear();
            }
        }
        static async Task MSP2_Login()
        {
            Console.Clear();
            bool loc1 = false;

            while (!loc1)
            {
                AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Login MSP2").LeftJustified());
                Console.Write("\n");
                var loc2 = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter username: ")
                    .PromptStyle("#71d5fb"));

                var loc3 = AnsiConsole.Prompt(new TextPrompt<string>("[[[#71d5fb]+[/]]] Enter password: ")
                    .PromptStyle("#71d5fb")
                    .Secret());

                var loc4 = Enum.GetValues(typeof(WebServer))
                    .Cast<WebServer>()
                    .Select(ws => (ws.loc3().Item1, ws.loc3().Item2))
                    .ToArray();

                var loc5 = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[[[#71d5fb]+[/]]] Select a server: ")
                        .PageSize(15)
                        .MoreChoicesText("[grey](Move up and down to reveal more servers)[/]")
                        .AddChoices(loc4.Select(aloc1 => aloc1.Item1))
                );

                var loc6 = loc4.First(aloc1 => aloc1.Item1 == loc5).Item2;
                var loc7 = new[] { "US", "CA", "AU", "NZ" }.Contains(loc6) ? "us" : "eu";

                string loc8 = null;
                string loc9 = null;

                AnsiConsole.Status()
                    .SpinnerStyle(Spectre.Console.Style.Parse("#71d5fb"))
                    .Start("Login...", ctx =>
                    {
                        ctx.Refresh();
                        ctx.Spinner(Spinner.Known.Circle);

                        var loc10 = $"https://{loc7}-secure.mspapis.com/loginidentity/connect/token";

                        using (var loc11 = new WebClient())
                        {
                            var loc12 = new NameValueCollection
                            {
                                ["client_id"] = "unity.client",
                                ["client_secret"] = "secret",
                                ["grant_type"] = "password",
                                ["scope"] = "openid nebula offline_access",
                                ["username"] = $"{loc6}|{loc2}",
                                ["password"] = loc3,
                                ["acr_values"] = "gameId:j68d"
                            };

                            var loc13 = loc11.UploadValues(loc10, loc12);
                            var loc14 = Encoding.Default.GetString(loc13);
                            dynamic loc15 = JsonConvert.DeserializeObject(loc14);


                            var loc16 = loc15["access_token"].ToString();
                            var loc17 = loc15["refresh_token"].ToString();

                            var loc18 = new JwtSecurityTokenHandler();
                            var loc19 = loc18.ReadJwtToken(loc16);
                            var loc20 = loc19.Payload["loginId"].ToString();

                            string loc21 =
                                $"https://{loc7}.mspapis.com/profileidentity/v1/logins/{loc20}/profiles?&pageSize=100&page=1&filter=region:{loc6}";
                            loc11.Headers.Add(HttpRequestHeader.Authorization,
                                "Bearer " + loc16);
                            string loc22 = loc11.DownloadString(loc21);

                            var loc9 = JArray.Parse(loc22)[0]["id"].ToString();

                            var loc23 = new NameValueCollection
                            {
                                ["grant_type"] = "refresh_token",
                                ["refresh_token"] = loc17,
                                ["acr_values"] = $"gameId:j68d profileId:{loc9}"
                            };

                            loc11.Headers.Remove(HttpRequestHeader.Authorization);
                            loc11.Headers.Add(HttpRequestHeader.Authorization,
                                "Basic dW5pdHkuY2xpZW50OnNlY3JldA==");
                            var loc24 = loc11.UploadValues(loc10, loc23);

                            var loc25 = Encoding.Default.GetString(loc24);
                            dynamic loc26 = JsonConvert.DeserializeObject(loc25);

                            loc8 = loc26["access_token"].ToString();

                            Console.Clear();
                        }
                    });
                while (true)
                {
                    loc1 = true;
                    Console.Clear();
                    AnsiConsole.Write(
                        new Rule("[#71d5fb]MSPTOOL[/] ・ Home").LeftJustified().RoundedBorder());
                    Console.Write("\n");
                    AnsiConsole.Markup("[#71d5fb]1[/]  > Mood Changer\n");
                    AnsiConsole.Markup("[#71d5fb]2[/]  > Gender Changer\n");
                    AnsiConsole.Markup("[#71d5fb]3[/]  > Logout\n\n");
                    AnsiConsole.Write(
                        new Rule(
                                "[slowblink][#71d5fb]ham & 6c0[/][/]")
                            .RightJustified().RoundedBorder());
                    var options = AnsiConsole.Prompt(
                        new TextPrompt<string>("\n[[[#71d5fb]+[/]]] Pick an option: ")
                            .PromptStyle("#71d5fb"));

                    switch (options)
                    {
                        case "1":
                            moodChanger(loc7, loc8, loc9);
                            Thread.Sleep(2000);
                            break;
                        case "2":
                            genderChanger(loc7, loc8, loc9);
                            Thread.Sleep(2000);
                            break;
                        case "3":
                            loc1 = false;
                            break;
                        default:
                            Console.WriteLine(
                                "\n\u001b[91mERROR\u001b[39m > \u001b[93mChoose an option which exists!");
                            System.Threading.Thread.Sleep(2000);
                            Console.Clear();
                            break;
                    }

                    if (!loc1)
                        break;
                }

                ;
            }
        }

        static async Task moodChanger(string region, string accessToken, string profileId)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Change Mood").LeftJustified().RoundedBorder());

            var loc1 = new (string Name, string Value)[]
            {
                ("Bunny", "bunny_hold"),
                ("Ice Skating", "noshoes_skating"),
                ("Swimming", "swim_new"),
                ("Spider Crawl", "2023_spidercrawl_lsz"),
                ("Bubblegum", "bad_2022_teenwalk_dg"),
                ("Like a Frog", "very_2022_froglike_lsz"),
                ("Cool Slide", "cool_slide"),
                ("Like Bambi", "bambislide"),
                ("Freezing", "xmas_2022_freezing_lsz"),
            };

            var loc2 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[[[#71d5fb]+[/]]] Select a mood: ")
                    .PageSize(10)
                    .AddChoices(loc1.Select(loc3 => loc3.Name))
            );

            var loc4 = loc1.First(loc3 => loc3.Name == loc2);

            using (HttpClient loc5 = new HttpClient())
            {
                loc5.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                loc5.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36 Edg/125.0.0.0");

                string loc6 =
                    $"https://{region}.mspapis.com/profileattributes/v1/profiles/{profileId}/games/j68d/attributes";

                HttpResponseMessage loc7 = await loc5.GetAsync(loc6);

                string loc8 = await loc7.Content.ReadAsStringAsync();
                JObject loc9 = JObject.Parse(loc8);

                loc9["additionalData"]["Mood"] = loc4.Value;

                string loc10 = loc9.ToString();
                HttpContent loc11 = new StringContent(loc10);
                loc11.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage loc12 = await loc5.PutAsync(loc6, loc11);
                if (loc12.IsSuccessStatusCode)
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Mood changed[/] [[Auto redirect in 2 seconds]][/]");
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Auto redirect in 2 seconds]][/]");

                }
            }
        }

        static async Task genderChanger(string region, string accessToken, string profileId)
        {
            Console.Clear();
            AnsiConsole.Write(
                new Rule("[#71d5fb]MSPTOOL[/] ・ Home ・ Change Gender").LeftJustified().RoundedBorder());

            var loc1 = new (string Name, string Value)[]
            {
                ("Girl", "Girl"),
                ("Boy", "Boy"),
            };

            var loc2 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[[[#71d5fb]+[/]]] Select a mood: ")
                    .PageSize(10)
                    .AddChoices(loc1.Select(loc3 => loc3.Name))
            );

            var loc4 = loc1.First(loc3 => loc3.Name == loc2);

            using (HttpClient loc5 = new HttpClient())
            {
                loc5.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                loc5.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36 Edg/125.0.0.0");

                string loc6 =
                    $"https://{region}.mspapis.com/profileattributes/v1/profiles/{profileId}/games/j68d/attributes";

                HttpResponseMessage loc7 = await loc5.GetAsync(loc6);

                string loc8 = await loc7.Content.ReadAsStringAsync();
                JObject loc9 = JObject.Parse(loc8);

                loc9["additionalData"]["Gender"] = loc4.Value;

                string loc10 = loc9.ToString();
                HttpContent loc11 = new StringContent(loc10);
                loc11.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage loc12 = await loc5.PutAsync(loc6, loc11);
                if (loc12.IsSuccessStatusCode)
                {
                    AnsiConsole.Markup(
                        "\n[#06c70c]SUCCESS[/] > [#f7b136][underline]Gender changed[/] [[Auto redirect in 2 seconds]][/]");
                }
                else
                {
                    AnsiConsole.Markup(
                        "\n[#fa1414]FAILED[/] > [#f7b136][underline]Unknown[/] [[Auto redirect in 2 seconds]][/]");
                }
            }
        }

        static bool vloc2()
        {
            using (HttpClient loc1 = new HttpClient())
            {
                string vloc4 = loc1.GetStringAsync(vloc3).Result;
                return vloc1.Trim() == vloc4.Trim();
            }
        }

        private static async Task InstallUpdate(string latestVersion)
        {
            string loc1 = XIU(latestVersion);

            string loc2 = string.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                loc2 = string.Format($"https://github.com/r-h-y/star/releases/download/{loc1}/msptool.exe", loc1);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                loc2 = string.Format(
                    $"https://github.com/r-h-y/star/releases/download/{loc1}/msptool_macos_arm64.zip", loc1);
            }

            string loc3 = string.Format(loc2, loc1);
            string loc4 = Path.Combine(Path.GetTempPath(), "msptool");
            string loc5 = Process.GetCurrentProcess().MainModule.FileName;
            using HttpClient loc6 = new HttpClient();

            byte[] loc7 = await loc6.GetByteArrayAsync(loc3);
            File.WriteAllBytes(loc4, loc7);
            File.Replace(loc4, loc5, null);
            Process.Start(new ProcessStartInfo { FileName = loc5, UseShellExecute = true });
            Environment.Exit(0);
        }

        private static string XIU(string version)
        {
            version = version.Replace("\r", "").Replace("\n", "");
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                version = version.Replace(c, '_');
            }

            return "v" + version.Trim();
        }

        static Dictionary<string, (string server, string username, string password)> login2()
        {
            var accounts = new System.Collections.Generic.Dictionary<string, (string server, string username, string password)>();

            if (File.Exists("mtlogin.txt"))
            {
                var loc1 = File.ReadAllLines("mtlogin.txt");
                foreach (var loc2 in loc1)
                {
                    var loc3 = loc2.Split(':');
                    if (loc3.Length == 3)
                    {
                        var loc4 = $"{loc3[1]}@{loc3[0]}";
                        accounts[loc4] = (loc3[0], loc3[1], loc3[2]);
                    }
                }
            }

            return accounts;
        }
    }
}
