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
	public class GeoPointMapClusterizationRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupGeoPointMapClusterizationCaption); } }
	}
	public class GeoPointMapClusterizationBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GeoPointMapClusterization; } }
	}
}
namespace DevExpress.DashboardWin.Native {	
	public class GeoPointMapClusterizationBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContex) {
			items.Add(new GeoPointMapClusterizationBarItem());
		}
	}
	public class GeoPointMapClusterizationBarCreator<TPageCategory, TBar> : DataBarCreator
		where TPageCategory : DashboardRibbonPageCategory, new()
		where TBar : MapToolsBarBase, new() {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GeoPointMapClusterizationRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GeoPointMapClusterizationRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new GeoPointMapClusterizationBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new TBar();
		}
	}
}
