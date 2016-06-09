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

using System.IO;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraReports.Service.Native.DAL;
using DevExpress.XtraReports.Service.Native.Services.BinaryStore;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public sealed class PrintMediator : MediatorBase<PrintId, PrintedDocument>, IPrintMediator {
		readonly IBinaryDataStorageService binaryDataStorageService;
		public PrintMediator(IDALService dalService, IBinaryDataStorageServiceProvider binaryDataStorageServiceProvider)
			: base(dalService) {
			Guard.ArgumentNotNull(binaryDataStorageServiceProvider, "binaryDataStorageServiceProvider");
			this.binaryDataStorageService = binaryDataStorageServiceProvider.GetService();
		}
		#region IPrintMediator Members
		public ServiceFault Fault {
			get { return Entity.Fault.FromStored(); }
			set { Entity.Fault = value.ToStored(Session); }
		}
		public Stream Content {
			get { return binaryDataStorageService.LoadStream(Entity.ExternalKey, Session); }
			set { Entity.ExternalKey = binaryDataStorageService.Create(value, Session); }
		}
		public bool ShouldStop {
			get {
				Entity.TaskRequestingAction.Reload();
				return Entity.TaskRequestingAction.Value != RequestingAction.None;
			}
			set { Entity.TaskRequestingAction.Value = value ? RequestingAction.Stop : RequestingAction.None; }
		}
		public override void Save() {
			Entity.TaskRequestingAction.SmartSave();
			base.Save();
		}
		public override void Delete() {
			if(Entity.ExternalKey != null) {
				binaryDataStorageService.Delete(Entity.ExternalKey, Session);
			}
			base.Delete();
		}
		#endregion
		protected override PrintedDocument CreateEntity(PrintId identity) {
			return PrintedDocument.Create(identity, Session);
		}
		protected override PrintedDocument GetEntityById(PrintId identity) {
			return PrintedDocument.GetById(identity, Session);
		}
	}
}
