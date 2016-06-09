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
	DefaultProperty("PageInfo"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRPageInfo.bmp"),
	XRDesigner("DevExpress.XtraReports.Design.XRPageInfoDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRPageInfoDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRPageInfo", "PageInfo"),
	XRToolboxSubcategoryAttribute(3, 1),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRPageInfo.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRPageInfo.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRPageInfo : XRControl {
		#region Fields & Properties
		private int startPageNumber = 1;
		private string format = "";
		private PageInfo pageInfo = PageInfo.NumberOfTotal;
		Band runningBand;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageInfoDefaultPadding")
#else
	Description("")
#endif
		]
		public static PaddingInfo DefaultPadding {
			get { return textPadding; }
		}
		protected override PaddingInfo GetDefaultPadding(float dpi) {
			return new PaddingInfo(DefaultPadding, dpi);
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CanGrow {
			get { return base.CanGrow; }
			set { base.CanShrink = value; }
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CanShrink {
			get { return base.CanShrink; }
			set { base.CanShrink = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageInfoStartPageNumber"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPageInfo.StartPageNumber"),
		DefaultValue(1),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public int StartPageNumber {
			get { return startPageNumber; }
			set {
				if (value < 0)
					throw (new ArgumentOutOfRangeException("StartPageNumber"));
				startPageNumber = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageInfoPageInfo"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPageInfo.PageInfo"),
		DefaultValue(PageInfo.NumberOfTotal),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public PageInfo PageInfo {
			get { return pageInfo; }
			set { pageInfo = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageInfoFormat"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPageInfo.Format"),
		DefaultValue(""),
		SRCategory(ReportStringId.CatBehavior),
		Editor("DevExpress.XtraReports.Design.FormatStringEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public string Format {
			get { return format; }
			set { format = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public override bool KeepTogether { get { return true; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageInfoRunningBand"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPageInfo.RunningBand"),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(DevExpress.XtraReports.Design.RunningBandTypeConverter)),
		DefaultValue(null),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public Band RunningBand {
			get { return runningBand != null && !runningBand.IsDisposed ? runningBand : null; }
			set { runningBand = value; }
		}
		[
		Bindable(false),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
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
		#endregion
		public XRPageInfo()
			: base() {
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeEnum("PageInfo", pageInfo);
			serializer.SerializeString("Format", format);
			serializer.SerializeInteger("StartPageNumber", startPageNumber);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			pageInfo = (PageInfo)serializer.DeserializeEnum("PageInfo", typeof(PageInfo), PageInfo.NumberOfTotal);
			format = serializer.DeserializeString("Format", "");
			startPageNumber = serializer.DeserializeInteger("StartPageNumber", 1);
		}
		#endregion
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return runningBand == null ? 
				(VisualBrick)new PageInfoTextBrick(this) : 
				(VisualBrick)new GroupingPageInfoTextBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			PageInfoTextBrickBase pageInfoTextBrick = (PageInfoTextBrickBase)brick;
			pageInfoTextBrick.Format = Format;
			pageInfoTextBrick.PageInfo = PageInfo;
			pageInfoTextBrick.StartPageNumber = StartPageNumber;
			GroupingPageInfoTextBrick groupingBrick = brick as GroupingPageInfoTextBrick;
			if(groupingBrick != null) {
				System.Diagnostics.Debug.Assert(runningBand != null);
				ps.PerformIfNotNull<GroupingManager>(item => item.GroupKeys.Add(runningBand));
				groupingBrick.GroupingObject = runningBand;
			}
		}
		protected override TextEditMode TextEditMode {
			get {
				return TextEditMode.None;
			}
		}
	}
}
