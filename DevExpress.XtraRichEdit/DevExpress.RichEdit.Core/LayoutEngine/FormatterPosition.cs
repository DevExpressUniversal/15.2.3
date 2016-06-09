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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region FormatterPosition
	public struct FormatterPosition :  IComparable<FormatterPosition> {
		public static readonly FormatterPosition MaxValue = new FormatterPosition(RunIndex.MaxValue, int.MaxValue, 0); 
		#region Fields
		RunIndex runIndex;
		int offset;
		int boxIndex;
		#endregion
		public FormatterPosition(RunIndex runIndex, int offset, int boxIndex) {
			this.runIndex = runIndex;
			this.offset = offset;
			this.boxIndex = boxIndex;
		}
		#region Properties
		public  int Offset { get { return offset; }  }
		public  RunIndex RunIndex { get { return runIndex; }  }
		public int BoxIndex { get { return boxIndex; } }
		#endregion
		public void SetOffset(int value) {
			this.offset = value;
		}
		public void SetRunIndex(RunIndex value) {
			this.runIndex = value;
		}
		public void SetBoxIndex(int value) {
			this.boxIndex = value;
		}
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public static bool operator <(FormatterPosition pos1, FormatterPosition pos2) {
			return pos1.RunIndex < pos2.RunIndex || (pos1.RunIndex == pos2.RunIndex && pos1.Offset < pos2.Offset);
		}
		public static bool operator <=(FormatterPosition pos1, FormatterPosition pos2) {
			return !(pos1 > pos2);
		}
		public static bool operator >(FormatterPosition pos1, FormatterPosition pos2) {
			return pos1.RunIndex > pos2.RunIndex || (pos1.RunIndex == pos2.RunIndex && pos1.Offset > pos2.Offset);
		}
		public static bool operator >=(FormatterPosition pos1, FormatterPosition pos2) {
			return !(pos1 < pos2);
		}
		public static bool operator ==(FormatterPosition pos1, FormatterPosition pos2) {
			return pos1.Offset == pos2.Offset && pos1.RunIndex == pos2.RunIndex;
		}
		public static bool operator !=(FormatterPosition pos1, FormatterPosition pos2) {
			return !(pos1 == pos2);
		}
		public bool AreEqual(FormatterPosition pos) {
			return pos.Offset == Offset && pos.RunIndex == RunIndex;
		}
		public bool AreLarger(FormatterPosition pos) {
			if (RunIndex > pos.RunIndex || (RunIndex == pos.RunIndex && Offset > pos.Offset))
				return true;
			return false;
		}
#if DEBUG
		public override string ToString() {
			return String.Format("RunIndex: {0}, Offset: {1}, BoxIndex={2}", RunIndex.ToString(), Offset, BoxIndex);
		}
#endif
		public  void OffsetRunIndex(int delta) {
			runIndex += delta;
		}
		#region IComparable<FormatterPosition> Members
		int IComparable<FormatterPosition>.CompareTo(FormatterPosition other) {
			return Compare(this, other);
		}
		int Compare(FormatterPosition position1, FormatterPosition position2) {
			if (position1 < position2)
				return -1;
			if (position1 > position2)
				return 1;
			return 0;
		}
		#endregion
	}
	#endregion
	#region FormatterPositionCollection
	public class FormatterPositionCollection : List<FormatterPosition> {
	}
	#endregion
}
