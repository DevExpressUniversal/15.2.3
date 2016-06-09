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

using DevExpress.XtraScheduler.Internal;
using System;
using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
namespace DevExpress.XtraScheduler.Design {
	public class AppointmentStatusCollectionCodeDomSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			if (manager == null || codeObject == null) {
				throw new ArgumentNullException();
			}
			CodeDomSerializer serializer = (CodeDomSerializer)manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer));
			if (serializer == null)
				return null;
			return serializer.Deserialize(manager, codeObject);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			object res = ((CodeDomSerializer)manager.GetSerializer(typeof(AppointmentStatusCollection).BaseType, typeof(CodeDomSerializer))).Serialize(manager, value);
			AppointmentStatusCollection list = value as AppointmentStatusCollection;
			if (list == null)
				return res;
			if (!(res is CodeStatementCollection))
				return res;
			CodeExpression targetObject = ((ExpressionContext)manager.Context.Current).Expression;
			if (targetObject == null)
				return res;
			CodeStatementCollection statements = (CodeStatementCollection)res;
			statements.Clear();
			CodeDomSerializer codeDomColorSerializer = ((CodeDomSerializer)manager.GetSerializer(typeof(Color), typeof(CodeDomSerializer)));
			foreach (AppointmentStatus item in list) {
				var someEnum = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(AppointmentStatusType)), item.Type.ToString());
				Color color = SchedulerBrushHelper.GetColorFromAppointmentStatusType(item.Type, item.Brush);
				CodeExpression colorCx = codeDomColorSerializer.Serialize(manager, color) as CodeExpression;
				CodeExpression cx = new CodeMethodInvokeExpression(targetObject, "Add", new CodeExpression[] { someEnum, new CodePrimitiveExpression(item.DisplayName), new CodePrimitiveExpression(item.MenuCaption), colorCx });
				if (cx == null)
					return res;
				statements.Add(cx);
			}
			return res;
		}
	}
}
