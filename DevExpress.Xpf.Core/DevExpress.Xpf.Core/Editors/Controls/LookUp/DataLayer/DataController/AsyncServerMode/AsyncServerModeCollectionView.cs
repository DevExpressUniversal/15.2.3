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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
namespace DevExpress.Xpf.Editors.Helpers {
	public class AsyncServerModeCollectionView : CollectionView, IServerModeCollectionView {
		readonly AsyncVisibleListWrapper asyncVisibleListWrapper;
		AsyncVisibleListWrapper Wrapper { get { return asyncVisibleListWrapper; } }
		new ICollectionView SourceCollection { get { return this.asyncVisibleListWrapper; } }
		public AsyncServerModeCollectionView(AsyncVisibleListWrapper asyncVisibleListWrapper) : base(Enumerable.Empty<object>()) {
			this.asyncVisibleListWrapper = asyncVisibleListWrapper;
			asyncVisibleListWrapper.Initialize();
			asyncVisibleListWrapper.CollectionChanged += OnVisibleListCollectionChanged;
		}
		public override bool IsEmpty { get { return SourceCollection.IsEmpty; } }
		public override object CurrentItem { get { return SourceCollection.CurrentItem; } }
		public override int IndexOf(object item) {
			object value = Wrapper.GetValueFromItem(item);
			return IndexOfValue(value);
		}
		public int IndexOfValue(object value) {
			return Wrapper.IndexOf(value);
		}
		public override bool CanFilter {
			get { return SourceCollection.CanFilter; }
		}
		public override bool CanGroup {
			get { return SourceCollection.CanGroup; }
		}
		public override bool CanSort {
			get { return SourceCollection.CanSort; }
		}
		public override IComparer Comparer {
			get { return null; }
		}
		public override bool Contains(object item) {
			return Wrapper.Contains(item);
		}
		public override int Count {
			get { return Wrapper.Count; }
		}
		public override int CurrentPosition {
			get { return SourceCollection.CurrentPosition; }
		}
		public override IDisposable DeferRefresh() {
			return SourceCollection.DeferRefresh();
		}
		public override object GetItemAt(int index) {
			return Wrapper[index];
		}
		void OnVisibleListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			OnCollectionChanged(e);
		}
		public void FetchItem(int visibleIndex) {
			Wrapper.FetchItem(visibleIndex);			
		}
		public void CancelItem(int visibleIndex) {
			Wrapper.CancelItem(visibleIndex);
		}
		public bool IsTempItem(int visibleIndex) {
			return Wrapper.IsTempItem(visibleIndex);
		}
		public object GetItem(int visibleIndex) {
			return asyncVisibleListWrapper[visibleIndex];
		}
	}
}
