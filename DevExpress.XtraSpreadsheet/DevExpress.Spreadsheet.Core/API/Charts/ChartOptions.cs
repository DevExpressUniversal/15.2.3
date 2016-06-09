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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Spreadsheet.Drawings;
namespace DevExpress.Spreadsheet.Charts {
	public enum DisplayBlanksAs {
		Zero = DevExpress.XtraSpreadsheet.Model.DisplayBlanksAs.Zero,
		Span = DevExpress.XtraSpreadsheet.Model.DisplayBlanksAs.Span,
		Gap = DevExpress.XtraSpreadsheet.Model.DisplayBlanksAs.Gap
	}
	[Flags]
	public enum ChartProtection {
		None = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.None,
		ChartObject = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.ChartObject,
		Data = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.Data,
		Formatting = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.Formatting,
		Selection = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.Selection,
		UserInterface = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.UserInterface,
		All = DevExpress.XtraSpreadsheet.Model.ChartSpaceProtection.All
	}
	public interface ChartOptions {
		CultureInfo Culture { get; set; }
		DisplayBlanksAs DisplayBlanksAs { get; set; }
		bool PlotVisibleOnly { get; set; }
		ChartProtection Protection { get; set; }
		bool RoundedCorners { get; set; }
		bool ShowDataLabelsOverMax { get; set; }
		bool Use1904DateSystem { get; set; }
	}
	public interface ChartLineOptions : ShapeFormat {
		bool Visible { get; set; }
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	using DevExpress.Office.Model;
	#region NativeChartOptions
	partial class NativeChartOptions : NativeObjectBase, ChartOptions {
		#region Fields
		readonly Model.Chart modelChart;
		#endregion
		public NativeChartOptions(Model.Chart modelChart) 
			: base() {
			this.modelChart = modelChart;
		}
		#region ChartOptions Members
		public CultureInfo Culture {
			get {
				CheckValid();
				return modelChart.Culture;
			}
			set {
				CheckValid();
				modelChart.Culture = value;
			}
		}
		public DisplayBlanksAs DisplayBlanksAs {
			get {
				CheckValid();
				return (DisplayBlanksAs)modelChart.DispBlanksAs;
			}
			set {
				CheckValid();
				modelChart.DispBlanksAs = (Model.DisplayBlanksAs)value;
			}
		}
		public bool PlotVisibleOnly {
			get {
				CheckValid();
				return modelChart.PlotVisibleOnly;
			}
			set {
				CheckValid();
				modelChart.PlotVisibleOnly = value;
			}
		}
		public ChartProtection Protection {
			get {
				CheckValid();
				return (ChartProtection)modelChart.Protection;
			}
			set {
				CheckValid();
				modelChart.Protection = (Model.ChartSpaceProtection)value;
			}
		}
		public bool RoundedCorners {
			get {
				CheckValid();
				return modelChart.RoundedCorners;
			}
			set {
				CheckValid();
				modelChart.RoundedCorners = value;
			}
		}
		public bool ShowDataLabelsOverMax {
			get {
				CheckValid();
				return modelChart.ShowDataLabelsOverMax;
			}
			set {
				CheckValid();
				modelChart.ShowDataLabelsOverMax = value;
			}
		}
		public bool Use1904DateSystem {
			get {
				CheckValid();
				return modelChart.Date1904;
			}
			set {
				CheckValid();
				modelChart.Date1904 = value;
			}
		}
		#endregion
	}
	#endregion
	#region IChartLineAdapter
	public interface IChartLineAdapter {
		Model.ShapeProperties ShapeProperties { get; }
		bool Visible { get; set; }
	}
	#endregion
	#region NativeChartLineOptions
	partial class NativeChartLineOptions : NativeShapeFormat, ChartLineOptions {
		readonly IChartLineAdapter adapter;
		public NativeChartLineOptions(IChartLineAdapter adapter, NativeWorkbook nativeWorkbook) :
			base(adapter.ShapeProperties, nativeWorkbook) {
			this.adapter = adapter;
		}
		public bool Visible {
			get {
				CheckValid();
				return adapter.Visible;
			}
			set {
				CheckValid();
				adapter.Visible = value;
			}
		}
	}
	#endregion
	#region MajorGridlinesAdapter
	partial class MajorGridlinesAdapter : IChartLineAdapter {
		readonly Model.AxisBase axis;
		public MajorGridlinesAdapter(Model.AxisBase axis) {
			this.axis = axis;
		}
		public Model.ShapeProperties ShapeProperties { get { return axis.MajorGridlines; } }
		public bool Visible { get { return axis.ShowMajorGridlines; } set { axis.ShowMajorGridlines = value; } }
	}
	#endregion
	#region MinorGridlinesAdapter
	partial class MinorGridlinesAdapter : IChartLineAdapter {
		readonly Model.AxisBase axis;
		public MinorGridlinesAdapter(Model.AxisBase axis) {
			this.axis = axis;
		}
		public Model.ShapeProperties ShapeProperties { get { return axis.MinorGridlines; } }
		public bool Visible { get { return axis.ShowMinorGridlines; } set { axis.ShowMinorGridlines = value; } }
	}
	#endregion
	#region LeaderLinesAdapter
	partial class LeaderLinesAdapter : IChartLineAdapter {
		readonly Model.DataLabels labels;
		public LeaderLinesAdapter(Model.DataLabels labels) {
			this.labels = labels;
		}
		public Model.ShapeProperties ShapeProperties { get { return labels.LeaderLinesProperties; } }
		public bool Visible { get { return labels.ShowLeaderLines; } set { labels.ShowLeaderLines = value; } }
	}
	#endregion
	#region DropLinesAdapter
	partial class DropLinesAdapter : IChartLineAdapter {
		readonly Model.ISupportsDropLines modelView;
		public DropLinesAdapter(Model.ISupportsDropLines modelView) {
			this.modelView = modelView;
		}
		public Model.ShapeProperties ShapeProperties { get { return modelView.DropLinesProperties; } }
		public bool Visible { 
			get { return modelView.ShowDropLines; } 
			set { modelView.ShowDropLines = value; } 
		}
	}
	#endregion
	#region HiLowLinesAdapter
	partial class HiLowLinesAdapter : IChartLineAdapter {
		readonly Model.ISupportsHiLowLines modelView;
		public HiLowLinesAdapter(Model.ISupportsHiLowLines modelView) {
			this.modelView = modelView;
		}
		public Model.ShapeProperties ShapeProperties { get { return modelView.HiLowLinesProperties; } }
		public bool Visible {
			get { return modelView.ShowHiLowLines; }
			set { modelView.ShowHiLowLines = value; }
		}
	}
	#endregion
	#region NativeSeriesLinesOptions
	partial class NativeSeriesLinesOptions : NativeObjectBase, ChartLineOptions {
		#region Fields
		readonly Model.SeriesLinesCollection seriesLines;
		readonly NativeWorkbook nativeWorkbook;
		NativeShapeFormat nativeShapeFormat;
		#endregion
		public NativeSeriesLinesOptions(Model.SeriesLinesCollection seriesLines, NativeWorkbook nativeWorkbook)
			: base() {
			this.seriesLines = seriesLines;
			this.nativeWorkbook = nativeWorkbook;
			SubscribeEvents();
		}
		Model.DocumentModel DocumentModel { get { return seriesLines.Parent.DocumentModel; } }
		#region ChartLineOptions Members
		public bool Visible {
			get {
				CheckValid();
				return seriesLines.Count > 0;
			}
			set {
				if (Visible == value)
					return;
				if (value) {
					Model.ShapeProperties seriesLineProperties = new Model.ShapeProperties(DocumentModel);
					seriesLineProperties.Parent = seriesLines.Parent;
					seriesLines.Add(seriesLineProperties);
				}
				else
					seriesLines.Clear();
			}
		}
		#endregion
		#region ShapeFormat Members
		public ShapeFill Fill {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat != null ? nativeShapeFormat.Fill : null;
			}
		}
		public ShapeOutline Outline {
			get {
				CheckValid();
				CheckShapeFormat();
				return nativeShapeFormat != null ? nativeShapeFormat.Outline : null;
			}
		}
		public void ResetToMatchStyle() {
			CheckValid();
			CheckShapeFormat();
			if (nativeShapeFormat != null)
				nativeShapeFormat.ResetToMatchStyle();
		}
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (value)
				SubscribeEvents();
			else
				UnsubscribeEvents();
			if (nativeShapeFormat != null)
				nativeShapeFormat.IsValid = value;
		}
		void CheckShapeFormat() {
			if (nativeShapeFormat == null && seriesLines.Count > 0)
				nativeShapeFormat = new NativeShapeFormat(seriesLines[0], nativeWorkbook);
		}
		void ReleaseShapeFormat() {
			if (nativeShapeFormat != null) {
				nativeShapeFormat.IsValid = false;
				nativeShapeFormat = null;
			}
		}
		void SubscribeEvents() {
			seriesLines.OnRemoveAt += seriesLines_OnRemoveAt;
			seriesLines.OnClear += seriesLines_OnClear;
		}
		void UnsubscribeEvents() {
			seriesLines.OnRemoveAt -= seriesLines_OnRemoveAt;
			seriesLines.OnClear -= seriesLines_OnClear;
		}
		void seriesLines_OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs e) {
			if (e.Index == 0)
				ReleaseShapeFormat();
		}
		void seriesLines_OnClear(object sender) {
			ReleaseShapeFormat();
		}
	}
	#endregion
}
