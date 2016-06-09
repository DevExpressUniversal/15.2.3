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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Model {
	#region CharactersGroupIterator
	public class CharactersGroupIterator {
		static readonly List<char> GroupSymbols = CreateGroupSymbolsTable();
		static List<char> CreateGroupSymbolsTable() {
			List<char> result = new List<char>();
			result.Add('+');
			result.Add('-');
			result.Add(',');
			result.Add('\'');
			return result;
		}
		RunIndex cachedRunIndex = new RunIndex(-1);
		string cachedRangeText = String.Empty;
		readonly PieceTable pieceTable;
		public CharactersGroupIterator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		protected PieceTable PieceTable { get { return pieceTable; } }
		public virtual DocumentModelPosition MoveNext(DocumentModelPosition pos) {
			Guard.ArgumentNotNull(pos, "pos");
			DocumentModelPosition result = pos.Clone();
			if (IsEndOfDocument(result))
				return result;
			MoveNextCore(result);
			return result;
		}
		protected internal virtual void MoveNextCore(DocumentModelPosition pos) {
			PieceTableIterator iterator = new CharactersDocumentModelIterator(PieceTable);
			if (!SkipCharacters(pos, iterator, IsGroupCharacter)) {
				if (!SkipCharacters(pos, iterator, IsWhiteSpace))
					iterator.MoveForwardCore(pos);
			}
		}
		bool SkipCharacters(DocumentModelPosition pos, PieceTableIterator iterator, Predicate<char> predicate) {
			if (!predicate(GetCharacter(pos)))
				return false;
			while (!IsEndOfDocument(pos) && predicate(GetCharacter(pos)))
				iterator.MoveForwardCore(pos);
			return true;
		}
		protected bool IsEndOfDocument(DocumentModelPosition pos) {
			return pos.LogPosition == PieceTable.DocumentEndLogPosition;
		}
		protected char GetCharacter(DocumentModelPosition pos) {
			if (this.cachedRunIndex != pos.RunIndex) {
				this.cachedRunIndex = pos.RunIndex;
				this.cachedRangeText = PieceTable.GetRunNonEmptyText(this.cachedRunIndex);
			}
			return this.cachedRangeText[pos.RunOffset];
		}
		protected internal bool IsGroupCharacter(char ch) {
			return Char.IsLetterOrDigit(ch) || GroupSymbols.Contains(ch);
		}
		protected internal bool IsWhiteSpace(char ch) {
			return Char.IsWhiteSpace(ch);
		}
	}
	#endregion
}
