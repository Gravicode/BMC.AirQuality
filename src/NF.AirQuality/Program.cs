using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace NF.AirQuality
{
    internal class Program
    {
        private const byte I2C_ADDRESS = 0x19; // I2C Device address, which can be changed by changing A1 and A0, the default address is 0x54
        private static AirQualitySensor airqualitysensor = new AirQualitySensor(I2C_ADDRESS);

       
        static void Setup()
        {
            Thread.Sleep(1000);
            // Get firmware version
            ushort version = airqualitysensor.GainVersion();
            Debug.WriteLine("version is : " + version);
            Thread.Sleep(1000);
        }

        static void Loop()
        {
            /*
             * @brief Get PM concentration in the air: parameters available
             * @n     PARTICLE_PM1_0_STANDARD
             * @n     PARTICLE_PM2_5_STANDARD
             * @n     PARTICLE_PM10_STANDARD
             * @n     PARTICLE_PM1_0_ATMOSPHERE
             * @n     PARTICLE_PM2_5_ATMOSPHERE
             * @n     PARTICLE_PM10_ATMOSPHERE
             * @n     PARTICLENUM_0_3_UM_EVERY0_1L_AIR
             * @n     PARTICLENUM_0_5_UM_EVERY0_1L_AIR
             * @n     PARTICLENUM_1_0_UM_EVERY0_1L_AIR
             * @n     PARTICLENUM_2_5_UM_EVERY0_1L_AIR
             * @n     PARTICLENUM_5_0_UM_EVERY0_1L_AIR
             * @n     PARTICLENUM_10_UM_EVERY0_1L_AIR
             * @n     PARTICLENUM_GAIN_VERSION
             */
            var num = airqualitysensor.GainParticlenumEvery0_1L(AirQualitySensor.PARTICLENUM_0_3_UM_EVERY0_1L_AIR);
            Debug.WriteLine("The number of particles with a diameter of 0.3um per 0.1 in lift-off is: " + num);

            var concentration1 = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM1_0_STANDARD);
            var concentration25 = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM2_5_STANDARD);
            var concentration10 = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM10_STANDARD);
            Debug.WriteLine("PM1.0 concentration: " + concentration1.ToString("F2") + " mg/m³");
            Debug.WriteLine("PM2.5 concentration: " + concentration25.ToString("F2") + " mg/m³");
            Debug.WriteLine("PM10 concentration: " + concentration10.ToString("F2") + " mg/m³");
          
            Thread.Sleep(1000);

            /*
            airqualitysensor.SetLowPower();
            Thread.Sleep(5000);
            airqualitysensor.Awake();
            Thread.Sleep(5000);
            */
        }
        static void Main()
        {
            Setup();
            while (true)
            {
                Loop();
            }
        }
    }
}
