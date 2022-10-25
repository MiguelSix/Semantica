;Archivo: C:\Users\wachi\OneDrive\Escritorio\AUTOMATAS\Semantica\prueba.cpp
;Fecha: 25/10/2022 9:49:53
#make_COM#
include 'emu8086.inc'
ORG 1000h
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
MOV AX, 61
PUSH AX
POP AX
MOV y, AX
MOV AX, 61
PUSH AX
MOV AX, 61
PUSH AX
POP AX
POP BX
CMP AX, BX
JNE if1
MOV AX, 10
PUSH AX
POP AX
MOV x, AX
if1:
RET
