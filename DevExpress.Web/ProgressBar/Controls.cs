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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web;
namespace DevExpress.Web.Internal {
	public class ProgressControl : ASPxInternalWebControl {
		private ASPxProgressBarBase progressBar = null;
		private Table mainTable = null;
		private TableCell mainCell = null;
		private WebControl divIndicator = null;
		private Table valueIndicatorTable = null;
		private TableCell valueIndicatorCell = null;
		private LiteralControl valueIndicatorLiteral = null;
		public ProgressControl(ASPxProgressBarBase progressBar) {
			this.progressBar = progressBar;
		}
		protected ASPxProgressBarBase ProgressBar {
			get { return progressBar; }
		}
		protected bool IsRightToLeft {
			get { return (ProgressBar as ISkinOwner).IsRightToLeft(); }
		}
		protected Table MainTable {
			get { return mainTable; }
		}
		protected TableCell MainCell {
			get { return mainCell; }
		}
		protected WebControl DivIndicator {
			get { return divIndicator; }
		}
		protected Table ValueIndicatorTable {
			get { return valueIndicatorTable; }
		}
		protected TableCell ValueIndicatorCell {
			get { return valueIndicatorCell; }
		}
		protected LiteralControl ValueIndicatorLiteral {
			get { return valueIndicatorLiteral; }
		}
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.mainCell = null;
			this.divIndicator = null;
			this.valueIndicatorTable = null;
			this.valueIndicatorCell = null;
			this.valueIndicatorLiteral = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateMainTable();
			CreateDivIndicator();
			if(ProgressBar.ShowPosition)
				CreateValueIndicator();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareMainTable();
			PrepareDivIndicator();
			if(ValueIndicatorTable != null)
				PrepareValueIndicator();
		}
		protected void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			this.mainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(MainCell);
		}
		protected void CreateDivIndicator() {
			this.divIndicator = RenderUtils.CreateDiv();
			MainCell.Controls.Add(DivIndicator);
			DivIndicator.ID = ProgressBar.GetDivIndicatorID();
		}
		protected void CreateValueIndicator() {
			this.valueIndicatorTable = RenderUtils.CreateTable();
			MainCell.Controls.Add(ValueIndicatorTable);
			TableRow row = RenderUtils.CreateTableRow();
			ValueIndicatorTable.Rows.Add(row);
			this.valueIndicatorCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ValueIndicatorCell);
			RenderUtils.AppendDefaultDXClassName(ValueIndicatorCell, ProgressBar.GetCssClassNamePrefix());
			ValueIndicatorCell.ID = ProgressBar.GetValueIndicatorCellID();
			this.valueIndicatorLiteral = RenderUtils.CreateLiteralControl();
			ValueIndicatorCell.Controls.Add(ValueIndicatorLiteral);
		}
		protected void PrepareMainTable() {
			AppearanceStyleBase style = ProgressBar.GetControlStyle();
			RenderUtils.AssignAttributes(ProgressBar, MainTable);
			style.AssignToControl(MainTable, AttributesRange.Common | AttributesRange.Font);
			RenderUtils.SetVisibility(MainTable, ProgressBar.IsClientVisible(), true);
			style.AssignToControl(MainCell, AttributesRange.Cell);
			RenderUtils.AppendDefaultDXClassName(MainCell, ProgressBar.GetMainCellCssClassName());
			RenderUtils.SetPaddings(MainCell, ProgressBar.GetPaddings());
			RenderUtils.SetStringAttribute(MainCell, "align", IsRightToLeft ? "right" : "left");
		}
		protected void PrepareDivIndicator() {
			IndicatorStyle style = ProgressBar.GetIndicatorStyle();
			style.AssignToControl(DivIndicator, AttributesRange.Common);
			if (DesignMode)
				RenderUtils.SetStyleStringAttribute(DivIndicator, "overflow", "hidden");
			DivIndicator.Width = ProgressBar.GetDivIndicatorWidth();
			if (DesignMode)
				DivIndicator.Height = ProgressBar.GetDivIndicatorHeight();
		}
		protected void PrepareValueIndicator() {
			AppearanceStyleBase style = ProgressBar.GetValueIndicatorStyle();
			style.AssignToControl(ValueIndicatorTable, AttributesRange.Common | AttributesRange.Font);
			style.AssignToControl(ValueIndicatorCell, AttributesRange.Cell | AttributesRange.Font);
			RenderUtils.SetMargins(ValueIndicatorTable, ProgressBar.GetValueIndicatorMargins());
			ValueIndicatorTable.Width = Unit.Percentage(100);
			if(DesignMode)
				ValueIndicatorTable.Height = ProgressBar.GetControlCellHeight();
			ValueIndicatorLiteral.Text = ProgressBar.GetIndicatorValueText();
		}
	}
}
namespace DevExpress.Web.Internal {
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	public class ProgressBarEditControl : ASPxProgressBarBase {
		ASPxProgressBar progressBar = null;
		public ProgressBarEditControl(ASPxProgressBar progressBar)
			: base(progressBar) {
			this.progressBar = progressBar;
			DisplayMode = ProgressBar.DisplayMode;
			ShowPosition = ProgressBar.ShowPosition;
			RightToLeft = progressBar.RightToLeft;
		}
		public ASPxProgressBar ProgressBar {
			get { return this.progressBar; }
		}
		protected override StylesBase CreateStyles() {
			return new ProgressBarStylesInternal(this);
		}
		protected override bool IsDefaultAppearanceEnabled() {
			return false;
		}
	}
	public class ProgressBarStylesInternal : DevExpress.Web.ProgressBarStyles {
		ProgressBarEditControl progressBarEditControl;
		public ProgressBarStylesInternal(ISkinOwner progressBarEditControl)
			: base(progressBarEditControl) {
			this.progressBarEditControl = (ProgressBarEditControl)progressBarEditControl;
		}
		public ProgressBarEditControl ProgressBarEditControl {
			get { return this.progressBarEditControl; }
		}
		protected internal override string GetCssClassNamePrefix() {
			return ProgressBarEditControl.ProgressBar.GetCssClassNamePrefix();
		}
		public override string GetCssFilePath() {
			return "";
		}
	}
}
