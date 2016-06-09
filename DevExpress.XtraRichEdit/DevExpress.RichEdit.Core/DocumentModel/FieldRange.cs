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
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region FieldResultEndRun
	public class FieldResultEndRun : TextRun {
		public FieldResultEndRun(Paragraph paragraph)
			: base(paragraph) {
		}
		#region Properties
		public override bool CanPlaceCaretBefore { get { return true; } }
		public override int Length { get { return 1; } set { } }
		#endregion
		protected internal override string GetText() {
			return String.Empty;
		}
		protected internal override string GetTextFast(ChunkedStringBuilder growBuffer) {
			return String.Empty;
		}
		protected internal override string GetRawTextFast(ChunkedStringBuilder growBuffer) {			
			return " ";
		}
		public override string GetPlainText(ChunkedStringBuilder growBuffer) {
			return String.Empty;
		}
		protected internal override string GetPlainText(ChunkedStringBuilder growBuffer, int from, int to) {
			return String.Empty;
		}
		public override bool CanJoinWith(TextRunBase nextRange) {
			return false;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			Exceptions.ThrowInternalException();
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable pieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			pieceTable.InsertFieldResultEndRunCore(targetPosition.ParagraphIndex, targetPosition.LogPosition);
			TextRunBase run = pieceTable.Runs[targetPosition.RunIndex];
			CopyCore(copyManager.TargetModel, run);		 
			return run;
		}
	}
	#endregion
	#region FieldCodeRunBase (abstract class)
	public abstract class FieldCodeRunBase : SpecialTextRun {
		protected FieldCodeRunBase(Paragraph paragraph)
			: base(paragraph) {
		}
		#region Properties
		public override bool CanPlaceCaretBefore { get { return true; } }
		public override int Length { get { return 1; } set { } }
		#endregion
		protected internal override string GetText() {
			return String.Empty;
		}
		protected internal override string GetTextFast(ChunkedStringBuilder growBuffer) {
			return String.Empty;
		}
		protected internal override string GetRawTextFast(ChunkedStringBuilder growBuffer) {
			Debug.Assert(false);
			return String.Empty;
		}
		protected internal override string GetPlainText(ChunkedStringBuilder growBuffer, int from, int to) {
			return GetPlainText(growBuffer);
		}
		public override bool CanJoinWith(TextRunBase nextRange) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable pieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			InsertMarkRange(pieceTable, targetPosition);
			TextRunBase range = pieceTable.Runs[targetPosition.RunIndex];
			range.CopyCharacterPropertiesFrom(copyManager, this);
			return range;
		}
		protected abstract void InsertMarkRange(PieceTable pieceTable, DocumentModelPosition pos);		
	}
	#endregion
	#region FieldCodeStartRun
	public class FieldCodeStartRun : FieldCodeRunBase {
		public FieldCodeStartRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public override string GetPlainText(ChunkedStringBuilder growBuffer) {
			return new String(new char[] { '\u0013' });
		}
		protected internal override string GetRawTextFast(ChunkedStringBuilder growBuffer) {
			return "{";
		}
		protected override void InsertMarkRange(PieceTable pieceTable, DocumentModelPosition pos) {
			pieceTable.InsertFieldCodeStartRunCore(pos.ParagraphIndex, pos.LogPosition);
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
	}
	#endregion
	#region FieldCodeEndRun
	public class FieldCodeEndRun : FieldCodeRunBase {
		public FieldCodeEndRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public override string GetPlainText(ChunkedStringBuilder growBuffer) {
			return new String(new char[] { '\u0015' });
		}
		protected internal override string GetRawTextFast(ChunkedStringBuilder growBuffer) {
			return "}";
		}
		protected override void InsertMarkRange(PieceTable pieceTable, DocumentModelPosition pos) {
			pieceTable.InsertFieldCodeEndRunCore(pos.ParagraphIndex, pos.LogPosition);
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
	}
	#endregion
}
