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

using DevExpress.Xpo;
namespace DevExpress.XtraReports.Service.Native.DAL {
	public class StoredDocumentRelatedData : ExtendedXPObject {
		internal const string StoredDocumentRelatedDataBoxedExportModeAssociationName = "StoredDocumentRelatedData-BoxedExportMode";
		string exportOptionsExternalKey;
		string prnxExternalKey;
		string watermarkExternalKey;
		string drillDownKeysExternalKey;
		StoredServiceFault fault;
		string documentCulture;
		string documentUICulture;
		public string ExportOptionsExternalKey {
			get { return exportOptionsExternalKey; }
			set { SetPropertyValue(() => ExportOptionsExternalKey, ref exportOptionsExternalKey, value); }
		}
		public string PrnxExternalKey {
			get { return prnxExternalKey; }
			set { SetPropertyValue(() => PrnxExternalKey, ref prnxExternalKey, value); }
		}
		public string WatermarkExternalKey {
			get { return watermarkExternalKey; }
			set { SetPropertyValue(() => WatermarkExternalKey, ref watermarkExternalKey, value); }
		}
		public string DrillDownKeysExternalKey {
			get { return drillDownKeysExternalKey; }
			set { SetPropertyValue(() => DrillDownKeysExternalKey, ref drillDownKeysExternalKey, value); }
		}
		[Aggregated]
		[Association(StoredDocumentRelatedDataBoxedExportModeAssociationName)]
		public XPCollection<BoxedExportMode> ExportModes {
			get { return GetCollection<BoxedExportMode>(() => ExportModes); }
		}
		[Aggregated]
		public StoredServiceFault Fault {
			get { return fault; }
			set { SetPropertyValue(() => Fault, ref fault, value); }
		}
		public string DocumentCulture {
			get { return documentCulture; }
			set { SetPropertyValue(() => DocumentCulture, ref documentCulture, value); }
		}
		public string DocumentUICulture {
			get { return documentUICulture; }
			set { SetPropertyValue(() => DocumentUICulture, ref documentUICulture, value); }
		}
		public StoredDocumentRelatedData(Session session)
			: base(session) {
		}
	}
}
