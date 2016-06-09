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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Design.ItemTemplates;
using System.Collections.Generic;
using EnvDTE;
using System.IO;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Design {
	#region DesignTimeTransactionHelper
	public static class DesignTimeTransactionHelper {
		public static void InvokeTransactedChange(IComponent component, TransactedChangeCallback callback, object context, string description) {
			InvokeTransactedChange(component, callback, context, description, null);
		}
		public static void InvokeTransactedChange(IComponent component, TransactedChangeCallback callback, object context, string description, MemberDescriptor member) {
			if (component == null)
				throw new ArgumentNullException("component");
			InvokeTransactedChange(component.Site, component, callback, context, description, member);
		}
		public static void InvokeTransactedChange(IServiceProvider serviceProvider, IComponent component, TransactedChangeCallback callback, object context, string description, MemberDescriptor member) {
			if (component == null)
				throw new ArgumentNullException("component");
			if (callback == null)
				throw new ArgumentNullException("callback");
			if (serviceProvider == null)
				throw new ArgumentNullException("serviceProvider");
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			using (DesignerTransaction transaction = host.CreateTransaction(description)) {
				IComponentChangeService service = (IComponentChangeService)serviceProvider.GetService(typeof(IComponentChangeService));
				if (service != null) {
					try {
						service.OnComponentChanging(component, member);
					} catch (CheckoutException exception) {
						if (exception != CheckoutException.Canceled) {
							throw exception;
						}
						return;
					}
				}
				try {
					if (callback(context)) {
						if (service != null)
							service.OnComponentChanged(component, member, null, null);
						TypeDescriptor.Refresh(component);
						transaction.Commit();
					}
				} finally {
				}
			}
		}
	}
	#endregion
	#region ComponentChangeHelper
	public class ComponentChangeHelper : IDisposable {
		Component component;
		DesignerTransaction transaction;
		IComponentChangeService changeService;
		public ComponentChangeHelper(Component component, string transactionName) {
			this.component = component;
			Begin(transactionName);
		}
		public void Dispose() {
			End();
		}
		void Begin(string transactionName) {
			IDesignerHost host = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			changeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			transaction = host.CreateTransaction(transactionName);
			changeService.OnComponentChanging(component, null);
		}
		void End() {
			changeService.OnComponentChanged(component, null, null, null);
			transaction.Commit();
			IDisposable disp = transaction as IDisposable;
			if (disp != null)
				disp.Dispose();
		}
	}
	#endregion
	#region IDataFieldsProvider
	public interface IDataFieldsProvider {
		bool UnboundMode { get; }
		DataFieldInfoCollection GetDataFields();
	}
	#endregion
	#region DefaultDataFieldsProvider<T>
	public class DefaultDataFieldsProvider<T> : IDataFieldsProvider where T : IPersistentObject {
		IPersistentObjectStorage<T> objectStorage;
		public DefaultDataFieldsProvider(IPersistentObjectStorage<T> objectStorage) {
			this.objectStorage = objectStorage;
		}
		#region IDataFieldsProvider Members
		public bool UnboundMode { get { return objectStorage.UnboundMode; } }
		public DataFieldInfoCollection GetDataFields() {
			return ((IInternalPersistentObjectStorage<T>)objectStorage).DataManager.GetFieldInfos();
		}
		#endregion
	}
	#endregion
	#region ProjectLanguage
	public enum ProjectLanguage { VB, CS }
	#endregion
	#region ProjectManager
	internal class ProjectManager {
		public ProjectManager(ISite site) {
			ProjectItem = (EnvDTE.ProjectItem)site.GetService(typeof(EnvDTE.ProjectItem));
			Project = ProjectItem.ContainingProject;
		}
		public EnvDTE.ProjectItem ProjectItem { get; private set; }
		public EnvDTE.Project Project { get; private set; }
		public ProjectLanguage Language { get { return ObtainProjectLanguage(); } }
		ProjectLanguage ObtainProjectLanguage() {
			string language = Project.CodeModel.Language;
			if (language == EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB)
				return ProjectLanguage.VB;
			return ProjectLanguage.CS;
		}
		public ProjectItem AddProjectItem(ProjectItemTemplate projectItemInfo) {
			ProjectItem insertedItem = null;
			using (ItemTemplateFromAssemblyExtractor itemTemplateExtractor = new ItemTemplateFromAssemblyExtractor(typeof(SchedulerControlDesigner).Assembly, projectItemInfo)) {
				string pathToProjectItem = itemTemplateExtractor.GetTemplateFilePath();
				insertedItem = AddProjectItem(pathToProjectItem, projectItemInfo);
			}
			return insertedItem;
		}
		protected EnvDTE.ProjectItem AddProjectItem(string pathToProjectItem, ProjectItemTemplate itemTemplate) {
			int maxTrialCount = 100;
			string projectItemName = String.Empty;
			for (int indx = 0; indx < maxTrialCount; indx++) {
				projectItemName = itemTemplate.GetProjectItemName(indx);
				if (!IsProjectItemExists(projectItemName))
					break;
			}
			if (String.IsNullOrEmpty(projectItemName))
				return null;
			return AddProjectItem(pathToProjectItem, projectItemName);
		}
		protected EnvDTE.ProjectItem AddProjectItem(string pathToProjectItem, string itemName) {
			ProjectItem.ProjectItems.AddFromTemplate(pathToProjectItem, itemName);
			return SearchProjectItem(Project.ProjectItems, itemName);
		}
		EnvDTE.ProjectItem SearchProjectItem(EnvDTE.ProjectItems projectItems, string itemName) {
			foreach (EnvDTE.ProjectItem projectItem in projectItems) {
				EnvDTE.ProjectItem subItem = SearchProjectItem(projectItem.ProjectItems, itemName);
				if (subItem != null)
					return subItem;
				if (projectItem.Name == itemName)
					return projectItem;
			}
			return null;
		}
		public bool IsProjectItemExists(string itemName) {
			if (IsTargetFileExist(itemName))
				return true;
			EnvDTE.ProjectItem projectItem = SearchProjectItem(Project.ProjectItems, itemName);
			return projectItem != null;
		}
		bool IsTargetFileExist(string itemName) {
			DirectoryInfo startDirInfo = new DirectoryInfo(Path.GetDirectoryName(Project.FullName));
			return IsFileExists(startDirInfo, itemName);
		}
		bool IsFileExists(DirectoryInfo currentDir, string fileName) {
			foreach (DirectoryInfo dir in currentDir.GetDirectories()) 
				if (IsFileExists(dir, fileName))
					return true;
			foreach (FileInfo file in currentDir.GetFiles())
				if (file.Name == fileName)
					return true;
			return false;
		}
	}	
	#endregion
	public delegate void SimpleActionDelegate();
}
