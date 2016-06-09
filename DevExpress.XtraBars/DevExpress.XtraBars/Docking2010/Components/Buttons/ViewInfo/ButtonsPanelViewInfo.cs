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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public class ButtonsPanelViewInfo : BaseButtonsPanelViewInfo {
		public ButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			if(button is BaseSeparator)
				return new WindowsUIBaseSeparatorInfo(button);
			if(button is Views.WindowsUI.ActionButton)
				return new WindowsUIBaseButtonInfo(button);
			return new BaseButtonInfo(button);
		}
	}
	public class OverviewButtonsPanelViewInfo : WindowsUIButtonsPanelViewInfo {
		public OverviewButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			return new OverviewButtonInfo(button);
		}
	}
	public class TabbedGroupButtonsPanelViewInfo : WindowsUIButtonsPanelViewInfo {
		const int DefaultTileSize = 150;
		public TabbedGroupButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		protected Views.WindowsUI.ITabbedGroupInfo Info {
			get { return ((TabbedGroupButtonsPanel)Panel).Owner as Views.WindowsUI.ITabbedGroupInfo; }
		}
		public override Size CalcMinSize(Graphics g) {
			Size baseSize = base.CalcMinSize(g);
			if(Info.IsTabHeaders) {
				int width = 0;
				if(Info.Group.Properties.HasTabWidth)
					width = Info.Group.Properties.ActualTabWidth;
				return new Size(System.Math.Max(baseSize.Width, width), baseSize.Height);
			}
			else {
				var tileSize = GetTileSize(Info.Group);
				var columnCount = GetColumnCount(Info.Group);
				return new Size(tileSize.Width * columnCount, tileSize.Height);
			}
		}
		static int GetColumnCount(Views.WindowsUI.TabbedGroup group) {
			return Math.Min(group.Items.Count, Math.Max(1, group.Properties.ActualTileColumnCount));
		}
		static Size GetTileSize(Views.WindowsUI.TabbedGroup group) {
			int tileSize = DefaultTileSize;
			if(group.Properties.HasTileSize)
				tileSize = group.Properties.ActualTileSize;
			return new Size(tileSize, tileSize);
		}
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			return Info.IsTabHeaders ? (BaseButtonInfo)new TabButtonInfo(button) : (BaseButtonInfo)new TileButtonInfo(button, Info);
		}
		public class TabButtonInfo : BaseButtonInfo {
			public TabButtonInfo(IBaseButton button)
				: base(button) {
			}
		}
		public class TileButtonInfo : BaseButtonInfo {
			Views.WindowsUI.ITabbedGroupInfo infoCore;
			public Views.WindowsUI.TabbedGroup Group {
				get { return infoCore.Group; }
			}
			public TileButtonInfo(IBaseButton button, Views.WindowsUI.ITabbedGroupInfo info)
				: base(button) {
				this.infoCore = info;
			}
			public override Size CalcMinSize(Graphics g, BaseButtonPainter painter) {
				return GetTileSize(Group);
			}
			public override void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal, bool calcIsLeft) {
				TextBounds = ImageBounds = Content = Rectangle.Empty;
				if(!Button.Properties.Visible) return;
				Size tileSize = GetTileSize(Group);
				Bounds = new Rectangle(offset, tileSize);
				Padding contentMargin = Group.Properties.HasTileContentMargin ?
					Group.Properties.ActualTileContentMargin : ((Views.WindowsUI.ITabbedGroupTileButtonPainter)painter).ContentMargin;
				Content = new Rectangle(
					Bounds.Left + contentMargin.Left,
					Bounds.Top + contentMargin.Top,
					Bounds.Width - contentMargin.Horizontal,
					Bounds.Height - contentMargin.Vertical);
				UpdatePaintAppearance(painter);
				UpdateActualImage(painter);
				bool hasText = CheckCaption(Button.Properties);
				bool hasImage = CheckImage(Button.Properties);
				Size textSize = GetTextSize(g, hasText);
				Size imageSize = GetImageSize(hasImage);
				if(hasText) {
					ContentAlignment textAlignment = hasImage ? ContentAlignment.TopLeft : ContentAlignment.MiddleCenter;
					if(Group.Properties.ActualTileTextAlignment != Views.WindowsUI.TileHeaderContentAlignment.Default)
						textAlignment = (ContentAlignment)(int)Group.Properties.ActualTileTextAlignment;
					textSize.Width = Math.Min(textSize.Width, Content.Width);
					textSize.Height = Math.Min(textSize.Height, Content.Height - imageSize.Height);
					TextBounds = DevExpress.Utils.PlacementHelper.Arrange(textSize, Content, textAlignment);
				}
				if(hasImage) {
					ContentAlignment imageAlignment = hasText ? ContentAlignment.BottomRight : ContentAlignment.MiddleCenter;
					if(Group.Properties.ActualTileTextAlignment != Views.WindowsUI.TileHeaderContentAlignment.Default)
						imageAlignment = (ContentAlignment)(int)Group.Properties.ActualTileImageAlignment;
					ImageBounds = DevExpress.Utils.PlacementHelper.Arrange(imageSize, Content, imageAlignment);
				}
			}
			protected override Size CalcButtonTextSize(GraphicsCache cache, AppearanceObject appearance) {
				DevExpress.Utils.Text.StringInfo info = null;
				if(ButtonPanelOwner != null && ButtonPanelOwner.AllowHtmlDraw)
					info = DevExpress.Utils.Text.StringPainter.Default.Calculate(cache.Graphics, appearance, appearance.TextOptions, Caption, Content, cache.Paint, this);
				return info != null ? info.Bounds.Size : Size.Round(appearance.CalcTextSize(cache, Caption, Content.Width)); ;
			}
			public override void UpdatePaintAppearance(BaseButtonPainter painter) {
				base.UpdatePaintAppearance(painter);
				PaintAppearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
				PaintAppearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
			}
		}
	}
}
