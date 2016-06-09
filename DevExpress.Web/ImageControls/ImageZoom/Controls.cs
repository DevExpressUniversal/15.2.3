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

using DevExpress.Web;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)]
	public class ImageZoomControlBase : ASPxInternalWebControl {
		protected ASPxImageZoom ImageZoom { get; private set; }
		protected ASPxImage Image { get; private set; }
		protected WebControl Wrapper { get; private set; }
		protected override bool HasRootTag() {
			return true;
		}
		public ImageZoomControlBase(ASPxImageZoom imageZoom) {
			ImageZoom = imageZoom;
		}
		protected override void CreateControlHierarchy() {
			Wrapper = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(Wrapper);
			Image = new ASPxImage() {
				ID = ImageZoomContstants.ImageID,
				ImageUrl = ImageZoom.ImageUrl,
				AlternateText = ImageZoom.AlternateText
			};
			if(ImageZoom.AccessibilityCompliant && string.IsNullOrEmpty(Image.AlternateText))
				Image.AlternateText = ImageUtils.GetAlternateTextByUrl(ImageZoom.ImageUrl);			
			Wrapper.Controls.Add(Image);
			if(ImageZoom.ShowHint)
				Wrapper.Controls.Add(new HintControl(ImageZoom));
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(ImageZoom, this);
			ImageZoom.GetControlStyle().AssignToControl(this);
			Wrapper.CssClass = ImageZoomStyles.WrapperClassName;
		}
	}
	[ToolboxItem(false)]
	public class ImageZoomControlDesignMode : ImageZoomControlBase {
		protected WebControl Cell { get; private set; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Table; }
		}
		public override ControlCollection Controls {
			get { return Cell.Controls; }
		}
		public ImageZoomControlDesignMode(ASPxImageZoom imageZoom)
			: base(imageZoom) {
		}
		protected override void CreateControlHierarchy() {
			CreateCell();
			base.CreateControlHierarchy();
		}
		protected void CreateCell() {
			WebControl row = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			base.Controls.Add(row);
			Cell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			row.Controls.Add(Cell);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.ApplyCellPaddingAndSpacing(this);
			if(string.IsNullOrEmpty(ImageZoom.ImageUrl)) {
				RenderUtils.AppendDefaultDXClassName(this, ImageZoomStyles.DesignTimeEmptyImageClassName);
				Image.ImageUrl = ImageZoom.Images.GetImageProperties(Page, ImageZoomImages.DesignTimeItemImageName).Url;
			}
			Image.Width = ImageZoom.Width;
			Image.Height = ImageZoom.Height;
		}
	}
	[ToolboxItem(false)]
	public class ImageZoomControl : ImageZoomControlBase {
		protected ASPxPopupControl ZoomWindow { get; private set; }
		protected ASPxPopupControl ExpandWindow { get; private set; }
		protected WebControl CloseButton { get; private set; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		public ImageZoomControl(ASPxImageZoom imageZoom)
			: base(imageZoom) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(ImageZoom.EnableZoomMode) {
				if(ImageZoom.SettingsZoomMode.ZoomWindowPosition == ZoomWindowPosition.Inside)
					Wrapper.Controls.Add(new ClipPanel(ImageZoom));
				else {
					ZoomWindow = CreatePopupControl();
					Controls.Add(ZoomWindow);
					ZoomWindow.Controls.Add(new ClipPanel(ImageZoom));
				}
			}
			if(ImageZoom.EnableExpandMode) {
				ExpandWindow = CreateModalPopup();
				Controls.Add(ExpandWindow);
				ExpandWindow.Controls.Add(new LargeImage(ImageZoom, ImageZoomStyles.ExpandWindowImageClassName));
				CloseButton = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				ExpandWindow.Controls.Add(CloseButton);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(ImageZoom.EnableZoomMode)
				PrepareZoomPosition();
			if(ImageZoom.EnableZoomMode && ImageZoom.SettingsZoomMode.ZoomWindowPosition != ZoomWindowPosition.Inside) {
				ZoomWindow.PopupControlRenderStyles.Assign(ImageZoom.StylesZoomWindow);
				ZoomWindow.PopupControlRenderImages.Assign(ImageZoom.ImagesZoomWindow);
			}
			if(ImageZoom.EnableExpandMode) {
				ExpandWindow.PopupControlRenderStyles.Assign(ImageZoom.StylesExpandWindow);
				ExpandWindow.PopupControlRenderImages.Assign(ImageZoom.ImagesExpandWindow);
				CloseButton.ID = ImageZoomContstants.CloseButtonID;
				ImageZoom.GetDefaultCloseButtonStyle().AssignToControl(CloseButton);
				ImageZoom.GetCloseButtonImage().AssignToControl(CloseButton, DesignMode, !Enabled);
			}
		}
		protected void PrepareZoomPosition() {
			switch(ImageZoom.SettingsZoomMode.ZoomWindowPosition) {
				case ZoomWindowPosition.Top:
				ZoomWindow.PopupVerticalAlign = PopupVerticalAlign.Above;
				ZoomWindow.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
				ZoomWindow.PopupVerticalOffset = -ImageZoom.SettingsZoomMode.ZoomWindowOffset;
				break;
				case ZoomWindowPosition.Right:
				ZoomWindow.PopupVerticalAlign = PopupVerticalAlign.TopSides;
				ZoomWindow.PopupHorizontalAlign = PopupHorizontalAlign.OutsideRight;
				ZoomWindow.PopupHorizontalOffset = ImageZoom.SettingsZoomMode.ZoomWindowOffset;
				break;
				case ZoomWindowPosition.Bottom:
				ZoomWindow.PopupVerticalAlign = PopupVerticalAlign.Below;
				ZoomWindow.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
				ZoomWindow.PopupVerticalOffset = ImageZoom.SettingsZoomMode.ZoomWindowOffset;
				break;
				case ZoomWindowPosition.Left:
				ZoomWindow.PopupVerticalAlign = PopupVerticalAlign.TopSides;
				ZoomWindow.PopupHorizontalAlign = PopupHorizontalAlign.OutsideLeft;
				ZoomWindow.PopupHorizontalOffset = -ImageZoom.SettingsZoomMode.ZoomWindowOffset;
				break;
			}
		}
		protected ASPxPopupControl CreatePopupControlBase(string text) {
			return new ASPxPopupControl() {
				ShowHeader = false,
				EnableTheming = false,
				PopupAnimationType = AnimationType.Fade,
				CloseAnimationType = AnimationType.Fade,
				ShowFooter = ImageZoom.HasImageZoomNavigator() || !string.IsNullOrEmpty(text),
				FooterText = ImageZoom.HtmlEncode(text)
			};
		}
		protected ASPxPopupControl CreatePopupControl() {
			var popup = CreatePopupControlBase(ImageZoom.ZoomWindowText);
			popup.ID = ImageZoomContstants.ZoomWindowID;
			popup.PopupAction = PopupAction.None;
			popup.CloseAction = CloseAction.None;
			return popup;
		}
		protected ASPxPopupControl CreateModalPopup() {
			var popup = CreatePopupControlBase(ImageZoom.ExpandWindowText);
			popup.ID = ImageZoomContstants.ExpandWindowID;
			popup.CloseOnEscape = true;
			popup.Modal = true;
			popup.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			popup.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popup.ShowPageScrollbarWhenModal = true;
			popup.CssClass = ImageZoomStyles.ExpandWindowClassName;
			return popup;
		}
	}
	[ToolboxItem(false)]
	public class LargeImage : ASPxImage {
		public LargeImage(ASPxImageZoom imageZoom, string cssClass) {
			CssClass = cssClass;
			ClientIDMode = ClientIDMode.Static;
			if(!imageZoom.CanCreateLargeImageOnClient())
				ImageUrl = string.IsNullOrEmpty(imageZoom.LargeImageUrl) ? imageZoom.ImageUrl : imageZoom.LargeImageUrl;
			AlternateText = imageZoom.AlternateText;
			if(imageZoom.AccessibilityCompliant && string.IsNullOrEmpty(AlternateText))
				AlternateText = ImageUtils.GetAlternateTextByUrl(imageZoom.ImageUrl);
		}
	}
	[ToolboxItem(false)]
	public class ClipPanel : ASPxInternalWebControl {
		protected ASPxImageZoom ImageZoom { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		public ClipPanel(ASPxImageZoom imageZoom) {
			ImageZoom = imageZoom;
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new LargeImage(ImageZoom, ImageZoomStyles.ZoomWindowImageClassName));
		}
		protected override void PrepareControlHierarchy() {
			CssClass = ImageZoom.GetClipPanelCssClassName();
		}
	}
	[ToolboxItem(false)]
	public class HintControl : ASPxInternalWebControl {
		protected ASPxImageZoom ImageZoom { get; private set; }
		protected Image ImageHint { get; private set; }
		protected WebControl Text { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		public HintControl(ASPxImageZoom imageZoom) {
			ImageZoom = imageZoom;
		}
		protected override void CreateControlHierarchy() {
			ImageHint = RenderUtils.CreateImage();
			Controls.Add(ImageHint);
			if(ImageZoom.ShowHintText) {
				Text = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				Controls.Add(Text);
			}
		}
		protected override void PrepareControlHierarchy() {
			CssClass = ImageZoomStyles.HintClassName;
			ImageZoom.GetHintImage().AssignToControl(ImageHint, DesignMode, !Enabled);
			if(ImageZoom.ShowHintText)
				Text.Controls.Add(RenderUtils.CreateLiteralControl(ImageZoom.HtmlEncode(ImageZoom.GetHintText())));
		}
	}
}
