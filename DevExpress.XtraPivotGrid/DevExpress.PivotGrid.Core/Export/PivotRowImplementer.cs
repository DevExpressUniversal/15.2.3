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
using DevExpress.XtraExport.Helpers;
namespace DevExpress.PivotGrid.Export {
	public class PivotRowImplementer : IRowBase, IPivotExportItem<PivotRowImplementer> {
		int IRowBase.DataSourceRowIndex { get { return -1; } }
		bool IRowBase.IsGroupRow { get { return false; } }
		IList<PivotRowImplementer> IPivotExportItem<PivotRowImplementer>.Items { get { return Items; } }
		protected virtual IList<PivotRowImplementer> Items { get { return null; } }
		public bool IsColumnAreaRow { get; set; }
		public int ExportIndex { get; set; }
		public int Level { get; set; }
		public int LogicalPosition { get; set; }
		public FormatSettings FormatSettings { get; set; }
		public bool IsDataAreaRow { get { return true; } }
		int IRowBase.GetRowLevel() {
			return Level;
		}
	}
	public class PivotGroupRowImplementer : PivotRowImplementer, IGroupRow<PivotRowImplementer> {
		readonly List<PivotRowImplementer> innerRows = new List<PivotRowImplementer>();
		bool IRowBase.IsGroupRow { get { return true; } }
		protected override IList<PivotRowImplementer> Items { get { return innerRows; } }
		public bool IsCollapsed { get; set; }
		public string Caption { get; set; }
		string IGroupRow<PivotRowImplementer>.GetGroupRowHeader() {
			return Caption;
		}
		IEnumerable<PivotRowImplementer> IGroupRow<PivotRowImplementer>.GetAllRows() {
			return innerRows;
		}
	}
}
