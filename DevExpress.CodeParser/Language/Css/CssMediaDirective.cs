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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class CssMediaDirective : BaseCssElement, ICssMediaDirective
  {
	StringCollection _MediaTypes;
	CssMediaQueryCollection _Queries;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssMediaDirective cssMediaDirective = source as CssMediaDirective;
	  if (cssMediaDirective == null)
		return;
	  MediaTypes.Clear();
	  for (int i = 0; i < cssMediaDirective.MediaTypes.Count; i++)
		_MediaTypes.Add(cssMediaDirective._MediaTypes[i]);
	}
	#endregion
	#region Clone
	public override BaseElement Clone()
	{
	  return Clone(ElementCloneOptions.Default);
	}
	#endregion
	#region Clone(ElementCloneOptions options)
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CssMediaDirective lClone = new CssMediaDirective();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	public void AddQuery(CssMediaQuery query)
	{
	  if (query == null)
		return;
	  if (_Queries == null)
		_Queries = new CssMediaQueryCollection();
	  _Queries.Add(query);
	  AddDetailNode(query);
	  MediaTypes.Add(query.Name);
	}
	internal void SetQueries(CssMediaQueryCollection queries)
	{
	  if (queries == null)
		return;
	  foreach (CssMediaQuery query in queries)
		AddQuery(query);
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssMediaDirective;
	  }
	}
	public StringCollection MediaTypes
	{
	  get
	  {
		if (_MediaTypes == null)
		  _MediaTypes = new StringCollection();
		return _MediaTypes;
	  }
	}
	public int MediaTypesCount
	{
	  get
	  {
		if (_MediaTypes == null)
		  return 0;
		return _MediaTypes.Count;
	  }
	}
  }
  public class CssMediaQuery : BaseCssElement
  {
	CssMediaExpressionCollection _Expressions;
	public void AddExpression(CssMediaExpression expression)
	{
	  if (expression == null)
		return;
	  AddDetailNode(expression);
	  if (_Expressions == null)
		_Expressions = new CssMediaExpressionCollection();
	  _Expressions.Add(expression);
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CssMediaQuery clone = new CssMediaQuery();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  CssMediaQuery query = source as CssMediaQuery;
	  if (query == null)
		return;
	  base.CloneDataFrom(source, options);
	  Prefix = query.Prefix;
	  _Expressions = new CssMediaExpressionCollection();
	  ParserUtils.GetClonesFromNodes(DetailNodes, query.DetailNodes, _Expressions, query._Expressions);
	}
	public string MediaType { get; set; }
	public CssMediaQueryPrefix Prefix { get; set; }
	public CssMediaExpressionCollection Expressions
	{
	  get
	  {
		return _Expressions;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get { return LanguageElementType.CssMediaQuery; }
	}
  }
  public class CssMediaExpressionCollection : NodeList, ICssMediaExpressionCollection
  {
	#region CreateInstance
	protected override NodeList CreateInstance()
	{
	  return new CssMediaExpressionCollection();
	}
	#endregion
	#region this[int index]
	public new CssMediaExpression this[int index]
	{
	  get
	  {
		return (CssMediaExpression)base[index];
	  }
	  set
	  {
		base[index] = value;
	  }
	}
	#endregion
	#region IndexOf
	int ICssMediaExpressionCollection.IndexOf(ICssMediaExpression e)
	{
	  return this.IndexOf(e);
	}
	#endregion
	#region this[int index]
	ICssMediaExpression ICssMediaExpressionCollection.this[int index]
	{
	  get
	  {
		return this[index] as ICssMediaExpression;
	  }
	}
	#endregion
  }
  public sealed class CssMediaExpression : BaseCssElement, ICssMediaExpression
  {
	CssTerm _Expression;
	public CssMediaExpression()
	{
	  MediaFeature = string.Empty;
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CssMediaExpression clone = new CssMediaExpression();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssMediaExpression expr = source as CssMediaExpression;
	  if (expr == null)
		return;
	  _Expression = ParserUtils.GetCloneFromNodes(this, expr, expr._Expression) as CssTerm;
	}
	public string MediaFeature { get; set; }
	public CssTerm Expression
	{
	  get
	  {
		return _Expression;
	  }
	  set
	  {
		ReplaceDetailNode(_Expression, value);
		_Expression = value;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get { return LanguageElementType.CssMediaExpression; }
	}
  }
  public interface ICssMediaExpression
  {
	string MediaFeature { get; }
	CssTerm Expression { get; }
  }
  public interface ICssMediaExpressionCollection : ICollection
  {
	int IndexOf(ICssMediaExpression e);
	ICssMediaExpression this[int index] { get; }
  }
  public enum CssMediaQueryPrefix : byte
  {
	None,
	Not,
	Only
  }
  public class CssMediaQueryCollection : NodeList, ICssMediaQueryCollection
  {
	#region CreateInstance
	protected override NodeList CreateInstance()
	{
	  return new CssMediaQueryCollection();
	}
	#endregion
	#region this[int index]
	public new CssMediaQuery this[int index]
	{
	  get
	  {
		return (CssMediaQuery)base[index];
	  }
	  set
	  {
		base[index] = value;
	  }
	}
	#endregion
	#region IndexOf
	int ICssMediaQueryCollection.IndexOf(ICssMediaQuery e)
	{
	  return this.IndexOf(e);
	}
	#endregion
	#region this[int index]
	ICssMediaQuery ICssMediaQueryCollection.this[int index]
	{
	  get
	  {
		return this[index] as ICssMediaQuery;
	  }
	}
	#endregion
  }
  public interface ICssMediaQueryCollection : ICollection
  {
	int IndexOf(ICssMediaQuery e);
	ICssMediaQuery this[int index] { get; }
  }
  public interface ICssMediaQuery
  {
  }
  public enum CssPseudoClassType : byte
  {
	Unknown,
	Negation,
	MatchesAny,
	Dir,
	Lang,
	Hyperlink,
	NotVisitedLink,
	VisitedLink,
	LocalLink,
	Target,
	Scope,
	Current,
	Past,
	Future,
	Active,
	Hover,
	Focus,
	Enabled,
	Disabled,
	SelectedOption,
	IndeterminateValue,
	DefaultOption,
	InRange,
	OutOfRange,
	Required,
	Optional,
	ReadOnly,
	ReadWrite,
	Root,
	Empty,
	FirstChild,
	NthChild,
	LastChild,
	NthLastChild,
	OnlyChild,
	FirstOfType,
	NthOfType,
	LastOfType,
	NthLastOfType,
	OnlyOfType,
	NthMatch,
	Column,
	NthColumn,
	NthLastColumn
  }
}
