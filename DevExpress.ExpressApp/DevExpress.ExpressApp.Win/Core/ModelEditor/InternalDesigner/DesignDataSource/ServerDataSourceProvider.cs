#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class ServerDataSourceProvider : IListServer, ITypedList {
		private PropertyDescriptorProvider propertyDescriptorProvider;
		private List<FakeObject> innerObjects;
		private List<FakeObject> notSortedInnerObjects;
		ServerModeGroupHelper listSourceGroupHelper;
		public ServerDataSourceProvider(string targetType, string listName, IServiceProvider serviceProvider, bool showCollectionProperty, ColumnView columnView, List<FakeObject> fakeObjects) {
			propertyDescriptorProvider = new DesignerPropertyDescriptorProvider(targetType, listName, serviceProvider, showCollectionProperty);
			if(fakeObjects != null) {
				innerObjects = fakeObjects;
				notSortedInnerObjects = fakeObjects;
			}
			else {
				innerObjects = new List<FakeObject>();
				notSortedInnerObjects = new List<FakeObject>();
			}
			if(columnView != null) {
				this.listSourceGroupHelper = new ServerModeGroupHelper(columnView);
			}
		}
		private void SimpleSort(ICollection<ServerModeOrderDescriptor> sortInfo) {
			if(sortInfo != null && sortInfo.Count > 0) {
				IEnumerator<ServerModeOrderDescriptor> enumerator = sortInfo.GetEnumerator();
				enumerator.MoveNext();
				ServerModeOrderDescriptor first = enumerator.Current;
				if(first.IsDesc) {
					innerObjects.Reverse();
				}
				else {
					innerObjects = new List<FakeObject>(notSortedInnerObjects);
				}
			}
		}
		#region IListServer Members
		void IListServer.Apply(Data.Filtering.CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			SimpleSort(sortInfo);
			if(listSourceGroupHelper != null) {
				listSourceGroupHelper.Apply(groupSummaryInfo, totalSummaryInfo, innerObjects);
			}
		}
		event EventHandler<ServerModeExceptionThrownEventArgs> IListServer.ExceptionThrown {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		int IListServer.FindIncremental(Data.Filtering.CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			throw new NotImplementedException();
		}
		System.Collections.IList IListServer.GetAllFilteredAndSortedRows() {
			throw new NotImplementedException();
		}
		List<ListSourceGroupInfo> IListServer.GetGroupInfo(ListSourceGroupInfo parentGroup) {
			if(listSourceGroupHelper != null) {
				return listSourceGroupHelper.GetGroupInfo(parentGroup);
			}
			else {
				return new List<ListSourceGroupInfo>();
			}
		}
		int IListServer.GetRowIndexByKey(object key) {
			throw new NotImplementedException();
		}
		object IListServer.GetRowKey(int index) {
			return innerObjects[index].Key;
		}
		List<object> IListServer.GetTotalSummary() {
			if(listSourceGroupHelper != null) {
				return listSourceGroupHelper.TotalGroupInfo.Summary;
			}
			else {
				return new List<object>();
			}
		}
		object[] IListServer.GetUniqueColumnValues(Data.Filtering.CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			throw new NotImplementedException();
		}
		event EventHandler<ServerModeInconsistencyDetectedEventArgs> IListServer.InconsistencyDetected {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		int IListServer.LocateByValue(Data.Filtering.CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			throw new NotImplementedException();
		}
		bool IListServer.PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, System.Threading.CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
		void IListServer.Refresh() {
			throw new NotImplementedException();
		}
		#endregion
		#region IList Members
		int System.Collections.IList.Add(object value) {
			throw new NotImplementedException();
		}
		void System.Collections.IList.Clear() {
			throw new NotImplementedException();
		}
		bool System.Collections.IList.Contains(object value) {
			throw new NotImplementedException();
		}
		int System.Collections.IList.IndexOf(object value) {
			throw new NotImplementedException();
		}
		void System.Collections.IList.Insert(int index, object value) {
			throw new NotImplementedException();
		}
		bool System.Collections.IList.IsFixedSize {
			get { throw new NotImplementedException(); }
		}
		bool System.Collections.IList.IsReadOnly {
			get { throw new NotImplementedException(); }
		}
		void System.Collections.IList.Remove(object value) {
			throw new NotImplementedException();
		}
		void System.Collections.IList.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		object System.Collections.IList.this[int index] {
			get {
				return innerObjects[index];
			}
			set {
			}
		}
		#endregion
		#region ICollection Members
		void System.Collections.ICollection.CopyTo(Array array, int index) {
			throw new NotImplementedException();
		}
		int System.Collections.ICollection.Count {
			get {
				return innerObjects.Count;
			}
		}
		bool System.Collections.ICollection.IsSynchronized {
			get { throw new NotImplementedException(); }
		}
		object System.Collections.ICollection.SyncRoot {
			get { throw new NotImplementedException(); }
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
		#endregion
		#region ITypedList Members
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return propertyDescriptorProvider.GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return propertyDescriptorProvider.GetListName(listAccessors);
		}
		#endregion
	}
}
