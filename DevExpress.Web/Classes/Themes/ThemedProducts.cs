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
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DevExpress.Web.Internal {
	public abstract class ThemedProducts {
		public const string ASPxperienceName = "ASPxperience";
		public const string ASPxEditorsName = "ASPxEditors";
		public const string ASPxGridViewName = "ASPxGridView";
		public const string ASPxTreeListName = "ASPxTreeList";
		public const string ASPxHtmlEditorName = "ASPxHtmlEditor";
		public const string ASPxSpellCheckerName = "ASPxSpellChecker";
		public const string ASPxPivotGridName = "ASPxPivotGrid";
		public const string ASPxSchedulerName = "ASPxScheduler";
		public const string ASPxSpreadsheetName = "ASPxSpreadsheet";
		public const string ASPxRichEditName = "ASPxRichEdit";
		public const string XtraReportsName = "XtraReports";
		public const string XtraChartsName = "XtraCharts";
		public const string MVCExtensionsName = "MVC Extensions";
		public const string XafName = "eXpressApp Framework";
		private static Dictionary<string, ThemedProduct> dictionary = new Dictionary<string, ThemedProduct>();
		static ThemedProducts() {
			dictionary.Add(ASPxperienceName, new ASPxperienceProduct());
			dictionary.Add(ASPxEditorsName, new ASPxEditorsProduct());
			dictionary.Add(ASPxGridViewName, new ASPxGridViewProduct());
			dictionary.Add(ASPxTreeListName, new ASPxTreeListProduct());
			dictionary.Add(ASPxHtmlEditorName, new ASPxHtmlEditorProduct());
			dictionary.Add(ASPxSpellCheckerName, new ASPxSpellCheckerProduct());
			dictionary.Add(ASPxPivotGridName, new ASPxPivotGridProduct());
			dictionary.Add(ASPxSchedulerName, new ASPxSchedulerProduct());
			dictionary.Add(ASPxSpreadsheetName, new ASPxSpreadsheetProduct());
			dictionary.Add(ASPxRichEditName, new ASPxRichEditProduct());
			dictionary.Add(XtraReportsName, new XtraReportsProduct());
			dictionary.Add(XtraChartsName, new XtraChartsProduct());
			dictionary.Add(MVCExtensionsName, new MVCExtensionsProduct());
			dictionary.Add(XafName, new XafProduct());
		}
		public static ThemedProduct[] GetList() {
			List<ThemedProduct> list = new List<ThemedProduct>();
			foreach(string name in dictionary.Keys)
				list.Add(dictionary[name]);
			return list.ToArray();
		}
		public static ThemedProduct ASPxperience { get { return dictionary[ASPxperienceName]; } }
		public static ThemedProduct ASPxEditors { get { return dictionary[ASPxEditorsName]; } }
		public static ThemedProduct ASPxGridView { get { return dictionary[ASPxGridViewName]; } }
		public static ThemedProduct ASPxTreeList { get { return dictionary[ASPxTreeListName]; } }
		public static ThemedProduct ASPxHtmlEditor { get { return dictionary[ASPxHtmlEditorName]; } }
		public static ThemedProduct ASPxSpellChecker { get { return dictionary[ASPxSpellCheckerName]; } }
		public static ThemedProduct ASPxPivotGrid { get { return dictionary[ASPxPivotGridName]; } }
		public static ThemedProduct ASPxScheduler { get { return dictionary[ASPxSchedulerName]; } }
		public static ThemedProduct ASPxSpreadsheet { get { return dictionary[ASPxSpreadsheetName]; } }
		public static ThemedProduct ASPxRichEdit { get { return dictionary[ASPxRichEditName]; } }
		public static ThemedProduct XtraReports { get { return dictionary[XtraReportsName]; } }
		public static ThemedProduct XtraCharts { get { return dictionary[XtraChartsName]; } }
		public static ThemedProduct MVCExtensions { get { return dictionary[MVCExtensionsName]; } }
		public static ThemedProduct Xaf { get { return dictionary[XafName]; } }
	}
	public abstract class ThemedProduct {
		public abstract string Name { get; }
		public abstract string Title { get; }
		public abstract string[] Folders { get; }
		public abstract string[] SkinFiles { get; }
		public abstract ThemedProduct[] SubProducts { get; }
		public virtual bool Public { get { return true; } }
		public virtual bool ExtractSubProductsSkinFiles { get { return false; } }
		public abstract string AssemblyName { get; }
		public virtual bool AllowCustomizeSkinFile(string controlName) {
			return true;
		}
		public virtual bool AllowCustomizeCssFiles(string controlName) {
			return true;
		}
		public virtual Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return new Dictionary<string, Dictionary<string, string>>();
		}
		public virtual string[] GetControlElements(string controlName) {
			Dictionary<string, Dictionary<string, string>> collection = GetControlElementsMapCollection();
			if(collection.ContainsKey(controlName))
				return new List<string>(collection[controlName].Keys).ToArray();
			return new string[0];
		}
		public virtual string GetControlElementCssClass(string controlName, string elementName) {
			Dictionary<string, Dictionary<string, string>> collection = GetControlElementsMapCollection();
			if(collection.ContainsKey(controlName) && collection[controlName].ContainsKey(elementName)) {
				return collection[controlName][elementName];
			}
			return string.Empty;
		}
		public override string ToString() {
			return Title;
		}
	}
	public class ASPxperienceProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxperienceProduct() {
			ControlElementsMapCollection.Add("ASPxCallbackPanel", ASPxperienceProduct.CreateCallbackPanelElementsMap());
			ControlElementsMapCollection.Add("ASPxNavBar", ASPxperienceProduct.CreateNavBarElementsMap());
			ControlElementsMapCollection.Add("ASPxCloudControl", ASPxperienceProduct.CreateCloudControlElementsMap());
			ControlElementsMapCollection.Add("ASPxDataView", ASPxperienceProduct.CreateDataViewElementsMap());
			ControlElementsMapCollection.Add("ASPxHeadline", ASPxperienceProduct.CreateHeadlineElementsMap());
			ControlElementsMapCollection.Add("ASPxDockPanel", ASPxperienceProduct.CreateDockPanelElementsMap());
			ControlElementsMapCollection.Add("ASPxFileManager", ASPxperienceProduct.CreateFileManagerElementsMap());
			ControlElementsMapCollection.Add("ASPxFormLayout", ASPxperienceProduct.CreateFormLayoutElementsMap());
			ControlElementsMapCollection.Add("ASPxImageGallery", ASPxperienceProduct.CreateImageGalleryElementsMap());
			ControlElementsMapCollection.Add("ASPxImageSlider", ASPxperienceProduct.CreateImageSliderElementsMap());
			ControlElementsMapCollection.Add("ASPxImageZoom", ASPxperienceProduct.CreateImageZoomElementsMap());
			ControlElementsMapCollection.Add("ASPxImageZoomNavigator", ASPxperienceProduct.CreateImageZoomNavigatorElementsMap());
			ControlElementsMapCollection.Add("ASPxLoadingPanel", ASPxperienceProduct.CreateLoadingPanelElementsMap());
			ControlElementsMapCollection.Add("ASPxMenu", ASPxperienceProduct.CreateMenuElementsMap());
			ControlElementsMapCollection.Add("ASPxNewsControl", ASPxperienceProduct.CreateNewsControlElementsMap());
			ControlElementsMapCollection.Add("ASPxPanel", ASPxperienceProduct.CreatePanelElementsMap());
			ControlElementsMapCollection.Add("ASPxPageControl", ASPxperienceProduct.CreatePageControlElementsMap());
			ControlElementsMapCollection.Add("ASPxPager", ASPxperienceProduct.CreatePagerElementsMap());
			ControlElementsMapCollection.Add("ASPxPopupControl", ASPxperienceProduct.CreatePopupControlElementsMap());
			ControlElementsMapCollection.Add("ASPxPopupMenu", ASPxperienceProduct.CreatePopupMenuElementsMap());
			ControlElementsMapCollection.Add("ASPxRibbon", ASPxperienceProduct.CreateRibbonElementsMap());
			ControlElementsMapCollection.Add("ASPxRoundPanel", ASPxperienceProduct.CreateRoundPanelElementsMap());
			ControlElementsMapCollection.Add("ASPxSiteMapControl", ASPxperienceProduct.CreateSiteMapControlElementsMap());
			ControlElementsMapCollection.Add("ASPxSplitter", ASPxperienceProduct.CreateSplitterElementsMap());
			ControlElementsMapCollection.Add("ASPxTabControl", ASPxperienceProduct.CreateTabControlElementsMap());
			ControlElementsMapCollection.Add("ASPxTitleIndex", ASPxperienceProduct.CreateTitleIndexElementsMap());
			ControlElementsMapCollection.Add("ASPxTreeView", ASPxperienceProduct.CreateTreeViewElementsMap());
			ControlElementsMapCollection.Add("ASPxUploadControl", ASPxperienceProduct.CreateUploadControlElementsMap());
		}
		static Dictionary<string, string> CreatePanelElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxPanelElementsCss.Control);
			elements.Add("Expand Bar", ASPxPanelElementsCss.ExpandBar);
			elements.Add("Expand Button", ASPxPanelElementsCss.ExpandButton);
			return elements;
		}
		static Dictionary<string, string> CreateCallbackPanelElementsMap() {
			Dictionary<string, string> elements = CreatePanelElementsMap();
			elements.Add("Loading Div", ASPxLoadingPanelElementsCss.LoadingDiv);
			elements.Add("Loading Panel", ASPxLoadingPanelElementsCss.LoadingPanel);
			return elements;
		}
		static Dictionary<string, string> CreateNavBarElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxNavBarElementsCss.Control);
			elements.Add("Group Header", ASPxNavBarElementsCss.GroupHeader);
			elements.Add("Group Header Collapsed", ASPxNavBarElementsCss.GroupHeaderCollapsed);
			elements.Add("Expand Image", ASPxNavBarElementsCss.ExpandImage);
			elements.Add("Collapse Image", ASPxNavBarElementsCss.CollapseImage);
			elements.Add("Group Content", ASPxNavBarElementsCss.GroupContent);
			elements.Add("Item", ASPxNavBarElementsCss.Item);
			return elements;
		}
		static Dictionary<string, string> CreateCloudControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCloudControlElementsCss.Control);
			return elements;
		}
		static Dictionary<string, string> CreateDataViewElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxDataViewElementsCss.Control);
			elements.Add("Content", ASPxDataViewElementsCss.Content);
			elements.Add("Item", ASPxDataViewElementsCss.Item);
			elements.Add("Pager Panel", ASPxPagerElementsCss.Control);
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			return elements;
		}
		static Dictionary<string, string> CreateHeadlineElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxHeadlineElementsCss.Control);
			elements.Add("Header", ASPxHeadlineElementsCss.Header);
			elements.Add("Date", ASPxHeadlineElementsCss.Date);
			elements.Add("Content", ASPxHeadlineElementsCss.Content);
			elements.Add("Tail", ASPxHeadlineElementsCss.Tail);
			return elements;
		}
		static Dictionary<string, string> CreateFileManagerElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxFileManagerElementsCss.Control);
			elements.Add("Toolbar", ASPxFileManagerElementsCss.Toolbar);
			elements.Add("Toolbar Item", ASPxFileManagerElementsCss.ToolbarItem);
			elements.Add("Folder", ASPxFileManagerElementsCss.Folder);
			elements.Add("File", ASPxFileManagerElementsCss.File);
			elements.Add("Filter", ASPxFileManagerElementsCss.Filter);
			elements.Add("Upload Panel", ASPxFileManagerElementsCss.UploadPanel);
			elements.Add("Create Button Image", ASPxFileManagerElementsCss.CreateButtonImage);
			elements.Add("Rename Button Image", ASPxFileManagerElementsCss.RenameButtonImage);
			elements.Add("Move Button Image", ASPxFileManagerElementsCss.MoveButtonImage);
			elements.Add("Delete Button Image", ASPxFileManagerElementsCss.DeleteButtonImage);
			elements.Add("Refresh Button Image", ASPxFileManagerElementsCss.RefreshButtonImage);
			elements.Add("Download Button Image", ASPxFileManagerElementsCss.DownloadButtonImage);
			elements.Add("Copy Button Image", ASPxFileManagerElementsCss.CopyButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateFormLayoutElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxFormLayoutElementsCss.Control);
			elements.Add("Group Box", ASPxFormLayoutElementsCss.GroupBox);
			elements.Add("Group Box Caption", ASPxFormLayoutElementsCss.GroupBoxCaption);
			elements.Add("Group", ASPxFormLayoutElementsCss.Group);
			elements.Add("Group Cell", ASPxFormLayoutElementsCss.GroupCell);
			elements.Add("Item", ASPxFormLayoutElementsCss.Item);
			elements.Add("Item Caption Cell", ASPxFormLayoutElementsCss.ItemCaptionCell);
			elements.Add("Item Editor Cell", ASPxFormLayoutElementsCss.ItemNestedControlCell);
			elements.Add("Required Label", ASPxFormLayoutElementsCss.RequiredLabel);
			elements.Add("Optional Label", ASPxFormLayoutElementsCss.OptionalLabel);
			return elements;
		}
		static Dictionary<string, string> CreateImageGalleryElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxImageGalleryElementsCss.Control);
			elements.Add("Content", ASPxImageGalleryElementsCss.Content);
			elements.Add("Item", ASPxImageGalleryElementsCss.Item);
			elements.Add("Item Text", ASPxImageGalleryElementsCss.ItemText);
			elements.Add("Pager Panel", ASPxPagerElementsCss.Control);
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			elements.Add("Fullscreen Viewer Item", ASPxImageSliderElementsCss.Item);
			elements.Add("Fullscreen Viewer Previous Button Image", ASPxImageGalleryElementsCss.FullscreenViewerPrevButtonImage);
			elements.Add("Fullscreen Viewer Next Button Image", ASPxImageGalleryElementsCss.FullscreenViewerNextButtonImage);
			elements.Add("Fullscreen Viewer Thumbnail Item", ASPxImageSliderElementsCss.NavigationBarItem);
			elements.Add("Fullscreen Viewer Thumbnail Selected Item", ASPxImageSliderElementsCss.NavigationBarSelectedItem);
			elements.Add("Fullscreen Viewer Item Text", ASPxImageGalleryElementsCss.FullscreenViewerItemText);
			elements.Add("Fullscreen Viewer Navigation Bar Previous Page Button Image", ASPxImageSliderElementsCss.NavigationBarPrevButton);
			elements.Add("Fullscreen Viewer Navigation Bar Next Page Button Image", ASPxImageSliderElementsCss.NavigationBarNextButton);
			elements.Add("Fullscreen Viewer Play Button Image", ASPxImageGalleryElementsCss.FullscreenViewerPlayButtonImage);
			elements.Add("Fullscreen Viewer Pause Button Image", ASPxImageGalleryElementsCss.FullscreenViewerPauseButtonImage);
			elements.Add("Fullscreen Viewer Close Button Image", ASPxImageGalleryElementsCss.FullscreenViewerCloseButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateImageSliderElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxImageSliderElementsCss.Control);
			elements.Add("Item", ASPxImageSliderElementsCss.Item);
			elements.Add("Previous Button Image Horizontal", ASPxImageSliderElementsCss.PrevButtonHorizontal);
			elements.Add("Next Button Image Horizontal", ASPxImageSliderElementsCss.NextButtonHorizontal);
			elements.Add("Previous Button Image Vertical", ASPxImageSliderElementsCss.PrevButtonVertical);
			elements.Add("Next Button Image Vertical", ASPxImageSliderElementsCss.NextButtonVertical);
			elements.Add("Navigation Bar Item", ASPxImageSliderElementsCss.NavigationBarItem);
			elements.Add("Navigation Bar Selected Item", ASPxImageSliderElementsCss.NavigationBarSelectedItem);
			elements.Add("Navigation Bar Dot Item", ASPxImageSliderElementsCss.NavigationBarDotItem);
			elements.Add("Navigation Bar Previous Page Button Image", ASPxImageSliderElementsCss.NavigationBarPrevButton);
			elements.Add("Navigation Bar Next Page Button Image", ASPxImageSliderElementsCss.NavigationBarNextButton);
			elements.Add("Play Button Image", ASPxImageSliderElementsCss.PlayButtonImage);
			elements.Add("Pause Button Image", ASPxImageSliderElementsCss.PauseButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateImageZoomElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxImageZoomElementsCss.Control);
			elements.Add("Lens", ASPxImageZoomElementsCss.Lens);
			elements.Add("Hint", ASPxImageZoomElementsCss.Hint);
			elements.Add("Hint Image", ASPxImageZoomElementsCss.HintImage);
			elements.Add("Expand Window Close Button", ASPxImageZoomElementsCss.CloseButton);
			elements.Add("Expand Window Close Button Image", ASPxImageZoomElementsCss.CloseButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateImageZoomNavigatorElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxImageZoomNavigatorElementsCss.Control);
			elements.Add("Navigation Bar Item", ASPxImageZoomNavigatorElementsCss.NavigationBarItem);
			elements.Add("Navigation Bar Selected Item", ASPxImageZoomNavigatorElementsCss.NavigationBarSelectedItem);
			elements.Add("Navigation Bar Prev Button Image", ASPxImageZoomNavigatorElementsCss.NavigationBarPrevButton);
			elements.Add("Navigation Bar Next Button Image", ASPxImageZoomNavigatorElementsCss.NavigationBarNextButton);
			return elements;
		}
		static Dictionary<string, string> CreateLoadingPanelElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Loading Div", ASPxLoadingPanelElementsCss.LoadingDiv);
			elements.Add("Loading Panel", ASPxLoadingPanelElementsCss.LoadingPanel);
			return elements;
		}
		static Dictionary<string, string> CreatePopupMenuElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxPopupMenuElementsCss.Control);
			elements.Add("Item", ASPxPopupMenuElementsCss.Item);
			elements.Add("PopOut Image", ASPxPopupMenuElementsCss.PopOutImage);
			elements.Add("Scroll Up Button Image", ASPxPopupMenuElementsCss.ScrollUpButtonImage);
			elements.Add("Scroll Down Button Image", ASPxPopupMenuElementsCss.ScrollDownButtonImage);
			elements.Add("Checked Image", ASPxPopupMenuElementsCss.CheckedImage);
			return elements;
		}
		static Dictionary<string, string> CreateMenuElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxMenuElementsCss.Control);
			elements.Add("Root Menu", ASPxMenuElementsCss.RootMenu);
			elements.Add("Root Item", ASPxMenuElementsCss.RootItem);
			elements.Add("Sub Menu", ASPxPopupMenuElementsCss.Control);
			elements.Add("Sub Item", ASPxPopupMenuElementsCss.Item);
			elements.Add("PopOut Image", ASPxPopupMenuElementsCss.PopOutImage);
			elements.Add("Scroll Up Button Image", ASPxPopupMenuElementsCss.ScrollUpButtonImage);
			elements.Add("Scroll Down Button Image", ASPxPopupMenuElementsCss.ScrollDownButtonImage);
			elements.Add("Checked Image", ASPxPopupMenuElementsCss.CheckedImage);
			return elements;
		}
		static Dictionary<string, string> CreateNewsControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxNewsControlElementsCss.Control);
			elements.Add("Pager Panel", ASPxNewsControlElementsCss.PagerPanel);
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			elements.Add("Content", ASPxNewsControlElementsCss.Content);
			elements.Add("Item", ASPxHeadlineElementsCss.Control);
			elements.Add("Item Header", ASPxHeadlineElementsCss.Header);
			elements.Add("Item Date", ASPxHeadlineElementsCss.Date);
			elements.Add("Item Content", ASPxHeadlineElementsCss.Content);
			elements.Add("Item Tail", ASPxHeadlineElementsCss.Tail);
			elements.Add("Back To Top Panel", ASPxNewsControlElementsCss.BackToTopPanel);
			elements.Add("Back To Top Image", ASPxNewsControlElementsCss.BackToTopImage);
			return elements;
		}
		static Dictionary<string, string> CreateTabControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTabControlElementsCss.Control);
			elements.Add("Tab Panel", ASPxTabControlElementsCss.TabPanel);
			elements.Add("Tab", ASPxTabControlElementsCss.Tab);
			elements.Add("Active Tab", ASPxTabControlElementsCss.ActiveTab);
			elements.Add("Scroll Button", ASPxTabControlElementsCss.ScrollButton);
			elements.Add("Scroll Left Button Image", ASPxTabControlElementsCss.ScrollLeftButtonImage);
			elements.Add("Scroll Right Button Image", ASPxTabControlElementsCss.ScrollRightButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreatePageControlElementsMap() {
			Dictionary<string, string> elements = ASPxperienceProduct.CreateTabControlElementsMap();
			elements.Add("Content", ASPxPageControlElementsCss.Content);
			return elements;
		}
		static Dictionary<string, string> CreatePagerElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxPagerElementsCss.Control);
			elements.Add("Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Button", ASPxPagerElementsCss.Button);
			elements.Add("First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			return elements;
		}
		static Dictionary<string, string> CreatePopupControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxPopupControlElementsCss.Control);
			elements.Add("Header", ASPxPopupControlElementsCss.Header);
			elements.Add("CloseButton Image", ASPxPopupControlElementsCss.CloseButtonImage);
			elements.Add("Content", ASPxPopupControlElementsCss.Content);
			elements.Add("Footer", ASPxPopupControlElementsCss.Footer);
			elements.Add("SizeGrip Image", ASPxPopupControlElementsCss.SizeGripImage);
			return elements;
		}
		static Dictionary<string, string> CreateDockPanelElementsMap() {
			Dictionary<string, string> elements = ASPxperienceProduct.CreatePopupControlElementsMap();
			elements["Control"] = ASPxDockPanelElementsCss.Control;
			return elements;
		}
		static Dictionary<string, string> CreateRoundPanelElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxRoundPanelElementsCss.Control);
			elements.Add("Header", ASPxRoundPanelElementsCss.Header);
			elements.Add("Content", ASPxRoundPanelElementsCss.Content);
			elements.Add("Collapse Button Image", ASPxRoundPanelElementsCss.CollapseButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateSiteMapControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxSiteMapElementsCss.Control);
			elements.Add("Node (Root Level)", ASPxSiteMapElementsCss.RootLevelNode);
			elements.Add("Node (Level 1)", ASPxSiteMapElementsCss.FirstLevelNode);
			elements.Add("Node (Level 2)", ASPxSiteMapElementsCss.SecondLevelNode);
			elements.Add("Node (Level 3)", ASPxSiteMapElementsCss.ThirdLevelNode);
			elements.Add("Node (Level 4)", ASPxSiteMapElementsCss.FourthLevelNode);
			elements.Add("Node (Other)", ASPxSiteMapElementsCss.OtherLevelNode);
			elements.Add("Butllet Image", ASPxSiteMapElementsCss.BulletImage);
			return elements;
		}
		static Dictionary<string, string> CreateSplitterElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxSplitterElementsCss.Control);
			elements.Add("Pane", ASPxSplitterElementsCss.Pane);
			elements.Add("Vertical Separator", ASPxSplitterElementsCss.VerticalSeparator);
			elements.Add("Horizontal Separator", ASPxSplitterElementsCss.HorizontalSeparator);
			elements.Add("Vertical Collapse Forward Button Image", ASPxSplitterElementsCss.VerticalCollapseForwardButtonImage);
			elements.Add("Vertical Collapse Backward Button Image", ASPxSplitterElementsCss.VerticalCollapseBackwardButtonImage);
			elements.Add("Vertical Separator Button Image", ASPxSplitterElementsCss.VerticalSeparatorButtonImage);
			elements.Add("Horizontal Collapse Forward Button Image", ASPxSplitterElementsCss.HorizontalCollapseForwardButtonImage);
			elements.Add("Horizontal Collapse Backward Button Image", ASPxSplitterElementsCss.HorizontalCollapseBackwardButtonImage);
			elements.Add("Horizontal Separator Button Image", ASPxSplitterElementsCss.HorizontalSeparatorButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateTitleIndexElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTitleIndexElementsCss.Control);
			elements.Add("Index Panel", ASPxTitleIndexElementsCss.IndexPanel);
			elements.Add("Index Panel Item", ASPxTitleIndexElementsCss.IndexPanelItem);
			elements.Add("Filter Panel", ASPxTitleIndexElementsCss.FilterPanel);
			elements.Add("Filter Input", ASPxTitleIndexElementsCss.FilterInput);
			elements.Add("Filter Panel Info", ASPxTitleIndexElementsCss.FilterPanelInfo);
			elements.Add("Group Header", ASPxTitleIndexElementsCss.GroupHeader);
			elements.Add("Group Header Text", ASPxTitleIndexElementsCss.GroupHeaderText);
			elements.Add("Item", ASPxTitleIndexElementsCss.Item);
			elements.Add("Back To Top Panel", ASPxTitleIndexElementsCss.BackToTopPanel);
			elements.Add("Back To Top Image", ASPxTitleIndexElementsCss.BackToTopImage);
			return elements;
		}
		static Dictionary<string, string> CreateTreeViewElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTreeViewElementsCss.Control);
			elements.Add("Node", ASPxTreeViewElementsCss.Node);
			elements.Add("Node Text", ASPxTreeViewElementsCss.NodeText);
			elements.Add("Expand Button Image", ASPxTreeViewElementsCss.ExpandButtonImage);
			elements.Add("Collapse Button Image", ASPxTreeViewElementsCss.CollapseButtonImage);
			elements.Add("Checked Node Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Unchecked Node Image", ASPxCheckBoxElementsCss.UncheckedImage);
			elements.Add("Grayed Node Image", ASPxCheckBoxElementsCss.GrayedImage);
			return elements;
		}
		static Dictionary<string, string> CreateUploadControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxUploadControlElementsCss.Control);
			elements.Add("Path Text Box", ASPxUploadControlElementsCss.PathTextBox);
			elements.Add("Clear File Selection Image", ASPxUploadControlElementsCss.ClearFileSelectionImage);
			elements.Add("Browse Button", ASPxUploadControlElementsCss.BrowseButton);
			elements.Add("Button", ASPxUploadControlElementsCss.Button);
			elements.Add("Error Frame", ASPxUploadControlElementsCss.ErrorFrame);
			return elements;
		}
		static Dictionary<string, string> CreateRibbonElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("File Tab", ASPxRibbonElementsCss.FileTab);
			elements.Add("Active Tab", ASPxRibbonElementsCss.ActiveTab);
			elements.Add("Inactive Tab", ASPxRibbonElementsCss.InactiveTab);
			elements.Add("Tab Content", ASPxRibbonElementsCss.TabContent);
			elements.Add("Group", ASPxRibbonElementsCss.Group);
			elements.Add("Group Label", ASPxRibbonElementsCss.GroupLabel);
			elements.Add("Group Separator", ASPxRibbonElementsCss.GroupSeparator);
			elements.Add("Small Text Item", ASPxRibbonElementsCss.SmallTextItem);
			elements.Add("Small Item", ASPxRibbonElementsCss.SmallItem);
			elements.Add("Large Item", ASPxRibbonElementsCss.LargeItem);
			elements.Add("Minimize Button", ASPxRibbonElementsCss.MinimizeButton);
			return elements;
		}
		public override string Name {
			get { return ThemedProducts.ASPxperienceName; }
		}
		public override string Title {
			get { return "Navigation and Layout Controls"; }
		}
		public override string[] Folders {
			get { return new string[] { "Web" }; }
		}
		public override string[] SkinFiles {
			get {
				return new string[] { "ASPxCallbackPanel.skin", "ASPxCloudControl.skin", "ASPxDataView.skin", "ASPxHeadline.skin", "ASPxLoadingPanel.skin", 
					"ASPxMenu.skin", "ASPxNavBar.skin", "ASPxNewsControl.skin", "ASPxPanel.skin", "ASPxPageControl.skin", "ASPxPager.skin", "ASPxPopupControl.skin", "ASPxPopupMenu.skin",
					"ASPxRatingControl.skin", "ASPxRoundPanel.skin", "ASPxSiteMapControl.skin", "ASPxSplitter.skin", "ASPxTabControl.skin", "ASPxTitleIndex.skin", "ASPxTreeView.skin", "ASPxUploadControl.skin", 
					"ASPxFileManager.skin", "ASPxDockPanel.skin", "ASPxImageSlider.skin", "ASPxFormLayout.skin", "ASPxImageGallery.skin", "ASPxRibbon.skin", "ASPxImageZoom.skin", "ASPxImageZoomNavigator.skin" };
			}
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ASPxperienceProduct.ControlElementsMapCollection;
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[0]; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyWeb; }
		}
		public override bool AllowCustomizeCssFiles(string controlName) {
			return controlName != "ASPxRatingControl";
		}
	}
	public class ASPxEditorsProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxEditorsProduct() {
			ControlElementsMapCollection.Add("ASPxBinaryImage", ASPxEditorsProduct.CreateImageElementsMap());
			ControlElementsMapCollection.Add("ASPxButton", ASPxEditorsProduct.CreateButtonElementsMap());
			ControlElementsMapCollection.Add("ASPxCalendar", ASPxEditorsProduct.CreateCalendarElementsMap());
			ControlElementsMapCollection.Add("ASPxButtonEdit", ASPxEditorsProduct.CreateButtonEditElementsMap());
			ControlElementsMapCollection.Add("ASPxCaptcha", ASPxEditorsProduct.CreateCaptchaElementsMap());
			ControlElementsMapCollection.Add("ASPxCheckBox", ASPxEditorsProduct.CreateCheckBoxElementsMap());
			ControlElementsMapCollection.Add("ASPxCheckBoxList", ASPxEditorsProduct.CreateCheckBoxListElementsMap());
			ControlElementsMapCollection.Add("ASPxColorEdit", ASPxEditorsProduct.CreateColorEditElementsMap());
			ControlElementsMapCollection.Add("ASPxComboBox", ASPxEditorsProduct.CreateComboBoxElementsMap());
			ControlElementsMapCollection.Add("ASPxDateEdit", ASPxEditorsProduct.CreateDateEditElementsMap());
			ControlElementsMapCollection.Add("ASPxDropDownEdit", ASPxEditorsProduct.CreateDropDownEditElementsMap());
			ControlElementsMapCollection.Add("ASPxFilterControl", ASPxEditorsProduct.CreateFilterControlElementsMap());
			ControlElementsMapCollection.Add("ASPxHyperLink", ASPxEditorsProduct.CreateHyperLinkElementsMap());
			ControlElementsMapCollection.Add("ASPxImage", ASPxEditorsProduct.CreateImageElementsMap());
			ControlElementsMapCollection.Add("ASPxLabel", ASPxEditorsProduct.CreateLabelElementsMap());
			ControlElementsMapCollection.Add("ASPxListBox", ASPxEditorsProduct.CreateListBoxElementsMap());
			ControlElementsMapCollection.Add("ASPxMemo", ASPxEditorsProduct.CreateMemoElementsMap());
			ControlElementsMapCollection.Add("ASPxProgressBar", ASPxEditorsProduct.CreateProgressBarElementsMap());
			ControlElementsMapCollection.Add("ASPxRadioButton", ASPxEditorsProduct.CreateRadioButtonElementsMap());
			ControlElementsMapCollection.Add("ASPxRadioButtonList", ASPxEditorsProduct.CreateRadioButtonListElementsMap());
			ControlElementsMapCollection.Add("ASPxSpinEdit", ASPxEditorsProduct.CreateSpinEditElementsMap());
			ControlElementsMapCollection.Add("ASPxTextBox", ASPxEditorsProduct.CreateTextBoxElementsMap());
			ControlElementsMapCollection.Add("ASPxTimeEdit", ASPxEditorsProduct.CreateTimeEditElementsMap());
			ControlElementsMapCollection.Add("ASPxTokenBox", ASPxEditorsProduct.CreateTokenBoxElementsMap());
			ControlElementsMapCollection.Add("ASPxTrackBar", ASPxEditorsProduct.CreateTrackBarElementsMap());
			ControlElementsMapCollection.Add("ASPxValidationSummary", ASPxEditorsProduct.CreateValidationSummaryElementsMap());
		}
		static Dictionary<string, string> CreateImageElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxImageElementsCss.Control);
			elements.Add("Caption", ASPxImageElementsCss.Caption);
			return elements;
		}
		static Dictionary<string, string> CreateButtonElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxButtonElementsCss.Control);
			return elements;
		}
		static Dictionary<string, string> CreateButtonEditElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxButtonEditElementsCss.Control);
			elements.Add("Edit Area", ASPxTextBoxElementsCss.EditArea);
			elements.Add("Button", ASPxButtonEditElementsCss.Button);
			elements.Add("Ellipsis Image", ASPxButtonEditElementsCss.EllipsisImage);		   
			return elements;
		}
		static Dictionary<string, string> CreateCalendarElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCalendarElementsCss.Control);
			elements.Add("Header", ASPxCalendarElementsCss.Header);
			elements.Add("Previous Year Button Image", ASPxCalendarElementsCss.PreviousYearButtonImage);
			elements.Add("Next Year Button Image", ASPxCalendarElementsCss.NextYearButtonImage);
			elements.Add("Previous Month Button Image", ASPxCalendarElementsCss.PreviousMonthButtonImage);
			elements.Add("Next Month Button Image", ASPxCalendarElementsCss.NextMonthButtonImage);
			elements.Add("Day Header", ASPxCalendarElementsCss.DayHeader);
			elements.Add("Week Number", ASPxCalendarElementsCss.WeekNumber);
			elements.Add("Day", ASPxCalendarElementsCss.Day);
			elements.Add("Today", ASPxCalendarElementsCss.Today);
			elements.Add("Selected Day", ASPxCalendarElementsCss.SelectedDay);
			elements.Add("Other day", ASPxCalendarElementsCss.OtherDay);
			elements.Add("Weekend", ASPxCalendarElementsCss.Weekend);
			elements.Add("Footer", ASPxCalendarElementsCss.Footer);
			elements.Add("Button", ASPxCalendarElementsCss.Button);
			elements.Add("Fast Navigation Content", ASPxCalendarElementsCss.FastNavigationContent);
			elements.Add("Fast Navigation Footer", ASPxCalendarElementsCss.FastNavigationFooter);
			elements.Add("Fast Navigation Month Area", ASPxCalendarElementsCss.FastNavigationMonthArea);
			elements.Add("Fast Navigation Year Area", ASPxCalendarElementsCss.FastNavigationYearArea);
			elements.Add("Fast Navigation Month", ASPxCalendarElementsCss.FastNavigationMonth);
			elements.Add("Fast Navigation Year", ASPxCalendarElementsCss.FastNavigationYear);
			elements.Add("Fast Navigation Previous Year Button Image", ASPxCalendarElementsCss.FastNavigationPrevYearButtonImage);
			elements.Add("Fast Navigation Next Year Button Image", ASPxCalendarElementsCss.FastNavigationNextYearButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateCaptchaElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCaptchaElementsCss.Control);
			elements.Add("Text Box Label", ASPxCaptchaElementsCss.TextBoxLabel);
			elements.Add("Text Box", ASPxCaptchaElementsCss.TextBox);
			elements.Add("Refresh Button Image", ASPxCaptchaElementsCss.RefreshButtonImage);
			elements.Add("Refresh Button Text", ASPxCaptchaElementsCss.RefreshButtonText);
			return elements;
		}
		static Dictionary<string, string> CreateCheckBoxElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCheckBoxElementsCss.Control);
			elements.Add("Checked Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Unchecked Image", ASPxCheckBoxElementsCss.UncheckedImage);
			elements.Add("Grayed Image", ASPxCheckBoxElementsCss.GrayedImage);
			return elements;
		}
		static Dictionary<string, string> CreateCheckBoxListElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCheckBoxListElementsCss.Control);
			elements.Add("Checked Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Unchecked Image", ASPxCheckBoxElementsCss.UncheckedImage);
			return elements;
		}
		static Dictionary<string, string> CreateDropDownEditBasisElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxDropDownEditElementsCss.Control);
			elements.Add("Edit Area", ASPxTextBoxElementsCss.EditArea);
			elements.Add("Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateColorEditElementsMap() {
			Dictionary<string, string> elements = ASPxEditorsProduct.CreateDropDownEditBasisElementsMap();
			elements.Add("Color Table", ASPxColorEditElementsCss.ColorTable);
			elements.Add("Color Cell", ASPxColorEditElementsCss.ColorCell);
			elements.Add("Color Div", ASPxColorEditElementsCss.ColorCellDiv);
			return elements;
		}
		static Dictionary<string, string> CreateComboBoxElementsMap() {
			Dictionary<string, string> elements = ASPxEditorsProduct.CreateDropDownEditBasisElementsMap();
			elements.Add("List Box", ASPxComboBoxElementsCss.ListBox);
			Dictionary<string, string> listBoxElementsMap = ASPxEditorsProduct.CreateListBoxElementsMap();
			listBoxElementsMap.Remove("Control");
			foreach(string key in listBoxElementsMap.Keys)
				elements.Add(key, listBoxElementsMap[key]);
			return elements;
		}
		static Dictionary<string, string> CreateDateEditElementsMap() {
			Dictionary<string, string> elements = ASPxEditorsProduct.CreateDropDownEditBasisElementsMap();
			elements.Add("Calendar", ASPxDateEditElementsCss.Calendar);
			Dictionary<string, string> calendarElementsMap = ASPxEditorsProduct.CreateCalendarElementsMap();
			calendarElementsMap.Remove("Control");
			foreach(string key in calendarElementsMap.Keys)
				elements.Add(key, calendarElementsMap[key]);
			return elements;
		}
		static Dictionary<string, string> CreateDropDownEditElementsMap() {
			Dictionary<string, string> elements = ASPxEditorsProduct.CreateDropDownEditBasisElementsMap();
			elements.Add("Drop Down Window", ASPxDropDownEditElementsCss.DropDownWindow);
			return elements;
		}
		static Dictionary<string, string> CreateFilterControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxFilterControlElementsCss.Control);
			elements.Add("Group Type", ASPxFilterControlElementsCss.GroupType);
			elements.Add("Add Group Image", ASPxFilterControlElementsCss.AddGroupImage);
			elements.Add("Property Name", ASPxFilterControlElementsCss.PropertyName);
			elements.Add("Operator", ASPxFilterControlElementsCss.Operator);
			elements.Add("Value", ASPxFilterControlElementsCss.Value);
			elements.Add("Remove Button Image", ASPxFilterControlElementsCss.RemoveButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateHyperLinkElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxHyperLinkElementsCss.Control);
			return elements;
		}
		static Dictionary<string, string> CreateLabelElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxLabelElementsCss.Control);
			return elements;
		}
		static Dictionary<string, string> CreateListBoxElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxListBoxElementsCss.Control);
			elements.Add("Item", ASPxListBoxElementsCss.Item);
			return elements;
		}
		static Dictionary<string, string> CreateMemoElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxMemoElementsCss.Control);
			elements.Add("Edit Area", ASPxMemoElementsCss.EditArea);
			return elements;
		}
		static Dictionary<string, string> CreateProgressBarElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxProgressBarElementsCss.Control);
			elements.Add("Indicator", ASPxProgressBarElementsCss.Indicator);
			return elements;
		}
		static Dictionary<string, string> CreateRadioButtonElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxRadioButtonElementsCss.Control);
			elements.Add("Checked Image", ASPxRadioButtonElementsCss.CheckedImage);
			elements.Add("Unchecked Image", ASPxRadioButtonElementsCss.UncheckedImage);
			return elements;
		}
		static Dictionary<string, string> CreateRadioButtonListElementsMap() {
			Dictionary<string, string> elements = ASPxEditorsProduct.CreateRadioButtonElementsMap();
			elements["Control"] = ASPxRadioButtonListElementsCss.Control;
			return elements;
		}
		static Dictionary<string, string> CreateSpinEditElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxSpinEditEditElementsCss.Control);
			elements.Add("Edit Area", ASPxTextBoxElementsCss.EditArea);
			elements.Add("Large Increment Button", ASPxSpinEditEditElementsCss.LargeIncrementButton);
			elements.Add("Large Increment Image", ASPxSpinEditEditElementsCss.LargeIncrementImage);
			elements.Add("Large Decrement Button", ASPxSpinEditEditElementsCss.LargeDecrementButton);
			elements.Add("Large Decrement Image", ASPxSpinEditEditElementsCss.LargeDecrementImage);
			elements.Add("Increment Button", ASPxSpinEditEditElementsCss.IncrementButton);
			elements.Add("Increment Image", ASPxSpinEditEditElementsCss.IncrementImage);
			elements.Add("Decrement Button", ASPxSpinEditEditElementsCss.DecrementButton);
			elements.Add("Decrement Image", ASPxSpinEditEditElementsCss.DecrementImage);
			return elements;
		}
		static Dictionary<string, string> CreateTextBoxElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTextBoxElementsCss.Control);
			elements.Add("Edit Area", ASPxTextBoxElementsCss.EditArea);
			return elements;
		}
		static Dictionary<string, string> CreateTimeEditElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxSpinEditEditElementsCss.Control);
			elements.Add("Edit Area", ASPxTextBoxElementsCss.EditArea);
			elements.Add("Increment Button", ASPxSpinEditEditElementsCss.IncrementButton);
			elements.Add("Increment Image", ASPxSpinEditEditElementsCss.IncrementImage);
			elements.Add("Decrement Button", ASPxSpinEditEditElementsCss.DecrementButton);
			elements.Add("Decrement Image", ASPxSpinEditEditElementsCss.DecrementImage);
			return elements;
		}
		static Dictionary<string, string> CreateTokenBoxElementsMap() {
			Dictionary<string, string> elements = ASPxEditorsProduct.CreateComboBoxElementsMap();
			elements.Remove("Drop Down Button");
			elements.Remove("Drop Down Button Image");
			elements.Add("Token", ASPxTokenBoxElementsCss.Token);
			elements.Add("TokenText", ASPxTokenBoxElementsCss.TokenText);
			elements.Add("TokenRemoveButton", ASPxTokenBoxElementsCss.TokenRemoveButton);
			elements.Add("Input", ASPxTokenBoxElementsCss.Input);
			return elements;
		}
		static Dictionary<string, string> CreateTrackBarElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTrackBarElementsCss.Control);
			elements.Add("Increment Button", ASPxTrackBarElementsCss.IncrementButton);
			elements.Add("Decrement Button", ASPxTrackBarElementsCss.DecrementButton);
			elements.Add("Large Tick", ASPxTrackBarElementsCss.LargeTick);
			elements.Add("Small Tick", ASPxTrackBarElementsCss.SmallTick);
			elements.Add("Selected Tick", ASPxTrackBarElementsCss.SelectedTick);
			elements.Add("Item", ASPxTrackBarElementsCss.Item);
			elements.Add("Label", ASPxTrackBarElementsCss.Label);
			elements.Add("Track", ASPxTrackBarElementsCss.Track);
			elements.Add("Bar Highlight", ASPxTrackBarElementsCss.BarHighlight);
			elements.Add("Main Drag Handle", ASPxTrackBarElementsCss.MainDragHandler);
			elements.Add("Secondary Drag Handle", ASPxTrackBarElementsCss.SecondaryDragHandler);
			elements.Add("Value Tooltip", ASPxTrackBarElementsCss.ValueTooltip);
			return elements;
		}
		static Dictionary<string, string> CreateValidationSummaryElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxValidationSummaryElementsCss.Control);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ASPxEditorsProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxEditorsName; }
		}
		public override string Title {
			get { return "Data Editors"; }
		}
		public override string[] Folders {
			get { return new string[] { "Editors" }; }
		}
		public override string[] SkinFiles {
			get {
				return new string[] { "ASPxBinaryImage.skin", "ASPxButton.skin", "ASPxButtonEdit.skin", "ASPxCalendar.skin", "ASPxCaptcha.skin", "ASPxCheckBox.skin", "ASPxColorEdit.skin", 
					"ASPxComboBox.skin", "ASPxDateEdit.skin", "ASPxDropDownEdit.skin", "ASPxFilterControl.skin", "ASPxHyperLink.skin", "ASPxImage.skin", "ASPxLabel.skin", 
					"ASPxListBox.skin", "ASPxMemo.skin", "ASPxProgressBar.skin", "ASPxRadioButton.skin", "ASPxRadioButtonList.skin", "ASPxSpinEdit.skin", 
					"ASPxTextBox.skin", "ASPxTimeEdit.skin", "ASPxTokenBox.skin", "ASPxTrackBar.skin", "ASPxCheckBoxList.skin", "ASPxValidationSummary.skin"  };
			}
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyWeb; }
		}
	}
	public class ASPxGridViewProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxGridViewProduct() {
			ControlElementsMapCollection.Add("ASPxGridView", ASPxGridViewProduct.CreateGridViewElementsMap());
			ControlElementsMapCollection.Add("ASPxGridLookup", ASPxGridViewProduct.CreateGridLookupElementsMap());
			ControlElementsMapCollection.Add("ASPxCardView", ASPxGridViewProduct.CreateCardViewElementsMap());
		}
		static Dictionary<string, string> CreateCardViewElementsMap() {
			var elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCardViewElementsCss.Control);
			elements.Add("Title Panel", ASPxCardViewElementsCss.TitlePanel);
			elements.Add("Header Panel", ASPxCardViewElementsCss.HeaderPanel);
			elements.Add("Command Item", ASPxCardViewElementsCss.CommandItem);
			elements.Add("Card", ASPxCardViewElementsCss.Card);
			elements.Add("Focused Card", ASPxCardViewElementsCss.FocusedCard);
			elements.Add("Selected Card", ASPxCardViewElementsCss.SelectedCard);
			elements.Add("Selected Card Checked Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Selected Card Unchecked Image", ASPxCheckBoxElementsCss.UncheckedImage);
			elements.Add("Filter Bar", ASPxCardViewElementsCss.FilterBar);
			elements.Add("Filter Bar Expression", ASPxCardViewElementsCss.FilterBarExpression);
			elements.Add("Summary Panel", ASPxCardViewElementsCss.SummaryPanel);
			elements.Add("Summary Item", ASPxCardViewElementsCss.SummaryItem);
			elements.Add("Pager Panel", ASPxCardViewElementsCss.PagerPanel);
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			return elements;
		}
		static Dictionary<string, string> CreateGridViewElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxGridViewElementsCss.Control);
			elements.Add("Adaptive Header Panel", ASPxGridViewElementsCss.AdaptiveHeaderPanel);
			elements.Add("Title Panel", ASPxGridViewElementsCss.TitlePanel);
			elements.Add("Group Panel", ASPxGridViewElementsCss.GroupPanel);
			elements.Add("Group Header", ASPxGridViewElementsCss.GroupHeader);
			elements.Add("Group Header Sort Up Image", ASPxGridViewElementsCss.GroupHeaderSortUpImage);
			elements.Add("Group Header Sort Down Image", ASPxGridViewElementsCss.GroupHeaderSortDownImage);
			elements.Add("Group Header Filter Image", ASPxGridViewElementsCss.GroupHeaderFilterImage);
			elements.Add("Group Footer", ASPxGridViewElementsCss.GroupFooter);
			elements.Add("Filter Row", ASPxGridViewElementsCss.FilterRow);
			elements.Add("Filter Row Button Image", ASPxGridViewElementsCss.FilterRowButtonImage);
			elements.Add("Group Row", ASPxGridViewElementsCss.GroupRow);
			elements.Add("Expanded Button Image", ASPxGridViewElementsCss.ExpandedButtonImage);
			elements.Add("Collapsed Button Image", ASPxGridViewElementsCss.CollapsedButtonImage);
			elements.Add("Command Column Item", ASPxGridViewElementsCss.CommandColumnItem);
			elements.Add("Data Row", ASPxGridViewElementsCss.DataRow);
			elements.Add("Focused Row", ASPxGridViewElementsCss.FocusedRow);
			elements.Add("Selected Row", ASPxGridViewElementsCss.SelectedRow);
			elements.Add("Selected Row Checked Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Selected Row Unchecked Image", ASPxCheckBoxElementsCss.UncheckedImage);
			elements.Add("Preview Row", ASPxGridViewElementsCss.PreviewRow);
			elements.Add("Footer", ASPxGridViewElementsCss.Footer);
			elements.Add("Filter Bar", ASPxGridViewElementsCss.FilterBar);
			elements.Add("Filter Bar Expression", ASPxGridViewElementsCss.FilterBarExpression);
			elements.Add("Pager Panel", ASPxGridViewElementsCss.PagerPanel); 
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			return elements;
		}
		static Dictionary<string, string> CreateGridLookupElementsMap() {
			Dictionary<string, string> elements = ASPxGridViewProduct.CreateGridViewElementsMap();
			elements["Control"] = ASPxButtonEditElementsCss.Control;
			elements.Add("Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("Drop Down Window", ASPxDropDownEditElementsCss.DropDownWindow);
			elements.Add("Grid View", ASPxGridViewElementsCss.Control);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxGridViewName; }
		}
		public override string Title {
			get { return "GridView"; }
		}
		public override string[] Folders {
			get { return new string[] { "GridView", "CardView" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxGridView.skin", "ASPxGridLookup.skin", "ASPxCardView.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyWeb; }
		}
	}
	public class ASPxTreeListProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxTreeListProduct() {
			ControlElementsMapCollection.Add("ASPxTreeList", ASPxTreeListProduct.CreateTreeListElementsMap());
		}
		static Dictionary<string, string> CreateTreeListElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTreeListElementsCss.Control);
			elements.Add("Header", ASPxTreeListElementsCss.Header);
			elements.Add("Node", ASPxTreeListElementsCss.Node);
			elements.Add("Selected Node", ASPxTreeListElementsCss.SelectedNode);
			elements.Add("Selected Node Checked Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Selected Node Unchecked Image", ASPxCheckBoxElementsCss.UncheckedImage);
			elements.Add("Focused Node", ASPxTreeListElementsCss.FocusedNode);
			elements.Add("Expanded Button Image", ASPxTreeListElementsCss.ExpandedButtonImage);
			elements.Add("Collapsed Button Image", ASPxTreeListElementsCss.CollapsedButtonImage);
			elements.Add("Preview", ASPxTreeListElementsCss.Preview);
			elements.Add("Group Footer", ASPxTreeListElementsCss.GroupFooter);
			elements.Add("Footer", ASPxTreeListElementsCss.Footer);
			elements.Add("Pager Panel", ASPxTreeListElementsCss.PagerPanel);
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxTreeListName; }
		}
		public override string Title {
			get { return "TreeList"; }
		}
		public override string[] Folders {
			get { return new string[] { "TreeList" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxTreeList.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyTreeListWeb; }
		}
	}
	public class ASPxHtmlEditorProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxHtmlEditorProduct() {
			ControlElementsMapCollection.Add("ASPxHtmlEditor", ASPxHtmlEditorProduct.CreateHtmlEditorElementsMap());
		}
		static Dictionary<string, string> CreateHtmlEditorElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxHtmlEditorElementsCss.Control);
			elements.Add("Toolbar", ASPxHtmlEditorElementsCss.Toolbar);
			elements.Add("Toolbar Menu", ASPxMenuElementsCss.RootMenu);
			elements.Add("Toolbar Item", ASPxMenuElementsCss.RootItem);
			elements.Add("Toolbar Item PopOut Image", ASPxHtmlEditorElementsCss.ToolbarItemPopOutImage);
			elements.Add("Toolbar Drop Down", ASPxDropDownEditElementsCss.Control);
			elements.Add("Toolbar Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Toolbar Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("DesignView Area", ASPxHtmlEditorElementsCss.DesignViewArea);
			elements.Add("Status Bar", ASPxHtmlEditorElementsCss.StatusBar);
			elements.Add("Status Bar Tab", ASPxHtmlEditorElementsCss.StatusBarTab);
			elements.Add("Status Active Bar Tab", ASPxHtmlEditorElementsCss.StatusBarActiveTab);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxHtmlEditorName; }
		}
		public override string Title {
			get { return "HtmlEditor"; }
		}
		public override string[] Folders {
			get { return new string[] { "HtmlEditor" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxHtmlEditor.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors, ThemedProducts.ASPxSpellChecker }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyHtmlEditorWeb; }
		}
	}
	public class ASPxSpellCheckerProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxSpellCheckerProduct() {
			ControlElementsMapCollection.Add("ASPxSpellChecker", ASPxSpellCheckerProduct.CreateSpellCheckerElementsMap());
		}
		static Dictionary<string, string> CreateSpellCheckerElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Error Word", ASPxSpellCheckerElementsCss.ErrorWord);
			elements.Add("Popup Window", ASPxPopupControlElementsCss.Control);
			elements.Add("Popup Window Header", ASPxPopupControlElementsCss.Header);
			elements.Add("Popup Window Close Button Image", ASPxPopupControlElementsCss.CloseButtonImage);
			elements.Add("Popup Window Content", ASPxPopupControlElementsCss.Content);
			elements.Add("Button", ASPxButtonElementsCss.Control);
			elements.Add("Text Container", ASPxSpellCheckerElementsCss.TextContainer);
			elements.Add("Text Box", ASPxTextBoxElementsCss.Control);
			elements.Add("List Box", ASPxListBoxElementsCss.Control);
			elements.Add("List Box Item", ASPxListBoxElementsCss.Item);
			elements.Add("Check Box", ASPxCheckBoxElementsCss.Control);
			elements.Add("Check Box Checked Image", ASPxCheckBoxElementsCss.CheckedImage);
			elements.Add("Check Box Unchecked Image", ASPxCheckBoxElementsCss.UncheckedImage);
			elements.Add("Drop Down", ASPxDropDownEditElementsCss.Control);
			elements.Add("Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("Round Panel", ASPxRoundPanelElementsCss.Control);
			elements.Add("Round Panel Header", ASPxRoundPanelElementsCss.Header);
			elements.Add("Round Panel Content", ASPxRoundPanelElementsCss.Content);
			elements.Add("Round Panel Collapse Button Image", ASPxRoundPanelElementsCss.CollapseButtonImage);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ASPxSpellCheckerProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxSpellCheckerName; }
		}
		public override string Title {
			get { return "SpellChecker"; }
		}
		public override string[] Folders {
			get { return new string[] { "SpellChecker" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxSpellChecker.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblySpellCheckerWeb; }
		}
	}
	public class ASPxPivotGridProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxPivotGridProduct() {
			ControlElementsMapCollection.Add("ASPxPivotGrid", ASPxPivotGridProduct.CreatePivotGridElementsMap());
		}
		static Dictionary<string, string> CreatePivotGridElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string,string>();
			elements.Add("Control", ASPxPivotGridElementsCss.Control);
			elements.Add("Filter Area", ASPxPivotGridElementsCss.FilterArea);
			elements.Add("Data Area", ASPxPivotGridElementsCss.DataArea);
			elements.Add("Column Area", ASPxPivotGridElementsCss.ColumnArea);
			elements.Add("Row Area", ASPxPivotGridElementsCss.RowArea);
			elements.Add("Header", ASPxPivotGridElementsCss.Header);
			elements.Add("Header Text", ASPxPivotGridElementsCss.HeaderText);
			elements.Add("Header Filter Button Image", ASPxPivotGridElementsCss.HeaderFilterButtonImage);
			elements.Add("Header Expanded Button Image", ASPxPivotGridElementsCss.HeaderExpandedButtonImage);
			elements.Add("Header Collapsed Button Image", ASPxPivotGridElementsCss.HeaderCollapsedButtonImage);
			elements.Add("Header Sort Up Button Image", ASPxPivotGridElementsCss.HeaderSortUpButtonImage);
			elements.Add("Header Sort Down Button Image", ASPxPivotGridElementsCss.HeaderSortDownButtonImage);
			elements.Add("Header Group Separator Image", ASPxPivotGridElementsCss.GroupSeparatorImage);
			elements.Add("Column Field Value", ASPxPivotGridElementsCss.ColumnFieldValue);
			elements.Add("Column Total Field Value", ASPxPivotGridElementsCss.ColumnTotalFieldValue);
			elements.Add("Column Grand Total Field Value", ASPxPivotGridElementsCss.ColumnGrandTotalFieldValue);
			elements.Add("Row Field Value", ASPxPivotGridElementsCss.RowFieldValue);
			elements.Add("Row Total Field Value", ASPxPivotGridElementsCss.RowTotalFieldValue);
			elements.Add("Row Grand Total Field Value", ASPxPivotGridElementsCss.RowGrandTotalFieldValue);
			elements.Add("Cell", ASPxPivotGridElementsCss.Cell);
			elements.Add("Total Cell", ASPxPivotGridElementsCss.TotalCell);
			elements.Add("Grand Total Cell", ASPxPivotGridElementsCss.GrandTotalCell);
			elements.Add("Prefilter Panel", ASPxPivotGridElementsCss.PrefilterPanel);
			elements.Add("Prefilter Panel Link", ASPxPivotGridElementsCss.PrefilterPanelLink);
			elements.Add("Prefilter Button Image", ASPxPivotGridElementsCss.PrefilterButtonImage);
			elements.Add("Pager Panel", ASPxPivotGridElementsCss.PagerPanel);
			elements.Add("Pager Summary", ASPxPagerElementsCss.Summary);
			elements.Add("Pager Button", ASPxPagerElementsCss.Button);
			elements.Add("Pager First Button Image", ASPxPagerElementsCss.FirstButtonImage);
			elements.Add("Pager Previous Button Image", ASPxPagerElementsCss.PreviousButtonImage);
			elements.Add("Pager Next Button Image", ASPxPagerElementsCss.NextButtonImage);
			elements.Add("Pager Last Button Image", ASPxPagerElementsCss.LastButtonImage);
			elements.Add("Pager All Button Image", ASPxPagerElementsCss.AllButtonImage);
			elements.Add("Pager Current Page Number", ASPxPagerElementsCss.CurrentPageNumber);
			elements.Add("Pager Page Number", ASPxPagerElementsCss.PageNumber);
			elements.Add("Page Size Item", ASPxPagerElementsCss.PageSizeItem);
			elements.Add("Page Size Item Editor", ASPxPagerElementsCss.PageSizeItemEditor);
			elements.Add("Page Size Item Button", ASPxPagerElementsCss.PageSizeItemButton);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxPivotGridName; }
		}
		public override string Title {
			get { return "PivotGrid"; }
		}
		public override string[] Folders {
			get { return new string[] { "PivotGrid" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxPivotGrid.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyASPxPivotGrid; }
		}
	}
	public class ASPxSchedulerProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxSchedulerProduct() {
			ControlElementsMapCollection.Add("ASPxDateNavigator", ASPxSchedulerProduct.CreateDateNavigatorElementsMap());
			ControlElementsMapCollection.Add("MonthEdit", ASPxSchedulerProduct.CreateComboBoxElementsMap());
			ControlElementsMapCollection.Add("RecurrenceTypeEdit", ASPxSchedulerProduct.CreateRecurrenceTypeEditElementsMap());
			ControlElementsMapCollection.Add("WeekDaysEdit", ASPxSchedulerProduct.CreateComboBoxElementsMap());
			ControlElementsMapCollection.Add("WeekOfMonthEdit", ASPxSchedulerProduct.CreateComboBoxElementsMap());
			ControlElementsMapCollection.Add("ASPxScheduler", ASPxSchedulerProduct.CreateSchedulerElementsMap());
		}
		static Dictionary<string, string> CreateDateNavigatorElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxCalendarElementsCss.Control);
			elements.Add("Header", ASPxCalendarElementsCss.Header);
			elements.Add("Previous Year Button Image", ASPxCalendarElementsCss.PreviousYearButtonImage);
			elements.Add("Next Year Button Image", ASPxCalendarElementsCss.NextYearButtonImage);
			elements.Add("Previous Month Button Image", ASPxCalendarElementsCss.PreviousMonthButtonImage);
			elements.Add("Next Month Button Image", ASPxCalendarElementsCss.NextMonthButtonImage);
			elements.Add("Day Header", ASPxCalendarElementsCss.DayHeader);
			elements.Add("Week Number", ASPxCalendarElementsCss.WeekNumber);
			elements.Add("Day", ASPxCalendarElementsCss.Day);
			elements.Add("Today", ASPxCalendarElementsCss.Today);
			elements.Add("Selected Day", ASPxCalendarElementsCss.SelectedDay);
			elements.Add("Other day", ASPxCalendarElementsCss.OtherDay);
			elements.Add("Weekend", ASPxCalendarElementsCss.Weekend);
			elements.Add("Footer", ASPxCalendarElementsCss.Footer);
			elements.Add("Button", ASPxCalendarElementsCss.Button); 
			return elements;
		}
		static Dictionary<string, string> CreateComboBoxElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxDropDownEditElementsCss.Control);
			elements.Add("Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("List Box", ASPxListBoxElementsCss.Control);
			elements.Add("Item", ASPxListBoxElementsCss.Item);
			return elements;
		}
		static Dictionary<string, string> CreateRecurrenceTypeEditElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxRadioButtonListElementsCss.Control);
			elements.Add("Checked Image", ASPxRadioButtonElementsCss.CheckedImage);
			elements.Add("Unchecked Image", ASPxRadioButtonElementsCss.UncheckedImage);
			return elements;
		}
		static Dictionary<string, string> CreateSchedulerElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxSchedulerElementsCss.Control);
			elements.Add("Toolbar Container", ASPxSchedulerElementsCss.ToolbarContainer);
			elements.Add("Toolbar", ASPxSchedulerElementsCss.Toolbar);
			elements.Add("View Navigator Button", ASPxSchedulerElementsCss.ViewNavigatorButton);
			elements.Add("View Navigator Backward Button Image", ASPxSchedulerElementsCss.ViewNavigatorBackwardButtonImage);
			elements.Add("View Navigator Forward Button Image", ASPxSchedulerElementsCss.ViewNavigatorForwardButtonImage);
			elements.Add("View Navigator Go To Date Button", ASPxSchedulerElementsCss.ViewNavigatorGoToDateButton);
			elements.Add("View Navigator Go To Date Button Image", ASPxSchedulerElementsCss.ViewNavigatorGoToDateButtonImage);
			elements.Add("Visible Interval", ASPxSchedulerElementsCss.VisibleInterval);
			elements.Add("View Selector", ASPxSchedulerElementsCss.ViewSelector);
			elements.Add("View Selector Button", ASPxSchedulerElementsCss.ViewSelectorButton);
			elements.Add("Resource Navigator", ASPxSchedulerElementsCss.ResourceNavigator);
			elements.Add("Resource Navigator Button", ASPxSchedulerElementsCss.ResourceNavigatorButton);
			elements.Add("Resource Navigator First Button Image", ASPxSchedulerElementsCss.ResourceNavigatorFirstButtonImage);
			elements.Add("Resource Navigator Previous Page Button Image", ASPxSchedulerElementsCss.ResourceNavigatorPrevPageButtonImage);
			elements.Add("Resource Navigator Previous Button Image", ASPxSchedulerElementsCss.ResourceNavigatorPrevButtonImage);
			elements.Add("Resource Navigator Last Button Image", ASPxSchedulerElementsCss.ResourceNavigatorLastButtonImage);
			elements.Add("Resource Navigator Next Page Button Image", ASPxSchedulerElementsCss.ResourceNavigatorNextPageButtonImage);
			elements.Add("Resource Navigator Next Button Image", ASPxSchedulerElementsCss.ResourceNavigatorNextButtonImage);
			elements.Add("Resource Navigator Increase Button Image", ASPxSchedulerElementsCss.ResourceNavigatorIncreaseButtonImage);
			elements.Add("Resource Navigator Decrease Button Image", ASPxSchedulerElementsCss.ResourceNavigatorDecreaseButtonImage);
			elements.Add("Resource Selector", ASPxDropDownEditElementsCss.Control);
			elements.Add("Resource Selector Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Resource Selector Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("Resource Header", ASPxSchedulerElementsCss.ResourceHeader);
			elements.Add("Time Ruler Hours Item", ASPxSchedulerElementsCss.TimeRulerHoursItem);
			elements.Add("Time Ruler Minute Item", ASPxSchedulerElementsCss.TimeRulerMinuteItem);
			elements.Add("Time Cell", ASPxSchedulerElementsCss.TimeCell);
			elements.Add("Time Marker", ASPxSchedulerElementsCss.TimeMarker);
			elements.Add("Navigation Button", ASPxSchedulerElementsCss.NavigationButton);
			elements.Add("Backward Navigation Button Image", ASPxSchedulerElementsCss.BackwardNavigationButtonImage);
			elements.Add("Forward Navigation Button Image", ASPxSchedulerElementsCss.ForwardNavigationButtonImage);
			elements.Add("Appointment", ASPxSchedulerElementsCss.Appointment);
			elements.Add("Appointment Reminder Image", ASPxSchedulerElementsCss.ReminderImage);
			elements.Add("Recurrence Appointment Image", ASPxSchedulerElementsCss.RecurrenceImage);
			elements.Add("No Recurrence Appointment Image", ASPxSchedulerElementsCss.NoRecurrenceImage);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ASPxSchedulerProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxSchedulerName; }
		}
		public override string Title {
			get { return "Scheduler"; }
		}
		public override string[] Folders {
			get { return new string[] { "Scheduler" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxDateNavigator.skin", "ASPxScheduler.skin", "MonthEdit.skin", "WeekDaysEdit.skin", "WeekOfMonthEdit.skin", "RecurrenceTypeEdit.skin"}; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblySchedulerWeb; }
		}
	}
	public class ASPxSpreadsheetProduct : ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxSpreadsheetProduct() {
			ControlElementsMapCollection.Add("ASPxSpreadsheet", ASPxSpreadsheetProduct.CreateSpreadsheetElementsMap());
		}
		static Dictionary<string, string> CreateSpreadsheetElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxSpreadsheetElementsCss.Control);
			elements.Add("Columns Header", ASPxSpreadsheetElementsCss.ColumnsHeader);
			elements.Add("Rows Header", ASPxSpreadsheetElementsCss.RowsHeader);
			elements.Add("Content Area", ASPxSpreadsheetElementsCss.ContentArea);
			elements.Add("Content Area Vertical Grid Line", ASPxSpreadsheetElementsCss.ContentAreaVerticalGridLine);
			elements.Add("Content Area Horizontal Grid Line", ASPxSpreadsheetElementsCss.ContentAreaHorizontalGridLine);
			elements.Add("Ribbon Active Tab", ASPxRibbonElementsCss.ActiveTab);
			elements.Add("Ribbon Inactive Tab", ASPxRibbonElementsCss.InactiveTab);
			elements.Add("Ribbon Tab Content", ASPxRibbonElementsCss.TabContent);
			elements.Add("Ribbon Group", ASPxRibbonElementsCss.Group);
			elements.Add("Ribbon Group Label", ASPxRibbonElementsCss.GroupLabel);
			elements.Add("Ribbon Small Item", ASPxRibbonElementsCss.SmallItem);
			elements.Add("Ribbon Large Item", ASPxRibbonElementsCss.LargeItem);
			elements.Add("Ribbon Minimize Button", ASPxRibbonElementsCss.MinimizeButton);
			elements.Add("Sheet Tabs", "dxtc-bottom");
			elements.Add("Sheet Tabs Panel", "dxtc-bottom " + ASPxTabControlElementsCss.TabPanel);
			elements.Add("Sheet Tabs Tab", "dxtc-bottom " + ASPxTabControlElementsCss.Tab);
			elements.Add("Sheet Tabs Active Tab", "dxtc-bottom " + ASPxTabControlElementsCss.ActiveTab);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ASPxSpreadsheetProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxSpreadsheetName; }
		}
		public override string Title {
			get { return "Spreadsheet"; }
		}
		public override string[] Folders {
			get { return new string[] { "Spreadsheet" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxSpreadsheet.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyWebSpreadsheet; }
		}
	}
	public class ASPxRichEditProduct : ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static ASPxRichEditProduct() {
			ControlElementsMapCollection.Add("ASPxRichEdit", ASPxRichEditProduct.CreateRichEditElementsMap());
		}
		static Dictionary<string, string> CreateRichEditElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxRichEditElementsCss.Control);
			elements.Add("Ruler", ASPxRichEditElementsCss.Ruler);
			elements.Add("Ruler Major Division", ASPxRichEditElementsCss.RulerMajorDivision);
			elements.Add("Ruler Minor Division", ASPxRichEditElementsCss.RulerMinorDivision);
			elements.Add("View", ASPxRichEditElementsCss.View);
			elements.Add("Status Bar", ASPxRichEditElementsCss.StatusBar);
			elements.Add("Ribbon Active Tab", ASPxRibbonElementsCss.ActiveTab);
			elements.Add("Ribbon Inactive Tab", ASPxRibbonElementsCss.InactiveTab);
			elements.Add("Ribbon Tab Content", ASPxRibbonElementsCss.TabContent);
			elements.Add("Ribbon Group", ASPxRibbonElementsCss.Group);
			elements.Add("Ribbon Group Label", ASPxRibbonElementsCss.GroupLabel);
			elements.Add("Ribbon Small Item", ASPxRibbonElementsCss.SmallItem);
			elements.Add("Ribbon Large Item", ASPxRibbonElementsCss.LargeItem);
			elements.Add("Ribbon Minimize Button", ASPxRibbonElementsCss.MinimizeButton);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return ASPxRichEditProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.ASPxRichEditName; }
		}
		public override string Title {
			get { return "RichEdit"; }
		}
		public override string[] Folders {
			get { return new string[] { "RichEdit" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ASPxRichEdit.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyWebRichEdit; }
		}
	}
	public class XtraReportsProduct : ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static XtraReportsProduct() {
			ControlElementsMapCollection.Add("ASPxDocumentViewer", XtraReportsProduct.CreateReportDocumentViewerElementsMap());
			ControlElementsMapCollection.Add("ReportDocumentMap", XtraReportsProduct.CreateReportDocumentMapElementsMap());
			ControlElementsMapCollection.Add("ReportToolbar", XtraReportsProduct.CreateReportToolbarMapElementsMap());
			ControlElementsMapCollection.Add("ReportParametersPanel", XtraReportsProduct.CreateReportParametersPanelElementsMap());
		}
		static Dictionary<string, string> CreateReportDocumentViewerElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Report Document Map", ASPxTreeViewElementsCss.Control);
			elements.Add("Report Document Map Node", ASPxTreeViewElementsCss.Node);
			elements.Add("Report Document Map Node Text", ASPxTreeViewElementsCss.NodeText);
			elements.Add("Report Document Map Expand Button Image", ASPxTreeViewElementsCss.ExpandButtonImage);
			elements.Add("Report Document Map Collapse Button Image", ASPxTreeViewElementsCss.CollapseButtonImage);
			elements.Add("Report ToolBar", ASPxMenuElementsCss.Control);
			elements.Add("Report ToolBar Menu", ASPxMenuElementsCss.RootMenu);
			elements.Add("Report ToolBar Item", ASPxMenuElementsCss.RootItem);
			elements.Add("Report ToolBar Item Drop Down", ASPxDropDownEditElementsCss.Control);
			elements.Add("Report ToolBar Item Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Report ToolBar Item Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("Report ToolBar Item Text Box", ASPxTextBoxElementsCss.Control);
			elements.Add("Report ToolBar Search Button Image", ReportToolbarElementsCss.SearchButtonImage);
			elements.Add("Report ToolBar Print Button Image", ReportToolbarElementsCss.PrintButtonImage);
			elements.Add("Report ToolBar Print Page Button Image", ReportToolbarElementsCss.PrintPageButtonImage);
			elements.Add("Report ToolBar First Page Button Image", ReportToolbarElementsCss.FirstPageButtonImage);
			elements.Add("Report ToolBar Prev Page Button Image", ReportToolbarElementsCss.PrevPageButtonImage);
			elements.Add("Report ToolBar Next Page Button Image", ReportToolbarElementsCss.NextPageButtonImage);
			elements.Add("Report ToolBar Last Page Button Image", ReportToolbarElementsCss.LastPageButtonImage);
			elements.Add("Report ToolBar Save Button Image", ReportToolbarElementsCss.SaveButtonImage);
			elements.Add("Report ToolBar Save Window Button Image", ReportToolbarElementsCss.SaveWindowButtonImage);
			elements.Add("Report Parameters Panel Parameter Label", ASPxLabelElementsCss.Control);
			elements.Add("Report Parameters Panel Parameter Value Text Box", ASPxTextBoxElementsCss.Control);
			elements.Add("Report Parameters Panel Button", ASPxButtonElementsCss.Control);
			return elements;
		}
		static Dictionary<string, string> CreateReportDocumentMapElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxTreeViewElementsCss.Control);
			elements.Add("Node", ASPxTreeViewElementsCss.Node);
			elements.Add("Node Text", ASPxTreeViewElementsCss.NodeText);
			elements.Add("Expand Button Image", ASPxTreeViewElementsCss.ExpandButtonImage);
			elements.Add("Collapse Button Image", ASPxTreeViewElementsCss.CollapseButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateReportToolbarMapElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Control", ASPxMenuElementsCss.Control);
			elements.Add("Menu", ASPxMenuElementsCss.RootMenu);
			elements.Add("Item", ASPxMenuElementsCss.RootItem);
			elements.Add("Item Drop Down", ASPxDropDownEditElementsCss.Control);
			elements.Add("Item Drop Down Button", ASPxDropDownEditElementsCss.DropDownButton);
			elements.Add("Item Drop Down Button Image", ASPxDropDownEditElementsCss.DropDownButtonImage);
			elements.Add("Item Text Box", ASPxTextBoxElementsCss.Control);
			elements.Add("Search Button Image", ReportToolbarElementsCss.SearchButtonImage);
			elements.Add("Print Button Image", ReportToolbarElementsCss.PrintButtonImage);
			elements.Add("Print Page Button Image", ReportToolbarElementsCss.PrintPageButtonImage);
			elements.Add("First Page Button Image", ReportToolbarElementsCss.FirstPageButtonImage);
			elements.Add("Prev Page Button Image", ReportToolbarElementsCss.PrevPageButtonImage);
			elements.Add("Next Page Button Image", ReportToolbarElementsCss.NextPageButtonImage);
			elements.Add("Last Page Button Image", ReportToolbarElementsCss.LastPageButtonImage);
			elements.Add("Save Button Image", ReportToolbarElementsCss.SaveButtonImage);
			elements.Add("Save Window Button Image", ReportToolbarElementsCss.SaveWindowButtonImage);
			return elements;
		}
		static Dictionary<string, string> CreateReportParametersPanelElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Parameter Label", ASPxLabelElementsCss.Control);
			elements.Add("Parameter Value Text Box", ASPxTextBoxElementsCss.Control);
			elements.Add("Button", ASPxButtonElementsCss.Control);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return XtraReportsProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.XtraReportsName; }
		}
		public override string Title {
			get { return "Reports"; }
		}
		public override string[] Folders {
			get { return new string[] { "XtraReports" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "ReportToolbar.skin", "ReportViewer.skin", "ReportParametersPanel.skin", "ReportDocumentMap.skin", "ASPxDocumentViewer.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyReportsWeb; }
		}
	}
	public class XtraChartsProduct: ThemedProduct {
		static readonly Dictionary<string, Dictionary<string, string>> ControlElementsMapCollection = new Dictionary<string, Dictionary<string, string>>();
		static XtraChartsProduct() {
			ControlElementsMapCollection.Add("WebChartControl", XtraChartsProduct.CreateChartControlElementsMap());
		}
		static Dictionary<string, string> CreateChartControlElementsMap() {
			Dictionary<string, string> elements = new Dictionary<string, string>();
			elements.Add("Loading Panel", WebChartControlElementsCss.LoadingPanelControl);
			return elements;
		}
		public override Dictionary<string, Dictionary<string, string>> GetControlElementsMapCollection() {
			return XtraChartsProduct.ControlElementsMapCollection;
		}
		public override string Name {
			get { return ThemedProducts.XtraChartsName; }
		}
		public override string Title {
			get { return "Charts"; }
		}
		public override string[] Folders {
			get { return new string[] { "Chart" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { "WebChartControl.skin" }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience }; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyChartsWeb; }
		}
	}
	public class MVCExtensionsProduct: ThemedProduct {
		public override string Name {
			get { return ThemedProducts.MVCExtensionsName; }
		}
		public override string Title {
			get { return "MVC Extensions"; }
		}
		public override string[] Folders {
			get { return new string[] { }; }
		}
		public override string[] SkinFiles {
			get {
				return new string[] { "MVCxGridView.skin", "MVCxGridLookup.skin", "MVCxHtmlEditor.skin", "MVCxSpellChecker.skin",
					"MVCxCallbackPanel.skin", "MVCxPanel.skin", "MVCxMenu.skin", "MVCxNavBar.skin", "MVCxRibbon.skin", "MVCxPopupControl.skin", "MVCxRatingControl.skin", "MVCxRoundPanel.skin", 
					"MVCxSplitter.skin", "MVCxPageControl.skin", "MVCxTabControl.skin", "MVCxTreeView.skin", "MVCxUploadControl.skin",
					"MVCxButton.skin", "MVCxButtonEdit.skin", "MVCxCalendar.skin", "MVCxCaptcha.skin", "MVCxCheckBox.skin", "MVCxColorEdit.skin", 
					"MVCxComboBox.skin", "MVCxDateEdit.skin", "MVCxDropDownEdit.skin", "MVCxHyperLink.skin", "MVCxLabel.skin", 
					"MVCxListBox.skin", "MVCxMemo.skin", "MVCxProgressBar.skin", "MVCxRadioButton.skin", "MVCxRadioButtonList.skin", 
					"MVCxSpinEdit.skin", "MVCxTextBox.skin", "MVCxTimeEdit.skin", "MVCxTokenBox.skin" , "MVCxChartControl.skin",
					"MVCxReportViewer.skin", "MVCxReportToolbar.skin", "MVCxDocumentViewer.skin", "MVCxTrackBar.skin", "MVCxCheckBoxList.skin", "MVCxPivotGrid.skin", 
					"MVCxPopupMenu.skin", "MVCxLoadingPanel.skin", "MVCxScheduler.skin", "MVCxDockPanel.skin", "MVCxDataView.skin", "MVCxTreeList.skin",
					"MVCxValidationSummary.skin", "MVCxFileManager.skin", "MVCxImageSlider.skin", "MVCxImageGallery.skin", "MVCxImageZoom.skin", "MVCxImageZoomNavigator.skin", 
					"MVCxFormLayout.skin", "MVCxSpreadsheet.skin", "MVCxRichEdit.skin", "MVCxCardView.skin"
				};
			}
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors, ThemedProducts.ASPxGridView, 
				ThemedProducts.ASPxHtmlEditor, ThemedProducts.ASPxSpellChecker, ThemedProducts.XtraCharts, ThemedProducts.XtraReports, 
				ThemedProducts.ASPxTreeList, ThemedProducts.ASPxPivotGrid, ThemedProducts.ASPxScheduler };
			}
		}
		public override bool Public {
			get { return false; }
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyMVC; }
		}
		public override bool AllowCustomizeSkinFile(string controlName) {
			return false;
		}
		public override bool AllowCustomizeCssFiles(string controlName) {
			return false;
		}
	}
	public class XafProduct : ThemedProduct {
		public override string Name {
			get { return ThemedProducts.XafName; }
		}
		public override string Title {
			get { return "Xaf"; }
		}
		public override string[] Folders {
			get { return new string[] { "Xaf" }; }
		}
		public override string[] SkinFiles {
			get { return new string[] { }; }
		}
		public override ThemedProduct[] SubProducts {
			get { return new ThemedProduct[] { ThemedProducts.ASPxperience, ThemedProducts.ASPxEditors, ThemedProducts.ASPxGridView, ThemedProducts.ASPxTreeList }; }
		}
		public override bool ExtractSubProductsSkinFiles { 
			get { return true; } 
		}
		public override string AssemblyName {
			get { return AssemblyInfo.SRAssemblyExpressAppWeb; }
		}
	}
}
