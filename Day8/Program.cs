//part1();
part2();

static void part1() {
	var heights = new List<List<Tree>>();
	foreach (var line in File.ReadLines("input.txt")) {
		heights.Add(
			(from c in line select new Tree(c)).ToList()
		);
	}

	var width = heights[0].Count;
	var height = heights.Count;
	// Horizontal scans
	for (int y = 0; y < height; y++) {
		scanRowOrCol(0,       y,  1, 0, width);
		scanRowOrCol(width-1, y, -1, 0, width);
	}
	// Vertical scans
	for (int x = 0; x < width; x++) {
		scanRowOrCol(x, 0,        0,  1, height);
		scanRowOrCol(x, height-1, 0, -1, height);
	}

	var visible = heights.Select(row => row.Select(h => h.seen ? 1 : 0).Sum()).Sum();
	Console.WriteLine(visible);

	void scanRowOrCol(int startX, int startY, int dx, int dy, int size) {
		var highest = 0;
		var x = startX;
		int y = startY;
		heights[y][x].seen = true;
		for (int i = 0; i < size; i++) {
			if (heights[y][x].h > highest) {
				heights[y][x].seen = true;
				highest = heights[y][x].h;
			}
			x+= dx;
			y+= dy;
		}
	}
}

static void part2() {
	var heights = new List<List<Tree>>();
	foreach (var line in File.ReadLines("input.txt")) {
		heights.Add(
			(from c in line select new Tree(c)).ToList()
		);
	}

	var width = heights[0].Count;
	var height = heights.Count;

	var maxScore = 0;
	for (int y = 0; y < height; y++) {
		for (int x = 0; x < width; x++) {
			var curr = heights[y][x].h;
			var score = 1;
			// Horizontal scans
			score*= scanRowOrCol(x+1, y,  1, 0, width-1-x, curr);
			score*= scanRowOrCol(x-1, y, -1, 0, x, curr);
			// Vertical scans
			score*= scanRowOrCol(x, y+1, 0,  1, height-1-y, curr);
			score*= scanRowOrCol(x, y-1, 0, -1, y, curr);

			maxScore = Math.Max(maxScore, score);
		}
	}

	Console.WriteLine(maxScore);

	int scanRowOrCol(int startX, int startY, int dx, int dy, int size, int maxHeight) {
		var x = startX;
		int y = startY;
		var count = 0;
		for (int i = 0; i < size; i++) {
			count++;
			if (heights[y][x].h >= maxHeight) {
				return count;
			}
			x+= dx;
			y+= dy;
		}
		return count;
	}
}

class Tree {
	public readonly int h;
	public bool seen;

	public Tree(char c) {
		this.h = int.Parse(c.ToString());
	}
}