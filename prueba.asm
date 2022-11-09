;Archivo: prueba.asm
;Fecha: 08/11/2022 18:57:34
#make_COM#
include 'emu8086.inc'
ORG 100h
;Variables:
	area DW ?
	radio DW ?
	pi DW ?
	resultado DW ?
	x DW ?
	a DW ?
	d DW ?
	altura DW ?
	i DW ?
	j DW ?
	y DW ?
	k DW ?
	z DW ?
PRINT "Introduce la altura de la piramide: "
CALL SCAN_NUM 
MOV altura, CX
PRINT ''
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if1
MOV AX, altura
PUSH AX
POP AX
MOV i, AX
InicioFor0:
MOV AX, i
PUSH AX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE FinFor0
MOV AX, 1
PUSH AX
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
inicioWhile0:
MOV AX, j
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, i
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finWhile0
MOV AX, j
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX, 0
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if2
PRINT "*"
JMP else2
if2:
PRINT "-"
else2:
MOV AX, 1
PUSH AX
ADD j, 1
JMP inicioWhile0
finWhile0:
PRINTN ""
PRINT ""
SUB i, 1

JMP InicioFor0
FinFor0:
MOV AX, 0
PUSH AX
POP AX
MOV k, AX
inicioDo0:
PRINT "-"
MOV AX, 2
PUSH AX
ADD k, 2
MOV AX, k
PUSH AX
MOV AX, altura
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finDo0
JMP inicioDo0
finDo0:
PRINTN ""
PRINT ""
JMP else1
if1:
PRINTN ""
PRINTN "Error: la altura debe de ser mayor que 2"
PRINT ""
else1:
MOV AX, 1
PUSH AX
MOV AX, 1
PUSH AX
POP BX
POP AX
CMP AX, BX
JE if3
PRINT "Esto no se debe imprimir"
MOV AX, 2
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if4
PRINT "Esto tampoco"
JMP else4
if4:
else4:
JMP else3
if3:
else3:
MOV AX, 258
PUSH AX
POP AX
MOV a, AX
PRINT "Valor de variable int 'a' antes del casteo: "
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX, a
PUSH AX
POP AX
MOV AX, 2
PUSH AX
POP AX
MOV y, AX
PRINTN ""
PRINT "Valor de variable char 'y' despues del casteo de a: "
MOV AX, y
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
PRINTN "A continuacion se intenta asignar un int a un char sin usar casteo: "
PRINT ""
MOV AX, 4C00H
INT 21H
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
