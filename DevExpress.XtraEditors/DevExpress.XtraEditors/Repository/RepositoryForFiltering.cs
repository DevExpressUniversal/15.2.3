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

namespace DevExpress.XtraEditors.Repository {
	using System;
	using System.ComponentModel.DataAnnotations;
	using DevExpress.Data.Utils;
	using DevExpress.Utils.Filtering;
	using DevExpress.XtraEditors.Filtering.Repository;
	using Utils.Filtering.Internal;
	partial class DefaultEditorsRepository {
		public virtual RepositoryItem GetRepositoryItemForFiltering(Type type, AnnotationAttributes annotationAttributes, DataType? dataType) {
			RepositoryItem item = null;
			object annotationKey = annotationAttributes.Key;
			if(type.IsGenericType) {
				var filterAttributes = new FilterAttributes(annotationAttributes, type);
				Type viewModelTypeDefinition = type.GetGenericTypeDefinition();
				if(viewModelTypeDefinition == typeof(IRangeValueViewModel<>)) {
					var valueType = type.GetGenericArguments()[0];
					if(IsDateTime(valueType)) {
						DateTimeRangeUIEditorType rangeEditorType = DateTimeRangeUIEditorType.Default;
						if(filterAttributes.HasEditorAttribute)
							rangeEditorType = filterAttributes.GetRangeEditorSettings().DateTimeEditorType.GetValueOrDefault();
						item = CreateDefaultFilteringDateTimeRangeEdit(rangeEditorType, dataType);
					}
					else {
						RangeUIEditorType rangeEditorType = RangeUIEditorType.Default;
						if(filterAttributes.HasEditorAttribute)
							rangeEditorType = filterAttributes.GetRangeEditorSettings().NumericEditorType.GetValueOrDefault();
						item = CreateDefaultFilteringRangeEdit(rangeEditorType, dataType);
					}
				}
				if(viewModelTypeDefinition == typeof(ICollectionValueViewModel<>)) {
					LookupUIEditorType editorType = LookupUIEditorType.Default;
					if(filterAttributes.HasEditorAttribute)
						editorType = filterAttributes.GetLookupEditorSettings().EditorType;
					bool useFlags = false;
					if(filterAttributes.HasEditorAttribute)
						useFlags = filterAttributes.GetLookupEditorSettings().UseFlags;
					item = CreateDefaultFilteringLookupEdit(editorType, useFlags);
				}
				if(viewModelTypeDefinition == typeof(ISimpleValueViewModel<>)) {
					BooleanUIEditorType editorType = BooleanUIEditorType.Default;
					if(filterAttributes.HasEditorAttribute)
						editorType = filterAttributes.GetBooleanEditorSettings().EditorType;
					item = CreateDefaultFilteringChoiceEdit(editorType);
				}
				if(viewModelTypeDefinition == typeof(IEnumValueViewModel<>)) {
					var enumType = type.GetGenericArguments()[0];
					LookupUIEditorType editorType = LookupUIEditorType.Default;
					if(filterAttributes.HasEditorAttribute)
						editorType = filterAttributes.GetEnumEditorSettings().EditorType;
					bool useFlags = false;
					if(filterAttributes.HasEditorAttribute)
						useFlags = filterAttributes.GetEnumEditorSettings().UseFlags;
					item = CreateDefaultFilteringEnumChoiceEdit(editorType, enumType, useFlags);
				}
			}
			return item;
		}
		protected virtual RepositoryItem CreateDefaultFilteringRangeEdit(RangeUIEditorType editorType, DataType? dataType) {
			return new RepositoryItemFilterUIEditorContainerEdit(editorType, dataType);
		}
		protected virtual RepositoryItem CreateDefaultFilteringDateTimeRangeEdit(DateTimeRangeUIEditorType editorType, DataType? dataType) {
			return new RepositoryItemFilterUIEditorContainerEdit(editorType, dataType);
		}
		protected virtual RepositoryItem CreateDefaultFilteringLookupEdit(LookupUIEditorType editorType, bool useFlags) {
			return new RepositoryItemFilterUIEditorContainerEdit(editorType, useFlags);
		}
		protected virtual RepositoryItem CreateDefaultFilteringChoiceEdit(BooleanUIEditorType editorType) {
			return new RepositoryItemFilterUIEditorContainerEdit(editorType);
		}
		protected virtual RepositoryItem CreateDefaultFilteringEnumChoiceEdit(LookupUIEditorType editorType, Type enumType, bool useFlags) {
			return new RepositoryItemFilterUIEditorContainerEdit(editorType, enumType, useFlags);
		}
	}
}
