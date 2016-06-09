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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Design {
	[DXToolboxItem(false)]
	public partial class ColumnsEdit : XtraUserControl {
		#region Fields
		readonly List<ColumnInfoEdit> editors;
		ColumnsInfoUI columnsInfo;
		DocumentUnit defaultUnitType = DocumentUnit.Inch;
		DocumentModelUnitConverter valueUnitConverter;
		#endregion
		public ColumnsEdit() {
			InitializeComponent();
			this.editors = new List<ColumnInfoEdit>();
			this.valueUnitConverter = new DocumentModelUnitTwipsConverter();
		}
		#region Properties
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		protected internal ColumnsInfoUI ColumnsInfo {
			get { return columnsInfo; }
			set {
				columnsInfo = value;
				UpdateForm();
			}
		}
		protected internal List<ColumnInfoEdit> Editors { get { return editors; } }
		public DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } set { valueUnitConverter = value; } }
		public DocumentUnit DefaultUnitType { get { return defaultUnitType; } set { defaultUnitType = value; } }
		#endregion
		protected internal virtual void SubscribeControlsEvents() {
			editors.ForEach(SubscribeControlEvents);
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			editors.ForEach(UnsubscribeControlEvents);
		}
		protected internal virtual void SubscribeControlEvents(ColumnInfoEdit control) {
			control.WidthChanged += ChangeWidth;
			control.SpacingChanged += ChangeSpacing;
		}
		protected internal virtual void UnsubscribeControlEvents(ColumnInfoEdit control) {
			control.WidthChanged -= ChangeWidth;
			control.SpacingChanged -= ChangeSpacing;
		}
		protected internal virtual void CreateEditors() {
			columnsInfo.Columns.ForEach(CreateColumnEditor);
		}
		protected internal virtual void DeleteEditors() {
			editors.ForEach(DeleteColumnEditor);
			editors.Clear();
		}
		protected internal virtual void UpdateEditorsCount() {	  
			for (int i = editors.Count - 1; i >= columnsInfo.Columns.Count; i--) {
				DeleteColumnEditor(editors[i]);
				editors.Remove(editors[i]);
			}
			for (int i = editors.Count; i < columnsInfo.Columns.Count; i++)
				CreateColumnEditor(columnsInfo.Columns[i]);
		}
		protected internal virtual void CreateColumnEditor(ColumnInfoUI info) {
			ColumnInfoEdit editor = new ColumnInfoEdit(info, DefaultUnitType, ValueUnitConverter);
			editor.Location = new Point(0, (editors.Count > 0) ? (editor.Size.Height + editors[editors.Count - 1].Location.Y) : 0);
			editors.Add(editor);
			panel.Controls.Add(editors[editors.Count - 1]);
		}
		protected internal virtual void DeleteColumnEditor(ColumnInfoEdit control) {
			if (panel.Controls.Contains(control))
				panel.Controls.Remove(control);
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateEditorsCount();
				ApplyEditorsAvailability();
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			List<ColumnInfoUI> columns = columnsInfo.Columns;
			System.Diagnostics.Debug.Assert(editors.Count == columns.Count);
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				UpdateEditor(editors[i], columns[i]);
		}
		protected internal virtual void UpdateEditor(ColumnInfoEdit control, ColumnInfoUI info) {
			control.ColumnInfo = info;
		}
		protected internal virtual void ChangeWidth(object sender, EventArgs e) {
			columnsInfo.RecalculateColumnsByWidthAfterIndex(((ColumnInfoEdit)sender).ColumnInfo.Number - 1);
			UpdateForm();
		}
		protected internal virtual void ChangeSpacing(object sender, EventArgs e) {  
			columnsInfo.RecalculateColumnsBySpacingAfterIndex(((ColumnInfoEdit)sender).ColumnInfo.Number - 1);
			UpdateForm();
		}
		protected internal virtual void EnableEditors(int from, int to, bool enabled) {
			for (int i = from; i <= to; i++) {
				editors[i].AllowWidth = enabled;
				editors[i].AllowSpacing = enabled;
			}
		}
		protected internal virtual void ApplyEditorsAvailability() {
			if (!columnsInfo.ColumnCount.HasValue)
				return;
			EnableEditors(0, columnsInfo.ColumnCount.Value - 1, false);
			if (!columnsInfo.EqualColumnWidth.HasValue) 
				return;
			if (columnsInfo.EqualColumnWidth.Value && columnsInfo.ColumnCount.Value > 0)
				EnableEditors(0, 0, true);
			else
				EnableEditors(0, columnsInfo.ColumnCount.Value - 1, true);
			editors[columnsInfo.ColumnCount.Value - 1].AllowSpacing = false;
		}
	}
}
