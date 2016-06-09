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
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Navigation {
	public class NavigationPaneViewInfo : ObjectInfoArgs {
		INavigationPane ownerCore;
		public Rectangle ButtonsBounds { get; set; }
		public Rectangle PageBounds { get; set; }
		public Rectangle ClientBounds { get; set; }
		public Rectangle ResizeBounds { get; set; }
		public Rectangle BackgroundBounds { get; set; }
		public NavigationPaneViewInfo(INavigationPane owner) {
			ownerCore = owner;
		}
		protected internal INavigationPane Owner {
			get { return ownerCore; }
		}
		public virtual Size CalcNCMinSize(Graphics g, Rectangle bounds, NavigationPanePainter painter){
			Size buttonSize = Owner.ButtonsPanel.ViewInfo.CalcMinSize(g);
			Rectangle buttonClientBounds = new Rectangle(Point.Empty, new Size(buttonSize.Width, bounds.Height));
			Size buttonsPanelSize = painter.CalcBoundsByClientRectangle(this, buttonClientBounds).Size;
			return buttonsPanelSize;
		}
		public virtual Rectangle CalcNC(Graphics g, Rectangle bounds, NavigationPanePainter painter) {
			Owner.ButtonsPanel.ViewInfo.SetDirty();
			Size buttonSize = Owner.ButtonsPanel.ViewInfo.CalcMinSize(g);
			Rectangle buttonClientBounds = new Rectangle(Point.Empty, new Size(buttonSize.Width, bounds.Height));
			Size buttonsPanelSize = painter.CalcBoundsByClientRectangle(this, buttonClientBounds).Size;
			if(Owner.IsRightToLeftLayout())
				ButtonsBounds = DevExpress.Utils.PlacementHelper.Arrange(buttonsPanelSize, bounds, ContentAlignment.TopRight);
			else
				ButtonsBounds = DevExpress.Utils.PlacementHelper.Arrange(buttonsPanelSize, bounds, ContentAlignment.TopLeft);
			Owner.ButtonsPanel.ViewInfo.Calc(g, painter.GetObjectClientRectangle(this));
			int resizeZoneWidth = 3;
			if((Owner as INavigationFrame).SelectedPage != null) {
				resizeZoneWidth = Owner.IsRightToLeftLayout() ? (Owner as INavigationFrame).SelectedPage.BackgroundPadding.Left : Owner.SelectedPage.BackgroundPadding.Right;
				resizeZoneWidth += Owner.IsRightToLeftLayout() ? ((Owner as INavigationFrame).SelectedPage.Painter as NavigationPagePainter).SizingMargins.Left :
					((Owner as INavigationFrame).SelectedPage.Painter as NavigationPagePainter).SizingMargins.Right;
			}
			if(Owner.State != NavigationPaneState.Collapsed) {
				if(Owner.IsRightToLeftLayout()) {
					BackgroundBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width - ButtonsBounds.Width, bounds.Height);
					PageBounds = new Rectangle(bounds.Left, bounds.Top, bounds.Width - ButtonsBounds.Width, bounds.Height);
					ResizeBounds = new Rectangle(bounds.Left, bounds.Y, resizeZoneWidth, bounds.Height);
				}
				else {
					PageBounds = new Rectangle(ButtonsBounds.Right, bounds.Top, bounds.Width - ButtonsBounds.Width, bounds.Height);
					ResizeBounds = new Rectangle(bounds.Right - resizeZoneWidth, bounds.Y, resizeZoneWidth, bounds.Height);
				}
			}
			else {
				PageBounds = new Rectangle(ButtonsBounds.Right, bounds.Top, bounds.Width - ButtonsBounds.Width, bounds.Height);
				ResizeBounds = Rectangle.Empty;
			}
			CalcPages(g, PageBounds);
			if(selectedPageInfo != null && Owner.State != NavigationPaneState.Collapsed) {
				ClientBounds = selectedPageInfo.ClientBounds;
				return ClientBounds;
			}
			if(Owner.Pages.Count == 0)
				return Rectangle.Empty;
			return new Rectangle(bounds.Right, bounds.Top, 0, bounds.Width);
		}
		protected internal NavigationPageViewInfo selectedPageInfo;
		protected virtual void CalcPages(System.Drawing.Graphics g, Rectangle bounds) {
			if((Owner as INavigationFrame).SelectedPage == null) {
				selectedPageInfo = null;
				return;
			}
			selectedPageInfo = (Owner as INavigationFrame).SelectedPage.ViewInfo;
			selectedPageInfo.CalcNC(g, bounds, selectedPageInfo.Owner.Painter);
		}
	}
	public class TabPaneViewInfo : NavigationPaneViewInfo {
		public TabPaneViewInfo(INavigationPane owner) : base(owner as ITabPane) { }
		public override Rectangle CalcNC(Graphics g, Rectangle bounds, NavigationPanePainter painter) {
			bounds.X = 0;
			Owner.ButtonsPanel.ViewInfo.SetDirty();
			Owner.ButtonsPanel.BeginUpdate();
			if((Owner as INavigationFrame).IsRightToLeft())
				Owner.ButtonsPanel.ContentAlignment = ContentAlignment.TopRight;
			else
				Owner.ButtonsPanel.ContentAlignment = ContentAlignment.TopLeft;
			(Owner.ButtonsPanel as DevExpress.XtraBars.Docking2010.ButtonsPanel).RightToLeft = (Owner as INavigationFrame).IsRightToLeft();
			Owner.ButtonsPanel.CancelUpdate();
			Size buttonSize = Owner.ButtonsPanel.ViewInfo.CalcMinSize(g);
			Rectangle buttonClientBounds = new Rectangle(Point.Empty, new Size(bounds.Width, buttonSize.Height));
			Size buttonsPanelSize = painter.CalcBoundsByClientRectangle(this, buttonClientBounds).Size;
			if(Owner.IsRightToLeftLayout())
				ButtonsBounds = DevExpress.Utils.PlacementHelper.Arrange(buttonsPanelSize, bounds, ContentAlignment.TopRight);
			else
				ButtonsBounds = DevExpress.Utils.PlacementHelper.Arrange(buttonsPanelSize, bounds, ContentAlignment.TopLeft);
			Owner.ButtonsPanel.ViewInfo.Calc(g, painter.GetObjectClientRectangle(this));
			PageBounds = new Rectangle(0, ButtonsBounds.Bottom, bounds.Width, bounds.Height - ButtonsBounds.Height);
			CalcPages(g, PageBounds);
			if(selectedPageInfo != null) {
				ClientBounds = selectedPageInfo.ClientBounds;
				return ClientBounds;
			}
			if(Owner.Pages.Count == 0)
				return Rectangle.Empty;
			return new Rectangle(bounds.Right, bounds.Top, 0, bounds.Width);
		}
		protected override void CalcPages(System.Drawing.Graphics g, Rectangle bounds) {
			if((Owner as INavigationFrame).SelectedPage == null) {
				selectedPageInfo = null;
				return;
			}
			selectedPageInfo = (Owner as INavigationFrame).SelectedPage.ViewInfo;
			selectedPageInfo.CalcNCWithoutCaption(g, bounds, selectedPageInfo.Owner.Painter);
		}
	}
}
