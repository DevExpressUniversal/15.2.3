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

using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Notifications.Win {
	public class WinNotificationsMessageListViewController : NotificationsMessageListViewController {
		private GridListEditor gridListEditor;
		private RepositoryItem protectedContentTextEdit;
		private void GridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
			Notification item = gridListEditor.GridView.GetRow(e.RowHandle) as Notification;
			SetRepositoryItem(item, e);
		}
		private RepositoryItem GetProtectedContentRepositoryItem() {
			if(protectedContentTextEdit == null) {
				protectedContentTextEdit = CreateProtectedContentRepositoryItem();
			}
			return protectedContentTextEdit;
		}
		private RepositoryItem CreateProtectedContentRepositoryItem() {
			RepositoryEditorsFactory repositoryFactory = gridListEditor.RepositoryFactory;
			IModelListView model = gridListEditor.Model;
			ITypeInfo objectTypeInfo = model.ModelClass.TypeInfo;
			return repositoryFactory.CreateRepositoryItem(true, model.Columns[0], objectTypeInfo.Type);
		}
		protected void SetRepositoryItem(Notification item, CustomRowCellEditEventArgs e) {
			if(NeedProtectedContent(item, e.Column.FieldName)) {
				e.RepositoryItem = GetProtectedContentRepositoryItem();
			}
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			if(Frame.GetController<GridListEditorMemberLevelSecurityController>() != null) {
				Frame.GetController<GridListEditorMemberLevelSecurityController>().Active["WinNotificationMessageListViewController"] = false;
			}
			gridListEditor = View.Editor as GridListEditor;
			if(gridListEditor != null) {
				gridListEditor.GridView.CustomRowCellEdit += new CustomRowCellEditEventHandler(GridView_CustomRowCellEdit);
			}
		}
		protected override void OnDeactivated() {
			if(gridListEditor != null && gridListEditor.GridView != null) {
				gridListEditor.GridView.CustomRowCellEdit -= new CustomRowCellEditEventHandler(GridView_CustomRowCellEdit);
			}
			base.OnDeactivated();
		}
	}
}
