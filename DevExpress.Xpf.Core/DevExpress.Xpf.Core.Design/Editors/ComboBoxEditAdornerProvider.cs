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

extern alias Platform;
using DevExpress.Design.UI;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using Microsoft.Windows.Design.Model;
using System;
using System.Linq;
using System.Windows.Input;
using Platform::DevExpress.Xpf.Editors;
#if !SL
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Core.Design {
	public static class HelpMeChooseLinkHelper {
		public static string Link {
			get {
#if !SL
				return "http://documentation.devexpress.com/#WPF/CustomDocument7352";
#else
				return "http://documentation.devexpress.com/#Silverlight/CustomDocument5182";
#endif
			}
		}
	}
	public class SelectorEditAdornerProvider : EditorAdornerProviderBase {
		protected override ICommand CreateCommand() {
			return (ICommand)new SelectorEditShowItemsSourceWizardCommand();
		}
	}
	class SelectorEditShowItemsSourceWizardCommand : WpfDelegateCommand<ModelItem> {
		class WizardInfo {
			public ModelItem ModelItem { get; set; }
			public ItemsSourceWizardViewModel Model { get; set; }
		}
		public SelectorEditShowItemsSourceWizardCommand() : base(ShowWizard, false) { }
		static void ShowWizard(ModelItem editor) {
			var wizard = new ItemsSourceWizardWindow();
			var provider = new SelectorEditDataAccessTechnologyCollectionProvider();
			var viewModel = new ViewModelGenerator(provider).CreateViewModel(HelpMeChooseLinkHelper.Link);
			wizard.DataContext = viewModel;
			wizard.Tag = new WizardInfo() {
				Model = viewModel,
				ModelItem = editor,
			};
			wizard.Closed += wizard_Closed;
			wizard.Show();
		}
		static void wizard_Closed(object sender, EventArgs e) {
			var wizard = (ItemsSourceWizardWindow)sender;
			var wizardInfo = (WizardInfo)wizard.Tag;
			if(!wizardInfo.Model.IsApply)
				return;
			DataSourceGeneratorContainer generatorContainer = (DataSourceGeneratorContainer)wizardInfo.Model.DataSourceConfigurationViewModel.ConfiguratedDataSourceType;
			generatorContainer.Generator.Generate(generatorContainer.Properties.Select(d => d.DataContext), wizardInfo.ModelItem);
		}
	}
}
