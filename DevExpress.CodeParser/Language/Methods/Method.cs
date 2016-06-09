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
using System.ComponentModel;
using System.Collections.Specialized;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum CallingConventionSpecifier
  {
		Default,
	  StdCall,
	  Cdecl,
	  FastCall,
	  ClrCall,
	  ThisCall,
	  Pascal,
	  Fortran,
	  SysCall,
	  Far,
	  Export,
		Inline,
		ForceInline
  }
  public enum MethodTypeEnum : byte
  {
		Constructor,
		Destructor,
	Function,
		Void,
	Property,
		Event,
	Finalizer
  }
	public enum OperatorType
	{
		None,
		Decrement,
		Increment,
		UnaryNegation,
		UnaryPlus,
		LogicalNot,
		True,
		False,
		AddressOf,
		OnesComplement,
		PointerDereference,
		Addition,
		Subtraction,
		Multiply,
		Division,
		Modulus,
		ExclusiveOr,
		BitwiseAnd,
		BitwiseOr,
		LogicalAnd,
		LogicalOr,
		Assign,
		LeftShift,
		RightShift,
		SignedRightShift,
		UnsignedRightShift,
		Equality,
		GreaterThan,
		LessThan,
		Inequality,
		GreaterThanOrEqual,
		LessThanOrEqual,
		UnsignedRightShiftAssignment,
		MemberSelection,
		RightShiftAssignment,
		MultiplicationAssignment,
		PointerToMemberSelection,
		SubtractionAssignment,
		ExclusiveOrAssignment,
		LeftShiftAssignment,
		ModulusAssignment,
		AdditionAssignment,
		BitwiseAndAssignment,
		BitwiseOrAssignment,
		Comma,
		DivisionAssignment,
		Concatenate,
		Exponent,
		IntegerDivision,
		Like
	}
	[Flags]
	public enum MethodFlags : byte
	{
		None = 0x0000,
		IsClassOperator = 0x0001,
		IsImplicitCast = 0x0002,
		IsExplicitCast = 0x0004
	}
	public class EmptyVisibilityArray
	{
		public static readonly MemberVisibility[] Value = new MemberVisibility[0];
	}
	public class Method : MemberWithParameters, IMethodElement, IMethodElementModifier
	{
		const int INT_MaintainanceComplexity = 3;
		BaseVariable _ImplicitVariable;
		TypeReferenceExpressionCollection _ExceptionSpecification;
		MethodTypeEnum _MethodType;
		MethodFlags _MethodFlags;
		string _CharsetModifier;
		StringCollection _Handles;
		bool _MethodHasTryBlock;
		bool _IsConst;
		bool _IsMemberFunctionConst;
		CallingConventionSpecifier _CallingConvention;
		OperatorType _OverloadsOperator;
		bool _GenerateCodeBlock = true;
		string _Lib;
		string _Alias;
		SourceRange _ParamOpenRange;
		SourceRange _ParamCloseRange;
		SourceRange _MethodKeyWordRange;
	  #region Method
	  public Method()
	  {
	}
	  #endregion
		#region Method(string name)
		public Method(string name)
			: this()
		{
			InternalName = name;
		}
		#endregion
		#region Method(string type, string name)
		public Method(string type, string name)
			: this(name)
		{
			MemberType = type;
		}
		#endregion
	#region NeedToBackOut
	private bool NeedToBackOut(LanguageElement element)
		{
			return element.ElementType != LanguageElementType.Class &&
				element.ElementType != LanguageElementType.Interface &&
				element.ElementType != LanguageElementType.Struct &&
				element.ElementType != LanguageElementType.Module;
	}
	#endregion
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Local;
		}
		#endregion
	  #region ToString
	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append(MemberType + " " + InternalName + "(" );	  		
		if (Parameters != null )
		  for(int i = 0; i < Parameters.Count; i++)
			sb.AppendFormat( "{0} {1}{2}", 
				((Param)Parameters[i]).ParamType, 
				((Param)Parameters[i]).InternalName, 
				i == Parameters.Count - 1 ? String.Empty : ","
				);
		sb.Append( ")" );
		return sb.ToString();
	  }
	  #endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if (_MethodType == MethodTypeEnum.Constructor)
			{
				if (IsStatic)
					return ImageIndex.StaticConstructor;
				else
					return ImageIndex.Constructor;
			}
			else
			{
				if (IsStatic)
					return ImageIndex.StaticMethod;
				else
					return ImageIndex.Method;
			}
		}
		#endregion
		#region GetCyclomaticComplexity
		public override int GetCyclomaticComplexity()
		{
			return 1 + GetChildCyclomaticComplexity();
		}
		#endregion
		#region RangeIsClean
		public override bool RangeIsClean(SourceRange sourceRange)
		{
			return (sourceRange.Top > BlockStart.Bottom) && (sourceRange.Bottom < BlockEnd.Top);
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Method lClone = new Method();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	#region FlagIsSet
	protected bool FlagIsSet(MethodFlags flag)
		{
			return (_MethodFlags & flag) == flag;
		}
		protected void SetFlagValue(MethodFlags flag, bool value)
		{
			_MethodFlags = value ? _MethodFlags | flag : _MethodFlags & (~flag);
	}
	#endregion
	#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Method))
				return;			
			Method lSource = (Method)source;
			_MethodType = lSource._MethodType;
			_MethodFlags = lSource._MethodFlags;
			_Lib = lSource._Lib;
			_Alias = lSource._Alias;
			_IsConst = lSource._IsConst;
			_IsMemberFunctionConst = lSource.IsMemberFunctionConst;
	  _CallingConvention = lSource._CallingConvention;
	  _OverloadsOperator = lSource._OverloadsOperator;
			_CharsetModifier = lSource._CharsetModifier;
			_ParamOpenRange = lSource.ParamOpenRange;
			_ParamCloseRange = lSource.ParamCloseRange;
			_MethodKeyWordRange = lSource.MethodKeyWordRange;
			_GenerateCodeBlock = lSource._GenerateCodeBlock;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _ParamOpenRange = ParamOpenRange;
	  _ParamCloseRange = ParamCloseRange;
	  _MethodKeyWordRange = MethodKeyWordRange;
	}
	#region CreateMethod
	public Method CreateMethod()
	{
	  Method clone = new Method();
	  clone.CloneDataFrom(this, ElementCloneOptions.Default);
	  clone.RemoveAllNodes();
	  return clone;
	}
	#endregion
	#region CreateMethodPrototype
		public MethodPrototype CreateMethodPrototype()
		{
			MethodPrototype result = new MethodPrototype(MemberType, Name);
			if (MemberTypeReference != null)
				result.MemberTypeReference = MemberTypeReference.Clone() as TypeReferenceExpression;
			result.MemberType = MemberType;
			result.MethodType = MethodType;
	  result.IsConst = IsConst;
	  result.IsMemberFunctionConst = IsMemberFunctionConst;
			if (AccessSpecifiers != null)
				result.AccessSpecifiers = AccessSpecifiers.Clone() as AccessSpecifiers;
			int count = ParameterCount;
			for (int i = 0; i < count; i++)
				result.Parameters.Add(Parameters[i].Clone() as Param);
			return result;
	}
	#endregion
	#region CreateAnonymousMethod
	public AnonymousMethodExpression CreateAnonymousMethod()
		{
			AnonymousMethodExpression result = new AnonymousMethodExpression();
			for (int i = 0; i < ParameterCount; i++)
				result.AddParameter(Parameters[i].Clone() as Param);
			for (int i = 0; i < NodeCount; i++)
				result.AddNode((Nodes[i] as LanguageElement).Clone() as LanguageElement);
			return result;
	}
	#endregion
	#region IsEventHandler
	public bool IsEventHandler()
	{
	  return StructuralParserServicesHolder.IsEventHandler(this);	  
	}
	#endregion
	#region IsMainProcedure
	public bool IsMainProcedure()
	{
	  return StructuralParserServicesHolder.IsMainProcedure(this);
	}
	#endregion
	public bool IsInitializeComponent()
	{
	  return StructuralParserServicesHolder.IsInitializeComponent(this);
	}
	public bool IsSerializationConstructor
	{
	  get
	  {
		return StructuralParserServicesHolder.IsSerializationConstructor(this);
	  }
	}
	#region ContainsFlag
	bool ContainsFlag(MethodCharacteristics options, MethodCharacteristics flag)
	{
	  return (options & flag) == flag;
	}
	#endregion
	#region CheckInteriorPtrPointer
	bool CheckInteriorPtrPointer()
	{
	  return MemberTypeReference != null &&
		StructuralParserServicesHolder.IsInteriorPtrPointer(MemberTypeReference);		
	}
	#endregion
	#region CheckPinPtrPointer
	bool CheckPinPtrPointer()
	{
	  return MemberTypeReference != null &&
		StructuralParserServicesHolder.IsInteriorPtrPointer(MemberTypeReference);
	}
	#endregion
	#region ContainsMethod
	bool ContainsMethod(ITypeElement baseType, Method method)
	{
	  IElementFilter filter = StructuralParserServicesHolder.GetMemberSignatureFilter(this);
	  IMemberElementCollection members = baseType.FindMembers(method.Name, filter, false);
	  return members != null && members.Count > 0;
	}
	#endregion
	#region CheckInterfaceImplementor
	bool CheckInterfaceImplementor()
	{
	  if (ImplementsCount > 0)
		return true;
	  if (Visibility != MemberVisibility.Public)
		return false;
	  Class parent = GetParentClass();
	  if (parent == null)
		return false;
	  ITypeElement[] baseTypes = StructuralParserServicesHolder.GetAllBaseTypes(parent);
	  if (baseTypes == null || baseTypes.Length == 0)
		return false;
	  int count = baseTypes.Length;
	  for (int i = 0; i < count; i++)
	  {
		ITypeElement baseType = baseTypes[i] as IInterfaceElement;
		if (baseType == null || !(ContainsMethod(baseType, this)))
		  continue;
		return true;
	  }
	  return false;
	}
	#endregion
	#region CheckInClass
	bool CheckInClass()
	{
	  if (Parent == null)
		return false;
	  LanguageElementType type = Parent.ElementType;
	  return type == LanguageElementType.Class ||
		type == LanguageElementType.ManagedClass ||
		type == LanguageElementType.ValueClass;
	}
	#endregion
	#region CheckInStruct
	bool CheckInStruct()
	{
	  if (Parent == null)
		return false;
	  LanguageElementType type = Parent.ElementType;
	  return type == LanguageElementType.Struct ||
		type == LanguageElementType.ValueStruct ||
		type == LanguageElementType.ManagedStruct;
	}
	#endregion
	#region CheckInModule
	bool CheckInModule()
	{
	  if (Parent == null)
		return false;
	  return Parent.ElementType == LanguageElementType.Module ;
	}
	#endregion
	#region CheckInInterface
	bool CheckInInterface()
	{
	  if (Parent == null)
		return false;
	  LanguageElementType type = Parent.ElementType;
	  return type == LanguageElementType.Interface ||
		type == LanguageElementType.InterfaceClass ||
		type == LanguageElementType.InterfaceStruct;
	}
	#endregion
	#region HasCharacteristics
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool HasCharacteristics(MethodCharacteristics options)
	{
	  if (ContainsFlag(options, MethodCharacteristics.New) && IsNew)
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.Abstract) && IsAbstract)
		return true;	  
	  if (ContainsFlag(options, MethodCharacteristics.Virtual) && IsVirtual)
		return true;	  
	  if (ContainsFlag(options, MethodCharacteristics.Override) && IsOverride)
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.ClassOperator) && IsClassOperator)
		return true;	 
	  if (ContainsFlag(options, MethodCharacteristics.Extern) && IsExtern)
		return true;	  
	  if (ContainsFlag(options, MethodCharacteristics.MainProcedure) && IsMainProcedure())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.Generic) && IsGeneric)
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.InClass) && CheckInClass())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.InStruct) && CheckInStruct())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.InModule) && CheckInModule())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.InInterface) && CheckInInterface())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.InteriorPtrPointer) && CheckInteriorPtrPointer())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.PinPtrPointer) && CheckPinPtrPointer())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.EventHandler) && IsEventHandler())
		return true;
	  if (ContainsFlag(options, MethodCharacteristics.InterfaceImplementer) && CheckInterfaceImplementor())
		return true;
			if (ContainsFlag(options, MethodCharacteristics.IsExtensionMethod) && ((IMethodElement)this).IsExtensionMethod())
				return true;
			if (ContainsFlag(options, MethodCharacteristics.IsWebMethod) && IsWebMethod)
				return true;
	  return false;
	}
	#endregion
	#region ThisMaintenanceComplexity
	protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Method;
			}
	}
	#endregion
	#region Handles
	public StringCollection Handles
		{
			get
			{
				if (_Handles == null)
					_Handles = new StringCollection();
				return _Handles;
			}
		}
		#endregion
	#region HandlesCount
	public int HandlesCount
	{
	  get
	  {
		if (_Handles == null)
		  return 0;
		return _Handles.Count;
	  }
	}
	#endregion
		#region IsConstructor
		[Description("Returns true if this method is constructor.")]
		[Category("Details")]
		public bool IsConstructor
		{
			get
			{
				return MethodType == MethodTypeEnum.Constructor;
			}
			set 
			{
				if (value)
					MethodType = MethodTypeEnum.Constructor;
			}
		}
		#endregion
		#region IsDestructor
		[Description("Returns true if this method is destructor.")]
		[Category("Details")]
		public bool IsDestructor
		{
			get
			{
				return MethodType == MethodTypeEnum.Destructor;
			}
			set 
			{
				if (value)
					MethodType = MethodTypeEnum.Destructor;
			}
		}
		#endregion
		#region IsFinalizer
		[Description("Returns true if this method is finalizer.")]
		[Category("Details")]
		public bool IsFinalizer
		{
			get
			{
				return MethodType == MethodTypeEnum.Finalizer;
			}
			set 
			{
				if (value)
					MethodType = MethodTypeEnum.Finalizer;
			}
		}
		#endregion
		#region IsClassOperator
		[Description("Returns true if this method is operator override.")]
		[Category("Details")]
		public bool IsClassOperator
		{
			get
			{
				return FlagIsSet(MethodFlags.IsClassOperator);
			}
			set
			{
				SetFlagValue(MethodFlags.IsClassOperator, value);
			}
		}
		#endregion
		#region IsImplicitCast
		[Description("Returns true if this method defines implicit type cast.")]
		[Category("Details")]
		public bool IsImplicitCast
		{
			get
			{
				return FlagIsSet(MethodFlags.IsImplicitCast);
			}
			set
			{
				SetFlagValue(MethodFlags.IsImplicitCast, value);
			}
		}
		#endregion
		#region IsExplicitCast
		[Description("Returns true if this method defines explicit type cast.")]
		[Category("Details")]
		public bool IsExplicitCast
		{
			get
			{
				return FlagIsSet(MethodFlags.IsExplicitCast);
			}
			set
			{
				SetFlagValue(MethodFlags.IsExplicitCast, value);
			}
		}
		#endregion
	#region IsConst
		public override bool IsConst
		{
			get
			{
				return _IsConst;
			}
			set
			{
				_IsConst = value;
			}
	}
	#endregion
	#region CallingConvention
	public CallingConventionSpecifier CallingConvention
		{
			get
			{
				return _CallingConvention;
			}
			set
			{
				_CallingConvention = value;
			}
	}
	#endregion
	#region ExceptionSpecification
	public TypeReferenceExpressionCollection ExceptionSpecification
		{
			get
			{
				return _ExceptionSpecification;
			}
			set
			{
				_ExceptionSpecification = value;
			}
	}
	#endregion
	#region MethodHasTryBlock
	public bool MethodHasTryBlock
		{
			get
			{
				return _MethodHasTryBlock;
			}
			set
			{
				_MethodHasTryBlock = value;
			}
	}
	#endregion
	#region IsMemberFunctionConst
	public bool IsMemberFunctionConst
		{
			get
			{
				return _IsMemberFunctionConst;
			}
			set
			{
				_IsMemberFunctionConst = value;
			}
	}
	#endregion
	#region IsExplicitInterfaceMethod
		[Obsolete("Use IsExplicitInterfaceMember instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsExplicitInterfaceMethod
		{
			get
			{
				return IsExplicitInterfaceMember;
			}
			set
			{
				IsExplicitInterfaceMember = value;
			}
	}
	#endregion
	#region ValidVisibilities
	public override MemberVisibility[] ValidVisibilities
	{
	  get
	  {
		if (IsExplicitInterfaceMember || IsPartial)
		  return EmptyVisibilityArray.Value;
		if (IsClassOperator)
		  return new MemberVisibility[] { MemberVisibility.Public };
		MemberVisibility[] visibilites = base.ValidVisibilities;
		if (IsOverride | IsVirtual)
		{
		  visibilites = base.ValidVisibilities;
		  if (visibilites == null)
			return null;
		  List<MemberVisibility> list = new List<MemberVisibility>();
		  foreach (MemberVisibility visibility in visibilites)
			if (visibility != MemberVisibility.Private)
			  list.Add(visibility);
		  return list.ToArray();
		}
		return visibilites;
	  }
	}
		#endregion
		#region VisibilityIsFixed
		[Description("True if the visibility of this member can not be changed (e.g., illegal, param, local, or if this is a static constructor).")]
		[Category("Access")]
		public override bool VisibilityIsFixed
		{
			get
			{
		if ((IsStatic && IsConstructor) || IsDestructor || IsClassOperator)
					return true;
				else
					return base.VisibilityIsFixed;
			}
		}
		#endregion
	#region OverloadsOperator
		public OperatorType OverloadsOperator
		{
			get
			{
				return _OverloadsOperator;
			}
			set
			{
				_OverloadsOperator = value;
			}
	}
	#endregion
	#region CharsetModifier
		[Description("Returns char set modifier set for this method.")]
		[Category("Details")]
		public string CharsetModifier
		{
			get
			{
				return _CharsetModifier;
			}
			set
			{
				_CharsetModifier = value;
			}
		}
		#endregion
		#region MethodType
		[Description("The type of this method (e.g., Constructor, Destructor, Function, or Void).")]
		[Category("Details")]
		public MethodTypeEnum MethodType
		{
			get
			{
				return _MethodType;
			}
			set
			{
				_MethodType = value;
			}
		}
		#endregion
		#region ParamOpenRange
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
		#endregion
		#region ParamCloseRange
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
		#endregion
	#region SetAsRange
	public void SetAsRange(SourceRange range)
		{
	  AsRange = range;
		}
		#endregion		
	#region SetMethodKeyWordRange
	public void SetMethodKeyWordRange(SourceRange range)
		{
	  ClearHistory();
			_MethodKeyWordRange = range;
		}
		public SourceRange MethodKeyWordRange
		{
			get
			{
				return GetTransformedRange(_MethodKeyWordRange);
			}
		}
		#endregion
		#region Initializer
		public ConstructorInitializer Initializer
		{
			get
			{
				if (HasInitializer)
					return Nodes[0] as ConstructorInitializer;
				return null;
			}
		}
		#endregion
		#region HasInitializer
		public bool HasInitializer
		{
			get
			{
				return Nodes != null && NodeCount > 0 && Nodes[0] is ConstructorInitializer;
			}
		}
		#endregion
	#region ImplicitVariable
		public BaseVariable ImplicitVariable
		{
			get
			{
				return _ImplicitVariable;
			}
			set
			{
				_ImplicitVariable = value;
			}
	}
	#endregion
	#region GenerateCodeBlock
	public bool GenerateCodeBlock
		{
			get
			{
				return _GenerateCodeBlock;
			}
			set
			{
				_GenerateCodeBlock = value;
			}
	}
	#endregion
	#region HasImplicitVariable
		public bool HasImplicitVariable
		{
			get
			{
				return _ImplicitVariable != null;
			}
	}
	#endregion
	#region CanContainCode
	public override bool CanContainCode
		{
			get 
			{
				return true;
			}
	}
	#endregion
	#region IsNewContext
	public override bool IsNewContext
		{
			get 
			{
				return true;
			}
	}
	#endregion
	#region Lib
	public string Lib
		{
			get
			{
				return _Lib;
			}
			set
			{
				_Lib = value;
			}
	}
	#endregion
	#region Alias
	public string Alias
		{
			get
			{
				return _Alias;
			}
			set
			{
				_Alias = value;
			}
	}
	#endregion
		public bool IsWebMethod
		{
			get
			{
		return StructuralParserServicesHolder.IsWebMethod(this);
			}
		}
	public bool HasDllImportAttribute
	{
	  get
	  {
		return StructuralParserServicesHolder.HasDllImportAttribute(this);
	  }
	}
	public static bool CheckExtensionMethod(IMethodElement method)
	{
	  return CheckExtensionMethod(null, method);
	}
		public static bool CheckExtensionMethod(ISourceTreeResolver resolver, IMethodElement method)
		{
	  return StructuralParserServicesHolder.CheckExtensionMethod(resolver, method);
		}
	#region IMethodElement Members
	bool IMethodElement.IsExtensionMethod()
		{
			if (IsConstructor || IsDestructor)
				return false;
			return CheckExtensionMethod(this);
		}
	bool IMethodElement.IsExtensionMethod(ISourceTreeResolver resolver)
	{
	  if (IsConstructor || IsDestructor)
		return false;
	  return CheckExtensionMethod(resolver, this);
	}
		IExpressionCollection IMethodElement.HandlesExpressions
		{
			get
			{
				return HandlesExpressions;
			}
		}
		IExpressionCollection IMethodElement.ImplementsExpressions
		{
			get
			{
				return ImplementsExpressions;
			}
		}
		string IMethodElement.Alias
		{
			get
			{
				return Alias;
			}
		}
		string IMethodElement.Lib
		{
			get
			{
				return Lib;
			}
		}
		bool IMethodElement.IsConstructor
		{
			get
			{
				return IsConstructor;
			}
		}
		bool IMethodElement.IsDestructor
		{
			get
			{
				return IsDestructor;
			}
		}
		bool IMethodElement.IsTypeInitializer
		{
			get
			{
				return IsConstructor && IsStatic;
			}
		}
		IBaseVariable IMethodElement.ImplicitVariable 
		{
			get
			{
				return ImplicitVariable;
			}
		}
		#endregion
	#region IsAsynchronous
		[Description("True if this method has async modifier.")]
		[Category("Access")]
		[DefaultValue(false)]
		public bool IsAsynchronous
		{
			get
			{
				if (HasAccessSpecifiers)
					return AccessSpecifiers.IsAsynchronous;
				return false;
			}
	  set
	  {
		EnsureAccessSpecifiers();
				AccessSpecifiers.IsAsynchronous = value;
	  }
		}
		#endregion
	#region IMethodElementModifier Members
	void IMethodElementModifier.SetIsClassOperator(bool isClassOperator)
	{
	  IsClassOperator = isClassOperator;
	}
	void IMethodElementModifier.SetIsExplicitCast(bool isExplicitCast)
	{
	  IsExplicitCast = isExplicitCast;
	}
	void IMethodElementModifier.SetIsImplicitCast(bool isImplicitCast)
	{
	  IsImplicitCast = isImplicitCast;
	}
	#endregion
  }
}
