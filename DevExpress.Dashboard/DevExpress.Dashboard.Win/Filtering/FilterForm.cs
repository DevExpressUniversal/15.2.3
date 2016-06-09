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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.DashboardWin.Native {
	public partial class FilterForm : XtraForm {
		readonly FilterCriteriaEditorControl filterCriteriaEditor;
		protected FilterCriteriaEditorControl FilterCriteriaEditor { get { return filterCriteriaEditor; } }
		protected FilterForm() {
			InitializeComponent();
		}
		public FilterForm(IParameterCreator parameterCreator, IFilteredComponent filteredComponent, IList<IParameter> oldParameters, UserLookAndFeel lookAndFeel): this() {
			this.barAndDockingController.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			this.filterCriteriaEditor = CreateFilterCriteriaEditor(parameterCreator, filteredComponent, oldParameters, lookAndFeel);
			this.filterCriteriaEditor.Dock = DockStyle.Fill;
			this.filterCriteriaEditor.MinimumSize = this.filterControlPanel.MinimumSize;
			this.filterControlPanel.Controls.Add(this.filterCriteriaEditor);
			SubscribeEvents();
		}
		protected virtual FilterCriteriaEditorControl CreateFilterCriteriaEditor(IParameterCreator parameterCreator, IFilteredComponent filteredComponent, IList<IParameter> oldParameters, UserLookAndFeel lookAndFeel) {
			return new FilterCriteriaEditorControl(parameterCreator, filteredComponent, oldParameters, lookAndFeel);
		}
		protected virtual void SubscribeEvents() {
			FilterCriteriaEditor.CriteriaChanged += OnFilterEditorControlCriteriaChanged;
		}
		protected virtual void UnsubscribeEvents() {
			if(filterCriteriaEditor != null)
				FilterCriteriaEditor.CriteriaChanged -= OnFilterEditorControlCriteriaChanged;
		}
		protected virtual void ApplyChanges() {
			this.filterCriteriaEditor.ApplyFilterCriteria();
		}
		protected void UpdateApplyButtonEnabledState(bool isChanged) {
			btnApply.Enabled = isChanged;
		}
		void btnOK_Click(object sender, EventArgs e) {
			ApplyChanges();
			DialogResult = DialogResult.OK;
		}
		void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void btnApply_Click(object sender, EventArgs e) {
			ApplyChanges();
		}
		void OnFilterEditorControlCriteriaChanged(object sender, FilterCriteriaEditorCriteriaChangedEventArgs e) {
			UpdateApplyButtonEnabledState(e.IsChanged);
		}
		FilterColumn GetFirstSelectableNode(IEnumerable list) {
			foreach(FilterColumn parentColumn in list)
				if(parentColumn.Children == null)
					return parentColumn;
				else {
					FilterColumn column = GetFirstSelectableNode(parentColumn.Children);
					if(column != null)
						return column;
				}
			return null;
		}
	}
}
