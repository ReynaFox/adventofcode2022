//part1();
part2();

static void part1() {
	var sum = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		var part1 = line[0..(line.Length/2)];
		var part2 = line[(line.Length/2)..^0];
		var shared = part1.Intersect(part2).First();

		var prio = shared >= 'a' ? shared-'a'+1 : shared-'A'+27;

		sum += prio;
	}

	Console.WriteLine(sum);
}

static void part2() {
	var sum = 0;

	var lineInGroup = 0;
	IEnumerable<char>? intersect = null;
	foreach (var line in File.ReadLines("input.txt")) {
		intersect = intersect == null
			? line
			: intersect.Intersect(line);

		lineInGroup++;
		if (lineInGroup == 3) {
			var shared = intersect.First();

			var prio = shared >= 'a' ? shared-'a'+1 : shared-'A'+27;
			sum += prio;

			intersect = null;
			lineInGroup = 0;
		}
	}

	Console.WriteLine(sum);
}