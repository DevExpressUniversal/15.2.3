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
using System.ComponentModel;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Import.Rtf;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Import.Xaml;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office.Services;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
using DevExpress.Compatibility.System.Drawing.Imaging;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraRichEdit.Layout;
using System.Drawing.Imaging;
using DevExpress.XtraRichEdit;
using System.Runtime.InteropServices;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.PInvoke;
using System.Diagnostics;
#else
using System.Windows;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region PasteSelectionCommand
	public class PasteSelectionCommand : TransactedInsertObjectCommand {
		public PasteSelectionCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PasteSelectionCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.PasteSelection; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PasteSelectionCommandFormat")]
#endif
		public DocumentFormat Format {
			get {
				PasteSelectionCoreCommand command = (PasteSelectionCoreCommand)InsertObjectCommand;
				return command.Format;
			}
			set {
				PasteSelectionCoreCommand command = (PasteSelectionCoreCommand)InsertObjectCommand;
				command.Format = value;
			}
		}
		protected internal override bool KeepLastParagraphMarkInSelection { get { return false; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new PasteSelectionCoreCommand(Control, new ClipboardPasteSource());
		}
		public override void UpdateUIState(ICommandUIState state) {
			Command pasteSelectionCommand = InsertObjectCommand;
			ICommandUIState commandState = pasteSelectionCommand.CreateDefaultCommandUIState();
			pasteSelectionCommand.UpdateUIState(commandState);
			if (commandState.Enabled && commandState.Visible) {
				ApplyCommandRestrictionOnEditableControl(state, Options.Behavior.Paste, commandState.Enabled);
				ApplyDocumentProtectionToSelectedCharacters(state);
			}
			else {
				state.Enabled = false;
				state.Visible = commandState.Visible;
				ApplyCommandRestrictionOnEditableControl(state, Options.Behavior.Paste, state.Enabled);
			}
			state.Checked = commandState.Checked;
			UpdateUIStateViaService(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			Debug.Assert(Commands.Count == 2);
			InsertObjectCommand.UpdateUIState(state);
		}
	}
	#endregion
	#region PasteExternalMultilineTextCommand
	public class PasteExternalMultilineTextCommand : TransactedInsertObjectCommand {
		public PasteExternalMultilineTextCommand(IRichEditControl control, string text)
			: base(control) {
			PasteExternalMultilineTextCoreCommand command = (PasteExternalMultilineTextCoreCommand)InsertObjectCommand;
			command.Text = text;
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.PasteSelection; } }
		protected internal override bool KeepLastParagraphMarkInSelection { get { return false; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new PasteExternalMultilineTextCoreCommand(Control, String.Empty);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			Debug.Assert(Commands.Count == 2);
			InsertObjectCommand.UpdateUIState(state);
		}
	}
	#endregion
#if SILVERLIGHT
	#region PasteInternalSelectionCommand
	public class PasteInternalSelectionCommand : PasteSelectionCommand {
		public PasteInternalSelectionCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new PasteInternalSelectionCoreCommand(Control, new ClipboardPasteSource());
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			Debug.Assert(Commands.Count == 2);
			InsertObjectCommand.UpdateUIState(state);
		}
	}
	#endregion
#endif
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region PasteSource (abstract class)
	public abstract class PasteSource {
		public virtual object GetData(string format) {
			return GetData(format, false);
		}
		public virtual bool ContainsData(string format) {
			return ContainsData(format, false);
		}
		public abstract object GetData(string format, bool autoConvert);
		public abstract bool ContainsData(string format, bool autoConvert);
	}
	#endregion
	#region ClipboardPasteSource
	public class ClipboardPasteSource : PasteSource {
		public override object GetData(string format, bool autoConvert) {
#if !SL
			IDataObject dataObject = OfficeClipboard.GetDataObject();
			if (dataObject == null)
				return null;
			else
				return dataObject.GetData(format, autoConvert);
#else
			return OfficeClipboard.GetData(format);
#endif
		}
		public override bool ContainsData(string format, bool autoConvert) {
#if !SL
			IDataObject dataObject = OfficeClipboard.GetDataObject();
			if (dataObject == null)
				return false;
			else
				return dataObject.GetDataPresent(format, autoConvert);
#else
			return OfficeClipboard.ContainsData(format);
#endif
		}
	}
	#endregion
	#region DataObjectPasteSource
	public class DataObjectPasteSource : PasteSource {
		IDataObject dataObject;
		public DataObjectPasteSource(IDataObject dataObject) {
			this.dataObject = dataObject;
		}
		public IDataObject DataObject { get { return dataObject; } set { dataObject = value; } }
		public override object GetData(string format, bool autoConvert) {
			try {
				if (dataObject != null) {
#if !SL
					return dataObject.GetData(format, autoConvert);
#else
					return dataObject.GetData(format);
#endif
				}
				else
					return null;
			}
			catch (System.Security.SecurityException) {
				return null; 
			}
		}
		public override bool ContainsData(string format, bool autoConvert) {
			try {
				if (dataObject != null)
					return dataObject.GetDataPresent(format, autoConvert);
				else
					return false;
			}
			catch (System.Security.SecurityException) {
				return false; 
			}
		}
	}
	#endregion
	#region EmptyPasteSource
	public class EmptyPasteSource : PasteSource {
		public override object GetData(string format, bool autoConvert) {
			return null;
		}
		public override bool ContainsData(string format, bool autoConvert) {
			return false;
		}
	}
	#endregion
	#region PasteSelectionCoreCommand
	public class PasteSelectionCoreCommand : MultiCommand {
		DocumentFormat format = DocumentFormat.Undefined;
		readonly PasteSource pasteSource;
		Exception pasteException;
		public PasteSelectionCoreCommand(IRichEditControl control, PasteSource pasteSource)
			: base(control) {
			Guard.ArgumentNotNull(pasteSource, "pasteSource");
			this.pasteSource = pasteSource;
			AssignPasteSource();
		}
		#region Properties
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Paste; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteDescription; } }
		public override string ImageName { get { return "Paste"; } }
		public DocumentFormat Format {
			get { return format; }
			set {
				if (format == value)
					return;
				format = value;
				Commands.Clear();
				CreateCommands();
				AssignPasteSource();
			}
		}
		public PasteSource PasteSource {
			get {
				return pasteSource;
			}
		}
		#endregion
		protected internal virtual void AddCommand(PasteContentCommandBase command) {
			if (CanAddCommandFormat(command))
				Commands.Add(command);
		}
		protected internal virtual bool CanAddCommandFormat(PasteContentCommandBase command) {
			return this.Format == DocumentFormat.Undefined || this.Format == command.Format;
		}
		protected internal override void ForceExecuteCore(ICommandUIState state) {
			pasteException = null;
			try {
				base.ForceExecuteCore(state);
				if (pasteException != null)
					DocumentServer.RaiseInvalidFormatException(pasteException);
			}
			finally {
				pasteException = null;
			}
		}
		protected internal override bool ExecuteCommand(Command command, ICommandUIState state) {
			try {
				pasteException = null;
				base.ExecuteCommand(command, state); 
				return true;
			}
			catch (Exception e) {
				if (pasteException == null)
					pasteException = e;
				return false;
			}
		}
		protected internal override void CreateCommands() {
			PasteContentCommandBase command = DocumentModel.CommandsCreationStrategy.CreateDefaultPasteCommand(Control);
			if (command != null)
				AddCommand(command);
#if !SL
			if (ContainsData(OfficeDataFormats.XMLSpreadsheet) || PasteFromIE()) { 
				AddCommand(new PasteHtmlTextCommand(Control));
				AddCommand(new PasteRtfTextCommand(Control));
			}
			else {
				AddCommand(new PasteRtfTextCommand(Control));
				AddCommand(new PasteHtmlTextCommand(Control));
			}
#else
			AddCommand(new PasteRtfTextCommand(Control));
#endif
			AddCommand(new PasteSilverlightXamlTextCommand(Control));
			AddCommand(new PastePlainTextCommand(Control));
#if !SL
			AddCommand(new PasteImageCommand(Control));
			AddCommand(new PasteMetafileCommand(Control));
			AddCommand(new PasteImagesFromFilesCommand(Control));
#endif
		}
		protected bool PasteFromIE() {
			return ContainsData(OfficeDataFormats.MsSourceUrl);
		}
		bool ContainsData(string format) {
			return new ClipboardPasteSource().ContainsData(format);
		}
		protected internal virtual void AssignPasteSource() {
			int count = Commands.Count;
			for (int i = 0; i < count; i++) {
				PasteContentCommandBase command = Commands[i] as PasteContentCommandBase;
				if (command != null)
					command.PasteSource = pasteSource;
			}
		}
	}
	#endregion
	#region PasteDataObjectCoreCommand
	public class PasteDataObjectCoreCommand : PasteSelectionCoreCommand {
		public PasteDataObjectCoreCommand(IRichEditControl control, IDataObject dataObject)
			: base(control, new DataObjectPasteSource(dataObject)) {
		}
		public IDataObject DataObject { get { return ((DataObjectPasteSource)PasteSource).DataObject; } set { ((DataObjectPasteSource)PasteSource).DataObject = value; } }
		protected internal override void CreateCommands() {
			AddCommand(new PasteLoadDocumentFromFileCommand(Control)); 
#if !SL
			if (PasteFromIE()) {
				AddCommand(new PasteHtmlTextFromDragDropCommand(Control));
				AddCommand(new PasteRtfTextCommand(Control));
			}
			else {
				AddCommand(new PasteRtfTextCommand(Control));
				AddCommand(new PasteHtmlTextFromDragDropCommand(Control));
			}
#else
			AddCommand(new PasteRtfTextCommand(Control));
#endif
			AddCommand(new PastePlainTextCommand(Control));
#if !SL
			AddCommand(new PasteImageCommand(Control));
			AddCommand(new PasteImagesFromFilesCommand(Control));
#endif
		}
	}
	#endregion
	#region PasteContentCommandBase (abstract class)
	public abstract class PasteContentCommandBase : InsertObjectCommandBase {
		PasteSource pasteSource;
		protected PasteContentCommandBase(IRichEditControl control)
			: base(control) {
			this.pasteSource = new EmptyPasteSource();
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Paste; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteDescription; } }
		public override string ImageName { get { return "Paste"; } }
		public abstract DocumentFormat Format { get; }
		public virtual PasteSource PasteSource { get { return pasteSource; } set { pasteSource = value; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
#if !SL
			CheckExecutedAtUIThread();
#endif
			bool enabled = IsContentEditable && IsDataAvailable();
			state.Enabled = enabled;
			state.Visible = true;
			state.Checked = false;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal abstract bool IsDataAvailable();
	}
	#endregion
	#region PastePlainTextCommand
	public class PastePlainTextCommand : PasteContentCommandBase {
		public PastePlainTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.PlainText; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PastePlainText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PastePlainTextDescription; } }
		protected internal override void ModifyModel() {
			string text = GetTextData();
			if (String.IsNullOrEmpty(text))
				return;
			DocumentLogPosition position = DocumentModel.Selection.End;
			ActivePieceTable.InsertPlainText(position, text, GetForceVisible());
		}
		protected internal virtual string GetTextData() {
			string result = PasteSource.GetData(OfficeDataFormats.UnicodeText) as string;
			if (result != null)
				return result;
			result = PasteSource.GetData(OfficeDataFormats.Text) as string;
			return result;
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.UnicodeText) ||
				PasteSource.ContainsData(OfficeDataFormats.Text) ||
				PasteSource.ContainsData(OfficeDataFormats.OemText);
		}
	}
	#endregion
	#region PasteContentConvertedToDocumentModelCommandBase (abstract class)
	public abstract class PasteContentConvertedToDocumentModelCommandBase : PasteContentCommandBase {
		PieceTablePasteContentConvertedToDocumentModelCommandBase innerCommand;
		protected PasteContentConvertedToDocumentModelCommandBase(IRichEditControl control)
			: base(control) {
			CreateInnerCommand();
		}
		protected internal PieceTablePasteContentConvertedToDocumentModelCommandBase InnerCommand { get { return innerCommand; } }
		public override DocumentFormat Format { get { return InnerCommand.Format; } }
		public override PasteSource PasteSource { get { return InnerCommand.PasteSource; } set { InnerCommand.PasteSource = value; } }
		protected internal void CreateInnerCommand() {
			this.innerCommand = CreateInnerCommandCore();
		}
		protected internal abstract PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore();
		protected internal override void ModifyModel() {
			InnerCommand.ForceInsertFloatingObjectAtParagraphStart = true;
			InnerCommand.Execute();
		}
		protected internal override bool IsDataAvailable() {
			return InnerCommand.IsDataAvailable();
		}
	}
	#endregion
	#region PasteRtfTextCommand
	public class PasteRtfTextCommand : PasteContentConvertedToDocumentModelCommandBase {
		public PasteRtfTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteRtfText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteRtfTextDescription; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteRtfTextCommand(ActivePieceTable);
		}
	}
	#endregion
	#region PasteSilverlightXamlTextCommand
	public class PasteSilverlightXamlTextCommand : PasteContentConvertedToDocumentModelCommandBase {
		public PasteSilverlightXamlTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteSilverlightXamlText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteSilverlightXamlTextDescription; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteSilverlightXamlTextCommand(ActivePieceTable);
		}
	}
	#endregion
#if !SL
	#region PasteImagesFromFilesCommand
	public class PasteImagesFromFilesCommand : PasteContentCommandBase {
		public PasteImagesFromFilesCommand(IRichEditControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteFiles; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteFilesDescription; } }
		protected internal override void ModifyModel() {
			string[] files = GetFileDropListData();
			if (files == null || files.Length == 0)
				return;
			Rectangle columnBounds = PasteImageCommand.GetCurrentColumnBounds(ActiveView.CaretPosition);
			foreach (string fileName in files) {
				if (File.Exists(fileName))
					PasteImageFromFile(fileName, columnBounds);
			}
		}
		protected internal virtual void PasteImageFromFile(string fileName, Rectangle columnBounds) {
			MemoryStreamBasedImage image = null;
			try {
				image = ImageLoaderHelper.ImageFromFile(fileName);
			}
			catch {
			}
			if (image != null) {
				PasteImageCommand command = new PasteImageCommand(Control);
				command.InsertPicture(image, columnBounds);
			}
		}
		protected internal string[] GetFileDropListData() {
			return PasteSource.GetData(OfficeDataFormats.FileDrop, true) as string[];
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.InlinePictures, state.Enabled);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.FileDrop, true);
		}
	}
	#endregion
#endif
	#region PasteLoadDocumentFromFileCommand
	public class PasteLoadDocumentFromFileCommand : PasteContentCommandBase {
		public PasteLoadDocumentFromFileCommand(IRichEditControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteFiles; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteFilesDescription; } }
		protected internal override void ModifyModel() {
			FileInfo[] files = GetFileDropListData();
			if (files == null || files.Length != 1)
				return;
			try {
#if !SL
				if (DocumentServer.RaiseDocumentClosing())
					DocumentServer.LoadDocument(files[0].FullName);
#else
				Stream stream = files[0].OpenRead();
				DocumentFormat format = DocumentModel.AutodetectDocumentFormat(files[0].Name);
				DocumentModel.LoadDocument(stream, format, String.Empty);
#endif
			}
			catch {
			}
		}
		protected internal virtual bool CanLoadFile(string fileName) {
			return DocumentModel.AutodetectDocumentFormat(fileName, false) != DocumentFormat.Undefined;
		}
		protected internal FileInfo[] GetFileDropListData() {
			object data = PasteSource.GetData(OfficeDataFormats.FileDrop, true);
			FileInfo[] result = data as FileInfo[];
			if (result != null)
				return result;
			string[] fileNames = data as string[];
			if (fileNames == null)
				return null;
			List<FileInfo> files = new List<FileInfo>();
			int count = fileNames.Length;
			for (int i = 0; i < count; i++)
				files.Add(new FileInfo(fileNames[i]));
			return files.ToArray();
		}
		protected internal override bool IsDataAvailable() {
#if !SL
			if (!PasteSource.ContainsData(OfficeDataFormats.FileDrop, true))
				return false;
			FileInfo[] files = GetFileDropListData();
			if (files == null || files.Length != 1)
				return false;
			return CanLoadFile(files[0].Name);
#else
			return true;
#endif
		}
	}
	#endregion
#if !SL
	#region PasteHtmlTextCommand
	public class PasteHtmlTextCommand : PasteContentConvertedToDocumentModelCommandBase {
		public PasteHtmlTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteHtmlText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteHtmlTextDescription; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteHtmlClipboardDataCommand(ActivePieceTable);
		}
	}
	public class PasteHtmlTextFromDragDropCommand : PasteContentConvertedToDocumentModelCommandBase {
		public PasteHtmlTextFromDragDropCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteHtmlDragAndDropDataCommand(ActivePieceTable);
		}
	}
	#endregion
	#region HtmlClipboardData
	public class HtmlClipboardData {
		string htmlContent = String.Empty;
		string sourceUri = String.Empty;
		int startFragment;
		int endFragment;
		public HtmlClipboardData(byte[] content) {
			if (content == null || content.Length == 0)
				return;
			Parse(content);
		}
		public string HtmlContent { get { return htmlContent; } }
		public string SourceUri { get { return sourceUri; } }
		public int StartFragment { get { return startFragment; } }
		public int EndFragment { get { return endFragment; } }
		protected internal virtual void Parse(byte[] content) {
			Dictionary<string, string> description = CreateContentDescription(content);
			int offset = CalculateHtmlContentOffset(description);
			int fragmentStart = CalculateFragmentStart(description);
			if (fragmentStart < 0)
				fragmentStart = offset;
			int fragmentEnd = CalculateFragmentEnd(description);
			if (fragmentEnd < 0)
				fragmentEnd = content.Length;
			if (offset < 0 && fragmentStart >= 0)
				offset = fragmentStart;
			if (offset < 0)
				return;
			this.htmlContent = Encoding.UTF8.GetString(content, offset, content.Length - offset);
			this.startFragment = Encoding.UTF8.GetCharCount(content, offset, fragmentStart - offset);
			this.endFragment = Encoding.UTF8.GetCharCount(content, offset, fragmentEnd - offset);
			this.sourceUri = CalculateSourceUri(description);
		}
		int CalculateFragmentStart(Dictionary<string, string> description) {
			return GetIntegerValue(description, "StartFragment");
		}
		int CalculateFragmentEnd(Dictionary<string, string> description) {
			return GetIntegerValue(description, "EndFragment");
		}
		protected internal Dictionary<string, string> CreateContentDescription(byte[] content) {
			Dictionary<string, string> result = new Dictionary<string, string>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			using (MemoryStream stream = new MemoryStream(content)) {
				StreamReader reader = new StreamReader(stream, Encoding.UTF8);
				while (!reader.EndOfStream) {
					string line = reader.ReadLine();
					if (string.IsNullOrEmpty(line)) continue;
					string[] pair = line.Split(':');
					if (pair.Length < 2)
						break;
					result[pair[0]] = line.Substring(pair[0].Length + 1);
				}
				return result;
			}
		}
		protected internal virtual int CalculateHtmlContentOffset(Dictionary<string, string> description) {
			return GetIntegerValue(description, "StartHTML");
		}
		int GetIntegerValue(Dictionary<string, string> description, string key) {
			string value;
			if (!description.TryGetValue(key, out value))
				return -1;
			try {
				return Convert.ToInt32(value);
			}
			catch {
				return -1;
			}
		}
		protected internal virtual string CalculateSourceUri(Dictionary<string, string> description) {
			string value;
			if (!description.TryGetValue("SourceURL", out value))
				return String.Empty;
			else
				return value;
		}
	}
	#endregion
	#region PasteImageCommand
	public class PasteImageCommand : PasteContentCommandBase {
		public PasteImageCommand(IRichEditControl control)
			: base(control) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteImage; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteImageDescription; } }
		protected internal override void ModifyModel() {
			Image image = AquireImage();
			if (image != null)
				InsertPicture(new MemoryStreamBasedImage(image, null));
		}
		protected internal virtual Image AquireImage() {
			return PasteSource.GetData(OfficeDataFormats.Bitmap, true) as Image;
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.Bitmap, true);
		}
		protected internal virtual bool InsertPicture(MemoryStreamBasedImage image) {
			Rectangle bounds = GetCurrentColumnBounds();
			return InsertPicture(image, bounds);
		}
		protected internal virtual bool InsertPicture(MemoryStreamBasedImage image, Rectangle columnBounds) {
			if (image == null || image.Image == null)
				return false;
			if (!DocumentModel.DocumentCapabilities.InlinePicturesAllowed)
				return false;
			int scale = CalculateImageScale(image.Image, columnBounds);
			InsertPictureCore(image, scale, scale);
			return true;
		}
		protected internal virtual int GetImageScale(Size imageSizeInLayoutUnits, Rectangle columnBounds) {
			float scaleX = 1F;
			float scaleY = 1F;
			if (imageSizeInLayoutUnits.Width > columnBounds.Width)
				scaleX = (float)columnBounds.Width / (float)imageSizeInLayoutUnits.Width;
			if (imageSizeInLayoutUnits.Height > columnBounds.Height)
				scaleY = (float)columnBounds.Height / (float)imageSizeInLayoutUnits.Height;
			float scale = Math.Min(scaleX, scaleY);
			return (int)(scale * 100);
		}
		protected internal int CalculateImageScale(Image image, Rectangle columnBounds) {
			if(ActiveView is SimpleView)
				return 100;
			float dpiX = (image.RawFormat.Equals(ImageFormat.Emf)) ? (float)MetafileHelper.MetafileResolution : image.HorizontalResolution;
			float dpiY = (image.RawFormat.Equals(ImageFormat.Emf)) ? (float)MetafileHelper.MetafileResolution : image.VerticalResolution;
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			int imageWidthInLayoutUnits = unitConverter.PixelsToLayoutUnits(image.Width, dpiX);
			int imageHeightInLayoutUnits = unitConverter.PixelsToLayoutUnits(image.Height, dpiY);
			return GetImageScale(new Size(imageWidthInLayoutUnits, imageHeightInLayoutUnits), columnBounds);
		}
		protected internal virtual Rectangle GetCurrentColumnBounds() {
			return GetCurrentColumnBounds(ActiveView.CaretPosition);
		}
		protected internal static Rectangle GetCurrentColumnBounds(CaretPosition caretPosition) {
			if (caretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return caretPosition.LayoutPosition.Column.Bounds;
			else
				return new Rectangle(0, 0, int.MaxValue, int.MaxValue);
		}
		protected internal virtual void InsertPictureCore(MemoryStreamBasedImage image, int scaleX, int scaleY) {
			OfficeImage richEditImage = DocumentModel.CreateImage(image);
			ActivePieceTable.InsertInlinePicture(DocumentModel.Selection.End, richEditImage, scaleX, scaleY, GetForceVisible());
		}
	}
	#endregion
	#region PasteMetafileCommand
	public class PasteMetafileCommand : PasteImageCommand {
		public PasteMetafileCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteMetafileImage; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteMetafileImageDescription; } }
		protected internal override Image AquireImage() {
#if !DXPORTABLE
			return MetafileHelper.GetEnhMetafileFromClipboard();
#else
			return null;
#endif
		}
		protected internal override bool IsDataAvailable() {
			return OfficeClipboard.ContainsData(OfficeDataFormats.EnhancedMetafile);
		}
	}
#endregion
#else
			#region PasteSelectionCoreCommand
		public class PasteInternalSelectionCoreCommand : PasteSelectionCoreCommand {
		public PasteInternalSelectionCoreCommand(IRichEditControl control, PasteSource pasteSource) : base(control, pasteSource) { }
		protected internal override void CreateCommands() {
			AddCommand(new PasteInternalRtfTextCommand(Control));
		}
	}
	#endregion
	#region PasteInternalRtfTextCommand
	public class PasteInternalRtfTextCommand : PasteRtfTextCommand {
		public PasteInternalRtfTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteRtfText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PasteRtfTextDescription; } }
		protected internal override PieceTablePasteContentConvertedToDocumentModelCommandBase CreateInnerCommandCore() {
			return new PieceTablePasteInternalRtfTextCommand(ActivePieceTable);
		}
	}
	#endregion
#endif
	#region PasteExternalMultilineTextCoreCommand
	public class PasteExternalMultilineTextCoreCommand : PastePlainTextCommand {
		string text;
		public PasteExternalMultilineTextCoreCommand(IRichEditControl control, string text)
			: base(control) {
			this.text = text;
		}
		public string Text { get { return text; } set { text = value; } }
		protected internal override string GetTextData() {
			return Text;
		}
		protected internal override bool IsDataAvailable() {
			return !String.IsNullOrEmpty(GetTextData());
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Model {
	public abstract class PieceTablePasteContentCommand : PieceTableCommand {
		PasteSource pasteSource;
		protected PieceTablePasteContentCommand(PieceTable pieceTable)
			: base(pieceTable) {
			this.pasteSource = new EmptyPasteSource();
		}
		public abstract DocumentFormat Format { get; }
		public PasteSource PasteSource { get { return pasteSource; } set { pasteSource = value; } }
		protected internal abstract bool IsDataAvailable();
	}
	public abstract class PieceTablePasteContentConvertedToDocumentModelCommandBase : PieceTablePasteContentCommand {
		bool forceInsertFloatingObjectAtParagraphStart;
		InsertOptions insertOptions;
		protected PieceTablePasteContentConvertedToDocumentModelCommandBase(PieceTable pieceTable, InsertOptions insertOptions)
			: base(pieceTable) {
			this.insertOptions = insertOptions;
		}
		protected PieceTablePasteContentConvertedToDocumentModelCommandBase(PieceTable pieceTable)
			: base(pieceTable) {
			this.insertOptions = DocumentModel.CopyPasteOptions.InsertOptions;
		}
		public bool ForceInsertFloatingObjectAtParagraphStart { get { return forceInsertFloatingObjectAtParagraphStart; } set { forceInsertFloatingObjectAtParagraphStart = value; } }
		public bool CopyBetweenInternalModels { get; set; }
		public bool PasteFromIE { get; set; }
		public InsertOptions InsertOptions { get { return insertOptions; } }
		protected internal override void CalculateExecutionParameters() {
		}
		protected internal override void ExecuteCore() {
			string sizeCollection = GetAdditionalContentString();
			DocumentModel source = CreateSourceDocumentModel(sizeCollection);
			PasteContent(source, DocumentModel.Selection.End, sizeCollection);
		}
		protected internal override void CalculateApplyChangesParameters() {
		}
		protected internal override void ApplyChanges() {
		}
		protected internal virtual string GetAdditionalContentString() {
			return string.Empty;
		}
		protected internal virtual void PasteContent(DocumentModel source, DocumentLogPosition pos, string sizeCollection) {
			if (source == null)
				return;
			Paragraph anchorRunParagraph = null;
			if (forceInsertFloatingObjectAtParagraphStart) {
				FloatingObjectAnchorRun anchorRun = GetDocumentModelSingleFloatingObjectAnchorRun(source);
				if (anchorRun != null) {
					anchorRunParagraph = PreprocessSingleFloatingObjectInsertion(anchorRun);
					pos = anchorRunParagraph.LogPosition;
				}
			}
			source.PreprocessContentBeforeInsert(PieceTable, pos);
			PasteDocumentModel(pos, source, PasteFromIE);
		}
		Paragraph PreprocessSingleFloatingObjectInsertion(FloatingObjectAnchorRun run) {
			DocumentModelPosition modelPosition = DocumentModel.Selection.Interval.End;
			PieceTable pieceTable = modelPosition.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[modelPosition.ParagraphIndex];
			OffsetNewlyInsertedFloatingObjectIfNeed(run, paragraph);
			return paragraph;
		}
		void OffsetNewlyInsertedFloatingObjectIfNeed(FloatingObjectAnchorRun newRun, Paragraph paragraph) {
			RunIndex lastRunIndex = paragraph.LastRunIndex;
			TextRunCollection runs = paragraph.PieceTable.Runs;
			for (RunIndex i = paragraph.FirstRunIndex; i < lastRunIndex; i++) {
				FloatingObjectAnchorRun run = runs[i] as FloatingObjectAnchorRun;
				if (run == null)
					break;
				if (ShouldOffsetNewRun(newRun, run)) {
					OffsetNewlyInsertedFloatingObject(newRun, run);
					i = paragraph.FirstRunIndex - 1; 
				}
			}
		}
		void OffsetNewlyInsertedFloatingObject(FloatingObjectAnchorRun newRun, FloatingObjectAnchorRun run) {
			FloatingObjectProperties properties = newRun.FloatingObjectProperties;
			Point offset = properties.Offset;
			int shift = DocumentModel.UnitConverter.DocumentsToModelUnits(50);
			offset.X += shift;
			offset.Y += shift;
			properties.Offset = offset;
			properties.ZOrder = Math.Max(properties.ZOrder, run.FloatingObjectProperties.ZOrder + 1);
		}
		bool ShouldOffsetNewRun(FloatingObjectAnchorRun newRun, FloatingObjectAnchorRun run) {
			FloatingObjectProperties newFloatingObjectProperties = newRun.FloatingObjectProperties;
			FloatingObjectProperties floatingObjectProperties = run.FloatingObjectProperties;
			return newFloatingObjectProperties.Offset == floatingObjectProperties.Offset &&
				newFloatingObjectProperties.HorizontalPositionType == floatingObjectProperties.HorizontalPositionType &&
				newFloatingObjectProperties.VerticalPositionType == floatingObjectProperties.VerticalPositionType;
		}
		FloatingObjectAnchorRun GetDocumentModelSingleFloatingObjectAnchorRun(DocumentModel model) {
			PieceTable pieceTable = model.MainPieceTable;
			if (pieceTable.Runs.Count == 2 && pieceTable.Runs[RunIndex.Zero] is FloatingObjectAnchorRun)
				return (FloatingObjectAnchorRun)pieceTable.Runs[RunIndex.Zero];
			else
				return null;
		}
		protected internal virtual void PasteDocumentModel(DocumentLogPosition pos, DocumentModel source, bool pasteFromIE) {
			PasteDocumentModel(pos, source, false, pasteFromIE);
		}
		protected internal virtual void PasteDocumentModel(DocumentLogPosition pos, DocumentModel source, bool suppressFieldsUpdate, bool pasteFromIE) {
			PieceTableInsertContentConvertedToDocumentModelCommand command = CreatePasteDocumentModelCommand(pos, source, suppressFieldsUpdate, pasteFromIE);
			command.Execute();
		}
		protected internal virtual PieceTableInsertContentConvertedToDocumentModelCommand CreatePasteDocumentModelCommand(DocumentLogPosition pos, DocumentModel source, bool suppressFieldsUpdate, bool pasteFromIE) {
			PieceTableInsertContentConvertedToDocumentModelCommand command = new PieceTableInsertContentConvertedToDocumentModelCommand(PieceTable, source, pos, insertOptions, false);
			command.SuppressFieldsUpdate = suppressFieldsUpdate;
			command.CopyBetweenInternalModels = CopyBetweenInternalModels;
			command.PasteFromIE = pasteFromIE;
			command.SuppressCopySectionProperties = SuppressCopySectionProperties(source);
			return command;
		}
		protected virtual bool SuppressCopySectionProperties(DocumentModel source) {
			ParagraphCollection paragraphs = source.MainPieceTable.Paragraphs;
			Paragraph lastParagraph = paragraphs.Last;
			bool shouldPreserveTableProperties = paragraphs.Count > 1 && paragraphs[new ParagraphIndex(paragraphs.Count - 2)].GetCell() != null;
			return !lastParagraph.IsEmpty || shouldPreserveTableProperties || DocumentModel.CopyPasteOptions.MaintainDocumentSectionSettings;
		}
		protected internal virtual void SetSuppressStoreImageSize(DocumentModel documentModel, string sizeCollection) {
			if (sizeCollection != null) {
				TextRunCollection runs = PieceTable.Runs;
				int sizeIndex = 0;
				for (int i = 0; i < runs.Count; i++) {
					InlinePictureRun pictureRun = runs[new RunIndex(i)] as InlinePictureRun;
					try {
						string[] sizes = sizeCollection.Split(new char[] { ',', ';' });
						if (pictureRun != null) {
							if (InternalOfficeImageHelper.GetSuppressStore(pictureRun.Image)) {
								pictureRun.ScaleX = Convert.ToInt32(sizes[sizeIndex]);
								pictureRun.ScaleY = Convert.ToInt32(sizes[sizeIndex + 1]);
								sizeIndex++;
							}
						}
					}
					catch {
					}
				}
			}
		}
		protected internal abstract DocumentModel CreateSourceDocumentModel(string sizeCollection);
	}
	public class ClipboardStringContent {
		public ClipboardStringContent(string content) {
			StringContent = content;
		}
		public string StringContent { get; set; }
	}
	public abstract class PieceTablePasteTextContentConvertedToDocumentModelCommandBase : PieceTablePasteContentConvertedToDocumentModelCommandBase {
		protected PieceTablePasteTextContentConvertedToDocumentModelCommandBase(PieceTable pieceTable, InsertOptions insertOptions)
			: base(pieceTable, insertOptions) {
		}
		protected PieceTablePasteTextContentConvertedToDocumentModelCommandBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal virtual void PasteContent(ClipboardStringContent content, DocumentLogPosition pos, string sizeCollection) {
			DocumentModel source = CreateDocumentModelFromContentString(content, sizeCollection);
			PasteContent(source, pos, sizeCollection);
		}
		protected internal override DocumentModel CreateSourceDocumentModel(string sizeCollection) {
			ClipboardStringContent content = GetContent();
			return CreateDocumentModelFromContentString(content, sizeCollection);
		}
		protected internal virtual DocumentModel CreateDocumentModelFromContentString(ClipboardStringContent content, string sizeCollection) {
			if (content == null || String.IsNullOrEmpty(content.StringContent))
				return null;
			DocumentModel result = PieceTable.DocumentModel.CreateNew();
			result.FieldOptions.CopyFrom(PieceTable.DocumentModel.FieldOptions);
			result.DocumentCapabilities.CopyFrom(PieceTable.DocumentModel.DocumentCapabilities);
			result.IntermediateModel = true;
			if (!PieceTable.IsMain) {
				result.DocumentCapabilities.HeadersFooters = DocumentCapability.Disabled;
				result.DocumentCapabilities.Sections = DocumentCapability.Disabled;
			}
			PopulateDocumentModelFromContentStringCore(result, content, sizeCollection);
			return result;
		}
		protected internal abstract ClipboardStringContent GetContent();
		protected internal abstract void PopulateDocumentModelFromContentStringCore(DocumentModel documentModel, ClipboardStringContent content, string sizeCollection);
	}
	public class PieceTablePasteRtfTextCommand : PieceTablePasteTextContentConvertedToDocumentModelCommandBase {
		public PieceTablePasteRtfTextCommand(PieceTable pieceTable, InsertOptions insertOptions)
			: base(pieceTable, insertOptions) {
		}
		public PieceTablePasteRtfTextCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Rtf; } }
		protected internal override ClipboardStringContent GetContent() {
			return new ClipboardStringContent(PasteSource.GetData(OfficeDataFormats.Rtf) as string);
		}
		protected internal override string GetAdditionalContentString() {
			return PasteSource.GetData(OfficeDataFormats.SuppressStoreImageSize) as string;
		}
		protected internal override void PopulateDocumentModelFromContentStringCore(DocumentModel documentModel, ClipboardStringContent content, string sizeCollection) {
			documentModel.DeleteDefaultNumberingList(documentModel.NumberingLists);
			RtfDocumentImporterOptions options = new RtfDocumentImporterOptions();
			options.SuppressLastParagraphDelete = true;
			options.CopySingleCellAsText = DocumentModel.BehaviorOptions.PasteSingleCellAsText;
			options.LineBreakSubstitute = DocumentModel.BehaviorOptions.PasteLineBreakSubstitution;
			options.PasteFromIE = GetIsPasteFromIe();
			PasteFromIE = options.PasteFromIE;
			using (IRtfImporter importer = documentModel.InternalAPI.ImporterFactory.CreateRtfImporter(documentModel, options)) {
				importer.UpdateFields = documentModel.FieldOptions.UpdateFieldsOnPaste;
				importer.Import(PrepareInputStream(content.StringContent));
			}
			SetSuppressStoreImageSize(documentModel, sizeCollection);
		}
		private bool GetIsPasteFromIe() {
			return PasteSource.ContainsData(OfficeDataFormats.MsSourceUrl);
		}
		protected internal Stream PrepareInputStream(string str) {
#if!SL
			Encoding encoding = DXEncoding.Default;
			if (!encoding.IsSingleByte)
				encoding = EmptyEncoding.Instance;
			int maxLength = 32768;
			if (str.Length < maxLength)
				return new MemoryStream(encoding.GetBytes(str));
			ChunkedMemoryStream stream = new ChunkedMemoryStream();
			int offset = 0;
			byte[] buffer = new byte[maxLength*2];
			while (offset < str.Length) {
				int length = Math.Min(maxLength/2, str.Length - offset);
				int bufferLength = encoding.GetBytes(str, offset, length, buffer, 0);
				stream.Write(buffer, 0, bufferLength);
				offset += length;
			}
			stream.Position = 0;
			return stream;
#else
			return new MemoryStream(EmptyEncoding.Instance.GetBytes(str));
#endif
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.Rtf);
		}
	}
	public class PieceTablePasteSilverlightXamlTextCommand : PieceTablePasteTextContentConvertedToDocumentModelCommandBase {
		public PieceTablePasteSilverlightXamlTextCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Xaml; } }
		protected internal override ClipboardStringContent GetContent() {
			MemoryStream stream = PasteSource.GetData(OfficeDataFormats.SilverlightXaml) as MemoryStream;
			if (stream == null)
				return new ClipboardStringContent(String.Empty);
			using (StreamReader reader = new StreamReader(stream)) {
				return new ClipboardStringContent(reader.ReadToEnd());
			}
		}
		protected internal override void PopulateDocumentModelFromContentStringCore(DocumentModel documentModel, ClipboardStringContent content, string sizeCollection) {
			documentModel.DeleteDefaultNumberingList(documentModel.NumberingLists);
			XamlDocumentImporterOptions options = new XamlDocumentImporterOptions();
			options.LineBreakSubstitute = DocumentModel.BehaviorOptions.PasteLineBreakSubstitution;
			IXamlImporter importer = documentModel.InternalAPI.ImporterFactory.CreateXamlImporter(documentModel, options);
			importer.Import(PrepareInputStream(content.StringContent));
			SetSuppressStoreImageSize(documentModel, sizeCollection);
		}
		protected internal Stream PrepareInputStream(string str) {
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.SilverlightXaml);
		}
	}
	public class PieceTablePasteHtmlTextCommand : PieceTablePasteTextContentConvertedToDocumentModelCommandBase {
		public PieceTablePasteHtmlTextCommand(PieceTable pieceTable, InsertOptions insertOptions)
			: base(pieceTable, insertOptions) {
		}
		public PieceTablePasteHtmlTextCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override DocumentFormat Format { get { return DocumentFormat.Html; } }
		protected internal override ClipboardStringContent GetContent() {
			return new ClipboardStringContent(String.Empty);
		}
		protected internal override void PopulateDocumentModelFromContentStringCore(DocumentModel documentModel, ClipboardStringContent content, string sizeCollection) {
			HtmlDocumentImporterOptions options = new HtmlDocumentImporterOptions();
			options.SourceUri = String.Empty;
			options.Encoding = Encoding.UTF8;
			options.IgnoreMetaCharset = true;
			options.LineBreakSubstitute = DocumentModel.BehaviorOptions.PasteLineBreakSubstitution;
			documentModel.InheritService<IUriStreamService>(PieceTable.DocumentModel);
			IHtmlImporter importer = documentModel.InternalAPI.ImporterFactory.CreateHtmlImporter(documentModel, options);
			importer.Import(PrepareInputStream(content.StringContent));
		}
		protected internal Stream PrepareInputStream(string str) {
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}
		protected internal override bool IsDataAvailable() {
			return false;
		}
	}
#if !SL
	public class HtmlClipboardStringContent : ClipboardStringContent {
		public HtmlClipboardStringContent(string sourseUri, string content, int fragmentStart, int fragmentEnd)
			: base(content) {
			SourceUri = sourseUri;
			StartFragment = fragmentStart;
			EndFragment = fragmentEnd;
		}
		public string SourceUri { get; set; }
		public int StartFragment { get; set; }
		public int EndFragment { get; set; }
	}
	public class PieceTablePasteHtmlDragAndDropDataCommand : PieceTablePasteHtmlClipboardDataCommand {
		public PieceTablePasteHtmlDragAndDropDataCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override ClipboardStringContent GetContent() {
			object data = PasteSource.GetData(OfficeDataFormats.Html);
			byte[] bytes = data as byte[];
			if (bytes == null)
				bytes = Encoding.UTF8.GetBytes(data as string);
			HtmlClipboardData htmlData = new HtmlClipboardData(bytes);
			return new HtmlClipboardStringContent(htmlData.SourceUri, htmlData.HtmlContent, htmlData.StartFragment, htmlData.EndFragment);
		}
		protected internal override bool IsDataAvailable() {
			return PasteSource.ContainsData(OfficeDataFormats.Html);
		}
	}
	public class PieceTablePasteHtmlClipboardDataCommand : PieceTablePasteHtmlTextCommand {
		readonly static string StartFragmentCommentText = "StartFragment";
		readonly static string EndFragmentCommentText = "EndFragment";
		readonly static string StartFragmentComment = CreateHTMLComment(StartFragmentCommentText);
		readonly static string EndFragmentComment = CreateHTMLComment(EndFragmentCommentText);
		static string CreateHTMLComment(string commentText) {
			return String.Format("<!--{0}-->", commentText);
		}
		public PieceTablePasteHtmlClipboardDataCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		[System.Security.SecuritySafeCritical]
		protected internal override ClipboardStringContent GetContent() {
#if !DXPORTABLE
			int CF_HTML = Win32.RegisterClipboardFormat("HTML Format");
			if (!Win32.IsClipboardFormatAvailable(CF_HTML))
				return new ClipboardStringContent(String.Empty);
			if (!Win32.OpenClipboard(IntPtr.Zero))
				return new ClipboardStringContent(String.Empty);
			IntPtr hMem = Win32.GetClipboardData(CF_HTML);
			IntPtr pData = Win32.GlobalLock(hMem);
			try {
				int length = Win32.GlobalSize(hMem).ToInt32();
				byte[] bytes = new byte[length];
				Marshal.Copy(pData, bytes, 0, length);
				HtmlClipboardData htmlData = new HtmlClipboardData(bytes);
				return new HtmlClipboardStringContent(htmlData.SourceUri, htmlData.HtmlContent, htmlData.StartFragment, htmlData.EndFragment);
			}
			finally {
				Win32.GlobalUnlock(hMem);
				Win32.CloseClipboard();
			}
#else
			return null;
#endif
		}
		protected internal override void PopulateDocumentModelFromContentStringCore(DocumentModel documentModel, ClipboardStringContent content, string sizeCollection) {
			HtmlClipboardStringContent htmlContent = (HtmlClipboardStringContent)content;
			HtmlDocumentImporterOptions options = new HtmlDocumentImporterOptions();
			options.SourceUri = htmlContent.SourceUri;
			options.Encoding = Encoding.UTF8;
			options.LineBreakSubstitute = DocumentModel.BehaviorOptions.PasteLineBreakSubstitution;
			DocumentModel tempModel = documentModel.CreateNew();
			tempModel.InheritServicesForImport(DocumentModel);
			IHtmlImporter importer = DocumentModel.InternalAPI.ImporterFactory.CreateHtmlImporter(tempModel, options);
			importer.RegisterCommentMarksToCollectPositions(StartFragmentCommentText, EndFragmentCommentText);
			string stringContent = ClearHTMLContent(htmlContent.StringContent, htmlContent.StartFragment, htmlContent.EndFragment);
			importer.Import(PrepareInputStream(stringContent));
			DocumentLogPosition startFragmentPos = importer.GetMarkPosition(StartFragmentCommentText);
			DocumentLogPosition endFragmentPos = importer.GetMarkPosition(EndFragmentCommentText);
			CopyFragment(tempModel, documentModel, startFragmentPos, endFragmentPos);
		}
		void CopyFragment(DocumentModel sourceModel, DocumentModel targetModel, DocumentLogPosition startFragmentPos, DocumentLogPosition endFragmentPos) {
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(sourceModel.MainPieceTable, targetModel.MainPieceTable, ParagraphNumerationCopyOptions.CopyAlways);
			CopySectionOperation operation = sourceModel.CreateCopySectionOperation(copyManager);
			operation.Execute(startFragmentPos, endFragmentPos - startFragmentPos, false);
		}
		string ClearHTMLContent(string htmlContent, int startFragent, int endFragment) {
			string partBefore = RemoveFragmentMarks(htmlContent.Substring(0, startFragent));
			string middlePart = RemoveFragmentMarks(htmlContent.Substring(startFragent, endFragment - startFragent));
			string partAfter = RemoveFragmentMarks(htmlContent.Substring(endFragment));
			return partBefore + StartFragmentComment + middlePart + EndFragmentComment + partAfter;
		}
		string RemoveFragmentMarks(string content) {
			return content.Replace(StartFragmentComment, String.Empty).Replace(EndFragmentComment, String.Empty); ;
		}
		protected internal override bool IsDataAvailable() {
			return OfficeClipboard.ContainsData(OfficeDataFormats.Html);
		}
		protected override bool SuppressCopySectionProperties(DocumentModel source) {
			return true;
		}
	}
#else
			#region PieceTablePasteInternalRtfTextCommand
		public class PieceTablePasteInternalRtfTextCommand : PieceTablePasteRtfTextCommand {
		public PieceTablePasteInternalRtfTextCommand(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override ClipboardStringContent GetContent() {
			return new ClipboardStringContent(OfficeClipboard.rtfClipboard);
		}
		protected internal override bool IsDataAvailable() {
			return !string.IsNullOrEmpty(OfficeClipboard.rtfClipboard);
		}
	}
	#endregion
#endif
}
