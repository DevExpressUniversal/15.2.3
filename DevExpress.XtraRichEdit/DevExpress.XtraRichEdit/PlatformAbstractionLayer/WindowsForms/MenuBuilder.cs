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
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using System.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Menu {
	#region WinFormsRichEditMenuBuilderUIFactory
	public class WinFormsRichEditMenuBuilderUIFactory : IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> {
		#region IMenuBuilderUIFactory<RichEditCommand,RichEditCommandId> Members
		public IDXMenuCheckItemCommandAdapter<RichEditCommandId> CreateMenuCheckItemAdapter(RichEditCommand command) {
			return new RichEditMenuCheckItemCommandWinAdapter(command);
		}
		public IDXMenuItemCommandAdapter<RichEditCommandId> CreateMenuItemAdapter(RichEditCommand command) {
			return new RichEditMenuItemCommandWinAdapter(command);
		}
		public IDXPopupMenu<RichEditCommandId> CreatePopupMenu() {
			return new RichEditPopupMenu();
		}
		public IDXPopupMenu<RichEditCommandId> CreateSubMenu() {
			return new RichEditPopupMenu();
		}
		#endregion
	}
	#endregion
	#region WinFormsRichEditContentMenuBuilder
	public class WinFormsRichEditContentMenuBuilder : RichEditContentMenuBuilder {
		public WinFormsRichEditContentMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		protected internal override RichEditHitTestResult CalculateCursorHitTestResult() {
			RichEditControl control = (RichEditControl)Control;
			Point cursorPosition = control.PointToClient(System.Windows.Forms.Cursor.Position);
			Point physicalPoint = control.GetPhysicalPoint(cursorPosition);
			return Control.InnerControl.ActiveView.CalculateHitTest(physicalPoint, DocumentLayoutDetailsLevel.Box);
		}
	}
	#endregion
	#region WinFormsRichEditContentRadialMenuBuilder
	public class WinFormsRichEditContentRadialMenuBuilder : WinFormsRichEditContentMenuBuilder {
		public WinFormsRichEditContentRadialMenuBuilder(IRichEditControl control, IMenuBuilderUIFactory<RichEditCommand, RichEditCommandId> uiFactory)
			: base(control, uiFactory) {
		}
		protected override void AddClipboardMenuItems(IDXPopupMenu<RichEditCommandId> menu, InnerRichEditControl innerControl) {
			IDXPopupMenu<RichEditCommandId> subMenu = UiFactory.CreateSubMenu();
			subMenu.Caption = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_ClipboardSubItem);
			base.AddClipboardMenuItems(subMenu, innerControl);
			AppendSubmenu(menu, subMenu, false);
		}
		protected override RichEditTableCellsAlignmentSubmenuBuilder CreateTableCellsAlignmentSubmenuBuilder() {
			return new RichEditTableCellsAlignmentRadialMenuSubmenuBuilder(Control, UiFactory);
		}
	}
	#endregion
}
