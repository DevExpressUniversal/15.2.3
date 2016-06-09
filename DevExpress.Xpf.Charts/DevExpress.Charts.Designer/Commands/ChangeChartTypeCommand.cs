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
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Security;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
using Microsoft.Windows.Design.Metadata;
namespace DevExpress.Charts.Designer.Native {
	public class ChangeChartTypeCommand : ChartCommandBase {
		string caption;
		string imageName;
		Type seriesType;
		public override string Caption {
			get { return caption; }
		}
		public override string ImageName {
			get { return imageName; }
		}
		public ChangeChartTypeCommand(WpfChartModel chartModel, Type seriesType)
			: base(chartModel) {
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
			}
			if (source is RangeAreaSeries2D) {
				DXPropertyIdentifier value2Property = new DXPropertyIdentifier(typeof(RangeAreaSeries2D), "Value2");
				pointModelItem.Properties[value2Property].SetValue(RangeAreaSeries2D.GetValue2(sourcePoint));
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
		void CopySeriesPoints(Series source, Series target) {
			foreach (SeriesPoint point in source.Points)
				target.Points.Add(point);
		}
		[ SecuritySafeCritical ]
		void CopySeriesPoints(Series source, IEditingContext context, IModelItem newSeriesModelItem) {
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
		}
		[ SecuritySafeCritical ]
		void CopySeriesProperties(Series source, IModelItem newSeriesModelItem) {
			newSeriesModelItem.Properties["DisplayName"].SetValue(source.DisplayName);
		}
		protected override bool CanExecute(object parameter) {
			return PreviewChart.Diagram != null && PreviewChart.Diagram.Series != null && PreviewChart.Diagram.Series.Count > 0 && PreviewChart.Diagram.GetType() == ChartDesignerPropertiesProvider.GetSupportedDiagramType(this.seriesType);
		}
		protected override object RuntimeRedo(HistoryItem historyItem) {
			PreviewChart.Diagram.Series.Clear();
			PreviewChart.Diagram.Series.AddRange((SeriesCollection)historyItem.NewValue);
			return null;
		}
		protected override object RuntimeUndo(HistoryItem historyItem) {
			PreviewChart.Diagram.Series.Clear();
			PreviewChart.Diagram.Series.AddRange((SeriesCollection)historyItem.OldValue);
			return null;
		}
		[ SecuritySafeCritical ]
		public override void DesigntimeApply(IModelItem chartModelItem, HistoryItem historyItem, IDesignTimeProvider designTimeProvider) {
			chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection.Clear();
			foreach (Series series in (SeriesCollection)historyItem.NewValue) {
				IModelItem seriesModelItem = chartModelItem.Context.CreateItem(seriesType);
				CopySeriesProperties(series, seriesModelItem);
				CopySeriesPoints(series, chartModelItem.Context, seriesModelItem);
				chartModelItem.Properties["Diagram"].Value.Properties["Series"].Collection.Add(seriesModelItem);
			}
		}
		public override void RuntimeApply(ChartControl chartControl, HistoryItem historyItem) {
			Type diagramType = ChartDesignerPropertiesProvider.GetSupportedDiagramType(seriesType);
			if (chartControl.Diagram.GetType() != diagramType)
				chartControl.Diagram = (Diagram)Activator.CreateInstance(diagramType);
			else
				chartControl.Diagram.Series.Clear();
			chartControl.Diagram.Series.AddRange((SeriesCollection)historyItem.NewValue);
		}
		public override CommandResult RuntimeExecute(object parameter) {
			CompositeHistoryItem resultItem = new CompositeHistoryItem();
			SeriesCollection oldSeriesCollection = new SeriesCollection();
			oldSeriesCollection.AddRange(PreviewChart.Diagram.Series);
			SeriesCollection newSeriesCollection = new SeriesCollection();
			foreach (Series series in PreviewChart.Diagram.Series) {
				Series newSeries = Activator.CreateInstance(seriesType) as Series;
				CopySeriesProperties(series, newSeries);
				CopySeriesPoints(series, newSeries);
				newSeriesCollection.Add(newSeries);
			}
			Type diagramType = ChartDesignerPropertiesProvider.GetSupportedDiagramType(seriesType);
			if (PreviewChart.Diagram.GetType() != diagramType) {
				RemoveDiagramCommand removeDiagramCommand = new RemoveDiagramCommand(ChartModel);
				resultItem.HistoryItems.Add(removeDiagramCommand.RuntimeExecute(null).HistoryItem);
				CreateDiagramCommand diagramCommand = new CreateDiagramCommand(ChartModel);
				resultItem.HistoryItems.Add(diagramCommand.RuntimeExecute(diagramType).HistoryItem);
			}
			else
				PreviewChart.Diagram.Series.Clear();
			PreviewChart.Diagram.Series.AddRange(newSeriesCollection);
			resultItem.HistoryItems.Add(new HistoryItem(new ExecuteCommandInfo(parameter), this, null, oldSeriesCollection, newSeriesCollection));
			return new CommandResult(resultItem);
		}
	}
}
