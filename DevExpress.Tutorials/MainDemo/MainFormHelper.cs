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
using System.Text;
using DevExpress.DemoData;
using System.Runtime.InteropServices;
using DevExpress.XtraNavBar;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using System.Diagnostics;
using System.Linq;
using DevExpress.DemoData.Model;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.DXperience.Demos {
	public enum ImageSize { Small16, Large32 }
	public class MainFormRegisterDemoHelper {
		public static string GetTitle(SimpleModule module) {
			string title = module.DisplayName;
			if(module.IsUpdated)
				return title + string.Format(" ({0})", DevExpress.Tutorials.Properties.Resources.Updated);
			if(module.IsNew)
				return title + string.Format(" ({0})", DevExpress.Tutorials.Properties.Resources.New);
			return title;
		}
		static void AddNewAndUpdatedDemos(List<ModuleInfo> newAndUpdated) {
			newAndUpdated.Sort(delegate(ModuleInfo x, ModuleInfo y) { return x.Priority - y.Priority; });
			AddDemos(newAndUpdated);
		}
		static void AddHighlightedDemos(List<ModuleInfo> highlighted) {
			highlighted.Sort(delegate(ModuleInfo x, ModuleInfo y) { return x.Priority - y.Priority; });
			AddDemos(highlighted);
		}
		static void AddDemos(List<ModuleInfo> demos) {
			foreach(ModuleInfo item in demos) {
				ModulesInfo.Add(item);
			}
		}
		public static void RegisterDemos(string productID) {
			Product product = Repository.Platforms.SelectMany(p => p.Products).First(p => p.Name == productID);
			Demo demo = product.Demos.FirstOrDefault(x => x.Modules.Count > 0);
			List<ModuleInfo> newAndUpdated = new List<ModuleInfo>();
			List<ModuleInfo> highlighted = new List<ModuleInfo>();
			List<ModuleInfo> regular = new List<ModuleInfo>();
			ModuleInfo about = null;
			foreach(var module in demo.Modules.Cast<SimpleModule>()) {
				 ModuleInfo info;
				info = new ModuleInfo(GetTitle(module), module.Type, module.Description, module.Icon.Image, module.Group);
				if(module is ExampleModule) info.Uri = (module as ExampleModule).Uri;
				if(module.Group == "About")
					about = info;
				else
					regular.Add(info);
				if(module.IsNew || module.IsUpdated) {
					ModuleInfo newInfo = new ModuleInfo(info);
					newInfo.Group = DevExpress.Tutorials.Properties.Resources.NewUpdateGroup;
					newInfo.Priority = module.NewUpdatedPriority;
					newAndUpdated.Add(newInfo);
				}
				if(module.IsFeatured) {
					ModuleInfo highlightednewInfoModule = new ModuleInfo(info);
					highlightednewInfoModule.Group = DevExpress.Tutorials.Properties.Resources.HighlightedFeaturesGroup;
					highlightednewInfoModule.Priority = module.FeaturedPriority;
					highlighted.Add(highlightednewInfoModule);
				}
			}
			ModulesInfo.Add(about);
			AddNewAndUpdatedDemos(newAndUpdated);
			AddHighlightedDemos(highlighted);
			AddDemos(regular);
		}
	}
	public class MainFormHelper {
		[DllImport("kernel32.dll")]
		public static extern bool SetProcessWorkingSetSize(IntPtr handle,
			int minimumWorkingSetSize, int maximumWorkingSetSize);
		public static void SelectNavBarItem(NavBarControl nbControl, object startModule) {
			foreach(NavBarGroup group in nbControl.Groups) {
				foreach(NavBarItemLink item in group.ItemLinks) {
					ModuleInfo info = item.Item.Tag as ModuleInfo;
					if(info != null && (startModule.Equals(info.TypeName) || startModule.Equals(info.FullTypeName))) {
						nbControl.SelectedLink = item;
						nbControl.GetViewInfo().MakeLinkVisible(item);
						return;
					}
				}
			}
		}
		public static void SelectAccordionControlItem(AccordionControl nbControl, object startModule) {
			foreach(AccordionControlElement group in nbControl.Elements) {
				foreach(AccordionControlElement item in group.Elements) {
					ModuleInfo info = item.Tag as ModuleInfo;
					if(info != null && (startModule.Equals(info.TypeName) || startModule.Equals(info.FullTypeName))) {
						nbControl.SelectElement(item);
						item.Visible = true;
						if(!group.Expanded) group.Expanded = true;
						return;
					}
				}
			}
		}
		public static void SetCommandLineArgs(string[] args, out string startModule, out bool fullWindow) {
			startModule = string.Empty;
			fullWindow = false;
			foreach(string name in args) {
				if(name.IndexOf(DemoHelper.StartModuleParameter) == 0) startModule = DemoHelper.GetModuleName(name);
				if(name.Equals(DemoHelper.FullWindowModeParameter)) fullWindow = true;
			}
		}
		public static void SetFormClientSize(Rectangle workingArea, Form form, int width, int height) {
			Size tempSize = DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(width, height));
			width = tempSize.Width;
			height = tempSize.Height;
			form.ClientSize = new Size(Math.Min(workingArea.Width, width), Math.Min(workingArea.Height, height));
			form.Location = new Point(workingArea.X + (workingArea.Width - form.Width) / 2, workingArea.Y + (workingArea.Height - form.Height) / 2);
		}
		public static void SetBarButtonImage(BarItem item, string name) {
			item.LargeGlyph = GetImage(name, ImageSize.Large32);
			item.Glyph = GetImage(name, ImageSize.Small16);
		}
		public static Image GetImage(string name, ImageSize size) {
			if(string.IsNullOrEmpty(name)) return null;
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.Tutorials.Images.{0}_{1}.png", name, GetImageSizeString(size)),
				typeof(RibbonMainForm).Assembly);
		}
		static string GetImageSizeString(ImageSize size) {
			if(size == ImageSize.Small16) return "16x16";
			return "32x32";
		}
		public static void RegisterDefaultBonusSkin() {
			DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");
		}
		public static void RegisterRibbonDefaultBonusSkin() {
				DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2013");
		}
		public static string GetFileName(string extension, string filter) {
			using(SaveFileDialog dialog = new SaveFileDialog()) {
				dialog.Filter = filter;
				dialog.FileName = Application.ProductName;
				dialog.DefaultExt = extension;
				if(dialog.ShowDialog() == DialogResult.OK)
					return dialog.FileName;
			}
			return String.Empty;
		}
		public static void OpenExportedFile(string fileName) {
			if(XtraMessageBox.Show(DevExpress.Tutorials.Properties.Resources.OpenFileQuestion, DevExpress.Tutorials.Properties.Resources.ExportCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				Process process = new Process();
				try {
					process.StartInfo.FileName = fileName;
					process.Start();
				}
				catch {
				}
			}
		}
		public static void ShowExportErrorMessage() {
			XtraMessageBox.Show(DevExpress.Tutorials.Properties.Resources.ExportError, DevExpress.Tutorials.Properties.Resources.ExportCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		public static void ShowExportErrorMessage(Exception e) {
			XtraMessageBox.Show(e.Message, DevExpress.Tutorials.Properties.Resources.ExportCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	public class NavigationMenuHelper {
		public static void CreateNavigationMenu(BarSubItem menu, NavBarControl navBar, BarManager manager) {
			foreach(NavBarGroup group in navBar.Groups) {
				if(group.Caption != DevExpress.Tutorials.Properties.Resources.NewUpdateGroup &&
					group.Caption != DevExpress.Tutorials.Properties.Resources.HighlightedFeaturesGroup &&
					group.Caption != DevExpress.Tutorials.Properties.Resources.OutdatedStylesGroup && group.Visible)
					menu.ItemLinks.Add(CreateBarSubItem(manager, group));
			}
		}
		static BarItem CreateBarSubItem(BarManager manager, NavBarGroup group) {
			BarSubItem item = new BarSubItem(manager, group.Caption);
			item.Popup += new EventHandler(item_Popup);
			foreach(NavBarItemLink link in group.ItemLinks)
				item.ItemLinks.Add(CreateBarButtonItem(manager, link));
			return item;
		}
		static void item_Popup(object sender, EventArgs e) {
			BarSubItem menu = sender as BarSubItem;
			foreach(BarItemLink link in menu.ItemLinks)
				((BarButtonItem)link.Item).Down =
					ModulesInfo.Instance.CurrentModuleBase == GetModuleInfoBarItemLink(link);
		}
		static ModuleInfo GetModuleInfoBarItemLink(BarItemLink link) {
			NavBarItemLink nLink = link.Item.Tag as NavBarItemLink;
			if(nLink == null) return null;
			return nLink.Item.Tag as ModuleInfo;
		}
		static BarButtonItem CreateBarButtonItem(BarManager manager, NavBarItemLink link) {
			BarButtonItem item = new BarButtonItem(manager, link.Item.Caption);
			item.Glyph = link.Item.SmallImage;
			item.ItemClick += new ItemClickEventHandler(item_ItemClick);
			item.Tag = link;
			item.ButtonStyle = BarButtonStyle.Check;
			return item;
		}
		static void SetLinkVisible(NavBarItemLink link) {
			if(link.Visible) return;
			link.Visible = true;
			link.Group.Visible = true;
		}
		static void item_ItemClick(object sender, ItemClickEventArgs e) {
			NavBarItemLink link = (NavBarItemLink)e.Item.Tag;
			SetLinkVisible(link);
			ShowModule(link.NavBar, link);
		}
		public static int GetLinkCount(NavBarControl navBar) {
			int ret = 0;
			foreach(NavBarGroup group in navBar.Groups) {
				if(group.Visible) {
					foreach(NavBarItemLink link in group.ItemLinks) {
						if(link.Visible) ret++;
					}
				}
			}
			return ret;
		}
		static NavBarItemLink GetNext(NavBarControl navBar, bool forceSearch) { return GetNext(navBar, forceSearch, false); }
		static NavBarItemLink GetNext(NavBarControl navBar, bool forceSearch, bool demo) {
			bool allowSearch = forceSearch;
			NavBarItemLink ret = null;
			foreach(NavBarGroup group in navBar.Groups) {
				if(group.Caption == DevExpress.Tutorials.Properties.Resources.NewUpdateGroup && demo) continue;
				if(group.Visible) {
					foreach(NavBarItemLink link in group.ItemLinks) {
						if(allowSearch && link.Visible) {
							ret = link;
							return ret;
						}
						if(link.Visible && link == navBar.SelectedLink)
							allowSearch = true;
					}
				}
			}
			return null;
		}
		static NavBarItemLink GetPrev(NavBarControl navBar, bool forceSearch) {
			bool allowSearch = forceSearch;
			NavBarItemLink ret = null;
			for(int i = navBar.Groups.Count - 1; i >= 0; i--) {
				if(navBar.Groups[i].Visible) {
					for(int j = navBar.Groups[i].ItemLinks.Count - 1; j >= 0; j--) {
						NavBarItemLink temp = navBar.Groups[i].ItemLinks[j];
						if(allowSearch && temp.Visible) {
							ret = temp;
							return ret;
						}
						if(temp.Visible && temp == navBar.SelectedLink)
							allowSearch = true;
					}
				}
			}
			return null;
		}
		public static void ShowNext(NavBarControl navBar) {
			NavBarItemLink link = GetNext(navBar, false);
			if(link == null)
				link = GetNext(navBar, true);
			ShowModule(navBar, link);
		}
		public static void ShowPrev(NavBarControl navBar) {
			NavBarItemLink link = GetPrev(navBar, false);
			if(link == null)
				link = GetPrev(navBar, true);
			ShowModule(navBar, link);
		}
		public static void StartDemo(NavBarControl navBar) {
			NavBarItemLink link = null;
			if(navBar.SelectedLink != null)
				link = GetNext(navBar, false);
			else
				link = GetNext(navBar, true, true);
			if(link == null) {
				RibbonMainForm form = navBar.FindForm() as RibbonMainForm;
				if(form != null) {
					form.ClearNavBarFilter();
					link = GetNext(navBar, true, true);
				}
			}
			ShowModule(navBar, link);
		}
		static void ShowModule(NavBarControl navBar, NavBarItemLink link) {
			if(link == null) return;
			navBar.SelectedLink = link;
			navBar.GetViewInfo().MakeLinkVisible(link);
		}
	}
	public class AccordionNavigationMenuHelper {
		public static void CreateNavigationMenu(BarSubItem menu, AccordionControl accordionControl, BarManager manager) {
			foreach(AccordionControlElement group in accordionControl.Elements) {
				if(group.Text != DevExpress.Tutorials.Properties.Resources.NewUpdateGroup &&
					group.Text != DevExpress.Tutorials.Properties.Resources.HighlightedFeaturesGroup &&
					group.Text != DevExpress.Tutorials.Properties.Resources.OutdatedStylesGroup && group.Visible)
					menu.ItemLinks.Add(CreateBarSubItem(manager, group));
			}
		}
		static BarItem CreateBarSubItem(BarManager manager, AccordionControlElement group) {
			BarSubItem item = new BarSubItem(manager, group.Text);
			item.Popup += new EventHandler(item_Popup);
			foreach(AccordionControlElement node in group.Elements)
				item.ItemLinks.Add(CreateBarButtonItem(manager, node));
			return item;
		}
		static void item_Popup(object sender, EventArgs e) {
			BarSubItem menu = sender as BarSubItem;
			foreach(BarItemLink link in menu.ItemLinks)
				((BarButtonItem)link.Item).Down =
					ModulesInfo.Instance.CurrentModuleBase == GetModuleInfoBarItemLink(link);
		}
		static ModuleInfo GetModuleInfoBarItemLink(BarItemLink link) {
			AccordionControlElement node = link.Item.Tag as AccordionControlElement;
			if(node == null) return null;
			return node.Tag as ModuleInfo;
		}
		static BarButtonItem CreateBarButtonItem(BarManager manager, AccordionControlElement node) {
			BarButtonItem item = new BarButtonItem(manager, node.Text);
			item.ItemClick += new ItemClickEventHandler(item_ItemClick);
			item.Tag = node;
			item.ButtonStyle = BarButtonStyle.Check;
			return item;
		}
		static void item_ItemClick(object sender, ItemClickEventArgs e) {
			AccordionControlElement node = (AccordionControlElement)e.Item.Tag;
			ShowModule(node.AccordionControl, node);
		}
		public static int GetNodeCount(AccordionControl accordionControl) {
			int ret = 0;
			foreach(AccordionControlElement group in accordionControl.Elements) {
				if(group.Visible) {
					foreach(AccordionControlElement node in group.Elements) {
						if(node.Visible) ret++;
					}
				}
			}
			return ret;
		}
		static AccordionControlElement GetNext(AccordionControl accordionControl, bool forceSearch) { return GetNext(accordionControl, forceSearch, false); }
		static AccordionControlElement GetNext(AccordionControl accordionControl, bool forceSearch, bool demo) {
			bool allowSearch = forceSearch;
			AccordionControlElement ret = null;
			foreach(AccordionControlElement group in accordionControl.Elements) {
				if(group.Text == DevExpress.Tutorials.Properties.Resources.NewUpdateGroup && demo) continue;
				if(group.Visible) {
					foreach(AccordionControlElement node in group.Elements) {
						if(allowSearch && node.Visible) {
							ret = node;
							return ret;
						}
						if(node.Visible && node == accordionControl.SelectedElement)
							allowSearch = true;
					}
				}
			}
			return null;
		}
		static AccordionControlElement GetPrev(AccordionControl accordionControl, bool forceSearch) {
			bool allowSearch = forceSearch;
			AccordionControlElement ret = null;
			for(int i = accordionControl.Elements.Count - 1; i >= 0; i--) {
				if(accordionControl.Elements[i].Visible) {
					for(int j = accordionControl.Elements[i].Elements.Count - 1; j >= 0; j--) {
						AccordionControlElement temp = accordionControl.Elements[i].Elements[j];
						if(allowSearch && temp.Visible) {
							ret = temp;
							return ret;
						}
						if(temp.Visible && temp == accordionControl.SelectedElement)
							allowSearch = true;
					}
				}
			}
			return null;
		}
		public static void ShowNext(AccordionControl accordionControl) {
			AccordionControlElement node = GetNext(accordionControl, false);
			if(node == null)
				node = GetNext(accordionControl, true);
			ShowModule(accordionControl, node);
		}
		public static void ShowPrev(AccordionControl accordionControl) {
			AccordionControlElement node = GetPrev(accordionControl, false);
			if(node == null)
				node = GetPrev(accordionControl, true);
			ShowModule(accordionControl, node);
		}
		public static void StartDemo(AccordionControl accordionControl) {
			AccordionControlElement node = null;
			if(accordionControl.SelectedElement != null)
				node = GetNext(accordionControl, false);
			else
				node = GetNext(accordionControl, true, true);
			if(node == null) {
				RibbonMainForm form = accordionControl.FindForm() as RibbonMainForm;
				if(form != null) {
					node = GetNext(accordionControl, true, true);
				}
			}
			ShowModule(accordionControl, node);
		}
		static void ShowModule(AccordionControl accordionControl, AccordionControlElement node) {
			if(node == null) return;
			accordionControl.SelectElement(node);
			node.Visible = true;
			if(!node.OwnerElement.Expanded) node.OwnerElement.Expanded = true;
		}
	}
	public class NavBarFilter : IDisposable {
		Dictionary<NavBarItemLink, bool> initialLinksVisibility;
		Dictionary<NavBarGroup, bool> initialGroupsVisibility;
		NavBarItemLink initialSelectedLink = null;
		NavBarControl navBar;
		public NavBarFilter(NavBarControl navBar) {
			this.navBar = navBar;
		}
		protected NavBarControl NavBar { get { return navBar; } }
		public virtual void Dispose() {
			if(this.initialGroupsVisibility != null) this.initialGroupsVisibility.Clear();
			if(this.initialLinksVisibility != null) this.initialLinksVisibility.Clear();
			this.initialSelectedLink = null;
		}
		public void Reset() {
			FilterNavBar("");
			this.initialGroupsVisibility = null;
			this.initialLinksVisibility = null;
			this.initialSelectedLink = null;
		}
		void UpdateGroupsVisibility() {
			this.initialGroupsVisibility = new Dictionary<NavBarGroup, bool>();
			foreach(NavBarGroup group in NavBar.Groups) {
				if(group.Visible) this.initialGroupsVisibility[group] = true;
			}
		}
		void UpdateLinksVisibility() {
			this.initialLinksVisibility = new Dictionary<NavBarItemLink, bool>();
			this.initialSelectedLink = NavBar.SelectedLink;
			foreach(NavBarGroup group in NavBar.Groups) {
				foreach(NavBarItemLink link in group.ItemLinks) {
					if(link.Visible) initialLinksVisibility[link] = true;
				}
			}
		}
		public void FilterNavBar(string text) {
			if(initialLinksVisibility == null) UpdateLinksVisibility();
			if(initialGroupsVisibility == null) UpdateGroupsVisibility();
			if(NavBar.SelectedLink != null) this.initialSelectedLink = NavBar.SelectedLink;
			text = text.ToLowerInvariant();
			NavBar.BeginUpdate();
			try {
				foreach(KeyValuePair<NavBarItemLink, bool> pair in initialLinksVisibility) {
					string linkText = pair.Key.Caption.ToLowerInvariant();
					pair.Key.Visible = (string.IsNullOrEmpty(text) || linkText.Contains(text));
				}
				foreach(NavBarGroup group in NavBar.Groups) {
					if(group.VisibleItemLinks.Count == 0)
						group.Visible = false;
					else
						group.Visible = initialGroupsVisibility.ContainsKey(group);
				}
				CheckSelectedLink();
			}
			finally {
				NavBar.EndUpdate();
			}
		}
		void CheckSelectedLink() {
			if(initialSelectedLink != NavBar.SelectedLink) {
				if(NavBar.SelectedLink != null && NavBar.SelectedLink.Group != null) {
					NavBar.SelectedLink.Group.SelectedLinkIndex = -1;
				}
				if(NavBar.SelectedLink == null) {
					if(this.initialSelectedLink != null && this.initialSelectedLink.Visible && this.initialSelectedLink.Group.Visible) {
						NavBar.SelectedLink = this.initialSelectedLink;
					}
				}
			}
		}
	}
	public class RibbonMenuManager {
		RibbonMainForm parentForm = null;
		PrintOptions printOptions = new PrintOptions();
		public RibbonMenuManager(RibbonMainForm parentForm) {
			this.parentForm = parentForm;
		}
		public BarManager Manager { get { return parentForm.Ribbon.Manager; } }
		public void ShowReservGroup1(bool show) {
			parentForm.ReservGroup1.Visible = show;
		}
		public void ShowReservGroup2(bool show) {
			parentForm.ReservGroup2.Visible = show;
		}
		public void AllowExport(object obj) {
			parentForm.PrintExportGroup.Visible = obj != null;
		}
		public PrintOptions PrintOptions { get { return printOptions; } }
	}
	public class PrintOptions {
		bool showRibbonPreviewForm = true;
		public bool ShowRibbonPreviewForm {
			get { return showRibbonPreviewForm; }
			set { showRibbonPreviewForm = value; }
		}
	}
}
