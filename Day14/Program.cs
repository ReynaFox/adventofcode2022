part1();
part2();

const int empty = 0;
const int used = 1;
const int edge = 2; 

static void part1() {
	var paths = new List<List<Point>>();

	var start = new Point(500, 0);

	var boundsMin = new Point(start);
	var boundsMax = new Point(start);
	foreach (var line in File.ReadLines("input.txt")) {
		var points = (from s in line.Split("->") select Point.fromString(s)).ToList();
		foreach (var p in points) {
			boundsMin.x = Math.Min(boundsMin.x, p.x);
			boundsMin.y = Math.Min(boundsMin.y, p.y);
			boundsMax.x = Math.Max(boundsMax.x, p.x);
			boundsMax.y = Math.Max(boundsMax.y, p.y);
		}
		paths.Add(points);
	}

	boundsMax.x++;
	boundsMax.y++;
	boundsMin.x--;

	var grid = drawLines(paths, boundsMin, boundsMax);
	// Boundaries
	for (int x = 0; x <= boundsMax.x-boundsMin.x; x++) {
		grid[x, boundsMax.y-boundsMin.y] = edge;
	}
	for (int y = 0; y <= boundsMax.y-boundsMin.y; y++) {
		grid[0, y] = edge;
		grid[boundsMax.x-boundsMin.x, y] = edge;
	}

	var counter = 0;
	var relStart = new Point(start.x-boundsMin.x, start.y-boundsMin.y);
	while (true) {
		if (!simulate(relStart, grid)) break;
		counter++;
	}

	Console.WriteLine(counter);
}

static void part2() {
	var paths = new List<List<Point>>();

	var start = new Point(500, 0);

	var boundsMin = new Point(start);
	var boundsMax = new Point(start);
	foreach (var line in File.ReadLines("input.txt")) {
		var points = (from s in line.Split("->") select Point.fromString(s)).ToList();
		foreach (var p in points) {
			boundsMin.x = Math.Min(boundsMin.x, p.x);
			boundsMin.y = Math.Min(boundsMin.y, p.y);
			boundsMax.x = Math.Max(boundsMax.x, p.x);
			boundsMax.y = Math.Max(boundsMax.y, p.y);
		}
		paths.Add(points);
	}

	boundsMax.y+= 2;
	// Sand can fall boundsMax.y-1 tiles down before encountering the floor, so it'll always settle there.
	// In the worst case, we need to check an x coord of 500[+-]boundsMax.y
	boundsMin.x = Math.Min(boundsMin.x, 500-boundsMax.y);
	boundsMax.x = Math.Max(boundsMax.x, 500+boundsMax.y);

	var grid = drawLines(paths, boundsMin, boundsMax);
	// Floor
	for (int x = 0; x <= boundsMax.x-boundsMin.x; x++) {
		grid[x, boundsMax.y-boundsMin.y] = used;
	}

	var counter = 0;
	var relStart = new Point(start.x-boundsMin.x, start.y-boundsMin.y);
	while (true) {
		if (!simulate(relStart, grid)) break;
		counter++;
		if (grid[relStart.x, relStart.y] == used) break;
	}

	Console.WriteLine(counter);
}

static int[,] drawLines(List<List<Point>> paths, Point boundsMin, Point boundsMax) {
	var grid = new int[boundsMax.x-boundsMin.x+1, boundsMax.y-boundsMin.y+1];
	foreach (var path in paths) {
		for (int i = 1; i < path.Count; i++) {
			var from = path[i-1];
			var to = path[i];
			if (from.y == to.y) {
				for (int x = Math.Min(from.x, to.x); x <= Math.Max(from.x, to.x); x++) {
					grid[x-boundsMin.x, from.y-boundsMin.y] = used;
				}
			} else if (from.x == to.x) {
				for (int y = Math.Min(from.y, to.y); y <= Math.Max(from.y, to.y); y++) {
					grid[from.x-boundsMin.x, y-boundsMin.y] = used;
				}
			} else {
				throw new Exception("Diagonal");
			}
		}
	}
	return grid;
}

// Returns false if not settled at the end, ie it fell into the void.
static bool simulate(Point startP, int[,] grid) {
	var p = new Point(startP);

	do {
		if (grid[p.x, p.y+1] != used) {
			p.y++;
		} else if (grid[p.x-1, p.y+1] != used) {
			p.x--;
			p.y++;
		} else if (grid[p.x+1, p.y+1] != used) {
			p.x++;
			p.y++;
		} else {
			// Settled
			grid[p.x, p.y] = used;
			break;
		}
	} while (grid[p.x, p.y] != edge);

	return grid[p.x, p.y] != edge;
}

class Point {
	public int x, y;

	public static Point fromString(string s) {
		var parts = s.Trim().Split(',');
		return new Point(int.Parse(parts[0]), int.Parse(parts[1]));
	}

	public Point(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public Point(Point p) {
		x = p.x;
		y = p.y;
	}
}