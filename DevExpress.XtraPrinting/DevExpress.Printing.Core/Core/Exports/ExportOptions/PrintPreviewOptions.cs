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
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions"),
	]
	public class PrintPreviewOptions {
		#region fields & properties
		[Browsable(false)]
		public const string DefaultFileNameDefault = "Document";
		const bool DefaultShowOptionsBeforeExport = true;
		const ActionAfterExport DefaultActionAfterExport = ActionAfterExport.AskUser;
		bool showOptionsBeforeExport = DefaultShowOptionsBeforeExport;
		string defaultFileName = DefaultFileNameDefault;
		ActionAfterExport actionAfterExport = DefaultActionAfterExport;
		SaveMode saveMode = SaveMode.UsingSaveFileDialog;
		string defaultDirectory = string.Empty;
		PrintingSystemCommand defaultExportFormat = PrintingSystemCommand.ExportPdf;
		PrintingSystemCommand defaultSendFormat = PrintingSystemCommand.SendPdf;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsShowOptionsBeforeExport"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.ShowOptionsBeforeExport"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(DefaultShowOptionsBeforeExport),
		XtraSerializableProperty,
		]
		public bool ShowOptionsBeforeExport { get { return showOptionsBeforeExport; } set { showOptionsBeforeExport = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsDefaultFileName"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.DefaultFileName"),
		DefaultValue(DefaultFileNameDefault),
		XtraSerializableProperty,
		]
		public string DefaultFileName { get { return defaultFileName; } set { defaultFileName = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsActionAfterExport"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.ActionAfterExport"),
		DefaultValue(DefaultActionAfterExport),
		XtraSerializableProperty,
		]
		public ActionAfterExport ActionAfterExport { get { return actionAfterExport; } set { actionAfterExport = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsSaveMode"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.SaveMode"),
		DefaultValue(SaveMode.UsingSaveFileDialog),
		XtraSerializableProperty,
		]
		public SaveMode SaveMode { get { return saveMode; } set { saveMode = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsDefaultDirectory"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.DefaultDirectory"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string DefaultDirectory { get { return defaultDirectory; } set { defaultDirectory = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsDefaultExportFormat"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.DefaultExportFormat"),
		DefaultValue(PrintingSystemCommand.ExportPdf),
		XtraSerializableProperty,
		TypeConverter(typeof(PSCommandsExportTypeConverter)),
		]
		public PrintingSystemCommand DefaultExportFormat {
			get { 
				return defaultExportFormat; 
			}
			set {
				if(PSCommandHelper.IsExportCommand(value))
					defaultExportFormat = value; 
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintPreviewOptionsDefaultSendFormat"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.PrintPreviewOptions.DefaultSendFormat"),
		DefaultValue(PrintingSystemCommand.SendPdf),
		XtraSerializableProperty,
		TypeConverter(typeof(PSCommandsSendTypeConverter)),
		]
		public PrintingSystemCommand DefaultSendFormat {
			get { 
				return defaultSendFormat; 
			}
			set {
				if(PSCommandHelper.IsSendCommand(value))
					defaultSendFormat = value; 
			}
		}
		#endregion
		internal void Assign(PrintPreviewOptions source) {
			showOptionsBeforeExport = source.ShowOptionsBeforeExport;
			defaultFileName = source.DefaultFileName;
			actionAfterExport = source.ActionAfterExport;
			saveMode = source.SaveMode;
			defaultDirectory = source.DefaultDirectory;
			defaultExportFormat = source.DefaultExportFormat;
			defaultSendFormat = source.DefaultSendFormat;
		}
		internal bool ShouldSerialize() {
			return showOptionsBeforeExport != DefaultShowOptionsBeforeExport || defaultFileName != DefaultFileNameDefault ||
				actionAfterExport != DefaultActionAfterExport || saveMode != SaveMode.UsingSaveFileDialog || defaultDirectory != "" ||
				defaultExportFormat != PrintingSystemCommand.ExportPdf || defaultSendFormat != PrintingSystemCommand.SendPdf;
		}
	}
}
