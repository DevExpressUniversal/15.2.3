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

using DevExpress.Design.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class MessageLogWindowViewModel : ViewModelBase {
		List<ILogItem> items;
		IServiceContainer serviceContainer;
		public MessageLogWindowViewModel(IServiceContainer serviceContainer)
			: base(null) {
			this.serviceContainer = serviceContainer;
			items = new List<ILogItem>(LogServices.GetItems());
			ShowNext = new WpfDelegateCommand(() => SelectedIndex++, () => ShowNextItemEnabled);
			ShowPrevious = new WpfDelegateCommand(() => SelectedIndex--, () => ShowPreeviousEnabled);
			SelectedIndex++;
		}
		protected override IServiceContainer CreateServiceContainer() {
			return serviceContainer;
		}
		ILogServices LogServices { get { return this.ServiceContainer.Resolve<ILogServices>(); } }
		void UpdateDocument() {
			FlowDocument document = new FlowDocument();			
			if (SelectedItem != null)
				SelectedItem.OutText(document.Blocks);
			Document = document;
		}
		FlowDocument document;
		public FlowDocument Document {
			get {
				return document;
			}
			set {
				SetProperty<FlowDocument>(ref document, value, "Document");
			}
		}
		private ILogItem selectedItem;
		public ILogItem SelectedItem {
			get { return selectedItem; }
			set {
				if(SetProperty<ILogItem>(ref selectedItem, value, "SelectedItem", () => UpdateDocument())) {
					this.RaisePropertyChanged("ShowPreeviousEnabled");
					this.RaisePropertyChanged("ShowNextItemEnabled");
				}
			}
		}
		int selectedIndex = -1;
		public int SelectedIndex {
			get { return selectedIndex; }
			set {
				if (value < 0)
					value = -1;
				if (value >= items.Count)
					value = items.Count - 1;
				SetProperty<int>(ref selectedIndex, value, "SelectedIndex", () => this.UpdateSelectedItem());
			}
		}
		void UpdateSelectedItem() {
			SelectedItem = SelectedIndex < 0 || !items.Any() ? null : items.ElementAt(SelectedIndex);
		}
		public bool ShowPreeviousEnabled {
			get {
				return selectedIndex > 0 && items.Count > 0;
			}
		}
		public bool ShowNextItemEnabled {
			get {
				int count = items.Count;
				return count > 0 && selectedIndex+1 < count;
			}
		}
		public ICommand ShowNext { get; set; }
		public ICommand ShowPrevious { get; set; }
	}
}
