#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates {
	public class TemplateContentSettings : PropertiesBase {
		private const string LogonTemplateContentPathName = "LogonTemplateContentPath";
		private const string DefaultTemplateContentPathName = "DefaultTemplateContentPath";
		private const string DefaultVerticalTemplateContentPathName = "DefaultVerticalTemplateContentPath";
		private const string NestedFrameControlPathName = "NestedFrameControlPath";
		private const string DialogTemplateContentPathName = "DialogTemplateContentPath";
		public string LogonTemplateContentPath {
			get {
				return GetStringProperty("LogonTemplateContentPath", "");
			}
			set {
				SetStringProperty("LogonTemplateContentPath", "", value);
			}
		}
		public string DialogTemplateContentPath {
			get {
				return GetStringProperty("DialogTemplateContentPath", "");
			}
			set {
				SetStringProperty("DialogTemplateContentPath", "", value);
			}
		}
		public string FindDialogTemplateContentPath {
			get {
				return GetStringProperty("FindDialogTemplateContentPath", "");
			}
			set {
				SetStringProperty("FindDialogTemplateContentPath", "", value);
			}
		}
		public string NestedFrameControlPath {
			get {
				return GetStringProperty("NestedFrameControlPath", "");
			}
			set {
				SetStringProperty("NestedFrameControlPath", "", value);
			}
		}
		public string DefaultTemplateContentPath {
			get {
				return GetStringProperty("DefaultTemplateContentPath", "");
			}
			set {
				SetStringProperty("DefaultTemplateContentPath", "", value);
			}
		}
		public string DefaultVerticalTemplateContentPath {
			get {
				return GetStringProperty("DefaultVerticalTemplateContentPath", "");
			}
			set {
				SetStringProperty("DefaultVerticalTemplateContentPath", "", value);
			}
		}
	}
}
