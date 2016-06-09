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
using System.Threading;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Services;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Commands {
	public abstract class FinishAndMergeCommandBase : SnapMenuItemSimpleCommand {
		protected class MailMergeCommandData {
			byte[] documentBytes;
			SnapMailMergeExportOptions options;
			public MailMergeCommandData(byte[] documentBytes, SnapMailMergeExportOptions options) {
				this.documentBytes = documentBytes;
				this.options = options;
			}
			public byte[] DocumentBytes { get { return documentBytes; } }
			public SnapMailMergeExportOptions Options { get { return options; } }
		}
		protected FinishAndMergeCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option) {
			base.ApplyCommandsRestriction(state, option);
			state.Enabled = state.Enabled && IsMailMergeAvailable();
		}
		protected override bool CheckIsContentEditable() {
			return DocumentServer.Enabled;
		}
		bool IsMailMergeAvailable() {
			return DocumentModel.SnapMailMergeVisualOptions.IsMailMergeEnabled && !DocumentModel.AsynchronousOperationPerforming;
		}
		protected internal override void ExecuteCore() {
			if (DocumentModel.AsynchronousOperationPerforming)
				SnapExceptions.ThrowInvalidOperationException(Localization.SnapStringId.Msg_CannotPerformAsynchronousOperation);
			SnapMailMergeExportOptions options = ((SnapDocument)Control.Document).CreateSnapMailMergeExportOptions();
			if (options.DataSourceName != null)
				((ISnapControl)Control).ShowMailMergeExportOptionsForm(options, ShowMailMergeExportOptionsFormCallback, null);
		}
		protected internal void ShowMailMergeExportOptionsFormCallback(SnapMailMergeExportOptions properties, object callbackData) {
			ParameterizedThreadStart threadStart = obj => ExecuteCommandFromBackgroundThread((MailMergeCommandData)obj);
			Thread backgroundThread = new Thread(threadStart);
			backgroundThread.IsBackground = true;
#if !SL
			if (Thread.CurrentThread.ManagedThreadId == Control.InnerControl.ThreadId) {
				backgroundThread.CurrentCulture = Thread.CurrentThread.CurrentCulture;
				backgroundThread.CurrentUICulture = Thread.CurrentThread.CurrentUICulture;
			}
			backgroundThread.Priority = ThreadPriority.Lowest;
#endif
			DocumentModel.RaiseAsynchronousOperationStarted();
			backgroundThread.Start(new MailMergeCommandData(GetDocumentBytes(), properties));
		}
		byte[] GetDocumentBytes() {
			using (MemoryStream stream = new MemoryStream()) {
				DocumentModel.SaveDocument(stream, SnapDocumentFormat.Snap, string.Empty);
				return stream.ToArray();
			}
		}
		protected void ExecuteCommandFromBackgroundThread(MailMergeCommandData data) {
			using (MemoryStream stream = new MemoryStream(data.DocumentBytes)) {
				InternalSnapDocumentServer server = new InternalSnapDocumentServer(DocumentModel.DocumentFormatsDependencies);
				SubscribeServerEvents(server);
				try {
					server.LoadDocument(stream, SnapDocumentFormat.Snap);
					server.DocumentModel.InheritDataServices(DocumentModel);
					SnapDocumentModel mergeModel = (SnapDocumentModel)server.DocumentModel.CreateNew();
					mergeModel.FieldOptions.CopyFrom(DocumentModel.FieldOptions);
					mergeModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
					mergeModel.DocumentImportOptions.CopyFrom(DocumentModel.DocumentImportOptions);
					bool mailMergeExecuted = false;
					if (data.Options.ProgressIndicationFormVisible)
						mailMergeExecuted = server.InnerServer.SnapMailMerge(data.Options, mergeModel, new SnapMailMergeProgressIndication(DocumentModel));
					else
						mailMergeExecuted = server.InnerServer.SnapMailMerge(data.Options, mergeModel);
					Action method = delegate {
						DocumentModel.CreateModelForExportFunction = () => mergeModel;
						ApplyMailMergeResultAction();
						DocumentModel.CreateModelForExportFunction = null;
					};
					if (mailMergeExecuted)
						((ISnapInnerControlOwner)Control).BeginInvokeMethod(method);
					DocumentModel.RaiseAsynchronousOperationFinished();
					Action updateModel = delegate {
						InnerControl.OnUpdateUI();
					};
					((ISnapInnerControlOwner)Control).BeginInvokeMethod(updateModel);
				}
				finally {
					UnsubscribeServerEvents(server);
				}
			}
		}
		void SubscribeServerEvents(InternalSnapDocumentServer mailMergeServer) {
			InnerSnapControl innerControl = (InnerSnapControl)InnerControl;
			mailMergeServer.SnapMailMergeStarted += innerControl.OnSnapMailMergeStarted;
			mailMergeServer.SnapMailMergeRecordStarted += innerControl.OnSnapMailMergeRecordStarted;
			mailMergeServer.SnapMailMergeRecordFinished += innerControl.OnSnapMailMergeRecordFinished;
			mailMergeServer.SnapMailMergeFinished += innerControl.OnSnapMailMergeFinished;
			mailMergeServer.SnapMailMergeActiveRecordChanging += innerControl.OnSnapMailMergeActiveRecordChanging;
			mailMergeServer.SnapMailMergeActiveRecordChanged += innerControl.OnSnapMailMergeActiveRecordChanged;
		}
		void UnsubscribeServerEvents(InternalSnapDocumentServer mailMergeServer) {
			InnerSnapControl innerControl = (InnerSnapControl)InnerControl;
			mailMergeServer.SnapMailMergeStarted -= innerControl.OnSnapMailMergeStarted;
			mailMergeServer.SnapMailMergeRecordStarted -= innerControl.OnSnapMailMergeRecordStarted;
			mailMergeServer.SnapMailMergeRecordFinished -= innerControl.OnSnapMailMergeRecordFinished;
			mailMergeServer.SnapMailMergeFinished -= innerControl.OnSnapMailMergeFinished;
			mailMergeServer.SnapMailMergeActiveRecordChanging -= innerControl.OnSnapMailMergeActiveRecordChanging;
			mailMergeServer.SnapMailMergeActiveRecordChanged -= innerControl.OnSnapMailMergeActiveRecordChanged;
		}
		protected abstract void ApplyMailMergeResultAction();
	}
}
