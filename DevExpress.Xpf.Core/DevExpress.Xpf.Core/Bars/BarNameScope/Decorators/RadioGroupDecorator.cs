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

using DevExpress.Mvvm.Native;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System;
namespace DevExpress.Xpf.Bars.Native {
	public interface IBarCheckItem {
		bool AllowUncheckInGroup { get; set; }
		int GroupIndex { get; set; }
		bool? IsChecked { get; set; }
		bool IsThreeState { get; set; }
	}
	public interface IBarCheckItemLink {
		void UpdateCheckItemProperties();
	}
	public interface IBarCheckItemLinkControl {
		void OnSourceIsCheckedChanged();
	}
	class RadioGroupService : IRadioGroupService {
		readonly RadioGroupStrategy strategy;
		internal RadioGroupService() { }
		internal RadioGroupService(RadioGroupStrategy strategy) { this.strategy = strategy; }
		bool IRadioGroupService.CanUncheck(IBarCheckItem element) {
			if (strategy == null) return false;
			return strategy.CanUncheck(element);
		}
		void IRadioGroupService.OnChecked(IBarCheckItem element) {
			if (strategy == null) return;
			strategy.OnChecked(element);
		}
	}
	public class RadioGroupStrategy : IBarNameScopeDecorator {
		ElementRegistrator registrator;
		HashSet<int> uncheckingGroups;
		public RadioGroupStrategy() {
			this.uncheckingGroups = new HashSet<int>();
		}
		public bool CanUncheck(IBarCheckItem element) {
			return element.GroupIndex == -1 || element.AllowUncheckInGroup || uncheckingGroups.Contains(element.GroupIndex);
		}
		public void OnChecked(IBarCheckItem element) {
			if (element.GroupIndex == -1)
				return;
			uncheckingGroups.Add(element.GroupIndex);
			foreach (var item in EnumerateGroup(element.GroupIndex)) {
				if (item == element)
					continue;
				item.IsChecked = false;
			}
			uncheckingGroups.Remove(element.GroupIndex);
		}
		protected IEnumerable<IBarCheckItem> EnumerateGroup(int index) { return registrator == null ? Enumerable.Empty<IBarCheckItem>() : registrator.Values.OfType<IBarCheckItem>().Where(x => x.GroupIndex == index); }
		protected virtual void OnRegistratorChanged(ElementRegistrator sender, ElementRegistratorChangedArgs e) {
			var checkItem = e.Element as IBarCheckItem;
			if (checkItem == null)
				return;
			if (e.ChangeType == ElementRegistratorChangeType.ElementAdded)
				OnElementAdded(checkItem);
			if (e.ChangeType == ElementRegistratorChangeType.ElementRemoved)
				OnElementRemoved(checkItem);
		}
		protected virtual void OnElementAdded(IBarCheckItem checkItem) {
			if (checkItem == null)
				return;
			if (checkItem.IsChecked == true)
				OnChecked(checkItem);
		}
		protected virtual void OnElementRemoved(IBarCheckItem checkItem) {
			if (checkItem == null || checkItem.AllowUncheckInGroup || checkItem.IsChecked != true)
				return;
		}
		void IBarNameScopeDecorator.Attach(BarNameScope scope) {
			registrator = scope[typeof(IFrameworkInputElement)];
			if (registrator != null)
				registrator.Changed += OnRegistratorChanged;
		}		
		void IBarNameScopeDecorator.Detach() {
			if (registrator != null)
				registrator.Changed -= OnRegistratorChanged;
			registrator = null;			
		}
	}
}
