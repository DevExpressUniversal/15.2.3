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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Data.Summary;
using System.Windows;
using DevExpress.Xpf.Grid.Native;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Grid {
	public class FixedSummariesHelper {
		public IList<SummaryItemBase> FixedSummariesLeftCore {
			get { return fixedSummariesLeftCore; }
		}
		public IList<SummaryItemBase> FixedSummariesRightCore {
			get { return fixedSummariesRightCore; }
		}
		IList<SummaryItemBase> fixedSummariesLeftCore = new List<SummaryItemBase>();
		IList<SummaryItemBase> fixedSummariesRightCore = new List<SummaryItemBase>();
	}
	public interface ISummaryItemOwner : IEnumerable<SummaryItemBase>, INotifyCollectionChanged, IList, ISupportGetCachedIndex<SummaryItemBase> {
		void OnSummaryChanged(SummaryItemBase summaryItem, DependencyPropertyChangedEventArgs e);
		void BeginUpdate();
		void EndUpdate();
		void Add(SummaryItemBase item);
		void Remove(SummaryItemBase item);
		new SummaryItemBase this[int index] { get; }
	}
	public abstract class SummaryItemCollectionBase<T> : ObservableCollectionCore<T>, ISummaryItemOwner where T : SummaryItemBase, new() {
		readonly DataControlBase dataControl;
		readonly SummaryItemCollectionType collectionType;
		protected SummaryItemCollectionBase(DataControlBase dataControl, SummaryItemCollectionType collectionType) {
			this.dataControl = dataControl;
			this.collectionType = collectionType;
		}
		public override string ToString() {
			return string.Empty;
		}
		protected override void InsertItem(int index, T item) {
			base.InsertItem(index, item);
			item.Collection = this;
		}
		protected override void RemoveItem(int index) {
			this[index].Collection = null;
			base.RemoveItem(index);
		}
		protected internal List<T> GetActiveItems() {
			List<T> res = new List<T>();
			foreach(T item in this) {
				if(item.SummaryType != SummaryItemType.None) res.Add(item);
			}
			return res;
		}
		public T Add(SummaryItemType summaryType, string fieldName) {
			T res = new T();
			res.SummaryType = summaryType;
			res.FieldName = fieldName;
			Add(res);
			return res;
		}
		Locker originationElementCollectionChangedLocker = new Locker();
		void ISummaryItemOwner.OnSummaryChanged(SummaryItemBase summaryItem, DependencyPropertyChangedEventArgs e) {
			dataControl.GetDataControlOriginationElement().NotifyPropertyChanged(dataControl, e.Property,
				dc => {
					ISummaryItemOwner owner = collectionType == SummaryItemCollectionType.Group ? dc.GroupSummaryCore : dc.TotalSummaryCore;
					return CloneDetailHelper.SafeGetDependentCollectionItem(summaryItem, this, owner);
				},
				typeof(SummaryItemBase));
			originationElementCollectionChangedLocker.DoLockedAction(() =>
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
			);
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			originationElementCollectionChangedLocker.DoIfNotLocked(() =>
				dataControl.GetDataControlOriginationElement().NotifyCollectionChanged(dataControl,
					dc => collectionType == SummaryItemCollectionType.Total ? dc.TotalSummaryCore : dc.GroupSummaryCore,
					summaryItem => CloneDetailHelper.CloneElement<SummaryItemBase>((SummaryItemBase)summaryItem),
				e)
			);
			base.OnCollectionChanged(e);
		}
		int ISupportGetCachedIndex<SummaryItemBase>.GetCachedIndex(SummaryItemBase item) {
			return GetCachedIndex((T)item);
		}
		SummaryItemBase ISummaryItemOwner.this[int index] { get { return this[index]; } }
		#region IEnumerable<SummaryItemBase> Members
		IEnumerator<SummaryItemBase> IEnumerable<SummaryItemBase>.GetEnumerator() {
			foreach(SummaryItemBase item in this) {
				yield return item;
			}
		}
		void ISummaryItemOwner.Add(SummaryItemBase item) {
			Add((T)item);
		}
		void ISummaryItemOwner.Remove(SummaryItemBase item) {
			Remove((T)item);
		}
		#endregion
	}
	public enum SummaryItemCollectionType {
		Group,
		Total
	}
	public static class SummaryItemCollectionUpdater {
		public static void ClearColumnSummaries(SummaryItemCollectionType collectionType, IColumnCollection columns) {
			for(int i = 0; i < columns.Count; i++)
				GetActualSummaryItemCollection(columns[i], collectionType).Clear();
		}
		public static void Update(DataViewBase dataView, SummaryItemCollectionType collectionType, ISummaryItemOwner summaries, IColumnCollection columns) {
			if(dataView == null)
				return;
			if(collectionType == SummaryItemCollectionType.Total) {
				dataView.FixedSummariesHelper.FixedSummariesLeftCore.Clear();
				dataView.FixedSummariesHelper.FixedSummariesRightCore.Clear();
			}
			foreach(SummaryItemBase item in summaries) {
				string showInColumn = item.ActualShowInColumn;
				if(columns[showInColumn] != null) {
					RefreshSummariesByColumn(dataView, item, GetActualSummaryItemCollection(columns[showInColumn], collectionType), collectionType);
				}
				else {
					if(collectionType == SummaryItemCollectionType.Total && item.Visible)
						switch(item.Alignment) {
							case GridSummaryItemAlignment.Left: dataView.FixedSummariesHelper.FixedSummariesLeftCore.Add(item);
								break;
							case GridSummaryItemAlignment.Right: dataView.FixedSummariesHelper.FixedSummariesRightCore.Add(item);
								break;
						}
				}
			}
			UpdateSummariesIsLastProperty(dataView.FixedSummariesHelper.FixedSummariesLeftCore);
			UpdateSummariesIsLastProperty(dataView.FixedSummariesHelper.FixedSummariesRightCore);
		}
		static bool CanUseFixedTotalSummary(DataViewBase dataView, SummaryItemBase item) {
			if(item.SummaryType == SummaryItemType.Count)
				return true;
			if(String.IsNullOrWhiteSpace(item.FieldName))
				return false;
			return dataView.DataProviderBase.Columns[item.FieldName] != null;
		}
		static void UpdateSummariesIsLastProperty(IList<SummaryItemBase> totalSummariesCore) {
			if(totalSummariesCore.Count == 0)
				return;
			for(int index = 0; index < totalSummariesCore.Count - 1; index++)
				totalSummariesCore[index].IsLast = false;
			totalSummariesCore[totalSummariesCore.Count - 1].IsLast = true;
		}
		static IList<SummaryItemBase> GetActualSummaryItemCollection(ColumnBase column, SummaryItemCollectionType collectionType) {
			switch(collectionType) {
				case SummaryItemCollectionType.Group:
					return column.GroupSummariesCore;
				case SummaryItemCollectionType.Total:
					return column.TotalSummariesCore;
			}
			return null;
		}
		static void RefreshSummariesByColumn(DataViewBase dataView, SummaryItemBase item, IList<SummaryItemBase> columnSummaries, SummaryItemCollectionType collectionType) {
			if(!item.Visible || item.SummaryType == SummaryItemType.None) {
				columnSummaries.Remove(item);
				return;
			}
			if(collectionType == SummaryItemCollectionType.Group) {
				if(!columnSummaries.Contains(item))
					columnSummaries.Add(item);
				return;
			}
			switch(item.Alignment) {
				case GridSummaryItemAlignment.Left: dataView.FixedSummariesHelper.FixedSummariesLeftCore.Add(item);
					break;
				case GridSummaryItemAlignment.Right: dataView.FixedSummariesHelper.FixedSummariesRightCore.Add(item);
					break;
				default:
					if(!columnSummaries.Contains(item))
						columnSummaries.Add(item);
					break;
			}
		}
	}
}
