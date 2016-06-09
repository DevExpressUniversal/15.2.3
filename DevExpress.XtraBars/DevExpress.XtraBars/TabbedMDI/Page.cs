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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
namespace DevExpress.XtraTabbedMdi {
	public class XtraMdiTabPage : IXtraTabPage, IXtraTabPageExt, IDisposable, IAnimatedItem, ISupportXtraAnimation {
		internal IXtraTab tabControl;
		Form mdiChild;
		Image image = null;
		string text = null;
		int maxTabPageWidthCore;
		string toolTip = string.Empty;
		string toolTipTitle = string.Empty;
		ToolTipIconType toolTipIconType = ToolTipIconType.None;
		int imageIndex = -1;
		bool cachedVisible = false;
		DefaultBoolean showClosePageButtonCore;
		DefaultBoolean showPinPageButtonCore;
		DefaultBoolean allowGlyphSkinningCore;
		bool allowPinCore;
		SuperToolTip superTip;
		public XtraMdiTabPage(IXtraTab tabControl, Form mdiChild)
			: this(tabControl, mdiChild, false) {
		}
		public XtraMdiTabPage(IXtraTab tabControl, Form mdiChild, bool useFormIconAsImage) {
			this.tabControl = tabControl;
			this.mdiChild = mdiChild;
			if(useFormIconAsImage)
				this.image = GetImage(mdiChild);
			SubscribeMdiChild();
			this.showClosePageButtonCore = DefaultBoolean.Default;
			this.showPinPageButtonCore = DefaultBoolean.Default;
			this.allowGlyphSkinningCore = DefaultBoolean.Default;
			this.allowPinCore = true;
			this.appearanceCore = new PageAppearance();
			this.pinnedCore = false;
			Appearance.Changed += OnAppearanceChanged;
		}
		public static Image GetImage(Form mdiChild) {
			return (mdiChild.Icon != null) ? mdiChild.Icon.ToBitmap() : null;
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageMdiChild")]
#endif
		public Form MdiChild { get { return mdiChild; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageTabControl")]
#endif
		public virtual IXtraTab TabControl { get { return tabControl; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get {
				return image;
			}
			set {
				if(Image == value) return;
				image = value;
				OnImageChanged();
			}
		}
		protected virtual void OnImageChanged() {
			UpdateImageHelper();
			PageChanged();
		}
		SuperToolTip defaultSuperTip;
		void UpdateDefaultSuperTip() {
			defaultSuperTip.Items.Clear();
			ToolTipItem item = new ToolTipItem();
			item.Text = XtraMdiChild.Text;
			defaultSuperTip.Items.Add(item);
		}
		bool IsMdiTabTextSet { get { return XtraMdiChild != null && !string.IsNullOrEmpty(XtraMdiChild.TextMdiTab); } }
		SuperToolTip GetDefaultSuperTip() {
			if(!IsMdiTabTextSet)
				return null;
			if(defaultSuperTip == null) {
				defaultSuperTip = new SuperToolTip();
			}
			UpdateDefaultSuperTip();
			return defaultSuperTip;
		}
		XtraForm XtraMdiChild { get { return MdiChild as XtraForm; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageSuperTip"),
#endif
 Editor("DevExpress.XtraEditors.Design.ToolTipContainerUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual SuperToolTip SuperTip {
			get {
				if(superTip == null) {
					return GetDefaultSuperTip();
				}
				return superTip;
			}
			set {
				superTip = value;
				PageChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageText")]
#endif
		public virtual string Text {
			get {
				if(text == null) {
					if(IsMdiTabTextSet)
						return XtraMdiChild.TextMdiTab;
					return MdiChild.Text;
				}
				else
					return text;
			}
			set {
				text = value;
				SubscribeMdiChild();
				PageChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageTooltip")]
#endif
		public virtual string Tooltip {
			get {
				if(string.IsNullOrEmpty(toolTip)) {
					if(IsMdiTabTextSet)
						return XtraMdiChild.Text;
				}
				return toolTip;
			}
			set {
				toolTip = value;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageTooltipTitle")]
#endif
		public virtual string TooltipTitle {
			get {
				return toolTipTitle;
			}
			set {
				toolTipTitle = value;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageTooltipIconType")]
#endif
		public virtual ToolTipIconType TooltipIconType {
			get {
				return toolTipIconType;
			}
			set {
				toolTipIconType = value;
			}
		}
		void IDisposable.Dispose() {
			PageAssociation.Remove(MdiChild);
			if(Appearance != null) {
				Appearance.Changed -= OnAppearanceChanged;
				Appearance.Dispose();
				appearanceCore = null;
			}
			UnsubscribeMdiChild();
			ClearImageHeleper();
		}
		void IXtraTabPage.Invalidate() {
			TabControl.Invalidate(TabControl.Bounds);
		}
		int IXtraTabPage.TabPageWidth { get { return 0; } }
		Padding IXtraTabPage.ImagePadding { get { return new Padding(0); } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageShowCloseButton")]
#endif
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ShowCloseButton {
			get { return showClosePageButtonCore; }
			set {
				if(ShowCloseButton == value) return;
				showClosePageButtonCore = value;
				PageChanged();
			}
		}
		DefaultBoolean IXtraTabPage.ShowCloseButton {
			get {
				if(!MdiChild.ControlBox) return DefaultBoolean.False;
				else return ShowCloseButton;
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageAllowGlyphSkinning")]
#endif
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinningCore; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinningCore = value;
				PageChanged();
			}
		}
		PageAppearance appearanceCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageAppearance"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PageAppearance Appearance {
			get { return appearanceCore; }
		}
		bool ShouldSerializeAppearance() {
			return Appearance.ShouldSerialize();
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			PageChanged();
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageMaxTabPageWidth"),
#endif
 Category(CategoryName.Appearance), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int MaxTabPageWidth {
			get {
				if(maxTabPageWidthCore == 0 && TabControl is XtraTabbedMdiManager)
					return ((XtraTabbedMdiManager)TabControl).MaxTabPageWidth;
				return maxTabPageWidthCore;
			}
			set {
				if(maxTabPageWidthCore == value) return;
				maxTabPageWidthCore = value;
				PageChanged();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPagePageEnabled")]
#endif
		public bool PageEnabled { get { return MdiChild.Enabled; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPagePageVisible")]
#endif
		public bool PageVisible { get { return MdiChild.Visible; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageImageIndex")]
#endif
		public virtual int ImageIndex {
			get { return this.imageIndex; }
			set {
				if(ImageIndex == value) return;
				this.imageIndex = value;
				OnImageIndexChanged();
			}
		}
		AnimatedImageHelper imageHelper;
		protected AnimatedImageHelper ImageHelper {
			get {
				if(imageHelper == null)
					imageHelper = new AnimatedImageHelper(Image);
				return imageHelper;
			}
		}
		protected virtual void OnImageIndexChanged() {
			UpdateImageHelper();
			PageChanged();
		}
		protected internal virtual void ClearImageHeleper() {
			ImageHelper.Image = null;
			StopAnimation();
		}
		protected internal virtual void UpdateImageHelper() {
			StopAnimation();
			ImageHelper.Image = GetImage();
			StartAnimation();
		}
		protected virtual Image GetImage() {
			if(Image != null) return Image;
			return ImageCollection.GetImageListImage(TabControl.Images, ImageIndex);
		}
		protected virtual void PageChanged() {
			bool fire = cachedVisible != PageVisible || PageVisible;
			cachedVisible = PageVisible;
			if(fire)
				TabControl.LayoutChanged();
		}
		protected virtual void SubscribeMdiChild() {
			UnsubscribeMdiChild();
			if(text == null)
				MdiChild.TextChanged += new EventHandler(MdiChild_Changed);
			MdiChild.EnabledChanged += new EventHandler(MdiChild_Changed);
			MdiChild.VisibleChanged += new EventHandler(MdiChild_Changed);
		}
		protected virtual void UnsubscribeMdiChild() {
			MdiChild.TextChanged -= new EventHandler(MdiChild_Changed);
			MdiChild.EnabledChanged -= new EventHandler(MdiChild_Changed);
			MdiChild.VisibleChanged -= new EventHandler(MdiChild_Changed);
		}
		protected virtual void MdiChild_Changed(object sender, EventArgs e) {
			this.PageChanged();
		}
		System.Drawing.Text.HotkeyPrefix IXtraTabPageExt.HotkeyPrefixOverride {
			get { return System.Drawing.Text.HotkeyPrefix.None; }
		}
		int IXtraTabPageExt.MaxTabPageWidth { get { return MaxTabPageWidth; } }
		bool pinnedCore;
		public bool Pinned {
			get { return pinnedCore; }
			set {
				if(Pinned == value) return;
				pinnedCore = value;
				if(tabControl is XtraTabbedMdiManager)
					((XtraTabbedMdiManager)tabControl).SortPinnedItems();
			}
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageAllowPin")]
#endif
		[DefaultValue(true)]
		public virtual bool AllowPin {
			get { return allowPinCore; }
			set {
				if(AllowPin == value) return;
				allowPinCore = value;
				PageChanged();
			}
		}
		bool IXtraTabPageExt.UsePinnedTab { get { return AllowPin; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageShowPinButton")]
#endif
		[DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean ShowPinButton {
			get { return showPinPageButtonCore; }
			set {
				if(ShowPinButton == value) return;
				showPinPageButtonCore = value;
				PageChanged();
			}
		}
		#region IAnimatedItem Members
		Rectangle IAnimatedItem.AnimationBounds {
			get { return TabControl.ViewInfo.HeaderInfo.AllPages[this].Image; }
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
		object IAnimatedItem.Owner {
			get { return TabControl; }
		}
		void IAnimatedItem.UpdateAnimation(BaseAnimationInfo info) {
			ImageHelper.UpdateAnimation(info);
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return ImageHelper.FramesCount > 1; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return TabControl as Control; }
		}
		#endregion
		public virtual void StopAnimation() {
			XtraAnimator.RemoveObject(this);
		}
		public virtual void StartAnimation() {
			IAnimatedItem animItem = this;
			if(TabControl == null || animItem.FramesCount < 2) return;
			XtraAnimator.Current.AddEditorAnimation(null, this, animItem, new CustomAnimationInvoker(OnImageAnimation));
		}
		protected virtual void OnImageAnimation(BaseAnimationInfo animInfo) {
			IAnimatedItem animItem = this;
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(GetImage() == null || TabControl == null || info == null) return;
			if(MdiChild == null || !MdiChild.Enabled || !MdiChild.Visible) return;
			if(!info.IsFinalFrame) {
				GetImage().SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				TabControl.Invalidate(animItem.AnimationBounds);
			}
			else {
				StopAnimation();
				StartAnimation();
			}
		}
		protected internal virtual void OnRemovedFromManager() {
			ClearImageHeleper();
		}
		protected internal void OnAddedToManager() {
			UpdateImageHelper();
		}
	}
	[ListBindable(false)]
	public class XtraMdiTabPageCollection : CollectionBase, IDisposable, IEnumerable<XtraMdiTabPage> {
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageCollectionItem")]
#endif
		public XtraMdiTabPage this[int index] { get { return (XtraMdiTabPage)List[index]; } }
		public int Add(XtraMdiTabPage page) {
			return List.Add(page);
		}
		public void Dispose() {
			XtraMdiTabPage[] pages = new XtraMdiTabPage[Count];
			List.CopyTo(pages, 0);
			for(int i = 0; i < pages.Length; i++) {
				((IDisposable)pages[i]).Dispose();
			}
			List.Clear();
			GC.SuppressFinalize(this);
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("XtraMdiTabPageCollectionItem")]
#endif
		public XtraMdiTabPage this[Form mdiChild] {
			get {
				foreach(XtraMdiTabPage page in this) {
					if(ReferenceEquals(mdiChild, page.MdiChild))
						return page;
				}
				return null;
			}
		}
		public void Remove(IXtraTabPage page) {
			List.Remove(page);
		}
		public int IndexOf(IXtraTabPage page) {
			return List.IndexOf(page);
		}
		public void Insert(int index, IXtraTabPage page) {
			int lastPiunnedTabIndex = GetLastPinnedTabIndex();
			if(lastPiunnedTabIndex != 0 && index < lastPiunnedTabIndex)
				index = !(page as IXtraTabPageExt).Pinned ? lastPiunnedTabIndex++ : index;
			List.Insert(index, page);
		}
		int GetLastPinnedTabIndex() {
			int result = 0;
			foreach(IXtraTabPage page in List) {
				if(page is IXtraTabPageExt && (page as IXtraTabPageExt).Pinned) {
					result++;
				}
			}
			return result;
		}
		public bool IsValid(int index) {
			return index >= 0 && index < List.Count;
		}
		IEnumerator<XtraMdiTabPage> IEnumerable<XtraMdiTabPage>.GetEnumerator() {
			foreach(XtraMdiTabPage page in List)
				yield return page;
		}
	}
}
