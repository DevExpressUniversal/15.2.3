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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public static class SR_Mvvm {
		public const string Wizard_LearnMore = "Learn more...";
		public const string Wizard_LearnMoreNavigateUri = @"http://documentation.devexpress.com/#WPF/CustomDocument17425";
		public const string DataAccessLayerWizard_Title = "Entity Framework Data Model";
		public const string DataAccessLayerWizard_DataSourceListHeader = "Select Entity Framework Model";
		public const string DataAccessLayerWizard_RebuildNotice = "If a model is missing from the list, cancel the wizard and rebuild the project that contains the model.";
		public const string DataAccessLayerWizard_CreateNewEntityModel = "New ADO.NET Entity Data Model...";
		public const string DataAccessLayerWizard_AnnotationText =
			"Entity Framework Data Model code is generated based on entity classes available in the Entity Framework Model." +
			" Entity Framework Data Model encapsulates data access to real data through interfaces and provides implementataion to access real DB data at runtime an fake data at design-time. This dialog also lets you to specify Primary key(s) and Navigation properties avalable in the Entity Model to include into the Data Access Layer." +
			" Data access layer also enables you to provide fake data for unit tests.";
		public const string DataAccessLayerWizard_OldEntityFrameworkWarningText =
			"This model uses the Entity Framework version below 6.0.\r\nTo use this model, cancel the wizard, and convert your data model project to the Entity Framework version 6.0 or higher.";
		public const string DataAccessLayerWizard_SelectTables = "Select tables and views";
		public const string DataAccessLayerWizard_DisclaimerText = @"Entity Framework Data Model determines Primary Keys and Collection Properties based on *.edmx files ([Data Base First](http://msdn.microsoft.com/en-us/data/jj206878.aspx) and [Model First](http://msdn.microsoft.com/en-us/data/ff830362.aspx) workflows) or Entity Framework Conventions and Data Annotation attributes ([Code First](http://msdn.microsoft.com/en-US/data/jj679962) workflow).";
		public const string DataAccessLayerWizard_PrimaryKey = "Primary Key:";
		public const string DataAccessLayerWizard_ReadOnly = "Read only";
		public const string DataAccessLayerWizard_CollectionProperties = "Collection Properties:";
		public const string DataAccessLayerWizard_RebuildMessage = "After a new data source is created rebuild the current solution and run the {0} Wizard to proceed.";
		public const string ViewModelWizard_ListHeader = "Select View Model Type";
		const string STR_Entity = "Business Object";
		const string STR_Repository = "Collection";
		const string EntityViewModelTypeDescriptionText = @"This View Model provides the capability to change, save, and delete single object.";
		const string EntityRepositoryViewModelTypeDescriptionText = @"This View Model provides CRUD operations on the object collection.";
		public static string ViewModelWizard_Title = "ViewModel Wizard";
		public static string SingleObjectViewModelWizard_Title = "Business Object View Model Wizard";
		public static string CollectionViewModelWizard_Title = "Collection View Model Wizard";
		public static string ViewModelWizard_EntityViewModelDescriptionText {
			get { return EntityViewModelTypeDescriptionText; }
		}
		public static string ViewModelWizard_EntityRepositoryViewModelDescriptionText {
			get { return EntityRepositoryViewModelTypeDescriptionText; }
		}
		public const string ViewModelWizard_SelectEntityType = "Select Entity Type";
		public const string ViewModelWizard_ViewModelName = "View Model Class Name:";
		public const string ViewModelWizard_DisclaimerText = @"To guarantee Wizard works correctly with [Fluent API](http://msdn.microsoft.com/en-us/library/hh295844%28v=vs.103%29.aspx) or [Compiled Models](http://msdn.microsoft.com/en-us/library/system.data.entity.infrastructure.dbcompiledmodel.aspx) schemes please check data below and correct it if needed.";
		public const string Wizard_TitleFormat = "{0} - {1}";
		public const string ViewWizard_SelectViewModelType = "Select View Model";
		public const string ViewWizard_CreateNewViewModel = "Create New View Model";
		public const string ViewWizard_SelectViewType = "Select View Type";
		public const string ViewWizard_Title = "View Wizard";
		public const string SingleObjectViewWizard_Title = "Business Object View Wizard";
		public const string CollectionViewWizard_Title = "Collection View Wizard";
		public const string ViewWizard_MessageDialogText = "There is no any view model or any EntityFramework model. Do you want to create new EntityFramework model. " +
			"Warning: after a new EntityFramework model is created rebuild current solution and run Wizard to proceed.";
		public const string ViewWizard_LoadinTypesText = "Loading Types ...";
		public const string ViewWizard_ViewNameOption = "View Name:";
		public const string ViewWizard_EntityViewShortDescription = @"Business Object View XAML code is generated based on the existing View Model that exposes a public entity property. The view displays entity properties via [LayoutControl](http://documentation.devexpress.com/#WPF/clsDevExpressXpfLayoutControlLayoutControltopic). Commands available in View Model are represented via buttons in [RibbonControl](http://documentation.devexpress.com/#WPF/clsDevExpressXpfRibbonRibbonControltopic).";
		public const string ViewWizard_RepositoryViewShortDescription = @"Collection View XAML code is generated based on the existing View Model that exposes a public entity collection property. The view displays entity collection via [GridControl](http://documentation.devexpress.com/#WPF/clsDevExpressXpfGridGridControltopic). Commands available in View Model are represented via buttons in [RibbonControl](http://documentation.devexpress.com/#WPF/clsDevExpressXpfRibbonRibbonControltopic).";
		public const string ViewWizard_EntityWinUIViewShortDescription = @"Business Object View XAML code is generated based on the existing View Model that exposes a public entity property. The view displays entity properties via [LayoutControl](http://documentation.devexpress.com/#WPF/clsDevExpressXpfLayoutControlLayoutControltopic). Commands available in View Model are represented via buttons in [AppBar](http://help.devexpress.com/#WPF/CustomDocument16058).";
		public const string ViewWizard_RepositoryWinUIViewShortDescription = @"Collection View XAML code is generated based on the existing View Model that exposes a public entity collection property. The view displays entity collection via [GridControl](http://documentation.devexpress.com/#WPF/clsDevExpressXpfGridGridControltopic). Commands available in View Model are represented via buttons in [AppBar](http://help.devexpress.com/#WPF/CustomDocument16058).";
		public const string ViewWizard_ShowTypesFromActiveProjectOnly = "Show Types from Active Project only";
		public const string StartPageDescription = "Choose Model";
		public const string SelectingEntitiesPageDescription = "Configure Data Model";
		public const string ViewModelProperties = "Configure View Model";
		public const string ViewProperties = "Configure View";
		public const string ShowSolutionClassesOnly = "Show solution classes only";
		public const string TabbedMDIWizard_Title = "Data Bound Tabbed MDI View";
		public const string TabbedMDIWizard_DisclaimerText = @"This wizard creates a Tabbed MDI UI that performs CRUD operations against all selected data tables.";
		public const string MDIWizard_Title = "Data Bound MDI View";
		public const string MDIWizard_DisclaimerText = @"This wizard creates a MDI UI that performs CRUD operations against all selected data tables.";
		public const string OutlookInspiredWizard_Title = "Data Bound Outlook Inspired MDI View";
		public const string OutlookInspiredWizard_DisclaimerText = @"This wizard creates an Outlook Inspired MDI UI that performs CRUD operations against all selected data tables.";
		public const string HybridUIWizard_Title = "Data Bound Hybrid UI View";
		public const string HybridUIWizard_DisclaimerText = @"This wizard creates a Hybrid UI that performs CRUD operations against all selected data tables.";
		public const string BrowserUIWizard_Title = "Data Bound Browser View";
		public const string BrowserUIWizard_DisclaimerText = @"This wizard creates a Browser that performs CRUD operations against all selected data tables.";
		public const string WinFormsWizard_Title = "Data Bound View Model";
		public const string WinFormsWizard_DisclaimerText = @"This wizard creates a view model that performs CRUD operations against all selected data tables.";
		public const string TabbedMDIWizard_SelectTablesText = "Select Tables and Views";
		public const string TabbedMDIWizard_SelectTablesPageDescription = "Select Tables and Views";
		public const string TabbedMDIWizard_SelectUIType = "Select UI Type";
		public const string WCFDataServiceModel = "WCF Data Service Model";
		public const string EntityFrameworkModel = "Entity Framework Model";
		public const string DataModel = "Data Model";
		public const string ViewModel = "View Model";
		public const string RebuildQuestion = "Do you want to rebuild the current project?";
		public const string EmptyEntityFrameworkModelText = "Entity Framework Data Model is empty or contains errors.";
		public const string SelectAll = "Select All";
		static string GetWizardTitleWPF(MvvmConstructorContext context) {
			switch(context.TaskType) {
				case TaskType.DataLayer:
					return SR_Mvvm.DataAccessLayerWizard_Title;
				case TaskType.ViewModelLayer:
					switch(context.SelectedViewModelType) {
						case ViewModelType.Entity:
							return SR_Mvvm.SingleObjectViewModelWizard_Title;
						case ViewModelType.EntityRepository:
							return SR_Mvvm.CollectionViewModelWizard_Title;
					}
					return SR_Mvvm.ViewModelWizard_Title;
				case TaskType.ViewLayer:
					switch(context.SelectedViewType) {
						case ViewType.Entity:
							return SR_Mvvm.SingleObjectViewWizard_Title;
						case ViewType.Repository:
							return SR_Mvvm.CollectionViewWizard_Title;
					}
					return SR_Mvvm.ViewWizard_Title;
				case TaskType.TabbedMDI:
					switch(context.SelectedUIType) {
						case UIType.WindowsUI:
							return SR_Mvvm.HybridUIWizard_Title;
						case UIType.OutlookInspired:
							return SR_Mvvm.OutlookInspiredWizard_Title;
						case UIType.Standard:
							return SR_Mvvm.TabbedMDIWizard_Title;
						case UIType.Browser:
							return SR_Mvvm.BrowserUIWizard_Title;
						default:
							Debug.Fail("unknown UIType");
							return SR_Mvvm.TabbedMDIWizard_Title;
					}
			}
			return string.Empty;
			}
		static string GetWizardTitleWinForms(MvvmConstructorContext context) {
			switch(context.TaskType) {
				case TaskType.DataLayer:
					return SR_Mvvm.DataAccessLayerWizard_Title;
				case TaskType.ViewModelLayer:
					switch(context.SelectedViewModelType) {
						case ViewModelType.Entity:
							return SR_Mvvm.SingleObjectViewModelWizard_Title;
						case ViewModelType.EntityRepository:
							return SR_Mvvm.CollectionViewModelWizard_Title;
					}
					return SR_Mvvm.ViewModelWizard_Title;
				case TaskType.ViewLayer:
					switch(context.SelectedViewType) {
						case ViewType.Entity:
							return SR_Mvvm.SingleObjectViewWizard_Title;
						case ViewType.Repository:
							return SR_Mvvm.CollectionViewWizard_Title;
					}
					return SR_Mvvm.ViewWizard_Title;
				case TaskType.TabbedMDI:
					switch(context.SelectedUIType) {
						case UIType.WindowsUI:
							return SR_Mvvm.HybridUIWizard_Title;
						case UIType.OutlookInspired:
							return SR_Mvvm.OutlookInspiredWizard_Title;
						case UIType.Standard:
							return SR_Mvvm.TabbedMDIWizard_Title;
						case UIType.Browser:
							return SR_Mvvm.BrowserUIWizard_Title;
						default:
							Debug.Fail("unknown UIType");
							return SR_Mvvm.TabbedMDIWizard_Title;
					}
			}
			return string.Empty;
		}
		public static string GetWizardTitle(MvvmConstructorContext context) {
			if(context == null)
				return string.Empty;
			switch(context.PlatformType) {
				case PlatformType.WinForms:
					return GetWizardTitleWinForms(context);
				case PlatformType.WPF:
					return GetWizardTitleWPF(context);
			}
			return string.Empty;
		}
	}
}
