using System;
public class RuntimeException : Exception {
    public Token token;
    public RuntimeException(Token token, string message) : base(message) {
        this.token = token;
    }
}