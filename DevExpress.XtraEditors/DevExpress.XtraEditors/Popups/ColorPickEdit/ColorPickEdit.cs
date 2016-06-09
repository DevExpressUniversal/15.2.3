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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ColorPick.Picker;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ToolboxIcons;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTab;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraEditors {
	#region Registration
	public static class ColorPickEditRepositoryItemRegistrator {
		public static void Register() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources(String.Format("{0}.{1}", typeof(ToolboxIconsRootNS).Namespace, "ColorPickEdit.bmp"), Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(InternalEditorTypeName, typeof(ColorPickEdit), typeof(RepositoryItemColorPickEdit), typeof(ColorEditViewInfo), new ColorEditPainter(), true, img, typeof(DevExpress.Accessibility.PopupEditAccessible));
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		internal static string InternalEditorTypeName { get { return typeof(ColorPickEdit).Name; } }
	}
	#endregion
	[DXToolboxItem(true)]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "ColorPickEdit")]
	[DefaultBindingPropertyEx("Color")]
	[DefaultProperty("Color")]
	[Description("Advanced dropdown color picker.")]
	[ToolboxTabName(AssemblyInfo.DXTabCommon)]
	public class ColorPickEdit : ColorPickEditBase {
		static ColorPickEdit() {
			ColorPickEditRepositoryItemRegistrator.Register();
		}
		ColorItem selectedColorItem;
		public ColorPickEdit() {
			this.selectedColorItem = null;
		}
		[Browsable(false)]
		public bool IsAutomaticColorSelected {
			get { return SelectedColorItem != null && SelectedColorItem.IsAutoColor; }
		}
		protected internal void SetSelectedColorItem(ColorItem colorItem) {
			this.selectedColorItem = colorItem;
		}
		protected void CheckSelectedColorItem() {
			if(IsAutomaticColor(EditValue)) {
				SetSelectedColorItem(new AutoColorItem(Properties.AutomaticColor));
			}
			else {
				SetSelectedColorItem(new ColorItem(Color));
			}
		}
		protected bool IsAutomaticColor(object editValue) {
			return editValue != null && editValue.Equals(Properties.AutomaticColor);
		}
		protected internal virtual void OnAutomaticColorChanged() {
			CheckSelectedColorItem();
		}
		protected override void OnEditValueChanged() {
			CheckSelectedColorItem();
			base.OnEditValueChanged();
		}
		protected ColorItem SelectedColorItem { get { return selectedColorItem; } }
		protected override PopupBaseForm CreatePopupForm() {
			return CreatePopupFormInternal();
		}
		protected virtual PopupColorPickEditForm CreatePopupFormInternal() {
			return new PopupColorPickEditForm(this);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if(PopupFormCore != null) PopupFormCore.ProcessMouseWheel(e);
		}
		protected override bool IsAllTabsHidden {
			get { return base.IsAllTabsHidden && !Properties.ShowWebSafeColors; }
		}
		protected internal void ClosePopupCore(PopupCloseMode closeMode) {
			base.ClosePopup(closeMode);
		}
		protected internal bool InBars {
			get {
				return InplaceType == InplaceType.Bars;
			}
		}
		protected PopupColorPickEditForm PopupFormCore { get { return PopupForm as PopupColorPickEditForm; } }
	}
	public class PopupColorPickEditForm : PopupColorEditForm, IPopupColorPickEdit {
		public PopupColorPickEditForm(ColorEdit ownerEdit)
			: base(ownerEdit) {
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new PopupColorPickEditFormViewInfo(this);
		}
		protected internal override void OnBeforeShowPopup() {
			base.OnBeforeShowPopup();
			PopupColorBuilder.OnBeforeShowPopup();
		}
		protected override PopupColorBuilder CreatePopupColorEditBuilder() {
			return new PopupColorBuilderEx(this);
		}
		bool IPopupColorPickEdit.HasBorder {
			get {
				if(TabControl == null) return false;
				return TabControl.IsSinglePageTabControl && TabControl.SelectedTabPageIndex == 0;
			}
		}
		ColorPickEditBase IPopupColorPickEdit.OwnerEdit {
			get { return OwnerPickEdit; }
		}
		void IPopupColorPickEdit.SetSelectedColorItem(ColorItem colorItem) {
			OwnerPickEdit.SetSelectedColorItem(colorItem);
		}
		public override bool FormContainsFocus {
			get { return ContainsFocus || PopupColorBuilder.LockFocus; }
		}
		public override bool AllowMouseClick(Control control, Point pos) {
			bool allow = control == this || this.Contains(control);
			PopupAllowClickEventArgs e = new PopupAllowClickEventArgs(pos, control, allow);
			OnRaisePopupAllowClick(e);
			return e.Allow;
		}
		protected override PopupColorEditForm.ColorEditTabControl CreateTabControl() {
			return new ColorPickEditTabControl(this);
		}
		protected internal virtual void ProcessMouseWheel(MouseEventArgs e) {
			PopupColorBuilder.ProcessMouseWheel(e);
		}
		[Browsable(false)]
		public new ColorPickEditTabControl TabControl { get { return PopupColorBuilder != null ? base.TabControl as ColorPickEditTabControl : null; } }
		new protected PopupColorBuilderEx PopupColorBuilder { get { return base.PopupColorBuilder as PopupColorBuilderEx; } }
		[Category(CategoryName.Properties)]
		public new RepositoryItemColorPickEdit Properties { get { return base.Properties as RepositoryItemColorPickEdit; } }
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(PopupColorBuilder != null) PopupColorBuilder.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion
		#region Tab Control
		public class ColorPickEditTabControl : ColorEditTabControl {
			IPopupColorPickEdit owner;
			public ColorPickEditTabControl(IPopupColorPickEdit owner) {
				this.owner = owner;
			}
			protected override void CreatePages() {
				base.CreatePages();
				TabPages.Add(new XtraTabPage());
			}
			protected override BaseViewInfoRegistrator CreateViewInstance() {
				return new ColorPickEditTabControlSkinViewInfoRegistrator();
			}
			protected internal IPopupColorPickEdit Owner { get { return owner; } }
			protected int VisiblePageCount {
				get {
					int pageCount = 0;
					foreach(XtraTabPage page in TabPages) {
						if(page.PageVisible) pageCount++;
					}
					return pageCount;
				}
			}
			public bool IsSinglePageTabControl { get { return VisiblePageCount == 1; } }
		}
		public class ColorPickEditTabControlSkinViewInfoRegistrator : SkinViewInfoRegistrator {
			public override BaseTabControlViewInfo CreateViewInfo(IXtraTab tabControl) {
				return new ColorPickEditSkinTabControlViewInfo(tabControl);
			}
		}
		public class ColorPickEditSkinTabControlViewInfo : SkinTabControlViewInfo {
			public ColorPickEditSkinTabControlViewInfo(IXtraTab tabControl)
				: base(tabControl) {
			}
			protected override Rectangle CalcPageClientBounds() {
				if(TabControl.Owner != null && TabControl.Owner.HasBorder) return PageBounds;
				return base.CalcPageClientBounds();
			}
			public new ColorPickEditTabControl TabControl { get { return base.TabControl as ColorPickEditTabControl; } }
		}
		#endregion
		#region IPopupColorPickEdit Members
		void IPopupColorPickEdit.ClosePopup(PopupCloseMode closeMode) {
			OwnerPickEdit.ClosePopupCore(closeMode);
		}
		#endregion
		protected internal ColorPickEdit OwnerPickEdit { get { return OwnerEdit as ColorPickEdit; } }
	}
	public class PopupColorPickEditFormViewInfo : PopupBaseFormViewInfo {
		public PopupColorPickEditFormViewInfo(PopupColorPickEditForm form)
			: base(form) {
		}
		public override Rectangle ContentRect { get { return Rectangle.Inflate(base.ContentRect, 1, 1); } }
	}
	public class PopupColorBuilderEx : PopupColorBuilder, IDisposable {
		object resultColor;
		bool lockFocus;
		public PopupColorBuilderEx(IPopupColorEdit owner) : base(owner) {
			this.resultColor = Color.Empty;
			this.lockFocus = false;
		}
		public virtual void OnBeforeShowPopup() {
			UpdateInnerControls();
		}
		public override void OnShowPopup() {
			base.OnShowPopup();
			this.resultColor = Owner.EditValue;
		}
		protected override bool FindEditColor(Color color) {
			ForEach(control => control.SetColor(color, false));
			SelectBestTabPage(color);
			return true;
		}
		protected void SelectBestTabPage(Color color) {
			int pageIndex = GetBestTabPageIndex(color);
			if(pageIndex != -1) {
				TabControl.SelectedTabPageIndex = pageIndex;
			}
		}
		protected virtual int GetBestTabPageIndex(Color color) {
			RepositoryItemColorPickEdit prop = (RepositoryItemColorPickEdit)Owner.Properties;
			if(prop.ShowWebColors && WebTabInnerControl.ContainsColor(color)) {
				return 1;
			}
			else if(prop.ShowSystemColors && SystemTabInnerControl.ContainsColor(color)) {
				return 2;
			}
			else if(prop.ShowWebSafeColors && WebSafeTabInnerControl.ContainsColor(color)) {
				return 3;
			}
			else if(prop.ShowCustomColors && CustomTabInnerControl.ContainsColor(color)) {
				return 0;
			}
			return -1;
		}
		protected virtual void UpdateInnerControls() {
			DoAssign(CustomTabInnerControl);
		}
		protected override void SetTabPageProperties(int pageIndex, PopupBaseForm shadowForm) {
			BaseStyleControl control = null;
			XtraTabPage page = this.TabControl.TabPages[pageIndex];
			switch(pageIndex) {
				case 0:
					page.Text = GetLocalizedStringCore(StringId.ColorTabCustom);
					CustomTabInnerControl.SelectedColorChanged += OnSelectedColorChanged;
					CustomTabInnerControl.MoreButtonClick += OnMoreButtonClick;
					CustomTabInnerControl.AutomaticButtonClick += OnAutomaticButtonClick;
					page.PageVisible = Item.ShowCustomColors;
					control = CustomTabInnerControl;
					break;
				case 1:
					page.Text = GetLocalizedStringCore(StringId.ColorTabWeb);
					WebTabInnerControl.SelectedColorChanged += OnSelectedColorChanged;
					page.PageVisible = Item.ShowWebColors;
					control = WebTabInnerControl;
					break;
				case 2:
					page.Text = GetLocalizedStringCore(StringId.ColorTabSystem);
					SystemTabInnerControl.SelectedColorChanged += OnSelectedColorChanged;
					page.PageVisible = Item.ShowSystemColors;
					control = SystemTabInnerControl;
					break;
				case 3:
					page.Text = Localizer.Active.GetLocalizedString(StringId.ColorTabWebSafeColors);
					WebSafeTabInnerControl.SelectedColorChanged += OnSelectedColorChanged;
					page.PageVisible = Item.ShowWebSafeColors;
					control = WebSafeTabInnerControl;
					break;
			}
			control.Dock = DockStyle.Fill;
			if(Owner.LookAndFeel != null) {
				control.LookAndFeel.Assign(Owner.LookAndFeel);
			}
			page.Controls.Add(control);
		}
		public override void RefreshLookAndFeel() {
			UserLookAndFeel lookAndFeel = Owner.LookAndFeel;
			if(lookAndFeel != null)
				ForEach(control => control.LookAndFeel.Assign(lookAndFeel));
		}
		protected void ForEach(Action<InnerColorPickControlBase> callback) {
			callback(CustomTabInnerControl);
			callback(WebTabInnerControl);
			callback(SystemTabInnerControl);
			callback(WebSafeTabInnerControl);
		}
		protected void ForEachVisible(Action<InnerColorPickControlBase> callback) {
			ForEach(control => {
				if(control.Visible) callback(control);
			});
		}
		protected object ResultColor { get { return resultColor; } }
		public override bool LockFocus { get { return this.lockFocus; } }
		#region Handlers
		void OnMoreButtonClick(object sender, EventArgs e) {
			DoShowColorDialog();
		}
		void OnAutomaticButtonClick(object sender, EventArgs e) {
			OnAutomaticButtonClick();
		}
		protected virtual void OnAutomaticButtonClick() {
			RepositoryItemColorPickEdit prop = GetProperties();
			if(prop != null) {
				DoClose(prop.AutomaticColor);
			}
		}
		protected virtual void OnSelectedColorChanged(object sender, InnerColorPickControlSelectedColorChangedEventArgs e) {
			DoClose(e.NewColor);
		}
		protected virtual void DoShowColorDialog() {
			if(!Item.ShowColorDialog) return;
			try {
				this.lockFocus = true;
				Item.LockEventsCore();
				using(FrmColorPicker frm = new FrmColorPicker(Item)) {
					Owner.ClosePopup(PopupCloseMode.Cancel);
					frm.StartPosition = FormStartPosition.CenterScreen;
					frm.SelectedColor = GetColor();
					frm.TopMost = GetTopState();
					frm.ShowInTaskbar = false;
					RaiseColorPickDialogShowing(frm);
					if(frm.ShowDialog(GetOwner()) == DialogResult.OK) {
						Color color = frm.SelectedColor;
						DoClose(color);
						DoSaveColor(color);
					}
					RaiseColorPickDialogClosed(frm.DialogResult);
				}
			}
			finally {
				this.lockFocus = false;
				Item.ReleaseEventsCore(true);
			}
		}
		protected virtual void DoSaveColor(Color color) {
			RepositoryItemColorPickEdit properties = GetProperties();
			if(properties != null) {
				properties.RecentColors.InsertColor(color, 0);
				UpdateInnerControls();
			}
		}
		protected RepositoryItemColorPickEdit GetProperties() {
			return Owner != null ? Owner.Properties as RepositoryItemColorPickEdit : null;
		}
		protected new IPopupColorPickEdit Owner { get { return base.Owner as IPopupColorPickEdit; } }
		protected bool GetTopState() {
			ColorPickEdit ownerEdit = Owner.OwnerEdit as ColorPickEdit;
			return ownerEdit != null ? ownerEdit.InBars : false;
		}
		protected virtual IWin32Window GetOwner() {
			ColorPickEdit edit = Owner.OwnerEdit as ColorPickEdit;
			if(edit != null && edit.InBars) {
			}
			return null;
		}
		protected virtual Color GetColor() {
			if(Owner.Color.IsEmpty)
				return Color.FromArgb(0xFF, Owner.Color);
			return Owner.Color;
		}
		protected virtual void RaiseColorPickDialogShowing(XtraForm frm) {
			if(Item != null) Item.RaiseColorPickDialogShowing(new ColorPickDialogShowingEventArgs(frm));
		}
		protected virtual void RaiseColorPickDialogClosed(DialogResult result) {
			if(Item != null) Item.RaiseColorPickDialogClosed(new ColorPickDialogClosedEventArgs(result));
		}
		protected virtual void DoClose(Color color) {
			this.resultColor = color;
			if(Owner.IsPopupOpen) {
				Owner.ClosePopup();
			}
			else {
				if(Owner.OwnerEdit != null) Owner.OwnerEdit.EditValue = color;
			}
			Owner.ClosePopup();
			OnColorChanged();
		}
		#endregion
		protected override Size CalcBestSizeCore() {
			int popupWidth = DefaultPopupWidth;
			if(ShouldDrawControlBorder) {
				popupWidth += 2;
			}
			return new Size(popupWidth, CustomTabInnerControl.CalcBestHeight(ColorPickControlMaxRowItemCount));
		}
		protected virtual int DefaultPopupWidth { get { return 262; } }
		protected override void MarshalKeyToPopupCore(KeyEventArgs e) {
			ForEachVisible(control => control.DoKeyDown(e));
		}
		protected internal virtual void ProcessMouseWheel(MouseEventArgs e) {
			ForEachVisible(control => control.DoMouseWheel(e));
		}
		public override object ResultValue { get { return ResultColor; } }
		InnerColorPickControl customTabInnerControl = null;
		public InnerColorPickControl CustomTabInnerControl {
			get {
				if(this.customTabInnerControl == null) {
					this.customTabInnerControl = CreateCustomTabInnerControl();
				}
				return customTabInnerControl;
			}
		}
		protected virtual InnerColorPickControl CreateCustomTabInnerControl() {
			InnerColorPickControl control = CreateCustomTabInnerControlInstance();
			DoAssign(control);
			return control;
		}
		protected virtual void DoAssign(InnerColorPickControl control) {
			control.MaxRowItemCount = ColorPickControlMaxRowItemCount;
			control.Selectable = control.UserMouse = false;
			if(!ShouldDrawControlBorder) {
				control.BorderStyle = BorderStyles.NoBorder;
			}
			control.ThemeColors.AddColorRange(Item.ThemeColors.ToList());
			control.StandardColors.AddColorRange(Item.StandardColors.ToList());
			if(Item.RecentColors.IsEmpty) {
				control.RecentColors.Clear();
			}
			else {
				control.RecentColors.AddColorRange(Item.RecentColors.ToList(true));
			}
			control.AutomaticColor = Item.AutomaticColor;
			control.AutomaticButtonCaption = Item.AutomaticColorButtonCaption;
			control.AutomaticBorderColor = Item.AutomaticBorderColor;
		}
		protected virtual bool ShouldDrawControlBorder {
			get {
				if(Owner == null) return false;
				return GetVisibleTabCount() == 1;
			}
		}
		protected int GetVisibleTabCount() {
			if(Owner == null) return 0;
			int tabCount = 0;
			RepositoryItemColorPickEdit prop = (RepositoryItemColorPickEdit)Owner.Properties;
			if(prop.ShowCustomColors) {
				tabCount++;
			}
			if(prop.ShowSystemColors) {
				tabCount++;
			}
			if(prop.ShowWebColors) {
				tabCount++;
			}
			if(prop.ShowWebSafeColors) {
				tabCount++;
			}
			return tabCount;
		}
		protected virtual int ColorPickControlMaxRowItemCount { get { return 10; } }
		protected virtual InnerColorPickControl CreateCustomTabInnerControlInstance() {
			return new InnerColorPickControl();
		}
		InnerColorListControl webTabInnerControl = null;
		public InnerColorListControl WebTabInnerControl {
			get {
				if(this.webTabInnerControl == null) {
					this.webTabInnerControl = CreateWebTabInnerControl();
				}
				return this.webTabInnerControl;
			}
		}
		protected virtual InnerColorListControl CreateWebTabInnerControl() {
			InnerColorListControl control = CreateWebTabInnerControlInstance();
			control.Selectable = control.UserMouse = false;
			control.BorderStyle = BorderStyles.NoBorder;
			control.Colors.AddColorRange(ColorListBoxViewInfo.WebColors);
			IntPtr handle = control.Handle;
			return control;
		}
		protected virtual InnerColorListControl CreateWebTabInnerControlInstance() {
			return new InnerColorListControl();
		}
		InnerColorListControl systemTabInnerControl = null;
		public InnerColorListControl SystemTabInnerControl {
			get {
				if(this.systemTabInnerControl == null) {
					this.systemTabInnerControl = CreateSystemTabInnerControl();
				}
				return this.systemTabInnerControl;
			}
		}
		protected virtual InnerColorListControl CreateSystemTabInnerControl() {
			InnerColorListControl control = CreateSystemTabInnerControlInstance();
			control.Selectable = control.UserMouse = false;
			control.BorderStyle = BorderStyles.NoBorder;
			control.Colors.AddColorRange(ColorListBoxViewInfo.SystemColors);
			IntPtr handle = control.Handle;
			return control;
		}
		protected virtual InnerColorListControl CreateSystemTabInnerControlInstance() {
			return new InnerColorListControl();
		}
		InnerColorMatrixControl webSafeTabInnerControl = null;
		public InnerColorMatrixControl WebSafeTabInnerControl {
			get {
				if(this.webSafeTabInnerControl == null) {
					this.webSafeTabInnerControl = CreateWebSafeTabInnerControl();
				}
				return webSafeTabInnerControl;
			}
		}
		protected virtual InnerColorMatrixControl CreateWebSafeTabInnerControl() {
			InnerColorMatrixControl control = CreateWebSafeTabInnerControlInstance();
			control.Selectable = control.UserMouse = false;
			control.BorderStyle = BorderStyles.NoBorder;
			control.Colors.AddColorRange(ColorListBoxViewInfo.WebSafeColors);
			return control;
		}
		protected virtual InnerColorMatrixControl CreateWebSafeTabInnerControlInstance() {
			return new InnerColorMatrixControl();
		}
		protected RepositoryItemColorPickEdit Item { get { return Owner.Properties as RepositoryItemColorPickEdit; } }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.customTabInnerControl != null) {
					this.customTabInnerControl.Dispose();
				}
				this.customTabInnerControl = null;
				if(this.webTabInnerControl != null) {
					this.webTabInnerControl.Dispose();
				}
				this.webTabInnerControl = null;
				if(this.webSafeTabInnerControl != null) {
					this.webSafeTabInnerControl.Dispose();
				}
				this.webSafeTabInnerControl = null;
				if(this.systemTabInnerControl != null) {
					this.systemTabInnerControl.Dispose();
				}
				this.systemTabInnerControl = null;
			}
		}
	}
}
