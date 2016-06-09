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
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Localization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.Design;
using DevExpress.XtraBars;
using System.Drawing;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using System.CodeDom.Compiler;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design.ErrorList {
	[System.ComponentModel.ToolboxItem(false)]
	public class ErrorListUserControl : XtraUserControl, ISupportController {
		#region inner classes
		class CustomTreeList : TreeList {
			TreeListColumn lineColumn;
			TreeListColumn columnColumn;
			TreeListColumn descriptionColumn;
			public int Line {
				get { return (int)this.FocusedNode.GetValue(lineColumn) - 1; }
			}
			public int Column {
				get { return (int)this.FocusedNode.GetValue(columnColumn) - 1; }
			}
			public string Description {
				get { return (string)this.FocusedNode.GetValue(descriptionColumn); }
			}
			public CustomTreeList()
				: base(null) {
				descriptionColumn = new TreeListColumn();
				lineColumn = new TreeListColumn();
				columnColumn = new TreeListColumn();
				this.Columns.AddRange(new TreeListColumn[] { descriptionColumn, lineColumn, columnColumn });
				this.Dock = DockStyle.Fill;
				this.Location = new Point(0, 0);
				this.Name = "treeList";
				this.OptionsView.ShowIndicator = false;
				this.OptionsView.ShowRoot = false;
				this.OptionsView.AutoWidth = true;
				this.OptionsView.FocusRectStyle = DrawFocusRectStyle.CellFocus;
				this.OptionsSelection.EnableAppearanceFocusedRow = true;
				this.OptionsSelection.EnableAppearanceFocusedCell = true;
				this.OptionsBehavior.Editable = false;
				this.Size = new Size(634, 116);
				this.TabIndex = 0;
				this.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				descriptionColumn.Caption = ReportLocalizer.GetString(ReportStringId.ScriptEditor_ErrorDescription);
				descriptionColumn.Name = "Description";
				descriptionColumn.Visible = true;
				descriptionColumn.VisibleIndex = 0;
				descriptionColumn.Width = 861;
				descriptionColumn.MinWidth = 100;
				lineColumn.Caption = ReportLocalizer.GetString(ReportStringId.ScriptEditor_ErrorLine);
				lineColumn.Name = "Line";
				lineColumn.Visible = true;
				lineColumn.VisibleIndex = 1;
				lineColumn.Width = 50;
				lineColumn.MinWidth = 50;
				columnColumn.Caption = ReportLocalizer.GetString(ReportStringId.ScriptEditor_ErrorColumn);
				columnColumn.Name = "Column";
				columnColumn.Visible = true;
				columnColumn.VisibleIndex = 2;
				columnColumn.Width = 50;
				columnColumn.MinWidth = 50;
			}
		}
		#endregion
		private System.ComponentModel.IContainer components = null;
		CustomTreeList treeList;
		DevExpress.XtraBars.BarManager barManager;
		TreeListController activeController;
		string errorMessage;
		string Message {
			get {
				return !string.IsNullOrEmpty(ErrorMessage) ? ErrorMessage : GetValidScriptsMessage();
			}
		}
		public string ErrorMessage { 
			get { return errorMessage; }
			set { errorMessage = value; } 
		}
		public TreeListController ActiveController {
			get {
				return activeController;
			}
			set {
				activeController = value;
			}
		}
		TreeListController ISupportController.CreateController(IServiceProvider serviceProvider) {
			return new ErrorListController(serviceProvider);
		}
		public ErrorListUserControl() {
			treeList = new CustomTreeList();
			Initialize();
			treeList.MenuManager = barManager;
			this.Controls.Add(treeList);
			treeList.CustomDrawEmptyArea += new DevExpress.XtraTreeList.CustomDrawEmptyAreaEventHandler(treeList_CustomDrawEmptyArea);
			treeList.DoubleClick += new EventHandler(treeList_DoubleClick);
		}
		void Initialize() {
			this.components = new System.ComponentModel.Container();
			barManager = new RuntimeBarManager(this.components);
			barManager.Form = this;
		}
		public void SetLookAndFeel(IServiceProvider serviceProvider) {
			DesignLookAndFeelHelper.SetParentLookAndFeel(this.treeList, serviceProvider);
		}
		void treeList_DoubleClick(object sender, EventArgs e) {
			if(treeList.FocusedNode != null && ActiveController != null) {
				ReportTabControl tabControl = ActiveController.GetService(typeof(ReportTabControl)) as ReportTabControl;
				ScriptControl scriptControl = ActiveController.GetService(typeof(ScriptControl)) as ScriptControl;
				if(tabControl != null && scriptControl != null) {
					tabControl.SelectedIndex = TabIndices.Scripts;
					scriptControl.SetCaretPosition(treeList.Line, treeList.Column);
				}
			}
		}
		public void ShowErrors(CompilerErrorCollection errors, int linesLength) {
			treeList.ClearNodes();
			foreach(CompilerError error in errors) {
				treeList.AppendNode(new object[] { error.ErrorText, error.GetValidLine(linesLength), error.GetValidColumn(linesLength) }, null);
			}
		}
		public void ClearErrors() {
			treeList.ClearNodes();
		}
		protected CompilerError GetSelectedError() {
			if(treeList == null || treeList.FocusedNode == null)
				return null;
			CompilerError error = new CompilerError();
			error.Column = treeList.Column;
			error.Line = treeList.Line;
			error.ErrorText = treeList.Description;
			return error;
		}
		static int GetLine(CompilerError error, int linesLength) {
			return error.Line <= linesLength ? Math.Max(1, error.Line) : 1;
		}
		static int GetColumn(CompilerError error, int linesLength) {
			return error.Line <= linesLength ? Math.Max(1, error.Column) : 1;
		}
		void treeList_CustomDrawEmptyArea(object sender, DevExpress.XtraTreeList.CustomDrawEmptyAreaEventArgs e) {
			if(treeList.Nodes.Count != 0)
				return;
			TreeListPaintHelper.DrawString(e, Message); 
			e.Handled = true;
		}
		string GetValidScriptsMessage() {
			string message = ReportLocalizer.GetString(ReportStringId.ScriptEditor_ScriptsAreValid);
			if(ActiveController == null)
				return message;
			ScriptControl scriptControl = (ScriptControl)this.ActiveController.GetService(typeof(ScriptControl));
			if(scriptControl != null && scriptControl.TextWasChanged)
				message = ReportLocalizer.GetString(ReportStringId.ScriptEditor_ClickValidate);
			return message;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			try {
				if(treeList != null) {
					treeList.CustomDrawEmptyArea -= new DevExpress.XtraTreeList.CustomDrawEmptyAreaEventHandler(treeList_CustomDrawEmptyArea);
					treeList.DoubleClick -= new EventHandler(treeList_DoubleClick);
				}
			} finally {
				base.Dispose(disposing);
			}
		}
	}
	static class TreeListPaintHelper {
		public static void DrawString(DevExpress.XtraTreeList.CustomDrawEmptyAreaEventArgs e, string s) {
			Rectangle bounds = e.EmptyRows;
			bounds.Width = e.Bounds.Width;
			e.Cache.FillRectangle(e.Appearance.BackColor, bounds);
			Font font = e.Appearance.Font;
			Brush brush = e.Cache.GetSolidBrush(SystemColors.GrayText);
			using(StringFormat strFormat = new StringFormat()) {
				strFormat.Alignment = StringAlignment.Center;
				strFormat.LineAlignment = StringAlignment.Center;
				e.Cache.DrawString(s, font, brush, bounds, strFormat);
			}
		}
	}
}
namespace System.CodeDom.Compiler {
	public static class CompilerErrorExtentions {
		public static int GetValidLine(this CompilerError error, int linesLength) {
			return error.Line <= linesLength ? Math.Max(1, error.Line) : 1;
		}
		public static int GetValidColumn(this CompilerError error, int linesLength) {
			return error.Line <= linesLength ? Math.Max(1, error.Column) : 1;
		}
	}
}
