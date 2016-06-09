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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraSplashScreen.Design {
	class SplashScreenManagerSerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeStatementCollection statements = CreateComponentInitStatements(manager, (SplashScreenManager)value);
			return statements;
		}
		CodeStatementCollection CreateComponentInitStatements(IDesignerSerializationManager manager, SplashScreenManager component) {
			SplashScreenManagerStateSerializerBase serializer = SplashScreenManagerStateSerializerBase.Create(manager, component);
			return serializer.GetCodeStatements();
		}
	}
	abstract class SplashScreenManagerStateSerializerBase {
		SplashScreenManager componentCore;
		IDesignerSerializationManager serializationManager;
		public SplashScreenManagerStateSerializerBase(IDesignerSerializationManager sm, SplashScreenManager component) {
			this.componentCore = component;
			this.serializationManager = sm;
		}
		public SplashScreenManager Component { get { return this.componentCore; } }
		public IDesignerSerializationManager SerializationManager { get { return this.serializationManager; } }
		public static SplashScreenManagerStateSerializerBase Create(IDesignerSerializationManager manager, SplashScreenManager component) {
			if(component.ActiveSplashFormTypeInfo == null)
				return new NoneStateSerializer(manager, component);
			if(component.ActiveSplashFormTypeInfo.Mode == Mode.SplashScreen)
				return new SplashScreenStateSerializer(manager, component);
			return new WaitFormStateSerializer(manager, component);
		}
		public CodeExpression[] CreateCtorArgsList() {
			List<CodeExpression> result = new List<CodeExpression>();
			result.Add(new CodeThisReferenceExpression());
			if(Component.ActiveSplashFormTypeInfo != null)
				result.Add(new CodeTypeOfExpression(GetQualifierName(Component)));
			else
				result.Add(new CodePrimitiveExpression(null));
			result.Add(new CodePrimitiveExpression(Component.Properties.UseFadeInEffect));
			result.Add(new CodePrimitiveExpression(Component.Properties.UseFadeOutEffect));
			if(Component.SplashFormStartPosition == SplashFormStartPosition.Manual) {
				result.Add(GetSplashFormStartPositionExpression(Component.SplashFormStartPosition));
				result.Add(GetPointObjectCreateExpression(Component.SplashFormLocation));
			}
			if(VSServiceHelper.IsUserControlRootComponent(Component))
				result.Add(GetParentTypeUserControlExpression());
			if(Component.Properties.AllowGlowEffect) {
				result.Add(new CodePrimitiveExpression(Component.Properties.AllowGlowEffect));
			}
			return result.ToArray();
		}
		protected MemberAttributes? GetComponentAccessModifier() {
			PropertyDescriptor pd = TypeDescriptor.GetProperties(Component)["Modifiers"];
			if(pd == null) return null;
			return (MemberAttributes)pd.GetValue(Component);
		}
		protected CodeExpression GetParentTypeUserControlExpression() {
			return new CodeTypeOfExpression(typeof(UserControl));
		}
		protected CodeTypeReference GetQualifierName(SplashScreenManager component) {
			return new CodeTypeReference(component.ActiveSplashFormTypeInfo.TypeName, CodeTypeReferenceOptions.GlobalReference);
		}
		protected CodeExpression GetSplashFormStartPositionExpression(SplashFormStartPosition startPos) {
			CodeTypeReferenceExpression target = new CodeTypeReferenceExpression(startPos.GetType());
			return new CodeFieldReferenceExpression(target, Enum.GetName(startPos.GetType(), startPos));
		}
		protected CodeExpression GetPointObjectCreateExpression(Point pt) {
			CodeExpression[] args = new CodeExpression[] { new CodePrimitiveExpression(pt.X), new CodePrimitiveExpression(pt.Y) };
			return new CodeObjectCreateExpression(typeof(Point), args);
		}
		protected CodeAssignStatement CreateCodeAssignStatement(CodeExpression left, object value) {
			return new CodeAssignStatement(left, new CodePrimitiveExpression(value));
		}
		protected void AddPropertySectionHeader(CodeStatementCollection col, string componentName) {
			col.Add(new CodeCommentStatement(string.Empty));
			col.Add(new CodeCommentStatement(componentName));
			col.Add(new CodeCommentStatement(string.Empty));
		}
		public CodeStatementCollection GetCodeStatements() {
			CodeStatementCollection col = GetCodeStatementsCore();
			if(Component.Properties.ClosingDelay > 0) {
				AddPropertySectionHeader(col, Component.Site.Name);
				CodePropertyReferenceExpression propExp = new CodePropertyReferenceExpression(GetManagerReferenceExpression(), "ClosingDelay");
				col.Add(CreateCodeAssignStatement(propExp, Component.Properties.ClosingDelay));
			}
			return col;
		}
		protected abstract CodeExpression GetManagerReferenceExpression();
		protected abstract CodeStatementCollection GetCodeStatementsCore();
	}
	class SplashScreenStateSerializer : SplashScreenManagerStateSerializerBase {
		public SplashScreenStateSerializer(IDesignerSerializationManager sm, SplashScreenManager component) : base(sm, component) { }
		protected override CodeStatementCollection GetCodeStatementsCore() {
			base.SerializationManager.SerializationComplete += SerializationManager_SerializationComplete;
			CodeStatement localObj = new CodeVariableDeclarationStatement(Component.GetType(), Component.Site.Name, CtorExpression);
			return new CodeStatementCollection(new CodeStatement[] { localObj });
		}
		protected override CodeExpression GetManagerReferenceExpression() {
			return new CodeVariableReferenceExpression(Component.Site.Name);
		}
		void SerializationManager_SerializationComplete(object sender, EventArgs e) {
			base.SerializationManager.SerializationComplete -= SerializationManager_SerializationComplete;
			if(IsNeedFieldCleaning) AsyncCallCore((ParameterLessDelegate)delegate { ComponentCleaner.RemoveField(Component); });
		}
		void AsyncCallCore(Delegate method) {
			IDesignerHost host = VSServiceHelper.GetDesignerHost(Component);
			if(host == null)
				return;
			Control rootControl = host.RootComponent as Control;
			if(rootControl != null && rootControl.IsHandleCreated) rootControl.BeginInvoke(method);
		}
		bool IsNeedFieldCleaning { get { return base.SerializationManager.Properties.Count > 0; } }
		CodeExpression CtorExpression { get { return new CodeObjectCreateExpression(Component.GetType(), CreateCtorArgsList()); } }
	}
	class NoneStateSerializer : SplashScreenStateSerializer {
		public NoneStateSerializer(IDesignerSerializationManager sm, SplashScreenManager component) : base(sm, component) { }
	}
	class WaitFormStateSerializer : SplashScreenManagerStateSerializerBase {
		public WaitFormStateSerializer(IDesignerSerializationManager sm, SplashScreenManager component) : base(sm, component) { }
		protected override CodeStatementCollection GetCodeStatementsCore() {
			CodeTypeDeclaration codeType = SerializationManager.Context[typeof(CodeTypeDeclaration)] as CodeTypeDeclaration;
			if(codeType != null) {
				CodeMemberField field = new CodeMemberField(Component.GetType(), Component.Site.Name);
				MemberAttributes? attribs = GetComponentAccessModifier();
				if(attribs.HasValue) field.Attributes = attribs.Value;
				if(!codeType.Members.Contains(field)) codeType.Members.Add(field);
			}
			CodeExpression fieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), Component.Site.Name);
			CodeStatement assign = new CodeAssignStatement(fieldRef, new CodeObjectCreateExpression(Component.GetType(), CreateCtorArgsList()));
			return new CodeStatementCollection(new CodeStatement[] { assign });
		}
		protected override CodeExpression GetManagerReferenceExpression() {
			return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), Component.Site.Name);
		}
	}
}
