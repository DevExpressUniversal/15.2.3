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

using DevExpress.CodeParser;
using DevExpress.CodeConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace DevExpress.CsToVbConverter {
	[ConvertLanguage("CSharp", "Basic")]
	public class EventHandlerRule : ConvertRule {
		public override void Convert(ConvertArgs args) {
			Assignment assignment = args.ElementForConverting as Assignment;
			if(assignment == null) return;
			ElementReferenceExpression eventRef = assignment.LeftSide as ElementReferenceExpression;
			if(eventRef == null) return;
			ElementReferenceExpression methodRef = assignment.Expression as ElementReferenceExpression;
			if(methodRef == null) return;
			if(!Regex.IsMatch(methodRef.Name, "^On[A-Z].*$")) return;
			if(assignment.AssignmentOperator == AssignmentOperatorType.PlusEquals)
				args.NewElement = new AddHandler(eventRef, new AddressOfExpression(methodRef));
			else if(assignment.AssignmentOperator == AssignmentOperatorType.MinusEquals)
				args.NewElement = new RemoveHandler(eventRef, new AddressOfExpression(methodRef));
			args.NodesAndDetailsHandled = true;
		}
	}
}
