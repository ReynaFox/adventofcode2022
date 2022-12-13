part1();
part2();

static void part1() {
	var i = 0;
	var pair = 1;
	var sum = 0;
	List<object> list1 = null;
	foreach (var line in File.ReadLines("input.txt")) {
		var ii = i%3;
		if (ii == 0) list1 = parse(line);
		if (ii == 1) {
			var list2 = parse(line);
			if (compare(list1, list2) == -1) {
				sum += pair;
			}
			pair++;
		}
		i++;
	}

	Console.WriteLine(sum);
}

static List<object> parse(string s) {
	var p = 0;
	var result = parseList(s, ref p);
	return (List<object>)result[0];
}

static List<object> parseList(string s, ref int p) {
	var result = new List<object>();
	while (true) {
		if (s[p] == '[') {
			p++;
			result.Add(parseList(s, ref p));
			if (p >= s.Length) return result;
		} else {
			var end = s.IndexOfAny(new[] { '[', ']' }, p);
			result.AddRange(
				s.Substring(p, end-p)
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(n => (object)int.Parse(n))
			);
			p = end;
			if (s[p] == ']') {
				p++;
				return result;
			}
		}
	}
}

static int compare(object left, object right) {
	if (left is int li && right is int ri) {
		if (li == ri) return 0;
		return li < ri ? -1 : 1;
	}
	var ll = left as List<object>;
	var rl = right as List<object>;
	ll ??= new List<object> { (int)left };
	rl ??= new List<object> { (int)right };

	for (int i = 0; i < Math.Min(ll.Count, rl.Count); i++) {
		var cmp = compare(ll[i], rl[i]);
		if (cmp != 0) return cmp;
	}
	if (ll.Count == rl.Count) return 0;
	return ll.Count < rl.Count ? -1 : 1;
}

static void part2() {
	var lists = new List<List<object>>();
	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;

		lists.Add(parse(line));
	}
	lists.Add(parse("[[2]]"));
	lists.Add(parse("[[6]]"));

	lists.Sort(compare);

	var strLists = lists.Select(listStr).ToList();
	var key = (strLists.IndexOf("[[2]]")+1) * (strLists.IndexOf("[[6]]")+1);
	Console.WriteLine(key);
}

static string listStr(List<object> list) {
	return "["+string.Join(",", list.Select(
		l => l is int i
			? i.ToString()
			: listStr((List<object>)l)
	))+"]";
}