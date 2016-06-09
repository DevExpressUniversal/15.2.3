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
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Snap.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	#region SNBarCodeFieldActionListBase
	public abstract class SNBarCodeFieldActionListBase : FieldActionList<SNBarCodeField> {
		protected SNBarCodeFieldActionListBase(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		protected void ApplyNewBarCodeGenerator(BarCodeGeneratorBase generator) {
			FieldChanger.ApplyNewValue(controller => {
				DocumentModel model = SNBarCodeHelper.CreateBarCodeGeneratorDocumentModel(generator, (SnapDocumentModel)controller.DocumentModel);
				controller.SetSwitch(SNBarCodeField.BarCodeGeneratorSwitch, model, false);
			});
		}
	}
	#endregion
	#region SNBarCodeFieldActionList
	public class SNBarCodeFieldActionList : SNBarCodeFieldActionListBase {
		public SNBarCodeFieldActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_Symbology")]
		public BarCodeSymbology Symbology {
			get { return ParsedInfo.Symbology; }
			set {
				if (Symbology != value) {
					ApplyNewBarCodeGenerator(SNBarCodeHelper.CreateBarCodeGenerator(value));
					SNSmartTagService smartTagService = (SNSmartTagService)this.Component.Site.GetService(typeof(SNSmartTagService));
					smartTagService.UpdatePopup();
				}
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_Data")]
		public string Data {
			get { return ParsedInfo.Data; }
			set {
				if (Data != value) {
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeDataSwitch, newMode), value);
				}
			}
		}
		[
		ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_Module"),
		CustomLineController(typeof(SizePropertyLineController))
		]
		public int Module {
			get { return ParsedInfo.Module; }
			set {
				if (Module != value)
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeModuleSwitch, Convert.ToString(newMode, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_AutoModule")]
		public bool AutoModule {
			get { return ParsedInfo.AutoModule; }
			set {
				if (AutoModule != value) {
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeAutoModuleSwitch, Convert.ToString(newMode)), value);
				}
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_ShowData")]
		public bool ShowText {
			get { return ParsedInfo.ShowText; }
			set {
				if (ShowText != value) {
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeShowTextSwitch, Convert.ToString(newMode)), value);
				}
			}
		}
		[
		ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_Alignment"),
		Editor("DevExpress.XtraReports.Design.TextAlignmentEditor," + AssemblyInfo.SRAssemblyUtilsUI, typeof(UITypeEditor))
		]
		public TextAlignment Alignment {
			get { return ParsedInfo.Alignment; }
			set {
				if (Alignment != value) {
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeAlignmentSwitch, Enum.GetName(typeof(TextAlignment), newMode)), value);
				}
			}
		}
		[
		ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_TextAlignment"),
		Editor("DevExpress.XtraReports.Design.TextAlignmentEditor," + AssemblyInfo.SRAssemblyUtilsUI, typeof(UITypeEditor))
		]
		public TextAlignment TextAlignment {
			get { return ParsedInfo.TextAlignment; }
			set {
				if (TextAlignment != value) {
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeTextAlignmentSwitch, Enum.GetName(typeof(TextAlignment), newMode)), value);
				}
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.BarCodeSmartTagItem_Orientation")]
		public BarCodeOrientation Orientation {
			get { return ParsedInfo.Orientation; }
			set {
				if (Orientation != value) {
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNBarCodeField.BarCodeOrientationSwitch, Enum.GetName(typeof(BarCodeOrientation), newMode)), value);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Symbology", "Symbology");
			AddPropertyItem(actionItems, "Data", "Data");
			AddPropertyItem(actionItems, "Alignment", "Alignment");
			AddPropertyItem(actionItems, "TextAlignment", "TextAlignment");
			AddPropertyItem(actionItems, "Orientation", "Orientation");
			AddPropertyItem(actionItems, "Module", "Module");
			AddPropertyItem(actionItems, "ShowText", "ShowText");
			AddPropertyItem(actionItems, "AutoModule", "AutoModule");
		}
	}
	#endregion
	#region SNBarCodeCheckSumActionListBase
	public abstract class SNBarCodeCheckSumActionListBase : SNBarCodeFieldActionListBase {
		protected SNBarCodeCheckSumActionListBase(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		BarCodeGeneratorBase Generator { get { return ParsedInfo.BarCodeGenerator; } }
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.BarCodeGeneratorBase.CalcCheckSum")]
		public bool CalcCheckSum {
			get { return Generator.CalcCheckSum; }
			set {
				if (CalcCheckSum != value) {
					Generator.CalcCheckSum = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "CalcCheckSum", "CalcCheckSum");
		}
	}
	#endregion
	#region SNBarCodeCodaBarActionList
	public class SNBarCodeCodaBarActionList : SNBarCodeFieldActionListBase {
		public SNBarCodeCodaBarActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		CodabarGenerator Generator {
			get { return (CodabarGenerator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.CodabarGenerator.StartStopPair")]
		public CodabarStartStopPair StartStopPair {
			get { return Generator.StartStopPair; }
			set {
				if (StartStopPair != value) {
					Generator.StartStopPair = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.CodabarGenerator.WideNarrowRatio")]
		public float WideNarrowRatio {
			get { return Generator.WideNarrowRatio; }
			set {
				if (WideNarrowRatio != value) {
					Generator.WideNarrowRatio = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "StartStopPair", "StartStopPair");
			AddPropertyItem(actionItems, "WideNarrowRatio", "WideNarrowRatio");
		}
	}
	#endregion
	#region SNBarCode11ActionList
	public class SNBarCode11ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCode11ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Code11Generator Generator { get { return (Code11Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeCode39ActionList
	public class SNBarCodeCode39ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeCode39ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Code39Generator Generator { get { return (Code39Generator)ParsedInfo.BarCodeGenerator; } }
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.Code39Generator.WideNarrowRatio")]
		public float WideNarrowRatio {
			get { return Generator.WideNarrowRatio; }
			set {
				if (WideNarrowRatio != value) {
					Generator.WideNarrowRatio = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "WideNarrowRatio", "WideNarrowRatio");
		}
	}
	#endregion
	#region SNBarCodeCode39ExtendedActionList
	public class SNBarCodeCode39ExtendedActionList : SNBarCodeCode39ActionList {
		public SNBarCodeCode39ExtendedActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Code39ExtendedGenerator Generator { get { return (Code39ExtendedGenerator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeCode93ActionList
	public class SNBarCodeCode93ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeCode93ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Code93Generator Generator { get { return (Code93Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeCode93ExtendedActionList
	public class SNBarCodeCode93ExtendedActionList : SNBarCodeCode93ActionList {
		public SNBarCodeCode93ExtendedActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Code93ExtendedGenerator Generator { get { return (Code93ExtendedGenerator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeCode128ActionList
	public class SNBarCodeCode128ActionList : SNBarCodeFieldActionListBase {
		public SNBarCodeCode128ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Code128Generator Generator {
			get { return (Code128Generator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.Code128Generator.CharacterSet")]
		public Code128Charset CharacterSet {
			get { return Generator.CharacterSet; }
			set {
				if (CharacterSet != value) {
					Generator.CharacterSet = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "CharacterSet", "CharacterSet");
		}
	}
	#endregion
	#region SNBarCodeCodeMSIActionList
	public class SNBarCodeCodeMSIActionList : SNBarCodeFieldActionListBase {
		public SNBarCodeCodeMSIActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		CodeMSIGenerator Generator {
			get { return (CodeMSIGenerator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.CodeMSIGenerator.MSICheckSum")]
		public MSICheckSum MSICheckSum {
			get { return Generator.MSICheckSum; }
			set {
				if (MSICheckSum != value) {
					Generator.MSICheckSum = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "MSICheckSum", "MSICheckSum");
		}
	}
	#endregion
	#region SNBarCodeDataBarActionList
	public class SNBarCodeDataBarActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeDataBarActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		DataBarGenerator Generator { get { return (DataBarGenerator)ParsedInfo.BarCodeGenerator; } }
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataBarGenerator.Type")]
		public DataBarType Type {
			get { return Generator.Type; }
			set {
				if (Type != value) {
					Generator.Type = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataBarGenerator.SegmentsInRow")]
		public int SegmentsInRow {
			get { return Generator.SegmentsInRow; }
			set {
				if (SegmentsInRow != value) {
					Generator.SegmentsInRow = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataBarGenerator.FNC1Substitute")]
		public string FNC1Substitute {
			get { return Generator.FNC1Substitute; }
			set {
				if (FNC1Substitute != value) {
					Generator.FNC1Substitute = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Type", "Type");
			AddPropertyItem(actionItems, "SegmentsInRow", "SegmentsInRow");
			AddPropertyItem(actionItems, "FNC1Substitute", "FNC1Substitute");
		} 
	}
	#endregion
	#region SNBarCodeDataMatrixActionList
	public class SNBarCodeDataMatrixActionList : SNBarCodeFieldActionListBase {
		public SNBarCodeDataMatrixActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		DataMatrixGenerator Generator { get { return (DataMatrixGenerator)ParsedInfo.BarCodeGenerator; } }
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataMatrixGenerator.CompactionMode")]
		public DataMatrixCompactionMode CompactionMode {
			get { return Generator.CompactionMode; }
			set {
				if (CompactionMode != value) {
					Generator.CompactionMode = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataMatrixGenerator.MatrixSize")]
		public DataMatrixSize MatrixSize {
			get { return Generator.MatrixSize; }
			set {
				if (MatrixSize != value) {
					Generator.MatrixSize = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "CompactionMode", "CompactionMode");
			AddPropertyItem(actionItems, "MatrixSize", "MatrixSize");
		}
	}
	#endregion
	#region SNBarCodeDataBarGS1ActionList
	public class SNBarCodeDataBarGS1ActionList : SNBarCodeDataMatrixActionList {
		public SNBarCodeDataBarGS1ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		DataMatrixGS1Generator Generator { get { return (DataMatrixGS1Generator)ParsedInfo.BarCodeGenerator; } }
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataMatrixGS1Generator.FNC1Substitute")]
		public string FNC1Substitute {
			get { return Generator.FNC1Substitute; }
			set {
				if (FNC1Substitute != value) {
					Generator.FNC1Substitute = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.DataMatrixGenerator.MatrixSize")]
		public bool HumanReadableText {
			get { return Generator.HumanReadableText; }
			set {
				if (HumanReadableText != value) {
					Generator.HumanReadableText = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "FNC1Substitute", "FNC1Substitute");
			AddPropertyItem(actionItems, "HumanReadableText", "HumanReadableText");
		}
	}
	#endregion
	#region SNBarCodeEAN8ActionList
	public class SNBarCodeEAN8ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeEAN8ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		EAN8Generator Generator { get { return (EAN8Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeEAN13ActionList
	public class SNBarCodeEAN13ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeEAN13ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		EAN13Generator Generator { get { return (EAN13Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeEAN128ActionList
	public class SNBarCodeEAN128ActionList : SNBarCodeCode128ActionList {
		public SNBarCodeEAN128ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		EAN128Generator Generator {
			get { return (EAN128Generator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.EAN128Generator.FNC1Substitute")]
		public string FNC1Substitute {
			get { return Generator.FNC1Substitute; }
			set {
				if (FNC1Substitute != value) {
					Generator.FNC1Substitute = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.EAN128Generator.HumanReadableText")]
		public bool HumanReadableText {
			get { return Generator.HumanReadableText; }
			set {
				if (HumanReadableText != value) {
					Generator.HumanReadableText = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "FNC1Substitute", "FNC1Substitute");
			AddPropertyItem(actionItems, "HumanReadableText", "HumanReadableText");
		}
	}
	#endregion
	#region SNBarCodeIndustrial2of5ActionList
	public class SNBarCodeIndustrial2of5ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeIndustrial2of5ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Industrial2of5Generator Generator {
			get { return (Industrial2of5Generator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.Industrial2of5Generator.WideNarrowRatio")]
		public float WideNarrowRatio {
			get { return Generator.WideNarrowRatio; }
			set {
				if (WideNarrowRatio != value) {
					Generator.WideNarrowRatio = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "WideNarrowRatio", "WideNarrowRatio");
		}
	}
	#endregion
	#region SNBarCodeIntelligentMailActionList
	public class SNBarCodeIntelligentMailActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeIntelligentMailActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		IntelligentMailGenerator Generator { get { return (IntelligentMailGenerator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeInterleaved2of5ActionList
	public class SNBarCodeInterleaved2of5ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeInterleaved2of5ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Interleaved2of5Generator Generator {
			get { return (Interleaved2of5Generator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.Interleaved2of5Generator.WideNarrowRatio")]
		public float WideNarrowRatio {
			get { return Generator.WideNarrowRatio; }
			set {
				if (WideNarrowRatio != value) {
					Generator.WideNarrowRatio = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "WideNarrowRatio", "WideNarrowRatio");
		}
	}
	#endregion
	#region SNBarCodeITF14ActionList
	public class SNBarCodeITF14ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeITF14ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		ITF14Generator Generator { get { return (ITF14Generator)ParsedInfo.BarCodeGenerator; } }
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.ITF14Generator.WideNarrowRatio")]
		public float WideNarrowRatio {
			get { return Generator.WideNarrowRatio; }
			set {
				if (WideNarrowRatio != value) {
					Generator.WideNarrowRatio = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "WideNarrowRatio", "WideNarrowRatio");
		}
	}
	#endregion
	#region SNBarCodeMatrix2of5ActionList
	public class SNBarCodeMatrix2of5ActionList : SNBarCodeIndustrial2of5ActionList {
		public SNBarCodeMatrix2of5ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		Matrix2of5Generator Generator { get { return (Matrix2of5Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodePDF417ActionList
	public class SNBarCodePDF417ActionList : SNBarCodeFieldActionListBase {
		public SNBarCodePDF417ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		PDF417Generator Generator {
			get { return (PDF417Generator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.PDF417Generator.Columns")]
		public int Columns {
			get { return Generator.Columns; }
			set {
				if (Columns != value) {
					Generator.Columns = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.PDF417Generator.Rows")]
		public int Rows {
			get { return Generator.Rows; }
			set {
				if (Rows != value) {
					Generator.Rows = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.PDF417Generator.ErrorCorrectionLevel")]
		public ErrorCorrectionLevel ErrorCorrectionLevel {
			get { return Generator.ErrorCorrectionLevel; }
			set {
				if (ErrorCorrectionLevel != value) {
					Generator.ErrorCorrectionLevel = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.PDF417Generator.TruncateSymbol")]
		public bool TruncateSymbol {
			get { return Generator.TruncateSymbol; }
			set {
				if (TruncateSymbol != value) {
					Generator.TruncateSymbol = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.PDF417Generator.YToXRatio")]
		public float YToXRatio {
			get { return Generator.YToXRatio; }
			set {
				if (YToXRatio != value) {
					Generator.YToXRatio = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.PDF417Generator.CompactionMode")]
		public PDF417CompactionMode CompactionMode {
			get { return Generator.CompactionMode; }
			set {
				if (CompactionMode != value) {
					Generator.CompactionMode = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Columns", "Columns");
			AddPropertyItem(actionItems, "Rows", "Rows");
			AddPropertyItem(actionItems, "ErrorCorrectionLevel", "ErrorCorrectionLevel");
			AddPropertyItem(actionItems, "TruncateSymbol", "TruncateSymbol");
			AddPropertyItem(actionItems, "YToXRatio", "YToXRatio");
			AddPropertyItem(actionItems, "CompactionMode", "CompactionMode");
		}
	}
	#endregion
	#region SNBarCodePostNetActionList
	public class SNBarCodePostNetActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodePostNetActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		PostNetGenerator Generator { get { return (PostNetGenerator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeQRCodeActionList
	public class SNBarCodeQRCodeActionList : SNBarCodeFieldActionListBase {
		public SNBarCodeQRCodeActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		QRCodeGenerator Generator {
			get { return (QRCodeGenerator)ParsedInfo.BarCodeGenerator; }
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.QRCodeGenerator.CompactionMode")]
		public QRCodeCompactionMode CompactionMode {
			get { return Generator.CompactionMode; }
			set {
				if (CompactionMode != value) {
					Generator.CompactionMode = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.QRCodeGenerator.ErrorCorrectionLevel")]
		public QRCodeErrorCorrectionLevel ErrorCorrectionLevel {
			get { return Generator.ErrorCorrectionLevel; }
			set {
				if (ErrorCorrectionLevel != value) {
					Generator.ErrorCorrectionLevel = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		[ResDisplayName(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPrinting.BarCode.QRCodeGenerator.Version")]
		public QRCodeVersion Version {
			get { return Generator.Version; }
			set {
				if (Version != value) {
					Generator.Version = value;
					ApplyNewBarCodeGenerator(Generator);
				}
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "CompactionMode", "CompactionMode");
			AddPropertyItem(actionItems, "ErrorCorrectionLevel", "ErrorCorrectionLevel");
			AddPropertyItem(actionItems, "Version", "Version");
		}
	}
	#endregion
	#region SNBarCodeUPCAActionList
	public class SNBarCodeUPCAActionList : SNBarCodeEAN13ActionList {
		public SNBarCodeUPCAActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		UPCAGenerator Generator { get { return (UPCAGenerator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeUPCE0ActionList
	public class SNBarCodeUPCE0ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeUPCE0ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		UPCE0Generator Generator { get { return (UPCE0Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeUPCE1ActionList
	public class SNBarCodeUPCE1ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeUPCE1ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		UPCE1Generator Generator { get { return (UPCE1Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeUPCSupplemental2ActionList
	public class SNBarCodeUPCSupplemental2ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeUPCSupplemental2ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		UPCSupplemental2Generator Generator { get { return (UPCSupplemental2Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region SNBarCodeUPCSupplemental5ActionList
	public class SNBarCodeUPCSupplemental5ActionList : SNBarCodeCheckSumActionListBase {
		public SNBarCodeUPCSupplemental5ActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		UPCSupplemental5Generator Generator { get { return (UPCSupplemental5Generator)ParsedInfo.BarCodeGenerator; } }
	}
	#endregion
	#region BarCodeActionListDesigner
	public class BarCodeActionListDesigner : MergeFieldActionListDesigner<SNBarCodeField> {
		static Dictionary<BarCodeSymbology, Type> barCodeActionListTypes = new Dictionary<BarCodeSymbology, Type>();
		static BarCodeActionListDesigner() {
			barCodeActionListTypes[BarCodeSymbology.Codabar] = typeof(SNBarCodeCodaBarActionList);
			barCodeActionListTypes[BarCodeSymbology.Code11] = typeof(SNBarCode11ActionList);
			barCodeActionListTypes[BarCodeSymbology.Code39] = typeof(SNBarCodeCode39ActionList);
			barCodeActionListTypes[BarCodeSymbology.Code39Extended] = typeof(SNBarCodeCode39ExtendedActionList);
			barCodeActionListTypes[BarCodeSymbology.Code93] = typeof(SNBarCodeCode93ActionList);
			barCodeActionListTypes[BarCodeSymbology.Code93Extended] = typeof(SNBarCodeCode93ExtendedActionList);
			barCodeActionListTypes[BarCodeSymbology.Code128] = typeof(SNBarCodeCode128ActionList);
			barCodeActionListTypes[BarCodeSymbology.CodeMSI] = typeof(SNBarCodeCodeMSIActionList);
			barCodeActionListTypes[BarCodeSymbology.DataBar] = typeof(SNBarCodeDataBarActionList);
			barCodeActionListTypes[BarCodeSymbology.DataMatrix] = typeof(SNBarCodeDataMatrixActionList);
			barCodeActionListTypes[BarCodeSymbology.DataMatrixGS1] = typeof(SNBarCodeDataBarGS1ActionList);
			barCodeActionListTypes[BarCodeSymbology.EAN8] = typeof(SNBarCodeEAN8ActionList);
			barCodeActionListTypes[BarCodeSymbology.EAN13] = typeof(SNBarCodeEAN13ActionList);
			barCodeActionListTypes[BarCodeSymbology.EAN128] = typeof(SNBarCodeEAN128ActionList);
			barCodeActionListTypes[BarCodeSymbology.Industrial2of5] = typeof(SNBarCodeIndustrial2of5ActionList);
			barCodeActionListTypes[BarCodeSymbology.IntelligentMail] = typeof(SNBarCodeIntelligentMailActionList);
			barCodeActionListTypes[BarCodeSymbology.Interleaved2of5] = typeof(SNBarCodeInterleaved2of5ActionList);
			barCodeActionListTypes[BarCodeSymbology.ITF14] = typeof(SNBarCodeITF14ActionList);
			barCodeActionListTypes[BarCodeSymbology.Matrix2of5] = typeof(SNBarCodeMatrix2of5ActionList);
			barCodeActionListTypes[BarCodeSymbology.PDF417] = typeof(SNBarCodePDF417ActionList);
			barCodeActionListTypes[BarCodeSymbology.PostNet] = typeof(SNBarCodePostNetActionList);
			barCodeActionListTypes[BarCodeSymbology.QRCode] = typeof(SNBarCodeQRCodeActionList);
			barCodeActionListTypes[BarCodeSymbology.UPCA] = typeof(SNBarCodeUPCAActionList);
			barCodeActionListTypes[BarCodeSymbology.UPCE0] = typeof(SNBarCodeUPCE0ActionList);
			barCodeActionListTypes[BarCodeSymbology.UPCE1] = typeof(SNBarCodeUPCE1ActionList);
			barCodeActionListTypes[BarCodeSymbology.UPCSupplemental2] = typeof(SNBarCodeUPCSupplemental2ActionList);
			barCodeActionListTypes[BarCodeSymbology.UPCSupplemental5] = typeof(SNBarCodeUPCSupplemental5ActionList);
		}
		public BarCodeActionListDesigner(SnapFieldInfo fieldInfo, SNBarCodeField parsedInfo, SNSmartTagService frSmartTagService)
			: base(fieldInfo, parsedInfo, frSmartTagService) {
		}		
		protected override void RegisterActionLists(ActionListCollection list) {
			list.Add(new ContentTypeActionList(FieldInfo, this));
			list.Add(new DataFieldNameActionList(FieldInfo, this));
			list.Add(new SNBarCodeFieldActionList(FieldInfo, this));
			AddSymbologyBarCodeActionList(list);
		}
		void AddSymbologyBarCodeActionList(ActionListCollection list) {
			if (barCodeActionListTypes.ContainsKey(ParsedInfo.Symbology)) {
				IDesignerActionList actionList = (IDesignerActionList)Activator.CreateInstance(barCodeActionListTypes[ParsedInfo.Symbology], FieldInfo, this);
				list.Add(actionList);
			}
		}
	}
	#endregion
}
