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
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Pdf;
using System.IO;
using System.Drawing.Drawing2D;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class EmfPlusRegionNode {
		private RegionNodeDataType Type { get; set; }
		public Region Region { get; set; }
		public EmfPlusRegionNode(MetaReader reader) {
			Type = (RegionNodeDataType)reader.ReadUInt32();
			switch(Type) {
				case RegionNodeDataType.RegionNodeDataTypeRect:
					Region = new Region(reader.ReadRectF());
					break;
				case RegionNodeDataType.RegionNodeDataTypePath:
					Region =  new Region(new EmfPlusRegionNodePath(reader).Path);
					break;
				case RegionNodeDataType.RegionNodeDataTypeEmpty:
					Region = new Region();
					Region.MakeEmpty();
					break;
				case RegionNodeDataType.RegionNodeDataTypeInfinite: 
					Region = new Region();
					Region.MakeInfinite();
					break;
				default:
					Region = new EmfPlusRegionNodeChildNodes(reader, Type).Region;
					break;
			}
		}
	}
	public enum RegionNodeDataType {
		RegionNodeDataTypeAnd = 0x00000001,
		RegionNodeDataTypeOr = 0x00000002,
		RegionNodeDataTypeXor = 0x00000003,
		RegionNodeDataTypeExclude = 0x00000004,
		RegionNodeDataTypeComplement = 0x00000005,
		RegionNodeDataTypeRect = 0x10000000,
		RegionNodeDataTypePath = 0x10000001,
		RegionNodeDataTypeEmpty = 0x10000002,
		RegionNodeDataTypeInfinite = 0x10000003
	}
}
