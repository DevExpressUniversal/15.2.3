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
	public class DimensionDefinitionCodeDomSerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			DimensionDefinition dimensionDefinition = (DimensionDefinition)value;
			CodeExpression dataMemberExpr = null; 
			CodeExpression groupIntervalExpr = null;
			if (!string.IsNullOrEmpty(dimensionDefinition.DataMember))
				dataMemberExpr = new CodePrimitiveExpression(dimensionDefinition.DataMember);
			if (dimensionDefinition.DateTimeGroupInterval != DimensionDefinition.DefaultDateTimeGroupInterval)
				groupIntervalExpr = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(dimensionDefinition.DateTimeGroupInterval.GetType().FullName), dimensionDefinition.DateTimeGroupInterval.ToString());
			else if(dimensionDefinition.TextGroupInterval != DimensionDefinition.DefaultTextGroupInterval)
				groupIntervalExpr = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(dimensionDefinition.TextGroupInterval.GetType().FullName), dimensionDefinition.TextGroupInterval.ToString());
			if (dataMemberExpr != null) {
				if (groupIntervalExpr != null)
					return new CodeObjectCreateExpression(typeof(DimensionDefinition), dataMemberExpr, groupIntervalExpr);
				return new CodeObjectCreateExpression(typeof(DimensionDefinition), dataMemberExpr);
			}
			return null;
		}
	}
}
