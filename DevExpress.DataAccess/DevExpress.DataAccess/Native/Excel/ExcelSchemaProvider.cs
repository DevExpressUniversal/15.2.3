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
using System.IO;
using System.Linq;
using System.Threading;
using DevExpress.DataAccess.Excel;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
namespace DevExpress.DataAccess.Native.Excel {
	public class ExcelSchemaProvider : IExcelSchemaProvider {
		int testRowCount = 10;
		const string columnBaseName = "Column";
		#region Implementation of IExcelSchemaProvider
		public FieldInfo[] GetSchema(string fileName, Stream stream, ExcelDocumentFormat format, ExcelSourceOptionsBase optionsBase, CancellationToken token) {
			using(ISpreadsheetSource spreadsheetSource = ExcelDataLoaderHelper.CreateSource(stream, format, fileName, optionsBase)) {
				int effectiveTestCount = optionsBase.UseFirstRowAsHeader ? testRowCount + 1 : testRowCount;
				int columnCount = ExcelDataLoaderHelper.GetColumnCount(spreadsheetSource, effectiveTestCount, optionsBase);
				ISpreadsheetDataReader dataReader = spreadsheetSource.CreateReader(optionsBase);
				List<FieldInfo> schema = new List<FieldInfo>(columnCount);
				Predicate<string> exist = s => schema.Any(fi => fi.Name == s);
				if(optionsBase.UseFirstRowAsHeader) {
					dataReader.Read();
					for(int i = 0; i < dataReader.FieldsCount; i++) {
						token.ThrowIfCancellationRequested();
						schema.Add(new FieldInfo {
							Name = dataReader.GetFieldType(i) != XlVariantValueType.None
								? ExcelDataLoaderHelper.GetFieldName(exist, dataReader.GetString(i), columnBaseName + (i + 1))
								: ExcelDataLoaderHelper.GetFieldName(exist, columnBaseName + (i + 1))
						});
					}
				}
				for(int i = schema.Count; i < columnCount; i++) {
					schema.Add(new FieldInfo { Name = ExcelDataLoaderHelper.GetFieldName(exist, columnBaseName + (i + 1)) });
				}
				int count = effectiveTestCount;
				while(dataReader.Read() && count > 0) {
					for(int i = 0; i < dataReader.FieldsCount; i++) {
						token.ThrowIfCancellationRequested();
						Type cellType = ExcelDataLoaderHelper.GetType(dataReader.GetFieldType(i));
						schema[i].Type = cellType == null
							? schema[i].Type
							: schema[i].Type == null
								? cellType
								: cellType == schema[i].Type
									? cellType
									: typeof(string);
					}
					count--;
				}
				foreach(FieldInfo field in schema.Where(field => field.Type == null)) {
					field.Type = typeof(string);
				}
				token.ThrowIfCancellationRequested();
				return schema.ToArray();
			}
		}
		#endregion
	}
}
