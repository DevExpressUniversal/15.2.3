#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Utils.Themes;
namespace DevExpress.Xpf.Core.Native {
	public class ThemeResourceExtension : MarkupExtension {
		public ThemeResourceExtension() { }
		public ThemeResourceExtension(ThemeKeyExtensionGeneric themeKey) {
			if (themeKey != null)
				ThemeKey = themeKey;
		}
		[ThreadStatic]
		static ThemeResourceConverter resourceConverter = new ThemeResourceConverter();
		static protected ThemeResourceConverter ResourceConverter { get { return resourceConverter ?? (resourceConverter = new ThemeResourceConverter()); } }
		MultiBinding resourceBinding;
		protected MultiBinding ResourceBinding { get { return resourceBinding ?? (resourceBinding = CreateBinding()); } }
		public ThemeKeyExtensionGeneric ThemeKey { get; set; }
		protected virtual MultiBinding CreateBinding() {
			var binding = new MultiBinding();
			binding.Bindings.Add(new Binding { Path = new PropertyPath(ThemeManager.TreeWalkerProperty), RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
			binding.Bindings.Add(new Binding("ThemeKey") { Source = this });
			binding.Bindings.Add(new Binding { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
			binding.Converter = ResourceConverter;
			return binding;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if (serviceProvider is IProvideValueTarget && ((IProvideValueTarget)serviceProvider).TargetObject is Setter)
				return ResourceBinding;
			return ResourceBinding.ProvideValue(serviceProvider);
		}
	}
	public class ThemeResourceConverter : IMultiValueConverter {
		[ThreadStatic]
		static ReflectionHelper reflectionHelper = new ReflectionHelper();
		static ReflectionHelper ReflectionHelper { get { return reflectionHelper ?? (reflectionHelper = new ReflectionHelper()); } }
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (values.Any(val => val == DependencyProperty.UnsetValue))
				return null;
			var treeWalker = values[0] as ThemeTreeWalker;
			var themeName = treeWalker != null ? treeWalker.ThemeName : null;
			var themeKey = values[1] as ThemeKeyExtensionGeneric;
			ThemeKeyExtensionGeneric newThemeKey = Activator.CreateInstance(themeKey.GetType()) as ThemeKeyExtensionGeneric;
			newThemeKey.ThemeName = themeName == Theme.DefaultThemeName ? null : themeName;
			ReflectionHelper.SetPropertyValue(newThemeKey, "ResourceKey", ReflectionHelper.GetPropertyValue(themeKey, "ResourceKey"));
			var element = values[2] as FrameworkElement;
			return element != null ? element.FindResource(newThemeKey) : null;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
