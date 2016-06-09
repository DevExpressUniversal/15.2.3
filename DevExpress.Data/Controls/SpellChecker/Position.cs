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
namespace DevExpress.XtraSpellChecker.Parser {
	public abstract class Position {
		public static Position Null { get { return null; } }
		public static Position Undefined { get { return null; } }
		public static Position Add(Position position1, Position position2) {
			if (position1 != null)
				return position1.InternalAdd(position2);
			else 
				if (position2 != null)
					return position2.InternalAdd(position1);
				else
					return Null;
		}
		public static Position Subtract(Position position1, Position position2) {
			if (position1 != null)
				return position1.InternalSubtract(position2);
			else
				if (position2 != null)
					return position2.InternalSubtractFromNull();
				else
					return Null;
		}
		public static int Compare(Position position1, Position position2) {
			if (position1 != null)
				return position1.InternalCompare(position2);
			else
				if (position2 != null)
					return -position2.InternalCompare(position1);
				else
					return 0;
		}
		public static bool IsGreater(Position position1, Position position2) {
			return Compare(position1, position2) > 0;
		}
		public static bool IsGreaterOrEqual(Position position1, Position position2) {
			return IsGreater(position1, position2) || Equals(position1, position2);
		}
		public static bool Equals(Position position1, Position position2) {
			return Compare(position1, position2) == 0;
		}
		public static bool IsLess(Position position1, Position position2) {
			return Compare(position1, position2) < 0;
		}
		public static bool IsLessOrEqual(Position position1, Position position2) {
			return IsLess(position1, position2) || Equals(position1, position2);
		}
		public static Position operator -(Position position1, Position position2) {
			return Subtract(position1, position2);
		}
		public static Position operator +(Position position1, Position position2) {
			return Add(position1, position2);
		}
		public static Position operator ++(Position position) {
			return position.MoveForward();
		}
		public static Position operator --(Position position) {
			return position.MoveBackward();
		}
		protected Position(object actualPosition) {
			ActualPosition = actualPosition;
		}
		protected Position() {
		}
		protected abstract object ActualPosition { get; set; }
		protected virtual Position Zero { get { return Subtract(this, this); } }
		public bool IsZero { get { return Equals(Zero, this); } }
		public bool IsNegative { get { return IsLess(this, Zero); } }
		public bool IsPositive { get { return IsGreater(this, Zero); } }
		public abstract Position Clone();
		public abstract int ToInt();
		protected abstract int InternalCompare(Position position);
		protected abstract Position InternalAdd(Position position);
		protected abstract Position InternalSubtract(Position position);
		protected abstract Position InternalSubtractFromNull(); 
		protected abstract Position MoveBackward();
		protected abstract Position MoveForward();
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(this, obj))
				return true;
			Position position = obj as Position;
			if (position == null)
				return false;
			return Equals(this, position);
		}
		public override int GetHashCode() {
			return ActualPosition.GetHashCode();
		}
	}
}
