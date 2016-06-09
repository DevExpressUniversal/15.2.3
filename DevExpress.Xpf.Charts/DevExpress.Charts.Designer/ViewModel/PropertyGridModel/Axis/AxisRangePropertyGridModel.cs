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
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartAxisRangePropertyGridModel : NestedElementPropertyGridModelBase {
		readonly Range range;
		readonly SetAxisPropertyCommand setPropertyCommand;
		readonly SetAxisAttachedPropertyCommand setAttachedPropertyCommand;
		protected internal Range Range { get { return range; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected override ICommand SetObjectAttachedPropertyCommand { get { return setAttachedPropertyCommand; } }
		[Category(Categories.Behavior)]
		public object MaxValue {
			get { return Range.MaxValue; }
			set { SetProperty("MaxValue", value); }
		}
		[Category(Categories.Behavior)]
		public object MinValue {
			get { return Range.MinValue; }
			set { SetProperty("MinValue", value); }
		}
		[Category(Categories.Behavior)]
		public double SideMarginsValue {
			get { return Range.SideMarginsValue; }
			set { SetProperty("SideMarginsValue", value); }
		}
		[Category(Categories.Behavior)]
		public bool AutoSideMargins {
			get { return Range.AutoSideMargins; }
			set { SetProperty("AutoSideMargins", value); }
		}
		public WpfChartAxisRangePropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartAxisRangePropertyGridModel(ChartModelElement modelElement, Range range, string propertyPath)
			: base(modelElement, propertyPath) {
			this.range = range;
			this.setPropertyCommand = new SetAxisPropertyCommand(ChartModel);
			this.setAttachedPropertyCommand = new SetAxisAttachedPropertyCommand(ChartModel);
		}
		public override string ToString() {
			return "Range";
		}
	}
	public class WpfChartAxisY2DRangePropertyGridModel : WpfChartAxisRangePropertyGridModel {
		[Category(Categories.Behavior)]
		public bool AlwaysShowZeroLevel {
			get { return AxisY2D.GetAlwaysShowZeroLevel(Range); }
			set {
				SetAttachedProperty("AlwaysShowZeroLevel", value, typeof(AxisY2D),
					delegate(object targetObject, object newValue) {
						AxisY2D.SetAlwaysShowZeroLevel((Range)targetObject, (bool)newValue);
					},
					delegate(object targetObject) {
						return AxisY2D.GetAlwaysShowZeroLevel((Range)targetObject);
					});
			}
		}
		public WpfChartAxisY2DRangePropertyGridModel() : base() { }
		public WpfChartAxisY2DRangePropertyGridModel(ChartModelElement modelElement, Range range, string propertyPath)
			: base(modelElement, range, propertyPath) {
		}
	}
	public class WpfChartAxisY3DRangePropertyGridModel : WpfChartAxisRangePropertyGridModel {
		[Category(Categories.Behavior)]
		public bool AlwaysShowZeroLevel {
			get { return AxisY3D.GetAlwaysShowZeroLevel(Range); }
			set {
				SetAttachedProperty("AlwaysShowZeroLevel", value, typeof(AxisY3D),
					delegate(object targetObject, object newValue) {
						AxisY3D.SetAlwaysShowZeroLevel((Range)targetObject, (bool)newValue);
					},
					delegate(object targetObject) {
						return AxisY3D.GetAlwaysShowZeroLevel((Range)targetObject);
					});
			}
		}
		public WpfChartAxisY3DRangePropertyGridModel() : base() { }
		public WpfChartAxisY3DRangePropertyGridModel(ChartModelElement modelElement, Range range, string propertyPath)
			: base(modelElement, range, propertyPath) { }
	}
	public class WpfChartCircularAxisY2DRangePropertyGridModel : WpfChartAxisRangePropertyGridModel {
		[Category(Categories.Behavior)]
		public bool AlwaysShowZeroLevel {
			get { return CircularAxisY2D.GetAlwaysShowZeroLevel(Range); }
			set {
				SetAttachedProperty("AlwaysShowZeroLevel", value, typeof(CircularAxisY2D),
					delegate(object targetObject, object newValue) {
						CircularAxisY2D.SetAlwaysShowZeroLevel((Range)targetObject, (bool)newValue);
					},
					delegate(object targetObject) {
						return CircularAxisY2D.GetAlwaysShowZeroLevel((Range)targetObject);
					});
			}
		}
		public WpfChartCircularAxisY2DRangePropertyGridModel() : base() { }
		public WpfChartCircularAxisY2DRangePropertyGridModel(ChartModelElement modelElement, Range range, string propertyPath)
			: base(modelElement, range, propertyPath) { }
	}
}
