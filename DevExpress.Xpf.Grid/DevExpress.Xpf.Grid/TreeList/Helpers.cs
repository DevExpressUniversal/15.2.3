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

using DevExpress.Xpf.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Data.Helpers;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Utils;
using System.Windows;
using System.Windows.Data;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
#else
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListFilterHelper {
		public TreeListFilterHelper(TreeListDataProvider provider) {
			DataProvider = provider;
		}
		protected TreeListDataProvider DataProvider { get; private set; }
		protected TreeListView View { get { return DataProvider.View; } }
		protected TreeListDataHelperBase DataHelper { get { return DataProvider.DataHelper; } }
		public virtual object[] GetUniqueColumnValuesCore(DataColumnInfo column, bool includeFilteredOut, bool roundDataTime, bool useDisplayText) {
			if(!DataProvider.IsReady) return null;
			object[] allValues = GetColumnValues(column, includeFilteredOut, roundDataTime, useDisplayText);
			if(allValues == null || allValues.Length < 2) return allValues;
			try {
				Array.Sort(allValues, DataProvider.ValueComparer);
			} catch {
			}
			int uniqCount = 0, count = allValues.Length;
			object[] uniqValues = new object[count];
			object lastValue = null, curValue;
			for(int n = 0; n < count; n++) {
				curValue = allValues[n];
				if(n == 0 || DataProvider.ValueComparer.Compare(curValue, lastValue) != 0) {
					uniqValues[uniqCount++] = curValue;
				}
				lastValue = curValue;
			}
			allValues = new object[uniqCount];
			Array.Copy(uniqValues, 0, allValues, 0, uniqCount);
			return allValues;
		}
		public virtual CriteriaOperator CalcColumnFilterCriteriaByValue(DataColumnInfo columnInfo, object columnValue, bool roundDateTime, bool useDisplayText) {
			Type columnType = useDisplayText ? typeof(string) : columnInfo.Type;
			OperandProperty property = new OperandProperty(columnInfo.Name);
			if(columnValue == null || columnValue is DBNull)
				return property.IsNull();
			Type underlyingType = Nullable.GetUnderlyingType(columnType);
			if(underlyingType != null)
				columnType = underlyingType;
			if(roundDateTime && columnType == typeof(DateTime)) {
				DateTime min = ConvertToDate(columnValue);
				min = new DateTime(min.Year, min.Month, min.Day);
				try {
					DateTime next = min.AddDays(1);
					return (property >= min) & (property < next);
				} catch {
					return property >= min;
				}
			}
			columnValue = CorrectFilterValueType(columnType, columnValue);
			return property == new OperandValue(columnValue);
		}
		protected IList<TreeListNode> GetNodes(bool includeFilteredOut, DataColumnInfo columnInfo) {
			List<TreeListNode> nodes = new List<TreeListNode>();
			foreach(TreeListNode node in new TreeListNodeIterator(DataProvider.Nodes, true)) {
				if((node.IsVisible || includeFilteredOut) && UserCustomNodeAccounted(node, columnInfo))
					nodes.Add(node);
			}
			return nodes;
		}
		bool UserCustomNodeAccounted(TreeListNode node, DataColumnInfo columnInfo) {
			return View.RaiseCustomFiterPopupList(node, columnInfo);
		}
		protected virtual object[] GetColumnValues(DataColumnInfo columnInfo, bool includeFilteredOut, bool roundDateTime, bool displayText) {
			if(!DataProvider.IsReady) return null;
			int realCount = 0;
			IList<TreeListNode> nodes = GetNodes(includeFilteredOut, columnInfo);
			bool isDateTimeColumn = !displayText && (columnInfo.Type.Equals(typeof(DateTime)) || columnInfo.Type.Equals(typeof(DateTime?)));
			int count = nodes.Count;
			object[] allValues = new object[count];
			for(int n = 0; n < nodes.Count; n++) {
				TreeListNode node = nodes[n];
				object val = DataHelper.GetValue(node, columnInfo.Name);
				if(displayText) val = View.GetNodeDisplayText(node, columnInfo.Name, val);
				if(val == null || val is System.DBNull || !(val is System.IComparable)) continue;
				if(isDateTimeColumn & roundDateTime && (val is DateTime)) {
					DateTime org = (DateTime)val;
					val = new DateTime(org.Year, org.Month, org.Day);
				}
				allValues[realCount++] = val;
			}
			if(realCount != count) {
				if(realCount == 0) return null;
				object[] res = new object[realCount];
				Array.Copy(allValues, 0, res, 0, realCount);
				return res;
			}
			return allValues;
		}
		public static DateTime ConvertToDate(object val) {
			DateTime res = DateTime.MinValue;
			if(val == null) return res;
			try {
				if(!(val is DateTime)) {
					res = DateTime.Parse(val.ToString());
				} else
					res = (DateTime)val;
			} catch { }
			return res;
		}
		public static object CorrectFilterValueType(Type columnFilteredType, object filteredValue) {
			if(filteredValue == null)
				return filteredValue;
			if(columnFilteredType == null)
				return filteredValue;
			Type underlyingFilteredType = Nullable.GetUnderlyingType(columnFilteredType);
			if(underlyingFilteredType != null)
				columnFilteredType = underlyingFilteredType;
			Type currentType = filteredValue.GetType();
			if(columnFilteredType.IsAssignableFrom(currentType))
				return filteredValue;
			try {
				return Convert.ChangeType(filteredValue, columnFilteredType, null);
			} catch { }
			return filteredValue;
		}
	}
	public class TreeListNodeValuesCache {
		TreeListDataProvider provider;
		object[] values = null;
		public TreeListNodeValuesCache(TreeListDataProvider provider) {
			this.provider = provider;
		}
		protected virtual TreeListNode CurrentNode { get { return Provider.CurrentNode; } }
		protected bool CanSave { get { return CurrentNode != null; } }
		protected bool CanRestore { get { return this.values != null && CanSave; } }
		protected object[] Values { get { return values; } }
		protected TreeListDataProvider Provider { get { return provider; } }
		public void Clear() {
			this.values = null;
		}
		public void SaveValues() {
			Clear();
			if(!CanSave) return;
			this.values = new object[Provider.Columns.Count];
			for(int n = 0; n < Provider.Columns.Count; n++) {
				DataColumnInfo column = Provider.Columns[n];
				if(column.ReadOnly) continue;
				Values[n] = Provider.DataHelper.GetValue(CurrentNode, column);
			}
		}
		public void RestoreValues() {
			try {
				if(!CanRestore) return;
				for(int n = 0; n < Provider.Columns.Count; n++) {
					DataColumnInfo column = Provider.Columns[n];
					if(column.ReadOnly) continue;
					object oldValue = Provider.DataHelper.GetValue(CurrentNode, column);
					object newValue = Values[n];
					bool notequal = false;
					try {
						if(!(oldValue is IComparable)) oldValue = null;
						if(!(newValue is IComparable)) newValue = null;
						notequal = Provider.ValueComparer.Compare(oldValue, newValue) != 0;
					} catch {
						continue;
					}
					if(notequal) Provider.DataHelper.SetValue(CurrentNode, column, Values[n]);
				}
			} catch { }
			Clear();
		}
	}
}
