using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Robock.Models
{
    public static class ProductInfo
    {
        public static readonly ReadOnlyCollection<License> LicenseNotices = new List<License>
        {
            new License
            {
                Name = "CommonServiceLocator",
                Url = "https://github.com/unitycontainer/commonservicelocator",
                Body = License.MSPL
            },
            new License
            {
                Name = "FontAwesome",
                Url = "https://github.com/FortAwesome/Font-Awesome",
                Authors = new[] {"Fort Awesome"},
                Body = @"
Font Awesome Free License
-------------------------

Font Awesome Free is free, open source, and GPL friendly. You can use it for
commercial projects, open source projects, or really almost whatever you want.
Full Font Awesome Free license: https://fontawesome.com/license.

# Icons: CC BY 4.0 License (https://creativecommons.org/licenses/by/4.0/)
In the Font Awesome Free download, the CC BY 4.0 license applies to all icons
packaged as SVG and JS file types.

# Fonts: SIL OFL 1.1 License (https://scripts.sil.org/OFL)
In the Font Awesome Free download, the SIL OLF license applies to all icons
packaged as web and desktop font files.

# Code: MIT License (https://opensource.org/licenses/MIT)
In the Font Awesome Free download, the MIT license applies to all non-font and
non-icon files.

# Attribution
Attribution is required by MIT, SIL OLF, and CC BY licenses. Downloaded Font
Awesome Free files already contain embedded comments with sufficient
attribution, so you shouldn't need to do anything additional when using these
files normally.

We've kept attribution comments terse, so we ask that you do not actively work
to remove them from files, especially code. They're a great way for folks to
learn about Font Awesome.

# Brand Icons
All brand icons are trademarks of their respective owners. The use of these
trademarks does not indicate endorsement of the trademark holder by Font
Awesome, nor vice versa. **Please do not use brand logos for any purpose except
to represent the company, product, or service to which they refer.**"
            },
            new License
            {
                Name = "FontAwesome.Sharp",
                Url = "https://github.com/awesome-inc/FontAwesome.Sharp",
                Authors = new[] {"Awesome Incremented"},
                Body = License.Apache
            },
            new License
            {
                Name = "MetroRadiance",
                Url = "https://github.com/Grabacr07/MetroRadiance",
                Authors = new[] {"Manato KAMEYA"},
                Body = License.MIT.Replace("{yyyy}", "2014").Replace("{name of copyright owner}", "Manato KAMEYA")
            },
            new License
            {
                Name = "MetroRadiance.Chrome",
                Url = "https://github.com/Grabacr07/MetroRadiance",
                Authors = new[] {"Manato KAMEYA"},
                Body = License.MIT.Replace("{yyyy}", "2014").Replace("{name of copyright owner}", "Manato KAMEYA")
            },
            new License
            {
                Name = "MetroRadiance.Core",
                Url = "https://github.com/Grabacr07/MetroRadiance",
                Authors = new[] {"Manato KAMEYA"},
                Body = License.MIT.Replace("{yyyy}", "2014").Replace("{name of copyright owner}", "Manato KAMEYA")
            },
            new License
            {
                Name = "Microsoft.Practices.Unity",
                Url = "https://github.com/unitycontainer/unity",
                Body = License.ApacheMs
            },
            new License
            {
                Name = "Microsoft.Practices.Unity.Configuration",
                Url = "https://github.com/unitycontainer/unity",
                Body = License.ApacheMs
            },
            new License
            {
                Name = "Microsoft.Practices.Unity.RegistrationByConvetion",
                Url = "https://github.com/unitycontainer/unity",
                Body = License.ApacheMs
            },
            new License
            {
                Name = "Prism",
                Url = "https://github.com/PrismLibrary/Prism",
                Authors = new[] {".NET Foundation"},
                Body = License.MIT.Replace("{yyyy}", "").Replace("{name of copyright owner}", ".NET Foundation")
            },
            new License
            {
                Name = "Prism.Unity.Wpf",
                Url = "https://github.com/PrismLibrary/Prism",
                Authors = new[] {".NET Foundation"},
                Body = License.MIT.Replace("{yyyy}", "").Replace("{name of copyright owner}", ".NET Foundation")
            },
            new License
            {
                Name = "Prism.Wpf",
                Url = "https://github.com/PrismLibrary/Prism",
                Authors = new[] {".NET Foundation"},
                Body = License.MIT.Replace("{yyyy}", "").Replace("{name of copyright owner}", ".NET Foundation")
            },
            new License
            {
                Name = "ReactiveProperty",
                Url = "https://github.com/runceel/ReactiveProperty",
                Authors = new[] {"neuecc", "xin9le", "okazuki"},
                Body = License.MIT.Replace("{yyyy}", "2018").Replace("{name of copyright owner}", "neuecc, xin9le, okazuki")
            },
            new License
            {
                Name = "System.Reactive",
                Url = "https://github.com/dotnet/reactive",
                Authors = new[] {".NET Foundation and Contributors"},
                Body = @"
Copyright (c) .NET Foundation and Contributors
All Rights Reserved

Licensed under the Apache License, Version 2.0 (the ""License""); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied. See the License for the specific language governing permissions
and limitations under the License."
            },
            new License
            {
                Name = "SharpDx",
                Url = "http://sharpdx.org/",
                Authors = new[] {"Alexandre Mutel"},
                Body = License.MIT.Replace("{yyyy}", "2010-2015").Replace("{name of copyright owner}", "SharpDX - Alexandre Mutel")
            },
            new License
            {
                Name = "SharpDx.D3DCompiler",
                Url = "http://sharpdx.org/",
                Authors = new[] {"Alexandre Mutel"},
                Body = License.MIT.Replace("{yyyy}", "2010-2015").Replace("{name of copyright owner}", "SharpDX - Alexandre Mutel")
            },
            new License
            {
                Name = "SharpDx.Desktop",
                Url = "http://sharpdx.org/",
                Authors = new[] {"Alexandre Mutel"},
                Body = License.MIT.Replace("{yyyy}", "2010-2015").Replace("{name of copyright owner}", "SharpDX - Alexandre Mutel")
            },
            new License
            {
                Name = "SharpDx.Direct3D11",
                Url = "http://sharpdx.org/",
                Authors = new[] {"Alexandre Mutel"},
                Body = License.MIT.Replace("{yyyy}", "2010-2015").Replace("{name of copyright owner}", "SharpDX - Alexandre Mutel")
            },
            new License
            {
                Name = "SharpDx.DXGI",
                Url = "http://sharpdx.org/",
                Authors = new[] {"Alexandre Mutel"},
                Body = License.MIT.Replace("{yyyy}", "2010-2015").Replace("{name of copyright owner}", "SharpDX - Alexandre Mutel")
            },
            new License
            {
                Name = "SharpDx.Mathematics",
                Url = "http://sharpdx.org/",
                Authors = new[] {"Alexandre Mutel"},
                Body = License.MIT.Replace("{yyyy}", "2010-2015").Replace("{name of copyright owner}", "SharpDX - Alexandre Mutel")
            },
            new License
            {
                Name = "Microsoft.Wpf.Interop.DirectX",
                Url = "https://github.com/Microsoft/WPFDXInterop",
                Authors = new[] {"Microsoft"},
                Body = License.MIT.Replace("{yyyy}", "2015").Replace("{name of copyright owner}", "Microsoft")
            }
        }.OrderBy(w => w.Name).ToList().AsReadOnly();

        public static Lazy<string> Name => new Lazy<string>(() => GetAssemblyInfo<AssemblyTitleAttribute>().Title);

        public static Lazy<string> Description => new Lazy<string>(() => GetAssemblyInfo<AssemblyDescriptionAttribute>().Description);

        public static Lazy<string> Copyright => new Lazy<string>(() => GetAssemblyInfo<AssemblyCopyrightAttribute>().Copyright);

        public static Lazy<string> Version => new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version.ToString());

        private static T GetAssemblyInfo<T>() where T : Attribute
        {
            return (T) Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
        }
    }
}