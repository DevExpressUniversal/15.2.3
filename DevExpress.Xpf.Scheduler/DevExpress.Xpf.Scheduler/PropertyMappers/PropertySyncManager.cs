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
using System.Windows;
namespace DevExpress.Xpf.Scheduler.Native {
	#region DependencyPropertyManager
	public abstract class DependencyPropertySyncManager {
		readonly DependencyPropertyMapperTable propertyMapperTable;
		protected DependencyPropertySyncManager() {
			this.propertyMapperTable = CreatePropertyManager();
		}
		protected internal DependencyPropertyMapperTable PropertyMapperTable { get { return propertyMapperTable; } }
		public bool IsDeferredChanges { get { return PropertyMapperTable.IsDeferredChanges; } }
		protected virtual DependencyPropertyMapperTable CreatePropertyManager() {
			return new DependencyPropertyMapperTable();
		}
		public abstract void Register();
		public void Unregister() {
			PropertyMapperTable.UnregisterPropertyMappers();
		}
		public void Reactivate() {
			PropertyMapperTable.ActivatePropertyMappers(PropertyMapperSyncDirection.OwnerToInner);
		}
		public void Activate() {
			PropertyMapperTable.ActivatePropertyMappers(PropertyMapperSyncDirection.InnerToOwner);
		}
		public void Deactivate() {
			PropertyMapperTable.DeactivatePropertyMappers();
		}
		public virtual void Update(DependencyProperty property, object oldValue, object newValue) {
			PropertyMapperTable.UpdatePropertyMapper(property, oldValue, newValue);
		}
		public void StartDeferredChanges() {
			PropertyMapperTable.StartDeferredChanges();
		}
		public void CancelDeferredChanges() {
			PropertyMapperTable.CancelDeferredChanges();
		}
		public void CommitDeferredChanges() {
			PropertyMapperTable.CommitDeferredChanges();
		}
	}
	#endregion
}
