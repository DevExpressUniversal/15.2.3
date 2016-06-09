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
  using Diagnostics;
  public abstract class DocumentElement : BaseElement
	{
		TextRange _InternalRange = TextRange.Empty;
		DocumentHistorySlice _History = null;
		NodeList _Nodes = NodeList.Empty;
		NodeList _DetailNodes = NodeList.Empty;
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal string InternalName = String.Empty;
	SourceRange GetRecoveredRange(TextRange original)
	{
	  if (History == null || original.IsEmpty)
		return original.ToSourceRange();
	  return History.TransformWithRecover(original);
	}
	void CloneChildren(DocumentElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  if (options != null && options.NeedToCloneNodes(this))
	  {
		if (source._Nodes == null || source._Nodes == NodeList.Empty)
		  _Nodes = NodeList.Empty;
		else
		  _Nodes = source._Nodes.DeepClone(options);
	  }
	  if (source._DetailNodes == null || source._DetailNodes == NodeList.Empty)
		_DetailNodes = NodeList.Empty;
	  else
		_DetailNodes = source._DetailNodes.DeepClone(options);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected virtual void CloneRegions(BaseElement source, ElementCloneOptions options)
	{ }
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected static void SetHistory(NodeList list, DocumentHistorySlice history)
	{
	  SetHistory(list, 0, history);
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected static void SetHistory(NodeList list, int index, DocumentHistorySlice history)
		{
			if (list == null)
				return;
	  int lCount = list.Count;	  
	  for (int i = index; i < lCount; i++)
			{
				DocumentElement lChildNode = (DocumentElement)list[i];
				lChildNode.SetHistory(history, true);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual SourceRange GetTransformedRange(TextRange original)
		{
			if (History == null || original.IsEmpty)
				return original.ToSourceRange();
			return History.Transform(original);
		}
	protected virtual void UpdateRanges()
	{
	  _InternalRange = InternalRange;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ClearHistory()
		{
	  if (_History == null)
		return;
	  UpdateRanges();
			_History = null;
		}
		#region SelectCode(ITextView textView, bool aEnsureVisible)
		protected void SelectCode(IDXCoreTextView textView, int startLine, int startOffset, int endLine, int endOffset, bool ensureVisible)
		{
			if (textView == null)
				return;
			textView.Select(startLine, startOffset, endLine, endOffset, ensureVisible);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void SetInnerDetailNodes(NodeList value)
		{
			if (_DetailNodes == value)
				return;
	  if (value == null)
		value = NodeList.Empty;
			_DetailNodes = value;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void SetInnerNodes(NodeList value)
		{
			if (_Nodes == value)
				return;
	  if (value == null)
		value = NodeList.Empty;
			_Nodes = value;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal void PrepareDetailNodeList()
		{
			if (_DetailNodes == NodeList.Empty)
				_DetailNodes = CreateNodeList();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal void PrepareNodeList()
		{
			if (_Nodes == NodeList.Empty)
				_Nodes = CreateNodeList();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected long GetTotalMemoryForNodeList(NodeList nodeList)
		{
			if (nodeList == null || nodeList.Count == 0)
				return 0;
			long lMemory = 0;
			for (int i = 0; i < nodeList.Count; i++)
				lMemory += ((DocumentElement)nodeList[i]).GetTotalMemory();
			return lMemory;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected long GetTotalMemoryForDetailNodes()
		{
			return GetTotalMemoryForNodeList(_DetailNodes);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected long GetTotalMemoryForNodes()
		{
			return GetTotalMemoryForNodeList(_Nodes);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			DocumentElement lSource = source as DocumentElement;
	  if (lSource == null)
		return;
			InternalName = lSource.InternalName;
	  options.BeginChildrenCloning();
	  try
	  {
		CloneChildren(lSource, options);
	  }
	  finally
	  {
		options.EndChildrenCloning();
	  }
			SetRange(lSource.InternalRange);
	  CloneRegions(lSource, options);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public long GetTotalMemory()
		{
			return GetTotalMemory(false);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public long GetTotalMemory(bool forceFullCollection)
		{
			if (forceFullCollection)
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			long lDetailNodesMemory = GetTotalMemoryForDetailNodes();
			long lNodesMemory = GetTotalMemoryForNodes();
			long lThisSize = MemoryHelper.SizeOf(this);
			return lThisSize + lDetailNodesMemory + lNodesMemory;
		}
		#region DetailNodesFollowChildNodes
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool DetailNodesFollowChildNodes() 
		{
	  if (_DetailNodes == null || _DetailNodes.Count == 0)
				return false;
	  if (_Nodes == null || _Nodes.Count == 0)
				return false;
	  DocumentElement lFirstDetailNode = (DocumentElement)_DetailNodes[0];
	  DocumentElement lFirstChildNode = (DocumentElement)_Nodes[0];
			return (lFirstDetailNode.InternalRange.Start > lFirstChildNode.InternalRange.Start);
		}
		#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetHistory(DocumentHistorySlice history)
	{
	  SetHistory(history, true);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetHistory(DocumentHistorySlice history, bool isRecursive)
		{
	  SetHistory(0, history, isRecursive);
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void SetHistory(int nodeIndex, DocumentHistorySlice history, bool isRecursive)
	{
	  if (_History == null)
		_History = history;
	  if (isRecursive)
	  {
		SetHistory(_DetailNodes, history);
		SetHistory(_Nodes, nodeIndex, history);
	  }
	}
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			if (_Nodes != NodeList.Empty)
	  {
				_Nodes.Clear();
				_Nodes = NodeList.Empty;
	  }
			if (_DetailNodes != NodeList.Empty)
			{
				_DetailNodes.Clear();
				_DetailNodes = NodeList.Empty;
			}
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			try
			{
				CleanUpOwnedReferencesForNodes(_Nodes);
				CleanUpOwnedReferencesForNodes(_DetailNodes);
				_Nodes = NodeList.Empty;
				_DetailNodes = NodeList.Empty;
			}
			catch (Exception e)
			{
				Log.SendException(String.Format("Exception has been thrown while cleaning up {0} element references.", GetType().FullName), e);
			}
		}
		#endregion
		#region PrepareToParentNodes
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void PrepareToParentNodes()
		{
		}
		#endregion
		#region GetImageIndex
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual int GetImageIndex()
		{
			return ImageIndex.DocumentElement;
		}
		#endregion
		#region Old Drawing Code
		#endregion
		#region SelectCode(ITextView textView)
		public void SelectCode(IDXCoreTextView textView)
		{
			SelectCode(textView, false );
	}
		#endregion
		#region SelectCode(ITextView textView, bool aEnsureVisible)
		public void SelectCode(IDXCoreTextView textView, bool aEnsureVisible)
		{
			SelectCode(textView, InternalRange.Start.Line, InternalRange.Start.Offset, EndLine, EndOffset, aEnsureVisible);
		}
		#endregion
		#region Contains(int line, int offset)
		public virtual bool Contains(int line, int offset)
		{
			if (line < InternalRange.Start.Line || line > EndLine)
				return false;
			if (line == InternalRange.Start.Line && offset < InternalRange.Start.Offset)
				return false;
			if (line == EndLine && offset > EndOffset)
				return false;
			return true;
		}
		#endregion
	#region Contains(SourcePoint point)
#if !STANDALONE
	public bool Contains(SourcePoint point)
	{
	  return Contains(point.Line, point.Offset);
	}
#endif
	#endregion
		#region Contains(DocumentElement aElement)
		public bool Contains(DocumentElement aElement)
		{
			if (aElement == null)
				return false;
			return Contains(aElement.StartLine, aElement.StartOffset) && Contains(aElement.EndLine, aElement.EndOffset);
		}
		#endregion
		#region Contains(SourceRange aRange)
#if !STANDALONE
	public bool Contains(SourceRange aRange)
	{
	  return (Contains(aRange.Top.Line, aRange.Top.Offset)
		&& Contains(aRange.Bottom.Line, aRange.Bottom.Offset));
	}
#endif
	#endregion
	#region ContainedIn(int startLine, int startOffset, int endLine, int endOffset)
	public bool ContainedIn(int startLine, int startOffset, int endLine, int endOffset)
	{
	  if (InternalRange.Start.Line < startLine || EndLine > endLine)
		return false;
	  if (InternalRange.Start.Line == startLine && InternalRange.Start.Offset < startOffset)
		return false;
	  if (EndLine == endLine && EndOffset > endOffset)
		return false;
	  return true;
	}
	#endregion
	#region ContainedIn(SourcePoint start, SourcePoint end)
	public bool ContainedIn(SourcePoint start, SourcePoint end)
	{
	  return ContainedIn(start.Line, start.Offset, end.Line, end.Offset);
	}
	#endregion
	#region ContainedIn(SourceRange aRange)
	public bool ContainedIn(SourceRange aRange)
	{
	  return ContainedIn(aRange.Top.Line, aRange.Top.Offset, aRange.Bottom.Line,
		aRange.Bottom.Offset);
	}
	#endregion
		#region GetDetailNodeDescription
		public virtual string GetDetailNodeDescription(int index)
		{
			return String.Empty;
		}
		#endregion
		#region StartsAfter(int lineNumber, int columnOffset)
		public bool StartsAfter(int lineNumber, int columnOffset)
		{
			SourceRange range = InternalRange;
		return range.StartsAfter(lineNumber, columnOffset);
	}
		#endregion
		#region StartsAfter(DocumentElement documentElement)
		public bool StartsAfter(DocumentElement documentElement)
		{
			if (documentElement == null)
				return false;
			return StartsAfter(documentElement.EndLine, documentElement.EndOffset);
		}
		#endregion
		#region StartsAfter(SourcePoint sourcePoint)
		public bool StartsAfter(SourcePoint sourcePoint)
		{
			return StartsAfter(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region StartsBefore(int lineNumber, int columnOffset)
		public bool StartsBefore(int lineNumber, int columnOffset)
		{
			SourceRange range = InternalRange;
			return range.StartsBefore(lineNumber, columnOffset);
		}
		#endregion
		#region StartsBefore(DocumentElement documentElement)
		public bool StartsBefore(DocumentElement documentElement)
		{
			return StartsBefore(documentElement.InternalRange.Start.Line, documentElement.InternalRange.Start.Offset);
		}
		#endregion
		#region StartsBefore(SourcePoint sourcePoint)
		public bool StartsBefore(SourcePoint sourcePoint)
		{
			return StartsBefore(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region EndsAfter(int lineNumber, int columnOffset)
		public bool EndsAfter(int lineNumber, int columnOffset)
		{
			SourceRange range = InternalRange;
		return range.EndsAfter(lineNumber, columnOffset);
	}
		#endregion
		#region EndsAfter(DocumentElement documentElement)
		public bool EndsAfter(DocumentElement documentElement)
		{
			return EndsAfter(documentElement.EndLine, documentElement.EndOffset);
		}
		#endregion
		#region EndsAfter(SourcePoint sourcePoint)
		public bool EndsAfter(SourcePoint sourcePoint)
		{
			return EndsAfter(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region EndsBefore(int lineNumber, int columnOffset)
		public bool EndsBefore(int lineNumber, int columnOffset)
		{
			SourceRange range = InternalRange;
			return range.EndsBefore(lineNumber, columnOffset);
		}
		#endregion
		#region EndsBefore(DocumentElement documentElement)
		public bool EndsBefore(DocumentElement documentElement)
		{
			return EndsBefore(documentElement.InternalRange.Start.Line, documentElement.InternalRange.Start.Offset);
		}
		#endregion
		#region EndsBefore(SourcePoint sourcePoint)
		public bool EndsBefore(SourcePoint sourcePoint)
		{
			return EndsBefore(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region SetEnd(int lineNumber, int characterOffset)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEnd(int lineNumber, int characterOffset)
		{
	  ClearHistory();
			_InternalRange.End.Set(lineNumber, characterOffset);
		}
		#endregion
		#region SetEnd(SourceRange range)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEnd(SourceRange range)
		{
			SetEnd(range.End);
		}
		#endregion
		#region SetEnd(SourcePoint point)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEnd(SourcePoint point)
		{
	  ClearHistory();
			_InternalRange.End.Set(point);
		}
		#endregion
		#region SetEnd(Token token)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEnd(Token token)
		{
	  ClearHistory();
			_InternalRange.End.Set(token);
		}
		#endregion
		#region SetEndOffset
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEndOffset(int characterOffset)
		{
	  ClearHistory();
			_InternalRange.End.SetOffset(characterOffset);
		}
		#endregion
		#region SetRange(int startLine, int startOffset, int endLine, int endOffset)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetRange(int startLine, int startOffset, int endLine, int endOffset)
		{
	  ClearHistory();
			_InternalRange.Set(startLine, startOffset, endLine, endOffset);
		}
		#endregion
		#region SetRange(SourceRange range)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetRange(SourceRange range)
		{
	  ClearHistory();
			_InternalRange.Set(range);
		}
		#endregion
		#region SetRange(SourcePoint startPoint, SourcePoint endPoint)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetRange(SourcePoint startPoint, SourcePoint endPoint)
		{
	  ClearHistory();
			_InternalRange.Set(startPoint, endPoint);
		}
		#endregion
		#region SetStart(SourceRange range)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStart(SourceRange range)
		{
			SetStart(range.Start);
		}
		#endregion
		#region SetStart(SourcePoint point)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStart(SourcePoint point)
		{
	  ClearHistory();
			_InternalRange.Start.Set(point);
		}
		#endregion
		#region SetStart(int lineNumber, int characterOffset)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStart(int lineNumber, int characterOffset)
		{
	  ClearHistory();
			_InternalRange.Start.Set(lineNumber, characterOffset);
		}
		#endregion
		#region SetStart(Token token)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetStart(Token token)
		{
	  ClearHistory();
			_InternalRange.Start.Set(token);
		}
		#endregion
		#region InnerNodes
		protected internal override NodeList InnerNodes
		{
			get { return _Nodes; }
		}
		#endregion
		#region InnerDetailNodes
		protected internal override NodeList InnerDetailNodes
		{
	  get { return _DetailNodes; }
		}
		#endregion		
		#region StartLine
		[Description("The line number where this language element starts.")]
	[Category("Position")]
		public int StartLine
	{
		get
		{
			return InternalRange.Start.Line;
		}
	}
		#endregion
		#region EndLine
		[Description("The line number where this language element ends.")]
		[Category("Position")]
		public virtual int EndLine
	{
		get
		{
			return InternalRange.End.Line;
		}
	}
		#endregion
		#region StartOffset
		[Description("The column offset where this language element starts.")]
		[Category("Position")]
		public int StartOffset
	{
		get
		{
			return InternalRange.Start.Offset;
		}
	}
		#endregion
		#region EndOffset
		[Description("The column offset where this language element ends.")]
		[Category("Position")]
		public virtual int EndOffset
	{
		get
		{
			return InternalRange.End.Offset;
		}
	}
		#endregion
		#region ClassName
		[Description("The name of this language element class.")]
		[Category("Description")]
		public string ClassName
		{
			get
			{
				Type lType = this.GetType();
				return lType.FullName;
			}
		}
		#endregion
		#region Name
		[Description("The name of this language element.")]
		[Category("Description")]
		public override string Name
		{
			get
			{
				return InternalName;
			}
			set
			{
				if (InternalName == value)
					return;
		InternalName = value;			
			}
		}
		#endregion
		#region Nodes
		[Description("The language elements parented by this element.")]
		[Category("Family")]
		public override NodeList Nodes
		{
			get { return InnerNodes; }
		}
		#endregion
		#region DetailNodes
		[Description("The detail language elements associated with this element.")]
		[Category("Family")]
		public override NodeList DetailNodes
		{
			get { return InnerDetailNodes; }
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual DocumentHistorySlice History
		{
			get
			{
				return _History;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SourceRange InternalRange
		{
			get
			{
				return GetTransformedRange(_InternalRange);
			}
		}
	public SourceRange RecoveredRange
	{
	  get
	  {
		return GetRecoveredRange(_InternalRange);
	  }
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsNewContext
		{
			get
			{
				return false;
			}
		}
		public virtual SourceRange NameRange
		{
			get
			{
				return SourceRange.Empty;
			}
			set {}
		}
	}
}
