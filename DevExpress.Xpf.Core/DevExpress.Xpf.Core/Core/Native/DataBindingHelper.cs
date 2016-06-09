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
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
using DevExpress.Data;
#endif
namespace DevExpress.Xpf.Core.Native {
	public static class DataBindingHelper {
		public static IList ExtractDataSourceFromCollectionView(IEnumerable source) {
			return ExtractDataSource(source, new IEnumerableListExtractionAlgorithm());
		}
		public static IList ExtractDataSource(object dataSource, bool allowLiveDataShaping = true) {
			return ExtractDataSource(dataSource, allowLiveDataShaping ? ItemPropertyNotificationMode.PropertyChanged : ItemPropertyNotificationMode.None);
		}
		public static IList ExtractDataSource(object dataSource, ItemPropertyNotificationMode itemPropertyNotificationMode) {
			return ExtractDataSource(dataSource, new ComplexListExtractionAlgorithm(itemPropertyNotificationMode));
		}
		public static IList ExtractDataSource(object dataSource, IListExtractionAlgorithm algorithm) {
			return ExtractDataSourceCore(dataSource, algorithm);
		}
		static IList ExtractDataSourceCore(object dataSource, IListExtractionAlgorithm algorithm) {
			if(algorithm == null)
				return null;
			IList result = algorithm.Extract(dataSource);
			if (result is BindingListAdapter)
				((BindingListAdapter)result).OriginalDataSource = dataSource;
			return result;
		}
	}
	public interface IRefreshable {
		void Refresh();
	}
	class EnumerableObservableWrapper<T> : List<T>, INotifyCollectionChanged, IWeakEventListener, IRefreshable {
		readonly IEnumerable enumerable;
		public EnumerableObservableWrapper(IEnumerable enumerable) {
			this.enumerable = enumerable;
			if (enumerable is INotifyCollectionChanged)
				CollectionChangedEventManager.AddListener((INotifyCollectionChanged)enumerable, this);
			Repopulate();
		}
		void Repopulate() {
			Clear();
			foreach(T obj in enumerable)
				Add(obj);
		}
		void OnInnerCollectionChanged(NotifyCollectionChangedEventArgs e) {
			Repopulate();
			if (CollectionChanged != null)
				CollectionChanged(this, e);
		}
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(CollectionChangedEventManager)) {
				OnInnerCollectionChanged((NotifyCollectionChangedEventArgs)e);
				return true;
			}
			return false;
		}
		void IRefreshable.Refresh() {
			Repopulate();
		}
	}
	public interface IListExtractionAlgorithm {
		IList Extract(object dataSource);
	}
	public abstract class ListExtractionAlgorithmBase : IListExtractionAlgorithm {
		#region static
		protected static BindingListAdapter WrapIEnumerable(IEnumerable enumerable, ItemPropertyNotificationMode itemPropertyNotificationMode = ItemPropertyNotificationMode.PropertyChanged) {
			return BindingListAdapter.CreateFromList(CreateEnumerableWrapper(enumerable), itemPropertyNotificationMode);
		}
		protected static IList CreateEnumerableWrapper(IEnumerable enumerable) {
			Type genericType = GenericTypeHelper.GetGenericTypeArgument(enumerable.GetType(), typeof(IEnumerable<>));
			if(genericType != null) {
				Type genericWrapper = typeof(EnumerableObservableWrapper<>).MakeGenericType(new Type[] { genericType });
				return (IList)Activator.CreateInstance(genericWrapper, enumerable);
			} else {
				return new EnumerableObservableWrapper<object>(enumerable);
			}
		}
		protected static object ExtractDataSourceFromProvider(DataSourceProvider provider) {
			if (provider == null)
				return null;
			provider.InitialLoad();
			return provider.Data;
		}
		protected static bool IsCollectionOfDictionaries(IEnumerable enumerable) {
			Type elementType = GenericTypeHelper.GetGenericTypeArgument(enumerable.GetType(), typeof(IEnumerable<>));
			return typeof(IDictionary).IsAssignableFrom(elementType);
		}
		#endregion
		public abstract IList Extract(object dataSource);
	}
	public class ComplexListExtractionAlgorithm : ListExtractionAlgorithmBase {
		readonly ItemPropertyNotificationMode itemPropertyNotificationMode;
		public ComplexListExtractionAlgorithm(ItemPropertyNotificationMode itemPropertyNotificationMode = ItemPropertyNotificationMode.PropertyChanged) {
			this.itemPropertyNotificationMode = itemPropertyNotificationMode;
		}
		public override IList Extract(object dataSource) {
			if (dataSource == null)
				return null;
			DataSourceProvider provider = dataSource as DataSourceProvider;
			if (provider != null)
				dataSource = GetDataSourceFromProvider(provider);
			IListSource listSource = dataSource as IListSource;
			if (listSource != null)
				return GetListFromListSource(listSource);
			ICollectionView collectionView = dataSource as ICollectionView;
#if !SL
			if(collectionView == null) {
				CompositeCollection compositeCollection = dataSource as CompositeCollection;
				if(compositeCollection != null)
					collectionView = CollectionViewSource.GetDefaultView(compositeCollection);
			}
#endif
			if (collectionView != null) {
				return GetListFromICollectionView(collectionView);
			}
			IList list = dataSource as IList;
			IEnumerable enumerableDataSource = dataSource as IEnumerable;
			if (enumerableDataSource != null && IsCollectionOfDictionaries(enumerableDataSource)) {
				return GetListFromDictionary(list, enumerableDataSource);
			}
			INotifyCollectionChanged notifyCollectionChanged = list as INotifyCollectionChanged;
			IBindingList bindingList = list as IBindingList;
			if (list != null && notifyCollectionChanged != null && bindingList == null)
				list = BindingListAdapter.CreateFromList(list, itemPropertyNotificationMode);
			if (list == null) {
				IEnumerable enumerable = dataSource as IEnumerable;
				if (enumerable != null) {
					list = GetListFromIEnumerable(enumerable);
				}
			}
			return ValidateList(list);
		}
		IList ValidateList(IList list) {
			if(itemPropertyNotificationMode != ItemPropertyNotificationMode.None && list != null && !(list is IBindingList) && list.Count > 0 && list[0] is INotifyPropertyChanged) { 
				return BindingListAdapter.CreateFromList(list);
			}
			return list;
		}
		protected virtual object GetDataSourceFromProvider(DataSourceProvider provider) {
			return ExtractDataSourceFromProvider(provider);
		}
		protected virtual IList GetListFromListSource(IListSource listSource) {
			return listSource.GetList();
		}
		protected virtual IList GetListFromICollectionView(ICollectionView collectionView) {
			return new ICollectionViewHelper(collectionView,
#if SILVERLIGHT
								new object()
#else
 CollectionView.NewItemPlaceholder
#endif
			);
		}
		protected virtual IList GetListFromDictionary(IList list, IEnumerable enumerableDataSource) {
			IList dictionariesList = list ?? CreateEnumerableWrapper(enumerableDataSource);
			return new DictionaryListAdapter(dictionariesList);
		}
		protected virtual IList GetListFromIEnumerable(IEnumerable enumerable) {
			return WrapIEnumerable(enumerable, itemPropertyNotificationMode);
		}
	}
	public class IEnumerableListExtractionAlgorithm : ListExtractionAlgorithmBase {
		public override IList Extract(object dataSource) {
			IEnumerable enumerable = dataSource as IEnumerable;
			return enumerable != null ? WrapIEnumerable((IEnumerable)dataSource) : null;
		}
	}
	public class WrappedICollectionViewListExtractionAlgorithm : ComplexListExtractionAlgorithm {
		protected override IList GetListFromICollectionView(ICollectionView collectionView) {
			return WrapIEnumerable(collectionView);
		}
	}
}
