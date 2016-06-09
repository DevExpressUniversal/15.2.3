#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.TestScripts;
using System.Collections;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ASPxDefaultPropertyEditor : WebPropertyEditor, ITestable {
		private void grid_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e) {
			object obj = e.Column.Grid.GetRow(e.VisibleRowIndex);
			e.DisplayText = obj == null ? "" : obj.ToString();
		}
		private Boolean IsListMember() {
			return 
				!typeof(String).IsAssignableFrom(MemberInfo.MemberType)
				&&
				(
					typeof(IEnumerable).IsAssignableFrom(MemberInfo.MemberType)
					||
					typeof(ITypedList).IsAssignableFrom(MemberInfo.MemberType)
				);
		}
		protected override WebControl CreateViewModeControlCore() {
			if(IsListMember()) {
				ASPxGridView grid = new ASPxGridView();
				grid.CustomColumnDisplayText += new ASPxGridViewColumnDisplayTextEventHandler(grid_CustomColumnDisplayText);
				grid.Width = Unit.Percentage(100);
				grid.Settings.ShowColumnHeaders = false;
				GridViewDataColumn column = new GridViewDataColumn();
				column.FieldName = "DefaultEditorColumn";
				column.UnboundType = DevExpress.Data.UnboundColumnType.Object;
				grid.Columns.Add(column);
				return grid;
			}
			else {
				return new Label();
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			return CreateViewModeControlCore();
		}
		protected override void ReadEditModeValueCore() {
			if(IsListMember()) {
				((ASPxGridView)Editor).DataSource = PropertyValue;
				((ASPxGridView)Editor).DataBind();
			}
			else {
				((Label)Editor).Text = GetPropertyDisplayValue();
			}
		}
		protected override void ReadViewModeValueCore() {
			if((InplaceViewModeEditor is ASPxGridView) && IsListMember()) {
				((ASPxGridView)InplaceViewModeEditor).DataSource = PropertyValue;
				((ASPxGridView)InplaceViewModeEditor).DataBind();
			}
			else {
				base.ReadViewModeValueCore();
			}
		}
		protected override Object GetControlValueCore() {
			return null;
		}
		public ASPxDefaultPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			AllowEdit.SetItemValue("ASPxDefaultPropertyEditor", false);
		}
		public override bool IsCaptionVisible {
			get {
				if(IsListMember()) {
					return false;
				}
				else {
					return base.IsCaptionVisible;
				}
			}
		}
	}
}
