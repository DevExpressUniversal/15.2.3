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
using System.Collections;
using DevExpress.XtraPrinting.Native;
using System.Windows.Forms;
namespace DevExpress.XtraReports.Design {
	public class XRRichTextDesigner : XRRichTextBaseDesigner {
		XRRichText XRRichText {
			get { return (XRRichText)Component; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			XRControl parent = FindParent();
			if(parent != null)
				XRControl.Font = parent.GetEffectiveFont();
		}
		protected override InplaceTextEditorBase GetInplaceEditor(string text, bool selectAll) {
			return new InplaceDXRichTextEditor(fDesignerHost, XRRichText, text, selectAll);
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
			list.Add(new XRControlBookmarkDesignerActionList(this));
			list.Add(new XRFormattingControlDesignerActionList(this));
			list.Add(new XRRichTextDesignerActionList3(this));
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			properties["Rtf"] = new RtfPropertyDescriptor((PropertyDescriptor)properties["Rtf"]);
		}
	}
	class RtfPropertyDescriptor : WrappedPropertyDescriptor {
		public RtfPropertyDescriptor(PropertyDescriptor oldPropertyDescriptor) : base(oldPropertyDescriptor) {
		}
		public override object GetValue(object component) {
			return oldPropertyDescriptor.GetValue(component);
		}
		public override void SetValue(object component, object value) {
			ISite site = MemberDescriptor.GetSite(component);
			if(((XRRichText)component).Report == null || !CanWrapProperty(site)) {
				oldPropertyDescriptor.SetValue(component, value);
				return;
			}
			IComponentChangeService service = site != null ? (IComponentChangeService)site.GetService(typeof(IComponentChangeService)) : null;
			XRControlDesignerBase.RaiseComponentChanging(service, (XRRichText)component, oldPropertyDescriptor.Name);
			((XRRichText)component).SetDisplayPropertyFromTextWithDisplayColumnNames((string)value);
			XRControlDesignerBase.RaiseComponentChanged(service, (XRRichText)component, oldPropertyDescriptor.Name, null, ((XRRichText)component).Text);
		}
	}
}
