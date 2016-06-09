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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class GridListEditorMemberLevelSecurityController : ListViewControllerBase {
		private GridView gridView;
		private CollectionSourceBase collectionSource;
		private GridListEditor gridListEditor;
		private RepositoryItem protectedContentTextEdit;
		protected override void OnActivated() {
			base.OnActivated();
			if(View.IsControlCreated) {
				Initialize();
			}
			else {
				View.ControlsCreated += new EventHandler(View_ControlsCreated);
			}
		}
		protected override void OnDeactivated() {
			if(gridView != null) {
				gridView.CustomRowCellEdit -= new CustomRowCellEditEventHandler(GridView_CustomRowCellEdit);
			}
			gridListEditor = null;
			gridView = null;
			collectionSource = null;
			protectedContentTextEdit = null;
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			base.OnDeactivated();
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			Initialize();
		}
		private void Initialize() {
			gridListEditor = View.Editor as GridListEditor;
			if(gridListEditor != null) {
				Initialize(gridListEditor.GridView, gridListEditor.CollectionSource);
			}
		}
		private void Initialize(GridView gridView, CollectionSourceBase collectionSource) {
			if(collectionSource != null) {
				this.gridView = gridView;
				this.gridView.CustomRowCellEdit += new CustomRowCellEditEventHandler(GridView_CustomRowCellEdit);
				this.collectionSource = collectionSource;
			}
		}
		private void GridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
			string fieldName = e.Column.FieldName;
			if(e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound
				&& !(ListHelper.GetList(gridView.GridControl.DataSource) is DevExpress.Data.Async.IAsyncListServer) 
				&& NeedProtectedContent(e.RowHandle, fieldName)
			) {
				e.RepositoryItem = GetProtectedContentRepositoryItem();
			}
		}
		private bool NeedProtectedContent(int rowHandle, string fieldName) {
			object rowObj = gridView.GetRow(rowHandle);
			return rowObj != null && HasMember(rowObj, fieldName, collectionSource) && !CanRead(rowObj, fieldName.TrimEnd('!'), collectionSource);
		}
		public virtual bool CanReadLocal(object rowObj, string fieldName) {
			if(rowObj != null && HasMember(rowObj, fieldName, collectionSource) && !CanRead(rowObj, fieldName.TrimEnd('!'), collectionSource)) {
				return false;
			}
			return true;
		}
		protected override bool CanReadCore(object targetObject, string objectHandle, string memberName) {
			List<string> visibleMembers = new List<string>();
			foreach(GridColumn column in gridView.VisibleColumns) {
				visibleMembers.Add(column.FieldName.TrimEnd('!'));
			}
			foreach(GridColumn column in gridView.GroupedColumns) {
				visibleMembers.Add(column.FieldName.TrimEnd('!'));
			}
			if(!string.IsNullOrEmpty(gridView.PreviewFieldName)) {
				visibleMembers.Add(gridView.PreviewFieldName);
			}
			if(!visibleMembers.Contains(memberName)) {
				visibleMembers.Add(memberName);
			}
			return GetPermissionRequestsResult(targetObject, objectHandle, memberName, visibleMembers, collectionSource);
		}
		private RepositoryItem GetProtectedContentRepositoryItem() {
			if(protectedContentTextEdit == null) {
				protectedContentTextEdit = CreateProtectedContentRepositoryItem();
			}
			return protectedContentTextEdit;
		}
		private RepositoryItem CreateProtectedContentRepositoryItem() {
			RepositoryItem result = null;
			if(CreateCustomProtectedContentRepositoryItem != null) {
				CreateCustomProtectedContentRepositoryItemEventArgs args = new CreateCustomProtectedContentRepositoryItemEventArgs();
				CreateCustomProtectedContentRepositoryItem(this, args);
				result = args.ProtectedContentRepositoryItem;
			}
			else {
				result = CreateDefaultProtectedContentRepositoryItem();
			}
			return result;
		}
		private RepositoryItem CreateDefaultProtectedContentRepositoryItem() {
			RepositoryEditorsFactory repositoryFactory = gridListEditor.RepositoryFactory;
			IModelListView model = gridListEditor.Model;
			ITypeInfo objectTypeInfo = model.ModelClass.TypeInfo;
			return repositoryFactory.CreateRepositoryItem(true, model.Columns[0], objectTypeInfo.Type);
		}
		public event EventHandler<CreateCustomProtectedContentRepositoryItemEventArgs> CreateCustomProtectedContentRepositoryItem;
#if DebugTest
		public void DebugTest_Initialize(GridView gridView, CollectionSourceBase collectionSource) {
			Initialize(gridView, collectionSource);
		}
#endif
	}
	public class CreateCustomProtectedContentRepositoryItemEventArgs : EventArgs {
		public RepositoryItem ProtectedContentRepositoryItem { get; set; }
	}
}
