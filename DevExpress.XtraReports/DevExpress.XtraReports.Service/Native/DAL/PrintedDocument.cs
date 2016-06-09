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
using DevExpress.Data.Filtering;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Xpo;
using DevExpress.XtraReports.Service.Native.Services.Transient;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class PrintedDocument : SmartXPObject {
		internal const string PrintedDocumentContentAssociationName = "PrintedDocument-Content";
		public static PrintedDocument Create(PrintId printId, Session session) {
			return new PrintedDocument(session) {
				PrintId = printId.Value,
				CreationTime = DateTime.UtcNow,
				TaskRequestingAction = new TaskRequestingAction(session) { Value = RequestingAction.None }
			};
		}
		public static PrintedDocument GetById(PrintId printId, Session session) {
			var result = session.FindBy<PrintedDocument>(d => d.PrintId, printId.Value);
			if(result == null) {
				throw new NullReferenceException(Messages.DocumentDoesNotExist);
			}
			return result;
		}
		public static XPCollection<PrintedDocument> FindOutdated(DateTime criteria, Session session) {
			var outdateCriteria = XpoHelper.CreateOperator<PrintedDocument>(d => d.CreationTime, criteria, BinaryOperatorType.Less);
			return new XPCollection<PrintedDocument>(session, outdateCriteria);
		}
		string printId;
		DateTime creationTime;
		TaskStatus status;
		int progressPosition;
		StoredServiceFault fault;
		string externalKey;
		[Indexed(Unique = true)]
		public string PrintId {
			get { return printId; }
			set { SetPropertyValue(() => PrintId, ref printId, value); }
		}
		public virtual DateTime CreationTime {
			get { return creationTime; }
			set { SetPropertyValue(() => CreationTime, ref creationTime, value); }
		}
		[Delayed]
		[Aggregated]
		public TaskRequestingAction TaskRequestingAction {
			get { return GetDelayedPropertyValue<TaskRequestingAction>(() => TaskRequestingAction); }
			set { SetDelayedPropertyValue<TaskRequestingAction>(() => TaskRequestingAction, value); }
		}
		public TaskStatus Status {
			get { return status; }
			set { SetPropertyValue(() => Status, ref status, value); }
		}
		public int ProgressPosition {
			get { return progressPosition; }
			set { SetPropertyValue(() => ProgressPosition, ref progressPosition, value); }
		}
		[Aggregated]
		public StoredServiceFault Fault {
			get { return fault; }
			set { SetPropertyValue(() => Fault, ref fault, value); }
		}
		public string ExternalKey {
			get { return externalKey; }
			set { SetPropertyValue(() => ExternalKey, ref externalKey, value); }
		}
		public PrintedDocument(Session session)
			: base(session) {
		}
	}
}
