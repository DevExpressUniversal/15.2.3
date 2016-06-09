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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetFileItemBuilder
	public class SpreadsheetFileItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileNew));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileOpen));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileSave));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileSaveAs));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileQuickPrint));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FilePrint));
			SpreadsheetCommandBarButtonItem item = SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FilePrintPreview);
			item.SetFocusControlAfterInvokeCommandProperty(false);
			items.Add(item);
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileUndo));
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileRedo));
		}
	}
	#endregion
	#region SpreadsheetFileInfoItemBuilder
	public class SpreadsheetFileInfoItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(SpreadsheetCommandBarButtonItem.Create(SpreadsheetCommandId.FileShowDocumentProperties));
		}
	}
	#endregion
	#region SpreadsheetFileBarCreator
	public class SpreadsheetFileBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FileRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(CommonRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CommonBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new CommonBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFileItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FileRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CommonRibbonPageGroup();
		}
	}
	#endregion
	#region SpreadsheetFileInfoBarCreator
	public class SpreadsheetFileInfoBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FileRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(InfoRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CommonBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new CommonBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SpreadsheetFileInfoItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FileRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new InfoRibbonPageGroup();
		}
	}
	#endregion
	#region CommonBar
	public class CommonBar : ControlCommandBasedBar<SpreadsheetControl, SpreadsheetCommandId> {
		public CommonBar() {
		}
		public CommonBar(BarManager manager)
			: base(manager) {
		}
		public CommonBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCommon); } }
	}
	#endregion
	#region FileRibbonPage
	public class FileRibbonPage : ControlCommandBasedRibbonPage {
		public FileRibbonPage() {
		}
		public FileRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PageFile); } }
	}
	#endregion
	#region CommonRibbonPageGroup
	public class CommonRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public CommonRibbonPageGroup() {
		}
		public CommonRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupCommon); } }
	}
	#endregion
	#region InfoRibbonPageGroup
	public class InfoRibbonPageGroup : SpreadsheetControlRibbonPageGroup {
		public InfoRibbonPageGroup() {
		}
		public InfoRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_GroupInfo); } }
	}
	#endregion
}
