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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Event : MemberWithParameters, IEventElement, IEventElementModifier
	{
		bool _IsInterfaceEvent = false;
		Expression _Initializer;
	bool _GenerateAccessors = true;
	  #region Event
	  public Event()
	  {
	  }
	  #endregion
		#region Event(string name)
		public Event(string name): this()
		{
			InternalName = name;
		}
		#endregion
		void SetInitializer(Expression initializer)
		{
			if (initializer == null)
				return;
			ReplaceDetailNode(_Initializer, initializer);
			_Initializer = initializer;
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			if (IsStatic)
				return ImageIndex.StaticEvent;
			else
				return ImageIndex.Event;
		}
		#endregion
	  #region ToString
	  public override string ToString()
	  {
			return InternalName;
	  }
	  #endregion
		#region GetDefaultVisibility
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Illegal;
		}
		#endregion
	#region ClearInitializer
	public void ClearInitializer()
	{
	  if (_Initializer == null)
		return;
	  if (DetailNodes.Contains(_Initializer))
		DetailNodes.Remove(_Initializer);
	  _Initializer = null;
	}
	#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Event lClone = new Event();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			base.CloneDataFrom (source, options);
			if (source != null && source is Event)
			{
				Event lSource = source as Event;
		_GenerateAccessors = lSource._GenerateAccessors;
				this._IsInterfaceEvent = lSource.IsInterfaceEvent;
				if (lSource._Initializer != null)
				{
					_Initializer = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Initializer) as Expression;
					if (_Initializer == null)
						_Initializer = lSource._Initializer.Clone(options) as Expression;
				}
			}
		}
		#endregion   
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Event;
			}
		}
		public bool IsInterfaceEvent
		{
			get
			{
				return _IsInterfaceEvent;
			}
			set
			{
				_IsInterfaceEvent = value;
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
		[Description("Gets the Add accessor for this event, if declared.")]
		[Category("Family")]
		public EventAdd Adder
		{
			get
			{
				return FindChildByElementType(LanguageElementType.EventAdd) as EventAdd;
			}
		}
		[Description("Gets the Remove accessor for this event, if declared.")]
		[Category("Family")]
		public EventRemove Remover
		{
			get
			{
				return FindChildByElementType(LanguageElementType.EventRemove) as EventRemove;
			}
		}
		[Description("Gets the Raise accessor for this event, if declared.")]
		[Category("Family")]
		public EventRaise Raise
		{
			get
			{
				return FindChildByElementType(LanguageElementType.EventRaise) as EventRaise;
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
				SetInitializer(value);
			}
		}
		#region IEventElement Members
		public IMethodElement AddMethod
		{
			get
			{
				return Adder;
			}
			set
			{
			}
		}
		public IMethodElement RemoveMethod
		{
			get
			{
				return Remover;
			}
			set
			{
			}
		}
		public IMethodElement RaiseMethod
		{
			get
			{
				return Raise;
			}
			set
			{
			}
		}
		IExpression IEventElement.Initializer
		{
			get
			{
				return _Initializer;
			}
		}
	public virtual IElementCollection ImplicitElements
	{
	  get
	  {
		return IElementCollection.Empty;
	  }
	}
		#endregion
	#region IEventElementModifier Members
	void IEventElementModifier.SetAddMethod(IMethodElement method)
	{
	  EventAdd adder = method as EventAdd;
	  if (adder == null)
		return;
	  AddNode(adder);
	}
	void IEventElementModifier.SetRemoveMethod(IMethodElement method)
	{
	  EventRemove remover = method as EventRemove;
	  if (remover == null)
		return;
	  AddNode(remover);
	}
	#endregion 
  }
}
