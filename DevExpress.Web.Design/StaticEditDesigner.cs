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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class ASPxLabelDesigner : ASPxEditDesignerBase {
		protected ASPxLabel Label { get { return Component as ASPxLabel; } }
		protected override string GetDesignTimeHtmlInternal() {
			ASPxLabel label = ViewControl as ASPxLabel;
			bool isTextEmpty = string.IsNullOrEmpty(label.Text);
			if(isTextEmpty)
				label.Text = "[" + label.ID + "]";
			string html = base.GetDesignTimeHtmlInternal();
			if(isTextEmpty)
				label.Text = "";
			return html;
		}
		public string Text {
			get { return Label.Text; }
			set {
				Label.Text = value;
				PropertyChanged("Text");
			}
		}
		public string AssociatedControlID {
			get { return Label.AssociatedControlID; }
			set {
				Label.AssociatedControlID = value;
				PropertyChanged("AssociatedControlID");
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new LabelDesignerActionList(this);
		}
	}
	public class LabelDesignerActionList : ASPxWebControlDesignerActionList {
		public LabelDesignerActionList(ASPxLabelDesigner designer)
			: base(designer) {
		}
		public string Text {
			get { return (Designer as ASPxLabelDesigner).Text; }
			set { (Designer as ASPxLabelDesigner).Text = value; }
		}
		[IDReferenceProperty, TypeConverter(typeof(AssociatedControlConverter))]
		public string AssociatedControlID {
			get { return (Designer as ASPxLabelDesigner).AssociatedControlID; }
			set { (Designer as ASPxLabelDesigner).AssociatedControlID = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = base.GetSortedActionItems();
			actionItems.Add(new DesignerActionPropertyItem("Text", "Text"));
			actionItems.Add(new DesignerActionPropertyItem("AssociatedControlID", "Associated Control ID"));
			return actionItems;
		}
	}
	public class ASPxImageBaseDesigner : ASPxEditDesignerBase {
		public override bool IsThemableControl() {
			return false;
		}
		public bool ShowLoadingImage {
			get { return (Component as ASPxImageBase).ShowLoadingImage; }
			set {
				(Component as ASPxImageBase).ShowLoadingImage = value;
				PropertyChanged("ShowLoadingImage");
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ImageBaseDesignerActionList(this);
		}
	}
	public class ImageBaseDesignerActionList : ASPxWebControlDesignerActionList {
		public ImageBaseDesignerActionList(ASPxImageBaseDesigner designer)
			: base(designer) {
		}
		public bool ShowLoadingImage {
			get { return (Designer as ASPxImageBaseDesigner).ShowLoadingImage; }
			set { (Designer as ASPxImageBaseDesigner).ShowLoadingImage = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = base.GetSortedActionItems();
			actionItems.Add(new DesignerActionPropertyItem("ShowLoadingImage", "Show Loading Image"));
			return actionItems;
		}
	}
	public class ASPxImageDesigner : ASPxImageBaseDesigner {
		public string ImageUrl {
			get { return (Component as ASPxImage).ImageUrl; }
			set {
				(Component as ASPxImage).ImageUrl = value;
				PropertyChanged("ImageUrl");
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ImageDesignerActionList(this);
		}
	}
	public class ImageDesignerActionList : ImageBaseDesignerActionList {
		public ImageDesignerActionList(ASPxImageDesigner designer)
			: base(designer) {
		}
		[UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl {
			get { return (Designer as ASPxImageDesigner).ImageUrl; }
			set { (Designer as ASPxImageDesigner).ImageUrl = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = base.GetSortedActionItems();
			actionItems.Add(new DesignerActionPropertyItem("ImageUrl", "Image Url"));
			return actionItems;
		}
	}
	public class ASPxHyperLinkDesigner : ASPxEditDesignerBase {
		protected ASPxHyperLink HyperLink { get { return Component as ASPxHyperLink; } }
		protected override string GetDesignTimeHtmlInternal() {
			ASPxHyperLink link = ViewControl as ASPxHyperLink;
			bool isTextEmpty = string.IsNullOrEmpty(link.Text);
			if(isTextEmpty)
				link.Text = "[" + link.ID + "]";
			string html = base.GetDesignTimeHtmlInternal();
			if(isTextEmpty)
				link.Text = "";
			return html;
		}
		public string Text {
			get { return HyperLink.Text; }
			set {
				HyperLink.Text = value;
				PropertyChanged("Text");
			}
		}
		public string ImageUrl {
			get { return HyperLink.ImageUrl; }
			set {
				HyperLink.ImageUrl = value;
				PropertyChanged("ImageUrl");
			}
		}
		public string NavigateUrl {
			get { return HyperLink.NavigateUrl; }
			set {
				HyperLink.NavigateUrl = value;
				PropertyChanged("NavigateUrl");
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new HyperLinkDesignerActionList(this);
		}
	}
	public class HyperLinkDesignerActionList : ASPxWebControlDesignerActionList {
		public HyperLinkDesignerActionList(ASPxHyperLinkDesigner designer)
			: base(designer) {
		}
		public string Text {
			get { return (Designer as ASPxHyperLinkDesigner).Text; }
			set { (Designer as ASPxHyperLinkDesigner).Text = value; }
		}
		[UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl {
			get { return (Designer as ASPxHyperLinkDesigner).ImageUrl; }
			set { (Designer as ASPxHyperLinkDesigner).ImageUrl = value; }
		}
		[UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string NavigateUrl {
			get { return (Designer as ASPxHyperLinkDesigner).NavigateUrl; }
			set { (Designer as ASPxHyperLinkDesigner).NavigateUrl = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = base.GetSortedActionItems();
			actionItems.Add(new DesignerActionPropertyItem("Text", "Text"));
			actionItems.Add(new DesignerActionPropertyItem("ImageUrl", "Image Url"));
			actionItems.Add(new DesignerActionPropertyItem("NavigateUrl", "Navigate Url"));
			return actionItems;
		}
	}
	public class HyperLinkDataBindingHandler : DataBindingHandler {
		public override void DataBindControl(IDesignerHost designerHost, Control control) {
			DataBindingCollection dataBindings = ((IDataBindingsAccessor)control).DataBindings;
			DataBinding textBinding = dataBindings["Text"];
			DataBinding urlBinding = dataBindings["NavigateUrl"];
			if((textBinding != null) || (urlBinding != null)) {
				ASPxHyperLink link = (ASPxHyperLink)control;
				if(textBinding != null)
					link.Text = StringResources.SampleDataBoundText;
				if(urlBinding != null)
					link.NavigateUrl = StringResources.SampleDataBoundText;
			}
		}
	}
}
