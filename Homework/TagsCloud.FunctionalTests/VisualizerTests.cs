﻿using System.Collections.Generic;
using System.IO;
using Autofac;
using NUnit.Framework;
using TagsCloud.Visualization;
using TagsCloud.Visualization.ContainerVisitor;
using TagsCloud.Visualization.Extensions;
using TagsCloud.Visualization.LayouterCores;
using TagsCloud.Visualization.WordsReaders;
using TagsCloud.Visualization.WordsReaders.FileReaders;

namespace TagsCloud.FunctionalTests
{
    public class VisualizerTests
    {
        [TestCase("txt", TestName = "txt")]
        [TestCase("doc", TestName = "doc")]
        [TestCase("docx", TestName = "docx")]
        [TestCase("pdf", TestName = "pdf")]
        public void ShouldReadWords_From(string extension)
        {
            var settings = GenerateDefaultSettings();

            using var container = CreateContainer(settings, extension);

            var visualizer = container.Resolve<ITagsCloudVisualizer>();
            visualizer.GenerateImage(container.Resolve<IWordsProvider>())
                .Then(x => x.Dispose());
        }

        private TagsCloudModuleSettings GenerateDefaultSettings() =>
            new()
            {
                LayouterType = typeof(CircularCloudLayouter),
                LayoutVisitor = new RandomColorDrawerVisitor()
            };

        private IContainer CreateContainer(TagsCloudModuleSettings settings, string extension)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TagsCloudModule(settings));

            builder.RegisterType<TxtFileReader>().As<IFileReader>();
            builder.RegisterType<DocFileReader>().As<IFileReader>();
            builder.RegisterType<PdfFileReader>().As<IFileReader>();

            builder.Register(ctx =>
                    new FileProvider(Path.Combine(Directory.GetCurrentDirectory(), $"test.{extension}"),
                        ctx.Resolve<IEnumerable<IFileReader>>()))
                .As<IWordsProvider>();

            return builder.Build();
        }
    }
}