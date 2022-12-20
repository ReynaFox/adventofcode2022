part1();
part2();

static void part1() {
	var numbers = new List<Num>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		numbers.Add(new Num(int.Parse(line), numbers.Count));
	}

	// idxRef maps from the original processing order to where a number is now.
	var idxRef = new int[numbers.Count];
	for (int i = 0; i < idxRef.Length; i++) {
		idxRef[i] = i;
	}

	Console.WriteLine(string.Join(" ", numbers.Select(n => n.num)));
	for (int i = 0; i < numbers.Count; i++) {
		var srcIdx = idxRef[i];
		var item = numbers[srcIdx];
		var targetIdx = (srcIdx + (int)item.num) % (numbers.Count-1);
		if (targetIdx <= 0) targetIdx += numbers.Count-1;
		//Console.WriteLine($"Move {item.num} to {targetIdx}");

		// Move
		if (srcIdx != targetIdx) {
			numbers.RemoveAt(srcIdx);
			numbers.Insert(targetIdx, item);
		}
		if (srcIdx < targetIdx) {
			for (int j = srcIdx; j <= targetIdx; j++) {
				idxRef[numbers[j].origIdx] = j;
			}
		} else if (srcIdx > targetIdx) {
			for (int j = srcIdx; j <= targetIdx; j++) {
				idxRef[numbers[j].origIdx] = j;
			}
		}

		//Console.WriteLine(string.Join(" ", numbers.Select(n => n.num)));
	}

	var zeroIdx = numbers.FindIndex(n => n.num == 0);

	var num1 = numbers[(zeroIdx+1000) % numbers.Count].num;
	var num2 = numbers[(zeroIdx+2000) % numbers.Count].num;
	var num3 = numbers[(zeroIdx+3000) % numbers.Count].num;
	Console.WriteLine($"1000: {num1}");
	Console.WriteLine($"2000: {num2}");
	Console.WriteLine($"3000: {num3}");

	Console.WriteLine(num1+num2+num3);
}

static void part2() {
	const long key = 811_589_153L;
	var numbers = new List<Num>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		numbers.Add(new Num(long.Parse(line)*key, numbers.Count));
	}

	// idxRef maps from the original processing order to where a number is now.
	var idxRef = new int[numbers.Count];
	for (int i = 0; i < idxRef.Length; i++) {
		idxRef[i] = i;
	}

	Console.WriteLine("Initial: "+string.Join(" ", numbers.Select((n, i) => $"[{i}] {n.num}")));
	for (int rep = 0; rep < 10; rep++) {
		for (int i = 0; i < numbers.Count; i++) {
			var srcIdx = idxRef[i];
			var item = numbers[srcIdx];
			var targetIdx = (int)((srcIdx + item.num) % (numbers.Count-1));
			if (targetIdx <= 0) targetIdx += numbers.Count-1;

			// Move
			if (srcIdx != targetIdx) {
				numbers.RemoveAt(srcIdx);
				numbers.Insert(targetIdx, item);
			}
			if (srcIdx < targetIdx) {
				for (int j = srcIdx; j <= targetIdx; j++) {
					idxRef[numbers[j].origIdx] = j;
				}
			} else if (srcIdx > targetIdx) {
				for (int j = targetIdx; j <= srcIdx; j++) {
					idxRef[numbers[j].origIdx] = j;
				}
			}
		}
		//Console.WriteLine($"After round {rep+1}: "+string.Join(" ", numbers.Select(n => n.num)));
	}

	var zeroIdx = numbers.FindIndex(n => n.num == 0);

	var num1 = numbers[(zeroIdx+1000) % numbers.Count].num;
	var num2 = numbers[(zeroIdx+2000) % numbers.Count].num;
	var num3 = numbers[(zeroIdx+3000) % numbers.Count].num;
	Console.WriteLine($"1000: {num1}");
	Console.WriteLine($"2000: {num2}");
	Console.WriteLine($"3000: {num3}");

	Console.WriteLine(num1+num2+num3);
}

record Num(long num, int origIdx);