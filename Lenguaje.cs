//Olvera Morales Miguel Angel
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*              Requerimientos
* 1. Actualizar el dominante para variables en la expresion ---------------------> Hecho
*       Ejemplo: float x;       char y;       y = x;
*
* 2. Actualizar el dominante para el casteo y el valor de la subexpresion -------------------> Hecho
*
* 3. Programar un metodo de conversion de un valor a un tipo de dato ----------------------> Hecho
*       private float convert(float valor, string tipoDato)
*       Deberan usar el residuo de la division %255, %65535  
*
* 4. Evaluar nuevamente la condicion del if, while, do while con respecto al parametro que reciben  ---------------> Hecho
*
* 5. Levantar una excepcion en el scanf cuando la captura no sea un numero ----------------------> Hecho 
*
* 6. Ejecutar el for con respecto a los parametros que recibe ----------------------> Hecho
*/


namespace Semantica
{
    public class Lenguaje: Sintaxis
    {
        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();
        Variable.TipoDato dominante;
        public Lenguaje()
        {

        }
        public Lenguaje(string nombre) : base(nombre)
        {

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
            foreach(Variable v in variables)
            {
                log.WriteLine(v.getNombre() + ", " + v.getTipoDato() + ", " + v.getValor());
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }
            return false;
        }
        private void modificaValor(string nombre, float nuevoValor) 
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombre)) 
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable)) 
                {
                    return v.getValor();
                }
            }
            return 0;
        }

        private Variable.TipoDato getTipo(string nombreVariable)
        {
            foreach(Variable v in variables)
            {
                if(v.getNombre().Equals(nombreVariable)) 
                {
                    return v.getTipoDato();
                }
            }
            return Variable.TipoDato.Char;
        }
        //Requerimiento 6, metodo para volver a otra posicion del archivo
        //Se usa Position y seek, para ambos hay que limpiar el buffer
        private void cambiarPosicion(int posicion)
        {
            //Limpiamos el buffer y actualizamos la posicion
            archivo.DiscardBufferedData();
            archivo.BaseStream.Position = posicion;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Libreria();
            Variables();
            Main();
            displayVariables();
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
                switch(getContenido())
                {
                    case "int": tipo    = Variable.TipoDato.Int;    break;
                    case "float": tipo  = Variable.TipoDato.Float;  break;
                    default: tipo       = Variable.TipoDato.Char;   break;
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
                if(!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <"+ getContenido() + "> en la linea: " + linea, log);
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
            if (getContenido() != "case" && getContenido() !=  "break" && getContenido() != "default" && getContenido() != "}")
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
            else if(getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if(getContenido() == "for")
            {
                For(evaluacion);
            }
            else if(getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
            }
        }
        private Variable.TipoDato evaluaNumero(float resultado){
            if(resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }
            if (resultado <=255)
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
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }
            log.WriteLine();
            string nombreVariable = getContenido();
            match(Tipos.Identificador);
            log.Write(nombreVariable + " = ");
            match(Tipos.Asignacion);
            //Declaramos el dominante
            dominante = Variable.TipoDato.Char;
            Expresion();
            match(";");
            float resultado = stack.Pop();
            log.WriteLine("= " + resultado);
            log.WriteLine();
            //Actualizamos dominantes
            if(dominante < evaluaNumero(resultado))
            {
                dominante = evaluaNumero(resultado);
            }
            if(dominante <= getTipo(nombreVariable))
            {
                //Si el valor se puede asignar al tipo de dato actualizamos su valor
                if(evaluacion)
                {
                    modificaValor(nombreVariable, resultado);
                } 
            }
            //De lo contrario levantamos una excepcion
            else
            {
                throw new Error("Eror de semantica en la linea " + (linea - 1) + ": No podemos asignar un <" + dominante + "> a un <" + getTipo(nombreVariable) + ">", log); 
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");
            match("(");
            //Requerimiento 4, verificar que la condicion sea verdadera
            bool validarWhile = Condicion();
            if(evaluacion == false)
            {
                validarWhile = false;
            }
            match(")");
            if (getContenido() == "{") 
            {
                //Validacion de la condicion
                if(validarWhile)
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
                //Requerimiento 4
                if(validarWhile)
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
            //Requerimiento 4
            bool validarDo = Condicion();
            if(evaluacion == false)
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
            //Requerimiento 4, verificar que la condicion sea verdadera
            bool validarFor = true;
            //Requerimiento 6, a) guardar la direccion de la posicion del archivo de texto
            int posicionFor = i - getContenido().Length;
            int lineaFor = linea;
            //b) Metemos un ciclo (while) para despues validar el for
            while(validarFor)
            {
                validarFor = Condicion();
                if(evaluacion == false)
                {
                    validarFor = false;
                }
                match(";");
                Incremento(validarFor);
                match(")");
                if (getContenido() == "{")
                {
                BloqueInstrucciones(validarFor);  
                }
                else
                {
                    Instruccion(validarFor);
                }
                if(validarFor)
                {
                    //Requerimiento 6, c) regresar a la direccion de la posicion del archivo de texto
                    i = posicionFor;
                    linea = lineaFor;
                    cambiarPosicion(i);
                    //Requerimiento 6, d) sacar otro token
                    NextToken();
                }
            }
        }

        //Incremento -> Identificador ++ | --
        private void Incremento(bool evaluacion)
        {
            //Verificamos si la variable existe
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }
            string variable = getContenido();
            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                if(evaluacion)
                {
                    modificaValor(variable, getValor(variable) + 1);
                } 
                match("++");
            }
            else if(getContenido() == "--")
            {
                if(evaluacion)
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
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if(getContenido() == "default")
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
            match(":");
            ListaInstruccionesCase(evaluacion);
            if(getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if(getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion()
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            switch(operador)
            {
                case "==":
                    return e1 == e2;
                case ">": 
                    return e1 >  e2;
                case ">=":
                    return e1 >= e2;
                case "<":
                    return e1 <  e2;
                case "<=":
                    return e1 <= e2;
                default:
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            match("if");
            match("(");
            bool validarIf = Condicion();
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
                //Requerimiento 4, validar condicion en el else
                if (getContenido() == "{")
                {
                    if(evaluacion)
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
                    //Requerimiento 4
                    if(evaluacion)
                    {
                        Instruccion(!validarIf);
                    }
                    else
                    {
                        Instruccion(evaluacion = false);
                    }
                }
            }
        }

        //Printf -> printf(cadena || expresion);
        private void Printf(bool evaluacion)
        {
            match("printf");
            match("(");
            if(getClasificacion() == Tipos.Cadena)
            {
                if(evaluacion)
                {
                    secuenciasEscape(getContenido());
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                if(evaluacion){
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
            if(!existeVariable(getContenido()))
            {
                throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
            }
            if(evaluacion)
            {
                string valor = "" + Console.ReadLine();
                float valorNumerico;
                //Requerimiento 5
                //El readLine debe capturar un numero, pero si el usuario ingresa una cadena debemos levantar una excepcion
                //si el valor es un numero, modifica el valor, sino levantamos una excepcion de que no es un numero
                if(float.TryParse(valor, out valorNumerico))
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
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "+": stack.Push(n2 + n1); break;
                    case "-": stack.Push(n2 - n1); break;
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
                float n2 = stack.Pop();
                switch (operador)
                {
                    case "*": stack.Push(n2 * n1); break;
                    case "/": stack.Push(n2 / n1); break;
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
                if(dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            //Si es una variable
            else if (getClasificacion() == Tipos.Identificador)
            {
                log.Write(getContenido() + " ");
                //Verificamos si la variable existe
                if(!existeVariable(getContenido()))
                {
                    throw new Error("\nError de sintaxis en la linea: " + linea + ", la variable <"+ getContenido() + "> no existe", log);
                }
                //Actualizamos el dominante
                if(dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);
            }
            //Si hay un casteo obtenemos el tipo de dato y verificamos la sintaxis
            else
            {
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                bool huboCasteo = false;
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    //Requerimiento 2: Casteo y conversion de tipos
                    huboCasteo = true;
                    switch(getContenido())
                    {
                        case "char": casteo     = Variable.TipoDato.Char;   break;
                        case "int":     casteo  = Variable.TipoDato.Int;    break;
                        case "float": casteo    = Variable.TipoDato.Float;  break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                //Requerimiento 2: Si hay un casteo hay que convertir el valor
                if(huboCasteo)
                {
                    //Requerimiento 2: Casteo y conversion de tipos
                    //Actualizamos el dominante al casteo realizado y obtenemos el valor del stack para convertirlo al adecuado
                    dominante = casteo;
                    float valor = stack.Pop();
                    //Nombre de la variable para actualizar el valor
                    string nombre = getContenido();
                    //Obtenemos el residuo acorde al tipo de dato
                    switch(casteo)
                    {
                        case Variable.TipoDato.Int:  valor = valor % 65536; break;
                        case Variable.TipoDato.Char: valor = valor % 256;   break;
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