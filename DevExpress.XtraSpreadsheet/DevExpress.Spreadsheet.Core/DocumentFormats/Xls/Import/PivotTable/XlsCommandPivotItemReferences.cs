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
using System.IO;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandPivotItemReferences -- SxItm --
	public class XlsCommandPivotItemReferences : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x003c 
		};
		readonly List<XlsPivotFilterItemReWriter> filterItems = new List<XlsPivotFilterItemReWriter>();
		#endregion
		#region Properties
		public List<XlsPivotFilterItemReWriter> FilterItems { get { return filterItems; } }
		#endregion
		#region Methods
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builderPivotView = contentBuilder.CurrentBuilderPivotView;
			XlsContentType contentType = contentBuilder.ContentType;
			if (contentType == XlsContentType.Sheet && builderPivotView != null && builderPivotView.FilterCountItem != 0)
				ReadCore(reader, contentBuilder, builderPivotView.FilterCountItem);
			else if (contentType == XlsContentType.PivotCache)
				ReadCore(reader, contentBuilder, contentBuilder.CurrentPivotCacheBuilder.FilterCountItem);
			else
				base.ReadCore(reader, contentBuilder);
		}
		void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder, int itemCount) {
			using (XlsCommandStream itemStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
				using (BinaryReader itemReader = new BinaryReader(itemStream)) {
					for (int i = 0; i < itemCount; i++)
						filterItems.Add(XlsPivotFilterItemReWriter.FromStream(itemReader));
				}
			}
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsBuildPivotView builder = contentBuilder.CurrentBuilderPivotView;
			if (builder != null && builder.Reference != null) {
				builder.FilterCountItem = 0;
				AddSharedItemIndexes(builder.Reference);
			}
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			PivotCacheCalculatedItemCollection collection = contentBuilder.DocumentModel.PivotCaches.Last.CalculatedItems;
			int count = collection.Count;
			AddSharedItemIndexes(collection[count - 1].PivotArea.References.Last);
		}
		void AddSharedItemIndexes(PivotAreaReference reference) {
			foreach (XlsPivotFilterItemReWriter element in FilterItems)
				reference.SharedItemsIndex.Add(element.Value);
		}
		public override IXlsCommand GetInstance() {
			filterItems.Clear();
			return this;
		}
		#endregion
	}
	public class XlsPivotFilterItemReWriter : IEquatable<XlsPivotFilterItemReWriter> {
		int value = 0;
		public int Value {
			get { return value; }
			set {
				if (value >= 0 && value <= 32500 || value == 32767)
					this.value = value;
				else
					ValueChecker.CheckValue(value, 0, 32500, "XlsPivotFilterItemReWriter Value");
			}
		}
		public static XlsPivotFilterItemReWriter FromStream(BinaryReader reader) {
			XlsPivotFilterItemReWriter result = new XlsPivotFilterItemReWriter();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			Value = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)Value);
		}
		public override bool Equals(Object other) {
			if (typeof(XlsPivotFilterItemReWriter) != other.GetType())
				return false;
			return this.value == ((XlsPivotFilterItemReWriter)other).value;
		}
		public bool Equals(XlsPivotFilterItemReWriter other) {
			return this.value == other.value;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
}
