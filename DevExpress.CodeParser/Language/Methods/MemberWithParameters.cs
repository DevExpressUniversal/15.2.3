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

using System.ComponentModel;
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class MemberWithParameters : Member, IHasParens, IHasParameters, IWithParameters, IWithParametersModifier
	{
		LanguageElementCollection _Parameters;
		TextRangeWrapper _ParensRange;
		public MemberWithParameters()
		{
		}
		void AddAttributeSections()
		{
			if (_Parameters == null)
				return;
			int count = _Parameters.Count;
			for (int i = 0; i < count; i ++)
			{
				Param param = _Parameters[i] as Param;
				if (param == null)
					continue;
				NodeList attributes = param.AttributeSections;
				AddAttributeSections(param, attributes);
			}
		}
		void AddAttributeSections(Param param, NodeList nodeList)
		{
			if (param == null || nodeList == null)
				return;
			int count = nodeList.Count;
			for (int i = 0; i < count; i++)
			{
				AttributeSection section = nodeList[i] as AttributeSection;
				if (section != null)
					param.AddSupportElement(section);
			}
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
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is MemberWithParameters))
				return;			
			MemberWithParameters lSource = (MemberWithParameters)source;
			_Parameters = null;
	  if (lSource._Parameters != null)
			{
				_Parameters = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Parameters, lSource._Parameters);
				if (_Parameters.Count == 0 && lSource._Parameters.Count > 0)
					_Parameters = lSource._Parameters.DeepClone(options);
			}
			if (lSource._ParensRange != null)
				SetParensRange(lSource.ParensRange);
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _ParensRange = ParensRange;
	}
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			if (_Parameters != null)
				_Parameters.Clear();
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			int lIndex = -1;
			if (_Parameters != null)
				lIndex = _Parameters.IndexOf(oldElement);
			if (lIndex >= 0)
			{
				_Parameters.RemoveAt(lIndex);
				if (newElement != null)
					_Parameters.Insert(lIndex, newElement);
			}
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
	#region SetParameters
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParameters(LanguageElementCollection parameters)
		{
			if (_Parameters == parameters)
				return;
			_Parameters = parameters;
		}
		#endregion
		#region GetParameter(string name)
		public Param GetParameter(string name)
		{
			if (_Parameters == null)
				return null;
			int parameterCount = _Parameters.Count;
			if (parameterCount == 0)
				return null;
			for (int i = 0; i < parameterCount; i++)
			{
		Param parameter = _Parameters[i] as Param;
		if (parameter == null)
		  continue;
		if (IdentifiersMatch(parameter.Name, name))
				  return parameter;
			}
			return null;
		}
		#endregion
	#region GetParameter(int index)
	public Param GetParameter(int index)
	{
	  if (index < 0)
		return null;
	  if (_Parameters == null)
		return null;
	  if (_Parameters.Count <= index)
		return null;
	  return (_Parameters[index] as Param);
	}
	#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(Token parenOpen, Token parenClose)
		{
			SetParensRange(new SourceRange(parenOpen.Range.Start, parenClose.Range.End));			
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(SourceRange range)
		{
	  ClearHistory();
			if (range.IsEmpty)
				_ParensRange = null;
			else
				_ParensRange = range;
		}
	public void AddParameter(Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Add(parameter);
	  AddDetailNode(parameter);
	}
	public void AddParameters(LanguageElementCollection parameters)
	{
	  if (parameters == null)
		return;
	  foreach (LanguageElement parameter in parameters)
		AddParameter(parameter as Param);
	}
	public void RemoveParameter(Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Remove(parameter);
	  RemoveDetailNode(parameter);
	}
	public void RemoveParameters(LanguageElementCollection parameters)
	{
	  if (parameters == null)
		return;
	  foreach (LanguageElement parameter in parameters)
		RemoveParameter(parameter as Param);
	}
	public void InsertParameter(int index, Param parameter)
	{
	  if (parameter == null)
		return;
	  Parameters.Insert(index, parameter);
	  InsertDetailNode(index, parameter);
	}
		#region Parameters
		[Description("The parameters to this element.")]
		[Category("Details")]
		public LanguageElementCollection Parameters
		{
			get
			{
				if (_Parameters == null)
					_Parameters = new LanguageElementCollection();
				return _Parameters;
			}
		}
		#endregion
		#region ParameterCount
		[Description("The number of parameters declared by this element.")]
		[Category("Details")]
		public int ParameterCount
		{
			get
			{
				if (_Parameters == null)
					return 0;
				return _Parameters.Count;
			}
		}
		#endregion		
		public IEnumerable AllParameters
		{
			get
			{
				return new ElementEnumerable(this, typeof(Param));
			}
		}
		public SourceRange ParensRange
		{
			get
			{				
				if (_ParensRange == null)
					return SourceRange.Empty;
				return GetTransformedRange(_ParensRange);
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
	}
}
