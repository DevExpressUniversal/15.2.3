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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
using DevExpress.Xpo.DB.Exceptions;
namespace DevExpress.ExpressApp.Updating {
	public class CustomLoadVersionInfoListEventArgs : HandledEventArgs {
		public CustomLoadVersionInfoListEventArgs(IObjectSpace objectSpace) {
			ModuleInfoList = new List<IModuleInfo>();
			ObjectSpace = objectSpace;
		}
		public IList<IModuleInfo> ModuleInfoList { get; set; }
		public IObjectSpace ObjectSpace { get; private set; }
	}
	public abstract class DatabaseUpdaterBase : IDisposable {
		private Boolean useAllModuleUpdaters = false;
		private Boolean forceUpdateDatabase = false;
		protected IObjectSpaceProvider objectSpaceProvider;
		protected readonly IList<ModuleBase> modules;
		private void ModuleUpdater_StatusUpdating(Object sender, StatusUpdatingEventArgs e) {
			OnStatusUpdating(e.Context, e.Title, e.Message, e.AdditionalParams);
		}
		protected virtual void OnStatusUpdating(String context, String title, String message, params Object[] additionalParams) {
			if (StatusUpdating != null) {
				StatusUpdating(this, new StatusUpdatingEventArgs(context, title, message, additionalParams));
			}
		}
		protected virtual void OnConfirmation(CancelEventArgs args) {
			if(Confirmation != null) {
				Confirmation(this, args);
			}
		}
		protected void BeforeUpdateSchema(IList<ModuleUpdater> moduleUpdaters) {
			OnStatusUpdating(ApplicationStatusMesssageId.UpdateDatabaseSchema.ToString(), "", ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.UpdateDatabaseSchema));
			foreach(ModuleUpdater moduleUpdater in moduleUpdaters) {
				moduleUpdater.StatusUpdating += new EventHandler<StatusUpdatingEventArgs>(ModuleUpdater_StatusUpdating);
				moduleUpdater.UpdateDatabaseBeforeUpdateSchema();
			}
		}
		protected void AfterUpdateSchema(IList<ModuleUpdater> moduleUpdaters) {
			OnStatusUpdating(ApplicationStatusMesssageId.UpdateDatabaseData.ToString(), "", ApplicationStatusMesssagesLocalizer.Active.GetLocalizedString(ApplicationStatusMesssageId.UpdateDatabaseData));
			foreach(ModuleUpdater moduleUpdater in moduleUpdaters) {
				moduleUpdater.UpdateDatabaseAfterUpdateSchema();
				moduleUpdater.StatusUpdating -= new EventHandler<StatusUpdatingEventArgs>(ModuleUpdater_StatusUpdating);
			}
		}
		protected virtual void UpdateCore(IObjectSpace updatingObjectSpace, IList<IModuleInfo>  versionInfoList) {
			IList<ModuleUpdater> moduleUpdaters = GetModuleUpdaters(updatingObjectSpace, versionInfoList);
			BeforeUpdateSchema(moduleUpdaters);
			objectSpaceProvider.UpdateSchema();
			AfterUpdateSchema(moduleUpdaters);
		}
		protected abstract IList<CompatibilityError> CheckCompatibilityCore(bool getFirstVersionError);
		protected virtual IList<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, IList<IModuleInfo> versionInfoList) {
			List<ModuleUpdater> dbUpdaters = new List<ModuleUpdater>();
			foreach(ModuleBase module in modules) {
				Version moduleVersionFromDB = GetModuleVersion(versionInfoList, module.Name);
				if(ForceUpdateDatabase || module.Version > moduleVersionFromDB || useAllModuleUpdaters) {
					dbUpdaters.AddRange(module.GetModuleUpdaters(objectSpace, moduleVersionFromDB));
				}
			}
			return dbUpdaters;
		}
		protected Version GetModuleVersion(IList<IModuleInfo> versionInfoList, String name) {
			foreach(IModuleInfo moduleInfo in versionInfoList) {
				if(moduleInfo.Name == name) {
					return new Version(moduleInfo.Version);
				}
			}
			return new Version("0.0.0.0");
		}
		protected Boolean UseAllModuleUpdaters { get { return useAllModuleUpdaters; } set { useAllModuleUpdaters = value; } }
		public DatabaseUpdaterBase(IObjectSpaceProvider objectSpaceProvider, IList<ModuleBase> modules) {
			this.objectSpaceProvider = objectSpaceProvider;
			this.modules = modules;
		}
		public abstract void Update();
		public virtual CompatibilityError CheckCompatibility() {
			IList<CompatibilityError> result = CheckCompatibilityCore(true);
			if ((result != null) && (result.Count > 0)) {
				return result[0];
			}
			else {
				return null;
			}
		}
		public virtual IList<CompatibilityError> CheckCompatibilityForAllModules() {
			return CheckCompatibilityCore(false);
		}
		public Boolean ForceUpdateDatabase {
			get { return forceUpdateDatabase; }
			set { forceUpdateDatabase = value; }
		}
		public event CancelEventHandler Confirmation;
		public event EventHandler<StatusUpdatingEventArgs> StatusUpdating;
		public virtual void Dispose() {
			objectSpaceProvider = null;
			Confirmation = null;
			StatusUpdating = null;
		}
	}
	public class DatabaseUpdater : DatabaseUpdaterBase {
		private readonly Type moduleInfoType;
		private readonly String applicationName;
		private readonly Boolean isCheckAppName;
		private CompatibilityApplicationNameError CheckDatabaseApplicationName(IObjectSpace objectSpace, IList<IModuleInfo> versionInfoList) {
			if(isCheckAppName) {
				Tracing.Tracer.LogText("Check App Name Compatibility");
				String applicationNameFromDB = GetApplicationNameFromDB(versionInfoList);
				if(!String.IsNullOrEmpty(applicationNameFromDB) && (applicationNameFromDB != applicationName)) {
					return new CompatibilityApplicationNameError(null, applicationName, applicationNameFromDB, moduleInfoType.Name);
				}
				if(String.IsNullOrEmpty(applicationNameFromDB) && !String.IsNullOrEmpty(applicationName)) {
					UpdateVersion(objectSpace, applicationName, "", "", true);
				}
			}
			return null;
		}
		private IList<CompatibilityCheckVersionsError> CheckVersionForAllModules(IList<IModuleInfo> versionInfoList, bool getFirstError) {
			IList<CompatibilityCheckVersionsError> result = new List<CompatibilityCheckVersionsError>();
			Tracing.Tracer.LogText("Check stored module versions against the versions of the loaded modules");
			foreach(ModuleBase module in modules) {
				Version moduleVersionFromDB = GetModuleVersion(versionInfoList, module.Name);
				Tracing.Tracer.LogText("module '{0}' ({1}). Local version: {2}, Version on DB: {3}",
					module.Name, ReflectionHelper.GetAssemblyName(module.GetType().Assembly), module.Version, moduleVersionFromDB.ToString());
				if((module.Version != null) && (module.Version > moduleVersionFromDB)) {
					Tracing.Tracer.LogText("DatabaseIsOld");
					result.Add(new CompatibilityDatabaseIsOldError(null, module, moduleVersionFromDB));
				}
				if((module.Version != null) && (module.Version < moduleVersionFromDB)) {
					Tracing.Tracer.LogText("ApplicationIsOld");
					result.Add(new CompatibilityApplicationIsOldError(null, module, moduleVersionFromDB));
				}
				if(getFirstError && result.Count > 0) {
					break;
				}
			}
			return result;
		}
		private CompatibilityCheckVersionsError CheckVersions(IList<IModuleInfo> versionInfoList) {
			IList<CompatibilityCheckVersionsError> result = CheckVersionForAllModules(versionInfoList, true);
			return (result != null && result.Count > 0) ? result[0] : null;
		}
		private void SetNewVersionInDB(IObjectSpace objectSpace, IList<IModuleInfo> versionInfoList) {
			foreach(ModuleBase module in modules) {
				Version moduleVersionFromDB = GetModuleVersion(versionInfoList, module.Name);
				if(module.Version > moduleVersionFromDB) {
					String assemblyFileName = null;
					if(ReflectionHelper.HasPermissionToGetAssemblyName) {
						assemblyFileName = System.IO.Path.GetFileName(module.GetType().Assembly.Location);
					}
					UpdateVersion(objectSpace, module.Name, assemblyFileName, module.Version.ToString(), false);
				}
				Tracing.Tracer.LogText(string.Format("Module: {0}. Old version: {1}. New version: {2}",
					module.Name, moduleVersionFromDB, module.Version));
			}
			UpdateVersion(objectSpace, applicationName, String.Empty, String.Empty, true);
		}
		private void UpdateVersion(IObjectSpace objectSpace, String moduleName, String assemblyFileName, String version, Boolean isMain) {
			if(objectSpaceProvider.SchemaUpdateMode == SchemaUpdateMode.DatabaseAndSchema) {
				if(isMain && String.IsNullOrEmpty(moduleName)) {
					return;
				}
				IModuleInfo moduleInfo = (IModuleInfo)objectSpace.FindObject(moduleInfoType, new BinaryOperator("Name", moduleName));
				if(moduleInfo == null) {
					moduleInfo = (IModuleInfo)objectSpace.CreateObject(moduleInfoType);
					moduleInfo.Name = moduleName;
					moduleInfo.IsMain = isMain;
				}
				moduleInfo.AssemblyFileName = assemblyFileName;
				moduleInfo.Version = version;
				objectSpace.SetModified(moduleInfo);
				objectSpace.CommitChanges();
			}
		}
		private IList<IModuleInfo> LoadVersionInfoList(IObjectSpace objectSpace, out CompatibilityError compatibilityError) {
			compatibilityError = null;
			IList<IModuleInfo> result;
			CustomLoadVersionInfoListEventArgs args = new CustomLoadVersionInfoListEventArgs(objectSpace);
			try {
				if(CustomLoadVersionInfoList != null) {
					CustomLoadVersionInfoList(this, args);
				}
				if(!args.Handled) {
					result = new List<IModuleInfo>();
					if(moduleInfoType != null) {
						IList moduleInfoList = objectSpace.GetObjects(moduleInfoType);
						if(moduleInfoList != null) {
							foreach(IModuleInfo moduleInfo in moduleInfoList) {
								result.Add(moduleInfo);
							}
						}
					}
					int count = result.Count;
				}
				else {
					result = args.ModuleInfoList;
				}
				return result;
			}
			catch(UnableToOpenDatabaseException e) {
				Exception ex = e;
				if(e.InnerException != null) {
					ex = e.InnerException;
				}
				compatibilityError = new CompatibilityUnableToOpenDatabaseError(ex);
			}
			catch(SchemaCorrectionNeededException e) {
				compatibilityError = new CompatibilityUnableToOpenDatabaseError(e);
			}
			return null;
		}
		protected override IList<CompatibilityError> CheckCompatibilityCore(bool getFirstVersionError) {
			List<CompatibilityError> result = new List<CompatibilityError>();
			try {
				using(IObjectSpace objectSpace = objectSpaceProvider.CreateUpdatingObjectSpace(false)) {
					CompatibilityError versionInfoListError;
					IList<IModuleInfo> versionInfoList = LoadVersionInfoList(objectSpace, out versionInfoListError);
					if(versionInfoListError != null) {
						result.Add(versionInfoListError);
						return result;
					}
					CompatibilityApplicationNameError appNameError = CheckDatabaseApplicationName(objectSpace, versionInfoList);
					if(appNameError != null) {
						result.Add(appNameError);
						return result;
					}
					IList<CompatibilityCheckVersionsError> versionsErrors = CheckVersionForAllModules(versionInfoList, getFirstVersionError);
					if(versionsErrors != null && versionsErrors.Count > 0) {
						result.AddRange(versionsErrors);
						return result;
					}
					return null;
				}
			}
			catch(UnableToOpenDatabaseException e) {
				Exception ex = e;
				if(e.InnerException != null) {
					ex = e.InnerException;
				}
				result.Add(new CompatibilityUnableToOpenDatabaseError(ex));
			}
			catch(SchemaCorrectionNeededException e) {
				result.Add(new CompatibilityUnableToOpenDatabaseError(e));
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
				result.Add(new CompatibilityUnknownError(e));
			}
			return result;
		}
		protected String GetApplicationNameFromDB(IList<IModuleInfo> versionInfoList) {
			foreach(IModuleInfo moduleInfo in versionInfoList) {
				if(moduleInfo.IsMain == true) {
					return moduleInfo.Name;
				}
			}
			return "";
		}
		public DatabaseUpdater(IObjectSpaceProvider objectSpaceProvider, IList<ModuleBase> modules, String applicationName, Type moduleInfoType) : base(objectSpaceProvider, modules) {
			isCheckAppName = applicationName != null;
			Guard.ArgumentNotNull(objectSpaceProvider, "objectSpaceProvider");
			Guard.ArgumentNotNull(modules, "modules");
			this.objectSpaceProvider = objectSpaceProvider;
			this.applicationName = applicationName;
			this.moduleInfoType = moduleInfoType;
		}
		public override void Update() {
			Tracing.Tracer.LogSubSeparator("UpdateDB");
			try {
				using(IObjectSpace updatingObjectSpace = objectSpaceProvider.CreateUpdatingObjectSpace(true)) {
					Tracing.Tracer.LogValue("ConnectionString", objectSpaceProvider.ConnectionString);
					Tracing.Tracer.LockFlush();
					try {
						CompatibilityError versionInfoListError;
						IList<IModuleInfo> versionInfoList = LoadVersionInfoList(updatingObjectSpace, out versionInfoListError);
						if(versionInfoListError != null) {
							throw new CompatibilityException(versionInfoListError);
						}
						CompatibilityApplicationNameError appNameError = CheckDatabaseApplicationName(updatingObjectSpace, versionInfoList);
						if(appNameError != null) {
							throw new CompatibilityException(appNameError);
						}
						CompatibilityCheckVersionsError versionsError = CheckVersions(versionInfoList);
						if(versionsError == null) {
							if(!ForceUpdateDatabase && !System.Diagnostics.Debugger.IsAttached) {
								return;
							}
						}
						else {
							if(versionsError is CompatibilityApplicationIsOldError) {
								throw new CompatibilityException(versionsError);
							}
							if(versionsError is CompatibilityDatabaseIsOldError) {
								CancelEventArgs args = new CancelEventArgs(false);
								OnConfirmation(args);
								if(args.Cancel) {
									throw new CompatibilityException(versionsError);
								}
							}
						}
						UpdateCore(updatingObjectSpace, versionInfoList);
						SetNewVersionInDB(updatingObjectSpace, versionInfoList);
					}
					finally {
						Tracing.Tracer.ResumeFlush();
					}
				}
			}
			catch(UnableToOpenDatabaseException e) {
				Exception ex = e;
				if(e.InnerException != null) {
					ex = e.InnerException;
				}
				throw new CompatibilityException(new CompatibilityUnableToOpenDatabaseError(ex));
			}
			catch(SchemaCorrectionNeededException e) {
				throw new CompatibilityException(new CompatibilityUnableToOpenDatabaseError(e));
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
				throw new CompatibilityException(new CompatibilityUnknownError(e));
			}
		}
		public Type ModuleInfoType {
			get { return moduleInfoType; }
		}
		public event EventHandler<CustomLoadVersionInfoListEventArgs> CustomLoadVersionInfoList;
		public override void Dispose() {
			base.Dispose();
			CustomLoadVersionInfoList = null;
		}
	}
	public class DatabaseSchemaUpdater : DatabaseUpdaterBase {
		public DatabaseSchemaUpdater(IObjectSpaceProvider objectSpaceProvider, IList<ModuleBase> modules, String applicationName, Type moduleInfoType)
			: this(objectSpaceProvider, modules) {
		}
		public DatabaseSchemaUpdater(IObjectSpaceProvider objectSpaceProvider, IList<ModuleBase> modules)
			: base(objectSpaceProvider, modules) {
				UseAllModuleUpdaters = true;
		}
		protected virtual Boolean IsDebuggerAttached {
			get { return System.Diagnostics.Debugger.IsAttached; }
		}
		protected override IList<CompatibilityError> CheckCompatibilityCore(bool getFirstVersionError) {
			List<CompatibilityError> result = new List<CompatibilityError>();
			try {
				IDatabaseSchemaChecker schemaChecker = objectSpaceProvider as IDatabaseSchemaChecker;
				if(schemaChecker != null) {
					Exception exception;
					DatabaseSchemaState schemaState = schemaChecker.CheckDatabaseSchemaCompatibility(out exception);
					switch(schemaState) {
						case DatabaseSchemaState.DatabaseMissing:
							result.Add(new CompatibilityUnableToOpenDatabaseError(exception));
							break;
						case DatabaseSchemaState.SchemaRequiresUpdate:
							result.Add(new CompatibilityDatabaseIsOldError(exception, null, new Version()));
							break;
						default:
						case DatabaseSchemaState.SchemaExists:
							break;
					}
				}
			}
			catch(Exception e) {
				Tracing.Tracer.LogError(e);
				result.Add(new CompatibilityUnknownError(e));
			}
			return result;
		}
		public override void Update() {
			Tracing.Tracer.LogSubSeparator("UpdateDB");
			try {
				if(!ForceUpdateDatabase && !IsDebuggerAttached) {
					IList<CompatibilityError> errors = CheckCompatibilityCore(false);
					if(errors.Count > 0) {
						CancelEventArgs args = new CancelEventArgs(false);
						OnConfirmation(args);
						if(args.Cancel) {
							throw new CompatibilityException(errors[0]);
						}
					}
					else {
						return;
					}
				}
				using(IObjectSpace updatingObjectSpace = objectSpaceProvider.CreateUpdatingObjectSpace(true)) {
					Tracing.Tracer.LogValue("ConnectionString", objectSpaceProvider.ConnectionString);
					Tracing.Tracer.LockFlush();
					UpdateCore(updatingObjectSpace, new List<IModuleInfo>());
				}
			}
			finally {
				Tracing.Tracer.ResumeFlush();
			}
		}
	}
	public class CompatibilityError {
		private string message;
		private Exception exception;
		public CompatibilityError(string message, Exception exception) {
			this.message = message;
			this.exception = exception;
		}
		public string Message {
			get { return message; }
		}
		public Exception Exception {
			get { return exception; }
		}
	}
	[Serializable]
	public class CompatibilityException : InvalidOperationException {
		private CompatibilityError error;
		protected CompatibilityException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public CompatibilityException(CompatibilityError error)
			: base(error.Message, error.Exception) {
			this.error = error;
		}
		public CompatibilityError Error {
			get { return error; }
		}
	}
	public class CompatibilityUnknownError : CompatibilityError {
		public CompatibilityUnknownError(Exception exception)
			: base(exception != null ? exception.Message : string.Empty, exception) {
		}
	}
	public class CompatibilityUnableToOpenDatabaseError : CompatibilityError {
		public CompatibilityUnableToOpenDatabaseError(Exception exception) : base(exception != null ? exception.Message : string.Empty, exception) { }
	}
	public class CompatibilityApplicationNameError : CompatibilityError {
		private String applicationName;
		private String applicationNameFromDatabase;
		private String moduleInfoTypeName;
		public CompatibilityApplicationNameError(Exception exception, String applicationName, String applicationNameFromDatabase, String moduleInfoTypeName)
			: base(String.Format(
				"Database is designed for the '{0}' application while you are running the '{1}' application.\r\n" +
				"To fix this problem, you should check the {2} table in your database " +
				"and find there a record with the True value in the IsMain column (usually this is the first record). " +
				"Then, you should find the value in the Name column of this record " +
				"and set this value to the ApplicationName property of your application class. " +
				"You can do this either via Application Designer or via code.", applicationNameFromDatabase, applicationName, moduleInfoTypeName), exception) {
			this.applicationName = applicationName;
			this.applicationNameFromDatabase = applicationNameFromDatabase;
			this.moduleInfoTypeName = moduleInfoTypeName;
		}
		public string ApplicationName {
			get { return applicationName; }
		}
		public string ApplicationNameFromDatabase {
			get { return applicationNameFromDatabase; }
		}
	}
	public abstract class CompatibilityCheckVersionsError : CompatibilityError {
		private ModuleBase module;
		private Version versionFromDB;
		public CompatibilityCheckVersionsError(string message, Exception exception) : base(message, exception) { }
		public CompatibilityCheckVersionsError(string message, Exception exception, ModuleBase module, Version versionFromDB)
			: base(message, exception) {
			this.module = module;
			this.versionFromDB = versionFromDB;
		}
		public ModuleBase Module {
			get { return module; }
		}
		public Version ModuleVersion {
			get { return (module != null) ? module.Version : null; }
		}
		public Version VersionFromDatabase {
			get { return versionFromDB; }
		}
	}
	public class CompatibilityDatabaseIsOldError : CompatibilityCheckVersionsError {
		public CompatibilityDatabaseIsOldError(Exception exception, ModuleBase module, Version versionFromDB)
			: base(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CompatibilityDatabaseIsOldError), exception, module, versionFromDB) { }
	}
	public class CompatibilityApplicationIsOldError : CompatibilityCheckVersionsError {
		public CompatibilityApplicationIsOldError(string message, Exception exception) :
			base(message, exception) { }
		public CompatibilityApplicationIsOldError(Exception exception, ModuleBase module, Version versionFromDB)
			: base(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CompatibilityApplicationIsOldError, module.Version.ToString(), module.Name, versionFromDB.ToString()), exception, module, versionFromDB) { }
	}
}
