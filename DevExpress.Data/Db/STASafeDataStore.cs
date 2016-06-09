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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Helpers;
namespace DevExpress.Xpo.Providers {
	public class STASafeDataStore : IDataStore, IDataStoreSchemaExplorer, ICommandChannel {
		readonly IDataStore DataStore;
		public STASafeDataStore() { }
		public STASafeDataStore(IDataStore dataStore)
			: this() {
			this.DataStore = dataStore;
		}
		#region IDataStore Members
		public AutoCreateOption AutoCreateOption {
			get {
				return StaSafeHelper.Invoke(() => DataStore.AutoCreateOption);
			}
		}
		public ModificationResult ModifyData(params ModificationStatement[] dmlStatements) {
			return StaSafeHelper.Invoke(() => DataStore.ModifyData(dmlStatements));
		}
		public SelectedData SelectData(params SelectStatement[] selects) {
			return StaSafeHelper.Invoke(() => DataStore.SelectData(selects));
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables) {
			return StaSafeHelper.Invoke(() => DataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables));
		}
		#endregion
		#region IDataStoreSchemaExplorer Members
		public string[] GetStorageTablesList(bool includeViews) {
			return StaSafeHelper.Invoke(() => ((IDataStoreSchemaExplorer)DataStore).GetStorageTablesList(includeViews));
		}
		public DBTable[] GetStorageTables(params string[] tables) {
			return StaSafeHelper.Invoke(() => ((IDataStoreSchemaExplorer)DataStore).GetStorageTables(tables));
		}
		#endregion
		#region ICommandChannel Members
		public object Do(string command, object args) {
			ICommandChannel nestedCommandChannel = DataStore as ICommandChannel;
			if(nestedCommandChannel == null) {
				if(DataStore == null) throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				else throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, DataStore.GetType()));
			}
			return StaSafeHelper.Invoke(() => nestedCommandChannel.Do(command, args));
		}
		#endregion
	}
}
