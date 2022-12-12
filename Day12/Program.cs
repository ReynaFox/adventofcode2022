part1();
part2();

static void part1() {
	var heights = new List<List<int>>();
	var start = new Point(0, 0);
	var end = new Point(0, 0);

	var y = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		heights.Add(
			line.Select((c, i) => {
				if (c == 'S') {
					start = new Point(i, y);
					return 0;
				} else if (c == 'E') {
					end = new Point(i, y);
					return 'z'-'a';
				} else {
					return c-'a';
				}
			}).ToList()
		);
		y++;
	}

	Console.WriteLine(findPath(heights, start, end));
}

static void part2() {
	var heights = new List<List<int>>();
	var end = new Point(0, 0);

	var y = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		heights.Add(
			line.Select((c, i) => {
				if (c == 'S') {
					return 0;
				} else if (c == 'E') {
					end = new Point(i, y);
					return 'z'-'a';
				} else {
					return c-'a';
				}
			}).ToList()
		);
		y++;
	}

	var w = heights[0].Count;
	var h = heights.Count;
	var best = int.MaxValue;
	for (y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
			if (heights[y][x] != 0) continue;

			best = Math.Min(best, findPath(heights, new Point(x, y), end));
		}
	}
	Console.WriteLine(best);
}


static int findPath(List<List<int>> grid, Point start, Point end) {
	var w = grid[0].Count;
	var h = grid.Count;

	var shortest = new int[h, w];
	var origins = new Point?[h, w];
	var queue = new Queue<Point>();
	queue.Enqueue(start);

	for (int y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
			shortest[y, x] = int.MaxValue;
		}
	}


	while (true) {
		if (queue.Count == 0) {
			return int.MaxValue;
		}
		var p = queue.Dequeue();
		if (p.x == end.x && p.y == end.y) break;

		handle(p, p.x+1, p.y);
		handle(p, p.x-1, p.y);
		handle(p, p.x, p.y+1);
		handle(p, p.x, p.y-1);
	}

	var path = new char[h, w];
	for (int y = 0; y < h; y++) {
		for (int x = 0; x < w; x++) {
			path[y, x] = '.';
		}
	}

	// Count steps
	var steps = 0;
	var trace = end;
	while (trace != start) {
		steps++;
		var orig = origins[trace.y, trace.x];
		trace = orig;
	}
	return steps;

	void handle(Point curr, int x, int y) {
		if (x < 0 || x >= w || y < 0 || y >= h) return; // Out of bounds
		if (grid[y][x]-grid[curr.y][curr.x] >= 2) return; // Going up too much

		if (shortest[curr.y, curr.x]+1 < shortest[y, x]) {
			origins[y, x] = curr;
			shortest[y, x] = shortest[curr.y, curr.x]+1;
			var n = new Point(x, y);
			if (!queue.Contains(n)) {
				queue.Enqueue(n);
			}
		}
	}
}

record Point (int x, int y) {
	public readonly int x = x;
	public readonly int y = y;
}
