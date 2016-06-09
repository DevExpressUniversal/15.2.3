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

using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Specialized;
namespace DevExpress.XtraScheduler.Internal {
	public interface IInternalPersistentObjectStorage<T> where T : IPersistentObject {
		MappingCollection ActualMappings { get; }
		ISchedulerUnboundDataKeeper UnboundDataKeeper { get; }
		DataManager<T> DataManager { get; }
		ISchedulerStorageBase Storage { get; }
		bool IsDisposed { get; }
		bool IsLoading { get; }
		void LoadObjects(bool keepNonPersistentInformation);
		void LoadObjectsCore(bool keepNonPersistentInformation, LoadObjectsCoreHandler handler);
		void CommitExistingObject(T obj);
		void CommitNewObject(T obj);
		void RollbackExistingObject(T obj);
		void AppendDefaultMappings(MappingCollection target);
		void AppendMappings(MappingCollection target);
		void AppendCustomMappings();
		void ValidateDataSource();
		void ValidateDataSourceCore(StringCollection errors, DataFieldInfoCollection fields, MappingCollection mappings);
		void BeginInit();
		void EndInit();
		void RaiseReload(bool keepNonPersistentInformation);
		event EventHandler MappingsChanged;
		event EventHandler FilterChanged;
		event CancelListChangedEventHandler AutoReloading;
		event PersistentObjectStorageReloadEventHandler Reload;
		event EventHandler ObjectCollectionCleared;
		event EventHandler ObjectCollectionLoaded;
		event PersistentObjectCancelEventHandler ObjectInserting;
		event PersistentObjectEventHandler ObjectInserted;
		event PersistentObjectCancelEventHandler ObjectDeleting;
		event PersistentObjectEventHandler ObjectDeleted;
		event PersistentObjectStateChangingEventHandler InternalObjectChanging;
		event PersistentObjectCancelEventHandler ObjectChanging;
		event PersistentObjectEventHandler ObjectChanged;
	}
}
