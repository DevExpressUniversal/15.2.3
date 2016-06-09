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
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class RangeModel : DesignerChartElementModelBase {
		readonly Range range;
		AxisModel AxisModel { get { return (AxisModel)Parent; } }
		protected Range Range { get { return range; } }
		protected internal override ChartElement ChartElement { get { return range; } }
		[PropertyForOptions(0, ""), TypeConverter(typeof(BooleanTypeConverter))]
		public bool Auto {
			get { return Range.Auto; }
			set { SetProperty("Auto", value); }
		}
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool AutoSideMargins {
			get { return Range.AutoSideMargins; }
			set { SetProperty("AutoSideMargins", value); }
		}
		public double SideMarginsValue {
			get { return Range.SideMarginsValue; }
			set { SetProperty("SideMarginsValue", value); }
		}
		[PropertyForOptions(1, ""),
		DependentUpon("Auto"),
		TypeConverter(typeof(RangeValueTypeConverter))]
		public string MinValue {
			get { return Range.MinValue.ToString(); }
			set {
				object nativeValue = AxisModel.EditorValueToNative(value);
				SetProperty("MinValue", nativeValue);
			}
		}
		[PropertyForOptions(2, ""),
		DependentUpon("Auto"),
		TypeConverter(typeof(RangeValueTypeConverter))]
		public string MaxValue {
			get { return Range.MaxValue.ToString(); }
			set {
				object nativeValue = AxisModel.EditorValueToNative(value);
				SetProperty("MaxValue", nativeValue);
			}
		}
		public RangeModel(Range range, CommandManager commandManager)
			: base(commandManager) {
			this.range = range;
		}
	}
	[ModelOf(typeof(VisualRange)), TypeConverter(typeof(VisualRangeTypeConverter))]
	public class VisualRangeModel : RangeModel {
		protected VisualRange VisualRange { get { return (VisualRange)base.Range; } }
		public VisualRangeModel(VisualRange visualRange, CommandManager commandManager)
			: base(visualRange, commandManager) {
		}
	}
	[ModelOf(typeof(WholeRange)), TypeConverter(typeof(WholeRangeTypeConverter))]
	public class WholeRangeModel : RangeModel {
		protected WholeRange WholeRange { get { return (WholeRange)base.Range; } }
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool AlwaysShowZeroLevel {
			get { return WholeRange.AlwaysShowZeroLevel; }
			set { SetProperty("AlwaysShowZeroLevel", value); }
		}
		public WholeRangeModel(WholeRange wholeRange, CommandManager commandManager)
			: base(wholeRange, commandManager) {
		}
	}
	[TypeConverter(typeof(StripLimitTypeConverter))]
	public abstract class StripLimitModel : DesignerChartElementModelBase {
		readonly StripLimit stripLimit;
		protected StripLimit StripLimit { get { return stripLimit; } }
		protected internal override ChartElement ChartElement { get { return stripLimit; } }
		[PropertyForOptions("Behavior"), TypeConverter(typeof(BooleanTypeConverter))]
		public bool Enabled {
			get { return StripLimit.Enabled; }
			set { SetProperty("Enabled", value); }
		}
		[
		PropertyForOptions("Behavior"),
		DependentUpon("Enabled"),
		TypeConverter(typeof(AxisValueTypeConverter))
		]
		public object AxisValue {
			get { return StripLimit.AxisValue; }
			set { SetProperty("AxisValue", value); }
		}
		public StripLimitModel(StripLimit stripLimit, CommandManager commandManager)
			: base(commandManager) {
			this.stripLimit = stripLimit;
		}
	}
	[ModelOf(typeof(MinStripLimit))]
	public class MinStripLimitModel : StripLimitModel {
		protected new MinStripLimit StripLimit { get { return (MinStripLimit)base.StripLimit; } }
		public MinStripLimitModel(MinStripLimit stripLimit, CommandManager commandManager)
			: base(stripLimit, commandManager) {
		}
	}
	[ModelOf(typeof(MaxStripLimit))]
	public class MaxStripLimitModel : StripLimitModel {
		protected new MaxStripLimit StripLimit { get { return (MaxStripLimit)base.StripLimit; } }
		public MaxStripLimitModel(MaxStripLimit stripLimit, CommandManager commandManager)
			: base(stripLimit, commandManager) {
		}
	}
}
