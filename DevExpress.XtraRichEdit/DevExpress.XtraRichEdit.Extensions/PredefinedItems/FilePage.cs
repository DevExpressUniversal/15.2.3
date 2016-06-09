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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditFileItemBuilder
	public class RichEditFileItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new UndoItem());
			items.Add(new RedoItem());
			PopulateFileOperationItems(items);
			items.Add(new QuickPrintItem());
			items.Add(new PrintItem());
			items.Add(new PrintPreviewItem());
		}
		protected virtual void PopulateFileOperationItems(List<BarItem> items) {
			items.Add(new FileNewItem());
			items.Add(new FileOpenItem());
			items.Add(new FileSaveItem());
			items.Add(new FileSaveAsItem());
		}
	}
	#endregion
	#region FileNewItem
	public class FileNewItem: RichEditCommandBarButtonItem {
		public FileNewItem() {
		}
		public FileNewItem(BarManager manager)
			: base(manager) {
		}
		public FileNewItem(string caption)
			: base(caption) {
		}
		public FileNewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FileNew; } }
	}
	#endregion
	#region FileOpenItem
	public class FileOpenItem: RichEditCommandBarButtonItem {
		public FileOpenItem() {
		}
		public FileOpenItem(BarManager manager)
			: base(manager) {
		}
		public FileOpenItem(string caption)
			: base(caption) {
		}
		public FileOpenItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FileOpen; } }
	}
	#endregion
	#region FileSaveItem
	public class FileSaveItem: RichEditCommandBarButtonItem {
		public FileSaveItem() {
		}
		public FileSaveItem(BarManager manager)
			: base(manager) {
		}
		public FileSaveItem(string caption)
			: base(caption) {
		}
		public FileSaveItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FileSave; } }
	}
	#endregion
	#region FileSaveAsItem
	public class FileSaveAsItem: RichEditCommandBarButtonItem {
		public FileSaveAsItem() {
		}
		public FileSaveAsItem(BarManager manager)
			: base(manager) {
		}
		public FileSaveAsItem(string caption)
			: base(caption) {
		}
		public FileSaveAsItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.FileSaveAs; } }
	}
	#endregion
	#region PrintItem
	public class PrintItem: RichEditCommandBarButtonItem {
		public PrintItem() {
		}
		public PrintItem(BarManager manager)
			: base(manager) {
		}
		public PrintItem(string caption)
			: base(caption) {
		}
		public PrintItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.Print; } }
	}
	#endregion
	#region QuickPrintItem
	public class QuickPrintItem: RichEditCommandBarButtonItem {
		public QuickPrintItem() {
		}
		public QuickPrintItem(BarManager manager)
			: base(manager) {
		}
		public QuickPrintItem(string caption)
			: base(caption) {
		}
		public QuickPrintItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.QuickPrint; } }
	}
	#endregion
	#region PrintPreviewItem
	public class PrintPreviewItem: RichEditCommandBarButtonItem {
		public PrintPreviewItem() {
		}
		public PrintPreviewItem(BarManager manager)
			: base(manager) {
		}
		public PrintPreviewItem(string caption)
			: base(caption) {
		}
		public PrintPreviewItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.PrintPreview; } }
		protected override bool ShouldSetControlFocusAfterInvokeCommand { get { return false; } }
	}
	#endregion
	#region UndoRedoItemBase (abstract class)
	public abstract class UndoRedoItemBase: RichEditCommandBarButtonItem {
		protected UndoRedoItemBase() {
		}
		protected UndoRedoItemBase(BarManager manager)
			: base(manager) {
		}
		protected UndoRedoItemBase(string caption)
			: base(caption) {
		}
		protected UndoRedoItemBase(BarManager manager, string caption)
			: base(manager, caption) {
		}
	}
	#endregion
	#region UndoItem
	public class UndoItem : UndoRedoItemBase {
		public UndoItem() {
		}
		public UndoItem(BarManager manager)
			: base(manager) {
		}
		public UndoItem(string caption)
			: base(caption) {
		}
		public UndoItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.Undo; } }
	}
	#endregion
	#region RedoItem
	public class RedoItem : UndoRedoItemBase {
		public RedoItem() {
		}
		public RedoItem(BarManager manager)
			: base(manager) {
		}
		public RedoItem(string caption)
			: base(caption) {
		}
		public RedoItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override RichEditCommandId CommandId { get { return RichEditCommandId.Redo; } }
	}
	#endregion
	#region RichEditFileBarCreator
	public class RichEditFileBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(FileRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(CommonRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(CommonBar); } }
		public override int DockRow { get { return 0; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new CommonBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RichEditFileItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new FileRibbonPage();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new CommonRibbonPageGroup();
		}
	}
	#endregion
	#region CommonBar
	public class CommonBar : ControlCommandBasedBar<RichEditControl, RichEditCommandId> {
		public CommonBar() {
		}
		public CommonBar(BarManager manager)
			: base(manager) {
		}
		public CommonBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupCommon); } }
	}
	#endregion
	#region FileRibbonPage
	public class FileRibbonPage : ControlCommandBasedRibbonPage {
		public FileRibbonPage() {
		}
		public FileRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_PageFile); } }
		protected override RibbonPage CreatePage() {
			return new FileRibbonPage();
		}
	}
	#endregion
	#region CommonRibbonPageGroup
	public class CommonRibbonPageGroup : RichEditControlRibbonPageGroup {
		public CommonRibbonPageGroup() {
		}
		public CommonRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return RichEditExtensionsLocalizer.GetString(RichEditExtensionsStringId.Caption_GroupCommon); } }
	}
	#endregion
}
