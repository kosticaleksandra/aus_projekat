using Common;
using Microsoft.Win32;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT

            ModbusReadCommandParameters readParams = (ModbusReadCommandParameters)CommandParameters;

            byte[] request = new byte[12];
            int dist = 0;

            // Transaction ID (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)readParams.TransactionId)), 0, request, dist, 2);
            dist += 2;

            // Protocol ID (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)readParams.ProtocolId)), 0, request, dist, 2);
            dist += 2;

            // Length (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)readParams.Length)), 0, request, dist, 2);
            dist += 2;

            // Unit ID (1 bajt)
            request[dist++] = readParams.UnitId;

            // Function Code (1 bajt)
            request[dist++] = readParams.FunctionCode;

            // Start Address (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)readParams.StartAddress)), 0, request, dist, 2);
            dist += 2;

            // Quantity (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)readParams.Quantity)), 0, request, dist, 2);
            dist += 2;

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT

            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ModbusReadCommandParameters readParams = (ModbusReadCommandParameters)CommandParameters;

            int byteCount = response[8]; // broj bajtova podataka;

            // svaki registar ima 2 bajta
            for (int i = 0; i < readParams.Quantity; i++)
            {
                int byteIndex = 9 + i * 2;

                // citamo dva bajta i spajamo ih u ushort (big endian)
                ushort value = (ushort)((response[byteIndex] << 8) | response[byteIndex + 1]);

                result.Add(
                    new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)(readParams.StartAddress + i)), value);
            }

            return result;
        }
    }
}