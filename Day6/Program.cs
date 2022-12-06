part1(4);
part1(14); // Part 2

static void part1(int length) {
	foreach (var line in File.ReadLines("input.txt")) {
		for (int i = 0; i < line.Length-length; i++) {
			if (new HashSet<char>(line[i..(i+length)]).Count == length) {
				Console.WriteLine(i+length);
				break;
			}
		}
	}
}