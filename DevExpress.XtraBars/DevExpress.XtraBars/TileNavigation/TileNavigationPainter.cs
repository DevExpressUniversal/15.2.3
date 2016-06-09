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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ButtonPanel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraBars.Navigation {
	class OfficeNavigationBarPainter : TileControlPainter {
		public override void Draw(TileControlInfoArgs e) {
			base.Draw(e);
			NavigationBarCore ownerCore = e.ViewInfo.Owner as NavigationBarCore;
			if(ownerCore == null) return;
			ObjectPainter.DrawObject(e.Cache, ownerCore.GetButtonsPanelPainter(), (ObjectInfoArgs)ownerCore.ButtonsPanel.ViewInfo);
		}
		protected override void DrawSelected(TileControlInfoArgs e, TileItemViewInfo itemInfo) { }
		protected override void DrawRectangleNewGroup(TileControlInfoArgs e, Color color, Rectangle rect) { }
		protected override void DrawItemSimpleTexts(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			DrawItemSimpleTextsBackground(e, itemInfo);
			foreach(TileItemElementViewInfo elementInfo in itemInfo.Elements) {
				NavigationBarElementViewInfo eInfo = elementInfo as NavigationBarElementViewInfo;
				if(eInfo == null) return;
				if(e.ViewInfo.Owner.Properties.Orientation == System.Windows.Forms.Orientation.Horizontal)
					e.Cache.Graphics.DrawString(eInfo.Text, eInfo.PaintAppearance.Font, e.Cache.GetSolidBrush(eInfo.PaintAppearance.ForeColor), eInfo.TextBounds, eInfo.StringFormat);
				else
					using(StringFormat str = eInfo.PaintAppearance.TextOptions.GetStringFormat().Clone() as StringFormat) {
						str.FormatFlags = StringFormatFlags.NoWrap;
						eInfo.PaintAppearance.DrawVString(e.Cache, eInfo.Text, eInfo.PaintAppearance.Font, e.Cache.GetSolidBrush(eInfo.PaintAppearance.ForeColor), eInfo.TextBounds, str, 270);
					}
			}
		}
		protected override void DrawItemTexts(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(((OfficeNavigationBarViewInfo)e.ViewInfo).OwnerCore.Compact)
				return;
			base.DrawItemTexts(e, itemInfo);
		}
		protected override void DrawItemImage(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(((OfficeNavigationBarViewInfo)e.ViewInfo).OwnerCore.Compact)
				base.DrawItemImage(e, itemInfo);
		}
		protected override void DrawItemBackground(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(e.ViewInfo is OfficeNavigationBarViewInfo && (e.ViewInfo as OfficeNavigationBarViewInfo).OwnerCore.AllowItemSkinning) {
				NavigationBarItemViewInfo itemInfoCore = itemInfo as NavigationBarItemViewInfo;
				if(itemInfoCore == null) return;
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, itemInfoCore.GetSkinInfo());
				return;
			}
			base.DrawItemBackground(e, itemInfo);
		}
		protected override void DrawItemBorder(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			if(e.ViewInfo is OfficeNavigationBarViewInfo && (e.ViewInfo as OfficeNavigationBarViewInfo).OwnerCore.AllowItemSkinning)
				return;
			base.DrawItemBorder(e, itemInfo);
		}
		protected override void DrawItemHtmlTexts(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			XPaint prev = e.Cache.Paint;
			e.Cache.Paint = new XPaint();
			try {
				foreach(TileItemElementViewInfo info in itemInfo.Elements) {
					if(info != null && info.StringInfo != null)
						StringPainter.Default.DrawString(e.Cache, info.StringInfo);
				}
			}
			finally {
				e.Cache.Paint = prev;
			}
		}
	}
	class NavigationBarButtonsPanelPainter : BaseButtonsPanelSkinPainter {
		public NavigationBarButtonsPanelPainter(ISkinProvider provider)
			: base(provider) {			
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new NavigationBarButtonSkinPainter(Provider);
		}
	}
	class NavigationBarButtonSkinPainter : BaseButtonSkinPainter {
		public NavigationBarButtonSkinPainter(ISkinProvider provider) : base(provider) { }
		protected void SetDefaultForeColor(BaseButtonInfo info) { }
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Button is NavigationBarCustomizationButton) return;
			base.DrawBackground(cache, info);
		}
		protected override void CheckForeColor(BaseButtonInfo info) {
			base.CheckForeColor(info);
			if(info.Button is NavigationBarCustomizationButton) {
				OfficeNavigationBarViewInfo barInfo = (info.Button as NavigationBarCustomizationButton).Owner.ControlCore.ViewInfoCore;
				if(info.Pressed) {
					info.PaintAppearance.ForeColor = barInfo.GetDefaultButtonPressedColor();
					return;
				}
				if(info.Hot) {
					info.PaintAppearance.ForeColor = barInfo.GetDefaultButtonHotColor();
					return;
				}
				info.PaintAppearance.ForeColor = barInfo.GetDefaultButtonForeColor();
			}
		}
	}
}
