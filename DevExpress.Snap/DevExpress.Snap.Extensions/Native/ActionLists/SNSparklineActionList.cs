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
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraPrinting;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Sparkline;
using System.Globalization;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraReports.Design;
using System.ComponentModel;
using DevExpress.Snap.Localization;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native.Parameters;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Design;
using DevExpress.XtraReports.Native;
using DevExpress.Data.Browsing;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Extensions.Localization;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class SNSparklineActionList : FieldActionList<SNSparklineField> {
		public SNSparklineActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_ViewType")]
		public SparklineViewType ViewType {
			get {
				return ParsedInfo.ViewType;
			}
			set {
				if(ViewType == value)
					return;
				ApplyNewValue((controller, viewType) => controller.SetSwitch(SNSparklineField.SnSparklineViewTypeSwitch, SNSparklineField.GetSparklineViewTypeString(viewType)), value);
				SNSmartTagService smartTagService = (SNSmartTagService)this.Component.Site.GetService(typeof(SNSmartTagService));
				smartTagService.UpdatePopup();
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_Width")]
		public int Width {
			get {
				return ParsedInfo.Width;
			}
			set {
				if(Width == value)
					return;
				ApplyNewValue((controller, width) => controller.SetSwitch(SNSparklineField.SnSparklineWidthSwitch, Convert.ToString(width, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_Height")]
		public int Height {
			get {
				return ParsedInfo.Height;
			}
			set {
				if(Height == value)
					return;
				ApplyNewValue((controller, height) => controller.SetSwitch(SNSparklineField.SnSparklineHeightSwitch, Convert.ToString(height, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_HighlightMaxPoint")]
		public bool HighlightMaxPoint {
			get { return ParsedInfo.HighlightMaxPoint; }
			set {
				if(HighlightMaxPoint == value) return;
				ApplyNewValue((controller, highlightMaxPoint) => controller.SetSwitch(SNSparklineField.SnSparklineHighlightMaxPointSwitch, Convert.ToString(highlightMaxPoint)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_HighlightMinPoint")]
		public bool HighlightMinPoint {
			get { return ParsedInfo.HighlightMinPoint; }
			set {
				if(HighlightMinPoint == value) return;
				ApplyNewValue((controller, highlightMinPoint) => controller.SetSwitch(SNSparklineField.SnSparklineHighlightMinPointSwitch, Convert.ToString(highlightMinPoint)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_HighlightStartPoint")]
		public bool HighlightStartPoint {
			get { return ParsedInfo.HighlightStartPoint; }
			set {
				if(HighlightStartPoint == value) return;
				ApplyNewValue((controller, highlightStartPoint) => controller.SetSwitch(SNSparklineField.SnSparklineHighlightStartPointSwitch, Convert.ToString(highlightStartPoint)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_HighlightEndPoint")]
		public bool HighlightEndPoint {
			get { return ParsedInfo.HighlightEndPoint; }
			set {
				if(HighlightEndPoint == value) return;
				ApplyNewValue((controller, highlightEndPoint) => controller.SetSwitch(SNSparklineField.SnSparklineHighlightEndPointSwitch, Convert.ToString(highlightEndPoint)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_Color")]
		public Color Color {
			get { return ParsedInfo.Color; }
			set {
				if(Color == value) return;
				ApplyNewValue((controller, color) => controller.SetSwitch(SNSparklineField.SnSparklineColorSwitch, Convert.ToString(color.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_MaxPointColor")]
		public Color MaxPointColor {
			get { return ParsedInfo.MaxPointColor; }
			set {
				if(MaxPointColor == value) return;
				ApplyNewValue((controller, maxPointColor) => controller.SetSwitch(SNSparklineField.SnSparklineMaxPointColorSwitch, Convert.ToString(maxPointColor.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_MinPointColor")]
		public Color MinPointColor {
			get { return ParsedInfo.MinPointColor; }
			set {
				if(MinPointColor == value) return;
				ApplyNewValue((controller, minPointColor) => controller.SetSwitch(SNSparklineField.SnSparklineMinPointColorSwitch, Convert.ToString(minPointColor.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_StartPointColor")]
		public Color StartPointColor {
			get { return ParsedInfo.StartPointColor; }
			set {
				if(StartPointColor == value) return;
				ApplyNewValue((controller, startPointColor) => controller.SetSwitch(SNSparklineField.SnSparklineStartPointColorSwitch, Convert.ToString(startPointColor.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_EndPointColor")]
		public Color EndPointColor {
			get { return ParsedInfo.EndPointColor; }
			set {
				if(EndPointColor == value) return;
				ApplyNewValue((controller, endPointColor) => controller.SetSwitch(SNSparklineField.SnSparklineEndPointColorSwitch, Convert.ToString(endPointColor.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_NegativePointColor")]
		public Color NegativePointColor {
			get { return ParsedInfo.NegativePointColor; }
			set {
				if(NegativePointColor == value) return;
				ApplyNewValue((controller, negativePointColor) => controller.SetSwitch(SNSparklineField.SnSparklineNegativePointColorSwitch, Convert.ToString(negativePointColor.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "ViewType", "ViewType");
			AddPropertyItem(actionItems, "Width", "Width");
			AddPropertyItem(actionItems, "Height", "Height");
			AddPropertyItem(actionItems, "HighlightMaxPoint", "HighlightMaxPoint");
			AddPropertyItem(actionItems, "HighlightMinPoint", "HighlightMinPoint");
			AddPropertyItem(actionItems, "HighlightStartPoint", "HighlightStartPoint");
			AddPropertyItem(actionItems, "HighlightEndPoint", "HighlightEndPoint");
			AddPropertyItem(actionItems, "Color", "Color");
			AddPropertyItem(actionItems, "MaxPointColor", "MaxPointColor");
			AddPropertyItem(actionItems, "MinPointColor", "MinPointColor");
			AddPropertyItem(actionItems, "StartPointColor", "StartPointColor");
			AddPropertyItem(actionItems, "EndPointColor", "EndPointColor");
			AddPropertyItem(actionItems, "NegativePointColor", "NegativePointColor");
		}
	}
	public class SNAreaSparklineActionList : SNLineSparklineActionList {
		public SNAreaSparklineActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_AreaOpacity")]
		public byte AreaOpacity {
			get {
				return ParsedInfo.AreaOpacity;
			}
			set {
				if(AreaOpacity == value)
					return;
				ApplyNewValue((controller, areaOpacity) => controller.SetSwitch(SNSparklineField.SnSparklineAreaOpacitySwitch, Convert.ToString(areaOpacity, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "AreaOpacity", "AreaOpacity");
		}
	}
	public class SNBarBaseSparklineActionList : FieldActionList<SNSparklineField> {
		public SNBarBaseSparklineActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_BarDistance")]
		public int BarDistance {
			get {
				return ParsedInfo.BarDistance;
			}
			set {
				if(BarDistance == value)
					return;
				ApplyNewValue((controller, barDistance) => controller.SetSwitch(SNSparklineField.SnSparklineBarDistanceSwitch, Convert.ToString(barDistance, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "BarDistance", "BarDistance");
		}
	}
	public class SNBarSparklineActionList : SNBarBaseSparklineActionList {
		public SNBarSparklineActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_HighlightNegativePoints")]
		public bool HighlightNegativePoints {
			get {
				return ParsedInfo.HighlightNegativePoints;
			}
			set {
				if(HighlightNegativePoints == value)
					return;
				ApplyNewValue((controller, highlightNegativePoints) => controller.SetSwitch(SNSparklineField.SnSparklineHighlightNegativePointsSwitch, Convert.ToString(highlightNegativePoints)), value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "HighlightNegativePoints", "HighlightNegativePoints");
		}
	}
	public class SNLineSparklineActionList : FieldActionList<SNSparklineField> {
		public SNLineSparklineActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_LineWidth")]
		public int LineWidth {
			get {
				return ParsedInfo.LineWidth;
			}
			set {
				if(LineWidth == value)
					return;
				ApplyNewValue((controller, lineWidth) => controller.SetSwitch(SNSparklineField.SnSparklineLineWidthSwitch, Convert.ToString(lineWidth, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_HighlightNegativePoints")]
		public bool HighlightNegativePoints {
			get {
				return ParsedInfo.HighlightNegativePoints;
			}
			set {
				if(HighlightNegativePoints == value)
					return;
				ApplyNewValue((controller, highlightNegativePoints) => controller.SetSwitch(SNSparklineField.SnSparklineHighlightNegativePointsSwitch, Convert.ToString(highlightNegativePoints)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_ShowMarkers")]
		public bool ShowMarkers {
			get {
				return ParsedInfo.ShowMarkers;
			}
			set {
				if(ShowMarkers == value)
					return;
				ApplyNewValue((controller, showMarkers) => controller.SetSwitch(SNSparklineField.SnSparklineShowMarkersSwitch, Convert.ToString(showMarkers)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_MarkerSize")]
		public int MarkerSize {
			get {
				return ParsedInfo.MarkerSize;
			}
			set {
				if(MarkerSize == value)
					return;
				ApplyNewValue((controller, markerSize) => controller.SetSwitch(SNSparklineField.SnSparklineMarkerSizeSwitch, Convert.ToString(markerSize, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_MaxPointMarkerSize")]
		public int MaxPointMarkerSize {
			get {
				return ParsedInfo.MaxPointMarkerSize;
			}
			set {
				if(MaxPointMarkerSize == value)
					return;
				ApplyNewValue((controller, maxPointMarkerSize) => controller.SetSwitch(SNSparklineField.SnSparklineMaxPointMarkerSizeSwitch, Convert.ToString(maxPointMarkerSize, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_MinPointMarkerSize")]
		public int MinPointMarkerSize {
			get {
				return ParsedInfo.MinPointMarkerSize;
			}
			set {
				if(MinPointMarkerSize == value)
					return;
				ApplyNewValue((controller, minPointMarkerSize) => controller.SetSwitch(SNSparklineField.SnSparklineMinPointMarkerSizeSwitch, Convert.ToString(minPointMarkerSize, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_StartPointMarkerSize")]
		public int StartPointMarkerSize {
			get {
				return ParsedInfo.StartPointMarkerSize;
			}
			set {
				if(StartPointMarkerSize == value)
					return;
				ApplyNewValue((controller, startPointMarkerSize) => controller.SetSwitch(SNSparklineField.SnSparklineStartPointMarkerSizeSwitch, Convert.ToString(startPointMarkerSize, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_EndPointMarkerSize")]
		public int EndPointMarkerSize {
			get {
				return ParsedInfo.EndPointMarkerSize;
			}
			set {
				if(EndPointMarkerSize == value)
					return;
				ApplyNewValue((controller, endPointMarkerSize) => controller.SetSwitch(SNSparklineField.SnSparklineEndPointMarkerSizeSwitch, Convert.ToString(endPointMarkerSize, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_NegativePointMarkerSize")]
		public int NegativePointMarkerSize {
			get {
				return ParsedInfo.NegativePointMarkerSize;
			}
			set {
				if(NegativePointMarkerSize == value)
					return;
				ApplyNewValue((controller, negativePointMarkerSize) => controller.SetSwitch(SNSparklineField.SnSparklineNegativePointMarkerSizeSwitch, Convert.ToString(negativePointMarkerSize, NumberFormatInfo.InvariantInfo)), value);
			}
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.SparklineSmartTagItem_MarkerColor")]
		public Color MarkerColor {
			get {
				return ParsedInfo.MarkerColor;
			}
			set {
				if(MarkerColor == value)
					return;
				ApplyNewValue((controller, markerColor) => controller.SetSwitch(SNSparklineField.SnSparklineMarkerColorSwitch, Convert.ToString(markerColor.ToArgb(), NumberFormatInfo.InvariantInfo)), value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "LineWidth", "LineWidth");
			AddPropertyItem(actionItems, "HighlightNegativePoints", "HighlightNegativePoints");
			AddPropertyItem(actionItems, "ShowMarkers", "ShowMarkers");
			AddPropertyItem(actionItems, "MarkerSize", "MarkerSize");
			AddPropertyItem(actionItems, "MaxPointMarkerSize", "MaxPointMarkerSize");
			AddPropertyItem(actionItems, "MinPointMarkerSize", "MinPointMarkerSize");
			AddPropertyItem(actionItems, "StartPointMarkerSize", "StartPointMarkerSize");
			AddPropertyItem(actionItems, "EndPointMarkerSize", "EndPointMarkerSize");
			AddPropertyItem(actionItems, "NegativePointMarkerSize", "NegativePointMarkerSize");
			AddPropertyItem(actionItems, "MarkerColor", "MarkerColor");
		}
	}
	public class SNWinLossSparklineActionList : SNBarBaseSparklineActionList {
		public SNWinLossSparklineActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
	}
	#region SparklineDataBindingActionList
	public class SparklineDataBindingActionList : FieldActionList<SNMergeFieldSupportsEmptyFieldDataAlias> {
		IDataSourceCollectionProvider dataSourceCollectionProvider;
		public SparklineDataBindingActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
				dataSourceCollectionProvider = (IDataSourceCollectionProvider)serviceProvider.GetService(typeof(IDataSourceCollectionProvider));
		}
		[
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.Snap.Extensions.Native.ActionLists.SnapDesignBindingEditor," + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))
		]
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_Binding")]
		public SnapDesignBinding Binding {
			get { return GetSnapDesignBinding(); }
			set {
				string name = GetDataFieldName(value);
				string source = GetDataSourceName(value);
				if(string.IsNullOrEmpty(name)) source = string.Empty;
				ApplyNewValue((controller, newDataSourceName) => UpdateBindingValues(controller, SNSparklineField.SnSparklineDataSourceNameSwitch, newDataSourceName), new BindingInfo() { SourceName = source, FieldName = name });
			}
		}
		[
	   RefreshProperties(RefreshProperties.All),
	   ]
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_EmptyFieldDataAlias")]
		public string EmptyFieldDataAlias {
			get {
				return ParsedInfo.EmptyFieldDataAlias;
			}
			set {
				if (string.IsNullOrEmpty(value))
					ApplyNewValue((controller, newMode) => controller.RemoveSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EmptyFieldDataAliasSwitch), string.Empty);
				else if (EmptyFieldDataAlias != value) {
					ApplyNewValue((controller, newText) => controller.SetSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EmptyFieldDataAliasSwitch, newText), value);
				}
			}
		}
		[RefreshProperties(RefreshProperties.All),]
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_EnableEmptyFieldDataAlias")]
		public bool EnableEmptyFieldDataAlias {
			get {
				return ParsedInfo.EnableEmptyFieldDataAlias;
			}
			set {
				ApplyNewValue(ChangeEnableEmptyFieldDataAlias, value);
			}
		}
		void ChangeEnableEmptyFieldDataAlias(InstructionController controller, bool useAlias) {
			if (useAlias)
				controller.SetSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EnableEmptyFieldDataAliasSwitch);
			else
				controller.RemoveSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EnableEmptyFieldDataAliasSwitch);
		}
		SnapDesignBinding GetSnapDesignBinding() {
			CalculatedFieldBase calculatedFieldBase = FieldsHelper.GetParsedInfoCore(FieldInfo.PieceTable, FieldInfo.Field);
			SNSparklineField sparklineField = calculatedFieldBase as SNSparklineField;
			object dataSource = null;
			string dataMember = string.Empty;
			string sourceName = sparklineField.DataSourceName;
			string fieldName = sparklineField.DataFieldName;
			if(sparklineField != null)
				dataMember = sourceName + (string.IsNullOrEmpty(sourceName) ? fieldName : string.Format(".{0}", fieldName));
			IFieldDataAccessService dataAccessService = FieldInfo.PieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			if(dataAccessService != null) {
				IFieldPathService fieldPathService = dataAccessService.FieldPathService;
				FieldPathInfo fieldPathInfo = fieldPathService.FromString(sourceName);
				if(IsFieldPathInfoRoot(fieldPathInfo)) {
					RootFieldDataSourceInfo rootFieldDataSourceInfo = fieldPathInfo.DataSourceInfo as RootFieldDataSourceInfo;
					dataSource = FieldInfo.PieceTable.DocumentModel.DataSources.GetDataSourceByName(rootFieldDataSourceInfo.Name);
					dataMember = dataMember.Replace(string.Format("/{0}", rootFieldDataSourceInfo.Name), string.Empty);
					dataMember = dataMember.TrimStart('.');
				}
				else {
					string contextSource = GetContextBinding();
					dataSource = FieldInfo.PieceTable.DocumentModel.DataSources.Count != 0 ? FieldInfo.PieceTable.DocumentModel.DataSources.DefaultDataSourceInfo.DataSource : null;
					dataSource = dataSource ?? GetRootDataSource();
					string res = string.Empty;
					if(!string.IsNullOrEmpty(contextSource)) {
						res = contextSource;
						if(!string.IsNullOrEmpty(dataMember))
							res += string.Format(".{0}", dataMember);
						dataMember = res;
					}
				}
			}
			return new SnapDesignBinding(dataSource, dataMember);
		}
		struct BindingInfo {
			public string SourceName;
			public string FieldName;
		}
		void UpdateBindingValues(InstructionController controller, string name, BindingInfo info) {
			SNSparklineHelper.SaveBindingValues(controller, SNSparklineField.SnSparklineDataSourceNameSwitch, info.SourceName, info.FieldName);
		}
		object GetRootDataSource() {
			var rootField = GetRootField();
			if(rootField == null) return null;
			IFieldDataAccessService dataAccessService = FieldInfo.PieceTable.DocumentModel.GetService<IFieldDataAccessService>();
			CalculatedFieldBase rootFieldBase = FieldsHelper.GetParsedInfoCore(FieldInfo.PieceTable, rootField);
			IDataFieldNameOwner rootFieldNameOwner = rootFieldBase as IDataFieldNameOwner;
			if(rootFieldNameOwner == null) return null;
			FieldPathInfo rootFieldPathInfo = dataAccessService.FieldPathService.FromString(rootFieldNameOwner.DataFieldName);
			if(IsFieldPathInfoRoot(rootFieldPathInfo))
				return FieldInfo.PieceTable.DocumentModel.DataSources.GetDataSourceByName(((RootFieldDataSourceInfo)rootFieldPathInfo.DataSourceInfo).Name);
			return null;
		}
		Field GetRootField() {
			var field = FieldInfo.Field;
			Field root = field;
			while(field != null) {
				root = field;
				field = field.Parent;
			}
			return root;
		}
		bool IsFieldPathInfoRoot(FieldPathInfo fieldPathInfo) {
			return fieldPathInfo.DataSourceInfo.FieldDataSourceType == FieldDataSourceType.Root;
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Binding", "Binding");
			AddPropertyItem(actionItems, "EmptyFieldDataAlias", "EmptyFieldDataAlias");
			AddPropertyItem(actionItems, "EnableEmptyFieldDataAlias", "EnableEmptyFieldDataAlias");
		}
		string GetDataSourceName(SnapDesignBinding designBinding) {
			SNDataInfo dataInfo = designBinding != null ? designBinding.SelectedFieldDataInfo : null;
			return SparklineBindingHelper.GetDataSourceNameFromDataInfo(dataInfo, FieldInfo, ((SparklineDataSourceCollectionProvider)dataSourceCollectionProvider).DataSourceName);
		}
		private string GetContextBinding() {
			return SparklineBindingHelper.GetContextBinding(FieldInfo);
		}
		string GetDataFieldName(SnapDesignBinding designBinding) {
			if(designBinding == null) return string.Empty;
			return SparklineBindingHelper.GetDataFieldNameFromDataInfo(designBinding.SelectedFieldDataInfo);
		}
	}
	#endregion SparklineDataBindingActionList
	#region SparklineActionListDesigner
	public class SparklineActionListDesigner : MergeFieldActionListDesigner<SNSparklineField> {
		static Dictionary<SparklineViewType, Type> sparklineActionListTypes = new Dictionary<SparklineViewType, Type>();
		static SparklineActionListDesigner() {
			sparklineActionListTypes[SparklineViewType.Area] = typeof(SNAreaSparklineActionList);
			sparklineActionListTypes[SparklineViewType.Bar] = typeof(SNBarSparklineActionList);
			sparklineActionListTypes[SparklineViewType.Line] = typeof(SNLineSparklineActionList);
			sparklineActionListTypes[SparklineViewType.WinLoss] = typeof(SNWinLossSparklineActionList);
		}
		public SparklineActionListDesigner(SnapFieldInfo fieldInfo, SNSparklineField parsedInfo, SNSmartTagService frSmartTagService)
			: base(fieldInfo, parsedInfo, frSmartTagService) {
		}
		protected override IDataSourceCollectionProvider GetDataSourceCollectionProvider() {
			return new SparklineDataSourceCollectionProvider();
		}
		protected override void RegisterActionLists(ActionListCollection list) {
			list.Add(new ContentTypeActionList(FieldInfo, this));
			list.Add(new SparklineDataBindingActionList(FieldInfo, this));
			list.Add(new SNSparklineActionList(FieldInfo, this));
			AddViewTypeSparklineActionList(list);
		}
		void AddViewTypeSparklineActionList(ActionListCollection list) {
			if(sparklineActionListTypes.ContainsKey(ParsedInfo.ViewType)) {
				IDesignerActionList actionList = (IDesignerActionList)Activator.CreateInstance(sparklineActionListTypes[ParsedInfo.ViewType], FieldInfo, this);
				list.Add(actionList);
			}
		}
	}
	#endregion
	public class SnapDesignBindingEditor : DesignBindingEditor {
		protected override object GetEditValue(DesignTreeListBindingPicker designTreeListBindingPicker) {
			SnapDesignBinding snapDesignBinding = new SnapDesignBinding(designTreeListBindingPicker.SelectedBinding.DataSource, designTreeListBindingPicker.SelectedBinding.DataMember);
			snapDesignBinding.SelectedFieldDataInfo = SnapFieldListTreeView.GetFieldSNDataInfoByTreeListNode(designTreeListBindingPicker.SelectedNode);
			return snapDesignBinding;
		}
	}
}
