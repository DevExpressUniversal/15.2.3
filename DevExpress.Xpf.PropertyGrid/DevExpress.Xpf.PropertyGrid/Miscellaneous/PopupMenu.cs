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
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.PropertyGrid {
	public class PropertyGridViewPopupMenu : DevExpress.Xpf.Bars.PopupMenu {
		readonly Locker changeIsOpenLocker = new Locker();
		readonly Locker clickLocker = new Locker();
		public PropertyGridViewPopupMenu() : this(null) { }
		public PropertyGridViewPopupMenu(object context)
			: base(context) {
			HorizontalOffset = -1;
			VerticalOffset = -1;
			AllowMouseCapturing = true;
			UpdatePlacement();
			BarNameScope.SetIsScopeOwner(this, true);
		}
		public virtual void UpdatePlacement() {
			Placement = !SystemParameters.MenuDropAlignment ? PlacementMode.Left : PlacementMode.Right;
		}
		public void Show(RowDataBase rowData) {
			DataContext = rowData;
			IsOpen = true;			
		}
		void RaiseClickOnPlacementTargetAsync() {
			clickLocker.Lock();
			Dispatcher.BeginInvoke(new Action(RaiseClickOnPlacementTarget));
		}
		void RaiseClickOnPlacementTarget() {
			clickLocker.Unlock();
		}
		protected override object OnIsOpenCoerce(object value) {
			if (changeIsOpenLocker.IsLocked)
				return IsOpen;
			return base.OnIsOpenCoerce(value);
		}
		bool CheckPlacementTargetClicked(MouseEventArgs e) {
			if (PlacementTarget != null && e != null) {
				var point = e.GetPosition(PlacementTarget);
				if (point.X > 0 && point.Y > 0 && point.X < PlacementTarget.DesiredSize.Width && point.Y < PlacementTarget.DesiredSize.Height)
					return true;
			}
			return false;
		}
	}
	public class PropertyGridMenuHelper {
		PropertyGridViewPopupMenu menu;
		readonly PropertyGridView view;
		protected PropertyGridView View { get { return view; } }
		protected PropertyGridViewPopupMenu Menu { get { return menu ?? (menu = CreateMenu()); } }
		public bool MenuIsOpen { get { return menu.Return(x => x.IsOpen, () => false); } }
		protected virtual PropertyGridViewPopupMenu CreateMenu() {
			var result = new PropertyGridViewPopupMenu() { DataContext = null };			
			result.PlacementTarget = view;
			result.Opened += OnMenuOpened;
			result.Closed += OnMenuClosed;
			return result;
		}
		public PropertyGridMenuHelper(PropertyGridView view) {
			this.view = view;
		}
		protected virtual void OnMenuOpened(object sender, EventArgs e) {
			if (Menu.Tag is bool && (bool)Menu.Tag) {
				var popupMenuBarControl = Menu.PopupContent as PopupMenuBarControl;
				if (popupMenuBarControl != null)
					NavigationTree.SelectElement(popupMenuBarControl);
			}
		}
		protected virtual void OnMenuClosed(object sender, EventArgs e) {
			Menu.Items.Clear();
			Menu.UpdatePlacement();
			Menu.DataContext = null;
		}
		protected internal virtual bool CloseMenu(RowDataBase data) {
			if (Menu == null)
				return false;
			bool retValue = Menu.DataContext == data;
			if (Menu.IsOpen == true) {
				Menu.ClosePopup();
			}
			return retValue;
		}
		void ProcessMenuKeyDown(KeyEventArgs e) {			
		}
		internal void ProcessKeyDown(KeyEventArgs e) {
			if (e.Key == Key.Apps && !View.CellEditorOwner.CurrentCellEditor.If(x => x.IsEditorVisible).ReturnSuccess()) {
				if (Menu != null) {
					if (Menu.IsOpen)
						CloseMenu(View.SelectedItem as RowDataBase);
					else
						ShowCommandsMenu(View.SelectedItem as RowDataBase);
					e.Handled = true;
				}
			}
		}
		public bool ShowCollectionMenu(RowDataBase data, object relativeTo) {
			if (!data.IsCollectionRow)
				return false;
			var values = View.PropertyGrid.DataView.GetCollectionNewItemValues(data.Handle).With(x=>x.OfType<object>());
			if (values == null || values.Count()<=1)
				return false;
			Prepare(data, relativeTo);
			foreach (var element in values) {
				BarButtonItem buttonItem = new BarButtonItem();
				buttonItem.Content = Convert.ToString(element);
				buttonItem.Tag = element;
				buttonItem.ItemClick += (o, e) => {
					View.CollectionHelper.AddCollectionItem(data, e.Item.Tag);
				};
				Menu.Items.Add(buttonItem);
			}
			Menu.Show(data);
			return true;
		}
		public void ShowCommandsMenu(RowDataBase data) {
			Menu.Tag = true;
			ShowCommandsMenu(data, view.ItemContainerGenerator.ContainerFromItem(data) as RowControlBase);
		}
		public void ShowCommandsMenu(RowDataBase data, object relativeTo) {
			Prepare(data, relativeTo);
			data.ExecuteMenuCustomizations(Menu);
			View.PropertyGrid.ExecuteMenuCustomizations(Menu);
			data.Definition.ExecuteMenuCustomizations(Menu);
			Menu.Show(data);
		}
		protected void Prepare(RowDataBase data, object relativeTo) {
			Guard.ArgumentNotNull(relativeTo, "relativeTo");
			PrepareRowData(data);
			PrepareMenu(relativeTo);
		}
		protected void PrepareRowData(RowDataBase data) {
			View.SelectionStrategy.SelectViaHandle(data.Handle);
		}
		protected void PrepareMenu(object relativeTo) {
			Menu.Items.Clear();
			var uie = relativeTo as UIElement;
			var frec = relativeTo as FrameworkRenderElementContext;
			Point point = new Point();
			if (uie != null) {
				point = uie.TransformToAncestor(view).Transform(new Point(uie.RenderSize.Width, uie.RenderSize.Height));				
			} else  if (frec != null) {
				var inner = RenderTreeHelper.TransformToRoot(frec).Transform(new Point(frec.RenderSize.Width, frec.RenderSize.Height));
				point = frec.ElementHost.Parent.TransformToAncestor(view).Transform(inner);
			} else {
				throw new ArgumentException();
			}			
			Menu.HorizontalOffset = point.X;
			Menu.VerticalOffset = point.Y;
		}
	}
}
