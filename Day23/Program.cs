part1();
part2();


static void part1() {
	var elves = new List<Elf>();
	var y = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		for (int x = 0; x < line.Length; x++) {
			if (line[x] == '#') elves.Add(new Elf(x, y));
		}
		y++;
	}

	var targets = new Dictionary<(int x, int y), int>();
	var dirOfs = 0;
	for (int round = 0; round < 10; round++) {
		doMoves(elves, targets, dirOfs);
		dirOfs = (dirOfs+1)%4;
	}

	var minX = elves[0].x;
	var maxX = elves[0].x;
	var minY = elves[0].y;
	var maxY = elves[0].y;
	foreach (var elf in elves) {
		minX = Math.Min(minX, elf.x);
		minY = Math.Min(minY, elf.y);
		maxX = Math.Max(maxX, elf.x);
		maxY = Math.Max(maxY, elf.y);
	}

	Console.WriteLine((maxX-minX+1) * (maxY-minY+1) - elves.Count);
}

static void part2() {
	var elves = new List<Elf>();
	var y = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		for (int x = 0; x < line.Length; x++) {
			if (line[x] == '#') elves.Add(new Elf(x, y));
		}
		y++;
	}

	var targets = new Dictionary<(int x, int y), int>();
	var dirOfs = 0;
	var round = 1;
	while (true) {
		if (!doMoves(elves, targets, dirOfs)) {
			Console.WriteLine(round);
			break;
		}
		round++;
		dirOfs = (dirOfs+1)%4;
	}
}

static bool doMoves(List<Elf> elves, Dictionary<(int,int), int> targets, int dirOffset) {
	var directions = new[] {
		new[] {(-1, -1), ( 0, -1), ( 1, -1)}, // N
		new[] {(-1,  1), ( 0,  1), ( 1,  1)}, // S
		new[] {(-1, -1), (-1,  0), (-1,  1)}, // W
		new[] {( 1, -1), ( 1,  0), ( 1,  1)}  // E
	};


	targets.Clear();
	foreach (var elf in elves) {
		targets[(elf.x, elf.y)] = -1;
	}
	// Prepare phase
	foreach (var elf in elves) {
		elf.hasTarget = false;
		prepMove(elf, dirOffset);
	}

	// Execute phase
	var didMove = false;
	foreach (var elf in elves) {
		if (elf.hasTarget && targets[(elf.tgtX, elf.tgtY)] == 1) {
			elf.x = elf.tgtX;
			elf.y = elf.tgtY;
			didMove = true;
		}
	}
	return didMove;

	void prepMove(Elf elf, int dirOffset) {
		var checks = new int[3, 3];
		var hasNeighbor = false;
		for (int y = -1; y <= 1; y++)
		for (int x = -1; x <= 1; x++) {
			if ((x != 0 || y != 0) && targets.TryGetValue((elf.x+x, elf.y+y), out var val) && val == -1) {
				checks[x+1, y+1] = 1;
				hasNeighbor = true;
			}
		}

		if (!hasNeighbor) return;

		for (int i = 0; i < directions.Length; i++) {
			var dir = directions[(i+dirOffset)%directions.Length];
			var select = true;
			foreach (var d in dir) {
				if (checks[d.Item1+1, d.Item2+1] == 1) {
					select = false;
					break;
				}
			}
			if (select) {
				elf.hasTarget = true;
				elf.tgtX = elf.x+dir[1].Item1;
				elf.tgtY = elf.y+dir[1].Item2;
				if (targets.ContainsKey((elf.tgtX, elf.tgtY))) {
					targets[(elf.tgtX, elf.tgtY)]++;
				} else {
					targets[(elf.tgtX, elf.tgtY)] = 1;
				}
				return;
			}
		}
	}
}

static void dumpField(List<Elf> elves, int exMinX = 0, int exMinY = 0) {
	var minX = exMinX;
	var maxX = elves[0].x;
	var minY = exMinY;
	var maxY = elves[0].y;
	foreach (var elf in elves) {
		minX = Math.Min(minX, elf.x);
		minY = Math.Min(minY, elf.y);
		maxX = Math.Max(maxX, elf.x);
		maxY = Math.Max(maxY, elf.y);
	}
	var width = maxX-minX+1;
	var lines = new List<char[]>();
	for (int y = minY; y <= maxY; y++) {
		lines.Add(new string('.', width).ToCharArray());
	}
	foreach (var elf in elves) {
		lines[elf.y-minY][elf.x-minX] = '#';
	}
	foreach (var line in lines) {
		Console.WriteLine(string.Join("", line));
	}
}

class Elf {
	public int x, y;
	public bool hasTarget;
	public int tgtX, tgtY;

	public Elf(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override string ToString() {
		return $"Elf @({x}, {y}), "+(hasTarget ? $"-> ({tgtX}, {tgtY})" : "no move");
	}
}