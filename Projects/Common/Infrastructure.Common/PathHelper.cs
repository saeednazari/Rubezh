﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Infrastructure.Common
{
    public class PathHelper
    {
        public static string Plans = Data + "Plans.xml";
        public static string Report = Data + "ReportTemplates/TypesCountReport.xaml";

        public static string MetadataFileName = Data + "metadata.xml";
        public static string DeviceLibraryFileName = Data + "DeviceLibrary.xml";
        public static string TransormFileName = Data + "svg2xaml.xsl";

        public static string Data
        {
            get
            {
#if DEBUG
                string path = Assembly.GetExecutingAssembly().Location;
                path = path.Remove(path.LastIndexOf("\\"));
                path = path.Remove(path.LastIndexOf("\\"));
                path = path.Remove(path.LastIndexOf("\\"));
                path = path.Remove(path.LastIndexOf("\\"));
                path = path.Remove(path.LastIndexOf("\\"));
                path = path + "\\Data\\";
                return (path);
#else
                string path = Assembly.GetExecutingAssembly().Location;
                path = path.Remove(path.LastIndexOf("\\"));
                path = path.Remove(path.LastIndexOf("\\"));
                path = path + "\\Data\\";
                return(path);
#endif
            }
        }
    }
}