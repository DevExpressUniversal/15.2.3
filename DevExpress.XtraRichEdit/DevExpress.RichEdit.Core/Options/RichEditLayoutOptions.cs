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
using System.Runtime.InteropServices;
using DevExpress.Utils.Controls;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region RichEditLayoutOptions
	[ComVisible(true)]
	public class RichEditLayoutOptions : RichEditNotificationOptions {
		#region Fields
		bool allowTablesToExtendIntoMargins; 
		DraftViewLayoutOptions draftView;
		PrintLayoutViewLayoutOptions printLayoutView;
		SimpleViewLayoutOptions simpleView;
		#endregion
		public RichEditLayoutOptions() {
			draftView.Changed += OnInnerOptionsChanged;
			printLayoutView.Changed += OnInnerOptionsChanged;
			simpleView.Changed += OnInnerOptionsChanged;
		}
		protected internal virtual void OnInnerOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
		#region Properties
		[Obsolete("You should use the 'AllowTablesToExtendIntoMargins' property for a particular RichEditView instead", true)]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowTablesToExtendIntoMargins {
			get { return allowTablesToExtendIntoMargins; }
			set {
				if (this.allowTablesToExtendIntoMargins == value)
					return;
				bool oldValue = this.allowTablesToExtendIntoMargins;
				this.allowTablesToExtendIntoMargins = value;
				OnChanged("AllowTablesToExtendIntoMargins", oldValue, value);
			}
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditLayoutOptionsDraftView"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DraftViewLayoutOptions DraftView { get { return this.draftView; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditLayoutOptionsPrintLayoutView"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrintLayoutViewLayoutOptions PrintLayoutView { get { return this.printLayoutView; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditLayoutOptionsSimpleView"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SimpleViewLayoutOptions SimpleView { get { return this.simpleView; } }
		#endregion
		protected override void CreateInnerOptions() {
			this.draftView = CreateDraftViewLayoutOptions();
			this.printLayoutView = CreatePrintLayoutViewLayoutOptions();
			this.simpleView = CreateSimpleViewLayoutOptions();
		}
		protected internal virtual DraftViewLayoutOptions CreateDraftViewLayoutOptions() {
			return new DraftViewLayoutOptions();
		}
		protected internal virtual PrintLayoutViewLayoutOptions CreatePrintLayoutViewLayoutOptions() {
			return new PrintLayoutViewLayoutOptions();
		}
		protected internal virtual SimpleViewLayoutOptions CreateSimpleViewLayoutOptions() {
			return new SimpleViewLayoutOptions();
		}
		protected internal override void ResetCore() {
			this.DraftView.ResetCore();
			this.PrintLayoutView.ResetCore();
			this.SimpleView.ResetCore();
		}
	}
	#endregion
	#region ViewLayoutOptionsBase (abstract class)
	[ComVisible(true)]
	public abstract class ViewLayoutOptionsBase : RichEditNotificationOptions {
		internal static readonly string AllowTablesToExtendIntoMarginsPropertyName = "AllowTablesToExtendIntoMargins";
		internal static readonly string MatchHorizontalTableIndentsToTextEdgePropertyName = "MatchHorizontalTableIndentsToTextEdge";
		bool allowTablesToExtendIntoMargins;
		bool matchHorizontalTableIndentsToTextEdge;
		public virtual bool AllowTablesToExtendIntoMargins {
			get { return allowTablesToExtendIntoMargins; }
			set {
				if (allowTablesToExtendIntoMargins == value)
					return;
				bool oldValue = this.allowTablesToExtendIntoMargins;
				allowTablesToExtendIntoMargins = value;
				OnChanged(AllowTablesToExtendIntoMarginsPropertyName, oldValue, value);
			}
		}
		public virtual bool MatchHorizontalTableIndentsToTextEdge {
			get { return matchHorizontalTableIndentsToTextEdge; }
			set {
				if (matchHorizontalTableIndentsToTextEdge == value)
					return;
				bool oldValue = this.matchHorizontalTableIndentsToTextEdge;
				matchHorizontalTableIndentsToTextEdge = value;
				OnChanged(MatchHorizontalTableIndentsToTextEdgePropertyName, oldValue, value);
			}
		}
		protected internal override void ResetCore() {
			this.AllowTablesToExtendIntoMargins = GetDefaultAllowTablesToExtendIntoMargins();
			this.MatchHorizontalTableIndentsToTextEdge = GetDefaultMatchHorizontalTableIndentsToTextEdge();
		}
		protected internal abstract bool GetDefaultAllowTablesToExtendIntoMargins();
		protected internal abstract bool GetDefaultMatchHorizontalTableIndentsToTextEdge();
	}
	#endregion
	#region DraftViewLayoutOptions
	[ComVisible(true)]
	public class DraftViewLayoutOptions : ViewLayoutOptionsBase {
		#region Fields
		const bool allowTablesToExtendIntoMarginsByDefault = true;
		const bool matchHorizontalTableIndentsToTextEdgeDefault = false;
		#endregion
		#region Properties
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DraftViewLayoutOptionsAllowTablesToExtendIntoMargins"),
#endif
 DefaultValue(allowTablesToExtendIntoMarginsByDefault), NotifyParentProperty(true)]
		public override bool AllowTablesToExtendIntoMargins { get { return base.AllowTablesToExtendIntoMargins; } set { base.AllowTablesToExtendIntoMargins = value; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("DraftViewLayoutOptionsMatchHorizontalTableIndentsToTextEdge"),
#endif
		DefaultValue(matchHorizontalTableIndentsToTextEdgeDefault), NotifyParentProperty(true)]
		public override bool MatchHorizontalTableIndentsToTextEdge { get { return base.MatchHorizontalTableIndentsToTextEdge; } set { base.MatchHorizontalTableIndentsToTextEdge = value; } }
		#endregion
		protected internal override bool GetDefaultAllowTablesToExtendIntoMargins() {
			return allowTablesToExtendIntoMarginsByDefault;
		}
		protected internal override bool GetDefaultMatchHorizontalTableIndentsToTextEdge() {
			return matchHorizontalTableIndentsToTextEdgeDefault;
		}
	}
	#endregion
	#region PrintLayoutViewLayoutOptions
	[ComVisible(true)]
	public class PrintLayoutViewLayoutOptions : ViewLayoutOptionsBase {
		#region Fields
		const bool allowTablesToExtendIntoMarginsByDefault = false;
		const bool matchHorizontalTableIndentsToTextEdgeDefault = false;
		#endregion
		#region Properties
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintLayoutViewLayoutOptionsAllowTablesToExtendIntoMargins"),
#endif
 DefaultValue(allowTablesToExtendIntoMarginsByDefault), NotifyParentProperty(true)]
		public override bool AllowTablesToExtendIntoMargins { get { return base.AllowTablesToExtendIntoMargins; } set { base.AllowTablesToExtendIntoMargins = value; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("PrintLayoutViewLayoutOptionsMatchHorizontalTableIndentsToTextEdge"),
#endif
		DefaultValue(matchHorizontalTableIndentsToTextEdgeDefault), NotifyParentProperty(true)]
		public override bool MatchHorizontalTableIndentsToTextEdge { get { return base.MatchHorizontalTableIndentsToTextEdge; } set { base.MatchHorizontalTableIndentsToTextEdge = value; } }
		#endregion
		protected internal override bool GetDefaultAllowTablesToExtendIntoMargins() {
			return allowTablesToExtendIntoMarginsByDefault;
		}
		protected internal override bool GetDefaultMatchHorizontalTableIndentsToTextEdge() {
			return matchHorizontalTableIndentsToTextEdgeDefault;
		}
	}
	#endregion
	#region SimpleViewLayoutOptions
	[ComVisible(true)]
	public class SimpleViewLayoutOptions : ViewLayoutOptionsBase {
		#region Fields
		const bool allowTablesToExtendIntoMarginsByDefault = true;
		const bool matchHorizontalTableIndentsToTextEdgeDefault = true;
		#endregion
		#region Properties
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("SimpleViewLayoutOptionsAllowTablesToExtendIntoMargins"),
#endif
 DefaultValue(allowTablesToExtendIntoMarginsByDefault), NotifyParentProperty(true)]
		public override bool AllowTablesToExtendIntoMargins { get { return base.AllowTablesToExtendIntoMargins; } set { base.AllowTablesToExtendIntoMargins = value; } }
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("SimpleViewLayoutOptionsMatchHorizontalTableIndentsToTextEdge"),
#endif
		DefaultValue(matchHorizontalTableIndentsToTextEdgeDefault), NotifyParentProperty(true)]
		public override bool MatchHorizontalTableIndentsToTextEdge { get { return base.MatchHorizontalTableIndentsToTextEdge; } set { base.MatchHorizontalTableIndentsToTextEdge = value; } }
		#endregion
		protected internal override bool GetDefaultAllowTablesToExtendIntoMargins() {
			return allowTablesToExtendIntoMarginsByDefault;
		}
		protected internal override bool GetDefaultMatchHorizontalTableIndentsToTextEdge() {
			return matchHorizontalTableIndentsToTextEdgeDefault;
		}
	}
	#endregion
}
