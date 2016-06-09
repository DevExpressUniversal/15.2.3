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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.Office.Drawing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core;
using DevExpress.XtraRichEdit.Commands.Internal;
namespace DevExpress.Snap.Native.HoverDecorators {
	class PieceTablePasteSparklineDataInfoTemplateCommand : DevExpress.Snap.Core.Commands.PieceTablePasteDataInfoTemplateCommand {
		Dictionary<string, string> instructions;
		public PieceTablePasteSparklineDataInfoTemplateCommand(PieceTable pieceTable, Dictionary<string, string> instructions)
			: base(pieceTable) {
				this.instructions = instructions;
		}
		protected override Core.Native.Templates.TemplateBuilder CreateTemplateBuilder() {
			return new DevExpress.Snap.Core.Native.Templates.SparklineTemplateBuilder(instructions);
		}
	}
	public class SparklineBoxHoverLayout : HoverLayoutBase<SparklineBox>, IDropTarget {
		readonly HotZoneCollection hotZones;
		public SparklineBoxHoverLayout(RichEditView view, SparklineBox box, DocumentLogPosition start, PieceTable pieceTable, Point mousePosition)
			: base(view, box, start, pieceTable, mousePosition) {
			hotZones = box.HotZones;
		}
		public HotZoneCollection HotZones { get { return hotZones; } }
		protected override void OnMousePositionChanged() {
			foreach(DropValuesSparklineHotZone hotZone in HotZones)
				hotZone.IsHovered = DropFieldRoundHotZoneHoverCalculator.IsInRange(MousePosition, hotZone);
		}
		#region IDropTarget implementation
		public void OnDragDrop(DragEventArgs e) {
			PieceTable.DocumentModel.BeginUpdate();
			try {
				foreach(DropValuesSparklineHotZone hotZone in HotZones) {
					if(hotZone.IsHovered) {
						DoDragDropCore(e.Data);
						return;
					}
				}
			}
			finally {
				PieceTable.DocumentModel.EndUpdate();
			}
		}
		void DoDragDropCore(IDataObject data) {
			SnapPieceTable pieceTable = (SnapPieceTable)PieceTable;
			Field field = PieceTable.FindFieldByRunIndex(Box.StartPos.RunIndex);
			SNSparklineField sparklineField = (SNSparklineField)new SnapFieldCalculatorService().ParseField(PieceTable, field);
			pieceTable.DeleteFieldWithResult(field);
			var logPosition = DocumentModelPosition.FromRunStart(pieceTable, field.FirstRunIndex).LogPosition;
			PieceTable.DocumentModel.Selection.Start = logPosition;
			PieceTable.DocumentModel.Selection.End = logPosition;
			PieceTablePasteSparklineDataInfoTemplateCommand command = new PieceTablePasteSparklineDataInfoTemplateCommand(PieceTable, sparklineField.GetInstructions());
			command.PasteSource = new DataObjectPasteSource(data);
			command.CopyBetweenInternalModels = true;
			command.Execute();
		}
		public void OnDragEnter(DragEventArgs e) { }
		public void OnDragLeave(EventArgs e) { }
		public void OnDragOver(DragEventArgs e) { }
		#endregion
	}
	public class SparklineBoxHoverPainter : HoverPainterBase<SparklineBox> {
		readonly SparklineBoxHotZonePainter hotZonePainter;
		public SparklineBoxHoverPainter(SparklineBoxHoverLayout layout, Painter painter)
			: base(layout, painter) {
				this.hotZonePainter = new SparklineBoxHotZonePainter(Painter);
		}
		protected SparklineBoxHotZonePainter HotZonePainter { get { return hotZonePainter; } }
		public override void Draw() {
			foreach(IRoundHotZone hotZone in ((SparklineBoxHoverLayout)LayoutItem).HotZones)
				if(((DropValuesSparklineHotZone)hotZone).IsHovered || !LayoutItem.Box.IsEmpty)
					hotZone.Accept(HotZonePainter);
		}
		public override void Dispose() {
			base.Dispose();
			HotZonePainter.Dispose();
		}
	}
}
