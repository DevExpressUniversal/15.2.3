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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
#pragma warning disable 0618
	public class ASPxCheckedListBoxStringPropertyEditor : ASPxCheckedListBoxEditor {
#pragma warning restore 0618
		public ASPxCheckedListBoxStringPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
	}
	public class CheckedListBoxTemplate : ITemplate {
		private object currentObject;
		private string targetMemberName;
		public const string selectAllGuid = "DFD33816-6C2B-4A68-94AE-9C75C72248AC";
		public CheckedListBoxTemplate(object currentObject, string targetMemberName) {
			this.currentObject = currentObject;
			this.targetMemberName = targetMemberName;
		}
		private ASPxDropDownEdit GetDropDownEdit(Control container) {
			Control namingContainer = container.NamingContainer;
			while(namingContainer != null) {
				ASPxDropDownEdit edit = namingContainer as ASPxDropDownEdit;
				if(edit != null) {
					return edit;
				}
				namingContainer = namingContainer.NamingContainer;
			}
			return null;
		}
		public void InstantiateIn(Control container) {
			ASPxListBox listBox = new ASPxListBox();
			listBox.Items.Add(CaptionHelper.GetLocalizedText("Captions", "SelectAll", "(Select All)"), selectAllGuid);
			ICheckedListBoxItemsProvider listBoxItemsProvider = currentObject as ICheckedListBoxItemsProvider;
			if(listBoxItemsProvider != null) {
				Dictionary<Object, String> listBoxItems = listBoxItemsProvider.GetCheckedListBoxItems(targetMemberName);
				foreach(KeyValuePair<Object, String> pair in listBoxItems.OrderBy(key => key.Value)) {
					listBox.Items.Add(pair.Value, pair.Key);
				}
			}
			listBox.Width = Unit.Percentage(100);
			if(WebApplicationStyleManager.IsNewStyle) {
				listBox.Height = Unit.Pixel(250);
			}
			ASPxDropDownEdit edit = GetDropDownEdit(container);
			listBox.ClientSideEvents.SelectedIndexChanged = "function(s, e) {OnListBoxSelectionChanged(s, e, " + edit.ClientID + "); }";
			listBox.ID = "checkListBox";
			listBox.SelectionMode = ListEditSelectionMode.CheckColumn;
			container.Controls.Add(listBox);
			edit.ClientSideEvents.TextChanged = "function(s, e) {SynchronizeListBoxValues(s, e, " + listBox.ClientID + "); }";
			edit.JSProperties["cpCheckListBoxId"] = listBox.ClientID;
		}
	}
	#region Obsolete 14.2
	[Obsolete("Use the 'DevExpress.ExpressApp.Web.Editors.ASPx.ASPxCheckedListBoxStringPropertyEditor' class instead.")]
	[PropertyEditor(typeof(String), false)]
	public class ASPxCheckedListBoxEditor : ASPxPropertyEditor, ITestable {
		private ASPxDropDownEdit edit;
		public ASPxCheckedListBoxEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		protected override System.Web.UI.WebControls.WebControl CreateEditModeControlCore() {
			edit = new ASPxDropDownEdit();
			edit.ID = "comboBox";
			edit.DropDownWindowTemplate = new CheckedListBoxTemplate(CurrentObject, this.Model.PropertyName);
			edit.ValueChanged += new EventHandler(edit_ValueChanged);
			edit.ClientSideEvents.Init = "function(s){if (s.cpValuesToSelect) {window[s.cpCheckListBoxId].SelectValues(s.cpValuesToSelect.split(';')); UpdateSelectAllItemState(window[s.cpCheckListBoxId])}}";
			return edit;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(edit != null) {
				edit.ValueChanged -= new EventHandler(edit_ValueChanged);
				edit = null;
			}
		}
		protected internal override IJScriptTestControl GetEditorTestControlImpl() {
			return new JSASPxCheckedListBoxEditorTestControl();
		}
		void edit_ValueChanged(object sender, EventArgs e) {
			EditValueChangedHandler(this, EventArgs.Empty);
		}
		protected override void ReadEditModeValueCore() {
			if(PropertyValue != null) {
				ASPxDropDownEdit dropDownEdit = ((ASPxDropDownEdit)Editor);
				dropDownEdit.JSProperties["cpValuesToSelect"] = (String)PropertyValue;
				dropDownEdit.Text = CheckedListBoxItemsDisplayTextProvider.GetDisplayText((string)PropertyValue, Model.PropertyName, CurrentObject as ICheckedListBoxItemsProvider);
			}
		}
		protected override object GetControlValueCore() {
			string result = "";
			foreach(ListEditItem item in ((ASPxListBox)((ASPxDropDownEdit)Editor).FindControl("checkListBox")).SelectedItems) {
				if(item.Value.ToString() != CheckedListBoxTemplate.selectAllGuid) {
					result += item.Value + ";";
				}
			}
			return result.TrimEnd(';');
		}
	}
	#endregion
}
