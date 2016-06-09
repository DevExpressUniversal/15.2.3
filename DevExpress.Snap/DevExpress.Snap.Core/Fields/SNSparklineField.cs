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
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Fields {
	[ActionListDesigner("DevExpress.Snap.Extensions.Native.ActionLists.SparklineActionListDesigner," + AssemblyInfo.SRAssemblySnapExtensions)]
	public class SNSparklineField : SNMergeFieldSupportsEmptyFieldDataAlias {
		#region static
		public static new readonly string FieldType = "SNSPARKLINE";
		public static readonly string SnSparklineDataSourceNameSwitch = "dsn";
		public static readonly string SnSparklineViewTypeSwitch = "vt";
		public static readonly string SnSparklineWidthSwitch = "w";
		public static readonly string SnSparklineHeightSwitch = "h";
		public static readonly string SnSparklineHighlightMaxPointSwitch = "hmxp";
		public static readonly string SnSparklineHighlightMinPointSwitch = "hmnp";
		public static readonly string SnSparklineHighlightStartPointSwitch = "hsp";
		public static readonly string SnSparklineHighlightEndPointSwitch = "hep";
		public static readonly string SnSparklineColorSwitch = "cl";
		public static readonly string SnSparklineMaxPointColorSwitch = "mxpcl";
		public static readonly string SnSparklineMinPointColorSwitch = "mnpcl";
		public static readonly string SnSparklineStartPointColorSwitch = "spcl";
		public static readonly string SnSparklineEndPointColorSwitch = "epcl";
		public static readonly string SnSparklineNegativePointColorSwitch = "npcl";
		public static readonly string SnSparklineLineWidthSwitch = "lnw";
		public static readonly string SnSparklineHighlightNegativePointsSwitch = "hnp";
		public static readonly string SnSparklineShowMarkersSwitch = "sm";
		public static readonly string SnSparklineMarkerSizeSwitch = "msz";
		public static readonly string SnSparklineMaxPointMarkerSizeSwitch = "mxpmsz";
		public static readonly string SnSparklineMinPointMarkerSizeSwitch = "mnpmsz";
		public static readonly string SnSparklineStartPointMarkerSizeSwitch = "spmsz";
		public static readonly string SnSparklineEndPointMarkerSizeSwitch = "epmsz";
		public static readonly string SnSparklineNegativePointMarkerSizeSwitch = "npmsz";
		public static readonly string SnSparklineMarkerColorSwitch = "mcl";
		public static readonly string SnSparklineAreaOpacitySwitch = "ao";
		public static readonly string SnSparklineBarDistanceSwitch = "bd";
		public static readonly string AliasForEmptyFieldData = "<<IncorrectData>>";
		public static new CalculatedFieldBase Create() {
			return new SNSparklineField();
		}
		internal static readonly Dictionary<SparklineViewType, string> SparklineViewTypeStringDictionary = new Dictionary<SparklineViewType, string>();
		static readonly Dictionary<string, bool> sparklineSwitchesWithArgument;
		static SNSparklineField() {
			sparklineSwitchesWithArgument = CreateSwitchesWithArgument(
							SnSparklineViewTypeSwitch, SnSparklineWidthSwitch, SnSparklineHeightSwitch,
							SnSparklineHighlightMaxPointSwitch, SnSparklineHighlightMinPointSwitch, SnSparklineHighlightStartPointSwitch, SnSparklineHighlightEndPointSwitch,
							SnSparklineColorSwitch, SnSparklineMaxPointColorSwitch, SnSparklineMinPointColorSwitch, SnSparklineStartPointColorSwitch,
							SnSparklineEndPointColorSwitch, SnSparklineNegativePointColorSwitch, SnSparklineLineWidthSwitch, SnSparklineHighlightNegativePointsSwitch,
							SnSparklineShowMarkersSwitch, SnSparklineMarkerSizeSwitch, SnSparklineMaxPointMarkerSizeSwitch, SnSparklineMinPointMarkerSizeSwitch,
							SnSparklineStartPointMarkerSizeSwitch, SnSparklineEndPointMarkerSizeSwitch, SnSparklineNegativePointMarkerSizeSwitch,
							SnSparklineMarkerColorSwitch, SnSparklineAreaOpacitySwitch, SnSparklineBarDistanceSwitch, SnSparklineDataSourceNameSwitch,EmptyFieldDataAliasSwitch);
			foreach(KeyValuePair<string, bool> sw in MergefieldField.SwitchesWithArgument)
				sparklineSwitchesWithArgument.Add(sw.Key, sw.Value);
			SparklineViewTypeStringDictionary.Add(SparklineViewType.Area, "Area");
			SparklineViewTypeStringDictionary.Add(SparklineViewType.Bar, "Bar");
			SparklineViewTypeStringDictionary.Add(SparklineViewType.Line, "Line");
			SparklineViewTypeStringDictionary.Add(SparklineViewType.WinLoss, "WinLoss");
		}
		public static string GetSparklineViewTypeString(SparklineViewType type) {
			return SparklineViewTypeStringDictionary[type];
		}
		#endregion
		public SNSparklineField() : base() {
			InitSparklineViewTypeToViewFields();
		}
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			SetDataSourceName(instructions);
			SetViewType(instructions);
			SetWidth(instructions);
			SetHeight(instructions);
			SetHighlightMaxPoint(instructions);
			SetHighlightMinPoint(instructions);
			SetHighlightStartPoint(instructions);
			SetHighlightEndPoint(instructions);
			SetColor(instructions);
			SetMaxPointColor(instructions);
			SetMinPointColor(instructions);
			SetStartPointColor(instructions);
			SetEndPointColor(instructions);
			SetNegativePointColor(instructions);
			SetLineWidth(instructions);
			SetHighlightNegativePoints(instructions);
			SetShowMarkers(instructions);
			SetMarkerSize(instructions);
			SetMaxPointMarkerSize(instructions);
			SetMinPointMarkerSize(instructions);
			SetStartPointMarkerSize(instructions);
			SetEndPointMarkerSize(instructions);
			SetNegativePointMarkerSize(instructions);
			SetMarkerColor(instructions);
			SetAreaOpacity(instructions);
			SetBarDistance(instructions);
		}
		protected override bool SholdApplyFormating {
			get { return false; }
		}
		readonly SparklinePaintersCache paintersCache = new SparklinePaintersCache();
		SparklineData sparklineData = new SparklineData();
		internal IList<double> Values { get { return sparklineData.Values; } }
		public bool IsEmpty {
			get {
				return string.IsNullOrEmpty(DataFieldName) && string.IsNullOrEmpty(DataSourceName);
			}
		}
		SparklineSettings sparklineSettings = new SparklineSettings();
		readonly Size DefaultSize = new Size(400, 100);
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return sparklineSwitchesWithArgument; } }
		#region Common fields
		SparklineViewType viewType;
		int width = -1;
		int height = -1;
		string dataSourceName;
		public SparklineViewType DefaultViewType { get { return SparklineViewType.Line; } }
		public int DefaultWidth { get { return DefaultSize.Width; } }
		public int DefaultHeight { get { return DefaultSize.Height; } }
		public SparklineViewType ViewType {
			get { return viewType; }
			private set {
				if(viewType == value) return;
				viewType = value;
			}
		}
		public int Width {
			get {
				if(width < 0) return DefaultSize.Width;
				return width;
			}
			private set {
				if(width == value) return;
				width = value;
			}
		}
		public int Height {
			get {
				if(height < 0) return DefaultSize.Height;
				return height;
			}
			set {
				if(height == value) return;
				height = value;
			}
		}
		public SparklineViewBase View {
			get {
				if(sparklineSettings.View == null) InitView();
				return sparklineSettings.View;
			}
			set {
				if(View == value) return;
				sparklineSettings.SetView(value);
			}
		}
		public string DataSourceName { get { return dataSourceName; } }
		#endregion Common fields
		[Flags]
		enum FieldViewType {
			Line		= 0x0,
			Area		= 0x1,
			Bar		 = 0x2,
			WinLoss	 = 0x4,
			All		 = Line | Area | Bar | WinLoss,
		}
		[Flags]
		enum SparklineViewFields {
			None = 						0x000000000,
			BarDistance = 				0x000000001,
			AreaOpacity = 				0x000000002,
			LineWidth = 				0x000000004,
			HighlightNegativePoints = 	0x000000008,
			ShowMarkers = 				0x000000010,
			MarkerSize = 				0x000000020,
			MaxPointMarkerSize = 		0x000000040,
			MinPointMarkerSize = 		0x000000080,
			StartPointMarkerSize = 		0x000000100,
			EndPointMarkerSize = 		0x000000200,
			NegativePointMarkerSize = 	0x000000400,
			MarkerColor = 				0x000000800,
			HighlightMaxPoint = 		0x000001000,
			HighlightMinPoint = 		0x000002000,
			HighlightStartPoint = 		0x000004000,
			HighlightEndPoint = 		0x000008000,
			Color = 					0x000010000,
			MaxPointColor = 			0x000020000,
			MinPointColor = 			0x000040000,
			StartPointColor = 			0x000080000,
			EndPointColor = 			0x000100000,
			NegativePointColor = 		0x000200000,
		}
		Dictionary<SparklineViewType, SparklineViewFields> SparklineViewTypeToViewFields = new Dictionary<SparklineViewType, SparklineViewFields>();
		void InitSparklineViewTypeToViewFields() {
			if(SparklineViewTypeToViewFields.Count == Enum.GetValues(typeof(SparklineViewType)).Length) return;
			if(SparklineViewTypeToViewFields.Count != 0) SparklineViewTypeToViewFields.Clear();
			foreach(SparklineViewType item in Enum.GetValues(typeof(SparklineViewType)))
				SparklineViewTypeToViewFields.Add(item, SparklineViewFields.None);
		}
		#region SparklineViewBase fields
		const bool defHighlightMaxPoint = false;
		const bool defHighlightMinPoint = false;
		const bool defHighlightStartPoint = false;
		const bool defHighlightEndPoint = false;
		Color defColor = Color.Red;
		public bool DefaultHighlightMaxPoint { get { return defHighlightMaxPoint; } }
		public bool DefaultHighlightMinPoint { get { return defHighlightMinPoint; } }
		public bool DefaultHighlightStartPoint { get { return defHighlightStartPoint; } }
		public bool DefaultHighlightEndPoint { get { return defHighlightEndPoint; } }
		public Color DefaultColor { get { return defColor; } }
		public Color DefaultMaxPointColor { get { return Color.Empty; } }
		public Color DefaultMinPointColor { get { return Color.Empty; } }
		public Color DefaultStartPointColor { get { return Color.Empty; } }
		public Color DefaultEndPointColor { get { return Color.Empty; } }
		public Color DefaultNegativePointColor { get { return Color.Empty; } }
		bool highlightMaxPoint;
		bool highlightMinPoint;
		bool highlightStartPoint;
		bool highlightEndPoint;
		Color color;
		Color maxPointColor;
		Color minPointColor;
		Color startPointColor;
		Color endPointColor;
		Color negativePointColor;
		public bool HighlightMaxPoint {
			get { return highlightMaxPoint; }
			set {
				if(highlightMaxPoint == value) return;
				highlightMaxPoint = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.HighlightMaxPoint);
			}
		}
		public bool HighlightMinPoint {
			get { return highlightMinPoint; }
			set {
				if(highlightMinPoint == value) return;
				highlightMinPoint = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.HighlightMinPoint);
			}
		}
		public bool HighlightStartPoint {
			get { return highlightStartPoint; }
			set {
				if(highlightStartPoint == value) return;
				highlightStartPoint = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.HighlightStartPoint);
			}
		}
		public bool HighlightEndPoint {
			get { return highlightEndPoint; }
			set {
				if(highlightEndPoint == value) return;
				highlightEndPoint = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.HighlightEndPoint);
			}
		}
		public Color Color {
			get { return color; }
			set {
				if(color == value) return;
				color = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.Color);
			}
		}
		public Color MaxPointColor {
			get { return maxPointColor; }
			set {
				if(maxPointColor == value) return;
				maxPointColor = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.MaxPointColor);
			}
		}
		public Color MinPointColor {
			get { return minPointColor; }
			set {
				if(minPointColor == value) return;
				minPointColor = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.MinPointColor);
			}
		}
		public Color StartPointColor {
			get { return startPointColor; }
			set {
				if(startPointColor == value) return;
				startPointColor = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.StartPointColor);
			}
		}
		public Color EndPointColor {
			get { return endPointColor; }
			set {
				if(endPointColor == value) return;
				endPointColor = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.EndPointColor);
			}
		}
		public Color NegativePointColor {
			get { return negativePointColor; }
			set {
				if(negativePointColor == value) return;
				negativePointColor = value;
				OnPropertyChanged(FieldViewType.All, SparklineViewFields.NegativePointColor);
			}
		}
		#endregion SparklineViewBase fields
		#region Line fields
		const int defLineWidth = 1;
		const bool defHighlightNegativePoints = false;
		const bool defShowMarkers = false;
		const int defMarkerSize = 5;
		const int defMaxPointMarkerSize = 5;
		const int defMinPointMarkerSize = 5;
		const int defStartPointMarkerSize = 5;
		const int defEndPointMarkerSize = 5;
		const int defNegativePointMarkerSize = 5;
		public int DefaultLineWidth { get { return defLineWidth; } }
		public bool DefaultHighlightNegativePoints { get { return defHighlightNegativePoints; } }
		public bool DefaultShowMarkers { get { return defShowMarkers; } }
		public int DefaultMarkerSize { get { return defMarkerSize; } }
		public int DefaultMaxPointMarkerSize { get { return defMaxPointMarkerSize; } }
		public int DefaultMinPointMarkerSize { get { return defMinPointMarkerSize; } }
		public int DefaultStartPointMarkerSize { get { return defStartPointMarkerSize; } }
		public int DefaultEndPointMarkerSize { get { return defEndPointMarkerSize; } }
		public int DefaultNegativePointMarkerSize { get { return defNegativePointMarkerSize; } }
		public Color DefaultMarkerColor { get { return Color.Empty; } }
		int lineWidth;
		bool highlightNegativePoints;
		bool showMarkers;
		int markerSize;
		int maxPointMarkerSize;
		int minPointMarkerSize;
		int startPointMarkerSize;
		int endPointMarkerSize;
		int negativePointMarkerSize;
		Color markerColor;
		public int LineWidth {
			get { return lineWidth; }
			set {
				if(value < 1)
					throw new ArgumentException();
				if(lineWidth == value) return;
				lineWidth = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.LineWidth);
			}
		}
		public bool HighlightNegativePoints {
			get { return highlightNegativePoints; }
			set {
				if(highlightNegativePoints == value) return;
				highlightNegativePoints = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area | FieldViewType.Bar, SparklineViewFields.HighlightNegativePoints);
			}
		}
		public bool ShowMarkers {
			get { return showMarkers; }
			set {
				if(showMarkers == value) return;
				showMarkers = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.ShowMarkers);
			}
		}
		public int MarkerSize {
			get { return markerSize; }
			set {
				if(markerSize == value) return;
				markerSize = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.MarkerSize);
			}
		}
		public int MaxPointMarkerSize {
			get { return maxPointMarkerSize; }
			set {
				if(maxPointMarkerSize == value) return;
				maxPointMarkerSize = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.MaxPointMarkerSize);
			}
		}
		public int MinPointMarkerSize {
			get { return minPointMarkerSize; }
			set {
				if(minPointMarkerSize == value) return;
				minPointMarkerSize = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.MinPointMarkerSize);
			}
		}
		public int StartPointMarkerSize {
			get { return startPointMarkerSize; }
			set {
				if(startPointMarkerSize == value) return;
				startPointMarkerSize = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.StartPointMarkerSize);
			}
		}
		public int EndPointMarkerSize {
			get { return endPointMarkerSize; }
			set {
				if(endPointMarkerSize == value) return;
				endPointMarkerSize = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.EndPointMarkerSize);
			}
		}
		public int NegativePointMarkerSize {
			get { return negativePointMarkerSize; }
			set {
				if(negativePointMarkerSize == value) return;
				negativePointMarkerSize = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.NegativePointMarkerSize);
			}
		}
		public Color MarkerColor {
			get { return markerColor; }
			set {
				if(markerColor == value) return;
				markerColor = value;
				OnPropertyChanged(FieldViewType.Line | FieldViewType.Area, SparklineViewFields.MarkerColor);
			}
		}
		#endregion Line fields
		#region Area fields
		const byte defAreaOpacity = 135;
		public byte DefaultAreaOpacity { get { return defAreaOpacity; } }
		byte areaOpacity;
		public byte AreaOpacity {
			get { return areaOpacity; }
			set {
				if(areaOpacity == value) return;
				areaOpacity = value;
				OnPropertyChanged(FieldViewType.Area, SparklineViewFields.AreaOpacity);
			}
		}
		#endregion Area fields
		#region BarSparklineViewBase fields
		const int defBarDistance = 2;
		public int DefaultBarDistance { get { return defBarDistance; } }
		int barDistance;
		public int BarDistance {
			get { return barDistance; }
			set {
				if(barDistance == value) return;
				barDistance = value;
				OnPropertyChanged(FieldViewType.Bar | FieldViewType.WinLoss, SparklineViewFields.AreaOpacity);
			}
		}
		#endregion BarSparklineViewBase fields
		public override bool CanResize { get { return true; } }
		private void OnPropertyChanged(FieldViewType fieldViewType, SparklineViewFields flags) {
			if(fieldViewType.HasFlag(FieldViewType.Area))
				SparklineViewTypeToViewFields[SparklineViewType.Area] |= flags;
			if(fieldViewType.HasFlag(FieldViewType.Bar))
				SparklineViewTypeToViewFields[SparklineViewType.Bar] |= flags;
			if(fieldViewType.HasFlag(FieldViewType.Line))
				SparklineViewTypeToViewFields[SparklineViewType.Line] |= flags;
			if(fieldViewType.HasFlag(FieldViewType.WinLoss))
				SparklineViewTypeToViewFields[SparklineViewType.WinLoss] |= flags;
		}
		void CustomizeCurrentView() {
			if(SparklineViewTypeToViewFields[ViewType] == SparklineViewFields.None) return;
			SetBarDistance();
			SetAreaOpacity();
			SetLineWidth();
			SetMarkers();
			SetViewColor();
			SetPointColors();
			SetPointHighlight();
		}
		void SetBarDistance() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.BarDistance)) {
				BarSparklineViewBase barSparklineView = View as BarSparklineViewBase;
				if (barSparklineView != null)
					barSparklineView.BarDistance = BarDistance;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.BarDistance;
			}
		}
		void SetAreaOpacity() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.AreaOpacity)) {
				AreaSparklineView areaSparklineView = View as AreaSparklineView;
				if (areaSparklineView != null)
					areaSparklineView.AreaOpacity = AreaOpacity;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.AreaOpacity;
			}
		}
		void SetLineWidth() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.LineWidth)) {
				LineSparklineView lineSparklineView = View as LineSparklineView;
				if (lineSparklineView != null)
					lineSparklineView.LineWidth = LineWidth;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.LineWidth;
			}
		}
		void SetMarkers() {
			LineSparklineView lineSparklineView = View as LineSparklineView;
			SetLineViewShowMarkers(lineSparklineView);
			SetLineViewMarkerSize(lineSparklineView);
			SetLineViewMaxPointMarkerSize(lineSparklineView);
			SetLineViewMinPointMarkerSize(lineSparklineView);
			SetLineViewStartPointMarkerSize(lineSparklineView);
			SetLineViewEndPointMarkerSize(lineSparklineView);
			SetLineViewNegativePointMarkerSize(lineSparklineView);
			SetLineViewMarkerColor(lineSparklineView);
		}
		void SetLineViewShowMarkers(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.ShowMarkers)) {
				if (lineSparklineView != null)
					lineSparklineView.ShowMarkers = ShowMarkers;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.ShowMarkers;
			}
		}
		void SetLineViewMarkerSize(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.MarkerSize)) {
				if (lineSparklineView != null)
					lineSparklineView.MarkerSize = MarkerSize;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.MarkerSize;
			}
		}
		void SetLineViewMaxPointMarkerSize(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.MaxPointMarkerSize)) {
				if (lineSparklineView != null)
					lineSparklineView.MaxPointMarkerSize = MaxPointMarkerSize;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.MaxPointMarkerSize;
			}
		}
		void SetLineViewMinPointMarkerSize(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.MinPointMarkerSize)) {
				if (lineSparklineView != null)
					lineSparklineView.MinPointMarkerSize = MinPointMarkerSize;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.MinPointMarkerSize;
			}
		}
		void SetLineViewStartPointMarkerSize(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.StartPointMarkerSize)) {
				if (lineSparklineView != null)
					lineSparklineView.StartPointMarkerSize = StartPointMarkerSize;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.StartPointMarkerSize;
			}
		}
		void SetLineViewEndPointMarkerSize(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.EndPointMarkerSize)) {
				if (lineSparklineView != null)
					lineSparklineView.EndPointMarkerSize = EndPointMarkerSize;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.EndPointMarkerSize;
			}
		}
		void SetLineViewNegativePointMarkerSize(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.NegativePointMarkerSize)) {
				if (lineSparklineView != null)
					lineSparklineView.NegativePointMarkerSize = NegativePointMarkerSize;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.NegativePointMarkerSize;
			}
		}
		void SetLineViewMarkerColor(LineSparklineView lineSparklineView) {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.MarkerColor)) {
				if (lineSparklineView != null)
					lineSparklineView.MarkerColor = MarkerColor;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.MarkerColor;
			}
		}
		void SetViewColor() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.Color)) {
				View.Color = Color;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.Color;
			}
		}
		void SetPointHighlight() {
			SetHighlightMaxPoint();
			SetHighlightMinPoint();
			SetHighlightStartPoint();
			SetHighlightEndPoint();
			SetHighlightNegativePoints();
		}
		void SetHighlightMaxPoint() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.HighlightMaxPoint)) {
				View.HighlightMaxPoint = HighlightMaxPoint;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.HighlightMaxPoint;
			}
		}
		void SetHighlightMinPoint() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.HighlightMinPoint)) {
				View.HighlightMinPoint = HighlightMinPoint;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.HighlightMinPoint;
			}
		}
		void SetHighlightStartPoint() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.HighlightStartPoint)) {
				View.HighlightStartPoint = HighlightStartPoint;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.HighlightStartPoint;
			}
		}
		void SetHighlightEndPoint() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.HighlightEndPoint)) {
				View.HighlightEndPoint = HighlightEndPoint;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.HighlightEndPoint;
			}
		}
		void SetHighlightNegativePoints() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.HighlightNegativePoints)) {
				ISupportNegativePointsControl negativePointsControl = View as ISupportNegativePointsControl;
				if (negativePointsControl != null)
					negativePointsControl.HighlightNegativePoints = HighlightNegativePoints;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.HighlightNegativePoints;
			}
		}
		void SetPointColors() {
			SetMaxPointColor();
			SetMinPointColor();
			SetStartPointColor();
			SetEndPointColor();
			SetNegativePointColor();
		}
		void SetMaxPointColor() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.MaxPointColor)) {
				View.MaxPointColor = MaxPointColor;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.MaxPointColor;
			}
		}
		void SetMinPointColor() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.MinPointColor)) {
				View.MinPointColor = MinPointColor;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.MinPointColor;
			}
		}
		void SetStartPointColor() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.StartPointColor)) {
				View.StartPointColor = StartPointColor;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.StartPointColor;
			}
		}
		void SetEndPointColor() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.EndPointColor)) {
				View.EndPointColor = EndPointColor;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.EndPointColor;
			}
		}
		void SetNegativePointColor() {
			if (SparklineViewTypeToViewFields[ViewType].HasFlag(SparklineViewFields.NegativePointColor)) {
				View.NegativePointColor = NegativePointColor;
				SparklineViewTypeToViewFields[ViewType] &= ~SparklineViewFields.NegativePointColor;
			}
		}
		Dictionary<SparklineViewType, SparklineViewBase> viewTypeViewBase = new Dictionary<SparklineViewType, SparklineViewBase>();
		void InitView() {
			SparklineViewBase view;
			if(!viewTypeViewBase.TryGetValue(ViewType, out view)) {
				view = CreateViewBaseByViewType();
				viewTypeViewBase.Add(ViewType, view);
			}
			sparklineSettings.SetView(view);
		}
		SparklineViewBase CreateViewBaseByViewType() {
			switch(ViewType) {
				case SparklineViewType.Line:
					return new LineSparklineView(); 
				case SparklineViewType.Area:
					return new AreaSparklineView();
				case SparklineViewType.Bar:
					return new BarSparklineView();
				case SparklineViewType.WinLoss:
					return new WinLossSparklineView();
			}
			return null;
		}
		void SetViewType(InstructionCollection instructions) {
			string viewType = instructions.GetString(SnSparklineViewTypeSwitch);
			ViewType = !String.IsNullOrEmpty(viewType) ? (SparklineViewType)Enum.Parse(typeof(SparklineViewType), viewType, true)
				: SparklineViewType.Line;
		}
		void SetWidth(InstructionCollection instructions) {
			string width = instructions.GetString(SnSparklineWidthSwitch);
			Width = !String.IsNullOrEmpty(width) ? instructions.GetInt(SnSparklineWidthSwitch) : DefaultSize.Width;
		}
		void SetHeight(InstructionCollection instructions) {
			string height = instructions.GetString(SnSparklineHeightSwitch);
			Height = !String.IsNullOrEmpty(height) ? instructions.GetInt(SnSparklineHeightSwitch) : DefaultSize.Height;
		}
		void SetDataSourceName(InstructionCollection instructions) {
			string dataSourceName = instructions.GetString(SnSparklineDataSourceNameSwitch);
			this.dataSourceName = !String.IsNullOrEmpty(dataSourceName) ? dataSourceName : String.Empty;
		}
		void SetHighlightMaxPoint(InstructionCollection instructions) {
			string highlightMaxPoint = instructions.GetString(SnSparklineHighlightMaxPointSwitch);
			HighlightMaxPoint = !String.IsNullOrEmpty(highlightMaxPoint) ? Convert.ToBoolean(highlightMaxPoint) : defHighlightMaxPoint;
		}
		void SetHighlightMinPoint(InstructionCollection instructions) {
			string highlightMinPoint = instructions.GetString(SnSparklineHighlightMinPointSwitch);
			HighlightMinPoint = !String.IsNullOrEmpty(highlightMinPoint) ? Convert.ToBoolean(highlightMinPoint) : defHighlightMinPoint;
		}
		void SetHighlightStartPoint(InstructionCollection instructions) {
			string highlightStartPoint = instructions.GetString(SnSparklineHighlightStartPointSwitch);
			HighlightStartPoint = !String.IsNullOrEmpty(highlightStartPoint) ? Convert.ToBoolean(highlightStartPoint) : defHighlightStartPoint;
		}
		void SetHighlightEndPoint(InstructionCollection instructions) {
			string highlightEndPoint = instructions.GetString(SnSparklineHighlightEndPointSwitch);
			HighlightEndPoint = !String.IsNullOrEmpty(highlightEndPoint) ? Convert.ToBoolean(highlightEndPoint) : defHighlightEndPoint;
		}
		void SetColor(InstructionCollection instructions) {
			string color = instructions.GetString(SnSparklineColorSwitch);
			Color = !String.IsNullOrEmpty(color) ? Color.FromArgb(instructions.GetInt(SnSparklineColorSwitch)) : defColor;
		}
		void SetMaxPointColor(InstructionCollection instructions) {
			string maxPointColor = instructions.GetString(SnSparklineMaxPointColorSwitch);
			MaxPointColor = !String.IsNullOrEmpty(maxPointColor) ? Color.FromArgb(instructions.GetInt(SnSparklineMaxPointColorSwitch)) : Color.Empty;
		}
		void SetMinPointColor(InstructionCollection instructions) {
			string minPointColor = instructions.GetString(SnSparklineMinPointColorSwitch);
			MinPointColor = !String.IsNullOrEmpty(minPointColor) ? Color.FromArgb(instructions.GetInt(SnSparklineMinPointColorSwitch)) : Color.Empty;
		}
		void SetStartPointColor(InstructionCollection instructions) {
			string startPointColor = instructions.GetString(SnSparklineStartPointColorSwitch);
			StartPointColor = !String.IsNullOrEmpty(startPointColor) ? Color.FromArgb(instructions.GetInt(SnSparklineStartPointColorSwitch)) : Color.Empty;
		}
		void SetEndPointColor(InstructionCollection instructions) {
			string endPointColor = instructions.GetString(SnSparklineEndPointColorSwitch);
			EndPointColor = !String.IsNullOrEmpty(endPointColor) ? Color.FromArgb(instructions.GetInt(SnSparklineEndPointColorSwitch)) : Color.Empty;
		}
		void SetNegativePointColor(InstructionCollection instructions) {
			string negativePointColor = instructions.GetString(SnSparklineNegativePointColorSwitch);
			NegativePointColor = !String.IsNullOrEmpty(negativePointColor) ? Color.FromArgb(instructions.GetInt(SnSparklineNegativePointColorSwitch)) : Color.Empty;
		}
		void SetLineWidth(InstructionCollection instructions) {
			string lineWidth = instructions.GetString(SnSparklineLineWidthSwitch);
			LineWidth = !String.IsNullOrEmpty(lineWidth) ? instructions.GetInt(SnSparklineLineWidthSwitch) : defLineWidth;
		}
		void SetHighlightNegativePoints(InstructionCollection instructions) {
			string highlightNegativePoints = instructions.GetString(SnSparklineHighlightNegativePointsSwitch);
			HighlightNegativePoints = !String.IsNullOrEmpty(highlightNegativePoints) ? Convert.ToBoolean(highlightNegativePoints) : defHighlightNegativePoints;
		}
		void SetShowMarkers(InstructionCollection instructions) {
			string showMarkers = instructions.GetString(SnSparklineShowMarkersSwitch);
			ShowMarkers = !String.IsNullOrEmpty(showMarkers) ? Convert.ToBoolean(showMarkers) : defShowMarkers;
		}
		void SetMarkerSize(InstructionCollection instructions) {
			string markerSize = instructions.GetString(SnSparklineMarkerSizeSwitch);
			MarkerSize = !String.IsNullOrEmpty(markerSize) ? instructions.GetInt(SnSparklineMarkerSizeSwitch) : defMarkerSize;
		}
		void SetMaxPointMarkerSize(InstructionCollection instructions) {
			string maxPointMarkerSize = instructions.GetString(SnSparklineMaxPointMarkerSizeSwitch);
			MaxPointMarkerSize = !String.IsNullOrEmpty(maxPointMarkerSize) ? instructions.GetInt(SnSparklineMaxPointMarkerSizeSwitch) : defMaxPointMarkerSize;
		}
		void SetMinPointMarkerSize(InstructionCollection instructions) {
			string minPointMarkerSize = instructions.GetString(SnSparklineMinPointMarkerSizeSwitch);
			MinPointMarkerSize = !String.IsNullOrEmpty(minPointMarkerSize) ? instructions.GetInt(SnSparklineMinPointMarkerSizeSwitch) : defMinPointMarkerSize;
		}
		void SetStartPointMarkerSize(InstructionCollection instructions) {
			string startPointMarkerSize = instructions.GetString(SnSparklineStartPointMarkerSizeSwitch);
			StartPointMarkerSize = !String.IsNullOrEmpty(startPointMarkerSize) ? instructions.GetInt(SnSparklineStartPointMarkerSizeSwitch) : defStartPointMarkerSize;
		}
		void SetEndPointMarkerSize(InstructionCollection instructions) {
			string endPointMarkerSize = instructions.GetString(SnSparklineEndPointMarkerSizeSwitch);
			EndPointMarkerSize = !String.IsNullOrEmpty(endPointMarkerSize) ? instructions.GetInt(SnSparklineEndPointMarkerSizeSwitch) : defEndPointMarkerSize;
		}
		void SetNegativePointMarkerSize(InstructionCollection instructions) {
			string negativePointMarkerSize = instructions.GetString(SnSparklineNegativePointMarkerSizeSwitch);
			NegativePointMarkerSize = !String.IsNullOrEmpty(negativePointMarkerSize) ? instructions.GetInt(SnSparklineNegativePointMarkerSizeSwitch) : defNegativePointMarkerSize;
		}
		void SetMarkerColor(InstructionCollection instructions) {
			string markerColor = instructions.GetString(SnSparklineMarkerColorSwitch);
			MarkerColor = !String.IsNullOrEmpty(markerColor) ? Color.FromArgb(instructions.GetInt(SnSparklineMarkerColorSwitch)) : Color.Empty;
		}
		void SetAreaOpacity(InstructionCollection instructions) {
			string areaOpacity = instructions.GetString(SnSparklineAreaOpacitySwitch);
			AreaOpacity = !String.IsNullOrEmpty(areaOpacity) ? (byte)instructions.GetInt(SnSparklineAreaOpacitySwitch) : defAreaOpacity;
		}
		void SetBarDistance(InstructionCollection instructions) {
			string barDistance = instructions.GetString(SnSparklineBarDistanceSwitch);
			BarDistance = !String.IsNullOrEmpty(barDistance) ? instructions.GetInt(SnSparklineBarDistanceSwitch) : defBarDistance;
		}
		[SecuritySafeCritical]
		Image GetImage() {
			Rectangle bounds = new Rectangle(0, 0, Width, Height);
			Image image = null;
			if(bounds.Width > 0 && bounds.Height > 0) {
				image = new Bitmap(bounds.Width, bounds.Height);
				CustomizeCurrentView();
				BaseSparklinePainter painter = paintersCache.GetPainter(View);
				painter.Initialize(sparklineData, sparklineSettings, bounds);
				using(Graphics graphics = Graphics.FromImage(image)) {
					painter.Draw(graphics, null);
				}
			}
			return image;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(XtraRichEdit.Model.PieceTable sourcePieceTable, XtraRichEdit.Model.MailMergeDataMode mailMergeDataMode, XtraRichEdit.Model.Field documentField) {
			if(!string.IsNullOrEmpty(DataFieldName)) {
				IFieldDataAccessService fieldDataAccessService = sourcePieceTable.DocumentModel.GetService<IFieldDataAccessService>();
				if(fieldDataAccessService == null) return base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
				FieldDataValueDescriptor fieldDataValueDescriptor = fieldDataAccessService.GetFieldValueDescriptor((SnapPieceTable)sourcePieceTable, documentField, DataSourceName);
				ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(((SnapDocumentModel)sourcePieceTable.DocumentModel).DataSourceDispatcher);
				if(!string.IsNullOrEmpty(DataFieldName) && DataFieldName.Contains("."))
					calculationContext.FieldNames = new[] { DataFieldName };
				List<object> res = new List<object>();
				try {
					using(IDataEnumerator enumerator = calculationContext.GetChildDataEnumerator(fieldDataValueDescriptor.ParentDataContext, fieldDataValueDescriptor.RelativePath)) {
						if(enumerator != null && !IsSingleValueEnum(enumerator)) {
							while(enumerator.MoveNext()) {
								object value = enumerator.GetColumnValue(DataFieldName);
								if(value != null) res.Add(value);
							}
						}
					}
					if (EnableEmptyFieldDataAlias)
						if (res.Count == 0)
							return new CalculatedFieldValue(EmptyFieldDataAlias);
					sparklineData.SetValues(GetList(res));
				}
				finally {
					fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);
				}
			}
			SnapDocumentModel documentModel = ((SnapPieceTable)sourcePieceTable).DocumentModel;
			var oldRun = sourcePieceTable.Runs[documentField.Result.Start] as InlinePictureRun;
			if(oldRun != null && (oldRun.ScaleX != 100 || oldRun.ScaleY != 100)) {
				var newSize = SnapSizeConverter.ModelUnitsToPixels(documentModel, oldRun.ActualSize, oldRun.Image.HorizontalResolution);
				if(Width != newSize.Width || Height != newSize.Height) {
					using(InstructionController controller = new InstructionController(sourcePieceTable, this, documentField)) {
						string width = Switches.GetString(SnSparklineWidthSwitch);
						string height = Switches.GetString(SnSparklineHeightSwitch);
						if(String.IsNullOrEmpty(width) || Width != newSize.Width) {
							Width = newSize.Width;
							SNSparklineHelper.SaveSwitchValue(controller, SnSparklineWidthSwitch, Width.ToString());
						}
						if(String.IsNullOrEmpty(height) || Height != newSize.Height) {
							Height = newSize.Height;
							SNSparklineHelper.SaveSwitchValue(controller, SnSparklineHeightSwitch, Height.ToString());
						}
					}
				}
			}
			var img = GetImage();
			if(img == null) return base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);
			OfficeImage image = OfficeImage.CreateImage(img);
			DocumentModel targetModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
			SparklineRun run = ((SnapPieceTable)targetModel.MainPieceTable).InsertSparkline(DocumentLogPosition.Zero, image);
			run.ResizingShadowDisplayMode = ResizingShadowDisplayMode.WireFrame;
			run.LockAspectRatio = false;
			return new CalculatedFieldValue(targetModel);
		}
		bool IsSingleValueEnum(IDataEnumerator enumerator) {
			return enumerator != null && enumerator is SingleValueDataEnumerator;
		}
		private IList<double> GetList(List<object> values) {
			var list = new List<double>();
			try {
				foreach(var value in values) {
					var d = Convert.ToDouble(value);
					list.Add(d);
				}
			}
			catch { }
			return list;
		}
		protected internal override string[] GetNativeSwithes() {
			return new string[] {
				SnSparklineViewTypeSwitch,
				SnSparklineWidthSwitch,
				SnSparklineHeightSwitch,
				SnSparklineHighlightMaxPointSwitch,
				SnSparklineHighlightMinPointSwitch,
				SnSparklineHighlightStartPointSwitch,
				SnSparklineHighlightEndPointSwitch,
				SnSparklineColorSwitch,
				SnSparklineMaxPointColorSwitch,
				SnSparklineMinPointColorSwitch,
				SnSparklineStartPointColorSwitch,
				SnSparklineEndPointColorSwitch,
				SnSparklineNegativePointColorSwitch,
				SnSparklineLineWidthSwitch,
				SnSparklineHighlightNegativePointsSwitch,
				SnSparklineShowMarkersSwitch,
				SnSparklineMarkerSizeSwitch,
				SnSparklineMaxPointMarkerSizeSwitch,
				SnSparklineMinPointMarkerSizeSwitch,
				SnSparklineStartPointMarkerSizeSwitch,
				SnSparklineEndPointMarkerSizeSwitch,
				SnSparklineNegativePointMarkerSizeSwitch,
				SnSparklineMarkerColorSwitch,
				SnSparklineAreaOpacitySwitch,
				SnSparklineBarDistanceSwitch,
				SnSparklineDataSourceNameSwitch,
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		protected internal override string[] GetInvariableSwitches() {
			return new string[] {
				EmptyFieldDataAliasSwitch,
				EnableEmptyFieldDataAliasSwitch
			};
		}
		internal Dictionary<string, string> GetInstructions() {
			Dictionary<string, string> result = new Dictionary<string, string>();
			GetDefaults(result);
			GetHighlightInstructions(result);
			GetColorInstructions(result);
			GetMarkerIstructions(result);
			GetLineAndBarInstructions(result);
			return result;
		}
		void GetDefaults(Dictionary<string, string> instructions) {
			if (ViewType != DefaultViewType)
				instructions.Add(SnSparklineViewTypeSwitch, ViewType.ToString());
			if (Width != DefaultWidth)
				instructions.Add(SnSparklineWidthSwitch, Width.ToString());
			if (Height != DefaultHeight)
				instructions.Add(SnSparklineHeightSwitch, Height.ToString());
		}
		void GetHighlightInstructions(Dictionary<string, string> instructions) {
			if (HighlightMaxPoint != DefaultHighlightMaxPoint)
				instructions.Add(SnSparklineHighlightMaxPointSwitch, HighlightMaxPoint.ToString());
			if (HighlightMinPoint != DefaultHighlightMinPoint)
				instructions.Add(SnSparklineHighlightMinPointSwitch, HighlightMinPoint.ToString());
			if (HighlightStartPoint != DefaultHighlightStartPoint)
				instructions.Add(SnSparklineHighlightStartPointSwitch, HighlightStartPoint.ToString());
			if (HighlightEndPoint != DefaultHighlightEndPoint)
				instructions.Add(SnSparklineHighlightEndPointSwitch, HighlightEndPoint.ToString());
			if (HighlightNegativePoints != DefaultHighlightNegativePoints)
				instructions.Add(SnSparklineHighlightNegativePointsSwitch, HighlightNegativePoints.ToString());
		}
		void GetColorInstructions(Dictionary<string, string> instructions) {
			if (Color != DefaultColor)
				instructions.Add(SnSparklineColorSwitch, Color.ToArgb().ToString());
			if (MaxPointColor != DefaultMaxPointColor)
				instructions.Add(SnSparklineMaxPointColorSwitch, MaxPointColor.ToArgb().ToString());
			if (MinPointColor != DefaultMinPointColor)
				instructions.Add(SnSparklineMinPointColorSwitch, MinPointColor.ToArgb().ToString());
			if (StartPointColor != DefaultStartPointColor)
				instructions.Add(SnSparklineStartPointColorSwitch, StartPointColor.ToArgb().ToString());
			if (EndPointColor != DefaultEndPointColor)
				instructions.Add(SnSparklineEndPointColorSwitch, EndPointColor.ToArgb().ToString());
			if (NegativePointColor != DefaultNegativePointColor)
				instructions.Add(SnSparklineNegativePointColorSwitch, NegativePointColor.ToArgb().ToString());
		}
		void GetMarkerIstructions(Dictionary<string, string> instructions) {
			if (ShowMarkers != DefaultShowMarkers)
				instructions.Add(SnSparklineShowMarkersSwitch, ShowMarkers.ToString());
			if (MarkerSize != DefaultMarkerSize)
				instructions.Add(SnSparklineMarkerSizeSwitch, MarkerSize.ToString());
			if (MaxPointMarkerSize != DefaultMaxPointMarkerSize)
				instructions.Add(SnSparklineMaxPointMarkerSizeSwitch, MaxPointMarkerSize.ToString());
			if (MinPointMarkerSize != DefaultMinPointMarkerSize)
				instructions.Add(SnSparklineMinPointMarkerSizeSwitch, MinPointMarkerSize.ToString());
			if (StartPointMarkerSize != DefaultStartPointMarkerSize)
				instructions.Add(SnSparklineStartPointMarkerSizeSwitch, StartPointMarkerSize.ToString());
			if (EndPointMarkerSize != DefaultEndPointMarkerSize)
				instructions.Add(SnSparklineEndPointMarkerSizeSwitch, EndPointMarkerSize.ToString());
			if (NegativePointMarkerSize != DefaultNegativePointMarkerSize)
				instructions.Add(SnSparklineNegativePointMarkerSizeSwitch, NegativePointMarkerSize.ToString());
			if (MarkerColor != DefaultMarkerColor)
				instructions.Add(SnSparklineMarkerColorSwitch, MarkerColor.ToArgb().ToString());
		}
		void GetLineAndBarInstructions(Dictionary<string, string> instructions) {
			if (LineWidth != DefaultLineWidth)
				instructions.Add(SnSparklineLineWidthSwitch, LineWidth.ToString());
			if (AreaOpacity != DefaultAreaOpacity)
				instructions.Add(SnSparklineAreaOpacitySwitch, AreaOpacity.ToString());
			if (BarDistance != DefaultBarDistance)
				instructions.Add(SnSparklineBarDistanceSwitch, BarDistance.ToString());
		}
	}
	public class SparklineData : ISparklineData {
		static readonly double[] defaultValues = new double[] { 
			56, 44, 39, 38, 41, 41, 40, 46, 36, 47, 35, 47, 41, 29, 44, 43, 31, 32, 23, 36,
			35, 32, 33, 47, 38, 48, 48, 40, 59, 63, 54, 47, 55, 59, 57, 40, 43, 47, 37, 39,
			40, 45, 40, 33, 34, 29, 33, 42, 43, 36, 34, 28, 32, 35, 43, 51, 42, 48, 60, 58, 
			62, 54, 66, 53, 54, 47, 50, 60, 52, 66, 63, 68, 61, 71, 60, 67, 66, 48, 56, 46, 
			46, 52, 38, 39, 42, 36, 28, 34, 32, 25, 38, 39, 32, 27, 26, 37, 32, 19, 34 };
		public void SetValues(IList<double> values) {
			if(this.values != null) {
				this.values.Clear();
			}
			else {
				this.values = new List<double>();
			}
			this.values.AddRange(values);
		}
		List<double> values;
		#region ISparklineData
		public IList<double> Values {
			get {
				if(values != null && values.Count != 0) return values;
				return defaultValues;
			}
		}
		#endregion
		public bool IsEmpty { 
			get {
				return (values == null || values.Count == 0);
			}
		}
	}
	public class SparklineSettings : ISparklineSettings {
		readonly SparklineRange valueRange = new SparklineRange();
		SparklineViewBase view;
		#region ISparklineSettings Members
		public void SetView(SparklineViewBase sparklineView) {
			view = sparklineView;
		}
		public Padding Padding {
			get { return new Padding(0); }
		}
		public SparklineViewBase View {
			get { return view; }
		}
		public SparklineRange ValueRange {
			get { return valueRange; }
		}
		#endregion
	}
	public class SNSparklineFieldController : SizeAndScaleFieldController<SNSparklineField> {
		public SNSparklineFieldController(InstructionController controller)
			: base(controller, GetRectangularObject(controller)) {
		}
		static IRectangularObject GetRectangularObject(InstructionController controller) {
			SparklineRun sparklineRun = controller.PieceTable.Runs[controller.Field.Result.Start] as SparklineRun;
			if (sparklineRun == null)
				return null;
			OfficeImage image = (sparklineRun).Image;
			return new RichEditImageWrapper(image);
		}
		protected override void SetImageSizeInfoCore() {
			SetSize();
		}
	}
}
