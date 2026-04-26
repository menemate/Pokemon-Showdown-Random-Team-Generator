using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;

public enum Type
{
	Normal, Fighting, Flying, Poison, Ground, Rock, Bug, Ghost, Steel, Fire, Water, Grass,
	Electric, Psychic, Ice, Dragon, Dark, Fairy, Stellar, Nothing
}
public class Pokemon
{
	public string name {get; set;}
	public Type type1 {get; set;}
	public Type type2 {get; set;}
	public string stats {get; set;}
	public int generation {get; set;}
	public int bst{get; set;}
	public bool fullevo {get; set;}
	public double rate {get; set;}
	public string moreInfo {get; set;}

	public Pokemon(string n, Type t1, Type t2, string s, int g, int b, bool f, double r, string m)
	{
		this.name		= n;
		this.type1		= t1;
		this.type2		= t2;
		this.stats		= s;
		this.generation	= g;
		this.bst		= b;
		this.fullevo	= f;
		this.rate		= r;
		this.moreInfo	= m;
	}
}

public class Move
{
	public string name {get; set;}
	public Type type {get; set;}
	public int category {get; set;}
	public int power {get; set;}
	public int accuracy {get; set;}
	public int generation {get; set;}
	public List<string> props {get; set;}
	public double rate {get; set;}

	public Move(string n, Type t, int c, int p, int a, int g, List<string> ps, double r)
	{
		this.name		= n;
		this.type		= t;
		this.category	= c;
		this.power		= p;
		this.accuracy	= a;
		this.generation	= g;
		this.props		= ps;
		this.rate		= r;
	}
}
public class Ability
{
	public string name {get; set;}
	public string stats {get; set;}
	public int generation {get;set;}
	public List<string> props {get; set;}
	public double rate {get; set;}

	public Ability(string n, string s, int g, List<string> ps, double r)
	{
		this.name		= n;
		this.stats		= s;
		this.generation	= g;
		this.props		= ps;
		this.rate		= r;
	}
}
public class Item
{
	public string name {get; set;}
	public string stats {get; set;}
	public List<string> props {get; set;}
	public double rate {get; set;}
	public double ogRate {get; set;}

	public Item(string n, string s, List<string> ps, double r)
	{
		this.name		= n;
		this.stats		= s;
		this.props		= ps;
		this.rate		= r;
		this.ogRate		= r;
	}
}
public class PokemonGenerator
{
	public static void Main(string[] args)
	{
		string outputPath			= "data/pokepaste.txt";
		string outputPathTeams		= "data/GymLeaderTeams.txt";
		string outputPathPokemon	= "data/GymLeaderPokemon.txt";
		List<Pokemon> pokemons		= new List<Pokemon> {};
		List<Item> items			= new List<Item> {};
		List<Move> moves			= new List<Move> {};
		List<Ability> abilities		= new List<Ability> {};
		List<Type> teras			= new List<Type> {};

		//	Pokemon Trainer Tournament files.
		bool trainerTeams			= false;		//	Generate a row for the trainer team.
		bool trainerPokemon			= false;		//	Generate the team format.
		int startOnLine				= 1;			//	In case it's not the first team, change the line to something like 61, 121...

		bool banMegas				= false;		//	When true, no pokemon can mega evolve, it becomes true after 1 Mega is in the team.
		bool banZcrystals			= false;		//	When true, Z-Crystal won't appear, it becomes true after 1 Z-Crystal is on the team.
		bool notSingles				= false;		//	When true, doubles only moves appear
		string currentTerrain		= "none";		//	Will automatically change to "grass", "electric", "psychic" or "misty".
		int nPokemons				= 6;			//	How many pokemon in the team.

		//	Give pokemon a level.
		//	Calculation is (levelStart + Math.Min(255, Math.Round((1 - (double)(randomPokemon.bst - levelMinBST) / (levelMaxBST - levelMinBST))  * levelMult)))
		bool useLevels				= false;		//	Enable different levels per pokemon.
		int leveledPokemons			= 12;			//	How many pokemons have a level, if the value is 0, only the first pokemon has a level.
		int levelStart				= 100;			//	Minimum possible level of a pokemon.
		int levelMinBST				= 400;			//	Lowest BST to compare.
		int levelMaxBST				= 720;			//	Highest BST to compare.
		double levelMult			= 25;			//	Pokemon from Lowest to Highest BST range within levelStart and levelStart + levelMult

		//	Filter teams to make sure they use only certain pokemon, abilities or moves
		bool restrictMons			= false;		//	When true, uses includeMonTypes, includeMonGens, includeMonBST and includeMonName as filters.
		bool restrictAbils			= false;		//	When true, uses includeAbilGens and includeAbilName as filters.
		bool restrictMoves			= false;		//	When true, uses includeMoveTypes, includeMoveGens and includeMoveName as filters.

/*
	Type.Normal, Type.Fighting, Type.Flying, Type.Poison,
	Type.Ground, Type.Rock, Type.Bug, Type.Ghost,
	Type.Steel, Type.Fire, Type.Water, Type.Grass,
	Type.Electric, Type.Psychic, Type.Ice, Type.Dragon,
	Type.Dark, Type.Fairy, Type.Nothing
*/
		List<Type> includeMonTypes = new List<Type>
		{
			Type.Normal, Type.Fighting, Type.Flying, Type.Poison,
			Type.Ground, Type.Rock, Type.Bug, Type.Ghost,
			Type.Steel, Type.Fire, Type.Water, Type.Grass,
			Type.Electric, Type.Psychic, Type.Ice, Type.Dragon,
			Type.Dark, Type.Fairy, Type.Nothing
		};
		List<Type> includeMoveTypes = new List<Type>
		{
			Type.Normal, Type.Fighting, Type.Flying, Type.Poison,
			Type.Ground, Type.Rock, Type.Bug, Type.Ghost,
			Type.Steel, Type.Fire, Type.Water, Type.Grass,
			Type.Electric, Type.Psychic, Type.Ice, Type.Dragon,
			Type.Dark, Type.Fairy, Type.Nothing
		};
		List<int> includeMonGens	= new List<int>{	1, 2, 3, 4, 5, 6, 7, 8, 9	};
		List<int> includeMoveGens	= new List<int>{	1, 2, 3, 4, 5, 6, 7, 8, 9	};
		List<int> includeAbilGens	= new List<int>{	1, 2, 3, 4, 5, 6, 7, 8, 9	};

		int[] includeMonBST	= {0, 1000};	//	Minimum and maximum Base Stat Total to include in the team, min and max values are included.

/* "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" */
		List<string> includeMonName		= new List<string>{ "su", "mo", "me", "ve", "ma", "ju", "sa", "ra", "ke"};
		List<string> includeAbilName	= new List<string>{ "su", "mo", "me", "ve", "ma", "ju", "sa", "ra", "ke"};
		List<string> includeMoveName	= new List<string>{ "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};


		var lines			= new List<string>();
		var linesTeams		= new List<string>();
		var linesPokemon	= new List<string>();

		string pokemonsPath		= "data/pokemons.txt";
		string movesPath		= "data/moves.txt";
		string abilitiesPath	= "data/abilities.txt";
		string itemsPath		= "data/items.txt";
		string namesPath		= "data/names.txt";

		List<string> names		= new List<string>(File.ReadAllLines(namesPath));

		//	Read pokemon file.
		List<string> pokeLines	= new List<string>(File.ReadAllLines(pokemonsPath));
		foreach(string line in pokeLines)
		{
			string[] rows = line.Split('|');
			Pokemon pokemon = new Pokemon("", Type.Nothing, Type.Nothing, "", 0, 0, false, 100, "");
			for(int i = 0; i < 8; i++)
			{
				switch(i)
				{
					case 0:	pokemon.name = rows[i];	break;
					case 1:
						{
							Type t = Type.Nothing;
							switch(rows[i])
							{
								case "Normal": t = Type.Normal; break;
								case "Fighting": t = Type.Fighting; break;
								case "Flying": t = Type.Flying; break;
								case "Poison": t = Type.Poison; break;
								case "Ground": t = Type.Ground; break;
								case "Rock": t = Type.Rock; break;
								case "Bug": t = Type.Bug; break;
								case "Ghost": t = Type.Ghost; break;
								case "Steel": t = Type.Steel; break;
								case "Fire": t = Type.Fire; break;
								case "Water": t = Type.Water; break;
								case "Grass": t = Type.Grass; break;
								case "Electric": t = Type.Electric; break;
								case "Psychic": t = Type.Psychic; break;
								case "Ice": t = Type.Ice; break;
								case "Dragon": t = Type.Dragon; break;
								case "Dark": t = Type.Dark; break;
								case "Fairy": t = Type.Fairy; break;
								case "Nothing":
								default: Console.WriteLine("Error on pokemon type 1:" + rows[i]);
								break;
							}
							pokemon.type1 = t;
						}
					break;
					case 2:
						{
							Type t = Type.Nothing;
							switch(rows[i])
							{
								case "Normal": t = Type.Normal; break;
								case "Fighting": t = Type.Fighting; break;
								case "Flying": t = Type.Flying; break;
								case "Poison": t = Type.Poison; break;
								case "Ground": t = Type.Ground; break;
								case "Rock": t = Type.Rock; break;
								case "Bug": t = Type.Bug; break;
								case "Ghost": t = Type.Ghost; break;
								case "Steel": t = Type.Steel; break;
								case "Fire": t = Type.Fire; break;
								case "Water": t = Type.Water; break;
								case "Grass": t = Type.Grass; break;
								case "Electric": t = Type.Electric; break;
								case "Psychic": t = Type.Psychic; break;
								case "Ice": t = Type.Ice; break;
								case "Dragon": t = Type.Dragon; break;
								case "Dark": t = Type.Dark; break;
								case "Fairy": t = Type.Fairy; break;
								case "Nothing":	t = Type.Nothing; break;
								default: Console.WriteLine("Error on pokemon type 2:" + rows[i]);
								break;
							}
							pokemon.type2 = t;
						}
					break;
					case 3:	pokemon.stats = rows[i];	break;
					case 4:	pokemon.generation = Convert.ToInt32(rows[i]);	break;
					case 5:	pokemon.bst = Convert.ToInt32(rows[i]);	break;
					case 6:	
						pokemon.fullevo = Convert.ToBoolean(rows[i]);
						if(!pokemon.fullevo)
							pokemon.rate *= 1;
					break;
				}
				if(rows.Length == 8 && i == 7)
					pokemon.moreInfo = rows[i];
			}
			pokemons.Add(pokemon);
		}

		//	Read moves file.
		List<string> moveLines	= new List<string>(File.ReadAllLines(movesPath));
		foreach(string line in moveLines)
		{
			string[] rows = line.Split('|');
			Move move = new Move("", Type.Nothing, 0, 0, 0, 0, new List<string> {}, 100);
			for(int i = 0; i < 7; i++)
			{
				switch(i)
				{
					case 0:	move.name = rows[i];	break;
					case 1:
						{
							Type t = Type.Nothing;
							switch(rows[i])
							{
								case "Normal": t = Type.Normal; break;
								case "Fighting": t = Type.Fighting; break;
								case "Flying": t = Type.Flying; break;
								case "Poison": t = Type.Poison; break;
								case "Ground": t = Type.Ground; break;
								case "Rock": t = Type.Rock; break;
								case "Bug": t = Type.Bug; break;
								case "Ghost": t = Type.Ghost; break;
								case "Steel": t = Type.Steel; break;
								case "Fire": t = Type.Fire; break;
								case "Water": t = Type.Water; break;
								case "Grass": t = Type.Grass; break;
								case "Electric": t = Type.Electric; break;
								case "Psychic": t = Type.Psychic; break;
								case "Ice": t = Type.Ice; break;
								case "Dragon": t = Type.Dragon; break;
								case "Dark": t = Type.Dark; break;
								case "Fairy": t = Type.Fairy; break;
								default: Console.WriteLine("Error on move type:" + rows[i]); break;
							}
							move.type = t;
						}
					break;
					case 2:	{
						int cat = 0;
						switch(rows[i])
						{
							case "Physical":	cat = 0; break;
							case "Special":	cat = 1; break;
							case "Status":	cat = 2; break;
							default:	Console.WriteLine("Move Category is wrong: " + rows[i]); break;
						}
						move.category = cat;
					}	break;
					case 3:	move.power = Convert.ToInt32(rows[i]);	break;
					case 4:	move.accuracy = Convert.ToInt32(rows[i]);	break;
					case 5:	move.generation = Convert.ToInt32(rows[i]);	break;
					case 6:
						{
							string[] props = rows[i].Split(";");
							List<string> properties = new List<string>{};
							foreach(string p in props)
							{
								properties.Add(p);
							}
							move.props = properties;

							if(move.props.Contains("false"))
								move.rate *= 0;
						}	break;
				}
			}
			moves.Add(move);
		}

		//	Read abilities file.
		List<string> abilLines	= new List<string>(File.ReadAllLines(abilitiesPath));
		foreach(string line in abilLines)
		{
			string[] rows = line.Split('|');
			Ability ability = new Ability("", "", 0, new List<string>{}, 100);
			for(int i = 0; i < 4; i++)
			{
				switch(i)
				{
					case 0:	ability.name = rows[i];	break;
					case 1:	ability.stats = rows[i];	break;
					case 2:	ability.generation = Convert.ToInt32(rows[i]); break;
					case 3:	
						{
							string[] props = rows[i].Split(";");
							List<string> properties = new List<string>{};
							foreach(string p in props)
							{
								properties.Add(p);
							}
							ability.props = properties;

							if(ability.props.Contains("false"))
								ability.rate *= 0;
						}	break;
				}
			}
			abilities.Add(ability);
		}

		//	Read item file.
		List<string> itemLines	= new List<string>(File.ReadAllLines(itemsPath));
		foreach(string line in itemLines)
		{
			string[] rows = line.Split('|');
			Item item = new Item("", "", new List<string>{}, 100);
			for(int i = 0; i < 4; i++)
			{
				switch(i)
				{
					case 0:	item.name = rows[i];	break;
					case 1:	item.stats = rows[i];	break;
					case 2:	item.rate = Convert.ToDouble(rows[i]); item.ogRate = item.rate;	break;
					case 3:	
						{
							string[] props = rows[i].Split(";");
							List<string> properties = new List<string>{};
							foreach(string p in props)
							{
								properties.Add(p);
							}
							item.props = properties;

							if(item.props.Contains("false"))
							{
								item.rate *= 0;
								item.ogRate = 0;
							}
						}	break;
				}
			}
			items.Add(item);
		}

		List<string> chosenPokemons		= new List<string> {};
		List<string> chosenAbilities	= new List<string> {};
		List<string> chosenItems		= new List<string> {};
		List<List<string>> chosenMoves	= new List<List<string>> {};

		for(int i = 0; i < nPokemons; i++)
		{
			// Restricts pokemons when necessary.
			if (restrictMons)
			{
				foreach(Pokemon pokemon in pokemons)
				{
					pokemon.rate = 100;
					bool typeMatches = includeMonTypes.Any(type => type == pokemon.type1 || type == pokemon.type2);
					if(!typeMatches)
						pokemon.rate = 0;

					bool genMatches = includeMonGens.Any(gen => gen == pokemon.generation);
					if(!genMatches)
						pokemon.rate = 0;

					if(includeMonBST[0] > pokemon.bst || includeMonBST[1] < pokemon.bst)
						pokemon.rate = 0;

					bool textMatches	= includeMonName.Any(p => pokemon.name.StartsWith(p, StringComparison.OrdinalIgnoreCase));
					if(!textMatches)
						pokemon.rate = 0;
				}
			}

			//	Choose the pokemon.
			Pokemon tmpPokemon	= WeightRandomSelect.SelectRandomWeighted(pokemons, poke => poke.rate);
			Pokemon randomPokemon	= new Pokemon(tmpPokemon.name, tmpPokemon.type1, tmpPokemon.type2, tmpPokemon.stats, tmpPokemon.generation, tmpPokemon.bst, tmpPokemon.fullevo, tmpPokemon.rate, tmpPokemon.moreInfo);

			//	Assigns a mega stone to the pokemon, changes some parameters when needed. Mega Stone as item is assigned later.
			string megaType	= "nothing";
			string replaceAbility = "nothing";
			if(!banMegas && Probability.Roll(40))
			{
				switch(randomPokemon.name)
				{
					case "gengar":		megaType = "gengarite"; randomPokemon.moreInfo = "fast";  	break;
					case "gardevoir":	megaType = "gardevoirite";	break;
					case "ampharos":	megaType = "ampharosite"; randomPokemon.type2 = Type.Dragon;	randomPokemon.moreInfo = "slow";	break;
					case "venusaur":	megaType = "venusaurite"; randomPokemon.stats = "any";	break;
					case "charizard":
						if (Probability.Roll(50))
						{
							megaType = "charizardite x";
							randomPokemon.type2 = Type.Dragon;
							randomPokemon.stats = "mixed";
						}
						else
						{
							megaType = "charizardite y";
						}		break;
					case "blastoise":	megaType = "blastoisinite"; randomPokemon.stats = "spec";	break;
					case "mewtwo":
						if (Probability.Roll(50))
						{
							megaType = "mewtwonite x";
							randomPokemon.type2 = Type.Fighting;
							randomPokemon.stats	= "phys";
						}	
						else
						{
							megaType = "mewtwonite y";
						}		break;
					case "medicham":	megaType = "medichamite"; randomPokemon.stats = "phys";	break;
					case "blaziken":	megaType = "blazikenite"; randomPokemon.stats = "phys";	break;
					case "houndoom":	megaType = "houndoominite";	break;
					case "aggron":	megaType = "aggronite";	randomPokemon.type2 = Type.Nothing; randomPokemon.stats = "phys";	break;
					case "tyranitar":	megaType = "tyranitarite";		break;
					case "scizor":	megaType = "scizorite";		break;
					case "pinsir":	megaType = "pinsirite";	randomPokemon.type2 = Type.Flying;	break;
					case "banette":	megaType = "banettite";		break;
					case "lucario":
						if (Probability.Roll(50))
						{
							megaType = "lucarionite z";
							randomPokemon.stats = "spec";
							randomPokemon.moreInfo = "fast";
						}	
						else	megaType = "lucarionite";		break;
					case "abomasnow":	megaType = "abomasite";	randomPokemon.stats = "mixed";	randomPokemon.moreInfo = "slow";	break;
					case "aerodactyl":	megaType = "aerodactylite";		break;
					case "kangaskhan":	megaType = "kangaskhanite";		break;
					case "alakazam":	megaType = "alakazite";	randomPokemon.moreInfo = "fast";	break;
					case "heracross":	megaType = "heracronite";		break;
					case "mawile":	megaType = "mawilite";		break;
					case "absol":
						if(Probability.Roll(50))
						{
							megaType = "absolite z";
							randomPokemon.type2 = Type.Ghost;
							randomPokemon.moreInfo = "fast";
						}	
						else	megaType = "absolite";		break;
					case "manectric":	megaType = "manectite";	randomPokemon.moreInfo = "fast";	break;
					case "gyarados":	megaType = "gyaradosite"; randomPokemon.type2 = Type.Dark;		break;
					case "garchomp":
						if(Probability.Roll(50))
						{
							megaType = "garchompite z";
							randomPokemon.type2 = Type.Nothing;
							randomPokemon.stats = "mixed";
							randomPokemon.moreInfo = "fast";
						}	
						else	megaType = "garchompite";		break;
					case "latias":	megaType = "latiasite";		break;
					case "latios":	megaType = "latiosite";		break;
					case "sceptile":	megaType = "sceptilite"; randomPokemon.type2 = Type.Dragon;	randomPokemon.stats = "spec"; randomPokemon.moreInfo = "fast";	break;
					case "sableye":	megaType = "sablenite"; randomPokemon.stats = "tank";		break;
					case "altaria":	megaType = "altarianite"; randomPokemon.stats = "any"; randomPokemon.type2 = Type.Fairy;		break;
					case "gallade":	megaType = "galladite";		break;
					case "audino":	megaType = "audinite"; randomPokemon.type2 = Type.Fairy;		break;
					case "swampert":	megaType = "swampertite";		break;
					case "sharpedo":	megaType = "sharpedonite";		break;
					case "slowbro":	megaType = "slowbronite"; randomPokemon.stats = "spec";		break;
					case "metagross":	megaType = "metagrossite";		break;
					case "steelix":	megaType = "steelixite"; randomPokemon.stats = "phys";		break;
					case "glalie":	megaType = "glalitite";	randomPokemon.stats = "mixed";	break;
					case "diancie":	megaType = "diancite";	randomPokemon.stats = "mixed"; randomPokemon.moreInfo = "none";	break;
					case "pidgeot":	megaType = "pidgeotite"; randomPokemon.stats = "spec";		break;
					case "lopunny":	megaType = "lopunnite"; randomPokemon.type2 = Type.Fighting; randomPokemon.stats = "phys";	randomPokemon.moreInfo = "fast";	break;
					case "salamence":	megaType = "salamencite";		break;
					case "camerupt":	megaType = "cameruptite";		break;
					case "clefable":	megaType = "clefablite"; randomPokemon.type2 = Type.Flying;		break;
					case "beedrill":	megaType = "beedrillite"; randomPokemon.moreInfo = "fast";		break;
					case "starmie":	megaType = "starminite"; randomPokemon.stats = "mixed";		break;
					case "dragonite":	megaType = "dragoninite"; randomPokemon.stats = "mixed";		break;
					case "victreebel":	megaType = "victreebelite";		break;
					case "feraligatr":	megaType = "feraligite"; randomPokemon.type2 = Type.Dragon;		break;
					case "skarmory":	megaType = "skarmorite"; randomPokemon.stats = "phys";		break;
					case "meganium":	megaType = "meganiumite"; randomPokemon.type2 = Type.Fairy; randomPokemon.stats = "spec"; randomPokemon.moreInfo = "heavy";		break;
					case "froslass":	megaType = "froslassite"; randomPokemon.stats = "spec";		break;
					case "darkrai":	megaType = "darkranite";	randomPokemon.moreInfo = "heavy";	break;
					case "heatran":	megaType = "heatranite";		break;
					case "excadrill":	megaType = "excadrite";		break;
					case "scolipede":	megaType = "scolipite";		break;
					case "scrafty":	megaType = "scraftinite"; randomPokemon.stats = "phys";		break;
					case "emboar":	megaType = "emboarite"; randomPokemon.stats = "phys";		break;
					case "chanderlure":	megaType = "chanderlurite";		break;
					case "eelektross":	megaType = "eelektrossite";	randomPokemon.moreInfo = "none";	break;
					case "chesnaught":	megaType = "chesnaughtite";	randomPokemon.moreInfo = "slow";	break;
					case "greninja":	megaType = "greninjite"; randomPokemon.moreInfo = "fast";		break;
					case "delphox":	megaType = "delphoxite"; randomPokemon.moreInfo = "fast";		break;
					case "floette-eternal":	megaType = "floettite";		break;
					case "pyroar":	megaType = "pyroarite";	randomPokemon.moreInfo = "fast";	break;
					case "barbaracle":	megaType = "barbaracite"; randomPokemon.type2 = Type.Fighting;		break;
					case "dragalge":	megaType = "dragalgite";		break;
					case "hawlucha":	megaType = "hawluchanite";		break;
					case "zygarde-complete":	megaType = "zygardite";	randomPokemon.stats = "spec";	break;
					case "drampa":	megaType = "drampanite";		break;
					case "zeraora":	megaType = "zeraorite";		break;
					case "malamar":	megaType = "malamarite";		break;
					case "falinks":	megaType = "falinksite";		break;
					case "raichu":
						if (Probability.Roll(50))
						{
							megaType = "raichunite x";
							randomPokemon.stats = "phys";
						}	
						else
						{
							megaType = "raichunite y";
							randomPokemon.stats = "spec";
							randomPokemon.moreInfo = "fast";
						}		break;
					case "chimecho":	megaType = "chimechite"; randomPokemon.type2 = Type.Steel;		break;
					case "staraptor":	megaType = "staraptite"; randomPokemon.type1 = Type.Fighting;		break;
					case "golurk":	megaType = "golurkite";		break;
					case "crabominable":	megaType = "crabominite";		break;
					case "golisopod":	megaType = "golisopite"; randomPokemon.type2 = Type.Steel;		break;
					case "magearna":	megaType = "magearnite";	randomPokemon.moreInfo = "heavy";		break;
					case "meowstic":	megaType = "meowsticite";		break;
					case "baxcalibur":	megaType = "baxcalibrite";		break;
					case "tatsugiri":	megaType = "tatsugirinite";		break;
					case "glimmora":	megaType = "glimmoranite";		break;
					case "scovillain":	megaType = "scovillainite";		break;
					case "groudon":	megaType = "red orb";  randomPokemon.type2 = Type.Fire; replaceAbility = "Desolate Land";		break;
					case "kyogre":	megaType = "blue orb"; replaceAbility = "Primordial Sea";		break;
					case "rayquaza": 
						if(Probability.Roll(25))
						{
							randomPokemon.name = "Rayquaza-Mega";
							banMegas = true;								
						}
						break;
				}
			}

			//	Chooses forme of a pokemon to use, or keep original, changes some parameters when needed.
			switch(randomPokemon.name)
			{
				case "Tauros-Paldea-Combat":
					if(Probability.Roll(70))
					{
						List<string> formes = new List<string> { "Tauros-Paldea-Aqua", "Tauros-Paldea-Blaze" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "Tauros-Paldea-Aqua":
								randomPokemon.type2 = Type.Water;
							break;
							case "Tauros-Paldea-Blaze":
								randomPokemon.type2 = Type.Fire;
							break;
						}
					}
				break;
				case "castform":
					if(Probability.Roll(75))
					{
						List<string> formes = new List<string> { "Castform-Rainy", "Castform-Snowy", "Castform-Sunny" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "Castform-Rainy":
								randomPokemon.type1 = Type.Water;
							break;
							case "Castform-Snowy":
								randomPokemon.type1 = Type.Ice;
							break;
							case "Castform-Sunny":
								randomPokemon.type1 = Type.Fire;
							break;
						}
					}
				break;
				case "deoxys":
					if(Probability.Roll(75))
					{
						List<string> formes = new List<string> { "Deoxys-Attack", "Deoxys-Defense", "Deoxys-Speed" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "Deoxys-Attack":
								randomPokemon.stats = "mixed";
							break;
							case "Deoxys-Defense":
								randomPokemon.stats = "tank";
								randomPokemon.moreInfo = "none";
							break;
							case "Deoxys-Speed":
								randomPokemon.stats = "mixed";
							break;
						}
					}
				break;
				case "dialga":
					if(Probability.Roll(50))
						randomPokemon.name = "dialga-origin";
				break;
				case "palkia":
					if(Probability.Roll(50))
						randomPokemon.name = "palkia-origin";
				break;
				case "giratina":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "giratina-origin";
						randomPokemon.stats = "mixed";
					}
				break;
				case "shaymin":
					if (Probability.Roll(50))
					{
						randomPokemon.name = "shaymin-sky";
						randomPokemon.stats = "spec";
						randomPokemon.moreInfo = "fast";
					}
				break;
				case "arceus":
					if(Probability.Roll(94))
					{
						List<string> formes = new List<string> {"arceus-fighting", "arceus-flying", "arceus-poison", "arceus-ground", "arceus-rock", "arceus-bug", "arceus-ghost", "arceus-steel", "arceus-fire",
			"arceus-water", "arceus-grass", "arceus-electric", "arceus-psychic", "arceus-ice", "arceus-dragon", "arceus-dark", "arceus-fairy" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "arceus-fighting":
								randomPokemon.type1 = Type.Fighting;
							break;
							case "arceus-flying":
								randomPokemon.type1 = Type.Flying;
							break;
							case "arceus-poison":
								randomPokemon.type1 = Type.Poison;
							break;
							case "arceus-ground":
								randomPokemon.type1 = Type.Ground;
							break;
							case "arceus-rock":
								randomPokemon.type1 = Type.Rock;
							break;
							case "arceus-bug":
								randomPokemon.type1 = Type.Bug;
							break;
							case "arceus-ghost":
								randomPokemon.type1 = Type.Ghost;
							break;
							case "arceus-steel":
								randomPokemon.type1 = Type.Steel;
							break;
							case "arceus-fire":
								randomPokemon.type1 = Type.Fire;
							break;
							case "arceus-water":
								randomPokemon.type1 = Type.Water;
							break;
							case "arceus-grass":
								randomPokemon.type1 = Type.Grass;
							break;
							case "arceus-electric":
								randomPokemon.type1 = Type.Electric;
							break;
							case "arceus-psychic":
								randomPokemon.type1 = Type.Psychic;
							break;
							case "arceus-ice":
								randomPokemon.type1 = Type.Ice;
							break;
							case "arceus-dragon":
								randomPokemon.type1 = Type.Dragon;
							break;
							case "arceus-dark":
								randomPokemon.type1 = Type.Dark;
							break;
							case "arceus-fairy":
								randomPokemon.type1 = Type.Fairy;
							break;
						}
					}
				break;
				case "silvally":
					if(Probability.Roll(94))
					{
						List<string> formes = new List<string> {"silvally-fighting", "silvally-flying", "silvally-poison", "silvally-ground", "silvally-rock", "silvally-bug", "silvally-ghost", "silvally-steel", "silvally-fire",
			"silvally-water", "silvally-grass", "silvally-electric", "silvally-psychic", "silvally-ice", "silvally-dragon", "silvally-dark", "silvally-fairy" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "silvally-fighting":
								randomPokemon.type1 = Type.Fighting;
							break;
							case "silvally-flying":
								randomPokemon.type1 = Type.Flying;
							break;
							case "silvally-poison":
								randomPokemon.type1 = Type.Poison;
							break;
							case "silvally-ground":
								randomPokemon.type1 = Type.Ground;
							break;
							case "silvally-rock":
								randomPokemon.type1 = Type.Rock;
							break;
							case "silvally-bug":
								randomPokemon.type1 = Type.Bug;
							break;
							case "silvally-ghost":
								randomPokemon.type1 = Type.Ghost;
							break;
							case "silvally-steel":
								randomPokemon.type1 = Type.Steel;
							break;
							case "silvally-fire":
								randomPokemon.type1 = Type.Fire;
							break;
							case "silvally-water":
								randomPokemon.type1 = Type.Water;
							break;
							case "silvally-grass":
								randomPokemon.type1 = Type.Grass;
							break;
							case "silvally-electric":
								randomPokemon.type1 = Type.Electric;
							break;
							case "silvally-psychic":
								randomPokemon.type1 = Type.Psychic;
							break;
							case "silvally-ice":
								randomPokemon.type1 = Type.Ice;
							break;
							case "silvally-dragon":
								randomPokemon.type1 = Type.Dragon;
							break;
							case "silvally-dark":
								randomPokemon.type1 = Type.Dark;
							break;
							case "silvally-fairy":
								randomPokemon.type1 = Type.Fairy;
							break;
						}
					}
				break;
				case "darmanitan":
					if (Probability.Roll(50))
					{
						randomPokemon.name = "darmanitan-zen";
						randomPokemon.type2	= Type.Psychic;
						randomPokemon.stats	= "spec";
					}
				break;
				case "darmanitan-galar":
					if (Probability.Roll(50))
					{
						randomPokemon.name = "darmanitan-galar-zen";
						randomPokemon.type2	= Type.Fire;
						randomPokemon.moreInfo = "fast";
					}
				break;
				case "tornadus":
					if(Probability.Roll(50))
						randomPokemon.name = "tornadus-therian";
				break;
				case "thundurus":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "thundurus-therian";
						randomPokemon.stats = "spec";
					}
				break;
				case "landorus":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "landorus-therian";
						randomPokemon.stats = "phys";
					}
				break;
				case "enamorus":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "enamorus-therian";
						randomPokemon.stats = "any";
						randomPokemon.moreInfo = "slow";
					}
				break;
				case "kyurem":
					if(Probability.Roll(67))
					{
						if(!restrictMons || (includeMonBST[0] <= 700 && 700 <= includeMonBST[1]))
						{
							List<string> formes = new List<string> {"kyurem-black", "kyurem-white" };
							randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
							switch(randomPokemon.name)
							{
								case "kyurem-black":
									randomPokemon.stats = "phys";
								break;
								case "kyurem-white":
									randomPokemon.stats = "spec";
								break;
							}
						}
					}
				break;
				case "meloetta":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "meloetta-pirouette";
						randomPokemon.type2 = Type.Fighting;
						randomPokemon.stats = "phys";
						randomPokemon.moreInfo = "fast";
					}
				break;
				case "aegislash":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "aegislash-blade";
						randomPokemon.stats = "mixed";
						randomPokemon.moreInfo = "frail";
					}
				break;
				case "rotom":
					if(Probability.Roll(85))
					{
						if(!restrictMons || (includeMonBST[0] <= 520 && 520 <= includeMonBST[1]))
						{
							List<string> formes = new List<string> {"Rotom-Fan", "Rotom-Frost", "Rotom-Heat", "Rotom-Mow", "Rotom-Wash" };
							randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
							switch(randomPokemon.name)
							{
								case "Rotom-Fan":
									randomPokemon.type2 = Type.Flying;
								break;
								case "Rotom-Frost":
									randomPokemon.type2 = Type.Ice;
								break;
								case "Rotom-Heat":
									randomPokemon.type2 = Type.Fire;
								break;
								case "Rotom-Mow":
									randomPokemon.type2 = Type.Grass;
								break;
								case "Rotom-Wash":
									randomPokemon.type2 = Type.Water;
								break;
							}
						}
					}
				break;
				case "gourgeist":
					if(Probability.Roll(75))
					{
						List<string> formes = new List<string> {"gourgeist-large", "gourgeist-small", "gourgeist-super" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
					}
				break;
				case "wormadam":
					if(Probability.Roll(67))
					{
						List<string> formes = new List<string> {"Wormadam-Sandy", "Wormadam-Trash" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "Wormadam-Sandy":
								randomPokemon.type2 = Type.Ground;
							break;
							case "Wormadam-Trash":
								randomPokemon.type2 = Type.Steel;
							break;
						}
					}
				break;
				case "oricorio":
					if(Probability.Roll(75))
					{
						List<string> formes = new List<string> {"Oricorio-Pa'u", "Oricorio-Pom-Pom", "Oricorio-Sensu" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "Oricorio-Pa'u":
								randomPokemon.type1 = Type.Psychic;
							break;
							case "Oricorio-Pom-Pom":
								randomPokemon.type1 = Type.Electric;
							break;
							case "Oricorio-Sensu":
								randomPokemon.type1 = Type.Ghost;
							break;
						}
					}
				break;
				case "lycanroc":
					if(Probability.Roll(67))
					{
						List<string> formes = new List<string> {"Lycanroc-Dusk", "Lycanroc-Midnight" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
					}
				break;
				case "wishiwashi":
					if(Probability.Roll(80) && (!restrictMons || (includeMonBST[0] <= 620 && 620 <= includeMonBST[1])))
						randomPokemon.name = "Wishiwashi-School";
						randomPokemon.moreInfo = "slow";
				break;
				case "necrozma":
					if(Probability.Roll(75))
					{
						List<string> formes = new List<string> {};
						if(!restrictMons || (includeMonBST[0] <= 680 && 680 <= includeMonBST[1]))
						{
							formes.Add("Necrozma-Dawn-Wings");
							formes.Add("Necrozma-Dusk-Mane");
						}
						if(!restrictMons || (includeMonBST[0] <= 754 && 754 <= includeMonBST[1]))
						{
							formes.Add("Necrozma-Ultra");
						}
						if(formes.Count > 0)
						{
							randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
							switch(randomPokemon.name)
							{
								case "Necrozma-Dawn-Wings":
									randomPokemon.type2 = Type.Ghost;
									randomPokemon.stats = "spec";
								break;
								case "Necrozma-Dusk-Mane":
									randomPokemon.type2 = Type.Steel;
									randomPokemon.stats = "phys";
								break;
								case "Necrozma-Ultra":
									randomPokemon.type2 = Type.Dragon;
									randomPokemon.moreInfo = "fast";
								break;
							}
						}
					}
				break;
				case "eiscue":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "eiscue-noice";
						randomPokemon.stats = "phys";
						randomPokemon.moreInfo = "fast";
					}
				break;
				case "indeedee":
					if(Probability.Roll(50))
						randomPokemon.name = "indeedee-f";
				break;
				case "oinkologne":
					if(Probability.Roll(50))
						randomPokemon.name = "oinkologne-f";
				break;
				case "zacian":
					if(Probability.Roll(50))
					{
						if(!restrictMons || (includeMonBST[0] <= 700 && 700 <= includeMonBST[1]))
						{
							randomPokemon.name = "zacian-crowned";
							randomPokemon.type2 = Type.Steel;
						}
					}
				break;
				case "zamazenta":
					if(Probability.Roll(50))
					{
						if(!restrictMons || (includeMonBST[0] <= 700 && 700 <= includeMonBST[1]))
						{
							randomPokemon.name = "zamazenta-crowned";
							randomPokemon.type2 = Type.Steel;
						}
					}
				break;
				case "urshifu":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "Urshifu-Rapid-Strike";
						randomPokemon.type2 = Type.Water;
					}
				break;
				case "calyrex":
					if(Probability.Roll(70))
					{
						if(!restrictMons || (includeMonBST[0] <= 680 && 680 <= includeMonBST[1]))
						{
							List<string> formes = new List<string> { "Calyrex-Ice", "Calyrex-Shadow" };
							randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
							switch(randomPokemon.name)
							{
								case "Calyrex-Ice":
									randomPokemon.type2 = Type.Ice;
									randomPokemon.stats = "phys";
									randomPokemon.moreInfo = "heavy";
								break;
								case "Calyrex-Shadow":
									randomPokemon.type2 = Type.Ghost;
									randomPokemon.stats = "spec";
									randomPokemon.moreInfo = "fast";
								break;
							}
						}
					}
				break;
				case "ursaluna":
					if(Probability.Roll(50))
					{
						if(!restrictMons || (includeMonBST[0] <= 555 && 555 <= includeMonBST[1]))
						{
							randomPokemon.name = "ursaluna-bloodmoon";
							randomPokemon.stats = "spec";
							randomPokemon.moreInfo = "heavy";
						}
					}
				break;
				case "basculegion":
					if(Probability.Roll(50))
					{
						randomPokemon.name = "basculegion-f";
						randomPokemon.stats = "mixed";
					}
				break;
				case "palafin":
					if(!restrictMons || (Probability.Roll(60) && includeMonBST[0] <= 650 && 650 <= includeMonBST[1]))
						randomPokemon.name = "palafin-hero";
				break;
				case "ogerpon":
					if(Probability.Roll(75))
					{
						List<string> formes = new List<string> { "Ogerpon-Cornerstone", "Ogerpon-Hearthflame", "Ogerpon-Wellspring" };
						randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
						switch(randomPokemon.name)
						{
							case "Ogerpon-Cornerstone":
								randomPokemon.type2 = Type.Rock;
							break;
							case "Ogerpon-Hearthflame":
								randomPokemon.type2 = Type.Fire;
							break;
							case "Ogerpon-Wellspring":
								randomPokemon.type2 = Type.Water;
							break;
						}
					}
				break;
				case "terapagos":
					if(Probability.Roll(70))
					{
						List<string> formes = new List<string> {};
						if(!restrictMons || (includeMonBST[0] <= 600 && 600 <= includeMonBST[1]))
							formes.Add("Terapagos-Terastal");
						if(!restrictMons || (includeMonBST[0] <= 700 && 700 <= includeMonBST[1]))
							formes.Add("Terapagos-Stellar");

						if(formes.Count > 0)
						{	
							randomPokemon.name = formes[Random.Shared.Next(formes.Count)];
							switch(randomPokemon.name)
							{
								case "Terapagos-Terastal":
									randomPokemon.stats = "any";
								break;
								case "Terapagos-Stellar":
									randomPokemon.stats = "spec";
								break;
							}
						}
					}
				break;
				
				default: break;
			}
			
			//	Rate calculation for every ability.
			foreach(Ability abil in abilities)
			{
				abil.rate = 100;
				if(abil.props.Contains("false"))
					abil.rate *= 0;

				//	Doubles and Triples abilities.
				if(notSingles && abil.props.Contains("doubles"))
					abil.rate = 150;

				if(restrictAbils)
				{
					bool genMatches = includeAbilGens.Any(gen => gen == abil.generation);
					if(!genMatches)
						abil.rate = 0;

					bool textMatches	= includeAbilName.Any(p => abil.name.StartsWith(p, StringComparison.OrdinalIgnoreCase));
					if(!textMatches)
						abil.rate = 0;
				}

				switch(randomPokemon.stats)
				{
					//	No special oriented abilities for physical attackers.
					case "phys":
						if(abil.stats == "special")
							abil.rate *= 0;
					break;
					//	No physical oriented abilities for special attackers.
					case "spec":
						if(abil.stats == "physical")
							abil.rate *= 0;
					break;
					//	Mixed attackers get a slight boost to offence abilities.
					case "mixed":
						if(abil.stats == "offence")
							abil.rate *= 1.2;
					break;
					//	Tanky pokemons have a slight boost on healing and defensive abilities.
					case "tank":
						if(abil.stats == "defence" || abil.stats == "heal")
							abil.rate *= 1.3;
					break;
					case "any":

					break;
				}

				if(randomPokemon.moreInfo == "fast" && abil.stats == "speed")
					abil.rate = 0;

				if(randomPokemon.moreInfo == "phys" && abil.stats == "special")
					abil.rate = 0;
				if(randomPokemon.moreInfo == "spec" && abil.stats == "physical")
					abil.rate = 0;

				//	Exclude abilities that could give Toxic Orb for Steel or Poison pokemon or Flame Orb for Fire pokemon.
				if(abil.props.Any(ab => ab.Contains("toxicorb")) && (randomPokemon.type1 == Type.Steel || randomPokemon.type1 == Type.Poison || randomPokemon.type2 == Type.Poison || randomPokemon.type2 == Type.Steel))
					abil.rate *= 0;
				if(abil.props.Any(ab => ab.Contains("flameorb")) && (randomPokemon.type1 == Type.Fire || randomPokemon.type2 == Type.Fire))
					abil.rate *= 0;

				if(abil.props.Contains("nowater") && (randomPokemon.type1 == Type.Water || randomPokemon.type2 == Type.Water))
					abil.rate = 0;
				if(abil.props.Contains("nofire") && (randomPokemon.type1 == Type.Fire || randomPokemon.type2 == Type.Fire))
					abil.rate = 0;
				if(abil.props.Contains("nodragon") && (randomPokemon.type1 == Type.Dragon || randomPokemon.type2 == Type.Dragon))
					abil.rate = 0;

				//	Give Shedinja only these abilities.
				if(randomPokemon.name == "shedinja" && abil.name != "Wonder Guard" && abil.name != "Sturdy")
					abil.rate = 0;
			}
			Ability tmpAbility		= WeightRandomSelect.SelectRandomWeighted(abilities, abil => abil.rate);
			Ability randomAbility	= new Ability(tmpAbility.name, tmpAbility.stats, tmpAbility.generation, tmpAbility.props, tmpAbility.rate);

			if (replaceAbility != "nothing")
				randomAbility = abilities.First(ab => ab.name == replaceAbility);

			// Assign rate for every item
			foreach(Item item in items)
			{
				item.rate = item.ogRate;
				if(item.props.Contains("false"))
					item.rate *= 0;

				switch(randomPokemon.stats)
				{
					// Remove special oriented items for physical attackers and viceversa.
					case "phys":
						if(item.stats == "special")
							item.rate *= 0;
					break;
					case "spec":
						if(item.stats == "physical")
							item.rate *= 0;
					break;
					//	Slightly boost rates of offence items for mixed attackers.
					case "mixed":
						if(item.stats == "offence")
							item.rate *= 1.2;
					break;
					//	Slightly boost rates of defence items for tanky pokemons.
					case "tank":
						if(item.stats == "defence")
							item.rate *= 1.2;
					break;
					case "any":

					break;
				}
				switch (randomAbility.stats)
				{
					// Slightly boost rates of offence items for offence abilities.
					case "offence":
						if(item.stats == "offence")
							item.rate *= 1.2;
					break;
					// Removes special items for physical boosting abilities and viceversa.
					case "physical":
						if(item.stats == "special")
							item.rate *= 0;
					break;
					case "special":
						if(item.stats == "physical")
							item.rate *= 0;
					break;
					//	This just increases rate of Weakness Policy for abilities that make it easier to take a supereffective move.
					case "weakness":
						if(item.props.Contains("weakness"))
							item.rate *= 40;
					break;
					//	Abilities that boost speed don't need items that boost speed even more, so the rates are halved.
					case "speed":
						if(item.stats == "speed")
							item.rate *= 0.5;
					break;
					case "defence":

					break;
				}
				if(randomPokemon.moreInfo == "phys" && item.stats == "special")
					item.rate = 0;
				if(randomPokemon.moreInfo == "spec" && item.stats == "physical")
					item.rate = 0;
				//	Farfetch'd and evo can have Leek.
				if((randomPokemon.name == "farfetch'd" || randomPokemon.name == "sirfetch'd") && item.name == "Leek")
					item.rate = 3000;
				//	Marowak can have Thick Club.
				if((randomPokemon.name == "marowak" || randomPokemon.name == "marowak-alola") && item.name == "Thick Club")
					item.rate = 8000;
				//	Ditto can have Metal Powder or Quick Powder.
				if(randomPokemon.name == "ditto" && (item.name == "Metal Powder" || item.name == "Quick Powder"))
					item.rate = 4000;
				//	Pikachu can have Light Ball.
				if((randomPokemon.name == "pikachu" || randomPokemon.name == "pikachu-starter") && item.name == "Light Ball")
					item.rate = 48000;
				//	Increase Focus Sash for frail pokemons.
				if(randomPokemon.moreInfo == "frail" && item.name == "Focus Sash" && randomAbility.name != "Sturdy")
					item.rate *= 100;
				//	Remove Focus Sash on tanky pokemon.
				if((randomPokemon.stats == "tank" || randomAbility.name == "Sturdy") && item.name == "Focus Sash")
					item.rate = 0;
				if(randomPokemon.name == "shedinja")
				{
					if(randomAbility.name == "Sturdy")
					{
						if(item.name != "Heavy-Duty Boots" && item.name != "Choice Band" && item.name != "Choice Scarf" && item.name != "Ability Shield" && 
							item.name != "Air Balloon" && item.name != "Quick Claw" && item.name != "Safety Goggles")
							item.rate = 0;
					}
					else
					{
						if(item.name != "Heavy-Duty Boots" && item.name != "Choice Band" && item.name != "Choice Scarf" && item.name != "Ability Shield" && 
							item.name != "Air Balloon" && item.name != "Quick Claw" && item.name != "Safety Goggles" && item.name != "Focus Sash")
							item.rate = 0;
					}
				}
				//	Apply items that require tanking only to tank pokemon.
				if(randomPokemon.stats != "tank" && item.props.Contains("tank"))
					item.rate = 0;
				//	Some abilities are better when not switching, no Eject items for them.
				if(randomAbility.props.Contains("stay") && (item.name == "Eject Pack" || item.name == "Eject Button"))
					item.rate = 0;
				//	Removes Air Balloon for Flying and Levitate pokemon.
				if((randomPokemon.type1 == Type.Flying || randomPokemon.type2 == Type.Flying || randomAbility.name == "Levitate") && item.name == "Air Balloon")
					item.rate *= 0;
				//	Remove speed items on slow pokemon.
				if(randomPokemon.moreInfo == "slow" && item.stats == "speed" && item.name != "Quick Claw" && item.name != "Custap Berry")
					item.rate = 0;
				//	Removes items that increase stats for abilities that don't want that (Contrary), also Z crystals might increase stats.
				if(randomAbility.props.Contains("nostatraise") && (item.props.Contains("statraise") || item.props.Contains("crystal")))
					item.rate *= 0;
				//	Removes non-berry items from abilities that work with berries.
				if(randomAbility.props.Contains("berry") && !item.props.Contains("berry"))
					item.rate *= 0;
				//	Some abilities can have Damp Rock or Heat Rock at specified rate.
				if(randomAbility.props.Any(ab => ab.Contains("damprock")) && item.name == "Damp Rock")
				{
					string match = randomAbility.props.First(ab => ab.Contains("damprock"));
					item.rate = Convert.ToDouble(match.Substring(match.IndexOf("damprock") + "damprock".Length)) * 100;
				}
				if(randomAbility.props.Any(ab => ab.Contains("heatrock")) && item.name == "Heat Rock")
				{
					string match = randomAbility.props.First(ab => ab.Contains("heatrock"));
					item.rate = Convert.ToDouble(match.Substring(match.IndexOf("heatrock") + "heatrock".Length)) * 100;
				}
				//	Eviolite rate for non-fully evolved pokemon.
				if(!randomPokemon.fullevo && item.props.Contains("notevo"))
				{
					item.rate = 15000;
				}
				//	A terrain ability enables the chance for seed to appear at a set rate.
				if(randomAbility.props.Any(ab => ab.Contains("seed")) && item.props.Any(ab => ab.Contains("seed")) && randomPokemon.moreInfo != "frail")
				{
					string match = randomAbility.props.First(ab => ab.Contains("seed"));
					currentTerrain = match.Substring(match.IndexOf("seed") + "seed".Length);
					string itemMatch = item.props.First(ab => ab.Contains("seed"));
					string itemTerrain = itemMatch.Substring(itemMatch.IndexOf("seed") + "seed".Length);
					if(itemTerrain == currentTerrain)
						item.rate = 1000;
				}
				//	This happens even after a pokemon with terrain ability. Sets the rate for a seed of that terrain.
				if(item.props.Any(ab => ab.Contains("seed")) && currentTerrain != "none" && !randomAbility.props.Contains("berry") && !randomAbility.props.Contains("nostatraise")  && randomPokemon.moreInfo != "frail")
				{
					string itemMatch = item.props.First(ab => ab.Contains("seed"));
					string itemTerrain = itemMatch.Substring(itemMatch.IndexOf("seed") + "seed".Length);
					if(itemTerrain == currentTerrain)
						item.rate = 1000;
				}
				//	Same logic as Damp Rock and Heat Rock, for Flame Orb and Toxic Orb instead.
				if(randomAbility.props.Any(ab => ab.Contains("flameorb")) && item.name == "Flame Orb")
				{
					string match = randomAbility.props.First(ab => ab.Contains("flameorb"));
					item.rate = Convert.ToDouble(match.Substring(match.IndexOf("flameorb") + "flameorb".Length)) * 100;
				}
				if(randomAbility.props.Any(ab => ab.Contains("toxicorb")) && item.name == "Toxic Orb")
				{
					string match = randomAbility.props.First(ab => ab.Contains("toxicorb"));
					item.rate = Convert.ToDouble(match.Substring(match.IndexOf("toxicorb") + "toxicorb".Length)) * 100;
				}
				//	Some abilities boost Life Orb rate, since this item can be present with other abilities it multiplies instead of being set.
				if(randomAbility.props.Any(ab => ab.Contains("lifeorb")) && item.name == "Life Orb")
				{
					string match = randomAbility.props.First(ab => ab.Contains("lifeorb"));
					item.rate *= Convert.ToDouble(match.Substring(match.IndexOf("lifeorb") + "lifeorb".Length));
				}
				//	Gorilla Tactics forces either Choice Band or Choice Scarf, Imposter forces speed boosting items.
				if (randomAbility.props.Contains("gorilla"))
				{
					if(item.props.Contains("choiced") && item.stats != "special")
						item.rate *= 50;
					else
						item.rate *= 0;
				}
				if (randomAbility.props.Contains("imposter"))
				{
					if(item.stats == "speed" && (item.props.Contains("choiced") || item.props.Contains("berry")))
						item.rate *= 50;
					else
						item.rate *= 0;
				}
				//	Punching abilities boost punching items, non-punching abilities nerf the rate.
				if(randomAbility.props.Contains("punching") && item.props.Contains("punching"))
					item.rate *= 40;
				//	Timewarping abilities enable Booster Energy to appear.
				if(randomAbility.props.Contains("booster") && item.props.Contains("booster"))
					item.rate = 8000;
				//	Sound abilities boost sound items (Throat Spray).
				if(randomAbility.props.Contains("sound") && item.props.Contains("sound"))
					item.rate *= 25;
				//	This is specifically for Simple, increases items that boost stats and halves others. Works with any "balance" ability with "ownstatraise".
				if(randomAbility.props.Contains("ownstatraise") && randomAbility.stats == "balance")
				{
					if(item.props.Contains("statraise"))
						item.rate *= 25;
					else
						item.rate *= 0.5;
				}
				//	Crit abilities and items boost.
				if(randomAbility.props.Contains("crit") && item.props.Contains("crit"))
					item.rate *= 10;
				//	Some abilities want only items that are used quickly, so remove the others.
				if(randomAbility.props.Contains("quickuse") && !item.props.Contains("quickuse"))
					item.rate *= 0;
				//	Magic Guard boosts Life Orb and enables Sticky Barb at set rate.
				if(randomAbility.props.Contains("magicguard"))
				{
					if(item.name == "Life Orb")
						item.rate *= 50;
					if(item.name == "Sticky Barb")
						item.rate = 5000;
				}
				//	Zoom Lens for slow pokemon only.
				if(item.name == "Zoom Lens" && randomPokemon.moreInfo != "slow")
					item.rate = 0;
				//	No Custap for fast pokemon.
				if((item.name == "Custap Berry" || item.name == "Quick Claw") && randomPokemon.moreInfo == "fast")
					item.rate = 0;
				//	Removes water related items for abilities that don't want water. Same with fire
				if(randomAbility.props.Contains("nowater") && item.props.Contains("water"))
					item.rate *= 0;
				if(randomAbility.props.Contains("nofire") && item.props.Contains("fire"))
					item.rate *= 0;
				if(randomAbility.props.Contains("nodragon") && item.props.Contains("dragon"))
					item.rate *= 0;
				//	Remove Loaded Dice from Skill Link.
				if(randomAbility.props.Contains("multihit") && item.props.Contains("multihit"))
					item.rate = 0;
				//	Remove Assault Vest from Prankster and Mycelium.
				if(randomAbility.props.Contains("status") && item.props.Contains("assault"))
					item.rate = 0;

				//	Heavy-Duty Boots boost for types weak to rock.
				if(item.name == "Heavy-Duty Boots")
				{
					switch (randomPokemon.type1)
					{
						case Type.Bug:
						case Type.Flying:
						case Type.Fire:
						case Type.Ice: item.rate *= 4; break;
					}
					switch (randomPokemon.type2)
					{
						case Type.Bug:
						case Type.Flying:
						case Type.Fire:
						case Type.Ice: item.rate *= 4; break;
					}
				}
				//	Black Sludge only for poison.
				if(item.name == "Black Sludge" && randomPokemon.type1 != Type.Poison && randomPokemon.type2 != Type.Poison)
					item.rate = 0;

				if(banZcrystals && item.props.Contains("crystal"))
					item.rate *= 0;
			}
			Item tmpItem	= WeightRandomSelect.SelectRandomWeighted(items, item => item.rate);
			Item randomItem	= new Item(tmpItem.name, tmpItem.stats, tmpItem.props, tmpItem.rate);

			/* double perc = randomItem.rate / items.Sum(item => item.rate) * 100;
			double avrg = randomItem.rate / (items.Sum(item => item.rate) / items.Count);
			Item maxItem	= items.MaxBy(i => i.rate);
			double perc2 = maxItem.rate / items.Sum(item => item.rate) * 100;
			double avrg2 = maxItem.rate / (items.Sum(item => item.rate) / items.Count);
			Console.WriteLine("Chosen: " + randomItem.name + " " + Math.Round(perc, 2) + " " + Math.Round(avrg, 2) + " VS "
			+ maxItem.name + " " + Math.Round(perc2, 2) + " " + Math.Round(avrg2, 2)); */

			//	Overrides selected item to the mega stone if chosen earlier.
			if(megaType != "nothing")
			{
				banMegas = true;
				randomItem = new Item(megaType, "balance", new List<string> {"true"}, 100);
			}
			//	Removes the chance of obtaining ulterior Z crystals.
			if(randomItem.props.Contains("crystal") && !banZcrystals)
				banZcrystals = true;

			List<Move> pickedMoves		= new List<Move>{};
			List<string> randomMoves	= new List<string>{};
			bool zMoveFound				= false;
			/* bool abilMoveFound			= false; */
			bool itemMoveFound			= false;
			bool chargingMoveUsed		= false;
			int clayRate				= 0;
			for(int j = 0; j < 4; j++)
			{
				foreach(Move move in moves)
				{
					move.rate = 100;
					bool isUsed = false;

					//	Change move types.
					if (randomItem.props.Contains("plate") && move.name == "Judgment")
					{
						switch (randomItem.name)
						{
							case "Draco Plate":	move.type = Type.Dragon;	break;
							case "Dread Plate":	move.type = Type.Dark;	break;
							case "Earth Plate":	move.type = Type.Ground;	break;
							case "Fist Plate":	move.type = Type.Fighting;	break;
							case "Flame Plate":	move.type = Type.Fire;	break;
							case "Icicle Plate":	move.type = Type.Ice;	break;
							case "Insect Plate":	move.type = Type.Bug;	break;
							case "Iron Plate":	move.type = Type.Steel;	break;
							case "Meadow Plate":	move.type = Type.Grass;	break;
							case "Mind Plate":	move.type = Type.Psychic;	break;
							case "Pixie Plate":	move.type = Type.Fairy;	break;
							case "Sky Plate":	move.type = Type.Flying;	break;
							case "Splash Plate":	move.type = Type.Water;	break;
							case "Spooky Plate":	move.type = Type.Ghost;	break;
							case "Stone Plate":	move.type = Type.Rock;	break;
							case "Toxic Plate":	move.type = Type.Poison;	break;
							case "Zap Plate":	move.type = Type.Electric;	break;
							default:	move.type = Type.Normal;	break;
						}
					}
					if(move.name == "Revelation Dance")
						move.type = randomPokemon.type1;

					if (restrictMoves)
					{
						bool typeMatches = includeMoveTypes.Any(type => type == move.type);
						if(!typeMatches)
							move.rate = 0;

						bool genMatches = includeMoveGens.Any(gen => gen == move.generation);
						if(!genMatches)
							move.rate = 0;

						bool textMatches	= includeMoveName.Any(p => move.name.StartsWith(p, StringComparison.OrdinalIgnoreCase));
						if(!textMatches)
							move.rate = 0;
					}
					
					//	Removes the move if already chosen.
					if(pickedMoves.Any(m => m.name == move.name))
						move.rate *= 0;

					//	Doubles and Triples.
					if(notSingles && move.props.Contains("doubles"))
					{
						isUsed = true;
						move.rate *= 3;
					}
					if(!notSingles && move.props.Contains("doubles"))
						move.rate *= 0.33;
					
					//	Heavily reduces the chance of an offensive move to be chosen when it's typing was already chosen.
					if(move.category == 0 || move.category == 1)
					{
						if(pickedMoves.Any(picked => (picked.category == 0 || picked.category == 1) && picked.type == move.type))
							move.rate *= 0.025;
					}

					//	Early move rates based on pokemon.
					//	"direct" moves mainly deal direct damage or use a stat that isn't the pokemon ATK or SPA.
					switch(randomPokemon.stats)
					{
						case "phys":
							switch (move.category)
							{
								case 0:
									move.rate *= 2.5;
								break;
								case 1:
									if(move.props.Contains("category"))
										move.rate *= 2.5;
									else
										move.rate *= 0;
								break;
								case 2:
									move.rate *= 1.5;
								break;
							}
							if(move.props.Contains("physical"))
								move.rate *= 2;
							if(move.props.Contains("special"))
								move.rate *= 0;
							if(move.props.Contains("offence"))
								move.rate *= 1.2;
							if(move.props.Contains("speed"))
								move.rate *= 1.5;
							if(move.props.Contains("defence") && !move.props.Contains("offence") && !move.props.Contains("physical") && !move.props.Contains("special"))
								move.rate *= 0.25;
							if(move.props.Contains("direct"))
								move.rate *= 0;
						break;
						case "spec":
							switch (move.category)
							{
								case 0:
									move.rate *= 0;
								break;
								case 1:
									move.rate *= 2.5;
								break;
								case 2:
									move.rate *= 1.5;
								break;
							}
							if(move.props.Contains("physical"))
								move.rate *= 0;
							if(move.props.Contains("special"))
								move.rate *= 2;
							if(move.props.Contains("offence"))
								move.rate *= 1.2;
							if(move.props.Contains("speed"))
								move.rate *= 1.5;
							if(move.props.Contains("defence") && !move.props.Contains("offence") && !move.props.Contains("physical") && !move.props.Contains("special"))
								move.rate *= 0.25;
							if(move.props.Contains("direct"))
								move.rate *= 0;
						break;
						case "mixed":
							switch (move.category)
							{
								case 0:
									move.rate *= 1.75;
								break;
								case 1:
									move.rate *= 1.75;
								break;
								case 2:
									move.rate *= 0.5;
								break;
							}
							if(move.props.Contains("physical"))
								move.rate *= 0.8;
							if(move.props.Contains("special"))
								move.rate *= 0.8;
							if(move.props.Contains("offence"))
								move.rate *= 2;
							if(move.props.Contains("speed"))
								move.rate *= 1.75;
							if(move.props.Contains("defence") && !move.props.Contains("offence") && !move.props.Contains("physical") && !move.props.Contains("special"))
								move.rate *= 0.25;
							if(move.props.Contains("direct"))
								move.rate *= 0;

							if(pickedMoves.Any(p => p.category == 0))
							{
								if(move.props.Contains("special"))
									move.rate = 0;
							}
							if(pickedMoves.Any(p => p.category == 1))
							{
								if(move.props.Contains("physical"))
									move.rate = 0;
							}
						break;
						case "tank":
							switch (move.category)
							{
								case 0:
									move.rate *= 0.9;
									if(randomPokemon.moreInfo == "spec")
										move.rate = 0;
								break;
								case 1:
									move.rate *= 0.9;
									if(randomPokemon.moreInfo == "phys" && !move.props.Contains("category"))
										move.rate = 0;
								break;
								case 2:
									move.rate *= 2.2;
								break;
							}
							if(move.props.Contains("tank"))
								move.rate *= 3;
							if(move.props.Contains("defence"))
								move.rate *= 1.5;
							if(move.props.Contains("triage"))
								move.rate *= 2;
							if(move.props.Contains("direct"))
								move.rate *= 1.25;
							if(randomPokemon.moreInfo == "phys" && move.props.Contains("special"))
								move.rate = 0;
							if(randomPokemon.moreInfo == "spec" && move.props.Contains("physical"))
								move.rate = 0;

							if(pickedMoves.Any(p => p.category == 0))
							{
								if(move.props.Contains("special"))
									move.rate = 0;
							}
							if(pickedMoves.Any(p => p.category == 1))
							{
								if(move.props.Contains("physical"))
									move.rate = 0;
							}
						break;
						case "any":
							if(pickedMoves.Any(p => p.category == 0))
							{
								if(move.props.Contains("special"))
									move.rate = 0;
							}
							if(pickedMoves.Any(p => p.category == 1))
							{
								if(move.props.Contains("physical"))
									move.rate = 0;
							}
						break;
					}
					
					//	Move rates based on ability.
					switch(randomAbility.stats)
					{
						case "offence":
							switch (move.category)
							{
								case 0:
									move.rate *= 2;
								break;
								case 1:
									move.rate *= 2;
								break;
								case 2:
									move.rate *= 0.8;
								break;
							}
						break;
						//	Speed boosting abilities necessitate less speed boosting moves.
						case "speed":
							if(move.props.Contains("speed"))
								move.rate *= 0.25;
						break;
						case "physical":
							switch (move.category)
							{
								case 0:
									move.rate *= 2;
								break;
								case 1:
									move.rate *= 0;
								break;
								case 2:
									move.rate *= 0.8;
								break;
							}
						break;
						case "special":
							switch (move.category)
							{
								case 0:
									move.rate *= 0;
								break;
								case 1:
									move.rate *= 2;
								break;
								case 2:
									move.rate *= 0.8;
								break;
							}
						break;
						//	Healing abilities use less healing moves.
						case "heal":
							if(move.props.Contains("triage"))
								move.rate *= 0.5;
						break;
						//	Status inflicting abilities use less status inflicting moves.
						case "status":
							if(move.props.Contains("status"))
								move.rate *= 0.25;
						break;
					}

					//	Only speed boosting items reduce the rate of speed boosting moves.
					switch(randomItem.stats)
					{
						case "speed":
							if(move.props.Contains("speed"))
								move.rate *= 0.8;
						break;
					}

					//	Increase rates for STAB moves, which increases even more with Adaptability.
					int isAdapt = 1;
					if(randomAbility.name == "Adaptability")
						isAdapt = 3;
					if(randomPokemon.type1 == move.type || randomPokemon.type2 == move.type)
						move.rate *= 40 * isAdapt;

					//	Guaranteed damaging move if the previously chosen 3 moves were status moves.
					if(!pickedMoves.Any(pick => pick.category == 0 || pick.category == 1) && j == 3)
					{
						if(move.category == 2)
							move.rate *= 0;
					}

					//	Reduce or Boost STATUS moves depending on how many there are for each type.
					/*	Normal 0.3 (As of 04/04/26) 48 Status
						Fighting 0.9
						Flying 1.0
						Poison 1.0
						Ground 1.3
						Rock 1.1
						Bug 0.8
						Ghost 1.3
						Steel 1.0
						Fire 1.2
						Water 1.3
						Grass 0.6
						Electric 1.2
						Psychic 0.3 20 Status
						Ice 1.1
						Dragon 1.3
						Dark 0.5
						Fairy 1.0 */
					if(move.category == 2 && (move.type == randomPokemon.type1 || move.type == randomPokemon.type2))
					{
						int nStatus = moves.Count(m => m.type == move.type && m.category == 2);
						move.rate *= Math.Max(0.3, 1.5 - (0.1 * nStatus));
					}

					//	Boost offensive damaging moves of a certain type needed by the ability.
					if (randomAbility.props.Contains("type"))
					{
						if(randomAbility.props.Any(p => p == move.type.ToString().ToLower()) && move.category != 2)
						{
							if(!pickedMoves.Any(picked => picked.type == move.type && picked.category != 2))
								move.rate *= 12 * (j + 1);
						}
					}

					//	Boost moves of a certain type based on the Z crystal, guaranteed on the 4th move.
					if(randomItem.props.Contains("crystal"))
					{
						if(randomItem.props.Any(p => p == move.type.ToString().ToLower()) && move.generation <= 7)
						{
							if(!pickedMoves.Any(picked => picked.type == move.type && !(picked.power <= 60 && picked.power >= 10)))
								move.rate *= 5 * (j + 1);
							else
								zMoveFound = true;
						}
						else
						{
							if(j == 3 && !zMoveFound)
								move.rate *= 0;
						}
					}
					else if(randomItem.props.Contains("plate"))
					{
						// Check if any props of item has same type as the move, and the move is offensive.
						if(randomItem.props.Any(p => p == move.type.ToString().ToLower()) && move.category != 2)
						{
							if(!itemMoveFound)
								move.rate *= 5 * (j + 1);
						}
						else if(j == 3 && !itemMoveFound && move.name != "Judgment")	//	Do not remove Judgment from the rates.
						{
							move.rate *= 0;
						}
					}

					//	Remove Trick and Switcheroo from mega stone holders.
					if(move.props.Contains("choiced") && megaType != "nothing")
						move.rate *= 0;

					//	Whole calculation for moves with accuracy below 80, if an item or ability grant enough accuracy, these moves are boosted.
					double accuracy = 0;
					if(randomAbility.props.Any(ab => ab.Contains("accuracy")))
					{
						string match = randomAbility.props.First(ab => ab.Contains("accuracy"));
						accuracy += Convert.ToDouble(match.Substring(match.IndexOf("accuracy") + "accuracy".Length));
					}
					if(randomItem.props.Any(ab => ab.Contains("accuracy")))
					{
						string match = randomItem.props.First(ab => ab.Contains("accuracy"));
						accuracy += Convert.ToDouble(match.Substring(match.IndexOf("accuracy") + "accuracy".Length));
					}
					if(accuracy > 1)	//	Relistically anything below 20 will do nothing.
					{
						if((move.accuracy >= 100 - accuracy && move.accuracy <= 80) || move.props.Contains("noguard")){
							move.rate *= accuracy / 2; isUsed = true;
						}
					}

					//	Probability moves handling.
					double prob = 0;
					if(move.props.Any(p => p.Contains("prob")))
					{
						string moveProb = move.props.First(p => p.Contains("prob"));
						prob = Convert.ToDouble(moveProb.Substring(moveProb.IndexOf("prob") + "prob".Length));
					}
					if(randomAbility.name == "Serene Grace")
					{
						if(prob > 0)
						{
							move.rate *= prob;
							if(Probability.Roll(Convert.ToInt32(Math.Floor(prob * 2))))
								isUsed = true;
						}
						else if(move.category != 2)
							move.rate = 0;
						else if(move.category == 2)
							move.rate *= 10;
							
					}
					if(randomAbility.name == "Sheer Force")
					{
						double boost = 50 - prob;
						if(prob > 0 && prob < 50 && move.power > 60)
							move.rate *= boost;
						else if(move.category != 2)
							move.rate = 0;
						else if(move.category == 2)
							move.rate *= 10;
					}

					//	Status moves and weak moves are removed for Choice items.
					if(randomItem.props.Contains("choiced"))
					{
						switch (randomItem.stats)
						{
							case "physical":
								if(move.category != 0 && !move.props.Contains("choiced"))
									move.rate *= 0;
								if(move.props.Contains("choiced"))
									move.rate *= 20;
								else if(move.power < 40)
									move.rate *= 0;
							break;
							case "special":
								if(move.category != 1 && !move.props.Contains("choiced"))
									move.rate *= 0;
								if(move.props.Contains("choiced"))
									move.rate *= 20;
								else if(move.power < 40 && move.type != Type.Water)
									move.rate *= 0;
							break;
							case "speed":
								if(move.category == 2 && !move.props.Contains("choiced"))
									move.rate *= 0;
								if(move.props.Contains("choiced"))
									move.rate *= 20;
								else if(move.power <= 60 && move.type != Type.Steel)
									move.rate *= 0;
								if(move.props.Contains("negprio") || move.props.Contains("posprio") || move.name == "Gyro Ball")
									move.rate *= 0;
							break;
						}
					}

					//	Trick and Switcheroo only for Choice items or Black Sludge
					if(move.props.Contains("choiced") && !randomItem.props.Contains("choiced") && randomItem.name != "Black Sludge")
						move.rate = 0;
					if(move.props.Contains("choiced") && randomItem.name == "Black Sludge")
						move.rate *= 20;
					if(pickedMoves.Any(p => p.props.Contains("choiced")) && move.props.Contains("choiced"))
						move.rate = 0;
					

					//	Handling for speed.
					if(move.name == "Electro Ball")
					{
						if(randomPokemon.moreInfo == "fast")
							move.rate *= 2;
						else
							move.rate *= 0;
					}
					if(move.name == "Gyro Ball")
					{
						if(randomPokemon.moreInfo == "fast")
							move.rate *= 0;
						if(randomPokemon.moreInfo == "slow")
							move.rate *= 3;
						else if(randomPokemon.stats == "tank" && randomPokemon.moreInfo == "phys")
							move.rate *= 1.75;
						else
							move.rate *= 0;
					}
					if(move.name == "Me First" && randomPokemon.moreInfo != "fast")
						move.rate *= 0;
					if (move.props.Contains("posprio"))
					{
						if(randomPokemon.moreInfo == "fast")
							move.rate *= 0.5;
						if(randomPokemon.moreInfo == "slow")
							move.rate *= 2;
					}
					if (move.props.Contains("negprio"))
					{
						if(randomPokemon.moreInfo == "fast")
							move.rate *= 0;
					}
					if(randomPokemon.moreInfo == "slow" && move.name == "Curse")
						move.rate *= 5;

					//	Handling for fatasses.
					if(move.name == "Heat Crash" || move.name == "Heavy Slam")
					{
						if(randomPokemon.moreInfo == "heavy")
							move.rate *= 2;
						else
							move.rate = 0;
					}

					//	Disable switching moves on pokemons that have an ability that doesn't like switching.
					if(randomAbility.props.Contains("stay") && move.props.Contains("switch"))
					{
						if(move.name != "Baton Pass")
							move.rate = 0;
					}

					//	Aurora Veil rate.
					if(move.name == "Aurora Veil" || move.name == "Blizzard")
					{
						if(randomAbility.name == "Snow Warning" || pickedMoves.Any(m => m.name == "Snowscape") || chosenAbilities.Any(abil => abil == "Snow Warning") || chosenMoves.Any(moves => moves.Any(move => move == "Chilly Reception" || move == "Snowscape")))
							move.rate *= 5;
						else
						{
							if(move.name == "Aurora Veil")
								move.rate = 0;
							if(move.name == "Blizzard")
								move.rate *= 0.25;
						}
					}

					//	Double Shock and Burn Up only for electric and fire types.
					if(move.name == "Double Shock" && randomPokemon.type1 != Type.Electric && randomPokemon.type2 != Type.Electric)
						move.rate = 0;
					if(move.name == "Burn Up" && randomPokemon.type1 != Type.Fire && randomPokemon.type2 != Type.Fire)
						move.rate = 0;

					//	Rest Sleep Talk strategies.
					if(move.name == "Rest" || move.name == "Sleep Talk")
					{
						if(pickedMoves.Any(move => move.name == "Rest" || move.name == "Sleep Talk"))
							move.rate *= 400;
						else if(j > 2)
							move.rate = 0;
					}
					if(pickedMoves.Any(move => move.name == "Rest" || move.name == "Sleep Talk"))
					{
						if(move.props.Contains("triage") && move.name != "Rest")
							move.rate *= 0;
					}

					//	Remove more than one setup move of same category.
					if(pickedMoves.Any(m => m.props.Contains("offence") || m.props.Contains("special") || m.props.Contains("physical")) && (move.props.Contains("offence") || move.props.Contains("special") || move.props.Contains("physical")))
						move.rate = 0;
					if(pickedMoves.Any(m => m.props.Contains("defence")) && move.props.Contains("defence"))
						move.rate = 0;
					if(pickedMoves.Any(m => m.props.Contains("speed")) && move.props.Contains("speed"))
						move.rate = 0;

					if(pickedMoves.Any(m => m.props.Contains("physical")) && move.category == 1)
						move.rate = 0;
					if(pickedMoves.Any(m => m.props.Contains("special")) && move.category == 0)
						move.rate = 0;

					//	This is mainly for Contrary.
					if(randomAbility.props.Contains("ownstatlower"))
					{
						if(move.props.Contains("ownstatlower")){
							move.rate *= 20; isUsed = true;
						}
						if(move.props.Contains("ownstatraise") || move.props.Contains("defence") || move.props.Contains("offence") || move.props.Contains("physical") || move.props.Contains("special") || move.props.Contains("speed"))
							move.rate *= 0;
					}
					//	Items that want the pokemon to lower its own stats to be used.
					if(randomItem.props.Contains("ownstatlower"))
					{
						if(move.props.Contains("ownstatlower")){
							move.rate *= 6; isUsed = true;
						}
					}
					//	Only physical moves for Gorilla Tactics.
					if(randomAbility.props.Contains("gorilla") && move.category != 0)
						move.rate *= 0;
					//	Status boosting abilities.
					if(randomAbility.props.Contains("status") && move.category == 2)
						move.rate *= 15;
					//	Punching abilities and items.
					if((randomAbility.props.Contains("punching") || randomItem.props.Contains("punching")) && move.props.Contains("punching")){
						move.rate *= 80; isUsed = true;}
					//	Punk Rock, Liquid Voice, Throat Spray
					if(randomAbility.props.Contains("sound") && randomAbility.stats == "special" && move.props.Contains("sound") && move.category != 2){
						move.rate *= 80; isUsed = true;}
					if(randomAbility.props.Contains("sound") && randomAbility.stats == "balance" && move.props.Contains("sound") && move.category != 2){
						move.rate *= 10; isUsed = true;}
					if(randomItem.props.Contains("sound") && move.props.Contains("sound"))
					{
						move.rate *= 80; isUsed = true;
					}
					//	Magic Guard boosted moves.
					if(randomAbility.props.Contains("magicguard") && (move.props.Contains("recoil") || move.props.Contains("magicguard"))){
						move.rate *= 20; isUsed = true;}
					//	Recoil boost abilities.
					if(randomAbility.props.Contains("recoil") && move.props.Contains("recoil")){
						move.rate *= 20; isUsed = true;}
					if(randomAbility.props.Contains("switch") && move.props.Contains("switch")){
						move.rate *= 10; isUsed = true;}
					//	Stat raising moves boost for Parental Bond and Simple.
					if (randomAbility.props.Contains("ownstatraise")){
						if(move.props.Contains("ownstatraise")){	
							move.rate *= 20; isUsed = true;
						}
						if(randomAbility.stats == "balance")		//	Simple.
						{
							if(move.props.Contains("offence") || move.props.Contains("physical") || move.props.Contains("special") || move.props.Contains("speed") || move.props.Contains("defence"))
								{move.rate *= 10; isUsed = true;}
						}
						else										//	PB.
						{
							if(move.props.Contains("multihit") || move.props.Contains("noguard"))
								move.rate = 0;
						}
					}
					//	Boost multihit moves for Skill Link and Loaded Dice, or touching abilities.
					if((randomAbility.props.Contains("multihit") || randomItem.props.Contains("multihit")) && move.props.Contains("multihit")){
						move.rate *= 80; isUsed = true;}
					if(randomAbility.props.Contains("touching") && move.props.Contains("multihit")){
						move.rate *= 30; isUsed = true;}
					//	Boost moves for abilities and/or items for crit, biting, pulse, technician and triage.
					if((randomAbility.props.Contains("crit") || randomItem.props.Contains("crit")) && move.props.Contains("crit")){
						move.rate *= 20; isUsed = true;}
					if(randomAbility.props.Contains("biting") && move.props.Contains("biting")){
						move.rate *= 80; isUsed = true;}
					if(randomAbility.props.Contains("slicing") && move.props.Contains("slicing")){
						move.rate *= 80; isUsed = true;}
					if(randomAbility.props.Contains("pulse") && move.props.Contains("pulse")){
						move.rate *= 80; isUsed = true;}
					if(randomAbility.props.Contains("power60") && move.power <= 60 && move.power >= 15){
						move.rate *= 20; isUsed = true;}
					if(randomAbility.props.Contains("power60") && move.power > 60 && move.power < 90){
						move.rate *= 0;	}
					if(randomAbility.props.Contains("triage") && move.props.Contains("triage")){
						move.rate *= 40; isUsed = true;}
					if(!randomAbility.props.Contains("triage") && pickedMoves.Any(p => p.props.Contains("triage")) && move.props.Contains("triage"))
						move.rate = 0;
					//	Boost Acrobatics on items that are used quickly, otherwise remove it.
					if(randomItem.props.Contains("quickuse") && move.props.Contains("quickuse"))
						{move.rate *= 6; isUsed = true;}
					if(!randomItem.props.Contains("quickuse") && move.props.Contains("quickuse"))
						move.rate *= 0;
					//	Assault Vest removes status moves.
					if(randomItem.props.Contains("assault") && move.category == 2)
						move.rate *= 0;
					//	Removes Water and Fire moves for abilities that nerf these types.
					if(randomAbility.props.Contains("nowater") && move.type == Type.Water)
						move.rate *= 0;
					if(randomAbility.props.Contains("nofire") && move.type == Type.Fire)
						move.rate *= 0;
					if(randomAbility.props.Contains("nodragon") && move.type == Type.Dragon)
						move.rate *= 0;
					//	Increase rates of charging moves while Power Herb is held until one is picked otherwise nerf them or even remove them.
					if(randomItem.props.Contains("charging") && move.props.Contains("charging") && !chargingMoveUsed){
						move.rate *= 80; isUsed = true;}
					if(!randomItem.props.Contains("charging") && move.props.Contains("charging"))
						move.rate *= 0.25;
					if(chargingMoveUsed && move.props.Contains("charging"))
						move.rate *= 0;
					//	Contact handling.
					if(randomAbility.props.Contains("contact"))
					{
						if(move.category == 1 && !move.props.Contains("contact"))
							move.rate = 0;
						if(move.category == 0 && move.props.Contains("nocontact"))
							move.rate = 0;	
					}
					if(move.props.Contains("highhp"))
					{
						if(randomItem.props.Contains("choiced"))
							move.rate *= 5;
						else
							move.rate = 0;
					}

					//	When using an All-In setup move, disable status moves.
					if(pickedMoves.Any(p => p.props.Contains("allin")))
					{
						if(move.category == 2)
							move.rate = 0;
					}
					if(move.props.Contains("allin") && pickedMoves.Count > 1)
						move.rate = 0;

					//	Disable Trick Room for non-slow pokemons or persistent.
					if(move.name == "Trick Room" && randomPokemon.moreInfo != "slow" && randomAbility.name != "Persistent")
						move.rate = 0;
					if(move.name == "Trick Room" && randomPokemon.moreInfo == "slow")
						move.rate *= 5;
					
					//	CAP abilities.
					if(randomAbility.props.Contains("persistent") && move.props.Contains("persistent"))
						move.rate *= 20;

					//	Boost Setup moves when Power Trip and Stored Power are present in the chosen moves.
					if(pickedMoves.Any(picked => picked.props.Contains("setup")))
					{
						if(move.props.Contains("physical"))
							move.rate *= 8 * j;
						if(move.props.Contains("special"))
							move.rate *= 8 * j;
						if(move.props.Contains("offence"))
							move.rate *= 10 * j;
						if(move.props.Contains("speed"))
							move.rate *= 10 * j;
						if(move.props.Contains("defence"))
							move.rate *= 10 * j;
						if(move.props.Contains("ownstatraise"))
							move.rate *= 5 * j;
					}
					if(move.props.Contains("setup") && j > 2)
						move.rate = 0;

					//	Increase low PP moves when Leppa Berry is selected.
					if(randomItem.name == "Leppa Berry" && move.props.Contains("lowpp"))
					{
						move.rate *= 10;
						if(move.name == "Revival Blessing")
							move.rate *= 10;
					}

					//	Tailwind Wind Rider
					if(randomAbility.name == "Wind Rider" && move.name == "Tailwind")
						move.rate *= 120;

					// Substitute boost on Leftovers and Black Sludge, or healing abilities.
					if(move.name == "Substitute")
					{
						if(randomItem.name == "Leftovers" || randomItem.name == "Black Sludge")
							move.rate *= 10;
						if(randomAbility.stats == "heal")
							move.rate *= 10;
					}

					//	Terrain boosted moves during terrain, otherwise reduce them.
					if(move.props.Contains("terrain"))
					{
						if(randomAbility.props.Any(ab => ab.Contains("seed")))
						{
							string match = randomAbility.props.First(ab => ab.Contains("seed"));
							string matchT = match.Substring(match.IndexOf("seed") + "seed".Length);
							if(matchT == move.type.ToString().ToLower())
								{move.rate *= 10; isUsed = true;}
							else
								move.rate *= 0.25;

							if(move.name == "Psyblade" && matchT == "electric")
								move.rate *= 40;
							
							if((move.name == "Bulldoze" || move.name == "Earthquake") && matchT != "grass")
								move.rate *= 4;

							if(move.name == "Floral Healing" && matchT == "grass")
								move.rate *= 12;

							if(move.name == "Misty Explosion" && matchT == "misty")
								move.rate *= 12;

							if(move.name == "Terrain Pulse")
								move.rate *= 20;
						}
						else
							move.rate *= 0.25;

						if(move.name == "Psyblade" || move.name == "Bulldoze" || move.name == "Earthquake")
							move.rate *= 4;
						else if(randomPokemon.type1 == Type.Flying || randomPokemon.type2 == Type.Flying)
							move.rate = 0;
					}
					
					//	Abilities that like having weather or terrain.
					if(randomAbility.name == "Swift Swim" && move.name == "Rain Dance")
					{
						if(!chosenAbilities.Any(a => a == "Drizzle"))
						{
							move.rate *= 40;
							isUsed = true;
						}
					}
					if((randomAbility.name == "Surge Surfer" || randomAbility.name == "Quark Drive") && move.name == "Electric Terrain")
					{
						if(!chosenAbilities.Any(a => a == "Electric Surge" || a == "Hadron Engine") && randomItem.name != "Booster Energy")
						{
							move.rate *= 40;
							isUsed = true;
						}
					}
					if((randomAbility.name == "Chlorophyll" || randomAbility.name == "Flower Gift" || randomAbility.name == "Protosynthesis" || randomAbility.name == "Solar Power") && move.name == "Sunny Day")
					{
						if(!chosenAbilities.Any(a => a == "Drought" || a == "Orichalcum Pulse") && randomItem.name != "Booster Energy")
						{
							move.rate *= 40;
							isUsed = true;
						}
					}
					if((randomAbility.name == "Sand Rush" || randomAbility.name == "Sand Force") && move.name == "Sandstorm")
					{
						if(!chosenAbilities.Any(a => a == "Sand Spit" || a == "Sand Stream"))
						{
							move.rate *= 40;
							isUsed = true;
						}
					}
					if(randomAbility.name == "Slush Rush" && move.name == "Snowscape")
					{
						if(!chosenAbilities.Any(a => a == "Snow Warning") && !chosenMoves.Any(moves => moves.Any(move => move == "Chilly Reception")))
						{
							move.rate *= 40;
							isUsed = true;
						}
					}

					

					//	Moves boosted by weather
					if(move.name == "Weather Ball")
					{
						if(randomAbility.props.Contains("weather"))
							move.rate *= 40;
						else
							move.rate *= 0;
					}
					if(randomAbility.name == "Drought" || randomAbility.name == "Orichalcum Pulse")
					{
						if(move.props.Contains("sun"))
							move.rate *= 5;
						if(move.props.Contains("rain"))
							move.rate *= 0;
					}
					//	Mega Sol handle.
					else if(randomAbility.props.Contains("megasol"))
					{
						if(move.props.Contains("sun"))
						{
							move.rate *= 20;
							if(move.props.Contains("charging"))
								move.rate *= 32;
						}
						if(move.type == Type.Fire)
							move.rate *= 5;
						if(move.props.Contains("rain"))
							move.rate *= 0;
					}
					else if(randomAbility.name == "Sand Spit" || randomAbility.name == "Sand Stream")
					{
						if(move.props.Contains("sand"))
							move.rate *= 5;
					}
					else if(randomAbility.name == "Drizzle")
					{
						if(move.props.Contains("rain"))
							move.rate *= 5;
						if(move.props.Contains("sun"))
							move.rate = 0;
					}
					else
					{
						if(move.props.Contains("sun") || move.props.Contains("rain") || move.props.Contains("sand"))
							move.rate *= 0.25;
					}

					//	No priority moves on Psychic Terrain.
					if(move.props.Contains("posprio"))
					{
						if(randomAbility.name == "Psychic Surge")
							move.rate = 0;
						if(chosenAbilities.Any(a => a == "Psychic Surge"))
							move.rate *= 0.5;
					}
					//	No status inflict moves on Misty Terrain.
					if (move.props.Contains("status"))
					{
						if(randomAbility.name == "Misty Surge")
							move.rate = 0;
						if(chosenAbilities.Any(a => a == "Misty Surge"))
							move.rate *= 0.5;
					}

					//	Moves not for Shedinja.
					if(randomPokemon.name == "shedinja")
					{
						if(move.props.Contains("triage") || move.props.Contains("defence") || move.props.Contains("recoil") || move.props.Contains("charging")
						|| move.props.Contains("clay") || move.props.Contains("magicguard") || move.props.Contains("negprio"))
							move.rate = 0;
					}
					
					if(move.props.Contains("false") && !isUsed)
						move.rate *= 0;
				}

				Move tmpMove 	= WeightRandomSelect.SelectRandomWeighted(moves, move => move.rate);
				Move randomMove	= new Move(tmpMove.name, tmpMove.type, tmpMove.category, tmpMove.power, tmpMove.accuracy, tmpMove.generation, tmpMove.props, tmpMove.rate);
				pickedMoves.Add(randomMove);
				randomMoves.Add(randomMove.name);
				if(randomMove.props.Contains("charging"))
					chargingMoveUsed = true;
				if(randomItem.props.Contains("plate") && randomItem.props.Any(p => p == randomMove.type.ToString().ToLower()) && randomMove.category != 2)
					itemMoveFound	= true;
				if(randomMove.props.Contains("clay"))
					clayRate += 20;
				if(randomMove.name == "Tera Blast" || randomMove.name == "Revelation Dance")
				{
					if(randomAbility.props.Contains("ownstatlower"))
					{
						randomMove.props.Add("terastellar");
					}
					else
					{
						for(int t = 1; t < 3; t++)
						{
							Type tmpType = randomPokemon.type1;
							if(t == 2)
								tmpType	= randomPokemon.type2;
							switch(tmpType)
							{
								case Type.Normal:	randomMove.props.AddRange(new string[] {"teraghost", "terafighting"});	break;
								case Type.Fighting:	randomMove.props.AddRange(new string[] {"terasteel", "teraice"});		break;
								case Type.Flying:	randomMove.props.AddRange(new string[] {"teraground", "terafighting"});	break;
								case Type.Poison:	randomMove.props.AddRange(new string[] {"teradark", "teraground"});		break;
								case Type.Ground:	randomMove.props.AddRange(new string[] {"terawater", "terafire"});		break;
								case Type.Rock:		randomMove.props.AddRange(new string[] {"terafairy", "terafighting"});	break;
								case Type.Bug:		randomMove.props.AddRange(new string[] {"terasteel", "terawater"});		break;
								case Type.Ghost:	randomMove.props.AddRange(new string[] {"terafighting", "terafairy"});	break;
								case Type.Steel:	randomMove.props.AddRange(new string[] {"teraflying", "terawater"});	break;
								case Type.Fire:		randomMove.props.AddRange(new string[] {"teragrass", "teraground"});	break;
								case Type.Water:	randomMove.props.AddRange(new string[] {"teradragon", "terafire"});		break;
								case Type.Grass:	randomMove.props.AddRange(new string[] {"terarock", "terafire"});		break;
								case Type.Electric:	randomMove.props.AddRange(new string[] {"teraflying", "teraice"});		break;
								case Type.Psychic:	randomMove.props.AddRange(new string[] {"teradark", "terafairy"});		break;
								case Type.Ice:		randomMove.props.AddRange(new string[] {"terafire", "teraelectric"});	break;
								case Type.Dragon:	randomMove.props.AddRange(new string[] {"terasteel", "teraground"});	break;
								case Type.Dark:		randomMove.props.AddRange(new string[] {"terapoison", "teraflying"});	break;
								case Type.Fairy:	randomMove.props.AddRange(new string[] {"terasteel", "teraground"});	break;
							}
						}
					}
				}
			}

			if(clayRate > 0)
			{
				if(Probability.Roll(clayRate))
					randomItem	= new Item("Light Clay", "balance", new List<string>{"false"}, clayRate * 100);
			}
		
			chosenPokemons.Add(randomPokemon.name);
			chosenAbilities.Add(randomAbility.name);
			chosenItems.Add(randomItem.name);
			chosenMoves.Add(randomMoves);

			string chosenTera = "";

			//	Some moves, abilities or items force a bunch of teras, if nothing was forced when choose based on move or Stellar.
			List<string> possibleTeras = new List<string>{};
			switch (randomPokemon.name)
			{
				case "ogerpon": possibleTeras.Add("grass");	break;
				case "Ogerpon-Cornerstone": possibleTeras.Add("rock");	break;
				case "Ogerpon-Hearthflame": possibleTeras.Add("fire");	break;
				case "Ogerpon-Wellspring": possibleTeras.Add("water");	break;
			}
			var abilTeras = randomAbility.props.Where(ab => ab.Contains("tera")).Select(ab => ab.Substring(ab.IndexOf("tera") + "tera".Length)).ToList();
			possibleTeras.AddRange(abilTeras);
			var itemTeras = randomItem.props.Where(it => it.Contains("tera")).Select(it => it.Substring(it.IndexOf("tera") + "tera".Length)).ToList();
			possibleTeras.AddRange(itemTeras);
			pickedMoves.SelectMany(s => s.props.Where(p => p.Contains("tera")).Select(p => p.Substring(p.IndexOf("tera") + 4))).ToList().ForEach(t => possibleTeras.Add(t.Trim()));
			if(possibleTeras.Count > 0)
				chosenTera = possibleTeras[Random.Shared.Next(possibleTeras.Count)];
			else {
				Random rand = new Random();
				int n = rand.Next(0,9);
				switch (n)
				{
					case 0:
					case 1: chosenTera = pickedMoves[0].type.ToString();	break;
					case 2:
					case 3: chosenTera = pickedMoves[1].type.ToString();	break;
					case 4:
					case 5: chosenTera = pickedMoves[2].type.ToString();	break;
					case 6:
					case 7: chosenTera = pickedMoves[3].type.ToString();	break;
					case 8: chosenTera = "Stellar";	break;
				}
			}

			string randomNick = names[Random.Shared.Next(names.Count)];
			Random rnd = new Random();
			int r = rnd.Next(3,8);
			switch (r)
			{
				case 6:	randomNick = string.Concat(randomPokemon.name.AsSpan(0,3), randomNick.AsSpan(3, randomNick.Length - 3)); break;
				case 7:	randomNick = string.Concat(randomNick.AsSpan(0,randomNick.Length - 3), randomPokemon.name.AsSpan(randomPokemon.name.Length - 3, 3)); break;
			}
		
			lines.Add(randomNick + " (" + randomPokemon.name + ") @ " + randomItem.name);
			lines.Add("Ability: " + randomAbility.name);
			if(useLevels && i <= leveledPokemons)
				lines.Add("Level: " + (levelStart + Math.Min(255, Math.Round((1 - (double)(randomPokemon.bst - levelMinBST) / (levelMaxBST - levelMinBST))  * levelMult))));
			if(Probability.Roll(1))
				lines.Add("Shiny: Yes");
			lines.Add("Tera Type: " + char.ToUpper(chosenTera[0]) + chosenTera.Substring(1));

			/* EV IV
				Phys
					EVs: 100 HP / 108 Atk / 100 Def / 0 SpA / 100 SpD / 100 Spe
					Adamant/Jolly/Hasty(berry) Nature
				Spec
					EVs: 100 HP / 0 Atk / 100 Def / 108 SpA / 100 SpD / 100 Spe
					Modest/Timid/Hasty(berry) Nature
				Slow
					EVs: 0 Spe
					Brave/Relaxed/Quiet/Sassy/Serious Nature
				Fast
					EVs: 252 Spe
					Timid/Jolly/Hasty(berry) Nature
				Frail
					EVs: 4 HP / 0 Def / 0 SpD
			*/
			Dictionary<string, int> evs = new Dictionary<string, int>
            {
                { "HP", 0 },
                { "Atk", 0 },
                { "Def", 0 },
                { "SpA", 0 },
                { "SpD", 0 },
                { "Spe", 0 }
            };
			string nature = "";
			Dictionary<string, int> ivs = new Dictionary<string, int>{ { "Atk", 31 },{ "Spe", 31 }};
			switch (randomPokemon.stats)
			{
				case "phys":
					{
						Random rPhys = new Random();
						string[] posNat = {"Adamant", "Jolly"};
						nature = posNat[rPhys.Next(0, posNat.Length)];

						evs = new Dictionary<string, int> {{ "HP", 60 },{ "Atk", 164 },{ "Def", 60 },{ "SpA", 0 },{ "SpD", 60 },{ "Spe", 164 }};
						if(randomPokemon.moreInfo == "fast") {
							evs = new Dictionary<string, int> {{ "HP", 0 },{ "Atk", 252 },{ "Def", 0 },{ "SpA", 0 },{ "SpD", 4 },{ "Spe", 252 }};
							nature = "Jolly";
						}
						else if(randomPokemon.moreInfo == "slow" || pickedMoves.Any(m => m.name == "Trick Room"))
						{
							evs = new Dictionary<string, int> {{ "HP", 136 },{ "Atk", 252 },{ "Def", 60 },{ "SpA", 0 },{ "SpD", 60 },{ "Spe", 0 }};
							nature = "Brave";
							if(pickedMoves.Any(m => m.name == "Gyro Ball" || m.name == "Trick Room"))
								ivs = new Dictionary<string, int>{ { "Atk", 31 },{ "Spe", 0 }};
						}
						else if(randomPokemon.moreInfo == "frail")
						{
							evs = new Dictionary<string, int> {{ "HP", 0 },{ "Atk", 252 },{ "Def", 0 },{ "SpA", 0 },{ "SpD", 4 },{ "Spe", 252 }};
						}
					}
				break;
				case "spec":
					{
						Random rSpec = new Random();
						string[] posNat = {"Modest", "Timid"};
						nature = posNat[rSpec.Next(0, posNat.Length)];

						evs = new Dictionary<string, int> {{ "HP", 60 },{ "Atk", 0 },{ "Def", 60 },{ "SpA", 164 },{ "SpD", 60 },{ "Spe", 164 }};
						ivs = new Dictionary<string, int>{ { "Atk", 0 },{ "Spe", 31 }};
						if(randomPokemon.moreInfo == "fast") {
							evs = new Dictionary<string, int> {{ "HP", 0 },{ "Atk", 0 },{ "Def", 0 },{ "SpA", 252 },{ "SpD", 4 },{ "Spe", 252 }};
							nature = "Timid";
						}
						else if(randomPokemon.moreInfo == "slow" || pickedMoves.Any(m => m.name == "Trick Room"))
						{
							evs = new Dictionary<string, int> {{ "HP", 136 },{ "Atk", 0 },{ "Def", 60 },{ "SpA", 252 },{ "SpD", 60 },{ "Spe", 0 }};
							nature = "Quiet";
							if(pickedMoves.Any(m => m.name == "Gyro Ball" || m.name == "Trick Room"))
								ivs = new Dictionary<string, int>{ { "Atk", 0 },{ "Spe", 0 }};
						}
						else if(randomPokemon.moreInfo == "frail")
						{
							evs = new Dictionary<string, int> {{ "HP", 0 },{ "Atk", 0 },{ "Def", 0 },{ "SpA", 252 },{ "SpD", 4 },{ "Spe", 252 }};
						}
					}
				break;
				case "tank":
					{
						int nPhys = pickedMoves.Count(m => m.category == 0);
						int nSpec = pickedMoves.Count(m => m.category == 1);
						int nStat = pickedMoves.Count(m => m.category == 2);
						if(nPhys > nSpec && nPhys > nStat)
						{
							evs = new Dictionary<string, int> {{ "HP", 128 },{ "Atk", 128 },{ "Def", 100 },{ "SpA", 0 },{ "SpD", 100 },{ "Spe", 52 }};
							nature = "Adamant";
						}
						else if(nSpec > nPhys && nSpec > nStat)
						{
							evs = new Dictionary<string, int> {{ "HP", 128 },{ "Atk", 0 },{ "Def", 100 },{ "SpA", 128 },{ "SpD", 100 },{ "Spe", 52 }};
							nature = "Modest";
							if(nSpec > nPhys && nPhys == 0)
								ivs = new Dictionary<string, int>{ { "Atk", 0 },{ "Spe", 31 }};
						}
						else
						{
							evs = new Dictionary<string, int> {{ "HP", 248 },{ "Atk", 0 },{ "Def", 104 },{ "SpA", 0 },{ "SpD", 104 },{ "Spe", 52 }};
							if(nSpec > nPhys)
							{
								Random rTank = new Random();
								string[] posNat = {"Bold", "Calm"};
								nature = posNat[rTank.Next(0, posNat.Length)];
								if(nPhys == 0)
									ivs = new Dictionary<string, int>{ { "Atk", 0 },{ "Spe", 31 }};
							}
							else if(nPhys > nSpec)
							{
								Random rTank = new Random();
								string[] posNat = {"Impish", "Careful"};
								nature = posNat[rTank.Next(0, posNat.Length)];
							}
							else
							{
								Random rTank = new Random();
								string[] posNat = {"Relaxed", "Sassy"};
								nature = posNat[rTank.Next(0, posNat.Length)];
							}
						}
						if(randomPokemon.moreInfo == "slow" || pickedMoves.Any(m => m.name == "Trick Room"))
						{
							evs["Spe"] -= 52;
							evs["HP"] += 4;
							evs["Def"] += 24;
							evs["SpD"] += 24;
							Random rTank = new Random();
							string[] posNat = {"Relaxed", "Sassy"};
							nature = posNat[rTank.Next(0, posNat.Length)];
							if(pickedMoves.Any(m => m.name == "Gyro Ball" || m.name == "Trick Room"))
								ivs["Spe"] = 0;
						}
					}
				break;
				case "mixed":
				case "any":
					{
						int nPhys = pickedMoves.Count(m => m.category == 0);
						int nSpec = pickedMoves.Count(m => m.category == 1);
						evs = new Dictionary<string, int> {{ "HP", 52 },{ "Atk", 0 },{ "Def", 52 },{ "SpA", 0 },{ "SpD", 52 },{ "Spe", 0 }};
						evs["Atk"] = 44 * nPhys;
						evs["SpA"] = 44 * nSpec;
						evs["Spe"] = Math.Min(352 - evs["Atk"] - evs["SpA"], 252);
						if(352 - evs["Atk"] - evs["SpA"] > 252)
							evs["HP"] += 352 - evs["Atk"] - evs["SpA"] - 252;

						if(randomPokemon.moreInfo == "fast") {
							int speDif = 252 - evs["Spe"];
							evs["Spe"] = 252;
							for(int h = 0; h < speDif; h += 4)
							{
								int modul = h % 12;
								switch (modul)
								{
									case 0: evs["HP"] -= 4;	break;
									case 4: evs["Def"] -= 4;	break;
									case 8: evs["SpD"] -= 4;	break;
								}
							}
							if(nPhys == 0)
								nature = "Timid";
							else if(nSpec == 0)
								nature = "Jolly";
							else
								nature = "Hasty";
						}
						else if(randomPokemon.moreInfo == "slow" || pickedMoves.Any(m => m.name == "Trick Room"))
						{
							int speDif = evs["Spe"];
							int hpDif = 0;
							evs["Spe"] = 0;
							if(speDif + evs["HP"] > 252)
								hpDif = speDif + evs["HP"] - 252;
							evs["HP"] += Math.Min(252 - evs["HP"], speDif);	/* Test */
							if(pickedMoves.Any(m => m.name == "Gyro Ball" || m.name == "Trick Room"))
								ivs["Spe"] = 0;
							if(nPhys == 0)
							{
								nature = "Quiet";
								evs["SpA"] += hpDif;
							}
							else if(nSpec == 0)
							{
								nature = "Brave";
								evs["Atk"] += hpDif;
							}
							else
							{
								evs["SpD"] += hpDif;
								nature = "Sassy";
							}
						}
						else if(randomPokemon.moreInfo == "frail")
						{
							evs = new Dictionary<string, int> {{ "HP", 0 },{ "Atk", 0 },{ "Def", 0 },{ "SpA", 0 },{ "SpD", 4 },{ "Spe", 252 }};
							if(nPhys > nSpec)
							{
								evs["Atk"] = 252;
								if(nSpec == 0)
								{
									Random rSpec = new Random();
									string[] posNat = {"Adamant", "Jolly"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
								else
								{
									Random rSpec = new Random();
									string[] posNat = {"Lonely", "Hasty"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
							}
							if(nSpec > nPhys)
							{
								evs["SpA"] = 252;
								if(nPhys == 0)
								{
									ivs["Atk"] = 0;
									Random rSpec = new Random();
									string[] posNat = {"Hardy", "Timid"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
								else
								{
									Random rSpec = new Random();
									string[] posNat = {"Mild", "Hasty"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
							}
							else
							{
								evs["Atk"] = 128;
								evs["SpA"] = 124;
								Random rSpec = new Random();
								string[] posNat = {"Lonely", "Mild", "Hasty"};
								nature = posNat[rSpec.Next(0, posNat.Length)];
							}
						}
						else
						{
							if(nPhys > nSpec)
							{
								if(nSpec == 0)
								{
									Random rSpec = new Random();
									string[] posNat = {"Adamant", "Jolly"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
								else
								{
									Random rSpec = new Random();
									string[] posNat = {"Lonely", "Hasty"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
							}
							else if(nSpec > nPhys)
							{
								if(nPhys == 0)
								{
									ivs["Atk"] = 0;
									Random rSpec = new Random();
									string[] posNat = {"Hardy", "Timid"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
								else
								{
									Random rSpec = new Random();
									string[] posNat = {"Mild", "Hasty"};
									nature = posNat[rSpec.Next(0, posNat.Length)];
								}
							}
							else
							{
								Random rSpec = new Random();
								string[] posNat = {"Lonely", "Mild", "Hasty"};
								nature = posNat[rSpec.Next(0, posNat.Length)];
							}
						}
					}
				break;
			}
			switch (randomItem.name)
			{
				case "Aguav Berry":
					{
						/* if(nature == "Naughty" || nature == "Rash" || nature == "Naive" || nature == "Lax")
						{} */
					}
					break;
				case "Figy Berry":
				case "Mago Berry":
				case "Wiki Berry":
					{
						if(nature == "Modest" || nature == "Timid" || nature == "Calm" || nature == "Bold"|| nature == "Jolly" || nature == "Brave")
							nature = "Hasty";
					}
					break;
				case "Iapapa Berry":
					{
						if(nature == "Mild" || nature == "Gentle" || nature == "Lonely" || nature == "Hasty")
							nature = "Naive";
					}
					break;
			}

			lines.Add("EVs: " + evs["HP"] + " HP / " + evs["Atk"] + " Atk / " + evs["Def"] + " Def / " + evs["SpA"] + " SpA / " + evs["SpD"] + " SpD / " + evs["Spe"] + " Spe");
			lines.Add(nature + " Nature");
			lines.Add("IVs: " + ivs["Atk"] + " Atk / " + ivs["Spe"] + " Spe");

			foreach(string s in randomMoves)
			{
				lines.Add("- " + s);
			}
			lines.Add("");
		}		

		File.WriteAllLines(outputPath, lines, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
		if (trainerPokemon && trainerTeams)
		{
			linesPokemon = new List<string>(lines.Count);
			int j = 0;
			int k = 0;
			string trainer = "[\"" + chosenPokemons[j] + "\", " + startOnLine + "], ";
			for (int i = 0; i < lines.Count; i++)
			{
				if(lines[i] == "")
				{
					if (i + 1 < lines.Count)
					{
						linesPokemon.Add("|" + lines[i + 1]);
						i++;
						k = startOnLine + i - j - 1;
						j++;
						trainer += "[\"" + chosenPokemons[j] + "\", " + k;
						if(j + 1 < nPokemons)
							trainer += "], ";
						else
							trainer += "]";
					}
					continue;
				}
				linesPokemon.Add(lines[i]);
			}
			if(linesPokemon.Count > 0)
				linesPokemon[0] = "|" + linesPokemon[0];
			linesTeams.Add(trainer);
			File.WriteAllLines(outputPathPokemon, linesPokemon, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
			File.WriteAllLines(outputPathTeams, linesTeams, new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
		}
	}

	public class WeightRandomSelect
	{
		private static readonly Random Random = new Random();

		public static T SelectRandomWeighted<T>(IReadOnlyList<T> items, Func<T, double> weightSelector)
		{
			double totalWeight = items.Sum(weightSelector);
			
			double randomValue = Random.NextDouble() * totalWeight;

			double sum = 0;
			foreach(var item in items)
			{
				sum += weightSelector(item);
				if(sum >= randomValue)
					return item;
			}
			return items[^1];
		}
	}

	public static class Probability
	{
		/// <summary>
		/// Returns true with exactly the specified percentage chance (1-100).
		/// Values outside 0-100 are clamped for safety.
		/// </summary>
		/// <param name="percentage">Chance of returning true, from 1 to 100.</param>
		/// <returns>true with percentage% probability, otherwise false.</returns>
		public static bool Roll(int percentage)
		{
			// Clamp to valid range
			if (percentage <= 0) return false;
			if (percentage >= 100) return true;

			// Random.Shared is thread-safe and available in .NET 6+
			return Random.Shared.Next(0, 100) < percentage;
		}
	}
}
