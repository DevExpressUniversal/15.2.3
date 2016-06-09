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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Export;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit.Export {
	#region DocumentExporterOptions (abstract class)
	[ComVisible(true)]
	public abstract class DocumentExporterOptions : RichEditNotificationOptions, IExporterOptions {
		#region Fields
		Encoding actualEncoding;
		string targetUri;
		#endregion
		protected DocumentExporterOptions() {
		}
		#region Properties
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
		#region TargetUri
		[Browsable(false)]
		public string TargetUri { get { return targetUri; } set { targetUri = value; } }
		protected internal virtual bool ShouldSerializeTargetUri() {
			return false;
		}
		#endregion
		protected internal abstract DocumentFormat Format { get; }
		#endregion
		protected internal override void ResetCore() {
			this.actualEncoding = GetDefaultEncoding();
			this.TargetUri = String.Empty;
		}
		protected virtual Encoding GetDefaultEncoding() {
			return DXEncoding.Default;
		}
		public virtual void CopyFrom(IExporterOptions value) {
			DocumentExporterOptions options = value as DocumentExporterOptions;
			if (options != null) {
				this.ActualEncoding = options.ActualEncoding;
			}
		}
	}
	#endregion
}
