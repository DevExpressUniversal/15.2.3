#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon {
	public class FormatConditionBarOptions {
		const string XmlShowBarOnly = "ShowBarOnly";
		const string XmlAllowNegativeAxis = "AllowNegativeAxis";
		const string XmlDrawAxis = "DrawAxis";
		const bool DefaultShowBarOnly = false;
		const bool DefaultAllowNegativeAxis = true;
		const bool DefaultDrawAxis = false;
		bool showBarOnly = DefaultShowBarOnly;
		bool allowNegativeAxis = DefaultAllowNegativeAxis;
		bool drawAxis = DefaultDrawAxis;
		IFormatStyleSettingsOwner owner;
		int lockUpdate;
		[
		DefaultValue(DefaultShowBarOnly)
		]
		public bool ShowBarOnly {
			get { return showBarOnly; }
			set {
				if(ShowBarOnly != value) {
					showBarOnly = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultAllowNegativeAxis)
		]
		public bool AllowNegativeAxis {
			get { return allowNegativeAxis; }
			set {
				if(AllowNegativeAxis != value) {
					allowNegativeAxis = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultDrawAxis)
		]
		public bool DrawAxis {
			get { return drawAxis; }
			set {
				if(DrawAxis != value) {
					drawAxis = value;
					OnChanged();
				}
			}
		}
		internal IFormatStyleSettingsOwner Owner {
			get { return owner; }
			set { this.owner = value; }
		}
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0) {
				OnChanged();
			}
		}
		public void Assign(FormatConditionBarOptions obj) {
			BeginUpdate();
			try {
				AllowNegativeAxis = obj.AllowNegativeAxis;
				DrawAxis = obj.DrawAxis;
				ShowBarOnly = obj.ShowBarOnly;
			}
			finally {
				EndUpdate();
			}
		}
		internal BarOptionsModel CreateModel() {
			return new BarOptionsModel() {
				AllowNegativeAxis = AllowNegativeAxis,
				DrawAxis = DrawAxis,
				ShowBarOnly = ShowBarOnly
			};
		}
		protected internal virtual void OnChanged() {
			if(Owner != null)
				Owner.OnChanged();
		}
		protected internal void SaveToXml(XElement element) {
			XmlHelper.Save(element, XmlShowBarOnly, ShowBarOnly, DefaultShowBarOnly);
			XmlHelper.Save(element, XmlAllowNegativeAxis, AllowNegativeAxis, DefaultAllowNegativeAxis);
			XmlHelper.Save(element, XmlDrawAxis, DrawAxis, DefaultDrawAxis);
		}
		protected internal void LoadFromXml(XElement element) {
			XmlHelper.Load<bool>(element, XmlShowBarOnly, x => showBarOnly = x);
			XmlHelper.Load<bool>(element, XmlAllowNegativeAxis, x => allowNegativeAxis = x);
			XmlHelper.Load<bool>(element, XmlDrawAxis, x => drawAxis = x);
		}
	}
}
