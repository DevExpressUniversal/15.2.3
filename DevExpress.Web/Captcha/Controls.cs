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
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Localization;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Captcha {
	public class RefreshButtonControl : SimpleButtonControl {
		const string ImageControlID = "RIMG";
		internal const string TextSpanID = "RTS";
		const string OnClickAttributeName = "onclick";
		RefreshButtonProperties properties;
		ASPxCaptcha owner;
		WebControl parentSpan;
		WebControl textSpan;
		public RefreshButtonControl(ASPxCaptcha owner)
			: base(owner.HtmlEncode(owner.RefreshButton.Text), owner.RefreshButton.Image, owner.RefreshButton.ImagePosition,
			RenderUtils.AccessibilityEmptyUrl) {
			this.owner = owner;
			this.properties = owner.RefreshButton;
			IsRightToLeft = (owner as ISkinOwner).IsRightToLeft();
		}
		protected ImageProperties GetDefaultRefreshButtonImage() {
			return this.owner.Images.GetImageProperties(Page, CaptchaImages.RefreshButtonDefaultImageName);
		}
		protected override void CreateControlHierarchy() {
			WebControl parent = null;
			if (!Enabled) {
				this.parentSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				this.Controls.Add(this.parentSpan);
				parent = this.parentSpan;
			} else  {
				Hyperlink = RenderUtils.CreateHyperLink();
				Controls.Add(Hyperlink);
				parent = Hyperlink;
			}
			if (this.properties.ShowImage &&
				(ButtonImagePosition == ImagePosition.Left ||
				ButtonImagePosition == ImagePosition.Top)) {
				CreateImage(parent);
				if (ButtonImagePosition == ImagePosition.Top && !IsTextEmpty() && !IsImageEmpty())
					parent.Controls.Add(RenderUtils.CreateBr());
			}
			if (!IsTextEmpty()) {
				this.textSpan = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				this.textSpan.ID = TextSpanID;
				parent.Controls.Add(this.textSpan);
				CreateText(textSpan);
			}
			if (!IsImageEmpty() &&
				(ButtonImagePosition == ImagePosition.Right ||
				ButtonImagePosition == ImagePosition.Bottom)) {
				if (ButtonImagePosition == ImagePosition.Bottom && !IsTextEmpty() && !IsImageEmpty())
					parent.Controls.Add(RenderUtils.CreateBr());
				CreateImage(parent);
			}
		}
		protected override void PrepareControlHierarchy() {
			if (this.parentSpan != null)
				ButtonStyle.AssignToControl(this.parentSpan, AttributesRange.All);
			PrepareHyperlink();
			PrepareImageControl();
			if (TextControl != null)
				PrepareText();
			if (textSpan != null) {
				this.owner.GetRefreshButtonTextStyle().AssignToControl(this.textSpan);
				RenderUtils.SetVerticalAlign(this.textSpan, VerticalAlign.Middle);
				if (Browser.IsIE && this.properties.ShowImage)
					RenderUtils.SetLineHeight(this.textSpan, Unit.Pixel(0));
			}
		}
		protected void PrepareImageControl() {
			if (ImageControl == null)
				return;
			ButtonImageSpacing = this.owner.GetRefreshButtonStyle().ImageSpacing;
			PrepareImage();
			ImageControl.ID = ImageControlID;
			if (string.IsNullOrEmpty(this.properties.Image.Url))
				GetDefaultRefreshButtonImage().AssignToControl(ImageControl, DesignMode, !Enabled);
			else
				this.properties.Image.AssignToControl(ImageControl, DesignMode, !Enabled);
		}
		protected void PrepareHyperlink() {
			if (Hyperlink == null)
				return;
			RenderUtils.PrepareHyperLink(Hyperlink, string.Empty, ButtonUrl, string.Empty,
				string.Empty, Enabled);
			if (ButtonStyle != null && !ButtonStyle.IsEmpty)
				ButtonStyle.AssignToControl(Hyperlink, AttributesRange.All);
			RenderUtils.AssignAttributes(this, Hyperlink);
		}
	}
}
