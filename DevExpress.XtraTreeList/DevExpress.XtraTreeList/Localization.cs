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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraTreeList.Localization {
	[ToolboxItem(false)]
	public class TreeListLocalizer : XtraLocalizer<TreeListStringId> {
		static TreeListLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<TreeListStringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<TreeListStringId> Active { 
			get { return XtraLocalizer<TreeListStringId>.Active; }
			set { XtraLocalizer<TreeListStringId>.Active = value; }
		}
		public override XtraLocalizer<TreeListStringId> CreateResXLocalizer() {
			return new TreeListResLocalizer();
		}
		public static XtraLocalizer<TreeListStringId> CreateDefaultLocalizer() { return new TreeListResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(TreeListStringId.MenuFooterSum, "Sum");
			AddString(TreeListStringId.MenuFooterMin, "Min");
			AddString(TreeListStringId.MenuFooterMax, "Max");
			AddString(TreeListStringId.MenuFooterCount, "Count");
			AddString(TreeListStringId.MenuFooterAverage, "Average");
			AddString(TreeListStringId.MenuFooterNone, "None");
			AddString(TreeListStringId.MenuFooterAllNodes, "All Nodes");
			AddString(TreeListStringId.MenuFooterSumFormat, "SUM={0:#.##}");
			AddString(TreeListStringId.MenuFooterMinFormat, "MIN={0}");
			AddString(TreeListStringId.MenuFooterMaxFormat, "MAX={0}");
			AddString(TreeListStringId.MenuFooterCountFormat, "{0}");
			AddString(TreeListStringId.MenuFooterAverageFormat, "AVG={0:#.##}");
			AddString(TreeListStringId.MenuColumnSortAscending, "Sort Ascending");
			AddString(TreeListStringId.MenuColumnSortDescending, "Sort Descending");
			AddString(TreeListStringId.MenuColumnClearSorting, "Clear Sorting");
			AddString(TreeListStringId.MenuColumnColumnCustomization, "Column Chooser");
			AddString(TreeListStringId.MenuColumnBandCustomization, "Column/Band Chooser");
			AddString(TreeListStringId.MenuColumnBestFit, "Best Fit");
			AddString(TreeListStringId.MenuColumnBestFitAllColumns, "Best Fit (all columns)");
			AddString(TreeListStringId.ColumnCustomizationText, "Customization");
			AddString(TreeListStringId.ColumnNamePrefix, "col");
			AddString(TreeListStringId.PrintDesignerHeader, "Print Settings");
			AddString(TreeListStringId.PrintDesignerDescription, "Set up various printing options for the current treelist.");
			AddString(TreeListStringId.InvalidNodeExceptionText, " Do you want to correct the value ?");
			AddString(TreeListStringId.MultiSelectMethodNotSupported, "Specified method will not work when OptionsBehavior.MultiSelect is inactive.");
			AddString(TreeListStringId.CustomizationFormColumnHint, "Drag and drop columns here to customize layout");
			AddString(TreeListStringId.CustomizationFormBandHint, "Drag and drop bands here to customize layout");
			AddString(TreeListStringId.CustomizationColumns, "Columns");
			AddString(TreeListStringId.CustomizationBands, "Bands");
			AddString(TreeListStringId.FilterPanelCustomizeButton, "Edit Filter");
			AddString(TreeListStringId.WindowErrorCaption, "Error");
			AddString(TreeListStringId.FilterEditorOkButton, "&OK");
			AddString(TreeListStringId.FilterEditorCancelButton, "&Cancel");
			AddString(TreeListStringId.FilterEditorApplyButton, "&Apply");
			AddString(TreeListStringId.FilterEditorCaption, "Filter Editor");
			AddString(TreeListStringId.MenuColumnAutoFilterRowHide, "Hide Auto Filter Row");
			AddString(TreeListStringId.MenuColumnAutoFilterRowShow, "Show Auto Filter Row");
			AddString(TreeListStringId.MenuColumnFilterEditor, "Filter Editor...");
			AddString(TreeListStringId.MenuColumnClearFilter, "Clear Filter");
			AddString(TreeListStringId.PopupFilterAll, "(All)");
			AddString(TreeListStringId.PopupFilterBlanks, "(Blanks)");
			AddString(TreeListStringId.PopupFilterNonBlanks, "(Non blanks)");
			AddString(TreeListStringId.MenuColumnFindFilterHide, "Hide Find Panel");
			AddString(TreeListStringId.MenuColumnFindFilterShow, "Show Find Panel");
			AddString(TreeListStringId.FindControlFindButton, "Find");
			AddString(TreeListStringId.FindControlClearButton, "Clear");
			AddString(TreeListStringId.MenuColumnExpressionEditor, "Expression Editor...");
			AddString(TreeListStringId.FindNullPrompt, "Enter text to search...");
			AddString(TreeListStringId.MenuColumnConditionalFormatting, "Conditional Formatting");
			AddString(TreeListStringId.SearchForBand, "Search for a band...");
		}
		#endregion
	}
	public class TreeListResLocalizer : TreeListLocalizer {
		ResourceManager manager = null;
		public TreeListResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraTreeList.LocalizationRes", typeof(TreeListResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(TreeListStringId id) {
			string resStr = "TreeListStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = string.Empty;
			return ret;
		}
	}
	#region enum TreeListStringId
	public enum TreeListStringId {
		MenuFooterSum,
		MenuFooterMin, 
		MenuFooterMax, 
		MenuFooterCount, 
		MenuFooterAverage,
		MenuFooterNone,
		MenuFooterAllNodes,
		MenuFooterSumFormat,
		MenuFooterMinFormat, 
		MenuFooterMaxFormat, 
		MenuFooterCountFormat, 
		MenuFooterAverageFormat,
		MenuColumnSortAscending, 
		MenuColumnSortDescending, 
		MenuColumnClearSorting,
		MenuColumnColumnCustomization, 
		MenuColumnBestFit, 
		MenuColumnBestFitAllColumns,
		ColumnCustomizationText,
		ColumnNamePrefix,
		PrintDesignerHeader,
		PrintDesignerDescription,
		InvalidNodeExceptionText,
		MultiSelectMethodNotSupported,
		CustomizationFormColumnHint,
		FilterPanelCustomizeButton,
		WindowErrorCaption,
		FilterEditorOkButton,
		FilterEditorCancelButton,
		FilterEditorApplyButton,
		FilterEditorCaption,
		MenuColumnAutoFilterRowHide,
		MenuColumnAutoFilterRowShow,
		MenuColumnFilterEditor,
		MenuColumnClearFilter,
		PopupFilterAll,
		PopupFilterBlanks,
		PopupFilterNonBlanks,
		MenuColumnFindFilterHide,
		MenuColumnFindFilterShow,
		FindControlFindButton,
		FindControlClearButton,
		MenuColumnExpressionEditor,
		FindNullPrompt,
		CustomizationFormBandHint,
		CustomizationColumns,
		CustomizationBands,
		MenuColumnBandCustomization,
		MenuColumnConditionalFormatting,
		SearchForBand
	}
	#endregion
}
