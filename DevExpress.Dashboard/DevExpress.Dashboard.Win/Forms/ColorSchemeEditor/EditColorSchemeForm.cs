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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Localization;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.DashboardWin.Coloring;
namespace DevExpress.DashboardWin.Native {
	public partial class EditColorSchemeForm : DashboardForm {
		List<ColorTableGridRow> gridDataSource;
		EditColorSchemeFormController formController;
		List<FormatterBase> formatters = new List<FormatterBase>();
		IList<DataSourceInfo> dataSources;
		ColorRepositoryKey Key { get { return (ColorRepositoryKey)cbColorScheme.EditValue; } }
		public EditColorSchemeForm() {
			InitializeComponent();
		}
		public EditColorSchemeForm(EditColorSchemeFormController formController, UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
			InitializeComponent();
			this.formController = formController;
			this.dataSources = formController.GetDataSourceInfos();
			LoadColorSchemeNames();
			btnNewColorScheme.Enabled = dataSources.Count != 0;
		}
		List<ColorTableGridRow> PrepareDataSource() {
			if(Key == null) return null;
			gridDataSource = new List<ColorTableGridRow>();
			ColorTableServerKeyComparer tableKeyComparer = new ColorTableServerKeyComparer();
			IEnumerable<ColorTableGridRow> userColors = formController.GetUserColors(Key);
			IEnumerable<ColorTableGridRow> dashboardColors = formController.GetCacheColors(Key);
			if(userColors != null) {
				gridDataSource.AddRange(userColors);
				if(dashboardColors != null)
					dashboardColors = dashboardColors
						.Where(entry => !userColors.Any(userEntry => tableKeyComparer.Equals(userEntry.Key, entry.Key)));
			}
			if(dashboardColors != null) {
				dashboardColors = dashboardColors.Select(entry => new ColorTableGridRow() {
					Key = entry.Key,
					Value = GetActualCacheColor(entry.Key, entry.Value)
				});
				gridDataSource.AddRange(dashboardColors);
			}
			return gridDataSource;
		}
		DesignerColor GetActualCacheColor(ColorTableServerKey tableKey, DesignerColor color) {
			DesignerColor inheritedColor = formController.GetInheritedColor(Key, tableKey);
			if(inheritedColor != null) {
				return inheritedColor;
			}
			return color.GetDefinition() is AutoAssignedColor ? color : new NotAppliedColor();
		}
		void BindColorGrid() {
			gridView1.Columns[0].SortMode = ColumnSortMode.Value;
			gridView1.Columns[0].SortOrder = ColumnSortOrder.Ascending;
			gridView1.Columns[0].FieldName = "Key";
			gridView1.Columns[1].FieldName = "Value";
			RefreshColorGridDataSource();
		}
		void RefreshColorGridDataSource() {
			gcColors.DataSource = PrepareDataSource();
			gcColors.Update();
		}
		void OnFormLoad(object sender, EventArgs e) {
			DashboardRepositoryItemColorEdit repositoryItem = new DashboardRepositoryItemColorEdit();
			gcColors.RepositoryItems.Add(repositoryItem);
			gridView1.Columns[1].ColumnEdit = repositoryItem;
		}
		void LoadColorSchemeNames() {
			List<ColorSchemeDataSourceRow> colorSchemeDataSource = new List<ColorSchemeDataSourceRow>();
			foreach(ColorRepositoryKey key in formController.ColorSchemeKeys) {
				colorSchemeDataSource.Add(new ColorSchemeDataSourceRow() { Key = key, DisplayText = ColorRepositoryKeyDisplayTextProvider.GetDisplayText(key, formController.DataInfoProvider) });
			}
			cbColorScheme.Properties.ValueMember = "Key";
			cbColorScheme.Properties.DisplayMember = "DisplayText";
			cbColorScheme.Properties.DataSource = colorSchemeDataSource;
			if(colorSchemeDataSource.Count > 0)
				cbColorScheme.EditValue = colorSchemeDataSource[0].Key;
			cbColorScheme.Properties.Columns.Clear();
			cbColorScheme.Properties.Columns.Add(new LookUpColumnInfo("DisplayText"));
			UpdateDeleteButtonState();
			UpdateNewValueButtonState();
		}
		void SaveColor(DesignerColor color) {
			if(gridView1.FocusedRowHandle < 0) return;
			ColorTableServerKey tableKey = (ColorTableServerKey)gridView1.GetFocusedRowCellValue(gridView1.Columns[0]);
			formController.SetColor(Key, tableKey, color);
		}
		void SetColor(DesignerColor color) {
			SaveColor(color);
			RefreshColorGridDataSource();
		}
		public void RetainColor() {
			DesignerColor color = gridView1.GetFocusedRowCellValue(gridView1.Columns[1]) as DesignerColor;
			if(color != null) {
				PaletteColor paletteColor = color.GetDefinition() as PaletteColor;
				if(paletteColor != null)
					SetColor(new DesignerColor(new PaletteColor(paletteColor.ColorIndex)));
			}
		}
		public void ClearColor() {
			if(gridView1.FocusedRowHandle < 0) return;
			ColorTableServerKey tableKey = (ColorTableServerKey)gridView1.GetFocusedRowCellValue(gridView1.Columns[0]);
			formController.RemoveColor(Key, tableKey);
			RefreshColorGridDataSource();
		}
		void btnNewColorScheme_Click(object sender, EventArgs e) {
			using(NewColorTableDialog newColorSchemeDialog =
				new NewColorTableDialog(LookAndFeel, dataSources, formController.ServiceProvider, formController.ColorSchemeKeys)) {
				if(newColorSchemeDialog.ShowDialog() == DialogResult.OK) {
					ColorRepositoryKey newKey = newColorSchemeDialog.ColorRepositoryKey;
					if(newKey.DimensionDefinitions.Count > 0) {
						formController.AddTable(newKey);
						LoadColorSchemeNames();
						cbColorScheme.EditValue = newKey;
					}
				}
			}
		}
		void btnRemoveColorScheme_Click(object sender, EventArgs e) {
			if(cbColorScheme.EditValue == null) return;
			formController.RemoveTable(Key);
			LoadColorSchemeNames();
		}
		void btnNewValue_Click(object sender, EventArgs e) {
			if(Key == null)
				return;
			using(NewColorRecordDialog newColorRecordDialog =
				new NewColorRecordDialog(LookAndFeel, formController.DataInfoProvider, formController.ServiceProvider, Key, gridDataSource.Select(row => row.Key))) {
				if(newColorRecordDialog.ShowDialog() == DialogResult.OK) {
					ColorTableServerKey newKey = newColorRecordDialog.ColorTableKey;
					formController.AddColor(Key, newKey);
					RefreshColorGridDataSource();
				}
			}
		}
		void btnOK_Click(object sender, EventArgs e) {
			formController.Redo();
		}
		void btnApply_Click(object sender, EventArgs e) {
			formController.Redo();
			RefreshColorGridDataSource();
		}
		void DoShowMenu(GridHitInfo hi) {
			GridViewMenu menu = null;
			if(hi.HitTest == GridHitTest.RowCell) {
				ColorTableServerKey tableKey = (ColorTableServerKey)hi.View.GetRowCellValue(hi.RowHandle, hi.View.Columns[0]);
				if(formController.IsColorCustom(Key, tableKey)) {
					menu = new GridViewResetColorMenu(this, gridView1, formController.IsColorInCache(Key, tableKey) 
						? DashboardWinLocalizer.GetString(DashboardWinStringId.MenuResetColor)
						: DashboardWinLocalizer.GetString(DashboardWinStringId.MenuRemoveColor));
				}
				else {
					menu = new GridViewRetainColorMenu(this, gridView1);
				}
				menu.Init(hi);
				menu.Show(hi.HitPoint);
			}
		}
		void OnGridCellValueChanged(object sender, CellValueChangedEventArgs e) {
			DesignerColor color = e.Value as DesignerColor;
			if(color != null)
				SaveColor(color);
		}
		void OnSelectedColorSchemeChanged(object sender, EventArgs e) {
			UpdateValueFormatters();
			UpdateDeleteButtonState();
			BindColorGrid();
		}
		void OnGridCustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column != gridView1.Columns[0]) return;
			e.DisplayText = ColorTableKeyDisplayTextProvider.GetDisplayText((ColorTableServerKey)e.Value, formatters, Key, formController.DataInfoProvider);
		}
		void UpdateValueFormatters() {
			formatters.Clear();
			foreach (DimensionDefinition definition in Key.DimensionDefinitions) {
				ValueFormatViewModel formatViewModel = null;
				DataSourceInfo dataInfo = formController.DataInfoProvider.GetDataSourceInfo(Key.DataSourceName, Key.DataMember);
				if (dataInfo != null) {
					DataFieldType dataFieldType = dataInfo.GetFieldType(definition.DataMember);
					switch (dataFieldType) {
						case DataFieldType.DateTime:
							Dimension dimension = new Dimension(string.Empty, definition);
							formatViewModel = new ValueFormatViewModel(new DateTimeFormatViewModel(new DataItemDateTimeFormat(dimension)));
							break;
						case DataFieldType.Integer:
						case DataFieldType.Float:
						case DataFieldType.Double:
						case DataFieldType.Decimal:
							DataItemNumericFormat numericFormat = new DataItemNumericFormat();
							formatViewModel = new ValueFormatViewModel(numericFormat.CreateViewModel(false, dataFieldType == DataFieldType.Decimal ? NumericFormatType.Currency : NumericFormatType.Number, formController.CurrencyCultureName));
							break;
					}
				}
				formatters.Add(formatViewModel != null ? FormatterBase.CreateFormatter(formatViewModel) : null);
			}
		}
		void UpdateDeleteButtonState() {
			btnRemoveColorScheme.Enabled = Key != null ? !formController.IsTableInCache(Key) : false;
		}
		void UpdateNewValueButtonState() {
			btnNewValue.Enabled = Key != null;
		}
		void gcColors_MouseUp(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right)
				DoShowMenu(gridView1.CalcHitInfo(new Point(e.X, e.Y)));
		}
		void OnCustomDrawEmptyForeground(object sender, CustomDrawEventArgs e) {
			GridView view = sender as GridView;
			if(view.RowCount != 0 || (dataSources != null && dataSources.Count != 0) || 
				(formController != null && formController.ColorSchemeKeys.Count() != 0)) return;
			StringFormat drawFormat = new StringFormat();
			drawFormat.Alignment = drawFormat.LineAlignment = StringAlignment.Center;
			e.Graphics.DrawString(DashboardWinLocalizer.GetString(DashboardWinStringId.ColorSchemeGridEmptyText), e.Appearance.Font, e.Appearance.GetForeBrush(e.Cache), new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), drawFormat);
		}
	}
	[UserRepositoryItem("RegisterDashboardColorEdit")]
	public class DashboardRepositoryItemColorEdit : RepositoryItemColorPickEdit {
		public const string DashboardRepositoryItemColorEditName = "DashboardColorEdit";
		static DashboardRepositoryItemColorEdit() { RegisterDashboardColorEdit(); }
		public static void RegisterDashboardColorEdit() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(DashboardRepositoryItemColorEditName,
			  typeof(DashboardColorEdit), typeof(DashboardRepositoryItemColorEdit),
			  typeof(ColorEditViewInfo), new DashboardColorEditPainter(), true, null, typeof(PopupEditAccessible)));
		}
		public override string EditorTypeName { get { return DashboardRepositoryItemColorEditName; } }
		public override bool ShowSystemColors { get { return false; } set { } }
		public override bool ShowWebSafeColors { get { return false; } set { } }
		public override bool ShowWebColors { get { return false; } set { } }
		public override bool ShowPopupShadow { get { return true; } set { } }
		public DashboardRepositoryItemColorEdit() {
			AllowHtmlDraw = DefaultBoolean.True;
		}
		protected override Color ConvertToColor(object editValue) {
			DesignerColor color = editValue as DesignerColor;
			if(color != null)
				return color.GetColor(new DashboardPalette());
			else
				return base.ConvertToColor(editValue);
		}
		protected override object ConvertToEditValue(object val) {
			if(val is DesignerColor)
				return val;
			if(val is Color) {
				return new DesignerColor(new UserColor((Color)val));
			}
			if(val is int) {
				return new DesignerColor(new PaletteColor((int)val));
			}
			return base.ConvertToEditValue(val);
		}
		protected override Color[,] CreateStandardColorsCore() {
			DashboardPalette palette = new DashboardPalette();
			int columnCount = Math.Min(palette.ColorsCount, 5);
			int rowCount = palette.ColorsCount / columnCount;
			Color[,] res = new Color[rowCount, columnCount];
			for(int i = 0; i < palette.ColorsCount; i++) {
				int rowIndex = i / columnCount;
				res[rowIndex, i - rowIndex * columnCount] = palette.GetColor(i);
			}
			return res;
		}
	}
	[DXToolboxItem(false)]
	public class DashboardColorEdit : ColorPickEdit {
		static DashboardColorEdit() { DashboardRepositoryItemColorEdit.RegisterDashboardColorEdit(); }
		public override string EditorTypeName { get { return DashboardRepositoryItemColorEdit.DashboardRepositoryItemColorEditName; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new DashboardRepositoryItemColorEdit Properties { get { return base.Properties as DashboardRepositoryItemColorEdit; } }
		public DashboardColorEdit() {
		}
		protected override PopupColorPickEditForm CreatePopupFormInternal() {
			return new DashboardColorPickEditForm(this);
		}
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			return e.Value;
		}
	}
	public class DashboardColorPickEditForm : PopupColorPickEditForm {
		public DashboardColorPickEditForm(DashboardColorEdit ownerEdit) : base(ownerEdit) { }
		protected override PopupColorBuilder CreatePopupColorEditBuilder() {
			return new DashboardPopupColorBuilder(this);
		}
		new protected DashboardPopupColorBuilder PopupColorBuilder { get { return base.PopupColorBuilder as DashboardPopupColorBuilder; } }
	}
	public class DashboardPopupColorBuilder : PopupColorBuilderEx {
		object resultValue;
		public DashboardPopupColorBuilder(DashboardColorPickEditForm form)
			: base(form) {
		}
		protected override void OnSelectedColorChanged(object sender, InnerColorPickControlSelectedColorChangedEventArgs e) {
			this.resultValue = e.NewColor;
			base.OnSelectedColorChanged(sender, e);
		}
		protected override InnerColorPickControl CreateCustomTabInnerControlInstance() {
			return new DashboardInnerColorPickControl();
		}
		public override void OnShowPopup() {
			base.OnShowPopup();
			resultValue = ResultColor;
		}
		public override object ResultValue { get { return resultValue; } }
	}
	public class DashboardInnerColorPickControl : InnerColorPickControl {
		public DashboardInnerColorPickControl() {
			ShowAutomaticButton = ShowThemePalette = false;
			GroupPadding = new Padding(4, 6, 4, 6);
			FirstRowGap = 12;
			StandardGroupCaption = DashboardWinLocalizer.GetString(DashboardWinStringId.ColorPickerPaletteColorsSectionCaption);
		}
	}
	public class DashboardColorEditPainter : ColorEditPainter {
		protected override void DrawColorText(ControlGraphicsInfoArgs info) {
			ColorEditViewInfo vi = info.ViewInfo as ColorEditViewInfo;
			if(vi.ColorTextRect.IsEmpty) return;
			DrawString(info, vi.ColorTextRect, GetDisplayText(vi), vi.PaintAppearance);
		}
		public string GetDisplayText(ColorEditViewInfo viewInfo) {
			DesignerColor color = viewInfo.EditValue as DesignerColor;
			if(color != null)
				return color.GetDisplayText();
			else
				return string.Empty;
		}
	}
	public class DashboardColorIndexChangedEventArgs : EventArgs {
		public DashboardColorIndexChangedEventArgs(int index) {
			this.Index = index;
		}
		public int Index { get; private set; }
	}
	public class ColorSchemeFormGridMenu : GridViewMenu {
		EditColorSchemeForm form;
		protected EditColorSchemeForm Form { get { return form; } }
		public ColorSchemeFormGridMenu(EditColorSchemeForm form, GridView view) : base(view) {
			this.form = form;
		}
	}
	public class GridViewResetColorMenu : ColorSchemeFormGridMenu {
		const string ResetMenuTag = "Reset";
		string resetItemCaption;
		public GridViewResetColorMenu(EditColorSchemeForm form, GridView view, string resetItemCaption) : base(form, view) {
			this.resetItemCaption = resetItemCaption;
		}
		protected override void CreateItems() {
			Items.Clear();
			Items.Add(CreateMenuItem(resetItemCaption, null, ResetMenuTag, true));
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			if(RaiseClickEvent(sender, null)) return;
			DXMenuItem item = sender as DXMenuItem;
			if(item.Tag == null) return;
			else if(item.Tag.ToString() == ResetMenuTag) {
				Form.ClearColor();
			}
		}
	}
	public class GridViewRetainColorMenu : ColorSchemeFormGridMenu {
		const string RetainMenuTag = "Retain";
		public GridViewRetainColorMenu(EditColorSchemeForm form, GridView view) : base(form, view) { }
		protected override void CreateItems() {
			Items.Clear();
			Items.Add(CreateMenuItem(DashboardWinLocalizer.GetString(DashboardWinStringId.MenuRetainColor), null, RetainMenuTag, true));
		}
		protected override void OnMenuItemClick(object sender, EventArgs e) {
			if(RaiseClickEvent(sender, null)) return;
			DXMenuItem item = sender as DXMenuItem;
			if(item.Tag == null) return;
			if(item.Tag.ToString() == RetainMenuTag) {
				Form.RetainColor();
			}
		}
	}
	public class ColorSchemeDataSourceRow {
		public ColorRepositoryKey Key { get; set; }
		public string DisplayText { get; set; }
	}
}
