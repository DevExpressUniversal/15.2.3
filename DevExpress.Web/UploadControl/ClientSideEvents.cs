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
using System.Collections.Generic;
using DevExpress.Web;
namespace DevExpress.Web {
	public class UploadControlClientSideEvents : ClientSideEvents {
		public UploadControlClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsFileUploadComplete"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileUploadComplete {
			get { return GetEventHandler("FileUploadComplete"); }
			set { SetEventHandler("FileUploadComplete", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsFilesUploadComplete"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FilesUploadComplete {
			get { return GetEventHandler("FilesUploadComplete"); }
			set { SetEventHandler("FilesUploadComplete", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsFileUploadStart"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Obsolete("This property is now obsolete. Use the FilesUploadStart property instead."),
		Browsable(false), 
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileUploadStart {
			get { return GetEventHandler("FileUploadStart"); }
			set { SetEventHandler("FileUploadStart", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsFilesUploadStart"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FilesUploadStart {
			get { return GetEventHandler("FilesUploadStart"); }
			set { SetEventHandler("FilesUploadStart", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsUploadingProgressChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string UploadingProgressChanged {
			get { return GetEventHandler("UploadingProgressChanged"); }
			set { SetEventHandler("UploadingProgressChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsTextChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string TextChanged {
			get { return GetEventHandler("TextChanged"); }
			set { SetEventHandler("TextChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsFileInputCountChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FileInputCountChanged {
			get { return GetEventHandler("FileInputCountChanged"); }
			set { SetEventHandler("FileInputCountChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsDropZoneEnter"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropZoneEnter {
			get { return GetEventHandler("DropZoneEnter"); }
			set { SetEventHandler("DropZoneEnter", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlClientSideEventsDropZoneLeave"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropZoneLeave {
			get { return GetEventHandler("DropZoneLeave"); }
			set { SetEventHandler("DropZoneLeave", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("FileUploadComplete");
			names.Add("FilesUploadComplete");
			names.Add("FileUploadStart");
			names.Add("FilesUploadStart");
			names.Add("UploadingProgressChanged");
			names.Add("TextChanged");
			names.Add("FileInputCountChanged");
			names.Add("DropZoneEnter");
			names.Add("DropZoneLeave");
		}
	}
}
