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
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxTreeList.Internal {
	[ToolboxItem(false)]
	public sealed class TreeListPager : ASPxPagerBase {
		ASPxTreeList treeList;
		public TreeListPager(ASPxTreeList treeList) 
			: base(treeList) {
			if(treeList == null)
				throw new ArgumentNullException();
			this.treeList = treeList;
			PagerSettings.Assign(TreeList.SettingsPager);
			Styles.Assign(TreeList.StylesPager);
			if(TreeList.SettingsPager.PageSizeItemSettings.Visible)
				Width = System.Web.UI.WebControls.Unit.Percentage(100);
		}
		ASPxTreeList TreeList { get { return treeList; } }
		TreeListDataHelper TreeDataHelper { get { return TreeList.TreeDataHelper; } }
		public override ISkinOwner ParentSkinOwner { get {return TreeList; } set { } }
		public override bool EnableViewState { get { return false; } set { } }
		protected override bool HasContent() { return true; }
		public override int PageCount { get { return TreeDataHelper.PageCount; } }
		public override int PageIndex { get { return TreeDataHelper.PageIndex; } set { TreeDataHelper.PageIndex = value; } }
		public override int ItemCount { get { return TreeDataHelper.TotalRowCount; } set { } }
		public override int ItemsPerPage { get { return TreeDataHelper.PageSize; } set { } }
		protected override bool RequireInlineLayout { get { return true; } }
		protected override string GetItemElementOnClick(string ID) {
			return TreeList.RenderHelper.GetPagerOnClick(ID);
		}
		protected override string GetPageSizeChangedHandler() {
			return TreeList.RenderHelper.GetPagerOnPageSizeChange();
		}
		protected override void PrepareControlHierarchy() {
			Font.CopyFrom(TreeList.Font);
			base.PrepareControlHierarchy();
		}
	}
}
