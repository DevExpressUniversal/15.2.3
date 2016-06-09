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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Bars {
	public class ImageAlignmentRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupImageAlignmentCaption); } }
	}
	public abstract class ImageAlignmentBarItem : CommandBarCheckItem {
		protected ImageAlignmentBarItem() {
			RibbonStyle = RibbonItemStyles.SmallWithoutText;
		}
	}
	public class ImageAlignmentTopLeftBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentTopLeft; } }
	}
	public class ImageAlignmentCenterLeftBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentCenterLeft; } }
	}
	public class ImageAlignmentBottomLeftBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentBottomLeft; } }
	}
	public class ImageAlignmentTopCenterBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentTopCenter; } }
	}
	public class ImageAlignmentCenterCenterBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentCenterCenter; } }
	}
	public class ImageAlignmentBottomCenterBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentBottomCenter; } }
	}
	public class ImageAlignmentTopRightBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentTopRight; } }
	}
	public class ImageAlignmentCenterRightBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentCenterRight; } }
	}
	public class ImageAlignmentBottomRightBarItem : ImageAlignmentBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ImageAlignmentBottomRight; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ImageAlignmentBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {			
			items.Add(new ImageAlignmentTopLeftBarItem());
			items.Add(new ImageAlignmentCenterLeftBarItem());
			items.Add(new ImageAlignmentBottomLeftBarItem());
			items.Add(new ImageAlignmentTopCenterBarItem());
			items.Add(new ImageAlignmentCenterCenterBarItem());
			items.Add(new ImageAlignmentBottomCenterBarItem());
			items.Add(new ImageAlignmentTopRightBarItem());
			items.Add(new ImageAlignmentCenterRightBarItem());
			items.Add(new ImageAlignmentBottomRightBarItem());
		}
	}
	public class ImageAlignmentBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ImageToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ImageAlignmentRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ImageToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ImageToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ImageAlignmentRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ImageAlignmentBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new ImageToolsBar();
		}
	}
}
