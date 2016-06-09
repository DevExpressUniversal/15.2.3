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
using System.IO;
using System.ComponentModel;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Options;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.API.Native;
using ApiMailMergeOptions = DevExpress.XtraRichEdit.API.Native.MailMergeOptions;
namespace DevExpress.Snap.Core.Native {
	public class InternalSnapDocumentServer : InternalRichEditDocumentServer, ISnapDocumentServer, IInnerSnapDocumentServerOwner {
		internal InternalSnapDocumentServer()
			: this(SnapDocumentFormatsDependecies.CreateDocumentFormatsDependencies()) {
		}
		internal InternalSnapDocumentServer(DocumentFormatsDependencies documentFormatsDependencies)
			: base(documentFormatsDependencies) {
		}
		internal InternalSnapDocumentServer(SnapDocumentModel documentModel)
			: base(documentModel) {
		}
		public new InnerSnapDocumentServer InnerServer { get { return (InnerSnapDocumentServer)base.InnerServer; } }
		protected internal new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public new SnapDocument Document { get { return (SnapDocument)base.Document; } }
		#region SnxBytes
		public byte[] SnxBytes {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.SnxBytes;
				return null;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.SnxBytes = value;
			}
		}
		#endregion
		#region Events
		#region BeforeConversion
		public event BeforeConversionEventHandler BeforeConversion {
			add {
				if (InnerServer != null)
					InnerServer.BeforeConversion += value;
			}
			remove {
				if (InnerServer != null)
					InnerServer.BeforeConversion -= value;
			}
		}
		#endregion
		#region BeforeDataSourceExport
		public event BeforeDataSourceExportEventHandler BeforeDataSourceExport {
			add {
				if (InnerServer != null)
					InnerServer.BeforeDataSourceExport += value;
			}
			remove {
				if (InnerServer != null)
					InnerServer.BeforeDataSourceExport -= value;
			}
		}
		#endregion
		#region AfterDataSourceImport
		public event AfterDataSourceImportEventHandler AfterDataSourceImport {
			add {
				if (InnerServer != null)
					InnerServer.AfterDataSourceImport += value;
			}
			remove {
				if (InnerServer != null)
					InnerServer.AfterDataSourceImport -= value;
			}
		}
		#endregion
		#region DataSourceChanged
		public event EventHandler DataSourceChanged {
			add {
				if (InnerServer != null)
					InnerServer.DataSourceChanged += value;
			}
			remove {
				if (InnerServer != null)
					InnerServer.DataSourceChanged -= value;
			}
		}
		#endregion
		#region AsynchronousOperationStarted
		public event EventHandler AsynchronousOperationStarted {
			add {
				if (InnerServer != null)
					InnerServer.AsynchronousOperationStarted += value;
			}
			remove {
				if (InnerServer != null)
					InnerServer.AsynchronousOperationStarted -= value;
			}
		}
		#endregion
		#region AsynchronousOperationFinished
		public event EventHandler AsynchronousOperationFinished {
			add {
				if (InnerServer != null)
					InnerServer.AsynchronousOperationFinished += value;
			}
			remove {
				if (InnerServer != null)
					InnerServer.AsynchronousOperationFinished -= value;
			}
		}
		#endregion
		#region SnapMailMergeStarted
		public event SnapMailMergeStartedEventHandler SnapMailMergeStarted {
			add { if (InnerServer != null) InnerServer.SnapMailMergeStarted += value; }
			remove { if (InnerServer != null) InnerServer.SnapMailMergeStarted -= value; }
		}
		#endregion
		#region SnapMailMergeRecordStarted
		public event SnapMailMergeRecordStartedEventHandler SnapMailMergeRecordStarted {
			add { if (InnerServer != null) InnerServer.SnapMailMergeRecordStarted += value; }
			remove { if (InnerServer != null) InnerServer.SnapMailMergeRecordStarted -= value; }
		}
		#endregion
		#region SnapMailMergeRecordFinished
		public event SnapMailMergeRecordFinishedEventHandler SnapMailMergeRecordFinished {
			add { if (InnerServer != null) InnerServer.SnapMailMergeRecordFinished += value; }
			remove { if (InnerServer != null) InnerServer.SnapMailMergeRecordFinished -= value; }
		}
		#endregion
		#region SnapMailMergeFinished
		public event SnapMailMergeFinishedEventHandler SnapMailMergeFinished {
			add { if (InnerServer != null) InnerServer.SnapMailMergeFinished += value; }
			remove { if (InnerServer != null) InnerServer.SnapMailMergeFinished -= value; }
		}
		#endregion
		#region SnapMailMergeActiveRecordChanging
		public event EventHandler SnapMailMergeActiveRecordChanging {
			add { if (InnerServer != null) InnerServer.SnapMailMergeActiveRecordChanging += value; }
			remove { if (InnerServer != null) InnerServer.SnapMailMergeActiveRecordChanging -= value; }
		}
		#endregion
		#region SnapMailMergeActiveRecordChanged
		public event EventHandler SnapMailMergeActiveRecordChanged {
			add { if (InnerServer != null) InnerServer.SnapMailMergeActiveRecordChanging += value; }
			remove { if (InnerServer != null) InnerServer.SnapMailMergeActiveRecordChanged -= value; }
		}
		#endregion
		#endregion
		protected internal override InnerRichEditDocumentServer CreateInnerServer(XtraRichEdit.Model.DocumentModel documentModel) {
			if (documentModel == null)
				return new InnerSnapDocumentServer(this);
			return new InnerSnapDocumentServer(this, (SnapDocumentModel)documentModel);
		}
		#region DocumentClosing
		public new event DocumentClosingEventHandler DocumentClosing {
			add { if (InnerServer != null) InnerServer.DocumentClosing += value; }
			remove { if (InnerServer != null) InnerServer.DocumentClosing -= value; }
		}
		#endregion
		#region EmptyDocumentCreated
		public new event DocumentImportedEventHandler EmptyDocumentCreated {
			add { if (InnerServer != null) InnerServer.EmptyDocumentCreated += value; }
			remove { if (InnerServer != null) InnerServer.EmptyDocumentCreated -= value; }
		}
		#endregion
		#region DocumentLoaded
		public new event DocumentImportedEventHandler DocumentLoaded {
			add { if (InnerServer != null) InnerServer.DocumentLoaded += value; }
			remove { if (InnerServer != null) InnerServer.DocumentLoaded -= value; }
		}
		#endregion
		#region Snap Mail Merge
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeStartedEventHandler MailMergeStarted {
			add { base.MailMergeStarted += value; }
			remove { base.MailMergeStarted -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeRecordStartedEventHandler MailMergeRecordStarted {
			add { base.MailMergeRecordStarted += value; }
			remove { base.MailMergeRecordStarted -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeRecordFinishedEventHandler MailMergeRecordFinished {
			add { base.MailMergeRecordFinished += value; }
			remove { base.MailMergeRecordFinished -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This event is not appropriate in Snap and has been rendered obsolete.")]
		public new event MailMergeFinishedEventHandler MailMergeFinished {
			add { base.MailMergeFinished += value; }
			remove { base.MailMergeFinished -= value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new ApiMailMergeOptions CreateMailMergeOptions() {
			return base.CreateMailMergeOptions();
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(Document document) {
			base.MailMerge(document);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, Document targetDocument) {
			base.MailMerge(options, targetDocument);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(IRichEditDocumentServer documentServer) {
			base.MailMerge(documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer) {
			base.MailMerge(options, targetDocumentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(string fileName, DocumentFormat format) {
			base.MailMerge(fileName, format);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(Stream stream, DocumentFormat format) {
			base.MailMerge(stream, format);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format) {
			base.MailMerge(options, fileName, format);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		public new void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format) {
			base.MailMerge(options, stream, format);
		}
		public SnapMailMergeExportOptions CreateSnapMailMergeExportOptions() {
			if (InnerServer != null)
				return InnerServer.CreateSnapMailMergeExportOptions();
			else if (DocumentModel != null)
				return new NativeSnapMailMergeExportOptions(DocumentModel, CreateInnerServer(DocumentModel));
			return null;
		}
		public void SnapMailMerge(SnapDocument document) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(document);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, SnapDocument targetDocument) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(options, targetDocument);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		public void SnapMailMerge(ISnapDocumentServer documentServer) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another SnapMailMerge method overload with appropriate parameters instead.")]
		public void SnapMailMerge(SnapMailMergeExportOptions options, ISnapDocumentServer targetDocumentServer) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(options, targetDocumentServer);
		}
		public void SnapMailMerge(string fileName, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(fileName, format);
		}
		public void SnapMailMerge(Stream stream, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(stream, format);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, string fileName, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(options, fileName, format);
		}
		public void SnapMailMerge(SnapMailMergeExportOptions options, Stream stream, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.SnapMailMerge(options, stream, format);
		}
		#endregion
		protected internal override RichEditControlOptionsBase CreateOptionsCore(InnerRichEditDocumentServer documentServer) {
			return new SnapDocumentServerOptions(documentServer);
		}
	}
	public class SnapDocumentServerOptions : RichEditDocumentServerOptions, ISnapControlOptions {
		internal SnapDocumentServerOptions(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		public new SnapFieldOptions Fields { get { return (SnapFieldOptions)base.Fields; } }
		#region SnapMailMergeVisualOptions
		[
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter("DevExpress.Snap.Core.Native.Options.NativeSnapMailMergeOptionsTypeConverter," + AssemblyInfo.SRAssemblySnapCore)]
		public SnapMailMergeVisualOptions SnapMailMergeVisualOptions {
			get {
				if (DocumentServer == null)
					return null;
				return ((SnapDocumentModel)DocumentServer.DocumentModel).SnapMailMergeVisualOptions;
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This member is deprecated and marked for removal from Snap. Use the SnapMailMergeVisualOptions property instead.")]
		public new RichEditMailMergeOptions MailMerge {
			get { return base.MailMerge; }
		}
		protected internal override void SubscribeInnerOptionsEvents() {
			base.SubscribeInnerOptionsEvents();
			RichEditNotificationOptions mailMergeOptions = SnapMailMergeVisualOptions as RichEditNotificationOptions;
			if (mailMergeOptions != null)
				mailMergeOptions.Changed += OnInnerOptionsChanged;
		}
		protected internal override void UnsubscribeInnerOptionsEvents() {
			base.UnsubscribeInnerOptionsEvents();
			RichEditNotificationOptions mailMergeOptions = SnapMailMergeVisualOptions as RichEditNotificationOptions;
			if (mailMergeOptions != null)
				mailMergeOptions.Changed -= OnInnerOptionsChanged;
		}
	}
}
