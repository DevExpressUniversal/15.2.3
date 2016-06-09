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
	#region PrevNextPageCommandBase (abstract class)
	public abstract class PrevNextPageCommandBase : RichEditSelectionCommand {
		protected PrevNextPageCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.Page; } }
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected internal override RichEditCommand CreateEnsureCaretVisibleVerticallyCommand() {
			EnsureCaretVisibleVerticallyForMovePrevNextPageCommand result = new EnsureCaretVisibleVerticallyForMovePrevNextPageCommand(Control);
			result.PhysicalOffsetAboveTargetRow = (int)Math.Round(DocumentModel.LayoutUnitConverter.PointsToFontUnitsF(72) / (float)ActiveView.ZoomFactor);
			return result;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region EnsureCaretVisibleVerticallyForMovePrevNextPageCommand
	public class EnsureCaretVisibleVerticallyForMovePrevNextPageCommand : EnsureCaretVisibleVerticallyCommand {
		int physicalOffsetAboveTargetRow;
		public EnsureCaretVisibleVerticallyForMovePrevNextPageCommand(IRichEditControl control)
			: base(control) {
		}
		public int PhysicalOffsetAboveTargetRow { get { return physicalOffsetAboveTargetRow; } set { physicalOffsetAboveTargetRow = value; } }
		protected internal override Rectangle CalculateTargetRowLogicalBounds(PageViewInfo pageViewInfo, CurrentRowInfo row) {
			Rectangle logicalRowBounds = ValidateRowBounds(pageViewInfo, row.BoundsRelativeToPage);
			Rectangle topmostRowBounds = ValidateRowBounds(pageViewInfo, CalculateTopmostRow(pageViewInfo.Page).Bounds);
			logicalRowBounds.Y -= (int)Math.Round(PhysicalOffsetAboveTargetRow * ActiveView.ZoomFactor);
			logicalRowBounds.Y = Math.Max(0, logicalRowBounds.Y);
			if (topmostRowBounds.Y < logicalRowBounds.Y)
				return topmostRowBounds;
			else
				return logicalRowBounds;
		}
		protected internal override int CalculatePhysicalOffsetForRowBoundsVisibility(Rectangle viewBounds, Rectangle physicalRowBounds) {
			if (viewBounds.Y <= physicalRowBounds.Y && physicalRowBounds.Bottom <= viewBounds.Bottom)
				return 0;
			return physicalRowBounds.Y - viewBounds.Y;
		}
		protected internal virtual Row CalculateTopmostRow(Page page) {
			ColumnCollection columns = page.Areas.First.Columns;
			Row result = columns[0].Rows.First;
			int count = columns.Count;
			for (int i = 1; i < count; i++) {
				Row row = columns[i].Rows.First;
				if (row.Bounds.Y < result.Bounds.Y)
					result = row;
			}
			return result;
		}
	}
	#endregion
}
