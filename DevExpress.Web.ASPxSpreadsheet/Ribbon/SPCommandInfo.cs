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
using System.Drawing;
using DevExpress.Office.Model;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Drawing.Printing;
using DevExpress.Office.Utils;
using DevExpress.Office;
using DevExpress.Office.API.Internal;
using DevExpress.Office.Design.Internal;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public static class SpreadsheetRibbonHelper {
		public static string CommandSeparator = ";#";
		const string LargeImagePostfix = "Large";
		static InnerSpreadsheetControl innerControl;
		static Dictionary<SpreadsheetCommandId, SpreadsheetCommand> commands;
		static InnerSpreadsheetControl InnerControl {
			get {
				if(innerControl == null) {
					var webSpreadsheetControl = new WebSpreadsheetControl();
					innerControl = webSpreadsheetControl.InnerControl;
				}
				return innerControl;
			}
		}
		static ASPxRibbon GetRibbonControl(object owner) {
			RibbonItemBase item = owner as RibbonItemBase;
			if(item != null)
				return GetRibbonControl(item.Ribbon);
			RibbonGroup group = owner as RibbonGroup;
			if(group != null)
				return GetRibbonControl(group.Ribbon);
			return owner as ASPxRibbon;
		}
		static Dictionary<SpreadsheetCommandId, SpreadsheetCommand> Commands {
			get {
				if(commands == null)
					commands = new Dictionary<SpreadsheetCommandId, SpreadsheetCommand>();
				return commands;
			}
		}
		static object locker = new object();
		public static SpreadsheetCommand GetCommandById(SpreadsheetCommandId commandID) {
			lock(locker) {
				if(Commands.ContainsKey(commandID))
					return Commands[commandID];
				var command = InnerControl.CreateCommand(commandID);
				Commands.Add(commandID, command);
				return command;
			}
		}
		public static string GetCommandImage(SpreadsheetCommandId commandID) {
			return GetCommandById(commandID).ImageName;
		}
		public static string GetCommandName(SpreadsheetCommandId commandID) {
			return GetCommandById(commandID).Id.ToString();
		}
		public static string GetCommandToolTip(SpreadsheetCommandId commandID) {
			return GetCommandById(commandID).Description;
		}
		public static string GetCommandText(SpreadsheetCommandId commandID) {
			return ClearAmpersand(GetCommandById(commandID).MenuCaption);
		}
		public static string GetCommandDescription(SpreadsheetCommandId commandID) {
			return ClearAmpersand(GetCommandById(commandID).Description);
		}
		static string GetCommandLargeImage(SpreadsheetCommandId commandID) {
			string img = GetCommandImage(commandID);
			img = string.IsNullOrEmpty(img) ? string.Empty : img + LargeImagePostfix;
			return img;
		}
		static string GetCommandSmallImage(SpreadsheetCommandId commandID) {
			return GetCommandImage(commandID);
		}
		public static ItemImagePropertiesBase GetRibbonItemLargeImageProperty(object owner, SpreadsheetCommandId commandID) {
			ItemImageProperties imageProperties = new ItemImageProperties();
			var imageName = GetCommandLargeImage(commandID);
			if(!string.IsNullOrEmpty(imageName)) {
				ASPxRibbon ribbon = GetRibbonControl(owner);
				if(ribbon != null) {
					imageProperties.CopyFrom(RibbonHelper.GetRibbonImageProperties(ribbon, SpreadsheetRibbonImages.RibbonSSSpriteName,
						delegate(ISkinOwner skinOwner) { return new SpreadsheetRibbonImages(skinOwner, ribbon.Images.IconSet); }, imageName, string.Empty));
				}
			}
			return imageProperties;
		}
		public static ItemImagePropertiesBase GetRibbonItemSmallImageProperty(object owner, SpreadsheetCommandId commandID) {
			var imageProperties = new ItemImageProperties();
			var imageName = GetCommandSmallImage(commandID);
			if(!string.IsNullOrEmpty(imageName)) {
				ASPxRibbon ribbon = GetRibbonControl(owner);
				if(ribbon != null) {
					imageProperties.CopyFrom(RibbonHelper.GetRibbonImageProperties(ribbon, SpreadsheetRibbonImages.RibbonSSSpriteName,
						delegate(ISkinOwner skinOwner) { return new SpreadsheetRibbonImages(skinOwner, ribbon.Images.IconSet); }, imageName, string.Empty));
				}
			}
			return imageProperties;
		}
		public static ItemImagePropertiesBase GetSpreadsheetSmallImageProperty(ASPxSpreadsheet owner, SpreadsheetCommandId commandID) {
			var imageProperties = new ItemImageProperties();
			var imageName = GetCommandSmallImage(commandID);
			if(!string.IsNullOrEmpty(imageName))
				imageProperties.CopyFrom(owner.Images.GetImageProperties(owner.Page, imageName));
			return imageProperties;
		}
		public static ItemImagePropertiesBase GetSpreadsheetLargeImageProperty(ASPxSpreadsheet owner, SpreadsheetCommandId commandID) {
			var imageProperties = new ItemImageProperties();
			var imageName = GetCommandLargeImage(commandID);
			if(!string.IsNullOrEmpty(imageName))
				imageProperties.CopyFrom(owner.Images.GetImageProperties(owner.Page, imageName));
			return imageProperties;
		}
		internal static ItemImagePropertiesBase GetRibbonGroupImageProperty(object owner, string imageName) {
			return GetRibbonItemImageProperty(owner, imageName);
		}
		internal static ItemImagePropertiesBase GetRibbonItemImageProperty(object owner, string imageName) {
			var imageProperties = new ItemImageProperties();
			ASPxRibbon ribbon = GetRibbonControl(owner);
			if(ribbon != null) {
				if(!string.IsNullOrEmpty(imageName)) {
					imageProperties.CopyFrom(RibbonHelper.GetRibbonImageProperties(ribbon, SpreadsheetRibbonImages.RibbonSSSpriteName,
						delegate(ISkinOwner skinOwner) { return new SpreadsheetRibbonImages(skinOwner, ribbon.Images.IconSet); }, imageName, string.Empty));
				} else {
					imageProperties.CopyFrom(EmptyImageProperties.GetGlobalEmptyImage(ribbon.Page));
				}
			}
			return imageProperties;
		}
		internal static ItemImagePropertiesBase GetRibbonGroupImageProperty(object owner, SpreadsheetCommandId commandID) {
			var imageName = GetCommandLargeImage(commandID);
			return GetRibbonGroupImageProperty(owner, imageName);
		}
		public static ListEditItemCollection GetFontSizes() {
			ListEditItemCollection fontSizesCollection = new ListEditItemCollection();
			PredefinedFontSizeCollection fontSizes = InnerControl.PredefinedFontSizeCollection;
			int count = fontSizes.Count;
			for(int i = 0; i < count; i++) {
				fontSizesCollection.Add(fontSizes[i].ToString(), fontSizes[i].ToString());
			}
			return fontSizesCollection;
		}
		static string ClearAmpersand(string buttonLable) {
			buttonLable = buttonLable.Contains("&&") ? buttonLable.Replace("&&", "&") : buttonLable.Replace("&", "");
			return buttonLable;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetMathBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			baseCommand.Add("Sum", new FunctionSum());
			baseCommand.Add("Average", new FunctionAverage());
			baseCommand.Add("Count Numbers", new FunctionCount());
			baseCommand.Add("Max", new FunctionMax());
			baseCommand.Add("Min", new FunctionMin());
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetFinancialBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddFinancialFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetLogicalBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddLogicalFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetTextBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddTextFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetDateTimeBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddDateTimeFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetLookupBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddLookupAndReferenceFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetTrigonometryBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddMathAndTrigonometryFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetStatisticalBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddStatisticalFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetEngineeringBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddEngineeringFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetCubeBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddCubeFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetInformationalBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddInformationalFunctions(baseCommand);
			return baseCommand;
		}
		internal static Dictionary<string, ISpreadsheetFunction> GetCompatibilityBaseFunctions() {
			Dictionary<string, ISpreadsheetFunction> baseCommand = new Dictionary<string, ISpreadsheetFunction>();
			FormulaCalculator.AddCompatibilityFunctions(baseCommand);
			return baseCommand;
		}
		public static void AddTabCollectionToControl(ASPxRibbon ribbon, RibbonTab[] tabs, bool clearExistingTabs) {
			if(clearExistingTabs) {
				foreach(RibbonTab tab in tabs) {
					RibbonTab foundTab = ribbon.Tabs.FindByName(tab.Name);
					if(foundTab != null)
						ribbon.Tabs.RemoveAll(i => i.Name == foundTab.Name);
				}
			}
			ribbon.Tabs.AddRange(tabs);
		}
		internal static void UpdateRibbonTabCollection(RibbonTabCollection ribbonTabs) {
			if(!ribbonTabs.IsEmpty) {
				foreach(RibbonTab tab in ribbonTabs) {
					foreach(RibbonGroup group in tab.Groups) {
						foreach(RibbonItemBase item in group.Items) {
							if((item is SRFormatFontNameCommand) || (item is SRFormatFontSizeCommand)) {
								var spNameorSizeItem = item as SRComboBoxCommandBase;
								if(spNameorSizeItem.Items.IsEmpty)
									spNameorSizeItem.FillItems();
							}
						}
					}
				}
			}
		}
		internal static void UpdateRibbonContextTabCategories(RibbonContextTabCategoryCollection contextTabCategoryCollection) {
			if(!contextTabCategoryCollection.IsEmpty) {
				foreach(RibbonContextTabCategory category in contextTabCategoryCollection) {
					UpdateRibbonTabCollection(category.Tabs);
				}
			}
		}
		internal static void PrepareDocumentUnitDependencyItems(RibbonTabCollection ribbonTabs, ASPxSpreadsheet owner) {
			var currentWorkSession = owner.GetCurrentWorkSessions();
			if(!ribbonTabs.IsEmpty && currentWorkSession != null) {
				var documentUnits = currentWorkSession.Document.Unit == DocumentUnit.Document ? DocumentUnit.Inch : currentWorkSession.Document.Unit;
				if(documentUnits != DocumentUnit.Inch)
					foreach(RibbonTab tab in ribbonTabs) {
						foreach(RibbonGroup group in tab.Groups) {
							foreach(RibbonItemBase item in group.Items) {
								if(item is SRPageSetupMarginsCommand) {
									var ribbonItem = item as SRPageSetupMarginsCommand;
									for(int i = 0; i < ribbonItem.Items.Count; i++)
										if(ribbonItem.Items[i] is SRMarginsCommandBase) {
											SRMarginsCommandBase marginItem = ribbonItem.Items[i] as SRMarginsCommandBase;
											var command = currentWorkSession.WebSpreadsheetControl.InnerControl.CreateCommand(marginItem.GetCommandId());
											if(marginItem.Text == SpreadsheetRibbonHelper.GetCommandText(marginItem.GetCommandId()))
												marginItem.ResetTemplateCaption(command.MenuCaption);
										}
								}
								if(item is SRPageSetupPaperKindCommand) {
									var ribbonItem = item as SRPageSetupPaperKindCommand;
									if(ribbonItem.Items.Count > 0)
										foreach(SRPagePaperKindBase paperKindItem in ribbonItem.Items)
											paperKindItem.ResetTemplateCaption(GetPaperKindCaption(paperKindItem.GetPagePaperKind(), currentWorkSession.Document));
									return;
								}
							}
						}
					}
			}
		}
		internal static string GetPaperKindCaption(PaperKind PagePaperKind, DevExpress.Spreadsheet.IWorkbook document) {
			Size paperSizeInTwips = PaperSizeCalculator.CalculatePaperSize(PagePaperKind);
			DocumentModel documentModel = InnerControl.DocumentModel;
			DocumentUnit unit = DocumentUnit.Document;
			if(document != null)
				unit = document.Unit == DocumentUnit.Document ? DocumentUnit.Inch : document.Unit;
			else
				unit = InnerControl.Unit == DocumentUnit.Document ? DocumentUnit.Inch : InnerControl.Unit;
			UnitConverter unitConverter = documentModel.InternalAPI.UnitConverters[unit];
			UIUnit width = new UIUnit(unitConverter.FromUnits(documentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips.Width)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			UIUnit height = new UIUnit(unitConverter.FromUnits(documentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips.Height)), unit, UnitPrecisionDictionary.DefaultPrecisions);
			return String.Format("<table class='dxss-mg-table'><tr class='dxss-mg-row'><td class='dxss-mg-title'>{0}</td></tr><tr class='dxss-mg-row'><td class='dxss-mg-cell'>{1} x {2}</td></tr></table>", PagePaperKind, width.ToString(), height.ToString());
		}
		internal static string GetMarginItemCaption(string defaultText) {
			if(defaultText.Contains("\t") && defaultText.Contains("\r\n")) {
				defaultText = defaultText.Replace("\t\t", "\t").Replace(" ", "");
				string[] marginLines = defaultText.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				string marginName = marginLines[0];
				defaultText = "<table class='dxss-mg-table'>";
				defaultText += "<tr class='dxss-mg-row'><td class='dxss-mg-title' colspan='6'>" + marginName + "</td></tr>";
				for(int i = 1; i < marginLines.Length; i++) {
					string[] marginValues = marginLines[i].Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
					defaultText += "<tr class='dxss-mg-row'>";
					for(int k = 0; k < marginValues.Length; k++) {
						defaultText += "<td class='dxss-mg-cell'>" + marginValues[k] + "</td>";
						if(k + 1 == marginValues.Length / 2)
							defaultText += "<td class='dxss-mg-md-sep'>&nbsp;</td>";
					}
					defaultText += "<td class='dxss-mg-end-sep'>&nbsp;</td>";
					defaultText += "</tr>";
				}
				defaultText += "</table>";
			}
			return defaultText;
		}
	}
}
