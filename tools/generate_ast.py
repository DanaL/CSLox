# A little python script to generate the C# classes which represent the AST for
# the interpreter. In Crafting Interpreters, Bob offered a Java command line
# app to do this because he didn't want to throw another language at the reader
# but I'm okay doing it in Python

import sys

def define_visitor(file, base_name, types):
    file.write("public interface IVisitor<T>\n{\n")

    for type in types:
        type_name = type.split(":")[0].strip()
        file.write("\tpublic T Visit" + type_name + base_name + "(" 
            + type_name + " " + base_name.lower() + ");\n")

    file.write("}")

def define_type(file, base_name, class_name, fields):
    fields_list = fields.split(", ")

    file.write("public class " + class_name + " : " + base_name + "\n{\n")

    # write the class fields
    for field in fields_list:
        file.write("\tpublic " + field.title() + ";\n")
    file.write("\n")

    # write the constructor
    file.write("\tpublic " + class_name + "(" + fields + ")\n\t{\n")
    for field in fields_list:
        name = field.split(" ")[1]
        file.write("\t\t" + name.capitalize() + " = " + name + ";\n")
    file.write("\t}\n\n") 

    # Visitor pattern
    file.write("\tpublic override T Accept<T>(IVisitor<T> visitor)\n\t{\n")
    file.write("\t\treturn visitor.Visit" + class_name + base_name + "(this);");
    file.write("\n\t}")

    file.write("\n}\n\n")

def define_ast(output_dir, base_name, types):
    path = output_dir + "/" + base_name + ".cs"
    with open(path, "w") as file:
        file.write("namespace CSLox;\n\n")
        
        define_visitor(file, base_name, types)
        file.write("\n\n")

        file.write("public abstract class " + base_name + " \n{\n")
        file.write("\tpublic abstract T Accept<T>(IVisitor<T> visitor);\n")
        file.write("}\n\n")

        for type in types:
            class_name = type.split(":")[0].strip()
            fields = type.split(":")[1].strip()
            define_type(file, base_name, class_name, fields)

def set_output_dir():
    if len(sys.argv) != 2:
        print("Usage: generate_ast.py <output directory>")
        exit(1)
    return sys.argv[1]

output_dir = set_output_dir()

types = ["Binary : Expr left, Token op, Expr right",
         "Grouping: Expr expression",
         "Literal: Object value",
         "Unary: Token op, Expr right",
         "Variable: Token name"]

stmtTypes = [ 
  "Expr : Expr expression",
  "Print      : Expr expression",
  "Var    : Token name, Expr initializer""
]

define_ast(output_dir, "Expr", types)
define_ast(output_dir, "Stmt", stmtTypes)

