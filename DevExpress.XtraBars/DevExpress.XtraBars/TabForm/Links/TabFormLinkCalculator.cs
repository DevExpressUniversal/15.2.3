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

using DevExpress.XtraBars.ViewInfo;
using System;
using System.Drawing;
namespace DevExpress.XtraBars {
	public class TabFormLinkCalculator {
		TabFormControlViewInfoBase viewInfo;
		public TabFormLinkCalculator(TabFormControlViewInfoBase viewInfo) {
			this.viewInfo = viewInfo;
		}
		public TabFormControlViewInfoBase ViewInfo { get { return viewInfo; } }
		public void Calculate(TabFormLinkInfoCollection links) {
			Rectangle rect = CalcAvailableBounds(links);
			Point itemLocation = rect.Location;
			int right = rect.X;
			int bottom = rect.Y;
			int top = rect.Bottom;
			for(int i = 0; i < links.Count; i++) {
				BarEditLinkViewInfo editLink = links[i] as BarEditLinkViewInfo;
				if(editLink != null && ViewInfo.IsRightToLeft)
					editLink.LockUpdateActiveEditor();
				itemLocation = CalcLinkViewInfo(links[i], rect, itemLocation, bottom);
				if(editLink != null && ViewInfo.IsRightToLeft)
					editLink.UnlockUpdateActiveEditor();
				if(links[i].Bounds == Rectangle.Empty)
					continue;
				top = Math.Min(links[i].Bounds.Top, top);
				right = Math.Max(links[i].Bounds.Right, right);
				bottom = Math.Max(links[i].Bounds.Bottom, bottom);
			}
			links.Bounds = new Rectangle(rect.X, top, right - rect.X, bottom - top);
		}
		Rectangle CalcAvailableBounds(TabFormLinkInfoCollection links) {
			Rectangle rect = ViewInfo.ClientRect;
			if(links.LayoutType == TabFormLinksLayoutType.Top) {
				rect.X += GetTopPanelLeft();
				rect.Width = GetTopPanelRight() - GetTopPanelLeft();
				rect.Height = ViewInfo.GetTopPanelHeight();
			}
			if(links.LayoutType == TabFormLinksLayoutType.Left || links.LayoutType == TabFormLinksLayoutType.Right) {
				rect.Y += ViewInfo.GetTopPanelHeight();
				rect.Width = (rect.Width - ViewInfo.CalcAddPageWidth()) / 2;
				if(links.LayoutType == TabFormLinksLayoutType.Right)
					rect.X += rect.Width + ViewInfo.CalcAddPageWidth();
				rect.Height = ViewInfo.CalcBestPageHeight();
			}
			int bestWidth = CalcCollectionBestWidth(links, rect.Width);
			if(links.LayoutType == TabFormLinksLayoutType.Right) rect.X = rect.Right - bestWidth;
			rect.Width = bestWidth;
			return rect;
		}
		internal int GetTopPanelLeft() {
			TabFormControl captionControl = ViewInfo.Owner as TabFormControl;
			if(captionControl != null) {
				if(captionControl.ShowTabsInTitleBar == ShowTabsInTitleBar.True) return 0;
				TabFormPainter painter = captionControl.GetFormPainter();
				if(painter != null) {
					return painter.GetTextRight(ViewInfo.GInfo.Graphics) - painter.Margins.Left;
				}
			}
			return 0;
		}
		internal int GetTopPanelRight() {
			TabFormControl captionControl = ViewInfo.Owner as TabFormControl;
			if(captionControl != null) {
				TabFormPainter painter = captionControl.GetFormPainter();
				if(painter != null) {
					return painter.GetButtonsLeft() - painter.Margins.Left;
				}
			}
			return ViewInfo.Bounds.Width;
		}
		Point CalcLinkViewInfo(BarLinkViewInfo linkInfo, Rectangle bounds, Point loc, int maxBottom) {
			Size linkSize = linkInfo.CalcLinkSize(ViewInfo.GInfo.Graphics, null);
			bool isFit = loc.X + linkSize.Width <= bounds.Right;
			if(!isFit) {
				linkInfo.Bounds = Rectangle.Empty;
				return loc;
			}
			Point linkLocation = new Point(loc.X, loc.Y + (bounds.Height - linkSize.Height) / 2);
			Rectangle linkRect = new Rectangle(linkLocation, linkSize);
			linkInfo.CalcViewInfo(ViewInfo.GInfo.Graphics, ViewInfo.Owner, linkRect);
			return new Point(linkInfo.Bounds.Right, loc.Y);
		}
		public int CalcCollectionBestWidth(TabFormLinkInfoCollection links, int maxWidth) {
			int bestWidth = 0;
			for(int i = 0; i < links.Count; i++) {
				Size linkSize = links[i].CalcLinkSize(ViewInfo.GInfo.Graphics, null);
				if(bestWidth + linkSize.Width > maxWidth)
					return bestWidth;
				bestWidth += linkSize.Width;
			}
			return bestWidth;
		}
	}
}
