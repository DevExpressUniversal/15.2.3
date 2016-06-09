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

using System;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Helpers;
namespace DevExpress.DashboardExport {
	public abstract class DashboardColumnImplementer : IColumn {
		const int DefaultWidth = 89;
		readonly DashboardColumnSettings settings = new DashboardColumnSettings();
		XlCellFormatting IColumn.Appearance { get { return settings.Appearance; } }
		XlCellFormatting IColumn.AppearanceHeader { get { return settings.Appearance; } }
		string IColumn.FieldName { get { return settings.FieldName; } }
		string IColumn.HyperlinkEditorCaption { get { return settings.HyperlinkEditorCaption; } }
		string IColumn.HyperlinkTextFormatString { get { return string.Empty; } }
		IUnboundInfo IColumn.UnboundInfo { get { return null; } }
		bool IColumn.IsVisible { get { return settings.Visible; } }
		int IColumn.GroupIndex { get { return -1; } }
		string IColumn.Name { get { return settings.Name; } }
		int IColumn.Width { get { return DefaultWidth; } }
		IEnumerable<object> IColumn.DataValidationItems { get { return null; } }
		int IColumn.VisibleIndex { get { return 0; } }
		ISparklineInfo IColumn.SparklineInfo { get { return CreateSparkline(); } }
		ColumnSortOrder IColumn.SortOrder{ get { return ColumnSortOrder.None; } }
		bool IColumn.IsCollapsed { get { return false; } }
		bool IColumn.IsFixedLeft { get { return false; } }
		bool IColumn.IsGroupColumn { get { return false; } }
		public FormatSettings FormatSettings { get; set; }
		public ColumnEditTypes ColEditType { get; set; }
		public string Header { get; set; }
		public int LogicalPosition { get; set; }
		protected DashboardColumnImplementer() {
			settings.Visible = true;
			settings.Appearance = new XlCellFormatting() { Font = new XlFont() };
		}
		protected virtual ISparklineInfo CreateSparkline() {
			return null;
		}
		IEnumerable<IColumn> IColumn.GetAllColumns() {
			yield break;
		}
		int IColumn.GetColumnGroupLevel() {
			return 0;
		}
		string IColumn.GetGroupColumnHeader() {
			return string.Empty;
		}
	}
}
