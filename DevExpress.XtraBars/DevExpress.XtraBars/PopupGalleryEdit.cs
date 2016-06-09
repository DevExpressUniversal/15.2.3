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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using System.Reflection;
using DevExpress.XtraBars.Ribbon;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.Controls;
using System.Collections;
using DevExpress.XtraBars.Ribbon.Handler;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemPopupGalleryEdit : RepositoryItemPopupBase, IDisplayTextEvaluatorOwner {
		internal static string PopupGalleryEditName { get { return "PopupGalleryEdit"; } }
		private static readonly object queryDisplayText = new object();
		EditValueTypeCollection editValueType;
		static RepositoryItemPopupGalleryEdit() {
			RegisterItem();
		}
		internal static void RegisterItem() {
			Bitmap img = ResourceImageHelper.CreateBitmapFromResources(String.Format("{0}.{1}", typeof(XtraBars.ToolboxIcons.ToolboxIconsRootNS).Namespace, "GalleryControl.bmp"), Assembly.GetExecutingAssembly());
			img.MakeTransparent(Color.Magenta);
			EditorClassInfo editorInfo = new EditorClassInfo(PopupGalleryEditName, typeof(PopupGalleryEdit), typeof(RepositoryItemPopupGalleryEdit), typeof(PopupGalleryEditViewInfo), new ButtonEditPainter(), true, img, typeof(DevExpress.Accessibility.PopupEditAccessible));
			EditorRegistrationInfo.Default.Editors.Add(editorInfo);
		}
		char separatorChar = ',';
		public RepositoryItemPopupGalleryEdit() : base() {
			ShowButtons = true;
			ShowPopupCloseButton = true;
			ShowSizeGrip = true;
			this.editValueType = EditValueTypeCollection.CSV;
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return PopupGalleryEditName; } }
		protected override bool NeededKeysPopupContains(Keys key) {
			switch(key) {
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Home:
				case Keys.PageDown:
				case Keys.PageUp:
				case Keys.End:
				case Keys.Enter:
					return true;
			}
			return base.NeededKeysPopupContains(key);
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemPopupGalleryEdit gitem = item as RepositoryItemPopupGalleryEdit;
			if(gitem == null)
				return;
			Gallery.Assign(gitem.Gallery);
			ShowButtons = gitem.ShowButtons;
			ShowPopupCloseButton = gitem.ShowPopupCloseButton;
			SeparatorChar = gitem.SeparatorChar;
			ShowSizeGrip = gitem.ShowSizeGrip;
			EditValueType = gitem.EditValueType;
			Events.AddHandler(queryDisplayText, gitem.Events[queryDisplayText]);
		}
		[DefaultValue(true)]
		public bool ShowButtons { get; set; }
		[DefaultValue(true)]
		public bool ShowPopupCloseButton { get; set; }
		[DefaultValue(true)]
		public bool ShowSizeGrip { get; set; }
		PopupGalleryEditGallery gallery;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupGalleryEditGallery Gallery {
			get {
				if(gallery == null)
					gallery = CreateGallery();
				return gallery;
			}
		}
		[ DXCategory(CategoryName.Format), DefaultValue(',')]
		public virtual char SeparatorChar {
			get { return this.separatorChar; }
			set {
				if(SeparatorChar == value) return;
				this.separatorChar = value;
				OnPropertiesChanged();
			}
		}
		[ DXCategory(CategoryName.Data), DefaultValue(EditValueTypeCollection.CSV)]
		public virtual EditValueTypeCollection EditValueType {
			get { return this.editValueType; }
			set {
				if(EditValueType == value) return;
				this.editValueType = value;
				OnPropertiesChanged();
			}
		}
		protected virtual PopupGalleryEditGallery CreateGallery() {
			return new PopupGalleryEditGallery(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.gallery != null)
					this.gallery.Dispose();
			}
			base.Dispose(disposing);
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			string res = base.GetDisplayText(format, editValue);
			QueryDisplayTextEventArgs e = new QueryDisplayTextEventArgs(editValue, res);
			RaiseQueryDisplayText(e);
			return e.DisplayText;
		}
		DisplayTextEvaluator displayTextEvaluator;
		protected internal DisplayTextEvaluator DisplayTextEvaluator {
			get {
				if(displayTextEvaluator == null)
					displayTextEvaluator = CreateDisplayTextEvaluator();
				return displayTextEvaluator;
			}
		}
		protected virtual DisplayTextEvaluator CreateDisplayTextEvaluator() {
			return new DisplayTextEvaluator(this);
		}
		protected internal virtual void PreQueryDisplayText(QueryDisplayTextEventArgs e) {
			DisplayTextEvaluator.CalcDisplayText(e);
		}
		[ DXCategory(CategoryName.Events)]
		public event QueryDisplayTextEventHandler QueryDisplayText {
			add { this.Events.AddHandler(queryDisplayText, value); }
			remove { this.Events.RemoveHandler(queryDisplayText, value); }
		}
		protected internal virtual void RaiseQueryDisplayText(QueryDisplayTextEventArgs e) {
			PreQueryDisplayText(e);
			QueryDisplayTextEventHandler handler = (QueryDisplayTextEventHandler)this.Events[queryDisplayText];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected virtual void SyncEditValue() {
			if(OwnerEdit != null)
				OwnerEdit.EditValue = DisplayTextEvaluator.GetCheckedItems(OwnerEdit.EditValue);
		}
		#region IDisplayTextEvaluatorOwner Members
		Type IDisplayTextEvaluatorOwner.FlagsType {
			get { return null; }
		}
		string IDisplayTextEvaluatorOwner.GetItemDescription(object item) {
			return ((GalleryItem)item).Caption;
		}
		object IDisplayTextEvaluatorOwner.GetItemValue(object item) {
			return ((GalleryItem)item).Value;
		}
		System.Collections.IEnumerable IDisplayTextEvaluatorOwner.GetItems(object editValue) {
			return OwnerEdit == null ? (IEnumerable)Gallery.GetAllItems() : (IEnumerable)((PopupGalleryEdit)OwnerEdit).CheckedItems;
		}
		Array IDisplayTextEvaluatorOwner.GetValues() {
			return Gallery.GetAllItems().Select(d => d.Value).ToArray();
		}
		bool IDisplayTextEvaluatorOwner.HasNegativeFlagsElements {
			get { return false; }
		}
		bool IDisplayTextEvaluatorOwner.IsFlags {
			get { return false; }
		}
		bool IDisplayTextEvaluatorOwner.IsItemChecked(object editValue, object item) {
			if(OwnerEdit == null) {
				IList<object> list = editValue as IList<object>;
				if(list != null && list.Contains(((GalleryItem)item).Value))
					return true;
				return false;
			}
			return ((PopupGalleryEdit)OwnerEdit).CheckedItems.Contains(((GalleryItem)item));
		}
		char IDisplayTextEvaluatorOwner.SeparatorChar {
			get { return SeparatorChar; }
		}
		EditValueTypeCollection IDisplayTextEvaluatorOwner.EditValueType {
			get { return EditValueType; }
		}
		#endregion
		protected internal virtual void OnGalleryEditPropertiesChanged() {
			DisplayTextEvaluator.UpdateValues();
			SyncEditValue();
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class PopupGalleryEditViewInfo : PopupBaseEditViewInfo {
		public PopupGalleryEditViewInfo(RepositoryItem item) : base(item) { 
		}
		protected PopupGalleryEdit PopupGalleryEdit { get { return OwnerEdit as PopupGalleryEdit; } }
		protected RepositoryItemPopupGalleryEdit ItemGallery { get { return (RepositoryItemPopupGalleryEdit)Item; } }
		protected override void OnEditValueChanged() {
			if(OwnerEdit == null)
				ItemGallery.DisplayTextEvaluator.UpdateValues();
			base.OnEditValueChanged();
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class PopupGalleryEditControl : GalleryControl {
		public PopupGalleryEditControl() : base() {
			SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse, false);
			TabStop = false;
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			BaseSetBoundsCore(x, y, width, height, specified);
		}
		public bool IsNeededKey(KeyEventArgs e) {
			return Handler.IsNeededKey(e);
		}
		protected internal virtual void OnEditorKeyDown(KeyEventArgs e) {
			OnKeyDownCore(e);
		}
		protected internal override bool AllowFocus { get { return false; } }
	}
	public class PopupGalleryFormViewInfo : CustomBlobPopupFormViewInfo {
		public PopupGalleryFormViewInfo(PopupBaseForm form) : base(form) { }
		protected override void CalcRects() {
			ShowSizeBar = ((PopupGalleryForm)Form).OwnerGalleryEdit.Properties.ShowSizeGrip;
			base.CalcRects();
		}
	}
	public class PopupGalleryForm : CustomBlobPopupForm {
		public PopupGalleryForm(PopupBaseEdit ownerEdit) : base(ownerEdit) {
			InitializeGalleryControl();
		}
		public PopupGalleryEdit OwnerGalleryEdit { get { return (PopupGalleryEdit)OwnerEdit; } }
		protected override void OnBeforeShowPopup() {
			base.OnBeforeShowPopup();
			GalleryControl.Gallery.Assign(OwnerGalleryEdit.Properties.Gallery);
			CheckGalleryItems();
		}
		protected override void ClosePopup(PopupCloseMode closeMode) {
			base.ClosePopup(closeMode);
			GalleryControl.Gallery.ReleaseEventHandlers();
		}
		protected virtual void CheckGalleryItems() {
			foreach(object value in OwnerGalleryEdit.CheckedValues) {
				GalleryItem item = GalleryControl.Gallery.GetItemByValue(value);
				if(item != null)
					item.Checked = true;
			}
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new PopupGalleryFormViewInfo(this);
		}
		protected virtual void InitializeGalleryControl() {
			GalleryControl = CreateGalleryControl();
			Controls.Add(GalleryControl);
			PopupGalleryEditGallery cloned = new PopupGalleryEditGallery();
			GalleryControl.SetGallery(cloned);
		}
		protected override void SetupButtons() {
			base.SetupButtons();
			this.fShowOkButton = OwnerGalleryEdit.Properties.ShowButtons;
			this.fCloseButtonStyle = OwnerGalleryEdit.Properties.ShowButtons && OwnerGalleryEdit.Properties.ShowPopupCloseButton ? BlobCloseButtonStyle.Caption : BlobCloseButtonStyle.None;
		}
		protected virtual GalleryControl CreateGalleryControl() {
			return new PopupGalleryEditControl() { BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder };
		}
		protected internal GalleryControl GalleryControl { get; set; }
		protected override Size CalcFormSizeCore() {
			Size size = GalleryControl.CalcGalleryBestSize();
			if(GalleryControl.Gallery.AutoSize == GallerySizeMode.Vertical) {
				size = GalleryControl.CalcGalleryBestSize(size);
			}
			return CalcFormSize(size);
		}
		public override object ResultValue {
			get {
				if(OwnerGalleryEdit == null)
					return null;
				OwnerGalleryEdit.SynchronizeCheckedItemsFromPopup();
				return OwnerGalleryEdit.EditValue;
			}
		}
		protected override Control EmbeddedControl { get { return GalleryControl;  }  }
	}
}
namespace DevExpress.XtraEditors {
	[
	ToolboxTabName(AssemblyInfo.DXTabCommon),
	DXToolboxItem(DXToolboxItemKind.Regular),
	Description("Editor with dropdown image gallery."),
	ToolboxBitmap(typeof(DevExpress.XtraBars.ToolboxIcons.ToolboxIconsRootNS), "PopupGalleryEdit"),
	Designer("DevExpress.XtraEditors.Design.PopupGalleryEditDesigner, " + AssemblyInfo.SRAssemblyBarsDesign)
	]
	public class PopupGalleryEdit : PopupBaseEdit {
		CheckedGalleryItemValueCollection checkedValues;
		CheckedGalleryItemCollection checkedItems;
		public PopupGalleryEdit() : base() { 
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			SynchronizeGalleryWithEditValue(EditValue);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return RepositoryItemPopupGalleryEdit.PopupGalleryEditName; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PopupGalleryEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemPopupGalleryEdit Properties { get { return (RepositoryItemPopupGalleryEdit)base.Properties; } }
		protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() {
			return new PopupGalleryForm(this);
		}
		internal void FireGalleryChanged() {
			FireChanged();
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			base.OnPopupClosed(closeMode);
			if(closeMode == PopupCloseMode.Cancel || (closeMode == PopupCloseMode.Immediate && Properties.ShowButtons))
				return;
			SynchronizeCheckedItemsFromPopup();
			Properties.OnGalleryEditPropertiesChanged();
		}
		protected PopupGalleryEditGallery PopupGallery {
			get {
				PopupGalleryForm popupForm = (PopupGalleryForm)PopupForm;
				if(popupForm == null) return null;
				return (PopupGalleryEditGallery)popupForm.GalleryControl.Gallery;
			}
		}
		protected internal PopupGalleryEditControl GetPopupGalleryControl() {
			if(!IsPopupOpen) return null;
			PopupGalleryEditGallery pg = PopupGallery;
			if(pg == null) return null;
			return pg.GalleryControl as PopupGalleryEditControl;
		}
		protected internal virtual void SynchronizeCheckedItemsFromPopup() {
			if(PopupGallery == null)
				return;
			List<GalleryItem> items = PopupGallery.GetCheckedItems();
			if(IsEqualLists(CheckedItems, items))
				return;
			CheckedItems.BeginUpdate();
			try {
				CheckedItems.Clear();
				foreach(GalleryItem item in items) {
					GalleryItem originalItem = Properties.Gallery.GetItemByValue(item.Value);
					if(originalItem != null)
						CheckedItems.Add(originalItem);
				}
			}
			finally {
				CheckedItems.EndUpdate();
			}
		}
		bool IsEqualLists(CheckedGalleryItemCollection checkItems, List<GalleryItem> items) { 
			if(checkItems.Count != items.Count)
				return false;
			for(int i = 0; i < checkItems.Count; i++) {
				if(checkItems[i] != items[i])
					return false;
			}
			return true;
		}
		public CheckedGalleryItemCollection CheckedItems {
			get {
				if(checkedItems == null)
					checkedItems = new CheckedGalleryItemCollection(this);
				return checkedItems;
			}
		}
		public CheckedGalleryItemValueCollection CheckedValues {
			get {
				if(checkedValues == null)
					checkedValues = new CheckedGalleryItemValueCollection(this);
				return checkedValues;
			}
		}
		protected internal virtual void OnCheckedValuesCollectionChanged() {
			SynchronizeCheckedItems();
			Properties.OnGalleryEditPropertiesChanged();
		}
		bool editValueSyncActive = false;
		protected virtual void SynchronizeGalleryWithEditValue(object editValue) {
			if(editValueSyncActive) return;
			GalleryItem item = Properties.Gallery.GetItemByValue(editValue);
			if(item == null)
				return;
			editValueSyncActive = true;
			Properties.LockEvents();
			try {
				CheckedItems.Clear();
				CheckedItems.Add(item);
			}
			finally {
				editValueSyncActive = false;
				Properties.UnLockEvents();
			}
		}
		protected virtual void SynchronizeCheckedItems() {
			CheckedItems.BeginUpdate();
			try {
				CheckedItems.Clear();
				foreach(object value in CheckedValues) {
					GalleryItem item = Properties.Gallery.GetItemByValue(value);
					if(item != null)
						CheckedItems.Add(item);
				}
			}
			finally {
				CheckedItems.CancelUpdate();
			}
		}
		protected internal virtual void OnCheckedItemsCollectionChanged() {
			SynchronizeCheckedValues();
			Properties.OnGalleryEditPropertiesChanged();
		}
		protected virtual void SynchronizeCheckedValues() {
			CheckedValues.BeginUpdate();
			try {
				CheckedValues.Clear();
				foreach(GalleryItem item in CheckedItems) {
					if(item.Value != null)
						CheckedValues.Add(item.Value);
				}
			}
			finally {
				CheckedValues.CancelUpdate();
			}
		}
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Enter)
				return true;
			return base.IsInputKey(keyData);
		}
		protected internal virtual bool IsGalleryNeededKey(KeyEventArgs e) {
			PopupGalleryEditControl gallery = GetPopupGalleryControl();
			return gallery != null && gallery.IsNeededKey(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			PopupGalleryEditControl gallery = GetPopupGalleryControl();
			if(gallery == null || !gallery.IsNeededKey(e))
				base.OnKeyDown(e);
			if(e.Handled)
				return;
			if(gallery != null) {
				if(e.KeyCode == Keys.Enter) {
					ClosePopup();
				}
				else {
					gallery.OnEditorKeyDown(e);
					e.Handled = true;
				}
			}
		}
		protected override TextBoxMaskBox CreateMaskBoxInstance() {
			return new PopupGalleryMaskBox(this);
		}
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_CHAR && msg.WParam.ToInt32() == ((int)Keys.Space)) { 
				if(GetPopupGalleryControl() != null)
					return;
			}
			base.WndProc(ref msg);
		}
	}
	public class PopupGalleryMaskBox : TextBoxMaskBox {
		public PopupGalleryMaskBox(TextEdit owner) : base(owner) { 
		}
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_CHAR) {
				if(((PopupGalleryEdit)OwnerEdit).IsPopupOpen)
					return;
			}
			base.WndProc(ref msg);
		}
	}
	public class PopupEditGalleryNavigationVertical : GalleryNavigationVertical {
		public PopupEditGalleryNavigationVertical(PopupGalleryEditGallery gallery)
			: base(gallery) {
		}
		public override void OnKeyDown(KeyEventArgs e) {
			if(!Gallery.HasCheckedItems) {
				if(e.KeyCode == Keys.Up || e.KeyCode == Keys.PageUp) {
					Gallery.ViewInfo.MoveLastVisibleItem();
					return;
				}
				if(e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown) {
					Gallery.ViewInfo.MoveFirstVisibleItem();
					return;
				}
			}
			base.OnKeyDown(e);
		}
	}
	public class PopupEditGalleryControlHandler : GalleryControlHandler {
		public PopupEditGalleryControlHandler(PopupGalleryEditGallery gallery)
			: base(gallery) {
		}
		protected override GalleryNavigationBase CreateNavigator() {
			if(Gallery.Orientation == Orientation.Vertical)
				return new PopupEditGalleryNavigationVertical(Gallery);
			return base.CreateNavigator();
		}
		public new PopupGalleryEditGallery Gallery { get { return base.Gallery as PopupGalleryEditGallery; } }
		public override bool IsNeededKey(KeyEventArgs e) {
			return e.KeyCode == Keys.Left || 
				e.KeyCode == Keys.Up || 
				e.KeyCode == Keys.Right || 
				e.KeyCode == Keys.Down || 
				e.KeyCode == Keys.Space || 
				e.KeyCode == Keys.PageDown || 
				e.KeyCode == Keys.PageUp || 
				e.KeyCode == Keys.Home || 
				e.KeyCode == Keys.End;
		}
	}
	public class PopupGalleryEditGallery : GalleryControlGallery {
		public PopupGalleryEditGallery(RepositoryItemPopupGalleryEdit gallery)
			: base() {
			GalleryItem = gallery;
		}
		public PopupGalleryEditGallery() : base() { 
		}
		public PopupGalleryEditGallery(GalleryControl gallery) : base(gallery) { 
		}
		protected override ItemCheckMode DefaultItemCheckMode {
			get { return ItemCheckMode.SingleCheck; }
		}
		protected override bool DefaultCheckSelectedItemViaKeyboard {
			get { return true; }
		}
		protected override GalleryControlHandler CreateHandler() {
			return new PopupEditGalleryControlHandler(this);
		}
		public RepositoryItemPopupGalleryEdit GalleryItem { get; private set; }
		bool closeOnItemClick = true;
		[DefaultValue(true)]
		public bool CloseOnItemClick {
			get { return closeOnItemClick; }
			set { closeOnItemClick = value; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseGalleryItemCheckMode"),
#endif
 System.ComponentModel.Category("Appearance"), DefaultValue(ItemCheckMode.SingleCheck)]
		public override ItemCheckMode ItemCheckMode {
			get { return base.ItemCheckMode; }
			set { base.ItemCheckMode = value; }
		}
		bool ShouldCloseOnItemClick {
			get {
				if(GalleryControl == null)
					return CloseOnItemClick;
				PopupGalleryForm form = GalleryControl.FindForm() as PopupGalleryForm;
				if(form == null || form.OwnerGalleryEdit == null)
					return CloseOnItemClick;
				if(form.OwnerGalleryEdit.Properties.ShowButtons)
					return false;
				return CloseOnItemClick;
			}
		}
		protected override void OnItemClickCore(DevExpress.XtraBars.RibbonGalleryBarItemLink inRibbonGalleryLink, BaseGallery gallery, GalleryItem item) {
			if(GalleryControl != null && ShouldCloseOnItemClick) {
				PopupGalleryForm form = GalleryControl.FindForm() as PopupGalleryForm;
				if(form.OwnerGalleryEdit != null)
					form.OwnerGalleryEdit.ClosePopup();
			}
			base.OnItemClickCore(inRibbonGalleryLink, gallery, item);
		}
		protected internal override void FireGalleryChanged() {
			if(PopupGalleryEdit != null)
				PopupGalleryEdit.FireGalleryChanged();
		}
		protected internal override void OnItemPropertiesChanged(string propName, bool updateLayout) {
			base.OnItemPropertiesChanged(propName, updateLayout);
			if(GalleryItem == null)
				return;
			if(propName == "Caption" || propName == "Value") {
				GalleryItem.OnGalleryEditPropertiesChanged();
			}
		}
		protected override void AssignParams(BaseGallery gallery) {
			base.AssignParams(gallery);
			PopupGalleryEditGallery pgallery = gallery as PopupGalleryEditGallery;
			if(pgallery == null)
				return;
			this.closeOnItemClick = pgallery.CloseOnItemClick;
		}
		protected internal override void AssignEventHandlers(BaseGallery gallery) {
			base.AssignEventHandlers(gallery);
			PopupGalleryEditGallery pgallery = gallery as PopupGalleryEditGallery;
			if(pgallery == null)
				return;
			AssignEvent(itemClickEvent, pgallery);
			AssignEvent(itemDoubleClickEvent, pgallery);
			AssignEvent(itemCheckedChanged, pgallery);
			AssignEvent(endScroll, pgallery);
			AssignEvent(customDrawItemImage, pgallery);
			AssignEvent(customDrawItemText, pgallery);
			AssignEvent(filterMenuPopupEvent, pgallery);
			AssignEvent(filterMenuItemClickEvent, pgallery);
			AssignEvent(galleryItemHover, pgallery);
			AssignEvent(galleryItemLeave, pgallery);
			AssignEvent(marqueeSelectionCompleted, pgallery);
		}
		void AssignEvent(object obj, PopupGalleryEditGallery pgallery) {
			Delegate handle = (Delegate)pgallery.Events[obj];
			if(handle != null)
				Events[obj] = handle;
		}
		void ReleaseEvent(object obj) {
			Delegate handle = (Delegate)Events[obj];
			if(handle != null)
				Events.RemoveHandler(obj, handle);
		}
		protected internal override void ReleaseEventHandlers() {
			base.ReleaseEventHandlers();
			ReleaseEvent(itemClickEvent);
			ReleaseEvent(itemDoubleClickEvent);
			ReleaseEvent(itemCheckedChanged);
			ReleaseEvent(endScroll);
			ReleaseEvent(customDrawItemImage);
			ReleaseEvent(customDrawItemText);
			ReleaseEvent(filterMenuPopupEvent);
			ReleaseEvent(filterMenuItemClickEvent);
			ReleaseEvent(galleryItemHover);
			ReleaseEvent(galleryItemLeave);
			ReleaseEvent(marqueeSelectionCompleted);
		}
	}
	public class CheckedGalleryItemCollectionBase : CollectionBase { 
		public CheckedGalleryItemCollectionBase(PopupGalleryEdit owner) {
			Owner = owner;
		}
		protected PopupGalleryEdit Owner { get; private set; }
		int LockIndex { get; set; }
		public void BeginUpdate() {
			LockIndex++;
		}
		public void CancelUpdate() {
			if(LockIndex == 0)
				return;
			LockIndex--;
		}
		public void EndUpdate() {
			if(LockIndex == 0)
				return;
			LockIndex--;
			if(LockIndex == 0)
				OnCollectionChanged();
		}
		public bool IsLockUpdate { get { return LockIndex > 0; } }
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			if(!IsLockUpdate)
				OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			if(!IsLockUpdate)
				OnCollectionChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(!IsLockUpdate)
				OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			if(!IsLockUpdate)
				OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
		}
	}
	public class CheckedGalleryItemValueCollection : CheckedGalleryItemCollectionBase {
		public CheckedGalleryItemValueCollection(PopupGalleryEdit owner) : base(owner) { }
		public int Add(object value) { return List.Add(value); }
		public void Insert(int index, object value) { List.Insert(index, value); }
		public void Remove(object value) { List.Remove(value); }
		public object this[int index] { get { return List[index]; } set { List[index] = value; } }
		protected override void OnCollectionChanged() {
			base.OnCollectionChanged();
			if(Owner != null)
				Owner.OnCheckedValuesCollectionChanged();
		}
	}
	public class CheckedGalleryItemCollection : CheckedGalleryItemCollectionBase {
		public CheckedGalleryItemCollection(PopupGalleryEdit owner) : base(owner) { }
		public int Add(GalleryItem item) { return List.Add(item); }
		public void Insert(int index, GalleryItem item) { List.Insert(index, item); }
		public void Remove(GalleryItem item) { List.Remove(item); }
		public GalleryItem this[int index] { get { return (GalleryItem)List[index]; } set { List[index] = value; } }
		public bool Contains(GalleryItem item) { return List.Contains(item); }
		protected override void OnCollectionChanged() {
			base.OnCollectionChanged();
			if(Owner != null)
				Owner.OnCheckedItemsCollectionChanged();
		}
	}
}
