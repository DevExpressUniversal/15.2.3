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

using System.Collections;
using System.Collections.Generic;
using DevExpress.CodeParser;
using System.Linq;
using System.ComponentModel.Composition;
using System;
namespace DevExpress.CodeConverter
{
	internal class CodeConverter
	{
		const string languageDelemiter = "->";
		RulesByMode convertRules;
		string from;
		string to;
		ConvertArguments arguments;
		public CodeConverter(string from, string to, RulesByMode convertRules) {
			this.convertRules = convertRules;
			this.from = from;
			this.to = to;
		}
		internal static ParserLanguageID GetLanguageID(string languageName) {
			switch (languageName) {
				case "CSharp":
					return ParserLanguageID.CSharp;
				case "Basic":
					return ParserLanguageID.Basic;
				case "C/C++":
					return ParserLanguageID.Cpp;
				case "JavaScript":
					return ParserLanguageID.JavaScript;
				default:
					return ParserLanguageID.None;
			}
		}
		public string Convert(string code, ConvertArguments arguments = null) {
			if (arguments == null)
				this.arguments = new ConvertArguments();
			else
				this.arguments = arguments;
			ParserLanguageID fromLanguageId = GetLanguageID(from);
			ParserLanguageID toLanguageId = GetLanguageID(to);
			if (fromLanguageId == ParserLanguageID.None || toLanguageId == ParserLanguageID.None)
				return null;
			ParserBase parser = LanguageUtils.Create(fromLanguageId).CreateParser();
			if(parser == null)
				return null;
			LanguageElement root = parser.ParseString(code);
			if (root == null)
				return null;
			ConvertArgs args = Convert(root);
			CodeGen codeGen = LanguageUtils.Create(toLanguageId).CreateCodeGen();
			return codeGen.GenerateCode(args.NewElement);
		}
		ConvertArgs Convert(LanguageElement element) {
			ConvertArgs args = new ConvertArgs(element, arguments.Resolver);
			args.Resolver.Collect(element);
			Convert(args);
			return args;
		}
		void ConverWithRules(ConvertArgs args) {
			foreach (string mode in arguments.Modes) {
				IEnumerable<ConvertRule> rules = convertRules.GetRules(mode);
				if (rules == null)
					continue;
				foreach (ConvertRule rule in rules) {
					rule.Convert(args);
					if (args != null && !args.Resolver.ApplyLeftoverRules(args))
						return;
				}
			}
		}
		void ConvertOnlyElement(ConvertArgs args) {
			ConverWithRules(args);
			if (args.NewElement != null && args.NewElement != args.ElementForConverting) {
				if (!args.NodesAndDetailsHandled) {
					AddDetailNodes(args.NewElement, args.ElementForConverting.DetailNodes);
					AddNodes(args.NewElement, args.ElementForConverting.Nodes);
				}				
				Replace(args);				
			} else
				args.NewElement = args.ElementForConverting; 
			args.Resolver.CollectConverted(args.NewElement);
		}
		void Replace(ConvertArgs args) {
			LanguageElement oldElementParent = args.ElementForConverting.Parent;
			if(oldElementParent == null)
				return;
			if(args.ElementForConverting.IsDetailNode) {
				if(args.PrecendingElements == null)
					oldElementParent.ReplaceDetailNode(args.ElementForConverting, args.NewElement);
			}
			else {
				if(args.PrecendingElements == null)
					oldElementParent.ReplaceNode(args.ElementForConverting, args.NewElement);
			}			
		}
		public virtual void RemoveAllDetails(LanguageElement element) {
			if(element.DetailNodes == null)
				return;
			object[] details = element.DetailNodes.ToArray();
			foreach (LanguageElement detailNode in details)
				element.RemoveDetailNode(element);				
		}
		void Convert(ConvertArgs args) {
			ConvertOnlyElement(args);
			ConvertNodes(args, args.NewElement.DetailNodes);
			ConvertNodes(args, args.NewElement.Nodes);
		}
		void ConvertNodes(ConvertArgs parentArgs, NodeList nodes) {
			if(nodes == null)
				return;			
			object[] array = nodes.ToArray();
			foreach(LanguageElement element in array)
				Convert(new ConvertArgs(element, arguments.Resolver));
		}
		void AddDetailNodes(LanguageElement parent, NodeList nodes) {
			if(nodes == null || parent == null)
				return;
			foreach(LanguageElement node in nodes)
				parent.AddDetailNode(node);			
		}
		void AddNodes(LanguageElement parent, NodeList nodes) {
			if(nodes == null || parent == null)
				return;
			foreach(LanguageElement node in nodes)
				parent.AddNode(node);
		}
		public static string GetConverterDescritption(string from, string to) {
			return string.Format("{0}{1}{2}", from, languageDelemiter, to);
		}
		public override string ToString() {
			return GetConverterDescritption(from, to);
		}
	}
}
