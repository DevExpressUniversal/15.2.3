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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Design;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Utils.Text.Internal;
using System.Drawing.Design;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemCheckEdit : BaseRepositoryItemCheckEdit, IHorzAlignmentProvider {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemCheckEdit Properties { get { return this; } }
		private static readonly object queryCheckStateByValue = new object();
		private static readonly object queryValueByCheckState = new object();
		private static readonly object checkedChanged = new object();
		private static readonly object checkStateChanged = new object();
		private static readonly object hyperlinkClick = new object();
		string displayValueChecked, displayValueUnchecked, displayValueGrayed;
		int radioGroupIndex;
		int imageIndexChecked, imageIndexUnchecked, imageIndexGrayed;
		Image pictureChecked, pictureUnchecked, pictureGrayed;
		object valueChecked, valueUnchecked, valueGrayed;
		object fImages;
		bool allowGrayed, hotTrackWhenReadOnly;
		StyleIndeterminate nullStyle;
		CheckStyles checkStyle;
		public RepositoryItemCheckEdit()
			: base() {
			this.Caption = EditorTypeName;
			this.checkStyle = CheckStyles.Standard;
			this.nullStyle = StyleIndeterminate.InactiveChecked;
			this.displayValueChecked = this.displayValueUnchecked = this.displayValueGrayed = string.Empty;
			this.pictureChecked = this.pictureUnchecked = this.pictureGrayed = null;
			this.valueChecked = true;
			this.valueUnchecked = false;
			this.valueGrayed = null;
			this.allowGrayed = false;
			this.radioGroupIndex = -1;
			this.imageIndexChecked = -1;
			this.imageIndexUnchecked = -1;
			this.imageIndexGrayed = -1;
			this.hotTrackWhenReadOnly = true;
		}
		protected virtual Image GetImageBrick(BaseCheckEditViewInfo cevi, PrintCellHelperInfo info) {
			MultiKey key = new MultiKey(new object[] { 
				cevi.Bounds.Size, 
				info.EditValue, 
				this.AutoHeight, 
				this.BorderStyle, 
				this.Enabled, 
				this.EditorTypeName,
				this.ValueChecked,
				this.ValueGrayed,
				this.ValueUnchecked,
				this.CheckStyle,
				this.PictureChecked,
				this.PictureGrayed,
				this.PictureUnchecked,
				this.Images,
				this.ImageIndexChecked,
				this.ImageIndexUnchecked,
				this.ImageIndexGrayed
			});
			Image img = GetCachedPrintImage(key, info.PS);
			CheckObjectInfoArgs args = cevi.CheckInfo as CheckObjectInfoArgs;
			if(img != null) return img;
			using(BitmapGraphicsHelper gHelper = new BitmapGraphicsHelper(args.GlyphRect.Size.Width, args.GlyphRect.Size.Height)) {
				args.Graphics = gHelper.Graphics;
				args.CheckState = this.GetStateByValue(info.EditValue);
				Rectangle oldRect = args.GlyphRect;
				args.GlyphRect = new Rectangle(Point.Empty, gHelper.Bitmap.Size);
				CheckObjectPainter painter = CheckPainterHelper.GetPainter(DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Flat);
				painter.DrawObject(args);
				args.GlyphRect = oldRect;
				return AddImageToPrintCache(key, gHelper.MemSafeBitmap, info.PS);
			}
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {			
			if(GetExportMode() == ExportMode.DisplayText) {
				return CreateTextBrick(info);
			}
			if(GlyphAlignment == HorzAlignment.Center || GlyphAlignment == HorzAlignment.Default) {
				CheckBoxBrick checkBrick = new XECheckBoxBrick(CreateBrickStyle(info, "check"));
				checkBrick.Value = info.EditValue;
				checkBrick.CheckText = info.DisplayText;
				checkBrick.CheckState = GetStateByValue(info.EditValue);
				return checkBrick;
			}
			CheckBoxTextBrick checkTextBrick = new XECheckBoxTextBrick(CreateBrickStyle(info, "check"));
			checkTextBrick.Text = Caption;
			checkTextBrick.CheckState = GetStateByValue(info.EditValue);
			checkTextBrick.CheckText = info.DisplayText;
			checkTextBrick.CheckBoxAlignment = GlyphAlignment;
			return checkTextBrick;
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemCheckEdit source = item as RepositoryItemCheckEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.Caption = source.Caption;
				this.valueChecked = source.ValueChecked;
				this.valueGrayed = source.ValueGrayed;
				this.valueUnchecked = source.ValueUnchecked;
				this.fImages = source.Images;
				this.pictureChecked = source.PictureChecked;
				this.pictureGrayed = source.PictureGrayed;
				this.pictureUnchecked = source.PictureUnchecked;
				this.displayValueChecked = source.DisplayValueChecked;
				this.displayValueUnchecked = source.DisplayValueUnchecked;
				this.displayValueGrayed = source.DisplayValueGrayed;
				this.radioGroupIndex = source.RadioGroupIndex;
				this.allowGrayed = source.AllowGrayed;
				this.imageIndexChecked = source.ImageIndexChecked;
				this.imageIndexUnchecked = source.ImageIndexUnchecked;
				this.imageIndexGrayed = source.ImageIndexGrayed;
				this.checkStyle = source.CheckStyle;
				this.nullStyle = source.NullStyle;
				this.hotTrackWhenReadOnly = source.HotTrackWhenReadOnly;
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(checkedChanged, source.Events[checkedChanged]);
			Events.AddHandler(checkStateChanged, source.Events[checkStateChanged]);
			Events.AddHandler(queryCheckStateByValue, source.Events[queryCheckStateByValue]);
			Events.AddHandler(queryValueByCheckState, source.Events[queryValueByCheckState]);
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditNullStyle"),
#endif
 DefaultValue(StyleIndeterminate.InactiveChecked)]
		public StyleIndeterminate NullStyle {
			get { return nullStyle; }
			set {
				if(NullStyle == value) return;
				this.nullStyle = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditCheckStyle"),
#endif
 RefreshProperties(RefreshProperties.All), DefaultValue(CheckStyles.Standard), SmartTagProperty("Check Style", "", SmartTagActionType.RefreshAfterExecute)]
		public CheckStyles CheckStyle {
			get { return checkStyle; }
			set {
				if(CheckStyle == value) return;
				checkStyle = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditPictureChecked"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image PictureChecked {
			get { return pictureChecked; }
			set {
				if(PictureChecked == value) return;
				pictureChecked = TransformPicture(value);
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditPictureUnchecked"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image PictureUnchecked {
			get { return pictureUnchecked; }
			set {
				if(PictureUnchecked == value) return;
				pictureUnchecked = TransformPicture(value);
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditPictureGrayed"),
#endif
 DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image PictureGrayed {
			get { return pictureGrayed; }
			set {
				if(PictureGrayed == value) return;
				pictureGrayed = TransformPicture(value);
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditValueChecked"),
#endif
	   DefaultValue(true),
	   Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
	   TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
	   ]
		public object ValueChecked {
			get { return valueChecked; }
			set {
				if(ValueChecked == value) return;
				valueChecked = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditValueUnchecked"),
#endif
		DefaultValue(false),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object ValueUnchecked {
			get { return valueUnchecked; }
			set {
				if(ValueUnchecked == value) return;
				valueUnchecked = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditCaption"),
#endif
 DefaultValue("CheckEdit"), SmartTagProperty("Caption", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public override string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditValueGrayed"),
#endif
		DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object ValueGrayed {
			get { return valueGrayed; }
			set {
				if(ValueGrayed == value) return;
				valueGrayed = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditDisplayValueChecked"),
#endif
 DefaultValue(""), Localizable(true)]
		public string DisplayValueChecked {
			get { return displayValueChecked; }
			set {
				if(value == null) value = "";
				if(DisplayValueChecked == value) return;
				displayValueChecked = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance),  DefaultValue(""), Localizable(true)]
		public string DisplayValueUnchecked {
			get { return displayValueUnchecked; }
			set {
				if(value == null) value = "";
				if(DisplayValueUnchecked == value) return;
				displayValueUnchecked = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance),  DefaultValue(""), Localizable(true)]
		public string DisplayValueGrayed {
			get { return displayValueGrayed; }
			set {
				if(value == null) value = "";
				if(DisplayValueGrayed == value) return;
				displayValueGrayed = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditAllowGrayed"),
#endif
 DefaultValue(false), RefreshProperties(System.ComponentModel.RefreshProperties.All)]
		public bool AllowGrayed {
			get { return allowGrayed; }
			set {
				if(AllowGrayed == value) return;
				this.allowGrayed = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditRadioGroupIndex"),
#endif
 DefaultValue(-1)]
		public int RadioGroupIndex {
			get { return radioGroupIndex; }
			set {
				if(RadioGroupIndex == value) return;
				radioGroupIndex = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditImages"),
#endif
 DefaultValue(null),
		TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))
		]
		public object Images {
			get { return fImages; }
			set {
				if(Images == value) return;
				fImages = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "CheckEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditQueryCheckStateByValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryCheckStateByValueEventHandler QueryCheckStateByValue {
			add { this.Events.AddHandler(queryCheckStateByValue, value); }
			remove { this.Events.RemoveHandler(queryCheckStateByValue, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditQueryValueByCheckState"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryValueByCheckStateEventHandler QueryValueByCheckState {
			add { this.Events.AddHandler(queryValueByCheckState, value); }
			remove { this.Events.RemoveHandler(queryValueByCheckState, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditCheckedChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler CheckedChanged {
			add { this.Events.AddHandler(checkedChanged, value); }
			remove { this.Events.RemoveHandler(checkedChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditCheckStateChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler CheckStateChanged {
			add { this.Events.AddHandler(checkStateChanged, value); }
			remove { this.Events.RemoveHandler(checkStateChanged, value); }
		}
		[ DXCategory(CategoryName.Events)]
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { Events.AddHandler(hyperlinkClick, value); }
			remove { Events.RemoveHandler(hyperlinkClick, value); }
		}
		protected internal void RaiseHyperlinkClick(HyperlinkClickEventArgs e) {
			HyperlinkClickEventHandler handler = Events[hyperlinkClick] as HyperlinkClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void RaiseCheckedChanged(EventArgs e) {
			if(IsLockEvents)
				return;
			EventHandler handler = (EventHandler)this.Events[checkedChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseCheckStateChanged(EventArgs e) {
			if(IsLockEvents)
				return;
			EventHandler handler = (EventHandler)this.Events[checkStateChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseQueryCheckStateByValue(QueryCheckStateByValueEventArgs e) {
			QueryCheckStateByValueEventHandler handler = (QueryCheckStateByValueEventHandler)this.Events[queryCheckStateByValue];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseQueryValueByCheckState(QueryValueByCheckStateEventArgs e) {
			QueryValueByCheckStateEventHandler handler = (QueryValueByCheckStateEventHandler)this.Events[queryValueByCheckState];
			if(handler != null) handler(GetEventSender(), e);
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditImageIndexChecked"),
#endif
 DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), ImageList("Images")]
		public int ImageIndexChecked {
			get { return imageIndexChecked; }
			set {
				if(ImageIndexChecked == value)
					return;
				imageIndexChecked = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditImageIndexUnchecked"),
#endif
 DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), ImageList("Images")]
		public int ImageIndexUnchecked {
			get { return imageIndexUnchecked; }
			set {
				if(ImageIndexUnchecked == value)
					return;
				imageIndexUnchecked = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditImageIndexGrayed"),
#endif
 DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), ImageList("Images")]
		public int ImageIndexGrayed {
			get { return imageIndexGrayed; }
			set {
				if(ImageIndexGrayed == value)
					return;
				imageIndexGrayed = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCheckEditHotTrackWhenReadOnly"),
#endif
 DefaultValue(true)]
		public bool HotTrackWhenReadOnly {
			get {
				return hotTrackWhenReadOnly;
			}
			set {
				if(hotTrackWhenReadOnly == value)
					return;
				hotTrackWhenReadOnly = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public override bool IsRadioButton {
			get {
				if(OwnerEdit != null && OwnerEdit.InplaceType != InplaceType.Grid) {
					return RadioGroupIndex >= 0;
				}
				return false;
			}
		}	  
		protected virtual ImageCollection GetImageCollection() {
			return DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.CheckBoxes.gif", typeof(RepositoryItemCheckEdit).Assembly, new Size(18, 18), Color.Magenta);
		}
		protected override internal ArrayList CheckImages {
			get {
				if(fCheckImages == null) LoadImages(GetImageCollection(), ref fCheckImages);
				return fCheckImages;
			}
		}
		public CheckState GetStateByValue(object val) {
			return GetStateByValue(val, AllowGrayed);
		}
		protected internal virtual CheckState GetStateByValue(object val, bool allowGrayed) {
			QueryCheckStateByValueEventArgs e = new QueryCheckStateByValueEventArgs(val);
			RaiseQueryCheckStateByValue(e);
			if(e.Handled) {
				if(e.CheckState == CheckState.Indeterminate && IsRadioButton) return CheckState.Unchecked;
				return e.CheckState;
			}
			if(IsEquals(ValueChecked, val)) return CheckState.Checked;
			if(IsEquals(ValueUnchecked, val)) return CheckState.Unchecked;
			if(allowGrayed && !IsRadioButton)
				return CheckState.Indeterminate;
			return CheckState.Unchecked;
		}
		public virtual object GetValueByState(CheckState state) {
			QueryValueByCheckStateEventArgs e = new QueryValueByCheckStateEventArgs(state);
			RaiseQueryValueByCheckState(e);
			if(e.Handled) return e.Value;
			if(state == CheckState.Checked) return ValueChecked;
			if(state == CheckState.Indeterminate && AllowGrayed && !IsRadioButton) return ValueGrayed;
			return ValueUnchecked;
		}
		protected override bool LockDefaultImmediateUpdateRowPosition { get { return true; }}
	}
}
namespace DevExpress.XtraEditors {	   
	[DefaultEvent("CheckedChanged"), DXToolboxItem(DXToolboxItemKind.Free), DefaultProperty("Checked"),
	 DefaultBindingPropertyEx("CheckState"),
	 Designer("DevExpress.XtraEditors.Design.CheckEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an associated option to be checked and unchecked."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "CheckEdit")
	]
	public class CheckEdit : BaseCheckEdit {
		public CheckEdit()
			: base() {
		}
		public override string EditorTypeName { get { return "CheckEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),SmartTagSearchNestedProperties]
		public new RepositoryItemCheckEdit Properties { get { return base.Properties as RepositoryItemCheckEdit; } }
		protected internal new CheckEditViewInfo ViewInfo { get { return base.ViewInfo as CheckEditViewInfo; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(false)]
		public override object EditValue {
			get { return base.EditValue; }
			set {
				bool prevChecked = Checked;
				CheckState state = CheckState;
				base.EditValue = value;
				if(IsLoading) return;
				if(prevChecked != Checked) {
					if(Checked && Properties.IsRadioButton) ClearRadioCheck();
					Properties.RaiseCheckedChanged(EventArgs.Empty);
				}
				if(state != CheckState) {
					Properties.RaiseCheckStateChanged(EventArgs.Empty);
					if(IsHandleCreated)
						AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
				}
			}
		}
		protected Cursor PrevCursor { get; set; }
		protected override void TryUpdateCursorByHyperlink(MouseEventArgs e) {
			base.TryUpdateCursorByHyperlink(e);
			if(Properties.GetAllowHtmlDraw() && ViewInfo.CheckInfo.StringInfo != null) {
				StringBlock b = ViewInfo.CheckInfo.StringInfo.GetLinkByPoint(e.Location);
				if(b == null) {
					if(PrevCursor != null)
						Cursor = PrevCursor;
					PrevCursor = null;
				}
				else {
					if(PrevCursor == null) 
						PrevCursor = Cursor;
					Cursor = Cursors.Hand;
				}
			}
		}
		protected override bool TryClickHyperlink(MouseEventArgs e) {
			if(Properties.GetAllowHtmlDraw() && ViewInfo.CheckInfo.StringInfo != null) {
				StringBlock b = ViewInfo.CheckInfo.StringInfo.GetLinkByPoint(e.Location);
				if(b != null) {
					Properties.RaiseHyperlinkClick(new HyperlinkClickEventArgs() { Link = b.Link, Text = b.Text });
					return true;
				}
			}
			return false;
		}
		protected virtual void ClearRadioCheck() {
			if(Parent == null) return;
			if(!Properties.IsRadioButton) return;
			foreach(Control control in Parent.Controls) {
				CheckEdit check = control as CheckEdit;
				if(check == null) continue;
				if(check.Properties.RadioGroupIndex == Properties.RadioGroupIndex) {
					check.TabStop = check == this;
					if(check == this) continue;
					check.Checked = false;
				}
			}
		}
		public override void Toggle() {
			if(Properties.ReadOnly) return;
			if(GetValidationCanceled()) return;
			if(Properties.IsRadioButton) {
				Checked = true;
				return;
			}
			if(CheckState == CheckState.Unchecked) {
				CheckState = CheckState.Checked;
				return;
			}
			if(CheckState == CheckState.Checked) {
				if(Properties.AllowGrayed) {
					CheckState = CheckState.Indeterminate;
					return;
				}
			}
			CheckState = CheckState.Unchecked;
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(Properties.IsRadioButton)
				TabStop = Checked;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditCheckState"),
#endif
 DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All), DefaultValue(CheckState.Unchecked), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), SmartTagProperty("Check State", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual CheckState CheckState {
			get { return Properties.GetStateByValue(EditValue); }
			set { EditValue = Properties.GetValueByState(value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditChecked"),
#endif
 DXCategory(CategoryName.Appearance), RefreshProperties(RefreshProperties.All), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Checked {
			get { return CheckState != CheckState.Unchecked; }
			set { CheckState = (!value ? CheckState.Unchecked : CheckState.Checked); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditQueryCheckStateByValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryCheckStateByValueEventHandler QueryCheckStateByValue {
			add { Properties.QueryCheckStateByValue += value; }
			remove { Properties.QueryCheckStateByValue -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditQueryValueByCheckState"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryValueByCheckStateEventHandler QueryValueByCheckState {
			add { Properties.QueryValueByCheckState += value; }
			remove { Properties.QueryValueByCheckState -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditCheckedChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler CheckedChanged {
			add { Properties.CheckedChanged += value; }
			remove { Properties.CheckedChanged -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckEditCheckStateChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler CheckStateChanged {
			add { Properties.CheckStateChanged += value; }
			remove { Properties.CheckStateChanged -= value; }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {   
	public class CheckEditViewInfo : BaseCheckEditViewInfo {
		CheckState checkState;
		public CheckEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		public new RepositoryItemCheckEdit Item { get { return base.Item as RepositoryItemCheckEdit; } }
		public override bool IsRequiredUpdateOnMouseMove {
			get {
				return !Item.ReadOnly || Item.HotTrackWhenReadOnly;
			}
		}
		protected override void OnEditValueChanged() {
			this.checkState = Item.GetStateByValue(EditValue, true);
			switch(CheckState) {
				case CheckState.Checked:
					this.fDisplayText = string.IsNullOrEmpty(Item.DisplayValueChecked) ? Localizer.Active.GetLocalizedString(StringId.CheckChecked) : Item.DisplayValueChecked; break;
				case CheckState.Unchecked:
					this.fDisplayText = string.IsNullOrEmpty(Item.DisplayValueUnchecked) ? Localizer.Active.GetLocalizedString(StringId.CheckUnchecked) : Item.DisplayValueUnchecked; break;
				default:
					this.fDisplayText = string.IsNullOrEmpty(Item.DisplayValueGrayed) ? Localizer.Active.GetLocalizedString(StringId.CheckIndeterminate) : Item.DisplayValueGrayed; break;
			}
			if(CheckInfo != null) {
				CheckObjectInfoArgs args = CheckInfo as CheckObjectInfoArgs;
				args.CheckState = CheckState;
			}
		}
		protected override void UpdateCheckProperties(BaseCheckObjectInfoArgs e) {
			base.UpdateCheckProperties(e);
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			args.CheckStyle = Item.CheckStyle;
			args.DefaultImages = null;
			if(CheckObjectPainter.RequireDefaultImages(args)) args.DefaultImages = Item.CheckImages;
			args.PictureChecked = Item.PictureChecked;
			args.PictureGrayed = Item.PictureGrayed;
			args.PictureUnchecked = Item.PictureUnchecked;
			args.CheckState = CheckState;
			args.NullStyle = Item.NullStyle;
			args.Images = Item.Images;
			args.ImageIndexChecked = Item.ImageIndexChecked;
			args.ImageIndexUnchecked = Item.ImageIndexUnchecked;
			args.ImageIndexGrayed = Item.ImageIndexGrayed;			
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			CheckEditViewInfo be = info as CheckEditViewInfo;
			if(be == null) return;
			this.checkState = be.checkState;
		}
		public override void Reset() {
			this.checkState = CheckState.Unchecked;
			base.Reset();
		}
		protected override BaseCheckObjectInfoArgs CreateCheckArgs() {
			return new CheckObjectInfoArgs(PaintAppearance);
		}
		protected override BaseCheckObjectPainter CreateCheckPainter() {
			if(IsPrinting) return CheckPainterHelper.GetPainter(ActiveLookAndFeelStyle.Flat);
			return CheckPainterHelper.GetPainter(LookAndFeel);
		}
		protected override ObjectState CalcObjectState() {
			ObjectState newState = base.CalcObjectState();
			if(Item.Enabled && Item.ReadOnly && !Item.HotTrackWhenReadOnly)
				return ObjectState.Normal;
			return newState;
		}
		public CheckState CheckState { get { return checkState; } }
	}	
}
namespace DevExpress.XtraEditors.Drawing {
	public class CheckEditPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawCheck(info);
		}
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
		}
		protected virtual void DrawCheck(ControlGraphicsInfoArgs info) {
			BaseCheckEditViewInfo vi = info.ViewInfo as BaseCheckEditViewInfo;
			vi.CheckInfo.Cache = info.Cache;
			vi.CheckInfo.IsDrawOnGlass = info.IsDrawOnGlass;
			Rectangle r = vi.CheckInfo.CaptionRect;
			try {				
				if(BaseEditViewInfo.ShowFieldBindings && vi.GetDataBindingText() != "") {
					vi.CheckInfo.CaptionRect = Rectangle.Empty;
				}
				if(XtraAnimator.Current.DrawFrame(info.Cache, vi.OwnerEdit, 0))
					vi.CheckPainter.DrawCaption(vi.CheckInfo);
				else
					vi.CheckPainter.DrawObject(vi.CheckInfo);
			}
			finally {
				vi.CheckInfo.Cache = null;
				vi.CheckInfo.CaptionRect = r;
			}
		}
	}
}
