using System;
public class RuntimeException : Exception {
    public Token token;
    public RuntimeException(Token token, string message) : base(message) {
        this.token = token;
    }
}

public class Return : Exception {
    public object value;
    public Return(object value) : base(null) {
        this.value = value;
    }
}