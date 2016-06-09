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
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellKey
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct CellKey : IComparable<CellKey>, ISheetPosition {
		readonly long m_value;
		[System.Diagnostics.DebuggerStepThrough]
		public CellKey(int sheetId, int columnIndex, int rowIndex) {
			m_value = ((long)sheetId << 34) + ((long)columnIndex << 20) + rowIndex;
		}
		public int RowIndex { get { return (int)(m_value & 0x000FFFFF); } } 
		public int ColumnIndex { get { return (int)((m_value >> 20) & 0x00003FFF); } } 
		public int SheetId { get { return (int)(m_value >> 34); } }
		public override bool Equals(object obj) {
			return ((obj is CellKey) && (this.m_value == ((CellKey)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			CellPosition position = GetPosition();
			return String.Format("{0}, SheetId={1}, m_value={2}", position, this.SheetId, m_value);
		}
		public CellPosition GetPosition() {
			return new CellPosition(this.ColumnIndex, this.RowIndex, PositionType.Relative, PositionType.Relative);
		}
		public static bool operator ==(CellKey index1, CellKey index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(CellKey index1, CellKey index2) {
			return index1.m_value != index2.m_value;
		}
		public long ToLong() {
			return m_value;
		}
		#region IComparable<CellKey> Members
		public int CompareTo(CellKey other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
		#region ISheetPosition Members
		public int LeftColumnIndex { get { return ColumnIndex; } }
		public int RightColumnIndex { get { return ColumnIndex; } }
		public int TopRowIndex { get { return RowIndex; } }
		public int BottomRowIndex { get { return RowIndex; } }
		#endregion
		public CellKey GetShifted(CellPositionOffset offset, Worksheet worksheet) {
			CellPosition shifted = this.GetPosition().GetShiftedAny(offset, worksheet);
			if (shifted.Equals(CellPosition.InvalidValue))
				DevExpress.Office.Utils.Exceptions.ThrowInternalException();
			return shifted.ToKey(worksheet.SheetId);
		}
	}
	#endregion
}
