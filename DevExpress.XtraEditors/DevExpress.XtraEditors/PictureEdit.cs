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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Drawing.Animation;
using System.Threading;
using System.Net;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Gesture;
using System.Collections.Generic;
using System.Drawing.Design;
using DevExpress.XtraEditors.Events;
using DevExpress.Data;
using DevExpress.XtraEditors.Camera;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public enum ZoomingOperationMode { Default, MouseWheel, ControlMouseWheel }
	public class RepositoryItemPictureEdit : RepositoryItem, IContextItemCollectionOptionsOwner, IContextItemCollectionOwner {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemPictureEdit Properties { get { return this; } }
		static object imageChanged = new object();
		static readonly object loadCompletedKey = new object();
		static object zoomPercentChanged = new object();
		static object popupMenuShowing = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object customContextButtonToolTip = new object();
		private static readonly object contextButtonValueChanged = new object();
		PictureEditCaption caption;
		int customHeight;
		bool showMenu, showScrollBars, allowScrollViaMouseDrag, useMetafiles, useDisabledStatePainter;
		bool allowDisposeImage = false;
		DefaultBoolean showZoomSubMenu;
		CameraMenuItemVisibility showCameraMenuItem;
		Image initialImage;
		Image errorImage;
		PictureStoreMode pictureStoreMode;
		PictureSizeMode sizeMode;
		ContentAlignment pictureAlignment;
		InterpolationMode pictureInterpolationMode;
		Padding padding;
		DefaultBoolean allowZoomOnMouseWheel = DefaultBoolean.Default;
		ZoomingOperationMode zoomingOperationMode = ZoomingOperationMode.Default;
		[ThreadStatic]
		static ImageCollection defaultImages;
		[ThreadStatic]
		static Image defaultInitialImage;
		public RepositoryItemPictureEdit() {
			this.pictureAlignment = ContentAlignment.MiddleCenter;
			this.pictureInterpolationMode = InterpolationMode.Default;
			this.customHeight = 0;
			this.pictureStoreMode = PictureStoreMode.Default;
			this.showMenu = true;
			this.showZoomSubMenu = DefaultBoolean.Default;
			this.showCameraMenuItem = CameraMenuItemVisibility.Never;
			this.showScrollBars = false;
			this.useDisabledStatePainter = true;
			this.allowScrollViaMouseDrag = false;
			this.useMetafiles = false;
			this.sizeMode = DefaultSizeMode;
			this.fAllowFocused = true;
			this.initialImage = DefaultInitialImage;
			this.errorImage = DefaultImages.Images[1];
			this.allowDisposeImage = false;
			this.padding = DefaultPadding;
			this.UseDefaultLayoutMode = true;
			this.caption = CreatePictureEditCaption();
		}
		protected virtual PictureEditCaption CreatePictureEditCaption() {
			return new PictureEditCaption(this);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public PictureEditCaption Caption { get { return caption; } }
		ContextItemCollection contextButtons;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtonsCollection();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		ContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null) {
					contextButtonOptions = CreateContextButtonOptions();
				}
				return contextButtonOptions;
			}
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			if(OwnerEdit != null)
				(OwnerEdit.ViewInfo as PictureEditViewInfo).IsContextButtonsReady = false;
			OnPropertiesChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Checked" || propertyName == "Value" || propertyName == "Rating")
				RaiseContextButtonValueChanged(new ContextButtonValueEventArgs(item, newValue));
			if(propertyName == "Visibility" || propertyName == "Value") {
				if(OwnerEdit != null) {
					OwnerEdit.Invalidate();
					OwnerEdit.Update();
				}
				return;
			}
			OnPropertiesChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode { get { return IsDesignMode; } }
		bool IContextItemCollectionOwner.IsRightToLeft {
			get { 
				if(OwnerEdit != null) return OwnerEdit.IsRightToLeft;
				return false;
			}
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.OpacityAnimation; } }
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new ContextItemCollectionOptions(this);
		}
		protected virtual ContextItemCollection CreateContextButtonsCollection() {
			return new ContextItemCollection(this);
		}
		protected Image GetBrickImage(PictureEditViewInfo peVi, PrintCellHelperInfo info) {
			MultiKey key = new MultiKey(new object[] { peVi.Bounds.Size, info.EditValue, this.AutoHeight, this.BorderStyle, this.Enabled, this.PictureAlignment, this.SizeMode, this.EditorTypeName });
			Image img = GetCachedPrintImage(key, info.PS);
			if(img != null) return img;
			using(BitmapGraphicsHelper gHelper = new BitmapGraphicsHelper(peVi.Bounds.Width, peVi.Bounds.Height)) {
				peVi.Bounds = new Rectangle(Point.Empty, gHelper.Bitmap.Size);
				peVi.CalcViewInfo(gHelper.Graphics);
				PictureEditPainter painter = new DevExpress.XtraEditors.Drawing.PictureEditPainter();
				ControlGraphicsInfoArgs args = new ControlGraphicsInfoArgs(peVi, new GraphicsCache(gHelper.Graphics), new Rectangle(Point.Empty, gHelper.Bitmap.Size));
				painter.Draw(args);
				return AddImageToPrintCache(key, gHelper.MemSafeBitmap, info.PS);
			}
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			IImageBrick brick = CreateImageBrick(info, CreateBrickStyle(info, "image"));
			object image;
			if(info.EditValue is Image || info.EditValue is Icon) {
				image = info.EditValue;
				if(info.EditValue is Icon) image = ((Icon)info.EditValue).ToBitmap();
			}
			else
				image = ByteImageConverter.FromByteArray(ByteImageConverter.ToByteArray(info.EditValue));
			PictureEditViewInfo peVi = PreparePrintViewInfo(info, false) as PictureEditViewInfo;
			switch(SizeMode) {
				case Controls.PictureSizeMode.Clip:
					brick.SizeMode = ImageSizeMode.Normal;
					break;
				case Controls.PictureSizeMode.Stretch:
					brick.SizeMode = ImageSizeMode.StretchImage;
					break;
				case Controls.PictureSizeMode.Zoom:
					brick.SizeMode = ImageSizeMode.ZoomImage;
					break;
				case Controls.PictureSizeMode.StretchHorizontal:
				case Controls.PictureSizeMode.Squeeze:
					brick.SizeMode = ImageSizeMode.Squeeze;
					break;
				case Controls.PictureSizeMode.StretchVertical:
					brick.SizeMode = ImageSizeMode.CenterImage;
					break;
			}
			peVi.Image = (Image)image;
			brick.Image = GetBrickImage(peVi, info);
			return brick;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemPictureEdit source = item as RepositoryItemPictureEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.pictureStoreMode = source.pictureStoreMode;
				this.customHeight = source.CustomHeight;
				this.pictureAlignment = source.PictureAlignment;
				this.showMenu = source.ShowMenu;
				this.showZoomSubMenu = source.ShowZoomSubMenu;
				this.showCameraMenuItem = source.ShowCameraMenuItem;
				this.showScrollBars = source.ShowScrollBars;
				this.allowScrollViaMouseDrag = source.AllowScrollViaMouseDrag;
				this.useMetafiles = source.UseMetafiles;
				this.zoomFactor = source.ZoomFactor;
				this.sizeMode = source.SizeMode;
				this.pictureInterpolationMode = source.PictureInterpolationMode;
				this.allowDisposeImage = source.AllowDisposeImage;
				this.useDisabledStatePainter = source.UseDisabledStatePainter;
				this.allowZoomOnMouseWheel = source.AllowZoomOnMouseWheel;
				this.allowScrollOnMouseWheel = source.AllowScrollOnMouseWheel;
				this.zoomingOperationMode = source.ZoomingOperationMode;
				this.enableLODImages = source.EnableLODImages;
				this.lODImageMinSize = source.LODImageMinSize;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(imageChanged, source.Events[imageChanged]);
			Events.AddHandler(loadCompletedKey, source.Events[loadCompletedKey]);
			Events.AddHandler(zoomPercentChanged, source.Events[zoomPercentChanged]);
			Events.AddHandler(popupMenuShowing, source.Events[popupMenuShowing]);
			Events.AddHandler(customContextButtonToolTip, source.Events[customContextButtonToolTip]);
			Events.AddHandler(contextButtonValueChanged, source.Events[contextButtonValueChanged]);
		}
		internal static Image DefaultInitialImage {
			get {
				if(defaultInitialImage == null)
					defaultInitialImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.loading.gif", typeof(PictureEdit).Assembly);
				return defaultInitialImage;
			}
		}
		static ImageCollection DefaultImages {
			get {
				if(defaultImages == null)
					defaultImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.PictureEditDefaultAsyncImages.png", typeof(PictureEdit).Assembly, new Size(16, 16), Color.Magenta);
				return defaultImages;
			}
		}
		protected virtual void ResetInitialImage() {
			InitialImage = DefaultInitialImage;
		}
		protected virtual bool ShouldSerializeInitialImage() {
			return !ImagesComparer.AreEqual(InitialImage, DefaultInitialImage);
		}
		bool enableLODImages = false;
		[DefaultValue(false)]
		public bool EnableLODImages {
			get { return enableLODImages; }
			set {
				if(EnableLODImages == value)
					return;
				enableLODImages = value;
				OnPropertiesChanged();
			}
		}
		int lODImageMinSize = 256;
		[DefaultValue(256)]
		internal int LODImageMinSize {
			get { return lODImageMinSize; }
			set {
				if(lODImageMinSize == value)
					return;
				lODImageMinSize = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditInitialImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image InitialImage {
			get {
				return initialImage;
			}
			set {
				if(ImagesComparer.AreEqual(initialImage, value)) return;
				initialImage = value;
			}
		}
		protected virtual void ResetErrorImage() {
			ErrorImage = DefaultImages.Images[1];
		}
		protected virtual bool ShouldSerializeErrorImage() {
			return !ImagesComparer.AreEqual(ErrorImage, DefaultImages.Images[1]);
		}
		public override bool AllowInplaceAutoFilter { get { return false; } }
		public override bool IsNonSortableEditor { get { return true; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditAllowDisposeImage"),
#endif
 DefaultValue(false)]
		public bool AllowDisposeImage {
			get { return allowDisposeImage; }
			set { allowDisposeImage = value; }
		}
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowZoomOnMouseWheel {
			get { return allowZoomOnMouseWheel; }
			set { allowZoomOnMouseWheel = value; }
		}
		DefaultBoolean allowScrollOnMouseWheel = DefaultBoolean.Default;
		[DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowScrollOnMouseWheel {
			get { return allowScrollOnMouseWheel; }
			set { allowScrollOnMouseWheel = value; }
		}
		[DefaultValue(ZoomingOperationMode.Default)]
		public ZoomingOperationMode ZoomingOperationMode {
			get { return zoomingOperationMode; }
			set { zoomingOperationMode = value; }
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditErrorImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image ErrorImage {
			get {
				return errorImage;
			}
			set {
				if(ImagesComparer.AreEqual(errorImage, value)) return;
				errorImage = value;
			}
		}
		protected virtual Padding DefaultPadding { get { return Padding.Empty; } }
		bool ShouldSerializePadding() { return Padding != DefaultPadding; }
		void ResetPadding() { Padding = DefaultPadding; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditPadding")
#else
	Description("")
#endif
]
		public Padding Padding {
			get { return padding; }
			set {
				if(Padding == value)
					return;
				padding = value;
				OnPropertiesChanged();
				OnScrollUpdate();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditAllowFocused"),
#endif
 DefaultValue(true)]
		public override bool AllowFocused {
			get { return base.AllowFocused; }
			set { base.AllowFocused = value; }
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditPictureStoreMode"),
#endif
 DefaultValue(PictureStoreMode.Default)]
		public PictureStoreMode PictureStoreMode {
			get { return pictureStoreMode; }
			set { pictureStoreMode = value; }
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "PictureEdit"; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditCustomHeight"),
#endif
 DefaultValue(0)]
		public int CustomHeight {
			get { return customHeight; }
			set {
				if(value < 0) value = 0;
				if(CustomHeight == value) return;
				customHeight = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditShowMenu"),
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
		internal bool AllowSubMenu {
			get {
				if(ShowZoomSubMenu == DefaultBoolean.Default) return ShowScrollBars;
				return ShowZoomSubMenu == DefaultBoolean.True;
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditShowZoomSubMenu"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowZoomSubMenu {
			get { return showZoomSubMenu; }
			set {
				if(ShowZoomSubMenu == value) return;
				showZoomSubMenu = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(CameraMenuItemVisibility.Never)]
		public CameraMenuItemVisibility ShowCameraMenuItem {
			get { return showCameraMenuItem; }
			set {
				if(ShowCameraMenuItem == value) return;
				showCameraMenuItem = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditUseDisabledStatePainter"),
#endif
 DefaultValue(true)]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainter; }
			set {
				if(UseDisabledStatePainter == value) return;
				useDisabledStatePainter = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditShowScrollBars"),
#endif
 DefaultValue(false)]
		public bool ShowScrollBars {
			get { return showScrollBars; }
			set {
				if(ShowScrollBars == value) return;
				showScrollBars = value;
				OnPropertiesChanged();
				OnScrollUpdate();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditAllowScrollViaMouseDrag"),
#endif
 DefaultValue(false)]
		public bool AllowScrollViaMouseDrag {
			get { return allowScrollViaMouseDrag; }
			set {
				if(AllowScrollViaMouseDrag == value) return;
				allowScrollViaMouseDrag = value;
				OnPropertiesChanged();
				OnScrollUpdate();
			}
		}
		[DXCategory(CategoryName.Behavior),  DefaultValue(false)]
		protected internal virtual bool UseMetafiles {
			get { return useMetafiles; }
			set {
				if(UseMetafiles == value) return;
				useMetafiles = value;
				OnPropertiesChanged();
				OnScrollUpdate();
			}
		}
		internal double ZoomFactorCore {
			get {
				if(SizeMode != PictureSizeMode.Clip && SizeMode != PictureSizeMode.Squeeze && SizeMode != PictureSizeMode.Zoom) return 1;
				return (double)ZoomFactor / 100;
			}
		}
		double zoomFactor = 100;
		internal double ZoomFactor {
			get { return zoomFactor; }
			set {
				if(ZoomFactor == value) return;
				SetZoomFactor(value);
			}
		}
		bool ShouldSerializeZoomPercent() {
			return ZoomPercent != 100.0;
		}
		void ResetZoomPercent() { ZoomPercent = 100; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditZoomPercent"),
#endif
 SmartTagProperty("Zoom Percent", "")]
		public double ZoomPercent {
			get { return ZoomFactor; }
			set { ZoomFactor = value; }
		}
		internal void SetZoomFactor(double val) {
			double oldValue = ZoomFactor;
			if(val < 1) val = 1;
			if(val > 10000) val = 10000;
			zoomFactor = val;
			OnZoomUpdate((double)oldValue, (double)ZoomFactor);
			OnPropertiesChanged();
			RaiseZoomPercentChanged(EventArgs.Empty);
		}
		protected virtual PictureSizeMode DefaultSizeMode { get { return PictureSizeMode.Clip; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoHeight { get { return false; } }
		bool ShouldSerializeSizeMode() { return SizeMode != DefaultSizeMode; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditSizeMode"),
#endif
 SmartTagProperty("Size Mode", "")]
		public virtual PictureSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if(SizeMode == value) return;
				sizeMode = value;
				OnSizeModeChanged();
			}
		}
		private void OnSizeModeChanged() {
			UseDefaultLayoutMode = true;
			OnPropertiesChanged();
			OnScrollUpdate();
		}
		void OnScrollUpdate() {
			if(PictureOwnerEdit != null) {
				if(!PictureOwnerEdit.lockUpdateScrollers)
					PictureOwnerEdit.Scrollers.ForceDisplayScrollBars();
				PictureOwnerEdit.Scrollers.UpdateCursors();
			}
		}
		void OnTryScrrollUpdate() {
			if(PictureOwnerEdit != null)
				PictureOwnerEdit.Scrollers.TryDisplayScrollBars(true);
		}
		internal bool UpdateZoomFromMouseWheel { get; set; }
		void OnZoomUpdate(double zoom1, double zoom2) {
			if(!IsLoading && UpdateZoomFromMouseWheel)
				UseDefaultLayoutMode = false;
			if(PictureOwnerEdit != null)
				PictureOwnerEdit.Scrollers.UpdateZoomScrollBars(zoom1, zoom2);
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditPictureAlignment"),
#endif
 DefaultValue(ContentAlignment.MiddleCenter), SmartTagProperty("Picture Alignment", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public ContentAlignment PictureAlignment {
			get { return pictureAlignment; }
			set {
				if(PictureAlignment == value) return;
				pictureAlignment = value;
				OnPropertiesChanged();
				OnScrollUpdate();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditPictureInterpolationMode"),
#endif
 DefaultValue(InterpolationMode.Default)]
		public InterpolationMode PictureInterpolationMode {
			get { return pictureInterpolationMode; }
			set {
				if(PictureInterpolationMode == value) return;
				pictureInterpolationMode = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditImageChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ImageChanged {
			add { this.Events.AddHandler(imageChanged, value); }
			remove { this.Events.RemoveHandler(imageChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditLoadCompleted"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler LoadCompleted {
			add { Events.AddHandler(loadCompletedKey, value); }
			remove { Events.RemoveHandler(loadCompletedKey, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditZoomPercentChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ZoomPercentChanged {
			add { this.Events.AddHandler(zoomPercentChanged, value); }
			remove { this.Events.RemoveHandler(zoomPercentChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPictureEditPopupMenuShowing"),
#endif
 DXCategory(CategoryName.Events)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { this.Events.AddHandler(popupMenuShowing, value); }
			remove { this.Events.RemoveHandler(popupMenuShowing, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseImageChanged(e);
		}
		protected internal virtual void RaiseImageChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[imageChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseLoadCompleted(EventArgs e) {
			EventHandler handler = (EventHandler)Events[loadCompletedKey];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseZoomPercentChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[zoomPercentChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaisePopupMenuShowing(PopupMenuShowingEventArgs e) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[popupMenuShowing];
			if(handler != null) handler(GetEventSender(), e);
		}
		PictureEdit PictureOwnerEdit {
			get {
				return OwnerEdit as PictureEdit;
			}
		}
		protected internal bool UseDefaultLayoutMode { get; set; }
		public ImageLayoutMode GetImageLayoutMode() {
			switch(SizeMode) {
				case PictureSizeMode.Squeeze: return ImageLayoutMode.Squeeze;
				case PictureSizeMode.Stretch: return ImageLayoutMode.Stretch;
				case PictureSizeMode.StretchHorizontal: return ImageLayoutMode.StretchHorizontal;
				case PictureSizeMode.StretchVertical: return ImageLayoutMode.StretchVertical;
				case PictureSizeMode.Zoom: return ImageLayoutMode.ZoomInside;
			}
			switch(PictureAlignment) {
				case ContentAlignment.BottomCenter: return ImageLayoutMode.BottomCenter;
				case ContentAlignment.BottomLeft: return ImageLayoutMode.BottomLeft;
				case ContentAlignment.BottomRight: return ImageLayoutMode.BottomRight;
				case ContentAlignment.MiddleCenter: return ImageLayoutMode.MiddleCenter;
				case ContentAlignment.MiddleLeft: return ImageLayoutMode.MiddleLeft;
				case ContentAlignment.MiddleRight: return ImageLayoutMode.MiddleRight;
				case ContentAlignment.TopCenter: return ImageLayoutMode.TopCenter;
				case ContentAlignment.TopLeft: return ImageLayoutMode.TopLeft;
				case ContentAlignment.TopRight: return ImageLayoutMode.TopRight;
			}
			return ImageLayoutMode.Squeeze;
		}
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		protected internal void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event ContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		protected internal void RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			ContextButtonToolTipEventHandler handler = Events[customContextButtonToolTip] as ContextButtonToolTipEventHandler;
			if(handler != null)
				handler(this, e);
		}
		public event ContextButtonValueChangedEventHandler ContextButtonValueChanged {
			add { Events.AddHandler(contextButtonValueChanged, value); }
			remove { Events.RemoveHandler(contextButtonValueChanged, value); }
		}
		protected internal void RaiseContextButtonValueChanged(ContextButtonValueEventArgs e) {
			ContextButtonValueChangedEventHandler handler = Events[contextButtonValueChanged] as ContextButtonValueChangedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		internal void OnPropertiesChangedCore() {
			OnPropertiesChanged();
		}
	}
}
namespace DevExpress.XtraEditors {
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultBindingPropertyEx("Image"), Docking(DockingBehavior.Ask),
	 Designer("DevExpress.XtraEditors.Design.PictureEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an end-user to edit an image."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(PictureEditActions), "ChooseImage", "Choose Image"),
	 SmartTagAction(typeof(PictureEditActions), "ErrorImage", "Error Image"), SmartTagAction(typeof(PictureEditActions), "InitialImage", "Initial Image"), ToolboxBitmap(typeof(ToolboxIconsRootNS), "PictureEdit")
	]
	public class PictureEdit : BaseEdit, IPictureMenu, IGestureClient, IMouseWheelSupport, ISupportContextItemsCursor {
		PictureMenu menu;
		bool lockFocus;
		internal PictureEditScrollers Scrollers;
		protected internal virtual bool AllowForceDisposeTempImage {
			get {
				return this.InplaceType == XtraEditors.Controls.InplaceType.Standalone;
			}
		}
		public PictureEdit() {
			this.menu = null;
			this.TabStop = false;
			this.lockFocus = false;
			this.imageLoader = new BackgroundImageLoader();
			this.imageLoader.Loaded += new BackgroundImageLoaderEventHandler(OnImageLoader_Loaded);
			this.Scrollers = new PictureEditScrollers(this);
		}
		public Point ImageToViewport(Point pt) {
			CheckViewInfo();
			float scaleX = ViewInfo.PictureScreenBounds.Width / ViewInfo.PictureSourceBounds.Width;
			float scaleY = ViewInfo.PictureScreenBounds.Height / ViewInfo.PictureSourceBounds.Height;
			float dx = pt.X - ViewInfo.PictureSourceBounds.X;
			float dy = pt.Y - ViewInfo.PictureSourceBounds.Y;
			return new Point((int)(ViewInfo.PictureSourceBounds.X + dx * scaleX), (int)(ViewInfo.PictureScreenBounds.Y + dy * scaleY));
		}
		public Point ViewportToImage(Point pt) {
			CheckViewInfo();
			float scaleX = ViewInfo.PictureScreenBounds.Width / ViewInfo.PictureSourceBounds.Width;
			float scaleY = ViewInfo.PictureScreenBounds.Height / ViewInfo.PictureSourceBounds.Height;
			float dx = pt.X - ViewInfo.PictureScreenBounds.X;
			float dy = pt.Y - ViewInfo.PictureScreenBounds.Y;
			return new Point((int)(ViewInfo.PictureSourceBounds.X + dx / scaleX), (int)(ViewInfo.PictureSourceBounds.Y + dy / scaleY));
		}
		protected void CheckViewInfo() {
			if(!ViewInfo.IsReady)
				ViewInfo.CalcViewInfo(null);
		}
		[Browsable(false)]
		public HScrollBar HScrollBar { get { return Scrollers.HScrollBar; } }
		[Browsable(false)]
		public VScrollBar VScrollBar { get { return Scrollers.VScrollBar; } }
		protected internal virtual bool AllowDrawImageDisabled {
			get { return Properties.UseDisabledStatePainter; }
		}
		protected internal virtual string EmptyImageText { get { return null; } }
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			ContextButtonsHandler.ViewInfo = ViewInfo.ContextButtonsViewInfo;
			ContextButtonsHandler.OnMouseEnter(e);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			ContextButtonsHandler.ViewInfo = ViewInfo.ContextButtonsViewInfo;
			ContextButtonsHandler.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			if(disposing) {
				if(this.imageLoader != null)
					this.imageLoader.Dispose();
				if(this.menu != null) {
					this.menu.Dispose();
					this.menu = null;
				}
				if(Scrollers != null) {
					Scrollers.Dispose();
					Scrollers = null;
				}
				if(ViewInfo != null)
					ViewInfo.StopAnimation();
			}
			base.Dispose(disposing);
		}
		void IPictureMenu.OnDialogShowing() { OnDialogShowingCore(); }
		void IPictureMenu.OnDialogClosed() { OnDialogClosedCore(); }
		Control IPictureMenu.OwnerControl { get { return this; } }
		protected virtual void OnDialogShowingCore() { }
		protected virtual void OnDialogClosedCore() { }
		Size defaultMinSize = new Size(20, 20);
		protected override Size CalcSizeableMinSize() { return defaultMinSize; }
		protected override Size CalcSizeableMaxSize() { return Size.Empty; }
		PictureStoreMode IPictureMenu.PictureStoreMode { get { return Properties.PictureStoreMode; } }
		Image IPictureMenu.Image {
			get { return ViewInfo.Image; }
			set { Image = value; }
		}
		bool IPictureMenu.LockFocus {
			get { return lockFocus; }
			set { lockFocus = value; }
		}
		public virtual void StartAnimation() { ViewInfo.StartAnimation(); }
		public virtual void StopAnimation() { ViewInfo.StopAnimation(); }
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!Visible) StopAnimation();
			else StartAnimation();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			StartAnimation();
		}
		protected override void OnEditValueChanging(ChangingEventArgs e) {
			base.OnEditValueChanging(e);
		}
		protected override void OnEditValueChanged() {
			ResetToDefaultLayoutMode();
			Scrollers.HScrollVisible = false;
			Scrollers.VScrollVisible = false;
			base.OnEditValueChanged();
			this.lastImageIsAsync = false;
		}
		protected virtual void ResetToDefaultLayoutMode() {
			Properties.UseDefaultLayoutMode = true;
			Scrollers.HorizontalScrollPosition = 0.0f;
			Scrollers.VerticalScrollPosition = 0.0f;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float HorizontalScrollPosition { get { return Scrollers.HorizontalScrollPosition; } set { Scrollers.HorizontalScrollPosition = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public float VerticalScrollPosition { get { return Scrollers.VerticalScrollPosition; } set { Scrollers.VerticalScrollPosition = value; } }
		bool IPictureMenu.ReadOnly { get { return Properties.ReadOnly; } }
		[
		Bindable(ControlConstants.NonObjectBindable), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditImage"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))
		]
		public virtual Image Image {
			get {
				object ev = EditValue;
				if(ev == null) return null;
				Image img = ev as Image;
				if(img != null) return img;
				if(ViewInfo != null) return ViewInfo.Image;
				return ByteImageConverter.FromByteArray(ByteImageConverter.ToByteArray(ev));
			}
			set { EditValue = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditTabStop"),
#endif
 DefaultValue(false)]
		public new bool TabStop { get { return base.TabStop; } set { base.TabStop = value; } }
		[Browsable(false)]
		public override bool EditorContainsFocus {
			get {
				if(lockFocus) return true;
				return base.EditorContainsFocus;
			}
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "PictureEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemPictureEdit Properties { get { return base.Properties as RepositoryItemPictureEdit; } }
		protected internal new PictureEditViewInfo ViewInfo { get { return base.ViewInfo as PictureEditViewInfo; } }
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return Properties.GetDisplayText(EditValue); }
		}
		protected override void OnSizeChanged(EventArgs e) {
			ViewInfo.IsContextButtonsReady = false;
			base.OnSizeChanged(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseDown(ee);
			ContextButtonsHandler.ViewInfo = ViewInfo.ContextButtonsViewInfo;
			if(ContextButtonsHandler.OnMouseDown(e))
				return;
			if(Scrollers != null) Scrollers.Owner_MouseDown(e);
			if(ee.Handled) return;
			if(ee.Button == MouseButtons.Right) {
				Focus();
			}
		}
		ContextItemCollectionHandler contextButtonsHandler;
		protected ContextItemCollectionHandler ContextButtonsHandler {
			get {
				if(contextButtonsHandler == null)
					contextButtonsHandler = CreateContextButtonsHandler();
				return contextButtonsHandler;
			}
		}
		private ContextItemCollectionHandler CreateContextButtonsHandler() {
			return new ContextItemCollectionHandler();
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if(e.KeyCode == Keys.Escape) {
				if(BindingManager != null) {
					BindingManager.CancelCurrentEdit();
					e.Handled = true;
				}
			}
			if(e.KeyData == Keys.Enter && EnterMoveNextControl) {
				this.ProcessDialogKey(Keys.Tab);
				e.Handled = true;
			}
		}
		protected internal virtual PictureMenu Menu {
			get {
				if(menu == null) menu = CreatePictureMenu();
				return menu;
			}
		}
		protected virtual PictureMenu CreatePictureMenu() {
			return new PictureMenu(this);
		}
		public void LoadImage() { Menu.LoadImage(); }
		protected override Size DefaultSize { get { return new Size(100, 96); } }
		protected override void OnMouseUp(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseUp(ee);
			if(ContextButtonsHandler.OnMouseUp(e))
				return;	   
			if(Scrollers != null) Scrollers.Owner_MouseUp(e);
			if(ee.Handled) return;
			if(ContextMenuStrip != null) return;
			if(ContextMenu == null && e.Button == MouseButtons.Right && Properties.ShowMenu) {
				Point pos = new Point(e.X, e.Y);
				EditHitInfo hitInfo = ViewInfo.CalcHitInfo(pos);
				if(hitInfo.IsInEdit) {
					PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(Menu, pos);
					Properties.RaisePopupMenuShowing(args);
					if(!args.Cancel) {
						MenuManagerHelper.ShowMenu(args.PopupMenu, Properties.LookAndFeel, MenuManager, this, args.Point);
						if(args.RestoreMenu) menu = null;
					}
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditImageChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ImageChanged {
			add { Properties.ImageChanged += value; }
			remove { Properties.ImageChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditLoadCompleted"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler LoadCompleted {
			add { Properties.LoadCompleted += value; }
			remove { Properties.LoadCompleted -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditZoomPercentChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ZoomPercentChanged {
			add { Properties.ZoomPercentChanged += value; }
			remove { Properties.ZoomPercentChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditPopupMenuShowing"),
#endif
 DXCategory(CategoryName.Events)]
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Properties.PopupMenuShowing += value; }
			remove { Properties.PopupMenuShowing -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditContextButtonClick"),
#endif
 DXCategory(CategoryName.Events)]
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Properties.ContextButtonClick += value; }
			remove { Properties.ContextButtonClick -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PictureEditCustomContextButtonToolTip"),
#endif
 DXCategory(CategoryName.Events)]
		public event ContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Properties.CustomContextButtonToolTip += value; }
			remove { Properties.CustomContextButtonToolTip -= value; }
		}
		#region LoadAsync
		bool lastImageIsAsync = false;
		BackgroundImageLoader imageLoader;
		void OnImageLoader_Loaded(object sender, BackgroundImageLoaderEventArgs e) {
			if(e.Cancelled) return;
			this.lastImageIsAsync = false;
			Image res = this.imageLoader.Result;
			if(e.HasError) {
				Image = Properties.ErrorImage;
			}
			else {
				Image = this.imageLoader.Result;
				ViewInfo.ClearPrevImage();
				this.lastImageIsAsync = Image != null;
			}
			Properties.RaiseLoadCompleted(e);
		}
		public void CancelLoadAsync() {
			imageLoader.Abort();
		}
		[Obsolete("Use LoadInProgress"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool LoadInProgess { get { return LoadInProgress; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool LoadInProgress { get { return imageLoader.Loading; } }
		public void LoadAsync(string url) {
			ViewInfo.SavePrevImage();
			ViewInfo.LastImageIsAsync = this.lastImageIsAsync;
			this.lastImageIsAsync = false;
			this.Image = Properties.InitialImage;
			this.imageLoader.Load(url);
		}
		#endregion
		#region Scrollers
		internal bool lockUpdateScrollers = false;
		protected override void OnValidating(CancelEventArgs e) {
			lockUpdateScrollers = true; 
			try {
				base.OnValidating(e);
			}
			finally {
				lockUpdateScrollers = false;
			}
		}
		public override object EditValue {
			get {
				return base.EditValue;
			}
			set {
				base.EditValue = value;
				ViewInfo.IsContextButtonsReady = false;
				UpdateScrollBars();
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(Scrollers != null) Scrollers.OnResize();
		}
		protected override void UpdateViewInfo(Graphics g) {
			base.UpdateViewInfo(g);
			Scrollers.TryDisplayScrollBars(false);
		}
		public void UpdateScrollBars() {
			if(!lockUpdateScrollers)
				Scrollers.ForceDisplayScrollBars();
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(XtraForm.ProcessSmartMouseWheel(this, ev)) return;
			OnMouseWheelCore(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ee) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ee);
			base.OnMouseWheel(e);
			if(Scrollers != null && !e.Handled) Scrollers.Owner_MouseWheel(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			ContextButtonsHandler.ViewInfo = ViewInfo.ContextButtonsViewInfo;
			ContextButtonsHandler.OnMouseMove(e);
			base.OnMouseMove(e);
			if(Scrollers != null) Scrollers.Owner_MouseMove(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(Scrollers != null) Scrollers.Owner_KeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			if(Scrollers != null) Scrollers.Owner_KeyUp(e);
		}
		protected override void OnStyleChanged(EventArgs e) {
			base.OnStyleChanged(e);
			if(Scrollers != null) Scrollers.UpdateScrollStyles();
		}
		#endregion
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m)) return;
			if(m.Msg == DevExpress.Utils.Drawing.Helpers.MSG.WM_MOUSEHWHEEL) {
				DXMouseEventArgs me = ControlHelper.GenerateMouseHWheel(ref m, this);
				OnMouseWheel(me);
				if(me.Handled) {
					m.Result = new IntPtr(1);
					CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
					return;
				}
			}
			base.WndProc(ref m);
		}
		public override Cursor Cursor {
			get { return base.Cursor; }
			set {
				base.Cursor = value;
				if(Scrollers != null)
					Scrollers.UpdateDefaultCursor(base.Cursor);
			}
		}
		public virtual ContextItemHitInfo CalcContextButtonHitInfo(Point point) {
			return ViewInfo.CalcContextButtonHitInfo(point);
		}
		#region IGestureClient Members
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(ViewInfo == null) return GestureAllowArgs.None;
			List<GestureAllowArgs> res = new List<GestureAllowArgs>();
			if(Scrollers.HScrollVisible || Scrollers.VScrollVisible) {
				res.Add(new GestureAllowArgs() {
					GID = GID.PAN,
					AllowID = GestureHelper.GC_PAN_ALL & (~(Scrollers.HScrollVisible ? 0 : GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY)) &
					(~(Scrollers.VScrollVisible ? 0 : GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY))
				});
			}
			res.Add(GestureAllowArgs.Zoom);
			res.Add(GestureAllowArgs.TwoFingerTap);
			return res.ToArray();
		}
		IntPtr IGestureClient.Handle { get { return IsHandleCreated ? Handle : IntPtr.Zero; } }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) { }
		Point over = Point.Empty;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				over = Point.Empty;
				return;
			}
			if(delta.IsEmpty) return;
			if(Scrollers.VScrollBar != null && delta.Y != 0) {
				float oldValue = Scrollers.VerticalScrollPosition;
				Scrollers.VerticalScrollPosition += -delta.Y;
				over.Y = (oldValue == Scrollers.VerticalScrollPosition ? over.Y + delta.Y : 0);
			}
			if(Scrollers.HScrollBar != null && delta.X != 0) {
				float oldValue = Scrollers.HorizontalScrollPosition;
				Scrollers.HorizontalScrollPosition += -delta.X;
				over.X = (oldValue == Scrollers.HorizontalScrollPosition ? over.X + delta.X : 0);
			}
			overPan = over;
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		DateTime lastTwoFingerTap = DateTime.MinValue;
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
			if(!info.IsBegin) return;
			if(DateTime.Now.Subtract(lastTwoFingerTap).TotalMilliseconds < 500) {
				if(Properties.SizeMode == PictureSizeMode.Clip) {
					Properties.SizeMode = PictureSizeMode.Squeeze;
				}
				else {
					if(Properties.SizeMode == PictureSizeMode.Squeeze) {
						Properties.SizeMode = PictureSizeMode.Clip;
						Properties.ZoomFactor = 150;
					}
				}
			}
			this.lastTwoFingerTap = DateTime.Now;
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		double effectiveZoom = 1;
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			if(info.IsBegin) {
				effectiveZoom = (double)Properties.ZoomFactor;
				Properties.SizeMode = PictureSizeMode.Clip;
				Properties.UseDefaultLayoutMode = false;
			}
			if(zoomDelta == 0) return;
			if(zoomDelta < 1 && (!Scrollers.ShouldShowHorizontalScroll && !Scrollers.ShouldShowVerticalScroll)) return;
			effectiveZoom = effectiveZoom * zoomDelta;
			Scrollers.OnZoom(center, effectiveZoom - (double)Properties.ZoomFactor, false);
		}
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		#endregion
		void ISupportContextItemsCursor.SetCursor(Cursor cursor) {
			Scrollers.LockUpdateDefaultCursor = true;
			try {
				Cursor = cursor;
			}
			finally {
				Scrollers.LockUpdateDefaultCursor = false;
			}
		}
		protected override void OnRightToLeftChanged() {
			base.OnRightToLeftChanged();
			Scrollers.TryDisplayScrollBars(true);
		}
		public string GetLoadedImageLocation() { return Menu.GetImageLocation(); }
	}
	public class PictureEditCaption : Caption {
		RepositoryItemPictureEdit owner;
		public PictureEditCaption(RepositoryItemPictureEdit owner) {
			this.owner = owner;
		}
		protected internal RepositoryItemPictureEdit Owner { get { return owner; } }
		protected override void OnChanged() {
			base.OnChanged();
			Owner.OnPropertiesChangedCore();
		}
	}
	internal class PictureEditScrollers : IDisposable, IMouseWheelScrollClient {
		VScrollBar vScrollBar = null;
		HScrollBar hScrollBar = null;
		bool allowUpdate = false;
		public PictureEditScrollers(PictureEdit edit) {
			Owner = edit;
			if(Owner.IsDesignMode) return;
			DefaultCursor = Owner.Cursor;
			HandCursor = DevExpress.Utils.CursorsHelper.LoadFromResource("DevExpress.Utils.Cursors.CursorHand.cur", typeof(DevExpress.Utils.CursorsHelper).Assembly);
			HandDragCursor = DevExpress.Utils.CursorsHelper.LoadFromResource("DevExpress.Utils.Cursors.CursorHandDrag.cur", typeof(DevExpress.Utils.CursorsHelper).Assembly);
			CreateScrollBars();
			VScrollVisible = false;
			HScrollVisible = false;
		}
		protected internal bool LockUpdateDefaultCursor { get; set; }
		protected PictureEdit Owner { get; private set; }
		protected Image Image { get { return Owner.Image; } }
		protected PictureEditViewInfo ViewInfo { get { return Owner.ViewInfo; } }
		protected RepositoryItemPictureEdit Properties { get { return Owner.Properties; } }
		protected Cursor HandCursor { get; set; }
		protected Cursor HandDragCursor { get; set; }
		protected Cursor DefaultCursor { get; set; }
		bool isMouseDrag = false;
		protected bool IsMouseDrag {
			get { return isMouseDrag; }
			set {
				if(IsMouseDrag == value)
					return;
				isMouseDrag = value;
				OnMouseDragChanged();
			}
		}
		protected virtual void OnMouseDragChanged() {
			UpdateCursors();
		}
		protected virtual bool AllowMouseDrag(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left)
				return false;
			return Properties.AllowScrollViaMouseDrag;
		}
		internal void UpdateDefaultCursor(Cursor cursor) {
			if(LockUpdateDefaultCursor) return;
			DefaultCursor = cursor;
		}
		internal void UpdateCursors() {
			if(Owner.IsDesignMode) return;
			LockUpdateDefaultCursor = true;
			try {
				if(!Owner.Properties.AllowScrollViaMouseDrag || (HorizontalMaxScrollPosition == 0 && VerticalMaxScrollPosition == 0) || ViewInfo.ActualSizeMode != PictureSizeMode.Clip) {
					Owner.Cursor = DefaultCursor;
					return;
				}
				Owner.Cursor = IsMouseDrag ? HandDragCursor : HandCursor;
			}
			finally {
				LockUpdateDefaultCursor = false;
			}
		}
		internal void Owner_MouseUp(MouseEventArgs e) {
			IsMouseDrag = false;
		}
		Point MouseLocationPrev { get; set; }
		internal void Owner_MouseMove(MouseEventArgs e) {
			if(!IsMouseDrag)
				return;
			if(Properties.UseDefaultLayoutMode) {
				UpdateScrollPositions();
				Properties.UseDefaultLayoutMode = false;
			}
			HorizontalScrollPosition += -e.Location.X + MouseLocationPrev.X;
			VerticalScrollPosition += -e.Location.Y + MouseLocationPrev.Y;
			MouseLocationPrev = e.Location;
			ViewInfo.IsReady = false;
			Owner.Invalidate();
			Owner.Update();
			if(VScrollBar != null)
				VScrollBar.OnAction(ScrollNotifyAction.MouseMove);
			if(HScrollBar != null)
				HScrollBar.OnAction(ScrollNotifyAction.MouseMove);
		}
		protected virtual void UpdateScrollPositions() {
			if(Image == null)
				return;
			if(Properties.SizeMode == PictureSizeMode.Clip) {
				HorizontalScrollPosition = (float)(ViewInfo.PictureSourceBounds.X * Properties.ZoomFactorCore);
				VerticalScrollPosition = (float)(ViewInfo.PictureSourceBounds.Y * Properties.ZoomFactorCore);
			}
			else if(Properties.SizeMode == PictureSizeMode.Zoom ||
				Properties.SizeMode == PictureSizeMode.Squeeze) {
				float koeff = ViewInfo.PictureScreenBounds.Width / Image.Width;
				Properties.ZoomFactor = koeff * 100;
				HorizontalScrollPosition = VerticalScrollPosition = 0.0f;
			}
		}
		internal void Owner_MouseDown(MouseEventArgs e) {
			if(!AllowMouseDrag(e))
				return;
			MouseLocationPrev = e.Location;
			IsMouseDrag = true;
		}
		internal void Owner_MouseWheel(MouseEventArgs e) {
			if(wheelScrollHelper == null) wheelScrollHelper = new MouseWheelScrollHelper(this);
			wheelScrollHelper.OnMouseWheel(e);
		}
		internal void Owner_KeyDown(KeyEventArgs e) {
			if(Owner.Properties.AllowSubMenu && Owner.Menu != null) {
				if(e.KeyCode == Keys.Divide) Owner.Menu.FullSize();
				if(e.KeyCode == Keys.Multiply) Owner.Menu.FitImage();
				if(e.KeyCode == Keys.Subtract) Owner.Menu.ZoomOut();
				if(e.KeyCode == Keys.Add) Owner.Menu.ZoomIn();
			}
		}
		internal void Owner_KeyUp(KeyEventArgs e) {
		}
		MouseWheelScrollHelper wheelScrollHelper;
		protected bool AllowZooming {
			get {
				if(Properties.AllowZoomOnMouseWheel == DefaultBoolean.Default && !Properties.ShowScrollBars) return false;
				if(Properties.AllowZoomOnMouseWheel == DefaultBoolean.False) return false;
				return Control.ModifierKeys == Keys.Control || Owner.Properties.ZoomingOperationMode == ZoomingOperationMode.MouseWheel;
			}
		}
		protected bool AllowScrolling {
			get {
				if(Properties.AllowScrollOnMouseWheel == DefaultBoolean.Default)
					return Properties.ShowScrollBars;
				return Properties.AllowScrollOnMouseWheel == DefaultBoolean.True;
			}
		}
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			if(AllowZooming)
				OnZoom(e);
			else if(AllowScrolling)
				OnScroll(e);
		}
		protected virtual void OnScroll(MouseWheelScrollClientArgs e) {
			if(e.Horizontal)
				HorizontalScrollPosition += e.Distance;
			else
				VerticalScrollPosition += e.Distance;
		}
		protected bool LockZoomUpdate { get { return Properties.UpdateZoomFromMouseWheel; } set { Properties.UpdateZoomFromMouseWheel = value; } }
		protected internal virtual void OnZoom(MouseWheelScrollClientArgs e) {
			OnZoom(e.Location, -e.Distance, true);
		}
		protected double MinimumZoomFactor {
			get {
				float zx = 1.0f / Owner.ViewInfo.ImageWidthF;
				float zy = 1.0f / Owner.ViewInfo.ImageHeightF;
				return Math.Max(zx, zy);
			}
		}
		protected internal virtual void OnZoom(Point location, double deltaZoom, bool animate) {
			if(animate) {
				if(XtraAnimator.Current.Get(ViewInfo, ViewInfo.PictureBoundsAnimationId) != null)
					return;
				Owner.ViewInfo.SavePictureBounds();
				if(!Owner.ViewInfo.IsPictureBoundsLocked)
					Owner.ViewInfo.LockPictureBounds();
			}
			float oldX = HorizontalScrollPosition + (location.X - Owner.ViewInfo.PictureScreenBounds.X);
			float oldY = VerticalScrollPosition + (location.Y - Owner.ViewInfo.PictureScreenBounds.Y);
			oldX /= Owner.ViewInfo.ImageWidthF;
			oldY /= Owner.ViewInfo.ImageHeightF;
			int distance = (int)deltaZoom;
			LockZoomUpdate = true;
			try {
				Owner.Properties.ZoomFactor += distance;
				Owner.Properties.ZoomFactor = Math.Max(Owner.Properties.ZoomFactor, MinimumZoomFactor);
			}
			finally {
				LockZoomUpdate = false;
			}
			float newX = oldX * Owner.ViewInfo.ImageWidthF - (location.X - Owner.ViewInfo.ContentRect.X);
			float newY = oldY * Owner.ViewInfo.ImageHeightF - (location.Y - Owner.ViewInfo.ContentRect.Y);
			LockUpdateScrollBars = true;
			try {
				HorizontalScrollPosition = newX;
				VerticalScrollPosition = newY;
			}
			finally {
				LockUpdateScrollBars = false;
			}
			Owner.ViewInfo.CalcPictureBounds(true);
			if(animate) {
				Owner.ViewInfo.AnimatePictureBounds();
			}
			UpdateScrollBars();
		}
		internal void UpdateScrollStyles() {
			if(vScrollBar != null) vScrollBar.LookAndFeel.Assign(Owner.LookAndFeel);
			if(hScrollBar != null) hScrollBar.LookAndFeel.Assign(Owner.LookAndFeel);
		}
		public void Dispose() {
			HandCursor.Dispose();
			HandDragCursor.Dispose();
			if(vScrollBar != null) {
				vScrollBar.Dispose();
				vScrollBar = null;
			}
			if(hScrollBar != null) {
				hScrollBar.Dispose();
				hScrollBar = null;
			}
		}
		internal void UpdateZoomScrollBars(double prevZoom, double nextZoom) {
			if(LockZoomUpdate)
				return;
			if(Image == null)
				return;
			PointF oldCenter = new PointF(HorizontalScrollPosition + ViewInfo.PictureScreenBounds.Width / 2, VerticalScrollPosition + ViewInfo.PictureScreenBounds.Height / 2);
			oldCenter.X /= (float)(Image.Width * prevZoom / 100);
			oldCenter.Y /= (float)(Image.Height * prevZoom / 100);
			HorizontalScrollPosition = oldCenter.X * ImageWidth - ViewInfo.PictureScreenBounds.Width / 2;
			VerticalScrollPosition = oldCenter.Y * ImageHeight - ViewInfo.PictureScreenBounds.Height / 2;
			UpdateScrollBars();
		}
		bool checkForceDisplayScrollBars = false;
		public void TryDisplayScrollBars(bool force) {
			if((allowUpdate || force) && AllowScrollBar) {
				DisplayScrollBars();
				allowUpdate = false;
				if(checkForceDisplayScrollBars) {
					ForceDisplayScrollBars();
					checkForceDisplayScrollBars = false;
				}
			}
		}
		public void ForceDisplayScrollBars() {
			DisplayScrollBars();
			this.checkForceDisplayScrollBars = true;
			Owner.LayoutChanged();
		}
		bool HorzLeft { get { return Properties.PictureAlignment == ContentAlignment.BottomLeft || Properties.PictureAlignment == ContentAlignment.MiddleLeft || Properties.PictureAlignment == ContentAlignment.TopLeft; } }
		bool HorzCenter { get { return Properties.PictureAlignment == ContentAlignment.BottomCenter || Properties.PictureAlignment == ContentAlignment.MiddleCenter || Properties.PictureAlignment == ContentAlignment.TopCenter; } }
		bool HorzRight { get { return Properties.PictureAlignment == ContentAlignment.BottomRight || Properties.PictureAlignment == ContentAlignment.MiddleRight || Properties.PictureAlignment == ContentAlignment.TopRight; } }
		bool VertTop { get { return Properties.PictureAlignment == ContentAlignment.TopCenter || Properties.PictureAlignment == ContentAlignment.TopLeft || Properties.PictureAlignment == ContentAlignment.TopRight; } }
		bool VertMiddle { get { return Properties.PictureAlignment == ContentAlignment.MiddleCenter || Properties.PictureAlignment == ContentAlignment.MiddleLeft || Properties.PictureAlignment == ContentAlignment.MiddleRight; } }
		bool VertBottom { get { return Properties.PictureAlignment == ContentAlignment.BottomCenter || Properties.PictureAlignment == ContentAlignment.BottomLeft || Properties.PictureAlignment == ContentAlignment.BottomRight; } }
		void CreateScrollBars() {
			vScrollBar = new DevExpress.XtraEditors.VScrollBar();
			ScrollBarBase.ApplyUIMode(vScrollBar);
			hScrollBar = new DevExpress.XtraEditors.HScrollBar();
			ScrollBarBase.ApplyUIMode(hScrollBar);
			vScrollBar.ValueChanged += new EventHandler(OnVerticalScrollBarValueChanged);
			hScrollBar.ValueChanged += new EventHandler(OnHorizontalScrollBarValueChanged);
			vScrollBar.Cursor = DefaultCursor;
			hScrollBar.Cursor = DefaultCursor;
		}
		bool LockUpdateHorizontalScrollPosition { get; set; }
		bool LockUpdateVerticalScrollPosition { get; set; }
		void OnScrollBarValueChangedCore(object sender, EventArgs e) {
			Owner.Properties.UseDefaultLayoutMode = false;
			Owner.ViewInfo.IsReady = false;
			Owner.Invalidate();
		}
		void OnHorizontalScrollBarValueChanged(object sender, EventArgs e) {
			if(!LockUpdateHorizontalScrollPosition)
				HorizontalScrollPosition = this.hScrollBar.Value;
			OnScrollBarValueChangedCore(sender, e);
		}
		void OnVerticalScrollBarValueChanged(object sender, EventArgs e) {
			if(!LockUpdateVerticalScrollPosition)
				VerticalScrollPosition = this.vScrollBar.Value;
			OnScrollBarValueChangedCore(sender, e);
		}
		void UpdateContextButtons() {
			ViewInfo.ContextButtonsViewInfo.Refresh();
		}
		void UpdateScrollBars() {
			if(!Properties.ShowScrollBars)
				return;
			UpdateContextButtons();
			HScrollVisible = ShouldShowHorizontalScroll;
			VScrollVisible = ShouldShowVerticalScroll;
			VScrollBar.Minimum = 0;
			HScrollBar.Minimum = 0;
			if(ShouldShowHorizontalScroll) {
				HScrollBar.BeginUpdate();
				try {
					LockUpdateHorizontalScrollPosition = true;
					HScrollBar.Maximum = Owner.ViewInfo.ImageWidth;
					HScrollBar.LargeChange = ViewInfo.ContentRect.Width;
					HScrollBar.SmallChange = 1;
					HScrollBar.Value = (int)HorizontalScrollPosition;
				}
				finally {
					LockUpdateHorizontalScrollPosition = false;
					HScrollBar.EndUpdate();
				}
			}
			if(ShouldShowVerticalScroll) {
				VScrollBar.BeginUpdate();
				try {
					LockUpdateVerticalScrollPosition = true;
					VScrollBar.Maximum = Owner.ViewInfo.ImageHeight;
					VScrollBar.LargeChange = ViewInfo.ContentRect.Height;
					VScrollBar.SmallChange = 1;
					VScrollBar.Value = (int)VerticalScrollPosition;
				}
				finally {
					LockUpdateVerticalScrollPosition = false;
					VScrollBar.EndUpdate();
				}
			}
		}
		internal bool VScrollVisible {
			get { return vScrollBar != null && vScrollBar.ActualVisible; }
			set {
				if(vScrollBar != null) {
					vScrollBar.Parent = Owner;
					if(VScrollVisible != value) ViewInfo.IsContextButtonsReady = false;
					vScrollBar.SetVisibility(value);
					allowUpdate = true;
				}
			}
		}
		internal bool HScrollVisible {
			get { return hScrollBar != null && hScrollBar.ActualVisible; }
			set {
				if(hScrollBar != null) {
					HScrollBar.Parent = Owner;
					if(HScrollVisible != value) ViewInfo.IsContextButtonsReady = false;
					hScrollBar.SetVisibility(value);
					allowUpdate = true;
				}
			}
		}
		internal HScrollBar HScrollBar { get { return hScrollBar; } }
		internal VScrollBar VScrollBar { get { return vScrollBar; } }
		int VScrollHeight { get { return this.ViewInfo.ClientRect.Height - (HScrollVisible && !VScrollBar.IsOverlapScrollBar ? HScrollBar.GetDefaultHorizontalScrollBarHeight() : 0); } }
		int HScrollWidth { get { return this.ViewInfo.ClientRect.Width - (VScrollVisible ? VScrollBar.GetDefaultVerticalScrollBarWidth() : 0); } }
		int VScrollWidth { get { return (vScrollBar != null && VScrollVisible ? vScrollBar.Width : 0); } }
		int HScrollHeight { get { return (hScrollBar != null && HScrollVisible ? hScrollBar.Height : 0); } }
		float ImageWidth { get { return (float)(Image.Width * Properties.ZoomFactorCore); } }
		float ImageHeight { get { return (float)(Image.Height * Properties.ZoomFactorCore); } }
		bool AllowScrollBar {
			get {
				if(Image == null)
					return false;
				return ViewInfo.ActualSizeMode == PictureSizeMode.Clip && Properties.ShowScrollBars && !ViewInfo.ContentRect.IsEmpty && ViewInfo.UseMetafiles;
			}
		}
		protected internal bool ShouldShowHorizontalScroll { get { return Image != null && ImageWidth > ViewInfo.ContentRect.Width; } }
		protected internal bool ShouldShowVerticalScroll { get { return Image != null && ImageHeight > ViewInfo.ContentRect.Height; } }
		void DisplayScrollBars() {
			if(AllowScrollBar) {
				if(vScrollBar == null) CreateScrollBars();
				HScrollVisible = ShouldShowHorizontalScroll;
				VScrollVisible = ShouldShowVerticalScroll;
				if(VScrollVisible && !HScrollVisible) {
					HScrollVisible = ShouldShowHorizontalScroll;
				}
				if(VScrollVisible) {
					vScrollBar.Bounds = new Rectangle(this.ViewInfo.ClientRect.X + (ViewInfo.RightToLeft ? 0 : this.ViewInfo.ClientRect.Width - VScrollBar.GetDefaultVerticalScrollBarWidth()),
					ViewInfo.ClientRect.Y, VScrollBar.GetDefaultVerticalScrollBarWidth(), VScrollHeight);
				}
				if(HScrollVisible) {
					this.hScrollBar.Bounds = new Rectangle(this.ViewInfo.ClientRect.X + (ViewInfo.RightToLeft ? (VScrollVisible ? VScrollBar.GetDefaultVerticalScrollBarWidth() : 0) : 0),
						ViewInfo.ClientRect.Y + this.ViewInfo.ClientRect.Height - HScrollBar.GetDefaultHorizontalScrollBarHeight(),
						HScrollWidth, HScrollBar.GetDefaultHorizontalScrollBarHeight());
				}
				UpdateScrollBars();
			}
			else {
				if(vScrollBar != null) {
					VScrollVisible = HScrollVisible = false;
				}
			}
			if(allowUpdate)
				Owner.LayoutChanged();
			UpdateCursors();
		}
		protected bool LockUpdateScrollBars { get; set; }
		float horizontalScrollPosition = 0.0f;
		protected internal float HorizontalScrollPosition {
			get {
				return horizontalScrollPosition;
			}
			set {
				horizontalScrollPosition = ConstrainValue(value, HorizontalMaxScrollPosition);
				if(hScrollBar != null && !LockUpdateScrollBars) {
					LockUpdateHorizontalScrollPosition = true;
					hScrollBar.Value = (int)HorizontalScrollPosition;
					LockUpdateHorizontalScrollPosition = false;
				}
			}
		}
		private float ConstrainValue(float value, float maxValue) {
			return Math.Min(Math.Max(0.0f, value), maxValue);
		}
		float verticalScrollPosition = 0.0f;
		protected internal float VerticalScrollPosition {
			get { return verticalScrollPosition; }
			set {
				verticalScrollPosition = ConstrainValue(value, VerticalMaxScrollPosition);
				if(vScrollBar != null && !LockUpdateScrollBars) {
					LockUpdateVerticalScrollPosition = true;
					vScrollBar.Value = (int)VerticalScrollPosition;
					LockUpdateVerticalScrollPosition = false;
				}
			}
		}
		public Rectangle GetFocusRectRectangle(Rectangle rect) {
			if(VScrollBar.IsOverlapScrollBar) return rect;
			rect.Width -= VScrollWidth;
			rect.Height -= HScrollHeight;
			if(ViewInfo.RightToLeft) rect.X += VScrollWidth;
			return rect;
		}
		internal Rectangle ScrollSquareRectangle {
			get {
				if(VScrollBar.IsOverlapScrollBar) return Rectangle.Empty;
				if(VScrollVisible && HScrollVisible)
					return new Rectangle(
						this.ViewInfo.ClientRect.X + this.ViewInfo.ClientRect.Width - SystemInformation.VerticalScrollBarWidth,
						this.ViewInfo.ClientRect.Y + this.ViewInfo.ClientRect.Height - SystemInformation.HorizontalScrollBarHeight,
						SystemInformation.VerticalScrollBarWidth, SystemInformation.HorizontalScrollBarHeight);
				return Rectangle.Empty;
			}
		}
		#region IMouseWheelScrollClient Members
		bool IMouseWheelScrollClient.PixelModeHorz { get { return true; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return true; } }
		#endregion
		public float HorizontalMaxScrollPosition {
			get {
				return Math.Max(0, Owner.ViewInfo.ImageWidth - Owner.ViewInfo.ContentRect.Width);
			}
		}
		public float VerticalMaxScrollPosition {
			get {
				return Math.Max(0, Owner.ViewInfo.ImageHeight - Owner.ViewInfo.ContentRect.Height);
			}
		}
		protected internal virtual void OnResize() {
			UpdateScrollBars();
			HorizontalScrollPosition = Math.Min(HorizontalScrollPosition, HorizontalMaxScrollPosition);
			VerticalScrollPosition = Math.Min(VerticalScrollPosition, VerticalMaxScrollPosition);
			Owner.ViewInfo.IsReady = false;
			Owner.Invalidate();
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class PictureEditViewInfo : BaseEditViewInfo, IHeightAdaptable, IAnimatedItem, ISupportXtraAnimationEx, ISupportContextItems {
		public static int DefaultCustomHeight = 18;
		Image image;
		int _customHeight;
		ContextItemCollectionViewInfo contextButtonsViewInfo;
		public PictureEditViewInfo(RepositoryItem item)
			: base(item) {
				IsContextButtonsReady = false;
		}
		internal bool UseMetafiles {
			get {
				return true;
			}
		}
		protected internal virtual bool IsContextButtonsReady { get; set; }
		protected internal string EmptyImageText { get { return OwnerEdit != null ? ((PictureEdit)OwnerEdit).EmptyImageText : null; } }
		protected internal virtual bool ShouldDrawImageDisabled {
			get {
				bool res = State == ObjectState.Disabled;
				if(OwnerEdit != null)
					res &= ((PictureEdit)OwnerEdit).AllowDrawImageDisabled;
				return res;
			}
		}
		protected internal virtual bool UseDefaultLayoutMode {
			get {
				if(Image is Metafile && !Item.UseMetafiles)
					return true;
				return Item.UseDefaultLayoutMode;
			}
		}
		double ZoomFactor {
			get {
				if(Item.SizeMode != PictureSizeMode.Clip && UseDefaultLayoutMode)
					return 1;
				return UseMetafiles ? Item.ZoomFactorCore : 1;
			}
		}
		protected internal int ImageWidth { get { return Image != null ? (int)(Image.Width * ZoomFactor) : -1; } }
		protected internal int ImageHeight { get { return Image != null ? (int)(Image.Height * ZoomFactor) : -1; } }
		protected internal float ImageWidthF { get { return Image != null ? (float)(Image.Width * ZoomFactor) : -1.0f; } }
		protected internal float ImageHeightF { get { return Image != null ? (float)(Image.Height * ZoomFactor) : -1.0f; } }
		protected internal Size ImageSize {
			get {
				try {
					return new Size((int)(Image.Width * ZoomFactor), (int)(Image.Height * ZoomFactor));
				}
				catch {
					Image = null;
					return Size.Empty;
				}
			}
		}
		public PictureSizeMode ActualSizeMode {
			get {
				if(Image is Metafile && !UseMetafiles)
					return PictureSizeMode.Stretch;
				if(IsSystemImage || !UseDefaultLayoutMode)
					return PictureSizeMode.Clip;
				return Item.SizeMode;
			}
		}
		protected override bool UseDisabledAppearance {
			get {
				return base.UseDisabledAppearance && ShouldDrawImageDisabled;
			}
		}
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			if(PaintAppearance.BackColor == Color.Transparent && OwnerControl != null && OwnerControl.Parent != null) {
				PaintAppearance.ForeColor = OwnerControl.Parent.ForeColor;
			}
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			PictureEditViewInfo be = info as PictureEditViewInfo;
			if(be == null) return;
			this.image = be.image;
			PictureScreenBounds = be.PictureScreenBounds;
			PictureSourceBounds = be.PictureSourceBounds;
			this._customHeight = be._customHeight;
		}
		RectangleF pictureScreenBounds;
		public RectangleF PictureScreenBounds {
			get { return pictureScreenBounds; }
			set { pictureScreenBounds = value; } 
		}
		public RectangleF PictureSourceBounds { get; set; }
		public RectangleF PictureScreenBoundsStart { get; set; }
		public RectangleF PictureScreenBoundsEnd { get; set; }
		public RectangleF PictureSourceBoundsStart { get; set; }
		public RectangleF PictureSourceBoundsEnd { get; set; }
		public Image GetActualImage() {
			if(!EnableLODImages || Images.Length == 0)
				return Image;
			return Images[GetActualImageLevel()];
		}
		public RectangleF GetActualPictureSourceBounds() {
			if(!EnableLODImages || Images.Length == 0)
				return PictureSourceBounds;
			int level = GetActualImageLevel();
			int koeff = 1 << level;
			RectangleF res = new RectangleF(PictureSourceBounds.X / koeff, PictureSourceBounds.Y / koeff, PictureSourceBounds.Width / koeff, PictureSourceBounds.Height / koeff);
			return res;
		}
		public int GetActualImageLevel() {
			if(Item.ZoomFactorCore > 1.0 || Images.Length == 0)
				return 0;
			double zoom = 1.0;
			int level;
			for(level = 0; level < Images.Length; level++) {
				zoom /= 2.0;
				if(zoom < ZoomFactor)
					break;
			}
			return Math.Min(level, Images.Length - 1);
		}
		protected internal void SavePictureBounds() {
			PictureScreenBoundsStart = PictureScreenBounds;
			PictureSourceBoundsStart = PictureSourceBounds;
		}
		protected int PictureBoundsLockCount { get; set; }
		protected internal void LockPictureBounds() {
			PictureBoundsLockCount++;
		}
		protected internal bool IsPictureBoundsLocked { get { return PictureBoundsLockCount > 0; } }
		protected internal void UnlockPictureBounds() {
			if(PictureBoundsLockCount > 0) {
				PictureBoundsLockCount--;
				if(PictureBoundsLockCount == 0) {
					PictureScreenBounds = PictureScreenBoundsEnd;
					PictureSourceBounds = PictureSourceBoundsEnd;
				}
			}
		}
		internal readonly object PictureBoundsAnimationId = new object();
		private static int PictureBoundsAnimationLength = 300;
		protected internal virtual void AnimatePictureBounds() {
			DoubleSplineAnimationInfo info = XtraAnimator.Current.Get(this, PictureBoundsAnimationId) as DoubleSplineAnimationInfo;
			if(info != null) XtraAnimator.Current.Animations.Remove(info);
			info = new DoubleSplineAnimationInfo(this, PictureBoundsAnimationId, 0.0, 1.0, PictureBoundsAnimationLength);
			XtraAnimator.Current.AddAnimation(info);
		}
		ImageLoadInfo imageInfo;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageLoadInfo ImageInfo {
			get { return imageInfo; }
			set { imageInfo = value; }
		}
		protected PictureEdit PictureEdit { get { return (PictureEdit)OwnerEdit; } }
		public PointF PicturePosition {
			get {
				if(OwnerEdit == null) return PointF.Empty;
				return new PointF(PictureEdit.Scrollers.HorizontalScrollPosition, PictureEdit.Scrollers.VerticalScrollPosition);
			}
		}
		protected internal virtual void CalcPictureBounds() {
			CalcPictureBounds(false);
		}
		protected internal virtual void CalcPictureBounds(bool forceCalcPictureBounds) {
			if(Image == null || (IsPictureBoundsLocked && !forceCalcPictureBounds))
				return;
			if(ActualSizeMode == PictureSizeMode.Stretch) {
				CalcPictureBoundsStretch();
			}
			else if(ActualSizeMode == PictureSizeMode.StretchHorizontal) {
				CalcPictureBoundsStretchHorizontal();
			}
			else if(ActualSizeMode == PictureSizeMode.StretchVertical) {
				CalcPictureBoundsStretchVertical();
			}
			else if(ActualSizeMode == PictureSizeMode.Zoom && UseDefaultLayoutMode) {
				CalcPictureBoundsZoom();
			}
			else if(ActualSizeMode == PictureSizeMode.Squeeze && UseDefaultLayoutMode) {
				CalcPictureBoundsSqueeze();
			}
			else {
				CalcPictureBoundsClip();
			}
			UpdatePictureBoundsByContentAlignment();
			UpdateScrollPositionsInClipSizeMode();
		}
		protected virtual void UpdateScrollPositionsInClipSizeMode() {
			if(!UseDefaultLayoutMode || ActualSizeMode != PictureSizeMode.Clip || PictureEdit == null)
				return;
			PictureEdit.Scrollers.HorizontalScrollPosition = (float)(PictureSourceBounds.X * Item.ZoomFactorCore);
			PictureEdit.Scrollers.VerticalScrollPosition = (float)(PictureSourceBounds.Y * Item.ZoomFactorCore);
			PictureEdit.UpdateScrollBars();
			Item.UseDefaultLayoutMode = false;
			CalcPictureBoundsClip();
		}
		protected RectangleF ContentRectF { get { return new RectangleF(ContentRect.X, ContentRect.Y, ContentRect.Width, ContentRect.Height); } }
		protected virtual void UpdatePictureBoundsByContentAlignment() {
			if(ActualSizeMode == PictureSizeMode.Stretch || !UseDefaultLayoutMode)
				return;
			RectangleF r = ImageLayoutHelper.GetImageBounds(ContentRectF, PictureScreenBounds.Size, ContentAlignment2ImageLayoutMode(ActualContentAlignment));
			PictureScreenBounds = new RectangleF(r.X, r.Y, r.Width, r.Height);
			if(ActualSizeMode != PictureSizeMode.Clip)
				return;
			float x = Math.Max(0.0f, ContentRect.X - r.X) / ImageWidth;
			float y = Math.Max(0.0f, ContentRect.Y - r.Y) / ImageHeight;
			float width = Math.Min((float)ContentRect.Width / ImageWidth, 1.0f);
			float height = Math.Min((float)ContentRect.Height / ImageHeight, 1.0f);
			PictureSourceBounds = new RectangleF(x * Image.Width, y * Image.Height, width * Image.Width, height * Image.Height);
			PictureScreenBounds = ImageLayoutHelper.GetImageBounds(ContentRectF, new SizeF(width * ImageWidth, height * ImageHeight), ContentAlignment2ImageLayoutMode(ActualContentAlignment));
		}
		private ImageLayoutMode ContentAlignment2ImageLayoutMode(ContentAlignment alignement) {
			switch(alignement) {
				case ContentAlignment.TopLeft:
					return ImageLayoutMode.TopLeft;
				case ContentAlignment.TopCenter:
					return ImageLayoutMode.TopCenter;
				case ContentAlignment.TopRight:
					return ImageLayoutMode.TopRight;
				case ContentAlignment.MiddleLeft:
					return ImageLayoutMode.MiddleLeft;
				case ContentAlignment.MiddleCenter:
					return ImageLayoutMode.MiddleCenter;
				case ContentAlignment.MiddleRight:
					return ImageLayoutMode.MiddleRight;
				case ContentAlignment.BottomLeft:
					return ImageLayoutMode.BottomLeft;
				case ContentAlignment.BottomCenter:
					return ImageLayoutMode.BottomCenter;
			}
			return ImageLayoutMode.BottomRight;
		}
		protected virtual void CalcPictureBoundsByZoomAndOffset(float zoomFactor, PointF offset) {
			float sourceWidth = ContentRect.Width / zoomFactor;
			float sourceHeight = ContentRect.Height / zoomFactor;
			sourceWidth = Math.Min(sourceWidth, Image.Width);
			sourceHeight = Math.Min(sourceHeight, Image.Height);
			float x = Math.Max(0.0f, offset.X / zoomFactor);
			float y = Math.Max(0.0f, offset.Y / zoomFactor);
			RectangleF sourceRect = new RectangleF(x, y, sourceWidth, sourceHeight);
			RectangleF screenRect = new RectangleF(ContentRect.X, ContentRect.Y, sourceWidth * zoomFactor, sourceHeight * zoomFactor);
			if(ContentRect.Width - screenRect.Width >= 1.0f) {
				RectangleF rect = ImageLayoutHelper.GetImageBounds(ContentRectF, screenRect.Size, ContentAlignment2ImageLayoutMode(ActualContentAlignment));
				screenRect.X = rect.X;
				sourceRect.X = 0.0f;
			}
			if(ContentRect.Height - screenRect.Height >= 1.0f) {
				RectangleF rect = ImageLayoutHelper.GetImageBounds(ContentRectF, screenRect.Size, ContentAlignment2ImageLayoutMode(ActualContentAlignment));
				screenRect.Y = rect.Y;
				sourceRect.Y = 0.0f;
			}
			if(IsPictureBoundsLocked) {
				PictureScreenBoundsEnd = screenRect;
				PictureSourceBoundsEnd = sourceRect;
			}
			else {
				PictureSourceBounds = sourceRect;
				PictureScreenBounds = screenRect;
			}
		}
		protected virtual void CalcPictureBoundsClip() {
			if(UseDefaultLayoutMode) {
				PictureScreenBounds = ImageLayoutHelper.GetImageBounds(ContentRectF, new SizeF(ImageWidthF, ImageHeightF), ContentAlignment2ImageLayoutMode(ActualContentAlignment));
				PictureSourceBounds = new Rectangle(0, 0, Image.Width, Image.Height);
			}
			else {
				CalcPictureBoundsByZoomAndOffset((float)ZoomFactor, PicturePosition);
			}
		}
		protected virtual void CalcPictureBoundsSqueeze() {
			PictureScreenBounds = ImageLayoutHelper.GetImageBounds(ContentRectF, new SizeF(Image.Width, Image.Height), ImageLayoutMode.Squeeze);
			PictureSourceBounds = new Rectangle(0, 0, Image.Width, Image.Height);
		}
		protected virtual void CalcPictureBoundsZoom() {
			PictureScreenBounds = ImageLayoutHelper.GetImageBounds(ContentRectF, new SizeF(Image.Width, Image.Height), ImageLayoutMode.ZoomInside);
			PictureSourceBounds = new Rectangle(0, 0, Image.Width, Image.Height);
		}
		protected virtual void CalcPictureBoundsStretchVertical() {
			PictureScreenBounds = new Rectangle(ContentRect.X + (ContentRect.Width - Image.Width) / 2, ContentRect.Y, Image.Width, ContentRect.Height);
			PictureSourceBounds = new Rectangle(0, 0, Image.Width, Image.Height);
		}
		protected virtual void CalcPictureBoundsStretchHorizontal() {
			PictureScreenBounds = new Rectangle(ContentRect.X, ContentRect.Y + (ContentRect.Height - Image.Height) / 2, ContentRect.Width, Image.Height);
			PictureSourceBounds = new Rectangle(0, 0, Image.Width, Image.Height);
		}
		protected virtual void CalcPictureBoundsStretch() {
			PictureScreenBounds = ContentRect;
			PictureSourceBounds = new Rectangle(0, 0, Image.Width, Image.Height);
		}
		public override TextOptions DefaultTextOptions {
			get {
				return new TextOptions(HorzAlignment.Center, VertAlignment.Center, WordWrap.Default, Trimming.EllipsisCharacter);
			}
		}
		public override bool RequireClipping { get { return true; } }
		public override bool IsSupportFastViewInfo { get { return false; } }
		protected internal override bool AllowDrawParentBackground { get { return true; } }
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(Image);
				return imageHelper;
			}
		}
		Rectangle IAnimatedItem.AnimationBounds {
			get {
				return new Rectangle((int)PictureScreenBounds.X, (int)PictureScreenBounds.Y, (int)PictureScreenBounds.Width, (int)PictureScreenBounds.Height);
			}
		}
		int IAnimatedItem.AnimationInterval { get { return ImageHelper.AnimationInterval; } }
		int[] IAnimatedItem.AnimationIntervals { get { return ImageHelper.AnimationIntervals; } }
		AnimationType IAnimatedItem.AnimationType { get { return ImageHelper.AnimationType; } }
		int IAnimatedItem.FramesCount { get { return ImageHelper.FramesCount; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return ImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner { get { return OwnerEdit; } }
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		Control ISupportXtraAnimation.OwnerControl { get { return OwnerEdit; } }
		bool ISupportXtraAnimation.CanAnimate {
			get {
				if(IsPictureBoundsLocked)
					return true;
				return ((IAnimatedItem)this).FramesCount > 1;
			}
		}
		public override void Reset() {
			this.image = null;
			base.Reset();
		}
		public override void Clear() {
			base.Clear();
			this._customHeight = 0;
		}
		protected override void UpdateFromEditor() {
			base.UpdateFromEditor();
			if(OwnerEdit == null) {
				this._customHeight = DefaultCustomHeight;
			}
		}
		public int CustomHeight { get { return _customHeight; } }
		public new RepositoryItemPictureEdit Item { get { return base.Item as RepositoryItemPictureEdit; } }
		public Image Image {
			get {
				if(image != null && !DevExpress.Utils.Paint.XPaint.IsImageValid(image)) {
					Image = null;
				}
				return image;
			}
			set {
				image = value;
				ImageHelper.Image = image;
			}
		}
		protected override void OnEditValueChanged() {
			if(IsEqualsEditValueCore)
				return;
			if(Images != null && Images[0] != Image) {
				ClearLODImages();
			}
			Item.UseDefaultLayoutMode = true;
			StopAnimation();
			object val = EditValue;
			ConvertEditValueEventArgs args = Item.DoFormatEditValue(val);
			if(args.Handled) val = args.Value;
			if(val == null || val is Image || val is Icon) {
				this.image = val as Image;
				if(val is Icon) this.image = ((Icon)val).ToBitmap();
			}
			else {
				this.image = ByteImageConverter.FromByteArray(ByteImageConverter.ToByteArray(val));
			}
			if(image != null && !DevExpress.Utils.Paint.XPaint.IsImageValid(image)) image = null;
			ImageHelper.Image = image;
			StartAnimation();
		}
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(this);
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = this;
			if(OwnerEdit == null || OwnerEdit.IsDesignMode || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(null, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(Image == null || OwnerEdit == null || info == null) return;
			if(!info.IsFinalFrame) {
				Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				OwnerEdit.Invalidate(animItem.AnimationBounds);
			}
			else {
				StopAnimation();
				StartAnimation();
			}
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			PictureScreenBounds = new RectangleF(PictureScreenBounds.X + x, PictureScreenBounds.Y + y, PictureScreenBounds.Width, PictureScreenBounds.Height);
			if(CaptionInfo != null) CaptionInfo.OffsetContent(x, y);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			if(!FocusRect.IsEmpty && FocusRectThin > 1) {
				fContentRect.Inflate(1, 1);
			}
			fContentRect.X += Item.Padding.Left;
			fContentRect.Width -= Item.Padding.Horizontal;
			fContentRect.Y += Item.Padding.Top;
			fContentRect.Height -= Item.Padding.Vertical;
			CheckLODImages();
			CalcPictureBounds();
			CalcContextButtons();
			CalcCaptionInfo();
		}
		CaptionViewInfo captionInfo;
		CaptionPainter captionPainter;
		protected internal CaptionViewInfo CaptionInfo { get { return captionInfo; } }
		public CaptionPainter CaptionPainter {
			get {
				if(captionPainter == null) captionPainter = new CaptionPainter();
				return captionPainter;
			}
		}
		public override void CalcViewInfo(Graphics g) {
			if(CaptionInfo != null) CaptionInfo.IsReady = false;
			base.CalcViewInfo(g);
		}
		public override void UpdateBoundValues(DataController controller, int row) {
			string source = Item.Caption.GetDisplayText();
			if(string.IsNullOrEmpty(source) || !DXBindingParser.Default.ContainsBindings(source)) {
				CaptionText = null;
				return;
			}
			CaptionText = DXBindingParser.Default.ParseBindings(controller, row, source);
		}
		string captionText;
		public string CaptionText {
			get { return captionText; }
			set {
				if(CaptionText == value) return;
				captionText = value;
				if(CaptionInfo != null) CaptionInfo.Text = captionText;
			}
		}
		protected internal virtual void EnsureCaptionInfo() {
			if(CaptionInfo != null && CaptionInfo.IsReady) return;
			CalcCaptionInfo();
		}
		protected virtual void CalcCaptionInfo() {
			if(captionInfo == null) {
				captionInfo = Item.Caption.CreateViewInfo();
			}
			Graphics g = GInfo.AddGraphics(null);
			try {
				Item.Caption.UpdateViewInfo(CaptionInfo, CaptionText);
				CaptionInfo.SetTargetBounds(this.ContentRect);
				ObjectPainter.CalcObjectBounds(g, CaptionPainter, CaptionInfo);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected Rectangle LastContextButtonsDisplayRectangle { get; set; }
		private void CalcContextButtons() {
			if(IsContextButtonsReady) return;
			if(LastContextButtonsDisplayRectangle != ((ISupportContextItems)this).DisplayBounds) {
				ContextButtonsViewInfo.Refresh();
			}
			ContextButtonsViewInfo.CalcItems();
			IsContextButtonsReady = true;
		}
		protected internal virtual bool EnableLODImages {
			get {
				if(((IAnimatedItem)this).IsAnimated)
					return false;
				if(Image == null || (Image.Width < Item.LODImageMinSize && Image.Height < Item.LODImageMinSize))
					return false;
				return Item.EnableLODImages;
			}
		}
		protected virtual void CheckLODImages() {
			if(!EnableLODImages) {
				ClearLODImages();
				return;
			}
			if(Image == null)
				return;
			if(Images != null) {
				int lastSize = Math.Max(Images[Images.Length - 1].Width, Images[Images.Length - 1].Height);
				if(lastSize / 2 > Item.LODImageMinSize) {
					ClearLODImages();
				}
			}
			if(Images == null)
				Images = GenerateLODImages();
		}
		private System.Drawing.Image[] GenerateLODImages() {
			int count = 0;
			int size = Math.Max(Image.Width, Image.Height);
			while(size > Item.LODImageMinSize) {
				size = size / 2;
				count++;
			}
			if(count == 0)
				return new Image[0];
			Image[] res = new Image[count];
			res[0] = Image;
			for(int i = 1; i < count; i++) {
				res[i] = CreateHalfSizeImage(res[i - 1]);
			}
			return res;
		}
		protected Image CreateHalfSizeImage(Image image) {
			return new Bitmap(image, image.Width / 2, image.Height / 2);
		}
		protected internal virtual void ClearLODImages() {
			if(Images == null)
				return;
			for(int i = 1; i < Images.Length; i++) {
				if(Images[i] != null)
					Images[i].Dispose();
			}
			Images = null;
		}
		protected internal Image[] Images {
			get;
			set;
		}
		public bool IsSqueezeZoomed(Size boundsRectSize) {
			if(ImageWidth > boundsRectSize.Width) {
				return true;
			}
			if(ImageHeight > boundsRectSize.Height) {
				return true;
			}
			return false;
		}
		protected internal bool IsSystemImage {
			get { return Image == RepositoryItemPictureEdit.DefaultInitialImage; }
		}
		protected ContentAlignment ActualContentAlignment {
			get {
				return IsSystemImage ? ContentAlignment.MiddleCenter : Item.PictureAlignment;
			}
		}
		public override bool DefaultAllowDrawFocusRect { get { return true; } }
		public override bool DrawFocusRect {
			get {
				if(OwnerEdit != null) {
					return (Item.AllowFocused && OwnerEdit.EditorContainsFocus);
				}
				return false;
			}
		}
		protected override Size CalcContentSize(Graphics g) {
			Size size = new Size(16, 16);
			if(CustomHeight > 0) size.Height = Math.Max(CustomHeight, 2);
			else {
				if(Image != null && !Image.Size.IsEmpty) size = ImageSize;
			}
			return size;
		}
		bool IsAutoZoomMode(int width) {
			return Image != null && ImageWidth > 0 && ImageWidth > width &&
				(Item.SizeMode == PictureSizeMode.Zoom || Item.SizeMode == PictureSizeMode.Squeeze);
		}
		int IHeightAdaptable.CalcHeight(GraphicsCache cache, int width) {
			int res = 0, prevHeight = this._customHeight;
			this._customHeight = Item.CustomHeight;
			try {
				res = CalcBestFit(cache.Graphics).Height;
			}
			finally {
				this._customHeight = prevHeight;
			}
			if(IsAutoZoomMode(width))
				res = Math.Min(res, ImageHeight * width / ImageWidth);
			return res;
		}
		internal Rectangle ScrollSquareRectangle {
			get {
				if(OwnerEdit == null) return Rectangle.Empty;
				return ((PictureEdit)OwnerEdit).Scrollers.ScrollSquareRectangle;
			}
		}
		protected override void CalcFocusRect(Rectangle bounds) {
			base.CalcFocusRect(bounds);
			if(OwnerEdit != null && fFocusRect != Rectangle.Empty) {
				fFocusRect = ((PictureEdit)OwnerEdit).Scrollers.GetFocusRectRectangle(fFocusRect);
			}
		}
		void ISupportXtraAnimationEx.OnEndAnimation(BaseAnimationInfo info) {
			if(OwnerEdit != null && OwnerEdit.IsHandleCreated)
				OwnerEdit.Invalidate();
			UnlockPictureBounds();
		}
		protected RectangleF CalcRect(RectangleF start, RectangleF end, float value) {
			return new RectangleF(
					start.X + value * (end.X - start.X),
					start.Y + value * (end.Y - start.Y),
					start.Width + value * (end.Width - start.Width),
					start.Height + value * (end.Height - start.Height));
		}
		void ISupportXtraAnimationEx.OnFrameStep(BaseAnimationInfo info) {
			if(info.AnimationId != PictureBoundsAnimationId)
				return;
			DoubleSplineAnimationInfo spInfo = info as DoubleSplineAnimationInfo;
			PictureScreenBounds = CalcRect(PictureScreenBoundsStart, PictureScreenBoundsEnd, (float)spInfo.Value);
			PictureSourceBounds = CalcRect(PictureSourceBoundsStart, PictureSourceBoundsEnd, (float)spInfo.Value);
			if(OwnerEdit != null && OwnerEdit.IsHandleCreated)
				OwnerEdit.Invalidate();
		}
		protected internal Rectangle CalcLoadingImageBounds(Size imageSize) {
			float k = Math.Max((float)imageSize.Width / ContentRect.Width, (float)imageSize.Height / ContentRect.Height);
			Size size = k < 1 ? image.Size : new Size((int)(imageSize.Width / k), (int)(imageSize.Height / k));
			Point location = new Point(ContentRect.X + (ContentRect.Width - size.Width) / 2, ContentRect.Y + (ContentRect.Height - size.Height) / 2);
			return new Rectangle(location, size);
		}
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return Item.ContextButtonOptions; }
		}
		Rectangle ISupportContextItems.DisplayBounds {
			get { return ContentRect; }
		}
		Rectangle ISupportContextItems.DrawBounds {
			get { return ContentRect; }
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return ContentRect; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get {
				return ((RepositoryItemPictureEdit)Item).ContextButtons;
			}
		}
		Control ISupportContextItems.Control {
			get {
				return OwnerEdit;
			}
		}
		bool ISupportContextItems.DesignMode { get { return Item.IsDesignMode; } }
		bool ISupportContextItems.CloneItems { get { return false; } }
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) { }
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
			Item.RaiseContextButtonClick(e);
		}
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			Item.RaiseCustomContextButtonToolTip(e); 
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return ItemHorizontalAlignment.Left;
		}
		ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return ItemLocation.Default;
		}
		protected int ContextButtonGlyphToCaptionIndent { get { return 3; } }
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return ContextButtonGlyphToCaptionIndent;
		}
		ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return ItemVerticalAlignment.Center;
		}
		UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return LookAndFeel.ActiveLookAndFeel; }
		}
		void ISupportContextItems.Redraw() {
			if(OwnerEdit != null) {
				OwnerEdit.Invalidate();
			}
		}
		void ISupportContextItems.Update() {
			if(OwnerEdit != null) {
				OwnerEdit.Update();
			}
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			if(OwnerEdit != null) {
				OwnerEdit.Invalidate(rect);
			}
		}
		protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
			get {
				if(contextButtonsViewInfo == null)
					contextButtonsViewInfo = CreateContextButtonsViewInfo();
				return contextButtonsViewInfo;
			}
		}
		ContextItemCollectionViewInfo CreateContextButtonsViewInfo() {
			return new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems, ((ISupportContextItems)this).Options, this);
		}
		public override ToolTipControlInfo GetToolTipInfo(Point point) {
			ToolTipControlInfo info = base.GetToolTipInfo(point);
			ToolTipControlInfo contextBtnInfo = ContextButtonsViewInfo.GetToolTipInfo(point);
			return contextBtnInfo == null ? info : contextBtnInfo;
		}	   
		public virtual ContextItemHitInfo CalcContextButtonHitInfo(Point point) {
			return contextButtonsViewInfo.CalcHitInfo(point);
		}
		internal Image PrevImage { get; set; }
		internal RectangleF PrevPictureScreenBounds { get; set; }
		internal RectangleF PrevPictureSourceBounds { get; set; }
		internal void SavePrevImage() {
			ClearPrevImage();
			PrevImage = Image;
			PrevPictureScreenBounds = PictureScreenBounds;
			PrevPictureSourceBounds = PictureSourceBounds;
		}
		internal void ClearPrevImage() {
			if(!LastImageIsAsync)
				return;
			LastImageIsAsync = false;
			if(PrevImage != null)
				PrevImage.Dispose();
			PrevImage = null;
		}
		internal bool LastImageIsAsync { get; set; }
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class PictureEditPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawImage(info);
			DrawScrollSquareRectangle(info);
			DrawCaption(info);
			DrawContextButtons(info);
		}
		protected virtual void DrawCaption(ControlGraphicsInfoArgs info) {
			PictureEditViewInfo vi = (PictureEditViewInfo)info.ViewInfo;
			vi.EnsureCaptionInfo();
			if(vi.CaptionInfo == null) return;
			ObjectPainter.DrawObject(info.Cache, vi.CaptionPainter, vi.CaptionInfo);
		}
		private void DrawContextButtons(ControlGraphicsInfoArgs info) {
			PictureEditViewInfo vi = (PictureEditViewInfo)info.ViewInfo;
			new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(vi.ContextButtonsViewInfo, info.Cache, info.Bounds));
		}
		protected virtual bool ShouldDrawThumbnailImage(ImageLoadInfo imageInfo) {
			return imageInfo != null && imageInfo.IsLoaded && imageInfo.RenderImageInfo != null;
		}
		protected virtual void DrawImage(ControlGraphicsInfoArgs info) {
			PictureEditViewInfo vi = info.ViewInfo as PictureEditViewInfo;
			Size imageSize = Size.Empty;
			try {
				if(vi.Image != null) imageSize = vi.Image.Size;
			}
			catch { }
			if(vi.Image == null || imageSize.IsEmpty) {
				string text = (vi.EmptyImageText == null) ? Localizer.Active.GetLocalizedString(StringId.DataEmpty) : vi.EmptyImageText;
				if(vi.Item.NullText != null && vi.Item.NullText.Length > 0) text = vi.Item.NullText;
				vi.PaintAppearance.DrawString(info.Cache, text, vi.ClientRect, vi.PaintAppearance.GetStringFormat(vi.DefaultTextOptions));
				return;
			}
			RectangleF screen = vi.PictureScreenBounds;
			RectangleF source = vi.GetActualPictureSourceBounds();
			Image image = vi.GetActualImage();
			if(vi.PrevImage != null) {
				DrawImageCore(info, vi, vi.PrevPictureScreenBounds, vi.PrevPictureSourceBounds, vi.PrevImage, true);
			}
			DrawImageCore(info, vi, screen, source, image, false);
		}
		private void DrawImageCore(ControlGraphicsInfoArgs info, PictureEditViewInfo vi, RectangleF screen, RectangleF source, Image image, bool prevImage) {
			bool enabled = !vi.ShouldDrawImageDisabled;
			InterpolationMode oldInterpolationMode = info.Graphics.InterpolationMode;
			try {
				info.Graphics.InterpolationMode = vi.Item.PictureInterpolationMode;
				GraphicsClipState state = info.Cache.ClipInfo.SaveAndSetClip(vi.ContentRect);
				try {
					if(ShouldDrawThumbnailImage(vi.ImageInfo)) {
						Color backColor = vi.ImageInfo.BackColor;
						ImageLoaderPaintHelper.DrawRenderImage(info.Graphics, vi.ImageInfo, ToRectangle(screen), backColor, !vi.ShouldDrawImageDisabled);
					}
					else if(vi.ShouldDrawImageDisabled) {
						if(image is Metafile) {
							info.Graphics.DrawImage(image, ScaleRect(screen, source, vi.Image));
							return;
						}
						PointF[] points = new PointF[] { new PointF(screen.X, screen.Y), new PointF(screen.Right, screen.Y), new PointF(screen.X, screen.Bottom) };
						info.Graphics.DrawImage(image, points, source, GraphicsUnit.Pixel, DevExpress.Utils.Paint.XPaint.DisabledImageAttr);
					}
					else {
						if(image is Metafile) {
							info.Graphics.DrawImage(image, ScaleRect(screen, source, vi.Image));
							return;
						}
						if(vi.Item.ZoomFactorCore == 1.0) {
							Rectangle rect = prevImage ? ToRectangle(screen) : GetPictureScreenBounds(vi, source.Size);
							info.Graphics.DrawImage(image, rect, ToRectangle(source), GraphicsUnit.Pixel);
						}
						else
							info.Graphics.DrawImage(image, screen, source, GraphicsUnit.Pixel);
					}
				}
				finally {
					info.Cache.ClipInfo.RestoreClipRelease(state);
				}
			}
			finally {
				info.Graphics.InterpolationMode = oldInterpolationMode;
			}
		}
		private RectangleF ScaleRect(RectangleF screen, RectangleF source, Image image) {
			float scaleX = screen.Width / source.Width;
			float scaleY = screen.Height / source.Height;
			float x = screen.X - source.X * scaleX;
			float y = screen.Y - source.Y * scaleY;
			float width = screen.Right + (image.Width - source.Right) * scaleX - x;
			float height = screen.Bottom + (image.Height - source.Bottom) * scaleY - y;
			RectangleF res = new RectangleF(x, y, width, height);
			return res;
		}
		protected Rectangle GetPictureScreenBounds(PictureEditViewInfo vi, SizeF size) {
			if(vi.ImageInfo == null || vi.ImageInfo.IsLoaded) return ToRectangle(vi.PictureScreenBounds);
			return vi.CalcLoadingImageBounds(new Size((int)size.Width, (int)size.Height));
		}
		Rectangle ToRectangle(RectangleF rect) {
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		} 
		protected override void DrawFocusRect(ControlGraphicsInfoArgs info) {
			BaseEditViewInfo vi = info.ViewInfo as BaseEditViewInfo;
			if(!vi.AllowDrawFocusRect || vi.FocusRect.IsEmpty) return;
			Brush brush = vi.PaintAppearance.GetBackBrush(info.Cache);
			Rectangle r = vi.FocusRect;
			if(vi.DrawFocusRect) {
				r.Inflate(-1, -1); 
				info.Paint.DrawFocusRectangle(info.Graphics, r,
					info.ViewInfo.PaintAppearance.ForeColor, Color.Empty);
			} else {
				if(vi.Item.UseParentBackground || (vi.AllowDrawContent && vi.FillBackground)) return;
				DrawRectangle(info, brush, r, 1);
			}
		}
		public virtual void DrawScrollSquareRectangle(ControlGraphicsInfoArgs info) {
			PictureEditViewInfo vi = info.ViewInfo as PictureEditViewInfo;
			if(vi.ScrollSquareRectangle.IsEmpty) return;
			info.ViewInfo.PaintAppearance.FillRectangle(info.Cache, vi.ScrollSquareRectangle);
		}
	}
}
namespace DevExpress.XtraEditors.Events {
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public class PopupMenuShowingEventArgs : CancelEventArgs {
		DXPopupMenu menu;
		Point p;
		bool restoreMenu = false;
		public PopupMenuShowingEventArgs(DXPopupMenu menu, Point point) {
			this.menu = menu;
			this.p = point;
		}
		public DXPopupMenu PopupMenu { get { return menu; } }
		public Point Point {
			get { return p; }
			set { p = value; }
		}
		public bool RestoreMenu {
			get { return restoreMenu; }
			set { restoreMenu = value; }
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public enum CameraMenuItemVisibility { Auto, Always, Never }
	public enum PictureStoreMode {Default, Image, ByteArray};
	public enum PictureSizeMode { Clip, Stretch, Zoom, StretchHorizontal, StretchVertical, Squeeze }
	public interface IPictureMenu {
		UserLookAndFeel LookAndFeel { get; }
		PictureStoreMode PictureStoreMode { get; }
		Image Image {
			get;
			set;
		}
		void OnDialogShowing();
		void OnDialogClosed();
		object EditValue {
			get;
			set;
		}
		bool LockFocus {
			get;
			set;
		}
		bool ReadOnly {
			get;
		}
		Control OwnerControl { get; }
	}
	[ToolboxItem(false)]
	public class PictureMenu : DXPopupMenu {
		int[] zoomSteps = new int[] { 5, 10, 15, 20, 30, 50, 70, 100, 150, 200, 300, 500, 700, 1000 };
		OpenFileDialog openFile;
		SaveFileDialog saveFile;
		TakePictureDialog takePictureDialog;
		IPictureMenu menuControl;
		RepositoryItemTrackBar zoomEdit;
		DXEditMenuItem zoomTo;
		DXMenuCheckItem zoomFullSize, zoomFitImage;
		DXMenuItem miCut, miCopy, miPaste, miDelete, miLoad, miSave, miTake;
		DXSubMenuItem miZoomMenu;
		string imageLocation = null;
		bool updateValues = false;
		public PictureMenu(IPictureMenu menuControl) {
			this.menuControl = menuControl;	
			InitializeComponent();
		}
		bool ReadOnlyControl {
			get {
				if(menuControl == null) return true;
				return menuControl.ReadOnly;
			}
		}
		protected virtual IPictureMenu MenuControl { get { return menuControl; } }
		internal string GetImageLocation() { return string.Format("{0}", imageLocation); }
		protected override void OnBeforePopup() {
			if(miCut != null) miCut.Enabled = (SourceControlImage != null) && !ReadOnlyControl;
			if(miCopy != null) miCopy.Enabled = (SourceControlImage != null);
			if(miPaste != null) miPaste.Enabled = (ClipboardImage != null) && !ReadOnlyControl;
			if(miDelete != null) miDelete.Enabled = (SourceControlImage != null) && !ReadOnlyControl;
			if(miLoad != null) miLoad.Enabled = !ReadOnlyControl;
			if(miSave != null) miSave.Enabled = (SourceControlImage != null);
			if(miTake != null && OwnerEdit != null) {
				miTake.Enabled = AllowTakePictureDialog;
				miTake.Visible = miTake.Visible ? EnsureTakePictureItemVisible() : false;
			}
			if(miZoomMenu != null && OwnerEdit != null)
				miZoomMenu.Visible = OwnerEdit.Properties.AllowSubMenu && SourceControlImage != null && !(SourceControlImage is Metafile);
			zoomTo.Visible = OwnerEdit.MenuManager != null;
			base.OnBeforePopup();
		}
		bool EnsureTakePictureItemVisible() {
			if(OwnerEdit.Properties.ShowCameraMenuItem == CameraMenuItemVisibility.Auto)
				return miTake.Enabled;
			else
				return OwnerEdit.Properties.ShowCameraMenuItem == CameraMenuItemVisibility.Always;
		}
		[ThreadStatic]
		static ImageCollection imageList = null;
		static void LoadImages() {
			if(imageList == null) {
				imageList = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.PictureMenu.png", typeof(PictureMenu).Assembly, new Size(16, 16), Color.Empty);
			}
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, EventHandler clickHandler, Image image, object tag) {
			return CreateMenuItem(caption, clickHandler, image, tag, false);
		}
		protected virtual DXMenuItem CreateMenuItem(string caption, EventHandler clickHandler, Image image, object tag, bool beginGroup) {
			DXMenuItem item = new DXMenuItem(caption, clickHandler, image);
			item.BeginGroup = beginGroup;
			item.Tag = tag;
			return item;
		}
		protected virtual RepositoryItemTrackBar ZoomEdit {
			get {
				if(zoomEdit == null) {
					zoomEdit = new RepositoryItemZoomTrackBar();
					zoomEdit.Tag = this;
					zoomEdit.BorderStyle = BorderStyles.NoBorder;
					zoomEdit.Minimum = 5;
					zoomEdit.Maximum = 1000;
					zoomEdit.LargeChange = 50;
					zoomEdit.SmallChange = 10;
					zoomEdit.ShowValueToolTip = true;
					zoomEdit.BeforeShowValueToolTip += new TrackBarValueToolTipEventHandler(zoomEdit_BeforeShowValueToolTip);
					zoomEdit.ValueChanged += new EventHandler(zoomEdit_ValueChanged);
				}
				return zoomEdit;
			}
		}
		PictureEdit OwnerEdit { 
			get {
				if(MenuControl == null) return null;
				return MenuControl.OwnerControl as PictureEdit; 
			} 
		}
		protected virtual void LocalizatorChanged(object sender, EventArgs e) {
			const int imageCut = 0, imageCopy = 1, imagePaste = 2, imageDelete = 3, imageLoad = 4, imageSave = 5, fullSize = 6, fitImage = 7, zoomIn = 8, zoomOut = 9, takePic = 10;
			LoadImages();
			this.Items.Clear();
			miCut = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuCut), new EventHandler(OnClickedCut), imageList.Images[imageCut], StringId.PictureEditMenuCut);
			this.Items.Add(miCut);
			miCopy = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuCopy), new EventHandler(OnClickedCopy), imageList.Images[imageCopy], StringId.PictureEditMenuCopy);
			this.Items.Add(miCopy);
			miPaste = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuPaste), new EventHandler(OnClickedPaste), imageList.Images[imagePaste], StringId.PictureEditMenuPaste);
			this.Items.Add(miPaste);
			miDelete = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuDelete), new EventHandler(OnClickedDelete), imageList.Images[imageDelete], StringId.PictureEditMenuDelete);
			this.Items.Add(miDelete);
			miLoad = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuLoad), new EventHandler(OnClickedLoad), imageList.Images[imageLoad], StringId.PictureEditMenuLoad, true);
			this.Items.Add(miLoad);
			miTake = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.TakePictureMenuItem), new EventHandler(OnClickedTakePicture), imageList.Images[takePic], StringId.TakePictureMenuItem);
			this.Items.Add(miTake);
			miSave = CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuSave), new EventHandler(OnClickedSave), imageList.Images[imageSave], StringId.PictureEditMenuSave);
			this.Items.Add(miSave);
			takePictureDialog.Caption = Localizer.Active.GetLocalizedString(StringId.TakePictureDialogTitle);
			openFile.Filter = Localizer.Active.GetLocalizedString(StringId.PictureEditOpenFileFilter);
			openFile.Title = Localizer.Active.GetLocalizedString(StringId.PictureEditOpenFileTitle);
			saveFile.Filter = Localizer.Active.GetLocalizedString(StringId.PictureEditSaveFileFilter);
			saveFile.Title = Localizer.Active.GetLocalizedString(StringId.PictureEditSaveFileTitle);
			if(OwnerEdit != null) {
				miZoomMenu = new DXSubMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuZoom), new EventHandler(OnBeforeZoomPopup));
				miZoomMenu.BeginGroup = true;
				miZoomMenu.Tag = StringId.PictureEditMenuZoom;
				zoomFullSize = new DXMenuCheckItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuFullSize), false, imageList.Images[fullSize], new EventHandler(OnFullSize));
				zoomFullSize.Tag = StringId.PictureEditMenuFullSize;
				zoomFitImage = new DXMenuCheckItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuFitImage), false, imageList.Images[fitImage], new EventHandler(OnFitImage));
				zoomFitImage.Tag = StringId.PictureEditMenuFitImage;
				miZoomMenu.Items.Add(zoomFullSize);
				miZoomMenu.Items.Add(zoomFitImage);
				miZoomMenu.Items.Add(CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuZoomIn), new EventHandler(OnZoomIn), imageList.Images[zoomIn], StringId.PictureEditMenuZoomIn, true));
				miZoomMenu.Items.Add(CreateMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuZoomOut), new EventHandler(OnZoomOut), imageList.Images[zoomOut], StringId.PictureEditMenuZoomOut));
				zoomTo = new DXEditMenuItem(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuZoomTo), ZoomEdit, null);
				zoomTo.BeginGroup = true;
				zoomTo.Width = 200;
				zoomTo.Tag = StringId.PictureEditMenuZoomTo;
				miZoomMenu.Items.Add(zoomTo);
				this.Items.Add(miZoomMenu);
			}
		}
		void zoomEdit_BeforeShowValueToolTip(object sender, TrackBarValueToolTipEventArgs e) {
			e.ShowArgs.ToolTip = string.Format(Localizer.Active.GetLocalizedString(StringId.PictureEditMenuZoomToolTip), e.ShowArgs.ToolTip);
		}
		void zoomEdit_ValueChanged(object sender, EventArgs e) {
			updateValues = true;
			try {
				TrackBarControl edit = sender as TrackBarControl;
				OwnerEdit.Properties.SizeMode = PictureSizeMode.Clip;
				OwnerEdit.Properties.ZoomFactor = edit.Value;
			} finally {
				updateValues = false;
			}
		}
		void OnBeforeZoomPopup(object sender, EventArgs e) {
			updateValues = true;
			try {
				if(zoomTo != null) {
					if(OwnerEdit.Properties.SizeMode == PictureSizeMode.Clip)
						zoomTo.EditValue = OwnerEdit.Properties.ZoomFactor;
					else zoomTo.EditValue = GetStretchZoomPercent();
				}
				zoomFullSize.Checked = (OwnerEdit.Properties.SizeMode == PictureSizeMode.Clip && OwnerEdit.Properties.ZoomFactor == 100);
				zoomFitImage.Checked = (OwnerEdit.Properties.SizeMode == PictureSizeMode.Squeeze || OwnerEdit.Properties.SizeMode == PictureSizeMode.Zoom);
			} finally {
				updateValues = false;
			}
		}
		void OnZoomIn(object sender, EventArgs e) { ZoomIn(); }
		internal void ZoomIn() {
			double val = SetClipMode();
			for(int i = 0; i < zoomSteps.Length; i++) {
				if(val < zoomSteps[i]) {
					OwnerEdit.Properties.SetZoomFactor(zoomSteps[i]);
					break;
				}
			}
		}
		void OnZoomOut(object sender, EventArgs e) { ZoomOut(); }
		internal void ZoomOut() {
			double val = SetClipMode();
			for(int i = zoomSteps.Length - 1; i >= 0; i--) {
				if(val > zoomSteps[i]) {
					OwnerEdit.Properties.SetZoomFactor(zoomSteps[i]);
					break;
				}
			}
		}
		int GetStretchZoomPercent() {
			if(OwnerEdit.Image == null) return 100;
			if(OwnerEdit.ViewInfo.ClientRect.Width > OwnerEdit.Image.Width && OwnerEdit.ViewInfo.ClientRect.Height > OwnerEdit.Image.Height) return 100;
			return (int)Math.Min(OwnerEdit.ViewInfo.PictureScreenBounds.Width * 100 / OwnerEdit.ViewInfo.PictureSourceBounds.Width, OwnerEdit.ViewInfo.PictureScreenBounds.Height * 100 / OwnerEdit.ViewInfo.PictureSourceBounds.Height);
		}
		double SetClipMode() {
			OwnerEdit.lockUpdateScrollers = true;
			try {
				double val = (double)OwnerEdit.Properties.ZoomFactor;
				if(OwnerEdit.Properties.SizeMode != PictureSizeMode.Clip) {
					val = GetStretchZoomPercent();
					OwnerEdit.Properties.SizeMode = PictureSizeMode.Clip;
				}
				return val;
			} finally {
				OwnerEdit.lockUpdateScrollers = false;
			}
		}
		void OnFullSize(object sender, EventArgs e) { FullSize(); }
		internal void FullSize() {
			if(updateValues) return;
			OwnerEdit.Properties.SizeMode = PictureSizeMode.Clip;
			OwnerEdit.Properties.ZoomFactor = 100;
		}
		void OnFitImage(object sender, EventArgs e) { FitImage(); }
		internal void FitImage() {
			if(updateValues) return;
			OwnerEdit.Properties.SizeMode = PictureSizeMode.Squeeze;
		}
		public override void Dispose() {
			Localizer.ActiveChanged -= new EventHandler(LocalizatorChanged);
			if(MenuControl != null && MenuControl.Image != null) {
				PictureEdit edit = MenuControl as PictureEdit;
				if(edit != null && edit.Properties.AllowDisposeImage)
				MenuControl.Image.Dispose();
			}
			this.menuControl = null;
			if(this.openFile != null) this.openFile.Dispose();
			if(this.saveFile != null) this.saveFile.Dispose();
			if(zoomEdit != null) {
				zoomEdit.Dispose();
				zoomEdit = null;
			}
			base.Dispose();
		}
		protected virtual void InitializeComponent() {
			Localizer.ActiveChanged += new EventHandler(LocalizatorChanged);
			takePictureDialog = new TakePictureDialog();
			openFile = new OpenFileDialog();
			saveFile = new SaveFileDialog();
			saveFile.FilterIndex = 3;
			LocalizatorChanged(null, EventArgs.Empty);
		}
		protected virtual void OnClickedTakePicture(object sender, EventArgs e) {
			TakePictureFromCamera();
		}
		void TakePictureFromCamera() {
			if(MenuControl != null) {
				MenuControl.LockFocus = true;
				MenuControl.OnDialogShowing();
			}
			try {
				if(takePictureDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					try {
						PasteImage(takePictureDialog.Image);
					}
					catch { XtraMessageBox.Show(MenuControl.LookAndFeel, Localizer.Active.GetLocalizedString(StringId.PictureEditOpenFileError), Localizer.Active.GetLocalizedString(StringId.PictureEditOpenFileErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Error); }
				}
			}
			finally {
				if(MenuControl != null) {
					MenuControl.OnDialogClosed();
					MenuControl.LockFocus = false;
				}
			}
		}
		protected virtual void OnClickedLoad(object sender, EventArgs e) {
			LoadImage();
		}
		protected virtual void OnClickedSave(object sender, EventArgs e) {
			if(SourceControlImage == null) return;
			if(MenuControl != null) {
				MenuControl.LockFocus = true;
				MenuControl.OnDialogShowing();
			}
			try {
				if(saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					FileInfo fi = new FileInfo(saveFile.FileName);
					FileStream fs = null;
					ImageFormat imf = null;
					try {
						if(fi.Exists) fi.Delete();
						fs = new FileStream(saveFile.FileName, FileMode.Create);
						switch(saveFile.FilterIndex) {
							case 1: imf = ImageFormat.Bmp; break;
							case 2: imf = ImageFormat.Gif; break;
							case 4: imf = ImageFormat.Png; break;
							default: imf = ImageFormat.Jpeg; break;
						}
						SourceControlImage.Save(fs, imf);
					}
					catch(Exception ex) {
						XtraMessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					finally { 
						if(fs != null)
							fs.Close(); 
					}
				}
			}
			finally {
				if(MenuControl != null) {
					MenuControl.OnDialogClosed();
					MenuControl.LockFocus = false;
				}
			}
		}
		protected virtual void OnClickedCut(object sender, EventArgs e) {
			((Control)MenuControl).Refresh();
			ClipboardImage = SourceControlImage;
			PasteImage(null);
		}
		protected virtual void OnClickedCopy(object sender, EventArgs e) { 
			ClipboardImage = SourceControlImage; 
		}
		protected virtual void OnClickedPaste(object sender, EventArgs e) { 
			PasteImage(ClipboardImage); 
		}
		protected virtual void OnClickedDelete(object sender, EventArgs e) {
			PasteImage(null);
			if(MenuControl != null)
				((Control)MenuControl).Refresh();
		}
		protected virtual Image SourceControlImage {
			get { 
				if(MenuControl != null) {
					return MenuControl.Image;
				}
				return null;
			}
		}
		bool IsImagePictureStoreMode { 
			get {
				if(OwnerEdit != null && MenuControl.PictureStoreMode == PictureStoreMode.Default)
					return OwnerEdit.InplaceType != InplaceType.Grid;
				return MenuControl.PictureStoreMode == PictureStoreMode.Image;
			}
		}
		Image tempImage = null;
		protected virtual void PasteImage(Image im) { PasteImage(im, null); }
		protected virtual void PasteImage(Image im, string openFile) {
			if(MenuControl != null) {
				this.imageLocation = openFile;
				DisposeTempImage();
				if(MenuControl.Image != null) tempImage = MenuControl.Image;
				if(!(MenuControl.EditValue is byte[]) && IsImagePictureStoreMode)
					MenuControl.EditValue = im;
				else
					MenuControl.EditValue = ByteImageConverter.ToByteArray(im, ByteImageConverter.GetImageFormatByPixelFormat(im));
				if(OwnerEdit != null && OwnerEdit.AllowForceDisposeTempImage) DisposeTempImage();
			}
		}
		internal void DisposeTempImage() {
			if(tempImage != null && !ReferenceEquals(tempImage, MenuControl.Image)) {
				tempImage.Dispose();
				tempImage = null;
			}
		}
		private Image ClipboardImage {
			get {
				try {
					if(Clipboard.ContainsImage())
						return Clipboard.GetImage();
				}
				catch { }
				return null;
			}
			set {
				try {
					Clipboard.SetImage(value);
				}
				catch {
					XtraMessageBox.Show(Localizer.Active.GetLocalizedString(StringId.PictureEditCopyImageError), Localizer.Active.GetLocalizedString(StringId.CaptionError), MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
		internal void LoadImage() {
			if(MenuControl != null) {
				MenuControl.LockFocus = true;
				MenuControl.OnDialogShowing();
			}
			try {
				openFile.FilterIndex = openFile.Filter.Split('|').Length / 2 - 1;
				if(openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					try {
						PasteImage(Image.FromFile(openFile.FileName), openFile.FileName);
					} catch { 
						XtraMessageBox.Show(MenuControl.LookAndFeel, Localizer.Active.GetLocalizedString(StringId.PictureEditOpenFileError), Localizer.Active.GetLocalizedString(StringId.PictureEditOpenFileErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Error);
						imageLocation = string.Empty;
					}
				}
			}
			finally {
				if(MenuControl != null) {
					MenuControl.OnDialogClosed();
					MenuControl.LockFocus = false;
				}
			}
		}
		bool AllowTakePictureDialog { 
			get {
				try {
					var deviceInfoList = CameraControl.GetDevices();
					return deviceInfoList.Count > 0;
				}
				catch {
					return false;
				}
			}
		}
	}
	public class BackgroundImageLoaderEventArgs : EventArgs {
		Exception error;
		bool cancelled;
		public BackgroundImageLoaderEventArgs(Exception error, bool cancelled) {
			this.cancelled = cancelled;
			this.error = error;
		}
		public Exception Error { get { return error; } }
		public bool HasError { get { return error != null; } }
		public bool Cancelled { get { return cancelled; } }
	}
	public delegate void BackgroundImageLoaderEventHandler(object sender, BackgroundImageLoaderEventArgs e);
	public class BackgroundImageLoader : IDisposable {
		Image result = null;
		BackgroundWorker worker;
		public event BackgroundImageLoaderEventHandler Loaded;
		public BackgroundImageLoader() {
		}
		public void Abort() {
			if(this.worker != null) {
				this.worker.CancelAsync();
				this.result = null;
				this.worker = null;
			}
		}
		public virtual void Dispose() {
			Loaded = null;
			Abort();
		}
		public bool Loading { get { return worker != null; } }
		public void Load(string file) {
			if(Loading) {
				Abort();
			}
			LoadAsync(file);
		}
		void LoadAsync(string file) {
			this.worker = new BackgroundWorker();
			this.worker.WorkerSupportsCancellation = true;
			this.worker.DoWork += new DoWorkEventHandler(LoadAsyncThread);
			this.worker.RunWorkerAsync(file);
			this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadAsyncCompleted);
		}
		public Image Result { get { return result; } }
		void LoadAsyncCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if(!e.Cancelled) {
				Exception ex = e.Result as Exception;
				this.result = e.Result as Image;
				if(Loaded != null) Loaded(this, new BackgroundImageLoaderEventArgs(ex, false));
			}
			else {
				if(Loaded != null) Loaded(this, new BackgroundImageLoaderEventArgs(null, true));
			}
			((BackgroundWorker)sender).Dispose();
			if(this.worker == sender) this.worker = null;
		}
		void LoadAsyncThread(object sender, DoWorkEventArgs e) {
			try {
				LoadAsyncThreadCore(sender, e);
			}
			catch(Exception ex) {
				if(!e.Cancel) e.Result = ex;
			}
		}
		void LoadAsyncThreadCore(object sender, DoWorkEventArgs e) {
			BackgroundWorker worker = sender as BackgroundWorker;
			WebRequest request = WebRequest.Create(CalculateUri(e.Argument.ToString()));
			if(CheckCancel(worker, e, request)) return;
			MemoryStream ms;
			using(WebResponse response = request.GetResponse()) {
				if(CheckCancel(worker, e, request)) return;
				Stream stream = response.GetResponseStream();
				if(CheckCancel(worker, e, request)) return;
				ms = new MemoryStream();
				byte[] buffer = new byte[1000];
				while(true) {
					if(CheckCancel(worker, e, request)) return;
					int len = stream.Read(buffer, 0, buffer.Length);
					if(len == 0) break;
					ms.Write(buffer, 0, len);
				}
				response.Close();
			}
			ms.Seek(0, SeekOrigin.Begin);
			Image res = Image.FromStream(ms);
			CheckCancel(worker, e, request);
			if(!e.Cancel) {
				e.Result = res;
			}
			else {
				res.Dispose();
			}
		}
		bool CheckCancel(BackgroundWorker worker, DoWorkEventArgs e, WebRequest request) {
			if(worker.CancellationPending) {
				request.Abort();
				e.Cancel = true;
				return true;
			}
			return false;
		}
		Uri CalculateUri(string path) {
			try {
				return new Uri(path);
			}
			catch(UriFormatException) {
				path = Path.GetFullPath(path);
				return new Uri(path);
			}
		}
	}
}
