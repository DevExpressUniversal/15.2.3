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
using System.Linq;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class FillCheckListBoxItemsDetailViewController : ObjectViewController<DetailView, ICheckedListBoxItemsProvider> {
		protected virtual void ConfigurePropertyEditor() {
			foreach(CheckedListBoxStringPropertyEditor editor in View.GetItems<CheckedListBoxStringPropertyEditor>()) {
				CheckedListBoxStringPropertyEditor.FillItems(editor.Control.Properties, ViewCurrentObject, editor.PropertyName);
			}
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			if(ViewCurrentObject != null) {
				ViewCurrentObject.ItemsChanged += new EventHandler(ViewCurrentObject_ItemsChanged);
			}
			ConfigurePropertyEditor();
		}
		private void ViewCurrentObject_ItemsChanged(object sender, EventArgs e) {
			ConfigurePropertyEditor();
		}
		protected override void OnDeactivated() {
			if(ViewCurrentObject != null) {
				ViewCurrentObject.ItemsChanged -= new EventHandler(ViewCurrentObject_ItemsChanged);
			}
			base.OnDeactivated();
		}
	}
	public class FillCheckListBoxItemsViewController : ViewController<ListView> {
		ICheckedListBoxItemsProvider provider = null;
		public FillCheckListBoxItemsViewController() {
			TargetViewNesting = Nesting.Nested;
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			PropertyCollectionSource source = View.CollectionSource as PropertyCollectionSource;
			if(source != null) {
				provider = ObjectSpace.GetObject(source.MasterObject) as ICheckedListBoxItemsProvider;
				if(provider != null) {
					provider.ItemsChanged += new EventHandler(provider_ItemsChanged);
					ConfigureGridColumns(provider);
				}
			}
		}
		protected virtual void ConfigureGridColumns(ICheckedListBoxItemsProvider provider) {
			GridListEditor gridListEditor = View.Editor as GridListEditor;
			if(gridListEditor != null) {
				ConfigureGridColumnsCore(gridListEditor.Columns, provider);
			}
		}
		protected void ConfigureGridColumnsCore(IList<ExpressApp.Editors.ColumnWrapper> columns, ICheckedListBoxItemsProvider provider) {
			foreach(XafGridColumnWrapper columnWrapper in columns) {
#pragma warning disable 0618
				if(typeof(CheckedListBoxPropertyEditor).IsAssignableFrom(columnWrapper.GridColumnInfo.Model.PropertyEditorType)) {
#pragma warning restore 0618
					CheckedListBoxStringPropertyEditor.FillItems(columnWrapper.Column.ColumnEdit as RepositoryItemCheckedComboBoxEdit, provider, columnWrapper.PropertyName);
					columnWrapper.Column.FilterMode = XtraGrid.ColumnFilterMode.DisplayText;
				}
			}
		}
		private void provider_ItemsChanged(object sender, EventArgs e) {
			ConfigureGridColumns((ICheckedListBoxItemsProvider)sender);
		}
		protected override void OnDeactivated() {
			if(provider != null) {
				provider.ItemsChanged -= new EventHandler(provider_ItemsChanged);
				provider = null;
			}
			base.OnDeactivated();
		}
	}
}
