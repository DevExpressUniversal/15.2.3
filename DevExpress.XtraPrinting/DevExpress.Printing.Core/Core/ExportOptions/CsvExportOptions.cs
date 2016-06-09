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
using System.Globalization;
using System.Text;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraExport;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.CsvExportOptions")]
	public class CsvExportOptions : TextExportOptionsBase {
		const bool DefaultFollowReportLayout = true;
		const bool DefaultSkipEmptyRows = true;
		const bool DefaultSkipEmptyColumns = true;
		static bool followReportLayout = DefaultFollowReportLayout;
		bool skipEmptyRows = DefaultSkipEmptyRows;
		bool skipEmptyColumns = DefaultSkipEmptyColumns;
		[Browsable(false),
		DefaultValue(DefaultFollowReportLayout),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public static bool FollowReportLayout { get { return followReportLayout; } set { followReportLayout = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CsvExportOptionsSkipEmptyRows"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.CsvExportOptions.SkipEmptyRows"),
		DefaultValue(DefaultSkipEmptyRows),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool SkipEmptyRows { get { return skipEmptyRows; } set { skipEmptyRows = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CsvExportOptionsSkipEmptyColumns"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.CsvExportOptions.SkipEmptyColumns"),
		DefaultValue(DefaultSkipEmptyColumns),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool SkipEmptyColumns { get { return skipEmptyColumns; } set { skipEmptyColumns = value; } }
		static string DefaultCsvSeparator {
			get { return CultureInfo.CurrentCulture.TextInfo.ListSeparator; }
		}
		public CsvExportOptions()
			: this(DefaultCsvSeparator, DefaultEncoding) {
		}
		public CsvExportOptions(string separator, Encoding encoding)
			: this(separator, encoding, TextExportMode.Text) {
		}
		public CsvExportOptions(string separator, Encoding encoding, TextExportMode textExportMode)
			: this(separator, encoding, textExportMode, true, true) {
		}
		public CsvExportOptions(string separator, Encoding encoding, TextExportMode textExportMode, bool skipEmptyRows, bool skipEmptyColumns)
			: base(separator, encoding, textExportMode) {
				this.skipEmptyRows = skipEmptyRows;
				this.skipEmptyColumns = skipEmptyColumns;
		}
		CsvExportOptions(CsvExportOptions source)
			: base(source) {
		}
		protected internal override ExportOptionsBase CloneOptions() {
			return new CsvExportOptions(this);
		}
		protected override string GetDefaultSeparator() {
			return DefaultCsvSeparator;
		}
		protected override bool GetDefaultQuoteStringsWithSeparators() {
			return true;
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			CsvExportOptions csvSource = (CsvExportOptions)source;
			skipEmptyColumns = csvSource.SkipEmptyColumns;
			skipEmptyRows = csvSource.SkipEmptyRows;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || skipEmptyColumns != DefaultSkipEmptyColumns || skipEmptyRows != DefaultSkipEmptyRows;
		}
	}
}
