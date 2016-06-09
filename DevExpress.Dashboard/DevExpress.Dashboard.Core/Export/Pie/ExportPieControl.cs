#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Utils.Controls;
using DevExpress.XtraCharts;
using DevExpress.XtraPrinting;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardExport {
	public class ExportPieControl : IDisposable, IDashboardChartControl, ISimpleDiagramPanel {
		readonly ChartContainer chartContainer = new ChartContainer();
		readonly List<PieSeries> series = new List<PieSeries>();
		SimpleDiagram diagram;
		Size size;
		int updateLockCounter;
		Rectangle DefaultBounds { get { return new Rectangle(new Point(0, 0), series[0].Bounds.Size); } }
		IRangeControlClientExtension IDashboardChartControl.RangeControlClient { get { return null; } }
		bool IDashboardChartControl.BorderVisible { get { return DefaultBooleanUtils.ToBoolean(chartContainer.Border.Visibility, true); } set { chartContainer.Border.Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); } }
		object IDashboardChartControl.DataSource { get { return chartContainer.DataSource; } set { chartContainer.DataSource = value; } }
		Palette IDashboardChartControl.Palette { get { return chartContainer.Palette; } }
		Diagram IDashboardChartControl.Diagram {
			get { return diagram; }
			set {
				diagram = (SimpleDiagram)value;
				diagram.CustomPanel = this;
			}
		}
		IDashboardChartLegend IDashboardChartControl.Legend { get { return new DashboardChartLegend(chartContainer.Legend); } }
		IList IDashboardChartControl.Series { get { return series; } }
		Size IDashboardChartControl.Size { get { return size; } set { size = value; } }
		bool IDashboardChartControl.SeriesPointSelectionEnabled {
			get { return chartContainer.SeriesSelectionMode != SeriesSelectionMode.Series; }
			set { chartContainer.SeriesSelectionMode = value ? SeriesSelectionMode.Point : SeriesSelectionMode.Series; } 
		}
		public IChartContainer ChartContainer { get { return chartContainer; } }
		event EventHandler<CustomDrawSeriesPointEventArgs> CustomDrawSeriesPoint;
		event EventHandler<CustomDrawAxisLabelEventArgs> CustomDrawAxisLabel;
		event EventHandler<CustomDrawCrosshairEventArgs> CustomDrawCrosshair;
		event EventHandler<CustomDrawSeriesEventArgs> CustomDrawSeries;
		event EventHandler<CustomDrawSeriesPointEventArgs> IDashboardChartControl.CustomDrawSeriesPoint {
			add { CustomDrawSeriesPoint = (EventHandler<CustomDrawSeriesPointEventArgs>)Delegate.Combine(CustomDrawSeriesPoint, value); }
			remove { CustomDrawSeriesPoint = (EventHandler<CustomDrawSeriesPointEventArgs>)Delegate.Remove(CustomDrawSeriesPoint, value); }
		}
		event EventHandler<CustomDrawAxisLabelEventArgs> IDashboardChartControl.CustomDrawAxisLabel {
			add { CustomDrawAxisLabel = (EventHandler<CustomDrawAxisLabelEventArgs>)Delegate.Combine(CustomDrawAxisLabel, value); }
			remove { CustomDrawAxisLabel = (EventHandler<CustomDrawAxisLabelEventArgs>)Delegate.Remove(CustomDrawAxisLabel, value); }
		}
		event EventHandler<CustomDrawCrosshairEventArgs> IDashboardChartControl.CustomDrawCrosshair {
			add { CustomDrawCrosshair = (EventHandler<CustomDrawCrosshairEventArgs>)Delegate.Combine(CustomDrawCrosshair, value); }
			remove { CustomDrawCrosshair = (EventHandler<CustomDrawCrosshairEventArgs>)Delegate.Remove(CustomDrawCrosshair, value); }
		}
		event EventHandler<CustomDrawSeriesEventArgs> IDashboardChartControl.CustomDrawSeries {
			add { CustomDrawSeries = (EventHandler<CustomDrawSeriesEventArgs>)Delegate.Combine(CustomDrawSeries, value); }
			remove { CustomDrawSeries = (EventHandler<CustomDrawSeriesEventArgs>)Delegate.Remove(CustomDrawSeries, value); }
		}
		public ExportPieControl() {
			chartContainer.Padding.Left = 0;
			chartContainer.Padding.Top = 0;
			chartContainer.Padding.Right = 0;
			chartContainer.Padding.Bottom = 0;
			chartContainer.CustomDrawSeriesPoint += (sender, e) => {
				if(CustomDrawSeriesPoint != null)
					CustomDrawSeriesPoint(this, e);
			};
			chartContainer.CustomDrawAxisLabel += (sender, e) => {
				if(CustomDrawAxisLabel != null)
					CustomDrawAxisLabel(sender, e);
			};
			chartContainer.CustomDrawCrosshair += (sender, e) => {
				if(CustomDrawCrosshair != null)
					CustomDrawCrosshair(sender, e);
			};
			chartContainer.CustomDrawSeries += (sender, e) => {
				if(CustomDrawSeries != null)
					CustomDrawSeries(sender, e);
			};
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				chartContainer.Dispose();
				if(diagram != null)
					diagram.Dispose();
				foreach(PieSeries ser in series)
					ser.Dispose();
				series.Clear();
			}
		}
		IList<Rectangle> ISimpleDiagramPanel.Arrange(IList<Series> seriesList, Rectangle diagramBounds) {
			return new Rectangle[] { DefaultBounds };
		}
		bool IDashboardChartControl.IsInUpdate {
			get { return updateLockCounter > 0; }
		}
		void IDashboardChartControl.BeginUpdate() {
			if(updateLockCounter == 0)
				chartContainer.BeginUpdate();
			updateLockCounter++;
		}
		void IDashboardChartControl.EndUpdate() {
			updateLockCounter--;
			if(updateLockCounter == 0)
				chartContainer.EndUpdate();
		}
		void IDashboardChartControl.Invalidate() {
			chartContainer.Invalidate();
		}
		public void Print(IBrickGraphics graph, IList selectedValues, Size actualPrintSize) {
			ExportHelper.DrawEmptyBrick(graph, actualPrintSize);
			foreach(PieSeries ser in series) {
				chartContainer.BeginUpdate();
				try {
					chartContainer.Series.Clear();
					chartContainer.Series.Add(ser);
					chartContainer.Diagram = diagram;
				}
				finally {
					chartContainer.EndUpdate();
				}
				Size seriesBounds = ser.Bounds.Size;
				Image image = chartContainer.GetImage(seriesBounds);
				if(DataUtils.ListContains(selectedValues, ((IValuesProvider)ser).SelectionValues))
					image = ExportContentScrollableControl.BlackoutImage(image);
				ImageBrick brick = new ImageBrick();
				brick.Image = image;
				brick.Sides = BorderSide.None;
				graph.DrawBrick(brick, new Rectangle(ser.Bounds.Location, seriesBounds));
			}
		}
	}
}
