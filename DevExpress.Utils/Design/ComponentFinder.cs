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
using System.ComponentModel.Design;
using System.Collections.Generic;
namespace DevExpress.Utils.Design {
	#region ComponentFinder (helper class)
	public static class ComponentFinder {
		public static T FindComponentOfType<T>(IServiceProvider serviceProvider) where T : Component {
			if (serviceProvider == null)
				return default(T);
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if (host == null)
				return default(T);
			foreach (IComponent component in host.Container.Components)
				if (typeof(T).IsAssignableFrom(component.GetType()))
					return (T)component;
			return default(T);
		}
		public static List<T> FindComponentsOfType<T>(IServiceProvider serviceProvider) where T : Component {
			List<T> result = new List<T>();
			if (serviceProvider == null)
				return result;
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if (host == null)
				return result;
			foreach (IComponent component in host.Container.Components)
				if (typeof(T).IsAssignableFrom(component.GetType()))
					result.Add((T)component);
			return result;
		}
	}
	#endregion
}
