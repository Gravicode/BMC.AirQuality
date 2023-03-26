using DFRobot.AirQuality;

namespace TestDriver
{
    internal class Program
    {
        private const byte I2C_1 = 0x01; // Use i2c1 interface (or i2c0 with configuring Raspberry Pi file) to drive sensor
        private const byte I2C_ADDRESS = 0x19; // I2C Device address, which can be changed by changing A1 and A0, the default address is 0x54
        private static AirQualitySensor airqualitysensor = new  AirQualitySensor(I2C_ADDRESS);

        static void Main(string[] args)
        {
            Setup();
            while (true)
            {
                Loop();
            }
        }

        static void Setup()
        {
            Thread.Sleep(1000);
            // Get firmware version
            ushort version = airqualitysensor.GainVersion();
            Console.WriteLine("version is : " + version);
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
            Console.WriteLine("The number of particles with a diameter of 0.3um per 0.1 in lift-off is: " + num);

            var concentration = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM1_0_STANDARD);
            Console.WriteLine("PM1.0 concentration: " + concentration.ToString("F2") + " mg/m³");
            Thread.Sleep(1000);

            /*
            airqualitysensor.SetLowPower();
            Thread.Sleep(5000);
            airqualitysensor.Awake();
            Thread.Sleep(5000);
            */
        }
       
    }
}