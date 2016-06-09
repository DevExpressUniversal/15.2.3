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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using Box = DevExpress.XtraRichEdit.Layout.Box;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.Office.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Native {
	public class ChartBox : InlinePictureBox {
		const int Margin = 20;
		const int InnerMargin = 20; 
		List<string> arguments;
		List<string> values;
		readonly HotZoneCollection hotZones = new HotZoneCollection();
		public HotZoneCollection HotZones {
			get {
				if (hotZones.Count == 0)
					InitializeHotZones();
				return hotZones;
			}
		}
		public bool IsEmpty {
			get {
				if (arguments != null && arguments.Count > 0)
					return false;
				if (values != null && values.Count > 0)
					return false;
				return true;
			}
		}
		#region Calculations
		void InitializeHotZones() {
			if (ActualSizeBounds.IsEmpty)
				return;
			float radius = CalculateRadius(ActualSizeBounds);
			PointF[] hotZoneCenters = GetHotZoneCenters(ActualSizeBounds, radius);
			hotZones.Add(new DropValuesHotZone(hotZoneCenters[0], radius));
			hotZones.Add(new DropArgumentsHotZone(hotZoneCenters[1], radius));
		}
		float CalculateRadius(Rectangle bounds) {
			int maxVertDiameter = bounds.Height - 2 * Margin;
			const float HotZonesTotalWidthToChartWidthRatio = 4.0f / 7.0f;
			int maxHorzDiameter = (int)(bounds.Width * HotZonesTotalWidthToChartWidthRatio - InnerMargin) / 2;
			return Math.Min(maxVertDiameter, maxHorzDiameter) / 2;
		}
		PointF[] GetHotZoneCenters(Rectangle bounds, float radius) {
			float distanceToHorzBorder = bounds.Y + bounds.Height / 2;
			float distanceToVertBorder = bounds.X + bounds.Width / 2 - InnerMargin / 2 - radius;
			return new PointF[] { new PointF { X = distanceToVertBorder, Y = distanceToHorzBorder },
				new PointF { X = distanceToVertBorder + 2 * radius + InnerMargin, Y = distanceToHorzBorder } };
		}
		#endregion
		public override Box CreateBox() {
			return new ChartBox();
		}
		public OfficeImage GetImageForPrinting(PieceTable pieceTable) {
			return base.GetImageCore(GetRun(pieceTable), true);
		}
		protected override OfficeImage GetImageCore(TextRunBase run, bool readOnly) {
			ChartRun chartRun = (ChartRun)run;
			bool showChartInfoPanel = ((SnapFieldOptions)run.DocumentModel.FieldOptions).ShowChartInfoPanel;
			if (readOnly || !showChartInfoPanel || run.DocumentModel.ModelForExport)
				return OfficeImage.CreateImage(chartRun.Image.NativeImage);
			return GetDecoratedImage(chartRun);
		}
		OfficeImage GetDecoratedImage(ChartRun chartRun) {
			Bitmap maskedImage = new Bitmap(chartRun.Image.NativeImage);
			using (ISNChartContainer chartContainer = GetChartContainer(chartRun)) {
				if (chartContainer != null) {
					DocumentLayoutUnitConverter unitConverter = chartRun.DocumentModel.LayoutUnitConverter;
					SNChart chart = chartContainer.Chart;
					values = chart.GetUniqueValues();
					arguments = chart.GetUniqueArguments();
					using (Graphics graphics = Graphics.FromImage(maskedImage)) {
						graphics.PageUnit = (GraphicsUnit)unitConverter.GraphicsPageUnit;
						graphics.PageScale = unitConverter.GraphicsPageScale;
						DrawInfoPanel(graphics, chart);
					}
				}
			}
			return OfficeImage.CreateImage(maskedImage);
		}
		void DrawInfoPanel(Graphics graphics, SNChart chart) {
			using (ChartBoxPainter painter = new ChartBoxPainter()) {
				using (GDIPlusPreciseTextMeasurer textMeasurer = new GDIPlusPreciseTextMeasurer(graphics, painter.BoldFont)) {
					var infoPanelViewInfo = ChartBoxInfoPanelViewInfo.Create(Bounds, values, arguments, textMeasurer);
					if (infoPanelViewInfo != null && infoPanelViewInfo.IsVisible)
						painter.DrawInfoPanel(graphics, infoPanelViewInfo);
				}
			}
		}
		protected internal override void ExportHotZones(Painter painter) {
			if (!IsEmpty)
				return;
			using (ChartBoxHotZonePainter hotZonePainter = new ChartBoxHotZonePainter(painter)) {
				foreach (IRoundHotZone hotZone in HotZones) {
					hotZone.Accept(hotZonePainter);
				}
			}
		}
		ISNChartContainer GetChartContainer(ChartRun chartRun) {
			Field field = chartRun.PieceTable.FindNonTemplateFieldByRunIndex(this.StartPos.RunIndex);
			if (field == null)
				return null;
			SNChartField chartField = new SnapFieldCalculatorService().ParseField(chartRun.PieceTable, field) as SNChartField;
			if (chartField == null)
				return null;
			ISNChartContainer chartContainer = chartField.GetChartContainer((SnapDocumentModel)chartRun.DocumentModel, new SnapFieldInfo(chartRun.PieceTable, field));
			return chartContainer;
		}
	}
}
