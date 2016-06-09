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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
namespace DevExpress.DataAccess.Design {
	public class VSConnectionStorageService : IConnectionStorageService {
		readonly IServiceProvider serviceProvider;
		readonly VSConnectionStringsService globalConnectionsService;
		public VSConnectionStorageService(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			globalConnectionsService = new VSConnectionStringsService(serviceProvider);
		}
		IConnectionStringInfo[] GetConnectionAccessors() {
			return globalConnectionsService.GetConnections();
		}
		#region IConnectionProviderService Members
		public IEnumerable<SqlDataConnection> GetConnections() {
			IEnumerable<SqlDataConnection> fromServerExplorer = GetConnectionsFromServerExplorer();
			IEnumerable<SqlDataConnection> fromAppConfig = GetConnectionsFromAppConfig();
			IEnumerable<SqlDataConnection> result = fromAppConfig == null ? fromServerExplorer.ToList() : fromAppConfig.Union(fromServerExplorer).ToList();
			foreach(SqlDataConnection connection in result)
				PatchConnectionString(connection);
			return result;
		}
		IEnumerable<SqlDataConnection> GetConnectionsFromServerExplorer() {
			foreach(IConnectionStringInfo connection in GetConnectionAccessors())
				if((connection.Location & DataConnectionLocation.ServerExplorer) == DataConnectionLocation.ServerExplorer) {
					SqlDataConnection sqlConnection = AppConfigHelper.CreateSqlConnection(connection, false);
					if(sqlConnection != null)
						yield return sqlConnection;
				}
		}
		IEnumerable<SqlDataConnection> GetConnectionsFromAppConfig() {
			string fileName = globalConnectionsService.GetFileName();
			if(fileName != null)
				try {
					return VSAppConfigHelper.GetConnections(globalConnectionsService).Values;
				}
				catch(ConfigurationException ex) {
					ILookAndFeelService lookAndFeelService = this.serviceProvider.GetService<ILookAndFeelService>();
					IUIService uiService = this.serviceProvider.GetService<IUIService>();
					IWin32Window win32Window = uiService != null ? uiService.GetDialogOwnerWindow() : null;
					IExceptionHandler exceptionHandler = new LoaderExceptionHandler(win32Window,
						lookAndFeelService.LookAndFeel) {
							Caption = "Configuration error",
							ErrorsMessage = "Cannot access the list of saved connections."
						};
					exceptionHandler.HandleException(ex);
				}
			return null;
		}
		public bool Contains(string connectionName) {
			IEnumerable<SqlDataConnection> list = GetConnectionsFromAppConfig();
			return list != null && list.Any(c => c.Name == connectionName);
		}
		public bool CanSaveConnection { get { return true; } }
		string SubLanguage {
			get {
				return ProjectHelper.IsWebProject(serviceProvider) ||
					   ProjectHelper.IsWebApplication(serviceProvider)
					? "CSharp/Web"
					: "CSharp";
			}
		}
		public void SaveConnection(string name, IDataConnection connection, bool saveCredentials) {
			Project project = ProjectHelper.GetActiveProject(serviceProvider);
			string configFileName = this.globalConnectionsService.GetFileName();
			try {
				if(configFileName == null) {
					configFileName = globalConnectionsService.ConfigFileName;
					project.DTE.SuppressUI = true;
					string templatePath =
						((Solution2)project.DTE.Solution).GetProjectItemTemplate(
							globalConnectionsService.ConfigTemplateName, SubLanguage);
					project.ProjectItems.AddFromTemplate(templatePath, configFileName);
					configFileName = Path.Combine((string)project.Properties.Item("FullPath").Value, configFileName);
					project.DTE.ActiveDocument.Close(vsSaveChanges.vsSaveChangesYes);
					project.DTE.SuppressUI = false;
				}
				else {
					FileInfo configFile = new FileInfo(configFileName);
					IVsQueryEditQuerySave2 queryEditService = (IVsQueryEditQuerySave2)serviceProvider.GetService(typeof(SVsQueryEditQuerySave));
					tagVSQueryEditResult pfEditVerdict;
					tagVSQueryEditResultFlags prgfMoreInfo;
					int exitcode = queryEditService.QueryEditFiles(0, 1, new[] { configFile.FullName }, null, null, out pfEditVerdict, out prgfMoreInfo);
					if(exitcode != VSConstants.S_OK)
						throw new Exception(string.Format("Microsoft.VisualStudio.Shell.Interop.IVsQueryEditQuerySave2.QueryEditFiles call finished with exitcode {0}", exitcode));
					if(pfEditVerdict != tagVSQueryEditResult.QER_EditOK)
						throw new IOException(string.Format("Changes at '{0}' are not allowed", configFile.FullName));
				}
				VSAppConfigHelper.SaveConnection(name, configFileName, connection, saveCredentials);
			}
			catch(Exception exception) {
				connection.StoreConnectionNameOnly = false;
				if(!saveCredentials)
					connection.BlackoutCredentials();
				IUIService uiService = serviceProvider.GetService<IUIService>();
				ILookAndFeelService lookAndFeelService = this.serviceProvider.GetService<ILookAndFeelService>();
				ExceptionHandler exceptionHandler = new ExceptionHandler(lookAndFeelService.LookAndFeel, uiService.GetDialogOwnerWindow(), "Cannot save the connection.");
				exceptionHandler.HandleException(exception);
			}
		}
		void PatchConnectionString(SqlDataConnection connection) {
			if(connection != null && connection.HasConnectionString)
				connection.ConnectionString = globalConnectionsService.PatchConnectionString(connection.ConnectionString);
		}
		#endregion
	}
	[Guid("53544C4D-5984-11D3-A606-005004775AB1")]
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IVsQueryEditQuerySave2 {
		[PreserveSig]
		int QueryEditFiles(
			tagVSQueryEditFlags rgfQueryEdit,
			int cFiles,
			[In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 1)] string[] rgpszMkDocuments,
			[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)]tagVSQEQSFlags[] rgrgf,
			[In, MarshalAs(UnmanagedType.LPArray, ArraySubType = (UnmanagedType)80, SizeParamIndex = 1)] VSQEQS_FILE_ATTRIBUTE_DATA[] rgFileInfo,
			out tagVSQueryEditResult pfEditVerdict,
			out tagVSQueryEditResultFlags prgfMoreInfo);
	}
}
