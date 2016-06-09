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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IOverviewContainerInfo : IContentContainerInfo {
		bool TryGetValue(Document document, out IDocumentInfo info);
		void Register(Document document);
		void Unregister(Document document);
	}
	class OverviewButton : BaseButton, IButton {
		public IOverviewContainerDefaultProperties OverviewContainerProperties { get; set; }
		public OverviewButton(string caption)
			: base() {
			Caption = caption;
			Appearance.Font = SegoeUIFontsCache.GetSegoeUIFont(6f);
		}
	}
	class OverviewContainerInfo : BaseContentContainerInfo, IOverviewContainerInfo, IButtonsPanelOwner, IInteractiveElementInfo {
		IDictionary<Document, IDocumentInfo> documentInfosCore;
		ButtonsPanel buttonsPanelCore;
		public OverviewContainerInfo(WindowsUIView view, OverviewContainer container)
			: base(view, container) {
			documentInfosCore = new Dictionary<Document, IDocumentInfo>();
			InitButtonPanel();
		}
		protected override void OnDispose() {
			Document[] documents = new Document[DocumentInfos.Count];
			DocumentInfos.Keys.CopyTo(documents, 0);
			for(int i = 0; i < documents.Length; i++)
				Unregister(documents[i]);
			base.OnDispose();
		}
		public void Register(Document document) {
			IDocumentInfo documentInfo;
			if(!DocumentInfos.TryGetValue(document, out documentInfo)) {
				documentInfo = CreateDocumentInfo(document);
				documentInfo.SetParentInfo(this);
				LayoutHelper.Register(this, documentInfo);
				DocumentInfos.Add(document, documentInfo);
				AddOverviewButton(document);
			}
		}
		void AddOverviewButton(Document document) {
			OverviewButton button = new OverviewButton(document.Caption);
			button.Tag = document;
			if(Container.Parent is IParentOverviewContainer) {
				button.OverviewContainerProperties = ((IParentOverviewContainer)Container.Parent).OverviewContainerProperties;
			}
			else
				button.OverviewContainerProperties = new OverviewContainerDefaultProperties(Owner.OverviewContainerProperties);
			ButtonsPanel.Buttons.Add(button);
		}
		void RemoveOverviewButton(Document document) {
			IButton button = GetButtonByDocument(document);
			if(button != null)
				ButtonsPanel.Buttons.Remove(button);
		}
		IButton GetButtonByDocument(Document document) {
			foreach(IButton button in ButtonsPanel.Buttons) {
				if(button.Properties.Tag == document)
					return button;
			}
			return null;
		}
		public void Unregister(Document document) {
			IDocumentInfo documentInfo;
			if(DocumentInfos.TryGetValue(document, out documentInfo)) {
				DocumentInfos.Remove(document);
				LayoutHelper.Unregister(this, documentInfo);
				documentInfo.SetParentInfo(null);
				Ref.Dispose(ref documentInfo);
				RemoveOverviewButton(document);
			}
		}
		protected internal ButtonsPanel ButtonsPanel {
			get { return buttonsPanelCore; }
		}
		protected void InitButtonPanel() {
			buttonsPanelCore = CreateButtonsPanel();
			ButtonsPanel.WrapButtons = true;
			ButtonsPanel.Orientation = System.Windows.Forms.Orientation.Vertical;
			ButtonsPanel.ButtonInterval = 10;
			ButtonsPanel.ContentAlignment = ContentAlignment.TopLeft;
			ButtonsPanel.Changed += EmbeddedButtonPanelChanged;
			ButtonsPanel.ButtonClick += OnButtonClick;
		}
		void EmbeddedButtonPanelChanged(object sender, EventArgs e) {
			if(Owner != null)
				Owner.LayoutChanged();
		}
		void OnButtonClick(object sender, ButtonEventArgs e) {
			Document document = e.Button.Properties.Tag as Document;
			if(document != null && Container.Parent != null) {
				IContentContainerInternal target = Container.Parent as IContentContainerInternal;
				if(target != null) {
					Owner.Controller.Activate(target);
					target.ActivateDocument(document);
				}
			}
		}
		protected virtual ButtonsPanel CreateButtonsPanel() {
			return new OverviewButtonsPanel(this);
		}
		protected virtual IDocumentInfo CreateDocumentInfo(Document document) {
			return new DocumentInfo(Owner, document);
		}
		protected IDictionary<Document, IDocumentInfo> DocumentInfos {
			get { return documentInfosCore; }
		}
		public bool TryGetValue(Document document, out IDocumentInfo info) {
			return DocumentInfos.TryGetValue(document, out info);
		}
		public override Type GetUIElementKey() {
			return typeof(IOverviewContainerInfo);
		}
		protected new OverviewContainerInfoPainter Painter {
			get { return base.Painter as OverviewContainerInfoPainter; }
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			Rectangle buttonsPanelBounds = CalcContentWithMargins(content);
			ButtonsPanel.ViewInfo.SetDirty();
			ButtonsPanel.ViewInfo.Calc(g, buttonsPanelBounds);
		}
		public object Images {
			get { return Owner.Manager.Images; }
		}
		public ObjectPainter GetPainter() {
			return new OverviewButtonsPanelSkinPainter(Owner.ElementsLookAndFeel);
		}
		public bool IsSelected {
			get { return false; }
		}
		public void Invalidate() {
			if(Owner != null)
				Owner.Invalidate();
		}
		public void Invalidate(Rectangle bounds) {
			if(Owner != null)
				Owner.Invalidate(bounds);
		}
		public void ProcessMouseDown(System.Windows.Forms.MouseEventArgs e) {
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseDown(e);
		}
		public void ProcessMouseMove(System.Windows.Forms.MouseEventArgs e) {
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseMove(e);
		}
		public void ProcessMouseUp(System.Windows.Forms.MouseEventArgs e) {
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseUp(e);
		}
		public void ProcessMouseLeave() {
			if(ButtonsPanel != null)
				ButtonsPanel.Handler.OnMouseLeave();
		}
		public void ProcessMouseWheel(System.Windows.Forms.MouseEventArgs e) { }
		#region IButtonsPanelOwner Members
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return false; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return false; }
		}
		DevExpress.XtraEditors.ButtonsPanelControl.ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return null; }
		}
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return true; }
		}
		#endregion
	}
	class OverviewContainerInfoPainter : ContentContainerInfoPainter {
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) {
			OverviewContainerInfo containerInfo = info as OverviewContainerInfo;
			ObjectPainter.DrawObject(cache,
				((IButtonsPanelOwner)containerInfo).GetPainter(),
				(ObjectInfoArgs)containerInfo.ButtonsPanel.ViewInfo);
		}
		public override Padding GetContentMargins() {
			return new Padding(40, 0, 40, 40);
		}
	}
	class OverviewContainerInfoSkinPainter : OverviewContainerInfoPainter {
		ISkinProvider providerCore;
		public OverviewContainerInfoSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		public override Padding GetContentMargins() {
			SkinElement overviewContainer = GetOverviewContainerElement();
			if(overviewContainer != null) {
				var edges = overviewContainer.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual SkinElement GetOverviewContainerElement() {
			return GetSkin()[MetroUISkins.SkinOverviewContainer];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore);
		}
	}
}
