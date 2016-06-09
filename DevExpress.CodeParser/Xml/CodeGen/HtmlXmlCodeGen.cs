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
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Xml
#else
namespace DevExpress.CodeParser.Xml
#endif
{
	public class HtmlXmlCodeGen: CodeGen
	{
		public HtmlXmlCodeGen()
			: base()
		{
			if (Options != null)
				Options.IndentString = "\t";
		}
		public HtmlXmlCodeGen(CodeGenOptions options)
			: base(options)
		{
		}
		protected override HtmlXmlCodeGenBase CreateHtmlXmlGen()
		{
			return new HtmlXmlNodesCodeGen(this);
		}
		#region Creating methods (return null)...
		protected override TemplateParameterCodeGenBase CreateTemplateParameterGen()
		{
			return null;
		}
		protected override TemplateCodeGenBase CreateTemplateGen() 
		{
			return null;
		}
		protected override DirectiveCodeGenBase CreateDirectiveGen() 
		{
			return null;
		}
		protected override ExpressionCodeGenBase CreateExpressionGen()
		{
			return null;
		}
		protected override MemberCodeGenBase CreateMemberGen() 
		{
			return null;
		}
		protected override StatementCodeGenBase CreateStatementGen() 
		{
			return null;
		}
		protected override SupportElementCodeGenBase CreateSupportElementGen() 
		{
			return null;
		}
		protected override TypeDeclarationCodeGenBase CreateTypeDeclarationGen() 
		{
			return null;
		}
		protected override XmlCodeGenBase CreateXmlGen() 
		{
			return null;
		}
		protected override NamespaceReferenceGenBase CreateNamespaceReferenceGen()
		{
			return null;
		}
		protected override NamespaceGenBase CreateNamespaceGen() 
		{
			return null;
		}
		protected override SnippetCodeGenBase CreateSnippetGen()
		{
			return new HtmlXmlSnippetCodeGen(this);
		}
		#endregion
		public override void GenerateElement(LanguageElement element)
		{
			if (element == null)
				return;
			if (element is XmlNode || element is Comment || element is BaseCssElement)
				HtmlXmlCodeGen.GenerateElement(element);
			else if (element is ISnippetCodeElement)
				SnippetGen.GenerateElement(element);
			else if (element is ElementList)
				GenerateXmlElementList(element as ElementList);
			else
			{
				GenerateNodes(element.Nodes);
			}
		}
		void GenerateNodes(ICollection nodes)
		{
			if (nodes == null)
				return;
			int count = nodes.Count;
			if (count <= 0)
				return;
			IEnumerator lEnum = nodes.GetEnumerator();
			while (lEnum.MoveNext())
			{
				LanguageElement element = lEnum.Current as LanguageElement;
				GenerateElement(element);
			}
		}
		void GenerateXmlElementList(ElementList list)
		{
			if (list == null)
					return;
			int count = list.Nodes.Count;
			LanguageElement element = null;
			for (int i = 0; i < count; i++)
			{
				element = list.Nodes[i] as LanguageElement;
				if (element != null && (element is XmlNode || element is Comment || element is BaseCssElement || element is ISnippetCodeElement))
					GenerateElement(element);
			}
		}
		#region MemberVisibilitySpecifier
		public override void GenerateMemberVisibilitySpecifier(MemberVisibilitySpecifier specifier)
		{
		}
		#endregion
	}
}
