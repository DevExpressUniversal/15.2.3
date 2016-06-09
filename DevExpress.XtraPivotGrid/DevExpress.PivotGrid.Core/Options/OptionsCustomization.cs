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
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
#else
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraPivotGrid.Customization {
	public enum CustomizationFormStyle { Simple, Excel2007 };
	public enum CustomizationFormLayout { StackedDefault, StackedSideBySide, TopPanelOnly, BottomPanelOnly2by2, BottomPanelOnly1by4 };
	[Flags]
	public enum CustomizationFormAllowedLayouts { All = 31, StackedDefault = 1, StackedSideBySide = 2, TopPanelOnly = 4, BottomPanelOnly2by2 = 8, BottomPanelOnly1by4 = 16 }
}
namespace DevExpress.XtraPivotGrid.Data {
	public enum HeaderPresenterType { FilterAreaHeaders, ColumnAreaHeaders, RowAreaHeaders, DataAreaHeaders, FieldList }
	public static class PivotEnumExtensionsBase {
		public static PivotArea ToPivotArea(HeaderPresenterType presenterType) {
			switch(presenterType) {
				case HeaderPresenterType.FilterAreaHeaders:
					return PivotArea.FilterArea;
				case HeaderPresenterType.ColumnAreaHeaders:
					return PivotArea.ColumnArea;
				case HeaderPresenterType.RowAreaHeaders:
					return PivotArea.RowArea;
				case HeaderPresenterType.DataAreaHeaders:
					return PivotArea.DataArea;
				default:
					throw new ArgumentException("presenterType");
			}
		}
		public static PivotSortOrder ToPivotSortOrder(PivotSortBySummaryOrder sortBySummaryOrder, PivotSortOrder defaultOrder) {
			switch(sortBySummaryOrder) {
				case PivotSortBySummaryOrder.Ascending:
					return PivotSortOrder.Ascending;
				case PivotSortBySummaryOrder.Descending:
					return PivotSortOrder.Descending;
				case PivotSortBySummaryOrder.Default:
					return defaultOrder;
				default:
					throw new ArgumentException("PivotSortBySummaryOrder");
			}
		}
	}
	public static class CustomizationFormEnumExtensions {
		public static PivotGridStringId GetStringId(CustomizationFormLayout layout) {
			switch(layout) {
				case CustomizationFormLayout.StackedDefault:
					return PivotGridStringId.CustomizationFormStackedDefault;
				case CustomizationFormLayout.StackedSideBySide:
					return PivotGridStringId.CustomizationFormStackedSideBySide;
				case CustomizationFormLayout.TopPanelOnly:
					return PivotGridStringId.CustomizationFormTopPanelOnly;
				case CustomizationFormLayout.BottomPanelOnly2by2:
					return PivotGridStringId.CustomizationFormBottomPanelOnly2by2;
				case CustomizationFormLayout.BottomPanelOnly1by4:
					return PivotGridStringId.CustomizationFormBottomPanelOnly1by4;
				default:
					throw new ArgumentException("FieldListLayout");
			}
		}
		public static bool IsLayoutAllowed(CustomizationFormAllowedLayouts allowedLayouts, CustomizationFormLayout layout) {
			if(allowedLayouts == CustomizationFormAllowedLayouts.All)
				return true;
			int test =  PivotGridFieldBase.Pow(2, (int)layout);
			return (test & (int)allowedLayouts) != 0;
		}
	}
}
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsCustomization : PivotGridOptionsBase {
		bool allowSort;
		bool allowFilter;
		bool allowFilterBySummary;
		bool allowDrag;
		bool allowDragInCustomizationForm;
		bool allowSortInCustomizationForm;
		bool allowFilterInCustomizationForm;
		bool allowCustomizationForm;
		bool allowExpand;
		bool allowSortBySummary;
		bool allowPrefilter;
		bool deferredUpdates;
		bool allowExpandOnDoubleClick;
		AllowHideFieldsType allowHideFields;
		CustomizationFormStyle customizationFormStyle;
		CustomizationFormLayout customizationFormLayout;
		CustomizationFormAllowedLayouts customizationFormAllowedLayouts;
		public PivotGridOptionsCustomization(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsCustomization(EventHandler optionsChanged, IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.allowSort = true;
			this.allowFilter = true;
			this.allowFilterBySummary = true;
			this.allowDrag = true;
			this.allowDragInCustomizationForm = true;
			this.allowExpand = true;
			this.allowSortBySummary = true;
			this.allowPrefilter = true;
			this.allowHideFields = AllowHideFieldsType.WhenCustomizationFormVisible;
			this.deferredUpdates = false;
			this.allowSortInCustomizationForm = false;
			this.allowFilterInCustomizationForm = false;
			this.allowCustomizationForm = true;
			this.customizationFormStyle = CustomizationFormStyle.Simple;
			this.customizationFormLayout = CustomizationFormLayout.StackedDefault;
			this.customizationFormAllowedLayouts = CustomizationFormAllowedLayouts.All;
			this.allowExpandOnDoubleClick = true;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowSort"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowSort {
			get { return allowSort; }
			set {
				if(value == AllowSort) return;
				allowSort = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowFilter"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowFilter {
			get { return allowFilter; }
			set {
				if(value == AllowFilter) return;
				allowFilter = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowFilterBySummary"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowFilterBySummary {
			get { return allowFilterBySummary; }
			set {
				if(value == AllowFilterBySummary) return;
				allowFilterBySummary = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowDrag"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowDrag {
			get { return allowDrag; }
			set {
				if(value == AllowDrag) return;
				allowDrag = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowCustomizationForm"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowCustomizationForm {
			get { return allowCustomizationForm; }
			set {
				if(value == AllowCustomizationForm) return;
				allowCustomizationForm = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowSortInCustomizationForm"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowSortInCustomizationForm {
			get { return allowSortInCustomizationForm; }
			set {
				if(value == AllowSortInCustomizationForm) return;
				allowSortInCustomizationForm = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowFilterInCustomizationForm"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowFilterInCustomizationForm {
			get { return allowFilterInCustomizationForm; }
			set {
				if(value == AllowFilterInCustomizationForm) return;
				allowFilterInCustomizationForm = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowDragInCustomizationForm"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowDragInCustomizationForm {
			get { return allowDragInCustomizationForm; }
			set {
				if(value == AllowDragInCustomizationForm) return;
				allowDragInCustomizationForm = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowExpand"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowExpand {
			get { return allowExpand; }
			set {
				if(value == AllowExpand) return;
				allowExpand = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowSortBySummary"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowSortBySummary {
			get { return allowSortBySummary; }
			set {
				if(value == AllowSortBySummary) return;
				allowSortBySummary = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowHideFields"),
#endif
		DefaultValue(AllowHideFieldsType.WhenCustomizationFormVisible), XtraSerializableProperty(),
		NotifyParentProperty(true)
		]
		public AllowHideFieldsType AllowHideFields {
			get { return allowHideFields; }
			set {
				if(value == AllowHideFields) return;
				allowHideFields = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowPrefilter"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowPrefilter {
			get { return allowPrefilter; }
			set {
				if(value == AllowPrefilter) return;
				allowPrefilter = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationCustomizationFormStyle"),
#endif
		DefaultValue(CustomizationFormStyle.Simple), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public CustomizationFormStyle CustomizationFormStyle {
			get { return customizationFormStyle; }
			set {
				if(value == customizationFormStyle) return;
				customizationFormStyle = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationCustomizationFormLayout"),
#endif
		DefaultValue(CustomizationFormLayout.StackedDefault), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public CustomizationFormLayout CustomizationFormLayout {
			get { return customizationFormLayout; }
			set {
				if(value == customizationFormLayout)
					return;
				if(!CustomizationFormEnumExtensions.IsLayoutAllowed(CustomizationFormAllowedLayouts, value))
					return;
				customizationFormLayout = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationCustomizationFormAllowedLayouts"),
#endif
#if !SL && !DXPORTABLE
		Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		DefaultValue(CustomizationFormAllowedLayouts.All), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public CustomizationFormAllowedLayouts CustomizationFormAllowedLayouts {
			get { return customizationFormAllowedLayouts; }
			set {
				if(value == customizationFormAllowedLayouts) return;
				customizationFormAllowedLayouts = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationDeferredUpdates"),
#endif
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool DeferredUpdates {
			get { return deferredUpdates; }
			set {
				if(value == deferredUpdates) return;
				deferredUpdates = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsCustomizationAllowExpandOnDoubleClick"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowExpandOnDoubleClick {
			get { return allowExpandOnDoubleClick; }
			set {
				if(value == allowExpandOnDoubleClick) return;
				allowExpandOnDoubleClick = value;
				OnOptionsChanged();
			}
		}
	}
}
