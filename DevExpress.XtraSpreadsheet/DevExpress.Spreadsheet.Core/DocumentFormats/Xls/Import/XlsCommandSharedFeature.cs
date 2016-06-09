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

using System.IO;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandSharedFeature
	public class XlsCommandSharedFeature : XlsCommandRecordBase {
		static short[] typeCodes = new short[] {
			0x0812  
		};
		XlsSharedFeatureBase feature;
		protected internal XlsSharedFeatureBase Feature { get { return feature; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Sheet) {
				FutureRecordHeader header = FutureRecordHeader.FromStream(reader);
				using (XlsCommandStream stream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader featureReader = new BinaryReader(stream)) {
						XlsSharedFeatureType featureType = (XlsSharedFeatureType)featureReader.ReadUInt16();
						if (featureType == XlsSharedFeatureType.Protection)
							feature = XlsSharedFeatureProtection.FromStream(featureReader);
						else if (featureType == XlsSharedFeatureType.Fec2)
							feature = XlsSharedFeatureErrorCheck.FromStream(featureReader);
						else if (featureType == XlsSharedFeatureType.Factoid)
							feature = XlsSharedFeatureSmartTags.FromStream(featureReader);
						else
							contentBuilder.ThrowInvalidFile("Unknown shared feature type");
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			feature.Execute(contentBuilder);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedFeature();
		}
	}
	#endregion
	#region XlsCommandSharedFeature11
	public class XlsCommandSharedFeature11 : XlsCommandRecordBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x0875, 
			0x0812  
		};
		#endregion
		SharedFeatureTable tableObject;
		public SharedFeatureTable TableObject { get { return tableObject; } set { tableObject = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Sheet) {
				TableFutureRecordHeader header = TableFutureRecordHeader.FromStream(reader);
				using (XlsCommandStream tableStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader tableReader = new BinaryReader(tableStream)) {
						tableObject = new SharedFeatureTable(contentBuilder.CurrentSheet, header.CellRangeInfo, header.RecordTypeId);
						tableObject.Read(tableReader, contentBuilder);
					}
				}
			} else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			tableObject.CreateTable(contentBuilder);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedFeature11();
		}
	}
	#endregion 
	#region XlsCommandSharedFeature12
	public class XlsCommandSharedFeature12 : XlsCommandSharedFeature11 {
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedFeature12();
		}
	}
	#endregion
}
