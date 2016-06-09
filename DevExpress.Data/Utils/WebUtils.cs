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
namespace DevExpress.WebUtils {
	public interface IViewBagOwner {
		T GetViewBagProperty<T>(string objectPath, string propertyName, T value);
		void SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value);
	}
	public class ViewStatePersisterCore {
		IViewBagOwner viewBagOwner;
		string objectPath;
		public ViewStatePersisterCore() : this(null) {}
		public ViewStatePersisterCore(IViewBagOwner viewBagOwner) : this(viewBagOwner, string.Empty) { }
		public ViewStatePersisterCore(IViewBagOwner viewBagOwner, string objectPath) {
			this.viewBagOwner = viewBagOwner;
			this.objectPath = objectPath;
		}
		protected IViewBagOwner ViewBagOwner { get { return viewBagOwner; } }
		protected virtual string ViewBagObjectPath { get { return objectPath != null ? objectPath : this.GetType().Name; } }
		protected virtual T GetViewBagProperty<T>(string name, T value) {
			if(viewBagOwner == null) return value;
			return (T)viewBagOwner.GetViewBagProperty(ViewBagObjectPath, name, value);
		}
		protected virtual void SetViewBagProperty<T>(string name, T defaultValue, T value) {
			if(viewBagOwner == null) return;
			viewBagOwner.SetViewBagProperty(ViewBagObjectPath, name, defaultValue, value);
		}
	}
}
