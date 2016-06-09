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
using System.Collections.ObjectModel;
using DevExpress.DemoData.Helpers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.DemoBase.Helpers;
using System.Windows.Input;
namespace DevExpress.Xpf.DemoBase.Internal {
	sealed class DemoBaseControlModuleSelector : DemoBaseControlPart {
		class GroupedLink : IGroupedLink {
			public string Title { get; set; }
			public string Description { get; set; }
			public ICommand SelectCommand { get; set; }
			public bool IsNew { get; set; }
			public bool IsUpdated { get; set; }
			public bool IsHighlighted { get; set; }
		}
		class GroupedLinks : IGroupedLinks {
			public string Header { get; set; }
			public IEnumerable<IGroupedLink> Links { get; set; }
			public ICommand ShowAllCommand { get { return null; } }
		}
		IGroupedLinksControlService GroupedLinksControlService {
			get { return GetService<IGroupedLinksControlService>(); }
		}
		public ICommand LinkSelectedCommand { get; private set; }
		public override void OnNavigatedTo(CompletePageState prevState, CompletePageState newState) {
			GroupedLinksControlService.Enable();
		}
		public DemoBaseControlModuleSelector(ProductDescription product, DemoBaseControl demoBaseControl)
			: base(demoBaseControl) {
			LinkSelectedCommand = new DelegateCommand<IGroupedLink>(link => {
				GroupedLinksControlService.Disable();
				((GroupedLink)link).SelectCommand.Do(c => c.Execute(null));
			});
			Product = product;
			Initialized();
		}
		public ProductDescription Product {
			get { return GetProperty(() => Product); }
			set { SetProperty(() => Product, value); }
		}
		public IEnumerable<IGroupedLinks> Modules {
			get { return GetProperty(() => Modules); }
			set { SetProperty(() => Modules, value); }
		}
		string filterText;
		public string FilterText {
			get { return filterText; }
			set { SetProperty(ref filterText, value, () => filterText); }
		}
		public void Filter(string filterText) {
			UpdateModules();
			FilterText = filterText;
		}
		void UpdateModules() {
			if(Product.Modules == null) {
				Modules = null;
				return;
			}
			if(Modules != null)
				return;
			Modules = Product.Modules
				.GroupBy(m => m.GroupTitle.Text)
				.Where(g => g.Any())
				.Select(g => new GroupedLinks {
					Header = g.Key,
					Links = g.Select(md => new GroupedLink {
						Title = md.Title.Text,
						Description = md.ShortDescription.Text,
						SelectCommand = md.SelectCommand,
						IsNew = md.IsNew,
						IsUpdated = md.IsUpdated,
						IsHighlighted = md.IsHighlighted
					}).ToList()
				}).ToList();
		}
	}
}
