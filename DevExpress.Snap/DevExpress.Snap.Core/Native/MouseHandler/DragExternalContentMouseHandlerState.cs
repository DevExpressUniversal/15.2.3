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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Snap.Core.Options;
using System.Data;
namespace DevExpress.Snap.Core.Native.MouseHandler {
	public interface IDropFieldTarget {
		void OnDragOver(DragEventArgs e);
		bool DoDragDrop(DragEventArgs e);
		HotZone VisibleHotZone { get; }
	}
	public enum DragCaretType {
		Default,
		InsertField,
		InsertList,
		InsertMasterDetail
	}
	public class DragCaretGiveFeedbackEventArgs : EventArgs {
		readonly DocumentLogPosition logPosition;
		DragCaretType caretType;
		public DragCaretGiveFeedbackEventArgs(DocumentLogPosition logPosition) {
			this.logPosition = logPosition;
			this.caretType = DragCaretType.Default;
		}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		public DragCaretType CaretType { get { return caretType; } set { caretType = value; } }
	}
	public interface IDragCaretGiveFeedbackSupport {
		event EventHandler<DragCaretGiveFeedbackEventArgs> GiveFeedback;
	}
	public class SnapDragExternalContentMouseHandlerState : DragExternalContentMouseHandlerState {
		SNDataInfo[] dataInfos;
		List<string> insertPositionDataSourses;
		InsertActionTree tree;
		public SnapDragExternalContentMouseHandlerState(RichEditMouseHandler mouseHandler)
			: base(mouseHandler) {
				this.insertPositionDataSourses = new List<string>();
		}
		protected InsertActionTree ActionTree {
			get {
				if (tree == null)
					tree = CalculateInsertActionTree();
				return tree;
			}
		}
		public override bool UseHover { get { return true; } }
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public override void Start() {
			SubscribeToGiveFeedbackEvent();
			base.Start();
		}
		public override void Finish() {
			UnsubscribeFromGiveFeedbackEvent();
			base.Finish();
		}
		void SubscribeToGiveFeedbackEvent() {
			IDragCaretGiveFeedbackSupport giveFeedbackSupport = CaretVisualizer as IDragCaretGiveFeedbackSupport;
			if (giveFeedbackSupport != null)
				giveFeedbackSupport.GiveFeedback += OnCaretVisualizerGiveFeedback;
		}
		void UnsubscribeFromGiveFeedbackEvent() {
			IDragCaretGiveFeedbackSupport giveFeedbackSupport = CaretVisualizer as IDragCaretGiveFeedbackSupport;
			if (giveFeedbackSupport != null)
				giveFeedbackSupport.GiveFeedback -= OnCaretVisualizerGiveFeedback;
		}
		public override void OnDragOver(DragEventArgs e) {
			base.OnDragOver(e);
			IDropFieldTarget dropTarget = Control.InnerControl.ActiveView.TableController as IDropFieldTarget;
			if (dropTarget != null)
				dropTarget.OnDragOver(e);
		}
		protected override bool ShouldShowVisualFeedback(Point point) {
			RichEditView activeView = Control.InnerControl.ActiveView;
			if (activeView.HoverLayout != null)
				return false;
			if (!ValidateActionTree())
				return false;
			IDropFieldTarget dropTarget = activeView.TableController as IDropFieldTarget;
			if (dropTarget == null)
				return true;
			return dropTarget.VisibleHotZone == null;
		}
		bool ValidateActionTree() {
			if (this.dataInfos == null)
				return true;
			if (this.dataInfos.Length == 0)
				return false;
			return ActionTree != null && !ActionTree.IsEmpty;
		}
		protected override void OnDragDropCore(DragEventArgs e) {
			HideVisualFeedback();
			if (IsMailMergeFieldDropped(e))
				return;
			IDropFieldTarget dropFieldTarget = Control.InnerControl.ActiveView.TableController as IDropFieldTarget;
			if (dropFieldTarget != null && dropFieldTarget.DoDragDrop(e))
				return;
			IDropTarget dropTarget = Control.InnerControl.ActiveView.HoverLayout as IDropTarget;
			if (dropTarget != null) {
				dropTarget.OnDragDrop(e);
				Control.InnerControl.ActiveView.HoverLayout = null;
				return;
			}
			base.OnDragDropCore(e);
		}
		protected internal virtual IDataObject GetDataObject(SNDataInfo[] dataInfo) {
			return new DataObject(SnapDataFormats.SNDataInfoFullName, dataInfo);
		}
		bool IsMailMergeFieldDropped(DragEventArgs e) {
			IDataObject data = e.Data;
			SNDataInfo[] dataInfo = data.GetData(SnapDataFormats.SNDataInfoFullName) as SNDataInfo[];
			if (dataInfo == null)
				return false;
			if (!ShouldDropMailMergeFild(dataInfo, new Point(e.X, e.Y)))
				return false;
			Control.InnerControl.DocumentModel.BeginUpdate();
			SelectionHelper.ClearSelection(Control.InnerControl.DocumentModel.Selection);
			PasteDataInfoTemplate(dataInfo);
			Control.InnerControl.DocumentModel.EndUpdate();
			return true;
		}
		bool ShouldDropMailMergeFild(SNDataInfo[] dataInfo, Point point) {
			SnapMailMergeVisualOptions options = ((DevExpress.Snap.Core.Options.ISnapControlOptions)Control.InnerControl.Options).SnapMailMergeVisualOptions;
			if (options.DataSource == null) 
				return false;
			Selection selection = Control.InnerControl.DocumentModel.Selection;
			DragCaretType caretType = CalculateDragCaretType();
			if (caretType != DragCaretType.InsertField)
				return false;
			RichEditHitTestResult hitTestResult = CalculateHitTest(point);
			if (hitTestResult == null)
				return false;
			DocumentLogPosition pos = CalcHitTestLogPosition(hitTestResult);
			if (selection.NormalizedStart > pos || selection.NormalizedEnd < pos)
				return false;
			return true;
		}
		protected internal virtual DocumentLogPosition CalcHitTestLogPosition(RichEditHitTestResult hitTestResult) {
			DocumentModelPosition pos = GetHitTestDocumentModelPosition(hitTestResult);
			return pos.LogPosition;
		}
		void PasteDataInfoTemplate(SNDataInfo[] dataInfo) {
			PasteDataInfoTemplateCommand command = new PasteDataInfoTemplateCommand(Control);
			command.PasteSource = new DataObjectPasteSource(GetDataObject(dataInfo));
			command.Execute();
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			if (dataObject != null)
				this.dataInfos = dataObject.GetData(SnapDataFormats.SNDataInfoFullName) as SNDataInfo[];
			return base.ContinueDrag(point, allowedEffects, dataObject);
		}
		protected internal override DragDropEffects CalculateDragDropEffects() {
			if (!ValidateActionTree())
				return DragDropEffects.None;
			return base.CalculateDragDropEffects();
		}
		void OnCaretVisualizerGiveFeedback(object sender, DragCaretGiveFeedbackEventArgs e) {
			e.CaretType = CalculateDragCaretType();
		}
		InsertActionTree CalculateInsertActionTree() {
			TemplateBuilder builder = DocumentModel.CreateTemplateBuilder();
			SnapPieceTable pieceTable = DocumentModel.ActivePieceTable;
			SnapBookmarkController bookmarkController = new SnapBookmarkController(pieceTable);
			SnapBookmark bookmark = bookmarkController.FindInnermostTemplateBookmarkByPosition(CaretLogPosition);
			this.insertPositionDataSourses = builder.GetInsertPositionDataSources(pieceTable, bookmark);
			return builder.CalculateInsertActionTree(DocumentModel, dataInfos, this.insertPositionDataSourses);
		}
		DragCaretType CalculateDragCaretType() {
			if (this.dataInfos == null)
				return DragCaretType.Default;
			return DragCaretTypeCalculator.CalculateDragCaretType(ActionTree, this.insertPositionDataSourses);
		}
		protected internal override Command CreateDropCommand(DocumentModelPosition pos, IDataObject dataObject) {
			if (dataObject != null)
				return new SnapDragCopyExternalContentCommand(Control, pos, dataObject);
			return base.CreateDropCommand(pos, dataObject);
		}
	}
	internal static class DragCaretTypeCalculator {
		public static DragCaretType CalculateDragCaretType(InsertActionTree actionTree, List<string> insertPositionDataSources) {
			InsertActionTreeNode rootNode = actionTree.RootNode;
			DragCaretType result = DragCaretType.Default;
			foreach (InsertActionTreeNode node in rootNode.ChildNodes) {
				InsertObjectActionBase insertObjectAction = node.Action as InsertObjectActionBase;
				if (insertObjectAction == null) {
					result = GetDragCaretType(result, DragCaretType.InsertField);
					continue;
				}
				string dataReferenceString = insertObjectAction.GetDataReferenceString();
				int index = insertPositionDataSources.IndexOf(dataReferenceString);
				if (index < 0) {
					result = GetDragCaretType(result, CalculateDragCaretTypeRecursive(node));
					continue;
				}
				DragCaretType type = CalculateDragCaretTypeCore(node, insertPositionDataSources, index - 1);
				result = GetDragCaretType(result, type);
			}
			return result;
		}
		static DragCaretType CalculateDragCaretTypeRecursive(InsertActionTreeNode rootNode) {
			DragCaretType result = rootNode.Action is InsertSNListAction ? DragCaretType.InsertList : DragCaretType.InsertField;
			foreach (InsertActionTreeNode node in rootNode.ChildNodes) {
				DragCaretType innerType = CalculateDragCaretTypeRecursive(node);
				if (result == DragCaretType.InsertList && innerType == DragCaretType.InsertList)
					result = DragCaretType.InsertMasterDetail;
				else
					result = GetDragCaretType(result, innerType);
			}
			return result;
		}
		static DragCaretType CalculateDragCaretTypeCore(InsertActionTreeNode parentNode, List<string> insertPositionDataSource, int index) {
			DragCaretType result = DragCaretType.Default;
			foreach (InsertActionTreeNode node in parentNode.ChildNodes) {
				InsertObjectActionBase insertObjectAction = node.Action as InsertObjectActionBase;
				if (insertObjectAction == null) {
					DragCaretType type = CalculateDragCaretTypeRecursive(node);
					result = GetDragCaretType(result, type);
					continue;
				}
				if (index < 0) {
					DragCaretType type = CalculateDragCaretTypeRecursive(node);
					result = GetDragCaretType(result, type);
					continue;
				}
				if (insertPositionDataSource[index] == insertObjectAction.GetDataReferenceString())
					result = CalculateDragCaretTypeCore(node, insertPositionDataSource, index - 1);
			}
			return result;
		}
		static DragCaretType GetDragCaretType(DragCaretType oldValue, DragCaretType newValue) {
			return newValue > oldValue ? newValue : oldValue;
		}
	}
}
