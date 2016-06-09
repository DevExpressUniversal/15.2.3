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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraNavBar;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using System.Collections.Generic;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public enum NavBarHintObjectType { None, Group, Link, Button, NavigationPaneOveflowGroup, NavigationPaneOveflowChevron,NavPaneContentButton };
	public class NavBarHintInfo : IComparable {
		bool show;
		string text;
		object hintObject;
		NavBarHintObjectType objectType;
		AppearanceObject appearance;
		static NavBarHintInfo empty = null;
		public static NavBarHintInfo Empty { 
			get { 
				if(empty == null) empty = new NavBarHintInfo(null);
				return empty; 
			} 
		}
		public NavBarHintInfo(AppearanceObject appearance) {
			this.show = false;
			this.text = string.Empty;
			this.hintObject = null;
			this.objectType = NavBarHintObjectType.None;
			this.appearance = appearance;
		}
		protected internal void SetHint(NavBarHintObjectType objectType, object hintObject, string text, SuperToolTip superTip) {
			this.objectType = objectType;
			this.hintObject = hintObject;
			this.text = text;
			this.show = true;
			this.SuperTip = superTip;
		}
		public AppearanceObject Appearance { get { return appearance; } set { appearance = value; } }
		public bool Show { get { return show; } set { show = value; } }
		public string Text { get { return text; } set { text = value; } }
		public object HintObject { get { return hintObject; } set { hintObject = value; } }
		public NavBarHintObjectType ObjectType { get { return objectType; } set { objectType = value; } }
		public SuperToolTip SuperTip { get; set; }
		int IComparable.CompareTo(object obj) {
			NavBarHintInfo hi = obj as NavBarHintInfo;
			if(hi == null) return -1;
			return (Show == hi.Show && Text == hi.Text && ObjectType == hi.ObjectType && HintObject == hi.HintObject) ? 0 : -1;
		}
		public override bool Equals(object obj) {
			return ((IComparable)this).CompareTo(obj) == 0;
		}
		public override int GetHashCode() { return base.GetHashCode(); }
	}
	public class NavBarObjectInfoArgs : ObjectInfoArgs {
		NavBarControl navBar;
		public NavBarObjectInfoArgs(GraphicsCache cache, NavBarControl navBar, Rectangle bounds, ObjectState state) : base(cache, bounds, state) {
			this.navBar = navBar;
		}
		public NavBarControl NavBar { get { return navBar; } }
	}
	public class NavGroupClientInfoArgs : ObjectInfoArgs {
		Rectangle clientBounds, clientInnerBounds;
		ArrayList links;
		NavBarGroup group;
		NavGroupInfoArgs navGroupInfo;
		bool inForm;
		public NavGroupClientInfoArgs(NavBarGroup group) {
			this.group = group;
			this.links = null;
			this.clientBounds = this.clientInnerBounds = Rectangle.Empty;
			this.inForm = false;
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			clientBounds.Offset(x,y);
			clientInnerBounds.Offset(x,y);
			if(Links == null) return;
			foreach(NavLinkInfoArgs link in Links) {
				link.OffsetContent(x, y);
			}
		}
		public NavBarGroup Group { get { return group; } }
		public NavGroupInfoArgs NavGroupInfo { get { return navGroupInfo; } set { navGroupInfo = value; } }
		public bool InForm { get { return inForm; } set { inForm = value; } }
		public ArrayList Links { 
			get { 
				if(links == null) links = new ArrayList();
				return links; 
			} 
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			NavGroupClientInfoArgs cinfo = info as NavGroupClientInfoArgs;
			if(cinfo == null) return;
			this.clientBounds = cinfo.clientBounds;
			this.clientInnerBounds = cinfo.clientInnerBounds;
			this.group = cinfo.group;
			if(this.links == null) links = new ArrayList();
			else this.links.Clear();
			foreach(NavLinkInfoArgs link in cinfo.Links) { 
				this.links.Add(new NavLinkInfoArgs(link.Cache, link.Link, link.Bounds, link.State));
				(this.links[this.links.Count - 1] as NavLinkInfoArgs).Assign(link);
			}
		}
		public virtual void Reset() {
			this.clientBounds = this.clientInnerBounds = Rectangle.Empty;
			if(HasLinks) Links.Clear();
		}
		public NavBarControl NavBar { get { return Group != null ? Group.NavBar : null; } }
		public Rectangle ClientBounds { get { return clientBounds; } set { clientBounds = value; } }
		public Rectangle ClientInnerBounds { get { return clientInnerBounds; } set { clientInnerBounds = value; } }
		public bool HasLinks { get { return this.links != null && this.links.Count > 0; } }
		public Rectangle LastLinkBounds {
			get {
				Rectangle res = Rectangle.Empty;
				if(!HasLinks) return res;
				res = (Links[Links.Count - 1] as NavLinkInfoArgs).Bounds;
				return res;
			}
		}
		public Rectangle FirstLinkBounds {
			get {
				Rectangle res = Rectangle.Empty;
				if(!HasLinks) return res;
				res = (Links[0] as NavLinkInfoArgs).Bounds;
				return res;
			}
		}
		public bool CanDraw { get { return ClientBounds.Width > 0 && ClientBounds.Height > 0; } }
	}
	public class NavGroupInfoArgs : StyleObjectInfoArgs {
		NavBarGroup group;
		NavGroupClientInfoArgs clientInfo;
		Rectangle captionBounds, imageBounds, buttonBounds, footerBounds, captionClientBounds;
		int captionMaxWidth;
		AppearanceObject paintAppearance;
		ObjectState paintAppearanceState = ObjectState.Normal;
		int paintIndex = 0;
		StringInfo stringInfo;
		public virtual void Assign(NavGroupInfoArgs info) {
			this.group = info.Group;
			if(this.clientInfo == null) this.clientInfo = new NavGroupClientInfoArgs(Group);
			this.clientInfo.Assign(info.clientInfo);
			this.captionBounds = info.captionBounds;
			this.imageBounds = info.imageBounds;
			this.footerBounds = info.footerBounds;
			this.captionClientBounds = info.captionClientBounds;
			this.paintAppearance.Assign(info.paintAppearance);
			this.paintAppearanceState = info.paintAppearanceState;
			this.paintIndex = info.paintIndex;
			this.stringInfo = info.stringInfo;
		}
		public NavGroupInfoArgs(NavBarGroup group, Rectangle bounds) : base(null, bounds, null, ObjectState.Normal) {
			this.group = group;
			this.clientInfo = CreateClientInfo();
			this.captionClientBounds = this.footerBounds = this.captionBounds = this.imageBounds = this.buttonBounds = Rectangle.Empty;
			this.captionMaxWidth = 0;
			this.paintAppearance = new AppearanceObject();
			UpdatePaintAppearance();
		}
		protected virtual NavGroupClientInfoArgs CreateClientInfo() {
			return new NavGroupClientInfoArgs(Group);
		}
		public int PaintIndex { get { return paintIndex; } set { paintIndex = value; } }
		public Rectangle ClientInfoBounds { get { return ClientInfo == null ? Rectangle.Empty : ClientInfo.Bounds; } }
		public override AppearanceObject Appearance {
			get {
				return PaintAppearance;
			}
		}
		protected virtual void UpdatePaintAppearance() {
			if(Group == null) return;
			this.paintAppearanceState = Group.StateCore;
			PaintAppearance = Group.GetHeaderAppearance();
		}
		public AppearanceObject PaintAppearance {
			get { 
				if(Group == null) return base.Appearance;
				if(Group.StateCore  != this.paintAppearanceState) UpdatePaintAppearance();
				return paintAppearance; 
			}
			set { 
				if(paintAppearance != null) paintAppearance.Dispose();
				paintAppearance = value;
			}
		}
		public override Graphics Graphics { 
			get { return base.Graphics; }
			set {
				base.Graphics = value;
				ClientInfo.Graphics = value;
			}
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(!this.captionBounds.IsEmpty) this.captionBounds.Offset(x, y);
			if(!this.imageBounds.IsEmpty) this.imageBounds.Offset(x, y);
			if(!this.buttonBounds.IsEmpty) this.buttonBounds.Offset(x, y);
			if(!this.footerBounds.IsEmpty) this.footerBounds.Offset(x, y);
			if(!this.captionClientBounds.IsEmpty) this.captionClientBounds.Offset(x, y);
		}
		public Rectangle FooterBounds { get { return footerBounds; } set { footerBounds = value; } } 
		public Rectangle CaptionBounds { get { return captionBounds; } set { captionBounds = value; } }
		protected internal int CaptionMaxWidth { get { return captionMaxWidth; } set { captionMaxWidth = value; } }
		public Rectangle ImageBounds { get { return imageBounds; } set { imageBounds = value; } }
		public Rectangle ButtonBounds { get { return buttonBounds; } set { buttonBounds = value; } }
		public Rectangle CaptionClientBounds { get { return captionClientBounds; } set { captionClientBounds = value; } }
		public Rectangle LastLinkBounds { get { return ClientInfo.LastLinkBounds; } }
		public Rectangle FirstLinkBounds { get { return ClientInfo.FirstLinkBounds; } }
		public int LinkAreaHeight { get { return LastLinkBounds.Bottom - FirstLinkBounds.Top; } }
		public ArrayList Links { get { return ClientInfo.Links; } }
		public override GraphicsCache Cache {
			get { return base.Cache; }
			set { 
				base.Cache = value;
				ClientInfo.Cache = value;
			}
		}
		public NavGroupClientInfoArgs ClientInfo { get { return clientInfo; } }
		public NavBarGroup Group { get { return group; } }
		public bool CanDrawGroupClient { get { return ClientInfo.CanDraw; } }
		public StringInfo StringInfo {
			get { return this.stringInfo; }
			set {
				this.stringInfo = value;
			}
		}
	}
	public class NavLinkInfoArgs : ObjectInfoArgs {
		Rectangle imageRectangle;
		NavBarItemLink link;
		Rectangle captionRectangle, hitRectangle, realCaptionRectangle;
		StringInfo stringInfo;
		public NavLinkInfoArgs(GraphicsCache cache, NavBarItemLink link, Rectangle bounds, ObjectState state) : base(cache, bounds, state) {
			realCaptionRectangle = hitRectangle = captionRectangle = imageRectangle = Rectangle.Empty;
			this.link = link;
			stringInfo = null;
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			NavLinkInfoArgs linfo = info as NavLinkInfoArgs;
			if(linfo == null) return;
			this.imageRectangle = linfo.imageRectangle;
			this.link = linfo.link;
			this.captionRectangle = linfo.captionRectangle;
			this.hitRectangle = linfo.hitRectangle;
			this.realCaptionRectangle = linfo.realCaptionRectangle;
			this.stringInfo = linfo.stringInfo;
		}
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			imageRectangle.Offset(x, y);
			captionRectangle.Offset(x, y);
			hitRectangle.Offset(x, y);
			realCaptionRectangle.Offset(x, y);
		}
		public NavBarItemLink Link { get { return link; } }
		public Rectangle ImageRectangle {
			get { return imageRectangle; }
			set {
				imageRectangle = value;
			}
		}
		public Rectangle RealCaptionRectangle {
			get { return realCaptionRectangle; }
			set {
				realCaptionRectangle = value;
			}
		}
		public Rectangle CaptionRectangle {
			get { return captionRectangle; }
			set {
				captionRectangle = value;
			}
		}
		public Rectangle HitRectangle {
			get { return hitRectangle; }
			set {
				hitRectangle = value;
			}
		}
		public StringInfo StringInfo {
			get { return stringInfo; }
			set {
				stringInfo = value;
			}
		}
	}
	public class BaseNavLinkPainter : ObjectPainter {
		TextOptions smallOptions, largeOptions;
		NavBarControl navBar;
		public BaseNavLinkPainter(NavBarControl navBar) {
			this.navBar = navBar;
			this.smallOptions = CreateOptions(true);
			this.largeOptions = CreateOptions(false);
		}
		public virtual NavBarControl NavBar { get { return navBar; } }
		protected virtual TextOptions CreateOptions(bool small) {
			TextOptions opt = new TextOptions(null);
			opt.VAlignment = VertAlignment.Center;
			if(small) {
				opt.WordWrap = WordWrap.NoWrap;
				opt.HAlignment = HorzAlignment.Near;
				opt.Trimming = Trimming.EllipsisCharacter;
			} else {
				opt.WordWrap = WordWrap.Wrap;
				opt.HAlignment = HorzAlignment.Center;
				opt.Trimming = Trimming.None;
				opt.VAlignment = VertAlignment.Top;
			}
			return opt;
		}
		protected virtual bool IsDrawLinkHotOrPressed(ObjectInfoArgs e) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			if((li.Link.Enabled || li.Link.NavBar.AllowSelectDisabledLink) && (e.State == ObjectState.Hot || e.State == ObjectState.Pressed || e.State == ObjectState.Selected)) return true;
			return false;
		}
		public virtual ObjectState CalcLinkState(NavBarItemLink link, ObjectState state) {
			if((state & ObjectState.Selected) != 0) return ObjectState.Pressed;
			if(link.NavBar.EachGroupHasSelectedLinkCore && link.Group.SelectedLink == link) return ObjectState.Pressed;
			return state;
		}
		public virtual AppearanceObject GetLinkAppearance(ObjectInfoArgs e) {
			NavBarItemLink link = GetLink(e);
			ObjectState state = e.State;
			if(!link.Enabled) state = ObjectState.Disabled;
			return GetLinkAppearance(e, state);
		}
		public virtual AppearanceObject GetLinkAppearance(ObjectInfoArgs e, ObjectState state) {
			NavBarItemLink link = GetLink(e);
			return link.Item.GetItemAppearance(state);
		}
		protected virtual StringFormat GetLinkFormat(ObjectInfoArgs e) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			bool useSmall = li.Link.Group.GetLinksUseSmallImage();
			AppearanceObject appearance = GetLinkAppearance(e, ObjectState.Normal);
			return appearance.GetTextOptions().GetStringFormat(useSmall ? this.smallOptions : this.largeOptions);
		}
		protected Size CalcImageSize(ObjectInfoArgs e) { return NavBar.GroupPainter.ClientPainter.CalcImageSize(e, GetLink(e).Group); }
		public NavBarControl GetNavBar(ObjectInfoArgs e) {
			return GetLink(e).Group.NavBar;
		}
		public NavBarItemLink GetLink(ObjectInfoArgs e) {
			NavLinkInfoArgs linkInfo = e as NavLinkInfoArgs;
			return linkInfo.Link;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			Rectangle r = e.Bounds;
			NavBarItemLink link = GetLink(e);
			if(link.Group.GetShowAsIconsView()) {
				if(link.Item.IsSeparator()) return CalcListViewSeparatorBounds(e, r);
				return CalcListViewBounds(e, r);
			}
			if(link.Item.IsSeparator()) r = CalcSeparatorBounds(e, r);
			else if(link.Group.GetLinksUseSmallImage()) r = CalcSmallBounds(e, r);
			else r = CalcLargeBounds(e, r);
			return r;
		}
		public virtual int GetImageIndent(ObjectInfoArgs e) {
			return 2;
		}
		protected virtual int GetSeparatorHeight() {
			var height = GetSeparatorImageSize().Height;
			height = height == 0 ? 2 : height;
			return height;
		}
		protected virtual Rectangle CalcListViewBounds(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			Size size = CalcImageSize(e);
			Rectangle ir = r;
			ir.X += 2;
			ir.Height = size.Height + GetImageIndent(e) * 2;
			ir.Width = size.Width + GetImageIndent(e) * 2;
			li.ImageRectangle = ir;
			li.HitRectangle = li.RealCaptionRectangle = li.CaptionRectangle = Rectangle.Empty;
			return ir;
		}
		protected virtual Rectangle CalcListViewSeparatorBounds(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			var res = GetSeparatorBounds(r.Location, e);
			res.X += 2;
			li.HitRectangle = li.RealCaptionRectangle = li.CaptionRectangle = li.ImageRectangle = Rectangle.Empty;
			return res;
		}
		protected bool IsItemExists(NavLinkInfoArgs li) {
			return li != null && li.Link != null && li.Link.Item != null;
		}
		protected virtual Rectangle CalcSmallBounds(ObjectInfoArgs e, Rectangle r) {
			Rectangle ir = r, tr = r;
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			NavBarItemLink link = GetLink(e);
			Size textSize;
			Size size = IsItemExists(li) ? li.Link.Item.GetPrefferedImageSize(CalcImageSize(e)) : CalcImageSize(e);
			if(!size.IsEmpty) {
				ir.X += 2;
				ir.Height = size.Height + GetImageIndent(e) * 2;
				ir.Width = size.Width + GetImageIndent(e) * 2;
				tr.X = ir.Right + 4;
				tr.Width = r.Right - 4 - tr.X;
			}
			else {
				ir = Rectangle.Empty;
				int indent = CalcLinkIndentWithoutImage(tr.X, li);
				tr.X += indent;
				tr.Width -= indent;
			}
			textSize = CalcBestTextSize(e, tr.Width);
			if(ir.Height < textSize.Height + 4) 
				ir.Y += ((textSize.Height + 4)- ir.Height) / 2;
			tr.Height = Math.Max(ir.Height, textSize.Height + 4);
			int maxWidth = tr.Width;
			tr.Width = textSize.Width;
			tr = NavBarViewInfo.CheckElementCaptionLocation(GetLinkAppearance(e), tr, maxWidth);
			li.ImageRectangle = CheckBounds(ir);
			li.CaptionRectangle = CheckBounds(tr);
			li.RealCaptionRectangle = li.CaptionRectangle;
			if(!size.IsEmpty)
				li.HitRectangle = new Rectangle(ir.Location, new Size(tr.Right - ir.Location.X, ir.Bottom - tr.Y));
			else
				li.HitRectangle = li.CaptionRectangle;
			r.Height = tr.Height;
			li.Bounds = r;
			return r;
		}
		protected Rectangle CheckBounds(Rectangle bounds) {
			if(IsRightToleft) {
				if(NavBar.NavPaneForm != null) return new Rectangle(NavBar.NavPaneForm.Width - bounds.Right, bounds.Y, bounds.Width, bounds.Height);
				int x = CalcContentX(bounds);
				return new Rectangle(x, bounds.Y, bounds.Width, bounds.Height);
			}
			return bounds;
		}
		protected virtual int CalcContentX(Rectangle bounds) {
			return NavBar.Width - bounds.Right;
		}
		protected bool IsOverlapScrollBar {
			get {
				if(NavBar == null || NavBar.ViewInfo == null) return true;
				return NavBar.ViewInfo.ScrollBar == null || NavBar.ViewInfo.ScrollBar.IsOverlapScrollBar;
			}
		}
		protected bool IsRightToleft {
			get {
				if(NavBar == null) return false;
				return NavBar.IsRightToLeft;
			}
		}
		protected virtual int CalcLinkIndentWithoutImage(int textX, NavLinkInfoArgs li) {
			return 4;
		}
		protected virtual Rectangle CalcSeparatorBounds(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			var res = GetSeparatorBounds(r.Location, e);
			res.Height += CalcLargeVertIndent(e) * 2;
			li.ImageRectangle = res;
			li.CaptionRectangle = res;
			li.RealCaptionRectangle = res;
			li.HitRectangle = res;
			li.Bounds = res;
			return res;
		}
		protected virtual Rectangle CalcLargeImageRect(ObjectInfoArgs e, Rectangle r) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			NavBarItemLink link = GetLink(e);
			Size size = IsItemExists(li) ? li.Link.Item.GetPrefferedImageSize(CalcImageSize(e)) : CalcImageSize(e);
			if(size.IsEmpty)
				return Rectangle.Empty;
			size.Width += GetImageIndent(e) * 2;
			size.Height += GetImageIndent(e) * 2;
			Rectangle res = r;
			res.X = r.X + (r.Width - size.Width) / 2;
			res.Width = size.Width;
			res.Height = size.Height;
			if(res.X < r.X) res.X = r.X;
			if(res.Width > r.Width) res.Width = r.Width;
			return res;
		}
		protected virtual int CalcLargeVertIndent(ObjectInfoArgs e) {
			return 0;
		}
		static int SmallImageLeftIndent = 2;
		static int IndentBetweenImageAndText = 4;
		static int SmallTextRightIndent = 4;
		static int LargeLinkContentIndent = 2;
		public virtual int CalcSmallBestWidth(ObjectInfoArgs e, Rectangle r, NavBarGroup group) {
			Size size = CalcImageSize(e), textSize = CalcTextSize(e, 10000);
			return size.Width + GetImageIndent(e) * 2 + textSize.Width + SmallImageLeftIndent + IndentBetweenImageAndText + SmallTextRightIndent;
		}
		public virtual int CalcBestWidth(ObjectInfoArgs e, Rectangle r, NavBarGroup group) {
			Rectangle rect = Rectangle.Empty;
			if( group.GroupStyle == NavBarGroupStyle.LargeIconsList || group.GroupStyle == NavBarGroupStyle.LargeIconsText ) rect.Width = CalcLargeBestWidth( e, r, group );
			else rect.Width = CalcSmallBestWidth(e, r, group);
			return CalcBoundsByClientRectangle(e, rect).Width;
		}
		public virtual int CalcLargeBestWidth(ObjectInfoArgs e, Rectangle r, NavBarGroup group) {
			Rectangle imageRect = CalcLargeImageRect(e, r);
			Size textSize = CalcTextSize(e, 10000);
			return 2 * LargeLinkContentIndent + Math.Max(textSize.Width, imageRect.Width);
		}
		protected virtual Rectangle CalcLargeBounds(ObjectInfoArgs e, Rectangle source) {
			NavLinkInfoArgs li = e as NavLinkInfoArgs;
			Rectangle r = source;
			r.Y += CalcLargeVertIndent(e);
			Rectangle res = CalcLargeImageRect(e, r);
			li.ImageRectangle = res;
			res = r;
			if(li.ImageRectangle.Height > 0)
				res.Y = li.ImageRectangle.Bottom + 2;
			res.Inflate(-2, 0);
			r.Height = res.Height;
			Size textSize = li.Link.AllowHtmlString ? CalcHtmlStringSize(e, res.Width) : CalcTextSize(e, res.Width);
			res.Height = textSize.Height + 2;
			li.CaptionRectangle = res;
			AppearanceObject app = GetLinkAppearance(e);
			switch(app.HAlignment) {
				case HorzAlignment.Near:
					break;
				case HorzAlignment.Far:
					res.X = res.Right - textSize.Width;
					break;
				default:
					res.X = res.X + (res.Width - textSize.Width) / 2;
					break;
			}
			res.Width = textSize.Width;
			li.RealCaptionRectangle = res;
			r.Y = source.Y;
			r.Height = (li.CaptionRectangle.Bottom - source.Top) + CalcLargeVertIndent(e);
			li.Bounds = r;
			if(!li.ImageRectangle.Size.IsEmpty)
				li.HitRectangle = new Rectangle(li.ImageRectangle.Location, new Size(res.Right - li.ImageRectangle.Location.X, res.Bottom - li.ImageRectangle.Location.Y));
			else
				li.HitRectangle = li.CaptionRectangle;
			return r;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Empty;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavLinkInfoArgs linkInfo = e as NavLinkInfoArgs;
			CustomDrawNavBarElementEventArgs custom = CreateDrawArgs(e);
			RaiseDrawLink(custom);
			if(custom.Handled) return;
			DrawLinkBackground(linkInfo);
			DrawLinkCore(linkInfo, custom);
			DrawLinkSelection(linkInfo);
		}
		protected virtual void DrawLinkBackground(NavLinkInfoArgs linkInfo) {
		}
		protected virtual void DrawLinkSelection(NavLinkInfoArgs e) {
			if(!NavBar.DesignManager.IsComponentSelected(e.Link.Item)) return;
			NavBar.DesignManager.DrawSelection(e.Cache, e.Bounds, GetLinkAppearance(e).BackColor);
		}
		protected virtual void DrawLinkCore(NavLinkInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			NavBarItemLink link = GetLink(e);
			if(link.Group.GetShowAsIconsView()) {
				DrawListView(custom);
				return;
			}
			if(link.Group.GetLinksUseSmallImage()) 
				DrawSmall(custom);
			else 
				DrawLarge(custom);
		}
		protected CustomDrawNavBarElementEventArgs CreateDrawArgs(ObjectInfoArgs e) {
			CustomDrawNavBarElementEventArgs args = new CustomDrawNavBarElementEventArgs(e, e.Bounds, GetLinkAppearance(e));
			NavBarItemLink link = GetLink(e);
			args.Caption = link.Caption;
			args.Image = link.GetImage();
			return args;
		}
		protected virtual void RaiseDrawLink(CustomDrawNavBarElementEventArgs custom) {
			NavBar.RaiseCustomDrawLink(custom);
		}
		protected virtual void DrawLinkImage(CustomDrawNavBarElementEventArgs custom, bool isSmall) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			Image image = custom.Image;
			Rectangle imageRect = li.ImageRectangle;
			DrawLinkImageBorder(custom.ObjectInfo, imageRect);
			if(image != null)  {
				Size imageSize = IsItemExists(li) ? li.Link.Item.GetPrefferedImageSize(image.Size) : image.Size;
				imageRect.X += (imageRect.Width - imageSize.Width) / 2;
				imageRect.Y += (imageRect.Height - imageSize.Height) / 2;
				imageRect.Size = imageSize;
				if(li.Link.Item.GetAllowGlyphSkinning()) {
					DevExpress.Utils.Paint.XPaint.Graphics.DrawImage(custom.Graphics, image, imageRect, new Rectangle(Point.Empty, imageRect.Size), ImageColorizer.GetColoredAttributes(custom.Appearance.ForeColor));
				}
				else if(!li.Link.Enabled) {
					DevExpress.Utils.Paint.XPaint.Graphics.DrawImage(custom.Graphics, image, imageRect, new Rectangle(Point.Empty, imageRect.Size), false);
				}
				else {
					Rectangle rect = new Rectangle(imageRect.Location, li.Link.Item.GetPrefferedImageSize(imageRect.Size));
					DevExpress.Utils.Paint.XPaint.Graphics.DrawImage(custom.Graphics, image, rect);
				}
			}
		}
		protected virtual void DrawLinkImageBorder(ObjectInfoArgs e, Rectangle r) {
			NavBarItemLink link = GetLink(e);
			Color borderColor = link.NavBar.PaintAppearance.GroupBackground.BorderColor;
			Brush dark = e.Cache.GetSolidBrush(ControlPaint.Dark(borderColor));
			Brush light = e.Cache.GetSolidBrush(ControlPaint.LightLight(borderColor));
			if(IsDrawLinkHotOrPressed(e)) {
				if(e.State == ObjectState.Hot) {
					Brush darkSaved = dark;
					dark = light;
					light = darkSaved;
				}
				e.Graphics.FillRectangle(dark, new Rectangle(r.X, r.Y, r.Width - 1, 1));
				e.Graphics.FillRectangle(dark, new Rectangle(r.X, r.Y + 1, 1, r.Height - 1));
				e.Graphics.FillRectangle(light, new Rectangle(r.X, r.Bottom - 1, r.Width, 1));
				e.Graphics.FillRectangle(light, new Rectangle(r.Right - 1, r.Y, 1, r.Height - 1));
			}
		}
		protected virtual Brush GetLinkCaptionBrush(ObjectInfoArgs e) {
			AppearanceObject style = GetLinkAppearance(e);
			return style.GetForeBrush(e.Cache);
		}
		protected virtual Brush GetLinkCaptionDisabledBrush(ObjectInfoArgs e, Brush foreBrush) {
			if(foreBrush is SolidBrush) {
				Color color = ((SolidBrush)foreBrush).Color;
				return e.Cache.GetSolidBrush(ControlPaint.LightLight(color));
			}
			return null;
		}
		protected virtual void DrawLinkDisabledCaption(CustomDrawNavBarElementEventArgs custom, AppearanceObject appearance, Brush foreBrush, bool isSmall) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			Rectangle r = li.CaptionRectangle;
			if (li.Link.AllowHtmlString) {
				CalcHtmlStringInfo(li, custom.Caption, 0);
				StringPainter.Default.UpdateLocation(li.StringInfo, li.RealCaptionRectangle);
				StringPainter.Default.DrawString(custom.Cache, li.StringInfo);
			} else {
				Brush disabledBrush = GetLinkCaptionDisabledBrush(custom.ObjectInfo, foreBrush);
				if (disabledBrush != null) {
					r.Offset(1, 1);
					appearance.DrawString(custom.Cache, custom.Caption, r, custom.Appearance.Font, disabledBrush, GetLinkFormat(custom.ObjectInfo));
				}
				appearance.DrawString(custom.Cache, custom.Caption, li.CaptionRectangle, custom.Appearance.Font, foreBrush, GetLinkFormat(custom.ObjectInfo));
			}
		}
		protected virtual void DrawLinkCaption(CustomDrawNavBarElementEventArgs custom, bool isSmall) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			Brush foreBrush = GetLinkCaptionBrush(custom.ObjectInfo);
			if (!li.Link.Enabled)
				DrawLinkDisabledCaption(custom, custom.Appearance, foreBrush, isSmall);
			else {
				if (li.Link.AllowHtmlString) {
					CalcHtmlStringInfo(li, custom.Caption, 0);
					StringPainter.Default.UpdateLocation(li.StringInfo, li.RealCaptionRectangle);
					StringPainter.Default.DrawString(custom.Cache, li.StringInfo);
				} else
					custom.Appearance.DrawString(custom.Cache, custom.Caption, li.CaptionRectangle, custom.Appearance.Font, foreBrush, GetLinkFormat(custom.ObjectInfo));
			}
		}
		protected ISkinProvider GetISkinProvider() {
			if(NavBar != null) {
				if(NavBar.LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin)
					return DefaultSkinProvider.Default;
				ISkinProvider isp = NavBar as ISkinProvider;
				return isp;
			}
			return null;
		}
		protected SkinElement GetSkinElement() {
			DevExpress.Skins.Skin skin = DevExpress.Skins.CommonSkins.GetSkin(GetISkinProvider());
			var element = skin[DevExpress.Skins.CommonSkins.SkinLabelLine];
			if(element == null) {
				skin = DevExpress.Skins.CommonSkins.GetSkin(DefaultSkinProvider.Default);
				element = skin[DevExpress.Skins.CommonSkins.SkinLabelLine];
				if(element == null) return null;
			}
			return element;
		}
		protected virtual SkinElementInfo GetSeparatorInfo(Rectangle bounds) {
			return new SkinElementInfo(GetSkinElement(), bounds);
		}
		protected virtual Size GetSeparatorImageSize() {
			var element = GetSkinElement();
			if(element == null) return Size.Empty;
			return element.Size.MinSize;
		}
		protected virtual void DrawLinkSeparator(CustomDrawNavBarElementEventArgs custom) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			if(!li.Link.Item.IsSeparator()) return;
			Rectangle r = custom.RealBounds;
			r.Height = GetSeparatorImageSize().Height;
			r.X = custom.RealBounds.X + (custom.RealBounds.Width - r.Width) / 2;
			r.Y = custom.RealBounds.Y + (custom.RealBounds.Height - r.Height) / 2;
			ObjectPainter.DrawObject(custom.Cache, SkinElementPainter.Default, GetSeparatorInfo(r));
		}
		protected virtual void DrawListView(CustomDrawNavBarElementEventArgs custom) {
			DrawLinkImage(custom, true);
		}
		protected virtual void DrawSmall(CustomDrawNavBarElementEventArgs custom) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			if(li.Link.Item.IsSeparator()) {
				DrawLinkSeparator(custom);
				return;
			}
			DrawLinkImage(custom, true);
			DrawLinkCaption(custom, true);
		}
		protected virtual void DrawLarge(CustomDrawNavBarElementEventArgs custom) {
			NavLinkInfoArgs li = custom.ObjectInfo as NavLinkInfoArgs;
			if(li.Link.Item.IsSeparator()) {
				DrawLinkSeparator(custom);
				return;
			}
			DrawLinkImage(custom, false);
			DrawLinkCaption(custom, false);
		}
		protected virtual string GetCaption(ObjectInfoArgs e) {
			NavBarItemLink link = GetLink(e);
			string text = link.Caption;
			if(text == null || text.Length == 0 || text.Trim().Length == 0) text = "Wg";
			return text;
		}
		public virtual Size CalcTextSize(ObjectInfoArgs e, int maxWidth) {
			NavBarItemLink link = GetLink(e);
			if(link.Item.IsSeparator()) return GetSeparatorSize(e);
			string text = link.Caption;
			Size size = GetLinkAppearance(e).CalcTextSize(e.Graphics, GetLinkFormat(e), GetCaption(e), maxWidth).ToSize();
			size.Width ++;
			size.Height ++;
			return size;
		}
		static ObjectState[] objectStateValues = (ObjectState[])Enum.GetValues(typeof(ObjectState));
		public virtual Size CalcBestTextSize(ObjectInfoArgs e, int maxWidth) {
			NavBarItemLink link = GetLink(e);
			if(link.Item.IsSeparator()) return GetSeparatorSize(e);
			NavBarControl nb = link.NavBar;
			if (nb != null && nb.ViewInfo != null && nb.ViewInfo.LinkSizesCache.ContainsKey(link)) {
				if(link.AllowHtmlString)
					CalcHtmlStringInfo(e, GetCaption(e), maxWidth);
				return nb.ViewInfo.LinkSizesCache[link];
			}			 
			NavLinkInfoArgs infoArgs = new NavLinkInfoArgs(e.Cache, null, Rectangle.Empty, ObjectState.Normal);
			infoArgs.Assign(e);
			infoArgs.State = ObjectState.Normal;
			Size sz = Size.Empty;
			if (link.AllowHtmlString) {
				sz = CalcHtmlStringSize(e, maxWidth);
			} else {
				sz = CalcTextSize(infoArgs, maxWidth);
				int bestWidth = sz.Width,
					bestHeight = sz.Height;
				foreach(ObjectState current in objectStateValues) {
				if(current == ObjectState.Normal)
					continue;
				infoArgs.State = current;
				if(!GetLinkAppearance(infoArgs).Options.UseFont)
					continue;
				Size size = CalcTextSize(infoArgs, maxWidth);
				bestWidth = Math.Max(bestWidth, size.Width);
				bestHeight = Math.Max(bestHeight, size.Height);
				}
				sz = new Size(bestWidth, bestHeight);
			}	
			if(nb != null && nb.ViewInfo != null)
				nb.ViewInfo.AddItemToCache(link, sz);
			return sz;
		}
		protected virtual Size GetSeparatorSize(ObjectInfoArgs e) {
			return new Size(e.Bounds.Size.Width, GetSeparatorHeight());
		}
		protected virtual Rectangle GetSeparatorBounds(Point location, ObjectInfoArgs e) {
			return new Rectangle(location, GetSeparatorSize(e));
		}
		public virtual Size CalcHtmlStringSize(ObjectInfoArgs e, int maxWidth) {
			NavLinkInfoArgs linfo = e as NavLinkInfoArgs;
			NavBarItemLink link = GetLink(e);
			if (link.Item.IsSeparator()) return GetSeparatorSize(e);
			CalcHtmlStringInfo(linfo, GetCaption(e), maxWidth);
			Size size = linfo.StringInfo.Bounds.Size;
			size.Width++;
			size.Height++;
			return size;
		}
		public virtual void CalcHtmlStringInfo(ObjectInfoArgs e, string caption, int maxWidth) {
			NavLinkInfoArgs linfo = e as NavLinkInfoArgs;
			AppearanceObject linkAppearance = GetLinkAppearance(e);
			StringInfo stringInfo = StringPainter.Default.Calculate(e.Graphics, linkAppearance, null, caption, maxWidth, null, linfo.Link.NavBar.ViewInfo.GetHtmlDrawContext());
			linfo.StringInfo = stringInfo;
		}
	}
	public class NavBarSelectedLinkChangedEventArgs : EventArgs {
		NavBarGroup group;
		NavBarItemLink link;
		public NavBarSelectedLinkChangedEventArgs(NavBarGroup group, NavBarItemLink link) {
			this.group = group;
			this.link = link;
		}
		public NavBarGroup Group { get { return group; } }
		public NavBarItemLink Link { get { return link; } }
	}
	public delegate void NavBarSelectedLinkChangedEventHandler(object sender, NavBarSelectedLinkChangedEventArgs e);
	public class CustomDrawObjectEventArgs : EventArgs {
		ObjectInfoArgs objectInfo;
		bool handled;
		AppearanceObject appearance;
		Rectangle realBounds;
		public CustomDrawObjectEventArgs(ObjectInfoArgs objectInfo, Rectangle realBounds, AppearanceObject appearance) {
			this.objectInfo = objectInfo;
			this.appearance = appearance;
			this.realBounds = realBounds;
			this.handled = false;
		}
		public virtual Rectangle RealBounds {
			get { return realBounds; }
		}
		protected void SetRealBounds(Rectangle bounds) {
			this.realBounds = bounds;
		}
		public ObjectInfoArgs ObjectInfo {
			get { return objectInfo; }
		}
		public virtual AppearanceObject Appearance {
			get { return appearance; }
		}
		public virtual bool Handled { 
			get { return handled; }
			set { handled = value;
			}
		}
		public virtual Graphics Graphics {
			get { return ObjectInfo.Graphics; }
		}
		public virtual GraphicsCache Cache {
			get { return ObjectInfo.Cache; }
		}
	}
	public delegate void CustomDrawObjectEventHandler(object sender, CustomDrawObjectEventArgs e);
	public class CustomDrawNavBarElementEventArgs : CustomDrawObjectEventArgs  {
		string caption;
		Image image;
		public CustomDrawNavBarElementEventArgs(ObjectInfoArgs objectInfo, Rectangle realBounds, AppearanceObject appearance) : base(objectInfo, realBounds, appearance) {
			this.image = null;
			this.caption = "";
		}
		public string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				caption = value;
			}
		}
		public Image Image { 
			get { return image; }
			set { image = value; }
		}
	}
	public delegate void CustomDrawNavBarElementEventHandler(object sender, CustomDrawNavBarElementEventArgs e);
	public class NavBarViewInfo : IDisposable, IStringImageProvider {
		int scrollRange;
		GraphicsInfo gInfo;
		NavBarControl navBar;
		ArrayList groups;
		bool disposed = false;
		Timer hintTimer, buttonTimer;
		BorderPainter borderPainter;
		DevExpress.Utils.Win.ToolTipWindow toolTip;
		protected bool fIsReady;
		Rectangle upButtonBounds, downButtonBounds, clientRectangle;
		Rectangle bounds;
		NavBarHitInfo hotInfo;
		NavBarHitInfo pressedInfo;
		NavBarHitTest[] validHotTracks, validPressedInfo;
		NavBarState[] validPressedStateInfo;
		NavBarState state;
		NavBarDragDropInfo dragDropInfo;
		NavBarDropTargetArgs dropTarget;
		NavBarHintInfo lastHintInfo;
		NavBarAppearances paintAppearance;
		NavVScrollBar scrollBar;
		const int VScrollStep = 12;
		protected Rectangle scrollBarRectangle;
		bool scrollBarVisible;
		public NavBarViewInfo(NavBarControl navBar) {
			this.scrollBar = new NavVScrollBar();
			this.scrollBarRectangle = Rectangle.Empty;
			DevExpress.XtraEditors.ScrollBarBase.ApplyUIMode(this.scrollBar);
			this.ScrollBar.Minimum = 0;
			this.ScrollBar.SmallChange = VScrollStep;
			this.scrollBar.ValueChanged += new EventHandler(OnScrollBarValueChanged);
			this.paintAppearance = new NavBarAppearances();
			this.lastHintInfo = null;
			this.toolTip = new DevExpress.Utils.Win.ToolTipWindow();
			this.gInfo = new GraphicsInfo();
			this.toolTip.MouseTransparent = true;
			this.toolTip.ToolTipCustomDraw += new DevExpress.Utils.Win.ToolTipCustomDrawEventHandler(OnCustomDrawToolTip);
			this.toolTip.ToolTipCalcSize += new DevExpress.Utils.Win.ToolTipCalcSizeEventHandler(OnCalcToolTipSize);
			this.dropTarget = new NavBarDropTargetArgs();
			this.state = NavBarState.Normal;
			this.navBar = navBar;
			this.hotInfo = CreateHitInfo();
			this.pressedInfo = CreateHitInfo();
			this.borderPainter = CreateBorderPainter();
			this.fIsReady = false;
			this.groups = new ArrayList();
			this.validHotTracks = CreateValidHotTracks();
			this.validPressedStateInfo = CreateValidPressedStateInfo();
			this.validPressedInfo = CreateValidPressedInfo();
			this.buttonTimer = new Timer();
			this.buttonTimer.Interval = ButtonAutoRepeatInterval;
			this.buttonTimer.Tick += new EventHandler(OnButtonAutoRepeat);
			this.hintTimer = new Timer();
			this.hintTimer.Interval = ShowHintInterval;
			this.hintTimer.Tick += new EventHandler(OnHintTimer);
			Clear();
			this.dragDropInfo = new NavBarDragDropInfo(this, CreateHitInfo());
		}
		#region IStringImageProvider Members
		Image IStringImageProvider.GetImage(string id) {
			if (NavBar == null || NavBar.HtmlImages == null) return null;
			return NavBar.HtmlImages.Images[id];
		}
		#endregion
		internal DevExpress.Accessibility.BaseAccessible dxAccessible;
		protected internal virtual void ResetAccessible() {
			this.dxAccessible = null;
		}
		protected internal virtual DevExpress.Accessibility.BaseAccessible DXAccessible { 
			get {
				if(dxAccessible == null) dxAccessible = CreateAccessibleInstance();
				return dxAccessible;
			}
		}
		protected virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.XtraNavBar.Accessibility.BaseNavBarAccessible(NavBar, this); 
		}
		protected virtual int ButtonAutoRepeatInterval { get { return 300; } }
		public virtual void Dispose() {
			if(HintTimer != null) {
				HintTimer.Dispose();
				this.hintTimer = null;
			}
			if(ButtonTimer != null) {
				ButtonTimer.Dispose();
				this.buttonTimer = null;
			}
			if(ToolTip != null) {
				this.toolTip.ToolTipCustomDraw -= new DevExpress.Utils.Win.ToolTipCustomDrawEventHandler(OnCustomDrawToolTip);
				this.toolTip.ToolTipCalcSize -= new DevExpress.Utils.Win.ToolTipCalcSizeEventHandler(OnCalcToolTipSize);
				this.toolTip.Dispose();
				this.toolTip = null;
			}
			if(PaintAppearance != null) {
				this.paintAppearance.Dispose();
			}
			if(scrollBar != null) {
				this.scrollBar.ValueChanged -= new EventHandler(OnScrollBarValueChanged);
				this.scrollBarVisible = false;
				this.scrollBar.Dispose();
			}
			this.disposed = true;
		}
		int lockScroll = 0;
		protected virtual void OnScrollBarValueChanged(object sender, EventArgs e) {
			if(this.lockScroll != 0) return;
			NavBar.DesignManager.SelectComponent(NavBar, SelectionTypes.Replace);
			TopY = ScrollBar.Value;
			Clear();
			NavBar.Invalidate();
		}
		protected void BeginScrollUpdate() {
			lockScroll++;
		}
		protected void EndScrollUpdate() {
			lockScroll--;
		}
		public virtual bool ScrollBarVisible {
			get { return scrollBarVisible; }
			set {
				if(ScrollBarRectangle.IsEmpty && value) value = false;
				if(ScrollBarVisible == value) return;
				scrollBarVisible = value;
				OnScrollBarVisibleChanged();
			}
		}
		protected virtual void OnScrollBarVisibleChanged() {
			if(ScrollBarVisible) {
				ScrollBar.Bounds = ScrollBarRectangle;
				ScrollBar.Parent = NavBar;
				ScrollBar.SetVisibility(true);
			}
			else {
				ScrollBar.SetVisibility(false);
				ScrollBar.Parent = null;
			}
			if(NavBar.ExplorerBarStretchLastGroup) Calc(Bounds);
		}
		public int ScrollRange { get { return scrollRange; } set { scrollRange = value; } }
		public virtual NavVScrollBar ScrollBar { get { return scrollBar; } }
		public Rectangle ScrollBarRectangle { get { return scrollBarRectangle; } }
		public bool IsDisposed { get { return disposed; } }
		public virtual NavBarGroupStyle GetDefaultGroupStyle() { return NavBarGroupStyle.LargeIconsText; }
		bool paintAppearanceDirty;
		public virtual NavBarAppearances PaintAppearance { 
			get { 
				if(paintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance; 
			} 
		}
		public void SetPaintAppearanceDirty() { this.paintAppearanceDirty = true; }
		protected internal NavBarHitTest[] ValidHotTracks { get { return validHotTracks; } }
		protected internal NavBarHitTest[] ValidPressedInfo { get { return validPressedInfo; } }
		protected internal NavBarState[] ValidPressedStateInfo { get { return validPressedStateInfo; } }
		protected virtual BorderPainter CreateBorderPainter() {
			if(NavBar == null) return new EmptyBorderPainter();
			if(NavBar.BorderStyle == BorderStyles.Default) return CreateDefaultBorderPainter();
			return BorderHelper.GetPainter(NavBar.BorderStyle);
		}
		protected virtual BorderPainter CreateDefaultBorderPainter() {
			return new EmptyBorderPainter();
		}
		public BorderPainter BorderPainter { get { return borderPainter; } }
		public virtual GraphicsInfo GInfo { get { return gInfo; } }
		public NavBarDropTargetArgs DropTarget { 
			get { return dropTarget; } 
			set {
				if(value == null) return;
				if(DropTarget.IsEquals(value)) return;
				Invalidate(DropTarget);
				dropTarget = value;
				Invalidate(DropTarget);
			}
		}
		public void UpdateGroupPaintIndexes() { 
			for(int n = 0; n < Groups.Count; n++) {
				((NavGroupInfoArgs)Groups[n]).PaintIndex = n;
			}
		}
		public Timer HintTimer { get { return hintTimer; } }
		public Timer ButtonTimer { get { return buttonTimer; } }
		public NavBarDragDropInfo DragDropInfo { get { return dragDropInfo; } }
		public Rectangle Bounds { get { return bounds; } }
		public Rectangle Client { get { return clientRectangle; } set { clientRectangle = value; } }
		public NavBarControl NavBar { get { return navBar; } }
		public ArrayList Groups {
			get { return groups; }
		}
		protected Rectangle CheckBounds(Rectangle bounds) {
			if(IsRightToLeft) return new Rectangle(NavBar.Width - bounds.Right, bounds.Y, bounds.Width, bounds.Height);
			return bounds;
		}
		protected internal bool IsRightToLeft {
			get {
				if(NavBar == null) return false;
				return NavBar.IsRightToLeft;
			}
		}
		protected virtual bool IsLinkVisible(NavBarItemLink link) {
			NavLinkInfoArgs linkInfo = GetLinkInfo(link);
			if(linkInfo == null || !Client.Contains(linkInfo.Bounds)) return false;
			return true;
		}
		public virtual void MakeGroupVisible(NavBarGroup group) { }
		public virtual void MakeLinkVisible(NavBarItemLink link) {
			if(IsLinkVisible(link)) return;
			if(!link.Visible || link.Group == null || !link.Group.IsVisible || !link.Item.IsVisible) return;
			link.Group.Expanded = true;
			if(!link.Group.Expanded || IsLinkVisible(link)) return;
			for(int n = 0; n < link.Group.VisibleItemLinks.Count; n++) {
				link.Group.TopVisibleLinkIndex = n;
				if(IsLinkVisible(link)) return;
			}
		}
		public virtual void Clear() {
			Groups.Clear();
			this.clientRectangle = this.upButtonBounds = this.downButtonBounds = this.bounds = Rectangle.Empty;
			fIsReady = false;
		}
		public virtual bool IsReady { get { return fIsReady; } }
		public void Calc(Rectangle bounds) {
			Clear();
			this.bounds = bounds;
			GInfo.AddGraphics(null);
			try {
				CalcViewInfo(bounds);
				UpdateGroupBounds();
				fIsReady = true;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		public virtual int GetGroupTopVisibleLinkIndex(NavBarGroup group) {
			return group == null ? 0 : group.TopVisibleLinkIndex;
		}
		protected void UpdatePaintAppearance() {
			this.paintAppearanceDirty = false;
			UpdatePaintAppearanceCore();
		}
		protected virtual void UpdatePaintAppearanceCore() {
			paintAppearance.Combine(NavBar.Appearance, NavBar.View.GetAppearanceDefaultInfo(NavBar));
			paintAppearance.UpdateRightToLeft(IsRightToLeft);
		}
		internal bool inUpdateGroupBounds = false;
		protected virtual void UpdateGroupBounds() {
			if(this.inUpdateGroupBounds)
				return;
			this.inUpdateGroupBounds = true;
			try {
				foreach(NavBarGroup group in NavBar.Groups) {
					NavGroupInfoArgs groupArgs = GetGroupInfo(group);
					if(!group.IsVisible || groupArgs == null || groupArgs.ClientInfo.ClientInnerBounds == Rectangle.Empty || NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed)
						group.UpdateControlContainer(Rectangle.Empty);
					else
						group.UpdateControlContainer(groupArgs.ClientInfo.ClientInnerBounds);
				}
			}
			finally {
				UpdateWrappers();
				this.inUpdateGroupBounds = false;
			}
		}
		private void UpdateWrappers() {
			foreach(Control control in NavBar.Controls) {
				NavBarGroupControlContainerWrapper wrapper = control as NavBarGroupControlContainerWrapper;
				if(wrapper != null)
					wrapper.Refresh();
			}
		}
		public virtual bool AllowSelectedLink { get { return true; } }
		public virtual bool AllowScrollBar { get { return false; } }
		public virtual bool AllowLinkDrag { get { return true; } }
		public virtual bool AllowGroupDrag { get { return false; } }
		public virtual bool AllowSelectDisabledLink { get { return false; } }
		public virtual bool AllowHideGroupCaptions { get { return true; } }
		public virtual bool AllowListViewMode { get { return true; } }
		public virtual bool AllowOnlySmallImages { get { return false; } }
		public virtual bool AllowExpandCollapse { get { return false; } }
		protected internal virtual bool AllowDrawCaption { get { return true; } }
		public virtual Rectangle UpButtonBounds { get { return upButtonBounds; } set { upButtonBounds = value; } }
		public virtual Rectangle DownButtonBounds { get { return downButtonBounds; } set { downButtonBounds = value; } }
		protected virtual int CalcGroupVertIndent(NavGroupInfoArgs ia) {
			return 0;
		}
		protected virtual void OnCustomDrawToolTip(object sender, DevExpress.Utils.Win.ToolTipCustomDrawEventArgs e) {
			if(LastHintInfo == null) return;
			NavBarCustomDrawHintEventArgs args = new NavBarCustomDrawHintEventArgs(LastHintInfo, e.PaintArgs, ((Control)sender).ClientRectangle);
			NavBar.RaiseCusomDrawHint(args);
			e.Handled = args.Handled;
			if(args.Handled) return;
			Control hint = sender as Control;
			if(!object.Equals(hint.Font, args.Appearance.Font)) 
				hint.Font = args.Appearance.Font;
			hint.BackColor = args.Appearance.BackColor;
			hint.ForeColor = args.Appearance.ForeColor;
		}
		protected virtual void OnCalcToolTipSize(object sender, DevExpress.Utils.Win.ToolTipCalcSizeEventArgs e) {
			if(LastHintInfo == null) return;
			NavBarCalcHintSizeEventArgs args = new NavBarCalcHintSizeEventArgs(LastHintInfo, e.Size);
			NavBar.RaiseCalcHintSize(args);
			e.Size = args.Size;
		}
		protected virtual NavBarHintInfo LastHintInfo { get { return lastHintInfo; } }
		protected virtual DevExpress.Utils.Win.ToolTipWindow ToolTip { get { return toolTip; } }
		protected virtual int ShowHintInterval { get { return NavBar.ShowHintInterval; } }
		protected virtual void OnHintTimer(object sender, EventArgs e) { 
			ShowHint(); 	
			StopTimer();
		}
		protected virtual string GetLinkHint(NavBarItemLink link) {
			bool iconsView = link.Group.GetShowAsIconsView();
			string hint = link.Item.Hint;
			if(iconsView) {
				if(hint == null || hint.Trim().Length == 0) return link.Caption;
				return string.Format("{0}: {1}", link.Caption, hint);
			}
			return hint;
		}
		protected virtual bool GetCanShowHint(NavBarHitInfo hitInfo) {
			if(hitInfo.InGroupCaption) return NavBar.ShowGroupHint;
			if(hitInfo.InLink) return NavBar.ShowLinkHint;
			return false;
		}
		protected virtual NavBarHintInfo CalcHintInfo(NavBarHitInfo hitInfo) {
			NavBarHintInfo hint = new NavBarHintInfo(PaintAppearance.Hint);
			if(hitInfo.InLink) {
				hint.SetHint(NavBarHintObjectType.Link, hitInfo.Link, GetLinkHint(hitInfo.Link), hitInfo.Link.Item.SuperTip);
			}
			if(hitInfo.InGroupCaption) {
				hint.SetHint(NavBarHintObjectType.Group, hitInfo.Group, hitInfo.Group.Hint, hitInfo.Group.SuperTip);
			}
			return hint;
		}
		internal object GetHtmlDrawContext() { return this; }
		public void ShowHint() {
			NavBarHintInfo hint = NavBarHintInfo.Empty;
			if(GetCanShowHint(HotInfo)) hint = CalcHintInfo(HotInfo);
			if(hint.Show) {
				NavBarGetHintEventArgs getHint = new NavBarGetHintEventArgs(hint);
				NavBar.RaiseGetHint(getHint);
				if(getHint.HintInfo.Show && hint.Text != null && hint.Text.Length > 0) {
					this.lastHintInfo = hint;
					ToolTip.Font = hint.Appearance.Font;
					ToolTip.BackColor = hint.Appearance.BackColor;
					ToolTip.ForeColor = hint.Appearance.ForeColor;
					ToolTip.ToolTip = hint.Text;
					Point p = CalcHintPosition();
					ToolTip.ShowTip(p, new Point(p.X, p.Y + 10));
					return;
				}
			}
			HideHint();
		}
		protected virtual Point CalcHintPosition() {
			Point p = Control.MousePosition;
			if(Cursor.Current != null)
				p.Offset(Cursor.Current.Size.Width, -5);
			return p;
		}
		public virtual void HideHint() {
			ToolTip.HideTip();
			bool needUpdate = LastHintInfo != null && LastHintInfo.ObjectType == NavBarHintObjectType.Group;
			if(needUpdate) Application.DoEvents();
			this.lastHintInfo = null;
		}
		protected internal virtual void StopTimer() {
			HintTimer.Stop();
		}
		public virtual bool AllowTooltipController { get { return true; } }
		ToolTipControlInfo lastToolInfo = null;
		public virtual ToolTipControlInfo GetTooltipObjectInfo(NavBarHitInfo hitInfo) {
			if(GetCanShowHint(hitInfo)) {
				NavBarHintInfo hintInfo = CalcHintInfo(hitInfo);
				NavBarGetHintEventArgs getHint = new NavBarGetHintEventArgs(hintInfo);
				NavBar.RaiseGetHint(getHint);
				ToolTipControlInfo toolInfo = new ToolTipControlInfo(hintInfo, hintInfo.Show ? hintInfo.Text : string.Empty);
				if(lastToolInfo != null) {
					if(object.Equals(hintInfo, lastToolInfo.Object)) {
						toolInfo = lastToolInfo;
						toolInfo.Object = hintInfo;
						toolInfo.ForcedShow = DefaultBoolean.Default;
					}
				}
				this.lastToolInfo = toolInfo;
				toolInfo.Interval = CalcInterval(ShowHintInterval, hintInfo);
				toolInfo.SuperTip = hintInfo.SuperTip;
				return toolInfo;
			}
			this.lastToolInfo = null;
			return null;			
		}
		public virtual ToolTipControlInfo GetTooltipObjectInfo(Point point) {
			NavBarHitInfo hitInfo = CalcHitInfo(point);
			return GetTooltipObjectInfo(hitInfo);
		}
		protected virtual void RestartTimer(NavBarHintInfo newHintInfo) {
			HintTimer.Stop();
			HintTimer.Interval = CalcInterval(ShowHintInterval, newHintInfo);
			HintTimer.Start();
		}
		protected virtual int CalcInterval(int interval, NavBarHintInfo newHintInfo) {
			NavBarItemLink link = newHintInfo.HintObject as NavBarItemLink;
			if(link != null && link.Group.GetShowAsIconsView()) return interval / 2;
			return interval;
		}
		protected virtual Rectangle CalcClientRectangle(Rectangle bounds) {
			ObjectInfoArgs borderIA = new StyleObjectInfoArgs(null, bounds, PaintAppearance.GroupHeader, ObjectState.Normal);
			return BorderPainter.GetObjectClientRectangle(borderIA);
		}
		protected virtual void UpdatePainters() {
			this.borderPainter = CreateBorderPainter();
		}
		protected virtual void RemoveUnusedLinksFromCache() {
			if(LinkSizesCache.Count == 0)
				return;
			List<NavBarItemLink> linksToRemove = new List<NavBarItemLink>(LinkSizesCache.Count);
			foreach(NavBarItemLink link in LinkSizesCache.Keys) {
				bool found = false;
				foreach(NavBarGroup group in NavBar.Groups) {
					if(group.ItemLinks.Contains(link)) {
						found = true;
						break;
					}
				}
				if(!found)
					linksToRemove.Add(link);
			}
			foreach(NavBarItemLink link in linksToRemove)
				LinkSizesCache.Remove(link);
		}
		protected virtual void CalcViewInfo(Rectangle bounds) {
			UpdatePainters();
			GInfo.AddGraphics(GInfo.Graphics);
			try {
				this.clientRectangle = CalcClientRectangle(bounds);
				CalcGroupsViewInfo(Client);
				UpdateGroupPaintIndexes();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual int CalcGroupsViewInfo(Rectangle bounds) {
			Rectangle r = bounds;
			NavGroupInfoArgs expandedGroup = null;
			int maxBottom = r.Top;
			for(int n = 0; n < NavBar.Groups.Count; n++) {
				NavBarGroup group = NavBar.Groups[n];
				if(!group.IsVisible || (NavBar.GetHideGroupCaptions() && !group.Expanded)) continue;
				NavGroupInfoArgs groupArgs = new NavGroupInfoArgs(group, r);
				groupArgs.Graphics = GInfo.Graphics;
				int height = NavBar.GroupPainter.CalcObjectMinBounds(groupArgs).Height;
				groupArgs.Bounds = new Rectangle(r.Location, new Size(r.Width, height));
				NavBar.GroupPainter.CalcObjectBounds(groupArgs);
				groupArgs.Graphics = null;
				Groups.Add(groupArgs);
				if(expandedGroup == null) {
					if(group.Expanded) {
						expandedGroup = groupArgs;
					}
				}
				if(NavBar.GetHideGroupCaptions()) groupArgs.Bounds = new Rectangle(groupArgs.Bounds.Location, new Size(groupArgs.Bounds.Width, 0));
				r.Y = maxBottom = groupArgs.Bounds.Bottom;
				if(n != NavBar.Groups.Count - 1)
					 r.Y += CalcGroupVertIndent(groupArgs);
			}
			return CalcExpandedGroupInfo(expandedGroup, bounds, maxBottom);
		}
		protected int CalcExpandedGroupInfo(NavGroupInfoArgs expandedGroup, Rectangle bounds, int maxBottom) {
			int freeHeight = bounds.Bottom - maxBottom;
			int rest = CalcRestHeight(expandedGroup != null ? expandedGroup.Group : null);
			int minHeight = NavBar.GroupPainter.CalcMinGroupClientHeight(expandedGroup);
			int yOffset = (freeHeight > minHeight ? freeHeight : minHeight);
			Rectangle expandedRect = bounds;
			if(expandedGroup != null) {
				expandedRect.Y = expandedGroup.Bounds.Bottom;
				expandedRect.Height = yOffset;
				for(int n = Groups.Count - 1; n >= 0; n--) {
					NavGroupInfoArgs ga = Groups[n] as NavGroupInfoArgs;
					if(ga == expandedGroup) break;
					ga.OffsetContent(0, yOffset);
				}
				expandedGroup.ClientInfo.Bounds = expandedRect;
				expandedGroup.Graphics = GInfo.Graphics;
				NavBar.GroupPainter.ClientPainter.CalcObjectBounds(expandedGroup.ClientInfo);
				expandedGroup.Graphics = null;
			}
			CalcButtonPositions(expandedGroup, true);
			return maxBottom;
		}
		protected virtual Size CalcButtonSize(NavGroupInfoArgs activeGroupArgs) {
			UpDownButtonObjectInfoArgs args = new UpDownButtonObjectInfoArgs(activeGroupArgs.Cache, activeGroupArgs.Bounds, activeGroupArgs.Group.Appearance, activeGroupArgs.State, NavBar.ButtonPainter, true);
			return NavBar.ButtonPainter.CalcObjectMinBounds(args).Size;
		}
		protected virtual int CalcButtonIndent(IndentType indent) {
			return 6;
		}
		protected virtual bool IsScrollBarMode { 
			get {
				if(NavBar.SkinExplorerBarViewScrollStyle == SkinExplorerBarViewScrollStyle.Default) {
					if(DevExpress.XtraEditors.ScrollBarBase.GetUIMode(ScrollBar) == XtraEditors.ScrollUIMode.Touch) return true;
				}
				return NavBar.SkinExplorerBarViewScrollStyle == SkinExplorerBarViewScrollStyle.ScrollBar;
			}
		}
		protected virtual void CalcButtonPositions(NavGroupInfoArgs activeGroupArgs, bool updateScrollbar) {
			if(IsScrollBarMode) {
				CalcScrollBarPosition(activeGroupArgs, updateScrollbar);
				return;
			}
			if(activeGroupArgs == null) return;
			DownButtonBounds = UpButtonBounds = Rectangle.Empty;
			NavBarGroup group = activeGroupArgs.Group;
			Rectangle r = activeGroupArgs.ClientInfo.ClientBounds;
			if(r.IsEmpty) return;
			if(activeGroupArgs.Links.Count == 0) return;
			Size size = CalcButtonSize(activeGroupArgs);
			int bc = (GetGroupTopVisibleLinkIndex(group) > 0 ? 1 : 0) + (activeGroupArgs.LastLinkBounds.Bottom > r.Bottom ? 1 : 0);
			if(bc == 0) return;
			if(r.Height - ((size.Height + CalcButtonIndent(IndentType.Bottom)) * bc) < 0) {
				int nh = (r.Height - CalcButtonIndent(IndentType.Bottom)) / bc;
				size.Height = Math.Min(nh, size.Height);
			}
			if(size.Height < 0) return;
			if(GetGroupTopVisibleLinkIndex(group) > 0) {
				Rectangle button = r;
				button = r;
				button.Y += CalcButtonIndent(IndentType.Top);
				button.X = r.Right - (size.Width + CalcButtonIndent(IndentType.Right));
				button.Size = size;
				UpButtonBounds = button;
			}
			if(activeGroupArgs.LastLinkBounds.Bottom > r.Bottom) {
				Rectangle button = r;
				button.Y = button.Bottom - (CalcButtonIndent(IndentType.Bottom) + size.Height);
				button.X = r.Right - (size.Width + CalcButtonIndent(IndentType.Right));
				button.Size = size;
				DownButtonBounds = button;
			}
			UpButtonBounds = CheckBounds(UpButtonBounds);
			DownButtonBounds = CheckBounds(DownButtonBounds);
		}
		protected virtual void CalcScrollBarPosition(NavGroupInfoArgs activeGroupArgs, bool updateScrollbar) {
			this.scrollBarRectangle = Rectangle.Empty;
			if(activeGroupArgs == null) return;
			if(!updateScrollbar) return;
			this.scrollRange = 0;
			Rectangle r = activeGroupArgs.ClientInfo.ClientBounds;
			int h = activeGroupArgs.LinkAreaHeight;
			if(r.IsEmpty || (TopY == 0 && activeGroupArgs.LastLinkBounds.Bottom < r.Bottom) || (h != 0 && h <= r.Height)) return;
			scrollBarRectangle = r;
			if(!IsRightToLeft) r.X = r.Right - ScrollBar.GetDefaultVerticalScrollBarWidth();
			r.Width = ScrollBar.GetDefaultVerticalScrollBarWidth();
			this.scrollBarRectangle = r;
			BeginScrollUpdate();
			try {
				ScrollBar.Maximum = this.scrollRange = (activeGroupArgs.LastLinkBounds.Bottom - activeGroupArgs.ClientInfo.FirstLinkBounds.Y);
				ScrollBar.LargeChange = r.Height;
				ScrollBar.Value = TopY;
			}
			finally {
				EndScrollUpdate();
			}
		}
		public virtual int ClientLinkOffsetY { get { return TopY; } }
		public virtual bool IsExplorerBar { get { return false; } }
		int topY;
		public virtual int TopY {
			get {
				if(!IsScrollBarMode) return 0;
				return topY; 
			}
			set {
				if(!IsScrollBarMode) return;
				if(value >= ScrollRange) value = ScrollRange - 1;
				if(value >= ScrollBar.Maximum - ScrollBar.LargeChange) value = ScrollBar.Maximum - ScrollBar.LargeChange;
				if(value < 0) value = 0;
				if(value == TopY) return;
				topY = value;
				fIsReady = false;
				NavBar.Invalidate();
			}
		}
		protected virtual int CalcRestHeight(NavBarGroup group) {
			bool found = false;
			int res = 0;
			foreach(NavGroupInfoArgs ia in Groups) {
				if(ia.Group == group) {
					found = true;
					continue;
				}
				if(found) res += ia.Bounds.Height;
			}
			return res;
		}
		public virtual void Draw(PaintEventArgs e) {
			UpdateStates();
			GraphicsCache cache = new GraphicsCache(e);
			GraphicsInfoArgs info = new GraphicsInfoArgs(cache, Client);
			Rectangle r = Bounds;
			try {
				OnBeforeDraw(cache);
				DrawBorderAndBackground(cache, r);
				cache.ClipInfo.MaximumBounds = Client;
				GraphicsClipState clipState = cache.ClipInfo.SaveAndSetClip(Client);
				DrawGroups(cache);
				if(NavBar.OptionsNavPane.NavPaneState == NavPaneState.Expanded) DrawButtons(cache);
				if(!NavBar.Enabled) {
					BackgroundPaintHelper.PaintDisabledControl(IsSkinned ? NavBar.LookAndFeel : null, cache, Bounds);
				}
				cache.ClipInfo.RestoreClipRelease(clipState);
			}
			finally {
				OnAfterDraw(cache);
				cache.Dispose();
			}
		}
		protected virtual bool IsSkinned { get { return false; } }
		protected virtual void OnBeforeDraw(GraphicsCache e) {
			ScrollBarVisible = !ScrollBarRectangle.IsEmpty;
			if(ScrollBar.Bounds != ScrollBarRectangle)
				ScrollBar.Bounds = ScrollBarRectangle;
		}
		protected virtual void OnAfterDraw(GraphicsCache e) {
		}
		protected internal virtual ObjectState CalcGroupState(NavGroupInfoArgs groupInfo) {
			return groupInfo.Group.State;
		}
		protected internal virtual void CalcLinksState(NavGroupInfoArgs ga) {
			foreach(NavLinkInfoArgs li in ga.Links) {
				li.State = NavBar.LinkPainter.CalcLinkState(li.Link, li.Link.State);
			}
		}
		protected virtual void UpdateStates() {
			foreach(NavGroupInfoArgs ga in Groups) {
				ga.State = CalcGroupState(ga);
				CalcLinksState(ga);
			}
		}
		protected virtual void DrawButtons(GraphicsCache e) {
			DrawButton(e, UpButtonBounds, UpButtonState, true);
			DrawButton(e, DownButtonBounds, DownButtonState, false);
		}
		protected virtual void DrawButton(GraphicsCache e, Rectangle bounds, ObjectState state, bool isUpButton) {
			if(bounds.IsEmpty) return;
			AppearanceObject appearance = PaintAppearance.Button;
			switch(state) {
				case ObjectState.Selected : 
				case ObjectState.Pressed : appearance = PaintAppearance.ButtonPressed; break;
				case ObjectState.Hot : appearance = PaintAppearance.ButtonHotTracked; break;
				case ObjectState.Disabled : appearance = PaintAppearance.ButtonDisabled; break;
			}
			StyleObjectInfoArgs args = new UpDownButtonObjectInfoArgs(e, bounds, appearance, state, NavBar.GroupPainter.UpDownButtonPainter, isUpButton);
			NavBar.ButtonPainter.DrawObject(args);
		}
		protected internal virtual ObjectState UpButtonState {
			get { return CalcButtonState(NavBarState.UpButtonPressed, NavBarHitTest.UpButton); 	}
		}
		protected internal virtual ObjectState DownButtonState {
			get { return CalcButtonState(NavBarState.DownButtonPressed, NavBarHitTest.DownButton); 	}
		}
		protected virtual ObjectState CalcButtonState(NavBarState pressedState, NavBarHitTest btnHitTest) {
			NavBarHitInfo hitInfo = CalcHitInfo(MousePosition);
			if(State == pressedState) {
				if(hitInfo.HitTest != btnHitTest) return ObjectState.Normal;
				return ObjectState.Pressed;
			}
			if(State == NavBarState.Normal) {
				if(hitInfo.HitTest == btnHitTest) return ObjectState.Hot;
			}
			return ObjectState.Normal;
		}
		protected virtual void DrawBackground(GraphicsCache e, Rectangle r) {
			CustomDrawObjectEventArgs custom = new CustomDrawObjectEventArgs(new ObjectInfoArgs(e, r, ObjectState.Normal), r, PaintAppearance.Background);
			RaiseDrawBackground(custom);
			if(custom.Handled) return;
			DrawBackgroundCore(e, custom, r);
		}
		protected virtual void DrawBackgroundCore(GraphicsCache e, CustomDrawObjectEventArgs custom, Rectangle r) {
			custom.Appearance.FillRectangle(e, r);
		}
		protected virtual void RaiseDrawBackground(CustomDrawObjectEventArgs custom) {
			NavBar.RaiseCustomDrawBackground(custom);
		}
		protected virtual void DrawBorderAndBackground(GraphicsCache e, Rectangle r) {
			BorderObjectInfoArgs ia = new BorderObjectInfoArgs(e, r, PaintAppearance.Background);
			BorderPainter.DrawObject(ia);
			DrawBackground(e, Client);
		}
		protected virtual void DrawGroups(GraphicsCache cache) {
			for(int i = Groups.Count - 1; i >= 0; i--) {
				NavGroupInfoArgs groupInfo = Groups[i] as NavGroupInfoArgs;
				ObjectPainter.DrawObject(cache, NavBar.GroupPainter, groupInfo);
			}
		}
		protected internal virtual NavBarHitInfo HotInfo {
			get { return hotInfo; }
		}
		protected internal virtual NavBarHitInfo PressedInfo {
			get { return pressedInfo; }
		}
		protected virtual void Invalidate(NavBarDropTargetArgs target) {
			if(target.Group != null) {
				InvalidateGroupCaption(target.Group);
				InvalidateGroupClient(target.Group);
			}
		}
		public virtual void InvalidateHitObject(NavBarHitInfo hitInfo) {
			if(hitInfo.HitTest == NavBarHitTest.None) return;
			if(hitInfo.InLink) {
				InvalidateLink(hitInfo.Link);
				return;
			}
			if(hitInfo.InGroupCaption) {
				InvalidateGroupCaption(hitInfo.Group);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.GroupBottomButton || hitInfo.HitTest == NavBarHitTest.GroupCaptionButton) {
				NavGroupInfoArgs groupInfo = GetGroupInfo(hitInfo.Group);
				if(groupInfo != null) Invalidate(groupInfo.ButtonBounds);
				return;
			}
			if(hitInfo.InGroup) {
				InvalidateGroupClient(hitInfo.Group);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.UpButton) {
				Invalidate(UpButtonBounds);
				return;
			}
			if(hitInfo.HitTest == NavBarHitTest.DownButton) {
				Invalidate(DownButtonBounds);
				return;
			}
		}
		public virtual void InvalidateLink(NavBarItemLink link) {
			NavLinkInfoArgs li = GetLinkInfo(link);
			if(li == null) return;
			Invalidate(li.Bounds);
		}
		public virtual void InvalidateGroupCaption(NavBarGroup group) {
			NavGroupInfoArgs ga = GetGroupInfo(group);
			if(ga == null) return;
			Invalidate(ga.Bounds);
		}
		public virtual void InvalidateGroupClient(NavBarGroup group) {
			NavGroupInfoArgs groupInfo = GetGroupInfo(group);
			if(groupInfo == null || groupInfo.ClientInfo.Bounds.IsEmpty) return;
			Invalidate(groupInfo.ClientInfo.Bounds);
		}
		public virtual void Invalidate(Rectangle r) {
			if(NavBar == null || NavBar.Disposing) return;
			NavBar.Invalidate(r);
		}
		public virtual NavGroupInfoArgs GetGroupInfo(NavBarGroup group) {
			foreach(NavGroupInfoArgs e in Groups) {
				if(e.Group == group) return e;
			}
			return null;
		}
		public virtual NavLinkInfoArgs GetLinkInfo(NavBarItemLink link) {
			if(link == null) return null;
			NavGroupInfoArgs ga = GetGroupInfo(link.Group);
			if(ga == null) return null;
			foreach(NavLinkInfoArgs e in ga.Links) {
				if(e.Link == link) return e;
			}
			return null;
		}
		public void ClearPressedInfo() {
			NavBarHitInfo oldInfo = PressedInfo;
			this.pressedInfo = CreateHitInfo();
			InvalidateHitObject(oldInfo);
		}
		public void ClearHotInfo() { ClearHotInfo(true); }
		public void ClearHotInfo(bool hideHint) {
			if(hideHint) {
				StopTimer();
				HideHint();
			}
			NavBarHitInfo oldInfo = HotInfo;
			this.hotInfo = CreateHitInfo();
			InvalidateHitObject(oldInfo);
		}
		public virtual NavBarHitInfo CalcHitInfo(Point p) {
			NavBarHitInfo hitInfo = CreateHitInfo();
			hitInfo.CalcHitInfo(p, ValidHotTracks);
			return hitInfo;
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			if(GetFocusedControl() is NavBarGroupControlContainer) {
				return;
			}
			TopY -= e.Delta;
		}
		protected Control GetFocusedControl() {
			foreach(NavBarGroup group in NavBar.Groups) {
				if(group.ControlContainer != null && group.ControlContainer.Focused) return group.ControlContainer;
			}
			return null;
		}
		protected virtual bool AllowDesignClick(NavBarHitInfo hitInfo) {
			if(hitInfo.HitTest == NavBarHitTest.GroupCaptionButton) return true;
			if(hitInfo.HitTest == NavBarHitTest.GroupBottomButton) return true;
			if(hitInfo.HitTest == NavBarHitTest.DownButton || hitInfo.HitTest == NavBarHitTest.UpButton) return true;
			if(hitInfo.HitTest == NavBarHitTest.GroupCaption && !IsExplorerBar) {
				NavGroupInfoArgs groupInfo = GetGroupInfo(hitInfo.Group);
				if(groupInfo == null) return false;
				if(new Rectangle(groupInfo.Bounds.X, groupInfo.Bounds.Y, 10, groupInfo.Bounds.Height).Contains(hitInfo.HitPoint) ||
					new Rectangle(groupInfo.Bounds.Right - 20, groupInfo.Bounds.Y, 20, groupInfo.Bounds.Height).Contains(hitInfo.HitPoint))
						return true;
			}
			return false;
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			HideHint();
			NavBarHitInfo hitInfo = CalcHitInfo(new Point(e.X, e.Y));
			ClearHotInfo();
			if(e.Button == MouseButtons.Left) {
				ClearPressedInfo();
				int index = Array.IndexOf(ValidPressedInfo, hitInfo.HitTest);
				if(NavBar.IsDesignMode) {
					if(index != -1) {
						UpdateDesigner(hitInfo);
					}
					if(!AllowDesignClick(hitInfo)) return;
				}
				if(hitInfo.Link != null && !hitInfo.Link.Enabled && !AllowSelectDisabledLink) return;
				if(index == -1) return;
				NavBar.Capture = true;
				DoPress(hitInfo, ValidPressedStateInfo[index]);
			}
		}
		protected internal virtual void AccessibleNotifyClients(NavBarHitInfo hitInfo) {
#if DXWhidbey                
			int childId = 0;
			if(hitInfo.InGroup || hitInfo.InLink) childId = 1000 * hitInfo.NavBar.Groups.IndexOf(hitInfo.Group);
			if(hitInfo.InLink) {
				childId += hitInfo.Group.ItemLinks.IndexOf(hitInfo.Link);
				NavBar.AccessibleNotifyClients(AccessibleEvents.Selection, childId + 1);
				NavBar.AccessibleNotifyClients(AccessibleEvents.Focus, childId + 1);
			}
#endif
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			if(NavBar.IsDesignMode) {
				if(e.Button == MouseButtons.Right) {
					NavBar.DesignManager.OnRightClick(e);
					return;
				}
				if(!AllowDesignClick(PressedInfo)) return;
			}
			NavBarHitInfo hitInfo = CalcHitInfo(new Point(e.X, e.Y));
			if(e.Button == MouseButtons.Left) {
				bool equals = hitInfo.IsEquals(PressedInfo);
				bool isLinkPressed = PressedInfo.InLink;
				ClearPressedInfo();
				Form form = Form.ActiveForm;
				SetState(NavBarState.Normal);
				if(equals && (NavBar.Focused || NavBar.IsDesignMode)) {
					if(isLinkPressed && !NavBar.GetSelectLinkOnPress()) {
						NavBar.SelectedLink = hitInfo.Link;
					}
					DoClick(hitInfo);
				}
			}
			if(!IsDisposed) 
				UpdateHotInfo(MousePosition);
			NavBar.Capture = false;
		}
		public virtual Point MousePosition {
			get { 
				if(NavBar.IsDisposed) return Point.Empty;
				return NavBar.PointToClient(Control.MousePosition);
			}
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			if(NavBar.IsDesignMode) return;
			ScrollBar.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.MouseMove);
			if(e.Button == MouseButtons.None) 
				UpdateHotInfo(new Point(e.X, e.Y));
			UpdateCursor();
			if(e.Button == MouseButtons.Left) {
				Point p = MousePosition; 
				if(PressedInfo.InLink || PressedInfo.InGroup) {
					p.Offset(-PressedInfo.HitPoint.X, -PressedInfo.HitPoint.Y);
					if(Math.Abs(p.X) > 5 || Math.Abs(p.Y) > 5) {
						StartDragDrop(PressedInfo);
					}
				}
			}
		}
		protected virtual void UpdateDesigner(NavBarHitInfo hitInfo) {
			if(!NavBar.IsDesignMode || NavBar.Site == null) return;
			object obj = null;
			if(hitInfo.InGroupCaption) obj = hitInfo.Group;
			if(hitInfo.InLink) obj = hitInfo.Link.Item;
			if(obj == null) return;
			NavBar.DesignManager.SelectComponent(obj);
		}
		protected virtual void StartDragDrop(NavBarHitInfo pressedInfo) {
			if(pressedInfo.InLink && AllowLinkDrag && pressedInfo.Link.Item.CanDrag) {
				if((pressedInfo.Group.GetDragDropFlags() & NavBarDragDrop.AllowDrag) == 0) return;
				dragDropInfo.Dispose();
				dragDropInfo = new NavBarDragDropInfo(this, PressedInfo.Clone() as NavBarHitInfo);
				DragDropInfo.Start();
				ClearPressedInfo();
				SetState(NavBarState.Normal);
			}
			if(pressedInfo.InGroupCaption && AllowGroupDrag) {
				if((NavBar.GetDragDropFlags() & NavBarDragDrop.AllowDrag) == 0) return;
				dragDropInfo = new NavBarDragDropInfo(this, PressedInfo.Clone() as NavBarHitInfo);
				ClearPressedInfo();
				SetState(NavBarState.Normal);
			}
		}
		public virtual void OnMouseLeave(EventArgs e) {
			UpdateHotInfo(new Point(-10000, -10000));
			UpdateCursor();
			HideHint();
		}
		public virtual void OnMouseEnter(EventArgs e) {
			UpdateHotInfo(MousePosition);
			UpdateCursor();
		}
		protected virtual void SetCursor(Cursor newCursor) {
			NavBar.SetCursor(newCursor);
		}
		protected virtual void RestoreCursor() {
			NavBar.RestoreCursor();
		}
		protected virtual bool CanUpdateCursor { get { return !NavBar.IsDesignMode; } }
		protected virtual void UpdateCursor() {
			if(!CanUpdateCursor) return;
			NavBarHitInfo hitInfo = CalcHitInfo(MousePosition);
			if(hitInfo.Link != null) {
				if(hitInfo.Link.Item.IsSeparator()) {
					RestoreCursor();
					return;
				}
				if((hitInfo.Link.Enabled || AllowSelectDisabledLink) && NavBar.HotTrackedItemCursor != Cursors.Default) 
					SetCursor(NavBar.HotTrackedItemCursor);
				else 
					RestoreCursor();
				return;
			}
			if(hitInfo.InGroupCaption) {
				if(NavBar.HotTrackedGroupCursor != Cursors.Default) 
					SetCursor(NavBar.HotTrackedGroupCursor);
				else 
					RestoreCursor();
				return;
			}
			RestoreCursor();
		}
		protected internal virtual void DoPress(NavBarHitInfo hitInfo, NavBarState pressedState) {
			SetState(pressedState);
			this.pressedInfo = hitInfo;
			RaisePressedEvent(pressedState, hitInfo);
			InvalidateHitObject(PressedInfo);
			if(hitInfo.HitTest == NavBarHitTest.UpButton) 
				DoButtonPress(hitInfo, true);
			if(hitInfo.HitTest == NavBarHitTest.DownButton) 
				DoButtonPress(hitInfo, false);
			if(hitInfo.HitTest == NavBarHitTest.ExpandButton)
				DoExandButtonPress();
			if(hitInfo.HitTest == NavBarHitTest.ContentButton)
				DoContentButtonPress();
			AccessibleNotifyClients(hitInfo);
		}
		protected internal virtual void DoClick(NavBarHitInfo hitInfo) {
			if(hitInfo.InGroupCaption || hitInfo.HitTest == NavBarHitTest.GroupBottomButton) {
				DoGroupClick(hitInfo);
				return;
			}
			if(hitInfo.InLink) {
				DoLinkClick(hitInfo);
				return;
			}
		}
		protected virtual void OnButtonAutoRepeat(object sender, EventArgs e) {
			ButtonTimer.Stop();
			int prevTime = ButtonTimer.Interval;
			NavBarHitInfo hitInfo = CalcHitInfo(NavBar.PointToClient(Control.MousePosition));
			if(!hitInfo.IsEquals(PressedInfo)) return;
			if(PressedInfo.HitTest == NavBarHitTest.UpButton) {
				DoButtonPress(PressedInfo, true);
			}
			if(PressedInfo.HitTest == NavBarHitTest.DownButton) {
				DoButtonPress(PressedInfo, false);
			}
			ButtonTimer.Interval = Math.Max(prevTime / 2, 10);
		}
		protected internal void ActivateGroupContent() { DoContentButtonPress(); }
		protected virtual void DoExandButtonPress() {
			if(NavBar.OptionsNavPane.IsAnimationInProgress)
				return;
			NavBar.OptionsNavPane.allowAnimation = true;
			NavBar.OptionsNavPane.NavPaneState = InvertState(NavBar.OptionsNavPane.NavPaneState);
		}
		protected virtual void DoContentButtonPress() { }
		protected virtual NavPaneState InvertState(NavPaneState state) {
			return state == NavPaneState.Expanded ? NavPaneState.Collapsed : NavPaneState.Expanded;
		}
		protected void DoButtonPress(NavBarHitInfo hitInfo, bool isUp) {
			ObjectState buttonState = isUp ? UpButtonState : DownButtonState;
			if(buttonState == ObjectState.Disabled) return;
			OnButtonPress(hitInfo, isUp);
			ButtonTimer.Interval = ButtonAutoRepeatInterval;
			ButtonTimer.Start();
		}
		protected virtual internal void OnButtonPress(NavBarHitInfo hitInfo, bool isUp) {
			DoMoveTopLinkIndex(hitInfo.Group, isUp ? -1 : 1);
		}
		protected virtual bool ShouldMoveTopLinkIndex(NavBarGroup group, int delta) {
			if(group == null) return false;
			return true;
		}
		protected virtual void DoMoveTopLinkIndex(NavBarGroup group, int delta) {
			if(!ShouldMoveTopLinkIndex(group, delta)) return;
			int index = group.TopVisibleLinkIndex + delta;
			if(index < 0) index = 0;
			if(index > group.ItemLinks.Count + 1) index = group.ItemLinks.Count - 1;
			group.TopVisibleLinkIndex = index;
		}
		protected internal void DoGroupClick(NavBarGroup group) {
			NavBarHitInfo hi = CreateHitInfo();
			hi.SetGroup(group);
			DoGroupClick(hi);
		}
		protected virtual void DoGroupClick(NavBarHitInfo hitInfo) {
			NavBarGroup group = hitInfo.Group;
			if(!RaiseGroupExpanding(group)) return;
			group.Expanded = !group.Expanded;
			MakeGroupVisible(group);
		}
		protected virtual bool RaiseGroupExpanding(NavBarGroup group) {
			NavBarGroupCancelEventArgs e = new NavBarGroupCancelEventArgs(group);
			group.RaiseGroupExpanding(e);
			if(e.Cancel) return false;
			if(NavBar != null && NavBar.ActiveGroup != null) {
				NavBarGroupCancelEventArgs ee = new NavBarGroupCancelEventArgs(NavBar.ActiveGroup);
				NavBar.ActiveGroup.RaiseGroupExpanding(ee);
				if(ee.Cancel) return false;
			}
			return true;
		}
		protected virtual void DoLinkClick(NavBarHitInfo hitInfo) {
			if(!hitInfo.Link.Enabled && !AllowSelectDisabledLink) return;
			NavBar.RaiseLinkClicked(hitInfo.Link);
		}
		public virtual NavBarGroup PressedGroup {
			get { 
				if(State == NavBarState.GroupPressed) return PressedInfo.Group;
				return null;
			}
		}
		public virtual NavBarItemLink PressedLink {
			get { 
				if(State == NavBarState.LinkPressed) return PressedInfo.Link;
				return null;
			}
		}
		public virtual NavBarGroup HotTrackedGroup {
			get { 
				if(HotInfo.InGroupCaption) return HotInfo.Group;
				return null;
			}
		}
		public virtual NavBarItemLink HotTrackedLink {
			get { 
				if(HotInfo.InLink && (Array.IndexOf(ValidHotTracks, HotInfo.HitTest) != -1)) return HotInfo.Link;
				return null;
			}
		}
		protected virtual void RaisePressedEvent(NavBarState state, NavBarHitInfo hit) {
			switch(state) {
				case NavBarState.LinkPressed : 
					if(hit.Link.Enabled || AllowSelectDisabledLink) {
						NavBar.RaiseLinkPressed(hit.Link); 
						if(NavBar.Focused && NavBar.GetSelectLinkOnPress())
							hit.Group.SelectedLink = hit.Link;
					}
				break;
			}
		}
		protected internal virtual void RaiseHotEvent(NavBarHitInfo oldInfo, NavBarHitInfo newInfo) {
			NavBarItemLink oldLink = oldInfo.Link, newLink = newInfo.Link;
			if(Array.IndexOf(ValidHotTracks, newInfo.HitTest) == -1) newLink = null;
			if(Array.IndexOf(ValidHotTracks, oldInfo.HitTest) == -1) oldLink = null;
			if(!AllowTooltipController) {
				NavBarHintInfo newHintInfo = CalcHintInfo(newInfo);
				if(LastHintInfo != null && !Object.Equals(LastHintInfo, newHintInfo)) {
					HideHint();
					StopTimer();
				}
				if(newHintInfo.Show) RestartTimer(newHintInfo);
			}
			if(newLink != oldLink) {
				NavBar.RaiseHotTrackedLinkChanged(newLink); 
			}
		}
		protected virtual void SetState(NavBarState state) {
			if(state == State) return;
			ClearPressedInfo();
			ClearHotInfo();
			this.state = state;
		}
		public virtual NavBarState State { get { return state; } }
		protected virtual void UpdateHotInfo(Point p) {
			if(NavBar.IsDesignMode) return;
			NavBarHitInfo hitInfo = CalcHitInfo(p), oldInfo = HotInfo;
			if(!HotInfo.IsEquals(hitInfo)) {
				ClearHotInfo(false);
				hotInfo = hitInfo;
				if(Array.IndexOf(ValidHotTracks, HotInfo.HitTest) != -1)
					InvalidateHitObject(HotInfo);
				RaiseHotEvent(oldInfo, HotInfo);
			}
			hotInfo = hitInfo;
		}
		protected internal NavBarGroup GetExpandedGroup() {
			NavBarGroup firstVisible = null;
			for(int n = 0; n < NavBar.Groups.Count; n++) {
				NavBarGroup group = NavBar.Groups[n];
				if(!group.IsVisible) continue;
				if(firstVisible == null) firstVisible = group;
				if(group.Expanded) return group;
			}
			return firstVisible;
		}
		public virtual NavBarHitInfo CreateHitInfo() { return new NavBarHitInfo(NavBar); }
		protected virtual NavBarHitTest[] CreateValidHotTracks() {
			return new NavBarHitTest[] {
					NavBarHitTest.UpButton, NavBarHitTest.DownButton,
					NavBarHitTest.GroupCaption, NavBarHitTest.GroupCaptionButton, NavBarHitTest.GroupBottomButton,
					NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage };
		}
		protected virtual NavBarHitTest[] CreateValidPressedInfo() {
			return new NavBarHitTest[] {
					NavBarHitTest.UpButton, NavBarHitTest.DownButton,
					NavBarHitTest.GroupCaption, NavBarHitTest.GroupCaptionButton, NavBarHitTest.GroupBottomButton,
					NavBarHitTest.LinkCaption, NavBarHitTest.LinkImage };
		}
		protected virtual NavBarState[] CreateValidPressedStateInfo() {
			return new NavBarState[] {
					NavBarState.UpButtonPressed, NavBarState.DownButtonPressed,
					NavBarState.GroupPressed, NavBarState.GroupPressed, NavBarState.GroupPressed,
					NavBarState.LinkPressed, NavBarState.LinkPressed };
		}
		protected internal virtual void OnSelectedLinkChanged(NavBarGroup group) {
			if(!IsScrollBarMode || group == null) return;
			var info = GetGroupInfo(group);
			if(info == null) return;
			var linkInfo = GetLinkInfo(group.SelectedLink);
			if(linkInfo == null) return;
			if(info.ClientInfoBounds.IsEmpty) return;
			int newTopY = TopY;
			if(linkInfo.Bounds.Top < info.ClientInfoBounds.Top) {
				newTopY-= (info.ClientInfoBounds.Top - linkInfo.Bounds.Top);
			}
			if(linkInfo.Bounds.Bottom > info.ClientInfoBounds.Bottom) {
				newTopY += (linkInfo.Bounds.Bottom - info.ClientInfoBounds.Bottom);
			}
			TopY = newTopY;
		}
		LinkSizesCache linkSizesCache;
		protected internal LinkSizesCache LinkSizesCache {
			get {
				if(linkSizesCache == null)
					linkSizesCache = new LinkSizesCache();
				return linkSizesCache;
			}
		}
		protected internal void AddItemToCache(NavBarItemLink link, Size size) {
			LinkSizesCache.Add(link, size);
		}
		protected internal void ResetLinkSizesCache() {
			LinkSizesCache.Clear();
		}
		protected internal virtual void OnSizeChanged() {
			this.ScrollBar.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.Resize);
			ResetLinkSizesCache();
		}
		public bool IsAttachedToOfficeNavigationBar { get { return NavBar.IsAttachedToOfficeNavigationBar; } }
		protected internal virtual void EnsureGroupControls() {
		}
		protected internal virtual void OnAttachToOfficeNavigationBar() {
		}
		protected internal virtual void OnDetachToOfficeNavigationBar() {
		}
		protected internal void OnItemCollectionChanged() {
			ResetLinkSizesCache();
			Clear();
		}
		public static Rectangle CheckElementCaptionLocation(AppearanceObject obj, Rectangle caption, int maxWidth) {
			if(obj.Options.UseTextOptions) {
				if(obj.TextOptions.HAlignment == HorzAlignment.Center)
					caption.X += (maxWidth - caption.Width) / 2;
				else if(obj.TextOptions.HAlignment == HorzAlignment.Far)
					caption.X += maxWidth - caption.Width;
			}
			return caption;
		}
	}
	public class NavBarDesignTimeManager : BaseDesignTimeManager {
		ContextMenu groupMenu, itemMenu;
		MenuItem groupStyleMenu;
		MenuItem groupDelete, itemDelete, groupAddItem, groupAddSeparator;
		public NavBarDesignTimeManager(NavBarControl navBar) : base(navBar, null) {
			this.groupMenu = CreateGroupMenu();
			this.itemMenu = CreateItemMenu();
		}
		const int AddItemId = 4;
		protected virtual ContextMenu CreateGroupMenu() {
			ContextMenu menu = new ContextMenu();
			this.groupStyleMenu = new MenuItem("&Change GroupStyle");
			menu.Popup += new EventHandler(OnGroupStyleMenuPopup);
			menu.MenuItems.Add(new MenuItem("Add &group", new EventHandler(OnGroupMenuAddGroup)));
			menu.MenuItems.Add(groupDelete = new MenuItem("&Delete group", new EventHandler(OnGroupMenuDeleteGroup)));
			menu.MenuItems.Add(new MenuItem("-"));
			menu.MenuItems.Add(new MenuItem("&Expand group", new EventHandler(OnGroupMenuExpandGroup)));
			menu.MenuItems.Add(groupAddItem = new MenuItem("&Add item", new EventHandler(OnGroupMenuAddItem)));
			menu.MenuItems.Add(groupAddSeparator = new MenuItem("&Add separator", new EventHandler(OnGroupMenuAddSeparator)));
			menu.MenuItems.Add(this.groupStyleMenu);
			return menu;
		}
		protected virtual void UpdateGroupStyleMenu(NavBarGroup group) {
			if(this.groupStyleMenu == null) return;
			this.groupStyleMenu.Enabled = group != null;
			if(group == null) return;
			string curVal = Enum.GetName(typeof(NavBarGroupStyle), group.GroupStyle);
			if(this.groupStyleMenu.MenuItems.Count == 0) {
				foreach(string str in Enum.GetNames(typeof(NavBarGroupStyle))) {
					MenuItem item = this.groupStyleMenu.MenuItems.Add(str, new EventHandler(OnGroupStyleMenuClick));
					if(curVal == str) item.Checked = true;
				}
			} else {
				foreach(MenuItem item in this.groupStyleMenu.MenuItems) {
					item.Checked = (curVal == item.Text);
				}
			}
		}
		protected virtual ContextMenu CreateItemMenu() {
			ContextMenu menu = new ContextMenu();
			menu.MenuItems.Add(new MenuItem("&Add item", new EventHandler(OnItemMenuAddItem)));
			menu.MenuItems.Add(itemDelete = new MenuItem("&Delete item", new EventHandler(OnItemMenuDeleteItem)));
			return menu;
		}
		public NavBarControl NavBar { get { return Owner as NavBarControl; } }
		public override ISite Site { get { return NavBar == null ? null : NavBar.Site; } }
		public override void Dispose() {
			if(this.groupMenu != null) this.groupMenu.Dispose();
			if(this.itemMenu != null) this.itemMenu.Dispose();
			if(this.groupStyleMenu != null) this.groupStyleMenu.Dispose();
			base.Dispose();
		}
		public override void InvalidateComponent(object component) {
			NavBar.Invalidate(false);
		}
		protected override void OnDesignTimeSelectionChanged(object component) {
			NavBarGroup group = component as NavBarGroup;
			NavBarItem item = component as NavBarItem;
			if(group != null && group.NavBar == NavBar) InvalidateComponent(group);
			if(item != null && item.NavBar == NavBar) InvalidateComponent(item);
		}
		public virtual void OnRightClick(MouseEventArgs e) {
			NavBarHitInfo hitInfo = NavBar.CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.InGroupCaption) {
				this.menuObject = hitInfo.Group;
				SelectComponent(hitInfo.Group);
				this.groupAddItem.Enabled = hitInfo.Group.GroupStyle != NavBarGroupStyle.ControlContainer;
				this.groupDelete.Enabled = InheritanceHelper.AllowCollectionItemRemove(NavBar, TypeDescriptor.GetProperties(NavBar)["Groups"], NavBar.Groups, hitInfo.Group);
				GroupMenu.Show(NavBar, new Point(e.X, e.Y));
				return;
			}
			if(hitInfo.InLink) {
				SelectComponent(hitInfo.Link.Item);
				this.menuObject = hitInfo.Link;
				ItemMenu.Show(NavBar, new Point(e.X, e.Y));
				return;
			}
		}
		protected ContextMenu GroupMenu { get { return groupMenu; } }
		protected ContextMenu ItemMenu { get { return itemMenu; } }
		object menuObject = null;
		protected virtual void OnGroupStyleMenuPopup(object sender, EventArgs e) {
			UpdateGroupStyleMenu(this.menuObject as NavBarGroup);
		}
		protected virtual void OnGroupStyleMenuClick(object sender, EventArgs e) {
			NavBarGroup group = this.menuObject as NavBarGroup;
			if(group == null) return;
			string val = ((MenuItem)sender).Text;
			group.GroupStyle = (NavBarGroupStyle)Enum.Parse(typeof(NavBarGroupStyle), val);
		}
		protected void OnGroupMenuDeleteGroup(object sender, EventArgs e) {
			NavBarGroup group = this.menuObject as NavBarGroup;
			if(group != null) group.Dispose();
		}
		protected void OnGroupMenuAddGroup(object sender, EventArgs e) {
			NavBarGroup group = NavBar.Groups.Add();
			SelectComponent(group);
		}
		protected void OnGroupMenuExpandGroup(object sender, EventArgs e) {
			NavBarGroup group = this.menuObject as NavBarGroup;
			if(group != null) {
				group.Expanded = true;
				NavBar.FireChanged();
			}
		}
		protected void OnGroupMenuAddItem(object sender, EventArgs e) {
			NavBarGroup group = this.menuObject as NavBarGroup;
			if(group == null) return;
			AddItem(group);
			group.Expanded = true;
		}
		protected void OnGroupMenuAddSeparator(object sender, EventArgs e) {
			NavBarGroup group = this.menuObject as NavBarGroup;
			if(group == null) return;
			AddSeparator(group);
			group.Expanded = true;
		}
		protected void OnItemMenuAddItem(object sender, EventArgs e) {
			NavBarItemLink link = this.menuObject as NavBarItemLink;
			if(link != null) {
				link = AddItem(link.Group);
				if(link != null) {
					SelectComponent(link.Item);
					NavBar.ViewInfo.MakeLinkVisible(link);
				}
			}
		}
		protected NavBarItemLink AddItem(NavBarGroup group) {
			if(group == null) return null;
			NavBarItem item = NavBar.Items.Add();
			return group.ItemLinks.Add(item);
		}
		protected NavBarItemLink AddSeparator(NavBarGroup group) {
			if(group == null) return null;
			NavBarItem item = NavBar.Items.Add(true);
			return group.ItemLinks.Add(item);
		}
		protected void OnItemMenuDeleteItem(object sender, EventArgs e) {
			NavBarItemLink link = this.menuObject as NavBarItemLink;
			if(link != null) {
				int linkIndex = -1;
				NavBarGroup group = link.Group;
				if(group != null) linkIndex = link.Group.VisibleItemLinks.IndexOf(link);
				link.Item.Dispose();
				if(linkIndex != -1) {
					linkIndex = Math.Min(linkIndex, group.VisibleItemLinks.Count - 1);
					if(linkIndex != -1) SelectComponent(group.VisibleItemLinks[linkIndex].Item);
				}
			}
		}
	}
	public class LinkSizesCache {
		Dictionary<NavBarItemLink, Size> dict;
		public LinkSizesCache() {
			this.dict = new Dictionary<NavBarItemLink, Size>();
		}
		public virtual void Remove(NavBarItemLink link){
			if(ContainsKey(link)) dict.Remove(link);
		}
		public virtual bool ContainsKey(NavBarItemLink link) {
			return dict.ContainsKey(link);
		}
		public virtual void Add(NavBarItemLink link, Size size) {
			dict.Add(link, size);
		}
		public virtual void Clear() {
			dict.Clear();
		}
		public Dictionary<NavBarItemLink, Size>.KeyCollection Keys { get { return dict.Keys; } }
		public int Count { get { return dict.Count; } }
		public Size this[NavBarItemLink link] { get { return dict[link]; } }
	}
}
