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
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class BoxedExportMode : ExtendedXPObject {
		StoredDocumentRelatedData owner;
		SupportedExportOptionKind exportOptionKind;
		int exportOption;
		[Association(StoredDocumentRelatedData.StoredDocumentRelatedDataBoxedExportModeAssociationName)]
		public StoredDocumentRelatedData Owner {
			get { return owner; }
			set { SetPropertyValue<StoredDocumentRelatedData>(() => Owner, ref owner, value); }
		}
		public SupportedExportOptionKind ExportOptionKind {
			get { return exportOptionKind; }
			set { SetPropertyValue(() => ExportOptionKind, ref exportOptionKind, value); }
		}
		public int ExportOption {
			get { return exportOption; }
			set { SetPropertyValue(() => ExportOption, ref exportOption, value); }
		}
		public BoxedExportMode(Session session)
			: base(session) {
		}
		public BoxedExportMode(RtfExportMode exportMode, StoredDocumentRelatedData owner, Session session)
			: this(SupportedExportOptionKind.Rtf, (int)exportMode, owner, session) {
		}
		public BoxedExportMode(HtmlExportMode exportMode, StoredDocumentRelatedData owner, Session session)
			: this(SupportedExportOptionKind.Html, (int)exportMode, owner, session) {
		}
		public BoxedExportMode(ImageExportMode exportMode, StoredDocumentRelatedData owner, Session session)
			: this(SupportedExportOptionKind.Image, (int)exportMode, owner, session) {
		}
		public BoxedExportMode(XlsExportMode exportMode, StoredDocumentRelatedData owner, Session session)
			: this(SupportedExportOptionKind.Xls, (int)exportMode, owner, session) {
		}
		public BoxedExportMode(XlsxExportMode exportMode, StoredDocumentRelatedData owner, Session session)
			: this(SupportedExportOptionKind.Xlsx, (int)exportMode, owner, session) {
		}
		BoxedExportMode(SupportedExportOptionKind exportOptionKind, int exportOption, StoredDocumentRelatedData owner, Session session)
			: this(session) {
			ExportOptionKind = exportOptionKind;
			ExportOption = exportOption;
			Owner = owner;
		}
		public T GetExportOption<T>() {
			return (T)Enum.ToObject(typeof(T), ExportOption);
		}
	}
}
