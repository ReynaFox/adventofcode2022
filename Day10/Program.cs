part1();
part2();

static void part1() {
	var cycles = 0;
	var regX = 1;
	var sum = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(' ');
		var op = parts[0];
		switch (op) {
		case "noop":
			cycle();
			break;
		case "addx":
			var arg = int.Parse(parts[1]);
			cycle();
			cycle();
			regX += arg;
			break;
		}
	}

	Console.WriteLine(sum);

	void cycle() {
		cycles++;
		if ((cycles-20) % 40 == 0) {
			sum += cycles*regX;
		}
	}
}

static void part2() {
	var cycles = 0;
	var regX = 1;
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(' ');
		var op = parts[0];
		switch (op) {
		case "noop":
			draw();
			break;
		case "addx":
			var arg = int.Parse(parts[1]);
			draw();
			draw();
			regX += arg;
			break;
		}
	}

	void draw() {
		Console.Write(
			cycles-1 <= regX && regX <= cycles+1
				? '#'
				: '.'
		);

		cycles++;
		if (cycles >= 40) {
			cycles -= 40;
			Console.WriteLine();
		}
	}
}