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
using System.ComponentModel;
using System.Linq;
using System.Runtime;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraReports;
namespace DevExpress.XtraPrinting.Native.DrillDown {
	public interface IDrillDownServiceBase {
		bool IsDrillDowning { get; set; }
		void Reset();
		IDictionary<DrillDownKey, bool> Keys { get; }
	}
	public struct DrillDownKey {
		public static readonly DrillDownKey Empty = new DrillDownKey(string.Empty, new int[0]);
		static DrillDownKey() {
			PrintingSystemXmlSerializer.ObjectConverterInstance.RegisterConverter(new OneTypeCustomObjectConverter(typeof(DrillDownKey), new DrillDownKeyConverter()));
		}
		public static void EnsureStaticConstructor() {
		}
		public static bool operator ==(DrillDownKey left, DrillDownKey right) {
			return left.EqualsCore(right);
		}
		[TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
		public static bool operator !=(DrillDownKey left, DrillDownKey right) {
			return !(left == right);
		}
		int hashCode;
		string name;
		int[] indices;
		public DrillDownKey(string name, params int[] indices) {
			this.name = name;
			this.indices = indices;
			hashCode = 0;
			for(int i = 0; i < indices.Length; i++)
				hashCode ^= HashCodeHelper.RotateValue(indices[i], indices.Length + 1);
			hashCode ^= HashCodeHelper.RotateValue(name.GetHashCode(), indices.Length + 1);
		 }
		public override string ToString() {
			string result = name;
			for(int i = 0; i < indices.Length; i++)
				result += "," + indices[i];
			return result;
		}
		public static DrillDownKey Parse(string s) {
			string[] items = s.Split(',');
			if(items.Length > 0) {
				int[] indices = new int[items.Length - 1];
				for(int i = 0; i < indices.Length; i++)
					indices[i] = int.Parse(items[i + 1]);
				return new DrillDownKey(items[0], indices);
			}
			return DrillDownKey.Empty;
		}
		public override int GetHashCode() {
			return hashCode;
		}
		public override bool Equals(object obj) {
			 return (obj is DrillDownKey)  ?  EqualsCore((DrillDownKey)obj) : false;
		}
		bool EqualsCore(DrillDownKey other) {
			if(indices.Length != other.indices.Length || name != other.name)
				return false;
			for(int i = 0; i < indices.Length; i++) {
				if(indices[i] != other.indices[i])
					return false;
			}
			return true;
		}
	}
	class DrillDownKeyConverter : ICustomObjectConverter {
		bool ICustomObjectConverter.CanConvert(Type type) {
			return IsValidType(type);
		}
		private static bool IsValidType(Type type) {
			return typeof(DrillDownKey).IsAssignableFrom(type);
		}
		object ICustomObjectConverter.FromString(Type type, string str) {
			if(IsValidType(type))
				return DrillDownKey.Parse(str);
			return null;
		}
		Type ICustomObjectConverter.GetType(string typeName) {
			if(typeName == typeof(DrillDownKey).GetType().FullName)
				return typeof(DrillDownKey);
			return null;
		}
		string ICustomObjectConverter.ToString(Type type, object obj) {
			if(obj is DrillDownKey)
				return ((DrillDownKey)obj).ToString();
			return string.Empty;
		}
	}
	interface IUpdateDrillDownReportStrategy {
		void Update(IReport report, PrintingSystemBase oldPrintingSystem);
	}
	class UpdateDrillDownReportStrategy : IUpdateDrillDownReportStrategy {
		public virtual void Update(IReport report, PrintingSystemBase oldPrintingSystem) {
			report.PrintingSystemBase.Watermark.CopyFrom(oldPrintingSystem.Watermark);
			report.PrintingSystemBase.Graph.PageBackColor = oldPrintingSystem.Graph.PageBackColor;
		}
	}	
}
