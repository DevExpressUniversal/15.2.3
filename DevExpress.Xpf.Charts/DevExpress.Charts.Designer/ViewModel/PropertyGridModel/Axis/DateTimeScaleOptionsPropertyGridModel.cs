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
	public abstract class WpfChartDateTimeScaleOptionsBasePropertyGridModel : NestedElementPropertyGridModelBase {
		public static WpfChartDateTimeScaleOptionsBasePropertyGridModel CreatePropertyGridModel(ChartModelElement modelElement, DateTimeScaleOptionsBase dateTimeOptions, string propertyPath) {
			if (dateTimeOptions is AutomaticDateTimeScaleOptions)
				return new WpfChartAutomaticDateTimeScaleOptionsPropertyGridModel(modelElement, (AutomaticDateTimeScaleOptions)dateTimeOptions, propertyPath);
			if (dateTimeOptions is ManualDateTimeScaleOptions)
				return new WpfChartManualDateTimeScaleOptionsPropertyGridModel(modelElement, (ManualDateTimeScaleOptions)dateTimeOptions, propertyPath);
			if (dateTimeOptions is ContinuousDateTimeScaleOptions)
				return new WpfChartContinuousDateTimeScaleOptionsPropertyGridModel(modelElement, (ContinuousDateTimeScaleOptions)dateTimeOptions, propertyPath);
			return null;
		}
		readonly DateTimeScaleOptionsBase dateTimeOptions;
		readonly SetAxisPropertyCommand setPropertyCommand;
		protected internal DateTimeScaleOptionsBase DateTimeScaleOptions { get { return dateTimeOptions; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		public WpfChartDateTimeScaleOptionsBasePropertyGridModel(ChartModelElement modelElement, DateTimeScaleOptionsBase dateTimeOptions, string propertyPath)
			: base(modelElement, propertyPath) {
			this.dateTimeOptions = dateTimeOptions;
			this.setPropertyCommand = new SetAxisPropertyCommand(ChartModel);
		}
		public abstract DateTimeScaleOptionsBase CreateScaleOptions();
	}
	public class WpfChartAutomaticDateTimeScaleOptionsPropertyGridModel : WpfChartDateTimeScaleOptionsBasePropertyGridModel {
		new AutomaticDateTimeScaleOptions DateTimeScaleOptions { get { return (AutomaticDateTimeScaleOptions)base.DateTimeScaleOptions; } }
		[Category(Categories.Behavior)]
		public AggregateFunction AggregateFunction {
			get { return DateTimeScaleOptions.AggregateFunction; }
			set { SetProperty("AggregateFunction", value); }
		}
		public WpfChartAutomaticDateTimeScaleOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartAutomaticDateTimeScaleOptionsPropertyGridModel(ChartModelElement modelElement, AutomaticDateTimeScaleOptions dateTimeOptions, string propertyPath)
			: base(modelElement, dateTimeOptions, propertyPath) {
		}
		public override DateTimeScaleOptionsBase CreateScaleOptions() {
			return new AutomaticDateTimeScaleOptions();
		}
	}
	public class WpfChartManualDateTimeScaleOptionsPropertyGridModel : WpfChartDateTimeScaleOptionsBasePropertyGridModel {
		new ManualDateTimeScaleOptions DateTimeScaleOptions { get { return (ManualDateTimeScaleOptions)base.DateTimeScaleOptions; } }
		[Category(Categories.Behavior)]
		public DateTimeGridAlignment GridAlignment {
			get { return DateTimeScaleOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		[Category(Categories.Behavior)]
		public AggregateFunction AggregateFunction {
			get { return DateTimeScaleOptions.AggregateFunction; }
			set { SetProperty("AggregateFunction", value); }
		}
		[Category(Categories.Behavior)]
		public DateTimeMeasureUnit MeasureUnit{
			get { return DateTimeScaleOptions.MeasureUnit; }
			set { SetProperty("MeasureUnit", value); }
		}
		[Category(Categories.Behavior)]
		public bool AutoGrid {
			get { return DateTimeScaleOptions.AutoGrid; }
			set { SetProperty("AutoGrid", value); }
		}
		[Category(Categories.Behavior)]
		public double GridSpacing{
			get { return DateTimeScaleOptions.GridSpacing; }
			set { SetProperty("GridSpacing", value); }
		}
		[Category(Categories.Behavior)]
		public double GridOffset {
			get { return DateTimeScaleOptions.GridOffset; }
			set { SetProperty("GridOffset", value); }
		}
		public WpfChartManualDateTimeScaleOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartManualDateTimeScaleOptionsPropertyGridModel(ChartModelElement modelElement, ManualDateTimeScaleOptions dateTimeOptions, string propertyPath)
			: base(modelElement, dateTimeOptions, propertyPath) {
		}
		public override DateTimeScaleOptionsBase CreateScaleOptions() {
			return new ManualDateTimeScaleOptions();
		}
	}
	public class WpfChartContinuousDateTimeScaleOptionsPropertyGridModel : WpfChartDateTimeScaleOptionsBasePropertyGridModel {
		new ContinuousDateTimeScaleOptions DateTimeScaleOptions { get { return (ContinuousDateTimeScaleOptions)base.DateTimeScaleOptions; } }
		[Category(Categories.Behavior)]
		public DateTimeGridAlignment GridAlignment {
			get { return DateTimeScaleOptions.GridAlignment; }
			set { SetProperty("GridAlignment", value); }
		}
		[Category(Categories.Behavior)]
		public bool AutoGrid {
			get { return DateTimeScaleOptions.AutoGrid; }
			set { SetProperty("AutoGrid", value); }
		}
		[Category(Categories.Behavior)]
		public double GridSpacing {
			get { return DateTimeScaleOptions.GridSpacing; }
			set { SetProperty("GridSpacing", value); }
		}
		[Category(Categories.Behavior)]
		public double GridOffset {
			get { return DateTimeScaleOptions.GridOffset; }
			set { SetProperty("GridOffset", value); }
		}
		public WpfChartContinuousDateTimeScaleOptionsPropertyGridModel() : this(null, null, string.Empty) { }
		public WpfChartContinuousDateTimeScaleOptionsPropertyGridModel(ChartModelElement modelElement, ContinuousDateTimeScaleOptions dateTimeOptions, string propertyPath)
			: base(modelElement, dateTimeOptions, propertyPath) {
		}
		public override DateTimeScaleOptionsBase CreateScaleOptions() {
			return new ContinuousDateTimeScaleOptions();
		}
	}
}
