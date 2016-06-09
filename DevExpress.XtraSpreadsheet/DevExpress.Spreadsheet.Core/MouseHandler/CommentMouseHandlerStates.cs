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

using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Utils;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region CommentMouseHandlerStateBase (abstract class)
	public abstract class CommentMouseHandlerStateBase : CancellableMouseHandlerStateBase {
		#region Fields
		readonly CommentMouseHandlerStateStrategy strategy;
		readonly CommentBox box;
		Rectangle visualFeedbackBounds;
		Point startPoint;
		Point currentPoint;
		Page page;
		#endregion
		protected CommentMouseHandlerStateBase(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
			this.strategy = CreatePlatformStrategy();
			this.box = hitTestResult.CommentBox;
			Debug.Assert(box != null);
			this.page = hitTestResult.Page;
			Debug.Assert(page != null);
			this.visualFeedbackBounds = box.Bounds;
			this.startPoint = hitTestResult.LogicalPoint;
			this.currentPoint = hitTestResult.LogicalPoint;
		}
		#region Properties
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Cell; } }
		protected internal CommentBox Box { get { return box; } }
		protected internal Point StartPoint { get { return startPoint; } }
		protected internal Point CurrentPoint { get { return currentPoint; } }
		#endregion
		CommentMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateCommentMouseHandlerStateStrategy(this);
		}
		protected internal override void UpdateObjectProperties() {
			page = CurrentHitTestResult.Page;
			currentPoint = CurrentHitTestResult.LogicalPoint;
			int offsetX = currentPoint.X - StartPoint.X;
			int offsetY = currentPoint.Y - StartPoint.Y;
			visualFeedbackBounds = CalculateVisualFeedbackBounds(box.Bounds, offsetX, offsetY);
		}
		protected internal override void BeginVisualFeedback() {
			ShowVisualFeedbackCore();
		}
		protected internal override void ShowVisualFeedback() {
			ShowVisualFeedbackCore();
		}
		void ShowVisualFeedbackCore() {
			this.strategy.ShowVisualFeedback(visualFeedbackBounds, page);
		}
		protected internal override void HideVisualFeedback() {
			this.strategy.HideVisualFeedback();
		}
		protected internal abstract Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY);
	}
	#endregion
	#region DragCommentMouseHandlerState
	public class CommentDragMouseHandlerState : CommentMouseHandlerStateBase {
		public CommentDragMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
		}
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.DragRange;
		}
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			originalBounds.Offset(offsetX, offsetY);
			return originalBounds;
		}
		protected internal override void CommitChanges(Point point) {
			Control.BeginUpdate();
			try {
				int offsetX = CurrentPoint.X - StartPoint.X;
				int offsetY = CurrentPoint.Y - StartPoint.Y;
				int commentIndex = Box.GetCommentIndex();
				DocumentModel.ActiveSheet.MoveComment(commentIndex, offsetX, offsetY);
			} finally {
				Control.EndUpdate();
			}
		}
	}
	#endregion
	#region  CommentResizeMouseHandlerState
	public class CommentResizeMouseHandlerState : CommentMouseHandlerStateBase {
		#region Fields
		readonly CommentResizeHotZoneBase hotZone;
		#endregion
		public CommentResizeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult, CommentResizeHotZoneBase hotZone)
			: base(mouseHandler, hitTestResult) {
			Guard.ArgumentNotNull(hotZone, "hotZone");
			this.hotZone = hotZone;
		}
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.SmallCross;
		}
		protected internal override Rectangle CalculateVisualFeedbackBounds(Rectangle originalBounds, int offsetX, int offsetY) {
			Rectangle bounds = hotZone.CalculateVisualFeedbackBounds(originalBounds, offsetX, offsetY);
			bounds = hotZone.ForceKeepOriginalAspectRatio(bounds, originalBounds);
			return bounds;
		}
		protected internal override void CommitChanges(Point point) {
			Control.BeginUpdate();
			try {
				Rectangle size = hotZone.CreateValidBoxBounds(CurrentPoint);
				Rectangle originalBounds = Box.Bounds;
				size = hotZone.ForceKeepOriginalAspectRatio(size, originalBounds);
				int commentIndex = Box.GetCommentIndex();
				DocumentModel.ActiveSheet.ResizeComment(commentIndex, size.Width, size.Height);
				int offsetX = size.Left - originalBounds.Left;
				int offsetY = size.Top - originalBounds.Top;
				DocumentModel.ActiveSheet.MoveComment(commentIndex, offsetX, offsetY);
			}
			finally {
				Control.EndUpdate();
			}
		}
	}
	#endregion
}
