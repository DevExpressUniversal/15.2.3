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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  public class VBEvent : Event
	{
		const string STR_Custom = "Custom";
	IElementCollection _ImplicitElements;
		#region Event
		public VBEvent()
		{
		}
		#endregion
	public VBEvent(string name) : base(name)
	{
	}
	DelegateDefinition GetEventDelegate()
	{
	  string eventHandlerName = Name + "EventHandler";
	  DelegateDefinition eventDelegate = new DelegateDefinition(eventHandlerName);
	  if (Parameters != null)
		foreach (Param eventParameter in Parameters)
		{
		  if (eventParameter == null)
			continue;
		  Param clone = eventParameter.Clone() as Param;
		  if (clone != null)
			eventDelegate.AddParameter(clone);
		}
	  eventDelegate.Visibility = Visibility;
	  eventDelegate.SetRange(Range);
	  eventDelegate.SetNameRange(NameRange);
	  eventDelegate.SetParent(this);
	  return eventDelegate;
	}
	Variable GetEventField(TypeReferenceExpression type)
	{
	  Variable eventField = new Variable(Name + "Event");
	  if (type != null)
	  {
		eventField.MemberType = type.Name;
		eventField.SetMemberTypeReference(type);
	  }
	  eventField.Visibility = MemberVisibility.Private;
	  eventField.SetRange(Range);
	  eventField.SetNameRange(NameRange);
	  eventField.SetParent(this);
	  return eventField;
	}
	IElementCollection GetImplicitElements()
	{
	  IElementCollection result = new IElementCollection();
	  TypeReferenceExpression type = null;
	  if (MemberTypeReference == null)
	  {
		DelegateDefinition eventDelegate = GetEventDelegate();
		result.Add(eventDelegate);
		type = new TypeReferenceExpression(eventDelegate.Name);
	  }
	  else
	  {
		type = MemberTypeReference.Clone() as TypeReferenceExpression;
	  }
	  if (type == null)
		return result;
	  Variable eventField = GetEventField(type.Clone() as TypeReferenceExpression);
	  result.Add(eventField);
	  return result;
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			VBEvent lClone = new VBEvent();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override MemberVisibility[] ValidVisibilities
		{
			get
			{
				return BaseValidVisibilities;
			}
		}
		public override bool VisibilityIsFixed
		{
			get
			{
				if (IsStatic)
					return true;
				else
					return false;
			}
		}
	public override IElementCollection ImplicitElements
	{
	  get
	  {
		if (_ImplicitElements == null)
		  _ImplicitElements = GetImplicitElements();
		return _ImplicitElements;
	  }
	}
	}
}
