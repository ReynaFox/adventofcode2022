using System.Text.RegularExpressions;

part1(true);
part1(false); // Part 2

static void part1(bool reverse) {
	var regexMove = new Regex(@"^move (\d+) from (\d+) to (\d+)");
	var regexStack = new Regex(@"\[(.)\]");

	var lines = File.ReadLines("input.txt");
	var linesEnum = lines.GetEnumerator();
	List<List<char>> stacks = new List<List<char>>();

	string line;
	do {
		linesEnum.MoveNext();
		line = linesEnum.Current;

		var lineStacks = line.Chunk(4).Select(c => new string(c)).ToArray();
		while (stacks.Count < lineStacks.Length) stacks.Add(new List<char>());
		for (int i = 0; i < lineStacks.Length; i++) {
			var item = lineStacks[i];
			var match = regexStack.Match(item);
			if (!match.Success) continue;

			// Insert, so that the bottom most line will be the first index.
			stacks[i].Insert(0, match.Groups[1].Value[0]);
		}
	} while (!string.IsNullOrEmpty(line));

	while (linesEnum.MoveNext()) {
		line = linesEnum.Current;

		var groups = regexMove.Match(line).Groups;
		var amount = int.Parse(groups[1].Value);
		var from = int.Parse(groups[2].Value)-1;
		var to = int.Parse(groups[3].Value)-1;

		var movedStack = stacks[from].TakeLast(amount);
		stacks[to].AddRange(reverse ? movedStack.Reverse() : movedStack);
		stacks[from].RemoveRange(stacks[from].Count-amount, amount);
	}

	foreach (var stack in stacks) {
		Console.Write(stack.Last());
	}
	Console.WriteLine();
}