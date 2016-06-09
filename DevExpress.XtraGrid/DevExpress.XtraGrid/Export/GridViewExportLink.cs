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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraExport;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Export {
	[Obsolete("Use the BaseView.ExportTo* methods")]
	public class GridViewExportLink : BaseExportLink {
		private Stack groupFooterStack;
		private GridColumnsInfo visibleColumnsInfo;
		private int rowWidth;
		private GridRowInfo rowInfo;
		private int currentRowHandle = InvalidRowHandle;
		protected int fCacheWidth;
		protected int fCacheHeight;
		protected bool fExpandAll = true;
		protected bool fExportDetails = true;
		protected const int InvalidRowHandle = DataController.InvalidRow;
		protected ArrayList fRows;
		protected bool fRowsMade;
		protected bool isMasterView;
		protected int fHeaderStyle;
		protected int fFooterStyle;
		protected int fGroupFooterStyle;
		protected int fGroupRowStyle;
		protected int fGroupLevelStyle;
		protected int fPreviewStyle;
		protected int fGroupButtonStyle;
		protected int fPreviewLineCount = 1;
		public GridViewExportLink(BaseView view, IExportProvider provider): 
			base(view, provider) {
			this.fRows = new ArrayList();
			groupFooterStack = new Stack();
		}
		protected override BaseAppearanceCollection CreateAppearanceCollectionInstance() {
			return new GridViewAppearances(View); 
		}
		public new GridViewAppearances ExportAppearance { 
			get { return base.ExportAppearance as GridViewAppearances; }
		}
		protected override void UpdateAppearanceScheme(BaseAppearanceCollection coll) {
			if(View.PaintStyle== null) return;
			GridViewAppearances res = coll as GridViewAppearances;
			if(View.PaintStyle.IsSkin) {
				res.Combine(res, View.BaseInfo.PaintStyles["Flat"].GetAppearanceDefaultInfo(View));
			}
		}
		bool ShowPreview(int rowHandle) {
			return ViewInfo.CalcRowPreviewHeight(rowHandle) != 0;
		}
		private void PushGroupFooter(int handle) {
			groupFooterStack.Push(handle);
		}
		private int PopGroupFooter() {
			if(groupFooterStack.Count > 0)
				return (int)groupFooterStack.Pop();
			else
				return InvalidRowHandle;
		}
		private void MakeRowList() {
			if(fRowsMade)
				return;
			fRows.Clear();
			VisibleIndexCollection indexes = new VisibleIndexCollection(View.DataController, View.DataController.GroupInfo);
			indexes.BuildVisibleIndexes(View.DataController.VisibleListSourceRowCount, true, this.fExpandAll);
			fRows.AddRange(indexes);
			fRowsMade = true;
		}
		private void MakeSelectedRowList() {
			if(fRowsMade)
				return;
			fRows.Clear();
			int[] list = View.DataController.Selection.GetNormalizedSelectedRows();
			if(View.DataController.IsGrouped) {
				VisibleIndexCollection indexes = new VisibleIndexCollection(View.DataController, View.DataController.GroupInfo);
				indexes.Clear();
				for(int n = 0; n < list.Length; n++) {
					GroupRowInfo group = View.DataController.GroupInfo.GetGroupRowInfoByControllerRowHandle(list[n]);
					if(group == null || group.ParentGroup != null) continue;
					indexes.BuildVisibleIndexes(group, this.fExpandAll);
				}
				fRows.AddRange(indexes);
			} else {
				fRows.AddRange(list);
			}
			fRowsMade = true;
		}
		private BaseView GetDetailView(int rowHandle, int relationIndex) {
			if(!View.CanExpandMasterRowEx(rowHandle, relationIndex))
				return null;
			BaseView defaultView;
			string levelName;
			BaseView result = View.CreateLevelDefault(rowHandle, relationIndex, out defaultView, out levelName);
			result.BeginUpdate();
			DevExpress.Data.Details.MasterRowInfo mi = new DevExpress.Data.Details.MasterRowInfo(View.DataController, rowHandle, null);
			DevExpress.Data.Details.DetailInfo di = mi.CreateDetail(View.DataController.GetDetailList(rowHandle, relationIndex), relationIndex);
			View.ApplyLevelDefaults(result, defaultView, di, levelName);
			result.ParentView = null;
			result.ParentView = View;
			return result;
		}
		protected GridViewInfo ViewInfo { get { return View.ViewInfo as GridViewInfo; } }
		private AppearanceObject GetRowStyle(int rowHandle) {
			return rowInfo.Appearance;
		}
		private void CalcVisibleColumnsInfo() {
			if(visibleColumnsInfo == null)
				visibleColumnsInfo = ((GridViewInfo)View.ViewInfo).CreateVisibleColumnsInfo(ref rowWidth);
		}
		private void CalcRowInfo(int rowHandle) {
			if(rowHandle != currentRowHandle) {
				CalcVisibleColumnsInfo();
				rowInfo = ViewInfo.CreateRowInfo(rowHandle, rowWidth, visibleColumnsInfo);
				rowInfo.RowState = ViewInfo.CalcRowState(rowInfo) & (~(GridRowCellState.Focused | GridRowCellState.Selected | GridRowCellState.FocusedCell));
				ViewInfo.UpdateRowAppearanceCore(rowInfo);
				currentRowHandle = rowHandle;
			}
		}
		protected void ModifyGroupSeparatorsStyle(int startCol, int row) {
			for(int i = startCol; i >= 0; i--)
				Provider.SetCellStyle(i, row, fGroupLevelStyle);
		}
		protected void ModifyMasterSeparatorStyle(int col, int row, ExportCacheCellStyle style) {
			if(col < 0 || row < 0) return;
			style.TopBorder.Width = 0;
			style.BottomBorder.Width = 0;
			style.LeftBorder.Width = 1;
			style.RightBorder.Width = 1;
			Provider.SetCellStyle(col, row, style);
		}
		protected int GetColumnWidth(int index) {
			return ((GridColumn)View.VisibleColumns[index]).VisibleWidth;
		}
		protected void LoadDetails(ref int row, int handle) {
			if(fExpandAll) {
				int detailCount = View.GetRelationCount(handle);
				for(int i = 0; i < detailCount; i++)
					LoadDetailContent(row + i, handle, i);
				row += detailCount;
			} else {
				LoadDetailContent(row, handle, -1);
				row++;
			}
		}
		protected void LoadDetailContent(int row, int handle, int relationIndex) {
			int level = View.GetRowLevel(handle);
			ModifyGroupSeparatorsStyle(level - 1, row);			
			ModifyMasterSeparatorStyle(level, row, GetDataRowStyle(handle));
			Provider.SetCellStyleAndUnion(level + 1, row, fCacheWidth - level - 1, 1, GetDataRowStyle(handle));
			BaseView detailView = null;
			bool disposeView = false;
			if(fExpandAll) {
				detailView = GetDetailView(handle, relationIndex);
				disposeView = true;
			}
			else
				detailView = View.GetVisibleDetailView(handle);
			if(detailView != null) {
				IExportProvider detailProvider = Provider.Clone("", null);
				BaseExportLink detailLink = detailView.CreateExportLink(detailProvider);
				if(detailLink != null) {
					detailLink.Copy(this);
					detailLink.ExportTo(false);
					((IExportInternalProvider)Provider).SetCacheToCell(
						level + 1, row, detailProvider as IExportInternalProvider);
				}
				if(disposeView) 
					detailView.Dispose();
			}
		}
		protected void LoadGroupFooters(ref int row, int count) {
			for(int i = 0; i < count; i++)
				LoadGroupFooterContent(ref row, PopGroupFooter());
		}
		protected virtual ExportCacheCellStyle GetHeaderStyle() {
			return GridStyleToExportStyle(ExportAppearance.HeaderPanel, null, null);
		}
		protected virtual ExportCacheCellStyle GetFooterStyle() {
			return GridStyleToExportStyle(ExportAppearance.FooterPanel, null, null);
		}
		protected virtual ExportCacheCellStyle GetGroupFooterStyle() {
			return GridStyleToExportStyle(ExportAppearance.GroupFooter, null, null);
		}
		protected virtual ExportCacheCellStyle GetGroupRowStyle() {
			return GridStyleToExportStyle(ExportAppearance.GroupRow, null, null);
		}
		protected virtual ExportCacheCellStyle GetGroupLevelStyle() {
			ExportCacheCellStyle result = GetGroupRowStyle();
			result.TopBorder.Width = 0;
			result.BottomBorder.Width = 0;
			return result;
		}
		protected virtual ExportCacheCellStyle GetPreviewStyle() {
			return GridStyleToExportStyle(ExportAppearance.Preview, null, null);
		}
		protected virtual ExportCacheCellStyle GetGroupButtonStyle() {
			return GridStyleToExportStyle(ExportAppearance.GroupButton, null, null);
		}
		protected virtual ExportCacheCellStyle GetDataRowStyle(int rowHandle) {
			if(currentRowHandle != rowHandle) CalcRowInfo(rowHandle);
			return GridStyleToExportStyle(GetRowStyle(rowHandle), 
				ExportAppearance.VertLine, 
				ExportAppearance.HorzLine);
		}
		protected virtual ExportCacheCellStyle GetDataRowCellStyle(int rowHandle, GridColumn column) {
			CalcRowInfo(rowHandle);
			GridCellInfo ci = ((GridViewInfo)View.ViewInfo).CreateCellInfo(rowInfo as GridDataRowInfo, visibleColumnsInfo[column]);
			ci.State = rowInfo.RowState;
			ViewInfo.UpdateCellAppearanceCore(ci);
			AppearanceObject style;
			style = View.GetRowCellStyle(ci.RowHandle, ci.ColumnInfo.Column, ci.State, ci.Appearance);
			ExportCacheCellStyle exportStyle = GridStyleToExportStyle(style, 
				ExportAppearance.VertLine, ExportAppearance.HorzLine);
			exportStyle.TextAlignment = HorzAlignmentToStringAlignment(style.HAlignment);
			return exportStyle;
		}
		protected virtual void RegisterStyles() {
			fHeaderStyle = Provider.RegisterStyle(GetHeaderStyle());
			fFooterStyle = Provider.RegisterStyle(GetFooterStyle());
			fGroupFooterStyle = Provider.RegisterStyle(GetGroupFooterStyle());
			fGroupRowStyle = Provider.RegisterStyle(GetGroupRowStyle());
			fGroupLevelStyle = Provider.RegisterStyle(GetGroupLevelStyle());
			fPreviewStyle = Provider.RegisterStyle(GetPreviewStyle());
			fGroupButtonStyle = Provider.RegisterStyle(GetGroupButtonStyle());
		}
		protected virtual void SetColumnsWidth() {
			int startCol = View.SortInfo.GroupCount;
			if(isMasterView && ExportDetails) 
				startCol++;
			for(int i = 0; i < View.VisibleColumns.Count; i++)
				Provider.SetColumnWidth(startCol + i, GetColumnWidth(i));
			if(View.VisibleColumns.Count == 0) {
				if(View.SortInfo.GroupCount > 0)
					Provider.SetColumnWidth(startCol, 600);
				else {
					if(fCacheWidth > 0)
						Provider.SetColumnWidth(0, 600);
				}
			}
		} 
		protected virtual int CalcCacheWidth() {			
			int result = View.VisibleColumns.Count + View.SortInfo.GroupCount;
			if(View.VisibleColumns.Count == 0)
				result++;
			if(View.VisibleColumns.Count > 0 || View.SortInfo.GroupCount > 0) {
				if(fExportAll) {
					if(IsMasterViewMode) {
						isMasterView = true;
						if(fExportDetails)
							result++;
					}
				}
			}
			return result;
		}
		protected virtual int CalcCacheHeight() {
			int result = 0;
			if(View.VisibleColumns.Count > 0 || View.SortInfo.GroupCount > 0) {
				if(fExportAll)
					MakeRowList();
				else
					MakeSelectedRowList();
				for(int i = 0; i < fRows.Count; i++) {
					result += GetRowHeight((int)fRows[i]);
					if(View.GroupFooterShowMode != GroupFooterShowMode.Hidden && View.VisibleColumns.Count > 0) {
						if(View.IsGroupRow((int)fRows[i]) && View.IsExistAnyRowFooterCell((int)fRows[i])) {
							switch(View.GroupFooterShowMode) {
								case GroupFooterShowMode.VisibleAlways:
									result += GetFooterHeight();
									break;
								case GroupFooterShowMode.VisibleIfExpanded:
									if(fExpandAll || (!fExpandAll && View.GetRowExpanded((int)fRows[i])))
										result += GetFooterHeight();
									break;
							}
						}
					}
					if(ShowPreview((int)fRows[i])) {
						if(!View.IsGroupRow((int)fRows[i]))
							result += this.fPreviewLineCount;
					} 
					if(View.IsMasterRow((int)fRows[i]) && isMasterView) {
						if(fExportDetails) {
							if(fExpandAll || View.GetMasterRowExpanded((int)fRows[i])) {
								if(fExpandAll)
									result += View.GetRelationCount((int)fRows[i]);
								else
									result++;
							}
						}
					}
				}
			}
			if(View.OptionsView.ShowColumnHeaders)
				result += GetHeaderHeight();
			if(View.OptionsView.ShowFooter && fExportAll)
				result += GetFooterHeight();
			return result;
		}
		protected virtual int LoadCache(int startRow) {			
			OnProgress(0, ExportPhase.Link);
			int row = startRow;
			if(View.OptionsView.ShowColumnHeaders)
				LoadHeaderContent(ref row);
			if(View.VisibleColumns.Count != 0 || View.SortInfo.GroupCount != 0) {				
				for(int i = 0; i < fRows.Count; i++) {
					if(View.IsGroupRow((int)fRows[i])) {
						if(View.VisibleColumns.Count > 0) {
							int level = View.GetRowLevel((int)fRows[i]);
							if(level < groupFooterStack.Count)
								LoadGroupFooters(ref row, groupFooterStack.Count - level);
						}
						LoadGroupRowContent(ref row, (int)fRows[i]);
						if(View.VisibleColumns.Count > 0) {
							switch(View.GroupFooterShowMode) {
								case GroupFooterShowMode.VisibleAlways:
									PushGroupFooter((int)fRows[i]);
									break;
								case GroupFooterShowMode.VisibleIfExpanded:
									if(fExpandAll || (!fExpandAll && View.GetRowExpanded((int)fRows[i])))
										PushGroupFooter((int)fRows[i]);
									break;
							};
						}
					}
					else {
						LoadRowContent(ref row, (int)fRows[i]);
						if(ShowPreview((int)fRows[i]))
							LoadPreviewContent(ref row, (int)fRows[i]);
						if(isMasterView)
							if(fExportDetails)
								if(fExpandAll || View.GetMasterRowExpanded((int)fRows[i])) 
									LoadDetails(ref row, (int)fRows[i]);
					}
					if(fRows.Count > 1)
						OnProgress(i * 100 / (fRows.Count - 1), ExportPhase.Link);
				}
				if(groupFooterStack.Count > 0 && View.VisibleColumns.Count > 0)
					LoadGroupFooters(ref row, groupFooterStack.Count);
			}			
			if(View.OptionsView.ShowFooter && fExportAll)
				LoadFooterContent(ref row);
			OnProgress(100, ExportPhase.Link);
			return row;
		}
		protected virtual void LoadHeaderContent(ref int row) {
			if(View.VisibleColumns.Count == 0) {
				if(fCacheWidth > 0) 	
					Provider.SetCellStyleAndUnion(0, row, fCacheWidth, 1, fHeaderStyle);
			} else {
				int startCol = 0;
				for(int i = 0; i < View.VisibleColumns.Count; i++) {
					Provider.SetCellString(startCol + i, row, View.GetVisibleColumn(i).GetTextCaption());
					Provider.SetCellStyle(startCol + i, row, fHeaderStyle);
					if(i == 0) {
						if(isMasterView && fExportDetails) {
							Provider.SetCellUnion(startCol + i, row, View.SortInfo.GroupCount + 2, 1);
							startCol += View.SortInfo.GroupCount + 1;
						} else {
							Provider.SetCellUnion(startCol + i, row, View.SortInfo.GroupCount + 1, 1);
							startCol += View.SortInfo.GroupCount;
						}
					}
				}
			}
			row++;
		}				
		protected virtual void LoadFooterContent(ref int row) {
			if(View.VisibleColumns.Count == 0) {
				if(fCacheWidth > 0)	
					Provider.SetCellStyleAndUnion(0, row, fCacheWidth, 1, fFooterStyle);
				row++;
			}
			else {
				int startCol = 0;
				int maxFooterCount = View.ViewInfo.GetMaxColumnFooterCount();
				for(int f = 0; f < maxFooterCount; f++) {
					for(int i = 0; i < View.VisibleColumns.Count; i++) {
						GridColumn column = View.VisibleColumns[i] as GridColumn;
						GridSummaryItem sitem = column.Summary.GetActiveItem(f);
						if(sitem != null) {
							if(ExportCellsAsDisplayText)
								Provider.SetCellString(startCol + i, row, sitem.GetDisplayText(sitem.SummaryValue, false));
							else
								Provider.SetCellData(startCol + i, row, sitem.SummaryValue);
						}
						Provider.SetCellStyle(startCol + i, row, fFooterStyle);
						if(i == 0) {
							if(isMasterView && fExportDetails) {
								Provider.SetCellUnion(startCol + i, row, View.SortInfo.GroupCount + 2, 1);
								startCol += View.SortInfo.GroupCount + 1;
							}
							else {
								Provider.SetCellUnion(startCol + i, row, View.SortInfo.GroupCount + 1, 1);
								startCol += View.SortInfo.GroupCount;
							}
						}
					}
					row++;
				}
			}
		}
		protected virtual void LoadGroupRowContent(ref int row, int handle) {
			int level = View.GetRowLevel(handle);
			ModifyGroupSeparatorsStyle(level - 1, row);
			if(fExpandAll || View.GetRowExpanded(handle))
				Provider.SetCellString(level, row, " - ");
			else
				Provider.SetCellString(level, row, " + ");
			Provider.SetCellStyle(level, row, fGroupButtonStyle);
			Provider.SetCellUnion(level + 1, row, fCacheWidth - level - 1, 1);
			Provider.SetCellString(level + 1, row, View.GetGroupRowDisplayText(handle));
			Provider.SetCellStyle(level + 1, row, fGroupRowStyle);
			row++;
		}
		protected virtual void LoadRowContent(ref int row, int handle) {
			int level = View.GetRowLevel(handle);
			ModifyGroupSeparatorsStyle(level - 1, row);
			if(isMasterView && fExportDetails) {
				Provider.SetCellStyle(level, row, GetDataRowStyle(handle));
				if(View.IsShowDetailButtons) {
					if(fExpandAll || View.GetMasterRowExpanded(handle))
						Provider.SetCellString(level, row, " - ");
					else
						Provider.SetCellString(level, row, " + ");
					level++;
				}				
			} 
			LoadRowData(level, row, handle);
			row += GetRowHeight(handle);
		}
		protected virtual void LoadRowData(int level, int row, int handle) {
			for(int i = 0; i < View.VisibleColumns.Count; i++) {
				if(ExportCellsAsDisplayText)
					Provider.SetCellString(level + i, row, View.GetRowCellDisplayText(handle, View.GetVisibleColumn(i)));
				else
					Provider.SetCellData(level + i, row, View.GetRowCellValue(handle, View.GetVisibleColumn(i)));				
				if(i == 0 && isMasterView && ExportDetails && !View.IsShowDetailButtons) {
					Provider.SetCellStyleAndUnion(level + i, row, 2, 1, GetDataRowCellStyle(handle, View.GetVisibleColumn(i)));
					level++;
				} else
					Provider.SetCellStyle(level + i, row, GetDataRowCellStyle(handle, View.GetVisibleColumn(i)));
			}
		}
		protected virtual void LoadPreviewContent(ref int row, int handle) {
			int level = View.GetRowLevel(handle);
			for(int i = 0; i < this.fPreviewLineCount; i++)
				ModifyGroupSeparatorsStyle(level, row + i);
			Provider.SetCellUnion(level, row, fCacheWidth - level, fPreviewLineCount);
			string preview = string.Empty;
			if (View.Columns[View.PreviewFieldName] != null) {
				preview = View.GetRowCellDisplayText(handle, View.Columns[View.PreviewFieldName]);
			}
			Provider.SetCellString(level, row, preview);
			Provider.SetCellStyle(level, row, fPreviewStyle);
			row += fPreviewLineCount;
		}
		protected virtual void LoadGroupFooterContent(ref int row, int handle) {
			if(!View.IsExistAnyRowFooterCell(handle))
				return;
			int level = View.GetRowLevel(handle);			
			ModifyGroupSeparatorsStyle(level - 1, row);
			LoadGroupFooterData(ref row, handle, level);
		}
		protected virtual void LoadGroupFooterData(ref int row, int handle, int level) {
			if(View.VisibleColumns.Count == 0) 
				Provider.SetCellStyleAndUnion(level, row, fCacheWidth - level, 1, fGroupFooterStyle);
			else {
				int startCol = level;
				for(int i = 0; i < View.VisibleColumns.Count; i++) {
					GridColumn column = View.VisibleColumns[i] as GridColumn;
					if(ExportCellsAsDisplayText) {
						string rowFooterCellText = View.GetRowFooterCellText(handle, column);
						if(rowFooterCellText != "")
							Provider.SetCellString(startCol + i, row,	rowFooterCellText);
					} else {
						object rowFooterCellValue = View.GetRowSummaryItem(handle, column).Value;
						if(rowFooterCellValue != null)
							Provider.SetCellData(startCol + i, row,	rowFooterCellValue);
					}
					Provider.SetCellStyle(startCol + i, row, fGroupFooterStyle);
					if(i == 0) {
						if(isMasterView && fExportDetails) {
							if(level < View.SortInfo.GroupCount + 1) {
								Provider.SetCellUnion(startCol + i, row, View.SortInfo.GroupCount - level + 2, 1);
								startCol += (View.SortInfo.GroupCount - level + 1);
							}
						} else {
							if(level < View.SortInfo.GroupCount) {
								Provider.SetCellUnion(startCol + i, row, View.SortInfo.GroupCount - level + 1, 1);
								startCol += (View.SortInfo.GroupCount - level);
							}
						}
					}
				}
			}			
			row++;
		}
		protected virtual int GetHeaderHeight() {
			return 1;
		}
		protected virtual int GetRowHeight(int handle) {
			return 1;
		}
		protected virtual int GetFooterHeight() {
			return ViewInfo.GetMaxColumnFooterCount();
		}
		protected override int GetCacheWidth() {
			return fCacheWidth;
		}
		protected override int GetCacheHeight() {
			return fCacheHeight;
		}
		protected override ExportCacheCellStyle GetDefaultStyle() {
			ExportCacheCellStyle result = Provider.GetDefaultStyle();
			result.TopBorder.Width = 1;
			result.BottomBorder.Width = 1;
			result.LeftBorder.Width = 1;
			result.RightBorder.Width = 1;
			return result;
		}
		protected override void DoExport() {
			fCacheWidth = CalcCacheWidth();
			fCacheHeight = CalcCacheHeight();
			if(fCacheWidth > 0 && fCacheHeight > 0) {
				Provider.SetRange(fCacheWidth, fCacheHeight, false);
				SetColumnsWidth();
				RegisterStyles();
				LoadCache(0);
			}
		}
		protected override bool TestView() {
			return fView is GridView;
		}
		public override void Copy(BaseExportLink link) {
			base.Copy(link);
			if(link is GridViewExportLink) {
				fExpandAll = ((GridViewExportLink)link).ExpandAll;	
				fExportDetails = ((GridViewExportLink)link).ExportDetails;
			}
		}
		protected bool IsMasterViewMode { get { return View.DataController.IsSupportMasterDetail && View.AllowMasterDetail && Provider is IExportInternalProvider; } }
		public new GridView View {
			get {
				return (GridView)fView;
			}
		}
		public bool ExpandAll {
			get {
				return fExpandAll;
			}
			set {
				fExpandAll = value;
			}
		}
		public bool ExportDetails {
			get {
				return fExportDetails;
			}
			set {
				fExportDetails = value;
			}
		}
	}
}
