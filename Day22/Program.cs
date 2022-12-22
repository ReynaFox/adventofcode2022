part1();
part2();

const int rotLeft = -2;
const int rotRight = -1;
const int dirRight = 0;
const int dirDown = 1;
const int dirLeft = 2;
const int dirUp = 3;

static void part1() {
	var directions = new (int x, int y)[] {
		(1, 0),
		(0, 1),
		(-1, 0),
		(0, -1)
	};

	var (map, instructions) = read();
	var width = map[0].Length;
	var height = map.Count;

	var x = map[0].IndexOf('.');
	var y = 0;
	var dir =  dirRight;

	foreach (var item in instructions) {
		if (item == rotLeft) {
			dir = (dir+4-1) % 4;
		} else if (item == rotRight) {
			dir = (dir+1) % 4;
		} else {
			var d = directions[dir];
			for (int i = 0; i < item; i++) {
				var nextX = x;
				var nextY = y;
				do {
					nextX = (nextX+width+d.x)%width;
					nextY = (nextY+height+d.y)%height;
				} while (map[nextY][nextX] == ' ');

				if (map[nextY][nextX] == '#') {
					break;
				}
				x = nextX;
				y = nextY;
			}
		}
	}

	Console.WriteLine($"End pos ({x}, {y}) facing {dir} -> {1000*(y+1) + 4*(x+1) + dir}");
}

static void part2() {
	var directions = new (int x, int y)[] {
		(1, 0),
		(0, 1),
		(-1, 0),
		(0, -1)
	};
	const int rot180 = -3;
	const int rotNone = -4;

	var (map, instructions) = read();
	var width = map[0].Length;
	var height = map.Count;

	/* Example values
	const int faceSize = 4;
	var faceMap = new [] {
		new [] { -1, -1, 0, -1 },
		new [] { 1, 2, 3, -1 },
		new [] { -1, -1, 4, 5 },
	};
	var faces = new [] {
		new Face(2, 0, new Connection(5, rot180), null, new Connection(2, rotLeft), new Connection(1, rot180)),
		new Face(0, 1, null, new Connection(4, rot180), new Connection(5, rotRight), new Connection(0, rot180)),
		new Face(1, 1, null, new Connection(4, rotLeft), null, new Connection(0, rotRight)),
		new Face(2, 1, new Connection(5, rotRight), null, null, null),
		new Face(2, 2, null, new Connection(1, rot180), new Connection(2, rotRight), null),
		new Face(3, 2, new Connection(0, rot180), new Connection(1, rotLeft), null, null)
	};
	*/
	// Real input values
	const int faceSize = 50;
	var faceMap = new [] {
		new [] { -1,  0,  1 },
		new [] { -1,  2, -1 },
		new [] {  3,  4, -1 },
		new [] {  5, -1, -1 },
	};
	var faces = new [] {
		new Face(1, 0, null, null, new Connection(3, rot180), new Connection(5, rotRight)),
		new Face(2, 0, new Connection(4, rot180), new Connection(2, rotRight), null, new Connection(5, rotNone)),
		new Face(1, 1, new Connection(1, rotLeft), null, new Connection(3, rotLeft), null),
		new Face(0, 2, null, null, new Connection(0, rot180), new Connection(2, rotRight)),
		new Face(1, 2, new Connection(1, rot180), new Connection(5, rotRight), null, null),
		new Face(0, 3, new Connection(4, rotLeft), new Connection(1, rotNone), new Connection(0, rotLeft), null)
	};

	var x = map[0].IndexOf('.');
	var y = 0;
	var dir =  dirRight;

	var walk = new char[height][];
	for (int yy = 0; yy < height; yy++) {
		walk[yy] = map[yy].ToCharArray();
	}

	foreach (var item in instructions) {
		if (item == rotLeft) {
			dir = (dir+4-1) % 4;
		} else if (item == rotRight) {
			dir = (dir+1) % 4;
		} else {
			for (int i = 0; i < item; i++) {
				var d = directions[dir];
				walk[y][x] = dir switch {
					dirRight => '>',
					dirDown => 'v',
					dirLeft => '<',
					dirUp => '^'
				};

				var nextX = x+d.x;
				var nextY = y+d.y;
				var nextDir = dir;

				if (nextX < 0 || nextY < 0 || nextX >= width || nextY >= height || map[nextY][nextX] == ' ') {
					// Crossing an edge
					var faceX = x/faceSize;
					var faceY = y/faceSize;
					var oldFace = faceMap[faceY][faceX];
					var faceSubX = x-faceX*faceSize;
					var faceSubY = y-faceY*faceSize;
					var conn = dir switch {
						dirRight => faces[oldFace].right,
						dirDown => faces[oldFace].down,
						dirLeft => faces[oldFace].left,
						dirUp => faces[oldFace].up
					};
					switch (conn.rotation) {
					case rotLeft:
						if (dir == dirUp || dir == dirDown) {
							nextX = faceSize-1-faceSubY;
							nextY = faceSize-1-faceSubX;
						} else {
							nextX = faceSubY;
							nextY = faceSubX;
						}
						nextDir = (dir+4-1) % 4;
						break;
					case rotRight:
						if (dir == dirUp || dir == dirDown) {
							nextX = faceSubY;
							nextY = faceSubX;
						} else {
							nextX = faceSize-1-faceSubY;
							nextY = faceSize-1-faceSubX;
						}
						nextDir = (dir+1) % 4;
						break;
					case rot180:
						if (dir == dirUp || dir == dirDown) {
							nextX = faceSize-1-faceSubX;
							nextY = faceSubY;
						} else {
							nextX = faceSubX;
							nextY = faceSize-1-faceSubY;
						}
						nextDir = (dir+2) % 4;
						break;
					case rotNone:
						if (dir == dirUp || dir == dirDown) {
							nextX = faceSubX;
							nextY = faceSize-1-faceSubY;
						} else {
							nextX = faceSize-1-faceSubX;
							nextY = faceSubY;
						}
						break;
					}
					var nextFace = faces[conn.face];
					nextX += nextFace.x*faceSize;
					nextY += nextFace.y*faceSize;
				}

				if (map[nextY][nextX] == '#') {
					break;
				}
				x = nextX;
				y = nextY;
				dir = nextDir;
			}
		}
	}

	foreach (var line in walk) {
		Console.WriteLine(string.Join("", line));
	}

	Console.WriteLine($"End pos ({x}, {y}) facing {dir} -> {1000*(y+1) + 4*(x+1) + dir}");
}

static (List<string>, List<int>) read() {
	var inMap = true;
	var map = new List<string>();
	var instructions = new List<int>();
	var width = 0;
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) {
			inMap = false;
			continue;
		}
		if (inMap) {
			map.Add(line);
			width = Math.Max(width, line.Length);
		} else {
			var rots = new[] { 'R', 'L' };
			var nextRot = -1;
			while (true) {
				if (nextRot+1 >= line.Length) break;
				var p = line.IndexOfAny(rots, nextRot+1);
				if (p == -1) p = line.Length;

				instructions.Add(int.Parse(line.Substring(nextRot+1, p-nextRot-1)));

				if (p < line.Length) {
					instructions.Add(line[p] == 'R' ? rotRight : rotLeft);
				}
				nextRot = p;
			}
		}
	}
	var height = map.Count;
	for (int i = 0; i < height; i++) {
		if (map[i].Length == width) continue;
		map[i] = map[i].PadRight(width);
	}

	return (map, instructions);
}

record Face(int x, int y, Connection? right, Connection? down, Connection? left, Connection? up);

record Connection(int face, int rotation);