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
using DevExpress.Design.UI;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class HierarchyItemViewModel : WpfBindableBase {
		object item;
		string name;
		List<HierarchyItemViewModel> children;
		HierarchyItemViewModel parent;
		bool isSelected;
		public HierarchyItemViewModel(object item) {
			this.item = item;
			children = new List<HierarchyItemViewModel>();
		}
		public void AddChild(HierarchyItemViewModel child) {
			if(child != null && !children.Contains(child)) {
				children.Add(child);
				child.parent = this;
			}
		}
		public IEnumerable<HierarchyItemViewModel> Children { get { return children; } }
		public string Name {
			get {
				if(!string.IsNullOrEmpty(this.name))
					return this.name;
				if(item != null)
					return item.ToString();
				return string.Empty;
			}
			set {
				this.name = value;
			}
		}
		public object Item {
			get {
				return this.item;
			}
		}
		public HierarchyItemViewModel Parent {
			get {
				return parent;
			}
		}
		public bool IsSelected {
			get {
				return this.isSelected;
			}
			set {
				SetProperty<bool>(ref isSelected, value, "IsSelected");
			}
		}
	}
}
