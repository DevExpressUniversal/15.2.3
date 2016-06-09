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
using System.Linq;
using DevExpress.Utils.Controls;
using System.ComponentModel;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraMap.Native;
using System.Drawing.Design;
using System.Drawing;
using DevExpress.Map.Native;
namespace DevExpress.XtraMap {
	public abstract class MapNotificationOptions : BaseOptions {
		protected MapNotificationOptions() {
			Reset();
		}
		#region Events
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapNotificationOptionsChanged")]
#endif
		public event BaseOptionChangedEventHandler Changed { add { ChangedCore += value; } remove { ChangedCore -= value; } }
		#endregion
		public override void Reset() {
			BeginUpdate();
			try {
				ResetCore();
			} finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnChanged<T>(string name, T oldValue, T newValue) {
			OnChanged(new BaseOptionChangedEventArgs(name, oldValue, newValue));
		}
		protected internal abstract void ResetCore();
	}
	public class NavigationPanelOptions : MapNotificationOptions, IMapStyleOwner {
		internal const string DefaultCoordinatesPattern = "";
		const int DefaultNavigationPanelHeight = 80;
		bool isGeoProjection;
		bool showMilesScale;
		bool showKilometersScale;
		bool showCoordinates;
		bool showZoomTrackbar;
		bool showScrollButtons;
		int height = DefaultNavigationPanelHeight;
		bool visible;
		string yCoordinatePattern;
		string xCoordinatePattern;
		readonly NavigationPanelAppearance appearance;
		internal NavigationPanelAppearance Appearance { get { return appearance; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsGeoProjection { get { return isGeoProjection; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsHeight"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultNavigationPanelHeight)]
		public int Height {
			get { return height; }
			set {
				if (height == value)
					return;
				int oldValue = height;
				height = value;
				OnChanged("Height ", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if (visible == value)
					return;
				visible = value;
				OnChanged("Visible", !value, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsShowScrollButtons"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowScrollButtons {
			get { return showScrollButtons; }
			set {
				if (showScrollButtons == value)
					return;
				showScrollButtons = value;
				OnChanged("showScrollButtons", !value, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsShowZoomTrackbar"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowZoomTrackbar {
			get { return showZoomTrackbar; }
			set {
				if (showZoomTrackbar == value)
					return;
				showZoomTrackbar = value;
				OnChanged("showZoomTrackbar", !value, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsShowCoordinates"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowCoordinates {
			get { return showCoordinates; }
			set {
				if (showCoordinates == value)
					return;
				showCoordinates = value;
				OnChanged("showCoordinates", !value, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsShowKilometersScale"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowKilometersScale {
			get {
				return showKilometersScale;
			}
			set {
				if (showKilometersScale == value)
					return;
				showKilometersScale = value;
				OnChanged("showKilometersScale", !value, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsShowMilesScale"),
#endif
		NotifyParentProperty(true), DefaultValue(true)]
		public bool ShowMilesScale {
			get {
				return showMilesScale;
			}
			set {
				if (showMilesScale == value)
					return;
				showMilesScale = value;
				OnChanged("showMilesScale", !value, value);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsYCoordinatePattern"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultCoordinatesPattern),
		TypeConverter("DevExpress.XtraMap.Design.CoordinatesPatternTypeConverter, " + AssemblyInfo.SRAssemblyMapDesign)]
		public string YCoordinatePattern {
			get {
				return yCoordinatePattern;
			}
			set {
				if (String.Equals(yCoordinatePattern, value))
					return;
				string oldValue = yCoordinatePattern;
				yCoordinatePattern = value;
				OnChanged("yCoordinatePattern", oldValue, yCoordinatePattern);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsXCoordinatePattern"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultCoordinatesPattern),
		TypeConverter("DevExpress.XtraMap.Design.CoordinatesPatternTypeConverter, " + AssemblyInfo.SRAssemblyMapDesign)]
		public string XCoordinatePattern {
			get {
				return xCoordinatePattern;
			}
			set {
				if (String.Equals(xCoordinatePattern, value))
					return;
				string oldValue = xCoordinatePattern;
				xCoordinatePattern = value;
				OnChanged("xCoordinatePattern", oldValue, xCoordinatePattern);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsCoordinatesStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public TextElementStyle CoordinatesStyle { get { return appearance.CoordinatesStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsScaleStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public TextElementStyle ScaleStyle { get { return appearance.ScaleStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsBackgroundStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle BackgroundStyle { get { return appearance.BackgroundStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsHotTrackedItemStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle HotTrackedItemStyle { get { return appearance.HotTrackedItemStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsPressedItemStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle PressedItemStyle { get { return appearance.PressedItemStyle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("NavigationPanelOptionsItemStyle"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle ItemStyle { get { return appearance.ItemStyle; } }
		public NavigationPanelOptions() {
			appearance = new NavigationPanelAppearance(this);
		}
		#region IMapStyleOwner implementation
		void IMapStyleOwner.OnStyleChanged() {
			RaiseOnChanged(new BaseOptionChangedEventArgs());
		}
		#endregion
		protected internal override void ResetCore() {
			showMilesScale = true;
			showKilometersScale = true;
			showCoordinates = true;
			showZoomTrackbar = true;
			showScrollButtons = true;
			visible = true;
			ResetPatterns(true);
		}
		internal void ResetPatterns(bool isGeoProjection) {
			this.isGeoProjection = isGeoProjection;
			this.yCoordinatePattern = DefaultCoordinatesPattern;
			this.xCoordinatePattern = DefaultCoordinatesPattern;
		}
	}
	public class SearchPanelOptions : MapNotificationOptions, IMapStyleOwner {
		readonly SearchPanelAppearance appearance;
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public BackgroundStyle BackgroundStyle { get { return appearance.BackgroundStyle; } }
		public SearchPanelOptions() {
			appearance = new SearchPanelAppearance(this);
		}
		#region IMapStyleOwner implementation
		void IMapStyleOwner.OnStyleChanged() {
			RaiseOnChanged(new BaseOptionChangedEventArgs());
		}
		#endregion
		protected internal override void ResetCore() {
		}
	}
}
namespace DevExpress.XtraMap.Printing {
	public class PrintOptions : MapNotificationOptions {
		const MapPrintSizeMode DefaultSizeMode = MapPrintSizeMode.Normal;
		MapPrintSizeMode sizeMode = DefaultSizeMode;
		bool printMiniMap = false;
		bool printNavigationPanel = false;
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("PrintOptionsSizeMode"),
#endif
		Category(SRCategoryNames.Behavior), NotifyParentProperty(true), DefaultValue(DefaultSizeMode)]
		public MapPrintSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if (sizeMode == value)
					return;
				MapPrintSizeMode oldValue = SizeMode;
				sizeMode = value;
				OnChanged("SizeMode", oldValue, sizeMode);
			}
		}
		[Category(SRCategoryNames.Behavior), NotifyParentProperty(true), DefaultValue(false)]
		public bool PrintMiniMap {
			get { return printMiniMap; }
			set {
				if (printMiniMap == value)
					return;
				bool oldValue = printMiniMap;
				printMiniMap = value;
				OnChanged("PrintMiniMap", oldValue, printMiniMap);
			}
		}
		[Category(SRCategoryNames.Behavior), NotifyParentProperty(true), DefaultValue(false)]
		public bool PrintNavigationPanel {
			get { return printNavigationPanel; }
			set {
				if (printNavigationPanel == value)
					return;
				bool oldValue = printNavigationPanel;
				printNavigationPanel = value;
				OnChanged("PrintNavigationPanel", oldValue, printNavigationPanel);
			}
		}
		protected internal override void ResetCore() {
			sizeMode = DefaultSizeMode;
			printMiniMap = false;
			printNavigationPanel = false;
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			PrintOptions printOptions = options as PrintOptions;
			if(printOptions == null)
				return;
			SizeMode = printOptions.SizeMode;
		}
		public override string ToString() {
			return "(MapPrintOptions)";
		}
	}
}
