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
using DevExpress.Data.IO;
using PivotCellValue = DevExpress.XtraPivotGrid.Data.PivotCellValue;
using FormattedPivotCellValue = DevExpress.XtraPivotGrid.Data.FormattedPivotCellValue;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class MeasuresStorage {
		protected MeasuresStorage() {
		}
		public abstract int Count { get; }
		public abstract PivotCellValue GetPivotCellValue(IQueryMetadataColumn measure);
		public abstract object GetValue(IQueryMetadataColumn measure);	   
		public abstract bool SetFormattedValue(IQueryMetadataColumn measure, object value, string format, int locale);
		public abstract void SaveToStream(MeasureStorageKeepHelperBase helper, TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes);
		internal abstract void Remove(IQueryMetadataColumn column);
	}
	public abstract class MeasureStorageKeepHelperBase {
		internal abstract MeasuresStorage Load(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes);
		public virtual void SaveToStream(TypedBinaryWriter writer, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
		}
		public virtual void ReadFromStream(TypedBinaryReader reader, List<IQueryMetadataColumn> columnIndexes) {
		}
	}
}
