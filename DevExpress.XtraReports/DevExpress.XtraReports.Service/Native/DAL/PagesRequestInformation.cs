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
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpo;
using DevExpress.XtraReports.Service.Native.Services;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class PagesRequestInformation : ExtendedXPObject {
		protected static ILoggingService Logger {
			get { return DefaultLogger.Current; }
		}
		public static IQueryable<PagesRequestInformation> FindUnansweredByDocument(DocumentId id, UnitOfWork session) {
			var documentIdValue = id.Value;
			return session.Query<PagesRequestInformation>().Where(x => x.DocumentIdValue == documentIdValue && x.ResponseInformation == null);
		}
		public static XPCollection<PagesRequestInformation> FindOutdated(DateTime criteria, UnitOfWork session) {
			var outdateCriteria = XpoHelper.CreateOperator<PagesRequestInformation>(d => d.CreationTime, criteria, BinaryOperatorType.Less);
			return new XPCollection<PagesRequestInformation>(session, outdateCriteria);
		}
		PageCompatibility compatibility;
		int? addition;
		PagesResponseInformation response;
		string documentIdValue;
		byte[] serializedPageIndexes;
		DateTime creationTime;
		[Size(80)]
		public byte[] SerializedPageIndexes {
			get { return serializedPageIndexes; }
			set { SetPropertyValue(() => SerializedPageIndexes, ref serializedPageIndexes, value); }
		}
		public PageCompatibility Compatibility {
			get { return compatibility; }
			set { SetPropertyValue(() => Compatibility, ref compatibility, value); }
		}
		public int? Addition {
			get { return addition; }
			set { SetPropertyValue(() => Addition, ref addition, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Indexed]
		public string DocumentIdValue {
			get { return documentIdValue; }
			set { SetPropertyValue(() => DocumentIdValue, ref documentIdValue, value); }
		}
		public DocumentId DocumentId {
			get { return new DocumentId(documentIdValue); }
			set { DocumentIdValue = value.Value; }
		}
		[Aggregated]
		public PagesResponseInformation ResponseInformation {
			get { return response; }
			set { SetPropertyValue(() => ResponseInformation, ref response, value); }
		}
		public DateTime CreationTime {
			get { return creationTime; }
			set { SetPropertyValue(() => CreationTime, ref creationTime, value); }
		}
		public PagesRequestInformation(Session session)
			: base(session) {
		}
		protected override void OnDeleting() {
			base.OnDeleting();
			WriteLog("OnDeleting");
		}
		protected override void OnSaved() {
			base.OnSaved();
			WriteLog("OnSaved");
		}
		[Conditional("DEBUG")]
		void WriteLog(string funcName) {
			var optimisticLockField = ClassInfo.GetMember("OptimisticLockField").GetValue(this);
			var optimisticLockFieldInDataLayer = ClassInfo.GetMember("OptimisticLockFieldInDataLayer").GetValue(this);
			Logger.Info("({0}) PageRequestInformation.{1}[Oid:{2}; OptimisticLockField:{3}; OptimisticLockFieldInDataLayer:{4}]",
				DocumentIdValue, funcName, Oid, optimisticLockField, optimisticLockFieldInDataLayer);
		}
	}
}
