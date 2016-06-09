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
using System.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Commands {
	#region NextScreenCommand
	public class NextScreenCommand : PrevNextScreenCommandBase {
		public NextScreenCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextScreenCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextScreen; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return true; } }
		protected internal override bool ExtendSelection { get { return false; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextScreenCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveScreenDown; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextScreenCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveScreenDownDescription; } }
		#endregion
		protected internal override bool ScrollScreen() {
			if (ActiveView.VerticalScrollController.ScrollPageDown()) {
				ActiveView.OnVerticalScroll();
				return true;
			}
			else
				return false;
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			return ActivePieceTable.DocumentEndLogPosition;
		}
		protected internal override NextCaretPositionVerticalDirectionCalculator CreateCaretPositionCalculator() {
			return new NextCaretPositionLineDownCalculator(Control);
		}
		protected internal override bool ShouldUseAnotherPage(Column column, Point logicalCaretPoint) {
			return column.Rows.Last.Bounds.Top < logicalCaretPoint.Y;
		}
		protected internal override Row GetTargetRowAtColumn(Column column) {
			return column.Rows.First;
		}
		protected internal override DocumentLogPosition GetDefaultLogPosition() {
			return ActivePieceTable.DocumentEndLogPosition;
		}
		protected internal override bool ShouldCalculateNewCaretLogPosition(DocumentLogPosition previousLogPosition, DocumentLogPosition currentLogPosition) {
			return previousLogPosition == currentLogPosition;
		}
		protected internal override bool IsNewPositionInCorrectDirection(DocumentLogPosition previousLogPosition, DocumentLogPosition currentLogPosition) {
			return previousLogPosition < currentLogPosition;
		}
	}
	#endregion
	#region ExtendNextScreenCommand
	public class ExtendNextScreenCommand : NextScreenCommand {
		public ExtendNextScreenCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendNextScreenCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendNextScreen; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override PlaceCaretToPhysicalPointCommand CreatePlaceCaretToPhysicalPointCommand() {
			return new ExtendSelectionToPhysicalPointCommand(Control);
		}
	}
	#endregion
}
