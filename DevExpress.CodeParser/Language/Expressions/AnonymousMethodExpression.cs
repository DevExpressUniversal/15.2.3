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
	public class AnonymousMethodExpression : Expression, IHasParameters, IAnonymousMethodExpression, IHasBlock, IWithParametersModifier
	{
		LanguageElementCollection _Parameters;
		SourceRange _NameRange = SourceRange.Empty;
		SourceRange _ParamOpenRange = SourceRange.Empty;
		SourceRange _ParamCloseRange = SourceRange.Empty;
		SourceRange _BlockStart;
		SourceRange _BlockEnd;
	bool _IsAsynchronous;
	bool _ParameterListOmitted;
		public AnonymousMethodExpression()
		{
		}
	void IWithParametersModifier.AddParameter(IParameterElement parameter)
	{
	  AddParameter(parameter as Param);
	}
	void IWithParametersModifier.RemoveParameter(IParameterElement parameter)
	{
	  RemoveParameter(parameter as Param);
	}
	void IWithParametersModifier.InsertParameter(int index, IParameterElement parameter)
	{
	  InsertParameter(index, parameter as Param);
	}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			int lIdx = Parameters.IndexOf(oldElement);
			if (lIdx >= 0)
			{
				Parameters.RemoveAt(lIdx);
				if (newElement != null)
					Parameters.Insert(lIdx, newElement);
			}
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
	  base.CloneDataFrom(source, options);
			if (!(source is AnonymousMethodExpression))
				return;
			AnonymousMethodExpression lSource = (AnonymousMethodExpression)source;
			if (lSource._Parameters != null)
			{
				_Parameters = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Parameters, lSource._Parameters);
				if (_Parameters.Count == 0 && lSource._Parameters.Count > 0)
					_Parameters = lSource._Parameters.DeepClone(options);
			}		
	  _NameRange = lSource.NameRange;
			_ParamOpenRange = lSource.ParamOpenRange;
			_ParamCloseRange = lSource.ParamCloseRange;
			_BlockStart = lSource.BlockStart;
			_BlockEnd = lSource.BlockEnd;
	  _IsAsynchronous = lSource._IsAsynchronous;
	  _ParameterListOmitted = lSource._ParameterListOmitted;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	  _ParamOpenRange = ParamOpenRange;
	  _ParamCloseRange = ParamCloseRange;
	  _BlockStart = BlockStart;
	  _BlockEnd = BlockEnd;
	}
		public void SetBlockStart(SourceRange blockStart)
		{
	  ClearHistory();
			_BlockStart = blockStart;
		}
		public void SetBlockEnd(SourceRange blockEnd)
		{
	  ClearHistory();
			_BlockEnd = blockEnd;
		}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			return resolver.Resolve(this);
		}
		public override string ToString()
		{
			return "delegate";
		}
		public void AddParameters(LanguageElementCollection parameters)
		{
			if (parameters == null)
				return;
			for (int i = 0; i < parameters.Count; i++)
				AddParameter(parameters[i] as Param);
		}
		public void AddParameter(Param param)
		{
			if (param == null)
				return;
			AddDetailNode(param);
			Parameters.Add(param);
		}
		public void RemoveParameter(Param param)
		{
			if (param == null)
				return;
			RemoveDetailNode(param);
			Parameters.Remove(param);
		}
	public void InsertParameter(int index, Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Insert(index, parameter);
	  InsertDetailNode(index, parameter);
	}
		public override void CleanUpOwnedReferences()
		{
			_Parameters = null;
			base.CleanUpOwnedReferences();
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AnonymousMethodExpression lClone = new AnonymousMethodExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		private TypeReferenceExpression GetMethodType()
		{
	  return StructuralParserServicesHolder.GetMethodType(this);
		}
		public Method CreateMethod(string methodName)
		{
	  return CreateMethod(methodName, GetMethodType(), Parameters);
		}
	public Method CreateMethod(string methodName, TypeReferenceExpression methodType, LanguageElementCollection parameters)
	{
	  if (methodName == null || methodName.Length == 0 || methodType == null)
		return null;
	  string typeStr = StructuralParserServicesHolder.GenerateElement(methodType);
	  Method method = new Method(typeStr, methodName);
	  if (methodType.Name != "void")
		method.MethodType = MethodTypeEnum.Function;
	  else
		method.MethodType = MethodTypeEnum.Void;
	  method.MemberTypeReference = methodType;
	  if (parameters != null && parameters.Count > 0)
		method.SetParameters(parameters.DeepClone() as LanguageElementCollection); 
	  for (int i = 0; i < NodeCount; i++)
		method.AddNode((Nodes[i] as LanguageElement).Clone() as LanguageElement);
	  return method;
	}
	#region GetCyclomaticComplexity
	public override int GetCyclomaticComplexity()
	{
	  return 1+GetChildCyclomaticComplexity();
	}
	#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.AnonymousMethodExpression;
			}
		}
		public override bool IsNewContext
		{
			get
			{
				return true;
			}
		}
		public override bool CanContainCode
		{
			get
			{
				return true;
			}
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
		public int ParameterCount
		{
			get
			{
				if (_Parameters == null)
					return 0;
				return _Parameters.Count;
			}
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
		public SourceRange ParamOpenRange
		{
			get
			{
				return GetTransformedRange(_ParamOpenRange);
			}
			set
			{
		ClearHistory();
				_ParamOpenRange = value;
			}
		}
		public SourceRange ParamCloseRange
		{
			get
			{
				return GetTransformedRange(_ParamCloseRange);
			}
			set
			{
		ClearHistory();
				_ParamCloseRange = value;
			}
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
		#region IWithParameters Members
		IParameterElementCollection IWithParameters.Parameters
		{
			get
			{
				if (_Parameters == null)
					return EmptyLiteElements.EmptyIParameterElementCollection;
				LiteParameterElementCollection lParameters = new LiteParameterElementCollection();
				lParameters.AddRange(_Parameters);
				return lParameters;
			}
		}
		#endregion
		#region IHasBlock Members
		SourceRange IHasBlock.BlockStart
		{
			get
			{
				return _BlockStart;
			}
		}
		SourceRange IHasBlock.BlockEnd
		{
			get
			{
				return _BlockEnd;
			}
		}
		#endregion
	[Description("True if this anonimous method expression has async modifier.")]
		[Category("Access")]
		[DefaultValue(false)]
	public bool IsAsynchronous
	{
	  get
	  {
		return _IsAsynchronous;
	  }
	  set
	  {
		_IsAsynchronous = value;
	  }
	}
	public virtual bool ParameterListOmitted
	{
	  get
	  {
		return _ParameterListOmitted;
	  }
	  set
	  {
		_ParameterListOmitted = value;
	  }
	}
	}
}
