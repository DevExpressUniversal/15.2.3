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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandEmpty
	public class XlsCommandEmpty : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.Seek(Size, SeekOrigin.Current);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandBeginOfSubstream
	public class XlsCommandBeginOfSubstream : XlsCommandContentBase {
		#region Fields
		protected internal const short BeginOfStreamCode = 0x0809;
		XlsContentBeginOfSubstream content = new XlsContentBeginOfSubstream();
		#endregion
		#region Properties
		public XlsSubstreamType SubstreamType {
			get { return this.content.SubstreamType; }
			protected internal set { this.content.SubstreamType = value; }
		}
		public int FileHistoryFlags {
			get { return this.content.FileHistoryFlags; }
			protected internal set { this.content.FileHistoryFlags = value; }
		}
		#endregion
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.StartContent(SubstreamType);
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals)
				contentBuilder.DocumentModel.Properties.WorkbookWindowPropertiesList.Clear();
			if(contentBuilder.ContentType == XlsContentType.Sheet)
				contentBuilder.ClearConditionalFormats();
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return this.content;
		}
	}
	#endregion
	#region XlsCommandEndOfSubstream
	public class XlsCommandEndOfSubstream : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if(contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				contentBuilder.StyleSheet.RegisterTheme();
				contentBuilder.StyleSheet.RegisterStyles();
				contentBuilder.RegisterDefinedNames();
				contentBuilder.ClearDataCollectors();
			}
			else if(contentBuilder.ContentType == XlsContentType.Sheet) {
				contentBuilder.CreateDrawingObjects();
				contentBuilder.CreateNotes();
				contentBuilder.CreateSelection();
				contentBuilder.RegisterConditionalFormats();
				contentBuilder.CleanupIncompliantSharedFormulas();
				contentBuilder.ClearDataCollectors();
				contentBuilder.SetupSheetProtection();
				contentBuilder.CheckTableNames();
			}
			else if (contentBuilder.ContentType == XlsContentType.Chart) {
				contentBuilder.SetupChartDataCache();
				contentBuilder.CurrentChart = null;
				contentBuilder.ClearDataCollectors();
			}
			contentBuilder.EndContent();
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandInterfaceHeader
	public class XlsCommandInterfaceHeader : XlsCommandEncodingBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Encoding = DXEncoding.GetEncodingFromCodePage(reader.ReadNotCryptedInt16());
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandAddDelMenuItems
	public class XlsCommandAddDelMenuItems : XlsCommandBase {
		#region Fields
		byte addMenuItemsCount;
		byte delMenuItemsCount;
		#endregion
		#region Properties
		public byte AddMenuItemsCount {
			get { return this.addMenuItemsCount; }
			protected internal set { this.addMenuItemsCount = value; }
		}
		public byte DelMenuCount {
			get { return this.delMenuItemsCount; }
			protected internal set { this.delMenuItemsCount = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.addMenuItemsCount = reader.ReadByte();
			this.delMenuItemsCount = reader.ReadByte();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(this.addMenuItemsCount);
			writer.Write(this.delMenuItemsCount);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandInterfaceEnd
	public class XlsCommandInterfaceEnd : XlsCommandEmpty {
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandEncoding
	public class XlsCommandEncoding : XlsCommandEncodingBase {
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.Options.ActualEncoding = Encoding;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandSheetIdTable
	public class XlsCommandSheetIdTable : XlsCommandContentBase {
		XlsContentSheetIdTable content = new XlsContentSheetIdTable();
		public List<int> SheetIdTable { get { return content.SheetIdTable; } }
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			int count = SheetIdTable.Count;
			for (int i = 0; i < count; i++) {
				contentBuilder.DocumentModel.Properties.SheetIdTable.Add(SheetIdTable[i]);
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandBuiltInFunctionGroupCount
	public class XlsCommandBuiltInFunctionGroupCount : XlsCommandShortPropertyValueBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.BuiltInFunctionGroupCount = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandOleObjectSize
	public class XlsCommandOleObjectSize : XlsCommandBase {
		public CellRangeInfo Range { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt16(); 
			int firstRow = reader.ReadUInt16();
			int lastRow = reader.ReadUInt16();
			int firstColumn = reader.ReadByte();
			int lastColumn = reader.ReadByte();
			Range = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.OleObjectRange = Range;
		}
		protected override void WriteCore(BinaryWriter writer) {
			const ushort magicConstant = 0; 
			writer.Write(magicConstant);
			if (Range != null) {
				writer.Write((ushort)Range.First.Row);
				writer.Write((ushort)Range.Last.Row);
				writer.Write((byte)Range.First.Column);
				writer.Write((byte)Range.Last.Column);
			}
			else {
				writer.Write((ushort)0);
				writer.Write((ushort)0);
				writer.Write((byte)0);
				writer.Write((byte)0);
			}
		}
		protected override short GetSize() {
			return 8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandWindowsProtected
	public class XlsCommandWindowsProtected : XlsCommandBoolPropertyBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.Protection.LockWindows = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandProtected
	public class XlsCommandProtected : XlsCommandBoolPropertyBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.Protection.LockStructure = Value;
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.Properties.Protection.SheetLocked = Value;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				contentBuilder.CurrentChart.Protection = Value ? ChartSpaceProtection.All : ChartSpaceProtection.None;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPasswordVerifier
	public class XlsCommandPasswordVerifier : XlsCommandShortPropertyValueBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			ProtectionByPasswordVerifier byPasswordVerifier = new ProtectionByPasswordVerifier((ushort)Value);
			ProtectionCredentials credentials = contentBuilder.DocumentModel.Properties.Protection.Credentials;
			credentials.RegisterPasswordVerifier(byPasswordVerifier);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			ProtectionByPasswordVerifier byPasswordVerifier = new ProtectionByPasswordVerifier((ushort)Value);
			WorksheetProtectionOptions protection = contentBuilder.CurrentSheet.Properties.Protection;
			protection.BeginUpdate();
			try {
				ProtectionCredentials credentials = new ProtectionCredentials();
				credentials.RegisterPasswordVerifier(byPasswordVerifier);
				protection.Credentials = credentials;
			}
			finally {
				protection.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandProtectForRevisions
	public class XlsCommandProtectForRevisions : XlsCommandBoolPropertyBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.Protection.LockRevisions = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandProtectForRevisionsPasswordVerifier
	public class XlsCommandProtectForRevisionsPasswordVerifier : XlsCommandShortPropertyValueBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (Value != 0) {
				ProtectionCredentials protectionCredentials = contentBuilder.DocumentModel.Properties.Protection.Credentials;
				protectionCredentials.RegisterWorkbookRevisionsProtection(new ProtectionByWorkbookRevisions((ushort)Value));
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandShouldSaveBackup
	public class XlsCommandShouldSaveBackup : XlsCommandBoolPropertyBase {
		bool hasValue = true;
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			hasValue = Size != 0;
			if(hasValue)
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if(hasValue)
				contentBuilder.DocumentModel.Properties.SaveBackup = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandObjectsDisplayOptions
	public class XlsCommandDisplayObjectsOptions : XlsCommandBase {
		DisplayObjectsOptions displayObjects;
		public DisplayObjectsOptions DisplayObjects { get { return this.displayObjects; } set { this.displayObjects = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.displayObjects = (DisplayObjectsOptions)reader.ReadInt16();
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.DisplayObjects = DisplayObjects;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)DisplayObjects);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandIs1904DateSystemUsed
	public class XlsCommandIs1904DateSystemUsed : XlsCommandBoolPropertyBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.CalculationOptions.DateSystem = Value ? DateSystem.Date1904 : DateSystem.Date1900;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPrecisionAsDisplayed
	public class XlsCommandPrecisionAsDisplayed : XlsCommandBoolPropertyBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Value = reader.ReadInt16() == 0x00;
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.CalculationOptions.PrecisionAsDisplayed = Value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Convert.ToInt16(!Value));
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandRefreshAllOnLoading
	public class XlsCommandRefreshAllOnLoading : XlsCommandBoolPropertyBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.RefreshAllOnLoading = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandWorkbookBoolProperties
	public class XlsCommandWorkbookBoolProperties : XlsCommandContentBase {
		XlsContentWorkbookBool content = new XlsContentWorkbookBool();
		#region Properties
		public bool SaveExternalLinksValues { 
			get { return !content.NotSaveExternalLinksValues; }
			set { content.NotSaveExternalLinksValues = !value; } 
		}
		public bool HasEnvelope { 
			get { return content.HasEnvelope; } 
			set { content.HasEnvelope = value; } 
		}
		public bool EnvelopeVisible { 
			get { return content.EnvelopeVisible; } 
			set { content.EnvelopeVisible = value; } 
		}
		public bool EnvelopeInitDone { 
			get { return content.EnvelopeInitDone; } 
			set { content.EnvelopeInitDone = value; } 
		}
		public bool ShowBordersOfUnselectedTables { 
			get { return !content.HideBordersOfUnselTables; } 
			set { content.HideBordersOfUnselTables = !value; } 
		}
		#endregion
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			DocumentModel documentModel = contentBuilder.DocumentModel;
			documentModel.Properties.CalculationOptions.SaveExternalLinkValues = SaveExternalLinksValues;
			documentModel.Properties.ShowBordersOfUnselectedTables = ShowBordersOfUnselectedTables;
			documentModel.MailOptions.EnvelopeInitDone = EnvelopeInitDone;
			documentModel.MailOptions.EnvelopeVisible = EnvelopeVisible;
			documentModel.MailOptions.HasEnvelope = HasEnvelope;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandFont
	public class XlsCommandFont : XlsCommandContentBase {
		public const double FontCoeff = 20.0;
		public const short DefaultNormal = 400;
		public const short DefaultBoldness = 700;
		XlsContentFont content = new XlsContentFont();
		public XlsContentFont Content { get { return content; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
			XlsFontInfo info = new XlsFontInfo();
			info.Bold = content.Bold;
			info.Boldness = content.Boldness;
			info.Condense = content.Condense;
			info.Extend = content.Extend;
			info.Italic = content.Italic;
			info.Outline = content.Outline;
			info.Shadow = content.Shadow;
			info.StrikeThrough = content.StrikeThrough;
			info.Charset = content.Charset;
			info.FontFamily = content.FontFamily;
			info.Name = content.FontName;
			info.Size = content.Size;
			info.FontColorIndex = content.ColorIndex;
			info.FontColor.ColorIndex = contentBuilder.StyleSheet.GetPaletteColorIndex(content.ColorIndex, true);
			info.SchemeStyle = XlFontSchemeStyles.None;
			info.Script = content.Script;
			info.Underline = content.Underline;
			contentBuilder.StyleSheet.Fonts.Add(info);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
		protected internal void SetFontInfo(RunFontInfo fontInfo) {
			content.Bold = fontInfo.Bold;
			content.Condense = fontInfo.Condense;
			content.Extend = fontInfo.Extend;
			content.Italic = fontInfo.Italic;
			content.Outline = fontInfo.Outline;
			content.Shadow = fontInfo.Shadow;
			content.StrikeThrough = fontInfo.StrikeThrough;
			content.Charset = fontInfo.Charset;
			content.FontFamily = fontInfo.FontFamily;
			content.FontName = fontInfo.Name;
			content.Size = fontInfo.Size;
			content.ColorIndex = fontInfo.ColorIndex;
			content.Script = fontInfo.Script;
			content.Underline = fontInfo.Underline;
		}
	}
	#endregion
	#region XlsCommandNumberFormat
	public class XlsCommandNumberFormat : XlsCommandContentBase {
		XlsContentNumberFormat content = new XlsContentNumberFormat();
		#region Properties
		public int FormatId { 
			get { return content.FormatId; } 
			set { content.FormatId = value; } 
		}
		public string FormatCode { 
			get { return content.FormatCode; } 
			set { content.FormatCode = value; } 
		}
		#endregion
		public override void Execute(XlsContentBuilder contentBuilder) {
			string formatCode = FormatCode;
			if (string.Equals(formatCode, "GENERAL", StringComparison.OrdinalIgnoreCase))
				formatCode = string.Empty;
			contentBuilder.StyleSheet.RegisterNumberFormat(FormatId, formatCode);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandSupportNaturalLanguagesFormulaInput
	public class XlsCommandSupportNaturalLanguagesFormulaInput : XlsCommandBoolPropertyBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.SupportNaturalLanguagesFormulaInput = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandSheetInformation
	public class XlsCommandSheetInformation : XlsCommandBase {
		#region Fields
		const int basePartSize = 6;
		const int maxSheetNameLength = 31;
		int startPosition;
		SheetVisibleState visibleState;
		SheetType type;
		ShortXLUnicodeString name;
		#endregion
		public XlsCommandSheetInformation() {
			this.name = new ShortXLUnicodeString();
		}
		#region Properties
		public int StartPosition { get { return startPosition; } set { startPosition = value; } }
		public SheetVisibleState VisibleState { get { return visibleState; } set { visibleState = value; } }
		public SheetType Type { get { return type; } set { type = value; } }
		public string Name { get { return this.name.Value; } set { this.name.Value = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.startPosition = reader.ReadNotCryptedInt32();
			this.visibleState = (SheetVisibleState)(reader.ReadByte() & 0x03);
			this.type = (SheetType)reader.ReadByte();
			this.name = ShortXLUnicodeString.FromStream(reader);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.StyleSheet.RegisterFormats();
			contentBuilder.RegisterSheetInfo(Type, Name, VisibleState);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(StartPosition);
			writer.Write((byte)VisibleState);
			writer.Write((byte)Type);
			this.name.Write(writer);
		}
		protected override short GetSize() {
			return (short)(basePartSize + this.name.Length);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandRecalcInformation
	public class XlsCommandRecalcInformation : XlsCommandBase {
		#region Fields
		const short size = 0x08;
		const int defaultRecalcId = 0x0001be22;
		int recalcId;
		#endregion
		protected internal int RecalcId { get { return this.recalcId; } set { this.recalcId = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadInt32();
			this.recalcId = reader.ReadInt32();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(XlsCommandFactory.GetTypeCodeByType(GetType()));
			writer.BaseStream.Seek(2, SeekOrigin.Current);
			writer.Write(defaultRecalcId);
		}
		protected override short GetSize() {
			return size;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandIndex
	public class XlsCommandIndex : XlsCommandContentBase {
		XlsContentIndex content = new XlsContentIndex();
		public XlsContentIndex Index { get { return content; } set { content = value; } }
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override IXlsContent GetContent() {
			return content;
		}
		public override IXlsCommand GetInstance() {
			this.content = new XlsContentIndex();
			return this;
		}
	}
	#endregion
	#region XlsCommandCalculationMode
	public class XlsCommandCalculationMode : XlsCommandBase {
		int calculationMode;
		public ModelCalculationMode CalculationMode { 
			get {
				if(this.calculationMode == 0)
					return ModelCalculationMode.Manual;
				if(this.calculationMode == 2)
					return ModelCalculationMode.AutomaticExceptTables;
				return ModelCalculationMode.Automatic;
			} 
			set {
				if(value == ModelCalculationMode.Manual)
					this.calculationMode = 0;
				else if(value == ModelCalculationMode.AutomaticExceptTables)
					this.calculationMode = 2;
				else
					this.calculationMode = 1;
			}
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.calculationMode = reader.ReadInt16();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			CalculationModeOverride overrideCalculationMode = contentBuilder.Options.OverrideCalculationMode;
			if(overrideCalculationMode == CalculationModeOverride.None)
				contentBuilder.DocumentModel.Properties.CalculationOptions.CalculationMode = CalculationMode;
			else
				contentBuilder.DocumentModel.Properties.CalculationOptions.CalculationMode = (ModelCalculationMode)overrideCalculationMode;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)this.calculationMode);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandIterationsEnabled
	public class XlsCommandIterationsEnabled : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.CalculationOptions.IterationsEnabled = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandIterationCount
	public class XlsCommandIterationCount : XlsCommandShortPropertyValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			int calcCount = Value;
			if(calcCount < 1)
				calcCount = 1;
			if(calcCount > short.MaxValue)
				calcCount = short.MaxValue;
			contentBuilder.DocumentModel.Properties.CalculationOptions.MaximumIterations = calcCount;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandReferenceMode
	public class XlsCommandReferenceMode : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.UseR1C1ReferenceStyle = !Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandDefaultRowHeight
	public class XlsCommandDefaultRowHeight : XlsCommandContentBase {
		XlsContentDefaultRowHeight content = new XlsContentDefaultRowHeight();
		#region Properties
		public bool IsCustomHeight { get { return content.CustomHeight; } set { content.CustomHeight = value; } }
		public bool ZeroHeight { get { return content.ZeroHeightOnEmptyRows; } set { content.ZeroHeightOnEmptyRows = value; } }
		public bool ThickTopBorder { get { return content.ThickTopBorder; } set { content.ThickTopBorder = value; } }
		public bool ThickBottomBorder { get { return content.ThickBottomBorder; } set { content.ThickBottomBorder = value; } }
		public int DefaultRowHeightInTwips { 
			get { return content.DefaultRowHeightInTwips; }
			set { content.DefaultRowHeightInTwips = value; } 
		}
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			SheetFormatProperties FormatProperties = contentBuilder.CurrentSheet.Properties.FormatProperties;
			FormatProperties.IsCustomHeight = IsCustomHeight;
			FormatProperties.ThickBottomBorder = ThickBottomBorder;
			FormatProperties.ThickTopBorder = ThickTopBorder;
			FormatProperties.ZeroHeight = ZeroHeight;
			FormatProperties.DefaultRowHeight = contentBuilder.DocumentModel.UnitConverter.TwipsToModelUnits(DefaultRowHeightInTwips);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandAdditionalWorksheetInformation
	public class XlsCommandAdditionalWorksheetInformation : XlsCommandContentBase {
		#region Fields
		XlsContentWsBool content = new XlsContentWsBool();
		#endregion
		#region Properties
		public bool ShowPageBreaks { get { return content.ShowPageBreaks; } set { content.ShowPageBreaks = value; } }
		public bool IsDialog { get { return content.IsDialog; } set { content.IsDialog = value; } }
		public bool ApplyStyles { get { return content.ApplyStyles; } set { content.ApplyStyles = value; } }
		public bool ShowRowSumsBelow { get { return content.ShowRowSumsBelow; } set { content.ShowRowSumsBelow = value; } }
		public bool ShowColumnSumsRight { get { return content.ShowColumnSumsRight; } set { content.ShowColumnSumsRight = value; } }
		public bool FitToPage { get { return content.FitToPage; } set { content.FitToPage = value; } }
		public bool SynchronizeHorizontalScrolling { get { return content.SynchronizeHorizontalScrolling; } set { content.SynchronizeHorizontalScrolling = value; } }
		public bool SynchronizeVerticalScrolling { get { return content.SynchronizeVerticalScrolling; } set { content.SynchronizeVerticalScrolling = value; } }
		public bool TransitionFormulaEvaluation { get { return content.TransitionFormulaEvaluation; } set { content.TransitionFormulaEvaluation = value; } }
		public bool TransitionFormulaEntry { get { return content.TransitionFormulaEntry; } set { content.TransitionFormulaEntry = value; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			WorksheetProperties sheetProperties = contentBuilder.CurrentSheet.Properties;
			ModelWorksheetView view = contentBuilder.CurrentSheet.ActiveView;
			view.ShowPageBreaks = ShowPageBreaks;
			sheetProperties.IsDialog = IsDialog;
			sheetProperties.GroupAndOutlineProperties.ApplyStyles = ApplyStyles;
			sheetProperties.GroupAndOutlineProperties.ShowColumnSumsRight = ShowColumnSumsRight;
			sheetProperties.GroupAndOutlineProperties.ShowRowSumsBelow = ShowRowSumsBelow;
			sheetProperties.PrintSetup.FitToPage = FitToPage;
			view.SynchronizeHorizontalScrolling = SynchronizeHorizontalScrolling;
			view.SynchronizeVerticalScrolling = SynchronizeVerticalScrolling;
			sheetProperties.TransitionOptions.TransitionFormulaEntry = TransitionFormulaEntry;
			sheetProperties.TransitionOptions.TransitionFormulaEvaluation = TransitionFormulaEvaluation;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandDefaultColumnWidth
	public class XlsCommandDefaultColumnWidth : XlsCommandBase {
		int value;
		public int Value { 
			get { return this.value; } 
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue);
				this.value = value; 
			} 
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.value = reader.ReadUInt16();
			int bytesToRead = Size - 2;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.Properties.FormatProperties.BaseColumnWidth = Value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)this.value);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandDimensions
	public class XlsCommandDimensions : XlsCommandContentBase {
		XlsContentDimensions content = new XlsContentDimensions();
		public XlsContentDimensions Dimensions { get { return content; } }
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandRow
	public class XlsCommandRow : XlsCommandContentBase {
		XlsContentRow content = new XlsContentRow();
		#region Properties
		public int HeightInTwips { get { return content.HeightInTwips; } set { content.HeightInTwips = value; } }
		public int Index { get { return content.Index; } set { content.Index = value; } }
		public int FirstColumnIndex { get { return content.FirstColumnIndex; } set { content.FirstColumnIndex = value; } }
		public int LastColumnIndex { get { return content.LastColumnIndex; } set { content.LastColumnIndex = value; } }
		public int OutlineLevel { get { return content.OutlineLevel; } set { content.OutlineLevel = value; } }
		public bool IsCollapsed { get { return content.IsCollapsed; } set { content.IsCollapsed = value; } }
		public bool IsHidden { get { return content.IsHidden; } set { content.IsHidden = value; } }
		public bool IsCustomHeight { get { return content.IsCustomHeight; } set { content.IsCustomHeight = value; } }
		public bool HasFormatting { get { return content.HasFormatting; } set { content.HasFormatting = value; } }
		public int FormatIndex { get { return content.FormatIndex; } set { content.FormatIndex = value; } }
		public bool HasThickBorder { get { return content.HasThickBorder; } set { content.HasThickBorder = value; } }
		public bool HasMediumBorder { get { return content.HasMediumBorder; } set { content.HasMediumBorder = value; } }
		public bool HasPhoneticGuide { get { return content.HasPhoneticGuide; } set { content.HasPhoneticGuide = value; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Worksheet sheet = contentBuilder.CurrentSheet;
			if (sheet == null)
				return;
			if(IsCustomRow()) {
				Row row = sheet.Rows[Index];
				row.BeginUpdate();
				try {
					row.Height = contentBuilder.DocumentModel.UnitConverter.TwipsToModelUnits(HeightInTwips);
					row.OutlineLevel = OutlineLevel;
					row.IsCollapsed = IsCollapsed;
					row.IsHidden = IsHidden;
					row.IsCustomHeight = IsCustomHeight;
					row.IsThickTopBorder = HasThickBorder;
					row.IsThickBottomBorder = HasMediumBorder;
					if (OutlineLevel > 0 && OutlineLevel <= 7 && sheet.Properties.FormatProperties.OutlineLevelRow < OutlineLevel)
						sheet.Properties.FormatProperties.OutlineLevelRow = OutlineLevel;
				}
				finally {
					row.EndUpdate();
				}
				if (HasFormatting && FormatIndex > contentBuilder.DocumentModel.StyleSheet.DefaultCellFormatIndex)
					row.AssignCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(FormatIndex));
			}
		}
		bool IsCustomRow() {
			return (FirstColumnIndex != LastColumnIndex) || IsCustomHeight || IsCollapsed ||
				IsHidden || HasFormatting || HasThickBorder || HasMediumBorder || HasPhoneticGuide ||
				OutlineLevel > 0 || HeightInTwips == 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandDSF
	public class XlsCommandDSF : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)0);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandCountry
	public class XlsCommandCountry : XlsCommandContentBase {
		XlsContentCountry content = new XlsContentCountry();
		#region Properties
		public int DefaultCountryIndex { get { return content.DefaultCountryIndex; } set { content.DefaultCountryIndex = value; } }
		public int CountryIndex { get { return content.CountryIndex; } set { content.CountryIndex = value; } }
		#endregion
		public override void Execute(XlsContentBuilder contentBuilder) {
			CultureInfo culture = XlsCountryCodes.GetCultureInfo(CountryIndex);
			contentBuilder.DocumentModel.InnerCulture = culture;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandContinue
	public class XlsCommandContinue : XlsCommandRecordBase {
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder.DataCollector != null)
				contentBuilder.DataCollector.PutData(Data, contentBuilder);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandCalculationDelta
	public class XlsCommandCalculationDelta : XlsCommandBase {
		double value;
		public double Value { get { return this.value; } set { this.value = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.value = reader.ReadDouble();
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.CalculationOptions.IterativeCalculationDelta = this.value;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(this.value);
		}
		protected override short GetSize() {
			return sizeof(double);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandRecalculateBeforeSaved
	public class XlsCommandRecalculateBeforeSaved : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.Properties.CalculationOptions.RecalculateBeforeSaving = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPrintRowColHeaders
	public class XlsCommandPrintRowColHeadings : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.PrintSetup.PrintHeadings = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPrintGridLines
	public class XlsCommandPrintGridLines : XlsCommandBoolPropertyBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Value = (reader.ReadInt16() & 0x0001) != 0;
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.PrintSetup.PrintGridLines = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPrintGridLinesSet
	public class XlsCommandPrintGridLinesSet : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.CurrentSheet.PrintSetup.PrintGridLinesSet = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandGuts
	public class XlsCommandGuts : XlsCommandContentBase {
		XlsContentGuts content = new XlsContentGuts();
		#region Properties
		public int RowGutterMaxOutlineLevel {
			get { return content.RowGutterMaxOutlineLevel; }
			set { content.RowGutterMaxOutlineLevel = value; }
		}
		public int ColumnGutterMaxOutlineLevel {
			get { return content.ColumnGutterMaxOutlineLevel; }
			set { content.ColumnGutterMaxOutlineLevel = value; }
		}
		#endregion
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandScenarioProtected
	public class XlsCommandScenarioProtected : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			WorksheetProtectionOptions protection = contentBuilder.CurrentSheet.Properties.Protection;
			protection.BeginUpdate();
			try {
				protection.ScenariosLocked = Value;
				if (Value)
					protection.SheetLocked = true;
			}
			finally {
				protection.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandObjectsProtected
	public class XlsCommandObjectsProtected : XlsCommandBoolPropertyBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			WorksheetProtectionOptions protection = contentBuilder.CurrentSheet.Properties.Protection;
			protection.BeginUpdate();
			try {
				protection.ObjectsLocked = Value;
				if (Value)
					protection.SheetLocked = true;
			}
			finally {
				protection.EndUpdate();
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPalette
	public class XlsCommandPalette : XlsCommandBase {
		const int maxPaletteSize = 56;
		readonly List<Color> colors = new List<Color>();
		public IList<Color> Colors { get { return colors; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int count = reader.ReadInt16();
			if(count > maxPaletteSize)
				contentBuilder.ThrowInvalidFile("More than 56 colors in palette record!");
			for(int i = 0; i < count; i++) {
				Colors.Add(XlsLongRGB.FromStream(reader));
			}
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			int count = Math.Min(Colors.Count, maxPaletteSize);
			Palette palette = contentBuilder.DocumentModel.StyleSheet.Palette;
			for(int i = 0; i < count; i++)
				palette[i + Palette.BuiltInColorsCount] = Colors[i];
		}
		protected override void WriteCore(BinaryWriter writer) {
			int count = Math.Min(Colors.Count, maxPaletteSize);
			writer.Write((short)count);
			for(int i = 0; i < count; i++) {
				XlsLongRGB.Write(writer, Colors[i]);
			}
		}
		protected override short GetSize() {
			int count = Math.Min(Colors.Count, maxPaletteSize);
			return (short)(count * 4 + 2);
		}
		public override IXlsCommand GetInstance() {
			this.colors.Clear();
			return this;
		}
	}
	#endregion
	#region XlsCommandExcel9File
	public class XlsCommandExcel9File : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(Size > 0)
				reader.ReadBytes(Size);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandContinueFrt
	public class XlsCommandContinueFrt : XlsCommandContinueFrtBase {
		public XlsCommandContinueFrt() : base() { }
		public override IXlsCommand GetInstance() {
			return new XlsCommandContinueFrt();
		}
	}
	#endregion
	#region XlsCommandContinueFrt11
	public class XlsCommandContinueFrt11 : XlsCommandContinueFrtBase {
		public XlsCommandContinueFrt11() : base() { }
		public override IXlsCommand GetInstance() {
			return new XlsCommandContinueFrt11();
		}
	}
	#endregion
	#region XlsCommandContinueFrt12
	public class XlsCommandContinueFrt12 : XlsCommandContinueFrtBase {
		public XlsCommandContinueFrt12() : base() { }
		public override void Execute(XlsContentBuilder contentBuilder) {
			if(contentBuilder.DataCollector != null)
				contentBuilder.DataCollector.PutData(Data, contentBuilder);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandContinueFrt12();
		}
	}
	#endregion
	#region XlsCommandVBAProject
	public class XlsCommandVBAProject : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.HasVBAProject = true;
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandVBAProjectHasNoMacros
	public class XlsCommandVBAProjectHasNoMacros : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.DocumentModel.VbaProjectContent.HasNoMacros = true;
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandCodeName
	public class XlsCommandCodeName : XlsCommandBase {
		XLUnicodeString name = new XLUnicodeString();
		public string Name {
			get { return name.Value; }
			set { name.Value = value; }
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.name = XLUnicodeString.FromStream(reader);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			string codeName = Name;
			if(!CodeNameHelper.IsValidCodeName(codeName))
				codeName = CodeNameHelper.CleanUp(codeName);
			contentBuilder.DocumentModel.Properties.CodeName = codeName;
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			string codeName = Name;
			if(!CodeNameHelper.IsValidCodeName(codeName))
				codeName = CodeNameHelper.CleanUp(codeName);
			contentBuilder.CurrentSheet.Properties.CodeName = codeName;
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.name.Write(writer);
		}
		protected override short GetSize() {
			return (short)name.Length;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandFrtWrapper
	public class XlsCommandFrtWrapper : XlsCommandBase {
		#region Properties
		public IXlsCommand InnerCommand { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			byte[] data = reader.ReadBytes(Size - 4);
			ReadInnerCommand(contentBuilder, data);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (InnerCommand != null)
				InnerCommand.Execute(contentBuilder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld frtHeader = new FutureRecordHeaderOld();
			frtHeader.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			frtHeader.Write(writer);
			InnerCommand.Write(writer);
			int paddingSize = 8 - InnerCommand.GetRecordSize();
			if (paddingSize > 0)
				writer.Write(new byte[paddingSize]);
		}
		protected override short GetSize() {
			int result = 4 + InnerCommand.GetRecordSize();
			if (result < 12)
				result = 12;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		void ReadInnerCommand(XlsContentBuilder contentBuilder, byte[] data) {
			using (MemoryStream ms = new MemoryStream(data, false)) {
				using (BinaryReader baseReader = new BinaryReader(ms)) {
					using (XlsReader reader = new XlsReader(baseReader)) {
						InnerCommand = contentBuilder.CommandFactory.CreateCommand(reader);
						InnerCommand.Read(reader, contentBuilder);
					}
				}
			}
		}
	}
	#endregion
	#region XlsCommandWorkbookExtendedProperties
	public class XlsCommandWorkbookExtendedProperties : XlsCommandBase {
		#region Fields
		int countRecord;
		bool errorFrtCode = false;
		BitwiseContainer flags = new BitwiseContainer(32, new int[] { 1, 1, 1, 1, 2 });
		BitwiseContainer extFlags = new BitwiseContainer(16, new int[] { 1, 1, 6 });
		private enum XlsFlags {
			AutoRecover = 0, 
			HidePivotList = 1, 
			FilterPrivacy = 2, 
			EmbedFactoids = 3, 
			FactoidDisplay = 4, 
			SavedDuringRecovery = 5, 
			CreatedViaMinimalSave = 6, 
			OpenedViaDataRecovery = 7, 
			OpenedViaSafeLoad = 8 
		}
		private enum XlsExtFlags {
			BuggedUserAboutSolution = 0, 
			ShowInkAnnotation = 1, 
			PublishedBookItems = 4, 
			ShowPivotChartFilter = 5, 
		}
		#endregion
		#region Properties
		public int CountRecord { get { return countRecord; } set { countRecord = value; } }
		public bool IsAutoRecover {
			get { return flags.GetBoolValue((int)XlsFlags.AutoRecover); }
			set { flags.SetBoolValue((int)XlsFlags.AutoRecover, value); }
		}
		public bool IsHidePivotList {
			get { return flags.GetBoolValue((int)XlsFlags.HidePivotList); }
			set { flags.SetBoolValue((int)XlsFlags.HidePivotList, value); }
		}
		public bool IsFilterPrivacy {
			get { return flags.GetBoolValue((int)XlsFlags.FilterPrivacy); }
			set { flags.SetBoolValue((int)XlsFlags.FilterPrivacy, value); }
		}
		public bool IsEmbedFactoids {
			get { return flags.GetBoolValue((int)XlsFlags.EmbedFactoids); }
			set { flags.SetBoolValue((int)XlsFlags.EmbedFactoids, value); }
		}
		public int IsFactoidDisplay {
			get { return flags.GetIntValue((int)XlsFlags.FactoidDisplay); }
			set { flags.SetIntValue((int)XlsFlags.FactoidDisplay, value); }
		}
		public bool IsSavedDuringRecovery {
			get { return flags.GetBoolValue((int)XlsFlags.SavedDuringRecovery); }
			set { flags.SetBoolValue((int)XlsFlags.SavedDuringRecovery, value); }
		}
		public bool IsCreatedViaMinimalSave {
			get { return flags.GetBoolValue((int)XlsFlags.CreatedViaMinimalSave); }
			set { flags.SetBoolValue((int)XlsFlags.CreatedViaMinimalSave, value); }
		}
		public bool IsOpenedViaDataRecovery {
			get { return flags.GetBoolValue((int)XlsFlags.OpenedViaDataRecovery); }
			set { flags.SetBoolValue((int)XlsFlags.OpenedViaDataRecovery, value); }
		}
		public bool IsOpenedViaSafeLoad {
			get { return flags.GetBoolValue((int)XlsFlags.OpenedViaSafeLoad); }
			set { flags.SetBoolValue((int)XlsFlags.OpenedViaSafeLoad, value); }
		}
		public bool IsBuggedUserAboutSolution {
			get { return extFlags.GetBoolValue((int)XlsExtFlags.BuggedUserAboutSolution); }
			set { extFlags.SetBoolValue((int)XlsExtFlags.BuggedUserAboutSolution, value); }
		}
		public bool IsShowInkAnnotation {
			get { return extFlags.GetBoolValue((int)XlsExtFlags.ShowInkAnnotation); }
			set { extFlags.SetBoolValue((int)XlsExtFlags.ShowInkAnnotation, value); }
		}
		public bool IsPublishedBookItems {
			get { return extFlags.GetBoolValue((int)XlsExtFlags.PublishedBookItems); }
			set { extFlags.SetBoolValue((int)XlsExtFlags.PublishedBookItems, value); }
		}
		public bool IsShowPivotChartFilter {
			get { return extFlags.GetBoolValue((int)XlsExtFlags.ShowPivotChartFilter); }
			set { extFlags.SetBoolValue((int)XlsExtFlags.ShowPivotChartFilter, value); }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader frt = FutureRecordHeader.FromStream(reader);
			short idFrt = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandWorkbookExtendedProperties));
			if (frt.RecordTypeId == idFrt) {
				CountRecord = reader.ReadInt32(); 
				flags.IntContainer = reader.ReadInt32();
				if (CountRecord > 21)
					extFlags.ShortContainer = reader.ReadInt16();
				else if (CountRecord > 20)
					extFlags.ShortContainer = reader.ReadByte();
			}
			else {
				errorFrtCode = true;
				reader.ReadBytes(this.Size - frt.GetSize());
			}
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (!errorFrtCode)
				if (contentBuilder.DocumentModel.Properties != null)
					contentBuilder.DocumentModel.Properties.HidePivotFieldList = IsHidePivotList;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandWorkbookExtendedProperties));
			header.Write(writer);
			writer.Write((int)22); 
			writer.Write((int)flags.IntContainer);
			writer.Write((short)extFlags.ShortContainer);
		}
		protected override short GetSize() {
			return 22;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandTemplate
	public class XlsCommandTemplate : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (Size > 0)
				reader.ReadBytes(Size);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
