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
using System.Collections;
using System.Windows;
namespace DevExpress.Xpf.Docking.Base {
	public class LogicalElementsEnumerator : IEnumerator {
		readonly IEnumerator LogicalEnumerator;
		public LogicalElementsEnumerator(DockLayoutManager manager) {
			LogicalEnumerator = new object[] { 
				manager.DockHintsContainer,
				manager.InternalElementsContainer
			}.GetEnumerator();
		}
		#region IEnumerator Members
		object IEnumerator.Current {
			get { return LogicalEnumerator.Current; }
		}
		bool IEnumerator.MoveNext() {
			return LogicalEnumerator.MoveNext();
		}
		void IEnumerator.Reset() {
			LogicalEnumerator.Reset();
		}
		#endregion
	}
	public class CustomizationEnumerator : IEnumerator {
		readonly IEnumerator CustomizationControls;
		public CustomizationEnumerator(DockLayoutManager manager) {
			UIElement[] controls = new UIElement[0];
			controls = manager.CustomizationController.GetChildren();
			CustomizationControls = controls.GetEnumerator();
		}
		#region IEnumerator Members
		object IEnumerator.Current {
			get { return CustomizationControls.Current; }
		}
		bool IEnumerator.MoveNext() {
			return CustomizationControls.MoveNext();
		}
		void IEnumerator.Reset() {
			CustomizationControls.Reset();
		}
		#endregion
	}
	public class DockLayoutManagerEnumerator : IEnumerator {
		readonly IEnumerator PanelsEnumerator;
		public DockLayoutManagerEnumerator(DockLayoutManager manager) {
			BaseLayoutItem[] items = manager.GetItems();
			items = Array.FindAll(items, IsElementWithControl);
			PanelsEnumerator = items.GetEnumerator();
		}
		bool IsElementWithControl(BaseLayoutItem item) {
			LayoutPanel panel = item as LayoutPanel;
			if(panel != null)
				return panel.Control != null;
			LayoutControlItem controlItem = item as LayoutControlItem;
			if(controlItem != null)
				return controlItem.Control != null;
			return false;
		}
		#region IEnumerator Members
		object IEnumerator.Current {
			get {
				return (PanelsEnumerator.Current is LayoutPanel) ?
					((LayoutPanel)PanelsEnumerator.Current).Control :
					((LayoutControlItem)PanelsEnumerator.Current).Control;
			}
		}
		bool IEnumerator.MoveNext() {
			return PanelsEnumerator.MoveNext();
		}
		void IEnumerator.Reset() {
			PanelsEnumerator.Reset();
		}
		#endregion
	}
	public class MergedEnumerator : IEnumerator {
		readonly IEnumerator[] Enumerators;
		public MergedEnumerator(IEnumerator[] enumerators) {
			Enumerators = enumerators;
		}
		#region IEnumerator Members
		IEnumerator currentEnumerator;
		object IEnumerator.Current {
			get {
				if(currentEnumerator != null) return currentEnumerator.Current;
				throw new NotSupportedException();
			}
		}
		int index = -1;
		bool IEnumerator.MoveNext() {
			while(EnsureCurrentEnumerator()) {
				if(currentEnumerator.MoveNext()) return true;
				currentEnumerator = null;
			}
			return false;
		}
		bool EnsureCurrentEnumerator() {
			if(currentEnumerator == null) {
				if(++index < Enumerators.Length) {
					currentEnumerator = Enumerators[index];
				}
			}
			return currentEnumerator != null;
		}
		void IEnumerator.Reset() {
			currentEnumerator = null;
			index = -1;
		}
		#endregion
	}
}
