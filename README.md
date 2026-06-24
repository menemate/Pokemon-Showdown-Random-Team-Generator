Run program.cs

You can edit some variables to your need, like:
banMegas //	When true, no pokemon can mega evolve, it becomes true after 1 Mega is in the team.
banZcrystals //	When true, Z-Crystal won't appear, it becomes true after 1 Z-Crystal is on the team.
notSingles //	When true, doubles only moves/abilities appear
nPokemons //	How many pokemon in the team.

useLevels	// Enable different levels per pokemon.
leveledPokemons	//	How many pokemons have a level, if the value is 0, only the first pokemon has a level.
levelStart //	Minimum possible level of a pokemon.
levelMinBST //	Lowest BST to compare.
levelMaxBST //  Highest BST to compare.
levelMult	//  Pokemon from Lowest to Highest BST range within levelStart and levelStart + levelMult

restrictMons	//	When true, uses includeMonTypes, includeMonGens, includeMonBST and includeMonName as filters.
restrictAbils	//	When true, uses includeAbilGens and includeAbilName as filters.
restrictMoves	//	When true, uses includeMoveTypes, includeMoveGens and includeMoveName as filters.

Possible types
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
		List<int> includeAbilGens	= new List<int>{	1, 2, 3, 4, 5, 6, 7, 8, 9	};
		List<int> includeMoveGens	= new List<int>{	1, 2, 3, 4, 5, 6, 7, 8, 9	};

		int[] includeMonBST	= {0, 1000};	//	Minimum and maximum Base Stat Total to include in the team, min and max values are included.

/* "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" */
		List<string> includeMonName		= new List<string>{ "zoroark", "zygarde-1", "arceus", "silvally"};
		List<string> includeAbilName	= new List<string>{ "Competitive", "Thermal Exchange", "Flame Body", "As One (Glastrier)", "Galvanize", "Guts", "Imposter", "Marvel Scale", "Queenly", "Quick Draw", "Serene", "Super Luck", "Triage", "Water V"};
		List<string> includeMoveName	= new List<string>{ "u", "v", "z"};
