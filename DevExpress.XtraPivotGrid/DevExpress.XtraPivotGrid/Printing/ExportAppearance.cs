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

using System.ComponentModel;
using System.Drawing;
using DevExpress.PivotGrid.Printing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPivotGrid.Printing {
	public class ExportAppearanceObject : AppearanceObject, IPivotPrintAppearance {
		internal static string
			optUseBorderWidth = "UseBorderWidth",
			optUseBorderStyle = "UseBorderStyle";
		float borderWidth;
		BrickBorderStyle borderStyle;
		public ExportAppearanceObject()
			: base() {
			this.borderWidth = 1.0f;
			this.borderStyle = BrickBorderStyle.Center;
		}
		public new ExportAppearanceOptions Options { get { return (ExportAppearanceOptions)base.Options; } }
		void ResetBorderWidth() { BorderWidth = 1.0f; }
		protected bool ShouldSerializeBorderWidth() { return BorderWidth != 1.0f; }
		[
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.BackColor"),
		XtraSerializableProperty(), RefreshProperties(RefreshProperties.All)
		]
		public float BorderWidth {
			get { return borderWidth; }
			set {
				if(borderWidth == value) return;
				borderWidth = value;
				if(!IsLoading) {
					try { 
						Options.BeginUpdate();
						Options.UseBorderWidth = borderWidth != 1.0;
					} finally { 
						Options.CancelUpdate(); 
					}
				}
				OnPaintChanged();
			}
		}
		void ResetBorderStyle() { BorderStyle = BrickBorderStyle.Center; }
		protected bool ShouldSerializeBorderStyle() { return BorderStyle != BrickBorderStyle.Center; }
		[
		DXDisplayName(typeof(DevExpress.Utils.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Utils.AppearanceObject.BorderStyle"),
		XtraSerializableProperty(), RefreshProperties(RefreshProperties.All)
		]
		public BrickBorderStyle BorderStyle {
			get { return borderStyle; }
			set {
				if(borderStyle == value) return;
				borderStyle = value;
				if(!IsLoading) {
					try {
						Options.BeginUpdate();
						Options.UseBorderStyle = borderStyle != BrickBorderStyle.Center;
					} finally {
						Options.CancelUpdate();
					}
				}
				OnPaintChanged();
			}
		}
		protected override AppearanceOptions CreateOptions() {
			return new ExportAppearanceOptions();
		}
		public override void Reset() {
			BeginUpdate();
			try {
				base.Reset();
				ResetBorderWidth();
			} finally {
				EndUpdate();
			}			
		}
		#region IPivotPrintAppearance
		VertAlignment IPivotPrintAppearance.TextVerticalAlignment {
			get { return TextOptions.VAlignment; }
			set { TextOptions.VAlignment = value; }
		}
		HorzAlignment IPivotPrintAppearance.TextHorizontalAlignment {
			get { return TextOptions.HAlignment; }
			set { TextOptions.HAlignment = value; }
		}
		StringFormat IPivotPrintAppearance.StringFormat {
			get { return GetStringFormat(); }
		}
		Font IPivotPrintAppearance.Font {
			get { return GetFont(); }
		}
		Color IPivotPrintAppearance.ForeColor {
			get { return GetForeColor(); }
		}
		Color IPivotPrintAppearance.BackColor {
			get { return GetBackColor(); }
		}
		Color IPivotPrintAppearance.BorderColor {
			get { return GetBorderColor(); }
		}
		IPivotPrintAppearanceOptions IPivotPrintAppearance.Options {
			get { return Options; }
		}
		#endregion
	}
	public class ExportAppearanceOptions : AppearanceOptions, IPivotPrintAppearanceOptions {
		bool useBorderWidth, useBorderStyle;
		[
		DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Printing.ExportAppearanceOptions.UseBorderWidth"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseBorderWidth {
			get { return useBorderWidth; }
			set {
				if(useBorderWidth == value) return;
				bool prevValue = useBorderWidth;
				useBorderWidth = value;
				OnChanged(ExportAppearanceObject.optUseBorderWidth, prevValue, useBorderWidth);
			}
		}
		[
		DefaultValue(false), XtraSerializableProperty(),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Printing.ExportAppearanceOptions.UseBorderStyle"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		]
		public virtual bool UseBorderStyle {
			get { return useBorderStyle; }
			set {
				if(useBorderStyle == value) return;
				bool prevValue = useBorderStyle;
				useBorderStyle = value;
				OnChanged(ExportAppearanceObject.optUseBorderStyle, prevValue, useBorderStyle);
			}
		}
		public override void Reset() {
			base.Reset();
			this.useBorderWidth = false;
			this.useBorderStyle = false;
		}
		public override void Assign(DevExpress.Utils.Controls.BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				ExportAppearanceOptions opt = options as ExportAppearanceOptions;
				if(opt == null) return;
				this.useBorderWidth = opt.UseBorderWidth;
				this.useBorderStyle = opt.UseBorderStyle;
			} finally {
				EndUpdate();
			}
		}
	}
}
