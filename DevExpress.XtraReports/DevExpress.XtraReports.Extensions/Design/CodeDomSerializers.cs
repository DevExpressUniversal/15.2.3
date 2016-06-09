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
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.CodeDom;
using System.Drawing;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design
{
	public class XRCodeDomSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			Guard.ArgumentNotNull(manager, "manager");
			Guard.ArgumentNotNull(codeObject, "codeObject");
			CodeDomSerializer serializer = manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer)) as CodeDomSerializer;
			return (serializer != null) ? serializer.Deserialize(manager, codeObject) : null;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			Guard.ArgumentNotNull(manager, "manager");
			Guard.ArgumentNotNull(value, "value");
			CodeDomSerializer serializer = manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer)) as CodeDomSerializer;
			if(serializer == null) return null;
			return serializer.Serialize(manager, value);
		}
	}
	public class XRControlCodeDomSerializer : XRCodeDomSerializer {
		#region static
		protected static int GetCodeStatementIndex(CodeStatementCollection statements, string propertyName) {
			for(int i = 0; i < statements.Count; i++) {
				if(StatementContainsProperty(statements[i], propertyName))
					return i;
			}
			return -1;
		}
		protected static bool StatementContainsProperty(CodeStatement statement, string propertyName) {
			CodeAssignStatement assignStatement = statement as CodeAssignStatement;
			if(assignStatement != null) {
				if(assignStatement.Left is CodePropertyReferenceExpression) {
					CodePropertyReferenceExpression expression = assignStatement.Left as CodePropertyReferenceExpression;
					return expression.PropertyName == propertyName;
				}
			}
			return false;
		}
		protected static string GetName(IDesignerSerializationManager manager, IComponent comp) {
			return (comp != null && comp.Site != null) ? manager.GetName(comp) : null;
		}
		#endregion
		public XRControlCodeDomSerializer() {
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			object codeObject = base.Serialize(manager, value);
			if(HasMixedInheritedChildren((XRControl)value) && codeObject != null)
				SerializeChildrenIndex(manager, (CodeStatementCollection)codeObject, (XRControl)value);
			return codeObject;
		}
		static bool HasMixedInheritedChildren(XRControl parent) {
			if(parent == null || parent.IsDisposed)
				return false;
			bool hasInheritedControl = false;
			bool hasNoInheritedControl = false;
			foreach(XRControl control in parent.Controls) {
				InheritanceAttribute attribute = TypeDescriptor.GetAttributes(control)[typeof(InheritanceAttribute)] as InheritanceAttribute;
				if(attribute != null && attribute.InheritanceLevel != InheritanceLevel.NotInherited) hasInheritedControl = true;
				else hasNoInheritedControl = true;
				if(hasInheritedControl && hasNoInheritedControl) return true;
			}
			return false;
		}
		void SerializeChildrenIndex(IDesignerSerializationManager manager, CodeStatementCollection statements, XRControl control) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(statements, "statements");
			if(control.Controls.Count <= 1) return;
			for(int i = control.Controls.Count - 1; i >= 0; i--) {
				XRControl component = control.Controls[i];
				if(component.Site == null || component.Site.Container != control.Site.Container) continue;
				InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
				if(attribute.InheritanceLevel == InheritanceLevel.InheritedReadOnly) continue;
				statements.Add(new CodeExpressionStatement(
					new CodeMethodInvokeExpression(
						new CodeMethodReferenceExpression(
							new CodePropertyReferenceExpression(
								base.SerializeToExpression(manager, control),
								"Controls"),
							"SetChildIndex"),
						base.SerializeToExpression(manager, component),
						base.SerializeToExpression(manager, 0))));
			}
		}
	}
	public class XRSubreportCodeDomSerializer : XRControlCodeDomSerializer {
		class ReportSourceMemberCodeDomSerializer : MemberCodeDomSerializer {
			MemberCodeDomSerializer parent;
			public ReportSourceMemberCodeDomSerializer() { 
			}
			public ReportSourceMemberCodeDomSerializer(MemberCodeDomSerializer parent) {
				this.parent = parent;
			}
			public override void Serialize(IDesignerSerializationManager manager, object owningObject, MemberDescriptor descriptor, CodeStatementCollection statements) {
				Guard.ArgumentNotNull(descriptor, "descriptor");
				PropertyDescriptor property = (PropertyDescriptor)descriptor;
				SerializeReportSourceProperty(manager, owningObject, statements, property); 
			}
			public override bool ShouldSerialize(IDesignerSerializationManager manager, object owningObject, MemberDescriptor descriptor) {
				return parent != null ? parent.ShouldSerialize(manager, owningObject, descriptor) : true;
			}
			internal void SerializeReportSourceProperty(IDesignerSerializationManager manager, object owningObject, CodeStatementCollection statements, PropertyDescriptor property) {
				object propertyValue = property.GetValue(owningObject);
				if(propertyValue != null) {
					CodeExpression ownerReferenceExpression = SerializeToExpression(manager, owningObject);
					if(GetExpression(manager, propertyValue) == null) {
						SetExpression(manager, propertyValue, new CodePropertyReferenceExpression(ownerReferenceExpression, "ReportSource"));
					}
					Type subreportType = propertyValue.GetType();
					statements.Add(new CodeAssignStatement(
						new CodePropertyReferenceExpression(ownerReferenceExpression, "ReportSource"),
						new CodeObjectCreateExpression(new CodeTypeReference(subreportType))
					));
				}
			}
		}
		class XRSubreportDesignerSerializationProvider : IDesignerSerializationProvider {
			public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType) {
				if(serializerType != typeof(MemberCodeDomSerializer) || !typeof(PropertyDescriptor).IsAssignableFrom(objectType))
					return null;
				if(currentSerializer == null || currentSerializer is ReportSourceMemberCodeDomSerializer)
					return null;
				PropertyDescriptor propertyDescriptor = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;
				return (propertyDescriptor != null && propertyDescriptor.Name == "ReportSource") 
					? new ReportSourceMemberCodeDomSerializer((MemberCodeDomSerializer)currentSerializer)
					: null;
			}
		}
		bool serializationProviderAdded;
		public XRSubreportCodeDomSerializer() {
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			Guard.ArgumentNotNull(manager, "manager");
			Guard.ArgumentNotNull(value, "value");
			IDesignerSerializationProvider provider = new XRSubreportDesignerSerializationProvider();
			try {
				manager.AddSerializationProvider(provider);
				serializationProviderAdded = true;
				return base.Serialize(manager, value);
			} finally {
				serializationProviderAdded = false;
				manager.RemoveSerializationProvider(provider);
			}
		}
		public override CodeStatementCollection SerializeMember(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member) {
			Guard.ArgumentNotNull(manager, "manager");
			Guard.ArgumentNotNull(owningObject, "owningObject");
			Guard.ArgumentNotNull(member, "member");
			IDesignerSerializationProvider provider = serializationProviderAdded ? new XRSubreportDesignerSerializationProvider() : null;
			if(provider != null) 
				manager.AddSerializationProvider(provider);
			try {
				return base.SerializeMember(manager, owningObject, member);
			} finally {
				if(provider != null)
					manager.RemoveSerializationProvider(provider);
			}
		}
	}
}
