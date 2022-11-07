//Olvera Morales Miguel Angel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*              Requerimientos
* 1. Requerimiento 1:
*       a) Agregar el residuo de la division en PorFactor -------------> OK
*       b) Agregar en Instruccion los incrementos de termino y de factor: a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1 -------------> OK
*          en donde el "1" puede ser cualquier numero entero o una expresion
*       c) Programar el destructor de la clase Lenguaje, sin invocar al metodo de cerrar archivo  -------------> OK
* 2. Requerimiento 2:
*       c) Marcar errores semanticos cuando los incrementos de termino o factor superen el rango de la variable --------> OK
*       d) Considerar el inciso b) y c) para el for -------------> OK
*       e) Considerar el inciso b) y c) para el while y el do-while -------------> OK
* 3. Requerimiento 3:
*       a) Considerar las variables y los casteos de las expresiones matematicas en assembly
*       b) Considerar el residuo de la division en assembly -------------> OK 
*       c) Programar el printf y el scanf en assembly   -------------> OK
* 4. Requerimiento 4:
*       a) Programar el "else" en assembly
*       b) Programar el "for" en assembly
* 5. Requerimiento 5:
*       a) Programar el "while" en assembly
*       b) Programar el "do-while" en assembly
*/


namespace Semantica
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        int contadorIfs;
        int contadorFors;
        int contadorWhile;
        int contadorDoWhile;
        public Lenguaje()
        {
            contadorIfs = contadorFors = contadorWhile = contadorDoWhile = 0;

        }
        public Lenguaje(string nombre) : base(nombre)
        {
            contadorIfs = contadorFors = contadorWhile = contadorDoWhile = 0;

        }
        //Destructor
        public void Dispose()
        {
            Console.WriteLine("\n\nDestructor ejecutado exitosamente");
            cerrar();
        }
        private void secuenciasEscape(string cadena)
        {
            cadena = cadena.Trim('"');
            string aux = cadena;
            cadena = cadena.Replace("\\n", "\n");
            cadena = cadena.Replace("\\t", "\t");
            /*
            if(cadena.Contains("\n"))
            {
                asm.WriteLine("PRINTN " + "\"" + "" + "\"");
            }
            if(cadena.Contains("\t"))
            {
                asm.WriteLine("PRINT " + "\"" + "   " + "\"");
            }
            else
            {
                asm.WriteLine("PRINT " + "\"" + cadena + "\"");
            }
            */
            aux = aux.Replace("\\n", " ");
            aux = aux.Replace("\\t", "");
            asm.WriteLine("PRINT " + "\"" + aux + "\"");
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

        private string incrementoFor(bool evaluacion, bool evaluacionASM)
        {
            string variable = getContenido();
            //Verificamos si la variable existe
            if (existeVariable(variable) == false)
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <" + getContenido() + "> no existe", log);
            }
            match(Tipos.Identificador);
            string operador = getContenido();
            if (getContenido() == "++")
            {
                match("++");
                if (evaluacion)
                {
                    modificaValor(variable, getValor(variable) + 1);
                }
                return operador;
            }
            else if (getContenido() == "--")
            {
                match("--");
                if (evaluacion)
                {
                    modificaValor(variable, getValor(variable) - 1);
                }
                return operador;
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion(evaluacionASM);
                float valor = getValor(variable) + stack.Pop();
                if (evaluacion)
                {
                    modificaValor(variable, valor);
                    if (evaluacionASM)
                    {
                        asm.WriteLine("POP AX");
                    }
                }
                return operador;
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion(evaluacionASM);
                float valor = getValor(variable) - stack.Pop();
                if (evaluacion)
                {
                    modificaValor(variable, valor);
                    if (evaluacionASM)
                    {
                        asm.WriteLine("POP AX");
                    }
                }
                return operador;
            }
            else
            {
                return operador;
            }
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesASM();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_SCAN_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
            asm.WriteLine("END");
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
        private void BloqueInstrucciones(bool evaluacion, bool evaluacionASM)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, evaluacionASM);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool evaluacionASM)
        {
            Instruccion(evaluacion, evaluacionASM);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, evaluacionASM);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool evaluacionASM)
        {
            Instruccion(evaluacion, evaluacionASM);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, evaluacionASM);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool evaluacionASM)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion, evaluacionASM);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion, evaluacionASM);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, evaluacionASM);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, evaluacionASM);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion, evaluacionASM);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion, evaluacionASM);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion, evaluacionASM);
            }
            else
            {
                Asignacion(evaluacion, evaluacionASM);
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
        private void Asignacion(bool evaluacion, bool evaluacionASM)
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
                float valor, valorStack;
                switch (operador)
                {
                    case "++":
                        Incremento(evaluacion, nombreVariable, evaluacionASM);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("INC " + nombreVariable);
                        }
                        break;
                    case "--":
                        Incremento(evaluacion, nombreVariable, evaluacionASM);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("DEC " + nombreVariable);
                        }
                        break;
                    case "*=":
                        match(Tipos.IncrementoFactor);
                        Expresion(evaluacionASM);
                        valorStack = stack.Pop();
                        valor = valorStack * getValor(nombreVariable);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("MUL " + nombreVariable + ", " + valor);
                        }
                        if (dominante < evaluaNumero(valor))
                        {
                            dominante = evaluaNumero(valor);
                        }
                        if (dominante <= getTipo(nombreVariable))
                        {
                            if (evaluacion)
                            {
                                modificaValor(nombreVariable, valor);
                            }
                        }
                        else
                        {
                            throw new Error("\nError de semantica en la linea: " + linea + ", no se le puede asignar a la variable <" + nombreVariable + "> un valor de tipo <" + dominante + ">", log);
                        }
                        modificaValor(nombreVariable, valor);
                        break;
                    case "/=":
                        match(Tipos.IncrementoFactor);
                        Expresion(evaluacionASM);
                        valorStack = stack.Pop();
                        valor = getValor(nombreVariable) / valorStack;
                        if (evaluacionASM)
                        {
                            asm.WriteLine("DIV " + nombreVariable + ", " + valorStack);
                        }
                        modificaValor(nombreVariable, valor);
                        break;
                    case "-=":
                        match(Tipos.IncrementoTermino);
                        Expresion(evaluacionASM);
                        valorStack = stack.Pop();
                        valor = getValor(nombreVariable) - valorStack;
                        if (evaluacionASM)
                        {
                            asm.WriteLine("SUB " + nombreVariable + ", " + valorStack);
                        }
                        modificaValor(nombreVariable, valor);
                        break;
                    case "+=":
                        match(Tipos.IncrementoTermino);
                        Expresion(evaluacionASM);
                        valorStack = stack.Pop();
                        valor = getValor(nombreVariable) + valorStack;
                        if (evaluacionASM)
                        {
                            asm.WriteLine("ADD " + nombreVariable + ", " + valorStack);
                        }
                        if (dominante < evaluaNumero(valor))
                        {
                            dominante = evaluaNumero(valor);
                        }
                        if (dominante <= getTipo(nombreVariable))
                        {
                            if (evaluacion)
                            {
                                modificaValor(nombreVariable, valor);
                            }
                        }
                        else
                        {
                            throw new Error("\nError de semantica en la linea: " + linea + ", no se le puede asignar a la variable <" + nombreVariable + "> un valor de tipo <" + dominante + ">", log);
                        }
                        modificaValor(nombreVariable, valor);
                        break;
                    case "%=":
                        match(Tipos.IncrementoFactor);
                        Expresion(evaluacionASM);
                        valorStack = stack.Pop();
                        valor = getValor(nombreVariable) % valorStack;
                        if (evaluacionASM)
                        {
                            asm.WriteLine("MOD " + nombreVariable + ", " + valorStack);
                        }
                        modificaValor(nombreVariable, valor);
                        break;
                }
                match(Tipos.FinSentencia);
                //Requerimiento 1.C
            }
            else
            {
                //log.Write(nombreVariable + " = ");
                match(Tipos.Asignacion);
                Expresion(evaluacionASM);
                match(";");
                float resultado = stack.Pop();
                if (evaluacionASM)
                {
                    asm.WriteLine("POP AX");
                }
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
                if (evaluacionASM)
                {
                    asm.WriteLine("MOV " + nombreVariable + ", AX");
                }
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool evaluacionASM)
        {
            string etiquetaInicioWhile = "inicioWhile" + contadorWhile;
            string etiquetaFinWhile = "finWhile" + contadorWhile++;
            match("while");
            match("(");
            //Verificar que la condicion sea verdadera
            bool validarWhile;
            string var = getContenido();
            int posicionWhile = i - getContenido().Length;
            int lineaWhile = linea;
            do
            {
                if (evaluacionASM)
                {
                    asm.WriteLine(etiquetaInicioWhile + ":");
                }
                validarWhile = Condicion(etiquetaFinWhile, evaluacionASM);
                if (!evaluacion)
                {
                    validarWhile = evaluacion;
                }
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarWhile, evaluacionASM);
                }
                else
                {
                    Instruccion(validarWhile, evaluacionASM);
                }
                if (validarWhile)
                {
                    i = posicionWhile;
                    linea = lineaWhile;
                    cambiarPosicion(i);
                    NextToken();
                }
                if (evaluacionASM)
                {
                    asm.WriteLine("JMP " + etiquetaInicioWhile);
                    asm.WriteLine(etiquetaFinWhile + ":");
                }
                evaluacionASM = false;
            } while (validarWhile);
        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool evaluacionASM)
        {
            string etiquetaInicioDo = "inicioDo" + contadorDoWhile;
            string etiquetaFinDo = "finDo" + ++contadorDoWhile;
            bool validarDo = evaluacion;
            int posicionDo = i - getContenido().Length;
            int lineaDo = linea;
            match("do");
            do
            {
                if (evaluacionASM)
                {
                    asm.WriteLine(etiquetaInicioDo + ":");
                }
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarDo, evaluacionASM);
                }
                else
                {
                    Instruccion(validarDo, evaluacionASM);
                }
                match("while");
                match("(");
                string var = getContenido();
                validarDo = Condicion(etiquetaFinDo, evaluacionASM);
                if (!evaluacion)
                {
                    validarDo = evaluacion;
                }
                if (validarDo)
                {
                    i = posicionDo;
                    linea = lineaDo;
                    cambiarPosicion(i);
                    NextToken();
                }
                if (evaluacionASM)
                {
                    asm.WriteLine("JMP " + etiquetaInicioDo);
                    asm.WriteLine(etiquetaFinDo + ":");
                }
                evaluacionASM = false;
            } while (validarDo);
            match(")");
            match(";");
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool evaluacionASM)
        {
            string etiquetaInicioFor = "InicioFor" + contadorFors;
            string etiquetaFinFor = "FinFor" + contadorFors++;
            asm.WriteLine(etiquetaInicioFor);
            match("for");
            match("(");
            Asignacion(evaluacion, evaluacionASM);
            bool validarFor = true;
            int posicionFor = i - getContenido().Length;
            int lineaFor = linea;
            while (validarFor)
            {
                if (evaluacionASM)
                {
                    asm.WriteLine(etiquetaInicioFor + ":");
                }
                validarFor = Condicion(etiquetaFinFor, evaluacionASM);
                match(";");
                if (!evaluacion)
                {
                    validarFor = false;
                }
                string nombreIncremento = getContenido();
                string tipoIncremento = incrementoFor(evaluacion == false, evaluacionASM == false);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, evaluacionASM);
                }
                else
                {
                    Instruccion(validarFor, evaluacionASM);
                }
                /*
                if (tipoIncremento == "++")
                {
                    modificaValor(nombreIncremento, getValor(nombreIncremento) + 1);
                    if (evaluacionASM)
                    {
                        asm.WriteLine("INC " + nombreIncremento);
                    }
                }
                else if (tipoIncremento == "--")
                {
                    modificaValor(nombreIncremento, getValor(nombreIncremento) - 1);
                    if (evaluacionASM)
                    {
                        asm.WriteLine("DEC " + nombreIncremento);
                    }
                }
                else if (tipoIncremento == "+=")
                {
                    float valor = stack.Pop();
                    modificaValor(nombreIncremento, getValor(nombreIncremento) + valor);
                    if (evaluacionASM)
                    {
                        asm.WriteLine("ADD " + nombreIncremento + ", " + valor);
                    }
                }
                else if (tipoIncremento == "-=")
                {
                    float valor = stack.Pop();
                    modificaValor(nombreIncremento, getValor(nombreIncremento) - valor);
                    if (evaluacionASM)
                    {
                        asm.WriteLine("SUB " + nombreIncremento + ", " + valor);
                    }
                }
                */
                else
                {
                    throw new Error("Error en el incremento del for <" + nombreIncremento + ">", log);
                }
                if (validarFor)
                {
                    //Regresar a la direccion de la posicion del archivo de texto
                    i = posicionFor;
                    linea = lineaFor;
                    cambiarPosicion(i);
                    NextToken();
                }
            }
            if (evaluacionASM)
            {
                asm.WriteLine("JMP " + etiquetaInicioFor);
                asm.WriteLine(etiquetaFinFor + ":");
            }
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(bool evaluacion, string nombreVariable, bool evaluacionASM)
        {
            float valorVariable = getValor(nombreVariable);
            float valor;
            //Requerimiento 2.C y 1.B
            if (getContenido() == "++")
            {
                match("++");
                if (evaluacionASM)
                {
                    asm.WriteLine("INC " + nombreVariable);
                }
                if (evaluacion)
                {
                    valor = valorVariable + 1;
                    if (dominante < evaluaNumero(valor))
                    {
                        dominante = evaluaNumero(valor);
                    }
                    if (dominante <= getTipo(nombreVariable))
                    {
                        if (evaluacion)
                        {
                            modificaValor(nombreVariable, valor);
                        }
                    }
                    else
                    {
                        throw new Error("Eror de semantica en la linea " + (linea) + ": No podemos asignar un <" + dominante + "> a un <" + getTipo(nombreVariable) + ">", log);
                    }
                }
            }
            else if (getContenido() == "--")
            {
                if (evaluacionASM)
                {
                    asm.WriteLine("DEC " + nombreVariable);
                }
                match("--");
                if (evaluacion)
                {
                    modificaValor(nombreVariable, (valorVariable - 1));
                }
            }
            else if (getContenido() == "+=")
            {
                match(Tipos.IncrementoTermino);
                Expresion(evaluacionASM);
                valor = stack.Pop();
                valor = valorVariable + valor;
                if (evaluacionASM)
                {
                    asm.WriteLine("ADD " + nombreVariable + ", " + valor);
                }
                if (dominante < evaluaNumero(valor))
                {
                    dominante = evaluaNumero(valor);
                }
                if (dominante <= getTipo(nombreVariable))
                {
                    if (evaluacion)
                    {
                        modificaValor(nombreVariable, valor);
                    }
                }
                else
                {
                    throw new Error("\nError de semantica en la linea: " + linea + ", no se le puede asignar a la variable <" + nombreVariable + "> un valor de tipo <" + dominante + ">", log);
                }
                modificaValor(nombreVariable, valor);
            }
            else if (getContenido() == "-=")
            {
                match(Tipos.IncrementoTermino);
                Expresion(evaluacionASM);
                valor = stack.Pop();
                valor = valorVariable - valor;
                if (evaluacionASM)
                {
                    asm.WriteLine("SUB " + nombreVariable + ", " + valor);
                }
                modificaValor(nombreVariable, valor);
            }
            else if (getContenido() == "*=")
            {
                match(Tipos.IncrementoFactor);
                Expresion(evaluacionASM);
                valor = stack.Pop();
                valor = valor * valorVariable;
                if (evaluacionASM)
                {
                    asm.WriteLine("MUL " + nombreVariable + ", " + valor);
                }
                if (dominante < evaluaNumero(valor))
                {
                    dominante = evaluaNumero(valor);
                }
                if (dominante <= getTipo(nombreVariable))
                {
                    if (evaluacion)
                    {
                        modificaValor(nombreVariable, valor);
                    }
                }
                else
                {
                    throw new Error("\nError de semantica en la linea: " + linea + ", no se le puede asignar a la variable <" + nombreVariable + "> un valor de tipo <" + dominante + ">", log);
                }
                modificaValor(nombreVariable, valor);
            }
            else
            {
                throw new Error("Error en el incremento de la variable <" + nombreVariable + ">", log);
            }
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool evaluacionASM)
        {
            match("switch");
            match("(");
            Expresion(evaluacion);
            stack.Pop();
            if (evaluacionASM)
            {
                asm.WriteLine("POP AX");
            }
            match(")");
            match("{");
            ListaDeCasos(evaluacion, evaluacionASM);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                ListaInstruccionesCase(evaluacion, evaluacionASM);
                if (getContenido() == "break")
                {
                    match("break");
                    match(";");
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool evaluacionASM)
        {
            match("case");
            Expresion(evaluacionASM);
            stack.Pop();
            if (evaluacionASM)
            {
                asm.WriteLine("POP AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, evaluacionASM);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, evaluacionASM);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool evaluacionASM)
        {
            Expresion(evaluacionASM);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(evaluacionASM);
            float e2 = stack.Pop();
            if (evaluacionASM)
            {
                asm.WriteLine("POP AX");
            }
            float e1 = stack.Pop();
            if (evaluacionASM)
            {
                asm.WriteLine("POP BX");
                asm.WriteLine("CMP AX, BX");
            }
            switch (operador)
            {
                case "==":
                    if (evaluacionASM)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }
                    return e1 == e2;
                case ">":
                    if (evaluacionASM)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    return e1 > e2;
                case ">=":
                    if (evaluacionASM)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    return e1 >= e2;
                case "<":
                    if (evaluacionASM)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    return e1 < e2;
                case "<=":
                    if (evaluacionASM)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    return e1 <= e2;
                default:
                    if (evaluacionASM)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool evaluacionASM)
        {
            string etiquetaIf = "if" + ++contadorIfs;
            string etiquetaElse = "else" + ++contadorIfs;
            match("if");
            match("(");
            bool validarIf = Condicion(etiquetaIf, evaluacionASM);
            if (!evaluacion)
            {
                validarIf = false;
            }
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, evaluacionASM);
            }
            else
            {
                Instruccion(validarIf, evaluacionASM);
            }
            if (getContenido() == "else")
            {
                match("else");
                //Validar condicion en el else
                if (evaluacionASM)
                {
                    asm.WriteLine("JMP " + etiquetaElse);
                    contadorIfs++;
                    asm.WriteLine(etiquetaIf + ":");
                }
                if (getContenido() == "{")
                {
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf, evaluacionASM);
                    }
                    else
                    {
                        BloqueInstrucciones(evaluacion = false, evaluacionASM);
                    }
                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf, evaluacionASM);
                    }
                    else
                    {
                        Instruccion(evaluacion = false, evaluacionASM);
                    }
                }
            }
            else
            {
                if (evaluacionASM)
                {
                    contadorIfs++;
                    asm.WriteLine(etiquetaIf + ":");
                }
            }
        }

        //Printf -> printf(cadena || expresion);
        private void Printf(bool evaluacion, bool evaluacionASM)
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
                Expresion(evaluacionASM);
                float resultado = stack.Pop();
                decimal resultadoEntero = (decimal)Math.Truncate(resultado);
                asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    Console.Write(resultado);
                    //Imprimir numeros usando la macro
                    if (evaluacionASM)
                    {
                        asm.WriteLine("MOV AX, " + resultadoEntero);
                        asm.WriteLine("CALL PRINT_NUM");
                    }
                    /*
                    asm.WriteLine("PRINT " + "\"PARTE ENTERA:\"");
                    asm.WriteLine("MOV AX," + Math.Truncate(resultado));
                    asm.WriteLine("CALL PRINT_NUM");
                    asm.WriteLine("PRINT " + "\"\"");
                    asm.WriteLine("PRINT " + "\"PARTE DECIMAL:\"");
                    asm.WriteLine("MOV AX, DX");
                    asm.WriteLine("CALL PRINT_NUM");
                    */
                }
            }
            match(")");
            match(Tipos.FinSentencia);
        }

        //Scanf -> scanf(cadena  -> , -> & -> identificador);
        private void Scanf(bool evaluacion, bool evaluacionASM)
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
                if (evaluacionASM)
                {
                    asm.WriteLine("CALL SCAN_NUM ");
                    asm.WriteLine("MOV " + getContenido() + ", CX");
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
            BloqueInstrucciones(true, true);
        }

        //Expresion -> Termino MasTermino
        private void Expresion(bool evaluacionASM)
        {
            Termino(evaluacionASM);
            MasTermino(evaluacionASM);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool evaluacionASM)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(evaluacionASM);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if (evaluacionASM)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if (evaluacionASM)
                {
                    asm.WriteLine("POP AX");
                }
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool evaluacionASM)
        {
            Factor(evaluacionASM);
            PorFactor(evaluacionASM);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool evaluacionASM)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(evaluacionASM);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if (evaluacionASM)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if (evaluacionASM)
                {
                    asm.WriteLine("POP AX");
                }
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    //Requerimiento 1.A
                    case "%":
                        stack.Push(n2 % n1);
                        if (evaluacionASM)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool evaluacionASM)
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
                if (evaluacionASM)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");
                }
                match(Tipos.Numero);
            }
            //Si es una variable
            else if (getClasificacion() == Tipos.Identificador)
            {
                string variable = getContenido();
                if (evaluacionASM)
                {
                    asm.WriteLine("MOV BX, " + variable);
                    asm.WriteLine("PUSH BX");
                }
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
                Expresion(evaluacionASM);
                match(")");
                //Si hay un casteo hay que convertir el valor
                if (huboCasteo)
                {
                    //Casteo y conversion de tipos
                    //Actualizamos el dominante al casteo realizado y obtenemos el valor del stack para convertirlo al adecuado
                    dominante = casteo;
                    float valor = stack.Pop();
                    if (evaluacionASM)
                    {
                        asm.WriteLine("POP AX");
                    }
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
                    if (evaluacionASM)
                    {
                        asm.WriteLine("MOV AX, " + valor);
                        asm.WriteLine("PUSH AX");
                    }
                }
            }
        }
    }
}