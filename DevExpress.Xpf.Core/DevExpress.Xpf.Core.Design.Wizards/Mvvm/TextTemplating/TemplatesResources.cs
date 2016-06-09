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
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DevExpress.Design.Mvvm;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public class TemplatesResources {
		public const string STR_TemplatesNamespace = "DevExpress.Xpf.Core.Design.Wizards.Mvvm.Resources";
		public static IEnumerable<Type> GetViewModelCommonResourceNames(TemplateGenerationContext context) {
			yield return typeof(Resources.Common.Utils.DbExtensionsTemplate);
			yield return typeof(Resources.Common.Utils.ExpressionHelperTemplate);
			yield return typeof(Resources.Common.ViewModel.ISingleObjectViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.DocumentManagerServiceExtensionsTemplate);
			yield return typeof(Resources.Common.ViewModel.SingleObjectViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.ReadOnlyCollectionViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.CollectionViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.AddRemoveDetailEntitiesViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.AddRemoveJunctionDetailEntitiesViewModelTemplate);
			if (context.PlatformType == PlatformType.WPF) {
				yield return typeof(Resources.Common.ViewModel.InstantFeedbackCollectionViewModelTemplate);
			}
			yield return typeof(Resources.Common.ViewModel.PeekCollectionViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.MessagesTemplate);
			yield return typeof(Resources.Common.ViewModel.EntitiesViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.LookUpEntitiesViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.DocumentsViewModelTemplate);
			yield return typeof(Resources.Common.ViewModel.PersistentLayoutHelperTemplate);
		}
		public static Type GetViewModelResourceType(ViewModelType viewModelType) {
			switch(viewModelType) {
				case ViewModelType.Simple:
					return typeof(Resources.ViewModels.EmptyViewModelTemplate);
				case ViewModelType.Entity:
					return typeof(Resources.ViewModels.EntityEditViewModelTemplate);
				case ViewModelType.EntityRepository:
					return typeof(Resources.ViewModels.EntityRepositoryViewModelTemplate);
			}
			return null;
		}
		public static IEnumerable<Type> GetDataModelResources(TemplateGenerationContext context, DbContainerType containerType, bool withoutDesignTime) {
			List<Type> list = new List<Type>();
			list.Add(typeof(Resources.Common.Utils.ExpressionHelperTemplate));
			list.Add(typeof(Resources.Common.Utils.DbExtensionsTemplate));
			list.Add(typeof(Resources.Common.DataModel.IReadOnlyRepositoryTemplate));
			list.Add(typeof(Resources.Common.DataModel.IRepositoryTemplate));
			list.Add(typeof(Resources.Common.DataModel.IUnitOfWorkTemplate));
			if(!withoutDesignTime) {
				list.Add(typeof(Resources.Common.DataModel.DesignTime.DesignTimeReadOnlyRepositoryTemplate));
				list.Add(typeof(Resources.Common.DataModel.DesignTime.DesignTimeRepositoryTemplate));
				list.Add(typeof(Resources.Common.DataModel.DesignTime.DesignTimeUnitOfWorkTemplate));
				list.Add(typeof(Resources.Common.DataModel.DesignTime.DesignTimeInstantFeedbackSourceTemplate));
				list.Add(typeof(Resources.Common.DataModel.DesignTime.DesignTimeUnitOfWorkFactoryTemplate));
			}
			list.Add(typeof(Resources.Common.DataModel.UnitOfWorkBaseTemplate));
			list.Add(typeof(Resources.Common.DataModel.DbExceptionsTemplate));
			list.Add(GetDataModelTemplateType(typeof(Resources.Common.DataModel.EntityFramework.DbExceptionsConverterTemplate), typeof(Resources.Common.DataModel.WCF.DbExceptionsConverterTemplate), containerType));
			list.Add(GetDataModelTemplateType(typeof(Resources.Common.DataModel.EntityFramework.DbReadOnlyRepositoryTemplate), typeof(Resources.Common.DataModel.WCF.DbReadOnlyRepositoryTemplate), containerType));
			list.Add(GetDataModelTemplateType(typeof(Resources.Common.DataModel.EntityFramework.DbRepositoryTemplate), typeof(Resources.Common.DataModel.WCF.DbRepositoryTemplate), containerType));
			list.Add(GetDataModelTemplateType(typeof(Resources.Common.DataModel.EntityFramework.DbUnitOfWorkTemplate), typeof(Resources.Common.DataModel.WCF.DbUnitOfWorkTemplate), containerType));
			if(context.PlatformType != PlatformType.WinForms) {
				list.Add(GetDataModelTemplateType(typeof(Resources.Common.DataModel.EntityFramework.InstantFeedbackSourceTemplate), typeof(Resources.Common.DataModel.WCF.InstantFeedbackSourceTemplate), containerType));
			}
			list.Add(GetDataModelTemplateType(typeof(Resources.Common.DataModel.EntityFramework.DbUnitOfWorkFactoryTemplate), typeof(Resources.Common.DataModel.WCF.DbUnitOfWorkFactoryTemplate), containerType));
			list.Add(typeof(Resources.Common.DataModel.EntityStateTemplate));
			list.Add(typeof(Resources.DataModel.UnitOfWorkSourceTemplate));
			if(containerType == DbContainerType.WCF) {
				list.Add(typeof(Resources.Common.DataModel.WCF.QueryableWrapperTemplate));
			}
			return list;
		}
		public static string GetDataModelResourceName(string resourceFileName, DbContainerType containerType) {
			string subFolder = containerType == DbContainerType.WCF ? TemplatesConstants.STR_WCFSubFolder : TemplatesConstants.STR_EntityFrameworkSubFolder;
			return Path.Combine(subFolder, resourceFileName);
		}
		public static Type GetDataModelTemplateType(Type efType, Type wcfType, DbContainerType containerType) {
			return containerType == DbContainerType.WCF ? wcfType : efType;
		}
		static bool IsOutlookUIView(Type templateType) {
			return templateType.Name.EndsWith("View_OutlookTemplate") || templateType.Name.EndsWith("Outlook");
		}
		static bool IsWinUIView(Type templateType) {
			return templateType.Name.EndsWith("View_WinUITemplate") || templateType.Name.EndsWith("WinUI"); 
		}
		public static bool IsView(Type templateType) {
			return IsOutlookUIView(templateType) || IsWinUIView(templateType) || templateType.Name.EndsWith("ViewTemplate") || templateType.Name.EndsWith("TabbedMDI");
		}
		public static bool IsViewModel(Type templateType) {
			return templateType.Name.EndsWith("ViewModelTemplate");
		}
		public static Type GetViewResourceType(PlatformType platformType, ViewType viewType, UIType uiType) {
			if(platformType == PlatformType.WinForms) {
				switch(viewType) {
					case ViewType.None:
						throw new InvalidOperationException();
					case ViewType.Entity:
						switch(uiType) {
							case UIType.WindowsUI:   
								return typeof(Resources.Views.WinForms.WinUI.ElementView_WinUI);
							case UIType.OutlookInspired:
								return typeof(Resources.Views.WinForms.Outlook.ElementView_Outlook);
							default:
								return typeof(Resources.Views.WinForms.Standart.ElementView_TabbedMDI);
						}
					case ViewType.Repository:
						switch(uiType) {
							case UIType.WindowsUI: 
								return typeof(Resources.Views.WinForms.WinUI.CollectionView_WinUI);
							case UIType.OutlookInspired:
								return typeof(Resources.Views.WinForms.Outlook.CollectionView_Outlook);
							default:
								return typeof(Resources.Views.WinForms.Standart.CollectionView_TabbedMDI);
						}
				}
			}
			switch(viewType) {
				case ViewType.None:
					throw new InvalidOperationException();
				case ViewType.Entity:
					return GetViewResourceType(typeof(Resources.Views.ElementViewTemplate), typeof(Resources.Views.ElementView_WinUITemplate), uiType);
				case ViewType.Repository:
					return GetViewResourceType(typeof(Resources.Views.RepositoryViewTemplate), typeof(Resources.Views.RepositoryView_WinUITemplate), uiType);
			}
			return null;
		}
		public static Type GetDocumentManagerViewResourceType(PlatformType platformType, UIType uiType) {
			if(platformType == PlatformType.WinForms) {
				switch(uiType) {
					case UIType.WindowsUI:
						return typeof(Resources.Views.WinForms.WinUI.DocumentManagerView_WinUI);
					case UIType.OutlookInspired:
						return typeof(Resources.Views.WinForms.Outlook.DocumentManagerView_Outlook);
					default:
						return typeof(Resources.Views.WinForms.Standart.DocumentManagerView_TabbedMDI); 
				}
			}
			switch(uiType) {
				case UIType.OutlookInspired: return typeof(Resources.Views.DocumentManagerView_OutlookTemplate);
				case UIType.WindowsUI: return typeof(Resources.Views.DocumentManagerView_WinUITemplate);
				case UIType.Browser: return typeof(Resources.Views.DocumentManagerView_BrowserTemplate);
				default: return typeof(Resources.Views.DocumentManagerViewTemplate);
			}
		}
		public static Type GetViewResourceType(Type standardType, Type winUIType, UIType uiType) {
			return uiType == UIType.WindowsUI ? winUIType : standardType;
		}
	}
	public class WinFormsRealLookUpInfo :DevExpress.Mvvm.UI.Native.ViewGenerator.TypeNamePropertyPair {
		public WinFormsRealLookUpInfo(string foreignKeyProperty, string navigationProperty, string typeFullName, string propertyName)
			: base(typeFullName, propertyName) {
				foreignKeyPropertyInfoCore = foreignKeyProperty;
				navigationPropertyInfoCore = navigationProperty;
		}
		string foreignKeyPropertyInfoCore;
		public string ForeignKeyPropertyInfo { get { return foreignKeyPropertyInfoCore; } }
		string navigationPropertyInfoCore;
		public string NavigationPropertyInfo { get { return navigationPropertyInfoCore; } }
	}
}
