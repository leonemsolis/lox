using System;
using System.Collections.Generic;
public abstract class LoxCallable {
    public abstract int Arity();
    public abstract object Call(Interpreter interpreter, List<object>arguments);

    public void DefineInEnvironment(Environment environment, string callableName) {
        environment.Define(callableName, this);
    }
}

public class BuiltInClock : LoxCallable {
    public override int Arity() {
        return 0;
    }

    public override object Call(Interpreter interpreter, List<object> arguments) {
        return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public override string ToString()
    {
        return "<native fn>";
    }
}