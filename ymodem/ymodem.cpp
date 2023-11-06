#include "ymodem.h"
#include <stdint.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>


YModem_Header_t header = {};
YModem_Packet_t packet = {};
uint8_t* pHeader = (uint8_t*)&header;
uint8_t* pPacket = (uint8_t*)&packet;
int bytesToSend = 0;

/* http://www.ccsinfo.com/forum/viewtopic.php?t=24977 */
static unsigned short crc16(const unsigned char* buf, unsigned long count)
{
	unsigned short crc = 0;
	int i;

	while (count--) {
		crc = crc ^ *buf++ << 8;

		for (i = 0; i < 8; i++) {
			if (crc & 0x8000) {
				crc = crc << 1 ^ 0x1021;
			}
			else {
				crc = crc << 1;
			}
		}
	}
	return crc;
}

int YModem_Start(char* _name, int size) {
	printf("\n>%s\n", __func__);
	bytesToSend = size;
	memset(pHeader, 0, sizeof(YModem_Header_t));
	header.command = SOH;
	header.counter = 0;
	header.counterInv = ~header.counter;
	memcpy(header.name, _name, strlen(_name));
	sprintf_s(header.sizeAscii, 12, "%d", size);
	header.crc16 = crc16(pHeader, sizeof(YModem_Header_t) - sizeof(header.crc16));

#if 0
	for (int i = 0; i < sizeof(YModem_Packet_t); i++) {
		printf("%02X", pPacket[i]);
	}
#else
	for (int i = 0; i < sizeof(header.name); i++) {
		printf("%c", header.name[i]);
	}
	printf(" %s", header.sizeAscii);

#endif

	return 0;
}

int YModem_Send(uint8_t* _data, int _next) {
	return YModem_SendRaw(SOH, _data, _next);
}

int YModem_Request(char* _data, int _next) {
	return YModem_SendRaw(ROF, (uint8_t*)_data, _next);
}

int YModem_SendRaw(uint8_t command, uint8_t* _data, int _next) {
	printf("\n>%s\n", __func__);
	memset(pPacket, 0, sizeof(YModem_Packet_t));
	packet.command = command;
	packet.counter += _next;
	packet.counterInv = ~packet.counter;
	memcpy(packet.data, _data, bytesToSend < sizeof(packet.data) ? bytesToSend : sizeof(packet.data));
	packet.crc16 = crc16(pPacket, sizeof(YModem_Packet_t) - sizeof(packet.crc16));

#if 0
	for (int i = 0; i < sizeof(YModem_Packet_t); i++) {
		printf("%02X", pPacket[i]);
	}
#else
	for (int i = 0; i < sizeof(packet.data); i++) {
		printf("%c", packet.data[i]);
	}
#endif

	if (_next) {
		if (bytesToSend > sizeof(packet.data)) {
			bytesToSend -= sizeof(packet.data);
		}
		else {
			bytesToSend = 0;
		}
	}

	return bytesToSend;
}

int YModem_End(void) {
	printf("\n>%s\n", __func__);
	uint8_t command = EOT;
	printf("%02X", command);
	return 0;
}