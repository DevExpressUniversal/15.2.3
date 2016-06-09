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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using DevExpress.XtraTreeList.Design.Serializers;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraTreeList.Design.Serializers {
	sealed public class ViewStylesCodeDomSerializer : DevExpress.Utils.Serializers.HashCodeDomSerializer {
		protected override bool IsNeedSerializeEntry(object value, DictionaryEntry entry) {
			if(entry.Key != null && entry.Value != null && entry.Key.Equals(entry.Value)) return false;
			return true;
		}
		protected override string AddMethodName { get { return "AddReplace"; } }
	}
	sealed public class TreeListAlphaStylesCodeDomSerializer : DevExpress.Utils.Serializers.HashCodeDomSerializer {
		protected override bool IsNeedSerializeEntry(object value, DictionaryEntry entry) {
			bool res = ((DevExpress.XtraTreeList.Blending.AlphaStyles)value).IsNeedSerializeStyle(entry);
			return res;
		}
		protected override string AddMethodName { get { return "AddReplace"; } }
	}
	public class TreeListNodesCodeDomSerializer : CodeDomSerializer {
		private IDesignerSerializationManager manager = null;
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			TreeListNodes nodes = value as TreeListNodes;
			if(nodes == null || nodes.Count == 0) return null;
			if(!nodes.TreeList.IsUnboundMode) return null;
			object stackVal = manager.Context.Current;
			if(stackVal == null) return null;
#if DXWhidbey
			if (stackVal is ExpressionContext) {
				stackVal = ((ExpressionContext)stackVal).Expression;
			}
#endif
			CodeExpression refExp = null;
			if(stackVal is CodePropertyReferenceExpression) {
				refExp = stackVal as CodePropertyReferenceExpression;
			} else {
				if(stackVal.GetType().Name == "CodeValueExpression") {
					refExp = stackVal.GetType().InvokeMember("Expression", System.Reflection.BindingFlags.GetProperty, null, stackVal, null) as CodeExpression;
				}
			}
			if(!(refExp is CodePropertyReferenceExpression)) return null;
			refExp = ((CodePropertyReferenceExpression)refExp).TargetObject;
			CodeStatementCollection cs = new CodeStatementCollection();
			this.manager = manager;
			SerializeTreeListMethod("BeginUnboundLoad", refExp, cs);
			TreeListOperationSerializeNodes op = new TreeListOperationSerializeNodes(cs, this, refExp, nodes.TreeList);
			nodes.TreeList.NodesIterator.DoOperation(op);
			SerializeTreeListMethod("EndUnboundLoad", refExp, cs);
			this.manager = null;
			return cs;
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return null;
		}
		internal CodeExpression SerializeParameterExpression(object[] param) {
			CodeArrayCreateExpression result = new CodeArrayCreateExpression();
			result.CreateType = new CodeTypeReference(param.GetType());
			for(int i = 0; i < param.Length; i++) {
				object arg = param[i];
				CodeExpression exp = null;
				if(arg != null) {
					Type argType = arg.GetType();
					TypeCode argCode = Type.GetTypeCode(argType);
					if(argCode == TypeCode.DateTime) {
						DateTime dt = (DateTime)arg;
						exp = new CodeObjectCreateExpression(argType, new CodeExpression[] {new CodePrimitiveExpression(dt.Year), new CodePrimitiveExpression(dt.Month), new CodePrimitiveExpression(dt.Day), new CodePrimitiveExpression(dt.Hour), new CodePrimitiveExpression(dt.Minute), new CodePrimitiveExpression(dt.Second)});
					}
					if(argCode == TypeCode.Decimal) {
					}
				}
				if(exp == null) exp = new CodePrimitiveExpression(arg);
				result.Initializers.Add(exp);
			}
			return result;
		}
		protected void SerializeTreeListMethod(string methodName, CodeExpression ce, CodeStatementCollection cs) {
			CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression();
			invoke.Method = new CodeMethodReferenceExpression(ce, methodName);
			cs.Add(invoke);
		}
	}
}
namespace DevExpress.XtraTreeList.Design {
	internal class TreeListOperationSerializeNodes : DevExpress.XtraTreeList.Nodes.Operations.TreeListOperation {
		private CodeStatementCollection cs; 
		private TreeListNodesCodeDomSerializer serializer;
		private TreeList treeList;
		private CodeExpression tlRef;
		private int defImageIndex, defSelectImageIndex, defStateImageIndex;
		private CheckState defCheckState;
		internal TreeListOperationSerializeNodes(CodeStatementCollection cs, TreeListNodesCodeDomSerializer serializer,
			CodeExpression tlRef, TreeList treeList) {
			this.cs = cs;
			this.serializer = serializer;
			this.tlRef = tlRef;
			this.treeList = treeList;
			SetDefaultPropertyValues();
		}
		public override void Execute(TreeListNode node) { 
			CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression();
			invoke.Method = MethodExpression;
			invoke.Parameters.Add(serializer.SerializeParameterExpression(GetNodeData(node)));
			int id = (node.ParentNode == null ? -1 : node.ParentNode.Id);
			CodeExpression ce = new CodePrimitiveExpression(id);
			invoke.Parameters.Add(ce);
			if(NeedsSerializeImageIndexes(node)) {
				invoke.Parameters.Add(new CodePrimitiveExpression(node.ImageIndex));
				invoke.Parameters.Add(new CodePrimitiveExpression(node.SelectImageIndex));
				invoke.Parameters.Add(new CodePrimitiveExpression(node.StateImageIndex));
			}
			if(node.CheckState != defCheckState) 
				invoke.Parameters.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CheckState)), node.CheckState.ToString()));
			cs.Add(invoke);
		}
		private object[] GetNodeData(TreeListNode node) {
			object[] data = new object[treeList.Columns.Count];
			for(int i = 0; i < treeList.Columns.Count; i++) {
				object cellData = node[i];
				if(cellData is Array) {
					Array arr = (Array)cellData;
					if(arr.Length > 10)
						cellData = "{Length=" + arr.Length.ToString() + "}";
				}
				data[i] = cellData;
			}
			return data;
		}
		private void SetDefaultPropertyValues() {
			defImageIndex = (int)GetDefaultValueAttributeValue("ImageIndex");
			defSelectImageIndex = (int)GetDefaultValueAttributeValue("SelectImageIndex");
			defStateImageIndex = (int)GetDefaultValueAttributeValue("StateImageIndex");
			defCheckState = (CheckState)GetDefaultValueAttributeValue("CheckState");
		}
		private object GetDefaultValueAttributeValue(string propertyName) {
			Type nodeType = typeof(TreeListNode);
			MemberInfo[] info = nodeType.GetMember(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if(info != null && info.Length > 0) {
				object[]attributes = info[0].GetCustomAttributes(typeof(DefaultValueAttribute), true);
				if(attributes != null && attributes.Length > 0) {
					DefaultValueAttribute myAttribute = (DefaultValueAttribute)attributes[0];
					return myAttribute.Value; 
				}
			}
			return -1;
		}
		private bool NeedsSerializeImageIndexes(TreeListNode node) {
			return (node.ImageIndex != defImageIndex ||
				node.SelectImageIndex != defSelectImageIndex ||
				node.StateImageIndex != defStateImageIndex);
		}
		private CodeMethodReferenceExpression MethodExpression {
			get { return new CodeMethodReferenceExpression(tlRef, AppendNodeName); }
		}
		private string AppendNodeName { get { return "AppendNode"; } }
	}
}
