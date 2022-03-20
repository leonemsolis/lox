using System.Collections.Generic;
using System.IO;
using System;

public class GenerateAst {
    public static void DefineAst(string outputDir, string baseName, List<string> types) {
        string path = $"{outputDir}/{baseName}.cs";
        var content = new List<string>();
        content.Add("using System.Collections.Generic;");
        content.Add("");
        content.Add($"public abstract class {baseName} " + "{");

        // The base Accept() method
        content.Add("\tpublic abstract T Accept<T>(Visitor<T> visitor);");
        content.Add("");
        DefineVisitor(content, baseName, types);
        foreach(var type in types) {
            var typeSplited = type.Split(':');
            var className = typeSplited[0].Trim();
            var fields = typeSplited[1].Trim();
            DefineType(content, baseName, className, fields);
        }
        content.Add("}");
        File.WriteAllLines(path, content);
    }

    private static void DefineVisitor(List<string>content, string baseName, List<string>types) {
        content.Add("\tpublic interface Visitor<T> {");
        foreach(var type in types) {
            var typeName = type.Split(':')[0].Trim();
            content.Add($"\t\tT visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }
        content.Add("\t}");
    }

    private static void DefineType(List<string> content, string baseName, string className, string fields) {
        content.Add($"\tpublic class {className} : {baseName} " + "{");
        var fieldsSplited = fields.Split(new string[] {", "}, StringSplitOptions.None);
        foreach(var field in fieldsSplited) {
            content.Add($"\t\tpublic {field};");
        }
        content.Add("");
        content.Add($"\t\tpublic {className}({fields}) " + "{");
        foreach(var field in fieldsSplited) {
            var name = field.Split(' ')[1];
            content.Add($"\t\t\tthis.{name} = {name};");
        }
        content.Add("\t\t}");

        // Visitor pattern
        content.Add("");
        content.Add("\t\tpublic override T Accept<T>(Visitor<T> visitor) {");
        content.Add($"\t\t\treturn visitor.visit{className}{baseName}(this);");
        content.Add("\t\t}");
        content.Add("\t}");
    }
}