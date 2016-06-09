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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class CodeElement : LanguageElement
	{
		SupportElements _SupportElements;
		#region CodeElement
		public CodeElement()
		{
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			if (_SupportElements != null)
			{
				_SupportElements.CleanUp();
				_SupportElements = null;
			}
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region TransferCommentsTo
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void TransferCommentsTo(CodeElement element)
		{
			if (_SupportElements != null)
				_SupportElements.TransferCommentsTo(element);
		}
		#endregion
		#region TransferXmlDocCommentTo
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void TransferXmlDocCommentTo(CodeElement element)
		{
			if (_SupportElements != null)
				_SupportElements.TransferXmlDocCommentTo(element);
		}
		#endregion
		#region SetDocComment
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetDocComment(XmlDocComment value)
		{
			SupportElements.DocComment = value;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int SkipStartSupportElement(LanguageElement start, SupportElement support)
		{
			if (start == null || support == null || start.OwningList == null)
				return -1;
			int lIndex = start.OwningList.IndexOf(start) - 1;
			while(lIndex >= 0) 
			{
				LanguageElement lResult = (LanguageElement)start.OwningList[lIndex];
				if(!(lResult is SupportElement) || lResult.Range.Start <= support.Range.End)
					break;
				lIndex--;
			}
			return lIndex + 1;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int SkipEndSupportElement(LanguageElement start, SupportElement support)
		{
			if (start == null || support == null || start.OwningList == null)
				return -1;
			int lIndex = start.OwningList.IndexOf(start) + 1;
			while (lIndex < start.OwningList.Count)
			{
				LanguageElement lResult = (LanguageElement)start.OwningList[lIndex];
				if(!(lResult is SupportElement) || lResult.Range.Start >= support.Range.Start)
					break;
				lIndex ++;
			}
			return lIndex;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetAttributes(LanguageElementCollection attributes)
		{
			SetAttributes((NodeList)attributes);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void SetAttributes(NodeList attributes)
		{
			SetAttributeSections(attributes);
			AddAttributeSections(attributes);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InsertSupportElement(SupportElement element, out NodeList nodeList, out int index)
		{
			GetSupportElementNodeListAndIndex(element, out nodeList, out index);
			if (nodeList != null && index >= 0)
			{
				element.SetTarget(this);
				InsertNodeToNodeListWithoutReindex(nodeList, index, element);
			}
		}
	public void AddAttributeSection(AttributeSection section)
	{
	  if (section == null)
		return;
	  NodeList sections = new NodeList();
	  sections.Add(section);
	  AddAttributeSections(sections);
	  SupportElements.AddAttributeSection(section);
			CollectAttributes(sections);
	}
		#region GetSupportElementNodeListAndIndex
		void GetSupportElementNodeListAndIndex(SupportElement element, out NodeList nodeList, out int index)
		{
			AddSupportElemetIfOwningListIsNull(element);
			switch (element.Position)
			{
				case SupportElementPosition.Before:
					nodeList = OwningList;
					index = SkipStartSupportElement(this, element);
					break;
				case SupportElementPosition.After:
					nodeList = OwningList;
					index = SkipEndSupportElement(this, element);
					break;
				case SupportElementPosition.Inside:
					LanguageElement lInsertBefore = GetElementToInsertBefore(DetailNodes, element);
					if (lInsertBefore == null)
					{
						PrepareNodeList();
						nodeList = Nodes;
						lInsertBefore = GetElementToInsertBefore(nodeList, element);
					}
					else
					{
						PrepareDetailNodeList();
						nodeList = DetailNodes;
					}
					if (lInsertBefore == null)
						index = nodeList.Count;
					else
						index = nodeList.IndexOf(lInsertBefore);
					break;
				default:
					nodeList = null;
					index = - 1;
					break;
			}
		}
		#endregion
		#region AddSupportElemetIfOwningListIsNull
		void AddSupportElemetIfOwningListIsNull(SupportElement element)
		{
			if (OwningList != null ||
				element.Position == SupportElementPosition.Inside)
				return;
			element.SetTarget(this);
		}
		#endregion
		#region CollectAttributes
		void CollectAttributes(NodeList sections)
		{
			if (sections == null)
				return;
			for (int i=0; i < sections.Count; i++)
			{
				AttributeSection lSection = sections[i] as AttributeSection;
				if (lSection != null)
					AddAttributes(lSection);
			}
		}
		#endregion
		void AddAttributes(AttributeSection section)
		{
			for (int i = 0; i < section.AttributeCollection.Count; i++)
			{
				Attribute lAttribute = (Attribute)section.AttributeCollection[i];
				lAttribute.SetTarget(this);
				SupportElements.AddAttribute(lAttribute);
			}
		}
		#region AddAttributeSections
		void AddAttributeSections(NodeList sections)
		{
			if (sections == null)
				return;
			for (int i = 0; i < sections.Count; i++)
			{
				AttributeSection lSection = sections[i] as AttributeSection;
				if (lSection != null)
					AddSupportElement(lSection);
			}
		}
		#endregion
		void SetAttributeSections(NodeList sections)
		{
			if (sections == null || sections.Count == 0)
				return;
			SupportElements.AttributeSections = sections;
			CollectAttributes(sections);
		}
		LanguageElement GetElementToInsertBefore(NodeList nodes, SupportElement support)
		{
			if (nodes == null || nodes.Count == 0 || support == null)
				return null;
	  LanguageElement last = (LanguageElement)nodes[nodes.Count - 1];
	  if (!support.StartsBefore(last))
		return null;
	  Do doElement = this as Do;
			for (int i = 0; i < nodes.Count; i++)
			{
				LanguageElement lNode = (LanguageElement)nodes[i];
		if (doElement != null && doElement.Condition == lNode)
		  continue;
				if (support.StartsBefore(lNode))
					return lNode;
			}
			return null;
		}		
		void RestoreSupportElementLinks(NodeList nodeList, NodeList sourceNodeList)
		{
			if (sourceNodeList == null)
				return;
			if (nodeList.Count != sourceNodeList.Count)
				return;
			for (int i = 0; i < sourceNodeList.Count; i++)
			{
				SupportElement lSupportElement = nodeList[i] as SupportElement;
				if (lSupportElement == null)
					continue;
				SupportElement lSourceSupportElement = (SupportElement)sourceNodeList[i];
				CodeElement lSourceTargetNode = lSourceSupportElement.TargetNode as CodeElement;
				if (lSourceTargetNode == null)
					continue;
				if (lSourceTargetNode == lSourceSupportElement.Parent)
					lSupportElement.SetTarget(this);
				else
				{
					int lIndex = sourceNodeList.IndexOf(lSourceTargetNode);
		  if (lIndex < 0 || lIndex > sourceNodeList.Count)
			return;
					CodeElement lTargetNode = (CodeElement)nodeList[lIndex];
					lSupportElement.SetTarget(lTargetNode);
					if (lSupportElement is XmlDocComment)
						lTargetNode.SetDocComment((XmlDocComment)lSupportElement);
					else if (lSupportElement is Comment)
						lTargetNode.AddComment((Comment)lSupportElement);
					else if (lSupportElement is AttributeSection)
					{
						AttributeSection lSection = (AttributeSection)lSupportElement;
			lTargetNode.SupportElements.AddAttributeSection(lSection);
						lTargetNode.AddAttributes(lSection);
					}
				}
			}
		}
		void RestoreSupportElementLinks(CodeElement source)
		{
			RestoreSupportElementLinks(DetailNodes, source.DetailNodes);
			RestoreSupportElementLinks(Nodes, source.Nodes);
		}
		void GetSupportNodes(out LanguageElement topSupportElement, out LanguageElement bottomSupportElement)
		{	  
	  ElementRangeHelper.GetFullBlockNodes(this, BlockElements.AllSupportElements, false, out topSupportElement, out bottomSupportElement);
			if (topSupportElement == this)
				topSupportElement = null;
			if (bottomSupportElement == this)
				bottomSupportElement = null;
		}
		SupportElements SupportElements
		{
			get
			{
				if (_SupportElements == null)
					_SupportElements = new SupportElements();
				return _SupportElements;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected LanguageElement GetLanguageElementLink(LanguageElement source)
		{
			if (source == null)
				return null;
			if (source.IsDetailNode)
				return DetailNodes[source.Index] as LanguageElement;
			else
				return Nodes[source.Index] as LanguageElement;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected SupportElement GetSupportElementLink(SupportElement source)
		{
			if (source == null)
				return null;
			source.SetTarget(this);
			return GetLanguageElementLink(source) as SupportElement;			
		}
		#region CloneDataFrom(BaseElement source)
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is CodeElement))
				return;			
			CodeElement lSource = (CodeElement)source;
			RestoreSupportElementLinks(lSource);
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_SupportElements != null && _SupportElements.ContainsElement(oldElement))
				_SupportElements.ReplaceElement(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region AddSupportElement
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void AddSupportElement(SupportElement element)
		{
			if (element == null)
				return;
			LanguageElement lOldParent = element.Parent;						
			if (lOldParent != null)
				lOldParent.RemoveNode(this);			
			NodeList lNodeList;
			int lIndex;
			GetSupportElementNodeListAndIndex(element, out lNodeList, out lIndex);
			if (lNodeList != null && lIndex >= 0)
			{
				element.SetTarget(this);
				InsertNodeToNodeList(lNodeList, lIndex, element);
			}			
		}
		#endregion    
		#region AddComment
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComment(Comment comment)
		{
			AddComment(comment, false);
		}
		#endregion
	public void AddComment(Comment comment, bool useSorting)
	{
	  if (comment == null)
		return;
	  if (!useSorting)
	  {
		SupportElements.AddComment(comment);
		return;
	  }
	  SourcePoint point = comment.Range.Start;
	  int index = GetNodeIndexAfter(SupportElements.InnerComments, point.Line, point.Offset);
	  if (index < 0)
		SupportElements.AddComment(comment);
	  else
		SupportElements.InsertComment(index, comment);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void RemoveComment(Comment element)
	{
	  if (element == null)
		return;
	  SupportElements.RemoveComment(element);
	}
		#region AddCommentNode
		public void AddCommentNode(Comment comment)
		{
			AddSupportElement(comment);
			Comments.Add(comment);
		}
		#endregion
	public Attribute FindAttribute(string name)
	{
	  if (AttributeCount == 0)
		return null;
	  NodeList nodes = Attributes;
	  foreach (LanguageElement node in nodes)
		  if (String.Compare(node.Name, name, StringComparison.CurrentCulture) == 0)
		  return node as Attribute;
	  return null;
	}
		public IEnumerable AllExpressions
		{
			get
			{
				return new ElementEnumerable(this, typeof(Expression), true);
			}
		}
		#region Attributes
		[Description("Gets an ArrayList of Attributes for this element.")]
		[Category("SupportElements")]
		public NodeList Attributes
		{
			get
			{
				return SupportElements.Attributes;
			}
		}
		#endregion
		#region AttributeCount
		[Description("The number of Attributes associated with this element.")]
		[Category("SupportElements")]
		public int AttributeCount
		{
			get
			{
				if (_SupportElements == null)
					return 0;
				return _SupportElements.AttributeCount;
			}
		}
		#endregion
		#region AttributeSections
		[Description("Gets an ArrayList of AttributeSections for this element.")]
		[Category("SupportElements")]
		public NodeList AttributeSections
		{
			get
			{
				return SupportElements.AttributeSections;
			}
		}
		#endregion
		#region AttributeSectionsCount
		[Description("The number of AttributeSections associated with this element.")]
		[Category("SupportElements")]
		public int AttributeSectionsCount
		{
			get
			{
				if (_SupportElements == null)
					return 0;
				return _SupportElements.AttributeSectionsCount;
			}
		}
		#endregion
		#region Comments
		[Description("A create-on-demand collection of comments bound to this element.")]
		[Category("SupportElements")]
		public CommentCollection Comments
		{
			get
			{
				ParseOnDemandIfNeeded();
				return SupportElements.InnerComments;
			}
		}
		#endregion
		#region CommentCount
		[Description("The number of comments bound to this element.")]
		[Category("SupportElements")]
		public int CommentCount
		{
			get
			{
				return _SupportElements == null ? 0 : _SupportElements.CommentCount;
			}
		}
		#endregion
		#region DocComment
		[Description("Gets xml doc comment associated with this element.")]
		[Category("SupportElements")]
		public XmlDocComment DocComment
		{
			get
			{
				if (_SupportElements == null)
					return null;
				return _SupportElements.DocComment;
			}
		}
		#endregion
		public LanguageElement FirstNode
		{
			get
			{	
				LanguageElement topSupportElement;
				LanguageElement bottomSupportElement;
				GetSupportNodes(out topSupportElement, out bottomSupportElement);
				LanguageElement topElement = FirstChild;				
				if (topSupportElement == null)
					return topElement;
				if (topElement == null || topSupportElement.Range.Start < topElement.Range.Start)
					return topSupportElement;
				return topElement;
			}
		}
		public LanguageElement LastNode
		{
			get
			{	
				LanguageElement topSupportElement;
				LanguageElement bottomSupportElement;
				GetSupportNodes(out topSupportElement, out bottomSupportElement);
				LanguageElement bottomElement = LastChild;				
				if (bottomSupportElement == null)
					return bottomElement;
				if (bottomElement == null || bottomSupportElement.Range.End > bottomElement.Range.End)
					return bottomSupportElement;
				return bottomElement;
			}
		}
	}
}
