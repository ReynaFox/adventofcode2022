run(part1Answer);
run(part2Answer);

static void run(Func<Folder, int> callback) {
	var root = new Folder(null);
	var current = root;

	var inList = false;
	foreach (var line in File.ReadLines("input.txt")) {
		if (line[0] == '$') {
			inList = false;
			switch (line.Substring(2, 2)) {
			case "cd":
				var arg = line[5..];
				if (arg == "/") current = root;
				else if (arg == "..") current = current.parent;
				else current = current.contents[arg];
				break;
			case "ls":
				inList = true;
				break;
			}
		} else if (inList) {
			var parts = line.Split(' ');
			if (parts[0] == "dir") {
				current.contents[parts[1]] = new Folder(current);
			} else {
				current.directSize += int.Parse(parts[0]);
			}
		}
	}

	Console.WriteLine(callback(root));
}

static int part2Answer(Folder root) {
	var rootSize = computeTotalSize(root).folderSize;
	var available = 70_000_000 - rootSize;
	var minFree = 30_000_000 - available;

	 return findSmallest(root);

	int findSmallest(Folder curr) {
		var result = curr.folderSize >= minFree ? curr.folderSize : int.MaxValue;
		foreach (var sub in curr.contents.Values) {
			result = Math.Min(result, findSmallest(sub));
		}
		return result;
	}
}

static int part1Answer(Folder root) {
	return computeTotalSize(root).sumTotal;
}

static (int sumTotal, int folderSize) computeTotalSize(Folder f) {
	f.folderSize = f.directSize;
	var sumTotal = 0;
	foreach (var sub in f.contents.Values) {
		var (sum, size) = computeTotalSize(sub);
		f.folderSize += size;
		sumTotal += sum;
	}
	if (f.folderSize <= 100_000) sumTotal += f.folderSize;
	return (sumTotal, f.folderSize);
}

class Folder {
	public readonly Folder parent;
	public int folderSize;

	public readonly Dictionary<string, Folder> contents = new();
	public int directSize = 0;
	public Folder(Folder parent) {
		this.parent = parent;
	}
}