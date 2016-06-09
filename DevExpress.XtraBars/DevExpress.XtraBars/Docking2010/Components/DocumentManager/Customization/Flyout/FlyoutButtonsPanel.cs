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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class FlyoutButton : BaseButton, IButton {
		public FlyoutButton(FlyoutCommand command) {
			Caption = command.Text;
			Image = command.Image;
			Command = command;
		}
		public FlyoutCommand Command { get; set; }
	}
	public class FlyoutButtonInfo : BaseButtonInfo {
		public FlyoutButtonInfo(IBaseButton button)
			: base(button) {
		}
		public override void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal, bool calcIsLeft) {
			TextBounds = ImageBounds = Content = Rectangle.Empty;
			if(!Button.Properties.Visible) return;
			UpdatePaintAppearance(painter);
			UpdateActualImage(painter);
			bool hasText = CheckCaption(Button.Properties);
			bool hasImage = CheckImage(Button.Properties);
			Size textSize = GetTextSize(g, hasText);
			Size imageSize = GetImageSize(hasImage);
			int interval = textSize.IsEmpty || imageSize.IsEmpty ? 0 : painter.ImageToTextInterval;
			Size minSize = GetMinSize(painter, hasText, hasImage);
			Size contentSize = GetContentSize(textSize, imageSize, interval, minSize);
			Rectangle r = painter.CalcBoundsByClientRectangle(this, new Rectangle(Point.Empty, contentSize));
			r.Width = Math.Max(r.Width, maxRect.Width);
			r.Height = Math.Max(r.Height, maxRect.Height);
			Bounds = new Rectangle(offset, r.Size);
			Content = painter.GetObjectClientRectangle(this);
			CalcImageAndTextBounds(Content.Location, textSize, imageSize, interval, Content);
		}
		protected override void CalcDefault(ref Rectangle first, ref Rectangle second, Point offset, int interval, Rectangle content) {
			CalcCenter(ref first, ref second, offset, interval, content);
		}
	}
	public class FlyoutButtonPainter : BaseButtonPainter {
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 18, 4);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -18, -4);
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			Rectangle innerBounds = Rectangle.Inflate(info.Bounds, -2, -2);
			cache.Graphics.FillRectangle(info.PaintAppearance.GetForeBrush(cache), info.Bounds);
			cache.Graphics.FillRectangle(info.PaintAppearance.GetBackBrush(cache), innerBounds);
			if(info.Hot)
				cache.Graphics.FillRectangle(cache.GetSolidBrush(Color.FromArgb(125, info.PaintAppearance.ForeColor)), innerBounds);
			if(info.Pressed)
				cache.Graphics.FillRectangle(info.PaintAppearance.GetForeBrush(cache), innerBounds);
		}
		protected override void DrawText(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Pressed)
				cache.DrawString(info.Caption, info.PaintAppearance.GetFont(), info.PaintAppearance.GetBackBrush(cache), info.TextBounds, info.PaintAppearance.TextOptions.GetStringFormat());
			else
				base.DrawText(cache, info);
		}
	}
	public class FlyoutButtonsPanel : ButtonsPanel {
		public FlyoutButtonsPanel(IButtonsPanelOwner owner)
			: base(owner) {
			ButtonSize = new Size(100, 30);
		}
		public Size ButtonSize { get; set; }
		protected override IButtonsPanelViewInfo CreateViewInfo() {
			return new FlyoutButtonsPanelViewInfo(this);
		}
	}
	public class FlyoutButtonsPanelViewInfo : BaseButtonsPanelViewInfo {
		public FlyoutButtonsPanelViewInfo(IButtonsPanel panel)
			: base(panel) {
		}
		FlyoutButtonsPanel FlyoutButtonsPanel { get { return Panel as FlyoutButtonsPanel; } }
		protected override BaseButtonInfo CreateButtonInfo(IBaseButton button) {
			return new FlyoutButtonInfo(button);
		}
		public override Size CalcMinSize(Graphics g) {
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			if(IsReady && !Content.Size.IsEmpty)
				return painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, Content.Size)).Size;
			Size result = new Size(0, 0);
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			int visibleButtons = 0;
			Size customButtonSize = Size.Empty;
			if(FlyoutButtonsPanel != null)
				customButtonSize = FlyoutButtonsPanel.ButtonSize;
			foreach(IBaseButton button in Panel.Buttons) {
				BaseButtonInfo buttonInfo = CreateButtonInfo(button);
				Size buttonSize = buttonInfo.CalcMinSize(g, buttonPainter);
				visibleButtons += buttonSize.Width != 0 ? 1 : 0;
				result.Width = Panel.IsHorizontal ? result.Width + Math.Max(buttonSize.Width, customButtonSize.Width) : Math.Max(customButtonSize.Width, Math.Max(buttonSize.Width, result.Width));
				result.Height = Panel.IsHorizontal ? Math.Max(result.Height, Math.Max(buttonSize.Height, customButtonSize.Height)) : result.Height + Math.Max(customButtonSize.Width, buttonSize.Height);
			}
			if(Panel.IsHorizontal)
				result.Width += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			else
				result.Height += visibleButtons >= 2 ? (visibleButtons - 1) * Panel.ButtonInterval : 0;
			MinSize = painter.CalcBoundsByClientRectangle(null, new Rectangle(Point.Empty, result)).Size;
			return MinSize;
		}
		public override void Calc(Graphics g, Rectangle bounds) {
			if(IsReady) return;
			Bounds = bounds;
			BaseButtonsPanelPainter painter = Panel.Owner.GetPainter() as BaseButtonsPanelPainter;
			BaseButtonPainter buttonPainter = painter.GetButtonPainter();
			Content = painter.GetObjectClientRectangle(this); ;
			IBaseButton[] buttons = SortButtonList(Panel.Buttons);
			Buttons = new List<BaseButtonInfo>();
			foreach(IBaseButton button in buttons) {
				BaseButtonInfo info = CreateButtonInfo(button);
				Buttons.Add(info);
			}
			CalcButtonInfos(g, buttonPainter, Panel.ButtonInterval, true, Buttons, Content.Location);
			Bounds = painter.CalcBoundsByClientRectangle(this, Content);
			Panel.BeginUpdate();
			Panel.Bounds = Bounds;
			Panel.CancelUpdate();
		}
		protected override Point CalcButtonInfos(Graphics g, BaseButtonPainter buttonPainter, int interval, bool horz, IEnumerable<BaseButtonInfo> buttons, Point offset) {
			int width = 0; 
			if(FlyoutButtonsPanel != null)
				width = FlyoutButtonsPanel.ButtonSize.Width;
			foreach(BaseButtonInfo buttonInfo in buttons) {
				Rectangle r = new Rectangle(Point.Empty, new Size(width, Content.Height));
				buttonInfo.Calc(g, buttonPainter, offset, r, horz);
				offset.X += (buttonInfo.Bounds.Width + interval);
			}
			return offset;
		}
	}
	public class FlyoutButtonsPanelSkinPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new FlyoutButtonPainter();
		}
	}
}
