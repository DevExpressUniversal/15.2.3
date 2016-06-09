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
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Tools.DBUpdater {
	class Program {
		private const String STR_Silent = "-silent";
		private const String STR_ConnectionString = "-connectionString";
		private const String STR_ForceUpdate = "-forceUpdate";
		private enum ExitCode : int { UpdateCompleted = 0, UpdateError = 1, UpdateNotNeeded = 2 }
		private const String updateNotNeeded = "The database doesn't need to be updated";
		private const String updateCompleted = "Database update completed successfully";
		private const String updateError = "The database can't be updated";
		private static String usage = String.Format(@"DBUpdater.exe [{0}] ""Path to the application configuration file"" [""Path to the application assembly file""] [{1} ""Custom connection string""] [{2}]", STR_Silent, STR_ConnectionString, STR_ForceUpdate);
		private static void GetDBUpdaterParameters(string configFileName, out string connectionString, out string modules, out string tablePrefixes) {
			connectionString = null;
			modules = "";
			tablePrefixes = null;
			ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
			exeConfigurationFileMap.ExeConfigFilename = configFileName;
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
			if(configuration.ConnectionStrings.ConnectionStrings["connectionString"] != null) {
				connectionString = configuration.ConnectionStrings.ConnectionStrings["connectionString"].ConnectionString;
			}
			if(configuration.AppSettings.Settings["Modules"] != null) {
				modules = configuration.AppSettings.Settings["Modules"].Value;
			}
			if(configuration.AppSettings.Settings["TablePrefixes"] != null) {
				tablePrefixes = configuration.AppSettings.Settings["TablePrefixes"].Value;
			}
		}
		private static void WriteHeader() {
			DBUpdaterTracing.WriteLine(String.Format("{0} eXpressApp Framework Database Updater. Version: {1}", AssemblyInfo.AssemblyCompany, AssemblyInfo.FileVersion));
			DBUpdaterTracing.WriteLine(string.Format("{0} All rights reserved.", AssemblyInfo.AssemblyCopyright));
			DBUpdaterTracing.WriteLine();
		}
		private static int Main(string[] args) {
			ExitCode exitCode = ExitCode.UpdateError;
			WriteHeader();
			if(args.Length == 0) {
				DBUpdaterTracing.WriteLine(string.Format("Usage: {0}", usage));
				DBUpdaterTracing.WriteLine("Exit codes: 0 - " + updateCompleted + ".");
				DBUpdaterTracing.WriteLine("            1 - " + updateError + ".");
				DBUpdaterTracing.WriteLine("            2 - " + updateNotNeeded + ".");
			}
			else {
				try {
					string configFileName = null;
					bool silent = false;
					string applicationFileName = null;
					string customConnectionString = null;
					if(args[0].ToLower() == STR_Silent.ToLower()) {
						silent = true;
						DBUpdaterTracing.WriteLine("Silent mode ON");
						if(args.Length > 1) {
							configFileName = args[1];
							if((args.Length > 2)
								&& (args[2].ToLower() != STR_ConnectionString.ToLower())
								&& (args[2].ToLower() != STR_ForceUpdate.ToLower())) {
								applicationFileName = args[2];
							}
						}
						else {
							throw new Exception("The application configuration file parameter couldn't be found.");
						}
					}
					else {
						DBUpdaterTracing.WriteLine("Silent mode OFF");
						configFileName = args[0];
						if((args.Length > 1)
							&& (args[1].ToLower() != STR_ConnectionString.ToLower())
							&& (args[1].ToLower() != STR_ForceUpdate.ToLower())) {
							applicationFileName = args[1];
						}
					}
					if(!File.Exists(configFileName)) {
						Exception e = null;
						try {
							configFileName = Path.Combine(System.Environment.CurrentDirectory, configFileName);
							configFileName = Path.GetFullPath(configFileName);
						}
						catch(Exception ex) {
							e = ex;
						}
						if(!File.Exists(configFileName) || (e != null)) {
							throw new Exception(String.Format("The application configuration file '{0}' couldn't be found.", configFileName), e);
						}
					}
					if(!string.IsNullOrEmpty(applicationFileName)) {
						Exception e = null;
						try {
							applicationFileName = Path.GetFullPath(applicationFileName);
						}
						catch(Exception ex) {
							e = ex;
						}
						if(!File.Exists(applicationFileName) || (e != null)) {
							throw new Exception(String.Format("The application assembly file '{0}' couldn't be found.", applicationFileName), e);
						}
					}
					for(Int32 i = 0; i < args.Length; i++) {
						if(args[i].ToLower() == STR_ConnectionString.ToLower()) {
							if((i + 1) <= (args.Length - 1)) {
								customConnectionString = args[i + 1];
								if(String.IsNullOrEmpty(customConnectionString)) {
									throw new Exception("The specified connection string parameter value is empty.");
								}
								break;
							}
							else {
								throw new Exception("The connection string parameter couldn't be found. Ensure that it is enclosed in double quotes.");
							}
						}
					}
					Boolean forceUpdate = false;
					for(Int32 i = 0; i < args.Length; i++) {
						if(args[i].ToLower() == STR_ForceUpdate.ToLower()) {
							forceUpdate = true;
						}
					}
					string connectionString;
					string modules;
					string tablePrefixes;
					GetDBUpdaterParameters(configFileName, out connectionString, out modules, out tablePrefixes);
					if(!String.IsNullOrEmpty(customConnectionString)) {
						connectionString = customConnectionString;
					}
					string diffsPath = Path.GetDirectoryName(configFileName);
					if(configFileName.ToLower().IndexOf("web.config") == -1) {
						diffsPath = Path.Combine(diffsPath, @"..\..\");
					}
					DesignerModelFactory dmf = new DesignerModelFactory();
					string assembliesPath = string.Empty;
					try {
						dmf.GetFileContainingApplicationType(configFileName, applicationFileName, ref assembliesPath);
					}
					catch(DesignerModelFactory_FindApplicationAssemblyException e) {
						throw new Exception(e.Message + Environment.NewLine +
							"To avoid ambiguity, specify assembly file name as the third parameter." + Environment.NewLine + usage);
					}
					XafApplication application = dmf.CreateApplicationByConfigFile(configFileName, applicationFileName, ref assembliesPath);
					ISupportSplashScreen temp = application as ISupportSplashScreen;
					if(temp != null) {
						temp.RemoveSplash();
					}
					if(!string.IsNullOrEmpty(connectionString)) {
						application.ConnectionString = connectionString;
					}
					else {
						DBUpdaterTracing.WriteLine("ConnectionString is not found in the configuration file. DBUpdater will use the ConnectionString specified in the Application designer.");
					}
					if(tablePrefixes != null) {
						application.TablePrefixes = tablePrefixes;
					}
					application.Setup(application.ApplicationName, application.ConnectionString, modules.Split(';'));
					application.StatusUpdating += new EventHandler<StatusUpdatingEventArgs>(Application_StatusUpdating);
					foreach(IObjectSpaceProvider provider in application.ObjectSpaceProviders) {
						if(provider.SchemaUpdateMode == SchemaUpdateMode.DatabaseAndSchema) {
							DatabaseUpdaterBase dbUpdater = application.CreateDatabaseUpdater(provider);
							exitCode = ProcessObjectSpaceProvider(dbUpdater, provider.ConnectionString, forceUpdate, silent);
						}
					}
					application.StatusUpdating -= new EventHandler<StatusUpdatingEventArgs>(Application_StatusUpdating);					
					application.Dispose();
				}
				catch(Exception e) {
					exitCode = ExitCode.UpdateError;
					DBUpdaterTracing.WriteException(e, updateError);
				}
			}
			return (int)exitCode;
		}
		private static ExitCode ProcessObjectSpaceProvider(DatabaseUpdaterBase dbUpdater, string connectionString, Boolean forceUpdate, Boolean silent) {
			ExitCode exitCode = ExitCode.UpdateError;
			dbUpdater.StatusUpdating += new EventHandler<StatusUpdatingEventArgs>(DBUpdater_StatusUpdating);
			DBUpdaterTracing.WriteLine("Updating database via the following connection string:");
			DBUpdaterTracing.WriteLine(connectionString);
			DBUpdaterTracing.WriteLine();
			IList<CompatibilityError> errors = dbUpdater.CheckCompatibilityForAllModules();
			if(forceUpdate || dbUpdater is DatabaseSchemaUpdater) {
				if(errors == null) {
					errors = new List<CompatibilityError>();
				}
				errors.Add(new CompatibilityDatabaseIsOldError(null, null, null));
			}
			if((errors != null) && (errors.Count > 0)) {
				exitCode = UpdateDataBase(errors, dbUpdater, silent);
			}
			else {
				DBUpdaterTracing.WriteLine(updateNotNeeded + ".");
				exitCode = ExitCode.UpdateNotNeeded;
			}
			dbUpdater.StatusUpdating -= new EventHandler<StatusUpdatingEventArgs>(DBUpdater_StatusUpdating);
			return exitCode;
		}
		private static bool CanUpdateDataBase(IList<CompatibilityError> errors) {
			foreach(CompatibilityError error in errors) {
				if(error is CompatibilityApplicationIsOldError) {
					return false;
				}
			}
			return true;
		}
		private static bool CheckErorsByErrorType(IList<CompatibilityError> errors, params Type[] errorTypes) {
			bool result = false;
			foreach(CompatibilityError error in errors) {
				foreach(Type errorType in errorTypes) {
					if(errorType.IsAssignableFrom(error.GetType())) {
						result = true;
						break;
					}
				}
				if(result) {
					break;
				}
			}
			return result;
		}
		private static void PrintErrors(IList<CompatibilityError> errors) {
			foreach(CompatibilityError error in errors) {
				DBUpdaterTracing.WriteLine(error.Message);
				DBUpdaterTracing.WriteLine();
			}
		}
		private static void PrintErrorsCustomFormat(IList<CompatibilityError> errors) {
			foreach(CompatibilityError error in errors) {
				CompatibilityCheckVersionsError compatibilityCheckVersionsError = error as CompatibilityCheckVersionsError;
				if(compatibilityCheckVersionsError != null) {
					if(compatibilityCheckVersionsError.Module != null &&
						compatibilityCheckVersionsError.ModuleVersion != null &&
						compatibilityCheckVersionsError.VersionFromDatabase != null) {
						DBUpdaterTracing.WriteLine(
							String.Format("The module '{0}' has version '{1}', the version from the database is '{2}'.",
							compatibilityCheckVersionsError.Module.Name, compatibilityCheckVersionsError.ModuleVersion, compatibilityCheckVersionsError.VersionFromDatabase));
					}
				}
			}
		}
		private static ExitCode UpdateDataBase(IList<CompatibilityError> errors, DatabaseUpdaterBase dbUpdater, bool silent) {
			if(CheckErorsByErrorType(errors, typeof(CompatibilityCheckVersionsError), typeof(CompatibilityUnableToOpenDatabaseError))) {
				if(CheckErorsByErrorType(errors, typeof(CompatibilityCheckVersionsError))) {
					if(CanUpdateDataBase(errors)) {
						PrintErrorsCustomFormat(errors);
						if(!silent) {
							DBUpdaterTracing.WriteLine("Please backup the database before the update and press <Enter>.");
							Console.ReadLine();
						}
					}
					else {
						PrintErrors(errors);
						DBUpdaterTracing.WriteLine(updateNotNeeded + ".");
						return ExitCode.UpdateNotNeeded;
					}
				}
				else {
					if(!silent) {
						DBUpdaterTracing.WriteLine("The database doesn't exist. It'll be created now.");
						DBUpdaterTracing.WriteLine();
					}
				}
				dbUpdater.Update();
				DBUpdaterTracing.WriteLine();
				DBUpdaterTracing.WriteLine(updateCompleted + ".");
				DBUpdaterTracing.WriteLine();
				if(!silent) {
					DBUpdaterTracing.WriteLine("Please disconnect all connected users and press <Enter>.");
					Console.ReadLine();
				}
				return ExitCode.UpdateCompleted;
			}
			else {
				DBUpdaterTracing.WriteLine(updateError + ":");
				PrintErrors(errors);
				return ExitCode.UpdateError;
			}
		}
		private static void DBUpdater_StatusUpdating(Object sender, StatusUpdatingEventArgs e) {
			DBUpdaterTracing.WriteLine(e.Message);
		}
		private static void Application_StatusUpdating(Object sender, StatusUpdatingEventArgs e) {
			DBUpdaterTracing.WriteLine(e.Message);
		}
	}
}
