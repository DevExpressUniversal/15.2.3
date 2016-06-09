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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using System.IO;
using System.Reflection;
using System.Net;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using System.Collections.Specialized;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls), 
	XRDesigner("DevExpress.XtraReports.Design.XRCheckBoxDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRCheckBoxDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultBindableProperty("CheckState"),
	DefaultProperty("Checked"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRCheckBox.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRCheckBox", "CheckBox"),
	XRToolboxSubcategoryAttribute(0, 1),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRCheckBox.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRCheckBox.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRCheckBox : XRFieldEmbeddableControl {
		private CheckState checkState = CheckState.Unchecked;
		private HorzAlignment glyphAlignment = HorzAlignment.Near;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCheckBoxChecked"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCheckBox.Checked"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.All),
		DefaultValue(false),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool Checked {
			get {
				return !(checkState == System.Windows.Forms.CheckState.Unchecked);
			}
			set {
				if(value != Checked) {
					checkState = (value) ? System.Windows.Forms.CheckState.Checked :
						System.Windows.Forms.CheckState.Unchecked;
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCheckBoxCheckState"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCheckBox.CheckState"),
		TypeConverter(typeof(DevExpress.Utils.Design.CheckStateTypeConverter)),
		SRCategory(ReportStringId.CatData),
		DefaultValue(System.Windows.Forms.CheckState.Unchecked),
		RefreshProperties(RefreshProperties.All),
		Bindable(true),
		XtraSerializableProperty,
		]
		public CheckState CheckState {
			get { return checkState; }
			set { if(checkState != value)  checkState = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCheckBoxTextAlignment"),
#endif
		DefaultValue(TextAlignment.MiddleLeft),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCheckBox.GlyphAlignment"),
		TypeConverter(typeof(DevExpress.XtraReports.Design.GlyphAlignmentConverter)),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(HorzAlignment.Near),
		XtraSerializableProperty
		]
		public HorzAlignment GlyphAlignment {
			get { return glyphAlignment; }
			set { glyphAlignment = value; }
		}
		[
 		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override StringTrimming TextTrimming {
			get { return base.TextTrimming; }
			set { base.TextTrimming = value; }
		}
		public XRCheckBox()
			: base() {
			base.TextAlignment = TextAlignment.MiddleLeft;
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeEnum("CheckState", checkState);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			checkState = (CheckState)serializer.DeserializeEnum("CheckState", typeof(CheckState), CheckState.Unchecked);
		}
		#endregion
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if(GlyphAlignment == HorzAlignment.Center)
				return new CheckBoxBrick(this);
			return new CheckBoxTextBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			SetTextProperties(brick);
			((ICheckBoxBrick)brick).CheckState = CheckState;
			CheckBoxTextBrick checkBoxTextBrick = brick as CheckBoxTextBrick;
			if(checkBoxTextBrick != null)
				checkBoxTextBrick.CheckBoxAlignment = GlyphAlignment == HorzAlignment.Default ? HorzAlignment.Near : GlyphAlignment;
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			CheckState = ((ICheckBoxBrick)brick).CheckState;
		}
		internal override void SetDataBindingWhenDroppedFromTheFieldList(XRBinding binding) {
			base.SetDataBindingWhenDroppedFromTheFieldList(binding);
			this.Text = binding.DisplayColumnName;
		}
		protected override void ValidateBrick(VisualBrick brick, RectangleF bounds, PointF offset) {
			if(DesignMode) {
				SetDesignerBrickText(brick);
			}
		}
	}
}
