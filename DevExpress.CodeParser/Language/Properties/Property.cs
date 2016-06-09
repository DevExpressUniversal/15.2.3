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
	public class Property : MemberWithParameters, IPropertyElement, IPropertyElementModifier
	{
	const string STR_Value = "value";
		private const int INT_MaintainanceComplexity = 0;
		Expression _Initializer;
		bool _IsAutoImplemented = false;
	bool _GenerateAccessors = true;
	bool _GenerateParens = false;
		#region protected fields...
		SourceRange _IndexOpenRange;
		SourceRange _IndexCloseRange;
		#endregion
		#region Property
		public Property()
		{
	}
		#endregion
		#region Property(string name)
		public Property(string name): this()
		{
			InternalName = name;
		}
		#endregion
		#region Property(string type, string name)
		public Property(string type, string name)
			: this(name)
		{
			MemberType = type;
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			_Initializer = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Initializer == oldElement)
				_Initializer = (TypeReferenceExpression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Property))
				return;			
			Property lSource = (Property)source;
			_IndexOpenRange = lSource.IndexOpenRange;
			_IndexCloseRange = lSource.IndexCloseRange;
			_IsAutoImplemented = lSource._IsAutoImplemented;
	  _GenerateAccessors = lSource._GenerateAccessors;
	  _GenerateParens = lSource._GenerateParens;
			_Initializer = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Initializer) as Expression;
			if (_Initializer == null && lSource._Initializer != null)
				_Initializer = lSource._Initializer.Clone() as Expression;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _IndexOpenRange = IndexOpenRange;
	  _IndexCloseRange = IndexCloseRange;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetIndexedPropertyNameWithoutImplementsQualifier()
	{
	  if (!IsIndexed)
		return;
	  string oldPropName = Name;
	  string implementsQualifierName = oldPropName.Substring(0, oldPropName.LastIndexOf('.') + 1);
	  if (string.IsNullOrEmpty(implementsQualifierName))
		return;
	  Name = oldPropName.Replace(implementsQualifierName, string.Empty);
	  if (HasGetter)
	  {
		string getterName = Getter.Name;
		Getter.Name = getterName.Replace(implementsQualifierName, string.Empty);
	  }
	  if (HasSetter)
	  {
		string setterName = Setter.Name;
		Setter.Name = setterName.Replace(implementsQualifierName, string.Empty);
	  }
	}
		#region GetCyclomaticComplexity
		public override int GetCyclomaticComplexity()
		{
			int result = 0;
			Set lSetter = Setter;
			if (lSetter != null)
				result += lSetter.GetCyclomaticComplexity();
			Get lGetter = Getter;
			if (lGetter != null)
				result += lGetter.GetCyclomaticComplexity();
			return result;
		}
		#endregion
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Illegal;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
	  if (IsStatic)
		return ImageIndex.StaticProperty;
	  else if (IsIndexed)
		return ImageIndex.IndexedProperty;
	  else
				return ImageIndex.Property;
		}
		#endregion
	#region ToString
	  public override string ToString()
	  {
		return InternalName;
	  }
	  #endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Property lClone = new Property();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Property;
			}
		}
		#region HasGetter
		[Description("True if this property can be read.")]
		[Category("Details")]
		[DefaultValue(true)]
		public bool HasGetter
		{
			get
			{
				return Getter != null;
			}
		}
		#endregion
		#region HasSetter
		[Description("True if this property can be written to.")]
		[Category("Details")]
		public bool HasSetter
		{
			get
			{
				return Setter != null;
			}
		}
		#endregion
		#region IsReadable
		[Description("True if this property can be read.")]
		[Category("Details")]
		[DefaultValue(true)]
		[Obsolete("Use HasGetter instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsReadable
		{
			get
			{
				return Getter != null;
			}
		}
		#endregion
		#region IsWritable
		[Description("True if this property can be written to.")]
	[Category("Details")]
		[Obsolete("Use HasSetter instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsWritable
		{
			get
			{
				return Setter != null;
			}
		}
		#endregion
		#region IsIndexed
		[Description("True if this property is an indexer.")]
		[Category("Details")]
	[DefaultValue(false)]
		public bool IsIndexed
		{
			get
			{
				return ParameterCount > 0;
			}
		}
		#endregion
		#region Setter
		[Description("Gets the Set accessor for this property, if available. Returns null if this property is read-only.")]
		[Category("Family")]
		public Set Setter
		{
			get
			{
				return FindChildByElementType(LanguageElementType.PropertyAccessorSet) as Set;
			}
		}
		#endregion
		#region Getter
		[Description("Returns the Get accessor for this property, if available. Returns null if this property is write-only.")]
		[Category("Family")]
		public Get Getter
		{
			get
			{
				return FindChildByElementType(LanguageElementType.PropertyAccessorGet) as Get;
			}
		}
		#endregion
		#region IndexOpenRange
		public SourceRange IndexOpenRange
		{
			get
			{
				return GetTransformedRange(_IndexOpenRange);
			}
	  set
	  {
		ClearHistory();
		_IndexOpenRange = value;
	  }
		}
		#endregion
		#region IndexCloseRange
		public SourceRange IndexCloseRange
		{
			get
			{
				return GetTransformedRange(_IndexCloseRange);
			}
	  set
	  {
		ClearHistory();
		_IndexCloseRange = value;
	  }
		}
		#endregion
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
		public bool IsAutoImplemented
		{
			get
			{
				return _IsAutoImplemented;
			}
			set
			{
				_IsAutoImplemented = value;
			}
		}
	[Browsable(false)]
	public bool GenerateAccessors
	{
			get
			{
		return _GenerateAccessors;
			}
			set
			{
		_GenerateAccessors = value;
			}
		}
	[Browsable(false)]
	public bool GenerateParens
	{
	  get
	  {
		return _GenerateParens;
	  }
	  set
	  {
		_GenerateParens = value;
	  }
	}
		public Expression Initializer
		{
			get
			{
				return _Initializer;
			}
			set
			{
				_Initializer = value;
			}
		}
		#region IPropertyElement Members
		IMethodElement IPropertyElement.GetMethod
		{
			get
			{
				return Getter;
			}
		}
		IMethodElement IPropertyElement.SetMethod
		{
			get
			{
				return Setter;
			}
		}
		IExpression IPropertyElement.Initializer
		{
			get
			{
				return _Initializer;
			}
		}
		#endregion
	#region IPropertyElementModifier Members
	void IPropertyElementModifier.SetGetMethod(IMethodElement method)
	{
	  Get getter = method as Get;
	  if (getter == null)
		return;
	  AddNode(getter);
	}
	void IPropertyElementModifier.SetSetMethod(IMethodElement method)
	{
	  Set setter = method as Set;
	  if (setter == null)
		return;
	  AddNode(setter);
	}
	#endregion
  }
}
