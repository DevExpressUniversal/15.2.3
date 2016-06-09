#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class CodeBuilder {
		private readonly StringBuilder builder;
		private int indentLevel;
		public CodeBuilder() {
			builder = new StringBuilder();
			indentLevel = 0;
		}
		public void PushIndent() {
			indentLevel++;
		}
		public void PopIndent() {
			indentLevel--;
		}
		public CodeBuilder AppendIndent() {
			builder.Append(string.Empty.PadRight(indentLevel, '\t'));
			return this;
		}
		public static string TypeToString(Type type) {
			if(type == null) {
				return string.Empty;
			}
			if(Nullable.GetUnderlyingType(type) != null) {
				return TypeToString(Nullable.GetUnderlyingType(type)) + "?";
			}
			if(!type.IsGenericType) {
				if(type.Name == "Void") {
					return "void";
				}
				string result = "global::" + type.FullName.Replace('+', '.');
				return result.Replace("&", "");
			}
			string genericArgsStr = string.Empty;
			string typeName = type.Name;
			string[] splits = typeName.Split('`');
			if(splits.Length > 0) {
				typeName = splits[0];
				Type[] genericArgs = type.GetGenericArguments();
				if(genericArgs.Length > 0) {
					foreach(Type currentArg in genericArgs) {
						genericArgsStr += TypeToString(currentArg) + ",";
					}
					genericArgsStr = genericArgsStr.TrimEnd(',');
				}
			}
			return string.Format("global::{0}.{1}<{2}>", type.Namespace, typeName, genericArgsStr);
		}
		public CodeBuilder Append(string text) {
			builder.Append(text);
			return this;
		}
		public CodeBuilder Append(Type type) {
			builder.Append(TypeToString(type));
			return this;
		}
		public CodeBuilder AppendFormat(string format, params object[] args) {
			builder.AppendFormat(format, args);
			return this;
		}
		public CodeBuilder AppendLineFormat(string format, params object[] args) {
			AppendIndent();
			builder.AppendFormat(format, args);
			builder.AppendLine();
			return this;
		}
		public CodeBuilder AppendLine(string text) {
			AppendIndent();
			builder.AppendLine(text);
			return this;
		}
		public CodeBuilder AppendNewLine() {
			builder.AppendLine();
			return this;
		}
		public override string ToString() {
			return builder.ToString();
		}
	}
}
