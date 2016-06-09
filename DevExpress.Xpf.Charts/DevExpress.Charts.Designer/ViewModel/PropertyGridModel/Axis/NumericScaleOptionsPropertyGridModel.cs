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
	public abstract class WpfChartNumericScaleOptionsBasePropertyGridModel : NestedElementPropertyGridModelBase {
		public static WpfChartNumericScaleOptionsBasePropertyGridModel CreatePropertyGridModel(ChartModelElement modelElement, NumericScaleOptionsBase numericOptions, string propertyPath) {
			if (numericOptions is AutomaticNumericScaleOptions)
				return new WpfChartAutomaticNumericScaleOptionsPropertyGridModel(modelElement, (AutomaticNumericScaleOptions)numericOptions, propertyPath);
			if (numericOptions is ManualNumericScaleOptions)
				return new WpfChartManualNumericScaleOptionsPropertyGridModel(modelElement, (ManualNumericScaleOptions)numericOptions, propertyPath);
			if (numericOptions is ContinuousNumericScaleOptions)
				return new WpfChartContinuousNumericScaleOptionsPropertyGridModel(modelElement, (ContinuousNumericScaleOptions)numericOptions, propertyPath);
			return null;
		}
		readonly NumericScaleOptionsBase numericOptions;
		readonly SetAxisPropertyCommand setPropertyCommand;
		protected internal NumericScaleOptionsBase NumericScaleOptions { get { return numericOptions; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		public WpfChartNumericScaleOptionsBasePropertyGridModel(ChartModelElement modelElement, NumericScaleOptionsBase numericOptions, string propertyPath)
			: base(modelElement, propertyPath) {
			this.numericOptions = numericOptions;
			this.setPropertyCommand = new SetAxisPropertyCommand(ChartModel);
		}
		public abstract NumericScaleOptionsBase CreateScaleOptions();
	}
	public class WpfChartAutomaticNumericScaleOptionsPropertyGridModel : WpfChartNumericScaleOptionsBasePropertyGridModel {
		new AutomaticNumericScaleOptions NumericScaleOptions { get { return (AutomaticNumericScaleOptions)base.NumericScaleOptions; } }
		[Category(Categories.Behavior)]
		public AggregateFunction AggregateFunction {
			get { return NumericScaleOptions.AggregateFunction; }
			set { SetProperty("AggregateFunction", value); }
		}
		public WpfChartAutomaticNumericScaleOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartAutomaticNumericScaleOptionsPropertyGridModel(ChartModelElement modelElement, AutomaticNumericScaleOptions numericOptions, string propertyPath)
			: base(modelElement, numericOptions, propertyPath) {
		}
		public override NumericScaleOptionsBase CreateScaleOptions() {
			return new AutomaticNumericScaleOptions();
		}
	}
	public class WpfChartManualNumericScaleOptionsPropertyGridModel : WpfChartNumericScaleOptionsBasePropertyGridModel {
		new ManualNumericScaleOptions NumericScaleOptions { get { return (ManualNumericScaleOptions)base.NumericScaleOptions; } }
		[Category(Categories.Behavior)]
		public AggregateFunction AggregateFunction {
			get { return NumericScaleOptions.AggregateFunction; }
			set { SetProperty("AggregateFunction", value); }
		}
		[Category(Categories.Behavior)]
		public double MeasureUnit{
			get { return NumericScaleOptions.MeasureUnit; }
			set { SetProperty("MeasureUnit", value); }
		}
		[Category(Categories.Behavior)]
		public bool AutoGrid {
			get { return NumericScaleOptions.AutoGrid; }
			set { SetProperty("AutoGrid", value); }
		}
		[Category(Categories.Behavior)]
		public double GridSpacing{
			get { return NumericScaleOptions.GridSpacing; }
			set { SetProperty("GridSpacing", value); }
		}
		[Category(Categories.Behavior)]
		public double GridOffset {
			get { return NumericScaleOptions.GridOffset; }
			set { SetProperty("GridOffset", value); }
		}
		[Category(Categories.Behavior)]
		public double GridAlignment {
			get { return NumericScaleOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		public WpfChartManualNumericScaleOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartManualNumericScaleOptionsPropertyGridModel(ChartModelElement modelElement, ManualNumericScaleOptions numericOptions, string propertyPath)
			: base(modelElement, numericOptions, propertyPath) {
		}
		public override NumericScaleOptionsBase CreateScaleOptions() {
			return new ManualNumericScaleOptions();
		}
	}
	public class WpfChartContinuousNumericScaleOptionsPropertyGridModel : WpfChartNumericScaleOptionsBasePropertyGridModel {
		new ContinuousNumericScaleOptions NumericScaleOptions { get { return (ContinuousNumericScaleOptions)base.NumericScaleOptions; } }
		[Category(Categories.Behavior)]
		public bool AutoGrid {
			get { return NumericScaleOptions.AutoGrid; }
			set { SetProperty("AutoGrid", value); }
		}
		[Category(Categories.Behavior)]
		public double GridSpacing {
			get { return NumericScaleOptions.GridSpacing; }
			set { SetProperty("GridSpacing", value); }
		}
		[Category(Categories.Behavior)]
		public double GridOffset {
			get { return NumericScaleOptions.GridOffset; }
			set { SetProperty("GridOffset", value); }
		}
		[Category(Categories.Behavior)]
		public double GridAlignment {
			get { return NumericScaleOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		public WpfChartContinuousNumericScaleOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartContinuousNumericScaleOptionsPropertyGridModel(ChartModelElement modelElement, ContinuousNumericScaleOptions numericOptions, string propertyPath)
			: base(modelElement, numericOptions, propertyPath) {
		}
		public override NumericScaleOptionsBase CreateScaleOptions() {
			return new ContinuousNumericScaleOptions();
		}
	}
}
