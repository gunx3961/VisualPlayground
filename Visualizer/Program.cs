﻿using System;

namespace Visualizer
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new VisualizerGame())
                game.Run();
        }
    }
}
