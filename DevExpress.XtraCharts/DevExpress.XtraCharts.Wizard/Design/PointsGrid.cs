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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	[DXToolboxItem(false)]
	public class PointsGrid : DataGrid, IMouseValidator {
		#region inner classes
		class PointsGridArgumentColumn : DataGridTextBoxColumn {
			CurrencyManager listManager;
			public PointsGridArgumentColumn(PropertyDescriptor propertyDescriptor, CurrencyManager listManager)
				: base(propertyDescriptor) {
				this.listManager = listManager;
			}
			void SetEmptyText() {
				DataGridTextBox textBox = TextBox as DataGridTextBox;
				if (textBox != null)
					textBox.Text = String.Empty;
			}
			protected override void Abort(int rowNum) {
				if (rowNum < listManager.Count && (string)GetColumnValueAtRow(listManager, rowNum) == String.Empty)
					SetEmptyText();
				else
					base.Abort(rowNum);
			}
			public void BeginEdit() {
				EnterNullValue();
				SetEmptyText();
			}
		}
		#endregion
		private System.Windows.Forms.ContextMenu pointsContextMenu;
		private System.Windows.Forms.MenuItem menuItemAdd;
		private System.Windows.Forms.MenuItem menuItemInsert;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemClear;
		private System.Windows.Forms.MenuItem menuItemSeparator;
		private System.Windows.Forms.MenuItem menuItemMoveUp;
		private System.Windows.Forms.MenuItem menuItemMoveDown;
		UserLookAndFeel lookAndFeel = UserLookAndFeel.Default;
		CurrencyManager oldListManager = null;
		int buttonColumnCount = 0;
		bool lockEnterProc = false;
		bool lockFinishEdit = false;
		internal new CurrencyManager ListManager { get { return base.ListManager; } }
		public new ScrollBar HorizScrollBar { get { return base.HorizScrollBar; } }
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
			set { lookAndFeel = value; }
		}
		public PointsGrid()
			: base() {
			InitializeComponent();
		}
		#region IMouseValidator implementation
		bool IMouseValidator.CanMouseDown {
			get {
				if (CurrentCell.ColumnNumber != 0)
					return true;
				if (ListManager.Count == 0)
					return false;
				PropertyDescriptorCollection properties = ListManager.GetItemProperties();
				string str = properties[0].GetValue(ListManager.Current) as string;
				return !(str == null || str == String.Empty);
			}
		}
		#endregion
		void InitializeComponent() {
			this.pointsContextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemAdd = new System.Windows.Forms.MenuItem();
			this.menuItemInsert = new System.Windows.Forms.MenuItem();
			this.menuItemDelete = new System.Windows.Forms.MenuItem();
			this.menuItemClear = new System.Windows.Forms.MenuItem();
			this.menuItemSeparator = new System.Windows.Forms.MenuItem();
			this.menuItemMoveUp = new System.Windows.Forms.MenuItem();
			this.menuItemMoveDown = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			this.pointsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.menuItemAdd,
			this.menuItemInsert,
			this.menuItemDelete,
			this.menuItemClear,
			this.menuItemSeparator,
			this.menuItemMoveUp,
			this.menuItemMoveDown});
			this.pointsContextMenu.Popup += new System.EventHandler(this.pointsContextMenu_Popup);
			this.menuItemAdd.Index = 0;
			this.menuItemAdd.Text = "Add";
			this.menuItemAdd.Click += new System.EventHandler(this.menuItemAdd_Click);
			this.menuItemInsert.Index = 1;
			this.menuItemInsert.Text = "Insert";
			this.menuItemInsert.Click += new System.EventHandler(this.menuItemInsert_Click);
			this.menuItemDelete.Index = 2;
			this.menuItemDelete.Text = "Delete";
			this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
			this.menuItemClear.Index = 3;
			this.menuItemClear.Text = "Clear";
			this.menuItemClear.Click += new System.EventHandler(this.menuItemClear_Click);
			this.menuItemSeparator.Index = 4;
			this.menuItemSeparator.Text = "-";
			this.menuItemMoveUp.Index = 5;
			this.menuItemMoveUp.Text = "Move Up";
			this.menuItemMoveUp.Click += new System.EventHandler(this.menuItemMoveUp_Click);
			this.menuItemMoveDown.Index = 6;
			this.menuItemMoveDown.Text = "Move Down";
			this.menuItemMoveDown.Click += new System.EventHandler(this.menuItemMoveDown_Click);
			this.ContextMenu = this.pointsContextMenu;
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);
		}
		void CreateTableStyle() {
			int columnsCount = ListManager.GetItemProperties().Count;
			if (columnsCount > 0) {
				var style = new DataGridTableStyle(ListManager);
				style.GridColumnStyles.Clear();
				PropertyDescriptorCollection properties = ListManager.GetItemProperties();
				PointsGridArgumentColumn argumentColumn = new PointsGridArgumentColumn(properties[0], ListManager);
				argumentColumn.HeaderText = properties[0].Name;
				argumentColumn.MappingName = properties[0].Name;
				style.GridColumnStyles.Add(argumentColumn);
				for (int i = 1; i < properties.Count - 3; i++) {
					DataGridTextBoxColumn column = new DataGridTextBoxColumn(properties[i], "G");
					column.HeaderText = properties[i].Name;
					column.MappingName = properties[i].Name;
					style.GridColumnStyles.Add(column);
				}
				buttonColumnCount = 0;
				DataGridButtonColumn buttonColumn = null;
				if (properties[properties.Count - 3].IsBrowsable) {
					ColorGridButtonColumn colorButtonColumn = new ColorGridButtonColumn(this, ListManager);
					colorButtonColumn.MappingName = properties[properties.Count - 3].Name;
					colorButtonColumn.HeaderText = properties[properties.Count - 3].Name;
					colorButtonColumn.ControlSize = new Size(MeasureString(colorButtonColumn.HeaderText).Width + 20, 20);
					colorButtonColumn.Click += new ButtonColumnClickHandler(ColorButtonColumn_Click);
					colorButtonColumn.ClearColorClick += new ButtonColumnClickHandler(ClearColorButtonColumn_Click);
					colorButtonColumn.Alignment = HorizontalAlignment.Left;
					style.GridColumnStyles.Add(colorButtonColumn);
					buttonColumnCount++;
				}
				if (properties[properties.Count - 2].IsBrowsable) {
					buttonColumn = new DataGridButtonColumn(this);
					buttonColumn.MappingName = properties[properties.Count - 2].Name;
					buttonColumn.HeaderText = properties[properties.Count - 2].Name;
					buttonColumn.ControlSize = new Size(MeasureString(buttonColumn.HeaderText).Width, 20);
					buttonColumn.Click += new ButtonColumnClickHandler(AnnotationsButtonColumn_Click);
					buttonColumn.Alignment = HorizontalAlignment.Right;
					style.GridColumnStyles.Add(buttonColumn);
					buttonColumnCount++;
				}
				if (properties[properties.Count - 1].IsBrowsable) {
					buttonColumn = new DataGridButtonColumn(this);
					buttonColumn.MappingName = properties[properties.Count - 1].Name;
					buttonColumn.HeaderText = properties[properties.Count - 1].Name;
					buttonColumn.ControlSize = new Size(MeasureString(buttonColumn.HeaderText).Width, 20);
					buttonColumn.Click += new ButtonColumnClickHandler(LinksButtonColumn_Click);
					buttonColumn.Alignment = HorizontalAlignment.Right;
					style.GridColumnStyles.Add(buttonColumn);
					buttonColumnCount++;
				}
				UnsubscribeColumnStyleEvents();
				TableStyles.Clear();
				TableStyles.Add(style);
				ResizeColumns();
				HorizScrollBar.Visible = false;
			}
		}
		void ResizeColumns() {
			if (TableStyles.Count > 0) {
				SuspendLayout();
				DataGridTableStyle style = TableStyles[0];
				int columnsCount = style.GridColumnStyles.Count;
				int clientWidth = BorderStyle == BorderStyle.None ? ClientSize.Width - style.RowHeaderWidth : ClientSize.Width - style.RowHeaderWidth - 4;
				int buttonColumnsWidth = 0;
				for (int i = 1; i <= buttonColumnCount; i++)
					buttonColumnsWidth += style.GridColumnStyles[style.GridColumnStyles.Count - i].Width;
				int valueColumnWidth = (clientWidth - buttonColumnsWidth) / (columnsCount - buttonColumnCount);
				int argumentColumnWidth = clientWidth - valueColumnWidth * (columnsCount - buttonColumnCount - 1) - buttonColumnsWidth;
				style.GridColumnStyles[0].Width = argumentColumnWidth;
				for (int i = 1; i < style.GridColumnStyles.Count - buttonColumnCount; i++)
					style.GridColumnStyles[i].Width = valueColumnWidth;
				ResumeLayout();
			}
		}
		Size MeasureString(string text) {
			using (TextMeasurer textMeasurer = new TextMeasurer()) {
				SizeF size = textMeasurer.MeasureString(text, Font);
				return new Size(MathUtils.Ceiling(size.Width), MathUtils.Ceiling(size.Height));
			}
		}
		void OnPositionChanged(object sender, EventArgs args) {
			if (ListManager.Position != ListManager.Count - 1)
				ValidateCollection();
		}
		void UnsubscribeColumnStyleEvents() {
			foreach (DataGridTableStyle tableStyle in TableStyles) {
				foreach (DataGridColumnStyle columnStyle in tableStyle.GridColumnStyles) {
					DataGridButtonColumn buttonColumn = columnStyle as DataGridButtonColumn;
					if (buttonColumn != null) {
						buttonColumn.Click -= new ButtonColumnClickHandler(LinksButtonColumn_Click);
						buttonColumn.Click -= new ButtonColumnClickHandler(AnnotationsButtonColumn_Click);
					}
					ColorGridButtonColumn colorColumn = columnStyle as ColorGridButtonColumn;
					if (colorColumn != null) {
						colorColumn.Click -= new ButtonColumnClickHandler(ColorButtonColumn_Click);
						colorColumn.ClearColorClick -= new ButtonColumnClickHandler(ClearColorButtonColumn_Click);
					}
				}
			}
		}
		void SubscribeEvents() {
			UnsubscribeEvents();
			if (ListManager != null) {
				ListManager.PositionChanged += new EventHandler(OnPositionChanged);
				oldListManager = ListManager;
			}
		}
		void UnsubscribeEvents() {
			if (oldListManager != null) {
				if (Enabled)
					((SeriesPointCollection)oldListManager.List).Validate();
				oldListManager.PositionChanged -= new EventHandler(OnPositionChanged);
				oldListManager = null;
			}
		}
		public void pointsContextMenu_Popup(object sender, System.EventArgs e) {
			DataGrid.HitTestInfo info = HitTest(PointToClient(Cursor.Position));
			int rowIndex;
			ResetSelection();
			if (info.Row >= 0 && info.Row < ListManager.Count) {
				CurrentCell = new DataGridCell(info.Row, info.Column);
				rowIndex = info.Row;
			}
			else
				rowIndex = CurrentRowIndex;
			if (rowIndex >= 0)
				Select(rowIndex);
			if (ListManager != null) {
				menuItemAdd.Enabled = true;
				menuItemDelete.Enabled = ListManager.Count > 0;
				menuItemInsert.Enabled = ListManager.Count > 0;
				menuItemMoveUp.Enabled = rowIndex > 0;
				menuItemMoveDown.Enabled = rowIndex < ListManager.Count - 1;
			}
			else
				foreach (MenuItem item in pointsContextMenu.MenuItems)
					item.Enabled = false;
		}
		internal void MovePoint(int oldRowIndex, int newRowIndex) {
			((SeriesPointCollection)ListManager.List).Move(oldRowIndex, newRowIndex);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			int row = CurrentCell.RowNumber;
			int column = CurrentCell.ColumnNumber;
			DataGridTextBoxColumn gridColumn;
			switch (keyData) {
				case Keys.Right:
				case Keys.Tab:
				case Keys.Return:
					if ((keyData & Keys.Right) == Keys.Right) {
						gridColumn = TableStyles[0].GridColumnStyles[column] as DataGridTextBoxColumn;
						if (gridColumn == null || gridColumn.TextBox.SelectionStart + gridColumn.TextBox.SelectionLength != gridColumn.TextBox.TextLength)
							break;
					}
					if (FinishEditOperation()) {
						CurrentCell = new DataGridCell(row, column + 1);
						if (CurrentCell.ColumnNumber == column)
							CurrentCell = new DataGridCell(row + 1, 0);
					}
					return true;
				case Keys.Up:
				case Keys.Down:
				case Keys.Up | Keys.Shift:
				case Keys.Down | Keys.Shift:
				case Keys.Tab | Keys.Shift:
					if (!FinishEditOperation())
						return true;
					if (keyData == (Keys.Up | Keys.Shift) && CurrentCell.ColumnNumber == 0 && row > 0) {
						PropertyDescriptorCollection properties = ListManager.GetItemProperties();
						string str = properties[0].GetValue(ListManager.Current) as string;
						if (str == null || str == String.Empty) {
							CurrentCell = new DataGridCell(--row, CurrentCell.ColumnNumber);
							Select(row);
							return true;
						}
					}
					break;
				case Keys.Left:
				case Keys.Left | Keys.Shift:
				case Keys.Right | Keys.Shift:
					gridColumn = TableStyles[0].GridColumnStyles[column] as DataGridTextBoxColumn;
					if (gridColumn != null &&
						(((keyData & Keys.Left) == Keys.Left && gridColumn.TextBox.SelectionStart == 0) ||
						((keyData & Keys.Right) == Keys.Right && gridColumn.TextBox.SelectionStart + gridColumn.TextBox.SelectionLength == gridColumn.TextBox.TextLength)) &&
						!FinishEditOperation())
						return true;
					break;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if (keyData == Keys.Escape && CurrentCell.ColumnNumber == 0) {
				DataGridTextBoxColumn gridColumn = TableStyles[0].GridColumnStyles[0] as DataGridTextBoxColumn;
				if (gridColumn != null) {
					DataGridTextBox textBox = gridColumn.TextBox as DataGridTextBox;
					if (textBox != null && !textBox.IsInEditOrNavigateMode && textBox.Text == String.Empty)
						return true;
				}
			}
			bool res = base.ProcessDialogKey(keyData);
			if (keyData == Keys.Delete && CurrentCell.ColumnNumber != 0 && ListManager.Count == 0)
				CurrentCell = new DataGridCell(CurrentCell.RowNumber, 0);
			return res;
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeEvents();
			base.Dispose(disposing);
		}
		protected override void OnDataSourceChanged(EventArgs e) {
			UpdateTableStyle();
			base.OnDataSourceChanged(e);
		}
		protected override void ColumnStartedEditing(Rectangle bounds) {
			try {
				lockFinishEdit = true;
				base.ColumnStartedEditing(bounds);
			}
			finally {
				lockFinishEdit = false;
			}
		}
		protected override void OnMouseDown(MouseEventArgs args) {
			if (FinishEditOperation())
				base.OnMouseDown(args);
		}
		protected override void OnEnter(EventArgs args) {
			if (lockEnterProc)
				lockEnterProc = false;
			else
				base.OnEnter(args);
		}
		protected override void OnLeave(EventArgs args) {
			if (FinishEditOperation(false)) {
				if (!lockFinishEdit)
					ValidateCollection();
				base.OnLeave(args);
			}
		}
		protected override void OnCurrentCellChanged(EventArgs e) {
			base.OnCurrentCellChanged(e);
			if (CurrentCell.ColumnNumber != 0)
				if (ListManager.Count == 0)
					CurrentCell = new DataGridCell(0, 0);
				else {
					PropertyDescriptorCollection properties = ListManager.GetItemProperties();
					string str = properties[0].GetValue(ListManager.Current) as string;
					if (str == null || str == String.Empty)
						CurrentCell = new DataGridCell(CurrentCell.RowNumber, 0);
				}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			ResizeColumns();
		}
		protected override void OnPaint(PaintEventArgs eventArgs) {
			try {
				base.OnPaint(eventArgs);
			}
			catch {
			}
		}
		void BeginRowEdit(int rowNumber) {
			PointsGridArgumentColumn gridColumn = TableStyles[0].GridColumnStyles[0] as PointsGridArgumentColumn;
			if (gridColumn != null)
				gridColumn.BeginEdit();
		}
		void menuItemAdd_Click(object sender, System.EventArgs e) {
			CurrentCell = new DataGridCell(ListManager.Count, 0);
			BeginRowEdit(ListManager.Count);
		}
		void menuItemInsert_Click(object sender, System.EventArgs e) {
			int pos = CurrentRowIndex;
			((IBindingList)ListManager.List).AddNew();
			MovePoint(ListManager.Count - 1, pos);
			CurrentCell = new DataGridCell(pos, 0);
			BeginRowEdit(pos);
		}
		void menuItemDelete_Click(object sender, System.EventArgs e) {
			int row = CurrentRowIndex;
			int column = CurrentCell.ColumnNumber;
			if (row >= 0) {
				ListManager.List.RemoveAt(row);
				if (row >= ListManager.Count) {
					if (ListManager.Count == 0)
						return;
					row = ListManager.Count - 1;
				}
				CurrentCell = new DataGridCell(row, column);
			}
		}
		void menuItemClear_Click(object sender, System.EventArgs e) {
			ListManager.List.Clear();
			CurrentCell = new DataGridCell(0, 0);
		}
		void menuItemMoveUp_Click(object sender, System.EventArgs e) {
			int row = CurrentRowIndex;
			int column = CurrentCell.ColumnNumber;
			if (row > 0) {
				int newRow = row - 1;
				MovePoint(row, newRow);
				CurrentCell = new DataGridCell(newRow, column);
			}
		}
		void menuItemMoveDown_Click(object sender, System.EventArgs e) {
			int row = CurrentRowIndex;
			int column = CurrentCell.ColumnNumber;
			if (row >= 0 && row < ListManager.Count - 1) {
				int newRow = row + 1;
				MovePoint(row, newRow);
				CurrentCell = new DataGridCell(newRow, column);
			}
		}
		void LinksButtonColumn_Click(ButtonColumnEventArgs e) {
			SeriesPoint point = (SeriesPoint)ListManager.List[e.Row];
			using (TaskLinkCollectionEditorForm form = new TaskLinkCollectionEditorForm(point.Relations)) {
				form.Initialize(null);
				form.ShowDialog(this);
			}
		}
		void AnnotationsButtonColumn_Click(ButtonColumnEventArgs e) {
			SeriesPoint point = (SeriesPoint)ListManager.List[e.Row];
			using (AnnotationCollectionEditorForm form = new AnnotationCollectionEditorForm((IAnnotationCollection)point.Annotations)) {
				if (this.lookAndFeel != null && lookAndFeel.Style == LookAndFeelStyle.Skin)
					form.LookAndFeel.SetSkinStyle(this.lookAndFeel.ActiveSkinName);
				form.Initialize(CommonUtils.FindChartContainer(point).Chart);
				form.ShowDialog(this);
			}
		}
		void ColorButtonColumn_Click(ButtonColumnEventArgs e) {
			SeriesPoint point = ListManager.List[e.Row] as SeriesPoint;
			if (point == null)
				return;
			using (ColorDialog form = new ColorDialog()) {
				DialogResult result = form.ShowDialog(this);
				if (result == DialogResult.OK)
					point.Color = form.Color;
			}
		}
		void ClearColorButtonColumn_Click(ButtonColumnEventArgs e) {
			SeriesPoint point = ListManager.List[e.Row] as SeriesPoint;
			if (point == null)
				return;
			point.Color = Color.Empty;
		}
		public bool FinishEditOperation(bool needReselect = true) {
			if (lockFinishEdit || TableStyles.Count == 0)
				return true;
			int column = CurrentCell.ColumnNumber;
			DataGridTextBoxColumn gridColumn = TableStyles[0].GridColumnStyles[column] as DataGridTextBoxColumn;
			int row = CurrentRowIndex;
			if (row < 0 || row >= ListManager.Count || gridColumn == null)
				return true;
			DataGridTextBox textBox = gridColumn.TextBox as DataGridTextBox;
			if (textBox == null || textBox.IsInEditOrNavigateMode)
				return true;
			PropertyDescriptorCollection properties = ListManager.GetItemProperties();
			try {
				properties[column].SetValue(ListManager.Current, textBox.Text);
			}
			catch {
				lockEnterProc = true;
				if (needReselect) {
					string mess = (column == 0) ? string.Format("An argument {0} isn't compatible with the current argument scale type.",
						textBox.Text) : string.Format("A value {0} isn't compatible with the current value scale type.", properties[column].Name);
					XtraMessageBox.Show(lookAndFeel, this, mess);
					textBox.SelectAll();
					textBox.Focus();
				}
				return false;
			}
			return true;
		}
		public void ValidateCollection() {
			if (Enabled && ListManager != null && ListManager.List != null)
				((SeriesPointCollection)ListManager.List).Validate();
		}
		public void UpdateTableStyle() {
			SubscribeEvents();
			if (ListManager == null)
				return;
			CreateTableStyle();
		}
	}
}
