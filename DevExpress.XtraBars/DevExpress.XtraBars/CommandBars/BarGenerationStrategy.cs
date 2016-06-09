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
namespace DevExpress.XtraBars.Commands.Design {
	#region BarGenerationStrategy (abstract class)
	public abstract class BarGenerationStrategy {
		public abstract void AddToContainer(IComponent component);
		public abstract void RemoveFromContainer(IComponent component);
		public abstract void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue);
		public abstract void OnComponentChanging(object component, MemberDescriptor member);
		public abstract IComponent CreateComponent(Type type);
	}
	#endregion
	#region RunTimeBarGenerationStrategy
	public class RunTimeBarGenerationStrategy : BarGenerationStrategy {
		public override void AddToContainer(IComponent component) {
		}
		public override void RemoveFromContainer(IComponent component) {
		}
		public override void OnComponentChanging(object component, MemberDescriptor member) {
		}
		public override void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue) {
		}
		public override IComponent CreateComponent(Type type) {
			return Activator.CreateInstance(type) as IComponent;
		}
	}
	#endregion
}
