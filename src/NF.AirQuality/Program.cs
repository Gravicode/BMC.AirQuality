using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.I2c;
using GHIElectronics.TinyCLR.Drivers.BasicGraphics;
using GHIElectronics.TinyCLR.Drivers.SolomonSystech;
using GHIElectronics.TinyCLR.Drivers.SolomonSystech.SSD1306;
using GHIElectronics.TinyCLR.Pins;

namespace NF.AirQuality
{
    internal class Program
    {
        private const byte I2C_ADDRESS = 0x19; // I2C Device address, which can be changed by changing A1 and A0, the default address is 0x54
        private static AirQualitySensor airqualitysensor = new AirQualitySensor(I2C_ADDRESS, GHIElectronics.TinyCLR.Pins.FEZFlea.I2cBus.I2c1);
        static SSD1306Controller display;
        static BasicGraphics graphic;
        static void Setup()
        {
            var settings = SSD1306Controller.GetConnectionSettings();
            var controller = I2cController.FromName(FEZFlea.I2cBus.I2c2);
            var device = controller.GetDevice(settings);
            display = new SSD1306Controller(device);
            graphic = new BasicGraphics(128,64,ColorFormat.OneBpp);
            
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
            graphic.Clear();
            graphic.DrawString("--BMC Air Quality--",1,0,0);
            graphic.DrawString($"{MeasureAirQuality(concentration25)}", 1, 0, 10);
            graphic.DrawString($"PM 1.0: {concentration1} mg/m3",1,0,20);
            graphic.DrawString($"PM 2.5: {concentration25} mg/m3",1,0,30);
            graphic.DrawString($"PM 10: {concentration10} mg/m3",1,0,40);
            graphic.DrawString($"Particle 0.3: {concentration10}",1,0,50);
            graphic.DrawString($"Particle 0.5: {concentration10}",1,0,60);
          
            display.DrawBufferNative(graphic.Buffer);
           
            Thread.Sleep(1000);

            /*
            airqualitysensor.SetLowPower();
            Thread.Sleep(5000);
            airqualitysensor.Awake();
            Thread.Sleep(5000);
            */
        }
    
        public static string MeasureAirQuality(double pmValue)
        {
            

            string airQualityLevel = "";

            if (pmValue <= 12.0)
            {
                airQualityLevel = "Good";
            }
            else if (pmValue <= 35.4)
            {
                airQualityLevel = "Moderate";
            }
            else if (pmValue <= 55.4)
            {
                airQualityLevel = "Unhealthy for Sensitive Groups";
            }
            else if (pmValue <= 150.4)
            {
                airQualityLevel = "Unhealthy";
            }
            else if (pmValue <= 250.4)
            {
                airQualityLevel = "Very Unhealthy";
            }
            else
            {
                airQualityLevel = "Hazardous";
            }

            return $"({pmValue.ToString("n1")}) " + airQualityLevel;
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
