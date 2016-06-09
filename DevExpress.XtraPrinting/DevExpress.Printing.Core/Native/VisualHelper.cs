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
using System.Windows.Media;
namespace DevExpress.XtraPrinting.Native {
	public static class VisualHelper {
		public static readonly DependencyProperty OffsetProperty =
			DependencyProperty.RegisterAttached("Offset", typeof(Point), typeof(VisualHelper), new PropertyMetadata(new Point(), OnOffsetChanged));
		public static readonly DependencyProperty ClipToBoundsProperty =
			DependencyProperty.RegisterAttached("ClipToBounds", typeof(bool), typeof(VisualHelper), new PropertyMetadata(false, OnClipToBoundsChanged));
		public static readonly DependencyProperty IsVisualBrickBorderProperty =
			DependencyProperty.RegisterAttached("IsVisualBrickBorder", typeof(bool), typeof(VisualHelper), new PropertyMetadata(false));
		public static Point GetOffset(DependencyObject obj) {
			return (Point)obj.GetValue(OffsetProperty);
		}
		public static void SetOffset(DependencyObject obj, Point value) {
			obj.SetValue(OffsetProperty, value);
		}
		public static bool GetClipToBounds(DependencyObject obj) {
			return (bool)obj.GetValue(ClipToBoundsProperty);
		}
		public static void SetClipToBounds(DependencyObject obj, bool value) {
			obj.SetValue(ClipToBoundsProperty, value);
		}
		public static bool GetIsVisualBrickBorder(DependencyObject obj) {
			return (bool)obj.GetValue(IsVisualBrickBorderProperty);
		}
		public static void SetIsVisualBrickBorder(DependencyObject obj, bool value) {
			obj.SetValue(IsVisualBrickBorderProperty, value);
		}	 
		static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = d as FrameworkElement;
			if(element == null)
				throw new NotSupportedException();
			Point offset = (Point)e.NewValue;
			element.RenderTransform = new TranslateTransform() { X = offset.X, Y = offset.Y };
		}
		static void OnClipToBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FrameworkElement element = d as FrameworkElement;
			if(element == null)
				throw new NotSupportedException();
			if((bool)e.NewValue) {
				element.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), new Size(element.Width, element.Height)) };
			}
		}
	}
}
