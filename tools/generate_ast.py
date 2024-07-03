# A little python script to generate the C# classes which represent the AST for
# the interpreter. In Crafting Interpreters, Bob offered a Java command line
# app to do this because he didn't want to throw another language at the reader
# but I'm okay doing it in Python

import sys

def define_ast(output_dir, base_name, types):
    path = output_dir + "/" + base_name + ".cs"
def set_output_dir():
    if len(sys.argv) != 2:
        print("Usage: generate_ast.py <output directory>")
        exit(1)
    return sys.argv[1]

output_dir = set_output_dir()


