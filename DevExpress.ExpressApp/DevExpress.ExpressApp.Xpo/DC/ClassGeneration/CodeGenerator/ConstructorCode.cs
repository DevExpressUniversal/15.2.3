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
using DevExpress.Xpo.Helpers;
namespace DevExpress.ExpressApp.DC.ClassGeneration {
	internal class ConstructorCode : MethodCodeBase {
		private readonly List<string> baseConstructorArguments;
		private List<Type> logicTypes;
		private EntityMetadata entityMetadata;
		public ConstructorCode(string className)
			: base(className) {
			baseConstructorArguments = new List<string>();
			logicTypes = new List<Type>();
		}
		protected override void GetCodeCore(CodeBuilder builder) {
			base.GetCodeCore(builder);
			builder.AppendFormat("{0}(", Name);
			AppendParameters(builder);
			if(baseConstructorArguments.Count == 0) {
				builder.Append(") {");
			}
			else {
				builder.AppendFormat(") : base({0}) {{", string.Join(", ", baseConstructorArguments.ToArray()));
			}
			builder.AppendNewLine();
			builder.PushIndent();
			GetConstructorBody(builder);
			builder.PopIndent();
			builder.AppendLine("}");
		}
		protected virtual void GetConstructorBody(CodeBuilder builder) {
			foreach(Type logicType in logicTypes) {
				string logicName = DcSpecificWords.FieldLogicInstancePrefix + logicType.Name;
				InitLogicIfNeed(builder, logicType, logicName, entityMetadata);
			}
		}
		public void AddParameter(ParameterCode parameter) {
			AddParameterCore(parameter);
		}
		public void AddBaseConstructorArgument(string baseConstructorArgument) {
			baseConstructorArguments.Add(baseConstructorArgument);
		}
		public void AddLogicInfo(List<Type> logicTypes, EntityMetadata entityMetadata) {
			this.logicTypes = logicTypes;
			this.entityMetadata = entityMetadata;
		}
	}
	internal class DataEntityClassConstructorCode : ConstructorCode {
		private readonly InstancePropertyCode instancePropertyCode;
		private readonly DataMetadata dataMetadata;
		public DataEntityClassConstructorCode(string className, InstancePropertyCode instancePropertyCode, DataMetadata dataMetadata) : base(className) {
			this.instancePropertyCode = instancePropertyCode;
			this.dataMetadata = dataMetadata;
			AddParameter(new ParameterCode("instance", instancePropertyCode.FieldTypeFullName));
			AddBaseConstructorArgument("instance.Session");
		}
		protected override void GetConstructorBody(CodeBuilder builder) {
			builder.AppendLineFormat("this.{0} = instance;", instancePropertyCode.FieldName);
			builder.AppendLine("this.Oid = instance.Oid;");
			foreach(DataMetadata aggregatedDataMetadata in dataMetadata.AggregatedData) {
				InterfaceMetadata primaryInterface = aggregatedDataMetadata.PrimaryInterface;
				builder.AppendLineFormat("this.{0} = (({1})instance).PersistentInterfaceData;", CodeModelGeneratorHelper.GetDataClassFieldName(aggregatedDataMetadata), CodeBuilder.TypeToString(typeof(IPersistentInterface<>).MakeGenericType(primaryInterface.InterfaceType)));
			}
		}
	}
}
