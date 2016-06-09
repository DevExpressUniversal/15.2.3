ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Vrne absolutno vrednost števila, število brez predznaka.",
		arguments: [
			{
				name: "število",
				description: "je realno število, ki mu želite poiskati absolutno vrednost."
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Vrne povečane obresti za vrednostni papir, ki izplača obresti ob zapadlosti.",
		arguments: [
			{
				name: "izdaja",
				description: "je datum izdaje vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "poravnava",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "mera",
				description: "je letna obrestna mera vrednostnega papirja"
			},
			{
				name: "vrednost",
				description: "je imenska vrednost vrednostnega papirja"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "ACOS",
		description: "Vrne arkus kosinus števila, v radianih, iz obsega od 0 do Pi. Arkus kosinus je kot, katerega kosinus je dano število.",
		arguments: [
			{
				name: "število",
				description: "je kosinus želenega kota, ki mora biti med -1 in 1."
			}
		]
	},
	{
		name: "ACOSH",
		description: "Vrne inverzni hiperbolični kosinus števila.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število, enako ali večje kot 1."
			}
		]
	},
	{
		name: "ACOT",
		description: "Vrne arccot števila, v radianih v obsegu od 0 do Pi.",
		arguments: [
			{
				name: "number",
				description: " je kotangens želenega kota"
			}
		]
	},
	{
		name: "ACOTH",
		description: " Vrne inverzni hiperbolični kotangens števila.",
		arguments: [
			{
				name: "number",
				description: "je hiperbolični kotangens želenega kota"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Ustvari sklic na celico kot besedilo. Podati morate številko vrstice in številko stolpca.",
		arguments: [
			{
				name: "št_vrstice",
				description: "je številka vrstice pri sklicevanju na celico: Št_vrstice = 1 za vrstico 1."
			},
			{
				name: "št_stolpca",
				description: "je številka stolpca pri sklicevanju na celico: na primer št_stolpca = 4 za stolpec D."
			},
			{
				name: "abs_št",
				description: "določa vrsto sklica: absolutni = 1; absolutna vrstica/relativni stolpec = 2; relativna vrstica/absolutni stolpec = 3; relativni = 4."
			},
			{
				name: "a1",
				description: "je logična vrednost za določanje sloga sklicevanja: A1 slog = 1 ali TRUE; R1C1 slog = 0 ali FALSE."
			},
			{
				name: "besedilo_lista",
				description: "je besedilo za določanje imena delovnega lista, ki se uporabi kot zunanji sklic."
			}
		]
	},
	{
		name: "AND",
		description: "Preveri, ali imajo vsi argumenti vrednost TRUE, in vrne vrednost TRUE, če imajo vsi argumenti vrednost TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logično1",
				description: "od 1 do 255 pogojev, ki jih želite preskusiti, in so lahko ali TRUE ali FALSE. Lahko so logične vrednosti, matrike ali sklici"
			},
			{
				name: "logično2",
				description: "od 1 do 255 pogojev, ki jih želite preskusiti, in so lahko ali TRUE ali FALSE. Lahko so logične vrednosti, matrike ali sklici"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Pretvori rimsko številko v arabsko.",
		arguments: [
			{
				name: "besedilo",
				description: "je rimska številka, ki jo želite pretvoriti"
			}
		]
	},
	{
		name: "AREAS",
		description: "Vrne število področij v sklicu. Področje je obseg povezanih celic ali ena sama.",
		arguments: [
			{
				name: "sklic",
				description: "je sklic na celico ali obseg celic in se lahko sklicuje na več področij."
			}
		]
	},
	{
		name: "ASIN",
		description: "Vrne arkus sinus števila v radianih, iz obsega od -Pi/2 do Pi/2.",
		arguments: [
			{
				name: "število",
				description: "je sinus iskanega kota, ki mora biti med -1 in 1."
			}
		]
	},
	{
		name: "ASINH",
		description: "Vrne inverzni hiperbolični sinus števila.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število, ki je enako ali večje kot 1."
			}
		]
	},
	{
		name: "ATAN",
		description: "Vrne arkus tangens števila v radianih, iz obsega od -Pi/2 do Pi/2.",
		arguments: [
			{
				name: "število",
				description: "je tangens želenega kota."
			}
		]
	},
	{
		name: "ATAN2",
		description: "Vrne arkus tangens podanih x- in y-koordinat v radianih, iz obsega od -Pi do Pi, brez -Pi.",
		arguments: [
			{
				name: "št_x",
				description: "je x-koordinata točke."
			},
			{
				name: "št_y",
				description: "je y-koordinata točke."
			}
		]
	},
	{
		name: "ATANH",
		description: "Vrne inverzni hiperbolični tangens števila.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število med -1 in 1 brez -1 in 1."
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Vrne povprečje absolutnih odstopanj podatkovnih točk od srednje vrednosti. Argumenti so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 argumentov, za katere želiš določiti povprečje absolutnih odstopanj"
			},
			{
				name: "število2",
				description: "od 1 do 255 argumentov, za katere želiš določiti povprečje absolutnih odstopanj"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Vrne aritmetično povprečno vrednost njegovih argumentov, ki so lahko števila, imena, matrike ali sklici, ki vsebujejo števila.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 številskih argumentov, za katere iščete povprečno vrednost."
			},
			{
				name: "število2",
				description: "od 1 do 255 številskih argumentov, za katere iščete povprečno vrednost."
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Vrne aritmetično povprečno vrednost svojih argumentov. Besedilo in logična vrednost FALSE v argumentih se ovrednoti kot 0, logična vrednost TRUE pa kot 1. Argumenti so lahko števila, imena, matrike ali sklici.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 argumentov, za katere želite povprečje"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 argumentov, za katere želite povprečje"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Najde povprečje (aritmetično povprečno vrednost) za celice, navedene z danim pogojem ali kriterijem.",
		arguments: [
			{
				name: "obseg",
				description: "je obseg celic, katere želite ovrednotiti"
			},
			{
				name: "pogoji",
				description: "je pogoj ali kriterij v obliki števila, izraza ali besedila, ki določa, katere celice bodo uporabljene za iskanje povprečja"
			},
			{
				name: "obseg_za_povprečje",
				description: "so dejanske celice, ki bodo uporabljene za iskanje povprečja. Če bodo izpuščene, bodo uporabljene celice v obsegu"
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Najde povprečje (aritmetično povprečno vrednost) za celice, navedene v danem nizu pogojev ali kriterijev.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "obseg_za_povprečje",
				description: "so dejanske celice, ki bodo uporabljene za iskanje povprečja."
			},
			{
				name: "obseg_pogojev",
				description: "je obseg celic, katere želite ovrednotiti za določen pogoj"
			},
			{
				name: "pogoji",
				description: "je pogoj ali kriterij v obliki števila, izraza ali besedila, ki določa, katere celice bodo uporabljene za iskanje povprečja"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Pretvori število v besedilo (baht).",
		arguments: [
			{
				name: "število",
				description: "je število, ki ga želite pretvoriti"
			}
		]
	},
	{
		name: "BASE",
		description: "Pretvori število v besedilo z danim korenom (osnova).",
		arguments: [
			{
				name: "število",
				description: "je število, ki ga želite pretvoriti"
			},
			{
				name: "koren",
				description: "je osnovni koren, v katerega želite število pretvoriti"
			},
			{
				name: "min_dolžina",
				description: "je najmanjša dolžina vrnjenega niza. Če niso dodane izpuščene vodilne ničle"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Vrne spremenjeno Besselovo funkcijo In(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri ovrednotite funkcijo"
			},
			{
				name: "n",
				description: "je red Besselove funkcije"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Vrne Besselovo funkcijo Jn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri ovrednotite funkcijo"
			},
			{
				name: "n",
				description: "je red Besselove funkcije"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Vrne spremenjeno Besselovo funkcijo Kn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri ovrednotite funkcijo"
			},
			{
				name: "n",
				description: "je red funkcije"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Vrne Besselovo funkcijo Yn(x).",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri ovrednotite funkcijo"
			},
			{
				name: "n",
				description: "je red funkcije"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Vrne beta porazdelitev verjetnosti.",
		arguments: [
			{
				name: "x",
				description: "je vrednost med A in B, pri kateri se funkcija vrednoti."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za porazdelitev gostote verjetnosti uporabite FALSE."
			},
			{
				name: "A",
				description: "je izbirna spodnja meja intervala x. Če je izpuščen, velja A = 0."
			},
			{
				name: "B",
				description: "je izbirna zgornja meja intervala x. Če je izpuščen, velja B = 1."
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Vrne inverzno kumulativno beta porazdelitev gostote verjetnosti (BETA.DIST).",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z beta porazdelitvijo."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "A",
				description: "je izbirna spodnja meja intervala x. Če je izpuščen, velja A = 0."
			},
			{
				name: "B",
				description: "je izbirna zgornja meja intervala x. Če je izpuščen, velja B = 1."
			}
		]
	},
	{
		name: "BETADIST",
		description: "Vrne kumulativno porazdelitev beta gostote verjetnosti.",
		arguments: [
			{
				name: "x",
				description: "je vrednost med A in B, pri kateri se vrednoti funkcija."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "A",
				description: "je izbirna spodnja meja intervala x. Če je izpuščeno, velja A = 0."
			},
			{
				name: "B",
				description: "je izbirna zgornja meja intervala x. Če je izpuščeno, velja B = 1."
			}
		]
	},
	{
		name: "BETAINV",
		description: "Vrne inverzno kumulativno porazdelitev beta gostote verjetnosti (BETADIST).",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s porazdelitvijo beta."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in mora biti večji od 0."
			},
			{
				name: "A",
				description: "je izbirna spodnja meja intervala x. Če je izpuščeno, velja A = 0."
			},
			{
				name: "B",
				description: "je izbirna zgornja meja intervala x. Če je izpuščeno, velja B = 1."
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Pretvori dvojiško število v desetiško.",
		arguments: [
			{
				name: "število",
				description: "je dvojiško število, ki ga želite pretvoriti"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Pretvori dvojiško število v šestnajstiško.",
		arguments: [
			{
				name: "število",
				description: "je dvojiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Pretvori dvojiško število v osmiško.",
		arguments: [
			{
				name: "število",
				description: "je dvojiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Vrne posamezno binomsko porazdelitveno verjetnost.",
		arguments: [
			{
				name: "število_s",
				description: "je število uspešnih preizkusov."
			},
			{
				name: "poskusi",
				description: "je število neodvisnih preizkusov."
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspeha vsakega preizkusa."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Vrne verjetnost preskusnega rezultata z binomsko porazdelitvijo.",
		arguments: [
			{
				name: "poskusi",
				description: "je število samostojnih poskusov"
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspeha posameznega preskusa"
			},
			{
				name: "število_s",
				description: "je število uspehov pri poskusih"
			},
			{
				name: "število_s2",
				description: "če je na voljo, ta funkcija vrne verjetnost, da bo število uspešnih poskusov med številko_s in številko_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Vrne najmanjšo vrednost, za katero je kumulativna binomska porazdelitev večja ali enaka vrednosti kriterija.",
		arguments: [
			{
				name: "poskusi",
				description: "je število Bernoullijevih poskusov."
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspešnosti posameznega poskusa in je število med 0 in vključno 1."
			},
			{
				name: "alfa",
				description: "je vrednost kriterija in je število med 0 in vključno 1."
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Vrne posamezno binomsko porazdelitveno verjetnost.",
		arguments: [
			{
				name: "število_s",
				description: "je število uspešnih preskusov."
			},
			{
				name: "poskusi",
				description: "je število neodvisnih preskusov."
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspeha vsakega preskusa."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "BITAND",
		description: "Vrne bitno vrednost »And« dveh števil.",
		arguments: [
			{
				name: "število1",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			},
			{
				name: "število2",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Vrne število, ki jih biti shift_amount premaknejo na levo.",
		arguments: [
			{
				name: "število",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			},
			{
				name: "shift_amount",
				description: "je število bitov, ki jih želite premakniti"
			}
		]
	},
	{
		name: "BITOR",
		description: "Vrne bitno vrednost »Or« dveh števil.",
		arguments: [
			{
				name: "število1",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			},
			{
				name: "število2",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Vrne število, ki jih biti shift_amount premaknejo na desno.",
		arguments: [
			{
				name: "število",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			},
			{
				name: "shift_amount",
				description: "je število bitov, ki jih želite premakniti"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Vrne bitno vrednost »Exclusive Or« dveh števil.",
		arguments: [
			{
				name: "number1",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			},
			{
				name: "number2",
				description: "je decimalni predstavnik binarnega števila, ki ga želite oceniti"
			}
		]
	},
	{
		name: "CEILING",
		description: "Zaokroži število navzgor, na najbližji mnogokratnik značilnega števila.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki jo želite zaokrožiti."
			},
			{
				name: "pomembnost",
				description: "je mnogokratnik, s katerim želite zaokroževati."
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Zaokroži število na najbližje celo število ali večkratnik osnove navzgor.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki jo želite zaokrožiti"
			},
			{
				name: "osnova",
				description: "je večkratnik, na katerega želite zaokrožiti"
			},
			{
				name: "način",
				description: "če je dana in je neničelna, bo ta funkcija zaokrožila na število, ki ni nič"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Zaokroži število navzgor na najbližje celo število ali najbližji mnogokratnik značilnega števila.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki jo želite zaokrožiti."
			},
			{
				name: "pomembnost",
				description: "je mnogokratnik, s katerim želite zaokroževati."
			}
		]
	},
	{
		name: "CELL",
		description: "Vrne informacijo o oblikovanju, mestu ali vsebini prve celice glede na vrstni red branja lista v sklicu.",
		arguments: [
			{
				name: "vrsta_informacij",
				description: "je besedilo, ki navaja vrsto želene informacije o celici."
			},
			{
				name: "sklic",
				description: "je celica, o kateri želite informacije"
			}
		]
	},
	{
		name: "CHAR",
		description: "Vrne znak, ki ga določa kodno število, iz nabora znakov vašega računalnika.",
		arguments: [
			{
				name: "število",
				description: "je število med 1 in 255, s katerim določite želeni znak."
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Vrne verjetnost dvorepe porazdelitve Hi-kvadrat.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti porazdelitev, in je nenegativno število"
			},
			{
				name: "stop_prostosti",
				description: "je število stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "CHIINV",
		description: "Vrne inverzno verjetnost dvorepe porazdelitve Hi-kvadrat.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s porazdelitvijo Hi-kvadrat, in je vrednost med 0 in vključno 1."
			},
			{
				name: "stop_prostosti",
				description: "je število stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Vrne levorepo verjetnost porazdelitve hi-kvadrat.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti porazdelitev, in je nenegativno število."
			},
			{
				name: "stop_prostosti",
				description: "je število stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost, ki določi, kaj naj funkcija vrne: kumulativno porazdelitveno funkcijo = TRUE; porazdelitev gostote verjetnosti = FALSE."
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Vrne levorepo verjetnost porazdelitve hi-kvadrat.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti porazdelitev, in je nenegativno število."
			},
			{
				name: "stop_prostosti",
				description: "je število stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Vrne inverzno verjetnost levorepe porazdelitve hi-kvadrat.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s porazdelitvijo hi-kvadrat, in je vrednost med 0 in vključno 1."
			},
			{
				name: "stop_prostosti",
				description: "je število stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Vrne inverzno verjetnost desnorepe porazdelitve hi-kvadrat.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s porazdelitvijo hi-kvadrat, in je vrednost med 0 in vključno 1."
			},
			{
				name: "stop_prostosti",
				description: "je število stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Vrne preskus neodvisnosti: vrednost iz porazdelitve hi-kvadrat za statistične in ustrezne stopnje prostosti.",
		arguments: [
			{
				name: "dejanski_obseg",
				description: "je obseg podatkov z opazovanji za preskus pričakovanih vrednosti."
			},
			{
				name: "pričakovani_obseg",
				description: "je obseg podatkov z razmerji vsot vrst in vsot stolpcev proti skupni vsoti."
			}
		]
	},
	{
		name: "CHITEST",
		description: "Vrne preizkus neodvisnosti: vrednost iz porazdelitve hi-kvadrat za statistične in ustrezne stopnje prostosti.",
		arguments: [
			{
				name: "dejanski_obseg",
				description: "je obseg podatkov, ki vsebuje opazovanja za preizkus pričakovanih vrednosti."
			},
			{
				name: "pričakovani_obseg",
				description: "je obseg podatkov, ki vsebuje razmerje vsot vrst in vsot stolpcev proti skupni vsoti."
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Izbere vrednost ali dejanje, ki naj se izvede, s seznama vrednosti, ki temelji na indeksni številki.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "št_indeksa",
				description: "navaja, kateri argument z vrednostjo je izbran. »Št_indeksa« mora biti število med 1 in 254, formula ali sklic na število med 1 in 254."
			},
			{
				name: "vrednost1",
				description: "so števila med 1 in 254, sklici na celice, določena imena, formule, funkcije ali besedilni argumenti, med katerimi izbere CHOOSE"
			},
			{
				name: "vrednost2",
				description: "so števila med 1 in 254, sklici na celice, določena imena, formule, funkcije ali besedilni argumenti, med katerimi izbere CHOOSE"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Iz besedila odstrani vse znake, ki se ne tiskajo.",
		arguments: [
			{
				name: "besedilo",
				description: "je kateri koli delovni list, s katerega želimo odstraniti znake, ki se ne tiskajo."
			}
		]
	},
	{
		name: "CODE",
		description: "Vrne številsko kodo prvega znaka v besedilnem nizu, v naboru znakov, ki ga uporablja računalnik.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo, ki mu določate kodo prvega znaka."
			}
		]
	},
	{
		name: "COLUMN",
		description: "Vrne številko stolpca danega sklica.",
		arguments: [
			{
				name: "sklic",
				description: "je celica ali obseg celic, ki jim določate številko stolpca. Če tega ni, je uporabljena celica, ki vsebuje funkcijo COLUMN."
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Vrne število stolpcev v matriki ali v sklicu.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali matrična formula oziroma sklic na obseg celic, za katerega določate število stolpcev."
			}
		]
	},
	{
		name: "COMBIN",
		description: "Vrne število kombinacij za dano število predmetov.",
		arguments: [
			{
				name: "število",
				description: "je število predmetov."
			},
			{
				name: "izbrano_število",
				description: "je število predmetov v vsaki kombinaciji."
			}
		]
	},
	{
		name: "COMBINA",
		description: "Vrne število kombinacij za dano število elementov (s ponovitvami), ki jih je mogoče izbrati med skupnim številom elementov.",
		arguments: [
			{
				name: "število",
				description: "je skupno število elementov"
			},
			{
				name: "število_izbrano",
				description: "je število elementov v vsaki kombinaciji"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Pretvori realne in imaginarne koeficiente v kompleksna števila.",
		arguments: [
			{
				name: "realno_št",
				description: "je realni koeficient kompleksnega števila"
			},
			{
				name: "i_št",
				description: " je imaginarni koeficient kompleksnega števila"
			},
			{
				name: "pripona",
				description: "je pripona za imaginarno komponento kompleksnega števila"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Združi več besedilnih nizov v enega.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "besedilo1",
				description: "od 1 do 255 besedilnih nizov, ki naj se združijo v en besedilni niz, in so lahko besedilni nizi, števila ali sklici na eno samo celico"
			},
			{
				name: "besedilo2",
				description: "od 1 do 255 besedilnih nizov, ki naj se združijo v en besedilni niz, in so lahko besedilni nizi, števila ali sklici na eno samo celico"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Vrne interval zaupanja za populacijsko srednjo vrednost.",
		arguments: [
			{
				name: "alfa",
				description: "je raven pomembnosti, ki se uporabi za izračun ravni zaupanja, in je število, ki je večje od 0 in manjše od 1"
			},
			{
				name: "standardni_odklon",
				description: "je domnevno znan standardni odklon populacije za obseg podatkov. Standardni_odklon mora biti večje kot 0"
			},
			{
				name: "velikost",
				description: "je velikost vzorca"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Z normalno porazdelitvijo vrne interval zaupanja za populacijsko srednjo vrednost.",
		arguments: [
			{
				name: "alfa",
				description: "je raven pomembnosti za izračun ravni zaupanja, in je število, ki je večje od 0 in manjše od 1."
			},
			{
				name: "standardni_odklon",
				description: "je domnevno znan standardni odklon populacije za obseg podatkov. »Standardni_odklon« mora biti večje kot 0."
			},
			{
				name: "velikost",
				description: "je velikost vzorca."
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "S Studentovo t-porazdelitvijo vrne interval zaupanja za populacijsko srednjo vrednost.",
		arguments: [
			{
				name: "alfa",
				description: "je raven pomembnosti za izračun ravni zaupanja, in je število, ki je večje od 0 in manjše od 1."
			},
			{
				name: "standardni_odklon",
				description: "je domnevno znan standardni odklon populacije za obseg podatkov. »Standardni_odklon« mora biti večje kot 0."
			},
			{
				name: "velikost",
				description: "je velikost vzorca."
			}
		]
	},
	{
		name: "CONVERT",
		description: "Pretvori število iz enega merskega sistema v drugega.",
		arguments: [
			{
				name: "število",
				description: "je vrednost v iz_enot za pretvorbo"
			},
			{
				name: "od_enote",
				description: "je enot za število"
			},
			{
				name: "do_enote",
				description: "je enot za rezultat"
			}
		]
	},
	{
		name: "CORREL",
		description: "Vrne korelacijski koeficient med dvema naboroma podatkov.",
		arguments: [
			{
				name: "matrika1",
				description: "so vrednosti obsega celic. Vrednosti morajo biti števila, imena, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "matrika2",
				description: "je drug obseg celic z vrednostmi. Vrednosti morajo biti števila, imena, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "COS",
		description: "Vrne kosinus kota.",
		arguments: [
			{
				name: "število",
				description: "je kot v radianih, za katerega želite poiskati kosinus"
			}
		]
	},
	{
		name: "COSH",
		description: "Vrne hiperbolični kosinus števila.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število."
			}
		]
	},
	{
		name: "COT",
		description: "Vrne kotangens kota.",
		arguments: [
			{
				name: "number",
				description: "je kot v radianih, za katerega želite dobiti kotangens"
			}
		]
	},
	{
		name: "COTH",
		description: "Vrne hiperbolični kotangens števila.",
		arguments: [
			{
				name: "number",
				description: "je kot v radianih, za katerega želite dobiti kotangens"
			}
		]
	},
	{
		name: "COUNT",
		description: "Prešteje celice v obsegu, ki vsebujejo števila.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 argumentov, ki vsebujejo ali se sklicujejo na različne vrste podatkov, pri tem pa se preštevajo le števila"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 argumentov, ki vsebujejo ali se sklicujejo na različne vrste podatkov, pri tem pa se preštevajo le števila"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Prešteje neprazne celice v obsegu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 argumentov, ki predstavljajo vrednosti in celice, ki jih želite prešteti. Vrednosti so lahko informacija katere koli vrste"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 argumentov, ki predstavljajo vrednosti in celice, ki jih želite prešteti. Vrednosti so lahko informacija katere koli vrste"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Izračuna število praznih celic v navedenem obsegu.",
		arguments: [
			{
				name: "obseg",
				description: "je obseg, v katerem želite prešteti prazne celice."
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Prešteje celice v obsegu, ki se ujemajo z danim pogojem.",
		arguments: [
			{
				name: "obseg",
				description: "je obseg celic, v katerem želite prešteti neprazne celice."
			},
			{
				name: "pogoji",
				description: "je pogoj v obliki števila, izraza ali besedila, ki določa, katere celice naj se preštejejo."
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Prešteje celice, ki jih navaja dani niz pogojev ali kriterijev.",
		arguments: [
			{
				name: "obseg_pogojev",
				description: "je obseg celic, katere želite ovrednotiti za določen pogoj"
			},
			{
				name: "pogoji",
				description: "je pogoj v obliki števila, izraza ali besedila, ki določa, katere celice bodo preštete"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Vrne število dni od začetka obdobja vrednostnega papirja do datuma poravnave.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "pogostost",
				description: "je število plačil vrednostnega papirja na leto"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Vrne naslednji datum poravnave po datumu poravnave.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "pogostost",
				description: "je število plačil vrednostnega papirja na leto"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Vrne število vrednostnih papirjev, ki bodo plačani med datumom poravnave in datumom zapadlosti.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "pogostost",
				description: "je število plačil vrednostnega papirja na leto"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Vrne prejšnji datum vrednostnega papirja pred datumom poravnave.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "pogostost",
				description: "je število plačil vrednostnega papirja na leto"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "COVAR",
		description: "Vrne kovarianco, ki je povprečje produktov odklonov za vsak par podatkovnih točk v dveh podatkovnih množicah.",
		arguments: [
			{
				name: "matrika1",
				description: "je prvi obseg celic celih števil, ki morajo biti števila, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "matrika2",
				description: "je drugi obseg celic celih števil, ki morajo biti števila, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Vrne populacijsko kovarianco, ki je povprečje produktov odklonov za vsak par podatkovnih točk v dveh podatkovnih množicah.",
		arguments: [
			{
				name: "matrika1",
				description: "je prvi obseg celic celih števil, ki morajo biti števila, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "matrika2",
				description: "je drugi obseg celic celih števil, ki morajo biti števila, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Vrne vzorčno kovarianco, ki je povprečje rezultatov odklonov za vsak par podatkovnih točk v dveh podatkovnih množicah.",
		arguments: [
			{
				name: "matrika1",
				description: "je prvi obseg celic celih števil, ki morajo biti števila, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "matrika2",
				description: "je drugi obseg celic celih števil, ki morajo biti števila, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Vrne najmanjšo vrednost, za katero je kumulativna binomska porazdelitev večja ali enaka vrednosti kriterija.",
		arguments: [
			{
				name: "poskusi",
				description: "je število Bernoullijevih poskusov."
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspešnosti posameznega poskusa in je število med 0 in vključno 1."
			},
			{
				name: "alfa",
				description: "je vrednost kriterija in je število med 0 in vključno 1."
			}
		]
	},
	{
		name: "CSC",
		description: "Vrne kosekans kota.",
		arguments: [
			{
				name: "number",
				description: "je kotv radianih, ki mu želite izračunati kosekans"
			}
		]
	},
	{
		name: "CSCH",
		description: "Vrne hiperbolični kosekans kota.",
		arguments: [
			{
				name: "number",
				description: "je kot v radianih, ki mu želite izračunati hiperbolični kosekans"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Vrne kumulativne obresti, plačane med dvema obdobjema.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera"
			},
			{
				name: "št_obdobij",
				description: "je skupno število plačilnih obdobij"
			},
			{
				name: "sv",
				description: "je sedanja vrednost"
			},
			{
				name: "začetno_obdobje",
				description: "je prvo obdobje v izračunu"
			},
			{
				name: "končno_obdobje",
				description: "je zadnje obdobje v izračunu"
			},
			{
				name: "vrsta",
				description: "je čas plačila"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Vrne kumulativno glavnico, plačano s posojilom med dvema obdobjema.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera"
			},
			{
				name: "št_obdobij",
				description: "je skupno število plačilnih obdobij"
			},
			{
				name: "sv",
				description: "je sedanja vrednost"
			},
			{
				name: "začetno_obdobje",
				description: "je prvo obdobje v izračunu"
			},
			{
				name: "končno_obdobje",
				description: "je zadnje obdobje v izračunu"
			},
			{
				name: "vrsta",
				description: "je čas plačila"
			}
		]
	},
	{
		name: "DATE",
		description: "Vrne število, ki predstavlja datum v kodi za datum in uro programa Spreadsheet.",
		arguments: [
			{
				name: "leto",
				description: "je število od 1900 do 9999 v Spreadsheetu za Windows ali od 1904 do 9999 v Spreadsheetu za Macintosh."
			},
			{
				name: "mesec",
				description: "je število od 1 do 12, ki predstavlja mesec v letu."
			},
			{
				name: "dan",
				description: "je število od 1 do 31, ki predstavlja dan v mesecu."
			}
		]
	},
	{
		name: "DATEDIF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATEVALUE",
		description: "Pretvori datum v obliki besedila v število, ki predstavlja datum v Spreadsheetovi kodi za datum in uro.",
		arguments: [
			{
				name: "besedilo_datuma",
				description: "je besedilo, ki predstavlja datum v Spreadsheetovi obliki datuma med 1.1.1900 (Windows) ali 1.1.1904 (Macintosh) in 31.12.9999."
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Izračuna povprečje vrednosti v stolpcu, na seznamu ali v zbirki podatkov, ki ustrezajo navedenim pogojem.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DAY",
		description: "Vrne dan v mesecu, število med 1 in 31.",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število v datumsko-časovni kodi, ki jo uporablja Spreadsheet"
			}
		]
	},
	{
		name: "DAYS",
		description: "Vrne število dni med dvema datumoma.",
		arguments: [
			{
				name: "končni_datum",
				description: "začetni_datum in končni_datum sta datuma, katerih število dni želite dobiti"
			},
			{
				name: "začetni_datum",
				description: "začetni_datum in končni_datum sta datuma, katerih število dni želite dobiti"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Vrne število dni med dvema datumoma v 360-dnevnem letu (dvanajst 30-dnevnih mesecev).",
		arguments: [
			{
				name: "začetni_datum",
				description: "Začetni_datum in Končni_datum sta dva datuma, med katerima želite poznati število dni."
			},
			{
				name: "končni_datum",
				description: "Začetni_datum in Končni_datum sta dva datuma, med katerima želite poznati število dni."
			},
			{
				name: "metoda",
				description: "je logična vrednost, ki določa metodo za izračun: ZDA (NASD) = FALSE ali izpuščeno; Evropska = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Vrne amortizacijo sredstva za določeno obdobje po metodi fiksnopojemajočega salda.",
		arguments: [
			{
				name: "stroški",
				description: "je začetna cena sredstva."
			},
			{
				name: "vrednost_po_amor",
				description: "je rešena vrednost na koncu življenjske dobe sredstva."
			},
			{
				name: "št_obdobij",
				description: "je število obdobij, prek katerih se amortizira sredstvo (imenovano tudi življenjska doba sredstva)."
			},
			{
				name: "obdobje",
				description: "je obdobje, za katerega želite izračunati amortizacijo. Obdobje mora biti v istih enotah kot življenjska doba."
			},
			{
				name: "mesec",
				description: "je število mesecev v prvem letu. Kadar jih izpustimo, se privzame 12 mesecev."
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Prešteje celice s števili v polju (stolpcu) zapisov zbirke podatkov, ki ustrezajo pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Prešteje neprazne celice v polju (stolpcu) zapisov zbirke podatkov, ki ustrezajo pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DDB",
		description: "Vrne amortizacijo sredstva za določeno obdobje z metodo dvojnopojemajočega salda ali s kakšno drugo metodo, ki jo določite.",
		arguments: [
			{
				name: "stroški",
				description: "je začetna cena sredstva."
			},
			{
				name: "vrednost_po_amor",
				description: "je rešena vrednost na koncu življenjske dobe sredstva."
			},
			{
				name: "št_obdobij",
				description: "je število obdobij, prek katerih se amortizira sredstvo (imenovano tudi življenjska doba sredstva)."
			},
			{
				name: "obdboje",
				description: "je obdobje, za katerega želite izračunati amortizacijo. Obdobje mora biti v istih enotah kot življenjska doba."
			},
			{
				name: "faktor",
				description: "je hitrost upadanja salda. Izpustitev faktorja pomeni, da je ta 2 (metoda dvojnopojemajočega salda)."
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Pretvori desetiško število v dvojiško.",
		arguments: [
			{
				name: "število",
				description: " je desetiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Pretvori desetiško število v šestnajstiško.",
		arguments: [
			{
				name: "število",
				description: "je desetiško celo število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Pretvori desetiško število v osmiško.",
		arguments: [
			{
				name: "število",
				description: "je desetiško celo število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Pretvori besedilo, ki predstavlja številko, v danem korenu v decimalno število.",
		arguments: [
			{
				name: "število",
				description: "je število, ki ga želite pretvoriti"
			},
			{
				name: "koren",
				description: "je osnovni koren števila, ki ga želite pretvoriti"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Pretvori radiane v stopinje.",
		arguments: [
			{
				name: "kot",
				description: "je kot v radianih, ki ga želite pretvoriti."
			}
		]
	},
	{
		name: "DELTA",
		description: "Preskusi, ali sta dve števili enaki.",
		arguments: [
			{
				name: "število1",
				description: "je prvo število"
			},
			{
				name: "število2",
				description: "je drugo število"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Vrne vsoto kvadratov odklonov podatkovnih točk iz njihovih vzorčnih srednjih vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 argumentov ali matriko ali matriko sklicev, za katere želite določiti vsoto kvadratnih odklonov uporabo funkcije DEVSQ"
			},
			{
				name: "število2",
				description: "od 1 do 255 argumentov ali matriko ali matriko sklicev, za katere želite določiti vsoto kvadratnih odklonov uporabo funkcije DEVSQ"
			}
		]
	},
	{
		name: "DGET",
		description: "Iz zbirke podatkov izvleče en zapis, ki ustreza pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je območje celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DISC",
		description: "Vrne stopnjo rabata vrednostnega papirja.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "cena",
				description: "je cena vrednostnega papirja na 100 € imenske vrednosti"
			},
			{
				name: "odkup",
				description: "je amortizacijska vrednost vrednostnega papirja na €100 imenske vrednosti"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "DMAX",
		description: "Vrne največje število v polju (stolpcu) zapisov zbirke podatkov, ki ustreza pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je območje celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DMIN",
		description: "Vrne najmanjše število v polju (stolpcu) zapisov zbirke podatkov, ki ustreza pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je območje celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Pretvori število v besedilo z uporabo valutne oblike.",
		arguments: [
			{
				name: "število",
				description: "je število, sklic na celico s številom ali formula za vrednotenje števila."
			},
			{
				name: "decimalna_mesta",
				description: "je število decimalnih mest desno od decimalne vejice. Število je po potrebi zaokroženo; če izpustite, sta uporabljeni dve decimalni mesti."
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Pretvori ceno v dolarjih, izraženo kot ulomek, v ceno v dolarjih, izraženo kot decimalno število.",
		arguments: [
			{
				name: "ulomek_valuta",
				description: "je število, izraženo kot ulomek"
			},
			{
				name: "imenovalec",
				description: "je celo število, uporabljeno v imenovalcu ulomka"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Pretvori ceno v dolarjih, izraženo kot decimalno število, v ceno v dolarjih, izraženo kot ulomek.",
		arguments: [
			{
				name: "decimalka_valuta",
				description: "je decimalno število"
			},
			{
				name: "imenovalec",
				description: "je celo število, uporabljeno v imenovalcu ulomka"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Zmnoži vrednosti v polju (stolpcu) zapisov zbirke podatkov, ki ustrezajo pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Na osnovi izbranih vnosov zbirke podatkov oceni standardni odklon. Ocena temelji na vzorcu.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Izračuna standardni odklon, ki temelji na celotni populaciji izbranih vnosov zbirke podatkov.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DSUM",
		description: "Sešteje števila v polju (stolpcu) zapisov zbirke podatkov, ki ustrezajo pogojem, ki ste jih določili.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DVAR",
		description: "Na osnovi izbranih vnosov zbirke podatkov oceni varianco. Ocena temelji na vzorcu.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "DVARP",
		description: "Izračuna varianco celotne populacije izbranih vnosov zbirke podatkov.",
		arguments: [
			{
				name: "zbirka_podatkov",
				description: "je obseg celic, ki sestavlja seznam ali zbirko podatkov. Zbirka podatkov je seznam sorodnih podatkov."
			},
			{
				name: "polje",
				description: "je bodisi oznaka stolpca v dvojnih narekovajih bodisi številka, ki predstavlja položaj stolpca na seznamu."
			},
			{
				name: "pogoji",
				description: "je obseg celic s pogoji, ki jih določite sami. Obseg vsebuje oznako stolpca in celico pod njo, ki vsebuje pogoj."
			}
		]
	},
	{
		name: "EDATE",
		description: "Vrne zaporedno število datuma, ki je določeno število mesecev pred ali po začetnem datumu.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum"
			},
			{
				name: "meseci",
				description: " je število mesecev pred ali po začetnem datumu"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Vrne efektivno letno obrestno mero.",
		arguments: [
			{
				name: "nominalna_obr_mera",
				description: "je nominalna obrestna mera"
			},
			{
				name: "št_obdobij_leto",
				description: "je število združenih obdobij na leto"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Vrne niz v obliki URL-ja.",
		arguments: [
			{
				name: "besedilo",
				description: "je niz, ki bo podan v obliki URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Vrne zaporedno število zadnjega dneva meseca pred ali po navedenem številu mesecev.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum"
			},
			{
				name: "meseci",
				description: " je število mesecev pred ali po začetnem datumu"
			}
		]
	},
	{
		name: "ERF",
		description: "Vrne funkcijo napake.",
		arguments: [
			{
				name: "spodnja_meja",
				description: "je spodnja meja za integriranje ERF"
			},
			{
				name: "zgornja_meja",
				description: "je zgornja meja za integriranje ERF"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Vrne funkcijo napake.",
		arguments: [
			{
				name: "X",
				description: "je spodnja meja za integriranje ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Vrne komplementarno funkcijo napake.",
		arguments: [
			{
				name: "x",
				description: "je spodnja meja za integriranje ERF"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Vrne komplementarno funkcijo napake.",
		arguments: [
			{
				name: "X",
				description: "je spodnja meja za integriranje ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Vrne število, ki ustreza vrednosti napake.",
		arguments: [
			{
				name: "vrednost_napake",
				description: "je vrednost napake, za katero želite poiskati ID, in je lahko dejanska vrednost napake ali sklic na celico, v kateri je vrednost napake"
			}
		]
	},
	{
		name: "EVEN",
		description: "Zaokroži pozitivno število navzgor in negativno število navzdol na najbližje sodo celo število.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki se zaokroži."
			}
		]
	},
	{
		name: "EXACT",
		description: "Preveri, ali sta dva besedilna niza popolnoma enaka, in vrne TRUE ali FALSE. EXACT loči velike in male črke.",
		arguments: [
			{
				name: "besedilo1",
				description: "je prvi besedilni niz"
			},
			{
				name: "besedilo2",
				description: "je drugi besedilni niz"
			}
		]
	},
	{
		name: "EXP",
		description: " Vrne e na potenco navedenega števila.",
		arguments: [
			{
				name: "število",
				description: "je eksponent z osnovo e. Konstanta e je enaka 2,71828182845904 in je osnova naravnega logaritma."
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Vrne eksponentno porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je vrednost funkcije in je nenegativno število."
			},
			{
				name: "lambda",
				description: "je vrednost parametra in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost, ki naznači, kaj naj funkcija vrne: kumulativno porazdelitveno funkcijo = TRUE; porazdelitev gostote verjetnosti = FALSE."
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Vrne eksponentno porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je vrednost funkcije in je nenegativno število"
			},
			{
				name: "lambda",
				description: "je vrednost parametra in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost, ki označi, kaj naj funkcija vrne: kumulativno porazdelitveno funkcijo = TRUE; porazdelitev gostote verjetnosti = FALSE."
			}
		]
	},
	{
		name: "F.DIST",
		description: "Vrne F-porazdelitev (levorepo) verjetnosti (stopnja razpršenosti) za dve podatkovni množici.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri se vrednoti funkcija, in je nenegativno število."
			},
			{
				name: "stop_prostosti1",
				description: "je števec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "stop_prostosti2",
				description: "je imenovalec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost, ki določi, kaj naj funkcija vrne: kumulativno porazdelitveno funkcijo = TRUE; porazdelitev gostote verjetnosti = FALSE."
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Vrne F-porazdelitev (desnorepo) verjetnosti (stopnja razpršenosti) za dve podatkovni množici.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri se vrednoti funkcija, in je nenegativno število"
			},
			{
				name: "stop_prostosti1",
				description: "je števec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "stop_prostosti2",
				description: "je imenovalec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "F.INV",
		description: "Vrne inverzno F verjetnostno (levorepo) porazdelitev: če p = F.DIST(x, ...), potem F.INV(p, ...) = x.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s F-kumulativno porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "stop_prostosti1",
				description: "je števec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "stop_prostosti2",
				description: "je imenovalec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Vrne inverzno F verjetnostno (desnorepo) porazdelitev: če p = F.DIST.RT(x, ...), potem F.INV.RT(p, ...) = x.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s F-kumulativno porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "stop_prostosti1",
				description: "je števec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "stop_prostosti2",
				description: "je imenovalec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "F.TEST",
		description: "Vrne rezultat preskusa F, ki je dvorepa verjetnost, da se varianci argumentov »matrika1« in »matrika2« bistveno ne razlikujeta.",
		arguments: [
			{
				name: "matrika1",
				description: "je prva matrika ali obseg podatkov, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila (presledki se prezrejo)."
			},
			{
				name: "matrika2",
				description: "je druga matrika ali obseg podatkov, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila (presledki se prezrejo)."
			}
		]
	},
	{
		name: "FACT",
		description: "Vrne fakulteto števila, ki je enaka 1*2*3*...*število.",
		arguments: [
			{
				name: "število",
				description: "je nenegativno število, ki mu želite določiti fakulteto."
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Vrne dvojno fakulteto števila.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, za katero želite dvojno fakulteto"
			}
		]
	},
	{
		name: "FALSE",
		description: "Vrne logično vrednost FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Vrne F-porazdelitev (dvorepo) verjetnosti (stopnja razpršenosti) za dve podatkovni množici.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri se vrednoti funkcija, in je nenegativno število."
			},
			{
				name: "stop_prostosti1",
				description: "je števec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "stop_prostosti2",
				description: "je imenovalec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "FIELD",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIELDPICTURE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			},
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "FIND",
		description: "Vrne začetni položaj besedilnega niza v drugem besedilnem nizu. FIND loči velike in male črke.",
		arguments: [
			{
				name: "iskano_besedilo",
				description: "je besedilo, ki ga želite poiskati. Uporabite dvojne narekovaje (prazno besedilo), če želite primerjati s prvim znakom v argumentu »V_besedilu«; uporaba nadomestnih znakov ni dovoljena."
			},
			{
				name: "v_besedilu",
				description: "je besedilo, ki vsebuje iskano besedilo."
			},
			{
				name: "začetni_znak",
				description: "določi znak, pri katerem se iskanje začne. Prvi znak v argumentu »V_besedilu« je znak številka 1. Če izpuščeno, velja »Začetni_znak = 1«."
			}
		]
	},
	{
		name: "FINV",
		description: "Vrne inverzno verjetnostno F-porazdelitev (dvorepo): če p = FDIST(x,...), potem FINV(p,...) = x.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana s F-kumulativno porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "stop_prostosti1",
				description: "je števec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			},
			{
				name: "stop_prostosti2",
				description: "je imenovalec stopenj prostosti, ki je število med 1 in 10^10 in ni 10^10."
			}
		]
	},
	{
		name: "FISHER",
		description: "Vrne Fischerjevo transformacijo.",
		arguments: [
			{
				name: "x",
				description: "je številska vrednost, ki jo želite pretvoriti, in je število med -1 in 1, ni pa -1 ali 1."
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Vrne inverzno Fischerjevo transformacijo: če y = FISHER(x), potem FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "je vrednost, ki jo želite pretvoriti."
			}
		]
	},
	{
		name: "FIXED",
		description: "Zaokroži številko na določeno število decimalnih mest in vrne rezultat kot besedilo z vejicami ali brez njih.",
		arguments: [
			{
				name: "število",
				description: "je število, ki ga želite zaokrožiti in pretvoriti v besedilo."
			},
			{
				name: "decimalna_mesta",
				description: "je število števk desno od decimalne vejice. Če nič ne vnesete, sta decimalki dve."
			},
			{
				name: "brez_vejic",
				description: "je logična vrednost: ne prikaže ločil tisočic v vrnjenem besedilu = TRUE; prikaže ločila v vrnjenem besedilu = FALSE ali ni določena."
			}
		]
	},
	{
		name: "FLOOR",
		description: "Zaokroži število navzdol do najbližjega mnogokratnika značilnega števila.",
		arguments: [
			{
				name: "število",
				description: "je številska vrednost, ki jo želite zaokrožiti."
			},
			{
				name: "pomembnost",
				description: "je mnogokratnik, na katerega želite zaokroževati. Število in značilno število morata biti ali obe pozitivni ali obe negativni."
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Zaokroži število na najbližje celo število ali večkratnik osnove navzdol.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki jo želite zaokrožiti"
			},
			{
				name: "osnova",
				description: "je večkratnik, na katerega želite zaokrožiti"
			},
			{
				name: "način",
				description: "če je dana in je neničelna, bo ta funkcija zaokrožila na število, ki se približa številu nič"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Zaokroži število navzdol na najbližje celo število ali najbližji mnogokratnik značilnega števila.",
		arguments: [
			{
				name: "število",
				description: "je številska vrednost, ki jo želite zaokrožiti."
			},
			{
				name: "pomembnost",
				description: "je mnogokratnik, na katerega želite zaokroževati"
			}
		]
	},
	{
		name: "FORECAST",
		description: "Izračuna ali predvidi bodočo vrednost vzdolž linearnega trenda z uporabo obstoječih vrednosti.",
		arguments: [
			{
				name: "x",
				description: "je podatkovna točka, ki ji želite predvideti vrednost, in mora biti številska vrednost."
			},
			{
				name: "znani_y-i",
				description: "je odvisna matrika ali obseg številskih podatkov."
			},
			{
				name: "znani_x-i",
				description: "je neodvisna matrika ali obseg številskih podatkov. Varianca parametra ne sme biti nič."
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Vrne formulo kot niz.",
		arguments: [
			{
				name: "sklic",
				description: "je sklic na formulo"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Preračuna, kako pogosto se vrednosti pojavljajo v obsegu vrednosti, in nato vrne navpično matriko števil, ki ima en element več kot »matrika_predalov«.",
		arguments: [
			{
				name: "matrika_podatkov",
				description: "je matrika vrednosti ali sklic na množico vrednosti, v kateri želite šteti frekvence (presledki in besedilo so prezrti)."
			},
			{
				name: "matrika_predalov",
				description: "je matrika ali sklic na intervale, v katerih želite razvrščati v skupine vrednosti iz »matrika_podatkov«."
			}
		]
	},
	{
		name: "FTEST",
		description: "Vrne rezultat F-preizkusa, ki je dvorepa verjetnost, da se varianci argumentov »matrika1« in »matrika2« bistveno ne razlikujeta.",
		arguments: [
			{
				name: "matrika1",
				description: "je prva matrika ali obseg podatkov, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila (presledki se prezrejo)."
			},
			{
				name: "matrika2",
				description: "je druga matrika ali obseg podatkov, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila (presledki se prezrejo)."
			}
		]
	},
	{
		name: "FV",
		description: "Vrne bodočo vrednost naložbe, ki temelji na periodičnih, enakih plačilih in nespremenljivi obrestni meri.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera na obdobje. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "obdobja",
				description: "je skupno število plačilnih obdobij na naložbo."
			},
			{
				name: "plačilo",
				description: "je plačilo, nakazano v vsakem obdobju; ne sme se spreminjati prek življenja naložbe."
			},
			{
				name: "sv",
				description: "je sedanja vrednost ali enkratni znesek, ki predstavlja sedanjo vrednost vrste bodočih plačil. Če je izpuščena, je sedanja vrednost 0."
			},
			{
				name: "vrsta",
				description: "je število, ki označuje, kdaj so plačila dospela: na začetku obdobja = 1; na koncu obdobja = 0 ali ni določeno."
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Vrne bodočo vrednost začetne glavnice po uporabi niza sestavljenih obrestnih mer.",
		arguments: [
			{
				name: "glavnica",
				description: "je sedanja vrednost"
			},
			{
				name: "razpored",
				description: "je matrika obrestnih mer, ki bodo uporabljene"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Vrne vrednost funkcije Gama.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, ki ji želite izračunati funkcijo Gama"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Vrne gama porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti porazdelitev, in je nenegativno število."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in je pozitivno število. Če je beta = 1, GAMMA.DIST vrne standardno gama porazdelitev."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: vrne kumulativno porazdelitveno funkcijo = TRUE; vrne verjetnostno masno funkcijo = FALSE ali izpuščeno."
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Vrne inverzno gama kumulativno porazdelitev: če je p = GAMMA.DIST(x, ...), potem je GAMMA.INV(p, ...) = x.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z gama porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in je pozitivno število. Kadar je beta = 1, GAMMA.INV vrne inverzno standardno gama porazdelitev."
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Vrne gama porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti porazdelitev, in je nenegativno število."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in je pozitivno število. Če je beta = 1, GAMMADIST vrne standardno gama porazdelitev."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: vrne kumulativno porazdelitveno funkcijo = TRUE; vrne verjetnostno masno funkcijo = FALSE ali izpuščeno."
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Vrne inverzno gama kumulativno porazdelitev: če je p = GAMMADIST(x,...), potem je GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z gama porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in je pozitivno število. Kadar je beta = 1, GAMMAINV vrne inverzno standardno gama porazdelitev."
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Vrne naravni logaritem gama funkcije.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, za katero želite izračunati pozitivno število GAMMALN."
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Vrne naravni logaritem gama funkcije.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, za katero želite izračunati pozitivno število GAMMALN.PRECISE"
			}
		]
	},
	{
		name: "GAUSS",
		description: "",
		arguments: [
		]
	},
	{
		name: "GCD",
		description: "Vrne največji skupni delitelj.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "je 1 do 255 vrednosti"
			},
			{
				name: "število2",
				description: "je 1 do 255 vrednosti"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Vrne geometrično srednjo vrednost matrike ali obsega pozitivnih številskih podatkov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati srednjo vrednost"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati srednjo vrednost"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Preskusi, ali je število večje od mejne vrednosti.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki se preverja po korakih"
			},
			{
				name: "korak",
				description: "je mejna vrednost"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Ekstrahira podatke, ki so shranjeni v vrtilni tabeli.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "podatkovno_polje",
				description: "je ime podatkovnega polja, iz katerega se podatki ekstrahirajo"
			},
			{
				name: "vrtilna_tabela",
				description: "je sklic na celico ali na obseg celic v vrtilni tabeli, ki vsebuje podatke, katere želite pridobiti."
			},
			{
				name: "polje",
				description: "polje, na katerega se sklicujete"
			},
			{
				name: "element",
				description: "element polja, na katerega se sklicujete"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Vrne števila v trendu eksponentne rasti, ujemajočem z znanimi podatkovnimi točkami.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je nabor y-vrednosti, ki jih že poznate v odnosu y = b*m^x, in je matrika ali obseg pozitivnih števil."
			},
			{
				name: "znani_x-i",
				description: "je izbirni nabor x-vrednosti, ki jih morda že poznate v odnosu y = b*m^x, in je matrika ali obseg enake velikosti kot znani y-i."
			},
			{
				name: "novi_x-i",
				description: "so nove x-vrednosti, za katere želite, da GROWTH vrne ustrezne y-vrednosti."
			},
			{
				name: "konstanta",
				description: "je logična vrednost: konstanta b se izračuna normalno, če je konstanta enaka TRUE; b je 1, če je konstanta enaka FALSE ali vrednost ni določena."
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Vrne harmonično srednjo vrednost za nabor pozitivnih števil. Harmonična srednja vrednost je obratna vrednost aritmetične srednje vrednosti za obratne (recipročne) vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati harmonično srednjo vrednost"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati harmonično srednjo vrednost"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Pretvori šestnajstiško število v dvojiško.",
		arguments: [
			{
				name: "število",
				description: "je šestnajstiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Pretvori šestnajstiško število v desetiško.",
		arguments: [
			{
				name: "število",
				description: "je šestnajstiško število, ki ga želite pretvoriti"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Pretvori šestnajstiško število v osmiško.",
		arguments: [
			{
				name: "število",
				description: " je šestnajstiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Poišče vrednost v zgornji vrstici tabele ali matrike vrednosti in vrne vrednost iz istega stolpca in vrstice, ki jo vi določite.",
		arguments: [
			{
				name: "iskana_vrednost",
				description: "je vrednost, ki naj se poišče v prvi vrstici tabele, in je lahko vrednost, sklic ali besedilni niz."
			},
			{
				name: "matrika_tabele",
				description: "je tabela, v kateri se iščejo podatki, in vsebuje besedilo, števila ali logične vrednosti. Table_array je lahko sklic na obseg ali na ime obsega."
			},
			{
				name: "št_indeksa_vrstice",
				description: "je številka vrstice v table_array, iz katere naj se vrne ujemajoča vrednost. Prva vrstica vrednosti v tabeli je vrstica 1."
			},
			{
				name: "obseg_iskanja",
				description: "je logična vrednost: če želite, da bo ujemanje približno (urejeno v naraščajočem vrstnem redu), jo nastavite na TRUE ali pustite prazno; če želite natančno ujemanje, pa naj bo FALSE."
			}
		]
	},
	{
		name: "HOUR",
		description: "Vrne uro, ki je število med 0 (polnoč) in 23 (23:00).",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število v datumsko-časovni kodi, ki jo uporablja Spreadsheet, ali besedilo v časovni obliki, na primer 16:48:00 ali 4:48:00 PM"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Ustvari bližnjico ali skok, ki odpre dokument, shranjen na vašem disku, v omrežnem strežniku ali v internetu.",
		arguments: [
			{
				name: "mesto_povezave",
				description: "je besedilo, ki označuje pot in datotečno ime dokumenta, ki ga želite odpreti, mesto diska, naslov UNC ali pot spletnega naslova."
			},
			{
				name: "prijazno_ime",
				description: "je besedilo ali številka, ki je prikazana v celici. Če jo izpustite, bo v celici prikazano besedilo Link_location."
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Vrne hipergeometrično porazdelitev.",
		arguments: [
			{
				name: "vzorec_s",
				description: "je število uspehov v vzorcu."
			},
			{
				name: "številka_vzorca",
				description: "je velikost vzorca."
			},
			{
				name: "populacija_s",
				description: "je število uspehov v populaciji."
			},
			{
				name: "številka_populacije",
				description: "je velikost populacije."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Vrne hipergeometrično porazdelitev.",
		arguments: [
			{
				name: "vzorec_s",
				description: "je število uspehov v vzorcu."
			},
			{
				name: "velikost_vzorca",
				description: "je velikost vzorca."
			},
			{
				name: "populacija_s",
				description: "je število uspehov v populaciji."
			},
			{
				name: "velikost_populacije",
				description: "je velikost populacije."
			}
		]
	},
	{
		name: "IF",
		description: "Preveri, ali je pogoj izpolnjen, in vrne eno vrednost, če je TRUE, in drugo vrednost, če je FALSE.",
		arguments: [
			{
				name: "logični_test",
				description: "je katera koli vrednost ali izraz, ki se lahko vrednoti s TRUE ali FALSE."
			},
			{
				name: "vrednost_če_je_true",
				description: "je vrednost, ki se vrne, če je »Logični_test« enak TRUE. Če je izpuščena, se vrne TRUE. Gnezdite lahko do sedem funkcij IF."
			},
			{
				name: "vrednost_če_je_false",
				description: "je vrednost, ki se vrne, če je »Logični_test« enak FALSE. Če je izpuščena, se vrne FALSE."
			}
		]
	},
	{
		name: "IFERROR",
		description: "Vrne value_if_error, če je izraz napaka, in vrednost samega izraza.",
		arguments: [
			{
				name: "vrednost",
				description: "je katera koli vrednost ali izraz ali sklic"
			},
			{
				name: "vrednost_če_napaka",
				description: "je katera koli vrednost ali izraz ali sklic"
			}
		]
	},
	{
		name: "IFNA",
		description: "Vrne vrednost, ki jo določite, če se izraz razreši v #N/V, v nasprotnem primeru vrne vrednost izraza.",
		arguments: [
			{
				name: "vrednost",
				description: "je katera koli vrednost ali izraz ali sklic"
			},
			{
				name: "vrednost_če_nv",
				description: "je katera koli vrednost ali izraz ali sklic"
			}
		]
	},
	{
		name: "IMABS",
		description: "Vrne absolutno vrednost (modul) kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega absolutno vrednost želite"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Vrne imaginarni koeficient kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega imaginarni koeficient želite"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Vrne argument q, kot, izražen v radianih.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega argument želite"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Vrne kompleksno izpeljanko kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega izpeljanko želite"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Vrne kosinus kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega kosinus želite"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Vrne hiperbolični kosinus kompleksnega števila.",
		arguments: [
			{
				name: "število",
				description: "je kompleksno število, ki mu želite izračunati hiperbolični kosinus"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Vrne kotangens kompleksnega števila.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksno število, ki mu želite izračunati kotangens"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Vrne kosekans kompleksnega števila.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksno število, ki mu želite izračunati kosekans"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Vrne hiperbolični kosekans kompleksnega števila.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksno število, ki mu želite izračunati hiperbolični kosekans"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Vrne količnik dveh kompleksnih števil.",
		arguments: [
			{
				name: "ištevilo1",
				description: "je kompleksni števec ali deljenec"
			},
			{
				name: "ištevilo2",
				description: "je kompleksni imenovalec ali delitelj"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Vrne eksponent kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega eksponent želite"
			}
		]
	},
	{
		name: "IMLN",
		description: "Vrne naravni logaritem kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega naravni logaritem želite"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Vrne logaritem z osnovo 10 kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega navadni logaritem želite"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Vrne logaritem z osnovo 2 kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega logaritem z osnovo 2 želite"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Vrne kompleksno število potencirano na celo število.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, ki ga želite potencirati"
			},
			{
				name: "število",
				description: "je potenca, na katero želite potencirati kompleksno število"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Vrne zmnožek 1 do 255 kompleksnih števil.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ištevilo1",
				description: "Ištevilo1, Ištevilo2,... je 1 do 255 kompleksnih števil za množenje."
			},
			{
				name: "ištevilo2",
				description: "Ištevilo1, Ištevilo2,... je 1 do 255 kompleksnih števil za množenje."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Vrne realni koeficient kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega realni koeficient želite"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Vrne sekans kompleksnega števila.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksno število, ki mu želite izračunati sekans"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Vrne hiperbolični sekans kompleksnega števila.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksno število, ki mu želite izračunati hiperbolični sekans"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Vrne sinus kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega sinus želite"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Vrne hiperbolični sinus kompleksnega števila.",
		arguments: [
			{
				name: "število",
				description: "je kompleksno število, ki mu želite izračunati hiperbolični sinus"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Vrne kvadratni koren kompleksnega števila.",
		arguments: [
			{
				name: "ištevilo",
				description: "je kompleksno število, katerega kvadratni koren želite"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Vrne razliko dveh kompleksnih števil.",
		arguments: [
			{
				name: "ištevilo1",
				description: "je kompleksno število, od katerega odštejete »ištevilo2«"
			},
			{
				name: "ištevilo2",
				description: "je kompleksno število, katerega odštejete od »ištevilo1«"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Vrne vsoto kompleksnih števil.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ištevilo1",
				description: "je 1 do 255 kompleksnih števil za seštevanje"
			},
			{
				name: "ištevilo2",
				description: "je 1 do 255 kompleksnih števil za seštevanje"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Vrne tangens kompleksnega števila.",
		arguments: [
			{
				name: "inumber",
				description: "je kompleksno število, ki mu želite izračunati tangens"
			}
		]
	},
	{
		name: "INDEX",
		description: "Vrne vrednost ali sklic na celico v preseku določene vrstice in stolpca v navedenem obsegu.",
		arguments: [
			{
				name: "matrika",
				description: "je obseg celic ali matrična konstanta, ki se je vnesla kot matrika."
			},
			{
				name: "št_vrstice",
				description: "izbere vrstico v matriki ali v sklicu, iz katerega bo vrnjena vrednost. Če to izpustite, morate navesti argument »Št_stolpca«."
			},
			{
				name: "št_stolpca",
				description: "izbere stolpec v matriki ali v sklicu, iz katerega bo vrnjena vrednost. Če to izpustite, morate navesti argument »Št_vrstice«."
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Vrne sklic, ki ga določa besedilni niz.",
		arguments: [
			{
				name: "besedilo_sklica",
				description: "je sklic na celico, ki vsebuje A1- ali R1C1- slog sklicevanja, ime, ki je določeno kot sklic, ali sklic na celico, kot na besedilni niz."
			},
			{
				name: "a1",
				description: "je logična vrednost, ki določa vrsto sklicevanja v argumentu »Besedilo_sklica«: slog R1C1 = FALSE; slog A1 = TRUE ali ni določeno."
			}
		]
	},
	{
		name: "INFO",
		description: "Vrne informacije o trenutnem operacijskem okolju.",
		arguments: [
			{
				name: "besedilo_vrste",
				description: "je besedilo, ki določa, kakšna vrsta informacije naj se vrne."
			}
		]
	},
	{
		name: "INT",
		description: "Število zaokroži navzdol do najbližjega celega števila.",
		arguments: [
			{
				name: "število",
				description: "je realno število, ki ga želite zaokrožiti navzdol v celo število."
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Izračuna presečišče regresijske premice, ki gre skozi podatkovne točke znanih x-ov in znanih y-ov, z osjo y.",
		arguments: [
			{
				name: "znani_y-1",
				description: "je odvisen nabor opazovanj ali podatkov, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "znani_x-i",
				description: "je neodvisen nabor opazovanj ali podatkov, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "INTRATE",
		description: "Vrne obrestno mero za v celoti vloženi vrednostni papir.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "naložba",
				description: "je količina, vložena v vrednostni papir"
			},
			{
				name: "odkup",
				description: "je količina, ki bo prejeta ob zapadlosti"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "IPMT",
		description: "Vrne plačilo obresti za naložbo v navedenem obdobju, ki temelji na periodičnih, enakih plačilih in nespremenljivi obrestni meri.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera na obdobje. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "obdobje",
				description: "je obdobje, v katerem iščete obresti, in mora biti v obsegu od 1 do »št_plačil«."
			},
			{
				name: "št_plačil",
				description: "je skupno število plačilnih obdobij na naložbo."
			},
			{
				name: "sv",
				description: "je sedanja vrednost ali enkratni znesek, ki predstavlja sedanjo vrednost vrste bodočih plačil."
			},
			{
				name: "pv",
				description: "je bodoča vrednost ali blagajniško stanje, ki ga želite doseči po izvedbi zadnjega plačila. Če ni določeno, velja »Pv = 0«."
			},
			{
				name: "vrsta",
				description: "je logična vrednost, ki predstavlja čas plačila: na koncu obdobja = 0 ali ni določeno, na začetku obdobja = 1."
			}
		]
	},
	{
		name: "IRR",
		description: "Vrne notranjo stopnjo donosa za vrsto denarnih tokov.",
		arguments: [
			{
				name: "vrednosti",
				description: "je matrika ali sklic na celice s števili, ki jim želite določiti notranjo stopnjo donosa."
			},
			{
				name: "domneva",
				description: "je število, za katerega ugibate, da je blizu rezultatu IRR; 0.1 (10 odstotkov), če je izpuščeno."
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Preveri, ali gre za sklic na prazno celico, in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je celica ali ime, ki se sklicuje na celico, ki jo želite preskusiti."
			}
		]
	},
	{
		name: "ISERR",
		description: "Preveri, ali je vrednost napaka (#VREDN!, #SKLIC!, #DEL/0!, #ŠTEV!, #IME? ali #NIČ!), z izjemo #N/V, in vrne vrednost TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Preveri, ali je vrednost napaka (#N/V, #VREDN!, #SKLIC!, #DEL/0!, #ŠTEV!, #IME? ali #NIČ!) in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Vrne TRUE, če je število sodo.",
		arguments: [
			{
				name: "število",
				description: "je vrednost za preskus"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Preveri, ali je sklic povezan s celico, ki vsebuje formulo, in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "sklic",
				description: "je sklic na celico, ki ga želite preveriti. Sklic je lahko sklic na celico, formulo ali ime, ki se sklicuje na celico"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Preveri, ali je vrednost logična vrednost (TRUE ali FALSE), in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost."
			}
		]
	},
	{
		name: "ISNA",
		description: "Preveri, ali je vrednost  #N/V, in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost."
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Preveri, ali vrednost ni besedilo (prazne celice niso besedilo), in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti: celica, formula ali ime, s katerim se sklicujete na celico, formulo ali vrednost."
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Preveri, ali je vrednost število, in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost."
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Zaokroži število navzgor na najbližje celo število ali na najbližji mnogokratnik značilnega števila.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki jo želite zaokrožiti."
			},
			{
				name: "pomembnost",
				description: "je izbirni mnogokratnik, na katerega želite zaokrožiti."
			}
		]
	},
	{
		name: "ISODD",
		description: "Vrne TRUE, če je število liho.",
		arguments: [
			{
				name: "število",
				description: "je vrednost za preskus"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Vrne število ISO številke tedna v letu za dani datum.",
		arguments: [
			{
				name: "datum",
				description: "je datumsko-časovna koda, ki jo uporablja Spreadsheet za izračun datuma in ure"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Vrne obresti, plačane v določenem obdobju naložbe.",
		arguments: [
			{
				name: "mera",
				description: "obrestna mera na obdobje. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "obdobje",
				description: "obdobje, za katerega želite poiskati obresti."
			},
			{
				name: "št_obdobij",
				description: "število plačilnih obdobij za naložbo."
			},
			{
				name: "sv",
				description: "enkratno izplačilo v sedanji vrednosti vrste bodočih izplačil."
			}
		]
	},
	{
		name: "ISREF",
		description: "Preveri, ali je vrednost sklic, in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost."
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Preveri, ali je vrednost besedilo, in vrne TRUE ali FALSE.",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preskusiti. Vrednost se lahko sklicuje na celico, formulo ali ime, s katerim se sklicujete na celico, formulo ali vrednost."
			}
		]
	},
	{
		name: "KURT",
		description: "Vrne sploščenost podatkovne množice.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati sploščenost"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati sploščenost"
			}
		]
	},
	{
		name: "LARGE",
		description: "Vrne k-to največjo vrednost nabora podatkov, na primer peto največje število.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov, za katero želite določiti k-to največjo vrednost."
			},
			{
				name: "k",
				description: "je mesto vrednosti za vračanje (od največjega) v matriki ali v obsegu celic."
			}
		]
	},
	{
		name: "LCM",
		description: "Vrne najmanjši skupni mnogokratnik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "je 1 do 255 vrednosti, za katere želite najmanjši skupni mnogokratnik"
			},
			{
				name: "število2",
				description: "je 1 do 255 vrednosti, za katere želite najmanjši skupni mnogokratnik"
			}
		]
	},
	{
		name: "LEFT",
		description: "Vrne določeno število znakov od začetka besedilnega niza.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilni niz, v katerem so znaki, ki jih želite izvleči."
			},
			{
				name: "št_znakov",
				description: "določa, koliko znakov naj izvleče LEFT; enega, če je izpuščeno."
			}
		]
	},
	{
		name: "LEN",
		description: "Vrne število znakov v besedilnem nizu.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo, ki mu želite poiskati dolžino. Presledki se štejejo kot znaki."
			}
		]
	},
	{
		name: "LINEST",
		description: "Vrne statistiko, ki opisuje linearni trend, ujemajoč z znanimi podatkovnimi točkami, s prilagajanjem premici po metodi najmanjših kvadratov.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je nabor y-vrednosti, ki so vam že znane za razmerje y = mx + b."
			},
			{
				name: "znani_x-i",
				description: "je izbiren nabor x-vrednosti, ki jih že poznate za razmerje y = mx + b"
			},
			{
				name: "konstanta",
				description: "je logična vrednost: konstanta b se izračuna normalno, če je konstanta enaka TRUE ali ni določena; b ima vrednost 0, če je konstanta enaka FALSE."
			},
			{
				name: "statistika",
				description: "je logična vrednost: vrne dodatno regresijsko statistiko = TRUE; vrne m-koeficiente in konstanto b = FALSE ali ni določena."
			}
		]
	},
	{
		name: "LN",
		description: "Vrne naravni logaritem števila.",
		arguments: [
			{
				name: "število",
				description: "je pozitivno realno število, ki mu želite določiti naravni logaritem."
			}
		]
	},
	{
		name: "LOG",
		description: "Vrne logaritem števila z osnovo, ki jo določite.",
		arguments: [
			{
				name: "število",
				description: "je pozitivno realno število, ki mu želite določiti logaritem."
			},
			{
				name: "osnova",
				description: "je osnova logaritma; 10, če je izpuščeno."
			}
		]
	},
	{
		name: "LOG10",
		description: "Vrne desetiški logaritem števila.",
		arguments: [
			{
				name: "število",
				description: "je pozitivno realno število, ki mu želite določiti desetiški logaritem."
			}
		]
	},
	{
		name: "LOGEST",
		description: "Vrne statistiko, s katero je opisana eksponentna krivulja, ki ustreza znanim podatkovnim točkam.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je nabor y-vrednosti, ki jih že poznate v odnosu y = b*m^x."
			},
			{
				name: "znani_x-i",
				description: "je izbiren nabor x-vrednosti, ki jih že poznate v odnosu y = b*m^x."
			},
			{
				name: "konstanta",
				description: "je logična vrednost: konstanta b se izračuna normalno, če je konstanta enaka TRUE ali ni določeno; b je enako 1, če je konstanta enaka FALSE."
			},
			{
				name: "statistika",
				description: "je logična vrednost: vrne dodatno regresijsko statistiko = TRUE; vrne m-koeficiente in konstanto b = FALSE ali ni določena."
			}
		]
	},
	{
		name: "LOGINV",
		description: "Vrne inverzno logaritmično normalno kumulativno porazdelitev funkcije x-a, kjer je ln(x) normalno porazdeljen, s parametroma »Srednja_vrednost« in »Standardni_odklon«.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z logaritmično normalno porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "srednja_vrednost",
				description: "je srednja vrednost za ln(x)."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon za ln(x) in je pozitivno število."
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Vrne logaritmično normalno porazdelitev za x, kjer je ln(x) normalno porazdeljen, s parametroma »srednja_vrednost« in »standardni_odklon«.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti funkcijo, in je pozitivno število."
			},
			{
				name: "srednja_vrednost",
				description: "je srednja vrednost za ln(x)."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon za ln(x) in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Vrne inverzno logaritmično normalno kumulativno porazdelitev funkcije x-a, kjer je ln(x) normalno porazdeljen, s parametroma Mean in Standard_dev.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z logaritmično normalno porazdelitvijo, in je število med 0 in vključno 1."
			},
			{
				name: "srednja_vrednost",
				description: "je srednja vrednost za ln(x)."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon za ln(x) in je pozitivno število."
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Vrne kumulativno logaritmično normalno porazdelitev za x, kjer je ln(x) normalno porazdeljen, s parametroma »Srednja_vrednost« in »Standardni_odklon«.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri želite vrednotiti funkcijo, in je pozitivno število."
			},
			{
				name: "srednja_vrednost",
				description: "je srednja vrednost za ln(x)."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon za ln(x) in je pozitivno število."
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Poišče vrednost bodisi iz obsega, ki vsebuje le eno vrstico ali le en stolpec, bodisi iz matrike.Na voljo zaradi združljivosti s prejšnjimi različicami.",
		arguments: [
			{
				name: "iskana_vrednost",
				description: "je vrednost, ki jo LOOKUP išče v parametru »Vektor_iskanja« in je lahko številka, besedilo, logična vrednost, ime ali sklic na vrednost."
			},
			{
				name: "vektor_iskanja",
				description: "je obseg, ki vsebuje le eno vrstico ali le en stolpec besedila, števil ali logičnih vrednosti, razporejenih v naraščajočem vrstnem redu."
			},
			{
				name: "vektor_rezultata",
				description: "je obseg, ki vsebuje le eno vrstico ali le en stolpec enake velikosti kot »Vektor_iskanja«."
			}
		]
	},
	{
		name: "LOWER",
		description: "Pretvori vse črke v besedilnem nizu v male črke.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo, ki ga želite pretvoriti v male črke. Znaki v besedilu, ki niso črke, ne bodo spremenjeni."
			}
		]
	},
	{
		name: "MATCH",
		description: "Vrne relativni položaj elementa v matriki, ki se ujema z navedeno vrednostjo v navedenem vrstnem redu.",
		arguments: [
			{
				name: "iskana_vrednost",
				description: "je vrednost, ki jo uporabite za iskanje vrednosti v matriki, številka, logična vrednost ali sklic na eno od tega."
			},
			{
				name: "matrika_iskanja",
				description: "je neprekinjen obseg celic, v katerem so možne iskane vrednosti, matrika vrednosti ali sklic na matriko."
			},
			{
				name: "vrsta_ujemanja",
				description: "je število 1, 0, ali -1, ki označuje, katera vrednost naj se vrne."
			}
		]
	},
	{
		name: "MAX",
		description: "Vrne največjo vrednost v množici vrednosti. Prezre logične vrednosti in besedilo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, za katera želite poiskati največjo vrednost"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, za katera želite poiskati največjo vrednost"
			}
		]
	},
	{
		name: "MAXA",
		description: "Vrne največjo vrednost v množici vrednosti. Ne prezre logičnih vrednosti ali besedila.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, med katerimi želite poiskati največjo vrednost"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, med katerimi želite poiskati največjo vrednost"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Vrne determinanto matrike.",
		arguments: [
			{
				name: "matrika",
				description: "je številska matrika z enakim številom vrstic in stolpcev, obseg celic ali matrična konstanta."
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Vrne mediano ali število v sredini množice danih števil.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera iščete mediano"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera iščete mediano"
			}
		]
	},
	{
		name: "MID",
		description: "Vrne znake iz sredine besedilnega niza, če sta podana začetni položaj in dolžina.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilni niz, v katerem so znaki, ki jih želite izvleči."
			},
			{
				name: "prvi_znak",
				description: "je položaj prvega znaka, ki ga želite izvleči iz besedila. Prvi znak v besedilu je 1."
			},
			{
				name: "št_znakov",
				description: "določa število znakov, ki jih vrne iz besedila."
			}
		]
	},
	{
		name: "MIN",
		description: "Vrne najmanjšo vrednost v množici vrednosti. Prezre logične vrednosti in besedilo.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, za katera želite poiskati najmanjšo vrednost"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, za katera želite poiskati najmanjšo vrednost"
			}
		]
	},
	{
		name: "MINA",
		description: "Vrne najmanjšo vrednost v množici vrednosti. Ne prezre logičnih vrednosti in besedila.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, med katerimi želite poiskati najmanjšo vrednost"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 števil, prazne celice, logične vrednosti ali besedilna števila, med katerimi želite poiskati najmanjšo vrednost"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Vrne minute, ki so celo število med 0 in 59.",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število v datumsko-časovni kodi, ki jo uporablja Spreadsheet, ali besedilo v časovni obliki, na primer 16:48:00 ali 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Vrne inverzno matriko matrike, shranjene v polju.",
		arguments: [
			{
				name: "matrika",
				description: "je številska matrika z enakim številom vrstic in stolpcev, obseg celic ali matrična konstanta."
			}
		]
	},
	{
		name: "MIRR",
		description: "Vrne notranjo stopnjo donosa za vrsto periodičnih denarnih tokov, upoštevajoč oba: ceno naložbe in obresti na vnovično naložbo denarja.",
		arguments: [
			{
				name: "vrednosti",
				description: "je matrika ali sklic na celice s števili, ki predstavljajo vrsto plačil (negativno) in prihodek (pozitivno) v pravilnih časovnih obdobjih."
			},
			{
				name: "obrestna_mera",
				description: "je obrestna mera, ki jo plačate za denar v denarnih tokovih."
			},
			{
				name: "mera_reinvesticije",
				description: "je obrestna mera, ki jo prejmete na denarna tokova med vnovično naložbo."
			}
		]
	},
	{
		name: "MMULT",
		description: "Vrne produkt dveh matrik, ki je matrika z enakim številom vrstic kot »matrika1« in z enakim številom stolpcev kot »matrika2«.",
		arguments: [
			{
				name: "matrika1",
				description: "je prva matrika števil za množenje in mora imeti enako število stolpcev kot ima »matrika2« vrstic."
			},
			{
				name: "matrika2",
				description: "je prva matrika števil za množenje in mora imeti enako število stolpcev kot ima »matrika2« vrstic."
			}
		]
	},
	{
		name: "MOD",
		description: "Vrne ostanek deljenja.",
		arguments: [
			{
				name: "število",
				description: "je število, ki mu želite poiskati ostanek po deljenju."
			},
			{
				name: "delitelj",
				description: "je število, s katerim želite deliti število."
			}
		]
	},
	{
		name: "MODE",
		description: "Vrne najpogostejšo vrednost v matriki ali v obsegu podatkov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite način"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite način"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Vrne navpično matriko najpogostejših ali ponavljajočih se vrednosti v matriki ali obsegu podatkov. Za vodoravno matriko uporabite =TRANSPOSE(MODE.MULT(število1,število2, ...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katere želite način."
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katere želite način."
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Vrne najpogostejšo vrednost v matriki ali v obsegu podatkov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite način"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite način"
			}
		]
	},
	{
		name: "MONTH",
		description: "Vrne mesec, ki je število od 1 (januar) do 12 (december).",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število v datumsko-časovni kodi, ki jo uporablja Spreadsheet"
			}
		]
	},
	{
		name: "MROUND",
		description: "Vrne število, zaokroženo na želeni večkratnik.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki se zaokroži"
			},
			{
				name: "večkratnik",
				description: "je večkratnik, na katerega želite zaokrožiti število"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Vrne mnogočlenski niz števil.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "je 1 do 255 vrednosti, za katere želite mnogočlenski niz števil"
			},
			{
				name: "število2",
				description: "je 1 do 255 vrednosti, za katere želite mnogočlenski niz števil"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Vrne matriko enote za določeno dimenzijo.",
		arguments: [
			{
				name: "dimenzija",
				description: "je celo število, ki določa dimenzijo matrike enote, ki jo želite dobiti"
			}
		]
	},
	{
		name: "N",
		description: "Pretvori neštevilsko vrednost v število, datume v zaporedna števila, TRUE v 1, kar koli drugega pa v 0 (nič).",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite pretvoriti."
			}
		]
	},
	{
		name: "NA",
		description: "Vrne vrednost napake #N/V (vrednost ni na voljo).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Vrne negativno binomsko porazdelitev, ki je verjetnost, da boste doživeli število_f neuspehov pred uspehom, ki je po vrstnem redu število_s, upoštevajoč, da je konstantna verjetnost uspeha enaka verjetnost_s.",
		arguments: [
			{
				name: "število_f",
				description: "je število neuspehov."
			},
			{
				name: "število_s",
				description: "je prag uspehov."
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspeha in je število med 0 in 1."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Vrne negativno binomsko porazdelitev, ki je verjetnost, da boste doživeli »število_f« neuspehov pred uspehom, ki je po vrstnem redu »število_s«, upoštevajoč, da je konstantna verjetnost uspeha enaka »verjetnost_s«.",
		arguments: [
			{
				name: "število_f",
				description: "je število neuspehov."
			},
			{
				name: "število_s",
				description: "je prag uspehov."
			},
			{
				name: "verjetnost_s",
				description: "je verjetnost uspeha in je število med 0 in 1."
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Vrne število celotnih delovnih dni med dvema datumoma.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum"
			},
			{
				name: "končni_datum",
				description: "je zaporedna številka datuma, ki predstavlja končni datum"
			},
			{
				name: "prazniki",
				description: "je izbirni niz ene ali več zaporednih številk datuma, ki bodo izključene iz delovnega koledarja, kot so državni in zvezni prazniki ter porodniški dopusti"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Vrne število celotnih delovnih dni med dvema datumoma s parametri vikendov po meri.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum."
			},
			{
				name: "končni_datum",
				description: "je zaporedna številka datuma, ki predstavlja končni datum."
			},
			{
				name: "vikend",
				description: "je število ali nabor, ki določa, kdaj nastopijo vikendi."
			},
			{
				name: "prazniki",
				description: "je izbirni nabor ene ali več zaporednih številk datuma, ki bodo izključene iz delovnega koledarja, kot so državni in zvezni prazniki ter porodniški dopusti."
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Vrne nominalno letno obrestno mero.",
		arguments: [
			{
				name: "efektivna_obr_mera",
				description: "je efektivna obrestna mera"
			},
			{
				name: "št_obdobij_leto",
				description: "je število združenih obdobij na leto"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Vrne normalno porazdelitev za navedeno srednjo vrednost in standardni odklon.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, za katero iščete porazdelitev."
			},
			{
				name: "srednja_vrednost",
				description: "je aritmetična srednja vrednost porazdelitve."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon porazdelitve in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za porazdelitev gostote verjetnosti pa uporabite FALSE."
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Vrne inverzno normalno kumulativno porazdelitev za navedeno srednjo vrednost in standardni odklon.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, ki ustreza normalni porazdelitvi, in je število med 0 in vključno 1."
			},
			{
				name: "srednja_vrednost",
				description: "je aritmetična srednja vrednost porazdelitve."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon porazdelitve in je pozitivno število."
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Vrne standardno normalno porazdelitev (ima srednjo vrednost nič in standardni odklon ena).",
		arguments: [
			{
				name: "z",
				description: "je vrednost, za katero iščete porazdelitev."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost, ki določi, kaj naj funkcija vrne: kumulativno porazdelitveno funkcijo = TRUE; porazdelitev gostote verjetnosti = FALSE."
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Vrne inverzno standardno normalno kumulativno porazdelitev (ima srednjo vrednost nič in standardni odklon ena).",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, ki ustreza normalni porazdelitvi, in je število med 0 in vključno 1."
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Vrne normalno kumulativno porazdelitev za navedeno srednjo vrednost in standardni odklon.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, za katero iščete porazdelitev."
			},
			{
				name: "srednja_vrednost",
				description: "je aritmetična srednja vrednost porazdelitve."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon porazdelitve in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa uporabite FALSE."
			}
		]
	},
	{
		name: "NORMINV",
		description: "Vrne inverzno normalno kumulativno porazdelitev za navedeno srednjo vrednost in standardni odklon.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, ki ustreza normalni porazdelitvi, in je število med 0 in vključno 1."
			},
			{
				name: "srednja_vrednost",
				description: "je aritmetična srednja vrednost porazdelitve."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon porazdelitve in je pozitivno število."
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Vrne standardno normalno kumulativno porazdelitev (ima srednjo vrednost nič in standardni odklon ena).",
		arguments: [
			{
				name: "z",
				description: "je vrednost, za katero iščete porazdelitev."
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Vrne inverzno standardno normalno kumulativno porazdelitev (ima srednjo vrednost nič in standardni odklon ena).",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, ki ustreza normalni porazdelitvi, in je število med 0 in vključno 1."
			}
		]
	},
	{
		name: "NOT",
		description: "Spremeni FALSE v TRUE ali TRUE v FALSE.",
		arguments: [
			{
				name: "logično",
				description: "je vrednost ali izraz, ki se vrednoti kot TRUE ali FALSE."
			}
		]
	},
	{
		name: "NOW",
		description: "Vrne trenutni datum in uro, oblikovano kot datum in ura.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Vrne število obdobij za naložbo, ki temelji na periodičnih, enakih plačilih in nespremenljivi obrestni meri.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera na obdobje. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "plačilo",
				description: "je plačilo nakazano v vsakem obdobju; ne sme se spreminjati prek življenja naložbe."
			},
			{
				name: "sv",
				description: "je sedanja vrednost ali enkratni znesek, ki predstavlja sedanjo vrednost vrste bodočih plačil."
			},
			{
				name: "pv",
				description: "je bodoča vrednost ali blagajniško stanje, ki ga želite doseči po izvedbi zadnjega plačila. Če izpustite, je uporabljena vrednost nič."
			},
			{
				name: "vrsta",
				description: "je logična vrednost: plačilo v začetku obdobja = 1; plačilo na koncu obdobja = 0 ali izpuščeno."
			}
		]
	},
	{
		name: "NPV",
		description: "Vrne sedanjo neto vrednost naložbe, ki temelji na diskontni stopnji in na vrsti bodočih plačil (negativne vrednosti) in prihodku (pozitivne vrednosti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "stopnja",
				description: "je diskontna stopnja v enem obdobju."
			},
			{
				name: "vrednost1",
				description: "od 1 do 254 enakomerno porazdeljenih plačil in prihodkov, ki se pojavljajo na koncu posameznega obdobja"
			},
			{
				name: "vrednost2",
				description: "od 1 do 254 enakomerno porazdeljenih plačil in prihodkov, ki se pojavljajo na koncu posameznega obdobja"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Pretvori besedilo v številko, neodvisno od lokalnega načina.",
		arguments: [
			{
				name: "besedilo",
				description: "je niz, ki predstavlja število, ki ga želite pretvoriti"
			},
			{
				name: "decimalno_ločilo",
				description: "je znak, ki je uporabljen kot ločilo v nizu"
			},
			{
				name: "skupina_ločilo",
				description: "je znak, ki je uporabljen kot skupinsko ločilo v nizu"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Pretvori osmiško število v dvojiško.",
		arguments: [
			{
				name: "število",
				description: "je osmiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Pretvori osmiško število v desetiško.",
		arguments: [
			{
				name: "število",
				description: "je osmiško število, ki ga želite pretvoriti"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Pretvori osmiško število v šestnajstiško.",
		arguments: [
			{
				name: "število",
				description: "je osmiško število, ki ga želite pretvoriti"
			},
			{
				name: "mesta",
				description: "je število znakov za uporabo"
			}
		]
	},
	{
		name: "ODD",
		description: "Zaokroži pozitivno število navzgor in negativno število navzdol do najbližjega lihega celega števila.",
		arguments: [
			{
				name: "število",
				description: "je vrednost, ki se zaokroži."
			}
		]
	},
	{
		name: "OFFSET",
		description: "Vrne sklic na obseg, ki je dano število vrstic in stolpcev iz danega sklica.",
		arguments: [
			{
				name: "sklic",
				description: "je sklic, na katerem temelji odmik, in je sklic na celico ali na obseg priležnih celic."
			},
			{
				name: "vrstice",
				description: "je število vrstic, navzgor ali navzdol, na katere naj se sklicuje zgornja leva celica rezultata."
			},
			{
				name: "stolpci",
				description: "je število stolpcev, levo ali desno, na katere naj se sklicuje zgornja leva celica rezultata."
			},
			{
				name: "višina",
				description: "je višina v številu vrstic, ki jo želite za rezultat. Če to opustite, bo višina enaka, kot je določeno v argumentu Sklic«."
			},
			{
				name: "širina",
				description: "je širina v številu stolpcev, ki jo želite za rezultat. Če to opustite, bo širina enaka, kot je določeno v argumentu Sklic«."
			}
		]
	},
	{
		name: "OR",
		description: "Preveri, ali ima kateri argument vrednost TRUE; vrne vrednost TRUE ali FALSE. Vrne vrednost FALSE, če imajo vsi argumenti vrednost FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logično1",
				description: "od 1 do 255 pogojev, ki jih želite preskusiti, in so lahko ali TRUE ali FALSE"
			},
			{
				name: "logično2",
				description: "od 1 do 255 pogojev, ki jih želite preskusiti, in so lahko ali TRUE ali FALSE"
			}
		]
	},
	{
		name: "PARAMETER",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "PDURATION",
		description: "Vrne število obdobij, ki jih investicija zahteva, če želi doseči določeno vrednost.",
		arguments: [
			{
				name: "rate",
				description: "je obrestna mera na obdobje."
			},
			{
				name: "pv",
				description: "je sedanja vrednost investicije"
			},
			{
				name: "fv",
				description: "je želena vrednost investicije v prihodnosti"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Vrne Pearsonov korelacijski koeficient r.",
		arguments: [
			{
				name: "matrika1",
				description: "je nabor neodvisnih vrednosti."
			},
			{
				name: "matrika2",
				description: "je nabor odvisnih vrednosti."
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Vrne k-ti percentil vrednosti v obsegu.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov, ki določa relativni položaj."
			},
			{
				name: "k",
				description: "je vrednost percentila, ki je med 0 in vključno 1."
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Vrne k-ti percentil vrednosti v obsegu, kjer je k v obsegu med 0 in izključno 1.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov, ki določa relativni položaj."
			},
			{
				name: "k",
				description: "je vrednost percentila, ki je med 0 in vključno 1."
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Vrne k-ti percentil vrednosti v obsegu, kjer je k v obsegu med 0 in vključno 1.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov, ki določa relativni položaj."
			},
			{
				name: "k",
				description: "je vrednost percentila, ki je med 0 in vključno 1."
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Vrne rang vrednosti v množici podatkov kot odstotek podatkovne množice.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov s številskimi vrednostmi, ki določa relativni položaj."
			},
			{
				name: "x",
				description: "je vrednost, za katero želite poznati rang."
			},
			{
				name: "pomembnost",
				description: "je izbirna vrednost, ki označuje število pomembnih števk za vrnjeno odstotno vrednost. Če ta argument izpustite, so uporabljene tri števke (0.xxx%)."
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Vrne rang vrednosti v množici podatkov kot odstotek podatkovne množice (od 0 do izključno 1).",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov s številskimi vrednostmi, ki določa relativni položaj."
			},
			{
				name: "x",
				description: "je vrednost, za katero želite poznati rang."
			},
			{
				name: "spomembnost",
				description: "je izbirna vrednost, ki označuje število pomembnih števk za vrnjeno odstotno vrednost. Če ta argument izpustite, so uporabljene tri števke (0,xxx%)."
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Vrne rang vrednosti v množici podatkov kot odstotek podatkovne množice (od 0 do vključno 1).",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov s številskimi vrednostmi, ki določa relativni položaj."
			},
			{
				name: "x",
				description: "je vrednost, za katero želite poznati rang."
			},
			{
				name: "pomembnost",
				description: "je izbirna vrednost, ki označuje število pomembnih števk za vrnjeno odstotno vrednost. Če ta argument izpustite, so uporabljene tri števke (0,xxx%)."
			}
		]
	},
	{
		name: "PERMUT",
		description: "Vrne število permutacij za dano število predmetov ki so lahko izbrani izmed vseh predmetov.",
		arguments: [
			{
				name: "število",
				description: "je število vseh predmetov."
			},
			{
				name: "število_izbranih",
				description: "je število predmetov v vsaki permutaciji."
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Vrne število permutacij za dano število predmetov (s ponovitvami), ki jih je mogoče izbrati med skupnim številom predmetov.",
		arguments: [
			{
				name: "število",
				description: "je skupno število predmetov"
			},
			{
				name: "število_izbrano",
				description: "je število predmetov v vsaki permutaciji"
			}
		]
	},
	{
		name: "PHI",
		description: "Vrne vrednost porazdelitve gostote za standardno normalno porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je število, ki mu želite izračunati porazdelitev gostote za standardno normalno porazdelitev"
			}
		]
	},
	{
		name: "PI",
		description: "Vrne vrednost Pi na 15 decimalnih mest točno (3,14159265358979).",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Izračuna plačilo za posojilo, ki temelji na enakih plačilih in nespremenljivi obrestni meri.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera za posojilo na obdobje."
			},
			{
				name: "obdobja",
				description: "je skupno število plačilnih obdobij na posojilo. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "sv",
				description: "je sedanja vrednost: skupen znesek, enak sedanji vrednosti vrste bodočih plačil."
			},
			{
				name: "pv",
				description: "je bodoča vrednost ali blagajniško stanje, ki ga želite doseči po izvedbi zadnjega plačila, nič, če je izpuščeno."
			},
			{
				name: "vrsta",
				description: "je logična vrednost: plačilo v začetku obdobja = 1; plačilo na koncu obdobja = 0 ali izpuščeno."
			}
		]
	},
	{
		name: "POISSON",
		description: "Vrne Poissonovo porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je število dogodkov."
			},
			{
				name: "srednja_vrednost",
				description: "je pričakovana številska vrednost in je pozitivno število."
			},
			{
				name: "kunulativno",
				description: "je logična vrednost: za kumulativno Poissonovo verjetnost uporabite TRUE, za Poissonovo verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Vrne Poissonovo porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je število dogodkov."
			},
			{
				name: "srednja_vrednost",
				description: "je pričakovana številska vrednost in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno Poissonovo verjetnost uporabite TRUE, za Poissonovo verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "POWER",
		description: "Vrne potenco števila.",
		arguments: [
			{
				name: "število",
				description: "je osnova potence in je lahko katero koli realno število."
			},
			{
				name: "potenca",
				description: "je eksponent potence."
			}
		]
	},
	{
		name: "PPMT",
		description: "Vrne plačilo na glavnico za naložbo, ki temelji na enakih plačilih in nespremenljivi obrestni meri.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera na obdobje. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "obdobja",
				description: "določa obdobje in mora biti v obsegu od 1 do »št_plačil«."
			},
			{
				name: "št_plačil",
				description: "je skupno število plačilnih obdobij na naložbo."
			},
			{
				name: "sv",
				description: "je sedanja vrednost: skupen znesek, enak sedanji vrednosti vrste bodočih plačil."
			},
			{
				name: "pv",
				description: "je bodoča vrednost ali blagajniško stanje, ki ga želite doseči po izvedbi zadnjega plačila."
			},
			{
				name: "vrsta",
				description: "je logična vrednost: plačilo v začetku obdobja = 1; plačilo na koncu obdobja = 0 ali izpuščeno."
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Vrne ceno vrednostnega papirja z rabatom na 100 € imenske vrednosti.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "rabata",
				description: "je stopnja rabata vrednostnega papirja"
			},
			{
				name: "odkup",
				description: "je amortizacijska vrednost vrednostnega papirja na 100 € imenske vrednosti"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "PROB",
		description: "Vrne verjetnost, da so vrednosti obsega med obema mejama ali enake spodnji meji.",
		arguments: [
			{
				name: "x_obseg",
				description: "je obseg številskih vrednosti za x, ki so pridružene verjetnosti."
			},
			{
				name: "verjet_obseg",
				description: "je nabor verjetnosti, povezanih z vrednostmi v »X_obseg«, in so vrednosti med 0 in 1 in niso 0."
			},
			{
				name: "spodnja_meja",
				description: "je spodnja meja za vrednost, za katero določate verjetnost."
			},
			{
				name: "zgornja_meja",
				description: "je izbirna zgornja meja vrednosti. Če jo izpustite, PROB vrne verjetnost, da so vrednosti v »X_obseg« enake »spodnja_meja«."
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Množi števila, ki so bila podana kot argumenti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, logičnih vrednosti ali besedilnih predstavitev števil, ki jih želite zmnožiti"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, logičnih vrednosti ali besedilnih predstavitev števil, ki jih želite zmnožiti"
			}
		]
	},
	{
		name: "PROPER",
		description: "Pretvori besedilni niz v velike in male črke; vsako prvo črko v besedi v veliko začetnico, vse preostale črke pa pretvori v male.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo v narekovajih, formula, ki vrne besedilo ali sklic na celico z besedilom, kjer želite delno postaviti velike začetnice. "
			}
		]
	},
	{
		name: "PV",
		description: "Vrne sedanjo vrednost naložbe: celotna vsota vrednosti vrste bodočih plačil v tem trenutku.",
		arguments: [
			{
				name: "mera",
				description: "je obrestna mera na obdobje. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4."
			},
			{
				name: "obdobja",
				description: "je skupno število plačilnih obdobij na naložbo."
			},
			{
				name: "plačilo",
				description: "je plačilo, izvedeno v vsakem obdobju, in se ne more spremeniti prek življenja naložbe."
			},
			{
				name: "sv",
				description: "je bodoča vrednost ali blagajniško stanje, ki ga želite doseči po izvedbi zadnjega plačila."
			},
			{
				name: "vrsta",
				description: "je logična vrednost: plačilo v začetku obdobja = 1; plačilo na koncu obdobja = 0 ali izpuščeno."
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Vrne kvartil nabora podatkov.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg celic s številskimi vrednostmi, za katere želite določiti kvartilne vrednosti."
			},
			{
				name: "kvartil",
				description: "je število: minimalna vrednost = 0; prvi kvartil = 1; mediana = 2; tretji kvartil = 3; maksimalna vrednost = 4."
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Vrne kvartil nabora podatkov na osnovi vrednosti percentila med 0 in izključno 1.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg celic s številskimi vrednostmi, za katere želite določiti kvartilne vrednosti."
			},
			{
				name: "kvartil",
				description: "je število: minimalna vrednost = 0; prvi kvartil = 1; mediana = 2; tretji kvartil = 3; maksimalna vrednost = 4."
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Vrne kvartil nabora podatkov na podlagi vrednosti percentila med 0 in vključno 1.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg celic s številskimi vrednostmi, za katere želite določiti kvartilne vrednosti."
			},
			{
				name: "kvartil",
				description: "je število: minimalna vrednost = 0; prvi kvartil = 1; mediana = 2; tretji kvartil = 3; maksimalna vrednost = 4."
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Vrne del celega števila deljenja.",
		arguments: [
			{
				name: "deljenec",
				description: "je deljenec"
			},
			{
				name: "delitelj",
				description: "je delitelj"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Pretvori stopinje v radiane.",
		arguments: [
			{
				name: "kot",
				description: "je kot v stopinjah, ki ga želite pretvoriti."
			}
		]
	},
	{
		name: "RAND",
		description: "Vrne naključno število, ki je večje ali enako 0 in manjše od 1, enakomerno porazdeljeno (spremembe vnovičnega izračuna).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Vrne naključno število med navedenimi števili.",
		arguments: [
			{
				name: "najmnajše",
				description: "je najmanjše celo število, ki ga vrne funkcija RANDBETWEEN"
			},
			{
				name: "največje",
				description: "je največje celo število, ki ga vrne funkcija RANDBETWEEN"
			}
		]
	},
	{
		name: "RANGE",
		description: "",
		arguments: [
			{
				name: "",
				description: ""
			}
		]
	},
	{
		name: "RANK",
		description: "Vrne rang števila na seznamu števil, ki je relativna velikost števila glede na druge vrednosti na seznamu.",
		arguments: [
			{
				name: "število",
				description: "je število, ki mu želite poiskati rang."
			},
			{
				name: "sklic",
				description: "je matrika števil ali sklic na seznamu števil. Neštevilske vrednosti so prezrte."
			},
			{
				name: "vrstni_red",
				description: "je število, ki določa, kako bo urejen rang na seznamu: če je 0 ali izpuščeno, bo rang urejen padajoče; če je katera koli neničelna vrednost, bo rang urejen naraščajoče"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Vrne rang števila na seznamu števil, ki je relativna velikost števila glede na druge vrednosti na seznamu. Če ima več vrednosti enak rang, vrne povprečni rang.",
		arguments: [
			{
				name: "število",
				description: "je število, ki mu želite poiskati rang."
			},
			{
				name: "sklic",
				description: "je matrika števil ali sklic na seznam števil. Neštevilske vrednosti se ne upoštevajo."
			},
			{
				name: "vrstni_red",
				description: "je število: če je 0 ali izpuščeno, bo rang urejen padajoče; če je katera koli neničelna vrednost, bo rang urejen naraščajoče."
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Vrne rang števila na seznamu števil, ki je relativna velikost števila glede na druge vrednosti na seznamu. Če ima več vrednosti enak rang, vrne najvišji rang tiste množice vrednosti.",
		arguments: [
			{
				name: "število",
				description: "je število, ki mu želite poiskati rang."
			},
			{
				name: "sklic",
				description: "je matrika števil ali sklic na seznam števil. Neštevilske vrednosti se ne upoštevajo."
			},
			{
				name: "vrstni_red",
				description: "je število: če je 0 ali izpuščeno, bo rang urejen padajoče; če je katera koli neničelna vrednost, bo rang urejen naraščajoče."
			}
		]
	},
	{
		name: "RATE",
		description: "Vrne obrestno mero na obdobje posojila ali naložbe. Za četrtletna plačila pri 6 % APR na primer uporabite 6 %/4.",
		arguments: [
			{
				name: "obdobja",
				description: "je skupno število plačilnih obdobij na posojilo ali naložbo."
			},
			{
				name: "plačilo",
				description: "je plačilo, izvedeno v vsakem obdobju, in se ne more spremeniti prek življenja posojila ali naložbe."
			},
			{
				name: "sv",
				description: "je sedanja vrednost: skupen znesek enak sedanji vrednosti vrste bodočih plačil."
			},
			{
				name: "pv",
				description: "je bodoča vrednost ali blagajniško stanje, ki ga želite doseči po izvedbi zadnjega plačila. Če je izpuščeno, se uporabi Pv = 0."
			},
			{
				name: "vrsta",
				description: "je logična vrednost: plačilo v začetku obdobja = 1; plačilo na koncu obdobja = 0 ali izpuščeno."
			},
			{
				name: "domneva",
				description: "je vaša domneva, kakšna bo mera. Če je izpuščeno, je vaša domneva (Domneva) enaka 0,1 (10 odstotkov)."
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Vrne količino, ki je prejeta ob zapadlosti za v celoti vloženi vrednostni papir.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "naložba",
				description: "je količina, vložena v vrednostni papir"
			},
			{
				name: "rabat",
				description: "je stopnja rabata vrednostnega papirja"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Zamenja del besedilnega niza z drugim besedilnim nizom.",
		arguments: [
			{
				name: "staro_besedilo",
				description: "je besedilo, v katerem želite zamenjati nekatere znake."
			},
			{
				name: "mesto_znaka",
				description: "je mesto znaka v starem besedilu, ki ga želite zamenjati z novim besedilom."
			},
			{
				name: "št_znakov",
				description: "je število znakov v »staro_besedilo«, ki ga želite zamenjati z »novo_besedilo«."
			},
			{
				name: "novo_besedilo",
				description: "je besedilo, ki bo zamenjalo znake v starem besedilu."
			}
		]
	},
	{
		name: "REPT",
		description: "Ponovi besedilo tolikokrat, kolikor krat je navedeno. Uporabite REPT, če želite zapolniti celico z več ponovitvami besedilnega niza.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo, ki ga želite ponavljati."
			},
			{
				name: "št_ponovitev",
				description: "je pozitivno število, ki določa, kolikokrat se besedilo ponovi."
			}
		]
	},
	{
		name: "RIGHT",
		description: "Vrne določeno število znakov od konca besedilnega niza.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilni niz, v katerem so znaki, ki jih želite izvleči."
			},
			{
				name: "št_znakov",
				description: "določa, koliko znakov želite izvleči. Enega, če je izpuščeno."
			}
		]
	},
	{
		name: "ROMAN",
		description: "Pretvori arabsko številko v rimsko v obliki besedila.",
		arguments: [
			{
				name: "število",
				description: "je arabska številka, ki jo želite pretvoriti."
			},
			{
				name: "oblika",
				description: "je številka, ki določa vrsto rimske številke."
			}
		]
	},
	{
		name: "ROUND",
		description: "Zaokroži število prek določenega števila števk.",
		arguments: [
			{
				name: "število",
				description: "je število, ki ga želite zaokrožiti."
			},
			{
				name: "št_števk",
				description: "določa, na koliko decimalnih mest želite zaokrožiti število. Negativno število pomeni zaokrožanje na levo od decimalne vejice, nič pa pomeni zaokrožanje na najbližje celo število."
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Zaokroži število navzdol proti nič.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število, ki ga želite zaokrožiti navzdol."
			},
			{
				name: "št_števk",
				description: "določa, na koliko decimalnih mest želite zaokrožiti število. Negativno število pomeni zaokrožanje na levo od decimalne vejice, nič ali izpuščeno pa pomeni zaokrožanje na najbližje celo število."
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Zaokroži število navzgor, stran od nič.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število, ki ga želite zaokrožiti."
			},
			{
				name: "št_števk",
				description: "določa, na koliko decimalnih mest želite zaokrožiti število. Negativno število pomeni zaokrožanje na levo od decimalne vejice, nič ali izpuščeno pa pomeni zaokrožanje na najbližje celo število."
			}
		]
	},
	{
		name: "ROW",
		description: "Vrne številko vrstice za sklic.",
		arguments: [
			{
				name: "sklic",
				description: "je celica ali obseg celic, za katere želite število vrstice; če je izpuščeno, vrne celico, ki vsebuje funkcijo ROW."
			}
		]
	},
	{
		name: "ROWS",
		description: "Vrne število vrstic v sklicu ali v matriki.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika, matrična formula ali sklic na obseg celic, za katere želimo poznati število vrstic."
			}
		]
	},
	{
		name: "RRI",
		description: "Vrne ekvivalentno obrestno mero za rast investicije.",
		arguments: [
			{
				name: "nper",
				description: "je število obdobij investicije"
			},
			{
				name: "pv",
				description: "je trenutna vrednost investicije"
			},
			{
				name: "fv",
				description: "je prihodnja vrednost investicije"
			}
		]
	},
	{
		name: "RSQ",
		description: "Vrne kvadrat Pearsonovega korelacijskega koeficienta.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je matrika ali obseg podatkovnih točk, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "znani_x-i",
				description: "je matrika ali obseg podatkovnih točk, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "RTD",
		description: "Pridobi sprotne podatke iz programa, ki podpira avtomatizacijo COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ID_programa",
				description: "je ime »ID_programa« za registrirani dodatek COM za avtomatizacijo. Ime zapišite v narekovajih."
			},
			{
				name: "strežnik",
				description: "je ime strežnika, v katerem naj se bi dodatek izvajal. Navedite ime v narekovajih. Če se dodatek izvaja lokalno, uporabite prazen niz."
			},
			{
				name: "tema1",
				description: "je od 1 do 38 parametrov, ki navajajo del podatkov."
			},
			{
				name: "tema2",
				description: "je od 1 do 38 parametrov, ki navajajo del podatkov."
			}
		]
	},
	{
		name: "SEARCH",
		description: "Vrne številko znaka, kjer je prvič – gledano z leve proti desni - najden poseben znak ali besedilni niz (ne loči velikih in malih črk).",
		arguments: [
			{
				name: "iskano_besedilo",
				description: "je besedilo, ki ga želite poiskati. Uporabite lahko nadomestna znaka »?« in »*«. Če želite iskati znaka »?« in »*«, uporabite »~?« in »~*«."
			},
			{
				name: "v_besedilu",
				description: "je besedilo, v katerem želite najti »Iskano_besedilo«."
			},
			{
				name: "št_začetka",
				description: "je številka znaka v argumentu »V_besedilu«, šteto od leve, pri katerem želite začeti iskanje. Če je izpuščeno, se uporabi 1."
			}
		]
	},
	{
		name: "SEC",
		description: "Vrne sekans kota.",
		arguments: [
			{
				name: "number",
				description: "je kot v radianih, ki mu želite izračunati sekans"
			}
		]
	},
	{
		name: "SECH",
		description: "Vrne hiperbolični sekans kota.",
		arguments: [
			{
				name: "number",
				description: "je kot v radianih, ki mu želite izračunati hiperbolični sekans"
			}
		]
	},
	{
		name: "SECOND",
		description: "Vrne sekunde, ki so celo število med 0 in 59.",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število v datumsko-časovni kodi, ki jo uporablja Spreadsheet, ali besedilo v časovni obliki, na primer 16:48:23 ali 4:48:47 PM"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Vrne vsoto potenciranih nizov glede na formulo.",
		arguments: [
			{
				name: "x",
				description: "je vnosna vrednost za potencirane nize"
			},
			{
				name: "n",
				description: "je začetna potenca, na katero želite potencirati x"
			},
			{
				name: "m",
				description: "je korak, po katerem povečate n za vsak člen v nizu"
			},
			{
				name: "koeficienti",
				description: "je niz koeficientov, s katerimi je vsaka naslednja potenca x-a pomnožena"
			}
		]
	},
	{
		name: "SHEET",
		description: "Vrne število listov sklicevanega lista.",
		arguments: [
			{
				name: "vrednost",
				description: "je ime lista ali sklic lista. Če je izpuščen, je vrnjena številka lista, ki vsebuje funkcijo"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Vrne število listov v sklicu.",
		arguments: [
			{
				name: "sklic",
				description: "je sklic, za katerega želite vedeti, koliko listov vsebuje. Če je izpuščen, je vrnjeno število listov v delovnem zvezku, ki vsebujejo funkcijo"
			}
		]
	},
	{
		name: "SIGN",
		description: "Vrne predznak števila: 1, če je število pozitivno, nič, če je število nič, ali -1, če je število negativno.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število."
			}
		]
	},
	{
		name: "SIN",
		description: "Vrne sinus kota.",
		arguments: [
			{
				name: "pštevilo",
				description: "je kot v radianih, za katerega želite poiskati sinus. Stopinje*PI()/180 = radiani."
			}
		]
	},
	{
		name: "SINH",
		description: "Vrne hiperbolični sinus števila.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število."
			}
		]
	},
	{
		name: "SKEW",
		description: "Vrne asimetrijo porazdelitve, ki je označitev stopnje asimetrije porazdelitve okoli njene srednje vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati asimetrijo"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati asimetrijo"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Vrne asimetrijo porazdelitve glede na populacijo: označitev stopnje asimetrije porazdelitve okoli njene srednje vrednosti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati asimetrijo"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil ali imen, matrik ali sklicev, ki vsebujejo števila, za katera želite izračunati asimetrijo"
			}
		]
	},
	{
		name: "SLN",
		description: "Vrne linearno amortizacijo sredstva za eno obdobje.",
		arguments: [
			{
				name: "stroški",
				description: "je začetna cena sredstva."
			},
			{
				name: "vrednost_po_amor",
				description: "je rešena vrednost na koncu življenjske dobe sredstva."
			},
			{
				name: "št_obdobij",
				description: "je število obdobij, prek katerih se amortizira sredstvo (imenovano tudi življenjska doba sredstva)."
			}
		]
	},
	{
		name: "SLOPE",
		description: "Vrne naklon regresijske premice skozi dane podatkovne točke.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je matrika ali obseg celic številsko odvisnih podatkovnih točk, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "znani_x-i",
				description: "je množica neodvisnih podatkovnih točk, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "SMALL",
		description: "Vrne k-to najmanjšo vrednost nabora podatkov, na primer peto najmanjše število.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg številskih podatkov, za katere želite določiti k-to najmanjšo vrednost."
			},
			{
				name: "k",
				description: "je mesto vrednosti za vračanje (od najmanjšega) v matriki ali v obsegu celic."
			}
		]
	},
	{
		name: "SQRT",
		description: "Vrne pozitivni kvadratni koren števila.",
		arguments: [
			{
				name: "število",
				description: "je število, ki mu želite določiti kvadratni koren."
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Vrne kvadratni koren za (število * pi).",
		arguments: [
			{
				name: "število",
				description: "je število, s katerim je p pomnožen"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Vrne normalizirano vrednost iz porazdelitve, ki je označena s srednjo vrednostjo in standardnim odklonom.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, ki jo želite normalizirati."
			},
			{
				name: "srednja_vrednost",
				description: "je aritmetična srednja vrednost porazdelitve."
			},
			{
				name: "standardni_odklon",
				description: "je standardni odklon porazdelitve in je pozitivno število."
			}
		]
	},
	{
		name: "STDEV",
		description: "Oceni standardni odklon glede na vzorec (v vzorcu prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, ki ustrezajo vzorcu populacije in so lahko števila ali sklici s številkami"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, ki ustrezajo vzorcu populacije in so lahko števila ali sklici s številkami"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Izračuna standardni odklon na osnovi celotne populacije, podane v obliki argumentov (prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, ki ustrezajo populaciji in so lahko števila ali sklici, ki vsebujejo števila"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, ki ustrezajo populaciji in so lahko števila ali sklici, ki vsebujejo števila"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Oceni standardni odklon vzorca na osnovi vzorca (v vzorcu prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 argumentov, ki ustrezajo vzorcu populacije in so lahko števila ali sklici, ki vsebujejo števila"
			},
			{
				name: "število2",
				description: "od 1 do 255 argumentov, ki ustrezajo vzorcu populacije in so lahko števila ali sklici, ki vsebujejo števila"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Ugotovi standardni odklon, ki temelji na vzorcu, ki vsebuje logične vrednosti in besedilo. Besedilo in logična vrednost FALSE imata vrednost 0, logična vrednost TRUE pa ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 vrednosti, ki ustrezajo vzorcu iz populacije in so lahko vrednosti, imena ali sklici na vrednosti"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 vrednosti, ki ustrezajo vzorcu iz populacije in so lahko vrednosti, imena ali sklici na vrednosti"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Izračuna standardni odklon na podlagi celotne populacije, navedene v obliki argumentov (prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, ki ustrezajo populaciji in so lahko števila ali sklici s števili"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, ki ustrezajo populaciji in so lahko števila ali sklici s števili"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Izračuna standardni odklon, ki temelji na celotni populaciji, vključno z logičnimi vrednostmi in besedilom. Besedilo in logična vrednost FALSE imata vrednost 0, logična vrednost TRUE pa ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 vrednosti, ki ustrezajo populaciji in so lahko vrednosti, imena, matrike ali sklici, ki vsebujejo vrednosti"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 vrednosti, ki ustrezajo populaciji in so lahko vrednosti, imena, matrike ali sklici, ki vsebujejo vrednosti"
			}
		]
	},
	{
		name: "STEYX",
		description: "Vrne standardno napako predvidenih y-vrednosti za vsak x v regresiji.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je matrika ali obseg odvisnih podatkovnih točk, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			},
			{
				name: "znani_x-i",
				description: "je matrika ali obseg neodvisnih podatkovnih točk, ki so lahko števila ali imena, matrike ali sklici, ki vsebujejo števila."
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Zamenja staro besedilo z novim v besedilnem nizu.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo ali sklic na celico z besedilom, v katerem želite zamenjevati znake."
			},
			{
				name: "staro_besedilo",
				description: "je obstoječe besedilo, ki ga želite zamenjati. Če se velike in male črke v »Staro_besedilo« ne ujemajo z velikimi in malimi črkami v besedilu, funkcija SUBSTITUTE ne bo zamenjala besedila."
			},
			{
				name: "novo_besedilo",
				description: "je besedilo, s katerim želite zamenjati »Staro_besedilo«."
			},
			{
				name: "št_primerka",
				description: "določa, katero pojavitev »Staro_besedilo« želite zamenjati. Če je izpuščeno, bodo zamenjane vse pojavitve »Staro_besedilo«."
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Vrne delno vsoto s seznama ali iz zbirke podatkov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "št_funkcije",
				description: "je število med 1 do 11, ki navaja, katera funkcija se uporabi za izračun delnih vsot na seznamu."
			},
			{
				name: "sklic1",
				description: "od 1 do 254 obsegov ali sklicev, katerim želite določiti delne vsote"
			}
		]
	},
	{
		name: "SUM",
		description: "Sešteje vsa števila v obsegu celic.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, ki jih želite sešteti. Logične vrednosti in besedilo v celicah se prezrejo, tudi če jih vnesete kot argumente"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, ki jih želite sešteti. Logične vrednosti in besedilo v celicah se prezrejo, tudi če jih vnesete kot argumente"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Sešteje celice, ki jih določa podan pogoj ali kriterij.",
		arguments: [
			{
				name: "obseg",
				description: "je obseg celic, ki ga želite vrednotiti."
			},
			{
				name: "pogoji",
				description: "je pogoj ali kriterij v obliki števila, izraza ali besedila, ki določa, katere celice naj se seštevajo."
			},
			{
				name: "obseg_seštevanja",
				description: "so dejanske celice, ki se bodo seštele. Če je izpuščeno, bodo uporabljene celice v obsegu."
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Doda celice, ki jih navaja dani niz pogojev ali kriterijev.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "obseg_seštevanja",
				description: "so dejanske celice za seštevanje."
			},
			{
				name: "obseg_pogojev",
				description: "je obseg celic, ki jih želite ovrednotiti za določen pogoj"
			},
			{
				name: "pogoji",
				description: "je pogoj ali kriterij v obliki števila, izraza ali besedila, ki določa, katere celice bodo dodane"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Vrne vsoto produktov ustreznih obsegov ali matrik.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrika1",
				description: "od 2 do 255 matrik, katerih elemente želite množiti in nato sešteti. Vse matrike morajo imeti enake razsežnosti"
			},
			{
				name: "matrika2",
				description: "od 2 do 255 matrik, katerih elemente želite množiti in nato sešteti. Vse matrike morajo imeti enake razsežnosti"
			},
			{
				name: "matrika3",
				description: "od 2 do 255 matrik, katerih elemente želite množiti in nato sešteti. Vse matrike morajo imeti enake razsežnosti"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Vrne vsoto kvadratov argumentov. Argumenti so lahko števila, matrike, imena ali sklici, ki vsebujejo števila.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 števil, matrik, imen ali sklicev, za katere želite izračunati vsoto kvadratov"
			},
			{
				name: "število2",
				description: "od 1 do 255 števil, matrik, imen ali sklicev, za katere želite izračunati vsoto kvadratov"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Izračuna vsoto razlik kvadratov pripadajočih števil v dveh obsegih ali matrikah.",
		arguments: [
			{
				name: "matrika_x",
				description: "je prvi obseg ali matrika števil in je lahko število ali ime, matrika ali sklic, ki vsebuje števila."
			},
			{
				name: "matrika_y",
				description: "je drugi obseg ali matrika števil in je lahko število ali ime, matrika ali sklic, ki vsebuje števila."
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Izračuna skupno vsoto vseh vsot kvadratov števil v dveh pripadajočih obsegih ali matrikah.",
		arguments: [
			{
				name: "matrika_x",
				description: "je prvi obseg ali matrika števil in je lahko število ali ime, matrika ali sklic, ki vsebuje števila."
			},
			{
				name: "matrika_y",
				description: "je drugi obseg ali matrika števil in je lahko število ali ime, matrika ali sklic, ki vsebuje števila."
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Izračuna vsoto kvadratov razlik v dveh pripadajočih obsegih ali matrikah.",
		arguments: [
			{
				name: "matrika_x",
				description: "je prvi obseg ali matrika vrednosti in je lahko število ali ime, matrika ali sklic, ki vsebuje števila."
			},
			{
				name: "matrika_y",
				description: "je drugi obseg ali matrika vrednosti in je lahko število ali ime, matrika ali sklic, ki vsebuje števila."
			}
		]
	},
	{
		name: "SYD",
		description: "Vrne amortizacijo po metodi vsote letnih števk za sredstvo prek določenega obdobja.",
		arguments: [
			{
				name: "stroški",
				description: "je začetna cena sredstva."
			},
			{
				name: "vrednost_po_amor",
				description: "je rešena vrednost na koncu življenjske dobe sredstva."
			},
			{
				name: "št_obdobij",
				description: "je število obdobij, prek katerih se amortizira sredstvo (imenovano tudi življenjska doba sredstva)."
			},
			{
				name: "obdobje",
				description: "je obdobje in mora biti v enakih enotah kot življenjska doba."
			}
		]
	},
	{
		name: "T",
		description: "Preveri, ali je vrednost besedilo; če je, vrne besedilo, če ni, vrne dvojne narekovaje (prazno besedilo).",
		arguments: [
			{
				name: "vrednost",
				description: "je vrednost, ki jo želite preveriti."
			}
		]
	},
	{
		name: "T.DIST",
		description: "Vrne levorepo Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je številska vrednost, pri kateri se porazdelitev vrednoti."
			},
			{
				name: "stop_prostosti",
				description: "je celo število, ki označuje število stopenj prostosti, ki označujejo porazdelitev."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za porazdelitev gostote verjetnosti pa FALSE."
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Vrne dvorepo Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je številčna vrednosti, pri kateri se porazdelitev vrednoti."
			},
			{
				name: "stop_prostosti",
				description: "je celo število, ki označuje število stopenj prostosti, ki označujejo porazdelitev."
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Vrne desnorepo Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je številska vrednost, pri kateri se porazdelitev vrednoti."
			},
			{
				name: "stop_prostosti",
				description: "je celo število, ki označuje število stopenj prostosti, ki označujejo porazdelitev"
			}
		]
	},
	{
		name: "T.INV",
		description: "Vrne levorepo inverzno Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z dvorepo Studentovo t-porazdelitvijo in je število med 0 in vključno 1."
			},
			{
				name: "stop_prostosti",
				description: "je pozitivno celo število, ki označuje število stopenj prostosti in tako tudi porazdelitev."
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Vrne desnorepo inverzno Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z dvorepo Studentovo t-porazdelitvijo in je število med 0 in vključno 1."
			},
			{
				name: "stop_prostosti",
				description: "je pozitivno celo število, ki označuje število stopenj prostosti in tako tudi porazdelitev."
			}
		]
	},
	{
		name: "T.TEST",
		description: "Vrne verjetnost, povezano s Studentovim t-preizkusom.",
		arguments: [
			{
				name: "matrika1",
				description: "je prvi nabor podatkov."
			},
			{
				name: "matrika2",
				description: "je drugi nabor podatkov."
			},
			{
				name: "repi",
				description: "določa, koliko repov porazdelitve bo vrnila funkcija: enorepa porazdelitev = 1; dvorepa porazdelitev = 2."
			},
			{
				name: "vrsta",
				description: "je vrsta t-preizkusa: v paru = 1; dvovzorčni, z enako varianco (homoskedastični) = 2; dvovzorčni, z neenako varianco = 3."
			}
		]
	},
	{
		name: "TAN",
		description: "Vrne tangens kota.",
		arguments: [
			{
				name: "število",
				description: "je kot v radianih, za katerega želite poiskati tangens. Stopinje*PI()/180 = radiani."
			}
		]
	},
	{
		name: "TANH",
		description: "Vrne hiperbolični tangens števila.",
		arguments: [
			{
				name: "število",
				description: "je katero koli realno število."
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Vrne donos, ki je enak obveznici, za zakladno menico.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave zakladne menice, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti zakladne menice, izražen kot zaporedna številka datuma"
			},
			{
				name: "rabat",
				description: "je stopnja rabata zakladne menice"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Vrne ceno zakladne menice na 100 € imenske vrednosti.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave zakladne menice, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti zakladne menice, izražen kot zaporedna številka datuma"
			},
			{
				name: "rabat",
				description: "je stopnja rabata zakladne menice"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Vrne donos za zakladno menico.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave zakladne menice, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti zakladne menice, izražen kot zaporedna številka datuma"
			},
			{
				name: "cena",
				description: "je cena zakladne menice na 100 € imenske vrednosti"
			}
		]
	},
	{
		name: "TDIST",
		description: "Vrne Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je številska vrednost, pri kateri se porazdelitev vrednoti."
			},
			{
				name: "stop_prostosti",
				description: "je celo število, ki označuje število stopenj prostosti, ki označujejo porazdelitev."
			},
			{
				name: "repi",
				description: "določa, koliko repov porazdelitve bo vrnila funkcija: enorepa porazdelitev = 1; dvorepa porazdelitev = 2."
			}
		]
	},
	{
		name: "TEXT",
		description: "Vrednost pretvori v besedilo v točno določeni obliki zapisa števila.",
		arguments: [
			{
				name: "vrednost",
				description: "je številska vrednost, formula za vrednotenje na številsko vrednost ali sklic na celico s številsko vrednostjo."
			},
			{
				name: "oblika_besedila",
				description: "je številska oblika v besedilni obliki iz pogovornega okna »Oblika celic«, zavihka »Številke« in polja »Kategorija« (ne »Splošno«)."
			}
		]
	},
	{
		name: "TIME",
		description: "Pretvori ure, minute in sekunde, navedene kot števila, v Spreadsheetovo zaporedno število v časovni obliki.",
		arguments: [
			{
				name: "ura",
				description: "je število med 0 in 23, ki predstavlja uro."
			},
			{
				name: "minuta",
				description: "je število med 0 in 59, ki predstavlja minuto."
			},
			{
				name: "sekunda",
				description: "je število med 0 in 59, ki predstavlja sekundo."
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Pretvori uro, zapisano v besedilni obliki, v Spreadsheetovo zaporedno število za uro; števila od 0 (12:00:00) do 0,999988426 (23:59:59). Po vnosu formule oblikujte število z obliko zapisa ure.",
		arguments: [
			{
				name: "besedilo_ure",
				description: "je besedilni niz, ki navaja uro v eni od oblik zapisa ure programa Spreadsheet (informacije o datumu v nizu so prezrte)."
			}
		]
	},
	{
		name: "TINV",
		description: "Vrne dvorepo inverzno Studentovo t-porazdelitev.",
		arguments: [
			{
				name: "verjetnost",
				description: "je verjetnost, povezana z dvorepo Studentovo t-porazdelitvijo in je število med 0 in vključno 1."
			},
			{
				name: "stop_Prostosti",
				description: "je naravno število, ki označuje stopnje prostosti in tako tudi porazdelitev."
			}
		]
	},
	{
		name: "TODAY",
		description: "Vrne trenutni datum, oblikovan kot datum.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Pretvori navpični obseg celic v vodoravni obseg in obratno.",
		arguments: [
			{
				name: "matrika",
				description: "je obseg celic na delovnem listu ali matrika vrednosti, ki jo želite transponirati."
			}
		]
	},
	{
		name: "TREND",
		description: "Vrne števila v linearnem trendu, ujemajočem z znanimi podatkovnimi točkami po metodi najmanjših kvadratov.",
		arguments: [
			{
				name: "znani_y-i",
				description: "je obseg ali matrika y-vrednosti, ki jih že poznate za razmerje y = mx + b."
			},
			{
				name: "znani_x-i",
				description: "je izbiren obseg ali matrika x-vrednosti, ki jih že poznate za razmerje y = mx + b in je enake velikosti kot znani y-i (Znani_y-i)."
			},
			{
				name: "novi_x-i",
				description: "je obseg ali matrika novih x-vrednosti, za katere naj TREND vrne pripadajoče y-vrednosti."
			},
			{
				name: "konstanta",
				description: "je logična vrednost: konstanta b se izračuna normalno, če je konstanta enaka TRUE ali ni določena; b ima vrednost 0, če je konstanta enaka FALSE."
			}
		]
	},
	{
		name: "TRIM",
		description: "Iz besedilnega niza odstrani vse presledke, razen enojnih presledkov med besedami.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo, iz katerega želite odstraniti presledke."
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Vrne srednjo vrednost iz množice podatkovnih vrednosti.",
		arguments: [
			{
				name: "matrika",
				description: "je obseg ali matrika vrednosti, ki se obreže in izračuna povprečje."
			},
			{
				name: "odstotek",
				description: "je racionalno število (ulomek) – delež podatkovnih točk, ki jih želite izločiti iz računanja."
			}
		]
	},
	{
		name: "TRUE",
		description: "Vrne logično vrednost TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Prireže število na celo število tako, da odstrani decimalni del števila ali ulomek.",
		arguments: [
			{
				name: "število",
				description: "je število, ki ga želite prirezati."
			},
			{
				name: "št_števk",
				description: "je število, ki določa natančnost prirezovanja; 0, če izpuščeno."
			}
		]
	},
	{
		name: "TTEST",
		description: "Vrne verjetnost, povezano s Studentovim t-preskusom.",
		arguments: [
			{
				name: "matrika1",
				description: "je prvi nabor podatkov."
			},
			{
				name: "matrika2",
				description: "je drugi nabor podatkov."
			},
			{
				name: "repi",
				description: "določa, koliko repov porazdelitve bo vrnila funkcija: enorepa porazdelitev = 1; dvorepa porazdelitev = 2."
			},
			{
				name: "vrsta",
				description: "je vrsta t-preizkusa: v paru = 1; dvovzorčni, z enako varianco (homoskedastični) = 2; dvovzorčni, z neenako varianco = 3."
			}
		]
	},
	{
		name: "TYPE",
		description: "Vrne celo število, ki predstavlja podatkovno vrsto vrednosti: število = 1; besedilo = 2; logična vrednost = 4; vrednost napake = 16; matrika = 64.",
		arguments: [
			{
				name: "vrednost",
				description: "je lahko katera koli vrednost."
			}
		]
	},
	{
		name: "UNICODE",
		description: "Vrne število (kodno točko), ki ustreza prvemu znaku besedila.",
		arguments: [
			{
				name: "besedilo",
				description: "je znak, za katerega želite pridobiti vrednost Unicode"
			}
		]
	},
	{
		name: "UPPER",
		description: "Pretvori besedilni niz v vse velike črke.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo, ki ga želite pretvoriti v velike črke, sklic ali besedilni niz."
			}
		]
	},
	{
		name: "VALUE",
		description: "Pretvori besedilni niz, ki predstavlja število, v število.",
		arguments: [
			{
				name: "besedilo",
				description: "je besedilo v narekovajih ali sklic na celice z besedilom, ki ga želite pretvoriti."
			}
		]
	},
	{
		name: "VAR",
		description: "Oceni odmik glede na vzorec (v vzorcu prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo vzorcu populacije"
			},
			{
				name: "število2",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo vzorcu populacije"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Izračuna varianco na osnovi celotne populacije (v populaciji prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo populaciji"
			},
			{
				name: "število2",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo populaciji"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Oceni varianco na osnovi vzorca (v vzorcu prezre logične vrednosti in besedilo).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo vzorcu populacije"
			},
			{
				name: "število2",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo vzorcu populacije"
			}
		]
	},
	{
		name: "VARA",
		description: "Ugotovi varianco, ki temelji na vzorcu, ki vsebuje tudi logične vrednosti in besedilo. Besedilo in logična vrednost FALSE imata vrednost 0, logična vrednost TRUE pa ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 argumentov, ki ustrezajo vzorcu iz populacije"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 argumentov, ki ustrezajo vzorcu iz populacije"
			}
		]
	},
	{
		name: "VARP",
		description: "Izračuna odmik na podlagi celotne populacije (prezre logične vrednosti in besedilo v populaciji).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "število1",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo populaciji"
			},
			{
				name: "število2",
				description: "od 1 do 255 številskih argumentov, ki ustrezajo populaciji"
			}
		]
	},
	{
		name: "VARPA",
		description: "Izračuna varianco, ki temelji na celotni populaciji, ki vsebuje tudi logične vrednosti in besedilo. Besedilo in logična vrednost FALSE imata vrednost 0, logična vrednost TRUE pa ima vrednost 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "vrednost1",
				description: "od 1 do 255 argumentov, ki ustrezajo populaciji"
			},
			{
				name: "vrednost2",
				description: "od 1 do 255 argumentov, ki ustrezajo populaciji"
			}
		]
	},
	{
		name: "VDB",
		description: "Vrne amortizacijo sredstva za poljubno obdobje (tudi za delna obdobja), ki ga določite, z metodo dvojnopojemajočega salda ali s kakšno drugo metodo, ki jo določite.",
		arguments: [
			{
				name: "stroški",
				description: "je začetna cena sredstva."
			},
			{
				name: "vrednost_po_amor",
				description: "je rešena vrednost na koncu življenjske dobe sredstva."
			},
			{
				name: "št_obdobij",
				description: "je število obdobij, prek katerih se amortizira sredstvo (imenovano tudi življenjska doba sredstva)."
			},
			{
				name: "začetno_obdobje",
				description: "je začetno obdobje, za katerega želite izračunati amortizacijo, in mora biti v istih časovnih enotah kot »št_obdobij«."
			},
			{
				name: "končno_obdobje",
				description: "je zadnje obdobje, za katerega želite izračunati amortizacijo, in mora biti v istih časovnih enotah kot »št_obdobij«."
			},
			{
				name: "faktor",
				description: "je mera, s katero saldo upada. Če jo izpustite, je privzeta vrednost 2 (način dvojnopojemajočega salda)."
			},
			{
				name: "brez_preklopa",
				description: "program preklopi na linearno amortizacijo, ko je amortizacija večja od izračunanega upadajočega salda = FALSE ali izpuščeno; program ne preklopi = TRUE."
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Poišče vrednost v skrajnem levem stolpcu tabele in vrne vrednost v isti vrstici iz stolpca, ki ga navedete. Privzeto mora biti tabela urejena v naraščajočem vrstnem redu.",
		arguments: [
			{
				name: "iskana_vrednost",
				description: "je vrednost, ki naj se poišče v prvem stolpcu matrike in je lahko vrednost, sklic ali besedilni niz."
			},
			{
				name: "matrika_tabele",
				description: "je tabela z besedilom, števili ali logičnimi vrednostmi, iz katere so dobljeni podatki. Table_array je lahko sklic na obseg ali ime obsega."
			},
			{
				name: "št_indeksa_stolpca",
				description: "je številka stolpca v table_array, s katerega naj se vrne ujemajoča vrednost. Prvi stolpec vrednosti v tabeli je stolpec 1."
			},
			{
				name: "obseg_iskana",
				description: "je logična vrednost: če želite najti najboljše ujemanje v prvem stolpcu (urejeno v naraščajočem vrstnem redu) = TRUE ali izpuščeno; če želite najti natančno ujemanje = FALSE."
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Vrne število od 1 do 7, ki označuje dan v tednu datuma.",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število, ki predstavlja datum."
			},
			{
				name: "vrsta_rezultata",
				description: "je število: za nedeljo = 1 do sobote = 7, uporabite 1; za ponedeljek = 1 do nedelje = 7, uporabite 2; za ponedeljek = 0 do nedelje = 6, uporabite 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Vrne število tednov v letu.",
		arguments: [
			{
				name: "serijska_številka",
				description: "je datumsko-časovna koda, ki jo uporablja program Spreadsheet za izračune datuma in časa"
			},
			{
				name: "vrsta_vrednosti",
				description: "je število (1 ali 2), ki določa vrsto vrnjene vrednosti"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Vrne Weibullovo porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri se vrednoti funkcija, in je nenegativno število."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Vrne Weibullovo porazdelitev.",
		arguments: [
			{
				name: "x",
				description: "je vrednost, pri kateri se vrednoti funkcija, in je nenegativno število."
			},
			{
				name: "alfa",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "beta",
				description: "je parameter porazdelitve in je pozitivno število."
			},
			{
				name: "kumulativno",
				description: "je logična vrednost: za kumulativno porazdelitveno funkcijo uporabite TRUE; za verjetnostno masno funkcijo pa FALSE."
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Vrne zaporedno številko datuma pred ali po navedenem številu delovnih dni.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum"
			},
			{
				name: "dnevi",
				description: "je število delovnih dni pred ali po začetnem datumu"
			},
			{
				name: "prazniki",
				description: "je izbirna matrika ene ali več zaporednih številk datuma, ki bodo izključene iz delovnega koledarja, kot so državni in zvezni prazniki ter porodniški dopusti"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Vrne zaporedno številko datuma pred ali po navedenem številu delovnih dni s parametri vikendov po meri.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum"
			},
			{
				name: "dnevi",
				description: "je število delovnih dni pred ali po začetnem datumu"
			},
			{
				name: "vikend",
				description: "je število ali nabor, ki določa, kdaj nastopijo vikendi"
			},
			{
				name: "prazniki",
				description: "je izbirna matrika za več zaporednih številk datuma, ki bodo izključeni iz delovnega koledarja, na primer državni in zvezni prazniki ter porodniški dopusti"
			}
		]
	},
	{
		name: "XIRR",
		description: "Vrne notranjo stopnjo povračila za razpored pretokov denarja.",
		arguments: [
			{
				name: "vrednosti",
				description: "je niz pretokov denarja, ki ustrezajo razporedu plačil v datumih"
			},
			{
				name: "datumi",
				description: "je razpored datumov plačil, ki ustrezajo plačilom pretokov denarja"
			},
			{
				name: "domneva",
				description: "je število, za katerega menite, da je blizu rezultatu XIRR"
			}
		]
	},
	{
		name: "XNPV",
		description: "Vrne sedanjo neto vrednost za razpored pretokov denarja.",
		arguments: [
			{
				name: "stopnja",
				description: "je stopnja rabata, ki jo boste uporabili na pretokih denarja"
			},
			{
				name: "vrednosti",
				description: "je niz pretokov denarja, ki ustrezajo razporedu plačil v datumih"
			},
			{
				name: "datumi",
				description: "je razpored datumov plačil, ki ustrezajo plačilom pretokov denarja"
			}
		]
	},
	{
		name: "XOR",
		description: "Vrne logični »Exclusive Or« vseh argumentov.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logical1",
				description: "so pogoji 1 do 254, ki jih želite preskusiti in so lahko TRUE ali FALSE ter so lahko logične vrednosti, matrike ali sklici"
			},
			{
				name: "logical2",
				description: "so pogoji 1 do 254, ki jih želite preskusiti in so lahko TRUE ali FALSE ter so lahko logične vrednosti, matrike ali sklici"
			}
		]
	},
	{
		name: "YEAR",
		description: "Vrne leto datuma, ki je celo število v intervalu od 1900 - 9999.",
		arguments: [
			{
				name: "serijska_številka",
				description: "je število v datumsko-časovni kodi, ki jo uporablja Spreadsheet"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Vrne ulomek leta, ki predstavlja število celih dni med začetnim in končnim datumom.",
		arguments: [
			{
				name: "začetni_datum",
				description: "je zaporedna številka datuma, ki predstavlja začetni datum"
			},
			{
				name: "končni_datum",
				description: "je zaporedna številka datuma, ki predstavlja končni datum"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Vrne letni donos za vrednostni papir z rabatom. Na primer, zakladna menica.",
		arguments: [
			{
				name: "poravnava",
				description: "je datum poravnave vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "zapadlost",
				description: "je datum zapadlosti vrednostnega papirja, izražen kot zaporedna številka datuma"
			},
			{
				name: "cena",
				description: "je cena vrednostnega papirja na 100 € imenske vrednosti"
			},
			{
				name: "odkup",
				description: "je amortizacijska vrednost vrednostnega papirja na 100 € imenske vrednosti"
			},
			{
				name: "osnova",
				description: "je vrsta na osnovi štetja dni za uporabo"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Vrne enorepo P-vrednost z-preizkusa.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov, proti katerim se preizkuša x."
			},
			{
				name: "x",
				description: "je vrednost, ki se preizkuša."
			},
			{
				name: "sigma",
				description: "je (znani) standardni odklon populacije. Če je izpuščena, bo uporabljen standardni odklon vzorca"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Vrne enorepo P-vrednost z-preizkusa.",
		arguments: [
			{
				name: "matrika",
				description: "je matrika ali obseg podatkov, proti katerim se preizkuša x."
			},
			{
				name: "x",
				description: "je vrednost, ki se preizkuša."
			},
			{
				name: "sigma",
				description: "je (znani) standardni odklon populacije. Če se izpusti, bo uporabljen standardni odklon vzorca"
			}
		]
	}
];