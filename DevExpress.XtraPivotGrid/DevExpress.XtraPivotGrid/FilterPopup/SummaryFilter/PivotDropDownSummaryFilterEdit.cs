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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.FilterDropDown;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.XtraPivotGrid.FilterPopup.SummaryFilter {
	public class PivotDropDownSummaryFilterEdit : PivotDropDownFilterEditBase {
		protected internal new PivotSummaryFilterPopupContainerEdit ContainerEdit {
			get { return (PivotSummaryFilterPopupContainerEdit)base.ContainerEdit; }
		}
		public PivotDropDownSummaryFilterEdit(IPivotGridDropDownFilterEditOwner owner,
				PivotGridViewInfoData data, PivotGridField field, Rectangle bounds)
			: this(owner, null, data, field, bounds) { }
		public PivotDropDownSummaryFilterEdit(IPivotGridDropDownFilterEditOwner owner, Control parentControl,
					PivotGridViewInfoData data, PivotGridField field, Rectangle bounds)
			: base(owner, parentControl, data, field, bounds) {
		}
		protected override Size PopupFormContentSize {
			get { return ContainerEdit.PopupForm.ContentSize; }
		}
		protected override BlobBaseEdit CreateContainerEdit() {
			ISummaryFilterController filter = CreateFilter();
			return new PivotSummaryFilterPopupContainerEdit(filter);
		}
		protected virtual ISummaryFilterController CreateFilter() {
			return new PivotSummaryFilterController(Field);
		}
	}
	public class PivotSummaryFilterPopupContainerEdit : BlobBaseEdit {
		ISummaryFilterController filter;
		PivotSummaryFilterPopupContainerEdit() { }
		public PivotSummaryFilterPopupContainerEdit(ISummaryFilterController filter) {
			this.filter = filter;
		}
		protected ISummaryFilterController Filter { get { return filter; } set { filter = value; } }
		void ApplyFilter() {
			Filter.ApplyFilter();
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new PivotSummaryFilterPopupContainerForm(this, filter);
		}
		protected override void ClosePopup(PopupCloseMode closeMode) {
			base.ClosePopup(closeMode);
			if(closeMode == PopupCloseMode.Normal)
				ApplyFilter();
		}
		public new PivotSummaryFilterPopupContainerForm PopupForm { get { return (PivotSummaryFilterPopupContainerForm)base.PopupForm; } }
	}
	public class PivotSummaryFilterPopupContainerForm : BlobBasePopupForm {
		static float ScaleFactor {
			get {
				if(WindowsFormsSettings.TouchUIMode == TouchUIMode.True) {
					float scaleFactor =  WindowsFormsSettings.TouchScaleFactor;
					return (scaleFactor > 1) ? (1 + scaleFactor) / 2 : scaleFactor;
				} else {
					return 1;
				}
			}
		}
		readonly ISummaryFilterController filterController;
		PivotSummaryFilterPanel filterPanel;
		SimpleButton clearButton;
		PivotSummaryFilterPopupContainerForm() : base(null) { }
		public PivotSummaryFilterPopupContainerForm(BlobBaseEdit ownerEdit, ISummaryFilterController filter)
			: base(ownerEdit) {
			this.filterController = filter;
			this.filterPanel = new PivotSummaryFilterPanel(filter);
			SubscribeEvents();
			Controls.Add(FilterPanel);
			FilterPanel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			Controls.Add(ClearButton);
			UpdateClearButtonEnabled();
		}
		protected override Size DefaultMinFormSize { 
			get {
				float scaleFactor = PivotSummaryFilterPopupContainerForm.ScaleFactor;
				return new Size((int)(450 * scaleFactor), (int)(300 * scaleFactor)); 
			}
		}
		public SimpleButton ClearButton { get { return clearButton; } }
		protected override Control EmbeddedControl { get { return FilterPanel; } }
		protected PivotSummaryFilterPanel FilterPanel { get { return filterPanel; } }
		public ISummaryFilterController FilterController { get { return filterController; } }
		protected new PivotSummaryFilterPopupFormViewInfo ViewInfo { get { return base.ViewInfo as PivotSummaryFilterPopupFormViewInfo; } }
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new PivotSummaryFilterPopupFormViewInfo(this);
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			return control == this || this.Contains(control) || FilterPanel.AllowMouseClick(control);
		}
		protected override void CreateButtons() {
			base.CreateButtons();
			this.clearButton = new SimpleButton();
			ClearButton.AllowFocus = false;
		}
		protected override void UpdateButtons() {
			base.UpdateButtons();
			ClearButton.ButtonStyle = OwnerEdit.Properties.ButtonsStyle;
			ClearButton.LookAndFeel.ParentLookAndFeel = OwnerEdit.LookAndFeel;
			ClearButton.Text = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterClearButton);
			UpdateButtonPainters(ClearButton);
		}
		protected override void UpdateControlPositionsCore() {
			base.UpdateControlPositionsCore();
			ClearButton.Bounds = ViewInfo.ClearButtonRect;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			BeginControlUpdate();
			try {
				FilterPanel.InitControls();
			} finally {
				EndControlUpdate();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.clearButton != null) {
					this.clearButton.Dispose();
					this.clearButton = null;
				}
				if(this.filterPanel != null) {
					this.filterPanel.Dispose();
					this.filterPanel = null;
				}
			}
			base.Dispose(disposing);
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter && OkButton.Enabled) {
				e.Handled = true;
				OwnerEdit.ClosePopup();
				return;
			}
			base.ProcessKeyDown(e);
		}
		public override void ShowPopupForm() {
			base.ShowPopupForm();
			UpdateOKButtonEnabled();
			FocusFormControl(FilterPanel);
		}
		protected virtual void UpdateOKButtonEnabled() {
			OkButton.Enabled = FilterController.CanAccept;
		}
		void UpdateClearButtonEnabled() {
			ClearButton.Enabled = FilterController.StartValue != null || FilterController.EndValue != null;
		}
		protected virtual void SubscribeEvents() {
			FilterController.Updated += filter_Updated;
			ClearButton.Click += ClearButton_Click;
		}
		protected virtual void UnsubscribeEvents() {
			FilterController.Updated -= filter_Updated;
			ClearButton.Click -= ClearButton_Click;
		}
		void filter_Updated(object sender, EventArgs e) {
			UpdateOKButtonEnabled();
			UpdateClearButtonEnabled();
		}
		void ClearButton_Click(object sender, EventArgs e) {
			FilterPanel.ClearFilter();
		}
	}
	public class PivotSummaryFilterPopupFormViewInfo : CustomBlobPopupFormViewInfo {
		const int SideIndent = 12;
		Rectangle clearButtonRect;
		Size clearButtonSize;
		public PivotSummaryFilterPopupFormViewInfo(PopupBaseForm form)
			: base(form) {
		}
		public new PivotSummaryFilterPopupContainerForm Form { get { return base.Form as PivotSummaryFilterPopupContainerForm; } }
		public Rectangle ClearButtonRect { get { return clearButtonRect; } }
		protected Size ClearButtonSize { get { return clearButtonSize; } }
		protected override void Clear() {
			base.Clear();
			this.clearButtonRect = Rectangle.Empty;
			this.clearButtonSize = Size.Empty;
		}
		protected override void CalcButtonSize() {
			base.CalcButtonSize();
			if(ClearButtonSize != Size.Empty) return;
			GInfo.AddGraphics(null);
			try {
				this.clearButtonSize = Form.ClearButton.CalcBestFit(GInfo.Graphics);
				this.clearButtonSize.Width += 32;
			} finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			int y = SizeBarRect.Y + (SizeBarRect.Height - ClearButtonSize.Height) / 2;
			int x;
			if(SizeGripRect.IsEmpty || !IsLeftSizeGrip)
				x = (SizeBarRect.Left + SideIndent);
			else
				x = SizeGripRect.Right;
			if(IsRightToLeft)
				x = bounds.Width - x - ClearButtonSize.Width;
			clearButtonRect = new Rectangle(x, y, ClearButtonSize.Width, ClearButtonSize.Height);
		}
	}
}
