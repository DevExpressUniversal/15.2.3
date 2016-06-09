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
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls), 
	XRDesigner("DevExpress.XtraReports.Design.XRZipCodeDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRZipCodeDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultBindableProperty("Text"),
	DefaultProperty("Text"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRZipCode.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRZipCode", "ZipCode"),
	XRToolboxSubcategoryAttribute(1, 3),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRZipCode.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRZipCode.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRZipCode : XRFieldEmbeddableControl {
		const int defaultSegmentWidth = 4;
		int segmentWidth = defaultSegmentWidth;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRZipCodeSegmentWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRZipCode.SegmentWidth"),
		DefaultValue(defaultSegmentWidth),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public int SegmentWidth {
			get { return segmentWidth; }
			set { segmentWidth = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRZipCodeText"),
#endif
		DefaultValue("0"),
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		public XRZipCode()
			: base() {
			this.Text = "0";
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeInteger("SegmentWidth", segmentWidth);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			segmentWidth = serializer.DeserializeInteger("SegmentWidth", XRConvert.Convert(defaultSegmentWidth, GraphicsDpi.Pixel, Dpi));
		}
		#endregion
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new ZipCodeBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			SetTextProperties(brick);
			((ZipCodeBrick)brick).SegmentWidth = SegmentWidth;
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			SegmentWidth = ((ZipCodeBrick)brick).SegmentWidth;
		}
	}
}
