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
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Navigation {
	public class NavigationPanePainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			NavigationPaneViewInfo info = e as NavigationPaneViewInfo;
			if(info != null) {
				DrawBackground(e.Graphics, info);
				DrawPage(e.Graphics, info);
				DrawButtonsBackground(e.Graphics, info);
				DrawButtons(e.Graphics, info);
			}
		}
		protected virtual void DrawButtonsBackground(Graphics graphics, NavigationPaneViewInfo info) {
			if(info.Owner.State !=  NavigationPaneState.Collapsed)
				graphics.FillRectangle(new SolidBrush(Color.FromArgb(205, 205, 205)), info.ButtonsBounds);
			else
				graphics.FillRectangle(new SolidBrush(Color.FromArgb(103, 103, 103)), info.ButtonsBounds);
		}
		protected virtual void DrawPage(Graphics graphics, NavigationPaneViewInfo info) {
			if(info.selectedPageInfo == null) return;
			NavigationPage selectedPage = (info.Owner as INavigationFrame).SelectedPage as NavigationPage;
			if(selectedPage != null && selectedPage.Properties.CanBorderColorBlending) {
				Color borderColor = selectedPage.Properties.ActualAppearanceCaption.BorderColor;
				using(SkinElementCustomColorizer colorizer = new SkinElementCustomColorizer(borderColor)) {
					ObjectPainter.DrawObject(info.Cache, info.selectedPageInfo.Owner.Painter, info.selectedPageInfo);
				}
			}
			else 
				ObjectPainter.DrawObject(info.Cache, info.selectedPageInfo.Owner.Painter, info.selectedPageInfo);
		}
		protected virtual void DrawBackground(Graphics graphics, NavigationPaneViewInfo info) {
			graphics.FillRectangle(Brushes.Red, info.Bounds);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			var info = e as NavigationPaneViewInfo;
			return new Rectangle(new Point(info.BackgroundBounds.X + 1, info.BackgroundBounds.Y), new Size(info.BackgroundBounds.Width - 2, info.BackgroundBounds.Height));
		}
		public int OverlapValue { get { return GetOverlapValue(); } }
		int GetOverlapValue() { return 0; }
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		protected virtual void DrawButtons(Graphics graphics, NavigationPaneViewInfo info) {
			ObjectPainter.DrawObject(info.Cache, (info.Owner as IButtonsPanelOwner).GetPainter(), info.Owner.ButtonsPanel.ViewInfo as ObjectInfoArgs);
		}
	}
	public class NavigationPaneSkinPainter : NavigationPanePainter {
		ISkinProvider skinProviderCore;
		public NavigationPaneSkinPainter(ISkinProvider provider) {
			skinProviderCore = provider;
		}
		protected ISkinProvider SkinProvider {
			get { return skinProviderCore; }
		}
		protected override void DrawButtonsBackground(Graphics graphics, NavigationPaneViewInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetButtonBackground());
			elementInfo.Bounds = info.ButtonsBounds;
			elementInfo.ImageIndex = info.Owner.State  != NavigationPaneState.Collapsed ? 0 : 1;
			if(elementInfo.Element != null)
				ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, elementInfo);
			else {
				SolidBrush brush = new SolidBrush(CommonSkins.GetSkin((info.Owner as NavigationFrame).LookAndFeel).Colors.GetColor("Control"));
				graphics.FillRectangle(brush, elementInfo.Bounds);
			}
		}
		protected override void DrawBackground(Graphics graphics, NavigationPaneViewInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetButtonBackground());
			elementInfo.Bounds = info.Bounds;
			ObjectPainter.DrawObject(info.Cache, SkinElementPainter.Default, elementInfo);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			var buttonBackground = GetButtonBackground() ;
			if(buttonBackground != null){
				if((e as NavigationPaneViewInfo).Owner.IsRightToLeftLayout()){
					SkinPaddingEdges saved = buttonBackground.ContentMargins;
					saved.Left = buttonBackground.ContentMargins.Right;
					saved.Right = buttonBackground.ContentMargins.Left;
					return saved.Inflate(client);
				}
				return buttonBackground.ContentMargins.Inflate(client);
			}
			return client;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			var info = e as NavigationPaneViewInfo;
			var buttonBackground = GetButtonBackground();
			if(buttonBackground != null)
				return buttonBackground.ContentMargins.Deflate(info.ButtonsBounds);
			return info.ButtonsBounds;
		}
		protected virtual Skin GetNavigationSkin() {
			return NavigationPaneSkins.GetSkin(SkinProvider);
		}
		protected virtual Skin GetSkin() {
			return DockingSkins.GetSkin(SkinProvider);
		}
		protected internal virtual SkinElement GetButtonBackground() {
			return  GetNavigationSkin()[NavigationPaneSkins.SkinPaneButtonsBackground] ??  GetSkin()[DockingSkins.SkinHideBarLeft];
		}
	}
	public class TabPaneSkinPainter : NavigationPaneSkinPainter {
		public TabPaneSkinPainter(ISkinProvider provider) : base(provider) { }
		protected internal override SkinElement GetButtonBackground() {
			return GetNavigationSkin()[NavigationPaneSkins.SkinTabPaneButtonsBackground];
		}
	}
}
