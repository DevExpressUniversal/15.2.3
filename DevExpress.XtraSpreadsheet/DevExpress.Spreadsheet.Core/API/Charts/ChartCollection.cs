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
using System.ComponentModel;
using DevExpress.Office;
namespace DevExpress.Spreadsheet.Charts {
	public interface ChartCollection : ISimpleCollection<Chart> {
		Chart Add(ChartType chartType);
		Chart Add(ChartType chartType, Range range);
		bool Remove(Chart chart);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Chart chart);
		int IndexOf(Chart chart);
		Chart GetChartById(int id);
		IList<Chart> GetCharts(string chartName);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Collections;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Office.API.Internal;
	using DevExpress.XtraSpreadsheet.Utils;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.Utils;
	partial class NativeChartCollection : NativeObjectBase, ChartCollection {
		readonly NativeWorksheet worksheet;
		readonly List<NativeChart> innerList = new List<NativeChart>();
		public NativeChartCollection(NativeWorksheet worksheet) 
		: base() {
			this.worksheet = worksheet;
		}
		internal List<NativeChart> InnerList { get { return innerList; } }
		#region ChartCollection Members
		public Chart Add(ChartType chartType) {
			CheckValid();
			worksheet.DocumentModel.BeginUpdate();
			try {
				Model.CreateEmptyChartCommand command = new Model.CreateEmptyChartCommand(worksheet.ModelWorksheet, ApiErrorHandler.Instance, (Model.ChartType)chartType);
				if (!command.Execute())
					return null;
				worksheet.ModelWorksheet.InsertChart(command.Chart);
			}
			finally {
				worksheet.DocumentModel.EndUpdate();
			}
			return innerList[innerList.Count - 1];
		}
		public Chart Add(ChartType chartType, Range range) {
			CheckValid();
			Guard.ArgumentNotNull(range, "range");
			Range absoluteRange = range.GetRangeWithAbsoluteReference();
			Model.CellRange modelRange = ((NativeWorksheet)range.Worksheet).GetModelSingleRange(absoluteRange);
			worksheet.DocumentModel.BeginUpdate();
			try {
				Model.CreateChartCommand command = new Model.CreateChartCommand(worksheet.ModelWorksheet, ApiErrorHandler.Instance, (Model.ChartType)chartType, modelRange);
				if (!command.Execute())
					return null;
				worksheet.ModelWorksheet.InsertChart(command.Chart);
			}
			finally {
				worksheet.DocumentModel.EndUpdate();
			}
			return innerList[innerList.Count - 1];
		}
		public bool Remove(Chart chart) {
			CheckValid();
			int index = IndexOf(chart);
			if (index != -1)
				RemoveAt(index);
			return index != -1;
		}
		public void RemoveAt(int index) {
			CheckValid();
			NativeChart item = innerList[index];
			worksheet.RemoveNativeDrawing(item);
			if (item.IsValid) {
				item.IsValid = false;
				worksheet.ModelWorksheet.RemoveDrawing(item.ModelChart);
			}
		}
		public void Clear() {
			CheckValid();
			worksheet.DocumentModel.BeginUpdate();
			try {
				int count = this.Count;
				for (int i = count - 1; i >= 0; i--) {
					NativeChart item = innerList[i];
					worksheet.RemoveNativeDrawing(item);
					if (item.IsValid) {
						item.IsValid = false;
						worksheet.ModelWorksheet.RemoveDrawing(item.ModelChart);
					}
				}
			}
			finally {
				worksheet.DocumentModel.EndUpdate();
			}
		}
		public bool Contains(Chart chart) {
			return IndexOf(chart) != -1;
		}
		public int IndexOf(Chart chart) {
			CheckValid();
			NativeChart nativeChart = chart as NativeChart;
			if (nativeChart != null)
				return InnerList.IndexOf(nativeChart);
			return -1;
		}
		public Chart GetChartById(int id) {
			CheckValid();
			foreach (NativeChart item in innerList) {
				if (item.Id == id && item.IsValid)
					return item;
			}
			return null;
		}
		public IList<Chart> GetCharts(string chartName) {
			CheckValid();
			List<Chart> result = new List<Chart>();
			foreach (NativeChart item in innerList) {
				if (item.Name == chartName && item.IsValid)
					result.Add(item);
			}
			return result;
		}
		#endregion
		#region ISimpleCollection<Chart> Members
		public Chart this[int index] {
			get {
				CheckValid();
				return innerList[index]; 
			}
		}
		#endregion
		#region IEnumerable<Chart> Members
		public IEnumerator<Chart> GetEnumerator() {
			CheckValid();
			return new EnumeratorConverter<NativeChart, Chart>(innerList.GetEnumerator(), ConvertNativeChartToChart);
		}
		Chart ConvertNativeChartToChart(NativeChart item) {
			return item;
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			CheckValid();
			return innerList.GetEnumerator();
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			CheckValid();
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		public int Count {
			get {
				CheckValid();
				return innerList.Count; 
			}
		}
		public bool IsSynchronized {
			get {
				CheckValid();
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		public object SyncRoot {
			get {
				CheckValid();
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		#endregion
		internal void AddCore(NativeChart item) {
			innerList.Add(item);
		}
		internal void InsertCore(int index, NativeChart item) {
			innerList.Insert(index, item);
		}
		internal int IndexOf(NativeChart item) {
			return innerList.IndexOf(item);
		}
		internal void RemoveCore(NativeChart item) {
			innerList.Remove(item);
		}
		internal void ClearCore() {
			innerList.Clear();
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			foreach (NativeChart item in innerList)
				item.IsValid = value;
		}
	}
}
