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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.UI {
	[ListBindable(BindableSupport.No)]
	[TypeConverter(typeof(CollectionTypeConverter))]
	public class FormattingRuleCollection : Collection<FormattingRule> {
		#region Inner Classes
		class OverlayStyleCalculator {
			readonly XRControlStyle style;
			readonly XRControlStyle overlayedStyle;
			public OverlayStyleCalculator(XRControlStyle style, XRControlStyle overlayedStyle) {
				this.style = style;
				this.overlayedStyle = overlayedStyle;
			}
			public void OverlayStyle() {
				style.ChangeStyle(CalculateProperty);
			}
			void CalculateProperty(StyleProperty property, XRControlStyle result) {
				if(BrickStyle.PropertyIsSet(overlayedStyle, property))
					overlayedStyle.CopyProperties(result, property);
			}
		}
		#endregion
		#region Fields & Properties
		readonly XRControl owner;
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingRuleCollectionStyle")]
#endif
		[Obsolete("The Style property is now obsolete. Use the GetEffectiveStyle method instead.")]
		public XRControlStyle Style {
			get { return GetEffectiveStyle(); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("FormattingRuleCollectionVisible")]
#endif
		[Obsolete("The Visible property is now obsolete. Use the GetEffectiveVisible method instead.")]
		public DefaultBoolean Visible {
			get { return GetEffectiveVisible(); }
		}
		#endregion
		#region Constructors
		public FormattingRuleCollection(XRControl owner) {
			this.owner = owner;
		}
		#endregion
		#region Methods
		internal static void OverlayStyle(XRControlStyle style, XRControlStyle overlayedStyle) {
			if(overlayedStyle != null)
				new OverlayStyleCalculator(style, overlayedStyle).OverlayStyle();
		}
		public XRControlStyle GetEffectiveStyle() {
			XRControlStyle resultStyle = new XRControlStyle();
			if(Count > 0 && owner.Report != null)
				foreach(FormattingRule rule in this)
					if(rule.EvaluateCondition(owner.Report.DataContext))
						OverlayStyle(resultStyle, rule.Formatting);
			return resultStyle;
		}
		internal bool TryGetEffectiveStyle(out XRControlStyle style) {
			if(Count > 0 && owner.Report != null) {
				style = new XRControlStyle();
				foreach(FormattingRule rule in this)
					if(rule.EvaluateCondition(owner.Report.DataContext))
						OverlayStyle(style, rule.Formatting);
				return true;
			}
			style = null;
			return false;
		}
		public DefaultBoolean GetEffectiveVisible() {
			if(owner.Report != null) {
				for(int i = this.Count - 1; i >= 0; i--)
					if(this[i].Formatting.Visible != DefaultBoolean.Default && this[i].EvaluateCondition(owner.Report.DataContext))
						return this[i].Formatting.Visible;
			}
			return DefaultBoolean.Default;
		}
		public override bool Equals(object obj) {
			FormattingRuleCollection another = obj as FormattingRuleCollection;
			if(!object.ReferenceEquals(another, null) && another.Count == this.Count) {
				for(int i = 0; i < another.Count; i++)
					if(another[i] != this[i])
						return false;
				return true;
			}
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void AddRange(IEnumerable<FormattingRule> items) {
			foreach(FormattingRule item in items)
				Add(item);
		}
		internal void Add(string name) {
			FormattingRule formattingRule = owner.RootReport.FormattingRuleSheet[name];
			Add(formattingRule);
		}
		protected override void InsertItem(int index, FormattingRule item) {
			base.InsertItem(index, item);
			item.Disposed += item_Disposed;
		}
		protected override void RemoveItem(int index) {
			this[index].Disposed -= item_Disposed;
			base.RemoveItem(index);
		}
		protected override void ClearItems() {
			foreach(FormattingRule item in this)
				item.Disposed -= item_Disposed;
			base.ClearItems();
		}
		void item_Disposed(object sender, EventArgs e) {
			Remove((FormattingRule)sender);
		}
		#endregion
	}
}
