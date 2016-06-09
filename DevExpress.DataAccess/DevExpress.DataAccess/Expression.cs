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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
#if !DXPORTABLE
using System.Runtime.Serialization.Formatters.Binary;
#endif
namespace DevExpress.DataAccess {
	[TypeConverter(typeof(ExpressionTypeConverter))]
	[Serializable]
	public class Expression : IFormattable {
		class ExpressionTypeConverter : TypeConverter {
			static readonly ConstructorInfo constructorInfo =
				typeof(Expression).GetConstructor(new[] { typeof(string), typeof(Type) });
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				string s = value as string;
				if(s != null)
					return new Expression(s);
				return base.ConvertFrom(context, culture, value);
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if(sourceType == typeof(string))
					return true;
				return base.CanConvertFrom(context, sourceType);
			}
			#region Overrides of TypeConverter
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				if(destinationType == typeof(InstanceDescriptor))
					return true;
				return base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
				Type destinationType) {
				Expression expression = value as Expression;
				if(expression != null) {
					if(destinationType == typeof(InstanceDescriptor)) {
						return new InstanceDescriptor(constructorInfo,
							new object[] { expression.ExpressionString, expression.ResultType });
					}
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			#endregion
		}
		public string ExpressionString { get; set; }
		public Type ResultType { get; set; }
		public Expression(string expressionString) : this(expressionString, null) { }
		public Expression(string expressionString, Type resultType) {
			ExpressionString = expressionString;
			ResultType = resultType;
		}
		public Expression() {
		}
		public override string ToString() {
			return ExpressionString;
		}
		#region IFormattable Members
		string IFormattable.ToString(string format, IFormatProvider formatProvider) {
			return ExpressionString;
		}
		#endregion
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj))
				return true;
			if(obj == null || GetType() != obj.GetType())
				return false;
			Expression other = (Expression)obj;
			return string.Equals(ExpressionString, other.ExpressionString, StringComparison.Ordinal)
				   && ResultType == other.ResultType;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal static string ConvertToString(Expression value) {
			if(value == null)
				return null;
			return string.Format("({0})({1})", value.ResultType == null ? "null" : value.ResultType.AssemblyQualifiedName,
				value.ExpressionString);
		}
		internal static Expression ConvertFromString(string value) {
			const string regxTypeName = @"[\w\\`\[\]\.+*&,]+";
			const string regxAssemblyName = @"[\w\.]+";
			const string regxInt = @"\d{1,5}";
			const string regxVersionValue = regxInt + @"\." + regxInt + @"\." + regxInt + @"\." + regxInt;
			const string regxVersion = @"Version=" + regxVersionValue;
			const string regxCultureValue = @"[\w-""]+";
			const string regxCulture = @"Culture=" + regxCultureValue;
			const string regxByte = @"[\da-f]{2}";
			const string regxTokenActualValue = @"(?:" + regxByte + "){8,}";
			const string regxTokenValue = @"(?:(?:" + regxTokenActualValue + ")|(?:null))";
			const string regxToken = @"PublicKeyToken=" + regxTokenValue;
			const string regxAssemblyProperties = regxVersion + ", " + regxCulture + ", " + regxToken;
			const string regxAssemblyFullName = regxAssemblyName + @"(?:, " + regxAssemblyProperties + @")?";
			const string regxTypeFullName = regxTypeName + @"(?:, " + regxAssemblyFullName + @")?";
			const string pattern = @"\A\((" + regxTypeFullName + @")\)\((.*)\)\Z";
			Regex regex = new Regex(pattern);
			Match match = regex.Match(value);
			if(match.Success)
				return new Expression(match.Groups[2].Value) {
					ResultType = Type.GetType(match.Groups[1].Value, false)
				};
			Regex legacy = new Regex(@"\A(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)\Z");
			if(!legacy.IsMatch(value))
				return new Expression(value);
#if DXPORTABLE
			throw new InvalidOperationException();
#else
			BinaryFormatter formatter = new BinaryFormatter();
			byte[] buffer = Convert.FromBase64String(value);
			object obj;
			using(MemoryStream stream = new MemoryStream(buffer))
				obj = formatter.Deserialize(stream);
			try {
				return (Expression)obj;
			} catch(InvalidCastException) {
				Type type = obj.GetType();
				if(!string.Equals(type.FullName, typeof(Expression).FullName, StringComparison.Ordinal))
					throw;
				PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(type);
				string exprString = (string)pdc["ExpressionString"].GetValue(obj);
				PropertyDescriptor pdResultType = pdc["ResultType"];
				Type resultType = pdResultType == null ? null : (Type)pdResultType.GetValue(obj);
				return new Expression(exprString, resultType);
			}
#endif
		}
	}
}
