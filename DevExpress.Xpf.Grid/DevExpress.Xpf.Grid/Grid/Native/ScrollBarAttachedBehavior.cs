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
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid.Native {
	public class ScrollModeToBoolConverter : MarkupExtension, IValueConverter {
		public Orientation Orientation { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(((ScrollMode)value) == ScrollMode.Pixel)
				return Orientation;
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ScrollBarAttachedBehavior {
		public static readonly DependencyProperty UpdateThumbOrientationProperty = DependencyProperty.RegisterAttached("UpdateThumbOrientation", typeof(Orientation?), typeof(ScrollBarAttachedBehavior), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnUpdateThumbOrientationChanged));
		public static readonly DependencyProperty ScrollBarAttachedBehaviorProperty = DependencyProperty.RegisterAttached("ScrollBarAttachedBehavior", typeof(ScrollBarAttachedBehavior), typeof(ScrollBarAttachedBehavior), new PropertyMetadata(null));
		public static Orientation? GetUpdateThumbOrientation(DependencyObject obj) {
			return (Orientation?)obj.GetValue(UpdateThumbOrientationProperty);
		}
		public static void SetUpdateThumbOrientation(DependencyObject obj, Orientation? value) {
			obj.SetValue(UpdateThumbOrientationProperty, value);
		}
		public static ScrollBarAttachedBehavior GetScrollBarAttachedBehavior(DependencyObject obj) {
			return (ScrollBarAttachedBehavior)obj.GetValue(ScrollBarAttachedBehaviorProperty);
		}
		public static void SetScrollBarAttachedBehavior(DependencyObject obj, ScrollBarAttachedBehavior value) {
			obj.SetValue(ScrollBarAttachedBehaviorProperty, value);
		}
		static void OnUpdateThumbOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is Thumb)) return;
			Orientation? orientation = GetUpdateThumbOrientation(d);
			if(orientation.HasValue)
				SetScrollBarAttachedBehavior(d, new ScrollBarAttachedBehavior((Thumb)d, orientation.Value));
			else {
				ScrollBarAttachedBehavior thumbBehaviour = GetScrollBarAttachedBehavior(d);
				thumbBehaviour.Release();
				SetScrollBarAttachedBehavior(d, null);
			}
		}
		Thumb thumb;
		SizeHelperBase sizeHelper;
		public ScrollBarAttachedBehavior(Thumb thumb, Orientation orientation) {
			this.thumb = thumb;
			sizeHelper = SizeHelperBase.GetDefineSizeHelper(orientation);
			this.thumb.LayoutUpdated += thumb_LayoutUpdated;
		}
		Rect oldRect;
		void thumb_LayoutUpdated(object sender, EventArgs e) {
			if(!thumb.IsDragging) 
				return;
			FrameworkElement parent = VisualTreeHelper.GetParent(thumb) as FrameworkElement;
			if(parent == null) 
				return;
			Rect rect = LayoutHelper.GetRelativeElementRect(thumb, parent);
			if(oldRect == rect || sizeHelper.GetDefinePoint(rect.TopLeft) == 0 || Math.Round(sizeHelper.GetDefinePoint(rect.BottomRight)) == sizeHelper.GetDefinePoint(new Point(parent.ActualWidth, parent.ActualHeight)))
				return;
			oldRect = rect;
			FieldInfo fieldInfo = typeof(Thumb).GetField("_originThumbPoint", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
			Point origPoint = (Point)fieldInfo.GetValue(thumb);
			Point point = Mouse.GetPosition(thumb);
			if(Math.Abs(sizeHelper.GetSecondaryPoint(origPoint) - sizeHelper.GetSecondaryPoint(point)) > 150) return;
			fieldInfo.SetValue(thumb, sizeHelper.CreatePoint(sizeHelper.GetDefinePoint(point), sizeHelper.GetSecondaryPoint(origPoint)));
		}
		void Release() {
			thumb.LayoutUpdated -= thumb_LayoutUpdated;
			thumb = null;
		}
	}
}
