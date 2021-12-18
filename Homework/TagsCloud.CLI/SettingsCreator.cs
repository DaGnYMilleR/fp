﻿using System;
using System.Drawing;
using System.IO;
using TagsCloud.Visualization;
using TagsCloud.Visualization.ContainerVisitor;
using TagsCloud.Visualization.Drawers;
using TagsCloud.Visualization.FontFactory;
using TagsCloud.Visualization.ImagesSavior;

namespace TagsCloud.Words
{
    public class SettingsCreator
    {
        public TagsCloudModuleSettings Create(Options options) =>
            new()
            {
                Center = Point.Empty,
                InputWordsFile = options.WordsFile,
                BoringWordsFile = options.BoringWordsFile,
                FontSettings = new FontSettings
                {
                    FamilyName = options.FamilyName,
                    MaxSize = options.MaxSize,
                    FontStyle = Enum.TryParse<FontStyle>(options.FontStyle, true, out var style) 
                        ? style : FontStyle.Regular
                },

                SaveSettings = new SaveSettings
                {
                    OutputDirectory = options.OutputDirectory ?? GetDirectoryForSavingExamples(),
                    OutputFileName = options.OutputFile,
                    Extension = options.Extension
                },

                LayouterType = CreateLayouterTypeFromName(options.Algorithm),
                LayoutVisitor = CreateDrawerVisitorFromName(options.Color)
            };

        private IContainerVisitor CreateDrawerVisitorFromName(string textColor)
        {
            if (textColor == "random")
                return new RandomColorDrawerVisitor();

            var color = Color.FromName(textColor);
            return new ConcreteColorDrawerVisitor(new ImageSettings {Color = color});
        }

        private Type CreateLayouterTypeFromName(string name)
        {
            return name switch
            {
                "circular" => typeof(CircularCloudLayouter),
                _ => throw new ArgumentException($"Layouter {name} is not defined")
            };
        }

        private string GetDirectoryForSavingExamples()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Examples");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}