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

using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.Localization;
using DevExpress.XtraVerticalGrid.Rows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraVerticalGrid.Internal {
	public class FindPanelOwner : FindPanelOwnerBase {
		string findFilterText;
		public VGridControlBase Grid { get; protected set; }
		public ISearchControl SearchControl { get; set; }
		public FindPanelOwner(VGridControlBase grid) {
			Grid = grid;
		}
		public override void FocusOwner() {
			if (Grid.FocusedRow == null || !Grid.VisibleRows.Contains(Grid.FocusedRow))
				Grid.FocusedRow = Grid.GetFirstVisible();
			Grid.Focus();
		}
		public override void InitButtons() {
			if (FindPanel == null)
				return;
			FindPanel.FindEdit.Properties.NullValuePrompt = Grid.OptionsFind.FindNullPrompt;
			FindPanel.FindEdit.Properties.NullValuePromptShowForEmptyValue = true;
			FindPanel.FindEdit.Properties.ShowNullValuePromptWhenFocused = true;
			FindPanel.FindButton.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.FindControlFindButton);
			FindPanel.ClearButton.Text = VGridLocalizer.Active.GetLocalizedString(VGridStringId.FindControlClearButton);
			FindPanel.AdjustButtonSize(FindPanel.FindButton);
			FindPanel.AdjustButtonSize(FindPanel.ClearButton);
			FindPanel.FindButton.Visible = Grid.OptionsFind.ShowFindButton;
			FindPanel.CloseButton.Visible = Grid.OptionsFind.ShowCloseButton && Grid.OptionsFind.Visibility != FindPanelVisibility.Always;
			FindPanel.ClearButton.Visible = Grid.OptionsFind.ShowClearButton;
		}
		public override void FocusFindEditor() {
			if (TryCloseActiveEditor())
				FindPanel.FindEdit.Focus();
			else
				Grid.Focus();
		}
		public override void HideFindPanel() {
			if (Grid.OptionsFind.Visibility == FindPanelVisibility.Always)
				return;
			Grid.HideFindPanel();
		}
		public override void UpdateTimer(Timer updateTimer) {
			updateTimer.Interval = Grid.OptionsFind.FindDelay;
		}
		public override bool ApplyFindFilter(string text) {
			if (findFilterText == text)
				return false;
			if (text == null)
				text = string.Empty;
			if (FindPanel != null && Grid.FindPanelVisible)
				FindPanel.SetFilterText(text);
			findFilterText = text;
			Grid.FindFilterText = text;
			ParseFindFilterText(text);
			FilterGrid();
			return true;   
		}
		void FilterGrid() {
			Grid.FindFilterCriteria = this.findFilterCriteria;
		}
		protected override void FindPanelChanged() {
			FindPanel.LookAndFeel.ParentLookAndFeel = Grid.ElementsLookAndFeel;
			InitButtons();
		}
		internal bool TryCloseActiveEditor() {
			if (Grid.ActiveEditor == null)
				return true;
			Grid.ContainerHelper.BeginAllowHideException();
			try {
				Grid.CloseEditor();
			}
			catch (HideException) {
				return false;
			}
			finally {
				Grid.ContainerHelper.EndAllowHideException();
			}
			return true;
		}
		FindSearchParserResults findFilterParserResults = null;
		CriteriaOperator findFilterCriteria = null;
		protected void ParseFindFilterText(string text) {
			this.findFilterParserResults = null;
			this.findFilterCriteria = null;
			if (!string.IsNullOrEmpty(text)) {
				this.findFilterParserResults = new FindSearchParser().Parse(text, GetFindColumnNames());
				if(Grid.FindByDisplayText) {
					this.findFilterParserResults.AppendColumnFieldPrefixes();
				}
				this.findFilterCriteria = DxFtsContainsHelperAlt.Create(findFilterParserResults, FilterCondition.Contains, false);
			}
		}
		public IDataColumnInfo[] GetFindColumnNames() {
			return Grid.GetFindColumnNames();
		}
		public string GetFindMatchedText(string fieldName, string displayText) {
			if (findFilterParserResults == null)
				return string.Empty;
			return findFilterParserResults.GetMatchedText(fieldName, displayText);
		}
		public bool IsFindFilterActive {
			get { return !string.IsNullOrEmpty(Grid.FindFilterText); }
		}
		protected internal virtual bool IsAllowHighlightFind(RowProperties col) {
			if (col != null && IsFindFilterActive && Grid.OptionsFind.HighlightFindResults && IsAllowFindColumn(col))
				return true;
			return false;
		}
		internal protected virtual bool IsAllowFindColumn(RowProperties col) {
			if(col == null || string.IsNullOrEmpty(col.FieldName) || !col.Bindable)
				return false;
			if(Grid.OptionsFind.FindFilterColumns == "*") {
				DataColumnInfo columnInfo = Grid.DataManager.Columns[col.FieldName];
				if (columnInfo == null && !Grid.DataManager.CanFindColumn(columnInfo))
					return false;
				return true;
			}
			return string.Concat(";", Grid.OptionsFind.FindFilterColumns, ";").Contains(string.Concat(";", col.FieldName, ";"));
		}
		public virtual void InitFindPanel() {
			if (FindPanel == null)
				return;
			FindPanel.SetFilterText(Grid.FindFilterText);
			UpdateFindPanelFindMode();
		}
		void UpdateFindPanelFindMode() {
			FindPanel.AllowAutoApply = (Grid.OptionsFind.FindMode == FindMode.Always || Grid.OptionsFind.FindMode == FindMode.Default);
		}
	}
}
