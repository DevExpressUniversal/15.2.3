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
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Design {
	class RedirectedPropertyDescriptor : PropertyDescriptor {
		object sourceObject;
		PropertyDescriptor sourceDescriptor;
		public RedirectedPropertyDescriptor(object sourceObject, PropertyDescriptor sourceDescriptor)
			: base(sourceDescriptor) {
			this.sourceDescriptor = sourceDescriptor;
			this.sourceObject = sourceObject;
		}
		public override bool CanResetValue(object component) { return sourceDescriptor.CanResetValue(sourceObject); }
		public override Type ComponentType {
			get { return sourceObject.GetType(); }
		}
		public override object GetValue(object component) { return sourceDescriptor.GetValue(sourceObject); }
		public override bool IsReadOnly { get { return sourceDescriptor.IsReadOnly; } }
		public override Type PropertyType { get { return sourceDescriptor.PropertyType; } }
		public override void ResetValue(object component) {
			sourceDescriptor.ResetValue(sourceObject);
		}
		public override void SetValue(object component, object value) {
			sourceDescriptor.SetValue(sourceObject, value);
		}
		public override bool ShouldSerializeValue(object component) { return false; }
	}
	public class BarHeaderItemLinkInfoProvider : BarLinkInfoProvider {
		public BarHeaderItemLinkInfoProvider(BarItemLink link) : base(link) { }
		[Browsable(false)]
		public override bool BeginGroup {
			get {
				return base.BeginGroup;
			}
			set {
				base.BeginGroup = value;
			}
		}
		[Browsable(false)]
		public override bool ActAsButtonGroup {
			get {
				return base.ActAsButtonGroup;
			}
			set {
				base.ActAsButtonGroup = value;
			}
		}
		[Browsable(false)]
		public override string KeyTip {
			get {
				return base.KeyTip;
			}
			set {
				base.KeyTip = value;
			}
		}
	}
	public class BarLinkInfoProvider {
		BarItemLink link;
		public static void Reset(BarItems items) {
			foreach(BarItem item in items) {
				item.LinkProvider = null;
			}
		}
		public static BarLinkInfoProvider GetLinkInfo(BarItem item) {
			return item == null ? null : item.LinkProvider;
		}
		public static void SetLinkInfo(BarItem item, BarItemLink link) {
			if(item != null) item.LinkProvider = item.CreateLinkInfoProvider(link);
		}
		public BarLinkInfoProvider(BarItemLink link) {
			this.link = link;
		}
		[Browsable(false)]
		public virtual BarItemLink Link {
			get { return link; }
			set {
				link = value;
			}
		}
		[Category("Link settings"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkBeginGroup"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual bool BeginGroup {
			get {
				return Link == null ? false : Link.BeginGroup;
			}
			set {
				if(Link == null) return;
				Link.BeginGroup = value;
			}
		}
		[Category("Link settings"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkActAsButtonGroup"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual bool ActAsButtonGroup {
			get {
				return Link == null ? false : Link.ActAsButtonGroup;
			}
			set {
				if(Link == null) return;
				Link.ActAsButtonGroup = value;
			}
		}
		[Category("Link settings"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarItemLinkKeyTip"),
#endif
 RefreshProperties(RefreshProperties.All)]
		public virtual string KeyTip {
			get {
				return Link == null ? string.Empty : Link.KeyTip;
			}
			set {
				if(Link == null) return;
				Link.KeyTip = value;
			}
		}
	}
	public class RibbonExpandColldapeItemLinkInfoProvider : BarLinkInfoProvider {
		public RibbonExpandColldapeItemLinkInfoProvider(BarItemLink link) : base(link) { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsButtonGroup {
			get { return base.ActAsButtonGroup; }
			set { base.ActAsButtonGroup = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string KeyTip {
			get { return base.KeyTip; }
			set { base.KeyTip = value; }
		}
	}
}
