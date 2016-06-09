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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;	   
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Helpers;
using DevExpress.Skins;				   
using DevExpress.Utils.Controls;
using DevExpress.Utils.Menu;
using DevExpress.Data;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
namespace DevExpress.XtraEditors.Frames {			
	[ToolboxItem(false)]  
	public class DataSourceStructViewInfo : XtraScrollableControl, ISkinProvider {
		protected List<RowViewInfo> rowsViewInfoCore;
		protected ObservableCollection<IFormatRule> formatRulesCore;
		protected List<IFormatRule> hiddenFormatRuleCore;
		Dictionary<string, string> columnsSource;
		string defaultColumnName, selectionColumnName;
		int selectedRowIndexCore = -1;							   
		SizeF scaleFactor = new SizeF(1, 1);
		VScrollBar scroll;
		public DataSourceStructViewInfo() {
			DoubleBuffered = true;
			CreateScrollBar();
			scroll.ValueChanged += VScrollBar_ValueChanged;
			formatRulesCore = new ObservableCollection<IFormatRule>();
			columnsSource = new Dictionary<string, string>();
			formatRulesCore.CollectionChanged += formatRulesCore_CollectionChanged;
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
			hiddenFormatRuleCore = new List<IFormatRule>();
		}															   
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			CellPainter.SelectedBackColor = Color.Empty;		   
		}
		void CreateScrollBar() {
			scroll = new XtraEditors.VScrollBar();
			Controls.Add(scroll);
			scroll.LargeChange = 2;
			scroll.SmallChange = 1;
		}
		void VScrollBar_ValueChanged(object sender, EventArgs e) {
			UpdateRows();   
		}
		void formatRulesCore_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			UpdateRows();			
		}
		public IList<IFormatRule> FormatRules { get { return formatRulesCore; } }
		[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IList<IFormatRule> HiddenFormatRule { get { return hiddenFormatRuleCore; } }
		public Dictionary<string, string> ColumnsSource { get { return columnsSource; } }
		public int SelectedRowIndex { get { return selectedRowIndexCore; } }
		public string DefaultColumnName { get { return defaultColumnName; } }
		internal IDXMenuManager MenuManager { get; set; }
		public virtual void SetRules(IFormatRuleCollection ruleCollection) {
			for(int i = 0; i < ruleCollection.Count; i++) {
				FormatRuleBase frb = ruleCollection.GetRule(i);
				FormatConditionRuleBase fcrb = frb.RuleCast<FormatConditionRuleBase>();
				string ruleName = string.Empty;
				var fr = new FormatRule() {
					ColumnFieldName = frb.ColumnFieldName,
					RuleBase = fcrb,
					RuleType = GetRuleType(fcrb, out ruleName),
					ApplyToRow = GetApplyToRow(ruleCollection, i),
					Enabled = frb.Enabled
				};
				fr.RuleName = ruleName;
				if(!columnsSource.ContainsKey(frb.ColumnFieldName)) hiddenFormatRuleCore.Add(fr);
				else formatRulesCore.Add(fr);
			}														   
		}
		bool GetApplyToRow(IFormatRuleCollection ruleCollection, int i) {
			var tcolumn = ((System.Collections.IList)ruleCollection)[i];
			var pi = tcolumn.GetType().InvokeMember("ApplyToRow", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public,
					null, tcolumn, null);
			return (bool)pi;
		}
		protected RuleType GetRuleType(FormatConditionRuleBase fcrb, out string ruleName) {
			RuleType ruleType = RuleType.ColorScale2;
			ruleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScale);
			if(fcrb is FormatConditionRule2ColorScale) {
				ruleType = RuleType.ColorScale2;
				ruleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScale);
			}
			if(fcrb is FormatConditionRule3ColorScale) {
				ruleType = RuleType.ColorScale3;
				ruleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleColorScale);
			}
			if(fcrb is FormatConditionRuleDataBar) {
				ruleType = RuleType.DataBar;
				ruleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleDataBar);
			}
			if(fcrb is FormatConditionRuleIconSet) {
				ruleType = RuleType.IconSet;
				ruleName = Localizer.Active.GetLocalizedString(StringId.ManageRuleIconSet); ;
			}
			if(fcrb is FormatConditionRuleValue) {
				ruleType = RuleType.CellValue;
				ruleName = string.Format(Localizer.Active.GetLocalizedString(StringId.ManageRuleThatContainCellValue)); 
			}
			if(fcrb is FormatConditionRuleDateOccuring) {
				ruleType = RuleType.DatesOccurring;
				ruleName = ((FormatConditionRuleDateOccuring)fcrb).DateType.ToString(); 
			}
			if(fcrb is FormatConditionRuleAboveBelowAverage) {
				ruleType = RuleType.Average;
				ruleName = GetAverageType((FormatConditionRuleAboveBelowAverage)fcrb);
			}
			if(fcrb is FormatConditionRuleTopBottom) {
				ruleType = RuleType.RankedValues;
				var ranked = (FormatConditionRuleTopBottom)fcrb;
				string percent = (ranked.RankType == FormatConditionValueType.Percent) ? " %" : string.Empty;
				ruleName = string.Format("{0} {1}{2}", (ranked.TopBottom == FormatConditionTopBottomType.Top) ?
						   Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesTop) :
						   Localizer.Active.GetLocalizedString(StringId.ManageRuleRankedValuesBottom), ranked.Rank, percent);
			}
			if(fcrb is FormatConditionRuleUniqueDuplicate) {
				ruleType = RuleType.UniqueOrDuplicate;
				ruleName = ((FormatConditionRuleUniqueDuplicate)fcrb).FormatType == FormatConditionUniqueDuplicateType.Unique ?
							Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateUnique) :
							Localizer.Active.GetLocalizedString(StringId.ManageRuleUniqueOrDuplicateDuplicate);
			}
			if(fcrb is FormatConditionRuleExpression) {
				ruleType = RuleType.Formula;
				ruleName = string.Format("{0}: {1}", Localizer.Active.GetLocalizedString(StringId.ManageRuleFormula), ((FormatConditionRuleExpression)fcrb).Expression);
			}
			return ruleType;
		}
		string GetAverageType(FormatConditionRuleAboveBelowAverage average) {
			string averageType = string.Empty;
			switch(average.AverageType) {
				case FormatConditionAboveBelowType.Above:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleAboveAverage); break;
				case FormatConditionAboveBelowType.Below:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleBelowAverage); break;
				case FormatConditionAboveBelowType.EqualOrAbove:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleEqualOrAboveAverage); break;
				case FormatConditionAboveBelowType.EqualOrBelow:
					averageType = Localizer.Active.GetLocalizedString(StringId.ManageRuleEqualOrBelowAverage); break;
			}
			return averageType;
		}   
		public virtual void SetColumns(List<ColumnNameInfo> columnNames, string defaultColumnName) {
			foreach(var column in columnNames)
				if(column.Visible)
					AdditionColumnsSource(column);
			this.defaultColumnName = defaultColumnName;
		}
		void AdditionColumnsSource(ColumnNameInfo column) {
			try {
				columnsSource.Add(column.Key, column.Value);
			} catch(ArgumentException) {
				throw new ColumnFieldNameException(string.Format("The FieldName property value for the {0} column is not set or not unique.", column.Name));
			}
		}
		public SizeF ScaleFactor { get { return scaleFactor; } set { scaleFactor = value; } }
		protected void UpdateRows() {
			rowsViewInfoCore = new List<RowViewInfo>();
			AddHeaderToViewInfo();
			if(formatRulesCore.Count == 0) { Update(); return; }
			int index = 0;
			foreach(var rule in formatRulesCore) AddRuleToViewInfo(rule, index++);
			Update();
		}
		public new void Update() {
			CalculateLayout();
			Invalidate();
		}
		void AddHeaderToViewInfo() {
			rowsViewInfoCore.Add(CreateHeaderViewInfo());
		}
		void AddRuleToViewInfo(IFormatRule rule, int index) {
			RowViewInfo viewInfo = CreateRowViewInfo(rule, index);
			if(index == SelectedRowIndex) viewInfo.State = ObjectState.Selected;
			else viewInfo.State = ObjectState.Normal;
			rowsViewInfoCore.Add(viewInfo);
		}
		protected virtual RowViewInfo CreateHeaderViewInfo() {
			RowViewInfo viewInfo = new RowViewInfo(-1, ScaleFactor, 4);
			viewInfo.Cells.Add(new HeaderViewInfo(null, Localizer.Active.GetLocalizedString(StringId.ManageRuleGridCaptionRule)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, Localizer.Active.GetLocalizedString(StringId.ManageRuleGridCaptionFormat)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, Localizer.Active.GetLocalizedString(StringId.ManageRuleGridCaptionApplyToTheRow)));
			viewInfo.Cells.Add(new HeaderViewInfo(null, Localizer.Active.GetLocalizedString(StringId.ManageRuleGridCaptionColumn)));
			return viewInfo;
		}
		protected virtual RowViewInfo CreateRowViewInfo(IFormatRule rule, int index) {
			RowViewInfo viewInfo = new RowViewInfo(index, ScaleFactor, 4) { Rule = rule as FormatRule };
			viewInfo.Cells.Add(new RuleLabelViewInfo(rule.RuleBase) { NameRule = rule.RuleName });
			viewInfo.Cells.Add(new RulePreviewViewInfo(rule.RuleBase));
			viewInfo.Cells.Add(new RuleCheckBoxViewInfo(rule.RuleBase) { ApplyToRow = ((FormatRule)rule).ApplyToRow });
			viewInfo.Cells.Add(CreateComboBoxViewInfo(rule));
			return viewInfo;
		}
		RuleComboBoxViewInfo CreateComboBoxViewInfo(IFormatRule rule) {
			var ruleComboBoxViewInfo = new RuleComboBoxViewInfo(rule.RuleBase) { FieldName = rule.ColumnFieldName };
			string caption;
			ruleComboBoxViewInfo.Caption = ColumnsSource.TryGetValue(rule.ColumnFieldName, out caption) ? caption : string.Empty;
			return ruleComboBoxViewInfo;
		}
		#region override event
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			ProcessEvent(EventType.MouseDown, e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			ProcessEvent(EventType.MouseUp, e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ProcessEvent(EventType.MouseMove, e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			ProcessEvent(EventType.MouseLeave, e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			Draw(e);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			UpdateRows();
		}
		protected override void OnMouseWheelCore(MouseEventArgs ev) {
			base.OnMouseWheelCore(ev);
			int changeDelay = ev.Delta > 0 ? -1 : 1; 
			if(scroll.Visible && scroll.Minimum <= scroll.Value + changeDelay && scroll.Value + changeDelay < scroll.Maximum) scroll.Value += changeDelay;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if((keyData == Keys.Up || keyData == Keys.Down)) {
				int changeDelay = keyData == Keys.Up ? -1 : 1;
				if(0 <= selectedRowIndexCore + changeDelay && selectedRowIndexCore + changeDelay < rowsViewInfoCore.Count - 1) {
					selectedRowIndexCore += changeDelay;
					UpdateRows();
					if(!rowsViewInfoCore[selectedRowIndexCore + 1].RowVisible) {
						if(scroll.Visible && scroll.Minimum <= scroll.Value + changeDelay && scroll.Value + changeDelay < scroll.Maximum) scroll.Value += changeDelay;
					}
					return true;
				}
			}
			return base.ProcessDialogKey(keyData);
		}
		#endregion
		#region Row change
		protected void SwapItems(int index1, int index2) { 
			int frc = FormatRules.Count;
			if(index1 < 0 || index2 < 0 || index1 == index2 || index1 >= frc || index2 >= frc) return;
			int minIndex = Math.Min(index1, index2);
			var temp = FormatRules[minIndex];
			FormatRules.RemoveAt(minIndex);
			FormatRules.Insert(minIndex + 1, temp);
			selectedRowIndexCore = index2;
			UpdateRows();
		}
		public void RowUp() {
			SwapItems(SelectedRowIndex, SelectedRowIndex - 1);
		}
		public void RowDown() {
			SwapItems(SelectedRowIndex, SelectedRowIndex + 1);
		}
		public void RowDelete() {
			if(SelectedRowIndex < 0 || SelectedRowIndex >= FormatRules.Count) return;
			FormatRules.RemoveAt(SelectedRowIndex);
			if(FormatRules.Count == 0) selectedRowIndexCore = -1;
			UpdateRows();
		}
		#endregion
		public CellHitInfo CalcHitInfo(Point p) {
			CellHitInfo result = new CellHitInfo() { HitType = CellHitInfoType.Nothing };
			ForeachCell((rvi, cell) => {
				if(cell.IsCellArea(p)) {
					if(cell is RuleComboBoxViewInfo) result.HitType = CellHitInfoType.Combo;
					if(cell is RuleCheckBoxViewInfo) result.HitType = CellHitInfoType.Check;
					result.Cell = cell;
					result.Row = rvi;
				}
			});
			return result;
		}
		protected void ForeachCell(Action<RowViewInfo, CellViewInfo> method) {
			if(rowsViewInfoCore == null) return;
			foreach(RowViewInfo rvi in rowsViewInfoCore)
				foreach(CellViewInfo cvi in rvi.Cells)
					method(rvi, cvi);
		}
		public void ProcessEvent(EventType eventType, EventArgs ea) {
			MouseEventArgs mea = ea as MouseEventArgs;
			if(eventType == EventType.MouseDown) {
				Select();
				Focus();
			}
			CellHitInfo hitInfo = null;
			if(mea != null) hitInfo = CalcHitInfo(mea.Location);
			if(hitInfo == null) return;
			if(hitInfo.Row == null) return;
			if(eventType == EventType.MouseDown) {
				selectedRowIndexCore = hitInfo.Row.Index;
				UpdateRows();
				ProcessMouseDown(hitInfo);
			}
		}
		void ProcessMouseDown(CellHitInfo hitInfo) {
			if(hitInfo == null) return;
			if(hitInfo.HitType == CellHitInfoType.Combo)
				ShowAddControlMenu(hitInfo);
			if(hitInfo.HitType == CellHitInfoType.Check) {
				var vi = hitInfo.Cell as RuleCheckBoxViewInfo;
				if(vi == null) return;
				((FormatRule)hitInfo.Row.Rule).ApplyToRow = !((FormatRule)hitInfo.Row.Rule).ApplyToRow;
				UpdateRows();
			}
		}
		#region Menu
		void ShowAddControlMenu(CellHitInfo hitInfo) {
			var vi = hitInfo.Cell as RuleComboBoxViewInfo;
			if(vi == null) return;
			DXPopupMenu controlsMenu = CreateMenu(hitInfo.Row.Rule, vi);
			Point p = new Point(vi.CellRect.X, vi.CellRect.Bottom);
			MenuManagerHelper.ShowMenu(controlsMenu, LookAndFeel.ActiveLookAndFeel, MenuManager, this, p);
		}
		protected virtual DXPopupMenu CreateMenu(IFormatRule rule, RuleComboBoxViewInfo vi) {
			DXPopupMenu columnsMenu = new DXPopupMenu();
			if(columnsSource == null) return columnsMenu;
			foreach(var item in columnsSource) {
				DXMenuItem menuItem = new DXMenuItem(item.Value);
				menuItem.Click += (s, e) => { rule.ColumnFieldName = item.Key; UpdateRows(); };
				columnsMenu.Items.Add(menuItem);
			}
			return columnsMenu;
		}
		#endregion
		int MaxVisibleRow { get { return (Size.Height - RowViewInfo.HeightCell) / RowViewInfo.HeightCell; } }
		public void CalculateLayout() {
			if(rowsViewInfoCore == null) return;
			bool isScrollBarVisible = VisibleRows();
			int index = -1;
			foreach(RowViewInfo rvi in rowsViewInfoCore) {
				if(rvi.RowVisible) index++;
				rvi.CalculateLayout(ClientSize, index, isScrollBarVisible, scroll.Width, ScaleFactor);
			}
		}
		bool VisibleRows() {
			rowsViewInfoCore[0].RowVisible = true;
			int countVisibleRow = VisibleRowsFilterColumn();
			int currentNotVisibleRow = (countVisibleRow + 1) - MaxVisibleRow;
			scroll.Maximum = currentNotVisibleRow <= 1 ? 0 : currentNotVisibleRow;
			if(scroll.Value >= scroll.Maximum && scroll.Maximum != 0)
				scroll.Value = scroll.Maximum - 1;
			bool isScrollBarVisible = currentNotVisibleRow > 1;
			SetVScrollBarVisible(isScrollBarVisible);
			if(isScrollBarVisible) VisibleRowsFilterScrollBar();
			return isScrollBarVisible;
		}
		void VisibleRowsFilterScrollBar() {
			int visibleIndex = 0;
			for(int i = 1; i < rowsViewInfoCore.Count; i++)
				if(rowsViewInfoCore[i].RowVisible) {
					rowsViewInfoCore[i].RowVisible = visibleIndex >= scroll.Value && visibleIndex <= scroll.Value + MaxVisibleRow;
					visibleIndex++;
				}
		}
		int VisibleRowsFilterColumn() {
			int countVisibleRow = 0;
			for(int i = 1; i < rowsViewInfoCore.Count; i++) {
				var row = rowsViewInfoCore[i];
				row.RowVisible = IsVisibleRowByFilterColumn(row);
				if(row.RowVisible) countVisibleRow++;
			}
			return countVisibleRow;
		}
		bool IsVisibleRowByFilterColumn(RowViewInfo row) {
			return IsFilterByAllColumns() || IsFilterBySelectionColumn(row);
		}
		bool IsFilterByAllColumns() {
			return selectionColumnName == Localizer.Active.GetLocalizedString(StringId.ManageRuleAllColumns);
		}
		bool IsFilterBySelectionColumn(RowViewInfo row) {
			RuleComboBoxViewInfo ruleComboBox = row.Cells[3] as RuleComboBoxViewInfo;
			return ruleComboBox != null && selectionColumnName == ruleComboBox.FieldName;
		}
		void SetVScrollBarVisible(bool visible) {
			scroll.Visible = visible;
			scroll.Enabled = visible;
			if(!visible) return;
			Point location = new Point(ClientRectangle.Right - (scroll.GetDefaultVerticalScrollBarWidth() + 2), ClientRectangle.Top + RowViewInfo.HeightCell + 1);
			Size size = new Size((scroll.GetDefaultVerticalScrollBarWidth() + 1), ClientSize.Height - (RowViewInfo.HeightCell + 2));
			scroll.Bounds = new Rectangle(location, size);
		}
		void DrawBorder(GraphicsCache cache) {
			SkinGridBorderPainter sgbp = new SkinGridBorderPainter(this);
			ObjectInfoArgs oia = new ObjectInfoArgs(cache, Bounds, ObjectState.Normal);
			oia.OffsetContent(-Location.X, -Location.Y);
			sgbp.DrawObject(oia);
		}
		public void Draw(PaintEventArgs e) {
			lock(this) {
				GraphicsCache cache = new GraphicsCache(new DXPaintEventArgs(e, Handle));
				cache.Cache.CheckCache();
				try {								 
					DevExpress.Utils.Paint.XPaint.Graphics.BeginPaint(e.Graphics);
					ForeachCell((rvi, cvi) => {
						cvi.Cache = cache;
						if(rvi.RowVisible) CellPainter.Draw(cvi, this);
						cvi.Cache = null;
					});
					DrawBorder(cache);
				} finally {
					DevExpress.Utils.Paint.XPaint.Graphics.EndPaint(e.Graphics);
				}
			}
		}
		string ISkinProvider.SkinName {
			get {
				return this.LookAndFeel.ActiveLookAndFeel.SkinName;
			}
		}
		public void FilterVisibleColumn(string selectedColumn) {
			if(selectedColumn == null) selectedColumn = string.Empty;
			selectionColumnName = selectedColumn;
			UpdateRows();
		}
	}
	#region RowViewInfo
	public class RowViewInfo : ObjectInfoArgs {
		int countCells;
		int indexCore;	
		List<CellViewInfo> cells;
		static SizeF scale;
		public List<CellViewInfo> Cells { get { return cells; } }
		public int Index { get { return indexCore; } }
		public IFormatRule Rule { get; set; }
		public bool RowVisible { get; set; }
		public static int HeightCell {
			get {
				int height;
				if(WindowsFormsSettings.TouchUIMode == LookAndFeel.TouchUIMode.True) height = 35;
				else height = 25;
				return (int)(height * scale.Width);
			}
		}
		public RowViewInfo(int index, SizeF scale, int countCells) {
			this.countCells = countCells;
			cells = new List<CellViewInfo>();
			indexCore = index;
			RowViewInfo.scale = scale;
		}
		public void CalculateLayout(Size size, int index, bool isScrollBarVisible, int scrollOffset, SizeF scale) {
			if(cells == null) return;
			if(!RowVisible) return;
			Point leftTop = Point.Empty;
			int width = size.Width / countCells;
			int remainderWidth = size.Width % countCells;
			int lastIndexElement = cells.Count - 1;
			foreach(CellViewInfo cell in cells) {
				if(cells.IndexOf(cell) == lastIndexElement) width += remainderWidth;
				cell.CellRect = new Rectangle(new Point(leftTop.X, leftTop.Y + index * HeightCell), new Size(width, HeightCell));
				leftTop.Offset(width, 0);
			}
			if(isScrollBarVisible) SetOffset(0, scrollOffset + 2, lastIndexElement);
		}
		public override ObjectState State {
			get {
				return base.State;
			}
			set {
				base.State = value;
				foreach(CellViewInfo cell in cells) cell.State = State;
			}
		}
		internal void SetOffset(int offsetHeight, int scrollOffset, int lastIndexElement) {
			if(cells == null) return;
			int index = 0;
			foreach(CellViewInfo cell in cells) {
				if(cell is HeaderViewInfo) continue;
				cell.CellRect = new Rectangle(cell.CellRect.X, cell.CellRect.Y + offsetHeight, cell.CellRect.Width + (index == lastIndexElement ? -scrollOffset : 0), cell.CellRect.Height);
				index++;
			}
		}
	}
	#endregion
	#region CellPainter
	public static class CellPainter {
		static RepositoryItemComboBox repositoryItem = new RepositoryItemComboBox();
		static ButtonEditViewInfo viewInfo = (ButtonEditViewInfo)repositoryItem.CreateViewInfo();
		static BaseEditPainter painter = repositoryItem.CreatePainter();
		static RepositoryItemCheckEdit repositoryItemCheck = new RepositoryItemCheckEdit();
		static CheckEditViewInfo viewInfoCheck = (CheckEditViewInfo)repositoryItemCheck.CreateViewInfo();
		static CheckEditPainter painterCheck = (CheckEditPainter)repositoryItemCheck.CreatePainter();
		public static void Draw(CellViewInfo cvi, DataSourceStructViewInfo dssvi) {
			if(cvi is HeaderViewInfo) {
				DrawHeader(cvi as HeaderViewInfo, dssvi);
			} else {
				DrawState(cvi, dssvi);
				if(cvi is RuleLabelViewInfo)
					DrawRuleLabel(cvi as RuleLabelViewInfo, dssvi);
				else if(cvi is RulePreviewViewInfo)
					DrawRulePreview(cvi as RulePreviewViewInfo);
				else if(cvi is RuleCheckBoxViewInfo)
					DrawRuleCheckBox(cvi as RuleCheckBoxViewInfo);
				else if(cvi is RuleComboBoxViewInfo)
					DrawRuleComboBox(cvi as RuleComboBoxViewInfo, dssvi);
			}
			DrawCellRect(cvi, dssvi);
		}
		internal static Color SelectedBackColor = Color.Empty;
		static Color GetSelectedBackColor(CellViewInfo args, DataSourceStructViewInfo dssvi) {
			if(SelectedBackColor == Color.Empty)
				SelectedBackColor = CommonSkins.GetSkin(dssvi)[CommonSkins.SkinSelection].Color.BackColor;
			return SelectedBackColor;
		}
		static void DrawState(CellViewInfo args, DataSourceStructViewInfo dssvi) {
			switch(args.State) {
				case ObjectState.Selected:
					args.Cache.FillRectangle(GetSelectedBackColor(args, dssvi), args.CellRect);
					break;
			}
		}
		public static void DrawCellRect(CellViewInfo cvi, DataSourceStructViewInfo dssvi) {
			SkinGridBorderPainter sgbp = new SkinGridBorderPainter(dssvi);
			Rectangle left, right, top, bottm;
			left = right = top = bottm = cvi.CellRect;
			left.Width = 1;
			right.X = right.Right;
			right.Width = 1;
			top.Height = 1;
			bottm.Y = bottm.Bottom;
			bottm.Height = 1;
			List<Rectangle> list = new List<Rectangle>() { left, right, top, bottm };
			foreach(Rectangle r in list) {
				ObjectInfoArgs oia = new ObjectInfoArgs(cvi.Cache, r, cvi.State);
				sgbp.DrawObject(oia);
			}
		}
		static void DrawHeader(HeaderViewInfo hvi, DataSourceStructViewInfo dssvi) {
			hvi.Appearance.ForeColor = CommonSkins.GetSkin(dssvi).GetSystemColor(SystemColors.ControlText);
			HeadArgs ha = new HeadArgs(hvi) {
				Cache = hvi.Cache,
				Caption = hvi.Header,
				CaptionRect = new Rectangle(Point.Empty, hvi.InnerCellRect.Size),
				Bounds = new Rectangle(hvi.CellRect.Location, new Size(hvi.CellRect.Width + 1, hvi.CellRect.Height + 1))
			};
			SkinHeaderObjectPainter shop = new SkinHeaderObjectPainter(dssvi);
			shop.DrawObject(ha);
		}
		static void DrawRuleLabel(RuleLabelViewInfo rlvi, DataSourceStructViewInfo dssvi) {
			rlvi.Appearance.ForeColor = CommonSkins.GetSkin(dssvi).GetSystemColor(SystemColors.ControlText);
			viewInfo.Appearance.DrawString(rlvi.Cache, rlvi.NameRule, rlvi.InnerCellRect, rlvi.Appearance.GetForeBrush(rlvi.Cache));
		}
		static void DrawRulePreview(RulePreviewViewInfo rpvi) {
			((IFormatConditionDrawPreview)rpvi.Owner).Draw(rpvi.DrawPreviewArgs);
		}
		static void DrawRuleCheckBox(RuleCheckBoxViewInfo rcheckbvi) {
			viewInfoCheck = rcheckbvi.GetViewInfo(viewInfoCheck);
			painterCheck.Draw(new ControlGraphicsInfoArgs(viewInfoCheck, rcheckbvi.Cache, rcheckbvi.InnerCellRect));
		}
		static void DrawRuleComboBox(RuleComboBoxViewInfo rcbvi, DataSourceStructViewInfo dssvi) {
			rcbvi.Appearance.ForeColor = CommonSkins.GetSkin(dssvi).GetSystemColor(SystemColors.InfoText);
			if(rcbvi.State == ObjectState.Selected) {
				viewInfo = rcbvi.GetViewInfo(viewInfo);
				painter.Draw(new ControlGraphicsInfoArgs(viewInfo, rcbvi.Cache, rcbvi.InnerCellRect));
			} else {
				viewInfo.Appearance.DrawString(rcbvi.Cache, rcbvi.Caption, rcbvi.InnerCellRect, rcbvi.Appearance.GetForeBrush(rcbvi.Cache));
			}
		}
	}
	#endregion
	#region CellHitInfo
	public enum CellHitInfoType { Nothing, Combo, Check }
	public class CellHitInfo {
		public CellHitInfoType HitType { get; set; }
		public CellViewInfo Cell { get; set; }
		public RowViewInfo Row { get; set; }
	}
	#endregion
	public class HeadArgs : HeaderObjectInfoArgs {
		HeaderViewInfo hvi;
		public HeadArgs(FormatConditionRuleBase owner, string name) {
			hvi = new HeaderViewInfo(owner, name);
		}
		public HeadArgs(HeaderViewInfo hvi) {
			SetAppearance(hvi.Appearance);
			this.hvi = hvi;
		}
		public override string Caption {
			get {
				return hvi.Header;
			}
			set {
				hvi.Header = value;
			}
		}
	}
	#region CellViewInfo
	public class CellViewInfo : StyleObjectInfoArgs {
		int offset = 2;
		FormatConditionRuleBase ownerCore;
		Rectangle cellRect, innerCellRect;
		public FormatConditionRuleBase Owner { get { return ownerCore; } }
		public Rectangle CellRect {
			get { return cellRect; }
			set {
				cellRect = value;
				innerCellRect = GetInner(cellRect);
			}
		}
		public Rectangle InnerCellRect {
			get { return innerCellRect; }
		}
		public CellViewInfo(FormatConditionRuleBase owner) {
			ownerCore = owner;
		}
		protected virtual Rectangle GetInner(Rectangle rect) {
			Rectangle temp = rect;
			if(WindowsFormsSettings.TouchUIMode == LookAndFeel.TouchUIMode.True) offset = 5;
			temp.Inflate(-2 * offset, -offset);
			return temp;
		}
		public bool IsCellArea(Point p) {
			return cellRect.Contains(p);
		}
	}
	public class HeaderViewInfo : CellViewInfo {
		public string Header { get; set; }
		public HeaderViewInfo(FormatConditionRuleBase owner, string name)
			: base(owner) {
			Header = name;
		}
	}
	public class RuleLabelViewInfo : CellViewInfo {
		public string NameRule { get; set; }
		public RuleLabelViewInfo(FormatConditionRuleBase owner) : base(owner) { }
	}
	public class RulePreviewViewInfo : CellViewInfo {
		public FormatConditionDrawPreviewArgs DrawPreviewArgs {
			get {
				if(Owner is FormatConditionRuleAppearanceBase) {
					var appearance = Owner as FormatConditionRuleAppearanceBase;
					return new FormatConditionDrawPreviewArgs(Graphics, InnerCellRect, appearance.Appearance, "AaBbCcYyZz");
				} else
					return new FormatConditionDrawPreviewArgs(Graphics, InnerCellRect, AppearanceObject.EmptyAppearance, string.Empty);
			}
		} 
		public RulePreviewViewInfo(FormatConditionRuleBase owner) : base(owner) { }
	}
	public class RuleCheckBoxViewInfo : CellViewInfo {
		public bool? ApplyToRow { get; set; }
		public RuleCheckBoxViewInfo(FormatConditionRuleBase owner) : base(owner) { }
		internal CheckEditViewInfo GetViewInfo(CheckEditViewInfo vi) {
			vi.EditValue = ApplyToRow;
			vi.Bounds = InnerCellRect;
			vi.CalcViewInfo(Graphics);
			return vi;
		}
	}
	public class RuleComboBoxViewInfo : CellViewInfo {
		public string FieldName { get; set; }
		public string Caption { get; set; }
		public RuleComboBoxViewInfo(FormatConditionRuleBase owner) : base(owner) { }
		internal ButtonEditViewInfo GetViewInfo(ButtonEditViewInfo vi) {
			vi.Bounds = InnerCellRect;
			vi.EditValue = Caption;
			vi.PaintAppearance.ForeColor = Appearance.ForeColor;
			vi.CalcViewInfo(Graphics);
			return vi;
		}
		protected override Rectangle GetInner(Rectangle rect) {
			if(State != ObjectState.Selected) return base.GetInner(rect);
			Rectangle temp = rect;
			temp.Size = new Size(rect.Width + 1, rect.Height + 1);
			return temp;
		}
	}
	#endregion
	public class ColumnFieldNameException : Exception {
		public ColumnFieldNameException(string message) : base(message) { }
		public ColumnFieldNameException(string message, Exception inner) : base(message, inner) { }
		public override string StackTrace {
			get { return null; }
		}
	}
}
