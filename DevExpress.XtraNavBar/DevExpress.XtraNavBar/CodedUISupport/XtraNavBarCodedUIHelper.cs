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

using System.Windows.Forms;
using System;
using DevExpress.Utils.CodedUISupport;
using DevExpress.XtraNavBar;
using System.Drawing;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraNavBar.CodedUISupport {
	public class XtraNavBarCodedUIHelper : IXtraNavBarCodedUIHelper {
		RemoteObject remoteObject;
		public XtraNavBarCodedUIHelper(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public NavBarControlElements GetNavBarElementFromPoint(IntPtr windowHandle, int pointX, int pointY, out string groupCaption, out string itemCaption) {
			itemCaption = groupCaption = null;
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null)
				return GetNavBarElementFromPoint(navBar, new Point(pointX, pointY), ref groupCaption, ref itemCaption);
			return NavBarControlElements.Unknown;
		}
		protected NavBarControlElements GetNavBarElementFromPoint(NavBarControl navBar, Point clientPoint, ref string groupCaption, ref string itemCaption) {
			NavBarHitInfo hitInfo = navBar.CalcHitInfo(clientPoint);
			if(hitInfo.Group != null)
				groupCaption = hitInfo.Group.Caption;
			if(hitInfo.Link != null)
				itemCaption = hitInfo.Link.Caption;
			switch(hitInfo.HitTest) {
				case NavBarHitTest.Link:
				case NavBarHitTest.LinkCaption:
				case NavBarHitTest.LinkImage:
					return NavBarControlElements.ItemLink;
				case NavBarHitTest.GroupBottom:
				case NavBarHitTest.GroupBottomButton:
				case NavBarHitTest.GroupCaption:
					return NavBarControlElements.Group;
				case NavBarHitTest.GroupCaptionButton:
					return NavBarControlElements.GroupButton;
				case NavBarHitTest.UpButton:
					return NavBarControlElements.UpButton;
				case NavBarHitTest.DownButton:
					return NavBarControlElements.DownButton;
				case NavBarHitTest.ExpandButton:
					return NavBarControlElements.ExpandButton;
				case NavBarHitTest.NavigationPaneOverflowPanelButton:
					NavBarNavigationPaneHitInfo navHit = hitInfo as NavBarNavigationPaneHitInfo;
					NavigationPaneOverflowPanelObjectInfo button = navHit.OverflowPanelButton;
					if(button != null) {
						NavBarGroup group = button.Group;
						if(group != null)
							groupCaption = group.Caption;
						else groupCaption = configureButtonName;
					}
					return NavBarControlElements.OverflowPanelButton;
				case NavBarHitTest.NavigationPaneSplitter:
					return NavBarControlElements.Splitter;
				default:
					return NavBarControlElements.Unknown;
			}
		}
		const string configureButtonName = "ConfigureButton";
		public string GetNavBarElementRectangleOrMakeElementVisible(IntPtr windowHandle, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null) {
				Rectangle rectangle = GetNavBarElementRectangleOrMakeElementVisible(navBar, elementType, groupCaption, itemCaption);
				if(rectangle != Rectangle.Empty)
					return CodedUIUtils.ConvertToString(rectangle);
			}
			return null;
		}
		protected Rectangle GetNavBarElementRectangleOrMakeElementVisible(NavBarControl navBar, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			switch(elementType) {
				case NavBarControlElements.ItemLink:
					NavBarItemLink link = GetLink(navBar, groupCaption, itemCaption);
					if(link != null) {
						if(!link.Group.Expanded || !IsLinkVisible(navBar, link))
							this.MakeLinkVisible(navBar, link);
						else {
							NavLinkInfoArgs linkInfo = navBar.ViewInfo.GetLinkInfo(link);
							if(linkInfo != null)
								return linkInfo.HitRectangle;
						}
					}
					return Rectangle.Empty;
				case NavBarControlElements.Group:
				case NavBarControlElements.GroupButton:
					NavBarGroup navGroup = GetGroup(navBar, groupCaption);
					if(navGroup != null) {
						if(!IsGroupVisible(navBar, navGroup))
							this.MakeGroupVisible(navBar, navGroup);
						else {
							NavGroupInfoArgs groupInfo = navBar.ViewInfo.GetGroupInfo(navGroup);
							if(groupInfo != null) {
								if(elementType == NavBarControlElements.GroupButton)
									return groupInfo.ButtonBounds;
								else return groupInfo.Bounds;
							}
						}
					}
					return Rectangle.Empty;
				case NavBarControlElements.UpButton:
					return navBar.ViewInfo.UpButtonBounds;
				case NavBarControlElements.DownButton:
					return navBar.ViewInfo.DownButtonBounds;
				case NavBarControlElements.ExpandButton:
				case NavBarControlElements.OverflowPanelButton:
				case NavBarControlElements.Splitter:
					NavigationPaneViewInfo navPaneViewInfo = navBar.ViewInfo as NavigationPaneViewInfo;
					if(navPaneViewInfo != null) {
						switch(elementType) {
							case NavBarControlElements.ExpandButton:
								return navPaneViewInfo.HeaderInfo.ButtonBounds;
							case NavBarControlElements.Splitter:
								return navPaneViewInfo.SplitterBounds;
							case NavBarControlElements.OverflowPanelButton:
								if(groupCaption == configureButtonName) 
								{
									NavigationPaneOverflowPanelObjectInfo buttonBounds = navPaneViewInfo.OverflowInfo.Buttons[navPaneViewInfo.OverflowInfo.Buttons.Count - 1] as NavigationPaneOverflowPanelObjectInfo;
									return buttonBounds.Bounds;
								}
								else {
									foreach(NavigationPaneOverflowPanelObjectInfo info in navPaneViewInfo.OverflowInfo.Buttons) {
										if(info.Group.Caption == groupCaption) {
											return info.Bounds;
										}
									}
									break;
								}
						}
					}
					break;
			}
			return Rectangle.Empty;
		}
		public void MakeNavBarElementVisible(IntPtr windowHandle, NavBarControlElements elementType, string groupCaption, string itemCaption) {
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null) {
				switch(elementType) {
					case NavBarControlElements.ItemLink:
						NavBarItemLink link = GetLink(navBar, groupCaption, itemCaption);
						if(link != null)
							this.MakeLinkVisible(navBar, link);
						break;
					case NavBarControlElements.Group:
					case NavBarControlElements.GroupButton:
						NavBarGroup group = GetGroup(navBar, groupCaption);
						if(group != null)
							this.MakeGroupVisible(navBar, group);
						break;
				}
			}
		}
		public string[] GetGroupCaptionsInOverflowPanel(IntPtr windowHandle) {
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null) {
				if(navBar.PaintStyleKind != NavBarViewKind.NavigationPane) return null;
				NavigationPaneViewInfo vi = (NavigationPaneViewInfo)navBar.ViewInfo;
				string[] captions = new string[vi.OverflowPanelGroups.Count];
				int i = 0;
				foreach(NavBarGroup nbg in vi.OverflowPanelGroups) {
					captions[i] = nbg.Caption;
					i++;
				}
				return captions;
			}
			return null;
		}
		public string[] GetGroupsCaptions(IntPtr windowHandle) {
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null) {
				string[] captions = new string[navBar.Groups.Count];
				for(int i = 0; i < captions.Length; i++)
					captions[i] = navBar.Groups[i].Caption;
				return captions;
			}
			return null;
		}
		public string[] GetLinksCaptions(IntPtr windowHandle, string groupCaption) {
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null) {
				foreach(NavBarGroup group in navBar.Groups)
					if(group.Caption == groupCaption) {
						string[] captions = new string[group.ItemLinks.Count];
						for(int i = 0; i < captions.Length; i++)
							captions[i] = group.ItemLinks[i].Caption;
						return captions;
					}
			}
			return null;
		}
		protected void MakeLinkVisible(NavBarControl navBar, NavBarItemLink link) {
			navBar.BeginInvoke(new MethodInvoker(delegate {
				navBar.ViewInfo.MakeLinkVisible(link);
			}));
		}
		protected bool IsLinkVisible(NavBarControl navBar, NavBarItemLink link) {
			NavLinkInfoArgs linkInfo = navBar.ViewInfo.GetLinkInfo(link);
			if(linkInfo != null)
				return navBar.ClientRectangle.IntersectsWith(linkInfo.HitRectangle);
			return false;
		}
		protected void MakeGroupVisible(NavBarControl navBar, NavBarGroup group) {
			navBar.BeginInvoke(new MethodInvoker(delegate {
				NavGroupInfoArgs info = navBar.ViewInfo.GetGroupInfo(group);
				if(info == null) return;
				Rectangle r = info.Bounds;
				if(!info.ImageBounds.IsEmpty) r.Y = info.ImageBounds.Y;
				if(!info.ClientInfo.Bounds.IsEmpty)
					r.Height = info.ClientInfo.Bounds.Bottom - r.Top;
				else
					r.Height = info.Bounds.Bottom - r.Top;
				int newTopDelta = 0;
				if(r.Bottom > navBar.ViewInfo.Client.Bottom) {
					newTopDelta = (r.Bottom - navBar.ViewInfo.Client.Bottom);
				}
				if((r.Y - newTopDelta) < navBar.ViewInfo.Client.Y) {
					newTopDelta -= (navBar.ViewInfo.Client.Y - (r.Y - newTopDelta));
				}
				navBar.ViewInfo.TopY += newTopDelta;
			}));
		}
		protected bool IsGroupVisible(NavBarControl navBar, NavBarGroup group) {
			NavGroupInfoArgs groupInfo = navBar.ViewInfo.GetGroupInfo(group);
			if(groupInfo != null)
				return navBar.ClientRectangle.IntersectsWith(groupInfo.Bounds);
			return false;
		}
		protected NavBarItemLink GetLink(NavBarControl navBar, string groupCaption, string itemCaption) {
			foreach(NavBarGroup group in navBar.Groups)
				if(groupCaption == group.Caption)
					foreach(NavBarItemLink link in group.VisibleItemLinks)
						if(link.Item.Caption == itemCaption)
							return link;
			return null;
		}
		protected NavBarGroup GetGroup(NavBarControl navBar, string groupCaption) {
			foreach(NavBarGroup group in navBar.Groups)
				if(groupCaption == group.Caption)
					return group;
			return null;
		}
		public string GetNavBarGroupExpandedState(IntPtr windowHandle, string groupCaption) {
			NavBarControl navBar = Control.FromHandle(windowHandle) as NavBarControl;
			if(navBar != null) {
				foreach(NavBarGroup group in navBar.Groups)
					if(group.Caption == groupCaption) {
						return group.Expanded.ToString();
					}
			}
			return null;
		}
	}
}
