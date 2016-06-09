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
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Bars {
	public class ObjectFactory {
		public ObjectFactory() {
			creatorsList = new List<ObjectCreator>();
		}
		List<ObjectCreator> creatorsList;
		public List<ObjectCreator> CreatorsList { get { return creatorsList; } }
		public object CreateObject(string typeName) {
			foreach(ObjectCreator c in CreatorsList) {
				object obj = c.Create(typeName);
				if(obj != null) return obj;
			}
			return null;
		}
	}
	public class BarFactory : ObjectFactory { 
		static BarFactory defaultFactory;
		public static BarFactory Default {
			get {
				if(defaultFactory == null) 
					defaultFactory = new BarFactory();
				return defaultFactory;
			}
		}
		public BarFactory() {
			CreatorsList.Add(new BarCreator());
		}
	}
	public class ItemLinkFactory : ObjectFactory { 
		static ItemLinkFactory defaultFactory;
		public static ItemLinkFactory Default {
			get {
				if(defaultFactory == null) 
					defaultFactory = new ItemLinkFactory();
				return defaultFactory;
			}
		}
		public ItemLinkFactory() {
			CreatorsList.Add(new ItemLinkCreator());
		}
	}
	public class ObjectCreator {
		protected virtual System.Reflection.Assembly GetAssembly() {
			return System.Reflection.Assembly.GetExecutingAssembly();
		}
		public virtual object Create(string typeName) {
			System.Reflection.Assembly asm = GetAssembly();
			if(asm == null) return null;
			Type tp = asm.GetType(typeName);
			if(tp == null) return null;
			System.Reflection.ConstructorInfo info = tp.GetConstructor(System.Type.EmptyTypes);
			if(info == null) return null;
			return info.Invoke(null);
		}
	}
	public class BarCreator : ObjectCreator {
	}
	public class ItemLinkCreator : ObjectCreator {
	}
}
