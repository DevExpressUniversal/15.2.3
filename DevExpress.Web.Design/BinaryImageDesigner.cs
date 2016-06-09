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
namespace DevExpress.Web.Design {
	public class ASPxBinaryImageDesigner : ASPxEditDesignerBase {
		public override bool IsThemableControl() {
			return false;
		}
		ASPxBinaryImage BinaryImage { get { return (ASPxBinaryImage)Component; } }
		public bool AllowEdit {
			get { return BinaryImage.AllowEdit; }
			set {
				BinaryImage.AllowEdit = value;
				PropertyChanged("AllowEdit");
			}
		}
		public bool EnableServerResize {
			get { return BinaryImage.EnableServerResize; }
			set {
				BinaryImage.EnableServerResize = value;
				PropertyChanged("EnableServerResize");
			}
		}
		public BinaryStorageMode BinaryStorageMode {
			get { return BinaryImage.BinaryStorageMode; }
			set {
				BinaryImage.BinaryStorageMode = value;
				PropertyChanged("BinaryStorageMode");
			}
		}
		public ImageSizeMode ImageSizeMode {
			get { return BinaryImage.ImageSizeMode; }
			set {
				BinaryImage.ImageSizeMode = value;
				PropertyChanged("ImageSizeMode");
			}
		}
		public bool ShowLoadingImage {
			get { return BinaryImage.ShowLoadingImage; }
			set {
				BinaryImage.ShowLoadingImage = value;
				PropertyChanged("ShowLoadingImage");
			}
		}
		public Unit Width {
			get { return BinaryImage.Width; }
			set {
				BinaryImage.Width = value;
				PropertyChanged("Width");
			}
		}
		public Unit Height {
			get { return BinaryImage.Height; }
			set {
				BinaryImage.Height = value;
				PropertyChanged("Height");
			}
		}
		protected override bool IsControlRequireHttpHandlerRegistration() { return true; }
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new BinaryImageDesignerActionList(this);
		}
	}
	public class BinaryImageDesignerActionList : ASPxWebControlDesignerActionList {
		ASPxBinaryImageDesigner BinaryImageDesigner { get { return (ASPxBinaryImageDesigner) Designer; } }
		public BinaryImageDesignerActionList(ASPxBinaryImageDesigner designer)
			: base(designer) { }
		public bool AllowEdit {
			get { return BinaryImageDesigner.AllowEdit; }
			set { BinaryImageDesigner.AllowEdit = value; }
		}
		public bool EnableServerResize {
			get { return BinaryImageDesigner.EnableServerResize; }
			set { BinaryImageDesigner.EnableServerResize = value; }
		}
		public BinaryStorageMode BinaryStorageMode {
			get { return BinaryImageDesigner.BinaryStorageMode; }
			set { BinaryImageDesigner.BinaryStorageMode = value; }
		}
		public ImageSizeMode ImageSizeMode {
			get { return BinaryImageDesigner.ImageSizeMode; }
			set { BinaryImageDesigner.ImageSizeMode = value; }
		}
		public bool ShowLoadingImage {
			get { return BinaryImageDesigner.ShowLoadingImage; }
			set { BinaryImageDesigner.ShowLoadingImage = value; }
		}
		public Unit Width {
			get { return BinaryImageDesigner.Width; }
			set { BinaryImageDesigner.Width = value; }
		}
		public Unit Height {
			get { return BinaryImageDesigner.Height; }
			set { BinaryImageDesigner.Height = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection actionItems = base.GetSortedActionItems();
			actionItems.Add(new DesignerActionPropertyItem("AllowEdit", "Allow Edit"));
			actionItems.Add(new DesignerActionPropertyItem("Width", "Width"));
			actionItems.Add(new DesignerActionPropertyItem("Height", "Height"));
			actionItems.Add(new DesignerActionPropertyItem("EnableServerResize", "Enable Server Resize"));
			if(EnableServerResize)
				actionItems.Add(new DesignerActionPropertyItem("ImageSizeMode", "Image Size Mode"));
			actionItems.Add(new DesignerActionPropertyItem("BinaryStorageMode", "Binary Storage Mode"));
			actionItems.Add(new DesignerActionPropertyItem("ShowLoadingImage", "Show Loading Image"));
			return actionItems;
		}
	}
}
