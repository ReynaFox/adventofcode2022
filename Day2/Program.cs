part1();
part2();

static void part1() {
	var scores = new Dictionary<string, int>() {
		{ "A", 1 },
		{ "B", 2 },
		{ "C", 3 },
		{ "X", 1 },
		{ "Y", 2 },
		{ "Z", 3 },
	};

	var score = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(' ');
		var opponent = scores[parts[0]];
		var mine = scores[parts[1]];

		var outcomeScore = 0;
		if (opponent == mine) {
			outcomeScore = 3;
		}else if (mine==3 && opponent==1) {
			outcomeScore = 0;
		} else if ((mine==1 && opponent==3) || (mine > opponent)) {
			outcomeScore = 6;
		}

		score += outcomeScore + mine;
	}

	Console.WriteLine(score);
}

static void part2() {
	var scores = new Dictionary<string, int>() {
		{ "A", 1 },
		{ "B", 2 },
		{ "C", 3 },
		{ "X", 0 },
		{ "Y", 3 },
		{ "Z", 6 },
	};

	var score = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(' ');
		var opponent = scores[parts[0]];
		var targetResult = parts[1];

		var outcomeScore = scores[targetResult];
		var mine = targetResult[0] switch {
			'X' => opponent == 1 ? 3 : opponent-1,
			'Y' => opponent,
			'Z' => opponent == 3 ? 1 : opponent+1
		};

		score += outcomeScore + mine;
	}

	Console.WriteLine(score);
}