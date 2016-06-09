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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraPrinting;
using System.Drawing.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemImageEdit : RepositoryItemBlobBaseEdit {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemImageEdit Properties { get { return this; } }
		private static object imageChanged = new object();
		PictureStoreMode pictureStoreMode;
		[Browsable(false)]
		public override string EditorTypeName { get { return "ImageEdit"; } }
		[ThreadStatic]
		static object defaultImages;
		bool showMenu;
		PictureSizeMode sizeMode;
		ContentAlignment pictureAlignment;
		protected internal override object DefaultImages {
			get {
				if(defaultImages == null)
					defaultImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.ImageEx.bmp", typeof(RepositoryItemImageEdit).Assembly, new Size(16, 13), Color.Magenta);
				return defaultImages;
			}
		}
		public RepositoryItemImageEdit() {
			this.showMenu = true;
			this.sizeMode = PictureSizeMode.Clip;
			this.pictureAlignment = ContentAlignment.MiddleCenter;
			this.pictureStoreMode = PictureStoreMode.Default;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			info.EditValue = info.DisplayText;
			ITextBrick brick = CreateTextBrick(info);
			brick.ForeColor = info.Appearance.ForeColor;
			return brick;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemImageEdit source = item as RepositoryItemImageEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.showMenu = source.ShowMenu;
				this.sizeMode = source.SizeMode;
				this.pictureAlignment = source.PictureAlignment;
				this.pictureStoreMode = source.pictureStoreMode;
			} finally {
				EndUpdate();
			}
			Events.AddHandler(imageChanged, source.Events[imageChanged]);
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageEditPictureStoreMode"),
#endif
 DefaultValue(PictureStoreMode.Default)]
		public PictureStoreMode PictureStoreMode {
			get { return pictureStoreMode; }
			set { pictureStoreMode = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageEditPictureAlignment"),
#endif
 DefaultValue(ContentAlignment.MiddleCenter), SmartTagProperty("Picture Alignment", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public ContentAlignment PictureAlignment {
			get { return pictureAlignment; }
			set {
				if(PictureAlignment == value) return;
				pictureAlignment = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageEditSizeMode"),
#endif
 DefaultValue(PictureSizeMode.Clip), SmartTagProperty("Size Mode", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public PictureSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if(SizeMode == value) return;
				sizeMode = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageEditShowMenu"),
#endif
 DefaultValue(true)]
		public bool ShowMenu {
			get { return showMenu; }
			set {
				if(ShowMenu == value) return;
				showMenu = value;
				OnPropertiesChanged();
			}
		}
		protected internal override void RaisePopupAllowClick(PopupBaseEdit popupBaseEdit, PopupAllowClickEventArgs e) {
			ZoomTrackBarControl control = e.Control as ZoomTrackBarControl;
			if(control != null && control.Properties.Tag is PictureMenu)
				e.Allow = true;
			base.RaisePopupAllowClick(popupBaseEdit, e);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemImageEditImageChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ImageChanged {
			add { this.Events.AddHandler(imageChanged, value); }
			remove { this.Events.RemoveHandler(imageChanged, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseImageChanged(e);
		}
		protected internal virtual void RaiseImageChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[imageChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		public override bool AllowInplaceAutoFilter { get { return false; } }
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultBindingPropertyEx("Image"),
	 Designer("DevExpress.XtraEditors.Design.ImageEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays an image within the drop-down window."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(ImageEditActions), "Image", "Choose Image", SmartTagActionType.CloseAfterExecute),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "ImageEdit")
	]
	public class ImageEdit : BlobBaseEdit {
		public ImageEdit() {
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "ImageEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemImageEdit Properties { get { return base.Properties as RepositoryItemImageEdit; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new ImagePopupForm(this);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageEditImage"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get {
				object ev = EditValue;
				if(ev == null) return null;
				Image img = ev as Image;
				if(img != null) return img;
				return ByteImageConverter.FromByteArray(ByteImageConverter.ToByteArray(ev));
			}
			set { EditValue = value; }
		}
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return Properties.GetDisplayText(EditValue); }
		}
		[Browsable(false)]
		public override object EditValue {
			get { return base.EditValue; }
			set { base.EditValue = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ImageEditImageChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ImageChanged {
			add { Properties.ImageChanged += value; }
			remove { Properties.ImageChanged -= value; }
		}
		public override void ClosePopup() {
			base.ClosePopup();
			ImagePopupForm ipf = PopupForm as ImagePopupForm;
			if(ipf != null) ipf.ForceDisposeTempImage();
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class ImageEditViewInfo : BlobBaseEditViewInfo {
		public ImageEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		protected override void OnEditValueChanged() {
			this.RefreshDisplayText = false;
			if(!IsDataEmpty) {
				this.fDisplayText = Localizer.Active.GetLocalizedString(StringId.ImagePopupPicture);
			} else
				this.fDisplayText = Localizer.Active.GetLocalizedString(StringId.ImagePopupEmpty);
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class ImagePopupForm : BlobBasePopupForm {
		PictureEdit picture;
		protected class PictureEditEx : PictureEdit {
			ImagePopupForm form;
			protected internal override bool AllowForceDisposeTempImage { get { return false; } }
			public PictureEditEx(ImagePopupForm form) {
				this.form = form;
			}
			protected override void OnDialogShowingCore() {
				base.OnDialogShowingCore();
				if(this.form != null) this.form.HideShadows();
			}
			protected override void OnDialogClosedCore() {
				base.OnDialogClosedCore();
				if(this.form != null) {
					this.form.ShowHideShadows();
				}
			}
		}
		internal void ForceDisposeTempImage() {
			if(Picture != null && Picture.Menu != null) Picture.Menu.DisposeTempImage();
		}
		public ImagePopupForm(ImageEdit ownerEdit)
			: base(ownerEdit) {
			this.picture = CreatePictureEdit();
			this.picture.Properties.AllowFocused = false;
			this.picture.BorderStyle = BorderStyles.NoBorder;
			this.picture.Visible = false;
			this.picture.TabStop = false;
			this.picture.FocusOnMouseDown = false;
			this.picture.MenuManager = OwnerEdit.MenuManager;
			this.picture.Properties.PictureStoreMode = OwnerEdit.Properties.PictureStoreMode;
			this.picture.EditValueChanged += new EventHandler(OnPicture_Modified);
			this.Controls.Add(picture);
			CreateSeparatorLine();
			UpdatePictureEdit();
		}
		protected virtual PictureEdit CreatePictureEdit() {
			return new PictureEditEx(this);
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			if(base.AllowMouseClick(control, mousePosition)) return true;
			IPictureMenu pmenu = Picture as IPictureMenu;
			if(pmenu.LockFocus) return true;
			return false;
		}
		protected override Control EmbeddedControl { get { return Picture; } }
		protected virtual PictureEdit Picture { get { return picture; } }
		[Browsable(false)]
		public new ImageEdit OwnerEdit { get { return base.OwnerEdit as ImageEdit; } }
		protected virtual void UpdatePictureEdit() {
			Picture.Properties.BeginUpdate();
			try {
				Picture.Properties.ReadOnly = OwnerEdit.Properties.ReadOnly;
				Picture.Properties.SizeMode = OwnerEdit.Properties.SizeMode;
				Picture.Properties.PictureAlignment = OwnerEdit.Properties.PictureAlignment;
				Picture.Properties.ShowMenu = OwnerEdit.Properties.ShowMenu;
				Picture.Properties.NullText = OwnerEdit.Properties.NullText;
				Picture.MenuManager = OwnerEdit.MenuManager;
			} finally {
				Picture.Properties.EndUpdate();
			}
		}
		[DXCategory(CategoryName.Focus)]
		public override bool FormContainsFocus { get { return base.FormContainsFocus || Picture.EditorContainsFocus; } }
		public override void ShowPopupForm() {
			BeginControlUpdate();
			try {
				Picture.EditValue = OwnerEdit.EditValue;
				Picture.IsModified = false;
			} finally {
				EndControlUpdate();
			}
			base.ShowPopupForm();
			FocusFormControl(Picture);
		}
		protected virtual void OnPicture_Modified(object sender, EventArgs e) {
			if(IsControlUpdateLocked) return;
			OkButton.Enabled = true;
		}
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue { get { return Picture.EditValue; } }
		protected override void OnVisibleChanged(EventArgs e) {
			if(Visible) UpdatePictureEdit();
			base.OnVisibleChanged(e);
		}
	}
}
