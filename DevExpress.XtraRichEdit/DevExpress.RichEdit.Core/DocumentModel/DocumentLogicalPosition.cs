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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentLogPosition
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct DocumentLogPosition : IConvertToInt<DocumentLogPosition>, IComparable<DocumentLogPosition> {
		int m_value;
		public static readonly DocumentLogPosition MinValue = new DocumentLogPosition(0);
		public static readonly DocumentLogPosition Zero = new DocumentLogPosition(0);
		public static readonly DocumentLogPosition MaxValue = new DocumentLogPosition(int.MaxValue);
		[DebuggerStepThrough]
		public DocumentLogPosition(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is DocumentLogPosition) && (this.m_value == ((DocumentLogPosition)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static DocumentLogPosition operator +(DocumentLogPosition pos, int value) {
			return new DocumentLogPosition(pos.m_value + value);
		}
		public static int operator -(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value - pos2.m_value;
		}
		public static DocumentLogPosition operator -(DocumentLogPosition pos, int value) {
			return new DocumentLogPosition(pos.m_value - value);
		}
		public static DocumentLogPosition operator ++(DocumentLogPosition pos) {
			return new DocumentLogPosition(pos.m_value + 1);
		}
		public static DocumentLogPosition operator --(DocumentLogPosition pos) {
			return new DocumentLogPosition(pos.m_value - 1);
		}
		public static bool operator <(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value < pos2.m_value;
		}
		public static bool operator <=(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value <= pos2.m_value;
		}
		public static bool operator >(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value > pos2.m_value;
		}
		public static bool operator >=(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value >= pos2.m_value;
		}
		public static bool operator ==(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value == pos2.m_value;
		}
		public static bool operator !=(DocumentLogPosition pos1, DocumentLogPosition pos2) {
			return pos1.m_value != pos2.m_value;
		}
		#region IComparable<DocumentLogPosition> Members
		int IComparable<DocumentLogPosition>.CompareTo(DocumentLogPosition other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
		#region IConvertToInt<DocumentLogPosition> Members
		int IConvertToInt<DocumentLogPosition>.ToInt() {
			return m_value;
		}
		DocumentLogPosition IConvertToInt<DocumentLogPosition>.FromInt(int value) {
			return new DocumentLogPosition(value);
		}
		#endregion
	}
	#endregion
}
