ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Returnerar absolutvärdet av ett tal. Ett tal utan tecken.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal som du vill beräkna absolutvärdet på."
			}
		]
	},
	{
		name: "ADRESS",
		description: "Skapar en cellreferens som text med ett angivet antal rader och kolumner.",
		arguments: [
			{
				name: "rad",
				description: "är radnumret som används till cellreferensen: radnummer = 1 för rad 1."
			},
			{
				name: "kolumn",
				description: "är kolumnnumret som används till cellreferensen. T ex, kolumnnummer = 4 för kolumn D."
			},
			{
				name: "abs",
				description: "anger referenstypen: absolut = 1; absolut rad/relativ kolumn = 2; relativ rad/absolut kolumn = 3; relativ =4."
			},
			{
				name: "a1",
				description: "är ett logiskt värde som anger referenstypen: A1 typ = 1 eller SANT; R1C1 typ = 0 eller FALSKT."
			},
			{
				name: "bladnamn",
				description: "är text som anger namnet på det kalkylblad som ska användas som externreferens"
			}
		]
	},
	{
		name: "AMORT",
		description: "Returnerar amorteringsinbetalningen för en investering baserat på periodiska, konstanta betalningar och en konstant ränta.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "period",
				description: "anger perioden och måste vara i intervallet 1 till periodantal."
			},
			{
				name: "periodantal",
				description: "är det totala antalet betalningsperioder i en investering."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet: den totala summan som en serie framtida betalningar är värda just nu."
			},
			{
				name: "slutvärde",
				description: "är det framtida värde eller saldo som du vill uppnå efter att sista betalningen har gjorts."
			},
			{
				name: "typ",
				description: "är ett logiskt värde: betalning i början av perioden = 1; betalning i slutet av perioden = 0 eller utelämnat."
			}
		]
	},
	{
		name: "ANTAL",
		description: "Räknar antalet celler i ett område som innehåller tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255 argument som kan innehålla eller referera till olika datatyper men endast tal räknas."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255 argument som kan innehålla eller referera till olika datatyper men endast tal räknas."
			}
		]
	},
	{
		name: "ANTAL.OM",
		description: "Räknar antalet celler som motsvarar givet villkor i ett område.",
		arguments: [
			{
				name: "område",
				description: "är det cellområde där du vill räkna antalet ej tomma celler."
			},
			{
				name: "villkor",
				description: "är villkoret i form av ett tal, uttryck eller text, som definierar vilka celler som kommer att räknas."
			}
		]
	},
	{
		name: "ANTAL.OMF",
		description: "Räknar antalet celler som anges av en given uppsättning villkor.",
		arguments: [
			{
				name: "villkorsområde",
				description: "är cellområdet som du vill värdera för det specifika villkoret"
			},
			{
				name: "villkor",
				description: "är villkoret i form av ett tal, uttryck eller text som definierar vilka celler som ska räknas"
			}
		]
	},
	{
		name: "ANTAL.TOMMA",
		description: "Räknar antal tomma celler i ett angivet område.",
		arguments: [
			{
				name: "område",
				description: "är det område som du vill räkna antalet tomma celler i."
			}
		]
	},
	{
		name: "ANTALBLAD",
		description: "Returnerar antalet blad i en referens.",
		arguments: [
			{
				name: "ref",
				description: "är en referens som du vill veta hur många blad den innehåller. Om det utelämnas returneras antalet blad i arbetsboken som innehåller funktionen."
			}
		]
	},
	{
		name: "ANTALV",
		description: "Räknar antalet celler i ett område som inte är tomma.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255 argument som representerar värden som du vill räkna. Värden kan vara vilken typ av information som helst."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255 argument som representerar värden som du vill räkna. Värden kan vara vilken typ av information som helst."
			}
		]
	},
	{
		name: "ÅR",
		description: "Returnerar året för ett datum, ett heltal mellan 1900-9999.",
		arguments: [
			{
				name: "serienummer",
				description: "är ett tal i datum-tid-koden som Spreadsheet använder."
			}
		]
	},
	{
		name: "ARABISKA",
		description: "Konverterar romerska siffror till arabiska.",
		arguments: [
			{
				name: "text",
				description: "är det tal med romerska siffror som du vill konvertera."
			}
		]
	},
	{
		name: "ARBETSDAGAR",
		description: "Returnerar serienumret till datumet före eller efter ett givet antal arbetsdagar.",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum"
			},
			{
				name: "dagar",
				description: "är antalet icke-helgdagar före eller efter startdatumet"
			},
			{
				name: "lediga",
				description: "är en valfri matris med en eller flera datumserienummer som ska uteslutas från arbetskalendern som till exempel nationella högtidsdagar"
			}
		]
	},
	{
		name: "ARBETSDAGAR.INT",
		description: "Returnerar datumets serienummer före eller efter ett angivet antal arbetsdagar med egna helgparametrar.",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum."
			},
			{
				name: "dagar",
				description: "är antalet icke-helgdagar och icke lediga dagar före eller efter startdatum."
			},
			{
				name: "helg",
				description: "är ett nummer eller en sträng som anger när helger infaller."
			},
			{
				name: "lediga",
				description: "är en frivillig matris med ett eller flera datumserienummer som inte ska räknas som arbetsdagar, till exempel högtider."
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Returnerar arcus cosinus för ett tal, i radianer i intervallet 0 till Pi. Arcus cosinus är vinkeln vars cosinus är Tal.",
		arguments: [
			{
				name: "tal",
				description: "är cosinusvärdet av vinkeln du vill ha och måste ligga mellan -1 och 1."
			}
		]
	},
	{
		name: "ARCCOSH",
		description: "Returnerar inverterad hyperbolisk cosinus för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal som är större än eller lika med 1."
			}
		]
	},
	{
		name: "ARCCOT",
		description: "Returnerar arcus cotangens för ett tal, i radianer i intervallet 0 till Pi.",
		arguments: [
			{
				name: "tal",
				description: "är cotangensvärdet av vinkeln du vill ha."
			}
		]
	},
	{
		name: "ARCCOTH",
		description: "Returnerar inverterad hyperbolisk cotangens för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är det hyperboliska cotangensvärdet av vinkeln du vill ha."
			}
		]
	},
	{
		name: "ARCSIN",
		description: "Returnerar arcus sinus för ett tal i radianer, i intervallet -Pi/2 till Pi/2.",
		arguments: [
			{
				name: "tal",
				description: "är sinusvärdet av vinkeln du vill ha och måste ligga mellan -1 och 1."
			}
		]
	},
	{
		name: "ARCSINH",
		description: "Returnerar hyperbolisk arcus sinus för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal som är större än eller lika med 1."
			}
		]
	},
	{
		name: "ARCTAN",
		description: "Returnerar arcus tangens för ett tal i radianer, i intervallet -Pi/2 till Pi/2.",
		arguments: [
			{
				name: "tal",
				description: "är tangensvärdet för vinkeln som du vill ha."
			}
		]
	},
	{
		name: "ARCTAN2",
		description: "Returnerar arcus tangens för de angivna x- och y-koordinaterna, i radianer mellan -Pi och Pi, vilket exkluderar -Pi.",
		arguments: [
			{
				name: "x",
				description: "är punktens x-koordinat."
			},
			{
				name: "y",
				description: "är punktens y-koordinat."
			}
		]
	},
	{
		name: "ARCTANH",
		description: "Returnerar inverterad hyperbolisk tangens för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal mellan -1 och 1, förutom -1 och 1."
			}
		]
	},
	{
		name: "ÅRDEL",
		description: "Returnerar ett tal som representerar antal hela dagar av ett år mellan startdatum och stoppdatum .",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum"
			},
			{
				name: "stoppdatum",
				description: "är ett datumserienummer som representerar slutdatum"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som används"
			}
		]
	},
	{
		name: "ÄREJTEXT",
		description: "Kontrollerar om ett värde inte är text (tomma celler är inte text) och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa: en cell; formel; eller ett namn som refererar till en cell, formel eller ett värde."
			}
		]
	},
	{
		name: "ÄRF",
		description: "Kontrollerar om ett värde är ett fel (#VÄRDEFEL!, #REFERENS!, #DIVISION/0!, #OGILTIGT!, #NAMN? eller #SKÄRNING!) utom #SAKNAS, och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa. Värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller ett värde."
			}
		]
	},
	{
		name: "ÄRFEL",
		description: "Kontrollerar om ett värde består av ett felvärde (#SAKNAS!, #VÄRDEFEL!, #REFERENS!, #DIVISION/0!, #OGILTIGT!, #NAMN? eller #SKÄRNING!) och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är det värde du vill testa. Värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller värde."
			}
		]
	},
	{
		name: "ÄRFORMEL",
		description: "Kontrollerar om värdet refererar till en cell som innehåller en formel och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "ref",
				description: "är en referens till cellen som du vill testa. Referensen kan vara en cellreferens, en formel eller ett namn som refererar till en cell."
			}
		]
	},
	{
		name: "ÄRJÄMN",
		description: "Returnerar SANT om talet är jämt.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som ska testas"
			}
		]
	},
	{
		name: "ÄRLOGISK",
		description: "Kontrollerar om ett värde är ett logiskt värde (SANT eller FALSKT) och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa. Värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller ett värde."
			}
		]
	},
	{
		name: "ÄRREF",
		description: "Kontrollerar om ett värde är en referens och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa. Ett värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller ett värde."
			}
		]
	},
	{
		name: "ÄRSAKNAD",
		description: "Kontrollerar om ett värde är #Saknas (otillgängligt värde) och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa. Värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller värde"
			}
		]
	},
	{
		name: "ÅRSAVSKR",
		description: "Returnerar den årliga avskrivningssumman för en tillgång under en angiven period.",
		arguments: [
			{
				name: "kostnad",
				description: "är initialkostnaden för tillgången."
			},
			{
				name: "restvärde",
				description: "är värdet efter avskrivningen."
			},
			{
				name: "livslängd",
				description: "är antalet perioder som tillgången minskar i värde (avskrivningstid)."
			},
			{
				name: "period",
				description: "är perioden. Måste anges i samma enhet som Livslängd."
			}
		]
	},
	{
		name: "ÅRSRÄNTA",
		description: "Returnerar räntesatsen för en fullinvesterad säkerhet.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "investering",
				description: "är det investerade beloppet "
			},
			{
				name: "inlösen",
				description: "är beloppet som utbetalas på förfallodagen"
			},
			{
				name: "bas",
				description: "är bastypen för antal dagar som ska användas"
			}
		]
	},
	{
		name: "ÄRTAL",
		description: "Kontrollerar om ett värde är ett tal och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa. Värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller ett värde."
			}
		]
	},
	{
		name: "ÄRTEXT",
		description: "Kontrollerar om ett värde är text och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är värdet du vill testa. Värde kan referera till en cell, formel eller ett namn som refererar till en cell, formel eller ett värde."
			}
		]
	},
	{
		name: "ÄRTOM",
		description: "Kontrollerar om värdet refererar till en tom cell och returnerar SANT eller FALSKT.",
		arguments: [
			{
				name: "värde",
				description: "är cellen eller ett namn som refererar till den cell som du vill testa."
			}
		]
	},
	{
		name: "ÄRUDDA",
		description: "Returnerar SANT om talet är udda.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som ska testas"
			}
		]
	},
	{
		name: "AVERAGEA",
		description: "Returnerar medelvärdet (aritmetiskt medelvärde) av argumenten, utvärderar text och FALSKT i argument som 0; SANT utvärderas som 1. Argument kan vara tal, namn, matriser eller referenser.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255 argument som du vill ha medelvärdet för."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255 argument som du vill ha medelvärdet för."
			}
		]
	},
	{
		name: "AVKORTA",
		description: "Avkortar ett tal till ett heltal genom att ta bort decimaler.",
		arguments: [
			{
				name: "tal",
				description: "är talet som du vill avkorta."
			},
			{
				name: "antal_siffror",
				description: "är ett tal som anger precisionen på avkortningen. Om det utelämnas används 0 (noll)."
			}
		]
	},
	{
		name: "AVKPÅINVEST",
		description: "Returnerar motsvarande ränta för en investerings tillväxt.",
		arguments: [
			{
				name: "periodantal",
				description: "är antalet perioder för investeringen."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet på investeringen."
			},
			{
				name: "slutvärde",
				description: "är det framtida värdet på investeringen."
			}
		]
	},
	{
		name: "AVRUNDA",
		description: "Avrundar ett tal till ett angivet antal decimaler.",
		arguments: [
			{
				name: "tal",
				description: "är talet som du vill avrunda."
			},
			{
				name: "decimaler",
				description: "är det antal decimaler som du vill avrunda till. Negativa avrundningar; noll till närmaste heltal."
			}
		]
	},
	{
		name: "AVRUNDA.NEDÅT",
		description: "Avrundar ett tal nedåt mot noll.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal som du vill avrunda nedåt."
			},
			{
				name: "decimaler",
				description: "är ett antal siffror som du vill runda av. Negativa avrundningar till vänster om decimalkommat; noll eller utelämnade, till närmaste heltal."
			}
		]
	},
	{
		name: "AVRUNDA.UPPÅT",
		description: "Avrundar ett tal uppåt från noll.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal som du vill avrunda uppåt."
			},
			{
				name: "decimaler",
				description: "är ett antal siffror som du vill runda av. Negativa avrundningar till vänster om decimalkommat; noll eller utelämnade, till närmaste heltal."
			}
		]
	},
	{
		name: "BAHTTEXT",
		description: "Konverterar ett tal till text (baht).",
		arguments: [
			{
				name: "tal",
				description: "är ett tal som du vill konvertera."
			}
		]
	},
	{
		name: "BAS",
		description: "Konverterar ett tal till textformat med en given talbas.",
		arguments: [
			{
				name: "tal",
				description: "är talet du vill konvertera."
			},
			{
				name: "talbas",
				description: "är talbasen du vill konvertera talet till."
			},
			{
				name: "minimilängd",
				description: "är minimilängden på strängen som returneras. Om det utelämnas läggs inga inledande nollor till."
			}
		]
	},
	{
		name: "BELOPP",
		description: "Returnerar beloppet som utbetalas på förfallodagen för ett betalt värdepapper.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "investering",
				description: "är det investerade beloppet "
			},
			{
				name: "ränta",
				description: "är värdepapperets diskonteringsränta"
			},
			{
				name: "bas",
				description: "är bastypen för antal dagar som ska användas"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Returnerar den modifierade Bessel-funktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna funktionen för"
			},
			{
				name: "n",
				description: "är ordningstalet för Bessel-funktionen"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Returnerar Bessel-funktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna funktionen för"
			},
			{
				name: "n",
				description: "är ordningstalet för Bessel-funktionen"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Returnerar den modifierade Bessel-funktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna funktionen för"
			},
			{
				name: "n",
				description: "är ordningstalet för Bessel-funktionen"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Returnerar Bessel-funktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna funktionen för"
			},
			{
				name: "n",
				description: "är ordningstalet för Bessel-funktionen"
			}
		]
	},
	{
		name: "BETA.FÖRD",
		description: "Returnerar funktionen för betasannolikhetsfördelning.",
		arguments: [
			{
				name: "x",
				description: "är värdet mellan A och B där du vill utvärdera funktionen."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och använd FALSKT för sannolikhetsfunktionen."
			},
			{
				name: "A",
				description: "är en frivillig nedre gräns för intervallet x. Om den utelämnas är A = 0."
			},
			{
				name: "B",
				description: "är en frivillig övre gräns för intervallet x. Om den utelämnas är B = 1."
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Returnerar inversen till den kumulativa betafördelningsfunktionen (BETA.FÖRD).",
		arguments: [
			{
				name: "Sannolikhet",
				description: "är sannolikheten som associeras med betafördelningen."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "A",
				description: "är en frivillig nedre gräns för intervallet x. Om den utelämnas är A = 0."
			},
			{
				name: "B",
				description: "är en frivillig övre gräns för intervallet x. Om den utelämnas är B = 1."
			}
		]
	},
	{
		name: "BETAFÖRD",
		description: "Returnera den kumulativa betafördelningsfunktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet mellan A och B där funktionen ska beräknas."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "A",
				description: "är ett frivilligt undre gränsvärde till intervallet för x. Om det utelämnas är A = 0."
			},
			{
				name: "B",
				description: "är ett frivilligt övre gränsvärde till intervallet för x. Om det utelämnas är B = 1."
			}
		]
	},
	{
		name: "BETAINV",
		description: "Returnera inversen till den kumulativa betafördelningsfunktionen (BETAFÖRD).",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med betafördelningen."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen och måste vara större än 0."
			},
			{
				name: "A",
				description: "är ett frivilligt undre gränsvärde till intervallet för x. Om det utelämnas är A = 0."
			},
			{
				name: "B",
				description: "är ett frivilligt övre gränsvärde till intervallet för x. Om det utelämnas är B = 1."
			}
		]
	},
	{
		name: "BETALNING",
		description: "Beräknar betalningen av ett lån baserat på regelbundna betalningar och en konstant ränta.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period på lånet. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "periodantal",
				description: "är det totala antalet betalningsperioder för lånet."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet: den totala summan som en serie framtida betalningar är värda just nu."
			},
			{
				name: "slutvärde",
				description: "är det framtida värde eller det saldo som du vill uppnå efter att sista betalningen har gjorts. Om det utelämnas används värdet 0 (noll)."
			},
			{
				name: "typ",
				description: "är ett logiskt värde: betalning i början av perioden = 1; betalning i slutet av perioden = 0 eller utelämnat."
			}
		]
	},
	{
		name: "BIN.TILL.DEC",
		description: "Konverterar ett binärt tal till ett decimalt.",
		arguments: [
			{
				name: "tal",
				description: "är det binära talet som du vill konvertera"
			}
		]
	},
	{
		name: "BIN.TILL.HEX",
		description: "Konverterar ett binärt tal till ett hexadecimalt.",
		arguments: [
			{
				name: "tal",
				description: "är det binära talet som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken du vill använda"
			}
		]
	},
	{
		name: "BIN.TILL.OKT",
		description: "Konverterar ett binärt tal till ett oktalt.",
		arguments: [
			{
				name: "tal",
				description: "är det binära talet som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken du vill använda"
			}
		]
	},
	{
		name: "BINOM.FÖRD",
		description: "Returnerar den individuella binomialfördelningen.",
		arguments: [
			{
				name: "antal_l",
				description: "är antalet lyckade försök."
			},
			{
				name: "försök",
				description: "är antalet oberoende försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas för varje försök."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen för massa."
			}
		]
	},
	{
		name: "BINOM.FÖRD.INTERVALL",
		description: "Returnerar sannolikheten för ett testresultat med en binomialfördelning.",
		arguments: [
			{
				name: "försök",
				description: "är antalet oberoende försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas för varje försök."
			},
			{
				name: "antal_l",
				description: "är antalet lyckade försök."
			},
			{
				name: "antal_l2",
				description: "den här funktionen returnerar sannolikheten för att antalet lyckade försök kommer att ligga mellan antal_l och antal_l2."
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Returnerar det minsta värdet för vilket den kumulativa binomialfördelningen är större än eller lika med ett villkorsvärde.",
		arguments: [
			{
				name: "försök",
				description: "är antalet Bernoulli-försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas för varje försök, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "alfa",
				description: "är villkorsvärdet, ett tal mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "BINOMFÖRD",
		description: "Returnera den individuella binomialfördelningen.",
		arguments: [
			{
				name: "antal_l",
				description: "är antalet lyckade försök."
			},
			{
				name: "försök",
				description: "är antalet oberoende försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas för varje försök."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen för massa."
			}
		]
	},
	{
		name: "BITELLER",
		description: "Returnerar 'Eller' bitvis för två tal.",
		arguments: [
			{
				name: "tal1",
				description: "är det binära talet i decimalform som du vill beräkna."
			},
			{
				name: "tal2",
				description: "är det binära talet i decimalform som du vill beräkna."
			}
		]
	},
	{
		name: "BITHSKIFT",
		description: "Returnerar ett tal som flyttats till höger med angivet flytta_antal bitar.",
		arguments: [
			{
				name: "tal",
				description: "är decimalformen för det binära talet som du vill beräkna."
			},
			{
				name: "flytta_antal",
				description: "är antalet bitar du vill flytta talet till höger med."
			}
		]
	},
	{
		name: "BITOCH",
		description: "Returnerar 'Och' bitvis för två tal.",
		arguments: [
			{
				name: "tal1",
				description: "är det binära talet i decimalform som du vill beräkna."
			},
			{
				name: "tal2",
				description: "är det binära talet i decimalform som du vill beräkna."
			}
		]
	},
	{
		name: "BITVSKIFT",
		description: "Returnerar ett tal som flyttats till vänster med angivet flytta_antal bitar.",
		arguments: [
			{
				name: "tal",
				description: "är decimalformen för det binära talet som du vill beräkna."
			},
			{
				name: "flytta_antal",
				description: "är antalet bitar du vill flytta talet till vänster med."
			}
		]
	},
	{
		name: "BITXELLER",
		description: "Returnerar 'Exklusivt eller' bitvis för två tal.",
		arguments: [
			{
				name: "tal1",
				description: "är det binära talet i decimalform som du vill beräkna."
			},
			{
				name: "tal2",
				description: "är det binära talet i decimalform som du vill beräkna."
			}
		]
	},
	{
		name: "BLAD",
		description: "Returnerar bladnummer för bladet som refereras.",
		arguments: [
			{
				name: "värde",
				description: "är namnet på bladet eller en referens som du vill ha bladnumret för. Om det utelämnas returneras numret på det blad som innehåller funktionen."
			}
		]
	},
	{
		name: "BRÅK",
		description: "Omvandlar ett tal uttryckt som ett decimaltal till ett tal uttryckt som ett bråk.",
		arguments: [
			{
				name: "decimaltal",
				description: "är ett decimaltal"
			},
			{
				name: "heltal",
				description: "är heltalet som kommer att användas som nämnare i bråket"
			}
		]
	},
	{
		name: "BYT.UT",
		description: "Ersätter gammal text med ny text i en textsträng.",
		arguments: [
			{
				name: "text",
				description: "är en text eller en referens till en cell med textinnehåll som du vill ersätta tecken i."
			},
			{
				name: "gammal_text",
				description: "är den befintliga texten som du vill ersätta. Om gammal_text inte stämmer överens med text kommer BYT.UT att ersätta texten."
			},
			{
				name: "ny_text",
				description: "är texten som du vill ersätta gammal_text med."
			},
			{
				name: "antal_förekomster",
				description: "anger vilken förekomst av gammal_text som du vill ersätta. Om utelämnad ersätts alla förekomster av gammal_text."
			}
		]
	},
	{
		name: "CELL",
		description: "Returnerar information om formatering, position eller innehåll i den första cellen, enligt bladets läsriktning, i referensen.",
		arguments: [
			{
				name: "infotyp",
				description: "är ett textvärde som anger vilken typ av cellinformation du vill ha."
			},
			{
				name: "referens",
				description: "är cellen som du vill ha information om."
			}
		]
	},
	{
		name: "CHI2.FÖRD",
		description: "Returnerar den vänstersidiga sannolikheten för chi2-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen, ett icke-negativt tal."
			},
			{
				name: "frihetsgrad",
				description: "är antalet frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde som funktionen ska returnera: kumulativa fördelningsfunktionen = SANT, sannolikhetsfunktionen = FALSKT."
			}
		]
	},
	{
		name: "CHI2.FÖRD.RT",
		description: "Returnerar den högersidiga sannolikheten för chi2-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen, ett icke-negativt tal."
			},
			{
				name: "frihetsgrad",
				description: "är antalet frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "CHI2.INV",
		description: "Returnerar inversen till den vänstersidiga sannolikheten för chi2-fördelningen.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är en sannolikhet som är associerad med chi2-fördelningen, ett värde mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrad",
				description: "är antalet frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "CHI2.INV.RT",
		description: "Returnerar inversen till den högersidiga sannolikheten för chi2-fördelningen.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är en sannolikhet som är associerad med chi2-fördelningen, ett värde mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrad",
				description: "är antalet frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "CHI2.TEST",
		description: "Returnerar oberoendetesten: värdet från chi2-fördelningen för statistiken och lämpligt antal frihetsgrader.",
		arguments: [
			{
				name: "observerat_omr",
				description: "är dataområdet som innehåller observationer att testa mot förväntade värden."
			},
			{
				name: "förväntat_omr",
				description: "är dataområdet som innehåller förhållandet mellan produkten av rad- och kolumnsummor och totalsumman."
			}
		]
	},
	{
		name: "CHI2FÖRD",
		description: "Returnera den ensidiga sannolikheten av chi2-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen, ett icke-negativt tal."
			},
			{
				name: "frihetsgrader",
				description: "är antalet frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "CHI2INV",
		description: "Returnera inversen till chi2-fördelningen.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med chi2-fördelningen, ett värde mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrader",
				description: "är antalet frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "CHI2TEST",
		description: "Returnera oberoendetesten: värdet från chi2-fördelningen för statistiken och lämpligt antal frihetsgrader.",
		arguments: [
			{
				name: "observerat_omr",
				description: "är dataområdet som innehåller observationer att testa mot förväntade värden."
			},
			{
				name: "förväntat_omr",
				description: "är dataområdet som innehåller förhållandet mellan produkten av rad- och kolumnsummor och totalsumman."
			}
		]
	},
	{
		name: "COS",
		description: "Returnerar cosinus för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha cosinus för"
			}
		]
	},
	{
		name: "COSH",
		description: "Returnerar hyperboliskt cosinus för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal."
			}
		]
	},
	{
		name: "COT",
		description: "Returnerar cotangens för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha cotangens för."
			}
		]
	},
	{
		name: "COTH",
		description: "Returnerar hyperbolisk cotangens för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha hyperbolisk cotangens för."
			}
		]
	},
	{
		name: "CSC",
		description: "Returnerar cosekant för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha cosekant för."
			}
		]
	},
	{
		name: "CSCH",
		description: "Returnerar hyperbolisk cosekant för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha hyperbolisk cosekant för."
			}
		]
	},
	{
		name: "DAG",
		description: "Returnerar en dag i månaden, ett tal mellan 1 och 31.",
		arguments: [
			{
				name: "serienummer",
				description: "är ett tal i datum-tid-koden som Spreadsheet använder."
			}
		]
	},
	{
		name: "DAGAR",
		description: "Returnerar antalet dagar mellan två datum.",
		arguments: [
			{
				name: "stoppdatum",
				description: "startdatum och stoppdatum är de två datum för vilka du vill veta antalet mellanliggande dagar."
			},
			{
				name: "startdatum",
				description: "startdatum och stoppdatum är de två datum för vilka du vill veta antalet mellanliggande dagar."
			}
		]
	},
	{
		name: "DAGAR360",
		description: "Returnerar antalet dagar mellan två datum baserat på ett år med 360 dagar (tolv 30-dagars månader).",
		arguments: [
			{
				name: "startdatum",
				description: "startdatum och stoppdatum är de två datum för vilka du vill veta antalet mellanliggande dagar."
			},
			{
				name: "stoppdatum",
				description: "startdatum och stoppdatum är de två datum för vilka du vill veta antalet mellanliggande dagar."
			},
			{
				name: "metod",
				description: "är ett logiskt värde som anger beräkningsmetoden: U.S. (NASD) = FALSKT eller utelämnad; Europeisk = SANT."
			}
		]
	},
	{
		name: "DANTAL",
		description: "Räknar antalet celler i fältet (kolumnen) med poster i databasen som innehåller tal som matchar de villkor du angivit.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör listan eller databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett mellan citationstecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DANTALV",
		description: "Räknar icketomma celler i fältet (kolumnen) med poster i databasen som matchar villkoren du anger.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
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
		description: "Returnerar numret för ett angivet datum i Spreadsheets datum-tidskod.",
		arguments: [
			{
				name: "år",
				description: "är ett tal mellan 1900 och 9999 i Spreadsheet för Windows eller 1904 och 9999 i Spreadsheet för Macintosh."
			},
			{
				name: "månad",
				description: "är ett tal från 1 till 12 som representerar månaden på året."
			},
			{
				name: "dag",
				description: "är ett tal från 1 till 31 som representerar dagen i månaden."
			}
		]
	},
	{
		name: "DATUMVÄRDE",
		description: "Konverterar ett datum i form av text till ett tal som står för datumen i Spreadsheets datum-tidskod.",
		arguments: [
			{
				name: "datumtext",
				description: "är text som representerar ett datum i Spreadsheet datumformat, mellan 1900-01-01 (Windows) eller 1904-01-01 (Macintosh) och 9999-12-31."
			}
		]
	},
	{
		name: "DB",
		description: "Returnerar avskrivningen för en tillgång under en angiven tid enligt metoden för fast degressiv avskrivning.",
		arguments: [
			{
				name: "kostnad",
				description: "är initialkostnaden för tillgången."
			},
			{
				name: "restvärde",
				description: "är värdet vid avskrivningstidens slut."
			},
			{
				name: "livslängd",
				description: "är antalet perioder under den tid då tillgången skrivs av och minskar i värde. (Avskrivningstid)"
			},
			{
				name: "period",
				description: "är den period för vilken du vill beräkna värdeminskningen. Period måsta anges i samma enhet som Livslängd."
			},
			{
				name: "månader",
				description: "är antal månader det första året. Om månader är utelämnat antas de vara 12."
			}
		]
	},
	{
		name: "DEC.TILL.BIN",
		description: "Konverterar ett decimalt tal till ett binärt.",
		arguments: [
			{
				name: "tal",
				description: "är det decimala heltal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken som används"
			}
		]
	},
	{
		name: "DEC.TILL.HEX",
		description: "Konverterar ett decimalt tal till ett hexadecimalt.",
		arguments: [
			{
				name: "tal",
				description: "är det decimala heltal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken som används"
			}
		]
	},
	{
		name: "DEC.TILL.OKT",
		description: "Konverterar ett decimalt tal till oktalt.",
		arguments: [
			{
				name: "tal",
				description: "är det decimala heltal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken du vill använda"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Konverterar ett tal i textformat i en given bas till ett decimaltal.",
		arguments: [
			{
				name: "tal",
				description: "är talet du vill konvertera."
			},
			{
				name: "talbas",
				description: "är talbasen för det tal du konverterar."
			}
		]
	},
	{
		name: "DECTAL",
		description: "Omvandlar ett pris uttryckt som ett bråk till ett decimaltal.",
		arguments: [
			{
				name: "bråktal",
				description: "är ett tal uttryckt som ett bråk"
			},
			{
				name: "heltal",
				description: "är heltalet som kommer att användas som nämnare i bråket"
			}
		]
	},
	{
		name: "DEGAVSKR",
		description: "Returnerar en tillgångs värdeminskning under en viss period med hjälp av dubbel degressiv avskrivning eller någon annan metod som du anger.",
		arguments: [
			{
				name: "kostnad",
				description: "är initialkostnaden för tillgången."
			},
			{
				name: "restvärde",
				description: "är värdet efter avskrivningen."
			},
			{
				name: "livslängd",
				description: "är antalet perioder under den tid då tillgången skrivs av och minskar i värde. (Avskrivningstid)"
			},
			{
				name: "period",
				description: "är perioden för vilken du vill beräkna värdeminskningen. Argumentet Period måste anges i samma enhet som Livslängd."
			},
			{
				name: "faktor",
				description: "är måttet för hur fort tillgången avskrivs. Om Faktor utelämnas, används värdet 2 (dubbel degressiv avskrivning)."
			}
		]
	},
	{
		name: "DELSUMMA",
		description: "Returnerar en delsumma i en lista eller databas.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funktionsnr",
				description: "är ett tal mellan 1 och 11 som anger summeringsfunktionen för delsumman."
			},
			{
				name: "ref1",
				description: "är 1 till 254 områden eller referenser för vilka du vill ha delsumman."
			}
		]
	},
	{
		name: "DELTA",
		description: "Testar om två värden är lika.",
		arguments: [
			{
				name: "tal1",
				description: "är det första talet"
			},
			{
				name: "tal2",
				description: "är det andra talet"
			}
		]
	},
	{
		name: "DHÄMTA",
		description: "Tar fram en enda post ur en databas enligt de villkor du anger.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör databasen. en databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DISK",
		description: "Returnerar diskonteringsräntan för ett värdepapper.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "pris",
				description: "är värdepapperets pris per 1 000 kr nominellt värde"
			},
			{
				name: "inlösen",
				description: "är värdepapperets inlösningsvärde per 1 000 kr nominellt värde"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som ska användas"
			}
		]
	},
	{
		name: "DMAX",
		description: "Returnerar det största talet i ett fält (en kolumn) med poster, i från databasen, som stämmer överens med de villkor du angav.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DMEDEL",
		description: "Beräknar medelvärdet för en kolumns värden i en lista eller databas enligt de villkor du angivit.",
		arguments: [
			{
				name: "databas",
				description: "är ett område med celler som utgör listan eller databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DMIN",
		description: "Returnerar det minsta talet i ett fält (kolumn) med poster, i från databasen, som stämmer överens med de villkor du angav.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DPRODUKT",
		description: "Multiplicerar värdena i fältet (kolumnen), med poster, i databasen som matchar de villkor du angav.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DSTDAV",
		description: "Uppskattar standardavvikelsen baserad på ett sampel från valda databasposter.",
		arguments: [
			{
				name: "databas",
				description: "är det område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DSTDAVP",
		description: "Beräknar standardavvikelsen baserad på hela populationen av valda databasposter.",
		arguments: [
			{
				name: "databas",
				description: "är ett område med celler som utgör listan eller databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DSUMMA",
		description: "Adderar talen i fältet (kolumnen) av poster i en databas som matchar villkoren du anger.",
		arguments: [
			{
				name: "databas",
				description: "är ett område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DUBBELFAKULTET",
		description: "Returnerar dubbelfakulteten för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är det värde som den dubbla fakulteten ska returneras för"
			}
		]
	},
	{
		name: "DVARIANS",
		description: "Uppskattar variansen baserad på ett sampel från valda databasposter.",
		arguments: [
			{
				name: "databas",
				description: "är ett område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "DVARIANSP",
		description: "Beräknar variansen på hela populationen av valda databasposter.",
		arguments: [
			{
				name: "databas",
				description: "är ett område med celler som utgör databasen. En databas är en lista med relaterade data."
			},
			{
				name: "fält",
				description: "är antingen kolumnens etikett inom citattecken eller ett tal som representerar kolumnens position i listan."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som innehåller de villkor du anger. Området inkluderar en kolumnetikett och en cell, under etiketten, för ett villkor."
			}
		]
	},
	{
		name: "EDATUM",
		description: "Returnerar serienumret till det datum som ligger angivet antal månader innan eller efter startdatumet.",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum"
			},
			{
				name: "månader",
				description: "är antalet månader före eller efter startdatumet"
			}
		]
	},
	{
		name: "EFFRÄNTA",
		description: "Returnerar den effektiva årsräntan.",
		arguments: [
			{
				name: "nominalränta",
				description: "är den nominella räntesatsen"
			},
			{
				name: "antal_perioder",
				description: "är antalet ränteperioder per år"
			}
		]
	},
	{
		name: "ELLER",
		description: "Kontrollerar om något av argumenten har värdet SANT och returnerar SANT eller FALSKT. Returnerar FALSKT bara om alla argument har värdet FALSKT.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "är 1 till 255 villkor du vill testa som kan ha värdet SANT eller FALSKT."
			},
			{
				name: "logisk2",
				description: "är 1 till 255 villkor du vill testa som kan ha värdet SANT eller FALSKT."
			}
		]
	},
	{
		name: "ERSÄTT",
		description: "Ersätter del av textsträng med en annan.",
		arguments: [
			{
				name: "gammal_text",
				description: "är texten som du vill ersätta vissa tecken i."
			},
			{
				name: "startpos",
				description: "är positionen för det tecken i Gammal_text som du vill ersätta med Ny_text."
			},
			{
				name: "antal_tecken",
				description: "är antalet tecken i Gammal_text som du vill ersätta."
			},
			{
				name: "ny_text",
				description: "är texten som ersätter tecken i Gammal_text."
			}
		]
	},
	{
		name: "EXAKT",
		description: "Kontrollerar om två textsträngar är exakt likadana och returnerar värdet SANT eller FALSKT. EXAKT är skifteslägeskänsligt.",
		arguments: [
			{
				name: "text1",
				description: "är den första textsträngen."
			},
			{
				name: "text2",
				description: "är den andra textsträngen."
			}
		]
	},
	{
		name: "EXP",
		description: "Returnerar e upphöjt till ett angivet tal.",
		arguments: [
			{
				name: "tal",
				description: "är exponenten som används tillsammans med basen e. Konstanten e är lika med 2.71828182845904, basen i en naturlig logaritm."
			}
		]
	},
	{
		name: "EXPON.FÖRD",
		description: "Returnerar exponentialfördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet av funktionen, ett icke-negativt tal."
			},
			{
				name: "lambda",
				description: "är parametervärdet, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde som funktionen ska returnera: den kumulativa fördelningsfunktionen = SANT; sannolikhetsfunktionen = FALSKT."
			}
		]
	},
	{
		name: "EXPONFÖRD",
		description: "Returnera exponentialfördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet av funktionen, ett icke-negativt tal."
			},
			{
				name: "lambda",
				description: "är parametervärdet, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde som funktionen ska returnera: den kumulativa fördelningsfunktionen = SANT; sannolikhetsfunktionen = FALSKT."
			}
		]
	},
	{
		name: "EXPREGR",
		description: "Returnerar statistik som beskriver en exponentiell kurva som matchar kända datapunkter.",
		arguments: [
			{
				name: "kända_y",
				description: "är den mängd y-värden som är kända i förhållandet y = b*m^x."
			},
			{
				name: "kända_x",
				description: "är en frivillig mängd x-värden som är kända i förhållandet y = b*m^x."
			},
			{
				name: "konst",
				description: "är ett logiskt värde: konstanten b beräknas normalt om konst = SANT eller utesluten; konstanten b ska vara 1 om konst = FALSKT."
			},
			{
				name: "statistik",
				description: "är ett logiskt värde: returnera extra regressionsstatistik = SANT; returnera m-koefficienten och konstanten b = FALSKT eller utelämnad."
			}
		]
	},
	{
		name: "EXPTREND",
		description: "Returnerar tal i en exponentiell tillväxttrend som matchar kända datapunkter.",
		arguments: [
			{
				name: "kända_y",
				description: "är den mängd y-värden som är kända i förhållandet y = b*m^x, en matris eller ett område med positiva tal."
			},
			{
				name: "kända_x",
				description: "är en alternativ mängd med x-värden som är kända i förhållandet y = b*m^x, en matris eller ett område med samma storlek som kända_y."
			},
			{
				name: "nya_x",
				description: "är de nya x-värden som du vill att EXPTREND ska returnera motsvarande y-värden för."
			},
			{
				name: "konst",
				description: "är ett logiskt värde. Konstanten b beräknas normalt om konst = SANT, och sätts till 1 om konst = FALSKT eller utelämnas."
			}
		]
	},
	{
		name: "EXTEXT",
		description: "Returnerar tecknen från mitten av en textsträng med en startposition och längd som du anger.",
		arguments: [
			{
				name: "text",
				description: "är texten som innehåller de tecken du vill ta fram."
			},
			{
				name: "startpos",
				description: "är positionen för det första tecknet du vill ta fram. Första tecknet i Texten är 1."
			},
			{
				name: "antal_tecken",
				description: "anger hur många tecken som ska returneras från texten."
			}
		]
	},
	{
		name: "F.FÖRD",
		description: "Returnerar den vänstersidiga F-sannolikhetsfördelningen (grad av skillnad) för två datamängder.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett icke-negativt tal."
			},
			{
				name: "frihetsgrad1",
				description: "är täljarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "frihetsgrad2",
				description: "är nämnarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde som funktionen ska returnera: den kumulativa fördelningsfunktionen = SANT, sannolikhetsfunktionen = FALSKT."
			}
		]
	},
	{
		name: "F.FÖRD.RT",
		description: "Returnerar den högersidiga F-sannolikhetsfördelningen (grad av skillnad) för två datamängder.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett icke-negativt tal."
			},
			{
				name: "frihetsgrad1",
				description: "är täljarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "frihetsgrad2",
				description: "är nämnarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "F.INV",
		description: "Returnerar inversen till den vänstersidiga F-sannolikhetsfördelningen: om p = F.FÖRD(x,...), då F.INV(p,...) = x.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är en sannolikhet som är associerad med F-sannolikhetsfördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrad1",
				description: "är täljarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "frihetsgrad2",
				description: "är nämnarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Returnerar inversen till den högersidiga F-sannolikhetsfördelningen: om p = F.FÖRD.RT(x,...), då F.INV.RT(p,...) = x.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är en sannolikhet som är associerad med F-sannolikhetsfördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrad1",
				description: "är täljarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "frihetsgrad2",
				description: "är nämnarens frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "F.TEST",
		description: "Returnerar resultatet av en F-test, den tvåsidiga sannolikheten att varianserna i Matris1 och Matris2 inte är markant olika.",
		arguments: [
			{
				name: "matris1",
				description: "är den första matrisen eller dataområdet och kan vara tal, namn, matriser eller referenser som innehåller tal (tomma celler ignoreras)."
			},
			{
				name: "matris2",
				description: "är den andra matrisen eller dataområdet och kan vara tal, namn, matriser eller referenser som innehåller tal (tomma celler ignoreras)."
			}
		]
	},
	{
		name: "FAKULTET",
		description: "Returnerar ett tals fakultet. D.v.s. produkten av 1*2*3*...*tal.",
		arguments: [
			{
				name: "tal",
				description: "är det icke-negativa tal som du vill ha fakulteten av."
			}
		]
	},
	{
		name: "FALSKT",
		description: "Returnerar det logiska värdet FALSKT.",
		arguments: [
		]
	},
	{
		name: "FASTTAL",
		description: "Rundar av ett tal till det angivna antalet decimaler och returnerar resultatet som text med eller utan kommatecken.",
		arguments: [
			{
				name: "tal",
				description: "är talet som du vill runda av och konvertera till text."
			},
			{
				name: "decimaler",
				description: "är antalet siffror till höger om decimalkommat. Om det utelämnas, Decimaler = 2."
			},
			{
				name: "ej_komma",
				description: "är ett logiskt värde: visa inte kommatecken i returnerad text = SANT; visa kommatecken i returnerad text = FALSKT eller utelämnad."
			}
		]
	},
	{
		name: "FEL.TYP",
		description: "Returnerar ett tal som motsvarar ett felvärde.",
		arguments: [
			{
				name: "felvärde",
				description: "är felvärdet som du vill ha identifieringsnummer för, och kan vara ett faktiskt felvärde eller en referens till en cell som innehåller ett felvärde."
			}
		]
	},
	{
		name: "FELF",
		description: "Returnerar felfunktionen.",
		arguments: [
			{
				name: "undre_gräns",
				description: "är den undre integrationsgränsen för ERF"
			},
			{
				name: "övre_gräns",
				description: "är den övre integrationsgränsen för ERF"
			}
		]
	},
	{
		name: "FELF.EXAKT",
		description: "Returnerar felfunktionen.",
		arguments: [
			{
				name: "X",
				description: "är den undre integrationsgränsen för FELF.EXAKT"
			}
		]
	},
	{
		name: "FELFK",
		description: "Returnerar den komplimenterande felfunktionen.",
		arguments: [
			{
				name: "x",
				description: "är den undre integrationsgränsen för ERFC"
			}
		]
	},
	{
		name: "FELFK.EXAKT",
		description: "Returnerar den komplimenterande felfunktionen.",
		arguments: [
			{
				name: "X",
				description: "är den undre integrationsgränsen för FELFK.EXAKT"
			}
		]
	},
	{
		name: "FFÖRD",
		description: "Returnera den högersidiga F-sannolikhetsfördelningen (grad av skillnad) för två datamängder.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett icke-negativt tal."
			},
			{
				name: "frihetsgrader1",
				description: "är täljarens antal frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "frihetsgrader2",
				description: "är nämnarens antal frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
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
		description: "Returnera inversen till den högersidiga F-sannolikhetsfördelningen: om p = FFÖRD(x,...), då FINV(p,...) = x.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med den kumulativa F-fördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrader1",
				description: "är täljarens antal frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			},
			{
				name: "frihetsgrader2",
				description: "är nämnarens antal frihetsgrader, ett tal mellan 1 och 10^10, exklusive 10^10."
			}
		]
	},
	{
		name: "FISHER",
		description: "Returnerar Fisher-transformationen.",
		arguments: [
			{
				name: "x",
				description: "är det värde som du vill ha transformationen för, ett tal mellan -1 och 1, utom -1 och 1."
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Returnerar inversen till Fisher-transformationen: om y = FISHER(x), då är FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "är värdet som du vill utföra den inversa transformationen på."
			}
		]
	},
	{
		name: "FORMELTEXT",
		description: "Returnerar en formel som en sträng.",
		arguments: [
			{
				name: "ref",
				description: "är en referens till en formel."
			}
		]
	},
	{
		name: "FÖRRÄNTNING",
		description: "Returnerar ett framtida värde av ett begynnelsekapital beräknat på flera olika räntenivåer.",
		arguments: [
			{
				name: "kapital",
				description: "är nuvärdet"
			},
			{
				name: "räntor",
				description: "är en matris över de räntesatser som ska gälla"
			}
		]
	},
	{
		name: "FÖRSKJUTNING",
		description: "Returnerar en referens till ett område som är ett givet antal rader och kolumner från en given referens.",
		arguments: [
			{
				name: "ref",
				description: "är referensen som anger utgångspunkt för förskjutningen, en referens till en cell eller ett område av intilliggande celler."
			},
			{
				name: "rader",
				description: "är antal rader ner eller upp som du vill att övre vänstra cellen i resultatet ska referera till."
			},
			{
				name: "kolumner",
				description: "är antal kolumner till vänster eller höger som du vill att övre vänstra cellen i resultatet ska referera till."
			},
			{
				name: "höjd",
				description: "är höjden i antal rader som du vill att resultatet ska vara, om det utelämnas så används samma höjd som Referens."
			},
			{
				name: "bredd",
				description: "är bredden i antal kolumner som du vill att resultatet ska vara, om det utelämnas så används samma bredd som Referens."
			}
		]
	},
	{
		name: "FREKVENS",
		description: "Beräknar hur ofta värden uppstår inom ett område med värden och returnerar en vertikal matris med tal som har ett element mer än en fackmatris.",
		arguments: [
			{
				name: "datamatris",
				description: "är en matris eller en referens till ett område med värden för vilka du vill ha beräkningsfrekvensen (mellanslag och text ignoreras)."
			},
			{
				name: "fackmatris",
				description: "är en matris eller en referens till intervaller i vilka du vill gruppera värdena i en datamatris."
			}
		]
	},
	{
		name: "FTEST",
		description: "Returnera resultatet av ett F-test, den tvåsidiga sannolikheten att varianserna i Matris1 och Matris2 inte skiljer sig åt markant.",
		arguments: [
			{
				name: "matris1",
				description: "är den första matrisen eller det första dataområdet och kan vara tal, namn, matriser eller referenser som innehåller tal (tomma celler ignoreras)."
			},
			{
				name: "matris2",
				description: "är den andra matrisen eller det andra dataområdet och kan vara tal, namn, matriser eller referenser som innehåller tal (tomma celler ignoreras)."
			}
		]
	},
	{
		name: "GAMMA",
		description: "Returnerar värdet för gammafunktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna gamma för."
			}
		]
	},
	{
		name: "GAMMA.FÖRD",
		description: "Returnerar gammafördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen, ett icke-negativt tal."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen, ett positivt tal. Om beta = 1 returnerar GAMMA.FÖRD standardgammafördelningen."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: returnera den kumulativa fördelningsfunktionen = SANT; returnera sannolikhetsfunktionen för massa = FALSKT eller utelämnad"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Returnerar inversen till den kumulativa gammafördelningen: om p = GAMMA.FÖRD(x,...), då GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med gammafördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen, ett positivt tal. Om beta = 1 returnerar GAMMA.INV inversen till standardgammafördelningen."
			}
		]
	},
	{
		name: "GAMMAFÖRD",
		description: "Returnera gammafördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen, ett icke-negativt tal."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen, ett positivt tal. Om beta = 1 returnerar GAMMAFÖRD standardgammafördelningen."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: returnera den kumulativa fördelningsfunktionen = SANT; returnera sannolikhetsfunktionen för massa = FALSKT eller utelämnad."
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Returnera inversen till den kumulativa gammafördelningen: om p = GAMMAFÖRD(x,...), då GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med gammafördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen. ett positivt tal."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen. Om beta = 1 returnerar GAMMAINV standardgammafördelningen."
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Returnerar den naturliga logaritmen för gammafunktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna GAMMALN för, ett positivt tal."
			}
		]
	},
	{
		name: "GAMMALN.EXAKT",
		description: "Returnerar den naturliga logaritmen för gammafunktionen.",
		arguments: [
			{
				name: "x",
				description: "är värdet som du vill beräkna GAMMALN.EXAKT för, ett positivt tal."
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
		name: "GEMENER",
		description: "Konverterar samtliga bokstäver i en text till gemener.",
		arguments: [
			{
				name: "text",
				description: "är textsträngen som du vill konvertera till gemener. Tecken i texten som inte är bokstäver påverkas inte."
			}
		]
	},
	{
		name: "GEOMEDEL",
		description: "Returnerar det geometriska medelvärdet av en matris eller av ett område positiva numeriska data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är från 1 till 255 tal, namn, matriser eller referenser som innehåller tal, vars medelvärde du vill beräkna."
			},
			{
				name: "tal2",
				description: "är från 1 till 255 tal, namn, matriser eller referenser som innehåller tal, vars medelvärde du vill beräkna."
			}
		]
	},
	{
		name: "GRADER",
		description: "Konverterar radianer till grader.",
		arguments: [
			{
				name: "vinkel",
				description: "är vinkeln i radianer som du vill konvertera."
			}
		]
	},
	{
		name: "HÄMTA.PIVOTDATA",
		description: "Extraherar data som sparats i en pivottabell.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "datafält",
				description: "är namnet på det datafält som du vill extrahera data från"
			},
			{
				name: "pivottabell",
				description: "är en referens till en cell eller ett område av celler i pivottabellen som innehåller de data du vill hämta."
			},
			{
				name: "fält",
				description: "fält som hänvisas till."
			},
			{
				name: "objekt",
				description: "fältobjekt som hänvisas till."
			}
		]
	},
	{
		name: "HARMMEDEL",
		description: "Returnerar det harmoniska medelvärdet av ett dataområde med positiva tal: motsvarigheten av det aritmetiska medelvärdet av motsvarigheter.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 tal eller namn, matriser eller referenser som innehåller tal, vars harmoniska medelvärde du vill beräkna."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 tal eller namn, matriser eller referenser som innehåller tal, vars harmoniska medelvärde du vill beräkna."
			}
		]
	},
	{
		name: "HELTAL",
		description: "Rundar av ett tal till närmaste heltal.",
		arguments: [
			{
				name: "tal",
				description: "är det reella talet du vill runda av, neråt, till närmaste heltal."
			}
		]
	},
	{
		name: "HEX.TILL.BIN",
		description: "Konverterar ett hexadecimalt tal till ett binärt.",
		arguments: [
			{
				name: "tal",
				description: "är det hexadecimala tal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är antalet tecken som du vill använda"
			}
		]
	},
	{
		name: "HEX.TILL.DEC",
		description: "Konverterar ett hexadecimalt tal till ett decimalt.",
		arguments: [
			{
				name: "tal",
				description: "är det hexadecimala talet som du vill konvertera"
			}
		]
	},
	{
		name: "HEX.TILL.OKT",
		description: "Konverterar ett hexadecimalt tal till ett oktalt.",
		arguments: [
			{
				name: "tal",
				description: "är det hexadecimala tal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är antalet tecken som du vill använda"
			}
		]
	},
	{
		name: "HITTA",
		description: "Returnerar startpositionen för en textsträng inom en annan textsträng. SÖK är skiftlägeskänsligt.",
		arguments: [
			{
				name: "sök",
				description: "är texten du söker efter. Om du vill matcha de första bokstäverna i inom_text använder du dubbla citattecken (ingen text); jokertecken tillåts inte."
			},
			{
				name: "inom_text",
				description: "är den text som innehåller den text som du vill hitta."
			},
			{
				name: "startpos",
				description: "anger vid vilken bokstav sökningen ska börja. Första bokstaven i inom_text är bokstav nummer 1. Om den utelämnas så är startpos = 1."
			}
		]
	},
	{
		name: "HÖGER",
		description: "Returnerar det angivna antalet tecken från slutet av en textsträng.",
		arguments: [
			{
				name: "text",
				description: "är textsträngen som innehåller de tecken du vill ta fram."
			},
			{
				name: "antal_tecken",
				description: "anger hur många tecken du vill ta fram, 1 om utelämnat."
			}
		]
	},
	{
		name: "HYPERLÄNK",
		description: "Skapar en genväg eller ett hopp som öppnar ett dokument som är lagrat på din hårddisk, en server på nätverket eller på Internet.",
		arguments: [
			{
				name: "Länk_placering",
				description: "är texten som ger sökvägen och filnamnet på dokumentet som ska öppnas, en plats på hårddisken, en UNC-adress eller en URL-sökväg."
			},
			{
				name: "vänligt_namn",
				description: "är texten eller talet som visas i cellen. Om det utelämnas visar cellen namnet på länken."
			}
		]
	},
	{
		name: "HYPGEOM.FÖRD",
		description: "Returnerar den hypergeometriska fördelningen.",
		arguments: [
			{
				name: "sampel",
				description: "är antal lyckade försök i samplet."
			},
			{
				name: "antal_sampel",
				description: "är samplets storlek."
			},
			{
				name: "population",
				description: "är antal lyckade försök i populationen."
			},
			{
				name: "antal_population",
				description: "är populationens storlek."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen, använd FALSKT för sannolikhetsfunktionen."
			}
		]
	},
	{
		name: "HYPGEOMFÖRD",
		description: "Returnera den hypergeometriska fördelningen.",
		arguments: [
			{
				name: "sampel",
				description: "är antal lyckade försök i samplet."
			},
			{
				name: "antal_sampel",
				description: "är samplets storlek."
			},
			{
				name: "population",
				description: "är antal lyckade försök i populationen"
			},
			{
				name: "antal_population",
				description: "är populationens storlek."
			}
		]
	},
	{
		name: "ICKE",
		description: "Ändrar FALSKT till SANT eller SANT till FALSKT.",
		arguments: [
			{
				name: "logisk",
				description: "är ett värde eller uttryck som kan beräknas till SANT eller FALSKT."
			}
		]
	},
	{
		name: "IDAG",
		description: "Returnerar dagens datum formaterat som ett datum.",
		arguments: [
		]
	},
	{
		name: "IMABS",
		description: "Returnerar absolutvärdet av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta absolutvärdet för"
			}
		]
	},
	{
		name: "IMAGINÄR",
		description: "Returnerar den imaginära koefficienten av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta den imaginära koefficienten för"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Returnerar argumentet theta, en vinkel uttryckt i radianer.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta argumentet för"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Returnerar cosinus av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta cosinus för"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Returnerar hyperbolisk cosinus för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta hyperbolisk cosinus för."
			}
		]
	},
	{
		name: "IMCOT",
		description: "Returnerar cotangens för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta cotangensen för."
			}
		]
	},
	{
		name: "IMCSC",
		description: "Returnerar cosekant för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta cosekanten för."
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Returnerar hyperbolisk cosekant för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta hyperbolisk cosekant för."
			}
		]
	},
	{
		name: "IMDIFF",
		description: "Returnerar differensen mellan två komplexa tal.",
		arguments: [
			{
				name: "ital1",
				description: "är det komplexa tal som du vill subtrahera ital2 ifrån"
			},
			{
				name: "ital2",
				description: "är det komplexa tal som du subtraherar från ital1"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Returnerar kvoten av två komplexa tal.",
		arguments: [
			{
				name: "ital1",
				description: "är det komplexa talet i täljaren"
			},
			{
				name: "ital2",
				description: "är det komplexa talet i täljaren"
			}
		]
	},
	{
		name: "IMEUPPHÖJT",
		description: "Returnerar e upphöjt till ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill upphöja e till"
			}
		]
	},
	{
		name: "IMKONJUGAT",
		description: "Returnerar det komplexa konjugatet till ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta konjugatet för"
			}
		]
	},
	{
		name: "IMLN",
		description: "Returnerar den naturliga logaritmen av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta den naturliga logaritmen för"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Returnerar 10-logaritmen av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta 10-logaritmen för"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Returnerar 2-logaritmen av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta 2-logaritmen för"
			}
		]
	},
	{
		name: "IMPRODUKT",
		description: "Returnerar produkten av 1 till 255 komplexa tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ital1",
				description: "Ital1, Ital2,... är från 1 till 255 komplexa tal som du vill multiplicera."
			},
			{
				name: "ital2",
				description: "Ital1, Ital2,... är från 1 till 255 komplexa tal som du vill multiplicera."
			}
		]
	},
	{
		name: "IMREAL",
		description: "Returnerar realkoefficienten av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta realkoefficienten för"
			}
		]
	},
	{
		name: "IMROT",
		description: "Returnerar kvadratroten av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta kvadratroten för"
			}
		]
	},
	{
		name: "IMSEK",
		description: "Returnerar sekant för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta sekanten för."
			}
		]
	},
	{
		name: "IMSEKH",
		description: "Returnerar hyperbolisk sekant för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta hyperbolisk sekant för."
			}
		]
	},
	{
		name: "IMSIN",
		description: "Returnerar sinus av ett komplext tal.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill veta sinus för"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Returnerar hyperbolisk sinus för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta hyperbolisk sinus för."
			}
		]
	},
	{
		name: "IMSUM",
		description: "Returnerar summan av komplexa tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ital1",
				description: "är från 1 till 255 komplexa tal som du vill addera"
			},
			{
				name: "ital2",
				description: "är från 1 till 255 komplexa tal som du vill addera"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Returnerar tangens för ett komplext tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett komplext tal som du vill veta tangensen för."
			}
		]
	},
	{
		name: "IMUPPHÖJT",
		description: "Returnerar ett komplext tal upphöjt till en heltalsexponent.",
		arguments: [
			{
				name: "ital",
				description: "är ett komplext tal som du vill upphöja till en potens"
			},
			{
				name: "tal",
				description: "är potensen som du vill upphöja det komplexa talet till"
			}
		]
	},
	{
		name: "INDEX",
		description: "Returnerar ett värde eller referens för cellen vid skärningen av en viss rad och kolumn i ett givet område.",
		arguments: [
			{
				name: "matris",
				description: "är ett område med celler eller en matriskonstant."
			},
			{
				name: "rad",
				description: "markerar raderna i Matris eller Referens som ska returnera ett värde. Om det utelämnas krävs Kolumn_num."
			},
			{
				name: "kolumn",
				description: "markerar kolumnerna i Matris eller Referens som ska returnera ett värde. Om det utelämnas krävs Rad_num."
			}
		]
	},
	{
		name: "INDIREKT",
		description: "Returnerar referensen angiven av en textsträng.",
		arguments: [
			{
				name: "reftext",
				description: "är en referens till en cell som innehåller en referens av typen A1 eller R1C1, ett namn definierat som en referens eller en referens till en cell som en textsträng."
			},
			{
				name: "a1",
				description: "är ett logiskt värde som anger referenstypen i Ref_text: R1C1-typ = FALSKT; A1-typ = SANT eller utelämnad."
			}
		]
	},
	{
		name: "INFO",
		description: "Returnerar information om operativsystemet.",
		arguments: [
			{
				name: "typ",
				description: "är text som anger vilken typ av information du vill returnera."
			}
		]
	},
	{
		name: "INITIAL",
		description: "Konverterar text: ändrar första bokstaven i varje ord till versal och konverterar alla andra bokstäver till gemener.",
		arguments: [
			{
				name: "text",
				description: "är en text innesluten i citattecken, en formel som returnerar text eller en referens till en cell som innehåller text som delvis ska ha stora bokstäver."
			}
		]
	},
	{
		name: "IR",
		description: "Returnerar avkastningsgraden för en serie penningflöden.",
		arguments: [
			{
				name: "värden",
				description: "är en matris eller en referens till celler som innehåller tal som du vill beräkna avkastningsgraden för."
			},
			{
				name: "gissning",
				description: "är ett tal som du tror ligger nära resultatet för IR; 0,1 (10 procent) om den utelämnas."
			}
		]
	},
	{
		name: "ISO.RUNDA.UPP",
		description: "Avrundar ett tal uppåt, till närmsta heltal eller till närmsta signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda"
			},
			{
				name: "signifikans",
				description: "är den frivilliga multipel som du vill avrunda till"
			}
		]
	},
	{
		name: "ISOVECKONR",
		description: "Returnerar veckonumret (ISO) för ett visst datum.",
		arguments: [
			{
				name: "datum",
				description: "är datum/tid-koden som används av Spreadsheet för datum- och tidsberäkningar."
			}
		]
	},
	{
		name: "JÄMN",
		description: "Rundar av ett positivt tal uppåt, och ett negativt tal nedåt, till närmaste jämna heltal.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda."
			}
		]
	},
	{
		name: "KLOCKSLAG",
		description: "Konverterar timmar, minuter och sekunder angivna som tal till Spreadsheet-serienummer, formaterade med tidsformat.",
		arguments: [
			{
				name: "timme",
				description: "är ett tal mellan 0 och 23 som representerar timmar."
			},
			{
				name: "minut",
				description: "är ett tal mellan 0 och 59 som representerar minuter."
			},
			{
				name: "sekund",
				description: "är ett tal mellan 0 och 59 som representerar sekunder."
			}
		]
	},
	{
		name: "KOD",
		description: "Returnerar en numerisk kod för det första tecknet i en textsträng, i teckenuppsättningen som används av din dator.",
		arguments: [
			{
				name: "text",
				description: "är texten vars första tecken returneras som kod."
			}
		]
	},
	{
		name: "KODAWEBBADRESS",
		description: "Returnerar en URL-kodad sträng.",
		arguments: [
			{
				name: "text",
				description: "är en sträng som ska URL-kodas"
			}
		]
	},
	{
		name: "KOLUMN",
		description: "Returnerar kolumntalet för en referens.",
		arguments: [
			{
				name: "ref",
				description: "är cellen eller området med celler som du vill infoga kolumntal i. Om det utelämnas kommer cellen som innehåller funktionen KOLUMN att användas."
			}
		]
	},
	{
		name: "KOLUMNER",
		description: "Returnerar antalet kolumner i en matris eller en referens.",
		arguments: [
			{
				name: "matris",
				description: "är en matris, en matrisformel eller en referens till ett cellområde som du vill ha reda på antal kolumner i."
			}
		]
	},
	{
		name: "KOMBIN",
		description: "Returnerar antalet kombinationer för ett givet antal objekt.",
		arguments: [
			{
				name: "antal",
				description: "är antalet objekt."
			},
			{
				name: "valt_antal",
				description: "är antalet objekt i varje kombination."
			}
		]
	},
	{
		name: "KOMBINA",
		description: "Returnerar antalet kombinationer med repetitioner för ett givet antal objekt.",
		arguments: [
			{
				name: "tal",
				description: "är antalet objekt."
			},
			{
				name: "valt_tal",
				description: "är antalet objekt i varje kombination."
			}
		]
	},
	{
		name: "KOMPLEX",
		description: "Konverterar en real- och en imaginärkoefficient till ett komplext tal.",
		arguments: [
			{
				name: "realdel",
				description: "är den reella koefficienten till det komplexa talet"
			},
			{
				name: "imaginärdel",
				description: "är den imaginära koefficienten till det komplexa talet"
			},
			{
				name: "suffix",
				description: "är suffixet till den imaginära komponenten i det komplexa talet"
			}
		]
	},
	{
		name: "KONFIDENS",
		description: "Returnera konfidensintervallet för en populations medelvärde med en normalfördelning.",
		arguments: [
			{
				name: "alfa",
				description: "är signifikansnivån som används för att beräkna konfidensnivån, ett tal som är större än 0 och mindre än 1."
			},
			{
				name: "standardavvikelse",
				description: "är populationens standardavvikelse för dataområdet och den förväntas vara känd. Standardavvikelse måste vara större än 0."
			},
			{
				name: "storlek",
				description: "är sampelstorleken."
			}
		]
	},
	{
		name: "KONFIDENS.NORM",
		description: "Returnerar konfidensintervallet för en populations medelvärde, med normalfördelning.",
		arguments: [
			{
				name: "alfa",
				description: "är signifikansnivån som används för att beräkna konfidensintervallet, ett tal som är större än 0 och mindre än 1."
			},
			{
				name: "standardavvikelse",
				description: "är populationens standardavvikelse för dataområdet och den förväntas vara känd. Standardavvikelse måste vara större än 0."
			},
			{
				name: "storlek",
				description: "är sampelstorleken."
			}
		]
	},
	{
		name: "KONFIDENS.T",
		description: "Returnerar konfidensintervallet för en populations medelvärde, med students T-fördelning.",
		arguments: [
			{
				name: "alfa",
				description: "är signifikansnivån som används för att beräkna konfidensintervallet, ett tal som är större än 0 och mindre än 1."
			},
			{
				name: "standardavvikelse",
				description: "är populationens standardavvikelse för dataområdet och den förväntas vara känd. Standardavvikelse måste vara större än 0."
			},
			{
				name: "storlek",
				description: "är sampelstorleken."
			}
		]
	},
	{
		name: "KONVERTERA",
		description: "Konverterar ett tal från en enhet till en annan.",
		arguments: [
			{
				name: "tal",
				description: "är värdet i ursprungsenheten som ska konverteras"
			},
			{
				name: "ursprungsenhet",
				description: "är enheten för talet"
			},
			{
				name: "ny_enhet",
				description: "är enheten för resultatet"
			}
		]
	},
	{
		name: "KORREL",
		description: "Returnerar korrelationskoefficienten mellan två datamängder.",
		arguments: [
			{
				name: "matris1",
				description: "är ett cellområde med värden. Värdena ska vara tal, namn, matriser eller referenser som innehåller tal."
			},
			{
				name: "matris2",
				description: "är ett andra cellområde med värden. Värdena ska vara tal, namn, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "KOVAR",
		description: "Returnera kovariansen, medelvärdet av produkterna av avvikelser för varje datapunktspar i två datamängder.",
		arguments: [
			{
				name: "matris1",
				description: "är det första cellområdet med heltal och måste vara tal, matriser eller referenser som innehåller tal."
			},
			{
				name: "matris2",
				description: "är det andra cellområdet med heltal och måste vara tal, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "KOVARIANS.P",
		description: "Returnerar populationens kovarians, medelvärdet av produkterna av avvikelser för varje datapunktspar i två datamängder.",
		arguments: [
			{
				name: "matris1",
				description: "är det första cellområdet med heltal och måste vara tal, matriser eller referenser som innehåller tal."
			},
			{
				name: "matris2",
				description: "är det andra cellområdet med heltal och måste vara tal, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "KOVARIANS.S",
		description: "Returnerar samplets kovarians, medelvärdet av produkterna av avvikelser för varje datapunktspar i två datamängder.",
		arguments: [
			{
				name: "matris1",
				description: "är det första cellområdet med heltal och måste vara tal, matriser eller referenser som innehåller tal."
			},
			{
				name: "matris2",
				description: "är det andra cellområdet med heltal och måste vara tal, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "KRITBINOM",
		description: "Returnera det minsta värdet för vilket den kumulativa binomialfördelningen är större än eller lika med villkorsvärdet.",
		arguments: [
			{
				name: "försök",
				description: "är antalet Bernoulli-försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas för varje försök, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "alfa",
				description: "är villkorsvärdet, ett tal mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "KUMPRIS",
		description: "Returnerar det ackumulerade kapitalbeloppet som har betalats på ett lån mellan två perioder.",
		arguments: [
			{
				name: "ränta",
				description: "är räntesatsen"
			},
			{
				name: "antal_perioder",
				description: "är det totala antalet betalningsperioder"
			},
			{
				name: "nuvärde",
				description: "är nuvärdet"
			},
			{
				name: "startperiod",
				description: "är första perioden i beräkningen"
			},
			{
				name: "slutperiod",
				description: "är sista perioden i beräkningen"
			},
			{
				name: "typ",
				description: "är val av tidpunkt för inbetalningar"
			}
		]
	},
	{
		name: "KUMRÄNTA",
		description: "Returnerar den ackumulerade räntan som betalats mellan två perioder.",
		arguments: [
			{
				name: "ränta",
				description: "är räntesatsen"
			},
			{
				name: "antal_perioder",
				description: "är det totala antalet betalningsperioder"
			},
			{
				name: "nuvärde",
				description: "är nuvärdet"
			},
			{
				name: "startperiod",
				description: "är första perioden i beräkningen"
			},
			{
				name: "slutperiod",
				description: "är sista perioden i beräkningen"
			},
			{
				name: "typ",
				description: "är val av tidpunkt för inbetalningar"
			}
		]
	},
	{
		name: "KUPANT",
		description: "Returnerar antalet kuponger som ska betalas mellan likviddag och förfallodag.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "frekvens",
				description: "är antalet kupongutbetalningar/ränteutbetalningar per år"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som ska användas"
			}
		]
	},
	{
		name: "KUPDAGBB",
		description: "Returnerar antal dagar från början av kupongperioden till likviddagen.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodatum uttryckt som ett datumserienummer"
			},
			{
				name: "frekvens",
				description: "är antalet kupongutbetalningar/ränteutbetalningar per år"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som ska användas"
			}
		]
	},
	{
		name: "KUPFKD",
		description: "Returnerar senaste kupongdatum före likviddagen.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "frekvens",
				description: "är antalet kupongutbetalningar/ränteutbetalningar per år"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som ska användas"
			}
		]
	},
	{
		name: "KUPNKD",
		description: "Returnerar nästa kupongdatum efter likviddagen.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "frekvens",
				description: "är antalet kupongutbetalningar/ränteutbetalningar per år"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som ska användas"
			}
		]
	},
	{
		name: "KVADAVV",
		description: "Returnerar summan av kvadratavvikelserna av datapunkter från deras sampel medelvärde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 argument, matriser eller matrisreferenser, på vilka du vill KVADAVV ska beräkna."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 argument, matriser eller matrisreferenser, på vilka du vill KVADAVV ska beräkna."
			}
		]
	},
	{
		name: "KVADRATSUMMA",
		description: "Returnerar summan av argumentens kvadrater. Argumenten kan vara tal, matriser, namn eller referenser till celler som innehåller tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är från 1 till 255 tal, matriser, namn eller referenser till matriser, som du vill ha summan av kvadraterna av."
			},
			{
				name: "tal2",
				description: "är från 1 till 255 tal, matriser, namn eller referenser till matriser, som du vill ha summan av kvadraterna av."
			}
		]
	},
	{
		name: "KVARTIL",
		description: "Returnera kvartilen av en datamängd.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller cellområdet med numeriska värden som du vill ha kvartilvärdet för."
			},
			{
				name: "kvartil",
				description: "är ett tal: minimivärde = 0; 1:a kvartilen = 1; medianvärde = 2; 3:e kvartilen = 3; maxvärde = 4."
			}
		]
	},
	{
		name: "KVARTIL.EXK",
		description: "Returnerar kvartilen av en datamängd, utifrån percentilvärden från 0..1 exklusiv.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller cellområdet med numeriska värden som du vill ha kvartilvärdet för."
			},
			{
				name: "kvartil",
				description: "är ett tal: minimivärde = 0; 1:a kvartilen = 1; medianvärde = 2; 3:e kvartilen = 3; maximivärde = 4."
			}
		]
	},
	{
		name: "KVARTIL.INK",
		description: "Returnerar kvartilen av en datamängd, utifrån percentilvärden från 0..1 inklusiv.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller cellområdet med numeriska värden som du vill ha kvartilvärdet för."
			},
			{
				name: "kvartil",
				description: "är ett tal: minimivärde = 0; 1:a kvartilen = 1; medianvärde = 2; 3:e kvartilen = 3; maximivärde = 4."
			}
		]
	},
	{
		name: "KVOT",
		description: "Returnerar heltalsdelen av en division.",
		arguments: [
			{
				name: "täljare",
				description: "är täljaren"
			},
			{
				name: "nämnare",
				description: "är nämnaren"
			}
		]
	},
	{
		name: "LÄNGD",
		description: "Returnerar antalet tecken i en textsträng.",
		arguments: [
			{
				name: "text",
				description: "är den text vars längd du vill veta. Blanksteg räknas som tecken."
			}
		]
	},
	{
		name: "LETAKOLUMN",
		description: "Letar i översta raden av en tabell eller matris med värden och returnerar värdet i samma kolumn från en rad som du anger.",
		arguments: [
			{
				name: "letauppvärde",
				description: "är värdet som ska hittas i första raden i en tabell och kan vara ett värde, en referens eller en textsträng."
			},
			{
				name: "tabell",
				description: "är en tabell innehållande, text, tal eller logiska värden där data kommer att sökas efter. Tabell_matris kan vara en referens till ett område eller ett områdesnamn."
			},
			{
				name: "radindex",
				description: "är antalet rader i tabellmatrisen som det matchande värdet ska returneras från. Rad 1 är första raden med värden i tabellen."
			},
			{
				name: "ungefärlig",
				description: "är ett logiskt värde: för att hitta det som bäst stämmer överens i översta raden (sorterat i stigande ordning) = SANT eller utelämnad; vid identisk matchning = FALSKT."
			}
		]
	},
	{
		name: "LETARAD",
		description: "Söker efter ett värde i den vänstra kolumnen i tabellen och returnerar sedan ett värde från en kolumn som du anger i samma rad. Som standard måste tabellen vara sorterad i stigande ordning.",
		arguments: [
			{
				name: "letauppvärde",
				description: "är värdet som ska letas efter i första kolumnen i tabellen och kan vara ett värde, en referens eller en textsträng."
			},
			{
				name: "tabellmatris",
				description: "är en tabell innehållande text, tal eller logiska värden i vilken data tas emot. Tabellmatris kan referera till ett område eller ett områdesnamn."
			},
			{
				name: "kolumnindex",
				description: "är kolumnnumret i tabellmatris varifrån matchande värden ska returneras. Första kolumnen med värden i tabellen är kolumn 1."
			},
			{
				name: "ungefärlig",
				description: "är ett logiskt värde: sök efter det som matchar bäst i den första kolumnen (sorterat i stigande ordning) = SANT eller utelämnad; sök efter exakt matchning = FALSKT."
			}
		]
	},
	{
		name: "LETAUPP",
		description: "Returnerar ett värde antingen från ett enrads- eller enkolumnsområde eller från en matris. Finns med för bakåtkompatibilitet.",
		arguments: [
			{
				name: "letauppvärde",
				description: "är det värde som LETAUPP söker efter i Letaupp_matrisen och kan vara ett tal, text, ett logiskt värde eller en referens till ett värde."
			},
			{
				name: "letauppvektor",
				description: "är ett område som innehåller endast en rad eller kolumn med text, tal eller logiska värden placerade i stigande ordning."
			},
			{
				name: "resultatvektor",
				description: "är ett område som innehåller endast en rad eller kolumn, i samma storlek som Letaupp_matrisen."
			}
		]
	},
	{
		name: "LINAVSKR",
		description: "Returnerar den linjära avskrivningen för en tillgång under en period.",
		arguments: [
			{
				name: "kostnad",
				description: "är initialkostnaden för tillgången."
			},
			{
				name: "restvärde",
				description: "är värdet efter avskrivningen."
			},
			{
				name: "livslängd",
				description: "är antalet perioder tillgången minskar i värde (kallas ibland för en tillgångs avskrivningstid)."
			}
		]
	},
	{
		name: "LN",
		description: "Returnerar den naturliga logaritmen för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är det positiva reella tal som du vill ha den naturliga logaritmen för."
			}
		]
	},
	{
		name: "LOG",
		description: "Returnerar logaritmen av ett tal för basen som du anger.",
		arguments: [
			{
				name: "tal",
				description: "är det positiva reella tal som du vill veta logaritmen för."
			},
			{
				name: "bas",
				description: "är logaritmens bas. Sätts till 10 om den inte anges."
			}
		]
	},
	{
		name: "LOG10",
		description: "Returnerar 10-logaritmen för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är det positiva reella tal som du vill ha 10-logaritmen för."
			}
		]
	},
	{
		name: "LOGINV",
		description: "Returnera inversen till den kumulativa lognormalfördelningsfunktionen av x, där ln(x) normalfördelas med parametrarna Medelvärde och Standardavvikelse.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med lognormalfördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "medelvärde",
				description: "är medelvärdet av ln(x)."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för ln(x), ett positivt tal."
			}
		]
	},
	{
		name: "LOGNORM.FÖRD",
		description: "Returnerar lognormalfördelningen av x, där ln(x) normalfördelas med parametrarna Medelvärde och Standardavvikelse.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett positivt tal."
			},
			{
				name: "medelvärde",
				description: "är medelvärdet av ln(x)."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen av ln(x), ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och använd FALSKT för sannolikhetsfunktionen."
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Returnerar den kumulativa lognormalfördelningsfunktionen av x, där ln(x) normalfördelas med parametrarna Medelvärde och Standardavvikelse.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med lognormalfördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "medelvärde",
				description: "är medelvärdet av ln(x)."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för ln(x), ett positivt tal."
			}
		]
	},
	{
		name: "LOGNORMFÖRD",
		description: "Returnera den kumulativa lognormalfördelningen av x, där ln(x) normalfördelas med parametrarna Medelvärde och Standardavvikelse.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett positivt tal."
			},
			{
				name: "medelvärde",
				description: "är medelvärdet av ln(x)."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för ln(x), ett positivt tal."
			}
		]
	},
	{
		name: "LUTNING",
		description: "Returnerar lutningen av en linjär regressionslinje genom de givna datapunkterna.",
		arguments: [
			{
				name: "kända_y",
				description: "är en matris eller ett cellområde med beroende numeriska datapunkter och kan vara tal, namn, matriser eller referenser som innehåller tal."
			},
			{
				name: "kända_x",
				description: "är en mängd av oberoende datapunkter och kan vara tal, namn, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "MÅNAD",
		description: "Returnerar månaden, ett tal mellan 1 (januari) och 12 (december).",
		arguments: [
			{
				name: "serienummer",
				description: "är ett tal i datum-tid-koden som Spreadsheet använder."
			}
		]
	},
	{
		name: "MAVRUNDA",
		description: "Returnerar ett tal avrundat till en given multipel.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda"
			},
			{
				name: "multipel",
				description: "är den multipel som du vill runda till"
			}
		]
	},
	{
		name: "MAX",
		description: "Returnerar det maximala värdet i en lista av argument. Ignorerar logiska värden och text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 tal, tomma celler, logiska värden eller texttal ur vilka du vill hitta det maximala värdet."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 tal, tomma celler, logiska värden eller texttal ur vilka du vill hitta det maximala värdet."
			}
		]
	},
	{
		name: "MAXA",
		description: "Returnerar det högsta värdet i en mängd värden. Ignorerar inte logiska värden och text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255, tomma celler, logiska värden eller texttal för vilka du vill ha maxvärdet."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255, tomma celler, logiska värden eller texttal för vilka du vill ha maxvärdet."
			}
		]
	},
	{
		name: "MDETERM",
		description: "Returnerar matrisen som är avgörandet av en matris.",
		arguments: [
			{
				name: "matris",
				description: "är en numerisk matris med lika antal rader och kolumner, antingen ett cellområde eller en matriskonstant."
			}
		]
	},
	{
		name: "MEDEL",
		description: "Returnerar medelvärdet av argumenten som kan vara tal, namn, matriser eller referenser som innehåller tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 numeriska argument som du vill ha medelvärdet av."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 numeriska argument som du vill ha medelvärdet av."
			}
		]
	},
	{
		name: "MEDEL.OM",
		description: "Hittar medelvärdet (aritmetiskt medelvärde) för de celler som anges av en given uppsättning villkor.",
		arguments: [
			{
				name: "område",
				description: "är cellområdet som du vill värdera"
			},
			{
				name: "villkor",
				description: "är villkoret i form av ett tal, uttryck eller text som definierar vilka celler som används för att hitta medelvärdet"
			},
			{
				name: "medelområde",
				description: "är de celler som används för att hitta medelvärdet. Om de utelämnas används cellerna i området."
			}
		]
	},
	{
		name: "MEDEL.OMF",
		description: "Hittar medelvärdet (aritmetiskt medelvärde) för de celler som anges av en given uppsättning villkor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "medelområde",
				description: "är de celler som används för att hitta medelvärdet."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som du vill värdera för det specifika villkoret"
			},
			{
				name: "villkor",
				description: "är villkoret i form av ett tal, uttryck eller text som definierar vilka celler som används för att hitta medelvärdet"
			}
		]
	},
	{
		name: "MEDELAVV",
		description: "Returnerar medelvärdet från de absoluta avvikelsernas datapunkter från deras medelvärde. Argument kan vara tal eller namn, matriser eller referenser som innehåller tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 argument som du vill beräkna medelvärdet för från de absoluta avvikelserna."
			},
			{
				name: "tal2",
				description: "är 1 till 255 argument som du vill beräkna medelvärdet för från de absoluta avvikelserna."
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Returnerar medianen eller talet i mitten av de angivna talen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 tal eller namn, matriser eller referenser som innehåller tal ur vilka du vill veta medianen."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 tal eller namn, matriser eller referenser som innehåller tal ur vilka du vill veta medianen."
			}
		]
	},
	{
		name: "MENHET",
		description: "Returnerar enhetsmatrisen för den angivna dimensionen.",
		arguments: [
			{
				name: "dimension",
				description: "är ett heltal som anger dimensionen på den enhetsmatris du vill returnera."
			}
		]
	},
	{
		name: "MGM",
		description: "Returnerar minsta gemensamma multipel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 värden som du vill ha minsta gemensamma multipel för"
			},
			{
				name: "tal2",
				description: "är 1 till 255 värden som du vill ha minsta gemensamma multipel för"
			}
		]
	},
	{
		name: "MIN",
		description: "Returnerar det minsta värdet i en lista av argument. Ignorerar logiska värden och text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 tal, tomma celler, logiska värden eller texttal ur vilka du vill veta minimivärdet."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 tal, tomma celler, logiska värden eller texttal ur vilka du vill veta minimivärdet."
			}
		]
	},
	{
		name: "MINA",
		description: "Returnerar det lägsta värdet i en mängd värden. Ignorerar inte logiska värden och text.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255 tal, tomma celler, logiska värden eller texttal ur vilka du vill veta minimivärdet."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255 tal, tomma celler, logiska värden eller texttal ur vilka du vill veta minimivärdet."
			}
		]
	},
	{
		name: "MINSTA",
		description: "Returnerar det n:te minsta värdet i en datamängd, t ex det femte minsta talet.",
		arguments: [
			{
				name: "matris",
				description: "är en matris eller ett område av numeriska värde som du vill bestämma det n:te minsta värdet ur."
			},
			{
				name: "n",
				description: "är den position (från det minsta värdet) i matrisen eller dataområdet som ska returneras."
			}
		]
	},
	{
		name: "MINUT",
		description: "Returnerar minuten, ett tal från 0 till 59.",
		arguments: [
			{
				name: "serienummer",
				description: "är ett tal i datum-tid-koden som Spreadsheet använder eller text i tidsformat, t ex 16:48:00."
			}
		]
	},
	{
		name: "MINVERT",
		description: "Returnerar en invers av en matris för matrisen som är lagrad i en matris.",
		arguments: [
			{
				name: "matris",
				description: "är en numerisk matris med lika antal rader och kolumner, antingen ett cellområde eller en konstant i en matris."
			}
		]
	},
	{
		name: "MMULT",
		description: "Returnerar matrisprodukten av två matriser, en matris med samma antal rader som matris1 och samma antal kolumner som matris2.",
		arguments: [
			{
				name: "matris1",
				description: "är första matrisen med tal som ska multipliceras och måste därför ha lika många kolumner som Matris2 har rader."
			},
			{
				name: "matris2",
				description: "är första matrisen med tal som ska multipliceras och måste därför ha lika många kolumner som Matris2 har rader."
			}
		]
	},
	{
		name: "MODIR",
		description: "Returnerar avkastningsgraden för en serie periodiska penningflöden med tanke på både investeringskostnader och räntan på återinvesteringen av pengar.",
		arguments: [
			{
				name: "värden",
				description: "är en matris eller en referens till celler som innehåller tal som representerar en serie betalningar (negativa) och inkomster (positiva) under regelbundna perioder."
			},
			{
				name: "kapitalränta",
				description: "är den ränta du betalar för pengar som används i betalningsströmmarna."
			},
			{
				name: "återinvesteringsränta",
				description: "är den ränta du får på betalningsströmmarna när du återinvesterar dem."
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Returnerar multinomialen för en uppsättning tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 värden som du vill ha multinomialen för"
			},
			{
				name: "tal2",
				description: "är 1 till 255 värden som du vill ha multinomialen för"
			}
		]
	},
	{
		name: "N",
		description: "Konverterar värden som inte är tal till tal, datum till serienummer, SANT till 1 och allt annat till 0 (noll).",
		arguments: [
			{
				name: "värde",
				description: "är värdet som du vill konvertera."
			}
		]
	},
	{
		name: "NEGBINOM.FÖRD",
		description: "Returnerar den negativa binomialfördelningen, sannolikheten att Antal_M försök ska misslyckas innan Antal_L lyckas, med Sannolikhet_Ls sannolikhet att lyckas.",
		arguments: [
			{
				name: "antal_m",
				description: "är antalet misslyckade försök."
			},
			{
				name: "antal_l",
				description: "är tröskelvärdet för antalet lyckade försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas, ett tal mellan 0 och 1."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och använd FALSKT för sannolikhetsfunktionen."
			}
		]
	},
	{
		name: "NEGBINOMFÖRD",
		description: "Returnera den negativa binomialfördelningen, sannolikheten att Antal_M försök ska misslyckas innan Antal_L lyckas, med Sannolikhet_Ls sannolikhet att lyckas.",
		arguments: [
			{
				name: "antal_m",
				description: "är antalet misslyckade försök."
			},
			{
				name: "antal_l",
				description: "är tröskelvärdet för antalet lyckade försök."
			},
			{
				name: "sannolikhet_l",
				description: "är sannolikheten att lyckas, ett tal mellan 0 och 1."
			}
		]
	},
	{
		name: "NETNUVÄRDE",
		description: "Returnerar nuvärdet av en serie betalningar baserad på en diskonteringsränta och serier med framtida betalningar (negativa värden) och inkomster (positiva värden).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ränta",
				description: "är räntesatsen per period."
			},
			{
				name: "värde1",
				description: "är från 1 till 254 betalningar och inkomster jämt fördelade i tiden och förekommande i slutet av varje period."
			},
			{
				name: "värde2",
				description: "är från 1 till 254 betalningar och inkomster jämt fördelade i tiden och förekommande i slutet av varje period."
			}
		]
	},
	{
		name: "NETTOARBETSDAGAR",
		description: "Returnerar antalet hela arbetsdagar mellan två datum.",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum"
			},
			{
				name: "stoppdatum",
				description: "är ett datumserienummer som representerar slutdatum"
			},
			{
				name: "lediga",
				description: "är en valfri mängd av en eller flera seriedatumnummer som inte ska räknas som arbetsdagar, t.ex. högtider."
			}
		]
	},
	{
		name: "NETTOARBETSDAGAR.INT",
		description: "Returnerar antalet hela arbetsdagar mellan två datum med egna helgparametrar.",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum."
			},
			{
				name: "stoppdatum",
				description: "är ett datumserienummer som representerar slutdatum."
			},
			{
				name: "helg",
				description: "är ett nummer eller en sträng som anger när helger infaller."
			},
			{
				name: "lediga",
				description: "är en valfri mängd om en eller flera datumserienummer som inte ska räknas som arbetsdagar, till exempel högtider."
			}
		]
	},
	{
		name: "NOMAVKDISK",
		description: "Returnerar den årliga avkastningen för diskonterade värdepapper.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "pris",
				description: "är värdepapperets pris per 1 000 kr nominellt värde"
			},
			{
				name: "inlösen",
				description: "är värdepapperets inlösningsvärde per 1 000 kr nominellt värde"
			},
			{
				name: "bas",
				description: "är bastypen för antal dagar som ska användas"
			}
		]
	},
	{
		name: "NOMRÄNTA",
		description: "Returnerar den årliga nominella räntesatsen.",
		arguments: [
			{
				name: "effektiv_ränta",
				description: "är den effektiva räntesatsen"
			},
			{
				name: "antal_perioder",
				description: "är antalet ränteperioder per år"
			}
		]
	},
	{
		name: "NORM.FÖRD",
		description: "Returnerar normalfördelningen för det angivna medelvärdet och standardavvikelsen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen."
			},
			{
				name: "medelvärde",
				description: "är det aritmetiska medelvärdet av fördelningen."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för fördelningen, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen."
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Returnerar inversen till den kumulativa normalfördelningen för det angivna medelvärdet och standardavvikelsen.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är ett sannolikhetsvärde som motsvarar normalfördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "medelvärde",
				description: "är det aritmetiska medelvärdet av fördelningen."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för fördelningen, ett positivt tal."
			}
		]
	},
	{
		name: "NORM.S.FÖRD",
		description: "Returnerar standardnormalfördelningen (har ett medelvärde på noll och en standardavvikelse på ett).",
		arguments: [
			{
				name: "z",
				description: "är värdet där du vill beräkna fördelningen."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde som funktionen ska returnera: den kumulativa fördelningsfunktionen = SANT, sannolikhetsfunktionen = FALSKT."
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Returnerar inversen till den kumulativa standardnormalfördelningen (har ett medelvärde på noll och en standardavvikelse på ett).",
		arguments: [
			{
				name: "sannolikhet",
				description: "är ett sannolikhetsvärde som motsvarar normalfördelningen, ett tal mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "NORMFÖRD",
		description: "Returnera den kumulativa normalfördelningen för det angivna medelvärdet och standardavvikelsen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna fördelningen."
			},
			{
				name: "medelvärde",
				description: "är det aritmetiska medelvärdet av fördelningen."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för fördelningen, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen."
			}
		]
	},
	{
		name: "NORMINV",
		description: "Returnera inversen till den kumulativa normalfördelningen för det angivna medelvärdet och standardavvikelsen.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är ett sannolikhetsvärde som motsvarar normalfördelningen, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "medelvärde",
				description: "är det aritmetiska medelvärdet av fördelningen."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för fördelningen, ett positivt tal."
			}
		]
	},
	{
		name: "NORMSFÖRD",
		description: "Returnera den kumulativa standardnormalfördelningen (har ett medelvärde på noll och en standardavvikelse på ett).",
		arguments: [
			{
				name: "z",
				description: "är värdet där du vill beräkna fördelningen."
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Returnera inversen till den kumulativa standardnormalfördelningen (har ett medelvärde på noll och en standardavvikelse på ett).",
		arguments: [
			{
				name: "sannolikhet",
				description: "är ett sannolikhetsvärde som motsvarar normalfördelningen, ett tal mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "NU",
		description: "Returnerar dagens datum och aktuell tid formaterat som datum och tid.",
		arguments: [
		]
	},
	{
		name: "NUVÄRDE",
		description: "Returnerar nuvärdet av en investering: den totala summan som en serie med framtida betalningar är värd nu.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "periodantal",
				description: "är det totala antalet betalningsperioder i en investering."
			},
			{
				name: "betalning",
				description: "är den betalning som görs över varje period och den kan inte ändras under investeringens giltighet."
			},
			{
				name: "slutvärde",
				description: "är det framtida värde eller det saldo som du vill uppnå efter att sista betalningen har gjorts."
			},
			{
				name: "typ",
				description: "är ett logiskt värde: betalning i början av perioden = 1; betalning i slutet av perioden = 0 eller utelämnat."
			}
		]
	},
	{
		name: "OCH",
		description: "Kontrollerar om alla argument utvärderas till SANT och returnerar SANT om alla dess argument är lika med SANT.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "är 1 till 255 villkor du vill testa som kan ha värdet SANT eller FALSKT och som kan vara logiska värden, matriser eller referenser."
			},
			{
				name: "logisk2",
				description: "är 1 till 255 villkor du vill testa som kan ha värdet SANT eller FALSKT och som kan vara logiska värden, matriser eller referenser."
			}
		]
	},
	{
		name: "OKT.TILL.BIN",
		description: "Konverterar ett oktalt tal till ett binärt.",
		arguments: [
			{
				name: "tal",
				description: "är det oktala tal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken du vill använda"
			}
		]
	},
	{
		name: "OKT.TILL.DEC",
		description: "Konverterar ett oktalt tal till ett decimalt.",
		arguments: [
			{
				name: "tal",
				description: "är det oktala tal som du vill konvertera"
			}
		]
	},
	{
		name: "OKT.TILL.HEX",
		description: "Konverterar ett oktalt tal till ett hexadecimalt.",
		arguments: [
			{
				name: "tal",
				description: "är det oktala tal som du vill konvertera"
			},
			{
				name: "antal_siffror",
				description: "är det antal tecken du vill använda"
			}
		]
	},
	{
		name: "OM",
		description: "Kontrollerar om ett villkor uppfylls och returnerar ett värde om ett villkor beräknas till SANT och ett annat värde om det beräknas till FALSKT.",
		arguments: [
			{
				name: "logisk_test",
				description: "är ett värde eller ett uttryck som kan beräknas till SANT eller FALSKT."
			},
			{
				name: "värde_om_sant",
				description: "är värdet som returneras om Logiskt_test är SANT. Om det utelämnas returneras SANT. Du kan kapsla upp till sju OM-funktioner."
			},
			{
				name: "värde_om_falskt",
				description: "är värdet som returnerar om Logiskt_test är FALSKT. Om det utelämnas returneras FALSKT."
			}
		]
	},
	{
		name: "OMFEL",
		description: "Returnerar värde_om_fel om uttrycket är ett fel, i annat fall värdet för uttrycket.",
		arguments: [
			{
				name: "värde",
				description: "är ett värde, ett uttryck eller en referens"
			},
			{
				name: "värde_om_fel",
				description: "är ett värde, ett uttryck eller en referens"
			}
		]
	},
	{
		name: "OMRÅDEN",
		description: "Returnerar antalet områden i en referens. Ett område är sammanhängande celler eller en enda cell.",
		arguments: [
			{
				name: "ref",
				description: "är en referens till en cell, ett område av celler eller flera områden."
			}
		]
	},
	{
		name: "OMSAKNAS",
		description: "Returnerar ett angivet värde om uttrycket ger #SAKNAS, annars returneras resultatet av uttrycket.",
		arguments: [
			{
				name: "värde",
				description: "är ett värde, ett uttryck eller en referens."
			},
			{
				name: "värde_om_saknas",
				description: "är ett värde, ett uttryck eller en referens."
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
		name: "PASSA",
		description: "Returnerar elementets relativa position i en matris som matchar ett angivet värde i en angiven ordning.",
		arguments: [
			{
				name: "letauppvärde",
				description: "är värdet som du använder för att hitta värdet du vill ha i matrisen, t ex ett tal, text, ett logiskt värde eller en referens till något av dessa."
			},
			{
				name: "letauppvektor",
				description: "är ett kontinuerligt område av celler som innehåller möjliga sökvärden, som t ex ett matrisvärde eller en referens till en matris."
			},
			{
				name: "typ",
				description: "är ett tal 1, 0 eller -1 som anger vilket värde som ska returneras."
			}
		]
	},
	{
		name: "PEARSON",
		description: "Returnerar korrelationskoefficienten till Pearsons momentprodukt, r.",
		arguments: [
			{
				name: "matris1",
				description: "är mängden av oberoende värden."
			},
			{
				name: "matris2",
				description: "är mängden av beroende värden."
			}
		]
	},
	{
		name: "PERCENTIL",
		description: "Returnera den n:te percentilen av värden i ett område.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller dataområdet som definierar relativ position."
			},
			{
				name: "n",
				description: "är percentilvärdet mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "PERCENTIL.EXK",
		description: "Returnerar den n:te percentilen av värden i ett område, där n är i intervallet 0..1 exklusiv.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller dataområdet som definierar relativ position."
			},
			{
				name: "n",
				description: "är percentilvärdet mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "PERCENTIL.INK",
		description: "Returnerar den n:te percentilen av värden i ett område, där n är i intervallet 0..1 inklusiv.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller dataområdet som definierar relativ position "
			},
			{
				name: "n",
				description: "är percentilvärdet mellan 0 och 1 inklusiv."
			}
		]
	},
	{
		name: "PERIODER",
		description: "Returnerar antalet perioder för en investering baserad på periodisk betalning och en konstant ränta.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "betalning",
				description: "är den betalning som görs varje period. Den kan inte ändras under investeringens giltighet."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet eller det nuvarande samlade värdet av en serie framtida betalningar."
			},
			{
				name: "slutvärde",
				description: "är det framtida värde eller det saldo som du vill uppnå efter att sista betalningen har gjorts. Om det utesluts används värdet noll."
			},
			{
				name: "typ",
				description: "är ett logiskt värde: betalning i början av perioden = 1; betalning i slutet av perioden = 0 eller utelämnat."
			}
		]
	},
	{
		name: "PERMUT",
		description: "Returnerar antal permutationer för ett givet antal objekt som kan väljas från de totala antalet objekt.",
		arguments: [
			{
				name: "tal",
				description: "är det totala antalet objekt."
			},
			{
				name: "valt_tal",
				description: "är antalet objekt i varje permutation."
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Returnerar antal permutationer för ett givet antal objekt (med repetitioner) som kan väljas från det totala antalet objekt.",
		arguments: [
			{
				name: "tal",
				description: "är det totala antalet objekt."
			},
			{
				name: "valt_tal",
				description: "är antalet objekt i varje permutation."
			}
		]
	},
	{
		name: "PHI",
		description: "Returnerar värdet för densitetsfunktionen för en standardnormalfördelning.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna densiteten för standardnormalfördelningen."
			}
		]
	},
	{
		name: "PI",
		description: "Returnerar värdet pi, 3,14159265358979, med 15 decimaler.",
		arguments: [
		]
	},
	{
		name: "PLÖPTID",
		description: "Returnerar antalet perioder som krävs för en investering att uppnå ett visst värde.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet för investeringen."
			},
			{
				name: "slutvärde",
				description: "är det önskade framtida värdet på investeringen."
			}
		]
	},
	{
		name: "POISSON",
		description: "Returnera Poisson-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är antalet händelser."
			},
			{
				name: "medelvärde",
				description: "är det förväntade numeriska värdet, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa Poisson-sannolikheten och FALSKT för Poisson-sannolikhetsfunktionens massa."
			}
		]
	},
	{
		name: "POISSON.FÖRD",
		description: "Returnerar Poisson-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är antalet händelser."
			},
			{
				name: "medelvärde",
				description: "är det förväntade numeriska värdet, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa Poisson-sannolikheten och FALSKT för Poisson-sannolikhetsfunktionens massa."
			}
		]
	},
	{
		name: "PREDIKTION",
		description: "Beräknar eller förutsäger ett framtida värde längs en linjär trendlinje genom att använda redan existerande värden.",
		arguments: [
			{
				name: "x",
				description: "är datapunkten där du vill förutsäga värdet och måste vara ett numeriskt värde."
			},
			{
				name: "kända_y",
				description: "är den underordnade matrisen eller ett område med data."
			},
			{
				name: "kända_x",
				description: "är den oberoende matrisen eller område med numeriska data. Variansen av kända_x måste vara noll."
			}
		]
	},
	{
		name: "PRISDISK",
		description: "Returnerar priset per 1 000 kr nominellt värde för en diskonterad säkerhet.",
		arguments: [
			{
				name: "betalning",
				description: "är värdepapperets likviddag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "ränta",
				description: "är värdepapperets diskonteringsränta"
			},
			{
				name: "inlösen",
				description: "är värdepapperets inlösningsvärde per 1 000 kr nominellt värde"
			},
			{
				name: "bas",
				description: "är bastypen för antal dagar som ska användas"
			}
		]
	},
	{
		name: "PROCENTRANG",
		description: "Returnera rangen för ett värde i en datamängd i procent av datamängden.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller dataområdet med numeriska värden som definierar relativ position."
			},
			{
				name: "x",
				description: "är det värde som du vill veta rangen för."
			},
			{
				name: "signifikans",
				description: "är ett frivilligt värde som anger antal signifikanta siffror i det returnerade procentvärdet. Om det utelämnas används tre siffror (0,xxx %)."
			}
		]
	},
	{
		name: "PROCENTRANG.EXK",
		description: "Returnerar rangen för ett värde i en datamängd som en andel i procent (0..1 exklusiv) av datamängden.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller dataområdet med numeriska värden som definierar relativ position."
			},
			{
				name: "x",
				description: "är det värde som du vill veta rangen för."
			},
			{
				name: "signifikans",
				description: "är ett frivilligt värde som anger antal signifikanta siffror i det returnerade procentvärdet. Om det utelämnas används tre siffror (0,xxx %)."
			}
		]
	},
	{
		name: "PROCENTRANG.INK",
		description: "Returnerar rangen för ett värde i en datamängd som en andel i procent (0..1 inklusiv) av datamängden.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller dataområdet med numeriska värden som definierar relativ position."
			},
			{
				name: "x",
				description: "är det värde som du vill veta rangen för."
			},
			{
				name: "signifikans",
				description: "är ett frivilligt värde som anger antal signifikanta siffror i det returnerade procentvärdet. Om det utelämnas används tre siffror (0,xxx %)."
			}
		]
	},
	{
		name: "PRODUKT",
		description: "Multiplicerar alla tal angivna som argument.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 tal, logiska värden eller texttal som du vill multiplicera."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 tal, logiska värden eller texttal som du vill multiplicera."
			}
		]
	},
	{
		name: "PRODUKTSUMMA",
		description: "Returnerar summan av produkter av korresponderande områden eller matriser.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matris1",
				description: "är 2 till 255 matriser för vilka du vill multiplicera och sedan addera komponenter. Alla matriser måste ha samma dimensioner."
			},
			{
				name: "matris2",
				description: "är 2 till 255 matriser för vilka du vill multiplicera och sedan addera komponenter. Alla matriser måste ha samma dimensioner."
			},
			{
				name: "matris3",
				description: "är 2 till 255 matriser för vilka du vill multiplicera och sedan addera komponenter. Alla matriser måste ha samma dimensioner."
			}
		]
	},
	{
		name: "RAD",
		description: "Returnerar en referens radnummer.",
		arguments: [
			{
				name: "ref",
				description: "är cellen eller ett enkelt område av celler som du vill ha radnumret från; om det utelämnas returneras cellen som innehåller RAD-funktionen."
			}
		]
	},
	{
		name: "RADER",
		description: "Returnerar antal rader i en referens eller matris.",
		arguments: [
			{
				name: "matris",
				description: "är en matris, en matrisformel eller en referens till ett område med celler som du vill veta antalet rader i."
			}
		]
	},
	{
		name: "RADIANER",
		description: "Konverterar grader till radianer.",
		arguments: [
			{
				name: "vinkel",
				description: "är en vinkel, i grader, som du vill konvertera."
			}
		]
	},
	{
		name: "RALÅN",
		description: "Returnerar den ränta som betalats under en viss period för en investering.",
		arguments: [
			{
				name: "ränta",
				description: "räntesats per period. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "period",
				description: "period för vilken du vill ta reda på räntan."
			},
			{
				name: "periodantal",
				description: "antal betalningsperioder i en investering."
			},
			{
				name: "nuvärde",
				description: "klumpsumma som motsvarar nuvärdet av en serie framtida betalningar."
			}
		]
	},
	{
		name: "RANG",
		description: "Returnera rangordningen för ett tal i en lista med tal: dess storlek i relation till andra värden i listan.",
		arguments: [
			{
				name: "tal",
				description: "är talet vars rang du vill veta."
			},
			{
				name: "ref",
				description: "är en matris av eller en referens till en lista med tal. Icke-numeriska värden ignoreras."
			},
			{
				name: "ordning",
				description: "är ett tal: rangordning i listan som är sorterad i fallande ordning = 0 eller utelämnat; rangordning i listan som är sorterad i stigande ordning = vilket icke-nollvärde som helst."
			}
		]
	},
	{
		name: "RANG.EKV",
		description: "Returnerar rangordningen för ett tal i en lista med tal: dess storlek i relation till andra värden i listan; om fler än ett värde har samma rang returneras den högsta rangen i den mängden.",
		arguments: [
			{
				name: "tal",
				description: "är talet vars rang du vill veta."
			},
			{
				name: "ref",
				description: "är en matris av eller en referens till en lista med tal. Icke-numeriska värden ignoreras."
			},
			{
				name: "ordning",
				description: "är ett tal: rangordning i listan som är sorterad i fallande ordning = 0 eller utelämnat; rangordning i listan som är sorterad i stigande ordning = alla icke-nollvärden."
			}
		]
	},
	{
		name: "RANG.MED",
		description: "Returnerar rangordningen för ett tal i en lista med tal: dess storlek i relation till andra värden i listan; om fler än ett värde har samma rang returneras medelvärdet.",
		arguments: [
			{
				name: "tal",
				description: "är talet vars rang du vill veta."
			},
			{
				name: "ref",
				description: "är en matris av eller en referens till en lista med tal. Icke-numeriska värden ignoreras."
			},
			{
				name: "ordning",
				description: "är ett tal: rangordning i listan som är sorterad i fallande ordning = 0 eller utelämnat; rangordning i listan som är sorterad i stigande ordning = alla icke-nollvärden."
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
		name: "RÄNTA",
		description: "Returnerar räntesatsen per period för ett lån eller en investering. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta.",
		arguments: [
			{
				name: "periodantal",
				description: "är det totala antalet betalningsperioder för lånet eller investeringen."
			},
			{
				name: "betalning",
				description: "är den betalning som görs varje period och den kan inte ändras under lånets eller investeringens giltighet."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet eller summan av det nuvarande värdet av ett antal framtida betalningar."
			},
			{
				name: "slutvärde",
				description: "är det framtida värdet eller det saldo som du vill uppnå efter att sista betalningen har gjorts. Om det utelämnas används Fv = 0."
			},
			{
				name: "typ",
				description: "är ett logiskt värde: betalning i början av perioden = 1; betalning i slutet av perioden = 0 eller utelämnat."
			},
			{
				name: "gissning",
				description: "är din gissning vad räntan kommer att bli; om utelämnat sätts värdet Gissa = 0,1 (10 procent)"
			}
		]
	},
	{
		name: "RBETALNING",
		description: "Returnerar ränteinbetalningen för en investering och vald period, baserat på periodiska, konstanta betalningar och en konstant ränta.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "period",
				description: "är perioden som räntan ska beräknas för och måste ligga i intervallet 1 till periodantal."
			},
			{
				name: "periodantal",
				description: "är det totala antalet betalningsperioder i en investering."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet eller det nuvarande samlade värdet av en serie framtida betalningar."
			},
			{
				name: "slutvärde",
				description: "är det framtida värde eller det saldo som du vill uppnå efter att sista betalningen har gjorts. Om det utesluts så är Fv = 0."
			},
			{
				name: "typ",
				description: "är ett logiskt värde som representerar när betalningar ska ske: slutet av perioden = 0 eller utelämnad, i början av perioden = 1."
			}
		]
	},
	{
		name: "REGR",
		description: "Returnerar statistik som beskriver en linjär trend som matchar kända datapunkter, genom att passa in en rak linje och använda minsta kvadrat-metoden.",
		arguments: [
			{
				name: "kända_y",
				description: "är den mängd y-värden som är kända i förhållandet y = mx + b."
			},
			{
				name: "kända_x",
				description: "är en frivillig mängd x-värden som är kända i förhållandet y = mx + b."
			},
			{
				name: "konst",
				description: "är ett logiskt värde: konstanten b beräknas normalt om konst = SANT eller utesluten; konstanten b ska vara 0 om konst = FALSKT."
			},
			{
				name: "statistik",
				description: "är ett logiskt värde: returnera extra regressionsstatistik = SANT; returnera m-koefficienter och konstanten b = FALSKT eller utelämnad."
			}
		]
	},
	{
		name: "RENSA",
		description: "Tar bort alla blanksteg från en textsträng förutom enkla blanksteg mellan ord.",
		arguments: [
			{
				name: "text",
				description: "är texten som du vill ta bort blanksteg från."
			}
		]
	},
	{
		name: "REP",
		description: "Upprepar en text ett bestämt antal gånger. Använd REP för att fylla en cell med samma textsträng flera gånger.",
		arguments: [
			{
				name: "text",
				description: "är texten som du vill upprepa."
			},
			{
				name: "antal_gånger",
				description: "är ett positivt tal som anger antal gånger texten ska upprepas."
			}
		]
	},
	{
		name: "REST",
		description: "Returnerar resten efter att ett tal har dividerats med en divisor.",
		arguments: [
			{
				name: "tal",
				description: "är talet som du vill ha resten av efter att divisionen utförts."
			},
			{
				name: "divisor",
				description: "är talet som du dividerar det andra talet med."
			}
		]
	},
	{
		name: "RKV",
		description: "Returnerar korrelationskoefficienten till Pearsons momentprodukt genom de givna datapunkterna.",
		arguments: [
			{
				name: "kända_y",
				description: "är en matris eller ett område med datapunkter och kan vara tal, namn, matriser eller referenser som innehåller tal."
			},
			{
				name: "kända_x",
				description: "är en matris eller ett område med datapunkter och kan vara tal, namn, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "ROMERSK",
		description: "Konverterar arabiska siffror till romerska, som text.",
		arguments: [
			{
				name: "tal",
				description: "är det tal med arabiska siffror som du vill konvertera."
			},
			{
				name: "format",
				description: "är den siffra som anger vilken typ av romersk siffra du vill ha."
			}
		]
	},
	{
		name: "ROT",
		description: "Returnerar ett tals kvadratrot.",
		arguments: [
			{
				name: "tal",
				description: "är talet som du vill ha kvadratroten av."
			}
		]
	},
	{
		name: "ROTPI",
		description: "Returnerar kvadratroten av (tal * pi).",
		arguments: [
			{
				name: "tal",
				description: "är det tal som pi multipliceras med"
			}
		]
	},
	{
		name: "RTD",
		description: "Hämtar realtidsdata från ett program som stöder COM-automation.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "är namnet på progID för ett registrerat COM-automationstillägg. Sätt citattecken runt namnet."
			},
			{
				name: "server",
				description: "är namnet på den server som tilläggen bör köras ifrån. Sätt citattecken runt namnet. Om tillägget körs lokalt använder du en tom sträng."
			},
			{
				name: "ämne1",
				description: "är 1 till 38 parametrar som anger ett datablock."
			},
			{
				name: "ämne2",
				description: "är 1 till 38 parametrar som anger ett datablock."
			}
		]
	},
	{
		name: "RUNDA.NER",
		description: "Avrundar ett tal nedåt, till närmaste signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är det numeriska värde som du vill avrunda."
			},
			{
				name: "signifikans",
				description: "är den multipel som du vill avrunda till. tal och Signifikans måste båda vara antingen negativa eller positiva."
			}
		]
	},
	{
		name: "RUNDA.NER.EXAKT",
		description: "Avrundar ett tal nedåt, till närmsta heltal eller till närmsta signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är det numeriska värde som du vill avrunda."
			},
			{
				name: "signifikans",
				description: "är den multipel som du vill avrunda till. Tal och Signifikans måste båda vara antingen negativa eller positiva."
			}
		]
	},
	{
		name: "RUNDA.NER.MATEMATISKT",
		description: "Avrundar ett tal nedåt till närmaste heltal eller närmaste signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda."
			},
			{
				name: "signifikans",
				description: "är den multipel som du vill avrunda till."
			},
			{
				name: "typvärde",
				description: "den här funktionen avrundar nedåt mot noll när den har angivits och inte är noll."
			}
		]
	},
	{
		name: "RUNDA.UPP",
		description: "Avrundar ett tal uppåt, till närmaste signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill runda av."
			},
			{
				name: "signifikans",
				description: "är den multipel som du vill avrunda till."
			}
		]
	},
	{
		name: "RUNDA.UPP.EXAKT",
		description: "Avrundar ett tal till närmaste heltal eller närmaste signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda."
			},
			{
				name: "signifikans",
				description: "är den multipel som du vill avrunda till."
			}
		]
	},
	{
		name: "RUNDA.UPP.MATEMATISKT",
		description: "Avrundar ett tal uppåt till närmaste heltal eller till närmaste signifikanta multipel.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda."
			},
			{
				name: "signifikans",
				description: "är den multipel som du vill avrunda till."
			},
			{
				name: "typvärde",
				description: "den här funktionen avrundar uppåt från noll när den har angivits och inte är noll."
			}
		]
	},
	{
		name: "SAKNAS",
		description: "Returnerar felvärdet #Saknas (värdet är inte tillgängligt).",
		arguments: [
		]
	},
	{
		name: "SAMMANFOGA",
		description: "Sammanfogar flera textsträngar till en.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "text1",
				description: "är 1 till 255 textsträngar som ska sammanfogas till en enda textsträng och kan vara textsträngar, tal eller cellreferenser till en enskild cell."
			},
			{
				name: "text2",
				description: "är 1 till 255 textsträngar som ska sammanfogas till en enda textsträng och kan vara textsträngar, tal eller cellreferenser till en enskild cell."
			}
		]
	},
	{
		name: "SANNOLIKHET",
		description: "Returnerar sannolikheten att värden i ett område ligger mellan två gränser eller är lika med en lägre gräns.",
		arguments: [
			{
				name: "x_område",
				description: "är det intervall med numeriska värden på x till vilka det finns motsvarande sannolikheter."
			},
			{
				name: "sannolikhetsområde",
				description: "är ett antal sannolikheter som är associerade med värden i X-området, värden mellan 0 och 1 och exkluderat 0."
			},
			{
				name: "undre_gräns",
				description: "är det undre gränsvärdet på värdet som du vill ha sannolikheter till."
			},
			{
				name: "övre_gräns",
				description: "är ett frivilligt övre gränsvärde på värdena. Om utelämnad, så returnerar SANNOLIKHET sannolikheten att värdena i X_området är lika med undre_gräns."
			}
		]
	},
	{
		name: "SANT",
		description: "Returnerar det logiska värdet SANT.",
		arguments: [
		]
	},
	{
		name: "SEK",
		description: "Returnerar sekant för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha sekant för."
			}
		]
	},
	{
		name: "SEKH",
		description: "Returnerar hyperbolisk sekant för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha hyperbolisk sekant för."
			}
		]
	},
	{
		name: "SEKUND",
		description: "Returnerar sekunden, ett tal från 0 till 59.",
		arguments: [
			{
				name: "serienummer",
				description: "är ett tal i datum-tid-koden som Spreadsheet använder eller text i tidsformat, t ex 16:48:23."
			}
		]
	},
	{
		name: "SERIESUMMA",
		description: "Returnerar summan av en potensserie baserad på formeln.",
		arguments: [
			{
				name: "x",
				description: "är indatavärdet till potensserien"
			},
			{
				name: "n",
				description: "är initialpotensen som du vill upphöja x till"
			},
			{
				name: "m",
				description: "anger hur mycket n ska ökas för varje term i potensserien"
			},
			{
				name: "koefficienter",
				description: "är en mängd av koefficienter som varje efterföljande potens av x ska multipliceras med"
			}
		]
	},
	{
		name: "SGD",
		description: "Returnerar den största gemensamma nämnaren.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 värden"
			},
			{
				name: "tal2",
				description: "är 1 till 255 värden"
			}
		]
	},
	{
		name: "SIN",
		description: "Returnerar sinus för en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "är vinkeln i radianer som du vill ha sinus för. Grader * PI()/180 = radianer."
			}
		]
	},
	{
		name: "SINH",
		description: "Returnerar hyperbolisk sinus för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal."
			}
		]
	},
	{
		name: "SKÄRNINGSPUNKT",
		description: "Beräknar punkten där en linje korsar y-axeln genom att använda en regressionslinje ritad genom de kända de kända x- och y-värdena.",
		arguments: [
			{
				name: "kända_y",
				description: "är den beroende mängden i observationen av data och kan vara tal, namn, matriser eller referenser som innehåller tal."
			},
			{
				name: "kända_x",
				description: "är den oberoende mängden observationer eller data och kan vara tal, namn, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "SLSTEG",
		description: "Testar om ett tal är större än ett gränsvärde.",
		arguments: [
			{
				name: "tal",
				description: "är värdet att testa mot steg"
			},
			{
				name: "steg",
				description: "är tröskelvärdet"
			}
		]
	},
	{
		name: "SLUMP",
		description: "Returnerar ett slumptal större än eller lika med 0 och mindre än 1 (ändringar sker vid omberäkning).",
		arguments: [
		]
	},
	{
		name: "SLUMP.MELLAN",
		description: "Returnerar ett slumptal mellan de tal som du anger.",
		arguments: [
			{
				name: "nedre",
				description: "är det minsta heltal som Slump.mellan kommer att returnera"
			},
			{
				name: "övre",
				description: "är det största heltal som Slump.mellan kommer att returnera"
			}
		]
	},
	{
		name: "SLUTMÅNAD",
		description: "Returnerar ett serienummer till den sista dagen i månaden före eller efter ett angivet antal månader.",
		arguments: [
			{
				name: "startdatum",
				description: "är ett datumserienummer som representerar startdatum"
			},
			{
				name: "månader",
				description: "är antalet månader före eller efter startdatumet"
			}
		]
	},
	{
		name: "SLUTVÄRDE",
		description: "Returnerar det framtida värdet av en investering, baserat på en periodisk, konstant betalning och en konstant ränta.",
		arguments: [
			{
				name: "ränta",
				description: "är räntan per period. Använd t ex 6%/4 för kvartalsvisa betalningar med 6% ränta."
			},
			{
				name: "periodantal",
				description: "är det totala antalet betalningsperioder i investeringen."
			},
			{
				name: "betalning",
				description: "är den betalning som görs varje period. Den kan inte ändras under investeringens giltighet."
			},
			{
				name: "nuvärde",
				description: "är nuvärdet eller det nuvarande samlade värdet av en serie framtida betalningar. Om det utelämnas så är Pv = 0."
			},
			{
				name: "typ",
				description: "är ett värde som representerar när betalningen sker: en betalning i början av perioden = 1; en betalning i slutet av perioden = 0 eller utelämnad."
			}
		]
	},
	{
		name: "SNEDHET",
		description: "Returnerar snedheten i en fördelning, d.v.s. graden av asymmetri kring en fördelnings medelvärde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är från 1 till 255 tal, namn, matriser eller referenser som innehåller tal, vars snedhet du vill beräkna."
			},
			{
				name: "tal2",
				description: "är från 1 till 255 tal, namn, matriser eller referenser som innehåller tal, vars snedhet du vill beräkna."
			}
		]
	},
	{
		name: "SNEDHET.P",
		description: "Returnerar snedheten i en fördelning baserat på en population, d.v.s. graden av asymmetri kring en fördelnings medelvärde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är från 1 till 254 tal, namn, matriser eller referenser som innehåller tal som du vill beräkna populationens snedhet för."
			},
			{
				name: "tal2",
				description: "är från 1 till 254 tal, namn, matriser eller referenser som innehåller tal som du vill beräkna populationens snedhet för."
			}
		]
	},
	{
		name: "SÖK",
		description: "Returnerar antalet tecken vilka ett givet tecken eller textsträng söker efter först, läser från höger till vänster (ej skiftlägeskänslig).",
		arguments: [
			{
				name: "sök",
				description: "är texten som du vill hitta. Du kan använda jokertecknen ? och *; använd ~? och ~* om du vill hitta tecknen ? och *."
			},
			{
				name: "inom",
				description: "är texten som du vill söka efter i söktext."
			},
			{
				name: "startpos",
				description: "är antalet tecken i inom_text, från vänster räknat, som du vill starta sökning vid. Om den utelämnas används 1."
			}
		]
	},
	{
		name: "SSVXEKV",
		description: "Returnerar avkastningen motsvarande obligationsräntan för statsskuldväxlar.",
		arguments: [
			{
				name: "betalning",
				description: "är likviddagen för statsskuldväxlar uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är förfallodag för statsskuldväxlar uttryckt som ett datumserienummer"
			},
			{
				name: "ränta",
				description: "är diskonteringsräntan för statsskuldväxlar"
			}
		]
	},
	{
		name: "SSVXPRIS",
		description: "Returnerar priset per 1 000 kr nominellt värde för en statsskuldväxel.",
		arguments: [
			{
				name: "betalning",
				description: "är likviddagen för statsskuldväxlar uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är förfallodag för statsskuldväxlar uttryckt som ett datumserienummer"
			},
			{
				name: "ränta",
				description: "är diskonteringsräntan för statsskuldväxlar"
			}
		]
	},
	{
		name: "SSVXRÄNTA",
		description: "Returnerar avkastningen för en statsskuldväxel.",
		arguments: [
			{
				name: "betalning",
				description: "är likviddagen för statsskuldväxlar uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är förfallodag för statsskuldväxlar uttryckt som ett datumserienummer"
			},
			{
				name: "pris",
				description: "är priset på statsskuldväxlar per 1 000 kr nominellt värde"
			}
		]
	},
	{
		name: "STÄDA",
		description: "Tar bort alla icke-utskrivbara tecken från texten.",
		arguments: [
			{
				name: "text",
				description: "är information från ett kalkylblad som du vill ta bort icke-utskrivbara tecken från."
			}
		]
	},
	{
		name: "STANDARDISERA",
		description: "Returnerar ett normaliserat värde från en fördelning karaktäriserad av medelvärden och standardavvikelser.",
		arguments: [
			{
				name: "x",
				description: "är det värde som du vill normalisera."
			},
			{
				name: "medelvärde",
				description: "är det aritmetiska medelvärdet av fördelningen."
			},
			{
				name: "standardavvikelse",
				description: "är standardavvikelsen för fördelningen, ett positivt tal."
			}
		]
	},
	{
		name: "STDAV",
		description: "Beräkna standardavvikelsen utifrån ett sampel (logiska värden och text i samplet ignoreras).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal som motsvarar ett sampel i en population, och kan vara tal eller referenser som innehåller tal."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal som motsvarar ett sampel i en population, och kan vara tal eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "STDAV.P",
		description: "Beräknar standardavvikelsen baserad på hela populationen angiven som argument (ignorerar logiska värden och text).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal som motsvarar en population och kan vara tal eller referenser som innehåller tal."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal som motsvarar en population och kan vara tal eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "STDAV.S",
		description: "Uppskattar standardavvikelsen baserad på ett sampel (ignorerar logiska värden och text i samplet).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal som motsvarar ett sampel i en population och kan vara tal eller referenser som innehåller tal."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal som motsvarar ett sampel i en population och kan vara tal eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "STDAVP",
		description: "Beräkna standardavvikelsen utifrån hela populationen angiven som argument (logiska värden och text ignoreras).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal som motsvarar en population, och kan vara tal eller referenser som innehåller tal."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal som motsvarar en population, och kan vara tal eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "STDEVA",
		description: "Uppskattar standardavvikelsen baserad på ett sampel inklusive logiska värden och text. Text och det logiska värdet FALSKT har värdet 0; det logiska värdet SANT har värdet 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255 värden som motsvarar ett sampel i en population och kan vara värden, namn eller referenser till värden."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255 värden som motsvarar ett sampel i en population och kan vara värden, namn eller referenser till värden."
			}
		]
	},
	{
		name: "STDEVPA",
		description: "Beräknar standard avvikelsen baserad på hela populationen, inklusive logiska värden och text. Text och det logiska värdet FALSKT har värdet 0; det logiska värdet SANT har värdet 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är 1 till 255 värden som motsvarar en population och kan vara värden, namn, matriser eller referenser som innehåller värden."
			},
			{
				name: "värde2",
				description: "är 1 till 255 värden som motsvarar en population och kan vara värden, namn, matriser eller referenser som innehåller värden."
			}
		]
	},
	{
		name: "STDFELYX",
		description: "Returnerar standardfelet för ett förutspått y-värde för varje x-värde i regressionen.",
		arguments: [
			{
				name: "kända_y",
				description: "är en matris eller ett område med beroende datapunkter och kan vara tal, namn, matriser eller referenser som innehåller tal."
			},
			{
				name: "kända_x",
				description: "är en matris eller ett område med oberoende datapunkter och kan vara tal, namn, matriser eller referenser som innehåller tal."
			}
		]
	},
	{
		name: "STÖRSTA",
		description: "Returnerar det n:te största värdet i en datamängd, t ex det femte största talet.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller området med data som du vill bestämma det n:te största värdet ur."
			},
			{
				name: "n",
				description: "är den position (från det största värdet) i matrisen eller dataområdet som ska returneras."
			}
		]
	},
	{
		name: "SUMMA",
		description: "Adderar samtliga tal i ett cellområde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 tal som ska summeras. Logiska värden och text ignoreras i cellerna, men inkluderas om de skrivs som argument."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 tal som ska summeras. Logiska värden och text ignoreras i cellerna, men inkluderas om de skrivs som argument."
			}
		]
	},
	{
		name: "SUMMA.OM",
		description: "Adderar celler enligt ett angivet villkor.",
		arguments: [
			{
				name: "område",
				description: "är det cellområde som du vill värdera."
			},
			{
				name: "villkor",
				description: "är det villkor i form av ett tal, uttryck eller text som definierar vilka celler som ska adderas."
			},
			{
				name: "summaområde",
				description: "är de celler som ska summeras. Om de utelämnas används cellerna i området."
			}
		]
	},
	{
		name: "SUMMA.OMF",
		description: "Adderar de celler som anges av en given uppsättning villkor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "summaområde",
				description: "är de celler som ska adderas."
			},
			{
				name: "villkorsområde",
				description: "är cellområdet som du vill värdera för det specifika villkoret"
			},
			{
				name: "villkor",
				description: "är villkoret i form av ett tal, uttryck eller text som definierar vilka celler som ska adderas"
			}
		]
	},
	{
		name: "SUMMAX2MY2",
		description: "Summerar skillnaderna mellan kvadraterna i två motsvarande områden eller matriser.",
		arguments: [
			{
				name: "xmatris",
				description: "är den första matrisen eller området med värden och kan vara ett tal eller namn, en matris eller referens som innehåller tal."
			},
			{
				name: "ymatris",
				description: "är den andra matrisen eller området med värden och kan vara ett tal eller namn, en matris eller referens som innehåller tal."
			}
		]
	},
	{
		name: "SUMMAX2PY2",
		description: "Returnerar totalsumman av summan av kvadraterna på tal i två motsvarande områden eller matriser.",
		arguments: [
			{
				name: "xmatris",
				description: "är den första matrisen eller området med värden och kan vara ett tal eller namn, en matris eller referens som innehåller tal."
			},
			{
				name: "ymatris",
				description: "är den andra matrisen eller området med värden och kan vara ett tal eller namn, en matris eller referens som innehåller tal."
			}
		]
	},
	{
		name: "SUMMAXMY2",
		description: "Summerar kvadraten på skillnaderna mellan två motsvarande områden eller matriser.",
		arguments: [
			{
				name: "xmatris",
				description: "är den första matrisen eller området med värden och kan vara ett tal eller namn, en matris eller referens som innehåller tal."
			},
			{
				name: "ymatris",
				description: "är den andra matrisen eller område med värden och kan vara ett tal eller namn, en matris eller referens som innehåller tal."
			}
		]
	},
	{
		name: "T",
		description: "Kontrollerar om ett värde är text och returnerar texten om det är det, annars returneras dubbla citattecken (ingen text).",
		arguments: [
			{
				name: "värde",
				description: "är värdet som ska testas."
			}
		]
	},
	{
		name: "T.FÖRD",
		description: "Returnerar vänstersidig students t-fördelning.",
		arguments: [
			{
				name: "x",
				description: "är det numeriska värdet där du vill beräkna fördelningen."
			},
			{
				name: "frihetsgrader",
				description: "är ett heltal som anger det antal frihetsgrader som karaktäriserar fördelningen."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen."
			}
		]
	},
	{
		name: "T.FÖRD.2T",
		description: "Returnerar tvåsidig students t-fördelning.",
		arguments: [
			{
				name: "x",
				description: "är det numeriska värdet där du vill beräkna fördelningen."
			},
			{
				name: "frihetsgrader",
				description: "är ett heltal som anger antalet frihetsgrader som karaktäriserar fördelningen."
			}
		]
	},
	{
		name: "T.FÖRD.RT",
		description: "Returnerar högersidig students t-fördelning.",
		arguments: [
			{
				name: "x",
				description: "är det numeriska värdet där du vill beräkna fördelningen."
			},
			{
				name: "frihetsgrader",
				description: "är ett heltal som anger antalet frihetsgrader som karaktäriserar fördelningen."
			}
		]
	},
	{
		name: "T.INV",
		description: "Returnerar den vänstersidiga inversen till students t-fördelning.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med students tvåsidiga t-fördelning, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrader",
				description: "är ett positivt heltal som anger antalet frihetsgrader som karaktäriserar fördelningen."
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Returnerar den tvåsidiga inversen till students t-fördelning.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med students tvåsidiga t-fördelning, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrader",
				description: "är ett positivt heltal som anger antalet frihetsgrader som karaktäriserar fördelningen."
			}
		]
	},
	{
		name: "T.TEST",
		description: "Returnerar sannolikheten associerad med students t-test.",
		arguments: [
			{
				name: "matris1",
				description: "är den första datamängden."
			},
			{
				name: "matris2",
				description: "är den andra datamängden."
			},
			{
				name: "sidor",
				description: "anger antalet fördelningssidor som ska returneras: ensidig fördelning = 1; tvåsidig fördelning = 2."
			},
			{
				name: "typ",
				description: "är typen av t-test: parat = 1, två sampel med lika varians (homoscedastisk) = 2, två sampel med olika varians = 3."
			}
		]
	},
	{
		name: "TALVÄRDE",
		description: "Konverterar text till ett tal oberoende av språk.",
		arguments: [
			{
				name: "text",
				description: "är den sträng som innehåller talet du vill konvertera."
			},
			{
				name: "decimaltecken",
				description: "är det tecken som används som decimaltecken i strängen."
			},
			{
				name: "tusentalsavgränsare",
				description: "är det tecken som används som tusentalsavgränsare i strängen."
			}
		]
	},
	{
		name: "TAN",
		description: "Returnerar en vinkels tangent.",
		arguments: [
			{
				name: "tal",
				description: "är den vinkel i radianer som du vill ha tangens för. Grader * PI()/180 = radianer."
			}
		]
	},
	{
		name: "TANH",
		description: "Returnerar hyperbolisk tangens för ett tal.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal."
			}
		]
	},
	{
		name: "TECKEN",
		description: "Returnerar ett tals tecken: 1 om talet är positivt, noll om talet är noll eller -1 om talet är negativt.",
		arguments: [
			{
				name: "tal",
				description: "är ett reellt tal."
			}
		]
	},
	{
		name: "TECKENKOD",
		description: "Returnerar tecknet som anges av koden från datorns teckenuppsättning.",
		arguments: [
			{
				name: "tal",
				description: "är ett tal mellan 1 och 255 som anger vilket tecken du vill ha."
			}
		]
	},
	{
		name: "TEXT",
		description: "Konverterar ett värde till text i ett specifikt talformat.",
		arguments: [
			{
				name: "värde",
				description: "är ett tal, en formel som kan beräknas till ett numerisk värde eller en referens till en cell som innehåller ett numeriskt värde."
			},
			{
				name: "format",
				description: "är ett tal i textformat från Kategori-rutan under Tal-fliken i dialogrutan Formatera celler (inte generellt)."
			}
		]
	},
	{
		name: "TEXTNUM",
		description: "Konverterar en textsträng som representerar ett tal till ett tal.",
		arguments: [
			{
				name: "text",
				description: "är en text inom citattecken eller en referens till en cell som innehåller texten som du vill konvertera."
			}
		]
	},
	{
		name: "TFÖRD",
		description: "Returnera studentens t-fördelning.",
		arguments: [
			{
				name: "x",
				description: "är ett numeriskt värde där du vill beräkna fördelningen."
			},
			{
				name: "frihetsgrader",
				description: "är ett heltal som anger det antal frihetsgrader som karaktäriserar fördelningen."
			},
			{
				name: "sidor",
				description: "anger antalet fördelningssidor som ska returneras: ensidig fördelning = 1; tvåsidig fördelning = 2."
			}
		]
	},
	{
		name: "TIDVÄRDE",
		description: "Konverterar en texttid till ett Spreadsheet-serienummer för tid, ett tal från 0 (00:00:00) till  0,999988426 (23:59:59). Formatera talet med ett tidsformat när du angett formeln.",
		arguments: [
			{
				name: "tidstext",
				description: "är en sträng som anger en tid i något av Spreadsheets tidsformat (datuminformation i strängen ignoreras)."
			}
		]
	},
	{
		name: "TIMME",
		description: "Returnerar timmen som ett tal mellan 0 och 23.",
		arguments: [
			{
				name: "serienummer",
				description: "är numret i datum-tid-koden som Spreadsheet använder eller text i tidsformat, t.ex. 16:48:00."
			}
		]
	},
	{
		name: "TINV",
		description: "Returnera den tvåsidiga inversen till studentens t-fördelning.",
		arguments: [
			{
				name: "sannolikhet",
				description: "är sannolikheten associerad med studentens tvåsidiga t-fördelning, ett tal mellan 0 och 1 inklusiv."
			},
			{
				name: "frihetsgrader",
				description: "är ett positivt heltal som visar antalet frihetsgrader som karaktäriserar fördelningen."
			}
		]
	},
	{
		name: "TOPPIGHET",
		description: "Returnerar en datamängds fördelning.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal, vars fördelning du vill beräkna."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal, vars fördelning du vill beräkna."
			}
		]
	},
	{
		name: "TRANSPONERA",
		description: "Konverterar ett vertikalt cellområde till ett horisontellt och tvärtom.",
		arguments: [
			{
				name: "matris",
				description: "är ett cellområde i ett kalkylblad eller en matris med värden som du vill transponera."
			}
		]
	},
	{
		name: "TREND",
		description: "Returnerar tal i en linjär trend som matchar kända datapunkter, genom att använda minsta kvadrat-metoden.",
		arguments: [
			{
				name: "kända_y",
				description: "är ett område eller en matris med y-värden som är kända i förhållandet y = mx + b."
			},
			{
				name: "kända_x",
				description: "är ett valfritt område eller en matris med x-värden som är kända i förhållandet  y = mx + b, en matris med samma storlek som kända_y."
			},
			{
				name: "nya_x",
				description: "är ett område eller en matris med nya x-värden för vilka du vill att TREND ska returnera motsvarande y-värden."
			},
			{
				name: "konst",
				description: "är ett logiskt värde: konstanten b beräknas normalt om konst = SANT eller utesluten; konstanten b ska vara 0 om konst = FALSKT."
			}
		]
	},
	{
		name: "TRIMMEDEL",
		description: "Returnerar medelvärdet av mittenpunkterna i en mängd data.",
		arguments: [
			{
				name: "matris",
				description: "är matrisen eller området som ska behandlas."
			},
			{
				name: "procent",
				description: "anger hur många datavärden som ska tas bort från var ända."
			}
		]
	},
	{
		name: "TTEST",
		description: "Returnera sannolikheten associerad med en students t-test.",
		arguments: [
			{
				name: "matris1",
				description: "är den första datamängden."
			},
			{
				name: "matris2",
				description: "är den andra datamängden."
			},
			{
				name: "sidor",
				description: "anger antalet fördelningssidor som ska returneras: ensidig fördelning = 1; tvåsidig fördelning = 2."
			},
			{
				name: "typ",
				description: "är typen av t-test: parat = 1, två sampel med samma varians (homoscedastisk) = 2, två sampel med olika varians = 3"
			}
		]
	},
	{
		name: "TYPVÄRDE",
		description: "Returnera det vanligast förekommande eller mest repeterade värdet i en matris eller ett dataområde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal som du vill beräkna typvärdet för."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal som du vill beräkna typvärdet för."
			}
		]
	},
	{
		name: "TYPVÄRDE.ETT",
		description: "Returnerar det vanligast förekommande eller mest repeterade värdet i en matris eller ett dataområde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal vars typvärde du vill beräkna."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal vars typvärde du vill beräkna."
			}
		]
	},
	{
		name: "TYPVÄRDE.FLERA",
		description: "Returnerar en vertikal matris med de vanligast förekommande eller mest repeterade värdena i en matris eller ett dataområde. För en horisontell matris använder du =TRANSPONERA(TYPVÄRDE.FLERA(tal1,tal2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal vars typvärde du vill beräkna."
			},
			{
				name: "tal2",
				description: "är 1 till 255 tal, namn, matriser eller referenser som innehåller tal vars typvärde du vill beräkna."
			}
		]
	},
	{
		name: "UDDA",
		description: "Rundar av ett positivt tal uppåt, och ett negativt nedåt, till närmaste udda heltal.",
		arguments: [
			{
				name: "tal",
				description: "är värdet som du vill avrunda."
			}
		]
	},
	{
		name: "UNICODE",
		description: "Returnerar talet (kodpunkten) som motsvarar det första tecknet i texten.",
		arguments: [
			{
				name: "text",
				description: "är tecknet som du vill ha Unicode-värdet för."
			}
		]
	},
	{
		name: "UPPHÖJT.TILL",
		description: "Returnerar resultatet av ett tal upphöjt till en exponent.",
		arguments: [
			{
				name: "tal",
				description: "är basen, vilket reellt tal som helst."
			},
			{
				name: "exponent",
				description: "är exponenten som basen upphöjs till."
			}
		]
	},
	{
		name: "UPPLOBLRÄNTA",
		description: "Returnerar den upplupna räntan för värdepapper som ger avkastning på förfallodagen.",
		arguments: [
			{
				name: "utgivning",
				description: "är värdepapperets utgivningsdag uttryckt som ett datumserienummer"
			},
			{
				name: "förfall",
				description: "är värdepapperets förfallodag uttryckt som ett datumserienummer"
			},
			{
				name: "kupongränta",
				description: "är värdepapperets årliga kupongränta"
			},
			{
				name: "nominellt",
				description: "är värdepapperets nominella värde"
			},
			{
				name: "bas",
				description: "är bastyp för antal dagar som ska användas"
			}
		]
	},
	{
		name: "VÄLJ",
		description: "Väljer ett värde eller en åtgärd som ska utföras från en lista av värden, baserad på ett indexnummer.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "index",
				description: "anger vilket argument som väljs. Index_num måste vara mellan 1 och 254, en formel eller en referens till ett tal mellan 1 och 254."
			},
			{
				name: "värde1",
				description: "är från 1 till 254 tal, cellreferenser, definierade namn, formler, funktioner eller text argument som VÄLJ väljer från."
			},
			{
				name: "värde2",
				description: "är från 1 till 254 tal, cellreferenser, definierade namn, formler, funktioner eller text argument som VÄLJ väljer från."
			}
		]
	},
	{
		name: "VALUTA",
		description: "Konverterar ett tal till text med valutaformat.",
		arguments: [
			{
				name: "tal",
				description: "är ett tal, en referens till en cell som innehåller ett tal eller en formel som kan beräknas till ett tal."
			},
			{
				name: "decimaler",
				description: "är antalet siffror till höger om decimalkommat. Talet rundas av som det behövs; om det utelämnas, Decimaler = 2."
			}
		]
	},
	{
		name: "VÄNSTER",
		description: "Returnerar det angivna antalet tecken från början av en textsträng.",
		arguments: [
			{
				name: "text",
				description: "är den textsträng som innehåller tecknen du vill ha fram."
			},
			{
				name: "antal_tecken",
				description: "anger hur många tecken du vill att VÄNSTER ska extrahera; 1 om det utelämnas."
			}
		]
	},
	{
		name: "VARA",
		description: "Uppskattar variansen baserad på ett sampel, inklusive logiska värden och text. Text och det logiska värdet FALSKT har värdet 0; det logiska värdet SANT har värdet 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är mellan 1 och 255 värdeargument som motsvarar ett sampel i en population."
			},
			{
				name: "värde2",
				description: "är mellan 1 och 255 värdeargument som motsvarar ett sampel i en population."
			}
		]
	},
	{
		name: "VÄRDETYP",
		description: "Returnerar ett heltal som anger ett värdes datatyp: tal=1; text=2; logiskt värde=4; felvärde=16; matris=64.",
		arguments: [
			{
				name: "värde",
				description: "kan vara valfritt värde"
			}
		]
	},
	{
		name: "VARIANS",
		description: "Beräkna variansen utifrån ett sampel (logiska värden och text i samplet ignoreras).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 numeriska argument som motsvarar ett sampel i en population."
			},
			{
				name: "tal2",
				description: "är 1 till 255 numeriska argument som motsvarar ett sampel i en population."
			}
		]
	},
	{
		name: "VARIANS.P",
		description: "Beräknar variansen baserad på hela populationen (ignorerar logiska värden och text i populationen).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är mellan 1 och 255 numeriska argument som motsvarar en population."
			},
			{
				name: "tal2",
				description: "är mellan 1 och 255 numeriska argument som motsvarar en population."
			}
		]
	},
	{
		name: "VARIANS.S",
		description: "Uppskattar variansen baserad på ett sampel (ignorerar logiska värden och text i samplet).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är från 1 till 255 numeriska argument som motsvarar ett sampel i en population."
			},
			{
				name: "tal2",
				description: "är från 1 till 255 numeriska argument som motsvarar ett sampel i en population."
			}
		]
	},
	{
		name: "VARIANSP",
		description: "Beräkna variansen utifrån hela populationen (logiska värden och text i populationen ignoreras).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "är 1 till 255 numeriska argument som motsvarar en population."
			},
			{
				name: "tal2",
				description: "är 1 till 255 numeriska argument som motsvarar en population."
			}
		]
	},
	{
		name: "VARPA",
		description: "Beräknar variansen baserad på hela populationen, inklusive logiska värden och text. Text och det logiska värdet FALSKT har värdet 0; det logiska värdet SANT har värdet 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "värde1",
				description: "är 1 till 255 argument som motsvarar en population."
			},
			{
				name: "värde2",
				description: "är 1 till 255 argument som motsvarar en population."
			}
		]
	},
	{
		name: "VDEGRAVSKR",
		description: "Returnerar avskrivningen för en tillgång under en angiven period eller del av period genom dubbel degressiv avskrivning eller annan metod som du anger.",
		arguments: [
			{
				name: "kostnad",
				description: "är initialkostnaden för tillgången."
			},
			{
				name: "restvärde",
				description: "är värdet efter avskrivningen."
			},
			{
				name: "livslängd",
				description: "är antalet perioder under den tid då tillgången skrivs av och minskar i värde. (Avskrivningstid)"
			},
			{
				name: "startperiod",
				description: "är startperioden för beräkningen av avskrivningen, i samma enheter som Livslängd."
			},
			{
				name: "slutperiod",
				description: "är slutperioden för beräkningen av avskrivningen, i samma enheter som Livslängd."
			},
			{
				name: "faktor",
				description: "är måttet för hur fort tillgången avskrivs. Om det utelämnas används värdet 2 (dubbel degressiv avskrivning)."
			},
			{
				name: "inget_byte",
				description: "Växla till linjär avskrivning när sådan avskrivning är större än den degressiva avskrivningen = FALSKT eller utelämnad; växla inte = SANT."
			}
		]
	},
	{
		name: "VECKODAG",
		description: "Returnerar ett tal mellan 1 och 7 som identifierar veckodagen för ett datum.",
		arguments: [
			{
				name: "serienummer",
				description: "är ett tal som representerar ett datum."
			},
			{
				name: "returtyp",
				description: "är ett tal: för söndag=1 till lördag=7, använd 1; för måndag=1 till söndag=7, använd 2; för måndag=0 till söndag=6, använd 3."
			}
		]
	},
	{
		name: "VECKONR",
		description: "Omvandlar ett serienummer till ett veckonummer.",
		arguments: [
			{
				name: "tal",
				description: "är datum/tid-koden som används av Spreadsheet för datum- och tidsberäkningar"
			},
			{
				name: "returtyp",
				description: "är ett tal (1, 2 eller 3) som bestämmer typen på returvärdet"
			}
		]
	},
	{
		name: "VERSALER",
		description: "Konverterar text till versaler.",
		arguments: [
			{
				name: "text",
				description: "är texten som du vill konvertera till versaler. Kan vara en referens eller en textsträng."
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Returnera Weibull-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett icke-negativt tal."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen för massa."
			}
		]
	},
	{
		name: "WEIBULL.FÖRD",
		description: "Returnerar Weibull-fördelningen.",
		arguments: [
			{
				name: "x",
				description: "är värdet där du vill beräkna funktionen, ett icke-negativt tal."
			},
			{
				name: "alfa",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "beta",
				description: "är en parameter till fördelningen, ett positivt tal."
			},
			{
				name: "kumulativ",
				description: "är ett logiskt värde: använd SANT för den kumulativa fördelningsfunktionen och FALSKT för sannolikhetsfunktionen för massa."
			}
		]
	},
	{
		name: "XELLER",
		description: "Returnerar ett logiskt 'Exklusivt eller' för alla argument.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "är 1 till 254 villkor du vill testa som kan ha värdet SANT eller FALSKT och kan vara logiska värden, matriser eller referenser."
			},
			{
				name: "logisk2",
				description: "är 1 till 254 villkor du vill testa som kan ha värdet SANT eller FALSKT och kan vara logiska värden, matriser eller referenser."
			}
		]
	},
	{
		name: "XIRR",
		description: "Returnerar internräntan för ett schema över betalningsströmmar som inte nödvändigtvis är periodiska.",
		arguments: [
			{
				name: "värden",
				description: "är en serie betalningsströmmar som motsvarar ett schema över betalningsdatum"
			},
			{
				name: "datum",
				description: "är ett schema över betalningsdatum som motsvarar betalningsströmmarna"
			},
			{
				name: "gissning",
				description: "är det tal du antar ligger nära resultatet av XIRR"
			}
		]
	},
	{
		name: "XNUVÄRDE",
		description: "Returnerar det diskonterade nuvärdet för ett schema över betalningsströmmar som inte nödvändigtvis är periodiska.",
		arguments: [
			{
				name: "ränta",
				description: "är diskonteringsräntan som gäller för betalningsströmmarna"
			},
			{
				name: "värden",
				description: "är en serie betalningsströmmar som motsvarar ett schema över betalningsdatum"
			},
			{
				name: "datum",
				description: "är ett schema över betalningsdatum som motsvarar betalningsströmmarna"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Returnerar det ensidiga P-värdet av en z-test.",
		arguments: [
			{
				name: "matris",
				description: "är den matris eller det område med data som x ska testas mot."
			},
			{
				name: "x",
				description: "är värdet som ska testas."
			},
			{
				name: "sigma",
				description: "är den kända populations standardavvikelse. Om den utelämnas kommer sampelstandardavvikelsen att användas."
			}
		]
	},
	{
		name: "ZTEST",
		description: "Returnera det ensidiga P-värdet av en z-test.",
		arguments: [
			{
				name: "matris",
				description: "är den matris eller det område med data som x ska testas mot."
			},
			{
				name: "x",
				description: "är värdet som ska testas."
			},
			{
				name: "sigma",
				description: "är känd populations standardavvikelse. Om den utelämnas används sampelstandardavvikelsen."
			}
		]
	}
];