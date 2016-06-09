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
using System.Drawing;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.Utils.Controls;
using DevExpress.XtraCharts;
using DevExpress.XtraPrinting;
using System.Collections;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardExport {
	public class ExportChartControl : IDisposable, IDashboardChartControl {
		readonly ChartContainer chartContainer = new ChartContainer();
		int updateLockCounter;
		IRangeControlClientExtension IDashboardChartControl.RangeControlClient { get { return chartContainer.RangeControlClient ; } }
		bool IDashboardChartControl.BorderVisible { get { return DefaultBooleanUtils.ToBoolean(chartContainer.Border.Visibility, true); } set { chartContainer.Border.Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); } }
		object IDashboardChartControl.DataSource { get { return chartContainer.DataSource; } set { chartContainer.DataSource = value; } }
		Diagram IDashboardChartControl.Diagram { get { return chartContainer.Diagram; } set { chartContainer.Diagram = value; } }
		IDashboardChartLegend IDashboardChartControl.Legend { get { return new DashboardChartLegend(chartContainer.Legend); } }
		IList IDashboardChartControl.Series { get { return chartContainer.Series; } }
		Size IDashboardChartControl.Size { get { return chartContainer.Size; } set { chartContainer.Size = value; } }
		Palette IDashboardChartControl.Palette { get { return chartContainer.Palette; } }
		public IPrintable Printable { get { return chartContainer.Printable; } }
		public bool SeriesPointSelectionEnabled { 
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
		public ExportChartControl() {
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
			if (disposing)
				chartContainer.Dispose();
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
	}
}
