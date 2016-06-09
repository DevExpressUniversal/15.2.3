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

extern alias Platform;
using System;
using System.Security;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer.Native {
	public class ChangeSeriesTypeCommand : SeriesOptionsCommandBase {
		const double defaultCloseValueOffset = 1.0;
		const double defaultHighValueOffset = 2.0;
		const double defaultLowValueOffset = -1.0;
		const double defaultOpenValueOffset = 0.0;
		const double defaultValue2Offset = 1.0;
		const double defaultWeight = 1.0;
		string caption;
		readonly TimeSpan defaultDateTimeValue2Offset = TimeSpan.FromDays(1.0);
		string imageName;
		Type seriesType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeSeriesTypeCommand(WpfChartModel model, Type seriesType)
			: base(model) {
			this.seriesType = seriesType;
			this.imageName = GlyphUtils.Series + seriesType.Name;
			ChartDesignerStringIDs captionID = (ChartDesignerStringIDs)Enum.Parse(typeof(ChartDesignerStringIDs), seriesType.Name);
			this.caption = ChartDesignerLocalizer.GetString(captionID);
		}
		[ SecuritySafeCritical ]
		void CopyAdditionalValuesForPoint(Series source, SeriesPoint sourcePoint, IModelItem pointModelItem) {
			if (source is RangeBarSeries2D) {
				DXPropertyIdentifier value2Property = new DXPropertyIdentifier(typeof(RangeBarSeries2D), "Value2");
				pointModelItem.Properties[value2Property].SetValue(RangeBarSeries2D.GetValue2(sourcePoint));
				DXPropertyIdentifier dateTimeValue2Property = new DXPropertyIdentifier(typeof(RangeBarSeries2D), "DateTimeValue2");
				pointModelItem.Properties[dateTimeValue2Property].SetValue(RangeBarSeries2D.GetDateTimeValue2(sourcePoint));
			}
			if (source is RangeAreaSeries2D) {
				DXPropertyIdentifier value2Property = new DXPropertyIdentifier(typeof(RangeAreaSeries2D), "Value2");
				pointModelItem.Properties[value2Property].SetValue(RangeAreaSeries2D.GetValue2(sourcePoint));
				DXPropertyIdentifier dateTimeValue2Property = new DXPropertyIdentifier(typeof(RangeAreaSeries2D), "DateTimeValue2");
				pointModelItem.Properties[dateTimeValue2Property].SetValue(RangeAreaSeries2D.GetDateTimeValue2(sourcePoint));
			}
			if (source is BubbleSeries2D) {
				DXPropertyIdentifier weightProperty = new DXPropertyIdentifier(typeof(BubbleSeries2D), "Weight");
				pointModelItem.Properties[weightProperty].SetValue(BubbleSeries2D.GetWeight(sourcePoint));
			}
			if (source is BubbleSeries3D) {
				DXPropertyIdentifier weightProperty = new DXPropertyIdentifier(typeof(BubbleSeries3D), "Weight");
				pointModelItem.Properties[weightProperty].SetValue(BubbleSeries3D.GetWeight(sourcePoint));
			}
			if (source is FinancialSeries2D) {
				DXPropertyIdentifier lowValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "LowValue");
				DXPropertyIdentifier openValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "OpenValue");
				DXPropertyIdentifier closeValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "CloseValue");
				DXPropertyIdentifier highValueProperty = new DXPropertyIdentifier(typeof(FinancialSeries2D), "HighValue");
				pointModelItem.Properties[lowValueProperty].SetValue(FinancialSeries2D.GetLowValue(sourcePoint));
				pointModelItem.Properties[openValueProperty].SetValue(FinancialSeries2D.GetOpenValue(sourcePoint));
				pointModelItem.Properties[closeValueProperty].SetValue(FinancialSeries2D.GetCloseValue(sourcePoint));
				pointModelItem.Properties[highValueProperty].SetValue(FinancialSeries2D.GetHighValue(sourcePoint));
			}
		}
		SeriesPoint CopySeriesPoint(SeriesPoint sourcePoint) {
			SeriesPoint newPoint = new SeriesPoint(sourcePoint.Argument, sourcePoint.Value);
			if (seriesType == typeof(BubbleSeries2D)) {
				double weight = BubbleSeries2D.GetWeight(sourcePoint);
				if (Double.IsNaN(weight))
					weight = defaultWeight;
				BubbleSeries2D.SetWeight(newPoint, weight);
			}
			if (seriesType == typeof(BubbleSeries3D)) {
				double weight = BubbleSeries3D.GetWeight(sourcePoint);
				if (Double.IsNaN(weight))
					weight = defaultWeight;
				BubbleSeries3D.SetWeight(newPoint, weight);
			}
			if (seriesType == typeof(RangeAreaSeries2D)) {
				double value2 = RangeAreaSeries2D.GetValue2(sourcePoint);
				if (Double.IsNaN(value2))
					value2 = sourcePoint.Value + defaultValue2Offset;
				RangeAreaSeries2D.SetValue2(newPoint, value2);
				DateTime dateTimeValue2 = RangeAreaSeries2D.GetDateTimeValue2(sourcePoint);
				if (dateTimeValue2 == DateTime.MaxValue && sourcePoint.DateTimeValue < DateTime.MaxValue - defaultDateTimeValue2Offset)
					dateTimeValue2 = sourcePoint.DateTimeValue + defaultDateTimeValue2Offset;
				RangeAreaSeries2D.SetDateTimeValue2(newPoint, dateTimeValue2);
			}
			if (seriesType.IsSubclassOf(typeof(RangeBarSeries2D))) {
				double value2 = RangeBarSeries2D.GetValue2(sourcePoint);
				if (Double.IsNaN(value2))
					value2 = sourcePoint.Value + defaultValue2Offset;
				RangeBarSeries2D.SetValue2(newPoint, value2);
				DateTime dateTimeValue2 = RangeBarSeries2D.GetDateTimeValue2(sourcePoint);
				if (dateTimeValue2 == DateTime.MaxValue && sourcePoint.DateTimeValue < DateTime.MaxValue - defaultDateTimeValue2Offset)
					dateTimeValue2 = sourcePoint.DateTimeValue + defaultDateTimeValue2Offset;
				RangeBarSeries2D.SetDateTimeValue2(newPoint, dateTimeValue2);
			}
			if (seriesType.IsSubclassOf(typeof(FinancialSeries2D))) {
				double openValue = FinancialSeries2D.GetOpenValue(sourcePoint);
				if (Double.IsNaN(openValue))
					openValue = sourcePoint.Value + defaultOpenValueOffset;
				FinancialSeries2D.SetOpenValue(newPoint, openValue);
				double closeValue = FinancialSeries2D.GetCloseValue(sourcePoint);
				if (Double.IsNaN(closeValue))
					closeValue = sourcePoint.Value + defaultCloseValueOffset;
				FinancialSeries2D.SetCloseValue(newPoint, closeValue);
				double lowValue = FinancialSeries2D.GetLowValue(sourcePoint);
				if (Double.IsNaN(lowValue))
					lowValue = sourcePoint.Value + defaultLowValueOffset;
				FinancialSeries2D.SetLowValue(newPoint, lowValue);
				double highValue = FinancialSeries2D.GetHighValue(sourcePoint);
				if (Double.IsNaN(highValue))
					highValue = sourcePoint.Value + defaultHighValueOffset;
				FinancialSeries2D.SetHighValue(newPoint, highValue);
			}
			return newPoint;
		}
		void CopySeriesPoints(Series source, Series target) {
			if (!ChartDesignerPropertiesProvider.GetIsAutoPointsAdded(source))
				foreach (SeriesPoint point in source.Points)
					target.Points.Add(CopySeriesPoint(point));
		}
		void CopySeriesPoints(Series source, IModelItem newSeriesModelItem, IEditingContext context) {
			foreach (SeriesPoint point in source.Points) {
				IModelItem pointModelItem = context.CreateItem(typeof(SeriesPoint));
				pointModelItem.Properties["Argument"].SetValue(point.Argument);
				pointModelItem.Properties["Value"].SetValue(point.Value);
				CopyAdditionalValuesForPoint(source, point, pointModelItem);
				newSeriesModelItem.Properties["Points"].Collection.Add(pointModelItem);
			}
		}
		void CopySeriesProperties(Series source, Series target) {
			target.DisplayName = source.DisplayName;
			target.DataSource = source.DataSource;
			target.ArgumentDataMember = source.ArgumentDataMember;
			target.ValueDataMember = source.ValueDataMember;
			if (source is FinancialSeries2D && target is FinancialSeries2D) {
				((FinancialSeries2D)target).LowValueDataMember = ((FinancialSeries2D)source).LowValueDataMember;
				((FinancialSeries2D)target).HighValueDataMember = ((FinancialSeries2D)source).HighValueDataMember;
				((FinancialSeries2D)target).OpenValueDataMember = ((FinancialSeries2D)source).OpenValueDataMember;
				((FinancialSeries2D)target).CloseValueDataMember = ((FinancialSeries2D)source).CloseValueDataMember;
			}
			string weightDataMember = String.Empty;
			if (source is BubbleSeries2D)
				weightDataMember = ((BubbleSeries2D)source).WeightDataMember;
			if (target is BubbleSeries2D)
				((BubbleSeries2D)target).WeightDataMember = weightDataMember;
			if (source is BubbleSeries3D)
				weightDataMember = ((BubbleSeries3D)source).WeightDataMember;
			if (target is BubbleSeries3D)
				((BubbleSeries3D)target).WeightDataMember = weightDataMember;
			string value2DataMember = String.Empty;
			if (source is RangeBarSeries2D)
				value2DataMember = ((RangeBarSeries2D)source).Value2DataMember;
			if (target is RangeBarSeries2D)
				((RangeBarSeries2D)target).Value2DataMember = value2DataMember;
			if (source is RangeAreaSeries2D)
				value2DataMember = ((RangeAreaSeries2D)source).Value2DataMember;
			if (target is RangeAreaSeries2D)
				((RangeAreaSeries2D)target).Value2DataMember = value2DataMember;
		}
		void CopySeriesProperties(Series source, IModelItem newSeriesModelItem) {
			if (!String.IsNullOrEmpty(source.DisplayName))
			newSeriesModelItem.Properties["DisplayName"].SetValue(source.DisplayName);
			if (!String.IsNullOrEmpty(source.ArgumentDataMember))
				newSeriesModelItem.Properties["ArgumentDataMember"].SetValue(source.ArgumentDataMember);
			if (!String.IsNullOrEmpty(source.ValueDataMember))
				newSeriesModelItem.Properties["ValueDataMember"].SetValue(source.ValueDataMember);
			if (source is FinancialSeries2D && seriesType.IsSubclassOf(typeof(FinancialSeries2D))) {
				if (!String.IsNullOrEmpty(((FinancialSeries2D)source).LowValueDataMember))
					newSeriesModelItem.Properties["LowValueDataMember"].SetValue(((FinancialSeries2D)source).LowValueDataMember);
				if (!String.IsNullOrEmpty(((FinancialSeries2D)source).HighValueDataMember))
					newSeriesModelItem.Properties["HighValueDataMember"].SetValue(((FinancialSeries2D)source).HighValueDataMember);
				if (!String.IsNullOrEmpty(((FinancialSeries2D)source).OpenValueDataMember))
					newSeriesModelItem.Properties["OpenValueDataMember"].SetValue(((FinancialSeries2D)source).OpenValueDataMember);
				if (!String.IsNullOrEmpty(((FinancialSeries2D)source).CloseValueDataMember))
					newSeriesModelItem.Properties["CloseValueDataMember"].SetValue(((FinancialSeries2D)source).CloseValueDataMember);
			}
			string weightDataMember = String.Empty;
			if (source is BubbleSeries2D)
				weightDataMember = ((BubbleSeries2D)source).WeightDataMember;
			if (seriesType == typeof(BubbleSeries2D) && !String.IsNullOrEmpty(weightDataMember))
				newSeriesModelItem.Properties["WeightDataMember"].SetValue(weightDataMember);
			if (source is BubbleSeries3D)
				weightDataMember = ((BubbleSeries3D)source).WeightDataMember;
			if (seriesType == typeof(BubbleSeries3D) && !String.IsNullOrEmpty(weightDataMember))
				newSeriesModelItem.Properties["WeightDataMember"].SetValue(weightDataMember);
			string value2DataMember = String.Empty;
			if (source is RangeBarSeries2D)
				value2DataMember = ((RangeBarSeries2D)source).Value2DataMember;
			if (seriesType.IsSubclassOf(typeof(RangeBarSeries2D)) && !String.IsNullOrEmpty(value2DataMember))
				newSeriesModelItem.Properties["Value2DataMember"].SetValue(value2DataMember);
			if (source is RangeAreaSeries2D)
				value2DataMember = ((RangeAreaSeries2D)source).Value2DataMember;
			if (seriesType == typeof(RangeAreaSeries2D) && !String.IsNullOrEmpty(value2DataMember))
				newSeriesModelItem.Properties["Value2DataMember"].SetValue(value2DataMember);
		}
		protected override bool CanExecute(object parameter) {
			Type supportedDiagramType = ChartDesignerPropertiesProvider.GetSupportedDiagramType(this.seriesType);
			return base.CanExecute(parameter) && SeriesModel != null && supportedDiagramType == PreviewChart.Diagram.GetType() && SeriesModel.Series.GetType() != this.seriesType;
		}
		[ SecuritySafeCritical ]
		protected override void DesignTimeApplyInternal(IModelItem seriesAccess, object value) { }
		protected override void RedoInternal(WpfChartSeriesModel model, object newValue) { }
		protected override void RuntimeApplyInternal(Series series, object value) { }
		protected override object RuntimeRedo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			if (seriesIndex >= 0) {
				PreviewChart.Diagram.Series.RemoveAt(seriesIndex);
				Series series = (Series)historyItem.NewValue;
				if (series is XYSeries2D) {
					int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[PaneIndexKey];
					if (paneIndex >= 0) {
						Pane pane = ((XYDiagram2D)PreviewChart.Diagram).Panes[paneIndex];
						XYDiagram2D.SetSeriesPane((XYSeries2D)series, pane);
					}
				}
				PreviewChart.Diagram.Series.Insert(seriesIndex, series);
			}
			else
				PreviewChart.Diagram.SeriesTemplate = (Series)historyItem.NewValue;
			return historyItem.NewValue;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			if (seriesIndex >= 0) {
				PreviewChart.Diagram.Series.RemoveAt(seriesIndex);
				PreviewChart.Diagram.Series.Insert(seriesIndex, (Series)historyItem.OldValue);
			}
			else
				PreviewChart.Diagram.SeriesTemplate = (Series)historyItem.OldValue;
			return historyItem.OldValue;
		}
		protected override void UndoInternal(WpfChartSeriesModel model, object oldValue) { }
		[SecuritySafeCritical]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			Series oldSeries = historyItem.OldValue as Series;
			IModelItem seriesModelItem = chartModelItem.Context.CreateItem(seriesType);
			CopySeriesProperties(oldSeries, seriesModelItem);
			if (!ChartDesignerPropertiesProvider.GetIsAutoPointsAdded(oldSeries))
				CopySeriesPoints(oldSeries, seriesModelItem, chartModelItem.Context);
			int oldSeriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesDesigntimeIndexKey];
			if (oldSeriesIndex >= 0) {
				if (historyItem.NewValue is XYSeries2D) {
					int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[PaneIndexKey];
					if (paneIndex >= 0) {
						string paneName = (string)chartModelItem.Properties["Diagram"].Value.Properties["Panes"].Collection[paneIndex].Properties["Name"].Value.GetCurrentValue();
						IModelItem bindingItem = designTimeProvider.CreateBindingItem(seriesModelItem, paneName);
						DXPropertyIdentifier panePropIdentifier = new DXPropertyIdentifier(typeof(XYDiagram2D), "SeriesPane");
						seriesModelItem.Properties[panePropIdentifier].SetValue(bindingItem);
					}
				}
				chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection.RemoveAt(oldSeriesIndex);
				chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection.Insert(oldSeriesIndex, seriesModelItem);
			}
			else
				chartModelItem.Properties["Diagram"].Value.Properties["SeriesTemplate"].SetValue(seriesModelItem);
		}
		public bool IsSeriesAtCommandState(WpfChartSeriesModel seriesModel) {
			return seriesModel.Series.GetType() == seriesType;
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			int seriesIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[SeriesIndexKey];
			Series oldSeries = (Series)historyItem.OldValue;
			Series newSeries = (Series)Activator.CreateInstance(seriesType);
			CopySeriesProperties(oldSeries, newSeries);
			if (!ChartDesignerPropertiesProvider.GetIsAutoPointsAdded(newSeries))
				CopySeriesPoints(oldSeries, newSeries);
			if (seriesIndex >= 0) {
				chartControl.Diagram.Series.RemoveAt(seriesIndex);
				if (newSeries is XYSeries2D) {
					int paneIndex = historyItem.ExecuteCommandInfo.IndexByNameDictionary[PaneIndexKey];
					if (paneIndex >= 0) {
						Pane pane = ((XYDiagram2D)chartControl.Diagram).Panes[paneIndex];
						XYDiagram2D.SetSeriesPane((XYSeries2D)newSeries, pane);
					}
				}
				chartControl.Diagram.Series.Insert(seriesIndex, newSeries);
			}
			else
				chartControl.Diagram.SeriesTemplate = newSeries;
		}
		public override CommandResult RuntimeExecute(object parameter) {
			if ((parameter == null) || (parameter is bool && (bool)parameter)) {
				Series oldSeries = SeriesModel.Series;
				Series newSeries = (Series)Activator.CreateInstance(seriesType);
				CopySeriesProperties(oldSeries, newSeries);
				if (!ChartDesignerPropertiesProvider.GetIsAutoPointsAdded(newSeries))
					CopySeriesPoints(oldSeries, newSeries);
				int seriesIndex = SeriesModel.GetSelfIndex();
				ElementIndexItem[] indexItems = new ElementIndexItem[2];
				int paneIndex = 0;
				if (oldSeries is XYSeries2D && newSeries is XYSeries2D) {
					XYSeries2D oldXYSeries2D = (XYSeries2D)oldSeries;
					XYDiagram2D xyDiagram = (XYDiagram2D)PreviewChart.Diagram;
					Pane oldSeriesPane = XYDiagram2D.GetSeriesPane(oldXYSeries2D);
					paneIndex = xyDiagram.Panes.IndexOf(oldSeriesPane);
					XYSeries2D newXYSeries2D = (XYSeries2D)newSeries;
					XYDiagram2D.SetSeriesPane(newXYSeries2D, oldSeriesPane);
					indexItems = new ElementIndexItem[3];
					indexItems[2] = new ElementIndexItem(PaneIndexKey, paneIndex);
				}
				indexItems[0] = new ElementIndexItem(SeriesIndexKey, seriesIndex);
				indexItems[1] = new ElementIndexItem(SeriesDesigntimeIndexKey, SeriesModel.GetSelfDesigntimeIndex());
				PreviewChart.Diagram.Series.Remove(oldSeries);
				if (seriesIndex >= 0)
					PreviewChart.Diagram.Series.Insert(seriesIndex, newSeries);
				else
					PreviewChart.Diagram.SeriesTemplate = newSeries;
				return new CommandResult(new HistoryItem(new ExecuteCommandInfo(parameter, indexItems), this, null, oldSeries, newSeries), newSeries);
			}
			return null;
		}
	}
}
