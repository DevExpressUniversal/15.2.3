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
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.Design.ASPxSiteMapDataSourceDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxSiteMapDataSource.bmp"), 
	DisplayName(StringResources.ASPxSiteMapDataSource_DisplayName),
	DescriptionAttribute(StringResources.ASPxSiteMapDataSource_Description),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)
	]
	public class ASPxSiteMapDataSource : SiteMapDataSource {
		private bool fProviderAssignedOnlyOne = false;
		private bool fNeedDataSourceChanged = false;
		private bool fIsLoadedFromFile = false;
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapDataSourceEnableRoles"),
#endif
 DefaultValue(false)]
		public bool EnableRoles {
			get { return ViewStateUtils.GetBoolProperty(ViewState, "EnableRoles", false); }
			set { ViewStateUtils.SetBoolProperty(ViewState, "EnableRoles", false, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxSiteMapDataSourceSiteMapProvider")]
#endif
		public override string SiteMapProvider {
			get { return base.SiteMapProvider; }
			set { base.SiteMapProvider = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapDataSourceShowStartingNode"),
#endif
		Category("Behavior"), DefaultValue(false)]
		public override bool ShowStartingNode {
			get { return base.ShowStartingNode; }
			set { base.ShowStartingNode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapDataSourceStartingNodeUrl"),
#endif
		UrlProperty, Editor("DevExpress.Web.Design.SiteMapNodeUrlEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public override string StartingNodeUrl {
			get { return base.StartingNodeUrl; }
			set { base.StartingNodeUrl = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxSiteMapDataSourceProvider")]
#endif
		new public SiteMapProvider Provider {
			get { return base.Provider; }
			set {
				fNeedDataSourceChanged = false;
				base.Provider = value;
				fNeedDataSourceChanged = true;
				if((fProviderAssignedOnlyOne) || (!fIsLoadedFromFile)) {
					fProviderAssignedOnlyOne = true;
					OnDataSourceChanged(EventArgs.Empty);
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSiteMapDataSourceSiteMapFileName"),
#endif
		 UrlProperty, Editor("DevExpress.Web.Design.SiteMapFileNameEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SiteMapFileName {
			get { return ViewStateUtils.GetStringProperty(ViewState, "SiteMapFileName", ""); }
			set {
				if(UrlUtils.IsAppRelativePath(value) || value == "") {
					if(Path.GetExtension(value) == ".sitemap" || value == "") {
						ViewStateUtils.SetStringProperty(ViewState, "SiteMapFileName", "", value);
						OnDataSourceChanged(EventArgs.Empty);
					}
					else
						throw new ArgumentException("Extension SiteMapFileName must be equal to '.sitemap'.");
				}
				else
					throw new ArgumentException("Property value must be application relative path.");
			}
		}
		public ASPxSiteMapDataSource()
			: base() {
			ShowStartingNode = false;
		}
		public void LoadFromFile(string siteMapFileName) {
			UnboundSiteMapProvider provider = new UnboundSiteMapProvider();
			provider.EnableRoles = EnableRoles;
			provider.LoadFromFile(siteMapFileName);
			fIsLoadedFromFile = true;
			Provider = provider;
			fProviderAssignedOnlyOne = true;
		}
		public override void RenderControl(HtmlTextWriter writer) {
			base.RenderControl(writer);
		}
		protected override void OnDataSourceChanged(EventArgs e) {
			if ((fProviderAssignedOnlyOne) && (fNeedDataSourceChanged) || DesignMode)
				base.OnDataSourceChanged(e);
		}
		protected override HierarchicalDataSourceView GetHierarchicalView(string viewPath) {
			TryToLoadFromSiteMapFile();
			return base.GetHierarchicalView(viewPath);
		}
		public override DataSourceView GetView(string viewName) {
			TryToLoadFromSiteMapFile();
			return base.GetView(viewName);
		}
		protected internal string GetSiteMapFileName() {
			if (SiteMapFileName != "") {
				if (UrlUtils.IsAppRelativePath(SiteMapFileName))
					return SiteMapFileName;
				else					
					return UnboundSiteMapProvider.DefaultSiteMapFileName;
			}
			else
				return UnboundSiteMapProvider.DefaultSiteMapFileName;
		}
		private bool IsCustomProviderAssigned() {
			return !DesignMode && SiteMapProvider == "" && !fProviderAssignedOnlyOne;
		}
		private void TryToLoadFromSiteMapFile() {
			if(IsCustomProviderAssigned())
				LoadFromFile(GetSiteMapFileName());
		}
	}
}
