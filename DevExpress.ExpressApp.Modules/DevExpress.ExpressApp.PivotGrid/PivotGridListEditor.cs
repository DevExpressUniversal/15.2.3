#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.ExpressApp.PivotGrid {
	internal static class NumericTypes {
		private static readonly List<Type> numericTypes = new List<Type>();
		static NumericTypes() {
			numericTypes.AddRange(new Type[] {
				typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
				typeof(ulong), typeof(char), typeof(float), typeof(double), typeof(decimal)});
		}
		public static bool IsNumeric(Type type, bool checkNullableUnderlyingType) {
			bool result = numericTypes.Contains(type);
			if(!result && checkNullableUnderlyingType) {
				Type nullableUnderlyingType = Nullable.GetUnderlyingType(type);
				if(nullableUnderlyingType != null) {
					result = numericTypes.Contains(nullableUnderlyingType);
				}
			}
			return result;
		}
	}
	public interface IDataObjectTypeProvider {
		Type DataObjectType { get; set; }
	}
	public abstract class PivotGridListEditorBase : ListEditor, IComplexListEditor, IControlOrderProvider, ISupportToolTip {
		public const string Alias = "PivotGrid";
		private PivotGridData pivotGridData;
		private string defaultSettings;
		protected object pivotGridControl;
		protected object chartControl;
		protected virtual IPivotSettings PivotSettings {
			get { return ((IModelPivotListView)Model).PivotSettings; }
		}
		protected void InitializeOptions(PivotGridOptionsLayout optionsLayout) {
			optionsLayout.StoreAllOptions = true;
			optionsLayout.StoreAppearance = true;
			optionsLayout.Columns.AddNewColumns = PivotSettings.AddNewColumns;
			optionsLayout.Columns.RemoveOldColumns = PivotSettings.RemoveOldColumns;
			optionsLayout.Columns.StoreAllOptions = true;
			optionsLayout.Columns.StoreAppearance = true;
		}
		protected void ResetSettings() {
			((DevExpress.ExpressApp.Model.Core.ModelNode)PivotSettings).ClearValue("Settings");
			ApplyModel();
		}
		protected void SavePivotSettings() {
			if(Control != null) {
				PivotSettings.Settings = GetPivotGridSettings();
			}
		}
		private bool isLoadSettings = false;
		internal void SafeLoadPivotSettings() {
			try {
				isLoadSettings = true;
				foreach(DevExpress.ExpressApp.Model.Core.ModelNode node in ((DevExpress.ExpressApp.Model.Core.ModelNode)PivotSettings).EnumerateAllLayers()) {
					LoadPivotSettings(node.GetValueInThisLayer<string>("Settings"));
				}
			}
			finally {
				isLoadSettings = false;
			}
		}
		protected abstract void LoadPivotSettings(string settings);
		protected abstract string GetPivotGridSettings();
		protected abstract PivotGridFieldBase CreatePivotGridField(string propertyName, PivotArea pivotArea);
		#region SetupPivotGridField (Aka FieldBuilder)
		private PivotSummaryType GetSummaryType(IMemberInfo memberInfo) {
			PivotSummaryType result = PivotSummaryType.Count;
			if(NumericTypes.IsNumeric(memberInfo.MemberType, true)) {
				result = PivotSummaryType.Sum;
			}
			return result;
		}
		private void SetPivotGridFieldDisplayFormat(PivotGridFieldBase field, string displayFormat) {
			field.CellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			field.CellFormat.FormatString = displayFormat;
			field.ValueFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			field.ValueFormat.FormatString = displayFormat;
			field.GrandTotalCellFormat.FormatType = DevExpress.Utils.FormatType.Custom;
			field.GrandTotalCellFormat.FormatString = displayFormat;
		}
		protected virtual void SetupPivotGridFieldCaption(PivotGridFieldBase field, IMemberInfo memberInfo) {
			if(Model != null && Model.ModelClass.AllMembers[memberInfo.Name] != null) {
				field.Caption = Model.ModelClass.AllMembers[memberInfo.Name].Caption;
			} else {
				field.Caption = CaptionHelper.GetFullMemberCaption(memberInfo.Owner, memberInfo.Name);
			}
		}
		protected virtual void SetupPivotGridField(PivotGridFieldBase field, IModelColumn column) {
			SetupPivotGridField(field, column.ModelMember.MemberInfo);
		}
		protected virtual void SetupPivotGridField(PivotGridFieldBase field, IMemberInfo memberInfo) {
			if(!isLoadSettings) {
				field.Name = "Field_" + field.FieldName;
				field.SummaryType = GetSummaryType(memberInfo);
				field.Options.AllowRunTimeSummaryChange = true;
				if(memberInfo.MemberType == typeof(DateTime)) {
					field.GroupInterval = DevExpress.XtraPivotGrid.PivotGroupInterval.Date;
				}
				IModelColumn modelColumn = Model.Columns[field.FieldName];
				if(modelColumn != null) {
					SetPivotGridFieldDisplayFormat(field, modelColumn.DisplayFormat);
					((ISupportToolTip)this).SetToolTip(field, modelColumn);
				}
				SetupPivotGridFieldCaption(field, memberInfo);
			}
		}
		protected abstract void SetupPivotGridFieldToolTip(PivotGridFieldBase field, IModelToolTip toolTipModel);
		#endregion
		protected void InitializePivotGridData(PivotGridData pivotGridData){
			defaultSettings = GetPivotGridSettings();
			this.pivotGridData = pivotGridData;
			if(pivotGridData is IDataObjectTypeProvider) {
				((IDataObjectTypeProvider)pivotGridData).DataObjectType = Model.ModelClass.TypeInfo.Type;
			}
			if(objectSpace != null) {
				pivotGridData.OptionsData.CustomObjectConverter = new PersistentObjectConverter(objectSpace);
			}
			ApplyModel();
		}
		protected virtual string GetDisplayText(PivotGridFieldBase field, object value) {
			if(field != null && value != null && value.GetType() == typeof(bool)) {
				IModelMember modelMember = Model.ModelClass.FindMember(field.FieldName);
				if(modelMember != null) {
					if((bool)value) {
						if(!string.IsNullOrEmpty(modelMember.CaptionForTrue)) {
							return modelMember.CaptionForTrue;
						}
					} else {
						if(!string.IsNullOrEmpty(modelMember.CaptionForFalse)) {
							return modelMember.CaptionForFalse;
						}
					}
				}
			}
			return null;
		}
		protected override void OnControlsCreated() {
			base.OnControlsCreated();
			UpdateControlDataSource();
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			UpdateControlDataSource();
		}
		protected void UpdateControlDataSource() {
			if(TryUpdateControlDataSource()) {
				if(ControlDataSourceChanged != null) {
					ControlDataSourceChanged(this, EventArgs.Empty);
				}
			}
		}
		protected virtual bool TryUpdateControlDataSource() {
			if(pivotGridData != null) {
				pivotGridData.ListSource = DataSource as IList;
				return true;
			}
			return false;
		}
		protected override sealed void AssignDataSourceToControl(object dataSource) {
		}
		public PivotGridListEditorBase(IModelListView model)
			: base(model) {
		}
		public override void Dispose() {
			pivotGridControl = null;
			chartControl = null;
			base.Dispose();
		}
		public override void Refresh() {
			UpdateControlDataSource();
		}
		public override void ApplyModel() {
			base.ApplyModel();
			if(pivotGridData != null) {
				pivotGridData.Fields.Clear();
				if(string.IsNullOrEmpty(PivotSettings.Settings)) {
					LoadPivotSettings(defaultSettings);
					List<PivotGridFieldBase> pivotFields = new List<PivotGridFieldBase>();
					IList<IModelColumn> visibleColumns = Model.Columns.GetVisibleColumns();
					foreach(IModelColumn column in Model.Columns) {
						string displayablePropertyName = GetDisplayablePropertyName(column.PropertyName);
						PivotGridFieldBase field = CreatePivotGridField(displayablePropertyName, PivotArea.RowArea);
						if(column.ModelMember != null) {
							SetupPivotGridField(field, column);
						}
						field.Visible = visibleColumns.Contains(column);
						if(column.GroupIndex >= 0) {
							field.Area = PivotArea.RowArea;
						}
						if(column.Summary.Count > 0) {
							field.Area = PivotArea.DataArea;
							field.SummaryType = (DevExpress.Data.PivotGrid.PivotSummaryType)Enum.Parse(typeof(DevExpress.Data.PivotGrid.PivotSummaryType), column.Summary[0].SummaryType.ToString(), true);
						}
						pivotFields.Add(field);
					}
						foreach(PivotGridFieldBase pivotField in pivotFields) {
							pivotGridData.Fields.Add(pivotField);
						}
				} else {
					SafeLoadPivotSettings();
				}
				if(Model.ModelClass != null) {
					foreach(PivotGridFieldBase field in pivotGridData.Fields) {
						IMemberInfo memberInfo = Model.ModelClass.TypeInfo.FindMember(field.FieldName);
						if(memberInfo != null) {
							SetupPivotGridFieldCaption(field, memberInfo);
							((ISupportToolTip)this).SetToolTip(field, Model.Columns[field.FieldName]);
						}
					}
				}
				pivotGridData.OptionsChartDataSource.DataProvideMode = PivotChartDataProvideMode.UseCustomSettings;
				pivotGridData.OptionsChartDataSource.ProvideDataByColumns = PivotSettings.ChartDataVertical;
				pivotGridData.OptionsChartDataSource.ProvideRowTotals = PivotSettings.ShowRowTotals;
				pivotGridData.OptionsChartDataSource.ProvideRowGrandTotals = PivotSettings.ShowRowGrandTotals;
				pivotGridData.OptionsChartDataSource.ProvideColumnTotals = PivotSettings.ShowColumnTotals;
				pivotGridData.OptionsChartDataSource.ProvideColumnGrandTotals = PivotSettings.ShowColumnGrandTotals;
			}
		}
		public override void SaveModel() {
			base.SaveModel();
			SavePivotSettings();
		}
		public override void BreakLinksToControls() {
			pivotGridControl = null;
			chartControl = null;
			if(pivotGridData != null) {
				pivotGridData = null;
			}
			base.BreakLinksToControls();
		}
		public override IList GetSelectedObjects() {
			return new List<object>();
		}
		public override SelectionType SelectionType {
			get { return SelectionType.None; }
		}
		public override IContextMenuTemplate ContextMenuTemplate {
			get { return null; }
		}
		public object PivotGridControl { get { return pivotGridControl; } }
		public object ChartControl { get { return chartControl; } }
		#region IControlOrderProvider Members
		public int GetIndexByObject(object obj) {
			IList objects = GetOrderedObjects();
			return objects.IndexOf(obj);
		}
		public object GetObjectByIndex(int index) {
			IList objects = GetOrderedObjects();
			if((index >= 0) && (index < objects.Count)) {
				return objects[index];
			}
			return null;
		}
		public IList GetOrderedObjects() {
			if(List is IListServer) {
				return null;
			}
			else {
				return List;
			}
		}
		#endregion
		#region IComplexListEditor Members
		private IObjectSpace objectSpace;
		public void Setup(CollectionSourceBase collectionSource, XafApplication application) {
			objectSpace = collectionSource.ObjectSpace;
		}
		#endregion
#if DebugTest
		public PivotGridFieldBase DebugTest_CreatePivotGridField(string propertyName, PivotArea pivotArea) {
			return CreatePivotGridField(propertyName, pivotArea);
		}
		public void DebugTest_ResetSettings() {
			ResetSettings();
		}
		public PivotGridData DebugTest_PivotGridData {
			get { return pivotGridData; }
		}
#endif
		void ISupportToolTip.SetToolTip(object element, IModelToolTip model) {
			SetupPivotGridFieldToolTip((PivotGridFieldBase)element, model);
		}
		public event EventHandler<EventArgs> ControlDataSourceChanged;
	}
	public class PersistentObjectConverter : ICustomObjectConverter {
		private IObjectSpace objectSpace;
		public PersistentObjectConverter(IObjectSpace objectSpace) {
			Guard.ArgumentNotNull(objectSpace, "objectSpace");
			this.objectSpace = objectSpace;
		}
		public bool CanConvert(Type type) {
			ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(type); 
			if(ti != null) {
				return ti.IsPersistent;
			}
			return false;
		}
		public object FromString(Type type, string str) {
			if(type != null) {
				return objectSpace.GetObjectByHandle(str);
			}
			return null;
		}
		public Type GetType(string typeName) {
			ITypeInfo ti = XafTypesInfo.Instance.FindTypeInfo(typeName); 
			if(ti != null) {
				return ti.Type;
			}
			return null;
		}
		public string ToString(Type type, object obj) {
			return objectSpace.GetObjectHandle(obj);
		}
	}
}
