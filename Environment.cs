using System.Collections.Generic;
public class Environment {
    public static int EnvIndex = 0;
    public int Index;
    public Environment enclosing;
    private Dictionary<string, object> values = new Dictionary<string, object>();
    
    public Environment() {
        Index = Environment.EnvIndex++;
        enclosing = null;
    }

    public Environment(Environment enclosing) {
        Index = Environment.EnvIndex++;
        this.enclosing = enclosing;
    }

    public void Define(string name, object value) => values[name] = value;

    public void AssignAt(int distance, Token name, object value) {
        Ancestor(distance).values[name.lexeme] = value;
    }

    public object GetAt(int distance, string name) {
        return Ancestor(distance).values[name];
    }

    private Environment Ancestor(int distance) {
        Environment environment = this;
        for(int i = 0; i < distance; i++) {
            environment = environment.enclosing;
        }
        return environment;
    }

    public object Get(Token name) {
        if(values.ContainsKey(name.lexeme))
            return values[name.lexeme];
        
        if(enclosing != null) return enclosing.Get(name);

        throw new RuntimeException(name, "Undefined variable '" + name.lexeme + "'.");
    }

    public void Assign(Token name, object value) {
        if(values.ContainsKey(name.lexeme)) {
            values[name.lexeme] = value;
            return;
        }
        if(enclosing != null) {
            enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeException(name, "Undefined variable '" + name.lexeme + "'.");
    }
}