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
using System.ComponentModel;
namespace DevExpress.Xpf.Gauges {
	public class PredefinedElementKind {
		Type type;
		string name;
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("PredefinedElementKindType")]
#endif
		public Type Type { get { return type; } }
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("PredefinedElementKindName")]
#endif
		public string Name { get { return name; } }
		internal PredefinedElementKind(Type type, string name) {
			this.type = type;
			this.name = name;
		}
		public override string ToString() {
			return name;
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class PredefinedElementKinds {
		static void Add(List<PredefinedElementKind> kindList, Type type) {
			INamedElement namedElement = Activator.CreateInstance(type) as INamedElement;
			if (namedElement != null)
				kindList.Add(new PredefinedElementKind(type, namedElement.Name));
		}
		protected static void FillKindList(List<PredefinedElementKind> kindList, Type baseType) {
			foreach (Type currentType in baseType.Assembly.GetTypes()) 
				if (!currentType.IsAbstract && currentType.IsSubclassOf(baseType))
					Add(kindList, currentType);			
		}
	}
	public class PredefinedScaleLabelPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedScaleLabelPresentations() {
			FillKindList(kinds, typeof(PredefinedScaleLabelPresentation));
		}
	}
	public class PredefinedTickmarksPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedTickmarksPresentations() {
			FillKindList(kinds, typeof(PredefinedTickmarksPresentation));
		}
	}
	public class PredefinedCircularGaugeModels : PredefinedElementKinds{
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> ModelKinds { get { return kinds; } }
		static PredefinedCircularGaugeModels() {
			FillKindList(kinds, typeof(CircularGaugeModel));
		}
	}
	public class PredefinedSpindleCapPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedSpindleCapPresentations() {
			FillKindList(kinds, typeof(PredefinedSpindleCapPresentation));
		}
	}
	public class PredefinedArcScaleLinePresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedArcScaleLinePresentations() {
			FillKindList(kinds, typeof(PredefinedArcScaleLinePresentation));
		}
	}
	public class PredefinedArcScaleNeedlePresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedArcScaleNeedlePresentations() {
			FillKindList(kinds, typeof(PredefinedArcScaleNeedlePresentation));
		}
	}
	public class PredefinedArcScaleMarkerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedArcScaleMarkerPresentations() {
			FillKindList(kinds, typeof(PredefinedArcScaleMarkerPresentation));
		}
	}
	public class PredefinedArcScaleRangeBarPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedArcScaleRangeBarPresentations() {
			FillKindList(kinds, typeof(PredefinedArcScaleRangeBarPresentation));
		}
	}
	public class PredefinedCircularGaugeLayerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedCircularGaugeLayerPresentations() {
			FillKindList(kinds, typeof(PredefinedCircularGaugeLayerPresentation));
		}
	}
	public class PredefinedArcScaleLayerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedArcScaleLayerPresentations() {
			FillKindList(kinds, typeof(PredefinedArcScaleLayerPresentation));
		}
	}
	public class PredefinedArcScaleRangePresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedArcScaleRangePresentations() {
			FillKindList(kinds, typeof(PredefinedArcScaleRangePresentation));
		}
	}
	public class PredefinedLinearGaugeModels : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> ModelKinds { get { return kinds; } }
		static PredefinedLinearGaugeModels() {
			FillKindList(kinds, typeof(LinearGaugeModel));
		}
	}
	public class PredefinedLinearScaleMarkerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearScaleMarkerPresentations() {
			FillKindList(kinds, typeof(PredefinedLinearScaleMarkerPresentation));
		}
	}
	public class PredefinedLinearScaleLinePresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearScaleLinePresentations() {
			FillKindList(kinds, typeof(PredefinedLinearScaleLinePresentation));
		}
	}
	public class PredefinedLinearScaleRangeBarPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearScaleRangeBarPresentations() {
			FillKindList(kinds, typeof(PredefinedLinearScaleRangeBarPresentation));
		}
	}
	public class PredefinedLinearScaleLevelBarPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearScaleLevelBarPresentations() {
			FillKindList(kinds, typeof(PredefinedLinearScaleLevelBarPresentation));
		}
	}
	public class PredefinedLinearGaugeLayerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearGaugeLayerPresentations() {
			FillKindList(kinds, typeof(PredefinedLinearGaugeLayerPresentation));
		}
	}
	public class PredefinedLinearScaleLayerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearScaleLayerPresentations() {
			FillKindList(kinds, typeof(PredefinedLinearScaleLayerPresentation));
		}
	}
	public class PredefinedLinearScaleRangePresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedLinearScaleRangePresentations() {
			FillKindList(kinds, typeof(PredefinedLinearScaleRangePresentation));
		}
	}
	public class PredefinedDigitalGaugeLayerPresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedDigitalGaugeLayerPresentations() {
			FillKindList(kinds, typeof(PredefinedDigitalGaugeLayerPresentation));
		}
	}
	public class PredefinedDigitalGaugeModels : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> ModelKinds { get { return kinds; } }
		static PredefinedDigitalGaugeModels() {
			FillKindList(kinds, typeof(DigitalGaugeModel));
		}
	}
	public class PredefinedStatePresentations : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> PresentationKinds { get { return kinds; } }
		static PredefinedStatePresentations() {
			FillKindList(kinds, typeof(PredefinedStatePresentation));
		}
	}
	public class PredefinedStateIndicatorModels : PredefinedElementKinds {
		static List<PredefinedElementKind> kinds = new List<PredefinedElementKind>();
		public static List<PredefinedElementKind> ModelKinds { get { return kinds; } }
		static PredefinedStateIndicatorModels() {
			FillKindList(kinds, typeof(StateIndicatorModel));
		}
	}
}
