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
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web.Design {
	public class ASPxImageZoomNavigatorDesigner : ASPxImageSliderDesignerBase {
		protected ASPxImageZoomNavigator ZoomNavigator {
			get { return ImageSliderInternal as ASPxImageZoomNavigator; }
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ASPxImageZoomNavigatorDesignerActionList(this);
		}
		public Orientation Orientation {
			get { return ZoomNavigator.Orientation; }
			set {
				ZoomNavigator.Orientation = value;
				PropertyChanged("Orientation");
			}
		}
		public NavigationButtonVisibilityMode NavigationButtonVisibility {
			get { return ZoomNavigator.NavigationButtonVisibility; }
			set {
				ZoomNavigator.NavigationButtonVisibility = value;
				PropertyChanged("NavigationButtonVisibility");
			}
		}
		public NavigationBarPagingMode PagingMode {
			get { return ZoomNavigator.PagingMode; }
			set {
				ZoomNavigator.PagingMode = value;
				PropertyChanged("PagingMode");
			}
		}
		public ActiveItemChangeAction ActiveItemChangeAction {
			get { return ZoomNavigator.ActiveItemChangeAction; }
			set {
				ZoomNavigator.ActiveItemChangeAction = value;
				PropertyChanged("ActiveItemChangeAction");
			}
		}
		public int VisibleItemCount {
			get { return ZoomNavigator.VisibleItemCount; }
			set {
				ZoomNavigator.VisibleItemCount = value;
				PropertyChanged("VisibleItemCount");
			}
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new ImageZoomNavigatorItemsOwner(ZoomNavigator, DesignerHost)));
		}
	}
	public class ASPxImageZoomNavigatorDesignerActionList : ASPxImageSliderDesignerActionListBase {
		protected ASPxImageZoomNavigatorDesigner ImageZoomNavigatorDesigner {
			get { return ImageSliderDesignerInternal as ASPxImageZoomNavigatorDesigner; }
		}
		public ASPxImageZoomNavigatorDesignerActionList(ASPxImageSliderDesignerBase designer)
			: base(designer) {
		}
		public Orientation Orientation {
			get { return ImageZoomNavigatorDesigner.Orientation; }
			set { ImageZoomNavigatorDesigner.Orientation = value; }
		}
		public NavigationButtonVisibilityMode NavigationButtonVisibility {
			get { return ImageZoomNavigatorDesigner.NavigationButtonVisibility; }
			set { ImageZoomNavigatorDesigner.NavigationButtonVisibility = value; }
		}
		public NavigationBarPagingMode PagingMode {
			get { return ImageZoomNavigatorDesigner.PagingMode; }
			set { ImageZoomNavigatorDesigner.PagingMode = value; }
		}
		public ActiveItemChangeAction ActiveItemChangeAction {
			get { return ImageZoomNavigatorDesigner.ActiveItemChangeAction; }
			set { ImageZoomNavigatorDesigner.ActiveItemChangeAction = value; }
		}
		public int VisibleItemsCount {
			get { return ImageZoomNavigatorDesigner.VisibleItemCount; }
			set { ImageZoomNavigatorDesigner.VisibleItemCount = value; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("Orientation", "Orientation"));
			collection.Add(new DesignerActionPropertyItem("NavigationButtonVisibility", "Navigation Button Visibility"));
			collection.Add(new DesignerActionPropertyItem("PagingMode", "Paging Mode"));
			collection.Add(new DesignerActionPropertyItem("ActiveItemChangeAction", "Active Item Change Action"));
			collection.Add(new DesignerActionPropertyItem("VisibleItemsCount", "Visible Items Count"));
			return collection;
		}
	}
	public class ImageZoomNavigatorItemsOwner : FlatCollectionItemsOwner<ImageZoomNavigatorItem> {
		public ImageZoomNavigatorItemsOwner(object control, IServiceProvider provider) 
			: base(control, provider, ((ASPxImageZoomNavigator)control).Items) {
		}
	}
}
