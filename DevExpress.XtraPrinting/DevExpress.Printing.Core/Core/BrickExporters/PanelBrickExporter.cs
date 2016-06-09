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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using System;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class PanelBrickExporter : VisualBrickExporter {
		PanelBrick PanelBrick { get { return Brick as PanelBrick; } }
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			PanelBrick panelBrick = PanelBrick;
			foreach (Brick brick in panelBrick.Bricks) {
				RectangleF brickRect = brick.GetViewRectangle();
				brickRect.Offset(clientRect.X, clientRect.Y);
				if(gr.ClipBounds.IsEmpty || gr.ClipBounds.IntersectsWith(brickRect))
					GetExporter(gr, brick).Draw(gr, brickRect, clientRect);
			}
			SetBrick(panelBrick);
		}
		protected override RectangleF GetClientRect(RectangleF rect) {
			return rect;
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			exportProvider.SetContent(string.Format(DevExpress.XtraPrinting.Export.Rtf.RtfTags.FontSize, 1));
		}
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			exportProvider.SetNavigationUrl(PanelBrick);
			exportProvider.RaiseHtmlItemCreated(PanelBrick);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			BrickViewData[] data = GetViewData(rect, clipRect, exportContext);
			if(!(exportContext is RtfExportContext) && data.Length > 0 && !string.IsNullOrEmpty(PanelBrick.AnchorName))
				data[0].TableCell = new AnchorCell(data[0].TableCell, PanelBrick.AnchorName);
			return data;
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
		}
		protected BrickViewData[] GetViewData(RectangleF boundsF, RectangleF clipBoundsF, ExportContext exportContext) {
			if(!exportContext.CanPublish(PanelBrick))
				return new BrickViewData[0];
			List<BrickViewData> dataContainer = new List<BrickViewData>();
			if(exportContext.RawDataMode) {
				ProcessViewData(boundsF, clipBoundsF, exportContext, (exportData, brickRect) => {
					dataContainer.AddRange(exportData);
				});
			} else {
				Rectangle bounds = GraphicsUnitConverter.Round(boundsF);
				Rectangle clipBounds = GraphicsUnitConverter.Round(clipBoundsF);
				RectangleDivider divider = new RectangleDivider(Rectangle.Intersect(bounds, clipBounds));
				ProcessViewData(boundsF, clipBoundsF, exportContext, (exportData, brickRect) => {
					dataContainer.AddRange(exportData);
					divider.AddInnerArea(GraphicsUnitConverter.Round(brickRect), false);
				});
				if(exportContext is RtfPageExportContext) {
					GetRtfViewData(exportContext, bounds, dataContainer);
				} else {
					GetUsualViewData(exportContext, bounds, divider, dataContainer);
				}
			}			
			return dataContainer.ToArray();
		}
		void GetRtfViewData(ExportContext exportContext, Rectangle bounds, List<BrickViewData> dataContainer) {
			BrickViewData outerPanel = GetOuterPanel(exportContext, bounds);
			if(outerPanel != null && !HasBrickWithTheSameBounds(dataContainer, bounds))
				dataContainer.Insert(0, outerPanel);
		}
		static bool HasBrickWithTheSameBounds(List<BrickViewData> dataContainer, Rectangle bounds) {
			foreach(BrickViewData data in dataContainer) {
				if(data.OriginalBounds.Equals(bounds))
					return true;
			}
			return false;
		}
		void GetUsualViewData(ExportContext exportContext, Rectangle bounds, RectangleDivider divider, List<BrickViewData> dataContainer) {
			List<Rectangle> emptyAreas = GenerateEmptyAreas(divider, exportContext.AllowEmptyAreas);
			if(emptyAreas.Count > 0) {
				foreach(Rectangle area in emptyAreas)
					dataContainer.Add(exportContext.CreateBrickViewData(GetAreaStyle(exportContext, area, bounds), area, PanelBrick));
			}
		}
		void ProcessViewData(RectangleF boundsF, RectangleF clipBoundsF, ExportContext exportContext, Action<BrickViewData[], RectangleF> action) {
			PanelBrick panelBrick = PanelBrick;
			try {
				Rectangle clipBounds = GraphicsUnitConverter.Round(clipBoundsF);
				foreach(Brick brick in PanelBrick.Bricks) {
					RectangleF brickRectF = GetViewDataBounds(exportContext, brick);
					brickRectF = GraphicsUnitConverter.Convert(brickRectF, GraphicsDpi.Document, GraphicsDpi.DeviceIndependentPixel);
					brickRectF.Offset(boundsF.X, boundsF.Y);
					brickRectF.Intersect(boundsF);
					if(!clipBounds.IntersectsWith(GraphicsUnitConverter.Round(brickRectF)))
						continue;
					if(brick is VisualBrick && !(brick is CheckBoxBrick) && ((VisualBrick)brick).BackColor == Color.Transparent && PanelBrick.BackColor != Color.Transparent)
						(brick as VisualBrick).BackColor = PanelBrick.BackColor;
					BrickViewData[] exportData = exportContext.GetData(brick, brickRectF, RectangleF.Intersect(clipBoundsF, brickRectF));
					if(exportData != null) {
						ValidateInnerData(exportData, exportContext);
						action(exportData, brickRectF);
					}
				}
			} finally {
				SetBrick(panelBrick);
			}
		}
		protected virtual RectangleF GetViewDataBounds(ExportContext exportContext, Brick brick) {
			return brick.GetViewRectangle();
		}
		protected virtual void ValidateInnerData(BrickViewData[] innerData, ExportContext exportContext) {
		}
		protected virtual BrickViewData GetOuterPanel(ExportContext exportContext, RectangleF boundsF) {
			return exportContext.CreateBrickViewData(GetAreaStyle(exportContext, boundsF, boundsF), boundsF, TableCell);
		}
		protected virtual List<Rectangle> GenerateEmptyAreas(RectangleDivider divider, bool allowEmptyAreas) {
			return divider.GenerateEmptyAreas();
		}
	}
}
