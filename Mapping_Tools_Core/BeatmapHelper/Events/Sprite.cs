﻿using System;
using Mapping_Tools_Core.BeatmapHelper.IO;
using Mapping_Tools_Core.BeatmapHelper.Types;
using Mapping_Tools_Core.Exceptions;
using Mapping_Tools_Core.MathUtil;

namespace Mapping_Tools_Core.BeatmapHelper.Events {
    public class Sprite : Event, IHasStoryboardLayer {
        public StoryboardLayer Layer { get; set; }
        public Origin Origin { get; set; }

        /// <summary>
        /// This is a partial path to the image file for this sprite.
        /// </summary>
        public string FilePath { get; set; }

        public Vector2 Pos { get; set; }

        /// <summary>
        /// Serializes this object to .osu code.
        /// </summary>
        /// <returns></returns>
        public override string GetLine() {
            return $"Sprite,{Layer},{Origin},\"{FilePath}\",{Pos.X.ToRoundInvariant()},{Pos.Y.ToRoundInvariant()}";
        }

        /// <summary>
        /// Deserializes a string of .osu code and populates the properties of this object.
        /// </summary>
        /// <param name="line"></param>
        public override void SetLine(string line) {
            string[] values = line.Split(',');

            if (values[0] != "Sprite" && values[0] != "4") {
                throw new BeatmapParsingException("This line is not a sprite.", line);
            }

            if (Enum.TryParse(values[1], out StoryboardLayer layer))
                Layer = layer;
            else throw new BeatmapParsingException("Failed to parse layer of sprite.", line);

            if (Enum.TryParse(values[2], out Origin origin))
                Origin = origin;
            else throw new BeatmapParsingException("Failed to parse origin of sprite.", line);

            FilePath = values[3].Trim('"');

            if (!FileFormatHelper.TryParseDouble(values[4], out double x))
                throw new BeatmapParsingException("Failed to parse X position of sprite.", line);

            if (!FileFormatHelper.TryParseDouble(values[5], out double y))
                throw new BeatmapParsingException("Failed to parse Y position of sprite.", line);

            Pos = new Vector2(x, y);
        }
    }
}