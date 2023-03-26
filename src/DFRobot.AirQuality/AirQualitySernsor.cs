using System.Device.I2c;

namespace DFRobot.AirQuality
{
    public class AirQualitySensor
    {
        //Select PM type
        public const byte PARTICLE_PM1_0_STANDARD = 0X05;
        public const byte PARTICLE_PM2_5_STANDARD = 0X07;
        public const byte PARTICLE_PM10_STANDARD = 0X09;
        public const byte PARTICLE_PM1_0_ATMOSPHERE = 0X0B;
        public const byte PARTICLE_PM2_5_ATMOSPHERE = 0X0D;
        public const byte PARTICLE_PM10_ATMOSPHERE = 0X0F;
        public const byte PARTICLENUM_0_3_UM_EVERY0_1L_AIR = 0X11;
        public const byte PARTICLENUM_0_5_UM_EVERY0_1L_AIR = 0X13;
        public const byte PARTICLENUM_1_0_UM_EVERY0_1L_AIR = 0X15;
        public const byte PARTICLENUM_2_5_UM_EVERY0_1L_AIR = 0X17;
        public const byte PARTICLENUM_5_0_UM_EVERY0_1L_AIR = 0X19;
        public const byte PARTICLENUM_10_UM_EVERY0_1L_AIR = 0X1B;
        public const byte PARTICLENUM_GAIN_VERSION = 0X1D;

        private I2cDevice i2cDevice;

        public AirQualitySensor(byte address, int bus = 1)
        {
            var i2cConnectionSettings = new I2cConnectionSettings(bus, address);
            i2cDevice = I2cDevice.Create(i2cConnectionSettings);
        }

        public int GainParticleConcentrationUgM3(byte PMtype)
        {
            byte[] buf = ReadReg(PMtype, 2);
            int concentration = (buf[0] << 8) + buf[1];
            return concentration;
        }

        public int GainParticlenumEvery0_1L(byte PMtype)
        {
            byte[] buf = ReadReg(PMtype, 2);
            int particlenum = (buf[0] << 8) + buf[1];
            return particlenum;
        }

        public byte GainVersion()
        {
            byte[] version = ReadReg(PARTICLENUM_GAIN_VERSION, 1);
            return version[0];
        }

        public void SetLowPower()
        {
            byte[] mode = { 0x01 };
            WriteReg(0x01, mode);
        }

        public void Awake()
        {
            byte[] mode = { 0x02 };
            WriteReg(0x01, mode);
        }

        private void WriteReg(byte reg, byte[] data)
        {
            while (true)
            {
                try
                {
                    //i2cDevice.Write(reg, data);
                    var regdata = new List<byte>();
                    regdata.Add(reg);
                    regdata.AddRange(data);
                    //i2cDevice.Write(reg, data);
                    i2cDevice.Write(regdata.ToArray());
                    return;
                }
                catch
                {
                    Console.WriteLine("Please check connection!");
                    Thread.Sleep(1000);
                }
            }
        }

        private byte[] ReadReg(byte reg, int length)
        {
            byte[] buffer = new byte[length];
            try
            {
                
                //i2cDevice.Read(reg, buffer);
                i2cDevice.WriteRead(new byte []{ reg }, buffer);

            }
            catch
            {
                buffer[0] = 0xFF;
                buffer[1] = 0xFF;
            }
            return buffer;
        }
    }
}