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
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.ChartRangeControlClient.Core;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Sparkline.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraEditors {
	[
	DefaultEvent("CustomizeSeries"),
	DefaultProperty("DataProvider"),
	ToolboxTabName(AssemblyInfo.DXTabCommon)
	]
	public abstract class ChartRangeControlClientBase : Component, IRangeControlClient, ISupportInitialize {
		#region Nested Classes : CoreClientDelegate, FakeDataPoint
		sealed class CoreClientDelegate : IChartCoreClientDelegate {
			readonly ChartRangeControlClientBase client;
			public CoreClientDelegate(ChartRangeControlClientBase client) {
				this.client = client;
			}
			#region IChartCoreClientDelegate
			void IChartCoreClientDelegate.InteractionUpdated() {
				client.InteractionUpdated();
			}
			#endregion
		}
		protected class FakeDataPoint<T> {
			readonly string series;
			readonly T argument;
			readonly double value;
			public string Series { get { return series; } }
			public T Argument { get { return argument; } }
			public double Value { get { return value; } }
			public FakeDataPoint(string series, T argument, double value) {
				this.series = series;
				this.argument = argument;
				this.value = value;
			}
		}
		#endregion
		protected const int FakePointsCount = 10;
		protected const int FakeSeriesCount = 3;
		internal const string DefaultPaletteName = "Office";
		static readonly object customizeSeriesKey = new object();
		static List<string> predefinedPalettes;
		public static IEnumerable<string> PredefinedPalettes {
			get {
				if (predefinedPalettes == null) {
					predefinedPalettes = new List<string>();
					List<ChartRangeControlClientPalette> palettes = ChartRangeControlClientPalette.PredefinedPalettes;
					for (int i = 0; i < palettes.Count; i++)
						predefinedPalettes.Add(palettes[i].Name);
				}
				return predefinedPalettes;
			}
		}
		readonly ChartRangeControlClientDataProvider dataProvider;
		readonly CoreClientDelegate coreClientDelegate;
		readonly ChartCoreClient coreClient;
		readonly ChartRangeControlClientGridOptions gridOptions;
		readonly ChartRangeControlClientRange range;
		readonly int randomSeed = (int)DateTime.Now.Ticks;
		ChartRangeControlClientPalette currentPalette;
		string paletteName;
		List<object> grid;
		IRangeControl rangeControl;
		ISkinProvider rangeControlSkinProvider;
		bool loading;
		protected int RandomSeed { 
			get { return randomSeed; } 
		}
		internal bool IsDesignMode { 
			get { return DesignMode; } 
		}
		internal bool Loading { 
			get { return loading; } 
		}
		internal ChartCoreClient CoreClient {
			get { return coreClient; }
		}
		internal ChartRangeControlClientPalette CurrentPalette {
			get { return currentPalette; }
		}
		public ChartRangeControlClientGridOptions GridOptions {
			get { return gridOptions; }
		}
		public ChartRangeControlClientRange Range {
			get { return range; }
		}
		public override ISite Site {
			get {
				return base.Site;
			}
			set {
				base.Site = value;
				dataProvider.UpdateBindingSource(true);
			}
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientBase.DataProvider"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientBaseDataProvider"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter))
		]
		public ChartRangeControlClientDataProvider DataProvider {
			get { return dataProvider; }
		}
		[
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.ChartRangeControlClientBase.PaletteName"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ChartRangeControlClientBasePaletteName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor(typeof(ChartRangeControlClientPaletteTypeEditor), typeof(UITypeEditor)),
		TypeConverter(typeof(ChartRangeControlClientPaletteTypeConverter)),
		DefaultValue(DefaultPaletteName)
		]
		public string PaletteName {
			get {
				if (string.IsNullOrEmpty(paletteName))
					return DefaultPaletteName;
				return paletteName;
			}
			set {
				if (paletteName != value) {
					if (string.IsNullOrWhiteSpace(value) || (ChartRangeControlClientPalette.GetPalette(value) == null))
						throw new ArgumentException("Invalid palette name");
					paletteName = value;
					UpdateSkin();
				}
			}
		}
		public event EventHandler<ClientDataSourceProviderCustomizeSeriesEventArgs> CustomizeSeries {
			add { this.Events.AddHandler(customizeSeriesKey, value); }
			remove { this.Events.RemoveHandler(customizeSeriesKey, value); }
		}
		event ClientRangeChangedEventHandler RangeControlRangeChanged;
		public ChartRangeControlClientBase(SparklineScaleType scaleType, ChartRangeControlClientGridOptions gridOptions, ChartRangeControlClientRange range) {
			this.loading = false;
			this.range = range;
			this.range.SetClient(this);
			this.dataProvider = new ChartRangeControlClientDataProvider(this, scaleType);
			this.coreClient = new ChartCoreClient();
			this.coreClientDelegate = new CoreClientDelegate(this);
			this.coreClient.Delegate = coreClientDelegate;
			this.coreClient.DataProvider = dataProvider.ClientDataProvider;
			this.gridOptions = gridOptions;
			this.gridOptions.Changed = GridOptionsChanged;
			UpdateSkin();
		}
		#region IRangeControlClient
		string IRangeControlClient.InvalidText {
			get { return Localizer.Active.GetLocalizedString(StringId.ChartRangeControlClientNoData); }
		}
		bool IRangeControlClient.IsValid {
			get { return coreClient.HasDataToPresent; }
		}
		bool IRangeControlClient.IsCustomRuler {
			get {
				return (grid != null) && (grid.Count == 0);
			}
		}
		double IRangeControlClient.NormalizedRulerDelta {
			get { return 1; }
		}
		int IRangeControlClient.RangeBoxBottomIndent {
			get { return 0; }
		}
		int IRangeControlClient.RangeBoxTopIndent {
			get { return 0; }
		}
		object IRangeControlClient.RulerDelta {
			get { return 0; }
		}
		event ClientRangeChangedEventHandler IRangeControlClient.RangeChanged {
			add { RangeControlRangeChanged += value; }
			remove { RangeControlRangeChanged -= value; }
		}
		bool IRangeControlClient.IsValidType(Type type) {
			return IsValidType(type);
		}
		double IRangeControlClient.GetNormalizedValue(object value) {
			return coreClient.Normalize(ValidateValue(value));
		}
		object IRangeControlClient.GetValue(double normalizedValue) {
			return NativeValue(coreClient.GetValue(normalizedValue));
		}
		void IRangeControlClient.DrawContent(RangeControlPaintEventArgs e) {
			coreClient.DrawContent(e.Graphics, e.ContentBounds, e.NormalTransform);
		}
		void IRangeControlClient.OnRangeControlChanged(IRangeControl rangeControl) {
			if (this.rangeControl != rangeControl) {
				ISupportLookAndFeel supportLookAndFeel = this.rangeControl as ISupportLookAndFeel;
				if (supportLookAndFeel != null) {
					supportLookAndFeel.LookAndFeel.StyleChanged -= LookAndFeelStyleChanged;
					rangeControlSkinProvider = null;
				}
				this.rangeControl = rangeControl;
				dataProvider.UpdateBindingSource(false);
				if (this.rangeControl != null) {
					SparklineRangeData dataRange = coreClient.Interaction.Ranges.DataArgumentRange;
					if (dataRange.IsValid)
						this.rangeControl.SelectedRange = new RangeControlRange(dataRange.Min, dataRange.Max);
					else
						this.rangeControl.SelectedRange = new RangeControlRange();
					supportLookAndFeel = this.rangeControl as ISupportLookAndFeel;
					if (supportLookAndFeel != null) {
						supportLookAndFeel.LookAndFeel.StyleChanged += LookAndFeelStyleChanged;
						rangeControlSkinProvider = supportLookAndFeel.LookAndFeel;
					}
				}
				UpdateSkin();
			}
		}
		List<object> IRangeControlClient.GetRuler(RulerInfoArgs e) {
			grid = coreClient.GenerateGrid(ValidateValue(e.MinValue), ValidateValue(e.MaxValue), e.RulerWidthInPixels, GridOptions.CoreGridOptions);
			return grid;
		}
		string IRangeControlClient.RulerToString(int ruleIndex) {
			return FormatValue((double)grid[ruleIndex]);
		}
		string IRangeControlClient.ValueToString(double normalizedValue) {
			return FormatValue(coreClient.GetValue(normalizedValue));	
		}
		void IRangeControlClient.ValidateRange(NormalizedRangeInfo info) {
			SnapRange(info);
		}
		bool IRangeControlClient.DrawRuler(RangeControlPaintEventArgs e) {
			if ((grid == null) || (grid.Count == 0)) {
				Graphics graphics = e.Graphics;
				string message = GridOptions.ShowGridlinesErrorMessage ? Localizer.Active.GetLocalizedString(StringId.ChartRangeControlClientInvalidGrid) : string.Empty;
				if (!String.IsNullOrWhiteSpace(message)) {
					using (Font font = new Font("Arial", 12, FontStyle.Bold))
					using (Brush brush = new SolidBrush(Color.FromArgb(126, Color.Blue))) {
						SizeF messageSize = graphics.MeasureString(message, font, e.ContentBounds.Size);
						float x = Math.Max(0, e.ContentBounds.Size.Width * 0.5f - messageSize.Width * 0.5f);
						float y = Math.Max(0, e.ContentBounds.Size.Height * 0.5f - messageSize.Height * 0.5f);
						StringFormat format = new StringFormat();
						format.Alignment = StringAlignment.Center;
						graphics.DrawString(message, font, brush, new RectangleF(x, y, messageSize.Width, messageSize.Height), format);
					}
				}
			}
			return false; 
		}
		double IRangeControlClient.ValidateScale(double newScale) { return newScale; }
		void IRangeControlClient.OnRangeChanged(object rangeMinimum, object rangeMaximum) { }
		bool IRangeControlClient.SupportOrientation(Orientation orientation) { return true; }
		object IRangeControlClient.GetOptions() { return this; }
		void IRangeControlClient.Calculate(Rectangle contentRect) { }
		void IRangeControlClient.OnClick(RangeControlHitInfo hitInfo) { }
		void IRangeControlClient.OnResize() { }
		void IRangeControlClient.UpdateHotInfo(RangeControlHitInfo hitInfo) { }
		void IRangeControlClient.UpdatePressedInfo(RangeControlHitInfo hitInfo) { }
		#endregion
		#region ISupportInitialize
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
			Range.Validate();
		}
		#endregion
		string FormatValue(double value) {
			ChartRangeControlClientGridOptions gridOptions = GridOptions;
			object nativeValue = gridOptions.GetNativeGridValue(value);
			string actualFormat = "{0}";
			if (!string.IsNullOrWhiteSpace(gridOptions.LabelFormat))
				actualFormat = "{0:" + gridOptions.LabelFormat + "}";
			return String.Format(gridOptions.ActualLabelFormatProvider, actualFormat, nativeValue);
		}
		void UpdateSkin() {
			if (PaletteName == DefaultPaletteName) {
				string actualPaletteName = GetActualPaletteNameFromSkin(rangeControlSkinProvider);
				this.currentPalette = ChartRangeControlClientPalette.GetPalette(actualPaletteName);
			} else
				this.currentPalette = ChartRangeControlClientPalette.GetPalette(PaletteName);
			dataProvider.RefreshData();
		}
		void LookAndFeelStyleChanged(object sender, EventArgs e) {
			UpdateSkin();
		}
		void SnapRange(NormalizedRangeInfo range) {
			if ((range.Type == RangeControlValidationType.Range) && (range.ChangedBound != ChangedBoundType.None)) {
				SparklineRangeData coreRange = new SparklineRangeData(range.Range.Minimum, range.Range.Maximum);
				coreClient.SnapNormalRange(coreRange, (SnapBounds)range.ChangedBound, GridOptions.CoreGridOptions);
				range.Range.Minimum = coreRange.Min;
				range.Range.Maximum = coreRange.Max;
			}
		}
		void InteractionUpdated() {
			range.UpdateWithDataArgumentRange();
			InvalidateRangeControl(false);
		}
		string GetActualPaletteNameFromSkin(ISkinProvider skinProvider) {
			if (skinProvider != null) {
				Skin skin = EditorsSkins.GetSkin(skinProvider);
				if (skin != null) {
					SkinElement clientElement = skin[EditorsSkins.ChartRangeControlClientElement];
					if ((clientElement != null) && clientElement.Properties.ContainsProperty(EditorsSkins.ChartRangeControlClientPaletteName)) {
						ChartRangeControlClientPaletteName enumeration = (ChartRangeControlClientPaletteName)clientElement.Properties[EditorsSkins.ChartRangeControlClientPaletteName];
						return enumeration.ToString();
					}
				}
			}
			return DefaultPaletteName;
		}
		protected void GridOptionsChanged() {
			dataProvider.UpdateFakeData();
			InvalidateRangeControl(false);
		}
		protected abstract double ValidateValue(object value);
		protected abstract object NativeValue(double value);
		protected virtual bool IsValidType(Type type) {
			return (type == typeof(double));
		}
		protected internal abstract List<object> CreateFakeData();
		internal void SetCustomRange(Tuple<object, object> range) {
			object min = range.Item1;
			object max = range.Item2;
			if ((min != null) && (max != null))
				coreClient.SetCustomArgumentRange(new SparklineRangeData(ValidateValue(min), ValidateValue(max)));
			else
				coreClient.SetCustomArgumentRange(new SparklineRangeData());
			dataProvider.UpdateFakeData();
			InvalidateRangeControl(false);
		}
		internal Tuple<object, object> GetArgumentRange() {
			SparklineRangeData internalArgumentRange = coreClient.Interaction.Ranges.DataArgumentRange;
			return new Tuple<object, object>(NativeValue(internalArgumentRange.Min), NativeValue(internalArgumentRange.Max));
		}
		internal void InvalidateRangeControl(bool forceSetRange) {
			if (RangeControlRangeChanged != null) {
				RangeControlClientRangeEventArgs args = new RangeControlClientRangeEventArgs();
				args.InvalidateContent = true;
				args.AnimatedViewport = true;
				args.MakeRangeVisible = true;
				SparklineRangeData dataRange = coreClient.Interaction.Ranges.DataArgumentRange;
				double dataMin = SparklineMathUtils.IsValidDouble(dataRange.Min) ? dataRange.Min : 0.0;
				double dataMax = SparklineMathUtils.IsValidDouble(dataRange.Max) ? dataRange.Max : 0.0;
				if ((rangeControl != null) && !forceSetRange) {
					RangeControlRange selectedRange = rangeControl.SelectedRange;
					double selectedMin = ValidateValue(selectedRange.Minimum);
					double selectedMax = ValidateValue(selectedRange.Maximum);
					args.Range = new RangeControlRange(NativeValue(Math.Max(selectedMin, dataMin)), NativeValue(Math.Min(selectedMax, dataMax)));
				} else
					args.Range = new RangeControlRange(NativeValue(dataMin), NativeValue(dataMax));
				RangeControlRangeChanged(this, args);
			}
		}
		internal void RaiseCustomizeSeries(ClientDataSourceProviderCustomizeSeriesEventArgs eventArgs) {
			EventHandler<ClientDataSourceProviderCustomizeSeriesEventArgs> handler = (EventHandler<ClientDataSourceProviderCustomizeSeriesEventArgs>)this.Events[customizeSeriesKey];
			if (handler != null)
				handler(this, eventArgs);
		}
	}
}
