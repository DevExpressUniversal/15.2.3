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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Globalization;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal void GenerateDataReference(string tag, bool isNumber, IDataReference data) {
			data.ResetCachedValue();
			DataReferenceType referenceType = data.GetReferenceType(isNumber);
			if (referenceType == DataReferenceType.None)
				return;
			ChartDataReference dataReference = (ChartDataReference)data;
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				switch (referenceType) {
					case DataReferenceType.MultiLevelString:
						GenerateMultilevelStringReference(dataReference);
						break;
					case DataReferenceType.Number:
						GenerateNumberReference(dataReference);
						break;
					case DataReferenceType.NumberLiteral:
						GenerateNumberLiteralReference(dataReference);
						break;
					case DataReferenceType.String:
						GenerateStringReference(dataReference);
						break;
					case DataReferenceType.StringLiteral:
						GenerateStringLiteralReference(dataReference);
						break;
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateStringLiteralReference(ChartDataReference dataReference) {
			OpenXmlStringReference reference = new OpenXmlStringReference();
			reference.FillFromDataReference(dataReference);
			try {
				WriteStartElement("strLit", DrawingMLChartNamespace);
				int pointCount = reference.Points == null ? 0 : reference.Points.Count;
				if (pointCount > 0) {
					int lastIndex = reference.Points[pointCount - 1].Index;
					GenerateChartSimpleIntAttributeTag("ptCount", lastIndex + 1);
					for (int i = 0; i < pointCount; i++)
						GenerateChartStringPoint(reference.Points[i]);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateStringReference(ChartDataReference dataReference) {
			OpenXmlStringReference reference = new OpenXmlStringReference();
			reference.FillFromDataReference(dataReference);
			try {
				WriteStartElement("strRef", DrawingMLChartNamespace);
				GenerateChartSimpleStringTag("f", reference.FormulaBody);
				GenerateChartStringCache(reference);
			}
			finally {
				WriteEndElement();
			}
		}
		#region Multilevel
		void GenerateMultilevelStringReference(ChartDataReference dataReference) {
			OpenXmlChartMLDataReference reference = new OpenXmlChartMLDataReference();
			reference.FillFromDataReference(dataReference);
			try {
				WriteStartElement("multiLvlStrRef", DrawingMLChartNamespace);
				GenerateChartSimpleStringTag("f", reference.FormulaBody);
				GenerateChartMlStringCache(reference);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		void GenerateNumberReference(ChartDataReference dataReference) {
			OpenXmlNumberReference reference = new OpenXmlNumberReference();
			reference.FillFromDataReference(dataReference);
			try {
				WriteStartElement("numRef", DrawingMLChartNamespace);
				GenerateChartSimpleStringTag("f", reference.FormulaBody);
				GenerateChartNumberCache(reference);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateNumberLiteralReference(ChartDataReference dataReference) {
			OpenXmlNumberReference reference = new OpenXmlNumberReference();
			reference.FillFromDataReference(dataReference);
			try {
				WriteStartElement("numLit", DrawingMLChartNamespace);
				if (!string.IsNullOrEmpty(reference.FormatCode))
					GenerateChartSimpleStringTag("formatCode", reference.FormatCode);
				int pointCount = reference.Points == null ? 0 : reference.Points.Count;
				if (pointCount > 0) {
					int lastIndex = reference.Points[pointCount - 1].Index;
					GenerateChartSimpleIntAttributeTag("ptCount", lastIndex + 1);
					for (int i = 0; i < pointCount; i++)
						GenerateChartNumberPoint(reference.Points[i]);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartNumberCache(OpenXmlNumberReference reference) {
			try {
				WriteStartElement("numCache", DrawingMLChartNamespace);
				if (!string.IsNullOrEmpty(reference.FormatCode))
					GenerateChartSimpleStringTag("formatCode", reference.FormatCode);
				int pointCount = reference.Points == null ? 0 : reference.Points.Count;
				if (pointCount > 0) {
					int lastIndex = reference.Points[pointCount - 1].Index;
					GenerateChartSimpleIntAttributeTag("ptCount", lastIndex + 1);
					for (int i = 0; i < pointCount; i++)
						GenerateChartNumberPoint(reference.Points[i]);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartNumberPoint(NumberPoint numberPoint) {
			if (string.IsNullOrEmpty(numberPoint.Value))
				return;
			try {
				WriteStartElement("pt", DrawingMLChartNamespace);
				WriteIntValue("idx", numberPoint.Index);
				if (!string.IsNullOrEmpty(numberPoint.FormatCode))
					WriteStringValue("formatCode", numberPoint.FormatCode);
				GenerateChartSimpleStringTag("v", numberPoint.Value);
			}
			finally {
				WriteEndElement();
			}
		}
		#region String cache
		void GenerateChartMlStringCache(OpenXmlChartMLDataReference reference) {
			try {
				WriteStartElement("multiLvlStrCache", DrawingMLChartNamespace);
				int levelCount = reference.Levels == null ? 0 : reference.Levels.Count;
				if (levelCount > 0) {
					int maxIndex = 0;
					for (int i = 0; i < levelCount; i++)
						maxIndex = Math.Max(maxIndex, reference.Levels[i].Points.Count);
					GenerateChartSimpleIntAttributeTag("ptCount", maxIndex);
					for (int i = 0; i < levelCount; i++)
						GenerateChartMlStringCacheLevel(reference.Levels[i]);
				}
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartMlStringCacheLevel(OpenXmlChartMLReferenceLevel level) {
			int pointCount = level.Points.Count;
			if (pointCount == 0)
				return;
			try {
				WriteStartElement("lvl", DrawingMLChartNamespace);
				GenerateChartStringPoints(level.Points, false);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartStringCache(OpenXmlStringReference reference) {
			try {
				WriteStartElement("strCache", DrawingMLChartNamespace);
				GenerateChartStringPoints(reference.Points, true);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartStringPoints(IList<StringPoint> points, bool includePointCount) {
			int pointCount = points == null ? 0 : points.Count;
			if (pointCount > 0) {
				int lastIndex = points[pointCount - 1].Index;
				if (includePointCount)
					GenerateChartSimpleIntAttributeTag("ptCount", lastIndex + 1);
				for (int i = 0; i < pointCount; i++)
					GenerateChartStringPoint(points[i]);
			}
		}
		void GenerateChartStringPoint(StringPoint stringPoint) {
			if (string.IsNullOrEmpty(stringPoint.Value))
				return;
			try {
				WriteStartElement("pt", DrawingMLChartNamespace);
				WriteIntValue("idx", stringPoint.Index);
				GenerateChartSimpleStringTag("v", stringPoint.Value);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
