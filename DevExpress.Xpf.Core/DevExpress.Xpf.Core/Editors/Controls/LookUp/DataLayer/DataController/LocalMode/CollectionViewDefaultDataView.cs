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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Settings;
namespace DevExpress.Xpf.Editors.Helpers {
	public class CollectionViewDefaultDataView : ListServerDataView, IItemsProviderCollectionViewSupport {
		new IEnumerable<DataProxy> View {
			get { return base.View as LocalDataProxyViewCache; }
		}
		public event EventHandler<ItemsProviderCurrentChangedEventArgs> CurrentChanged;
		readonly ICollectionViewHelper collectionViewHelper;
		readonly Func<bool> syncWithCurrent;
		readonly DataControllerICollectionViewSupport collectionViewSupport;
		readonly bool useCollectionView;
		public CollectionViewDefaultDataView(bool useCollectionView, Func<bool> syncWithCurrent, IListServer server, string valueMember, string displayMember, IEnumerable<GroupingInfo> groups, IEnumerable<SortingInfo> sorts,
			string filter)
			: base(server, valueMember, displayMember, groups, sorts, filter) {
			this.useCollectionView = useCollectionView;
			this.syncWithCurrent = syncWithCurrent;
			collectionViewHelper = (ICollectionViewHelper)server;
			collectionViewSupport = new DataControllerICollectionViewSupport(this);
		}
		ICollectionViewHelper IItemsProviderCollectionViewSupport.DataSync {
			get { return collectionViewHelper; }
		}
		ICollectionView IItemsProviderCollectionViewSupport.ListSource {
			get { return collectionViewHelper.Collection; }
		}
		bool IItemsProviderCollectionViewSupport.IsSynchronizedWithCurrentItem {
			get { return syncWithCurrent(); }
		}
		void IItemsProviderCollectionViewSupport.RaiseCurrentChanged(object currentItem) {
			if (CurrentChanged != null)
				CurrentChanged(this, new ItemsProviderCurrentChangedEventArgs(currentItem));
		}
		void IItemsProviderCollectionViewSupport.SetCurrentItem(object currentItem) {
			collectionViewSupport.SetCurrentItem(currentItem);
		}
		void IItemsProviderCollectionViewSupport.SyncWithCurrentItem() {
			collectionViewSupport.SyncWithCurrent();
		}
		protected override void DisposeInternal() {
			base.DisposeInternal();
			collectionViewSupport.Release();
			collectionViewHelper.FilterSortGroupInfoChanged -= CollectionViewHelperFilterSortGroupInfoChanged;
		}
		protected override void InitializeSource() {
			collectionViewHelper.AllowSyncSortingAndGrouping = false;
			collectionViewSupport.Initialize();
			if (useCollectionView)
				collectionViewHelper.Initialize();
			base.InitializeSource();
			collectionViewHelper.FilterSortGroupInfoChanged += CollectionViewHelperFilterSortGroupInfoChanged;
		}
		void CollectionViewHelperFilterSortGroupInfoChanged(object sender, CollectionViewFilterSortGroupInfoChangedEventArgs e) {
			RaiseInconsistencyDetected();
		}
		protected override void InitializeView(object source) {
			var enumerable = (IList)source;
			InitializeSource();
			SetView(new LocalDataProxyViewCache(DataAccessor, enumerable.Cast<object>().Select(x => DataAccessor.CreateProxy(x, -1))));
		}
		public override IEnumerator<DataProxy> GetEnumerator() {
			return View.GetEnumerator();
		}
		public override CurrentDataView CreateCurrentDataView(object handle, IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			return useCollectionView
				? CreateCollectionViewDataView(handle, groups, sorts, filterCriteria, displayFilterCriteria)
				: CreatePlainDataView(handle, groups, sorts, filterCriteria, displayFilterCriteria);
		}
		CurrentDataView CreateCollectionViewDataView(object handle, IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			bool createFiltered = GetIsCurrentViewFIltered(groups, sorts, filterCriteria) || !string.IsNullOrEmpty(displayFilterCriteria);
			return createFiltered 
				? (CurrentDataView)new CollectionViewCurrentFilteredSortedDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember, groups, sorts, filterCriteria, displayFilterCriteria) 
				: new CollectionViewCurrentDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember);
		}
		CurrentDataView CreatePlainDataView(object handle, IList<GroupingInfo> groups, IList<SortingInfo> sorts, string filterCriteria, string displayFilterCriteria) {
			bool createFiltered = sorts.If(x => x.Count > 0).ReturnSuccess() || groups.If(x => x.Count > 0).ReturnSuccess() || !string.IsNullOrEmpty(filterCriteria) || !string.IsNullOrEmpty(displayFilterCriteria);
			return createFiltered
				? (CurrentDataView)new LocalCurrentFilteredSortedDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember, groups, sorts, filterCriteria, displayFilterCriteria)
				: new LocalCurrentPlainDataView(this, handle, DataAccessor.ValueMember, DataAccessor.DisplayMember);
		}
		public override bool ProcessInconsistencyDetected() {
			return false;
		}
	}
}
