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

using DevExpress.Utils.Serializing;
using System;
using System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsFilterPopup : PivotGridOptionsFilterBase {
		bool allowMultiSelect;
		bool allowIncrementalSearch;
		bool allowContextMenu;
		bool isRadioMode;
		bool showToolbar;
		FilterPopupToolbarButtons toolbarButtons;
		const FilterPopupToolbarButtons DefaultToolbarButtons = FilterPopupToolbarButtons.ShowOnlyAvailableItems | FilterPopupToolbarButtons.RadioMode |
			FilterPopupToolbarButtons.InvertFilter;
		public PivotGridOptionsFilterPopup(PivotOptionsFilterEventHandler optionsChanged)
			: base(optionsChanged) {
			this.allowMultiSelect = true;
			this.allowIncrementalSearch = true;
			this.allowContextMenu = true;
			this.isRadioMode = false;
			this.showToolbar = true;
			this.toolbarButtons = DefaultToolbarButtons;
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsFilterPopupAllowMultiSelect"),
#endif
		DefaultValue(true), XtraSerializableProperty()
		]
		public bool AllowMultiSelect {
			get { return allowMultiSelect; }
			set {
				if(allowMultiSelect == value) return;
				allowMultiSelect = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsFilterPopupAllowIncrementalSearch"),
#endif
		DefaultValue(true), XtraSerializableProperty()
		]
		public bool AllowIncrementalSearch {
			get { return allowIncrementalSearch; }
			set {
				if(allowIncrementalSearch == value) return;
				allowIncrementalSearch = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsFilterPopupAllowContextMenu"),
#endif
		DefaultValue(true), XtraSerializableProperty()
		]
		public bool AllowContextMenu {
			get { return allowContextMenu; }
			set {
				if(allowContextMenu == value) return;
				allowContextMenu = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsFilterPopupAllowFilterTypeChanging"),
#endif
		DefaultValue(false), XtraSerializableProperty(), Obsolete("The AllowFilterTypeChanging property is obsolete. Use the PivotGridFieldBase.ShowNewValues property instead."),
		Browsable(false)
		]
		public bool AllowFilterTypeChanging { get; set; }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsFilterPopupIsRadioMode"),
#endif
		DefaultValue(false), XtraSerializableProperty()
		]
		public bool IsRadioMode {
			get { return isRadioMode; }
			set {
				if(isRadioMode == value) return;
				isRadioMode = value;
				OnOptionsChanged();
			}
		}
		[
		DefaultValue(true), XtraSerializableProperty()
		]
		public bool ShowToolbar {
			get { return showToolbar; }
			set {
				if(showToolbar == value) return;
				showToolbar = value;
				OnOptionsChanged();
			}
		}
		[
		Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(DefaultToolbarButtons), XtraSerializableProperty()
		]
		public FilterPopupToolbarButtons ToolbarButtons {
			get { return toolbarButtons; }
			set {
				if(toolbarButtons == value) return;
				toolbarButtons = value;
				OnOptionsChanged();
			}
		}
	}
}
