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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region IXlsChartBuilder
	public interface IXlsChartBuilder {
		void Execute(XlsContentBuilder contentBuilder);
	}
	#endregion
	#region XlsChartEmptyBuilder
	public class XlsChartEmptyBuilder : IXlsChartBuilder {
		static IXlsChartBuilder instance = new XlsChartEmptyBuilder();
		public static IXlsChartBuilder Instance { get { return instance; } }
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
		}
		#endregion
	}
	#endregion
	#region XlsChartRootBuilder
	public class XlsChartRootBuilder : IXlsChartBuilder, IXlsChartFrameContainer, IXlsChartExtPropertyVisitor, IXlsChartTextContainer, IXlsChartDefaultTextContainer, IXlsChartTextFormatContainer {
		[Flags]
		enum ApplyFlags {
			None = 0x0000,
			RightAngleAxes = 0x0001,
			Perspective = 0x0002,
			RotationX = 0x0004,
			RotationY = 0x0008,
			HeightPercent = 0x0010,
			ShowDataLabelsOverMax = 0x0020,
			BackWallThickness = 0x0040,
			FloorThickness = 0x0080,
			DispBlankAs = 0x0100
		}
		#region Fields
		XlsChartFrame frame;
		ApplyFlags flags = ApplyFlags.None;
		List<XlsChartTextBuilder> titles = new List<XlsChartTextBuilder>();
		XlsChartLayout12A plotAreaLayout = new XlsChartLayout12A();
		XlsChartTextFormat textFormat = null;
		readonly XlsChartDefaultTextCollection defaultText = new XlsChartDefaultTextCollection();
		int defaultTextId;
		#endregion
		#region Properties
		public double Left { get; set; }
		public double Top { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public XlsChartFrame Frame { get { return frame; } }
		public bool RightAngleAxes { get; set; }
		public int Perspective { get; set; }
		public int RotationX { get; set; }
		public int RotationY { get; set; }
		public int HeightPercent { get; set; }
		public bool ShowDataLabelsOverMax { get; set; }
		public int BackWallThickness { get; set; }
		public int FloorThickness { get; set; }
		public DisplayBlanksAs DispBlankAs { get; set; }
		public List<XlsChartTextBuilder> Titles { get { return titles; } }
		public XlsChartLayout12A PlotAreaLayout { get { return plotAreaLayout; } }
		public bool ManualPlotArea { get; set; }
		public bool AlwaysAutoPlotArea { get; set; }
		public XlsChartTextFormat TextFormat { get { return textFormat; } }
		public XlsChartDefaultTextCollection DefaultText { get { return defaultText; } }
		public int DefaultTextId {
			get { return defaultTextId; }
			set {
				if (value != 0)
					ValueChecker.CheckValue(value, 2, 3, "DefaultTextId");
				defaultTextId = value;
			}
		}
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			Chart chart = contentBuilder.CurrentChart;
			if (chart == null)
				return;
			if (flags != ApplyFlags.None) {
				SetupView3D(chart);
				SetupWallsAndFloor(chart);
				SetupOptions(chart);
			}
			SetupChartSpace(chart.ShapeProperties);
			SetupChartTitle(contentBuilder, chart);
			SetupDataLabels(contentBuilder);
			PlotAreaLayout.SetupLayout(chart.PlotArea.Layout);
			if (TextFormat != null && TextFormat.IsValidCheckSum(contentBuilder, null)) {
				TextFormat.SetupTextProperties(chart.TextProperties);
				return;
			}
		}
		#endregion
		void SetupView3D(Chart chart) {
			View3DOptions view3d = chart.View3D;
			view3d.BeginUpdate();
			try {
				if ((flags & ApplyFlags.RightAngleAxes) != 0)
					view3d.RightAngleAxes = !RightAngleAxes;
				if ((flags & ApplyFlags.Perspective) != 0) {
					view3d.Perspective = Perspective;
					if (chart.Views[0].ViewType == ChartViewType.Pie3D)
						view3d.RightAngleAxes = false;
				}
				if ((flags & ApplyFlags.RotationX) != 0)
					view3d.XRotation = RotationX;
				if ((flags & ApplyFlags.RotationY) != 0)
					view3d.YRotation = RotationY;
				if ((flags & ApplyFlags.HeightPercent) != 0) {
					view3d.HeightPercent = HeightPercent;
					view3d.AutoHeight = true;
				}
			}
			finally {
				view3d.EndUpdate();
			}
		}
		void SetupWallsAndFloor(Chart chart) {
			if ((flags & ApplyFlags.BackWallThickness) != 0) {
				chart.BackWall.Thickness = BackWallThickness;
				chart.SideWall.Thickness = BackWallThickness;
			}
			if ((flags & ApplyFlags.FloorThickness) != 0)
				chart.Floor.Thickness = FloorThickness;
		}
		void SetupOptions(Chart chart) {
			if ((flags & ApplyFlags.ShowDataLabelsOverMax) != 0)
				chart.ShowDataLabelsOverMax = ShowDataLabelsOverMax;
			if ((flags & ApplyFlags.DispBlankAs) != 0)
				chart.DispBlanksAs = DispBlankAs;
		}
		void SetupChartTitle(XlsContentBuilder contentBuilder, Chart chart) {
			TitleOptions title = chart.Title;
			foreach (XlsChartTextBuilder text in Titles) {
				if (text.ObjectLink.IsChartTitle) {
					if (text.Deleted)
						chart.AutoTitleDeleted = true;
					else
						text.SetupTitle(contentBuilder, title);
					break;
				}
			}
		}
		void SetupChartSpace(ShapeProperties shapeProperties) {
			if (frame != null)
				frame.SetupShapeProperties(shapeProperties);
		}
		void SetupDataLabels(XlsContentBuilder contentBuilder) {
			foreach (XlsChartSeriesBuilder seriesBuilder in contentBuilder.SeriesFormats) {
				if (seriesBuilder.ViewIndex == -1)
					continue;
				SeriesWithDataLabelsAndPoints series = seriesBuilder.ChartSeries as SeriesWithDataLabelsAndPoints;
				if (series != null) {
					SetupDataLabelsFromDataFormat(contentBuilder, series, seriesBuilder.SeriesIndex);
					SetupDataLabelsFromText(contentBuilder, series, seriesBuilder.SeriesIndex);
				}
			}
		}
		void SetupDataLabelsFromDataFormat(XlsContentBuilder contentBuilder, SeriesWithDataLabelsAndPoints series, int index) {
			if (series == null)
				return;
			List<XlsChartSeriesBuilder> seriesFormats = contentBuilder.SeriesFormats;
			foreach (XlsChartSeriesBuilder seriesFormat in seriesFormats) {
				foreach (XlsChartDataFormat dataFormat in seriesFormat.DataFormats)
					if (dataFormat.SeriesIndex == index)
						dataFormat.SetupSeriesDataLabels(series);
			}
		}
		void SetupDataLabelsFromText(XlsContentBuilder contentBuilder, SeriesWithDataLabelsAndPoints series, int index) {
			if (series == null)
				return;
			foreach (XlsChartTextBuilder text in Titles) {
				XlsChartTextObjectLink objectLink = text.ObjectLink;
				if (objectLink.IsSeriesOrDataPoint && objectLink.SeriesIndex == index)
					text.SetupSeriesDataLabels(contentBuilder, series);
			}
		}
		#region IXlsChartFrameContainer Members
		void IXlsChartFrameContainer.Add(XlsChartFrame frame) {
			this.frame = frame;
		}
		#endregion
		#region IXlsChartExtPropertyVisitor Members
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMax item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropScaleMin item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropLogBase item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStyle item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropThemeOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropColorMapOverride item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropNoMultiLvlLbl item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelSkip item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickMarkSkip item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTickLabelPos item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBaseTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFormatCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMajorTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropMinorTimeUnit item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShowDLblsOverMax item) {
			ShowDataLabelsOverMax = item.Value;
			flags |= ApplyFlags.ShowDataLabelsOverMax;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropBackWallThickness item) {
			BackWallThickness = item.Value;
			flags |= ApplyFlags.BackWallThickness;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropFloorThickness item) {
			FloorThickness = item.Value;
			flags |= ApplyFlags.FloorThickness;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropDispBlankAs item) {
			if (item.Value == 0x0067) {
				DispBlankAs = DisplayBlanksAs.Gap;
				flags |= ApplyFlags.DispBlankAs;
			}
			if (item.Value == 0x0069) {
				DispBlankAs = DisplayBlanksAs.Span;
				flags |= ApplyFlags.DispBlankAs;
			}
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropStartSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropEndSurface item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropShapeProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropTextProps item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropOverlay item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPieCombo item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRightAngAxOff item) {
			RightAngleAxes = item.Value;
			flags |= ApplyFlags.RightAngleAxes;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropPerspective item) {
			Perspective = item.Value;
			flags |= ApplyFlags.Perspective;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationX item) {
			RotationX = item.Value;
			flags |= ApplyFlags.RotationX;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropRotationY item) {
			RotationY = item.Value;
			flags |= ApplyFlags.RotationY;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropHeightPercent item) {
			HeightPercent = (int)item.Value;
			flags |= ApplyFlags.HeightPercent;
		}
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropCultureCode item) { }
		void IXlsChartExtPropertyVisitor.Visit(XlsChartExtPropSymbol item) { }
		#endregion
		#region IXlsChartTextContainer Members
		void IXlsChartTextContainer.Add(XlsChartTextBuilder item) {
			if (defaultTextId == 2 || defaultTextId == 3)
				this.defaultText.Add(new XlsChartDefaultText(defaultTextId, item));
			else
				this.titles.Add(item);
			defaultTextId = 0;
		}
		#endregion
		#region IXlsChartTextFormatContainer Members
		void IXlsChartTextFormatContainer.Add(XlsChartTextFormat properties) {
			textFormat = properties;
		}
		#endregion
	}
	#endregion
}
