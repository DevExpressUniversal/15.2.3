#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardWin.Design {
	public class ColorSchemeDimensionKeyCodeDomSerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			ColorSchemeDimensionKey dimensionKey = (ColorSchemeDimensionKey)value;
			CodeStatementCollection statements = new CodeStatementCollection();
			string name = manager.GetName(dimensionKey.DimensionDefinition);
			if(name == null) {
				name = GetUniqueName(manager, dimensionKey.DimensionDefinition);
				CodeVariableDeclarationStatement statement = new CodeVariableDeclarationStatement(dimensionKey.DimensionDefinition.GetType(), name);
				statement.InitExpression = (CodeExpression)GetSerializer(manager, dimensionKey.DimensionDefinition).Serialize(manager, dimensionKey.DimensionDefinition);
				statements.Add(statement);
			}
			string name2 = manager.GetName(dimensionKey);
			if(name2 == null) {
				name2 = GetUniqueName(manager, dimensionKey);
				CodeVariableDeclarationStatement statement2 = new CodeVariableDeclarationStatement(dimensionKey.GetType(), name2);
				CodeExpression keyValueExpression = (CodeExpression)GetSerializer(manager, dimensionKey.Value).Serialize(manager, dimensionKey.Value);
				statement2.InitExpression = new CodeObjectCreateExpression(typeof(ColorSchemeDimensionKey), new CodeVariableReferenceExpression(name), keyValueExpression);
				statements.Add(statement2);
			}
			SetExpression(manager, value, new CodeVariableReferenceExpression(name2));
			return statements;
		}
	}
}
