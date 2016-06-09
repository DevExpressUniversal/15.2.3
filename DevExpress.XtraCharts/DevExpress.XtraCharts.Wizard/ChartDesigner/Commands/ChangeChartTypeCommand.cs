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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Designer.Native {
	public class ChangeSeriesViewOldValues {
		readonly SeriesBase series;
		readonly ViewType view;
		readonly int paneIndex;
		public SeriesBase Series { get { return series; } }
		public ViewType View { get { return view; } }
		public int PaneIndex { get { return paneIndex; } }
		public ChangeSeriesViewOldValues(SeriesBase series, ViewType view, int paneIndex) {
			this.series = series;
			this.view = view;
			this.paneIndex = paneIndex;
		}
	}
	public class ChangeChartTypeCommand : ChartCommandBase {
		readonly Chart chart;
		public ChangeChartTypeCommand(CommandManager commandManager, Chart chart)
			: base(commandManager) {
			this.chart = chart;
		}
		int GetpaneIndex(Series series) {
			int paneIndex = -1;
			XYDiagramSeriesViewBase xyView = series.View as XYDiagramSeriesViewBase;
			if (xyView != null)
				paneIndex = ((IPane)xyView.Pane).PaneIndex;
			else {
				SwiftPlotSeriesView swiftPlotView = series.View as SwiftPlotSeriesView;
				if (swiftPlotView != null)
					paneIndex = ((IPane)swiftPlotView.Pane).PaneIndex;
				else {
					GanttSeriesView ganttView = series.View as GanttSeriesView;
					if (ganttView != null)
						paneIndex = ((IPane)ganttView.Pane).PaneIndex;
				}
			}
			return paneIndex;
		}
		void SetPaneIndex(SeriesBase series, int paneIndex) {
			XYDiagram xyDiagram = chart.Diagram as XYDiagram;
			if (xyDiagram != null)
				((XYDiagramSeriesViewBase)series.View).Pane = xyDiagram.Panes[paneIndex];
			else {
				SwiftPlotDiagram swiftPlotDiagram = chart.Diagram as SwiftPlotDiagram;
				if (swiftPlotDiagram != null)
					((SwiftPlotSeriesView)series.View).Pane = swiftPlotDiagram.Panes[paneIndex];
			}
		}
		protected override object CreatePropertiesCache(ChartElement chartElement) {
			return ((Chart)chartElement).AnnotationRepository.ToArray();
		}
		protected override void RestoreProperties(ChartElement chartElement, object properties) {
			Annotation[] restoringAnnotations = (Annotation[])properties;
			AnnotationRepository annotations = ((Chart)chartElement).AnnotationRepository;
			annotations.BeginUpdate();
			try {
				annotations.Clear();
				annotations.AddRange(restoringAnnotations);
			} finally {
				annotations.EndUpdate();
			}
		}
		public override bool CanExecute(object parameter) {
			return true;
		}
		public override HistoryItem ExecuteInternal(object parameter) {
			ViewType viewType = (ViewType)parameter;
			object properties = CreatePropertiesCache(chart);
			chart.Container.LockChangeService();
			List<ChangeSeriesViewOldValues> oldValues = new List<ChangeSeriesViewOldValues>();
			for (int i = 0; i < chart.Series.Count; i++) {
				Series series = chart.Series[i];
				int oldPaneIndex = GetpaneIndex(series);
				if (!series.IsAutoCreated && series.View.GetType() != SeriesViewFactory.GetType(viewType))
					oldValues.Add(new ChangeSeriesViewOldValues(series, SeriesViewFactory.GetViewType(series.View), oldPaneIndex));
			}
			if (chart.DataContainer.SeriesTemplate.View.GetType() != SeriesViewFactory.GetType(viewType))
				oldValues.Add(new ChangeSeriesViewOldValues(chart.DataContainer.SeriesTemplate, SeriesViewFactory.GetViewType(chart.DataContainer.SeriesTemplate.View), -1));
			try {
				for (int i = 0; i < chart.Series.Count; i++) {
					Series series = chart.Series[i];
					if (!series.IsAutoCreated && series.View.GetType() != SeriesViewFactory.GetType(viewType))
						series.ChangeView(viewType);
				}
				if (chart.DataContainer.SeriesTemplate.View.GetType() != SeriesViewFactory.GetType(viewType))
					chart.DataContainer.SeriesTemplate.ChangeView(viewType);
			}
			catch (Exception e) {
				XtraMessageBox.Show(e.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return null;
			}
			chart.Container.UnlockChangeService();
			chart.Container.Changed();
			return new HistoryItem(this, oldValues, null, parameter) { TargetObject = properties };
		}
		public override void UndoInternal(HistoryItem historyItem) {
			List<ChangeSeriesViewOldValues> oldValues = (List<ChangeSeriesViewOldValues>)historyItem.OldValue;
			chart.Container.LockChangeService();
			int panesCount = (int)historyItem.Parameter;
			foreach (ChangeSeriesViewOldValues oldValue in oldValues)
				oldValue.Series.ChangeView(oldValue.View);
			foreach (ChangeSeriesViewOldValues oldValue in oldValues)
				if (oldValue.PaneIndex >= 0)
					SetPaneIndex(oldValue.Series, oldValue.PaneIndex);
			chart.Container.UnlockChangeService();
			chart.Container.Changed();
			RestoreProperties(chart, historyItem.TargetObject);
		}
		public override void RedoInternal(HistoryItem historyItem) {
			ExecuteInternal(historyItem.Parameter);
		}
	}
}
