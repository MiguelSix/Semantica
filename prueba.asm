;Archivo: prueba.asm
;Fecha: 07/11/2022 23:10:00
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
MOV AX, 4C00H
INT 21H
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
