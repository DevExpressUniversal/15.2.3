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
using System.Runtime.InteropServices;
using DevExpress.Office.Internal;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	[ComVisible(true)]
	public class DocumentSaveOptions : RichEditNotificationOptions, IDocumentSaveOptions<DocumentFormat> {
		#region Fields
		string defaultFileName;
		string currentFileName;
		DocumentFormat defaultFormat;
		DocumentFormat currentFormat;
		#endregion
		#region Properties
		#region DefaultFileName
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentSaveOptionsDefaultFileName"),
#endif
NotifyParentProperty(true)]
		public string DefaultFileName {
			get { return defaultFileName; }
			set {
				if(defaultFileName == value)
					return;
				string oldValue = defaultFileName;
				defaultFileName = value;
				OnChanged("DefaultFileName", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeDefaultFileName() {
			return !String.IsNullOrEmpty(DefaultFileName);
		}
		protected internal virtual void ResetDefaultFileName() {
			DefaultFileName = String.Empty;
		}
		#endregion
		#region CurrentFileName
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentSaveOptionsCurrentFileName"),
#endif
 NotifyParentProperty(true)]
		public string CurrentFileName {
			get { return currentFileName; }
			set {
				if(currentFileName == value)
					return;
				string oldValue = currentFileName;
				currentFileName = value;
				OnChanged("CurrentFileName", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeCurrentFileName() {
			return !String.IsNullOrEmpty(CurrentFileName);
		}
		protected internal virtual void ResetCurrentFileName() {
			CurrentFileName = String.Empty;
		}
		#endregion
		#region DefaultFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentSaveOptionsDefaultFormat"),
#endif
NotifyParentProperty(true)]
		public DocumentFormat DefaultFormat {
			get { return GetDefaultFormat(); }
			set { SetDefaultFormat(value); }
		}
		protected internal virtual DocumentFormat GetDefaultFormat() {
			return this.defaultFormat;
		}
		protected internal virtual void SetDefaultFormat(DocumentFormat value) {
			if (this.defaultFormat == value)
				return;
			DocumentFormat oldValue = this.defaultFormat;
			this.defaultFormat = value;
			OnChanged("DefaultFormat", oldValue, value);
		}
		protected internal virtual bool ShouldSerializeDefaultFormat() {
			return DefaultFormat != DocumentFormat.Rtf;
		}
		protected internal virtual void ResetDefaultFormat() {
			DefaultFormat = DocumentFormat.Rtf;
		}
		#endregion
		#region CurrentFormat
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DocumentSaveOptionsCurrentFormat"),
#endif
NotifyParentProperty(true)]
		public DocumentFormat CurrentFormat {
			get { return GetCurrentFormat(); }
			set { SetCurrentFormat(value); }
		}
		protected internal virtual DocumentFormat GetCurrentFormat() {
			return this.currentFormat;
		}
		protected internal virtual void SetCurrentFormat(DocumentFormat value) {
			if (this.currentFormat == value)
				return;
			DocumentFormat oldValue = this.currentFormat;
			this.currentFormat = value;
			OnChanged("CurrentFormat", oldValue, value);
		}
		protected internal virtual bool ShouldSerializeCurrentFormat() {
			return CurrentFormat != DocumentFormat.Undefined;
		}
		protected internal virtual void ResetCurrentFormat() {
			CurrentFormat = DocumentFormat.Undefined;
		}
		#endregion
		protected internal bool CanSaveToCurrentFileName {
			get {
#if !SL
				return true;
#else
				return false;
#endif
			}
		}
		#endregion
		protected internal override void ResetCore() {
			ResetDefaultFileName();
			ResetCurrentFileName();
			ResetDefaultFormat();
			ResetCurrentFormat();
		}
	}
}
