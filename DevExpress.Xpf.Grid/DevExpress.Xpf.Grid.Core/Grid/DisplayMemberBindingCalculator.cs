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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using System.Reflection;
namespace DevExpress.Xpf.Grid {
	public interface IDispalyMemberBindingClient {
		void UpdateColumns();
		void UpdateSimpleBinding();
	}
	public class DisplayMemberBindingCalculator {
		interface ICalculatorStrategy {
			object GetValue(int rowHandle, int listSourceRowIndex);
			void SetValue(int rowHandle, object value);
			Type GetColumnType();
			bool IsSimpleMode { get; }
		}
		class DefaultStrategy : ICalculatorStrategy {
			internal RowData RowData { get; private set; }
			DataViewBase GridView { get; set; }
			ColumnBase Column { get; set; }
			public DefaultStrategy(DisplayMemberBindingCalculator owner) {
				this.Column = owner.Column;
				this.GridView = owner.GridView;
				RowData = new StandaloneRowData(owner.GridView.VisualDataTreeBuilder);
			}
			public object GetValue(int rowHandle, int listSourceRowIndex) {
				RowData.conditionalFormattingLocker.DoIfNotLocked(() => RowData.AssignFromCore(rowHandle, listSourceRowIndex, Column));
				GridCellData gridCellData = (GridCellData)RowData.GetCellDataByColumn(Column);
				object value = gridCellData.DisplayMemberBindingValue;
				ClearRowData();
				return value;
			}
			public void SetValue(int rowHandle, object value) {
				RowData data = GetRowDataByRowHandle(rowHandle);
				data.AssignFrom(rowHandle);
				GridCellData gridColumnData = (GridCellData)data.GetCellDataByColumn(Column);
				gridColumnData.DisplayMemberBindingValue = value;
				ClearRowData();
			}
			public Type GetColumnType() {
				RowData.AssignFrom(0);
				GridColumnData gridColumnData = RowData.GetCellDataByColumn(Column);
				object value = gridColumnData.Value;
				ClearRowData();
				if(value == null)
					return null;
				return value.GetType();
			}
			RowData GetRowDataByRowHandle(int rowHandle) {
				return GridView.GetRowData(rowHandle) ?? RowData;
			}
			void ClearRowData() {
				if(RowData.RowHandle != null && RowData.RowHandle.Value != DataControlBase.InvalidRowHandle)
					RowData.AssignFrom(DataControlBase.InvalidRowHandle);
			}
			public bool IsSimpleMode { get { return false; } }
		}
		class SimpleBindingStrategy : ICalculatorStrategy {
			ISimpleBindingProcessor SimpleBindingProcessor { get; set; }
			DataControlBase DataControl { get; set; }
			public SimpleBindingStrategy(DisplayMemberBindingCalculator owner) {
				this.SimpleBindingProcessor = owner.Column.SimpleBindingProcessor;
				this.DataControl = owner.GridView.DataControl;
			}
			public object GetValue(int rowHandle, int listSourceRowIndex) {
				object row = null;
				if(rowHandle != DataControlBase.InvalidRowHandle)
					row = GetRowByRowHandle(rowHandle);
				else if(listSourceRowIndex != DataControlBase.InvalidRowHandle)
					row = DataControl.DataProviderBase.GetRowByListIndex(listSourceRowIndex);
				return SimpleBindingProcessor.GetValue(row);
			}
			public void SetValue(int rowHandle, object value) {
				SimpleBindingProcessor.SetValue(GetRowByRowHandle(rowHandle), value);
			}
			public Type GetColumnType() {
				object value = GetValue(0, 0);
				if(value != null)
					return value.GetType();
				return null;
			}
			object GetRowByRowHandle(int rowHandle) {
				return DataControl.GetRow(rowHandle);
			}
			public bool IsSimpleMode { get { return true; } }
		}
		ICalculatorStrategy strategyCore;
		ICalculatorStrategy Strategy {
			get {
				ValidateStrategy();
				return strategyCore;
			}
		}
		void ValidateStrategy() {
			if(strategyCore == null || (strategyCore.IsSimpleMode != Column.IsSimpleBindingEnabled)) {
				if(Column.IsSimpleBindingEnabled)
					strategyCore = new SimpleBindingStrategy(this);
				else
					strategyCore = new DefaultStrategy(this);
			}
		}
		internal static string GetBindingNameCore(BindingBase binding) {
			if(binding == null)
				return null;
			Binding castBinding = binding as Binding;
			if(castBinding != null && castBinding.Path != null)
				return castBinding.Path.Path;
			MultiBinding castmBinding = binding as MultiBinding;
			if(castmBinding != null && castmBinding.Bindings.Count > 0)
				return GetBindingNameCore(castmBinding.Bindings[0]);
			return null;
		}
		public static string GetBindingName(BindingBase binding) {
			if(binding == null)
				return null;
			string bindingName = GetBindingNameCore(binding);
			if(bindingName == null)
				return binding.GetHashCode().ToString();
			return bindingName;
		}
		public static void ValidateBinding(BindingBase displayMemberBinding) {
			if(displayMemberBinding == null) return;
			foreach(Binding binding in GetBindings(displayMemberBinding)) {
				binding.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
				if(binding.Converter == null)
					binding.Converter = DisplayMemberBindingConverter.Instance;
				if(ShouldBindToEntireRow(binding)) {
					binding.Path = new PropertyPath(RowPath);
					continue;
				}
				PropertyPath originalPath = binding.Path;
				string path = originalPath.Path;
				if(ShouldPatchPath(path)) {
					binding.Path = new PropertyPath(RowPathWithDot + path, originalPath.PathParameters.ToArray());
				}
			}
		}
		const string RowPath = "RowData.Row";
		internal const string RowPathWithDot = RowPath + ".";
		internal const string DataPath = "Data.";
		static bool ShouldBindToEntireRow(Binding binding) {
			return binding.Path == null || String.IsNullOrEmpty(binding.Path.Path) || binding.Path.Path == ".";
		}
		static bool ShouldPatchPath(string path) {
			return !ContainsRowData(path) && !StartsWithData(path) && !StartsWithView(path);
		}
		static bool ContainsRowData(string path) {
			return path.Contains(RowPath);
		}
		static bool StartsWithData(string path) {
			return path.StartsWith(DataPath);
		}
		static bool StartsWithView(string path) {
			return path.StartsWith("View.");
		}
		static IEnumerable<Binding> GetBindings(BindingBase binding) {
			List<Binding> bindings = new List<Binding>();
			if(binding is Binding)
				bindings.Add((Binding)binding);
			else if(binding is MultiBinding)
				bindings = ((MultiBinding)binding).Bindings.Cast<Binding>().ToList();
			else if(binding is PriorityBinding)
				bindings = ((PriorityBinding)binding).Bindings.Cast<Binding>().ToList();
			return bindings;
		}
		public DataViewBase GridView { get; private set; }
		ColumnBase Column { get; set; }
#if DEBUGTEST
		public RowData DebugTestRowData { get { return (strategyCore as DefaultStrategy).Return(x => x.RowData, () => null); } }
#endif
		public DisplayMemberBindingCalculator(DataViewBase gridView, ColumnBase column) {
			GridView = gridView;
			Column = column;
		}
		public object GetValue(int rowHandle) {
			return GetValue(rowHandle, DataControlBase.InvalidRowIndex);
		}
		public object GetValue(int rowHandle, int listSourceRowIndex) {
			if(GridView.IsDesignTime)
				return null;
			bool isSimpleMode = Strategy.IsSimpleMode;
			object value = Strategy.GetValue(rowHandle, listSourceRowIndex);
			if(Strategy.IsSimpleMode != isSimpleMode)
				value = Strategy.GetValue(rowHandle, listSourceRowIndex);
			return value;
		}
		public void SetValue(int rowHandle, object value) {
			bool isSimpleMode = Strategy.IsSimpleMode;
			Strategy.SetValue(rowHandle, value);
			if(Strategy.IsSimpleMode != isSimpleMode)
				Strategy.SetValue(rowHandle, value);
		}
		public Type GetColumnType() {
			return Strategy.GetColumnType();
		}
		public UnboundColumnType GetUnboundColumnType() {
			if(GridView.DataProviderBase.DataRowCount == 0)
				return UnboundColumnType.Object;
			Type columnType = GetColumnType();
			if(columnType != null && columnType.IsEnum) 
				return UnboundColumnType.Object;
			TypeCode typeCode = Type.GetTypeCode(columnType);
			switch(typeCode) {
				case TypeCode.Boolean: return UnboundColumnType.Boolean;
				case TypeCode.Char:
				case TypeCode.String: return UnboundColumnType.String;
				case TypeCode.DateTime: return UnboundColumnType.DateTime;
				case TypeCode.Double:
				case TypeCode.Single:
				case TypeCode.Decimal: return UnboundColumnType.Decimal;
				case TypeCode.DBNull:
				case TypeCode.Empty:
				case TypeCode.Object: return UnboundColumnType.Object;
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64: return UnboundColumnType.Integer;
			}
			return UnboundColumnType.Object;
		}
	}
	class DisplayMemberBindingConverter : IValueConverter {
		public static DisplayMemberBindingConverter Instance = new DisplayMemberBindingConverter();
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return value;
		}
	}
	interface ISimpleBindingProcessor {
		object GetValue(object row);
		void SetValue(object row, object newValue);
		PropertyDescriptor DescriptorToListen { get; }
	}
	internal class SimpleBindingProcessor : ISimpleBindingProcessor {
		IValueConverter converter;
		object converterParameter;
		CultureInfo userCultureInfo;
		CultureInfo cultureInfoCore;
		CultureInfo CultureInfo {
			get {
				if(cultureInfoCore == null)
					cultureInfoCore = userCultureInfo ?? ((XmlLanguage)OwnerControl.GetValue(FrameworkElement.LanguageProperty)).GetSpecificCulture();
				return cultureInfoCore;
			}
		}
		static Func<Binding, int> GetDelay = CreateGetDelayFunction();
		static Func<Binding, int> CreateGetDelayFunction() {
			PropertyInfo propertyInfo = typeof(Binding).GetProperty("Delay");
			if(propertyInfo != null)
				return (Func<Binding, int>)Delegate.CreateDelegate(typeof(Func<Binding, int>), propertyInfo.GetGetMethod());
			else
				return (Binding b) => 0;
		}
		string pathCore;
		ColumnBase column;
		DataColumnInfo valueColumnInfo;
		DataControlBase OwnerControl { get { return column.OwnerControl; } }
		public PropertyDescriptor DescriptorToListen { get; private set; }
		SimpleBindingState ValidStates { get; set; }
		public bool IsEnabled { get { return ValidStates.HasFlag(SimpleBindingState.All); } }
		public SimpleBindingProcessor(ColumnBase column) {
			this.column = column;
		}
		public void Validate(SimpleBindingState changedState) {
			ValidateIfNeeded(changedState, SimpleBindingState.Bidning, ValidateBinding);
			ValidateIfNeeded(changedState, SimpleBindingState.Data, ValidateData);
			ValidateIfNeeded(changedState, SimpleBindingState.Field, ValidateField);
		}
		void ValidateBinding() {
			Binding binding = column.Binding as Binding;
			pathCore = (binding != null && binding.GetType().Equals(typeof(Binding)) && ValidateBindingSettings(binding)) ? GetPath(binding) : null;
			ChangeState(SimpleBindingState.Bidning, pathCore != null);
			ValidateData();
		}
		void ValidateData() {
			valueColumnInfo = GetValueColumnInfo();
			DescriptorToListen = null;
			if(valueColumnInfo != null) {
				PropertyDescriptor descriptor = valueColumnInfo.PropertyDescriptor;
				Type componentType = descriptor.ComponentType;
				if(!typeof(INotifyPropertyChanged).IsAssignableFrom(componentType))
					DescriptorToListen = TypeDescriptor.GetProperties(componentType)[descriptor.Name];
				ChangeState(SimpleBindingState.Data, true);
			} else
				ChangeState(SimpleBindingState.Data, false);
			ValidateField();
		}
		void ValidateField() {
			var info = GetInfoByFieldName(column.FieldName);
			ChangeState(SimpleBindingState.Field, info == null || info.Unbound);
		}
		void ChangeState(SimpleBindingState state, bool validValue) {
			if(validValue)
				ValidStates |= state;
			else
				ValidStates &= ~state;
		}
		static void ValidateIfNeeded(SimpleBindingState change, SimpleBindingState flag, Action validateAction) {
			if(change.HasFlag(flag))
				validateAction();
		}
		void Disable() {
			ChangeState(SimpleBindingState.All, false);
		}
		public object GetValue(object row) {
			if(row != null && CheckDataAccess(row)) {
				object value = valueColumnInfo.PropertyDescriptor.GetValue(row);
				if(converter != null)
					value = converter.Convert(value, typeof(object), converterParameter, CultureInfo);
				value = CoerceValue(value, typeof(object));
				if(value == DependencyProperty.UnsetValue)
					Disable();
				return value;
			}
			return null;
		}
		public void SetValue(object row, object newValue) {
			if(row != null && CheckDataAccess(row)) {
				PropertyDescriptor descriptor = valueColumnInfo.PropertyDescriptor;
				object value = newValue;
				if(converter != null)
					value = converter.ConvertBack(value, descriptor.PropertyType, converterParameter, CultureInfo);
				value = CoerceValue(value, descriptor.PropertyType);
				if(value == DependencyProperty.UnsetValue)
					Disable();
				else
					descriptor.SetValue(row, value);
			}
		}
		object CoerceValue(object value, Type targetType) {
			if(value == null)
				return NullValueForType(targetType);
			if(!targetType.IsAssignableFrom(value.GetType()) || Convert.IsDBNull(value) || value == Binding.DoNothing)
				return DependencyProperty.UnsetValue;
			return value;
		}
		static object NullValueForType(Type type) {
			if(type == null)
				return null;
			if(!type.IsValueType)
				return null;
			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				return null;
			return DependencyProperty.UnsetValue;
		}
		bool CheckDataAccess(object row) {
			if(valueColumnInfo.PropertyDescriptor.ComponentType.IsAssignableFrom(row.GetType()))
				return true;
			Disable();
			return false;
		}
		public void ResetLanguage() {
			cultureInfoCore = null;
		}
		bool ValidateBindingSettings(Binding binding) {
			converter = binding.Converter == DisplayMemberBindingConverter.Instance ? null : binding.Converter;
			converterParameter = binding.ConverterParameter;
			userCultureInfo = binding.ConverterCulture;
			ResetLanguage();
			return ValidateBindingProperties(binding);
		}
		protected internal static bool ValidateBindingProperties(Binding binding) {
			Func<Binding, bool>[] propertyCheckers = { b => b.FallbackValue == DependencyProperty.UnsetValue,
															 b => String.IsNullOrEmpty(b.BindingGroupName),
															 b => b.TargetNullValue == DependencyProperty.UnsetValue,
															 b => b.StringFormat == null,
															 b => !b.IsAsync,
															 b => b.AsyncState == null,
															 b => b.XPath == null,
															 b => !b.NotifyOnSourceUpdated,
															 b => !b.NotifyOnTargetUpdated,
															 b => !b.ValidatesOnDataErrors,
															 b => GetDelay(b) == 0
														 };
			foreach(var checker in propertyCheckers)
				if(!checker(binding))
					return false;
			return true;
		}
		static string GetPath(Binding binding) {
			PropertyPath propertyPath = binding.Path;
			if(propertyPath == null)
				return null;
			string path = propertyPath.Path;
			if(string.IsNullOrEmpty(path))
				return null;
			return ExtractField(path);
		}
		static string ExtractField(string path) {
			string[] pathHeaders = new string[] { DisplayMemberBindingCalculator.RowPathWithDot, DisplayMemberBindingCalculator.DataPath };
			string field = path;
			for(int i = 0; i < pathHeaders.Length; i++) {
				if(path.StartsWith(pathHeaders[i])) {
					string[] splitedPath = path.Split(new string[] { pathHeaders[i] }, StringSplitOptions.RemoveEmptyEntries);
					if(splitedPath.Length == 1)
						field = splitedPath[0];
					break;
				}
			}
			return IsFieldValid(field) ? field : null;
		}
		protected internal static bool IsFieldValid(string field) {
			char[] fieldChars = field.ToCharArray();
			if(fieldChars.Length == 0)
				return false;
			if(!Char.IsLetter(fieldChars[0]))
				return false;
			if(fieldChars.Length > 1) {
				for(int i = 1; i < fieldChars.Length; i++) {
					if(!Char.IsLetterOrDigit(fieldChars[i]))
						return false;
				}
			}
			return true;
		}
		DataColumnInfo GetValueColumnInfo() {
			var info = GetInfoByFieldName(pathCore);
			if(info == null)
				return null;
			Type componentType = info.PropertyDescriptor.ComponentType;
			if(typeof(System.Data.DataRowView).IsAssignableFrom(componentType))
				return info;
			Type[] unsupportedTypes = { typeof(DependencyObject), typeof(System.Dynamic.IDynamicMetaObjectProvider), typeof(System.ComponentModel.ICustomTypeDescriptor) };
			foreach(Type unsupportedType in unsupportedTypes) {
				if(unsupportedType.IsAssignableFrom(componentType))
					return null;
			}
			return info;
		}
		DataColumnInfo GetInfoByFieldName(string fieldName) {
			if(OwnerControl == null)
				return null;
			return OwnerControl.DataProviderBase.GetActualColumnInfo(fieldName);
		}
	}
	[Flags]
	enum SimpleBindingState {
		None = 0,
		Field = 1,
		Data = 1 << 1,
		Bidning = 1 << 2,
		All = Field | Data | Bidning
	}
}
