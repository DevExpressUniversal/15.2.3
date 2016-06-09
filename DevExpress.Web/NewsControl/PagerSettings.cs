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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public class NewsControlPagerSettings : PagerSettingsEx, IDataViewEndlessPagingSettigns {
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlPagerSettingsSEOFriendly"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public override SEOFriendlyMode SEOFriendly
		{
			get { return base.SEOFriendly; }
			set { base.SEOFriendly = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlPagerSettingsShowDefaultImages"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public override bool ShowDefaultImages
		{
			get { return GetBoolProperty("ShowDefaultImages", false); }
			set
			{
				SetBoolProperty("ShowDefaultImages", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlPagerSettingsPosition"),
#endif
		DefaultValue(PagerPosition.Bottom), NotifyParentProperty(true)]
		public override PagerPosition Position
		{
			get { return (PagerPosition)GetEnumProperty("Position", PagerPosition.Bottom); }
			set
			{
				SetEnumProperty("Position", PagerPosition.Bottom, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsControlPagerSettingsEndlessPagingMode"),
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
	DevExpressWebLocalizedDescription("NewsControlPagerSettingsShowMoreItemsText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string ShowMoreItemsText
		{
			get { return GetStringProperty("ShowMoreItemsText", ""); }
			set { SetStringProperty("ShowMoreItemsText", "", value); }
		}
		public NewsControlPagerSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var settings = source as NewsControlPagerSettings;
			if(settings != null) {
				EndlessPagingMode = settings.EndlessPagingMode;
				ShowMoreItemsText = settings.ShowMoreItemsText;
			}
		}
		protected override AllButtonProperties CreateAllButtonProperties(IPropertiesOwner owner) {
			return new NewsControlPagerAllButtonProperties(owner);
		}
		protected override NextButtonProperties CreateNextButtonProperties(IPropertiesOwner owner) {
			return new NewsControlPagerNextButtonProperties(owner);
		}
		protected override PrevButtonProperties CreatePrevButtonProperties(IPropertiesOwner owner) {
			return new NewsControlPagerPrevButtonProperties(owner);
		}
		protected override FirstButtonProperties CreateFirstButtonProperties(IPropertiesOwner owner) {
			return new NewsControlPagerFirstButtonProperties(owner);
		}
		protected override LastButtonProperties CreateLastButtonProperties(IPropertiesOwner owner) {
			return new NewsControlPagerLastButtonProperties(owner);
		}
		protected override SummaryProperties CreateSummaryProperties(IPropertiesOwner owner) {
			return new NewsControlPagerSummaryProperties(owner);
		}
		protected override PageSizeItemSettings CreatePageSizeItemSettings(IPropertiesOwner owner) {
			return new NewsControlPagerPageSizeItemSettings(owner);
		}
	}
	public class NewsControlPagerAllButtonProperties : AllButtonProperties {
		public NewsControlPagerAllButtonProperties(IPropertiesOwner owner)
			: base(owner, false, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_All)) {
		}
	}
	public class NewsControlPagerNextButtonProperties: NextButtonProperties {
		public NewsControlPagerNextButtonProperties(IPropertiesOwner owner)
			: base(owner, false, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_Next)) {
		}
	}
	public class NewsControlPagerPrevButtonProperties: PrevButtonProperties {
		public NewsControlPagerPrevButtonProperties(IPropertiesOwner owner)
			: base(owner, false, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_Prev)) {
		}
	}
	public class NewsControlPagerFirstButtonProperties: FirstButtonProperties {
		public NewsControlPagerFirstButtonProperties(IPropertiesOwner owner)
			: base(owner, false, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_First)) {
		}
	}
	public class NewsControlPagerLastButtonProperties: LastButtonProperties {
		public NewsControlPagerLastButtonProperties(IPropertiesOwner owner)
			: base(owner, false, ASPxperienceLocalizer.GetString(ASPxperienceStringId.Pager_Last)) {
		}
	}
	public class NewsControlPagerSummaryProperties: SummaryProperties {
		public NewsControlPagerSummaryProperties(IPropertiesOwner owner)
			: base(owner, PagerButtonPosition.Left, 
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.NewsControl_Page),
				ASPxperienceLocalizer.GetString(ASPxperienceStringId.NewsControl_Page)) {
		}
	}
	public class NewsControlPagerPageSizeItemSettings : PageSizeItemSettings {
		public NewsControlPagerPageSizeItemSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		public override string Text {
			get { return GetStringProperty("Text", GetDefaultText()); }
			set {
				SetStringProperty("Text", GetDefaultText(), value);
				Changed();
			}
		}
		protected override string[] GetDefaultPageSizeItems() {
			return new string[] { "3", "5", "10", "20" };
		}
		protected internal override string GetDefaultText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.DataView_PagerRowPerPage);
		}
	}
}
