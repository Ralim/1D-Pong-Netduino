using System;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.NetduinoPlus;
using Microsoft.SPOT.Hardware;
using System.Threading;
using Glovebox.MicroFramework;

namespace Coatsy.Netduino.NeoPixel
{

    public delegate void DoCycle();

    /// <summary>
    /// Frame primatives - generic across NeoPixel Rings, Stips and Grids
    /// </summary>
    public class NeoPixelFrameBase : NeoPixelAction
    {

        #region Pixel Colour Definitions

        /// <summary>
        /// NeoPixels run cool on this palette
        /// </summary>
        public Pixel[] coolPalette = new Pixel[] { 
            Pixel.CoolColours.WarmRed, 
            Pixel.CoolColours.WarmGreen, 
            Pixel.CoolColours.WarmBlue
        };
 


        protected Pixel[] colourList = new Pixel[]
        {
            Pixel.Colour.Black,
            Pixel.Colour.AliceBlue,
            Pixel.Colour.AntiqueWhite,
            Pixel.Colour.Aqua,
            Pixel.Colour.Aquamarine,
            Pixel.Colour.Azure,
            Pixel.Colour.Beige,
            Pixel.Colour.Bisque,
            Pixel.Colour.BlanchedAlmond,
            Pixel.Colour.Blue,
            Pixel.Colour.BlueViolet,
            Pixel.Colour.Brown,
            Pixel.Colour.BurlyWood,
            Pixel.Colour.CadetBlue,
            Pixel.Colour.Chartreuse,
            Pixel.Colour.Chocolate,
            Pixel.Colour.Coral,
            Pixel.Colour.CornflowerBlue,
            Pixel.Colour.Cornsilk,
            Pixel.Colour.Crimson,
            Pixel.Colour.Cyan,
            Pixel.Colour.DarkBlue,
            Pixel.Colour.DarkCyan,
            Pixel.Colour.DarkGoldenRod,
            Pixel.Colour.DarkGray,
            Pixel.Colour.DarkGreen,
            Pixel.Colour.DarkKhaki,
            Pixel.Colour.DarkMagenta,
            Pixel.Colour.DarkOliveGreen,
            Pixel.Colour.DarkOrange,
            Pixel.Colour.DarkOrchid,
            Pixel.Colour.DarkRed,
            Pixel.Colour.DarkSalmon,
            Pixel.Colour.DarkSeaGreen,
            Pixel.Colour.DarkSlateBlue,
            Pixel.Colour.DarkSlateGray,
            Pixel.Colour.DarkTurquoise,
            Pixel.Colour.DarkViolet,
            Pixel.Colour.DeepPink,
            Pixel.Colour.DeepSkyBlue,
            Pixel.Colour.DimGray,
            Pixel.Colour.DodgerBlue,
            Pixel.Colour.FireBrick,
            Pixel.Colour.FloralWhite,
            Pixel.Colour.ForestGreen,
            Pixel.Colour.Fuchsia,
            Pixel.Colour.Gainsboro,
            Pixel.Colour.GhostWhite,
            Pixel.Colour.Gold,
            Pixel.Colour.GoldenRod,
            Pixel.Colour.Gray,
            Pixel.Colour.Green,
            Pixel.Colour.GreenYellow,
            Pixel.Colour.HoneyDew,
            Pixel.Colour.HotPink,
            Pixel.Colour.IndianRed,
            Pixel.Colour.Indigo,
            Pixel.Colour.Ivory,
            Pixel.Colour.Khaki,
            Pixel.Colour.Lavender,
            Pixel.Colour.LavenderBlush,
            Pixel.Colour.LawnGreen,
            Pixel.Colour.LemonChiffon,
            Pixel.Colour.LightBlue,
            Pixel.Colour.LightCoral,
            Pixel.Colour.LightCyan,
            Pixel.Colour.LightGoldenRodYellow,
            Pixel.Colour.LightGray,
            Pixel.Colour.LightGreen,
            Pixel.Colour.LightPink,
            Pixel.Colour.LightSalmon,
            Pixel.Colour.LightSeaGreen,
            Pixel.Colour.LightSkyBlue,
            Pixel.Colour.LightSlateGray,
            Pixel.Colour.LightSteelBlue,
            Pixel.Colour.LightYellow,
            Pixel.Colour.Lime,
            Pixel.Colour.LimeGreen,
            Pixel.Colour.Linen,
            Pixel.Colour.Magenta,
            Pixel.Colour.Maroon,
            Pixel.Colour.MediumAquaMarine,
            Pixel.Colour.MediumBlue,
            Pixel.Colour.MediumOrchid,
            Pixel.Colour.MediumPurple,
            Pixel.Colour.MediumSeaGreen,
            Pixel.Colour.MediumSlateBlue,
            Pixel.Colour.MediumSpringGreen,
            Pixel.Colour.MediumTurquoise,
            Pixel.Colour.MediumVioletRed,
            Pixel.Colour.MidnightBlue,
            Pixel.Colour.MintCream,
            Pixel.Colour.MistyRose,
            Pixel.Colour.Moccasin,
            Pixel.Colour.NavajoWhite,
            Pixel.Colour.Navy,
            Pixel.Colour.OldLace,
            Pixel.Colour.Olive,
            Pixel.Colour.OliveDrab,
            Pixel.Colour.Orange,
            Pixel.Colour.OrangeRed,
            Pixel.Colour.Orchid,
            Pixel.Colour.PaleGoldenRod,
            Pixel.Colour.PaleGreen,
            Pixel.Colour.PaleTurquoise,
            Pixel.Colour.PaleVioletRed,
            Pixel.Colour.PapayaWhip,
            Pixel.Colour.PeachPuff,
            Pixel.Colour.Peru,
            Pixel.Colour.Pink,
            Pixel.Colour.Plum,
            Pixel.Colour.PowderBlue,
            Pixel.Colour.Purple,
            Pixel.Colour.Red,
            Pixel.Colour.RosyBrown,
            Pixel.Colour.RoyalBlue,
            Pixel.Colour.SaddleBrown,
            Pixel.Colour.Salmon,
            Pixel.Colour.SandyBrown,
            Pixel.Colour.SeaGreen,
            Pixel.Colour.SeaShell,
            Pixel.Colour.Sienna,
            Pixel.Colour.Silver,
            Pixel.Colour.SkyBlue,
            Pixel.Colour.SlateBlue,
            Pixel.Colour.SlateGray,
            Pixel.Colour.Snow, 
            Pixel.Colour.SpringGreen,
            Pixel.Colour.SteelBlue,
            Pixel.Colour.Tan,
            Pixel.Colour.Teal,
            Pixel.Colour.Thistle,
            Pixel.Colour.Tomato, 
            Pixel.Colour.Turquoise, 
            Pixel.Colour.Violet,
            Pixel.Colour.Wheat,
            Pixel.Colour.White,
            Pixel.Colour.WhiteSmoke,
            Pixel.Colour.Yellow, 
            Pixel.Colour.YellowGreen
        };
        #endregion


        public DoCycle[] cycles;

        private int pixelCount;

        public int Length {
            get { return pixelCount; }
            private set { pixelCount = value; }
        }


        private NeoPixelSPI neoPixel;

        public Pixel[] Frame { get; set; }

        public NeoPixelFrameBase(int _pixelCount, string name) : base(name) {
            pixelCount = _pixelCount;
            neoPixel = new NeoPixelSPI(Pins.GPIO_PIN_D10, SPI.SPI_module.SPI1);
            Frame = new Pixel[pixelCount];

            // init frame to all black - specifically not null
            FrameClear();
        }

        #region Primitive Frame Manipulation Methods
        /// <summary>
        /// Clear the entire frame to black (pixels off) & makes sure colours are not NULL (just in case)
        /// </summary>
        public void FrameClear() {
            FrameSet(Pixel.Colour.Black);
        }

        /// <summary>
        /// Fill entire frame with one colour
        /// </summary>
        /// <param name="pixel"></param>
        public void FrameSet(Pixel pixel) {
            for (int i = 0; i < Frame.Length; i++) {
                Frame[i] = pixel;
            }
        }

        /// <summary>
        /// set specific frame pixels a colour - useful for letters on grids, patterns etc
        /// </summary>
        /// <param name="colour">The colour to set the pixel</param>
        /// <param name="pixelPos"></param>
        public void FrameSet(Pixel colour, ushort[] pixelPos) {
            for (int i = 0; i < pixelPos.Length; i++) {
                if (pixelPos[i] >= Frame.Length) { continue; }
                Frame[pixelPos[i]] = colour;
            }
        }


        /// <summary>
        /// set specific frame pixel a set colour
        /// </summary>
        /// <param name="colour">The colour to set the pixel</param>
        /// <param name="pixelPos">The pixels position in the frame</param>
        public void FrameSet(Pixel colour, ushort pixelPos)
        {
            if (pixelPos < Frame.Length)
            {
                Frame[pixelPos] = colour;//Write out the colour
            }  
        }

        /// <summary>
        /// set specific frame pixels from a rolling palette of colours
        /// </summary>
        /// <param name="pixelPos"></param>
        /// <param name="palette"></param>
        public void FrameSet(Pixel[] palette, ushort[] pixelPos) {
            for (int i = 0; i < pixelPos.Length; i++) {
                if (pixelPos[i] >= Frame.Length) { continue; }
                Frame[pixelPos[i]] = palette[i % palette.Length];
            }
        }


        /// <summary>
        /// fill frame pixels from a specified position and repeat 
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="startPos"></param>
        /// <param name="repeat"></param>
        public void FrameSet(Pixel pixel, ushort startPos, ushort repeat = 1) {
            for (int i = startPos, r = 0; r < repeat; i++, r++) {
                Frame[i % Frame.Length] = pixel;
            }
        }

        /// <summary>
        /// fill frame pixels from a specified position and repeat from a palette of colours
        /// </summary>
        /// <param name="pixel"></param>
        /// <param name="startPos"></param>
        /// <param name="repeat"></param>
        public void FrameSet(Pixel[] pixel, ushort startPos, ushort repeat = 1) {
            for (int i = startPos, r = 0; r < repeat; i++, r++) {
                Frame[i % Frame.Length] = pixel[i % pixel.Length];
            }
        }

        /// <summary>
        /// fill frame from a rolling pallet
        /// </summary>
        /// <param name="palette"></param>
        public void FrameSet(Pixel[] palette) {
            for (int i = 0; i < Frame.Length; i++) {
                Frame[i] = palette[i % palette.Length];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="palette"></param>
        public void FrameSetBlocks(Pixel[] palette)
        {
            if (palette == null || palette.Length == 0)
            {
                FrameClear();
            }
            else if (palette.Length >= pixelCount)
            {
                FrameSet(palette);
            }
            else
            {
                var leftovers = pixelCount % palette.Length;
                int leftoversUsed = 0;
                int thisPixel = 0;
                int baseBlockSize = pixelCount / palette.Length;
                for (int i = 0; i < palette.Length; i++)
                {
                    for (int j = 0; j < baseBlockSize; j++)
                    {
                        Frame[thisPixel] = palette[i];
                        thisPixel++;
                    }
                    if (leftoversUsed < leftovers)
                    {
                        Frame[thisPixel] = palette[i];
                        thisPixel++;
                        leftoversUsed++;
                    }
                }
            }
        }


        /// <summary>
        /// Swap specified pixels with wrap
        /// </summary>
        /// <param name="pixel1"></param>
        /// <param name="pixel2"></param>
        public void FramePixelSwap(ushort pixel1, ushort pixel2) {
            Pixel temp = Frame[pixel2 % pixelCount];
            Frame[pixel2 % pixelCount] = Frame[pixel1 % pixelCount];
            Frame[pixel1 % pixelCount] = temp;
        }

        public void FramePixelForward(ushort pixelIndex, ushort stepSize = 1) {
            if (pixelIndex >= Frame.Length) { return; }

            int length = Frame.Length;
            int newIndex = (pixelIndex + stepSize) % length;

            Pixel p = Frame[newIndex];
            Frame[newIndex] = Frame[pixelIndex];
            Frame[pixelIndex] = p;
        }


        /// <summary>
        /// Shift wrap forward a block of pixels by specified amount
        /// </summary>
        /// <param name="blockSize"></param>
        public void FrameShiftForward(ushort blockSize = 1) {
            blockSize = (ushort)(blockSize % Length);

            int i;
            Pixel[] temp = new Pixel[blockSize];

            for (i = 0; i < blockSize; i++) {
                temp[i] = Frame[Frame.Length - blockSize + i];
            }

            for (i = Frame.Length - 1; i >= blockSize; i--) {
                Frame[i] = Frame[i - blockSize];
            }

            for (i = 0; i < blockSize; i++) {
                Frame[i] = temp[i];
            }
        }

        /// <summary>
        /// Shift wrap forward a block of pixels by specified amount
        /// </summary>
        /// <param name="blockSize"></param>
        public void FrameShiftBack(ushort blockSize = 1) {
            blockSize = (ushort)(blockSize % Length);

            int i;
            Pixel[] temp = new Pixel[blockSize];

            for (i = 0; i < blockSize; i++) {
                temp[i] = Frame[i];
            }

            for (i = blockSize; i < Frame.Length; i++) {
                Frame[i - blockSize] = Frame[i];
            }

            for (i = 0; i < blockSize; i++) {
                int p = Frame.Length - blockSize + i;
                Frame[p] = temp[i];
            }
        }


        /// <summary>
        /// cycle the pixels moving them up by increment pixels
        /// </summary>
        /// <param name="increment">number of positions to shift. Negative numbers backwards. If this is more than the number of LEDs, the result wraps</param>
        public void FrameShift(int increment = 1)
        {
            //this creates less garbage:)
            if (increment > 0) { FrameShiftForward((ushort)increment); }
            else if (increment < 0) { FrameShiftBack((ushort)Utilities.Absolute(increment)); }
        }

        /// <summary>
        /// Forces an update with the current contents of currentDisplay
        /// </summary>
        public void FrameDraw() {
            neoPixel.ShowPixels(Frame);
        }

        #endregion

        #region Higher Level Display Methods

        /// <summary>
        /// move a singel pixel around (or along) the ring (or strip) - always starts at position 0
        /// </summary>
        /// <param name="pixelColour">Colour of the pixel to show</param>
        /// <param name="cycles">Number of whole cycles to rotate</param>
        /// <param name="stepDelay">Delay between steps (ms)</param>
        public void SpinColour(Pixel pixelColour, int cycles = 1, int stepDelay = 250)
        {
            SpinColourOnBackground(pixelColour, Pixel.Colour.Black, cycles, stepDelay);

        }

        public void SpinColourOnBackground(Pixel pixelColour, Pixel backgroundColour, int cycles = 1, int stepDelay = 250)
        {

            FrameSet(backgroundColour);
            FrameSet(pixelColour, new ushort[] { (ushort)0 });

            FrameDraw();

            for (int i = 0; i < cycles; i++)
            {
                for (int j = 0; j < pixelCount; j++)
                {
                    FrameShift();
                    FrameDraw();
                    Thread.Sleep(stepDelay);
                }
            }

        }
        #endregion

        public void RunCommand(Command command)
        {
            switch (command.CommandType)
            {
                case CommandType.Parent:
                    foreach (Command child in command.Commands)
                    {
                        RunCommand(child);
                    }
                    break;
                case CommandType.Repeat:
                    for (int i = 0; i < command.Repetitions; i++)
                    {
                        foreach (Command child in command.Commands)
                        {
                            RunCommand(child);
                        }
                        Thread.Sleep(command.PauseBetween);
                    }
                    break;
                case CommandType.Light1Pixel:
                    FrameClear();
                    FrameSet(getPixel(command.PrimaryColour), new ushort[] { (ushort)command.StartingPosition });
                    FrameDraw();
                    break;
                case CommandType.LightMultiPixel:
                    FrameClear();
                    FrameSet(getPixel(command.PrimaryColour), UShortArrayFromIntArray(command.PixelPositions));
                    FrameDraw();
                    break;
                case CommandType.Set1Pixel:
                    FrameSet(getPixel(command.PrimaryColour), new ushort[] { (ushort)command.PixelPositions[0] });
                    FrameDraw();
                    break;
                case CommandType.SetMultiPixel:
                    for (int i = 0; i < command.PixelPositions.Length; i++)
                    {
                        FrameSet(getPixel(command.PrimaryColour), (ushort)command.PixelPositions[i]);
                    }
                    FrameDraw();
                    break;
                case CommandType.Rotate:
                    FrameShift(command.RotateIncrement == 0 ? 1 : command.RotateIncrement);
                    FrameDraw();
                    break;
                case CommandType.AllOff:
                    FrameClear();
                    FrameDraw();
                    break;
                case CommandType.AllOn:
                    FrameSet(getPixel(command.PrimaryColour));
                    FrameDraw();
                    break;
                case CommandType.Spin:
                    SpinColour(getPixel(command.PrimaryColour), command.Cycles, command.StepTime);
                    break;
                case CommandType.SpinOnBackground:
                    SpinColourOnBackground(getPixel(command.PrimaryColour), getPixel(command.SecondaryColour), command.Cycles, command.StepTime);
                    break;
                case CommandType.AlternateColours:
                    var colourList = GetColourListFromColourSet(command.ColourSet);
                    FrameSet(colourList);
                    FrameDraw();
                    break;
                case CommandType.ColourBlocks:
                    var thecolourList = GetColourListFromColourSet(command.ColourSet);
                    FrameSetBlocks(thecolourList);
                    FrameDraw();
                    break;
                case CommandType.Wait:
                    break;
                default:
                    break;
            }
            if (command.CommandType != CommandType.Parent)
                Thread.Sleep(command.PauseAfter);
        }

        private ushort[] UShortArrayFromIntArray(int[] input)
        {
            ushort[] output = new ushort[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = (ushort)input[i];
            }

            return output;
        }

        /// <summary>
        /// pass a PixelColour enum and get the corresponding Pixel of that colour
        /// assumes that the colourList and the PixelColour enum are in sync
        /// </summary>
        /// <param name="pixelColour">PixelColour of the pixel required</param>
        /// <returns></returns>
        protected Pixel getPixel(PixelColour pixelColour)
        {
            return colourList[(int)pixelColour];
        }

        protected Pixel[] GetColourListFromColourSet(PixelColour[] colourSet)
        {
            var colourList = new Pixel[colourSet.Length];
            for (int i = 0; i < colourSet.Length; i++)
            {
                colourList[i] = getPixel(colourSet[i]);
            }
            return colourList;
        }

    }
}
