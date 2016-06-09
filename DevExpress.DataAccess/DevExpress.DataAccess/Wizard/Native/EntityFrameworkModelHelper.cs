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
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.Model;
using DevExpress.Entity.ProjectModel;
namespace DevExpress.DataAccess.Wizard.Native {
	public class EntityFrameworkModelHelper : IEntityFrameworkModelHelper {
		public static EntityFrameworkModelHelper Create(ISolutionTypesProvider solutionTypesProvider, IExceptionHandler exceptionHandler, IWaitFormActivator waitFormActivator) {
			Exception entityModelException = null;
			EntityFrameworkModelHelper efModelHelper = new EntityFrameworkModelHelper(solutionTypesProvider);
			waitFormActivator.ShowWaitForm(false, true, true);
			waitFormActivator.SetWaitFormCaption(DataAccessLocalizer.GetString(DataAccessStringId.LoadingDataPanelText));
			waitFormActivator.SetWaitFormDescription(string.Empty);
			waitFormActivator.EnableCancelButton(false);
			try {
				efModelHelper.RefreshEntityModel();
			} catch(Exception ex) {
				entityModelException = ex;
			} finally {
				waitFormActivator.CloseWaitForm();
			}
			if(entityModelException == null)
				return efModelHelper;
			exceptionHandler.HandleException(entityModelException);
			return null;
		}
		ISolutionTypesProvider solutionTypesProvider;
		IEnumerable<IContainerInfo> containers;
		DataAccessEntityFrameworkModel entityFrameworkModel;
		public IEnumerable<IContainerInfo> Containers { get { return containers; } }
		public DataAccessEntityFrameworkModel EntityFrameworkModel { get { return entityFrameworkModel; } }
		public bool IsCustomAssembly { get; private set; }
		public string CustomAssemblyPath { get; private set; }
		public string[] ReferencedAssemblies { get; private set; }
		public EntityFrameworkModelHelper(ISolutionTypesProvider solutionTypesProvider) {			
			this.solutionTypesProvider = solutionTypesProvider;
		}
		public void RefreshEntityModel() {
			entityFrameworkModel = new DataAccessEntityFrameworkModel(this.solutionTypesProvider);
			containers = EntityFrameworkModel.GetContainersInfo() ?? Enumerable.Empty<IContainerInfo>();
		}
		public bool OpenAssembly(string filePath, IExceptionHandler exceptionHandler) {
			Assembly assembly = Assembly.LoadFile(filePath);
			try {
				AppDomain currentDomain = AppDomain.CurrentDomain;
				ResolveEventHandler resolveFunction = (s, args) => {
					if(args.Name.StartsWith("EntityFramework")) {
						string folderPath = Path.GetDirectoryName(filePath);
						if(folderPath == null)
							return null;
						string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
						return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
					}
					return null;
				};
				currentDomain.AssemblyResolve += resolveFunction;
				try {
					CustomAssemblyPath = filePath;
					IsCustomAssembly = true;
					Type[] assemblyLoadFileGetTypes = assembly.GetTypes();
					ReferencedAssemblies = assembly.GetReferencedAssemblies().Where(an => an.FullName.StartsWith("EntityFramework")).Select(an => an.Name).ToArray();
					SolutionTypesProviderConsole solutionTypesProviderConsole = new SolutionTypesProviderConsole(assembly.FullName);
					solutionTypesProviderConsole.AddRange(assemblyLoadFileGetTypes);
					solutionTypesProvider = solutionTypesProviderConsole;
					RefreshEntityModel();
				} finally {
					currentDomain.AssemblyResolve -= resolveFunction;
				}
			} catch(ReflectionTypeLoadException ex) {
				exceptionHandler.HandleException(ex.LoaderExceptions.Length == 1 ? ex.LoaderExceptions[0] : new AggregateException(ex.LoaderExceptions));
				return false;
			}
			return true;
		}
	}
}
