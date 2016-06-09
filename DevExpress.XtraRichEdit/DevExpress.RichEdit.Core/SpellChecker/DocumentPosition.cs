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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraSpellChecker.Parser;
namespace DevExpress.XtraRichEdit.SpellChecker {
	#region PositionOffset
	public class PositionOffset : Position {
		int m_value;
		public PositionOffset(int value) {
			this.m_value = value;
		}
		public PositionOffset()
			: this(0) {
		}
		protected internal int Offset { get { return m_value; } }
		protected override object ActualPosition {
			get {
				return m_value;
			}
			set {
				if (!(value is int))
					Exceptions.ThrowArgumentException("ActualPosition", value);
				m_value = (int)value;
			}
		}
		public override int ToInt() {
			return m_value;
		}
		protected override Position InternalAdd(Position position) {
			if (position == null)
				return Clone();
			PositionOffset positionOffset = position as PositionOffset;
			if (positionOffset == null)
				Exceptions.ThrowArgumentException("position", position);
			return InternalAdd(positionOffset);
		}
		protected virtual PositionOffset InternalAdd(PositionOffset position) {
			return new PositionOffset(m_value + position.m_value);
		}
		protected override int InternalCompare(Position position) {
			if (position == null)
				return m_value;
			PositionOffset positionOffset = position as PositionOffset;
			if (positionOffset == null)
				Exceptions.ThrowArgumentException("position", position);
			return InternalCompare(positionOffset);
		}
		protected virtual int InternalCompare(PositionOffset position) {
			return m_value - position.m_value;
		}
		protected override Position InternalSubtract(Position position) {
			if (position == null)
				return Clone();
			PositionOffset positionOffset = position as PositionOffset;
			if (positionOffset == null)
				Exceptions.ThrowArgumentException("position", position);
			return InternalSubtract(positionOffset);
		}
		protected virtual PositionOffset InternalSubtract(PositionOffset position) {
			return new PositionOffset(m_value - position.m_value);
		}
		protected override Position InternalSubtractFromNull() {
			return new PositionOffset(-m_value);
		}
		protected override Position MoveForward() {
			return new PositionOffset(m_value + 1);
		}
		protected override Position MoveBackward() {
			return new PositionOffset(m_value - 1);
		}
		public override Position Clone() {
			return new PositionOffset(m_value);
		}
	}
	#endregion
	internal class EmptyDocumentPosition : DocumentPosition {
		public EmptyDocumentPosition() { }
		protected internal override DocumentLogPosition LogPosition { get { return DocumentLogPosition.Zero; } }
		protected override object ActualPosition {
			get { return LogPosition; }
			set { }
		}
		protected internal override bool IsValid { get { return true; } }
		protected override Position InternalAdd(Position position) {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override Position InternalSubtract(Position position) {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override Position InternalSubtractFromNull() {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override Position MoveBackward() {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override Position MoveForward() {
			Exceptions.ThrowInternalException();
			return null;
		}
		public override bool UpdatePosition() {
			return true;
		}
		public override Position Clone() {
			return new EmptyDocumentPosition();
		}
	}
	#region DocumentPosition
	public class DocumentPosition : Position {
		DocumentModelPosition position;
		bool isValid;
		static Position zero = new EmptyDocumentPosition();
		protected DocumentPosition() { }
		public DocumentPosition(PieceTable pieceTable)
			: this(new DocumentModelPosition(pieceTable)) {
		}
		public DocumentPosition(DocumentModelPosition value) {
			this.position = value;
			this.isValid = true;
		}
		#region Properties
		protected internal virtual bool IsValid { get { return isValid; } }
		internal DocumentModelPosition Position {
			get {
				if (!IsValid)
					UpdatePosition();
				return position;
			}
		}
		protected PieceTable PieceTable { get { return Position.PieceTable; } }
		protected internal virtual DocumentLogPosition LogPosition { get { return Position.LogPosition; } }
		protected override object ActualPosition {
			get { return Position; }
			set {
				DocumentModelPosition pos = value as DocumentModelPosition;
				if (pos == null)
					Exceptions.ThrowArgumentException("ActualPosition", value);
				position = pos;
			}
		}
		protected override Position Zero { get { return zero; } }
		#endregion
		public override int ToInt() {
			return ((IConvertToInt<DocumentLogPosition>)LogPosition).ToInt();
		}
		protected internal void InvalidatePosition() {
			this.isValid = false;
		}
		protected override Position InternalAdd(Position position) {
			if (position == null)
				return Clone();
			PositionOffset positionOffset = position as PositionOffset;
			if (positionOffset == null)
				Exceptions.ThrowArgumentException("position", position);
			return InternalAdd(positionOffset);
		}
		DocumentPosition InternalAdd(PositionOffset position) {
			DocumentLogPosition logPosition = Position.LogPosition + position.Offset;
			if (logPosition > PieceTable.DocumentEndLogPosition)
				Exceptions.ThrowArgumentException("position", position);
			return new DocumentPosition(PositionConverter.ToDocumentModelPosition(PieceTable, logPosition));
		}
		protected override Position InternalSubtract(Position position) {
			if (position == null)
				return Clone();
			DocumentPosition documentPosition = position as DocumentPosition;
			if (documentPosition != null)
				return InternalSubtract(documentPosition);
			PositionOffset positionOffset = position as PositionOffset;
			if (positionOffset != null)
				return InternalSubtract(positionOffset);
			Exceptions.ThrowArgumentException("position", position);
			return null;
		}
		DocumentPosition InternalSubtract(PositionOffset position) {
			DocumentLogPosition logPosition = Position.LogPosition - position.Offset;
			if (logPosition < PieceTable.DocumentStartLogPosition)
				Exceptions.ThrowArgumentException("position", position);
			return new DocumentPosition(PositionConverter.ToDocumentModelPosition(PieceTable, logPosition));
		}
		PositionOffset InternalSubtract(DocumentPosition position) {
			return new PositionOffset(Position.LogPosition - position.Position.LogPosition);
		}
		protected override Position InternalSubtractFromNull() {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override int InternalCompare(Position position) {
			if (position == null)
				return 1;
			DocumentPosition documentPosition = position as DocumentPosition;
			if (documentPosition == null)
				Exceptions.ThrowArgumentException("position", position);
			return InternalCompare(documentPosition);
		}
		int InternalCompare(DocumentPosition position) {
			return LogPosition - position.LogPosition;
		}
		protected override Position MoveForward() {
			return new DocumentPosition(DocumentModelPosition.MoveForward(Position));
		}
		protected override Position MoveBackward() {
			return new DocumentPosition(DocumentModelPosition.MoveBackward(Position));
		}
		public override string ToString() {
			return Position.LogPosition.ToString();
		}
		public override Position Clone() {
			return new DocumentPosition(Position.Clone());
		}
		public virtual bool UpdatePosition() {
			DocumentLogPosition logPosition = position.LogPosition;
			PieceTable pieceTable = position.PieceTable;
			if (logPosition < pieceTable.DocumentStartLogPosition || logPosition > pieceTable.DocumentEndLogPosition)
				isValid = false;
			else {
				position.Update();
				isValid = true;
			}
			return isValid;
		}
	}
	#endregion
}
