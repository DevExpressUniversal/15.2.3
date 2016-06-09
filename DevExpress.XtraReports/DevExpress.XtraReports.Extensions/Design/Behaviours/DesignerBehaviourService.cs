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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
namespace DevExpress.XtraReports.Design.Behaviours {
	public interface IDesignerBehaviourService {
		IDesignerBehaviour GetBehaviour(XRControlDesigner designer);
	}
	class DesignerBehaviourService : IDesignerBehaviourService, IDisposable {
		IServiceProvider servProvider;
		Dictionary<Type, IDesignerBehaviour> dictionary = new Dictionary<Type, IDesignerBehaviour>();
		public DesignerBehaviourService(IServiceProvider servProvider) {
			this.servProvider = servProvider;
		}
		public virtual IDesignerBehaviour GetBehaviour(XRControlDesigner designer) {
			Type behaviourType = GetBehaviourType(designer);
			if(behaviourType == null)
				return null;
			IDesignerBehaviour behaviour;
			if(!dictionary.TryGetValue(behaviourType, out behaviour)) {
				behaviour = Activator.CreateInstance(behaviourType, servProvider) as IDesignerBehaviour;
				dictionary.Add(behaviourType, behaviour);
			}
			if(behaviour is DesignerBehaviour)
				((DesignerBehaviour)behaviour).SetCurrentDesigner(designer);
			return behaviour;
		}
		static Type GetBehaviourType(IDesigner designer) {
			DesignerBehaviourAttribute attr = TypeDescriptor.GetAttributes(designer)[typeof(DesignerBehaviourAttribute)] as DesignerBehaviourAttribute;
			return attr != null ? attr.AdornerType : null;
		}
		void IDisposable.Dispose() {
			if(dictionary != null) {
				foreach(object item in dictionary.Values)
					if(item is IDisposable)
						((IDisposable)item).Dispose();
				dictionary.Clear();
				dictionary = null;
			}
		}
	}
}
