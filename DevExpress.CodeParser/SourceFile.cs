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
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;
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
  using Diagnostics;
  using Patterns;
  public enum OptionStrict { On, Off }
  public enum OptionInfer { On, Off}
  public enum OptionExplicit { On, Off }
  public class RegionDirectiveFilter : ElementFilterBase
	{
		public override bool Apply(IElement element)
		{
			if (element == null)
				return false;
			return element.ElementType == LanguageElementType.Region;
		}
	}
	public enum SourceFileBuildAction : int
	{
		None = 0,
		Compile = 1,
		Content = 2,
		EmbeddedResource = 3,
	Shadow = 4
	}
	public class SourceFile : VisualStudioDocument,
		IAliasList, 
		ICompilerDirectives, 
		IRegions, 
		IUsingList, 
		IIncludeDirectiveList,
		ITextStrings, 
		IComments, 
		IXmlDocComments,
		IDemandObjectProxy,
		ISourceFile,
		IAspSourceFile
	{
	enum LastSimple
	{
	  IfOrRegion = 0,
	  Elif = 1,
	  Else = 2,
	  End = 3
	}
	  #region private fields...
	bool _WasParsedWithNullProject;
		DateTime? _LastWriteTime;
		long? _FileSize;
	int _ThreadHandlingCounter = 0;
		DemandObjectManager _Manager;
		CompilerDirective _CompilerDirectiveRootNode;
	RegionDirective _SimpleDirectiveHolder;
	OptionStrict? _OptionStrict = null;
	OptionInfer? _OptionInfer = null;
	OptionExplicit? _OptionExplicit = null;
		NameValueCollection _AliasList;
	RegionDirective _RegionRootNode;
	FullIfDirective _FullDirectiveRootNode;
	bool _MustCreateFullDirectiveRootNode;
	SortedList _UsingList;
		SortedList _IncludeDirectiveList;
		SortedList _AssemblyList;
		Hashtable _AliasHash;
		TextStringCollection _TextStringCollection;
		CommentCollection _AllComments;
		CommentCollection _AllXmlDocComments;
	StringCollection _IncludedScriptFiles;
	StringCollection _IncludedStyleSheetFiles;
	string _MasterPageFile;
		List<string> _MacrosCalls;
	string _ModelTypeName;
	MacroInfoCollection _MacroRanges;
	bool _ContainsPartialTypeAndShouldBeParsed;
		bool _IsOpened;
		bool _SymbolsInvalidated;
	bool _SymbolsInvalidating;
		bool _SymbolsBuilt;
		bool _HasUnparsedDocumentCode;
	bool _NeedReload;
	bool _SaveUserFormat;
	bool _IsCustomToolOutput;
		DotNetLanguageType _AspPageLanguage = DotNetLanguageType.Unknown;
		string _AspPageBaseType;
	string _CodeBehindFileName;
		LanguageElement _CodeLayer;
		private StringCollection _DeclaredNamespaces;
		Dictionary<string, object> _Data;
		SourceFileBuildAction _LoadBuildAction;
	StringCollection _FriendAssemblyNamesList;
	IDteProjectItem _CachedProjectItem;
	bool _IsLink;
		#endregion
		#region SourceFile
		public SourceFile()
			: this(String.Empty)
		{
		}
		#endregion
		#region SourceFile
		public SourceFile(string fileName)
		{
			InternalName = fileName;
			_TextStringCollection = new TextStringCollection();
			_AllXmlDocComments = new CommentCollection();
			_AllComments = new CommentCollection();
			_LoadBuildAction = SourceFileBuildAction.Compile;
	  _SimpleDirectiveHolder = new RegionDirective();
	  _MustCreateFullDirectiveRootNode = true;
			SetFilePath(fileName);
		}
		#endregion
		void SetFileSizeAndLastWriteTime()
		{
	  string fileName = InternalName;
			if (string.IsNullOrEmpty(fileName))
				return;
			DateTime lastWriteTime = DateTime.MinValue;
			long fileSize = 0;
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				if (fileInfo.Exists)
				{
					lastWriteTime = fileInfo.LastWriteTime;
					fileSize = fileInfo.Length;
				}
			}
			catch { }
			FileSize = fileSize;
			LastWriteTime = lastWriteTime;
		}
		StringCollection CloneStringCollection(StringCollection sourceCollection)
		{
			if (sourceCollection == null)
				return null;
			StringCollection clonedCollection = new StringCollection();
			for (int i = 0; i < sourceCollection.Count; i++)
		clonedCollection.Add(sourceCollection[i]);
			return clonedCollection;
		}
		object IDemandObjectProxy.Key
		{
			get
			{
				return Name;
			}
		}
	private RegionDirective CalculateRegionRootNode()
	{
	  RegionDirective resultNode = new RegionDirective();
	  RegionDirective holder = SimpleDirectiveHolder;
	  if (holder == null || holder.NodeCount == 0)
		return resultNode;
	  LanguageElement currentRootNode = resultNode;
	  for (int i = 0; i < holder.NodeCount; i++)
	  {
		LanguageElement sourceNode = holder.Nodes[i] as LanguageElement;
		LanguageElement currentDirective = sourceNode.Clone() as LanguageElement;
		switch (currentDirective.ElementType)
		{
		  case LanguageElementType.Region:
			currentRootNode.AddNode(currentDirective);
			currentRootNode = currentDirective;
			break;
		  case LanguageElementType.EndRegionDirective:
			SourcePoint endPoint = currentDirective.Range.End;
			UpdateParentRanges(currentRootNode, resultNode, endPoint);
						UpdateParentEndTokenLength(currentRootNode, currentDirective.Range);
			if (currentRootNode != resultNode)
			  currentRootNode = currentRootNode.Parent;
			break;
		}
	  }
	  if (Document != null && Document.History != null)
		resultNode.SetHistory(Document.History.Branch());
	  resultNode.SetParent(this);
	  return resultNode;
	}
	FullIfDirective CalculateFullDirectiveRootNode()
	{
	  _MustCreateFullDirectiveRootNode = false;
	  _FullDirectiveRootNode = new FullIfDirective();
	  FullDirective current = _FullDirectiveRootNode;
	  bool isSatisfied = true;
	  Stack<LastSimple> stack = new Stack<LastSimple>();
	  foreach(PreprocessorDirective directive in _SimpleDirectiveHolder.Nodes)
	  {
		if(directive == null)
		  continue;
		switch(directive.ElementType)
		{
		  case LanguageElementType.IfDirective:
			IfDirective @if = directive as IfDirective;
			if(!isSatisfied ||@if == null)
			  return null;
			FullIfDirective fullIf = new FullIfDirective();
			fullIf.IfDirective = @if;
			fullIf.Range = new SourceRange(@if.Range.Start, SourcePoint.Empty);
			isSatisfied = @if.ExpressionValue;
			stack.Push(LastSimple.IfOrRegion);
			fullIf.ParentFull = current;
			current.NestedFulls.Add(fullIf);
			current = fullIf;
			break;
		  case LanguageElementType.ElifDirective:
			FullIfDirective fullIf2 = current as FullIfDirective;
			ElifDirective @elif = directive as ElifDirective;
			if(fullIf2 == null || @elif == null || stack.Count == 0 || stack.Pop() > LastSimple.Elif)
			  return null;
			fullIf2.ElifDirectives.Add(@elif);
			isSatisfied = @elif.ExpressionValue;
			stack.Push(LastSimple.Elif);
			break;
		  case LanguageElementType.ElseDirective:
			FullIfDirective fullIf3 = current as FullIfDirective;
			ElseDirective @else = directive as ElseDirective;
			if(fullIf3 == null || @else == null || stack.Count == 0 || stack.Pop() >= LastSimple.Else)
			  return null;
			fullIf3.ElseDirective = @else;
			isSatisfied = @else.IsSatisfied;
			stack.Push(LastSimple.Else);
			break;
		  case LanguageElementType.EndifDirective:
			FullIfDirective fullIf4 = current as FullIfDirective;
			EndIfDirective endIf = directive as EndIfDirective;
			if(fullIf4 == null || fullIf4.ParentFull == null || endIf == null || stack.Count == 0)
			  return null;
			fullIf4.EndifDirective = endIf as EndIfDirective;
			fullIf4.Range = new SourceRange(fullIf4.Range.Start, endIf.Range.End);
			current = fullIf4.ParentFull;
			isSatisfied = true;
			stack.Pop();
			break;
		  case LanguageElementType.Region:
			RegionDirective region = directive as RegionDirective;
			if(!isSatisfied || region == null)
			  return null;
			FullRegionDirective fullRegion = new FullRegionDirective();
			fullRegion.RegionDirective = region;
			fullRegion.Range = new SourceRange(region.Range.Start, SourcePoint.Empty);
			isSatisfied = true;
			stack.Push(LastSimple.IfOrRegion);
			fullRegion.ParentFull = current;
			current.NestedFulls.Add(fullRegion);
			current = fullRegion;
			break;
		  case LanguageElementType.EndRegionDirective:
			FullRegionDirective fullRegion2 = current as FullRegionDirective;
			EndRegionDirective endRegion = directive as EndRegionDirective;
			if(fullRegion2 == null || fullRegion2.ParentFull == null || endRegion == null || stack.Count == 0)
			  return null;
			fullRegion2.EndRegionDirective = endRegion;
			fullRegion2.Range = new SourceRange(fullRegion2.Range.Start, endRegion.Range.End);
			current = fullRegion2.ParentFull;
			isSatisfied = true;
			stack.Pop();
			break;
		}
	  }
	  if (current != _FullDirectiveRootNode)
		return null;
	  return _FullDirectiveRootNode;
	}
		SourceFile DemandObjectData(DemandObjectManager manager)
		{
			if (manager != null)
				return (SourceFile)manager.RequestObject(this);
			return null;
		}
		NodeList DemandNodes()
		{
			DemandObjectManager manager = _Manager;
			if (manager != null)
			{
				SourceFile lData = (SourceFile)DemandObjectData(manager);
				if (lData == null)
					return base.Nodes;
				return lData.Nodes;
			}
			return base.Nodes;
		}
		public SourceFile GetParsedFile(bool bindComments)
		{
	  ParsingMode mode = ParsingMode.Simple;
	  if (bindComments)
		mode = ParsingMode.BindComments;
	  return GetParsedFile(mode);
		}
	public SourceFile GetParsedFile(ParsingMode mode)
	{
	  ISourceFileManager manager = _Manager as ISourceFileManager;
	  if (manager == null)
		return null;
	  return manager.GetParsedFile(this, mode) as SourceFile;
	}
		NameValueCollection CloneNameValueCollection(NameValueCollection source)
		{
			if (source == null)
				return null;
			NameValueCollection lResult = new NameValueCollection();
			for (int i = 0; i < source.Count; i++)
			{
				string lName = source.GetKey(i);
				string[] lValues = source.GetValues(i);
				if (lValues == null || lValues.Length == 0)
					continue;
				string lValue = lValues[0];
				lResult.Add(lName, lValue);
			}
			return lResult;
		}
		Hashtable CloneHashtable(Hashtable source)
		{
			if (source == null)
				return null;
			Hashtable result = new Hashtable();
			IDictionaryEnumerator e = source.GetEnumerator();
			while (e.MoveNext())
			{
				object key = e.Key;
				ICloneable value = e.Value as ICloneable;
				if (value != null)
					value = value.Clone() as ICloneable;
				result.Add(key, value);
			}			
			return result;
		}
		SortedList CloneSortedList(SortedList source)
		{
			if (source == null)
				return null;
			SortedList lResult = new SortedList();
			IDictionaryEnumerator lEnum = source.GetEnumerator();
			while (lEnum.MoveNext())
			{
				object lKey = lEnum.Key;
				object lValue = lEnum.Value;
				lResult.Add(lKey, lValue);
			}
			return lResult;
		}
	void SetParent(Hashtable aliasHash)
	{
	  if (aliasHash == null)
		return;
	  foreach (DictionaryEntry entry in aliasHash)
	  {
		IElementModifier modifier = entry.Value as IElementModifier;
		if (modifier != null)
		  modifier.SetParent(this);
	  }
	}
		SourceFileBuildAction GetBuildAction()
		{
	  return StructuralParserServicesHolder.StructuralParserServices.GetBuildAction(this);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
	  if (options != null)
		options.SourceFile = this;
			base.CloneDataFrom(source, options);
			if (!(source is SourceFile))
				return;
			SourceFile lSource = (SourceFile)source;
			_IsOpened = lSource._IsOpened;
			_SymbolsInvalidated = lSource._SymbolsInvalidated;
	  _SymbolsInvalidating = lSource._SymbolsInvalidating;
			_OptionStrict = lSource._OptionStrict;
			_OptionInfer = lSource._OptionInfer;
	  _OptionExplicit = lSource.OptionExplicit;
	  _WasParsedWithNullProject = lSource._WasParsedWithNullProject;
			_CodeBehindFileName = lSource._CodeBehindFileName;
			if (lSource._CompilerDirectiveRootNode != null)
			{
				_CompilerDirectiveRootNode = ParserUtils.GetCloneFromNodes(this, lSource, lSource._CompilerDirectiveRootNode) as CompilerDirective;
				if (_CompilerDirectiveRootNode == null)
					_CompilerDirectiveRootNode = lSource._CompilerDirectiveRootNode.Clone(options) as CompilerDirective;
			}
			if (lSource._SimpleDirectiveHolder != null)
			{
				_SimpleDirectiveHolder = ParserUtils.GetCloneFromNodes(this, lSource, lSource._SimpleDirectiveHolder) as RegionDirective;
				if (_SimpleDirectiveHolder == null)
					_SimpleDirectiveHolder = lSource._SimpleDirectiveHolder.Clone(options) as RegionDirective;
			}
			NameValueCollection sourceAliasList = lSource._AliasList;
			if (sourceAliasList != null && sourceAliasList.Count != 0)
				_AliasList = CloneNameValueCollection(sourceAliasList);
			Hashtable sourceAliasHash = lSource._AliasHash;
			if (sourceAliasHash != null && sourceAliasHash.Count != 0)
				_AliasHash = CloneHashtable(sourceAliasHash);
			SortedList sourceUsingList = lSource._UsingList;
			if (sourceUsingList != null && sourceUsingList.Count != 0)
				_UsingList = CloneSortedList(sourceUsingList);
			SortedList sourceIncludeDirectiveList = lSource._IncludeDirectiveList;
			if (sourceIncludeDirectiveList != null && sourceIncludeDirectiveList.Count != 0)
				_IncludeDirectiveList = CloneSortedList(sourceIncludeDirectiveList);
			StringCollection sourceIncludedScriptFiles = lSource._IncludedScriptFiles;
			if (sourceIncludedScriptFiles != null && sourceIncludedScriptFiles.Count != 0)
				_IncludedScriptFiles = CloneStringCollection(sourceIncludedScriptFiles);
	  StringCollection sourceIncludedStyleSheetFiles = lSource._IncludedStyleSheetFiles;
	  if (sourceIncludedStyleSheetFiles != null && sourceIncludedStyleSheetFiles.Count != 0)
		_IncludedStyleSheetFiles = CloneStringCollection(sourceIncludedStyleSheetFiles);
	  _MasterPageFile = lSource.MasterPageFile;
	  _ModelTypeName = lSource._ModelTypeName;
			SortedList sourceAssemblyList = lSource._AssemblyList;
			if (sourceAssemblyList != null && sourceAssemblyList.Count != 0)
				_AssemblyList = CloneSortedList(sourceAssemblyList);
			CloneMacrosCalls(lSource.MacrosCalls);
			if (lSource._TextStringCollection != null)
			{
				_TextStringCollection = new TextStringCollection();
				ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _TextStringCollection, lSource._TextStringCollection);
				if (_TextStringCollection.Count == 0 && lSource._TextStringCollection.Count > 0)
					_TextStringCollection = lSource._TextStringCollection.DeepClone(options) as TextStringCollection;
			}
			if ((_AllComments == null || _AllComments.Count == 0) && lSource._AllComments != null)
			{
				_AllComments = new CommentCollection();
				ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _AllComments, lSource._AllComments);
				if (_AllComments.Count == 0 && lSource._AllComments.Count > 0)
					_AllComments = lSource._AllComments.DeepClone(options) as CommentCollection;
			}
			if ((_AllXmlDocComments == null || _AllXmlDocComments.Count == 0) && lSource._AllXmlDocComments != null)
			{
				_AllXmlDocComments = new CommentCollection();
				ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _AllXmlDocComments, lSource._AllXmlDocComments);
				if (_AllXmlDocComments.Count == 0 && lSource._AllXmlDocComments.Count > 0)
					_AllXmlDocComments = lSource._AllXmlDocComments.DeepClone(options) as CommentCollection;
			}
			if (lSource.DeclaredNamespacesCount > 0)
				_DeclaredNamespaces = CloneStringCollection(lSource.DeclaredNamespaces);
			if (lSource.FriendAssemblyNamesCount > 0)
				_FriendAssemblyNamesList = CloneStringCollection(lSource._FriendAssemblyNamesList);
		}
		#endregion
	protected override SourceRange GetTransformedRange(TextRange original)
	{
	  if (History == null || original.IsEmpty)
		return original.ToSourceRange();
	  TextPoint end = History.TransformWithRecover(original).End;
	  return new SourceRange(1, 1, end.Line, end.Offset);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void FillFormattingCollection(FormattingParsingElementCollection data)
	{
	  Tokens = data.GetSourceFileTokens();
	}
		public virtual void LoadCache()
		{
			IProjectElement project = Project;
			if (project == null)
				return;			
			if (SymbolsBuilt)
				return;
	  try
	  {
		if (CachedProjectItem != null)
		{
		  LoadBuildAction = CachedProjectItem.GetBuildAction();
		  CachedProjectItem = null;
		}
		if ((IsOpened || HasUnparsedDocumentCode) && !NeedReload && !WasParsedWithNullProject)
		  InvalidateProjectSymbols();
		else
		{
		  StartThreadHandling();
		  try
		  {
			ClearProjectSymbols();
			BuildProjectSymbols();
		  }
		  finally
		  {
			EndThreadHandling();
		  }
		}
	  }
	  finally
	  {
	  }
		}
		#region InvalidateRange
		public override void InvalidateRange(SourceRange range)
		{
	  if (_AllXmlDocComments != null)
		ParserUtils.RemoveNodesInRange(_AllXmlDocComments, range, true);
	  if (_AllComments != null)
		ParserUtils.RemoveNodesInRange(_AllComments, range, true);
			_TextStringCollection.RemoveElementsInRange(range);
	  RemoveRegionsInRange(range);	  
			if (_CompilerDirectiveRootNode != null)
		ParserUtils.RemoveNodesInRange(_CompilerDirectiveRootNode.Nodes, range, true);   
		}
		#endregion
		#region BindAdditionalElementsToCode
		public virtual void BindAdditionalElementsToCode(IDisposableEditPointFactory textDoc)
		{
	  if (_SimpleDirectiveHolder != null && _SimpleDirectiveHolder.Nodes != null)
		foreach (LanguageElement lElement in _SimpleDirectiveHolder.Nodes)
					lElement.SetHistory(History);
			if (_CompilerDirectiveRootNode != null && _CompilerDirectiveRootNode.Nodes != null)
				foreach(LanguageElement lElement in _CompilerDirectiveRootNode.Nodes)
					lElement.SetHistory(History);
		}
		#endregion
		#region BindAdditionalElementsToCode
		public virtual void BindAdditionalElementsToCode(IDisposableEditPointFactory textDoc, SourceRange containingRange)
		{
	  if (_SimpleDirectiveHolder != null && _SimpleDirectiveHolder.Nodes != null)
		foreach (LanguageElement lElement in _SimpleDirectiveHolder.Nodes)
					if (containingRange.Contains(lElement.Range))
						lElement.SetHistory(History);
			if (_CompilerDirectiveRootNode != null && _CompilerDirectiveRootNode.Nodes != null)
				foreach(LanguageElement lElement in _CompilerDirectiveRootNode.Nodes)
					if (containingRange.Contains(lElement.Range))
						lElement.SetHistory(History);
		}
		#endregion
	public static SourceFile Parse(string code, string language)
	{
	  if (code == null)
		return null;
	  ParserBase parser = StructuralParserServicesHolder.StructuralParserServices.GetParserFromLanguageID(language);
	  if (parser == null)
		return null;
	  return parser.ParseString(code) as SourceFile;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Open()
		{
			_IsOpened = true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Close()
		{
			_IsOpened = false;
			if (HasManager)
				_Manager.ReplaceObject(this, null);
			SetDocument(null);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetName(string value)
		{
			object lOldKey = ((IDemandObjectProxy)this).Key;
			InternalName = value;
			if (value != null)
			{
				SetFilePath(value);
			}
			ClearFileName();
			if (HasManager)
				_Manager.UpdateKey(lOldKey, ((IDemandObjectProxy)this).Key);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void InvalidateProjectSymbols()
		{
			IProjectElement lProject = Project;
			if (lProject == null)
				return;
	  if (!SymbolsInvalidated)
		lProject.InvalidateProjectSymbols(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void Invalidate()
		{
			IProjectElement project = Project;
			if (project == null)
				return;
			SourceFile proxy = project.FindDiskFile(this) as SourceFile;
			if (proxy == null)
				return;
			project.SynchronizeDiskFile(proxy.Name, null);
			project.ReleaseDiskFile(proxy);
			proxy.InvalidateProjectSymbols();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void BuildProjectSymbols()
		{
			IProjectElement lProject = Project;
			if (lProject == null)
				return;
			lProject.BuildProjectSymbols(this);
		}
	void ClearProjectSymbols()
	{
	  IProjectElement lProject = Project;
	  if (lProject == null)
		return;
	  lProject.ClearProjectSymbols(this);
	}
	void CloneMacrosCalls(List<string> macrosCalls)
		{
			if (macrosCalls == null)
			{
				_MacrosCalls = null;
				return;
			}
			_MacrosCalls = new List<string>();
			_MacrosCalls.AddRange(macrosCalls);
		}
		void RemoveMacrosCalls(List<string> macroCalls, string fileName)
		{
			if (macroCalls == null || macroCalls.Count == 0 || Project == null)
				return;
			Project.RemoveFilePathsForMacroCalls(macroCalls, fileName);
		}
		public void RemoveMacrosCalls()
		{
			RemoveMacrosCalls(MacrosCalls, Name);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Synchronize(SourceFile fileNode)
		{
			SetRange(fileNode.Range);
			_UsingList = fileNode._UsingList;
			_AliasList = fileNode._AliasList;
			_AliasHash = fileNode._AliasHash;
	  _IncludeDirectiveList = fileNode._IncludeDirectiveList;
	  _IncludedScriptFiles = fileNode._IncludedScriptFiles;
	  _IncludedStyleSheetFiles = fileNode._IncludedStyleSheetFiles;
	  _MasterPageFile = fileNode._MasterPageFile;
	  _ModelTypeName = fileNode._ModelTypeName;
	  WasParsedWithNullProject = fileNode.WasParsedWithNullProject; 
	  _FriendAssemblyNamesList = fileNode._FriendAssemblyNamesList;
			CloneMacrosCalls(fileNode.MacrosCalls);
	  _RegionRootNode = null;
	  _FullDirectiveRootNode = null;
			MacroRanges.Clear();
			MacroInfoCollection macroInfo = fileNode.MacroRanges;
			if (macroInfo != null && macroInfo.Count > 0)
				MacroRanges.AddRange(macroInfo);
			HasUnparsedDocumentCode = fileNode.HasUnparsedDocumentCode;
			AspPageLanguage = fileNode.AspPageLanguage;
			AspPageBaseType = fileNode.AspPageBaseType;
	  CodeBehindFileName = fileNode.CodeBehindFileName;
			OptionStrict = fileNode.OptionStrict;
			OptionInfer = fileNode.OptionInfer;
	  OptionExplicit = fileNode.OptionExplicit;
	  fileNode.IsLink = IsLink;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public SourceFile GetSourceFileProxy()
		{
			IProjectElement project = Project;
			if (project == null)
				return this;
			return project.FindDiskFile(this) as SourceFile;
		}
		public override MemberVisibility[] GetValidVisibilities()
		{
			return new MemberVisibility[] {MemberVisibility.Public, MemberVisibility.Internal};
		}
	public void AddComment(Comment comment)
	{
	  AddComment(comment, false);
	}
		#region AddComment
	public void AddComment(Comment comment, bool useSorting)
	{
	  if (comment == null)
		return;
	  if (useSorting)
	  {
		SourcePoint point = comment.Range.Start;
		int index = GetNodeIndexAfter(InnerAllComments, point.Line, point.Offset);
		if (index < 0)
		  InnerAllComments.Add(comment);
		else
		  InnerAllComments.Insert(index, comment);
	  }
	  else
		InnerAllComments.Add(comment);
	}
		#endregion
	public void RemoveComment(Comment comment)
	{
	  if (comment == null)
		return;
	  InnerAllComments.Remove(comment);	  
	}
		#region AddXmlDocComment
		public void AddXmlDocComment(XmlDocComment xmlDoc)
		{
			if (xmlDoc == null)
				return;
			InnerAllXmlDocComments.Add(xmlDoc);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddInvalidateMacro(string name)
		{
			if (_MacrosCalls == null)
				_MacrosCalls = new List<string>();
			_MacrosCalls.Add(name);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearInvalidateMacros()
		{
			_MacrosCalls = null;
		}
	public void AddFriendAssemblyName(string name)
	{
	  if (_FriendAssemblyNamesList == null)
		_FriendAssemblyNamesList = new StringCollection();
	  if (String.IsNullOrEmpty(name) || _FriendAssemblyNamesList.Contains(name))
		return;
	  _FriendAssemblyNamesList.Add(name);
	}
	public void RemoveFriendAssemblyName(string name)
	{
	  if (_FriendAssemblyNamesList == null || !_FriendAssemblyNamesList.Contains(name))
		return;
	  _FriendAssemblyNamesList.Remove(name);
	}
	public void ClearFriendAssemblyNames()
	{
	  if (_FriendAssemblyNamesList == null)
		return;
	  _FriendAssemblyNamesList.Clear();
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			SourceFile lClone = new SourceFile();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion		
		public Expression GetAlias(string name)
		{
			if (name == null)
				return null;
			Hashtable hash = AliasHash;
			if (hash == null)
				return null;
			object obj = hash[name];
			if (obj == null)
				return null;
			return obj as Expression;
		}
		public T GetData<T>(string key)
		{
			if (!HasData(key))
				return default(T);
			return (T)_Data[key];
		}
		public void SetData<T>(string key, T data)
		{
			if (_Data == null)
				_Data = new Dictionary<string, object>();
			_Data[key] = data;
		}
		public bool HasData(string key)
		{
			return _Data != null && _Data.ContainsKey(key);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetOptionStrict(OptionStrict? optionStrict)
		{
			_OptionStrict = optionStrict;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetOptionInfer(OptionInfer? optionInfer)
		{
			_OptionInfer = optionInfer;
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetOptionExplicit(OptionExplicit? optionExplicit)
	{
	  _OptionExplicit = optionExplicit;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetMasterPageFile(string masterPageFile)
	{
	  _MasterPageFile = masterPageFile;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetModelTypeName(string modelTypeName)
	{
	  _ModelTypeName = modelTypeName;
	}
		#region InnerAllComments
		protected CommentCollection InnerAllComments
		{
			get
			{
				if (_AllComments == null)
					_AllComments = new CommentCollection();
				return _AllComments;
			}
		}
		#endregion
		#region InnerAllXmlDocComment
		protected CommentCollection InnerAllXmlDocComments
		{
			get
			{
				if (_AllXmlDocComments == null)
					_AllXmlDocComments = new CommentCollection();
				return _AllXmlDocComments;
			}
		}
		#endregion
	internal IDteProjectItem CachedProjectItem
	{
	  get { return _CachedProjectItem; }
	  set { _CachedProjectItem = value; }
	}
	#region OptionStrict
	public OptionStrict OptionStrict
	{
	  get
	  {
		if (_OptionStrict != null)
		  return _OptionStrict.Value;
		if (Project != null)
		  return Project.OptionStrict;
		return OptionStrict.Off;
	  }
	  set
	  {
		_OptionStrict = value;
	  }
	}
	#endregion
	public OptionInfer OptionInfer
	{
	  get
	  {
		if (_OptionInfer != null)
		  return _OptionInfer.Value;
				if (Project != null)
					return Project.OptionInfer;
		return OptionInfer.On;
	  }
	  set
	  {
		_OptionInfer = value;
	  }
	}
	public OptionExplicit OptionExplicit
	{
	  get
	  {
		if (_OptionExplicit != null)
		  return _OptionExplicit.Value;
		if (Project != null)
		  return Project.OptionExplicit;
		return OptionExplicit.On;
	  }
	  set
	  {
		_OptionExplicit = value;
	  }
	}
		public override NodeList Nodes
		{
			get
			{
				return DemandNodes();
			}
		}		
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.SourceFile;
			}
		}
		public bool IsOpened
		{
			get
			{
				return _IsOpened;
			}
		}
	public bool IsGenerated
	{
	  get
	  {
		if (string.IsNullOrEmpty(FilePath))
		  return false;
		string name = Path.GetFileNameWithoutExtension(FilePath);
		return !string.IsNullOrEmpty(name) && name.EndsWith(".g.i");
	  }
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SymbolsInvalidated
		{
			get
			{
				return _SymbolsInvalidated;
			}
			set
			{
				_SymbolsInvalidated = value;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool SymbolsInvalidating
	{
	  get
	  {
		return _SymbolsInvalidating;
	  }
	  set
	  {
		_SymbolsInvalidating = value;
	  }
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool SymbolsBuilt
		{
			get
			{
				return _SymbolsBuilt;
			}
			set
			{
				_SymbolsBuilt = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DateTime LastWriteTime
		{
			get
			{
		if (!_LastWriteTime.HasValue)
		  SetFileSizeAndLastWriteTime();
				return _LastWriteTime.Value;
			}
			set
			{
				_LastWriteTime = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public long FileSize
		{
			get
			{
		if (!_FileSize.HasValue)
		  SetFileSizeAndLastWriteTime();
				return _FileSize.Value;
			}
			set
			{
				_FileSize = value;
			}
		}
		public bool WasModifiedOnDisk
		{
			get
			{
				FileInfo lFileInfo = new FileInfo(Name);
		try
		{
		  if (LastWriteTime.ToString() == lFileInfo.LastWriteTime.ToString()
			&& FileSize == lFileInfo.Length)
			return false;
		  return true;
		}
		catch
		{
		  return true;
		}
			}
		}
		public IEnumerable AllNamespaces
		{
			get
			{
				return new ElementEnumerable(this, typeof(Namespace), true);
			}
		}
		public IEnumerable AllTypes
		{
			get
			{
				return new ElementEnumerable(this, typeof(TypeDeclaration), true);
			}
		}
		public bool HasUnparsedDocumentCode
		{
			get
			{
				return _HasUnparsedDocumentCode;
			}
			set
			{
				_HasUnparsedDocumentCode = value;
			}
		}
		#region MacroRanges
		public MacroInfoCollection MacroRanges
		{
			get
			{
				if (_MacroRanges == null)
					_MacroRanges = new MacroInfoCollection();
				return _MacroRanges;
			}
		}
		#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsThreadHandling
		{
			get
			{
		return _ThreadHandlingCounter > 0;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void StartThreadHandling()
	{
	  _ThreadHandlingCounter++;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void EndThreadHandling()
	{
	  _ThreadHandlingCounter--;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public DotNetLanguageType AspPageLanguage
		{
			get
			{				
				return _AspPageLanguage;
			}
			set
			{
				_AspPageLanguage = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string AspPageBaseType
		{
			get
			{
				return _AspPageBaseType;
			}
			set
			{
				_AspPageBaseType = value;
			}
		}
		#region CompilerDirectives
		public CompilerDirectiveCollection CompilerDirectives
		{
			get
			{
				CompilerDirectiveCollection compilerDirectives = new CompilerDirectiveCollection();
				if (CompilerDirectiveRootNode == null)
					return compilerDirectives;
				foreach(CompilerDirective directive in CompilerDirectiveRootNode.Nodes)		
					compilerDirectives.Add(directive);
				return compilerDirectives;
			}
		}
		#endregion
		#region CompilerDirectiveRootNode
		public CompilerDirective CompilerDirectiveRootNode
		{
			get
			{
		DemandObjectManager manager = _Manager;
			  if (manager != null)
			  {
				  SourceFile realObject = (SourceFile)DemandObjectData(manager);
		  if (realObject != null)
			return realObject.CompilerDirectiveRootNode;
			  }
				if (_CompilerDirectiveRootNode == null)
					_CompilerDirectiveRootNode = new CompilerDirective();
				return _CompilerDirectiveRootNode;
			}
		}
		#endregion
	void UpdateParentRanges(LanguageElement currentRootNode, RegionDirective resultNode, SourcePoint endPoint)
	{
	  LanguageElement current = currentRootNode;
	  while (current != null && current != resultNode)
	  {
		current.SetRange(new SourceRange(current.Range.Start, endPoint));
		current = current.Parent;
	  }
	}
		void UpdateParentEndTokenLength(LanguageElement currentRootNode, SourceRange endRange)
		{
			RegionDirective currentRegion = currentRootNode as RegionDirective;
			if (currentRegion == null || endRange.IsEmpty)
				return;
			int startOffset = endRange.Start.Offset;
			int endOffset = endRange.End.Offset;
			currentRegion.SetEndTokenLength(endOffset - startOffset);
		}   
	internal void InvalidateSimples(SourceRange range, LanguageElementCollection trailingSimples)
	{
	  int left, right;
	  if(!ParserUtils.GetBounds(SimpleDirectiveHolder.Nodes, range, out left, out right) && left == -1)
		return;
	  int count = SimpleDirectiveHolder.NodeCount;
	  for(int i = right + 1; i < count; i++)
		trailingSimples.Add(SimpleDirectiveHolder.Nodes[i]);
	  SimpleDirectiveHolder.RemoveNodesFromIndex(left);
	}
	internal void RestoreTrailingRegions(LanguageElementCollection trailingRegions)
	{
	  if (trailingRegions == null)
		return;
	  int count = trailingRegions.Count;
	  for (int i = 0; i < count; i++)
		AddRegionDirective(trailingRegions[i]);
	}
	internal void SetRegionDirectiveHolderParent(LanguageElement parent)
	{
	  SimpleDirectiveHolder.SetParent(parent);
	}
	public void AddRegionDirective(RegionDirective regionDirective)
	{
	  AddSimpleDirective(regionDirective);
	}
	public void AddEndRegionDirective(EndRegionDirective endRegionDirective)
	{
	  AddSimpleDirective(endRegionDirective);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void AddRegionDirective(LanguageElement region)
	{
	  AddSimpleDirective(region as PreprocessorDirective);
	}
	public void AddDirective(PreprocessorDirective directive)
	{
	  if(directive == null)
		return;
	  LanguageElementType type = directive.ElementType;
	  if(type != LanguageElementType.Region && type != LanguageElementType.EndRegionDirective)
		CompilerDirectiveRootNode.AddNode(directive);
	  AddSimpleDirective(directive, false);
	}
	public void AddSimpleDirective(PreprocessorDirective directive)
	{
	  AddSimpleDirective(directive, true);
	}
	public void AddSimpleDirective(PreprocessorDirective directive, bool inEach)
	{
	  if(directive == null)
		return;
	  LanguageElementType type = directive.ElementType;
	  bool isRegion = type == LanguageElementType.Region || type == LanguageElementType.EndRegionDirective;
	  if (!(isRegion || type == LanguageElementType.IfDirective || type == LanguageElementType.ElifDirective ||
		type == LanguageElementType.ElseDirective || type == LanguageElementType.EndifDirective))
		return;
	  _MustCreateFullDirectiveRootNode = true;
	  SimpleDirectiveHolder.AddNode(directive);
	  if(isRegion)
		_RegionRootNode = null;
	  else if(inEach)
		CompilerDirectiveRootNode.AddNode(directive);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void RemoveRegionDirective(LanguageElement region)
	{
	  if (region == null)
		return;
	  _RegionRootNode = null;
	  _FullDirectiveRootNode = null;
	  SimpleDirectiveHolder.RemoveNode(region);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void RemoveRegionDirectives(IList<LanguageElement> regions)
	{
	  if (regions != null)
		foreach(LanguageElement element in regions)
		  RemoveRegionDirective(element);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void RemoveRegionsInRange(SourceRange range)
	{
	  if (_SimpleDirectiveHolder == null)
		_SimpleDirectiveHolder = new RegionDirective();
	  int left, right;
	  if(ParserUtils.GetBounds(_SimpleDirectiveHolder.Nodes, range, out left, out right))
	  {
		_RegionRootNode = null;
		_FullDirectiveRootNode = null;
		_SimpleDirectiveHolder.RemoveNodesBetweenIndexes(left, right);
	  }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void RemoveRegionDirectivesFrom(int index)
	{
	  if (index < 0)
		return;
	  _RegionRootNode = null;
	  _FullDirectiveRootNode = null;
	  SimpleDirectiveHolder.RemoveNodesFromIndex(index);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void RemoveCorruptedRegions(SourceRange range)
	{
	  int left, right;
	  if(ParserUtils.GetBounds(SimpleDirectiveHolder.Nodes, range, out left, out right))
		for(int i = left; i <= right; i++)
		{
		  LanguageElement directive = SimpleDirectiveHolder.Nodes[i] as LanguageElement;
		  if(directive != null && ParserUtils.RangeIsCorrupted(directive.Range))
			SimpleDirectiveHolder.Nodes.Remove(directive);
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetHistoryForRegions(DocumentHistorySlice history)
	{
	  if (history != null && SimpleDirectiveHolder != null)
		SimpleDirectiveHolder.SetHistory(history);
	}
		public RegionDirectiveCollection Regions
		{
			get
			{
				RegionDirectiveCollection regionDirectives = new RegionDirectiveCollection();
				if (RegionRootNode == null || RegionRootNode.NodeCount == 0)
					return regionDirectives;
				foreach (LanguageElement element in RegionRootNode.Nodes)
					if (element is RegionDirective)
						regionDirectives.Add((RegionDirective)element);
				return regionDirectives;
			}
		}
		public IEnumerable AllRegions
		{
			get
			{
				return new ElementEnumerable(RegionRootNode, new RegionDirectiveFilter(), true);
			}
		}
		public RegionDirective RegionRootNode
		{
			get
			{
		if (_RegionRootNode == null)
		  _RegionRootNode = CalculateRegionRootNode();
		return _RegionRootNode;
			}
		}
	public FullIfDirective FullDirectiveRootNode
	{
	  get
	  {
		if(_MustCreateFullDirectiveRootNode)
		  _FullDirectiveRootNode = CalculateFullDirectiveRootNode();
		return _FullDirectiveRootNode;
	  }
	}
	internal RegionDirective SimpleDirectiveHolder
	{
	  get
	  {
		DemandObjectManager manager = _Manager;
		if (manager != null)
		{
		  SourceFile realObject = (SourceFile)DemandObjectData(manager);
		  if (realObject != null)
			return realObject.SimpleDirectiveHolder;
		}
				if (_SimpleDirectiveHolder == null)
					_SimpleDirectiveHolder = new RegionDirective();
		return _SimpleDirectiveHolder;
	  }
	}
		#region UsingList
		public SortedList UsingList	
		{
			get
			{
				if (_UsingList == null)
					_UsingList = new SortedList();
				return _UsingList;
			}
			set
			{
				_UsingList = value;
			}
		}
		#endregion
		#region AssemblyList
		public SortedList AssemblyList
		{
			get
			{
				if (_AssemblyList == null)
					_AssemblyList = new SortedList();
				return _AssemblyList;
			}
			set
			{
				_AssemblyList = value;
			}
		}
		#endregion
		#region IncludeDirectiveList
		public SortedList IncludeDirectiveList
		{
			get
			{
				if (_IncludeDirectiveList == null)
					_IncludeDirectiveList = new SortedList();
				return _IncludeDirectiveList;
			}
			set
			{
				_IncludeDirectiveList = value;
			}
		}
		#endregion
		#region AliasList
		public NameValueCollection AliasList
		{
			get
			{
				if (_AliasList == null)
					_AliasList = new NameValueCollection();
				return _AliasList;
			}
			set
			{
				_AliasList = value;
			}
		}
		#endregion
		#region AliasHash
		public Hashtable AliasHash
		{
			get
			{
				if (_AliasHash == null)
					_AliasHash = new Hashtable();
				return _AliasHash;
			}
			set
			{
				_AliasHash = value;
			}
		}
		#endregion
		#region TextStrings
		public TextStringCollection TextStrings
		{
			get
			{
				return _TextStringCollection;
			}
			set
			{
				_TextStringCollection = value;
			}
		}
		#endregion
		#region AllComments
		public CommentCollection AllComments
		{
			get
			{		
				ParseAllPostponedElements();
				return InnerAllComments;
			}
		}
		#endregion
		#region AllXmlDocComments
		public CommentCollection AllXmlDocComments
		{
			get
			{
				ParseAllPostponedElements();
				return InnerAllXmlDocComments;
			}
		}
		#endregion
		DemandObjectManager IDemandObjectProxy.Manager
		{
			get { return _Manager; }
			set { _Manager = value; }
		}
		bool HasManager
		{
			get	{	return _Manager != null; }
		}
		public LanguageElement Asp 
		{ 
			get 
			{
				return this;
			}
		}
		public LanguageElement Code 
		{
			get 
			{
				return _CodeLayer;
			}
			set
			{
				_CodeLayer = value;
			}
		}
	public string CodeBehindFileName
	{
	  get
	  {
		return _CodeBehindFileName;
	  }
	  set
	  {
		_CodeBehindFileName = value;
	  }
	}
		public bool IsAspFile
		{
			get
			{
				return AspPageLanguage != DotNetLanguageType.Unknown && Code != null;
			}
		}
	public bool IsHeaderFile
	{
	  get
	  {
		if (string.IsNullOrEmpty(FilePath))
		  return false;
		string extension = Path.GetExtension(FilePath);
		return extension != null && string.Compare(extension, ".h", StringComparison.CurrentCultureIgnoreCase) == 0;
	  }
	}
		public int DeclaredNamespacesCount
		{
			get	
			{ 
				if (_DeclaredNamespaces == null)
					return 0;
				return _DeclaredNamespaces.Count; 
			}
		}
		public StringCollection DeclaredNamespaces
		{
			get	
			{
				if (_DeclaredNamespaces == null)
					_DeclaredNamespaces = new StringCollection();
				return _DeclaredNamespaces; 
			}
		}
		public int LinesCount
		{
			get
			{
				return Range.LineCount;
			}
		}
	public StringCollection IncludedScriptFiles
	{
	  get 
			{
				if (_IncludedScriptFiles == null)
					_IncludedScriptFiles = new StringCollection();
				return _IncludedScriptFiles; 
			}
	}
	public StringCollection IncludedStyleSheetFiles
	{
	  get
	  {
		if (_IncludedStyleSheetFiles == null)
		  _IncludedStyleSheetFiles = new StringCollection();
		return _IncludedStyleSheetFiles;
	  }
	}
	public string MasterPageFile
	{
	  get
	  {
		return _MasterPageFile;
	  }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string ModelTypeName 
	{ 
	  get {
		return _ModelTypeName;
	  }
	}
	public virtual int FriendAssemblyNamesCount
	{
	  get
	  {
				return _FriendAssemblyNamesList != null ? _FriendAssemblyNamesList.Count : 0;
	  }
	}
	public virtual string[] FriendAssemblyNames
	{
	  get
	  {
				if (_FriendAssemblyNamesList == null)
					return new string[0];
				string[] result = new string[_FriendAssemblyNamesList.Count];
				_FriendAssemblyNamesList.CopyTo(result, 0);
				return result;
	  }
	}
		public SourceFileBuildAction BuildAction
		{
			get { return GetBuildAction(); }
		}
		public bool HasCompileBuildAction
		{
			get { return BuildAction == SourceFileBuildAction.Compile; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public List<string> MacrosCalls
		{
			get
			{
				return _MacrosCalls;
			}
			set
			{
				_MacrosCalls = value;
			}
		}
		public SourceFileBuildAction LoadBuildAction
		{
			get { return _LoadBuildAction; }
			set { _LoadBuildAction = value; }
		}
	public bool IsLink
	{
	  get { return _IsLink; }
	  set { _IsLink = value; }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool WasParsedWithNullProject
	{
	  get
	  {
		return _WasParsedWithNullProject;
	  }
	  set
	  {
		_WasParsedWithNullProject = value;
	  }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
		public bool NeedReload
		{
			get
			{
				return _NeedReload;
			}
			set
			{
				_NeedReload = value;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool ContainsPartialTypeAndShouldBeParsed
	{
	  get
	  {
		return _ContainsPartialTypeAndShouldBeParsed;
	  }
	  set
	  {
		_ContainsPartialTypeAndShouldBeParsed = value;
	  }
	}
	public bool IsCustomToolOutput
	{
	  get { return _IsCustomToolOutput; }
	  set { _IsCustomToolOutput = value; }
	}
	public bool SaveUserFormat
	{
	  get { return _SaveUserFormat; }
	  set { _SaveUserFormat = value; }
	}
	}
}
