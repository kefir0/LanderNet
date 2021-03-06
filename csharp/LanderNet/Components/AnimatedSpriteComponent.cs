﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LanderNet.UI.Util;
using TpsGraphNet;

namespace LanderNet.UI.Components
{
    public class AnimatedSpriteComponent
    {
        #region private static methods

        private static IEnumerable<string> GetFileNames(string prefix, int frameCount, int frameIncrement,
            int initialFrame, bool? reverseAnimation)
        {
            if (frameCount <= 0) throw new ArgumentException("Frame count should be greater than zero");
            var fileNames =
                Enumerable.Range(0, frameCount).Select(x => GetFileName(x*frameIncrement + initialFrame, prefix));
            if (reverseAnimation ?? RandomHelper.YesOrNo()) fileNames = fileNames.Reverse();
            return fileNames;
        }

        private static string GetFileName(int spriteIndex, string prefix)
        {
            return $"{prefix}{spriteIndex:D4}.bmp";
        }

        #endregion

        #region private static fields

        private static readonly ConcurrentDictionary<Tuple<string, int, int>, Sprite> SpriteCache =
            new ConcurrentDictionary<Tuple<string, int, int>, Sprite>();

        #endregion

        #region public constructors

        public AnimatedSpriteComponent(string prefix, int frameCount, double fps = 10, int frameIncrement = 1,
            int initialFrame = 0, bool? reverseAnimation = null, int decodePixelWidth = 0, int decodePixelHeight = 0)
            : this(GetFileNames(prefix, frameCount, frameIncrement, initialFrame, reverseAnimation).ToArray(), fps,
                decodePixelWidth, decodePixelHeight)
        {
        }

        #endregion

        #region public methods

        public Sprite GetCurrentSprite()
        {
            return _spritesByRawIndex[CurrentSpriteIndexRaw];
        }

        private Sprite GetSprite(string fileName)
        {
            Sprite sprite;
            var cacheKey = new Tuple<string, int, int>(fileName, _decodePixelWidth, _decodePixelHeight);

            if (!SpriteCache.TryGetValue(cacheKey, out sprite))
            {
                sprite = BitmapUtils.GetResourceSprite(fileName, _decodePixelWidth, _decodePixelHeight);
                SpriteCache.TryAdd(cacheKey, sprite);
            }

            return sprite;
        }

        public AnimatedSpriteComponent Clone(double scale = 1)
        {
            return new AnimatedSpriteComponent(_fileNames, _fps, (int) (_decodePixelWidth*scale), (int) (_decodePixelHeight*scale));
        }

        #endregion

        #region public properties and indexers

        public int CurrentSpriteIndexRaw
        {
            get
            {
                // A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond.
                const double ticksPerSecond = 10000*1000;
                var seconds = DateTime.Now.Ticks/ticksPerSecond + _randomSecondsOffset;
                var result = (int) ((seconds*_fps)%_spritesByRawIndex.Length);
                return result;
            }
        }

        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public Func<bool> VisibilityFunc { get; set; }

        public bool IsVisible => VisibilityFunc == null || VisibilityFunc();

        public BlendMode? BlendMode { get; set; }

        #endregion

        #region private constructors

        private AnimatedSpriteComponent(IEnumerable<string> fileNames, double fps, int decodePixelWidth,
            int decodePixelHeight)
        {
            _decodePixelWidth = decodePixelWidth;
            _decodePixelHeight = decodePixelHeight;

            _fileNames = fileNames.ToArray();
            _spritesByRawIndex = _fileNames.Select(GetSprite).ToArray();
            _fps = fps;
            _randomSecondsOffset = RandomHelper.Instance.Next(1000);
        }

        #endregion

        #region private fields

        private readonly Sprite[] _spritesByRawIndex;
        private readonly double _fps;
        private readonly int _randomSecondsOffset;
        private readonly int _decodePixelWidth;
        private readonly int _decodePixelHeight;
        private readonly string[] _fileNames;

        #endregion
    }
}