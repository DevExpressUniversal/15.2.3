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
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid {
	[ContentProperty("Format")]
	public abstract class FormatConditionBase : DependencyObject, ISupportManager {
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FormatConditionCollection Owner {
			get { return (FormatConditionCollection)GetValue(OwnerProperty); }
			internal set { SetValue(OwnerPropertyKey, value); }
		}
		static readonly DependencyPropertyKey OwnerPropertyKey = DependencyProperty.RegisterReadOnly("Owner", typeof(FormatConditionCollection), typeof(FormatConditionBase), new PropertyMetadata(null));
		public static readonly DependencyProperty OwnerProperty = OwnerPropertyKey.DependencyProperty;
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public string MeasureName {
			get { return (string)GetValue(MeasureNameProperty); }
			set { SetValue(MeasureNameProperty, value); }
		}
		public static readonly DependencyProperty MeasureNameProperty = DependencyProperty.Register("MeasureName", typeof(string), typeof(FormatConditionBase), new PropertyMetadata(null, (d, e) => ((FormatConditionBase)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public string Expression {
			get { return (string)GetValue(ExpressionProperty); }
			set { SetValue(ExpressionProperty, value); }
		}
		public static readonly DependencyProperty ExpressionProperty = DependencyProperty.Register("Expression", typeof(string), typeof(FormatConditionBase), new PropertyMetadata(null, (d, e) => ((FormatConditionBase)d).OnInfoPropertyChanged(e)));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public string PredefinedFormatName {
			get { return (string)GetValue(PredefinedFormatNameProperty); }
			set { SetValue(PredefinedFormatNameProperty, value); }
		}
		public static readonly DependencyProperty PredefinedFormatNameProperty =
			DependencyProperty.Register("PredefinedFormatName", typeof(string), typeof(FormatConditionBase), new PropertyMetadata(null, (d, e) => ((FormatConditionBase)d).OnFormatNameChanged()));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public bool ApplyToSpecificLevel {
			get { return (bool)GetValue(ApplyToSpecificLevelProperty); }
			set { SetValue(ApplyToSpecificLevelProperty, value); }
		}
		public static readonly DependencyProperty ApplyToSpecificLevelProperty =
			DependencyProperty.Register("ApplyToSpecificLevel", typeof(bool), typeof(FormatConditionBase), new PropertyMetadata(false, (d, e) => ((FormatConditionBase)d).OnChanged(e)));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public string RowName {
			get { return (string)GetValue(RowNameProperty); }
			set { SetValue(RowNameProperty, value); }
		}
		public static readonly DependencyProperty RowNameProperty =
			DependencyProperty.Register("RowName", typeof(string), typeof(FormatConditionBase), new UIPropertyMetadata(null, (d,e) => {}, CoerceRowColumnName));
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public string ColumnName {
			get { return (string)GetValue(ColumnNameProperty); }
			set { SetValue(ColumnNameProperty, value); }
		}
		public static readonly DependencyProperty ColumnNameProperty =
			DependencyProperty.Register("ColumnName", typeof(string), typeof(FormatConditionBase), new UIPropertyMetadata(null, (d,e) => {}, CoerceRowColumnName));
		static object CoerceRowColumnName(DependencyObject d, object baseValue) {
			if((baseValue as string) == DataControlDialogContext.AnyFieldNameFieldName) {
				((FormatConditionBase)d).ApplyToSpecificLevel = false;
				return null;
			}
			if((baseValue as string) == DataControlDialogContext.GrandTotalNameFieldName)
				return null;
			return baseValue;
		}
		protected void OnInfoPropertyChanged(DependencyPropertyChangedEventArgs e) {
			SyncProperty(e.Property);
			OnChanged(e);
		}
		protected virtual void SyncProperty(DependencyProperty property) {
			SyncIfNeeded(property, FormatConditionBase.MeasureNameProperty, () => Info.FieldName = MeasureName);
			SyncIfNeeded(property, FormatConditionBase.ExpressionProperty, () => Info.Expression = Expression);
		}
		void SyncProperties() {
			SyncProperty(null);
		}
		protected void SyncIfNeeded(DependencyProperty sourceProperty, DependencyProperty targetProperty, Action syncAction) {
			if(sourceProperty == targetProperty || sourceProperty == null)
				syncAction();
		}
		const string predefinedFormatsOwnerPath = "Owner.Owner.";
		public abstract DependencyProperty FormatPropertyForBinding { get; }
		internal abstract IEnumerable<AggregationItemValueStorage> GetSummaries();
		void OnFormatNameChanged() {
			Info.OnFormatNameChanged(this, PredefinedFormatName, predefinedFormatsOwnerPath, FormatPropertyForBinding);
		}
		protected static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var condition = (FormatConditionBase)d;
			condition.Info.FormatCore = e.NewValue as Freezable;
			condition.OnChanged(e, FormatConditionBaseInfo.GetChangeType(e));
		}
		protected internal static object OnCoerceFreezable(DependencyObject d, object baseValue) {
			if(d is FormatConditionBase && ((FormatConditionBase)d).serializationLocker.IsLocked)
				return baseValue;
			return FormatConditionBaseInfo.OnCoerceFreezable(baseValue);
		}
		protected void OnChanged(DependencyPropertyChangedEventArgs e, FormatConditionChangeType changeType = FormatConditionChangeType.All) {
			Owner.Do(x => x.OnItemPropertyChanged(this, e, changeType));
		}
		internal protected FormatConditionBaseInfo Info { get; private set; }
		protected abstract FormatConditionBaseInfo CreateInfo();
		public FormatConditionBase() {
			Info = CreateInfo();
			SyncProperties();
		}
		internal bool IsValid { get { return GetValue(FormatPropertyForBinding) != null && CanAttach; } }
		protected virtual bool CanAttach { get { return GetSummaries().Count() == 0 || ApplyToSpecificLevel; } }
		public virtual string GetApplyToFieldName() {
			return MeasureName;
		}
		internal IEnumerable<IColumnInfo> GetUnboundColumnInfo() {
			return Info.GetUnboundColumnInfo();
		}
		internal IEnumerable<ConditionalFormatSummaryInfo> CreateSummaryItems() {
			return Info.CreateSummaryItems();
		}
		BaseEditUnit ISupportManager.CreateEditUnit() {
			BaseEditUnit unit = CreateEmptyEditUnit();
			UpdateEditUnit(unit);
			return unit;
		}
		protected abstract BaseEditUnit CreateEmptyEditUnit();
		protected virtual void UpdateEditUnit(BaseEditUnit unit) {
			unit.Expression = Expression;
			unit.FieldName = MeasureName;
			unit.PredefinedFormatName = PredefinedFormatName;
			unit.RowName = !ApplyToSpecificLevel ? DataControlDialogContext.AnyFieldNameFieldName : string.IsNullOrEmpty(RowName) ? DataControlDialogContext.GrandTotalNameFieldName : RowName;
			unit.ColumnName = !ApplyToSpecificLevel ? DataControlDialogContext.AnyFieldNameFieldName : string.IsNullOrEmpty(ColumnName) ? DataControlDialogContext.GrandTotalNameFieldName : ColumnName;
			ConditionEditUnit conditionEditUnit = unit as ConditionEditUnit;
			if(conditionEditUnit != null)
				conditionEditUnit.ApplyToRow = ApplyToSpecificLevel;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public virtual string TypeName { get { return GetType().Name; } set { } }
		Locker serializationLocker = new Locker();
		internal void OnDeserializeStart() {
			serializationLocker.Lock();
		}
		internal void OnDeserializeEnd() {
			serializationLocker.Unlock();
			CoerceValue(FormatPropertyForBinding);
		}
		bool XtraShouldSerializeFormat() {
			return string.IsNullOrEmpty(PredefinedFormatName);
		}
	}
}
