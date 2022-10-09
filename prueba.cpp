//Olvera Morales Miguel Angel
#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado, x;
int a, d, altura, i, j;
char y;
// Este programa calcula el volumen de un cilindro.
void main(){
    //Requerimiento 5.- Levanta una excepcion en el scanf si la captura no es un numero
    printf("Introduce la altura de la piramide: ");
    scanf("altura", &altura);

    //Requerimiento 6.- Ejecutar el for y for anidado
    if(altura >2)
        for(i = altura; i > 0; i--)
        {
            for(j = 0; j < altura-i; j++){
                if(j!=2){
                    printf("*");
                }
                else{
                    printf("-");//Requerimiento 4.- evalua nuevamente los else
                }
            }
            printf("\n");
        }
    else
        printf("\nError: la altura debe de ser mayor que 2\n");
    if(1 != 1){
        printf("\nEsto no se debe imprimir");
        if(2 == 2){
            printf("\nEsto tampoco");     //Requerimiento 4.- evalua nuevamente los if respecto al parametro que reciben
        }
    }
    a = 256;
    printf("\nValor de variable int a antes del casteo: ");
    printf(a);
    y = (char)(a);  //Requerimiento 2 y 3, actualiza el dominante y convierte el valor con una funcion
    printf("\nValor de variable char y despues del casteo de a: ");
    printf(y);

    for(j = 0; j < altura-1; j++){
        if(j!=2){
            printf("*");
        }
        else{
            printf("-");//Requerimiento 4.- evalua nuevamente los else
        }
    }
    for(i = 0; i < altura; i++)
    {
        printf("\n");
        printf(i);   
    }
    printf("\nA continuacion se intenta asignar un int a un char sin usar casteo: \n");
    y = a; //Requerimiento 1.- debe marcar error
}