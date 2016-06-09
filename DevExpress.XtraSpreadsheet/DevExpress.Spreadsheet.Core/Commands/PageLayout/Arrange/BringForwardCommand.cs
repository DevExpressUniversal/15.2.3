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
using DevExpress.Office.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ArrangeBringForwardCommand
	public class ArrangeBringForwardCommand : SpreadsheetDrawingZOrderCommandBase {
		public ArrangeBringForwardCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ArrangeBringForward; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_FloatingObjectBringForward; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_FloatingObjectBringForwardDescription; } }
		public override string ImageName { get { return "FloatingObjectBringForward"; } }
		#endregion
		protected internal override void ModifyDrawing(IDrawingObject drawing) {
			ActiveSheet.DrawingObjectsByZOrderCollections.BringForward(drawing);
		}
	}
	#endregion
	#region SpreadsheetDrawingZOrderCommandBase (abstract class)
	public abstract class SpreadsheetDrawingZOrderCommandBase : SpreadsheetDrawingCommand {
		protected SpreadsheetDrawingZOrderCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override DocumentCapability Capability { get { return DocumentModel.BehaviorOptions.Drawing.ChangeZOrder; } }
	}
	#endregion
	#region SpreadsheetDrawingResizeCommandBase (abstract class)
	public abstract class SpreadsheetDrawingResizeCommandBase : SpreadsheetDrawingCommand {
		protected SpreadsheetDrawingResizeCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override DocumentCapability Capability { get { return DocumentModel.BehaviorOptions.Drawing.Resize; } }
	}
	#endregion
	#region SpreadsheetDrawingMoveCommandBase (abstract class)
	public abstract class SpreadsheetDrawingMoveCommandBase : SpreadsheetDrawingCommand {
		protected SpreadsheetDrawingMoveCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override DocumentCapability Capability { get { return DocumentModel.BehaviorOptions.Drawing.Move; } }
	}
	#endregion
	#region SpreadsheetDrawingRotateCommandBase (abstract class)
	public abstract class SpreadsheetDrawingRotateCommandBase : SpreadsheetDrawingCommand {
		protected SpreadsheetDrawingRotateCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override DocumentCapability Capability { get { return DocumentModel.BehaviorOptions.Drawing.Rotate; } }
	}
	#endregion
}
