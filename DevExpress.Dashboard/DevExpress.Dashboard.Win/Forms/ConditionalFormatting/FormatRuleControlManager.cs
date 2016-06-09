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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DashboardWin.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class FormatRuleControlManager : DashboardUserControl, IFormatRuleManagerView {
		static void HideShowForm(Form form, bool isHidden) {
			form.AllowTransparency = isHidden;
			form.Enabled = !isHidden;
			form.Opacity = isHidden ? 0 : 1;
		}
		readonly Locker locker = new Locker();
		event ViewStateChangedEventHandler stateUpdated;
		event EventHandler filterDataItemChanged;
		event EventHandler calculatedByDataItemChanged;
		event FormatRuleEditingEventHandler editing;
		event FormatRuleMovingEventHandler moving;
		event FormatRuleDeletingEventHandler deleting;
		event FormatRuleEnablingEventHandler enabling;
		int FocusedRowIndex { get { return gridViewRules.FocusedRowHandle; } }
		IList<IFormatRuleView> Rules { get { return (IList<IFormatRuleView>)this.gridRules.DataSource; } }
		public FormatRuleControlManager() : base() {
			InitializeComponent();
			btnDown.Image = ImageHelper.GetImage("ConditionalFormatting.Down");
			btnUp.Image = ImageHelper.GetImage("ConditionalFormatting.Up");
			locker.Lock();
			layoutControl.BeginUpdate();
			try {
				repItemEnabled.EditValueChanging += OnEnabledChanging;
				repItemEnabled.CheckedChanged += OnEnabledChanged;
				cbFilterBy.SelectedIndexChanged += OnFilterDataItemSelectedIndexChanged;
				cbCalculateBy.SelectedIndexChanged += OnCalculatedByDataItemChanged;
			}
			finally {
				layoutControl.EndUpdate();
				locker.Unlock();
			}
#if DEBUGTEST
			layoutControl.AllowCustomization = true;
#endif
		}
		protected override IXtraResizableControl GetInnerIXtraResizableControl() {
			return layoutControl;
		}
		IFormatRuleView GetRuleView(int index) {
			return (IFormatRuleView)gridViewRules.GetRow(index);
		}
		void OnCalculatedByDataItemChanged(object sender, EventArgs e) {
			if(calculatedByDataItemChanged != null)
				calculatedByDataItemChanged(this, EventArgs.Empty);
		}
		void OnFilterDataItemSelectedIndexChanged(object sender, EventArgs e) {
			if(filterDataItemChanged != null)
				filterDataItemChanged(this, EventArgs.Empty);
		}
		void OnEnabledChanging(object sender, ChangingEventArgs e) {
			if(!Rules[FocusedRowIndex].IsValid) {
				XtraMessageBox.Show(LookAndFeel, Parent, 
					DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleInvalidRuleChanging), 
					DashboardWinLocalizer.GetString(DashboardWinStringId.FormatRuleInvalidRule),
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				e.Cancel = true;
			}
		}
		void OnEnabledChanged(object sender, EventArgs e) {
			CheckEdit checkEdit = (CheckEdit)sender;
			RaiseEnabling(FocusedRowIndex, checkEdit.Checked);
		}
		void OnDeleteClick(object sender, EventArgs e) {
			int index = FocusedRowIndex;
			RaiseDeleting(index);
			EnsureEnabled(FocusedRowIndex);
		}
		void OnUpClick(object sender, EventArgs e) {
			int index = FocusedRowIndex;
			int newIndex = index - 1;
			if(newIndex < 0) return;
			RaiseMoving(index, newIndex);
		}
		void OnDownClick(object sender, EventArgs e) {
			int index = FocusedRowIndex;
			int newIndex = index + 1;
			if(newIndex >= gridViewRules.RowCount) return;
			RaiseMoving(index, newIndex);
		}
		void OnGridRulesDoubleClick(object sender, EventArgs e) {
			OnEditRule();
		}
		void OnEditClick(object sender, EventArgs e) {
			OnEditRule();
		}
		void OnEditRule() {
			if(!btnEdit.Enabled) return;
			using(Form form = RaiseEditing(FocusedRowIndex)) {
				Form parentForm = Parent.FindForm();
				HideShowForm(parentForm, true);
				form.ShowDialog(parentForm);
				HideShowForm(parentForm, false);
				RaiseChanged();
			}
		}
		void EnsureEnabled(int index) {
			if(index == GridControl.InvalidRowHandle) {
				btnEdit.Enabled = false;
				btnDelete.Enabled = false;
				btnUp.Enabled = false;
				btnDown.Enabled = false;
			} else {
				btnEdit.Enabled = Rules[index].IsValid;
				btnDelete.Enabled = true;
				btnUp.Enabled = index > 0;
				btnDown.Enabled = index < gridViewRules.RowCount - 1;
			}
		}
		void gridViewRules_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			EnsureEnabled(e.FocusedRowHandle);
		}
		void RaiseChanged() {
			if(stateUpdated != null)
				stateUpdated(this, new ViewStateChangedEventArgs());
		}
		Form RaiseEditing(int index) {
			if(editing != null)
				return editing(this, new FormatRuleEditingEventArgs(GetRuleView(index)));
			else
				throw new ArgumentException("Cannot show edit rule form");
		}
		void RaiseMoving(int oldIndex, int newIndex) {
			gridViewRules.BeginUpdate();
			if(moving != null) {
				moving(this, new FormatRuleMovingEventArgs(GetRuleView(oldIndex), GetRuleView(newIndex)));
			}
			gridViewRules.FocusedRowHandle = newIndex;
			gridViewRules.EndUpdate();
		}
		void RaiseDeleting(int index) {
			if(deleting != null)
				deleting(this, new FormatRuleDeletingEventArgs(GetRuleView(index)));
		}
		void RaiseEnabling(int index, bool enabled) {
			if(enabling != null)
				enabling(this, new FormatRuleEnablingEventArgs(GetRuleView(index), enabled));
		}
		void SetBarManager(BarItem item) {
			item.Manager = barManager;
			BarSubItem subItem = item as BarSubItem;
			if(subItem != null)
				foreach(BarItemLink link in subItem.ItemLinks) {
					SetBarManager(link.Item);
				}
		}
		#region IFormatRuleBaseControlView Members
		event ViewStateChangedEventHandler IFormatRuleBaseControlView.StateUpdated {
			add { stateUpdated += value; }
			remove { stateUpdated -= value; }
		}
		bool IFormatRuleBaseControlView.IsValid { get { return true; } }
		void IFormatRuleBaseControlView.Initialize(IFormatRuleBaseViewInitializationContext initializationContext) {
			this.gridColumnEnabled.FieldName = "Enabled";
			this.gridColumnCaption.FieldName = "Caption";
			this.gridColumnCalculateBy.FieldName = "DataItemCaption";
			this.gridColumnApplyTo.FieldName = "DataItemApplyToCaption";
		}
		#endregion
		#region IFormatRuleManagerView Members
		void IFormatRuleManagerView.SetRules(IList<IFormatRuleView> rules) {
			this.gridRules.DataSource = rules;
			this.gridRules.Update();
			EnsureEnabled(FocusedRowIndex);
		}
		void IFormatRuleManagerView.SetFilterDataItems(IEnumerable items) {
			EditorsInitializer.FillComboItems(cbFilterBy, items);
		}
		void IFormatRuleManagerView.SetCalculatedByDataItems(IEnumerable items) {
			EditorsInitializer.FillComboItems(cbCalculateBy, items);
		}
		void IFormatRuleManagerView.ClearPopupMenuItems() {
			barManager.Items.Clear();
			popupMenuAddRule.ItemLinks.Clear();
		}
		void IFormatRuleManagerView.SetPopupMenuItems(IList<BarItem> items, bool addSeparator) {
			for(int i = 0; i < items.Count; i++) {
				BarItem item = items[i];
				SetBarManager(item);
				BarItemLink link = popupMenuAddRule.ItemLinks.Add(item);
				if(i == 0)
					link.BeginGroup = true;
			}
		}
		int IFormatRuleManagerView.SelectedFilterDataItemIndex {
			get { return this.cbFilterBy.SelectedIndex; }
			set { this.cbFilterBy.SelectedIndex = value; }
		}
		int IFormatRuleManagerView.SelectedCalculatedByDataItemIndex {
			get { return this.cbCalculateBy.SelectedIndex; }
			set { this.cbCalculateBy.SelectedIndex = value; }
		}
		event EventHandler IFormatRuleManagerView.FilterDataItemChanged {
			add { filterDataItemChanged += value; }
			remove { filterDataItemChanged -= value; }
		}
		event EventHandler IFormatRuleManagerView.CalculatedByDataItemChanged {
			add { calculatedByDataItemChanged += value; }
			remove { calculatedByDataItemChanged -= value; }
		}
		event FormatRuleEditingEventHandler IFormatRuleManagerView.Editing {
			add { editing += value; }
			remove { editing -= value; }
		}
		event FormatRuleMovingEventHandler IFormatRuleManagerView.Moving {
			add { moving += value; }
			remove { moving -= value; }
		}
		event FormatRuleDeletingEventHandler IFormatRuleManagerView.Deleting {
			add { deleting += value; }
			remove { deleting -= value; }
		}
		event FormatRuleEnablingEventHandler IFormatRuleManagerView.Enabling {
			add { enabling += value; }
			remove { enabling -= value; }
		}
		#endregion
	}
}
