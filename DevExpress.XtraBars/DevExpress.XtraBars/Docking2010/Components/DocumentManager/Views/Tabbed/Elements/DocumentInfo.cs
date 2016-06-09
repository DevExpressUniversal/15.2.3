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

using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface IDocumentInfo : IBaseDocumentInfo, IUIElement {
		Document Document { get; }
		IXtraTabPage TabPage { get; }
		IDocumentGroupInfo GroupInfo { get; }
		void SetGroupInfo(IDocumentGroupInfo info);
		void StartAnimation();
		void StopAnimation();
	}
	class DocumentInfo : BaseElementInfo, IXtraTabPage, IXtraTabPageExt, IDocumentInfo, IAnimatedItem, ISupportXtraAnimation {
		Document documentCore;
		public DocumentInfo(TabbedView owner, Document document)
			: base(owner) {
			documentCore = document;
			if(Document.IsAnimated)
				((IDocumentInfo)this).StartAnimation();
		}
		protected override void OnDispose() {
			LayoutHelper.Unregister(this);
			ClearAnimation();
			documentCore = null;
			base.OnDispose();
		}
		public override System.Type GetUIElementKey() {
			return typeof(IDocumentInfo);
		}
		void ClearAnimation() {
			XtraAnimator.RemoveObject(this);
			if(imageHelper != null && imageHelper.Image != null)
				imageHelper.Image = null;
		} 
		BaseDocument IBaseDocumentInfo.BaseDocument {
			get { return documentCore; }
		}
		public Document Document {
			get { return documentCore; }
		}
		public IDocumentGroupInfo GroupInfo {
			get { return groupInfoCore; }
		}
		#region IXtraTabPage Members
		PageAppearance IXtraTabPage.Appearance {
			get { return Document.Appearance; }
		}
		System.Drawing.Image IXtraTabPage.Image {
			get { return Document.GetActualImage(); }
		}
		int IXtraTabPage.ImageIndex {
			get { return Document.ImageIndex; }
		}
		System.Windows.Forms.Padding IXtraTabPage.ImagePadding {
			get { return new System.Windows.Forms.Padding(); }
		}
		void IXtraTabPage.Invalidate() {
			if(!IsDisposing) Owner.Invalidate();
		}
		bool IXtraTabPage.PageEnabled {
			get { return Document.IsEnabled; }
		}
		bool IXtraTabPage.PageVisible {
			get { return (Document != null) && Document.IsVisible; }
		}
		DevExpress.Utils.DefaultBoolean IXtraTabPage.ShowCloseButton {
			get {
				if(!Document.Properties.CanClose)
					return DefaultBoolean.False;
				if(!Document.IsDockPanel && !Document.IsFloatDocument)
					if(Document.Form != null && !Document.Form.ControlBox)
						return DevExpress.Utils.DefaultBoolean.False;
				return DevExpress.Utils.DefaultBoolean.Default;
			}
		}
		DefaultBoolean IXtraTabPage.AllowGlyphSkinning {
			get { return (Document != null) ? (Document.Properties.CanUseGlyphSkinning ? DefaultBoolean.True : DefaultBoolean.False) : DefaultBoolean.Default; }
		}
		IXtraTab IXtraTabPage.TabControl {
			get { return (Document != null) ? Document.Parent.GetTab() : null; }
		}
		int IXtraTabPage.TabPageWidth {
			get { return (Document != null) ? Document.Properties.ActualTabWidth : 0; }
		}
		int IXtraTabPageExt.MaxTabPageWidth { 
			get { return (Document != null) ? Document.Properties.ActualMaxTabWidth : 0; } 
		}
		bool IXtraTabPageExt.Pinned {
			get { return (Document != null) ? Document.Pinned : false; }
			set {
				if(Document != null) {
					Document.Pinned = value;
				}
			}
		}
		bool IXtraTabPageExt.UsePinnedTab { get { return Document != null ? Document.Properties.CanPin : false; } }
		DefaultBoolean IXtraTabPageExt.ShowPinButton { get { return Document != null && Document.Properties.CanShowPinButton ? DefaultBoolean.Default : DefaultBoolean.False; } }
		string IXtraTabPage.Text {
			get { return Document.Caption; }
		}
		string IXtraTabPage.Tooltip {
			get { return CalcTooltip(); }
		}
		protected string CalcTooltip() {
			if(Document == null) return null;
			string toolTip = Document.Tooltip;
			if(!Document.IsControlLoaded)
				toolTip = Document.Caption;
			if(string.IsNullOrEmpty(toolTip)) {
				var xtraForm = Document.Control as DevExpress.XtraEditors.XtraForm;
				if(xtraForm != null && !string.IsNullOrEmpty(xtraForm.TextMdiTab))
					toolTip = xtraForm.Text;
				else if(GroupInfo != null && GroupInfo.Tab.ViewInfo.HeaderInfo.AllPages[this].IsTextTrimming)
					toolTip = Document.Caption;
			}
			return toolTip;
		}
		DevExpress.Utils.ToolTipIconType IXtraTabPage.TooltipIconType {
			get { return (Document != null) ? Document.TooltipIconType : ToolTipIconType.None; }
		}
		string IXtraTabPage.TooltipTitle {
			get { return (Document != null) ? Document.TooltipTitle : null; }
		}
		DevExpress.Utils.SuperToolTip IXtraTabPage.SuperTip {
			get { return (Document != null) ? Document.SuperTip : null; }
		}
		#endregion
		#region IXtraTabPageExt Members
		System.Drawing.Text.HotkeyPrefix IXtraTabPageExt.HotkeyPrefixOverride {
			get { return System.Drawing.Text.HotkeyPrefix.None; }
		}
		#endregion
		#region IDocumentInfo Members
		IXtraTabPage IDocumentInfo.TabPage {
			get { return this; }
		}
		IDocumentGroupInfo groupInfoCore;
		void IDocumentInfo.SetGroupInfo(IDocumentGroupInfo info) {
			if(groupInfoCore == info) return;
			LayoutHelper.Unregister(this);
			groupInfoCore = info;
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return GroupInfo; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
		protected override void CalcCore(Graphics g, Rectangle bounds) { }
		protected override void UpdateStyleCore() {
			Document.UpdateStyle();
		}
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(Document.GetImageForAnimation());
				return imageHelper;
			}
		}
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds {
			get {return Bounds;}
		}
		int IAnimatedItem.AnimationInterval { get { return ImageHelper.AnimationInterval; } }
		int[] IAnimatedItem.AnimationIntervals { get { return ImageHelper.AnimationIntervals; } }
		DevExpress.Utils.Drawing.Animation.AnimationType IAnimatedItem.AnimationType { get { return ImageHelper.AnimationType; } }
		int IAnimatedItem.FramesCount { get { return ImageHelper.FramesCount; } }
		int IAnimatedItem.GetAnimationInterval(int frameIndex) {
			return ImageHelper.GetAnimationInterval(frameIndex);
		}
		bool IAnimatedItem.IsAnimated {
			get { return ImageHelper.IsAnimated; }
		}
		void IAnimatedItem.OnStart() { }
		void IAnimatedItem.OnStop() { }
		object IAnimatedItem.Owner {
			get { return this.Document.Manager.GetOwnerControl(); }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return ImageHelper.FramesCount > 1 && Document.Properties.CanAnimate; }
		}
		System.Windows.Forms.Control ISupportXtraAnimation.OwnerControl {
			get { return Document.Manager.GetOwnerControl(); }
		}
		#endregion
		void IDocumentInfo.StopAnimation() {
			XtraAnimator.RemoveObject(this);
			Document.IsAnimated = false;
		}
		void IDocumentInfo.StartAnimation() {
			IAnimatedItem animItem = this;
			if(Document != null || ImageHelper.Image != Document.Image)
				ImageHelper.Image = Document.Image;
			if(Document == null || animItem.FramesCount < 2) return;
			if(((ISupportXtraAnimation)this).CanAnimate)
				Document.IsAnimated = true;
			XtraAnimator.Current.AddEditorAnimation(null, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IDocumentInfo documentInfo = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(Document == null || Document.Image == null || info == null || !((ISupportXtraAnimation)this).CanAnimate) {
				documentInfo.StopAnimation();
				return;
			}
			if((Document.IsFloatDocument && Document.Form.Visible) || Document.IsFloating) return;
			if(!info.IsFinalFrame) {
				Document.Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				((IXtraTabPage)this).Invalidate();
			}
			else {
				documentInfo.StopAnimation();
				documentInfo.StartAnimation();
			}
		}
	}
}
