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
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Variable : BaseVariable, IFieldElement
	{
		Variable _AncestorVariable;
		Variable _PreviousVariable;
		Variable _NextVariable;
		bool _IsEvent = false;
		Expression _FixedSize;
		bool _HasType = true;
		bool _IsAspxTag = false;
	bool _IsRunAtServer;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ArrayModifier = String.Empty;
		public Variable()
		{
		}
		public Variable(string name)
			: this(String.Empty, name)
		{
		}
		public Variable(string type, string name)
			: base()
		{
			MemberType = type;
			InternalName = name;
		}
		#region GetCanBeDocumented
		protected internal override bool GetCanBeDocumented()
		{
			return (!InsideMethod && !InsideProperty);
		}
		#endregion		
		#region ToString
		public override string ToString()
		{
			if (MemberType == null || MemberType == String.Empty)
				return Name;
			return MemberType + " " + Name;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void SetFixedSize(Expression expression)
		{
			ReplaceDetailNode(_FixedSize, expression);
			_FixedSize = expression;
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_FixedSize == oldElement)
				SetFixedSize(newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			_FixedSize = null;
			if (_PreviousVariable != null && _NextVariable != null)
			{
				_PreviousVariable._NextVariable = null;
				_PreviousVariable.CleanUpOwnedReferences();
				_PreviousVariable = null;
				_NextVariable._PreviousVariable = null;
				_NextVariable.CleanUpOwnedReferences();
				_NextVariable = null;
			}
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  Variable sourceVariable = source as Variable;
	  if (sourceVariable == null)
		return;
			_HasType = sourceVariable._HasType;
			_IsAspxTag = sourceVariable._IsAspxTag;
	  if (sourceVariable._FixedSize != null)
	  {
		_FixedSize = ParserUtils.GetCloneFromNodes(this, sourceVariable, sourceVariable._FixedSize) as Expression;
		if (_FixedSize == null)
		  _FixedSize = sourceVariable._FixedSize.Clone(options) as Expression;
	  }
	  if (sourceVariable._FixedSize != null)
	  {
		_FixedSize = ParserUtils.GetCloneFromNodes(this, sourceVariable, sourceVariable._FixedSize) as TypeReferenceExpression;
		if (_FixedSize == null)
		  _FixedSize = sourceVariable._FixedSize.Clone(options) as TypeReferenceExpression;
	  }
			options.AddClonedElement(sourceVariable, this);
			if (sourceVariable._PreviousVariable != null)
				_PreviousVariable = RequestClonedVariable(sourceVariable._PreviousVariable, options);
			if (sourceVariable._NextVariable != null)
				_NextVariable = RequestClonedVariable(sourceVariable._NextVariable, options);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void SetNextVariable(Variable variable)
		{
			if (_NextVariable == variable)
				return;
			_NextVariable = variable;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void SetPreviousVariable(Variable variable)
		{
			if (_PreviousVariable == variable)
				return;
			_PreviousVariable = variable;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void SetAncestorVariable(Variable variable)
		{
			if (_AncestorVariable == variable)
				return;
			_AncestorVariable = variable;
		}
		public ArrayList GetDeclarationList()
		{
			ArrayList lList = new ArrayList();
			if (!IsInDeclarationList)
			{
				lList.Add(this);
				return lList;
			}
			Variable lStart = this;
			while (lStart != null && !lStart.IsStart)
				lStart = lStart.PreviousVariable;
			if (lStart == null)
				return lList;
			lList.Add(lStart);
			while (lStart != null && !lStart.IsEnd)
			{
				lStart = lStart.NextVariable;
				lList.Add(lStart);
			}
			return lList;
		}
		public ArrayList GetReferencesInRange(SourceRange range)
		{
			ArrayList lResult = new ArrayList();
			if (range == SourceRange.Empty)
				return lResult;
			ArrayList lReferences = References;
			for (int i = 0; i < lReferences.Count; i++)
			{
				LanguageElement lElement = (LanguageElement)lReferences[i];
				if (range.Contains(lElement.Range))
					lResult.Add(lElement);
			}
			return lResult;
		}
		Variable RequestClonedVariable(Variable variable, ElementCloneOptions options)
		{
			Variable clone = options.GetClonedElement(variable) as Variable;
			if (clone != null)
				return clone;
			clone = variable.Clone(options) as Variable;
			options.AddClonedElement(variable, clone);
			return clone;
		}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{	  
			Variable clone = new Variable();
			clone.CloneDataFrom(this, options);
			return clone;
	}
	#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Variable;
			}
		}
		#region IsDefaultVisibility
		public override bool IsDefaultVisibility
		{
			get
			{
				if (_AncestorVariable != null)
					return _AncestorVariable.IsDefaultVisibility;
				else
					return base.IsDefaultVisibility;
			}
		}
		#endregion
	#region IsNew
	public override bool IsNew
	{
	  get
	  {
		if (_AncestorVariable != null)
		  return _AncestorVariable.IsNew;
		else
		  return base.IsNew;
	  }
	}
	#endregion
	#region IsReadOnly
		public override bool IsReadOnly
		{
			get
			{
				if (_AncestorVariable != null)
					return _AncestorVariable.IsReadOnly;
				else
					return base.IsReadOnly;
			}
		}
		#endregion
		#region IsStatic
		public override bool IsStatic
		{
			get
			{
				if (_AncestorVariable != null)
					return _AncestorVariable.IsStatic;
				else
					return base.IsStatic;
			}
		}
		#endregion
		#region IsVolatile
		public override bool IsVolatile
		{
			get
			{
				if (_AncestorVariable != null)
					return _AncestorVariable.IsVolatile;
				else
					return base.IsVolatile;
			}
		}
		#endregion
	#region IsWithEvents
	public override bool IsWithEvents
	{
	  get
	  {
		if (_AncestorVariable != null)
		  return _AncestorVariable.IsWithEvents;
		else
		  return base.IsWithEvents;
	  }
	}
	#endregion
		#region Visibility
		public override MemberVisibility Visibility
		{
			get
			{
				if (_AncestorVariable != null)
					return _AncestorVariable.Visibility;
				else
					return base.Visibility;
			}
		}
		#endregion
	#region AncestorVariable
	[Description("The Variable that contibutes type, modifiers (static, volatile, new, and/or readonly), and visibility to this variable. This property will be assigned if this variable was declared in a list (e.g., \"int a, b;\").")]
	[Category("Modifiers")]
	public Variable AncestorVariable
	{
	  get
	  {
		return _AncestorVariable;
	  }
	}
	#endregion
	#region InheritsModifiers
	[Description("True if this variable inherits modifiers from a previously-declared variable. For example, in the C# declaration \"int aa, bb;\", bb inherits its type and modifiers from aa. If this property is true, you can access the variable providing the modifiers through the AncestorVariable property.")]
	[Category("Modifiers")]
	public bool InheritsModifiers
	{
	  get
	  {
		return (_AncestorVariable != null);
	  }
	}
	#endregion
		#region PreviousVariable
		[Description("Gets previous variable if this variable is in declaration list. E.g. for the code int i, j, k; previous variable for \"j\" will be \"i\".")]
		[Category("Family")]
		public Variable PreviousVariable
		{
			get
			{
				return _PreviousVariable;
			}
		}
		#endregion
		#region NextVariable
		[Description("Gets next variable if this variable is in declaration list. E.g. for the code int i, j, k; next variable for \"j\" will be \"k\".")]
		[Category("Family")]
		public Variable NextVariable
		{
			get
			{
				return _NextVariable;
			}
		}
		#endregion
		#region IsInDeclarationList
		[Description("Returns true if this variable is in variable declaration list. E.g. for the code int i, j, k; variables i, j, k are in list.")]
		[Category("Family")]
		public bool IsInDeclarationList
		{
			get
			{
				return (PreviousVariable != null) || (NextVariable != null);
			}
		}
		#endregion
		#region FirstVariable
		[Description("Returns first variable inside variable delcaration list. E.g. for the code int i, j, k; it will return i variable.")]
		[Category("Family")]
		public Variable FirstVariable
		{
			get
			{
				Variable first = this;
				Variable current = first;
				while (current != null)
				{
					first = current;
					current = current.PreviousVariable;
				}
				return first;
			}
		}
		#endregion
		#region FirstVariable
		[Description("Returns last variable inside variable delcaration list. E.g. for the code int i, j, k; it will return k variable.")]
		[Category("Family")]
		public Variable LastVariable
		{
			get
			{
				Variable last = this;
				Variable current = last;
				while (current != null)
				{
					last = current;
					current = current.NextVariable;
				}
				return last;
			}
		}
		#endregion
		#region IsStart
		[Description("Returns true if this variable is in the start of a variable declaration list.")]
		[Category("Family")]
		public bool IsStart
		{
			get
			{
				return (PreviousVariable == null) && (NextVariable != null);
			}
		}
		#endregion
		#region IsMiddle
		[Description("Returns true if this variable is in the middle of a variable declaration list.")]
		[Category("Family")]
		public bool IsMiddle
		{
			get
			{
				return (PreviousVariable != null) && (NextVariable != null);
			}
		}
		#endregion
		#region IsEnd
		[Description("Returns true if this variable is in the end of a variable declaration list.")]
		[Category("Family")]
		public bool IsEnd
		{
			get
			{
				return (PreviousVariable != null) && (NextVariable == null);
			}
		}
		#endregion
		#region HasType
		[Description("Returns true if this variable has type.")]
		[Category("Family")]
		public bool HasType
		{
			get
			{
				return _HasType;
			}
			set
			{
				_HasType = value;
			}
		}
		#endregion
		[Description("Returns true if this variable declares fixed size buffer.")]
		[Category("Family")]
		public override bool IsFixedSizeBuffer
		{
			get
			{
				return _FixedSize != null;
			}
		}
		[Description("Gets or sets fixed size variable expression.")]
		[Category("Family")]
		public Expression FixedSize
		{
			get
			{
				return _FixedSize;
			}
			set
			{
				SetFixedSize(value);
			}
		}	
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool IsAspxTag
		{
			get
			{
				return _IsAspxTag;
			}
			set
			{
				_IsAspxTag = value;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool IsRunAtServer
	{
	  get
	  {
		return _IsRunAtServer;
	  }
	  set
	  {
		_IsRunAtServer = value;
	  }
	}
		#region IFieldElement Members
		bool IFieldElement.IsConst
		{
			get
			{
				return false;
			}
		}
		IExpression IFieldElement.Expression
		{
			get
			{
				return null;
			}
		}
		bool IFieldElement.IsBitField
		{
			get
			{
				return IsBitField;
			}
		}
		IExpression IFieldElement.BitFieldSize
		{
			get
			{
				return BitFieldSize;
			}
		}
		IElement IFieldElement.NodeLink
		{
			get
			{
				return null;
			}
		}
		bool IFieldElement.HasNodeLink
		{
			get
			{
				return false;
			}
		}
		#endregion
		public bool IsEvent
		{
			get
			{
				return _IsEvent;
			}
			set
			{
				_IsEvent = value;
			}
		}
	}
}
