using System.Collections.Generic;

public class LoxClass : LoxCallable {
    public string name;
    public LoxClass(string name) {
        this.name = name;
    }

    public override string ToString()
    {
        return name;
    }

    public override int Arity()
    {
        return 0;
    }

    public override object Call(Interpreter interpreter, List<object> arguments)
    {
        LoxInstance instance = new LoxInstance(this);
        return instance;
    }
}

public class LoxInstance {
    private LoxClass klass;
    private Dictionary<string, object> fields = new Dictionary<string, object>();
    public LoxInstance(LoxClass klass) {
        this.klass = klass;
    }

    public override string ToString()
    {
        return klass.name + " instance";
    }

    public object Get(Token name) {
        if(fields.ContainsKey(name.lexeme)) {
            return fields[name.lexeme];
        }

        throw new RuntimeException(name, "Undefined property '" + name.lexeme + "'.");
    }
}