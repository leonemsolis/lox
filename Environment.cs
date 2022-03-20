using System.Collections.Generic;
public class Environment {
    private Dictionary<string, object> values = new Dictionary<string, object>();

    public void Define(string name, object value) => values.Add(name, value);

    public object Get(Token name) {
        if(values.ContainsKey(name.lexeme))
            return values[name.lexeme];
        throw new RuntimeException(name, "Undefined variable '" + name.lexeme + "'.");
    }

    public void Assign(Token name, object value) {
        if(values.ContainsKey(name.lexeme)) {
            values[name.lexeme] = value;
            return;
        }
        throw new RuntimeException(name, "Undefined variable '" + name.lexeme + "'.");
    }
}