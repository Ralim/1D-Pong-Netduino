using System;
using Microsoft.SPOT;
using Coatsy.Netduino.NeoPixel;
using System.Threading;

namespace MakerDen
{
    public class MakerBaseNeoPixelStrip : MakerBaseIoT
    {
        static BasicStrip strip;
        static Thread neoPixelThread;

        private static void CreateCyclesCollection()
        {

            strip.cycles = new DoCycle[] {

          
            new DoCycle(strip.Rainbow),
            new DoCycle(strip.FrameClear),
            };
        }

        protected static void StartNeoPixel()
        {
            strip = new BasicStrip(50, "neostripA");

            CreateCyclesCollection();

            neoPixelThread = new Thread(StartNeoPixelThread);
            neoPixelThread.Priority = ThreadPriority.AboveNormal;
            neoPixelThread.Start();
        }

        private static void StartNeoPixelThread()
        {
            while (true)
            {
                for (int i = 0; i < strip.cycles.Length; i++)
                {
                    strip.ExecuteCycle(strip.cycles[i]);
                }
            }
        }
    }
}
