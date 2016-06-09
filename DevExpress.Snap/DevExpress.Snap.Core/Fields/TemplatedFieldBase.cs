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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Snap.Core.Native.Services;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core {
	public class TemplateFieldInterval {
		readonly DocumentLogInterval interval;
		readonly string templateSwitch;
		public TemplateFieldInterval(DocumentLogInterval interval, string templateSwitch) {
			Guard.ArgumentNotNull(interval, "interval");
			this.interval = interval;
			this.templateSwitch = templateSwitch;
		}
		public DocumentLogInterval Interval { get { return interval; } }
		public string TemplateSwitch { get { return templateSwitch; } }
	}
	public abstract class TemplatedFieldBase : CalculatedFieldBase {
		public static readonly string TemplateSwitch = "t";		
		public DocumentLogInterval TemplateInterval { get; set; }
		public string DataSourceName { get; set; }
		protected override Dictionary<string, bool> SwitchesWithArguments { get { Exceptions.ThrowInternalException(); return null; } }
		public override bool IsSwitchWithArgument(string fieldSpecificSwitch) {
			return true;
		}
		public override void Initialize(PieceTable pieceTable, InstructionCollection switches) {
			base.Initialize(pieceTable, switches);
			TemplateInterval = switches.GetSwitchArgumentDocumentInterval(TemplateSwitch);
			DataSourceName = switches.GetArgumentAsString(0);
		}
		protected override FieldResultOptions GetCharacterFormatFlag() {
			FieldResultOptions result = base.GetCharacterFormatFlag();
			if (result == FieldResultOptions.None)
				return FieldResultOptions.DoNotApplyFieldCodeFormatting;
			return result;
		}	   
		protected DocumentLogInterval AppendResult(PieceTable sourcePieceTable, PieceTable targetPieceTable, DocumentLogInterval sourceInterval, IFieldContext itemDataContext, bool pageBreakBefore) {			
			DocumentModelPosition targetPosition = DocumentModelPosition.FromRunStart(targetPieceTable, new RunIndex(targetPieceTable.Runs.Count - 1));
			if (targetPieceTable.DocumentModel.ModelForExport) {
				((SnapDocumentModel)targetPieceTable.DocumentModel).SetRootDataContext(itemDataContext);
				CopyHelper.CopyCore(sourcePieceTable, targetPieceTable, sourceInterval, targetPosition.LogPosition, true, false, UpdateFieldOperationType.Normal);
			}
			else
				CopyHelper.CopyCore(sourcePieceTable, targetPieceTable, sourceInterval, targetPosition.LogPosition, true);
			if (pageBreakBefore)
				targetPieceTable.Paragraphs[targetPosition.ParagraphIndex].PageBreakBefore = true;
			return new DocumentLogInterval(targetPosition.LogPosition, targetPieceTable.DocumentEndLogPosition - targetPosition.LogPosition);
		}
		protected override CalculatedFieldBase.FieldMailMergeType MailMergeType() {
			return CalculatedFieldBase.FieldMailMergeType.MailMerge;
		}
		protected bool IsInsideParentFieldCode(Field field) {
			Field parent = field.Parent;
			if (parent == null)
				return false;
			return parent.Code.Contains(field.Code.Start);
		}
		protected void InsertSeparator(SnapPieceTable targetPieceTable, DocumentLogPosition position, bool hidden) {
			targetPieceTable.InsertSeparator(position);
			targetPieceTable.LastInsertedSeparatorRunInfo.Run.Hidden = hidden;
		}
		public virtual void ProcessValue(PieceTable sourcePieceTable, Field documentField, Action<FieldDataValueDescriptor> processAction) {
			IFieldDataAccessService fieldDataAccessService = sourcePieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if (fieldDataAccessService == null)
				return;
			SnapPieceTable snapSourcePieceTable = (SnapPieceTable)sourcePieceTable;
			FieldDataValueDescriptor fieldDataValueDescriptor = fieldDataAccessService.GetFieldValueDescriptor(snapSourcePieceTable, documentField, DataSourceName);
			ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(((SnapDocumentModel)sourcePieceTable.DocumentModel).DataSourceDispatcher);			
			try {
				processAction(fieldDataValueDescriptor);
			}
			finally {
				fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);		  
			}
		}
		public override CalculatedFieldValue Update(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			return base.Update(sourcePieceTable, mailMergeDataMode, documentField).AddOptions(FieldResultOptions.SuppressUpdateInnerCodeFields);
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			SnapPieceTable snapSourcePieceTable = (SnapPieceTable)sourcePieceTable;
			IFieldDataAccessService fieldDataAccessService = sourcePieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			Action<FieldDataValueDescriptor> processAction = delegate(FieldDataValueDescriptor fieldDataValueDescriptor) {
				ResultItemInfoCollection resultItemInfos = GetResultItems(snapSourcePieceTable, fieldDataAccessService, fieldDataValueDescriptor, targetModel.ModelForExport, documentField);
				targetModel.InheritDataServices(sourcePieceTable.DocumentModel);
				CalculateDocumentVariableEventRouter router = new CalculateDocumentVariableEventRouter(sourcePieceTable.DocumentModel);
				targetModel.CalculateDocumentVariable += router.OnCalculateDocumentVariable;
				try {
					InsertResultItems(snapSourcePieceTable, (SnapPieceTable)targetModel.MainPieceTable, resultItemInfos);
					if (!targetModel.ModelForExport)
						targetModel.UpdateFields(UpdateFieldOperationType.Normal, null);
				}
				finally {
					targetModel.CalculateDocumentVariable -= router.OnCalculateDocumentVariable;
				}
			};
			ProcessValue(sourcePieceTable, documentField, processAction);
			return new CalculatedFieldValue(targetModel);
		}
		protected virtual void InsertResultItems(SnapPieceTable sourcePieceTable, SnapPieceTable targetPieceTable, ResultItemInfoCollection resultItemInfos) {
			List<DocumentLogInterval> bookmarkIntervals = new List<DocumentLogInterval>();
			int count = resultItemInfos.Count;
			List<Table> tablesToJoin = new List<Table>(count);
			bool modelForExport = targetPieceTable.DocumentModel.ModelForExport;
			IExportService exportService = targetPieceTable.DocumentModel.GetService<IExportService>();
			for (int i = 0; i < count; i++) {
				ResultItemInfo info = resultItemInfos[i];
				bool insertParagraphBefore = i == 0 && (info.TemplateInfo.StartType == TemplateStartEndType.Table || info.TemplateInfo.StartType == TemplateStartEndType.Text);
				if (insertParagraphBefore) {
					Paragraph paragraph = targetPieceTable.InsertParagraph(DocumentLogPosition.Zero);
					targetPieceTable.Runs[paragraph.LastRunIndex].Hidden = true;
				}
				if (exportService != null)
					exportService.TemplateInfo = CreateSnapTemplateInfo(info);
				DocumentLogInterval resultInterval = AppendResult(sourcePieceTable, targetPieceTable, info.TemplateInfo.ActualInterval, info.DataContext, info.PageBreakBefore);
				if (!modelForExport) {
					InsertSeparator(targetPieceTable, resultInterval.Start, true);
					resultInterval = new DocumentLogInterval(resultInterval.Start, resultInterval.Length + 1);
				}
				Table startOuterTable = GetTableToJoinWithPrevTable(targetPieceTable, resultInterval, info.TemplateInfo, tablesToJoin);
				if (startOuterTable != null) {
					AddTablesToJoin(tablesToJoin, startOuterTable);					
				}
				else
					JoinTablesIfNeeded(targetPieceTable, tablesToJoin);
				Table endOuterTable = GetEndOuterTable(targetPieceTable, resultInterval, info.TemplateInfo);
				if (endOuterTable != startOuterTable) {
					JoinTablesIfNeeded(targetPieceTable, tablesToJoin);
					if (endOuterTable != null)
						AddTablesToJoin(tablesToJoin, endOuterTable);
				}
				bookmarkIntervals.Add(resultInterval);
			}
			JoinTablesIfNeeded(targetPieceTable, tablesToJoin);
			if (!targetPieceTable.DocumentModel.ModelForExport) {
				Debug.Assert(bookmarkIntervals.Count == count);
				List<SnapBookmark> bookmarks = new List<SnapBookmark>(count);
				for (int i = 0; i < count; i++) {
					DocumentLogInterval interval = bookmarkIntervals[i];
					ResultItemInfo info = resultItemInfos[i];
					SnapTemplateInfo templateInfo = CreateSnapTemplateInfo(info);
					bookmarks.Add(targetPieceTable.CreateSnapBookmark(interval.Start, interval.Length, info.DataContext, info.TemplateInfo.Interval, sourcePieceTable, templateInfo));
				}				
				SnapBookmark firstListBookmark = count > 0 ? bookmarks[0] : null;
				SnapBookmark lastListBookmark = count > 0 ? bookmarks[bookmarks.Count - 1] : null;
				for (int i = 0; i < count; i++) {
					SnapTemplateInfo templateInfo = bookmarks[i].TemplateInterval.TemplateInfo;
					ResultItemInfo info = resultItemInfos[i];
					ItemsIntervalInfo intervalInfo = info.IntervalInfo;
					if (intervalInfo != null) {
						templateInfo.FirstGroupBookmark = bookmarks[intervalInfo.StartGroupIndex];
						templateInfo.LastGroupBookmark = bookmarks[Math.Min(intervalInfo.EndGroupIndex, bookmarks.Count - 1)];
#if DEBUGTEST || DEBUG
						if(templateInfo.FirstGroupBookmark.Start > templateInfo.LastGroupBookmark.Start)
							Exceptions.ThrowInternalException();
#endif
						templateInfo.FirstListBookmark = firstListBookmark;
						templateInfo.LastListBookmark = lastListBookmark;
					}
				}
			}
		}
		void AddTablesToJoin(List<Table> tablesToJoin, Table table) {
			int count = tablesToJoin.Count;
			if(count== 0 || tablesToJoin[count - 1] != table)
				tablesToJoin.Add(table);
		}
		SnapTemplateInfo CreateSnapTemplateInfo(ResultItemInfo info) {
			SnapTemplateInfo templateInfo = new SnapTemplateInfo(info.TemplateInfo.TemplateType);
			ItemsIntervalInfo intervalInfo = info.IntervalInfo;
			if (intervalInfo != null) {
				templateInfo.FirstGroupIndex = intervalInfo.GroupPropertiesIndex;
				templateInfo.FieldInGroupCount = intervalInfo.FieldInGroupCount;
			}
			return templateInfo;
		}
		public abstract List<TemplateFieldInterval> GetTemplateIntervals(PieceTable pieceTable);
		private void JoinTablesIfNeeded(PieceTable pieceTable, List<Table> tablesToJoin) {
			if (tablesToJoin.Count >= 2) {
				pieceTable.JoinTables(tablesToJoin.ToArray());
			}
			tablesToJoin.Clear();
		}
		protected virtual Table GetTableToJoinWithPrevTable(PieceTable targetPieceTable, DocumentLogInterval resultInterval, TemplateInfo info, List<Table> tablesToJoin) {
			if(tablesToJoin.Count == 0 || info.StartType != TemplateStartEndType.Table)
				return null;
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(targetPieceTable, resultInterval.Start);
			return GetOuterTable(targetPieceTable, position.ParagraphIndex);
		}		
		protected virtual Table GetEndOuterTable(SnapPieceTable targetPieceTable, DocumentLogInterval resultInterval, TemplateInfo info) {
			if (info.EndType != TemplateStartEndType.Table)
				return null;
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(targetPieceTable, resultInterval.Start + resultInterval.Length - 1);
			return GetOuterTable(targetPieceTable, position.ParagraphIndex);
		}
		protected virtual Table GetOuterTable(PieceTable pieceTable, ParagraphIndex paragraphIndex) {
			TableCell startCell = pieceTable.Paragraphs[paragraphIndex].GetCell();
			if (startCell == null)
				return null;
			Table result = startCell.Table;
			while (result.ParentCell != null)
				result = result.ParentCell.Table;
			return result;
		}
		protected internal override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options) {			
			return UpdateFieldOperationType.Normal | UpdateFieldOperationType.Copy | UpdateFieldOperationType.CreateModelForExport;
		}
		protected abstract ResultItemInfoCollection GetResultItems(SnapPieceTable sourcePieceTable, IFieldDataAccessService fieldDataAccessService, FieldDataValueDescriptor fieldDataValueDescriptor, bool modelForExport, Field documentField);
		protected virtual void AddTemplateIntervalIfExists(List<TemplateFieldInterval> target, string templateSwitch) {
			if (String.IsNullOrEmpty(templateSwitch))
				return;
			DocumentLogInterval interval = Switches.GetSwitchArgumentDocumentInterval(templateSwitch);
			if (interval != null)
				target.Add(new TemplateFieldInterval(interval, templateSwitch));
		}
		public virtual void OnRemoveTemplateSwitches(PieceTable pieceTable, InstructionController instructionController, List<string> templateSwitches) {			
		}
		public override bool IsSwitchArgumentField(string fieldSpecificSwitch) {
			return true;
		}
	}
}
