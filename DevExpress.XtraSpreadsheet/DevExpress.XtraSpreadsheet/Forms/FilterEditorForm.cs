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

using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class FilterEditorForm : ExpressionEditorForm {
		#region fields
		FilterEditorViewModel viewModel;
		SpreadsheetFieldListTreeView fieldList;
		string dataMember;
		SpreadsheetControl control;
		#endregion
		FilterEditorForm() {
		}
		public FilterEditorForm(FilterEditorViewModel viewModel, SpreadsheetControl control)
			: base(viewModel, null) {
			this.Text = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_MailMergeFilterExpressionEditor);
			this.viewModel = viewModel;
			this.control = control;
			this.dataMember = viewModel.DataMember;
			InitializeFieldList();
		}
		#region Properties
		protected internal FilterEditorViewModel ViewModel { get { return viewModel; } }
		#endregion
		void InitializeFieldList() {
			this.fieldList = new SpreadsheetFieldListTreeView();
			this.Controls.Add(fieldList);
			this.fieldList.Visible = false;
			this.fieldList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.fieldList.SpreadsheetControl = control;
			this.fieldList.DataMember = dataMember;
			this.fieldList.ShowComplexChildren = false;
			this.fieldList.NeedDoubleClick = false;
			SubscribeEvents();
			this.fieldList.RefreshTreeList();
		}
		protected override Data.ExpressionEditor.ExpressionEditorLogic CreateExpressionEditorLogic() {
			return new FilterEditorFormLogic(this, null, ViewModel.Parameters);
		}
		protected override object ExtractContext(object contextInstance) {
			FilterEditorViewModel filterEditorInstance = contextInstance as FilterEditorViewModel;
			if (filterEditorInstance != null)
				viewModel = filterEditorInstance;
			return base.ExtractContext(contextInstance);
		}
		public void ShowFieldList() {
			fieldList.Visible = true;
			fieldList.Bounds = GetParametersEditorBounds();
			fieldList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			HideParameters();
		}
		public void HideFieldList() {
			if (fieldList == null || !fieldList.Visible)
				return;
			fieldList.Visible = false;
			ShowParameters();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			UnsubscribeEvents();
		}
		void SubscribeEvents() {
			fieldList.MouseDoubleClick += new MouseEventHandler(fieldList_MouseDoubleClick);
			fieldList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(fieldList_FocusedNodeChanged);
		}
		void UnsubscribeEvents() {
			fieldList.MouseDoubleClick -= new MouseEventHandler(fieldList_MouseDoubleClick);
			fieldList.FocusedNodeChanged -= new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(fieldList_FocusedNodeChanged);
		}
		void fieldList_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e) {
			DataMemberListNodeBase node = fieldList.FocusedNode as DataMemberListNodeBase;
			if (node == null || node.Property == null)
				return;
			if (!node.HasChildren)
				SetDescription(GetResourceString("Fields Description Prefix") + node.Property.PropertyType.ToString());
			else SetDescription(string.Empty);
		}
		void fieldList_MouseDoubleClick(object sender, MouseEventArgs e) {
			string[] result = fieldList.SelectedNodes();
			if(result.Length > 0)
				fEditorLogic.InsertTextInExpressionMemo(MailMergeFieldInfo.WrapColumnInfoInBrackets(result[0], string.Empty));
		}
	}
}
