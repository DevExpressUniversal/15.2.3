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
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraReports.Service.Native.DAL;
namespace DevExpress.XtraReports.Service.Native.Services.Transient {
	public abstract class MediatorBase<TId, VEntity> : IEntityMediator<TId, VEntity>
		where VEntity : XPObject {
		readonly UnitOfWork session;
		VEntity entity;
		bool disposed;
		protected MediatorBase(IDALService dalService) {
			Guard.ArgumentNotNull(dalService, "dalService");
			session = dalService.CreateUnitOfWork();
		}
		#region IEntityMediator<TId, VEntity>
		public UnitOfWork Session {
			get { return session; }
		}
		public void Initialize(TId id, MediatorInitialization init) {
			entity = init == MediatorInitialization.New
				? CreateEntity(id)
				: GetEntityById(id);
		}
		public VEntity Entity {
			get { return entity; }
		}
		public virtual void Save() {
			var smartXPObject = Entity as SmartXPObject;
			if(smartXPObject != null) {
				smartXPObject.SmartSave();
			}
			session.CommitChanges();
		}
		public virtual void Delete() {
			session.Delete(Entity);
			session.CommitChanges();
		}
		public virtual void ReloadEntity() {
			session.Reload(Entity, true);
		}
		#endregion
		#region IDisposable Members
		~MediatorBase() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		public virtual void Dispose(bool disposing) {
			if(disposed) {
				return;
			}
			if(disposing) {
				session.Dispose();
			}
			disposed = true;
		}
		#endregion
		protected abstract VEntity CreateEntity(TId identity);
		protected abstract VEntity GetEntityById(TId identity);
	}
}
