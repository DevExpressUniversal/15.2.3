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
using System.Text;
using DevExpress.Web.ASPxPivotGrid.Html;
namespace DevExpress.Web.ASPxPivotGrid.Data {
	public class CallbackState {
		public const string SerializedLayoutName = "FLDS";
		public const string CollapsedStateName = "FVS";
		public const string SavedDataCellsName = "DSS";
		public const string SavedFieldValuesName = "FITEMS";
		public const string SavedOLAPFieldValuesName = "OLPS";
		public const string SavedOLAPColumnsName = "OLCS";
		public const string SavedFieldDataTypesName = "FDT";
		public const string SavedDenyExpandStateName = "FVSE";
		public const string SavedFilterValuesIsEmptyName = "FSE";
		public const string InnerValueDelimiter = "_DEL_";
		public const string PartDelimiter = "_CBDEL_";
		public const string StartConstSign = SerializedLayoutName + PartDelimiter;
		public const string SavedOLAPSessionID = "OLSID";
		readonly Dictionary<string, string> stateValues;
		public CallbackState() {
			this.stateValues = new Dictionary<string, string>();
		}
		public string this[string name] {
			get {
				string res;
				if(stateValues.TryGetValue(name, out res))
					return res.Replace(InnerValueDelimiter, CallbackCommands.ArgumentsSeparator.ToString());
				return String.Empty;
			}
			set { stateValues[name] = value; }
		}
		public void Clear() {
			this.stateValues.Clear();
		}
		public void FromString(string state) {
			Clear();
			if(string.IsNullOrEmpty(state)) return;
			string[] values = state.Split(new string[] { PartDelimiter }, StringSplitOptions.None);
			for(int i = 0; i < values.Length; i += 2) {
				if(i == values.Length - 1) break;
				string name = values[i],
					value = values[i + 1];
				this[name] = value;
			}
		}
		public override string ToString() {
			StringBuilder stb = new StringBuilder();
			foreach(KeyValuePair<string, string> pair in stateValues) {
				string name = pair.Key, value = pair.Value;
				stb.Append(name).Append(PartDelimiter)
					.Append(value.Replace(CallbackCommands.ArgumentsSeparator.ToString(), InnerValueDelimiter)).Append(PartDelimiter);
			}
			if(stb[stb.Length - 1] == '|')
				stb.Remove(stb.Length - 2, 1);
			return stb.ToString();
		}
	}
}
