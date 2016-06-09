#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class MapShapefileRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupMapShapefileCaption); } }
	}
	public class MapLoadBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapLoad; } }
		protected override string GetDefaultCaption() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandMapLoadRibbonCaption);
		}
	}
	public class MapImportBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapImport; } }
		protected override string GetDefaultCaption() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.CommandMapImportRibbonCaption);
		}
	}
	public class MapDefaultShapefileBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapDefaultShapefile; } }
	}
	public class MapWorldCountriesBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapWorldCountries; } }
	}
	public class MapEuropeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapEurope; } }
	}
	public class MapAsiaBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapAsia; } }
	}
	public class MapNorthAmericaBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapNorthAmerica; } }
	}
	public class MapSouthAmericaBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapSouthAmerica; } }
	}
	public class MapAfricaBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapAfrica; } }
	}
	public class MapUSABarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapUSA; } }
	}
	public class MapCanadaBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapCanada; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class MapShapefileBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new MapLoadBarItem());
			items.Add(new MapImportBarItem());
			MapDefaultShapefileBarItem customShapefileBarItem = new MapDefaultShapefileBarItem();
			customShapefileBarItem.AddBarItem(new MapWorldCountriesBarItem());
			customShapefileBarItem.AddBarItem(new MapEuropeBarItem());
			customShapefileBarItem.AddBarItem(new MapAsiaBarItem());
			customShapefileBarItem.AddBarItem(new MapNorthAmericaBarItem());
			customShapefileBarItem.AddBarItem(new MapSouthAmericaBarItem());
			customShapefileBarItem.AddBarItem(new MapAfricaBarItem());
			customShapefileBarItem.AddBarItem(new MapUSABarItem());
			customShapefileBarItem.AddBarItem(new MapCanadaBarItem());
			items.Add(customShapefileBarItem);
		}
	}
	public class MapShapefileBarCreator<TPageCategory, TBar> : DashboardItemDesignBarCreator
		where TPageCategory : DashboardRibbonPageCategory, new()
		where TBar : MapToolsBarBase, new() {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MapShapefileRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MapShapefileRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MapShapefileBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new TBar();
		}
	}
}
