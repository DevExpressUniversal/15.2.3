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
using DevExpress.XtraReports.UI;
namespace DevExpress.Data.Browsing {
	public static class FieldTypeConverter {
		public static Type ToType(FieldType fieldType) {
			if (fieldType == FieldType.Boolean)
				return typeof(bool);
			if (fieldType == FieldType.DateTime)
				return typeof(DateTime);
			if (fieldType == FieldType.TimeSpan)
				return typeof(TimeSpan);
			if (fieldType == FieldType.Byte)
				return typeof(byte);
			if (fieldType == FieldType.Int16)
				return typeof(Int16);
			if (fieldType == FieldType.Decimal)
				return typeof(Decimal);
			if (fieldType == FieldType.Double)
				return typeof(double);
			if (fieldType == FieldType.Float)
				return typeof(float);
			if (fieldType == FieldType.Int32)
				return typeof(int);
			if (fieldType == FieldType.String)
				return typeof(string);
			if (fieldType == FieldType.Guid)
				return typeof(Guid);
			return typeof(object);
		}
	}
}
