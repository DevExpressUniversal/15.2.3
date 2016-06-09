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
using DevExpress.Spreadsheet;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	[ComVisible(true)]
	public class WorkbookSaveOptions : SpreadsheetNotificationOptions { 
		#region Fields
		string defaultFileName;
		string currentFileName;
		DocumentFormat defaultFormat;
		DocumentFormat currentFormat;
		string startupPath = string.Empty;
		string altStartupPath = string.Empty;
		string libraryPath = string.Empty;
		#endregion
		#region Properties
		#region DefaultFileName
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookSaveOptionsDefaultFileName"),
#endif
NotifyParentProperty(true)]
		public string DefaultFileName
		{
			get { return defaultFileName; }
			set
			{
				if (defaultFileName == value)
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
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookSaveOptionsCurrentFileName"),
#endif
 NotifyParentProperty(true)]
		public string CurrentFileName
		{
			get { return currentFileName; }
			set
			{
				if (currentFileName == value)
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
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookSaveOptionsDefaultFormat"),
#endif
NotifyParentProperty(true)]
		public DocumentFormat DefaultFormat
		{
			get { return defaultFormat; }
			set
			{
				if (defaultFormat == value)
					return;
				DocumentFormat oldValue = defaultFormat;
				defaultFormat = value;
				OnChanged("DefaultFormat", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeDefaultFormat() {
			return DefaultFormat != DocumentFormat.OpenXml;
		}
		protected internal virtual void ResetDefaultFormat() {
			DefaultFormat = DocumentFormat.OpenXml;
		}
		#endregion
		#region CurrentFormat
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("WorkbookSaveOptionsCurrentFormat"),
#endif
NotifyParentProperty(true)]
		public DocumentFormat CurrentFormat
		{
			get { return currentFormat; }
			set
			{
				if (currentFormat == value)
					return;
				DocumentFormat oldValue = currentFormat;
				currentFormat = value;
				OnChanged("CurrentFormat", oldValue, value);
			}
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
		#region Environment
		protected internal string StartupPath {
			get { return startupPath; }
			set { startupPath = string.IsNullOrEmpty(value) ? string.Empty : value; }
		}
		protected internal string AltStartupPath {
			get { return altStartupPath; }
			set { altStartupPath = string.IsNullOrEmpty(value) ? string.Empty : value; }
		}
		protected internal string LibraryPath {
			get { return libraryPath; }
			set { libraryPath = string.IsNullOrEmpty(value) ? string.Empty : value; }
		}
		protected internal void ResetEnvironment() {
			this.startupPath = string.Empty;
			this.altStartupPath = string.Empty;
			this.libraryPath = string.Empty;
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			ResetDefaultFileName();
			ResetCurrentFileName();
			ResetDefaultFormat();
			ResetCurrentFormat();
			ResetEnvironment();
		}
		protected internal void CopyFrom(WorkbookSaveOptions source) {
			Guard.ArgumentNotNull(source, "source");
			this.defaultFileName = source.defaultFileName;
			this.currentFileName = source.currentFileName;
			this.defaultFormat = source.defaultFormat;
			this.currentFormat = source.currentFormat;
			this.startupPath = source.startupPath;
			this.altStartupPath = source.altStartupPath;
			this.libraryPath = source.libraryPath;
		}
	}
}
