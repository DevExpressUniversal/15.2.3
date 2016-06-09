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

#if SILVERLIGHT
extern alias Platform;
#endif
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System;
using System.Windows.Media;
using DevExpress.Design.UI;
#if SILVERLIGHT
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Core.Design.CoreUtils {
	public class ContextMenuButton : Button {
		protected override void OnClick() {
			base.OnClick();
			if(ContextMenu != null) {
				ContextMenu.PlacementTarget = this;
				ContextMenu.IsOpen = true;
			}
		}
	}
	public partial class ContextMenuToggleButton : ToggleStateButton {
		public bool AllowShowInnerContextMenu {
			get { return (bool)GetValue(AllowShowInnerContextMenuProperty); }
			set { SetValue(AllowShowInnerContextMenuProperty, value); }
		}
		public static readonly DependencyProperty AllowShowInnerContextMenuProperty =
			DependencyProperty.Register("AllowShowInnerContextMenu", typeof(bool), typeof(ContextMenuToggleButton), new PropertyMetadata(false));
		protected override void OnClick() {
			base.OnClick();
			if(AllowShowInnerContextMenu) {
				ContextMenuButton button = WpfLayoutHelper.FindElement(this, element => element is ContextMenuButton) as ContextMenuButton;
				if(button != null && button.ContextMenu != null)
					button.ContextMenu.IsOpen = true;
			}
		}
	}
	public class ImageSourceToImageConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new Image() { Source = value as ImageSource, Stretch = Stretch.None };
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
