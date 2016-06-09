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
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
using BaseHeaderButtonPreferredConstructor = DevExpress.XtraEditors.Controls.EditorButtonPreferredConstructorAttribute;
namespace DevExpress.XtraEditors.ButtonPanel {
	[TypeConverter(typeof(DevExpress.XtraEditors.Controls.EditorButtonTypeConverter))]
	public class BaseButton : ICaptionSupport, IDXImageUriClient, IDxImageUriProvider, IButtonProperties {
		public event EventHandler Changed;
		public event EventHandler Disposed;
		Image imageCore;
		DxImageUri imageUriCore;
		string captionCore;
		bool useImageCore, useCaptionCore;
		bool enabledCore, visibleCore;
		int imageIndexCore;
		int visibleIndexCore;
		ButtonStyle styleCore;
		bool checkedCore;
		string toolTipCore;
		object glyphsCore;
		SuperToolTip superTipCore;
		ImageLocation imageLocationCore;
		AppearanceObject appearanceCore;
		object tagCore;
		int groupIndexCore;
		bool isLeftCore;
		protected BaseButton() {
			imageIndexCore = -1;
			visibleCore = true;
			useCaptionCore = true;
			enabledCore = true;
			useImageCore = true;
			visibleIndexCore = -1;
			captionCore = "Button";
			toolTipCore = string.Empty;
			superTipCore = null;
			imageLocationCore = ImageLocation.Default;
			imageCore = null;
			checkedCore = false;
			styleCore = ButtonStyle.PushButton;
			glyphsCore = null;
			tagCore = null;
			groupIndexCore = -1;
			isLeftCore = false;
			OnCreate();
		}
		protected virtual void OnCreate() {
			imageUriCore = new DxImageUri();
			imageUriCore.SetClient(this);
			this.appearanceCore = CreateAppearance();
			Appearance.Changed += AppearanceChanged;
		}
		public void Dispose() {
			OnDispose();
		}
		protected virtual void OnDispose() {
			imageUriCore.Dispose();
			if(Appearance != null) {
				Appearance.Changed -= AppearanceChanged;
				RaiseDisposed(EventArgs.Empty);
				appearanceCore.Dispose();
				appearanceCore = null;
			}
			ownerCore = null;
		}
		#region IButton
		[Browsable(false)]
		public virtual bool? IsChecked {
			get { return Style == ButtonStyle.CheckButton ? (bool?)Checked : (bool?)null; }
		}
		IButtonProperties IBaseButton.Properties {
			get { return this; }
		}
		public event EventHandler CheckedChanged;
		#endregion IButton
		#region IButtonProperties
		[DefaultValue(ButtonStyle.PushButton), Category("Behavior")]
		public virtual ButtonStyle Style {
			get { return styleCore; }
			set { SetValue(ref styleCore, value, "Toggle"); }
		}
		[Editor(ControlConstants.MultilineStringEditor, typeof(System.Drawing.Design.UITypeEditor))]
		[DefaultValue(""), Category("ToolTip")]
		public virtual string ToolTip {
			get { return toolTipCore; }
			set { toolTipCore = value; }
		}
		[Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign,
			typeof(System.Drawing.Design.UITypeEditor))]
		[DefaultValue(null), Category("ToolTip")]
		public virtual SuperToolTip SuperTip {
			get { return superTipCore; }
			set { superTipCore = value; }
		}
		[DefaultValue(-1), Category("Appearance")]
		[Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)),
		DevExpress.Utils.ImageList("Images")]
		public virtual int ImageIndex {
			get { return imageIndexCore; }
			set { SetValue(ref imageIndexCore, value, "ImageIndex"); }
		}
		[DefaultValue(-1), Category("Behavior")]
		public virtual int GroupIndex {
			get { return groupIndexCore; }
			set { SetValue(ref groupIndexCore, value, "GroupIndex"); }
		}
		[DefaultValue("Button"), Category("Appearance")]
		public virtual string Caption {
			get { return captionCore; }
			set { SetValue(ref captionCore, value, "Caption"); }
		}
		[DefaultValue(null), Category("Appearance"), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get { return imageCore; }
			set { SetValue(ref imageCore, value, "Image"); }
		}
		[Category("Appearance"), Editor("DevExpress.Utils.Design.DxImageUriEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
		public virtual string ImageUri {
			get { return imageUriCore; }
			set {
				if(value == imageUriCore.Uri) return;
				imageUriCore.Uri = value;
				if(!IsUpdateLocked)
					OnObjectChanged("ImageUri");
			}
		}
		public bool ShouldSerializeImageUri() {
			return ImageUri != String.Empty;
		}
		public void ResetImageUri() {
			imageUriCore.Reset();
		}
		protected virtual ImageLocation ImageLocationCore {
			get { return imageLocationCore; }
			set { SetValue(ref imageLocationCore, value, "ImageLocation"); }
		}
		[DefaultValue(true), Category("Appearance")]
		public virtual bool UseImage {
			get { return useImageCore; }
			set { SetValue(ref useImageCore, value, "UseImage"); }
		}
		[DefaultValue(true), Category("Appearance")]
		public virtual bool UseCaption {
			get { return useCaptionCore; }
			set { SetValue(ref useCaptionCore, value, "UseText"); }
		}
		[DefaultValue(true), Category("Appearance")]
		public virtual bool Visible {
			get { return visibleCore; }
			set { SetValue(ref visibleCore, value, "Visible"); }
		}
		[DefaultValue(true), Category("Behavior")]
		public virtual bool Enabled {
			get { return enabledCore; }
			set { SetValue(ref enabledCore, value, "Enabled"); }
		}
		[DefaultValue(-1), Category("Appearance")]
		public virtual int VisibleIndex {
			get { return visibleIndexCore; }
			set { SetValue(ref visibleIndexCore, value, "VisibleIndex"); }
		}
		[DefaultValue(false), Category("Behavior")]
		public virtual bool Checked {
			get { return checkedCore; }
			set {
				if(checkedCore != value) {
					SetValue(ref checkedCore, value, "Checked");
					if(IsLockCheckEvent) return;
					RaiseCheckedChanged();
				}
			}
		}
		void RaiseCheckedChanged() {
			if(CheckedChanged != null)
				CheckedChanged(this, EventArgs.Empty);
		}
		protected void RaiseDisposed(EventArgs e) {
			if(Disposed != null)
				Disposed(this, e);
		}
		[DefaultValue(null), Category("Data")]
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public virtual object Tag {
			get { return tagCore; }
			set { SetValue(ref tagCore, value, "Tag"); }
		}
		[Category("Appearance"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual object Glyphs {
			get { return glyphsCore; }
			set { SetValue(ref glyphsCore, value, "Glyphs"); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public AppearanceObject Appearance { get { return appearanceCore; } }
		protected virtual bool ShouldSerializeAppearance() { return Appearance != null && Appearance.ShouldSerialize(); }
		protected virtual void ResetAppearance() { Appearance.Reset(); }
		protected internal bool IsLeft {
			get { return GetIsLeft(); }
			set { SetIsLeft(value); }
		}
		protected virtual bool GetIsLeft() {
			return isLeftCore;
		}
		protected virtual void SetIsLeft(bool value) {
			SetValue(ref isLeftCore, value, "IsLeft");
		}
		ImageLocation IButtonProperties.ImageLocation {
			get { return ImageLocationCore; }
			set { ImageLocationCore = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Images {
			get {
				var owner = GetOwner();
				return owner != null ? owner.Images : null;
			}
		}
		#endregion IButtonProperties
		IButtonsPanel ownerCore;
		public void SetOwner(IButtonsPanel owner) {
			if(mergedOwnerCore == null)
				ownerCore = owner;
		}
		IButtonsPanel mergedOwnerCore;
		public void SetMerged(IButtonsPanel mergedOwner) {
			mergedOwnerCore = mergedOwner;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public IButtonsPanel GetOwner() {
			return mergedOwnerCore ?? ownerCore;
		}
		protected IButtonsPanel GetNativeOwner() {
			return ownerCore;
		}
		protected virtual AppearanceObject CreateAppearance() {
			return new SerializableAppearanceObject();
		}
		void AppearanceChanged(object sender, EventArgs e) {
			OnObjectChanged("Appearance");
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		string ICaptionSupport.Caption { get { return Caption; } }
		#region ISupportGroupUpdate Members
		bool lockCheckEventCore;
		internal bool IsLockCheckEvent { get { return lockCheckEventCore; } }
		public void LockCheckEvent() {
			lockCheckEventCore = true;
			BeginUpdate();
		}
		public void UnlockCheckEvent() {
			lockCheckEventCore = false;
			CancelUpdate();
		}
		#endregion
		protected void SetValue<T>(ref T field, T value, string propertyName) {
			if(object.Equals(field, value)) return;
			field = value;
			if(!IsUpdateLocked)
				OnObjectChanged(propertyName);
		}
		protected void OnObjectChanged(string propertyName) {
			RaiseChanged(new PropertyChangedEventArgs(propertyName));
		}
		protected void RaiseChanged(EventArgs e) {
			if(Changed != null) Changed(this, e);
		}
		int lockUpdateCore = 0;
		#region IButtonProperties Members
		[Browsable(false)]
		public bool IsUpdateLocked {
			get { return lockUpdateCore != 0; }
		}
		public void BeginUpdate() {
			lockUpdateCore++;
		}
		public void EndUpdate() {
			lockUpdateCore--;
			RaiseChanged(EventArgs.Empty);
		}
		public void CancelUpdate() {
			lockUpdateCore--;
		}
		#endregion
		#region IDXImageUriClient Members
		bool IDXImageUriClient.SupportsLookAndFeel {
			get { return CheckOwnerLookAndFeelSupport(); }
		}
		bool IDXImageUriClient.SupportsGlyphSkinning {
			get {
				IButtonsPanelOwner panelOwner = GetPanelOwner();
				if(panelOwner == null) return false;
				return panelOwner.AllowGlyphSkinning;
			}
		}
		IButtonsPanelOwner GetPanelOwner() {
			IButtonsPanel panel = GetOwner();
			if(panel == null) return null;
			return panel.Owner;
		}
		bool CheckOwnerLookAndFeelSupport() {
			IButtonsPanelOwner panelOwner = GetPanelOwner();
			if(panelOwner == null) return false;
			return panelOwner is ISupportLookAndFeel;
		}
		LookAndFeel.UserLookAndFeel IDXImageUriClient.LookAndFeel {
			get {
				if(!CheckOwnerLookAndFeelSupport()) return null;
				return (GetPanelOwner() as ISupportLookAndFeel).LookAndFeel;
			}
		}
		void IDXImageUriClient.SetGlyphSkinningValue(bool value) { }
		bool IDXImageUriClient.IsDesignMode { get { return false; } }
		#endregion
		#region IDxImageUriProvider Members
		DxImageUri IDxImageUriProvider.ImageUri { get { return imageUriCore; } }
		#endregion
	}
	public class BaseSeparator : BaseButton {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override bool? IsChecked {
			get { return (bool?)null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public sealed override object Images {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override object Glyphs {
			get { return null; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override int ImageIndex {
			get { return base.ImageIndex; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override int GroupIndex {
			get { return base.GroupIndex; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override string ToolTip {
			get { return base.ToolTip; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override SuperToolTip SuperTip {
			get { return base.SuperTip; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override string Caption {
			get { return base.Caption; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override object Tag {
			get { return base.Tag; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override bool Checked {
			get { return false; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override bool UseCaption {
			get { return base.UseCaption; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override bool UseImage {
			get { return base.UseImage; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected sealed override ImageLocation ImageLocationCore {
			get { return base.ImageLocationCore; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override ButtonStyle Style {
			get { return base.Style; }
			set { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public sealed override bool Enabled {
			get { return base.Enabled; }
			set { }
		}
	}
}
namespace DevExpress.XtraEditors.ButtonsPanelControl {
	public class ExpandButton : BasePinButton, IBaseButton {
		public override int ImageIndex {
			get { return Checked ? 3 : 4; }
		}
	}
	public class LayoutViewCardExpandButton : ExpandButton {
		public LayoutViewCardExpandButton(Utils.Drawing.GroupObjectInfoArgs info) {
			GroupObjectInfoArgs = info;
		}
		public Utils.Drawing.GroupObjectInfoArgs GroupObjectInfoArgs { get; set; }
	}
	public class RibbonPageGroupButton : BaseButton, IDefaultButton {
		public RibbonPageGroupButton(Utils.Drawing.GroupObjectInfoArgs info) {
			GroupObjectInfoArgs = info;
		}
		public override bool UseCaption {
			get { return false; }
			set { }
		}
		public Utils.Drawing.GroupObjectInfoArgs GroupObjectInfoArgs { get; set; }
	}
	public class GroupBoxButton : BaseButton, IBaseButton, IButtonProperties {
		DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocationCore;
		public GroupBoxButton() { }
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image, bool useCaption, bool enabled, string toolTip)
			: this(caption, image, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, toolTip, useCaption, -1, -1) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image, string imageUri, bool useCaption, bool enabled, string toolTip)
			: this(caption, image, imageUri, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, toolTip, useCaption, -1, enabled, null, true, false, true, null, null, -1) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image, int imageIndex, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, string imageUri, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image)
			: this(caption, image, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, -1) {
		}
		public GroupBoxButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public GroupBoxButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public GroupBoxButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image, string imageUri)
			: this(caption, image, imageUri, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, String.Empty, true, -1, true, null, true, false, true, null, null, -1) {
		}
		public GroupBoxButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public GroupBoxButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public GroupBoxButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex)
			: this(caption, image, string.Empty, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex)
			: this(caption, null, imageUri, -1, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public GroupBoxButton(string caption, Image image, string imageUri, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex)
			: this() {
			Image = image;
			ImageUri = imageUri;
			Caption = caption;
			ImageIndex = imageIndex;
			ImageLocation = imageLocation;
			Visible = visible;
			UseCaption = useCaption;
			Enabled = enabled;
			UseImage = useImage;
			VisibleIndex = visibleIndex;
			Checked = @checked;
			Style = style;
			ToolTip = toolTip;
			SuperTip = superTip;
			Glyphs = glyphs;
			Tag = tag;
			GroupIndex = groupIndex;
		}
		[DefaultValue(DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default), Category("Appearance")]
		public DevExpress.XtraEditors.ButtonPanel.ImageLocation ImageLocation {
			get { return imageLocationCore; }
			set { SetValue(ref imageLocationCore, value, "ImageLocation"); }
		}
		DevExpress.XtraEditors.ButtonPanel.ImageLocation IButtonProperties.ImageLocation {
			get { return (DevExpress.XtraEditors.ButtonPanel.ImageLocation)(int)(ImageLocation); }
			set { ImageLocation = (DevExpress.XtraEditors.ButtonPanel.ImageLocation)((int)value); }
		}
		[Browsable(false)]
		public new AppearanceObject Appearance { get { return base.Appearance; } }
		protected internal IButtonsPanel Owner {
			get { return GetOwner(); }
		}
	}
	public class ButtonControl : DevExpress.XtraEditors.ButtonPanel.BaseButton, IBaseButton, IButtonProperties {
		DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocationCore;
		public ButtonControl()
			: base() {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image, bool useCaption, bool enabled, string toolTip)
			: this(caption, image, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, toolTip, useCaption, -1, -1) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image, string imageUri, bool useCaption, bool enabled, string toolTip)
			: this(caption, image, imageUri, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, toolTip, useCaption, -1, enabled, null, true, false, true, null, null, -1, false) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image, int imageIndex, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, string imageUri, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image)
			: this(caption, image, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, -1) {
		}
		public ButtonControl(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public ButtonControl(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public ButtonControl(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, groupIndex, false) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image, string imageUri)
			: this(caption, image, imageUri, -1, DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, ButtonStyle.PushButton, null, true, -1, true, null, true, false, true, null, null, -1, false) {
		}
		public ButtonControl(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public ButtonControl(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public ButtonControl(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, groupIndex, false) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft)
			: this(caption, image, string.Empty, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, string imageUri, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft)
			: this(caption, null, imageUri, -1, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public ButtonControl(string caption, Image image, string imageUri, int imageIndex, DevExpress.XtraEditors.ButtonPanel.ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft)
			: this() {
			Image = image;
			Caption = caption;
			ImageIndex = imageIndex;
			ImageLocation = imageLocation;
			Visible = visible;
			UseCaption = useCaption;
			Enabled = enabled;
			UseImage = useImage;
			VisibleIndex = visibleIndex;
			Checked = @checked;
			Style = style;
			ToolTip = toolTip;
			SuperTip = superTip;
			Glyphs = glyphs;
			Tag = tag;
			GroupIndex = groupIndex;
			IsLeft = isLeft;
		}
		[DefaultValue(DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default), Category("Appearance")]
		public DevExpress.XtraEditors.ButtonPanel.ImageLocation ImageLocation {
			get { return imageLocationCore; }
			set { SetValue(ref imageLocationCore, value, "ImageLocation"); }
		}
		DevExpress.XtraEditors.ButtonPanel.ImageLocation IButtonProperties.ImageLocation {
			get { return (DevExpress.XtraEditors.ButtonPanel.ImageLocation)(int)(ImageLocation); }
			set { ImageLocation = (DevExpress.XtraEditors.ButtonPanel.ImageLocation)((int)value); }
		}
		[DefaultValue(false), Category("Appearance")]
		public new bool IsLeft {
			get { return GetIsLeft(); }
			set { SetIsLeft(value); }
		}
		[Browsable(false)]
		public new AppearanceObject Appearance { get { return base.Appearance; } }
		protected internal IButtonsPanel Owner {
			get { return GetOwner(); }
		}
	}
}
