﻿using System;
using System.Collections.ObjectModel;
using System.Linq;

using Robock.Models;
using Robock.Mvvm;

namespace Robock.ViewModels.Tabs
{
    internal class AboutTabViewModel : ViewModel
    {
        public ReadOnlyCollection<LicenseViewModel> Libraries => ProductInfo.LicenseNotices.Select(w => new LicenseViewModel(w)).ToList().AsReadOnly();

        public string Name => ProductInfo.Name.Value;

        public string Version => $"Version {ProductInfo.Version.Value}".Trim();

        public string Copyright => ProductInfo.Copyright.Value;

        public string OsVersion => $"OsVersion = {Environment.OSVersion}";
    }
}