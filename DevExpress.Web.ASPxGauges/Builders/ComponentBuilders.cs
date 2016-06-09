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
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.UI;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Localization;
namespace DevExpress.Web.ASPxGauges.Design {
	public class ASPxGaugeControlBuilder : ControlBuilder {
		public override void ProcessGeneratedCode(CodeCompileUnit codeCompileUnit, CodeTypeDeclaration baseType, CodeTypeDeclaration derivedType, CodeMemberMethod buildMethod, System.CodeDom.CodeMemberMethod dataBindingMethod) {
			if(buildMethod != null) ProcessComponentInitializationCode(buildMethod);
			base.ProcessGeneratedCode(codeCompileUnit, baseType, derivedType, buildMethod, dataBindingMethod);
		}
		protected void ProcessComponentInitializationCode(CodeMemberMethod buildMethod) {
			Type buildedType = Type.GetType(buildMethod.ReturnType.BaseType, false);
			if(typeof(ASPxGaugeControl).IsAssignableFrom(buildedType)) {
				CodeExpression beginBuild = ControlBuilderHelper.CreateBeginMethodInvokeStatement("BeginInit");
				CodeExpression endBuild = ControlBuilderHelper.CreateEndMethodInvokeStatement("EndInit");
				if(beginBuild != null && endBuild != null) {
					ControlBuilderHelper.RebuildBuildMethod(buildMethod, beginBuild, endBuild);
				}
			}
		}
	}
}
namespace DevExpress.Web.ASPxGauges.Base {
	public static class ControlBuilderHelper {
		public static void RebuildBuildMethod(CodeMemberMethod buildMethod, CodeExpression beginMethod, CodeExpression endMethod) {
			int statementCount = buildMethod.Statements.Count;
			if(statementCount <= 2) return;
			CodeStatement[] statements = new CodeStatement[statementCount];
			buildMethod.Statements.CopyTo(statements, 0);
			buildMethod.Statements.Clear();
			for(int i = 0; i < statements.Length; i++) {
				if(i == 2) {					
					buildMethod.Statements.Add(beginMethod);
				}
				buildMethod.Statements.Add(statements[i]);
				if(i == statementCount - 2) {   
					buildMethod.Statements.Add(endMethod);
				}
			}
		}
		public static CodeExpression CreateBeginMethodInvokeStatement(string methodName) {
			CodeMethodInvokeExpression beginBuildMethod = new CodeMethodInvokeExpression();
			CodeExpression _ctrlVarRef = new CodeVariableReferenceExpression("__ctrl");
			beginBuildMethod.Method = new CodeMethodReferenceExpression(_ctrlVarRef, methodName);
			return beginBuildMethod;
		}
		public static CodeExpression CreateEndMethodInvokeStatement(string methodName) {
			CodeMethodInvokeExpression endBuildMethod = new CodeMethodInvokeExpression();
			CodeExpression _ctrlVarRef = new CodeVariableReferenceExpression("__ctrl");
			endBuildMethod.Method = new CodeMethodReferenceExpression(_ctrlVarRef, methodName);
			return endBuildMethod;
		}
	}
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
	[TypeConverter(typeof(ThicknessWebConverter))]
	public class ThicknessWeb : IThickness {
		int leftCore;
		int rightCore;
		int topCore;
		int bottomCore;
		public ThicknessWeb() { }
		public ThicknessWeb(IThickness t) {
			leftCore = t.Left;
			topCore = t.Top;
			rightCore = t.Right;
			bottomCore = t.Bottom;
		}
		public ThicknessWeb(int all) {
			leftCore = all;
			topCore = all;
			rightCore = all;
			bottomCore = all;
		}
		public ThicknessWeb(int left, int top, int right, int bottom) {
			leftCore = left;
			topCore = top;
			rightCore = right;
			bottomCore = bottom;
		}
		[RefreshProperties(RefreshProperties.All)]
		public int All {
			get {
				if(leftCore == bottomCore && leftCore == rightCore && topCore == bottomCore)
					return leftCore;
				return -1;
			}
			set {
				leftCore = value;
				topCore = value;
				rightCore = value;
				bottomCore = value;
			}
		}
		public int Left {
			get { return leftCore; }
			set { leftCore = value; }
		}
		public int Top {
			get { return topCore; }
			set { topCore = value; }
		}
		public int Right {
			get { return rightCore; }
			set { rightCore = value; }
		}
		public int Bottom {
			get { return bottomCore; }
			set { bottomCore = value; }
		}
		[Browsable(false)]
		public int Height {
			get { return Top + Bottom; }
		}
		[Browsable(false)]
		public int Width {
			get { return Left + Right; }
		}
		public override bool Equals(object obj) {
			if(!(obj is IThickness)) return false;
			return this == (IThickness)obj;
		}
		public static bool operator ==(ThicknessWeb t1, ThicknessWeb t2) {
			return ((((t1.Left == t2.Left) && (t1.Top == t2.Top)) && (t1.Right == t2.Right)) && (t1.Bottom == t2.Bottom));
		}
		public static bool operator !=(ThicknessWeb t1, ThicknessWeb t2) {
			return !(t1 == t2);
		}
		public static bool operator ==(ThicknessWeb t1, IThickness t2) {
			return ((((t1.Left == t2.Left) && (t1.Top == t2.Top)) && (t1.Right == t2.Right)) && (t1.Bottom == t2.Bottom));
		}
		public static bool operator !=(ThicknessWeb t1, IThickness t2) {
			return !(t1 == t2);
		}
		public override int GetHashCode() {
			return (((Left ^ ((Top << 13) | (Top >> 0x13))) ^ ((Width << 0x1a) | (Width >> 6))) ^ ((Height << 7) | (Height >> 0x19)));
		}
	}
	public class ThicknessWebConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return ((destinationType == typeof(InstanceDescriptor)) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			string str = value as string;
			if(str == null) {
				return base.ConvertFrom(context, culture, value);
			}
			string str2 = str.Trim();
			if(str2.Length == 0) {
				return null;
			}
			if(culture == null) {
				culture = System.Globalization.CultureInfo.CurrentCulture;
			}
			char ch = culture.TextInfo.ListSeparator[0];
			string[] strArray = str2.Split(new char[] { ch });
			int[] numArray = new int[strArray.Length];
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
			for(int i = 0; i < numArray.Length; i++) {
				numArray[i] = (int)converter.ConvertFromString(context, culture, strArray[i]);
			}
			if(numArray.Length != 4) {
				if(numArray.Length == 1)
					return new ThicknessWeb(numArray[0]);
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgTextParsingError) + str2, typeof(Thickness).Name));
			}
			return new ThicknessWeb(numArray[0], numArray[1], numArray[2], numArray[3]);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(value is ThicknessWeb) {
				ThicknessWeb thickness = (ThicknessWeb)value;
				if(destinationType == typeof(string)) {
					if(culture == null) {
						culture = System.Globalization.CultureInfo.CurrentCulture;
					}
					string separator = culture.TextInfo.ListSeparator + " ";
					TypeConverter converter = TypeDescriptor.GetConverter(typeof(int));
					if(thickness.All == -1) {
						string[] strArray = new string[4];
						strArray[0] = converter.ConvertToString(context, culture, thickness.Left);
						strArray[1] = converter.ConvertToString(context, culture, thickness.Top);
						strArray[2] = converter.ConvertToString(context, culture, thickness.Right);
						strArray[3] = converter.ConvertToString(context, culture, thickness.Bottom);
						return string.Join(separator, strArray);
					}
					else return converter.ConvertToString(context, culture, thickness.All);
				}
				if(destinationType == typeof(InstanceDescriptor)) {
					if(thickness.All == -1) {
						ConstructorInfo constructor = typeof(ThicknessWeb).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
						if(constructor != null) {
							return new InstanceDescriptor(constructor, new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom });
						}
					}
					else {
						ConstructorInfo constructor = typeof(ThicknessWeb).GetConstructor(new Type[] { typeof(int) });
						if(constructor != null) {
							return new InstanceDescriptor(constructor, new object[] { thickness.All });
						}
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
			if(propertyValues == null) {
				throw new ArgumentNullException("propertyValues");
			}
			object left = propertyValues["Left"];
			object top = propertyValues["Top"];
			object right = propertyValues["Right"];
			object bottom = propertyValues["Bottom"];
			if(
				(((left == null) || (top == null)) || ((right == null) || (bottom == null))) ||
				((!(left is int) || !(top is int)) || (!(right is int) || !(bottom is int)))) {
				throw new ArgumentException(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgInvalidClassCreationParameters), typeof(ThicknessWeb).Name));
			}
			return new ThicknessWeb((int)left, (int)top, (int)right, (int)bottom);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return TypeDescriptor.GetProperties(typeof(ThicknessWeb), attributes).Sort(new string[] { "All", "Left", "Top", "Right", "Bottom" });
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
