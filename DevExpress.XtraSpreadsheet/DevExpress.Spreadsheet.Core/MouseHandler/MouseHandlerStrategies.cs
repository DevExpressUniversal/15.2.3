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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using LayoutPictureBox = DevExpress.XtraSpreadsheet.Layout.PictureBox;
using LayoutDrawingBox = DevExpress.XtraSpreadsheet.Layout.DrawingBox;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region SpreadsheetMouseHandlerStrategyFactory (abstract class)
	public abstract class SpreadsheetMouseHandlerStrategyFactory {
		public abstract SpreadsheetMouseHandlerStrategy CreateMouseHandlerStrategy(SpreadsheetMouseHandler mouseHandler);
		public abstract DragFloatingObjectManuallyMouseHandlerStateStrategy CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state);
		public abstract ResizeRowMouseHandlerStateStrategy CreateResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state);
		public abstract ResizeColumnMouseHandlerStateStrategy CreateResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state);
		public abstract SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy CreateSpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state);
		public abstract DragRangeManuallyMouseHandlerStateStrategy CreateDragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state);
		public abstract CommentMouseHandlerStateStrategy CreateCommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state);
	}
	#endregion
	#region SpreadsheetMouseHandlerStrategyBase (abstract class)
	public abstract class SpreadsheetMouseHandlerStrategyBase {
		readonly SpreadsheetMouseHandler mouseHandler;
		protected SpreadsheetMouseHandlerStrategyBase(SpreadsheetMouseHandler mouseHandler) {
			this.mouseHandler = mouseHandler;
		}
		public SpreadsheetMouseHandler MouseHandler { get { return mouseHandler; } }
		public ISpreadsheetControl Control { get { return MouseHandler.Control; } }
	}
	#endregion
	#region SpreadsheetMouseHandlerStrategy (abstract class)
	public abstract class SpreadsheetMouseHandlerStrategy : SpreadsheetMouseHandlerStrategyBase {
		protected SpreadsheetMouseHandlerStrategy(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected virtual IOfficeScroller OfficeScroller { get { return MouseHandler.OfficeScroller; } }
		protected internal abstract void StartOfficeScroller(Point clientPoint);
		protected internal abstract IOfficeScroller CreateOfficeScroller();
		protected internal abstract MouseEventArgs CreateFakeMouseMoveEventArgs();
		protected internal abstract MouseEventArgs ConvertMouseEventArgs(MouseEventArgs screenMouseEventArgs);
		protected internal abstract DragEventArgs ConvertDragEventArgs(DragEventArgs screenDragEventArgs);
		protected internal abstract void AutoScrollerOnDragOver(Point pt);
		protected internal abstract void OnMouseUp(MouseEventArgs e);
	}
	#endregion
	#region SpreadsheetMouseHandlerStateStrategyBase<T> (abstract class)
	public abstract class SpreadsheetMouseHandlerStateStrategyBase<T> : SpreadsheetMouseHandlerStrategyBase where T : SpreadsheetMouseHandlerState {
		readonly T state;
		protected SpreadsheetMouseHandlerStateStrategyBase(T state)
			: base(state.MouseHandler) {
			Guard.ArgumentNotNull(state, "state");
			this.state = state;
		}
		#region Properties
		protected T State { get { return state; } }
		protected DocumentModel DocumentModel { get { return State.DocumentModel; } }
		#endregion
	}
	#endregion
	#region DragFloatingObjectManuallyMouseHandlerStateStrategy (abstract class)
	public abstract class DragFloatingObjectManuallyMouseHandlerStateStrategy : SpreadsheetMouseHandlerStateStrategyBase<DragFloatingObjectManuallyMouseHandlerState> {
		protected DragFloatingObjectManuallyMouseHandlerStateStrategy(DragFloatingObjectManuallyMouseHandlerState state)
			: base(state) {
		}
		protected Rectangle InitialShapeBounds { get { return State.InitialShapeBounds; } }
		protected Rectangle InitialContentBounds { get { return State.InitialContentBounds; } }
		protected LayoutDrawingBox Box { get { return State.Box; } }
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void EndVisualFeedback();
		protected internal abstract void ShowVisualFeedback(Rectangle bounds, Page page, OfficeImage image);
		protected internal abstract void HideVisualFeedback(Rectangle bounds, Page page);
	}
	#endregion
	#region DragRangeManuallyMouseHandlerStateStrategy (abstract class)
	public abstract class DragRangeManuallyMouseHandlerStateStrategy : SpreadsheetMouseHandlerStateStrategyBase<DragRangeManuallyMouseHandlerStateBase> {
		protected DragRangeManuallyMouseHandlerStateStrategy(DragRangeManuallyMouseHandlerStateBase state)
			: base(state) {
		}
		protected internal abstract void ShowVisualFeedback(Rectangle bounds, Page page);
		protected internal abstract void HideVisualFeedback();
	}
	#endregion
	#region ResizeMouseHandlerStateStrategy (public abstract)
	public abstract class ResizeMouseHandlerStateStrategy : SpreadsheetMouseHandlerStateStrategyBase<ContinueResizeMouseHandlerState> {
		protected ResizeMouseHandlerStateStrategy(ContinueResizeMouseHandlerState state)
			: base(state) {
		}
		protected internal abstract void DrawReversibleLine(int coordinate);
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void EndVisualFeedback();
		protected internal abstract void ShowVisualFeedback();
		protected internal abstract void HideVisualFeedback();
	}
	#endregion
	#region ResizeRowMouseHandlerStateStrategy (abstract class)
	public abstract class ResizeRowMouseHandlerStateStrategy : ResizeMouseHandlerStateStrategy {
		protected ResizeRowMouseHandlerStateStrategy(ContinueResizeRowsMouseHandlerState state)
			: base(state) {
		}
	}
	#endregion
	#region ResizeColumnMouseHandlerStateStrategy (abstract class)
	public abstract class ResizeColumnMouseHandlerStateStrategy : ResizeMouseHandlerStateStrategy {
		protected ResizeColumnMouseHandlerStateStrategy(ContinueResizeColumnsMouseHandlerState state)
			: base(state) {
		}
	}
	#endregion
	#region SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy
	public abstract class SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy : SpreadsheetMouseHandlerStateStrategyBase<SpreadsheetRectangularObjectResizeMouseHandlerState> {
		protected SpreadsheetRectangularObjectResizeMouseHandlerStateStrategy(SpreadsheetRectangularObjectResizeMouseHandlerState state)
			: base(state) {
		}
		protected Rectangle InitialShapeBounds { get { return State.InitialShapeBounds; } }
		protected Rectangle InitialContentBounds { get { return State.InitialContentBounds; } }
		protected LayoutDrawingBox Box { get { return State.Box; } }
		protected internal abstract void BeginVisualFeedback();
		protected internal abstract void EndVisualFeedback();
		protected internal abstract void ShowVisualFeedback(Rectangle bounds, Page page, OfficeImage image);
		protected internal abstract void HideVisualFeedback();
	}
	#endregion
	#region CommentMouseHandlerStateStrategy (abstract class)
	public abstract class CommentMouseHandlerStateStrategy : SpreadsheetMouseHandlerStateStrategyBase<CommentMouseHandlerStateBase> {
		protected CommentMouseHandlerStateStrategy(CommentMouseHandlerStateBase state)
			: base(state) {
		}
		protected internal abstract void ShowVisualFeedback(Rectangle bounds, Page page);
		protected internal abstract void HideVisualFeedback();
	}
	#endregion
}
