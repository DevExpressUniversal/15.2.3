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
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraScheduler.Commands.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region FileRibbonPage
	public class FileRibbonPage : ControlCommandBasedRibbonPage {
		public FileRibbonPage() {
		}
		public FileRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_PageFile); }
		}
	}
	#endregion
	#region CommonRibbonPageGroup
	public class CommonRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public CommonRibbonPageGroup() {
		}
		public CommonRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupCommon); }
		}
	}
	#endregion
	#region SchedulerCommonBarCreator
	public class SchedulerCommonBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FileRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(CommonRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CommonBar); } }
		public override int DockColumn { get { return 0; } }
		public override int DockRow { get { return 0; } }
		public override Bar CreateBar() {
			return new CommonBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerCommonItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CommonRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FileRibbonPage();
		}
	}
	#endregion
	#region CommonBar
	public class CommonBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public CommonBar() {
		}
		public CommonBar(BarManager manager)
			: base(manager) {
		}
		public CommonBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupCommon); }
		}
	}
	#endregion
	#region SchedulerCommonItemBuilder
	public class SchedulerCommonItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new OpenScheduleItem());
			items.Add(new SaveScheduleItem());
		}
	}
	#endregion
	#region PrintRibbonPageGroup
	public class PrintRibbonPageGroup : SchedulerControlRibbonPageGroup {
		public PrintRibbonPageGroup() {
		}
		public PrintRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupPrint); }
		}
	}
	#endregion
	#region SchedulerPrintBarCreator
	public class SchedulerPrintBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FileRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PrintRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PrintBar); } }
		public override int DockColumn { get { return 1; } }
		public override int DockRow { get { return 0; } }
		public override Bar CreateBar() {
			return new PrintBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new SchedulerPrintItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PrintRibbonPageGroup();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FileRibbonPage();
		}
	}
	#endregion
	#region PrintBar
	public class PrintBar : ControlCommandBasedBar<SchedulerControl, SchedulerCommandId> {
		public PrintBar() {
		}
		public PrintBar(BarManager manager)
			: base(manager) {
		}
		public PrintBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText {
			get { return SchedulerExtensionsLocalizer.GetString(SchedulerExtensionsStringId.Caption_GroupPrint); }
		}
	}
	#endregion
	#region SchedulerPrintItemBuilder
	public class SchedulerPrintItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new PrintPreviewItem());
			items.Add(new PrintItem());
			items.Add(new PrintPageSetupItem());
		}
	}
	#endregion
	#region PrintPreviewItem
	public class PrintPreviewItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public PrintPreviewItem() {
		}
		public PrintPreviewItem(string caption)
			: base(caption) {
		}
		public PrintPreviewItem(BarManager manager)
			: base(manager) {
		}
		public PrintPreviewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.PrintPreview; }
		}
	}
	#endregion
	#region PrintItem
	public class PrintItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public PrintItem() {
		}
		public PrintItem(string caption)
			: base(caption) {
		}
		public PrintItem(BarManager manager)
			: base(manager) {
		}
		public PrintItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.Print; }
		}
	}
	#endregion
	#region PrintPageSetupItem
	public class PrintPageSetupItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public PrintPageSetupItem() {
		}
		public PrintPageSetupItem(string caption)
			: base(caption) {
		}
		public PrintPageSetupItem(BarManager manager)
			: base(manager) {
		}
		public PrintPageSetupItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.PrintPageSetup; }
		}
	}
	#endregion
	#region SaveScheduleItem
	public class SaveScheduleItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public SaveScheduleItem() {
		}
		public SaveScheduleItem(string caption)
			: base(caption) {
		}
		public SaveScheduleItem(BarManager manager)
			: base(manager) {
		}
		public SaveScheduleItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.SaveSchedule; }
		}
	}
	#endregion
	#region OpenScheduleItem
	public class OpenScheduleItem : ControlCommandBarButtonItem<SchedulerControl, SchedulerCommandId> {
		public OpenScheduleItem() {
		}
		public OpenScheduleItem(string caption)
			: base(caption) {
		}
		public OpenScheduleItem(BarManager manager)
			: base(manager) {
		}
		public OpenScheduleItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override SchedulerCommandId CommandId {
			get { return SchedulerCommandId.OpenSchedule; }
		}
	}
	#endregion
}
