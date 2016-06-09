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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IDocumentSelectorInfo<T> :
		IDocumentGroupInfo<T>, IInteractiveElementInfo where T : DocumentSelector {
		IButtonsPanel ButtonsPanel { get; }
		bool ShowPageHeaders { get; }
		ToolTipControlInfo GetToolTipControlInfo(BaseViewHitInfo hitInfo);
		IDocumentSelectorButton DocumentSelectorButton { get; }
		IDocumentSelectorFlyoutPanelInfo DocumentSelectorFlyoutPanelInfo { get; }
	}
	abstract class DocumentSelectorInfo<T> : DocumentGroupInfo<T>, IDocumentSelectorInfo<T>, IButtonsPanelOwnerEx where T : DocumentSelector {
		ButtonsPanel buttonsPanelCore;
		ObjectPainter buttonsPanelPainter;
		IDocumentSelectorButton documentSelectorButton;
		IDocumentSelectorFlyoutPanelInfo documentSelectorFlyoutPanelInfo;
		public DocumentSelectorInfo(WindowsUIView view, T group)
			: base(view, group) {
			documentButtons = new Dictionary<Document, IBaseButton>();
			buttonsPanelCore = CreateButtonsPanel();
			ButtonsPanel.ButtonChecked += OnButtonChecked;
			ButtonsPanel.Changed += OnButtonsPanelChanged;
			InitilaizeButtonsPanel(Group.Items.ToArray());
			Group.SelectionChanged += Group_SelectionChanged;
			if(AllowShowDocumentSelectorButton) {
				documentSelectorButton = CreateDocumentSelectorButton(buttonsPanelCore);
				documentSelectorButton.CheckedChanged += OnDocumentSelectorButtonCheckedChanged;
				documentSelectorButton.Changed += OnDocumentSelectorButtonChanged;
				documentSelectorFlyoutPanelInfo = CreateDocumentSelectorFlyoutPanelInfo(view, buttonsPanelCore);
				documentSelectorFlyoutPanelInfo.ButtonsInterval = Painter.ButtonsInterval;
			}
		}
		protected override void OnDispose() {
			Group.SelectionChanged -= Group_SelectionChanged;
			ButtonsPanel.ButtonChecked -= OnButtonChecked;
			ButtonsPanel.Changed -= OnButtonsPanelChanged;
			if(AllowShowDocumentSelectorButton) {
				documentSelectorButton.CheckedChanged -= OnDocumentSelectorButtonCheckedChanged;
				documentSelectorButton.Changed -= OnDocumentSelectorButtonChanged;
				documentSelectorFlyoutPanelInfo.DetachContentControl();
				documentSelectorFlyoutPanelInfo.ProcessCancel();
				Ref.Dispose(ref documentSelectorButton);
				Ref.Dispose(ref documentSelectorFlyoutPanelInfo);
			}
			Ref.Dispose(ref buttonsPanelCore);
			base.OnDispose();
		}
		protected override void UpdateStyleCore() {
			base.UpdateStyleCore();
			buttonsPanelPainter = GetButtonsPanelPainter();
			if(DocumentSelectorButton != null)
				DocumentSelectorButton.UpdateStyle();
			if(documentSelectorFlyoutPanelInfo != null) {
				documentSelectorFlyoutPanelInfo.UpdateStyle();
				documentSelectorFlyoutPanelInfo.ButtonsInterval = Painter.ButtonsInterval;
			}
		}
		protected abstract ObjectPainter GetButtonsPanelPainter();
		protected override void ResetStyleCore() {
			buttonsPanelPainter = null;
			base.ResetStyleCore();
			if(DocumentSelectorButton != null)
				DocumentSelectorButton.ResetStyle();
			if(documentSelectorFlyoutPanelInfo != null)
				documentSelectorFlyoutPanelInfo.ResetStyle();
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new ButtonsPanel(this);
		}
		IButtonsPanel IDocumentSelectorInfo<T>.ButtonsPanel {
			get { return ButtonsPanel; }
		}
		bool IDocumentSelectorInfo<T>.ShowPageHeaders {
			get { return GetShowPageHeadersCore(); }
		}
		protected abstract bool GetShowPageHeadersCore();
		public ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		void Group_SelectionChanged(object sender, DocumentEventArgs e) {
			IBaseButton button = ButtonsPanel.Buttons.FindFirst(
				(btn) => ((DocumentButton)btn).Tag == e.Document);
			if(button != null) {
				lockButtonChecked++;
				button.Properties.Checked = true;
				lockButtonChecked--;
			}
		}
		void OnButtonsPanelChanged(object sender, EventArgs e) {
			if(Group != null)
				Group.LayoutChanged();
		}
		int lockButtonChecked = 0;
		bool CanPerformClick(IBaseButton button) {
			Document document = button.Properties.Tag as Document;
			if(document == null) return true;
			return Group.CanHeaderClick(document);
		}
		void OnButtonChecked(object sender, ButtonEventArgs e) {
			if(lockButtonChecked > 0) return;
			lockButtonChecked++;
			Document document = e.Button.Properties.Tag as Document;
			if(document != null) {
				UncheckDocumentSelectorButton();
				Owner.StartItemsNavigation(Group);
				Group.SetSelected(document);
				Owner.Manager.InvokePatchActiveChildren();
				Group.LayoutChanged();
			}
			lockButtonChecked--;
		}
		protected bool IsHorizontal {
			get { return Group.Properties.ActualOrientation == Orientation.Horizontal; }
		}
		protected new DocumentSelectorInfoPainter<T> Painter {
			get { return base.Painter as DocumentSelectorInfoPainter<T>; }
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			Document[] documents = Group.Items.ToArray();
			Rectangle pageContent = CalcContentWithMargins(content);
			for(int i = 0; i < documents.Length; i++) {
				IDocumentInfo documentInfo;
				if(DocumentInfos.TryGetValue(documents[i], out documentInfo))
					documentInfo.Calc(g, pageContent);
			}
		}
		protected void CalcButtonsPanel(Graphics g, Rectangle headersRect) {
			ButtonsPanel.ViewInfo.SetDirty();
			Size buttonsPanelMinSize = ButtonsPanel.ViewInfo.CalcMinSize(g);
			CheckDocumentSelectorButtonVisible(buttonsPanelMinSize, headersRect);
			Size buttonMinSize = CalcDocumentSelectorButtonMinSize(g);
			ButtonsPanel.ViewInfo.Calc(g, new Rectangle(headersRect.Location, new Size(headersRect.Width - buttonMinSize.Width, headersRect.Height - buttonMinSize.Height)));
			CalcDocumentSelectorButton(g, buttonsPanelMinSize, headersRect);
			if(documentSelectorFlyoutPanelInfo != null && documentSelectorFlyoutPanelInfo.IsShown)
				CalcDocumentSelectorFlyoutPanelInfo();			
		}
		protected override Rectangle GetBoundsCore(Document document) {
			IDocumentInfo documentInfo;
			if(DocumentInfos.TryGetValue(document, out documentInfo) && documentInfo.IsVisible)
				return documentInfo.Bounds;
			return Content;
		}
		void InitilaizeButtonsPanel(Document[] documents) {
			ButtonsPanel.BeginUpdate();
			ButtonsPanel.Buttons.Clear();
			ButtonsPanel.Orientation = GetButtonsPanelOrientation();
			ButtonsPanel.ContentAlignment = GetButtonsPanelContentAlignment();
			ButtonsPanel.WrapButtons = GetButtonsPanelWrap();
			ButtonsPanel.ButtonInterval = Painter.ButtonsInterval;
			bool inverseOrder = GetIsButtonsPanelInverseOrder();
			for(int i = 0; i < documents.Length; i++) {
				if(inverseOrder)
					RegisterButton(documents[(documents.Length - 1) - i]);
				else
					RegisterButton(documents[i]);
			}
			ButtonsPanel.CancelUpdate();
		}
		protected abstract bool GetIsButtonsPanelInverseOrder();
		protected abstract Orientation GetButtonsPanelOrientation();
		protected abstract ContentAlignment GetButtonsPanelContentAlignment();
		protected abstract bool GetButtonsPanelWrap();
		protected override void OnBeforeDocumentInfoRegistered(IDocumentInfo info) {
			base.OnBeforeDocumentInfoRegistered(info);
			ButtonsPanel.BeginUpdate();
			RegisterButton(info.Document, true);
			ButtonsPanel.CancelUpdate();
		}
		protected override void OnBeforeDocumentInfoUnRegistered(IDocumentInfo info) {
			ButtonsPanel.BeginUpdate();
			UnregisterButton(info.Document);
			ButtonsPanel.CancelUpdate();
			base.OnBeforeDocumentInfoUnRegistered(info);
		}
		IDictionary<Document, IBaseButton> documentButtons;
		void RegisterButton(Document document, bool canInsert) {
			IBaseButton documentButton;
			if(!documentButtons.TryGetValue(document, out documentButton)) {
				documentButton = new DocumentButton(document, document == Group.SelectedDocument);
				if(canInsert)
					ButtonsPanel.Buttons.Insert(0, documentButton);
				else
					ButtonsPanel.Buttons.Add(documentButton);
				documentButtons.Add(document, documentButton);
			}
		}
		void RegisterButton(Document document) {
			RegisterButton(document, false);
		}
		void UnregisterButton(Document document) {
			IBaseButton documentButton;
			if(documentButtons.TryGetValue(document, out documentButton)) {
				documentButtons.Remove(document);
				Ref.Dispose(ref documentButton);
			}
		}
		public void ProcessMouseDown(MouseEventArgs e) {
			if(IsDisposing) return;
			if(DocumentSelectorButton != null) DocumentSelectorButton.Handler.OnMouseDown(e);
			ButtonsPanel.Handler.OnMouseDown(e);
		}
		public void ProcessMouseMove(MouseEventArgs e) {
			if(IsDisposing) return;
			if(DocumentSelectorButton != null) DocumentSelectorButton.Handler.OnMouseMove(e);
			ButtonsPanel.Handler.OnMouseMove(e);
		}
		public void ProcessMouseUp(MouseEventArgs e) {
			if(IsDisposing) return;
			if(DocumentSelectorButton != null) DocumentSelectorButton.Handler.OnMouseUp(e);
			ButtonsPanel.Handler.OnMouseUp(e);
		}
		public void ProcessMouseLeave() {
			if(IsDisposing) return;
			if(DocumentSelectorButton != null) DocumentSelectorButton.Handler.OnMouseLeave();
			ButtonsPanel.Handler.OnMouseLeave();
		}
		public void ProcessMouseWheel(MouseEventArgs e) { }
		#region DocumentSelectorButton
		protected virtual bool AllowShowDocumentSelectorButton { get { return true; } }
		public IDocumentSelectorButton DocumentSelectorButton { 
			get { return documentSelectorButton; } 
		}
		void OnDocumentSelectorButtonChanged(object sender, EventArgs e) {
			System.ComponentModel.PropertyChangedEventArgs args = e as System.ComponentModel.PropertyChangedEventArgs;
			if(args == null || args.PropertyName != "Visible" || DocumentSelectorButton == null) return;
			if(!DocumentSelectorButton.Properties.Visible)
				DocumentSelectorFlyoutPanelCancel();
			else if(DocumentSelectorButton.Properties.Checked)
				DocumentSelectorFlyoutPanelShow();
		}
		IDocumentSelectorFlyoutPanelInfo IDocumentSelectorInfo<T>.DocumentSelectorFlyoutPanelInfo { 
			get { return documentSelectorFlyoutPanelInfo; } 
		}
		protected virtual IDocumentSelectorFlyoutPanelInfo CreateDocumentSelectorFlyoutPanelInfo(WindowsUIView owner, IButtonsPanel buttonsPanel) {
			return new DocumentSelectorFlyoutPanelInfo(owner, buttonsPanel); 
		}
		protected virtual IDocumentSelectorButton CreateDocumentSelectorButton(IButtonsPanel owner) { 
			return new DocumentSelectorButton(owner); 
		}
		void DocumentSelectorFlyoutPanelShow() {
			if(documentSelectorFlyoutPanelInfo == null) return;
			CalcDocumentSelectorFlyoutPanelInfo();
			documentSelectorFlyoutPanelInfo.ProcessShow(Point.Empty, true);
		}
		void DocumentSelectorFlyoutPanelCancel() {
			if(documentSelectorFlyoutPanelInfo == null) return;			
			documentSelectorFlyoutPanelInfo.ProcessCancel();
		}
		void OnDocumentSelectorButtonCheckedChanged(object sender, EventArgs e) {
			if(DocumentSelectorButton.Properties.Checked)
				DocumentSelectorFlyoutPanelShow();
			else {
				if(documentSelectorFlyoutPanelInfo.IsShown)
					DocumentSelectorFlyoutPanelCancel();
			}
		}
		protected virtual void CheckDocumentSelectorButtonVisible(Size buttonsPanelMinSize, Rectangle headersRect) {
			if(DocumentSelectorButton == null) return;
			DocumentSelectorButton.Properties.Visible = (headersRect.Width < buttonsPanelMinSize.Width || headersRect.Height < buttonsPanelMinSize.Height);
		}
		Size CalcDocumentSelectorButtonMinSize(Graphics g) {
			if(DocumentSelectorButton == null || !DocumentSelectorButton.Properties.Visible) return Size.Empty;
			if(!DocumentSelectorButton.Properties.Visible) return Size.Empty;
			Size minSize = DocumentSelectorButton.Info.CalcMinSize(g, DocumentSelectorButton.Painter);
			bool horz = GetButtonsPanelOrientation() != System.Windows.Forms.Orientation.Vertical;
			int buttonWidth = horz ? minSize.Width + Painter.ButtonsInterval : 0;
			int buttonHeight = horz ? 0 : minSize.Height + Painter.ButtonsInterval;
			return new Size(buttonWidth, buttonHeight);
		}
		protected virtual IBaseButton[] GetInvisibleButtons() {
			if(ButtonsPanel == null || ButtonsPanel.ViewInfo == null || !ButtonsPanel.ViewInfo.IsReady) return null;
			List<IBaseButton> buttons = new List<IBaseButton>(ButtonsPanel.Buttons);
			foreach(BaseButtonInfo info in ButtonsPanel.ViewInfo.Buttons) {
				if(buttons.Contains(info.Button))
					buttons.Remove(info.Button);
			}
			return buttons.ToArray();
		}
		void CalcDocumentSelectorFlyoutPanelInfo() {
			IBaseButton[] invisibleButtons = GetInvisibleButtons();
			if(invisibleButtons == null || documentSelectorFlyoutPanelInfo == null) return;
			documentSelectorFlyoutPanelInfo.AttachDocumentButtons(invisibleButtons);
			using(IMeasureContext context = Owner.BeginMeasure()) {
				documentSelectorFlyoutPanelInfo.SetDirty();
				Size size = documentSelectorFlyoutPanelInfo.CalcMinSize(context.Graphics);
				Point realLocation = CalcDocumentSelectorFlyoutPanelLocation(size);
				documentSelectorFlyoutPanelInfo.Calc(context.Graphics, new Rectangle(realLocation, size));
			}
		}
		protected virtual Point CalcDocumentSelectorFlyoutPanelLocation(Size documentSelectorMenuSize) { 
			return Point.Empty; 
		}
		protected void CalcDocumentSelectorButton(Graphics g, Size buttonsPanelMinSize, Rectangle headersRect) {
			if(DocumentSelectorButton == null || !DocumentSelectorButton.Properties.Visible) return;
			bool horz = GetButtonsPanelOrientation() != System.Windows.Forms.Orientation.Vertical;
			Rectangle bounds = CalcDocumentSelectorButtonCore(g, buttonsPanelMinSize, headersRect);
			DocumentSelectorButton.Info.Calc(g, DocumentSelectorButton.Painter, bounds.Location, bounds, horz);
		}
		protected virtual Rectangle CalcDocumentSelectorButtonCore(Graphics g, Size buttonsPanelMinSize, Rectangle headersRect) { 
			return Rectangle.Empty; 
		}
		void UncheckDocumentSelectorButton() {
			if(DocumentSelectorButton == null) return;
			DocumentSelectorButton.Properties.Checked = false;
		}
		public override bool DragMoveHitTest(Point point) {
			return base.DragMoveHitTest(point) && !DocumentSelectorButtonHitTest(point);
		}
		bool DocumentSelectorButtonHitTest(Point point) {
			return (DocumentSelectorButton != null) && DocumentSelectorButton.Properties.Visible && DocumentSelectorButton.HitTest(point);
		}
		#endregion
		#region IButtonsPanelOwner Members
		object IButtonsPanelOwner.Images {
			get { return (Owner.Manager != null) ? Owner.Manager.Images : null; }
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			return buttonsPanelPainter;
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return Owner.IsFocused; }
		}
		void IButtonsPanelOwner.Invalidate() {
			if(!IsDisposing && ButtonsPanel.ViewInfo != null) Owner.Invalidate(ButtonsPanel.ViewInfo.Bounds);
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return Group.Properties.CanHtmlDrawHeaders; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return Group.AppearanceHeaderButton; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return Group.HeaderButtonBackgroundImages; }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return true; }
		}
		#endregion
		protected abstract class DocumentSelectorHeaderInfo : ContentContainerHeaderInfo {
			public DocumentSelectorHeaderInfo(WindowsUIView view, IDocumentSelectorInfo<T> containerInfo)
				: base(view, containerInfo) { }
			public new IDocumentSelectorInfo<T> ContainerInfo {
				get { return base.ContainerInfo as IDocumentSelectorInfo<T>; }
			}
		}
		#region IButtonsPanelOwnerEx Members
		Padding IButtonsPanelOwnerEx.ButtonBackgroundImageMargin {
			get { return Padding.Empty; }
			set { }
		}
		bool IButtonsPanelOwnerEx.CanPerformClick(IBaseButton button) {
			return CanPerformClick(button);
		}
		#endregion
		#region IDocumentSelectorInfo<T> Members
		ToolTipControlInfo IDocumentSelectorInfo<T>.GetToolTipControlInfo(BaseViewHitInfo hitInfo) {
			return (buttonsPanelCore as IToolTipControlClient).GetObjectInfo(hitInfo.HitPoint);
		}
		#endregion
	}
	class DocumentButton : BaseButton, IButton {
		Document documentCore;
		public DocumentButton(Document document, bool @checked) {
			this.documentCore = document;
			Checked = @checked;
		}
		public Document Document {
			get { return documentCore; }
		}
		public override int GroupIndex {
			get { return 0; }
			set { }
		}
		public override object Tag {
			get { return documentCore; }
			set { }
		}
		public override string Caption {
			get { return string.IsNullOrEmpty(documentCore.Header) ? documentCore.Caption : documentCore.Header; }
			set { }
		}
		public override Image Image {
			get { return documentCore.Image; }
			set { }
		}
		public override ButtonStyle Style {
			get { return ButtonStyle.CheckButton; }
			set { }
		}
		public override int ImageIndex {
			get { return documentCore.ImageIndex; }
			set { }
		}
		public override bool Enabled {
			get { return base.Enabled && Document.Properties.CanActivate; }
			set { base.Enabled = value; }
		}
	}
	abstract class DocumentSelectorInfoPainter<T> : ContentContainerInfoPainter where T : DocumentSelector {
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) {
			IDocumentSelectorInfo<T> selectorInfo = info as IDocumentSelectorInfo<T>;
			if(selectorInfo != null && selectorInfo.ShowPageHeaders) {
				ObjectPainter.DrawObject(cache, ((IButtonsPanelOwner)selectorInfo).GetPainter(),
				  (ObjectInfoArgs)selectorInfo.ButtonsPanel.ViewInfo);
				DrawDocumentSelectorButton(cache, selectorInfo.DocumentSelectorButton);
				if(selectorInfo.DocumentSelectorFlyoutPanelInfo != null && selectorInfo.DocumentSelectorFlyoutPanelInfo.IsShown)
					selectorInfo.DocumentSelectorFlyoutPanelInfo.Invalidate();
			}
		}
		protected virtual void DrawDocumentSelectorButton(GraphicsCache cache, IDocumentSelectorButton button) {
			if(button == null || !button.Properties.Visible) return;
			CalcDocumentSelectorButtonState(button);
			ObjectPainter.DrawObject(cache, button.Painter, (ObjectInfoArgs)button.Info);
		}
		void CalcDocumentSelectorButtonState(IDocumentSelectorButton button) {
			ObjectState state = ObjectState.Normal;
			if(!button.Properties.Enabled)
				state |= ObjectState.Disabled;
			if(button == button.Handler.HotButton)
				state |= ObjectState.Hot;
			if(button == button.Handler.PressedButton)
				state |= ObjectState.Pressed;
			if(button.IsChecked.HasValue && button.IsChecked.Value && !(button is DefaultButton))
				state |= ObjectState.Pressed;
			button.Info.State = state;
		}
		public virtual int ButtonsInterval {
			get { return 4; }
		}
	}
	abstract class DocumentSelectorInfoSkinPainter<T> : DocumentSelectorInfoPainter<T> where T : DocumentSelector {
		ISkinProvider providerCore;
		public DocumentSelectorInfoSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		public override int ButtonsInterval {
			get {
				object interval = GetButtonsIntervalProperty();
				if(interval != null)
					return (int)interval;
				return base.ButtonsInterval;
			}
		}
		public override Padding GetContentMargins() {
			SkinElement documentSelector = GetDocumentSelectorElement();
			if(documentSelector != null) {
				var edges = documentSelector.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual object GetButtonsIntervalProperty() {
			return GetSkin().Properties[MetroUISkins.PageGroupButtonsInterval];
		}
		protected virtual SkinElement GetDocumentSelectorElement() {
			return GetSkin()[MetroUISkins.SkinPageGroup];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore);
		}
	}
}
