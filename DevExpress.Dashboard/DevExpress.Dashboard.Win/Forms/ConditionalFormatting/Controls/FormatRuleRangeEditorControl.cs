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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	class FormatRuleRangeEditorControl : DashboardUserControl {
		public const string Infinity = "∞";
		bool lockCellValueChanging = false;
		bool isPercent;
		RepositoryItemComboBox minInfinityComboBox;
		DataFieldType dataType;
		public event EventHandler RangeViewChanged;
		public event FormatRuleRangeViewChangingEventHandler RangeViewChanging;
		public IList<IFormatRuleRangeView> Ranges {
			get { return (IList<IFormatRuleRangeView>)gridRanges.DataSource; }
			set {
				int topRowIndex = gridViewRanges.TopRowIndex;
				IList<IFormatRuleRangeView> oldRanges = gridRanges.DataSource as IList<IFormatRuleRangeView>;
				bool restorePosition = oldRanges != null && oldRanges.Count == value.Count;
				if(value != null && value.Count > 0 && object.Equals(value[value.Count - 1].RightValue, RangeInfo.NegativeInfinity))
					value[value.Count - 1].RightValue = new MinInfinityItem(this).ToString();
				gridRanges.DataSource = value;
				if(restorePosition)
					gridViewRanges.TopRowIndex = topRowIndex;
				gridRanges.Update();
			}
		}
		public bool IsPercent {
			get { return isPercent; }
			set {
				isPercent = value;
				ApplyMask(gridColumnLeft.ColumnEdit);
				ApplyMask(gridColumnRight.ColumnEdit);
				ApplyMask(minInfinityComboBox);
				gridRanges.Invalidate();
			}
		}
		public FormatRuleRangeEditorControl()  {
			InitializeComponent();
		}
		public void Insert(IFormatRuleRangeView rangeView) {
			int index = gridViewRanges.FocusedRowHandle;
			IList<IFormatRuleRangeView> newRanges = Ranges;
			if(index != GridControl.InvalidRowHandle)
				newRanges.Insert(index + 1, rangeView);
			else
				newRanges.Add(rangeView);
			gridRanges.RefreshDataSource();
		}
		public void Delete() {
			int index = gridViewRanges.FocusedRowHandle;
			int indexToDelete = -1;
			if (index != GridControl.InvalidRowHandle) 
				indexToDelete = index;
			 else if (gridViewRanges.DataRowCount > 0) 
				indexToDelete = gridViewRanges.DataRowCount - 1;
			if(indexToDelete >= 0) {
				gridViewRanges.DeleteRow(indexToDelete);
				if(indexToDelete == 0)
					Ranges[0].LeftValue = null;
				else if(indexToDelete < Ranges.Count)
					Ranges[indexToDelete].LeftValue = Ranges[indexToDelete - 1].RightValue;
			}
			gridRanges.Update();
		}
		public void ReverseStyles() {
			IList<IFormatRuleRangeView> ranges = Ranges;
			for(int i = 0; i < ranges.Count / 2; i++) {
				int index1 = i;
				int index2 = ranges.Count - 1 - i;
				IStyleSettings style1 = ((IStyleSettings)ranges[index1].Style.ToCoreStyle()).Clone();
				IStyleSettings style2 = ((IStyleSettings)ranges[index2].Style.ToCoreStyle()).Clone();
				ranges[index1].Style = StyleSettingsContainer.ToStyleContainer(style2, ranges[index2].Style.Mode);
				ranges[index2].Style = StyleSettingsContainer.ToStyleContainer(style1, ranges[index1].Style.Mode);
			}
			gridRanges.RefreshDataSource();
		}
		public void Initialize(DataFieldType dataType, DateTimeGroupInterval groupInterval) {
			this.dataType = dataType;
			FormatRuleRepositoryItemPopupContainerEdit riStyleEdit = new FormatRuleRepositoryItemPopupContainerEdit();
			gridRanges.RepositoryItems.Add(riStyleEdit);
			gridColumnStyle.ColumnEdit = riStyleEdit;
			riStyleEdit.EditValueChanged += riStyleEdit_EditValueChanged;
			RepositoryItemComboBox riCombo = new RepositoryItemComboBox();
			riCombo.TextEditStyle = TextEditStyles.DisableTextEditor;
			riCombo.CustomDisplayText += riCombo_CustomDisplayText;
			EnumManager.Iterate<DashboardFormatConditionComparisonType>((type) => riCombo.Items.Add(ComparisonTypeItem.Wrap(type)));
			gridRanges.RepositoryItems.Add(riCombo);
			gridColumnComparisonType.ColumnEdit = riCombo;
			minInfinityComboBox = new RepositoryItemComboBox();
			minInfinityComboBox.Items.Add(new MinInfinityItem(this));
			minInfinityComboBox.Closed += OnMinInfinityComboBoxClosed;
			gridRanges.RepositoryItems.Add(minInfinityComboBox);
			if(dataType == DataFieldType.DateTime) {
				RepositoryItemDateEdit riDate = new RepositoryItemDateEdit();
				riDate.VistaEditTime = DateTimeHelper.GetEditTime(groupInterval);
				riDate.VistaCalendarViewStyle = DateTimeHelper.GetCalendarStyle(groupInterval);
				gridRanges.RepositoryItems.Add(riDate);
				gridColumnLeft.ColumnEdit = riDate;
				gridColumnRight.ColumnEdit = riDate;
			}
			else {
				RepositoryItemTextEdit ri = new RepositoryItemTextEdit();
				ri.ParseEditValue += OnColumnValueEditParseEditValue;
				gridColumnLeft.ColumnEdit = ri;
				gridColumnRight.ColumnEdit = ri;
			}
			gridColumnStyle.FieldName = "Style";
			gridColumnLeft.FieldName = "LeftValue";
			gridColumnComparisonType.FieldName = "ComparisonTypeItem";
			gridColumnRight.FieldName = "RightValue";
			gridViewRanges.CellValueChanged += OnGridViewRangesCellValueChanged;
			gridViewRanges.ShowingEditor += OnGridViewRangesShowingEditor;
			gridViewRanges.CustomColumnDisplayText += OnGridViewRangesCustomColumnDisplayText;
			gridViewRanges.CustomRowCellEditForEditing += OnGridViewRangesCustomRowCellEditForEditing;
			gridViewRanges.RowCellStyle += OnGridViewRangesRowCellStyle;
			gridViewRanges.ValidatingEditor += OnGridViewRangesValidatingEditor;
		}
		void ApplyMask(RepositoryItem item) {
			RepositoryItemTextEdit edit = item as RepositoryItemTextEdit;
			if(edit != null) {
				edit.Mask.UseMaskAsDisplayFormat = true;
				if(isPercent) {
					edit.Mask.MaskType = MaskType.Numeric;
					edit.Mask.EditMask = "P";
				}
				else if(dataType == DataFieldType.Integer) {
					edit.Mask.MaskType = MaskType.Numeric;
					edit.Mask.EditMask = "n0";
				}
				else if(dataType == DataFieldType.Double || dataType == DataFieldType.Float || dataType == DataFieldType.Decimal) {
					edit.Mask.MaskType = MaskType.Numeric;
					edit.Mask.EditMask = "###,###,###,###,##0.00############";
				}
				else
					edit.Mask.MaskType = MaskType.None;
			}
		}
		void OnMinInfinityComboBoxClosed(object sender, ClosedEventArgs e) {
			gridViewRanges.FocusedColumn = null;
		}
		void OnColumnValueEditParseEditValue(object sender, ConvertEditValueEventArgs e) {
			e.Value = DevExpress.DashboardCommon.Native.Helper.ConvertToType(e.Value, dataType);
		}
		void OnGridViewRangesRowCellStyle(object sender, RowCellStyleEventArgs e) {
			if(e.Column == gridColumnLeft && e.RowHandle == 0)
				e.Appearance.ForeColor = Color.Gray;
		}
		void OnGridViewRangesCustomColumnDisplayText(object sender, XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e) {
			if(e.Column == gridColumnLeft && e.ListSourceRowIndex == 0)
				e.DisplayText = IsPercent ? "100.00 %" : Infinity;
			if(e.Column == gridColumnRight && e.ListSourceRowIndex == gridViewRanges.RowCount - 1 &&  MinInfinityItem.Is(e.Value))
				e.DisplayText = e.Value.ToString();
		}
		void OnGridViewRangesCustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e) {
			if(e.Column == gridColumnRight && e.RowHandle == gridViewRanges.RowCount - 1)
				e.RepositoryItem = minInfinityComboBox;
		}
		void OnGridViewRangesShowingEditor(object sender, CancelEventArgs e) {
			e.Cancel = gridViewRanges.FocusedColumn == gridColumnLeft && gridViewRanges.FocusedRowHandle == 0;
		}
		void riStyleEdit_EditValueChanged(object sender, EventArgs e) {
			 gridViewRanges.PostEditor();
		}
		void OnGridViewRangesCellValueChanged(object sender, XtraGrid.Views.Base.CellValueChangedEventArgs e) {
			if(!lockCellValueChanging) {
				GridColumn targetColumn = null;
				int targetChangingIndex = e.RowHandle;
				int currentValueIndex = e.RowHandle;
				if(e.Column == gridColumnLeft) {
					targetColumn = gridColumnRight;
					targetChangingIndex -= 1;
					currentValueIndex -= 1;
				}
				if(e.Column == gridColumnRight) {
					targetColumn = gridColumnLeft;
					targetChangingIndex += 1;
				}
				bool validIndex = targetChangingIndex >= 0 && targetChangingIndex < gridViewRanges.RowCount;
				if(targetColumn != null) {
					if(validIndex) {
						lockCellValueChanging = true;
						gridViewRanges.SetRowCellValue(targetChangingIndex, targetColumn, e.Value);
						RaiseRangeViewChanging(currentValueIndex, e.Value);
						lockCellValueChanging = false;
					}
					else
						RaiseRangeViewChanging(currentValueIndex, e.Value);
				}
				if(e.Column == gridColumnStyle)
					RaiseRangeViewChanging(targetChangingIndex, (StyleSettingsContainer)e.Value);
				if(e.Column == gridColumnComparisonType)
					RaiseRangeViewChanging(targetChangingIndex, ((ComparisonTypeItem)e.Value).ComparisonType);
				RaiseRangeViewChanged();
			}
		}
		void OnGridViewRangesValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e) {
			if(MinInfinityItem.Is(e.Value) || gridViewRanges.FocusedColumn != gridColumnLeft && gridViewRanges.FocusedColumn != gridColumnRight)
				return;
			object maxAvailable = null;
			object minAvailable = null;
			if(gridViewRanges.FocusedColumn == gridColumnLeft && gridViewRanges.FocusedRowHandle > 0) {
				maxAvailable = Ranges[gridViewRanges.FocusedRowHandle - 1].LeftValue;
				minAvailable = Ranges[gridViewRanges.FocusedRowHandle].RightValue;
			}
			if(gridViewRanges.FocusedColumn == gridColumnRight) {
				maxAvailable = Ranges[gridViewRanges.FocusedRowHandle].LeftValue;
				if(gridViewRanges.FocusedRowHandle + 1 < gridViewRanges.RowCount)
					minAvailable = Ranges[gridViewRanges.FocusedRowHandle + 1].RightValue;
			}
			if(MinInfinityItem.Is(minAvailable))
				minAvailable = null;
			if(IsPercent) {
				if(maxAvailable == null)
					maxAvailable = 100;
				if(minAvailable == null)
					minAvailable = 0;
			}
			if(maxAvailable != null && ValueManager.CompareValues(maxAvailable, e.Value, dataType) <= 0) 
				e.Valid = false;
			if(minAvailable != null && ValueManager.CompareValues(minAvailable, e.Value, dataType) >= 0) 
				e.Valid = false;
			if(!e.Valid) {
				if(minAvailable != null && maxAvailable != null)
					e.ErrorText = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeEditorControlBetweenValidateMessage), minAvailable, maxAvailable);
				else if(minAvailable != null)
					e.ErrorText = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeEditorControlGreaterValidateMessage), minAvailable);
				else if(maxAvailable != null)
					e.ErrorText = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.RangeEditorControlLessValidateMessage), maxAvailable);
			}
		}
		void riCombo_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
		}
		void RaiseRangeViewChanging(int index, object value) {
			if(RangeViewChanging != null)
				RangeViewChanging(this, new FormatRuleRangeViewChangingEventArgs(index, value));
		}
		void RaiseRangeViewChanging(int index, DashboardFormatConditionComparisonType valueComparison) {
			if(RangeViewChanging != null)
				RangeViewChanging(this, new FormatRuleRangeViewChangingEventArgs(index, valueComparison));
		}
		void RaiseRangeViewChanging(int index, StyleSettingsContainer style) {
			if(RangeViewChanging != null)
				RangeViewChanging(this, new FormatRuleRangeViewChangingEventArgs(index, style));
		}
		void RaiseRangeViewChanged() {
			if(RangeViewChanged != null)
				RangeViewChanged(this, new EventArgs());
		}
		#region Windows Form Designer generated code
		private GridControl gridRanges;
		private GridColumn gridColumnStyle;
		private GridColumn gridColumnLeft;
		private GridColumn gridColumnComparisonType;
		private GridColumn gridColumnRight;
		private GridView gridViewRanges;
		private void InitializeComponent() {
			this.gridRanges = new DevExpress.XtraGrid.GridControl();
			this.gridViewRanges = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnStyle = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnLeft = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnComparisonType = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnRight = new DevExpress.XtraGrid.Columns.GridColumn();
			((System.ComponentModel.ISupportInitialize)(this.gridRanges)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewRanges)).BeginInit();
			this.SuspendLayout();
			this.gridRanges.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridRanges.Location = new System.Drawing.Point(0, 0);
			this.gridRanges.MainView = this.gridViewRanges;
			this.gridRanges.Name = "gridRanges";
			this.gridRanges.Size = new System.Drawing.Size(250, 200);
			this.gridRanges.TabIndex = 0;
			this.gridRanges.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewRanges});
			this.gridViewRanges.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.gridViewRanges.Appearance.Row.Options.UseFont = true;
			this.gridViewRanges.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnStyle,
			this.gridColumnLeft,
			this.gridColumnComparisonType,
			this.gridColumnRight});
			this.gridViewRanges.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
			this.gridViewRanges.GridControl = this.gridRanges;
			this.gridViewRanges.Name = "gridViewRanges";
			this.gridViewRanges.OptionsCustomization.AllowColumnMoving = false;
			this.gridViewRanges.OptionsCustomization.AllowColumnResizing = false;
			this.gridViewRanges.OptionsCustomization.AllowFilter = false;
			this.gridViewRanges.OptionsCustomization.AllowGroup = false;
			this.gridViewRanges.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridViewRanges.OptionsCustomization.AllowSort = false;
			this.gridViewRanges.OptionsFilter.AllowColumnMRUFilterList = false;
			this.gridViewRanges.OptionsFilter.AllowFilterEditor = false;
			this.gridViewRanges.OptionsFilter.AllowFilterIncrementalSearch = false;
			this.gridViewRanges.OptionsFilter.AllowMRUFilterList = false;
			this.gridViewRanges.OptionsFilter.AllowMultiSelectInCheckedFilterPopup = false;
			this.gridViewRanges.OptionsFind.AllowFindPanel = false;
			this.gridViewRanges.OptionsFind.HighlightFindResults = false;
			this.gridViewRanges.OptionsMenu.EnableColumnMenu = false;
			this.gridViewRanges.OptionsMenu.EnableFooterMenu = false;
			this.gridViewRanges.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridViewRanges.OptionsMenu.ShowAutoFilterRowItem = false;
			this.gridViewRanges.OptionsMenu.ShowDateTimeGroupIntervalItems = false;
			this.gridViewRanges.OptionsMenu.ShowGroupSortSummaryItems = false;
			this.gridViewRanges.OptionsMenu.ShowSplitItem = false;
			this.gridViewRanges.OptionsView.ShowColumnHeaders = false;
			this.gridViewRanges.OptionsView.ShowGroupPanel = false;
			this.gridViewRanges.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewRanges.OptionsView.ShowIndicator = false;
			this.gridViewRanges.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewRanges.RowHeight = 24;
			this.gridColumnStyle.Name = "gridColumnStyle";
			this.gridColumnStyle.Visible = true;
			this.gridColumnStyle.VisibleIndex = 0;
			this.gridColumnLeft.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumnLeft.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.gridColumnLeft.Name = "gridColumnLeft";
			this.gridColumnLeft.Visible = true;
			this.gridColumnLeft.VisibleIndex = 1;
			this.gridColumnComparisonType.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumnComparisonType.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.gridColumnComparisonType.Name = "gridColumnComparisonType";
			this.gridColumnComparisonType.Visible = true;
			this.gridColumnComparisonType.VisibleIndex = 2;
			this.gridColumnRight.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumnRight.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.gridColumnRight.Name = "gridColumnRight";
			this.gridColumnRight.Visible = true;
			this.gridColumnRight.VisibleIndex = 3;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Controls.Add(this.gridRanges);
			this.Name = "FormatRuleRangeEditorControl";
			this.Size = new System.Drawing.Size(250, 200);
			((System.ComponentModel.ISupportInitialize)(this.gridRanges)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewRanges)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
	#region Inner Controls
	[UserRepositoryItem("RegisterDashboardFormatRulePopupContainerEdit")]
	class FormatRuleRepositoryItemPopupContainerEdit : RepositoryItemPopupContainerEdit {
		internal const string DashboardFormatRuleRepositoryItemPopupContainerEditName = "FormatRulePopupContainerEdit";
		static FormatRuleRepositoryItemPopupContainerEdit() { RegisterDashboardFormatRulePopupContainerEdit(); }
		public static void RegisterDashboardFormatRulePopupContainerEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(DashboardFormatRuleRepositoryItemPopupContainerEditName,
			  typeof(FormatRulePopupContainerEdit), typeof(FormatRuleRepositoryItemPopupContainerEdit),
			  typeof(FormatRulePopupContainerEditViewInfo), new FormatRulePopupContainerEditPainter(), true, null, typeof(PopupEditAccessible)));
		}
		public override string EditorTypeName { get { return DashboardFormatRuleRepositoryItemPopupContainerEditName; } }
		public override bool ShowPopupShadow { get { return true; } set { } }
		public FormatRuleRepositoryItemPopupContainerEdit() {
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			return string.Empty;
		}
	}
	[DXToolboxItem(false)]
	class FormatRulePopupContainerEdit : PopupContainerEdit {
		static FormatRulePopupContainerEdit() { FormatRuleRepositoryItemPopupContainerEdit.RegisterDashboardFormatRulePopupContainerEdit(); }
		public override string EditorTypeName { get { return FormatRuleRepositoryItemPopupContainerEdit.DashboardFormatRuleRepositoryItemPopupContainerEditName; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new FormatRuleRepositoryItemPopupContainerEdit Properties { get { return base.Properties as FormatRuleRepositoryItemPopupContainerEdit; } }
		public FormatRulePopupContainerEdit() {
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new FormatRuleStyleInfoEditForm(this);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(PopupForm != null)
				((FormatRuleStyleInfoEditForm)PopupForm).StyleManager.Style = (StyleSettingsContainer)EditValue;
		}
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			return e.Value;
		}
	}
	class FormatRulePopupContainerEditViewInfo : PopupContainerEditViewInfo {
		const int DefaultStyleRectSize = 16;
		Rectangle styleBoxRect;
		StyleSettingsContainer style;
		public new FormatRuleRepositoryItemPopupContainerEdit Item { get { return base.Item as FormatRuleRepositoryItemPopupContainerEdit; } }
		public StyleSettingsContainer Style { get { return style; } }
		public Rectangle StyleBoxRect { get { return styleBoxRect; } }
		public FormatRulePopupContainerEditViewInfo(RepositoryItem item) : base(item) {
		}
		protected override void Assign(BaseControlViewInfo info) {
			base.Assign(info);
			FormatRulePopupContainerEditViewInfo source = info as FormatRulePopupContainerEditViewInfo;
			if(source != null) {
				this.style.Assign(source.Style);
				this.styleBoxRect = source.styleBoxRect;
			}
		}
		public override void Reset() {
			this.style = StyleSettingsContainer.CreateDefaultEmpty(Style == null ? StyleMode.Appearance : Style.Mode);
			base.Reset();
		}
		public override void Clear() {
			this.styleBoxRect = Rectangle.Empty;
			base.Clear();
		}
		public override void Offset(int x, int y) {
			base.Offset(x, y);
			if(!StyleBoxRect.IsEmpty) styleBoxRect.Offset(x, y);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			this.RefreshDisplayText = false;
			this.style = (StyleSettingsContainer)EditValue;
		}
		protected override void CalcClientRect(Rectangle bounds) {
			base.CalcClientRect(bounds);
			CalcStyleBoxRect();
		}
		protected virtual void CalcStyleBoxRect() {
			Rectangle contentRect = ClientRect;
			int buttonWidth = 16;
			styleBoxRect = new Rectangle(contentRect.X + (contentRect.Width - DefaultStyleRectSize - buttonWidth) / 2,
				contentRect.Y + (contentRect.Height - DefaultStyleRectSize) / 2, 
				DefaultStyleRectSize, 
				DefaultStyleRectSize);
		}
	}
	class FormatRuleStyleInfoEditForm : PopupBaseForm {
		public FormatRuleStyleInfoEditForm()
			: this(null) {
		}
		public FormatRuleStyleInfoEditForm(FormatRulePopupContainerEdit ownerEdit) : base(ownerEdit) {
			StyleSettingsContainer style = (StyleSettingsContainer)ownerEdit.EditValue;
			Control control = new FormatRuleStyleSettingsControl(style.Mode, style.Mode != StyleMode.GradientNonemptyStop && style.Mode != StyleMode.BarGradientNonemptyStop, LookAndFeel);
			Controls.Add(control);
			StyleManager.Style = style;
			StyleManager.StyleChanged += OnSelectedStyleChanged;
		}
		internal IStyleContainerManager StyleManager {
			get {
				foreach(Control control in Controls) {
					IStyleContainerManager styleManager = control as IStyleContainerManager;
					if(styleManager != null)
						return styleManager;
				}
				throw new ArgumentNullException("There is no control to select style");
			}
		}
		public override object ResultValue { get { return StyleManager.Style; } }
		protected override Size CalcFormSizeCore() {
			return Controls[0].MinimumSize;
		}
		protected virtual void OnSelectedStyleChanged(object sender, StyleSettingsContainerItemChangedEventArgs e) {
			OwnerEdit.EditValue = e.New;
			this.Close();
		}
	}
	class FormatRulePopupContainerEditPainter : ButtonEditPainter {
		protected override void DrawButtons(ControlGraphicsInfoArgs info) {
			DrawColorBox(info);
			base.DrawButtons(info);
		}
		void DrawColorBox(ControlGraphicsInfoArgs info) {
			FormatRulePopupContainerEditViewInfo vi = info.ViewInfo as FormatRulePopupContainerEditViewInfo;
			StyleSettingsContainerPainter.Draw(vi.Style, info, vi.StyleBoxRect);
		}
	}
	#endregion Inner Controls
}
