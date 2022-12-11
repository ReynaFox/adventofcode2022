using System.Text.RegularExpressions;

part1();
part2();

static void part1() {
	var lines = File.ReadLines("input.txt");
	var linesEnum = lines.GetEnumerator();

	var monkeys = new List<Monkey>();
	while (linesEnum.MoveNext()) {
		// Ignore the monkey number line, it's incrementing anyway
		monkeys.Add(Monkey.fromReader(linesEnum));

		// Skip break line
		linesEnum.MoveNext();
	}

	for (int round = 0; round < 20; round++) {
		foreach (var m in monkeys) {
			m.counter += m.items.Count;
			foreach (var item in m.items) {
				var arg1 = m.arg1old ? item : m.arg1;
				var arg2 = m.arg2old ? item : m.arg2;
				var value = m.op switch {
					'*' => arg1*arg2,
					'+' => arg1+arg2,
				};
				value /= 3;
				var newOwner = (value % m.testArg) == 0 ? m.throwOnTrue : m.throwOnFalse;
				monkeys[newOwner].items.Add(value);
			}
			m.items.Clear();
		}
	}

	var mostActive = monkeys.OrderByDescending(i => i.counter).Take(2).ToArray();
	Console.WriteLine(mostActive[0].counter * mostActive[1].counter);
}

static void part2() {
	var lines = File.ReadLines("input.txt");
	var linesEnum = lines.GetEnumerator();

	var monkeys = new List<Monkey>();
	var divisor = 1;
	while (linesEnum.MoveNext()) {
		// Ignore the monkey number line, it's incrementing anyway
		var m = Monkey.fromReader(linesEnum);
		divisor *= m.testArg;
		monkeys.Add(m);

		// Skip break line
		linesEnum.MoveNext();
	}


	for (int round = 0; round < 10000; round++) {
		var mc = 0;
		foreach (var m in monkeys) {
			mc++;
			m.counter += m.items.Count;
			foreach (var item in m.items) {
				var arg1 = m.arg1old ? item : m.arg1;
				var arg2 = m.arg2old ? item : m.arg2;
				var value = m.op switch {
					'*' => arg1*arg2,
					'+' => arg1+arg2,
				};
				value %= divisor;
				var newOwner = (value % m.testArg) == 0 ? m.throwOnTrue : m.throwOnFalse;
				monkeys[newOwner].items.Add(value);
			}
			m.items.Clear();
		}
	}

	var mostActive = monkeys.OrderByDescending(i => i.counter).Take(2).ToArray();
	Console.WriteLine(mostActive[0].counter * mostActive[1].counter);
}

class Monkey {
	public static Regex exprRegex = new Regex(@"(\d+|old)\s*([+*])\s*(\d+|old)");

	public readonly List<long> items = new();
	public char op;
	public int arg1, arg2;
	public bool arg1old, arg2old;
	public int testArg;
	public int throwOnTrue, throwOnFalse;

	public long counter;

	public static Monkey fromReader(IEnumerator<string> file) {
		var result = new Monkey();

		// Starting items line
		file.MoveNext();
		 result.items.AddRange(
			file.Current.Split(':')[1]
				.Split(',')
				.Select(s => long.Parse(s.Trim()))
		);
		// Operation line
		file.MoveNext();
		var expr = file.Current.Split(':')[1].Split('=')[1];
		var match = exprRegex.Match(expr);
		result.op = match.Groups[2].Value[0];
		result.arg1old = match.Groups[1].Value == "old";
		result.arg2old = match.Groups[3].Value == "old";
		if (!result.arg1old) result.arg1 = int.Parse(match.Groups[1].Value);
		if (!result.arg2old) result.arg2 = int.Parse(match.Groups[3].Value);
		// Test line
		file.MoveNext();
		result.testArg = int.Parse(file.Current.Substring(file.Current.LastIndexOf(' ')+1));
		// True line
		file.MoveNext();
		result.throwOnTrue = int.Parse(file.Current.Substring(file.Current.LastIndexOf(' ')+1));
		// False line
		file.MoveNext();
		result.throwOnFalse = int.Parse(file.Current.Substring(file.Current.LastIndexOf(' ')+1));

		return result;
	}
}