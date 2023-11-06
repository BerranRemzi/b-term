// ymodem.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

//#include <iostream>
#include <stdio.h>
#include <stdint.h>
#include "ymodem.h"

uint8_t data0[] = 
"// Run program: Ctrl + F5 or Debug > Start Without Debugging menu\
// Debug program: F5 or Debug > Start Debugging menu\
\
// Tips for Getting Started: \
//   1. Use the Solution Explorer window to add/manage files\
//   2. Use the Team Explorer window to connect to source control\
//   3. Use the Output window to see build output and other messages\
//   4. Use the Error List window to view errors\
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project\
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file\
";

uint8_t data1[] = "Hello World";

int main()
{
    uint8_t *pData = data1;
    printf("ymodem - BerranRemzi\n");
    //printf("\nYModem_Start\n");
    YModem_Start("test.txt", sizeof(data1));

    //printf("\nYModem_Send\n");
    int end = 1;
    while (end) {
        end = YModem_Send(pData, 1);
        //if(received == ACK)
        pData += PACKET_SIZE;
    }
    YModem_End();
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
