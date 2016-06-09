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
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010 {
	public class WindowsUIBaseSeparatorInfo : BaseSeparatorInfo {
		static Size DefaultImageSize = new Size(1, 52);
		public WindowsUIBaseSeparatorInfo(IBaseButton separator)
			: base(separator) {
		}
		protected override Size GetImageSize(bool hasImage) {
			Size separatorSize = DefaultImageSize;
			Size imageSize = base.GetImageSize(hasImage);
			return new Size(Math.Max(separatorSize.Width, imageSize.Width), Math.Max(separatorSize.Height, imageSize.Height));
		}
	}
	public class WindowsUIBaseButtonInfo : BaseButtonInfo {
		static Size DefaultImageSize = new Size(40, 40);
		public WindowsUIBaseButtonInfo(IBaseButton button)
			: base(button) {
		}
		protected override Size GetImageSize(bool hasImage) {
			var windwosUIButtonPanelOwner = ButtonPanelOwner as IWindowsUIButtonPanelOwner;
			Size glyphSize = DefaultImageSize;
			if(windwosUIButtonPanelOwner != null && !windwosUIButtonPanelOwner.UseButtonBackgroundImages)
				glyphSize = Size.Empty;
			Size imageSize = base.GetImageSize(hasImage);
			return new Size(Math.Max(imageSize.Width, glyphSize.Width), Math.Max(imageSize.Height, glyphSize.Height));
		}
		protected override Size GetContentSize(Size textSize, Size imageSize, int interval, Size minSize) {
			bool isSide = IsSideImageLocation(Button.Properties.ImageLocation);
			int w = isSide ? textSize.Width + imageSize.Width + interval : Math.Max(textSize.Width, imageSize.Width);
			int h = isSide ? Math.Max(textSize.Height, imageSize.Height) : textSize.Height + imageSize.Height + interval;
			return !isSide ? new Size(Math.Max(w, minSize.Width), h) : new Size(w, Math.Max(h, minSize.Height));
		}
		protected new internal System.Collections.Generic.List<AppearanceObject> AppearanceList {
			get { return base.AppearanceList; }
			internal set { base.AppearanceList = value; }
		}
		protected new internal Control ParentControl { get { return base.ParentControl; } }
		protected internal AppearanceObject StateAppearances {
			get { return base.GetStateAppearance(); }
		}
		protected override IButtonsPanelOwner GetButtonPanelOwner() {
			DevExpress.XtraBars.Docking2010.Views.WindowsUI.IHeaderButton headerButton = Button as DevExpress.XtraBars.Docking2010.Views.WindowsUI.IHeaderButton;
			if(headerButton is DevExpress.XtraBars.Docking2010.Views.WindowsUI.ContentContainerHeaderInfo.BackButton)
				return null;
			if(headerButton != null)
				return headerButton.Owner;
			return base.GetButtonPanelOwner();
		}
	}
	public class OverviewButtonInfo : BaseButtonInfo {
		static Size DefaultItemSize = new Size(250, 90);
		public OverviewButtonInfo(IBaseButton button)
			: base(button) {
		}
		public StringInfo TextInfo { get; set; }
		protected internal virtual Views.WindowsUI.IOverviewContainerDefaultProperties OverviewContainerProperties {
			get { return ((Views.WindowsUI.OverviewButton)Button).OverviewContainerProperties; }
		}
		public override void Calc(Graphics g, BaseButtonPainter painter, Point offset, Rectangle maxRect, bool isHorizontal, bool calcIsLeft) {
			Size contentSize = GetContentSize();
			Rectangle r = painter.CalcBoundsByClientRectangle(this, new Rectangle(Point.Empty, contentSize));
			Bounds = new Rectangle(offset, r.Size);
			Content = painter.GetObjectClientRectangle(this);
			TextBounds = Rectangle.Empty;
			UpdatePaintAppearance(painter);
			UpdateActualImage(painter);
			bool hasText = CheckCaption(Button.Properties);
			bool hasImage = CheckImage(Button.Properties);
			Size imageSize = GetImageSize(hasImage);
			if(imageSize.Height > contentSize.Height || imageSize.Width > contentSize.Width)
				imageSize = contentSize;
			int interval = imageSize.IsEmpty || imageSize == contentSize ? 0 : painter.ImageToTextInterval;
			Rectangle maxTextBounds = GetMaxTextBounds(contentSize, imageSize, interval);
			Size textSize = GetTextSize(g, hasText, maxTextBounds);
			interval = textSize.IsEmpty || imageSize.IsEmpty ? 0 : painter.ImageToTextInterval;
			Size imageAndTextSize;
			if(IsHorizontalImageLocation())
				imageAndTextSize = new Size(textSize.Width + imageSize.Width + interval, Math.Max(textSize.Height, imageSize.Height));
			else
				imageAndTextSize = new Size(Math.Max(textSize.Width, imageSize.Width), textSize.Height + imageSize.Height + interval);
			Rectangle textAndImageBounds = PlacementHelper.Arrange(imageAndTextSize, Content, ContentAlignment.MiddleCenter);
			CalcImageAndTextBounds(textAndImageBounds.Location, textSize, imageSize, interval, textAndImageBounds);
		}
		protected Rectangle GetMaxTextBounds(Size contentSize, Size imageSize, int interval) {
			Rectangle result = Rectangle.Empty;
			if(IsHorizontalImageLocation())
				result.Size = new Size(contentSize.Width - imageSize.Width - interval, contentSize.Height);
			else
				result.Size = new Size(contentSize.Width, contentSize.Height - imageSize.Height - interval);
			if(result.Size.Height <= 0 || result.Size.Width <= 0)
				result.Size = Size.Empty;
			return result;
		}
		protected bool IsHorizontalImageLocation() {
			if(Button.Properties.ImageLocation == DevExpress.XtraEditors.ButtonPanel.ImageLocation.AboveText || Button.Properties.ImageLocation == DevExpress.XtraEditors.ButtonPanel.ImageLocation.BelowText) return false;
			return true;
		}
		protected virtual Size GetTextSize(Graphics g, bool hasText, Rectangle maxTextBounds) {
			if(!hasText || maxTextBounds.Size.IsEmpty) return Size.Empty;
			if(OverviewContainerProperties.CanHtmlDraw) {
				TextInfo = StringPainter.Default.Calculate(g, PaintAppearance, PaintAppearance.TextOptions, Caption, maxTextBounds);
				return TextInfo.Bounds.Size;
			}
			Size textSize = hasText ? Size.Round(PaintAppearance.CalcTextSize(g, Caption, maxTextBounds.Width)) : Size.Empty;
			if(textSize.Height > maxTextBounds.Height)
				textSize.Height = maxTextBounds.Height;
			return textSize;
		}
		protected Size GetContentSize() {
			if(OverviewContainerProperties.ActualItemSize.HasValue)
				return OverviewContainerProperties.ActualItemSize.Value;
			return DefaultItemSize;
		}
		public override Size CalcMinSize(Graphics g, BaseButtonPainter painter) {
			UpdateActualImage(painter);
			Size contentSize = GetContentSize();
			Rectangle client = new Rectangle(Point.Empty, contentSize);
			return painter.CalcBoundsByClientRectangle(this, client).Size;
		}
		public override void UpdatePaintAppearance(BaseButtonPainter painter) {
			AppearanceHelper.Combine(PaintAppearance,
				new AppearanceObject[] { GetStateAppearance(), Button.Properties.Appearance }, painter.DefaultAppearance);
			PaintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			PaintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
		}
		protected internal AppearanceObject StateAppearances {
			get { return GetStateAppearance(); }
		}
		protected override AppearanceObject GetStateAppearance() {
			if(Pressed)
				return OverviewContainerProperties.ActualAppearancePressed;
			if(Hot)
				return OverviewContainerProperties.ActualAppearanceHovered;
			return OverviewContainerProperties.ActualAppearanceNormal;
		}
	}
}
