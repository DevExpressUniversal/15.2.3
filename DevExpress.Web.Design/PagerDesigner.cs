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
	using DevExpress.Web.Internal;
	public class ASPxPagerDesigner : ASPxWebControlDesigner {
		private ASPxPager pager;
		public ASPxPager Pager {
			get { return pager; }
		}
		public override void Initialize(IComponent component) {
			this.pager = (ASPxPager)component;
			base.Initialize(component);
		}
		protected override string GetBaseProperty() {
			return "ItemCount";
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new PagerDesignerActionList(this);
		}
		public override bool HasClientSideEvents() {
			return false;
		}
		public override bool HasCommonDesigner() {
			return false;
		}
	}
	public class PagerDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxPagerDesigner designer;
		public int ItemCount {
			get { return designer.Pager.ItemCount; }
			set {
				IComponent component = designer.Component;
				TypeDescriptor.GetProperties(component)["ItemCount"].SetValue(component, value); 
			}
		}
		public PagerDesignerActionList(ASPxPagerDesigner pagerControlDesigner)
			: base(pagerControlDesigner) {
			this.designer = pagerControlDesigner;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Insert(0, new DesignerActionPropertyItem("ItemCount",
				StringResources.Pager_EditItemCount,
				StringResources.ActionList_MiscCategory,
				StringResources.Pager_EditItemCountDescription));
			return collection;
		}
	}
}
