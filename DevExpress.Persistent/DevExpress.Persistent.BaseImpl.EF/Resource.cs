#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.BaseImpl.EF {
	[DefaultProperty("Caption")]
	public class Resource : IResource, IXafEntityObject {
		public Resource() {
			Events = new List<Event>();
		}
		[Key, VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
		public Int32 Key { get; protected set; }
		public String Caption { get; set; }
		[Browsable(false)]
		public Int32 Color_Int { get; protected set; }
		public virtual IList<Event> Events { get; set; }
		[NotMapped, VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
		public Object Id {
			get { return Key; }
		}
		[NotMapped, VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
		public Int32 OleColor {
			get {
				return ColorTranslator.ToOle(Color.FromArgb(Color_Int));
			}
		}
		[NotMapped]
		public Color Color {
			get { return Color.FromArgb(Color_Int); }
			set { Color_Int = value.ToArgb(); }
		}
		void IXafEntityObject.OnCreated()
		{
		}
		void IXafEntityObject.OnSaving()
		{
		}
		void IXafEntityObject.OnLoaded()
		{
			Int32 count = Events.Count;
		}
	}
}
