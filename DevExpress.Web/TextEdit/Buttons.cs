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
using System.Text;
using System.Web.UI;
using System.Web;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public enum ButtonsPosition { Left, Right }
	public enum ClearButtonDisplayMode { Auto, Never, OnHover, Always}
	public class EditButton: CollectionItem {
		private ButtonImageProperties fImage = null;
		public EditButton()
			: base() {
		}
		public EditButton(string text)
			: base() {
			Text = text;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonEnabled"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", true); }
			set {
				SetBoolProperty("Enabled", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ButtonImageProperties Image {
			get {
				if(fImage == null)
					fImage = new ButtonImageProperties(this);
				return fImage;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonImagePosition"),
#endif
		DefaultValue(ImagePosition.Left), NotifyParentProperty(true), AutoFormatEnable]
		public virtual ImagePosition ImagePosition {
			get { return (ImagePosition)GetObjectProperty("ImagePosition", ImagePosition.Left); }
			set {
				SetObjectProperty("ImagePosition", ImagePosition.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonPosition"),
#endif
		DefaultValue(ButtonsPosition.Right), NotifyParentProperty(true), AutoFormatEnable]
		public ButtonsPosition Position {
			get { return (ButtonsPosition)GetObjectProperty("Position", ButtonsPosition.Right); }
			set {
				SetObjectProperty("Position", ButtonsPosition.Right, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public virtual string Text {
			get { return GetStringProperty("Text", ""); }
			set {
				SetStringProperty("Text", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool Visible {
			get { return GetBoolProperty("Visible", true); }
			set {
				SetBoolProperty("Visible", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set {
				SetUnitProperty("Width", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditButtonClientVisible"),
#endif
		Category("Client-Side"), NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public virtual bool ClientVisible {
			get { return ClientVisibleInternal; }
			set { ClientVisibleInternal = value; }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			EditButton src = source as EditButton;
			if (src != null) {
				Enabled = src.Enabled;
				Image.Assign(src.Image);
				ImagePosition = src.ImagePosition;
				Position = src.Position;
				Text = src.Text;
				Visible = src.Visible;
				Width = src.Width;
				ToolTip = src.ToolTip;
				ClientVisible = src.ClientVisible;
			}
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected internal virtual ImageProperties GetDefaultImage(Page page, EditorImages images, bool rtl) {
			return images.GetImageProperties(page, EditorImages.ButtonEditEllipsisImageName);
		}
		protected internal virtual bool GetClientVisible() {
			return ClientVisible;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Image };
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ClearButton : EditButton {
		private IWebControlObject owner = null;
		public ClearButton()
			: base() {
		}
		public ClearButton(string text)
			: base(text) {
		}
		public ClearButton(IWebControlObject owner)
			: base() {
			this.owner = owner;
		}
		[Obsolete("Use the DisplayMode property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public AutoBoolean Visibility {
			get { return (AutoBoolean)GetEnumProperty("Visibility", AutoBoolean.Auto); }
			set {
				SetEnumProperty("Visibility", AutoBoolean.Auto, value);
				LayoutChanged();
			}
		}
		[AutoFormatEnable(), DefaultValue(ClearButtonDisplayMode.Auto), NotifyParentProperty(true)]
		public ClearButtonDisplayMode DisplayMode {
			get { return (ClearButtonDisplayMode)GetEnumProperty("DisplayMode", ClearButtonDisplayMode.Auto); }
			set { SetEnumProperty("DisplayMode", ClearButtonDisplayMode.Auto, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ClientVisible { get { return base.ClientVisible; } set { base.ClientVisible = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Visible {
			get {
				var prop = this.owner as ButtonEditPropertiesBase;
				return prop != null && prop.IsClearButtonVisible() || GetBoolProperty("Visible", false);
			}
			set {
				SetBoolProperty("Visible", false, value);
				LayoutChanged();
			}
		}
		protected internal override ImageProperties GetDefaultImage(Page page, EditorImages images, bool rtl) {
			return images.GetImageProperties(page, EditorImages.ButtonEditClearButtonImageName);
		}
		protected internal override bool GetClientVisible() {
			return false;
		}
		protected override bool IsDesignMode() {
			if(this.owner != null)
				return this.owner.IsDesignMode();
			return false;
		}
		protected override bool IsLoading() {
			if(this.owner != null)
				return this.owner.IsLoading();
			return false;
		}
		protected override void LayoutChanged() {
			if(this.owner != null)
				this.owner.LayoutChanged();
		}
		protected override void TemplatesChanged() {
			if(this.owner != null)
				this.owner.TemplatesChanged();
		}
		public override string ToString() {
			return Text;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ClearButton src = source as ClearButton;
			if(src != null) {
				DisplayMode = src.DisplayMode;
			}
		}
	}
	public class EditButtonCollection: Collection {
		public EditButtonCollection()
			: base() {			
		}
		public EditButtonCollection(IWebControlObject control)
			: base(control) {
		}
#if !SL
	[DevExpressWebLocalizedDescription("EditButtonCollectionItem")]
#endif
		public EditButton this[int index] {
			get { return GetItem(index) as EditButton; }
		}
		public void Add(EditButton item) {
			base.Add(item);
		}
		public EditButton Add() {
			EditButton item = new EditButton();
			base.Add(item);
			return item;
		}
		public void Add(string text) {
			Add(new EditButton(text));
		}
		public EditButton FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		public int IndexOf(EditButton item) {
			return base.IndexOf(item);
		}
		public int IndexOfText(string text) {
			for (int i = 0; i < Count; i++)
				if (this[i].Text == text)
					return i;
			return -1;
		}
		protected override Type GetKnownType() {
			return typeof(EditButton);
		}		
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();				
		}
	}
}
