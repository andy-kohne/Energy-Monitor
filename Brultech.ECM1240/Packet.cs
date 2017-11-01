using System;
using System.Text;

namespace Brultech.ECM1240
{
    public class Packet
    {
        /// <summary>
        /// Line voltage, with one decimal place resolution.
        /// </summary>
        /// <remarks>From "Installation Guide", voltage readings are derived from the AC secondary of the wall transformer.</remarks>
        public decimal Volt { get; }

        /// <summary>
        /// The concatenated number is used to determine the origin of the packet when reading datafrom multiple ECM-1240 devices.
        /// </summary>
        /// <remarks>Concatenation of the unit id and 5 digits of the device id.</remarks>
        public string SerialNumber => string.Format("{1}{0:00000}", DeviceAddress, UnitId);

        public ulong Ch1AbsoluteWsCounter { get; }
        public ulong Ch2AbsoluteWsCounter { get; }
        public ulong Ch1PolarizedWsCounter { get; }
        public ulong Ch2PolarizedWsCounter { get; }
        public decimal Ch1Current { get; }
        public decimal Ch2Current { get; }

        /// <summary>
        /// This is a continuous counter which rolls over after 16777216 seconds.
        /// </summary>
        /// <remarks>        
        /// This three-byte counter increments every second and is synchronized with the ECM-1220.
        /// The software needs to
        /// be aware that the counter rolls over past this value and make provisions for proper
        /// adjustment when this occurs.
        /// </remarks>
        public uint SecondsCounter { get; }

        public uint Aux1WsCounter { get; }
        public uint Aux2WsCounter { get; }
        public uint Aux3WsCounter { get; }
        public uint Aux4WsCounter { get; }
        public uint Aux5WsCounter { get; }

        public decimal DcVoltage { get; }
        public ushort DeviceAddress { get; }
        public byte UnitId { get; }

        /// <summary>
        /// Counter indicating the number of times the unit has been reset
        /// </summary>
        /// <value>The counter counts from 0 to 7, then rolls over back to zero.</value>
        /// <remarks>
        /// This counter is incremented whenever the ECM-1220
        /// is reset either manually or using a software command. Resetting the ECM-1220 causes
        /// all KWh, elapsed time, and data-logger memory to be zeroed. When this occurs, the
        /// 3-bit counter described here will increment by one. 
        /// </remarks>
        public byte ResetCounter { get; }

        public bool Ch1PolarityBit { get; }
        public bool Ch2PolarityBit { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Device ID: {0}", DeviceAddress).AppendLine();
            sb.AppendFormat("Seconds: {0}", SecondsCounter).AppendLine();
            sb.AppendFormat("Ch1- Abs WS: {0}, Pol WS: {1}", Ch1AbsoluteWsCounter, Ch1PolarizedWsCounter).AppendLine();
            sb.AppendFormat("Ch2- Abs WS: {0}, Pol WS: {1}", Ch2AbsoluteWsCounter, Ch2PolarizedWsCounter).AppendLine();
            sb.AppendFormat("Aux1 WS: {0}", Aux1WsCounter).AppendLine();
            sb.AppendFormat("Aux2 WS: {0}", Aux2WsCounter).AppendLine();
            sb.AppendFormat("Aux3 WS: {0}", Aux3WsCounter).AppendLine();
            sb.AppendFormat("Aux4 WS: {0}", Aux4WsCounter).AppendLine();
            sb.AppendFormat("Aux5 WS: {0}", Aux5WsCounter).AppendLine();
            return sb.ToString();
        }

        public static void ValidatePacket(byte[] packetBytes)
        {
            // ensure correct length
            if (packetBytes.Length != 65)
                throw new ArgumentException("size incorrect", nameof(packetBytes));
            //calculate checksum
            var checksum = 0;
            for (var i = 0; i <= 63; i++)
            {
                checksum += packetBytes[i];
            }
            checksum = checksum % 256;
            //evaluate the checksum
            if (checksum != packetBytes[64])
                throw new PacketException();
        }

        public Packet(byte[] packetBytes)
        {
            ValidatePacket(packetBytes);

            //parse the data
            //byte 4: Volt Hi Byte
            //byte 5: Volt Low Byte
            //The value of “Volt” must be divided by ten. This provides one decimal place resolution
            Volt = packetBytes.GetBigEndianUShort(3) / 10;


            //channel 1 & 2 counters
            //ch1 absolute ws byte 5 (LSB) -> byte 9 (MSB)
            Ch1AbsoluteWsCounter = packetBytes.Get5ByteCounter(5);
            //ch2 absolute ws byte 10 (LSB) -> byte 14 (MSB)
            Ch2AbsoluteWsCounter = packetBytes.Get5ByteCounter(10);
            //ch1 polarized ws byte 15 (LSB) -> byte 19 (MSB)
            Ch1PolarizedWsCounter = packetBytes.Get5ByteCounter(15);
            //ch2 polarized ws byte 20 (LSB) -> byte 24 (MSB)
            Ch2PolarizedWsCounter = packetBytes.Get5ByteCounter(20);

            //bytes 25 -> 28 reserved

            //pre-programmed serial number
            //byte 29: serial number Hi byte
            //byte 30: serial number Low byte
            DeviceAddress = BitConverter.ToUInt16(packetBytes, 29);

            //parse the flag byte
            ResetCounter = (byte)(packetBytes[31] & 0x7);
            Ch1PolarityBit = (packetBytes[31] & 0x8) != 0;
            Ch2PolarityBit = (packetBytes[31] & 0x10) != 0;

            //pre-programmed id byte
            UnitId = packetBytes[32];

            //channel 1 & 2 current, value represents 1/100th of an amp
            //ch1 current: byte 33 (LSB) -> byte 34 (MSB)
            Ch1Current = BitConverter.ToUInt16(packetBytes, 33) / 100;
            //ch2 current: byte 35 (LSB) -> byte 36 (MSB)
            Ch2Current = BitConverter.ToUInt16(packetBytes, 35) / 100;

            //seconds counter
            //byte 37 (LSB) -> byte 39 (MSB)
            SecondsCounter = packetBytes.Get3ByteCounter(37);

            //aux counters; 4 byte watt-second counters
            //aux 1: byte 40 (LSB) -> byte 43 (MSB)
            Aux1WsCounter = packetBytes.Get4ByteCounter(40);
            //aux 2: byte 44 (LSB) -> byte 47 (MSB)
            Aux2WsCounter = packetBytes.Get4ByteCounter(44);
            //aux 3: byte 48 (LSB) -> byte 51 (MSB)
            Aux3WsCounter = packetBytes.Get4ByteCounter(48);
            //aux 4: byte 52 (LSB) -> byte 55 (MSB)
            Aux4WsCounter = packetBytes.Get4ByteCounter(52);
            //aux 5: byte 56 (LSB) -> byte 59 (MSB)
            Aux5WsCounter = packetBytes.Get4ByteCounter(56);

            //dc voltage: byte 60 (LSB) -> byte 61 (MSB)
            DcVoltage = BitConverter.ToUInt16(packetBytes, 60) / 100;

        }

        public Packet(ushort deviceAddress, byte unitId, decimal volt, ulong ch1AbsoluteWsCounter, ulong ch2AbsoluteWsCounter, ulong ch1PolarizedWsCounter, ulong ch2PolarizedWsCounter, decimal ch1Current, decimal ch2Current, uint secondsCounter, uint aux1WsCounter, uint aux2WsCounter, uint aux3WsCounter, uint aux4WsCounter, uint aux5WsCounter,
            decimal dcVoltage, byte resetCounter, bool ch1PolarityBit, bool ch2PolarityBit)
        {
            DeviceAddress = deviceAddress;
            UnitId = unitId;
            Volt = volt;
            Ch1AbsoluteWsCounter = ch1AbsoluteWsCounter;
            Ch2AbsoluteWsCounter = ch2AbsoluteWsCounter;
            Ch1PolarizedWsCounter = ch1PolarizedWsCounter;
            Ch2PolarizedWsCounter = ch2PolarizedWsCounter;
            Ch1Current = ch1Current;
            Ch2Current = ch2Current;
            SecondsCounter = secondsCounter;
            Aux1WsCounter = aux1WsCounter;
            Aux2WsCounter = aux2WsCounter;
            Aux3WsCounter = aux3WsCounter;
            Aux4WsCounter = aux4WsCounter;
            Aux5WsCounter = aux5WsCounter;
            DcVoltage = dcVoltage;
            ResetCounter = resetCounter;
            Ch1PolarityBit = ch1PolarityBit;
            Ch2PolarityBit = ch2PolarityBit;
        }
    }
}