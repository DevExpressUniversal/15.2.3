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
using System.Collections.Generic;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class MethodCodeBase : MemberCode {
		private readonly List<ParameterCode> parameters;
		public MethodCodeBase(string name) : this(name, null) { }
		public MethodCodeBase(string name, string typeFullName)
			: base(name, typeFullName) {
			Visibility = Visibility.Public;
			parameters = new List<ParameterCode>();
		}
		protected void AddParameterCore(ParameterCode parameter) {
			parameters.Add(parameter);
		}
		protected void AppendParameters(CodeBuilder builder) {
			bool isFirstParameter = true;
			foreach(ParameterCode parameterCode in Parameters) {
				if(!isFirstParameter) {
					builder.Append(", ");
				}
				isFirstParameter = false;
				parameterCode.GetCode(builder);
			}
		}
		protected void InitLogicIfNeed(CodeBuilder builder, Type logicType, string logicName, EntityMetadata entityMetadata) {
			bool hasConstructorWithInterface = false;
			foreach(InterfaceMetadata interfaceInfo in entityMetadata.OwnImplementedInterfaces) {
				if(logicType.GetConstructor(new Type[] { interfaceInfo.InterfaceType }) != null) {
					hasConstructorWithInterface = true;
					break;
				}
			}
			builder.AppendLineFormat("if({0} == null) {{", logicName);
			builder.PushIndent();
			if(hasConstructorWithInterface) {
				builder.AppendLineFormat("{0} = new {1}(this);", logicName, CodeBuilder.TypeToString(logicType));
			}
			else {
				builder.AppendLineFormat("{0} = new {1}();", logicName, CodeBuilder.TypeToString(logicType));
			}
			builder.PopIndent();
			builder.AppendLine("}");
		}
		public ParameterCode[] Parameters {
			get { return parameters.ToArray(); }
		}
	}
}
