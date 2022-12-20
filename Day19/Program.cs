using System.Text.RegularExpressions;

part1();
//part2();

static void part1() {
	var blueprints = read();

	var sum = 0;
	for (int i = 0; i < blueprints.Count; i++) {
		var g = check(
			blueprints[i], 24, new Resources(0, 0, 0, 0), 
			new Resources(1, 0, 0, 0)
		);
		Console.WriteLine($"{i+1}: {g}");
		sum += (i+1)*g;
	}
	Console.WriteLine($"Total: {sum}");

}

static void part2() {
	var blueprints = read();

	var sum = 1;
	for (int i = 0; i < 3; i++) {
		var g = check(
			blueprints[i], 32, new Resources(0, 0, 0, 0), 
			new Resources(1, 0, 0, 0)
		);
		Console.WriteLine($"{i+1}: {g}");
		sum *= g;
	}
	Console.WriteLine($"Total: {sum}");

}

static List<Blueprint> read() {
	var regex = new Regex(@"([a-z]+) robot.*?(\d+) ore(?: and (\d+))?");
	var blueprints = new List<Blueprint>();

	foreach (var line in File.ReadLines("input.txt")) {
		var matches = regex.Matches(line);

		var blueprint = new Blueprint();
		var maxOre = 0;
		var maxClay = 0;
		var maxObs = 0;
		foreach (Match match in matches) {
			var robot = new Resources(int.Parse(match.Groups[2].Value), 0, 0, 0);
			switch (match.Groups[1].Value) {
			case "ore":
				blueprint.ore = robot;
				break;
			case "clay":
				blueprint.clay = robot;
				break;
			case "obsidian":
				robot = robot.add(0, int.Parse(match.Groups[3].Value), 0, 0);
				blueprint.obsidian = robot;
				break;
			case "geode":
				robot = robot.add(0, 0, int.Parse(match.Groups[3].Value), 0);
				blueprint.geode = robot;
				break;
			}
			maxOre = Math.Max(maxOre, robot.ore);
			maxClay = Math.Max(maxClay, robot.clay);
			maxObs = Math.Max(maxObs, robot.obsidian);
		}
		blueprint.max = new Resources(maxOre, maxClay, maxObs, 0);

		blueprints.Add(blueprint);
	}
	return blueprints;
}

static int check(Blueprint b, int stepsLeft, Resources resources, Resources income) {
	if (stepsLeft == 0) {
		return resources.geode;
	}
	if (stepsLeft == 1) {
		return resources.geode + income.geode;
	}
	var buildOptions = new List<Action> { Action.nothing };
	if (income.ore < b.max.ore && b.ore.canAfford(resources)) buildOptions.Add(Action.ore);
	if (income.clay < b.max.clay  && b.clay.canAfford(resources)) buildOptions.Add(Action.clay);
	if (income.obsidian < b.max.obsidian  && b.obsidian.canAfford(resources)) buildOptions.Add(Action.obsidian);
	if (b.geode.canAfford(resources)) buildOptions.Add(Action.geode);
	//buildOptions.Reverse();

	var best = 0;
	var paths = new List<string>();
	foreach (var option in buildOptions) {
		var newIncome = option switch {
			Action.nothing => income,
			Action.ore => income.add(1, 0, 0, 0),
			Action.clay => income.add(0, 1, 0, 0),
			Action.obsidian => income.add(0, 0, 1, 0),
			Action.geode => income.add(0, 0, 0, 1)
		};
		var cost = option switch {
			Action.nothing => new Resources(0, 0, 0, 0),
			Action.ore => b.ore,
			Action.clay => b.clay,
			Action.obsidian => b.obsidian,
			Action.geode => b.geode
		};
		var newResources = resources.sub(cost).add(income);
		best = Math.Max(best, check(b, stepsLeft-1, newResources, newIncome));
	}

	return best;
}

enum Action {
	nothing = 0,
	ore = 1,
	clay = 2,
	obsidian = 3,
	geode = 4
}

class Blueprint {
	public Resources ore;
	public Resources clay;
	public Resources obsidian;
	public Resources geode;

	public Resources max;
}

record Resources(int ore, int clay, int obsidian, int geode) {
	public Resources add(Resources r) {
		return new Resources(ore+r.ore, clay+r.clay, obsidian+r.obsidian, geode+r.geode);
	}

	public Resources add(int ore, int clay, int obsidian, int geode) {
		return new Resources(this.ore+ore, this.clay+clay, this.obsidian+obsidian, this.geode+geode);
	}

	public Resources sub(Resources r) {
		return new Resources(ore-r.ore, clay-r.clay, obsidian-r.obsidian, geode-r.geode);
	}

	public bool canAfford(Resources r) {
		return r.ore >= ore && r.clay >= clay && r.obsidian >= obsidian;
	}
}