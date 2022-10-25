//Olvera Morales Miguel Angel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*              Requerimientos
* 1. Requerimiento 1:
*       a) Agregar el residuo de la division en PorFactor -------------> OK
*       b) Agregar en Instruccion los incrementos de termino y de factor: a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
*          en donde el "1" puede ser cualquier numero entero o una expresion
*       c) Programar el destructor de la clase Lenguaje, sin invocar al metodo de cerrar archivo
* 2. Requerimiento 2:
*       c) Marcar errores semanticos cuando los incrementos de termino o factor superen el rango de la variable --------> OK
*       d) Considerar el inciso b) y c) para el for
*       e) Considerar el inciso b) y c) para el while y el do-while
* 3. Requerimiento 3:
*       a) Considerar las variables y los casteos de las expresiones matematicas en assembly
*       b) Considerar el residuo de la division en assembly
*/


namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int contadorIfs;
        public Lenguaje()
        {
            contadorIfs = 0;

        }
        public Lenguaje(string nombre) : base(nombre)
        {
            contadorIfs = 0;

        }
        ~Lenguaje()
        {
            Console.WriteLine("Destructor");
            cerrar();
        }
        private void secuenciasEscape(string cadena)
        {
            //Eliminamos las comillas de la cadena
            cadena = cadena.Trim('"');
            cadena = cadena.Replace("\\n", "\n");
            cadena = cadena.Replace("\\t", "\t");
            Console.Write(cadena);
        }

        private void addVariable(string nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }

        private void displayVariables()
        {
            log.WriteLine("\n\nVariables:");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + ", " + v.getTipoDato() + ", " + v.getValor());
            }
        }

        private void variablesASM()
        {
            asm.WriteLine(";Variables:");
            foreach (Variable v in variables)
            {
                asm.WriteLine("\t" + v.getNombre() + " DW ?");
            }
        }
        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        private void modificaValor(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }
            return 0;
        }

        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getTipoDato();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Metodo para volver a otra posicion del archivo
        //Se usa Position y seek, para ambos hay que limpiar el buffer
        private void cambiarPosicion(int posicion)
        {
            //Limpiamos el buffer y actualizamos la posicion
            archivo.DiscardBufferedData();
            archivo.BaseStream.Position = posicion;
        }

        private float incrementoFor(bool evaluacion)
        {
            string variable = getContenido();
            //Verificamos si la variable existe
            if (existeVariable(variable) == false)
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                if (evaluacion)
                {
                    //Si es un ++, aumentamos el valor en 1
                    return getValor(variable) + 1;
                }
            }
            else
            {
                match("--");
                //Si es un --, restamos el valor en 1
                if (evaluacion)
                {
                    return getValor(variable) - 1;
                }
            }
            return 0;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("ORG 1000h");
            Libreria();
            Variables();
            variablesASM();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            //asm.WriteLine("END");
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                    default: tipo = Variable.TipoDato.Char; break;
                }

                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en la linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }

        private bool evaluaSemantica(string nombreVariable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(nombreVariable);
            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion)
        {
            //Verificamos si la variable existe
            if (!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }
            log.WriteLine();
            string nombreVariable = getContenido();
            match(Tipos.Identificador);
            //Declaramos el dominante
            dominante = Variable.TipoDato.Char;

            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                //Requerimiento 1.B
                string operador = getContenido();
                switch (operador)
                {
                    case "++": ; break;
                    case "--": ; break;
                }
                //Requerimiento 1.C
            }
            else
            {
                log.Write(nombreVariable + " = ");
                match(Tipos.Asignacion);
                Expresion();
                match(";");
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                log.WriteLine("= " + resultado);
                log.WriteLine();
                //Actualizamos dominantes
                if (dominante < evaluaNumero(resultado))
                {
                    dominante = evaluaNumero(resultado);
                }
                if (dominante <= getTipo(nombreVariable))
                {
                    //Si el valor se puede asignar al tipo de dato actualizamos su valor
                    if (evaluacion)
                    {
                        modificaValor(nombreVariable, resultado);
                        
                    }
                }
                //De lo contrario levantamos una excepcion
                else
                {
                    throw new Error("Eror de semantica en la linea " + (linea - 1) + ": No podemos asignar un <" + dominante + "> a un <" + getTipo(nombreVariable) + ">", log);
                }
                asm.WriteLine("MOV " + nombreVariable + ", AX");
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            //Verificar que la condicion sea verdadera
            bool validarWhile = Condicion("");
            if (evaluacion == false)
            {
                validarWhile = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                //Validacion de la condicion
                if (validarWhile)
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    BloqueInstrucciones(evaluacion = false);
                }
            }
            else
            {
                if (validarWhile)
                {
                    Instruccion(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion = false);
                }
            }
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            match("do");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion);
            }
            else
            {
                Instruccion(evaluacion);
            }
            match("while");
            match("(");
            bool validarDo = Condicion("");
            if (evaluacion == false)
            {
                validarDo = false;
            }
            match(")");
            match(";");
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            match("for");
            match("(");
            Asignacion(evaluacion);
            //Verificar que la condicion sea verdadera
            bool validarFor = true;
            //Guardar la direccion de la posicion del archivo de texto
            int posicionFor = i - getContenido().Length;
            int lineaFor    = linea;
            //Metemos un ciclo (while) para despues validar el for
            while (validarFor)
            {
                validarFor = Condicion("");
                if (evaluacion == false)
                {
                    validarFor = false;
                }
                match(";");
                //Aqui se arregla un mal incremento dentro del for
                string nombreIncremento = getContenido();
                float valorIncremento   = incrementoFor(validarFor);
                //Requerimiento 1.D
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);
                }
                else
                {
                    Instruccion(validarFor);
                }
                if (validarFor)
                {
                    //Regresar a la direccion de la posicion del archivo de texto
                    i = posicionFor;
                    linea = lineaFor;
                    cambiarPosicion(i);
                    NextToken();
                    //Actualizamos el valor del contador del for
                    modificaValor(nombreIncremento, valorIncremento);
                }
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion, string nombreVariable)
        {
            //Verificamos si la variable existe
            if (!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }
            string variable = getContenido();
            match(Tipos.Identificador);
            //Requerimiento 2.C
            if (getContenido() == "++")
            {
                if (evaluacion)
                {
                    if(dominante < evaluaNumero(getValor(variable + 1)))
                    {
                        dominante = evaluaNumero(getValor(variable + 1));
                    }
                    if(dominante <= getTipo(variable))
                    {
                        modificaValor(variable, getValor(variable) + 1);
                    }
                    else
                    {
                        throw new Error("Eror de semantica en la linea " + (linea) + ": No podemos asignar un <" + dominante + "> a un <" + getTipo(variable) + ">", log);
                    }
                }
                match("++");
            }
            else if (getContenido() == "--")
            {
                if (evaluacion)
                {
                    modificaValor(variable, getValor(variable) - 1);
                }
                match("--");
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                ListaInstruccionesCase(evaluacion);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP AX");
            float e1 = stack.Pop();
            asm.WriteLine("POP BX");
            asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                case ">":
                    return e1 > e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 < e2;
                case "<=":
                    return e1 <= e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + ++contadorIfs;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf);
            if (!evaluacion)
            {
                validarIf = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);
            }
            else
            {
                Instruccion(validarIf);
            }
            if (getContenido() == "else")
            {
                match("else");
                //Validar condicion en el else
                if (getContenido() == "{")
                {
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion = false);
                    }
                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf);
                    }
                    else
                    {
                        Instruccion(evaluacion = false);
                    }
                }
            }
            asm.WriteLine(etiquetaIf + ":");
        }

        //Printf -> printf(cadena || expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                if (evaluacion)
                {
                    secuenciasEscape(getContenido());
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    Console.Write(resultado);
                }
            }
            match(")");
            match(Tipos.FinSentencia);
        }

        //Scanf -> scanf(cadena  -> , -> & -> identificador);
        private void Scanf(bool evaluacion)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            //Verificamos si la variable existe
            if (!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }
            if (evaluacion)
            {
                string valor = "" + Console.ReadLine();
                float valorNumerico;
                //El readLine debe capturar un numero, pero si el usuario ingresa una cadena debemos levantar una excepcion
                //si el valor es un numero, modifica el valor, sino levantamos una excepcion de que no es un numero
                if (float.TryParse(valor, out valorNumerico))
                {
                    modificaValor(getContenido(), valorNumerico);
                }
                else
                {
                    throw new Error("\nError de sintaxis en la linea: " + linea + ", el valor ingresado no es un numero", log);
                }
            }
            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Main -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true);
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+": stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-": stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "*": stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/": stack.Push(n2 / n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    //Requerimiento 1.A
                    case "%": stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            //Si es un numero
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
                match(Tipos.Numero);
            }
            //Si es una variable
            else if (getClasificacion() == Tipos.Identificador)
            {
                log.Write(getContenido() + " ");
                //Verificamos si la variable existe
                if (!existeVariable(getContenido()))
                {
                    throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
                }
                //Actualizamos el dominante
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                //Requerimiento 3.A
                match(Tipos.Identificador);
            }
            //Si hay un casteo obtenemos el tipo de dato y verificamos la sintaxis
            else
            {
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                bool huboCasteo = false;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    //Casteo y conversion de tipos
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "char": casteo = Variable.TipoDato.Char; break;
                        case "int": casteo = Variable.TipoDato.Int; break;
                        case "float": casteo = Variable.TipoDato.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                //Si hay un casteo hay que convertir el valor
                if (huboCasteo)
                {
                    //Casteo y conversion de tipos
                    //Actualizamos el dominante al casteo realizado y obtenemos el valor del stack para convertirlo al adecuado
                    dominante = casteo;
                    float valor = stack.Pop();
                    asm.WriteLine("POP AX");
                    //Nombre de la variable para actualizar el valor
                    string nombre = getContenido();
                    //Obtenemos el residuo acorde al tipo de dato
                    switch (casteo)
                    {
                        case Variable.TipoDato.Int: valor = valor % 65536; break;
                        case Variable.TipoDato.Char: valor = valor % 256; break;
                            //Como trabajamos originalmente con variables tipo float, no hay que hacer nada
                    }
                    //Modificamos el valor de la variable y la agregamos al stack
                    modificaValor(nombre, valor);
                    stack.Push(valor);
                }
            }
        }
    }
}