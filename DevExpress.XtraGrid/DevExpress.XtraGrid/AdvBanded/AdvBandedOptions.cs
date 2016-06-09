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
using System.Collections;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Views.BandedGrid {
	public class AdvBandedGridOptionsView : BandedGridOptionsView {
		public AdvBandedGridOptionsView() { 
			this.ColumnAutoWidth = false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override bool AllowCellMerge {
			get { return false; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override bool RowAutoHeight {
			get { return false; }
			set {
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("AdvBandedGridOptionsViewColumnAutoWidth"),
#endif
 DefaultValue(false),]
		public override bool ColumnAutoWidth {
			get { return base.ColumnAutoWidth; }
			set { base.ColumnAutoWidth = value; }
		}
	}
	public class AdvBandedGridOptionsSelection : GridOptionsSelection {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override GridMultiSelectMode MultiSelectMode {
			get { return GridMultiSelectMode.RowSelect; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean ShowCheckBoxSelectorInColumnHeader {
			get { return base.ShowCheckBoxSelectorInColumnHeader; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean ShowCheckBoxSelectorInGroupRow {
			get { return base.ShowCheckBoxSelectorInGroupRow; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean ShowCheckBoxSelectorChangesSelectionNavigation {
			get { return base.ShowCheckBoxSelectorChangesSelectionNavigation; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override DefaultBoolean ShowCheckBoxSelectorInPrintExport {
			get { return base.ShowCheckBoxSelectorInPrintExport; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override int CheckBoxSelectorColumnWidth {
			get { return base.CheckBoxSelectorColumnWidth; }
			set { }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool ResetSelectionClickOutsideCheckboxSelector {
			get { return base.ResetSelectionClickOutsideCheckboxSelector; }
			set { }
		}
	}
	public class AdvBandedGridOptionsNavigation : GridOptionsNavigation {
		bool useAdvHorzNavigation, useAdvVertNavigation;
		public AdvBandedGridOptionsNavigation() {
			this.useAdvHorzNavigation = this.useAdvVertNavigation = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("AdvBandedGridOptionsNavigationUseAdvHorzNavigation"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseAdvHorzNavigation {
			get { return useAdvHorzNavigation; }
			set {
				if(UseAdvHorzNavigation == value) return;
				bool prevValue = UseAdvHorzNavigation;
				useAdvHorzNavigation = value;
				OnChanged(new BaseOptionChangedEventArgs("UseAdvHorzNavigation", prevValue, UseAdvHorzNavigation));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("AdvBandedGridOptionsNavigationUseAdvVertNavigation"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool UseAdvVertNavigation {
			get { return useAdvVertNavigation; }
			set {
				if(UseAdvVertNavigation == value) return;
				bool prevValue = UseAdvVertNavigation;
				useAdvVertNavigation = value;
				OnChanged(new BaseOptionChangedEventArgs("UseAdvVertNavigation", prevValue, UseAdvVertNavigation));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				AdvBandedGridOptionsNavigation opt = options as AdvBandedGridOptionsNavigation;
				if(opt == null) return;
				this.useAdvHorzNavigation = opt.UseAdvHorzNavigation;
				this.useAdvVertNavigation = opt.UseAdvVertNavigation;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
