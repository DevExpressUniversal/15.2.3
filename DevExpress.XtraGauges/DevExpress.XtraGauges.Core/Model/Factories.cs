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
using DevExpress.XtraGauges.Core.Drawing;
using System.Drawing;
using DevExpress.XtraGauges.Core.XAML;
using DevExpress.XtraGauges.Core.SHP;
using DevExpress.XtraGauges.Core.Base;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Model {
	public static class PredefinedCoreNames {
		public static readonly string LinearGaugeRotationNode;
		static PredefinedCoreNames() {
			LinearGaugeRotationNode = "LinearGaugeRotationNode";
		}
	}
	public static class PredefinedShapeNames {
		public static readonly string ArcScaleShape;
		public static readonly string LinearScaleShape;
		public static readonly string BackgroundLayer;
		public static readonly string Effect;
		public static readonly string Needle;
		public static readonly string Tick;
		public static readonly string SpindleCap;
		public static readonly string BarStart;
		public static readonly string BarPacked;
		public static readonly string BarEmpty;
		public static readonly string BarEnd;
		public static readonly string IndicatorLEDs;
		public static readonly string DigitalBGNear;
		public static readonly string DigitalBGCenter;
		public static readonly string DigitalBGFar;
		public static readonly string Marker;
		public static readonly string RangeBar;
		public static readonly string IndicatorState;
		public static readonly string DashboardIndicatorState;
		public static readonly string MenuItemFrame;
		public static readonly string ItemFrame;
		public static readonly string Label;
		public static readonly string ImageIndicator;
		static PredefinedShapeNames() {
			ArcScaleShape = "ArcScaleShape";
			LinearScaleShape = "LinearScaleShape";
			BackgroundLayer = "BackgroundLayer";
			Effect = "Effect";
			Needle = "Needle";
			SpindleCap = "SpindleCap";
			Tick = "Tick";
			BarStart = "BarStart";
			BarPacked = "BarPacked";
			BarEmpty = "BarEmpty";
			BarEnd = "BarEnd";
			Marker = "Marker";
			RangeBar = "RangeBar";
			IndicatorState = "IndicatorState";
			DashboardIndicatorState = "DashboardIndicatorState";
			IndicatorLEDs = "IndicatorLEDs";
			DigitalBGNear = "Back_Start";
			DigitalBGCenter = "Back";
			DigitalBGFar = "Back_End";
			MenuItemFrame = "MenuItemFrame";
			ItemFrame = "ItemFrame";
			Label = "Label";
			ImageIndicator = "ImageIndicator";
		}
	}
	public static class ShapeLoaderFactory {
		static readonly object syncObj = new object();
		static Dictionary<DefaultShapeLoaderType, IShapeLoader> defaultLoaders;
		static ShapeLoaderFactory() {
			defaultLoaders = new Dictionary<DefaultShapeLoaderType, IShapeLoader>();
			defaultLoaders.Add(DefaultShapeLoaderType.XAMLLoader, new XamlLoader());
			defaultLoaders.Add(DefaultShapeLoaderType.SHPLoader, new ShpLoader());
		}
		public static IShapeLoader GetDefaultShapeLoader(DefaultShapeLoaderType type) {
			lock(syncObj) {
				return defaultLoaders[type];
			}
		}
	}
	public static class UniversalShapesFactory {
		static readonly object syncObj = new object();
		static Dictionary<string, BaseShape> defaultShapes;
		static UniversalShapesFactory() {
			defaultShapes = new Dictionary<string, BaseShape>();
			IShapeLoader loader = ShapeLoaderFactory.GetDefaultShapeLoader(DefaultShapeLoaderType.XAMLLoader);
			ComplexShape uiShapes = loader.LoadFromResources("DevExpress.XtraGauges.Core.Resources.XAML.point.xaml");
			defaultShapes.Add(PredefinedShapeNames.MenuItemFrame, uiShapes.Collection[PredefinedShapeNames.MenuItemFrame]);
			defaultShapes.Add(PredefinedShapeNames.ItemFrame, uiShapes.Collection[PredefinedShapeNames.ItemFrame]);
		}
		public static BaseShape GetShape(string shapeName) {
			lock(syncObj) {
				return defaultShapes[shapeName].Clone();
			}
		}
	}
	public static class XAMLResoucesFactory {
		static readonly object syncObj = new object();
		static Dictionary<string, BaseShape> shapesCore;
		static XAMLResoucesFactory() {
			shapesCore = new Dictionary<string, BaseShape>();
			shapesCore.Add("Empty", BaseShape.Empty);
		}
		private static void LoadDigitalShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch (shapeName) {
				case "pointer": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "point.xaml")); break;
				case "defaultDigit7SShape": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Digital.indicator-7.xaml")); break;
				case "defaultDigit14SShape": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Digital.indicator-14.xaml")); break;
				case "defaultDigitMatrix5x8Shape": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Digital.matrixIndicator5x7.xaml")); break;
				case "defaultDigitMatrix8x14Shape": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Digital.Indicator8x14.xaml")); break;
			}
			resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.Digital.";
			switch (shapeName) {
				case "BGDigital_1": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style1.xaml")); break;
				case "BGDigital_2": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style2.xaml")); break;
				case "BGDigital_3": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style3.xaml")); break;
				case "BGDigital_4": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style4.xaml")); break;
				case "BGDigital_5": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style5.xaml")); break;
				case "BGDigital_6": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style6.xaml")); break;
				case "BGDigital_7": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style7.xaml")); break;
				case "BGDigital_8": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style8.xaml")); break;
				case "BGDigital_9": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style9.xaml")); break;
				case "BGDigital_10": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style10.xaml")); break;
				case "BGDigital_11": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style11.xaml")); break;
				case "BGDigital_12": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style12.xaml")); break;
				case "BGDigital_13": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style13.xaml")); break;
				case "BGDigital_14": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style14.xaml")); break;
				case "BGDigital_15": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style15.xaml")); break;
				case "BGDigital_16": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style16.xaml")); break;
				case "BGDigital_17": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style17.xaml")); break;
				case "BGDigital_18": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style18.xaml")); break;
				case "BGDigital_19": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style19.xaml")); break;
				case "BGDigital_20": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style20.xaml")); break;
				case "BGDigital_21": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style21.xaml")); break;
				case "BGDigital_22": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style22.xaml")); break;
				case "BGDigital_23": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style23.xaml")); break;
				case "BGDigital_24": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style24.xaml")); break;
				case "BGDigital_25": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style25.xaml")); break;
				case "BGDigital_26": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Style26.xaml")); break;
			}
		}
		static void LoadStateIndicatorShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.State.";
			switch (shapeName) {
				case "EqualizerShapes": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Equalizer.xaml")); break;
				case "ProgressShapes": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "ProgressItems.xaml")); break;
				case "CarIconsShapes": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "CarIcons.xaml")); break;
				case "ElectricLight": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "ElectricLight.xaml")); break;
				case "Arrow": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Arrow.xaml")); break;
				case "DashboardIndicator": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "DashboardIndicator.xaml")); break;
				case "Smile": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Smile.xaml")); break;
				case "LightSignal": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "LightSignal.xaml")); break;
				case "Flag": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "Flags.xaml")); break;
				case "Currency": shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + "CurrencySymbols.xaml")); break;
			}
		}
		static void LoadCircularFullShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch (shapeName) {
				case "Circular.Full.Style1": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style2": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style3": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style4": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style5": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style6": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style7": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style8": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style9": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style10": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style11": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style12": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style13": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style14": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style15": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style16": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style17": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style18": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style19": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style20": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style21": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style22": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style23": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style24": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style25": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style26": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style27": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Full.Style28": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Temp.Demo.demo_360": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Temp.Demo.Demo.SysInfo": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Temp.Demo.CarPanel": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
			}
		}
		static void LoadCircularHalfShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.Circular.";
			switch(shapeName) {
				case "Circular.Half.Style1": shapesCore.Add("Circular.Half.Style1", loader.LoadFromResources(resourceRoot + "Half.Style1.xaml")); break;
				case "Circular.Half.Style2": shapesCore.Add("Circular.Half.Style2", loader.LoadFromResources(resourceRoot + "Half.Style2.xaml")); break;
				case "Circular.Half.Style3": shapesCore.Add("Circular.Half.Style3", loader.LoadFromResources(resourceRoot + "Half.Style3.xaml")); break;
				case "Circular.Half.Style4": shapesCore.Add("Circular.Half.Style4", loader.LoadFromResources(resourceRoot + "Half.Style4.xaml")); break;
				case "Circular.Half.Style5": shapesCore.Add("Circular.Half.Style5", loader.LoadFromResources(resourceRoot + "Half.Style5.xaml")); break;
				case "Circular.Half.Style6": shapesCore.Add("Circular.Half.Style6", loader.LoadFromResources(resourceRoot + "Half.Style6.xaml")); break;
				case "Circular.Half.Style7": shapesCore.Add("Circular.Half.Style7", loader.LoadFromResources(resourceRoot + "Half.Style7.xaml")); break;
				case "Circular.Half.Style8": shapesCore.Add("Circular.Half.Style8", loader.LoadFromResources(resourceRoot + "Half.Style8.xaml")); break;
				case "Circular.Half.Style9": shapesCore.Add("Circular.Half.Style9", loader.LoadFromResources(resourceRoot + "Half.Style9.xaml")); break;
				case "Circular.Half.Style10": shapesCore.Add("Circular.Half.Style10", loader.LoadFromResources(resourceRoot + "Half.Style10.xaml")); break;
				case "Circular.Half.Style11": shapesCore.Add("Circular.Half.Style11", loader.LoadFromResources(resourceRoot + "Half.Style11.xaml")); break;
				case "Circular.Half.Style12": shapesCore.Add("Circular.Half.Style12", loader.LoadFromResources(resourceRoot + "Half.Style12.xaml")); break;
				case "Circular.Half.Style13": shapesCore.Add("Circular.Half.Style13", loader.LoadFromResources(resourceRoot + "Half.Style13.xaml")); break;
				case "Circular.Half.Style14": shapesCore.Add("Circular.Half.Style14", loader.LoadFromResources(resourceRoot + "Half.Style14.xaml")); break;
				case "Circular.Half.Style15": shapesCore.Add("Circular.Half.Style15", loader.LoadFromResources(resourceRoot + "Half.Style15.xaml")); break;
				case "Circular.Half.Style16": shapesCore.Add("Circular.Half.Style16", loader.LoadFromResources(resourceRoot + "Half.Style16.xaml")); break;
				case "Circular.Half.Style17": shapesCore.Add("Circular.Half.Style17", loader.LoadFromResources(resourceRoot + "Half.Style17.xaml")); break;
				case "Circular.Half.Style18": shapesCore.Add("Circular.Half.Style18", loader.LoadFromResources(resourceRoot + "Half.Style18.xaml")); break;
				case "Circular.Half.Style19": shapesCore.Add("Circular.Half.Style19", loader.LoadFromResources(resourceRoot + "Half.Style19.xaml")); break;
				case "Circular.Half.Style20": shapesCore.Add("Circular.Half.Style20", loader.LoadFromResources(resourceRoot + "Half.Style20.xaml")); break;
				case "Circular.Half.Style21": shapesCore.Add("Circular.Half.Style21", loader.LoadFromResources(resourceRoot + "Half.Style21.xaml")); break;
				case "Circular.Half.Style22": shapesCore.Add("Circular.Half.Style22", loader.LoadFromResources(resourceRoot + "Half.Style22.xaml")); break;
				case "Circular.Half.Style23": shapesCore.Add("Circular.Half.Style23", loader.LoadFromResources(resourceRoot + "Half.Style23.xaml")); break;
				case "Circular.Half.Style24": shapesCore.Add("Circular.Half.Style24", loader.LoadFromResources(resourceRoot + "Half.Style24.xaml")); break;
				case "Circular.Half.Style25": shapesCore.Add("Circular.Half.Style25", loader.LoadFromResources(resourceRoot + "Half.Style25.xaml")); break;
				case "Circular.Half.Style26": shapesCore.Add("Circular.Half.Style26", loader.LoadFromResources(resourceRoot + "Half.Style26.xaml")); break;
				case "Circular.Half.Style27": shapesCore.Add("Circular.Half.Style27", loader.LoadFromResources(resourceRoot + "Half.Style27.xaml")); break;
				case "Circular.Half.Style28": shapesCore.Add("Circular.Half.Style28", loader.LoadFromResources(resourceRoot + "Half.Style28.xaml")); break;
			}
		}
		static void LoadCircularQuarterShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch (shapeName) {
				case "Circular.Quarter.Style1.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style2.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style3.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style4.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style5.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style6.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style7.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style8.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style9.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style10.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style11.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style12.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style13.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style14.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style15.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style16.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style17.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style18.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style19.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style20.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style21.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style22.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style23.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style24.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style25.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style26.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style27.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style28.Left": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style1.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style2.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style3.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style4.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style5.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style6.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style7.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style8.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style9.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style10.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style11.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style12.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style13.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style14.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style15.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style16.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style17.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style18.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style19.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style20.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style21.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style22.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style23.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style24.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style25.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style26.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style27.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Quarter.Style28.Right": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
			}
		}
		static void LoadCircularThreeFouthShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch (shapeName) {
				case "Circular.ThreeFourth.Style1": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style2": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style3": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style4": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style5": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style6": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style7": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style8": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style9": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style10": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style11": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style12": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style13": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style14": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style15": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style16": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style17": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style18": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style19": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style20": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style21": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style22": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style23": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style24": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style25": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style26": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style27": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.ThreeFourth.Style28": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
			}
		}
		static void LoadLinearShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch (shapeName) {
				case "Linear.Style1": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style2": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style3": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style4": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style5": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style6": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style7": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style8": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style9": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style10": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style11": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style12": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style13": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style14": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style15": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style16": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style17": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style18": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style19": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style20": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style21": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style22": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style23": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style24": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style25": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Linear.Style26": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
			}
		}
		static void LoadCircularWideShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch(shapeName) {
				case "Circular.Wide.Style1": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style2": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style3": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style4": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style5": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style6": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style7": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style8": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style9": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style10": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style11": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style12": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style13": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style14": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
				case "Circular.Wide.Style15": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
			}
		}
		static void LoadMarkerShapes(IShapeLoader loader, string shapeName) {
			string resourceRoot = "DevExpress.XtraGauges.Core.Resources.XAML.";
			switch(shapeName) {
				case "Markers": LoadShapeFromResources(loader, shapeName, resourceRoot); break;
			}
		}
		private static void LoadShapeFromResources(IShapeLoader loader, string shapeName, string resourceRoot) {
			shapesCore.Add(shapeName, loader.LoadFromResources(resourceRoot + shapeName + ".xaml"));
		}
		public static void LoadShape(string shapeName) {
			IShapeLoader loader = ShapeLoaderFactory.GetDefaultShapeLoader(DefaultShapeLoaderType.XAMLLoader);
			LoadDigitalShapes(loader, shapeName);
			LoadLinearShapes(loader, shapeName);
			LoadCircularFullShapes(loader, shapeName);
			LoadCircularHalfShapes(loader, shapeName);
			LoadCircularQuarterShapes(loader, shapeName);
			LoadCircularThreeFouthShapes(loader, shapeName);
			LoadCircularWideShapes(loader, shapeName);
			LoadStateIndicatorShapes(loader, shapeName);
			LoadMarkerShapes(loader, shapeName);
		}
		public static BaseShape GetShape(string shapeName) {
			lock(syncObj) {
				if(!shapesCore.ContainsKey(shapeName)) LoadShape(shapeName);
				return shapesCore[shapeName].Clone();
			}
		}
	}
	public static class DigitShapesFactory {
		static readonly object syncObj = new object();
		static Dictionary<DigitShapeType, BaseShape> defaultShapes;
		static DigitShapesFactory() {
			defaultShapes = new Dictionary<DigitShapeType, BaseShape>();
		}
		static void LoadShapeCore(DigitShapeType type) {
			IShapeLoader loader = ShapeLoaderFactory.GetDefaultShapeLoader(DefaultShapeLoaderType.XAMLLoader);
			switch (type) {
				case DigitShapeType.Segment7: LoadShape("defaultDigit7SShape", type); break;
				case DigitShapeType.Segment14: LoadShape("defaultDigit14SShape", type); break;
				case DigitShapeType.Matrix5x8: LoadShape("defaultDigitMatrix5x8Shape", type); break;
				case DigitShapeType.Matrix8x14: LoadShape("defaultDigitMatrix8x14Shape", type); break;
			}
		}
		public static BaseShape GetDefaultDigitShape(DigitShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapeCore(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
		static void LoadShape(string shapeName, DigitShapeType type) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(shapeName) as ComplexShape;
			defaultShapes.Add(type, templateShape.Collection[PredefinedShapeNames.IndicatorLEDs]);
		}
	}
	public static class StateIndicatorShapesFactory {
		static readonly object syncObj = new object();
		static Dictionary<StateIndicatorShapeType, BaseShape> defaultShapes;
		static StateIndicatorShapesFactory() {
			defaultShapes = new Dictionary<StateIndicatorShapeType, BaseShape>();
		}
		private static void LoadShapeCore(StateIndicatorShapeType type) {
			switch(type) {
				case StateIndicatorShapeType.Default:
					ComplexShape defaultShape = XAMLResoucesFactory.GetShape("Currency") as ComplexShape;
					defaultShapes.Add(StateIndicatorShapeType.Default, defaultShape.Collection["Default"]);
					break;
				case StateIndicatorShapeType.ElectricLight1:
				case StateIndicatorShapeType.ElectricLight2:
				case StateIndicatorShapeType.ElectricLight3:
				case StateIndicatorShapeType.ElectricLight4:
					ComplexShape electricLights = XAMLResoucesFactory.GetShape("ElectricLight") as ComplexShape;
					defaultShapes.Add(ElectricLights.Off, electricLights.Collection[PredefinedShapeNames.IndicatorState + "1"]);
					defaultShapes.Add(ElectricLights.Red, electricLights.Collection[PredefinedShapeNames.IndicatorState + "2"]);
					defaultShapes.Add(ElectricLights.Amber, electricLights.Collection[PredefinedShapeNames.IndicatorState + "3"]);
					defaultShapes.Add(ElectricLights.Green, electricLights.Collection[PredefinedShapeNames.IndicatorState + "4"]);
					break;
				case StateIndicatorShapeType.Arrow1:
				case StateIndicatorShapeType.Arrow2:
				case StateIndicatorShapeType.Arrow3:
				case StateIndicatorShapeType.Arrow4:
				case StateIndicatorShapeType.Arrow5:
				case StateIndicatorShapeType.Arrow6:
				case StateIndicatorShapeType.Arrow7:
				case StateIndicatorShapeType.Arrow8:
				case StateIndicatorShapeType.Arrow9:
				case StateIndicatorShapeType.Arrow10:
					ComplexShape arrows = XAMLResoucesFactory.GetShape("Arrow") as ComplexShape;
					defaultShapes.Add(Arrows.Up, arrows.Collection[PredefinedShapeNames.IndicatorState + "1"]);
					defaultShapes.Add(Arrows.Down, arrows.Collection[PredefinedShapeNames.IndicatorState + "2"]);
					defaultShapes.Add(Arrows.Left, arrows.Collection[PredefinedShapeNames.IndicatorState + "3"]);
					defaultShapes.Add(Arrows.Right, arrows.Collection[PredefinedShapeNames.IndicatorState + "4"]);
					defaultShapes.Add(Arrows.LeftUp, arrows.Collection[PredefinedShapeNames.IndicatorState + "5"]);
					defaultShapes.Add(Arrows.RightUp, arrows.Collection[PredefinedShapeNames.IndicatorState + "6"]);
					defaultShapes.Add(Arrows.LeftDown, arrows.Collection[PredefinedShapeNames.IndicatorState + "8"]);
					defaultShapes.Add(Arrows.RightDown, arrows.Collection[PredefinedShapeNames.IndicatorState + "7"]);
					defaultShapes.Add(Arrows.DoubleUp, arrows.Collection[PredefinedShapeNames.IndicatorState + "9"]);
					defaultShapes.Add(Arrows.DoubleDown, arrows.Collection[PredefinedShapeNames.IndicatorState + "10"]);
					break;
				case StateIndicatorShapeType.Smile1:
				case StateIndicatorShapeType.Smile2:
				case StateIndicatorShapeType.Smile3:
				case StateIndicatorShapeType.Smile4:
				case StateIndicatorShapeType.Smile5:
					ComplexShape smiles = XAMLResoucesFactory.GetShape("Smile") as ComplexShape;
					defaultShapes.Add(Smiles.Laughing, smiles.Collection[PredefinedShapeNames.IndicatorState + "1"]);
					defaultShapes.Add(Smiles.Happy, smiles.Collection[PredefinedShapeNames.IndicatorState + "2"]);
					defaultShapes.Add(Smiles.Neutral, smiles.Collection[PredefinedShapeNames.IndicatorState + "3"]);
					defaultShapes.Add(Smiles.Sad, smiles.Collection[PredefinedShapeNames.IndicatorState + "4"]);
					defaultShapes.Add(Smiles.Crying, smiles.Collection[PredefinedShapeNames.IndicatorState + "5"]);
					break;
				case StateIndicatorShapeType.TrafficLight1:
				case StateIndicatorShapeType.TrafficLight2:
				case StateIndicatorShapeType.TrafficLight3:
				case StateIndicatorShapeType.TrafficLight4:
					ComplexShape trafficLights = XAMLResoucesFactory.GetShape("LightSignal") as ComplexShape;
					defaultShapes.Add(TrafficLights.Off, trafficLights.Collection[PredefinedShapeNames.IndicatorState + "1"]);
					defaultShapes.Add(TrafficLights.Red, trafficLights.Collection[PredefinedShapeNames.IndicatorState + "2"]);
					defaultShapes.Add(TrafficLights.Amber, trafficLights.Collection[PredefinedShapeNames.IndicatorState + "3"]);
					defaultShapes.Add(TrafficLights.Green, trafficLights.Collection[PredefinedShapeNames.IndicatorState + "4"]);
					break;
				case StateIndicatorShapeType.FlagUSA:
				case StateIndicatorShapeType.FlagChina:
				case StateIndicatorShapeType.FlagJapan:
				case StateIndicatorShapeType.FlagIndia:
				case StateIndicatorShapeType.FlagGermany:
				case StateIndicatorShapeType.FlagUK:
				case StateIndicatorShapeType.FlagRussia:
				case StateIndicatorShapeType.FlagFrance:
				case StateIndicatorShapeType.FlagBrazil:
				case StateIndicatorShapeType.FlagItaly:
				case StateIndicatorShapeType.FlagSpain:
				case StateIndicatorShapeType.FlagCanada:
				case StateIndicatorShapeType.FlagSouthKorea:
				case StateIndicatorShapeType.FlagIran:
				case StateIndicatorShapeType.FlagIndonesia:
				case StateIndicatorShapeType.FlagAustralia:
				case StateIndicatorShapeType.FlagTaiwan:
				case StateIndicatorShapeType.FlagTurkey:
				case StateIndicatorShapeType.FlagNetherlands:
					ComplexShape flags = XAMLResoucesFactory.GetShape("Flag") as ComplexShape;
					defaultShapes.Add(Flags.USA, flags.Collection["USA"]);
					defaultShapes.Add(Flags.China, flags.Collection["China"]);
					defaultShapes.Add(Flags.Japan, flags.Collection["Japan"]);
					defaultShapes.Add(Flags.India, flags.Collection["India"]);
					defaultShapes.Add(Flags.Germany, flags.Collection["Germany"]);
					defaultShapes.Add(Flags.UK, flags.Collection["UK"]);
					defaultShapes.Add(Flags.Russia, flags.Collection["Russia"]);
					defaultShapes.Add(Flags.France, flags.Collection["France"]);
					defaultShapes.Add(Flags.Brazil, flags.Collection["Brazil"]);
					defaultShapes.Add(Flags.Italy, flags.Collection["Italy"]);
					defaultShapes.Add(Flags.Spain, flags.Collection["Spain"]);
					defaultShapes.Add(Flags.Canada, flags.Collection["Canada"]);
					defaultShapes.Add(Flags.SouthKorea, flags.Collection["SouthKorea"]);
					defaultShapes.Add(Flags.Iran, flags.Collection["Iran"]);
					defaultShapes.Add(Flags.Indonesia, flags.Collection["Indonesia"]);
					defaultShapes.Add(Flags.Australia, flags.Collection["Australia"]);
					defaultShapes.Add(Flags.Taiwan, flags.Collection["Taiwan"]);
					defaultShapes.Add(Flags.Turkey, flags.Collection["Turkey"]);
					defaultShapes.Add(Flags.Netherlands, flags.Collection["Netherlands"]);
					break;
				case StateIndicatorShapeType.Currency:
				case StateIndicatorShapeType.CurrencyUSD:
				case StateIndicatorShapeType.CurrencyCent:
				case StateIndicatorShapeType.CurrencyEUR:
				case StateIndicatorShapeType.CurrencyUAH:
				case StateIndicatorShapeType.CurrencyGBP:
				case StateIndicatorShapeType.CurrencyJPY:
				case StateIndicatorShapeType.CurrencyITL:
				case StateIndicatorShapeType.CurrencyESP:
				case StateIndicatorShapeType.CurrencyFRF:
				case StateIndicatorShapeType.CurrencyNLG:
				case StateIndicatorShapeType.CurrencyINR:
				case StateIndicatorShapeType.CurrencyBRR:
				case StateIndicatorShapeType.CurrencyILS:
				case StateIndicatorShapeType.CurrencyMNT:
				case StateIndicatorShapeType.CurrencyKRW:
				case StateIndicatorShapeType.CurrencyCRC:
				case StateIndicatorShapeType.CurrencyNGN:
				case StateIndicatorShapeType.CurrencyLAK:
				case StateIndicatorShapeType.CurrencyRUR:
					ComplexShape currency = XAMLResoucesFactory.GetShape("Currency") as ComplexShape;
					defaultShapes.Add(Currency.General, currency.Collection["GenericCurencySymbol"]);
					defaultShapes.Add(Currency.USD, currency.Collection["USD"]);
					defaultShapes.Add(Currency.Cent, currency.Collection["Cent"]);
					defaultShapes.Add(Currency.EUR, currency.Collection["EUR"]);
					defaultShapes.Add(Currency.UAH, currency.Collection["UAH"]);
					defaultShapes.Add(Currency.GBP, currency.Collection["GBP"]);
					defaultShapes.Add(Currency.JPY, currency.Collection["JPY"]);
					defaultShapes.Add(Currency.ITL, currency.Collection["ITL"]);
					defaultShapes.Add(Currency.ESP, currency.Collection["ESP"]);
					defaultShapes.Add(Currency.FRF, currency.Collection["FRF"]);
					defaultShapes.Add(Currency.NLG, currency.Collection["NLG"]);
					defaultShapes.Add(Currency.INR, currency.Collection["INR"]);
					defaultShapes.Add(Currency.BRR, currency.Collection["BRR"]);
					defaultShapes.Add(Currency.ILS, currency.Collection["ILS"]);
					defaultShapes.Add(Currency.MNT, currency.Collection["MNT"]);
					defaultShapes.Add(Currency.KRW, currency.Collection["KRW"]);
					defaultShapes.Add(Currency.CRC, currency.Collection["CRC"]);
					defaultShapes.Add(Currency.NGN, currency.Collection["NGN"]);
					defaultShapes.Add(Currency.LAK, currency.Collection["LAK"]);
					defaultShapes.Add(Currency.RUR, currency.Collection["RUR"]);
					break;
				case StateIndicatorShapeType.Equalizer0:
				case StateIndicatorShapeType.Equalizer1:
				case StateIndicatorShapeType.Equalizer2:
				case StateIndicatorShapeType.Equalizer3:
				case StateIndicatorShapeType.Equalizer4:
					ComplexShape equalizerShapes = XAMLResoucesFactory.GetShape("EqualizerShapes") as ComplexShape;
					defaultShapes.Add(EqualizerShapes.EmptyBar, equalizerShapes.Collection[PredefinedShapeNames.IndicatorState + "0"]);
					defaultShapes.Add(EqualizerShapes.GrayBar, equalizerShapes.Collection[PredefinedShapeNames.IndicatorState + "1"]);
					defaultShapes.Add(EqualizerShapes.RedBar, equalizerShapes.Collection[PredefinedShapeNames.IndicatorState + "2"]);
					defaultShapes.Add(EqualizerShapes.AmberBar, equalizerShapes.Collection[PredefinedShapeNames.IndicatorState + "3"]);
					defaultShapes.Add(EqualizerShapes.GreenBar, equalizerShapes.Collection[PredefinedShapeNames.IndicatorState + "4"]);
					break;
				case StateIndicatorShapeType.CarBattery1:
				case StateIndicatorShapeType.CarBattery2:
				case StateIndicatorShapeType.CarBattery3:
				case StateIndicatorShapeType.CarBattery4:
				case StateIndicatorShapeType.CarABS1:
				case StateIndicatorShapeType.CarABS2:
				case StateIndicatorShapeType.CarABS3:
				case StateIndicatorShapeType.CarABS4:
				case StateIndicatorShapeType.CarEngine1:
				case StateIndicatorShapeType.CarEngine2:
				case StateIndicatorShapeType.CarEngine3:
				case StateIndicatorShapeType.CarEngine4:
				case StateIndicatorShapeType.CarParkingLights1:
				case StateIndicatorShapeType.CarParkingLights2:
				case StateIndicatorShapeType.CarParkingLights3:
				case StateIndicatorShapeType.CarParkingLights4:
				case StateIndicatorShapeType.CarLowBeam1:
				case StateIndicatorShapeType.CarLowBeam2:
				case StateIndicatorShapeType.CarLowBeam3:
				case StateIndicatorShapeType.CarLowBeam4:
				case StateIndicatorShapeType.CarHiBeam1:
				case StateIndicatorShapeType.CarHiBeam2:
				case StateIndicatorShapeType.CarHiBeam3:
				case StateIndicatorShapeType.CarHiBeam4:
				case StateIndicatorShapeType.CarAirBag1:
				case StateIndicatorShapeType.CarAirBag2:
				case StateIndicatorShapeType.CarAirBag3:
				case StateIndicatorShapeType.CarAirBag4:
				case StateIndicatorShapeType.CarWater1:
				case StateIndicatorShapeType.CarWater2:
				case StateIndicatorShapeType.CarWater3:
				case StateIndicatorShapeType.CarWater4:
				case StateIndicatorShapeType.CarGas1:
				case StateIndicatorShapeType.CarGas2:
				case StateIndicatorShapeType.CarGas3:
				case StateIndicatorShapeType.CarGas4:
				case StateIndicatorShapeType.CarMotorOil1:
				case StateIndicatorShapeType.CarMotorOil2:
				case StateIndicatorShapeType.CarMotorOil3:
				case StateIndicatorShapeType.CarMotorOil4:
					ComplexShape carIconsShapes = XAMLResoucesFactory.GetShape("CarIconsShapes") as ComplexShape;
					defaultShapes.Add(CarIcons.BatteryRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".BatteryRed"]);
					defaultShapes.Add(CarIcons.BatteryOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".BatteryOrange"]);
					defaultShapes.Add(CarIcons.BatteryGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".BatteryGreen"]);
					defaultShapes.Add(CarIcons.BatteryGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".BatteryGray"]);
					defaultShapes.Add(CarIcons.ABSRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ABSRed"]);
					defaultShapes.Add(CarIcons.ABSOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ABSOrange"]);
					defaultShapes.Add(CarIcons.ABSGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ABSGreen"]);
					defaultShapes.Add(CarIcons.ABSGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ABSGray"]);
					defaultShapes.Add(CarIcons.EngineRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".EngineRed"]);
					defaultShapes.Add(CarIcons.EngineOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".EngineOrange"]);
					defaultShapes.Add(CarIcons.EngineGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".EngineGreen"]);
					defaultShapes.Add(CarIcons.EngineGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".EngineGray"]);
					defaultShapes.Add(CarIcons.ParkingLightsRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ParkingLightsRed"]);
					defaultShapes.Add(CarIcons.ParkingLightsOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ParkingLightsOrange"]);
					defaultShapes.Add(CarIcons.ParkingLightsGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ParkingLightsGreen"]);
					defaultShapes.Add(CarIcons.ParkingLightsGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".ParkingLightsGray"]);
					defaultShapes.Add(CarIcons.LowBeamRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".LowBeamRed"]);
					defaultShapes.Add(CarIcons.LowBeamOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".LowBeamOrange"]);
					defaultShapes.Add(CarIcons.LowBeamGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".LowBeamGreen"]);
					defaultShapes.Add(CarIcons.LowBeamGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".LowBeamGray"]);
					defaultShapes.Add(CarIcons.HiBeamRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".HiBeamRed"]);
					defaultShapes.Add(CarIcons.HiBeamOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".HiBeamOrange"]);
					defaultShapes.Add(CarIcons.HiBeamGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".HiBeamGreen"]);
					defaultShapes.Add(CarIcons.HiBeamGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".HiBeamGray"]);
					defaultShapes.Add(CarIcons.AirBagRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".AirBagRed"]);
					defaultShapes.Add(CarIcons.AirBagOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".AirBagOrange"]);
					defaultShapes.Add(CarIcons.AirBagGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".AirBagGreen"]);
					defaultShapes.Add(CarIcons.AirBagGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".AirBagGray"]);
					defaultShapes.Add(CarIcons.WaterRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".WaterRed"]);
					defaultShapes.Add(CarIcons.WaterOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".WaterOrange"]);
					defaultShapes.Add(CarIcons.WaterGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".WaterGreen"]);
					defaultShapes.Add(CarIcons.WaterGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".WaterGray"]);
					defaultShapes.Add(CarIcons.GasRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".GasRed"]);
					defaultShapes.Add(CarIcons.GasOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".GasOrange"]);
					defaultShapes.Add(CarIcons.GasGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".GasGreen"]);
					defaultShapes.Add(CarIcons.GasGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".GasGray"]);
					defaultShapes.Add(CarIcons.MotorOilRed, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".MotorOilRed"]);
					defaultShapes.Add(CarIcons.MotorOilOrange, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".MotorOilOrange"]);
					defaultShapes.Add(CarIcons.MotorOilGreen, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".MotorOilGreen"]);
					defaultShapes.Add(CarIcons.MotorOilGray, carIconsShapes.Collection[PredefinedShapeNames.IndicatorState + ".MotorOilGray"]);
					break;
				case StateIndicatorShapeType.ProgressItem0:
				case StateIndicatorShapeType.ProgressItem1:
				case StateIndicatorShapeType.ProgressItem2:
				case StateIndicatorShapeType.ProgressItem3:
				case StateIndicatorShapeType.ProgressItem4:
				case StateIndicatorShapeType.ProgressItem5:
				case StateIndicatorShapeType.ProgressItem6:
				case StateIndicatorShapeType.ProgressItem7:
				case StateIndicatorShapeType.ProgressItem8:
				case StateIndicatorShapeType.ProgressItem9:
				case StateIndicatorShapeType.ProgressItem10:
				case StateIndicatorShapeType.ProgressItem11:
				case StateIndicatorShapeType.ProgressItem12:
				case StateIndicatorShapeType.ProgressItem13:
				case StateIndicatorShapeType.ProgressItem14:
					ComplexShape progressItemShapes = XAMLResoucesFactory.GetShape("ProgressShapes") as ComplexShape;
					defaultShapes.Add(ProgressItemShapes.GrayBar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".GrayBar"]);
					defaultShapes.Add(ProgressItemShapes.RedBar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".RedBar"]);
					defaultShapes.Add(ProgressItemShapes.OrangeBar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".OrangeBar"]);
					defaultShapes.Add(ProgressItemShapes.LimeBar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".LimeBar"]);
					defaultShapes.Add(ProgressItemShapes.BlueBar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".BlueBar"]);
					defaultShapes.Add(ProgressItemShapes.GrayCircle, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".GrayCircle"]);
					defaultShapes.Add(ProgressItemShapes.RedCircle, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".RedCircle"]);
					defaultShapes.Add(ProgressItemShapes.OrangeCircle, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".OrangeCircle"]);
					defaultShapes.Add(ProgressItemShapes.LimeCircle, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".LimeCircle"]);
					defaultShapes.Add(ProgressItemShapes.BlueCircle, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".BlueCircle"]);
					defaultShapes.Add(ProgressItemShapes.GrayStar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".GrayStar"]);
					defaultShapes.Add(ProgressItemShapes.RedStar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".RedStar"]);
					defaultShapes.Add(ProgressItemShapes.OrangeStar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".OrangeStar"]);
					defaultShapes.Add(ProgressItemShapes.LimeStar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".LimeStar"]);
					defaultShapes.Add(ProgressItemShapes.BlueStar, progressItemShapes.Collection[PredefinedShapeNames.IndicatorState + ".BlueStar"]);
					break;
				case StateIndicatorShapeType.DashboardTrendArrowUp:
				case StateIndicatorShapeType.DashboardTrendArrowDown:
				case StateIndicatorShapeType.DashboardWarning:
					ComplexShape dashboardIndicators = XAMLResoucesFactory.GetShape("DashboardIndicator") as ComplexShape;
					defaultShapes.Add(DashboardIndicators.TrendUp, dashboardIndicators.Collection[PredefinedShapeNames.DashboardIndicatorState + "0"]);
					defaultShapes.Add(DashboardIndicators.TrendDown, dashboardIndicators.Collection[PredefinedShapeNames.DashboardIndicatorState + "1"]);
					defaultShapes.Add(DashboardIndicators.Warning, dashboardIndicators.Collection[PredefinedShapeNames.DashboardIndicatorState + "2"]);
					break;
			}
		}
		public static BaseShape GetIndicatorStateShape(StateIndicatorShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapeCore(type);
					shape = defaultShapes[type];
				}
				shape.Name = PredefinedShapeNames.IndicatorState;
				return shape.Clone();
			}
		}
	}
	public static class TickmarkShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<TickmarkShapeType, BaseShape> defaultShapes;
		static TickmarkShapeFactory() {
			defaultShapes = new Dictionary<TickmarkShapeType, BaseShape>();
		}
		private static void LoadShapeCore(TickmarkShapeType type) {
			switch(type) {
				case TickmarkShapeType.Default:
				case TickmarkShapeType.Line:
					BaseShape defaultShape = new PolylineShape(
						new PointF[] { 
						new PointF(-5, 0), new PointF(5, 0) }
					);
					defaultShape.Name = "DefaultTickmark";
					defaultShape.Bounds = new RectangleF2D(-5, 0, 10, 10);
					defaultShape.Appearance.BorderBrush = new SolidBrushObject(Color.White);
					defaultShape.Appearance.ContentBrush = new SolidBrushObject(Color.White);
					defaultShapes.Add(TickmarkShapeType.Default, defaultShape);
					defaultShapes.Add(TickmarkShapeType.Line, defaultShape);
					break;
				case TickmarkShapeType.Box:
					BaseShape boxShape = new BoxShape(new RectangleF2D(-5f, -5f, 10, 10));
					boxShape.Name = "DefaultTickmark";
					boxShape.Bounds = new RectangleF2D(-5f, -5f, 10, 10);
					boxShape.Appearance.BorderBrush = new SolidBrushObject(Color.White);
					boxShape.Appearance.ContentBrush = new SolidBrushObject(Color.White);
					defaultShapes.Add(TickmarkShapeType.Box, boxShape);
					break;
				case TickmarkShapeType.Diamond:
					BaseShape diamondShape = new PolygonShape(
							new PointF[] { 
							new PointF(-5.0f, 0.0f) ,
							new PointF(0.0f, 2.5f) ,
							new PointF(5f, 0f) ,
							new PointF(0.0f, -2.5f)
							}
						);
					diamondShape.Name = "DefaultTickmark";
					diamondShape.Bounds = new RectangleF2D(-5f, -2.5f, 10f, 5f);
					diamondShape.Appearance.BorderBrush = new SolidBrushObject(Color.White);
					diamondShape.Appearance.ContentBrush = new SolidBrushObject(Color.White);
					defaultShapes.Add(TickmarkShapeType.Diamond, diamondShape);
					break;
				case TickmarkShapeType.Circular_Default1: LoadShapes("Circular.Full.Style1", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Default2: LoadShapes("Circular.Full.Style1", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Default3: LoadShapes("Circular.Full.Style1", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Default4: LoadShapes("Circular.Full.Style1", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style1_1: LoadShapes("Circular.Full.Style1", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style1_2: LoadShapes("Circular.Full.Style1", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style1_3: LoadShapes("Circular.Full.Style1", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style1_4: LoadShapes("Circular.Full.Style1", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style2_1: LoadShapes("Circular.Full.Style2", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style2_2: LoadShapes("Circular.Full.Style2", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style3_1: LoadShapes("Circular.Full.Style3", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style3_2: LoadShapes("Circular.Full.Style3", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style3_3: LoadShapes("Circular.Full.Style3", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style3_4: LoadShapes("Circular.Full.Style3", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style4_1: LoadShapes("Circular.Full.Style4", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style4_2: LoadShapes("Circular.Full.Style4", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style4_3: LoadShapes("Circular.Full.Style4", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style4_4: LoadShapes("Circular.Full.Style4", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style5_1: LoadShapes("Circular.Full.Style5", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style5_2: LoadShapes("Circular.Full.Style5", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style5_3: LoadShapes("Circular.Full.Style5", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style5_4: LoadShapes("Circular.Full.Style5", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style5_5: LoadShapes("Circular.Full.Style5", new string[] { "5" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style6_1: LoadShapes("Circular.Full.Style6", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style6_2: LoadShapes("Circular.Full.Style6", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style6_3: LoadShapes("Circular.Full.Style6", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style6_4: LoadShapes("Circular.Full.Style6", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style6_5: LoadShapes("Circular.Full.Style6", new string[] { "5" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style7_1: LoadShapes("Circular.Full.Style7", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style7_2: LoadShapes("Circular.Full.Style7", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style8_1: LoadShapes("Circular.Full.Style8", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style8_2: LoadShapes("Circular.Full.Style8", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style8_3: LoadShapes("Circular.Full.Style8", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style8_4: LoadShapes("Circular.Full.Style8", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style8_5: LoadShapes("Circular.Full.Style8", new string[] { "5" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style9_1: LoadShapes("Circular.Full.Style9", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style9_2: LoadShapes("Circular.Full.Style9", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style10_1: LoadShapes("Circular.Full.Style10", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style10_2: LoadShapes("Circular.Full.Style10", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style10_3: LoadShapes("Circular.Full.Style10", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style10_4: LoadShapes("Circular.Full.Style10", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style11_1: LoadShapes("Circular.Full.Style11", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style11_2: LoadShapes("Circular.Full.Style11", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style11_3: LoadShapes("Circular.Full.Style11", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style11_4: LoadShapes("Circular.Full.Style11", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style12_1: LoadShapes("Circular.Full.Style12", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style12_2: LoadShapes("Circular.Full.Style12", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style12_3: LoadShapes("Circular.Full.Style12", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style12_4: LoadShapes("Circular.Full.Style12", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style13_1: LoadShapes("Circular.Full.Style13", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style13_2: LoadShapes("Circular.Full.Style13", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style13_3: LoadShapes("Circular.Full.Style13", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style13_4: LoadShapes("Circular.Full.Style13", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style13_5: LoadShapes("Circular.Full.Style13", new string[] { "5" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style14_1: LoadShapes("Circular.Full.Style14", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style14_2: LoadShapes("Circular.Full.Style14", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style14_3: LoadShapes("Circular.Full.Style14", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style14_4: LoadShapes("Circular.Full.Style14", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style14_5: LoadShapes("Circular.Full.Style14", new string[] { "5" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style15_1: LoadShapes("Circular.Full.Style15", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style15_2: LoadShapes("Circular.Full.Style15", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style15_3: LoadShapes("Circular.Full.Style15", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style15_4: LoadShapes("Circular.Full.Style15", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style16_1: LoadShapes("Circular.Full.Style16", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style16_2: LoadShapes("Circular.Full.Style16", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style17_1: LoadShapes("Circular.Full.Style17", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style17_2: LoadShapes("Circular.Full.Style17", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style18_1: LoadShapes("Circular.Full.Style18", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style18_2: LoadShapes("Circular.Full.Style18", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style19_1: LoadShapes("Circular.Full.Style19", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style19_2: LoadShapes("Circular.Full.Style19", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style20_1: LoadShapes("Circular.Full.Style20", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style20_2: LoadShapes("Circular.Full.Style20", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style21_1: LoadShapes("Circular.Full.Style21", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style21_2: LoadShapes("Circular.Full.Style21", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style22_1: LoadShapes("Circular.Full.Style22", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style22_2: LoadShapes("Circular.Full.Style22", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style23_1: LoadShapes("Circular.Full.Style23", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style23_2: LoadShapes("Circular.Full.Style23", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style24_1: LoadShapes("Circular.Full.Style24", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style24_2: LoadShapes("Circular.Full.Style24", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style25_1: LoadShapes("Circular.Full.Style25", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style25_2: LoadShapes("Circular.Full.Style25", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style26_1: LoadShapes("Circular.Full.Style26", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style26_2: LoadShapes("Circular.Full.Style26", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style27_1: LoadShapes("Circular.Full.Style27", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Circular_Style28_1: LoadShapes("Circular.Full.Style28", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Default1: LoadShapes("Linear.Style1", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Default2: LoadShapes("Linear.Style1", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style1_1: LoadShapes("Linear.Style1", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style1_2: LoadShapes("Linear.Style1", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style2_1: LoadShapes("Linear.Style2", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style2_2: LoadShapes("Linear.Style2", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style3_1: LoadShapes("Linear.Style3", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style3_2: LoadShapes("Linear.Style3", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style4_1: LoadShapes("Linear.Style4", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style4_2: LoadShapes("Linear.Style4", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style5_1: LoadShapes("Linear.Style5", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style5_2: LoadShapes("Linear.Style5", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style5_3: LoadShapes("Linear.Style5", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style6_1: LoadShapes("Linear.Style6", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style6_2: LoadShapes("Linear.Style6", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style6_3: LoadShapes("Linear.Style6", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style7_1: LoadShapes("Linear.Style7", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style7_2: LoadShapes("Linear.Style7", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style7_3: LoadShapes("Linear.Style7", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style8_1: LoadShapes("Linear.Style8", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style8_2: LoadShapes("Linear.Style8", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style8_3: LoadShapes("Linear.Style8", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style9_1: LoadShapes("Linear.Style9", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style9_2: LoadShapes("Linear.Style9", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style9_3: LoadShapes("Linear.Style9", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style10_1: LoadShapes("Linear.Style10", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style10_2: LoadShapes("Linear.Style10", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style10_3: LoadShapes("Linear.Style10", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style11_1: LoadShapes("Linear.Style11", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style11_2: LoadShapes("Linear.Style11", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style11_3: LoadShapes("Linear.Style11", new string[] { "3" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style11_4: LoadShapes("Linear.Style11", new string[] { "4" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style12_1: LoadShapes("Linear.Style12", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style12_2: LoadShapes("Linear.Style12", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style13_1: LoadShapes("Linear.Style13", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style13_2: LoadShapes("Linear.Style13", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style14_1: LoadShapes("Linear.Style14", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style14_2: LoadShapes("Linear.Style14", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style15_1: LoadShapes("Linear.Style15", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style15_2: LoadShapes("Linear.Style15", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style16_1: LoadShapes("Linear.Style16", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style16_2: LoadShapes("Linear.Style16", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style17_1: LoadShapes("Linear.Style17", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style17_2: LoadShapes("Linear.Style17", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style18_1: LoadShapes("Linear.Style18", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style18_2: LoadShapes("Linear.Style18", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style19_1: LoadShapes("Linear.Style19", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style19_2: LoadShapes("Linear.Style19", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style20_1: LoadShapes("Linear.Style20", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style20_2: LoadShapes("Linear.Style20", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style21_1: LoadShapes("Linear.Style21", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style21_2: LoadShapes("Linear.Style21", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style22_1: LoadShapes("Linear.Style22", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style22_2: LoadShapes("Linear.Style22", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style23_1: LoadShapes("Linear.Style23", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style23_2: LoadShapes("Linear.Style23", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style24_1: LoadShapes("Linear.Style24", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style24_2: LoadShapes("Linear.Style24", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style25_1: LoadShapes("Linear.Style25", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style25_2: LoadShapes("Linear.Style25", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style26_1: LoadShapes("Linear.Style26", new string[] { "1" }, new TickmarkShapeType[] { type }); break;
				case TickmarkShapeType.Linear_Style26_2: LoadShapes("Linear.Style26", new string[] { "2" }, new TickmarkShapeType[] { type }); break;
			}
		}
		public static BaseShape GetDefaultTickmarkShape(TickmarkShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapeCore(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
		static void LoadShapes(string template, string[] postfixes, TickmarkShapeType[] types) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			for(int i = 0; i < postfixes.Length; i++) {
				BaseShape tickShape = templateShape.Collection[PredefinedShapeNames.Tick + postfixes[i]];
				defaultShapes.Add(types[i], tickShape);
			}
		}
	}
	public static class MarkerPointerShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<MarkerPointerShapeType, BaseShape> defaultShapes;
		static MarkerPointerShapeFactory() {
			defaultShapes = new Dictionary<MarkerPointerShapeType, BaseShape>();
			LoadShape("Markers", "1", MarkerPointerShapeType.Default);
			LoadShape("Markers", "1", MarkerPointerShapeType.TriangleLeft);
			LoadShape("Markers", "2", MarkerPointerShapeType.SliderLeft);
			LoadShape("Markers", "3", MarkerPointerShapeType.Circle);
			LoadShape("Markers", "4", MarkerPointerShapeType.WedgeLeft);
			LoadShape("Markers", "5", MarkerPointerShapeType.Diamond);
			LoadShape("Markers", "6", MarkerPointerShapeType.ArrowLeft);
			LoadShape("Markers", "7", MarkerPointerShapeType.Box);
			LoadShape("Markers", "8", MarkerPointerShapeType.Star);
			LoadShape("Markers", "9", MarkerPointerShapeType.Button);
			LoadShape("Markers", "10", MarkerPointerShapeType.SnowFlake);
			LoadShape("Markers", "11", MarkerPointerShapeType.TriangleRight);
			LoadShape("Markers", "12", MarkerPointerShapeType.SliderRight);
			LoadShape("Markers", "13", MarkerPointerShapeType.WedgeRight);
			LoadShape("Markers", "14", MarkerPointerShapeType.ArrowRight);
		}
		public static BaseShape GetDefaultMarkerShape(MarkerPointerShapeType type) {
			lock(syncObj) {
				return defaultShapes[type].Clone();
			}
		}
		static void LoadShape(string template, string postfix, MarkerPointerShapeType type) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			BaseShape markerShape = templateShape.Collection[PredefinedShapeNames.Marker + postfix];
			NormalizeMarkerShape(markerShape);
			defaultShapes.Add(type, markerShape);
		}
		static void NormalizeMarkerShape(BaseShape markerShape) {
			RectangleF box = markerShape.BoundingBox;
			Matrix transform = new Matrix(1, 0, 0, 1, -box.Left - box.Width * 0.5f, -box.Top - box.Height * 0.5f);
			markerShape.Accept(
					delegate(BaseShape s) {
						s.BeginUpdate();
						s.Transform.Multiply(transform);
						s.EndUpdate();
					}
				);
			markerShape.Name = PredefinedShapeNames.Marker;
			transform.Dispose();
		}
	}
	public static class ScaleFactory {
		public static readonly ILinearScale EmptyLinearScale;
		public static readonly IArcScale EmptyArcScale;
		static ScaleFactory() {
			EmptyArcScale = new ArcScaleProvider(null);
			((ArcScaleProvider)EmptyArcScale).InitLabelsInternal(null);
			((ArcScaleProvider)EmptyArcScale).InitRangesInternal(null);
			EmptyLinearScale = new LinearScaleProvider(null);
			((LinearScaleProvider)EmptyLinearScale).InitLabelsInternal(null);
			((LinearScaleProvider)EmptyLinearScale).InitRangesInternal(null);
		}
	}
	public static class NeedleShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<NeedleShapeType, BaseShape> defaultShapes;
		static NeedleShapeFactory() {
			defaultShapes = new Dictionary<NeedleShapeType, BaseShape>();
		}
		static void LoadShapeCore(NeedleShapeType type) {
			switch (type) {
				case NeedleShapeType.Circular_Default: LoadShape("Circular.Full.Style1", type); break;
				case NeedleShapeType.CircularFull_Style1: LoadShape("Circular.Full.Style1", type); break;
				case NeedleShapeType.CircularFull_Style2: LoadShape("Circular.Full.Style2", type); break;
				case NeedleShapeType.CircularFull_Style3: LoadShape("Circular.Full.Style3", type); break;
				case NeedleShapeType.CircularFull_Style4: LoadShape("Circular.Full.Style4", type); break;
				case NeedleShapeType.CircularFull_Style5: LoadShape("Circular.Full.Style5", type); break;
				case NeedleShapeType.CircularFull_Style6: LoadShape("Circular.Full.Style6", type); break;
				case NeedleShapeType.CircularFull_Style7: LoadShape("Circular.Full.Style7", type); break;
				case NeedleShapeType.CircularFull_Style8: LoadShape("Circular.Full.Style8", type); break;
				case NeedleShapeType.CircularFull_Style9: LoadShape("Circular.Full.Style9", type); break;
				case NeedleShapeType.CircularFull_Style10: LoadShape("Circular.Full.Style10", type); break;
				case NeedleShapeType.CircularFull_Style11: LoadShape("Circular.Full.Style11", type); break;
				case NeedleShapeType.CircularFull_Style12: LoadShape("Circular.Full.Style12", type); break;
				case NeedleShapeType.CircularFull_Style13: LoadShape("Circular.Full.Style13", type); break;
				case NeedleShapeType.CircularFull_Style14: LoadShape("Circular.Full.Style14", type); break;
				case NeedleShapeType.CircularFull_Style15: LoadShape("Circular.Full.Style15", type); break;
				case NeedleShapeType.CircularFull_Style16: LoadShape("Circular.Full.Style16", type); break;
				case NeedleShapeType.CircularFull_Style17: LoadShape("Circular.Full.Style17", type); break;
				case NeedleShapeType.CircularFull_Style18: LoadShape("Circular.Full.Style18", type); break;
				case NeedleShapeType.CircularFull_Style19: LoadShape("Circular.Full.Style19", type); break;
				case NeedleShapeType.CircularFull_Style20: LoadShape("Circular.Full.Style20", type); break;
				case NeedleShapeType.CircularFull_Style21: LoadShape("Circular.Full.Style21", type); break;
				case NeedleShapeType.CircularFull_Style22: LoadShape("Circular.Full.Style22", type); break;
				case NeedleShapeType.CircularFull_Style23: LoadShape("Circular.Full.Style23", type); break;
				case NeedleShapeType.CircularFull_Style24: LoadShape("Circular.Full.Style24", type); break;
				case NeedleShapeType.CircularFull_Style25: LoadShape("Circular.Full.Style25", type); break;
				case NeedleShapeType.CircularFull_Style26: LoadShape("Circular.Full.Style26", type); break;
				case NeedleShapeType.CircularFull_Style27: LoadShape("Circular.Full.Style27", type); break;
				case NeedleShapeType.CircularFull_Style28: LoadShape("Circular.Full.Style28", type); break;
				case NeedleShapeType.CircularHalf_Style23: LoadShape("Circular.Half.Style23", type); break;
				case NeedleShapeType.CircularFull_Clock: LoadShape("Temp.Demo.demo_360", type); break;
				case NeedleShapeType.CircularFull_ClockHour: LoadShape("Temp.Demo.demo_360", type, "1"); break;
				case NeedleShapeType.CircularFull_ClockMinute: LoadShape("Temp.Demo.demo_360", type, "2"); break;
				case NeedleShapeType.CircularFull_ClockSecond: LoadShape("Temp.Demo.demo_360", type, "3"); break;
				case NeedleShapeType.CircularWide_Style1: LoadShape("Circular.Wide.Style1", type); break;
				case NeedleShapeType.CircularWide_Style2: LoadShape("Circular.Wide.Style2", type); break;
				case NeedleShapeType.CircularWide_Style3: LoadShape("Circular.Wide.Style3", type); break;
				case NeedleShapeType.CircularWide_Style4: LoadShape("Circular.Wide.Style4", type); break;
				case NeedleShapeType.CircularWide_Style5: LoadShape("Circular.Wide.Style5", type); break;
				case NeedleShapeType.CircularWide_Style6: LoadShape("Circular.Wide.Style6", type); break;
				case NeedleShapeType.CircularWide_Style7: LoadShape("Circular.Wide.Style7", type); break;
				case NeedleShapeType.CircularWide_Style8: LoadShape("Circular.Wide.Style8", type); break;
				case NeedleShapeType.CircularWide_Style9: LoadShape("Circular.Wide.Style9", type); break;
				case NeedleShapeType.CircularWide_Style10: LoadShape("Circular.Wide.Style10", type); break;
				case NeedleShapeType.CircularWide_Style11: LoadShape("Circular.Wide.Style11", type); break;
				case NeedleShapeType.CircularWide_Style12: LoadShape("Circular.Wide.Style12", type); break;
				case NeedleShapeType.CircularWide_Style13: LoadShape("Circular.Wide.Style13", type); break;
				case NeedleShapeType.CircularWide_Style14: LoadShape("Circular.Wide.Style14", type); break;
				case NeedleShapeType.CircularWide_Style15: LoadShape("Circular.Wide.Style15", type); break;
			}
		}
		public static BaseShape GetDefaultNeedleShape(NeedleShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapeCore(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
		static void LoadShape(string template, NeedleShapeType type) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			BaseShape needleShape = templateShape.Collection[PredefinedShapeNames.Needle];
			needleShape.Name = PredefinedShapeNames.Needle;
			defaultShapes.Add(type, needleShape);
		}
		static void LoadShape(string template, NeedleShapeType type, string suffix) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			BaseShape needleShape = templateShape.Collection[PredefinedShapeNames.Needle + suffix];
			needleShape.Name = PredefinedShapeNames.Needle;
			defaultShapes.Add(type, needleShape);
		}
	}
	public static class ScaleLevelShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<LevelShapeSetType, ComplexShape> defaultShapes;
		static ScaleLevelShapeFactory() {
			defaultShapes = new Dictionary<LevelShapeSetType, ComplexShape>();
		}
		static void LoadShapeCore(LevelShapeSetType shapeSet) {
			switch (shapeSet) {
				case LevelShapeSetType.Default: LoadShapes("Linear.Style1", shapeSet); break;
				case LevelShapeSetType.Style1: LoadShapes("Linear.Style1", shapeSet); break;
				case LevelShapeSetType.Style2: LoadShapes("Linear.Style2", shapeSet); break;
				case LevelShapeSetType.Style3: LoadShapes("Linear.Style3", shapeSet); break;
				case LevelShapeSetType.Style4: LoadShapes("Linear.Style4", shapeSet); break;
				case LevelShapeSetType.Style5: LoadShapes("Linear.Style5", shapeSet); break;
				case LevelShapeSetType.Style6: LoadShapes("Linear.Style6", shapeSet); break;
				case LevelShapeSetType.Style7: LoadShapes("Linear.Style7", shapeSet); break;
				case LevelShapeSetType.Style8: LoadShapes("Linear.Style8", shapeSet); break;
				case LevelShapeSetType.Style9: LoadShapes("Linear.Style9", shapeSet); break;
				case LevelShapeSetType.Style10: LoadShapes("Linear.Style10", shapeSet); break;
				case LevelShapeSetType.Style11: LoadShapes("Linear.Style11", shapeSet); break;
				case LevelShapeSetType.Style12: LoadShapes("Linear.Style12", shapeSet); break;
				case LevelShapeSetType.Style13: LoadShapes("Linear.Style13", shapeSet); break;
				case LevelShapeSetType.Style14: LoadShapes("Linear.Style14", shapeSet); break;
				case LevelShapeSetType.Style15: LoadShapes("Linear.Style15", shapeSet); break;
				case LevelShapeSetType.Style16: LoadShapes("Linear.Style16", shapeSet); break;
				case LevelShapeSetType.Style17: LoadShapes("Linear.Style17", shapeSet); break;
				case LevelShapeSetType.Style18: LoadShapes("Linear.Style18", shapeSet); break;
				case LevelShapeSetType.Style19: LoadShapes("Linear.Style19", shapeSet); break;
				case LevelShapeSetType.Style20: LoadShapes("Linear.Style20", shapeSet); break;
				case LevelShapeSetType.Style21: LoadShapes("Linear.Style21", shapeSet); break;
				case LevelShapeSetType.Style22: LoadShapes("Linear.Style22", shapeSet); break;
				case LevelShapeSetType.Style23: LoadShapes("Linear.Style23", shapeSet); break;
				case LevelShapeSetType.Style24: LoadShapes("Linear.Style24", shapeSet); break;
				case LevelShapeSetType.Style25: LoadShapes("Linear.Style25", shapeSet); break;
				case LevelShapeSetType.Style26: LoadShapes("Linear.Style26", shapeSet); break;
			}
		}
		public static BaseShape GetDefaultLevelShape(LevelShapeSetType shapeSet) {
			lock(syncObj) {
				ComplexShape shape;
				if(!defaultShapes.TryGetValue(shapeSet, out shape)) {
					LoadShapeCore(shapeSet);
					shape = defaultShapes[shapeSet];
				}
				return shape.Clone();
			}
		}
		static void LoadShapes(string shapeName, LevelShapeSetType shapeSetType) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(shapeName) as ComplexShape;
			defaultShapes.Add(shapeSetType, templateShape);
		}
	}
	public static class DigitalBackgroundShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<DigitalBackgroundShapeSetType, ComplexShape> defaultShapes;
		static void LoadShape(string shapeName, DigitalBackgroundShapeSetType bsType) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(shapeName) as ComplexShape;
			defaultShapes.Add(bsType, templateShape);
		}
		static DigitalBackgroundShapeFactory() {
			defaultShapes = new Dictionary<DigitalBackgroundShapeSetType, ComplexShape>();
		}
		static void LoadShapeCore(DigitalBackgroundShapeSetType type) {
			switch (type) {
				case DigitalBackgroundShapeSetType.Default: LoadShape("BGDigital_2", type); break;
				case DigitalBackgroundShapeSetType.Style1: LoadShape("BGDigital_1", type); break;
				case DigitalBackgroundShapeSetType.Style2: LoadShape("BGDigital_2", type); break;
				case DigitalBackgroundShapeSetType.Style3: LoadShape("BGDigital_3", type); break;
				case DigitalBackgroundShapeSetType.Style4: LoadShape("BGDigital_4", type); break;
				case DigitalBackgroundShapeSetType.Style5: LoadShape("BGDigital_5", type); break;
				case DigitalBackgroundShapeSetType.Style6: LoadShape("BGDigital_6", type); break;
				case DigitalBackgroundShapeSetType.Style7: LoadShape("BGDigital_7", type); break;
				case DigitalBackgroundShapeSetType.Style8: LoadShape("BGDigital_8", type); break;
				case DigitalBackgroundShapeSetType.Style9: LoadShape("BGDigital_9", type); break;
				case DigitalBackgroundShapeSetType.Style10: LoadShape("BGDigital_10", type); break;
				case DigitalBackgroundShapeSetType.Style11: LoadShape("BGDigital_11", type); break;
				case DigitalBackgroundShapeSetType.Style12: LoadShape("BGDigital_12", type); break;
				case DigitalBackgroundShapeSetType.Style13: LoadShape("BGDigital_13", type); break;
				case DigitalBackgroundShapeSetType.Style14: LoadShape("BGDigital_14", type); break;
				case DigitalBackgroundShapeSetType.Style15: LoadShape("BGDigital_15", type); break;
				case DigitalBackgroundShapeSetType.Style16: LoadShape("BGDigital_16", type); break;
				case DigitalBackgroundShapeSetType.Style17: LoadShape("BGDigital_17", type); break;
				case DigitalBackgroundShapeSetType.Style18: LoadShape("BGDigital_18", type); break;
				case DigitalBackgroundShapeSetType.Style19: LoadShape("BGDigital_19", type); break;
				case DigitalBackgroundShapeSetType.Style20: LoadShape("BGDigital_20", type); break;
				case DigitalBackgroundShapeSetType.Style21: LoadShape("BGDigital_21", type); break;
				case DigitalBackgroundShapeSetType.Style22: LoadShape("BGDigital_22", type); break;
				case DigitalBackgroundShapeSetType.Style23: LoadShape("BGDigital_23", type); break;
				case DigitalBackgroundShapeSetType.Style24: LoadShape("BGDigital_24", type); break;
				case DigitalBackgroundShapeSetType.Style25: LoadShape("BGDigital_25", type); break;
				case DigitalBackgroundShapeSetType.Style26: LoadShape("BGDigital_26", type); break;
			}
		}
		public static BaseShape GetDigitalBackgroundShape(DigitalBackgroundShapeSetType shapeSet) {
			lock(syncObj) {
				ComplexShape shape;
				if(!defaultShapes.TryGetValue(shapeSet, out shape)) {
					LoadShapeCore(shapeSet);
					shape = defaultShapes[shapeSet];
				}
				return shape.Clone();
			}
		}
	}
	public static class DigitalEffectShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<DigitalEffectShapeType, BaseShape> defaultShapes;
		static void LoadShape(string shapeName, DigitalEffectShapeType esType) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(shapeName) as ComplexShape;
			BaseShape effectShape = (templateShape != null) ?
				templateShape.Collection[PredefinedShapeNames.Effect] : BaseShape.Empty;
			defaultShapes.Add(esType, effectShape);
		}
		static DigitalEffectShapeFactory() {
			defaultShapes = new Dictionary<DigitalEffectShapeType, BaseShape>();
		}
		private static void LoadShapes(DigitalEffectShapeType type) {
			switch (type) {
				case DigitalEffectShapeType.Empty: LoadShape("Empty", type); break;
				case DigitalEffectShapeType.Default: LoadShape("BGDigital_6", type); break;
				case DigitalEffectShapeType.Style6: LoadShape("BGDigital_6", type); break;
				case DigitalEffectShapeType.Style9: LoadShape("BGDigital_9", type); break;
			}
		}
		public static BaseShape GetDigitalEffectShape(DigitalEffectShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapes(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
	}
	public static class BackgroundLayerShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<BackgroundLayerShapeType, BaseShape> defaultShapes;
		static BackgroundLayerShapeFactory() {
			defaultShapes = new Dictionary<BackgroundLayerShapeType, BaseShape>();
		}
		private static void LoadShapesCore(BackgroundLayerShapeType shapeType) {
			switch (shapeType) {
				case BackgroundLayerShapeType.CircularFull_Default: LoadShape("Circular.Full.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style1: LoadShape("Circular.Full.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style2: LoadShape("Circular.Full.Style2", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style3: LoadShape("Circular.Full.Style3", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style4: LoadShape("Circular.Full.Style4", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style4_1: LoadShape("Circular.Full.Style4", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style5: LoadShape("Circular.Full.Style5", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style5_1: LoadShape("Circular.Full.Style5", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style6: LoadShape("Circular.Full.Style6", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style7: LoadShape("Circular.Full.Style7", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style8: LoadShape("Circular.Full.Style8", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style9: LoadShape("Circular.Full.Style9", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style10: LoadShape("Circular.Full.Style10", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style11: LoadShape("Circular.Full.Style11", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style12: LoadShape("Circular.Full.Style12", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style13: LoadShape("Circular.Full.Style13", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style14: LoadShape("Circular.Full.Style14", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style15: LoadShape("Circular.Full.Style15", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style16: LoadShape("Circular.Full.Style16", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style17: LoadShape("Circular.Full.Style17", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style18: LoadShape("Circular.Full.Style18", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style19: LoadShape("Circular.Full.Style19", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style20: LoadShape("Circular.Full.Style20", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style21: LoadShape("Circular.Full.Style21", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style22: LoadShape("Circular.Full.Style22", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style23: LoadShape("Circular.Full.Style23", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style24: LoadShape("Circular.Full.Style24", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style25: LoadShape("Circular.Full.Style25", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style26: LoadShape("Circular.Full.Style26", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style27: LoadShape("Circular.Full.Style27", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Style28: LoadShape("Circular.Full.Style28", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_Clock: LoadShape("Temp.Demo.demo_360", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_WorldTimeClock: LoadShape("Temp.Demo.demo_360", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularFull_SysInfoBack: LoadShape("Temp.Demo.Demo.SysInfo", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_CarPanelBack: LoadShape("Temp.Demo.CarPanel", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Default: LoadShape("Circular.Half.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style1: LoadShape("Circular.Half.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style2: LoadShape("Circular.Half.Style2", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style3: LoadShape("Circular.Half.Style3", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style4: LoadShape("Circular.Half.Style4", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style5: LoadShape("Circular.Half.Style5", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style6: LoadShape("Circular.Half.Style6", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style7: LoadShape("Circular.Half.Style7", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style8: LoadShape("Circular.Half.Style8", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style9: LoadShape("Circular.Half.Style9", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style10: LoadShape("Circular.Half.Style10", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style11: LoadShape("Circular.Half.Style11", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style12: LoadShape("Circular.Half.Style12", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style13: LoadShape("Circular.Half.Style13", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style14: LoadShape("Circular.Half.Style14", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style15: LoadShape("Circular.Half.Style15", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style16: LoadShape("Circular.Half.Style16", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style17: LoadShape("Circular.Half.Style17", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style18: LoadShape("Circular.Half.Style18", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style19: LoadShape("Circular.Half.Style19", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style20: LoadShape("Circular.Half.Style20", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style21: LoadShape("Circular.Half.Style21", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style22: LoadShape("Circular.Half.Style22", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style23: LoadShape("Circular.Half.Style23", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style24: LoadShape("Circular.Half.Style24", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style25: LoadShape("Circular.Half.Style25", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style26: LoadShape("Circular.Half.Style26", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style27: LoadShape("Circular.Half.Style27", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularHalf_Style28: LoadShape("Circular.Half.Style28", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_DefaultLeft: LoadShape("Circular.Quarter.Style1.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style1Left: LoadShape("Circular.Quarter.Style1.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style2Left: LoadShape("Circular.Quarter.Style2.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style3Left: LoadShape("Circular.Quarter.Style3.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style4Left: LoadShape("Circular.Quarter.Style4.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style5Left: LoadShape("Circular.Quarter.Style5.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style6Left: LoadShape("Circular.Quarter.Style6.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style7Left: LoadShape("Circular.Quarter.Style7.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style8Left: LoadShape("Circular.Quarter.Style8.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style9Left: LoadShape("Circular.Quarter.Style9.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style10Left: LoadShape("Circular.Quarter.Style10.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style11Left: LoadShape("Circular.Quarter.Style11.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style12Left: LoadShape("Circular.Quarter.Style12.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style13Left: LoadShape("Circular.Quarter.Style13.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style14Left: LoadShape("Circular.Quarter.Style14.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style15Left: LoadShape("Circular.Quarter.Style15.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style16Left: LoadShape("Circular.Quarter.Style16.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style17Left: LoadShape("Circular.Quarter.Style17.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style18Left: LoadShape("Circular.Quarter.Style18.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style19Left: LoadShape("Circular.Quarter.Style19.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style20Left: LoadShape("Circular.Quarter.Style20.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style21Left: LoadShape("Circular.Quarter.Style21.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style22Left: LoadShape("Circular.Quarter.Style22.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style23Left: LoadShape("Circular.Quarter.Style23.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style24Left: LoadShape("Circular.Quarter.Style24.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style25Left: LoadShape("Circular.Quarter.Style25.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style26Left: LoadShape("Circular.Quarter.Style26.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style27Left: LoadShape("Circular.Quarter.Style27.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style28Left: LoadShape("Circular.Quarter.Style28.Left", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_DefaultRight: LoadShape("Circular.Quarter.Style1.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style1Right: LoadShape("Circular.Quarter.Style1.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style2Right: LoadShape("Circular.Quarter.Style2.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style3Right: LoadShape("Circular.Quarter.Style3.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style4Right: LoadShape("Circular.Quarter.Style4.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style5Right: LoadShape("Circular.Quarter.Style5.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style6Right: LoadShape("Circular.Quarter.Style6.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style7Right: LoadShape("Circular.Quarter.Style7.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style8Right: LoadShape("Circular.Quarter.Style8.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style9Right: LoadShape("Circular.Quarter.Style9.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style10Right: LoadShape("Circular.Quarter.Style10.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style11Right: LoadShape("Circular.Quarter.Style11.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style12Right: LoadShape("Circular.Quarter.Style12.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style13Right: LoadShape("Circular.Quarter.Style13.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style14Right: LoadShape("Circular.Quarter.Style14.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style15Right: LoadShape("Circular.Quarter.Style15.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style16Right: LoadShape("Circular.Quarter.Style16.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style17Right: LoadShape("Circular.Quarter.Style17.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style18Right: LoadShape("Circular.Quarter.Style18.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style19Right: LoadShape("Circular.Quarter.Style19.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style20Right: LoadShape("Circular.Quarter.Style20.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style21Right: LoadShape("Circular.Quarter.Style21.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style22Right: LoadShape("Circular.Quarter.Style22.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style23Right: LoadShape("Circular.Quarter.Style23.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style24Right: LoadShape("Circular.Quarter.Style24.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style25Right: LoadShape("Circular.Quarter.Style25.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style26Right: LoadShape("Circular.Quarter.Style26.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style27Right: LoadShape("Circular.Quarter.Style27.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularQuarter_Style28Right: LoadShape("Circular.Quarter.Style28.Right", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Default: LoadShape("Circular.ThreeFourth.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style1: LoadShape("Circular.ThreeFourth.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style2: LoadShape("Circular.ThreeFourth.Style2", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style3: LoadShape("Circular.ThreeFourth.Style3", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style4: LoadShape("Circular.ThreeFourth.Style4", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style5: LoadShape("Circular.ThreeFourth.Style5", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style6: LoadShape("Circular.ThreeFourth.Style6", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style7: LoadShape("Circular.ThreeFourth.Style7", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style8: LoadShape("Circular.ThreeFourth.Style8", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style9: LoadShape("Circular.ThreeFourth.Style9", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style10: LoadShape("Circular.ThreeFourth.Style10", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style11: LoadShape("Circular.ThreeFourth.Style11", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style12: LoadShape("Circular.ThreeFourth.Style12", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style13: LoadShape("Circular.ThreeFourth.Style13", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style14: LoadShape("Circular.ThreeFourth.Style14", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style15: LoadShape("Circular.ThreeFourth.Style15", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style16: LoadShape("Circular.ThreeFourth.Style16", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style17: LoadShape("Circular.ThreeFourth.Style17", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style18: LoadShape("Circular.ThreeFourth.Style18", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style19: LoadShape("Circular.ThreeFourth.Style19", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style20: LoadShape("Circular.ThreeFourth.Style20", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style21: LoadShape("Circular.ThreeFourth.Style21", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style22: LoadShape("Circular.ThreeFourth.Style22", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style23: LoadShape("Circular.ThreeFourth.Style23", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style24: LoadShape("Circular.ThreeFourth.Style24", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style25: LoadShape("Circular.ThreeFourth.Style25", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style26: LoadShape("Circular.ThreeFourth.Style26", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style27: LoadShape("Circular.ThreeFourth.Style27", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularThreeFourth_Style28: LoadShape("Circular.ThreeFourth.Style28", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Default: LoadShape("Linear.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style1: LoadShape("Linear.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style2: LoadShape("Linear.Style2", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style3: LoadShape("Linear.Style3", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style4: LoadShape("Linear.Style4", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style5: LoadShape("Linear.Style5", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style6: LoadShape("Linear.Style6", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style7: LoadShape("Linear.Style7", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style8: LoadShape("Linear.Style8", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style9: LoadShape("Linear.Style9", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style10: LoadShape("Linear.Style10", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style11: LoadShape("Linear.Style11", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style12: LoadShape("Linear.Style12", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style13: LoadShape("Linear.Style13", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style14: LoadShape("Linear.Style14", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style15: LoadShape("Linear.Style15", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style16: LoadShape("Linear.Style16", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style17: LoadShape("Linear.Style17", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style18: LoadShape("Linear.Style18", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style19: LoadShape("Linear.Style19", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style20: LoadShape("Linear.Style20", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style21: LoadShape("Linear.Style21", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style22: LoadShape("Linear.Style22", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style23: LoadShape("Linear.Style23", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style24: LoadShape("Linear.Style24", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style25: LoadShape("Linear.Style25", "1", shapeType); break;
				case BackgroundLayerShapeType.Linear_Style26: LoadShape("Linear.Style26", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style1: LoadShape("Circular.Wide.Style1", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style1_1: LoadShape("Circular.Wide.Style1", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style2: LoadShape("Circular.Wide.Style2", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style2_1: LoadShape("Circular.Wide.Style2", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style3: LoadShape("Circular.Wide.Style3", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style3_1: LoadShape("Circular.Wide.Style3", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style4: LoadShape("Circular.Wide.Style4", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style4_1: LoadShape("Circular.Wide.Style4", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style5: LoadShape("Circular.Wide.Style5", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style6: LoadShape("Circular.Wide.Style6", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style6_1: LoadShape("Circular.Wide.Style6", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style7: LoadShape("Circular.Wide.Style7", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style7_1: LoadShape("Circular.Wide.Style7", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style8: LoadShape("Circular.Wide.Style8", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style8_1: LoadShape("Circular.Wide.Style8", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style9: LoadShape("Circular.Wide.Style9", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style9_1: LoadShape("Circular.Wide.Style9", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style10: LoadShape("Circular.Wide.Style10", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style11: LoadShape("Circular.Wide.Style11", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style12: LoadShape("Circular.Wide.Style12", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style12_1: LoadShape("Circular.Wide.Style12", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style13: LoadShape("Circular.Wide.Style13", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style13_1: LoadShape("Circular.Wide.Style13", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style14: LoadShape("Circular.Wide.Style14", "1", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style14_1: LoadShape("Circular.Wide.Style14", "2", shapeType); break;
				case BackgroundLayerShapeType.CircularWide_Style15: LoadShape("Circular.Wide.Style15", "1", shapeType); break;
			}
		}
		public static BaseShape GetDefaultLayerShape(BackgroundLayerShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapesCore(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
		static void LoadShape(string template, string postfix, BackgroundLayerShapeType type) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			BaseShape layerShape = templateShape.Collection[PredefinedShapeNames.BackgroundLayer + postfix];
			layerShape.Name = PredefinedShapeNames.BackgroundLayer;
			defaultShapes.Add(type, layerShape);
		}
	}
	public static class EffectLayerShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<EffectLayerShapeType, BaseShape> defaultShapes;
		static EffectLayerShapeFactory() {
			defaultShapes = new Dictionary<EffectLayerShapeType, BaseShape>();
		}
		static void LoadShapeCore(EffectLayerShapeType type) {
			switch (type) {
				case EffectLayerShapeType.Empty: LoadShape("Empty", type); break;
				case EffectLayerShapeType.Circular_Default: LoadShape("Circular.Full.Style6", type); break;
				case EffectLayerShapeType.CircularFull_Style6: LoadShape("Circular.Full.Style6", type); break;
				case EffectLayerShapeType.CircularFull_Style7: LoadShape("Circular.Full.Style7", type); break;
				case EffectLayerShapeType.CircularFull_Style8: LoadShape("Circular.Full.Style8", type); break;
				case EffectLayerShapeType.CircularFull_Style9: LoadShape("Circular.Full.Style9", type); break;
				case EffectLayerShapeType.CircularQuarter_Style6Left: LoadShape("Circular.Quarter.Style6.Left", type); break;
				case EffectLayerShapeType.CircularQuarter_Style7Left: LoadShape("Circular.Quarter.Style7.Left", type); break;
				case EffectLayerShapeType.CircularQuarter_Style8Left: LoadShape("Circular.Quarter.Style8.Left", type); break;
				case EffectLayerShapeType.CircularQuarter_Style9Left: LoadShape("Circular.Quarter.Style9.Left", type); break;
				case EffectLayerShapeType.CircularQuarter_Style6Right: LoadShape("Circular.Quarter.Style6.Right", type); break;
				case EffectLayerShapeType.CircularQuarter_Style7Right: LoadShape("Circular.Quarter.Style7.Right", type); break;
				case EffectLayerShapeType.CircularQuarter_Style8Right: LoadShape("Circular.Quarter.Style8.Right", type); break;
				case EffectLayerShapeType.CircularQuarter_Style9Right: LoadShape("Circular.Quarter.Style9.Right", type); break;
				case EffectLayerShapeType.CircularThreeFourth_Style6: LoadShape("Circular.ThreeFourth.Style6", type); break;
				case EffectLayerShapeType.CircularThreeFourth_Style7: LoadShape("Circular.ThreeFourth.Style7", type); break;
				case EffectLayerShapeType.CircularThreeFourth_Style8: LoadShape("Circular.ThreeFourth.Style8", type); break;
				case EffectLayerShapeType.CircularThreeFourth_Style9: LoadShape("Circular.ThreeFourth.Style9", type); break;
				case EffectLayerShapeType.CircularWide_Style7: LoadShape("Circular.Wide.Style7", type); break;
				case EffectLayerShapeType.CircularWide_Style8: LoadShape("Circular.Wide.Style8", type); break;
				case EffectLayerShapeType.CircularWide_Style9: LoadShape("Circular.Wide.Style9", type); break;
				case EffectLayerShapeType.Linear_Style6: LoadShape("Linear.Style6", type); break;
				case EffectLayerShapeType.Linear_Style8: LoadShape("Linear.Style8", type); break;
				case EffectLayerShapeType.CircularFull_Clock: LoadShape("Temp.Demo.demo_360", type); break;
			}
		}
		public static BaseShape GetDefaultLayerShape(EffectLayerShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapeCore(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
		static void LoadShape(string template, EffectLayerShapeType type) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			BaseShape layerShape = (templateShape != null) ?
				templateShape.Collection[PredefinedShapeNames.Effect] : BaseShape.Empty;
			defaultShapes.Add(type, layerShape);
		}
	}
	public static class SpindleCapShapeFactory {
		static readonly object syncObj = new object();
		static Dictionary<SpindleCapShapeType, BaseShape> defaultShapes;
		static SpindleCapShapeFactory() {
			defaultShapes = new Dictionary<SpindleCapShapeType, BaseShape>();
		}
		static void LoadShapeCore(SpindleCapShapeType type) {
			switch (type) {
				case SpindleCapShapeType.Empty: LoadShape("Empty", type); break;
				case SpindleCapShapeType.CircularFull_Default: LoadShape("Circular.Full.Style2", type); break;
				case SpindleCapShapeType.CircularFull_Style2: LoadShape("Circular.Full.Style2", type); break;
				case SpindleCapShapeType.CircularFull_Style3: LoadShape("Circular.Full.Style3", type); break;
				case SpindleCapShapeType.CircularFull_Style4: LoadShape("Circular.Full.Style4", type); break;
				case SpindleCapShapeType.CircularFull_Style6: LoadShape("Circular.Full.Style6", type); break;
				case SpindleCapShapeType.CircularFull_Style8: LoadShape("Circular.Full.Style8", type); break;
				case SpindleCapShapeType.CircularFull_Style9: LoadShape("Circular.Full.Style9", type); break;
				case SpindleCapShapeType.CircularFull_Style16: LoadShape("Circular.Full.Style16", type); break;
				case SpindleCapShapeType.CircularFull_Style19: LoadShape("Circular.Full.Style19", type); break;
				case SpindleCapShapeType.CircularFull_Style20: LoadShape("Circular.Full.Style20", type); break;
				case SpindleCapShapeType.CircularFull_Style22: LoadShape("Circular.Full.Style22", type); break;
				case SpindleCapShapeType.CircularFull_Style25: LoadShape("Circular.Full.Style25", type); break;
				case SpindleCapShapeType.CircularFull_Style26: LoadShape("Circular.Full.Style26", type); break;
				case SpindleCapShapeType.CircularFull_Clock: LoadShape("Temp.Demo.demo_360", type); break;
			}
		}
		public static BaseShape GetDefaultSpindleCapShape(SpindleCapShapeType type) {
			lock(syncObj) {
				BaseShape shape;
				if(!defaultShapes.TryGetValue(type, out shape)) {
					LoadShapeCore(type);
					shape = defaultShapes[type];
				}
				return shape.Clone();
			}
		}
		static void LoadShape(string template, SpindleCapShapeType type) {
			ComplexShape templateShape = XAMLResoucesFactory.GetShape(template) as ComplexShape;
			BaseShape capShape = (templateShape != null) ?
				templateShape.Collection[PredefinedShapeNames.SpindleCap] : BaseShape.Empty;
			defaultShapes.Add(type, capShape);
		}
	}
}
