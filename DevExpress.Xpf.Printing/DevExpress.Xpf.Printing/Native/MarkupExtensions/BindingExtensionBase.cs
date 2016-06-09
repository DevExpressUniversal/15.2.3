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
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
namespace DevExpress.Xpf.Printing {
	public abstract class BindingExtensionBase : MarkupExtension {
		public BindingMode Mode { get; set; }
		public string Path { get; set; }
		public string Converter { get; set; }
		public object ConverterParameter { get; set; }
		public string StringFormat { get; set; }
		public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
		public object FallbackValue { get; set; }
		public string FallbackValueResourceKey { get; set; }
		protected object defaultFallbackValue;
		protected abstract PropertyPath BindingPath { get; }
		protected abstract RelativeSource RelativeSource { get; }
		public BindingExtensionBase() {
#if SL
			Mode = BindingMode.OneWay;
			defaultFallbackValue = null;
#else
			Mode = BindingMode.Default;
			defaultFallbackValue = DependencyProperty.UnsetValue;
#endif
			FallbackValue = defaultFallbackValue;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			if(FallbackValue != defaultFallbackValue && !string.IsNullOrEmpty(FallbackValueResourceKey))
				throw new ArgumentException("It is incorrect to use FallbackValue and FallbackValueResourceKey together");
			ResourceDictionary resources = GetResources(serviceProvider);
			return CreateBinding(resources);
		}
		protected virtual Binding CreateBinding(ResourceDictionary resources) {
			return new Binding() {
				Mode = Mode,
				Path = BindingPath,
				RelativeSource = RelativeSource,
				Converter = GetConverter(resources),
				ConverterParameter = ConverterParameter,
				StringFormat = StringFormat,
				UpdateSourceTrigger = UpdateSourceTrigger,
				FallbackValue = GetFallbackValue(resources)
			};
		}
		static ResourceDictionary GetResources(IServiceProvider serviceProvider) {
			IRootObjectProvider target = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
			if(target.RootObject is ResourceDictionary) {
				return (ResourceDictionary)target.RootObject;
			} else if(target.RootObject is FrameworkElement) {
				return ((FrameworkElement)target.RootObject).Resources;
			} else
				throw new NotSupportedException();
		}
		object GetFallbackValue(ResourceDictionary resources) {
			if(string.IsNullOrEmpty(FallbackValueResourceKey))
				return FallbackValue;
			return resources[FallbackValueResourceKey];
		}
		IValueConverter GetConverter(ResourceDictionary resources) {
			return Converter != null ? (IValueConverter)resources[Converter] : null;
		}
	}
}
