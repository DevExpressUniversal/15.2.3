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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraNavBar;
using System.Collections.Generic;
using DevExpress.Utils.Text;
namespace DevExpress.XtraNavBar.ViewInfo {
	public class BaseNavGroupClientPainter : ObjectPainter {
		BaseNavGroupPainter groupPainter;
		public BaseNavGroupClientPainter(BaseNavGroupPainter groupPainter) {
			this.groupPainter = groupPainter;
		} 
		protected BaseNavGroupPainter GroupPainter { get { return groupPainter; } }
		public virtual NavBarControl NavBar { get { return GroupPainter.NavBar; } }
		public NavBarGroup GetGroup(ObjectInfoArgs e) {
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			return clientInfo.Group;
		}
		public virtual Size CalcImageSize(ObjectInfoArgs e, NavBarGroup group) {
			if(!group.GetShowIcons())
				return Size.Empty;
			Size size = new Size(8, 8);
			foreach(NavBarItemLink link in group.ItemLinks) {
				Size imgSize = link.GetImageSize();
				size.Width = Math.Max(size.Width, imgSize.Width);
				size.Height = Math.Max(size.Height, imgSize.Height);
			}
			return size;
		}
		protected internal virtual int CalcGroupClientHeightByClientSize(Size clientSize) {
			return clientSize.Height;
		}
		protected virtual int CalcClientIndent(NavGroupClientInfoArgs e, IndentType indent) {
			if(e.Group.GroupStyle == NavBarGroupStyle.ControlContainer) return 0;
			if(indent == IndentType.Top) return 6;
			return 0;
		}
		protected virtual void CalcClientInnerBounds(NavGroupClientInfoArgs e) {
			Rectangle r = e.ClientBounds;
			r.X += CalcClientIndent(e, IndentType.Left);
			r.Width -= (CalcClientIndent(e, IndentType.Left) + CalcClientIndent(e, IndentType.Right));
			r.Y += CalcClientIndent(e, IndentType.Top);
			r.Height -= (CalcClientIndent(e, IndentType.Top) + CalcClientIndent(e, IndentType.Bottom));
			r.Height = Math.Max(0, r.Height);
			r.Width = Math.Max(0, r.Width);
			e.ClientInnerBounds = r;
		}
		protected virtual void CalcClientBounds(NavGroupClientInfoArgs e) {
			e.ClientBounds = e.Bounds;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			clientInfo.Reset();
			CalcClientBounds(clientInfo);
			CalcClientInnerBounds(clientInfo);
			Rectangle linkRect = clientInfo.ClientInnerBounds;
			int topVisibleIndex = clientInfo.InForm? 0: Math.Max(0, Math.Min(NavBar.ViewInfo.GetGroupTopVisibleLinkIndex(clientInfo.Group), clientInfo.Group.VisibleItemLinks.Count - 1));
			int visibleCount = clientInfo.Group.VisibleItemLinks.Count;
			int rowLinkCount = 0;
			linkRect.Y -= NavBar.ViewInfo.ClientLinkOffsetY;
			int maxBottom = linkRect.Y;
			for(int n = topVisibleIndex; n < visibleCount; n++) {
				rowLinkCount ++;
				NavBarItemLink link = clientInfo.Group.VisibleItemLinks[n];
				NavLinkInfoArgs li = new NavLinkInfoArgs(null, link, linkRect, ObjectState.Normal);
				li.Graphics = e.Graphics;
				li.Bounds = linkRect;
				clientInfo.Group.NavBar.LinkPainter.CalcObjectBounds(li);
				li.Graphics = null;
				if(clientInfo.Group.GetShowAsIconsView()) {
					li.Bounds = li.ImageRectangle;
					if(li.ImageRectangle.Right > clientInfo.ClientInnerBounds.Right) {
						if(rowLinkCount > 1) 
							n --;
						else 
							clientInfo.Links.Add(li);
						linkRect.X = clientInfo.ClientInnerBounds.Left;
						linkRect.Y = li.Bounds.Bottom;
						maxBottom = li.Bounds.Bottom;
						rowLinkCount = 0;
						continue;
					}
					else {
						linkRect.X = li.ImageRectangle.Right;
					}
					clientInfo.Links.Add(li);
					maxBottom = li.Bounds.Bottom;
				}
				else {
					clientInfo.Links.Add(li);
					linkRect.Y = li.Bounds.Bottom + CalcLinkInterval(false);
					maxBottom = linkRect.Y;
				}
			}
			maxBottom += CalcClientIndent(clientInfo, IndentType.Bottom);
			maxBottom = clientInfo.ClientInnerBounds.Y + CalcGroupClientHeightByClientSize(new Size(0, maxBottom - clientInfo.ClientInnerBounds.Y));
			return new Rectangle(0, 0, 0, maxBottom);
		}
		protected virtual int CalcLinkInterval(bool showAsIconsView) {
			if(showAsIconsView) return 0;
			return Math.Max(NavBar.LinkInterval, 0);
		}
		protected bool IsControlContainer(ObjectInfoArgs e) {
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			return clientInfo.Group.GroupStyle == NavBarGroupStyle.ControlContainer;
		}
		protected virtual void DrawBorder(NavGroupClientInfoArgs e) {
		}
		protected virtual void DrawBackground(NavGroupClientInfoArgs e) {
			CustomDrawObjectEventArgs custom = new CustomDrawObjectEventArgs(e, e.ClientBounds, GroupPainter.GetGroupAppearance(e));
			RaiseDrawGroupClientBackground(custom);
			if(custom.Handled) return;
			if(!e.InForm || IsControlContainer(e)) DrawBackgroundCore(e, custom);
			RaiseDrawGroupClientForeground(custom);
		}
		bool allowPartitallyVisibleLinks = true;
		public bool AllowPartitallyVisibleLinks { get { return allowPartitallyVisibleLinks; } set { allowPartitallyVisibleLinks = value; } }
		protected virtual void DrawBackgroundCore(NavGroupClientInfoArgs e, CustomDrawObjectEventArgs custom) {
			if(e.Group.TextureBackgroundBrush != null) {
				if(custom.Appearance.GetBackColor().A != 255)
					e.Graphics.FillRectangle(e.Group.TextureBackgroundBrush, e.ClientBounds);
			}
			custom.Appearance.DrawBackground(e.Cache, e.ClientBounds, true);
		}
		protected virtual void DrawLinks(NavGroupClientInfoArgs e) {
			foreach(NavLinkInfoArgs li in e.Links) {
				li.Cache = e.Cache;
				try {
					if(!AllowPartitallyVisibleLinks && li.Bounds.Bottom > e.ClientInnerBounds.Bottom) break; 
					e.Group.NavBar.LinkPainter.DrawObject(li);
					if(e.Group.NavBar.ViewInfo.DropTarget.IsEquals(li.Link)) {
						DrawLinkDropMark(e, li);
					}
				}
				finally {
					li.Cache = null;
				}
			}
			if(e.Links.Count == 0) {
				if(e.Group.NavBar.ViewInfo.DropTarget.Group == e.Group) {
					DrawLinkDropMark(e, null);
				}
			}
		}
		protected virtual AppearanceObject GetCollapsedAppearance(AppearanceObject src) {
			NavigationPaneViewInfo vi = NavBar.ViewInfo as NavigationPaneViewInfo;
			AppearanceObject obj = new AppearanceObject();
			AppearanceHelper.Combine(obj, src, vi.HeaderInfo.Appearance); 
			obj.Combine(src);
			return obj;
		}
		protected virtual AppearanceObject GetCollapsedCaptionAppearance() {
			if(NavBar.ViewInfo.PressedInfo.HitTest == NavBarHitTest.ContentButton) return GetCollapsedAppearance(NavBar.Appearance.NavPaneContentButtonPressed);
			else if(NavBar.NavPaneForm != null && NavBar.NavPaneForm.Visible) return GetCollapsedAppearance(NavBar.Appearance.NavPaneContentButtonReleased);
			else if(NavBar.ViewInfo.HotInfo.HitTest == NavBarHitTest.ContentButton) return GetCollapsedAppearance(NavBar.Appearance.NavPaneContentButtonHotTracked);
			return GetCollapsedAppearance(NavBar.Appearance.NavPaneContentButton);
		}
		protected virtual void DrawCollapsedClient(ObjectInfoArgs e) {
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			AppearanceObject app = GetCollapsedCaptionAppearance();
			e.Graphics.FillRectangle(app.GetBackBrush(e.Cache), e.Bounds);
		}
		public virtual void DrawCollapsedObject(ObjectInfoArgs e) {
			DrawCollapsedClient(e);
			if(NavBar.ViewInfo.AllowDrawCaption) DrawCollapsedGroupCaption(e);
		}
		protected void DrawCollapsedGroupCaption(ObjectInfoArgs e) {
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			NavBarGroup group = clientInfo.Group;
			AppearanceObject appearance = GetCollapsedCaptionAppearance();
			using(StringFormat str = appearance.TextOptions.GetStringFormat().Clone() as StringFormat) {
				str.Alignment = StringAlignment.Center;
				string groupCaption = group.GetAllowHtmlString() ? StringPainter.Default.RemoveFormat(group.Caption, true) : group.Caption;
				appearance.DrawVString(e.Cache, groupCaption, appearance.Font, appearance.GetForeBrush(e.Cache), clientInfo.Bounds, str, 270);
			}
		}
		protected virtual bool ShouldDrawCollapsedObject(NavGroupClientInfoArgs e) {
			if(!(e.NavBar.ViewInfo is NavigationPaneViewInfo)) return false;
			return e.NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			if(!clientInfo.CanDraw) return;
			if(ShouldDrawCollapsedObject(clientInfo) && !clientInfo.InForm) {
				DrawCollapsedObject(e);
				return;
			}
			GraphicsClipState clipState = null;
			GraphicsInfoState state = null;
			if(!IsControlContainer(clientInfo) && !clientInfo.InForm) {
				state = e.Cache.ClipInfo.SaveStateAPI();
				clipState = e.Cache.ClipInfo.SaveAndSetClip(clientInfo.Bounds);
			}
			DrawBorder(clientInfo);
			DrawBackground(clientInfo);
			if(!IsControlContainer(clientInfo)) {
				if(!clientInfo.InForm) e.Cache.ClipInfo.SetClip(clientInfo.ClientInnerBounds);
				DrawLinks(clientInfo);
			}
			if(state != null) {
				e.Cache.ClipInfo.RestoreClipRelease(clipState);
				e.Cache.ClipInfo.RestoreState(state);
			}
		}
		const int LinkDropVertIndent = 4;
		protected virtual void DrawLinkDropMark(NavGroupClientInfoArgs e, NavLinkInfoArgs li) {
			if(!NavBar.AllowDrawLinkDropMark)
				return;
			if(e.Group.GetShowAsIconsView()) {
				DrawIconsLinkDropMark(e, li);
				return;
			}
			int index = e.Links.IndexOf(li);
			bool insertAfter = e.Group.NavBar.ViewInfo.DropTarget.InsertAfter;
			AppearanceObject appearance = e.Group.NavBar.PaintAppearance.LinkDropTarget;
			Rectangle destRect = e.ClientBounds;
			destRect.Inflate(-1, 0);
			destRect.Height = 2;
			destRect.Y += LinkDropVertIndent;
			if(li != null) {
				if(insertAfter)
					destRect.Y = li.Bounds.Bottom - LinkDropVertIndent;
				else
					destRect.Y = li.Bounds.Top - 2;
			}
			Brush brush = appearance.GetBackBrush(e.Cache);
			e.Graphics.FillRectangle(brush, new Rectangle(destRect.X, destRect.Top + LinkDropVertIndent, destRect.Width, 1));
			int topIndent = 0;
			if(index == NavBar.ViewInfo.GetGroupTopVisibleLinkIndex(e.Group) && !insertAfter) topIndent = LinkDropVertIndent;
			int bottomIndent = 0;
			if(insertAfter) {
				if(index == -1 || index == e.Group.VisibleItemLinks.Count - 1) {
					bottomIndent = -LinkDropVertIndent;
					topIndent = 0;
				}
			}
			e.Graphics.FillPolygon(brush, 
				new Point[] { 
								new Point(destRect.X, destRect.Y + topIndent),
								new Point(destRect.X + 5, destRect.Y + 4),
								new Point(destRect.X, destRect.Y + 8 + bottomIndent)});
			e.Graphics.FillPolygon(brush, 
				new Point[] { 
								new Point(destRect.Right, destRect.Y + topIndent),
								new Point(destRect.Right - 6, destRect.Y + 4),
								new Point(destRect.Right, destRect.Y + 8 + bottomIndent)});
		}
		protected virtual void DrawIconsLinkDropMark(NavGroupClientInfoArgs e, NavLinkInfoArgs li) {
			int index = e.Links.IndexOf(li);
			bool insertAfter = e.Group.NavBar.ViewInfo.DropTarget.InsertAfter;
			AppearanceObject appearance = e.Group.NavBar.PaintAppearance.LinkDropTarget;
			Rectangle destRect = e.ClientBounds;
			destRect.Height = e.Group.GetImageSize().Height;
			destRect.Y += 4;
			destRect.X += 4;
			destRect.Width = 2;
			if(li != null) {
				if(insertAfter)
					destRect.X = li.Bounds.Right - 2;
				else
					destRect.X = li.Bounds.Left;
				destRect.Y = li.Bounds.Y;
				destRect.Height = li.Bounds.Height;
			}
			e.Graphics.FillRectangle(appearance.GetBackBrush(e.Cache), destRect);
		}
		protected virtual void RaiseDrawGroupClientBackground(CustomDrawObjectEventArgs e) {
			NavBar.RaiseCustomDrawGroupClientBackground(e);
		}
		protected virtual void RaiseDrawGroupClientForeground(CustomDrawObjectEventArgs e) {
			NavBar.RaiseCustomDrawGroupClientForeground(e);
		}
	}
	public enum IndentType { Left, Top, Right, Bottom };
	public class BaseNavGroupPainter : ObjectPainter {
		ObjectPainter buttonPainter;
		NavBarControl navBar;
		BaseNavGroupClientPainter clientPainter;
		public BaseNavGroupPainter(NavBarControl navBar) {
			this.navBar = navBar;
			this.buttonPainter = CreateButtonPainter();
			this.clientPainter = CreateGroupClientPainter();
		}
		public virtual NavBarControl NavBar { get { return navBar; } }
		public virtual ObjectPainter UpDownButtonPainter { get { return ButtonPainter; } }
		public BaseNavGroupClientPainter ClientPainter { get { return clientPainter; } }
		protected virtual ObjectPainter CreateButtonPainter() { return new ButtonObjectPainter(); }
		protected virtual BaseNavGroupClientPainter CreateGroupClientPainter() { return new BaseNavGroupClientPainter(this); }
		public NavBarGroup GetGroup(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			if(groupInfo != null) return groupInfo.Group;
			NavGroupClientInfoArgs clientInfo = e as NavGroupClientInfoArgs;
			if(clientInfo != null) return clientInfo.Group;
			return null;
		}
		public virtual int CalcMinGroupClientHeight(ObjectInfoArgs e) {	return 20;  }
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			Rectangle rect = e.Bounds;
			NavBarGroup group = GetGroup(e);
			Size size = CalcTextSize(e, e.Bounds.Width);
			size = ButtonPainter.CalcBoundsByClientRectangle(new StyleObjectInfoArgs(e.Cache, new Rectangle(0, 0, 10, size.Height), GetGroupAppearance(e), ObjectState.Normal)).Size;
			return new Rectangle(Point.Empty, size);
		}
		public virtual void CalcFooterBounds(NavGroupInfoArgs groupInfo, Rectangle bounds, Size buttonSize) {
			groupInfo.FooterBounds = Rectangle.Empty;
		}
		public virtual int CalcFooterHeight(NavGroupInfoArgs groupInfo, out Size buttonSize) {
			buttonSize = Size.Empty;
			return 0;
		}
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			Rectangle captionBounds = CalcClientBounds(e, groupInfo);
			e.Bounds = CheckBounds(e.Bounds);
			groupInfo.CaptionBounds = CheckBounds(captionBounds);
			return e.Bounds;
		}
		protected virtual Rectangle CalcClientBounds(ObjectInfoArgs e, NavGroupInfoArgs groupInfo) {
			return GetClientBounds(e);
		}
		protected virtual Rectangle GetClientBounds(ObjectInfoArgs e) {
			return ButtonPainter.GetObjectClientRectangle(GetButtonArgs(e, e.Bounds));			
		}
		protected ObjectInfoArgs GetButtonArgs(ObjectInfoArgs e, Rectangle bounds) {
			return GetButtonArgs(e, bounds, ObjectState.Normal);
		}
		protected ObjectInfoArgs GetButtonArgs(ObjectInfoArgs e, Rectangle bounds, ObjectState state) {
			return GetButtonArgs(e, bounds, GetGroupAppearance(e), state);
		}
		protected ObjectInfoArgs GetButtonArgs(ObjectInfoArgs e, Rectangle bounds, AppearanceObject style, ObjectState state) {
			return new StyleObjectInfoArgs(e.Cache, bounds, style, state);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			groupInfo.ClientInfo.NavGroupInfo = groupInfo;
			NavBarGroup group = GetGroup(e);
			if(group.Expanded || groupInfo.ClientInfo.CanDraw) ClientPainter.DrawObject(groupInfo.ClientInfo);
			if(e.Bounds.Height > 0 && e.Bounds.Width > 0) DrawGroupCaption(groupInfo);
			DrawSelection(groupInfo);
		}
		public virtual void DrawSelection(NavGroupInfoArgs e) {
			if(!NavBar.DesignManager.IsComponentSelected(e.Group)) return;
			NavBar.DesignManager.DrawSelection(e.Cache, e.Bounds, GetGroupAppearance(e).BackColor);
		}
		public virtual ObjectPainter ButtonPainter { get { return buttonPainter; } }
		protected CustomDrawNavBarElementEventArgs CreateDrawArgs(ObjectInfoArgs e) {
			CustomDrawNavBarElementEventArgs args = new CustomDrawNavBarElementEventArgs(e, e.Bounds, GetGroupAppearance(e));
			NavBarGroup group = GetGroup(e);
			args.Caption = group.Caption;
			args.Image = group.GetImage();
			return args;
		}
		protected virtual void DrawGroupCaptionCore(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			NavBarGroup group = GetGroup(e);
			ButtonPainter.DrawObject(GetButtonArgs(e, e.Bounds, custom.Appearance, e.State));
			if (e.Group.GetAllowHtmlString()) {
				StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionBounds.Location);
				StringPainter.Default.DrawString(e.Cache, e.StringInfo);
			} else
				custom.Appearance.DrawString(e.Cache, custom.Caption, e.CaptionBounds);
		}
		protected virtual void DrawGroupCaption(NavGroupInfoArgs e) {
			CustomDrawNavBarElementEventArgs custom = CreateDrawArgs(e);
			RaiseDrawGroupCaption(custom);
			if(custom.Handled) return;
			if(!string.Equals(e.Group.Caption, custom.Caption)) {
				UpdateCaptionBounds(e, custom.Caption);
			}
			DrawGroupCaptionCore(e, custom);
		}
		protected internal void UpdateCaptionBounds(NavGroupInfoArgs groupInfo, string customCaption) {
			Rectangle rect = groupInfo.CaptionBounds;
			rect.Width = CalcBestTextSize(groupInfo, 0, groupInfo.Group, customCaption).Width;
			if(groupInfo.CaptionMaxWidth != 0) rect.Width = Math.Min(groupInfo.CaptionMaxWidth, rect.Width);
			if(NavBar.IsRightToLeft) rect.X += groupInfo.CaptionBounds.Width - rect.Width;
			groupInfo.CaptionBounds = rect;
		}
		protected virtual void RaiseDrawGroupCaption(CustomDrawNavBarElementEventArgs e) {
			NavBar.RaiseCustomDrawGroupCaption(e);
		}
		public AppearanceObject GetGroupAppearance(ObjectInfoArgs e) {
			NavGroupInfoArgs groupInfo = e as NavGroupInfoArgs;
			if(groupInfo == null) {
				NavBarGroup group = GetGroup(e);
				AppearanceObject appearance = new AppearanceObject();
				AppearanceHelper.Combine(appearance, new AppearanceObject[] { group.AppearanceBackground, NavBar.PaintAppearance.GroupBackground });
				return appearance;
			}
			return groupInfo.PaintAppearance;
		}
		protected virtual Size CalcTextSize(ObjectInfoArgs e, int width) {
			NavBarGroup group = GetGroup(e);
			string text = group.Caption == "" ? "Wg" : group.Caption;
			Size size = CalcBestTextSize(e, width, group, text);
			size.Height ++;
			return size;
		}
		private Size CalcTextSizeCore(ObjectInfoArgs e, int width, NavGroupInfoArgs args, NavBarGroup group, string text, AppearanceObject obj) {
			if(group.GetAllowHtmlString()) {
				CalcHtmlStringInfo(e, group.Caption, width, obj);
				return args.StringInfo.Bounds.Size;
			}
			return obj.CalcTextSize(e.Graphics, text, width).ToSize();
		}
		static ObjectState[] objectStateValues = (ObjectState[])Enum.GetValues(typeof(ObjectState));
		protected virtual Size CalcBestTextSize(ObjectInfoArgs e, int width, NavBarGroup group, string text) {
			int bestWidth = 0, bestHeight = 0;
			NavGroupInfoArgs args = e as NavGroupInfoArgs;
			foreach(ObjectState state in objectStateValues) {
				AppearanceObject obj = GetGroupAppearanceByState(group, state);
				if(!obj.Options.UseFont && state != ObjectState.Normal)
					continue;
				Size size = CalcTextSizeCore(e, width, args, group, text, obj); 
				bestWidth = Math.Max(bestWidth, size.Width);
				bestHeight = Math.Max(bestHeight, size.Height);
			}
			return new Size(bestWidth, bestHeight);
		}
		public virtual AppearanceObject GetGroupAppearanceByState(NavBarGroup group, ObjectState state) {
			return group.GetHeaderAppearanceByState(state);
		}
		public virtual void CalcHtmlStringInfo(ObjectInfoArgs e, string caption, int width) {
			CalcHtmlStringInfo(e, caption, width, null);
		}
		public virtual void CalcHtmlStringInfo(ObjectInfoArgs e, string caption, int width, AppearanceObject appearance) {
			NavGroupInfoArgs args = e as NavGroupInfoArgs;
			if(args.StringInfo != null && args.StringInfo.SourceString == caption && args.StringInfo.IsEqualsAppearance(appearance))
				return;
			NavBarGroup group = GetGroup(e);
			StringInfo info = StringPainter.Default.Calculate(e.Graphics, appearance == null ? group.Appearance : appearance, null, caption, width, null, group.NavBar.ViewInfo.GetHtmlDrawContext());
			args.StringInfo = info;
		}
		protected virtual Rectangle CalcGroupCaptionImageBounds(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			Rectangle res = Rectangle.Empty;
			Size imgSize = e.Group.GetImageSize();
			if(custom != null && custom.Image != null) imgSize = custom.Image.Size;
			if(imgSize.Height > 0) {
				res = e.CaptionClientBounds;
				res.Size = imgSize;
				int imageHeight = e.Group.GetPrefferedImageSize(imgSize).Height;
				if(res.Size.Height < e.CaptionClientBounds.Height) {
					res.Y += (e.CaptionClientBounds.Height - imageHeight) / 2;
					res.X ++;
				} else
					res.Y = e.CaptionClientBounds.Bottom - imageHeight;
			}
			return res;
		}
		protected virtual void DrawGroupImage(NavGroupInfoArgs e, CustomDrawNavBarElementEventArgs custom) {
			Rectangle r = e.ImageBounds;
			if(r.IsEmpty) return;
			Image image = custom.Image;
			if(image != null && image.Size.Height > 0) {
				Rectangle imageRect = new Rectangle(r.Location, e.Group.GetPrefferedImageSize(r.Size));
				if(e.Group.GetAllowGlyphSkinning())
					e.Cache.Paint.DrawImage(e.Graphics, image, imageRect, new Rectangle(Point.Empty, r.Size), ImageColorizer.GetColoredAttributes(custom.Appearance.ForeColor));
				else
					e.Cache.Paint.DrawImage(e.Graphics, image, imageRect, new Rectangle(Point.Empty, r.Size), true);
			}
		}
		protected Rectangle CheckBounds(Rectangle bounds) {
			if(IsRightToLeft) {
				int x = CalcGroupX(bounds);
				return new Rectangle(x, bounds.Y, bounds.Width, bounds.Height);
			}
			return bounds;
		}
		protected virtual int CalcGroupX(Rectangle bounds) {
			return NavBar.Width - bounds.Right;
		}
		protected bool IsRightToLeft {
			get {
				if(NavBar == null) return false;
				return NavBar.IsRightToLeft;
			}
		}
	}
}
