;Archivo: prueba.asm
;Fecha: 07/11/2022 22:30:57
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
	z DW ?
MOV AX, 0
PUSH AX
POP AX
MOV j, AX
InicioFor0:
MOV AX, j
PUSH AX
MOV AX, 10
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE FinFor0
MOV AX, 2
PUSH AX
MOV AX, j
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
ADD j, 2

JMP InicioFor0
FinFor0:
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
MOV AX, 0
PUSH AX
POP AX
MOV i, AX
inicioWhile1:
MOV AX, i
PUSH AX
MOV AX, 10
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finWhile1
PRINTN ""
MOV AX, i
PUSH AX
POP AX
CALL PRINT_NUM
INC i
JMP inicioWhile1
finWhile1:
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
MOV AX, 2
PUSH AX
POP AX
MOV a, AX
MOV AX, 3
PUSH AX
POP AX
MOV d, AX
PRINTN ""
PRINTN ""
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
MOV AX, d
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
MOV AX, a
PUSH AX
MOV AX, d
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if1
JMP else1
if1:
MOV AX, 2
PUSH AX
POP AX
MOV d, AX
PRINTN "d es mayor que a"
MOV AX, a
PUSH AX
MOV AX, d
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if2
PRINTN "a es igual a d"
JMP else2
if2:
else2:
else1:
PRINTN ""
PRINTN ""
MOV AX, 0
PUSH AX
POP AX
MOV a, AX
inicioDo0:
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
PRINTN ""
INC a
MOV AX, a
PUSH AX
MOV AX, 10
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finDo0
JMP inicioDo0
finDo0:
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
PRINTN ""
MOV AX, 15
PUSH AX
POP AX
MOV a, AX
MOV AX, 5
PUSH AX
SUB a, 5
MOV AX, a
PUSH AX
POP AX
CALL PRINT_NUM
MOV AX, 4C00H
INT 21H
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
