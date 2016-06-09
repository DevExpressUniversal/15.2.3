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
using System.ComponentModel;
using DevExpress.Map;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
namespace DevExpress.Xpf.Map {
	public abstract class MiniMapBehavior : MapDependencyObject {
		protected internal abstract double CalculateZoomLevel(double value);
		protected internal virtual CoordPoint Center { get { return null; } }
	}
	public class FixedMiniMapBehavior : MiniMapBehavior {
		public static readonly DependencyProperty ZoomLevelProperty = DependencyPropertyManager.Register("ZoomLevel",
			typeof(double), typeof(FixedMiniMapBehavior), new FrameworkPropertyMetadata(1.0));
		public static readonly DependencyProperty CenterPointProperty = DependencyPropertyManager.Register("CenterPoint",
			typeof(GeoPoint), typeof(FixedMiniMapBehavior), new FrameworkPropertyMetadata(new GeoPoint(), null, CoerceCenterPoint));
		static object CoerceCenterPoint(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		[Category(Categories.Behavior)]
		public double ZoomLevel {
			get { return (double)GetValue(ZoomLevelProperty); }
			set { SetValue(ZoomLevelProperty, value); }
		}
		[Category(Categories.Behavior)]
		public GeoPoint CenterPoint {
			get { return (GeoPoint)GetValue(CenterPointProperty); }
			set { SetValue(CenterPointProperty, value); }
		}
		protected internal override CoordPoint Center { get { return CenterPoint != null ? CenterPoint : new GeoPoint(); } }
		protected override MapDependencyObject CreateObject() {
			return new FixedMiniMapBehavior();
		}
		protected internal override double CalculateZoomLevel(double value) {
			return ZoomLevel;
		}
	}
	public class DynamicMiniMapBehavior : MiniMapBehavior {
		public static readonly DependencyProperty ZoomOffsetProperty = DependencyPropertyManager.Register("ZoomOffset",
			typeof(double), typeof(DynamicMiniMapBehavior), new FrameworkPropertyMetadata(-4.0));
		public static readonly DependencyProperty MinZoomLevelProperty = DependencyPropertyManager.Register("MinZoomLevel",
			typeof(double), typeof(DynamicMiniMapBehavior), new FrameworkPropertyMetadata(1.0));
		public static readonly DependencyProperty MaxZoomLevelProperty = DependencyPropertyManager.Register("MaxZoomLevel",
			typeof(double), typeof(DynamicMiniMapBehavior), new FrameworkPropertyMetadata(30.0)); 
		[Category(Categories.Behavior)]
		public double ZoomOffset {
			get { return (double)GetValue(ZoomOffsetProperty); }
			set { SetValue(ZoomOffsetProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MinZoomLevel {
			get { return (double)GetValue(MinZoomLevelProperty); }
			set { SetValue(MinZoomLevelProperty, value); }
		}
		[Category(Categories.Behavior)]
		public double MaxZoomLevel {
			get { return (double)GetValue(MaxZoomLevelProperty); }
			set { SetValue(MaxZoomLevelProperty, value); }
		}
		protected override MapDependencyObject CreateObject() {
			return new DynamicMiniMapBehavior();
		}
		protected internal override double CalculateZoomLevel(double value) {
			return MathUtils.MinMax(value + ZoomOffset, MinZoomLevel, MaxZoomLevel);
		}
	}
}
