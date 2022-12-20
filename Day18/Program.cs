//part1();

using System.Reflection.Metadata.Ecma335;

part2();

static void part1() {
	var coords = new List<Point>();
	var max = new Point(0, 0, 0);
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(",");
		var p = new Point(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
		coords.Add(p);
		max.x = Math.Max(max.x, p.x);
		max.y = Math.Max(max.y, p.y);
		max.z = Math.Max(max.z, p.z);
	}

	var grid = new bool[max.x+3, max.y+3, max.z+3];
	foreach (var c in coords) {
		grid[c.x+1, c.y+1, c.z+1] = true; // Offset to allow for input coordinates 0 to still have a valid -1
	}

	var count = 0;
	foreach (var c in coords) {
		var x = c.x+1;
		var y = c.y+1;
		var z = c.z+1;

		if (!grid[x-1, y, z]) count++;
		if (!grid[x+1, y, z]) count++;
		if (!grid[x, y-1, z]) count++;
		if (!grid[x, y+1, z]) count++;
		if (!grid[x, y, z-1]) count++;
		if (!grid[x, y, z+1]) count++;
	}

	Console.WriteLine(count);
}

static void part2() {
	var coords = new List<Point>();
	var max = new Point(0, 0, 0);
	foreach (var line in File.ReadLines("input.txt")) {
		var parts = line.Split(",");
		var p = new Point(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
		coords.Add(p);
		max.x = Math.Max(max.x, p.x);
		max.y = Math.Max(max.y, p.y);
		max.z = Math.Max(max.z, p.z);
	}

	const int air = 0;
	const int solid = 1;
	const int steam = 2;

	var grid = new int[max.x+3, max.y+3, max.z+3];
	foreach (var c in coords) {
		grid[c.x+1, c.y+1, c.z+1] = solid; // Offset to allow for input coordinates 0 to still have a valid -1
	}

	floodfill();

	var count = 0;
	foreach (var c in coords) {
		var x = c.x+1;
		var y = c.y+1;
		var z = c.z+1;

		if (grid[x-1, y, z]==steam) count++;
		if (grid[x+1, y, z]==steam) count++;
		if (grid[x, y-1, z]==steam) count++;
		if (grid[x, y+1, z]==steam) count++;
		if (grid[x, y, z-1]==steam) count++;
		if (grid[x, y, z+1]==steam) count++;
	}

	Console.WriteLine(count);

	void floodfill() {
		var queue = new Queue<Point>();
		queue.Enqueue(new Point(0, 0, 0));

		while (queue.Count > 0) {
			var p = queue.Dequeue();
			if (grid[p.x, p.y, p.z] != air) continue;
			grid[p.x, p.y, p.z] = steam;

			if (p.x > 0 && grid[p.x-1, p.y, p.z] == air) queue.Enqueue(new Point(p.x-1, p.y, p.z));
			if (p.y > 0 && grid[p.x, p.y-1, p.z] == air) queue.Enqueue(new Point(p.x, p.y-1, p.z));
			if (p.z > 0 && grid[p.x, p.y, p.z-1] == air) queue.Enqueue(new Point(p.x, p.y, p.z-1));
			if (p.x < grid.GetLength(0)-1 && grid[p.x+1, p.y, p.z] == air) queue.Enqueue(new Point(p.x+1, p.y, p.z));
			if (p.y < grid.GetLength(1)-1 && grid[p.x, p.y+1, p.z] == air) queue.Enqueue(new Point(p.x, p.y+1, p.z));
			if (p.z < grid.GetLength(2)-1 && grid[p.x, p.y, p.z+1] == air) queue.Enqueue(new Point(p.x, p.y, p.z+1));
		}
	}
}

class Point {
	public int x, y, z;

	public Point(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}
}