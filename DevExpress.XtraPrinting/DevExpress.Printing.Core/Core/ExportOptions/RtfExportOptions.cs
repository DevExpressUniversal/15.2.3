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
using System.Globalization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.RtfExportOptions")]
	public class RtfExportOptions : PageByPageExportOptionsBase {
		const RtfExportMode DefaultExportMode = RtfExportMode.SingleFilePageByPage;
		const bool DefaultExportWatermarks = true;
		const bool DefaultExportPageBreaks = true;
		const bool DefaultEmptyFirstPageHeaderFooter = false;
		RtfExportMode exportMode = DefaultExportMode;
		bool exportWatermarks = DefaultExportWatermarks;
		bool exportPageBreaks = DefaultExportPageBreaks;
		bool emptyFirstPageHeaderFooter = DefaultEmptyFirstPageHeaderFooter;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("RtfExportOptionsExportMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.RtfExportOptions.ExportMode"),
		DefaultValue(DefaultExportMode),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty]
		public RtfExportMode ExportMode { get { return exportMode; } set { exportMode = value; } }
		protected internal override bool IsMultiplePaged {
			get { return ExportMode == RtfExportMode.SingleFilePageByPage; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("RtfExportOptionsExportWatermarks"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.RtfExportOptions.ExportWatermarks"),
		DefaultValue(DefaultExportWatermarks),
		TypeConverter(typeof(RtfExportWatermarksConverter)),
		XtraSerializableProperty]
		public bool ExportWatermarks { get { return exportWatermarks; } set { exportWatermarks = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.RtfExportOptions.ExportPageBreaks"),
		DefaultValue(DefaultExportPageBreaks),
		TypeConverter(typeof(RtfSingleFileOptionConverter)),
		XtraSerializableProperty]
		public bool ExportPageBreaks { get { return exportPageBreaks; } set { exportPageBreaks = value; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.RtfExportOptions.EmptyFirstPageHeaderFooter"),
		DefaultValue(DefaultEmptyFirstPageHeaderFooter),
		TypeConverter(typeof(RtfSingleFileOptionConverter)),
		XtraSerializableProperty]
		public bool EmptyFirstPageHeaderFooter { get { return emptyFirstPageHeaderFooter; } set { emptyFirstPageHeaderFooter = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("RtfExportOptionsPageRange"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.RtfExportOptions.PageRange"),
		TypeConverter(typeof(RtfPageRangeConverter)),
		XtraSerializableProperty]
		public override string PageRange { get { return base.PageRange; } set { base.PageRange = value; } }
		public RtfExportOptions() {
		}
		RtfExportOptions(RtfExportOptions source)
			: base(source) {
		}
		protected internal override ExportOptionsBase CloneOptions() {
			return new RtfExportOptions(this);
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			RtfExportOptions rtfSource = (RtfExportOptions)source;
			exportMode = rtfSource.ExportMode;
			exportWatermarks = rtfSource.ExportWatermarks;
			exportPageBreaks = rtfSource.ExportPageBreaks;
			emptyFirstPageHeaderFooter = rtfSource.EmptyFirstPageHeaderFooter;
		}
		protected internal override bool ShouldSerialize() {
			return exportMode != DefaultExportMode || exportWatermarks != DefaultExportWatermarks || 
				exportPageBreaks != DefaultExportPageBreaks || emptyFirstPageHeaderFooter != DefaultEmptyFirstPageHeaderFooter ||
				base.ShouldSerialize();
		}
	}
}
