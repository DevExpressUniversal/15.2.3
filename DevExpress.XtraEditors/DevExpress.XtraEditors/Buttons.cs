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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.Serialization;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.Utils.WXPaint;
using DevExpress.Utils.Drawing.Animation;
using System.Reflection;
using DevExpress.Utils.Text;
using System.Drawing.Design;
namespace DevExpress.XtraEditors.Controls {
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	public class EditorButtonCollection : CollectionBase {
		int _lockUpdate;
		public event CollectionChangeEventHandler CollectionChanged;
		public override string ToString() {
			if(Count == 0) return "None";
			if(Count == 1) {
				return string.Concat("{", this[0].Kind.ToString(), "}");
			}
			return string.Format("Count {0}", Count);
		}
		public EditorButtonCollection() {
			this._lockUpdate = 0;
		}
		protected virtual void BeginUpdate() {
			_lockUpdate++;
		}
		protected virtual void EndUpdate() {
			if(--_lockUpdate == 0)
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected void UpdateIndexes() {
			for(int n = Count - 1; n >= 0; n--) this[n].SetIndex(n);
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("EditorButtonCollectionItem")]
#endif
		public EditorButton this[int index] { get { return List[index] as EditorButton; } }
		public virtual void AddRange(EditorButton[] buttons) {
			BeginUpdate();
			try {
				foreach(EditorButton button in buttons) {
					Add(button);
				}
			}
			finally {
				EndUpdate();
			}
		}
		[Browsable(false)]
		public virtual int VisibleCount {
			get {
				int count = Count, res = 0;
				if(count == 0) return 0;
				for(int n = 0; n < count; n++) {
					if(this[n].Visible) res++;
				}
				return res;
			}
		}
		public virtual void Assign(EditorButtonCollection collection) {
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < collection.Count; n++) {
					EditorButton button = collection[n];
					EditorButton newButton = new EditorButton();
					newButton.Assign(button);
					Add(newButton);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual int Add(EditorButton button) {
			int res = IndexOf(button);
			if(res == -1) res = List.Add(button);
			return res;
		}
		public virtual int IndexOf(EditorButton button) { return List.IndexOf(button); }
		public virtual bool Contains(EditorButton button) { return List.Contains(button); }
		public virtual void Insert(int index, EditorButton button) {
			if(Contains(button)) return;
			List.Insert(index, button);
		}
		protected override void OnInsertComplete(int index, object item) {
			EditorButton button = item as EditorButton;
			button.Changed += new EventHandler(OnButton_Changed);
			button.SetCollection(this);
			UpdateIndexes();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			EditorButton button = item as EditorButton;
			button.Changed -= new EventHandler(OnButton_Changed);
			button.SetCollection(null);
			UpdateIndexes();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			if(Count == 0) return;
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					RemoveAt(n);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void OnButton_Changed(object sender, EventArgs e) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, sender));
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(_lockUpdate > 0) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
	}
	public class EditorButtonHelper {
		[ThreadStatic]
		static EditorButtonPainter[] painters = null;
		static EditorButtonPainter[] Painters {
			get {
				if(painters == null) CreatePainters();
				return painters;
			}
		}
		static void CreatePainters() {
			painters = new EditorButtonPainter[Enum.GetValues(typeof(BorderStyles)).Length];
			painters[(int)BorderStyles.NoBorder] = new EditorButtonPainter(new FlatButtonObjectPainter());
			painters[(int)BorderStyles.Simple] = new EditorButtonPainter(new SimpleButtonObjectPainter());
			painters[(int)BorderStyles.Flat] = new EditorButtonPainter(new FlatButtonObjectPainter());
			painters[(int)BorderStyles.HotFlat] = new EditorButtonPainter(new HotFlatButtonObjectPainter());
			painters[(int)BorderStyles.UltraFlat] = new EditorButtonPainter(new UltraFlatButtonObjectPainter());
			painters[(int)BorderStyles.Style3D] = new EditorButtonPainter(new Style3DButtonObjectPainter());
			painters[(int)BorderStyles.Office2003] = new EditorButtonPainter(new Office2003ButtonObjectPainter());
			painters[(int)BorderStyles.Default] = new EditorButtonPainter(new Office2003ButtonInBarsObjectPainter());
		}
		public static EditorButtonPainter GetPainter(BorderStyles style) {
			return GetPainter(style, null);
		}
		public static EditorButtonPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel, bool editorButton) {
			if(style == BorderStyles.Default) {
				if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP) return new WindowsXPEditorButtonPainter();
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) return new SkinEditorButtonPainter(lookAndFeel);
				if(lookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Office2003)
					return new EditorButtonPainter(lookAndFeel.Painter.Button);
				else
					style = BorderStyles.Office2003;
			}
			if(editorButton && style == BorderStyles.Office2003) style = BorderStyles.Default;
			return Painters[(int)style];
		}
		public static EditorButtonPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel) {
			return GetPainter(style, lookAndFeel, false);
		}
		public static EditorButtonPainter GetWindowsXPPainter() {
			if(DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) return new WindowsXPEditorButtonPainter();
			return GetPainter(BorderStyles.Flat);
		}
	}
	[TypeConverter(typeof(EditorButtonTypeConverter))] 
	public class EditorButton : ICaptionSupport, DevExpress.Utils.MVVM.ISupportCommandBinding { 
		public event EventHandler Changed;
		AppearanceObject appearance;
		bool _defaultButton;
		string fCaption, _toolTip = "";
		Image _image;
		KeyShortcut _shortcut;
		bool fEnabled, _isLeft, _visible;
		int _width;
		ButtonPredefines _kind;
		ImageLocation fImageLocation;
		int fImageIndex;
		object _imageList;
		object tag;
		protected int indexCore = -1;
		EditorButtonCollection collection;
		SuperToolTip superTip;
		bool enableImageTransparency;
		[EditorButtonPreferredConstructor]
		public EditorButton() {
			this.enableImageTransparency = true;
			this.tag = null;
			this.fImageIndex = -1;
			this._imageList = null;
			this._defaultButton = false;
			this.appearance = new SerializableAppearanceObject();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
			this.collection = null;
			Reset();
		}
		internal EditorButton(SerializationInfo si, StreamingContext context)
			: this() {
			DevExpress.Utils.Design.UniversalSerializer.DeserializeObject(this, si);
		}
		public EditorButton(ButtonPredefines kind, string toolTip)
			: this() {
			this._kind = kind;
			this._toolTip = toolTip;
		}
		public EditorButton(ButtonPredefines kind, string toolTip, SuperToolTip superTip)
			: this() {
			this._kind = kind;
			this._toolTip = toolTip;
			this.superTip = superTip;
		}
		public EditorButton(ButtonPredefines kind, Image image, SuperToolTip superTip)
			: this() {
			this._kind = kind;
			this.superTip = superTip;
			this._image = image;
			EnsureActualImage();
		}
		public EditorButton(object tag, ButtonPredefines kind)
			: this(kind) {
			this.tag = tag;
		}
		[EditorButtonPreferredConstructor]
		public EditorButton(ButtonPredefines kind)
			: this() {
			this._kind = kind;
		}
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, KeyShortcut.Empty, (AppearanceObject)null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, KeyShortcut.Empty, (AppearanceObject)null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, (AppearanceObject)null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, shortcut, (AppearanceObject)null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, string toolTip)
			:
					this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, (AppearanceObject)null, toolTip) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, string toolTip)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, shortcut, (AppearanceObject)null, toolTip) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, string toolTip, object tag)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, (AppearanceObject)null, toolTip, tag) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, string toolTip, object tag)
			:
			this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, shortcut, (AppearanceObject)null, toolTip, tag) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, AppearanceObject appearance) : this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, appearance, "") { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, AppearanceObject appearance) : this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, shortcut, appearance, "") { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip) : this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, appearance, toolTip, null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip) : this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, shortcut, appearance, toolTip, null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag) : this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, appearance, toolTip, tag, null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag) : this(kind, caption, width, enabled, visible, isLeft, imageAlignment, image, shortcut, appearance, toolTip, tag, null) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip) : this(kind, caption, width, enabled, visible, isLeft, (ImageLocation)(int)imageAlignment, image, shortcut, appearance, toolTip, tag, superTip, false) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip) : this(kind, caption, width, enabled, visible, isLeft, imageLocation, image, shortcut, appearance, toolTip, tag, superTip, false) { }
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, HorzAlignment imageAlignment, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency)
			: this(kind, caption, width, enabled, visible, isLeft, (ImageLocation)(int)imageAlignment, image, shortcut, appearance, toolTip, tag, superTip, enableImageTransparency) {
		}
		[EditorButtonPreferredConstructor]
		public EditorButton(ButtonPredefines kind, string caption, int width, bool enabled, bool visible, bool isLeft, ImageLocation imageLocation, Image image, KeyShortcut shortcut, AppearanceObject appearance, string toolTip, object tag, SuperToolTip superTip, bool enableImageTransparency)
			: this() {
			if(shortcut != null) this._shortcut = shortcut;
			this.enableImageTransparency = enableImageTransparency;
			this.tag = tag;
			this._toolTip = toolTip;
			this.superTip = superTip;
			this._visible = visible;
			this._kind = kind;
			this.fCaption = caption;
			this._width = width;
			this.fEnabled = enabled;
			this._isLeft = isLeft;
			this.fImageLocation = imageLocation;
			InitAppearance(appearance);
			this._image = image;
			EnsureActualImage();
		}
		protected virtual void InitAppearance(AppearanceObject appearance) {
			if(appearance != null) Appearance.Assign(appearance);
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		string ICaptionSupport.Caption {
			get {
				if(Caption.Length > 0) return Caption;
				return Kind.ToString();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonCollection"),
#endif
 Browsable(false)]
		public EditorButtonCollection Collection { get { return collection; } }
		internal void SetCollection(EditorButtonCollection collection) {
			if(this.Collection == collection) return;
			this.collection = collection;
			this.indexCore = -1;
			OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged(){ }
		internal void SetIndex(int index) { this.indexCore = index; }
		public virtual void Assign(EditorButton source) {
			this.enableImageTransparency = source.enableImageTransparency;
			this._toolTip = source.ToolTip;
			this.superTip = source.SuperTip;
			this.fImageIndex = source.ImageIndex;
			this._imageList = source.ImageList;
			this.fCaption = source.Caption;
			this.fEnabled = source.Enabled;
			this.fImageLocation = source.ImageLocation;
			ResetActualImage();
			this._image = source.Image;
			this._isLeft = source.IsLeft;
			this._kind = source.Kind;
			this._visible = source.Visible;
			this._width = source.Width;
			this.tag = source.Tag;
			this._shortcut = new KeyShortcut(source.Shortcut.Key);
			this._defaultButton = source._defaultButton;
			this.appearance.Assign(source.Appearance);
		}
		public virtual void Reset() {
			this._toolTip = "";
			this.superTip = null;
			this.fImageIndex = -1;
			this._imageList = null;
			this.fCaption = "";
			ResetActualImage();
			this._image = null;
			this._visible = this.fEnabled = true;
			this._isLeft = false;
			this._width = -1;
			this._kind = ButtonPredefines.Ellipsis;
			this.fImageLocation = ImageLocation.MiddleCenter;
			this._shortcut = KeyShortcut.Empty;
			this.appearance.BeginUpdate();
			this.enableImageTransparency = true;
			try {
				this.appearance.Reset();
			}
			finally {
				this.appearance.CancelUpdate();
			}
			OnChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int Index {
			get {
				if(indexCore == -1)
					indexCore = GetDefaultIndex();
				return indexCore;
			}
		}
		protected virtual int GetDefaultIndex(){
			if(Collection != null)
				return Collection.IndexOf(this);
			return -1;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsDefaultButton { get { return _defaultButton; } set { _defaultButton = value; } }
		bool ShouldSerializeImageIndex() { return ImageIndex != -1; }
		protected internal virtual int ImageIndex {
			get { return fImageIndex; }
			set {
				if(value < 0) value = -1;
				if(ImageIndex == value) return;
				fImageIndex = value;
				OnChanged();
			}
		}
		bool ShouldSerializeImageList() { return ImageList != null; }
		protected internal virtual object ImageList {
			get { return _imageList; }
			set {
				if(ImageList == value) return;
				_imageList = value;
				OnChanged();
			}
		}
		[DefaultValue(null),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonTag"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object Tag {
			get { return tag; }
			set { tag = value; }
		}
		bool ShouldSerializeShortcut() { return Shortcut.IsExist; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonShortcut"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Editor("DevExpress.XtraEditors.Design.EditorButtonShortcutEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual KeyShortcut Shortcut {
			get { return _shortcut; }
			set {
				if(value == null) value = KeyShortcut.Empty;
				if(Shortcut.Key == value.Key) return;
				_shortcut = value;
				OnChanged();
			}
		}
		internal bool ShouldSerializeSuperTip() { return SuperTip != null && !SuperTip.IsEmpty; }
		[Localizable(true),
		Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonSuperTip")
#else
	Description("")
#endif
]
		public SuperToolTip SuperTip {
			get { return superTip; }
			set {
				if(SuperTip == value) return;
				superTip = value;
				OnChanged();
			}
		}
		public void ResetSuperTip() { SuperTip = null; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonToolTip"),
#endif
 DefaultValue("")]
		public virtual string ToolTip {
			get { return _toolTip; }
			set {
				if(ToolTip == value) return;
				_toolTip = value;
				OnChanged();
			}
		}
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("EditorButtonAppearance")]
#endif
		public virtual AppearanceObject Appearance { get { return appearance; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonEnabled"),
#endif
 DefaultValue(true)]
		public virtual bool Enabled {
			get { return fEnabled; }
			set {
				if(Enabled == value) return;
				fEnabled = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonVisible"),
#endif
 DefaultValue(true)]
		public virtual bool Visible {
			get { return _visible; }
			set {
				if(Visible == value) return;
				_visible = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonIsLeft"),
#endif
 DefaultValue(false)]
		public virtual bool IsLeft {
			get { return _isLeft; }
			set {
				if(IsLeft == value) return;
				_isLeft = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonWidth"),
#endif
 DefaultValue(-1)]
		public virtual int Width {
			get { return _width; }
			set {
				if(Width == value) return;
				_width = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonKind"),
#endif
 DefaultValue(ButtonPredefines.Ellipsis)]
		public virtual ButtonPredefines Kind {
			get { return _kind; }
			set {
				if(Kind == value) return;
				_kind = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonCaption"),
#endif
 DefaultValue("")]
		public virtual string Caption {
			get { return fCaption; }
			set {
				if(value == null) value = "";
				if(Caption == value) return;
				fCaption = value;
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonEnableImageTransparency"),
#endif
 DefaultValue(true)]
		public bool EnableImageTransparency {
			get { return enableImageTransparency; }
			set {
				if(EnableImageTransparency == value) return;
				enableImageTransparency = value;
				OnEnableImageTransparencyChanged();
			}
		}
		protected virtual void OnEnableImageTransparencyChanged() {
			ResetActualImage();
			EnsureActualImage();
			OnChanged();
		}
		protected Image MakeImageTransparent(Image value) {
			if(value != null) {
				return ImageHelper.MakeTransparent(value, false);
			}
			return null;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonImage"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get { return _image; }
			set {
				if(Image == value) return;
				ResetActualImage();
				_image = value;
				OnChanged();
			}
		}
		Image _actualImage;
		protected internal Image ActualImage {
			get {
				if(_actualImage == null) 
					EnsureActualImage();
				return _actualImage;
			}
		}
		void EnsureActualImage() {
			_actualImage = Image;
			if(EnableImageTransparency)
				_actualImage = MakeImageTransparent(Image);
		}
		void ResetActualImage() {
			if(_actualImage != Image && _actualImage != null)
				_actualImage.Dispose();
			_actualImage = null;
		}
		[Obsolete(ObsoleteText.SRObsoleteImageLocation), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public HorzAlignment GlyphAlignment {
			get { return ImageAlignment; }
			set { ImageAlignment = value; }
		}
		[Obsolete(ObsoleteText.SRObsoleteImageLocation), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual HorzAlignment ImageAlignment {
			get {
				if(Enum.IsDefined(typeof(HorzAlignment), (int)ImageLocation))
					return (HorzAlignment)(int)ImageLocation;
				return HorzAlignment.Default;
			}
			set {
				ImageLocation = (ImageLocation)(int)value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("EditorButtonImageLocation"),
#endif
 DefaultValue(ImageLocation.MiddleCenter)]
		public virtual ImageLocation ImageLocation {
			get { return fImageLocation; }
			set {
				if(ImageLocation == value) return;
				fImageLocation = value;
				OnChanged();
			}
		}
		ImageAlignToText imageAlignToText;
		[DefaultValue(ImageAlignToText.None), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ImageAlignToText ImageToTextAlignment {
			get { return imageAlignToText; }
			set {
				if(ImageToTextAlignment == value)
					return;
				imageAlignToText = value;
				OnChanged();
			}
		}
		int indentBetweenImageAndText = -1;
		[DefaultValue(-1), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ImageToTextIndent {
			get { return indentBetweenImageAndText; }
			set {
				if(ImageToTextIndent == value)
					return;
				indentBetweenImageAndText = value;
				OnChanged();
			}
		}
		internal bool IsMiddleImageLocation() {
			return ImageLocation == ImageLocation.Default || ImageLocation == ImageLocation.MiddleCenter ||
				   ImageLocation == ImageLocation.MiddleLeft || ImageLocation == ImageLocation.MiddleRight;
		}
		protected virtual void Init() {
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		protected virtual void OnChanged() {
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		public event EventHandler Click;
		#region Commands
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseClick() {
			EventHandler handler = Click;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class EditorButtonPainter : StyleObjectPainter {
		const int textGlyphIndent = 3;
		protected ObjectPainter fButtonPainter;
		public EditorButtonPainter(ObjectPainter buttonPainter) {
			this.fButtonPainter = buttonPainter;
		}
		public int TextGlyphIndent { get { return textGlyphIndent; } }
		protected virtual ObjectInfoArgs GetButtonInfoArgs(ObjectInfoArgs e) { return e; }
		protected virtual void UpdateButtonInfo(ObjectInfoArgs e) {
		}
		public virtual ObjectPainter ButtonPainter { get { return fButtonPainter; } }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return ButtonPainter.CalcObjectBounds(e);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			UpdateButtonInfo(e);
			Rectangle res = ButtonPainter.GetObjectClientRectangle(GetButtonInfoArgs(e));
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			res.Inflate(-ee.Indent.Width, -ee.Indent.Height);
			return res;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			UpdateButtonInfo(e);
			Rectangle res = ButtonPainter.CalcBoundsByClientRectangle(GetButtonInfoArgs(e), client);
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			res.Inflate(ee.Indent.Width, ee.Indent.Height);
			return res;
		}
		protected virtual Rectangle CalcMinBestSize(ObjectInfoArgs e) {
			Size res = ButtonPainter.CalcObjectMinBounds(GetButtonInfoArgs(e)).Size;
			return new Rectangle(Point.Empty, res);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			Size kSize = CalcKindMinSize(ee);
			Size tSize = CalcCaptionSize(ee);
			Size aSize = CalcDropDownArrowSize(ee);
			int indentBetweenImageAndText = TextGlyphIndent;
			if(ee.Button.ImageToTextAlignment != ImageAlignToText.None && ee.Button.ImageToTextIndent > 0)
				indentBetweenImageAndText = ee.Button.ImageToTextIndent;
			Size res = kSize;
			if(!tSize.IsEmpty) {
				if(!ee.Button.IsMiddleImageLocation()) {
					res.Width = Math.Max(tSize.Width, kSize.Width);
					res.Height += tSize.Height + (kSize.Height > 0 ? indentBetweenImageAndText : 0);
				}
				else {
					res.Height = Math.Max(tSize.Height, kSize.Height);
					res.Width += tSize.Width + (kSize.Width > 0 ? indentBetweenImageAndText : 0);
				}
			}
			if(!aSize.IsEmpty)
				res.Width += aSize.Width;
			if(ee.Button.Width > 0) res.Width = ee.Button.Width;
			if(res.IsEmpty) {
				return CalcMinBestSize(e);
			}
			if(ImageAndTextLayoutHelper.ImageLeftAligned(ee.Button.ImageToTextAlignment) || ImageAndTextLayoutHelper.ImageRightAligned(ee.Button.ImageToTextAlignment)) {
				if(kSize.Width > 0 && tSize.Width > 0)
					res.Width = kSize.Width + tSize.Width + indentBetweenImageAndText;
			}
			if(ImageAndTextLayoutHelper.ImageTopAligned(ee.Button.ImageToTextAlignment) || ImageAndTextLayoutHelper.ImageBottomAligned(ee.Button.ImageToTextAlignment)) {
				if(kSize.Height > 0 && tSize.Height > 0)
					res.Height = kSize.Height + tSize.Height + indentBetweenImageAndText;
			}
			res.Width += DefaultIndent * 2;
			Rectangle rect, saveBounds = e.Bounds;
			e.Bounds = new Rectangle(Point.Empty, res);
			UpdateButtonInfo(e);
			rect = CalcBoundsByClientRectangle(e);
			e.Bounds = saveBounds;
			rect.Width += ee.Padding.Horizontal;
			rect.X -= ee.Padding.Left;
			rect.Height += ee.Padding.Vertical;
			rect.Y -= ee.Padding.Top;
			return rect;
		}
		protected int DefaultIndent { get { return 1; } }
		protected virtual RotateFlipType GetRotationAngle(ObjectInfoArgs e) {
			NavigatorEditorButtonInfoArgs ne = e as NavigatorEditorButtonInfoArgs;
			if(ne == null) return RotateFlipType.RotateNoneFlipNone;
			return ne.Orientation == Orientation.Vertical ? RotateFlipType.Rotate90FlipX : RotateFlipType.RotateNoneFlipNone;
		}
		protected virtual void DrawButton(ObjectInfoArgs e) {
			new RotateObjectPaintHelper().DrawRotated(e.Cache, GetButtonInfoArgs(e), ButtonPainter, GetRotationAngle(e));
		}
		int lockTextOnly = 0;
		protected bool IsTextOnlyDrawing { get { return lockTextOnly != 0; } }
		public virtual void DrawTextOnly(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			this.lockTextOnly++;
			bool prevFill = ee.FillBackground;
			ee.FillBackground = false;
			try {
				DrawObject(e);
			}
			finally {
				this.lockTextOnly--;
				ee.FillBackground = prevFill;
			}
		}
		protected virtual void DrawContent(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			Rectangle r = GetObjectClientRectangle(e); 
			r.X += DefaultIndent;
			r.Width -= DefaultIndent * 2;
			Brush brush = GetForeBrush(ee);
			Size kindSize = CalcKindSize(ee);
			Size arrowSize = CalcDropDownArrowSize(ee);
			Size realKindSize = CalcKindSizeCore(ee);
			bool drawCaption = CanDrawCaption(ee);
			if(ee.AllowHtmlDraw) ee.StringInfo = CalcHtmlStringInfo(ee);
			bool drawKind = !kindSize.IsEmpty && !(ee.Button.Kind == ButtonPredefines.Glyph && !ee.IsImageExists);
			bool drawArrow = !arrowSize.IsEmpty;
			Rectangle captionRect = Rectangle.Empty, kindRect = new Rectangle(r.X, r.Y + (r.Height - kindSize.Height) / 2, kindSize.Width, kindSize.Height), arrowRect = Rectangle.Empty;
			if(drawArrow) {
				if(ee.RightToLeft) {
					arrowRect = new Rectangle(r.X + 1, r.Y + (r.Height - arrowSize.Height) / 2, arrowSize.Width, arrowSize.Height);
					r.X += arrowRect.Width + TextGlyphIndent - 1;
					r.Width -= arrowRect.Width + TextGlyphIndent;
					kindRect.X = r.X;
				}
				else {
					arrowRect = new Rectangle(r.Right - arrowSize.Width - 1, r.Y + (r.Height - arrowSize.Height) / 2, arrowSize.Width, arrowSize.Height);
					r.Width -= arrowRect.Width + TextGlyphIndent;
				}
			}
			captionRect = r;
			if(ee.Button.ImageToTextAlignment != ImageAlignToText.None) {
				int indentBetweenImageAndText = ee.Button.ImageToTextIndent >= 0? ee.Button.ImageToTextIndent: TextGlyphIndent;
				if(ImageAndTextLayoutHelper.ImageLeftAligned(ee.Button.ImageToTextAlignment) ||
					ImageAndTextLayoutHelper.ImageRightAligned(ee.Button.ImageToTextAlignment)) { 
					ee.MaxTextWidth = r.Width - kindSize.Width - indentBetweenImageAndText;
				}
				Size captSize = CalcCaptionSize(ee);
				captionRect = ImageAndTextLayoutHelper.CalcTextBounds(r, captSize, kindSize, ee.Button.ImageToTextAlignment, indentBetweenImageAndText, ee.Button.Appearance);
				kindRect = ImageAndTextLayoutHelper.CalcImageAttachedToTextBounds(captionRect, kindSize, ee.Button.ImageToTextAlignment, indentBetweenImageAndText);
			}
			else if(drawKind) {
				switch(ee.Button.ImageLocation) {
					case ImageLocation.MiddleCenter:
						kindRect.X = r.X + (r.Width - kindRect.Width) / 2;
						break;
					case ImageLocation.Default:
					case ImageLocation.MiddleLeft:
						captionRect.Width = r.Width - kindSize.Width - TextGlyphIndent;
						captionRect.X = kindRect.Right + TextGlyphIndent;
						break;
					case ImageLocation.MiddleRight:
						kindRect.X = r.Right - kindRect.Width;
						captionRect.X = r.X;
						captionRect.Width = r.Width - kindSize.Width - TextGlyphIndent;
						break;
					case ImageLocation.TopLeft:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						captionRect.Y = r.Bottom - captionRect.Height;
						kindRect.Y = r.Y + TextGlyphIndent;
						break;
					case ImageLocation.TopRight:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						captionRect.Y = r.Bottom - captionRect.Height;
						kindRect.Y = r.Y + TextGlyphIndent;
						kindRect.X = r.Right - kindRect.Width;
						break;
					case ImageLocation.TopCenter:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						captionRect.Y = r.Bottom - captionRect.Height;
						kindRect.Y = r.Y + TextGlyphIndent;
						kindRect.X = r.X + (r.Width - kindRect.Width) / 2;
						break;
					case ImageLocation.BottomCenter:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						kindRect.Y = captionRect.Bottom + TextGlyphIndent;
						kindRect.X = r.X + (r.Width - kindRect.Width) / 2;
						break;
					case ImageLocation.BottomLeft:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						kindRect.Y = captionRect.Bottom + TextGlyphIndent;
						break;
					case ImageLocation.BottomRight:
						captionRect.Height = r.Height - kindSize.Height - TextGlyphIndent;
						kindRect.X = r.Right - kindRect.Width;
						kindRect.Y = captionRect.Bottom + TextGlyphIndent;
						break;
				}
			}
			else {
				Size captSize = CalcCaptionSize(ee);
				if(ee.Button.Image != null) {
					switch(ee.Button.ImageLocation) {
						case ImageLocation.MiddleRight:
							captionRect.X = captionRect.Right - captSize.Width;
							captionRect.Width = captSize.Width;
							break;
						case ImageLocation.MiddleCenter:
						case ImageLocation.Default:
							captionRect.X += (captionRect.Width - captSize.Width) / 2;
							captionRect.Width = captSize.Width;
							break;
					}
				}
			}
			if(!drawCaption) captionRect = Rectangle.Empty;
			if(!IsTextOnlyDrawing && drawKind)
				DrawKindImage(ee, kindRect);
			if(drawArrow) 
				DrawDropDownArrow(ee, arrowRect);
			if(!captionRect.IsEmpty) {
				StringFormat strFormat = ee.Appearance.GetStringFormat();
				System.Drawing.Text.HotkeyPrefix prev = strFormat.HotkeyPrefix;
				strFormat.HotkeyPrefix = ee.ShowHotKeyPrefix ? System.Drawing.Text.HotkeyPrefix.Show : System.Drawing.Text.HotkeyPrefix.Hide;
				if(ee.AllowHtmlDraw && ee.StringInfo != null) {
					DevExpress.Utils.Text.StringPainter.Default.UpdateLocation(ee.StringInfo, captionRect);
					DevExpress.Utils.Text.StringPainter.Default.DrawString(ee.Cache, ee.StringInfo);
				}
				else {
					ButtonPainter.DrawCaption(ee, ee.Button.Caption, ee.Appearance.Font, brush, captionRect, strFormat);
				}
				strFormat.HotkeyPrefix = prev;
			}
		}
		protected virtual void DrawDropDownArrow(EditorButtonObjectInfoArgs e, Rectangle rect) {
			DrawArrowCore(e.Cache, ButtonPredefines.DropDown, rect, GetForeBrush(e));
		}
		public override void DrawObject(ObjectInfoArgs e) {
			UpdateButtonInfo(e);
			if(IsTextOnlyDrawing) {
				DrawContent(e);
			}
			else {
				DrawButton(e);
				DrawContent(e);
				DrawFocusRectangle(e);
			}
		}
		protected virtual void DrawFocusRectangle(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if(!ee.DrawFocusRectangle) return;
			ObjectState saveState = e.State;
			try {
				e.State = ObjectState.Normal;
				Rectangle focus = ButtonPainter.GetFocusRectangle(GetButtonInfoArgs(e));
				focus.Inflate(-1, -1);
				DrawFocusRect(ee, focus);
			}
			finally {
				e.State = saveState;
			}
		}
		protected virtual void DrawFocusRect(EditorButtonObjectInfoArgs e, Rectangle rect) {
			ControlPaint.DrawFocusRectangle(e.Graphics, rect);
		}
		protected virtual internal Color GetForeColor(EditorButtonObjectInfoArgs e) {
			ButtonObjectPainter bp = ButtonPainter as ButtonObjectPainter;
			if(bp != null) return bp.GetForeColor(e);
			if(e.State == ObjectState.Disabled) {
				if(e.Appearance.ForeColor == SystemColors.Control) return SystemColors.ControlDark;
				return ControlPaint.Dark(e.Appearance.ForeColor, 0); 
			}
			if(e.Appearance.ForeColor == Color.Empty) return SystemColors.ControlText; ;
			return e.Appearance.GetForeColor();
		}
		protected Brush GetForeBrush(EditorButtonObjectInfoArgs e) {
			return e.Cache.GetSolidBrush(GetForeColor(e));
		}
		protected virtual void DrawArrowCore(GraphicsCache cache, ButtonPredefines kind, Rectangle rect, Brush brush) {
			int i, w, x;
			switch(kind) {
				case ButtonPredefines.SpinDown:
				case ButtonPredefines.Combo:
				case ButtonPredefines.Down:
				case ButtonPredefines.DropDown:
					for(x = rect.Left, w = rect.Width, i = rect.Top; i < rect.Bottom && w > 0; i++, x++, w -= 2)
						cache.Paint.FillRectangle(cache.Graphics, brush, x, i, w, 1);
					break;
				case ButtonPredefines.SpinUp:
				case ButtonPredefines.Up:
					for(x = rect.Left, w = rect.Width, i = rect.Bottom - 1; i >= rect.Top && w > 0; i--, x++, w -= 2)
						cache.Paint.FillRectangle(cache.Graphics, brush, x, i, w, 1);
					break;
				case ButtonPredefines.SpinRight:
				case ButtonPredefines.Right:
					for(x = rect.Top, w = rect.Height, i = rect.Left; i < rect.Right && w > 0; i++, x++, w -= 2)
						cache.Paint.FillRectangle(cache.Graphics, brush, i, x, 1, w);
					break;
				case ButtonPredefines.SpinLeft:
				case ButtonPredefines.Left:
					for(x = rect.Top, w = rect.Height, i = rect.Right - 1; i >= rect.Left && w > 0; i--, x++, w -= 2)
						cache.Paint.FillRectangle(cache.Graphics, brush, i, x, 1, w);
					break;
			}
		}
		protected virtual void DrawArrow(EditorButtonObjectInfoArgs e, Rectangle rect) {
			DrawArrowCore(e.Cache, e.Button.Kind, rect, GetForeBrush(e));
		}
		protected virtual void DrawKindImage(EditorButtonObjectInfoArgs e, Rectangle rect) {
			if(e.Button.Kind == ButtonPredefines.Glyph) {
				if(e.IsImageExists) {
					Rectangle r = new Rectangle(Point.Empty, e.ImageSize);
					if(!e.GetAllowGlyphSkinning()) {
						if(e.Button.Image != null) 
							e.Cache.Paint.DrawImage(e.Graphics, e.Button.ActualImage, rect, r, e.State != ObjectState.Disabled);
						else 
							ImageCollection.DrawImageListImage(e, e.Button.ImageList, e.Button.ImageIndex, rect, e.State != ObjectState.Disabled);
					}
					else {
						var color = GetForeColor(e);
						var attributes = ImageColorizer.GetColoredAttributes(color);
						if(e.Button.Image != null) 
							e.Cache.Paint.DrawImage(e.Graphics, e.Button.ActualImage, rect, r, attributes);
						else 
							ImageCollection.DrawImageListImage(e.Cache, e.Button.ImageList, e.Button.ImageIndex, rect, attributes);
					}
				}
			}
			else {
				Point p = rect.Location;
				int size;
				Brush brush = GetForeBrush(e);
				switch(e.Button.Kind) {
					case ButtonPredefines.SpinRight:
					case ButtonPredefines.SpinLeft:
					case ButtonPredefines.Right:
					case ButtonPredefines.Left:
					case ButtonPredefines.SpinDown:
					case ButtonPredefines.SpinUp:
					case ButtonPredefines.Up:
					case ButtonPredefines.Down:
					case ButtonPredefines.Combo:
					case ButtonPredefines.DropDown:
						DrawArrow(e, rect);
						break;
					case ButtonPredefines.Ellipsis:
						size = 2;
						for(int i = 0; i < 3; i++) {
							Rectangle r = new Rectangle(p.X + i * size * 2, p.Y, size, size);
							e.Cache.Paint.FillRectangle(e.Graphics, brush, r);
						}
						break;
					case ButtonPredefines.OK:
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 0, p.Y + 2, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 1, p.Y + 3, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 2, p.Y + 4, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 3, p.Y + 3, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 4, p.Y + 2, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 5, p.Y + 1, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 6, p.Y + 0, 1, 3);
						break;
					case ButtonPredefines.Clear:
					case ButtonPredefines.Close:
						SolidBrush sb = brush as SolidBrush;
						Color color = SystemColors.ControlText;
						if(sb != null) color = sb.Color;
						Pen pen = e.Cache.GetPen(color);
						Point p1, p2;
						p1 = new Point(p.X + 1, p.Y + 2);
						p2 = new Point(rect.Right - 2 - 2, rect.Bottom - 2 - 2);
						e.Cache.Paint.DrawLine(e.Graphics, pen, p1, p2);
						p1.Offset(1, 0); p2.Offset(1, 0);
						e.Cache.Paint.DrawLine(e.Graphics, pen, p1, p2);
						p1 = new Point(rect.Right - 2 - 2, p.Y + 2);
						p2 = new Point(rect.X + 1, rect.Bottom - 2 - 2);
						e.Cache.Paint.DrawLine(e.Graphics, pen, p1, p2);
						p1.Offset(1, 0); p2.Offset(1, 0);
						e.Cache.Paint.DrawLine(e.Graphics, pen, p1, p2);
						break;
					case ButtonPredefines.Delete:
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 0, p.Y + 0, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 6, p.Y + 0, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 1, p.Y + 1, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 5, p.Y + 1, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 2, p.Y + 2, 4, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 3, p.Y + 3, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 2, p.Y + 4, 4, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 1, p.Y + 5, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 5, p.Y + 5, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 0, p.Y + 6, 2, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 6, p.Y + 6, 2, 1);
						break;
					case ButtonPredefines.Undo:
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 0, p.Y + 2, 7, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 1, p.Y + 1, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 2, p.Y + 0, 1, 5);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 7, p.Y + 3, 1, 4);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 3, p.Y + 7, 4, 1);
						break;
					case ButtonPredefines.Redo:
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 1, p.Y + 2, 7, 1);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 6, p.Y + 1, 1, 3);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 5, p.Y + 0, 1, 5);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 0, p.Y + 3, 1, 4);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, p.X + 1, p.Y + 7, 4, 1);
						break;					
					case ButtonPredefines.Minus:
						e.Cache.Paint.FillRectangle(e.Graphics, brush, rect.X, rect.Y, 6, 2);
						break;
					case ButtonPredefines.Plus:
						e.Cache.Paint.FillRectangle(e.Graphics, brush, rect.X, rect.Y + 2, 6, 2);
						e.Cache.Paint.FillRectangle(e.Graphics, brush, rect.X + 2, rect.Y, 2, 6);
						break;
					case ButtonPredefines.Search:						
						Bitmap bmp = ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraEditors.Images.searchButton.png", typeof(RepositoryItem).Assembly);
						Rectangle src = new Rectangle(Point.Empty, bmp.Size);
						e.Paint.DrawImage(e.Graphics, bmp, rect, src, e.State != ObjectState.Disabled);
						break;
					default:
						break;
				}
			}
		}
		public virtual Size CalcKindMinSize(EditorButtonObjectInfoArgs e) {
			Size size = CalcKindSize(e);
			if(ButtonPainter is UltraFlatButtonObjectPainter) {
				if(!size.IsEmpty && size.Width < 8) size.Width = 8;
			}
			else {
				if(!size.IsEmpty && size.Width < 10) size.Width = 10;
			}
			return size;
		}
		public virtual Size CalcKindSize(EditorButtonObjectInfoArgs e) {
			if(ButtonPainter is UltraFlatButtonObjectPainter) {
				switch(e.Button.Kind) {
					case ButtonPredefines.Ellipsis: return new Size(8, 2);
				}
			}
			return CalcKindSizeCore(e);
		}
		protected virtual Size CalcDropDownArrowSize(EditorButtonObjectInfoArgs e) {
			if(e.DrawDropDownArrow)
				return new Size(7, 4);
			return Size.Empty;
		}
		protected virtual Size CalcKindSizeCore(EditorButtonObjectInfoArgs e) {
			Size size = Size.Empty;
			switch(e.Button.Kind) {
				case ButtonPredefines.Clear:
				case ButtonPredefines.Search: size = new Size(10, 10); break;
				case ButtonPredefines.Combo: size = new Size(7, 4); break;
				case ButtonPredefines.Down:
				case ButtonPredefines.Up: size = new Size(7, 4); break;
				case ButtonPredefines.SpinDown:
				case ButtonPredefines.SpinUp: size = new Size(5, 3); break;
				case ButtonPredefines.Left:
				case ButtonPredefines.Right: size = new Size(4, 7); break;
				case ButtonPredefines.SpinLeft:
				case ButtonPredefines.SpinRight: size = new Size(3, 5); break;
				case ButtonPredefines.Delete: size = new Size(8, 7); break;
				case ButtonPredefines.Close: size = new Size(11, 11); break;
				case ButtonPredefines.Ellipsis: size = new Size(10, 2); break;
				case ButtonPredefines.Minus: size = new Size(7, 2); break;
				case ButtonPredefines.OK: size = new Size(7, 7); break;
				case ButtonPredefines.Plus: size = new Size(6, 6); break;
				case ButtonPredefines.Undo:
				case ButtonPredefines.Redo: size = new Size(8, 8); break;
				case ButtonPredefines.Glyph:
					if(e.IsImageExists)
						size = e.ImageSize;
					else
						size = Size.Empty;
					break;
				case ButtonPredefines.DropDown:
					size = new Size(7, 4);
					break;
			}
			return size;
		}
		protected virtual bool CanDrawCaption(EditorButtonObjectInfoArgs e) {
			if(e.Button.Caption == null) return false;
			return (e.Button.Caption.Length > 0 &&
				   (int)e.Button.Kind >= 0 &&
				   (e.Button.ImageLocation != ImageLocation.MiddleCenter ||
					e.Button.ImageToTextAlignment != ImageAlignToText.None || 
					(e.Button.Image == null && e.Button.Kind == ButtonPredefines.Glyph))
				);
		}
		protected virtual Size CalcCaptionSize(EditorButtonObjectInfoArgs e) {
			Size res = Size.Empty;
			e.StringInfo = null;
			if(!CanDrawCaption(e)) return res;
			if(e.AllowHtmlDraw) {
				Color hyperlinkColor = e.LookAndFeel != null? EditorsSkins.GetSkin(e.LookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor): Color.Blue;
				e.StringInfo = CalcHtmlStringInfo(e);
				return e.StringInfo.Bounds.Size;
			}
			res = e.Appearance.CalcTextSize(e.Graphics, e.Button.Caption, e.MaxTextWidth).ToSize();
			res.Width += 1;
			res.Height += 1;
			return res;
		}
		protected virtual StringInfo CalcHtmlStringInfo(EditorButtonObjectInfoArgs e) {
			Color prev = e.Appearance.ForeColor;
			try {
				e.Appearance.ForeColor = GetForeColor(e);
				return StringPainter.Default.Calculate(e.Graphics, e.Appearance, e.Appearance.GetTextOptions(), e.Button.Caption, 0, null, e.ContextHtml);
			}
			finally { e.Appearance.ForeColor = prev; }
		}
	}
	public abstract class SkinTabButtonPainter : SkinEditorButtonPainter {
		public SkinTabButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			this.info.Bounds = ee.Bounds;
			this.info.Element = GetSkinElement(ee, ee.Button.Kind);
			this.info.State = ee.State;
			this.info.Cache = ee.Cache;
			this.info.ImageIndex = CalcImageIndex(ee.Button.Kind, ee.State);
			this.info.GlyphIndex = CalcGlyphIndex(ee.Button.Kind, ee.State);
		}
		protected override bool IsCustomContent(ButtonPredefines kind) {
			switch(kind) {
				case ButtonPredefines.Close:
				case ButtonPredefines.Left:
				case ButtonPredefines.Right:
				case ButtonPredefines.Up:
				case ButtonPredefines.Down:
					return false;
			}
			return true;
		}
		protected override int CalcImageIndex(ButtonPredefines kind, ObjectState state) {
			return base.CalcImageIndexCore(state);
		}
		protected virtual int CalcGlyphIndex(ButtonPredefines kind, ObjectState state) {
			int glyphNum = CalcGlyphNum(kind);
			int glyphState = CalcGlyphState(state);
			return glyphNum * 4 + glyphState;
		}
		protected int CalcGlyphNum(ButtonPredefines kind) {
			if(kind == ButtonPredefines.Left) return 1;
			if(kind == ButtonPredefines.Right) return 2;
			if(kind == ButtonPredefines.Up) return 3;
			if(kind == ButtonPredefines.Down) return 4;
			return 0;
		}
		protected int CalcGlyphState(ObjectState state) {
			ObjectState tempState = state & (~ObjectState.Selected);
			if(tempState == ObjectState.Disabled) return 3;
			if((tempState & ObjectState.Pressed) != 0) return 2;
			if((tempState & ObjectState.Hot) != 0) return 1;
			return 0;
		}
		public Point GetOffset(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			return GetSkinElement(ee, ee.Button.Kind).Offset.Offset;
		}
	}
	public class SkinTabPageButtonPainter : SkinTabButtonPainter {
		public SkinTabPageButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, ButtonPredefines kind) {
			return TabSkins.GetSkin(Provider)[TabSkins.SkinTabPageButton];
		}
	}
	public class SkinTabHeaderButtonPainter : SkinTabButtonPainter {
		public SkinTabHeaderButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, ButtonPredefines kind) {
			return TabSkins.GetSkin(Provider)[TabSkins.SkinTabHeaderButton];
		}
	}
	public class SkinTabCustomHeaderButtonPainter : SkinTabHeaderButtonPainter {
		public SkinTabCustomHeaderButtonPainter(ISkinProvider provider, ButtonPredefines kind)
			: base(provider) {
			if(IsCustomContent(kind))
				fButtonPainter = new CustomPainter();
		}
		class CustomPainter : SkinElementPainter {
			protected override void DrawSkinForeground(SkinElementInfo ee) {
			}
		}
	}
	public class SkinEditorButtonPainter : EditorButtonPainter {
		protected SkinElementInfo info;
		static AppearanceObject transparentAppearance;
		static AppearanceObject TransparentAppearance {
			get {
				if(transparentAppearance != null) return transparentAppearance;
				transparentAppearance = new AppearanceObject();
				transparentAppearance.BackColor = Color.Transparent;
				return transparentAppearance;
			}
		}
		ISkinProvider provider;
		public SkinEditorButtonPainter(ISkinProvider provider)
			: base(null) {
			this.provider = provider;
			this.fButtonPainter = CreatePainter();
			this.info = CreateSkinElementInfo();
		}
		public bool IsSingleLineBorder {
			get { return EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinEditorButton].Properties.GetBoolean(EditorsSkins.OptIsSingleLineBorder); }
		}
		protected virtual ObjectPainter CreatePainter() {
			return SkinElementPainter.Default;
		}
		protected virtual SkinElementInfo CreateSkinElementInfo() {
			return new SkinElementInfo(null);
		}
		public ISkinProvider Provider { get { return provider; } }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			UpdateButtonInfo(e);
			return e.Bounds;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			UpdateButtonInfo(e);
			if(!IsCustomContent(ee.Button.Kind)) { 
				Rectangle r = ButtonPainter.CalcObjectMinBounds(info);
				if(ee.Button.Width > 0) {
					int w = base.CalcObjectMinBounds(e).Width;
					r.Width = Math.Max(r.Width, w);
				}
				if(IsSingleLineBorder && ee.IsStandalone) r.Width++;
				return r;
			}
			Rectangle res = base.CalcObjectMinBounds(e);
			if(IsSingleLineBorder && ee.IsStandalone) res.Width++;
			return res;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if(!IsTextOnlyDrawing && ee.FillBackground) {
				SkinElement element = GetSkinElement(ee, ee.Button.Kind);
				if(element == null || element.Color.GetBackColor() == Color.Empty)
					e.Cache.Paint.FillRectangle(e.Graphics, ee.BackAppearance.GetBackBrush(e.Cache), ee.Bounds);
			}
			base.DrawObject(e);
		}
		protected override void DrawContent(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if(!IsCustomContent(ee.Button.Kind)) { 
				return;
			}
			base.DrawContent(e);
		}
		protected virtual bool IsCustomContent(ButtonPredefines kind) {
			switch(kind) {
				case ButtonPredefines.Close:
				case ButtonPredefines.Combo:
				case ButtonPredefines.SpinDown:
				case ButtonPredefines.SpinUp:
				case ButtonPredefines.SpinLeft:
				case ButtonPredefines.SpinRight:
				case ButtonPredefines.Search:
				case ButtonPredefines.Clear:
					return false;
			}
			return true;
		}
		Skin GetButtonSkin() {
			return CommonSkins.GetSkin(Provider);
		}
		protected SkinElement GetButtonSkinElement() {
			return GetButtonSkin()[CommonSkins.SkinButton];
		}
		protected Color GetButtonSkinColor(string color) {
			return GetButtonSkinElement().Properties.GetColor(color);
		}
		protected internal override Color GetForeColor(EditorButtonObjectInfoArgs e) {
			UpdateButtonInfo(e);
			Color color = this.info.Element.Color.GetForeColor();
			if((e.State & ObjectState.Disabled) != 0 && (e.Appearance.ForeColor == color || e.Appearance.ForeColor == SystemColors.ControlText)) {
				if(e.BuiltIn)
					color = EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinEditorButton].Properties.GetColor(CommonColors.DisabledText);
				else
					color = GetButtonSkinColor(CommonColors.DisabledText);
			}
			else {
				if(e.Appearance.ForeColor != SystemColors.ControlText) 
					return base.GetForeColor(e);
			}
			if(color != Color.Empty) return color;
			return base.GetForeColor(e);
		}
		protected virtual SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, ButtonPredefines kind) {
			string name = string.Empty;
			switch(kind) {
				case ButtonPredefines.Combo: name = EditorsSkins.SkinComboButton; break;
				case ButtonPredefines.Close: name = EditorsSkins.SkinCloseButton; break;
				case ButtonPredefines.SpinDown: name = EditorsSkins.SkinSpinDown; break;
				case ButtonPredefines.SpinUp: name = EditorsSkins.SkinSpinUp; break;
				case ButtonPredefines.SpinLeft: name = EditorsSkins.SkinSpinLeft; break;
				case ButtonPredefines.SpinRight: name = EditorsSkins.SkinSpinRight; break;
				case ButtonPredefines.Search: name = EditorsSkins.SkinSearchButton; break;
				case ButtonPredefines.Clear: name = EditorsSkins.SkinClearButton; break;
				default:
					if(e is NavigatorEditorButtonInfoArgs) {
						SkinElement res = EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinNavigatorButton];
						if(res != null) return res;
					}
					if(e.BuiltIn) return EditorsSkins.GetSkin(Provider)[EditorsSkins.SkinEditorButton];
					return CommonSkins.GetSkin(Provider)[CommonSkins.SkinButton];
			}
			return EditorsSkins.GetSkin(Provider)[name];
		}
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if(!ee.BuiltIn) {
				ee.Transparent = true;
				ee.FillBackground = false;
			}
			this.info.Bounds = ee.Bounds;
			this.info.Element = GetSkinElement(ee, ee.Button.Kind);
			this.info.State = ee.State;
			this.info.Cache = ee.Cache;
			this.info.ImageIndex = CalcImageIndex(ee.Button.Kind, ee.State);
			this.info.RightToLeft = ee.RightToLeft;
		}
		protected virtual int CalcImageIndex(ButtonPredefines kind, ObjectState state) {
			switch(kind) {
				case ButtonPredefines.Combo:
				case ButtonPredefines.Close:
					return -1;
			}
			return CalcImageIndexCore(state);
		}
		protected virtual int CalcImageIndexCore(ObjectState state) {
			ObjectState tempState = state & (~ObjectState.Selected);
			if(tempState == ObjectState.Disabled) return 3;
			if((tempState & ObjectState.Pressed) != 0) return 2; 
			if((tempState & ObjectState.Hot) != 0) return 1;
			if((state & ObjectState.Selected) != 0) {
				if(info.Element == null || info.Element.Image == null || info.Element.Image.ImageCount < 5) return 0;
				return 4;
			}
			return 0;
		}
		protected override ObjectInfoArgs GetButtonInfoArgs(ObjectInfoArgs e) { return info; }
		protected override void DrawFocusRect(EditorButtonObjectInfoArgs e, Rectangle rect) {
			e.Paint.DrawFocusRectangle(e.Graphics, rect, GetForeColor(e), Color.Empty);
		}
		protected override Size CalcDropDownArrowSize(EditorButtonObjectInfoArgs e) {
			if(!e.DrawDropDownArrow) 
				return base.CalcDropDownArrowSize(e);
			return CalcDropDownArrowSizeCore(e);
		}
		protected SkinGlyph GetDropDownArrowGlyph() {
			SkinElement element = CommonSkins.GetSkin(Provider)[CommonSkins.SkinDropDownButton2];
			return element.Glyph;
		}
		protected Size CalcDropDownArrowSizeCore(EditorButtonObjectInfoArgs ee) {
			SkinGlyph glyph = GetDropDownArrowGlyph();			
			if(glyph != null)
				return glyph.GetImageBounds(CalcDropDownArrowGlyphIndex(ee.State)).Size;
			return Size.Empty;
		}
		protected override void DrawDropDownArrow(EditorButtonObjectInfoArgs ee, Rectangle dropDownArrowRect) {
			SkinGlyph glyph = GetDropDownArrowGlyph();
			if(glyph == null)
				return;
			ee.Graphics.DrawImage(glyph.GetImages().Images[CalcDropDownArrowGlyphIndex(ee.State)], dropDownArrowRect);
		}
		protected int CalcDropDownArrowGlyphIndex(ObjectState state) {
			ObjectState result = state & (~ObjectState.Selected);
			if(result == ObjectState.Disabled) return 3;
			if((result & ObjectState.Pressed) != 0) return 2; 
			if((result & ObjectState.Hot) != 0) return 1;
			return 0;
		}
	}
	internal class WindowsXPEditorButtonPainterEx : WindowsXPEditorButtonPainter {
		protected override ObjectPainter CreateButtonPainter() {
			return new XPAdvButtonPainterMH();
		}
	}
	class Windows2012ServerButtonPainter : XPAdvButtonPainterMH {
		protected override Size MinSize { get { return new Size(16, 16); } }
	}
	public class WindowsXPEditorButtonPainter : EditorButtonPainter {
		XPAdvButtonInfoArgs infoArgs;
		public WindowsXPEditorButtonPainter()
			: base(null) {
			this.fButtonPainter = CreateButtonPainter();
			this.infoArgs = CreateInfoArgs();
		}
		protected virtual ObjectPainter CreateButtonPainter() {
			if(DevExpress.Utils.Drawing.Helpers.NativeVista.IsWindows2012Server)
				return new Windows2012ServerButtonPainter();
			return new XPAdvButtonPainter();
		}
		protected virtual XPAdvButtonInfoArgs CreateInfoArgs() {
			return new XPAdvButtonInfoArgs(ButtonPredefines.Glyph);
		}
		public new XPAdvButtonPainter ButtonPainter { get { return base.ButtonPainter as XPAdvButtonPainter; } }
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			infoArgs.Button = ee.Button.Kind;
			infoArgs.State = ee.State;
			infoArgs.Bounds = ee.Bounds;
			infoArgs.Cache = ee.Cache;
		}
		protected override Rectangle CalcMinBestSize(ObjectInfoArgs e) {
			Size res = new Size(10, 8);
			Rectangle rect, saveBounds = e.Bounds;
			e.Bounds = new Rectangle(Point.Empty, res);
			UpdateButtonInfo(e);
			rect = CalcBoundsByClientRectangle(e);
			e.Bounds = saveBounds;
			return rect;
		}
		protected internal override Color GetForeColor(EditorButtonObjectInfoArgs e) {
			UpdateButtonInfo(e);
			if(e.Appearance.ForeColor != SystemColors.ControlText) return base.GetForeColor(e);
			return ButtonPainter.GetForeColor(InfoArgs);
		}
		protected override ObjectInfoArgs GetButtonInfoArgs(ObjectInfoArgs e) { return InfoArgs; }
		protected virtual XPAdvButtonInfoArgs InfoArgs { get { return infoArgs; } }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			UpdateButtonInfo(e);
			if(!ButtonPainter.IsCustomContentButton(InfoArgs)) { 
				Rectangle r = ButtonPainter.CalcObjectMinBounds(InfoArgs);
				if(ee.Button.Width > 0) {
					int w = base.CalcObjectMinBounds(e).Width;
					r.Width = Math.Max(r.Width, w);
				}
				return r;
			}
			return base.CalcObjectMinBounds(e);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if(ee.FillBackground) e.Cache.Paint.FillRectangle(e.Graphics, ee.BackAppearance.GetBackBrush(e.Cache), ee.Bounds);
			base.DrawObject(e);
		}
		protected override void DrawContent(ObjectInfoArgs e) {
			if(!ButtonPainter.IsCustomContentButton(InfoArgs)) { 
				return;
			}
			base.DrawContent(e);
		}
	}
	public class EditorButtonObjectInfoArgs : StyleObjectInfoArgs {
		static AppearanceDefault defAppearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.Control, Color.Empty, HorzAlignment.Center, VertAlignment.Center);
		bool fillBackground, drawFocusRectangle;
		EditorButtonObjectInfoArgs parentButtonInfo;
		EditorButton button;
		Size indent;
		bool showHotKeyPrefix, builtIn = true, transparent = false, allowHtmlDraw = false, showFocusRectangle = true;
		bool isStandalone = false;
		public EditorButtonObjectInfoArgs(EditorButton button, AppearanceObject backStyle, bool isStandalone) : this(button, backStyle) {
			this.isStandalone = isStandalone;
		}
		public EditorButtonObjectInfoArgs(EditorButton button, AppearanceObject backStyle) : this(null, button, backStyle) { }
		public EditorButtonObjectInfoArgs(GraphicsCache cache, EditorButton button, AppearanceObject backStyle)
			: base(cache, Rectangle.Empty, null, backStyle, ObjectState.Normal) {
			this.indent = Size.Empty;
			this.showHotKeyPrefix = true;
			this.fillBackground = false;
			this.drawFocusRectangle = false;
			this.button = button;
			this.parentButtonInfo = null;
			this.MaxTextWidth = 0;
			UpdateButtonAppearance();
		}
		internal object ContextHtml { get; set; }
		protected internal int MaxTextWidth { get; set; }
		internal DevExpress.Utils.Text.StringInfo StringInfo { get; set; }
		internal bool DrawDropDownArrow { get; set; }
		public bool AllowHtmlDraw { get { return allowHtmlDraw; } set { allowHtmlDraw = value; } }
		public bool ShowFocusRectangle { get { return showFocusRectangle; } set { showFocusRectangle = value; } }
		public bool AllowGlyphSkinning { get; set; }
		public bool GetAllowGlyphSkinning() { 
			return AllowGlyphSkinning && State != ObjectState.Disabled; 
		}
		public bool IsStandalone { get { return isStandalone; } }
		public bool BuiltIn { get { return builtIn; } set { builtIn = value; } }
		public void UpdateButtonAppearance() {
			SetAppearance(new AppearanceObject(button.Appearance, DefaultAppearance));
			Appearance.TextOptions.RightToLeft = RightToLeft;
		}
		public AppearanceDefault DefaultAppearance { get { return defAppearance; } }
		protected virtual EditorButtonObjectInfoArgs CreateInstance() {
			return new EditorButtonObjectInfoArgs(button, null);
		}
		protected internal virtual EditorButtonObjectInfoArgs Clone() {
			EditorButtonObjectInfoArgs info = CreateInstance();
			info.Assign(this);
			return info;
		}
		public UserLookAndFeel LookAndFeel { get; set; }
		public Padding Padding { get; set; }
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			EditorButtonObjectInfoArgs bInfo = info as EditorButtonObjectInfoArgs;
			if(bInfo == null) return;
			this.fillBackground = bInfo.fillBackground;
			this.drawFocusRectangle = bInfo.drawFocusRectangle;
			this.button = bInfo.button;
			this.indent = bInfo.indent;
			this.showHotKeyPrefix = bInfo.showHotKeyPrefix;
		}
		public virtual bool Transparent { get { return transparent; } set { transparent = value; } }
		public virtual Size Indent { get { return indent; } set { indent = value; } }
		public virtual bool FillBackground { get { return fillBackground; } set { fillBackground = value; } }
		public virtual bool ShowHotKeyPrefix { get { return showHotKeyPrefix; } set { showHotKeyPrefix = value; } }
		public virtual bool DrawFocusRectangle { get { return drawFocusRectangle; } set { drawFocusRectangle = value; } }
		public virtual EditorButton Button { get { return button; } }
		public EditorButtonObjectInfoArgs ParentButtonInfo { get { return parentButtonInfo; } set { parentButtonInfo = value; } }
		public virtual bool Compare(EditorButtonObjectInfoArgs info) {
			if(info == null) return false;
			if(!this.GetType().Equals(info.GetType())) return false;
			return Bounds == info.Bounds && State == info.State;
		}
		public virtual bool IsImageExists {
			get {
				if(Button.Kind != ButtonPredefines.Glyph) return false;
				if(Button.Image != null) return true;
				return ImageCollection.IsImageListImageExists(Button.ImageList, Button.ImageIndex);
			}
		}
		public virtual bool IsCustomWidth {
			get {
				if(Button.Kind == ButtonPredefines.Glyph) return true;
				return false;
			}
		}
		public virtual Size ImageSize {
			get {
				if(!IsImageExists) return Size.Empty;
				if(Button.Image != null) return Button.Image.Size;
				return ImageCollection.GetImageListSize(Button.ImageList);
			}
		}
		public Image ActualImage { get { return Button.ActualImage; } }
		public int ImageIndex { get { return Button.ImageIndex; } }
		public object ImageList { get { return Button.ImageList; } }
	}
	public class GridSmartFlatFilterButtonPainter : ObjectPainter {
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return new Rectangle(Point.Empty, Image.Size);
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			GridFilterButtonInfoArgs ee = e as GridFilterButtonInfoArgs;
			Size size = CalcObjectMinBounds(e).Size;
			Rectangle bounds = new Rectangle(ee.Bounds.Right - size.Width, ee.Bounds.Y, size.Width, size.Height);
			bounds.Offset(-2, 2);
			ee.Bounds = bounds;
			return ee.Bounds;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GridFilterButtonInfoArgs ee = e as GridFilterButtonInfoArgs;
			bool isRegularState = ee.ParentObject.State == ObjectState.Normal && ee.State == ObjectState.Normal;
			if(isRegularState && !ee.Filtered) return;
			DrawSmartTag(ee);
		}
		[ThreadStatic]
		static Image image;
		static Image Image {
			get {
				if(image == null) {
					image = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Images.FilterButton.png", typeof(GridSmartFlatFilterButtonPainter).Assembly);
					((Bitmap)image).MakeTransparent(Color.Black);
				}
				return image;
			}
		}
		protected virtual Color GetColor(GridFilterButtonInfoArgs e) {
			return e.Appearance.ForeColor;
		}
		protected virtual float GetTransparency(GridFilterButtonInfoArgs e) {
			float transp = 0.4f;
			switch(e.State) {
				case ObjectState.Hot: transp = 0.6f; break;
				case ObjectState.Pressed: transp = 0.8f; break;
			}
			if(!e.Filtered) transp += 0.1f;
			return transp;
		}
		protected virtual void DrawSmartTag(GridFilterButtonInfoArgs e) {
			Color c = GetColor(e);
			float transp = GetTransparency(e);
			float[][] array = new float[5][];
			array[0] = new float[5] { -c.R, 0, 0, 0, 0 };
			array[1] = new float[5] { 0, -c.G, 0, 0, 0 };
			array[2] = new float[5] { 0, 0, -c.B, 0, 0 };
			array[3] = new float[5] { 0, 0, 0, transp, 0 };
			array[4] = new float[5] { 0.0f, 0f, 0f, 0, 1 };
			ColorMatrix grayMatrix = new ColorMatrix(array);
			ImageAttributes attr = new ImageAttributes();
			attr.ClearColorKey();
			attr.SetColorMatrix(grayMatrix);
			Rectangle r = e.Bounds;
			r.Size = Image.Size;
			e.Cache.Paint.DrawImage(e.Graphics, Image, r, 0, 0, r.Width, r.Height, GraphicsUnit.Pixel, attr);
			attr.Dispose();
		}
	}
	public class GridSmartOffice2003FilterButtonPainter : GridSmartFlatFilterButtonPainter {
		protected override Color GetColor(GridFilterButtonInfoArgs e) {
			if(e.Filtered) return Office2003Colors.Default[Office2003Color.Text];
			return Office2003Colors.Default[Office2003Color.Header2];
		}
	}
	public class GridSmartSkinFilterButtonPainter : ObjectPainter {
		ISkinProvider provider;
		public GridSmartSkinFilterButtonPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return SkinElementPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, new SkinElementInfo(GetSkin()));
		}
		protected SkinElement GetSkin() {
			return SkinManager.Default.GetSkin(SkinProductId.Grid, this.provider)[GridSkins.SkinSmartFilterButton];
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			GridFilterButtonInfoArgs ee = e as GridFilterButtonInfoArgs;
			Size size = CalcObjectMinBounds(e).Size;
			Rectangle bounds = new Rectangle(ee.Bounds.Right - size.Width, ee.Bounds.Y, size.Width, size.Height);
			int delta = 1;
			if(ee.RightToLeft) {
				delta = -1;
				bounds.X = ee.Bounds.X;
			}
			SkinElement el = GetSkin();
			bounds.Offset(el.Properties.GetInteger(GridSkins.OptFilterButtonOffsetX) * delta, el.Properties.GetInteger(GridSkins.OptFilterButtonOffsetY));
			ee.Bounds = bounds;
			return ee.Bounds;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			GridFilterButtonInfoArgs ee = e as GridFilterButtonInfoArgs;
			if(ee.ParentObject.State == ObjectState.Normal && !ee.Filtered && ee.State == ObjectState.Normal) return;
			SkinElementInfo info = new SkinElementInfo(GetSkin(), e.Bounds);
			info.RightToLeft = ee.RightToLeft;
			info.State = e.State;
			info.ImageIndex = -1;
			if(info.State == ObjectState.Normal) {
				if(ee.Filtered) {
					info.ImageIndex = ee.ParentObject.State != ObjectState.Normal ? 3 : 4;
				}
				else
					info.ImageIndex = 0;
			}
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
	}
	public class GridFilterButtonInfoArgs : EditorButtonObjectInfoArgs, ISupportObjectInfo {
		bool filtered = false;
		ObjectInfoArgs parentObject = null;
		public GridFilterButtonInfoArgs() : base(new EditorButton(ButtonPredefines.Combo), null) { }
		public bool Filtered { get { return filtered; } set { filtered = value; } }
		public virtual ObjectInfoArgs ParentObject { get { return parentObject; } set { parentObject = value; } }
	}
	public class SkinCheckButtonPainter : SkinEditorButtonPainter {
		public SkinCheckButtonPainter(ISkinProvider provider) : base(provider) { }
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			base.UpdateButtonInfo(e);
			EditorButtonObjectInfoArgs ee = e as EditorButtonObjectInfoArgs;
			if((ee.State & ObjectState.Pressed) != 0) info.ImageIndex = 5;
		}
		protected internal override Color GetForeColor(EditorButtonObjectInfoArgs e) {
			if(!e.BuiltIn) {
				if((e.State & ObjectState.Pressed) != 0) {
					var color =  GetButtonSkinElement().GetForeColor(e.State);
					if(e.Appearance.ForeColor == color || e.Appearance.ForeColor == SystemColors.ControlText) {
						Color pressedColor = GetButtonSkinColor(CommonSkins.SkinCheckButtonPressedForeColor);
						if(pressedColor != Color.Empty)
							return pressedColor;
					}
				}
			}
			return base.GetForeColor(e);
		}
	}
	public class GridFilterButtonPainter : ObjectPainter {
		EditorButtonPainter buttonPainter;
		public GridFilterButtonPainter(EditorButtonPainter buttonPainter) {
			this.buttonPainter = buttonPainter;
		}
		public EditorButtonPainter ButtonPainter { get { return buttonPainter; } }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle r = ButtonPainter.CalcObjectMinBounds(e);
			r.Height = Math.Max(r.Width - 2, r.Height);
			if(IsXPPainter) r.Height += 2;
			return r;
		}
		protected virtual bool IsXPPainter { get { return ButtonPainter is WindowsXPEditorButtonPainter; } }
		public override void DrawObject(ObjectInfoArgs e) {
			GridFilterButtonInfoArgs ee = e as GridFilterButtonInfoArgs;
			Rectangle r = e.Bounds;
			if(IsXPPainter) e.Bounds = Rectangle.Inflate(r, 0, -1);
			try {
				if(!ee.Filtered && IsXPPainter && e.State == ObjectState.Normal) e.State = ObjectState.Disabled;
				ButtonPainter.DrawObject(e);
			}
			finally {
				e.Bounds = r;
			}
		}
	}
	public class SkinGridFilterButtonPainter : GridFilterButtonPainter {
		class SkinFilterEditorButtonPainter : SkinEditorButtonPainter {
			public SkinFilterEditorButtonPainter(ISkinProvider provider) : base(provider) { }
			protected override bool IsCustomContent(ButtonPredefines kind) { return false; }
			protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, ButtonPredefines kind) {
				GridFilterButtonInfoArgs filter = (GridFilterButtonInfoArgs)e;
				return GridSkins.GetSkin(Provider)[filter.Filtered ? GridSkins.SkinFilterButtonActive : GridSkins.SkinFilterButton];
			}
		}
		public SkinGridFilterButtonPainter(ISkinProvider provider)
			: base(new SkinFilterEditorButtonPainter(provider)) {
		}
	}
	public class Office2003GridFilterButtonPainter : GridFilterButtonPainter {
		public Office2003GridFilterButtonPainter() : base(EditorButtonHelper.GetPainter(BorderStyles.Office2003)) { }
		public override void DrawObject(ObjectInfoArgs e) {
			base.DrawObject(e);
			Rectangle r = e.Bounds;
			if(e.State == ObjectState.Normal) {
				e.Paint.DrawRectangle(e.Graphics, SystemPens.Highlight, r);
			}
		}
	}
	public class FilterButtonHelper {
		public static ObjectPainter GetPainter(BorderStyles style) { return GetPainter(style, null); }
		public static ObjectPainter GetPainter(UserLookAndFeel lookAndFeel) { return GetPainter(BorderStyles.Default, lookAndFeel); }
		public static ObjectPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel) {
			if(style == BorderStyles.Default) {
				if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
				ActiveLookAndFeelStyle lf = lookAndFeel.ActiveStyle;
				switch(lf) {
					case ActiveLookAndFeelStyle.Office2003: return new Office2003GridFilterButtonPainter();
					case ActiveLookAndFeelStyle.Skin: return new SkinGridFilterButtonPainter(lookAndFeel);
				}
				return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(style, lookAndFeel));
			}
			if(style == BorderStyles.Office2003) return new Office2003GridFilterButtonPainter();
			return new GridFilterButtonPainter(EditorButtonHelper.GetPainter(style, lookAndFeel));
		}
		public static ObjectPainter GetSmartPainter(UserLookAndFeel lookAndFeel) {
			if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
			ActiveLookAndFeelStyle lf = lookAndFeel.ActiveStyle;
			if(lf == ActiveLookAndFeelStyle.Skin) return new GridSmartSkinFilterButtonPainter(lookAndFeel);
			if(lf == ActiveLookAndFeelStyle.Office2003) return new GridSmartOffice2003FilterButtonPainter();
			return new GridSmartFlatFilterButtonPainter();
		}
	}
	public class SkinDropDownButtonPainter : SkinEditorButtonPainter {
		public SkinDropDownButtonPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override bool IsCustomContent(ButtonPredefines kind) { return true; }
		protected override SkinElement GetSkinElement(EditorButtonObjectInfoArgs e, ButtonPredefines kind) {
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee != null && !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				return base.GetSkinElement(e, kind);
			}
			if(kind == ButtonPredefines.Glyph) return CommonSkins.GetSkin(Provider)[CommonSkins.SkinDropDownButton1];
			if(kind == ButtonPredefines.DropDown) return CommonSkins.GetSkin(Provider)[CommonSkins.SkinDropDownButton2];
			return null;
		}
		protected override void DrawArrow(EditorButtonObjectInfoArgs e, Rectangle rect) {
		}
	}
	public class DropDownButtonPainter : EditorButtonPainter {
		EditorButtonPainter buttonPainter = null;
		public DropDownButtonPainter(EditorButtonPainter buttonPainter)
			: base(buttonPainter) {
			this.buttonPainter = buttonPainter;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return this.ButtonPainter.CalcBoundsByClientRectangle(e, client);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee == null || !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				return base.GetObjectClientRectangle(e);
			}
			return ButtonPainter.GetObjectClientRectangle(e);
		}
		protected override void UpdateButtonInfo(ObjectInfoArgs e) {
			base.UpdateButtonInfo(e);
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee != null && ee.ViewInfo.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				ee.FillBackground = false;
				ee.Transparent = true;
			}
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			Rectangle arrowButton;
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee == null || !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				return base.CalcObjectBounds(e);
			}
			Rectangle baseButton = arrowButton = ee.Bounds;
			int arrowWidth = ee.ViewInfo.ArrowButtonWidth;
			if(ee.ViewInfo.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				int w = ObjectPainter.CalcObjectMinBounds(ee.Graphics, SkinElementPainter.Default, new SkinElementInfo(CommonSkins.GetSkin(ee.ViewInfo.OwnerControl.LookAndFeel)[CommonSkins.SkinDropDownButton2])).Width + 2;
				if(w > arrowWidth) arrowWidth = w;
			}
			baseButton.Width -= arrowWidth;
			if(ee.RightToLeft) {
				arrowButton.X = baseButton.X;
				arrowButton.Width = arrowWidth;
				if(CheckBorderStyle(ee)) 
					baseButton.X = arrowButton.Right;
				else 
					baseButton.X = arrowButton.Right - 1;
			}
			else {
				if(CheckBorderStyle(ee)) {
					arrowButton.X = baseButton.Right;
					arrowButton.Width = arrowWidth;
				}
				else {
					arrowButton.X = baseButton.Right - 1;
					arrowButton.Width = arrowWidth + 1;
				}
			}
			ee.BaseButtonInfo.Bounds = baseButton;
			ee.ArrowButtonInfo.Bounds = arrowButton;
			this.ButtonPainter.CalcObjectBounds(ee.BaseButtonInfo);
			this.ButtonPainter.CalcObjectBounds(ee.ArrowButtonInfo);
			return e.Bounds;
		}
		bool CheckBorderStyle(DropDownButtonObjectInfoArgs ee) {
			ActiveLookAndFeelStyle activeStyle = ee.ViewInfo.LookAndFeel.ActiveStyle;
			BorderStyles borderStyle = ee.ViewInfo.OwnerControl.ButtonStyle;
			if(borderStyle == BorderStyles.Default)
				return activeStyle == ActiveLookAndFeelStyle.Flat || activeStyle == ActiveLookAndFeelStyle.Style3D || activeStyle == ActiveLookAndFeelStyle.Skin;
			return borderStyle == BorderStyles.Flat || borderStyle == BorderStyles.Style3D;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee == null || !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				return ButtonPainter.CalcObjectMinBounds(e);
			}
			Size res = Size.Empty;
			Size sizeBase = ButtonPainter.CalcObjectMinBounds(ee.BaseButtonInfo).Size;
			Size sizeArrow = ButtonPainter.CalcObjectMinBounds(ee.ArrowButtonInfo).Size;
			res.Height = Math.Max(sizeBase.Height, sizeArrow.Height);
			res.Width = (sizeBase.Width + sizeArrow.Width);
			return new Rectangle(Point.Empty, res);
		}
		public new EditorButtonPainter ButtonPainter { get { return this.buttonPainter; } }
		protected override void DrawContent(ObjectInfoArgs e) {
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee == null || !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				base.DrawContent(e);
				return;
			}
			ee.BaseButtonInfo.BackAppearance = ee.ArrowButtonInfo.BackAppearance = ee.BackAppearance;
			this.DrawButton(ee, ee.BaseButtonInfo, -10000);
			this.DrawButton(ee, ee.ArrowButtonInfo, -10001);
			DrawSelectedFrame(ee);
		}
		protected virtual Color GetForeColor(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) return SystemColors.GrayText;
			Color color = GetStyle(e).GetForeColor();
			if(color == Color.Empty) return DefaultAppearance.ForeColor;
			return color;
		}
		protected Brush GetForeBrush(ObjectInfoArgs e) {
			return e.Cache.GetSolidBrush(GetForeColor(e));
		}
		protected virtual void DrawSelectedFrame(DropDownButtonObjectInfoArgs e) {
			if((e.State & ObjectState.Selected) != 0 && e.ViewInfo.IsOwnerDrawnSelectedFrame) {
				Brush brush = GetForeBrush(e);
				Rectangle r = e.Bounds;
				FlatButtonObjectPainter.DrawBounds(e, r, brush, brush);
			}
		}
		protected override void DrawButton(ObjectInfoArgs e) {
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee == null || !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				base.DrawButton(e);
				return;
			}
		}
		protected override void DrawFocusRectangle(ObjectInfoArgs e) {
			DropDownButtonObjectInfoArgs ee = e as DropDownButtonObjectInfoArgs;
			if(ee == null || !ee.ViewInfo.OwnerControl.IsSplitButtonVisible) {
				base.DrawFocusRectangle(e);
				return;
			}
		}
		protected virtual void DrawButton(DropDownButtonObjectInfoArgs ee, EditorButtonObjectInfoArgs info, int buttonId) {
			if(XtraAnimator.Current.DrawFrame(info.Cache, ee.ViewInfo.OwnerControl, buttonId)) {
				EditorButtonPainter eb = ee.ViewInfo.ButtonPainter.ButtonPainter;
				if(eb != null) eb.DrawTextOnly(info);
				return;
			}
			this.ButtonPainter.DrawObject(info);
		}
	}
}
namespace DevExpress.XtraEditors {
	public enum ImageLocation {
		Default = 0,
		TopCenter = 4,
		TopLeft = 5,
		TopRight = 6,
		MiddleCenter = 2,
		MiddleLeft = 1,
		MiddleRight = 3,
		BottomCenter = 7,
		BottomLeft = 8,
		BottomRight = 9
	}
}
