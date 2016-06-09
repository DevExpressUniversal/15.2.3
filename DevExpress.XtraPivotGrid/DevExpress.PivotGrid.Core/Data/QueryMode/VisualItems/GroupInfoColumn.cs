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

using System.Collections.Generic;
using DevExpress.Data.IO;
using System;
namespace DevExpress.PivotGrid.QueryMode {
	public class GroupInfoColumn : DictionaryContainer<GroupInfo, MeasuresStorage> {
		public GroupInfoColumn(int count)
			: base(count) {
		}
		public GroupInfoColumn()
			: base() {
		}
		internal MeasuresStorage CreateInstance(GroupInfo key, Func<MeasuresStorage> createNewDelegate) {
			MeasuresStorage measure = GetDictionaryValue(key);
			if(measure != null)
				return measure;
			measure = createNewDelegate();
			AddDictionaryValue(key, measure);
			return measure;
		}
		public MeasuresStorage this[GroupInfo key] {
			get { return GetDictionaryValue(key); }
			set { SetDictionaryValue(key, value); }
		}
		public virtual void SaveToStream(MeasureStorageKeepHelperBase helper, TypedBinaryWriter writer, Dictionary<GroupInfo, int> rowGroupIndexes, Dictionary<IQueryMetadataColumn, int> columnIndexes) {
			writer.Write(Count);
			foreach(KeyValuePair<GroupInfo, MeasuresStorage> pair in this) {
				writer.Write(rowGroupIndexes[pair.Key]);
				pair.Value.SaveToStream(helper, writer, columnIndexes);
			}
		}
		public void LoadFromStream(int rowCount, MeasureStorageKeepHelperBase helper, TypedBinaryReader reader, List<GroupInfo> rowGroups, List<IQueryMetadataColumn> columnIndexes) {
			EnsureCapcity(rowCount);
			for(int i = 0; i < rowCount; i++) {
				int groupIndex = reader.ReadInt32();
				MeasuresStorage measure = helper.Load(reader, columnIndexes);
				AddDictionaryValue(rowGroups[groupIndex], measure);
			}
		}
	}
}
