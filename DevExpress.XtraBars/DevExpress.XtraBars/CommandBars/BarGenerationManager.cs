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
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars.Commands.Internal;
namespace DevExpress.XtraBars.Commands.Design {
	#region BarGenerationManager<TControl, TCommandId> (abstract class)
	public abstract class BarGenerationManager<TControl, TCommandId> : BarGenerationManagerBase<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		protected BarGenerationManager(ControlCommandBarCreator creator, Component container, ControlCommandBarControllerBase<TControl, TCommandId> barController)
			: base(creator, container, barController) {
		}
		public override BarManager BarManager { get { return BarContainer as BarManager; } }
		Type SupportedBarType { get { return BarCreator.SupportedBarType; } }
		protected override BarCreationContextBase CreateBarCreationContext() {
			return new BarCreationContext();
		}
		protected override Component CreateBarItemGroup(ControlCommandBarCreator creator) {
			Bar bar = FindCommandBar();
			if (bar != null)
				return bar;
			return CreateBar(creator);
		}
		protected internal virtual Bar CreateBar(ControlCommandBarCreator creator) {
			Bar bar = creator.CreateBar();
			if (bar == null)
				return null;
			bar.DockStyle = BarDockStyle.Top;
			bar.DockRow = creator.DockRow;
			bar.DockCol = creator.DockColumn;
			if (InsertMode == BarInsertMode.Add)
				BarManager.Bars.Add(bar);
			else {
				BarManager.Bars.Insert(0, bar);
			}
			bar.ApplyDockRowCol();
			return bar;
		}
		protected override void ClearExistingItemsCore() {
			ClearBarManagerContent();
		}
		protected virtual Bar FindCommandBar() {
			Bars bars = BarManager.Bars;
			int count = bars.Count;
			for (int i = 0; i < count; i++)
				if (SupportedBarType.IsAssignableFrom(bars[i].GetType()))
					return bars[i];
			return null;
		}
		protected override void AddItemsToBarItemGroup(Component barItemGroup, List<BarItem> items) {
			Bar bar = barItemGroup as Bar;
			foreach (BarItem item in items) {
				AddItemLink(bar.ItemLinks, item);
				IBarSubItem subItem = item as IBarSubItem;
				if (subItem != null) {
					List<BarItem> subItems = subItem.GetSubItems();
					AddBarItems(BarManager.Items, subItems.ToArray());
				}
			}
		}
		protected override List<BarItem> GetBarContainerBarItems() {
			List<BarItem> result = new List<BarItem>();
			BarItems barItems = BarManager.Items;
			int count = barItems.Count;
			for (int i = 0; i < count; i++)
				result.Add(barItems[i]);
			return result;
		}
	}
	#endregion
}
