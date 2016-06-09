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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
namespace DevExpress.Snap.Core.Commands {
	public class TemplateModifierExecutor {
		readonly string templateSwitch;
		readonly SnapListFieldInfo fieldInfo;
		public TemplateModifierExecutor(SnapListFieldInfo fieldInfo, string templateSwitch) {
			this.fieldInfo = fieldInfo;
			this.templateSwitch = templateSwitch;
		}
		protected SnapListFieldInfo FieldInfo { get { return fieldInfo; } }
		protected string TemplateSwitch { get { return templateSwitch; } }
		public bool SuppressFieldsUpdateAfterUpdateInstruction { get; set; }
		public void ModifyTemplate(ITemplateModifier modifier) {
			SnapPieceTable pieceTable = FieldInfo.PieceTable;
			pieceTable.DocumentModel.BeginUpdate();
			using (InstructionController controller = pieceTable.CreateFieldInstructionController(FieldInfo.Field)) {
				if (controller != null) {
					modifier.Execute(controller, TemplateSwitch);
					controller.SuppressFieldsUpdateAfterUpdateInstruction = SuppressFieldsUpdateAfterUpdateInstruction;
				}
			}
			pieceTable.DocumentModel.EndUpdate();
		}
	}
	public interface ITemplateModifier {
		void Execute(InstructionController controller, string templateSwitch);
	}
	public abstract class InternalDocumentModelTemplateCommandBase : ITemplateModifier {
		readonly SnapDocumentModel sourceDocumentModel;
		protected InternalDocumentModelTemplateCommandBase(SnapDocumentModel sourceDocumentModel) {
			this.sourceDocumentModel = sourceDocumentModel;
		}
		protected SnapDocumentModel SourceDocumentModel { get { return sourceDocumentModel; } }
		public void Execute(InstructionController controller, string templateSwitch) {
			DocumentLogInterval templateInterval = controller.Instructions.GetSwitchArgumentDocumentInterval(SNListField.TemplateSwitch);
			SnapListFieldInfo listFieldInfo = new SnapListFieldInfo((SnapPieceTable)controller.PieceTable, controller.Field);
			DocumentModel groupHeaderModel = CreateModel((SnapPieceTable)controller.PieceTable, templateInterval, listFieldInfo);
			if (!groupHeaderModel.IsEmpty)
				controller.SetSwitch(templateSwitch, groupHeaderModel);
		}
		protected virtual DocumentModel CreateModel(SnapPieceTable pieceTable, DocumentLogInterval templateInterval, IFieldInfo listFieldInfo) {
			SnapDocumentModel result = (SnapDocumentModel)SourceDocumentModel.CreateNew();
			result.BeginSetContent();
			try {
				result.InheritDataServices(SourceDocumentModel);
				CreateModelCore(result);
			}
			finally {
				result.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
			return result;
		}
		protected virtual void CreateModelCore(DocumentModel result) { }
	}
	public class InternalRemoveHeaderFooterCommand : ITemplateModifier {
		public void Execute(InstructionController controller, string templateSwitch) {
			controller.RemoveSwitch(templateSwitch);
		}
	}
	public abstract class InternalInsertHeaderFooterCommandBase : InternalDocumentModelTemplateCommandBase {
		readonly int level;
		protected InternalInsertHeaderFooterCommandBase(SnapDocumentModel sourceDocumentModel, int level)
			: base(sourceDocumentModel) {
			this.level = level;
		}
		protected int Level { get { return level; } }
		protected abstract string BaseStyleName { get; }
		protected internal virtual DocumentModel CreateModel(SnapPieceTable pieceTable, DocumentLogInterval templateInterval, Action<Field, MergefieldField, PieceTable, DocumentLogPosition> setContentAction, Action<PieceTable> finishAction, Action<TableCollection> processTableBeforeFinishAction) {
			SnapDocumentModel result = TableCommandsHelper.CreateModel(SourceDocumentModel, pieceTable, templateInterval, setContentAction, processTableBeforeFinishAction, finishAction);
			result.BeginUpdate();
			try {
				StyleHelper.ApplyTableCellStyle(SourceDocumentModel, result, BaseStyleName, Level);
			}
			finally {
				result.EndUpdate();
			}
			return result;
		}
	}
	public class InsertHeaderCommandInternal : InternalInsertHeaderFooterCommandBase {
		SNDataInfo[] dataInfos;
		public InsertHeaderCommandInternal(SnapDocumentModel sourceDocumentModel, int level, SNDataInfo[] dataInfos)
			: this(sourceDocumentModel, level) {
				this.dataInfos = dataInfos;
		}
		public InsertHeaderCommandInternal(SnapDocumentModel sourceDocumentModel, int level)
			: base(sourceDocumentModel, level) {
		}
		protected override string BaseStyleName { get { return SnapDocumentModel.DefaultListHeaderStyleName; } }
		protected override DocumentModel CreateModel(SnapPieceTable pieceTable, DocumentLogInterval templateInterval, IFieldInfo listFieldInfo) {
			Action<Field, MergefieldField, PieceTable, DocumentLogPosition> setContentAction = delegate(Field field, MergefieldField mergeField, PieceTable targetPieceTable, DocumentLogPosition start) {
				string fieldName = String.Empty;
				if(mergeField != null && field.Parent == null) {
					if(!IsArrayNullOrEmpty(dataInfos))
						fieldName = GetFieldDisplayNameFromDataInfo(FieldPathService.DecodePath(mergeField.DataFieldName));
					if(string.IsNullOrEmpty(fieldName))
						fieldName = FieldsHelper.GetFieldDisplayName(SourceDocumentModel.DataSourceDispatcher, (SnapListFieldInfo)listFieldInfo, mergeField.DataFieldName, mergeField.DataFieldName);
				}
				if (!String.IsNullOrEmpty(fieldName))
					targetPieceTable.InsertText(start, fieldName);
			};
			Action<TableCollection> processTablesBeforeFinishAction = delegate(TableCollection tables) {
				int count = tables.Count - 1;
				for (int i = count; i >= 0; i--)
					tables[i].RemoveEmptyRows();
			};
			return CreateModel(pieceTable, templateInterval, setContentAction, null, processTablesBeforeFinishAction);
		}
		private string GetFieldDisplayNameFromDataInfo(string dataFieldName) {
			if(!IsArrayNullOrEmpty(dataInfos)) {
				foreach(var dataInfo in dataInfos) {
					if(!IsArrayNullOrEmpty(dataInfo.DataPaths) && string.Equals(dataInfo.DataPaths[dataInfo.EscapedDataPaths.Length - 1], dataFieldName))
						return dataInfo.DisplayName;
				}
			}
			return String.Empty;
		}
		private bool IsArrayNullOrEmpty(object[] array) {
			return array == null || array.Length == 0;
		}
	}
	public class InsertFooterCommandInternal : InternalInsertHeaderFooterCommandBase {
		public InsertFooterCommandInternal(SnapDocumentModel sourceDocumentModel, int level)
			: base(sourceDocumentModel, level) {
		}
		protected override string BaseStyleName { get { return SnapDocumentModel.DefaultListFooterStyleName; } }
		protected override DocumentModel CreateModel(SnapPieceTable pieceTable, DocumentLogInterval templateInterval, IFieldInfo listFieldInfo) {
			return CreateModel(pieceTable, templateInterval, null, null, TableCommandsHelper.MergeTablesTotally);
		}
	}
}
