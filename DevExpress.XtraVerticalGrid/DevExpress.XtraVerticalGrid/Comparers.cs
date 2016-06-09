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

using System.Collections;
using DevExpress.XtraVerticalGrid.Rows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraVerticalGrid.ViewInfo;
using System;
using DevExpress.Utils;
namespace DevExpress.XtraVerticalGrid {
	class RowSortDescriptor {
		List<BaseRow> rows = new List<BaseRow>();
		public BaseRow this[int index] { get { return rows[index]; } }
		public int Length { get { return rows.Count; } }
		public void Add(BaseRow row) {
			rows.Add(row);
		}
		public override string ToString() {
			string result = "";
			foreach(BaseRow row in rows) {
				result += row.Properties.GetName() + "->";
			}
			return result;
		}
	}
	class PGridRowComparer : IComparer, IComparer<BaseRow> {
		bool Alphabetical { get; set; }
		bool IgnoreCategories { get; set; }
		Dictionary<BaseRow, RowSortDescriptor> sortDescriptorsCache = new Dictionary<BaseRow, RowSortDescriptor>();
		public PGridRowComparer(bool alphabetical, bool ignoreCategories) {
			Alphabetical = alphabetical;
			IgnoreCategories = ignoreCategories;
		}
		public int Compare(object x, object y) {
			return CompareCore((BaseRow)x, (BaseRow)y);
		}
		int IComparer<BaseRow>.Compare(BaseRow x, BaseRow y) {
			return CompareCore(x, y);
		}
		int CompareCore(BaseRow x, BaseRow y) {
			if(object.ReferenceEquals(x, y))
				return 0;
			RowSortDescriptor xDescriptor = GetRowSortDescriptor(x);
			RowSortDescriptor yDescriptor = GetRowSortDescriptor(y);
			return Compare(xDescriptor, yDescriptor);
		}
		int Compare(RowSortDescriptor x, RowSortDescriptor y) {
			int xEnd = x.Length - 1;
			int yEnd = y.Length - 1;
			int compareEnd = Math.Min(xEnd, yEnd);
			for(int i = 0; i <= compareEnd; i++) {
				BaseRow xc = x[xEnd - i];
				BaseRow yc = y[yEnd - i];
				if(IgnoreCategories && (xc.XtraRowTypeID == RowTypeIdConsts.CategoryRowTypeId || yc.XtraRowTypeID == RowTypeIdConsts.CategoryRowTypeId))
					continue;
				int res = CompareRows(xc, yc, i == 0);
				if(res != 0)
					return res;
			}
			return Comparer<int>.Default.Compare(xEnd, yEnd);
		}
		RowSortDescriptor GetRowSortDescriptor(BaseRow row) {
			RowSortDescriptor res;
			if(!sortDescriptorsCache.TryGetValue(row, out res)) {
				res = GetRowSortDescriptorCore(row);
				sortDescriptorsCache.Add(row, res);
			}
			return res;
		}
		RowSortDescriptor GetRowSortDescriptorCore(BaseRow row) {
			BaseRow current = row;
			RowSortDescriptor sortDescriptor = new RowSortDescriptor();
			while(current != null) {
				sortDescriptor.Add(current);
				current = current.ParentRow;
			}
			return sortDescriptor;
		}
		int CompareRows(BaseRow x, BaseRow y, bool compareFixedStyle) {
			if(object.ReferenceEquals(x, y))
				return 0;
			if(compareFixedStyle) {
				FixedStyle xStyle = x.Fixed;
				FixedStyle yStyle = y.Fixed;
				if(ComparerHelper.CompareStyle(xStyle, yStyle, FixedStyle.Top))
					return -1;
				if(ComparerHelper.CompareStyle(yStyle, xStyle, FixedStyle.Top))
					return 1;
				if(ComparerHelper.CompareStyle(xStyle, yStyle, FixedStyle.None))
					return -1;
				if(ComparerHelper.CompareStyle(yStyle, xStyle, FixedStyle.None))
					return 1;
			}
			int res = CompareCaptions(x, y);
			if(res != 0)
				return res;
			res = x.Rows == y.Rows ? Comparer<int>.Default.Compare(x.Index, y.Index) : 0;
			if (res != 0)
				return res;
			IRetrievable pGridRowX = x as IRetrievable;
			IRetrievable pGridRowY = y as IRetrievable;
			if (pGridRowX != null && pGridRowY != null) {
				res = Comparer<int>.Default.Compare(pGridRowX.RetrieveIndex, pGridRowY.RetrieveIndex);
			}
			return res;
		}
		static IComparer<string> stringComparer = new NaturalStringComparer();
		int CompareCaptions(BaseRow x, BaseRow y) {
			if (Alphabetical) {
				return stringComparer.Compare(x.Properties != null ? x.Properties.Caption : null, y.Properties != null ? y.Properties.Caption : null);
			}
			return 0;
		}
	}
	static class ComparerHelper {
		public static bool CompareStyle(FixedStyle style0, FixedStyle style1, FixedStyle style) {
			return style0 == style && style1 != style;
		}
	}
	class FixedRowsPaintOrderComparer : IComparer<BaseRow>, IComparer {
		public int Compare(BaseRow x, BaseRow y) {
			FixedStyle xStyle = x.Fixed;
			FixedStyle yStyle = y.Fixed;
			if(ComparerHelper.CompareStyle(xStyle, yStyle, FixedStyle.None))
				return -1;
			if(ComparerHelper.CompareStyle(yStyle, xStyle, FixedStyle.None))
				return 1;
			if(ComparerHelper.CompareStyle(xStyle, yStyle, FixedStyle.Bottom))
				return -1;
			if(ComparerHelper.CompareStyle(yStyle, xStyle, FixedStyle.Bottom))
				return 1;
			return Comparer.Default.Compare(x.Index, y.Index);
		}
		public int Compare(object x, object y) {
			return Compare((BaseRow)x, (BaseRow)y);
		}
	}
	class RowViewInfoComparerAdapter : IComparer<BaseRowViewInfo>, IComparer {
		IComparer<BaseRow> rowComparer;
		public RowViewInfoComparerAdapter(IComparer<BaseRow> rowComparer) {
			this.rowComparer = rowComparer;
		}
		public int Compare(BaseRowViewInfo x, BaseRowViewInfo y) {
			return rowComparer.Compare(GetRow(x), GetRow(y));
		}
		public int Compare(object x, object y) {
			return Compare((BaseRowViewInfo)x, (BaseRowViewInfo)y);
		}
		BaseRow GetRow(BaseRowViewInfo rowViewInfo) {
			return ((BaseRowViewInfo)rowViewInfo).Row;
		}
	}
}
