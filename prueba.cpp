// Olvera Morales Miguel Angel
#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado, x;
int a, d, altura, i, j, y, k;
char z;
// Este programa calcula el volumen de un cilindro.
void main()
{
    /*
    printf("\nHolaaa\ncoco");
    a = 13;
    a %= 5;
    printf("\n");
    printf(a);
    printf("\n");

    d = 10;
    d -= 3;
    printf("\n");
    printf(d);
    printf("\n");

    j = 10;

    if(j%4 != 0){
        printf("\n");
        printf("El numero es impar");
    }
    else{
        printf("\n");
        printf("El numero es par");
    }

    j = 0;
    printf("\n");
    */
/*
    for (i = 0; i < 4; i++)
    {
        j = i;
        while (j % 2 != 0)
        {
            printf("\n");
            printf(j);
            printf("\n");
            printf("El numero es impar");
            j++;
        }
    }

    printf("\n");
    printf("Introduce la altura de la piramide: ");
    scanf("altura", &altura);
    if(altura > 2)
    {
        for (i = altura; i > 0; i-=1)
        {
            j = 0;
            while (j < altura - i)
            {
                printf("*");
                j+=1;
            }
            printf("\n");
        }
    }

    */

     //Requerimiento 5.- Levanta una excepcion en el scanf si la captura no es un numero
     printf("Introduce la altura de la piramide: ");
     scanf("altura", &altura);
     //Requerimiento 6.- Ejecutar el for y for anidado
     if(altura >2) {
         for(i = altura; i > 0; i-=1){
             j = 0;
             while(j < altura-i){
                 if(j%2 == 0){
                     printf("*");
                 }
                 else{
                     printf("-");//Requerimiento 4.- evalua nuevamente los else
                 }
                 j+=1;
             }
             printf("\n");
         }
         k = 0;
         do
         {
             printf("-");
             k += 2;
         } while(k < altura*2);
         printf("\n");
     }else
     {
         printf("\nError: la altura debe de ser mayor que 2\n");
     }
     if(1 != 1){
        printf("Esto no se debe imprimir");
        if(2 == 2){
            printf("Esto tampoco");     //Requerimiento 4.- evalua nuevamente los if respecto al parametro que reciben
        }
    }
    a = 258;
    printf("Valor de variable int 'a' antes del casteo: ");
    printf(a);
    y = (char)(a);  //Requerimiento 2 y 3, actualiza el dominante y convierte el valor con una funcion
    printf("\nValor de variable char 'y' despues del casteo de a: ");
    printf(y);
    printf("\nA continuacion se intenta asignar un int a un char sin usar casteo: \n");
    //y = a; //Requerimiento 1.- debe marcar error 
}