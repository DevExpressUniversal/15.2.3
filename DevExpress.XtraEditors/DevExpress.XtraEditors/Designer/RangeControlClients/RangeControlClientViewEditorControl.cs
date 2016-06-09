#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Reflection;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.Design {
	[ToolboxItem(false)]
	public partial class RangeControlClientViewEditorControl : UserControl {
		readonly IWindowsFormsEditorService edSvc;
		readonly Type initialValue;
		readonly List<Type> values;
		Type editValue;
		public Type EditValue { get { return editValue; } }
		public RangeControlClientViewEditorControl(IWindowsFormsEditorService edSvc, Type initialValue) {
			InitializeComponent();
			this.edSvc = edSvc;
			this.initialValue = initialValue;
			this.editValue = initialValue;
			this.values = GetInheritedTypes(typeof(ChartRangeControlClientView));
			InitializeListBox();
		}
		void InitializeListBox() {
			viewTypeListBox.BeginUpdate();
			for (int i = 0; i < values.Count; i++) {
				Type item = values[i];
				viewTypeListBox.Items.Add(item.Name);
				if (initialValue == item)
					viewTypeListBox.SelectedIndex = i;
			}
			viewTypeListBox.EndUpdate();
		}
		List<Type> GetInheritedTypes(Type ancestorType) {
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			List<Type> result = new List<Type>();
			foreach (Type type in types) {
				if (ancestorType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
					result.Add(type);
			}
			return result;
		}
		void CloseDropDown() {
			edSvc.CloseDropDown();
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if (keyData == Keys.Enter) {
				CloseDropDown();
				return true;
			}
			if (keyData == Keys.Escape) {
				editValue = initialValue;
				CloseDropDown();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		void viewTypeListBox_MouseUp(object sender, MouseEventArgs e) {
			CloseDropDown();
		}
		void viewTypeListBox_SelectedIndexChanged(object sender, EventArgs e) {
			if (viewTypeListBox.SelectedIndex >= 0)
				editValue = values[viewTypeListBox.SelectedIndex]; ;
		}
	}
}
