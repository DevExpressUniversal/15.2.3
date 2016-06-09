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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Layout {
	#region LineNumberBox
	public class LineNumberBox : MultiPositionBox {
		#region Fields
		readonly LineNumberCommonRun run;
		readonly Row row;
		readonly string text;
		#endregion
		public LineNumberBox(LineNumberCommonRun run, Row row, string text) {
			Guard.ArgumentNotNull(run, "run");
			Guard.ArgumentNotNull(row, "row");
			this.run = run;
			this.row = row;
			this.text = text;
			StartPos = new FormatterPosition(RunIndex.Zero, 0, 0);
			EndPos = new FormatterPosition(RunIndex.Zero, 0, 0);
		}
		#region Properties
		public Row Row { get { return row; } }
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public override bool IsHyperlinkSupported { get { return false; } }
		#endregion
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return null;
		}
		public override Box CreateBox() {
			return new LineNumberBox(run, row, text);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportLineNumberBox(this);
		}
		public override TextRunBase GetRun(PieceTable pieceTable) {
			return run;
		}
		public override string GetText(PieceTable table) {
			return text;
		}
	}
	#endregion
	#region LineNumberBoxCollection
	public class LineNumberBoxCollection : BoxCollectionBase<LineNumberBox> {
		protected internal override void RegisterSuccessfullItemHitTest(BoxHitTestCalculator calculator, LineNumberBox item) {
		}
		protected internal override void RegisterFailedItemHitTest(BoxHitTestCalculator calculator) {
		}
	}
	#endregion
	#region LineNumberCommonRun
	public class LineNumberCommonRun : TextRunBase {
		public LineNumberCommonRun(Paragraph paragraph)
			: base(paragraph) {
		}
		protected LineNumberCommonRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override bool CanPlaceCaretBefore { get { return false; } }
		public override bool CanJoinWith(TextRunBase nextRun) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			throw new NotImplementedException();
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		protected internal override MergedCharacterProperties GetParentMergedCharacterProperties() {
			CharacterPropertiesMerger merger = new CharacterPropertiesMerger(CharacterStyle.GetMergedCharacterProperties());
			merger.Merge(DocumentModel.DefaultCharacterProperties);
			return merger.MergedProperties;
		}
	}
	#endregion
}
