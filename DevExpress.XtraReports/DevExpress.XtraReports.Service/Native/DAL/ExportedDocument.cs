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
using DevExpress.Utils;
using DevExpress.Xpo;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class ExportedDocument : SmartXPObject {
		internal const string ExportedDocumentContentAssociationName = "ExportedDocument-Content";
		public static ExportedDocument Create(ExportId exportId, UnitOfWork session) {
			return new ExportedDocument(session) { ExportId = exportId, CreationTime = DateTime.UtcNow };
		}
		public static ExportedDocument GetById(ExportId exportId, UnitOfWork session) {
			var criteria = XpoHelper.CreateOperator<ExportedDocument>(d => d.ExportIdValue, exportId.Value);
			var result = session.FindObject<ExportedDocument>(criteria);
			if(result == null) {
				throw new DocumentDoesNotExistException(Messages.NoExportDocument);
			}
			return result;
		}
		public static XPCollection<ExportedDocument> FindOutdated(DateTime criteria, Session session) {
			var outdateCriteria = XpoHelper.CreateOperator<ExportedDocument>(d => d.CreationTime, criteria, BinaryOperatorType.Less);
			return new XPCollection<ExportedDocument>(session, outdateCriteria);
		}
		DateTime creationTime;
		TaskStatus status;
		int progressPosition;
		ExportId exportId;
		string name;
		string contentType;
		StoredServiceFault fault;
		string externalKey;
		public virtual DateTime CreationTime {
			get { return creationTime; }
			set { SetPropertyValue(() => CreationTime, ref creationTime, value); }
		}
		public virtual TaskStatus Status {
			get { return status; }
			set { SetPropertyValue(() => Status, ref status, value); }
		}
		public virtual int ProgressPosition {
			get { return progressPosition; }
			set { SetPropertyValue(() => ProgressPosition, ref progressPosition, value); }
		}
		[Indexed(Unique = true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ExportIdValue {
			get { return exportId != null ? exportId.Value : null; }
			set {
				string exportIdValue = ExportId.Value;
				SetPropertyValue(() => ExportIdValue, ref exportIdValue, value);
				ExportId.Value = value;
			}
		}
		[NonPersistent]
		public ExportId ExportId {
			get {
				if(exportId == null) {
					exportId = new ExportId();
				}
				return exportId;
			}
			set {
				Guard.ArgumentNotNull(value, "value");
				exportId = value;
			}
		}
		[Aggregated]
		public StoredServiceFault Fault {
			get { return fault; }
			set { SetPropertyValue(() => Fault, ref fault, value); }
		}
		public string Name {
			get { return name; }
			set { SetPropertyValue(() => Name, ref name, value); }
		}
		public string ContentType {
			get { return contentType; }
			set { SetPropertyValue(() => ContentType, ref contentType, value); }
		}
		public string ExternalKey {
			get { return externalKey; }
			set { SetPropertyValue(() => ExternalKey, ref externalKey, value); }
		}
		public ExportedDocument(Session session)
			: base(session) {
		}
	}
}
