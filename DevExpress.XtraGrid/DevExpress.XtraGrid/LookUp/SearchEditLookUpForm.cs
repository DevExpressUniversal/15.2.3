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
using System.Text;
using DevExpress.XtraGrid.Editors;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using System.IO;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.XtraEditors.Popup {
	public abstract class CustomBlogGridPopupForm : CustomBlobPopupForm {
		public CustomBlogGridPopupForm(GridLookUpEditBase ownerEdit)
			: base(ownerEdit) {
			this.ViewInfo.ShowSizeBar = Properties.ShowFooter;
		}
		protected override void Dispose(bool disposing) {
			DestroyCloseTimer();
			base.Dispose(disposing);
		}
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			if(Form.ActiveForm != null) {
				Form frm = Form.ActiveForm;
				if(Form.ActiveForm == OwnerEdit.FindForm()) ClosePopup();
			}
		}
		protected abstract object GetResultValue(int rowHandle, bool allowFireNewValue);
		protected override object QueryResultValue() {
			int rowHandle = View.FocusedRowHandle;
			if(View.IsValidRowHandle(rowHandle)) return GetResultValue(rowHandle, false);
			SaveGridLayout();
			ResetDataControllerFilter();
			return GetResultValue(GridControl.InvalidRowHandle, true);
		}
		protected override Size DefaultBlobFormSize {
			get {
				Size res = OwnerEdit.Properties.GetDesiredPopupFormSizeCore(false);
				if(res.Width == 0) res.Width = DefaultEmptySize.Width;
				if(res.Height == 0) res.Height = DefaultEmptySize.Height;
				int bestWidth = CalcBestFit(res);
				if(bestWidth != 0) res.Width = bestWidth;
				return res;
			}
		}
		protected virtual int BestFitPadding { get { return 0; } }
		public virtual int CalcBestFit(Size currentSize) {
			if(Properties.BestFitMode == BestFitMode.None || IsPopupWidthStored) return 0;
			object bw = Properties.PropertyStoreCore[LookUpPropertyNames.PopupBestWidth];
			if(bw != null) return (int)bw;
			Rectangle current = View.ViewRect;
			View.InternalSetViewRectCore(new Rectangle(View.ViewRect.Location, currentSize));
			int res = 0;
			if(Properties.BestFitMode == BestFitMode.BestFit) {
				View.BestFitColumns();
			}
			else {
				bool prevAutoWidth = View.OptionsView.ColumnAutoWidth;
				View.OptionsView.ColumnAutoWidth = false;
				View.BestFitColumns();
				res = View.ViewInfo.ViewRects.ColumnTotalWidth + View.ViewInfo.ViewRects.IndicatorWidth + (View.ViewInfo.ViewRects.Bounds.Width - View.ViewInfo.ViewRects.Client.Width) + BestFitPadding;
				res = Math.Min(res, Math.Max(100, Screen.GetWorkingArea(OwnerEdit).Width - 100));
				View.OptionsView.ColumnAutoWidth = prevAutoWidth;
			}
			View.InternalSetViewRectCore(current);
			Properties.PropertyStoreCore[LookUpPropertyNames.PopupBestWidth] = res;
			return res;
		}
		public override void ShowPopupForm() {
			View.BeginUpdate();
			try {
				if(ShouldStoreGridLayout) {
					MemoryStream ms = Properties.PropertyStoreCore["ViewSettingsManager"] as MemoryStream;
					if(ms != null) {
						ms.Seek(0, SeekOrigin.Begin);
						View.RestoreLayoutFromStream(ms);
					}
					View.CollapseAllGroups();
				}
				ShowingPopupForm();
				View.HideEditor();
			}
			finally {
				View.CancelUpdate();
			}
			if(LookUp != null) LookUp.Show(OwnerEdit.EditValue, OwnerEdit.AutoSearchText);
			base.ShowPopupForm();
		}
		protected virtual void ShowingPopupForm() {
			this.gridLayoutSaved = false;
		}
		bool gridLayoutSaved = false;
		protected virtual bool ShouldStoreGridLayout { get { return OwnerEdit != null && OwnerEdit.InplaceType != InplaceType.Standalone; } }
		protected void SaveGridLayout() {
			if(gridLayoutSaved) return;
			this.gridLayoutSaved = true;
			try {
				if(ShouldStoreGridLayout) {
					MemoryStream stream = new MemoryStream();
					View.SaveLayoutToStream(stream);
					IDisposable prev = Properties.PropertyStoreCore["ViewSettingsManager"] as IDisposable;
					if(prev != null) prev.Dispose();
					Properties.PropertyStoreCore["ViewSettingsManager"] = stream;
				}
			}
			catch { }
		}
		protected void ResetDataControllerFilter() {
			try {
				bool requestRows = Controller.FilterExpression != "";
				int visibleCount = GridViewInfo != null && GridViewInfo.RowsInfo != null ? GridViewInfo.RowsInfo.Count + 1 : 0;
				Controller.FilterExpression = string.Empty;
				if(requestRows && Controller != null) Controller.LoadRows(0, Math.Max(10, visibleCount));
			}
			catch { }
		}
		public override void HidePopupForm() {
			DestroyCloseTimer();
			SaveGridLayout();
			ResetDataControllerFilter();
			base.HidePopupForm();
		}
		GridViewInfo GridViewInfo { get { return View == null ? null : View.ViewInfo; } }
		DataController Controller { get { return Properties.Controller; } }
		Timer closeTimer = null;
		public override void CheckClosePopup(Control activeControl) {
			if(IsPopupFormClosing) return;
			if(closeTimer == null && activeControl != null && activeControl == OwnerEdit.FindForm()) {
				closeTimer = new Timer();
				closeTimer.Interval = 10;
				closeTimer.Tick += new EventHandler(OnCloseTimer);
				closeTimer.Enabled = true;
			}
		}
		void DestroyCloseTimer() {
			if(closeTimer != null) closeTimer.Dispose();
			closeTimer = null;
		}
		void OnCloseTimer(object sender, EventArgs e) {
			DestroyCloseTimer();
			if(Visible && !IsPopupFormClosing) ClosePopup();
		}
		protected GridControl Grid { get { return Properties.Grid; } }
		protected GridView View { get { return Properties.View; } }
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			if(base.AllowMouseClick(control, mousePosition)) return true;
			if(control == OwnerEdit.Parent || (control != null && control.FindForm() != null && control.FindForm().Contains(OwnerEdit))) return false;
			if(control != null) {
				var fc = control.FindForm();
				var f = OwnerEdit.FindForm();
				if(fc == f || (fc != null && fc.IsMdiChild)) return false;
			}
			return true;
		}
		protected virtual void UpdateDisplayFilter(string displayFilter) {
			if(LookUp != null) LookUp.SetDisplayFilter(displayFilter);
		}
		protected IGridLookUp LookUp { get { return View as IGridLookUp; } }
		public new RepositoryItemGridLookUpEditBase Properties { get { return OwnerEdit.Properties as RepositoryItemGridLookUpEditBase; } }
		public new GridLookUpEditBase OwnerEdit { get { return base.OwnerEdit as GridLookUpEditBase; } }
		public override bool AllowSizing { get { return Properties.PopupSizeable; } set { } }
	}
	[ToolboxItem(false)]
	public class PopupSearchLookUpEditForm : CustomBlogGridPopupForm {
		public PopupSearchLookUpEditForm(SearchLookUpEdit ownerEdit)
			: base(ownerEdit) {
			Controls.Add(Properties.LookUpPopup);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) Controls.Remove(Properties.LookUpPopup);
			base.Dispose(disposing);
		}
		class PopupSearchLookUpEditFormViewInfo : CustomBlobPopupFormViewInfo {
			public PopupSearchLookUpEditFormViewInfo(PopupSearchLookUpEditForm form) : base(form) { }
			protected override int CalcSizeBarHeight(Size gripSize) {
				return 18;
			}
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new PopupSearchLookUpEditFormViewInfo(this);
		}
		protected override void SetupButtons() {
			base.SetupButtons();
		}
		public override void HidePopupForm() {
			View.ClearColumnsFilter();
			base.HidePopupForm();
		}
		public override void ShowPopupForm() {
			Properties.ActivateGridDataSource();
			LookUpPopup.BeforeShowing();
			base.ShowPopupForm();
			FocusFormControl(LookUpPopup.FindTextBox);
		}
		internal TextEdit FindEdit { get { return LookUpPopup.teFind; } }
		protected override void ShowingPopupForm() {
			Grid.SetSelectable(false);
			Grid.ForceInitialize();
			base.ShowingPopupForm();
		}
		public override bool FormContainsFocus {
			get {
				if(base.FormContainsFocus) return true;
				if(LookUpPopup.IsFocused) return true;
				return false;
			}
		}
		protected override int BestFitPadding { get { return this.LookUpPopup.lc.Root.Padding.Width + this.LookUpPopup.lc.Root.Spacing.Width; } }
		protected override Size DefaultEmptySize { get { return new Size(500, 400); } }
		protected SearchEditLookUpPopup LookUpPopup { get { return Properties.LookUpPopup; } }
		protected override Control EmbeddedControl { get { return LookUpPopup; } }
		public new RepositoryItemSearchLookUpEdit Properties { get { return OwnerEdit.Properties as RepositoryItemSearchLookUpEdit; } }
		public new SearchLookUpEdit OwnerEdit { get { return base.OwnerEdit as SearchLookUpEdit; } }
		protected internal virtual void AddNewClick() {
			Properties.AddNewClick();
		}
		protected internal virtual void ClearClick() {
			Properties.ClearClick();
		}
		protected internal virtual void FindClick(string text, bool isButtonClick) {
			if(isButtonClick)
				Properties.ForceActivateGridDataSource();
			else {
				if(Properties.GetPopupFindMode() == FindMode.FindClick) return;
			}
			UpdateDisplayFilter(text);
		}
		protected override object GetResultValue(int rowHandle, bool allowFireNewValue) {
			if(View.IsGroupRow(rowHandle)) rowHandle = View.GetDataRowHandleByGroupRowHandle(rowHandle);
			if(View.DataController.IsValidControllerRowHandle(rowHandle)) {
				object value = View.DataController.GetValueEx(rowHandle, Properties.ValueMember);
				if(!AsyncServerModeDataController.IsNoValue(value)) return value;
			}
			return OldEditValue;
		}
		protected virtual bool AllowGridKeys { get { return (View.State == GridState.Editing || View.FocusedRowHandle == GridControl.AutoFilterRowHandle); } }
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(!Focused) {
				if(AllowGridKeys) {
					if(Grid.IsNeededKey(e.KeyData)) return;
				}
				else {
					if(e.KeyData == Keys.Enter && LookUpPopup.IsShouldCloseOnEnter()) {
						e.Handled = true;
						ClosePopup();
					}
				}
			}
			base.ProcessKeyDown(e);
		}
	}
}
