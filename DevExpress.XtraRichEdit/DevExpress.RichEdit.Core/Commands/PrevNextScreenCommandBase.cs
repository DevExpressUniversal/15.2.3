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
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Commands {
	#region PrevNextScreenCommandBase (abstract class)
	public abstract class PrevNextScreenCommandBase : RichEditSelectionCommand {
		protected PrevNextScreenCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool TryToKeepCaretX { get { return true; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Character; } }
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return pos.PieceTable.IsMain;
		}
		protected internal override void ChangeSelection(Selection selection) {
			Point physicalPoint = ActiveView.CreatePhysicalPoint(CaretPosition.PageViewInfo, CaretPosition.CalculateCaretBounds().Location);
			if (ScrollScreen()) {
				DocumentLogPosition currentLogPosition = selection.End;
				PlaceCaretToPhysicalPointCommand command = CreatePlaceCaretToPhysicalPointCommand();
				command.UpdateCaretX = false;
				command.PhysicalPoint = physicalPoint;
				command.Execute();
				if (ShouldCalculateNewCaretLogPosition(currentLogPosition, selection.End)) {
					DocumentLogPosition pos = CalculateCaretNewLogPosition(physicalPoint);
					if (pos != DocumentLogPosition.MaxValue) {
						if (!IsNewPositionInCorrectDirection(currentLogPosition, pos)) {
							if (!UpdateCaretPosition(DocumentLayoutDetailsLevel.Row))
								return;
							NextCaretPositionVerticalDirectionCalculator calculator = CreateCaretPositionCalculator();
							DocumentModelPosition newPosition = calculator.CalculateNextPosition(CaretPosition);
							if (!Object.ReferenceEquals(newPosition, null))
								pos = newPosition.LogPosition;
						}
						selection.End = pos;
						if (!ExtendSelection)
							selection.Start = pos;
					}
				}
			}
			else {
				base.ChangeSelection(selection);
			}
		}
		protected internal virtual PlaceCaretToPhysicalPointCommand CreatePlaceCaretToPhysicalPointCommand() {
			return new PlaceCaretToPhysicalPointCommand(Control);
		}
		protected internal virtual DocumentLogPosition CalculateCaretNewLogPosition(Point physicalPoint) {
			PageViewInfoRow pageViewInfoRow = ActiveView.PageViewInfoGenerator.GetPageRowAtPoint(physicalPoint, true);
			if (pageViewInfoRow == null)
				return DocumentLogPosition.MaxValue;
			PageViewInfo pageViewInfo = pageViewInfoRow.GetPageAtPoint(physicalPoint, false);
			if (pageViewInfo == null)
				return DocumentLogPosition.MaxValue;
			Point logicalPoint = ActiveView.CreateLogicalPoint(pageViewInfo.ClientBounds, physicalPoint);
			int x = logicalPoint.X;
			NextCaretPositionVerticalDirectionCalculator calculator = CreateCaretPositionCalculator();
			Point logicalCaretPoint = ActiveView.CreateLogicalPoint(pageViewInfo.ClientBounds, physicalPoint);
			Column column = calculator.GetTargetColumn(pageViewInfo.Page, logicalCaretPoint.X);
			if (ShouldUseAnotherPage(column, logicalCaretPoint)) {
				Row targetRow = calculator.ObtainTargetRowFromCurrentPageViewInfoAndCaretX(pageViewInfo, null, ref x);
				if (targetRow != null) {
					CharacterBox character = calculator.GetTargetCharacter(targetRow, x);
					return character.GetFirstPosition(ActivePieceTable).LogPosition;
				}
				else
					return GetDefaultLogPosition();
			}
			else {
				Row targetRow = GetTargetRowAtColumn(column);
				CharacterBox character = calculator.GetTargetCharacter(targetRow, x);
				return character.GetFirstPosition(ActivePieceTable).LogPosition;
			}
		}
		protected internal abstract bool ScrollScreen();
		protected internal abstract NextCaretPositionVerticalDirectionCalculator CreateCaretPositionCalculator();
		protected internal abstract bool ShouldUseAnotherPage(Column column, Point logicalCaretPoint);
		protected internal abstract Row GetTargetRowAtColumn(Column column);
		protected internal abstract DocumentLogPosition GetDefaultLogPosition();
		protected internal abstract bool ShouldCalculateNewCaretLogPosition(DocumentLogPosition previousLogPosition, DocumentLogPosition currentLogPosition);
		protected internal abstract bool IsNewPositionInCorrectDirection(DocumentLogPosition previousLogPosition, DocumentLogPosition currentLogPosition);
	}
	#endregion
}
