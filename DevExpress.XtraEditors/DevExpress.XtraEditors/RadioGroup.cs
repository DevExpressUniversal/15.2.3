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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Skins;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemRadioGroup : RepositoryItem {
		RadioGroupItemCollection items;
		int columns;
		RadioGroupItemsLayout itemsLayout;
		HorzAlignment glyphAlignment;
		static readonly object selectedIndexChanged = new object();
		bool enableFocusRect = false;
		bool allowAsyncSelection = true;
		public RepositoryItemRadioGroup() {
			this.items = CreateItemCollection();
			this.items.CollectionChanged += new CollectionChangeEventHandler(OnItems_CollectionChanged);
			this.glyphAlignment = HorzAlignment.Near;
			this.itemsLayout = RadioGroupItemsLayout.Default;
			this.columns = 0;
		}
		public new RadioGroup OwnerEdit { get { return base.OwnerEdit as RadioGroup; } }
		protected Image GetBrickImage(RadioGroupItemViewInfo itemVi, PrintCellHelperInfo info, int index) {
			MultiKey key = new MultiKey(new object[] { itemVi.PictureChecked, itemVi.PictureGrayed, itemVi.PictureUnchecked, itemVi.CheckStyle, itemVi.Bounds.Size, index, info.EditValue, this.AutoHeight, this.BorderStyle, this.Enabled, this.EditorTypeName });
			Image img = GetCachedPrintImage(key, info.PS);
			if(img != null) return img;
			using(BitmapGraphicsHelper gHelper = new BitmapGraphicsHelper(itemVi.GlyphRect.Size.Width, itemVi.GlyphRect.Size.Height)) {
				itemVi.Cache = new GraphicsCache(gHelper.Graphics);
				CheckObjectPainter painter = CheckPainterHelper.GetPainter(DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Flat);
				itemVi.GlyphRect = new Rectangle(Point.Empty, itemVi.GlyphRect.Size);
				itemVi.CheckState = Items.GetItemIndexByValue(info.EditValue) == index ? CheckState.Checked : CheckState.Unchecked;
				painter.DrawObject(itemVi);
				return AddImageToPrintCache(key, gHelper.MemSafeBitmap, info.PS);
			}
		}
		protected override BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = base.CreateBrickStyle(info, type);
			if(type != "panel") {
				style.Sides = DevExpress.XtraPrinting.BorderSide.None;
			}
			else {
				SetupTextBrickStyleProperties(info, style);
			}
			if(type == "check") {
				SetupTextBrickStyleProperties(info, style);
			}
			return style;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			IPanelBrick outerPanel = CreatePanelBrick(info, true, CreateBrickStyle(info, "panel"));
			RadioGroupViewInfo rgVi = PreparePrintViewInfo(info, true) as RadioGroupViewInfo;
			for(int i = 0; i < rgVi.ItemsInfo.Count; i++) {
				RadioGroupItemViewInfo itemVi = rgVi.ItemsInfo[i] as RadioGroupItemViewInfo;
				CheckBoxTextBrick cb = new XECheckBoxTextBrick();
				cb.Style = CreateBrickStyle(info, "check");
				cb.CheckState = Items.GetItemIndexByValue(info.EditValue) == i ? CheckState.Checked : CheckState.Unchecked;
				cb.Text = itemVi.Caption;
				cb.Rect = itemVi.Bounds;
				outerPanel.Bricks.Add(cb);
				if(cb.CheckState == CheckState.Checked) {
					outerPanel.Text = itemVi.Caption;
				}
			}
			outerPanel.Hint = "";
			return outerPanel;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemRadioGroup source = item as RepositoryItemRadioGroup;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.glyphAlignment = source.GlyphAlignment;
				this.allowAsyncSelection = source.AllowAsyncSelection;
				this.columns = source.Columns;
				this.itemsLayout = source.itemsLayout;
				this.Items.Assign(source.Items);
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(selectedIndexChanged, source.Events[selectedIndexChanged]);
		}
		protected virtual RadioGroupItemCollection CreateItemCollection() {
			return new RadioGroupItemCollection();
		}
		protected virtual void OnItems_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnPropertiesChanged();
		}
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Near; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemRadioGroup Properties { get { return this; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "RadioGroup"; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoHeight { get { return false; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRadioGroupItems"),
#endif
 DXCategory(CategoryName.Data), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.ComponentModel.Design.CollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor)), Localizable(true)]
		public RadioGroupItemCollection Items { get { return items; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRadioGroupGlyphAlignment"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(HorzAlignment.Near), SmartTagProperty("Glyph Alignment", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public HorzAlignment GlyphAlignment {
			get { return glyphAlignment; }
			set {
				if(GlyphAlignment == value) return;
				glyphAlignment = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRadioGroupAllowAsyncSelection"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public bool AllowAsyncSelection {
			get { return allowAsyncSelection; }
			set {
				if(AllowAsyncSelection == value) return;
				allowAsyncSelection = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRadioGroupEnableFocusRect"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(false)]
		public bool EnableFocusRect {
			get { return enableFocusRect; }
			set {
				if (EnableFocusRect == value) return;
				enableFocusRect = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRadioGroupColumns"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(0), SmartTagProperty("Columns", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public int Columns {
			get { return columns; }
			set {
				if(value < 0) value = 0;
				if(Columns == value) return;
				columns = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(RadioGroupItemsLayout.Default), DXCategory(CategoryName.Appearance)]
		public RadioGroupItemsLayout ItemsLayout {
			get { return itemsLayout; }
			set {
				if(ItemsLayout == value) return;
				itemsLayout = value;
				OnItemsLayoutChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemRadioGroupSelectedIndexChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler SelectedIndexChanged {
			add { this.Events.AddHandler(selectedIndexChanged, value); }
			remove { this.Events.RemoveHandler(selectedIndexChanged, value); }
		}
		protected internal virtual void RaiseSelectedIndexChanged() {
			EventHandler handler = (EventHandler)this.Events[selectedIndexChanged];
			if(handler != null) handler(GetEventSender(), EventArgs.Empty);
		}
		public override bool IsFilterLookUp { get { return true; } }
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		protected internal virtual RadioGroupItemsLayout GetActualItemsLayout() {
			if(ItemsLayout == RadioGroupItemsLayout.Default) return RadioGroupItemsLayout.Column;
			return ItemsLayout;
		}
		protected virtual void OnItemsLayoutChanged() {
			if(OwnerEdit != null) OwnerEdit.ResetItemsLayout();
			OnPropertiesChanged();
		}
	}
}
namespace DevExpress.XtraEditors {
	public enum RadioGroupItemsLayout { Default, Column, Flow }
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultEvent("SelectedIndexChanged"), Designer("DevExpress.XtraEditors.Design.RadioGroupDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Supports the selection of one of several options."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), Docking(DockingBehavior.Ask), SmartTagAction(typeof(RadioGroupActions), "Items", "Edit Items", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "RadioGroup")
	]
	public class RadioGroup : BaseEdit {
		public RadioGroup() {   }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.RadioGroupAccessible(this); 
		}
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down) return (Properties.Items.Count > 0);
			return base.IsInputKey(keyData);
		}
		public override bool IsNeededKey(KeyEventArgs e) {
			if(!IsNavigationKey(e.KeyCode)) return Properties.IsNeededKey(e.KeyData);
			if(InplaceType == InplaceType.Standalone) return true;
			if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
				e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Home || e.KeyCode == Keys.End) return false;
			if(e.KeyCode == Keys.Left) return SelectedIndex > 0;
			if(e.KeyCode == Keys.Right) return SelectedIndex < Properties.Items.Count - 1;
			return Properties.IsNeededKey(e.KeyData);
		}
		protected override void OnEditValueChanged() {
			if(!ViewInfo.IsSelectedIndexLocked) ViewInfo.CanRecalcSelectedIndex = true;
			int prevIndex = SelectedIndex;
			base.OnEditValueChanged();
			if(!ViewInfo.IsSelectedIndexLocked && SelectedIndex != prevIndex)
				Properties.RaiseSelectedIndexChanged();
			radioGroupValChanged = true;
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			if(!IsNavigationKey(e.KeyCode)) return;
			if(e.KeyData == Keys.Enter && EnterMoveNextControl) {
				this.ProcessDialogKey(Keys.Tab);
				e.Handled = true;
				return;
			}
			if(Properties.ReadOnly) return;
			int step = 0;
			Keys keyCode = e.KeyCode;
			if(ViewInfo.RightToLeft) 
				keyCode = TranslateRTLKeyCode(keyCode);
			switch(keyCode) {
				case Keys.Left:
				case Keys.Up:
					if(SelectedIndex == 0)
						SelectedIndex = ViewInfo.CheckIndex(Properties.Items.Count - 1, false);
					else
						SelectedIndex = ViewInfo.CheckIndex(SelectedIndex - 1, false);
					e.Handled = true;
					return;
				case Keys.Right:
				case Keys.Down:
					if(SelectedIndex == Properties.Items.Count - 1)
						SelectedIndex = ViewInfo.CheckIndex(0, true);
					else
						SelectedIndex = ViewInfo.CheckIndex(SelectedIndex + 1, true);
					e.Handled = true;
					return;
				case Keys.PageUp: step = -ViewInfo.ItemsPerColumn; break;
				case Keys.PageDown: step = ViewInfo.ItemsPerColumn; break;
				case Keys.Home: step = -SelectedIndex; break;
				case Keys.End: step = Properties.Items.Count; break;
			}
			SelectedIndex = CheckIndex(SelectedIndex + step, step > 0);
			e.Handled = true;
		}
		protected virtual Keys TranslateRTLKeyCode(Keys keyCode) {
			if(Properties.GetActualItemsLayout() == RadioGroupItemsLayout.Flow) {
				if(keyCode == Keys.Right) return Keys.Left;
				if(keyCode == Keys.Left) return Keys.Right;
			}
			return keyCode;
		}
		protected override bool ProcessMnemonic(char charCode) {
			if(!this.CanSelect || Properties.ReadOnly) return false;
			for(int i = 0; i < Properties.Items.Count; i++) {
				if(Control.IsMnemonic(charCode, Properties.Items[i].Description)) {
					if(Form.ActiveForm == FindForm() || (Form.ActiveForm != null && Form.ActiveForm.ActiveMdiChild == FindForm())) 
						Select();
					if(Focused) SelectedIndex = i;
					return true;
				}
			}
			return false;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(GetValidationCanceled()) return;
			if((e.Button & MouseButtons.Left) == 0) return;
			ViewInfo.HitIndex = ViewInfo.GetItemIndexByPoint(new Point(e.X, e.Y));
			UpdateItemState(ViewInfo.HitIndex);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			HotIndex = (ViewInfo.GetItemIndexByPoint(new Point(e.X, e.Y)));
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(GetValidationCanceled()) return;
			if(ViewInfo.HitIndex == -1) return;
			int upIndex = ViewInfo.GetItemIndexByPoint(new Point(e.X, e.Y));
			if(upIndex == ViewInfo.HitIndex && !Properties.ReadOnly && ViewInfo.GetItemEnabled(upIndex)) {
				SelectedIndex = upIndex;
				AccessibilityNotifyClients(AccessibleEvents.Focus, upIndex);			   
				AccessibilityNotifyClients(AccessibleEvents.Selection, upIndex);
			}
			SetDefaultState();
			UpdateItemState(upIndex);
		}
		public virtual Rectangle GetItemRectangle(int index) {
			if(index < 0 || index >= Properties.Items.Count) return Rectangle.Empty;
			return ViewInfo == null ? Rectangle.Empty : ViewInfo.GetItemRectangle(index);			
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseWheel(ee);
			if(!ee.Handled && !Properties.ReadOnly && Properties.AllowMouseWheel) {
				ee.Handled = true;
				SelectedIndex = CheckIndex(SelectedIndex + (ee.Delta > 0 ? -1 : 1), ee.Delta < 0);
			}
		}
		protected override void OnLostFocus(EventArgs e) {
			SetDefaultState();
			base.OnLostFocus(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			HotIndex = -1;
			base.OnMouseLeave(e);
		}
		protected virtual void UpdateItemState(int itemIndex) {
			if(itemIndex == -1) return;
			if (ViewInfo.ItemsInfo.Count == 0 || (ViewInfo.ItemsInfo.Count - 1) < itemIndex) {
				HotIndex = -1; 
				return;
			}
			RadioGroupItemViewInfo itemInfo = (RadioGroupItemViewInfo)ViewInfo.ItemsInfo[itemIndex];
			ViewInfo.UpdateItemState(itemInfo, itemIndex);
			Invalidate(itemInfo.Bounds);
		}
		int HotIndex { 
			get { return ViewInfo.HotIndex; }
			set {
				if(HotIndex == value) return;
				int oldHotIndex = HotIndex;
				ViewInfo.HotIndex = value;
				UpdateItemState(oldHotIndex);
				UpdateItemState(HotIndex);
			}
		}
		protected virtual void SetDefaultState() {
			if(ViewInfo.HitIndex == -1) return;
			int hitIndex = ViewInfo.HitIndex;
			ViewInfo.HitIndex = -1;
			UpdateItemState(hitIndex);
		}
		int CheckIndex(int index, bool forward) { 
			index = Math.Min(Math.Max(0, index), Properties.Items.Count - 1);
			return ViewInfo.CheckIndex(index, forward, false);
		}
		private bool IsNavigationKey(Keys key) {
			return (key == Keys.Left || key == Keys.Up || key == Keys.Right || key == Keys.Down ||
				key == Keys.PageUp || key == Keys.PageDown || key == Keys.Home || key == Keys.End || (key == Keys.Enter && EnterMoveNextControl));
		}
		bool radioGroupValChanged;
		protected override void OnEditValueChanging(ChangingEventArgs e) {
			radioGroupValChanged = false;
			base.OnEditValueChanging(e);
		}
		[ DXCategory(CategoryName.Properties), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue(false)]
		public override bool AutoSizeInLayoutControl {
			get { return base.AutoSizeInLayoutControl; }
			set { base.AutoSizeInLayoutControl = value; OnPropertiesChanged(true); }
		}
		protected override Size CalcSizeableMaxSize() {
			Size baseMaxSize = base.CalcSizeableMaxSize();
			if(AutoSizeInLayoutControl) baseMaxSize.Height = CalcSizeableMinSize().Height;
			return baseMaxSize;
		}
		protected override Size CalcSizeableMinSize() {
			Size baseSize = base.CalcSizeableMinSize();
			if(ViewInfo != null && AutoSizeInLayoutControl) {
				Size minSize = ViewInfo.CalcMinSizeForLayoutControl();
				return new Size(Math.Max(minSize.Width, baseSize.Width), Math.Max(minSize.Height, baseSize.Height));
			}
			return baseSize;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupSelectedIndex"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int SelectedIndex {
			get { return ViewInfo.SelectedIndex; }
			set {
				int prevIndex = SelectedIndex;
				ViewInfo.SelectedIndex = value;
				ViewInfo.BeginLockSelectedIndex();
				try {
					ViewInfo.CanRecalcSelectedIndex = false;
					radioGroupValChanged = true;
					EditValue = (SelectedIndex == -1 ? null : Properties.Items[SelectedIndex].Value);
					if(!radioGroupValChanged) ViewInfo.SelectedIndex = prevIndex;
					LayoutChanged();
				}
				finally {
					ViewInfo.EndLockSelectedIndex();
				}
				if(prevIndex != SelectedIndex) Properties.RaiseSelectedIndexChanged();
			}
		}
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text;} set {} }
		[Browsable(false)]
		public override string EditorTypeName { get { return "RadioGroup"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemRadioGroup Properties { get { return base.Properties as RepositoryItemRadioGroup; } }
		protected internal new  RadioGroupPainter Painter { get { return base.Painter as RadioGroupPainter; } }
		protected internal new RadioGroupViewInfo ViewInfo { get { return base.ViewInfo as RadioGroupViewInfo; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupSelectedIndexChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler SelectedIndexChanged {
			add { Properties.SelectedIndexChanged += value; }
			remove { Properties.SelectedIndexChanged -= value; }
		}
		protected override Size DefaultSize { get { return new Size(100, 96); } }
		protected override void OnChangeUICues(UICuesEventArgs e) {
			base.OnChangeUICues(e);
			RefreshVisualLayout();
		}
		public Rectangle GetFirstItemTextBounds() {
			Rectangle textRect =  ViewInfo.GetItemRectangle(0);
			if(textRect == Rectangle.Empty) return Rectangle.Empty;
			textRect.Y = textRect.Y + (textRect.Height - ViewInfo.GetTextAscentHeight()) / 2;
			textRect.Height = ViewInfo.GetTextAscentHeight();
			return textRect;
		}
		protected internal void ResetItemsLayout() {
			ViewInfo.ResetItemsLayout();
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class RadioGroupPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawRadioGroupItems(info);
			DrawFocusRect(info);
		}
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) {
			RadioGroupViewInfo vi = info.ViewInfo as RadioGroupViewInfo;
			if((!vi.DrawFocusRect && !vi.AllowDrawFocusRect) || vi.FocusRect.IsEmpty) return;
			base.DrawFocusRectCore(info);
		}
		protected virtual void DrawRadioGroupItems(ControlGraphicsInfoArgs info) {
			RadioGroupViewInfo vi = info.ViewInfo as RadioGroupViewInfo;
			foreach(RadioGroupItemViewInfo itemInfo in vi.ItemsInfo) {
				itemInfo.Cache = info.Cache;
				try {
					itemInfo.Appearance.Assign(vi.PaintAppearance);
					if(!itemInfo.Enabled || !vi.Enabled)
						itemInfo.Appearance.ForeColor = vi.AppearanceDisabled.ForeColor == Color.Empty ?
							LookAndFeelHelper.GetSystemColor(vi.LookAndFeel, SystemColors.GrayText) :
							vi.AppearanceDisabled.ForeColor;
					vi.RadioPainter.DrawObject(itemInfo);
					if(itemInfo.Focused) {
						if(vi.Item.AllowFocused && itemInfo.CaptionRect.Width > 0)
							info.Paint.DrawFocusRectangle(info.Graphics, vi.GetFocusRect(itemInfo),
								info.ViewInfo.PaintAppearance.ForeColor, Color.Empty);
					}
				}
				finally {
					itemInfo.Cache = null;
				}
			}
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class RadioGroupViewInfo : BaseEditViewInfo {
		public static int DefaultHorzContentIndent = 4, DefaultVertContentIndent = 4;
		int minItemHeight, hitIndex, hotIndex, selectedIndex;
		int lockSelectedIndex;
		bool canRecalcSelectedIndex;
		ArrayList itemsInfo;
		CheckObjectPainter radioPainter;
		InplaceType inplaceType;
		RadioGroupItemsLayoutBase itemsLayout;
		public RadioGroupViewInfo(RepositoryItem item) : base(item) {
		}
		public override bool IsSupportFastViewInfo { get { return false; } }
		public override void Reset() {
			hitIndex = hotIndex = selectedIndex = -1;
			itemsInfo = new ArrayList();
			canRecalcSelectedIndex = true;
			lockSelectedIndex = 0;
			inplaceType = (OwnerEdit == null ? InplaceType.Grid : InplaceType.Standalone);
			ResetItemsLayout();
			base.Reset();
		}
		protected virtual Color GetForeColor() {
				return GetSystemColor(SystemColors.WindowText);
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault app = base.CreateDefaultAppearance();
			if(Item.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) app.ForeColor = GetForeColor();
			if(Item.ReadOnly) app.BackColor = GetSystemColor(SystemColors.ControlLight);
			return app;
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(PaintAppearance.BackColor == Color.Transparent && OwnerControl != null && OwnerControl.Parent != null) {
				if(Appearance.ForeColor == Color.Empty)
					PaintAppearance.ForeColor = OwnerControl.Parent.ForeColor;
			}
		}
		public override void Clear() {
			base.Clear();
			ItemsInfo.Clear();
		}
		protected internal override bool AllowDrawParentBackground { get { return true; } }
		protected override void CalcFocusRect(Rectangle bounds) {
			FocusRect = bounds;
		}
		protected override void UpdatePainters() {
			this.radioPainter = CreateRadioPainter();
			base.UpdatePainters();
		}
		protected override void UpdateFromEditor() {
			base.UpdateFromEditor();
			inplaceType = (OwnerEdit == null ? InplaceType.Grid : OwnerEdit.InplaceType);
		}
		public override bool DrawFocusRect {
			get {
				if (OwnerEdit == null) return false;
				return (Item.AllowFocused && OwnerEdit.EditorContainsFocus && SelectedIndex == -1 && Item.EnableFocusRect); 
			}
		}
		public RadioGroupItemsLayoutBase ItemsLayout {
			get {
				if(itemsLayout == null)
					itemsLayout = CreateItemsLayout();
				return itemsLayout;
			}
		}
		protected internal int ItemsPerColumn { get { return ItemsLayout.GetItemsPerColumn(); } }
		protected virtual RadioGroupItemsLayoutBase CreateItemsLayout() {
			if(Item.GetActualItemsLayout() == RadioGroupItemsLayout.Flow)
				return new RadioGroupFlowItemsLayout(this);
			return new RadioGroupColumnItemsLayout(this);
		}
		protected override void CalcConstants() {
			base.CalcConstants();
			minItemHeight = 0;
			if(CanRecalcSelectedIndex) selectedIndex = Item.Items.GetItemIndexByValue(EditValue);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			ItemsLayout.Calculate();
		}
		protected override Size CalcContentSize(Graphics g) {
			return ItemsLayout.CalcContentSize(g);
		}
		protected Size SafeCalcItemBestSize() {
			return (Item.Items.Count > 0 ? GetItemBestSize(Item.Items[0]) : Size.Empty);
		}
		public Size CalcMinSizeForLayoutControl() {
			return CalcContentSize(null);
		}
		protected internal virtual Size CalcMaxItemSize() {
			Size res = Size.Empty;
			foreach(RadioGroupItem item in Item.Items) {
				Size sz = GetItemBestSize(item);
				res.Width = Math.Max(res.Width, sz.Width);
				res.Height = Math.Max(res.Height, sz.Height);
			}
			return res;
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			foreach(RadioGroupItemViewInfo itemInfo in ItemsInfo)
				itemInfo.OffsetContent(x, y);
		}
		protected virtual CheckObjectPainter CreateRadioPainter() {
			if(IsPrinting) return CheckPainterHelper.GetPainter(ActiveLookAndFeelStyle.Flat);
			return CheckPainterHelper.GetPainter(LookAndFeel);
		}
		protected internal virtual RadioGroupItemViewInfo CreateItemInfo(RadioGroupItem item) {
			RadioGroupItemViewInfo res = new RadioGroupItemViewInfo(item, PaintAppearance);
			if(!Item.Enabled || !item.Enabled) res.State = ObjectState.Disabled;
			res.AllowHtmlString = Item.GetAllowHtmlDraw();
			res.ShowHotKeyPrefix = OwnerControl != null ? OwnerControl.ShowKeyboardCuesCore : true;
			res.GlyphAlignment = Item.GlyphAlignment;
			res.LookAndFeel = LookAndFeel;
			res.RightToLeft = RightToLeft;
			return res;
		}
		protected internal virtual Size GetItemBestSize(RadioGroupItem item) {
			CheckObjectInfoArgs chArgs = CreateItemInfo(item);
			chArgs.GlyphAlignment = Item.GlyphAlignment;
			GInfo.AddGraphics(GInfo.Graphics);
			try {
				chArgs.Graphics = GInfo.Graphics;
				return RadioPainter.CalcObjectMinBounds(chArgs).Size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual Rectangle GetFocusRect(RadioGroupItemViewInfo itemInfo) {
			if(Item.GlyphAlignment == HorzAlignment.Center) return itemInfo.Bounds;
			Rectangle ret = itemInfo.CaptionRect;
			ret.Inflate(2, 0);
			return ret;
		}
		public int GetItemIndexByPoint(Point pt) {
			for(int index = 0; index < ItemsInfo.Count; index++) {
				RadioGroupItemViewInfo itemInfo = (RadioGroupItemViewInfo)ItemsInfo[index];
				if(itemInfo.Bounds.Contains(pt)) return index;
			}
			return -1;
		}
		public Rectangle GetItemRectangle(int index) {
			RadioGroupItemViewInfo itemInfo = GetItemByIndex(index);
			if(itemInfo == null) return Rectangle.Empty; 
			return itemInfo.Bounds;
		}
		public bool GetItemEnabled(int index) {
			RadioGroupItemViewInfo itemInfo = GetItemByIndex(index);
			return itemInfo == null || itemInfo.Enabled;
		}
		protected RadioGroupItemViewInfo GetItemByIndex(int index) {
			if(index < 0 || index > ItemsInfo.Count - 1) return null;
			return (RadioGroupItemViewInfo)ItemsInfo[index];
		}
		public virtual void UpdateItemState(RadioGroupItemViewInfo hitItemInfo, int aboveIndex) {
			hitItemInfo.State = ObjectState.Normal;
			if(!Item.Enabled || !hitItemInfo.Enabled) {
				hitItemInfo.State = ObjectState.Disabled;
				return;
			}
			if(aboveIndex == HotIndex && HitIndex == -1) hitItemInfo.State = ObjectState.Hot;
			if(aboveIndex == HitIndex) hitItemInfo.State = (HitIndex == HotIndex ? ObjectState.Pressed : ObjectState.Hot);
		}
		public virtual void BeginLockSelectedIndex() { lockSelectedIndex++; }
		public virtual void EndLockSelectedIndex() { 
			lockSelectedIndex--;
			if(!Item.AllowAsyncSelection) 
			RefreshSelectedIndex();
		}
		void RefreshSelectedIndex() {
			if(IsSelectedIndexLocked) return;
			int selectedIndex = Item.Items.GetItemIndexByValue(EditValue);
			if(selectedIndex != SelectedIndex && Item.OwnerEdit != null) {
				SelectedIndex = selectedIndex;
				Item.OwnerEdit.Refresh();
			}
		}
		public bool IsSelectedIndexLocked { get { return lockSelectedIndex != 0; } }
		public int HitIndex { get { return hitIndex; } set { hitIndex = value; } }
		public int HotIndex { get { return hotIndex; } set { hotIndex = value; } }
		public bool CanRecalcSelectedIndex { get { return canRecalcSelectedIndex; } set { canRecalcSelectedIndex = value; } }
		public ArrayList ItemsInfo { get { return itemsInfo; } }
		public CheckObjectPainter RadioPainter { get { return radioPainter; } }
		protected new RepositoryItemRadioGroup Item { get { return base.Item as RepositoryItemRadioGroup; } }
		public Rectangle ItemsRect { get { return Rectangle.Inflate(ContentRect, -HorzContentIndent, -VertContentIndent); } }
		protected internal virtual int HorzContentIndent { get { return DefaultHorzContentIndent; } }
		protected internal virtual int VertContentIndent { get { return inplaceType == InplaceType.Standalone ? DefaultVertContentIndent : 0; } }
		protected internal virtual int MinItemHeight {
			get {
				if(minItemHeight == 0) minItemHeight = GetItemBestSize(new RadioGroupItem(null, "Wg")).Height;
				return minItemHeight;
			}
		}
		public int SelectedIndex { 
			get { return selectedIndex; } 
			set {
				if(value < 0) value = -1;
				if(value > Item.Items.Count - 1) value = Item.Items.Count - 1;
				if(SelectedIndex == value) return;
				selectedIndex = value;
			} 
		}
		protected virtual int CheckIndexCore(int index, bool forward) {
			RadioGroupItemViewInfo item  = GetItemByIndex(index);
			if(item == null) return -1;
			if(item.Enabled) return index;
			return CheckIndexCore(forward ? index + 1 : index - 1, forward);
		}
		protected internal int CheckIndex(int index, bool forward, bool sequential) {
			int ind = CheckIndexCore(index, forward);
			if(ind > -1) return ind;
			if(sequential) 
				return forward ? CheckIndexCore(0, forward) : CheckIndexCore(ItemsInfo.Count - 1, forward);
			return CheckIndexCore(index, !forward);
		}
		protected internal int CheckIndex(int index, bool forward) { return CheckIndex(index, forward, true); }
		protected internal void ResetItemsLayout() {
			itemsLayout = null;
		}
	}
	public class RadioGroupItemViewInfo : CheckObjectInfoArgs {
		RadioGroupItem item;
		int columnIndex;
		bool focused;
		public RadioGroupItemViewInfo(RadioGroupItem item, AppearanceObject style) : base(style) {
			this.item = item;
			this.columnIndex = -1;
			this.focused = false;
			base.CheckStyle = CheckStyles.Radio;
			base.Caption = item.Description;
		}
		public RadioGroupItem Item { get { return item;} }
		public bool Enabled { get { return item.Enabled; } }
		public int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public bool Focused { get { return focused; } set { focused = value; } }
	}
	public abstract class RadioGroupItemsLayoutBase {
		public RadioGroupItemsLayoutBase(RadioGroupViewInfo viewInfo) {
			ViewInfo = viewInfo;
		}
		protected RadioGroupViewInfo ViewInfo { get; private set; }
		protected RepositoryItemRadioGroup Item { get { return ViewInfo.Item as RepositoryItemRadioGroup; } }
		public abstract void Calculate();
		public abstract Size CalcContentSize(Graphics g);
		public abstract int GetItemsPerColumn();
	}
	public class RadioGroupColumnItemsLayout : RadioGroupItemsLayoutBase {
		public RadioGroupColumnItemsLayout(RadioGroupViewInfo viewInfo) : base(viewInfo) {
			ItemsPerColumn = -1;
			Columns = 1;
		}
		public int ItemsPerColumn { get; private set; }
		public int Columns { get; private set; }
		public override void Calculate() {
			CalcColumnsParams();
			CalcItemsInfo();
		}
		protected void CalcColumnsParams() {
			if(Item.Columns > 0) {
				Columns = Math.Min(Item.Columns, Item.Items.Count);
			}
			else {
				int rowCount = ViewInfo.ItemsRect.Height / ViewInfo.MinItemHeight;
				if(rowCount == 0) {
					Columns = Item.Items.Count;
					ItemsPerColumn = 1;
					return;
				}
				Columns = Item.Items.Count / rowCount;
				if(Columns * rowCount < Item.Items.Count) Columns++;
			}
			if(Columns == 0) Columns = 1;
			ItemsPerColumn = (Item.Items.Count + Columns - 1) / Columns;
		}
		protected virtual void CalcItemsInfo() {
			int columnItemCount = 0;
			ViewInfo.GInfo.AddGraphics(null);
			try {
				for(int i = 0, columnIndex = 0; i < Item.Items.Count; i++) {
					RadioGroupItemViewInfo itemInfo = ViewInfo.CreateItemInfo(Item.Items[i]);
					itemInfo.ColumnIndex = columnIndex;
					if(ViewInfo.SelectedIndex == i) {
						itemInfo.CheckState = CheckState.Checked;
						itemInfo.Focused = ViewInfo.Focused;
					}
					itemInfo.Graphics = ViewInfo.GInfo.Graphics;
					itemInfo.Bounds = GetItemBounds(i);
					ViewInfo.RadioPainter.CalcObjectBounds(itemInfo);
					itemInfo.Graphics = null;
					ViewInfo.UpdateItemState(itemInfo, i);
					ViewInfo.ItemsInfo.Add(itemInfo);
					if(++columnItemCount == ItemsPerColumn) {
						columnIndex++;
						columnItemCount = 0;
					}
				}
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected virtual Rectangle GetItemBounds(int itemIndex) {
			if(ItemsPerColumn == 0) return Rectangle.Empty;
			int columnIndex = itemIndex / ItemsPerColumn;
			int rowIndex = itemIndex - columnIndex * ItemsPerColumn;
			Rectangle itemsRect = ViewInfo.ItemsRect;
			Rectangle itemBounds = new Rectangle(itemsRect.Left + 
				(ViewInfo.RightToLeft ? Columns - 1 - columnIndex : columnIndex) * (itemsRect.Width / Columns), 
				itemsRect.Top + rowIndex * (itemsRect.Height / ItemsPerColumn),
				itemsRect.Width / Columns, itemsRect.Height / ItemsPerColumn);
			if(itemBounds.Height > ViewInfo.MinItemHeight) {
				itemBounds.Y += (itemBounds.Height - ViewInfo.MinItemHeight) / 2;
				itemBounds.Height = ViewInfo.MinItemHeight;
			}
			return itemBounds;
		}
		public override Size CalcContentSize(Graphics g) {
			Size size;
			if(Columns != Item.Columns) ViewInfo.CalcViewInfo(g);
			if(Columns == 0 || Columns == 1 || ViewInfo.InplaceType != InplaceType.Standalone) size = CalcContentSizeSingleColumn(g);
			else size = CalcContentSizeMultipleColums(g);
			return new Size(size.Width + 2 * ViewInfo.HorzContentIndent, size.Height + 2 * ViewInfo.VertContentIndent);
		}
		protected virtual Size CalcContentSizeMultipleColums(Graphics g) {
			Size resultSize = Size.Empty;
			Size itemSize = Size.Empty;
			itemSize = ViewInfo.CalcMaxItemSize();
			return new Size(itemSize.Width * Columns, itemSize.Height * ItemsPerColumn);
		}
		protected virtual Size CalcContentSizeSingleColumn(Graphics g) {
			Size size = ViewInfo.CalcMaxItemSize();
			if(ViewInfo.InplaceType == InplaceType.Standalone) size.Height = size.Height * Item.Items.Count;
			else size.Width = size.Width * Item.Items.Count;
			return size;
		}
		public override int GetItemsPerColumn() {
			return ItemsPerColumn;
		}
	}
	public class RadioGroupFlowItemsLayout : RadioGroupItemsLayoutBase {
		public RadioGroupFlowItemsLayout(RadioGroupViewInfo viewInfo) : base(viewInfo) {
		}
		protected virtual int HorzItemIndent { get { return Math.Max(4, 2 * ViewInfo.HorzContentIndent); } }
		protected virtual int VertItemIndent { get { return Math.Max(4, 2 * ViewInfo.VertContentIndent); } }
		public override void Calculate() {
			ViewInfo.GInfo.AddGraphics(null);
			try {
				int firstItemIndexInLine = 0, currentY = ViewInfo.ItemsRect.Top +  (ViewInfo.ItemsRect.Top == 0 ? VertItemIndent : 0);
				Size currentRowSize = Size.Empty;
				for(int i = 0; i < Item.Items.Count; i++) {
					RadioGroupItemViewInfo itemInfo = ViewInfo.CreateItemInfo(Item.Items[i]);
					if(ViewInfo.SelectedIndex == i) {
						itemInfo.CheckState = CheckState.Checked;
						itemInfo.Focused = ViewInfo.Focused;
					}
					Size itemSize = ViewInfo.GetItemBestSize(Item.Items[i]);
					itemInfo.Bounds = new Rectangle(Point.Empty, itemSize);
					if(currentRowSize.Width + itemSize.Width + HorzItemIndent > ViewInfo.ItemsRect.Width) {
						CalculateLine(currentY, currentRowSize.Height, firstItemIndexInLine, i);
						currentY += currentRowSize.Height + VertItemIndent;
						currentRowSize = itemSize;
						firstItemIndexInLine = i;
					}
					else {
						currentRowSize.Width += itemSize.Width + HorzItemIndent;
						currentRowSize.Height = Math.Max(itemSize.Height, currentRowSize.Height);
					}
					ViewInfo.ItemsInfo.Add(itemInfo);
				}
				if(firstItemIndexInLine < Item.Items.Count)
					CalculateLine(currentY, currentRowSize.Height, firstItemIndexInLine, Item.Items.Count);
			}
			finally {
				ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CalculateLine(int y, int height, int start, int end) {
			int indent = ViewInfo.ItemsRect.Left == 0 ? HorzItemIndent : 0;
			int x = ViewInfo.RightToLeft ? (ViewInfo.ItemsRect.Right - indent) : (ViewInfo.ItemsRect.Left + indent);
			ArrayList items = ViewInfo.ItemsInfo;
			for(int i = start; i < end; i++) {
				RadioGroupItemViewInfo info = items[i] as RadioGroupItemViewInfo;
				if(info != null) {
					int width = info.Bounds.Width;
					if(ViewInfo.RightToLeft)
						x -= width;
					info.Bounds = new Rectangle(x, y, width, height);
					info.Graphics = ViewInfo.GInfo.Graphics;
					ViewInfo.RadioPainter.CalcObjectBounds(info);
					info.Graphics = null;
					ViewInfo.UpdateItemState(info, i);
					if(ViewInfo.RightToLeft)
						x -= HorzItemIndent;
					else
						x += width + HorzItemIndent;
				}
			}
		}
		public override Size CalcContentSize(Graphics g) {
			Size size = CalcContentSizeCore(g);
			return new Size(size.Width + 2 * ViewInfo.HorzContentIndent, size.Height + 2 * ViewInfo.VertContentIndent);
		}
		protected virtual Size CalcContentSizeCore(Graphics g) {
			Size size = Size.Empty;
			foreach(RadioGroupItem item in Item.Items) {
				Size itemSize = ViewInfo.GetItemBestSize(item);
				size.Height = Math.Max(size.Height, itemSize.Height);
				size.Width += itemSize.Width + 2 * ViewInfo.HorzContentIndent;
			}
			return size;
		}
		public override int GetItemsPerColumn() {
			return Item.Items.Count;
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	[ListBindable(false)]
	public class RadioGroupItemCollection : CollectionBase, System.Collections.Generic.IEnumerable<RadioGroupItem> {
		public event CollectionChangeEventHandler CollectionChanged;
		int lockUpdate;
		public RadioGroupItemCollection() { this.lockUpdate = 0; }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("RadioGroupItemCollectionItem")]
#endif
		public RadioGroupItem this[int index] {
			get { return List[index] as RadioGroupItem; }
			set { List[index] = value; }
		}
		public RadioGroupItem GetItemByValue(object value) {
			foreach(RadioGroupItem item in List) {
				if(item.Value == null) {
					if(value == null) return item;
				}
				else if(item.Value.Equals(value)) return item;
			}
			return null;
		}
		public int GetItemIndexByValue(object value) {
			for(int i = 0; i < Count; i++) {
				RadioGroupItem item = this[i];
				if(item.Value == null) {
					if(value == null) return i;
				}
				else if(item.Value.Equals(value)) return i;
			}
			return -1;
		}
		public virtual void Assign(RadioGroupItemCollection collection) {
			if(collection == null) return;
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < collection.Count; n++) {
					Add(collection[n].Clone() as RadioGroupItem);
				}
			}
			finally { EndUpdate(); }
		}
		public void AddEnum(Type enumType) {
			AddEnum(enumType, null);
		}
		public void AddEnum<TEnum>() {
			AddEnum<TEnum>(null);
		}
		public void AddEnum(Type enumType, Converter<object, string> displayTextConverter) {
			if(displayTextConverter == null)
				displayTextConverter = (v) => EnumDisplayTextHelper.GetDisplayText(v);
			BeginUpdate();
			try {
				Array values = EnumDisplayTextHelper.GetEnumValues(enumType);
				foreach(object obj in values) {
					Add(new RadioGroupItem(obj, displayTextConverter(obj)));
				}
			}
			finally { EndUpdate(); }
		}
		public void AddEnum<TEnum>(Converter<TEnum, string> displayTextConverter) {
			if(displayTextConverter == null)
				displayTextConverter = (v) => EnumDisplayTextHelper.GetDisplayText(v);
			BeginUpdate();
			try {
				var values = EnumDisplayTextHelper.GetEnumValues<TEnum>();
				foreach(TEnum value in values) {
					Add(new RadioGroupItem(value, displayTextConverter(value)));
				}
			}
			finally { EndUpdate(); }
		}
		public virtual void AddRange(RadioGroupItem[] items) {
			BeginUpdate();
			try {
				foreach(RadioGroupItem item in items) Add(item);
			}
			finally { EndUpdate(); }
		}
		public virtual void Add(RadioGroupItem item) {
			List.Add(item);
		}
		public virtual void Insert(int index, RadioGroupItem item) {
			List.Insert(index, item);
		}
		public virtual void Remove(RadioGroupItem item) {
			List.Remove(item);
		}
		public int IndexOf(RadioGroupItem item) {
			return List.IndexOf(item);
		}
		public virtual void BeginUpdate() {
			lockUpdate++;
		}
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected override void OnInsertComplete(int index, object value) {
			RadioGroupItem ri = value as RadioGroupItem;
			ri.Changed += new EventHandler(OnItemChanged);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
		}
		protected override void OnRemoveComplete(int index, object value) {
			RadioGroupItem ri = value as RadioGroupItem;
			ri.Changed -= new EventHandler(OnItemChanged);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			RadioGroupItem ri = oldValue as RadioGroupItem;
			ri.Changed -= new EventHandler(OnItemChanged);
			ri = newValue as RadioGroupItem;
			ri.Changed += new EventHandler(OnItemChanged);
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, newValue));
		}
		protected override void OnClear() {
			if(Count == 0) return;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally { EndUpdate(); }
		}
		protected override void OnClearComplete() {
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected virtual void OnItemChanged(object sender, EventArgs e) {
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdate != 0) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		#region IEnumerable<RadioGroupItem>
		System.Collections.Generic.IEnumerator<RadioGroupItem> System.Collections.Generic.IEnumerable<RadioGroupItem>.GetEnumerator() {
			foreach(RadioGroupItem item in InnerList) yield return item;
		}
		#endregion
	}
	[TypeConverter("DevExpress.XtraEditors.Design.RadioGroupItemTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class RadioGroupItem : ICloneable {
		public event EventHandler Changed;
		object _value, _tag;
		string description;
		bool enabled;
		public RadioGroupItem() : this(null, string.Empty, true) {}
		public RadioGroupItem(object value, [Localizable(true)]string description) : this(value, description, true) { }
		public RadioGroupItem(object value, [Localizable(true)]string description, bool enabled) : this(value, description, enabled, null) { }
		public RadioGroupItem(object value, [Localizable(true)]string description, bool enabled, object tag) {
			this._value = value;
			this._tag = tag;
			this.description = (description == null ? string.Empty : description);
			this.enabled = enabled;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupItemValue"),
#endif
 Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual object Value {
			get { return _value; }
			set { 
				if(Value != value) {
					_value = value; 
					ItemChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupItemDescription"),
#endif
 Localizable(true)]
		public virtual string Description {
			get { return description; }
			set { 
				if(value == null) value = string.Empty;
				if(Description != value) {
					description = value; 
					ItemChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupItemEnabled"),
#endif
 DefaultValue(true)]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled != value) {
					enabled = value;
					ItemChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RadioGroupItemTag"),
#endif
 DefaultValue(null), Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual object Tag {
			get { return _tag; }
			set { _tag = value; }
		}
		public virtual object Clone() {
			return new RadioGroupItem(Value, Description, Enabled, Tag); 
		}
		protected virtual void ItemChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		public override string ToString() {
			if(Description != string.Empty) return Description;
			return base.ToString();
		}
	}
}
