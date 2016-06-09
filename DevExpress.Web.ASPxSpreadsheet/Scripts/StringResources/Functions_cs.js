ASPxClientSpreadsheet.Functions = [
	{
		name: "A",
		description: "Ověří, zda mají všechny argumenty hodnotu PRAVDA, a v takovém případě vrátí hodnotu PRAVDA.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logická1",
				description: "je 1 až 255 testovaných podmínek, které mohou mít hodnotu PRAVDA nebo NEPRAVDA. Mohou to být logické hodnoty, matice nebo odkazy."
			},
			{
				name: "logická2",
				description: "je 1 až 255 testovaných podmínek, které mohou mít hodnotu PRAVDA nebo NEPRAVDA. Mohou to být logické hodnoty, matice nebo odkazy."
			}
		]
	},
	{
		name: "ABS",
		description: "Vrátí absolutní hodnotu čísla. Výsledek je číslo bez znaménka.",
		arguments: [
			{
				name: "číslo",
				description: "je reálné číslo, jehož absolutní hodnotu chcete zjistit."
			}
		]
	},
	{
		name: "ACCRINTM",
		description: "Vrátí nahromaděný úrok z cenného papíru, ze kterého je úrok placen k datu splatnosti.",
		arguments: [
			{
				name: "emise",
				description: "je datum emise cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "vypořádání",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "sazba",
				description: "je roční kupónová sazba cenného papíru."
			},
			{
				name: "nom_hodnota",
				description: "je nominální hodnota cenného papíru."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "ACOT",
		description: "Vrátí arkuskotangens čísla. Výsledek je v radiánech v rozsahu 0 až pí.",
		arguments: [
			{
				name: "číslo",
				description: "je kotangens požadovaného úhlu"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Vrátí inverzní hyperbolický kotangens čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je hyperbolický kotangens úhlu, který chcete získat"
			}
		]
	},
	{
		name: "ARABIC",
		description: "Převede římskou číslici na arabskou.",
		arguments: [
			{
				name: "text",
				description: "je římské číslo, které chcete převést."
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Vrátí hodnotu arkuskosinu čísla. Výsledek je v radiánech v rozsahu 0 až pí. Arkuskosinus je úhel, jehož kosinus je argument Číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota kosinu požadovaného úhlu, která musí ležet v intervalu -1 až 1."
			}
		]
	},
	{
		name: "ARCCOSH",
		description: "Vrátí hodnotu hyperbolického arkuskosinu čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo větší nebo rovné 1."
			}
		]
	},
	{
		name: "ARCSIN",
		description: "Vrátí arkussinus čísla. Výsledek je v radiánech v rozsahu -pí/2 až pí/2.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota sinu požadovaného úhlu, která musí ležet v intervalu -1 až 1."
			}
		]
	},
	{
		name: "ARCSINH",
		description: "Vrátí hyperbolický arkussinus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo větší nebo rovné 1."
			}
		]
	},
	{
		name: "ARCTG",
		description: "Vrátí arkustangens čísla. Výsledek je v radiánech v rozsahu -pí/2 až pí/2.",
		arguments: [
			{
				name: "číslo",
				description: "je tangens požadovaného úhlu."
			}
		]
	},
	{
		name: "ARCTG2",
		description: "Vrátí arkustangens zadané x-ové a y-ové souřadnice. Výsledek je v radiánech v rozsahu -pí až pí kromě hodnoty -pí.",
		arguments: [
			{
				name: "x_číslo",
				description: "je x-ová souřadnice bodu."
			},
			{
				name: "y_číslo",
				description: "je y-ová souřadnice bodu."
			}
		]
	},
	{
		name: "ARCTGH",
		description: "Vrátí hyperbolický arkustangens čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo mezi -1 a 1 kromě čísel -1 a 1."
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Vrátí průměrnou hodnotu (aritmetický průměr) argumentů. Text a logická hodnota NEPRAVDA mají hodnotu 0, logická hodnota PRAVDA má hodnotu 1. Argumenty mohou být čísla, názvy, matice nebo odkazy.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 argumentů, jejichž průměr chcete zjistit."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 argumentů, jejichž průměr chcete zjistit."
			}
		]
	},
	{
		name: "AVERAGEIF",
		description: "Zjistí průměrnou hodnotu (aritmetický průměr) buněk určených danou podmínkou nebo kritériem.",
		arguments: [
			{
				name: "oblast",
				description: "představuje oblast buněk, které chcete vyhodnotit."
			},
			{
				name: "kritérium",
				description: "je podmínka nebo kritérium v podobě čísla, výrazu nebo textu definujících buňky, jejichž průměr chcete zjistit."
			},
			{
				name: "oblast_pro_průměr",
				description: "jsou vlastní buňky, jejichž průměr bude zjištěn. Pokud parametr vynecháte, budou použity buňky v dané oblasti."
			}
		]
	},
	{
		name: "AVERAGEIFS",
		description: "Zjistí průměrnou hodnotu (aritmetický průměr) buněk určených danou sadou podmínek nebo kritérií.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "oblast_pro_průměr",
				description: "jsou vlastní buňky, jejichž průměr bude zjištěn."
			},
			{
				name: "oblast_kritérií",
				description: "představuje oblast buněk, které chcete vyhodnotit na základě určené podmínky."
			},
			{
				name: "kritérium",
				description: "je podmínka nebo kritérium v podobě čísla, výrazu nebo textu definujících buňky, jejichž průměr bude zjištěn."
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Převede číslo na text (baht).",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete převést"
			}
		]
	},
	{
		name: "BASE",
		description: "Převede číslo na textové vyjádření s danou číselnou základnou.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete převést."
			},
			{
				name: "radix",
				description: "je číselná základna, na kterou chcete číslo převést."
			},
			{
				name: "min_length",
				description: "je minimální délka vráceného řetězce. Není-li zadáno, nepřidají se počáteční nuly"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Vrátí modifikovanou Besselovu funkci In(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete funkci vyhodnotit."
			},
			{
				name: "n",
				description: "je řád Besselovy funkce."
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Vrátí Besselovu funkci Jn(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete funkci vyhodnotit."
			},
			{
				name: "n",
				description: "je řád Besselovy funkce."
			}
		]
	},
	{
		name: "BESSELK",
		description: "Vrátí modifikovanou Besselovu funkci Kn(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete funkci vyhodnotit."
			},
			{
				name: "n",
				description: "je řád funkce."
			}
		]
	},
	{
		name: "BESSELY",
		description: "Vrátí Besselovu funkci Yn(x).",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete funkci vyhodnotit."
			},
			{
				name: "n",
				description: "je řád funkce."
			}
		]
	},
	{
		name: "BETA.DIST",
		description: "Vrátí funkci rozdělení pravděpodobnosti beta.",
		arguments: [
			{
				name: "x",
				description: "je hodnota mezi hodnotami argumentů A a B, pro kterou chcete zjistit hodnotu funkce."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA; funkce hustoty pravděpodobnosti = NEPRAVDA."
			},
			{
				name: "A",
				description: "je nepovinná dolní mez intervalu hodnot x. Jestliže argument A nezadáte, bude jeho hodnota 0."
			},
			{
				name: "B",
				description: "je nepovinná horní mez intervalu hodnot x. Jestliže argument B nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Vrátí inverzní hodnotu kumulativní funkce hustoty pravděpodobnosti beta rozdělení (BETA.DIST).",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost rozdělení beta."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "A",
				description: "je nepovinná dolní mez intervalu hodnot x. Jestliže argument A nezadáte, bude jeho hodnota 0."
			},
			{
				name: "B",
				description: "je nepovinná horní mez intervalu hodnot x. Jestliže argument B nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "BETADIST",
		description: "Vrátí hodnotu kumulativní funkce hustoty pravděpodobnosti beta rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota mezi hodnotami argumentů A a B, pro kterou chcete zjistit hodnotu funkce."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "A",
				description: "je volitelná dolní mez intervalu hodnot x. Jestliže argument A nezadáte, bude jeho hodnota 0."
			},
			{
				name: "B",
				description: "je volitelná horní mez intervalu hodnot x. Jestliže argument B nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "BETAINV",
		description: "Vrátí inverzní hodnotu kumulativní funkce hustoty pravděpodobnosti beta rozdělení (inverzní funkce k funkci BETADIST).",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost beta rozdělení."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, který musí být větší než 0."
			},
			{
				name: "A",
				description: "je volitelná dolní mez intervalu hodnot x. Jestliže argument A nezadáte, bude jeho hodnota 0."
			},
			{
				name: "B",
				description: "je volitelná horní mez intervalu hodnot x. Jestliže argument B nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "BIN2DEC",
		description: "Převede binární číslo na dekadické.",
		arguments: [
			{
				name: "číslo",
				description: "je binární číslo, které chcete převést."
			}
		]
	},
	{
		name: "BIN2HEX",
		description: "Převede binární číslo na hexadecimální.",
		arguments: [
			{
				name: "číslo",
				description: "je binární číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "BIN2OCT",
		description: "Převede binární číslo na osmičkové.",
		arguments: [
			{
				name: "číslo",
				description: "je binární číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "BINOM.DIST",
		description: "Vrátí hodnotu binomického rozdělení pravděpodobnosti jednotlivých veličin.",
		arguments: [
			{
				name: "počet_úspěchů",
				description: "je počet úspěšných pokusů."
			},
			{
				name: "pokusy",
				description: "je počet nezávislých pokusů."
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost každého úspěšného pokusu."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, hromadná pravděpodobnostní funkce = NEPRAVDA."
			}
		]
	},
	{
		name: "BINOM.DIST.RANGE",
		description: "Vrátí pravděpodobnost výsledku pokusu pomocí binomického rozdělení.",
		arguments: [
			{
				name: "pokusy",
				description: "je počet nezávislých pokusů"
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost úspěchu v každém pokusu"
			},
			{
				name: "počet_úspěchů",
				description: "je počet úspěchů v pokusech"
			},
			{
				name: "počet_úspěchů2",
				description: "pokud je zadaná, vrátí tato funkce pravděpodobnost, že počet úspěšných pokusů bude ležet mezi hodnotami počet_úspěchů a počet_úspěchů2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Vrátí nejmenší hodnotu, pro kterou má kumulativní binomické rozdělení hodnotu větší nebo rovnu hodnotě kritéria.",
		arguments: [
			{
				name: "pokusy",
				description: "je počet Bernoulliho pokusů."
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost každého úspěšného pokusu. Argument je číslo mezi 0 a 1 včetně."
			},
			{
				name: "alfa",
				description: "je hodnota kritéria, číslo mezi 0 a 1 včetně."
			}
		]
	},
	{
		name: "BINOMDIST",
		description: "Vrátí hodnotu binomického rozdělení pravděpodobnosti jednotlivých veličin.",
		arguments: [
			{
				name: "počet_úspěchů",
				description: "je počet úspěšných pokusů."
			},
			{
				name: "pokusy",
				description: "je počet nezávislých pokusů."
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost každého úspěšného pokusu."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, hromadná pravděpodobnostní funkce = NEPRAVDA."
			}
		]
	},
	{
		name: "BITAND",
		description: "Vrátí bitové „A“ dvou čísel.",
		arguments: [
			{
				name: "číslo1",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			},
			{
				name: "číslo2",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			}
		]
	},
	{
		name: "BITLSHIFT",
		description: "Vrátí číslo posunuté doleva o hodnotu velikost_posunu v bitech.",
		arguments: [
			{
				name: "číslo",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			},
			{
				name: "velikost_posunu",
				description: "je počet bitů, o které chcete číslo posunout doleva"
			}
		]
	},
	{
		name: "BITOR",
		description: "Vrátí bitové „nebo“ ze dvou čísel.",
		arguments: [
			{
				name: "číslo1",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			},
			{
				name: "číslo2",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			}
		]
	},
	{
		name: "BITRSHIFT",
		description: "Vrátí číslo posunuté doprava o hodnotu velikost_posunu v bitech.",
		arguments: [
			{
				name: "číslo",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			},
			{
				name: "velikost_posunu",
				description: "je počet bitů, o které chcete číslo posunout doprava"
			}
		]
	},
	{
		name: "BITXOR",
		description: "Vrátí bitové „výhradní nebo“ ze dvou čísel.",
		arguments: [
			{
				name: "číslo1",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			},
			{
				name: "číslo2",
				description: "je desítkové vyjádření binárního čísla, které chcete vyhodnotit"
			}
		]
	},
	{
		name: "BUDHODNOTA",
		description: "Vrátí budoucí hodnotu investice vypočtenou na základě pravidelných konstantních splátek a konstantní úrokové sazby.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba vztažená na jedno období. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4."
			},
			{
				name: "pper",
				description: "je celkový počet platebních období investice."
			},
			{
				name: "splátka",
				description: "je platba provedená v každém období. Po dobu životnosti investice ji nelze měnit."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota nebo celková částka určující současnou hodnotu série budoucích plateb. Jestliže argument Souč_hod nezadáte, bude jeho hodnota rovna 0."
			},
			{
				name: "typ",
				description: "je hodnota, která představuje termín splátky: splátka na začátku období = 1, splátka na konci období = 0 nebo bez zadání."
			}
		]
	},
	{
		name: "ČAS",
		description: "Převede hodiny, minuty a sekundy zadané jako čísla na pořadové číslo aplikace Spreadsheet formátované pomocí formátu času.",
		arguments: [
			{
				name: "hodina",
				description: "je číslo od 0 do 23, které představuje hodiny."
			},
			{
				name: "minuta",
				description: "je číslo od 0 do 59, které představuje minuty."
			},
			{
				name: "sekunda",
				description: "je číslo od 0 do 59, které představuje sekundy."
			}
		]
	},
	{
		name: "ČASHODN",
		description: "Převede čas ve formě textového řetězce na pořadové číslo aplikace Spreadsheet vyjadřující čas, číslo od 0 (12:00:00 dop.) do 0,999988426 (11:59:59 odp.). Po zadání vzorce číslo zformátujte pomocí formátu času.",
		arguments: [
			{
				name: "čas",
				description: "je textový řetězec, který představuje čas v libovolném formátu času aplikace Spreadsheet (informace o datech v řetězci jsou přeskočeny)."
			}
		]
	},
	{
		name: "ČÁST",
		description: "Vrátí znaky z textového řetězce, je-li zadána počáteční pozice a počet znaků.",
		arguments: [
			{
				name: "text",
				description: "je textový řetězec, ze kterého chcete extrahovat požadované znaky."
			},
			{
				name: "start",
				description: "je pozice prvního znaku, který chcete extrahovat. První znak argumentu Text je číslo 1."
			},
			{
				name: "počet_znaků",
				description: "určí počet znaků argumentu Text, které mají být extrahovány."
			}
		]
	},
	{
		name: "CEILING.MATH",
		description: "Zaokrouhlí číslo nahoru na nejbližší celé číslo nebo na nejbližší násobek zadané hodnoty významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit"
			},
			{
				name: "významnost",
				description: "je násobek, na který chcete číslo zaokrouhlit"
			},
			{
				name: "režim",
				description: "pokud je zadaná a hodnota není rovná nula, bude tato funkce zaokrouhlovat směrem od nuly"
			}
		]
	},
	{
		name: "CEILING.PRECISE",
		description: "Zaokrouhlí číslo nahoru na nejbližší celé číslo nebo na nejbližší násobek zadané hodnoty významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit."
			},
			{
				name: "významnost",
				description: "je násobek, na který chcete číslo zaokrouhlit."
			}
		]
	},
	{
		name: "CELÁ.ČÁST",
		description: "Zaokrouhlí číslo dolů na nejbližší celé číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je reálné číslo, které chcete zaokrouhlit dolů na celé číslo."
			}
		]
	},
	{
		name: "ČETNOSTI",
		description: "Vypočte počet výskytů hodnot v oblasti hodnot a vrátí vertikální matici čísel, která má o jeden prvek více než argument Hodnoty.",
		arguments: [
			{
				name: "data",
				description: "je matice nebo odkaz na množinu hodnot, pro které chcete zjistit počet výskytů (prázdné buňky a text jsou přeskočeny)."
			},
			{
				name: "hodnoty",
				description: "je matice nebo odkaz na intervaly, do kterých chcete seskupit hodnoty argumentu Data."
			}
		]
	},
	{
		name: "CHIDIST",
		description: "Vrátí pravděpodobnost (pravý chvost) chí-kvadrát rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (nezáporné číslo), pro kterou chcete zjistit pravděpodobnost rozdělení."
			},
			{
				name: "volnost",
				description: "je počet stupňů volnosti. Argument je číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "CHIINV",
		description: "Vrátí inverzní hodnotu pravděpodobnosti (pravý chvost) chí-kvadrát rozdělení.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost chí-kvadrát rozdělení. Argument je číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "volnost",
				description: "je počet stupňů volnosti. Argument je číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "CHISQ.DIST",
		description: "Vrátí levostrannou pravděpodobnost rozdělení chí-kvadrát.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit pravděpodobnost rozdělení. Argument musí být nezáporné číslo."
			},
			{
				name: "volnost",
				description: "je počet stupňů volnosti. Argument je číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota, kterou funkce vrátí: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "CHISQ.DIST.RT",
		description: "Vrátí pravostrannou pravděpodobnost rozdělení chí-kvadrát.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit pravděpodobnost rozdělení. Argument musí být nezáporné číslo."
			},
			{
				name: "volnost",
				description: "je počet stupňů volnosti. Argument je číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "CHISQ.INV",
		description: "Vrátí hodnotu funkce inverzní k distribuční funkci levostranné pravděpodobnosti rozdělení chí-kvadrát.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost rozdělení chí-kvadrát. Argument je hodnota z uzavřeného intervalu 0 až 1."
			},
			{
				name: "volnost",
				description: "je počet stupňů volnosti. Argument je číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "CHISQ.INV.RT",
		description: "Vrátí hodnotu funkce inverzní k distribuční funkci pravostranné pravděpodobnosti rozdělení chí-kvadrát.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost rozdělení chí-kvadrát. Argument je hodnota z uzavřeného intervalu 0 až 1."
			},
			{
				name: "volnost",
				description: "je počet stupňů volnosti. Argument je číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "CHISQ.TEST",
		description: "Vrátí test nezávislosti: hodnota ze statistického rozdělení chí-kvadrát a příslušné stupně volnosti.",
		arguments: [
			{
				name: "aktuální",
				description: "je oblast dat obsahující pozorování, které chcete testovat a srovnávat s předpokládanými hodnotami."
			},
			{
				name: "očekávané",
				description: "je oblast dat obsahující podíl součinu součtů řádků a sloupců a celkového součtu."
			}
		]
	},
	{
		name: "CHITEST",
		description: "Vrátí test nezávislosti: hodnota chí-kvadrát rozdělení pro statistické jednotky a příslušné stupně volnosti.",
		arguments: [
			{
				name: "aktuální",
				description: "je oblast dat obsahující pozorování, která chcete testovat a srovnávat s předpokládanými hodnotami."
			},
			{
				name: "očekávané",
				description: "je oblast dat obsahující podíl součinu součtů řádků a sloupců a celkového součtu."
			}
		]
	},
	{
		name: "CHYBA.TYP",
		description: "Vrátí číslo odpovídající chybové hodnotě.",
		arguments: [
			{
				name: "chyba",
				description: "je chybová hodnota, pro kterou chcete zjistit identifikační číslo. Může to být aktuální chybová hodnota nebo odkaz na buňku obsahující chybovou hodnotu."
			}
		]
	},
	{
		name: "ČISTÁ.SOUČHODNOTA",
		description: "Vrátí čistou současnou hodnotu investice vypočítanou na základě diskontní sazby a série budoucích plateb (záporné hodnoty) a příjmů (kladné hodnoty).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sazba",
				description: "je diskontní sazba vztažená na délku jednoho období."
			},
			{
				name: "hodnota1",
				description: "je 1 až 254 plateb a příjmů rovnoměrně rozdělených v čase a vyskytujících se na konci každého období."
			},
			{
				name: "hodnota2",
				description: "je 1 až 254 plateb a příjmů rovnoměrně rozdělených v čase a vyskytujících se na konci každého období."
			}
		]
	},
	{
		name: "COMBINA",
		description: "Vrátí počet kombinací s opakováním pro daný počet položek.",
		arguments: [
			{
				name: "počet",
				description: "je celkový počet položek"
			},
			{
				name: "kombinace",
				description: "je počet položek v každé kombinaci"
			}
		]
	},
	{
		name: "COMPLEX",
		description: "Převede reálnou a imaginární část na komplexní číslo.",
		arguments: [
			{
				name: "reál",
				description: "je reálná část komplexního čísla."
			},
			{
				name: "imag",
				description: "je imaginární část komplexního čísla."
			},
			{
				name: "přípona",
				description: "je označení imaginární části komplexního čísla."
			}
		]
	},
	{
		name: "CONCATENATE",
		description: "Sloučí několik textových řetězců do jednoho.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "je 1 až 255 textových řetězců, které chcete sloučit do jediného textového řetězce. Mohou to být textové řetězce, čísla nebo odkazy na jedinou buňku."
			},
			{
				name: "text2",
				description: "je 1 až 255 textových řetězců, které chcete sloučit do jediného textového řetězce. Mohou to být textové řetězce, čísla nebo odkazy na jedinou buňku."
			}
		]
	},
	{
		name: "CONFIDENCE",
		description: "Vrátí interval spolehlivosti pro střední hodnotu základního souboru pomocí normálního rozdělení.",
		arguments: [
			{
				name: "alfa",
				description: "je hladina významnosti, pomocí které je vypočítána hladina spolehlivosti. Argument je číslo větší než 0 a menší než 1."
			},
			{
				name: "sm_odch",
				description: "je známá směrodatná odchylka základního souboru pro oblast dat. Argument sm_odch musí být větší než 0."
			},
			{
				name: "velikost",
				description: "je velikost výběru."
			}
		]
	},
	{
		name: "CONFIDENCE.NORM",
		description: "Vrátí interval spolehlivosti pro střední hodnotu základního souboru pomocí normálního rozdělení.",
		arguments: [
			{
				name: "alfa",
				description: "je hladina významnosti, pomocí které je vypočítána hladina spolehlivosti. Argument je číslo větší než 0 a menší než 1."
			},
			{
				name: "sm_odch",
				description: "je známá směrodatná odchylka základního souboru pro oblast dat. Argument sm_odch musí být větší než 0."
			},
			{
				name: "velikost",
				description: "je velikost výběru."
			}
		]
	},
	{
		name: "CONFIDENCE.T",
		description: "Vrátí interval spolehlivosti pro střední hodnotu základního souboru pomocí Studentova t-rozdělení.",
		arguments: [
			{
				name: "alfa",
				description: "je hladina významnosti, pomocí které je vypočítána hladina spolehlivosti. Argument je číslo větší než 0 a menší než 1."
			},
			{
				name: "sm_odch",
				description: "je známá směrodatná odchylka základního souboru pro oblast dat. Argument sm_odch musí být větší než 0."
			},
			{
				name: "velikost",
				description: "je velikost výběru."
			}
		]
	},
	{
		name: "CONVERT",
		description: "Převede číslo do jiného jednotkového měrného systému.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota v jednotkách argumentu z, kterou chcete převést."
			},
			{
				name: "z",
				description: "jsou jednotky čísla."
			},
			{
				name: "do",
				description: "jsou jednotky výsledku."
			}
		]
	},
	{
		name: "CORREL",
		description: "Vrátí korelační koeficient mezi dvěma množinami dat.",
		arguments: [
			{
				name: "matice1",
				description: "je oblast buněk s hodnotami. Hodnoty mohou být čísla, názvy, matice nebo odkazy obsahující čísla."
			},
			{
				name: "matice2",
				description: "je druhá oblast buněk s hodnotami. Hodnoty mohou být čísla, názvy, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "COS",
		description: "Vrátí kosinus úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, jehož kosinus chcete určit."
			}
		]
	},
	{
		name: "COSH",
		description: "Vrátí hyperbolický kosinus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo."
			}
		]
	},
	{
		name: "COT",
		description: "Vrátí kotangens úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, pro který chcete získat kotangens"
			}
		]
	},
	{
		name: "COTH",
		description: "Vrátí hyperbolický kotangens čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, pro který chcete získat hyperbolický kotangens"
			}
		]
	},
	{
		name: "COUNTBLANK",
		description: "Vrátí počet prázdných buněk v zadané oblasti buněk.",
		arguments: [
			{
				name: "oblast",
				description: "je oblast buněk, ve které chcete spočítat prázdné buňky."
			}
		]
	},
	{
		name: "COUNTIF",
		description: "Vrátí počet buněk v zadané oblasti, které splňují požadované kritérium.",
		arguments: [
			{
				name: "oblast",
				description: "je oblast buněk, ve které chcete spočítat neprázdné buňky."
			},
			{
				name: "kritérium",
				description: "jsou kritéria ve formě čísla, výrazu nebo textu definující buňky, které budou spočítány."
			}
		]
	},
	{
		name: "COUNTIFS",
		description: "Určí počet buněk na základě dané sady podmínek nebo kritérií.",
		arguments: [
			{
				name: "oblast_kritérií",
				description: "představuje oblast buněk, které chcete vyhodnotit na základě určené podmínky."
			},
			{
				name: "kritérium",
				description: "je podmínka v podobě čísla, výrazu nebo textu definující buňky, jejichž počet bude určen."
			}
		]
	},
	{
		name: "COUPDAYBS",
		description: "Vrátí počet dnů od začátku období placení kupónů do data splatnosti.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "počet_plateb",
				description: "je počet kupónových plateb v roce."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "COUPNCD",
		description: "Vrátí následující datum placení kupónu po datu zúčtování.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "počet_plateb",
				description: "je počet kupónových plateb v roce."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "COUPNUM",
		description: "Vrátí počet kupónů splatných mezi datem zúčtování a datem splatnosti.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "počet_plateb",
				description: "je počet kupónových plateb v roce."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "COUPPCD",
		description: "Vrátí předchozí datum placení kupónu před datem zúčtování.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "počet_plateb",
				description: "je počet kupónových plateb v roce."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "COVAR",
		description: "Vrátí hodnotu kovariance, průměrnou hodnotu součinů odchylek pro každou dvojici datových bodů ve dvou množinách dat.",
		arguments: [
			{
				name: "matice1",
				description: "je první oblast buněk obsahující celá čísla. Hodnoty argumentu musí být čísla, matice nebo odkazy obsahující čísla."
			},
			{
				name: "matice2",
				description: "je druhá oblast buněk obsahující celá čísla. Hodnoty argumentu musí být čísla, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "COVARIANCE.P",
		description: "Vrátí hodnotu kovariance základního souboru, průměrnou hodnotu součinů odchylek pro každou dvojici datových bodů ve dvou množinách dat.",
		arguments: [
			{
				name: "matice1",
				description: "je první oblast buněk obsahující celá čísla. Hodnoty argumentu musí být čísla, matice nebo odkazy obsahující čísla."
			},
			{
				name: "matice2",
				description: "je druhá oblast buněk obsahující celá čísla. Hodnoty argumentu musí být čísla, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "COVARIANCE.S",
		description: "Vrátí hodnotu kovariance výběru, průměrnou hodnotu součinů odchylek pro každou dvojici datových bodů ve dvou množinách dat.",
		arguments: [
			{
				name: "matice1",
				description: "je první oblast buněk obsahující celá čísla. Hodnoty argumentu musí být čísla, matice nebo odkazy obsahující čísla."
			},
			{
				name: "matice2",
				description: "je druhá oblast buněk obsahující celá čísla. Hodnoty argumentu musí být čísla, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "CRITBINOM",
		description: "Vrátí nejmenší hodnotu, pro kterou má kumulativní binomické rozdělení hodnotu větší nebo rovnu hodnotě kritéria.",
		arguments: [
			{
				name: "pokusy",
				description: "je počet Bernoulliho pokusů."
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost každého úspěšného pokusu. Argument je číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "alfa",
				description: "je hodnota kritéria, číslo mezi 0 a 1 (včetně)."
			}
		]
	},
	{
		name: "CSC",
		description: "Vrátí kosekans úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, pro který chcete získat kosekans"
			}
		]
	},
	{
		name: "CSCH",
		description: "Vrátí hyperbolický kosekans úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, pro který chcete získat hyperbolický kosekans"
			}
		]
	},
	{
		name: "CUMIPMT",
		description: "Vrátí kumulativní úrok splacený mezi dvěma obdobími.",
		arguments: [
			{
				name: "úrok",
				description: "je úroková sazba."
			},
			{
				name: "období",
				description: "je celkový počet platebních období."
			},
			{
				name: "půjčka",
				description: "je současná hodnota."
			},
			{
				name: "začátek",
				description: "je počáteční období ve výpočtu."
			},
			{
				name: "konec",
				description: "je koncové období ve výpočtu."
			},
			{
				name: "typ",
				description: "je časování plateb."
			}
		]
	},
	{
		name: "CUMPRINC",
		description: "Vrátí kumulativní jistinu splacenou mezi dvěma obdobími půjčky.",
		arguments: [
			{
				name: "úrok",
				description: "je úroková sazba."
			},
			{
				name: "období",
				description: "je celkový počet platebních období."
			},
			{
				name: "půjčka",
				description: "je současná hodnota."
			},
			{
				name: "začátek",
				description: "je počáteční období ve výpočtu."
			},
			{
				name: "konec",
				description: "je koncové období ve výpočtu."
			},
			{
				name: "typ",
				description: "je časování plateb."
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
		name: "DATUM",
		description: "Vrátí číslo, které představuje datum v kódu aplikace Spreadsheet pro datum a čas.",
		arguments: [
			{
				name: "rok",
				description: "je číslo od 1900 do 9999 v aplikaci Spreadsheet pro Windows nebo číslo od 1904 do 9999 v aplikaci Spreadsheet pro Macintosh."
			},
			{
				name: "měsíc",
				description: "je číslo od 1 do 12 představující měsíc v roce."
			},
			{
				name: "den",
				description: "je číslo od 1 do 31 představující den v měsíci."
			}
		]
	},
	{
		name: "DATUMHODN",
		description: "Převede datum ve formátu textu na číslo, které představuje datum v kódu aplikace Spreadsheet pro datum a čas.",
		arguments: [
			{
				name: "datum",
				description: "je text, který představuje datum ve formátu data aplikace Spreadsheet v rozsahu od 1/1/1900 (pro Windows) nebo 1/1/1904 (pro Macintosh) do 12/31/9999."
			}
		]
	},
	{
		name: "DAYS",
		description: "Vrátí počet dní mezi dvěma daty.",
		arguments: [
			{
				name: "konec",
				description: "začátek a konec jsou dvě kaledářní data, mezi kterými chcete zjistit počet dní"
			},
			{
				name: "začátek",
				description: "začátek a konec jsou dvě kalendářní data, mezi kterými chcete zjistit počet dní"
			}
		]
	},
	{
		name: "DEC2BIN",
		description: "Převede dekadické číslo na binární.",
		arguments: [
			{
				name: "číslo",
				description: "je dekadické celé číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "DEC2HEX",
		description: "Převede dekadické číslo na hexadecimální.",
		arguments: [
			{
				name: "číslo",
				description: "je dekadické celé číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "DEC2OCT",
		description: "Převede dekadické číslo na osmičkové.",
		arguments: [
			{
				name: "číslo",
				description: "je dekadické celé číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Převede textové vyjádření čísla v daném základu na desítkové číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete převést"
			},
			{
				name: "základ",
				description: "je základ číselné soustavy čísla, které převádíte"
			}
		]
	},
	{
		name: "DEGREES",
		description: "Převede radiány na stupně.",
		arguments: [
			{
				name: "úhel",
				description: "je úhel v radiánech, který chcete převést."
			}
		]
	},
	{
		name: "DÉLKA",
		description: "Vrátí počet znaků textového řetězce.",
		arguments: [
			{
				name: "text",
				description: "je text, jehož délku chcete zjistit. Mezery jsou počítány jako znaky."
			}
		]
	},
	{
		name: "DELTA",
		description: "Testuje rovnost dvou čísel.",
		arguments: [
			{
				name: "číslo1",
				description: "je první číslo."
			},
			{
				name: "číslo2",
				description: "je druhé číslo."
			}
		]
	},
	{
		name: "DEN",
		description: "Vrátí den v měsíci, číslo od 1 do 31.",
		arguments: [
			{
				name: "pořadové_číslo",
				description: "je číslo v kódu aplikace Spreadsheet pro datum a čas."
			}
		]
	},
	{
		name: "DENTÝDNE",
		description: "Vrátí číslo od 1 do 7 určující den v týdnu kalendářního data.",
		arguments: [
			{
				name: "pořadové",
				description: "je číslo, které představuje datum"
			},
			{
				name: "typ",
				description: "je číslo: pro týden od neděle = 1 do soboty = 7 použijte číslo 1, pro týden od pondělka = 1 do neděle = 7 použijte číslo 2, pro týden od pondělka = 0 do neděle = 6 použijte číslo 3"
			}
		]
	},
	{
		name: "DETERMINANT",
		description: "Vrátí determinant matice.",
		arguments: [
			{
				name: "pole",
				description: "je číselná matice se stejným počtem řádků a sloupců. Může to být oblast buněk nebo maticová konstanta."
			}
		]
	},
	{
		name: "DEVSQ",
		description: "Vrátí součet druhých mocnin odchylek datových bodů od jejich střední hodnoty výběru.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 argumentů nebo matice či odkaz na matici, pro které chcete vypočítat výsledek funkce DEVSQ."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 argumentů nebo matice či odkaz na matici, pro které chcete vypočítat výsledek funkce DEVSQ."
			}
		]
	},
	{
		name: "DISC",
		description: "Vrátí diskontní sazbu cenného papíru.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "cena",
				description: "je cena cenného papíru o nominální hodnotě 100 Kč."
			},
			{
				name: "zaruč_cena",
				description: "je výkupní hodnota cenného papíru o nominální hodnotě 100 Kč."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "DMAX",
		description: "Vrátí maximální hodnotu v poli (sloupci) záznamů databáze, která splňuje zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DMIN",
		description: "Vrátí minimální hodnotu v poli (sloupci) záznamů databáze, která splňuje zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DNES",
		description: "Vrátí aktuální datum formátované jako datum.",
		arguments: [
		]
	},
	{
		name: "DOLLARDE",
		description: "Převede částku v korunách vyjádřenou zlomkem na částku v korunách vyjádřenou desetinným číslem.",
		arguments: [
			{
				name: "zlomková_koruna",
				description: "je číslo vyjádřené zlomkem."
			},
			{
				name: "zlomek",
				description: "je celé číslo, které chcete použít jako jmenovatel zlomku."
			}
		]
	},
	{
		name: "DOLLARFR",
		description: "Převede částku v korunách vyjádřenou desetinným číslem na částku v korunách vyjádřenou zlomkem.",
		arguments: [
			{
				name: "desetinná_koruna",
				description: "je desetinné číslo."
			},
			{
				name: "zlomek",
				description: "je celé číslo, které chcete použít jako jmenovatel zlomku."
			}
		]
	},
	{
		name: "DOSADIT",
		description: "Nahradí existující text novým textem v textovém řetězci.",
		arguments: [
			{
				name: "text",
				description: "je text nebo odkaz na buňku obsahující text, ve kterém chcete zaměnit znaky."
			},
			{
				name: "starý",
				description: "je existující text, který chcete zaměnit. Jestliže malá a velká písmena v textu argumentu Starý nejsou shodná s malými a velkými písmeny v textu argumentu Text, funkce DOSADIT text nezamění."
			},
			{
				name: "nový",
				description: "je text, kterým chcete nahradit text argumentu Starý."
			},
			{
				name: "instance",
				description: "určuje, který výskyt textu argumentu Starý chcete nahradit. Jestliže tento argument nezadáte, bude nahrazen každý výskyt textu argumentu Starý."
			}
		]
	},
	{
		name: "DPOČET",
		description: "Vrátí počet buněk obsahujících čísla v poli (sloupci) záznamů databáze, které splňují zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DPOČET2",
		description: "Vrátí počet neprázdných buněk v poli (sloupci) záznamů databáze, které splňují zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DPRŮMĚR",
		description: "Vrátí průměr hodnot ve sloupci seznamu nebo databáze, které splňují zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DSMODCH",
		description: "Vrátí směrodatnou odchylku základního souboru vybraných položek databáze.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DSMODCH.VÝBĚR",
		description: "Odhadne směrodatnou odchylku výběru vybraných položek databáze.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DSOUČIN",
		description: "Vynásobí hodnoty v poli (sloupci) záznamů databáze, které splňují zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DSUMA",
		description: "Sečte čísla v poli (sloupci) záznamů databáze, které splňují zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DVAR",
		description: "Vrátí rozptyl základního souboru vybraných položek databáze.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DVAR.VÝBĚR",
		description: "Odhadne rozptyl výběru vybraných položek databáze.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "DZÍSKAT",
		description: "Vybere z databáze jeden záznam, který splňuje zadaná kritéria.",
		arguments: [
			{
				name: "databáze",
				description: "je oblast buněk, která tvoří seznam nebo databázi. Databáze je seznam vzájemně propojených dat."
			},
			{
				name: "pole",
				description: "je popisek sloupce v uvozovkách nebo číslo, které představuje umístění sloupce v seznamu."
			},
			{
				name: "kritéria",
				description: "je oblast buněk, která obsahuje zadaná kritéria. Oblast zahrnuje popisek sloupce a jednu buňku pod popiskem pro kritérium."
			}
		]
	},
	{
		name: "EDATE",
		description: "Vrátí pořadové číslo data, což je určený počet měsíců před nebo po počátečním datu.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "měsíce",
				description: "je počet měsíců před nebo po počátečním datu zadaném v argumentu Začátek."
			}
		]
	},
	{
		name: "EFFECT",
		description: "Vrátí efektivní roční úrokovou sazbu.",
		arguments: [
			{
				name: "úrok",
				description: "je nominální úroková sazba."
			},
			{
				name: "období",
				description: "je počet úročených období za rok."
			}
		]
	},
	{
		name: "ENCODEURL",
		description: "Vrátí řetězec zakódovaný do adresy URL.",
		arguments: [
			{
				name: "text",
				description: "je řetězec určený k zakódování do adresy URL"
			}
		]
	},
	{
		name: "EOMONTH",
		description: "Vrátí pořadové číslo posledního dne měsíce před nebo po určeném počtu měsíců.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "měsíce",
				description: "je počet měsíců před nebo po počátečním datu zadaném v argumentu Začátek."
			}
		]
	},
	{
		name: "ERF",
		description: "Vrátí chybovou funkci.",
		arguments: [
			{
				name: "dolní_limit",
				description: "je dolní mez pro integraci funkce ERF."
			},
			{
				name: "horní_limit",
				description: "je horní mez pro integraci funkce ERF."
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Vrátí chybovou funkci.",
		arguments: [
			{
				name: "x",
				description: "je dolní mez pro integraci funkce ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC",
		description: "Vrátí doplňkovou chybovou funkci.",
		arguments: [
			{
				name: "x",
				description: "je dolní mez pro integraci funkce ERF."
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Vrátí doplňkovou chybovou funkci.",
		arguments: [
			{
				name: "x",
				description: "je dolní mez pro integraci funkce ERFC.PRECISE"
			}
		]
	},
	{
		name: "EXP",
		description: "Vrátí základ přirozeného logaritmu umocněný na zadané číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je exponent použitý u základu e. Konstanta e je základ přirozeného logaritmu a je rovna hodnotě 2,71828182845904."
			}
		]
	},
	{
		name: "EXPON.DIST",
		description: "Vrátí hodnotu exponenciálního rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota funkce, nezáporné číslo."
			},
			{
				name: "lambda",
				description: "je hodnota parametru, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota pro funkci, kterou chcete vrátit: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "EXPONDIST",
		description: "Vrátí hodnotu exponenciálního rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota funkce, nezáporné číslo."
			},
			{
				name: "lambda",
				description: "je hodnota parametru, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota pro funkci, kterou chcete vrátit: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "F.DIST",
		description: "Vrátí hodnotu (levostranného) rozdělení pravděpodobnosti F (stupeň nonekvivalence) pro dvě množiny dat.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit rozdělení pravděpodobnosti. Argument musí být nezáporné číslo."
			},
			{
				name: "volnost1",
				description: "je počet stupňů volnosti v čitateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "volnost2",
				description: "je počet stupňů volnosti ve jmenovateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota, kterou funkce vrátí: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "F.DIST.RT",
		description: "Vrátí hodnotu (pravostranného) rozdělení pravděpodobnosti F (stupeň nonekvivalence) pro dvě množiny dat.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit rozdělení pravděpodobnosti. Argument musí být nezáporné číslo."
			},
			{
				name: "volnost1",
				description: "je počet stupňů volnosti v čitateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "volnost2",
				description: "je počet stupňů volnosti ve jmenovateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "F.INV",
		description: "Vrátí hodnotu inverzní funkce k distribuční funkci (levostranného) rozdělení pravděpodobnosti F: jestliže p = F.DIST(x,...), F.INV(p,...) = x.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost kumulativního rozdělení F, číslo mezi 0 a 1 včetně."
			},
			{
				name: "volnost1",
				description: "je počet stupňů volnosti v čitateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "volnost2",
				description: "je počet stupňů volnosti ve jmenovateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Vrátí hodnotu inverzní funkce k distribuční funkci (pravostranného) rozdělení pravděpodobnosti F: jestliže p = F.DIST.RT(x,...), F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost kumulativního rozdělení F, číslo mezi 0 a 1 včetně."
			},
			{
				name: "volnost1",
				description: "je počet stupňů volnosti v čitateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "volnost2",
				description: "je počet stupňů volnosti ve jmenovateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "F.TEST",
		description: "Vrátí výsledek F-testu, tj. pravděpodobnosti (dva chvosty), že se rozptyly v argumentech matice1 a matice2 výrazně neliší.",
		arguments: [
			{
				name: "matice1",
				description: "je první matice nebo oblast dat. Hodnoty argumentu mohou být čísla, matice nebo odkazy obsahující čísla. (Prázdné buňky jsou ignorovány.)"
			},
			{
				name: "matice2",
				description: "je druhá matice nebo oblast dat. Hodnoty argumentu mohou být čísla, matice nebo odkazy obsahující čísla. (Prázdné buňky jsou ignorovány.)"
			}
		]
	},
	{
		name: "FACTDOUBLE",
		description: "Vrátí dvojitý faktoriál čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, jejíž dvojitý faktoriál chcete vypočítat."
			}
		]
	},
	{
		name: "FAKTORIÁL",
		description: "Vrátí faktoriál čísla. Výsledek se rovná hodnotě 1*2*3*...*Číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je nezáporné číslo, jehož faktoriál chcete vypočítat."
			}
		]
	},
	{
		name: "FDIST",
		description: "Vrátí hodnotu F rozdělení (stupeň nonekvivalence) pravděpodobnosti (pravý chvost) pro dvě množiny dat.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (nezáporné číslo), pro kterou chcete vypočítat danou funkci."
			},
			{
				name: "volnost1",
				description: "je počet stupňů volnosti v čitateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "volnost2",
				description: "je počet stupňů volnosti ve jmenovateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
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
		name: "FINV",
		description: "Vrátí hodnotu inverzní funkce k funkci F rozdělení pravděpodobnosti (pravý chvost): Jestliže p = FDIST(x,...), pak FINV(p,...) = x.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost kumulativního F rozdělení, číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "volnost1",
				description: "je počet stupňů volnosti v čitateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			},
			{
				name: "volnost2",
				description: "je počet stupňů volnosti ve jmenovateli, číslo mezi 1 a 10^10 kromě čísla 10^10."
			}
		]
	},
	{
		name: "FISHER",
		description: "Vrátí hodnotu Fisherovy transformace.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit hodnotu transformace. Argument je číslo mezi -1 a 1 kromě čísel -1 a 1."
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Vrátí hodnotu inverzní funkce k Fisherově transformaci: jestliže y = FISHER(x), FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "je hodnota, pro kterou chcete zjistit hodnotu inverzní transformace."
			}
		]
	},
	{
		name: "FLOOR.MATH",
		description: "Zaokrouhlí číslo dolů na nejbližší celé číslo nebo na nejbližší násobek zadané hodnoty významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit"
			},
			{
				name: "významnost",
				description: "je násobek, na který chcete číslo zaokrouhlit"
			},
			{
				name: "režim",
				description: "pokud je zadaná a hodnota není rovná nula, bude tato funkce zaokrouhlovat směrem k nule"
			}
		]
	},
	{
		name: "FLOOR.PRECISE",
		description: "Zaokrouhlí číslo dolů na nejbližší celé číslo nebo na nejbližší násobek zadané hodnoty významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je číselná hodnota, kterou chcete zaokrouhlit."
			},
			{
				name: "významnost",
				description: "je násobek, na který chcete číslo zaokrouhlit. "
			}
		]
	},
	{
		name: "FORECAST",
		description: "Vypočte (odhadne) budoucí hodnotu lineárního trendu pomocí existujících hodnot.",
		arguments: [
			{
				name: "x",
				description: "je datový bod, pro který chcete odhadnout hodnotu. Musí to být číselná hodnota."
			},
			{
				name: "pole_y",
				description: "je závislá matice nebo oblast číselných dat."
			},
			{
				name: "pole_x",
				description: "je nezávislá matice nebo oblast číselných dat. Rozptyl argumentu x se nesmí rovnat nule."
			}
		]
	},
	{
		name: "FORMULATEXT",
		description: "Vrátí vzorec jako řetězec.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na vzorec"
			}
		]
	},
	{
		name: "FTEST",
		description: "Vrátí výsledek F-testu, tj. pravděpodobnosti (dva chvosty), že se rozptyly v argumentech matice1 a matice2 výrazně neliší.",
		arguments: [
			{
				name: "matice1",
				description: "je první matice nebo oblast dat. Hodnoty argumentu mohou být čísla, matice nebo odkazy obsahující čísla. (Prázdné buňky jsou ignorovány.)"
			},
			{
				name: "matice2",
				description: "je druhá matice nebo oblast dat. Hodnoty argumentu mohou být čísla, matice nebo odkazy obsahující čísla. (Prázdné buňky jsou ignorovány.)"
			}
		]
	},
	{
		name: "FVSCHEDULE",
		description: "Vrátí budoucí hodnotu počáteční jistiny po použití série sazeb složitého úroku.",
		arguments: [
			{
				name: "hodnota",
				description: "je současná hodnota."
			},
			{
				name: "sazby",
				description: "řada úrokových sazeb, které chcete použít."
			}
		]
	},
	{
		name: "GAMMA",
		description: "Vrátí hodnotu funkce gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete vypočítat gama"
			}
		]
	},
	{
		name: "GAMMA.DIST",
		description: "Vrátí hodnotu gama rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (nezáporné číslo), pro kterou chcete zjistit hodnotu rozdělení."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, kladné číslo. Pokud beta = 1, vrátí funkce GAMMA.DIST hodnotu standardního gama rozdělení."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: vrácení kumulativní distribuční funkce = PRAVDA; vrácení hromadné pravděpodobnostní funkce = NEPRAVDA nebo bez zadání."
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Vrátí hodnotu inverzní funkce k distribuční funkci kumulativního rozdělení gama: jestliže p = GAMMA.DIST(x,...), potom GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost rozdělení gama, číslo mezi 0 a 1 včetně."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, kladné číslo. Je-li argument beta roven 1, funkce GAMMA.INV vrátí inverzní funkci ke standardní distribuční funkci rozdělení gama."
			}
		]
	},
	{
		name: "GAMMADIST",
		description: "Vrátí hodnotu gama rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (nezáporné číslo), pro kterou chcete zjistit hodnotu rozdělení."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, kladné číslo. Pokud beta = 1, vrátí funkce GAMMADIST hodnotu standardního gama rozdělení."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: vrácení kumulativní distribuční funkce = PRAVDA; vrácení hromadné pravděpodobnostní funkce = NEPRAVDA nebo bez zadání."
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Vrátí hodnotu inverzní funkce ke kumulativnímu gama rozdělení: Jestliže p = GAMMADIST(x,...), pak GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost gama rozdělení, číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, kladné číslo. Pokud beta = 1, vrátí funkce GAMMAINV inverzní funkci ke standardnímu gama rozdělení."
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Vrátí přirozený logaritmus funkce gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete vypočítat hodnotu funkce GAMMALN. Argument musí být kladné číslo."
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Vrátí přirozený logaritmus funkce gama.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete vypočítat hodnotu funkce GAMMALN.PRECISE. Argument musí být kladné číslo."
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
		description: "Vrátí největší společný dělitel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "představují 1 až 255 hodnot."
			},
			{
				name: "číslo2",
				description: "představují 1 až 255 hodnot."
			}
		]
	},
	{
		name: "GEOMEAN",
		description: "Vrátí geometrický průměr matice nebo oblasti kladných číselných dat.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž geometrický průměr chcete zjistit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž geometrický průměr chcete zjistit."
			}
		]
	},
	{
		name: "GESTEP",
		description: "Testuje, zda je číslo větší než mezní hodnota.",
		arguments: [
			{
				name: "číslo",
				description: "je testovaná hodnota."
			},
			{
				name: "co",
				description: "je mezní hodnota."
			}
		]
	},
	{
		name: "HARMEAN",
		description: "Vrátí harmonický průměr množiny kladných čísel: reciproční hodnota aritmetického průměru recipročních čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž harmonický průměr chcete zjistit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž harmonický průměr chcete zjistit."
			}
		]
	},
	{
		name: "HEX2BIN",
		description: "Převede hexadecimální číslo na binární.",
		arguments: [
			{
				name: "číslo",
				description: "je hexadecimální číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "HEX2DEC",
		description: "Převede hexadecimální číslo na dekadické.",
		arguments: [
			{
				name: "číslo",
				description: "je hexadecimální číslo, které chcete převést."
			}
		]
	},
	{
		name: "HEX2OCT",
		description: "Převede hexadecimální číslo na osmičkové.",
		arguments: [
			{
				name: "číslo",
				description: "je hexadecimální číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "HLEDAT",
		description: "Vrátí číslo prvního nalezeného výskytu znaku nebo textového řetězce. Směr hledání je zleva doprava. Velká a malá písmena nejsou rozlišována.",
		arguments: [
			{
				name: "co",
				description: "je text, který chcete nalézt. Můžete použít zástupné znaky * a ?. Znaky * a ? naleznete pomocí řetězců ~? a ~* ."
			},
			{
				name: "kde",
				description: "je text, ve kterém chcete hledat znak nebo textový řetězec argumentu Co."
			},
			{
				name: "start",
				description: "je číslo znaku v argumentu Kde (počítáno zleva), ve kterém chcete začít vyhledávání. Jestliže tento argument nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "HODINA",
		description: "Vrátí hodiny jako číslo od 0 (12:00 dop.) do 23 (11:00 odp.).",
		arguments: [
			{
				name: "pořadové_číslo",
				description: "je číslo v kódu aplikace Spreadsheet pro datum a čas nebo text ve formátu času, například 16:48:00 nebo 4:48:00 odp."
			}
		]
	},
	{
		name: "HODNOTA",
		description: "Převede textový řetězec představující číslo na číslo.",
		arguments: [
			{
				name: "text",
				description: "je text v uvozovkách nebo odkaz na buňku obsahující text, který chcete převést."
			}
		]
	},
	{
		name: "HODNOTA.NA.TEXT",
		description: "Převede hodnotu na text v určitém formátu.",
		arguments: [
			{
				name: "hodnota",
				description: "je číslo, vzorec, jehož výsledek je číselná hodnota, nebo odkaz na buňku obsahující číselnou hodnotu."
			},
			{
				name: "formát",
				description: "je číselný formát ve formě textu vybraný v seznamu Druh na kartě Číslo dialogového okna Formát buněk (kromě obecného formátu)."
			}
		]
	},
	{
		name: "HYPERTEXTOVÝ.ODKAZ",
		description: "Vytvoří zástupce nebo odkaz, který otevře dokument uložený na pevném disku, síťovém serveru nebo na síti Internet.",
		arguments: [
			{
				name: "umístění",
				description: "je text představující cestu a název souboru s dokumentem, který chcete otevřít, umístění na pevném disku, adresu UNC nebo cestu URL."
			},
			{
				name: "název",
				description: "je text nebo číslo, které bude zobrazeno v buňce. Jestliže argument Název nezadáte, objeví se v buňce text argumentu Umístění."
			}
		]
	},
	{
		name: "HYPGEOM.DIST",
		description: "Vrátí hodnotu hypergeometrického rozdělení.",
		arguments: [
			{
				name: "úspěch",
				description: "je počet úspěšných pokusů ve výběru."
			},
			{
				name: "celkem",
				description: "je velikost výběru."
			},
			{
				name: "základ_úspěch",
				description: "je počet úspěšných pokusů v základním souboru."
			},
			{
				name: "základ_celkem",
				description: "je velikost základního souboru."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "HYPGEOMDIST",
		description: "Vrátí hodnotu hypergeometrického rozdělení.",
		arguments: [
			{
				name: "úspěch",
				description: "je počet úspěšných pokusů ve výběru."
			},
			{
				name: "celkem",
				description: "je velikost výběru."
			},
			{
				name: "základ_úspěch",
				description: "je počet úspěšných pokusů v základním souboru."
			},
			{
				name: "základ_celkem",
				description: "je velikost základního souboru."
			}
		]
	},
	{
		name: "IFERROR",
		description: "Pokud je výraz chybný, vrátí hodnotu hodnota_v_případě_chyby. V opačném případě vrátí vlastní hodnotu výrazu.",
		arguments: [
			{
				name: "hodnota",
				description: "je libovolná hodnota, výraz či odkaz."
			},
			{
				name: "hodnota_v_případě_chyby",
				description: "je libovolná hodnota, výraz či odkaz."
			}
		]
	},
	{
		name: "IFNA",
		description: "Vrátí zadanou hodnotu, pokud je výsledkem výrazu hodnota #N/A, v opačném případě vrátí výsledek výrazu.",
		arguments: [
			{
				name: "hodnota",
				description: "je jakákoli hodnota nebo výraz nebo odkaz"
			},
			{
				name: "hodnota_pokud_na",
				description: "je jakákoli hodnota nebo výraz nebo odkaz"
			}
		]
	},
	{
		name: "IMABS",
		description: "Vrátí absolutní hodnotu komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož absolutní hodnotu chcete zjistit."
			}
		]
	},
	{
		name: "IMAGINARY",
		description: "Vrátí imaginární část komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož imaginární část chcete nalézt."
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Vrátí argument q (úhel v radiánech).",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, pro které chcete nalézt argument."
			}
		]
	},
	{
		name: "IMCONJUGATE",
		description: "Vrátí komplexně sdružené číslo ke komplexnímu číslu.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, ke kterému chcete nalézt komplexně sdružené číslo."
			}
		]
	},
	{
		name: "IMCOS",
		description: "Vrátí kosinus komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož kosinus chcete nalézt."
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Vrátí hyperbolický kosinus komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, jehož hyperbolický kosinus chcete získat"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Vrátí kotangens komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, pro které chcete získat kotangens"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Vrátí kosekans komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, pro které chcete získat kosekans"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Vrátí hyperbolický kosekans komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, pro které chcete získat hyperbolický kosekans"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Vrátí podíl dvou komplexních čísel.",
		arguments: [
			{
				name: "ičíslo1",
				description: "je komplexní čitatel nebo dělenec."
			},
			{
				name: "ičíslo2",
				description: "je komplexní jmenovatel nebo dělitel."
			}
		]
	},
	{
		name: "IMEXP",
		description: "Vrátí exponenciální tvar komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož exponenciální tvar chcete nalézt."
			}
		]
	},
	{
		name: "IMLN",
		description: "Vrátí přirozený logaritmus komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož přirozený logaritmus chcete nalézt."
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Vrátí dekadický logaritmus komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož dekadický logaritmus chcete nalézt."
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Vrátí logaritmus komplexního čísla při základu 2.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož logaritmus při základu 2 chcete nalézt."
			}
		]
	},
	{
		name: "IMPOWER",
		description: "Vrátí komplexní číslo umocněné na celé číslo.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, které chcete umocnit."
			},
			{
				name: "číslo",
				description: "je mocnina, na kterou chcete komplexní číslo umocnit."
			}
		]
	},
	{
		name: "IMPRODUCT",
		description: "Vrátí součin 1 až 255 komplexních čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ičíslo1",
				description: "Hodnoty ičíslo1, ičíslo2,... představují 1 až 255 komplexních čísel, která chcete násobit."
			},
			{
				name: "ičíslo2",
				description: "Hodnoty ičíslo1, ičíslo2,... představují 1 až 255 komplexních čísel, která chcete násobit."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Vrátí reálnou část komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož reálnou část chcete nalézt."
			}
		]
	},
	{
		name: "IMSEC",
		description: "Vrátí sekans komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, pro které chcete získat sekans"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Vrátí hyperbolický sekans komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, pro které chcete získat hyperbolický sekans"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Vrátí sinus komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož sinus chcete nalézt."
			}
		]
	},
	{
		name: "IMSINH",
		description: "Vrátí hyperbolický sinus komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, jehož hyperbolický sinus chcete získat"
			}
		]
	},
	{
		name: "IMSQRT",
		description: "Vrátí druhou odmocninu komplexního čísla.",
		arguments: [
			{
				name: "ičíslo",
				description: "je komplexní číslo, jehož druhou odmocninu chcete nalézt."
			}
		]
	},
	{
		name: "IMSUB",
		description: "Vrátí rozdíl dvou komplexních čísel.",
		arguments: [
			{
				name: "ičíslo1",
				description: "je komplexní číslo, od kterého chcete odečíst argument ičíslo2."
			},
			{
				name: "ičíslo2",
				description: "je komplexní číslo, které chcete odečíst od argumentu ičíslo1."
			}
		]
	},
	{
		name: "IMSUM",
		description: "Vrátí součet komplexních čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ičíslo1",
				description: "představují 1 až 255 komplexních čísel, která chcete sečíst."
			},
			{
				name: "ičíslo2",
				description: "představují 1 až 255 komplexních čísel, která chcete sečíst."
			}
		]
	},
	{
		name: "IMTAN",
		description: "Vrátí tangens komplexního čísla.",
		arguments: [
			{
				name: "Ičíslo",
				description: "je komplexní číslo, jehož tangens chcete získat"
			}
		]
	},
	{
		name: "INDEX",
		description: "Vrátí hodnotu nebo odkaz na buňku v určitém řádku a sloupci v dané oblasti.",
		arguments: [
			{
				name: "pole",
				description: "je oblast buněk nebo maticová konstanta."
			},
			{
				name: "řádek",
				description: "vybere řádek v argumentu Pole nebo Odkaz, ze kterého bude vrácena hodnota. Jestliže tento argument nezadáte, musíte zadat argument Sloupec."
			},
			{
				name: "sloupec",
				description: "vybere sloupec v argumentu Pole nebo Odkaz, ze kterého bude vrácena hodnota. Jestliže tento argument nezadáte, musíte zadat argument Řádek."
			}
		]
	},
	{
		name: "INTERCEPT",
		description: "Vypočte souřadnice bodu, ve kterém čára protne osu y, pomocí proložení nejlepší regresní čáry známými hodnotami x a y.",
		arguments: [
			{
				name: "pole_y",
				description: "je závislá množina pozorování nebo dat. Hodnoty argumentu mohou být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			},
			{
				name: "pole_x",
				description: "je nezávislá množina pozorování nebo dat. Hodnoty argumentu mohou být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "INTRATE",
		description: "Vrátí úrokovou sazbu plně investovaného cenného papíru.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "investice",
				description: "je částka investovaná do cenného papíru."
			},
			{
				name: "zaruč_cena",
				description: "je částka, kterou obdržíte k datu splatnosti."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "INVERZE",
		description: "Vrátí inverzní matici k matici, která je uložena v oblasti definované jako matice.",
		arguments: [
			{
				name: "pole",
				description: "je číselná matice se stejným počtem řádků a sloupců. Může to být oblast buněk nebo maticová konstanta."
			}
		]
	},
	{
		name: "ISEVEN",
		description: "Vrátí logickou hodnotu PRAVDA, pokud je číslo sudé.",
		arguments: [
			{
				name: "číslo",
				description: "je testovaná hodnota."
			}
		]
	},
	{
		name: "ISFORMULA",
		description: "Ověří, jestli odkaz odkazuje na buňku obsahující vzorec, a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na buňku, kterou chcete testovat. Odkaz může být odkaz na buňku, vzorec nebo název odkazující na buňku"
			}
		]
	},
	{
		name: "ISO.CEILING",
		description: "Zaokrouhlí číslo nahoru na nejbližší celé číslo nebo na nejbližší násobek zadané hodnoty.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit."
			},
			{
				name: "hodnota",
				description: "je volitelný násobek, na který chcete číslo zaokrouhlit."
			}
		]
	},
	{
		name: "ISODD",
		description: "Vrátí logickou hodnotu PRAVDA, pokud je číslo liché.",
		arguments: [
			{
				name: "číslo",
				description: "je testovaná hodnota."
			}
		]
	},
	{
		name: "ISOWEEKNUM",
		description: "Vrátí číslo týdne ISO v roce pro dané datum.",
		arguments: [
			{
				name: "datum",
				description: "je kód pro datum a čas používaný Spreadsheetem pro výpočet data a času"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Vrátí výšku úroku zaplaceného za určité období investice.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba vztažená na jedno období. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4."
			},
			{
				name: "za",
				description: "je období, pro které chcete zjistit výšku úroku."
			},
			{
				name: "pper",
				description: "je počet platebních období investice."
			},
			{
				name: "souč",
				description: "je celková částka určující současnou hodnotu série budoucích plateb."
			}
		]
	},
	{
		name: "JE.CHYBA",
		description: "Ověří, zda se argument Hodnota rovná chybové hodnotě (#HODNOTA!, #ODKAZ!, #DĚLENÍ_NULOU!, #ČÍSLO!, #NÁZEV? nebo #NULL!) kromě #NENÍ_K_DISPOZICI a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.CHYBHODN",
		description: "Ověří, zda je argument Hodnota libovolná chybová hodnota (#NENÍ_K_DISPOZICI, #HODNOTA!, #ODKAZ!, #DĚLENÍ_NULOU!, #ČÍSLO!, #NÁZEV? nebo #NULL!), a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.ČISLO",
		description: "Ověří, zda je argument Hodnota číslo a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.LOGHODN",
		description: "Ověří, zda argument Hodnota je logická hodnota (PRAVDA nebo NEPRAVDA) a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.NEDEF",
		description: "Ověří, zda je argument Hodnota chybová hodnota #NENÍ_K_DISPOZICI, a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu se na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.NETEXT",
		description: "Ověří, zda argument Hodnota není text (prázdné buňky nejsou text) a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Může to být buňka, vzorec nebo název odkazující na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.ODKAZ",
		description: "Ověří, zda je argument Hodnota odkaz a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu se na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "JE.PRÁZDNÉ",
		description: "Ověří, zda odkaz v argumentu Hodnota odkazuje na prázdnou buňku a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je testovaná buňka nebo název, který se vztahuje k testované buňce."
			}
		]
	},
	{
		name: "JE.TEXT",
		description: "Ověří, zda je argument Hodnota text a vrátí hodnotu PRAVDA nebo NEPRAVDA.",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete testovat. Argument Hodnota se může vztahovat k buňce, vzorci nebo názvu odkazujícímu na buňku, vzorec nebo hodnotu."
			}
		]
	},
	{
		name: "KČ",
		description: "Převede číslo na text ve formátu měny.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, odkaz na buňku s číslem nebo vzorec, jehož výsledkem je číslo."
			},
			{
				name: "desetiny",
				description: "je počet desetinných míst vpravo od desetinné čárky. Číslo je podle potřeby zaokrouhleno. Jestliže argument Desetiny nezadáte, bude jeho hodnota 2."
			}
		]
	},
	{
		name: "KDYŽ",
		description: "Ověří, zda je podmínka splněna, a vrátí jednu hodnotu, jestliže je výsledkem hodnota PRAVDA, a jinou hodnotu, pokud je výsledkem hodnota NEPRAVDA.",
		arguments: [
			{
				name: "podmínka",
				description: "je libovolná hodnota nebo výraz, kterému může být přiřazena logická hodnota PRAVDA nebo NEPRAVDA."
			},
			{
				name: "ano",
				description: "je hodnota vrácená, je-li hodnota argumentu Podmínka PRAVDA. Jestliže ji nezadáte, bude vrácena hodnota PRAVDA. Můžete vnořit až sedm funkcí KDYŽ."
			},
			{
				name: "ne",
				description: "je hodnota vrácená, je-li hodnota argumentu Podmínka NEPRAVDA. Jestliže ji nezadáte, bude vrácena hodnota NEPRAVDA."
			}
		]
	},
	{
		name: "KÓD",
		description: "Vrátí číselný kód prvního znaku textového řetězce ze znakové sady definované v používaném počítači.",
		arguments: [
			{
				name: "text",
				description: "je textový řetězec, pro který chcete najít kód prvního znaku."
			}
		]
	},
	{
		name: "KOMBINACE",
		description: "Vrátí počet kombinací pro zadaný počet položek.",
		arguments: [
			{
				name: "počet",
				description: "je celkový počet položek."
			},
			{
				name: "kombinace",
				description: "je počet položek v každé kombinaci."
			}
		]
	},
	{
		name: "KURT",
		description: "Vrátí hodnotu excesu množiny dat.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, pro které chcete zjistit exces."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, pro které chcete zjistit exces."
			}
		]
	},
	{
		name: "LARGE",
		description: "Vrátí k-tou největší hodnotu množiny dat, například páté největší číslo.",
		arguments: [
			{
				name: "pole",
				description: "je matice nebo oblast dat, pro kterou chcete určit k-tou největší hodnotu."
			},
			{
				name: "k",
				description: "je pozice hledané hodnoty (počítáno od největší hodnoty) v matici nebo oblasti buněk."
			}
		]
	},
	{
		name: "LCM",
		description: "Vrátí nejmenší společný násobek.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "představují 1 až 255 hodnot, pro které chcete určit nejmenší společný násobek."
			},
			{
				name: "číslo2",
				description: "představují 1 až 255 hodnot, pro které chcete určit nejmenší společný násobek."
			}
		]
	},
	{
		name: "LINREGRESE",
		description: "Vrátí statistiku popisující lineární trend odpovídající známým datovým bodům proložením přímky vypočtené metodou nejmenších čtverců.",
		arguments: [
			{
				name: "pole_y",
				description: "je množina hodnot y počítaných pomocí rovnice y = mx + b."
			},
			{
				name: "pole_x",
				description: "je množina hodnot x počítaných pomocí rovnice y = mx + b."
			},
			{
				name: "b",
				description: "je logická hodnota: konstanta b je vypočítána, je-li argument b = PRAVDA nebo vynechán, konstanta b je rovna 0, je-li argument b = NEPRAVDA."
			},
			{
				name: "stat",
				description: "je logická hodnota: další návratová regresní statistika = PRAVDA, návratové koeficienty m a konstanta b = NEPRAVDA nebo bez zadání."
			}
		]
	},
	{
		name: "LINTREND",
		description: "Vrátí hodnoty lineárního trendu odpovídajícího známým datovým bodům pomocí metody nejmenších čtverců.",
		arguments: [
			{
				name: "pole_y",
				description: "je oblast nebo matice hodnot y určených z rovnice y = mx + b."
			},
			{
				name: "pole_x",
				description: "je volitelná oblast nebo matice hodnot x určených z rovnice y = mx + b. Matice musí být stejného typu jako matice argumentu Pole_y."
			},
			{
				name: "nová_x",
				description: "je oblast nebo matice nových hodnot x, pro které chcete zjistit odpovídající hodnoty y pomocí funkce LINTREND."
			},
			{
				name: "b",
				description: "je logická hodnota: konstanta b je vypočítána, je-li argument b = PRAVDA nebo vynechán, konstanta b je rovna 0, je-li argument b = NEPRAVDA."
			}
		]
	},
	{
		name: "LN",
		description: "Vrátí přirozený logaritmus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je kladné reálné číslo, jehož přirozený logaritmus chcete získat."
			}
		]
	},
	{
		name: "LOG",
		description: "Vrátí dekadický logaritmus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je kladné reálné číslo, jehož dekadický logaritmus chcete získat."
			}
		]
	},
	{
		name: "LOGINV",
		description: "Vrátí inverzní funkci ke kumulativní distribuční funkci logaritmicko-normálního rozdělení hodnot x, kde funkce ln(x) má normální rozdělení s parametry stř_hodn a sm_odch.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost logaritmicko-normálního rozdělení, číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "stř_hodn",
				description: "je střední hodnota funkce ln(x)."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka funkce ln(x), kladné číslo."
			}
		]
	},
	{
		name: "LOGLINREGRESE",
		description: "Vrátí statistiku, která popisuje exponenciální křivku odpovídající známým datovým bodům.",
		arguments: [
			{
				name: "pole_y",
				description: "je množina hodnot y počítaných pomocí rovnice y = b*m^x."
			},
			{
				name: "pole_x",
				description: "je množina hodnot x počítaných pomocí rovnice y = b*m^x."
			},
			{
				name: "b",
				description: "je logická hodnota: konstanta b je vypočítána, je-li argument b = PRAVDA nebo vynechán, konstanta b je rovna 1, je-li argument b = NEPRAVDA."
			},
			{
				name: "stat",
				description: "je logická hodnota: další návratová regresní statistika = PRAVDA, návratové koeficienty m a konstanta b = NEPRAVDA nebo bez zadání."
			}
		]
	},
	{
		name: "LOGLINTREND",
		description: "Vrátí hodnoty trendu exponenciálního růstu odpovídajícího známým datovým bodům.",
		arguments: [
			{
				name: "pole_y",
				description: "je množina hodnot y počítaných z rovnice y = b*m^x. Může to být matice nebo oblast kladných čísel."
			},
			{
				name: "pole_x",
				description: "je volitelná množina hodnot x počítaných z rovnice y = b*m^x. Může to být matice nebo oblast stejné velikosti jako oblast argumentu Pole_y."
			},
			{
				name: "nová_x",
				description: "jsou nové hodnoty x, pro které chcete zjistit odpovídající hodnoty y pomocí funkce LOGLINTREND."
			},
			{
				name: "b",
				description: "je logická hodnota: konstanta b je vypočítána, je-li argument b = PRAVDA, konstanta b je rovna 1, je-li argument b = NEPRAVDA nebo vynechán."
			}
		]
	},
	{
		name: "LOGNORM.DIST",
		description: "Vrátí hodnotu logaritmicko-normálního rozdělení hodnot x, kde funkce ln(x) má normální rozdělení s parametry Střední a Sm_odch.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit hodnotu rozdělení. Argument je kladné číslo."
			},
			{
				name: "střední",
				description: "je střední hodnota funkce ln(x)."
			},
			{
				name: "sm_odchylka",
				description: "je směrodatná odchylka funkce ln(x), kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Vrátí inverzní funkci ke kumulativní distribuční funkci logaritmicko-normálního rozdělení hodnot x, kde funkce ln(x) má normální rozdělení s parametry Stř_hodn a Sm_odch.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost logaritmicko-normálního rozdělení, číslo mezi 0 a 1 včetně."
			},
			{
				name: "stř_hodn",
				description: "je střední hodnota funkce ln(x)."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka funkce ln(x), kladné číslo."
			}
		]
	},
	{
		name: "LOGNORMDIST",
		description: "Vrátí hodnotu kumulativního logaritmicko-normálního rozdělení hodnot x, kde funkce ln(x) má normální rozdělení s parametry střední a sm_odch.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (kladné číslo), pro kterou chcete zjistit rozdělení."
			},
			{
				name: "střední",
				description: "je střední hodnota funkce ln(x)."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka funkce ln(x), kladné číslo."
			}
		]
	},
	{
		name: "LOGZ",
		description: "Vrátí logaritmus čísla při zadaném základu.",
		arguments: [
			{
				name: "číslo",
				description: "je kladné reálné číslo, jehož logaritmus chcete získat."
			},
			{
				name: "základ",
				description: "je základ logaritmu. Jestliže tento argument nezadáte, bude jeho hodnota 10."
			}
		]
	},
	{
		name: "MALÁ",
		description: "Převede všechna písmena textového řetězce na malá.",
		arguments: [
			{
				name: "text",
				description: "je text, který chcete převést na malá písmena. Znaky v argumentu Text, které nejsou písmena, se nezmění."
			}
		]
	},
	{
		name: "MAX",
		description: "Vrátí maximální hodnotu množiny hodnot. Přeskočí logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, jejichž maximální hodnotu chcete nalézt."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, jejichž maximální hodnotu chcete nalézt."
			}
		]
	},
	{
		name: "MAXA",
		description: "Vrátí maximální hodnotu v množině hodnot. Nepřeskočí logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, pro které chcete zjistit maximální hodnotu."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, pro které chcete zjistit maximální hodnotu."
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Vrátí medián, střední hodnotu množiny zadaných čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, názvů, matic nebo odkazů obsahujících čísla, pro která chcete nalézt medián."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, názvů, matic nebo odkazů obsahujících čísla, pro která chcete nalézt medián."
			}
		]
	},
	{
		name: "MĚSÍC",
		description: "Vrátí měsíc, číslo od 1 (leden) do 12 (prosinec).",
		arguments: [
			{
				name: "pořadové_číslo",
				description: "je číslo v kódu aplikace Spreadsheet pro datum a čas."
			}
		]
	},
	{
		name: "MIN",
		description: "Vrátí minimální hodnotu množiny hodnot. Přeskočí logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, jejichž minimální hodnotu chcete nalézt."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, jejichž minimální hodnotu chcete nalézt."
			}
		]
	},
	{
		name: "MINA",
		description: "Vrátí minimální hodnotu v množině hodnot. Nepřeskočí logické hodnoty a text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, pro které chcete zjistit minimální hodnotu."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 čísel, prázdných buněk, logických hodnot nebo čísel ve formátu textu, pro které chcete zjistit minimální hodnotu."
			}
		]
	},
	{
		name: "MINUTA",
		description: "Vrátí minuty, číslo od 0 do 59.",
		arguments: [
			{
				name: "pořadové_číslo",
				description: "je číslo v kódu aplikace Spreadsheet pro datum a čas nebo text ve formátu času, například 16:48:00 nebo 4:48:00 odp."
			}
		]
	},
	{
		name: "MÍRA.VÝNOSNOSTI",
		description: "Vrátí vnitřní výnosové procento série peněžních toků.",
		arguments: [
			{
				name: "hodnoty",
				description: "je matice nebo odkaz na buňky obsahující čísla, pro které chcete vypočítat vnitřní výnosové procento."
			},
			{
				name: "odhad",
				description: "je číslo, které představuje odhad blízký výsledku funkce MÍRA VÝNOSNOSTI. Jestliže tento argument nezadáte, bude mít hodnotu 0,1 (10 procent)."
			}
		]
	},
	{
		name: "MOD",
		description: "Vrátí zbytek po dělení čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, pro které chcete najít zbytek při dělení."
			},
			{
				name: "dělitel",
				description: "je číslo, kterým chcete dělit argument Číslo."
			}
		]
	},
	{
		name: "MOD.MÍRA.VÝNOSNOSTI",
		description: "Vrátí vnitřní sazbu výnosu pravidelných peněžních příjmů. Zohledňuje jak náklady na investice, tak úrok z reinvestic získaných peněžních prostředků.",
		arguments: [
			{
				name: "hodnoty",
				description: "je matice nebo odkaz na buňky obsahující čísla, která představují splátky (záporná hodnota) a příjmy (kladná hodnota) v pravidelných obdobích."
			},
			{
				name: "finance",
				description: "je úroková sazba, kterou zaplatíte za použití peněžních příjmů."
			},
			{
				name: "investice",
				description: "je úroková sazba, kterou získáte z reinvestic peněžních příjmů."
			}
		]
	},
	{
		name: "MODE",
		description: "Vrátí hodnotu, která se v matici nebo v oblasti dat vyskytuje nejčastěji.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž modus chcete zjistit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž modus chcete zjistit."
			}
		]
	},
	{
		name: "MODE.MULT",
		description: "Vrátí vertikální matici nejčastěji se vyskytujících (opakovaných) hodnot v matici nebo oblasti dat. Chcete-li získat horizontální matici, použijte vzorec =TRANSPOZICE(MODE.MULT(číslo1;číslo2;...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž modus chcete zjistit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž modus chcete zjistit."
			}
		]
	},
	{
		name: "MODE.SNGL",
		description: "Vrátí hodnotu, která se v matici nebo v oblasti dat vyskytuje nejčastěji.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž modus chcete zjistit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, jejichž modus chcete zjistit."
			}
		]
	},
	{
		name: "MROUND",
		description: "Vrátí číslo zaokrouhlené na požadovaný násobek.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit."
			},
			{
				name: "násobek",
				description: "je násobek, na který chcete číslo zaokrouhlit."
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Vrátí mnohočlen sady čísel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "představují 1 až 255 hodnot, pro které chcete určit mnohočlen."
			},
			{
				name: "číslo2",
				description: "představují 1 až 255 hodnot, pro které chcete určit mnohočlen."
			}
		]
	},
	{
		name: "MUNIT",
		description: "Vrátí matici jednotek pro zadanou dimenzi.",
		arguments: [
			{
				name: "dimenze",
				description: "je celé číslo určující dimenzi matice jednotek, kterou chcete vrátit"
			}
		]
	},
	{
		name: "N",
		description: "Převede nečíselnou hodnotu na číslo, kalendářní data na pořadová čísla, hodnotu PRAVDA na číslo 1 a všechny ostatní výrazy na číslo 0 (nula).",
		arguments: [
			{
				name: "hodnota",
				description: "je hodnota, kterou chcete převést."
			}
		]
	},
	{
		name: "NÁHČÍSLO",
		description: "Vrátí náhodné číslo větší nebo rovné 0 a menší než 1 určené na základě spojité distribuční funkce (změní se při každém přepočítání listu).",
		arguments: [
		]
	},
	{
		name: "NAHRADIT",
		description: "Nahradí část textového řetězce jiným textovým řetězcem.",
		arguments: [
			{
				name: "starý",
				description: "je text, ve kterém chcete zaměnit některé znaky."
			},
			{
				name: "start",
				description: "je pozice znaku v textu argumentu Starý, který chcete nahradit textem argumentu Nový."
			},
			{
				name: "znaky",
				description: "je počet znaků textu argumentu Starý, které chcete nahradit."
			},
			{
				name: "nový",
				description: "je nový text , kterým nahradíte text argumentu Starý."
			}
		]
	},
	{
		name: "NAJÍT",
		description: "Vrátí počáteční pozici jednoho textového řetězce v jiném textovém řetězci. Tato funkce rozlišuje malá a velká písmena.",
		arguments: [
			{
				name: "co",
				description: "je text, který chcete nalézt. Použijte uvozovky (prázdný text) k vyhledání prvního odpovídajícího znaku argumentu Kde. Zástupné znaky nejsou povoleny."
			},
			{
				name: "kde",
				description: "je text obsahující hledaný text."
			},
			{
				name: "start",
				description: "určuje znak, ve kterém začne vyhledávání. První znak argumentu Kde je znak číslo 1. Jestliže argument Start nezadáte, bude jeho hodnota 1. "
			}
		]
	},
	{
		name: "NE",
		description: "Změní hodnotu NEPRAVDA na PRAVDA nebo naopak.",
		arguments: [
			{
				name: "loghod",
				description: "je hodnota nebo výraz, který může být PRAVDA nebo NEPRAVDA."
			}
		]
	},
	{
		name: "NEBO",
		description: "Ověří, zda je nejméně jeden argument roven hodnotě PRAVDA, a vrátí hodnotu PRAVDA nebo NEPRAVDA. Vrátí hodnotu NEPRAVDA pouze v případě, že všechny argumenty jsou rovny hodnotě NEPRAVDA.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logická1",
				description: "je 1 až 255 podmínek, které chcete testovat a které mohou mít hodnotu PRAVDA nebo NEPRAVDA."
			},
			{
				name: "logická2",
				description: "je 1 až 255 podmínek, které chcete testovat a které mohou mít hodnotu PRAVDA nebo NEPRAVDA."
			}
		]
	},
	{
		name: "NEDEF",
		description: "Vrátí chybovou hodnotu #NENÍ_K_DISPOZICI (hodnota nedostupná).",
		arguments: [
		]
	},
	{
		name: "NEGBINOM.DIST",
		description: "Vrátí hodnotu negativního binomického rozdělení, tj. pravděpodobnost, že neúspěchy argumentu počet_neúspěchů nastanou dříve než úspěch argumentu počet_úspěchů s pravděpodobností určenou argumentem pravděpodobnost_úspěchu.",
		arguments: [
			{
				name: "počet_neúspěchů",
				description: "je počet neúspěšných pokusů."
			},
			{
				name: "počet_úspěchů",
				description: "je mezní počet úspěšných pokusů."
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost úspěchu, číslo mezi 0 a 1."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, hromadná pravděpodobnostní funkce = NEPRAVDA."
			}
		]
	},
	{
		name: "NEGBINOMDIST",
		description: "Vrátí hodnotu negativního binomického rozdělení, tj. pravděpodobnost, že počet neúspěchů určený argumentem počet_neúspěchů nastane dříve než počet úspěchů určených argumentem počet_úspěchů s pravděpodobností určenou argumentem pravděpodobnost_úspěchu.",
		arguments: [
			{
				name: "počet_neúspěchů",
				description: "je počet neúspěšných pokusů."
			},
			{
				name: "počet_úspěchů",
				description: "je mezní počet úspěšných pokusů."
			},
			{
				name: "pravděpodobnost_úspěchu",
				description: "je pravděpodobnost úspěchu, číslo mezi 0 a 1."
			}
		]
	},
	{
		name: "NEPRAVDA",
		description: "Vrátí logickou hodnotu NEPRAVDA.",
		arguments: [
		]
	},
	{
		name: "NEPŘÍMÝ.ODKAZ",
		description: "Vrátí odkaz určený textovým řetězcem.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na buňku obsahující odkaz ve stylu A1 nebo R1C1, název definovaný jako odkaz nebo odkaz na buňku ve formátu textového řetězce."
			},
			{
				name: "a1",
				description: "je logická hodnota, která určuje styl odkazu v argumentu Odkaz: styl R1C1 = NEPRAVDA, styl A1 = PRAVDA nebo bez zadání."
			}
		]
	},
	{
		name: "NETWORKDAYS",
		description: "Vrátí počet celých pracovních dnů mezi dvěma zadanými daty.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "konec",
				description: "je pořadové číslo, které představuje koncové datum."
			},
			{
				name: "svátky",
				description: "je volitelná množina jednoho nebo více pořadových čísel dat, která chcete vyloučit z kalendáře pracovních dnů, například státní nebo pohyblivé svátky."
			}
		]
	},
	{
		name: "NETWORKDAYS.INTL",
		description: "Vrátí počet celých pracovních dní mezi dvěma daty s vlastními parametry víkendu.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "konec",
				description: "je pořadové číslo představující koncové datum."
			},
			{
				name: "víkend",
				description: "je číslo nebo řetězec určující, které dny jsou považovány za víkendové."
			},
			{
				name: "svátky",
				description: "je volitelná sada jednoho nebo více pořadových čísel dat, která chcete vyloučit z kalendáře pracovních dnů, například státní nebo pohyblivé svátky."
			}
		]
	},
	{
		name: "NOMINAL",
		description: "Vrátí nominální roční úrokovou sazbu.",
		arguments: [
			{
				name: "úrok",
				description: "je efektivní úroková sazba."
			},
			{
				name: "období",
				description: "je počet úročených období za rok."
			}
		]
	},
	{
		name: "NORM.DIST",
		description: "Vrátí hodnotu normálního rozdělení pro zadanou střední hodnotu a směrodatnou odchylku.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit rozdělení."
			},
			{
				name: "střed_hodn",
				description: "je aritmetická střední hodnota rozdělení."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka rozdělení, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Vrátí inverzní funkci k distribuční funkci normálního kumulativního rozdělení pro zadanou střední hodnotu a směrodatnou odchylku.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost normálního rozdělení, číslo mezi 0 a 1 včetně."
			},
			{
				name: "střední",
				description: "je aritmetická střední hodnota rozdělení."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka rozdělení, kladné číslo."
			}
		]
	},
	{
		name: "NORM.S.DIST",
		description: "Vrátí standardní normální rozdělení (má střední hodnotu nula a směrodatnou odchylku jedna).",
		arguments: [
			{
				name: "z",
				description: "je hodnota, pro kterou chcete zjistit rozdělení."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota, kterou funkce vrátí: kumulativní distribuční funkce = TRUE, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Vrátí inverzní funkci k distribuční funkci standardního normálního kumulativního rozdělení (které má střední hodnotu rovnou 0 a směrodatnou odchylku 1).",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost normálního rozdělení, číslo mezi 0 a 1 včetně."
			}
		]
	},
	{
		name: "NORMDIST",
		description: "Vrátí hodnotu normálního kumulativního rozdělení pro zadanou střední hodnotu a směrodatnou odchylku.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit rozdělení."
			},
			{
				name: "střed_hodn",
				description: "je aritmetická střední hodnota rozdělení."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka rozdělení, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "NORMINV",
		description: "Vrátí inverzní funkci k normálnímu kumulativnímu rozdělení pro zadanou střední hodnotu a směrodatnou odchylku.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost normálního rozdělení, číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "střední",
				description: "je aritmetická střední hodnota rozdělení."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka rozdělení, kladné číslo."
			}
		]
	},
	{
		name: "NORMSDIST",
		description: "Vrátí hodnotu standardního normálního kumulativního rozdělení. (Střední hodnota daného rozdělení je rovna 0 a jeho směrodatná odchylka je rovna 1.).",
		arguments: [
			{
				name: "z",
				description: "je hodnota, pro kterou chcete zjistit rozdělení."
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Vrátí inverzní funkci ke standardnímu normálnímu kumulativnímu rozdělení. (Střední hodnota daného rozdělení je rovna 0 a jeho směrodatná odchylka je rovna 1).",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost normálního rozdělení, číslo mezi 0 a 1 (včetně)."
			}
		]
	},
	{
		name: "NUMBERVALUE",
		description: "Převede text na číslo nezávisle na prostředí.",
		arguments: [
			{
				name: "text",
				description: "je řetězec představující číslo, které chcete převést"
			},
			{
				name: "oddělovač_desetin",
				description: "je znak použitý jako oddělovač desetinných míst v řetězci"
			},
			{
				name: "oddělovač_skupin",
				description: "je znak použitý jako oddělovač skupin v řetězci"
			}
		]
	},
	{
		name: "NYNÍ",
		description: "Vrátí aktuální datum a čas formátované jako datum a čas.",
		arguments: [
		]
	},
	{
		name: "O.PROSTŘEDÍ",
		description: "Vrátí informaci o aktuálním pracovním prostředí.",
		arguments: [
			{
				name: "typ",
				description: "je text, který určuje typ požadované informace."
			}
		]
	},
	{
		name: "OCT2BIN",
		description: "Převede osmičkové číslo na binární.",
		arguments: [
			{
				name: "číslo",
				description: "je osmičkové číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "OCT2DEC",
		description: "Převede osmičkové číslo na dekadické.",
		arguments: [
			{
				name: "číslo",
				description: "je osmičkové číslo, které chcete převést."
			}
		]
	},
	{
		name: "OCT2HEX",
		description: "Převede osmičkové číslo na hexadecimální.",
		arguments: [
			{
				name: "číslo",
				description: "je osmičkové číslo, které chcete převést."
			},
			{
				name: "místa",
				description: "je počet znaků, které chcete použít u výsledku."
			}
		]
	},
	{
		name: "ODKAZ",
		description: "Vytvoří textový odkaz na buňku po zadání čísla řádku a sloupce.",
		arguments: [
			{
				name: "řádek",
				description: "je číslo řádku použité v odkazu na buňku: argument Řádek = 1 pro řádek číslo 1."
			},
			{
				name: "sloupec",
				description: "je číslo sloupce použité v odkazu na buňku: argument Sloupec = 4 pro sloupec D."
			},
			{
				name: "typ",
				description: "určuje typ odkazu: absolutní = 1, absolutní řádek/relativní sloupec = 2, relativní řádek/absolutní sloupec = 3, relativní = 4."
			},
			{
				name: "a1",
				description: "je logická hodnota určující styl odkazu: styl A1 = 1 nebo PRAVDA, styl R1C1 = 0 nebo NEPRAVDA."
			},
			{
				name: "list",
				description: "je text určující název listu, který bude použit jako externí odkaz."
			}
		]
	},
	{
		name: "ODMOCNINA",
		description: "Vrátí druhou odmocninu čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, jehož odmocninu chcete zjistit."
			}
		]
	},
	{
		name: "ODPIS.LIN",
		description: "Vrátí přímé odpisy aktiva pro jedno období.",
		arguments: [
			{
				name: "náklady",
				description: "je pořizovací cena aktiva."
			},
			{
				name: "zůstatek",
				description: "je zůstatková cena na konci období životnosti aktiva."
			},
			{
				name: "životnost",
				description: "je počet období, ve kterých je aktivum odepisováno (někdy se nazývá životnost aktiva)."
			}
		]
	},
	{
		name: "ODPIS.NELIN",
		description: "Vrátí směrné číslo ročních odpisů aktiva pro zadané období.",
		arguments: [
			{
				name: "náklady",
				description: "je pořizovací cena aktiva."
			},
			{
				name: "zůstatek",
				description: "je zůstatková cena na konci období životnosti aktiva."
			},
			{
				name: "životnost",
				description: "je počet období, ve kterých je aktivum odepisováno (někdy se nazývá životnost aktiva)."
			},
			{
				name: "za",
				description: "je období, které musí být vyjádřeno ve stejných jednotkách jako argument Životnost."
			}
		]
	},
	{
		name: "ODPIS.ZA.INT",
		description: "Vypočte odpisy aktiva pro každé zadané období, včetně neukončených období, pomocí dvojité degresivní metody odpisu nebo jiné metody, kterou zadáte.",
		arguments: [
			{
				name: "cena",
				description: "je pořizovací cena aktiva."
			},
			{
				name: "zůstatek",
				description: "je zbytková hodnota aktiva na konci období životnosti aktiva."
			},
			{
				name: "životnost",
				description: "je počet období, ve kterých jsou aktiva odepisována (někdy se nazývá životnost aktiva)."
			},
			{
				name: "začátek",
				description: "je počáteční období, pro které chcete vypočítat odpisy, ve stejných jednotkách jako argument Životnost."
			},
			{
				name: "konec",
				description: "je konečné období, po které chcete vypočítat odpisy, ve stejných jednotkách jako argument Životnost."
			},
			{
				name: "faktor",
				description: "je míra poklesu zůstatku. Jestliže tento argument nezadáte, bude mít hodnotu 2 (dvojitá degresivní metoda odpisu)."
			},
			{
				name: "nepřepínat",
				description: "je logická hodnota: přejít na přímé odpisy, pokud je hodnota odpisu větší než klesající zůstatek = NEPRAVDA nebo bez zadání, nepřecházet na přímé odpisy = PRAVDA."
			}
		]
	},
	{
		name: "ODPIS.ZRYCH",
		description: "Vypočítá odpis aktiva za určité období pomocí degresivní metody odpisu s pevným zůstatkem.",
		arguments: [
			{
				name: "náklady",
				description: "je pořizovací cena aktiva."
			},
			{
				name: "zůstatek",
				description: "je zůstatková cena na konci období životnosti aktiva."
			},
			{
				name: "životnost",
				description: "je počet období, po které je aktivum odepisováno (někdy se nazývá životnost aktiva)."
			},
			{
				name: "období",
				description: "je období, za které chcete vypočítat odpis. Argument Období musí být ve stejných jednotkách jako argument Životnost."
			},
			{
				name: "měsíc",
				description: "je počet měsíců v prvním roce odepisování. Jestliže argument Měsíc vynecháte, bude jeho hodnota 12."
			}
		]
	},
	{
		name: "ODPIS.ZRYCH2",
		description: "Vypočítá odpis aktiva za určité období pomocí dvojité degresivní metody odpisu nebo jiné metody, kterou zadáte.",
		arguments: [
			{
				name: "náklady",
				description: "je pořizovací cena aktiva."
			},
			{
				name: "zůstatek",
				description: "je zůstatková cena na konci období životnosti aktiva."
			},
			{
				name: "životnost",
				description: "je počet období, po které je aktivum odepisováno (někdy se nazývá životnost aktiva)."
			},
			{
				name: "období",
				description: "je období, za které chcete vypočítat odpis. Argument Období musí být ve stejných jednotkách jako argument Životnost."
			},
			{
				name: "faktor",
				description: "je míra poklesu zůstatku. Jestliže argument Faktor vynecháte, bude jeho hodnota 2 (dvojitá degresivní metoda odpisu)."
			}
		]
	},
	{
		name: "OPAKOVAT",
		description: "Několikrát zopakuje zadaný text. Použijte funkci OPAKOVAT, chcete-li vyplnit buňku určitým počtem výskytů textového řetězce.",
		arguments: [
			{
				name: "text",
				description: "je text, který chcete opakovat."
			},
			{
				name: "počet",
				description: "je kladné číslo určující, kolikrát chcete text opakovat."
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
		description: "Vrátí počet období požadovaných investicí k tomu, aby dosáhla zadané hodnoty.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba za období."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota investice"
			},
			{
				name: "bud_hod",
				description: "je požadovaná budoucí hodnota investice"
			}
		]
	},
	{
		name: "PEARSON",
		description: "Vrátí Pearsonův výsledný momentový korelační koeficient r.",
		arguments: [
			{
				name: "pole1",
				description: "je množina nezávislých hodnot."
			},
			{
				name: "pole2",
				description: "je množina závislých hodnot."
			}
		]
	},
	{
		name: "PERCENTIL",
		description: "Vrátí hodnotu k-tého percentilu hodnot v oblasti.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat, která definuje relativní umístění."
			},
			{
				name: "k",
				description: "je hodnota percentilu, která se nachází mezi 0 a 1 (včetně)."
			}
		]
	},
	{
		name: "PERCENTIL.EXC",
		description: "Vrátí hodnotu k-tého percentilu hodnot v oblasti, kde hodnota k spadá do oblasti 0..1 (s vyloučením hodnot 0 a 1).",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat, která definuje relativní umístění."
			},
			{
				name: "k",
				description: "je hodnota percentilu, která se nachází mezi 0 a 1 (včetně)."
			}
		]
	},
	{
		name: "PERCENTIL.INC",
		description: "Vrátí hodnotu k-tého percentilu hodnot v oblasti, kde hodnota k spadá do oblasti 0..1 (včetně).",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat, která definuje relativní umístění."
			},
			{
				name: "k",
				description: "je hodnota percentilu, která se nachází mezi 0 a 1 (včetně)."
			}
		]
	},
	{
		name: "PERCENTRANK",
		description: "Vrátí pořadí hodnoty v množině dat vyjádřené procentuální částí množiny dat.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat s číselnými hodnotami, která definuje relativní umístění."
			},
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit pořadí."
			},
			{
				name: "významnost",
				description: "je volitelná hodnota, která určuje počet významných desetinných míst výsledné procentuální hodnoty. Jestliže tento argument nezadáte, bude jeho hodnota 3 (0,xxx %)."
			}
		]
	},
	{
		name: "PERCENTRANK.EXC",
		description: "Vrátí pořadí hodnoty v množině dat vyjádřené procentuální částí (0..1, s vyloučením hodnot 0 a 1) množiny dat.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat s číselnými hodnotami, která definuje relativní umístění."
			},
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit pořadí."
			},
			{
				name: "významnost",
				description: "je volitelná hodnota, která určuje počet významných desetinných míst výsledné procentuální hodnoty. Jestliže tento argument nezadáte, bude jeho hodnota 3 (0,xxx %)."
			}
		]
	},
	{
		name: "PERCENTRANK.INC",
		description: "Vrátí pořadí hodnoty v množině dat vyjádřené procentuální částí (0..1, včetně) množiny dat.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat s číselnými hodnotami, která definuje relativní umístění."
			},
			{
				name: "x",
				description: "je hodnota, pro kterou chcete zjistit pořadí."
			},
			{
				name: "významnost",
				description: "je volitelná hodnota, která určuje počet významných desetinných míst výsledné procentuální hodnoty. Jestliže tento argument nezadáte, bude jeho hodnota 3 (0,xxx %)."
			}
		]
	},
	{
		name: "PERMUTACE",
		description: "Vrátí počet permutací pro zadaný počet objektů, které lze vybrat z celkového počtu objektů.",
		arguments: [
			{
				name: "počet",
				description: "je celkový počet objektů."
			},
			{
				name: "permutace",
				description: "je počet objektů v každé permutaci."
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Vrátí počet permutací pro zadaný počet objektů (s opakováním) které je možné vybrat z celkového počtu objektů.",
		arguments: [
			{
				name: "počet",
				description: "je počet objektů"
			},
			{
				name: "permutace",
				description: " v každé permutaci"
			}
		]
	},
	{
		name: "PHI",
		description: "Vrátí hodnotu funkce hustoty pro standardní normální rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je číslo, pro které chcete zjistit hustotu standardního normálního rozdělení"
			}
		]
	},
	{
		name: "PI",
		description: "Vrátí hodnotu čísla pí s přesností na 15 číslic. Výsledek je hodnota 3.14159265358979.",
		arguments: [
		]
	},
	{
		name: "PLATBA",
		description: "Vypočte splátku půjčky na základě konstantních splátek a konstantní úrokové sazby.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba půjčky vztažená na jedno období. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4."
			},
			{
				name: "pper",
				description: "je celkový počet splátek půjčky."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota: celková hodnota série budoucích plateb."
			},
			{
				name: "bud_hod",
				description: " je budoucí hodnota nebo hotovostní bilance, kterou chcete dosáhnout po splacení poslední platby. Jestliže tento argument nezadáte, bude jeho hodnota 0 (nula)."
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na konci období = 0 nebo bez zadání, splátka na začátku období = 1."
			}
		]
	},
	{
		name: "PLATBA.ÚROK",
		description: "Vrátí výšku úroku v určitém úrokovém období vypočtenou na základě pravidelných konstantních splátek a konstantní úrokové sazby.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba vztažená na jedno období. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4."
			},
			{
				name: "za",
				description: "je období, pro které chcete vypočítat úrok. Tato hodnota musí ležet v intervalu od 1 do Pper."
			},
			{
				name: "pper",
				description: "je celkový počet platebních období investice."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota nebo celková částka určující současnou hodnotu série budoucích plateb."
			},
			{
				name: "bud_hod",
				description: "je budoucí hodnota nebo hotovostní bilance, kterou chcete dosáhnout po splacení poslední platby. Jestliže argument Bud_hod nezadáte, bude jeho hodnota 0."
			},
			{
				name: "typ",
				description: "je logická hodnota, která představuje termín splátky: splátka na konci období = 0 nebo bez zadání, splátka na začátku období = 1"
			}
		]
	},
	{
		name: "PLATBA.ZÁKLAD",
		description: "Vrátí hodnotu splátky jistiny pro zadanou investici vypočtenou na základě pravidelných konstantních splátek a konstantní úrokové sazby.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba vztažená na jedno období. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4."
			},
			{
				name: "za",
				description: "určuje období. Musí nabývat hodnot od 1 do Pper."
			},
			{
				name: "pper",
				description: "je celkový počet platebních období investice."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota: celková hodnota série budoucích plateb."
			},
			{
				name: "bud_hod",
				description: "je budoucí hodnota nebo hotovostní bilance, kterou chcete dosáhnout po splacení poslední platby."
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na konci období = 0 nebo bez zadání, splátka na začátku období = 1"
			}
		]
	},
	{
		name: "POČET",
		description: "Vrátí počet buněk v rozsahu obsahujících čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 argumentů, které obsahují nebo odkazují na různé typy dat, spočítána budou však pouze čísla."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 argumentů, které obsahují nebo odkazují na různé typy dat, spočítána budou však pouze čísla."
			}
		]
	},
	{
		name: "POČET.BLOKŮ",
		description: "Vrátí počet oblastí v odkazu. Oblast může představovat souvislou oblast buněk nebo jedinou buňku.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na buňku nebo oblast buněk a může se vztahovat k více oblastem."
			}
		]
	},
	{
		name: "POČET.OBDOBÍ",
		description: "Vrátí počet období pro investici vypočítaný na základě pravidelných konstantních splátek a konstantní úrokové sazby. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba vztažená na jedno období."
			},
			{
				name: "splátka",
				description: "je platba provedená v každém období. Během období životnosti investice ji nelze změnit."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota nebo celková částka určující současnou hodnotu série budoucích plateb."
			},
			{
				name: "bud_hod",
				description: "je budoucí hodnota nebo hotovostní bilance, kterou chcete dosáhnout po splacení poslední platby. Jestliže argument nezadáte, bude jeho hodnota 0."
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na konci období = 0 nebo bez zadání, splátka na začátku období = 1."
			}
		]
	},
	{
		name: "POČET2",
		description: "Vrátí počet buněk v rozsahu, které nejsou prázdné.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 argumentů představujících hodnoty a buňky, které chcete spočítat. Hodnoty mohou představovat jakýkoli typ informací."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 argumentů představujících hodnoty a buňky, které chcete spočítat. Hodnoty mohou představovat jakýkoli typ informací."
			}
		]
	},
	{
		name: "POISSON",
		description: "Vrátí hodnotu Poissonova rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je počet událostí."
			},
			{
				name: "střední",
				description: "je předpokládaná číselná hodnota, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce Poissonova rozdělení pravděpodobnosti = PRAVDA, hromadná pravděpodobnostní funkce Poissonova rozdělení = NEPRAVDA."
			}
		]
	},
	{
		name: "POISSON.DIST",
		description: "Vrátí hodnotu Poissonova rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je počet událostí."
			},
			{
				name: "střední",
				description: "je předpokládaná číselná hodnota, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce Poissonova rozdělení pravděpodobnosti = PRAVDA, hromadná pravděpodobnostní funkce Poissonova rozdělení = NEPRAVDA."
			}
		]
	},
	{
		name: "POLÍČKO",
		description: "Vrátí informace o formátování, umístění nebo obsahu první buňky odkazu ve směru psaní daného listu.",
		arguments: [
			{
				name: "informace",
				description: "je text určující typ požadované informace o buňce."
			},
			{
				name: "odkaz",
				description: "je buňka, o které chcete získat informace."
			}
		]
	},
	{
		name: "POSUN",
		description: "Vrátí odkaz na oblast, která představuje daný počet řádků a sloupců z daného odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz na buňku nebo oblast buněk, od které chcete změřit odsazení."
			},
			{
				name: "řádky",
				description: "je počet řádků, nahoru nebo dolů, na které se má horní levá buňka výsledku odkazovat."
			},
			{
				name: "sloupce",
				description: "je počet sloupců, vpravo nebo vlevo, na které se má horní levá buňka výsledku odkazovat."
			},
			{
				name: "výška",
				description: "je požadovaná výška výsledku vyjádřená počtem řádků. Jestliže tento argument nezadáte, bude jeho hodnota stejná jako hodnota argumentu Odkaz."
			},
			{
				name: "šířka",
				description: "je požadovaná šířka výsledku vyjádřená počtem sloupců. Jestliže tento argument nezadáte, bude jeho hodnota stejná jako hodnota argumentu Odkaz."
			}
		]
	},
	{
		name: "POWER",
		description: "Umocní číslo na zadaný exponent.",
		arguments: [
			{
				name: "číslo",
				description: "je základ mocniny, kterým může být libovolné reálné číslo."
			},
			{
				name: "exponent",
				description: "je exponent, na který chcete základ umocnit."
			}
		]
	},
	{
		name: "POZVYHLEDAT",
		description: "Vrátí relativní polohu položky matice, která odpovídá určené hodnotě v určeném pořadí.",
		arguments: [
			{
				name: "co",
				description: "je hodnota, která bude použita k vyhledání požadované hodnoty v matici. Může to být číslo, text nebo logická hodnota nebo odkaz na jednu z uvedených položek."
			},
			{
				name: "prohledat",
				description: "je souvislá oblast buněk obsahující hledané hodnoty, matice hodnot nebo odkaz na matici."
			},
			{
				name: "shoda",
				description: "je číslo 1, 0 nebo -1 určující, která hodnota bude vrácena."
			}
		]
	},
	{
		name: "PRAVDA",
		description: "Vrátí logickou hodnotu PRAVDA.",
		arguments: [
		]
	},
	{
		name: "PRICEDISC",
		description: "Vrátí cenu diskontního cenného papíru o nominální hodnotě 100 Kč.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "diskont_sazba",
				description: "je diskontní sazba cenného papíru."
			},
			{
				name: "zaruč_cena",
				description: "je výkupní hodnota cenného papíru o nominální hodnotě 100 Kč."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "PROB",
		description: "Vrátí pravděpodobnost toho, že hodnoty v oblasti leží mezi dvěma mezemi nebo jsou rovny dolní mezi.",
		arguments: [
			{
				name: "x_oblast",
				description: "je oblast číselných hodnot x, pro které chcete zjistit požadovanou pravděpodobnost."
			},
			{
				name: "prst_oblast",
				description: "je množina hodnot pravděpodobností spojených s hodnotami argumentu X_oblast. Jsou to čísla mezi 0 a 1 kromě čísla 0."
			},
			{
				name: "dolní_limit",
				description: "je dolní mez hodnot, pro které chcete zjistit požadovanou pravděpodobnost."
			},
			{
				name: "horní_limit",
				description: "je volitelná horní mez hodnot. Jestliže tento argument nezadáte, funkce PROB vrátí pravděpodobnost toho, že hodnoty argumentu X_oblast jsou rovny argumentu Dolní_limit."
			}
		]
	},
	{
		name: "PROČISTIT",
		description: "Odstraní všechny mezery z textového řetězce kromě jednotlivých mezer mezi slovy.",
		arguments: [
			{
				name: "text",
				description: "je text, ze kterého chcete odstranit mezery."
			}
		]
	},
	{
		name: "PRŮMĚR",
		description: "Vrátí průměrnou hodnotu (aritmetický průměr) argumentů. Argumenty mohou být čísla či názvy, matice nebo odkazy, které obsahují čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentů, jejichž průměrnou hodnotu chcete zjistit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentů, jejichž průměrnou hodnotu chcete zjistit."
			}
		]
	},
	{
		name: "PRŮMODCHYLKA",
		description: "Vrátí průměrnou hodnotu absolutních odchylek datových bodů od jejich střední hodnoty. Argumenty mohou být čísla či názvy, matice nebo odkazy obsahující čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 argumentů, pro které chcete zjistit průměrnou hodnotu absolutních odchylek."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 argumentů, pro které chcete zjistit průměrnou hodnotu absolutních odchylek."
			}
		]
	},
	{
		name: "QUARTIL",
		description: "Vrátí hodnotu kvartilu množiny dat.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast buněk s číselnými hodnotami, pro které chcete zjistit hodnotu kvartilu."
			},
			{
				name: "kvartil",
				description: "je číslo: minimální hodnota = 0, první kvartil = 1, medián = 2, třetí kvartil = 3, maximální hodnota = 4."
			}
		]
	},
	{
		name: "QUARTIL.EXC",
		description: "Vrátí hodnotu kvartilu množiny dat na základě hodnot percentilu z oblasti 0..1 (s vyloučením hodnot 0 a 1).",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast buněk s číselnými hodnotami, pro které chcete zjistit hodnotu kvartilu."
			},
			{
				name: "kvartil",
				description: "je číslo: minimální hodnota = 0, první kvartil = 1, medián = 2, třetí kvartil = 3, maximální hodnota = 4."
			}
		]
	},
	{
		name: "QUARTIL.INC",
		description: "Vrátí hodnotu kvartilu množiny dat na základě hodnot percentilu z oblasti 0..1 (včetně).",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast buněk s číselnými hodnotami, pro které chcete zjistit hodnotu kvartilu."
			},
			{
				name: "kvartil",
				description: "je číslo: minimální hodnota = 0, první kvartil = 1, medián = 2, třetí kvartil = 3, maximální hodnota = 4."
			}
		]
	},
	{
		name: "QUOTIENT",
		description: "Vrátí celou část dělení.",
		arguments: [
			{
				name: "numerátor",
				description: "je dělenec."
			},
			{
				name: "denominátor",
				description: "je dělitel."
			}
		]
	},
	{
		name: "ŘÁDEK",
		description: "Vrátí číslo řádku odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je buňka nebo jediná oblast buněk, pro kterou chcete nalézt číslo řádku. Jestliže tento argument nezadáte, bude jeho hodnota buňka obsahující funkci Řádek."
			}
		]
	},
	{
		name: "RADIANS",
		description: "Převede stupně na radiány.",
		arguments: [
			{
				name: "úhel",
				description: "je úhel ve stupních, který chcete převést."
			}
		]
	},
	{
		name: "ŘÁDKY",
		description: "Vrátí počet řádků v odkazu nebo matici.",
		arguments: [
			{
				name: "pole",
				description: "je matice, maticový vzorec nebo odkaz na oblast buněk, pro kterou chcete nalézt počet řádků."
			}
		]
	},
	{
		name: "RANDBETWEEN",
		description: "Vrátí náhodné číslo mezi zadanými čísly.",
		arguments: [
			{
				name: "dolní",
				description: "je nejmenší celé číslo, které funkce RANDBETWEEN vrátí."
			},
			{
				name: "horní",
				description: "je největší celé číslo, které funkce RANDBETWEEN vrátí."
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
		description: "Vrátí pořadí čísla v seznamu čísel: jeho relativní velikost vzhledem k hodnotám v seznamu.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, jehož pořadí chcete zjistit."
			},
			{
				name: "odkaz",
				description: "je matice nebo odkaz na seznam čísel. Nečíselné hodnoty jsou ignorovány."
			},
			{
				name: "pořadí",
				description: "je číslo: pořadí v seznamu seřazeném sestupně = 0 nebo bez zadání, pořadí v seznamu seřazeném vzestupně = libovolná hodnota různá od nuly."
			}
		]
	},
	{
		name: "RANK.AVG",
		description: "Vrátí pořadí čísla v seznamu čísel: jeho relativní velikost vzhledem k ostatním hodnotám v seznamu. Má-li stejné pořadí více než jedna hodnota, bude vráceno průměrné pořadí.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, jehož pořadí chcete zjistit."
			},
			{
				name: "odkaz",
				description: "je matice nebo odkaz na seznam čísel. Nečíselné hodnoty jsou přeskočeny."
			},
			{
				name: "pořadí",
				description: "je číslo: pořadí v seznamu seřazeném sestupně = 0 nebo bez zadání, pořadí v seznamu seřazeném vzestupně = libovolná hodnota různá od nuly."
			}
		]
	},
	{
		name: "RANK.EQ",
		description: "Vrátí pořadí čísla v seznamu čísel: jeho relativní velikost vzhledem k ostatním hodnotám v seznamu. Má-li stejné pořadí více než jedna hodnota, bude vráceno nejvyšší pořadí dané množiny hodnot.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, jehož pořadí chcete zjistit."
			},
			{
				name: "odkaz",
				description: "je matice nebo odkaz na seznam čísel. Nečíselné hodnoty jsou přeskočeny."
			},
			{
				name: "pořadí",
				description: "je číslo: pořadí v seznamu seřazeném sestupně = 0 nebo bez zadání, pořadí v seznamu seřazeném vzestupně = libovolná hodnota různá od nuly."
			}
		]
	},
	{
		name: "RECEIVED",
		description: "Vrátí částku obdrženou k datu splatnosti plně investovaného cenného papíru.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "investice",
				description: "je částka investovaná do cenného papíru."
			},
			{
				name: "diskont_sazba",
				description: "je diskontní sazba cenného papíru."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "RKQ",
		description: "Vrátí druhou mocninu Pearsonova výsledného momentového korelačního koeficientu pomocí zadaných datových bodů.",
		arguments: [
			{
				name: "pole_y",
				description: "je matice nebo oblast datových bodů. Mohou to být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			},
			{
				name: "pole_x",
				description: "je matice nebo oblast datových bodů. Mohou to být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "ROK",
		description: "Vrátí rok kalendářního data, celé číslo v rozsahu od 1900 do 9999.",
		arguments: [
			{
				name: "pořadové_číslo",
				description: "je číslo v kódu aplikace Spreadsheet pro datum a čas."
			}
		]
	},
	{
		name: "ROK360",
		description: "Vrátí počet dnů mezi dvěma daty na základě roku s 360 dny (dvanáct měsíců s 30 dny).",
		arguments: [
			{
				name: "start",
				description: "Argumenty Start a Konec jsou data vymezující interval, jehož délku chcete určit."
			},
			{
				name: "konec",
				description: "Argumenty Start a Konec jsou data vymezující interval, jehož délku chcete určit."
			},
			{
				name: "metoda",
				description: "je logická hodnota určující metodu výpočtu: US (NASD) = NEPRAVDA nebo bez zadání, evropská = PRAVDA."
			}
		]
	},
	{
		name: "ROMAN",
		description: "Převede číslo napsané pomocí arabských číslic na římské číslice ve formátu textu.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo ve formě arabských číslic, které chcete převést."
			},
			{
				name: "forma",
				description: "je číslo určující typ požadovaných římských číslic."
			}
		]
	},
	{
		name: "ROUNDDOWN",
		description: "Zaokrouhlí číslo dolů směrem k nule.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo, které chcete zaokrouhlit dolů."
			},
			{
				name: "číslice",
				description: "je počet číslic, na které chcete číslo zaokrouhlit. Má-li tento argument zápornou hodnotu, bude zadané číslo zaokrouhleno směrem doleva od desetinné čárky. Pokud je roven nule nebo vynechán, bude zadané číslo zaokrouhleno na nejbližší celé číslo."
			}
		]
	},
	{
		name: "ROUNDUP",
		description: "Zaokrouhlí číslo nahoru směrem od nuly.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo, které chcete zaokrouhlit nahoru."
			},
			{
				name: "číslice",
				description: "je počet číslic, na které chcete číslo zaokrouhlit. Má-li tento argument zápornou hodnotu, bude zadané číslo zaokrouhleno směrem doleva od desetinné čárky. Pokud má hodnotu nula nebo je vynechán, bude zadané číslo zaokrouhleno na nejbližší celé číslo."
			}
		]
	},
	{
		name: "RRI",
		description: "Vrátí odpovídající úrokovou sazbu pro růst investic.",
		arguments: [
			{
				name: "nper",
				description: "je počet období pro investici"
			},
			{
				name: "souč_hod",
				description: "je současná hodnota investice"
			},
			{
				name: "bud_hod",
				description: "je budoucí hodnota investice"
			}
		]
	},
	{
		name: "RTD",
		description: "Načte data v reálném čase z programu podporujícího automatizaci modulu COM.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ID_programu",
				description: "je název ID registrovaného doplňku pro automatizaci modelu COM. Název je nutné uzavřít do uvozovek."
			},
			{
				name: "server",
				description: "je název serveru, na kterém má být doplněk spuštěn. Název je nutné uzavřít do uvozovek. Pokud je doplněk spuštěn v místním počítači, použijte prázdný řetězec."
			},
			{
				name: "téma1",
				description: "je 1 až 38 parametrů určujících jednotlivá data."
			},
			{
				name: "téma2",
				description: "je 1 až 38 parametrů určujících jednotlivá data."
			}
		]
	},
	{
		name: "SEC",
		description: "Vrátí sekans úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, pro který chcete získat sekans"
			}
		]
	},
	{
		name: "SECH",
		description: "Vrátí hyperbolický sekans úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, pro který chcete získat hyperbolický sekans"
			}
		]
	},
	{
		name: "SEKUNDA",
		description: "Vrátí sekundy, číslo od 0 do 59.",
		arguments: [
			{
				name: "pořadové_číslo",
				description: "je číslo v kódu aplikace Spreadsheet pro datum a čas nebo text ve formátu času, například 16:48:23 nebo 4:48:47 odp."
			}
		]
	},
	{
		name: "SERIESSUM",
		description: "Vrátí součet mocninné řady určené podle vzorce.",
		arguments: [
			{
				name: "x",
				description: "je počáteční hodnota mocninné řady."
			},
			{
				name: "n",
				description: "je počáteční mocnina, na kterou chcete umocnit argument x."
			},
			{
				name: "m",
				description: "je hodnota, o kterou chcete zvyšovat argument n pro každý člen řady."
			},
			{
				name: "koeficient",
				description: "je množina koeficientů, kterými je každá následující mocnina argumentu x vynásobena."
			}
		]
	},
	{
		name: "SHEET",
		description: "Vrátí číslo listu odkazovaného listu.",
		arguments: [
			{
				name: "hodnota",
				description: "je název listu nebo odkaz, pro který chcete zjistit číslo listu. Pokud ho vynecháte, vrátí číslo listu obsahujícího funkci"
			}
		]
	},
	{
		name: "SHEETS",
		description: "Vrátí počet listů v odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je odkaz, pro který chcete zjistit počet listů, které obsahuje. Pokud ho vynecháte, vrátí počet listů v sešitu obsahujícím funkci"
			}
		]
	},
	{
		name: "SIGN",
		description: "Vrátí znaménko čísla: číslo 1 pro kladné číslo, 0 pro nulu nebo -1 pro záporné číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo."
			}
		]
	},
	{
		name: "SIN",
		description: "Vrátí sinus úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, jehož sinus chcete zjistit. Stupně*PI()/180 = radiány."
			}
		]
	},
	{
		name: "SINH",
		description: "Vrátí hyperbolický sinus čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo."
			}
		]
	},
	{
		name: "SKEW",
		description: "Vrátí zešikmení rozdělení: charakteristika stupně asymetrie rozdělení kolem jeho střední hodnoty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, pro které chcete zjistit zešikmení."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo názvů, matic či odkazů obsahujících čísla, pro které chcete zjistit zešikmení."
			}
		]
	},
	{
		name: "SKEW.P",
		description: "Vrátí zešikmení rozdělení založené na základním souboru: charakteristika stupně asymetrie rozdělení kolem jeho střední hodnoty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 254 čísel nebo názvů, matic nebo odkazů obsahujících čísla, pro která chcete zjistit zešikmení"
			},
			{
				name: "číslo2",
				description: "je 1 až 254 čísel nebo názvů, matic nebo odkazů obsahujících čísla, pro která chcete zjistit zešikmení"
			}
		]
	},
	{
		name: "SLOPE",
		description: "Vrátí směrnici lineární regresní čáry proložené zadanými datovými body.",
		arguments: [
			{
				name: "pole_y",
				description: "je matice nebo oblast buněk závislých číselných datových bodů. Mohou to být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			},
			{
				name: "pole_x",
				description: "je množina nezávislých datových bodů. Mohou to být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "SLOUPCE",
		description: "Vrátí počet sloupců v matici nebo odkazu.",
		arguments: [
			{
				name: "pole",
				description: "je matice, maticový vzorec nebo odkaz na oblast buněk, pro které hledáte počet sloupců."
			}
		]
	},
	{
		name: "SLOUPEC",
		description: "Vrátí číslo sloupce odkazu.",
		arguments: [
			{
				name: "odkaz",
				description: "je buňka nebo souvislá oblast buněk, jejichž číslo sloupce hledáte. Jestliže tento argument nezadáte, bude použita buňka obsahující funkci SLOUPEC."
			}
		]
	},
	{
		name: "SMALL",
		description: "Vrátí k-tou nejmenší hodnotu v množině dat, například páté nejmenší číslo.",
		arguments: [
			{
				name: "pole",
				description: "je matice nebo oblast číselných dat, pro kterou chcete určit k-tou nejmenší hodnotu."
			},
			{
				name: "k",
				description: "je pozice hledané hodnoty (počítáno od nejmenší hodnoty) v matici nebo oblasti buněk."
			}
		]
	},
	{
		name: "SMODCH",
		description: "Vypočte směrodatnou odchylku základního souboru, který byl zadán jako argument (přeskočí logické hodnoty a text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají základnímu souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají základnímu souboru."
			}
		]
	},
	{
		name: "SMODCH.P",
		description: "Vypočte směrodatnou odchylku základního souboru, který byl zadán jako argument (přeskočí logické hodnoty a text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají základnímu souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají základnímu souboru."
			}
		]
	},
	{
		name: "SMODCH.VÝBĚR",
		description: "Odhadne směrodatnou odchylku výběru (přeskočí logické hodnoty a text ve výběru).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají výběru ze základního souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají výběru ze základního souboru."
			}
		]
	},
	{
		name: "SMODCH.VÝBĚR.S",
		description: "Odhadne směrodatnou odchylku výběru (přeskočí logické hodnoty a text ve výběru).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají výběru ze základního souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel nebo odkazů obsahujících čísla, které odpovídají výběru ze základního souboru."
			}
		]
	},
	{
		name: "SOUČHODNOTA",
		description: "Vrátí současnou hodnotu investice: celkovou hodnotu série budoucích plateb.",
		arguments: [
			{
				name: "sazba",
				description: "je úroková sazba vztažená na jedno období. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4."
			},
			{
				name: "pper",
				description: "je celkový počet platebních období investice."
			},
			{
				name: "splátka",
				description: "je splátka provedená v každém období. Během období životnosti investice ji nelze měnit."
			},
			{
				name: "bud_hod",
				description: "je budoucí hodnota nebo hotovostní bilance, kterou chcete dosáhnout po splacení poslední platby."
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na konci období = 0 nebo bez zadání, splátka na začátku období = 1."
			}
		]
	},
	{
		name: "SOUČIN",
		description: "Vynásobí všechna čísla zadaná jako argumenty.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, logických hodnot nebo čísel ve formátu textu, které chcete vynásobit."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, logických hodnot nebo čísel ve formátu textu, které chcete vynásobit."
			}
		]
	},
	{
		name: "SOUČIN.MATIC",
		description: "Vrátí součin dvou matic, matici se stejným počtem řádků jako matice argumentu Pole1 a stejným počtem sloupců jako matice argumentu Pole2.",
		arguments: [
			{
				name: "pole1",
				description: "je první matice čísel, kterou chcete násobit. Počet sloupců musí být stejný jako počet řádků matice argumentu Pole2."
			},
			{
				name: "pole2",
				description: "je první matice čísel, kterou chcete násobit. Počet sloupců musí být stejný jako počet řádků matice argumentu Pole2."
			}
		]
	},
	{
		name: "SOUČIN.SKALÁRNÍ",
		description: "Vrátí součet součinů odpovídajících oblastí nebo matic.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "pole1",
				description: "je 2 až 255 matic, jejichž jednotlivé položky chcete násobit a sečíst. U všech matice musí být zadány stejné dimenze."
			},
			{
				name: "pole2",
				description: "je 2 až 255 matic, jejichž jednotlivé položky chcete násobit a sečíst. U všech matice musí být zadány stejné dimenze."
			},
			{
				name: "pole3",
				description: "je 2 až 255 matic, jejichž jednotlivé položky chcete násobit a sečíst. U všech matice musí být zadány stejné dimenze."
			}
		]
	},
	{
		name: "SQRTPI",
		description: "Vrátí druhou odmocninu výrazu (číslo * pí).",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, kterým je číslo pí násobeno."
			}
		]
	},
	{
		name: "STANDARDIZE",
		description: "Vrátí normalizovanou hodnotu z rozdělení určeného střední hodnotou a směrodatnou odchylkou.",
		arguments: [
			{
				name: "x",
				description: "je hodnota, kterou chcete normalizovat."
			},
			{
				name: "střed_hodn",
				description: "je aritmetická střední hodnota rozdělení."
			},
			{
				name: "sm_odch",
				description: "je směrodatná odchylka rozdělení, kladné číslo."
			}
		]
	},
	{
		name: "STDEVA",
		description: "Odhadne směrodatnou odchylku výběru. Nepřeskočí logické hodnoty a text. Text a logická hodnota NEPRAVDA mají hodnotu 0, logická hodnota PRAVDA má hodnotu 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnot odpovídajících výběru ze základního souboru. Mohou to být hodnoty, názvy nebo odkazy, které obsahují hodnoty."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnot odpovídajících výběru ze základního souboru. Mohou to být hodnoty, názvy nebo odkazy, které obsahují hodnoty."
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Vypočte směrodatnou odchylku základního souboru. Nepřeskočí logické hodnoty a text. Text a logická hodnota NEPRAVDA mají hodnotu 0, logická hodnota PRAVDA má hodnotu 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnot odpovídajících základnímu souboru. Mohou to být hodnoty, názvy, matice nebo odkazy, které obsahují hodnoty."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnot odpovídajících základnímu souboru. Mohou to být hodnoty, názvy, matice nebo odkazy, které obsahují hodnoty."
			}
		]
	},
	{
		name: "STEJNÉ",
		description: "Ověří, zda jsou dva textové řetězce stejné a vrátí hodnotu PRAVDA nebo NEPRAVDA. Tato funkce rozlišuje malá a velká písmena.",
		arguments: [
			{
				name: "text1",
				description: "je první textový řetězec."
			},
			{
				name: "text2",
				description: "je druhý textový řetězec."
			}
		]
	},
	{
		name: "STEYX",
		description: "Vrátí standardní chybu předpovězené hodnoty y pro každou hodnotu x v regresi.",
		arguments: [
			{
				name: "pole_y",
				description: "je matice nebo oblast závislých datových bodů. Mohou to být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			},
			{
				name: "pole_x",
				description: "je matice nebo oblast nezávislých datových bodů. Mohou to být čísla nebo názvy, matice nebo odkazy obsahující čísla."
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Vrátí souhrn na listu nebo v databázi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funkce",
				description: "je číslo od 1 do 11 určující souhrnnou funkci použitou pro souhrn."
			},
			{
				name: "odkaz1",
				description: "je 1 až 254 oblastí nebo odkazů, pro které chcete vypočítat souhrn."
			}
		]
	},
	{
		name: "SUMA",
		description: "Sečte všechna čísla v oblasti buněk.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, které chcete sečíst. Logické hodnoty a text budou v buňkách přeskočeny. Pokud jsou však zadány jako argumenty, budou zahrnuty."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, které chcete sečíst. Logické hodnoty a text budou v buňkách přeskočeny. Pokud jsou však zadány jako argumenty, budou zahrnuty."
			}
		]
	},
	{
		name: "SUMA.ČTVERCŮ",
		description: "Vrátí součet druhých mocnin argumentů. Argumenty mohou představovat čísla, matice, názvy nebo odkazy na buňky obsahující čísla.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 čísel, matic, názvů nebo odkazů na matice, pro které chcete zjistit součet mocnin."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 čísel, matic, názvů nebo odkazů na matice, pro které chcete zjistit součet mocnin."
			}
		]
	},
	{
		name: "SUMIF",
		description: "Sečte buňky vybrané podle zadaných kritérií.",
		arguments: [
			{
				name: "oblast",
				description: "je oblast buněk, které chcete sečíst."
			},
			{
				name: "kritéria",
				description: "je kritérium ve formě čísla, výrazu nebo textu určující, které buňky budou sečteny."
			},
			{
				name: "součet",
				description: "jsou skutečné buňky, které budou sečteny. Jestliže tento argument nezadáte, budou použity buňky oblasti. "
			}
		]
	},
	{
		name: "SUMIFS",
		description: "Sečte buňky určené danou sadou podmínek nebo kritérií.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "oblast_součtu",
				description: "jsou vlastní buňky určené k součtu."
			},
			{
				name: "oblast_kritérií",
				description: "představuje oblast buněk, které chcete vyhodnotit na základě určené podmínky."
			},
			{
				name: "kritérium",
				description: "je podmínka nebo kritérium v podobě čísla, výrazu nebo textu definujících buňky, které chcete sečíst."
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Vypočte součet rozdílů čtverců dvou odpovídajících oblastí nebo polí.",
		arguments: [
			{
				name: "pole_x",
				description: "je první oblast nebo matice čísel. Může to být číslo nebo název, matice nebo odkaz obsahující čísla."
			},
			{
				name: "pole_y",
				description: "je druhá oblast nebo matice čísel. Může to být číslo nebo název, matice nebo odkaz obsahující čísla."
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Vrátí celkový součet součtů čtverců čísel ve dvou odpovídajících oblastech nebo maticích.",
		arguments: [
			{
				name: "pole_x",
				description: "je první oblast nebo matice čísel. Může to být číslo nebo název, matice nebo odkaz obsahující čísla."
			},
			{
				name: "pole_y",
				description: "je druhá oblast nebo matice čísel. Může to být číslo nebo název, matice nebo odkaz obsahující čísla."
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Vypočte součet čtverců rozdílů dvou odpovídajících oblastí nebo matic.",
		arguments: [
			{
				name: "pole_x",
				description: "je první oblast nebo matice hodnot. Může to být číslo nebo název, matice nebo odkaz obsahující čísla."
			},
			{
				name: "pole_y",
				description: "je druhá oblast nebo matice hodnot. Může to být číslo nebo název, matice nebo odkaz obsahující čísla."
			}
		]
	},
	{
		name: "SVYHLEDAT",
		description: "Vyhledá hodnotu v krajním levém sloupci tabulky a vrátí hodnotu ze zadaného sloupce ve stejném řádku. Tabulka musí být standardně seřazena vzestupně.",
		arguments: [
			{
				name: "hledat",
				description: "je hodnota hledaná v prvním sloupci tabulky. Může to být hodnota, odkaz nebo textový řetězec."
			},
			{
				name: "tabulka",
				description: "je prohledávaná tabulka s textem, čísly nebo logickými hodnotami. Argument Tabulka může být odkaz na oblast nebo název oblasti."
			},
			{
				name: "sloupec",
				description: "je číslo sloupce v argumentu Tabulka, ve kterém chcete vyhledat odpovídající hodnotu. První sloupec hodnot v tabulce je sloupec číslo 1."
			},
			{
				name: "typ",
				description: "je logická hodnota: nalézt nejbližší odpovídající hodnotu v prvním sloupci (seřazeném vzestupně) = PRAVDA nebo bez zadání, nalézt přesnou odpovídající hodnotu = NEPRAVDA."
			}
		]
	},
	{
		name: "T",
		description: "Ověří, zda je argument Hodnota text. Jestliže ano, vrátí text, jestliže ne, vrátí uvozovky (prázdný text).",
		arguments: [
			{
				name: "hodnota",
				description: "je testovaná hodnota."
			}
		]
	},
	{
		name: "T.DIST",
		description: "Vrátí hodnotu levostranného Studentova t-rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pro kterou chcete zjistit hodnotu rozdělení."
			},
			{
				name: "volnost",
				description: "je celé číslo představující počet stupňů volnosti, které určují rozdělení."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, funkce hustoty pravděpodobnosti = NEPRAVDA."
			}
		]
	},
	{
		name: "T.DIST.2T",
		description: "Vrátí hodnotu oboustranného Studentova t-rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pro kterou chcete zjistit hodnotu rozdělení."
			},
			{
				name: "volnost",
				description: "je celé číslo představující počet stupňů volnosti, které určují rozdělení."
			}
		]
	},
	{
		name: "T.DIST.RT",
		description: "Vrátí hodnotu pravostranného Studentova t-rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pro kterou chcete zjistit hodnotu rozdělení."
			},
			{
				name: "volnost",
				description: "je celé číslo představující počet stupňů volnosti, které určují rozdělení."
			}
		]
	},
	{
		name: "T.INV",
		description: "Vrátí levostrannou inverzní funkci k distribuční funkci Studentova t-rozdělení.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost oboustranného Studentova t-rozdělení. Může to být číslo mezi 0 a 1 včetně."
			},
			{
				name: "volnost",
				description: "je kladné celé číslo představující počet stupňů volnosti, které určují rozdělení."
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Vrátí oboustrannou inverzní funkci k distribuční funkci Studentova t-rozdělení.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost oboustranného Studentova t-rozdělení. Může to být číslo mezi 0 a 1 včetně."
			},
			{
				name: "volnost",
				description: "je kladné celé číslo představující počet stupňů volnosti, které určují rozdělení."
			}
		]
	},
	{
		name: "T.TEST",
		description: "Vrátí pravděpodobnost odpovídající Studentovu t-testu.",
		arguments: [
			{
				name: "matice1",
				description: "je první množina dat."
			},
			{
				name: "matice2",
				description: "je druhá množina dat."
			},
			{
				name: "chvosty",
				description: "určuje počet chvostů rozdělení, které chcete vrátit: jeden chvost rozdělení = 1, dva chvosty rozdělení = 2."
			},
			{
				name: "typ",
				description: "je typ t-testu: spárované výběry = 1, dva výběry se shodným rozptylem = 2, dva výběry s různým rozptylem = 3."
			}
		]
	},
	{
		name: "TBILLEQ",
		description: "Vrátí výnos směnky státní pokladny ekvivalentní výnosu obligace.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování směnky státní pokladny vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti směnky státní pokladny vyjádřené pořadovým číslem."
			},
			{
				name: "diskont_sazba",
				description: "je diskontní sazba směnky státní pokladny."
			}
		]
	},
	{
		name: "TBILLPRICE",
		description: "Vrátí cenu směnky státní pokladny o nominální hodnotě 100 Kč.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování směnky státní pokladny vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti směnky státní pokladny vyjádřené pořadovým číslem."
			},
			{
				name: "diskont_sazba",
				description: "je diskontní sazba směnky státní pokladny."
			}
		]
	},
	{
		name: "TBILLYIELD",
		description: "Vrátí výnos směnky státní pokladny.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování směnky státní pokladny vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti směnky státní pokladny vyjádřené pořadovým číslem."
			},
			{
				name: "cena",
				description: "je cena směnky státní pokladny o nominální hodnotě 100 Kč."
			}
		]
	},
	{
		name: "TDIST",
		description: "Vrátí hodnotu Studentova t-rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je číselná hodnota, pro kterou chcete zjistit rozdělení."
			},
			{
				name: "volnost",
				description: "je celé číslo představující počet stupňů volnosti, které určují rozdělení."
			},
			{
				name: "chvosty",
				description: "určuje počet chvostů rozdělení, které chcete vrátit: jeden chvost rozdělení = 1, dva chvosty rozdělení = 2."
			}
		]
	},
	{
		name: "TG",
		description: "Vrátí tangens úhlu.",
		arguments: [
			{
				name: "číslo",
				description: "je úhel v radiánech, jehož tangens chcete zjistit. Stupně * PI()/180 = radiány."
			}
		]
	},
	{
		name: "TGH",
		description: "Vrátí hyperbolický tangens čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je libovolné reálné číslo."
			}
		]
	},
	{
		name: "TINV",
		description: "Vrátí inverzní funkci (dva chvosty) ke Studentovu t-rozdělení.",
		arguments: [
			{
				name: "pravděpodobnost",
				description: "je pravděpodobnost Studentova t-rozdělení (dva chvosty). Může to být číslo mezi 0 a 1 (včetně)."
			},
			{
				name: "volnost",
				description: "je kladné celé číslo představující počet stupňů volnosti, které určují rozdělení."
			}
		]
	},
	{
		name: "TRANSPOZICE",
		description: "Převede vodorovnou oblast buněk na svislou nebo naopak.",
		arguments: [
			{
				name: "pole",
				description: "je oblast buněk listu nebo matice hodnot, kterou chcete transponovat."
			}
		]
	},
	{
		name: "TRIMMEAN",
		description: "Vrátí průměrnou hodnotu vnitřní části množiny datových hodnot.",
		arguments: [
			{
				name: "pole",
				description: "je oblast nebo matice hodnot, kterou chcete oříznout a pro kterou chcete vypočítat průměr zbývajících hodnot."
			},
			{
				name: "procenta",
				description: "je zlomek udávající počet datových bodů, které chcete vyloučit z horní a dolní části množiny dat."
			}
		]
	},
	{
		name: "TTEST",
		description: "Vrátí pravděpodobnost odpovídající Studentovu t-testu.",
		arguments: [
			{
				name: "matice1",
				description: "je první množina dat."
			},
			{
				name: "matice2",
				description: "je druhá množina dat."
			},
			{
				name: "chvosty",
				description: "určuje počet chvostů rozdělení, které chcete vrátit: jeden chvost rozdělení = 1, dva chvosty rozdělení = 2."
			},
			{
				name: "typ",
				description: "je typ t-testu: spárované výběry = 1, dva výběry se shodným rozptylem = 2, dva výběry s různým rozptylem = 3."
			}
		]
	},
	{
		name: "TYP",
		description: "Vrátí celé číslo představující datový typ hodnoty: číslo = 1, text = 2, logická hodnota = 4, chybová hodnota = 16, matice = 64.",
		arguments: [
			{
				name: "hodnota",
				description: "je libovolná hodnota."
			}
		]
	},
	{
		name: "UNICODE",
		description: "Vrátí číslo (bod kódu) odpovídající prvnímu znaku textu.",
		arguments: [
			{
				name: "text",
				description: "je znak, pro který chcete zjistit hodnotu kódu Unicode"
			}
		]
	},
	{
		name: "ÚROKOVÁ.MÍRA",
		description: "Vrátí úrokovou sazbu vztaženou na období půjčky nebo investice. Chcete-li například zadat čtvrtletní splátky realizované 6. dubna, použijte 6%/4.",
		arguments: [
			{
				name: "pper",
				description: "je celkový počet platebních období půjčky nebo investice."
			},
			{
				name: "splátka",
				description: "je splátka provedená v každém období. Během období životnosti půjčky nebo investice ji nelze měnit."
			},
			{
				name: "souč_hod",
				description: "je současná hodnota: celková hodnota série budoucích plateb."
			},
			{
				name: "bud_hod",
				description: "je budoucí hodnota nebo hotovostní bilance, kterou chcete dosáhnout po splacení poslední platby. Jestliže argument Bud_hod nezadáte, bude jeho hodnota 0."
			},
			{
				name: "typ",
				description: "je logická hodnota: splátka na začátku období = 1; splátka na konci období = 0 nebo bez zadání."
			},
			{
				name: "odhad",
				description: "je předpokládaný odhad sazby. Jestliže argument Odhad nezadáte, bude jeho hodnota 0,1 (10 procent)."
			}
		]
	},
	{
		name: "USEKNOUT",
		description: "Zkrátí číslo na celé číslo odstraněním desetinné nebo zlomkové části čísla.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete zkrátit."
			},
			{
				name: "desetiny",
				description: "je číslo určující přesnost zkrácení. Jestliže tento argument nezadáte, bude jeho hodnota 0."
			}
		]
	},
	{
		name: "VAR",
		description: "Vypočte rozptyl základního souboru (přeskočí logické hodnoty a text v základním souboru).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentů odpovídajících základnímu souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentů odpovídajících základnímu souboru."
			}
		]
	},
	{
		name: "VAR.P",
		description: "Vypočte rozptyl celého základního souboru (přeskočí logické hodnoty a text v základním souboru).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentů odpovídajících základnímu souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentů odpovídajících základnímu souboru."
			}
		]
	},
	{
		name: "VAR.S",
		description: "Odhadne rozptyl výběru (přeskočí logické hodnoty a text ve výběru).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 numerických argumentů odpovídajících výběru ze základního souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 numerických argumentů odpovídajících výběru ze základního souboru."
			}
		]
	},
	{
		name: "VAR.VÝBĚR",
		description: "Odhadne rozptyl výběru (přeskočí logické hodnoty a text ve výběru).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "číslo1",
				description: "je 1 až 255 číselných argumentů odpovídajících výběru ze základního souboru."
			},
			{
				name: "číslo2",
				description: "je 1 až 255 číselných argumentů odpovídajících výběru ze základního souboru."
			}
		]
	},
	{
		name: "VARA",
		description: "Odhadne rozptyl výběru. Nepřeskočí logické hodnoty a text. Text a logická hodnota NEPRAVDA mají hodnotu 0, logická hodnota PRAVDA má hodnotu 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnot argumentů odpovídajících výběru ze základního souboru."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnot argumentů odpovídajících výběru ze základního souboru."
			}
		]
	},
	{
		name: "VARPA",
		description: "Vypočte rozptyl základního souboru. Nepřeskočí logické hodnoty a text. Text a logická hodnota NEPRAVDA mají hodnotu 0, logická hodnota PRAVDA má hodnotu 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "hodnota1",
				description: "je 1 až 255 hodnot argumentů odpovídajících základnímu souboru."
			},
			{
				name: "hodnota2",
				description: "je 1 až 255 hodnot argumentů odpovídajících základnímu souboru."
			}
		]
	},
	{
		name: "VELKÁ",
		description: "Převede všechna písmena textového řetězce na velká.",
		arguments: [
			{
				name: "text",
				description: "je text, který chcete převést na velká písmena. Může to být odkaz nebo textový řetězec."
			}
		]
	},
	{
		name: "VELKÁ2",
		description: "Převede textový řetězec na formát, kdy jsou první písmena všech slov velká a ostatní písmena malá.",
		arguments: [
			{
				name: "text",
				description: "je text v uvozovkách, vzorec, jehož výsledkem je text, nebo odkaz na buňku s textem, ve kterém chcete převést první písmena slov na velká."
			}
		]
	},
	{
		name: "VVYHLEDAT",
		description: "Prohledá horní řádek tabulky nebo matice hodnot a vrátí hodnotu ze zadaného řádku obsaženou ve stejném sloupci.",
		arguments: [
			{
				name: "hledat",
				description: "je hodnota, kterou chcete vyhledat v prvním řádku tabulky. Může to být hodnota, odkaz nebo textový řetězec."
			},
			{
				name: "tabulka",
				description: "je prohledávaná tabulka obsahující text, čísla nebo logické hodnoty. Argument Tabulka může být odkaz na oblast nebo název oblasti."
			},
			{
				name: "řádek",
				description: "je číslo řádku v argumentu Tabulka, ze kterého bude vrácena odpovídající hodnota. První řádek hodnot tabulky je řádek číslo 1."
			},
			{
				name: "typ",
				description: "je logická hodnota: najít nejbližší odpovídající hodnotu v horním řádku (hodnoty jsou seřazeny vzestupně) = PRAVDA nebo bez zadání, najít přesnou odpovídající hodnotu = NEPRAVDA."
			}
		]
	},
	{
		name: "VYČISTIT",
		description: "Odstraní z textu všechny netisknutelné znaky.",
		arguments: [
			{
				name: "text",
				description: "je libovolná informace na listu, ze které chcete odstranit netisknutelné znaky."
			}
		]
	},
	{
		name: "VYHLEDAT",
		description: "Vyhledá požadovanou hodnotu v matici nebo v oblasti obsahující jeden řádek nebo jeden sloupec. Funkce je poskytnuta k zajištění zpětné kompatibility.",
		arguments: [
			{
				name: "co",
				description: "je hodnota vyhledávaná pomocí funkce VYHLEDAT v argumentu Hledat. Může to být číslo, text, logická hodnota, název nebo odkaz na hodnotu."
			},
			{
				name: "hledat",
				description: "je oblast obsahující pouze jeden řádek nebo jeden sloupec se vzestupně seřazeným textem, čísly nebo logickými hodnotami."
			},
			{
				name: "výsledek",
				description: "je oblast obsahující pouze jeden řádek nebo sloupec. Velikost oblasti je stejná jako velikost oblasti argumentu Hledat."
			}
		]
	},
	{
		name: "WEEKNUM",
		description: "Vrátí číslo týdne v roce.",
		arguments: [
			{
				name: "pořad_číslo",
				description: "je kód aplikace Spreadsheet pro datum a čas používaný k výpočtu data a času."
			},
			{
				name: "typ",
				description: "je číslo (1 nebo 2) určující typ návratové hodnoty."
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Vrátí hodnotu Weibullova rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (nezáporné číslo), pro kterou chcete zjistit rozdělení."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, hromadná pravděpodobnostní funkce = NEPRAVDA."
			}
		]
	},
	{
		name: "WEIBULL.DIST",
		description: "Vrátí hodnotu Weibullova rozdělení.",
		arguments: [
			{
				name: "x",
				description: "je hodnota (nezáporné číslo), pro kterou chcete zjistit rozdělení."
			},
			{
				name: "alfa",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "beta",
				description: "je parametr rozdělení, kladné číslo."
			},
			{
				name: "kumulativní",
				description: "je logická hodnota: kumulativní distribuční funkce = PRAVDA, hromadná pravděpodobnostní funkce = NEPRAVDA."
			}
		]
	},
	{
		name: "WORKDAY",
		description: "Vrátí pořadové číslo data před nebo po zadaném počtu pracovních dnů.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "dny",
				description: "je počet pracovních dnů před nebo po datu zadaném v argumentu Začátek."
			},
			{
				name: "svátky",
				description: "je volitelná řada jednoho nebo více pořadových čísel dat, která chcete vyloučit z kalendáře pracovních dnů, například státní nebo pohyblivé svátky."
			}
		]
	},
	{
		name: "WORKDAY.INTL",
		description: "Vrátí pořadové číslo data před nebo po zadaném počtu pracovních dnů s vlastními parametry víkendu.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "dny",
				description: "je počet dnů kromě víkendu a svátků před nebo po datu zadaném v argumentu začátek."
			},
			{
				name: "víkend",
				description: "je číslo nebo řetězec určující víkendy"
			},
			{
				name: "svátky",
				description: "je volitelná matice jednoho nebo více pořadových čísel dat, která chcete vyloučit z kalendáře pracovních dnů, například státní nebo pohyblivé svátky."
			}
		]
	},
	{
		name: "XIRR",
		description: "Vrátí vnitřní výnosnost pro harmonogram peněžních toků.",
		arguments: [
			{
				name: "hodnoty",
				description: "je posloupnost peněžních toků odpovídajících harmonogramu dat plateb."
			},
			{
				name: "data",
				description: "je harmonogram dat plateb odpovídající platbám peněžních toků. "
			},
			{
				name: "odhad",
				description: "je odhad výsledku funkce XIRR."
			}
		]
	},
	{
		name: "XNPV",
		description: "Vrátí čistou současnou hodnotu pro harmonogram peněžních toků.",
		arguments: [
			{
				name: "sazba",
				description: "je diskontní sazba pro peněžní toky."
			},
			{
				name: "hodnoty",
				description: "je posloupnost peněžních toků odpovídajících harmonogramu dat plateb."
			},
			{
				name: "data",
				description: "je harmonogram plateb odpovídající platbám peněžních toků."
			}
		]
	},
	{
		name: "XOR",
		description: "Vrátí logickou hodnotu Výhradní nebo všech argumentů.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logická1",
				description: "je 1 až 254 podmínek, které chcete testovat a které mohou mít hodnotu PRAVDA nebo NEPRAVDA a mohou být logické hodnoty, matice nebo odkazy"
			},
			{
				name: "logická2",
				description: "je 1 až 254 podmínek, které chcete testovat a které mohou mít hodnotu PRAVDA nebo NEPRAVDA a mohou být logické hodnoty, matice nebo odkazy"
			}
		]
	},
	{
		name: "YEARFRAC",
		description: "Vrátí část roku vyjádřenou zlomkem a představující počet celých dnů mezi počátečním a koncovým datem.",
		arguments: [
			{
				name: "začátek",
				description: "je pořadové číslo, které představuje počáteční datum."
			},
			{
				name: "konec",
				description: "je pořadové číslo, které představuje koncové datum."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "YIELDDISC",
		description: "Vrátí roční výnos diskontního cenného papíru, například směnky státní pokladny.",
		arguments: [
			{
				name: "vypořádání",
				description: "je datum zúčtování cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "splatnost",
				description: "je datum splatnosti cenného papíru vyjádřené pořadovým číslem."
			},
			{
				name: "cena",
				description: "je cena cenného papíru o nominální hodnotě 100 Kč."
			},
			{
				name: "zaruč_cena",
				description: "je výkupní hodnota cenného papíru o nominální hodnotě 100 Kč."
			},
			{
				name: "základna",
				description: "je typ výpočtu určující počet dnů v měsíci, který chcete použít."
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Vrátí P-hodnotu (jeden chvost) z-testu.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat, kterou chcete použít k testování argumentu x."
			},
			{
				name: "x",
				description: "je testovaná hodnota."
			},
			{
				name: "sigma",
				description: "je známá směrodatná odchylka základního souboru. Jestliže tento argument nezadáte, bude použita směrodatná odchylka výběru."
			}
		]
	},
	{
		name: "ZAOKR.DOLŮ",
		description: "Zaokrouhlí číslo dolů na nejbližší násobek zadané hodnoty významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je číselná hodnota, kterou chcete zaokrouhlit."
			},
			{
				name: "významnost",
				description: "je násobek, na který chcete číslo zaokrouhlit. Argumenty číslo a významnost musí být zároveň buď kladná, nebo záporná čísla."
			}
		]
	},
	{
		name: "ZAOKR.NAHORU",
		description: "Zaokrouhlí číslo nahoru na nejbližší násobek zadané hodnoty významnosti.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit."
			},
			{
				name: "významnost",
				description: "je násobek, na který chcete číslo zaokrouhlit."
			}
		]
	},
	{
		name: "ZAOKROUHLIT",
		description: "Zaokrouhlí číslo na zadaný počet číslic.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete zaokrouhlit."
			},
			{
				name: "číslice",
				description: "je počet číslic, na které chcete požadované číslo zaokrouhlit. Jestliže zadáte záporné číslo, bude zadané číslo zaokrouhleno směrem doleva od desetinné čárky. Pokud je hodnota argumentu nula, bude zadané číslo zaokrouhleno na nejbližší celé číslo."
			}
		]
	},
	{
		name: "ZAOKROUHLIT.NA.LICHÉ",
		description: "Zaokrouhlí kladné číslo nahoru a záporné číslo dolů na nejbližší liché celé číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je hodnota, kterou chcete zaokrouhlit."
			}
		]
	},
	{
		name: "ZAOKROUHLIT.NA.SUDÉ",
		description: "Zaokrouhlí kladné číslo nahoru a záporné číslo dolů na nejbližší sudé celé číslo.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete zaokrouhlit."
			}
		]
	},
	{
		name: "ZAOKROUHLIT.NA.TEXT",
		description: "Zaokrouhlí číslo na zadaný počet desetinných míst a vrátí výsledek ve formátu textu s čárkami nebo bez čárek.",
		arguments: [
			{
				name: "číslo",
				description: "je číslo, které chcete zaokrouhlit a převést na text."
			},
			{
				name: "desetiny",
				description: "je počet desetinných míst vpravo od desetinné čárky. Jestliže argument Desetiny nezadáte, bude jeho hodnota 2."
			},
			{
				name: "bez_čárky",
				description: "je logická hodnota: nezobrazovat čárky ve vráceném textu = PRAVDA, zobrazit čárky ve vráceném textu = NEPRAVDA nebo bez zadání."
			}
		]
	},
	{
		name: "ZÍSKATKONTDATA",
		description: "Extrahuje data uložená v kontingenční tabulce.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "datové_pole",
				description: "je název datového pole, ze kterého mají být data extrahována."
			},
			{
				name: "kontingenční_tabulka",
				description: "je odkaz na buňku nebo oblast buněk v kontingenční tabulce obsahující data, která chcete načíst."
			},
			{
				name: "pole",
				description: "pole, na které chcete odkazovat."
			},
			{
				name: "položka",
				description: "položka pole, na níž chcete odkazovat."
			}
		]
	},
	{
		name: "ZLEVA",
		description: "Vrátí zadaný počet znaků od počátku textového řetězce.",
		arguments: [
			{
				name: "text",
				description: "je textový řetězec obsahující znaky, které chcete extrahovat."
			},
			{
				name: "znaky",
				description: "určuje počet znaků, které budou pomocí funkce ZLEVA extrahovány. Jestliže tento argument nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "ZNAK",
		description: "Vrátí znak určený číslem kódu ze znakové sady definované v používaném počítači.",
		arguments: [
			{
				name: "kód",
				description: "je číslo od 1 do 255, které určuje požadovaný znak."
			}
		]
	},
	{
		name: "ZPRAVA",
		description: "Vrátí zadaný počet znaků od konce textového řetězce.",
		arguments: [
			{
				name: "text",
				description: "je textový řetězec obsahující znaky, které chcete extrahovat."
			},
			{
				name: "znaky",
				description: "určuje počet znaků, které chcete extrahovat. Jestliže tento argument nezadáte, bude jeho hodnota 1."
			}
		]
	},
	{
		name: "ZTEST",
		description: "Vrátí P-hodnotu (jeden chvost) z-testu.",
		arguments: [
			{
				name: "matice",
				description: "je matice nebo oblast dat, kterou chcete použít k testování argumentu x."
			},
			{
				name: "x",
				description: "je testovaná hodnota."
			},
			{
				name: "sigma",
				description: "je známá směrodatná odchylka základního souboru. Jestliže tento argument nezadáte, bude použita směrodatná odchylka výběru."
			}
		]
	},
	{
		name: "ZVOLIT",
		description: "Zvolí hodnotu nebo akci, která má být provedena, ze seznamu hodnot na základě zadaného argumentu Index.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index",
				description: "určuje, který argument hodnoty je vybrán. Argument Index musí náležet do rozsahu 1 až 254 nebo představovat vzorec či odkaz na číslo v rozsahu 1 až 254."
			},
			{
				name: "hodnota1",
				description: "je 1 až 254 hodnot, odkazů na buňky, definovaných názvů, vzorců, funkcí nebo textových argumentů, z nichž vybírá funkce ZVOLIT."
			},
			{
				name: "hodnota2",
				description: "je 1 až 254 hodnot, odkazů na buňky, definovaných názvů, vzorců, funkcí nebo textových argumentů, z nichž vybírá funkce ZVOLIT."
			}
		]
	}
];