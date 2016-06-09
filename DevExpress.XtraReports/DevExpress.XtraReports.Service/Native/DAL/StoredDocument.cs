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
using DevExpress.Data.Filtering;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class StoredDocument : SmartXPObject {
		public static StoredDocument Create(DocumentId documentId, Session session) {
			return new StoredDocument(session) {
				DocumentId = documentId,
				LastModifiedTime = DateTime.UtcNow,
				TaskRequestingAction = new TaskRequestingAction(session) { Value = RequestingAction.None },
				RelatedData = new StoredDocumentRelatedData(session)
			};
		}
		public static StoredDocument CreatePermanent(DocumentId documentId, Session session) {
			var result = Create(documentId, session);
			result.IsPermanent = true;
			return result;
		}
		public static StoredDocument GetById(DocumentId documentId, Session session) {
			var result = session.FindBy<StoredDocument>(d => d.DocumentIdValue, documentId.Value);
			if(result == null) {
				throw new DocumentDoesNotExistException();
			}
			return result;
		}
		public static XPCollection<StoredDocument> FindOutdated(DateTime criteria, Session session) {
			var outdateCriteria = BinaryOperator.And(
				XpoHelper.CreateOperator<StoredDocument>(d => d.LastModifiedTime, criteria, BinaryOperatorType.Less),
				XpoHelper.CreateOperator<StoredDocument>(d => d.IsPermanent, false, BinaryOperatorType.Equal));
			return new XPCollection<StoredDocument>(session, outdateCriteria);
		}
		DocumentId documentId;
		DateTime lastModifiedTime;
		TaskStatus status;
		int pageCount;
		int progressPosition;
		bool canChangePageSettings;
		ExportOptionKind hiddenOptions;
		bool isPermanent;
		[Indexed(Unique = true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string DocumentIdValue {
			get { return documentId != null ? documentId.Value : null; }
			set {
				var documentIdValue = DocumentId.Value;
				SetPropertyValue(() => DocumentIdValue, ref documentIdValue, value);
				DocumentId.Value = documentIdValue;
			}
		}
		[NonPersistent]
		public DocumentId DocumentId {
			get {
				if(documentId == null) {
					documentId = new DocumentId();
				}
				return documentId;
			}
			set { documentId = value; }
		}
		public DateTime LastModifiedTime {
			get { return lastModifiedTime; }
			set { SetPropertyValue(() => LastModifiedTime, ref lastModifiedTime, value); }
		}
		[Delayed, Aggregated]
		public TaskRequestingAction TaskRequestingAction {
			get { return GetDelayedPropertyValue<TaskRequestingAction>(() => TaskRequestingAction); }
			set { SetDelayedPropertyValue<TaskRequestingAction>(() => TaskRequestingAction, value); }
		}
		public TaskStatus Status {
			get { return status; }
			set { SetPropertyValue(() => Status, ref status, value); }
		}
		public int PageCount {
			get { return pageCount; }
			set { SetPropertyValue(() => PageCount, ref pageCount, value); }
		}
		public int ProgressPosition {
			get { return progressPosition; }
			set { SetPropertyValue(() => ProgressPosition, ref progressPosition, value); }
		}
		public bool CanChangePageSettings {
			get { return canChangePageSettings; }
			set { SetPropertyValue(() => CanChangePageSettings, ref canChangePageSettings, value); }
		}
		public ExportOptionKind HiddenOptions {
			get { return hiddenOptions; }
			set { SetPropertyValue(() => HiddenOptions, ref hiddenOptions, value); }
		}
		public bool IsPermanent {
			get { return isPermanent; }
			set { SetPropertyValue(() => IsPermanent, ref isPermanent, value); }
		}
		[Delayed, Aggregated]
		public StoredDocumentRelatedData RelatedData {
			get { return GetDelayedPropertyValue<StoredDocumentRelatedData>(() => RelatedData); }
			set { SetDelayedPropertyValue(() => RelatedData, value); }
		}
		public StoredDocument(Session session)
			: base(session) {
		}
	}
}
