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
using System.Globalization;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Fields {
	public interface IFieldIterator {
		char ReadNextChar();
		bool IsEnd();
		DocumentLogPosition Position { get; }
		char PeekNextChar();
		void AdvanceNextChar();
		void AdvanceNextChar(bool skipNestedFields);
		bool IsFieldCodeStart();
		bool IsFieldEnd();
		bool IsFieldCodeEnd();
	} 
	public class DocumentFieldIterator : IFieldIterator {
		readonly PieceTable pieceTable;
		DocumentModelPosition position;
		readonly CharactersDocumentModelIterator charactersIterator;
		RunIndex lastRunIndex;
		string currentRunText;
		int nestedLevel;
		public DocumentFieldIterator(PieceTable pieceTable, Field field) {
			this.charactersIterator = new CharactersDocumentModelIterator(pieceTable);
			this.pieceTable = pieceTable;
			this.position = DocumentModelPosition.FromRunStart(PieceTable, field.FirstRunIndex + 1);
			SkipNestedFields();
			TextRunBase range = PieceTable.Runs[position.RunIndex];
			currentRunText = range.GetRawTextFast(PieceTable.TextBuffer);
			lastRunIndex = field.Code.End;
		}
		protected internal virtual PieceTable PieceTable { get { return pieceTable; } }
		#region IFieldIterator Members
		public char ReadNextChar() {
			char result = PeekNextChar();
			AdvanceNextChar();
			return result;
		}
		public bool IsEnd() {
			return position.RunIndex == lastRunIndex;
		}
		public DocumentLogPosition Position { get { return position.LogPosition; } }
		public char PeekNextChar() {
			if (!IsEnd())
				return currentRunText[position.RunOffset];
			else
				return (char)0;
		}
		public bool IsFieldCodeStart() {
			return !IsEnd() && PieceTable.Runs[position.RunIndex] is FieldCodeStartRun;
		}
		public bool IsFieldEnd() {
			return !IsEnd() && PieceTable.Runs[position.RunIndex] is FieldResultEndRun;
		}
		public bool IsFieldCodeEnd() {
			return !IsEnd() && PieceTable.Runs[position.RunIndex] is FieldCodeEndRun;
		}
		public void AdvanceNextChar() {
			AdvanceNextChar(true);
		}
		public void AdvanceNextChar(bool skipNestedFields) {
			RunIndex prevRunIndex = position.RunIndex;
			if (prevRunIndex == lastRunIndex)
				return;
			position = charactersIterator.MoveForward(position);
			if (position.RunIndex != prevRunIndex) {
				if(skipNestedFields)
					SkipNestedFields();
				if (position.RunIndex != lastRunIndex)
					currentRunText = PieceTable.Runs[position.RunIndex].GetRawTextFast(PieceTable.TextBuffer);
			}
		}
		protected virtual bool IsNestedFieldBegin(TextRunBase range) {
			return range is FieldCodeStartRun;
		}
		protected virtual bool IsNestedFieldEnd(TextRunBase range) {
			return nestedLevel > 0 && range is FieldResultEndRun;
		}
		protected virtual void StartNestedField() {			
			TextRunCollection ranges = PieceTable.Runs;
			Debug.Assert(ranges[position.RunIndex] is FieldCodeStartRun);
			int innerNestedLevel = 1;
			while (innerNestedLevel > 0) {				
				position.RunIndex++;
				TextRunBase textRun = ranges[position.RunIndex];
				if (textRun is FieldCodeStartRun)
					innerNestedLevel++;
				if (textRun is FieldCodeEndRun)
					innerNestedLevel--;				
			}
			position = DocumentModelPosition.FromRunStart(PieceTable, position.RunIndex + 1);
			nestedLevel++;
		}
		protected virtual void EndNestedField() {
			position = DocumentModelPosition.FromRunStart(PieceTable, position.RunIndex + 1);
			nestedLevel--;
		}
		protected virtual void SkipNestedFields() {
			bool positionChanged;
			do {
				positionChanged = false;
				while (IsNestedFieldBegin(PieceTable.Runs[position.RunIndex])) {
					StartNestedField();
					positionChanged = true;
				}
				while (IsNestedFieldEnd(PieceTable.Runs[position.RunIndex])) {
					EndNestedField();
					positionChanged = true;
				}
			} while (positionChanged);
		}
		#endregion
	}
}
