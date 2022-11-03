;Archivo: C:\Users\wachi\OneDrive\Escritorio\AUTOMATAS\Semantica\prueba.cpp
;Fecha: 31/10/2022 9:59:16
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
MOV AX, 10
PUSH AX
POP AX
MOV AX, 10
CALL PRINT_NUM
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
END
