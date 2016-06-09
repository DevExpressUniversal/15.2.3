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
using System.Text;
using DevExpress.ExpressApp.Win.SystemModule;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraEditors.Customization;
using DevExpress.XtraTreeList.Columns;
using DevExpress.ExpressApp.Localization;
using DevExpress.XtraTreeList.Painter;
using DevExpress.XtraTreeList;
using System.Drawing;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.TreeListEditors.Win {
	public class TreeListEditorColumnChooserController : ColumnChooserControllerBase {
		private ObjectTreeList objectTreeList;
		private TreeListEditor treeListEditor;
		private void UpdateRemoveButtonState(CustomizationListBoxBase listBox) {
			RemoveButton.Enabled = listBox.SelectedIndex > -1;
		}
		private void UnsubscribeFromEvents() {
			if(objectTreeList != null) {
				objectTreeList.ShowCustomizationForm -= new EventHandler(objectTreeList_ShowCustomizationForm);
				objectTreeList.HideCustomizationForm -= new EventHandler(objectTreeList_HideCustomizationForm);
				objectTreeList = null;
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			UnsubscribeFromEvents();
			treeListEditor = ((ListView)View).Editor as TreeListEditor;
			if(treeListEditor != null) {
				objectTreeList = treeListEditor.TreeList as ObjectTreeList;
				if(objectTreeList != null) {
					objectTreeList.ShowCustomizationForm += new EventHandler(objectTreeList_ShowCustomizationForm);
					objectTreeList.HideCustomizationForm += new EventHandler(objectTreeList_HideCustomizationForm);
				}
			}
		}
		private void objectTreeList_HideCustomizationForm(object sender, EventArgs e) {
			DeleteButtons();
			if(objectTreeList != null && objectTreeList.CustomizationForm != null && objectTreeList.CustomizationForm.ActiveListBox != null) {
				objectTreeList.CustomizationForm.ActiveListBox.KeyDown -= new KeyEventHandler(ActiveListBox_KeyDown);
				objectTreeList.CustomizationForm.ActiveListBox.SelectedIndexChanged -= new EventHandler(ActiveListBox_SelectedIndexChanged);
			}
		}
		private void objectTreeList_ShowCustomizationForm(object sender, EventArgs e) {
			InsertButtons();
			UpdateRemoveButtonState(objectTreeList.CustomizationForm.ActiveListBox);
			objectTreeList.CustomizationForm.ActiveListBox.KeyDown += new KeyEventHandler(ActiveListBox_KeyDown);
			objectTreeList.CustomizationForm.ActiveListBox.SelectedIndexChanged += new EventHandler(ActiveListBox_SelectedIndexChanged);
		}		
		private void ActiveListBox_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateRemoveButtonState(sender as CustomizationListBoxBase);
		}
		private void ActiveListBox_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				RemoveSelectedColumn();
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			UnsubscribeFromEvents();
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			this.treeListEditor = null;
		}
		protected override Control ActiveListBox {
			get { return (objectTreeList != null && objectTreeList.CustomizationForm != null) ? objectTreeList.CustomizationForm.ActiveListBox : null; }
		}
		protected override List<string> GetUsedProperties() {
			List<string> result = new List<string>();
			foreach(IModelColumn columnInfoNodeWrapper in treeListEditor.Model.Columns) {
				result.Add(columnInfoNodeWrapper.PropertyName);
			}
			return result;
		}
		protected override ITypeInfo DisplayedTypeInfo {
			get { return ((ListView)View).ObjectTypeInfo; }
		}
		protected override void AddColumn(string propertyName) {
			IModelListView modelListView = ((ListView)View).Model;
			IModelColumn columnInfo = FindColumnModelByPropertyName(propertyName);
			if(columnInfo == null) {
				columnInfo = modelListView.Columns.AddNode<IModelColumn>();
				columnInfo.Id = propertyName;
				columnInfo.PropertyName = propertyName;
				columnInfo.Index = -1;
				treeListEditor.AddColumn(columnInfo);
			}
			else {
				throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotAddDuplicateProperty, propertyName));
			}
		}
		protected IModelColumn FindColumnModelByPropertyName(string propertyName) {
			IModelColumn columnInfo = null;
			IModelListView modelListView = ((ListView)View).Model;
			foreach(IModelColumn colInfo in modelListView.Columns) {
				if(colInfo.PropertyName == propertyName) {
					columnInfo = colInfo;
					break;
				}
			}
			return columnInfo;
		}
		protected override void RemoveSelectedColumn() {
			if(objectTreeList.CustomizationForm.ActiveListBox.SelectedItem != null) {
				treeListEditor.RemoveColumn(new TreeListColumnWrapper((TreeListColumn)objectTreeList.CustomizationForm.ActiveListBox.SelectedItem));
			}
		}
		public TreeListEditorColumnChooserController()
			: base() {
			TargetViewType = ViewType.ListView;
		}
	}
}
