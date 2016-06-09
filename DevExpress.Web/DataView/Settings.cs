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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public enum DataViewEndlessPagingMode { Disabled, OnClick, OnScroll };
	public class DataViewPagerSettings : PagerSettingsEx, IDataViewEndlessPagingSettigns {
		protected internal ASPxDataViewBase DataView {
			get {
				if(Owner is ASPxDataViewBase)
					return Owner as ASPxDataViewBase;
				else if(Owner is DVPager)
					return (Owner as DVPager).DataView;
				return null;
			}
		}
		public DataViewPagerSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewPagerSettingsShowNumericButtons"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public override bool ShowNumericButtons {
			get { return GetBoolProperty("ShowNumericButtons", false); }
			set {
				SetBoolProperty("ShowNumericButtons", false, value);
				Changed();
			}
		}
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable,
		DefaultValue(SEOFriendlyMode.Disabled), NotifyParentProperty(true)]
		public override SEOFriendlyMode SEOFriendly {
			get { return base.SEOFriendly; }
			set { base.SEOFriendly = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewPagerSettingsEndlessPagingMode"),
#endif
		AutoFormatDisable, DefaultValue(DataViewEndlessPagingMode.Disabled), NotifyParentProperty(true)]
		public DataViewEndlessPagingMode EndlessPagingMode
		{
			get { return (DataViewEndlessPagingMode)GetEnumProperty("EndlessPagingMode", DataViewEndlessPagingMode.Disabled); }
			set
			{
				if (EndlessPagingMode != value)
				{
					SetEnumProperty("EndlessPagingMode", DataViewEndlessPagingMode.Disabled, value);
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewPagerSettingsShowMoreItemsText"),
#endif
		AutoFormatDisable, DefaultValue(""), Localizable(true), NotifyParentProperty(true)]
		public string ShowMoreItemsText
		{
			get { return GetStringProperty("ShowMoreItemsText", ""); }
			set { SetStringProperty("ShowMoreItemsText", "", value); }
		}
		protected override FirstButtonProperties CreateFirstButtonProperties(IPropertiesOwner owner) {
			return new DataViewPagerFirstButtonProperties(owner);
		}
		protected override LastButtonProperties CreateLastButtonProperties(IPropertiesOwner owner) {
			return new DataViewPagerLastButtonProperties(owner);
		}
		protected override SummaryProperties CreateSummaryProperties(IPropertiesOwner owner) {
			return new DataViewPagerSummaryProperties(owner);
		}
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new DataViewPagerPageSizeItemSettings(owner);
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as DataViewPagerSettings;
			if(settings != null) {
				EndlessPagingMode = settings.EndlessPagingMode;
				ShowMoreItemsText = settings.ShowMoreItemsText;
			}
		}
	}
	public class DataViewTableLayoutSettings : PropertiesBase {
		public DataViewTableLayoutSettings()
			: base() {
		}
		public DataViewTableLayoutSettings(ASPxDataViewBase owner)
			: base(owner) {
		}
		protected ASPxDataViewBase DataView {
			get { return (ASPxDataViewBase)Owner; }
		}
		protected virtual int DefaultColumnCount {
			get { return 3; }
		}
		protected virtual int DefaultRowPerPage {
			get { return 3; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewTableLayoutSettingsColumnCount"),
#endif
		DefaultValue(3), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual int ColumnCount {
			get { return GetIntProperty("ColumnCount", DefaultColumnCount); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "ColumnCount");
				SetIntProperty("ColumnCount", DefaultColumnCount, value);
				Changed();
				if(DataView != null)
					DataView.DataLayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewTableLayoutSettingsRowsPerPage"),
#endif
		DefaultValue(3), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual int RowsPerPage {
			get { return GetIntProperty("RowsPerPage", DefaultRowPerPage); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "RowsPerPage");
				SetIntProperty("RowsPerPage", DefaultRowPerPage, value);
				Changed();
				if(DataView != null)
					DataView.DataLayoutChanged();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as DataViewTableLayoutSettings;
			if(settings != null) {
				ColumnCount = settings.ColumnCount;
				RowsPerPage = settings.RowsPerPage;
			}
		}
	}
	public class DataViewFlowLayoutSettings : PropertiesBase {
		public DataViewFlowLayoutSettings()
			: base() {
		}
		public DataViewFlowLayoutSettings(ASPxDataViewBase owner)
			: base(owner) {
		}
		protected ASPxDataViewBase DataView {
			get { return (ASPxDataViewBase)Owner; }
		}
		protected virtual int DefaultItemsPerPage {
			get { return 9; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DataViewFlowLayoutSettingsItemsPerPage"),
#endif
		DefaultValue(9), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual int ItemsPerPage {
			get { return GetIntProperty("ItemsPerPage", DefaultItemsPerPage); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "ItemsPerPage");
				SetIntProperty("ItemsPerPage", DefaultItemsPerPage, value);
				Changed();
				if(DataView != null)
					DataView.DataLayoutChanged();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as DataViewFlowLayoutSettings;
			if(settings != null)
				ItemsPerPage = settings.ItemsPerPage;
		}
	}
}
namespace DevExpress.Web.Internal {
	public class DataViewPagerFirstButtonProperties : FirstButtonProperties {
		public DataViewPagerFirstButtonProperties(IPropertiesOwner owner)
			: base(owner, true) {
		}
	}
	public class DataViewPagerLastButtonProperties : LastButtonProperties {
		public DataViewPagerLastButtonProperties(IPropertiesOwner owner)
			: base(owner, true) {
		}
	}
	public class DataViewPagerSummaryProperties : SummaryProperties {
		public DataViewPagerSummaryProperties(IPropertiesOwner owner)
			: base(owner, PagerButtonPosition.Inside, 
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerSummaryFormat),
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerSummaryAllPages)) {
		}
	}
	public class DataViewPagerPageSizeItemSettings : PageSizeItemSettings {
		protected internal new DataViewPagerSettings PagerSettings {
			get { return Owner as DataViewPagerSettings; }
		}
		public DataViewPagerPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override string[] GetDefaultPageSizeItems() {
			return IsDataViewTableLayout() ? new string[] { "3", "5", "10", "20" } : new string[] { "9", "15", "30", "60" };
		}
		protected internal override string GetDefaultCaption() {
			if(IsDataViewTableLayout())
				return ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerRowPerPage);
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerPageSize);
		}
		protected internal virtual bool IsDataViewTableLayout() {
			if(PagerSettings.DataView != null)
				return PagerSettings.DataView.LayoutInternal == Layout.Table;
			return false;
		}
	}
}
namespace DevExpress.Web.Internal {
	public interface IDataViewEndlessPagingSettigns {
		DataViewEndlessPagingMode EndlessPagingMode { get; set; }
		string ShowMoreItemsText { get; set; }
	}
}
