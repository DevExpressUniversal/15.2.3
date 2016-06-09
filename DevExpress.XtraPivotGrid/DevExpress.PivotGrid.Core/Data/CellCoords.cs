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
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotDataCoord : ICloneable {
		int col, row, data;
		PivotSummaryType summary;
		public PivotDataCoord(int columnIndex, int rowIndex, int dataIndex, PivotSummaryType summaryType) {
			this.col = columnIndex;
			this.row = rowIndex;
			this.data = dataIndex;
			this.summary = summaryType;
		}
		public int Col { get { return col; } }
		public int Row { get { return row; } }
		public int Data { get { return data; } }
		public PivotSummaryType Summary { get { return summary; } }
		public override int GetHashCode() {
			return Data ^ (Col << 5) ^ (Row << 18);
		}
		public override bool Equals(object obj) {
			PivotDataCoord b = obj as PivotDataCoord;
			if(b != null)
				return Col == b.Col && Row == b.Row && Data == b.Data && Summary == b.Summary;
			return base.Equals(obj);
		}
		public PivotDataCoord Clone() {
			return new PivotDataCoord(Col, Row, Data, Summary);
		}
		object ICloneable.Clone() {
			return Clone();
		}
		public class Iterator {
			PivotDataCoord child;
			bool iterateByColumn;
			public Iterator(PivotDataCoord child, bool byColumn) {
				this.child = child.Clone();
				this.iterateByColumn = byColumn;
			}
			public PivotDataCoord Child { get { return child; } }
			public int Coord {
				get { return iterateByColumn ? child.Col : child.Row; }
				set {
					if(iterateByColumn) child.col = value;
					else child.row = value;
				}
			}
			public bool MovePrevious(PivotDataSourceObjectLevelHelper dataSourceObjectLevelHelper, DefaultBoolean crossGroup) {
				if(crossGroup != DefaultBoolean.Default)
					Coord = dataSourceObjectLevelHelper.GetPrevIndex(this.iterateByColumn, Coord, crossGroup != DefaultBoolean.True);
				else
					Coord = dataSourceObjectLevelHelper.GetPrevIndex(this.iterateByColumn, Coord);
				return Coord != -1;
			}
		}
	}
}
