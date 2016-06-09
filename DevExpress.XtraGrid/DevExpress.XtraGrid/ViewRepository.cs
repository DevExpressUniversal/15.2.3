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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Repository {
	[ListBindable(false)]
	public class ViewRepositoryCollection : CollectionBase, System.Collections.Generic.IEnumerable<BaseView> {
		ViewRepository repository;
		public ViewRepositoryCollection(ViewRepository repository) {
			this.repository = repository;
		}
		protected ViewRepository Repository { get { return repository; } }
		public void AddRange(BaseView[] views) {
			foreach(BaseView view in views) Add(view);
		}
		public int Add(BaseView view) {
			int index = List.IndexOf(view);
			if(index < 0) index = List.Add(view);
			return index;
		}
		public void Remove(BaseView view) {
			if(List.Contains(view)) List.Remove(view);
		}
		public BaseView this[int index] { get { return List[index] as BaseView; } }
		protected override void OnInsertComplete(int position, object item) {
			BaseView view = item as BaseView;
			view.SetViewRepository(Repository);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, view));
		}
		protected override void OnRemoveComplete(int position, object item) {
			BaseView view = item as BaseView;
			view.SetViewRepository(null);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, view));
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
			InnerList.Clear();
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(Repository != null) Repository.OnChanged(e);
		}
		internal void Dispose() {
			ArrayList list = new ArrayList(InnerList);
			Clear();
			foreach(BaseView view in list) view.Dispose();
		}
		System.Collections.Generic.IEnumerator<BaseView> System.Collections.Generic.IEnumerable<BaseView>.GetEnumerator() {
			foreach(BaseView view in InnerList)
				yield return view;
		}
	}
	[ToolboxItem(false)]
	public class ViewRepository : Component {
		public event CollectionChangeEventHandler Changed;
		ViewRepositoryCollection views;
		GridControl grid;
		public ViewRepository(GridControl grid) {
			this.grid = grid;
			this.views = CreateViewRepositoryCollection();
		}
		protected virtual ViewRepositoryCollection CreateViewRepositoryCollection(){
			return new ViewRepositoryCollection(this);
		}
		[Browsable(false)]
		public GridControl Grid { get { return grid; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ViewRepositoryCollection Views { get { return views; } }
		protected internal virtual void OnChanged(CollectionChangeEventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
			Views.Dispose();
			base.Dispose(disposing);
		}
	}
}
