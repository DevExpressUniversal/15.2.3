#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public static class PdfUnicodeConverter {
		internal static readonly Dictionary<string, ushort> GlyphCodes = new Dictionary<string, ushort>() {
			{ PdfGlyphNames.A, 0x41 }, { PdfGlyphNames.AE, 0xc6 }, { PdfGlyphNames.AEacute, 0x1fc }, { PdfGlyphNames.Aacute, 0xc1 }, { PdfGlyphNames.Abreve, 0x102 }, { PdfGlyphNames.Acircumflex, 0xc2 }, 
			{ PdfGlyphNames.Adieresis, 0xc4 }, { PdfGlyphNames.Agrave, 0xc0 }, { PdfGlyphNames.Alpha, 0x391 }, { PdfGlyphNames.Alphatonos, 0x386 }, { PdfGlyphNames.Amacron, 0x100 }, 
			{ PdfGlyphNames.Aogonek, 0x104 }, { PdfGlyphNames.Aring, 0xc5 }, { PdfGlyphNames.Aringacute, 0x1fa }, { PdfGlyphNames.Atilde, 0xc3 }, { PdfGlyphNames.B, 0x42 }, { PdfGlyphNames.Beta, 0x392 }, 
			{ PdfGlyphNames.C, 0x43 }, { PdfGlyphNames.Cacute, 0x106 }, { PdfGlyphNames.Ccaron, 0x10c }, { PdfGlyphNames.Ccedilla, 0xc7 }, { PdfGlyphNames.Ccircumflex, 0x108 }, 
			{ PdfGlyphNames.Cdot, 0x10a }, { PdfGlyphNames.Chi, 0x3a7 }, { PdfGlyphNames.D, 0x44 }, { PdfGlyphNames.Dcaron, 0x10e }, { PdfGlyphNames.Dcroat, 0x110 }, { PdfGlyphNames.Delta, 0x394 }, 
			{ PdfGlyphNames.E, 0x45 }, { PdfGlyphNames.Ebreve, 0x114 }, { PdfGlyphNames.Ecaron, 0x11a }, { PdfGlyphNames.Emacron, 0x112 }, { PdfGlyphNames.Eacute, 0xc9 }, 
			{ PdfGlyphNames.Ecircumflex, 0xca }, { PdfGlyphNames.Edieresis, 0xcb }, { PdfGlyphNames.Edot, 0x116 }, { PdfGlyphNames.Egrave, 0xc8 }, { PdfGlyphNames.Eng, 0x14a }, 
			{ PdfGlyphNames.Eogonek, 0x118 }, { PdfGlyphNames.Epsilon, 0x395 }, { PdfGlyphNames.Epsilontonos, 0x388 }, { PdfGlyphNames.Eta, 0x397 }, { PdfGlyphNames.Etatonos, 0x389 }, 
			{ PdfGlyphNames.Eth, 0xd0 }, { PdfGlyphNames.Euro, 0x20ac }, { PdfGlyphNames.F, 0x46 }, { PdfGlyphNames.G, 0x47 }, { PdfGlyphNames.Gamma, 0x393 }, { PdfGlyphNames.Gbreve, 0x11e }, 
			{ PdfGlyphNames.Gcedilla, 0x122 }, { PdfGlyphNames.Gcircumflex, 0x11c }, { PdfGlyphNames.Gdot, 0x120 }, { PdfGlyphNames.H, 0x48 }, { PdfGlyphNames.H18533, 0x25cf }, 
			{ PdfGlyphNames.H18543, 0x25aa }, { PdfGlyphNames.H18551, 0x25ab }, { PdfGlyphNames.H22073, 0x25a1 }, { PdfGlyphNames.Hbar, 0x126 }, { PdfGlyphNames.Hcircumflex, 0x124 }, 
			{ PdfGlyphNames.I, 0x49 }, { PdfGlyphNames.IJ, 0x132 }, { PdfGlyphNames.Iacute, 0xcd }, { PdfGlyphNames.Ibreve, 0x12c }, { PdfGlyphNames.Icircumflex, 0xce }, { PdfGlyphNames.Idieresis, 0xcf }, 
			{ PdfGlyphNames.Idot, 0x130 }, { PdfGlyphNames.Idotaccent, 0x130 }, { PdfGlyphNames.Ifraktur, 0x2111 }, { PdfGlyphNames.Igrave, 0xcc }, { PdfGlyphNames.Imacron, 0x12a }, 
			{ PdfGlyphNames.Iogonek, 0x12e }, { PdfGlyphNames.Iota, 0x399 }, { PdfGlyphNames.Iotadieresis, 0x3aa }, { PdfGlyphNames.Iotatonos, 0x38a }, { PdfGlyphNames.Itilde, 0x128 }, 
			{ PdfGlyphNames.J, 0x4a }, { PdfGlyphNames.Jcircumflex, 0x134 }, { PdfGlyphNames.K, 0x4b }, { PdfGlyphNames.Kappa, 0x39a }, { PdfGlyphNames.Kcedilla, 0x136 }, { PdfGlyphNames.L, 0x4c }, 
			{ PdfGlyphNames.Lacute, 0x139 }, { PdfGlyphNames.Lambda, 0x39b }, { PdfGlyphNames.Lcaron, 0x13d }, { PdfGlyphNames.Lcedilla, 0x13b }, { PdfGlyphNames.Ldot, 0x13f }, 
			{ PdfGlyphNames.Lslash, 0x141 }, { PdfGlyphNames.M, 0x4d }, { PdfGlyphNames.Mu, 0x39c }, { PdfGlyphNames.N, 0x4e }, { PdfGlyphNames.Nacute, 0x143 }, { PdfGlyphNames.Ncaron, 0x147 }, 
			{ PdfGlyphNames.Ncedilla, 0x145 }, { PdfGlyphNames.Ntilde, 0xd1 }, { PdfGlyphNames.Nu, 0x39d }, { PdfGlyphNames.O, 0x4f }, { PdfGlyphNames.OE, 0x152 }, { PdfGlyphNames.Oacute, 0xd3 }, 
			{ PdfGlyphNames.Obreve, 0x14e }, { PdfGlyphNames.Ocircumflex, 0xd4 }, { PdfGlyphNames.Odblacute, 0x150 }, { PdfGlyphNames.Odieresis, 0xd6 }, { PdfGlyphNames.Ograve, 0xd2 }, 
			{ PdfGlyphNames.Ohm, 0x2126 }, { PdfGlyphNames.Omacron, 0x14c }, { PdfGlyphNames.Omega, 0x3a9 }, { PdfGlyphNames.Omegatonos, 0x38f }, { PdfGlyphNames.Omicron, 0x39f }, 
			{ PdfGlyphNames.Omicrontonos, 0x38c }, { PdfGlyphNames.Oslash, 0xd8 }, { PdfGlyphNames.Oslashacute, 0x1fe }, { PdfGlyphNames.Otilde, 0xd5 }, { PdfGlyphNames.P, 0x50 }, 
			{ PdfGlyphNames.Phi, 0x3a6 }, { PdfGlyphNames.Pi, 0x3a0 }, { PdfGlyphNames.Psi, 0x3a8 }, { PdfGlyphNames.Q, 0x51 }, { PdfGlyphNames.R, 0x52 }, { PdfGlyphNames.Racute, 0x154 }, 
			{ PdfGlyphNames.Rcaron, 0x158 }, { PdfGlyphNames.Rcedilla, 0x156 }, { PdfGlyphNames.Rfraktur, 0x211c }, { PdfGlyphNames.Rho, 0x3a1 }, { PdfGlyphNames.S, 0x53 }, 
			{ PdfGlyphNames.SF010000, 0x250c }, { PdfGlyphNames.SF020000, 0x2514 }, { PdfGlyphNames.SF030000, 0x2510 }, { PdfGlyphNames.SF040000, 0x2518 },  { PdfGlyphNames.SF050000, 0x253c }, 
			{ PdfGlyphNames.SF060000, 0x252c }, { PdfGlyphNames.SF070000, 0x2534 }, { PdfGlyphNames.SF080000, 0x251c }, { PdfGlyphNames.SF090000, 0x2524 }, { PdfGlyphNames.SF100000, 0x2500 }, 
			{ PdfGlyphNames.SF110000, 0x2502 }, { PdfGlyphNames.SF190000, 0x2561 }, { PdfGlyphNames.SF200000, 0x2562 }, { PdfGlyphNames.SF210000, 0x2556 }, { PdfGlyphNames.SF220000, 0x2555 }, 
			{ PdfGlyphNames.SF230000, 0x2563 }, { PdfGlyphNames.SF240000, 0x2551 }, { PdfGlyphNames.SF250000, 0x2557 }, { PdfGlyphNames.SF260000, 0x255d }, { PdfGlyphNames.SF270000, 0x255c }, 
			{ PdfGlyphNames.SF280000, 0x255b }, { PdfGlyphNames.SF360000, 0x255e }, { PdfGlyphNames.SF370000, 0x255f }, { PdfGlyphNames.SF380000, 0x255a }, { PdfGlyphNames.SF390000, 0x2554 }, 
			{ PdfGlyphNames.SF400000, 0x2569 }, { PdfGlyphNames.SF410000, 0x2566 }, { PdfGlyphNames.SF420000, 0x2560 }, { PdfGlyphNames.SF430000, 0x2550 }, { PdfGlyphNames.SF440000, 0x256c }, 
			{ PdfGlyphNames.SF450000, 0x2567 }, { PdfGlyphNames.SF460000, 0x2568 }, { PdfGlyphNames.SF470000, 0x2564 }, { PdfGlyphNames.SF480000, 0x2565 }, { PdfGlyphNames.SF490000, 0x2559 }, 
			{ PdfGlyphNames.SF500000, 0x2558 }, { PdfGlyphNames.SF510000, 0x2552 }, { PdfGlyphNames.SF520000, 0x2553 }, { PdfGlyphNames.SF530000, 0x256b }, { PdfGlyphNames.SF540000, 0x256a }, 
			{ PdfGlyphNames.Sacute, 0x15a }, { PdfGlyphNames.Scaron, 0x160 }, { PdfGlyphNames.Scedilla, 0x15e }, { PdfGlyphNames.Scircumflex, 0x15c }, { PdfGlyphNames.Sigma, 0x3a3 }, 
			{ PdfGlyphNames.T, 0x54 }, { PdfGlyphNames.Tau, 0x3a4 }, { PdfGlyphNames.Tbar, 0x166 }, { PdfGlyphNames.Tcaron, 0x164 }, { PdfGlyphNames.Tcedilla, 0x162 }, { PdfGlyphNames.Theta, 0x398 }, 
			{ PdfGlyphNames.Thorn, 0xde }, { PdfGlyphNames.U, 0x55 }, { PdfGlyphNames.Uacute, 0xda }, { PdfGlyphNames.Ubreve, 0x16c }, { PdfGlyphNames.Ucircumflex, 0xdb }, 
			{ PdfGlyphNames.Udblacute, 0x170 }, { PdfGlyphNames.Udieresis, 0xdc }, { PdfGlyphNames.Ugrave, 0xd9 }, { PdfGlyphNames.Umacron, 0x16a }, { PdfGlyphNames.Uogonek, 0x172 }, 
			{ PdfGlyphNames.Upsilon, 0x3a5 }, { PdfGlyphNames.Upsilon1, 0x3d2 }, { PdfGlyphNames.Upsilondieresis, 0x3ab }, { PdfGlyphNames.Upsilontonos, 0x38e }, { PdfGlyphNames.Uring, 0x16e }, 
			{ PdfGlyphNames.Utilde, 0x168 }, { PdfGlyphNames.V, 0x56 }, { PdfGlyphNames.W, 0x57 }, { PdfGlyphNames.Wacute, 0x1e82 }, { PdfGlyphNames.Wcircumflex, 0x174 }, 
			{ PdfGlyphNames.Wdieresis, 0x1e84 }, { PdfGlyphNames.Wgrave, 0x1e80 }, { PdfGlyphNames.X, 0x58 }, { PdfGlyphNames.Xi, 0x39e }, { PdfGlyphNames.Y, 0x59 }, { PdfGlyphNames.Yacute, 0xdd }, 
			{ PdfGlyphNames.Ycircumflex, 0x176 }, { PdfGlyphNames.Ydieresis, 0x178 }, { PdfGlyphNames.Ygrave, 0x1ef2 }, { PdfGlyphNames.Z, 0x5a }, { PdfGlyphNames.Zacute, 0x179 }, 
			{ PdfGlyphNames.Zcaron, 0x17d }, { PdfGlyphNames.Zdot, 0x17b }, { PdfGlyphNames.Zeta, 0x396 }, { PdfGlyphNames.a, 0x61 }, { PdfGlyphNames.a1, 0x2701 }, { PdfGlyphNames.a10, 0x2721 }, 
			{ PdfGlyphNames.a100, 0x275e }, { PdfGlyphNames.a101, 0x2761 }, { PdfGlyphNames.a102, 0x2762 }, { PdfGlyphNames.a103, 0x2763 }, { PdfGlyphNames.a104, 0x2764 }, { PdfGlyphNames.a105, 0x2710 }, 
			{ PdfGlyphNames.a106, 0x2765 }, { PdfGlyphNames.a107, 0x2766 }, { PdfGlyphNames.a108, 0x2767 }, { PdfGlyphNames.a109, 0x2660 }, { PdfGlyphNames.a11, 0x261b }, { PdfGlyphNames.a110, 0x2665 }, 
			{ PdfGlyphNames.a111, 0x2666 }, { PdfGlyphNames.a112, 0x2663 }, { PdfGlyphNames.a117, 0x2709 }, { PdfGlyphNames.a118, 0x2708 }, { PdfGlyphNames.a119, 0x2707 }, { PdfGlyphNames.a12, 0x261e }, 
			{ PdfGlyphNames.a120, 0x2460 }, { PdfGlyphNames.a121, 0x2461 }, { PdfGlyphNames.a122, 0x2462 }, { PdfGlyphNames.a123, 0x2463 }, { PdfGlyphNames.a124, 0x2464 }, { PdfGlyphNames.a125, 0x2465 },
			{ PdfGlyphNames.a126, 0x2466 }, { PdfGlyphNames.a127, 0x2467 }, { PdfGlyphNames.a128, 0x2468 }, { PdfGlyphNames.a129, 0x2469 }, { PdfGlyphNames.a13, 0x270c }, { PdfGlyphNames.a130, 0x2776 },
			{ PdfGlyphNames.a131, 0x2777 }, { PdfGlyphNames.a132, 0x2778 }, { PdfGlyphNames.a133, 0x2779 }, { PdfGlyphNames.a134, 0x277a }, { PdfGlyphNames.a135, 0x277b }, { PdfGlyphNames.a136, 0x277c },
			{ PdfGlyphNames.a137, 0x277d }, { PdfGlyphNames.a138, 0x277e }, { PdfGlyphNames.a139, 0x277f }, { PdfGlyphNames.a14, 0x270d }, { PdfGlyphNames.a140, 0x2780 }, { PdfGlyphNames.a141, 0x2781 },
			{ PdfGlyphNames.a142, 0x2782 }, { PdfGlyphNames.a143, 0x2783 }, { PdfGlyphNames.a144, 0x2784 }, { PdfGlyphNames.a145, 0x2785 }, { PdfGlyphNames.a146, 0x2786 }, { PdfGlyphNames.a147, 0x2787 },
			{ PdfGlyphNames.a148, 0x2788 }, { PdfGlyphNames.a149, 0x2789 }, { PdfGlyphNames.a15, 0x270e }, { PdfGlyphNames.a150, 0x278a }, { PdfGlyphNames.a151, 0x278b }, { PdfGlyphNames.a152, 0x278c },
			{ PdfGlyphNames.a153, 0x278d }, { PdfGlyphNames.a154, 0x278e }, { PdfGlyphNames.a155, 0x278f }, { PdfGlyphNames.a156, 0x2790 }, { PdfGlyphNames.a157, 0x2791 }, { PdfGlyphNames.a158, 0x2792 },
			{ PdfGlyphNames.a159, 0x2793 }, { PdfGlyphNames.a16, 0x270f },  { PdfGlyphNames.a160, 0x2794 }, { PdfGlyphNames.a161, 0x2192 }, { PdfGlyphNames.a162, 0x27a3 }, { PdfGlyphNames.a163, 0x2194 }, 
			{ PdfGlyphNames.a164, 0x2195 }, { PdfGlyphNames.a165, 0x2799 }, { PdfGlyphNames.a166, 0x279b }, { PdfGlyphNames.a167, 0x279c }, { PdfGlyphNames.a168, 0x279d }, { PdfGlyphNames.a169, 0x279e }, 
			{ PdfGlyphNames.a17, 0x2711 }, { PdfGlyphNames.a170, 0x279f }, { PdfGlyphNames.a171, 0x27a0 }, { PdfGlyphNames.a172, 0x27a1 }, { PdfGlyphNames.a173, 0x27a2 }, { PdfGlyphNames.a174, 0x27a4 },
			{ PdfGlyphNames.a175, 0x27a5 }, { PdfGlyphNames.a176, 0x27a6 }, { PdfGlyphNames.a177, 0x27a7 }, { PdfGlyphNames.a178, 0x27a8 }, { PdfGlyphNames.a179, 0x27a9 }, { PdfGlyphNames.a18, 0x2712 }, 
			{ PdfGlyphNames.a180, 0x27ab }, { PdfGlyphNames.a181, 0x27ad }, { PdfGlyphNames.a182, 0x27af }, { PdfGlyphNames.a183, 0x27b2 }, { PdfGlyphNames.a184, 0x27b3 }, { PdfGlyphNames.a185, 0x27b5 },
			{ PdfGlyphNames.a186, 0x27b8 }, { PdfGlyphNames.a187, 0x27ba }, { PdfGlyphNames.a188, 0x27bb }, { PdfGlyphNames.a189, 0x27bc }, { PdfGlyphNames.a19, 0x2713 }, { PdfGlyphNames.a190, 0x27bd }, 
			{ PdfGlyphNames.a191, 0x27be }, { PdfGlyphNames.a192, 0x279a }, { PdfGlyphNames.a193, 0x27aa }, { PdfGlyphNames.a194, 0x27b6 }, { PdfGlyphNames.a195, 0x27b9 }, { PdfGlyphNames.a196, 0x2798 }, 
			{ PdfGlyphNames.a197, 0x27b4 }, { PdfGlyphNames.a198, 0x27b7 }, { PdfGlyphNames.a199, 0x27ac }, { PdfGlyphNames.a2, 0x2702 }, { PdfGlyphNames.a20, 0x2714 }, { PdfGlyphNames.a200, 0x27ae }, 
			{ PdfGlyphNames.a201, 0x27b1 }, { PdfGlyphNames.a202, 0x2703 }, { PdfGlyphNames.a203, 0x2750 }, { PdfGlyphNames.a204, 0x2752 }, { PdfGlyphNames.a205, 0xf8dd }, { PdfGlyphNames.a206, 0xf8df }, 
			{ PdfGlyphNames.a21, 0x2715 }, { PdfGlyphNames.a22, 0x2716 }, { PdfGlyphNames.a23, 0x2717 }, { PdfGlyphNames.a24, 0x2718 }, { PdfGlyphNames.a25, 0x2719 }, { PdfGlyphNames.a26, 0x271a }, 
			{ PdfGlyphNames.a27, 0x271b }, { PdfGlyphNames.a28, 0x271c }, { PdfGlyphNames.a29, 0x2722 }, { PdfGlyphNames.a3, 0x2704 }, { PdfGlyphNames.a30, 0x2723 }, { PdfGlyphNames.a31, 0x2724 }, 
			{ PdfGlyphNames.a32, 0x2725 }, { PdfGlyphNames.a33, 0x2726 }, { PdfGlyphNames.a34, 0x2727 }, { PdfGlyphNames.a35, 0x2605 }, { PdfGlyphNames.a36, 0x2729 }, { PdfGlyphNames.a37, 0x272a }, 
			{ PdfGlyphNames.a38, 0x272b }, { PdfGlyphNames.a39, 0x272c }, { PdfGlyphNames.a4, 0x260e }, { PdfGlyphNames.a40, 0x272d }, { PdfGlyphNames.a41, 0x272e }, { PdfGlyphNames.a42, 0x272f }, 
			{ PdfGlyphNames.a43, 0x2730 }, { PdfGlyphNames.a44, 0x2731 }, { PdfGlyphNames.a45, 0x2732 }, { PdfGlyphNames.a46, 0x2733 }, { PdfGlyphNames.a47, 0x2734 }, { PdfGlyphNames.a48, 0x2735 }, 
			{ PdfGlyphNames.a49, 0x2736 }, { PdfGlyphNames.a5, 0x2706 }, { PdfGlyphNames.a50, 0x2737 }, { PdfGlyphNames.a51, 0x2738 }, { PdfGlyphNames.a52, 0x2739 }, { PdfGlyphNames.a53, 0x273a }, 
			{ PdfGlyphNames.a54, 0x273b }, { PdfGlyphNames.a55, 0x273c }, { PdfGlyphNames.a56, 0x273d }, { PdfGlyphNames.a57, 0x273e }, { PdfGlyphNames.a58, 0x273f }, { PdfGlyphNames.a59, 0x2740 }, 
			{ PdfGlyphNames.a6, 0x271d }, { PdfGlyphNames.a60, 0x2741 }, { PdfGlyphNames.a61, 0x2742 }, { PdfGlyphNames.a62, 0x2743 }, { PdfGlyphNames.a63, 0x2744 }, { PdfGlyphNames.a64, 0x2745 }, 
			{ PdfGlyphNames.a65, 0x2746 }, { PdfGlyphNames.a66, 0x2747 }, { PdfGlyphNames.a67, 0x2748 }, { PdfGlyphNames.a68, 0x2749 }, { PdfGlyphNames.a69, 0x274a }, { PdfGlyphNames.a7, 0x271e }, 
			{ PdfGlyphNames.a70, 0x274b }, { PdfGlyphNames.a71, 0x25cf }, { PdfGlyphNames.a72, 0x274d }, { PdfGlyphNames.a73, 0x25a0 }, { PdfGlyphNames.a74, 0x274f }, { PdfGlyphNames.a75, 0x2751 }, 
			{ PdfGlyphNames.a76, 0x25b2 }, { PdfGlyphNames.a77, 0x25bc }, { PdfGlyphNames.a78, 0x25c6 }, { PdfGlyphNames.a79, 0x2756 }, { PdfGlyphNames.a8, 0x271f }, { PdfGlyphNames.a81, 0x25d7 }, 
			{ PdfGlyphNames.a82, 0x2758 }, { PdfGlyphNames.a83, 0x2759 }, { PdfGlyphNames.a84, 0x275a }, { PdfGlyphNames.a85, 0xf8de }, { PdfGlyphNames.a86, 0xf8e0 }, { PdfGlyphNames.a87, 0xf8e1 }, 
			{ PdfGlyphNames.a88, 0xf8e2 }, { PdfGlyphNames.a89, 0xf8d7 }, { PdfGlyphNames.a9, 0x2720 }, { PdfGlyphNames.a90, 0xf8d8 }, { PdfGlyphNames.a91, 0xf8db }, { PdfGlyphNames.a92, 0xf8dc }, 
			{ PdfGlyphNames.a93, 0xf8d9 }, { PdfGlyphNames.a94, 0xf8da }, { PdfGlyphNames.a95, 0xf8e3 }, { PdfGlyphNames.a96, 0xf8e4 }, { PdfGlyphNames.a97, 0x275b }, { PdfGlyphNames.a98, 0x275c }, 
			{ PdfGlyphNames.a99, 0x275d }, { PdfGlyphNames.aacute, 0xe1 }, { PdfGlyphNames.abreve, 0x103 }, { PdfGlyphNames.acircumflex, 0xe2 }, { PdfGlyphNames.acute, 0xb4 }, 
			{ PdfGlyphNames.adieresis, 0xe4 }, { PdfGlyphNames.ae, 0xe6 }, { PdfGlyphNames.aeacute, 0x1fd }, { PdfGlyphNames.afii00208, 0x2015 }, { PdfGlyphNames.afii08941, 0x20a4 }, 
			{ PdfGlyphNames.afii10017, 0x410 }, { PdfGlyphNames.afii10018, 0x411 }, { PdfGlyphNames.afii10019, 0x412 }, { PdfGlyphNames.afii10020, 0x413 }, { PdfGlyphNames.afii10021, 0x414 }, 
			{ PdfGlyphNames.afii10022, 0x415 }, { PdfGlyphNames.afii10023, 0x401 }, { PdfGlyphNames.afii10024, 0x416 }, { PdfGlyphNames.afii10025, 0x417 }, { PdfGlyphNames.afii10026, 0x418 }, 
			{ PdfGlyphNames.afii10027, 0x419 }, { PdfGlyphNames.afii10028, 0x41a }, { PdfGlyphNames.afii10029, 0x41b }, { PdfGlyphNames.afii10030, 0x41c }, { PdfGlyphNames.afii10031, 0x41d }, 
			{ PdfGlyphNames.afii10032, 0x41e }, { PdfGlyphNames.afii10033, 0x41f }, { PdfGlyphNames.afii10034, 0x420 }, { PdfGlyphNames.afii10035, 0x421 }, { PdfGlyphNames.afii10036, 0x422 }, 
			{ PdfGlyphNames.afii10037, 0x423 }, { PdfGlyphNames.afii10038, 0x424 }, { PdfGlyphNames.afii10039, 0x425 }, { PdfGlyphNames.afii10040, 0x426 }, { PdfGlyphNames.afii10041, 0x427 }, 
			{ PdfGlyphNames.afii10042, 0x428 }, { PdfGlyphNames.afii10043, 0x429 }, { PdfGlyphNames.afii10044, 0x42a }, { PdfGlyphNames.afii10045, 0x42b }, { PdfGlyphNames.afii10046, 0x42c }, 
			{ PdfGlyphNames.afii10047, 0x42d }, { PdfGlyphNames.afii10048, 0x42e }, { PdfGlyphNames.afii10049, 0x42f }, { PdfGlyphNames.afii10050, 0x490 }, { PdfGlyphNames.afii10051, 0x402 }, 
			{ PdfGlyphNames.afii10052, 0x403 }, { PdfGlyphNames.afii10053, 0x404 }, { PdfGlyphNames.afii10054, 0x405 }, { PdfGlyphNames.afii10055, 0x406 }, { PdfGlyphNames.afii10056, 0x407 }, 
			{ PdfGlyphNames.afii10057, 0x408 }, { PdfGlyphNames.afii10058, 0x409 }, { PdfGlyphNames.afii10059, 0x40a }, { PdfGlyphNames.afii10060, 0x40b }, { PdfGlyphNames.afii10061, 0x40c }, 
			{ PdfGlyphNames.afii10062, 0x40e }, { PdfGlyphNames.afii10065, 0x430 }, { PdfGlyphNames.afii10066, 0x431 }, { PdfGlyphNames.afii10067, 0x432 }, { PdfGlyphNames.afii10068, 0x433 }, 
			{ PdfGlyphNames.afii10069, 0x434 }, { PdfGlyphNames.afii10070, 0x435 }, { PdfGlyphNames.afii10071, 0x451 }, { PdfGlyphNames.afii10072, 0x436 }, { PdfGlyphNames.afii10073, 0x437 }, 
			{ PdfGlyphNames.afii10074, 0x438 }, { PdfGlyphNames.afii10075, 0x439 }, { PdfGlyphNames.afii10076, 0x43a }, { PdfGlyphNames.afii10077, 0x43b }, { PdfGlyphNames.afii10078, 0x43c }, 
			{ PdfGlyphNames.afii10079, 0x43d }, { PdfGlyphNames.afii10080, 0x43e }, { PdfGlyphNames.afii10081, 0x43f }, { PdfGlyphNames.afii10082, 0x440 }, { PdfGlyphNames.afii10083, 0x441 }, 
			{ PdfGlyphNames.afii10084, 0x442 }, { PdfGlyphNames.afii10085, 0x443 }, { PdfGlyphNames.afii10086, 0x444 }, { PdfGlyphNames.afii10087, 0x445 }, { PdfGlyphNames.afii10088, 0x446 }, 
			{ PdfGlyphNames.afii10089, 0x447 }, { PdfGlyphNames.afii10090, 0x448 }, { PdfGlyphNames.afii10091, 0x449 }, { PdfGlyphNames.afii10092, 0x44a }, { PdfGlyphNames.afii10093, 0x44b }, 
			{ PdfGlyphNames.afii10094, 0x44c }, { PdfGlyphNames.afii10095, 0x44d }, { PdfGlyphNames.afii10096, 0x44e }, { PdfGlyphNames.afii10097, 0x44f }, { PdfGlyphNames.afii10098, 0x491 }, 
			{ PdfGlyphNames.afii10099, 0x452 }, { PdfGlyphNames.afii10100, 0x453 }, { PdfGlyphNames.afii10101, 0x454 }, { PdfGlyphNames.afii10102, 0x455 }, { PdfGlyphNames.afii10103, 0x456 }, 
			{ PdfGlyphNames.afii10104, 0x457 }, { PdfGlyphNames.afii10105, 0x458 }, { PdfGlyphNames.afii10106, 0x459 }, { PdfGlyphNames.afii10107, 0x45a }, { PdfGlyphNames.afii10108, 0x45b }, 
			{ PdfGlyphNames.afii10109, 0x45c }, { PdfGlyphNames.afii10110, 0x45e }, { PdfGlyphNames.afii10145, 0x40f }, { PdfGlyphNames.afii10193, 0x45f }, { PdfGlyphNames.afii61248, 0x2105 }, 
			{ PdfGlyphNames.afii61289, 0x2113 }, { PdfGlyphNames.afii61352, 0x2116 }, { PdfGlyphNames.agrave, 0xe0 }, { PdfGlyphNames.aleph, 0x2135 }, { PdfGlyphNames.alpha, 0x3b1 }, 
			{ PdfGlyphNames.alphatonos, 0x3ac }, { PdfGlyphNames.amacron, 0x101 }, { PdfGlyphNames.ampersand, 0x26 }, { PdfGlyphNames.angle, 0x2220 }, { PdfGlyphNames.angleleft, 0x2329 }, 
			{ PdfGlyphNames.angleright, 0x232a }, { PdfGlyphNames.anoteleia, 0x387 }, { PdfGlyphNames.aogonek, 0x105 }, { PdfGlyphNames.applelogo, 0xf000 }, { PdfGlyphNames.approxequal, 0x2248 }, 
			{ PdfGlyphNames.aring, 0xe5 }, { PdfGlyphNames.aringacute, 0x1fb }, { PdfGlyphNames.arrowboth, 0x2194 }, { PdfGlyphNames.arrowdblboth, 0x21d4 }, { PdfGlyphNames.arrowdbldown, 0x21d3 }, 
			{ PdfGlyphNames.arrowdblleft, 0x21d0 }, { PdfGlyphNames.arrowdblright, 0x21d2 }, { PdfGlyphNames.arrowdblup, 0x21d1 }, { PdfGlyphNames.arrowdown, 0x2193 }, 
			{ PdfGlyphNames.arrowhorizex, 0xf8e7 }, { PdfGlyphNames.arrowleft, 0x2190 }, { PdfGlyphNames.arrowright, 0x2192 }, { PdfGlyphNames.arrowup, 0x2191 }, { PdfGlyphNames.arrowupdn, 0x2195 }, 
			{ PdfGlyphNames.arrowupdnbse, 0x21a8 }, { PdfGlyphNames.arrowvertex, 0xf8e6 }, { PdfGlyphNames.asciicircum, 0x5e }, { PdfGlyphNames.asciitilde, 0x7e }, { PdfGlyphNames.asterisk, 0x2a }, 
			{ PdfGlyphNames.asteriskmath, 0x2217 }, { PdfGlyphNames.at, 0x40 }, { PdfGlyphNames.atilde, 0xe3 }, { PdfGlyphNames.b, 0x62 }, { PdfGlyphNames.backslash, 0x5c }, { PdfGlyphNames.bar, 0x7c }, 
			{ PdfGlyphNames.beta, 0x3b2 }, { PdfGlyphNames.block, 0x2588 }, { PdfGlyphNames.braceex, 0xf8f4 }, { PdfGlyphNames.braceleft, 0x7b }, { PdfGlyphNames.braceleftbt, 0xf8f1 }, 
			{ PdfGlyphNames.braceleftmid, 0xf8f2 }, { PdfGlyphNames.bracelefttp, 0xf8f1 }, { PdfGlyphNames.bracerightbt, 0xf8fe }, { PdfGlyphNames.bracerightmid, 0xf8fd }, 
			{ PdfGlyphNames.bracerighttp, 0xf8fc }, { PdfGlyphNames.braceright, 0x7d }, { PdfGlyphNames.bracketleft, 0x5b }, { PdfGlyphNames.bracketleftbt, 0xf8f0 }, { PdfGlyphNames.bracketleftex, 0xf8ef }, 
			{ PdfGlyphNames.bracketlefttp, 0xf8ee }, { PdfGlyphNames.bracketright, 0x5d }, { PdfGlyphNames.bracketrightbt, 0xf8fb }, { PdfGlyphNames.bracketrightex, 0xf8fa }, 
			{ PdfGlyphNames.bracketrighttp, 0xf8f9 }, { PdfGlyphNames.breve, 0x2d8 }, { PdfGlyphNames.brokenbar, 0xa6 }, { PdfGlyphNames.bullet, 0x2022 }, { PdfGlyphNames.c, 0x63 }, 
			{ PdfGlyphNames.cacute, 0x107 }, { PdfGlyphNames.caron, 0x2c7 }, { PdfGlyphNames.carriagereturn, 0x21b5 }, { PdfGlyphNames.ccaron, 0x10d }, { PdfGlyphNames.ccedilla, 0xe7 }, 
			{ PdfGlyphNames.ccircumflex, 0x109 }, { PdfGlyphNames.cdot, 0x10b }, { PdfGlyphNames.cedilla, 0xb8 }, { PdfGlyphNames.cent, 0xa2 }, { PdfGlyphNames.chi, 0x3c7 }, 
			{ PdfGlyphNames.circle, 0x25cb }, { PdfGlyphNames.circlemultiply, 0x2297 }, { PdfGlyphNames.circleplus, 0x2295 }, { PdfGlyphNames.circumflex, 0x2c6 }, { PdfGlyphNames.club, 0x2663 }, 
			{ PdfGlyphNames.colon, 0x3a }, { PdfGlyphNames.comma, 0x2c }, { PdfGlyphNames.congruent, 0x2245 }, { PdfGlyphNames.copyright, 0xa9 }, { PdfGlyphNames.copyrightsans, 0xf8e9 }, 
			{ PdfGlyphNames.copyrightserif, 0xf6d9 }, { PdfGlyphNames.currency, 0xa4 }, { PdfGlyphNames.d, 0x64 }, { PdfGlyphNames.dagger, 0x2020 }, { PdfGlyphNames.daggerdbl, 0x2021 }, 
			{ PdfGlyphNames.dcaron, 0x10f }, { PdfGlyphNames.dcroat, 0x111 }, { PdfGlyphNames.degree, 0xb0 }, { PdfGlyphNames.delta, 0x3b4 }, { PdfGlyphNames.diamond, 0x2666 }, 
			{ PdfGlyphNames.dieresis, 0xa8 }, { PdfGlyphNames.dieresistonos, 0x385 }, { PdfGlyphNames.divide, 0xf7 }, { PdfGlyphNames.dkshade, 0x2593 }, { PdfGlyphNames.dnblock, 0x2584 }, 
			{ PdfGlyphNames.dollar, 0x24 }, { PdfGlyphNames.dotaccent, 0x2d9 }, { PdfGlyphNames.dotlessi, 0x131 }, { PdfGlyphNames.dotmath, 0x22c5 }, { PdfGlyphNames.e, 0x65 }, 
			{ PdfGlyphNames.eacute, 0xe9 }, { PdfGlyphNames.ebreve, 0x115 }, { PdfGlyphNames.ecaron, 0x11b }, { PdfGlyphNames.ecircumflex, 0xea }, { PdfGlyphNames.edieresis, 0xeb }, 
			{ PdfGlyphNames.edot, 0x117 }, { PdfGlyphNames.egrave, 0xe8 }, { PdfGlyphNames.eight, 0x38 }, { PdfGlyphNames.element, 0x2208 }, { PdfGlyphNames.ellipsis, 0x2026 }, 
			{ PdfGlyphNames.emacron, 0x113 }, { PdfGlyphNames.emdash, 0x2014 }, { PdfGlyphNames.emptyset, 0x2205 }, { PdfGlyphNames.endash, 0x2013 }, { PdfGlyphNames.eng, 0x14b }, 
			{ PdfGlyphNames.equal, 0x3d }, { PdfGlyphNames.eogonek, 0x119 }, { PdfGlyphNames.epsilon, 0x3b5 }, { PdfGlyphNames.epsilontonos, 0x3ad }, { PdfGlyphNames.equivalence, 0x2261 }, 
			{ PdfGlyphNames.estimated, 0x212e }, { PdfGlyphNames.eta, 0x3b7 }, { PdfGlyphNames.etatonos, 0x3ae }, { PdfGlyphNames.eth, 0xf0 }, { PdfGlyphNames.exclam, 0x21 }, 
			{ PdfGlyphNames.exclamdbl, 0x203c }, { PdfGlyphNames.exclamdown, 0xa1 }, { PdfGlyphNames.existential, 0x2203 }, { PdfGlyphNames.f, 0x66 }, { PdfGlyphNames.female, 0x2640 }, 
			{ PdfGlyphNames.fi, 0xfb01 }, { PdfGlyphNames.filledbox, 0x25a0 }, { PdfGlyphNames.filledrect, 0x25ac }, { PdfGlyphNames.five, 0x35 }, { PdfGlyphNames.fiveeighths, 0x215d }, 
			{ PdfGlyphNames.fl, 0xfb02 }, { PdfGlyphNames.florin, 0x192 }, { PdfGlyphNames.four, 0x34 }, { PdfGlyphNames.fraction, 0x2044 }, { PdfGlyphNames.franc, 0x20a3 }, { PdfGlyphNames.g, 0x67 }, 
			{ PdfGlyphNames.gamma, 0x3b3 }, { PdfGlyphNames.gbreve, 0x11f }, { PdfGlyphNames.gcedilla, 0x123 }, { PdfGlyphNames.gcircumflex, 0x11d }, { PdfGlyphNames.gdot, 0x0121 }, 
			{ PdfGlyphNames.germandbls, 0xdf }, { PdfGlyphNames.gradient, 0x2207 }, { PdfGlyphNames.grave, 0x60 }, { PdfGlyphNames.greater, 0x3e }, { PdfGlyphNames.greaterequal, 0x2265 }, 
			{ PdfGlyphNames.guillemotleft, 0xab }, { PdfGlyphNames.guillemotright, 0xbb }, { PdfGlyphNames.guilsinglleft, 0x2039 }, { PdfGlyphNames.guilsinglright, 0x203a }, { PdfGlyphNames.h, 0x68 }, 
			{ PdfGlyphNames.hbar, 0x127 }, { PdfGlyphNames.hcircumflex, 0x125 }, { PdfGlyphNames.heart, 0x2665 }, { PdfGlyphNames.house, 0x2302 }, { PdfGlyphNames.hungarumlaut, 0x2dd }, 
			{ PdfGlyphNames.hyphen, 0x2d }, { PdfGlyphNames.i, 0x69 }, { PdfGlyphNames.iacute, 0xed }, { PdfGlyphNames.ibreve, 0x12d }, { PdfGlyphNames.icircumflex, 0xee }, 
			{ PdfGlyphNames.idieresis, 0xef }, { PdfGlyphNames.igrave, 0xec }, { PdfGlyphNames.ij, 0x133 }, { PdfGlyphNames.imacron, 0x12b }, { PdfGlyphNames.increment, 0x2206 }, 
			{ PdfGlyphNames.infinity, 0x221e }, { PdfGlyphNames.integral, 0x222b }, { PdfGlyphNames.integralbt, 0x2321 }, { PdfGlyphNames.integralex, 0xf8f5 }, { PdfGlyphNames.integraltp, 0x2320 }, 
			{ PdfGlyphNames.intersection, 0x2229 }, { PdfGlyphNames.invbullet, 0x25d8 }, { PdfGlyphNames.invcircle, 0x25d9 }, { PdfGlyphNames.invsmileface, 0x263b }, { PdfGlyphNames.iogonek, 0x12f }, 
			{ PdfGlyphNames.iota, 0x3b9 }, { PdfGlyphNames.iotadieresis, 0x3ca }, { PdfGlyphNames.iotadieresistonos, 0x390 }, { PdfGlyphNames.iotatonos, 0x3af }, { PdfGlyphNames.itilde, 0x129 }, 
			{ PdfGlyphNames.j, 0x6a }, { PdfGlyphNames.jcircumflex, 0x135 }, { PdfGlyphNames.k, 0x6b }, { PdfGlyphNames.kappa, 0x3ba }, { PdfGlyphNames.kcedilla, 0x137 }, 
			{ PdfGlyphNames.kgreenlandic, 0x138 }, { PdfGlyphNames.l, 0x6c }, { PdfGlyphNames.lacute, 0x13a }, { PdfGlyphNames.lambda, 0x3bb }, { PdfGlyphNames.lcaron, 0x13e }, 
			{ PdfGlyphNames.lcedilla, 0x13c }, { PdfGlyphNames.ldot, 0x140 }, { PdfGlyphNames.less, 0x3c }, { PdfGlyphNames.lessequal, 0x2264 }, { PdfGlyphNames.lfblock, 0x258c }, 
			{ PdfGlyphNames.logicaland, 0x2227 }, { PdfGlyphNames.logicalnot, 0xac }, { PdfGlyphNames.logicalor, 0x2228 }, { PdfGlyphNames.longs, 0x17f }, { PdfGlyphNames.lozenge, 0x25ca }, 
			{ PdfGlyphNames.lslash, 0x142 }, { PdfGlyphNames.ltshade, 0x2591 }, { PdfGlyphNames.m, 0x6d }, { PdfGlyphNames.macron, 0xaf }, { PdfGlyphNames.male, 0x2642 }, { PdfGlyphNames.minus, 0x2212 }, 
			{ PdfGlyphNames.minute, 0x2032 }, { PdfGlyphNames.mu, 0xb5 }, { PdfGlyphNames.multiply, 0xd7 }, { PdfGlyphNames.musicalnote, 0x266a }, { PdfGlyphNames.musicalnotedbl, 0x266b }, 
			{ PdfGlyphNames.n, 0x6e }, { PdfGlyphNames.nacute, 0x144 }, { PdfGlyphNames.napostrophe, 0x149 }, { PdfGlyphNames.nbspace, 0xa0 }, { PdfGlyphNames.ncaron, 0x148 }, 
			{ PdfGlyphNames.ncedilla, 0x146 }, { PdfGlyphNames.nine, 0x39 }, { PdfGlyphNames.notelement, 0x2209 }, { PdfGlyphNames.notequal, 0x2260 }, { PdfGlyphNames.notsubset, 0x2284 }, 
			{ PdfGlyphNames.nsuperior, 0x207f }, { PdfGlyphNames.ntilde, 0xf1 }, { PdfGlyphNames.nu, 0x3bd }, { PdfGlyphNames.numbersign, 0x23 }, { PdfGlyphNames.o, 0x6f }, { PdfGlyphNames.oacute, 0xf3 }, 
			{ PdfGlyphNames.obreve, 0x14f }, { PdfGlyphNames.ocircumflex, 0xf4 }, { PdfGlyphNames.odblacute, 0x151 }, { PdfGlyphNames.odieresis, 0xf6 }, { PdfGlyphNames.oe, 0x153 }, 
			{ PdfGlyphNames.ogonek, 0x2db }, { PdfGlyphNames.ograve, 0xf2 }, { PdfGlyphNames.omacron, 0x14d }, { PdfGlyphNames.omega, 0x3c9 }, { PdfGlyphNames.omega1, 0x3d6 }, 
			{ PdfGlyphNames.omegatonos, 0x3ce }, { PdfGlyphNames.omicron, 0x3bf }, { PdfGlyphNames.omicrontonos, 0x3cc }, { PdfGlyphNames.one, 0x31 }, { PdfGlyphNames.oneeighth, 0x215b }, 
			{ PdfGlyphNames.onehalf, 0xbd }, { PdfGlyphNames.onequarter, 0xbc }, { PdfGlyphNames.onesuperior, 0xb9 }, { PdfGlyphNames.openbullet, 0x25e6 }, { PdfGlyphNames.ordfeminine, 0xaa }, 
			{ PdfGlyphNames.ordmasculine, 0xba }, { PdfGlyphNames.orthogonal, 0x221f }, { PdfGlyphNames.oslash, 0xf8 }, { PdfGlyphNames.oslashacute, 0x1ff }, { PdfGlyphNames.otilde, 0xf5 }, 
			{ PdfGlyphNames.p, 0x70 }, { PdfGlyphNames.paragraph, 0xb6 }, { PdfGlyphNames.parenleft, 0x28 }, { PdfGlyphNames.parenleftbt, 0xf8ed }, { PdfGlyphNames.parenleftex, 0xf8ec }, 
			{ PdfGlyphNames.parenlefttp, 0xf8eb }, { PdfGlyphNames.parenright, 0x29 }, { PdfGlyphNames.parenrightbt, 0xf8f8 }, { PdfGlyphNames.parenrightex, 0xf8f7 }, { PdfGlyphNames.parenrighttp, 0xf8f6 }, 
			{ PdfGlyphNames.partialdiff, 0x2202 }, { PdfGlyphNames.percent, 0x25 }, { PdfGlyphNames.period, 0x2e }, { PdfGlyphNames.periodcentered, 0xb7 }, { PdfGlyphNames.perpendicular, 0x22a5 }, 
			{ PdfGlyphNames.perthousand, 0x2030 }, { PdfGlyphNames.peseta, 0x20a7 }, { PdfGlyphNames.phi, 0x3c6 }, { PdfGlyphNames.phi1, 0x3d5 }, { PdfGlyphNames.pi, 0x3c0 }, { PdfGlyphNames.plus, 0x2b }, 
			{ PdfGlyphNames.plusminus, 0xb1 }, { PdfGlyphNames.product, 0x220f }, { PdfGlyphNames.propersubset, 0x2282 }, { PdfGlyphNames.propersuperset, 0x2283 }, { PdfGlyphNames.proportional, 0x221d }, 
			{ PdfGlyphNames.psi, 0x3c8 }, { PdfGlyphNames.q, 0x71 }, { PdfGlyphNames.question, 0x3f }, { PdfGlyphNames.questiondown, 0xbf }, { PdfGlyphNames.quotedbl, 0x22 }, 
			{ PdfGlyphNames.quotedblbase, 0x201e }, { PdfGlyphNames.quotedblleft, 0x201c }, { PdfGlyphNames.quotedblright, 0x201d }, { PdfGlyphNames.quoteleft, 0x2018 }, 
			{ PdfGlyphNames.quotereversed, 0x201b }, { PdfGlyphNames.quoteright, 0x2019 }, { PdfGlyphNames.quotesinglbase, 0x201a }, { PdfGlyphNames.quotesingle, 0x27 }, { PdfGlyphNames.r, 0x72 }, 
			{ PdfGlyphNames.racute, 0x155 }, { PdfGlyphNames.radical, 0x221a }, { PdfGlyphNames.radicalex, 0x203e }, { PdfGlyphNames.rcaron, 0x159 }, { PdfGlyphNames.rcedilla, 0x157 }, 
			{ PdfGlyphNames.reflexsubset, 0x2286 }, { PdfGlyphNames.reflexsuperset, 0x2287 }, { PdfGlyphNames.registered, 0xae }, { PdfGlyphNames.registersans, 0xf8e8 }, 
			{ PdfGlyphNames.registerserif, 0xf6da }, { PdfGlyphNames.revlogicalnot, 0x2310 }, { PdfGlyphNames.rho, 0x3c1 }, { PdfGlyphNames.ring, 0x2da }, { PdfGlyphNames.rtblock, 0x2590 }, 
			{ PdfGlyphNames.s, 0x73 }, { PdfGlyphNames.sacute, 0x15b }, { PdfGlyphNames.scaron, 0x161 }, { PdfGlyphNames.scedilla, 0x15f }, { PdfGlyphNames.scircumflex, 0x15d }, 
			{ PdfGlyphNames.second, 0x2033 }, { PdfGlyphNames.section, 0xa7 }, { PdfGlyphNames.semicolon, 0x3b }, { PdfGlyphNames.seven, 0x37 }, { PdfGlyphNames.seveneighths, 0x215e }, 
			{ PdfGlyphNames.sfthyphen, 0xad }, { PdfGlyphNames.shade, 0x2592 }, { PdfGlyphNames.sigma, 0x3c3 }, { PdfGlyphNames.sigma1, 0x3c2 }, { PdfGlyphNames.similar, 0x223c }, 
			{ PdfGlyphNames.six, 0x36 }, { PdfGlyphNames.slash, 0x2f }, { PdfGlyphNames.smileface, 0x263a }, { PdfGlyphNames.space, 0x20 }, { PdfGlyphNames.spade, 0x2660 }, { PdfGlyphNames.sterling, 0xa3 }, 
			{ PdfGlyphNames.suchthat, 0x220b }, { PdfGlyphNames.summation, 0x2211 }, { PdfGlyphNames.sun, 0x263c }, { PdfGlyphNames.t, 0x74 }, { PdfGlyphNames.tau, 0x3c4 }, { PdfGlyphNames.tbar, 0x167 }, 
			{ PdfGlyphNames.tcaron, 0x165 }, { PdfGlyphNames.tcedilla, 0x163 }, { PdfGlyphNames.therefore, 0x2234 }, { PdfGlyphNames.theta, 0x3b8 }, { PdfGlyphNames.theta1, 0x3d1 }, 
			{ PdfGlyphNames.thorn, 0xfe }, { PdfGlyphNames.three, 0x33 }, { PdfGlyphNames.threeeighths, 0x215c }, { PdfGlyphNames.threequarters, 0xbe }, { PdfGlyphNames.threesuperior, 0xb3 }, 
			{ PdfGlyphNames.tilde, 0x2dc }, { PdfGlyphNames.tonos, 0x384 }, { PdfGlyphNames.trademark, 0x2122 }, { PdfGlyphNames.trademarksans, 0xf8ea }, { PdfGlyphNames.trademarkserif, 0xf6db }, 
			{ PdfGlyphNames.triagdn, 0x25bc }, { PdfGlyphNames.triaglf, 0x25c4 }, { PdfGlyphNames.triagrt, 0x25ba }, { PdfGlyphNames.triagup, 0x25b2 }, { PdfGlyphNames.two, 0x32 }, 
			{ PdfGlyphNames.twosuperior, 0xb2 }, { PdfGlyphNames.u, 0x75 }, { PdfGlyphNames.uacute, 0xfa }, { PdfGlyphNames.ubreve, 0x16d }, { PdfGlyphNames.ucircumflex, 0xfb }, 
			{ PdfGlyphNames.udblacute, 0x171 }, { PdfGlyphNames.udieresis, 0xfc }, { PdfGlyphNames.ugrave, 0xf9 }, { PdfGlyphNames.umacron, 0x16b }, { PdfGlyphNames.underscore, 0x5f }, 
			{ PdfGlyphNames.union, 0x222a }, { PdfGlyphNames.universal, 0x2200 }, { PdfGlyphNames.underscoredbl, 0x2017 }, { PdfGlyphNames.uogonek, 0x173 }, { PdfGlyphNames.upblock, 0x2580 }, 
			{ PdfGlyphNames.upsilon, 0x3c5 }, { PdfGlyphNames.upsilondieresis, 0x3cb }, { PdfGlyphNames.upsilondieresistonos, 0x3b0 }, { PdfGlyphNames.upsilontonos, 0x3cd }, { PdfGlyphNames.uring, 0x16f }, 
			{ PdfGlyphNames.utilde, 0x169 }, { PdfGlyphNames.v, 0x76 }, { PdfGlyphNames.w, 0x77 }, { PdfGlyphNames.wacute, 0x1e83 }, { PdfGlyphNames.wcircumflex, 0x175 }, 
			{ PdfGlyphNames.wdieresis, 0x1e85 }, { PdfGlyphNames.weierstrass, 0x2118 }, { PdfGlyphNames.wgrave, 0x1e81 }, { PdfGlyphNames.x, 0x78 }, { PdfGlyphNames.xi, 0x3be }, { PdfGlyphNames.y, 0x79 }, 
			{ PdfGlyphNames.yacute, 0xfd }, { PdfGlyphNames.ycircumflex, 0x177 }, { PdfGlyphNames.ydieresis, 0xff }, { PdfGlyphNames.yen, 0xa5 }, { PdfGlyphNames.ygrave, 0x1ef3 }, { PdfGlyphNames.z, 0x7a }, 
			{ PdfGlyphNames.zacute, 0x17a }, { PdfGlyphNames.zcaron, 0x17e }, { PdfGlyphNames.zdot, 0x17c }, { PdfGlyphNames.zero, 0x30 }, { PdfGlyphNames.zeta, 0x03b6 }
		};
		internal static short GetGlyphCode(short str, PdfSimpleFontEncoding encoding, IDictionary<string, ushort> glyphCodes) {
			string glyphName = encoding.GetGlyphName((byte)str);
			ushort glyphCode;
			if (glyphCodes.TryGetValue(encoding.GetGlyphName((byte)str), out glyphCode))
				return (short)glyphCode;
			switch (glyphName) {
				case PdfGlyphNames.Zdotaccent:
					if (glyphCodes.TryGetValue(PdfGlyphNames.Zdot, out glyphCode))
						return (short)glyphCode;
					break;
				case PdfGlyphNames.zdotaccent:
					if (glyphCodes.TryGetValue(PdfGlyphNames.zdot, out glyphCode))
						return (short)glyphCode;
					break;
			}
			return str;
		}
		internal static short GetGlyphCode(short str, PdfSimpleFontEncoding encoding) {
			return GetGlyphCode(str, encoding, GlyphCodes);
		}
	}
}
