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
using System.Linq;
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Helpers;
using DevExpress.LookAndFeel;
using DevExpress.XtraExport.Helpers;
using DevExpress.Export.Xl; 
namespace DevExpress.XtraEditors {
	public class FormatConditionRuleIconSet : FormatConditionRuleBase, IFormatRuleDraw, IFormatRuleContextImage, IFormatConditionRuleIconSet {
		FormatConditionIconSet iconSet;
		[DefaultValue(null)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content,true)]
		[Editor(typeof(DevExpress.XtraEditors.Design.FormatRuleIconSetUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public FormatConditionIconSet IconSet {
			get { return iconSet; }
			set {
				if(value is FormatConditionIconSetPredefined) value = value.Clone();
				if(IconSet == value) return;
				if(value != null && value.Owner != null) {
					throw new ArgumentException("Can't change owner of IconSet", "IconSet");
				}
				if(IconSet != null) IconSet.SetOwner(null);
				iconSet = value;
				if(iconSet != null) iconSet.SetOwner(this);
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		object XtraCreateIconSet(XtraItemEventArgs e) {
			if(IconSet == null) {
				return new FormatConditionIconSet();
			}
			IconSet.Icons.Clear();
			return IconSet;
		}
		protected override bool IsFitCore(IFormatConditionRuleValueProvider valueProvider) {
			return GetIcon(valueProvider) != null;
		}
		public override bool IsValid {
			get {
				return IconSet != null && IconSet.HasIcons;
			}
		}
		protected override void AssignCore(FormatConditionRuleBase rule) {
			base.AssignCore(rule);
			var source = rule as FormatConditionRuleIconSet;
			if(source == null) return;
			AssignIconSet(source);
		}
		protected virtual void AssignIconSet(FormatConditionRuleIconSet source) {
			if(source.IconSet == null) {
				IconSet = null;
				return;
			}
			IconSet = source.IconSet.Clone();
		}
		public override FormatConditionRuleBase CreateInstance() {
			return new FormatConditionRuleIconSet();
		}
		protected virtual FormatConditionIconSetIcon GetIcon(IFormatConditionRuleValueProvider valueProvider) {
			decimal? value = CheckQueryNumericValue(valueProvider);
			if(value == null) return null;
			decimal? min = null, max = null;
			if(IconSet.HasPercents()) {
				min = ConvertToNumeric(valueProvider.GetQueryValue(this, FormatRuleValueQueryKind.Minimum));
				max = ConvertToNumeric(valueProvider.GetQueryValue(this, FormatRuleValueQueryKind.Maximum));
			}
			if(IconSet.ValueType == FormatConditionValueType.Percent && (min == null || max == null)) return null;
			var state = valueProvider.GetState(this);
			var icons = state.GetValue < IList<FormatConditionIconSetIcon>>(SortedIconSet);
			foreach(var icon in icons) {
				decimal number = icon.Value;
				if(IconSet.ValueType == FormatConditionValueType.Percent) {
					number = GetPercentValue(min.Value, max.Value, number);
				}
				int res = decimal.Compare(value.Value, number);
				if(icon.ValueComparison == FormatConditionComparisonType.Greater) {
					if(res > 0) return icon;
				}
				if(icon.ValueComparison == FormatConditionComparisonType.GreaterOrEqual) {
					if(res >= 0) return icon;
				}
			}
			return null;
		}
		protected override FormatConditionRuleState GetQueryKindStateCore() {
			if(IconSet == null) return new FormatConditionRuleState(this);
			FormatRuleValueQueryKind kind = FormatRuleValueQueryKind.None;
			if(IconSet.HasPercents()) kind = FormatRuleValueQueryKind.Minimum | FormatRuleValueQueryKind.Maximum;
			FormatConditionRuleState res = new FormatConditionRuleState(this, kind);
			res.SetValue(SortedIconSet, IconSet.SortIcons());
			return res;
		}
		internal const string SortedIconSet = "SortedIconSet";
		#region IFormatRuleDraw Members
		void IFormatRuleDraw.DrawOverlay(FormatRuleDrawArgs e) {
			var icon = GetIcon(e.ValueProvider);
			if(icon == null || icon.GetIcon() == null) return;
			var image = icon.GetIcon();
			e.Cache.Paint.DrawImage(e.Cache.Graphics, image, e.Bounds.Location);
		}
		bool IFormatRuleDraw.AllowDrawValue { get { return true; } }
		#endregion
		#region IFormatRuleContextImage Members
		Image IFormatRuleContextImage.GetContextImage(FormatRuleDrawArgs e) {
			var icon = GetIcon(e.ValueProvider);
			if(icon == null || icon.GetIcon() == null) return null;
			return icon.GetIcon();
		}
		#endregion
		protected internal override void ResetVisualCache() {
			base.ResetVisualCache();
			if(IconSet != null) IconSet.ResetVisualCache();
		}
		protected override void DrawPreviewCore(Utils.Drawing.GraphicsCache cache, FormatConditionDrawPreviewArgs e) {
			if(IconSet == null || !IconSet.HasIcons) return;
			var icons = IconSet.SortIcons();
			int iconWidth = e.Bounds.Width / icons.Count;
			e.Appearance.FillRectangle(cache, e.Bounds);
			Rectangle bounds = Rectangle.Inflate(e.Bounds, 0, -2);
			if(iconWidth - 3 > icons[0].GetIcon().Width) iconWidth = icons[0].GetIcon().Width + 3;
			bounds.Width = iconWidth;
			for(int n = 0; n < icons.Count; n++) {
				Rectangle icon = Rectangle.Inflate(bounds, -1, 0);
				var image = icons[n].GetIcon();
				if(image.Width < icon.Width && image.Height < icon.Height) {
					icon.Size = image.Size;
					icon.Y += (bounds.Height - icon.Height) / 2;
				}
				cache.Paint.DrawImage(e.Graphics, image, icon);
				bounds.X += bounds.Width;
			}
		}
		#region IFormatConditionRuleIconSet
		XlCondFmtIconSetType IFormatConditionRuleIconSet.IconSetType {
			get { return ExportHelper.GetIconSetType(IconSet.Name); }
		}	
		bool IFormatConditionRuleIconSet.Percent {
			get { return IconSet.ValueType == FormatConditionValueType.Percent; }
		}	   
		bool IFormatConditionRuleIconSet.Reverse {
			get { return false; }
		}	   
		bool IFormatConditionRuleIconSet.ShowValues {
			get { return true; }
		}	
		IList<XlCondFmtValueObject> IFormatConditionRuleIconSet.Values {
			get {
				var values = new List<XlCondFmtValueObject>();
				foreach(var icon in IconSet.Icons.Reverse()) {
					XlCondFmtValueObjectType ot = IconSet.ValueType == FormatConditionValueType.Percent ? XlCondFmtValueObjectType.Percent : XlCondFmtValueObjectType.Number;
					XlCondFmtValueObject xcfvo = (icon.Value == Decimal.MinValue) ? new XlCondFmtValueObject() { Value = 0, ObjectType = ot } : new XlCondFmtValueObject() { ObjectType = ot, Value = (double)icon.Value };
					values.Add(xcfvo);
				}
				return values;
			}
		}
		#endregion
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
	public class FormatConditionIconSetIconCollection : DXCollectionBase<FormatConditionIconSetIcon> { 
		protected FormatConditionIconSet owner;
		public FormatConditionIconSetIconCollection(FormatConditionIconSet owner) {
			this.owner = owner;
		}
		bool raiseClearModified = false;
		protected override bool OnClear() {
			this.raiseClearModified = Count > 0;
			foreach(var icon in this) icon.Owner = null;
			base.OnClear();
			return true;
		}
		public override string ToString() {
			if(Count == 0) return "";
			return string.Format("(count={0})", Count);
		}
		public FormatConditionIconSetIcon this[int index] {
			get {
				return GetItem(index);
			}
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			if(raiseClearModified) OnModified(FormatConditionRuleChangeType.Data);
		}
		protected override bool OnInsert(int index, FormatConditionIconSetIcon value) {
			if(value.Owner != null) throw new ArgumentException("Can't use Icons from other collections", "Insert");
			value.Owner = owner;
			return base.OnInsert(index, value);
		}
		protected override void SetItem(int index, FormatConditionIconSetIcon value) {
			if(value.Owner != null) throw new ArgumentException("Can't use Icons from other collections", "SetItem");
			base.SetItem(index, value);
		}
		protected override void OnInsertComplete(int index, FormatConditionIconSetIcon value) {
			base.OnInsertComplete(index, value);
			OnModified(FormatConditionRuleChangeType.Data);
		}
		protected override void OnSetComplete(int index, FormatConditionIconSetIcon oldValue, FormatConditionIconSetIcon newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnModified(FormatConditionRuleChangeType.Data);
		}
		protected override void OnRemoveComplete(int index, FormatConditionIconSetIcon value) {
			base.OnRemoveComplete(index, value);
			value.Owner = null;
			OnModified(FormatConditionRuleChangeType.Data);
		}
		internal void OnModified(FormatConditionRuleChangeType changeType) {
			if(readOnly) throw new ReadOnlyException("FormatConditionIconSetIconCollection");
			if(owner != null) owner.OnModified(changeType);
		}
		bool readOnly = false;
		internal void MakeReadOnly() {
			this.readOnly = true;
		}
	}
	public class FormatConditionIconSetPredefined : FormatConditionIconSet {
		bool finalized = false;
		bool initialized = false;
		protected FormatConditionIconSetPredefined(bool autoPopulate, string name) {
			this.Name = name;
			if(autoPopulate) Populate();
		}
		protected internal override FormatConditionIconSet Clone() {
			return base.Clone();
		}
		protected void Populate() {
			if(initialized) return;
			initialized = true;
			PopulateCore();
			FinalizeIconSet();
		}
		protected virtual void PopulateCore() {
		}
		protected void FinalizeIconSet() {
			Icons.MakeReadOnly();
			this.finalized = true;
		}
		protected bool Finalized { get { return finalized; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override FormatConditionIconSetIconCollection Icons { get { return base.Icons; } }
		public override FormatConditionValueType ValueType {
			get { return base.ValueType; }
			set {
				if(Finalized) ThrowPredefinedIconSetException();
				base.ValueType = value;
			}
		}
		public override string Name {
			get {
				return base.Name;
			}
			set {
				if(Finalized) ThrowPredefinedIconSetException();
				base.Name = value;
			}
		}
		public override string CategoryName {
			get {
				return base.CategoryName;
			}
			set {
				if(Finalized) ThrowPredefinedIconSetException();
				base.CategoryName = value;
			}
		}
		internal void ThrowPredefinedIconSetException() {
			throw new ReadOnlyException("FormatConditionIconSetPredefined");
		}
	}
	[TypeConverter(typeof(UniversalTypeConverterEx))]
	public class FormatConditionIconSet : DevExpress.Utils.Serializing.Helpers.IXtraSupportDeserializeCollectionItem {
		string name = "", categoryName = "", title = "", rangeDescription = "";
		FormatConditionValueType valueType = FormatConditionValueType.Automatic;
		FormatConditionIconSetIconCollection icons;
		FormatConditionRuleIconSet owner;
		UserLookAndFeel lookAndFeel;
		public FormatConditionIconSet() {
			this.icons = new FormatConditionIconSetIconCollection(this);
		}
		internal FormatConditionIconSet(FormatConditionIconSetIcon[] icons) : this() {
			Icons.AddRange(icons);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public UserLookAndFeel LookAndFeel {
			get {
				if(lookAndFeel != null) return lookAndFeel; 
				return Owner == null ? null : Owner.LookAndFeel; 
			}
			set { lookAndFeel = value; }
		}
		public override string ToString() {
			if(string.IsNullOrEmpty(CategoryName)) return Name;
			return string.Format("{0}: {1}", CategoryName, Name);
		}
		internal FormatConditionRuleIconSet Owner { get { return owner; } }
		internal void SetOwner(FormatConditionRuleIconSet owner) {
			this.owner = owner;
		}
		internal void XtraClearIcons(XtraItemEventArgs e) { Icons.Clear(); }
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, 99, XtraSerializationFlags.DefaultValue),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual FormatConditionIconSetIconCollection Icons { get { return icons; } }
		[Browsable(false)]
		public virtual bool HasIcons { get { return Icons != null && Icons.Count > 0; } }
		[XtraSerializableProperty, DefaultValue("")]
		public virtual string Name { 
			get { return name; }
			set {
				if(value == null) value = "";
				if(Name == value) return;
				name = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string Title {
			get { return title; }
			set {
				title = value;
			}
		}
		[XtraSerializableProperty, DefaultValue("")]
		public virtual string CategoryName {
			get { return categoryName; }
			set {
				if(value == null) value = "";
				if(CategoryName == value) return;
				categoryName = value;
			}
		}
		[DefaultValue(FormatConditionValueType.Automatic)]
		[XtraSerializableProperty]
		public virtual FormatConditionValueType ValueType { 
			get { return valueType; }
			set {
				if(ValueType == value) return;
				valueType = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string RangeDescription {
			get { return rangeDescription; }
			set {
				rangeDescription = value;
			}
		}
		protected internal List<FormatConditionIconSetIcon> SortIcons() {
			List<FormatConditionIconSetIcon> res = new List<FormatConditionIconSetIcon>(Icons);
			res.Sort((a, b) => {
				int c = decimal.Compare(a.Value, b.Value);
				if(c != 0) return c * -1;
				if(a.ValueComparison == b.ValueComparison) return 0;
				if(a.ValueComparison == FormatConditionComparisonType.Greater) return -1;
				return 1;
			});
			return res;
		}
		protected internal bool HasPercents() {
			if(Icons == null || Icons.Count == 0) return false;
			if(ValueType == FormatConditionValueType.Percent) return true;
			return false;
		}
		protected internal virtual FormatConditionIconSet Clone() {
			FormatConditionIconSet res = new FormatConditionIconSet();
			res.ValueType = ValueType;
			res.Title = Title;
			res.Name = Name;
			res.CategoryName = CategoryName;
			res.Icons.Clear();
			foreach(var icon in Icons) {
				res.Icons.Add(icon.Clone());
			}
			return res;
		}
		protected internal virtual void OnModified(FormatConditionRuleChangeType changeType) {
			if(owner != null) owner.OnModified(changeType);
		}
		#region IXtraSupportDeserializeCollectionItem Members
		object Utils.Serializing.Helpers.IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return new FormatConditionIconSetIcon();
		}   
		void Utils.Serializing.Helpers.IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			Icons.Add(e.Item.Value as FormatConditionIconSetIcon);
		}
		#endregion
		internal FormatConditionValueType GetValueType() { return ValueType; }
		internal void ResetVisualCache() {
			foreach(var icon in Icons) icon.ResetVisualCache();
		}
	}
	[TypeConverter(typeof(UniversalTypeConverterEx)), Editor(typeof(DevExpress.XtraEditors.Design.FormatPredefinedIconSetIconUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
	public class FormatConditionIconSetIcon : ICaptionSupport {
		FormatConditionIconSet owner;
		Image icon;
		Image internalIcon;
		decimal value;
		FormatConditionComparisonType valueComparison = FormatConditionComparisonType.Greater;
		string predefinedName = "";
		[DefaultValue(null)]
		[XtraSerializableProperty]
		public Image Icon {
			get { return icon; }
			set {
				if(Icon == value) return;
				icon = value;
				this.internalIcon = null;
				OnModified(FormatConditionRuleChangeType.UI);
			}
		}
		public Image GetIcon() {
			if(!string.IsNullOrEmpty(PredefinedName)) {
				if(internalIcon == null) 
					internalIcon = DevExpress.XtraEditors.Helpers.IconSetImageLoader.GetDefault(Owner == null ? null : Owner.LookAndFeel).GetImage(PredefinedName);
				return internalIcon;
			}
			return Icon;
		}
		void ResetValue() { Value = 0; }
		bool ShouldSerializeValue() { return Value != 0; }
		[XtraSerializableProperty]
		public decimal Value {
			get { return value; }
			set {
				if(Value == value) return;
				this.value = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		[DefaultValue(FormatConditionComparisonType.Greater)]
		[XtraSerializableProperty]
		public FormatConditionComparisonType ValueComparison {
			get { return valueComparison; }
			set {
				if(ValueComparison == value) return;
				valueComparison = value;
				OnModified(FormatConditionRuleChangeType.Data);
			}
		}
		[DefaultValue("")]
		[XtraSerializableProperty]
		[Editor(typeof(DevExpress.XtraEditors.Design.FormatPredefinedIconUITypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
		public string PredefinedName {
			get { return predefinedName; }
			set {
				if(value == null) value = "";
				if(PredefinedName == value) return;
				predefinedName = value;
				OnNameChanged();
			}
		}
		void OnNameChanged() {
			this.internalIcon = null;
			OnModified(FormatConditionRuleChangeType.UI);
		}
		public override string ToString() {
			return string.Format("{0} {1} {2}{3}", PredefinedName, ValueComparison == FormatConditionComparisonType.Greater ? ">" : ">=", ValueToString(Value),
				(Owner != null && Owner.GetValueType() == FormatConditionValueType.Percent ? " %" : ""));
		}
		string ValueToString(decimal value) {
			if(value == decimal.MinValue) return "-Minimum";
			if(value == decimal.MaxValue) return "Maximum";
			return value.ToString();
		}
		protected internal virtual FormatConditionIconSetIcon Clone() {
			FormatConditionIconSetIcon icon = new FormatConditionIconSetIcon();
			icon.Icon = Icon;
			icon.Value = Value;
			icon.ValueComparison = ValueComparison;
			icon.PredefinedName = PredefinedName;
			return icon;
		}
		protected internal virtual void OnModified(FormatConditionRuleChangeType changeType) {
			if(Owner != null) Owner.OnModified(changeType);
		}
		protected internal FormatConditionIconSet Owner { 
			get { return owner; }
			set {
				owner = value;
				this.internalIcon = null;
			}
		}
		string ICaptionSupport.Caption { get { return ""; } }
		internal void ResetVisualCache() { this.internalIcon = null; }
	}
}
