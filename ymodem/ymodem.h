#ifndef YMODEM_H
#define YMODEM_H

#include <stdint.h>

#define PACKET_SEQNO_INDEX      (1)
#define PACKET_SEQNO_COMP_INDEX (2)

#define PACKET_HEADER           (3)     /* start, block, block-complement */
#define PACKET_TRAILER          (2)     /* CRC bytes */
#define PACKET_OVERHEAD         (PACKET_HEADER + PACKET_TRAILER)
#define PACKET_SIZE             (128)
#define PACKET_1K_SIZE          (1024)
#define PACKET_TIMEOUT          (1)

#define FILE_NAME_LENGTH (128)
#define FILE_SIZE_LENGTH (16)

/* ASCII control codes: */
#define SOH (0x01)      /* start of 128-byte data packet */
#define STX (0x02)      /* start of 1024-byte data packet */
#define EOT (0x04)      /* end of transmission */
#define ACK (0x06)      /* receive OK */
#define NAK (0x15)      /* receiver error; retry */
#define CAN (0x18)      /* two of these in succession aborts transfer */
#define CRC (0x43)      /* use in place of first NAK for CRC mode */
/* Extended ASCII control codes: */
#define EXT (0x80)
#define ROF	(EXT & SOH)	/* send file receive request */

/* Number of consecutive receive errors before giving up: */
#define MAX_ERRORS    (5)

#define REPEAT 0
#define NEXT 1

#pragma pack (1)
typedef struct {
	uint8_t command;
	uint8_t counter;
	uint8_t counterInv;
	uint8_t name[FILE_NAME_LENGTH];
	char sizeAscii[FILE_SIZE_LENGTH];
	uint16_t crc16;
} YModem_Header_t;

typedef struct {
	uint8_t command;
	uint8_t counter;
	uint8_t counterInv;
	uint8_t data[PACKET_SIZE];
	uint16_t crc16;
} YModem_Packet_t;
#pragma pack (1)

extern "C" __declspec(dllexport) int YModem_Request(char* _name, int size);
extern "C" __declspec(dllexport) int YModem_Start(char* _name, int size);
extern "C" __declspec(dllexport) int YModem_Send(uint8_t* _data, int _next);
extern "C" __declspec(dllexport) int YModem_SendRaw(uint8_t command, uint8_t * _data, int _next);
extern "C" __declspec(dllexport) int YModem_End(void);

#endif /* YMODEM_H */