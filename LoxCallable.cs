using System;
using System.Collections.Generic;
public abstract class LoxCallable {
    public abstract int Arity();
    public abstract object Call(Interpreter interpreter, List<object>arguments);
}

public class BuiltInPrint : LoxCallable {
    public override int Arity()
    {
        return 1;
    }

    public override object Call(Interpreter interpreter, List<object> arguments)
    {
        Console.WriteLine(arguments[0]);
        return null;
    }

    public override string ToString()
    {
        return "<native fn>";
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

public class LoxFunction : LoxCallable {
    private Stmt.Function declaration;
    private Environment closure;
    private bool isInitializer;
    public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer) {
        this.declaration = declaration;
        this.closure = closure;
        this.isInitializer = isInitializer;
    }
    public LoxFunction Bind(LoxInstance instance) {
        Environment environment = new Environment(closure);
        environment.Define("this", instance);
        return new LoxFunction(declaration, environment, isInitializer);
    }

    public override int Arity()
    {
        return declaration.parameters.Count;
    }

    public override object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new Environment(closure);
        for(int i = 0; i < declaration.parameters.Count; i++) {
            environment.Define(declaration.parameters[i].lexeme, arguments[i]);
        }
        try {
            interpreter.ExecuteBlock(declaration.body, environment);
        } catch(Return returnValue) {
            if(isInitializer) return closure.GetAt(0, "this");
            return returnValue.value;
        }
        if(isInitializer) return closure.GetAt(0, "this");
        return null;
    }

    public override string ToString()
    {
        return "<fn " + declaration.name.lexeme + ">";
    }
}