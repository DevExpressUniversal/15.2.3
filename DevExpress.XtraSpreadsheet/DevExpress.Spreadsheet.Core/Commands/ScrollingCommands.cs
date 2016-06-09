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
using DevExpress.Utils.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Office.Internal;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ScrollByPhysicalOffsetCommandBase (abstract class)
	public abstract class ScrollByPhysicalOffsetCommandBase : SpreadsheetMenuItemSimpleCommand {
		#region Fields
		int physicalOffset;
		bool executedSuccessfully;
		#endregion
		protected ScrollByPhysicalOffsetCommandBase(ISpreadsheetControl control)
			: base(control) {
			this.physicalOffset = DocumentModel.LayoutUnitConverter.DocumentsToLayoutUnits(50);
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public int PhysicalOffset { get { return physicalOffset; } set { physicalOffset = value; } }
		public bool ExecutedSuccessfully { get { return executedSuccessfully; } }
		#endregion
		protected internal override void ExecuteCore() {
			int offset = CalculateScrollOffset();
			if (offset == 0)
				return;
			PerformScroll(GetScrollBarAdapter(), offset);
			OnScroll(offset);
		}
		protected internal virtual void PerformScroll(IScrollBarAdapter scrollBarAdapter, int offset) {
			long previousValue = scrollBarAdapter.Value;
			PerformScrollCore(offset);
			this.executedSuccessfully = (previousValue != scrollBarAdapter.Value);
		}
		protected internal int CalculateScrollOffset() {
			int offset = CalculateScrollOffsetCore(GetPageGrid(), GetGridItemsCount(), Math.Abs(PhysicalOffset));
			return PhysicalOffset < 0 ? offset : -offset;
		}
		protected internal int CalculateScrollOffsetCore(PageGrid grid, int itemsCount, int offset) {
			int totalExtent = 0;
			for (int i = 0; i < itemsCount; i++) {
				totalExtent += grid[i].Extent;
				if (offset == totalExtent || itemsCount == 1)
					return i + 1;
				if (offset > totalExtent)
					continue;
				if (offset < totalExtent) {
					if (i > 0) {
						int halfColumnWidth = grid[i].Extent / 2;
						return (offset < totalExtent - halfColumnWidth) ? i : i + 1;
					}
					else
						return 0;
				}
			}
			return 0;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
		protected internal abstract IScrollBarAdapter GetScrollBarAdapter();
		protected internal abstract void PerformScrollCore(int offset);
		protected internal abstract int GetGridItemsCount();
		protected internal abstract PageGrid GetPageGrid();
		protected internal abstract void OnScroll(int offset);
	}
	#endregion
	#region ScrollHorizontallyByPhysicalOffsetCommand
	public class ScrollHorizontallyByPhysicalOffsetCommand : ScrollByPhysicalOffsetCommandBase {
		public ScrollHorizontallyByPhysicalOffsetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override IScrollBarAdapter GetScrollBarAdapter() {
			return ActiveView.HorizontalScrollController.ScrollBarAdapter;
		}
		protected internal override void PerformScrollCore(int offset) {
			ActiveView.ScrollLineLeftRight(offset);
		}
		protected internal override int GetGridItemsCount() {
			ScrollInfo scrollInfo = ActiveView.DocumentLayout.ScrollInfo;
			return scrollInfo.ScrollRightColumnModelIndex - scrollInfo.ScrollLeftColumnModelIndex + 1;
		}
		protected internal override PageGrid GetPageGrid() {
			IList<Page> pages = ActiveView.DocumentLayout.Pages;
			return pages[pages.Count - 1].GridColumns;
		}
		protected internal override void OnScroll(int offset) {
			ActiveView.OnHorizontalScroll(offset);
		}
	}
	#endregion
	#region ScrollVerticallyByPhysicalOffsetCommand
	public class ScrollVerticallyByPhysicalOffsetCommand : ScrollByPhysicalOffsetCommandBase {
		public ScrollVerticallyByPhysicalOffsetCommand(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override IScrollBarAdapter GetScrollBarAdapter() {
			return ActiveView.VerticalScrollController.ScrollBarAdapter;
		}
		protected internal override void PerformScrollCore(int offset) {
			ActiveView.ScrollLineUpDown(offset);
		}
		protected internal override int GetGridItemsCount() {
			ScrollInfo scrollInfo = ActiveView.DocumentLayout.ScrollInfo;
			return scrollInfo.ScrollBottomRowModelIndex - scrollInfo.ScrollTopRowModelIndex + 1;
		}
		protected internal override PageGrid GetPageGrid() {
			IList<Page> pages = ActiveView.DocumentLayout.Pages;
			return pages[pages.Count - 1].GridRows;
		}
		protected internal override void OnScroll(int offset) {
			ActiveView.OnVerticalScroll(offset);
		}
	}
	#endregion
}
