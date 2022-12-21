using System.Text.RegularExpressions;

part1();
part2();

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
	for (int i = 0; i < Math.Min(3, blueprints.Count); i++) {
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
	var buildOptions = new List<Action>();

	var oreSteps = affordableInSteps(b.ore);
	var claySteps = affordableInSteps(b.clay);
	var obsSteps = affordableInSteps(b.obsidian);
	var geodeSteps = affordableInSteps(b.geode);
	if (income.ore < b.max.ore && oreSteps <= stepsLeft-3) buildOptions.Add(Action.ore);
	if (income.clay < b.max.clay &&
	    resourceCheck(b.max.clay-income.clay, 5, b.obsidian.clay, resources.clay, income.clay) &&
	    claySteps <= stepsLeft-5) buildOptions.Add(Action.clay);
	if (income.obsidian < b.max.obsidian &&
	    resourceCheck(b.max.obsidian-income.obsidian, 3, b.geode.obsidian, resources.obsidian, income.obsidian) &&
	    obsSteps <= stepsLeft-3) buildOptions.Add(Action.obsidian);
	if (geodeSteps <= stepsLeft-1) buildOptions.Add(Action.geode);

	if (buildOptions.Count == 0) {
		// Out of things to build, run out the clock.
		return resources.geode + stepsLeft*income.geode;
	}

	int best = 0;
	foreach (var option in buildOptions) {
		// The resource computation: skip to the step we build, and add the income for the build step as well, then subtract build cost
		var subBest = option switch {
			Action.ore =>
				best = check(
					b, stepsLeft-oreSteps-1,
					resources + (oreSteps+1) * income - b.ore,
					income + new Resources(1, 0, 0, 0)
				),
			Action.clay =>
				check(
					b, stepsLeft-claySteps-1,
					resources + (claySteps+1) * income - b.clay,
					income + new Resources(0, 1, 0, 0)
				),
			Action.obsidian =>
				check(
					b, stepsLeft-obsSteps-1,
					resources + (obsSteps+1) * income - b.obsidian,
					income + new Resources(0, 0, 1, 0)
				),
			Action.geode =>
				check(
					b, stepsLeft-geodeSteps-1,
					resources + (geodeSteps+1) * income - b.geode,
					income + new Resources(0, 0, 0, 1)
				),
		};
		best = Math.Max(best, subBest);
	}

	return best;

	bool resourceCheck(int botsLeft, int stepLimit, int resourceCost, int resStock, int resIncome) {
		// If we would build a bot requiring resource X this step and every following one until we have
		// built another botsLeft of those, can we afford that on stocked resources + income alone?
		// Also cap on the step limit, as we don't build beyond that.
		var botCount = Math.Min(botsLeft, stepsLeft-stepLimit+1);

		var totalCost = botCount*resourceCost;
		var totalResources = resStock + resIncome*(botCount-1);

		// It's still useful to build a resource producer if we can't produce all buildings from stock and income
		return totalResources < totalCost;
	}

	// Returns after how many steps the build can be bought, or infinity if it's never affordable.
	int affordableInSteps(Resources r) {
		var oreSteps = r.ore > 0
			? (income.ore > 0 ? (int)Math.Ceiling((double)Math.Max(0, r.ore-resources.ore)/income.ore) : int.MaxValue)
			: -1;
		var claySteps = r.clay > 0
			? (income.clay > 0 ? (int)Math.Ceiling((double)Math.Max(0, r.clay-resources.clay)/income.clay) : int.MaxValue)
			: -1;
		var obsidianSteps = r.obsidian > 0
			? (income.obsidian > 0 ? (int)Math.Ceiling((double)Math.Max(0, r.obsidian-resources.obsidian)/income.obsidian) : int.MaxValue)
			: -1;

		return Math.Max(Math.Max(oreSteps, claySteps), obsidianSteps);
	}
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
	public static Resources operator +(Resources a, Resources b) {
		return new Resources(a.ore+b.ore, a.clay+b.clay, a.obsidian+b.obsidian, a.geode+b.geode);
	}

	public static Resources operator -(Resources a, Resources b) {
		return new Resources(a.ore-b.ore, a.clay-b.clay, a.obsidian-b.obsidian, a.geode-b.geode);
	}

	public static Resources operator *(Resources a, int b) {
		return new Resources(a.ore*b, a.clay*b, a.obsidian*b, a.geode*b);
	}

	public static Resources operator *(int a, Resources b) {
		return b*a;
	}

	public Resources add(int ore, int clay, int obsidian, int geode) {
		return new Resources(this.ore+ore, this.clay+clay, this.obsidian+obsidian, this.geode+geode);
	}
}