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
using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Preview;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraPrinting.Native {
	public interface ControllerRibbonPageGroupKind<T> {
		T Kind { get; set; } 
	}
	public abstract class RibbonControllerConfiguratorBase {
		#region static
		protected static Dictionary<string, Image> GetImagesFromAssembly(Assembly assembly, string resourcePath, string imagePrefix) {
			Dictionary<string, Image> images = new Dictionary<string, Image>();
			foreach(string resourceName in assembly.GetManifestResourceNames()) {
				if(resourceName.IndexOf(resourcePath) == 0) {
					System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
					images[Path.GetFileNameWithoutExtension(resourceName.Replace(resourcePath, imagePrefix))] = Bitmap.FromStream(stream);
				}
			}
			return images;
		}
		#endregion
		protected RibbonControl ribbonControl;
		protected RibbonStatusBar ribbonStatusBar;
		protected Dictionary<string, Image> ribbonImages;
		protected int currentID;
		protected const string CaptionSuffix = "_Caption";
		protected const string DescriptionSuffix = "_Description";
		protected const string STipTitleSuffix = "_STipTitle";
		protected const string STipContentSuffix = "_STipContent";
		protected const string LargeImageSuffix = "Large";
		protected string RibbonPrefix;
		protected string RibbonImagesNamePrefix;
		protected string ReferencedNamePrefix;
		object contextSpecifier;
		public virtual object ContextSpecifier { get { return contextSpecifier; } }
		protected string RibbonPageGroupPrefix { get { return RibbonPrefix + "PageGroup_"; } }
		protected string PageText { get { return RibbonPrefix + "PageText"; } }
		internal RibbonControl RibbonControl { get { return ribbonControl; } }
		protected RibbonControllerConfiguratorBase(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> ribbonImages) {
			this.ribbonControl = ribbonControl;
			this.ribbonStatusBar = ribbonStatusBar;
			this.ribbonImages = ribbonImages;
		}
		public void Configure(object contextSpecifier) {
			this.contextSpecifier = contextSpecifier;
			try {
				currentID = RibbonControl.Manager.MaxItemId;
				CreateItems();
				CreatePageGroups();
				RibbonControl.Manager.MaxItemId = currentID;
			}
			finally {
				this.contextSpecifier = null;
			}
		}
		protected abstract void CreateItems();
		protected abstract void CreatePageGroups();
		protected abstract string GetLocalizedString(string str);
		protected abstract string GetDefaultLocalizedString(string str);
		protected abstract string BarItemCommandToString(BarItem item);
		protected void LocalizeStrings<TRibbonPageGroup, TBarItem, TRuntimeBarItem, TPageGroupKind>(RibbonControl ribbonControl)
			where TRibbonPageGroup : RibbonPageGroup, ControllerRibbonPageGroupKind<TPageGroupKind>
			where TBarItem : BarItem {
			foreach(RibbonPage page in ribbonControl.Pages) {
				string pageText = GetTextForRibbonPage(page);
				if(!string.IsNullOrEmpty(pageText))
					page.Text = ConditionalLocalizeString(page.Text, ribbonControl, pageText);
				foreach(RibbonPageGroup group in page.Groups) {
					TRibbonPageGroup ppGroup = group as TRibbonPageGroup;
					if(ppGroup != null) {
						ppGroup.Text = ConditionalLocalizeString(ppGroup.Text, ribbonControl, RibbonPageGroupPrefix + ((TRibbonPageGroup)group).Kind.ToString());
						LocalizeSuperToolTipStrings(ribbonControl, ppGroup.SuperTip, RibbonPageGroupPrefix + ppGroup.Kind.ToString());
					}
				}
			}
			foreach(BarItem barItem in ribbonControl.Manager.Items) {
				TBarItem item = barItem as TBarItem;
				if(item != null && !(item is TRuntimeBarItem)) {
					item.Caption = ConditionalLocalizeString(item.Caption, ribbonControl, RibbonPrefix + BarItemCommandToString(item) + CaptionSuffix);
					LocalizeSuperToolTipStrings(ribbonControl, item.SuperTip, RibbonPrefix + BarItemCommandToString(item), item.ItemShortcut);
					if(item.Description != null)
						item.Description = ConditionalLocalizeString(item.Description, ribbonControl, RibbonPrefix + BarItemCommandToString(item) + DescriptionSuffix);
				}
			}
		}
		protected abstract string GetTextForRibbonPage(RibbonPage page); 
		protected string ConditionalLocalizeString(string strToLocalize, RibbonControl ribbonControl, string str) {
			string defaultSring = GetDefaultLocalizedString(str);
			if(defaultSring != null && (string.IsNullOrEmpty(strToLocalize) || string.Equals(strToLocalize, defaultSring))) {
				return GetLocalizedString(str);
			}
			return strToLocalize;
		}
		protected void LocalizeSuperToolTipStrings(RibbonControl ribbonControl, SuperToolTip superTip, string sTipAlias) { 
			LocalizeSuperToolTipStrings(ribbonControl, superTip, sTipAlias, null);
		}
		protected void LocalizeSuperToolTipStrings(RibbonControl ribbonControl, SuperToolTip superTip, string sTipAlias, BarShortcut barShortcut) {
			if(superTip != null) {
				foreach(ToolTipItem item in superTip.Items)
					if(item is ToolTipTitleItem) {
						if(!Object.ReferenceEquals(barShortcut, null) && barShortcut.IsExist) {
							string shortcut = " (" + barShortcut.ToString() + ")"; 
							item.Text = ConditionalLocalizeString(GetItemText(item.Text, shortcut), ribbonControl, sTipAlias + STipTitleSuffix) + shortcut;
						} else {
							item.Text = ConditionalLocalizeString(item.Text, ribbonControl, sTipAlias + STipTitleSuffix);
						}
					} else {
						item.Text = ConditionalLocalizeString(item.Text, ribbonControl, sTipAlias + STipContentSuffix);
					}
			}
		}
		static string GetItemText(string itemText, string shortcut) {
			if(string.IsNullOrEmpty(itemText))
				return string.Empty;
			if(itemText.EndsWith(shortcut))
			   return itemText.Substring(0, itemText.Length - shortcut.Length);
		   return itemText;
		}
		protected void AddBarItem(BarItem item, string commandString, RibbonItemStyles ribbonStyle, int groupIndex, string description) {
			item.SuperTip = CreateSuperToolTip(RibbonPrefix + commandString);
			item.Glyph = GetRibbonImage(commandString);
			item.LargeGlyph = GetRibbonImageLarge(commandString);
			item.Description = description;
			item.RibbonStyle = ribbonStyle;
			AddBarItem(item, groupIndex);
		}
		protected void AddBarItem(BarItem barItem, int groupIndex) {
			if(barItem is BarButtonItem && groupIndex != -1)
				((BarButtonItem)barItem).GroupIndex = groupIndex;
			AddBarItem(barItem);
		}
		protected void AddBarItem(BarItem barItem) {
			barItem.Id = GetID();
			if(barItem is ISupportContextSpecifier)
				((ISupportContextSpecifier)barItem).ContextSpecifier = this.ContextSpecifier;
			ribbonControl.Items.Add(barItem);
			if(barItem is BarEditItem)
				ribbonControl.Manager.RepositoryItems.Add(((BarEditItem)barItem).Edit);
			BarManagerConfigurator.AddToContainer(ribbonControl.Container, barItem);
		}
		protected RibbonPage CreateRibbonPage(Type pageType) {
			RibbonPage ribbonPage = (RibbonPage)Activator.CreateInstance(pageType);
			if(ribbonControl.Container != null) {
				INameCreationService serv = ribbonControl.Site != null ? ribbonControl.Site.GetService(typeof(INameCreationService)) as INameCreationService : null;
				if(serv != null) {
					string name = serv.CreateName(ribbonControl.Container, typeof(RibbonPage));
					ribbonControl.Container.Add(ribbonPage, name);
				} else
					ribbonControl.Container.Add(ribbonPage);
			}
			return ribbonPage;
		}
		protected T CreatePageGroup<T, E>(RibbonPage page, E kind, string glyphName) where T : RibbonPageGroup, ControllerRibbonPageGroupKind<E>, new() {
			T group = new T();
			group.AllowTextClipping = false;
			group.Kind = kind;
			group.Name = kind.ToString();
			group.ShowCaptionButton = false;
			group.Glyph = GetRibbonImage(glyphName);
			page.Groups.Add(group);
			BarManagerConfigurator.AddToContainer(ribbonControl.Container, group);
			return group;
		}
		protected Image GetRibbonImage(string imageName) {
			string fullName = RibbonImagesNamePrefix + imageName;
			return ribbonImages.ContainsKey(fullName) ? ribbonImages[fullName] : null;
		}
		protected Image GetRibbonImageLarge(string imageName) {
			return GetRibbonImage(GetLargeImageName(imageName));
		}
		protected SuperToolTip CreateSuperToolTip(string sTipAlias) {
			return CreateSuperToolTip(sTipAlias, null);
		}
		protected SuperToolTip CreateSuperToolTip(string sTipAlias, Image image) {
			SuperToolTip sTip = new SuperToolTip();
			ToolTipTitleItem title = new ToolTipTitleItem();
			sTip.Items.Add(title);
			ToolTipItem content = new ToolTipItem();
			content.LeftIndent = 6;
			if(image != null) content.Image = image;
			sTip.Items.Add(content);
			sTip.FixedTooltipWidth = true;
			sTip.MaxWidth = image == null ? 210 : 318;
			return sTip;
		}
		internal static string GetLargeImageName(string imageName) {
			return imageName + LargeImageSuffix;
		}
		int GetID() {
			return currentID++;
		}
	}
}
