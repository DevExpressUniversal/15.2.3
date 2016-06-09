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
using DevExpress.XtraRichEdit.Model;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class SnapFieldDataService : IFieldDataService {		
		class ConvertToDoubleEnumerator : IEnumerator<double> {
			IFieldContext parentDataContext;
			IDataEnumerator dataEnumerator;
			public ConvertToDoubleEnumerator(SnapPieceTable pieceTable, IFieldDataAccessService fieldDataAccessService, IFieldContext parentDataContext, IDataEnumerator dataEnumerator) {
				Guard.ArgumentNotNull(pieceTable, "pieceTable");
				Guard.ArgumentNotNull(fieldDataAccessService, "fieldDataAccessService");
				Guard.ArgumentNotNull(parentDataContext, "parentDataContext");
				Guard.ArgumentNotNull(dataEnumerator, "dataEnumerator");
				this.dataEnumerator = dataEnumerator;
				this.parentDataContext = parentDataContext;
			}
			public double Current {
				get { return Convert.ToDouble(dataEnumerator.Current); }
			}
			#region IDisposable
			public void Dispose() {
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			~ConvertToDoubleEnumerator() {
				Dispose(false);
			}
			protected virtual void Dispose(bool disposing) {				
				if (disposing) {
					if (parentDataContext != null) {
					}
				}
				parentDataContext = null;
			}
			#endregion
			object IEnumerator.Current {
				get { return this.Current; }
			}
			public bool MoveNext() {
				return dataEnumerator.MoveNext();
			}
			public void Reset() {
				dataEnumerator.Reset();				
			}
		}
		public bool BoundMode { get { return true; } }
		public virtual object GetFieldValue(MailMergeProperties mailMergeProperties, string fieldName, bool mapFieldName, MailMergeDataMode options, PieceTable pieceTable, Field field) {
			IFieldDataAccessService fieldDataAccessService = pieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if (fieldDataAccessService == null)
				return null;
			FieldDataValueDescriptor descriptor = fieldDataAccessService.GetFieldValueDescriptor((SnapPieceTable)pieceTable, field, fieldName);
			ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(((SnapDocumentModel)pieceTable.DocumentModel).DataSourceDispatcher);
			try {
				if(!string.IsNullOrEmpty(fieldName) && fieldName.Contains(".")) {
					List<string> existingFieldNames = new List<string>();
					if(calculationContext.FieldNames != null && calculationContext.FieldNames.Length > 0)
						existingFieldNames.AddRange(calculationContext.FieldNames);
					if(!existingFieldNames.Contains(fieldName)) {
						existingFieldNames.Add(fieldName);
						calculationContext.FieldNames = existingFieldNames.ToArray();
					}
				}
				return calculationContext.GetRawValue(descriptor.ParentDataContext, descriptor.RelativePath);
			}
			finally {
				fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);
			}
		}
		public IEnumerator<double> GetReferenceValuesEnumerator(PieceTable pieceTable, Field field, string reference) {
			if (reference.StartsWith("[") && reference.EndsWith("]"))
				return GetReferenceValuesEnumeratorCore(pieceTable, field, reference.Substring(1, reference.Length - 2));
			return null;
		}
		private IEnumerator<double> GetReferenceValuesEnumeratorCore(PieceTable pieceTable, Field field, string fieldName) {
			SnapPieceTable snapPieceTable = (SnapPieceTable)pieceTable;
			IFieldDataAccessService fieldDataAccessService = pieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if (fieldDataAccessService == null)
				return null;
			FieldDataValueDescriptor fieldDataValueDescriptor = fieldDataAccessService.GetFieldValueDescriptor(snapPieceTable, field, fieldName);
			IFieldContext parentDataContext = fieldDataValueDescriptor.ParentDataContext;
			ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(((SnapDocumentModel)pieceTable.DocumentModel).DataSourceDispatcher);
			try {
				IDataEnumerator enumerator = calculationContext.GetChildDataEnumerator(parentDataContext, fieldDataValueDescriptor.RelativePath);
				if (enumerator == null) {
					return new SingleReferenceValueEnumerator(0);
				}
				return new ConvertToDoubleEnumerator(snapPieceTable, fieldDataAccessService, parentDataContext, enumerator);
			}
			catch {
				fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);
			}
			return new SingleReferenceValueEnumerator(0);
		}
	}
}
