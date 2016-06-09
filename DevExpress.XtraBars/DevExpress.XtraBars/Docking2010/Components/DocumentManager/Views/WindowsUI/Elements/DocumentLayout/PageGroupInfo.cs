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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IPageGroupInfo : 
		IDocumentSelectorInfo<PageGroup> {
	}
	class PageGroupInfo : DocumentSelectorInfo<PageGroup>, IPageGroupInfo {
		public PageGroupInfo(WindowsUIView view, PageGroup group)
			: base(view, group) {
		}
		protected override ObjectPainter GetButtonsPanelPainter() {
			return ((WindowsUIViewPainter)Owner.Painter).GetPageGroupButtonsPanelPainter();
		}
		public override Type GetUIElementKey() {
			return typeof(IPageGroupInfo);
		}
		protected override bool GetShowPageHeadersCore() {
			return Group.Properties.CanShowPageHeaders;
		}
		protected override IContentContainerHeaderInfo CreateHeaderInfo() {
			return new PageGroupHeaderInfo(Owner, this);
		}
		protected override System.Windows.Forms.Orientation GetButtonsPanelOrientation() {
			return System.Windows.Forms.Orientation.Horizontal;
		}
		protected override bool GetButtonsPanelWrap() {
			return false;
		}
		protected override bool GetIsButtonsPanelInverseOrder() {
			return true;
		}
		protected override ContentAlignment GetButtonsPanelContentAlignment() {
			return ContentAlignment.MiddleRight;
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			CalcButtonsPanel(g, ((PageGroupHeaderInfo)HeaderInfo).HeadersRect);
			base.CalcContent(g, content);
		}
		protected override Rectangle CalcDocumentSelectorButtonCore(Graphics g, Size buttonsPanelMinSize, Rectangle headersRect) {
			if(buttonsPanelMinSize.IsEmpty || headersRect.IsEmpty) return Rectangle.Empty;
			Size minSize = DocumentSelectorButton.Info.CalcMinSize(g, DocumentSelectorButton.Painter);
			if(headersRect.Width < minSize.Width)
				headersRect.Width = minSize.Width;
			Rectangle bounds = DevExpress.Utils.PlacementHelper.Arrange(new Size(minSize.Width, buttonsPanelMinSize.Height), headersRect, ContentAlignment.MiddleRight);
			return new Rectangle(new Point(bounds.Right, bounds.Top), bounds.Size); 
		}
		protected override Point CalcDocumentSelectorFlyoutPanelLocation(Size documentSelectorMenuSize){
			return new Point(DocumentSelectorButton.Info.Bounds.Right - documentSelectorMenuSize.Width, DocumentSelectorButton.Info.Bounds.Bottom);
		}
		public override bool DragMoveHitTest(Point hitPoint) {
			return base.DragMoveHitTest(hitPoint) && ButtonsPanel.ViewInfo.CalcHitInfo(hitPoint) == null;
		}
		class PageGroupHeaderInfo : DocumentSelectorHeaderInfo {
			public PageGroupHeaderInfo(WindowsUIView view, IPageGroupInfo containerInfo)
				: base(view, containerInfo) {
			}
			public Rectangle HeadersRect { get; private set; }
			Size headersSize;
			Rectangle headers;
			protected override Size CalcMinContentSize(GraphicsCache cache, Size textSize, Size buttonSize, int interval) {
				headersSize = ContainerInfo.ButtonsPanel.ViewInfo.CalcMinSize(cache.Graphics);
				int contentWidth = Math.Max(textSize.Width + interval + buttonSize.Width, TitleInfo.Bounds.Width);
				headers = Painter.GetItemsBoundsByContentRectangle(new Rectangle(Point.Empty, headersSize));
				if(!ContainerInfo.ShowPageHeaders)
					return base.CalcMinContentSize(cache, textSize, buttonSize, interval);
				return new Size(contentWidth,
					Math.Max(headersSize.Height, textSize.Height));
			}
			protected override void CalcElements(Point offset, Size textSize, Size buttonSize, Size contentSize, int textOffset) {
				base.CalcElements(offset, textSize, buttonSize, contentSize, textOffset);
				CalcPageButtons(offset, contentSize);
			}
			protected virtual void CalcPageButtons(Point offset, Size contentSize) {
				Rectangle bounds = ContainerInfo.Bounds;
				Rectangle header = ContainerInfo.Header;
				Rectangle emptyHeaderRect = Painter.GetItemsBoundsByContentRectangle(Rectangle.Empty);
				System.Windows.Forms.Padding padding = new System.Windows.Forms.Padding(
					Math.Abs(emptyHeaderRect.Left), Math.Abs(emptyHeaderRect.Top), emptyHeaderRect.Right, emptyHeaderRect.Bottom);
				if(contentSize.Width == emptyHeaderRect.Width)
					HeadersRect = new Rectangle(
						offset.X,
						header.Top + padding.Top,
						bounds.Width - emptyHeaderRect.Width,
						header.Height - padding.Vertical);
				else
					HeadersRect = new Rectangle(
						header.Right,
						header.Top + padding.Top,
						bounds.Width - header.Width - padding.Right,
						header.Height - padding.Vertical);
			}
			protected override bool CanShowCustomButtons() {
				return !ContainerInfo.ShowPageHeaders;
			}
		}
	}	
	class PageGroupInfoPainter : DocumentSelectorInfoPainter<PageGroup> { }
	class PageGroupInfoSkinPainter : DocumentSelectorInfoSkinPainter<PageGroup> {
		public PageGroupInfoSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override object GetButtonsIntervalProperty() {
			return GetSkin().Properties[MetroUISkins.PageGroupButtonsInterval];
		}
		protected override SkinElement GetDocumentSelectorElement() {
			return GetSkin()[MetroUISkins.SkinPageGroup];
		}
	}
	class PageGroupHeaderInfoPainter : ContentContainerHeaderInfoPainter { }
	class PageGroupHeaderInfoSkinPainter : ContentContainerHeaderInfoSkinPainter {
		public PageGroupHeaderInfoSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetItemHeader() {
			return GetSkin()[MetroUISkins.SkinPageGroupItemHeader];
		}
	}
}
