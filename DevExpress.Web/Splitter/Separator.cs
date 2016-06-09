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

using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Utils;
using System;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class SplitterSeparators: PropertiesBase {
		bool isSeparator;
		SplitterSeparatorStyle style;
		SplitterSeparatorStyle collapsedStyle;
		SplitterSeparatorButtonStyle buttonStyle;
		HottrackedImageProperties image;
		HottrackedImageProperties forwardCollapseButtonImage;
		HottrackedImageProperties backwardCollapseButtonImage;
		public SplitterSeparators(SplitterPane owner)
			: this(owner, false) {			
		}
		public SplitterSeparators(SplitterPane owner, bool isSeparator)
			: base(owner) {
			this.isSeparator = isSeparator;
			this.buttonStyle = new SplitterSeparatorButtonStyle();
			this.style = new SplitterSeparatorStyle();
			this.collapsedStyle = new SplitterSeparatorStyle();
			this.image = new HottrackedImageProperties();
			this.forwardCollapseButtonImage = new HottrackedImageProperties();
			this.backwardCollapseButtonImage = new HottrackedImageProperties();
		}			 
		protected internal bool IsVertical {
			get { return Orientation == Orientation.Vertical; }
		}
		protected internal Orientation Orientation { get { return (isSeparator ? Owner.Parent : Owner).Orientation; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SplitterPane Owner { get { return (SplitterPane)base.Owner; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsSize"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(Unit), "")]
		public Unit Size {
			get { return GetUnitProperty("Size", Unit.Empty); }
			set {
				SplitterRenderHelper.CheckSizeType(value, false, false, false, "Size");
				SetUnitProperty("Size", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsVisible"),
#endif
		NotifyParentProperty(true), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean Visible {
			get { return GetDefaultBooleanProperty("Visible", DefaultBoolean.Default); }
			set { SetDefaultBooleanProperty("Visible", DefaultBoolean.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsSeparatorStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyle SeparatorStyle { get { return style; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsCollapsedStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorStyle CollapsedStyle { get { return collapsedStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsButtonStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SplitterSeparatorButtonStyle ButtonStyle { get { return buttonStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HottrackedImageProperties Image { get { return image; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsBackwardCollapseButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HottrackedImageProperties BackwardCollapseButtonImage { get { return backwardCollapseButtonImage; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterSeparatorsForwardCollapseButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public HottrackedImageProperties ForwardCollapseButtonImage { get { return forwardCollapseButtonImage; } }
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ButtonStyle, SeparatorStyle, CollapsedStyle, Image, ForwardCollapseButtonImage, BackwardCollapseButtonImage };
		}
		public override void Assign(PropertiesBase source) {
			if(source is SplitterSeparators) {
				SplitterSeparators src = (SplitterSeparators)source;
				Size = src.Size;
				Visible = src.Visible;
				ButtonStyle.Assign(src.ButtonStyle);
				SeparatorStyle.Assign(src.SeparatorStyle);
				CollapsedStyle.Assign(src.CollapsedStyle);
				Image.Assign(src.Image);
				BackwardCollapseButtonImage.Assign(src.BackwardCollapseButtonImage);
				ForwardCollapseButtonImage.Assign(src.ForwardCollapseButtonImage);
			}
			base.Assign(source);
		}		
	}
}
