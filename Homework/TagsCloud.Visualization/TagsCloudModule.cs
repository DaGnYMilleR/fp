﻿using System;
using Autofac;
using TagsCloud.Visualization.ContainerVisitor;
using TagsCloud.Visualization.Drawers;
using TagsCloud.Visualization.FontFactory;
using TagsCloud.Visualization.ImagesSavers;
using TagsCloud.Visualization.LayoutContainer.ContainerBuilder;
using TagsCloud.Visualization.LayouterCores;
using TagsCloud.Visualization.PointGenerator;
using TagsCloud.Visualization.WordsParser;
using TagsCloud.Visualization.WordsSizeServices;

namespace TagsCloud.Visualization
{
    public class TagsCloudModule : Module
    {
        private readonly TagsCloudModuleSettings settings;

        public TagsCloudModule(TagsCloudModuleSettings settings)
            => this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WordsService>().As<IWordsService>();

            builder.RegisterType<WordsParser.WordsParser>().As<IWordsParser>();

            builder.Register(_ => new FontFactory.FontFactory(settings.FontSettings)).As<IFontFactory>();
            builder.RegisterType<WordsSizeService>().As<IWordsSizeService>();
            builder.RegisterType<WordsContainerBuilder>().As<AbstractWordsContainerBuilder>();

            builder.Register(_ => new ArchimedesSpiralPointGenerator(settings.Center)).As<IPointGenerator>();
            builder.RegisterType(settings.LayouterType).As<ICloudLayouter>();

            builder.Register(_ => settings.LayoutVisitor).As<IContainerVisitor>();
            builder.RegisterType<Drawer>().As<IDrawer>();
            builder.Register(_ => new ImageSaver(settings.SaveSettings)).As<IImageSaver>();

            builder.RegisterType<LayouterCore>().As<ILayouterCore>();
        }
    }
}