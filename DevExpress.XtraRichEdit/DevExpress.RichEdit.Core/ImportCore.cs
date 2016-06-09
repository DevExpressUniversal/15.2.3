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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Import;
using DevExpress.Utils.Controls;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Import {
	#region DocumentImporterOptions (abstract class)
	[ComVisible(true)]
	public abstract class DocumentImporterOptions : RichEditNotificationOptions, IImporterOptions {
		#region Fields
		Encoding actualEncoding;
		string sourceUri;
		LineBreakSubstitute lineBreakSubstitute;
		UpdateFieldOptions updateField;
		#endregion
		protected DocumentImporterOptions() {
			CreateUpdateFieldOptions();
		}
		void CreateUpdateFieldOptions() {
			if (updateField != null)
				return;
			updateField = new UpdateFieldOptions();
			updateField.Changed += OnInnerOptionsChanged;
		}
		void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		#region Properties
		#region UpdateField
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("DocumentImporterOptionsUpdateField")]
#endif
		public UpdateFieldOptions UpdateField { get { return updateField; } }
		#endregion
		#region ActualEncoding
		protected internal virtual Encoding ActualEncoding {
			get { return actualEncoding; }
			set {
				if (Object.Equals(actualEncoding, value))
					return;
				Encoding oldEncoding = actualEncoding;
				actualEncoding = value;
				OnChanged("Encoding", oldEncoding, actualEncoding);
			}
		}
		#endregion
		#region SourceUri
		[Browsable(false)]
		public virtual string SourceUri { get { return sourceUri; } set { sourceUri = value; } }
		protected internal virtual bool ShouldSerializeSourceUri() {
			return false;
		}
		protected internal LineBreakSubstitute LineBreakSubstitute { get { return lineBreakSubstitute; } set {  lineBreakSubstitute = value; } }
		#endregion
		protected internal abstract DocumentFormat Format { get; }
		#endregion
		protected internal override void ResetCore() {
			this.actualEncoding = DXEncoding.Default;
			this.SourceUri = String.Empty;
			this.LineBreakSubstitute = LineBreakSubstitute.None;
			CreateUpdateFieldOptions();
			this.UpdateField.ResetCore();
		}
		public virtual void CopyFrom(IImporterOptions value) {
			DocumentImporterOptions options = value as DocumentImporterOptions;
			if (options != null) {
				this.ActualEncoding = options.ActualEncoding;
				this.LineBreakSubstitute = options.LineBreakSubstitute;
				this.UpdateField.CopyFrom(options.UpdateField);
			}
		}
	}
	#endregion
	#region XmlBasedDocumentImporterOptions
	[ComVisible(true)]
	public abstract class XmlBasedDocumentImporterOptions : DocumentImporterOptions {
		bool ignoreParseErrors;
		#region ReplaceSpaceWithNonBreakingSpaceInsidePre
		[DefaultValue(false), NotifyParentProperty(true)]
		public bool IgnoreParseErrors {
			get { return ignoreParseErrors; }
			set {
				if (value == ignoreParseErrors)
					return;
				ignoreParseErrors = value;
				OnChanged("IgnoreParseErrors", !value, value);
			}
		}
		#endregion        
		protected internal override void ResetCore() {
			base.ResetCore();
			this.IgnoreParseErrors = false;
		}
		public override void CopyFrom(IImporterOptions value) {
			base.CopyFrom(value);
			XmlBasedDocumentImporterOptions options = value as XmlBasedDocumentImporterOptions;
			 if (options != null) {
				 this.IgnoreParseErrors = options.IgnoreParseErrors;
			 }
		}
	}
	#endregion
	[ComVisible(true)]
	public class UpdateFieldOptions : RichEditNotificationOptions {
		const bool defaultDate = true;
		const bool defaultTime = true;
		bool date = defaultDate;
		bool time = defaultTime;
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("UpdateFieldOptionsDate"),
#endif
		DefaultValue(defaultDate)]
		[NotifyParentProperty(true)]
		public bool Date
		{
			get { return date; }
			set
			{
				if (Date == value)
					return;
				bool oldDate = date;
				date = value;
				OnChanged("Date", oldDate, date);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("UpdateFieldOptionsTime"),
#endif
		DefaultValue(defaultTime)]
		[NotifyParentProperty(true)]
		public bool Time
		{
			get { return time; }
			set
			{
				if (Time == value)
					return;
				bool oldTime = time;
				time = value;
				OnChanged("Time", oldTime, time);
			}
		}
		protected internal override void ResetCore() {
			this.Date = defaultDate;
			this.Time = defaultTime;
		}
		public virtual void CopyFrom(UpdateFieldOptions source) {
			this.Date = source.Date;
			this.Time = source.Time;
		}
		internal FieldUpdateOnLoadOptions GetNativeOptions() {
			return new FieldUpdateOnLoadOptions(Date, Time);
		}
	}
}
