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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class FieldDataAccessService : IFieldDataAccessService, IUpdateFieldService {
		readonly IFieldPathService fieldPathService;
		readonly IFieldContextService fieldContextService;
		SnapDocumentModel documentModel;
		public FieldDataAccessService(SnapDocumentModel documentModel, IFieldPathService fieldPathService, IFieldContextService fieldContextService) {
			Guard.ArgumentNotNull(fieldPathService, "fieldPathService");
			this.documentModel = documentModel;
			this.fieldPathService = fieldPathService;
			this.fieldContextService = fieldContextService;
		}
		public IFieldPathService FieldPathService { get { return fieldPathService; } }
		public IFieldContextService FieldContextService { get { return fieldContextService; } }
		public virtual FieldDataValueDescriptor GetFieldValueDescriptor(SnapPieceTable pieceTable, Field field, string fieldName) {
			return GetFieldValueDescriptorCore(pieceTable, field.FirstRunIndex, fieldName);
		}
		public virtual FieldDataValueDescriptor GetFieldValueDescriptor(SnapPieceTable pieceTable, DocumentLogPosition pos, string fieldName) {
			RunIndex runIndex = PositionConverter.ToDocumentModelPosition(pieceTable, pos).RunIndex;
			return GetFieldValueDescriptorCore(pieceTable, runIndex, fieldName);
		}
		FieldDataValueDescriptor GetFieldValueDescriptorCore(SnapPieceTable pieceTable, RunIndex runIndex, string fieldName) {
			IFieldContext parentFieldContext = null;
			FieldPathInfo fieldPathInfo = FieldPathService.FromString(fieldName);
			if (fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Root) {
				RootFieldDataSourceInfo rootInfo = (RootFieldDataSourceInfo)fieldPathInfo.DataSourceInfo;
				parentFieldContext = new RootFieldContext(pieceTable.DocumentModel.DataSourceDispatcher, rootInfo.Name);
			}
			else {
				parentFieldContext = GetFieldContext(pieceTable, runIndex);
				RelativeFieldDataSourceInfo relativeInfo = fieldPathInfo.DataSourceInfo as RelativeFieldDataSourceInfo;
				if (relativeInfo != null && relativeInfo.RelativeLevel > 0) {
					for(int i = 0; i < relativeInfo.RelativeLevel; i++) {
						ISingleObjectFieldContext singleObjectFieldContext = parentFieldContext as ISingleObjectFieldContext;
						if (singleObjectFieldContext != null) {
							parentFieldContext = singleObjectFieldContext.Parent;
							continue;
						}
						IDataControllerListFieldContext listFieldContext = parentFieldContext as IDataControllerListFieldContext;
						if (listFieldContext != null) {
							parentFieldContext = listFieldContext.Parent;
							continue;
						}
						break;
					}
				}
			}
			return new FieldDataValueDescriptor(parentFieldContext, fieldPathInfo.DataMemberInfo);
		}
		public IFieldContext GetFieldContext(SnapFieldInfo info) {
			return GetFieldContext(info.PieceTable, info.Field.FirstRunIndex);
		}
		protected virtual IFieldContext GetFieldContext(SnapPieceTable pieceTable, RunIndex index) {
			FieldContextOwnerFinder ownerFinder = new FieldContextOwnerFinder();
			IFieldContextOwner owner = ownerFinder.FindInnermostOwnerByPosition(pieceTable, index);
			if (owner == null)
				return ownerFinder.FindRootDataContext(pieceTable);
			return owner.FieldContext;
		}
		public bool CanCalculateValue(DocumentLogPosition pos, SnapPieceTable pieceTable, string fieldName) {			
			FieldDataValueDescriptor descriptor = GetFieldValueDescriptor(pieceTable, pos, fieldName);
			ICalculationContext calculationContext = FieldContextService.BeginCalculation(documentModel.DataSourceDispatcher);
			try {
				object value = calculationContext.GetRawValue(descriptor.ParentDataContext, descriptor.RelativePath);
				return value != FieldNull.Value;
			} finally {
				FieldContextService.EndCalculation(calculationContext);
			}
		}
	}
}
