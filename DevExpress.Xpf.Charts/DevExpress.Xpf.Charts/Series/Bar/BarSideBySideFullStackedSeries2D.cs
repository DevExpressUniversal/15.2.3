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

using System.ComponentModel;
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class BarSideBySideFullStackedSeries2D : BarFullStackedSeries2D, ISupportStackedGroup, ISideBySideStackedBarSeriesView {
		public static readonly DependencyProperty StackedGroupProperty = DependencyPropertyManager.Register("StackedGroup",
			typeof(object), typeof(BarSideBySideFullStackedSeries2D), new PropertyMetadata(null, StackedGroupPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BarSideBySideFullStackedSeries2DStackedGroup"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(ObjectTypeConverter)),
		XtraSerializableProperty
		]
		public object StackedGroup {
			get { return GetValue(StackedGroupProperty); }
			set { SetValue(StackedGroupProperty, value); }
		}
		static void StackedGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, new PropertyUpdateInfo(d, "StackedGroup"));
		}
		protected override bool NeedSeriesGroupsInteraction {
			get {
				return true;
			}
		}
		public BarSideBySideFullStackedSeries2D() {
			DefaultStyleKey = typeof(BarSideBySideFullStackedSeries2D);
		}
		#region ISideBySideStackedBarSeriesView
		object ISideBySideStackedBarSeriesView.StackedGroup {
			get {
				return this.StackedGroup;
			}
			set {
				this.StackedGroup = value;
			}
		}
		double ISideBySideBarSeriesView.BarDistance {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).BarDistance : 0; }
			set {
				if (Diagram is XYDiagram2D)
					((XYDiagram2D)Diagram).BarDistance = value;
			}
		}
		int ISideBySideBarSeriesView.BarDistanceFixed {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).BarDistanceFixed : 0; }
			set {
				if (Diagram is XYDiagram2D)
					((XYDiagram2D)Diagram).BarDistanceFixed = value;
			}
		}
		bool ISideBySideBarSeriesView.EqualBarWidth {
			get { return Diagram is XYDiagram2D ? ((XYDiagram2D)Diagram).EqualBarWidth : true; }
			set {
				if (Diagram is XYDiagram2D)
					((XYDiagram2D)Diagram).EqualBarWidth = value;
			}
		}
		object ISupportSeriesGroups.SeriesGroup {
			get { return StackedGroup; }
			set { StackedGroup = value; }
		}
		#endregion
		protected override double GetBarWidth(RefinedPoint pointInfo) {
			return ((ISideBySidePoint)pointInfo).BarWidth;
		}
		protected override int GetFixedOffset(RefinedPoint pointInfo) {
			return ((ISideBySidePoint)pointInfo).FixedOffset;
		}
		protected override double GetDisplayOffset(RefinedPoint pointInfo) {
			return ((ISideBySidePoint)pointInfo).Offset;
		}
		protected override Series CreateObjectForClone() {
			return new BarSideBySideFullStackedSeries2D();
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BarSideBySideFullStackedSeries2D barSideBySideFullStackedSeries2D = series as BarSideBySideFullStackedSeries2D;
			if (barSideBySideFullStackedSeries2D != null)
				CopyPropertyValueHelper.CopyPropertyValue(this, barSideBySideFullStackedSeries2D, StackedGroupProperty);
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.FullStackedGroupViewPointPatterns;
		}
		protected internal override string[] GetAvailableSeriesPatternPlaceholders() {
			return ToolTipPatternUtils.StackedGroupViewSeriesPatterns;
		}
	}
}
