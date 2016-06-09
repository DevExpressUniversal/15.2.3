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
using System.Data;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.PivotGrid.OLAP.SchemaEntities;
namespace DevExpress.PivotGrid.OLAP {
	public static class AdoMetadataHelper {
	   internal static IOLAPRowSet GetSchemaRowSet(AdomdConnection connection, string schemaName, string cubeName, Dictionary<string, object> restrictions) {
		   string catalogName = connection.Database;
		   AdomdRestrictionCollection adoRestrictions = new AdomdRestrictionCollection(connection.Version);
		   foreach(KeyValuePair<string, object> pair in restrictions)
			   adoRestrictions.Add(pair.Key, pair.Value);
		   return DataReaderWrapper.Wrap(connection.GetSchemaDataSet(schemaName, adoRestrictions).Tables[0]);
	   }
	}
	class AdoCellSetParser : OLAPCellSetParser {
		internal AdoCellSetParser(IOLAPMetadata owner)
			: base(owner) {
		}
		protected override IOLAPCellSet ExecuteCellSet(IOLAPCommand command, IQueryContext<OLAPCubeColumn> context) {
			return new XMLReaderCellSet(((AdomdCommand)command).ExecuteXmlReader());
		}
		public override IOLAPRowSet QueryDrillDown(IOLAPCommand command) {
			return DataReaderWrapper.Wrap(((AdomdCommand)command).ExecuteReader());
		}
	}
}
