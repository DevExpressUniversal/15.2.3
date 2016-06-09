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
using DevExpress.Data.Browsing;
namespace DevExpress.Snap.Core.Native.Data {
	public class SNDataInfo : DataInfo {
		public SNDataInfo(object dataSource, string dataMember, string displayName)
			: this(dataSource, new[] { dataMember }, displayName) {
		}
		public SNDataInfo(object dataSource, string dataMember)
			: this(dataSource, new[] { dataMember }, dataMember) { 
		}
		public SNDataInfo(object dataSource, string[] dataPaths)
			: this(dataSource, dataPaths, dataPaths) { 
		}
		public SNDataInfo(object dataSource, string[] dataPaths, string[] escDataPaths)
			: this(dataSource, dataPaths, escDataPaths, string.Empty) { 
		}
		public SNDataInfo(object dataSource, string[] dataPaths, string displayName) 
			: this(dataSource, dataPaths, dataPaths, displayName) { 
		}
		public SNDataInfo(object dataSource, string[] dataPaths, string[] escDataPaths, string displayName)
			: this(dataSource, string.Join(".", dataPaths), dataPaths, escDataPaths, displayName) {
		}
		public SNDataInfo(object dataSource, string member, string[] dataPaths, string displayName)
			: this(dataSource, member, dataPaths, dataPaths, displayName) {
		}
		public SNDataInfo(object dataSource, string member, string[] dataPaths, string[] escDataPaths, string displayName)
			: base(dataSource, member, displayName) {
			this.DataPaths = dataPaths;
			this.EscapedDataPaths = escDataPaths;
		}
		public string[] DataPaths { get; private set; }
		public string[] EscapedDataPaths { get; private set; }
		internal SNDataInfo[] RelatedData { get; set; }
		internal int Level { get { return DataPaths.Length; } }
		internal bool IsNeighbour(SNDataInfo dataInfo) {
			if (!Object.ReferenceEquals(Source, dataInfo.Source))
				return false;
			if (Level != dataInfo.Level)
				return false;
			for (int i = 0; i < Level - 1; i++) {
				if (DataPaths[i] != dataInfo.DataPaths[i])
					return false;
			}
			return true;
		}
		public override bool Equals(object obj) {
			SNDataInfo dataInfo = obj as SNDataInfo;
			if (dataInfo == null)
				return false;
			return Object.ReferenceEquals(Source, dataInfo.Source) && Member == dataInfo.Member;
		}
		public override int GetHashCode() {
			return (Source.GetHashCode() << 15) | Member.GetHashCode();
		}
	}
}
