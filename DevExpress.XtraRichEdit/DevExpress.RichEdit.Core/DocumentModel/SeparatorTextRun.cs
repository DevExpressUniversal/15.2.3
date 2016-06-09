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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Model {
	public class SeparatorTextRun : TextRun {
		protected internal static readonly string SeparatorText = "|";
		public SeparatorTextRun(Paragraph paragraph)
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
			return SeparatorText;
		}
		protected internal override string GetPlainText(ChunkedStringBuilder growBuffer, int from, int to) {
			return GetPlainText(growBuffer);
		}
		public override bool CanJoinWith(TextRunBase nextRange) {
			return false;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			base.Measure(boxInfo, measurer);
#if !DXPORTABLE
			boxInfo.Size = new System.Drawing.Size(0, boxInfo.Size.Height);
#else
			boxInfo.Size = new Size(0, boxInfo.Size.Height);
#endif
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
			pieceTable.InsertSeparatorTextRunCore(targetPosition.ParagraphIndex, targetPosition.LogPosition);
			TextRunBase run = pieceTable.Runs[targetPosition.RunIndex];
			CopyCore(copyManager.TargetModel, run);
			return run;
		}
	}
}
