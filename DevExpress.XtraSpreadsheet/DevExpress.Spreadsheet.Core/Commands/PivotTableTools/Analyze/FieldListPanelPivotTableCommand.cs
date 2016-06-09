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

using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region FieldListPanelPivotTableCommand
	public class FieldListPanelPivotTableCommand : PivotTableCommandBase {
		public FieldListPanelPivotTableCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FieldListPanelPivotTable; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FieldListPanelPivotTable; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_FieldListPanelPivotTableDescription; } }
		public override string ImageName { get { return "FieldListPanelPivotTable"; } }
		#endregion
		protected internal override void ExecuteCore() {
			WorkbookProperties properties = DocumentModel.Properties;
			properties.HidePivotFieldList = !properties.HidePivotFieldList;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			state.Checked = !DocumentModel.Properties.HidePivotFieldList;
		}
		protected override bool GetEnabled(PivotTable table) {
			bool enabled = table == null ? true : !table.DisableFieldList;
			return base.GetEnabled(table) && enabled;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region ShowHideFieldListPanelPivotTableContextMenuItemCommand
	public class ShowHideFieldListPanelPivotTableContextMenuItemCommand : FieldListPanelPivotTableCommand {
		public ShowHideFieldListPanelPivotTableContextMenuItemCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FieldListPanelPivotTableContextMenuItem; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override string MenuCaption { 
			get {
				return (DocumentModel.Properties.HidePivotFieldList)
					? XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_ShowFieldListPanelPivotTableContextMenuItem)
					: XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_HideFieldListPanelPivotTableContextMenuItem);
			}
		}
		public override string ImageName { get { return "FieldListPanelPivotTable"; } }
		#endregion
	}
	#endregion
	#region ShowHidePivotTablePanelCommand
	public class ShowHidePivotTablePanelCommand : PivotTableCommandBase {
		public ShowHidePivotTablePanelCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.None; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_None; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (!InnerControl.AllowShowingForms)
				return;
			if (DocumentModel.Properties.HidePivotFieldList) {
				InnerControl.HidePivotTableFieldsPanel();
				return;
			}
			PivotTable table = TryGetPivotTable();
			if (table != null && !table.DisableFieldList && DocumentServer.IsEditable)
				InnerControl.ShowPivotTableFieldsPanel(new FieldListPanelPivotTableViewModel(table, Control.InnerControl.ErrorHandler));
			else
				InnerControl.HidePivotTableFieldsPanel();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandsRestriction(state, DocumentCapability.Default);
			ApplyActiveSheetProtection(state);
		}
	}
	#endregion
}
