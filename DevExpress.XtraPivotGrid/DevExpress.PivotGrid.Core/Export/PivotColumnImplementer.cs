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
using System.Linq;
using DevExpress.Data;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.PivotGrid.Export {
	public class PivotColumnImplementer : IColumn, IPivotExportItem<PivotColumnImplementer> {
		readonly List<PivotColumnImplementer> innerColumns = new List<PivotColumnImplementer>();
		XlCellFormatting IColumn.Appearance { get { return Settings.Appearance; } }
		XlCellFormatting IColumn.AppearanceHeader { get { return Settings.Appearance; } }
		ColumnEditTypes IColumn.ColEditType { get { return Settings.EditType; } }
		string IColumn.FieldName { get { return Settings.FieldName; } }
		string IColumn.Header { get { return null; } }
		string IColumn.HyperlinkEditorCaption { get { return Settings.HyperlinkEditorCaption; } }
		string IColumn.HyperlinkTextFormatString{
			get { return string.Empty; }
		}
		IUnboundInfo IColumn.UnboundInfo { get { return null; } }
		bool IColumn.IsVisible { get { return Settings.Visible; } }
		int IColumn.GroupIndex { get { return -1; } }
		string IColumn.Name { get { return Settings.Name; } }
		int IColumn.Width { get { return Settings.Width; } }
		IEnumerable<object> IColumn.DataValidationItems { get { return null; } }
		IList<PivotColumnImplementer> IPivotExportItem<PivotColumnImplementer>.Items { get { return innerColumns; } }
		ISparklineInfo IColumn.SparklineInfo { get { return null; } }
		ColumnSortOrder IColumn.SortOrder{
			get { return ColumnSortOrder.None; }
		}
		public bool IsCollapsed { get; set; }
		public bool IsFixedLeft { get; set; }
		public bool IsGroupColumn { get; set; }
		public bool IsRowAreaColumn { get; set; }
		public int ExportIndex { get; set; }
		public FormatSettings FormatSettings { get; set; }
		public PivotColumnSettings Settings { get; set; }
		public int LogicalPosition { get; set; }
		public int Level { get; set; }
		public string GroupHeader { get; set; }
		int IColumn.VisibleIndex { get { return 0; } }  
		IEnumerable<IColumn> IColumn.GetAllColumns() {
			return innerColumns;
		}
		int IColumn.GetColumnGroupLevel() {
			return Level;
		}
		string IColumn.GetGroupColumnHeader() {
			return GroupHeader;
		}
	}
}
