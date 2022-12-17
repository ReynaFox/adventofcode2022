using System.Linq;
using System.Text.RegularExpressions;

//part1();
part2();

static void part1() {
	var blocks = new[] {
		new[] { "####" },

		new [] {
			".#.",
			"###",
			".#."
		},

		new [] {
			"..#",
			"..#",
			"###",
		},

		new [] {
			"#",
			"#",
			"#",
			"#",
		},

		new [] {
			"##",
			"##",
		}
	};

	string movements = "";
	foreach (var line in File.ReadLines("input.txt")) {
		movements = line;
		break;
	}

	// 13: The max number of height added per cycle of blocks
	// 405: ceil(2022/5)
	// 20: Some extra headroom.
	var fieldWidth = 7;
	var field = new bool[fieldWidth, 13*405+20];

	var nextBlock = 0;
	var moveIdx = 0;
	var topY = -1; // Since y=0 is open: the y with the highest solid place is y=-1
	for (int i = 0; i < 2022; i++) {
		doBlock(nextBlock);
		nextBlock = (nextBlock + 1) % blocks.Length;
		//dumpField(field, topY+5);
	}

	dumpField(field, topY+5);

	Console.WriteLine(topY+1);

	void doBlock(int type) {
		// A block's (x,y) coordinate is the lower left corner of the block.
		var x = 2;
		var y = topY+4;
		var block = blocks[type];

		while (true) {
			var dX = movements[moveIdx] == '<' ? -1 : 1;
			moveIdx = (moveIdx+1)%movements.Length;
			if (fits(block, x+dX, y)) x += dX;
			if (fits(block, x, y-1)) y--;
			else {
				set(block, x, y);
				topY = Math.Max(topY, y+block.Length-1);
				return;
			}
		}
	}

	bool fits(string[] block, int pX, int pY) {
		if (pX < 0 || pX+block[0].Length > fieldWidth || pY < 0) return false;
		var blockH = block.Length;
		for (int y = 0; y < blockH; y++) {
			var line = block[y];
			for (int x = 0; x < line.Length; x++) {
				if (line[x] == '.') continue;
				if (field[x+pX, pY+blockH-1-y]) return false;
			}
		}
		return true;
	}

	void set(string[] block, int pX, int pY) {
		var blockH = block.Length;
		for (int y = 0; y < blockH; y++) {
			var line = block[y];
			for (int x = 0; x < line.Length; x++) {
				if (line[x] == '.') continue;
				field[x+pX, pY+blockH-1-y] = true;
			}
		}
	}

	void dumpField(bool[,] field, int maxY) {
		var w = field.GetLength(0);
		var h =  field.GetLength(1);
		for (int y = maxY; y >= 0; y--) {
			Console.Write("|");
			for (int x = 0; x < w; x++) {
				Console.Write(field[x,y] ? '#' : '.');
			}
			Console.WriteLine("|");
		}
		Console.Write("+");
		for (int x = 0; x < w; x++) {
			Console.Write("-");
		}
		Console.WriteLine("+");
		Console.WriteLine();
	}
}

static void part2() {
	var blocks = new[] {
		new[] { 0b1111 },

		new [] {
			0b010,
			0b111,
			0b010
		},

		new [] {
			// This one appears flipped because more significant bits are to the right.
			0b100,
			0b100,
			0b111
		},

		new [] {
			1,
			1,
			1,
			1
		},

		new [] {
			0b11,
			0b11
		}
	};



	string movementsStr = "";
	foreach (var line in File.ReadLines("input.txt")) {
		movementsStr = line;
		break;
	}
	var movements = new int[movementsStr.Length];
	for (int i = 0; i < movementsStr.Length; i++) {
		movements[i] = movementsStr[i] == '<' ? -1 : 1;
	}

	var field = new List<byte>();
	
	var nextBlock = 0;
	var moveIdx = 0;
	var topY = -1; // Since y=0 is open: the y with the highest solid place is y=-1
	var totalRemoved = 0L;
	var moveStartHistory = new HashSet<int>[movements.Length];
	for (int i = 0; i < movements.Length; i++) {
		moveStartHistory[i] = new HashSet<int>();
	}
	var cycle = 0;
	var cycleStartHeight = 0L;
	var cycleStartBlocks = 0L;
	//var maxBlocks = 2022;
	var maxBlocks = 1_000_000_000_000L;
	for (long i = 0; i < maxBlocks; i++) {
		var block = blocks[nextBlock];
		while (field.Count <= topY+4+block.Length-1) field.Add(0x80);

		if (cycle <= 1) {
			if (moveStartHistory[moveIdx].Contains(nextBlock)) {
				if (cycle < 1) {
					// The first cycle has completed, clear everything as the initial ground may have interfered.
					cycle++;
					for (int j = 0; j < movements.Length; j++) {
						moveStartHistory[j].Clear();
					}
					cycleStartHeight = topY+1+totalRemoved;
					cycleStartBlocks = i;
				} else if (cycle == 1) {
					cycle++;
					var cycleEndHeight = topY+1+totalRemoved;
					var heightPerCycle = cycleEndHeight - cycleStartHeight;
					var blocksPerCycle = i-cycleStartBlocks;

					var remainingCycles = (maxBlocks-i)/blocksPerCycle;
					Console.WriteLine($"Skipping {remainingCycles} cycles of {blocksPerCycle} blocks");
					i += remainingCycles*blocksPerCycle;
					totalRemoved += remainingCycles*heightPerCycle;
					if (i >= maxBlocks) break;
				}
			}
			moveStartHistory[moveIdx].Add(nextBlock);
		}

		doBlock(block);
		nextBlock = (nextBlock + 1) % blocks.Length;

		if (i % 1000 == 0) {
			shiftField();
		}
		//dumpField(field, field.Count-1);
	}

	Console.WriteLine($"Next block that would dropped: {nextBlock}");

	Console.WriteLine();
	Console.WriteLine(topY+1+totalRemoved);

	void shiftField() {
		byte scan = 0;
		int minY = -1;
		for (int y = field.Count-1; y >= 0; y--) {
			scan |= field[y];
			if (scan == 0xFF) {
				minY = y;
				break;
			}
		}
		if (minY <= 1) return;
		
		// Drop everything so that the new minY would be 1
		var sub = minY-1;
		totalRemoved += sub;
		field.RemoveRange(0, sub);
		topY -= sub;
	}

	void doBlock(int[] block) {
		// A block's (x,y) coordinate is the lower left corner of the block.
		var x = 2;
		var y = topY+4;

		while (true) {
			var dX = movements[moveIdx];
			moveIdx = (moveIdx+1)%movements.Length;
			if (fits(block, x+dX, y)) x += dX;
			if (fits(block, x, y-1)) y--;
			else {
				set(block, x, y);
				topY = Math.Max(topY, y+block.Length-1);
				return;
			}
		}
	}

	bool fits(int[] block, int pX, int pY) {
		if (pX < 0 || pY < 0) return false;
		var blockH = block.Length;
		var yOfs = pY+blockH-1;
		for (int y = 0; y < blockH; y++) {
			var line = block[y];
			if ((field[yOfs-y] & (line << pX)) != 0) return false;
		}
		return true;
	}

	void set(int[] block, int pX, int pY) {
		var blockH = block.Length;
		var yOfs = pY+blockH-1;
		for (int y = 0; y < blockH; y++) {
			var line = block[y];
			field[yOfs-y] |= (byte)(line << pX);
		}
	}

	void dumpField(List<byte> field, int maxY) {
		var w = 7;
		var h =  field.Count;
		for (int y = maxY; y >= 0; y--) {
			Console.Write("|");
			for (int x = 0; x < w; x++) {
				Console.Write((field[y] & (1<<x)) != 0 ? '#' : '.');
			}
			Console.WriteLine("|");
		}
		Console.Write("+");
		for (int x = 0; x < w; x++) {
			Console.Write("-");
		}
		Console.WriteLine("+");
		Console.WriteLine();
	}
}
