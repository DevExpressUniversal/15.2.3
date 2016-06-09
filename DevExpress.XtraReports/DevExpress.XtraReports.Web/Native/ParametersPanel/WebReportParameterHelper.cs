#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public static class WebReportParameterHelper {
		public static ReadOnlyCollection<Type> SignedDecimalTypes { get; private set; }
		public static ReadOnlyCollection<Type> UnsignedDecimalTypes { get; private set; }
		public static ReadOnlyCollection<Type> DecimalTypes { get; private set; }
		public static ReadOnlyCollection<Type> FixedPointTypes { get; private set; }
		public static ReadOnlyCollection<Type> NumericTypes { get; private set; }
		static WebReportParameterHelper() {
			SignedDecimalTypes = new ReadOnlyCollection<Type>(new[] { typeof(int), typeof(short), typeof(long), typeof(sbyte) });
			UnsignedDecimalTypes = new ReadOnlyCollection<Type>(new[] { typeof(uint), typeof(ushort), typeof(ulong), typeof(byte) });
			FixedPointTypes = new ReadOnlyCollection<Type>(new[] { typeof(float), typeof(double), typeof(decimal) });
			DecimalTypes = new ReadOnlyCollection<Type>(SignedDecimalTypes.Concat(UnsignedDecimalTypes).ToArray());
			NumericTypes = new ReadOnlyCollection<Type>(DecimalTypes.Concat(FixedPointTypes).ToArray());
		}
		public static Dictionary<Enum, string> GetEnumValues(Type type) {
			var converter = TypeDescriptor.GetConverter(type);
			return Enum.GetValues(type)
				.Cast<Enum>()
				.ToDictionary(x => x, x => converter.ConvertToString(x));
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
		public static void ConfigureValidationSettings(ValidationSettings settings, bool isFieldRequired = true) {
			settings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			settings.ValidateOnLeave = true;
			settings.Display = Display.Dynamic;
			settings.RequiredField.IsRequired = isFieldRequired;
			settings.RequiredField.ErrorText = string.Empty;
			settings.ErrorText = string.Empty;
		}
		public static SpinEditNumberType GetSpinEditNumberType(Type type) {
			return DecimalTypes.Contains(type) ? SpinEditNumberType.Integer : SpinEditNumberType.Float;
		}
		public static void FillListEditItems(ListEditItemCollection collection, Type type) {
			foreach(var pair in GetEnumValues(type)) {
				collection.Add(pair.Value, pair.Key);
			}
		}
	}
}
