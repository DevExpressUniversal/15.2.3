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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardChartLegend : IDashboardChartLegend {
		readonly Legend legend;
		public LegendAlignmentVertical AlignmentVertical { get { return legend.AlignmentVertical; } set { legend.AlignmentVertical = value; } }
		public LegendAlignmentHorizontal AlignmentHorizontal { get { return legend.AlignmentHorizontal; } set { legend.AlignmentHorizontal = value; } }
		public DefaultBoolean Visibility { get { return legend.Visibility; } set { legend.Visibility = value; } }
		public LegendDirection Direction { get { return legend.Direction; } set { legend.Direction = value; } }
		public DefaultBoolean BorderVisibility { get { return legend.Border.Visibility; } set { legend.Border.Visibility = value; } }
		public DashboardChartLegend(Legend legend) {
			this.legend = legend;
		}
	}
	public interface IDashboardChartLegend {
		LegendAlignmentVertical AlignmentVertical { get; set; }
		LegendAlignmentHorizontal AlignmentHorizontal { get; set; }
		DefaultBoolean Visibility { get; set; }
		LegendDirection Direction { get; set; }
		DefaultBoolean BorderVisibility { get; set; }
	}
	public interface IDashboardChartControl {
		IRangeControlClientExtension RangeControlClient { get; }
		bool BorderVisible { get; set; }
		object DataSource { get; set; }
		Diagram Diagram { get; set; }
		IDashboardChartLegend Legend { get; }
		IList Series { get; }
		Size Size { get; set; }
		bool SeriesPointSelectionEnabled { get; set; }
		bool IsInUpdate { get; }
		Palette Palette { get; }
		event EventHandler<CustomDrawSeriesPointEventArgs> CustomDrawSeriesPoint;
		event EventHandler<CustomDrawAxisLabelEventArgs> CustomDrawAxisLabel;
		event EventHandler<CustomDrawCrosshairEventArgs> CustomDrawCrosshair;
		event EventHandler<CustomDrawSeriesEventArgs> CustomDrawSeries;
		void BeginUpdate();
		void EndUpdate();
		void Invalidate();
	}
}
