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

using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Printing {
	public static class ImageExportSettings {
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyPropertyManager.RegisterAttached("ImageSource", typeof(FrameworkElement), typeof(ImageExportSettings), new PropertyMetadata(ExportSettingDefaultValue.ImageSource));
		public static readonly DependencyProperty ImageRenderModeProperty =
			DependencyPropertyManager.RegisterAttached("ImageRenderMode", typeof(ImageRenderMode), typeof(ImageExportSettings), new PropertyMetadata(ExportSettingDefaultValue.ImageRenderMode));
		public static readonly DependencyProperty ImageKeyProperty =
			DependencyPropertyManager.RegisterAttached("ImageKey", typeof(object), typeof(ImageExportSettings), new PropertyMetadata(ExportSettingDefaultValue.ImageKey));
		public static readonly DependencyProperty ForceCenterImageModeProperty =
			DependencyPropertyManager.RegisterAttached("ForceCenterImageMode", typeof(bool), typeof(ImageExportSettings), new PropertyMetadata(false));
		public static FrameworkElement GetImageSource(DependencyObject obj) {
			return (FrameworkElement)obj.GetValue(ImageSourceProperty);
		}
		public static void SetImageSource(DependencyObject obj, FrameworkElement value) {
			obj.SetValue(ImageSourceProperty, value);
		}
		public static ImageRenderMode GetImageRenderMode(DependencyObject obj) {
			return (ImageRenderMode)obj.GetValue(ImageRenderModeProperty);
		}
		public static void SetImageRenderMode(DependencyObject obj, ImageRenderMode value) {
			obj.SetValue(ImageRenderModeProperty, value);
		}
		public static object GetImageKey(DependencyObject obj) {
			return (object)obj.GetValue(ImageKeyProperty);
		}
		public static void SetImageKey(DependencyObject obj, object value) {
			obj.SetValue(ImageKeyProperty, value);
		}
		public static bool GetForceCenterImageMode(DependencyObject obj) {
			return (bool)obj.GetValue(ForceCenterImageModeProperty);
		}
		public static void SetForceCenterImageMode(DependencyObject obj, bool value) {
			obj.SetValue(ForceCenterImageModeProperty, value);
		}
	}
}
