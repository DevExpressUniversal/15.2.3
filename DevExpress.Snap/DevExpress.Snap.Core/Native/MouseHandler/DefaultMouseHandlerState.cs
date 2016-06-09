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

using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using DevExpress.Snap.Core.Fields;
using DevExpress.Utils;
using DevExpress.Snap.Core.Options;
using System;
namespace DevExpress.Snap.Core.Native.MouseHandler {
	public class SnapDefaultMouseHandlerState : DefaultMouseHandlerState {
		public SnapDefaultMouseHandlerState(SnapMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		public new SnapMouseHandler MouseHandler { get { return (SnapMouseHandler)base.MouseHandler; } }
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		protected internal override DragContentMouseHandlerStateBase CreateExternalDragState() {
			return new SnapDragExternalContentMouseHandlerState(MouseHandler);
		}
		protected override void OnMouseMoveCore(PlatformIndependentMouseEventArgs e, Point physicalPoint, RichEditHitTestResult hitTestResult) {
			base.OnMouseMoveCore(e, physicalPoint, hitTestResult);
			if (MouseHandler.ActiveObject == null) {
				if (DocumentModel.HighlightedField != null)
					SetHighlightedField(null, DocumentModel.HighlightedField.FirstRunIndex, DocumentModel.HighlightedField.LastRunIndex);
				return;
			}
			Field field = MouseHandler.ActiveObject as Field;
			MouseHandler.FieldHierarchicalController.ChangeCurrentItem(field, hitTestResult.PieceTable);
			if (field != null && ((hitTestResult.Accuracy & HitTestAccuracy.ExactBox) == 0 || !field.ContainsRun(hitTestResult.Box.StartPos.RunIndex)))
				field = null;
			if (field == null && DocumentModel.HighlightedField == null)
				return;
			if (field == null && DocumentModel.HighlightedField != null) {
				SetHighlightedField(null, DocumentModel.HighlightedField.FirstRunIndex, DocumentModel.HighlightedField.LastRunIndex);
				return;
			}
			if (field != null) {
				FieldCalculatorService service = new FieldCalculatorService();
				CalculatedFieldBase parsedInfo = service.ParseField(hitTestResult.PieceTable, field);
				if (parsedInfo is SNMergeFieldBase) {
					if (DocumentModel.HighlightedField == null) {
						SetHighlightedField(field, field.FirstRunIndex, field.LastRunIndex);
					} else if (field != DocumentModel.HighlightedField) {
						RunIndex min = Algorithms.Min(field.FirstRunIndex, DocumentModel.HighlightedField.FirstRunIndex);
						RunIndex max = Algorithms.Max(field.LastRunIndex, DocumentModel.HighlightedField.LastRunIndex);
						SetHighlightedField(field, min, max);
					}
				} else
					if (DocumentModel.HighlightedField != null)
						SetHighlightedField(null, DocumentModel.HighlightedField.FirstRunIndex, DocumentModel.HighlightedField.LastRunIndex);
			}
		}
		void SetHighlightedField(Field field, RunIndex firstRunIndex, RunIndex lastRunIndex) {
			DocumentModel.BeginUpdate();
			DocumentModel.HighlightedField = field;
			DocumentModel.ApplyChangesCore(DocumentModel.ActivePieceTable, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout, firstRunIndex, lastRunIndex);
			DocumentModel.EndUpdate();
		}
		protected internal override void HandleMouseDown(RichEditHitTestResult hitTestResult) {
			if(HandleMouseDownOnActiveUIElements(hitTestResult))
				return;
			base.HandleMouseDown(hitTestResult);
			if(hitTestResult == null || MouseHandler.State is RichEditRectangularObjectResizeMouseHandlerState)
				return;
			SelectField(hitTestResult);
		}
		protected virtual bool HandleMouseDownOnActiveUIElements(RichEditHitTestResult hitTestResult) {
			List<ILayoutUIElementViewInfo> viewInfos = ((InnerSnapControl)Control.InnerControl).ActiveUIElementViewInfos;
			foreach(ILayoutUIElementViewInfo viewInfo in viewInfos)
				if(viewInfo.HandleMouseDown(Control, hitTestResult))
					return true;
			return false;
		}
		protected internal override bool HandleHotZoneMouseDownCore(RichEditHitTestResult hitTestResult, HotZone hotZone) {
			if(FieldsHelper.IsNotResizableField(hotZone))
				return false;
			return base.HandleHotZoneMouseDownCore(hitTestResult, hotZone);
		}
		protected internal virtual void SelectField(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DocumentModel.Selection.Length > 0)
				return;
			Field field = FieldsHelper.FindFieldByHitTestResult(hitTestResult);
			PieceTable pieceTable = hitTestResult.PieceTable;
			if(!ShouldSelectField(pieceTable, field))
				return;
			if (!Object.ReferenceEquals(ActivePieceTable, field.PieceTable))
				return;
			DocumentLogPosition start = pieceTable.GetRunLogPosition(field.FirstRunIndex);
			DocumentLogPosition end = pieceTable.GetRunLogPosition(field.LastRunIndex);
			SetSelection(start, end);
			if(ShouldBeginDragExistingSelection(hitTestResult))
				BeginDragExistingSelection(hitTestResult, false);
		}
		internal bool ShouldSelectField(PieceTable pieceTable, Field field) {
			if(field == null)
				return false;
			FieldSelectionOnMouseClickMode fieldSelection = DocumentModel.FieldOptions.FieldSelection;
			if(fieldSelection != FieldSelectionOnMouseClickMode.Always && fieldSelection != FieldSelectionOnMouseClickMode.Auto)
				return false;
			if(field.IsCodeView)
				return false;
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			CalculatedFieldBase parsedInfo = calculator.ParseField(pieceTable, field);
			return parsedInfo is MergefieldField;
		}
	}
}
