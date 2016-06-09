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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Design;
using System.Text;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.Office.Design.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.rgLeader")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblLeader")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.btnSet")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.btnClear")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.btnClearAll")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblDefaultTabStops")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblTabStopPosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblTabStopsToBeCleared")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblSeparator")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.rgAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.lblRemoveTabStops")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.tabStopPositionEdit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.TabsForm.edtDefaultTabWidth")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class TabsForm : XtraForm {
		const int MinimumAllowableTabStopPostionDistance = 4; 
		readonly UIUnitConverter unitConverter;
		readonly List<int> removedTabStops;
		readonly TabsFormController controller;
		readonly IFormOwner tabsFormOwner;
		readonly DocumentUnit unitType;
		readonly DocumentModelUnitConverter valueUnitConverter;
		bool isClearAllHappend;
		TabsForm() {
			InitializeComponent();
		}
		public TabsForm(TabsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.unitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			this.removedTabStops = new List<int>();
			this.tabsFormOwner = controllerParameters.FormOwner;
			DocumentUnit unitType = controllerParameters.Control.InnerControl.Unit;
			this.unitType = (unitType == DocumentUnit.Document) ? DocumentUnit.Inch : unitType;
			this.valueUnitConverter = controllerParameters.UnitConverter;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		public TabsFormController Controller { get { return controller; } }
		protected internal DocumentUnit UnitType { get { return unitType; } }
		protected IFormOwner TabsFormOwner { get { return tabsFormOwner; } }
		protected internal DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } }
		void InitializeForm() {
			this.edtDefaultTabWidth.ValueUnitConverter = ValueUnitConverter;
			this.tabStopPositionEdit.ValueUnitConverter = ValueUnitConverter;
			this.edtDefaultTabWidth.Properties.MinValue = ParagraphFormDefaults.MinDefaultTabStopPosition;
			this.edtDefaultTabWidth.Properties.MaxValue = ParagraphFormDefaults.MaxTabStopPositionByDefault;
			this.edtDefaultTabWidth.Properties.DefaultUnitType = UnitType;
			this.tabStopPositionEdit.Properties.UnitType = UnitType;
		}
		protected internal virtual TabsFormController CreateController(TabsFormControllerParameters controllerParameters) {
			return new TabsFormController(controllerParameters);
		}
		void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		void SubscribeControlsEvents() {
			this.tabStopPositionEdit.EditValueChanged += OnTabStopPositionEditValueChanged;
			this.tabStopPositionEdit.SelectedIndexChanged += OnTabStopPositionEditSelectedIndexChanged;
			this.edtDefaultTabWidth.ValueChanged += OnEdtDefaultTabWidthValueChanged;
			this.rgAlignment.SelectedIndexChanged += OnRgAlignmentSelectedIndexChanged;
			this.rgLeader.SelectedIndexChanged += OnRgLeaderSelectedIndexChanged;
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			if (TabsFormOwner != null)
				TabsFormOwner.Hide();
		}
		void UnsubscribeControlsEvents() {
			this.tabStopPositionEdit.EditValueChanged -= OnTabStopPositionEditValueChanged;
			this.tabStopPositionEdit.SelectedIndexChanged -= OnTabStopPositionEditSelectedIndexChanged;
			this.edtDefaultTabWidth.ValueChanged -= OnEdtDefaultTabWidthValueChanged;
			this.rgAlignment.SelectedIndexChanged -= OnRgAlignmentSelectedIndexChanged;
			this.rgLeader.SelectedIndexChanged -= OnRgLeaderSelectedIndexChanged;
		}
		void OnTabStopPositionEditSelectedIndexChanged(object sender, EventArgs e) {
			UpdateForm();
		}
		void OnRgLeaderSelectedIndexChanged(object sender, EventArgs e) {
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null)
				return;
			UpdateTabInfoAlignmentAndLeader(tabInfo);
		}
		void OnRgAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null)
				return;
			UpdateTabInfoAlignmentAndLeader(tabInfo);
		}
		void UpdateTabInfoAlignmentAndLeader(TabInfo tabInfo) {
			TabFormattingInfo tabFormattingInfo = Controller.TabFormattingInfo;
			tabFormattingInfo.Remove(tabInfo);
			TabAlignmentType alignment = TabsFormHelper.GetAlignmentFromIndex(this.rgAlignment.SelectedIndex);
			TabLeaderType leader = TabsFormHelper.GetLeaderFromIndex(this.rgLeader.SelectedIndex);
			TabInfo newTabInfo = new TabInfo(tabInfo.Position, alignment, leader);
			tabFormattingInfo.Add(newTabInfo);
		}
		void OnTabStopPositionEditValueChanged(object sender, EventArgs e) {
			UpdateSetAndClearButtons();
		}
		void OnEdtDefaultTabWidthValueChanged(object sender, EventArgs e) {
			if (this.edtDefaultTabWidth.Value.HasValue)
				Controller.DefaultTabWidth = this.edtDefaultTabWidth.Value.Value;
		}
		void UpdateFormCore() {
			this.tabStopPositionEdit.Properties.TabFormattingInfo = Controller.TabFormattingInfo;
			UpdateSetAndClearButtons();
			UpdateRgAlignment();
			UpdateRgLeader();
			UpdateLblRemoteTabs();
			this.edtDefaultTabWidth.Value = Controller.DefaultTabWidth;
		}
		void UpdateRgAlignment() {
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null) {
				this.rgAlignment.SelectedIndex = 0;
				return;
			}
			this.rgAlignment.SelectedIndex = TabsFormHelper.GetIndexFromAlignment(tabInfo.Alignment);
		}
		void UpdateRgLeader() {
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null) {
				this.rgLeader.SelectedIndex = 0;
				return;
			}
			this.rgLeader.SelectedIndex = TabsFormHelper.GetIndexFromLeader(tabInfo.Leader);
		}
		void UpdateLblRemoteTabs() {
			if (this.isClearAllHappend) {
				this.lblRemoveTabStops.Text = String.Format("{0}", XtraRichEditLocalizer.GetString(XtraRichEditStringId.TabForm_All));
				return;
			}
			this.lblRemoveTabStops.Text = String.Empty;
			int countItems = removedTabStops.Count;
			List<String> result = new List<string>();
			for (int i = 0; i < countItems; i++) {
				UIUnit unit = unitConverter.ToUIUnit(removedTabStops[i], UnitType);
				result.Add(unit.ToString());
				this.lblRemoveTabStops.Text = String.Join("; ", result.ToArray());
				if (this.lblRemoveTabStops.Bottom > this.tabStopPositionEdit.Bottom) {
					while (this.lblRemoveTabStops.Bottom > this.tabStopPositionEdit.Bottom) {
						result.RemoveAt(result.Count - 1);
						this.lblRemoveTabStops.Text = String.Join("; ", result.ToArray());
						this.lblRemoveTabStops.Text += "...";
					}
					break;
				}
			}
		}
		TabInfo GetEditedTabInfo() {
			if (Controller.TabFormattingInfo.Count < 1) {
				return null;
			}
			int indx = this.tabStopPositionEdit.TabStopPositionIndex;
			if (indx < 0)
				return null;
			return Controller.TabFormattingInfo[indx];
		}
		void OnBtnClearClick(object sender, EventArgs e) {
			int currentItemIndex = this.tabStopPositionEdit.TabStopPositionIndex;
			if (currentItemIndex < 0)
				return;
			TabFormattingInfo tabFormattingInfo = Controller.TabFormattingInfo;
			RemoveTabStop(currentItemIndex, tabFormattingInfo);
			int lastTabIndex = tabFormattingInfo.Count - 1;
			if (currentItemIndex > lastTabIndex)
				currentItemIndex = lastTabIndex;
			UpdateForm();
			UnsubscribeControlsEvents();
			try {
				if (lastTabIndex > -1)
					this.tabStopPositionEdit.TabStopPosition = tabFormattingInfo[currentItemIndex].Position;
			}
			finally {
				SubscribeControlsEvents();
			}
			return;
		}
		void RemoveTabStop(int currentItemIndex, TabFormattingInfo tabFormattingInfo) {
			TabInfo tabInfo = tabFormattingInfo[currentItemIndex];
			tabFormattingInfo.Remove(tabInfo);
			int currentItem = tabInfo.Position;
			if (!removedTabStops.Contains(currentItem)) {
				removedTabStops.Add(currentItem);
				removedTabStops.Sort();
			}
		}
		void OnBtnClearAllClick(object sender, EventArgs e) {
			Controller.TabFormattingInfo.Clear();
			this.isClearAllHappend = true;
			UpdateForm();
		}
		void OnBtnSetClick(object sender, EventArgs e) {
			if (!this.tabStopPositionEdit.DoValidate())
				return;
			int? tabStopPosition = this.tabStopPositionEdit.TabStopPosition;
			if (!tabStopPosition.HasValue)
				return;
			TabInfo closeTabInfo = FindTabStopWithClosePosition(Controller.TabFormattingInfo, tabStopPosition.Value);
			if (closeTabInfo != null) {
				tabStopPositionEdit.TabStopPosition = closeTabInfo.Position;
				return;
			}
			TabAlignmentType alignment = TabsFormHelper.GetAlignmentFromIndex(this.rgAlignment.SelectedIndex);
			TabLeaderType leader = TabsFormHelper.GetLeaderFromIndex(this.rgLeader.SelectedIndex);
			TabInfo item = new TabInfo(tabStopPosition.Value, alignment, leader);
			Controller.TabFormattingInfo.Add(item);
			UpdateForm();
		}
		void UpdateSetAndClearButtons() {
			if (IsSetAndClearButtonDisabled()) {
				btnSet.Enabled = false;
				btnClear.Enabled = false;
			}
			else {
				btnSet.Enabled = true;
				btnClear.Enabled = true;
			}
		}
		void OnBtnOkClick(object sender, EventArgs e) {
			OnBtnSetClick(sender, e);
			if (!this.tabStopPositionEdit.DoValidate()) {
				return;
			}
			Controller.ApplyChanges();
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		TabInfo FindTabStopWithClosePosition(TabFormattingInfo tabFormattinInfo, int newTabStopPosition) {
			int count = tabFormattinInfo.Count;
			for (int i = 0; i < count; i++) {
				TabInfo tab = tabFormattinInfo[i];
				int delta = Math.Abs(tab.Position - newTabStopPosition);
				if (delta <= MinimumAllowableTabStopPostionDistance)
					return tab;
			}
			return null;
		}
		bool IsSetAndClearButtonDisabled() {
			return String.IsNullOrEmpty(this.tabStopPositionEdit.EditValue as String);
		}
	}
}
