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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
namespace DevExpress.Export {
	public enum ClipboardMode {
		Default,
		PlainText,
		Formatted
	}
	public enum ProgressMode {
		Automatic,
		Always,
		Never
	}
	public class ClipboardOptions :BaseOptions {
		bool allowFormattedMode = true;
		public ClipboardOptions(bool allowFormattedMode = true) {
			this.copyColumnHeadersCore = DefaultBoolean.Default;
			this.allowRtfFormatCore = DefaultBoolean.Default;
			this.allowHtmlFormatCore = DefaultBoolean.Default;
			this.allowExcelFormatCore = DefaultBoolean.Default;
			this.allowCsvFormatCore = DefaultBoolean.Default;
			this.allowTxtFormatCore = DefaultBoolean.Default;
			this.clipboardModeCore = ClipboardMode.Default;
			this.showProgressCore = ProgressMode.Automatic;
			this.allowFormattedMode = allowFormattedMode;
			this.copyCollapsedDataCore = DefaultBoolean.Default;
		}
		public void Assign(ClipboardOptions optionsCore) {
			this.copyColumnHeadersCore = optionsCore.copyColumnHeadersCore;
			this.allowRtfFormatCore = optionsCore.allowRtfFormatCore;
			this.allowHtmlFormatCore = optionsCore.allowHtmlFormatCore;
			this.allowExcelFormatCore = optionsCore.allowExcelFormatCore;
			this.allowCsvFormatCore = optionsCore.allowCsvFormatCore;
			this.allowTxtFormatCore = optionsCore.allowTxtFormatCore;
			this.clipboardModeCore = optionsCore.clipboardModeCore;
			this.showProgressCore = optionsCore.showProgressCore;
			this.copyCollapsedDataCore = optionsCore.copyCollapsedDataCore;
		}
		ClipboardMode clipboardModeCore;
		[DefaultValue(ClipboardMode.Default), XtraSerializableProperty()]
		public ClipboardMode ClipboardMode {
			get {
				if(!allowFormattedMode) return ClipboardMode.Default;
				return clipboardModeCore;
			}
			set {
				clipboardModeCore = value;
			}
		}
		ProgressMode showProgressCore;
		[DefaultValue(ProgressMode.Automatic), XtraSerializableProperty()]
		public ProgressMode ShowProgress {
			get {
				return showProgressCore;
			}
			set {
				showProgressCore = value;
			}
		}
		DefaultBoolean copyColumnHeadersCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean CopyColumnHeaders {
			get {
				return copyColumnHeadersCore;
			}
			set {
				copyColumnHeadersCore = value;
			}
		}
		DefaultBoolean allowTxtFormatCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowTxtFormat {
			get {
				return allowTxtFormatCore;
			}
			set {
				allowTxtFormatCore = value;
			}
		}
		DefaultBoolean allowRtfFormatCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowRtfFormat {
			get {
				return allowRtfFormatCore;
			}
			set {
				allowRtfFormatCore = value;
			}
		}
		DefaultBoolean allowHtmlFormatCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowHtmlFormat {
			get {
				return allowHtmlFormatCore;
			}
			set {
				allowHtmlFormatCore = value;
			}
		}
		DefaultBoolean allowExcelFormatCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowExcelFormat {
			get {
				return allowExcelFormatCore;
			}
			set {
				allowExcelFormatCore = value;
			}
		}
		DefaultBoolean allowCsvFormatCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean AllowCsvFormat {
			get {
				return allowCsvFormatCore;
			}
			set {
				allowCsvFormatCore = value;
			}
		}
		DefaultBoolean copyCollapsedDataCore;
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty()]
		public DefaultBoolean CopyCollapsedData {
			get {
				return copyCollapsedDataCore;
			}
			set {
				copyCollapsedDataCore = value;
			}
		}
	}
}
