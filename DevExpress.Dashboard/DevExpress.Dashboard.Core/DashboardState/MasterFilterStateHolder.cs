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

using System.Linq;
using System.Collections.Generic;
using DevExpress.Data.IO;
namespace DevExpress.DashboardCommon.Native {
	public class MasterFilterStateHolder : BaseItemStateHolder {
		protected override void SerializeInternal(TypedBinaryWriter writer) {
			MasterFilterState value = (MasterFilterState)State;
			writer.WriteObject(value.IsSelectedValues);
			IValuesSet valuesSet = value.ValuesSet;
			IList<ISelectionColumn> selection = valuesSet.ToList<ISelectionColumn>();
			int length0 = selection.Count;
			writer.WriteObject(length0);
			for(int i = 0; i < length0; i++) {
				IList<object> valuesList = selection[i].ToList();
				writer.WriteObject(valuesList.Count);
				for (int j = 0; j < valuesList.Count(); j++)
					writer.WriteTypedObject(valuesList[j]);
			}
		}
		protected override void DeserializeInternal(TypedBinaryReader reader) {
			bool isSelectedValues = reader.ReadObject<bool>();
			int length0 = reader.ReadObject<int>();
			IList<ISelectionColumn> columns = new List<ISelectionColumn>();
			for(int i = 0; i < length0; i++) {
				IList<object> column = new List<object>();
				int length1 = reader.ReadObject<int>();
				for (int j = 0; j < length1; j++)
					column.Add(reader.ReadTypedObject());
				columns.Add(column.AsSelectionColumn());
			}
			State = new MasterFilterState() { IsSelectedValues = isSelectedValues, ValuesSet = columns.AsValuesSet() };
		}
	}
}
