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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class SeriesPointItemInitializer : IInstanceInitializer {
		object IInstanceInitializer.CreateInstance(TypeInfo type) {
			return new WpfChartSeriesPointPropertyGridModel();
		}
		IEnumerable<TypeInfo> IInstanceInitializer.Types {
			get {
				return new List<TypeInfo>() {
					new TypeInfo(typeof(WpfChartSeriesPointPropertyGridModel), ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.NewSeriesPoint)),
				};
			}
		}
	}
	public class SeriesPointPropertyGridModelCollection : ObservableCollection<WpfChartSeriesPointPropertyGridModel> {
		WpfChartModel chartModel;
		WpfChartSeriesModel seriesModel;
		public SeriesPointPropertyGridModelCollection(WpfChartModel chartModel, WpfChartSeriesModel seriesModel) {
			this.chartModel = chartModel;
			this.seriesModel = seriesModel;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (WpfChartSeriesPointPropertyGridModel newPoint in e.NewItems)
						if (newPoint.IsFake) {
							AddSeriesPointCommand command = new AddSeriesPointCommand(chartModel);
							((ICommand)command).Execute(seriesModel);
							break;
						}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (WpfChartSeriesPointPropertyGridModel oldPoint in e.OldItems) {
						if (seriesModel.SeriesPointCollectionModel.ModelCollection.Contains(oldPoint.PointModel)) {
							RemoveSeriesPointCommand command = new RemoveSeriesPointCommand(chartModel);
							((ICommand)command).Execute(oldPoint.PointModel);
							break;
						}
					}
					break;
				default:
					break;
			}
		}
		protected override void InsertItem(int index, WpfChartSeriesPointPropertyGridModel item) {
			if (item.IsFake)
				base.InsertItem(index, WpfChartSeriesPointPropertyGridModel.CreatePropertyGridModelForSeriesPoint((WpfChartModel)seriesModel.GetParent<WpfChartModel>(), seriesModel.Series));
			else
				base.InsertItem(index, item);
		}
	}
	public class WpfChartSeriesPointPropertyGridModel : PropertyGridModelBase {
		readonly SetSeriesPointPropertyCommand setPropertyCommand;
		readonly SetSeriesPointAttachedPropertyCommand setAttachedPropertyCommand;
		static WpfChartSeriesPointPropertyGridModel CreatePropertyGridModelForSeriesPoint(WpfChartModel chartModel, Series series, WpfChartSeriesPointModel pointModel) {
			if (series is RangeAreaSeries2D)
				return new WpfChartRangeAreaSeries2DPointPropertyGridModel(chartModel, pointModel);
			if (series is RangeBarSeries2D)
				return new WpfChartRangeBarSeries2DPointPropertyGridModel(chartModel, pointModel);
			if (series is PieSeries)
				return new WpfChartPieSeriesPointPropertyGridModel(chartModel, pointModel);
			if (series is BubbleSeries3D)
				return new WpfChartBubbleSeries3DPointPropertyGridModel(chartModel, pointModel);
			if (series is BubbleSeries2D)
				return new WpfChartBubbleSeries2DPointPropertyGridModel(chartModel, pointModel);
			if (series is FinancialSeries2D)
				return new WpfChartFinancialSeries2DPointPropertyGridModel(chartModel, pointModel);
			return new WpfChartSeriesPointPropertyGridModel(chartModel, pointModel);
		}
		public static WpfChartSeriesPointPropertyGridModel CreatePropertyGridModelForSeriesPoint(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel) {
			return CreatePropertyGridModelForSeriesPoint(chartModel, pointModel.Series, pointModel);
		}
		public static WpfChartSeriesPointPropertyGridModel CreatePropertyGridModelForSeriesPoint(WpfChartModel chartModel, Series series) {
			return CreatePropertyGridModelForSeriesPoint(chartModel, series, null);
		}
		WpfChartSeriesPointModel pointModel;
		internal bool IsFake { get { return pointModel == null; } }
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		protected override ICommand SetObjectAttachedPropertyCommand { get { return setAttachedPropertyCommand; } }
		protected SeriesPoint Point { get { return pointModel.SeriesPoint; } }
		protected internal WpfChartSeriesPointModel PointModel {
			get { return pointModel; }
			set { pointModel = value; }
		}
		[Category(Categories.Common)]
		public string Argument {
			get { return Point.Argument; }
			set {
				SeriesPointCommandParameter parameter = new SeriesPointCommandParameter(PointModel, value);
				SetProperty(new SetSeriesPointArgumentCommand(ChartModel), parameter);
			}
		}
		[Category(Categories.Common)]
		public double Value {
			get { return Point.Value; }
			set {
				SeriesPointCommandParameter parameter = new SeriesPointCommandParameter(PointModel, value);
				SetProperty(new SetSeriesPointValueCommand(ChartModel), parameter);
			}
		}
		[Category(Categories.Common)]
		public DateTime DateTimeValue{
			get{ return Point.DateTimeValue; }
			set{
				SeriesPointCommandParameter parameter = new SeriesPointCommandParameter(PointModel, value);
				SetProperty(new SetSeriesPointValueCommand(ChartModel), parameter);
			}
		}
		[Category(Categories.Common)]
		public object ToolTipHint{
			get{ return Point.ToolTipHint; }
			set {
				SetProperty("ToolTipHint", value, PointModel);
			}
		}
		[Category(Categories.Common)]
		public object Tag {
			get { return Point.Tag; }
			set {
				SetProperty("Tag", value, PointModel);
			}
		}
		public SolidColorBrush Brush {
			get { return Point.Brush; }
			set {
				SetProperty("Brush", value, PointModel);
			}
		}
		public WpfChartSeriesPointPropertyGridModel() : this(null, null) {
		}
		public WpfChartSeriesPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel) : base(chartModel) {
			this.pointModel = pointModel;
			setPropertyCommand = new SetSeriesPointPropertyCommand(ChartModel);
			setAttachedPropertyCommand = new SetSeriesPointAttachedPropertyCommand(ChartModel);
		}
	}
	public class WpfChartFinancialSeries2DPointPropertyGridModel : WpfChartSeriesPointPropertyGridModel {
		[Browsable(false)]
		public new double Value {
			get { return Double.NaN; }
			set { ; }
		}
		[Category(Categories.Data)]
		public double LowValue {
			get { return FinancialSeries2D.GetLowValue(Point); }
			set {
				SetAttachedProperty("LowValue", value, PointModel, typeof(FinancialSeries2D),
					delegate(object targetObject, object newValue) {
						FinancialSeries2D.SetLowValue((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return FinancialSeries2D.GetLowValue((SeriesPoint)targetObject);
					});
			}
		}
		[Category(Categories.Data)]
		public double HighValue {
			get { return FinancialSeries2D.GetHighValue(Point); }
			set {
				SetAttachedProperty("HighValue", value, PointModel, typeof(FinancialSeries2D),
					delegate(object targetObject, object newValue) {
						FinancialSeries2D.SetHighValue((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return FinancialSeries2D.GetHighValue((SeriesPoint)targetObject);
					});
			}
		}
		[Category(Categories.Data)]
		public double OpenValue {
			get { return FinancialSeries2D.GetOpenValue(Point); }
			set {
				SetAttachedProperty("OpenValue", value, PointModel, typeof(FinancialSeries2D),
					delegate(object targetObject, object newValue) {
						FinancialSeries2D.SetOpenValue((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return FinancialSeries2D.GetOpenValue((SeriesPoint)targetObject);
					});
			}
		}
		[Category(Categories.Data)]
		public double CloseValue {
			get { return FinancialSeries2D.GetCloseValue(Point); }
			set {
				SetAttachedProperty("CloseValue", value, PointModel, typeof(FinancialSeries2D),
					delegate(object targetObject, object newValue) {
						FinancialSeries2D.SetCloseValue((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return FinancialSeries2D.GetCloseValue((SeriesPoint)targetObject);
					});
			}
		}
		public WpfChartFinancialSeries2DPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel) : base(chartModel, pointModel) {
		}
	}
	public class WpfChartBubbleSeries2DPointPropertyGridModel : WpfChartSeriesPointPropertyGridModel {
		[Category(Categories.Data)]
		public double Weight {
			get { return BubbleSeries2D.GetWeight(Point); }
			set {
				SetAttachedProperty("Weight", value, PointModel, typeof(BubbleSeries2D),
					delegate(object targetObject, object newValue) {
						BubbleSeries2D.SetWeight((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return BubbleSeries2D.GetWeight((SeriesPoint)targetObject);
					});
			}
		}
		public WpfChartBubbleSeries2DPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel) : base(chartModel, pointModel) {
		}
	}
	public class WpfChartBubbleSeries3DPointPropertyGridModel : WpfChartSeriesPointPropertyGridModel {
		[Category(Categories.Data)]
		public double Weight {
			get { return BubbleSeries3D.GetWeight(Point); }
			set {
				SetAttachedProperty("Weight", value, PointModel, typeof(BubbleSeries3D),
					delegate(object targetObject, object newValue) {
						BubbleSeries3D.SetWeight((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return BubbleSeries3D.GetWeight((SeriesPoint)targetObject);
					});
			}
		}
		public WpfChartBubbleSeries3DPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel) : base(chartModel, pointModel) {
		}
	}
	public class WpfChartPieSeriesPointPropertyGridModel : WpfChartSeriesPointPropertyGridModel {
		[Category(Categories.Layout)]
		public double ExplodedDistance {
			get { return PieSeries.GetExplodedDistance(Point); }
			set {
				SetAttachedProperty("ExplodedDistance", value, PointModel, typeof(PieSeries),
					delegate(object targetObject, object newValue) {
						PieSeries.SetExplodedDistance((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return PieSeries.GetExplodedDistance((SeriesPoint)targetObject);
					});
			}
		}
		public WpfChartPieSeriesPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel)
			: base(chartModel, pointModel) {
		}
	}
	public class WpfChartRangeBarSeries2DPointPropertyGridModel : WpfChartSeriesPointPropertyGridModel {
		[Category(Categories.Data)]
		public double Value2 {
			get { return RangeBarSeries2D.GetValue2(Point); }
			set {
				SetAttachedProperty("Value2", value, PointModel, typeof(RangeBarSeries2D),
					delegate(object targetObject, object newValue) {
						RangeBarSeries2D.SetValue2((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return RangeBarSeries2D.GetValue2((SeriesPoint)targetObject);
					});
			}
		}
		[Category(Categories.Data)]
		public DateTime DateTimeValue2 {
			get { return RangeBarSeries2D.GetDateTimeValue2(Point); }
			set {
				SetAttachedProperty("DateTimeValue2", value, PointModel, typeof(RangeBarSeries2D),
					delegate(object targetObject, object newValue) {
						RangeBarSeries2D.SetDateTimeValue2((SeriesPoint)targetObject, (DateTime)newValue);
					},
					delegate(object targetObject) {
						return RangeBarSeries2D.GetDateTimeValue2((SeriesPoint)targetObject);
					});
			}
		}
		public WpfChartRangeBarSeries2DPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel)
			: base(chartModel, pointModel) {
		}
	}
	public class WpfChartRangeAreaSeries2DPointPropertyGridModel : WpfChartSeriesPointPropertyGridModel {
		[Category(Categories.Data)]
		public double Value2 {
			get { return RangeAreaSeries2D.GetValue2(Point); }
			set {
				SetAttachedProperty("Value2", value, PointModel, typeof(RangeAreaSeries2D),
					delegate(object targetObject, object newValue) {
						RangeAreaSeries2D.SetValue2((SeriesPoint)targetObject, (double)newValue);
					},
					delegate(object targetObject) {
						return RangeAreaSeries2D.GetValue2((SeriesPoint)targetObject);
					});
			}
		}
		[Category(Categories.Data)]
		public DateTime DateTimeValue2 {
			get { return RangeAreaSeries2D.GetDateTimeValue2(Point); }
			set {
				SetAttachedProperty("DateTimeValue2", value, PointModel, typeof(RangeAreaSeries2D),
					delegate(object targetObject, object newValue) {
						RangeAreaSeries2D.SetDateTimeValue2((SeriesPoint)targetObject, (DateTime)newValue);
					},
					delegate(object targetObject) {
						return RangeAreaSeries2D.GetDateTimeValue2((SeriesPoint)targetObject);
					});
			}
		}
		public WpfChartRangeAreaSeries2DPointPropertyGridModel(WpfChartModel chartModel, WpfChartSeriesPointModel pointModel)
			: base(chartModel, pointModel) {
		}
	}
}
