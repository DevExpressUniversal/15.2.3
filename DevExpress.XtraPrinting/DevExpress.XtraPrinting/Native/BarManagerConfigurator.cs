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
using System.ComponentModel;
using Microsoft.Win32;
using DevExpress.XtraBars;
namespace DevExpress.XtraPrinting.Native {
	public class BarManagerConfigurator {
		#region static
		public static void Configure(params BarManagerConfigurator[] configurators) {
			Configure(configurators, true);
		}
		public static void Configure(BarManagerConfigurator[] configurators, bool force) {
			if(configurators == null || configurators.Length == 0)
				throw new ArgumentException();
			BarManager manager = configurators[0].manager;
			manager.BeginInit();
			foreach(BarManagerConfigurator item in configurators) {
				if(force || item.UpdateNeeded)
					item.ConfigInternal();
			}
			manager.TransparentEditors = true;
			manager.EndInit();
			manager.ForceInitialize();
		}
		public static void SaveLayoutVersion(string key, string versionName, string versionValue) {
			try {
				RegistryKey regKey = Registry.CurrentUser.CreateSubKey(key);
				regKey.SetValue(versionName, versionValue);
			} catch { }
		}
		public static bool CheckLayoutVersion(string key, string versionName, string currentVersionValue) {
			try {
				RegistryKey regKey = Registry.CurrentUser.CreateSubKey(key);
				string previousVersionValue = regKey.GetValue(versionName) as string;
				return previousVersionValue == currentVersionValue;
			} catch {
				return false;
			}
		}
		public static void AddToContainer(IContainer container, Component component) {
			if(container != null)
				container.Add(component);
		}
		#endregion
		protected BarManager manager;
		public virtual bool UpdateNeeded {
			get { return false; }
		}
		protected virtual bool ShouldAddBarItemToContainer {
			get { return true; }
		} 
		public BarManagerConfigurator(BarManager manager) {
			this.manager = manager;
		}
		public void Configure() {
			Configure(this);
		}
		protected void AddComponentToContainer(Component component) {
			AddToContainer(manager.Container, component);
		}
		public virtual void ConfigInternal() {
		}
		protected virtual Bar CreateBar() {
			return new Bar();
		}
		#region AddBarMethods
		public virtual Bar AddBar(string barName, int dockCol, int dockRow, BarDockStyle barDockStyle, string text) {
			Bar bar = CreateBar();
			AddBar(bar, barName, dockCol, dockRow, barDockStyle, text);
			return bar;
		}
		public virtual void AddBar(Bar bar, string barName, int dockCol, int dockRow, BarDockStyle barDockStyle, string text) {
			bar.BarName = barName;
			bar.DockCol = dockCol;
			bar.DockRow = dockRow;
			bar.DockStyle = barDockStyle;
			bar.Text = text;
			manager.Bars.Add(bar);
		}
		protected virtual Bar AddMainMenuBar(string barName, int dockCol, int dockRow, BarDockStyle barDockStyle, string text) {
			Bar bar = AddBar(barName, dockCol, dockRow, barDockStyle, text);
			bar.OptionsBar.MultiLine = true;
			bar.OptionsBar.UseWholeRow = true;
			manager.MainMenu = bar;
			return bar;
		}
		protected virtual Bar AddStatusBar(string barName, int dockCol, int dockRow, BarDockStyle barDockStyle, string text) {
			Bar bar = AddBar(barName, dockCol, dockRow, barDockStyle, text);
			bar.OptionsBar.UseWholeRow = true;
			bar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
			bar.OptionsBar.AllowQuickCustomization = false;
			bar.OptionsBar.DrawDragBorder = false;
			manager.StatusBar = bar;
			return bar;
		}
		#endregion //AddBarMethods
		public void AddBarItem(BarLinksHolder barLinksHolder, BarItem barItem, string caption, string name, string hint, int imageIndex, bool beginGroup) {
			barItem.Caption = caption;
			barItem.Name = name;
			barItem.Hint = hint;
			AddBarItem(barLinksHolder, barItem, imageIndex, beginGroup);
		}
		public void AddBarItem(BarItem barItem, string caption, string name, string hint, int imageIndex) {
			barItem.Caption = caption;
			barItem.Name = name;
			barItem.Hint = hint;
			AddBarItemCore(barItem, imageIndex);
		}
		protected internal void AddBarItem(BarLinksHolder barLinksHolder, BarItem barItem, int imageIndex, bool beginGroup) {
			AddBarItemCore(barItem, imageIndex);
			AddLink(barLinksHolder, barItem, beginGroup);
		}
		void AddBarItemCore(BarItem barItem, int imageIndex) {
			barItem.Id = GetID();
			barItem.ImageIndex = imageIndex;
			manager.Items.Add(barItem);
			if(barItem is BarEditItem)
				manager.RepositoryItems.Add(((BarEditItem)barItem).Edit);
			if (ShouldAddBarItemToContainer)
				AddComponentToContainer(barItem);
		}
		int GetID() {
			return manager.MaxItemId++;
		}
		protected virtual BarSubItem AddBarSubItem(BarLinksHolder barLinksHolder, BarSubItem barItem, string caption, string name, string hint, int imageIndex, bool beginGroup) {
			AddBarItem(barLinksHolder, barItem, caption, name, hint, imageIndex, beginGroup);
			return barItem;
		}
		protected static void AddLink(BarLinksHolder barLinksHolder, BarItem barItem, bool beginGroup) {
			if(barLinksHolder != null)
				barLinksHolder.AddItem(barItem, new LinkPersistInfo(barItem, beginGroup));
		}
	}
}
