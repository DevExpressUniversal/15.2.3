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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Model {
	#region InputPosition
	public class InputPosition : ICloneable<InputPosition>, ISupportsCopyFrom<InputPosition> {
		#region Fields
		readonly PieceTable pieceTable;
		readonly CharacterFormattingBase characterFormatting;
		readonly CharacterFormattingInfo mergedCharacterFormatting;
		DocumentLogPosition logPosition;
		ParagraphIndex paragraphIndex;
		int characterStyleIndex;
		#endregion
		public InputPosition(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.characterFormatting = new CharacterFormattingBase(pieceTable, pieceTable.DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex, CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			this.mergedCharacterFormatting = new CharacterFormattingInfo();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentLogPosition LogPosition {
			get { return logPosition; }
			set {
				if (value == logPosition)
					return;
				if (!IsValidLogPosition(value))
					Exceptions.ThrowArgumentException("LogPosition", value);
				logPosition = value;
			}
		}
		public ParagraphIndex ParagraphIndex {
			get { return paragraphIndex; }
			set {
				if (value == paragraphIndex)
					return;
				if (!IsValidParagraphIndex(value))
					Exceptions.ThrowArgumentException("ParagraphIndex", value);
				paragraphIndex = value;
			}
		}
		public virtual CharacterFormattingBase CharacterFormatting { get { return characterFormatting; } }
		public CharacterFormattingInfo MergedCharacterFormatting { get { return mergedCharacterFormatting; } }
		public virtual int CharacterStyleIndex { get { return characterStyleIndex; } set { characterStyleIndex = value; } }
		#endregion
		internal bool IsValidLogPosition(DocumentLogPosition val) {
			if (val < DocumentLogPosition.Zero)
				return false;
			Paragraph lastParagraph = pieceTable.Paragraphs.Last;
			return val < (lastParagraph.LogPosition + lastParagraph.Length);
		}
		internal bool IsValidParagraphIndex(ParagraphIndex val) {
			if (val < new ParagraphIndex(0))
				return false;
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			return val < new ParagraphIndex(paragraphs.Count);
		}
		#region ICloneable<InputPosition> Members
		public InputPosition Clone() {
			InputPosition clone = new InputPosition(pieceTable);
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<InputPosition> Members
		public void CopyFrom(InputPosition value) {
			this.LogPosition = value.LogPosition;
			this.ParagraphIndex = value.ParagraphIndex;
			CopyFormattingFrom(value);
		}
		#endregion
		public void CopyFormattingFrom(InputPosition value) {
			CopyCharacterFormattingFrom(value);
			this.MergedCharacterFormatting.CopyFrom(value.MergedCharacterFormatting);
		}
		protected internal void CopyCharacterFormattingFrom(InputPosition value) {
			this.CharacterStyleIndex = value.CharacterStyleIndex;
			this.CharacterFormatting.CopyFrom(value.CharacterFormatting);
		}
	}
	#endregion
}
