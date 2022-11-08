;Archivo: prueba.asm
;Fecha: 08/11/2022 7:19:42
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
PRINTN ""
PRINTN "Holaaa"
PRINT "coco"
MOV AX, 13
PUSH AX
POP AX
MOV a, AX
MOV AX, 5
PUSH AX
MOV BX, AX
MOV AX,a
DIV BX
MOV a, DX
PRINTN ""
PRINT ""
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
