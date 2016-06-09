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
using System.ComponentModel;
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Controls;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum DataFieldNaming { FieldName, Name };
	public class PivotGridOptionsDataField : PivotGridOptionsBase {
		internal const string WrongDataFieldLocationText = "Wrong data field location.";
		PivotGridData data;
		PivotDataArea area;
		int areaIndex;
		int rowHeaderWidth;
		string caption;
		DataFieldNaming fieldNaming;
		bool enableFilteringByData;
		int columnValueLineCount, rowValueLineCount;
		public PivotGridOptionsDataField(PivotGridData data)
			: this(data, null) {
		}
		public PivotGridOptionsDataField(PivotGridData data, EventHandler optionsChanged)
			: this(data, optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsDataField(PivotGridData data, IViewBagOwner owner, string objectPath)
			: this(data, null, owner, objectPath) {
		}
		public PivotGridOptionsDataField(PivotGridData data, EventHandler optionsChanged, IViewBagOwner owner, string objectPath)
			: base(optionsChanged, owner, objectPath) {
			this.data = data;
			this.area = PivotDataArea.None;
			this.areaIndex = -1;
			this.rowHeaderWidth = PivotGridFieldBase.DefaultWidth;
			this.caption = string.Empty;
			this.fieldNaming = DataFieldNaming.FieldName;
			this.columnValueLineCount = 1;
			this.rowValueLineCount = 1;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldArea"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.Area"),
		DefaultValue(PivotDataArea.None), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotDataArea Area {
			get { return area; }
			set {
				if(value == Area) return;
				CheckArea(value, AreaIndex);
				this.area = value;
				Data.CorrectCalculations();
				OnChanged(true);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldAreaIndex"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.AreaIndex"),
		DefaultValue(-1), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public int AreaIndex {
			get { return areaIndex; }
			set {
				if(value < -1) value = -1;
				if(value == AreaIndex) return;
				CheckArea(Area, value);				
				AreaIndexOldCore = AreaIndex;
				AreaIndexCore = value;
				OnChanged(true);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotArea DataFieldArea {
			get { return GetArea(Area); }
			set { Area = GetArea(value); }
		}
		PivotArea GetArea(PivotDataArea area) {
			return area == PivotDataArea.RowArea ? PivotArea.RowArea : PivotArea.ColumnArea;
		}
		PivotDataArea GetArea(PivotArea area) {
			switch(area) {
				case PivotArea.RowArea:
					return PivotDataArea.RowArea;
				case PivotArea.ColumnArea:
					return PivotDataArea.ColumnArea;
				default:
					throw new ArgumentException(WrongDataFieldLocationText);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotDataArea DataFieldsLocationArea {
			get {
				if(Data == null || Data.DataFieldCount < 2) return PivotDataArea.None;
				return Area == PivotDataArea.RowArea ? Area : PivotDataArea.ColumnArea;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int DataFieldAreaIndex {
			get {
				if(Data == null || !Data.GetIsDataFieldsVisible(DataFieldArea == PivotArea.ColumnArea))
					return -1;
				int index = AreaIndex,
					fieldCount = Data.GetFieldCountByArea(DataFieldArea, false);
				if(index > fieldCount || index < 0)
					index = fieldCount;
				return index;
			}
			set {
				if(Data == null) return;
				if(value > Data.GetFieldCountByArea(DataFieldArea, false)) value = -1;
				AreaIndex = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool DataFieldVisible {
			get {
				if(Data == null || Data.Fields == null) return false;
				if(!Data.GetIsDataFieldsVisible(DataFieldArea == PivotArea.ColumnArea)) return false;
				return Area != PivotDataArea.None;
			}
			set {
				if(DataFieldVisible == value) return;
				if(value) {
					if(Area == PivotDataArea.None) {
						Area = PivotDataArea.ColumnArea;
					}
				} else Area = PivotDataArea.None;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldRowHeaderWidth"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.RowHeaderWidth"),
		DefaultValue(PivotGridFieldBase.DefaultWidth), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public int RowHeaderWidth {
			get { return rowHeaderWidth; }
			set {
				if(value < PivotGridFieldBase.DefaultMinWidth)
					value = PivotGridFieldBase.DefaultMinWidth;
				if(value == RowHeaderWidth) return;
				rowHeaderWidth = value;
				OnRowHeaderWidthChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldCaption"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.Caption"),
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), Localizable(true)
		]
		public string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(value == Caption) return;
				caption = value;
				OnChanged(false);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldFieldNaming"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataField.FieldNaming"),
		DefaultValue(DataFieldNaming.FieldName), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public DataFieldNaming FieldNaming {
			get { return fieldNaming; }
			set {
				if(value == fieldNaming) return;
				fieldNaming = value;
				OnChanged(false);
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		DefaultValue(false), XtraSerializableProperty()
		]
		public bool EnableFilteringByData {
			get { return enableFilteringByData; }
			set { enableFilteringByData = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeEnableFilteringByData() {
			return ShouldSerializeEnableFilteringByData();
		}
		bool ShouldSerializeEnableFilteringByData() { return EnableFilteringByData; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldColumnValueLineCount"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataFieldEx.ColumnValueLineCount"),
		DefaultValue(1), XtraSerializableProperty(),
		NotifyParentProperty(true)
		]
		public int ColumnValueLineCount {
			get { return columnValueLineCount; }
			set {
				columnValueLineCount = value;
				OnChanged(false);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsDataFieldRowValueLineCount"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsDataFieldEx.RowValueLineCount"),
		DefaultValue(1), XtraSerializableProperty(),
		NotifyParentProperty(true)
		]
		public int RowValueLineCount {
			get { return rowValueLineCount; }
			set {
				rowValueLineCount = value;
				OnChanged(false);
			}
		}
		internal int AreaIndexCore {
			get { return areaIndex; }
			set { areaIndex = value; }
		}
		protected int AreaIndexOldCore { get { return Data.DataField.AreaIndexOldCore; } set { Data.DataField.AreaIndexOldCore = value; } }
		protected PivotGridData Data { get { return data; } }
		protected virtual void OnRowHeaderWidthChanged() {
			base.OnChanged(new BaseOptionChangedEventArgs());
			Data.OnFieldSizeChanged(Data.DataField, true, false);
		}
		protected virtual void OnChanged(bool dataChanged) {
			base.OnChanged(new BaseOptionChangedEventArgs());
			if(Data.DataFieldCount > 1) {
				if(dataChanged)
					Data.DoRefresh();
				else
					Data.LayoutChanged();
			}
		}
		internal void CheckArea() {
			CheckArea(Area, AreaIndex);
		}
		void CheckArea(PivotDataArea area, int areaIndex) {
			if(!Data.IsLoading && !Data.IsLockUpdate && !Data.DataField.CanChangeLocationTo(GetArea(area), areaIndex))
				throw new ArgumentException(WrongDataFieldLocationText);
		}
	}
}
namespace DevExpress.XtraPivotGrid.Data {
	public class OptionsDataFieldEventArgs : EventArgs {
		bool dataChanged;
		public OptionsDataFieldEventArgs(bool dataChanged) {
			this.dataChanged = dataChanged;
		}
		public bool DataChanged { get { return dataChanged; } }
	}
}
