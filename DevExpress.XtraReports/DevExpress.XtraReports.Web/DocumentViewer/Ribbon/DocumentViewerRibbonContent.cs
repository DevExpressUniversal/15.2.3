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
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.XtraReports.Web.DocumentViewer.Ribbon {
	public class DocumentViewerRibbonTabBase : RibbonTab {
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return RibbonCommandService.CommandPrefix + DefaultName;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(DefaultName))
				return string.Format("{0} ({1})", base.ToString(), GetName());
			return base.ToString();
		}
	}
	public class DocumentViewerHomeRibbonTab : DocumentViewerRibbonTabBase {
		public DocumentViewerHomeRibbonTab()
			: this(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonHomeTabText)) {
		}
		public DocumentViewerHomeRibbonTab(string text) {
			Text = text;
		}
		[DefaultValue(XRWebStringResources.DocumentViewer_RibbonHomeTabText)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		protected override string DefaultName {
			get { return XRWebStringResources.DocumentViewer_RibbonHomeTabText; }
		}
	}
	public class DocumentViewerRibbonGroupBase : RibbonGroup {
		protected virtual string DefaultName {
			get { return string.Empty; }
		}
		[Browsable(false)]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		protected override string GetName() {
			return DefaultName;
		}
		protected override ItemImagePropertiesBase GetImage() {
			var baseImages = base.GetImage();
			if(!baseImages.IsEmpty)
				return baseImages;
			var imageProperties = new ItemImageProperties();
			imageProperties.CopyFrom(Image);
			ASPxRibbon ribbon = DocumentViewerRibbonHelper.GetRibbonControl(this);
			if(ribbon != null)
				imageProperties.Url = EmptyImageProperties.GetGlobalEmptyImage(ribbon.Page).Url;
			return imageProperties;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(DefaultName))
				return string.Format("{0} ({1})", base.ToString(), DefaultName);
			return base.ToString();
		}
	}
	public class DocumentViewerPrintRibbonGroup : DocumentViewerRibbonGroupBase {
		public DocumentViewerPrintRibbonGroup()
			: this(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonPrintGroupText)) {
		}
		public DocumentViewerPrintRibbonGroup(string text) {
			Text = text;
		}
		protected override string DefaultName {
			get { return XRWebStringResources.DocumentViewer_RibbonPrintGroupText; }
		}
		[DefaultValue(XRWebStringResources.DocumentViewer_RibbonPrintGroupText)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
	public class DocumentViewerNavigationRibbonGroup : DocumentViewerRibbonGroupBase {
		public DocumentViewerNavigationRibbonGroup()
			: this(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonNavigationGroupText)) {
		}
		public DocumentViewerNavigationRibbonGroup(string text) {
			Text = text;
		}
		protected override string DefaultName {
			get { return XRWebStringResources.DocumentViewer_RibbonNavigationGroupText; }
		}
		[DefaultValue(XRWebStringResources.DocumentViewer_RibbonNavigationGroupText)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
	public class DocumentViewerExportRibbonGroup : DocumentViewerRibbonGroupBase {
		public DocumentViewerExportRibbonGroup()
			: this(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonExportGroupText)) {
		}
		public DocumentViewerExportRibbonGroup(string text) {
			Text = text;
		}
		protected override string DefaultName {
			get { return XRWebStringResources.DocumentViewer_RibbonExportGroupText; }
		}
		[DefaultValue(XRWebStringResources.DocumentViewer_RibbonExportGroupText)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
	public class DocumentViewerReportRibbonGroup : DocumentViewerRibbonGroupBase {
		public DocumentViewerReportRibbonGroup()
			: this(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonReportGroupText)) {
		}
		public DocumentViewerReportRibbonGroup(string text) {
			Text = text;
		}
		protected override string DefaultName {
			get { return XRWebStringResources.DocumentViewer_RibbonReportGroupText; }
		}
		[DefaultValue(XRWebStringResources.DocumentViewer_RibbonReportGroupText)]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
}
