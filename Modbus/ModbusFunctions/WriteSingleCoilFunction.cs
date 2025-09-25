using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            //TO DO: IMPLEMENT


            ModbusWriteCommandParameters writeParams = (ModbusWriteCommandParameters)CommandParameters;

            byte[] request = new byte[12];
            int dist = 0;

            // Transaction ID (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)writeParams.TransactionId)), 0, request, dist, 2);
            dist += 2;

            // Protocol ID (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)writeParams.ProtocolId)), 0, request, dist, 2);
            dist += 2;

            // Length (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)writeParams.Length)), 0, request, dist, 2);
            dist += 2;

            // Unit ID (1 bajt)
            request[dist++] = writeParams.UnitId;

            // Function Code (1 bajt)
            request[dist++] = writeParams.FunctionCode;

            // Output Address (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)writeParams.OutputAddress)), 0, request, dist, 2);
            dist += 2;

            // Value (2 bajta)
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)writeParams.Value)), 0, request, dist, 2);
            dist += 2;

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            //TO DO: IMPLEMENT

            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            ModbusWriteCommandParameters writeParams = (ModbusWriteCommandParameters)CommandParameters;

            // Adresa na koju je upisano (bajtovi 8 i 9)
            ushort address = (ushort)((response[8] << 8) | response[9]);

            // Vrijednost koja je upisana (bajtovi 10 i 11)
            ushort value = (ushort)((response[10] << 8) | response[11]);

            // Dodajemo u rezultat
            result.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, address), value);

            return result;
        }
    }
}