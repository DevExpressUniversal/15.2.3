#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Native {
	public static class DataItemDefinitionDisplayTextProvider {
		public static string GetDimensionDefinitionDisplayText(DimensionDefinition dimensionDefinition, IDataSourceSchema dataSourceSchemaProvider) {
			string definitionDisplayText = dataSourceSchemaProvider != null ? dataSourceSchemaProvider.GetDataItemDefinitionDisplayText(dimensionDefinition.DataMember) : dimensionDefinition.DataMember;
			DataFieldType dataFieldType = dataSourceSchemaProvider != null ? dataSourceSchemaProvider.GetFieldType(dimensionDefinition.DataMember) : DataFieldType.Unknown;
			StringBuilder str = new StringBuilder(definitionDisplayText);
			if(!dimensionDefinition.Equals(DimensionDefinition.MeasureNamesDefinition)) {
				List<string> parameters = new List<string>();
				if((dataFieldType == DataFieldType.DateTime || dataFieldType == DataFieldType.Unknown) && dimensionDefinition.DateTimeGroupInterval != DateTimeGroupInterval.None)
					parameters.Add(GroupIntervalCaptionProvider.GetDateTimeGroupIntervalCaption(dimensionDefinition.DateTimeGroupInterval));
				if((dataFieldType == DataFieldType.Text || dataFieldType == DataFieldType.Unknown) && dimensionDefinition.TextGroupInterval != TextGroupInterval.None)
					parameters.Add(GroupIntervalCaptionProvider.GetTextGroupIntervalCaption(dimensionDefinition.TextGroupInterval));
				if(parameters.Count > 0) {
					str.AppendFormat(" (");
					str.Append(parameters[0]);
					for(int i = 1; i < parameters.Count; i++) {
						str.AppendFormat(", ");
						str.Append(parameters[i]);
					}
					str.AppendFormat(")");
				}
			}
			else
				return DashboardLocalizer.GetString(DashboardStringId.MeasureNamesDimensionName);
			return str.ToString();
		}
		public static string GetMeasureDefinitionTextInternal(MeasureDefinition measureDefinition, string dataMemberString) {
			StringBuilder str = new StringBuilder(dataMemberString);
			str.AppendFormat(" ({0})", Measure.GetSummaryTypeCaption(measureDefinition.SummaryType));
			return str.ToString();
		}
		public static string GetMeasureDefinitionString(MeasureDefinition measureDefinition) {
			return GetMeasureDefinitionTextInternal(measureDefinition, measureDefinition.DataMember);
		}
		public static string GetMeasureDefinitionDisplayText(MeasureDefinition measureDefinition, IDataSourceSchema dataSource) {
			return GetMeasureDefinitionTextInternal(measureDefinition, dataSource != null ? dataSource.GetDataItemDefinitionDisplayText(measureDefinition.DataMember) : measureDefinition.DataMember);
		}
	}
}
