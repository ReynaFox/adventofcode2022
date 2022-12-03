part1();
part2();

static void part1() {
	var maxCals = 0;
	var groupSum = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) {
			// End of group: check for new maximum & reset
			maxCals = Math.Max(maxCals, groupSum);
			groupSum = 0;
		} else {
			// Existing group: add count
			if (int.TryParse(line, out var cal)) {
				groupSum += cal;
			}
		}
	}

	// Close final group
	maxCals = Math.Max(maxCals, groupSum);

	Console.WriteLine(maxCals);
}

static void part2() {
	var calList = new List<int>();
	var groupSum = 0;

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) {
			// End of group: Add group value & reset
			calList.Add(groupSum);
			groupSum = 0;
		} else {
			// Existing group: add count
			if (int.TryParse(line, out var cal)) {
				groupSum += cal;
			}
		}
	}
	 
	// Close final group
	calList.Add(groupSum);

	Console.WriteLine(calList.OrderDescending().Take(3).Sum());
}