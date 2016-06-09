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
using System.Linq;
using System.Text;
using DevExpress.Utils.Localization.Internal;
using System.Resources;
using DevExpress.Utils.Localization;
namespace DevExpress.Xpf.Core.Design.Wizards {
	public abstract class DXLocalizer<T> : XtraLocalizer<T> where T : struct {
		protected override void AddString(T id, string str) {
			Dictionary<T, string> table = XtraLocalizierHelper<T>.GetStringTable(this);
			table[id] = str;
		}
	}
	public abstract class DXResXLocalizer<T> : XtraResXLocalizer<T> where T : struct {
		protected DXResXLocalizer(XtraLocalizer<T> embeddedLocalizer)
			: base(embeddedLocalizer) {
		}
#if DEBUGTEST
		public new string GetLocalizedStringFromResources(T id) {
			return base.GetLocalizedStringFromResources(id);
		}
#endif
	}
	public class ItemsSourceWizardLocalizer : DXLocalizer<ItemsSourceWizardStringId> {
		static ItemsSourceWizardLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<ItemsSourceWizardStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(ItemsSourceWizardStringId.WizardTitle, "Items Source Configuration Wizard");
			AddString(ItemsSourceWizardStringId.TechnologyCaption, "Data Access Technology");
			AddString(ItemsSourceWizardStringId.DataSourceTypeCaption, "Data Processing Mode");
			AddString(ItemsSourceWizardStringId.DataSourcesCaption, "Data Source");
			AddString(ItemsSourceWizardStringId.IEnumerableText, "Click Next to proceed to configure a data source.");
			AddString(ItemsSourceWizardStringId.CreateNewItemText, "After a new data source is created, rebuild the current solution and run the Items Source Configuration Wizard to proceed.");
			AddString(ItemsSourceWizardStringId.AvaliableDataSourceText, "No available data sources are found in the current project. Please create a new data source.");
			AddString(ItemsSourceWizardStringId.LearnMoreText, "The detailed information and examples on using data access technologies with DevExpress data-aware controls can be found online on our ");
			AddString(ItemsSourceWizardStringId.CreateNew, "Create New...");
			AddString(ItemsSourceWizardStringId.Back, "Back");
			AddString(ItemsSourceWizardStringId.Next, "Next");
			AddString(ItemsSourceWizardStringId.Finish, "Finish");
			AddString(ItemsSourceWizardStringId.OK, "OK");
			AddString(ItemsSourceWizardStringId.Add, "Add");
			AddString(ItemsSourceWizardStringId.Delete, "Delete");
			AddString(ItemsSourceWizardStringId.Enable, "Enable");
			AddString(ItemsSourceWizardStringId.Rows, "Sample Row Count:");
			AddString(ItemsSourceWizardStringId.DistinctValue, "Distinct Values");
			AddString(ItemsSourceWizardStringId.CollectionViewType, "View Type:");
			AddString(ItemsSourceWizardStringId.GroupDesc, "Group Descriptions");
			AddString(ItemsSourceWizardStringId.SortDesc, "Sort Descriptions");
			AddString(ItemsSourceWizardStringId.TestConnection, "Test");
			AddString(ItemsSourceWizardStringId.PageSize, "Page Size:");
			AddString(ItemsSourceWizardStringId.HelpMeChooseLink, "documentation web server");
			AddString(ItemsSourceWizardStringId.SupportTextPart1, "If you have quistions regarding this installer or need assistance with the setup process,");
			AddString(ItemsSourceWizardStringId.SupportTextPart2, "feel free contact us by writing to");
			AddString(ItemsSourceWizardStringId.SupportLink, "support@devexpress.com");
			AddString(ItemsSourceWizardStringId.SynchronizedWithCurrentItem, "Synchronize with Current Item");
			AddString(ItemsSourceWizardStringId.AreSourceRowsThreadSafe, "Check Threading");
			AddString(ItemsSourceWizardStringId.ShowDesignData, "Show Sample Data at Design-Time");
			AddString(ItemsSourceWizardStringId.DistinctValue, "Show Distinct Values");
			AddString(ItemsSourceWizardStringId.FlattenHierarchy, "Flatten Hierarchy");
			AddString(ItemsSourceWizardStringId.CreateNewRiaSourceHelpText, "To create a new RIA data source, close this wizard and add a new Domain Service Class to your server web project. Once the Domain Service class is created, rebuild your project and reopen the 'Items Source Configuration Wizard'. The new RIA data source will be displayed within the 'Data Source'.");
		}
		#endregion
		public override XtraLocalizer<ItemsSourceWizardStringId> CreateResXLocalizer() {
			throw new NotImplementedException();
		}
		public static string GetString(ItemsSourceWizardStringId id) {
			return Active.GetLocalizedString(id);
		}
		public static XtraLocalizer<ItemsSourceWizardStringId> CreateDefaultLocalizer() {
			return new ItemsSourceWizardResXLocalizer();
		}
	}
	public class ItemsSourceWizardResXLocalizer : DXResXLocalizer<ItemsSourceWizardStringId> {
		public ItemsSourceWizardResXLocalizer()
			: base(new ItemsSourceWizardLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Core.Design.Wizards.LocalizationRes", typeof(ItemsSourceWizardLocalizer).Assembly);
		}
	}
}
