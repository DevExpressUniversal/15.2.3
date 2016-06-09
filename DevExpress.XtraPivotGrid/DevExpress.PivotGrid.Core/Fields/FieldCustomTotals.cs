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
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.WebUtils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
#if SL
using DevExpress.Xpf.Collections;
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotGridCustomTotalBase : IComponentLoading, IViewBagOwner {
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3;
		PivotSummaryType summaryType;
		[NonSerialized]
		PivotGridCustomTotalCollectionBase collection;
		FormatInfo format;
		FormatInfo cellFormat;
		object tag;
		static FormatInfo countCellFormat;
		static PivotGridCustomTotalBase() {
			countCellFormat = new FormatInfo();
			countCellFormat.FormatType = FormatType.Numeric;
			countCellFormat.FormatString = "{0}";
		}
		public PivotGridCustomTotalBase()
			: this(PivotSummaryType.Sum) {
		}
		public PivotGridCustomTotalBase(PivotSummaryType summaryType) {
			this.summaryType = summaryType;
			this.collection = null;
			this.format = new FormatInfo(this, this, "Format");
			this.format.Changed += new EventHandler(OnFormatChanged);
			this.cellFormat = new FormatInfo(this, this, "CellFormat");
			this.cellFormat.Changed += new EventHandler(OnFormatChanged);
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridCustomTotalBaseSummaryType"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.SummaryType"),
		DefaultValue(PivotSummaryType.Sum), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set {
				if(SummaryType == value) return;
				summaryType = value;
				OnChanged();
			}
		}
		void ResetFormat() { Format.Reset(); }
		bool ShouldSerializeFormat() { return !Format.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridCustomTotalBaseFormat"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.Format"),
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true), NotifyParentProperty(true)
		]
		public FormatInfo Format { get { return format; } }
		bool XtraShouldSerializeFormat() { return ShouldSerializeFormat(); }
		void ResetCellFormat() { CellFormat.Reset(); }
		bool ShouldSerializeCellFormat() { return !CellFormat.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridCustomTotalBaseCellFormat"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.CellFormat"),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true), NotifyParentProperty(true)
		]
		public FormatInfo CellFormat { get { return cellFormat; } }
		bool XtraShouldSerializeCellFormat() { return ShouldSerializeCellFormat(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridCustomTotalBaseTag"),
#endif
 XtraSerializableProperty(), Category("Data"), DefaultValue(null),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotalBase.Tag"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
#if !SL && !DXPORTABLE
		[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(PivotObjectEditorTypeConverter)), NotifyParentProperty(true)
		]
#endif
		public object Tag { get { return tag; } set { tag = value; } }
		public string GetValueText(object value) {
			return GetValueText(false, -1, value);
		}
		internal string GetValueText(bool isColumn, int index, object value) {
			if(!Format.IsEmpty)
				return Format.GetDisplayText(value);
			if(Collection != null && Collection.Field != null)
				value = Collection.Field.GetValueText(isColumn, index, value);
			return FormatProvider.DefaultFormats[(int)SummaryType].GetDisplayText(value);
		}
		public FormatInfo GetCellFormat() {
			if(SummaryType == PivotSummaryType.Count && CellFormat.IsEmpty)
				return countCellFormat;
			return CellFormat;
		}
		protected PivotGridCustomTotalCollectionBase Collection { get { return collection; } }
		protected virtual void OnChanged() {
			if(Collection != null)
				Collection.OnFieldChanged();
		}
		protected internal void SetCollection(PivotGridCustomTotalCollectionBase collection) {
			this.collection = collection;
		}
		void OnFormatChanged(object sender, EventArgs e) {
			OnChanged();
		}
		public override string ToString() {
			return SummaryType.ToString();
		}
		public virtual void CloneTo(PivotGridCustomTotalBase clone) {
			clone.SummaryType = SummaryType;
			clone.Format.Assign(Format);
			clone.CellFormat.Assign(CellFormat);
			clone.Tag = Tag;
		}
		public virtual bool IsEqual(PivotGridCustomTotalBase total) {
			return total.SummaryType == SummaryType &&
				total.Format.IsEquals(Format) &&
				total.CellFormat.IsEquals(CellFormat) &&
				total.SummaryType == SummaryType;
		}
		static class FormatProvider {
			static DevExpress.Utils.Localization.XtraLocalizer<PivotGridStringId> localizer;
			static FormatInfo[] defaultFormats;
			internal static FormatInfo[] DefaultFormats {
				get {
					if(defaultFormats == null || !object.ReferenceEquals(localizer, PivotGridLocalizer.Active)) {
						Initialize();
					}
					return defaultFormats;
				}
			}
			static void Initialize() {
				PivotGridStringId[] ids = new PivotGridStringId[] { 
					PivotGridStringId.TotalFormatCount, PivotGridStringId.TotalFormatSum,
					PivotGridStringId.TotalFormatMin, PivotGridStringId.TotalFormatMax, PivotGridStringId.TotalFormatAverage,
					PivotGridStringId.TotalFormatStdDev, PivotGridStringId.TotalFormatStdDevp,
					PivotGridStringId.TotalFormatVar, PivotGridStringId.TotalFormatVarp, PivotGridStringId.TotalFormatCustom};
				defaultFormats = new FormatInfo[ids.Length];
				for(int i = 0; i < defaultFormats.Length; i++) {
					defaultFormats[i] = new FormatInfo();
					defaultFormats[i].FormatType = FormatType.Numeric;
					defaultFormats[i].FormatString = PivotGridLocalizer.GetString(ids[i]);
				}
				localizer = PivotGridLocalizer.Active;
			}
		}
		bool IComponentLoading.IsLoading {
			get { return collection == null || collection.Field == null || collection.Field.IsLoading; }
		}
		T IViewBagOwner.GetViewBagProperty<T>(string objectPath, string propertyName, T value) {
			return value;
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value) {
		}
	}
	[ListBindable(false)]
	public class PivotGridCustomTotalCollectionBase : CollectionBase {
		PivotGridFieldBase field;
		public PivotGridCustomTotalCollectionBase() { }
		public PivotGridCustomTotalCollectionBase(PivotGridFieldBase field)
			: this() {
			this.field = field;
		}
		public PivotGridCustomTotalCollectionBase(PivotGridCustomTotalBase[] totals)
			: this() {
			AssignArray(totals);
		}
		public void AssignArray(PivotGridCustomTotalBase[] totals) {
			Clear();
			for(int i = 0; i < totals.Length; i++)
				Add(totals[i]);
		}
		public PivotGridCustomTotalBase[] CloneToArray() {
			PivotGridCustomTotalBase[] result = new PivotGridCustomTotalBase[Count];
			for(int i = 0; i < Count; i++) {
				result[i] = CreateCustomTotal();
				this[i].CloneTo(result[i]);
			}
			return result;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridCustomTotalCollectionBaseItem"),
#endif
 NotifyParentProperty(true)]
		public PivotGridCustomTotalBase this[int index] { get { return InnerList[index] as PivotGridCustomTotalBase; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridCustomTotalCollectionBaseField"),
#endif
 NotifyParentProperty(true)]
		public PivotGridFieldBase Field {
			get { return field; }
			set { field = value; }
		}
		public PivotGridCustomTotalBase Add(PivotSummaryType summaryType) {
			PivotGridCustomTotalBase customTotal = CreateCustomTotal();
			customTotal.SummaryType = summaryType;
			AddCore(customTotal);
			return customTotal;
		}
		protected internal virtual PivotGridCustomTotalBase CreateCustomTotal() {
			return new PivotGridCustomTotalBase();
		}
		public void Add(PivotGridCustomTotalBase customTotal) {
			AddCore(customTotal);
		}
		public void Add(PivotGridCustomTotalCollectionBase customTotals) {
			for(int i = 0; i < customTotals.Count; i++) {
				PivotGridCustomTotalBase total = this.Add(customTotals[i].SummaryType);
				total.CellFormat.Assign(customTotals[i].CellFormat);
				total.Format.Assign(customTotals[i].Format);
			}
		}
		public bool Contains(PivotSummaryType summaryType) {
			foreach(PivotGridCustomTotalBase customTotal in List) {
				if(customTotal.SummaryType == summaryType)
					return true;
			}
			return false;
		}
		public int IndexOf(PivotGridCustomTotalBase customTotal) { 
			return List.IndexOf(customTotal); 
		}
		public void Remove(PivotGridCustomTotalBase customTotal) {
			List.Remove(customTotal);
		}
		protected virtual void AddCore(PivotGridCustomTotalBase customTotal) {
			List.Add(customTotal);
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			PivotGridCustomTotalBase customTotal = item as PivotGridCustomTotalBase;
			if(customTotal == null)
				return;
			customTotal.SetCollection(this);
			NotifyField();
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnRemoveComplete(index, item);
			NotifyField();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			NotifyField();
		}
		protected virtual void NotifyField() {
			if(Field != null)
				Field.OnCustomTotalChanged();
		}
		protected internal void OnFieldChanged() {
			NotifyField();
		}
	}
}
