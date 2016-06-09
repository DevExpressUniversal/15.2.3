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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Filter.Parser;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using DevExpress.Data.Filtering;
namespace DevExpress.XtraGrid.Filter {
	public class FilterCustomDialog2 : FilterCustomDialog {
		GridColumnCollection columns;
		ArrayList fieldList = new ArrayList();
		protected CheckEdit chb1, chb2;
		protected ComboBoxEdit cbo1, cbo2;
		#region Init
		public FilterCustomDialog2(GridColumn col, GridColumnCollection cols) : this(col, cols, false) {}
		public FilterCustomDialog2(GridColumn col, GridColumnCollection cols, bool isVisible) : base(col, false) {
			CreateCheckBox(ref chb1, piFirst,1);
			CreateCheckBox(ref chb2, piSecond,2);
			columns = cols;
			int dx = chb1.Width;
			this.Width += dx;
			e1.Left += dx;
			e2.Left += dx;
			foreach(GridColumn c in cols) {
				string s = c.FieldName;
				if(!c.Visible && !c.OptionsColumn.ShowInCustomizationForm) continue;
				if(s != null && s != "" && s != col.FieldName && c.GetTextCaption() != "" && (!isVisible || c.VisibleIndex != -1))
					fieldList.Add(c.GetTextCaption());
			}	
			CreateComboBox(ref cbo1, e1,1);
			CreateComboBox(ref cbo2, e2,2);
			SetFilter();
			chb1.TabIndex = 6;
			e1.TabIndex = 7;
			cbo1.TabIndex = 8;
			chb2.TabIndex = 26;
			e2.TabIndex = 27;
			cbo2.TabIndex = 28;
			SetLookAndFeel(CurrentColumn.View.ElementsLookAndFeel);
			UpdateEditorMenuManager(Controls);
		}
		#endregion
		#region Create
		private void CreateCheckBox(ref CheckEdit chb, ComboBoxEdit pi, int index) {
			chb = new CheckEdit();
			chb.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.CustomFilterDialog2FieldCheck);
			chb.Properties.AutoHeight = false;
			chb.Tag = pi.Tag;
			chb.CheckStateChanged += new EventHandler(FieldStatusChanged);
			chb.StyleController = layoutControlMain;
			chb.Name = "CheckBox" + index.ToString();
			layoutControlMain.BeginInit();
			XtraLayout.LayoutControlItem layoutItem = new XtraLayout.LayoutControlItem();
			layoutItem.Name = Guid.NewGuid().ToString();
			layoutItem.Control = chb;
			layoutItem.TextVisible = false;
			if(index == 1) {
				layoutControlMain.AddItem(layoutItem, layoutControlItem6, XtraLayout.Utils.InsertType.Left);
				emptySpaceItem6.Dispose();
			}
			if(index == 2) {
				layoutControlMain.AddItem(layoutItem, layoutControlItem7, XtraLayout.Utils.InsertType.Left);
				emptySpaceItem5.Dispose();
			}
			layoutItem.Parent = layoutControlItem6.Parent;
			layoutControlMain.EndInit();
			layoutItem.Width = layoutItem.MinSize.Width;
			layoutControlItem6.Width += 22;
			layoutControlItem7.Width += 22;
		}
		private void CreateComboBox(ref ComboBoxEdit cbo, Control c, int index) {
			cbo = new ComboBoxEdit();
			cbo.Location = c.Location;
			cbo.Size = c.Size;
			cbo.Properties.Items.AddRange(fieldList.ToArray());
			cbo.Visible = false;
			cbo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			cbo.StyleController = layoutControlMain;
			cbo.SelectedValueChanged += new System.EventHandler(comboBox_SelectedValueChanged);
			comboBox_SelectedValueChanged(cbo, null);
			cbo.BringToFront();
		}
		#endregion
		#region Changing
		private void FieldStatusChanged(object sender, EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			layoutControlMain.BeginUpdate();
			if(1.Equals(chb.Tag)) {
				layoutControlMain.Controls.Remove(layoutControlItem6.Control);
				layoutControlItem6.Control = chb.Checked ? cbo1 : e1;
			} else {
				layoutControlMain.Controls.Remove(layoutControlItem7.Control);
				layoutControlItem7.Control = chb.Checked ? cbo2 : e2;
			}
			layoutControlMain.EndUpdate();
			ChangeValues();
		}
		#endregion
		#region SetFilter
		string PropertyName(string name) {
			OperandProperty pr = CriteriaOperator.TryParse(name) as OperandProperty;
			if(!ReferenceEquals(pr, null))
				return pr.PropertyName;
			return name;
		}
		private string CaptionByField(string name) {
			if(columns[name] != null) return columns[name].GetTextCaption();
			return "";
		}
		private string FieldByCaption(string name) {
			foreach(GridColumn c in columns) {
				if(name == c.GetTextCaption()) {
					return c.FieldName;
				}
			}
			return "";
		}
		private string DisplayCaption(string s) {
			return "[" + s + "]";
		}
		private bool IsField(string name) {
			return fieldList.IndexOf(CaptionByField(PropertyName(name))) != -1;
		}
		private CheckEdit CheckBoxByName(object tag) {
			return (1.Equals(tag) ? chb1 : chb2);
		}
		private ComboBoxEdit ComboBoxByName(object tag) {
			return (1.Equals(tag) ? cbo1 : cbo2);
		}
		protected override bool SetEditorValue(ComboBoxEdit pi, BaseEdit e, CriteriaOperator operand) {
			if(base.SetEditorValue(pi, e, operand))
				return true;
			OperandProperty prop = operand as OperandProperty;
			if(!ReferenceEquals(prop, null)) {
				CheckBoxByName(pi.Tag).Checked = true;
				ComboBoxByName(pi.Tag).Text = CaptionByField(PropertyName(prop.PropertyName));
				return true;
			}
			return false;
		}
		protected override bool IsFilterExist(ComboBoxEdit pi, BaseEdit e) {
			if(pi.SelectedItem == null || pi.SelectedItem.Equals(DBNull.Value) || CurrentColumn.FieldName == "") return false;
			if(BlankSelected(pi)) return true;
			if(CheckBoxByName(pi.Tag) == null) return false;
			if(!CheckBoxByName(pi.Tag).Checked && e.EditValue == null) return false;	
			if(CheckBoxByName(pi.Tag).Checked && ComboBoxByName(pi.Tag).SelectedIndex < 0) return false;
			return true;
		}
		protected override CriteriaOperator GetFiltersCriterion() {
			return GroupOperator.Combine(GroupOperatorType,
				GetFilterCriterion(piFirst, e1, chb1.Checked, FieldByCaption(cbo1.Text)),
				GetFilterCriterion(piSecond, e2, chb2.Checked, FieldByCaption(cbo2.Text)));
		}
		#endregion
	}
}
