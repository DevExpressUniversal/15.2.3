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
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Collections;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid.Native;
#if SL
using DXFrameworkContentElement = DevExpress.Xpf.Core.DXFrameworkElement;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public interface IBandsOwner {
		void OnBandsChanged(NotifyCollectionChangedEventArgs e);
		void OnColumnsChanged(NotifyCollectionChangedEventArgs e);
		void OnLayoutPropertyChanged();
		IBandsCollection BandsCore { get; }
		List<BandBase> VisibleBands { get; }
		DataControlBase DataControl { get; }
		IBandsOwner FindClone(DataControlBase dataControl);
	}
	public interface IBandsCollection : IList, INotifyCollectionChanged, ILockable, ISupportGetCachedIndex<BandBase> {
	}
	public interface IBandColumnsCollection : IList, INotifyCollectionChanged, ILockable, ISupportGetCachedIndex<ColumnBase> { }
	public class BandCollection<TBand> : ObservableCollectionCore<TBand>, IBandsCollection where TBand : BandBase {
		int ISupportGetCachedIndex<BandBase>.GetCachedIndex(BandBase band) {
			return GetCachedIndex((TBand)band);
		}
	}
	public class BandColumnCollection<TColumn> : ObservableCollectionCore<TColumn>, IBandColumnsCollection where TColumn : ColumnBase {
		int ISupportGetCachedIndex<ColumnBase>.GetCachedIndex(ColumnBase column) {
			return GetCachedIndex((TColumn)column);
		}
	}
	public class BandIterator<TBand> : IEnumerator<TBand>, IEnumerable<TBand> where TBand : BandBase {
		Stack<TBand> stack = new Stack<TBand>();
		IBandsCollection startBands;
		public BandIterator(IBandsCollection bands) {
			startBands = bands;
		}
		#region IEnumerator Members
		TBand current;
		public TBand Current {
			get { return current; }
		}
		public bool MoveNext() {
			if(current == null && startBands != null)
				TraverseChildBands(startBands);
			else if(startBands != null)
				TraverseChildBands(current.BandsCore);
			UpdateCurrent();
			return current != null;
		}
		public void Reset() {
			stack.Clear();
			current = null;
		}
		void TraverseChildBands(IBandsCollection bands) {
			int count = bands.Count;
			for(int i = 0; i < count; i++) {
				TBand child = bands[(count - 1) - i] as TBand;
				if(child != null) stack.Push(child);
			}
		}
		void UpdateCurrent() {
			current = stack.Count > 0 ? stack.Pop() : null;
		}
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		#endregion
		#region IEnumerable<TBand> Members
		IEnumerator<TBand> IEnumerable<TBand>.GetEnumerator() {
			return this;
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			Reset();
		}
		#endregion
	}
	class BandsHelper {
		IBandsOwner owner;
		public BandsHelper(IBandsOwner owner, bool subscribe) {
			this.owner = owner;
			if(subscribe) owner.BandsCore.CollectionChanged += (d, e) => OnBandsChanged(e);
			OnBandsChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		internal void OnBandsChanged(NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				foreach(BandBase band in owner.BandsCore)
					band.Owner = owner;
			} else {
				if(e.OldItems != null) {
					foreach(BandBase band in e.OldItems)
						band.Owner = null;
				}
				if(e.NewItems != null) {
					foreach(BandBase band in e.NewItems)
						band.Owner = owner;
				}
			}
			DataControlBase dataControl = owner.DataControl;
			Action bandsChangedAction = () => owner.OnBandsChanged(e);
			if(dataControl != null) {
				dataControl.GetOriginationDataControl().syncPropertyLocker.DoLockedAction(bandsChangedAction);
				dataControl.GetDataControlOriginationElement().NotifyCollectionChanged(dataControl,
					dc => owner.FindClone(dc).BandsCore,
					band => BandsLayoutBase.CloneBand((BandBase)band),
					e);
			} else
				bandsChangedAction();
		}
	}
	class BandedViewSerializationHelper {
		Dictionary<string, BandBase> namedBands;
		Dictionary<string, ColumnBase> namedColumns;
		Dictionary<string, ColumnBase> fieldNamedColumns;
		public bool CanRemoveOldColumns { get; private set; }
		public bool CanAddNewColumns { get; private set; }
		public BandedViewSerializationHelper(DataControlBase dataControl) {
			namedBands = new Dictionary<string, BandBase>();
			namedColumns = new Dictionary<string, ColumnBase>();
			fieldNamedColumns = new Dictionary<string, ColumnBase>();
			CanRemoveOldColumns = dataControl.GetRemoveOldColumns();
			CanAddNewColumns = dataControl.GetAddNewColumns();
			SaveNamedElements(dataControl);
		}
		void SaveNamedElements(DataControlBase dataControl) {
			if(dataControl.BandsLayoutCore != null) {
				dataControl.BandsLayoutCore.ForeachBand((band) => {
					if(!String.IsNullOrWhiteSpace(band.Name))
						namedBands.Add(band.Name, band);
				});
			}
			foreach(ColumnBase column in dataControl.ColumnsCore) {
				if(!String.IsNullOrWhiteSpace(column.Name)) {
					namedColumns.Add(column.Name, column);
					continue;
				}
				if(!String.IsNullOrWhiteSpace(column.FieldName) && !fieldNamedColumns.ContainsKey(column.FieldName))
					fieldNamedColumns.Add(column.FieldName, column);
			}
		}
		public void ClearCollection(XtraItemRoutedEventArgs e) {
			IList items = e.Collection as IList;
			if(items == null)
				return;
			List<string> names = new List<string>();
			if(e.Item.ChildProperties != null) {
				foreach(XtraPropertyInfo item in e.Item.ChildProperties) {
					string name = GetSerializationName(item);
					if(!String.IsNullOrWhiteSpace(name))
						names.Add(name);
				}
			}
			for(int n = items.Count - 1; n >= 0; n--) {
				BaseColumn item = (BaseColumn)items[n];
				if(String.IsNullOrWhiteSpace(item.Name) || !names.Contains(item.Name))
					items.RemoveAt(n);
			}
		}
		public void FindColumn(XtraFindCollectionItemEventArgs e) {
			ColumnBase column = null;
			string name = GetSerializationName(e.Item);
			if(!String.IsNullOrWhiteSpace(name)) {
				namedColumns.TryGetValue(name, out column);
			}
			else {
				DataControlBase dataConrol = e.Owner as DataControlBase;
				if(dataConrol != null && dataConrol.UseFieldNameForSerialization) {
					string fieldName = GetSerializationFieldName(e.Item);
					if(!String.IsNullOrWhiteSpace(fieldName)) {
						fieldNamedColumns.TryGetValue(fieldName, out column);
					}
				}
			}
			MoveColumn(column, e.Collection as IList);
			e.CollectionItem = column;
		}
		public void FindBand(XtraFindCollectionItemEventArgs e, IBandsOwner newOwner) {
			BandBase band = null;
			string name = GetSerializationName(e.Item);
			if(!String.IsNullOrWhiteSpace(name)) {
				namedBands.TryGetValue(name, out band);
				ChangeBandsOwner(band, newOwner);
			}
			e.CollectionItem = band;
		}
		string GetSerializationName(XtraPropertyInfo item) {
			return GetSerializationProperty(item, "Name");
		}
		string GetSerializationFieldName(XtraPropertyInfo item) {
			return GetSerializationProperty(item, ColumnBase.FieldNamePropertyName);
		}
		string GetSerializationProperty(XtraPropertyInfo item, string propertyName) {
			if(item.ChildProperties == null) return null;
			string name = null;
			XtraPropertyInfo propertyInfo = item.ChildProperties[propertyName];
			if(propertyInfo != null && propertyInfo.Value != null) name = propertyInfo.Value.ToString();
			return name;
		}
		void ChangeBandsOwner(BandBase band, IBandsOwner newOwner) {
			if(band == null || newOwner == null || band.Owner == newOwner)
				return;
			band.Owner.BandsCore.Remove(band);
			newOwner.BandsCore.Add(band);
			band.Owner = newOwner;
		}
		void MoveColumn(ColumnBase column, IList collection) {
			if(column == null || collection == null)
				return;
			if(!collection.Contains(column)) {
				if(column.ParentBand != null)
					column.ParentBand.ColumnsCore.Remove(column);
				collection.Add(column);
			}
		}
	}
	internal class BandColumnDefinition : DependencyObject {
		public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(GridLength), typeof(BandColumnDefinition), new PropertyMetadata(new GridLength(1, GridUnitType.Star), (d, e) => ((BandColumnDefinition)d).OnWidthChanged()));
		public GridLength Width {
			get { return (GridLength)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		void OnWidthChanged() {
		}
	}
	internal class BandRowDefinition : DependencyObject {
		public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(GridLength), typeof(BandRowDefinition), new PropertyMetadata(new GridLength(1, GridUnitType.Star), (d, e) => ((BandRowDefinition)d).OnHeightChanged()));
		public GridLength Height {
			get { return (GridLength)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		void OnHeightChanged() {
		}
	}
	static class BandWalker {
		public static Func<DataControlBase, ColumnBase> CreateColumnCloneAccessor(ColumnBase column) {
			if(column == null || column.OwnerControl == null)
				return CreateEmptyAccessor<ColumnBase>();
			BandBase parentBand = column.ParentBand;
			if(parentBand == null)
				return CreateEmptyAccessor<ColumnBase>();
			int index = parentBand.ColumnsCore.GetCachedIndex(column);
			if(index < 0)
				return CreateEmptyAccessor<ColumnBase>();
			Func<DataControlBase, BandBase> bandCloneAccessor = CreateBandCloneAccessor(parentBand);
			return new Func<DataControlBase, ColumnBase>(dc => {
				BandBase bandClone = bandCloneAccessor(dc);
				return bandClone != null ? bandClone.ColumnsCore[index] as ColumnBase : null;
			});
		}
		public static Func<DataControlBase, BandBase> CreateBandCloneAccessor(BandBase band) {
			if(band == null || band.DataControl == null)
				return CreateEmptyAccessor<BandBase>();
			int[] indexes = GetIndexes(band);
			return new Func<DataControlBase, BandBase>(dc => GetBandByIndexes(dc, indexes));
		}
		static Func<DataControlBase, T> CreateEmptyAccessor<T>() where T : class {
			return dc => null;
		}
		static int[] GetIndexes(BandBase band) {
			List<int> indexes = new List<int>();
			BandBase currentBand = band;
			while(currentBand != null) {
				int index = -1;
				if(currentBand.Owner != null)
					index = currentBand.Owner.BandsCore.GetCachedIndex(currentBand);
				if(index < 0)
					return new int[0];
				indexes.Add(index);
				currentBand = currentBand.ParentBand;
			}
			return indexes.ToArray();
		}
		static BandBase GetBandByIndexes(DataControlBase dataControl, int[] indexes) {
			Stack<int> indexesStack = new Stack<int>(indexes);
			IBandsCollection bands = dataControl.BandsCore;
			BandBase result = null;
			while(indexesStack.Count > 0) {
				int index = indexesStack.Pop();
				result = (BandBase)bands[index];
				bands = result.BandsCore;
			}
			return result;
		}
	}
}
