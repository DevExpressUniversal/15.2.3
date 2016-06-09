#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
namespace DevExpress.DashboardCommon.Native {
	public class PivotStateHolder : BaseItemStateHolder {
		static void SerializeAreaState(TypedBinaryWriter writer, PivotAreaExpandCollapseState areaState) {
			SerializeValues(writer, areaState.HiddenValues);
			SerializeValues(writer, areaState.Values);
		}
		static PivotAreaExpandCollapseState DeserializeAreaState(TypedBinaryReader reader) {
			IList<object[]> hiddenValues = DeserializeValues(reader);
			IList<object[]> values = DeserializeValues(reader);
			return new PivotAreaExpandCollapseState(values, hiddenValues);
		}
		static void SerializeValues(TypedBinaryWriter writer, IList<object[]> values) {
			int count = values.Count;
			writer.WriteObject(count);
			foreach(object[] value in values) {
				int length = value.Length;
				writer.WriteObject(length);
				for(int i = 0; i < length; i++)
					writer.WriteTypedObject(value[i]);
			}
		}
		static IList<object[]> DeserializeValues(TypedBinaryReader reader) {
			List<object[]> values = new List<object[]>();
			int count = reader.ReadObject<int>();
			for(int valIndex = 0; valIndex < count; valIndex++) {
				int length = reader.ReadObject<int>();
				object[] value = new object[length];
				for(int i = 0; i < length; i++)
					value[i] = reader.ReadTypedObject();
				values.Add(value);
			}
			return values;
		}
		protected override void SerializeInternal(TypedBinaryWriter writer) {
			PivotDashboardItemExpandCollapseState state = (PivotDashboardItemExpandCollapseState)State;
			SerializeAreaState(writer, state.ColumnState);
			SerializeAreaState(writer, state.RowState);
		}
		protected override void DeserializeInternal(TypedBinaryReader reader) {
			PivotAreaExpandCollapseState columnArea = DeserializeAreaState(reader);
			PivotAreaExpandCollapseState rowArea = DeserializeAreaState(reader);
			State = new PivotDashboardItemExpandCollapseState(columnArea, rowArea);
		}
	}
}
