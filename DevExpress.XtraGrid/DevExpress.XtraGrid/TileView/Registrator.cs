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

using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.Registrator {
	public class TileViewInfoRegistrator : BaseInfoRegistrator {
		public override bool IsInternalView { get { return false; } }
		public override string ViewName { get { return "TileView"; } }
		public override string StyleOwnerName { get { return "Grid"; } }
		public override BaseView CreateView(GridControl grid) {
			BaseView view = new TileView();
			view.SetGridControl(grid);
			return view;
		}
		public override BaseViewHandler CreateHandler(BaseView view) {
			return new TileViewHandler(view);
		}
		protected override void RegisterViewPaintStyles() {
			PaintStyles.Add(new TileViewPaintStyle());
		}
		protected override Design.BaseGridDesigner CreateDesigner() {
			return new TileViewDesigner();
		}
	}
	public class TileViewDesigner : BaseGridDesigner {
		protected override void CreateGroups() {
			base.CreateGroups();
			var group = CreateDefaultMainGroup();
		}
		protected override DevExpress.Utils.Design.DesignerGroup CreateDefaultMainGroup() {
			var groups = base.CreateDefaultMainGroup();
			var originalLayout = groups.GetItemByCaption("Layout");
			if(originalLayout != null)
				groups.RemoveAt(groups.IndexOf(originalLayout));
			groups.Insert(1, Properties.Resources.ItemColumnsCaption, Properties.Resources.ItemColumnsDescription, "DevExpress.XtraGrid.Design.Tile.TileViewColumnDesigner", GetDefaultLargeImage(1), GetDefaultSmallImage(1), null);
			groups.Add(Properties.Resources.TileViewTileTemplateCaption, Properties.Resources.TileViewTileTemplateDescription, "DevExpress.XtraGrid.Design.Tile.TileViewItemTemplateDesigner", GetDefaultLargeImage(4), GetDefaultSmallImage(4), null, false);
			return groups;
		}
	}
}
