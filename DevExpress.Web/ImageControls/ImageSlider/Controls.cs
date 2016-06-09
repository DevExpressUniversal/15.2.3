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
using System.Linq;
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using System.ComponentModel;
namespace DevExpress.Web.Internal {
	public abstract class ImageSliderControlBase : ASPxInternalWebControl {
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		private bool? hasNavigateUrl = null;
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		protected ImageSliderItemCollectionBase Items {
			get { return ImageSlider.ItemsInternal; }
		}
		protected ImageSliderDataHelper DataHelper {
			get { return ImageSlider.DataHelper; }
		}
		protected bool HasNavigateUrl {
			get {
				if(!hasNavigateUrl.HasValue)
					hasNavigateUrl = DataHelper.HasNavigateUrl;
				return hasNavigateUrl.Value;
			}
		}
		public ImageSliderControlBase(ASPxImageSliderBase control) {
			ImageSlider = control;
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AssignAttributes(ImageSlider, this);
			ImageSlider.GetControlStyle().AssignToControl(this);
		}
	}
	[ToolboxItem(false)]
	public class ImageSliderControl : ImageSliderControlBase {
		public ImageSliderControl(ASPxImageSliderBase control)
			: base(control) {
		}
		protected override void CreateControlHierarchy() {
			if(!ImageSlider.SeoFriendlyInternal)
				return;
			foreach(ImageSliderItemBase item in Items)
				AddItem(item);
		}
		protected void AddItem(ImageSliderItemBase item) {
			Image image = RenderUtils.CreateImage();
			image.ImageUrl = DataHelper.GetImageUrl(item, false);
			image.AlternateText = DataHelper.GetItemText(item);
			WebControl control = image;
			if(HasNavigateUrl) {
				control = CreateHyperLink(item);
				control.Controls.Add(image);
			}
			Controls.Add(control);
		}
		protected WebControl CreateHyperLink(ImageSliderItemBase item) {
			InternalHyperLink hyperLink = RenderUtils.CreateHyperLink(true, true);
			hyperLink.NavigateUrl = DataHelper.GetNavigateUrl(item, false);
			return hyperLink;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ImageSliderImageAreaStyle imageAreaStyle = ImageSlider.GetImageAreaStyle();
			if(Width.IsEmpty)
				Width = imageAreaStyle.Width;
			if(Height.IsEmpty)
				Height = imageAreaStyle.Height;
			Style[HtmlTextWriterStyle.Display] = "none";
		}
	}
	[ToolboxItem(false)]
	public class TemplatesContainer : Control {
		public override void RenderControl(HtmlTextWriter writer) {
		}
	}
}
