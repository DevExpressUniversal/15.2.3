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
using System.Linq;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard {
	public class GroupingLevelInfo {
		string displayName;
		public string DisplayName {
			get {
				if(displayName == null) {
					displayName = string.Join(", ", Columns.Select(c => c.DisplayName).ToArray());
				}
				return displayName;
			}
		}
		public ColumnInfo[] Columns { get; private set; }
		public bool ContainsColumn(string columnName) {
			return Columns.Any(c => c.Name == columnName);
		}
		public GroupingLevelInfo(ColumnInfo[] columns) {
			Guard.ArgumentNotNull(columns, "columns");
			Columns = columns;
		}
		public override bool Equals(object obj) {
			GroupingLevelInfo other = obj as GroupingLevelInfo;
			if(other == null)
				return false;
			return ArrayHelper.ArraysEqual(Columns, other.Columns);
		}
		public override int GetHashCode() {
			int[] hashCodes = Columns.Select(x => x.GetHashCode()).ToArray();
			return HashCodeHelper.CalcHashCode(hashCodes);
		}
	}
}
