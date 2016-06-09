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
using DevExpress.XtraGrid;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using DevExpress.ExpressApp.SystemModule;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class GridListEditorController : ListViewControllerBase {
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			ITypeInfo objType = XafTypesInfo.Instance.FindTypeInfo(e.Object.GetType());
			if(View != null && (objType == View.ObjectTypeInfo)) {
				WinColumnsListEditor gridEditor = View.Editor as WinColumnsListEditor;
				if((gridEditor != null) && (gridEditor.Grid != null) && (gridEditor.DataSource != null) && !gridEditor.Grid.ContainsFocus && !gridEditor.ColumnView.IsEditing) {
					Int32 objectIndex = gridEditor.List.IndexOf(e.Object);
					if(objectIndex >= 0) {
						Int32 rowHandle = gridEditor.ColumnView.GetRowHandle(objectIndex);
						if(rowHandle != GridControl.NewItemRowHandle) {
							gridEditor.ColumnView.RefreshRow(rowHandle);
						}
					}
				}
			}
		}
		protected override void OnDeactivated() {
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			base.OnDeactivated();
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
		}
	}
}
