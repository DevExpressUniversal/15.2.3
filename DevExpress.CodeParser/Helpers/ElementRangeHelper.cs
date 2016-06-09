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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	[Flags]
	public enum BlockElements
	{
		None = 0,
		Attributes = 0x01,
		XmlDocComments = 0x02,
		SupportComments = 0x04,
		Region = 0x08,
		LeadingWhiteSpace = 0x10,
		TrailingWhiteSpace = 0x20,
		LeadingEmptyLines = 0x40,
		TrailingEmptyLines = 0x80,
		WithoutUnsuitableRegions = 0x100,
		AllSupportElements = Attributes | XmlDocComments | SupportComments,
		AllLeadingWhiteSpaces = LeadingWhiteSpace | LeadingEmptyLines,
		AllTrailingWhiteSpaces = TrailingWhiteSpace | TrailingEmptyLines,
		AllWhiteSpaces = AllLeadingWhiteSpaces | AllTrailingWhiteSpaces,
		All = AllSupportElements | AllWhiteSpaces | Region
	}
	public sealed class ElementRangeHelper
	{
		public const BlockElements DefaultBlockElements = BlockElements.Region | BlockElements.XmlDocComments | BlockElements.Attributes;
		const int INT_FirstColumnPos = 1;
		ElementRangeHelper() {}
		static bool IsOptionSet(BlockElements target, BlockElements option)
		{
			return (target & option) == option;
		}
		static bool CanIncludeNode(LanguageElement node, BlockElements blockElements)
		{
			if (node == null)
				return false;
			BlockElements option = ToBlockElements(node);
			if (option == BlockElements.None)
				return false;
			return ((blockElements & option) == option);
		}
		private static bool ContainsOtherCode(LanguageElement element, RegionDirective directive)
		{
			if (element == null || directive == null)
				return false;
			SourceRange directiveRange = directive.Range;
			LanguageElement parent = element.Parent;
			if (parent == null)
				return false;
	  LanguageElement firstNode = parent.GetChildAfter(directiveRange.Start);
	  if (firstNode == null || !directiveRange.Contains(firstNode.Range))
		return false;
	  LanguageElement testNode = firstNode;
	  if (testNode is SupportElement)
		testNode = firstNode.NextCodeSibling;
	  if (testNode == null || !directiveRange.Contains(testNode.Range))
		return false;
	  if (testNode.Range != element.Range)
		return true;
	  testNode = testNode.NextCodeSibling;
	  return testNode != null && directiveRange.Contains(testNode.Range);
		}
	static bool CheckOtherCodeInRegions(RegionDirective nodeRegion, RegionDirective region, LanguageElement current)
	{
	  return !RegionDirective.Equals(nodeRegion, region) &&
		ContainsOtherCode(current, region);
	}
	static bool BreaksRegions(LanguageElement node, LanguageElement current, BlockElements blockElements)
	{
	  if (node == null || current == null)
		return false;
	  RegionDirective currentRegion = current.ParentRegion;
	  RegionDirective nodeRegion = node.ParentRegion;
	  return BreaksRegions(node, current, blockElements, currentRegion, nodeRegion);
	}
	static bool BreaksRegions(LanguageElement node, LanguageElement current, BlockElements blockElements, RegionDirective currentRegion, RegionDirective nodeRegion)
		{
			if (node == null || current == null)
				return false;
	  if (currentRegion == null)
				return false;
	  if (IsOptionSet(blockElements, BlockElements.Region))
		return CheckOtherCodeInRegions(nodeRegion, currentRegion, current);
	  return !currentRegion.Contains(node.Range) || CheckOtherCodeInRegions(nodeRegion, currentRegion, current);
		}
		static LanguageElement ExpandToDeclarationListStart(LanguageElement node)
		{
			if (node == null)
				return node;
			if (!(node is Variable))
				return node;
			Variable variable = (Variable)node;
			if (!variable.IsInDeclarationList)
				return node;
			return variable.FirstVariable;
		}
		static LanguageElement ExpandToDeclarationListEnd(LanguageElement node)
		{
			if (node == null)
				return node;
			if (!(node is Variable))
				return node;
			Variable variable = (Variable)node;
			if (!variable.IsInDeclarationList)
				return node;
			return variable.LastVariable;
		} 
		static bool HasDirectivesBetweenNodes(LanguageElement start, LanguageElement end)
		{
			if (start == null || end == null)
				return false;
			SourceFile fileNode = start.FileNode;
	  if (fileNode == null)
		return false;
			CompilerDirectiveCollection directives = fileNode.CompilerDirectives;			
			if (directives == null || directives.Count == 0)
				return false;
			SourceRange range = new SourceRange(start.Range.End, end.Range.Start);
			range = range.LogicalRange;
			int count = directives.Count;
			for (int i = 0; i < count; i++)
			{
				CompilerDirective current = directives[i] as CompilerDirective;
				if (current != null && range.Contains(current.Range.Start))
					return true;
			}
			return false;
		}
	class ParentRegionCache
	{
	  public RegionDirective CurrentRegion;
	  public SourceRange CurrentRegionRange;
	  public RegionDirective NodeRegion;
	  public SourceRange NodeRegionRange;
	  public static ParentRegionCache Create(LanguageElement current, LanguageElement node)
	  {
		RegionDirective currentRegion = null;
		SourceRange currentRegionRange = SourceRange.Empty;
		if (current != null)
		{
		  currentRegion = current.ParentRegion;
		  currentRegionRange = currentRegion == null ? SourceRange.Empty : currentRegion.Range;
		}
		RegionDirective nodeRegion = null;
		SourceRange nodeRegionRange = SourceRange.Empty;
		if (node != null)
		{
		  nodeRegion = node.ParentRegion;
		  nodeRegionRange = nodeRegion == null ? SourceRange.Empty : nodeRegion.Range;
		}
		ParentRegionCache result = new ParentRegionCache();
		result.CurrentRegion = currentRegion;
		result.CurrentRegionRange = currentRegionRange;
		result.NodeRegion = nodeRegion;
		result.NodeRegionRange = nodeRegionRange;
		return result;
	  }
	  public void Update(LanguageElement current, LanguageElement node)
	  {
		if (current != null)
		{
		  if (!CurrentRegionRange.Contains(current.Range))
		  {
			CurrentRegion = current.ParentRegion;
			CurrentRegionRange = CurrentRegion == null ? SourceRange.Empty : CurrentRegion.Range;
		  }
		}
		if (node != null)
		{
		  if (!NodeRegionRange.Contains(node.Range))
		  {
			NodeRegion = node.ParentRegion;
			NodeRegionRange = NodeRegion == null ? SourceRange.Empty : NodeRegion.Range;
		  }
		}
	  }
	}
		static LanguageElement ExpandStartNode(LanguageElement node, BlockElements blockElements, bool includeComplitingElements, out LanguageElement target)
		{
			node = ExpandToDeclarationListStart(node);
			LanguageElement startNode = node;
			target = node;
			LanguageElement current = node;
	  ParentRegionCache regionCache = ParentRegionCache.Create(current, node);
			while (current != null)
			{
				if (current.CompletesPrevious && includeComplitingElements)
				{
					current = current.PreviousSibling;
					node = current;
					target = current;
					startNode = current;
					continue;
				}
				LanguageElement previous = current.PreviousSibling;
				if (!IsAttachedToTarget(previous, node) || HasDirectivesBetweenNodes(previous, current))
					break;
				current = previous;
		regionCache.Update(current, node);
		if (CanIncludeNode(current, blockElements) &&
		  !BreaksRegions(node, current, blockElements, regionCache.CurrentRegion, regionCache.NodeRegion))
				{
					startNode = current;
				}
		if (HasBadRegionBetweenNodes(current, blockElements))
		  break;
			}
			return startNode;
		}
		static bool HasBadRegionBetweenNodes(LanguageElement currentElement, BlockElements blockElements)
		{
			if (currentElement == null)
			{
				return false;
			}
			RegionDirective currentRegionDirective = currentElement.ParentRegion;
			if (currentRegionDirective == null)
				return false;
			if (!IsValidRegion(currentElement, currentRegionDirective, blockElements))
			{
				return true;
			}
			return false;
		}
		static LanguageElement ExpandEndNode(LanguageElement node, BlockElements blockElements, bool includeComplitingElements, out LanguageElement target)
		{
			node = ExpandToDeclarationListEnd(node);
			LanguageElement endNode = node;
			target = node;
			LanguageElement current = node;
	  ParentRegionCache regionCache = ParentRegionCache.Create(current, node);
			while (current != null)
			{
				if ((current.NextSibling != null) && current.NextSibling.CompletesPrevious && includeComplitingElements)
				{
					current = current.NextSibling;
					node = current;
					target = current;
					endNode = current;
					continue;
				}
				current = current.NextSibling;
				if (!IsAttachedToTarget(current, node))
					break;
		regionCache.Update(current, node);
				if (CanIncludeNode(current, blockElements) &&
		  !BreaksRegions(node, current, blockElements, regionCache.CurrentRegion, regionCache.NodeRegion))
					endNode = current;
			}
			return endNode;
		}
		static SourceRange GetFullBlockRange(IDocument document, LanguageElement startNode, LanguageElement endNode, BlockElements blockElements)
		{
			int startLine = startNode.StartLine;
			int startOffset = startNode.StartOffset;
			int endLine = endNode.EndLine;
			int endOffset = endNode.EndOffset;
			SourceRange range = new SourceRange(startLine, startOffset, endLine, endOffset);
			if (document == null)
				return range;
			if (IsOptionSet(blockElements, BlockElements.LeadingWhiteSpace))
				range = document.IncludeLeadingWhiteSpace(range);
			if (IsOptionSet(blockElements, BlockElements.TrailingWhiteSpace))
				range = document.IncludeTrailingWhiteSpace(range);
			if (IsOptionSet(blockElements, BlockElements.LeadingEmptyLines))
				range = document.IncludePrecedingEmptyLines(range);
			if (IsOptionSet(blockElements, BlockElements.TrailingEmptyLines))
				range = document.IncludeTrailingEmptyLines(range);
			return range;
		}
		static LanguageElement GetNext(LanguageElement startNode, LanguageElement target)
		{
			LanguageElement next = startNode.NextSibling;
			while (next != null && IsAttachedToTarget(next, target))
				next = next.NextSibling;
			if (next == null)
				next = startNode.Parent;
			return next;
		}
		static LanguageElement GetPrevious(LanguageElement startNode, LanguageElement target)
		{
			LanguageElement previous = startNode.PreviousSibling;
			while (previous != null && IsAttachedToTarget(previous, target))
				previous = previous.PreviousSibling;
			if (previous == null)
				previous = startNode.Parent;
			return previous;
		}
		static void IncludeRegion(BlockElements blockElements, LanguageElement region, LanguageElement previous, LanguageElement next, ref LanguageElement node)
		{
			while (CanIncludeNode(region, blockElements))
			{
				if (region.Range.IsEmpty)
					return;
				if (!(previous is SourceFile) && !(next is SourceFile))
				{
					if (region.Contains(previous.Range.Start) || region.Contains(previous.Range.End))
						return;
					if (region.Contains(next.Range.Start) || region.Contains(next.Range.End))
						return;
				}
				node = region;
				region = region.Parent as RegionDirective;
			}
		}
		static bool IsAttachedToTarget(LanguageElement node, LanguageElement target)
		{
			if (node == null || target == null)
				return false;
			SupportElement support = node as SupportElement;
			return support != null && support.TargetNode == target;
		}
		static BlockElements ToBlockElements(LanguageElement node)
		{
			if (node is Attribute || node is AttributeSection)
				return BlockElements.Attributes;
			if (node is XmlDocComment)
				return BlockElements.XmlDocComments;
			if (node is Comment)
				return BlockElements.SupportComments;
			if (node is RegionDirective)
				return BlockElements.Region;
			return BlockElements.None;
		}
		public static void GetFullBlockNodes(LanguageElement element, out LanguageElement startNode, out LanguageElement endNode)
		{
			GetFullBlockNodes(element, DefaultBlockElements, out startNode, out endNode);
		}
	public static void GetFullBlockNodes(LanguageElement element, BlockElements blockElements, out LanguageElement startNode, out LanguageElement endNode)
	{
	  GetFullBlockNodes(element, blockElements, true, out startNode, out endNode);
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void GetFullBlockNodes(LanguageElement element, BlockElements blockElements, bool includeComplitingElements, out LanguageElement startNode, out LanguageElement endNode)
		{
			LanguageElement startTarget;
			LanguageElement endTarget;
			startNode = ExpandStartNode(element, blockElements, includeComplitingElements, out startTarget);
			endNode = ExpandEndNode(element, blockElements, includeComplitingElements, out endTarget);
			LanguageElement previous = GetPrevious(startNode, startTarget);
			LanguageElement next = GetNext(endNode, endTarget);
			RegionDirective startRegion = startNode.ParentRegion;
			if (!IsValidRegion(element, startRegion, blockElements))
			{
				return;
			}
			RegionDirective endRegion = endNode.ParentRegion;
			IncludeRegion(blockElements, startRegion, previous, next, ref startNode);
			IncludeRegion(blockElements, endRegion, previous, next, ref endNode);
		}
		static bool IsValidRegion(LanguageElement element, RegionDirective startRegion, BlockElements blockElements)
		{
			if (!IsOptionSet(blockElements, BlockElements.WithoutUnsuitableRegions))
			{
				return true;
			}
			if (startRegion == null)
				return true;
			if (element == null)
				return false;
			string elementName = element.Name;
			string startRegionName = startRegion.Name;
			if (MatchName(elementName, startRegionName))
			{
				return true;
			}
			return false;
		}
		static bool MatchName(string elementName, string startRegionName)
		{
			if (elementName == null || startRegionName == null)
			{
				return false;
			}
			string fName = elementName.Trim();
			string sName = startRegionName.Trim();
			if (elementName == String.Empty || startRegionName == String.Empty)
			{
				return false;
			}
			fName = fName.ToLower();
			sName = sName.ToLower();
			if (fName == sName)
				return true;
			return false;
		}
		public static SourceRange GetFullBlockRange(LanguageElement element)
		{
			return GetFullBlockRange(element, DefaultBlockElements);
		}
		public static SourceRange GetFullBlockRange(LanguageElement element, BlockElements blockElements)
		{
			if (element == null)
				return SourceRange.Empty;
			LanguageElement startNode;
			LanguageElement endNode;
			GetFullBlockNodes(element, blockElements, out startNode, out endNode);
			return GetFullBlockRange(element.Document, startNode, endNode, blockElements);
		}
	}
}
