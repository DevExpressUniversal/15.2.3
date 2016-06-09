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

using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Web.SystemModule {
	public class ASPxCheckedComboBoxController : ViewController<ListView> {
		private Dictionary<string, bool> suitableEditorCache = new Dictionary<string, bool>();
		private bool hasSuitableEditor = false;
		private void Grid_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
			ProcessCustomColumnDisplayText(e);
		}
		private void UnsubscribeGridEvents() {
			suitableEditorCache.Clear();
			hasSuitableEditor = false;
			ASPxGridListEditor gridListEditor = GetGridListEditor();
			if(gridListEditor != null) {
				gridListEditor.CreateCustomDataItemTemplate -= gridListEditor_CreateCustomDataItemTemplate;
				if(gridListEditor.Grid != null) {
					gridListEditor.Grid.CustomColumnDisplayText -= Grid_CustomColumnDisplayText;
				}
			}
		}
		private void gridListEditor_CreateCustomDataItemTemplate(object sender, CreateCustomDataItemTemplateEventArgs e) {
			ProcessCreateCustomDataItemTemplate(e);
		}
		private void ProcessCreateCustomDataItemTemplate(CreateCustomDataItemTemplateEventArgs e) {
			if(IsSuitablePropertyEditor(e.ModelColumn)) {
				e.Template = null;
				e.Handled = true;
				e.CreateDefaultDataItemTemplate = false;
				hasSuitableEditor = true;
			}
		}
		protected override void OnViewControlsCreated() {
			base.OnViewControlsCreated();
			ASPxGridListEditor gridListEditor = GetGridListEditor();
			if(gridListEditor != null) {
				gridListEditor.CreateCustomDataItemTemplate += gridListEditor_CreateCustomDataItemTemplate;
				gridListEditor.Grid.CustomColumnDisplayText += Grid_CustomColumnDisplayText;
			}
		}
		private void ProcessCustomColumnDisplayText(ASPxGridViewColumnDisplayTextEventArgs e) {
			if(hasSuitableEditor) {
				ASPxGridListEditor gridListEditor = GetGridListEditor();
				if(gridListEditor != null) {
					IDataColumnInfo columnInfo = GetColumnInfo(e.Column, gridListEditor);
					if(columnInfo != null && IsSuitablePropertyEditor(columnInfo.Model)) {
						ICheckedListBoxItemsProvider itemsProvider = GetListBoxItemsProvider(gridListEditor, e.VisibleRowIndex);
						if(itemsProvider != null) {
							e.DisplayText = CheckedListBoxItemsDisplayTextProvider.GetDisplayText(e.Value as string, e.Column.FieldName, itemsProvider);
							e.Column.Settings.FilterMode = ColumnFilterMode.DisplayText;
						}
					}
				}
			}
		}
		protected virtual IDataColumnInfo GetColumnInfo(GridViewDataColumn column, ASPxGridListEditor gridListEditor) {
			return gridListEditor.GetColumnInfo(column) as IDataColumnInfo;
		}
		protected virtual ASPxGridListEditor GetGridListEditor() {
			return View.Editor as ASPxGridListEditor;
		}
		protected virtual ICheckedListBoxItemsProvider GetListBoxItemsProvider(ASPxGridListEditor gridListEditor, int visibleRowIndex) {
			return gridListEditor.GetObjectByIndex(visibleRowIndex) as ICheckedListBoxItemsProvider;
		}
		protected override void OnViewControlsDestroying() {
			UnsubscribeGridEvents();
			base.OnViewControlsDestroying();
		}
		protected override void OnDeactivated() {
			UnsubscribeGridEvents();
			base.OnDeactivated();
		}
		private bool IsSuitablePropertyEditor(IModelColumn model) {
			bool result;
			string propertyName = model.PropertyName;
			if(!suitableEditorCache.TryGetValue(propertyName, out result)) {
				Type propertyEditorType = model.PropertyEditorType;
				result = typeof(ASPxCheckedListBoxStringPropertyEditor).IsAssignableFrom(propertyEditorType);
				suitableEditorCache[propertyName] = result;
			}
			return result;
		}
#if DebugTest
		public bool DebugTest_IsSuitablePropertyEditor(IModelColumn model) {
			return IsSuitablePropertyEditor(model);
		}
		public void DebugTest_ProcessCustomColumnDisplayText(ASPxGridViewColumnDisplayTextEventArgs e) {
			ProcessCustomColumnDisplayText(e);
		}
		public void DebugTest_ProcessCreateCustomDataItemTemplate(CreateCustomDataItemTemplateEventArgs e) {
			ProcessCreateCustomDataItemTemplate(e);
		}
		public bool DebugTest_HasSuitableEditor {
			get { return hasSuitableEditor; }
			set { hasSuitableEditor = value; }
		}
#endif
	}
}
