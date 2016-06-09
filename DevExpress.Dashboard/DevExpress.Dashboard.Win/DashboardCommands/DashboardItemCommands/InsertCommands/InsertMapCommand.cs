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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Commands {
	public class InsertChoroplethMapCommand : InsertItemCommand<ChoroplethMapDashboardItem> {
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameMapItem; } }
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertChoroplethMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertChoroplethMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertChoroplethMapDescription; } }
		public override string ImageName { get { return "InsertChoroplethMap"; } }
		public InsertChoroplethMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class InsertGeoPointMapBaseCommand : DashboardItemCommand<GeoPointMapDashboardItemBase> {
		public override DashboardCommandId Id { get { return DashboardCommandId.SelectGeoPointDashboardItemType; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertGeoPointMapsCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertGeoPointMapsDescription; } }
		public override string ImageName { get { return "InsertGeoPointMap"; } }
		public InsertGeoPointMapBaseCommand(DashboardDesigner control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
		}
		protected override void ExecuteInternal(ICommandUIState state) {
		}
	}
	public class InsertGeoPointMapCommand : InsertItemCommand<GeoPointMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertGeoPointMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertGeoPointMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertGeoPointMapDescription; } }
		public override string ImageName { get { return "InsertGeoPointMap"; } }
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameMapItem; } }
		public InsertGeoPointMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class InsertBubbleMapCommand : InsertItemCommand<BubbleMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertBubbleMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertBubbleMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertBubbleMapDescription; } }
		public override string ImageName { get { return "InsertBubbleMap"; } }
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameMapItem; } }
		public InsertBubbleMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
	public class InsertPieMapCommand : InsertItemCommand<PieMapDashboardItem> {
		public override DashboardCommandId Id { get { return DashboardCommandId.InsertPieMap; } }
		public override DashboardWinStringId MenuCaptionStringId { get { return DashboardWinStringId.CommandInsertPieMapCaption; } }
		public override DashboardWinStringId DescriptionStringId { get { return DashboardWinStringId.CommandInsertPieMapDescription; } }
		public override string ImageName { get { return "InsertPieMap"; } }
		protected override DashboardStringId DefaultNameId { get { return DashboardStringId.DefaultNameMapItem; } }
		public InsertPieMapCommand(DashboardDesigner control)
			: base(control) {
		}
	}
}
