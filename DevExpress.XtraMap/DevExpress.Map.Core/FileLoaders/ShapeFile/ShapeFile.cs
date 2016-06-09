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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
namespace DevExpress.Map.Native {
	public class Shapefile {
		readonly ShpHeader shpHeader;
		readonly DbfHeader dbfHeader;
		readonly List<ShpRecord> shpRecords;
		readonly List<DbfRecord> dbfRecords;
		public ShpHeader ShpHeader { get { return shpHeader; } }
		public DbfHeader DbfHeader { get { return dbfHeader; } }
		public List<ShpRecord> ShpRecords { get { return shpRecords; } }
		public List<DbfRecord> DbfRecords { get { return dbfRecords; } }
		public Shapefile(Stream shpSource, Stream dbfSource)
			: this(shpSource, dbfSource, DXEncoding.Default) {
		}
		public Shapefile(Stream shpSource, Stream dbfSource, Encoding defaultEncoding) {
			ShpLoader shpLoader = shpSource != null ? new ShpLoader(shpSource) : null;
			DbfLoaderCore dbfLoader = dbfSource != null ? new DbfLoaderCore(dbfSource, defaultEncoding) : null;
			shpHeader = shpLoader != null ? shpLoader.Header : null;
			shpRecords = shpLoader != null ? shpLoader.Records : null;
			dbfHeader = dbfLoader != null ? dbfLoader.Header : null;
			dbfRecords = dbfLoader != null ? dbfLoader.Records : null;
		}
	}
	public class ShpHeader {
		public int FileCode { get; set; }
		public int Empty1 { get; set; }
		public int Empty2 { get; set; }
		public int Empty3 { get; set; }
		public int Empty4 { get; set; }
		public int Empty5 { get; set; }
		public int FileLength { get; set; }
		public int Version { get; set; }
		public int ShapeType { get; set; }
		public double Xmin { get; set; }
		public double Ymin { get; set; }
		public double Xmax { get; set; }
		public double Ymax { get; set; }
		public double Zmin { get; set; }
		public double Zmax { get; set; }
		public double Mmin { get; set; }
		public double Mmax { get; set; }
	}
}
