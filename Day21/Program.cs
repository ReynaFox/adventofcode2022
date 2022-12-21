part1();
part2();

static void part1() {
	var ops = new Dictionary<string, Op>();

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		var parts = line.Split(':');
		var name = parts[0];
		parts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (parts.Length == 1) {
			ops[name] = new Op(int.Parse(parts[0]));
		} else {
			ops[name] = new Op(parts[1][0], parts[0], parts[2]);
		}
	}

	Console.WriteLine(compute(ops["root"]));

	long compute(Op op) {
		if (op.operand == 'l') return op.value;

		var arg1 = compute(ops[op.arg1]);
		var arg2 = compute(ops[op.arg2]);
		switch (op.operand) {
		case '+': return arg1+arg2;
		case '-': return arg1-arg2;
		case '*': return arg1*arg2;
		case '/': return arg1/arg2;
		}
		return 0;
	}
}

static void part2() {
	var ops = new Dictionary<string, Op>();

	foreach (var line in File.ReadLines("input.txt")) {
		if (string.IsNullOrEmpty(line)) continue;
		var parts = line.Split(':');
		var name = parts[0];
		parts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (parts.Length == 1) {
			ops[name] = new Op(int.Parse(parts[0]));
		} else {
			ops[name] = new Op(parts[1][0], parts[0], parts[2]);
		}
	}

	var humanNode = ops["humn"];
	humanNode.operand = 'v';
	var root = ops["root"];
	markVar(root);

	var constVal = compute(ops[root.arg1].containsVar ? ops[root.arg2] : ops[root.arg1], 0);
	var computed = compute(ops[root.arg1].containsVar ? ops[root.arg1] : ops[root.arg2], constVal);


	Console.WriteLine($"{constVal} = {computed} -> {humanNode.value}");

	void markVar(Op op) {
		switch (op.operand) {
		case 'v':
			op.containsVar = true;
			break;
		case '+':
		case '-':
		case '*':
		case '/':
			markVar(ops[op.arg1]);
			markVar(ops[op.arg2]);
			op.containsVar = ops[op.arg1].containsVar | ops[op.arg2].containsVar;
			break;
		}
	}

	long compute(Op op, long required) {
		if (op.operand == 'l') return op.value;
		if (op.operand == 'v') {
			op.value = required;
			return required;
		}

		var sub1 = ops[op.arg1];
		var sub2 = ops[op.arg2];

		long arg1=0, arg2=0;

		if (sub1.containsVar) {
			arg2 = compute(sub2, 0);
			switch (op.operand) {
			case '+': arg1 = compute(sub1, required-arg2); break;
			case '-': arg1 = compute(sub1, required+arg2); break;
			case '*': arg1 = compute(sub1, required/arg2); break;
			case '/': arg1 = compute(sub1, required*arg2); break;
			}
		}

		if (sub2.containsVar) {
			arg1 = compute(sub1, 0);
			switch (op.operand) {
			case '+': arg2 = compute(sub2, required-arg1); break;
			case '-': arg2 = compute(sub2, -required+arg1); break;
			case '*': arg2 = compute(sub2, required/arg1); break;
			case '/': arg2 = compute(sub2, required*arg1); break;
			}
		}

		if (!sub1.containsVar && !sub2.containsVar) {
			// Constant
			arg1 = compute(sub1, 0);
			arg2 = compute(sub2, 0);
		}

		switch (op.operand) {
		case '+': return arg1+arg2;
		case '-': return arg1-arg2;
		case '*': return arg1*arg2;
		case '/': return arg1/arg2;
		}

		return 0;
	}
}

class Op {
	public char operand;
	public string arg1, arg2;
	public long value;
	
	public bool containsVar = false;

	public Op(char op, string arg1, string arg2) {
		operand = op;
		this.arg1 = arg1;
		this.arg2 = arg2;
	}

	public Op(int value) {
		operand = 'l';
		this.value = value;
	}
}