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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.Utils;
namespace DevExpress.SpreadsheetSource.Implementation {
	#region ColumnInfo
	public class ColumnInfo {
		public ColumnInfo(int firstIndex, int lastIndex, bool isHidden, int formatIndex) {
			FirstIndex = firstIndex;
			LastIndex = lastIndex;
			IsHidden = isHidden;
			FormatIndex = formatIndex;
		}
		public int FirstIndex { get; private set; }
		public int LastIndex { get; private set; }
		public bool IsHidden { get; private set; }
		public int FormatIndex { get; private set; }
	}
	#endregion
	#region ColumnInfoComparable
	class ColumnInfoComparable : IComparable<ColumnInfo> {
		int index;
		public ColumnInfoComparable(int index) {
			this.index = index;
		}
		public int CompareTo(ColumnInfo other) {
			if(index < other.FirstIndex)
				return 1;
			if(index > other.LastIndex)
				return -1;
			return 0;
		}
	}
	#endregion
	#region ColumnsCollection
	public class ColumnsCollection : List<ColumnInfo> {
		public bool IsColumnHidden(int columnIndex) {
			ColumnInfo info = FindColumn(columnIndex);
			if(info == null)
				return false;
			return info.IsHidden;
		}
		public int GetFormatIndex(int columnIndex) {
			ColumnInfo info = FindColumn(columnIndex);
			if(info == null)
				return -1;
			return info.FormatIndex;
		}
		public ColumnInfo FindColumn(int columnIndex) {
			int index = Algorithms.BinarySearch<ColumnInfo>(this, new ColumnInfoComparable(columnIndex));
			if(index >= 0)
				return this[index];
			return null;
		}
	}
	#endregion
}
