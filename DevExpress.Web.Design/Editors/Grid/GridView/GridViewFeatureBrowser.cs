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
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraEditors.FeatureBrowser;
namespace DevExpress.Web.Design {
	[CLSCompliant(false)]
	public class GridViewFeatureBrowserFrame : FeatureBrowserMainFrameWeb {
		public override Type FeatureBrowserFormBase { get { return typeof(GridViewFeatureBrowserForm); } }
		public override string[] XmlResourceFullNames { get { return new string[] { @"DevExpress.Web.Design.Editors.Grid.GridView.FeatureBrowserStructure.xml" }; } }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			TreeViewItems.ExpandAll();
		}
	}
	[CLSCompliant(false)]
	public class GridViewFeatureBrowserForm : FeatureTabbedViewForm {
		ASPxGridView grid;
		ASPxGridView Grid {
			get {
				if(grid == null)
					grid = (ASPxGridView)SourceObject;
				return grid;
			}
		}
		protected override void FillPageToFrameAssociator() {
			AddPageToFrameAssociation("Columns_Binding", GetColumnDataBindingEditorFrame());
			AddPageToFrameAssociation("Columns_Page", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Sorting_Page", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Grouping_Page", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Filtering", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Filtering_HeaderFilter", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Filtering_FilterBar", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Filtering_SearchPanel", GetColumnEditorFrame());
			AddPageToFrameAssociation("Editing", new GridViewFeatureBrowserEditingFrame());
			AddPageToFrameAssociation("CommandColumns_Editing", GetColumnEditorFrame());
			AddPageToFrameAssociation("Columns_Scrolling", GetColumnEditorFrame());
			AddPageToFrameAssociation("CommandColumns_Selection", GetColumnEditorFrame());
			AddPageToFrameAssociation("FocusSelectionNavigation", new GridViewFeatureBrowserPagingScrollingFrame());
			AddPageToFrameAssociation("Items_TotalSummary_Page", GetSummaryEditorFrame("Total Summary", Grid.TotalSummary));
			AddPageToFrameAssociation("Items_GroupSummary_Page", GetSummaryEditorFrame("Group Summary", Grid.GroupSummary));
			AddPageToFrameAssociation("FormatConditions_Selector", GetFormatConditionsEditorFrame(Grid.FormatConditions));
		}
		IEmbeddedFrame GetFormatConditionsEditorFrame(GridViewFormatConditionCollection formatConditions) {
			return new ItemsEditorFrame(new FormatConditionItemsOwner(Grid, grid.Site, formatConditions));
		}
		IEmbeddedFrame GetSummaryEditorFrame(string caption, ASPxSummaryItemCollection summaryItems) {
			return new ItemsEditorFrame(new SummaryItemsOwner(Grid, grid.Site, caption, summaryItems));
		}
		IEmbeddedFrame GetGroupSummaryEditorFrame() {
			return new ColumnsEditorFrame(Grid);
		}
		IEmbeddedFrame GetColumnDataBindingEditorFrame() {
			var result = GetColumnEditorFrame();
			result.ShowFilterEditorToolbar = true;
			return result;
		}
		ColumnsEditorFrame GetColumnEditorFrame() {
			return new ColumnsEditorFrame(Grid);
		}
		protected override FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new FeatureBrowserDefaultPageDescriptions();
		}
		protected override void OnLabelInfoItemClickGoto(string gotoName, string gotoValue) {
			var tabPage = MainTabControl.TabPages.FirstOrDefault(t => ((FeatureBrowserItemPage)t.Tag).Name == gotoName);
			if(tabPage != null) {
				MainTabControl.SelectedTabPage = tabPage;
				if(tabPage.Controls.Count != 0) {
					var featurePage = tabPage.Controls[0] as FeatureBrowserDefaultPageDescriptions;
					if(featurePage != null)
						featurePage.SelectProperty(gotoValue);
				}
			}
		}
	}
	[CLSCompliant(false)]
	public class GridViewFeatureBrowserEditingFrame : FeatureBrowserDefaultPageDescriptions {
		GridViewEditingDescriptionActionsOwner descriptionActions;
		protected override DescriptionActions DescriptionActions {
			get {
				if(descriptionActions == null)
					descriptionActions = new GridViewEditingDescriptionActionsOwner(GridView);
				return descriptionActions;
			}
		}
		ASPxGridView GridView { get { return (ASPxGridView)EditingObject; } }
	}
	[CLSCompliant(false)]
	public class GridViewFeatureBrowserPagingScrollingFrame : FeatureBrowserDefaultPageDescriptions { 
		GridViewPagingScrollingDescriptionActionsOwner descriptionActions;
		protected override DescriptionActions DescriptionActions {
			get {
				if(descriptionActions == null)
					descriptionActions = new GridViewPagingScrollingDescriptionActionsOwner(GridView);
				return descriptionActions;
			}
		}
		ASPxGridView GridView { get { return (ASPxGridView)EditingObject; } }
	}
	[CLSCompliant(false)]
	public class ColumnsEditorFrame : FeatureBrowserDefaultPageDescriptions {
		FilteredColumnsEditorFrame filteredEditor;
		public ColumnsEditorFrame(ASPxGridView grid)
			: base() {
			Grid = grid;
			Load += (s, e) => {
				SuspendLayout();
				var container = pgMain.Parent;
				container.Controls.Clear();
				FilteredEditor.Parent = container;
				FilteredEditor.Dock = DockStyle.Fill;
				pgMain = FilteredEditor.ItemPropertyGrid;
				ResumeLayout(true);
			};
		}
		public bool ShowFilterEditorToolbar { get; set; }
		protected override SplitContainerControl SplitContainer { get { return FilteredEditor.MainSplitContainer; } }
		ASPxGridView Grid { get; set; }
		FilteredColumnsEditorFrame FilteredEditor {
			get {
				if(filteredEditor == null) {
					filteredEditor = new FilteredColumnsEditorFrame(Grid, EmbeddedFrameInit);
					filteredEditor.ShowTopPanel = ShowFilterEditorToolbar;
				}
				return filteredEditor;
			}
		}
		public override void SelectProperty(string name) {
			FilteredEditor.SelectionChanged();
			base.SelectProperty(name);
		}
	}
	public class FilteredColumnsEditorFrame : ItemsEditorFrame {
		public FilteredColumnsEditorFrame(ASPxGridView grid, EmbeddedFrameInit embeddedInitObject)
			: base(new GridViewColumnPropertiesOwner(grid, grid.Columns, embeddedInitObject.Properties)) {
			EmbeddedFrameInitObject = embeddedInitObject;
			ParentChanged += (s, e) => {
				Initialize();
				PostponeInitializeFrame();
				PropertiesTabControl.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
				EditorPropertiesTab.PageVisible = false;
			};
		}
	}
	public class GridViewPagingScrollingDescriptionActionsOwner : GridViewFeatureDescriptionsOwner {
		public GridViewPagingScrollingDescriptionActionsOwner(ASPxGridView gridView)
			: base(gridView) {
		}
		protected internal override string FeatureItemName { get { return "FocusSelectionNavigation"; } }
		protected override void FillDescriptionItems() {
			AddDescriptionAction("CommandColumns_ShowSelectCheckbox", ColumnsOwner.CommandColumn_CheckShowSelectCheckbox, "Selection", "CommandColumns_Selection", "ShowSelectCheckbox", true, ColumnsOwner.CommandColumn_ShowSelectCheckboxChecked);
		}
	}
	public class GridViewEditingDescriptionActionsOwner : GridViewFeatureDescriptionsOwner {
		public GridViewEditingDescriptionActionsOwner(ASPxGridView gridView)
			: base(gridView) {
		}
		protected internal override string FeatureItemName { get { return "Editing"; } }
		protected override void FillDescriptionItems() {
			AddDescriptionCheckBoxCommandColumnEditing("CommandColumns_ShowNewButton", ColumnsOwner.CommandColumn_CheckNewButton, "ShowNewButton", ColumnsOwner.CommandColumn_ShowNewButtonChecked);
			AddDescriptionCheckBoxCommandColumnEditing("CommandColumns_ShowEditButton", ColumnsOwner.CommandColumn_CheckEditButton, "ShowEditButton", ColumnsOwner.CommandColumn_ShowEditButtonChecked);
			AddDescriptionCheckBoxCommandColumnEditing("CommandColumns_ShowDeleteButton", ColumnsOwner.CommandColumn_CheckDeleteButton, "ShowDeleteButton", ColumnsOwner.CommandColumn_ShowDeleteButtonChecked);
			AddDescriptionAction("EditForm_UseFormLayout", () => { NavigateToEditForm(GridViewCommonFormDesigner.EditFormLayoutItems_NavBarItemCaption); }, false, null);
		}
		void AddDescriptionCheckBoxCommandColumnEditing(string name, Action action, string gotoValue, Func<bool> getCheckBoxValue) { 
			AddDescriptionAction(name, action, "CommandColumns_Editing", gotoValue, true, getCheckBoxValue);
		}
	}
	public abstract class GridViewFeatureDescriptionsOwner : DescriptionActions {
		GridViewColumnPropertiesOwner columnsOwner;
		public GridViewFeatureDescriptionsOwner(ASPxGridView gridView) 
			: base(gridView) {
		}
		protected ASPxGridView GridView { get { return (ASPxGridView)WebControl; } }
		protected GridViewColumnPropertiesOwner ColumnsOwner {
			get {
				if(columnsOwner == null)
					columnsOwner = new GridViewColumnPropertiesOwner(GridView, GridView.Columns, null);
				return columnsOwner;
			}
		}
	}
}
