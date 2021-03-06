﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Framework.Core;
using Framework.Core.Filters.Base;
using Framework.Core.Filters.Spatial;
using Framework.Core.Metrics;
using Framework.Range;

namespace Framework.Filters.Noise
{
    /// <summary>
    /// Apply uniform noise to an image.
    /// </summary>
    /// <remarks>
    /// Implemented from
    /// (Myler, H.R., Weeks, A. R. . Pocket Handbook of Image Processing Algorithms in C.Prentice Hall).
    /// </remarks>
    [Filter("Uniform Noise", "Uniform Noise Filter", 
            "Apply uniform noise to an image", new string[] { "Noise" }, true)]
    [SmoothMetric]
    public class UniformNoise : SpatialDomainFilter
    {

        /// <summary>
        /// Generate default configurations for the filter.
        /// </summary>
        /// <returns>Dictionary used to pass configuration though the 
        /// configs parameter of the method ApplyFilter. 
        /// null if there is the filter doesn't use configurations.</returns>
        public override SortedDictionary<string, object> GetDefaultConfigs()
        {
            SortedDictionary<string, object> ret = new SortedDictionary<string, object>();

            ret.Add("Var", new Rangeable(0.5, 0, 255, 0.5));
            ret.Add("Mean", new Rangeable(10, 0, 255, 0.5));

            return ret;
        }

        /// <summary>
        /// Apply filter to a byte[,].
        /// </summary>
        /// <param name="img"></param>
        /// <param name="configs"></param>
        /// <returns></returns>
        public override byte[,] ApplyFilter(byte[,] img, SortedDictionary<string, object> configs)
        {
            // Pocket Handbook of Image Processing Algorithms- Uniform Noise
            Random rnd = new Random(); byte[] rnd_buffer = new byte[4]; ushort rnd_value;

            int width = img.GetLength(0), height = img.GetLength(1);

            byte[,] ret = new byte[width, height];

            double var = double.Parse(configs["Var"].ToString()), mean = double.Parse(configs["Mean"].ToString()), noise;
            double sqrt_var = Math.Sqrt(var);
            const double MAGIC = 1.9175345E-4;
            const double MAGIC2 = 1.7320508;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    rnd.NextBytes(rnd_buffer);
                    // Force to ushort, in range [0; 32767]
                    rnd_value = (ushort)(BitConverter.ToUInt16(rnd_buffer, 0) % 32768);

                    noise = sqrt_var * MAGIC * rnd_value + mean - sqrt_var * MAGIC2;

                    noise += img[x, y]; // add noise to image 
                    noise = Math.Max(byte.MinValue, Math.Min(byte.MaxValue, noise)); // trim

                    ret[x, y] = (Byte)Math.Ceiling(noise); // (Byte)(noise + 0.5);
                }
            }

            return ret;
        }

    }
}
