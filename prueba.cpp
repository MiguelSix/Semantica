// Olvera Morales Miguel Angel
#include <iostream>
#include <stdio.h>
#include <conio.h>
float area, radio, pi, resultado, x;
int a, d, altura, i, j, y;
char z;
// Este programa calcula el volumen de un cilindro.
void main()
{

    for (j = 0; j < 10; j += 2)
    {
        printf(j);
        printf("\n");
    }

    i = 0;
    while (i < 10)
    {
        printf("\n");
        printf(i);
        i++;
    }

    printf("\n");

    a = 2;
    d = 3;
    printf("\n");
    printf("\n");
    printf(a);
    printf("\n");
    printf(d);
    printf("\n");

    if (a > d)
    {
        printf("a es mayor que d");
    }
    else
    {
        d = 2;
        printf("d es mayor que a");
        if (a == d)
        {
            printf("\na es igual a d");
        }
    }

    printf("\n");
    printf("\n");

    a = 0;
    do{
        printf(a);
        printf("\n");
        a++;
    } while (a < 10);

    printf("\n");
    a = 15;
    a-= 5;
    printf(a);
}