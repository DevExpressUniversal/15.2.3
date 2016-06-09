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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	internal class EditorModificationObserver {
		private WinColumnsListEditor editor;
		private IViewModificationObserver viewModificationObserver = null;
		public EditorModificationObserver(WinColumnsListEditor editor) {
			this.editor = editor;
		}
		public void Attach() {
			if(editor != null && editor.ColumnView != null) {
				if(editor.DataSource != null) {
					editor.Grid.DataSourceChanged += Grid_DataSourceChanged;
				}
				else {
					AttachViewModificationObserver();
				}
				editor.FilterChanged += editor_FilterChanged;
			}
		}
		public void Detach() {
			if(editor != null) {
				editor.FilterChanged -= editor_FilterChanged;
			}
			DetachViewModificationObserver();
		}
		private void editor_FilterChanged(object sender, EventArgs e) {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		private void Grid_DataSourceChanged(object sender, EventArgs e) {
			((GridControl)sender).DataSourceChanged -= Grid_DataSourceChanged;
			AttachViewModificationObserver();
		}
		private void AttachViewModificationObserver() {
			DetachViewModificationObserver();
			if(editor.Grid.MainView is GridView) {
				viewModificationObserver = new GridViewModificationObserver<GridView>((GridView)editor.Grid.MainView);
				viewModificationObserver.Attach();
				viewModificationObserver.ViewChanged += viewModificationObserver_ViewChanged;
			}
		}
		private void DetachViewModificationObserver() {
			if(viewModificationObserver != null) {
				viewModificationObserver.ViewChanged -= viewModificationObserver_ViewChanged;
				viewModificationObserver.Detach();
				viewModificationObserver = null;
			}
		}
		private void viewModificationObserver_ViewChanged(object sender, EventArgs e) {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		public event EventHandler Changed;
	}
}
