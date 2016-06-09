#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Updating {
	public class DatabaseUpdaterManager : IDisposable {
		private readonly object compatibilityCheckLockObject = new object();
		private bool isCompatibilityChecked = false;
		private XafApplication application;
		private CheckCompatibilityType checkCompatibilityType = CheckCompatibilityType.ModuleInfo;
		private DatabaseUpdateMode databaseUpdateMode = DatabaseUpdateMode.UpdateOldDatabase;
		protected XafApplication Application {
			get { return application; }
		}
		[Browsable(false)]
		public bool IsCompatibilityChecked {
			get { return isCompatibilityChecked; }
			internal set { isCompatibilityChecked = value; }
		}
		public CheckCompatibilityType CheckCompatibilityType {
			get { return checkCompatibilityType; }
			set { checkCompatibilityType = value; }
		}
		public DatabaseUpdateMode DatabaseUpdateMode {
			get { return databaseUpdateMode; }
			set { databaseUpdateMode = value; }
		}
		private void DatabaseUpdater_StatusUpdating(Object sender, StatusUpdatingEventArgs e) {
			Application.UpdateStatus(e.Context, e.Title, e.Message, e.AdditionalParams);
		}
		protected virtual void CheckCompatibilityCore() {
			if(DatabaseUpdateMode != DatabaseUpdateMode.Never) {
				lock(compatibilityCheckLockObject) {
					foreach(IObjectSpaceProvider objectSpaceProvider in Application.ObjectSpaceProviders) {
						if(objectSpaceProvider.SchemaUpdateMode != SchemaUpdateMode.None) {
							if(CheckCompatibilityType == ExpressApp.CheckCompatibilityType.DatabaseSchema || objectSpaceProvider.ModuleInfoType != null) {
								DatabaseUpdaterBase databaseUpdater = CreateDatabaseUpdater(objectSpaceProvider);
								try {
									databaseUpdater.StatusUpdating += new EventHandler<StatusUpdatingEventArgs>(DatabaseUpdater_StatusUpdating);
									CompatibilityError compatibilityError = databaseUpdater.CheckCompatibility();
									if(compatibilityError != null) {
										if(compatibilityError is CompatibilityDatabaseIsOldError || compatibilityError is CompatibilityUnableToOpenDatabaseError) {
											DatabaseVersionMismatchEventArgs args = new DatabaseVersionMismatchEventArgs(databaseUpdater, compatibilityError);
											OnDatabaseVersionMismatch(args);
											if(args.Handled) {
												compatibilityError = databaseUpdater.CheckCompatibility();
											}
										}
										if(compatibilityError != null) {
											throw new CompatibilityException(compatibilityError);
										}
									}
									else if(DatabaseUpdateMode == DatabaseUpdateMode.UpdateDatabaseAlways) {
										DatabaseVersionMismatchEventArgs args = new DatabaseVersionMismatchEventArgs(databaseUpdater, null);
										databaseUpdater.ForceUpdateDatabase = true;
										OnDatabaseVersionMismatch(args);
									}
								}
								finally {
									databaseUpdater.Dispose();
								}
							}
						}
					}
				}
			}
		}
		protected virtual void OnCustomCheckCompatibility(CustomCheckCompatibilityEventArgs args) {
			if(CustomCheckCompatibility != null) {
				Tracing.Tracer.LogVerboseText("->CustomCheckCompatibility");
				CustomCheckCompatibility(this, args);
				Tracing.Tracer.LogVerboseText("<-CustomCheckCompatibility");
			}
		}
		protected virtual void OnDatabaseVersionMismatch(DatabaseVersionMismatchEventArgs args) {
			if(DatabaseVersionMismatch != null) {
				DatabaseVersionMismatch(this, args);
			}
		}
		public virtual DatabaseUpdaterBase CreateDatabaseUpdater(IObjectSpaceProvider objectSpaceProvider) {
			DatabaseUpdaterBase databaseUpdater = null;
			switch(CheckCompatibilityType) {
				case CheckCompatibilityType.DatabaseSchema:
					databaseUpdater = new DatabaseSchemaUpdater(objectSpaceProvider, Application.Modules);
					break;
				default:
				case CheckCompatibilityType.ModuleInfo:
					if(objectSpaceProvider.ModuleInfoType != null) {
						databaseUpdater = new DatabaseUpdater(objectSpaceProvider, Application.Modules, Application.ApplicationName, objectSpaceProvider.ModuleInfoType);
					}
					break;
			}
			DatabaseUpdaterEventArgs args = new DatabaseUpdaterEventArgs(databaseUpdater);
			if(DatabaseUpdaterCreating != null) {
				DatabaseUpdaterCreating(this, args);
			}
			return args.Updater;
		}
		public void CheckCompatibility() {
			CustomCheckCompatibilityEventArgs args = new CustomCheckCompatibilityEventArgs(isCompatibilityChecked, (Application.ObjectSpaceProviders.Count > 0) ? Application.ObjectSpaceProviders[0] : null, Application.Modules, Application.ApplicationName);
			OnCustomCheckCompatibility(args);
			if(!args.Handled && !isCompatibilityChecked) {
				Tracing.Tracer.LogSubSeparator("Checking Compatibility");
				foreach(IObjectSpaceProvider objectSpaceProvider in Application.ObjectSpaceProviders) {
					Tracing.Tracer.LogValue("ConnectionString", objectSpaceProvider.ConnectionString);
				}
				CheckCompatibilityCore();
				Tracing.Tracer.LogVerboseText("Compatibility is checked");
			}
			isCompatibilityChecked = true;
		}
		public DatabaseUpdaterManager(XafApplication application) {
			this.application = application;
		}
		public event EventHandler<DatabaseVersionMismatchEventArgs> DatabaseVersionMismatch;
		public event EventHandler<CustomCheckCompatibilityEventArgs> CustomCheckCompatibility;
		public event EventHandler<DatabaseUpdaterEventArgs> DatabaseUpdaterCreating;
		public void Dispose() {
			DatabaseVersionMismatch = null;
			CustomCheckCompatibility = null;
			DatabaseUpdaterCreating = null;
		}
	}
}
