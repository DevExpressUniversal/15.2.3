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
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.ButtonPanel;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.XtraEditors.ButtonsPanelControl {
	[ToolboxItem(false)]
	public class ButtonPanelControl : Control, IButtonsPanelOwnerEx, ISupportLookAndFeel, IButtonPanelControlAppearanceOwner, IToolTipControlClient {
		ButtonsPanelControl buttonsPanelCore;
		ToolTipController toolTipControllerCore;
		public ButtonPanelControl() {
			buttonsPanelCore = CreateButtonsPanel();
			SubscribeButtonsPanel();
			lookAndFeelCore = CreateLookAndFeel();
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				LookAndFeel.StyleChanged -= LookAndFeel_StyleChanged;
				UnsubscribeButtonsPanel();
				ButtonsPanel.Dispose();
				buttonsPanelCore = null;
			}
			base.Dispose(disposing);
		}
		protected virtual UserLookAndFeel CreateLookAndFeel() {
			return new UserLookAndFeel(this);
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			OnLookAndFeelStyleChanged();
		}
		protected virtual void OnLookAndFeelStyleChanged() {
			ButtonsPanel.UpdateStyle();
		}
		protected virtual ButtonsPanelControl CreateButtonsPanel() {
			return new ButtonsPanelControl(this);
		}
		[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraEditors.Design.ButtonsPanelControlDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign,
			typeof(System.Drawing.Design.UITypeEditor)), Category("Buttons"), Localizable(true)]
		public BaseButtonCollection Buttons {
			get { return ButtonsPanel.Buttons; }
		}
		[ DefaultValue(null), Category("Appearance")]
		public virtual ToolTipController ToolTipController {
			get { return toolTipControllerCore ?? ToolTipController.DefaultController; }
			set {
				if(ToolTipController == value) return;
				if(ToolTipController != null) ToolTipController.Disposed -= new EventHandler(ToolTipControllerDisposed);
				toolTipControllerCore = value;
				if(ToolTipController != null) ToolTipController.Disposed += new EventHandler(ToolTipControllerDisposed);
			}
		}
		[ DefaultValue(ContentAlignment.MiddleCenter), Category("Layout")]
		public ContentAlignment ContentAlignment {
			get { return ButtonsPanel.ContentAlignment; }
			set { ButtonsPanel.ContentAlignment = value; }
		}
		[ DefaultValue(Orientation.Horizontal), Category("Layout")]
		public Orientation Orientation {
			get { return ButtonsPanel.Orientation; }
			set { ButtonsPanel.Orientation = value; }
		}
		[ DefaultValue(0), Category("Layout")]
		public int ButtonInterval {
			get { return ButtonsPanel.ButtonInterval; }
			set { ButtonsPanel.ButtonInterval = value; }
		}
		protected void UpdateButtonPanel() {
			if(ButtonsPanel != null) {
				ButtonsPanel.ViewInfo.SetDirty();
				Invalidate();
				Update();
			}
		}
		protected void SubscribeButtonsPanel() {
			ButtonsPanel.ButtonClick += RaiseButtonClick;
			ButtonsPanel.ButtonChecked += RaiseButtonChecked;
			ButtonsPanel.ButtonUnchecked += RaiseButtonUnchecked;
			ButtonsPanel.Changed += ButtonsPanel_Changed;
		}
		protected void UnsubscribeButtonsPanel() {
			ButtonsPanel.ButtonClick -= RaiseButtonClick;
			ButtonsPanel.ButtonChecked -= RaiseButtonChecked;
			ButtonsPanel.ButtonUnchecked -= RaiseButtonUnchecked;
			ButtonsPanel.Changed -= ButtonsPanel_Changed;
		}
		#region Events
		static readonly object buttonClick = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler ButtonClick {
			add { this.Events.AddHandler(buttonClick, value); }
			remove { this.Events.RemoveHandler(buttonClick, value); }
		}
		static readonly object buttonUnchecked = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler ButtonUnchecked {
			add { this.Events.AddHandler(buttonUnchecked, value); }
			remove { this.Events.RemoveHandler(buttonUnchecked, value); }
		}
		static readonly object buttonChecked = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler ButtonChecked {
			add { this.Events.AddHandler(buttonChecked, value); }
			remove { this.Events.RemoveHandler(buttonChecked, value); }
		}
		protected virtual void RaiseButtonChecked(object sender, BaseButtonEventArgs e) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)this.Events[buttonChecked];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseButtonUnchecked(object sender, BaseButtonEventArgs e) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)this.Events[buttonUnchecked];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseButtonClick(object sender, BaseButtonEventArgs e) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)this.Events[buttonClick];
			if(handler != null) handler(this, e);
		}
		#endregion
		void ButtonsPanel_Changed(object sender, EventArgs e) {
			UpdateButtonPanel();
		}
		protected virtual void CalcButtonsPanel(Graphics g) {
			if(ButtonsPanel != null) {
				ButtonsPanel.ViewInfo.Calc(g, CalcClientRectangle(ClientRectangle));
			}
		}
		protected Rectangle CalcClientRectangle(Rectangle bounds) {
			return new Rectangle(
				bounds.Left + Padding.Left,
				bounds.Top + Padding.Top,
				bounds.Width - Padding.Horizontal,
				bounds.Height - Padding.Vertical);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseMove(e);
			ButtonsPanel.Handler.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ButtonsPanel.Handler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseMove(e);
			ButtonsPanel.Handler.OnMouseUp(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ButtonsPanel.Handler.OnMouseLeave();
			ToolTipController.RemoveClientControl(this);
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			ToolTipController.AddClientControl(this);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			UpdateButtonPanel();
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			UpdateButtonPanel();
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			UpdateButtonPanel();
		}
		protected ButtonsPanelControl ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			CalcButtonsPanel(e.Graphics);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				if(ButtonsPanel != null && ButtonsPanel.ViewInfo != null) {
					DrawButtonsPanel(cache);
				}
			}
		}
		protected virtual void DrawButtonsPanel(GraphicsCache cache) {
			ObjectPainter.DrawObject(cache, ((IButtonsPanelOwner)this).GetPainter(), (ObjectInfoArgs)ButtonsPanel.ViewInfo);
		}
		#region IButtonsPanelOwner Members
		public virtual DevExpress.Utils.Drawing.ObjectPainter GetPainter() {
			if(IsSkinPaintStyle)
				return new ButtonsPanelControlSkinPainter(LookAndFeel);
			return new ButtonsPanelControlPainter();
		}
		protected virtual internal bool IsSkinPaintStyle {
			get { return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; }
		}
		object imagesCore;
		[
		DefaultValue(null), Category("Appearance"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object Images {
			get { return imagesCore; }
			set {
				if(Images == value) return;
				imagesCore = value;
				UpdateButtonPanel();
			}
		}
		[Browsable(false)]
		public bool IsSelected {
			get { return false; }
		}
		#endregion
		#region ISupportLookAndFeel Members
		[Browsable(false)]
		public bool IgnoreChildren {
			get { return true; }
		}
		UserLookAndFeel lookAndFeelCore;
		[ Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeelCore; }
		}
		#endregion
		private System.ComponentModel.IContainer components = null;
		#region IButtonsPanelControlAppearanceOwner Members
		ButtonsPanelControlAppearance buttonAppearanceCore;
		object buttonBackgroundImagesCore;
		bool allowHtmlDrawCore;
		bool allowGlyphSkinningCore;
		[
		 DefaultValue(null), Category("Appearance"), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ButtonBackgroundImages {
			get { return buttonBackgroundImagesCore; }
			set {
				if(ButtonBackgroundImages == value) return;
				buttonBackgroundImagesCore = value;
				UpdateButtonPanel();
			}
		}
		[
		DefaultValue(false), Category("Appearance")]
		public bool AllowHtmlDraw {
			get { return allowHtmlDrawCore; }
			set {
				if(allowHtmlDrawCore == value) return;
				allowHtmlDrawCore = value;
				UpdateButtonPanel();
			}
		}
		[
		DefaultValue(false), Category("Appearance")]
		public bool AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(allowGlyphSkinningCore == value) return;
				allowGlyphSkinningCore = value;
				UpdateButtonPanel();
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
		public virtual ButtonsPanelControlAppearance AppearanceButton {
			get {
				if(buttonAppearanceCore == null) {
					buttonAppearanceCore = new ButtonsPanelControlAppearance(this);
					buttonAppearanceCore.Changed += OnAppearanceChanged;
				}
				return buttonAppearanceCore;
			}
		}
		#endregion
		void OnAppearanceChanged(object sender, EventArgs e) {
			UpdateButtonPanel();
		}
		#region IButtonsPanelControlAppearanceOwner Members
		public IButtonsPanelControlAppearanceProvider CreateAppearanceProvider() {
			return new ButtonsPanelControlAppearanceProvider();
		}
		#endregion
		#region IAppearanceOwner Members
		bool IAppearanceOwner.IsLoading {
			get { return false; }
		}
		#endregion
		#region IButtonsPanelOwnerEx Members
		Padding marginCore;
		[ Category("Appearance")]
		public Padding ButtonBackgroundImageMargin {
			get { return marginCore; }
			set {
				if(ButtonBackgroundImageMargin != value) {
					marginCore = value;
					UpdateButtonPanel();
				}
			}
		}
		bool ShouldSerializeButtonBackgroundImageMargin() { return ButtonBackgroundImageMargin != Padding.Empty; }
		void ResetButtonBackgroundImageMargin() { Padding = Padding.Empty; }
		void IButtonsPanelOwner.Invalidate() {
			if(ButtonsPanel != null && ButtonsPanel.ViewInfo != null)
				Invalidate(ButtonsPanel.ViewInfo.Bounds);
		}
		bool IButtonsPanelOwnerEx.CanPerformClick(IBaseButton button) {
			return true;
		}
		#endregion
		#region IToolTipControlClient Members
		void ToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		public ToolTipControlInfo GetObjectInfo(Point point) {
			return ButtonsPanel.GetObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return ButtonsPanel.ShowToolTips; } }
		#endregion
	}
}
