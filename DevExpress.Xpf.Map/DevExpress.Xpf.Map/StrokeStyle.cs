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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class StrokeStyle : MapDependencyObject {
		public static readonly DependencyProperty ThicknessProperty = DependencyPropertyManager.Register("Thickness",
			typeof(double), typeof(StrokeStyle), new PropertyMetadata(1.0, NotifyPropertyChanged));
		public static readonly DependencyProperty DashArrayProperty = DependencyPropertyManager.Register("DashArray",
			typeof(DoubleCollection), typeof(StrokeStyle), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty DashCapProperty = DependencyPropertyManager.Register("DashCap",
			typeof(PenLineCap), typeof(StrokeStyle), new PropertyMetadata(PenLineCap.Flat, NotifyPropertyChanged));
		public static readonly DependencyProperty DashOffsetProperty = DependencyPropertyManager.Register("DashOffset",
			typeof(double), typeof(StrokeStyle), new PropertyMetadata(0.0, NotifyPropertyChanged));
		public static readonly DependencyProperty EndLineCapProperty = DependencyPropertyManager.Register("EndLineCap",
			typeof(PenLineCap), typeof(StrokeStyle), new PropertyMetadata(PenLineCap.Flat, NotifyPropertyChanged));
		public static readonly DependencyProperty StartLineCapProperty = DependencyPropertyManager.Register("StartLineCap",
			typeof(PenLineCap), typeof(StrokeStyle), new PropertyMetadata(PenLineCap.Flat, NotifyPropertyChanged));
		public static readonly DependencyProperty LineJoinProperty = DependencyPropertyManager.Register("LineJoin",
			typeof(PenLineJoin), typeof(StrokeStyle), new PropertyMetadata(PenLineJoin.Miter, NotifyPropertyChanged));
		public static readonly DependencyProperty MiterLimitProperty = DependencyPropertyManager.Register("MiterLimit",
			typeof(double), typeof(StrokeStyle), new PropertyMetadata(1.0, NotifyPropertyChanged));
		[Category(Categories.Appearance)]
		public double Thickness {
			get { return (double)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		[Category(Categories.Appearance)]
		public DoubleCollection DashArray {
			get { return (DoubleCollection)GetValue(DashArrayProperty); }
			set { SetValue(DashArrayProperty, value); }
		}
		[Category(Categories.Appearance)]
		public PenLineCap DashCap {
			get { return (PenLineCap)GetValue(DashCapProperty); }
			set { SetValue(DashCapProperty, value); }
		}
		[Category(Categories.Appearance)]
		public double DashOffset {
			get { return (double)GetValue(DashOffsetProperty); }
			set { SetValue(DashOffsetProperty, value); }
		}
		[Category(Categories.Appearance)]
		public PenLineCap EndLineCap {
			get { return (PenLineCap)GetValue(EndLineCapProperty); }
			set { SetValue(EndLineCapProperty, value); }
		}
		[Category(Categories.Appearance)]
		public PenLineCap StartLineCap {
			get { return (PenLineCap)GetValue(StartLineCapProperty); }
			set { SetValue(StartLineCapProperty, value); }
		}
		[Category(Categories.Appearance)]
		public PenLineJoin LineJoin {
			get { return (PenLineJoin)GetValue(LineJoinProperty); }
			set { SetValue(LineJoinProperty, value); }
		}
		[Category(Categories.Appearance)]
		public double MiterLimit {
			get { return (double)GetValue(MiterLimitProperty); }
			set { SetValue(MiterLimitProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new StrokeStyle();
		}
	}
	public class StrokeDashArrayConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(targetType == typeof(DoubleCollection)) {
				DoubleCollection collection = value as DoubleCollection;
				if (collection != null)
					return CommonUtils.CloneDoubleCollection(collection);
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}
