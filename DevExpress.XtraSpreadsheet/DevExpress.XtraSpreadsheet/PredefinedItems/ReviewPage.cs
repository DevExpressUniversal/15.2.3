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
	#region SpreadsheetCommentsItemBuilder
	public class SpreadsheetCommentsItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewInsertComment));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewEditComment));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewDeleteComment));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewShowHideComment, RibbonItemStyles.SmallWithText));
		}
	}
	#endregion
	#region SpreadsheetChangesItemBuilder
	public class SpreadsheetChangesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewProtectSheet));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewUnprotectSheet));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewProtectWorkbook));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewUnprotectWorkbook));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.ReviewShowProtectedRangeManager));
		}
	}
	#endregion
	#region SpreadsheetCommentsBarCreator
	public class SpreadsheetCommentsBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(CommentsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CommentsBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new CommentsBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetCommentsItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CommentsRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetChangesBarCreator
	public class SpreadsheetChangesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(ReviewRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChangesRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ChangesBar); } }
		public override int DockRow { get { return 2; } }
		public override int DockColumn { get { return 4; } }
		public override Bar CreateBar() {
			return new ChangesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetChangesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new ReviewRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChangesRibbonPageGroup();
		}
	}
	#endregion
	#region CommentsBar
	public class CommentsBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public CommentsBar() {
		}
		public CommentsBar(BarManager manager)
			: base(manager) {
		}
		public CommentsBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupComments); } }
	}
	#endregion
	#region ChangesBar
	public class ChangesBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public ChangesBar() {
		}
		public ChangesBar(BarManager manager)
			: base(manager) {
		}
		public ChangesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChanges); } }
	}
	#endregion
	#region ReviewRibbonPage
	public class ReviewRibbonPage : ControlCommandBasedRibbonPage {
		public ReviewRibbonPage() {
		}
		public ReviewRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageReview); } }
	}
	#endregion
	#region CommentsRibbonPageGroup
	public class CommentsRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public CommentsRibbonPageGroup() {
		}
		public CommentsRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupComments); } }
	}
	#endregion
	#region ChangesRibbonPageGroup
	public class ChangesRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public ChangesRibbonPageGroup() {
		}
		public ChangesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupChanges); } }
	}
	#endregion
}
