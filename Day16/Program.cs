using System.Text.RegularExpressions;

//part1();
part2();


static void part1() {
	var (nodes, distances) = read();
	var startNode = nodes.FindIndex(n => n.name == "AA");

	//writeDistanceMatrix(nodes, distances);

	var visited = new List<bool>();
	foreach (var n in nodes) visited.Add(n.flow==0);

	var count = 0;
	
	var (best, path) = examinePath(startNode, 30, 0, visited);

	Console.WriteLine(best);
	Console.WriteLine(string.Join(" ", path));
	Console.WriteLine(count);


	(int, List<string>) examinePath(int currNode, int remainingSteps, int sum, List<bool> visited) {
		visited[currNode] = true;
		sum += remainingSteps*nodes[currNode].flow;

		var bestSum = sum;
		var bestPath = new List<string>();
		var isEnd = true;
		for (int i = 0; i < nodes.Count; i++) {
			if (visited[i]) continue;
			var nextRemaining = remainingSteps-1-distances[currNode, i];
			if (nextRemaining > 0) {
				isEnd = false;
				var (subSum, path) = examinePath(i, remainingSteps-1-distances[currNode, i], sum, visited);
				if (subSum > bestSum) {
					bestSum = subSum;
					bestPath = path;
				}
			}
		}

		if (isEnd) count++;

		visited[currNode] = false;

		bestPath.Insert(0, nodes[currNode].name);

		return (bestSum, bestPath);
	}
}

static void part2() {
	var (nodes, distances) = read();
	var startNode = nodes.FindIndex(n => n.name == "AA");

	//writeDistanceMatrix(nodes, distances);

	var visited = new List<bool>();
	foreach (var n in nodes) visited.Add(n.flow==0);
	visited[startNode] = true;
	var (best, path1, path2) =
		examinePath(startNode, startNode, 26, 26, 0, visited
		//new List<(string, int)>(), new List<(string, int)>()
		);

	Console.WriteLine(best);
	//Console.WriteLine(string.Join(" ", path1));
	//Console.WriteLine(string.Join(" ", path2));

	(int, List<string>, List<string>) examinePath(
		int currNode1, int currNode2, 
		int remainingSteps1, int remainingSteps2, 
		int sum, List<bool> visited

		//List<(string, int)> names1, List<(string, int)> names2
		) {
		var bestSum = sum;
		//var bestPath1 = new List<string>();
		//var bestPath2 = new List<string>();
		for (int i = 0; i < nodes.Count; i++) {
			if (visited[i]) continue;
			visited[i] = true;

			// Try adding on path 1
			var nextRemaining = remainingSteps1-1-distances[currNode1, i];
			if (nextRemaining > 0) {
				//names1.Add((nodes[i].name, 26-nextRemaining));
				var (subSum, path1, path2) = 
					examinePath(i, currNode2, nextRemaining,
						remainingSteps2, sum + nextRemaining*nodes[i].flow, visited
						//names1, names2
						);
				if (subSum > bestSum) {
					bestSum = subSum;
					//bestPath1 = path1;
					//bestPath1.Insert(0, nodes[i].name);
					//bestPath2 = path2;
				}
				//names1.RemoveAt(names1.Count-1);
			}

			// Try adding on path 2
			nextRemaining = remainingSteps2-1-distances[currNode2, i];
			if (nextRemaining > 0) {
				//names2.Add((nodes[i].name, 26-nextRemaining));
				var (subSum, path1, path2) = 
					examinePath(currNode1, i, remainingSteps1,
						nextRemaining, sum + nextRemaining*nodes[i].flow, visited
						//names1, names2
						);
				if (subSum > bestSum) {
					bestSum = subSum;
					//bestPath1 = path1;
					//bestPath2 = path2;
					//bestPath2.Insert(0, nodes[i].name);
				}
				//names2.RemoveAt(names2.Count-1);
			}

			visited[i] = false;

			if (currNode1 == startNode && currNode2 == startNode) {
				Console.Write(".");
			}
		}

		//return (bestSum, bestPath1, bestPath2);
		return (bestSum, null, null);
	}
}

static void writeDistanceMatrix(List<Node> nodes, int[,] distances) {
	Console.Write("   ");
	foreach (var n in nodes) {
		Console.Write(n.name+" ");
	}
	Console.WriteLine();
	for (int j = 0; j < nodes.Count; j++) {
		Console.Write(nodes[j].name+" ");
		for (int i = 0; i < nodes.Count; i++) {
			Console.Write($"{distances[i,j],2} ");
		}
		Console.WriteLine();
	}
}

static (List<Node>, int[,]) read() {
	var regex = new Regex(@"Valve ([A-Z]{2}).*rate=(\d+).*to valves? (.*)$");

	var nodesMap = new Dictionary<string, Node>();
	var nodes = new List<Node>();
	var nodesWithFlow = new List<string>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		var matches = regex.Match(line);
		var name = matches.Groups[1].Value;
		var node = new Node(
			name,
			int.Parse(matches.Groups[2].Value),
			from n in matches.Groups[3].Value.Split(",")
			select n.Trim()
		);
		node.idx = nodes.Count;
		nodes.Add(node);
		nodesMap[name] = node;
		if (node.flow > 0) nodesWithFlow.Add(name);
	}

	var distances = new int[nodes.Count, nodes.Count];

	// Init distance matrix
	for (int j = 0; j < nodes.Count; j++) {
		for (int i = 0; i < nodes.Count; i++) {
			distances[i, j] = i == j ? 0 : int.MaxValue;
		}
	}
	foreach (var node in nodes) {
		foreach (var conn in node.connections) {
			distances[node.idx, nodesMap[conn].idx] = 1;
		}
	}

	// Compute distance matrix
	for (int k = 0; k < nodes.Count; k++) {
		for (int j = 0; j < nodes.Count; j++) {
			for (int i = 0; i < nodes.Count; i++) {
				if (distances[i, j] > (long)distances[i, k] + (long)distances[k, j]) {
					distances[i, j] = distances[i, k] + distances[k, j];
				}
			}
		}
	}

	return (nodes, distances);
}

class Node {
	public readonly int flow;
	public int idx;
	public readonly string name;
	public readonly List<string> connections;

	public Node(string name, int flow, IEnumerable<string> conn) {
		this.name = name;
		this.flow = flow;
		connections = new List<string>(conn);
	}
}