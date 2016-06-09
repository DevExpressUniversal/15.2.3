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
using DevExpress.XtraBars;
using System.Collections.Generic;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetPictureFormatArrangeItemBuilder
	public class SpreadsheetPictureFormatArrangeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			SpreadsheetArrangeItemBuilder.PopulateArrangeItems(items);
		}
	}
	#endregion
	#region SpreadsheetPictureFormatArrangeBarCreator
	public class SpreadsheetPictureFormatArrangeBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(PictureFormatRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PictureFormatArrangeRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PictureToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PictureFormatArrangeBar); } }
		public override int DockRow { get { return 3; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new PictureFormatArrangeBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetPictureFormatArrangeItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new PictureFormatRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PictureFormatArrangeRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PictureToolsRibbonPageCategory();
		}
	}
	#endregion
	#region PictureFormatArrangeBar
	public class PictureFormatArrangeBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public PictureFormatArrangeBar() {
		}
		public PictureFormatArrangeBar(BarManager manager)
			: base(manager) {
		}
		public PictureFormatArrangeBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange); } }
	}
	#endregion
	#region PictureFormatRibbonPage
	public class PictureFormatRibbonPage : ControlCommandBasedRibbonPage {
		public PictureFormatRibbonPage() {
		}
		public PictureFormatRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFormat); } }
	}
	#endregion
	#region PictureFormatArrangeRibbonPageGroup
	public class PictureFormatArrangeRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public PictureFormatArrangeRibbonPageGroup() {
		}
		public PictureFormatArrangeRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupArrange); } }
	}
	#endregion
	#region PictureToolsRibbonPageCategory
	public class PictureToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<SpreadsheetControl, SpreadsheetCommandId> {
		public PictureToolsRibbonPageCategory() {
			this.Visible = false;
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageCategoryPictureTools); } }
		protected override SpreadsheetCommandId EmptyCommandId { get { return SpreadsheetCommandId.None; } }
		public override SpreadsheetCommandId CommandId { get { return SpreadsheetCommandId.ToolsPictureCommandGroup; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new PictureToolsRibbonPageCategory();
		}
	}
	#endregion
}
