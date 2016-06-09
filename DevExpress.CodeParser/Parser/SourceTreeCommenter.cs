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
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class SourceTreeCommenter
	{		
		SourceFile _SourceFile;
	DocumentHistorySlice _History;
		Hashtable NodesAndIndexes;
		IncludeDirective _FirstInclude;
	bool _UseSorting = false; 
	public SourceTreeCommenter()
	  : this(null)
	{
	}
	#region SourceTreeCommenter
		public SourceTreeCommenter(DocumentHistorySlice history)
		{
	  _History = history;
		}
		#endregion
	void AddComment(CodeElement taget, Comment comment)
	{
	  if (taget == null || comment == null)
		return;
	  taget.AddComment(comment, _UseSorting);
	}
	void AddComment(SourceFile taget, Comment comment)
	{
	  if (taget == null || comment == null)
		return;
	  taget.AddComment(comment, _UseSorting);
	}
		#region AddXmlDocCommentToStartNode
		private void AddXmlDocCommentToStartNode(XmlDocComment xmlDoc)
		{
			if (xmlDoc == null)
				return;
			SourceFile lFileNode = SourceFile;
			if (lFileNode != null)
				lFileNode.AddXmlDocComment(xmlDoc);
		}
		#endregion
		#region AddUsualCommentToStartNode
		private void AddUsualCommentToStartNode(Comment comment)
		{
			if (comment == null)
				return;
			SourceFile lFileNode = SourceFile;
			if (lFileNode != null)
		AddComment(lFileNode, comment);
		}
		#endregion
		void AddCommentToSourceFile(SourceFile fileNode, Comment comment)
		{
			AddCommentToSourceFile(fileNode, comment, null);
		}
		void AddCommentToSourceFile(SourceFile fileNode, Comment comment, LanguageElement node)
		{
			if (fileNode == null || comment == null)
				return;
			if (node != null && node.Parent == fileNode)
			{
				int index = node.Index;
				fileNode.InsertNode(index, comment);
			}
			else
			{
				NodeList nodes = fileNode.Nodes;
				if (nodes == null || nodes.Count == 0)
					fileNode.AddNode(comment);
				else
				{
					int index = nodes.Count;
					while (index > 0)
					{
						SupportElement element = nodes[index - 1] as SupportElement;
						if (element == null)
							break;
						if (comment.StartsAfter(element))
							break;
						index --;
					}
					fileNode.InsertNode(index, comment);
				}
			}
		}
		#region AddCommentsToStartNode
		private void AddCommentsToStartNode(CommentCollection comments)
		{
			if (comments == null)
				return;
			SourceFile lFileNode = SourceFile;
			if (lFileNode == null)
				return;
			for (int i = 0; i < comments.Count; i++)
			{
				Comment lComment = comments[i];
				if (lComment != null)
				{
					BindCommentToSourceFile(lFileNode, lComment);
					if (lFileNode.Range.End < lComment.Range.End)
						lFileNode.SetEnd(lComment.EndLine, lComment.EndOffset);
				}
			}
		}
		#endregion
	#region InsertSupportElement
		private void InsertSupportElement(CodeElement node, SupportElement element) 
		{
			NodeList lNodes;
			int lIndex;
			node.InsertSupportElement(element, out lNodes, out lIndex);
			if(lNodes != null && lNodes.Count != lIndex + 1)
				NodesAndIndexes[lNodes] = null;
		}
	#endregion
	#region ReindexNode
		private void ReindexNode(NodeList nodes)
		{
			LanguageElement element = null;
			for(int i = 0; i < nodes.Count; i++)
			{
				element = nodes[i] as LanguageElement;
				if (element != null)
					element.SetIndex(i);
			}
		}
	#endregion
	#region ReindexNodes
		private void ReindexNodes()
		{
			foreach(DictionaryEntry entry in NodesAndIndexes)
			{
				ReindexNode((NodeList)entry.Key);
			}
		}
	#endregion
		#region BindXmlDocCommentToNode
	private XmlDocComment BindXmlDocCommentToNode(CodeElement node, XmlDocComment xmlDoc)
		{
			XmlDocComment lXmlDoc = xmlDoc.ParseXmlNodes();
			if (lXmlDoc == null)
				lXmlDoc = xmlDoc;
			node.SetDocComment(lXmlDoc);
			InsertSupportElement(node, lXmlDoc);
			AddXmlDocCommentToStartNode(lXmlDoc);
	  return lXmlDoc;
		}
		#endregion
		#region BindUsualCommentToNode
		private void BindUsualCommentToNode(CodeElement node, Comment comment)
		{
			InsertSupportElement(node, comment);
			AddComment(node, comment);
			AddUsualCommentToStartNode(comment);
		}
		#endregion
		#region BindCommentToNode
		private void BindCommentToNode(CodeElement node, Comment comment)
		{
	  if (node == null)
		return;
	  if (comment is XmlDocComment)
		comment = BindXmlDocCommentToNode(node, comment as XmlDocComment);
	  else
		BindUsualCommentToNode(node, comment);
	  if (_History != null)
		comment.SetHistory(_History);
		}
		#endregion
		void BindCommentToSourceFile(SourceFile fileNode, Comment comment)
		{
			BindCommentToSourceFile(fileNode, comment, null);
		}
		void BindCommentToSourceFile(SourceFile fileNode, Comment comment, LanguageElement node)
		{
			if (comment == null || fileNode == null)
				return;
			if (comment is XmlDocComment)
			{
				XmlDocComment xmlDoc = comment as XmlDocComment;
				XmlDocComment parsedXmlDoc = xmlDoc.ParseXmlNodes();
				if (parsedXmlDoc == null)
					parsedXmlDoc = xmlDoc;
				AddCommentToSourceFile(fileNode, parsedXmlDoc, node);
				fileNode.AddXmlDocComment(parsedXmlDoc);
		comment = parsedXmlDoc;
			}
			else
			{
				AddCommentToSourceFile(fileNode, comment, node);
				AddComment(fileNode, comment);
			}
	  if (_History != null)
		comment.SetHistory(_History);
		}
	#region GetNextCodeElementSibling
	public CodeElement GetNextCodeElementSibling(LanguageElement node)
	{
			if (node == null)
				return null;
			NodeList list = node.OwningList;
			if (list == null)
				return null;
	  int index = list.IndexOf(node) + 1;
	  while(index < list.Count) 
	  {
		CodeElement result = list[index] as CodeElement;
		if (result != null)
		  return result;
		index ++;
	  }
	  return null;
	}
	#endregion
	#region CanStartFromComment
	private bool CanStartFromComment(LanguageElement node, Comment comment)
	{
	  if (node == null || comment == null)
		return false;
	  SourceRange commmentRange = comment.InternalRange;
	  if (node.Range.Start.Line != commmentRange.Start.Line)
		return false;
	  return node is Statement ||
		node is Method ||
		node is Property ||
		node is Class ||
		node is Namespace;
	}
	#endregion
	#region CanBindTrailingComment
	private bool CanBindTrailingComment(CodeElement node, Comment comment)
	{	  
	  if (node == null || comment == null)
		return false;	  
	  SourceRange nodeRange = node.Range;
	  SourceRange parentNodeRange = node.Parent != null ? node.Parent.Range : SourceRange.Empty;
	  SourceRange commentRange = comment.Range;
	  if (!parentNodeRange.Contains(commentRange))
		return false;
	  if (commentRange.Start.Line != nodeRange.End.Line ||
		commentRange.Start < nodeRange.End)
		return false;	  
	  LanguageElement next = GetNextCodeElementSibling(node);			
			if (next != null && commentRange.Start > next.Range.Start)
				return false;			
	  if (CanStartFromComment(next, comment))
		return false;			
			return true;				  
	}
	#endregion
	#region CouldEnterChildren
	private bool CouldEnterChildren(LanguageElement node)
	{
	  return node != null && (node.ParsingPostponedTokens || !node.HasUnparsedCode);
	}
	#endregion
	#region ContainRange
	bool ContainRange(SourceRange[] innerRanges, SourceRange range)
	{
	  if (innerRanges == null || range.IsEmpty)
		return false;
	  int count = innerRanges.Length;
	  for (int i = 0; i < count; i++)
		if (innerRanges[i].Contains(range))
		  return true;
	  return false;
	}
	#endregion
	SourcePoint GetNodesStartPoint(NodeList nodes)
	{
	  if (nodes == null || nodes.Count == 0)
		return SourcePoint.Empty;
	  SourcePoint start = SourcePoint.Empty;
	  int count = nodes.Count;
	  for (int i = 0; i < count; i++)
	  {
		if (!start.IsEmpty)
		  break;
		LanguageElement current = nodes[i] as LanguageElement;
		if (current == null)
		  continue;
		start = current.Range.Start;
	  }
	  return start;
	}
	SourcePoint GetNodesEndPoint(NodeList nodes)
	{
	  if (nodes == null || nodes.Count == 0)
		return SourcePoint.Empty;
	  SourcePoint end = SourcePoint.Empty;
	  int count = nodes.Count;
	  for (int i = count - 1; i >= 0; i--)
	  {
		if (!end.IsEmpty)
		  break;
		LanguageElement current = nodes[i] as LanguageElement;
		if (current == null)
		  continue;
		end = current.Range.End;
	  }
	  return end;
	}
	SourceRange GetRangeFromNodes(NodeList nodes)
	{
	  if (nodes == null || nodes.Count == 0)
		return SourceRange.Empty;
	  SourcePoint start = GetNodesStartPoint(nodes);
	  SourcePoint end = GetNodesEndPoint(nodes);
	  return new SourceRange(start, end);
	}
	#region GetDetailsRange
	SourceRange GetDetailsRange(LanguageElement node, bool invertedOrder)
	{
	  if (node == null)
		return SourceRange.Empty;
	  SourceRange detailsRange = GetRangeFromNodes(node.DetailNodes);
	  if (detailsRange.IsEmpty)
		return SourceRange.Empty;
	  IHasParens hasParens = node as IHasParens;
	  if (hasParens != null && !hasParens.ParensRange.IsEmpty)
		detailsRange = hasParens.ParensRange;
	  else if (!invertedOrder)
		detailsRange = new SourceRange(detailsRange.Start, new SourcePoint(detailsRange.End.Line + 1, 1));
	  if (hasParens != null && hasParens.ParensRange.IsEmpty)	  
		detailsRange = new SourceRange(node.Range.Start, detailsRange.End);
	  return detailsRange;
	}
	#endregion
	#region GetNodesRange
	SourceRange GetNodesRange(LanguageElement node, bool invertedOrder)
	{
	  if (node == null)
		return SourceRange.Empty;
	  SourceRange nodesRange = GetRangeFromNodes(node.Nodes);
	  if (nodesRange.IsEmpty)
		return SourceRange.Empty;
	  SourceRange result = new SourceRange(nodesRange.Start, new SourcePoint(nodesRange.End.Line + 1, 1));
	  DelimiterCapableBlock block = node as DelimiterCapableBlock;
	  IHasBlock hasBlock = node as IHasBlock;
	  if (block != null && (block.BlockType == DelimiterBlockType.Brace || 
		(!block.BlockStart.IsEmpty && !block.BlockEnd.IsEmpty)))
	  {
		if (block.BlockType == DelimiterBlockType.Brace)
		  result = !invertedOrder ? block.BlockRange : new SourceRange(block.BlockRange.Start, new SourcePoint(nodesRange.End.Line + 1, 1));
		else if (!block.BlockStart.IsEmpty && !block.BlockEnd.IsEmpty)
		  result = new SourceRange(block.BlockStart.End, block.BlockEnd.Start);
	  }
	  else if (hasBlock != null && !hasBlock.BlockStart.IsEmpty && !hasBlock.BlockEnd.IsEmpty)
		result = new SourceRange(hasBlock.BlockStart.End, hasBlock.BlockEnd.Start);
	  else if (!invertedOrder)
	  {
		SourceRange detailsRange = node.GetDetailNodeRange();
		IHasParens hasParens = node as IHasParens;
		if (hasParens != null && !hasParens.ParensRange.IsEmpty)
		  detailsRange = hasParens.ParensRange;
		if (!detailsRange.IsEmpty && detailsRange.StartsBefore(nodesRange.End.Line, nodesRange.End.Offset))
		  result = new SourceRange(new SourcePoint(detailsRange.End.Line + 1, 1), new SourcePoint(nodesRange.End.Line + 1, 1));
		else result = nodesRange;
	  }
	  return result;
	}
	#endregion
	#region GetInnerSourceRanges
	SourceRange[] GetInnerSourceRanges(SourceRange commonRange, SourceRange detailsRange, SourceRange nodesRange)
	{
	  detailsRange = detailsRange.LogicalRange;
	  nodesRange = nodesRange.LogicalRange;
	  if (nodesRange.Contains(detailsRange) || detailsRange.IntersectsWith(nodesRange))
	  {
		nodesRange = SourceRange.Union(detailsRange, nodesRange);
		detailsRange = SourceRange.Empty;
	  }
	  SourceRange first = detailsRange;
	  SourceRange second = nodesRange;
	  if (second.Start < first.Start)
	  {
		first = nodesRange;
		second = detailsRange;		
	  }
	  ArrayList result = new ArrayList();
	  if (commonRange.Start < first.Start)
		result.Add(new SourceRange(commonRange.Start, first.Start));
	  if (first.End < second.Start)
		result.Add(new SourceRange(first.End, second.Start));
	  if (second.End < commonRange.End)
		result.Add(new SourceRange(second.End, commonRange.End));
	  if (result.Count == 0)
		return null;
	  return (SourceRange[])result.ToArray(typeof(SourceRange));
	}
	#endregion
	#region GetInnerSourceRangesForNode
	SourceRange[] GetInnerSourceRangesForNode(LanguageElement node)
	{	  
	  bool invertedOrder = node.DetailNodesFollowChildNodes();
	  SourceRange detailsRange = GetDetailsRange(node, invertedOrder);
	  SourceRange nodesRange = GetNodesRange(node, invertedOrder);
	  return GetInnerSourceRanges(node.InternalRange, detailsRange, nodesRange);
	}
	#endregion
		bool HasIncludeDirectiveInside(SourcePoint start, SourcePoint end)
		{
			if (_FirstInclude == null)
				return false;
			SourcePoint includeStart = _FirstInclude.Range.Start;
			return includeStart >= start && includeStart <= end;
		}
	#region AddPrecedingCommentsToNode
	private void AddPrecedingCommentsToNode(LanguageElement node, CommentCollection comments)
	{
	  CodeElement lElement = node as CodeElement;
	  if (lElement == null)
		return;
	  SourceRange lNodeRange = node.InternalRange;						
	  while (comments.Count > 0)
	  {
		Comment lComment = (Comment)comments[0];
		SourceRange lCommentRange = lComment.InternalRange;				
				if (lCommentRange.End <= lNodeRange.Start)
		{
					if (HasIncludeDirectiveInside(lCommentRange.End, lNodeRange.Start))
					{
						lComment.Position = SupportElementPosition.Inside;
						BindCommentToSourceFile(_SourceFile, lComment, node);
						comments.RemoveAt(0);
					}
					else
					{
						lComment.Position = SupportElementPosition.Before;
						BindCommentToNode(lElement, lComment);
						comments.RemoveAt(0);
					}
		}
		else
		  break;
	  }
	}
		#endregion    
		private bool AddCommentToNextObjectInitializerChildren(LanguageElement node, CommentCollection comments, Comment comment, int index)
		{
			if (node.Parent == null || node.Parent.ElementType != LanguageElementType.ObjectInitializerExpression)
				return false;
			CodeElement nextSibling = GetNextCodeElementSibling(node);
			if (nextSibling == null)
				return false;
			comment.Position = SupportElementPosition.Before;
			BindCommentToNode(nextSibling, comment);
	  comments.RemoveAt(index);
			return true;
		}
	#region AddTrailingComment
	private void AddTrailingComment(LanguageElement node, CommentCollection comments)
	{
	  if (node == null || comments == null)
		return;
	  if (!(node is CodeElement))
		return;
	  int lIndex = 0;
	  CodeElement lElement = node as CodeElement;
	  while (lIndex < comments.Count)
	  {
		Comment lComment = (Comment)comments[lIndex];
		if (lComment.InternalRange.Start.Line > node.InternalRange.End.Line)		
		  break;				
		if (CanBindTrailingComment(lElement, lComment))
		{
					if (!AddCommentToNextObjectInitializerChildren(lElement, comments, lComment, lIndex))
					{
						lComment.Position = SupportElementPosition.After;
						BindCommentToNode(lElement, lComment);
						comments.RemoveAt(lIndex);
					}
		}
		else lIndex ++;
	  }
	}
	#endregion    
		private bool AddCommentToFirstObjectInitializerChildren(LanguageElement node, CommentCollection comments, Comment comment, int index)
		{
			if (node.ElementType != LanguageElementType.ObjectInitializerExpression || node.DetailNodeCount <= 0)
				return false;
			CodeElement firstDetailNode = node.DetailNodes[0] as CodeElement;
			if (firstDetailNode == null)
				return false;
			comment.Position = SupportElementPosition.Before;
			BindCommentToNode(firstDetailNode, comment);
			comments.RemoveAt(index);
			return true;
		}
	#region AddInnerCommentsBeforeChildren
	void AddInnerCommentsBeforeChildren(LanguageElement node, CommentCollection comments)
	{
			if (!CouldEnterChildren(node))
				return;
	  if (node is SourceFile)
		return;
	  SourceRange[] innerRanges = null;
	  bool innerRangesWhereCalculated = false;
	  int index = 0;
	  while (index <= comments.Count - 1)
	  {		
		Comment comment = comments[index] as Comment;
		if (comment == null)
		  continue;
		SourceRange commentRange = comment.InternalRange;
		SourceRange nodeRange = node.InternalRange;
		if (commentRange.Start > nodeRange.End)
		  break;
		if (comment.Range.End < nodeRange.Start)
		{
		  index++;
		  continue;
		}
		if (!innerRangesWhereCalculated)
		{
		  innerRanges = GetInnerSourceRangesForNode(node);
		  innerRangesWhereCalculated = true;
		}
		if (innerRanges == null)
		  return;
		if (!ContainRange(innerRanges, commentRange))
		{
		  index++;
		  continue;
		}
				if (!AddCommentToFirstObjectInitializerChildren(node, comments, comment, index))
				{
					comment.Position = SupportElementPosition.Inside;
					BindCommentToNode(node as CodeElement, comment);
					comments.RemoveAt(index);
				}
	  }
	}
	#endregion
	#region AddCommentsToNodes
	private void AddCommentsToNodes(NodeList nodes, CommentCollection comments)
	{
	  for (int i = 0; i < nodes.Count; )
	  {
		LanguageElement lNode = (LanguageElement)nodes[i];
		LanguageElement lNext = null;
		if(i < nodes.Count - 1)
		  lNext = (LanguageElement)nodes[i + 1];
		if(comments.Count == 0)
		  break;
		if(lNext == null || comments[0].Range.Start <= lNext.Range.Start)
		  AddCommentsToNode(lNode, comments);
				if(lNext == null)
		  break;
		while(lNext != nodes[++i])
		  ;
	  }
	}
	#endregion
	#region AddCommentsToChildren
	void AddCommentsToChildren(LanguageElement node, CommentCollection comments)
	{
	  if (node is SupportElement)
		return;
			if (comments == null || comments.Count == 0)
				return;
			LanguageElement next = GetNextCodeElementSibling(node);
			if (next != null && comments[0].Range.Start > next.Range.Start)
				return;
	  if (CouldEnterChildren(node))
	  {
				if (node is MethodCallExpression || node is MethodCall || node is ObjectCreationExpression || !node.DetailNodesFollowChildNodes())
		{
		  AddCommentsToNodes(node.DetailNodes, comments);
		  AddCommentsToNodes(node.Nodes, comments);
		}
		else
		{
		  AddCommentsToNodes(node.Nodes, comments);
		  AddCommentsToNodes(node.DetailNodes, comments);
		}
	  }
	}
	#endregion
		#region AddInnerCommentsAfterChildren
		private void AddInnerCommentsAfterChildren(LanguageElement node, CommentCollection comments)
		{
			CodeElement lElement = node as CodeElement;
			if (lElement == null)
				return;
			while (comments.Count > 0)
			{
				Comment lComment = (Comment)comments[0];
				if (node.Contains(lComment.InternalRange))
				{
					lComment.Position = SupportElementPosition.Inside;
					BindCommentToNode(lElement, lComment);
					comments.RemoveAt(0);
				}
				else
					break;
			}
		}
		#endregion		
	#region AddTrailingCommentsToNode
	private void AddTrailingCommentsToNode(LanguageElement node, CommentCollection comments)
	{
	  if (!(node is CodeElement))
		return;
	  while (comments.Count > 0)
	  {
		Comment lComment = (Comment)comments[0];
		if (CanBindTrailingComment(node as CodeElement, lComment))
		{
						lComment.Position = SupportElementPosition.After;
						BindCommentToNode(node as CodeElement, lComment);
						comments.RemoveAt(0);
				}
		else
		  break;
	  }
	}
	#endregion    
	#region AddSourceFileLevelComments
	void AddSourceFileLevelComments(LanguageElement node, CommentCollection comments)
	{
	  if (!(node.Parent is SourceFile) || node.NextCodeSibling != null)
		return;
	  int index = 0;
	  while (index <= comments.Count - 1)
	  {		
		Comment comment = (Comment)comments[index];
		SourceRange commentRange = comment.InternalRange;
		if (commentRange.Start > node.InternalRange.End)
		{
		  comment.Position = SupportElementPosition.After;
		  BindCommentToNode((CodeElement)node, comment);
		  comments.RemoveAt(index);  
		}
		else index++;		
	  }
	}
	#endregion
	#region GetHasParensParent
	LanguageElement GetHasParensParent(LanguageElement node)
	{
	  if (node == null)
		return null;
	  LanguageElement current = node;
	  while (true)
	  {
		LanguageElement parent = current.Parent;
		if (parent == null ||
		  MemberFilter.IsMember(parent) || 
		  TypeDeclarationFilter.IsTypeDeclaration(parent))
		  break;		
		if (parent is IHasParens)
		  return parent;
		current = parent;
	  }
	  return null;
	}
	#endregion
	#region OutOfParentParens
	bool OutOfParentParens(LanguageElement node, Comment comment)
	{	  
	  if (node == null || comment == null)
		return false;
	  SourceRange commentRange = comment.InternalRange;
	  LanguageElement parent = node.Parent; 
	  if (parent == null || !(parent is IHasParens))
		return false;
	  return parent.HasInDetails(node) && 
		!((IHasParens)parent).ParensRange.Contains(commentRange);		
	}
	#endregion
		#region AddCommentsToNode
		private void AddCommentsToNode(LanguageElement node, CommentCollection comments)
		{
			if (node is SupportElement || comments == null || comments.Count == 0)
				return;
			AddPrecedingCommentsToNode(node, comments);
			AddTrailingComment(node, comments);
	  AddInnerCommentsBeforeChildren(node, comments);
	  AddCommentsToChildren(node, comments);
		  AddInnerCommentsAfterChildren(node, comments);
			AddTrailingCommentsToNode(node, comments);
	  AddSourceFileLevelComments(node, comments);
		}
		#endregion
		IncludeDirective GetFirstIncludeDirective(CompilerDirectiveCollection directives)
		{
			if (directives == null)
				return null;
			foreach (CompilerDirective directive in directives)
			{
				IncludeDirective include = directive as IncludeDirective;
				if (include != null)
					return include;
			}
			return null;
		}
		#region StoreSourceFileNode
		private void StoreSourceFileNode(LanguageElement node)
		{
			if (node == null)
				return;
			if (node is SourceFile)
				_SourceFile = (SourceFile)node;
			else
			{
				VisualStudioDocument lDocument = node.GetParentDocument(); 
				if (lDocument is SourceFile) 
					_SourceFile = (SourceFile)lDocument;
			}
			if (_SourceFile != null && _SourceFile.CompilerDirectives != null)
				_FirstInclude = GetFirstIncludeDirective(_SourceFile.CompilerDirectives);
		}
		#endregion
		#region CleanUpSourceFileNode
		private void CleanUpSourceFileNode()
		{
			_SourceFile = null;
		}
		#endregion
		#region CommentNode
		public void CommentNode(LanguageElement node, CommentCollection comments)
		{
			if (node == null || comments == null)
				return;
			NodesAndIndexes = new Hashtable();
			StoreSourceFileNode(node);
			AddCommentsToNode(node, comments);
			AddCommentsToStartNode(comments);
			CleanUpSourceFileNode();
			ReindexNodes();
		}
		#endregion
		#region SourceFile
		protected SourceFile SourceFile
		{
			get
			{
				return _SourceFile;
			}
			set
			{
				if (_SourceFile == value)
					return;
				_SourceFile = value;
			}
		}
		#endregion
	public bool UseSorting
	{
	  get
	  {
		return _UseSorting;
	  }
	  set
	  {
		_UseSorting = value;
	  }
	}
	}
}
