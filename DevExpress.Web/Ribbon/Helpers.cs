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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public interface IRibbonInternalItem {
	}
	public static class RibbonHelper {
		public static readonly string[] ImagesPostfixes = new string[] { "_LI", "_RI", "_PI", "_UI", "_DI" };
		public static readonly string OneLineModeGroupExpandButtonImagePostfix = "I";
		public static readonly string OneLineModeGroupExpandButtonPopOutImagePostfix = "PI";
		public static string GetGroupExpandButtonID(RibbonGroup group) {
			return GetGroupID(group) + "_EBTN";
		}
		public static string GetOneLineModeGroupExpandButtonID(RibbonGroup group) {
			return GetGroupID(group) + "_OEBTN";
		}
		public static string GetOneLineModeGroupExpandButtonImageID(RibbonGroup group) {
			return GetOneLineModeGroupExpandButtonID(group) + OneLineModeGroupExpandButtonImagePostfix;
		}
		public static string GetOneLineModeGroupExpandButtonPopOutImageID(RibbonGroup group) {
			return GetOneLineModeGroupExpandButtonID(group) + OneLineModeGroupExpandButtonPopOutImagePostfix;
		}
		public static string GetGroupDialogBoxLauncherID(RibbonGroup group) {
			return GetGroupID(group) + "_DBL";
		}
		public static string GetItemID(RibbonItemBase item) {
			return GetItemID(item.Group, item);
		}
		public static string GetItemID(RibbonGroup group, RibbonItemBase item) {
			var id = "";
			if(item is RibbonDropDownToggleButtonItem) {
				id = ((RibbonDropDownToggleButtonItem)item).GetIndexPath();
			} else {
				id = item.Index.ToString();
			}
			return GetGroupID(group) + string.Format("I{0}", id);
		}
		public static string GetItemImageID(RibbonItemBase item, RibbonItemSize size) {
			return GetItemID(item) + (size == RibbonItemSize.Large ? ImagesPostfixes[0] : ImagesPostfixes[1]);
		}
		public static string GetItemPopOutImageID(RibbonItemBase item) {
			return GetItemID(item) + ImagesPostfixes[2];
		}
		public static string GetItemTemplateContainerID(RibbonTemplateItem item) {
			return GetItemID(item) + "_ITC";
		}
		public static string GetRibbonTabID(RibbonTab tab) {
			return string.Format("T{0}", tab.GetCommonIndex());
		}
		public static string GetDropDownMenuID(RibbonDropDownButtonItem item) {
			return GetItemID(item.Group, item) + "_PM";
		}
		public static string GetDropDownPopupID(RibbonItemBase item) {
			return GetItemID(item.Group, item) + "_IPC";
		}
		public static string GetComboBoxID(RibbonComboBoxItem item) {
			return GetItemID(item) + "_CMB";
		}
		public static string GetDateEditID(RibbonDateEditItem item) {
			return GetItemID(item) + "_DE";
		}
		public static string GetSpinEditID(RibbonSpinEditItem item) {
			return GetItemID(item) + "_SE";
		}
		public static string GetTextBoxID(RibbonTextBoxItem item) {
			return GetItemID(item) + "_TB";
		}
		public static string GetCheckBoxID(RibbonCheckBoxItem item) {
			return GetItemID(item) + "_CB";
		}
		public static string GetColorIndicatorID(RibbonColorButtonItemBase item) {
			return GetItemID(item) + "_CI";
		}
		public static string GetColorTableID(RibbonColorButtonItemBase item) {
			return GetItemID(item) + "_CT";
		}
		public static string GetMinimizeButtonID(ASPxRibbon ribbon) {
			return string.Format("{0}_{1}_{2}",
				RibbonControl.TabControlID,
				ribbon.RibbonControl.TabControl.GetTabsSpaceTemplateID(DevExpress.Web.TabSpaceTemplatePosition.Before),
				RibbonMinimizeButtonTemplate.ButtonID
			);
		}
		public static string GetPopupGalleryID(RibbonItemBase item) {
			return GetItemID(item.Group, item) + "_PG";
		}
		public static string GetGalleryDropDownItemID(RibbonGalleryItem galleryItem) {
			return string.Format("{0}i{1}", galleryItem.Group.Index, galleryItem.Index);
		}
		public static string GetGalleryBarItemID(RibbonGalleryItem galleryItem) {
			return GetItemID(galleryItem.Group.Owner) + string.Format("_{0}i{1}", galleryItem.Group.Index, galleryItem.Index);
		}
		public static string GetGalleryUpButtonID(RibbonGalleryBarItem galleryBar) {
			return GetItemID(galleryBar) + "_UB";
		}
		public static string GetGalleryUpButtonImageID(RibbonGalleryBarItem galleryBar) {
			return GetGalleryUpButtonID(galleryBar) + ImagesPostfixes[3];
		}
		public static string GetGalleryDownButtonID(RibbonGalleryBarItem galleryBar) {
			return GetItemID(galleryBar) + "_DB";
		}
		public static string GetGalleryDownButtonImageID(RibbonGalleryBarItem galleryBar) {
			return GetGalleryDownButtonID(galleryBar) + ImagesPostfixes[4];
		}
		public static string GetGalleryPopOutButtonID(RibbonGalleryBarItem galleryBar) {
			return GetItemID(galleryBar) + "_PB";
		}
		public static string GetGalleryPopOutButtonImageID(RibbonGalleryBarItem galleryBar) {
			return GetGalleryPopOutButtonID(galleryBar) + ImagesPostfixes[2];
		}
		public static object[] GetHoverItemImageObjects(RibbonItemBase item, System.Web.UI.Page page) {
			return GetItemImageObjects(item, page, (img, pg) => img.GetHottrackedScriptObject(pg));
		}
		public static object[] GetDisabledItemImageObjects(RibbonItemBase item, System.Web.UI.Page page) {
			return GetItemImageObjects(item, page, (img, pg) => img.GetDisabledScriptObject(pg));
		}
		public static object[] GetPressedItemImageObjects(RibbonItemBase item, System.Web.UI.Page page) {
			return GetItemImageObjects(item, page, (img, pg) => img.GetPressedScriptObject(pg));
		}
		public static object[] GetSelectedItemImageObjects(RibbonItemBase item, System.Web.UI.Page page) {
			return GetItemImageObjects(item, page, (img, pg) => img.GetSelectedScriptObject(pg));
		}
		public static string[] GetItemImagePrefixes(RibbonItemBase item) {
			List<string> objects = new List<string>();
			if(!item.GetLargeImage().IsEmpty)
				objects.Add(ImagesPostfixes[0]);
			if(!item.GetSmallImage().IsEmpty)
				objects.Add(ImagesPostfixes[1]);
			if(item is RibbonDropDownButtonItem || item is RibbonColorButtonItemBase)
				objects.Add(ImagesPostfixes[2]);
			return objects.ToArray();
		}
		public static object[] GetExpandGroupButtonImageObjects(RibbonGroup group, System.Web.UI.Page page) {
			List<object> objects = new List<object>();
			if(!group.OneLineModeSettings.Image.IsEmpty)
				objects.Add(group.OneLineModeSettings.Image.GetHottrackedScriptObject(page));
			objects.Add(group.Ribbon.Images.GetPopOutImageProperties().GetHottrackedScriptObject(page));
			return objects.ToArray();
		}
		public static string[] GetGalleryBarImagePrefixes() {
			return new[] { ImagesPostfixes[3], ImagesPostfixes[4], ImagesPostfixes[2] };
		}
		static object[] GetItemImageObjects(RibbonItemBase item, System.Web.UI.Page page, Func<ImagePropertiesBase, System.Web.UI.Page, object> getObject) {
			List<object> objects = new List<object>();
			var img32 = item.GetLargeImage();
			var img16 = item.GetSmallImage();
			if(!img32.IsEmpty)
				objects.Add(getObject(img32, page));
			if(!img16.IsEmpty)
				objects.Add(getObject(img16, page));
			if(item is RibbonDropDownButtonItem || item is RibbonColorButtonItemBase)
				objects.Add(getObject(item.Ribbon.Images.GetPopOutImageProperties(), page));
			return objects.ToArray();
		}
		public static void AddItemControlCustomJSProperty(RibbonItemBase item, CustomJSPropertiesEventArgs e) {
			e.Properties.Add("cpRibbonItemID", RibbonHelper.GetItemID(item));
		}
		public static List<Hashtable> GetClientItems(IList<RibbonTab> tabs) {
			List<Hashtable> list = new List<Hashtable>();
			foreach(RibbonTab tab in tabs) {
				var obj = new Hashtable();
				if(tab.Visible) {
					if(!string.IsNullOrEmpty(tab.GetName()))
						obj["n"] = tab.GetName();
					obj["g"] = GetClientTabGroups(tab);
					obj["i"] = tab.GetCommonIndex();
					if(tab.IsContext && tab.ContextTabCategory.ClientVisible)
						obj["v"] = true;
					if(tab.IsContext) {
						obj["c"] = tab.IsContext;
						obj["cn"] = tab.ContextTabCategory.Name;
					}
				}
				list.Add(obj);
			}
			return list;
		}
		public static string GetGroupID(RibbonGroup group) {
			return string.Format("T{0}G{1}", group.Tab.GetCommonIndex(), group.Index);
		}
		public static string ToHexColor(Color color) {
			string hexColor = "";
			if(color != null && !color.IsEmpty)
				hexColor = string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
			return hexColor;
		}
		static List<Hashtable> GetClientTabGroups(RibbonTab tab) {
			List<Hashtable> list = new List<Hashtable>();
			foreach(RibbonGroup group in tab.Groups) {
				var obj = new Hashtable();
				if(group.Visible) {
					if(!string.IsNullOrEmpty(group.GetName()))
						obj["n"] = group.GetName();
					obj["i"] = GetClientItems(group.Items);
				}
				list.Add(obj);
			}
			return list;
		}
		static object GetClientItems(IEnumerable<RibbonItemBase> items) {
			List<Hashtable> list = new List<Hashtable>();
			foreach(RibbonItemBase item in items) {
				var obj = new Hashtable();
				if(item.GetVisible()) {
					if(!string.IsNullOrEmpty(item.GetName()))
						obj["c"] = item.GetName();
					obj["t"] = (int)item.ItemType;
					obj["txt"] = item.Text;
					if(item is RibbonToggleButtonItem)
						obj["chk"] = ((RibbonToggleButtonItem)item).Checked;
					if(item is RibbonDropDownToggleButtonItem)
						obj["chk"] = ((RibbonDropDownToggleButtonItem)item).Checked;
					if(item is RibbonColorButtonItemBase) {
						obj["color"] = ToHexColor(((RibbonColorButtonItemBase)item).Color);
						obj["iac"] = ((RibbonColorButtonItemBase)item).IsAutomaticColorSelected;
					}
					if(!string.IsNullOrEmpty(item.GetSubGroupName()))
						obj["sg"] = item.GetSubGroupName();
					if(item is RibbonOptionButtonItem)
						obj["og"] = ((RibbonOptionButtonItem)item).OptionGroupName;
					if(item is RibbonDropDownButtonItem && ((RibbonDropDownButtonItem)item).GetItems().GetVisibleItemCount() > 0)
						obj["i"] = GetClientItems(((RibbonDropDownButtonItem)item).GetItems());
					if(item is RibbonGalleryDropDownItem) {
						RibbonGalleryProperties galleryProperties = ((RibbonGalleryDropDownItem)item).PropertiesDropDownGallery;
						obj["pr"] = GetGalleryDropDownProperties(galleryProperties);
						obj["i"] = GetGalleryItems(galleryProperties.Groups);
					}
					if(item is RibbonGalleryBarItem) {
						if(item.Ribbon.OneLineMode)
							obj["t"] = (int)RibbonItemType.GalleryDropDown;
						var galleryBar = (RibbonGalleryBarItem)item;
						obj["pr"] = GetGalleryBarProperties(galleryBar);
						obj["i"] = GetGalleryItems(galleryBar.Groups);
					}
					var buttonItem = item as RibbonButtonItem;
					if(buttonItem != null && !string.IsNullOrEmpty(buttonItem.NavigateUrl))
						obj["nu"] = buttonItem.Ribbon.ResolveUrl(buttonItem.NavigateUrl);
					if(!item.ClientEnabled)
						obj["cdi"] = true;
				}
				list.Add(obj);
			}
			return list;
		}
		private static object GetGalleryBarProperties(RibbonGalleryBarItem galleryBar) {
			var obj = new Hashtable();
			obj["row"] = galleryBar.PropertiesDropDownGallery.RowCount;
			obj["asi"] = galleryBar.AllowSelectItem;
			obj["rowBar"] = galleryBar.RowCount;
			obj["colMin"] = galleryBar.MinColumnCount;
			obj["colMax"] = galleryBar.MaxColumnCount;
			obj["val"] = galleryBar.Value;
			obj["iw"] = galleryBar.ImageWidth;
			obj["ih"] = galleryBar.ImageHeight;
			obj["st"] = galleryBar.ShowText;
			obj["ip"] = galleryBar.ImagePosition;
			obj["tw"] = galleryBar.MaxTextWidth;
			return obj;
		}
		private static object GetGalleryDropDownProperties(RibbonGalleryProperties galleryProperties) {
			var obj = new Hashtable();
			obj["row"] = galleryProperties.RowCount;
			obj["col"] = galleryProperties.ColumnCount;
			obj["val"] = galleryProperties.Value;
			obj["asi"] = galleryProperties.AllowSelectItem;
			obj["iw"] = galleryProperties.ImageWidth;
			obj["ih"] = galleryProperties.ImageHeight;
			obj["st"] = galleryProperties.ShowText;
			obj["ip"] = galleryProperties.ImagePosition;
			obj["tw"] = galleryProperties.MaxTextWidth;
			return obj;
		}
		private static object GetGalleryItems(RibbonGalleryGroupCollection galleryGroups) {
			List<List<Hashtable>> groups = new List<List<Hashtable>>();
			foreach(RibbonGalleryGroup galleryGroup in galleryGroups) {
				List<Hashtable> group = new List<Hashtable>();
				var i = 0;
				foreach(RibbonGalleryItem galleryItem in galleryGroup.Items) {
					if(galleryItem.Visible) {
						var item = new Hashtable();
						item["indx"] = i;
						item["val"] = galleryItem.Value;
						group.Add(item);
					}
					i++;
				}
				groups.Add(group);
			}
			return groups;
		}
		public static IEnumerable<RibbonGalleryItem> GetRibbonGalleriesItems(RibbonItemBase gallery) {
			IEnumerable<RibbonGalleryItem> galleryItems = null;
			if(gallery is RibbonGalleryDropDownItem)
				galleryItems = ((RibbonGalleryDropDownItem)gallery).GetAllItems();
			else if(gallery is RibbonGalleryBarItem)
				galleryItems = ((RibbonGalleryBarItem)gallery).GetAllItems();
			return galleryItems;
		}
		public static ASPxRibbon LookupRibbonControl(Control control, string ribbonControlID) {
			if(control == null || control.Page == null || String.IsNullOrEmpty(ribbonControlID))
				return null;
			List<string> splitID = ribbonControlID.Trim(' ').Split('.').ToList();
			string controlID = splitID[splitID.Count - 1];
			string masterPageName = splitID[0].ToLower();
			bool lookupInMasterPage = control.Page.Master != null && splitID.Count > 1 &&
				(masterPageName == control.Page.Master.GetType().BaseType.Name.ToLower() || masterPageName == control.Page.Master.GetType().Name.ToLower());
			Control foundControl = lookupInMasterPage ? FindControlHelper.LookupControlRecursive(control.Page.Master, controlID) : FindControlHelper.LookupControl(control, controlID);
			if(foundControl != null)
				return foundControl as ASPxRibbon;
			throw new Exception(string.Format(StringResources.RibbonExceptionText_ControlNotFound, controlID));
		}
		public static void AddTabCollectionToControl(ASPxRibbon ribbon, RibbonTab[] tabs, bool clearExistingTabs) {
			if(clearExistingTabs) {
				foreach(RibbonTab tab in tabs) {
					RibbonTab foundTab = ribbon.Tabs.Find(i => i.GetType() == tab.GetType());
					if(foundTab != null)
						ribbon.Tabs.RemoveAll(i => i.GetType() == foundTab.GetType());
				}
			}
			ribbon.Tabs.AddRange(tabs);
		}
		public static void AddContextCategoriesToControl(ASPxRibbon ribbon, RibbonContextTabCategory[] tabCategories, bool clearExistingTabs) {
			if(clearExistingTabs) {
				foreach(RibbonContextTabCategory tabCategory in tabCategories) {
					RibbonContextTabCategory foundTab = ribbon.ContextTabCategories.Find(i => i.GetType() == tabCategory.GetType());
					if(foundTab != null)
						ribbon.ContextTabCategories.RemoveAll(i => i.GetType() == foundTab.GetType());
				}
			}
			ribbon.ContextTabCategories.AddRange(tabCategories);
		}
		public static Control FindTemplateControlRecursive(Control control, string templateContainerID, string id) {
			Control container = FindControlHelper.LookupControlRecursive(control, templateContainerID);
			return (container != null) ? container.FindControl(id) : null;
		}
		public static ImageProperties GetRibbonImageProperties(ASPxRibbon ribbon, string spriteName, CreateRibbonImages create, string imageName, string imageSize) {
			RibbonImages images = ribbon.Images.GetImagesBySpriteName(spriteName, create) as RibbonImages;
			return images.GetImageProperties(ribbon.Page, imageName + imageSize);
		}
		public static void SyncRibbonControlCollection(Collection<RibbonTab> officeControlRibbonTabCollection, ASPxRibbon ribbon, NameValueCollection postCollection) {
			if(ribbon != null) {
				RenderUtils.LoadPostDataRecursive(ribbon, postCollection);
				foreach(RibbonTab tab in officeControlRibbonTabCollection)
					foreach(RibbonGroup group in tab.Groups)
						foreach(RibbonItemBase item in group.Items)
							if(!(item is IRibbonInternalItem) && (item is RibbonEditItemBase)) {
								var ribbonItem = ribbon.Tabs[tab.Index].Groups[group.Index].Items[item.Index];
								((RibbonEditItemBase)item).Value = ((RibbonEditItemBase)ribbonItem).Value;
							}
			}
		}
	}
	public delegate RibbonImages CreateRibbonImages(ISkinOwner skinOwner);
	public enum RibbonBlockType {
		RegularItems,
		HorizontalItems,
		LargeItems,
		SeparateItems
	}
	public enum RibbonItemType {
		Button = 0,
		Template = 1,
		DropDownButton = 2,
		DropDownMenuItem = 3,
		ToggleButton = 4,
		OptionButton = 5,
		SpinEdit = 6,
		ColorButton = 7,
		TextBox = 8,
		DateEdit = 9,
		ComboBox = 10,
		CheckBox = 11,
		DropDownToggleButton = 12,
		GalleryDropDown = 13,
		GalleryBar = 14
	}
	public class RibbonClientStateHelper {
		internal const string ActiveTabIndex = "ActiveTabIndex";
		internal const char ColorItemStateSeparator = '|';
		public RibbonClientStateHelper(ASPxRibbon ribbon) {
			Ribbon = ribbon;
		}
		public ASPxRibbon Ribbon { get; private set; }
		public Hashtable State { get; private set; }
		public void SyncClientState(Hashtable state) {
			State = state;
			SyncActiveTabIndex();
			SyncItems();
		}
		public void SyncClientStateString(string stateString) {
			SyncClientState(HtmlConvertor.FromJSON<Hashtable>(stateString));
		}
		public Hashtable GetClientState() {
			Hashtable state = new Hashtable();
			state.Add(ActiveTabIndex, Ribbon.ActiveTabIndex);
			foreach(RibbonTab tab in Ribbon.AllTabs) {
				AddTabStateToState(state, tab);
				foreach(RibbonGroup group in tab.Groups) {
					foreach(RibbonItemBase item in group.Items) {
						switch(item.ItemType) {
							case RibbonItemType.ColorButton:
								AddColorItemStateToState(state, (RibbonColorButtonItemBase)item);
								break;
							case RibbonItemType.OptionButton:
							case RibbonItemType.ToggleButton:
								AddCheckStateToState(state, item, ((RibbonToggleButtonItem)item).Checked);
								break;
							case RibbonItemType.DropDownToggleButton:
								AddDropDownToggleItemState(state, (RibbonDropDownToggleButtonItem)item);
								break;
							case RibbonItemType.GalleryDropDown:
								AddGalleryStateToState(state, item, ((RibbonGalleryDropDownItem)item).PropertiesDropDownGallery.Value);
								break;
							case RibbonItemType.GalleryBar:
								AddGalleryStateToState(state, item, ((RibbonGalleryBarItem)item).Value);
								break;
						}
					}
				}
			}
			return state;
		}
		public string GetClientStateString() {
			return HtmlConvertor.ToJSON(GetClientState(), false, false, true);
		}
		private void AddTabStateToState(Hashtable state, RibbonTab tab) {
			if(tab.IsContext && tab.ContextTabCategory.ClientVisible)
				state.Add(RibbonHelper.GetRibbonTabID(tab), true);
		}
		private void AddDropDownToggleItemState(Hashtable state, RibbonDropDownToggleButtonItem item) {
			AddCheckStateToState(state, item, item.Checked);
			foreach(RibbonItemBase menuItem in ((RibbonDropDownToggleButtonItem)item).Items) {
				if(menuItem.ItemType == RibbonItemType.DropDownToggleButton)
					AddDropDownToggleItemState(state, (RibbonDropDownToggleButtonItem)menuItem);
			}
		}
		protected void AddColorItemStateToState(Hashtable state, RibbonColorButtonItemBase item) {
			var properties = item.ColorNestedControlProperties;
			string itemState = properties.GetSerializedCustomColorTableItems() + ColorItemStateSeparator + ColorTranslator.ToHtml(item.Color);
			if(!string.IsNullOrEmpty(itemState))
				state.Add(RibbonHelper.GetItemID(item), itemState);
		}
		protected void AddCheckStateToState(Hashtable state, RibbonItemBase item, bool checkState) {
			if(checkState)
				state.Add(RibbonHelper.GetItemID(item), checkState);
		}
		protected void AddGalleryStateToState(Hashtable state, RibbonItemBase item, object galleryState) {
			if(galleryState != null)
				state.Add(RibbonHelper.GetItemID(item), galleryState);
		}
		protected void SyncActiveTabIndex() {
			if(State.ContainsKey(ActiveTabIndex))
				Ribbon.ActiveTabIndex = (int)State[ActiveTabIndex];
		}
		protected void SyncItems() {
			foreach(RibbonTab tab in Ribbon.AllTabs) {
				SyncTab(tab);
				foreach(RibbonGroup group in tab.Groups) {
					foreach(RibbonItemBase item in group.Items) {
						SyncItem(item);
						if(item is RibbonDropDownButtonItem)
							SyncMenuItems(((RibbonDropDownButtonItem)item).Items);
					}
				}
			}
		}
		protected void SyncItem(RibbonItemBase item) {
			string itemId = RibbonHelper.GetItemID(item);
			if(State.ContainsKey(itemId)) {
				switch(item.ItemType) {
					case RibbonItemType.ColorButton:
						var colorButton = (RibbonColorButtonItemBase)item;
						string[] args = (State[itemId].ToString()).Split(new char[] { ColorItemStateSeparator });
						var properties = colorButton.ColorNestedControlProperties;
						properties.DeserializeColorsToCustomColorTableItems(args[0]);
						if(args.Length > 1) {
							Color color = ColorTranslator.FromHtml(args[1]);
							if(colorButton.Color != color)
								colorButton.Color = color;
						}
						if(args.Length > 2)
							colorButton.IsAutomaticColorSelected = bool.Parse(args[2]);
						break;
					case RibbonItemType.OptionButton:
					case RibbonItemType.ToggleButton:
						((RibbonToggleButtonItem)item).Checked = bool.Parse(State[itemId].ToString());
						break;
					case RibbonItemType.DropDownToggleButton:
						((RibbonDropDownToggleButtonItem)item).Checked = bool.Parse(State[itemId].ToString());
						break;
					case RibbonItemType.GalleryDropDown:
						((RibbonGalleryDropDownItem)item).PropertiesDropDownGallery.Value = State[itemId] != null ? State[itemId].ToString() : null;
						break;
					case RibbonItemType.GalleryBar:
						((RibbonGalleryBarItem)item).Value = State[itemId] != null ? State[itemId].ToString() : null;
						break;
				}
			}
		}
		private void SyncMenuItems(RibbonDropDownButtonCollection items) {
			foreach(RibbonDropDownButtonItem item in items) {
				SyncItem(item);
				SyncMenuItems(item.Items);
			}
		}
		private void SyncTab(RibbonTab tab) {
			var tabID = RibbonHelper.GetRibbonTabID(tab);
			if(State.ContainsKey(tabID)) {
				var visible = bool.Parse(State[tabID].ToString());
				tab.ClientVisibleInternal = visible;
				tab.ContextTabCategory.ClientVisible = visible;
			}
		}
	}
	public class RibbonGalleryInfo {
		public RibbonGalleryInfo(RibbonGalleryDropDownItem item) {
			Groups = item.PropertiesDropDownGallery.Groups;
			ImageHeight = item.PropertiesDropDownGallery.ImageHeight;
			ImageWidth = item.PropertiesDropDownGallery.ImageWidth;
			MaxTextWidth = item.PropertiesDropDownGallery.MaxTextWidth;
			ImagePosition = item.PropertiesDropDownGallery.ImagePosition;
			ShowGroupCaption = item.PropertiesDropDownGallery.ShowGroupText;
			ShowText = item.PropertiesDropDownGallery.ShowText;
		}
		public RibbonGalleryInfo(RibbonGalleryBarItem item) {
			Groups = item.Groups;
			ImageHeight = item.ImageHeight;
			ImageWidth = item.ImageWidth;
			MaxTextWidth = item.MaxTextWidth;
			ImagePosition = item.ImagePosition;
			ShowGroupCaption = item.PropertiesDropDownGallery.ShowGroupText;
			ShowText = item.ShowText;
			MaxColumnCount = item.MaxColumnCount;
			RowCount = item.RowCount;
		}
		public RibbonGalleryGroupCollection Groups { get; set; }
		public bool ShowText { get; set; }
		public bool ShowGroupCaption { get; set; }
		public ImagePosition ImagePosition { get; set; }
		public Unit ImageWidth { get; set; }
		public Unit ImageHeight { get; set; }
		public Unit MaxTextWidth { get; set; }
		public int MaxColumnCount { get; set; }
		public int RowCount { get; set; }
	}
}
