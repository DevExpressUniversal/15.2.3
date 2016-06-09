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
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPKPIMeasureMetadataColumn : OLAPKPIMetadataColumn, IQueryAliasColumn {
		string measureColumn;
		string IQueryAliasColumn.OriginalColumn { get { return MeasureColumn; } }
		public string MeasureColumn { get { return measureColumn; } }
		public override byte TypeCode { get { return OLAPKPIMeasureTypeCode; } }
		public OLAPKPIMeasureMetadataColumn() { }
		public OLAPKPIMeasureMetadataColumn(int level, int cardinality, Type dataType, MetadataColumnBase parentColumn,
			OLAPHierarchy columnHierarchy, string drilldownColumn, string defaultMemberName, OLAPHierarchy hierarchy, OLAPDataType olapDataType,
									  string graphic, PivotKPIType type, string displayFolder, string measureColumn)
			: base(level, cardinality, dataType, parentColumn, columnHierarchy, drilldownColumn, defaultMemberName, hierarchy, olapDataType, graphic, type, displayFolder) {
				this.measureColumn = measureColumn;
		}
		protected override void SaveToStream(IQueryMetadata owner, TypedBinaryWriter writer) {
			base.SaveToStream(owner, writer);
			writer.Write(measureColumn);
		}
		public override void RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader) {
			base.RestoreFromStream(metadata, reader);
			measureColumn = reader.ReadString();
		}
	}
}
