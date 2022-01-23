﻿namespace Mapping_Tools_Core.BeatmapHelper.Events {
    public enum EventType {
        F, // Fade
        M, // Move
        MX, // Move X
        MY, // Move Y
        S, // Scale
        V, // Vector scale
        R, // Rotate
        C, // Colour
        L, // Loop
        T, // EventType-triggered loop
        P, // Parameters
        N, // Nothing, I guess...
    }
}