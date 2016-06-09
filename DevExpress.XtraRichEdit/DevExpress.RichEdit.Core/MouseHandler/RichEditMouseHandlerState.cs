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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Drawing;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Mouse {
	#region RichEditMouseHandlerState (abstract class)
	public abstract class RichEditMouseHandlerState : MouseHandlerState {
		HoverInfo lastHoverInfo = HoverInfo.Empty;
		protected RichEditMouseHandlerState(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public new RichEditMouseHandler MouseHandler { get { return (RichEditMouseHandler)base.MouseHandler; } }
		public IRichEditControl Control { get { return MouseHandler.Control; } }
		public DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		public PieceTable ActivePieceTable { get { return DocumentModel.ActivePieceTable; } }
		protected HoverInfo LastHoverInfo { get { return lastHoverInfo; } }
		public virtual bool UseHover { get { return false; } }
		public virtual bool SuppressDefaultMouseWheelProcessing { get { return false; } }
		#endregion
		protected Page GetTopLevelPage(RichEditHitTestResult initialHitTestResult) {
			if (initialHitTestResult.FloatingObjectBox != null && initialHitTestResult.FloatingObjectBoxPage != null)
				return initialHitTestResult.FloatingObjectBoxPage;
			else
				if (initialHitTestResult.CommentViewInfo != null)
					return initialHitTestResult.CommentViewInfo.CommentViewInfoPage;
				return initialHitTestResult.Page;
		}
		public override void OnMouseMove(MouseEventArgs e) {
			Point physicalPoint = new Point(e.X, e.Y);
			RichEditHitTestResult hitTestResult = CalculateNearestCharacterHitTest(physicalPoint, Control.InnerControl.DocumentModel.ActivePieceTable);
			OnMouseMoveCore(e, physicalPoint, hitTestResult);
		}
		protected virtual void OnMouseMoveCore(MouseEventArgs e, Point physicalPoint, RichEditHitTestResult hitTestResult) {
			if (UseHover)
				UpdateHover(hitTestResult);
			MouseHandler.SetActiveObject(CalculateActiveObject(hitTestResult, physicalPoint) );
		}
		protected internal virtual object CalculateActiveObject(RichEditHitTestResult hitTestResult, Point physicalPoint) {
			if (hitTestResult != null && hitTestResult.CommentViewInfo != null) {
				SetCommentViewInfoFocused(hitTestResult);
				return CalculateActiveObjectAsCommetViewInfo(hitTestResult);
			}
			bool checkMainPieceTableOnly = false;
			if (hitTestResult == null) {
				hitTestResult = Control.InnerControl.ActiveView.CalculateNearestPageHitTest(physicalPoint, true);
				if (hitTestResult != null && hitTestResult.CommentViewInfo != null) {
					SetCommentViewInfoFocused(hitTestResult);
					return CalculateActiveObjectAsCommetViewInfo(hitTestResult);
				}
				hitTestResult = CalculateNearestCharacterHitTest(physicalPoint, Control.InnerControl.DocumentModel.MainPieceTable);
				checkMainPieceTableOnly = true;
			}
			if (hitTestResult == null || hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Box || (hitTestResult.Accuracy & HitTestAccuracy.ExactBox) == 0)
				return null;
			if (checkMainPieceTableOnly || DocumentModel.ActivePieceTable.IsMain) {
				object result = CalculateActiveObjectAsComment(hitTestResult);
				if (result != null)
					return result;
			}
			if(!checkMainPieceTableOnly || DocumentModel.ActivePieceTable.IsMain)
				return CalculateActiveObjectAsField(hitTestResult);
			return null;
		}
		Box GetHighDetailLevelBox(RichEditHitTestResult hitTestResult) {
			return ((hitTestResult.Accuracy & HitTestAccuracy.ExactCharacter) != 0) ? hitTestResult.Character : hitTestResult.Box;
		}
		protected virtual void SetCommentViewInfoFocused(RichEditHitTestResult hitTestResult) {
			CommentViewInfoHelper helper = new CommentViewInfoHelper();
			helper.ResetFocused(hitTestResult.Page.Comments);
			hitTestResult.CommentViewInfo.Focused = true;
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.CommentViewInfo);
		}
		protected virtual object CalculateActiveObjectAsCommetViewInfo(RichEditHitTestResult hitTestResult) {
			return hitTestResult.CommentViewInfo;
		}
		protected virtual Comment CalculateActiveObjectAsComment(RichEditHitTestResult hitTestResult) {
			if(hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Character)
				return null;
			FormatterPosition start = hitTestResult.Character.StartPos;
			DocumentLogPosition logPosition = DocumentModel.MainPieceTable.GetRunLogPosition(start.RunIndex) + start.Offset;
			return DocumentModel.MainPieceTable.FindCommentByDocumentLogPosition(logPosition);
		}
		protected virtual Field CalculateActiveObjectAsField(RichEditHitTestResult hitTestResult) {
			return DocumentModel.ActivePieceTable.FindFieldByRunIndex(GetHighDetailLevelBox(hitTestResult).StartPos.RunIndex);
		}
		protected internal virtual void SetMouseCursor(RichEditCursor cursor) {
			Control.Cursor = cursor.Cursor;
		}
		protected internal virtual RichEditHitTestResult CalculateNearestCharacterHitTest(Point physicalPoint, PieceTable pieceTable) {
			return Control.InnerControl.ActiveView.CalculateNearestCharacterHitTest(physicalPoint, pieceTable);
		}
		protected virtual void UpdateHover(RichEditHitTestResult hitTestResult) {
			bool boxChanged = LastHoverInfo.UpdateHover(hitTestResult);
			UpdateHoverLayout(hitTestResult, Control.InnerControl.ActiveView, boxChanged);
		}
		protected virtual void UpdateHoverLayout(RichEditHitTestResult hitTestResult, RichEditView activeView, bool boxChanged) {
			if(LastHoverInfo.IsInsideBox) {
				IHoverLayoutItemsService hoverLayoutItemsService = DocumentModel.GetService<IHoverLayoutItemsService>();
				if(boxChanged)
					activeView.HoverLayout = hoverLayoutItemsService.Get(activeView, hitTestResult.Box, hitTestResult.LogPosition, hitTestResult.PieceTable, hitTestResult.LogicalPoint);
				else if(activeView.HoverLayout != null)
					activeView.HoverLayout.MousePosition = hitTestResult.LogicalPoint;
			} else
				activeView.HoverLayout = null;
			Control.RedrawEnsureSecondaryFormattingComplete(RefreshAction.Selection);
		}
	}
	#endregion
}
