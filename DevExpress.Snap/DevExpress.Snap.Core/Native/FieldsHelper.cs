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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Fields;
using FieldPathService = DevExpress.Snap.Core.Native.Data.Implementations.FieldPathService;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Native {
	public static class FieldsHelper {
		public static int GetLevel(this Field parentField) {
			Field field = parentField;
			int result = 0;
			while (field != null) {
				if(!field.DisableUpdate)					
					result++;
				field = field.Parent;
			}
			return result;
		}
		public static CalculatedFieldBase GetParentParsedInfo(SnapDocumentModel documentModel) {
			SnapFieldInfo fieldInfo = GetSelectedField(documentModel);
			if (fieldInfo == null || fieldInfo.Field.Parent == null)
				return null;
			return GetParsedInfoCore(fieldInfo.PieceTable, fieldInfo.Field.Parent);
		}
		public static CalculatedFieldBase GetParsedInfo(SnapDocumentModel documentModel) {
			SnapFieldInfo fieldInfo = GetSelectedField(documentModel);
			if (fieldInfo == null)
				return null;
			return GetParsedInfoCore(fieldInfo.PieceTable, fieldInfo.Field);
		}
		internal static CalculatedFieldBase GetParsedInfoCore(PieceTable pieceTable, Field field) {
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();			
			return calculator.ParseField(pieceTable, field);
		}
		public static SnapFieldInfo GetSelectedField(SnapDocumentModel documentModel) {
			FieldController controller = new FieldController();
			Field field = controller.FindTopmostFieldBySelection(documentModel.Selection);
			if (field == null)
				return null;
			return new SnapFieldInfo((SnapPieceTable)documentModel.Selection.PieceTable, field);
		}
		public static SnapListFieldInfo GetSelectedSNListField(SnapDocumentModel documentModel) {
			Selection selection = documentModel.Selection;
			PieceTable pieceTable = selection.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, Algorithms.Min(selection.NormalizedStart, pieceTable.DocumentEndLogPosition));
			Field startField = selection.PieceTable.FindFieldByRunIndex(start.RunIndex);
			if (startField == null)
				return null;
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(pieceTable, Algorithms.Min(selection.NormalizedEnd, pieceTable.DocumentEndLogPosition));
			Field endField = selection.PieceTable.FindFieldByRunIndex(end.RunIndex);
			if (endField == null)
				return null;
			SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
			while (startField != null) {
				SNListField snListField = calculator.ParseField((SnapPieceTable)selection.PieceTable, startField) as SNListField;
				if (snListField != null)
					return new SnapListFieldInfo((SnapPieceTable)selection.PieceTable, startField, snListField);
				startField = startField.Parent;
			}
			return null;
		}
		public static DesignBinding GetFieldDesignBindingEx(IDataSourceInfoContainer dataSourceInfoContainer, IFieldInfo fieldInfo) {
			CalculatedFieldBase calculatedFieldBase = FieldsHelper.GetParsedInfoCore(fieldInfo.PieceTable, fieldInfo.Field);
			if(calculatedFieldBase is SNTextField && ((SNTextField)calculatedFieldBase).IsParameter)
				return new DesignBinding(fieldInfo.PieceTable.DocumentModel.ParametersDataSource, ((SNTextField)calculatedFieldBase).DataFieldName);
			return GetFieldDesignBinding(fieldInfo.PieceTable.DocumentModel.DataSources, fieldInfo);
		}
		public static DesignBinding GetFieldDesignBinding(IDataSourceInfoContainer dataSourceInfoContainer, IFieldInfo fieldInfo) {
			return GetFieldDesignBinding(dataSourceInfoContainer, fieldInfo, String.Empty);
		}
		public static DesignBinding GetFieldDesignBinding(IDataSourceInfoContainer dataSourceInfoContainer, IFieldInfo fieldInfo, string dataMemberName) {
			SnapDocumentModel model = fieldInfo.PieceTable.DocumentModel;
			IFieldDataAccessService dataAccessService = model.GetService<IFieldDataAccessService>();
			if (dataAccessService == null)
				return null;
			SnapPieceTable pieceTable = fieldInfo.PieceTable;
			Field current = fieldInfo.Field;
			if (current == null)
				return null;
			current = current.DisableUpdate ? GetParent(current) : current;
			while (current != null) {
				CalculatedFieldBase parsedInfo = GetParsedInfoCore(pieceTable, current);
				IDataFieldNameOwner fieldNameOwner = parsedInfo as IDataFieldNameOwner;
				if (fieldNameOwner == null)
					break;
				SNSparklineField sparklineField = parsedInfo as SNSparklineField;
				string fieldName = fieldNameOwner.DataFieldName;
				if (sparklineField != null)
					fieldName = string.IsNullOrEmpty(sparklineField.DataSourceName) ? sparklineField.DataFieldName : string.Format("{0}.{1}", sparklineField.DataSourceName, sparklineField.DataFieldName);
				FieldPathInfo fieldPathInfo = dataAccessService.FieldPathService.FromString(fieldName);
				string fullPath = fieldPathInfo.GetFullPath();
				if (!String.IsNullOrEmpty(dataMemberName) && !String.IsNullOrEmpty(fullPath))
					dataMemberName = "." + dataMemberName;
				dataMemberName = fullPath + dataMemberName;
				if (fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Root) {
					return new DesignBinding(dataSourceInfoContainer.GetDataSourceByName(((RootFieldDataSourceInfo)fieldPathInfo.DataSourceInfo).Name), dataMemberName);
				}
				current = GetParent(current);
			}
			if (model.SnapMailMergeVisualOptions.DataSourceName != null)
				return new DesignBinding(model.SnapMailMergeVisualOptions.DataSource, dataMemberName);
			if (dataSourceInfoContainer.DefaultDataSourceInfo == null)
				return new DesignBinding(null, string.Empty);
			return new DesignBinding(dataSourceInfoContainer.DefaultDataSourceInfo.DataSource, dataMemberName);
		}
		public static DesignBinding GetFieldDesignBinding(IDataSourceDispatcher dataSourceDispatcher, IFieldInfo fieldInfo) {
			return GetFieldDesignBinding(dataSourceDispatcher, fieldInfo, String.Empty);
		}
		public static DesignBinding GetFieldDesignBinding(IDataSourceDispatcher dataSourceDispatcher, IFieldInfo fieldInfo, string dataMemberName) {
			SnapDocumentModel model = fieldInfo.PieceTable.DocumentModel;
			IFieldDataAccessService dataAccessService = model.GetService<IFieldDataAccessService>();
			if(dataAccessService == null)
				return null;
			SnapPieceTable pieceTable = fieldInfo.PieceTable;
			Field current = fieldInfo.Field;
			if(current == null)
				return null;
			current = current.DisableUpdate ? GetParent(current) : current;
			while(current != null) {
				CalculatedFieldBase parsedInfo = GetParsedInfoCore(pieceTable, current);
				IDataFieldNameOwner fieldNameOwner = parsedInfo as IDataFieldNameOwner;
				if(fieldNameOwner == null)
					break;
				SNSparklineField sparklineField = parsedInfo as SNSparklineField;
				string fieldName = fieldNameOwner.DataFieldName;
				if(sparklineField != null)
					fieldName = string.IsNullOrEmpty(sparklineField.DataSourceName) ? sparklineField.DataFieldName : string.Format("{0}.{1}", sparklineField.DataSourceName, sparklineField.DataFieldName);
				FieldPathInfo fieldPathInfo = dataAccessService.FieldPathService.FromString(fieldName);
				string fullPath = fieldPathInfo.GetFullPath();
				if(!String.IsNullOrEmpty(dataMemberName) && !String.IsNullOrEmpty(fullPath))
					dataMemberName = "." + dataMemberName;
				dataMemberName = fullPath + dataMemberName;
				if(fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Root) {
					return new DesignBinding(dataSourceDispatcher.GetDataSource(((RootFieldDataSourceInfo)fieldPathInfo.DataSourceInfo).Name), dataMemberName);
				}
				current = GetParent(current);
			}
			SnapMailMergeVisualOptions mailMergeOptions = model.SnapMailMergeVisualOptions;
			if (mailMergeOptions.DataSourceName != null) {
				if (!String.IsNullOrEmpty(mailMergeOptions.DataMember))
					if (!String.IsNullOrEmpty(dataMemberName))
						dataMemberName = mailMergeOptions.DataMember + "." + dataMemberName;
					else
						dataMemberName = mailMergeOptions.DataMember;
				return new DesignBinding(model.SnapMailMergeVisualOptions.DataSource, dataMemberName);
			}
			object defaultDataSource = dataSourceDispatcher.DefaultDataSource;
			if(object.ReferenceEquals(defaultDataSource, null))
				return new DesignBinding(null, string.Empty);
			return new DesignBinding(defaultDataSource, dataMemberName);
		}
		public static string GetFieldDisplayName(IDataSourceDispatcher dataSourceDispatcher, SnapListFieldInfo listFieldInfo, string dataFieldName, string defaultName) {
			DesignBinding binding = GetFieldDesignBinding(dataSourceDispatcher, listFieldInfo);
			IDataAccessService service = listFieldInfo.PieceTable.DocumentModel.GetService<IDataAccessService>();
			if (binding == null || service == null)
				return defaultName;
			string dataMember = binding.DataMember == null ? string.Empty : binding.DataMember;
			return FieldPathService.DecodePath(service.GetFieldDisplayName(binding.DataSource, dataMember, dataFieldName));
		}
		public static DesignBinding GetFieldDesignBindingEx(IDataSourceDispatcher dataSourceDispatcher, IFieldInfo fieldInfo) {
			CalculatedFieldBase calculatedFieldBase = FieldsHelper.GetParsedInfoCore(fieldInfo.PieceTable, fieldInfo.Field);
			if (calculatedFieldBase is SNTextField && ((SNTextField)calculatedFieldBase).IsParameter)
				return new DesignBinding(fieldInfo.PieceTable.DocumentModel.ParametersDataSource, ((SNTextField)calculatedFieldBase).DataFieldName);
			return GetFieldDesignBinding(dataSourceDispatcher, fieldInfo);
		}
		public static Field GetParent(Field field) {
			Field parent = field.Parent;
			while (parent != null && parent.DisableUpdate) {
				parent = parent.Parent;
			}
			return parent;
		}
		public static string GetFieldDisplayName(IDataSourceInfoContainer dataSourceInfoContainer, SnapListFieldInfo listFieldInfo, string dataFieldName, string defaultName) {
			DesignBinding binding = GetFieldDesignBinding(dataSourceInfoContainer, listFieldInfo);
			IDataAccessService service = listFieldInfo.PieceTable.DocumentModel.GetService<IDataAccessService>();
			if (binding == null || service == null)
				return defaultName;
			string dataMember = binding.DataMember == null ? string.Empty : binding.DataMember;
			return FieldPathService.DecodePath(service.GetFieldDisplayName(binding.DataSource, dataMember, dataFieldName));
		}
		public static bool IsParagraphWithinFieldCode(Paragraph paragraph) {
			RunIndex runIndex = paragraph.FirstRunIndex;
			Field field = paragraph.PieceTable.FindFieldByRunIndex(runIndex);
			if (field != null && paragraph.FirstRunIndex == field.Code.Start)
				field = field.Parent;
			while (field != null) {
				if (runIndex >= field.Result.Start)
					field = field.Parent;
				else
					return true;
			}
			return false;
		}
		public static Field FindFieldByParagraph(Paragraph paragraph) {
			return paragraph.PieceTable.FindFieldByRunIndex(paragraph.FirstRunIndex);
		}
		public static Field FindFieldByHitTestResult(RichEditHitTestResult hitTestResult) {
			FieldController fieldController = new FieldController();
			return fieldController.FindFieldByHitTestResult(hitTestResult);
		}
		public static bool IsNotResizableField(HotZone hotZone) {
			RectangularObjectHotZone rectangularObjectZone = hotZone as RectangularObjectHotZone;
			if (rectangularObjectZone != null) {
				PieceTable pieceTable = rectangularObjectZone.PieceTable;
				Field field = pieceTable.FindFieldByRunIndex(rectangularObjectZone.Box.StartPos.RunIndex);
				if (field != null) {
					SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
					SNMergeFieldBase parsedInfo = calculator.ParseField(pieceTable, field) as SNMergeFieldBase;
					if (parsedInfo != null && !parsedInfo.CanResize)
						return true;
				}
			}
			return false;
		}
		public static string GetSummaryItemTypeDisplayString(DevExpress.Data.SummaryItemType summaryItemType) {
			EnumHelper enumConverter = new EnumHelper(typeof(DevExpress.Data.SummaryItemType));
			return enumConverter.ConvertTo(summaryItemType);
		}
		public static string GetSummaryItemTypeCodeString(DevExpress.Data.SummaryItemType summaryItemType) {
			return Enum.GetName(typeof(DevExpress.Data.SummaryItemType), summaryItemType);
		}
	}
	public interface IFieldInfo {
		Field Field { get; }
		SnapPieceTable PieceTable { get; }
	}
	public class SnapFieldInfoBase<T> : IFieldInfo where T : CalculatedFieldBase {
		readonly Field field;
		readonly SnapPieceTable pieceTable;
		T parsedInfo;
		public SnapFieldInfoBase(SnapPieceTable pieceTable, Field field) : this(pieceTable, field, null) {
		}
		public SnapFieldInfoBase(SnapPieceTable pieceTable, Field field, T parsedInfo) {
			this.pieceTable = pieceTable;
			this.field = field;
			this.parsedInfo = parsedInfo;
		}
		public Field Field { get { return field; } }
		public SnapPieceTable PieceTable { get { return pieceTable; } }
		public T ParsedInfo {
			get {
				if (parsedInfo == null) {
					SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
					parsedInfo = calculator.ParseField(pieceTable, field) as T;					
				}
				return parsedInfo;
			}
		}
		public InstructionController CreateFieldInstructionController() {
			return PieceTable.CreateFieldInstructionController(Field);
		}
	}
	public class SnapListFieldInfo : SnapFieldInfoBase<SNListField> {
		public SnapListFieldInfo(SnapPieceTable pieceTable, Field field)
			: base(pieceTable, field) {
		}
		public SnapListFieldInfo(SnapPieceTable pieceTable, Field field, SNListField parsedInfo)
			: base(pieceTable, field, parsedInfo) {
		}
	}
	public class SnapFieldInfo : SnapFieldInfoBase<CalculatedFieldBase> {
		public SnapFieldInfo(SnapPieceTable pieceTable, Field field)
			: this(pieceTable, field, null) {
		}
		public SnapFieldInfo(SnapPieceTable pieceTable, Field field, CalculatedFieldBase parsedInfo)
			: base(pieceTable, field, parsedInfo) {
		}
		public SnapFieldInfo GetParentInfo() {
			if (Field.Parent == null)
				return null;
			return new SnapFieldInfo(PieceTable, Field.Parent);
		}
	}
	public class EnumHelper {
		System.Resources.ResourceManager resourceManager;
		Type enumType;
		public EnumHelper(Type enumType) {
			Type resourceFinder = typeof(DevExpress.Data.ResFinder);
			string resourceFileName = System.ComponentModel.DXDisplayNameAttribute.DefaultResourceFile;
			resourceManager = new System.Resources.ResourceManager(string.Concat(resourceFinder.Namespace, ".", resourceFileName), resourceFinder.Assembly);
			this.enumType = enumType;
		}
		internal static string GetResourceName(object enumValue) {
			return string.Concat(enumValue.GetType().FullName, ".", enumValue);
		}
		public string ConvertTo(object enumValue) {
			string displayName = resourceManager.GetString(GetResourceName(enumValue));
			return displayName;
		}
		public object ConvertFrom(string value) {
			if(enumType.BaseType != typeof(Enum))
				return null;
			foreach(var item in enumType.GetEnumValues()) {
				var resourceDisplayName = ConvertTo(item);
				if(String.Equals(resourceDisplayName, value))
					return item;
			}
			return null;
		}
	}
}
