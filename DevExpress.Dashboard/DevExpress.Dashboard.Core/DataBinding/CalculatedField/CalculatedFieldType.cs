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
using DevExpress.DashboardCommon.Localization;
using DevExpress.Data;
namespace DevExpress.DashboardCommon {
	public enum CalculatedFieldType {
		String,
		Integer,
		Decimal,
		Boolean,
		DateTime,
		Object,
	}
}
namespace DevExpress.DashboardCommon.Native {
	public static class CalculatedFieldTypeHelper {
		public static string GetLocalizedName(this CalculatedFieldType type) {
			DashboardStringId id;
			switch(type) {
				case CalculatedFieldType.Boolean:
					id = DashboardStringId.CalculatedFieldTypeBoolean;
					break;
				case CalculatedFieldType.DateTime:
					id = DashboardStringId.CalculatedFieldTypeDateTime;
					break;
				case CalculatedFieldType.Decimal:
					id = DashboardStringId.CalculatedFieldTypeDecimal;
					break;
				case CalculatedFieldType.Integer:
					id = DashboardStringId.CalculatedFieldTypeInteger;
					break;
				case CalculatedFieldType.String:
					id = DashboardStringId.CalculatedFieldTypeString;
					break;
				case CalculatedFieldType.Object:
				default:
					id = DashboardStringId.CalculatedFieldTypeObject;
					break;
			}
			return DashboardLocalizer.GetString(id);
		}
		public static UnboundColumnType ToUnboundColumnType(this  CalculatedFieldType type) {
			switch(type) {
				case CalculatedFieldType.Boolean:
					return UnboundColumnType.Boolean;
				case CalculatedFieldType.DateTime:
					return UnboundColumnType.DateTime;
				case CalculatedFieldType.Decimal:
					return UnboundColumnType.Decimal;
				case CalculatedFieldType.Integer:
					return UnboundColumnType.Integer;
				case CalculatedFieldType.String:
					return UnboundColumnType.String;
				default:
					return UnboundColumnType.Object;
			}
		}
		public static DataFieldType ToDataFieldType(this CalculatedFieldType type) {
			switch(type) {
				case CalculatedFieldType.Boolean:
					return DataFieldType.Bool;
				case CalculatedFieldType.DateTime:
					return DataFieldType.DateTime;
				case CalculatedFieldType.Decimal:
					return DataFieldType.Decimal;
				case CalculatedFieldType.Integer:
					return DataFieldType.Integer;
				case CalculatedFieldType.String:
					return DataFieldType.Text;
				default:
					return DataFieldType.Custom;
			}
		}
		public static Type ToType(this CalculatedFieldType type) {
			switch(type) {
				case CalculatedFieldType.Boolean:
					return typeof(bool);
				case CalculatedFieldType.DateTime:
					return typeof(DateTime);
				case CalculatedFieldType.Decimal:
					return typeof(decimal);
				case CalculatedFieldType.Integer:
					return typeof(int);
				case CalculatedFieldType.String:
					return typeof(string);
				default:
					return typeof(object);
			}
		}
		public static Type ToType(this DataFieldType dataFieldType) {
			switch(dataFieldType) {
				case DataFieldType.Bool:
					return typeof(bool);
				case DataFieldType.DateTime:
					return typeof(DateTime);
				case DataFieldType.Decimal:
					return typeof(decimal);
				case DataFieldType.Float:
					return typeof(float);
				case DataFieldType.Double:
					return typeof(double);
				case DataFieldType.Integer:
					return typeof(int);
				case DataFieldType.Text:
					return typeof(string);
				case DataFieldType.Custom:
				case DataFieldType.Unknown:
				default:
					return typeof(object);
			}
		}
	}
}
