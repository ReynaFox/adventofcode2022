using System.Text.RegularExpressions;

part1();
part2();


static void part1() {
	const int targetRow = 2000000;
	var regex = new Regex(@"at x=(-?\d+), y=(-?\d+).*at x=(-?\d+), y=(-?\d+)");

	var ranges = new List<Range>();
	var beacons = new List<int>();

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var matches = regex.Match(line);
		var sx = int.Parse(matches.Groups[1].Value);
		var sy = int.Parse(matches.Groups[2].Value);
		var bx = int.Parse(matches.Groups[3].Value);
		var by = int.Parse(matches.Groups[4].Value);

		var sensorRange = Math.Abs(sx-bx) + Math.Abs(sy-by);
		var yDist = Math.Abs(sy-targetRow);
		if (yDist > sensorRange) continue; // Sensor does not affect target row

		var width = sensorRange-yDist;
		ranges.Add(new Range(sx-width, sx+width));

		if (by == targetRow && beacons.IndexOf(bx) == -1) beacons.Add(bx);
	}

	beacons.Sort();
	var merged = mergeRanges(ranges);

	var sum = 0;
	var bi = 0;
	foreach (var r in merged) {
		sum += r.max-r.min+1;
		while (bi < beacons.Count && beacons[bi] < r.min) bi++;
		while (bi < beacons.Count && beacons[bi] <= r.max) {
			sum--;
			bi++;
		}
	}

	Console.WriteLine(sum);
}

static List<Range> mergeRanges(List<Range> ranges) {
	ranges.Sort((a, b) => a.min.CompareTo(b.min));

	// Merge ranges
	var result = new List<Range>();
	var curr = ranges.First();
	foreach (var r in ranges) {
		if (curr.min <= r.min && r.min <= curr.max+1) {
			curr.max = Math.Max(curr.max, r.max);
		} else {
			result.Add(curr);
			curr = r;
		}
	}
	result.Add(curr);
	return result;
}

static void part2() {
	var coordLimit = 4000000;
	var regex = new Regex(@"at x=(-?\d+), y=(-?\d+).*at x=(-?\d+), y=(-?\d+)");

	var sensors = new List<Sensor>();

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var matches = regex.Match(line);
		var sx = int.Parse(matches.Groups[1].Value);
		var sy = int.Parse(matches.Groups[2].Value);
		var bx = int.Parse(matches.Groups[3].Value);
		var by = int.Parse(matches.Groups[4].Value);

		var sensorRange = Math.Abs(sx-bx) + Math.Abs(sy-by);
		sensors.Add(new Sensor(sx, sy, sensorRange));
	}

	sensors.Sort((a, b) => a.y.CompareTo(b.y));

	var gapX = -1;
	var gapY = -1;
	for (int y = 0; y <= coordLimit; y++) {
		var ranges = new List<Range>();
		var activeSensors = from s in sensors where s.y-s.range <= y && y <= s.y+s.range select s;
		foreach (var s in activeSensors) {
			var yDist = Math.Abs(s.y-y);
			var width = s.range-yDist;
			ranges.Add(new Range(s.x-width, s.x+width));
		}

		var merged = mergeRanges(ranges);
		var first = merged.First();
		first.min = Math.Max(0, first.min);
		var last = merged.Last();
		last.max = Math.Min(coordLimit, last.max);

		if (first.min > 0) {
			gapX = 0;
			gapY = y;
			break;
		} else if (last.max < coordLimit) {
			gapX = coordLimit;
			gapY = y;
			break;
		} else if (merged.Count > 1) {
			gapX = first.max+1;
			gapY = y;
			break;
		}
	}
	
	Console.WriteLine(gapX*4_000_000L+gapY);
}

class Range {
	// Inclusive range.
	public int min, max;

	public Range(int min, int max) {
		this.min = min;
		this.max = max;
	}
}

record Sensor(int x, int y, int range);