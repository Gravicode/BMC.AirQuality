using Avalonia.Controls.Primitives;
using DFRobot.AirQuality;
using Iot.Device.Bh1745;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirQualityApp
{
    public class SensorAQ
    {
        static int Delay = 5000; //5 sec
        static int TimeFrame = (60/5)*5; // 5 minutes
        public static List<AirSensorData> TimeSeriesData { get; set; } = new();
        private const byte I2C_1 = 0x01; // Use i2c1 interface (or i2c0 with configuring Raspberry Pi file) to drive sensor
        private const byte I2C_ADDRESS = 0x19; // I2C Device address, which can be changed by changing A1 and A0, the default address is 0x54
        private static AirQualitySensor airqualitysensor;
        public static bool IsRunning { get; set; } = false;
        public static ushort Version { get; set; }
        static AirSensorData sensorData { set; get; } = new();
        CancellationTokenSource source;
        Thread th1;
        public SensorAQ()
        {
            if(airqualitysensor==null)
               airqualitysensor = new AirQualitySensor(I2C_ADDRESS);
            Setup();
        }

        public AirSensorData Current
        {
            get
            {
                return sensorData;
            }
        }
       public List<AirSensorData> GetData()
        {
            return TimeSeriesData;
        }
        public bool IsDataAvailable()
        {
            return TimeSeriesData.Count > 0;
        }
        public void Start()
        {
            if (!IsRunning)
            {
                source = new CancellationTokenSource();
                th1 = new Thread(()=>Loop(source.Token));
                th1.Start();
                IsRunning = true;
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                source.Cancel();
            }
        }
        static void Setup()
        {
            Thread.Sleep(1000);
            // Get firmware version
            Version = airqualitysensor.GainVersion();
            Console.WriteLine("version is : " + Version);
            
        }
       
        void Loop(CancellationToken token)
        {
            while (true)
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
                Console.WriteLine($"-- Measure date: {DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss")} --");
                sensorData.ParticleNum03 = airqualitysensor.GainParticlenumEvery0_1L(AirQualitySensor.PARTICLENUM_0_3_UM_EVERY0_1L_AIR);
                Console.WriteLine("The number of particles with a diameter of 0.3um per 0.1 in lift-off is: " + sensorData.ParticleNum03);
                sensorData.ParticleNum05 = airqualitysensor.GainParticlenumEvery0_1L(AirQualitySensor.PARTICLENUM_0_5_UM_EVERY0_1L_AIR);
                Console.WriteLine("The number of particles with a diameter of 0.5um per 0.1 in lift-off is: " + sensorData.ParticleNum05);

                sensorData.PM1 = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM1_0_STANDARD);
                sensorData.PM25 = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM2_5_STANDARD);
                sensorData.PM10 = airqualitysensor.GainParticleConcentrationUgM3(AirQualitySensor.PARTICLE_PM10_STANDARD);
                Console.WriteLine("PM1.0 concentration: " + sensorData.PM1.ToString("F2") + " mg/m³");
                Console.WriteLine("PM2.5 concentration: " + sensorData.PM25.ToString("F2") + " mg/m³");
                Console.WriteLine("PM10 concentration: " + sensorData.PM10.ToString("F2") + " mg/m³");

                sensorData.MeasureTime = DateTime.Now;
                TimeSeriesData.Add(sensorData);
                if (TimeSeriesData.Count > TimeFrame)
                {
                    TimeSeriesData.RemoveAt(0);
                }
                if (token.IsCancellationRequested) break;
                Thread.Sleep(Delay);
             }
            IsRunning = false;
        }
    }
    public class AirSensorData
    {
        public DateTime MeasureTime { get; set; }
        public int ParticleNum03 { get; set; }
        public int ParticleNum05 { get; set; }
        public int PM1 { get; set; }
        public int PM10 { get; set; }
        public int PM25 { get; set; }
    }
}
