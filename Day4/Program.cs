using System.Text.RegularExpressions;

part1();
part2();

static void part1() {
	var count = 0;

	var regex = new Regex(@"^(\d+)-(\d+),(\d+)-(\d+)");

	foreach (var line in File.ReadLines("input.txt")) {
		var groups = regex.Match(line).Groups;
		var range1Min = int.Parse(groups[1].Value);
		var range1Max = int.Parse(groups[2].Value);
		var range2Min = int.Parse(groups[3].Value);
		var range2Max = int.Parse(groups[4].Value);

		if (
			(range1Min >= range2Min && range1Max <= range2Max) ||
			(range2Min >= range1Min && range2Max <= range1Max)
		) {
			count++;
		}
	}

	Console.WriteLine(count);
}

static void part2() {
	var count = 0;

	var regex = new Regex(@"^(\d+)-(\d+),(\d+)-(\d+)");

	foreach (var line in File.ReadLines("input.txt")) {
		var groups = regex.Match(line).Groups;
		var range1Min = int.Parse(groups[1].Value);
		var range1Max = int.Parse(groups[2].Value);
		var range2Min = int.Parse(groups[3].Value);
		var range2Max = int.Parse(groups[4].Value);

		if (
			(range1Min >= range2Min && range1Min <= range2Max) ||
			(range1Max >= range2Min && range1Max <= range2Max) ||
			(range2Min >= range1Min && range2Min <= range1Max) ||
			(range2Max >= range1Min && range2Max <= range1Max)
		) {
			count++;
		}
	}

	Console.WriteLine(count);
}