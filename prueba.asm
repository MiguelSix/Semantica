;Archivo: C:\Users\wachi\OneDrive\Escritorio\AUTOMATAS\Semantica\prueba.cpp
;Fecha: 21/10/2022 9:55:07
#make_COM#
include 'emu8086.inc'
ORG 1000h
;Variables:
	area DB ?
	radio DB ?
	pi DB ?
	resultado DB ?
	x DB ?
	a DB ?
	d DB ?
	altura DB ?
	i DB ?
	j DB ?
	y DB ?
MOV AX, 3
PUSH AX
MOV AX, 5
PUSH AX
POP BX
POP AX
ADD AX, BX
PUSH AX
MOV AX, 8
PUSH AX
POP BX
POP AX
MUL BX
PUSH AX
MOV AX, 10
PUSH AX
MOV AX, 4
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
MOV AX, 2
PUSH AX
POP BX
POP AX
DIV BX
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP AX
MOV y, AX
RET
END
