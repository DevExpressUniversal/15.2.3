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

using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardStandardComboBoxEdit : LookUpEdit, IFilterElementEditor {
		public event EventHandler ElementSelectionChanged;
		int IFilterElementEditor.Height { get { return Height; } }
		public DashboardStandardComboBoxEdit()
			: base() {
			Properties.ShowHeader = false;
			Properties.ShowFooter = false;
			Properties.ValueMember = FilterElementValuePropertyDescriptor.Member;
			Properties.DisplayMember = FilterElementDisplayTextPropertyDescriptor.Member;
			Properties.Columns.Add(new LookUpColumnInfo(FilterElementDisplayTextPropertyDescriptor.Member));
			Properties.NullText = String.Empty;
			Properties.DropDownRows = 7;
			Properties.UseDropDownRowsAsMaxCount = true;
			BorderStyle = BorderStyles.NoBorder;
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(ElementSelectionChanged != null)
				ElementSelectionChanged(this, EventArgs.Empty);
		}
		void IFilterElementEditor.SetSelection(IEnumerable<int> selection) {
			EditValue = selection.Count() == 0 ? null : (object)selection.First();
		}
		IEnumerable<int> IFilterElementEditor.GetSelection() {
			return new List<int> { (int)EditValue };
		}
		object IFilterElementEditor.DataSource { 
			get { return Properties.DataSource; } 
			set { Properties.DataSource = value; } 
		}
		int IFilterElementEditor.GetIndex(Point location) {
			return -1;
		}
	}
}
