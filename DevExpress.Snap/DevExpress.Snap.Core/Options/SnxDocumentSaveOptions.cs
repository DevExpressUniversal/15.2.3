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

using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.API;
using System.ComponentModel;
namespace DevExpress.Snap.Core.Options {
	public class SnxDocumentSaveOptions : DocumentSaveOptions {
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new DocumentFormat DefaultFormat { get { return base.DefaultFormat; } set { base.DefaultFormat = value; } }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new DocumentFormat CurrentFormat { get { return (base.CurrentFormat); } set { base.CurrentFormat = value; } }
		protected internal override DocumentFormat GetDefaultFormat() {
			return SnapDocumentFormat.Snap;
		}
		protected internal override void SetDefaultFormat(DocumentFormat value) {
		}
		protected internal override bool ShouldSerializeDefaultFormat() {
			return false;
		}
		protected internal override void ResetDefaultFormat() {
			DefaultFormat = SnapDocumentFormat.Snap;
		}
		protected internal override DocumentFormat GetCurrentFormat() {
			return SnapDocumentFormat.Snap;
		}
		protected internal override void SetCurrentFormat(DocumentFormat value) {
		}
		protected internal override bool ShouldSerializeCurrentFormat() {
			return false;
		}
		protected internal override void ResetCurrentFormat() {
			CurrentFormat = SnapDocumentFormat.Snap;
		}
	}
}
