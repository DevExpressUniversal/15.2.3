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
using System.Collections.Specialized;
using System.ComponentModel;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public interface IHasBlock
  {
	SourceRange BlockStart { get; }
	SourceRange BlockEnd { get; }
	void SetBlockStart(SourceRange blockStart);
	void SetBlockEnd(SourceRange blockEnd);
  }
  public interface ICapableBlock : IHasBlock
  {
	DelimiterBlockType BlockType { get; }
	bool HasDelimitedBlock { get; }
  }
  public enum DelimiterBlockType : byte
  {
	None,
	Brace,
	Token,
	Middle
  }
  public abstract class DelimiterCapableBlock : CodeElement, ICapableBlock
  {
	#region protected fields...
	TextRangeWrapper _BlockStart;
	TextRangeWrapper _BlockEnd;
	DelimiterBlockType _BlockType;
	#endregion
	#region DelimiterCapableBlock
	public DelimiterCapableBlock()
	{
	  _BlockType = DelimiterBlockType.None;
	}
	#endregion
	SourceRange GetBlockCodeRangeFromChildren()
	{
	  SourceRange blockCodeRange = SourceRange.Empty;
	  LanguageElement firstChild = FirstChild;
	  LanguageElement lastChild = LastChild;
	  if (HasDelimitedBlock)
	  {
		Hashtable checkedElements = new Hashtable();
		while (firstChild != null && !firstChild.StartsAfter(BlockStart.End))
		{
		  object element = checkedElements[firstChild];
		  if (element != null)
			break;
		  checkedElements.Add(firstChild, firstChild);
		  firstChild = firstChild.NextSibling;
		}
		checkedElements.Clear();
		while (lastChild != null && !lastChild.EndsBefore(BlockEnd.Start))
		{
		  object element = checkedElements[lastChild];
		  if (element != null)
			break;
		  checkedElements.Add(lastChild, lastChild);
		  lastChild = lastChild.LastChild;
		}
		if (BlockType != DelimiterBlockType.Brace && lastChild == null && firstChild != null)
		  return new SourceRange(firstChild.Range.Start, BlockEnd.Start);
	  }
	  if (firstChild != null && lastChild != null)
		blockCodeRange = new SourceRange(firstChild.Range.Start, lastChild.Range.End);
	  return blockCodeRange;
	}
	SourceRange GetBlockCodeRangeForEmptyBlock()
	{
	  if (!HasDelimitedBlock)
		return SourceRange.Empty;
	  SourceRange blockCodeRange = SourceRange.Empty;
	  if (BlockType == DelimiterBlockType.Brace)
		return new SourceRange(BlockStart.End, BlockEnd.Start);
	  else if (BlockType == DelimiterBlockType.Token)
	  {
		blockCodeRange = new SourceRange(BlockStart.Start.Line + 1, 1, BlockEnd.Start.Line, BlockEnd.Start.Offset);
		SourceRange result = AdjustBlockCodeRangeToDetailNodes(blockCodeRange);
		result = AdjustRangeToParens(result);
		return result;
	  }
	  return SourceRange.Empty;
	}
	SourceRange AdjustRangeToParens(SourceRange range)
	{
	  Method method = this as Method;
	  if (method == null || method.ParamCloseRange.IsEmpty)
		return range;
	  if (range.StartsAfter(method.ParamCloseRange))
		return range;
	  return new SourceRange(method.ParamCloseRange.End.Line + 1, 1, range.End.Line, range.End.Offset);
	}
	SourceRange AdjustBlockCodeRangeToDetailNodes(SourceRange range)
	{
	  if (DetailNodeCount == 0)
		return range;
	  NodeList nodes = DetailNodes;
	  SourceRange blockCodeRange = range;
	  for (int i = 0; i < DetailNodeCount; i++)
	  {
		LanguageElement node = nodes[i] as LanguageElement;
		if (node == null)
		  continue;
		if (blockCodeRange.IsEmpty)
		  blockCodeRange = node.Range;
		if (node.StartsBefore(BlockStart.End) || node.EndsAfter(BlockEnd.Start))
		  continue;
		SourceRange nodeRange = node.Range;
		if (blockCodeRange.Contains(nodeRange))
		  blockCodeRange.Start = new SourcePoint(nodeRange.End.Line + 1, 1);
	  }
	  return blockCodeRange;
	}
	SourceRange AdjustBlockCodeRangeToNodes(SourceRange range, IList nodes)
	{
	  SourceRange blockCodeRange = range;
	  for (int i = 0; i < nodes.Count; i++)
	  {
		LanguageElement node = nodes[i] as LanguageElement;
		if (node == null)
		  continue;
		if (blockCodeRange.IsEmpty)
		  blockCodeRange = node.Range;
		if (node.StartsBefore(BlockStart.End) || node.EndsAfter(BlockEnd.Start))
		  continue;
		SourceRange nodeRange = node.Range;
		if (blockCodeRange.Start > nodeRange.Start)
		  blockCodeRange.Start = nodeRange.Start;
		if (blockCodeRange.End < nodeRange.End)
		  blockCodeRange.End = nodeRange.End;
	  }
	  return blockCodeRange;
	}
	SourceRange AdjustBlockCodeRangeToRegions(SourceRange range)
	{
	  LanguageElement regionRoot = FileNode.RegionRootNode;
	  if (regionRoot == null)
		return range;
	  SourceRange blockCodeRange = range;
	  LanguageElementCollection regions = ParserUtils.GetSelectedNodes(regionRoot, Range);
	  blockCodeRange = AdjustBlockCodeRangeToNodes(blockCodeRange, regions);
	  return blockCodeRange;
	}
	SourceRange AdjustBlockCodeRangeToComplilerDirectives(SourceRange range)
	{
	  LanguageElement directiveRoot = FileNode.CompilerDirectiveRootNode;
	  if (directiveRoot == null)
		return range;
	  SourceRange blockCodeRange = range;
	  LanguageElementCollection direcives = ParserUtils.GetSelectedNodes(directiveRoot, Range);
	  blockCodeRange = AdjustBlockCodeRangeToNodes(blockCodeRange, direcives);
	  return blockCodeRange;
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is DelimiterCapableBlock))
		return;
	  DelimiterCapableBlock lSource = (DelimiterCapableBlock)source;
	  _BlockStart = lSource.BlockStart;
	  _BlockEnd = lSource.BlockEnd;
	  _BlockType = lSource.BlockType;
	}
	#endregion
	#region ParsePostponedTokens()
	protected virtual void ParsePostponedTokens()
	{
	  if (PostponedData == null)
		return;
	  ParserBase lParser = PostponedData.Parser;
	  if (lParser == null)
		return;
	  if (!HasUnparsedCode)
		return;
	  SourceFile lSourceFile = GetSourceFile();
	  RegionDirective lRegionContext = PostponedData.RegionContext;
	  CompilerDirective lCompilerDirectiveContext = null;
	  LanguageElement lParsed = null;
	  ParserContext lContext = new ParserContext();
	  lContext.Context = this;
	  lContext.Document = Document;
	  lContext.ParseRange = InternalRange;
	  lContext.AutoSetValues();
	  if (lRegionContext != null)
		lContext.RegionContext = lRegionContext;
	  if (lSourceFile != null && lSourceFile.CompilerDirectiveRootNode != null)
	  {
		lCompilerDirectiveContext = lSourceFile.CompilerDirectiveRootNode.GetChildAt(InternalRange.Start) as CompilerDirective;
		if (lCompilerDirectiveContext != null)
		  lContext.CompilerDirectiveContext = lCompilerDirectiveContext;
	  }
	  ISourceReader lReader = PostponedData.UnparsedCode;
	  lReader.OffsetSubStream(InternalRange.Start.Line + PostponedData.LineOffset, InternalRange.Start.Offset + PostponedData.ColumnOffset);
	  DocumentHistorySlice history = History;
	  lParsed = lParser.Parse(lContext, lReader);
	  if (lParsed != null)
	  {
		Hashtable lCheckedElements = new Hashtable();
		while (lParsed is ParentToSingleStatement && !(((ParentToSingleStatement)lParsed).HasBlock))
		{
		  object lElement = lCheckedElements[lParsed];
		  if (lElement != null) 
			break;
		  lCheckedElements.Add(lParsed, lParsed);
		  LanguageElement lastChild = lParsed.LastChild;
		  if (lastChild != null)
			lParsed.SetEnd(lastChild.EndLine, lastChild.EndOffset);
		  lParsed = lParsed.Parent;
		}
		SetHistory(history);
		if (lContext.CompilerDirectiveContext != null)
		  lContext.CompilerDirectiveContext.SetHistory(history);
		if (lContext.RegionContext != null)
		  lContext.RegionContext.SetHistory(history);
	  }
	  if (HasPostponedComments)
	  {
		CommentCollection lComments = new CommentCollection();
		lComments.AddRange(PostponedComments);
		foreach (Comment comment in lComments)
		  comment.SetHistory(History);
		PostponedComments.Clear();
		lParser.IntroduceComments(this, lComments);
	  }
	  CleanUpPostponedData();
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _BlockStart = BlockStart;
	  _BlockEnd = BlockEnd;
	}
	public virtual void PatchIt(SourceRange sRange, SourceRange eRange)
	{
	  SetBlockEnd(eRange);
	  SetBlockStart(sRange);
	  SetBlockType(DelimiterBlockType.Token);
	}
	public override void ParseOnDemandIfNeeded()
	{
	  if (!UsePostponedParsing || !HasUnparsedCode)
		return;
	  lock (this)
	  {
		if (!ParsingPostponedTokens)
		{
		  ParsingPostponedTokens = true;
		  ParsePostponedTokens();
		  ParsingPostponedTokens = false;
		}
	  }
	}
	public SourceRange GetBlockCodeRange(bool checkRegions)
	{
	  return GetBlockCodeRange(checkRegions, false);
	}
	public SourceRange GetBlockCodeRange(bool checkRegions, bool checkDirectives)
	{
	  SourceRange blockCodeRange = GetBlockCodeRangeFromChildren();
	  if (checkRegions)
		blockCodeRange = AdjustBlockCodeRangeToRegions(blockCodeRange);
	  if (checkDirectives)
		blockCodeRange = AdjustBlockCodeRangeToComplilerDirectives(blockCodeRange);
	  if (blockCodeRange.IsEmpty)
		blockCodeRange = GetBlockCodeRangeForEmptyBlock();
	  return blockCodeRange;
	}
	#region GetCyclomaticComplexity
	public override int GetCyclomaticComplexity()
	{
	  return GetChildCyclomaticComplexity();
	}
	#endregion
	#region SetBlockType(DelimiterBlockType type)
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetBlockType(DelimiterBlockType type)
	{
	  _BlockType = type;
	}
	#endregion
	#region SetBlockStart()
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetBlockStart(SourceRange range)
	{
	  ClearHistory();
	  _BlockStart = range;
	}
	#endregion
	#region SetBlockEnd()
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetBlockEnd(SourceRange range)
	{
	  ClearHistory();
	  _BlockEnd = range;
	}
	#endregion
	#region HasDelimitedBlock
	[Description("Returns true if this node owns a delimited block of code. For example, in C#, a Method body is delimited by \"{\" and \"}\" characters.")]
	[Category("Block Delimiters")]
	public bool HasDelimitedBlock
	{
	  get
	  {
		return BlockStart != SourceRange.Empty && BlockEnd != SourceRange.Empty;
	  }
	}
	#endregion
	#region BlockStart
	[Description("The position of the start delimiter of a blocked pair. In C#, this will mark the position of the \"{\" for a node which owns such a block.")]
	[Category("Block Delimiters")]
	public SourceRange BlockStart
	{
	  get
	  {
		return GetTransformedRange(_BlockStart);
	  }
	}
	#endregion
	#region BlockEnd
	[Description("The position of the end delimiter of a blocked pair. In C#, this will mark the position of the \"}\" for a node which owns such a block.")]
	[Category("Block Delimiters")]
	public SourceRange BlockEnd
	{
	  get
	  {
		return GetTransformedRange(_BlockEnd);
	  }
	}
	#endregion
	#region BlockCodeRange
	[Description("Gets the range of code inside this element.")]
	[Category("Block Delimiters")]
	public SourceRange BlockCodeRange
	{
	  get
	  {
		return GetBlockCodeRange(false);
	  }
	}
	#endregion
	[Description("Gets the range of code iside this element, including region and compiler directives")]
	[Category("Block Delimiters")]
	public SourceRange ExtendedBlockCodeRange
	{
	  get
	  {
		return GetBlockCodeRange(true, true);
	  }
	}
	#region BlockRange
	[Description("Gets block range including block delimiters.")]
	[Category("Block Delimiters")]
	public SourceRange BlockRange
	{
	  get
	  {
		if (BlockType != DelimiterBlockType.Brace)
		  return BlockCodeRange;
		else
		{
		  SourcePoint lStart = BlockStart.Start.Clone();
		  SourcePoint lEnd = BlockEnd.End.Clone();
		  return new SourceRange(lStart, lEnd);
		}
	  }
	}
	#endregion
	#region BlockType
	[Description("The type of this delimited block.")]
	[Category("Block Delimiters")]
	public DelimiterBlockType BlockType
	{
	  get
	  {
		return _BlockType;
	  }
	}
	#endregion
	SourceRange IHasBlock.BlockStart
	{
	  get
	  {
		return BlockStart;
	  }
	}
	SourceRange IHasBlock.BlockEnd
	{
	  get
	  {
		return BlockEnd;
	  }
	}
  }
}
