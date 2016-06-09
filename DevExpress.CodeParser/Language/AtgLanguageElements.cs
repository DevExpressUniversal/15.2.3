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
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum ComplexFactorType 
	{
		Error,
		Parens,
		Brackets,
		Braces
	}
	public abstract class BaseAtgElement : LanguageElement
	{
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.Unknown;
			}
		}
	}
	public abstract class NamedAtgElement : BaseAtgElement
	{
		SourceRange _NameRange = SourceRange.Empty;
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is NamedAtgElement))
		return;
	  NamedAtgElement lSource = (NamedAtgElement)source;
	  _NameRange = lSource.NameRange;
	}
		public override SourceRange NameRange
		{
			get
			{
		return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = value;		
			}
		}
	}
	public class AtgCodeGenSetting : BaseAtgElement
	{
		string _Value = String.Empty;
		public override string ToString()
		{
			return String.Format("{0} = {1}", Name, Value);
		}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgCodeGenSetting))
		return;
	  AtgCodeGenSetting lSource = (AtgCodeGenSetting)source;
	  _Value = lSource._Value;
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgCodeGenSetting lClone = new AtgCodeGenSetting();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgCodeGenSetting;
			}
		}
	}
	public class AtgParserRule : NamedAtgElement
	{
		LanguageElementCollection _Parameters = null;
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgParserRule))
		return;
	  AtgParserRule lSource = (AtgParserRule)source;
	  if (lSource._Parameters != null)
	  {
		_Parameters = new LanguageElementCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Parameters, lSource._Parameters);
		if (_Parameters.Count == 0 && lSource._Parameters.Count > 0)
		  _Parameters = lSource._Parameters.DeepClone(options);
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgParserRule lClone = new AtgParserRule();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(Name);
			if (ParametersCount > 0)
				builder.AppendFormat("<{0}>", Parameters.ToString());
			return builder.ToString();
		}
		public void AddParameter(LanguageElement parameter)
		{
			if (parameter == null)
				return;
			Parameters.Add(parameter);
			this.AddDetailNode(parameter);
		}
		public LanguageElementCollection Parameters
		{
			get
			{
				if (_Parameters == null)
					_Parameters = new LanguageElementCollection();
				return _Parameters;
			}
		}
		public int ParametersCount
		{
			get
			{
				if (_Parameters == null)
					return 0;
				else
					return _Parameters.Count;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgParserRule;
			}
		}
	}
	public class AtgParserDeclaration : BaseAtgElement
	{
		public override string ToString()
		{
			return String.Format("PARSER {0}", Name);
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgParserPartition;
			}
		}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgParserDeclaration lClone = new AtgParserDeclaration();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	}
	public class AtgResolver : BaseAtgElement
	{
		Expression _Condition = null;
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgResolver))
		return;
	  AtgResolver lSource = (AtgResolver)source;
	  if (lSource._Condition != null)
	  {
		_Condition = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Condition) as Expression;
		if (_Condition == null)
		  _Condition = lSource._Condition.Clone(options) as Expression;
	  }				
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgResolver lClone = new AtgResolver();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public Expression Condition
		{
			get
			{
				return _Condition;
			}
			set
			{
				_Condition = value;
			}
		}
		public override string ToString()
		{
			string conditionStr = String.Empty;
			if (Condition != null)
				conditionStr = Condition.ToString();
			return String.Format("IF({0})", conditionStr);
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgResolver;
			}
		}
	}
	public class AtgFactor : NamedAtgElement
	{
		bool _IsWeak = false;
		bool _IsSync = false;
		ExpressionCollection _Parameters = null;
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgFactor))
		return;
	  AtgFactor lSource = (AtgFactor)source;
	  if (lSource._Parameters != null)
	  {
		_Parameters = new ExpressionCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Parameters, lSource._Parameters);
		if (_Parameters.Count == 0 && lSource._Parameters.Count > 0)
		  _Parameters = lSource._Parameters.DeepClone(options) as ExpressionCollection;
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgFactor lClone = new AtgFactor();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public void AddParameter(Expression parameter)
		{
			if (parameter == null)
				return;
			Parameters.Add(parameter);
			this.AddDetailNode(parameter);
		}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (IsWeak)
				builder.Append(" WEAK ");
			if (IsSync)
				builder.Append(" SYNC ");
			builder.Append(Name);
			if (ParametersCount > 0)
				builder.AppendFormat("<{0}>", Parameters.ToString());
			return builder.ToString();
		}
		public ExpressionCollection Parameters
		{
			get
			{
				if (_Parameters == null)
					_Parameters = new ExpressionCollection();
				return _Parameters;
			}
		}
		public int ParametersCount
		{
			get
			{
				if (_Parameters == null)
					return 0;
				else
					return _Parameters.Count;
			}
		}
		public bool IsWeak
		{
			get
			{
				return _IsWeak;
			}
			set
			{
				_IsWeak = value;
			}
		}
		public bool IsSync
		{
			get
			{
				return _IsSync;
			}
			set
			{
				_IsSync = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgFactor;
			}
		}
	}
	public class AtgCode : BaseAtgElement
	{
		SourceRange _BlockStart;
		SourceRange _BlockEnd;
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _BlockStart = BlockStart;
	  _BlockEnd = BlockEnd;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgCode))
		return;
	  AtgCode lSource = (AtgCode)source;
	  _BlockStart = lSource.BlockStart;
	  _BlockEnd = lSource.BlockEnd;
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgCode lClone = new AtgCode();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public SourceRange BlockStart
		{
			get
			{
		return GetTransformedRange(_BlockStart);
			}
			set
			{
		ClearHistory();
				_BlockStart = value;
			}
		}
		public SourceRange BlockEnd
		{
			get
			{
		return GetTransformedRange(_BlockEnd);
			}
			set
			{
		ClearHistory();
				_BlockEnd = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgCode;
			}
		}
	}
	public class AtgComplexFactor : AtgFactor
	{
		ComplexFactorType _ComplexType = ComplexFactorType.Error;
		AtgExpression _NestedExpression = null;
		SourceRange _BlockStart;
		SourceRange _BlockEnd;
		public override string ToString()
		{
			string expressionStr = String.Empty;
			if (NestedExpression != null)
				expressionStr = NestedExpression.ToString();
			switch (ComplexType)
			{
				case ComplexFactorType.Braces:
					return String.Format("lbr {0} rbr",expressionStr);
				case ComplexFactorType.Parens:
					return String.Format("({0})", expressionStr);
				case ComplexFactorType.Brackets:
					return String.Format("[{0}]", expressionStr);
				default:
					return String.Empty;
			}
		}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgComplexFactor))
		return;
	  AtgComplexFactor lSource = (AtgComplexFactor)source;
	  _BlockStart = lSource.BlockStart;
	  _BlockEnd = lSource.BlockEnd;
	  _ComplexType = lSource._ComplexType;
	   if (lSource._NestedExpression != null)
	  {
		_NestedExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._NestedExpression) as AtgExpression;
		if (_NestedExpression == null)
		  _NestedExpression = lSource._NestedExpression.Clone(options) as AtgExpression;
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgComplexFactor lClone = new AtgComplexFactor();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _BlockStart = BlockStart;
	  _BlockEnd = BlockEnd;
	}
		public SourceRange BlockStart
		{
			get
			{
		return GetTransformedRange(_BlockStart);
			}
			set
			{
		ClearHistory();
				_BlockStart = value;
			}
		}
		public SourceRange BlockEnd
		{
			get
			{
		return GetTransformedRange(_BlockEnd);
			}
			set
			{
		ClearHistory();
				_BlockEnd = value;
			}
		}
		public ComplexFactorType ComplexType
		{
			get
			{
				return _ComplexType;
			}
			set
			{
				_ComplexType = value;
			}
		}
		public AtgExpression NestedExpression
		{
			get
			{
				return _NestedExpression;
			}
			set
			{
				_NestedExpression = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgComplexFactor;
			}
		}
	}
	public class AtgFactorCollection : NodeList
	{
		protected int Add(AtgFactor element)
		{
			return base.Add(element);
		}
		protected void AddRange(AtgFactorCollection collection)
		{
			base.AddRange(collection);
		}
		protected int IndexOf(AtgFactor element)
		{
			return base.IndexOf(element);
		}
		protected void Insert(int index, AtgFactor element)
		{
			base.Insert(index, element);
		}
		protected void Remove(AtgFactor element)
		{
			base.Remove(element);
		}
		protected AtgFactor Find(AtgFactor element)
		{
			int lIndex = IndexOf(element);
			return lIndex < 0 ? null : this[lIndex];
		}
		protected bool Contains(AtgFactor element)
		{
			return (Find(element) != null);
		}
		protected override NodeList CreateInstance()
		{
			return new AtgFactorCollection();			
		}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder(string.Empty);
			for (int i = 0; i < Count; i++)
				builder.AppendFormat("{0} ", this[i]);
			return builder.ToString();
		}
		public new AtgFactor this[int index]
		{
			get
			{
				return (AtgFactor) base[index];
			}
			set
			{
				base[index] = value;
			}
		}		
	}
	public class AtgTerminal : BaseAtgElement
	{
		AtgResolver _Resolver = null;
		AtgFactorCollection _Factors = null;
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgTerminal))
		return;
	  AtgTerminal lSource = (AtgTerminal)source;
	  if (lSource._Factors != null)
	  {
		_Factors = new AtgFactorCollection();
		ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _Factors, lSource._Factors);
		if (_Factors.Count == 0 && lSource._Factors.Count > 0)
		  _Factors = lSource._Factors.DeepClone(options) as AtgFactorCollection;
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgTerminal lClone = new AtgTerminal();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (Resolver != null)
				builder.AppendFormat("{0} ", Resolver.ToString());
			if (FactorsCount > 0)
				builder.AppendFormat(Factors.ToString());
			return builder.ToString();
		}
		public AtgResolver Resolver
		{
			get
			{
				return _Resolver;
			}
			set
			{
				_Resolver = value;
			}
		}
		public void AddFactor(AtgFactor factor)
		{
			if (factor == null)
				return;
			if (Factors.IndexOf(factor) == -1)
			{
				Factors.Add(factor);
				this.AddNode(factor);
			}
		}
		public AtgFactorCollection Factors
		{
			get
			{
				if (_Factors == null)
					_Factors = new AtgFactorCollection();
				return _Factors;
			}
		}
		public int FactorsCount
		{
			get
			{
				if (_Factors == null)
					return 0;
				else
					return _Factors.Count;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgTerminal;
			}
		}
	}
	public class AtgTerminalCollection : NodeList
	{
		protected int Add(AtgTerminal element)
		{
			return base.Add(element);
		}
		protected void AddRange(AtgTerminalCollection collection)
		{
			base.AddRange(collection);
		}
		protected int IndexOf(AtgTerminal element)
		{
			return base.IndexOf(element);
		}
		protected void Insert(int index, AtgTerminal element)
		{
			base.Insert(index, element);
		}
		protected void Remove(AtgTerminal element)
		{
			base.Remove(element);
		}
		protected AtgTerminal Find(AtgTerminal element)
		{
			int lIndex = IndexOf(element);
			return lIndex < 0 ? null : this[lIndex];
		}
		protected bool Contains(AtgTerminal element)
		{
			return (Find(element) != null);
		}
		protected override NodeList CreateInstance()
		{
			return new AtgTerminalCollection();			
		}
		public new AtgTerminal this[int index]
		{
			get
			{
				return (AtgTerminal) base[index];
			}
			set
			{
				base[index] = value;
			}
		}		
	}
	public class AtgExpression : BaseAtgElement
	{
		AtgTerminalCollection _Terminals = null;
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgExpression))
		return;
	  AtgExpression lSource = (AtgExpression)source;
	  if (lSource._Terminals != null)
	  {
		_Terminals = new AtgTerminalCollection();
		ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _Terminals, lSource._Terminals);
		if (_Terminals.Count == 0 && lSource._Terminals.Count > 0)
		  _Terminals = lSource._Terminals.DeepClone(options) as AtgTerminalCollection;
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgExpression lClone = new AtgExpression();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			if (TerminalsCount > 0)
			{
				builder.Append(Terminals[0].ToString());
				if (TerminalsCount > 1)
					for (int i = 1; i < TerminalsCount; i++)
						builder.AppendFormat("\r\n | {0}", Terminals[i].ToString());
			}
			return builder.ToString();
		}
		public void AddTerminal(AtgTerminal terminal)
		{
			if (terminal == null)
				return;
			Terminals.Add(terminal);
			this.AddNode(terminal);
		}
		public AtgTerminalCollection Terminals
		{
			get
			{
				if (_Terminals == null)
					_Terminals = new AtgTerminalCollection();
				return _Terminals;
			}
		}
		public int TerminalsCount
		{
			get
			{
				if (_Terminals == null)
					return 0;
				else
					return _Terminals.Count;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgExpression;
			}
		}
	}
	public class AtgTokenAlias : NamedAtgElement
	{
		string _AliasName = String.Empty;
		SourceRange _AliasRange = SourceRange.Empty;
	protected void SetAliasRange(SourceRange value)
	{
	  ClearHistory();
	  _AliasRange = value;
	}
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _AliasRange = AliasRange;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgTokenAlias))
		return;
	  AtgTokenAlias lSource = (AtgTokenAlias)source;
	  _AliasName = lSource._AliasName;
	  _AliasRange = lSource.AliasRange;
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgTokenAlias lClone = new AtgTokenAlias();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public void SetAlias(Token aliasToken)
		{
			if (aliasToken == null)
				return;
			_AliasName = aliasToken.Value;
	  SetAliasRange(aliasToken.Range);
		}
		public override string ToString()
		{
			return String.Format("{0} = {1}", Name, AliasName);
		}
		public string AliasName
		{
			get
			{
				return _AliasName;
			}
			set
			{
				_AliasName = value;
			}
		}
		public SourceRange AliasRange
		{
			get
			{
				return GetTransformedRange(_AliasRange);
			}
			set
			{
		SetAliasRange(value);
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgTokenAlias;
			}
		}
	}
	public class AtgTokenDeclaration : NamedAtgElement
	{
		bool _IsKeyword = false;
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AtgTokenDeclaration))
		return;
	  AtgTokenDeclaration lSource = (AtgTokenDeclaration)source;
	  _IsKeyword = lSource._IsKeyword;
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgTokenDeclaration lClone = new AtgTokenDeclaration();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			if (_IsKeyword)
				return String.Format("{0} KEYWORD", Name);
			return Name;
		}
		public bool IsKeyword
		{
			get
			{
				return _IsKeyword;
			}
			set
			{
				_IsKeyword = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgTokenDeclaration;
			}
		}
	}
	public class AtgTokensSection : BaseAtgElement
	{
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgTokensSection lClone = new AtgTokensSection();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			return "TOKENS";
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgParserPartition;
			}
		}
	}
	public class AtgTokenNameSection : BaseAtgElement
	{
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgTokenNameSection lClone = new AtgTokenNameSection();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			return "TOKENNAMES";
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgParserPartition;
			}
		}
	}
	public class AtgProductions :BaseAtgElement
	{
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgProductions lClone = new AtgProductions();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			return "PRODUCTIONS";
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgParserPartition;
			}
		}
	}
	public class AtgFrame :BaseAtgElement
	{
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AtgFrame lClone = new AtgFrame();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
		public override string ToString()
		{
			return "FRAME";
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AtgParserPartition;
			}
		}
	}
}
