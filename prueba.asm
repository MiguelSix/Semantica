;Archivo: prueba.asm
;Fecha: 08/11/2022 16:28:19
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
inicioWhile0:
MOV AX, j
PUSH AX
MOV AX, 3
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE finWhile0
PRINTN ""
PRINT ""
PRINT "Hola"
MOV AX, 1
PUSH AX
ADD j, 1
JMP inicioWhile0
finWhile0:
MOV AX, 4C00H
INT 21H
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
