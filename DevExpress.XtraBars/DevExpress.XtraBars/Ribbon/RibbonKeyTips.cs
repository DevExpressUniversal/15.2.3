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
using System.Text;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Utils.Drawing;
using System.Diagnostics;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.Skins;
using DevExpress.XtraBars.ViewInfo;
using System.Drawing.Imaging;
using System.Media;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars.Ribbon {
	public interface ISupportRibbonKeyTip {
		string ItemCaption { get; }
		string ItemKeyTip { get; set; }
		string ItemUserKeyTip { get; set; }
		Point ShowPoint { get; }
		ContentAlignment Alignment { get; }
		int FirstIndex { get; set; }
		void Click();
		bool KeyTipEnabled { get; }
		bool HasDropDownButton { get; }
		bool KeyTipVisible { get; }
		bool IsCommandItem { get; }
	}
	public interface IHasRibbonKeyTipManager {
		RibbonBaseKeyTipManager KeyTipManager { get; }
	}
	public interface IKeyTipsOwnerControl {
		bool ShoulShowKeyTips { get; set; }		
		void ShowKeyTips();
	}
	public class BaseRibbonKeyTipItem : ISupportRibbonKeyTip { 
		RibbonControl ribbon;
		string itemKeyTip = string.Empty;
		string itemUserKeyTip = string.Empty;
		int firstIndex = 0;
		public BaseRibbonKeyTipItem(RibbonControl ribbon) {
			this.ribbon = ribbon;
		}
		[Browsable(false)]
		public RibbonControl Ribbon { get { return ribbon; } }
		protected virtual string ItemCaptionCore { get { return string.Empty; } }
		protected virtual string ItemUserKeyTipCore { get { return itemUserKeyTip; } set { itemUserKeyTip = value; } }
		protected virtual string ItemKeyTipCore { get { return itemKeyTip; } set { itemKeyTip = value; } }
		protected virtual ContentAlignment AlignmentCore { get { return ContentAlignment.MiddleCenter; } }
		protected virtual bool KeyTipEnabledCore { get { return true; } }
		protected virtual bool KeyTipVisibleCore { get { return true; } }
		protected virtual int FirstIndexCore { get { return firstIndex; } set { firstIndex = value; } }
		protected virtual Point ShowPointCore { get { return Point.Empty; } }
		protected virtual void ClickCore() { }
		protected virtual bool HasDropDownButtonCore { get { return false; } }
		protected virtual bool IsCommandItemCore { get { return false; } }
		[Browsable(false)]
		string ISupportRibbonKeyTip.ItemCaption { get { return ItemCaptionCore; } }
		string ISupportRibbonKeyTip.ItemUserKeyTip { 
			get { return ItemUserKeyTipCore; } 
			set { 
				if(value != null)ItemUserKeyTipCore = value.ToUpper();
				ItemUserKeyTipCore = string.Empty;
			} 
		}
		[Browsable(false)]
		string ISupportRibbonKeyTip.ItemKeyTip { get { return ItemKeyTipCore; } set { ItemKeyTipCore = value; } }
		[Browsable(false)]
		ContentAlignment ISupportRibbonKeyTip.Alignment { get { return AlignmentCore; } }
		[Browsable(false)]
		bool ISupportRibbonKeyTip.KeyTipEnabled { get { return KeyTipEnabledCore; } }
		[Browsable(false)]
		int ISupportRibbonKeyTip.FirstIndex { get { return FirstIndexCore; } set { FirstIndexCore = value; } }
		[Browsable(false)]
		Point ISupportRibbonKeyTip.ShowPoint { get { return ShowPointCore; } }
		void ISupportRibbonKeyTip.Click() { ClickCore(); }
		[Browsable(false)]
		bool ISupportRibbonKeyTip.HasDropDownButton { get { return HasDropDownButtonCore; } }
		[Browsable(false)]
		bool ISupportRibbonKeyTip.KeyTipVisible { get { return KeyTipVisibleCore; } }
		[Browsable(false)]
		bool ISupportRibbonKeyTip.IsCommandItem { get { return IsCommandItemCore; } }
	}
	public class DropDownButtonKeyTipItem : BaseRibbonKeyTipItem {
		BarButtonItemLink link;
		public DropDownButtonKeyTipItem(RibbonControl ribbon, BarButtonItemLink link) : base(ribbon) {
			this.link = link;
		}
		public BarButtonLinkViewInfo LinkInfo { 
			get {
				if (ButtonLink == null) return null;
				return ButtonLink.LinkViewInfo as BarButtonLinkViewInfo;
			} 
		}
		public RibbonItemViewInfo ItemInfo { 
			get {
				if (ButtonLink == null) return null;
				return ButtonLink.RibbonItemInfo;
			} 
		}
		public RibbonSplitButtonItemViewInfo SplitButtonInfo { get { return ItemInfo as RibbonSplitButtonItemViewInfo; } }
		public RibbonButtonItemViewInfo ButtonInfo { get { return ItemInfo as RibbonButtonItemViewInfo; } }
		public BarButtonItemLink ButtonLink { get { return link; } }
		public bool InMenu { 
			get {
				if (ButtonLink == null) return false;
				return ButtonLink.BarControl is CustomLinksControl;
			} 
		}
		protected override Point ShowPointCore {
			get {
				Point pt = Point.Empty;
				if(SplitButtonInfo != null) {
					if(SplitButtonInfo.CurrentLevel == RibbonItemStyles.Large) {
						pt.X = SplitButtonInfo.Bounds.X + SplitButtonInfo.Bounds.Width / 2;
						pt.Y = ButtonLink.GetKeyTipYPos(2);
						pt = Ribbon.PointToScreen(pt);
					}
				}
				else if(InMenu) {
					pt.X = LinkInfo.Rects[BarLinkParts.OpenArrow].X + LinkInfo.Rects[BarLinkParts.OpenArrow].Width / 2;
					pt.Y = LinkInfo.Bounds.Y + LinkInfo.Bounds.Height / 2;
					pt = LinkInfo.Link.BarControl.PointToScreen(pt);
				}
				return pt;
			}
		}
		protected override ContentAlignment AlignmentCore {
			get {
				if(SplitButtonInfo != null && SplitButtonInfo.IsLargeButton && SplitButtonInfo.CurrentLevel == RibbonItemStyles.Large) {
					return ContentAlignment.TopCenter;
				}
				else if(InMenu)
					return ContentAlignment.TopLeft;
				return base.AlignmentCore;
			}
		}
		protected override void ClickCore() {
			if(ItemInfo == null && LinkInfo == null) return;
			ButtonLink.KeyTipDropDownClick();
		}
		protected override string ItemUserKeyTipCore {
			get { return ButtonLink.DropDownKeyTip; }
			set { ButtonLink.DropDownKeyTip = value; }
		}
		protected override string ItemCaptionCore {
			get {
				if(ButtonLink != null) return ButtonLink.Caption;
				return base.ItemCaptionCore;
			}
		}
		protected override bool KeyTipEnabledCore {
			get { return link.Enabled; }
		}
	}
	public class RibbonApplicationButtonKeyTipItem : BaseRibbonKeyTipItem {
		public RibbonApplicationButtonKeyTipItem(RibbonControl ribbon) : base(ribbon) { }
		protected override string ItemCaptionCore { get { return "ApplicationButton"; } }
		protected override Point ShowPointCore {
			get {
				Point pt = Ribbon.ViewInfo.ApplicationButton.Bounds.Location;
				pt.Offset(Ribbon.ViewInfo.ApplicationButton.Bounds.Width / 2, Ribbon.ViewInfo.ApplicationButton.Bounds.Height / 2 - 1);
				if(Ribbon.IsOffice2010LikeStyle) {
					if(Ribbon.Pages.VisiblePages.Count > 0)
						pt.Y = Ribbon.PointToClient(((ISupportRibbonKeyTip)Ribbon.Pages.VisiblePages[0]).ShowPoint).Y;
				}
				return Ribbon.PointToScreen(pt); 
			}
		}
		protected override ContentAlignment AlignmentCore {
			get {
				if(Ribbon.IsOffice2010LikeStyle) {
					if(Ribbon.Pages.VisiblePages.Count > 0)
						return ContentAlignment.TopCenter;
					else
						return ContentAlignment.MiddleCenter;
				}
				return base.AlignmentCore;
			}
		}
		protected override string ItemUserKeyTipCore {
			get { return Ribbon.ApplicationButtonKeyTip; }
			set { Ribbon.ApplicationButtonKeyTip = value; }
		}
		protected override void ClickCore() {
			Ribbon.Invalidate(Ribbon.ViewInfo.ApplicationButton.Bounds);
			base.ClickCore();
			if(Ribbon == null) return;
			((RibbonHandler)Ribbon.Handler).ShowApplicationButtonPopup();
			Ribbon.KeyTipManager.HideKeyTips();
			if(Ribbon.ApplicationButtonPopupControl == null || Ribbon.ApplicationButtonPopupControl.IPopup == null) {
				if(Ribbon.ApplicationButtonDropDownControl != null)
					Ribbon.KeyTipManager.ActivateApplicationButtonControlKeyTips();
				IKeyTipsOwnerControl keyTipsOwner = Ribbon.ApplicationButtonDropDownControl as IKeyTipsOwnerControl;
				if(keyTipsOwner != null) {
					if(Ribbon.RibbonStyle == RibbonControlStyle.Office2013) {
						keyTipsOwner.ShoulShowKeyTips = true;
						return;
					}
					keyTipsOwner.ShowKeyTips();
				}
				return;
			}
			CustomLinksControl linksControl = Ribbon.ApplicationButtonPopupControl.IPopup.CustomControl as CustomLinksControl;
			if(linksControl == null) return;
			linksControl.KeyTipManager.Ribbon = Ribbon;
			linksControl.KeyTipManager.Parent = Ribbon.KeyTipManager;
			linksControl.KeyTipManager.ActivateKeyTips();
		}
	}
	public class ItemWithKeyTipCollection : CollectionBase {
		public int Add(ISupportRibbonKeyTip item) { return List.Add(item); }
		public void Insert(int index, ISupportRibbonKeyTip item) { List.Insert(index, item); }
		public ISupportRibbonKeyTip this[int index] { get { return List[index] as ISupportRibbonKeyTip; } set { List[index] = value; } }
		public int IndexOf(object item) { return List.IndexOf(item); }
	}
	public class NumericCharCollection : CollectionBase {
		public int Add(char c) { return List.Add(c); }
		public void Insert(int index, char c) { List.Insert(index, c); }
		public char this[int index] { get { return (char)List[index]; } set { List[index] = value; } }
	}
	public class NumericKeyTip : CollectionBase {
		int firstCount, secondCount;
		int firstIndex, secondIndex, thirdIndex;
		public NumericKeyTip() {
			ResetIndiciesAndCounts();
		}
		public int Add(NumericCharCollection coll) { return List.Add(coll); }
		public void Insert(int index, NumericCharCollection coll) { List.Insert(index, coll); }
		public NumericCharCollection this[int index] { get { return List[index] as NumericCharCollection; } set { List[index] = value; } }
		public void Remove(NumericCharCollection coll) { List.Remove(coll); }
		void ResetIndiciesAndCounts() {
			this.firstCount = this.secondCount = 0;
			this.firstIndex = this.secondIndex = this.thirdIndex = 0;
		}
		public void GenerateKeyTipChars(BaseKeyTipManager manager) {
			ResetIndiciesAndCounts();
			Add(GenerateFirstDigits(manager));
			if(this[0].Count == 0) {
				Remove(this[0]);
				GenerateDigitsCount(manager);
				return;
			}
			Add(GenerateSecondDigits(manager));
			if(this[1].Count == 0) {
				Remove(this[1]);
				GenerateDigitsCount(manager);
				return;
			}
			Add(GenerateThirdDigits(manager));
			if(this[2].Count == 0) Remove(this[2]);
			GenerateDigitsCount(manager);
		}
		NumericCharCollection GenerateFirstDigits(BaseKeyTipManager manager) {
			NumericCharCollection coll = new NumericCharCollection();
			string s;
			for(int i = 0; i < 10; i++) {
				s = i.ToString();
				if(!manager.IsUserKeyTipCollision(s)) coll.Add(s[0]);
			}
			return coll;
		}
		NumericCharCollection GenerateSecondDigits(BaseKeyTipManager manager) {
			NumericCharCollection coll = new NumericCharCollection();
			string s;
			int i, j;
			for(i = 0; i < 10; i++) {
				s = "";
				for(j = 0; j < this[0].Count; j++) {
					s = this[0][j].ToString() + i.ToString();
					if(manager.IsUserKeyTipCollision(s)) break;
				}
				if(j != this[0].Count) continue;
				coll.Add(s[1]);
			}
			return coll;
		}
		NumericCharCollection GenerateThirdDigits(BaseKeyTipManager manager) {
			NumericCharCollection coll = new NumericCharCollection();
			string s;
			for(int i = 0; i < 10; i++) {
				s = i.ToString();
				coll.Add(s[0]);
			}
			return coll;
		}
		public int FirstCount { get { return firstCount; } }
		public int SecondCount { get { return secondCount; } }
		public int FirstIndex { get { return firstIndex; } }
		public int SecondIndex { get { return secondIndex; } }
		public int ThirdIndex { get { return thirdIndex; } }
		void CountFirst() { firstIndex++; }
		void UpdateFirstIndex() {
			if(FirstIndex == this[0].Count) {
				firstIndex = FirstCount;
				secondIndex = SecondCount;
			}
		}
		void CountSecond() {
			secondIndex++;
			if(SecondIndex == SecondCount) {
				secondIndex = 0;
				firstIndex++;
			}
			UpdateFirstIndex();
		}
		void CountThird() {
			thirdIndex++;
			if(ThirdIndex == this[2].Count) {
				thirdIndex = 0;
				secondIndex++;
			}
			if(SecondIndex == this[1].Count) {
				secondIndex = SecondCount;
				firstIndex++;
			}
			UpdateFirstIndex();
		}
		public string GetNextKeyTip() {
			string s;
			if(FirstIndex < FirstCount) {
				s = this[0][FirstIndex].ToString();
				CountFirst();
			}
			else if(SecondIndex < SecondCount) {
				s = this[0][FirstIndex].ToString() + this[1][SecondIndex].ToString();
				CountSecond();
			}
			else {
				s = this[0][FirstIndex].ToString() + this[1][SecondIndex].ToString() + this[2][ThirdIndex].ToString();
				CountThird();
			}
			return s;
		}
		void GenerateDigitsCount(BaseKeyTipManager manager) {
			if(Count == 0) return;
			if(Count == 1) {
				this.firstCount = this[0].Count;
				return;
			}
			int itemsCount = manager.GetItemsWithEmptyKeyTip();
			int i = 0, j = 0, n;
			int beginIndex = itemsCount < 20 ? 0 : this[0].Count / 2;
			for(i = beginIndex; i < this[0].Count; i++) {
				for(j = 0; j < this[1].Count; j++) {
					n = (this[0].Count - i) + i * (this[1].Count - j);
					if(Count == 3) n += i * j * this[2].Count;
					if(n > itemsCount) break;
				}
				if(j != this[1].Count) break;
			}
			this.firstCount = this[0].Count - i;
			this.secondCount = this[1].Count - j;
			if(firstCount == this[0].Count) this.firstCount--;
			if(secondCount == this[1].Count) this.secondCount--;
		}
	}
	public class KeyTipFormViewInfo {
		KeyTipForm form;
		AppearanceObject paintAppearance;
		public KeyTipFormViewInfo(KeyTipForm form) {
			this.form = form;
			this.paintAppearance = null;	   
		}
		protected virtual AppearanceObject CreateAppearance() {
			return new AppearanceObject();
		}
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearance == null) paintAppearance = CreateAppearance();
				return paintAppearance;
			}
		}
		public KeyTipForm Form { get { return form; } }
		protected virtual void UpdatePaintAppearance() {
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Form.Appearance }, AppearanceDefault);
			PaintAppearance.TextOptions.HAlignment = HorzAlignment.Center;
		}
		protected virtual SkinElementInfo GetToolTipSkin() { 
			return new SkinElementInfo(RibbonSkins.GetSkin(Provider)[DevExpress.Skins.RibbonSkins.SkinKeyTipWindow]);
		}
		protected virtual AppearanceDefault AppearanceDefault {
			get { 
				SkinElementInfo info = GetToolTipSkin();
				if(info != null && info.Element != null) {
					AppearanceDefault res = info.Element.GetAppearanceDefault();
					if(res.ForeColor.IsEmpty)
						res.ForeColor = Color.Black;
					return res;
				}
				return new AppearanceDefault();
			}
		}
		public virtual void CalcViewInfo() {
			UpdatePaintAppearance();	
		}
		protected internal virtual ISkinProvider Provider {
			get {
				if(Form == null || Form.Ribbon == null || Form.Ribbon.GetController() == null) return null;
				return Form.Ribbon.GetController().LookAndFeel;
			}
		}
		public virtual Rectangle Bounds { get { return new Rectangle(Point.Empty, Form.Size); } }
	}
	public class KeyTipFormPainter : ObjectPainter {
		public virtual Size GetFormSize(KeyTipFormInfoArgs ki, Rectangle rect) {
			SkinElementInfo info = GetFormSkin(ki, rect);
			if(info == null) return Size.Empty;
			info.Bounds = SkinElementPainter.CalcBoundsByClientRectangle(ki.Graphics, SkinElementPainter.Default, info, rect);
			return info.Bounds.Size;
		}
		public virtual SkinElementInfo GetFormSkin(KeyTipFormInfoArgs ki, Rectangle rect) {
			SkinElement ski = RibbonSkins.GetSkin(ki.ViewInfo.Provider)[DevExpress.Skins.RibbonSkins.SkinKeyTipWindow];
			if(ski == null) return null;
			return new SkinElementInfo(ski, rect);
		}
		protected virtual void DrawBackground(KeyTipFormInfoArgs ki) {
			SkinElementInfo wi = GetFormSkin(ki, ki.ViewInfo.Bounds);
			if(wi == null) {
				ki.ViewInfo.PaintAppearance.FillRectangle(ki.Cache, ki.ViewInfo.Bounds);
				ki.Cache.DrawRectangle(new Pen(ki.ViewInfo.PaintAppearance.ForeColor), ki.ViewInfo.Bounds);
			}
			else
				SkinElementPainter.DrawObject(ki.Cache, new SkinElementPainter(), wi);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			KeyTipFormInfoArgs ki = e as KeyTipFormInfoArgs;
			if(ki == null) return;
			DrawBackground(ki);
			DrawKeyTipText(ki);
		}
		protected virtual void DrawKeyTipText(KeyTipFormInfoArgs ki) {
			Rectangle textRect = ki.ViewInfo.Bounds;
			SkinElementInfo info = GetFormSkin(ki, textRect);
			if(info == null) return;
			textRect.Width -= info.Element.ContentMargins.Width;
			textRect.Height -= info.Element.ContentMargins.Height;
			textRect.Offset(new Point(info.Element.ContentMargins.Left, info.Element.ContentMargins.Top));
			ki.ViewInfo.PaintAppearance.DrawString(ki.Cache, ki.Form.Text, textRect);
		}
	}
	public class KeyTipFormInfoArgs : ObjectInfoArgs {
		KeyTipForm form;
		public KeyTipFormInfoArgs(KeyTipForm form, GraphicsCache cache) : base(cache, new Rectangle(Point.Empty, form.Size), ObjectState.Normal) {
			this.form = form;
		}
		public KeyTipForm Form { get { return form; } }
		public KeyTipFormViewInfo ViewInfo { get { return Form.ViewInfo; } }
	}
	public class KeyTipPreviewControl : Control {
		KeyTipForm form;
		public KeyTipPreviewControl(RibbonControl ribbon) {
			form = new KeyTipForm(ribbon);
			form.Text = "DX";
			form.Size = new RibbonBaseKeyTipManager(form.Ribbon).CalcFormSize(form, form.Text);
		}
		public override string Text {
			get {
				return base.Text;
			}
			set {
				base.Text = value;
				form.Text = Text;
				Size = new RibbonBaseKeyTipManager(form.Ribbon).CalcFormSize(form, Text);
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateRegion();
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(IsHandleCreated) UpdateRegion();
		}
		protected virtual void UpdateRegion() {
			Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, Size), 2);			
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			specified |= BoundsSpecified.Width | BoundsSpecified.Height;
			base.SetBoundsCore(x, y, form.Width, form.Height, specified);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Size = form.Size = new RibbonBaseKeyTipManager(form.Ribbon).CalcFormSize(form, form.Text);
			using(GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e))) {
				form.Painter.DrawObject(new KeyTipFormInfoArgs(form, cache));
			}
		}
	}
	public class KeyTipForm : TopFormBase {
		RibbonControl ribbon;
		KeyTipFormViewInfo viewInfo;
		KeyTipFormPainter painter;
		AppearanceObject appearance;
		public KeyTipForm(RibbonControl ribbon) {
			this.ribbon = ribbon;
			this.viewInfo = null;
			this.painter = null;
			this.appearance = null;
		}
		public RibbonControl Ribbon { get { return ribbon; } set { ribbon = value; } }
		protected virtual KeyTipFormViewInfo CreateViewInfo() { return new KeyTipFormViewInfo(this); }
		protected virtual KeyTipFormPainter CreatePainter() { return new KeyTipFormPainter(); }
		protected virtual AppearanceObject CreateAppearance() { return new AppearanceObject(); }
		public KeyTipFormViewInfo ViewInfo { 
			get {
				if(viewInfo == null) viewInfo = CreateViewInfo();
				return viewInfo;
			} 
		}
		public KeyTipFormPainter Painter {
			get {
				if(painter == null) painter = CreatePainter();
				return painter;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			UpdateRegion();
		}
		protected virtual void UpdateRegion() {
			Region = NativeMethods.CreateRoundRegion(new Rectangle(Point.Empty, Size), 2);
		}
		public AppearanceObject Appearance {
			get {
				if(appearance == null) appearance = CreateAppearance();
				return appearance;
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(IsHandleCreated)UpdateRegion();
			LayoutChanged();
		}
		protected override void OnLocationChanged(EventArgs e) {
			base.OnLocationChanged(e);
			LayoutChanged();
		}
		protected virtual void LayoutChanged() {
			ViewInfo.CalcViewInfo();
			Invalidate();
		}
		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e))) {
				Painter.DrawObject(new KeyTipFormInfoArgs(this, cache));
			}
		}
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			base.WndProc(ref m);
			if(m.Msg == 0x84) {
				m.Result = new IntPtr(-1);
			}
		}
		public virtual Size CalcBestSize(GraphicsInfo gInfo) {
			using(GraphicsCache cache = new GraphicsCache(gInfo.Graphics)) {
				Rectangle rect = new Rectangle(Point.Empty, Ribbon.ViewInfo.PaintAppearance.PageHeader.CalcTextSize(gInfo.Graphics, Text, 0).ToSize());
				return Painter.GetFormSize(new KeyTipFormInfoArgs(this, cache), rect);
			}
		}
	}
	public class ContainerKeyTipManager : RibbonBaseKeyTipManager {
		BarItemLinkReadOnlyCollection links;
		RibbonBaseKeyTipManager parent;
		CustomLinksControl linksControl;		
		public ContainerKeyTipManager(RibbonControl ribbon, CustomLinksControl linksControl, BarItemLinkReadOnlyCollection links) : base(ribbon) {
			this.links = links;
			this.linksControl = linksControl;
		}
		public RibbonBaseKeyTipManager Parent { get { return parent; } set { parent = value; } }
		public CustomLinksControl LinksControl { get { return linksControl; } }
		public BarItemLinkReadOnlyCollection Links { 
			get { 
				if(Ribbon != null && Ribbon.IsDesignMode)return links;
				return links;
			} 
		}
		protected virtual bool ShouldIgnoreLink(BarItemLink link) {
			if (link is XtraBars.InternalItems.BarScrollItemLink) return true;
			if(Ribbon.IsDesignMode) return false;
			return link.Bounds == Rectangle.Empty;
		}		
		public virtual void GenerateContainerKeyTips() {
			ClearItems();
			for (int i = 0; i < Links.Count; i++)
			{
				if (ShouldIgnoreLink(Links[i])) continue;   
				Items.Add(Links[i] as ISupportRibbonKeyTip);
				if (Items[Items.Count - 1].HasDropDownButton)
				{
					AddDropDownButtonItem(Links[i]);
				}
			}
			GenerateKeyTips();
		}
		public override void ActivateKeyTips() {
			base.ActivateKeyTips();
			GenerateContainerKeyTips();
			ShowKeyTips();
		}
		public override void ActivateParentKeyTips() {
			if(Parent == null) return;
			Parent.ActivateKeyTips();
		}
		internal virtual bool ShouldCheckInterceptKey { get { return Show; } }
	}
	public class MinimizedRibbonKeyTipManager : RibbonKeyTipManager { 
		public MinimizedRibbonKeyTipManager(RibbonControl ribbon) : base(ribbon) { }
		public RibbonMinimizedControl MinimizedRibbon { get { return Ribbon as RibbonMinimizedControl; } }
		public override void CancelKeyTip() {
			if(!Show) return;
			if(MinimizedRibbon != null) {
				MinimizedRibbon.SourceRibbon.KeyTipManager.ActivatePageKeyTips();
				HideKeyTips();
			}
		}
		public override void ActivateKeyTips() {
			ActivatePanelKeyTips();
		}
	}
	public class RibbonPopupKeyTipManager : RibbonKeyTipManager {
		public RibbonPopupKeyTipManager(RibbonControl ribbon) : base(ribbon) { }
		public RibbonOneGroupControl OneGroupRibbon { get { return Ribbon as RibbonOneGroupControl; } }
		public override void CancelKeyTip() {
			if(!Show) return;
			if(OneGroupRibbon != null) 
				OneGroupRibbon.SourceRibbon.KeyTipManager.ActivatePanelKeyTips();
		}
	}
	public class BarsKeyTipManagerBase : BaseKeyTipManager {
		string filterString;
		bool show;
		public BarsKeyTipManagerBase() {
			this.filterString = string.Empty;
			this.show = false;
		}
		public string FilterString { get { return filterString; } }
		public bool Show { get { return show; } }
		protected void SetShow(bool show) { this.show = show; }
		protected internal virtual void ResetFilterString() { this.filterString = string.Empty; }
		public virtual bool IsNeededChar(char c) {
			string s = FilterString + Char.ToUpper(c).ToString();
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].KeyTipVisible && Items[i].KeyTipEnabled && IsFiltered(Items[i], s))
					return true;
			}
			return false;
		}
		protected bool EscapeChar(char c) { return c == 0x1b; }
		public void AddChar(char c) {
			if(IsSuspended) return;
			if(!IsNeededChar(c)) {
				if(!EscapeChar(c)) SystemSounds.Beep.Play();
				return;
			}
			if(!Show || FilterString.Length >= 3) return;
			if(FilterString.Length >= 3) return;
			c = Char.ToUpper(c);
			this.filterString += c.ToString();
			OnFilterStringChanged();
		}
		protected internal virtual void OnKeyTipSelected(ISupportRibbonKeyTip item) {
			HideKeyTips();
			this.filterString = string.Empty;
			item.Click();
		}
		protected virtual bool IsFiltered(ISupportRibbonKeyTip item, string fstring) {
			if(item.ItemKeyTip.StartsWith(fstring)) return true;
			return false;
		}
		protected virtual void UpdateFormsState() {
			for(int i = 0; i < Items.Count; i++) {
				if(!Items[i].KeyTipVisible)
					continue;
				if(!IsFiltered(Items[i], FilterString))
					Forms[i].Hide();
				else if(!Forms[i].Visible)
					Forms[i].Show();
			}
		}
		public virtual void HideKeyTips() {
			for(int i = 0; i < Items.Count; i++) {
				Forms[i].Hide();
			}
			this.show = false;
		}
		protected virtual void OnFilterStringChanged() {
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].ItemKeyTip == FilterString) {
					OnKeyTipSelected(Items[i]);
					return;
				}
			}
			UpdateFormsState();
		}
		public override void ShowKeyTips() {
			base.ShowKeyTips();
			this.show = true;
		}
		protected internal override void ClearItems() {
			base.ClearItems();
			this.filterString = string.Empty;
		}
	}
	public class RibbonBaseKeyTipManager : BarsKeyTipManagerBase {
		public static int FormWidthPadding = 4;
		public static int FormHeightPadding = 1;
		RibbonControl ribbon;
		public RibbonBaseKeyTipManager(RibbonControl ribbon): base() {
			this.ribbon = ribbon;
		}
		public RibbonControl Ribbon { get { return ribbon; } set { ribbon = value; } }
		public virtual void CancelKeyTip() {
			HideKeyTips();
		}
		protected virtual void AddDropDownButtonItem(BarItemLink link) {
			Items.Add(new DropDownButtonKeyTipItem(Ribbon, link as BarButtonItemLink));
		}
		protected virtual void AddItem(BarItemLink link) {
			BarButtonGroupLink bg = link as BarButtonGroupLink;
			if(bg != null) {
				if(Ribbon.IsDesignMode) {
					for(int i = 0; i < bg.Item.ItemLinks.Count; i++) {
						AddItem(bg.Item.ItemLinks[i]);
						if(Items[Items.Count - 1].HasDropDownButton)
							AddDropDownButtonItem(bg.Item.ItemLinks[i]);
					}
				}
				else {
					RibbonButtonGroupItemViewInfo bgInfo = bg.RibbonItemInfo as RibbonButtonGroupItemViewInfo;
					for(int i = 0; i < bgInfo.Items.Count; i++) {
						AddItem(bgInfo.Items[i].Item as BarItemLink);
						if(Items[Items.Count - 1].HasDropDownButton)
							AddDropDownButtonItem(bg.Item.ItemLinks[i]);
					}
				}
			}
			else Items.Add(link as ISupportRibbonKeyTip);
			if(Items[Items.Count - 1].HasDropDownButton)
				AddDropDownButtonItem(link);
		}
		protected virtual void AddItem(RibbonItemViewInfo itemInfo) {
			if(itemInfo is RibbonSeparatorItemViewInfo) return;
			RibbonButtonGroupItemViewInfo bg = itemInfo as RibbonButtonGroupItemViewInfo;
			if(bg != null) {
				for(int i = 0; i < bg.Items.Count; i++) {
					AddItem(bg.Items[i]);
					if(Items[Items.Count - 1].HasDropDownButton)
						AddDropDownButtonItem(itemInfo.Item as BarItemLink);
				}
			}
			else Items.Add(itemInfo.Item as ISupportRibbonKeyTip);
			if(Items[Items.Count - 1].HasDropDownButton)
				AddDropDownButtonItem(itemInfo.Item as BarButtonItemLink);
		}
		protected virtual void AddItemLinks(BarItemLinkCollection itemLinks) {
			for(int i = 0; i < itemLinks.Count; i++) {
				AddItem(itemLinks[i]);
			}
		}
		protected virtual Size GetFormMinSize(KeyTipForm form) {
			Size sz = GetFormSize(form, "W");
			sz.Width = sz.Height;
			return sz;
		}
		protected virtual Size GetFormSize(KeyTipForm form, string text) {
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				return form.CalcBestSize(gInfo);
			}
			finally {
				gInfo.ReleaseGraphics();
			}
		}
		protected internal override Size CalcFormSize(KeyTipForm form, string text) {
			Size sz = GetFormSize(form, text);
			Size ms = GetFormMinSize(form);
			return new Size(Math.Max(sz.Width, ms.Width), Math.Max(sz.Height, ms.Height));
		}
		protected override KeyTipForm CreateKeyTipForm() { return new KeyTipForm(Ribbon); }
		public override void ShowKeyTips() {
			Ribbon.ActiveKeyTipManager = this;
			base.ShowKeyTips();
		}
		public virtual void ActivateParentKeyTips() { }
		public virtual void ActivateKeyTips() {
			Ribbon.ActiveKeyTipManager = this;
		}
	}
	public enum RibbonKeyTipMode { PagesKeyTips, PanelKeyTips, ApplicationContentControl, BackstageViewControl }
	public class RibbonKeyTipManager : RibbonBaseKeyTipManager {
		RibbonKeyTipMode keyTipMode;
		public RibbonKeyTipManager(RibbonControl ribbon) : base(ribbon) { 
			this.keyTipMode = RibbonKeyTipMode.PagesKeyTips;
		}
		public RibbonKeyTipMode KeyTipMode { get { return keyTipMode; } }
		public override void CancelKeyTip() {
			if(!Show) return;
			base.CancelKeyTip();
			if(KeyTipMode == RibbonKeyTipMode.ApplicationContentControl) {
				Ribbon.HideApplicationButtonContentControl();
				ActivatePageKeyTips();
			}
			else if(KeyTipMode == RibbonKeyTipMode.PanelKeyTips)
				ActivatePageKeyTips();
			return;
		}
		protected virtual void AddApplicationButton() {
			if(!Ribbon.ViewInfo.IsAllowApplicationButton) return;
			if(!Ribbon.IsDesignMode && Ribbon.ApplicationButtonDropDownControl == null) return;
			Items.Add(new RibbonApplicationButtonKeyTipItem(Ribbon));
		}
		protected virtual void AddQuickAccessToolbarItemsInDesignTime() {
			AddItemLinks(Ribbon.Toolbar.ItemLinks);
		}
		protected virtual void AddQuickAccessToolbarItems() {
			if(Ribbon.ToolbarLocation == RibbonQuickAccessToolbarLocation.Hidden) return;
			int itemsCount = Ribbon.ViewInfo.Toolbar.VisibleButtonCount;
			if (itemsCount == 0) return;
			RibbonItemViewInfo lastItemInfo = Ribbon.ViewInfo.Toolbar.Items[ itemsCount - 1];
			if(lastItemInfo.Item == Ribbon.Toolbar.CustomizeItemLink || lastItemInfo.Item == Ribbon.Toolbar.DropDownItemLink) itemsCount--;
			for(int i = 0; i < itemsCount; i++) {
				if(Ribbon.ViewInfo.Toolbar.Items[i].Item == null) continue;
				AddItem(Ribbon.ViewInfo.Toolbar.Items[i].Item as BarItemLink);
			}
		}
		protected virtual bool ShouldAddRibbonPages {
			get {
				if(Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.Hide || (Ribbon.ShowPageHeadersMode == ShowPageHeadersMode.ShowOnMultiplePages && Ribbon.Pages.VisiblePages.Count <= 1)) return false;
				return true;
			}
		}
		public void GeneratePageKeyTips() {
			ClearItems();
			this.keyTipMode = RibbonKeyTipMode.PagesKeyTips;
			if(Ribbon.IsDesignMode) {
				foreach(RibbonPage page in Ribbon.TotalPageCategory.Pages)
					Items.Add(page as ISupportRibbonKeyTip);
			}
			else if(ShouldAddRibbonPages) {
				ArrayList pages = Ribbon.TotalPageCategory.GetVisiblePages();
				foreach(RibbonPage page in pages)
					Items.Add(page as ISupportRibbonKeyTip);
			}
			else if(Ribbon.SelectedPage != null){
				Items.Add(Ribbon.SelectedPage as ISupportRibbonKeyTip);
			}
			AddApplicationButton();
			if(Ribbon.IsDesignMode) AddQuickAccessToolbarItemsInDesignTime();
			else AddQuickAccessToolbarItems();
			GenerateKeyTips();
		}
		public void ActivatePageKeyTips() {
			GeneratePageKeyTips();
			ShowKeyTips();
		}
		public void GeneratePanelKeyTipsInDesignTime(RibbonPage page) {
			ClearItems();
			this.keyTipMode = RibbonKeyTipMode.PanelKeyTips;
			for (int i = 0; i < page.Groups.Count; i++) {
				Items.Add(page.Groups[i] as ISupportRibbonKeyTip);
				AddItemLinks(page.Groups[i].ItemLinks);
			}
			GenerateKeyTips();
		}
		public void GeneratePanelKeyTips()
		{
			ClearItems();
			this.keyTipMode = RibbonKeyTipMode.PanelKeyTips;
			for (int i = 0; i < Ribbon.ViewInfo.Panel.Groups.Count; i++)
			{
				if (Ribbon.ViewInfo.Panel.Groups[i].ShowCaptionButton)
					Items.Add(Ribbon.ViewInfo.Panel.Groups[i].PageGroup as ISupportRibbonKeyTip);
				for (int j = 0; j < Ribbon.ViewInfo.Panel.Groups[i].Items.Count; j++)
				{
					AddItem(Ribbon.ViewInfo.Panel.Groups[i].Items[j]);
				}
			}
			GenerateKeyTips();
		}
		public void ActivatePanelKeyTips() {
			Ribbon.CheckViewInfo();
			if(Ribbon.IsDesignMode) GeneratePanelKeyTipsInDesignTime(Ribbon.SelectedPage);
			else GeneratePanelKeyTips();
			ShowKeyTips();
		}
		public void ActivateApplicationButtonControlKeyTips() {
			if(Ribbon.ApplicationButtonControl != null)
				Ribbon.ShowApplicationButtonContentControl();
			this.keyTipMode = RibbonKeyTipMode.ApplicationContentControl;
			SetShow(true);
			Ribbon.Invalidate();
		}
		public override void ActivateKeyTips() {
			base.ActivateKeyTips();
			if(KeyTipMode == RibbonKeyTipMode.PagesKeyTips)
				ActivatePageKeyTips();
			else
				ActivatePanelKeyTips();
		}
	}
	public class BaseKeyTipManager {
		ItemWithKeyTipCollection items;
		NumericKeyTip numericKeyTip;
		int keyTipIndex;
		KeyTipForm[] forms;
		public BaseKeyTipManager() {
			this.items = new ItemWithKeyTipCollection();
			this.keyTipIndex = 10;
			this.numericKeyTip = new NumericKeyTip();
			this.forms = null;
			this.IsSuspended = false;
		}
		public ItemWithKeyTipCollection Items { get { return items; } }
		protected internal KeyTipForm[] Forms {
			get 
			{
				if(forms == null || forms.Length != Items.Count) forms = CreateKeyTipForms();
				return forms;
			}
		}
		protected virtual KeyTipForm CreateKeyTipForm() { return new KeyTipForm(null); }
		protected virtual KeyTipForm[] CreateKeyTipForms() { 
			KeyTipForm[] f = new KeyTipForm[Items.Count];
			for(int i = 0; i < f.Length; i++) {
				f[i] = CreateKeyTipForm();
				f[i].Text = Items[i].ItemKeyTip;
			}
			return f;
		}
		public bool IsContain(ISupportRibbonKeyTip item, string s) {
			if(s == string.Empty) return false;
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i] == item) continue;
				if(Items[i].ItemKeyTip.StartsWith(s)) return true;
			}
			return false;
		}
		public bool IsKeyTipCollision(ISupportRibbonKeyTip item, string s, bool checkAllItems) {
			if(s == string.Empty) return false;
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i] == item) {
					if(!checkAllItems) return false;
					continue;
				}
				if(Items[i].ItemKeyTip != string.Empty && (Items[i].ItemKeyTip.StartsWith(s) || s.StartsWith(Items[i].ItemKeyTip))) return true;
			}
			return false;
		}
		public bool IsKeyTipCollision() {
			for(int i = 0; i < Items.Count; i++) {
				if(IsKeyTipCollision(Items[i], Items[i].ItemKeyTip, true)) return true;
			}
			return false;
		}
		public bool IsSuspended {
			get;
			set;
		}
		bool ItemHasUserKeyTip(ISupportRibbonKeyTip item) { return item.ItemUserKeyTip != string.Empty && item.ItemUserKeyTip != null; }
		bool ItemHasCaption(ISupportRibbonKeyTip item) { return item.ItemCaption != string.Empty; }
		bool ItemKeyTipGenerated(ISupportRibbonKeyTip item) { return item.ItemKeyTip != string.Empty; }
		int KeyTipIndex { get { return keyTipIndex; } set { keyTipIndex = value; } }
		bool GenerateNumericKeyTips() {
			NumericKeyTip.GenerateKeyTipChars(this);
			for(int i = 0; i < Items.Count; i++) {
				if(ItemKeyTipGenerated(Items[i])) continue;
				if(!GenerateKeyTipByIndex(Items[i])) return false;
			}
			return true;
		}
		bool GenerateKeyTipByIndex(ISupportRibbonKeyTip item) {
			for(int i = 0; i < 999; i++) {
				item.ItemKeyTip = NumericKeyTip.GetNextKeyTip();
				if(IsKeyTipCollision(item, item.ItemKeyTip, true)) continue;
				if(ItemKeyTipGenerated(item)) break;
			}
			return true;
		}
		protected internal bool IsUserKeyTipCollision(string s) {
			for(int i = 0; i < Items.Count; i++) {
				if(!ItemHasUserKeyTip(Items[i])) continue;
				if(Items[i].ItemKeyTip != "" && s.StartsWith(Items[i].ItemKeyTip)) return true;
			}
			return false;
		}
		bool GenerateKeyTipsByCaption() {
			for(int i = 0; i < Items.Count; i++) {
				if(!ItemHasCaption(Items[i]) || ItemHasUserKeyTip(Items[i])) continue;
				GenerateFirstCharByCaption(Items[i]);
			}
			for(int i = 0; i < Items.Count; i++) {
				if(!ItemHasCaption(Items[i]) || ItemHasUserKeyTip(Items[i]) || !IsKeyTipCollision(Items[i], Items[i].ItemKeyTip, true)) continue;
				GenerateSecondCharByCaption(Items[i]);
			}
			for(int i = 0; i < Items.Count; i++) {
				if(!ItemHasCaption(Items[i]) || ItemHasUserKeyTip(Items[i]) || !IsKeyTipCollision(Items[i], Items[i].ItemKeyTip, true)) continue;
				GenerateThirdCharByCaption(Items[i]);
			}
			ResolveCollisions();
			return true;
		}
		protected virtual bool ShouldIgnoreChar(string s) {
			return s == "&" || s == " ";
		}
		bool GenerateFirstCharByCaption(ISupportRibbonKeyTip item) {
			for(int i = 0; i < item.ItemCaption.Length; i++) {
				if(IsUserKeyTipCollision(item.ItemCaption.Substring(i, 1).ToUpper())) continue;
				if(ShouldIgnoreChar(item.ItemCaption.Substring(i, 1))) continue;
				item.ItemKeyTip = item.ItemCaption.Substring(i, 1).ToUpper();
				item.FirstIndex = i;
				return true;
			}
			return false;
		}
		bool GenerateSecondCharByCaption(ISupportRibbonKeyTip item) {
			string s;
			for(int i = item.FirstIndex + 1; i < item.ItemCaption.Length; i++) {
				if(ShouldIgnoreChar(item.ItemCaption.Substring(i, 1))) continue;
				s = item.ItemKeyTip + item.ItemCaption.Substring(i, 1).ToUpper();
				if(IsContain(item, s)) continue;
				item.ItemKeyTip = s;
				item.FirstIndex = i;
				return true;
			}
			if(item.FirstIndex >= item.ItemCaption.Length - 1) {
				item.ItemKeyTip = item.ItemKeyTip + KeyTipIndex;
				KeyTipIndex++;
				return false;
			}
			item.ItemKeyTip = item.ItemKeyTip + item.ItemCaption.Substring(item.FirstIndex + 1, 1).ToUpper();
			item.FirstIndex = item.FirstIndex + 1;
			return false;
		}
		bool GenerateThirdCharByCaption(ISupportRibbonKeyTip item) {
			string s;
			if(item.ItemKeyTip.Length == 3) return true;
			for(int i = item.FirstIndex + 1; i < item.ItemCaption.Length; i++) {
				if(ShouldIgnoreChar(item.ItemCaption.Substring(i, 1))) continue;
				s = item.ItemKeyTip + item.ItemCaption.Substring(i, 1).ToUpper();
				if(IsContain(item, s)) continue;
				item.ItemKeyTip = s;
				item.FirstIndex = i;
				return true;
			}
			if(item.FirstIndex == item.ItemCaption.Length - 1) {
				item.ItemKeyTip = item.ItemKeyTip.Substring(0, 1).ToUpper() + KeyTipIndex;
				KeyTipIndex++;
				return false;
			}
			item.ItemKeyTip = item.ItemKeyTip + item.ItemCaption.Substring(item.FirstIndex + 1, 1).ToUpper();
			item.FirstIndex = item.FirstIndex + 1;
			return false;
		}
		void UpdateItemKeyTip(ISupportRibbonKeyTip item) {
			for(int i = KeyTipIndex; i < 99; i++) {
				item.ItemKeyTip = item.ItemKeyTip.Substring(0, 1) + i.ToString();
				if(!IsKeyTipCollision(item, item.ItemKeyTip, false)) {
					KeyTipIndex = i + 1;
					return;
				}
			}
		}
		public bool IsNumericCollision(string s) {
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].ItemUserKeyTip.StartsWith(s)) return true;
			}
			return false;
		}
		void ResolveCollisions() {
			for(int i = 0; i < Items.Count; i++) {
				if(!IsKeyTipCollision(Items[i], Items[i].ItemKeyTip, true)) continue;
				UpdateItemKeyTip(Items[i]);
			}
		}
		protected internal int GetItemsWithEmptyKeyTip() {
			int itemsCount = 0;
			for(int i = 0; i < Items.Count; i++) {
				if(!ItemKeyTipGenerated(Items[i])) itemsCount++;
			}
			return itemsCount;
		}
		public NumericKeyTip NumericKeyTip { get { return numericKeyTip; } }
		public void GenerateKeyTips() {
			PrepareForGeneration();
			GenerateKeyTipsByUserKeyTips();
			GenerateKeyTipsByCaption();
			GenerateNumericKeyTips();
		}
		private void PrepareForGeneration() {
			KeyTipIndex = 10;
		}
		protected virtual void GenerateKeyTipsByUserKeyTips() {
			for(int i = 0; i < Items.Count; i++) {
				if(Items[i].ItemUserKeyTip == string.Empty) continue;
				Items[i].ItemKeyTip = Items[i].ItemUserKeyTip;
			}
			ResolveCollisions();
		}
		protected internal virtual void ClearItems() {
			for(int i = 0; i < Forms.Length; i++) {
				Forms[i].Hide();
				Forms[i].Dispose();
			}
			this.forms = null;
			Items.Clear();
		}
		protected internal virtual Size CalcFormSize(KeyTipForm form, string text) {
			return new Size(24, 16);
		}
		protected virtual Point CalcFormLocation(Point showPoint, ContentAlignment align, Size sz) {
			switch(align) { 
				case ContentAlignment.TopLeft:
					return showPoint;
				case ContentAlignment.TopCenter:
					return new Point(showPoint.X - sz.Width / 2, showPoint.Y);
				case ContentAlignment.TopRight:
					return new Point(showPoint.X - sz.Width, showPoint.Y);
				case ContentAlignment.MiddleLeft:
					return new Point(showPoint.X, showPoint.Y - sz.Height / 2);
				case ContentAlignment.MiddleCenter:
					return new Point(showPoint.X - sz.Width / 2, showPoint.Y - sz.Height / 2);
				case ContentAlignment.MiddleRight:
					return new Point(showPoint.X - sz.Width, showPoint.Y - sz.Height / 2);
				case ContentAlignment.BottomLeft:
					return new Point(showPoint.X, showPoint.Y - sz.Height);
				case ContentAlignment.BottomCenter:
					return new Point(showPoint.X - sz.Width / 2, showPoint.Y - sz.Height);
				case ContentAlignment.BottomRight:
					return new Point(showPoint.X - sz.Width, showPoint.Y - sz.Height);
			}
			return showPoint;
		}
		protected virtual void UpdateForm(KeyTipForm form, ISupportRibbonKeyTip item) {
			Size sz = CalcFormSize(form, item.ItemKeyTip);
			form.Bounds = new Rectangle(CalcFormLocation(item.ShowPoint, item.Alignment, sz), sz);
		}
		protected virtual void ShowKeyTipsCore() {
			for(int i = 0; i < Items.Count; i++) {
				if(!Items[i].KeyTipVisible)
					continue;
				UpdateForm(Forms[i], Items[i]);
				Forms[i].Show();
				Forms[i].Opacity = Items[i].KeyTipEnabled ? 1.0f : 0.6f;
			}				
		}
		public virtual void ShowKeyTips() {
			ShowKeyTipsCore();
		}
	}
}
