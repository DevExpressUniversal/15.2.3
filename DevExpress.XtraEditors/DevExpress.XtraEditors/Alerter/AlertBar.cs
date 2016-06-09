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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Drawing.Design;
namespace DevExpress.XtraBars.Alerter {
	public enum AlertButtonStyle { Button, CheckButton };
	public enum AlertButtonState { Normal, Hot, Pressed, NormalChecked, HotChecked, PressedChecked };
	[Bindable(false)]
	public class AlertButtonCollection : CollectionBase {
		IAlertControl owner;
		public AlertButtonCollection() : this(null) { }
		public AlertButtonCollection(object owner) {
			if(owner is IAlertControl)
				this.owner = (IAlertControl)owner;
		}
		public AlertButton this[int index] {
			get { return List[index] as AlertButton; }
			set { List[index] = value; }
		}
		public AlertButton this[string name] {
			get {
				foreach(AlertButton item in this)
					if(item.Name == name) return item;
				return null;
			}
		}
		public int Add(object item) {
			AlertButton button = item as AlertButton;
			if(button == null)
				button = new AlertButton();
			return List.Add(button);
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertButtonCollectionPinButton")]
#endif
		public AlertButton PinButton {
			get {
				foreach(AlertButton item in this)
					if(item is AlertPinButton) return item;
				return null;
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertButtonCollectionCloseButton")]
#endif
		public AlertButton CloseButton {
			get {
				foreach(AlertButton item in this)
					if(item is AlertCloseButton) return item;
				return null;
			}
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			this[index].SetAlertControl(owner);
			if(!this[index].Predefined && this[index].Name == string.Empty)
				this[index].Name = GetButtonName();
		}
		string GetButtonName() {
			for(int i = 1; i < 1000; i++) {
				string name = string.Format("alertButton{0}", i);
				if(!IsExistName(name))
					return name;
			}
			return string.Empty;
		}
		internal bool IsExistName(string name) {
			foreach(AlertButton item in this)
				if(item.Name == name) return true;
			return false;
		}
		public AlertButton GetButtonByHint(string hint) {
			foreach(AlertButton item in this)
				if(item.Hint == hint) return item;
			return null;
		}
		internal AlertButton GetFirstPredefinedButton() {
			foreach(AlertButton item in this)
				if(item.Predefined) return item;
			return null;
		}
		internal AlertButton GetFirstCustomButton(int x) {
			foreach(AlertButton item in this)
				if(!item.Predefined && item.Bounds.Right > x) return item;
			return null;
		}
		#region Properties
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertButtonCollectionPredefinedButtonCount")]
#endif
		public int PredefinedButtonCount {
			get {
				int ret = 0;
				foreach(AlertButton item in this)
					if(item.Predefined) ret++;
				return ret;
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertButtonCollectionCustomButtonCount")]
#endif
		public int CustomButtonCount {
			get {
				int ret = 0;
				foreach(AlertButton item in this)
					if(!item.Predefined) ret++;
				return ret;
			}
		}
		#endregion
		#region Handler
		internal void OnMouseMove(MouseEventArgs e) {
			if(e.Button != MouseButtons.None) return;
			foreach(AlertButton item in this) {
				if(item.RealState == AlertButtonState.Pressed) continue;
				if(item.Bounds.Contains(e.X, e.Y))
					item.State = AlertButtonState.Hot;
				else item.State = AlertButtonState.Normal;
			}
		}
		internal void OnMouseLeave() {
			foreach(AlertButton item in this)
				item.State = AlertButtonState.Normal;
		}
		internal bool OnMouseDown(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return false;
			bool ret = false;
			foreach(AlertButton item in this) {
				if(item.Bounds.Contains(e.X, e.Y)) {
					item.State = AlertButtonState.Pressed;
					ret = true;
				}
			}
			return ret;
		}
		internal void OnMouseUp(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			foreach(AlertButton item in this) {
				if(item.Bounds.Contains(e.X, e.Y) && item.RealState == AlertButtonState.Pressed) {
					item.OnClick();
				}
				if(item.RealState == AlertButtonState.Pressed)
					item.State = AlertButtonState.Normal;
			}
		}
		internal AlertButton GetButtonByPoint(Point p) {
			foreach(AlertButton item in this) {
				if(item.Bounds.Contains(p.X, p.Y))
					return item;
			}
			return null;
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			foreach(AlertButton item in this)
				item.Dispose();
		}
		#endregion
	}
	public class AlertButton : IDisposable {
		protected bool actionElement = false;
		AlertButtonStyle style = AlertButtonStyle.Button;
		AlertButtonState state = AlertButtonState.Normal;
		AlertFormCore owner;
		Image image = null, imageDown = null;
		bool down = false, visible = true;
		Rectangle bounds = Rectangle.Empty;
		string hint = string.Empty, name = string.Empty;
		int imageIndex = -1, imageDownIndex = -1;
		public AlertButton() : this(null) { }
		public AlertButton(Image image) : this(image, AlertButtonStyle.Button) { }
		public AlertButton(Image image, AlertButtonStyle style) {
			this.image = image;
			this.style = style;
		}
		public void SetOwner(AlertFormCore form) {
			this.owner = form;
			owner.Paint += new PaintEventHandler(OnPaint);
		}
		#region Properties [Browsable(false)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AlertFormCore Owner { get { return owner; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if(bounds.Equals(value)) return;
				bounds = value;
				Invalidate();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AlertButtonState State {
			get {
				if(actionElement) return AlertButtonState.NormalChecked;
				if(Down) {
					return (AlertButtonState)(Convert.ToInt32(state) + 3);
				}
				return state;
			}
			set {
				if(state == value) return;
				state = value;
				Invalidate();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Predefined { get { return false; } }
		internal AlertButtonState RealState { get { return state; } }
		internal Image StateImage {
			get {
				if(Down && ImageDown != null) return ImageDown;
				return Image;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object Images {
			get {
				if(control == null) return null;
				return control.Images;
			}
		}
		IAlertControl control = null;
		internal void SetAlertControl(IAlertControl control) {
			if(this.control != control)
				this.control = control;
		}
		#endregion
		#region Properties
		protected virtual bool ShouldSerializeStyle() { return Style != AlertButtonStyle.Button; }
		protected virtual void ResetStyle() { Style = AlertButtonStyle.Button; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonStyle"),
#endif
 DXCategory(CategoryName.Behavior)]
		public AlertButtonStyle Style {
			get { return style; }
			set {
				if(style == value) return;
				style = value;
			}
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonImage"),
#endif
 DXCategory(CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return SkinImage == null ? image : SkinImage; }
			set { image = value; }
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonImageDown"),
#endif
 DXCategory(CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image ImageDown {
			get { return SkinImageDown == null ? imageDown : SkinImageDown; }
			set { imageDown = value; }
		}
		protected virtual Image SkinImage { get { return null; } }
		protected virtual Image SkinImageDown { get { return null; } }
		[DefaultValue(true), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonVisible"),
#endif
 DXCategory(CategoryName.Behavior)]
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value) return;
				visible = value;
				UpdateButtons();
			}
		}
		[DefaultValue(""), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonHint"),
#endif
 DXCategory(CategoryName.Appearance)]
		public string Hint {
			get { return hint; }
			set {
				if(hint == value) return;
				hint = value;
			}
		}
		[DefaultValue(""), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonName"),
#endif
 DXCategory(CategoryName.Data)]
		public string Name {
			get { return name; }
			set {
				if(name == value) return;
				name = value;
			}
		}
		[DefaultValue(false), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonDown"),
#endif
 DXCategory(CategoryName.Behavior)]
		public bool Down {
			get {
				if(Style != AlertButtonStyle.CheckButton) return false;
				return down;
			}
			set {
				if(down == value) return;
				down = value;
				Invalidate();
			}
		}
		[DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), DevExpress.Utils.ImageList("Images"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonImageIndex"),
#endif
 DXCategory(CategoryName.Appearance)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(imageIndex == value) return;
				imageIndex = value;
				this.Image = GetImage(null, imageIndex, null);
				Invalidate();
			}
		}
		[DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)), DevExpress.Utils.ImageList("Images"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertButtonImageDownIndex"),
#endif
 DXCategory(CategoryName.Appearance)]
		public int ImageDownIndex {
			get { return imageDownIndex; }
			set {
				if(imageDownIndex == value) return;
				imageDownIndex = value;
				this.ImageDown = GetImage(null, imageDownIndex, null);
				Invalidate();
			}
		}
		#endregion
		#region Painter
		protected virtual void OnPaint(object sender, PaintEventArgs e) {
			if(!Visible) return;
			DrawBackground(e.Graphics);
			DrawImage(e.Graphics, StateImage, bounds);
		}
		protected virtual void DrawImage(Graphics graphics, Image image, Rectangle bounds) {
			if(image == null) return;
			graphics.DrawImage(image, GetCenterRectangle(bounds, image));
		}
		protected virtual void DrawBackground(Graphics graphics) {
			if(State == AlertButtonState.Normal) return;
			GraphicsCache cache = new GraphicsCache(graphics);
			Skin skin = BarSkins.GetSkin(owner.ViewInfo.CurrentSkinProvider);
			SkinElement element = skin[BarSkins.SkinAlertBarItem];
			SkinElementInfo eInfo = new SkinElementInfo(element, bounds);
			eInfo.ImageIndex = Convert.ToInt32(State) - 1;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, eInfo);
		}
		protected internal Image GetSkinImage(int index) {
			Skin skin = BarSkins.GetSkin(owner.ViewInfo.CurrentSkinProvider);
			SkinElement element = skin["AlertButton"];
			if(element == null || element.Glyph.Image == null) return null;
			Image ret = element.Glyph.GetImages().Images[index];
			if((State == AlertButtonState.Hot || State == AlertButtonState.HotChecked) && element.Properties["ForeColorHot"] != null) {
				Color col = element.Properties.GetColor("ForeColorHot");
				if(!col.IsEmpty) return ImageColorizer.GetColoredImage(ret, col);
			}
			if((State == AlertButtonState.Pressed || State == AlertButtonState.PressedChecked || State == AlertButtonState.NormalChecked) && element.Properties["ForeColorPressed"] != null) {
				Color col = element.Properties.GetColor("ForeColorPressed");
				if(!col.IsEmpty) return ImageColorizer.GetColoredImage(ret, col);
			}
			return ImageColorizer.GetColoredImage(ret, element.Color.GetForeColor());
		}
		#endregion
		protected void Invalidate() {
			if(Owner == null) return;
			Owner.Invalidate(Bounds);
		}
		protected void UpdateButtons() {
			if(Owner == null) return;
			Owner.UpdateButtons();
		}
		Rectangle GetCenterRectangle(Rectangle rect, Image image) {
			return new Rectangle(rect.X + (rect.Width - image.Width) / 2, rect.Y + (rect.Height - image.Height) / 2, image.Width, image.Height);
		}
		public Size GetButtonSize() {
			if(!visible) return Size.Empty;
			if(image == null || owner == null) return new Size(10, 10);
			SkinElement element = BarSkins.GetSkin(owner.ViewInfo.CurrentSkinProvider)[BarSkins.SkinAlertBarItem];
			if(element == null) return new Size(image.Width + 6, image.Height + 6);
			return new Size(image.Width + element.ContentMargins.Width, image.Height + element.ContentMargins.Height);
		}
		public virtual void OnClick() {
			if(Owner.AlertControl != null)
				Owner.AlertControl.RaiseButtonClick(this, Owner.AlertInfo, Owner);
			if(Style == AlertButtonStyle.CheckButton) {
				Down = !Down;
				if(Owner.AlertControl != null)
					Owner.AlertControl.RaiseButtonDownChanged(this, Owner.AlertInfo, Owner);
			}
		}
		public void SetDown(bool down) {
			if(Down != down && Style == AlertButtonStyle.CheckButton)
				OnClick();
		}
		public override string ToString() {
			if(Hint != string.Empty) return Hint;
			else if(Name != string.Empty) return Name;
			return base.ToString();
		}
		#region IDisposable Members
		public virtual void Dispose() {
			if(owner != null)
				owner.Paint -= new PaintEventHandler(OnPaint);
		}
		#endregion
		internal void AssignFrom(AlertButton source, object images) {
			this.style = source.Style;
			this.name = source.Name;
			this.hint = source.Hint;
			this.down = source.Down;
			this.image = GetImage(source.Image, source.ImageIndex, images);
			this.imageDown = GetImage(source.ImageDown, source.ImageDownIndex, images);
			this.visible = source.Visible;
		}
		private Image GetImage(Image image, int index, object images) {
			if(image != null) return image;
			return ImageCollection.GetImageListImage(GetImageCollection(images), index);
		}
		private object GetImageCollection(object images) {
			if(images != null) return images;
			if(Owner == null) return null;
			return Owner.AlertControl.Images;
		}
	}
	public class AlertPredefinedButton : AlertButton {
		public AlertPredefinedButton(Image image) : base(image) { }
		public AlertPredefinedButton(Image image, AlertButtonStyle style) : base(image, style) { }
		public override bool Predefined { get { return true; } }
	}
	public class AlertCloseButton : AlertPredefinedButton {
		public AlertCloseButton(AlertFormCore form, AlertButtonCollection collection)
			: base(AlertControlHelper.WindowImages.Images[0]) {
			SetOwner(form);
			Bounds = form.ViewInfo.GetControlBoxElementRectangle(collection.PredefinedButtonCount, this.GetButtonSize());
		}
		protected override Image SkinImage { get { return GetSkinImage(3); } }
		public override void OnClick() {
			base.OnClick();
			Owner.CloseForm();
		}
	}
	public class AlertPinButton : AlertPredefinedButton {
		public AlertPinButton(AlertFormCore form, AlertButtonCollection collection)
			: base(AlertControlHelper.WindowImages.Images[1], AlertButtonStyle.CheckButton) {
			SetOwner(form);
			ImageDown = AlertControlHelper.WindowImages.Images[2];
			Bounds = form.ViewInfo.GetControlBoxElementRectangle(collection.PredefinedButtonCount, this.GetButtonSize());
		}
		protected override Image SkinImage { get { return GetSkinImage(1); } }
		protected override Image SkinImageDown { get { return GetSkinImage(2); } }
		public override void OnClick() {
			base.OnClick();
			Owner.Pin = this.Down;
		}
	}
}
