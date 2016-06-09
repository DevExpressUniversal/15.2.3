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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Forms {
	public class ReportStructureEditorFormControllerParameters : FormControllerParameters {
		internal ReportStructureEditorFormControllerParameters(ISnapControl control)
			: base(control) {
		}
#if !SL
	[DevExpressSnapCoreLocalizedDescription("ReportStructureEditorFormControllerParametersControl")]
#endif
		new public ISnapControl Control { get { return (ISnapControl)base.Control; } }
	}
	public class ReportStructureEditorFormController : FormController {
		readonly ISnapControl control;
		readonly Dictionary<string, FieldPathInfo> pathInfos = new Dictionary<string, FieldPathInfo>();
		IFieldPathService fieldPathService;
		public ReportStructureEditorFormController(ReportStructureEditorFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			FocusedNodeTag = GetFocusedNodeTag();
		}
		#region Properties
		public ISnapControl Control { get { return control; } }
		public Pair<string, int> FocusedNodeTag { get; set; }
		protected IFieldPathService FieldPathService {
			get {
				if (fieldPathService == null) {
					IFieldDataAccessService service = DocumentModel.GetService<IFieldDataAccessService>();
					if (service != null)
						fieldPathService = service.FieldPathService;
				}
				return fieldPathService;
			}
		}
		protected Dictionary<string, FieldPathInfo> PathInfos { get { return pathInfos; } }
		protected SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)Control.InnerControl.DocumentModel; } }
		protected SnapPieceTable PieceTable { get { return DocumentModel.ActivePieceTable; } }
		#endregion
		public void MoveUp() {
			FieldPathDataMemberInfo dataMemberInfo = GetEditedDataMemberInfo();
			if (dataMemberInfo == null || dataMemberInfo.IsEmpty)
				return;
			int index = FocusedNodeTag.Second;
			Algorithms.SwapElements(dataMemberInfo.LastItem.Groups, index - 1, index);
		}
		public void MoveDown() {
			FieldPathDataMemberInfo dataMemberInfo = GetEditedDataMemberInfo();
			if (dataMemberInfo == null || dataMemberInfo.IsEmpty)
				return;
			int index = FocusedNodeTag.Second;
			Algorithms.SwapElements(dataMemberInfo.LastItem.Groups, index, index + 1);
		}
		public override void ApplyChanges() {
			if (PathInfos.Count == 0 || FieldPathService == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				ApplyChangesCore();
				Field field = new SnapObjectModelController(PieceTable).GetTopLevelListFieldBySelection();
				if (field != null) {
					Selection selection = DocumentModel.Selection;
					SnapCaretPosition caretPos = DocumentModel.SelectionInfo.GetSnapCaretPositionFromSelection(selection.PieceTable, selection.NormalizedStart);
					PieceTable.FieldUpdater.UpdateFieldAndNestedFields(field);
					if (caretPos != null)
						DocumentModel.SelectionInfo.RestoreCaretPosition(caretPos);
					if (!DocumentModel.IsUpdateLocked)
						Control.InnerControl.ActiveView.EnsureCaretVisible();
					else
						DocumentModel.DeferredChanges.EnsureCaretVisible = true;
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void ApplyChangesCore() {
			foreach (var item in PathInfos) {
				if (item.Value == null)
					continue;
				InstructionController controller = CreateInstructionControllerByFieldName(item.Key);
				if (controller == null)
					continue;
				controller.SuppressFieldsUpdateAfterUpdateInstruction = true;
				controller.SetArgument(0, FieldPathService.GetStringPath(item.Value));
				controller.ApplyDeferredActions();
				PieceTable.UpdateTemplateByField(controller.Field, false);
			}
		}
		public Field FindSelectedListField() {
			SnapListFieldInfo fieldInfo = new ListFieldSelectionController(DocumentModel).FindListField();
			return fieldInfo != null ? fieldInfo.Field : null;
		}
		FieldPathDataMemberInfo GetEditedDataMemberInfo() {
			FieldPathInfo result;
			string fieldName = FocusedNodeTag.First;
			if (FieldPathService == null)
				return null;
			if (!PathInfos.TryGetValue(fieldName, out result)) {
				InstructionController controller = CreateInstructionControllerByFieldName(fieldName);
				if (controller == null)
					return null;
				result = FieldPathService.FromString(controller.GetArgumentAsString(0));
				PathInfos.Add(fieldName, result);
			}
			return result.DataMemberInfo;
		}
		InstructionController CreateInstructionControllerByFieldName(string fieldName) {
			Field field = PieceTable.FindFieldNearestToSelection(fieldName, false);
			if (field == null)
				return null;
			return PieceTable.CreateFieldInstructionController(field);
		}
		Pair<string, int> GetFocusedNodeTag() {
			SnapDocumentModel model = DocumentModel;
			SnapListFieldInfo fieldInfo = FieldsHelper.GetSelectedSNListField(model);
			if (fieldInfo == null)
				return null;
			SnapPieceTable pieceTable = model.ActivePieceTable;
			SnapBookmark bookmark = new SnapBookmarkController(pieceTable).FindInnermostTemplateBookmarkByPosition(model.Selection.Start);
			if (bookmark == null)
				return null;
			return new SnapObjectModelController(pieceTable).GetTag(bookmark.TemplateInterval.TemplateInfo, fieldInfo);
		}
	}
}
