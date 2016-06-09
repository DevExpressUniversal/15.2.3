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
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class TabsFormControl : UserControl, IDialogContent {
		const int MinimumAllowableTabStopPostionDistance = 4; 
		readonly TabsFormController controller;
		readonly UIUnitConverter unitConverter;
		readonly DocumentUnit unitType = DocumentUnit.Inch;
		readonly List<int> removedTabStops = new List<int>();
		bool isClearAllHappend;
		public TabsFormControl() {
			InitializeComponent();
			SubscribeControlsEvents();
		}
		public TabsFormControl(TabsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.unitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			DocumentUnit unitType = controllerParameters.Control.InnerControl.Unit;
			this.unitType = (unitType == DocumentUnit.Document) ? DocumentUnit.Inch : unitType;
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		public TabsFormController Controller { get { return controller; } }
		protected internal virtual TabsFormController CreateController(TabsFormControllerParameters controllerParameters) {
			return new TabsFormController(controllerParameters);
		}
		void SubscribeControlsEvents() {
			tabStopPositionEdit.EditValueChanged += OnTabStopPositionEditValueChanged;
			tabStopPositionEdit.SelectedIndexChanged += OnTabStopPositionEditSelectedIndexChanged;
			edtDefaultTabWidth.EditValueChanged += OnEdtDefaultTabWidthValueChanged;
			cbLeft.Checked += OnRgAlignmentSelectedIndexChanged;
			cbRight.Checked += OnRgAlignmentSelectedIndexChanged;
			cbCenter.Checked += OnRgAlignmentSelectedIndexChanged;
			cbDecimal.Checked += OnRgAlignmentSelectedIndexChanged;
			cbNone.Checked += OnRgLeaderSelectedIndexChanged;
			cbDots.Checked += OnRgLeaderSelectedIndexChanged;
			cbMiddleDots.Checked += OnRgLeaderSelectedIndexChanged;
			cbHyphens.Checked += OnRgLeaderSelectedIndexChanged;
			cbUnderline.Checked += OnRgLeaderSelectedIndexChanged;
			cbThickline.Checked += OnRgLeaderSelectedIndexChanged;
			cbEqualSign.Checked += OnRgLeaderSelectedIndexChanged;
			btnClear.Click += OnBtnClearClick;
			btnClearAll.Click += OnBtnClearAllClick;
			btnSet.Click += OnBtnSetClick;
		}
		void UnsubscribeControlsEvents() {
			tabStopPositionEdit.EditValueChanged -= OnTabStopPositionEditValueChanged;
			tabStopPositionEdit.SelectedIndexChanged -= OnTabStopPositionEditSelectedIndexChanged;
			edtDefaultTabWidth.EditValueChanged -= OnEdtDefaultTabWidthValueChanged;
			cbLeft.Checked -= OnRgAlignmentSelectedIndexChanged;
			cbRight.Checked -= OnRgAlignmentSelectedIndexChanged;
			cbCenter.Checked -= OnRgAlignmentSelectedIndexChanged;
			cbDecimal.Checked -= OnRgAlignmentSelectedIndexChanged;
			cbNone.Checked -= OnRgLeaderSelectedIndexChanged;
			cbDots.Checked -= OnRgLeaderSelectedIndexChanged;
			cbMiddleDots.Checked -= OnRgLeaderSelectedIndexChanged;
			cbHyphens.Checked -= OnRgLeaderSelectedIndexChanged;
			cbUnderline.Checked -= OnRgLeaderSelectedIndexChanged;
			cbThickline.Checked -= OnRgLeaderSelectedIndexChanged;
			cbEqualSign.Checked -= OnRgLeaderSelectedIndexChanged;
			btnClear.Click -= OnBtnClearClick;
			btnClearAll.Click -= OnBtnClearAllClick;
			btnSet.Click -= OnBtnSetClick;
		}
		void OnTabStopPositionEditSelectedIndexChanged(object sender, EventArgs e) {
			UpdateForm();
		}
		void OnRgLeaderSelectedIndexChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null)
				return;
			UpdateTabInfoAlignmentAndLeader(tabInfo);
		}
		void OnRgAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null)
				return;
			UpdateTabInfoAlignmentAndLeader(tabInfo);
		}
		void UpdateTabInfoAlignmentAndLeader(TabInfo tabInfo) {
			TabFormattingInfo tabFormattingInfo = Controller.TabFormattingInfo;
			tabFormattingInfo.Remove(tabInfo);
			TabAlignmentType alignment = TabsFormHelper.GetAlignmentFromIndex(GetAlignmentIndex());
			TabLeaderType leader = TabsFormHelper.GetLeaderFromIndex(GetLeaderIndex());
			TabInfo newTabInfo = new TabInfo(tabInfo.Position, alignment, leader);
			tabFormattingInfo.Add(newTabInfo);
		}
		void OnEdtDefaultTabWidthValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			if (edtDefaultTabWidth.Value.HasValue)
				Controller.DefaultTabWidth = edtDefaultTabWidth.Value.Value;
		}
		void OnTabStopPositionEditValueChanged(object sender, EventArgs e) {
			if (Controller == null)
				return;
			UpdateSetAndClearButtons();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			UpdateForm();
		}
		void InitializeForm() {
			if (Controller == null)
				return;
			edtDefaultTabWidth.MinValue = ParagraphFormDefaults.MinDefaultTabStopPosition;
			edtDefaultTabWidth.MaxValue = ParagraphFormDefaults.MaxTabStopPositionByDefault;
			edtDefaultTabWidth.DefaultUnitType = unitType;
			edtDefaultTabWidth.Value = null;
			edtDefaultTabWidth.Text = String.Empty;
			edtDefaultTabWidth.Value = Controller.DefaultTabWidth;
			tabStopPositionEdit.Properties.UnitType = unitType;
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			tabStopPositionEdit.Properties.TabFormattingInfo = Controller.TabFormattingInfo;
			UpdateSetAndClearButtons();
			UpdateRgAlignment();
			UpdateRgLeader();
			UpdateLblRemoteTabs();
			edtDefaultTabWidth.Value = Controller.DefaultTabWidth;
		}
		void UpdateLblRemoteTabs() {
			if (this.isClearAllHappend) {
				lblRemoveTabStops.Text = String.Format("{0}", XtraRichEditLocalizer.GetString(XtraRichEditStringId.TabForm_All));
				return;
			}
			lblRemoveTabStops.Text = String.Empty;
			int countItems = removedTabStops.Count;
			List<String> result = new List<string>();
			for (int i = 0; i < countItems; i++) {
				UIUnit unit = unitConverter.ToUIUnit(removedTabStops[i], unitType);
				result.Add(unit.ToString());
				lblRemoveTabStops.Text = String.Join("; ", result.ToArray());
				if (lblRemoveTabStops.ActualHeight > tabStopPositionEdit.ActualHeight) {
					while (lblRemoveTabStops.ActualHeight > tabStopPositionEdit.ActualHeight) {
						result.RemoveAt(result.Count - 1);
						lblRemoveTabStops.Text = String.Join("; ", result.ToArray());
						lblRemoveTabStops.Text += "...";
					}
					break;
				}
			}
		}
		bool IsSetAndClearButtonDisabled() {
			return String.IsNullOrEmpty(tabStopPositionEdit.EditValue as String);
		}
		void UpdateSetAndClearButtons() {
			if (IsSetAndClearButtonDisabled()) {
			   btnSet.IsEnabled = false;
			   btnClear.IsEnabled = false;
			}
			else {
				btnSet.IsEnabled = true;
				btnClear.IsEnabled = true;
			}
		}
		TabInfo GetEditedTabInfo() {
			if (Controller.TabFormattingInfo.Count < 1) {
				return null;
			}
			int indx = tabStopPositionEdit.TabStopPositionIndex;
			if (indx < 0)
				return null;
			return Controller.TabFormattingInfo[indx];
		}
		int GetAlignmentIndex() {
			return cbLeft.IsChecked.Value ? 0 :
				cbRight.IsChecked.Value ? 3 :
				cbCenter.IsChecked.Value ? 2 : 1;
		}
		int GetLeaderIndex() {
			return cbNone.IsChecked.Value ? 0 :
				cbDots.IsChecked.Value ? 1 :
				cbMiddleDots.IsChecked.Value ? 2 :
				cbHyphens.IsChecked.Value ? 3 :
				cbUnderline.IsChecked.Value ? 4 :
				cbThickline.IsChecked.Value ? 5 : 6;
		}
		void SetAlignmentIndex(int index) {
			cbLeft.IsChecked = index == 0;
			cbRight.IsChecked = index == 3;
			cbCenter.IsChecked = index == 2;
			cbDecimal.IsChecked = index == 1;
		}
		void SetLeaderIndex(int index) {
			cbNone.IsChecked = index == 0;
			cbDots.IsChecked = index == 1;
			cbMiddleDots.IsChecked = index == 2;
			cbHyphens.IsChecked = index == 3;
			cbUnderline.IsChecked = index == 4;
			cbThickline.IsChecked = index == 5;
			cbEqualSign.IsChecked = index == 6;
		}
		void UpdateRgAlignment() {
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null) {
				SetAlignmentIndex(0);
				return;
			}
			SetAlignmentIndex(TabsFormHelper.GetIndexFromAlignment(tabInfo.Alignment));
		}
		void UpdateRgLeader() {
			TabInfo tabInfo = GetEditedTabInfo();
			if (tabInfo == null) {
				SetLeaderIndex(0);
				return;
			}
			SetLeaderIndex(TabsFormHelper.GetIndexFromLeader(tabInfo.Leader));
		}
		void OnBtnClearAllClick(object sender, EventArgs e) {
			if (Controller == null)
				return;
			Controller.TabFormattingInfo.Clear();
			this.isClearAllHappend = true;
			UpdateForm();
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
		void OnBtnSetClick(object sender, EventArgs e) {
			if (Controller == null)
				return;
			if (!tabStopPositionEdit.DoValidate())
				return;
			int? tabStopPosition = tabStopPositionEdit.TabStopPosition;
			if (!tabStopPosition.HasValue)
				return;
			TabInfo closeTabInfo = FindTabStopWithClosePosition(Controller.TabFormattingInfo, tabStopPosition.Value);
			if (closeTabInfo != null) {
				tabStopPositionEdit.TabStopPosition = closeTabInfo.Position;
				return;
			}
			TabAlignmentType alignment = TabsFormHelper.GetAlignmentFromIndex(GetAlignmentIndex());
			TabLeaderType leader = TabsFormHelper.GetLeaderFromIndex(GetLeaderIndex());
			TabInfo item = new TabInfo(tabStopPosition.Value, alignment, leader);
			Controller.TabFormattingInfo.Add(item);
			UpdateForm();
		}
		void OnBtnClearClick(object sender, EventArgs e) {
			if (Controller == null)
				return;
			int currentItemIndex = tabStopPositionEdit.TabStopPositionIndex;
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
					tabStopPositionEdit.TabStopPosition = tabFormattingInfo[currentItemIndex].Position;
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
		protected internal virtual void ApplyChanges() {
			if (Controller != null)
				Controller.ApplyChanges();
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
}
