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

using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.NavBar {
	public partial class ImageAndTextContentPresenter : XPFContentPresenter {		
		void SetImageSettingsBinding() {
		}
		void SetLayoutSettingsBinding() {
		}
	}
	public partial class ImageAndTextDecorator : Control {
		void SetBindings() {
			if (this.GetTemplatedParent() == null)
				return;
			if (Image != null)
				BindingOperations.SetBinding(Image, Image.VisibilityProperty, 
					new Binding("Source") { Source = Image, Converter = new ImageSourceToVisibilityConverter(), ConverterParameter = DisplayMode });
				var mb = new MultiBinding();
				mb.Bindings.Add(new Binding("(0)") { Path = new PropertyPath(NavBarViewBase.DisplayModeProperty), RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
				if (this is NavPaneImageAndTextDecorator) {
					mb.Bindings.Add(new Binding() { Path = new PropertyPath(NavigationPaneExpandInfoProvider.IsCompleteCollapsedProperty) });
				}
				mb.Converter = new CollapsedNavPaneDisplayModeConverter();
				mb.ConverterParameter = this;
				SetBinding(ImageAndTextDecorator.DisplayModeProperty, mb);
			SetBinding(NavBarViewBase.ImageSettingsProperty, new Binding("(0)") { Path = new PropertyPath(NavBarViewBase.ImageSettingsProperty), RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
			SetBinding(NavBarViewBase.LayoutSettingsProperty, new Binding("(0)") { Path = new PropertyPath(NavBarViewBase.LayoutSettingsProperty), RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
		}
	}
	public class CollapsedNavPaneDisplayModeConverter : IMultiValueConverter {
		public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (parameter is NavPaneImageAndTextDecorator) {
				if (values[1] is bool && (bool)values[1])
					return DisplayMode.ImageAndText;
			}
			return values[0];
		}
		public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new System.NotImplementedException();
		}
	}
}
