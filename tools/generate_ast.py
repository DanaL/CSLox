# A little python script to generate the C# classes which represent the AST for
# the interpreter. In Crafting Interpreters, Bob offered a Java command line
# app to do this because he didn't want to throw another language at the reader
# but I'm okay doing it in Python

import sys

def define_type(file, base_name, class_name, fields):
    fields_list = fields.split(", ")

    file.write("public class " + class_name + " : " + base_name + "\n{\n")

    # write the class fields
    for field in fields_list:
        file.write("\t" + field.title() + ";\n")
    file.write("\n")

    # write the constructor
    file.write("\tpublic " + class_name + "(" + fields + ")\n\t{\n")
    for field in fields_list:
        name = field.split(" ")[1]
        file.write("\t\t" + name.capitalize() + " = " + name + ";\n")
    file.write("\t}\n}\n\n")

def define_ast(output_dir, base_name, types):
    path = output_dir + "/" + base_name + ".cs"
    with open(path, "w") as file:
        file.write("namespace CSLox;\n\n")
        file.write("public abstract class " + base_name + " {")
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
         "Unary: Token op, Expr right"]

define_ast(output_dir, "Expr", types)

