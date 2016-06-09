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
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraRichEdit.Layout {
	public abstract class CharacterMarkBox : MultiPositionBox {
		protected internal abstract char MarkCharacter { get; }
		public override FontInfo GetFontInfo(PieceTable pieceTable) {
			FontInfo result = base.GetFontInfo(pieceTable);
			DocumentModel documentModel = pieceTable.DocumentModel;
			if (!documentModel.FontCache.ShouldUseDefaultFontToDrawInvisibleCharacter(result, MarkCharacter)) {
				TextRunBase run = GetRun(pieceTable);
				int fontIndex = run.CalculateFontIndexCore(documentModel.Cache.CharacterFormattingCache.DefaultItem.FontName);
				result = documentModel.FontCache[fontIndex];
			}
			return result;
		}
	}
	public abstract class SingleCharacterMarkBox : SinglePositionBox {
		protected internal abstract char MarkCharacter { get; }
		public override FontInfo GetFontInfo(PieceTable pieceTable) {
			FontInfo result = base.GetFontInfo(pieceTable);
			DocumentModel documentModel = pieceTable.DocumentModel;
			if (!documentModel.FontCache.ShouldUseDefaultFontToDrawInvisibleCharacter(result, MarkCharacter)) {
				TextRunBase run = GetRun(pieceTable);
				int fontIndex = run.CalculateFontIndexCore(documentModel.Cache.CharacterFormattingCache.DefaultItem.FontName);
				result = documentModel.FontCache[fontIndex];
			}
			return result;
		}
	}
	#region ParagraphMarkBox
	public class ParagraphMarkBox : SingleCharacterMarkBox {
		public override bool IsVisible { get { return false; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return true; } }
		protected internal override char MarkCharacter { get { return Characters.PilcrowSign; } }
		public override Box CreateBox() {
			return new ParagraphMarkBox();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportParagraphMarkBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateParagraphMarkBoxHitTestManager(this);
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
}
