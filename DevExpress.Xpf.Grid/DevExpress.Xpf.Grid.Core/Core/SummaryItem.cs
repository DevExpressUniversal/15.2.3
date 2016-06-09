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
using DevExpress.Data;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Globalization;
using DevExpress.Data.Summary;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public enum GridSummaryItemAlignment { Default, Left, Right };
	public interface IAlignmentItem : ISummaryItem {
		GridSummaryItemAlignment Alignment { get; set; }
	}
	public interface IGroupFooterSummaryItem {
		string ShowInGroupColumnFooter { get; set; }
	}
	public abstract class SummaryItemBase : DependencyObject, DevExpress.Utils.Design.ICaptionSupport, IAlignmentItem {
		public static readonly DependencyProperty TagProperty;
		public static readonly DependencyProperty SummaryTypeProperty;
		public static readonly DependencyProperty FieldNameProperty;
		public static readonly DependencyProperty DisplayFormatProperty;
		public static readonly DependencyProperty ShowInColumnProperty;
		public static readonly DependencyProperty VisibleProperty;
		public static readonly DependencyProperty AlignmentProperty;
		public static readonly DependencyProperty IsLastProperty;
		static SummaryItemBase() {
			Type ownerType = typeof(SummaryItemBase);
			TagProperty = DependencyPropertyManager.Register("Tag", typeof(object), ownerType, new PropertyMetadata(null, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e)));
			SummaryTypeProperty = DependencyPropertyManager.Register("SummaryType", typeof(SummaryItemType), ownerType, new PropertyMetadata(SummaryItemType.None, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e)));
			FieldNameProperty = DependencyPropertyManager.Register("FieldName", typeof(string), ownerType, new PropertyMetadata(string.Empty, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e), (d, baseValue) => baseValue == null ? string.Empty : baseValue));
			DisplayFormatProperty = DependencyPropertyManager.Register("DisplayFormat", typeof(string), ownerType, new PropertyMetadata(string.Empty, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e), (d, baseValue) => baseValue == null ? string.Empty : baseValue));
			ShowInColumnProperty = DependencyPropertyManager.Register("ShowInColumn", typeof(string), ownerType, new PropertyMetadata(string.Empty, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e), (d, baseValue) => baseValue == null ? string.Empty : baseValue));
			VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e)));
			AlignmentProperty = DependencyPropertyManager.Register("Alignment", typeof(GridSummaryItemAlignment), ownerType, new PropertyMetadata(GridSummaryItemAlignment.Default, (d, e) => ((SummaryItemBase)d).OnSummaryChanged(e)));
			IsLastProperty = DependencyPropertyManager.Register("IsLast", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		WeakReference collectionReference;
		internal ISummaryItemOwner Collection { get { return collectionReference == null ? null : (ISummaryItemOwner)collectionReference.Target; } set { collectionReference = new WeakReference(value); } }
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseSummaryType"),
#endif
 XtraSerializableProperty]
		public SummaryItemType SummaryType {
			get { return (SummaryItemType)GetValue(SummaryTypeProperty); }
			set { SetValue(SummaryTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseTag"),
#endif
 TypeConverter(typeof(ObjectConverter)), Category(Categories.Data)]
		public object Tag {
			get { return GetValue(TagProperty); }
			set { SetValue(TagProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseFieldName"),
#endif
 XtraSerializableProperty]
		public string FieldName {
			get { return (string)GetValue(FieldNameProperty); }
			set { SetValue(FieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseDisplayFormat"),
#endif
 XtraSerializableProperty]
		public string DisplayFormat {
			get { return (string)GetValue(DisplayFormatProperty); }
			set { SetValue(DisplayFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseShowInColumn"),
#endif
 XtraSerializableProperty]
		public string ShowInColumn {
			get { return (string)GetValue(ShowInColumnProperty); }
			set { SetValue(ShowInColumnProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseVisible"),
#endif
 XtraSerializableProperty]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridCoreLocalizedDescription("SummaryItemBaseAlignment"),
#endif
 XtraSerializableProperty]
		public GridSummaryItemAlignment Alignment {
			get { return (GridSummaryItemAlignment)GetValue(AlignmentProperty); }
			set { SetValue(AlignmentProperty, value); }
		}
		public bool IsLast {
			get { return (bool)GetValue(IsLastProperty); }
			set { SetValue(IsLastProperty, value); }
		}
		internal virtual string ActualShowInColumn { get { return string.IsNullOrEmpty(ShowInColumn) ? FieldName : ShowInColumn; } }
		protected virtual string GetGroupDisplayFormat(string columnDisplayFormat) {
			return GetFooterDisplayFormat(GetGroupStringIdPrefix(), columnDisplayFormat, true, false);
		}
		protected virtual string GetGroupStringIdPrefix() {
			return "DefaultGroupSummaryFormatString_";
		}
		protected string GetFooterDisplayFormatSameColumn(string columnDisplayFormat, ColumnSummaryType columnSummaryType, bool forseUseColumnDisplayFormat) {
			return GetFooterDisplayFormat(columnSummaryType == ColumnSummaryType.Total ? "DefaultTotalSummaryFormatStringInSameColumn_" : "DefaultGroupColumnSummaryFormatStringInSameColumn_", columnDisplayFormat, false, forseUseColumnDisplayFormat);
		}
		protected internal virtual string GetFooterDisplayFormatSameColumn(string columnDisplayFormat, ColumnSummaryType columnSummaryType) {
			return GetFooterDisplayFormatSameColumn(columnDisplayFormat, columnSummaryType, false);
		}
		protected internal virtual string GetFooterDisplayFormat(string columnDisplayFormat, ColumnSummaryType columnSummaryType) {
			return GetFooterDisplayFormat(GetFooterStringIdPrefix(columnSummaryType), columnDisplayFormat, true, false);
		}
		protected virtual string GetFooterStringIdPrefix(ColumnSummaryType columnSummaryType) {
			return columnSummaryType == ColumnSummaryType.Total ? "DefaultTotalSummaryFormatString_" : "DefaultGroupColumnSummaryFormatString_";
		}
		internal string GetGroupDisplayFormat() {
			return GetFooterDisplayFormat(GetGroupStringIdPrefix(), String.Empty, true, true);
		}
		internal string GetFooterDisplayFormatSameColumn(ColumnSummaryType columnSummaryType) {
			return GetFooterDisplayFormatSameColumn(String.Empty, columnSummaryType, true);
		}
		internal string GetFooterDisplayFormat(ColumnSummaryType columnSummaryType) {
			return GetFooterDisplayFormat(GetFooterStringIdPrefix(columnSummaryType), String.Empty, true, true);
		}
		string GetFooterDisplayFormat(string stringIdPrefix, string columnDisplayFormat, bool columnIsSet, bool forseUseColumnDisplayFormat) {
			string res = FormatStringConverter.GetDisplayFormat(forseUseColumnDisplayFormat ? columnDisplayFormat : DisplayFormat);
			if(res != string.Empty)
				return res;
			if(SummaryType != SummaryItemType.None && SummaryType != SummaryItemType.Custom) {
				string summayType = SummaryType == SummaryItemType.Average ? "Avg" : SummaryType.ToString();
				res = GetFooterDisplayFormatCore(stringIdPrefix + summayType);
				if(columnDisplayFormat != string.Empty && SummaryType != SummaryItemType.Count) {
					if(columnIsSet) {
						return string.Format(res, columnDisplayFormat, "{1}");
					}
					else {
						return string.Format(res, columnDisplayFormat);
					}
				}
				return res;
			}
			return "{0}";
		}
		protected virtual string GetFooterDisplayFormatCore(string stringId) {
			return GridControlLocalizer.GetString(stringId);
		}
		public enum ColumnSummaryType { Total, Group }
		public string GetGroupDisplayText(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat) {
			return GetDisplayTextCore(language, GetGroupDisplayFormat(columnDisplayFormat), value, columnCaption);
		}
		public string GetGroupColumnDisplayText(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat) {
			return GetFooterDisplayTextCore(language, columnCaption, value, columnDisplayFormat, ColumnSummaryType.Group);
		}
		internal string GetFooterDisplayText(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat, bool forceColumnDisplayFormat) {
			return GetFooterDisplayTextCore(language, columnCaption, value, columnDisplayFormat, ColumnSummaryType.Total, forceColumnDisplayFormat);
		}
		public string GetFooterDisplayText(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat) {
			return GetFooterDisplayText(language, columnCaption, value, columnDisplayFormat, false);
		}
		string GetFooterDisplayTextCore(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat, ColumnSummaryType columnSummaryType, bool forceColumnDisplayFormat) {
			if(string.IsNullOrEmpty(ShowInColumn) || FieldName == ShowInColumn)
				return GetDisplayTextCore(language, GetFooterDisplayFormatSameColumn(
					columnDisplayFormat, columnSummaryType, forceColumnDisplayFormat), value, columnCaption);
			return GetFooterDisplayTextWithColumnNameCore(language, columnCaption, value, columnDisplayFormat, columnSummaryType);
		}
		string GetFooterDisplayTextCore(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat, ColumnSummaryType columnSummaryType) {
			return GetFooterDisplayTextCore(language, columnCaption, value, columnDisplayFormat, columnSummaryType, false);
		}
		string GetDisplayTextCore(IFormatProvider language, string format, params object[] args) {
			return DevExpress.Xpf.Core.Native.DisplayFormatHelper.GetDisplayTextFromDisplayFormat(language, format, args);
		}
		internal string GetFooterDisplayTextWithColumnName(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat) {
			return GetFooterDisplayTextWithColumnNameCore(language, columnCaption, value, columnDisplayFormat, ColumnSummaryType.Total);
		}
		string GetFooterDisplayTextWithColumnNameCore(IFormatProvider language, string columnCaption, object value, string columnDisplayFormat, ColumnSummaryType columnSummaryType) {
			return GetDisplayTextCore(language, GetFooterDisplayFormat(columnDisplayFormat, columnSummaryType), value, columnCaption);
		}
		protected void OnSummaryChanged(DependencyPropertyChangedEventArgs e) {
			if(Collection != null)
				Collection.OnSummaryChanged(this, e);
		}
		string DevExpress.Utils.Design.ICaptionSupport.Caption {
			get {
				if(SummaryType == SummaryItemType.None)
					return string.Empty;
				string res = SummaryType.ToString();
				if(!string.IsNullOrEmpty(FieldName))
					res = FieldName + " (" + res + ")";
				return res;
			}
		}
		internal bool EqualsToControllerSummaryItem(SummaryItem controllerSummaryItem) {
			return FieldName == controllerSummaryItem.FieldName 
				&& SummaryType == controllerSummaryItem.SummaryType;
		}
		internal bool IsInDataController(SummaryItemCollection controllerSummaries) {
			return controllerSummaries.GetSummaryItemByTag(this) != null;
		}
		internal virtual bool? IgnoreNullValues { get { return null; } }
	}
}
