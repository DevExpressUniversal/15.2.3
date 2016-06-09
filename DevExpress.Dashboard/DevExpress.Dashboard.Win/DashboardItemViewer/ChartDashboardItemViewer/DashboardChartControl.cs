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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraCharts;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardCommon.Viewer;
using System;
using System.Collections;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardChartControl : ChartControl, IDashboardChartControl {
		bool IDashboardChartControl.BorderVisible { get { return DefaultBooleanUtils.ToBoolean(BorderOptions.Visibility, true); } set { BorderOptions.Visibility = DefaultBooleanUtils.ToDefaultBoolean(value); } }
		IList IDashboardChartControl.Series { get { return Series; } }
		IDashboardChartLegend IDashboardChartControl.Legend { get { return new DashboardChartLegend(Legend); } }
		int updateLockCounter;
		event EventHandler<CustomDrawSeriesPointEventArgs> _CustomDrawSeriesPoint;
		event EventHandler<CustomDrawAxisLabelEventArgs> _CustomDrawAxisLabel;
		event EventHandler<CustomDrawCrosshairEventArgs> _CustomDrawCrosshair;
		event EventHandler<CustomDrawSeriesEventArgs> _CustomDrawSeries;
		event EventHandler<CustomDrawSeriesPointEventArgs> IDashboardChartControl.CustomDrawSeriesPoint {
			add { _CustomDrawSeriesPoint = (EventHandler<CustomDrawSeriesPointEventArgs>)Delegate.Combine(_CustomDrawSeriesPoint, value); }
			remove { _CustomDrawSeriesPoint = (EventHandler<CustomDrawSeriesPointEventArgs>)Delegate.Remove(_CustomDrawSeriesPoint, value); }
		}
		event EventHandler<CustomDrawAxisLabelEventArgs> IDashboardChartControl.CustomDrawAxisLabel {
			add { _CustomDrawAxisLabel = (EventHandler<CustomDrawAxisLabelEventArgs>)Delegate.Combine(_CustomDrawAxisLabel, value); }
			remove { _CustomDrawAxisLabel = (EventHandler<CustomDrawAxisLabelEventArgs>)Delegate.Remove(_CustomDrawAxisLabel, value); }
		}
		event EventHandler<CustomDrawCrosshairEventArgs> IDashboardChartControl.CustomDrawCrosshair {
			add { _CustomDrawCrosshair = (EventHandler<CustomDrawCrosshairEventArgs>)Delegate.Combine(_CustomDrawCrosshair, value); }
			remove { _CustomDrawCrosshair = (EventHandler<CustomDrawCrosshairEventArgs>)Delegate.Remove(_CustomDrawCrosshair, value); }
		}
		event EventHandler<CustomDrawSeriesEventArgs> IDashboardChartControl.CustomDrawSeries {
			add { _CustomDrawSeries = (EventHandler<CustomDrawSeriesEventArgs>)Delegate.Combine(_CustomDrawSeries, value); }
			remove { _CustomDrawSeries = (EventHandler<CustomDrawSeriesEventArgs>)Delegate.Remove(_CustomDrawSeries, value); }
		}
		public event EventHandler<PaintEventArgs> Painted;
		public DashboardChartControl() {
			CustomDrawAxisLabel += (sender, e) => { }; 
			CrosshairOptions.ShowArgumentLine = false;
			RefreshDataOnRepaint = false;
			AllowGesture = false;
			SelectionMode = ElementSelectionMode.None;
		}
		protected override void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e) {
			if(_CustomDrawSeriesPoint != null)
				_CustomDrawSeriesPoint(this, e);
			base.OnCustomDrawSeriesPoint(e);
		}
		protected override void OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e) {
			if(_CustomDrawAxisLabel != null)
				_CustomDrawAxisLabel(this, e);
			base.OnCustomDrawAxisLabel(e);
		}
		protected override void OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) {
			if(_CustomDrawCrosshair != null)
				_CustomDrawCrosshair(this, e);
			base.OnCustomDrawCrosshair(e);
		}
		protected override void OnCustomDrawSeries(CustomDrawSeriesEventArgs e) {
			if(_CustomDrawSeries != null)
				_CustomDrawSeries(this, e);
			base.OnCustomDrawSeries(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			try {
				base.OnPaint(e);				
				if(Painted != null)
					Painted(this, e);
			}
			catch {
				UserLookAndFeel lookAndFeel = LookAndFeel;
				Rectangle bounds = Bounds;
				Color color = lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin ?
					CommonSkins.GetSkin(lookAndFeel).Colors[CommonColors.Control] : CommonColors.GetSystemColor(CommonColors.Control);
				using(Brush brush = new SolidBrush(color))
					e.Graphics.FillRectangle(brush, bounds);
				WarningRenderer.MessageRenderer(e, bounds, DashboardWinLocalizer.GetString(DashboardWinStringId.ChartCannotRenderData), lookAndFeel);
			}
		}
		IRangeControlClientExtension IDashboardChartControl.RangeControlClient { get { return this; } }
		bool IDashboardChartControl.IsInUpdate {
			get { return updateLockCounter > 0; }
		}
		void IDashboardChartControl.BeginUpdate() {
			if(updateLockCounter == 0)
				BeginInit();
			updateLockCounter++;
		}
		void IDashboardChartControl.EndUpdate() {
			updateLockCounter--;
			if(updateLockCounter == 0)
				EndInit();
		}
		bool IDashboardChartControl.SeriesPointSelectionEnabled {
			get { return SeriesSelectionMode == SeriesSelectionMode.Point; }
			set { SeriesSelectionMode = value ? SeriesSelectionMode.Point : SeriesSelectionMode.Series; }
		}
		Palette IDashboardChartControl.Palette {
			get { return PaletteRepository[PaletteName]; }
		}
	}
}
