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
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class ASPxGridListEditorMemberLevelSecurityController : ListViewControllerBase {
		private ASPxGridView grid;
		private void Unsubscribe() {
			if(grid != null) {
				grid.CustomColumnDisplayText -= Grid_CustomColumnDisplayText;
				grid = null;
			}
		}
		private void Grid_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
			if(e.Column.UnboundType == Data.UnboundColumnType.Bound && View.CollectionSource != null) { 
				object targetObject = e.Column.Grid.GetRow(e.VisibleRowIndex);
				if(targetObject != null && HasMember(targetObject, e.Column.FieldName, View.CollectionSource)) {
					if(!CanRead(targetObject, e.Column.FieldName, View.CollectionSource)) {
						e.DisplayText = Application.Model.ProtectedContentText;
					}
				}
			}
		}
		protected override bool CanReadCore(object targetObject, string objectHandle, string memberName) {
			List<string> visibleMembers = new List<string>();
			foreach(GridViewColumn column in grid.VisibleColumns) {
				visibleMembers.Add(column.Name.TrimEnd('!'));
			}
			if(!string.IsNullOrEmpty(grid.PreviewFieldName)) {
				visibleMembers.Add(grid.PreviewFieldName);
			}
			if(!visibleMembers.Contains(memberName)) {
				visibleMembers.Add(memberName);
			}
			return GetPermissionRequestsResult(targetObject, objectHandle, memberName, visibleMembers, View.CollectionSource);
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			grid = View.Editor.Control as ASPxGridView;
			if(grid != null) {
				grid.CustomColumnDisplayText += Grid_CustomColumnDisplayText;
			}
		}
		protected override void OnViewControlsDestroying() {
			Unsubscribe();
			base.OnViewControlsDestroying();
			}
		protected override void OnDeactivated() {
			Unsubscribe();
			base.OnDeactivated();
		}
	}
}
