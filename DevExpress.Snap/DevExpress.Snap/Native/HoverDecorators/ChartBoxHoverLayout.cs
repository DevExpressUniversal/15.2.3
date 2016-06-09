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
using System.Windows.Forms;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.Office.Drawing;
namespace DevExpress.Snap.Native.HoverDecorators {
	public class ChartBoxHoverLayout : HoverLayoutBase<ChartBox>, IDropTarget {
		readonly HotZoneCollection hotZones;
		public ChartBoxHoverLayout(RichEditView view, ChartBox box, DocumentLogPosition start, PieceTable pieceTable, Point mousePosition)
			: base(view, box, start, pieceTable, mousePosition) {
			hotZones = box.HotZones;
		}
		public HotZoneCollection HotZones { get { return hotZones; } }
		protected override void OnMousePositionChanged() {
			foreach (DropFieldChartHotZone hotZone in HotZones)
				hotZone.IsHovered = DropFieldRoundHotZoneHoverCalculator.IsInRange(MousePosition, hotZone);
		}
		#region IDropTarget implementation
		public void OnDragDrop(DragEventArgs e) {
			foreach (DropFieldChartHotZone hotZone in HotZones) {
				if (hotZone.IsHovered) {
					DoDragDropCore(hotZone, e.Data);
					return;
				}
			}
		}
		void DoDragDropCore(DropFieldChartHotZone hotZone, IDataObject data) {
			SnapPieceTable pieceTable = (SnapPieceTable)PieceTable;
			Field field = PieceTable.FindFieldByRunIndex(Box.StartPos.RunIndex);
			SNChartField chartField = (SNChartField)new SnapFieldCalculatorService().ParseField(PieceTable, field);
			SnapFieldInfo fieldInfo = new SnapFieldInfo((SnapPieceTable)PieceTable, field);
			using (ChartContainer chartContainer = (ChartContainer)chartField.GetChartContainer(pieceTable.DocumentModel, fieldInfo)) {
				hotZone.DoDragDrop(data, chartContainer.Chart);
				SNChartHelper.SaveChartField(chartField, pieceTable, field, chartContainer.Chart);
			}
		}
		public void OnDragEnter(DragEventArgs e) { }
		public void OnDragLeave(EventArgs e) { }
		public void OnDragOver(DragEventArgs e) { }
		#endregion
	}
	public class ChartBoxHoverPainter : HoverPainterBase<ChartBox> {
		readonly ChartBoxHotZonePainter hotZonePainter;
		public ChartBoxHoverPainter(ChartBoxHoverLayout layout, Painter painter)
			: base(layout, painter) {
			this.hotZonePainter = new ChartBoxHotZonePainter(Painter);
		}
		protected ChartBoxHotZonePainter HotZonePainter { get { return hotZonePainter; } }
		public override void Draw() {
			foreach (IRoundHotZone hotZone in ((ChartBoxHoverLayout)LayoutItem).HotZones)
				if (((DropFieldChartHotZone)hotZone).IsHovered || !LayoutItem.Box.IsEmpty)
					hotZone.Accept(HotZonePainter);
		}
		public override void Dispose() {
			base.Dispose();
			HotZonePainter.Dispose();
		}
	}
}
