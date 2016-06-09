#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.Utils.Design;
using DevExpress.Web;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web {
	public class ReportToolbarButton : ReportToolbarItem, IDXObjectWrapper {
		const string
			EnabledPropertyName = "Enabled",
			ToolTipPropertyName = "ToolTip";
		const bool DefaultEnabled = true;
		ItemImageProperties image;
		public ReportToolbarButton() {
			image = new ItemImageProperties(this);
		}
		public ReportToolbarButton(ReportToolbarItemKind itemKind)
			: this(itemKind, true) {
		}
		public ReportToolbarButton(ReportToolbarItemKind itemKind, bool enabled)
			: this() {
			ItemKind = itemKind;
			Enabled = enabled;
		}
		#region properties
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarButtonText")]
#endif
		[Localizable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string Text {
			get {
				return GetStringProperty("Text", "");
			}
			set {
				SetStringProperty("Text", "", value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarButtonEnabled")]
#endif
		[NotifyParentProperty(true)]
		[DefaultValue(DefaultEnabled)]
		[SRCategory(ReportStringId.CatBehavior)]
		public bool Enabled {
			get { return GetBoolProperty(EnabledPropertyName, DefaultEnabled); }
			set { SetBoolProperty(EnabledPropertyName, DefaultEnabled, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarButtonItemKind")]
#endif
		[Editor("DevExpress.XtraReports.Web.Design.ButtonItemKindEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		[RefreshProperties(RefreshProperties.All)]
		public new ReportToolbarItemKind ItemKind {
			get { return base.ItemKind; }
			set { base.ItemKind = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarButtonToolTip")]
#endif
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatBehavior)]
		[Localizable(true)]
		[NotifyParentProperty(true)]
		public string ToolTip {
			get { return GetStringProperty(ToolTipPropertyName, ""); }
			set { SetStringProperty(ToolTipPropertyName, "", value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarButtonImageUrl")]
#endif
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatAppearance)]
		[Localizable(true)]
		[NotifyParentProperty(true)]
		[Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		[UrlProperty]
		[AutoFormatUrlProperty]
		public string ImageUrl {
			get { return image.Url; }
			set { image.Url = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportToolbarButtonImageUrlDisabled")]
#endif
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatAppearance)]
		[Localizable(true)]
		[NotifyParentProperty(true)]
		[Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		[UrlProperty]
		[AutoFormatUrlProperty]
		public string ImageUrlDisabled {
			get { return image.UrlDisabled; }
			set { image.UrlDisabled = value; }
		}
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[Localizable(true)]
		[TypeConverter("DevExpress.Web.Design.IconIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[AutoFormatDisable]
		[EditorAttribute("DevExpress.Web.Design.IconUITypeEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string IconID {
			get { return image.IconID; }
			set { image.IconID = value; }
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var button = source as ReportToolbarButton;
			if(button != null) {
				Text = button.Text;
				Enabled = button.Enabled;
				ToolTip = button.ToolTip;
				image.Assign(button.image);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { image };
		}
		internal ImageProperties GetImageProperties(Page page, ReportToolbarImages images) {
			string imageResourceName = ToolbarWebImageResource.GetImageResourceName(ItemKind);
			ImageProperties image = string.IsNullOrEmpty(imageResourceName)
				? new ImageProperties()
				: images.GetImageProperties(page, imageResourceName);
			image.CopyFrom(this.image);
			return image;
		}
		protected internal override void ValidateProperties() {
			if((ToolbarWebImageResource.GetImageResourceName(ItemKind) + ".gif") == ImageUrl)
				ImageUrl = string.Empty;
		}
		object IDXObjectWrapper.SourceObject {
			get { return image; }
		}
	}
}
