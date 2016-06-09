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
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Xpf.RichEdit.UI {
	[ToolboxItem(false)]
	public partial class ColumnsEditControl : UserControl {
		#region Fields
		readonly List<ColumnInfoEdit> editors;
		ColumnsInfoUI columnsInfo;
		DocumentUnit defaultUnitType = DocumentUnit.Inch;
		DocumentModelUnitConverter valueUnitConverter;
		#endregion
		public ColumnsEditControl() {
			InitializeComponent();
			this.editors = new List<ColumnInfoEdit>();
			this.valueUnitConverter = new DocumentModelUnitTwipsConverter();
		}
		#region Properties
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
				panel.RowDefinitions.RemoveAt(i + 1);
			}
			for (int i = editors.Count; i < columnsInfo.Columns.Count; i++) {
				RowDefinition rowDefinition = new RowDefinition();
				rowDefinition.Height = new GridLength(0.0, GridUnitType.Auto);
				panel.RowDefinitions.Add(rowDefinition);
				CreateColumnEditor(columnsInfo.Columns[i]);
			}
		}
		protected internal virtual void CreateColumnEditor(ColumnInfoUI info) {
			ColumnInfoEdit editor = new ColumnInfoEdit(info, DefaultUnitType, ValueUnitConverter);
			UIElementCollection panelChildren = panel.Children;
			int rowIndex = panelChildren.Count / 3;
			panelChildren.Add(editor.IndexEditor);
			panelChildren.Add(editor.WidthEditor);
			panelChildren.Add(editor.SpacingEditor);
			editor.IndexEditor.SetValue(Grid.ColumnProperty, 0);
			editor.IndexEditor.SetValue(Grid.RowProperty, rowIndex);
			editor.IndexEditor.Margin = GetPanelChildThickness(0);
			editor.WidthEditor.SetValue(Grid.ColumnProperty, 1);
			editor.WidthEditor.SetValue(Grid.RowProperty, rowIndex);
			editor.WidthEditor.Margin = GetPanelChildThickness(1);
			editor.SpacingEditor.SetValue(Grid.ColumnProperty, 2);
			editor.SpacingEditor.SetValue(Grid.RowProperty, rowIndex);
			editor.SpacingEditor.Margin = GetPanelChildThickness(2);
			editors.Add(editor);
		}
		Thickness GetPanelChildThickness(int childIndex) {
			FrameworkElement control = panel.Children[childIndex] as FrameworkElement;
			if (control != null)
				return control.Margin;
			else
				return new Thickness();
		}
		protected internal virtual void DeleteColumnEditor(ColumnInfoEdit control) {
			UIElementCollection panelChildren = panel.Children;
			if (panelChildren.Contains(control.IndexEditor))
				panelChildren.Remove(control.IndexEditor);
			if (panelChildren.Contains(control.WidthEditor))
				panelChildren.Remove(control.WidthEditor);
			if (panelChildren.Contains(control.SpacingEditor))
				panelChildren.Remove(control.SpacingEditor);
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
	#region ColumnInfoEdit
	public class ColumnInfoEdit {
		#region Fields
		ColumnInfoUI columnInfo;
		bool allowWidth = true;
		bool allowSpacing = true;
		DocumentUnit defaultUnitType;
		DocumentModelUnitConverter valueUnitConverter;
		TextEdit edtIndex;
		RichTextIndentEdit edtWidth;
		RichTextIndentEdit edtSpacing;
		#endregion
		public ColumnInfoEdit() {
			InitializeComponent();
		}
		public ColumnInfoEdit(ColumnInfoUI columnInfo, DocumentUnit defaultUnitType, DocumentModelUnitConverter valueUnitConverter) {
			Guard.ArgumentNotNull(columnInfo, "columnInfo");
			Guard.ArgumentNotNull(valueUnitConverter, "valueUnitConverter");
			this.columnInfo = columnInfo;
			this.defaultUnitType = defaultUnitType;
			this.valueUnitConverter = valueUnitConverter;
			InitializeComponent();
			UpdateForm();
		}
		void InitializeComponent() {
			this.edtIndex = new TextEdit();
			this.edtIndex.IsReadOnly = true;
			this.edtWidth = new RichTextIndentEdit();
			this.edtWidth.DefaultUnitType = defaultUnitType;
			this.edtWidth.ValueUnitConverter = valueUnitConverter;
			this.edtSpacing = new RichTextIndentEdit();
			this.edtSpacing.DefaultUnitType = defaultUnitType;
			this.edtSpacing.ValueUnitConverter = valueUnitConverter;
		}
		#region Properties
		[DefaultValue(true)]
		public bool AllowWidth {
			get { return allowWidth; }
			set {
				allowWidth = value;
				edtWidth.IsReadOnly = !value;
			}
		}
		public bool AllowSpacing {
			get { return allowSpacing; }
			set {
				allowSpacing = value;
				edtSpacing.IsReadOnly = !value;
			}
		}
		public ColumnInfoUI ColumnInfo {
			get { return columnInfo; }
			set {
				columnInfo = value;
				UpdateForm();
			}
		}
		public TextEdit IndexEditor { get { return edtIndex; } }
		public RichTextIndentEdit WidthEditor { get { return edtWidth; } }
		public RichTextIndentEdit SpacingEditor { get { return edtSpacing; } }
		#endregion
		#region Events
		#region WidthChanged
		EventHandler onWidthChanged;
		public event EventHandler WidthChanged { add { onWidthChanged += value; } remove { onWidthChanged -= value; } }
		protected internal virtual void RaiseWidthChanged() {
			if (onWidthChanged != null)
				onWidthChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SpacingChanged
		EventHandler onSpacingChanged;
		public event EventHandler SpacingChanged { add { onSpacingChanged += value; } remove { onSpacingChanged -= value; } }
		protected internal virtual void RaiseSpacingChanged() {
			if (onSpacingChanged != null)
				onSpacingChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			edtWidth.Value = ColumnInfo.Width;
			edtSpacing.Value = ColumnInfo.Spacing;
			edtIndex.EditValue = String.Format("{0}:", ColumnInfo.Number);
		}
		protected internal virtual void SubscribeControlsEvents() {
			edtWidth.ValueChanged += OnWidthChanged;
			edtSpacing.ValueChanged += OnSpacingChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			edtWidth.ValueChanged -= OnWidthChanged;
			edtSpacing.ValueChanged -= OnSpacingChanged;
		}
		void OnWidthChanged(object sender, EventArgs e) {
			ColumnInfo.Width = edtWidth.Value;
			RaiseWidthChanged();
		}
		void OnSpacingChanged(object sender, EventArgs e) {
			ColumnInfo.Spacing = edtSpacing.Value;
			RaiseSpacingChanged();
		}
	}
	#endregion
}
