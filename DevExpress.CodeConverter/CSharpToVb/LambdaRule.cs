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
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.CodeConverter;
using DevExpress.CodeParser;
namespace DevExpress.CsToVbConverter {
	[ConvertLanguage("CSharp", "Basic")]
	public class LambdaRule : ConvertRule {
		public override void Convert(ConvertArgs args) {
			LambdaExpression lambda = args.ElementForConverting as LambdaExpression;
			if(lambda == null || lambda.Nodes == null || lambda.NodeCount == 0) return;
			bool fixInternalBlock = FixInternalBlock(lambda);
			if(!fixInternalBlock && lambda.NodeCount == 1) {
				MethodCallExpression mce = lambda.Nodes[0] as MethodCallExpression;
				lambda.IsFunction =
					(mce == null || !Regex.IsMatch(mce.Name, "^[a-z].*Action$")) &&
					!args.Resolver.IsVoidMethod(mce) &&
					!(lambda.Nodes[0] is IAssignmentExpression) &&
					(lambda.Nodes[0] is Expression || !(lambda.Nodes[0] is Statement));
			} else {
				lambda.IsFunction = lambda.Nodes.OfType<Return>().Where(r => r.Expression != null).Any();
			}
		}
		bool FixInternalBlock(LambdaExpression lambda) {
			if(lambda.NodeCount != 1) return false;
			LambdaExpression internalBlock = lambda.Nodes[0] as LambdaExpression;
			if(internalBlock == null) return false;
			lambda.Nodes.Clear();
			lambda.Nodes.AddRange(internalBlock.Nodes);
			return true;
		}
	}
}
