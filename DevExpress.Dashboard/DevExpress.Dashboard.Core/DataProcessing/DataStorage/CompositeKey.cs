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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class CompositeKey {
		readonly int[] keys;
		public int[] Keys { get { return keys; } }
		public int Length { get { return keys.Length; } }
		public CompositeKey(params int[] keys) {
			this.keys = keys;
		}
		public CompositeKey GetParentKey() {
			return new CompositeKey(keys.Take(keys.Length - 1).ToArray());
		}
		public CompositeKey GetCurrentKey() {
			return new CompositeKey(keys.Reverse().Take(1).ToArray());
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for(int i = 0; i < keys.Length; i++) {
				sb.AppendFormat("{0}{1}", keys[i], i < keys.Length - 1 ? "," : String.Empty);
			}
			sb.Append("]");
			return sb.ToString();
		}
		public CompositeKey Join(CompositeKey key) {
			List<int> keys = new List<int>(this.keys);
			keys.AddRange(key.Keys);
			return new CompositeKey(keys.ToArray());
		}
		public override int GetHashCode() {
			return HashcodeHelper.GetCompositeHashCode(keys);
		}
		public override bool Equals(object obj) {
			CompositeKey key = (CompositeKey)obj;
			return CheckEquality(keys, key.keys);
		}
		bool CheckEquality(int[] keys1, int[] keys2) {
			if(keys1.Length != keys2.Length)
				return false;
			for(int i = 0; i < keys1.Length; i++) {
				if(keys1[i] != keys2[i])
					return false;
			}
			return true;
		}
	}
}
