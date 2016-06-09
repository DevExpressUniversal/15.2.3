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
	internal class EmfPlusMetafile {
		public MetafileDataType Type { get; set; }
		public System.Drawing.Imaging.Metafile Metafile { get; set; }
		public EmfPlusMetafile(MetaReader reader) {
			Type = (MetafileDataType)reader.ReadUInt32();
			uint metafileDataSize = reader.ReadUInt32();
			if(Type == MetafileDataType.MetafileDataTypeWmfPlaceable) {
				reader.ReadBytes(24); 
			}
			byte[] metafileData = reader.ReadBytes((int)metafileDataSize);
			MemoryStream str1 = new MemoryStream(metafileData);
			Metafile = (System.Drawing.Imaging.Metafile)Image.FromStream(str1, true);
		}
	}
	public enum MetafileDataType {
		MetafileDataTypeWmf = 0x00000001,
		MetafileDataTypeWmfPlaceable = 0x00000002,
		MetafileDataTypeEmf = 0x00000003,
		MetafileDataTypeEmfPlusOnly = 0x00000004,
		MetafileDataTypeEmfPlusDual = 0x00000005
	}
}
