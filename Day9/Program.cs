part1();
part2();

static void part1() {
	var positions = new HashSet<(int,int)>();
	var headPos = (x: 0, y: 0);
	var tailPos = (x: 0, y: 0);
	positions.Add(tailPos);
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(' ');
		var action = parts[0];
		var num = int.Parse(parts[1]);

		for (int i = 0; i < num; i++) {
			switch (action) {
			case "L": headPos.x -= 1; break;
			case "R": headPos.x += 1; break;
			case "D": headPos.y -= 1; break;
			case "U": headPos.y += 1; break;
			}

			if (Math.Abs(headPos.x-tailPos.x) > 1 || Math.Abs(headPos.y-tailPos.y) > 1) {
				tailPos.x += Math.Clamp(headPos.x-tailPos.x, -1, 1);
				tailPos.y += Math.Clamp(headPos.y-tailPos.y, -1, 1);
				positions.Add(tailPos);
			}
		}
	}

	Console.WriteLine(positions.Count);
}

static void part2() {
	var positions = new HashSet<(int,int)>();
	var rope = new (int x, int y)[10];
	positions.Add(rope[9]);
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(' ');
		var action = parts[0];
		var num = int.Parse(parts[1]);

		for (int i = 0; i < num; i++) {
			switch (action) {
			case "L": rope[0].x -= 1; break;
			case "R": rope[0].x += 1; break;
			case "D": rope[0].y -= 1; break;
			case "U": rope[0].y += 1; break;
			}

			for (int j= 1; j < rope.Length; j++) {
				if (Math.Abs(rope[j].x-rope[j-1].x) > 1 || Math.Abs(rope[j].y-rope[j-1].y) > 1) {
					rope[j].x += Math.Clamp(rope[j-1].x-rope[j].x, -1, 1);
					rope[j].y += Math.Clamp(rope[j-1].y-rope[j].y, -1, 1);
				}
			}
			positions.Add(rope[9]);
		}
	}

	Console.WriteLine(positions.Count);
}