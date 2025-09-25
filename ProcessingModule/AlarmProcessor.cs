using Common;
using System;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for alarm processing.
    /// </summary>
    public class AlarmProcessor
    {
        /// <summary>
        /// Processes the alarm for analog point.
        /// </summary>
        /// <param name="eguValue">The EGU value of the point.</param>
        /// <param name="configItem">The configuration item.</param>
        /// <returns>The alarm indication.</returns>
		public AlarmType GetAlarmForAnalogPoint(double eguValue, IConfigItem configItem)
        {
            if (eguValue < configItem.EGU_Min)
            {
                return AlarmType.REASONABILITY_FAILURE;
            }
            if (eguValue > configItem.EGU_Max)
            {
                return AlarmType.REASONABILITY_FAILURE;
            }
            // Ako je manja od minimuma ili low limita
            if (eguValue < configItem.LowLimit)
            {
                return AlarmType.LOW_ALARM;

                // Ako je veća od maksimuma ili high limita
            }
            else if (eguValue > configItem.HighLimit)
            {
                return AlarmType.HIGH_ALARM;
            }


            // Ako ništa od navedenog → nema alarma
            return AlarmType.NO_ALARM;
        }

        /// <summary>
        /// Processes the alarm for digital point.
        /// </summary>
        /// <param name="state">The digital point state</param>
        /// <param name="configItem">The configuration item.</param>
        /// <returns>The alarm indication.</returns>
		public AlarmType GetAlarmForDigitalPoint(ushort state, IConfigItem configItem)
        {
            //provjerimo da li je abnormaln i to dodamo ako ne onda no armal
            if (state == configItem.AbnormalValue)
            {
                return AlarmType.ABNORMAL_VALUE;
            }
            else
            {
                return AlarmType.NO_ALARM;
            }
        }
    }
}