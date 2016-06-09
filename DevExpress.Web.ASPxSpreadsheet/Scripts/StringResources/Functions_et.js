ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Tagastab arvu absoluutväärtuse, arvu ilma märgita.",
		arguments: [
			{
				name: "arv",
				description: "on reaalarv, mille absoluutväärtust soovite leida"
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Tagastab tagasimaksetähtajal intressi kandva väärtpaberi arvestatud intressi.",
		arguments: [
			{
				name: "emissioon",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tehing",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "määr",
				description: "on väärtpaberi aastane kupongiintressi määr"
			},
			{
				name: "nominaalväärtus",
				description: "on väärtpaberi nominaalväärtus"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "ACOS",
		description: "Annab vastuseks arvu arkuskoosinuse, radiaanides vahemikus 0 kuni pii. Arkuskoosinus on nurk, mille koosinus on Arv.",
		arguments: [
			{
				name: "arv",
				description: "on soovitud nurga koosinus, mis peab jääma vahemikku –1 kuni 1"
			}
		]
	},
	{
		name: "ACOSH",
		description: "Annab vastuseks arvu arkushüperboolse koosinuse.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv, mis on võrdne või suurem kui 1"
			}
		]
	},
	{
		name: "ACOT",
		description: "Tagastab arvu arkuskootangensi radiaanides vahemikus 0 kuni pii.",
		arguments: [
			{
				name: "arv",
				description: "on teie soovitud nurga kootangens"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Tagastab arvu areakootangensi.",
		arguments: [
			{
				name: "arv",
				description: "on teie soovitud nurga hüperboolne kootangens"
			}
		]
	},
	{
		name: "ADDRESS",
		description: "Loob lahtriviite tekstina, kui on määratud rea ja veeru numbrid.",
		arguments: [
			{
				name: "rea_nr",
				description: "on lahtriviites kasutatav reanumber: rea_nr = 1 rea 1 puhul"
			},
			{
				name: "veeru_nr",
				description: "on lahtriviites kasutatav veerunumber. Nt veeru D puhul veeru_nr = 4"
			},
			{
				name: "abs_arv",
				description: "määrab viitetüübi: absoluutne = 1; absoluutne rida/suhteline veerg = 2; suhteline rida/absoluutne veerg = 3; suhteline = 4"
			},
			{
				name: "a1",
				description: "on loogikaväärtus, mis määrab viitelaadi: A1 laad = 1 või TRUE; R1C1 laad = 0 või FALSE"
			},
			{
				name: "lehe_tekst",
				description: "on tekst, mis määrab töölehe nime, mida välise viitena kasutada"
			}
		]
	},
	{
		name: "AND",
		description: "Kontrollib, kas kõik argumendid on TRUE ja tagastab väärtuse TRUE, kui kõik argumendid on TRUE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "loogika1",
				description: "on 1–255 tingimust, mida soovite testida; tingimus võib olla kas TRUE või FALSE ja see võib olla loogikaväärtus, massiiv või viide"
			},
			{
				name: "loogika2",
				description: "on 1–255 tingimust, mida soovite testida; tingimus võib olla kas TRUE või FALSE ja see võib olla loogikaväärtus, massiiv või viide"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Teisendab Rooma numbri araabia numbriks.",
		arguments: [
			{
				name: "tekst",
				description: "on Rooma number, mida soovite teisendada"
			}
		]
	},
	{
		name: "AREAS",
		description: "Tagastab alade arvu viites. Ala on külgnevate lahtrite vahemik või eraldiseisev lahter.",
		arguments: [
			{
				name: "viide",
				description: "on viide lahtrile või lahtrivahemikule ning see võib viidata mitmele alale"
			}
		]
	},
	{
		name: "ASIN",
		description: "Annab vastuseks arvu arkussiinuse radiaanides, vahemikus -pii/2 kuni pii/2.",
		arguments: [
			{
				name: "arv",
				description: "on soovitud nurga siinus, mis peab jääma vahemikku –1 kuni 1"
			}
		]
	},
	{
		name: "ASINH",
		description: "Annab vastuseks arvu arkushüperboolse siinuse.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv, mis on võrdne või suurem kui 1"
			}
		]
	},
	{
		name: "ATAN",
		description: "Tagastab arvu arkustangensi radiaanides, vahemikus –pii/2 kuni pii/2.",
		arguments: [
			{
				name: "arv",
				description: "on soovitud nurga tangens"
			}
		]
	},
	{
		name: "ATAN2",
		description: "Annab vastuseks määratud x- ja y- koordinaatide arkustangensi radiaanides vahemikus -pii ja pii, jättes välja -pii.",
		arguments: [
			{
				name: "x_arv",
				description: "on punkti x-koordinaat"
			},
			{
				name: "y_arv",
				description: "on punkti y-koordinaat"
			}
		]
	},
	{
		name: "ATANH",
		description: "Annab vastuseks arvu arkushüperboolse tangensi.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv -1 ja 1 vahel, välja arvatud -1 ja 1"
			}
		]
	},
	{
		name: "AVEDEV",
		description: "Andmepunktide absoluuthälvete keskmine arvutatuna nende keskväärtusest. Argumentideks võivad olla arvud või lahtrinimed, massiivid või arve sisaldavad viited.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 argumenti, mille kohta soovite arvutada absoluuthälvete keskmise"
			},
			{
				name: "arv2",
				description: "on 1–255 argumenti, mille kohta soovite arvutada absoluuthälvete keskmise"
			}
		]
	},
	{
		name: "AVERAGE",
		description: "Tagastab argumentide keskmise (aritmeetilise keskväärtuse); argumentideks võivad olla arvud või lahtrinimed, massiivid või arve sisaldavad viited.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvargumenti, mille keskmist soovite arvutada"
			},
			{
				name: "arv2",
				description: "on 1–255 arvargumenti, mille keskmist soovite arvutada"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Tagastab oma argumentide keskmise (aritmeetilise keskväärtuse), väärtustades argumentides teksti ja väärtused FALSE 0-na; TRUE saab väärtuse 1. Argumendid võivad olla arvud, nimed, massiivid või viited.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 argumenti, mille keskmist soovite teada saada"
			},
			{
				name: "väärtus2",
				description: "on 1–255 argumenti, mille keskmist soovite teada saada"
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Leiab etteantud tingimuse või kriteeriumidega määratud lahtrite keskmise (aritmeetilise keskmise).",
		arguments: [
			{
				name: "vahemik",
				description: "on lahtrivahemik, mille soovite väärtustada"
			},
			{
				name: "kriteeriumid",
				description: "on arvu, avaldise või tekstina esitatud tingimus või kriteeriumid, mis määratleb, milliseid lahtreid keskmise leidmiseks kasutatakse"
			},
			{
				name: "keskmistatav_vahemik",
				description: "on tegelikud keskmise leidmiseks kasutatavad lahtrid. Vaikimisi kasutatakse vahemiku lahtreid "
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Leiab etteantud tingimuse või kriteeriumidega määratud lahtrite keskmise (aritmeetilise keskmise).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "keskmistatav_vahemik",
				description: "on tegelikud keskmise leidmiseks kasutatavad lahtrid."
			},
			{
				name: "kriteeriumide_vahemik",
				description: "on lahtrivahemik, mille soovite konkreetse tingimuse puhul väärtustada"
			},
			{
				name: "kriteeriumid",
				description: "on arvu, avaldise või tekstina esitatud tingimus või kriteeriumid, mis määratleb, milliseid lahtreid keskmise leidmiseks kasutatakse"
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Teisendab arvu tekstiks (bahtis).",
		arguments: [
			{
				name: "arv",
				description: "on arv, mida soovite teisendada"
			}
		]
	},
	{
		name: "BASE",
		description: "Teisendab arvu määratud arvusüsteemi aluse põhjal tekstkujule.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mida soovite teisendada"
			},
			{
				name: "arvusüsteemi_alus",
				description: "on arvusüsteemi alus, mida soovite arvu teisendamiseks kasutada"
			},
			{
				name: "miinimumpikkus",
				description: "on tagastatava stringi miinimumpikkus. Kui see ära jätta, siis algusnulle ei lisata"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Tagastab muudetud Besseli funktsiooni In(x).",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millel funktsioon väärtustatakse"
			},
			{
				name: "n",
				description: "on Besseli funktsiooni järk"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Tagastab Besseli funktsiooni Jn(x).",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millel funktsioon väärtustatakse"
			},
			{
				name: "n",
				description: "on Besseli funktsiooni järk"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Tagastab muudetud Besseli funktsiooni Kn(x).",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millel funktsioon väärtustatakse"
			},
			{
				name: "n",
				description: "on funktsiooni järk"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Tagastab Besseli funktsiooni Yn(x).",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millel funktsioon väärtustatakse"
			},
			{
				name: "n",
				description: "on funktsiooni järk"
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Beetajaotuse jaotusfunktsioon.",
		arguments: [
			{
				name: "x",
				description: "on funktsiooni argument vahemikust A kuni B"
			},
			{
				name: "alfa",
				description: "on jaotusparameeter, mis peab olema suurem kui 0"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, mis peab olema suurem kui 0"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			},
			{
				name: "A",
				description: "on valikuline argumendi x intervalli alampiir. Vaikesättena on A=0"
			},
			{
				name: "B",
				description: "on valikuline argumendi x intervalli ülempiir. Vaikesättena on B=1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Beetajaotuse tihedusfunktsiooni pöördfunktsioon (BETA.DIST).",
		arguments: [
			{
				name: "tõenäosus",
				description: "on beetajaotusega seotud tõenäosus"
			},
			{
				name: "alfa",
				description: "on jaotusparameeter, peab olema suurem kui 0"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, peab olema suurem kui 0"
			},
			{
				name: "A",
				description: "on valikuline argumendi x intervalli alampiir. Vaikesättena on A=0"
			},
			{
				name: "B",
				description: "valikuline argumendi x intervalli ülempiir. Vaikesättena on B=1"
			}
		]
	},
	{
		name: "BETADIST",
		description: "Beetajaotuse tihedusfunktsioon.",
		arguments: [
			{
				name: "x",
				description: "on funktsiooni argument vahemikust A kuni B"
			},
			{
				name: "alfa",
				description: "on jaotusparameeter, mis peab olema suurem kui 0"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, mis peab olema suurem kui 0"
			},
			{
				name: "A",
				description: "on valikulise argumendi x intervalli alampiir. Vaikesättena A=0"
			},
			{
				name: "B",
				description: "on valikulise argumendi x intervalli ülempiir. Vaikesättena B=1"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Beetajaotuse tihedusfunktsiooni pöördfunktsioon (BETADIST).",
		arguments: [
			{
				name: "tõenäosus",
				description: "on beetajaotusega seotud tõenäosus"
			},
			{
				name: "alfa",
				description: "on jaotusparameeter, peab olema suurem kui 0"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, peab olema suurem kui 0"
			},
			{
				name: "A",
				description: "on valikuline argumendi x intervalli alampiir. Vaikesättena on A=0"
			},
			{
				name: "B",
				description: "valikuline argumendi x intervalli ülempiir. Vaikesättena on B=1"
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Teisendab kahendarvu kümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kahendarv"
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Teisendab kahendarvu kuueteistkümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kahendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Teisendab kahendarvu kaheksandarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kahendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Termi binoomjaotuse tõenäosus.",
		arguments: [
			{
				name: "edukate_arv",
				description: "on edukate katsete arv"
			},
			{
				name: "katsete_arv",
				description: "sõltumatute katsete arv"
			},
			{
				name: "tõenäosus",
				description: "on iga katse edukuse tõenäosus"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosusmassi funktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Tagastab katsetulemuse tõenäosuse binoomjaotuse põhjal.",
		arguments: [
			{
				name: "katseid",
				description: "on sõltumatute katsete arv"
			},
			{
				name: "edukate_tõenäosus",
				description: "on iga katse õnnestumise tõenäosus"
			},
			{
				name: "edukate_arv",
				description: "on õnnestumiste arv katsetes"
			},
			{
				name: "edukate_arv2",
				description: "kui see argument on sisestatud, tagastab funktsioon tõenäosuse, et õnnestunud katsete arv jääb vahemikku edukate_arv kuni edukate_arv2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Väikseim väärtus, mille puhul on kumulatiivne binoomjaotus kriteeriumi väärtusest suurem või sellega võrdne .",
		arguments: [
			{
				name: "katsete_arv",
				description: "on Bernoulli katsete arv"
			},
			{
				name: "tõenäosus",
				description: "on iga katse edukuse tõenäosus vahemikus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "alfa",
				description: "on kriteeriumi väärtus vahemikus 0–1 (0 ja 1 k.a)"
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Termi binoomjaotuse tõenäosus.",
		arguments: [
			{
				name: "edukate_arv",
				description: "on edukate katsete arv"
			},
			{
				name: "katsete_arv",
				description: "sõltumatute katsete arv"
			},
			{
				name: "tõenäosus",
				description: "on iga katse edukuse tõenäosus"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosusmassi funktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "BITAND",
		description: "Tagastab kahe arvu 'ja'-väärtuse bitikuju.",
		arguments: [
			{
				name: "arv1",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			},
			{
				name: "arv2",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Tagastab nihke_hulga bittide poolt vasakule nihutatud arvu.",
		arguments: [
			{
				name: "arv",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			},
			{
				name: "nihke_hulk",
				description: "on bittide arv, mille võrra soovite arvu vasakule nihutada"
			}
		]
	},
	{
		name: "BITOR",
		description: "Tagastab kahe arvu 'või'-väärtuse bitikuju.",
		arguments: [
			{
				name: "arv1",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			},
			{
				name: "arv2",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Tagastab nihke_hulga bittide poolt paremale nihutatud arvu.",
		arguments: [
			{
				name: "arv",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			},
			{
				name: "nihke_hulk",
				description: "on bittide arv, mille võrra soovite arvu paremale nihutada"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Tagastab kahe arvu 'eksklusiivne või'-väärtuse bitikuju.",
		arguments: [
			{
				name: "arv1",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			},
			{
				name: "arv2",
				description: "on selle kahendarvu kümnendkuju, mille väärtust soovite arvutada"
			}
		]
	},
	{
		name: "CEILING",
		description: "Ümardab arvu ülespoole ümardusaluse lähima kordseni.",
		arguments: [
			{
				name: "arv",
				description: "on ümardatav arv"
			},
			{
				name: "ümardusalus",
				description: "on kordaja, milleni ümardatakse"
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Ümardab arvu ülespoole lähima täisarvuni või ümardusaluse lähima kordseni.",
		arguments: [
			{
				name: "arv",
				description: "on väärtus, mida soovite ümardada"
			},
			{
				name: "ümardusalus",
				description: "on kordne, milleni soovite ümardada"
			},
			{
				name: "režiim",
				description: "kui see argument on sisestatud ja pole null, ümardab see funktsioon nullist eemale"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Ümardab arvu absoluutväärtusest ülespoole ümardusaluse lähima kordseni või vastavalt määratud täpsusele.",
		arguments: [
			{
				name: "arv",
				description: "on ümardatav arv"
			},
			{
				name: "ümardusalus",
				description: "on kordaja, milleni ümardatakse"
			}
		]
	},
	{
		name: "CELL",
		description: "Tagastab teabe vastavalt lehe lugemisjärjestusele viite esimese lahtri vormingu, paigutuse või sisu kohta.",
		arguments: [
			{
				name: "teabe_tüüp",
				description: "on tekstiväärtus, mis määrab teie soovitud lahtriteabe tüübi."
			},
			{
				name: "viide",
				description: "on lahter, mille kohta soovite teavet"
			}
		]
	},
	{
		name: "CHAR",
		description: "Tagastab märgi, mille määrab teie arvutis seatud märgistiku koodinumber.",
		arguments: [
			{
				name: "arv",
				description: "on arv vahemikus 1–255, mis määrab teie soovitud märgi"
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Hii-ruutjaotuse parempoolse piirangu tõenäosus.",
		arguments: [
			{
				name: "x",
				description: "on mittenegatiivne arv, mille põhjal arvutatakse o-hüpoteesile vastav hii-ruutjaotuse tõenäosus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on vabadusastmete arv lõigus 1 kuni 10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Hii-ruutjaotuse parempoolse piirangu tõenäosuse pöördfunktsioon.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on hii-ruutjaotusega seotud tõenäosus lõigus 0 kuni 1 (k.a)"
			},
			{
				name: "vabadusastmete_arv",
				description: "on vabadusastmete arv lõigus1 kuni 10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Hii-ruutjaotuse vasakpoolse piirangu tõenäosus.",
		arguments: [
			{
				name: "x",
				description: "mittenegatiivne väärtus, millest arvutatakse jaotus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Hii-ruutjaotuse parempoolse piirangu tõenäosus.",
		arguments: [
			{
				name: "x",
				description: "mittenegatiivne väärtus, millest arvutatakse jaotus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Hii-ruutjaotuse vasakpoolse piirangu tõenäosuse pöördfunktsioon.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on hii-ruutjaotusega seotud tõenäosus lõigus 0–1(k.a)"
			},
			{
				name: "vabadusastmete_arv",
				description: "on vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Hii-ruutjaotuse parempoolse piirangu tõenäosuse pöördfunktsioon.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on hii-ruutjaotusega seotud tõenäosus lõigus 0–1(k.a)"
			},
			{
				name: "vabadusastmete_arv",
				description: "on vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Sõltumatuse test: hii-ruutjaotuse ja vabadusastmete väärtus.",
		arguments: [
			{
				name: "tegelik_vahemik",
				description: "on andmevahemik, mis sisaldab oodatud väärtusega testitavaid vaatlusi"
			},
			{
				name: "oodatud_vahemik",
				description: "on andmevahemik, mis sisaldab ridade ja veergude summade korrutise suhtarvu üldkokkuvõttesse"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Sõltumatuse test: hii-ruutjaotuse ja vabadusastmete väärtus.",
		arguments: [
			{
				name: "tegelik_vahemik",
				description: "on andmevahemik, mis sisaldab oodatud väärtusega testitavaid vaatlusi"
			},
			{
				name: "oodatud_vahemik",
				description: "on andmevahemik, mis sisaldab ridade ja veergude summade korrutise suhtarvu üldkokkuvõttesse"
			}
		]
	},
	{
		name: "CHOOSE",
		description: "Valib väärtusteloendist sooritatava väärtuse või toimingu, toetudes indeksarvule.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indeks",
				description: "määrab, milline väärtusargument valitakse. Indeks peab olema arv vahemikus 1 kuni 254, valem või viide mõnele arvule vahemikus 1–254"
			},
			{
				name: "väärtus1",
				description: "on 1–254 arvu, lahtriviidet, määratletud nime, valemit, funktsiooni või tekstiargumenti, millest VALI valib"
			},
			{
				name: "väärtus2",
				description: "on 1–254 arvu, lahtriviidet, määratletud nime, valemit, funktsiooni või tekstiargumenti, millest VALI valib"
			}
		]
	},
	{
		name: "CLEAN",
		description: "Eemaldab tekstist kõik printimatud märgid.",
		arguments: [
			{
				name: "tekst",
				description: "on suvaline tööleheteave, millest soovite printimatud märgid eemaldada"
			}
		]
	},
	{
		name: "CODE",
		description: "Tagastab tekstistringi esimese märgi numbrilise koodi teie arvuti kasutatavas märgistikus.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst, mille esimese märgi koodi soovite otsida"
			}
		]
	},
	{
		name: "COLUMN",
		description: "Tagastab suvalise viite veerunumbri.",
		arguments: [
			{
				name: "viide",
				description: "on lahter või külgnevate lahtrite vahemik, mille veerunumbrit soovite. Kui see on tühi, kasutatakse funktsiooni COLUMN sisaldavat lahtrit"
			}
		]
	},
	{
		name: "COLUMNS",
		description: "Tagastab veergude arvu massiivis või viites.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või massiivivalem või viide lahtrivahemikule, mille kohta soovite veergude arvu otsida"
			}
		]
	},
	{
		name: "COMBIN",
		description: "Tagastab antud üksuste kombinatsioonide arvu.",
		arguments: [
			{
				name: "arv",
				description: "on üksuste koguarv"
			},
			{
				name: "valitud_arv",
				description: "on üksuste arv igas kombinatsioonis"
			}
		]
	},
	{
		name: "COMBINA",
		description: "Tagastab kombinatsioonide arvu (koos kordustega) teatud arvu üksuste kohta.",
		arguments: [
			{
				name: "arv",
				description: "on üksuste arv kokku"
			},
			{
				name: "valitud_arv",
				description: "on üksuste arv iga kombinatsiooni korral"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Teisendab reaal- ja imaginaarosa kompleksarvuks.",
		arguments: [
			{
				name: "reaalosa",
				description: "on kompleksarvu reaalosa"
			},
			{
				name: "imaginaarosa",
				description: "on kompleksarvu imaginaarosa"
			},
			{
				name: "sufiks",
				description: "on kompleksarvu imaginaarosa järelliide"
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Ühendab mitu tekstistringi üheks stringiks.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "on 1–255 tekstüksust, mis ühendatakse üheks üksuseks ja mis võivad olla tekstistringid, arvud või üksikud lahtriviited"
			},
			{
				name: "tekst2",
				description: "on 1–255 tekstüksust, mis ühendatakse üheks üksuseks ja mis võivad olla tekstistringid, arvud või üksikud lahtriviited"
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Normaaljaotusega populatsioonikeskmise usaldusvahemik.",
		arguments: [
			{
				name: "alfa",
				description: "on olulisuse tase, mille alusel arvutatakse usaldustase, on suurem kui 0 ja väiksem kui 1"
			},
			{
				name: "standardhälve",
				description: "on andmevahemiku alusel arvutatud standardhälve populatsioonis, eelduse järgi teadaolev. Standardhälve peab olema suurem kui 0"
			},
			{
				name: "suurus",
				description: "on valimi suurus"
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Populatsioonikeskmise normaaljaotusega usaldusvahemik.",
		arguments: [
			{
				name: "alfa",
				description: "on olulisuse tase, mille alusel arvutatakse usaldustase, on suurem kui 0 ja väiksem kui 1"
			},
			{
				name: "standardhälve",
				description: "on andmevahemiku alusel arvutatud standardhälve populatsioonis, eelduse järgi teadaolev. Standardhälve peab olema suurem kui 0"
			},
			{
				name: "suurus",
				description: "on valimi suurus"
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Populatsioonikeskmise Studenti t-jaotusega usaldusvahemik.",
		arguments: [
			{
				name: "alfa",
				description: "on olulisuse tase, mille alusel arvutatakse usaldustase, on suurem kui 0 ja väiksem kui 1"
			},
			{
				name: "standardhälve",
				description: "on andmevahemiku alusel arvutatud standardhälve populatsioonis, eelduse järgi teadaolev. Standardhälve peab olema suurem kui 0"
			},
			{
				name: "suurus",
				description: "on valimi suurus"
			}
		]
	},
	{
		name: "CONVERT",
		description: "Teisendab arvu ühest mõõtühikute süsteemist teise.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav väärtus argumendiga ühikust määratud mõõtühikutes"
			},
			{
				name: "ühikust",
				description: "on arvu mõõtühikud"
			},
			{
				name: "ühikuks",
				description: "tulemi mõõtühikud"
			}
		]
	},
	{
		name: "CORREL",
		description: "Kahe andmemassiivi korrelatsioonikordaja.",
		arguments: [
			{
				name: "massiiv1",
				description: "on andmeid sisaldav lahtrivahemik. Andmed peavad olema arvud, lahtrinimed, massiivid või lahtriviidad arvudele"
			},
			{
				name: "massiiv2",
				description: "on andmeid sisaldav teine lahtrivahemik. Andmed peavad olema arvud, lahtrinimed, massiivid või lahtriviidad arvudele"
			}
		]
	},
	{
		name: "COS",
		description: "Tagastab nurga koosinuse.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille koosinust soovite leida"
			}
		]
	},
	{
		name: "COSH",
		description: "Annab vastuseks arvu hüperboolse koosinuse.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv"
			}
		]
	},
	{
		name: "COT",
		description: "Tagastab nurga kootangensi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille kootangensit soovite leida"
			}
		]
	},
	{
		name: "COTH",
		description: "Tagastab arvu hüperboolse kootangensi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille hüperboolset kootangensit soovite leida"
			}
		]
	},
	{
		name: "COUNT",
		description: "Loendab vahemiku lahtreid, mis sisaldavad arve.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 argumenti, mis võivad sisaldada või viidata mitmesugust tüüpi andmetele, kuid loendatakse ainult arvud"
			},
			{
				name: "väärtus2",
				description: "on 1–255 argumenti, mis võivad sisaldada või viidata mitmesugust tüüpi andmetele, kuid loendatakse ainult arvud"
			}
		]
	},
	{
		name: "COUNTA",
		description: "Loendab lahtrite arvu, mis pole tühjad.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 argumenti, mis tähistavad väärtusi ja lahtreid, mida soovite loendada. Väärtused võivad olla suvalist tüüpi teave"
			},
			{
				name: "väärtus2",
				description: "on 1–255 argumenti, mis tähistavad väärtusi ja lahtreid, mida soovite loendada. Väärtused võivad olla suvalist tüüpi teave"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Loendab tühjade lahtrite arvu määratud lahtrivahemikus.",
		arguments: [
			{
				name: "vahemik",
				description: "on vahemik, millest soovite tühje lahtreid loendada"
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Loendab vahemikus etteantud tingimusele vastavate lahtrite arvu.",
		arguments: [
			{
				name: "vahemik",
				description: "on lahtrivahemik, millest soovite mittetühje lahtreid loendada"
			},
			{
				name: "kriteeriumid",
				description: "on arvu, avaldise või tekstina esitatud tingimus, mis määratleb, milliseid lahtreid loendatakse"
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Loendab määratud tingimuste või kriteeriumide hulgaga määratud lahtrite arvu.",
		arguments: [
			{
				name: "kriteeriumide_vahemik",
				description: "on lahtrivahemik, mille soovite konkreetse tingimuse puhul väärtustada"
			},
			{
				name: "kriteeriumid",
				description: "on arvu, avaldise või tekstina esitatud tingimus, mis määratleb, milliseid lahtreid loendatakse"
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Tagastab päevade arvu kupongiperioodi algusest kupongi arvelduskuupäevani.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "sagedus",
				description: "on kupongimaksete arv aastas"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Tagastab järgmise kupongi alguskuupäeva pärast arvelduskuupäeva.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "sagedus",
				description: "on kupongimaksete arv aastas"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Tagastab tasutavate kupongide arvu arvelduskuupäevast maksetähtpäevani.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "sagedus",
				description: "on kupongimaksete arv aastas"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Tagastab eelmise kupongi alguskuupäeva enne arvelduskuupäeva.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "sagedus",
				description: "on kupongimaksete arv aastas"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "COVAR",
		description: "Kovariatsioon, kahe massiivi andmepunktipaaride standardhälvete korrutiste keskmine.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene täisarve sisaldav lahtrivahemik, peab sisaldama arve, massiive või lahtriviitasid arvudele"
			},
			{
				name: "massiiv2",
				description: "on teine täisarve sisaldav lahtrivahemik, peab sisaldama arve, massiive või lahtriviitasid arvudele"
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Populatsiooni kovariatsioon, kahe andmehulga andmepunktipaaride standardhälvete korrutiste keskmine.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene täisarve sisaldav lahtrivahemik, peab sisaldama arve, massiive või lahtriviitasid arvudele"
			},
			{
				name: "massiiv2",
				description: "on teine täisarve sisaldav lahtrivahemik, peab sisaldama arve, massiive või lahtriviitasid arvudele"
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Valimi kovariatsioon, kahe andmehulga andmepunktipaaride standardhälvete korrutiste keskmine.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene täisarve sisaldav lahtrivahemik, peab sisaldama arve, massiive või lahtriviitasid arvudele"
			},
			{
				name: "massiiv2",
				description: "on teine täisarve sisaldav lahtrivahemik, peab sisaldama arve, massiive või lahtriviitasid arvudele"
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Väikseim väärtus, mille puhul on kumulatiivne binoomjaotus suurem või võrdne kriteeriumi väärtusega.",
		arguments: [
			{
				name: "katsete_arv",
				description: "on Bernoulli katsete arv"
			},
			{
				name: "tõenäosus",
				description: "on iga katse edukuse tõenäosus vahemikus 0 kuni 1 (0 ja 1 k.a)"
			},
			{
				name: "alfa",
				description: "on kriteeriumi väärtus vahemikus 0 kuni 1 (0 ja 1 k.a)"
			}
		]
	},
	{
		name: "CSC",
		description: "Tagastab nurga koosekansi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille koosekansit soovite leida"
			}
		]
	},
	{
		name: "CSCH",
		description: "Tagastab nurga hüperboolse koosekansi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille hüperboolset koosekansit soovite leida"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Tagastab kahe perioodi vahel makstud kumulatiivse intressi.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär"
			},
			{
				name: "per_arv",
				description: "on makseperioodide koguarv"
			},
			{
				name: "praeg_väärtus",
				description: "on praegune väärtus"
			},
			{
				name: "perioodi_algus",
				description: "on arvutuse esimene periood"
			},
			{
				name: "perioodi_lõpp",
				description: "on arvutuse viimane periood"
			},
			{
				name: "tüüp",
				description: "on maksetähtajale osutav väärtus"
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Tagastab kahe perioodi vahelise kumulatiivse põhimakse laenult.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär"
			},
			{
				name: "per_arv",
				description: "on makseperioodide koguarv"
			},
			{
				name: "praeg_väärtus",
				description: "on praegune väärtus"
			},
			{
				name: "perioodi_algus",
				description: "on arvutuse esimene periood"
			},
			{
				name: "perioodi_lõpp",
				description: "on arvutuse viimane periood"
			},
			{
				name: "tüüp",
				description: "on maksetähtajale osutav väärtus"
			}
		]
	},
	{
		name: "DATE",
		description: "Tagastab arvu, mis tähistab Spreadsheet kuupäeva-kellaaja koodis kuupäeva.",
		arguments: [
			{
				name: "aasta",
				description: "on arv 1900–9999 Spreadsheet for Windowsis või 1904–9999 Spreadsheet for the Macintoshis"
			},
			{
				name: "kuu",
				description: "on arv vahemikus 1–12, mis tähistab kuud"
			},
			{
				name: "päev",
				description: "on arv vahemikus 1–31, mis tähistab kuupäeva"
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
		description: "Teisendab tekstina esitatud kuupäeva arvuks, mis tähistab seda kuupäeva Spreadsheet kuupäeva-kellaaja koodis.",
		arguments: [
			{
				name: "kuupäeva_tekst",
				description: "on tekst, mis tähistab kuupäeva Spreadsheet kuupäevavormingus, vahemikus 1.1.1900 (Windows) või 1.1.1904 (Macintosh) kuni 31.12.9999"
			}
		]
	},
	{
		name: "DAVERAGE",
		description: "Arvutab teie määratud tingimustele vastavate loendi või andmebaasi veerus olevate väärtuste keskmise.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veeru paigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DAY",
		description: "Tagastab kuupäeva, arvu vahemikus 1–31.",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv Spreadsheeti kasutatavas kuupäeva-kellaaja koodis"
			}
		]
	},
	{
		name: "DAYS",
		description: "Tagastab kahe kuupäeva vahele jääva päevade arvu.",
		arguments: [
			{
				name: "lõppkuupäev",
				description: "alguskuupäev ja lõppkuupäev on kaks kuupäeva, mille vahelist päevade arvu soovite leida"
			},
			{
				name: "alguskuupäev",
				description: "alguskuupäev ja lõppkuupäev on kaks kuupäeva, mille vahelist päevade arvu soovite leida"
			}
		]
	},
	{
		name: "DAYS360",
		description: "Tagastab 360-päevasele aastale (kaksteist 30-päevast kuud) toetudes päevade arvu kahe kuupäeva vahel.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "alguskuupäev ja lõppkuupäev on kaks kuupäeva, mille vahelist päevade arvu soovite teada saada"
			},
			{
				name: "lõppkuupäev",
				description: "alguskuupäev ja lõppkuupäev on kaks kuupäeva, mille vahelist päevade arvu soovite teada saada"
			},
			{
				name: "meetod",
				description: "on arvutusmeetodi määrav loogikaväärtus: USA (NASD) = FALSE või tühi; Euroopa = TRUE."
			}
		]
	},
	{
		name: "DB",
		description: "Tagastab vara amortisatsiooni määratud perioodi jooksul, kasutades degressiivset amortisatsioonimeetodit.",
		arguments: [
			{
				name: "maksumus",
				description: "on vara soetusmaksumus"
			},
			{
				name: "jääk",
				description: "on jääkväärtus vara kasutusea lõpus"
			},
			{
				name: "kestus",
				description: "on perioodide arv, mille jooksul vara amortiseerub (nn vara kasulik tööiga)"
			},
			{
				name: "periood",
				description: "on periood, mille kohta soovite amortisatsiooni arvutada. Periood peab kasutama samu ühikuid kui kestus"
			},
			{
				name: "kuude_arv",
				description: "on kuude arv esimesel aastal. Kui kuude arv on ära jäetud, on see vaikimisi 12"
			}
		]
	},
	{
		name: "DCOUNT",
		description: "Loendab lahtreid, mis sisaldavad andmebaasi kirjeväljal (veerus) arve, mis vastavad teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veerupaigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DCOUNTA",
		description: "Loendab andmebaasi kirjeteväljal (veerus) mittetühje lahtreid, mis vastavad teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veeru paigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DDB",
		description: "Tagastab vara amortisatsiooni määratud perioodi jooksul, kasutades topeltdegressiivset amortisatsioonimeetodit või mõnda muud teie määratud meetodit.",
		arguments: [
			{
				name: "maksumus",
				description: "on vara soetusmaksumus"
			},
			{
				name: "jääk",
				description: "on jääkväärtus vara kasutusea lõpus"
			},
			{
				name: "kestus",
				description: "on perioodide arv, mille jooksul vara amortiseerub (nn vara kasulik tööiga)"
			},
			{
				name: "periood",
				description: "on periood, mille kohta soovite amortisatsiooni arvutada. Perioodi ja kestuse puhul tuleb kasutada samu ühikuid"
			},
			{
				name: "tegur",
				description: "on kordaja, millega bilanss kahaneb. Kui tegur ära jäetakse, on see vaikimisi 2 (topeltdegressiivne amortisatsioonimeetod)"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Teisendab kümnendarvu kahendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kümnendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Teisendab kümnendarvu kuueteistkümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kümnendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Teisendab kümnendarvu kaheksandarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kümnendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Teisendab määratud arvusüsteemi alusel oleva arvu tekstkuju kümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mida soovite teisendada"
			},
			{
				name: "arvusüsteemi_alus",
				description: "on teisendatava arvu arvusüsteemi alus"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Teisendab radiaanid kraadideks.",
		arguments: [
			{
				name: "nurk",
				description: "on nurk radiaanides, mida soovite teisendada"
			}
		]
	},
	{
		name: "DELTA",
		description: "Kontrollib, kas kaks arvu on võrdsed.",
		arguments: [
			{
				name: "arv1",
				description: "on esimene arv"
			},
			{
				name: "arv2",
				description: "on teine arv"
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Keskväärtuse ja andmepunktide väärtuste hälvete ruutude summa.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on kuni 255 argumenti, massiiv või viide massiivile, mille andmete põhjal arvutatakse funktsiooni DEVSQ väärtus"
			},
			{
				name: "arv2",
				description: "on kuni 255 argumenti, massiiv või viide massiivile, mille andmete põhjal arvutatakse funktsiooni DEVSQ väärtus"
			}
		]
	},
	{
		name: "DGET",
		description: "Ekstraktib andmebaasist üksikkirje, mis vastab teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veeru paigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DISC",
		description: "Tagastab väärtpaberi diskontomäära.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "hind",
				description: "on väärtpaberi hind 100-eurose nominaalväärtuse kohta"
			},
			{
				name: "tagastushind",
				description: "on väärtpaberi tagastusväärtus 100-eurose nominaalväärtuse kohta"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "DMAX",
		description: "Tagastab andmebaasi kirjeväljal (veerus) suurima arvu, mis vastab teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veerusilt või arv, mis tähistab veerupaigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DMIN",
		description: "Tagastab andmebaasi kirjeväljal (veerus) vähima arvu, mis vastab teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veerusilt või arv, mis tähistab veerupaigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DOLLAR",
		description: "Teisendab arvu valuutavormingus tekstiks.",
		arguments: [
			{
				name: "arv",
				description: "on arv, viide mõnele arvu sisaldavale lahtrile või valem, mis annab väärtuseks mõne arvu"
			},
			{
				name: "kohtade_arv",
				description: "on komakohast paremale jäävate kohtade arv. Arv ümardatakse vastavalt vajadusele; kui see on tühi, siis kohtade_arv = 2"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Teisendab murruna väljendatud euro väärtuse kümnendarvuna väljendatud euro väärtuseks.",
		arguments: [
			{
				name: "murd_euro",
				description: "on murruna väljendatud arv"
			},
			{
				name: "murd",
				description: "on murru nimetajas kasutatav täisarv"
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Teisendab kümnendarvuna väljendatud euro väärtuse murruna väljendatud euro väärtuseks.",
		arguments: [
			{
				name: "kümnend_euro",
				description: "on kümnendarv"
			},
			{
				name: "murd",
				description: "on murru nimetajas kasutatav täisarv"
			}
		]
	},
	{
		name: "DPRODUCT",
		description: "Korrutab andmebaasi kirjeväljal (veerus) olevad väärtused, mis vastavad teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veeru paigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DSTDEV",
		description: "Esitab valimi standardhälbe, mille aluseks on valitud andmebaasikirjete valim.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veerusilt või arv, mis tähistab veerupaigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DSTDEVP",
		description: "Arvutab standardhälbe valitud andmebaasikirjete täispopulatsiooni põhjal.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veeru paigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DSUM",
		description: "Liidab andmebaasi kirjeväljal (veerus) olevad arvud, mis vastavad teie määratud tingimustele.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veerusilt või arv, mis tähistab veerupaigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DVAR",
		description: "Esitab valimi dispersiooni, mille aluseks on valitud andmebaasikirjete kogum.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veerusilt või arv, mis tähistab veerupaigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "DVARP",
		description: "Arvutab dispersiooni valitud andmebaasikirjete täispopulatsiooni põhjal.",
		arguments: [
			{
				name: "andmebaas",
				description: "on lahtrivahemik, mis moodustab loendi või andmebaasi. Andmebaas on seostuvate andmete loend"
			},
			{
				name: "väli",
				description: "on kas topeltjutumärkides veeru silt või arv, mis tähistab veeru paigutust loendis"
			},
			{
				name: "kriteeriumid",
				description: "on lahtrivahemik, mis sisaldab teie määratud tingimusi. Vahemik sisaldab ühe tingimusena veerusilti ja ühte sildi all asuvat lahtrit"
			}
		]
	},
	{
		name: "EDATE",
		description: "Tagastab alguskuupäevast näidatud kuude arvu võrra erineva kuupäeva järjenumbri.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "kuude_arv",
				description: "on kuude arv enne või pärast alguskuupäeva"
			}
		]
	},
	{
		name: "EFFECT",
		description: "Tagastab efektiivse aastaintressi määra.",
		arguments: [
			{
				name: "nominaalmäär",
				description: "on nominaalne intressimäär"
			},
			{
				name: "per_arv",
				description: "on makseperioodide arv aasta kohta"
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Tagastab URL-ina kodeeritud stringi.",
		arguments: [
			{
				name: "tekst",
				description: "on URL-ina kodeeritav string"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Tagastab argumendi kuupäeva kuust näidatud kuude arvu võrra hilisema või varasema kuu viimase päeva järjenumbri.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "kuude_arv",
				description: "on kuude arv enne või pärast alguskuupäeva"
			}
		]
	},
	{
		name: "ERF",
		description: "Tagastab veafunktsiooni.",
		arguments: [
			{
				name: "alampiir",
				description: "on funktsiooni ERF alumine integreerimisraja"
			},
			{
				name: "ülempiir",
				description: "on funktsiooni ERF ülemine integreerimisraja"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Tagastab veafunktsiooni.",
		arguments: [
			{
				name: "X",
				description: " on funktsiooni ERF.PRECISE integreerimise alampiir"
			}
		]
	},
	{
		name: "ERFC",
		description: "Tagastab komplementaarse veafunktsiooni.",
		arguments: [
			{
				name: "x",
				description: "on funktsiooni ERF alumine integreerimisraja"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Tagastab komplementaarse veafunktsiooni.",
		arguments: [
			{
				name: "X",
				description: " on funktsiooni ERFC.PRECISE integreerimise alampiir"
			}
		]
	},
	{
		name: "ERROR.TYPE",
		description: "Tagastab veaväärtusele vastava arvu.",
		arguments: [
			{
				name: "veaväärtus",
				description: "on veaväärtus, millele soovite saada identimisnumbrit. Võib olla tegelik veaväärtus või viide veaväärtust sisaldavale lahtrile"
			}
		]
	},
	{
		name: "EVEN",
		description: "Positiivse arvu ümardamine ülespoole ja negatiivse arvu ümardamine allapoole lähima paarisarvuni.",
		arguments: [
			{
				name: "arv",
				description: "on ümardatav väärtus"
			}
		]
	},
	{
		name: "EXACT",
		description: "Kontrollib, kas kaks tekstistringi on täpselt ühesugused ja tagastab väärtuse TRUE või FALSE. Funktsioon EXACT on tõstutundlik.",
		arguments: [
			{
				name: "tekst1",
				description: "on esimene tekstistring"
			},
			{
				name: "tekst2",
				description: "on teine tekstistring"
			}
		]
	},
	{
		name: "EXP",
		description: "Tagastab e, mis on tõstetud määratud astmesse.",
		arguments: [
			{
				name: "arv",
				description: "on e alusele rakendatud astendaja. Konstant e võrdub 2,71828182845904 ja see on naturaallogaritmi alus"
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Eksponentjaotus.",
		arguments: [
			{
				name: "x",
				description: "on funktsiooni argumendi väärtus (mittenegatiivne)"
			},
			{
				name: "lambda",
				description: "on parameetri väärtus (positiivne)"
			},
			{
				name: "kumulatiivsus",
				description: "on kumulatiivse jaotuse puhul TRUE; tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Eksponentjaotus.",
		arguments: [
			{
				name: "x",
				description: "on funktsiooni argumendi väärtus (mittenegatiivne)"
			},
			{
				name: "lambda",
				description: "on parameetri väärtus (positiivne)"
			},
			{
				name: "kumulatiivsus",
				description: "on kumulatiivse jaotuse puhul TRUE; tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "F.DIST",
		description: "F-tõenäosuse (vasakpoolse piiranguga) jaotuse väärtus kahe andmehulga dispersioonidel.",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millest funktsioon arvutatakse (mittenegatiivne arv)"
			},
			{
				name: "vabadusastmed1",
				description: "on lugeja vabadusastmete arv vahemikus 1 kuni 10^10 (viimane v.a)"
			},
			{
				name: "vabadusastmed2",
				description: "on nimetaja vabadusastmete arv vahemikus 1 kuni 10^10 (viimane v.a)"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "F-tõenäosuse (parempoolse piiranguga) jaotuse väärtus kahe andmehulga dispersioonidel.",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millest funktsioon arvutatakse (mittenegatiivne arv)"
			},
			{
				name: "vabadusastmed1",
				description: "on lugeja vabadusastmete arv vahemikus 1 kuni 10^10 (viimane v.a)"
			},
			{
				name: "vabadusastmed2",
				description: "on nimetaja vabadusastmete arv vahemikus 1 kuni 10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "F.INV",
		description: "F-tõenäosuse (vasakpoolse piiranguga) jaotuse pöördfunktsioon: kui p=F.DIST(x,...), siis F.INV(p,...)=x.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on F kumulatiivse jaotusega seotud tõenäosus lõigus 0–1 (k.a)"
			},
			{
				name: "vabadusastmed1",
				description: "on lugeja vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			},
			{
				name: "vabadusastmed2",
				description: "on nimetaja vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "F-tõenäosuse (parempoolse piiranguga) jaotuse pöördfunktsioon: kui p=F.DIST.RT(x,...), siis F.INV.RT(p,...)=x.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on F kumulatiivse jaotusega seotud tõenäosus lõigus 0–1 (k.a)"
			},
			{
				name: "vabadusastmed1",
				description: "on lugeja vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			},
			{
				name: "vabadusastmed2",
				description: "on nimetaja vabadusastmete arv lõigus 1–10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "F.TEST",
		description: "F-testi tulemus, kahepoolse piiranguga jaotuse tõenäosus, et kahe massiivi dispersioonid pole oluliselt erinevad.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene massiiv või andmevahemik, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvudele (tühje lahtreid ignoreeritakse)"
			},
			{
				name: "massiiv2",
				description: "on teine massiiv või andmevahemik, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvudele (tühje lahtreid ignoreeritakse)"
			}
		]
	},
	{
		name: "FACT",
		description: "Tagastab arvu faktoriaali, mis võrdub 1*2*3*...* arv.",
		arguments: [
			{
				name: "arv",
				description: "on mittenegatiivne arv, mille faktoriaali soovite leida"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Tagastab arvu topeltfaktoriaali.",
		arguments: [
			{
				name: "arv",
				description: "on väärtus, mille topeltfaktoriaali soovite arvutada"
			}
		]
	},
	{
		name: "FALSE",
		description: "Tagastab loogikaväärtuse FALSE.",
		arguments: [
		]
	},
	{
		name: "FDIST",
		description: "Kahe andmehulga F-tõenäosuse (parempoolse piiranguga) jaotusfunktsioon (dispersioon).",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millest funktsioon arvutatakse (mittenegatiivne arv)"
			},
			{
				name: "vabadusastmed1",
				description: "on lugeja vabadusastmete arv vahemikus 1 kuni 10^10 (viimane v.a)"
			},
			{
				name: "vabadusastmed2",
				description: "on nimetaja vabadusastmete arv vahemikus 1 kuni 10^10 (viimane v.a)"
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
		description: "Tagastab ühe tekstistringi alguspositsiooni mõne muu tekstistringi sees. Funktsioon FIND on tõstutundlik.",
		arguments: [
			{
				name: "otsitav_tekst",
				description: "on tekst, mida soovite otsida. Otsige teksti_seest esimest vastavat märki topeltjutumärkide (tühi tekst) abil; metamärgid pole lubatud"
			},
			{
				name: "teksti_seest",
				description: "on tekst, mis sisaldab teie otsitavat teksti"
			},
			{
				name: "algusnr",
				description: "määrab märgi, millest otsingut alustada. Esimene märk teksti_seest on märk nr 1. Kui ära jäetud, siis algusnr = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "F-tõenäosuse (parempoolse piiranguga) jaotuse pöördfunktsioon: kui p=FDIST(x,...), siis FINV(p,...)=x.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on F kumulatiivse jaotusega seotud tõenäosus lõigus 0 kuni 1"
			},
			{
				name: "vabadusastmed1",
				description: "on lugeja vabadusastmete arv lõigus 1 kuni 10^10 (viimane v.a)"
			},
			{
				name: "vabadusastmed2",
				description: "on nimetaja vabadusastmete arv lõigus 1 kuni 10^10 (viimane v.a)"
			}
		]
	},
	{
		name: "FISHER",
		description: "Fisheri teisendus.",
		arguments: [
			{
				name: "x",
				description: "on teisendatav väärtus vahemikus –1 kuni 1, (–1 ja 1 v.a)"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Fisheri pöördteisenduse väärtus: kui y=FISHER(x), siis FISHERINV(y)=x.",
		arguments: [
			{
				name: "y",
				description: "on väärtus, mille pöördteisenduse väärtust soovite leida"
			}
		]
	},
	{
		name: "FIXED",
		description: "Ümardab arvu määratud kohtadeni pärast koma ja tagastab tulemi tuhandeliste eraldajatega või eraldajateta tekstina.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mida soovite ümardada ja tekstiks teisendada"
			},
			{
				name: "kohtade_arv",
				description: "on komakohast paremale jäävate kohtade arv. Kui tühi, siis kohtade_arv = 2"
			},
			{
				name: "eraldajateta",
				description: "on loogikaväärtus: tagastatud tekstis tuhandeliste eraldajaid ei kuvata = TRUE; tagastatud tekstis kuvatakse tuhandeliste eraldajad = FALSE või tühi"
			}
		]
	},
	{
		name: "FLOOR",
		description: "Ümardab arvu allapoole, lähima kordse korduseni.",
		arguments: [
			{
				name: "arv",
				description: "on ümardatav arvuline väärtus"
			},
			{
				name: "ümardusalus",
				description: "on kordaja, milleni ümardatakse. Arv ja ümardusalus peavad mõlemad olema positiivsed või mõlemad negatiivsed"
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Ümardab arvu allapoole lähima täisarvuni või ümardusaluse lähima kordseni.",
		arguments: [
			{
				name: "arv",
				description: "on väärtus, mida soovite ümardada"
			},
			{
				name: "ümardusalus",
				description: "on kordne, milleni soovite ümardada"
			},
			{
				name: "režiim",
				description: "kui see argument on sisestatud ja pole null, ümardab see funktsioon nullile lähemale"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Ümardab arvu allapoole lähima täisarvuni või ümardusaluse kordseni.",
		arguments: [
			{
				name: "arv",
				description: "on ümardatav arvuline väärtus"
			},
			{
				name: "ümardusalus",
				description: "on kordaja, milleni ümardatakse. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Eeldatavad tulevikuväärtused arvestades jätkuvat lineaarset trendi.",
		arguments: [
			{
				name: "x",
				description: "arvväärtus, mille suhtes leitakse eeldatavat väärtust"
			},
			{
				name: "teada_y_väärtused",
				description: "on sõltuvate väärtuste massiiv"
			},
			{
				name: "teada_x_väärtused",
				description: "on sõltumatute väärtuste massiiv. Teada_x_väärtused dispersioon ei tohi olla null"
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Tagastab valemi stringina.",
		arguments: [
			{
				name: "viide",
				description: "on viide valemile"
			}
		]
	},
	{
		name: "FREQUENCY",
		description: "Arvutab, kui sageli väärtused mõnes väärtustevahemikus ette tulevad ja tagastab seejärel vertikaalse arvumassiivi, milles on üks element rohkem kui salvemassiivis.",
		arguments: [
			{
				name: "andmemassiiv",
				description: "on väärtustekogumi massiiv või viide väärtustekogumile, mille kohta soovite sagedusi loendada (tühje ja teksti ignoreeritakse)"
			},
			{
				name: "salvemassiiv",
				description: "on intervallide massiiv või viide intervallidele, milleks soovite väärtused andmemassiivis rühmitada"
			}
		]
	},
	{
		name: "FTEST",
		description: "F-testi tulemus, kahepoolse piiranguga jaotuse tõenäosus, et kahe massiivi dispersioonid pole oluliselt erinevad.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene andmemassiiv, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvudele (tühje lahtreid ignoreeritakse)"
			},
			{
				name: "massiiv2",
				description: "on esimene andmemassiiv, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvudele (tühje lahtreid ignoreeritakse)"
			}
		]
	},
	{
		name: "FV",
		description: "Tagastab kindlatel perioodilistel maksetel ja püsival intressimääral põhineva investeeringu tulevase väärtuse.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "per_arv",
				description: "on investeeringu makseperioodide koguarv"
			},
			{
				name: "makse",
				description: "on igas perioodis sooritatud makse; see ei tohi investeeringu kestuse jooksul muutuda"
			},
			{
				name: "praegune_väärt",
				description: "on praegune väärtus ehk paušaalsumma, mis tulevaste maksete sari on praegu väärt. Kui on tühi, praegune_väärt = 0"
			},
			{
				name: "tüüp",
				description: "on makse ajastust tähistav väärtus: makse perioodi alguses = 1; makse perioodi lõpus = 0 või on tühi"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Tagastab algse põhisumma tulevase väärtuse pärast mitme liitintressimäära rakendamist.",
		arguments: [
			{
				name: "põhisumma",
				description: "on praegune väärtus"
			},
			{
				name: "graafik",
				description: "on rakendatavate intressimäärade massiiv"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Tagastab gammafunktsiooni väärtuse.",
		arguments: [
			{
				name: "x",
				description: "on väärtus, mille gammaväärtust soovite arvutada"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Gammajaotus.",
		arguments: [
			{
				name: "x",
				description: "mittenegatiivne väärtus, millest arvutatakse jaotus"
			},
			{
				name: "alfa",
				description: "jaotusparameeter, positiivne arv"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, positiivne arv. Kui beeta=1, siis GAMMA.DIST väärtus on standardne gammajaotus"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotuse puhul TRUE; tõenäosusmassi funktsiooni puhul FALSE või jäetaks ära"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Kumulatiivse gammajaotuse pöördfunktsioon: kui p=GAMMA.DIST(x,...), siis GAMMA.INV(p,...)=x.",
		arguments: [
			{
				name: "tõenäosus",
				description: "gammajaotusega seotud tõenäosus lõigus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "alfa",
				description: "on jaotusparameeter, positiivne arv"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, positiivne arv. Kui beeta=1, siis GAMMA.INV väärtus on standardse gammajaotuse pöördväärtus"
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Gammajaotus.",
		arguments: [
			{
				name: "x",
				description: "mittenegatiivne väärtus, millest arvutatakse jaotus"
			},
			{
				name: "alfa",
				description: "jaotusparameeter, positiivne arv"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, positiivne arv. Kui beeta=1, siis GAMMADIST väärtus on standardne gammajaotus"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotuse puhul TRUE; vaikimisi või tõenäosusmassi funktsiooni pöördväärtuse puhul FALSE"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Kumulatiivse gammajaotuse pöördväärtus: kui p=GAMMADIST(x,...), siis GAMMAINV(p,...)=x.",
		arguments: [
			{
				name: "tõenäosus",
				description: "gammajaotusega seotud tõenäosus lõigus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "alfa",
				description: "on jaotusparameeter, positiivne arv"
			},
			{
				name: "beeta",
				description: "on jaotusparameeter, positiivne arv. Kui beeta=1, siis GAMMAINV väärtus on standardse gammajaotuse pöördväärtus"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Gammafunktsiooni naturaallogaritm.",
		arguments: [
			{
				name: "x",
				description: "on positiivne väärtus, millele arvutatakse GAMMALN"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Tagastab gammafunktsiooni naturaallogaritmi.",
		arguments: [
			{
				name: "x",
				description: " on väärtus, mille väärtust GAMMALN.PRECISE soovite arvutada (positiivne arv)"
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
		description: "Tagastab suurima ühisjagaja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on väärtused vahemikus 1–255"
			},
			{
				name: "arv2",
				description: "on väärtused vahemikus 1–255"
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Positiivsete arvude geomeetriline keskmine.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "kuni 255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse geomeetriline keskmine"
			},
			{
				name: "arv2",
				description: "kuni 255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse geomeetriline keskmine"
			}
		]
	},
	{
		name: "GESTEP",
		description: "Kontrollib, kas arv ületab läve.",
		arguments: [
			{
				name: "arv",
				description: "on väärtus, mida lävega võrreldakse"
			},
			{
				name: "samm",
				description: "on väärtus, mis määratleb läve"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Ekstraktib PivotTable-liigendtabelis talletatud andmed.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "andmeväli",
				description: "on selle andmevälja nimi, mille andmeid soovite ekstraktida"
			},
			{
				name: "PivotTable_liigendtabel",
				description: "on viide PivotTable-liigendtabeli lahtrile või lahtrivahemikule mis sisaldab vastuseks saadavaid andmeid"
			},
			{
				name: "väli",
				description: "viidatav väli"
			},
			{
				name: "üksus",
				description: "viidatav väljaüksus"
			}
		]
	},
	{
		name: "GROWTH",
		description: "Tagastab arvud teadaolevaile andmepunktidele vastavas eksponentsiaalses kasvutrendis.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on y-väärtuste hulk, mida te seoses y = b*m^x juba teate, positiivsete arvude massiiv või vahemik"
			},
			{
				name: "teada_x_väärtused",
				description: "on valikuline x-väärtuste hulk, mida võite seoses y = b*m^x juba teada, massiiv või vahemik, mis on niisama suur kui teada_y_väärtused"
			},
			{
				name: "uued_x_väärtused",
				description: "on uued x-väärtused, mille kohta soovite, et funktsioon GROWTH tagastaks vastavad y-väärtused"
			},
			{
				name: "konstant",
				description: "on loogikaväärtus: konstant b arvutatakse tavaliselt juhul, kui konstant = TRUE; b võrdub 1, kui konstant = FALSE või on tühi"
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Positiivsete arvude harmooniline keskmine: pöördarvude aritmeetilise keskmise pöördarv.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse harmooniline keskmine"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse harmooniline keskmine"
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Teisendab kuueteistkümnendarvu kahendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kuueteistkümnendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Teisendab kuueteistkümnendarvu kümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kuueteistkümnendarv"
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Teisendab kuueteistkümnendarvu kaheksandarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kuueteistkümnendarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "HLOOKUP",
		description: "Otsib väärtust mõne tabeli või väärtustemassiivi ülemisest reast ja tagastab väärtuse teie määratud reaga samas veerus.",
		arguments: [
			{
				name: "otsitav_väärtus",
				description: "on väärtus, mida tabeli esimesest reast otsida; see võib olla väärtus, viide või tekstistring"
			},
			{
				name: "massiiv",
				description: "on tekstist, arvudest või loogikaväärtustest koosnev tabel, millest andmeid otsitakse. Massiiv võib olla viide mõnele vahemikule või vahemiku nimele"
			},
			{
				name: "rea_indeks",
				description: "on massiivi reanumber, millest tuleks vastav väärtus tagastada. Tabeli esimene väärtusterida on rida 1"
			},
			{
				name: "vastendustüüp",
				description: "on loogikaväärtus: lähima vaste otsimine ülemisest reast (sorditud tõusvas järjestuses) = TRUE või jäetakse välja; täpse vaste otsimine = FALSE"
			}
		]
	},
	{
		name: "HOUR",
		description: "Tagastab tunnid arvuna 0 (12:00 EL) kuni 23 (11:00 PL).",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv Spreadsheeti kasutatavas kuupäeva-kellaaja koodis või tekst ajavormingus, nt 16:48:00 või 4:48:00 PL"
			}
		]
	},
	{
		name: "HYPERLINK",
		description: "Loob otsetee või hüppe, mis avab teie kõvakettale, võrguserverisse või Internetti salvestatud dokumendi.",
		arguments: [
			{
				name: "lingi_asukoht",
				description: "on tekst, mis annab avatava dokumendi tee ja failinime: asukoht kõvakettal, UNC-aadress või URL-tee"
			},
			{
				name: "sõbralik_nimi",
				description: "on tekst või arv, mis kuvatakse lahtris. Kui ära jäetud, kuvatakse lahtris lingi_asukoha tekst"
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Hüpergeomeetriline jaotus.",
		arguments: [
			{
				name: "edukaid_valimis",
				description: "on teatud omadustega elementide arv valimis"
			},
			{
				name: "valimi_suurus",
				description: "on valimi elementide koguarv"
			},
			{
				name: "edukaid_popul",
				description: "on teatud omadusega elementide arv populatsioonis"
			},
			{
				name: "popul_suurus",
				description: "on populatsiooni elementide koguarv"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Hüpergeomeetriline jaotus.",
		arguments: [
			{
				name: "edukaid_valimis",
				description: "on teatud omadustega elementide arv valimis"
			},
			{
				name: "valimi_suurus",
				description: "on valimi elementide koguarv"
			},
			{
				name: "edukaid_popul",
				description: "on teatud omadusega elementide arv populatsioonis"
			},
			{
				name: "popul_suurus",
				description: "on populatsiooni elementide koguarv"
			}
		]
	},
	{
		name: "IF",
		description: "Kontrollib, kas tingimused on täidetud ning tagastab ühe väärtuse, kui tingimus on TRUE, ja teise, kui tingimus on FALSE.",
		arguments: [
			{
				name: "loogilisuse_test",
				description: " on suvaline väärtus või avaldis, mis võib tulemiks anda väärtuse TRUE või FALSE"
			},
			{
				name: "väärtus_kui_tõene",
				description: "on väärtus, mis tagastatakse, kui loogilisuse_test on TRUE. Kui see on tühi, tagastatakse väärtus TRUE. Saate pesastada kuni seitse IF-funktsiooni"
			},
			{
				name: "väärtus_kui_väär",
				description: "on väärtus, mis tagastatakse, kui loogilisuse_test on FALSE. Kui see on tühi, tagastatakse väärtus FALSE"
			}
		]
	},
	{
		name: "IFERROR",
		description: "Tagastab argumendi väärtus_vea_korral, kui avaldis on viga ja avaldise väärtus mitte.",
		arguments: [
			{
				name: "väärtus",
				description: "on suvaline väärtus, avaldis või viide"
			},
			{
				name: "väärtus_vea_korral",
				description: "on suvaline väärtus, avaldis või viide"
			}
		]
	},
	{
		name: "IFNA",
		description: "Tagastab teie määratud väärtuse, kui avaldis annab tulemiks #N/A, muul juhul tagastab avaldise tulemi.",
		arguments: [
			{
				name: "väärtus",
				description: "on mis tahes väärtus, avaldis või viide"
			},
			{
				name: "väärtus_kui_na",
				description: "on mis tahes väärtus, avaldis või viide"
			}
		]
	},
	{
		name: "IMABS",
		description: "Tagastab kompleksarvu absoluutväärtuse (mooduli).",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille absoluutväärtust soovite leida"
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Tagastab kompleksarvu imaginaarosa.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille imaginaarosa soovite leida"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Tagastab argumendi q, radiaanides väljendatud nurga.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille argumenti soovite leida"
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Tagastab kompleksarvu kaaskompleksarvu.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille kaaskompleksarvu soovite leida"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Tagastab kompleksarvu koosinuse.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille koosinust soovite leida"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Tagastab kompleksarvu hüperboolse koosinuse.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille hüperboolset koosinust soovite leida"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Tagastab kompleksarvu kootangensi.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille kootangensit soovite leida"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Tagastab kompleksarvu koosekansi.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille koosekansit soovite leida"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Tagastab kompleksarvu hüperboolse koosekansi.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille hüperboolset koosekansit soovite leida"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Tagastab kahe kompleksarvu jagatise.",
		arguments: [
			{
				name: "iarv1",
				description: "on kompleksarvuline lugeja ehk jagatav"
			},
			{
				name: "iarv2",
				description: "on kompleksarvuline nimetaja ehk jagaja"
			}
		]
	},
	{
		name: "IMEXP",
		description: "Tagastab kompleksarvu eksponendi.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille eksponenti soovite leida"
			}
		]
	},
	{
		name: "IMLN",
		description: "Tagastab kompleksarvu naturaallogaritmi.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille naturaallogaritmi soovite leida"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Tagastab kompleksarvu kümnendlogaritmi.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille kümnendlogaritmi soovite leida"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Tagastab kompleksarvu kahendlogaritmi.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille logaritmi alusel 2 soovite leida"
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Tagastab kompleksarvu täisarvulise astendajaga astme.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mida soovite astendada"
			},
			{
				name: "arv",
				description: "on kompleksarvu astendaja"
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Tagastab 1–255 kompleksarvu korrutise.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "iarv1",
				description: "Iarv1, Iarv2,... on 1–255 korrutatavat kompleksarvu."
			},
			{
				name: "iarv2",
				description: "Iarv1, Iarv2,... on 1–255 korrutatavat kompleksarvu."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Tagastab kompleksarvu reaalosa.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille reaalosa soovite leida"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Tagastab kompleksarvu seekansi.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille seekansit soovite leida"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Tagastab kompleksarvu hüperboolse seekansi.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille hüperboolset seekansit soovite leida"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Tagastab kompleksarvu siinuse.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille siinust soovite leida"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Tagastab kompleksarvu hüperboolse siinuse.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille hüperboolset siinust soovite leida"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Tagastab ruutjuure kompleksarvust.",
		arguments: [
			{
				name: "iarv",
				description: "on kompleksarv, mille ruutjuurt soovite leida"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Tagastab kahe kompleksarvu vahe.",
		arguments: [
			{
				name: "iarv1",
				description: "on kompleksarv, millest lahutatakse iarv2"
			},
			{
				name: "iarv2",
				description: "on kompleksarv, mis lahutatakse kompleksarvust iarv1"
			}
		]
	},
	{
		name: "IMSUM",
		description: "Tagastab kompleksarvude summa.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "iarv1",
				description: "on 1–255 liidetavat kompleksarvu"
			},
			{
				name: "iarv2",
				description: "on 1–255 liidetavat kompleksarvu"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Tagastab kompleksarvu tangensi.",
		arguments: [
			{
				name: "arv",
				description: "on kompleksarv, mille tangensit soovite leida"
			}
		]
	},
	{
		name: "INDEX",
		description: "Tagastab määratud vahemikus teatud rea ja veeru ristumiskohas oleva lahtri väärtuse või viite.",
		arguments: [
			{
				name: "massiiv",
				description: "on lahtrivahemik või massiivikonstant."
			},
			{
				name: "rea_nr",
				description: "valib massiivis või viites rea, millest väärtust tagastada. Kui tühi, on veeru_nr kohustuslik"
			},
			{
				name: "veeru_nr",
				description: "valib massiivis või viites veeru, millest väärtust tagastada. Kui tühi, on rea_nr kohustuslik"
			}
		]
	},
	{
		name: "INDIRECT",
		description: "Tagastab mõne tekstistringi määratud viite.",
		arguments: [
			{
				name: "viite_tekst",
				description: "on viide lahtrile, mis sisaldab mõnda viidet laadis A1 või R1C1, viitena määratletud nime või viidet lahtrile, kui tekstistringile"
			},
			{
				name: "a1",
				description: "on loogikaväärtus, mis määrab viite tüübi viite_tekstis: R1C1-laad = FALSE; A1-laad = TRUE või jäetakse ära"
			}
		]
	},
	{
		name: "INFO",
		description: "Tagastab teabe praeguse töökeskkonna kohta.",
		arguments: [
			{
				name: "tüübi_tekst",
				description: "on tekst, mis määrab, millist tüüpi teavet soovite vastuseks saada."
			}
		]
	},
	{
		name: "INT",
		description: "Ümardab arvu allapoole lähima täisarvuni.",
		arguments: [
			{
				name: "arv",
				description: "on reaalarv, mida soovite allapoole täisarvuks ümardada"
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Punkt, milles x- ja y-väärtuste alusel arvutatud sobivaim regressioonijoon läbib y-telge.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on sõltuvate väärtuste massiiv"
			},
			{
				name: "teada_x_väärtused",
				description: "on sõltumatute väärtuste massiiv. Teada_x_väärtused dispersioon ei tohi olla null"
			}
		]
	},
	{
		name: "INTRATE",
		description: "Tagastab täielikult investeeritud väärtpaberite intressimäära.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "investeering",
				description: "on väärtpaberitesse investeeritud summa"
			},
			{
				name: "tagastushind",
				description: "on väärtpaberi tagastusväärtus"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "IPMT",
		description: "Tagastab kindlatel perioodilistel maksetel ja püsival intressimääral põhineva investeeringu intressimakse antud perioodi jaoks.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "periood",
				description: "on periood, mille kohta soovite intressi arvutada, see peab jääma vahemikku 1 kuni Per_arv"
			},
			{
				name: "per_arv",
				description: "on investeeringu makseperioodide koguarv"
			},
			{
				name: "praegune_väärt",
				description: "on praegune väärtus ehk paušaalsumma, mis tulevaste maksete sari on praegu väärt"
			},
			{
				name: "tul_väärt",
				description: "on tulevane väärtus ehk sularahasaldo, mida soovite pärast viimase makse sooritamist saavutada. Kui ära jäetud, siis Tul_väärt = 0"
			},
			{
				name: "tüüp",
				description: "on loogikaväärtus, mis tähistab makse ajastust: perioodi lõpus = 0 või jäetakse ära, perioodi alguses = 1"
			}
		]
	},
	{
		name: "IRR",
		description: "Tagastab rahavoo sisemise tasuvusmäära.",
		arguments: [
			{
				name: "väärtused",
				description: "on massiiv või viide lahtritele, mis sisaldavad arve, mille kohta soovite sisemise tasuvusmäära arvutada"
			},
			{
				name: "hinnang",
				description: "on arv, mis võib teie hinnangul olla sisemise tasuvusmäära tulemile lähedane; 0,1 (10 protsenti), kui tühi"
			}
		]
	},
	{
		name: "ISBLANK",
		description: "Kontrollib, kas viide on tühjale lahtrile ja annab vastuseks TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on lahter või selle lahtri nimi, mida soovite testida"
			}
		]
	},
	{
		name: "ISERR",
		description: "Kontrollib, kas väärtus on viga (#VALUE!, #REF!, #DIV/0!, #NUM!, #NAME?, or #NULL!, v.a #N/A) ja tagastab väärtuse TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata mõnele lahtrile, valemile või nimele, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISERROR",
		description: "Kontrollib, kas väärtus on viga (#N/A, #VALUE!, #REF!, #DIV/0!, #NUM!, #NAME? või #NULL!) ja tagastab väärtuse TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata mõnele lahtrile, valemile või nimele, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Tagastab väärtuse TRUE, kui arv on paaris.",
		arguments: [
			{
				name: "arv",
				description: "on kontrollitav väärtus"
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Kontrollib, kas viide osutab valemit sisaldavale lahtrile, ja tagastab siis väärtuse TÕENE või VÄÄR.",
		arguments: [
			{
				name: "viide",
				description: "on viide lahtrile, mida soovite testida. Viide võib olla lahtriviide, valem või lahtrile viitav nimi"
			}
		]
	},
	{
		name: "ISLOGICAL",
		description: "Kontrollib, kas väärtus on loogikaväärtus (TRUE või FALSE) ja annab vastuseks TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata mõnele lahtrile, valemile või nimele, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISNA",
		description: "Kontrollib, kas väärtus on #N/A ja tagastab väärtuse TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata lahtrile, valemile või nimele, mis viitab lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISNONTEXT",
		description: "Kontrollib, kas väärtus pole tekst (tühjad lahtrid pole tekst) ja annab vastuseks TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida: lahter, valem või nimi, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISNUMBER",
		description: "Kontrollib, kas väärtus on arv ja annab vastuseks TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata mõnele lahtrile, valemile või nimele, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Ümardab arvu absoluutväärtuselt ülespoole ümardusaluse lähima kordseni või vastavalt määratud täpsusele.",
		arguments: [
			{
				name: "arv",
				description: "on ümardatav arv"
			},
			{
				name: "ümardusalus",
				description: "on kordaja, milleni ümardatakse"
			}
		]
	},
	{
		name: "ISODD",
		description: "Tagastab väärtuse TRUE, kui arv on paaritu.",
		arguments: [
			{
				name: "arv",
				description: "on kontrollitav väärtus"
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Tagastab sisestatud kuupäevale vastava aasta ISO nädalanumbri.",
		arguments: [
			{
				name: "kuupäev",
				description: "on kuupäeva ja kellaaja kood, mida Spreadsheet kasutab kuupäeva ja kellaaja arvutamiseks"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Annab vastuseks kindla perioodi jooksul makstud investeeringuintressid.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "periood",
				description: "on periood, mille kohta soovite intresse teada saada"
			},
			{
				name: "per_arv",
				description: "on makseperioodide arv investeeringu jooksul"
			},
			{
				name: "praegune_väärt",
				description: "on paušaalsumma, mis tulevaste maksete sari on praegu väärt"
			}
		]
	},
	{
		name: "ISREF",
		description: "Kontrollib, kas väärtus on viide, ja annab vastuseks TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata mõnele lahtrile, valemile või nimele, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "ISTEXT",
		description: "Kontrollib, kas väärtus on tekst ja annab vastuseks TRUE või FALSE.",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite testida. Väärtus võib viidata mõnele lahtrile, valemile või nimele, mis viitab mõnele lahtrile, valemile või väärtusele"
			}
		]
	},
	{
		name: "KURT",
		description: "Ekstsess.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse ekstsess"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse ekstsess"
			}
		]
	},
	{
		name: "LARGE",
		description: "Suuruselt k-s väärtus andmehulgas. Näiteks suuruselt viies väärtus.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, millest leitakse suuruselt k-s väärtus"
			},
			{
				name: "k",
				description: "on väärtuse positsioon (kahanevas järjestuses) massiivis või lahtrivahemikus"
			}
		]
	},
	{
		name: "LCM",
		description: "Tagastab vähima ühiskordse.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 väärtust, mille vähimat ühiskordset soovite leida"
			},
			{
				name: "arv2",
				description: "on 1–255 väärtust, mille vähimat ühiskordset soovite leida"
			}
		]
	},
	{
		name: "LEFT",
		description: "Tagastab tekstistringi algusest alates määratud arvu märke.",
		arguments: [
			{
				name: "tekst",
				description: "on tekstiüksus, mis sisaldab ekstraktitavaid märke"
			},
			{
				name: "märkide_arv",
				description: "määrab, mitu märki peaks funktsioon LEFT ekstraktima; 1, kui ära jäetud"
			}
		]
	},
	{
		name: "LEN",
		description: "Tagastab märkide arvu tekstistringis.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst, mille pikkust soovite teada saada. Tühikuid arvestatakse märkidena"
			}
		]
	},
	{
		name: "LINEST",
		description: "Tagastab statistikud, mis kirjeldavad teadaolevaile andmepunktidele vastavat lineaartrendi, sobitades sirgjoont vähimruutude meetodi abil.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on y-väärtuste kogum, mida seoses y = mx + b juba teate"
			},
			{
				name: "teada_x_väärtused",
				description: "on valikuline x-väärtuste kogum, mida võite seoses y = mx + b juba teada"
			},
			{
				name: "konstant",
				description: "on loogikaväärtus: konstant b arvutatakse tavaliselt juhul, kui konstant = TRUE või on tühi; b võrdub 0, kui konstant = FALSE"
			},
			{
				name: "statistikud",
				description: "on loogikaväärtus: täiendavate regressioonistatistikute tagastus = TRUE; m-kordajate ja konstandi b tagastus = FALSE või tühi"
			}
		]
	},
	{
		name: "LN",
		description: "Tagastab arvu naturaallogaritmi.",
		arguments: [
			{
				name: "arv",
				description: "on positiivne reaalarv, mille naturaallogaritmi soovite leida"
			}
		]
	},
	{
		name: "LOG",
		description: "Tagastab arvu logaritmi määratud alusega.",
		arguments: [
			{
				name: "arv",
				description: "on positiivne reaalarv, mille logaritmi soovite leida"
			},
			{
				name: "alus",
				description: "on logaritmi alus; 10, kui ära jäetud"
			}
		]
	},
	{
		name: "LOG10",
		description: "Tagastab arvu kümnendlogaritmi.",
		arguments: [
			{
				name: "arv",
				description: "on positiivne reaalarv, mille kümnendlogaritmi soovite leida"
			}
		]
	},
	{
		name: "LOGEST",
		description: "Tagastab statistikud, mis kirjeldavad teadaolevaile andmepunktidele vastavat eksponentkõverat.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on y-väärtuste hulk, mida te seoses y = b*m^x juba teate"
			},
			{
				name: "teada_x_väärtused",
				description: "on valikuline x-väärtuste hulk, mida võite seoses y = b*m^x juba teada"
			},
			{
				name: "konstant",
				description: "on loogikaväärtus: konstant b arvutatakse tavaliselt juhul, kui konstant = TRUE või on tühi; b võrdub 1, kui konstant = FALSE"
			},
			{
				name: "statistikud",
				description: "on loogikaväärtus: täiendavate regressioonistatistikute tagastus = TRUE; m-kordajate ja konstandi b tagastus = FALSE või tühi"
			}
		]
	},
	{
		name: "LOGINV",
		description: "x-i lognormaaljaotuse jaotusfunktsiooni pöördfunktsiooni väärtus, kus ln(x) on parameetrite keskväärtus ja standardhälve normaaljaotus.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on lognormaaljaotusega seotud tõenäosus lõigus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "keskväärtus",
				description: "ln(x) keskväärtus"
			},
			{
				name: "standardhälve",
				description: "on ln(x) standardhälve, peab olema positiivne"
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "x-i logaritmiline normaaljaotus, kus ln(x) on parameetrite Keskväärtus ja Standardhälve normaaljaotus.",
		arguments: [
			{
				name: "x",
				description: "on positiivne arv, millest arvutatakse funktsiooni väärtus"
			},
			{
				name: "keskväärtus",
				description: "on ln(x) keskväärtus"
			},
			{
				name: "standardhälve",
				description: "ln(x) standardhälve, peab olema positiivne"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "x-i lognormaaljaotuse jaotusfunktsiooni pöördfunktsioon, kus ln(x) on parameetrite keskväärtus ja standardhälve normaaljaotus.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on lognormaaljaotusega seotud tõenäosus lõigus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "keskväärtus",
				description: "ln(x) keskväärtus"
			},
			{
				name: "standardhälve",
				description: "on ln(x) standardhälve, peab olema positiivne"
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "x-i kumulatiivne logaritmiline normaaljaotus, kus ln(x) on parameetrite Keskväärtus ja Standardhälve normaaljaotus.",
		arguments: [
			{
				name: "x",
				description: "on positiivne arv, millest arvutatakse funktsiooni väärtus"
			},
			{
				name: "keskväärtus",
				description: "on ln(x) keskväärtus"
			},
			{
				name: "standardhälve",
				description: "ln(x) standardhälve, peab olema positiivne"
			}
		]
	},
	{
		name: "LOOKUP",
		description: "Vaatab väärtuse järele, lähtudes kas üherealisest või üheveerulisest vahemikust või massiivist. On tagasiühilduv.",
		arguments: [
			{
				name: "otsitav_väärtus",
				description: "on väärtus, mida LOOKUP otsib kui lähtevektorit ja mis võib olla arv, tekst, loogikaväärtus või mõne väärtuse nimi või viide"
			},
			{
				name: "lähtevektor",
				description: "on vahemik, mis sisaldab ainult ühte rida või veergu teksti, arve või loogikaväärtusi, mis on paigutatud tõusvas järjestuses"
			},
			{
				name: "tulemivektor",
				description: "on vahemik, mis sisaldab ainult ühte rida või veergu, mis on niisama suur kui lähtevektor"
			}
		]
	},
	{
		name: "LOWER",
		description: "Teisendab kõik tähed tekstistringis väiketähtedeks.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst, mida soovite väiketähtedeks teisendada. Märke tekstis, mis pole tähed, ei muudeta"
			}
		]
	},
	{
		name: "MATCH",
		description: "Tagastab sellise üksuse suhtelise paigutuse massiivis, mis vastab määratud väärtusele määratud järjestuses.",
		arguments: [
			{
				name: "otsitav_väärtus",
				description: "on väärtus, mille abil saate massiivis otsida soovitud väärtust: arvu, teksti või loogikaväärtust või viidet ühele neist"
			},
			{
				name: "massiiv",
				description: "on pidev lahtrivahemik, mis sisaldab võimalikke otsinguväärtusi, väärtuste massiivi või viidet mõnele massiivile"
			},
			{
				name: "vastendustüüp",
				description: "on arv 1, 0 või -1, mis viitab, millist väärtust tagastada."
			}
		]
	},
	{
		name: "MAX",
		description: "Tagastab väärtustekogumi vähima väärtuse. Ignoreerib loogikaväärtusi ja teksti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite maksimumi"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite maksimumi"
			}
		]
	},
	{
		name: "MAXA",
		description: "Tagastab väärtustekogumi suurima väärtuse. Ei ignoreeri loogikaväärtusi ja teksti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite maksimumi"
			},
			{
				name: "väärtus2",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite maksimumi"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Tagastab massiivi maatriksdeterminandi.",
		arguments: [
			{
				name: "massiiv",
				description: "on arvuline massiiv, milles on võrdne arv ridu ja veerge, kas lahtrivahemik või massiivikonstant"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Tagastab mediaani ehk järjestatud arvukogumi keskmise arvu.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu või nime, massiivi või viidet, mis sisaldavad arve, mille mediaani soovite teada saada"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu või nime, massiivi või viidet, mis sisaldavad arve, mille mediaani soovite teada saada"
			}
		]
	},
	{
		name: "MID",
		description: "Tagastab märgid tekstistringi keskelt, kui on antud alguskoht ja pikkus.",
		arguments: [
			{
				name: "tekst",
				description: "on tekstiüksus, millest soovite märke ekstraktida"
			},
			{
				name: "algusnr",
				description: "on esimese märgi asukoht, mida soovite ekstraktida. Esimene märk tekstis on 1"
			},
			{
				name: "märkide_arv",
				description: "määratleb, mitu märki tekstist tagastada"
			}
		]
	},
	{
		name: "MIN",
		description: "Tagastab väärtustekogumi vähima väärtuse. Ignoreerib loogikaväärtusi ja teksti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite miinimumi"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite miinimumi"
			}
		]
	},
	{
		name: "MINA",
		description: "Tagastab väärtustekogumi vähima väärtuse. Ei ignoreeri loogikaväärtusi ja teksti.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite miinimumi"
			},
			{
				name: "väärtus2",
				description: "on 1–255 arvu, tühja lahtrit, loogikaväärtust või tekstilist arvu, mille kohta soovite miinimumi"
			}
		]
	},
	{
		name: "MINUTE",
		description: "Tagastab minutid arvuna vahemikus 0–59.",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv Spreadsheeti kasutatavas kuupäeva-kellaaja koodis või tekst ajavormingus, nt 16:48:00 või 4:48:00 PL"
			}
		]
	},
	{
		name: "MINVERSE",
		description: "Tagastab massiivis oleva maatriksi pöördmaatriksi.",
		arguments: [
			{
				name: "massiiv",
				description: "on arvuline massiiv, milles on võrdne arv ridu ja veerge, kas lahtrivahemik või massiivikonstant"
			}
		]
	},
	{
		name: "MIRR",
		description: "Tagastab perioodiliste rahavoogude sisemise tasuvusmäära, võttes arvesse nii investeeringu maksumust kui ka raha taasinvesteerimise intressi.",
		arguments: [
			{
				name: "väärtused",
				description: "on massiiv või viide lahtritele, mis sisaldavad regulaarsete perioodide peale jaotatud maksete (negatiivsed) ja sissetulekute (positiivsed) sarja tähistavaid arve"
			},
			{
				name: "finants_määr",
				description: "on intressimäär, mida tasute rahavoogudes kasutatud raha pealt"
			},
			{
				name: "taasinvest_määr",
				description: "on intressimäär, mille saate rahavoogude pealt need taasinvesteerides"
			}
		]
	},
	{
		name: "MMULT",
		description: "Tagastab kahe massiivi maatrikskorrutise, massiivi, milles on sama palju ridu kui argumendis massiiv1 ja sama palju veerge kui argumendis massiiv2.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene arvumassiiv, mida korrutada ja selles peab olema sama palju veerge kui argumendis massiiv2 on ridu"
			},
			{
				name: "massiiv2",
				description: "on esimene arvumassiiv, mida korrutada ja selles peab olema sama palju veerge kui argumendis massiiv2 on ridu"
			}
		]
	},
	{
		name: "MOD",
		description: "Tagastab jäägi, mis jääb pärast arvu jagamist jagajaga.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mille jääki soovite pärast jagamist leida"
			},
			{
				name: "jagaja",
				description: "on arv, millega soovite arvu jagada"
			}
		]
	},
	{
		name: "MODE",
		description: "Enim esinev väärtus andmekogumis.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse mood"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse mood"
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Tagastab massiivis või andmehulgas enim esinevate või korduvate väärtuste vertikaalse massiivi. Horisontaalse massiivi saamiseks kasutage funktsiooni =TRANSPOSE(MODE.MULT(arv1,arv2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse mood"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse mood"
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Enim esinev väärtus massiivis või andmehulgas.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse mood"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse mood"
			}
		]
	},
	{
		name: "MONTH",
		description: "Tagastab kuu, arvu 1 (jaanuar) kuni 12 (detsember).",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv Spreadsheeti kasutatavas kuupäeva-kellaaja koodis"
			}
		]
	},
	{
		name: "MROUND",
		description: "Tagastab ümardusaluse lähima kordseni ümardatud arvu.",
		arguments: [
			{
				name: "arv",
				description: "on väärtus, mida soovite ümardada"
			},
			{
				name: "kordne",
				description: "on arv, mille kordseni soovite ümardada"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Tagastab arvuhulga multinoomi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 väärtust, mille multinoomi soovite leida"
			},
			{
				name: "arv2",
				description: "on 1–255 väärtust, mille multinoomi soovite leida"
			}
		]
	},
	{
		name: "MUNIT",
		description: "Tagastab määratud dimensiooni ühikmaatriksi.",
		arguments: [
			{
				name: "dimensioon",
				description: "on täisarv, mis määrab selle ühikmaatriksi dimensiooni, mida soovite leida"
			}
		]
	},
	{
		name: "N",
		description: "Teisendab mittearvulise väärtuse arvuks, kuupäevad järjenumbriteks, TRUE arvuks 1, kõik muu arvuks 0 (null).",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida soovite teisendada"
			}
		]
	},
	{
		name: "NA",
		description: "Tagastab veaväärtuse #N/A (väärtus pole saadaval).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Negatiivne binoomjaotus, tõenäosus, et saadakse teatud edutute katsete arv enne väärtust edukate _arv, seda edukuse tõenäosusega Tõenäosus.",
		arguments: [
			{
				name: "edutute_arv",
				description: "on edutute katsete arv"
			},
			{
				name: "edukate_arv",
				description: "on edukate katsete arvu lävi"
			},
			{
				name: "tõenäosus",
				description: "on edukuse tõenäosus vahemikus 0–1"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Negatiivne binoomjaotus, tõenäosus, et saadakse teatud edutute katsete arv enne väärtust edukate _arv, seda edukuse tõenäosusega Tõenäosus.",
		arguments: [
			{
				name: "edutute_arv",
				description: "on edutute katsete arv"
			},
			{
				name: "edukate_arv",
				description: "on edukate katsete arvu lävi"
			},
			{
				name: "tõenäosus",
				description: "on edukuse tõenäosus vahemikus 0–1"
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Tagastab kahe kuupäeva vahele jäävate täistööpäevade arvu.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "lõppkuupäev",
				description: "on kuupäeva järjenumber, mis tähistab lõppkuupäeva"
			},
			{
				name: "pühad",
				description: "on valikuline hulk ühest või mitmest kuupäeva järjenumbrist, mis ei kuulu tööpäevade hulka (nt riigipühad ja liikuvad pühad)"
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Tagastab kahe kuupäeva vahele jäävate täistööpäevade arvu, arvestades kohandatud nädalavahetuseparameetritega.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "lõppkuupäev",
				description: "on kuupäeva järjenumber, mis tähistab lõppkuupäeva"
			},
			{
				name: "nädalavahetus",
				description: "on arv või string, mis määrab, millal on nädalavahetused"
			},
			{
				name: "pühad",
				description: "on valikuline hulk ühest või mitmest kuupäeva järjenumbrist, mis ei kuulu tööpäevade hulka (nt riigipühad ja liikuvad pühad)"
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Tagastab nominaalse aastaintressi määra.",
		arguments: [
			{
				name: "tegelik_määr",
				description: "on tegelik intressimäär"
			},
			{
				name: "per_arv",
				description: "on makseperioodide arv aasta kohta"
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Antud keskväärtusele ja standardhälbele vastav normaaljaotus.",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millest arvutatakse jaotus"
			},
			{
				name: "keskväärtus",
				description: "on jaotuse aritmeetiline keskväärtus"
			},
			{
				name: "standardhälve",
				description: "on jaotuse standardhälve, positiivne arv"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotuse puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Normaliseeritud normaaljaotuse jaotusfunktsiooni pöördfunktsioon määratud keskväärtuse ja standardhälbe jaoks.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on normaaljaotusele vastav tõenäosus lõigus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "keskväärtus",
				description: "on jaotuse aritmeetiline keskmine"
			},
			{
				name: "standardhälve",
				description: "jaotuse standardhälve, peab olema positiivne"
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Normaliseeritud normaaljaotus (keskväärtus on null ja standardhälve üks).",
		arguments: [
			{
				name: "z",
				description: "on väärtus, millest arvutatakse jaotus"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotusfunktsiooni puhul TRUE, tõenäosuse tihedusfunktsooni puhul FALSE"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Normaliseeritud kumulatiivse normaaljaotuse pöördfunktsioon (keskväärtus on null ja standardhälve üks).",
		arguments: [
			{
				name: "tõenäosus",
				description: "on normaaljaotusele vastav tõenäosus lõigus 0–1 (1 ja 0 k.a)"
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Antud keskväärtusele ja standardhälbele vastav normaaljaotus.",
		arguments: [
			{
				name: "x",
				description: "on väärtus, millest arvutatakse jaotus"
			},
			{
				name: "keskväärtus",
				description: "on jaotuse aritmeetiline keskväärtus"
			},
			{
				name: "standardhälve",
				description: "on jaotuse standardhälve"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotuse puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Normaliseeritud normaaljaotuse jaotusfunktsiooni pöördfunktsioon määratud keskväärtuse ja standardhälbe jaoks.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on normaaljaotusele vastav tõenäosus lõigus 0–1 (0 ja 1 k.a)"
			},
			{
				name: "keskväärtus",
				description: "on jaotuse aritmeetiline keskmine"
			},
			{
				name: "standardhälve",
				description: "jaotuse standardhälve, peab olema positiivne"
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Normaliseeritud normaaljaotus (keskväärtus on null ja standardhälve üks).",
		arguments: [
			{
				name: "z",
				description: "on väärtus, millest arvutatakse jaotus"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Normaliseeritud normaaljaotuse jaotusfunktsiooni pöördfunktsioon (keskväärtus on null ja standardhälve üks).",
		arguments: [
			{
				name: "tõenäosus",
				description: "on normaaljaotusele vastav tõenäosus lõigus 0–1 (1 ja 0 k.a)"
			}
		]
	},
	{
		name: "NOT",
		description: "Muudab väärtuse FALSE väärtuseks TRUE või väärtuse TRUE väärtuseks FALSE.",
		arguments: [
			{
				name: "loogika",
				description: "on väärtus või avaldis, mille väärtuseks saab anda TRUE või FALSE"
			}
		]
	},
	{
		name: "NOW",
		description: "Tagastab praeguse kuupäeva ja kellaaja, mis on vormindatud kuupäeva ja kellaajana.",
		arguments: [
		]
	},
	{
		name: "NPER",
		description: "Tagastab kindlatel perioodilistel maksetel ja püsival intressimääral põhineva investeeringu perioodide arvu.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "makse",
				description: "on igas perioodis sooritatud makse; see ei tohi investeeringu kestuse jooksul muutuda"
			},
			{
				name: "praegune_väärt",
				description: "on praegune väärtus ehk paušaalsumma, mis tulevaste maksete sari on praegu väärt"
			},
			{
				name: "tul_väärt",
				description: "on tulevane väärtus ehk sularahasaldo, mida soovite pärast viimase makse sooritamist saavutada. Kui tühi, kasutatakse nulli"
			},
			{
				name: "tüüp",
				description: "on loogikaväärtus: makse perioodi alguses = 1; makse perioodi lõpus = 0 või on tühi"
			}
		]
	},
	{
		name: "NPV",
		description: "Tagastab investeeringu praeguse netoväärtuse, võttes aluseks diskontomäära ja tulevaste maksete (negatiivsed väärtused) ning sissetuleku (positiivsed väärtused) sarja.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "määr",
				description: "on diskontomäär ühe perioodi vältel"
			},
			{
				name: "väärtus1",
				description: "on 1–254 makset ja sissetulekut, mis on jaotatud võrdsete ajavahemike peale ja mis leiavad aset iga perioodi lõpus"
			},
			{
				name: "väärtus2",
				description: "on 1–254 makset ja sissetulekut, mis on jaotatud võrdsete ajavahemike peale ja mis leiavad aset iga perioodi lõpus"
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Teisendab teksti arvuks lokaadist sõltumatul viisil.",
		arguments: [
			{
				name: "tekst",
				description: "on teisendatavat arvu kujutav string"
			},
			{
				name: "komakoha_eraldaja",
				description: "on stringis komakoha eraldajana kasutatav märk"
			},
			{
				name: "rühmaeraldaja",
				description: "on stringis rühmaeraldajana kasutatav märk"
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Teisendab kaheksandarvu kahendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kaheksandarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Teisendab kaheksandarvu kümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kaheksandarv"
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Teisendab kaheksandarvu kuueteistkümnendarvuks.",
		arguments: [
			{
				name: "arv",
				description: "on teisendatav kaheksandarv"
			},
			{
				name: "kohad",
				description: "on kasutatavate märkide arv"
			}
		]
	},
	{
		name: "ODD",
		description: "Ümardab positiivse väärtuse ülespoole ja negatiivse allapoole lähima paaritu täisarvuni.",
		arguments: [
			{
				name: "arv",
				description: "ümardatav arv"
			}
		]
	},
	{
		name: "OFFSET",
		description: "Tagastab viite vahemikule, mis on lähteviitest määratud arv ridu ja veerge.",
		arguments: [
			{
				name: "lähteviide",
				description: "on viide, mille suhtes soovite viidata, viide lahtrile või külgnevate lahtrite vahemikule"
			},
			{
				name: "ridade_arv",
				description: "on ridade arv (üles- või allapoole), millele peaks viitama tulemi ülemine vasakpoolne lahter"
			},
			{
				name: "veergude_arv",
				description: "on veergude arv, vasakule või paremale, millele peaks viitama tulemi ülemine vasakpoolne lahter"
			},
			{
				name: "kõrgus",
				description: "on kõrgus ridade arvus, mida soovite tulemiks, niisama kõrge kui lähteviide, kui on tühi"
			},
			{
				name: "laius",
				description: "on laius ridade arvus, mida soovite tulemiks, niisama lai kui lähteviide, kui on tühi"
			}
		]
	},
	{
		name: "OR",
		description: "Kontrollib, kas suvaline argument on TRUE, ja tagastab väärtuse TRUE või FALSE. Tagastab väärtuse FALSE ainult juhul, kui kõik argumendid on FALSE.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "loogika1",
				description: "on 1–255 tingimust, mida soovite testida; tingimus võib olla kas TRUE või FALSE"
			},
			{
				name: "loogika2",
				description: "on 1–255 tingimust, mida soovite testida; tingimus võib olla kas TRUE või FALSE"
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
		description: "Tagastab perioodide arvu, mis on nõutav, et investeering saavutaks määratud väärtuse.",
		arguments: [
			{
				name: "intressimäär",
				description: "on intressimäär perioodi kohta."
			},
			{
				name: "nüüdisväärtus",
				description: "on investeeringu nüüdisväärtus"
			},
			{
				name: "tulevane_väärtus",
				description: "on investeeringu jaoks soovitud tulevane väärtus"
			}
		]
	},
	{
		name: "PEARSON",
		description: "r, Pearsoni korrelatsioonikordaja.",
		arguments: [
			{
				name: "massiiv1",
				description: "on sõltumatute väärtuste hulk"
			},
			{
				name: "massiiv2",
				description: "on sõltuvate väärtuste hulk"
			}
		]
	},
	{
		name: "PERCENTILE",
		description: "Tagastab protsentuaalselt k-nda väärtuse andmevahemikus.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mis määrab suhtelised positsioonid"
			},
			{
				name: "k",
				description: "protsentuaalne väärtus vahemikus nullist üheni (k.a)"
			}
		]
	},
	{
		name: "PERCENTILE.EXC",
		description: "Tagastab andmevahemiku väärtuste k. protsentiili. k väärtus on vahemikus 0–1 (v.a).",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mis määrab suhtelised positsioonid"
			},
			{
				name: "k",
				description: "protsentiili väärtus vahemikus 0–1 (k.a)"
			}
		]
	},
	{
		name: "PERCENTILE.INC",
		description: "Tagastab andmevahemiku väärtuste k. protsentiili. k väärtus on vahemikus 0–1 (k.a).",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mis määrab suhtelised positsioonid"
			},
			{
				name: "k",
				description: "protsentiili väärtus vahemikus 0–1 (k.a)"
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Väärtuse asukoht kasvavalt järjestatud andmekogumis protsentuaalsena.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mis määrab suhtelised positsioonid"
			},
			{
				name: "x",
				description: "väärtus, mille positsiooni määratakse"
			},
			{
				name: "tähtsus",
				description: "mittekohustuslik argument, mis näitab komakohtade arvu, vaikimisi kolm (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Väärtuse asukoht kasvavalt järjestatud andmehulgas protsentuaalsena (0–1 v.a).",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mis määrab suhtelised positsioonid"
			},
			{
				name: "x",
				description: "väärtus, mille positsiooni määratakse"
			},
			{
				name: "tähtsus",
				description: "mittekohustuslik argument, mis näitab komakohtade arvu, vaikimisi kolm (0.xxx%)"
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Väärtuse asukoht kasvavalt järjestatud andmehulgas protsentuaalsena (0–1 k.a).",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mis määrab suhtelised positsioonid"
			},
			{
				name: "x",
				description: "väärtus, mille positsiooni määratakse"
			},
			{
				name: "tähtsus",
				description: "mittekohustuslik argument, mis näitab komakohtade arvu, vaikimisi kolm (0.xxx%)"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Permutatsioonide arv objektide hulgast etteantud valimi suuruse põhjal.",
		arguments: [
			{
				name: "arv",
				description: "on objektide koguarv"
			},
			{
				name: "valitud_arv",
				description: "on valitud objektide arv permutatsioonis"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Tagastab permutatsioonide arvu (koos kordustega) teatud arvu objektide kohta, mille saab objektide koguhulgast valida.",
		arguments: [
			{
				name: "arv",
				description: "on objektide arv kokku"
			},
			{
				name: "valitud_arv",
				description: "on objektide arv iga permutatsiooni korral"
			}
		]
	},
	{
		name: "PHI",
		description: "Tagastab normaliseeritud normaaljaotuse tihedusfunktsiooni väärtuse.",
		arguments: [
			{
				name: "x",
				description: "on arv, mille jaoks soovite normaliseeritud normaaljaotuse tihedust leida"
			}
		]
	},
	{
		name: "PI",
		description: "Tagastab pii väärtuse 3,14159265358979 (täpsusega 15 numbrikohta).",
		arguments: [
		]
	},
	{
		name: "PMT",
		description: "Arvutab kindlatel maksetel ja püsival intressimääral põhineva laenumakse.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär laenu perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "per_arv",
				description: "on laenu tagasimaksete koguarv"
			},
			{
				name: "praegune_väärt",
				description: "on praegune väärtus: tulevaste maksete sarja praeguse väärtuse kogusumma"
			},
			{
				name: "tul_väärt",
				description: "on tulevane väärtus ehk sularahasaldo, mida soovite pärast viimase makse sooritamist saavutada, 0 (null), kui tühi"
			},
			{
				name: "tüüp",
				description: "on loogikaväärtus: makse perioodi alguses = 1; makse perioodi lõpus = 0 või on tühi"
			}
		]
	},
	{
		name: "POISSON",
		description: "Poissoni jaotus.",
		arguments: [
			{
				name: "x",
				description: "on sündmuste kogum"
			},
			{
				name: "keskväärtus",
				description: "on eeldatav väärtus, peab olema positiivne"
			},
			{
				name: "kumulatiivsus",
				description: "on kumulatiivse Poissoni tõenäosuse puhul TRUE, Poissoni tõenäosusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Poissoni jaotus.",
		arguments: [
			{
				name: "x",
				description: "on sündmuste kogum"
			},
			{
				name: "keskväärtus",
				description: "on eeldatav väärtus, peab olema positiivne"
			},
			{
				name: "kumulatiivsus",
				description: "on kumulatiivse Poissoni tõenäosuse puhul TRUE, Poissoni tõenäosusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "POWER",
		description: "Tagastab mingisse astmesse tõstetud arvu tulemi.",
		arguments: [
			{
				name: "arv",
				description: "on alusarv, suvaline reaalarv"
			},
			{
				name: "astendaja",
				description: "on aste, millesse alusarv tõstetakse"
			}
		]
	},
	{
		name: "PPMT",
		description: "Tagastab kindlatel perioodilistel maksetel ja püsival intressimääral põhineva investeeringu puhul makse põhisummalt.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "periood",
				description: "määrab perioodi ja peab jääma vahemikku 1 kuni per_arv"
			},
			{
				name: "per_arv",
				description: "on investeeringu makseperioodide koguarv"
			},
			{
				name: "praegune_väärt",
				description: "on praegune väärtus: kogusumma, mis tulevaste maksete sari on praegu väärt"
			},
			{
				name: "tul_väärt",
				description: "on tulevane väärtus ehk sularahasaldo, mida soovite pärast viimase makse sooritamist saavutada"
			},
			{
				name: "tüüp",
				description: "on loogikaväärtus: makse perioodi alguses = 1; makse perioodi lõpus = 0 või jäetakse ära"
			}
		]
	},
	{
		name: "PRICEDISC",
		description: "Tagastab hinna diskonteeritud väärtpaberi 100-eurose nominaalväärtuse kohta.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "diskonto",
				description: "on väärtpaberi diskontomäär"
			},
			{
				name: "tagastushind",
				description: "on väärtpaberi tagastusväärtus 100-eurose nominaalväärtuse kohta"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "PROB",
		description: "Tõenäosus, et väärtused on kahe piiri vahel või võrdsed alampiiriga.",
		arguments: [
			{
				name: "x_vahemik",
				description: "on arvväärtuste hulk x, millele on olemas vastavad tõenäosused"
			},
			{
				name: "tõenäosusvahemik",
				description: "X_vahemiku väärtusega seotud tõenäosused vahemikus nullist üheni (null välja arvatud)"
			},
			{
				name: "alampiir",
				description: "on alampiir, mille suhtes arvutatakse tõenäosus"
			},
			{
				name: "ülempiir",
				description: "on mittekohustuslik ülempiir. Puudumisel eeldatakse, et X_vahemiku väärtused on samad kui Alampiiri omad"
			}
		]
	},
	{
		name: "PRODUCT",
		description: "Korrutab kõik argumentidena antud arvud.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, loogikaväärtust või teksti kujul esitatud arvu, mida soovite korrutada"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, loogikaväärtust või teksti kujul esitatud arvu, mida soovite korrutada"
			}
		]
	},
	{
		name: "PROPER",
		description: "Teisendab tekstistringi algsuurtähtedega tekstiks; iga sõna esimese tähe suurtäheks ja kõik ülejäänud väiketähtedeks.",
		arguments: [
			{
				name: "tekst",
				description: "on jutumärkidega ümbritsetud tekst, valem, mis annab tulemiks teksti või lahter, mis sisaldab osaliselt suurtähestatavat teksti"
			}
		]
	},
	{
		name: "PV",
		description: "Tagastab investeeringu praeguse väärtuse: tulevaste maksete sarja praeguse väärtuse kogusumma.",
		arguments: [
			{
				name: "määr",
				description: "on intressimäär perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%"
			},
			{
				name: "per_arv",
				description: "on investeeringu makseperioodide koguarv"
			},
			{
				name: "makse",
				description: "on igas perioodis sooritatud makse, mis ei tohi investeeringu kestuse jooksul muutuda"
			},
			{
				name: "tul_väärt",
				description: "on tulevane väärtus ehk sularahasaldo, mida soovite pärast viimase makse sooritamist saavutada"
			},
			{
				name: "tüüp",
				description: "on loogikaväärtus: makse perioodi alguses = 1; makse perioodi lõpus = 0 või jäetakse välja"
			}
		]
	},
	{
		name: "QUARTILE",
		description: "Andmekogumi kvartiil.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, millest arvutatakse kvartiil"
			},
			{
				name: "kvartiil",
				description: "on täisarv: esimene kvartiil=1; keskväärtus=2;kolmas kvartiil=3; suurim väärtus=4"
			}
		]
	},
	{
		name: "QUARTILE.EXC",
		description: "Andmehulga kvartiil, mis põhineb protsentiili väärtustel 0–1 (v.a).",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, millest arvutatakse kvartiil"
			},
			{
				name: "kvartiil",
				description: "on arv: väikseim väärtus=0; esimene kvartiil=1; keskväärtus=2; kolmas kvartiil=3; suurim väärtus=4"
			}
		]
	},
	{
		name: "QUARTILE.INC",
		description: "Andmehulga kvartiil, mis põhineb protsentiili väärtustel 0–1 (k.a).",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, millest arvutatakse kvartiil"
			},
			{
				name: "kvartiil",
				description: "on arv: väikseim väärtus=0; esimene kvartiil=1; keskväärtus=2; kolmas kvartiil=3; suurim väärtus=4"
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Tagastab jagatise täisarvulise osa.",
		arguments: [
			{
				name: "lugeja",
				description: "on jagatav"
			},
			{
				name: "nimetaja",
				description: "on jagaja"
			}
		]
	},
	{
		name: "RADIANS",
		description: "Teisendab kraadid radiaanideks.",
		arguments: [
			{
				name: "nurk",
				description: "on nurk kraadides, mida soovite teisendada"
			}
		]
	},
	{
		name: "RAND",
		description: "Tagastab ühtlase jaotusega juhusliku arvu, mis on suurem või võrdne 0-ga ja väiksem kui 1 (arv muutub uuesti arvutamisel).",
		arguments: [
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Tagastab juhusliku arvu teie määratud vahemikus.",
		arguments: [
			{
				name: "põhi",
				description: "on vähim täisarv, mille funktsioon RANDBETWEEN tagastab"
			},
			{
				name: "lagi",
				description: "on suurim täisarv, mille funktsioon RANDBETWEEN tagastab"
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
		description: "Tagastab arvu asukoha järjestatud arvuloendis.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mille asukohta soovite otsida"
			},
			{
				name: "viide",
				description: "on arvuloendi massiiv või viide arvuloendile. Mittearvulisi väärtusi ignoreeritakse"
			},
			{
				name: "järjestus",
				description: "on arv: asukoht laskuvas järjestuses sorditud loendis = 0 või jäetakse ära; asukoht tõusvas järjestuses sorditud loendis = suvaline nullist erinev väärtus"
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Tagastab arvu koha järjestatud arvuloendis, selle suuruse võrreldes loendi teiste väärtustega. Kui rohkem kui ühel väärtusel on sama koht, siis tagastatakse keskmine koht.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mille kohta soovite teada"
			},
			{
				name: "viide",
				description: "on arvuloendi massiiv või viide arvuloendile. Mittearvulisi väärtusi ignoreeritakse"
			},
			{
				name: "järjestus",
				description: "on arv: koht laskuvas järjestuses sorditud loendis = 0 või jäetakse ära; koht tõusvas järjestuses sorditud loendis = suvaline nullist erinev väärtus"
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Tagastab arvu koha järjestatud arvuloendis, selle suuruse võrreldes loendi teiste väärtustega. Kui rohkem kui ühel väärtusel on sama koht, siis tagastatakse selle väärtuste hulga kõrgeim koht.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mille kohta soovite teada"
			},
			{
				name: "viide",
				description: "on arvuloendi massiiv või viide arvuloendile. Mittearvulisi väärtusi ignoreeritakse"
			},
			{
				name: "järjestus",
				description: "on arv: koht laskuvas järjestuses sorditud loendis = 0 või jäetakse ära; koht tõusvas järjestuses sorditud loendis = suvaline nullist erinev väärtus"
			}
		]
	},
	{
		name: "RATE",
		description: "Tagastab intressimäära laenu või investeeringu perioodi kohta. Nt kasutage kvartalimaksete puhul 6%/4, kui aasta keskmine intressimäär on 6%.",
		arguments: [
			{
				name: "per_arv",
				description: "on laenu või investeeringu makseperioodide koguarv"
			},
			{
				name: "makse",
				description: "on igas perioodis sooritatud makse, mis ei tohi laenu või investeeringu kestuse jooksul muutuda"
			},
			{
				name: "praegune_väärt",
				description: "on praegune väärtus: tulevaste maksete sarja praeguse väärtuse kogusumma"
			},
			{
				name: "tul_väärt",
				description: "on tulevane väärtus ehk sularahasaldo, mida soovite pärast viimase makse sooritamist saavutada. Kui tühi, kasutab tul_väärt = 0"
			},
			{
				name: "tüüp",
				description: "on loogikaväärtus: makse perioodi alguses = 1; makse perioodi lõpus = 0 või on tühi"
			},
			{
				name: "hinnang",
				description: "on teie pakutav määr; kui tühi, siis hinnang = 0,1 (10 protsenti)"
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Tagastab täielikult investeeritud väärtpaberi eest lunastamistähtajal saadava summa.",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "investeering",
				description: "on väärtpaberitesse investeeritud summa"
			},
			{
				name: "diskonto",
				description: "on väärtpaberi diskontomäär"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "REPLACE",
		description: "Asendab osa tekstistringist teistsuguse tekstistringiga.",
		arguments: [
			{
				name: "vana_tekst",
				description: "on tekst, milles soovite mõne märgi asendada"
			},
			{
				name: "algusnr",
				description: "on selle märgi paigutus vanas_tekstis, mida soovite uue_tekstiga asendada"
			},
			{
				name: "märkide_arv",
				description: "on märkide arv vanas_tekstis, mida soovite asendada"
			},
			{
				name: "uus_tekst",
				description: "on tekst, mis asendab märgid vanas_tekstis"
			}
		]
	},
	{
		name: "REPT",
		description: "Kordab teksti määratud arvu kordi. Funktsiooni REPT abil saate täita lahtri mõne tekstistringiga teatud arv kordi.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst, mida soovite korrata"
			},
			{
				name: "arv_kordi",
				description: "on positiivne arv, mis määrab arvu, mitu korda teksti korrata"
			}
		]
	},
	{
		name: "RIGHT",
		description: "Tagastab tekstistringi lõpust alates määratud arvu märke.",
		arguments: [
			{
				name: "tekst",
				description: "on tekstiüksus, mis sisaldab ekstraktitavaid märke"
			},
			{
				name: "märkide_arv",
				description: "määrab, mitu märki soovite ekstraktida; 1, kui ära jäetud"
			}
		]
	},
	{
		name: "ROMAN",
		description: "Teisendab araabia numbrid tekstina esitatavateks rooma numbriteks.",
		arguments: [
			{
				name: "arv",
				description: "on araabia number, mida soovite teisendada"
			},
			{
				name: "vorm",
				description: "on arv, mis määrab teie soovitud rooma numbri tüübi."
			}
		]
	},
	{
		name: "ROUND",
		description: "Ümardab arvu määratud kohtade arvuni.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mida soovite ümardada"
			},
			{
				name: "kohtade_arv",
				description: "on kohtade arv, milleni soovite ümardada. Negatiivne ümardatakse komakohast vasakule; null lähima täisarvuni"
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Ümardab arvu allapoole, nullile lähemale.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv, mida soovite allapoole ümardada"
			},
			{
				name: "kohtade_arv",
				description: "on kohtade arv, milleni soovite ümardada. Negatiivne ümardatakse komakohast vasakule; kui null või ära jäetud, siis lähima täisarvuni"
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Ümardab arvu ülespoole, nullist eemale.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv, mida soovite ülespoole ümardada"
			},
			{
				name: "kohtade_arv",
				description: "on kohtade arv, milleni soovite ümardada. Negatiivne ümardatakse komakohast vasakule; kui null või ära jäetud, siis lähima täisarvuni"
			}
		]
	},
	{
		name: "ROW",
		description: "Tagastab suvalise viite reanumbri.",
		arguments: [
			{
				name: "viide",
				description: "on lahter või üksik lahtrivahemik, mille reanumbrit soovite; kui see on tühi, tagastab funktsiooni ROW sisaldava lahtri"
			}
		]
	},
	{
		name: "ROWS",
		description: "Tagastab ridade arvu viites või massiivis.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv, massiivivalem või viide lahtrivahemikule, mille kohta soovite ridade arvu otsida"
			}
		]
	},
	{
		name: "RRI",
		description: "Tagastab investeeringu kasvu jaoks võrdväärse intressimäära.",
		arguments: [
			{
				name: "perioodide_arv",
				description: "on investeeringuperioodide arv"
			},
			{
				name: "nüüdisväärtuse",
				description: "on investeeringu nüüdisväärtus"
			},
			{
				name: "tulevane_väärtus",
				description: "on investeeringu tulevane väärtus"
			}
		]
	},
	{
		name: "RSQ",
		description: "Andmepunktide Pearsoni korrelatsioonikordaja ruut.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on andmepunktide vahemik või massiiv, mis sisaldab arve, lahtrinimesid, massiive või lahtriviitasid arvandmetele"
			},
			{
				name: "teada_x_väärtused",
				description: "on andmepunktide vahemik või massiiv, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvandmetele"
			}
		]
	},
	{
		name: "RTD",
		description: "Laadib alla reaalajas andmed mõnest COM-automatiseerimist toetavast programmist.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "programmi-ID",
				description: "on registreeritud COM-automatiseerimise lisandprogrammi ID nimi. Pange nimi jutumärkidesse"
			},
			{
				name: "server",
				description: "on serveri nimi, kus lisandmoodul tuleks käivitada. Pange nimi jutumärkidesse. Kui lisandmoodul käivitatakse lokaalselt, kasutage tühja üksust"
			},
			{
				name: "teema1",
				description: "on 1–38 parameetrit, mis määravad andmetüki"
			},
			{
				name: "teema2",
				description: "on 1–38 parameetrit, mis määravad andmetüki"
			}
		]
	},
	{
		name: "SEARCH",
		description: "Tagastab selle märgi numbri, kus kindel märk või tekstistring esimesena leitakse, lugedes teksti vasakult paremale (pole tõstutundlik).",
		arguments: [
			{
				name: "otsitav_tekst",
				description: "on tekst, mida soovite otsida. Võite kasutada metamärke ? ja *; märke ? ja * saate otsida ~? ja ~* abil"
			},
			{
				name: "teksti_seest",
				description: "on tekst, millest soovite otsitav_tekst otsida"
			},
			{
				name: "algusnr",
				description: "on märgi number teksti_seest, vasakult loetuna, millest alates soovite otsimist alustada. Kui tühi, kasutatakse 1"
			}
		]
	},
	{
		name: "SEC",
		description: "Tagastab nurga seekansi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille seekansit soovite leida"
			}
		]
	},
	{
		name: "SECH",
		description: "Tagastab nurga hüperboolse seekansi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille hüperboolset seekansit soovite leida"
			}
		]
	},
	{
		name: "SECOND",
		description: "Tagastab sekundid arvuna vahemikus 0–59.",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv Spreadsheeti kasutatavas kuupäeva-kellaaja koodis või tekst ajavormingus, nt 16:48:00 või 4:48:00 PL"
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Tagastab valemil põhineva astmerea summa.",
		arguments: [
			{
				name: "x",
				description: "on astmerea sisendväärtus"
			},
			{
				name: "n",
				description: "on esimene aste, millesse soovite x-i tõsta"
			},
			{
				name: "m",
				description: "on samm, mille võrra n tõuseb sarja iga järgmise liikme jaoks"
			},
			{
				name: "kordajad",
				description: "on kordajate kogum, millega iga järgmist x-i astet korrutatakse"
			}
		]
	},
	{
		name: "SHEET",
		description: "Tagastab viidatud lehe numbri.",
		arguments: [
			{
				name: "väärtus",
				description: "on lehe või viite nimi, mille lehenumbrit soovite leida. Kui see argument välja jätta, tagastatakse funktsiooni sisaldava lehe number"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Tagastab lehtede arvu viites.",
		arguments: [
			{
				name: "viide",
				description: "on viide, milles sisalduvate lehtede arvu soovite leida. Kui see argument välja jätta, tagastatakse seda funktsiooni sisaldava töövihiku lehtede arv"
			}
		]
	},
	{
		name: "SIGN",
		description: "Tagastab arvu märgi: 1, kui arv on positiivne, null, kui arv on null või -1, kui arv on negatiivne.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv"
			}
		]
	},
	{
		name: "SIN",
		description: "Tagastab nurga siinuse.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille siinust soovite leida. Kraadid * PI()/180 = radiaanid"
			}
		]
	},
	{
		name: "SINH",
		description: "Annab vastuseks arvu hüperboolse siinuse.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv"
			}
		]
	},
	{
		name: "SKEW",
		description: "Asümmeetriakordaja: iseloomustab jaotuse asümmeetriat keskmise suhtes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse asümmeetrikordaja"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu või lahtrinime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse asümmeetrikordaja"
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Tagastab jaotuse asümmeetria populatsiooni põhjal: iseloomustab jaotuse asümmeetriat keskmise suhtes.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–254 arvu või nime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse asümmeetrikordaja"
			},
			{
				name: "arv2",
				description: "on 1–254 arvu või nime, massiivi või viidet arvväärtustele, mille põhjal arvutatakse asümmeetrikordaja"
			}
		]
	},
	{
		name: "SLN",
		description: "Tagastab vara ühtlase amortisatsiooni ühe perioodi kohta.",
		arguments: [
			{
				name: "maksumus",
				description: "on vara soetusmaksumus"
			},
			{
				name: "jääk",
				description: "on jääkväärtus vara kasutusea lõpus"
			},
			{
				name: "kestus",
				description: "on perioodide arv, mille jooksul vara amortiseerub (nn vara kasulik tööiga)"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Regressioonisirge tõus.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on sõltuvate andmepunktide massiiv või massiiv, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvandmetele"
			},
			{
				name: "teada_x_väärtused",
				description: "on sõltumatute andmepunktide hulk, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvandmetele"
			}
		]
	},
	{
		name: "SMALL",
		description: "Väiksuselt k-s väärtus andmehulgas. Näiteks väiksuselt viies väärtus.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, millest leitakse väiksuselt k-s väärtus"
			},
			{
				name: "k",
				description: "on väärtuse positsioon (kasvavas järjestuses) massiivis või lahtrivahemikus"
			}
		]
	},
	{
		name: "SQRT",
		description: "Tagastab arvu ruutjuure.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mille ruutjuurt soovite leida"
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Tagastab ruutjuure korrutisest (arv * pii).",
		arguments: [
			{
				name: "arv",
				description: "on arv, millega pii korrutatakse"
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Keskväärtuse ja standardhälbega iseloomustatud jaotuse normaliseeritud väärtus.",
		arguments: [
			{
				name: "x",
				description: "on normaliseeritav väärtus"
			},
			{
				name: "keskväärtus",
				description: "on jaotuse aritmeetiline keskmine"
			},
			{
				name: "standardhälve",
				description: "on jaotuse standardhälve, peab olema positiivne"
			}
		]
	},
	{
		name: "STDEV",
		description: "Esitab valimi standardhälbe, võttes aluseks kogumi (ignoreerib valimis loogikaväärtusi ja teksti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, mis vastavad populatsiooni valimile ja võivad olla arvud või arve sisaldavad viited"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, mis vastavad populatsiooni valimile ja võivad olla arvud või arve sisaldavad viited"
			}
		]
	},
	{
		name: "STDEV.P",
		description: "Arvutab standardhälbe argumentidena esitatud täispopulatsiooni põhjal (ignoreerib loogikaväärtusi ja teksti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, mis vastavad populatsioonile ja mis võivad olla arvud või arve sisaldavad viited"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, mis vastavad populatsioonile ja mis võivad olla arvud või arve sisaldavad viited"
			}
		]
	},
	{
		name: "STDEV.S",
		description: "Esitab valimi standardhälbe, võttes aluseks kogumi (ignoreerib valimis loogikaväärtusi ja teksti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, mis vastavad populatsiooni valimile ja võivad olla arvud või arve sisaldavad viited"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, mis vastavad populatsiooni valimile ja võivad olla arvud või arve sisaldavad viited"
			}
		]
	},
	{
		name: "STDEVA",
		description: "Esitab valimi standardhälbe mõne valimi põhjal, kaasates loogikaväärtused ja teksti. Teksti ja loogikaväärtuse FALSE väärtus on 0; loogikaväärtuse TRUE väärtus on 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 mõnele valimile vastavat väärtust, mis võivad olla väärtused, nimed või viited väärtustele"
			},
			{
				name: "väärtus2",
				description: "on 1–255 mõnele valimile vastavat väärtust, mis võivad olla väärtused, nimed või viited väärtustele"
			}
		]
	},
	{
		name: "STDEVP",
		description: "Arvutab standardhälbe argumentidena esitatud täispopulatsiooni põhjal (ignoreerib loogikaväärtusi ja teksti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, mis vastavad populatsioonile ja mis võivad olla arvud või arve sisaldavad viited"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, mis vastavad populatsioonile ja mis võivad olla arvud või arve sisaldavad viited"
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Arvutab standardhälbe täispopulatsiooni põhjal, kaasates loogikaväärtused ja teksti. Teksti ja loogikaväärtuse FALSE väärtus on 0; loogikaväärtuse TRUE väärtus on 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 mõnele populatsioonile vastavat väärtust, mis võivad olla väärtused, nimed, massiivid või väärtusi sisaldavad viited"
			},
			{
				name: "väärtus2",
				description: "on 1–255 mõnele populatsioonile vastavat väärtust, mis võivad olla väärtused, nimed, massiivid või väärtusi sisaldavad viited"
			}
		]
	},
	{
		name: "STEYX",
		description: "Arvutatud y-väärtuste standardviga igale x-väärtusele regressioonis.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on sõltuvate andmepunktide vahemik või massiiv, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvandmetele"
			},
			{
				name: "teada_x_väärtused",
				description: "on sõltumatute andmepunktide vahemik või massiiv, mis sisaldab arve, lahtrinimesid, massiive või viiteid arvandmetele"
			}
		]
	},
	{
		name: "SUBSTITUTE",
		description: "Asendab tekstistringis olemasoleva teksti uue tekstiga.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst või viide lahtrile, mis sisaldab teksti, milles soovite märke asendada"
			},
			{
				name: "vana_tekst",
				description: "on olemasolev tekst, mida soovite asendada. Kui vana_teksti suurtähestus ei vasta teksti suurtähestusele, ei asenda funktsioon ASENDA teksti"
			},
			{
				name: "uus_tekst",
				description: "on tekst, millega soovite vana_teksti asendada"
			},
			{
				name: "esinemisjuhu_nr",
				description: "määrab, millist vana_teksti esinemisjuhtu soovite asendada. Kui ära jäetud, asendatakse kõik vana_teksti esinemisjuhud"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Tagastab loendis või andmebaasis vahekokkuvõtte.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funktsiooni_nr",
				description: "on arv 1 kuni 11, mis määrab vahekokkuvõtte kokkuvõttefunktsiooni."
			},
			{
				name: "viide1",
				description: "on 1–254 vahemikku või viidet, mille kohta soovite vahekokkuvõtet"
			}
		]
	},
	{
		name: "SUM",
		description: "Liidab kõik arvud lahtrivahemikus.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 liidetavat arvu. Loogikaväärtusi ja teksti lahtrites ignoreeritakse, argumentidena tipituna kaasatakse"
			},
			{
				name: "arv2",
				description: "on 1–255 liidetavat arvu. Loogikaväärtusi ja teksti lahtrites ignoreeritakse, argumentidena tipituna kaasatakse"
			}
		]
	},
	{
		name: "SUMIF",
		description: "Liidab etteantud tingimuse või kriteeriumidega määratud lahtrid.",
		arguments: [
			{
				name: "vahemik",
				description: "on lahtrivahemik, mida soovite väärtustada"
			},
			{
				name: "kriteeriumid",
				description: "on arvu, avaldise või tekstina esitatud kriteeriumid või tingimus, mis määratleb, millised lahtrid liidetakse"
			},
			{
				name: "liida_vahemik",
				description: "on tegelikud liidetavad lahtrid. Vaikimisi kasutatakse vahemiku lahtreid"
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Liidab määratud tingimuste või kriteeriumide hulgaga määratud lahtrid.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "liidetav_vahemik",
				description: "on liidetavad lahtrid."
			},
			{
				name: "kriteeriumide_vahemik",
				description: "on lahtrivahemik, mille soovite konkreetse tingimuse puhul väärtustada"
			},
			{
				name: "kriteeriumid",
				description: "on arvu, avaldise või tekstina esitatud tingimus või kriteeriumid, mis määratleb, millised lahtrid liidetakse"
			}
		]
	},
	{
		name: "SUMPRODUCT",
		description: "Tagastab vastavate vahemike või massiivide korrutiste summa.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "massiiv1",
				description: "on 2–255 massiivi, mille jaoks soovite komponente korrutada ja seejärel lisada. Kõigi massiivide mõõtmed peavad olema ühesuurused"
			},
			{
				name: "massiiv2",
				description: "on 2–255 massiivi, mille jaoks soovite komponente korrutada ja seejärel lisada. Kõigi massiivide mõõtmed peavad olema ühesuurused"
			},
			{
				name: "massiiv3",
				description: "on 2–255 massiivi, mille jaoks soovite komponente korrutada ja seejärel lisada. Kõigi massiivide mõõtmed peavad olema ühesuurused"
			}
		]
	},
	{
		name: "SUMSQ",
		description: "Argumentide ruutude summa. Argumentideks võivad olla arvud, massiivid, lahtrinimed või viited arvväärtustele.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvu, massiivi, lahtrinime või lahtriviidet massiividele, mille põhjal arvutatakse ruutude summa"
			},
			{
				name: "arv2",
				description: "on 1–255 arvu, massiivi, lahtrinime või lahtriviidet massiividele, mille põhjal arvutatakse ruutude summa"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Massiivide vastavate elementide ruutude vahede summa.",
		arguments: [
			{
				name: "massiiv_x",
				description: "on esimene lahtrivahemik või massiiv, milleks on arv, lahtrinimi või lahtriviit arvudele"
			},
			{
				name: "massiiv_y",
				description: "teine lahtrivahemik või massiiv, milleks on arv, lahtrinimi või lahtriviide arvudele"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Massiivide vastavate elementide ruutude summa.",
		arguments: [
			{
				name: "massiiv_x",
				description: "on esimene lahtrivahemik või massiiv, milleks on arv, lahtrinimi või lahtriviide arvudele"
			},
			{
				name: "massiiv_y",
				description: "on teine lahtrivahemik või massiiv, milleks on arv, lahtrinimi või lahtriviide arvudele"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Massiivide vastavate elementide vahede ruutude summa.",
		arguments: [
			{
				name: "massiiv_x",
				description: "esimene lahtrivahemik või massiiv, milleks on arv, lahtrinimi või lahtriviit arvudele"
			},
			{
				name: "massiiv_y",
				description: "on teine lahtrivahemik või massiiv, milleks on arv, lahtrinimi või lahtriviit arvudele"
			}
		]
	},
	{
		name: "SYD",
		description: "Tagastab vara kumulatiivse amortisatsiooni määratud perioodi kohta.",
		arguments: [
			{
				name: "maksumus",
				description: "on vara soetusmaksumus"
			},
			{
				name: "jääk",
				description: "on jääkväärtus vara kasutusea lõpus"
			},
			{
				name: "kestus",
				description: "on perioodide arv, mille jooksul vara amortiseerub (nn vara kasulik tööiga)"
			},
			{
				name: "periood",
				description: "on periood; kasutada tuleb samu ühikuid nagu kestuse puhul"
			}
		]
	},
	{
		name: "T",
		description: "Kontrollib, kas väärtus on tekst ja kui on, siis tagastab teksti, või kui pole, siis tagastab topeltjutumärgid (tühja teksti).",
		arguments: [
			{
				name: "väärtus",
				description: "on väärtus, mida testida"
			}
		]
	},
	{
		name: "T.DIST",
		description: "Vasakpoolse piiranguga Studenti t-jaotus.",
		arguments: [
			{
				name: "x",
				description: "on argument, mille põhjal arvutada jaotusfunktsiooni väärtus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on jaotuse vabadusastmeid määrav täisarv"
			},
			{
				name: "kumulatiivsus",
				description: "on loogikaväärtus: kumulatiivse jaotuse puhul TRUE, tõenäosuse tihedusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Tagastab kahepoolse piiranguga Studenti t-jaotuse.",
		arguments: [
			{
				name: "x",
				description: "on argument, mille põhjal arvutada jaotusfunktsiooni väärtus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on jaotuse vabadusastmeid määrav täisarv"
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Tagastab parempoolse piiranguga Studenti t-jaotuse.",
		arguments: [
			{
				name: "x",
				description: "on argument, mille põhjal arvutada jaotusfunktsiooni väärtus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on jaotuse vabadusastmeid määrav täisarv"
			}
		]
	},
	{
		name: "T.INV",
		description: "Vasakpoolse piiranguga Studenti t-jaotuse pöördfunktsioon.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on kahepoolse piiranguga Studenti t-jaotusega seotud tõenäosused vahemikus 0–1 (k.a)"
			},
			{
				name: "vabadusastmete_arv",
				description: "positiivne täisarv, mis tähistab jaotuse vabadusastmete arvu"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Kahepoolse piiranguga Studenti t-jaotuse pöördfunktsioon.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on kahepoolse piiranguga Studenti t-jaotusega seotud tõenäosused vahemikus 0–1 (k.a)"
			},
			{
				name: "vabadusastmete_arv",
				description: "positiivne täisarv, mis tähistab jaotuse vabadusastmete arvu"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Studenti t-testiga seotud tõenäosus.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene andmehulk"
			},
			{
				name: "massiiv2",
				description: "on teine andmehulk"
			},
			{
				name: "piirangute_arv",
				description: "on jaotuse piirangute arv: ühepoolse piiranguga jaotus = 1; kahepoolse piiranguga jaotus = 2"
			},
			{
				name: "tüüp",
				description: "t-testi tüüp: paaris=1; kahene võrdne variatsioon=2; kahene mittevõrdne variatsioon=3"
			}
		]
	},
	{
		name: "TAN",
		description: "Tagastab nurga tangensi.",
		arguments: [
			{
				name: "arv",
				description: "on nurk radiaanides, mille tangensit soovite leida. Kraadid * PI()/180 = radiaanid"
			}
		]
	},
	{
		name: "TANH",
		description: "Annab vastuseks arvu hüperboolse tangensi.",
		arguments: [
			{
				name: "arv",
				description: "on suvaline reaalarv"
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Tagastab riigivõlakirja võlatähega võrreldava tootluse.",
		arguments: [
			{
				name: "tehing",
				description: "on riigivõlakirja tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on riigivõlakirja maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "diskonto",
				description: "on riigivõlakirja diskontomäär"
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Tagastab riigivõlakirja hinna 100-eurose nominaalväärtuse kohta.",
		arguments: [
			{
				name: "tehing",
				description: "on riigivõlakirja tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on riigivõlakirja maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "diskonto",
				description: "on riigivõlakirja diskontomäär"
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Tagastab riigivõlakirja tootluse.",
		arguments: [
			{
				name: "tehing",
				description: "on riigivõlakirja tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on riigivõlakirja maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "hind",
				description: "on riigivõlakirja hind 100-eurose nominaalväärtuse kohta"
			}
		]
	},
	{
		name: "TDIST",
		description: "Studenti t-jaotus.",
		arguments: [
			{
				name: "x",
				description: "on argument, mille põhjal arvutada jaotusfunktsiooni väärtus"
			},
			{
				name: "vabadusastmete_arv",
				description: "on jaotuse vabadusastmeid määrav täisarv"
			},
			{
				name: "piirangute_arv",
				description: "on piirangute arv: ühepoolse piiranguga jaotus = 1; kahepoolse piiranguga jaotus = 2"
			}
		]
	},
	{
		name: "TEXT",
		description: "Teisendab väärtuse kindlas arvuvormingus tekstiks.",
		arguments: [
			{
				name: "väärtus",
				description: "on arv, valem, mis annab väärtuseks mõne arvulise väärtuse, või viite mõnele arvulist väärtust sisaldavale lahtrile"
			},
			{
				name: "vorming_tekst",
				description: "on arvuvorming teksti kujul, mis on pärit dialoogiboksi Lahtrite vormindamine vahekaardi Arv loendiboksist Kategooria (mitte Üldine)"
			}
		]
	},
	{
		name: "TIME",
		description: "Teisendab arvudena antud tunnid, minutid ja sekundid Spreadsheeti järjenumbriks, mis on vormindatud ajavorminguga.",
		arguments: [
			{
				name: "tund",
				description: "on arv vahemikus 0–23, mis tähistab tundi"
			},
			{
				name: "minut",
				description: "on arv v ahemikus 0–59, mis tähistab minutit"
			},
			{
				name: "sekund",
				description: "on arv vahemikus 0–59, mis tähistab sekundit"
			}
		]
	},
	{
		name: "TIMEVALUE",
		description: "Teisendab tekstina esitatud kellaaja Spreadsheeti järjenumbriks kellaaja kohta, arv vahemikus 0 (12:00:00 EL) kuni 0,999988426 (11:59:59 PL). Vormindage arv kellaajavorminguga pärast valemi sisestamist.",
		arguments: [
			{
				name: "kellaaja_tekst",
				description: "on tekstistring, mis annab kellaaja suvalises Spreadsheeti kellaajavormingus (kuupäevateavet stringis ignoreeritakse)"
			}
		]
	},
	{
		name: "TINV",
		description: "Kahepoolse piiranguga Studenti t-jaotuse pöördväärtus.",
		arguments: [
			{
				name: "tõenäosus",
				description: "on kahepoolse piiranguga Studenti t-jaotusega seotud tõenäosused lõigus nullist üheni (kaasa arvatud)"
			},
			{
				name: "vabadusastmete_arv",
				description: "positiivne täisarv, jaotuse vabadusastmete arvu"
			}
		]
	},
	{
		name: "TODAY",
		description: "Tagastab praeguse kuupäeva, mis on vormindatud kuupäevana.",
		arguments: [
		]
	},
	{
		name: "TRANSPOSE",
		description: "Teisendab vertikaalse lahtrivahemiku horisontaalseks või vastupidi.",
		arguments: [
			{
				name: "massiiv",
				description: "on lahtrivahemik töölehel või väärtustemassiivis, mida soovite transponeerida"
			}
		]
	},
	{
		name: "TREND",
		description: "Tagastab mõnes lineaartrendis olevad arvud, mis vastavad teadaolevaile andmepunktidele, kasutades vähimruutude meetodit.",
		arguments: [
			{
				name: "teada_y_väärtused",
				description: "on nende y-väärtuste vahemik või massiiv, mida seoses y = mx + b juba teate"
			},
			{
				name: "teada_x_väärtused",
				description: "on nende x-väärtuste valikuline vahemik või massiiv, mida te seoses y = mx + b juba teate, massiiv, mis on niisama suur kui teada_y_väärtused"
			},
			{
				name: "uued_x_väärtused",
				description: "on uute x-väärtuste vahemik või massiiv, mille kohta soovite, et TREND tagastaks vastavad y-väärtused"
			},
			{
				name: "konstant",
				description: "on loogikaväärtus; konstant b arvutatakse tavaliselt juhul, kui konstant = TRUE või tühi; b võrdub 0, kui konstant = FALSE"
			}
		]
	},
	{
		name: "TRIM",
		description: "Eemaldab tekstistringist kõik tühikud, välja arvatud sõnadevahelised üksikud tühikud.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst, millest soovite tühikud eemaldada"
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Andmekogumi ahendi keskmine.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või lahtrivahemik, mida ahendatakse"
			},
			{
				name: "protsent",
				description: "on andmepunktide suhtarv, mis jäetakse andmekogumi algusest ja lõpust välja"
			}
		]
	},
	{
		name: "TRUE",
		description: "Tagastab loogikaväärtuse TRUE.",
		arguments: [
		]
	},
	{
		name: "TRUNC",
		description: "Kärbib arvu täisarvuni, eemaldades murdosa.",
		arguments: [
			{
				name: "arv",
				description: "on arv, mida soovite kärpida"
			},
			{
				name: "kohtade_arv",
				description: "on arv, mis määrab kärpe täpsuse, 0 (null), kui ära jäetud"
			}
		]
	},
	{
		name: "TTEST",
		description: "Studenti t-testiga seotud tõenäosus.",
		arguments: [
			{
				name: "massiiv1",
				description: "on esimene andmehulk"
			},
			{
				name: "massiiv2",
				description: "on teine andmehulk"
			},
			{
				name: "piirangute_arv",
				description: "on piirangute arv: ühepoolse piiranguga = 1; kahepoolse piiranguga jaotus = 2"
			},
			{
				name: "tüüp",
				description: "t-testi tüüp:paaris = 1; kahene võrdne variatsioon = 2; kahene mittevõrdne variatsioon = 3"
			}
		]
	},
	{
		name: "TYPE",
		description: "Tagastab täisarvu, mis tähistab mõne väärtuse andmetüüpi: arv = 1; tekst = 2; loogikaväärtus = 4; veaväärtus = 16; massiiv = 64.",
		arguments: [
			{
				name: "väärtus",
				description: "võib olla suvaline väärtus"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Tagastab teksti esimesele märgile vastava arvu (koodipunkti).",
		arguments: [
			{
				name: "tekst",
				description: "on märk, mille Unicode'i väärtust soovite leida"
			}
		]
	},
	{
		name: "UPPER",
		description: "Teisendab tekstistringi üleni suurtähtedeks.",
		arguments: [
			{
				name: "tekst",
				description: "on tekst, mida soovite suurtähtedeks teisendada, viide või tekstistring"
			}
		]
	},
	{
		name: "VALUE",
		description: "Teisendab arvu tähistava tekstistringi arvuks.",
		arguments: [
			{
				name: "tekst",
				description: "on jutumärkidega raamitud tekst või viide lahtrile, mis sisaldab teksti, mida soovite teisendada"
			}
		]
	},
	{
		name: "VAR",
		description: "Esitab valimi dispersiooni, mille aluseks on mõni valim (ignoreerib valimis loogikaväärtusi ja teksti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 numbrilist argumenti, mis vastavad populatsiooni valimile"
			},
			{
				name: "arv2",
				description: "on 1–255 numbrilist argumenti, mis vastavad populatsiooni valimile"
			}
		]
	},
	{
		name: "VAR.P",
		description: "Arvutab dispersiooni täispopulatsiooni põhjal (ignoreerib loogikaväärtusi ja teksti populatsioonis).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvulist argumenti, mis vastavad populatsioonile"
			},
			{
				name: "arv2",
				description: "on 1–255 arvulist argumenti, mis vastavad populatsioonile"
			}
		]
	},
	{
		name: "VAR.S",
		description: "Esitab valimi dispersiooni, mille aluseks on mõni valim (ignoreerib valimis loogikaväärtusi ja teksti).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 numbrilist argumenti, mis vastavad populatsiooni valimile"
			},
			{
				name: "arv2",
				description: "on 1–255 numbrilist argumenti, mis vastavad populatsiooni valimile"
			}
		]
	},
	{
		name: "VARA",
		description: "Esitab dispersiooni mõne valimi põhjal, kaasates loogikaväärtused ja teksti. Teksti ja loogikaväärtuse FALSE väärtus on 0; loogikaväärtuse TRUE väärtus on 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 mõnele valimile vastavat väärtusargumenti"
			},
			{
				name: "väärtus2",
				description: "on 1–255 mõnele valimile vastavat väärtusargumenti"
			}
		]
	},
	{
		name: "VARP",
		description: "Arvutab dispersiooni täispopulatsiooni põhjal (ignoreerib loogikaväärtusi ja teksti populatsioonis).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "arv1",
				description: "on 1–255 arvulist argumenti, mis vastavad populatsioonile"
			},
			{
				name: "arv2",
				description: "on 1–255 arvulist argumenti, mis vastavad populatsioonile"
			}
		]
	},
	{
		name: "VARPA",
		description: "Arvutab dispersiooni täispopulatsiooni põhjal, kaasates loogikaväärtused ja teksti. Teksti ja loogikaväärtuse FALSE väärtus on 0; loogikaväärtuse TRUE väärtus on 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "väärtus1",
				description: "on 1–255 mõnele populatsioonile vastavat väärtusargumenti"
			},
			{
				name: "väärtus2",
				description: "on 1–255 mõnele populatsioonile vastavat väärtusargumenti"
			}
		]
	},
	{
		name: "VDB",
		description: "Tagastab vara amortisatsiooni suvalise teie määratud perioodi kohta, sh osalised perioodid, kasutades topeltdegressiivset amortisatsioonimeetodit või mõnda muud teie määratud meetodit.",
		arguments: [
			{
				name: "maksumus",
				description: "on vara soetusmaksumus"
			},
			{
				name: "jääk",
				description: "on jääkväärtus vara kasutusea lõpus"
			},
			{
				name: "kestus",
				description: "on perioodide arv, mille jooksul vara amortiseerub (nn vara kasulik tööiga)"
			},
			{
				name: "perioodi_algus",
				description: "on algusperiood, mille kohta soovite amortisatsiooni arvutada, samades ühikutes kui kestus"
			},
			{
				name: "perioodi_lõpp",
				description: "on lõpp-periood, mille kohta soovite amortisatsiooni arvutada, samades ühikutes kui kestus"
			},
			{
				name: "tegur",
				description: "on kordaja, millega bilanss kahaneb, 2 (topeltdegressiivne bilanss), kui ära jäetud"
			},
			{
				name: "üleminekuta",
				description: "lüleminek ühtlasele amortisatsioonile, kui amortisatsioon on suurem kui kahanev bilanss = FALSE või jäetakse ära; üleminekuta = TRUE"
			}
		]
	},
	{
		name: "VLOOKUP",
		description: "Otsib väärtust mõne tabeli vasakpoolseimast veerust ja tagastab väärtuse teie määratud veeruga samas reas. Vaikimisi tuleb tabelit sortida tõusvas järjestuses.",
		arguments: [
			{
				name: "otsitav_väärtus",
				description: "on väärtus, mida tabeli esimesest veerust otsida; see võib olla väärtus, viide või tekstistring"
			},
			{
				name: "massiiv",
				description: "on tekstist, arvudest või loogikaväärtustest koosnev tabel, millest andmeid otsitakse. Massiiv võib olla viide vahemikule või vahemiku nimele"
			},
			{
				name: "veeru_indeks",
				description: "on massiivi veerunumber, millest tuleks vastav väärtus tagastada. Tabeli esimene väärtusteveerg on veerg 1"
			},
			{
				name: "vastendustüüp",
				description: "on loogikaväärtus: lähima vaste otsimine esimesest veerust (sorditud tõusvas järjestuses) = TRUE või on tühi; täpse vaste otsimine = FALSE"
			}
		]
	},
	{
		name: "WEEKDAY",
		description: "Tagastab arvu vahemikus 1–7, mis näitab kuupäevale vastavat nädalapäeva.",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv, mis tähistab kuupäeva"
			},
			{
				name: "tagastustüüp",
				description: "on arv: pühapäevast=1 laupäevani=7, kasutage 1; esmaspäevast=1 pühapäevani=7, kasutage 2; esmaspäevast=0 pühapäevani=6, kasutage 3"
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Tagastab nädala järjenumbri aastas.",
		arguments: [
			{
				name: "järjenumber",
				description: "on kuupäeva-kellaaja kood, mida Spreadsheet kasutab kuupäeva- ja kellaajaarvutustes"
			},
			{
				name: "tagastustüüp",
				description: "on arv (1 või 2), mis määratleb tagastatava väärtuse tüübi"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Weibulli jaotus.",
		arguments: [
			{
				name: "x",
				description: "mittenegatiivne väärtus, mille põhjal arvutatakse jaotus"
			},
			{
				name: "alfa",
				description: "on jaotuse parameeter, positiivne väärtus"
			},
			{
				name: "beeta",
				description: "on jaotuse parameeter, positiivne väärtus"
			},
			{
				name: "kumulatiivsus",
				description: "on kumulatiivse jaotuse puhul TRUE, tõenäosusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Weibulli jaotus.",
		arguments: [
			{
				name: "x",
				description: "mittenegatiivne väärtus, mille põhjal arvutatakse jaotus"
			},
			{
				name: "alfa",
				description: "on jaotuse parameeter, positiivne väärtus"
			},
			{
				name: "beeta",
				description: "on jaotuse parameeter, positiivne väärtus"
			},
			{
				name: "kumulatiivsus",
				description: "on kumulatiivse jaotuse puhul TRUE, tõenäosusfunktsiooni puhul FALSE"
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Tagastab näidatud kuupäevast antud tööpäevade arvu võrra varasema või hilisema kuupäeva järjenumbri.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "päevade_arv",
				description: "enne või pärast alguskuupäeva olevate päevade arv (v.a nädalalõpupäevad ja pühad)"
			},
			{
				name: "pühad",
				description: "on valikuline massiiv ühest või mitmest kuupäeva järjenumbrist, mis ei kuulu tööpäevade hulka (nt riigipühad ja liikuvad pühad)"
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Tagastab näidatud kuupäevast antud tööpäevade arvu võrra varasema või hilisema kuupäeva järjenumbri, arvestades kohandatud nädalavahetuseparameetritega.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "päevade_arv",
				description: "enne või pärast alguskuupäeva olevate päevade arv (v.a. nädalavahetus ja pühad)"
			},
			{
				name: "nädalavahetus",
				description: "on number või string, mis määrab, millal on nädalavahetused"
			},
			{
				name: "pühad",
				description: "on valikuline massiiv ühest või mitmest kuupäeva järjenumbrist, mis ei kuulu tööpäevade hulka (nt riigipühad ja liikuvad pühad)"
			}
		]
	},
	{
		name: "XIRR",
		description: "Tagastab rahavoogude sarja sisemise tagastusmäära.",
		arguments: [
			{
				name: "väärtused",
				description: "on rahavoogude sarjad, mis vastavad maksegraafikule kuupäevades"
			},
			{
				name: "kuupäevad",
				description: "on maksepäevade loend, mis vastab rahavoo maksetele"
			},
			{
				name: "hinnang",
				description: "on arv, mis teie arvates on funktsiooni XIRR tulemile lähedane"
			}
		]
	},
	{
		name: "XNPV",
		description: "Tagastab rahavoogude sarja praeguse netoväärtuse.",
		arguments: [
			{
				name: "määr",
				description: "on rahavoogudele kohaldatav allahindlusmäär"
			},
			{
				name: "väärtused",
				description: "on rahavoogude sarjad, mis vastavad maksegraafikule kuupäevades"
			},
			{
				name: "kuupäevad",
				description: "on maksepäevade loend, mis vastab rahavoo maksetele"
			}
		]
	},
	{
		name: "XOR",
		description: "Tagastab kõigi argumentide loogikaväärtuse 'eksklusiivne või'.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "loogika1",
				description: "on 1–254 tingimust, mida soovite testida, mis võivad olla tõesed või väärad ja mis võivad olla loogikaväärtused, massiivid või viited"
			},
			{
				name: "loogika2",
				description: "on 1–254 tingimust, mida soovite testida, mis võivad olla tõesed või väärad ja mis võivad olla loogikaväärtused, massiivid või viited"
			}
		]
	},
	{
		name: "YEAR",
		description: "Tagastab aastaarvu, täisarvu vahemikus 1900–9999.",
		arguments: [
			{
				name: "järjenumber",
				description: "on arv Spreadsheeti kasutatavas kuupäeva-kellaaja koodis"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Tagastab alguskuupäeva ja lõppkuupäeva vahele jääva aastaosa suuruse täispäevade arvuna.",
		arguments: [
			{
				name: "alguskuupäev",
				description: "on kuupäeva järjenumber, mis tähistab alguskuupäeva"
			},
			{
				name: "lõppkuupäev",
				description: "on kuupäeva järjenumber, mis tähistab lõppkuupäeva"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Tagastab diskonteeritud väärtpaberi aastatootluse (nt riigivõlakirja puhul).",
		arguments: [
			{
				name: "tehing",
				description: "on väärtpaberi tehingukuupäev, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "tähtaeg",
				description: "on väärtpaberi maksetähtaeg, mis on väljendatud kuupäeva järjenumbrina"
			},
			{
				name: "hind",
				description: "on väärtpaberi hind 100-eurose nominaalväärtuse kohta"
			},
			{
				name: "tagastushind",
				description: "on väärtpaberi tagastusväärtus 100-eurose nominaalväärtuse kohta"
			},
			{
				name: "alus",
				description: "on kasutatava päevaarvestusaluse tüüp"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Ühepoolse z-testi tõenäosuse väärtus.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või andmevahemik, mille suhtes rakendatakse z-testi"
			},
			{
				name: "x",
				description: "on testi väärtus"
			},
			{
				name: "sigma",
				description: "on populatsiooni (teadaolev) standardhälve. Kui tühi, kasutatakse valimi standardhälvet"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Ühepoolse piiranguga z-testi tõenäosuse väärtus.",
		arguments: [
			{
				name: "massiiv",
				description: "on massiiv või andmevahemik, mille suhtes rakendatakse z-testi"
			},
			{
				name: "x",
				description: "on testi väärtus"
			},
			{
				name: "sigma",
				description: "on populatsiooni (teadaolev) standardhälve. Kui tühi, kasutatakse valimi standardhälvet"
			}
		]
	}
];