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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Collections.Generic;
using System.Collections;
namespace DevExpress.Web.ASPxTreeList.Internal {
	[ToolboxItem(false)]
	public class TreeListCustomizationWindow : ASPxPopupControl {
		static readonly Paddings ContentPaddings = new Paddings(0);
		TreeListRenderHelper helper;
		Table mainTable;
		public TreeListCustomizationWindow(TreeListRenderHelper helper) 
			: base(helper.TreeList) {
			this.helper = helper;						
			EnableViewState = false;
			ParentSkinOwner = TreeList;
			AllowDragging = true;
			PopupAnimationType = AnimationType.Fade;
			CloseAction = CloseAction.CloseButton;
			ID = TreeListRenderHelper.DragAndDropTargetMark + TreeListRenderHelper.CustomizationWindowSuffix;			
		}
		protected TreeListRenderHelper RenderHelper { get { return helper; } }
		protected ASPxTreeList TreeList { get { return RenderHelper.TreeList; } }
		protected Table MainTable { get { return mainTable; } }		
		protected internal override Paddings GetContentPaddings(PopupWindow window) {
			return ContentPaddings; 
		}
		protected override void ClearControlFields() {			
			this.mainTable = null;
		}
		protected override bool BindContainersOnCreate() {
			return false;
		}
		protected override void CreateControlHierarchy() {			
			if(Request != null)
				LoadPostData(Request.Params);
			base.CreateControlHierarchy();
			this.mainTable = RenderUtils.CreateTable();			
			Controls.Add(MainTable);
			CreateHeaders();
			if(!String.IsNullOrEmpty(TreeList.ClientSideEvents.CustomizationWindowCloseUp))
				ClientSideEvents.CloseUp = RenderHelper.GetCustomizationWindowOnCloseUp();
			EnsureChildControlsRecursive(this, false);
		}
		protected override void PrepareControlHierarchy() {
			ClientInstanceName = ClientID.Replace('-', '_');
			TreeList.SettingsCustomizationWindow.AssignToPopupControl(this);
			MainTable.Width = Unit.Percentage(100);
			MainTable.CellSpacing = 3;
			TreeListStyles styles = TreeList.Styles;
			ControlStyle.CopyFrom(styles.CustomizationWindow);
			CloseButtonStyle.CopyFrom(styles.CustomizationWindowCloseButton);
			HeaderStyle.CopyFrom(styles.CustomizationWindowHeader);
			ContentStyle.CopyFrom(styles.CustomizationWindowContent);
			CloseButtonImage.CopyFrom(TreeList.Images.CustomizationWindowClose);
			if(RenderUtils.Browser.Platform.IsMSTouchUI)
				RenderUtils.AppendDefaultDXClassName(MainTable, TreeListHeaderStyle.MSTouchDraggableMarkerCssClassName);
		}
		void CreateHeaders() {
			IList<TreeListColumn> columns = GetColumnsCustomizationWindow();
			foreach(TreeListColumn column in columns) {
				TableRow row = RenderUtils.CreateTableRow();
				MainTable.Rows.Add(row);
				row.Cells.Add(new TreeListHeaderCell(TreeList.RenderHelper, column, false));
			}
			if(columns.Count < 1) {
				TableRow row = RenderUtils.CreateTableRow();
				MainTable.Rows.Add(row);
				row.Cells.Add(RenderUtils.CreateTableCell());
			}
		}
		IList<TreeListColumn> GetColumnsCustomizationWindow() {
			List<TreeListColumn> result = new List<TreeListColumn>();
			foreach(TreeListColumn column in TreeList.Columns) {
				if(column.Visible || !column.ShowInCustomizationForm)
					continue;
				result.Add(column);
			}
			result.Sort(delegate(TreeListColumn a, TreeListColumn b) {
				return Comparer.Default.Compare(a.GetCaption(), b.GetCaption());
			});
			return result;
		}
	}
}
