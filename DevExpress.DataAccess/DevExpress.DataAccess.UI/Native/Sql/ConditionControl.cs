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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native.Sql.QueryBuilder;
using DevExpress.Skins;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public partial class ConditionControl : XtraUserControl {
		static string GetOperatorText(BinaryOperatorType operatorType) {
			string text;
			switch(operatorType) {
				case BinaryOperatorType.Equal:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorEqualOperator);
					break;
				case BinaryOperatorType.NotEqual:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorNotEqualOperator);
					break;
				case BinaryOperatorType.Greater:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorGreaterOperator);
					break;
				case BinaryOperatorType.GreaterOrEqual:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorGreaterOrEqualOperator);
					break;
				case BinaryOperatorType.Less:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorLessOperator);
					break;
				case BinaryOperatorType.LessOrEqual:
					text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorLessOrEqualOperator);
					break;
				default:
					text = operatorType.ToString();
					break;
			}
			return text;
		}
		const int padding = 3;
		readonly Font normalFont, underlineFont;
		readonly HoverImageButton buttonOperator;
		readonly Color defaultColorEmptyValue = Color.Gray;
		readonly Control[] alignedControls;
		readonly BarManager barManager;
		readonly IDisplayNameProvider displayNameProvider;
		Color foreColor, foreColorEmptyValue, foreColorInvalidValue;
		Dictionary<BinaryOperatorType, Image[]> images;
		string leftTableName;
		string leftColumnName;
		string rightTableName;
		string rightColumnName;
		BinaryOperatorType operatorType = BinaryOperatorType.Equal;
		PopupMenu popupMenuLeftTable;
		PopupMenu popupMenuLeftColumn;
		PopupMenu popupMenuRightTable;
		PopupMenu popupMenuRightColumn;
		PopupMenu popupMenuOperation;
		Dictionary<string, List<string>> leftObjectNames;
		Dictionary<string, List<string>> rightObjectNames;
		bool allowChangeLeftTable;
		public ConditionControl(BarManager barManager, Dictionary<BinaryOperatorType, Image[]> images, Image removeNormal, Image removeHover)
			: this(barManager, null, images, removeNormal, removeHover) { }
		public ConditionControl(BarManager barManager, IDisplayNameProvider displayNameProvider, Dictionary<BinaryOperatorType, Image[]> images, Image removeNormal, Image removeHover)
			: this() {
			this.images = images;
			this.barManager = barManager;
			this.displayNameProvider = displayNameProvider;
			this.buttonOperator = new HoverImageButton(images[BinaryOperatorType.Equal]);
			this.buttonOperator.Top = this.labelLeftTable.Top + (int)Math.Round(Convert.ToDecimal(this.labelLeftColumn.Height - this.buttonOperator.Height)/2);
			this.buttonOperator.MouseClick += buttonOperator_MouseClick;
			Controls.Add(this.buttonOperator);
			HoverImageButton removeItemButton = new HoverImageButton(removeHover, removeNormal);
			removeItemButton.Top = this.labelLeftTable.Top + (int)Math.Round(Convert.ToDecimal(this.labelLeftColumn.Height - removeItemButton.Height)/2);
			removeItemButton.ToolTip = DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorRemoveConditionMessage);
			removeItemButton.MouseClick += removeItemButton_MouseClick;
			Controls.Add(removeItemButton);
			this.alignedControls = new Control[] {removeItemButton, this.labelLeftTable, this.labelLeftPoint, this.labelLeftColumn, this.buttonOperator, this.labelRightTable, this.labelRightPoint, this.labelRightColumn};
			InitializeMenus();
			UpdateSkinColors();
		}
		ConditionControl() {
			InitializeComponent();
			if(this.components == null)
				this.components = new Container();
			this.normalFont = this.labelLeftColumn.Font;
			this.underlineFont = new Font(this.normalFont, FontStyle.Underline);
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			this.foreColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("WindowText");
		}
		public Dictionary<BinaryOperatorType, Image[]> Images {
			get { return this.images; }
			set {
				this.images = value;
				CreateOperationMenu();
			}
		}
		public string LeftTableName {
			get { return this.leftTableName; }
			set {
				if(this.leftTableName == value)
					return;
				TableNameChangedEventArgs args = new TableNameChangedEventArgs(this.leftTableName);
				this.leftTableName = value;
				List<string> columnNames;
				this.leftObjectNames.TryGetValue(this.leftTableName, out columnNames);
				RefreshLeftColumnsMenu();
				if(columnNames == null || columnNames.All(c => c != this.leftColumnName))
					LeftColumnName = string.Empty;
				else {
					UpdateLeftColumnLabel();
				}
				UpdateLeftTableLabel();
				RefreshLeftTablesMenu();
				RefreshRightTablesMenu();
				RefreshRightColumnsMenu();
				AlignControls();
				if(LeftTableNameChanged != null && args.OldName != null)
					LeftTableNameChanged(this, args);
			}
		}
		public string LeftColumnName {
			get { return this.leftColumnName; }
			set {
				if(this.leftColumnName == value)
					return;
				this.leftColumnName = value;
				UpdateLeftColumnLabel();
				RefreshLeftColumnsMenu();
				AlignControls();
			}
		}
		public string RightTableName {
			get { return this.rightTableName; }
			set {
				if(this.rightTableName == value)
					return;
				TableNameChangedEventArgs args = new TableNameChangedEventArgs(this.rightTableName);
				this.rightTableName = value;
				List<string> columnNames;
				this.rightObjectNames.TryGetValue(this.rightTableName, out columnNames);
				RefreshRightColumnsMenu();
				if(columnNames == null || columnNames.All(c => c != this.rightColumnName))
					RightColumnName = string.Empty;
				else {
					UpdateRightColumnLabel();
				}
				UpdateRightTableLabel();
				RefreshRightTablesMenu();
				RefreshLeftTablesMenu();
				AlignControls();
				if(RightTableNameChanged != null && args.OldName != null)
					RightTableNameChanged(this, args);
			}
		}
		public string RightColumnName {
			get { return this.rightColumnName; }
			set {
				if(this.rightColumnName == value)
					return;
				this.rightColumnName = value;
				UpdateRightColumnLabel();
				RefreshRightColumnsMenu();
				AlignControls();
			}
		}
		public BinaryOperatorType OperatorType {
			get { return this.operatorType; }
			set {
				this.operatorType = value;
				UpdateOperatorText(value);
			}
		}
		public Dictionary<string, List<string>> LeftObjectNames {
			get { return this.leftObjectNames; }
			set {
				this.leftObjectNames = value;
				RefreshLeftTablesMenu();
				RefreshLeftColumnsMenu();
			}
		}
		public Dictionary<string, List<string>> RightObjectNames {
			get { return this.rightObjectNames; }
			set {
				this.rightObjectNames = value;
				RefreshRightTablesMenu();
				RefreshRightColumnsMenu();
			}
		}
		public bool AllowChangeOperatorType { get; set; }
		public bool AllowChangeMasterTable { get { return this.allowChangeLeftTable; } set { this.allowChangeLeftTable = value; } }
		public event EventHandler RemoveButtonClick;
		public event EventHandler<TableNameChangedEventArgs> LeftTableNameChanged;
		public event EventHandler<TableNameChangedEventArgs> RightTableNameChanged;
		internal event EventHandler<MouseEventArgs> UnderlineControlMouseDown;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.underlineFont.Dispose();
				if(this.components != null)
					this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeMenus() {
			this.popupMenuLeftTable = new PopupMenu(this.barManager);
			this.components.Add(this.popupMenuLeftTable);
			this.popupMenuLeftColumn = new PopupMenu(this.barManager);
			this.components.Add(this.popupMenuLeftColumn);
			this.popupMenuRightTable = new PopupMenu(this.barManager);
			this.components.Add(this.popupMenuRightTable);
			this.popupMenuRightColumn = new PopupMenu(this.barManager);
			this.components.Add(this.popupMenuRightColumn);
			this.popupMenuOperation = new PopupMenu(this.barManager);
			this.components.Add(this.popupMenuOperation);
			CreateOperationMenu();
		}
		void CreateOperationMenu() {
			while(this.popupMenuOperation.ItemLinks.Count > 0) {
				BarItemLink link = this.popupMenuOperation.ItemLinks[0];
				BarItem item = link.Item;
				item.ItemClick -= barItemOperator_ItemClick;
				this.popupMenuOperation.RemoveLink(link);
				item.Dispose();
			}
			foreach(BinaryOperatorType binaryOperatorType in this.images.Keys) {
				BarCheckItem newItem = new BarCheckItem(this.barManager, binaryOperatorType == OperatorType) {
					Caption = GetOperatorText(binaryOperatorType),
					Tag = binaryOperatorType,
					Glyph = this.images[binaryOperatorType][0]
				};
				newItem.ItemClick += barItemOperator_ItemClick;
				this.popupMenuOperation.AddItem(newItem);
			}
		}
		void UpdateTableLabel(Dictionary<string, List<string>> objectNames, LabelControl labelTable, string tableName) {
			if(String.IsNullOrEmpty(tableName)) {
				labelTable.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorEmptyTableText);
				labelTable.ForeColor = this.foreColorEmptyValue;
			} else {
				if(objectNames.ContainsKey(tableName)) {
					labelTable.ToolTip = string.Empty;
					labelTable.ForeColor = this.foreColor;
				} else {
					labelTable.ToolTip = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorInvalidQueryNameMessage), tableName);
					labelTable.ForeColor = this.foreColorInvalidValue;
				}
				labelTable.Text = string.Format("[{0}]", GetTableDisplayName(tableName));
			}
		}
		void UpdateRightTableLabel() {
			UpdateTableLabel(this.rightObjectNames, this.labelRightTable, this.rightTableName);
		}
		void UpdateLeftTableLabel() {
			UpdateTableLabel(this.leftObjectNames, this.labelLeftTable, this.leftTableName);
		}
		void UpdateColumnLabel(Dictionary<string, List<string>> objectNames, LabelControl labelColumn, LabelControl labelPoint, string tableName, string columnName) {
			if(String.IsNullOrEmpty(tableName)) {
				labelColumn.Visible = false;
				labelPoint.Visible = false;
			} else {
				labelColumn.Visible = true;
				labelPoint.Visible = true;
				if(String.IsNullOrEmpty(columnName)) {
					labelColumn.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.JoinEditorEmptyColumnText);
					labelColumn.ForeColor = this.foreColorEmptyValue;
				} else {
					if(objectNames.ContainsKey(tableName)) {
						if(objectNames.Single(p => p.Key == tableName).Value.Any(c => c == columnName)) {
							labelColumn.ToolTip = string.Empty;
							labelColumn.ForeColor = this.foreColor;
						} else {
							labelColumn.ToolTip = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorInvalidColumnNameMessage), columnName);
							labelColumn.ForeColor = this.foreColorInvalidValue;
						}
					} else {
						labelColumn.ToolTip = string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.MasterDetailEditorInvalidColumnQueryNameMessage), tableName);
						labelColumn.ForeColor = this.foreColorInvalidValue;
					}
					labelColumn.Text = string.Format("[{0}]", GetColumnDisplayName(tableName, columnName));
				}
			}
		}
		void UpdateRightColumnLabel() {
			UpdateColumnLabel(this.rightObjectNames, this.labelRightColumn, this.labelRightPoint, RightTableName, RightColumnName);
		}
		void UpdateLeftColumnLabel() {
			UpdateColumnLabel(this.leftObjectNames, this.labelLeftColumn, this.labelLeftPoint, LeftTableName, LeftColumnName);
		}
		void UpdateOperatorText(BinaryOperatorType operatorType) {
			this.buttonOperator.Images = new[] {this.images[operatorType][1], this.images[operatorType][1]};
		}
		void UpdateSkinColors() {
			Color backColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("Window");
			this.foreColor = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("WindowText");
			this.foreColorEmptyValue = EditorsSkins.GetSkin(LookAndFeel).Colors.GetColor(EditorsSkins.SkinFilterControlEmptyValueTextColor);
			this.foreColorInvalidValue = CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("Critical");
			if(this.foreColorEmptyValue.IsEmpty)
				this.foreColorEmptyValue = this.defaultColorEmptyValue;
			foreach(Control control in this.alignedControls) {
				control.BackColor = backColor;
			}
			UpdateLeftTableLabel();
			UpdateRightTableLabel();
			UpdateLeftColumnLabel();
			UpdateRightColumnLabel();
			UpdateOperatorText(OperatorType);
			AlignControls();
		}
		void RefreshRightColumnsMenu() {
			if(RightTableName == null)
				return;
			List<string> columns;
			if(RightObjectNames.TryGetValue(RightTableName, out columns))
				RefreshMenu(this.popupMenuRightColumn, columns.Select(c => new JoinMenuItem(c, c == RightColumnName, GetColumnDisplayName(RightTableName, c))), RightColumn_ItemClick);
		}
		void RefreshLeftColumnsMenu() {
			if(LeftTableName == null)
				return;
			List<string> columns;
			if(LeftObjectNames.TryGetValue(LeftTableName, out columns))
				RefreshMenu(this.popupMenuLeftColumn, columns.Select(c => new JoinMenuItem(c, c == LeftColumnName, GetColumnDisplayName(LeftTableName, c))), LeftColumn_ItemClick);
		}
		void RefreshLeftTablesMenu() {
			RefreshMenu(this.popupMenuLeftTable, LeftObjectNames.Where(o => o.Key != RightTableName).Select(o => new JoinMenuItem(o.Key, o.Key == LeftTableName, GetTableDisplayName(o.Key))), LeftTable_ItemClick);
		}
		void RefreshRightTablesMenu() {
			RefreshMenu(this.popupMenuRightTable, RightObjectNames.Where(o => o.Key != LeftTableName).Select(o => new JoinMenuItem(o.Key, o.Key == RightTableName, GetTableDisplayName(o.Key))), RightTable_ItemClick);
		}
		void RefreshMenu(PopupMenu menu, IEnumerable<JoinMenuItem> items, ItemClickEventHandler handler) {
			List<BarItem> deletedItems = new List<BarItem>(menu.ItemLinks.Count);
			foreach(BarItemLink link in menu.ItemLinks) {
				link.Item.ItemClick -= handler;
				deletedItems.Add(link.Item);
			}
			deletedItems.ForEach(item => this.barManager.Items.Remove(item));
			menu.ClearLinks();
			foreach(JoinMenuItem item in items) {
				BarCheckItem newItem = new BarCheckItem(this.barManager, item.Checked) {Caption = item.DisplayName, Name = item.Name};
				newItem.ItemClick += handler;
				menu.AddItem(newItem);
			}
		}
		void AlignControls() {
			for(int i = 1; i < this.alignedControls.Length; i++)
				this.alignedControls[i].Left = this.alignedControls[i - 1].Right + padding;
		}
		string GetTableDisplayName(string tableName) {
			if(displayNameProvider == null)
				return tableName;
			try {
				string displayName = displayNameProvider.GetFieldDisplayName(new[] { tableName });
				if(string.IsNullOrEmpty(displayName))
					return tableName;
				return displayName;
			}
			catch { return tableName; }
		}
		string GetColumnDisplayName(string tableName, string columnName) {
			if(displayNameProvider == null)
				return columnName;
			try {
				string displayName = displayNameProvider.GetFieldDisplayName(new[] { tableName, columnName });
				if(string.IsNullOrEmpty(displayName))
					return columnName;
				return displayName;
			}
			catch {
				return columnName;
			}
		}
		void UnderlineControl_MouseDown(object sender, MouseEventArgs e) {
			if(UnderlineControlMouseDown != null)
				UnderlineControlMouseDown(this, e);
		}
		void UnderlineLabel_MouseMove(object sender, MouseEventArgs e) {
			LabelControl label = sender as LabelControl;
			if(label == null)
				return;
			if(label == this.labelLeftTable && !AllowChangeMasterTable)
				return;
			label.Capture = label.Parent.RectangleToScreen(label.Bounds).Contains(label.PointToScreen(e.Location));
			label.Font = label.Capture ? this.underlineFont : this.normalFont;
		}
		void LeftTable_ItemClick(object sender, ItemClickEventArgs e) {
			LeftTableName = e.Item.Name;
			RefreshLeftTablesMenu();
		}
		void LeftColumn_ItemClick(object sender, ItemClickEventArgs e) {
			LeftColumnName = e.Item.Name;
			RefreshLeftColumnsMenu();
		}
		void RightTable_ItemClick(object sender, ItemClickEventArgs e) {
			RightTableName = e.Item.Name;
			RefreshRightTablesMenu();
		}
		void RightColumn_ItemClick(object sender, ItemClickEventArgs e) {
			RightColumnName = e.Item.Name;
			RefreshRightColumnsMenu();
		}
		void barItemOperator_ItemClick(object sender, ItemClickEventArgs e) {
			BinaryOperatorType type = (BinaryOperatorType)e.Item.Tag;
			OperatorType = type;
			foreach(BarCheckItemLink item in this.popupMenuOperation.ItemLinks) {
				((BarCheckItem)item.Item).Checked = ((BinaryOperatorType)((BarCheckItem)item.Item).Tag) == type;
			}
			UpdateOperatorText(type);
			AlignControls();
		}
		void labelLeftTable_MouseClick(object sender, MouseEventArgs e) {
			if(this.allowChangeLeftTable) {
				LabelControl label = sender as LabelControl;
				if(label != null && label.Capture) {
					label.Capture = false;
					label.Font = this.normalFont;
				}
				this.popupMenuLeftTable.ShowPopup(this.labelLeftTable.PointToScreen(e.Location));
			}
		}
		void labelLeftColumn_MouseClick(object sender, MouseEventArgs e) {
			LabelControl label = sender as LabelControl;
			if(label != null && label.Capture) {
				label.Capture = false;
				label.Font = this.normalFont;
			}
			this.popupMenuLeftColumn.ShowPopup(this.labelLeftColumn.PointToScreen(e.Location));
		}
		void labelRightTable_MouseClick(object sender, MouseEventArgs e) {
			LabelControl label = sender as LabelControl;
			if(label != null && label.Capture) {
				label.Capture = false;
				label.Font = this.normalFont;
			}
			this.popupMenuRightTable.ShowPopup(this.labelRightTable.PointToScreen(e.Location));
		}
		void labelRightColumn_MouseClick(object sender, MouseEventArgs e) {
			LabelControl label = sender as LabelControl;
			if(label != null && label.Capture) {
				label.Capture = false;
				label.Font = this.normalFont;
			}
			this.popupMenuRightColumn.ShowPopup(this.labelRightColumn.PointToScreen(e.Location));
		}
		void buttonOperator_MouseClick(object sender, MouseEventArgs e) {
			if(AllowChangeOperatorType) {
				this.buttonOperator.Capture = false;
				this.popupMenuOperation.ShowPopup(this.buttonOperator.PointToScreen(e.Location));
			}
		}
		void removeItemButton_MouseClick(object sender, MouseEventArgs e) {
			if(RemoveButtonClick != null)
				RemoveButtonClick(this, EventArgs.Empty);
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			UpdateSkinColors();
		}
	}
	public class TableNameChangedEventArgs : EventArgs {
		public string OldName { get; set; }
		public TableNameChangedEventArgs(string oldName) {
			OldName = oldName;
		}
	}
}
