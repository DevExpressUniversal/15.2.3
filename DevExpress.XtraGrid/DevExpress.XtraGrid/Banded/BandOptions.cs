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
namespace DevExpress.XtraGrid.Views.BandedGrid {
	public class BandedGridOptionsPrint : GridOptionsPrint {
		bool printBandHeader;
		public BandedGridOptionsPrint() {
			this.printBandHeader = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsPrintPrintBandHeader"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool PrintBandHeader {
			get { return printBandHeader; }
			set {
				if(PrintBandHeader == value) return;
				bool prevValue = PrintBandHeader;
				printBandHeader = value;
				OnChanged(new BaseOptionChangedEventArgs("PrintBandHeader", prevValue, PrintBandHeader));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				BandedGridOptionsPrint opt = options as BandedGridOptionsPrint;
				if(opt == null) return;
				this.printBandHeader = opt.printBandHeader;
	}
			finally {
				EndUpdate();
			}
		}
	}
	public class BandedGridOptionsView : GridOptionsView {
		bool showBands;
		public BandedGridOptionsView() {
			this.showBands = true;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), 
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsViewShowGroupedColumns"),
#endif
 DefaultValue(true), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public override bool ShowGroupedColumns {
			get { return true; }
			set { }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsViewShowBands"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowBands {
			get { return showBands; }
			set {
				if(ShowBands == value) return;
				bool prevValue = ShowBands;
				showBands = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowBands", prevValue, ShowBands));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				BandedGridOptionsView opt = options as BandedGridOptionsView;
				if(opt == null) return;
				this.showBands = opt.ShowBands;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class BandedGridOptionsCustomization : GridOptionsCustomization {
		bool allowChangeBandParent, allowChangeColumnParent, showBandsInCustomizationForm, allowBandResizing, allowBandMoving;
		public BandedGridOptionsCustomization() {
			this.allowChangeBandParent = false;
			this.allowChangeColumnParent = false;
			this.showBandsInCustomizationForm = true;
			this.allowBandResizing = true;
			this.allowBandMoving = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsCustomizationAllowBandResizing"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowBandResizing {
			get { return allowBandResizing; }
			set {
				if(AllowBandResizing == value) return;
				bool prevValue = AllowBandResizing;
				allowBandResizing = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowBandResizing", prevValue, AllowBandResizing));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsCustomizationAllowBandMoving"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowBandMoving {
			get { return allowBandMoving; }
			set {
				if(AllowBandMoving == value) return;
				bool prevValue = AllowBandMoving;
				allowBandMoving = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowBandMoving", prevValue, AllowBandMoving));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsCustomizationAllowChangeBandParent"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowChangeBandParent {
			get { return allowChangeBandParent; }
			set {
				if(AllowChangeBandParent == value) return;
				bool prevValue = AllowChangeBandParent;
				allowChangeBandParent = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowChangeBandParent", prevValue, AllowChangeBandParent));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsCustomizationAllowChangeColumnParent"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool AllowChangeColumnParent {
			get { return allowChangeColumnParent; }
			set {
				if(AllowChangeColumnParent == value) return;
				bool prevValue = AllowChangeColumnParent;
				allowChangeColumnParent = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowChangeColumnParent", prevValue, AllowChangeColumnParent));
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsCustomizationShowBandsInCustomizationForm"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowBandsInCustomizationForm {
			get { return showBandsInCustomizationForm; }
			set {
				if(ShowBandsInCustomizationForm == value) return;
				bool prevValue = ShowBandsInCustomizationForm;
				showBandsInCustomizationForm = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowBandsInCustomizationForm", prevValue, ShowBandsInCustomizationForm));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				BandedGridOptionsCustomization opt = options as BandedGridOptionsCustomization;
				if(opt == null) return;
				this.allowBandMoving = opt.AllowBandMoving;
				this.allowBandResizing = opt.AllowBandResizing;
				this.allowChangeBandParent = opt.AllowChangeBandParent;
				this.allowChangeColumnParent = opt.AllowChangeColumnParent;
				this.showBandsInCustomizationForm = opt.ShowBandsInCustomizationForm;
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class BandedGridOptionsHint : GridOptionsHint {
		bool showBandHeaderHints;
		public BandedGridOptionsHint() {
			this.showBandHeaderHints = true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("BandedGridOptionsHintShowBandHeaderHints"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool ShowBandHeaderHints {
			get { return showBandHeaderHints; }
			set {
				if(ShowBandHeaderHints == value) return;
				bool prevValue = ShowBandHeaderHints;
				showBandHeaderHints = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowBandHeaderHints", prevValue, ShowBandHeaderHints));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				BandedGridOptionsHint opt = options as BandedGridOptionsHint;
				if(opt == null) return;
				this.showBandHeaderHints = opt.ShowBandHeaderHints;
			}
			finally {
				EndUpdate();
			}
		}
	}
}
