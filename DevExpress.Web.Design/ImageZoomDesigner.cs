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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web;
namespace DevExpress.Web.Design {
	public class ASPxImageZoomDesigner : ASPxWebControlDesigner {
		public ASPxImageZoom ImageZoom { get; private set; }
		public override bool IsThemableControl() {
			return false;
		}
		public string AssociatedImageZoomNavigatorID {
			get { return ImageZoom.AssociatedImageZoomNavigatorID; }
			set {
				ImageZoom.AssociatedImageZoomNavigatorID = value;
				PropertyChanged("AssociatedImageZoomNavigatorID");
			}
		}
		public bool EnableZoomMode {
			get { return ImageZoom.EnableZoomMode; }
			set {
				ImageZoom.EnableZoomMode = value;
				PropertyChanged("EnableZoomWindow");
			}
		}
		public bool EnableExpandMode {
			get { return ImageZoom.EnableExpandMode; }
			set {
				ImageZoom.EnableExpandMode = value;
				PropertyChanged("EnableExpandMode");
			}
		}
		public MouseBoxOpacityMode MouseBoxOpacityMode {
			get { return ImageZoom.SettingsZoomMode.MouseBoxOpacityMode; }
			set {
				ImageZoom.SettingsZoomMode.MouseBoxOpacityMode = value;
				PropertyChanged("SettingsZoomMode.MouseBoxOpacityMode");
			}
		}
		public ZoomWindowPosition ZoomWindowPosition {
			get { return ImageZoom.SettingsZoomMode.ZoomWindowPosition; }
			set {
				ImageZoom.SettingsZoomMode.ZoomWindowPosition = value;
				PropertyChanged("SettingsZoomMode.ZoomWindowPosition");
			}
		}
		public string ImageUrl {
			get { return ImageZoom.ImageUrl; }
			set {
				ImageZoom.ImageUrl = value;
				PropertyChanged("ImageUrl");
			}
		}
		public string LargeImageUrl {
			get { return ImageZoom.LargeImageUrl; }
			set {
				ImageZoom.LargeImageUrl = value;
				PropertyChanged("LargeImageUrl");
			}
		}
		public override void Initialize(IComponent component) {
			ImageZoom = (ASPxImageZoom)component;
			base.Initialize(component);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxImageZoomDesignerActionList(this);
		}
	}
	public class ASPxImageZoomDesignerActionList : ASPxWebControlDesignerActionList {
		protected ASPxImageZoomDesigner ImageZoomDesigner { get { return (ASPxImageZoomDesigner)Designer; } }
		public bool EnableZoomMode {
			get { return ImageZoomDesigner.EnableZoomMode; }
			set { ImageZoomDesigner.EnableZoomMode = value; }
		}
		public bool EnableExpandMode {
			get { return ImageZoomDesigner.EnableExpandMode; }
			set { ImageZoomDesigner.EnableExpandMode = value; }
		}
		public MouseBoxOpacityMode MouseBoxOpacityMode {
			get { return ImageZoomDesigner.MouseBoxOpacityMode; }
			set { ImageZoomDesigner.MouseBoxOpacityMode = value; }
		}
		public ZoomWindowPosition ZoomWindowPosition {
			get { return ImageZoomDesigner.ZoomWindowPosition; }
			set { ImageZoomDesigner.ZoomWindowPosition = value; }
		}
		[UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl {
			get { return ImageZoomDesigner.ImageUrl; }
			set { ImageZoomDesigner.ImageUrl = value; }
		}
		[UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string LargeImageUrl {
			get { return ImageZoomDesigner.LargeImageUrl; }
			set { ImageZoomDesigner.LargeImageUrl = value; }
		}
		[IDReferenceProperty, TypeConverter(typeof(AssociatedControlConverter))]
		public string AssociatedImageZoomNavigatorID {
			get { return ImageZoomDesigner.AssociatedImageZoomNavigatorID; }
			set { ImageZoomDesigner.AssociatedImageZoomNavigatorID = value; }
		}
		public ASPxImageZoomDesignerActionList(ASPxImageZoomDesigner designer)
			: base(designer) {
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("EnableZoomMode", "Enable Zoom Mode"));
			collection.Add(new DesignerActionPropertyItem("EnableExpandMode", "Enable Expand Mode"));
			collection.Add(new DesignerActionPropertyItem("ImageUrl", "Image Url"));
			collection.Add(new DesignerActionPropertyItem("LargeImageUrl", "Large Image Url"));
			collection.Add(new DesignerActionPropertyItem("ZoomWindowPosition", "Zoom Window Position"));
			collection.Add(new DesignerActionPropertyItem("MouseBoxOpacityMode", "MouseBox Opacity Mode"));
			collection.Add(new DesignerActionPropertyItem("AssociatedImageZoomNavigatorID", "Associated Image Zoom Navigator ID"));
			return collection;
		}
	}
}
