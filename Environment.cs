using System.Collections.Generic;
public class Environment {
    public Environment enclosing;
    public Environment() {
        enclosing = null;
    }

    public Environment(Environment enclosing) {
        this.enclosing = enclosing;
    }

    private Dictionary<string, object> values = new Dictionary<string, object>();

    public void Define(string name, object value) => values.Add(name, value);

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