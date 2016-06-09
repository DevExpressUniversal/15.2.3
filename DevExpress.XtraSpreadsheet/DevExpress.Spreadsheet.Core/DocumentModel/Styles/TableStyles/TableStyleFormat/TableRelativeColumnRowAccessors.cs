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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ITableRelativeColumnRowCache
	public interface ITableRelativeColumnRowCache {
		int LastRelativeIndex { get; set; }
		int FirstRelativeDataIndex { get; set; }
		int LastRelativeDataIndex { get; set; }
		void RegisterRelativeIndex(int absoluteIndex, int relativeIndex);
		bool ContainsAbsoluteDataIndex(int absoluteDataIndex);
		int GetRelativeIndex(int absoluteIndex);
		bool HasVisibleColumnRowIndex(int absoluteIndex);
	}
	#endregion
	#region TableRelativeColumnsRowsCache
	public class TableRelativeColumnsRowsCache : ITableRelativeColumnRowCache {
		#region Fields
		readonly Dictionary<int, int> absoluteIndexToRelativeIndexTranslationTable = new Dictionary<int, int>();
		int lastRelativeIndex;
		int firstRelativeDataIndex;
		int lastRelativeDataIndex;
		#endregion
		public Dictionary<int, int> InnerTable { get { return absoluteIndexToRelativeIndexTranslationTable; } }
		#region ITableRelativeColumnRowAccessor Members
		public int LastRelativeIndex { get { return lastRelativeIndex; } set { lastRelativeIndex = value; } }
		public int FirstRelativeDataIndex { get { return firstRelativeDataIndex; } set { firstRelativeDataIndex = value; } }
		public int LastRelativeDataIndex { get { return lastRelativeDataIndex; } set { lastRelativeDataIndex = value; } }
		public void RegisterRelativeIndex(int absoluteIndex, int relativeIndex) {
			absoluteIndexToRelativeIndexTranslationTable.Add(absoluteIndex, relativeIndex);
		}
		public int GetRelativeIndex(int absoluteIndex) {
			return absoluteIndexToRelativeIndexTranslationTable[absoluteIndex];
		}
		public bool HasVisibleColumnRowIndex(int absoluteIndex) {
			return absoluteIndexToRelativeIndexTranslationTable.ContainsKey(absoluteIndex);
		}
		public bool ContainsAbsoluteDataIndex(int absoluteDataIndex) {
			int relativeDataIndex = GetRelativeIndex(absoluteDataIndex);
			return relativeDataIndex >= FirstRelativeDataIndex && relativeDataIndex <= LastRelativeDataIndex;
		}
		#endregion
	}
	#endregion
	#region TableAbsoluteColumnsRowsCache
	public class TableAbsoluteColumnsRowsCache : ITableRelativeColumnRowCache {
		#region Fields
		readonly int firstIndex;
		int lastRelativeIndex;
		int firstRelativeDataIndex;
		int lastRelativeDataIndex;
		#endregion
		public TableAbsoluteColumnsRowsCache(int firstIndex) {
			this.firstIndex = firstIndex;
		}
		#region ITableRelativeColumnRowAccessor Members
		public int LastRelativeIndex { get { return lastRelativeIndex; } set { lastRelativeIndex = value; } }
		public int FirstRelativeDataIndex { 
			get { return firstRelativeDataIndex; } 
			set { firstRelativeDataIndex = value; } 
		}
		public int LastRelativeDataIndex { 
			get { return lastRelativeDataIndex; } 
			set { lastRelativeDataIndex = value; } 
		}
		public bool ContainsAbsoluteDataIndex(int absoluteDataIndex) {
			int relativeDataIndex = GetRelativeIndex(absoluteDataIndex);
			return relativeDataIndex >= FirstRelativeDataIndex && relativeDataIndex <= LastRelativeDataIndex;
		}
		public int GetRelativeIndex(int absoluteIndex) {
			return absoluteIndex - firstIndex;
		}
		public void RegisterRelativeIndex(int absoluteIndex, int relativeIndex) {
		}
		public bool HasVisibleColumnRowIndex(int absoluteIndex) {
			return true;
		}
		#endregion
	}
	#endregion
} 
