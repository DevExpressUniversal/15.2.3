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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections;
using System.Reflection;
using DevExpress.XtraEditors.Controls;
using DevExpress.ExpressApp.Design;
namespace DevExpress.ExpressApp.Design {
	public partial class CheckListCollectionEditorForm : DevExpress.XtraEditors.XtraForm, ICollectionEditorForm {
		private IList dataSource;
		public CheckListCollectionEditorForm() {
			InitializeComponent();
		}
		public IList DataSource {
			get { return dataSource; }
			set {
				if(dataSource != value) {
					dataSource = value;
					AssignDataSourceToControl();
				}
			}
		}
		private void AssignDataSourceToControl() {
			itemList.Items.Clear();
			foreach(object obj in dataSource) {
				itemList.Items.Add(obj, GetDisplayText(obj));
			}
		}
		private string GetDisplayText(object container) {
			if(container is Type) {
				object[] displayNameAttributes = ((Type)container).GetCustomAttributes(typeof(DisplayNameAttribute), false);
				if(displayNameAttributes.Length > 0) {
					return ((DisplayNameAttribute)displayNameAttributes[0]).DisplayName;
				}
			}
			else {
				object[] defaultPropertyAttributes = container.GetType().GetCustomAttributes(typeof(DefaultPropertyAttribute), true);
				if(defaultPropertyAttributes.Length > 0) {
					PropertyInfo defaultProperty = container.GetType().GetProperty(((DefaultPropertyAttribute)defaultPropertyAttributes[0]).Name);
					if(defaultProperty != null) {
						return defaultProperty.GetValue(container, null).ToString();
					}
				}
			}
			return container.ToString();
		}
		public ICollection EditValue {
			get { return GetEditValue(itemList.CheckedItems); }
			set {
				if(value != null) {
					SetSelection(value);
				}
			}
		}
		private ICollection GetEditValue(BaseCheckedListBoxControl.CheckedItemCollection checkedItemCollection) {
			List<object> result = new List<object>();
			foreach(ListBoxItem item in checkedItemCollection) {
				result.Add(item.Value);
			}
			return result;
		}
		private void SetSelection(ICollection collection) {
			foreach(object obj in collection) {
				int index = GetItemByValueIndex(obj);
				if(index == -1) {
					throw new Exception(string.Format("The '{0}' element is not supported.", obj));
				}
				itemList.SetItemChecked(index, true);
			}
		}
		private int GetItemByValueIndex(object obj) {
			foreach(CheckedListBoxItem item in itemList.Items) {
				if(item.Value == obj) {
					return itemList.Items.IndexOf(item);
				}
			}
			return -1;
		}
		private void selectAllCheckBox_CheckedChanged(object sender, EventArgs e) {
			if(selectAllCheckBox.Checked) {
				itemList.CheckAll();
			}
			else {
				itemList.UnCheckAll();
			}
		}
		private void itemList_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			selectAllCheckBox.CheckedChanged -= new EventHandler(selectAllCheckBox_CheckedChanged);
			selectAllCheckBox.Checked = itemList.CheckedItems.Count == itemList.ItemCount;
			selectAllCheckBox.CheckedChanged += new EventHandler(selectAllCheckBox_CheckedChanged);
		}
	}
}
