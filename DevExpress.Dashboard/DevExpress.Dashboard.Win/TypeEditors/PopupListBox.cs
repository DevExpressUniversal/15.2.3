#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections;
using System.Drawing;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin.Native {
	class PopupListBox : ListBoxControl {
		bool compareByType = false;
		public object SelectedItemValue {
			get {
				ListItem item = SelectedItem as ListItem;
				return item != null ? item.Value : null;
			}
		}
		public PopupListBox(ICollection collection, object currentValue, bool compareByType, IWindowsFormsEditorService editServ) {
			this.compareByType = compareByType;
			PopulateItems(collection, currentValue);
			Click += new EventHandler(delegate(object sender, EventArgs e) {
				editServ.CloseDropDown();
			});
			BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
		}
		void PopulateItems(ICollection collection, object selectedValue) {
			Items.Clear();
			int selectedIndex = 0;
			foreach(ListItem listItem in collection) {
				int index = Items.Add(listItem);
				if(IsEqual(listItem.Value, selectedValue))
					selectedIndex = index;
			}
			SelectedIndex = ItemCount > 0 ? selectedIndex : -1;
			int displayCount = Math.Min(ItemCount, 10);
			Size = new Size(ClientSize.Width, displayCount * ViewInfo.ItemHeight + 10);
		}
		bool IsEqual(object value1, object value2) {
			if(value1 == null && value2 == null)
				return true;
			if(value1 == null || value2 == null)
				return false;
			if(compareByType) {
				return value1.GetType() == value2.GetType();
			} else {
				return value1.Equals(value2);
			}
		}
	}
}
