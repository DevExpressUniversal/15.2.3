ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Returnerer den absolutte værdi af et tal, dvs. tallet uden fortegn.",
		arguments: [
			{
				name: "tal",
				description: "er det reelle tal, som du vil have oplyst den absolutte værdi af"
			}
		]
	},
	{
		name: "ADRESSE",
		description: "Opretter en cellereference som tekst ud fra angivne række- og kolonnenumre.",
		arguments: [
			{
				name: "række",
				description: "er det rækkenummer, der skal bruges i cellereferencen: Rækkenummer = 1 for række 1"
			},
			{
				name: "kolonne",
				description: "er det kolonnenummer, der skal bruges i cellereferencen. Kolonnenummeret er f.eks.  4 for kolonne D"
			},
			{
				name: "abs_nr",
				description: "specificerer referencetypen: absolut = 1; absolut række/relativ kolonne = 2; relativ række/absolut kolonne = 3; relativ = 4"
			},
			{
				name: "a1",
				description: "er en logisk værdi, der angiver  referencemåden: type A1 = 1 eller SAND; type R1C1 = 0 eller FALSK"
			},
			{
				name: "arknavn",
				description: "er den tekst, der angiver navnet på det regneark, der skal bruges som ekstern reference"
			}
		]
	},
	{
		name: "AFKAST.DISKONTO",
		description: "Returnerer det årlige afkast for et diskonteret værdipapir, f.eks. en statsobligation.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "kurs",
				description: "er værdipapirets kurs pr. kr. 100 i pålydende værdi"
			},
			{
				name: "indløsningskurs",
				description: "er værdipapirets indløsningskurs pr. kr. 100 i pålydende værdi"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "AFKORT",
		description: "Afkorter et tal til et heltal ved at fjerne decimal- eller procentdelen af tallet.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, der skal afkortes"
			},
			{
				name: "antal_cifre",
				description: "er et tal, der angiver, hvor nøjagtig afkortelsen skal være. Sættes til 0 (nul), hvis intet angives"
			}
		]
	},
	{
		name: "AFRUND",
		description: "Afrunder et tal til et angivet antal decimaler.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, der skal afrundes"
			},
			{
				name: "antal_cifre",
				description: "er det antal decimaler, der skal afrundes til. Negative tal afrundes til venstre for kommaet, nul til det nærmeste heltal"
			}
		]
	},
	{
		name: "AFRUND.BUND.MAT",
		description: "Runder et tal ned til det nærmeste heltal eller til det nærmeste betydende multiplum.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, du vil afrunde"
			},
			{
				name: "betydning",
				description: "er det multiplum, du vil afrunde til"
			},
			{
				name: "tilstand",
				description: "hvis dette angives, og værdien ikke er nul, afrundes der væk fra nul"
			}
		]
	},
	{
		name: "AFRUND.GULV",
		description: "Runder et tal ned til det nærmeste multiplum af betydning.",
		arguments: [
			{
				name: "tal",
				description: "er den numeriske værdi, der skal afrundes"
			},
			{
				name: "betydning",
				description: "er det multiplum, der skal afrundes til. Tal og signifikans skal enten begge være positive eller begge være negative"
			}
		]
	},
	{
		name: "AFRUND.GULV.PRECISE",
		description: "Runder et tal ned til det nærmeste heltal eller det nærmeste multiplum af betydning.",
		arguments: [
			{
				name: "tal",
				description: "er den numeriske værdi, der skal afrundes"
			},
			{
				name: "betydning",
				description: "er det multiplum, der skal afrundes til. "
			}
		]
	},
	{
		name: "AFRUND.LOFT",
		description: "Runder et tal op til nærmeste multiplum af betydning.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal afrundes"
			},
			{
				name: "betydning",
				description: "er det multiplum, der skal afrundes til"
			}
		]
	},
	{
		name: "AKKUM.HOVEDSTOL",
		description: "Returnerer den akkumulerede hovedstol, betalt på et lån mellem to perioder.",
		arguments: [
			{
				name: "rente",
				description: "er rentefoden"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder"
			},
			{
				name: "nv",
				description: "er nutidsværdien"
			},
			{
				name: "startperiode",
				description: "er den første periode i beregningen"
			},
			{
				name: "slutperiode",
				description: "er den sidste periode i beregningen"
			},
			{
				name: "betalingstype",
				description: "er tidspunktet for ydelsen"
			}
		]
	},
	{
		name: "AKKUM.RENTE",
		description: "Returnerer den akkumulerede rente, betalt mellem to perioder.",
		arguments: [
			{
				name: "rente",
				description: "er rentefoden"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder"
			},
			{
				name: "nv",
				description: "er nutidsværdien"
			},
			{
				name: "startperiode",
				description: "er den første periode i beregningen"
			},
			{
				name: "slutperiode",
				description: "er den sidste periode i beregningen"
			},
			{
				name: "betalingstype",
				description: "er tidspunktet for ydelsen"
			}
		]
	},
	{
		name: "ANTAL.ARBEJDSDAGE",
		description: "Returnerer antal hele arbejdsdage mellem to datoer.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, der angiver startdatoen"
			},
			{
				name: "slutdato",
				description: "er et serielt datotal, der angiver slutdatoen"
			},
			{
				name: "feriedage",
				description: "er et valgfrit antal serielle datotal, der skal udelades fra beregningen, f.eks. helligdage"
			}
		]
	},
	{
		name: "ANTAL.ARBEJDSDAGE.INTL",
		description: "Returnerer antallet af hele arbejdsdage mellem to datoer med brugerdefinerede weekendparametre.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, som repræsenterer startdatoen"
			},
			{
				name: "slutdato",
				description: "er et serielt datotal, som repræsenterer slutdatoen"
			},
			{
				name: "weekend",
				description: "er et tal eller en streng, der angiver, hvornår weekender forekommer"
			},
			{
				name: "feriedage",
				description: "er et valgfrit sæt af et eller flere serielle datotal, der skal udelades fra arbejdskalenderen, som f.eks. offentlige helligdage og flydende helligdage"
			}
		]
	},
	{
		name: "ANTAL.BLANKE",
		description: "Tæller antallet af tomme celler i et angivet område.",
		arguments: [
			{
				name: "område",
				description: "er det område, hvor du vil have oplyst antallet af tomme celler"
			}
		]
	},
	{
		name: "ÅR",
		description: "Returnerer året i en dato, et heltal mellem 1900 og 9999.",
		arguments: [
			{
				name: "serienr",
				description: "er et tal i den dato- og klokkeslætskode, der anvendes af Spreadsheet"
			}
		]
	},
	{
		name: "ÅR.BRØK",
		description: "Returnerer årsbrøken, der repræsenterer antal hele dage mellem startdato og slutdato.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, der angiver startdatoen"
			},
			{
				name: "slutdato",
				description: "er et serielt datotal, der angiver slutdatoen"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "ARABISK",
		description: "Konverterer et romertal til arabisk.",
		arguments: [
			{
				name: "tekst",
				description: "er det romertal, du vil konvertere"
			}
		]
	},
	{
		name: "ARBEJDSDAG",
		description: "Returnerer det serielle datotal for dagen før eller efter et specifikt antal arbejdsdage.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, der angiver startdatoen"
			},
			{
				name: "dage",
				description: "er antal ikke-weekender og ikke-feriedage før eller efter startdatoen"
			},
			{
				name: "feriedage",
				description: "er en valgfri matrix af et eller flere serielle datotal, der skal udelades fra beregningen, f.eks. helligdage"
			}
		]
	},
	{
		name: "ARBEJDSDAG.INTL",
		description: "Returnerer det serielle datotal for dagen før eller efter det angivne antal arbejdsdage med brugerdefinerede  weekendparametre.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, som repræsenterer startdatoen"
			},
			{
				name: "dage",
				description: "er et tal eller en streng, der angiver, hvornår det er weekend"
			},
			{
				name: "weekend",
				description: "angiver de ugedage, der er weekenddage. FIX ME"
			},
			{
				name: "feriedage",
				description: "er en valgfri matrix af et eller flere serielle datotal, der skal udelades af arbejdskalenderen, som f.eks. offentlige helligdage og flydende helligdage"
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Returnerer arcus cosinus til et tal, i radianer i intervallet 0 til pi. Arcus cosinus er den vinkel, hvis cosinus er Tal.",
		arguments: [
			{
				name: "tal",
				description: "er den ønskede vinkels cosinus og skal være et tal mellem -1 og 1"
			}
		]
	},
	{
		name: "ARCCOSH",
		description: "Returnerer den inverse hyperbolske cosinus til et tal.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal lig med eller større end 1"
			}
		]
	},
	{
		name: "ARCCOT",
		description: "Returnerer arcus cotangens til et tal i radianer i intervallet 0 til Pi.",
		arguments: [
			{
				name: "tal",
				description: "er cotangens til den ønskede vinkel"
			}
		]
	},
	{
		name: "ARCCOTH",
		description: "Returnerer den inverse hyperbolske cotangens af et tal.",
		arguments: [
			{
				name: "tal",
				description: "er den hyperbolske cotangens af den ønskede vinkel"
			}
		]
	},
	{
		name: "ARCSIN",
		description: "Returnerer arcus sinus til et tal, i radianer i intervallet -pi/2 til pi/2.",
		arguments: [
			{
				name: "tal",
				description: "er den ønskede vinkels sinus og skal være et tal mellem -1 og 1"
			}
		]
	},
	{
		name: "ARCSINH",
		description: "Returnerer den inverse hyperbolske sinus til et tal.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal lig med eller større end 1"
			}
		]
	},
	{
		name: "ARCTAN",
		description: "Returnerer arcus tangens til et tal, i radianer i intervallet -pi/2 til pi/2.",
		arguments: [
			{
				name: "tal",
				description: "er tangens for den ønskede vinkel"
			}
		]
	},
	{
		name: "ARCTAN2",
		description: "Returnerer de specificerede  x- og y-koordinaters arcus tangens, i radianer mellem -pi og pi, forskellig fra -pi.",
		arguments: [
			{
				name: "x_koordinat",
				description: "er punktets x-koordinat"
			},
			{
				name: "y_koordinat",
				description: "er punktets y-koordinat"
			}
		]
	},
	{
		name: "ARCTANH",
		description: "Returnerer den inverse hyperbolske tangens til et tal.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal mellem -1 og 1, forskelligt fra -1 og 1"
			}
		]
	},
	{
		name: "ARK",
		description: "Returnerer arknummeret for det ark, der refereres til.",
		arguments: [
			{
				name: "værdi",
				description: "er navnet på et ark eller en reference, du vil have arknummeret for. Hvis dette udelades, returneres det antal ark, som indeholder funktionen"
			}
		]
	},
	{
		name: "ARK.FLERE",
		description: "Returnerer antal ark i en reference.",
		arguments: [
			{
				name: "reference",
				description: "er en reference, hvor du vil vide, hvor mange ark den indeholder. Hvis dette udelades, returneres det antal ark i projektmappen, som indeholder funktionen"
			}
		]
	},
	{
		name: "ÅRSAFSKRIVNING",
		description: "Returnerer den årlige afskrivning på et aktiv i en bestemt periode.",
		arguments: [
			{
				name: "købspris",
				description: "er aktivets kostpris"
			},
			{
				name: "restværdi",
				description: "er aktivets værdi ved afskrivningens afslutning"
			},
			{
				name: "levetid",
				description: "er antallet af afskrivningsperioder (aktivets levetid)"
			},
			{
				name: "periode",
				description: "er perioden. Periode skal anvende samme enheder som leveid"
			}
		]
	},
	{
		name: "BAHTTEKST",
		description: "Konverterer et tal til tekst (baht).",
		arguments: [
			{
				name: "tal",
				description: "er et tal, som du vil konvertere"
			}
		]
	},
	{
		name: "BASIS",
		description: "Konverterer et tal til en tekstrepræsentation med en given radikand (rod).",
		arguments: [
			{
				name: "tal",
				description: "er det tal, du vil konvertere"
			},
			{
				name: "radikand",
				description: "er den rodradikand, du vil konvertere tallet til"
			},
			{
				name: "min_længde",
				description: "er minimumlængden for den returnerede streng. Hvis denne udelades, tilføjes der ikke foranstillede nuller"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Returnerer den modificerede Bessel-funktion In(x).",
		arguments: [
			{
				name: "x",
				description: "er den værdi, hvor funktionen skal evalueres"
			},
			{
				name: "n",
				description: "er rækkefølgen for Bessel-funktionen"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Returner Bessel-funktionen Jn(x).",
		arguments: [
			{
				name: "x",
				description: "er den værdi, hvor funktionen skal evalueres"
			},
			{
				name: "n",
				description: "er rækkefølgen for Bessel-funktionen"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Returnerer den modificerede Bessel-funktion Kn(x).",
		arguments: [
			{
				name: "x",
				description: "er den værdi, hvor funktionen skal evalueres"
			},
			{
				name: "n",
				description: "er rækkefølgen for funktionen"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Returner Bessel-funktionen Yn(x).",
		arguments: [
			{
				name: "x",
				description: "er den værdi, hvor funktionen skal evalueres"
			},
			{
				name: "n",
				description: "er rækkefølgen for funktionen"
			}
		]
	},
	{
		name: "BETA.FORDELING",
		description: "Returnerer fordelingsfunktionen for betafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi i intervallet fra A til B, som funktionen skal evalueres for"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis SAND returneres fordelingsfunktionen. Hvis FALSK returneres punktsandsynligheden"
			},
			{
				name: "A",
				description: "er en valgfri nedre grænse i intervallet for x. Hvis grænsen ikke angives, sættes A = 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grænse i intervallet for x. Hvis grænsen ikke angives, sættes B= 0"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Returnerer den inverse fordelingsfunktion for betafordelingen (BETA.FORDELING).",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden knyttet til betafordelingen"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "A",
				description: "er en valgfri nedre grænse i intervallet for x. Hvis grænsen ikke angives, sættes A = 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grænse i intervallet for x. Hvis grænsen ikke angives, sættes B= 1"
			}
		]
	},
	{
		name: "BETAFORDELING",
		description: "Returnerer fordelingsfunktionen for betafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi i intervallet fra A til B, som funktionen skal evalueres for"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "A",
				description: "er en valgfri nedre grænse i intervallet for x. Hvis grænsen ikke angives, sættes A = 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grænse i intervallet for x. Hvis grænsen ikke angives, sættes B= 0"
			}
		]
	},
	{
		name: "BETAINV",
		description: "Returnerer den inverse fordelingsfunktion for betafordelingen (BETADIST).",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden knyttet til betafordelingen"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen, som skal være større end 0"
			},
			{
				name: "A",
				description: "er en valgfri nedre grænse i intervallet for x. Hvis grænsen ikke angives, sættes A = 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grænse i intervallet for x. Hvis grænsen ikke angives, sættes B= 0"
			}
		]
	},
	{
		name: "BIN.TIL.DEC",
		description: "Konverterer et binært tal til et decimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det binære tal, der skal konverteres"
			}
		]
	},
	{
		name: "BIN.TIL.HEX",
		description: "Konvertér et binært tal til et hexadecimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det binære tal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "BIN.TIL.OKT",
		description: "Konvertér et binært tal til et oktaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det binære tal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "BINOMIAL.DIST.INTERVAL",
		description: "Returnerer sandsynlighed for et forsøgsresultat ved hjælp af binomial fordeling.",
		arguments: [
			{
				name: "forsøg",
				description: "er antal uafhængige forsøg"
			},
			{
				name: "sandsynlighed_s",
				description: "er sandsynligheden for succes for hvert forsøg"
			},
			{
				name: "tal_s",
				description: "er antal vellykkede forsøg"
			},
			{
				name: "tal_s2",
				description: "hvis angivet, vil funktionen returnere sandsynligheden for, at det vellykkede antal forsøg er mellem tal_s og tal_s2"
			}
		]
	},
	{
		name: "BINOMIAL.FORDELING",
		description: "Returnerer punktsandsynligheden for binomialfordelingen.",
		arguments: [
			{
				name: "tal_s",
				description: "er antallet af gunstige udfald af forsøgene"
			},
			{
				name: "forsøg",
				description: "er antallet af uafhængige forsøg"
			},
			{
				name: "sandsynligheder",
				description: "er sandsynligheden for gunstigt udfald af hvert forsøg"
			},
			{
				name: "akkumuleret",
				description: "er en logisk værdi. Hvis SAND returneres fordelingsfunktionen. Hvis FALSK returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "BINOMIAL.INV",
		description: "Returnerer den mindste værdi, for hvilken den akkumulerede binomialfordeling er større end eller lig med en kriterieværdi.",
		arguments: [
			{
				name: "forsøg",
				description: "er antallet af Bernoulli-forsøg"
			},
			{
				name: "sandsynligheder",
				description: "er sandsynligheden for et gunstigt udfald af hvert forsøg, som er et tal mellem 0 og 1 (inklusive) "
			},
			{
				name: "alpha",
				description: "er kriterieværdien, som er et tal mellem 0 og 1 (inklusive)"
			}
		]
	},
	{
		name: "BINOMIALFORDELING",
		description: "Returnerer punktsandsynligheden for binomialfordelingen.",
		arguments: [
			{
				name: "tal_s",
				description: "er antallet af gunstige udfald af forsøgene"
			},
			{
				name: "forsøg",
				description: "er antallet af uafhængige forsøg"
			},
			{
				name: "sandsynlighed_s",
				description: "er sandsynligheden for et gunstigt udfald"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis SAND returneres fordelingsfunktionen. Hvis FALSK returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "BITELLER",
		description: "Returnerer et bitbaseret 'Eller' af to tal.",
		arguments: [
			{
				name: "tal1",
				description: "er den decimale repræsentation af det binære tal, du vil evaluere"
			},
			{
				name: "tal2",
				description: "er den decimale repræsentation af det binære tal, du vil evaluere"
			}
		]
	},
	{
		name: "BITLSKIFT",
		description: "Returnerer et tal forskudt til venstre med forskydning bit.",
		arguments: [
			{
				name: "tal",
				description: "er decimaltallet af det binære tal, som du vil evaluere"
			},
			{
				name: "forskydning",
				description: "er det antal bit, du vil forskyde tal til venstre med"
			}
		]
	},
	{
		name: "BITOG",
		description: "Returnerer et bitbaseret 'Og' af to tal.",
		arguments: [
			{
				name: "tal1",
				description: "er den decimale repræsentation af det binære tal, du vil evaluere"
			},
			{
				name: "tal2",
				description: "er den decimale repræsentation af det binære tal, du vil evaluere"
			}
		]
	},
	{
		name: "BITRSKIFT",
		description: "Returnerer et tal forskudt til højre med forskydning bit.",
		arguments: [
			{
				name: "tal",
				description: "er decimaltallet af det binære tal, som du vil evaluere"
			},
			{
				name: "forskydning",
				description: "er det antal bit, du vil forskyde tal til højre med"
			}
		]
	},
	{
		name: "BITXELLER",
		description: "Returnerer et bitbaseret 'Eksklusivt eller' af to tal.",
		arguments: [
			{
				name: "tal1",
				description: "er den decimale repræsentation af det binære tal, du vil evaluere"
			},
			{
				name: "tal2",
				description: "er den decimale repræsentation af det binære tal, du vil evaluere"
			}
		]
	},
	{
		name: "CELLE",
		description: "Returnerer oplysninger om formatering, placering eller indhold af den første referencecelle i arkets læserækkefølge.",
		arguments: [
			{
				name: "infotype",
				description: "er en tekstværdi, der angiver, hvilken type celleoplysninger der ønskes."
			},
			{
				name: "reference",
				description: "er den celle, du vil have oplysninger om"
			}
		]
	},
	{
		name: "CHAR",
		description: "Returnerer det tegn fra computerens tegnsæt, som kodenummeret angiver.",
		arguments: [
			{
				name: "tal",
				description: "er et tal mellem 1 og 255, som angiver, hvilket tegn der ønskes"
			}
		]
	},
	{
		name: "CHI2.FORD.RT",
		description: "Returnerer fraktilsandsynligheden for en chi2-fordeling.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), du vil evaluere distributionen for"
			},
			{
				name: "frihedsgrader",
				description: "er antallet af frihedsgrader- et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "CHI2.FORDELING",
		description: "Returnerer den venstresidede sandsynlighed for en chi2-fordeling.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), du vil evaluere distributionen for"
			},
			{
				name: "frihedsgrader",
				description: "er antallet af frihedsgrader- et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer tæthedsfunktionen"
			}
		]
	},
	{
		name: "CHI2.INV",
		description: "Returnerer den inverse venstresidede sandsynlighed for en chi2-fordeling.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden knyttet til chi2-fordelingen. Det er en værdi mellem 0 og 1"
			},
			{
				name: "frihedsgrader",
				description: "er antallet af frihedsgrader- et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "CHI2.INV.RT",
		description: "Returnerer den inverse fraktilsandsynlighed for chi2-fordelingen.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden knyttet til chi2-fordelingen. Det er en værdi mellem 0 og 1"
			},
			{
				name: "frihedsgrader",
				description: "er antallet af frihedsgrader- et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "CHI2.TEST",
		description: "Returnerer testen for uafhængighed, dvs. værdien fra chi2- fordelingen for den statistiske og den passende uafhængighed.",
		arguments: [
			{
				name: "observeret_værdi",
				description: "er det dataområde, som indeholder de observerede værdier, der skal testes mod de forventede værdier"
			},
			{
				name: "forventet_værdi",
				description: "er det dataområde, der indeholder produktet af rækketotalerne og kolonnetotalerne i forhold til hovedtotalen"
			}
		]
	},
	{
		name: "CHIFORDELING",
		description: "Returnerer fraktilsandsynligheden for en chi2-fordeling.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), du vil evaluere distributionen for"
			},
			{
				name: "frihedsgrader",
				description: "er antallet af frihedsgrader - et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "CHIINV",
		description: "Returnerer den inverse chi2-fordeling.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden knyttet til chi2-fordelingen. Det er en værdi mellem 0 og 1"
			},
			{
				name: "frihedsgrader",
				description: "er antallet af frihedsgrader - et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "CHITEST",
		description: "Returnerer testen for uafhængighed, dvs. værdien fra chi2- fordelingen for den statistiske og den passende uafhængighed.",
		arguments: [
			{
				name: "observeret_værdi",
				description: "er det dataområde, som indeholder de observerede værdier, der skal testes mod de forventede værdier"
			},
			{
				name: "forventet_værdi",
				description: "er det dataområde, der indeholder produktet af rækketotalerne og kolonnetotalerne i forhold til hovedtotalen"
			}
		]
	},
	{
		name: "COS",
		description: "Returnerer cosinus til en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, som du vil have cosinus for"
			}
		]
	},
	{
		name: "COSH",
		description: "Returnerer den hyperbolske cosinus til et tal.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal"
			}
		]
	},
	{
		name: "COT",
		description: "Returnerer cotangens af en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, du vil have cotangens af"
			}
		]
	},
	{
		name: "COTH",
		description: "Returnerer den hyperbolske cotagens af et tal.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, du vil have den hyperbolske cotangens af"
			}
		]
	},
	{
		name: "CSC",
		description: "Returnerer cosekanten af en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, du vil have cosekanten af"
			}
		]
	},
	{
		name: "CSCH",
		description: "Returnerer den hyperbolske cosekant af en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, du vil have den hyperbolske cosekant for"
			}
		]
	},
	{
		name: "DAG",
		description: "Returnerer dagen i måneden, et tal mellem 1 og 31.",
		arguments: [
			{
				name: "serienr",
				description: "er et tal i den dato- og klokkeslætskode, der anvendes af Spreadsheet"
			}
		]
	},
	{
		name: "DAGE",
		description: "Returnerer antal dage mellem de to datoer.",
		arguments: [
			{
				name: "slutdato",
				description: "startdato og slutdato er de to datoer, du vil finde antal dage mellem"
			},
			{
				name: "startdato",
				description: "startdato og slutdato er de to datoer, du vil finde antal dage mellem"
			}
		]
	},
	{
		name: "DAGE360",
		description: "Beregner antallet af dage mellem to datoer på baggrund af et år på 360 dage (12 måneder à 30 dage).",
		arguments: [
			{
				name: "startdato",
				description: "startdato og slutdato er de to datoer, som du vil have oplyst antallet af dage imellem"
			},
			{
				name: "slutdato",
				description: "startdato og slutdato er de to datoer, som du vil have oplyst antallet af dage imellem"
			},
			{
				name: "metode",
				description: "er en logisk værdi, der angiver beregningsmetoden: U.S. (NASD) = FALSK eller udeladt; Europæisk = SAND."
			}
		]
	},
	{
		name: "DATO",
		description: "Returnerer det tal, der repræsenterer datoen i Spreadsheets dato- og klokkeslætskode.",
		arguments: [
			{
				name: "år",
				description: "er et tal mellem 1900 og 9999 i Spreadsheet til Windows eller mellem 1904 og 9999 i Spreadsheet til Macintosh"
			},
			{
				name: "måned",
				description: "er et tal mellem 1 og 12, der repræsenterer måneden i året"
			},
			{
				name: "dag",
				description: "er et tal mellem 1 og 31, der repræsenterer dagen i måneden"
			}
		]
	},
	{
		name: "DATO.FORSKEL",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATOVÆRDI",
		description: "Konverterer en dato i form af tekst til et tal, der repræsenterer datoen i Spreadsheets dato- og klokkeslætskode.",
		arguments: [
			{
				name: "datotekst",
				description: "er tekst, der repræsenterer en dato i et Spreadsheet-datoformat mellem 1/1/1900 (Windows) eller 1/1/1904 (Macintosh) og 12/31/9999"
			}
		]
	},
	{
		name: "DB",
		description: "Returnerer afskrivningsbeløbet for et aktiv for en given periode vha. saldometoden.",
		arguments: [
			{
				name: "købspris",
				description: "er aktivets kostpris"
			},
			{
				name: "restværdi",
				description: "er aktivets værdi ved afskrivningens afslutning"
			},
			{
				name: "levetid",
				description: "angiver antallet af afskrivningsperioder, som aktivet afskrives over (aktivets levetid)"
			},
			{
				name: "periode",
				description: "er den periode, afskrivningen skal beregnes for. Periode skal anvende samme enheder som levetid"
			},
			{
				name: "måned",
				description: "er antallet af måneder det første år. Hvis måned udelades, forudsættes værdien at være 12"
			}
		]
	},
	{
		name: "DEC.TIL.BIN",
		description: "Konverterer et decimaltal til et binært tal.",
		arguments: [
			{
				name: "tal",
				description: "er det decimalheltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "DEC.TIL.HEX",
		description: "Konverterer et decimaltal til et hexadecimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det decimalheltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "DEC.TIL.OKT",
		description: "Konverterer et decimaltal til et oktaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det decimalheltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "DECIMAL",
		description: "Konverterer tekstrepræsentationen af et tal i en given rod til et decimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, du vil konvertere"
			},
			{
				name: "radikand",
				description: "er rodradikanden for det tal, du konverterer"
			}
		]
	},
	{
		name: "DELTA",
		description: "Undersøger, om to værdier er ens.",
		arguments: [
			{
				name: "tal1",
				description: "er det første tal"
			},
			{
				name: "tal2",
				description: "er det andet tal"
			}
		]
	},
	{
		name: "DHENT",
		description: "Uddrager en enkelt post fra en database, der opfylder de angivne betingelser.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, listen eller databasen består af. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnernes placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver celleområdet med de kriterier, du angiver. Området omfatter et kolonnenavnet og en celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DISKONTO",
		description: "Returnerer et værdipapirs diskonto.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "kurs",
				description: "er værdipapirets kurs pr. kr. 100 i pålydende værdi"
			},
			{
				name: "indløsningskurs",
				description: "er værdipapirets indløsningskurs pr. kr. 100 i pålydende værdi"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "DMAKS",
		description: "Returnerer den største værdi i feltet (kolonnen) med dokumenter i den database, der svarer til de kriterier, du angiver.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet i dobbelte anførseltegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver et celleområde, der indeholder de kriterier, du angiver. Området indeholder et kolonnenavn og én celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DMIDDEL",
		description: "Beregner gennemsnittet af værdierne i en kolonne på en liste eller i en database, der svarer til de betingelser, du angiver.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, listen eller databasen består af. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "er enten kolonnenavnene med dobbelte anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "er det celleområde, der indeholder de kriterier, du angiver. Området omfatter et kolonnenavn og en celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DMIN",
		description: "Returnerer den mindste værdi blandt markerede databaseposter, der svarer til de kriterier, du angiver.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver et celleområde, der indeholder de kriterier, du angiver. Celleområdet indeholder et kolonnenavn og én celle under navnet på et krierium"
			}
		]
	},
	{
		name: "DOBBELT.FAKULTET",
		description: "Returnerer et tals dobbelte fakultet.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, for hvilken det dobbelte fakultet skal returneres"
			}
		]
	},
	{
		name: "DPRODUKT",
		description: "Multiplicerer værdierne i feltet (kolonnen) med poster i databasen, der opfylder de betingelser, du har angivet.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver celleområdet med de kriterier, du angiver. Området omfatter et kolonnenavnet og en celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DSA",
		description: "Returnerer afskrivningsbeløbet for et aktiv for en given periode vha. dobbeltsaldometoden eller en anden angivet afskrivningsmetode.",
		arguments: [
			{
				name: "købspris",
				description: "er aktivets kostpris"
			},
			{
				name: "restværdi",
				description: "er aktivets værdi ved afskrivningens afslutning"
			},
			{
				name: "levetid",
				description: "angiver antallet af perioder, aktivet afskrives over (aktivets levetid)"
			},
			{
				name: "periode",
				description: "er den periode, afskrivningen skal beregnes for. Periode skal anvende samme enheder som levetid"
			},
			{
				name: "faktor",
				description: "angiver den sats, som saldoen falder med. Hvis faktor udelades, forudsættes den at være 2 (dobbeltsaldometoden)"
			}
		]
	},
	{
		name: "DSTDAFV",
		description: "Beregner et skøn over standardafvigelsen baseret på en stikprøve af markerede databaseposter.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver celleområdet med de kriterier, du angiver. Området omfatter et kolonnenavn og en celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DSTDAFVP",
		description: "Beregner standardafvigelsen baseret på hele populationen af markerede databaseposter.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "Angiver kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver celleområdet med de kriterier, du specificerer. Området omfatter et kolonnenavn og én celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DSUM",
		description: "Lægger de tal i feltet (kolonnen) med poster i databasen, der opfylder de angivne betingelser sammen.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver et celleområde med de kriterier, du angiver. Området omfatter et kolonnenavn og én celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DTÆL",
		description: "Tæller de celler, der indeholder tal, i feltet (kolonnen) med dokumenter i den database, der svarer til de angivne kriterier.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "er enten kolonnenavnene i dobbelte anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "er celleområdet, er indeholder de kriterier, som du angiver. Området omfatter et kolonnenavn"
			}
		]
	},
	{
		name: "DTÆLV",
		description: "Tæller udfyldte celler i feltet (kolonnen) med dokumenter databasen i den database, der svarer til de kriterier, du angiver.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, listen eller databasen består af. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet sat i dobbelt anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver et celleområde, der indeholder de kriterier, du angiver. Området omfatter et kolonnenavn og én celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DVARIANS",
		description: "Beregner et skøn over variansen baseret på en stikprøve af markerede databaseposter.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "angiver enten kolonnenavnet i dobbelt anførselstegn eller et tal, der repræsenterer kolonnernes placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver et celleområde med de kriterier, du angiver. Området omfatter et kolonnenavn og én celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "DVARIANSP",
		description: "Beregner varians baseret på hele populationen af markerede databaseposter.",
		arguments: [
			{
				name: "database",
				description: "er det celleområde, der udgør listen eller databasen. En database er en liste over beslægtede data"
			},
			{
				name: "felt",
				description: "er enten kolonnenavnene i dobbelte anførselstegn eller et tal, der repræsenterer kolonnens placering på listen"
			},
			{
				name: "kriterier",
				description: "angiver et celleområde med de kriterier, du angiver. Området omfatter et kolonnenavn og en celle under navnet på et kriterium"
			}
		]
	},
	{
		name: "EDATO",
		description: "Returnerer serienummeret for datoen, der er det angivne antal måneder før eller efter startdatoen.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, der repræsenterer startdatoen"
			},
			{
				name: "måneder",
				description: "er antal måneder før eller efter startdatoen"
			}
		]
	},
	{
		name: "EFFEKTIV.RENTE",
		description: "Returnerer den årlige effektive rente.",
		arguments: [
			{
				name: "nominel_rente",
				description: "er den nominelle rente"
			},
			{
				name: "nperår",
				description: "er antal sammensatte perioder pr. år"
			}
		]
	},
	{
		name: "EKSAKT",
		description: "Undersøger, om to tekststrenge er helt identiske, og returnerer SAND eller FALSK. EKSAKT skelner mellem store og små bogstaver.",
		arguments: [
			{
				name: "tekst1",
				description: "er den første tekststreng"
			},
			{
				name: "tekst2",
				description: "er den anden tekststreng"
			}
		]
	},
	{
		name: "EKSP",
		description: "Returnerer e opløftet til en potens af et givet tal.",
		arguments: [
			{
				name: "tal",
				description: "er den eksponent, der anvendes sammen med grundtalle e. Konstanten e er lig med 2,71828182845904., den naturlige logaritmes grundtal"
			}
		]
	},
	{
		name: "EKSP.FORDELING",
		description: "Returnerer eksponentialfordelingen.",
		arguments: [
			{
				name: "x",
				description: "er funktionens værdi (et ikke-negativt tal)"
			},
			{
				name: "lambda",
				description: "er parameterværdien (et positivt tal)"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer tæthedsfunktionen"
			}
		]
	},
	{
		name: "EKSPFORDELING",
		description: "Returnerer fordelingsfunktionen for eksponentialfordelingen.",
		arguments: [
			{
				name: "x",
				description: "er funktionens værdi (et ikke-negativt tal)"
			},
			{
				name: "lambda",
				description: "er parameterværdien (et positivt tal)"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer tæthedsfunktionen"
			}
		]
	},
	{
		name: "ELLER",
		description: "Undersøger, om nogle af argumenterne er SAND, og returnerer SAND eller FALSK. Returnerer kun FALSK, hvis alle argumenter er FALSK.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "er 1-255 betingelser, du vil teste, som kan være enten SAND eller FALSK"
			},
			{
				name: "logisk2",
				description: "er 1-255 betingelser, du vil teste, som kan være enten SAND eller FALSK"
			}
		]
	},
	{
		name: "ER.FEJL",
		description: "Undersøger, om en værdi er en fejl (#I/T, #VÆRDI!, #REFERENCE!, #DIVISION/0!, #NUM!, #NAVN? eller #NULL!), og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet. Værdien kan referere til en celle, en formel eller til et navn, der refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.FJL",
		description: "Undersøger, om en værdi er en fejl (#VÆRDI!, #REFERENCE!, #DIVISION/0!, #NUM!, #NAVN?, eller #NUL!) undtagen #I/T, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der skal testes. Værdien kan referere til en celle, en formel eller til et navn, der refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.FORMEL",
		description: "Kontrollerer, om en reference er til en celle, der indeholder en formel, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "reference",
				description: "er en reference til den celle, du vil kontrollere. Referencen kan være en cellereference, en formel eller et navn, der refererer til en celle"
			}
		]
	},
	{
		name: "ER.IKKE.TEKST",
		description: "Undersøger, om en værdi ikke er tekst (tomme celler er ikke tekst), og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet: En celle, en formel eller et navn, som refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.IKKE.TILGÆNGELIG",
		description: "Undersøger, om en værdi er #I/T, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet. Værdien kan referere til en celle, en formel eller til et navn, som refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.LIGE",
		description: "Returnerer SAND, hvis tallet er lige.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal testes"
			}
		]
	},
	{
		name: "ER.LOGISK",
		description: "Undersøger, om en værdi er en logisk værdi (SAND eller FALSK), og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet. Værdien kan referere til en celle, en formel eller til et navn, som refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.REFERENCE",
		description: "Undersøger, om en værdi er en reference, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet. Værdien kan referere til en celle, en formel eller til et navn, som refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.TAL",
		description: "Undersøger om en værdi er et tal, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet. Værdien kan referere til en celle, en formel eller til et navn, som refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.TEKST",
		description: "Undersøger, om en værdi er tekst, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der ønskes testet. Værdien kan referere til en celle, en formel eller til et navn, som refererer til en celle, formel eller værdi"
			}
		]
	},
	{
		name: "ER.TOM",
		description: "Undersøger, om en reference refererer til en tom celle, og returnerer SAND eller FALSK.",
		arguments: [
			{
				name: "værdi",
				description: "er den celle, eller et navn, der refererer til den celle, der ønskes testet"
			}
		]
	},
	{
		name: "ER.ULIGE",
		description: "Returnerer SAND, hvis tallet er ulige.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal testes"
			}
		]
	},
	{
		name: "ERF.PRECISE",
		description: "Returnerer fejlfunktionen.",
		arguments: [
			{
				name: "X",
				description: "er den nedre grænse for integrering af ERF.PRECISE"
			}
		]
	},
	{
		name: "ERFC.PRECISE",
		description: "Returnerer den komplementære fejlfunktion.",
		arguments: [
			{
				name: "X",
				description: "er den nedre grænse for integrering af ERFC.PRECISE"
			}
		]
	},
	{
		name: "ERSTAT",
		description: "Erstatter en del af en tekststreng med en anden tekststreng.",
		arguments: [
			{
				name: "gammel_tekst",
				description: "er den tekst, hvor der skal udskiftes et antal tegn"
			},
			{
				name: "start_ved",
				description: "er positionen for det tegn i gammel_tekst, der skal erstattes med ny_tekst"
			},
			{
				name: "antal_tegn",
				description: "er det antal tegn i gammel_tekst, der skal erstattes"
			},
			{
				name: "ny_tekst",
				description: "er den tekst, der skal erstatte tegn i gammel_tekst"
			}
		]
	},
	{
		name: "F.FORDELING",
		description: "Returnerer fraktilsandsynligheden for F-fordelingen (afvigelsesgraden) for to datasæt.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), funktionen skal evalueres for"
			},
			{
				name: "frihedsgrader1",
				description: "er frihedsgrader til tælleren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "frihedsgrader2",
				description: "er frihedsgrader til nævneren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer tæthedsfunktionen"
			}
		]
	},
	{
		name: "F.FORDELING.RT",
		description: "Returnerer fraktilsandsynligheden for F-fordelingen (afvigelsesgraden) for to datasæt.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), funktionen skal evalueres for"
			},
			{
				name: "frihedsgrader1",
				description: "er frihedsgrader til tælleren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "frihedsgrader2",
				description: "er frihedsgrader til nævneren - et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Returnerer den inverse fraktilsandsynlighed for F-fordelingen. Hvis p = F.FORDELING(x,...), så er F.FORDELING(p,...) = x.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til F-fordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "frihedsgrader1",
				description: "er frihedsgrader til tælleren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "frihedsgrader2",
				description: "er frihedsgrader til nævneren - et tal mellem 1 og 10^10"
			}
		]
	},
	{
		name: "F.INV.RT",
		description: "Returnerer den inverse fraktilsandsynlighed for F-fordeling. Hvis p = F.FORDELING.RT(x,...), så er FINV.RT(p,...) = x.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til F-fordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "frihedsgrader1",
				description: "er frihedsgrader til tælleren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "frihedsgrader2",
				description: "er frihedsgrader til nævneren - et tal mellem 1 og 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Returnerer resultatet af en F-test, dvs. den tosidige sandsynlighed for at varianserne til Matrix1 og Matrix2 ikke er tydeligt forskellige.",
		arguments: [
			{
				name: "matrix1",
				description: "er den første matrix eller det første dataområde. Det kan bestå af tal, navne, matrixer eller referencer, der indeholder tal (blanktegn ignoreres)"
			},
			{
				name: "matrix2",
				description: "er den anden matrix eller det andet dataområde. Det kan bestå af tal, navne, matrix eller referencer, der indeholder tal (blanktegn ignoreres)"
			}
		]
	},
	{
		name: "FAKULTET",
		description: "Returnerer et tals fakultet, svarende til 1*2*3*...* Tal.",
		arguments: [
			{
				name: "tal",
				description: "er det positive tal, du vil beregne fakultetet af"
			}
		]
	},
	{
		name: "FALSK",
		description: "Returnerer den logiske værdi FALSK.",
		arguments: [
		]
	},
	{
		name: "FAST",
		description: "afrunder et tal til det angivne antal decimaler og returnerer resultatet som tekst med eller uden kommaer.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, du vil afrunde og konvertere til tekst"
			},
			{
				name: "decimaler",
				description: "er antallet af cifre til højre for decimaltegnet. Hvis feltet ikke udfyldes, sættes Decimaler = 2"
			},
			{
				name: "ingen_punktummer",
				description: "er en logisk værdi: Vis ikke kommaerne i den returnerede tekst = SAND; vis ikke kommaerne i den returnerede tekst = FALSK eller udelades"
			}
		]
	},
	{
		name: "FEJLFUNK",
		description: "Returnerer fejlfunktionen.",
		arguments: [
			{
				name: "nedre_grænse",
				description: "er nederste grænse for integrering af FEJLFUNK"
			},
			{
				name: "øvre_grænse",
				description: "er øverste grænse for integrering af FEJLFUNK"
			}
		]
	},
	{
		name: "FEJLFUNK.KOMP",
		description: "Returnerer den komplementære fejlfunktion.",
		arguments: [
			{
				name: "x",
				description: "er nederste grænse for integrering af FEJLFUNK"
			}
		]
	},
	{
		name: "FEJLTYPE",
		description: "Returnerer et tal, der svarer til en fejlværdi.",
		arguments: [
			{
				name: "fejlværdi",
				description: "er den fejlværdi, som du vil have id-nummeret til, og kan være en faktisk fejlværdi eller en reference til en celle, der indeholder en fejlværdi"
			}
		]
	},
	{
		name: "FFORDELING",
		description: "Returnerer fraktilsandsynligheden for F-fordelingen (afvigelsesgraden) for to datasæt.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), funktionen skal evalueres for"
			},
			{
				name: "frihedsgrader1",
				description: "er frihedsgrader til tælleren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "frihedsgrader2",
				description: "er frihedsgrader til nævneren - et tal mellem 1 og 10^10, bortset fra 10^10"
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
		description: "Returnerer startpositionen for en tekststreng i en anden tekststreng. FIND skelner mellem store og små bogstaver.",
		arguments: [
			{
				name: "find_tekst",
				description: "er den tekst, der skal findes. Brug dobbelte anførselstegn (tom tekst) for at opfylde kriteriet for det første tegn i I_tekst; jokertegn er ikke tilladt"
			},
			{
				name: "i_tekst",
				description: "er den tekst, der indeholder den tekst, der skal findes"
			},
			{
				name: "start_ved",
				description: "angiver det tegn, som søgningen starter med. Første tegn i I_teksten er tegn nummer 1. Hvis det udelades, Start_ved = 1"
			}
		]
	},
	{
		name: "FINV",
		description: "Returnerer den inverse F-fordeling: hvis p = FFORDELING(x;...), så FFORDELING(p;...) = x.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til F-fordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "frihedsgrader1",
				description: "er frihedsgrader til tælleren - et tal mellem 1 og 10^10, bortset fra 10^10"
			},
			{
				name: "frihedsgrader2",
				description: "er frihedsgrader til nævneren - et tal mellem 1 og 10^10, bortset fra 10^10"
			}
		]
	},
	{
		name: "FISHER",
		description: "Returnerer Fisher-transformationen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi, transformationen skal udføres på. Det er et tal mellem -1 og 1, bortset fra -1 og 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Returnerer den inverse Fisher-transformation: hvis y = FISHER(x), så FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "er den værdi, som den inverse Fisher-transformation skal udføres på"
			}
		]
	},
	{
		name: "FJERN.OVERFLØDIGE.BLANKE",
		description: "Fjerner alle mellemrum fra en tekststreng, undtagen enkeltmellemrum mellem ord.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst, som mellemrum skal fjernes fra"
			}
		]
	},
	{
		name: "FORKLARINGSGRAD",
		description: "Returnerer kvadratet på Pearsons korrelationskoefficient gennem de givne datapunkter.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er en matrix eller et interval af datapunkter, som kan være tal, navne, lister eller referencer, der indeholder tal"
			},
			{
				name: "kendte_x'er",
				description: "er en matrix eller et interval af datapunkter, som kan være tal, navne, lister eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "FORMELTEKST",
		description: "Returnerer en formel som en streng.",
		arguments: [
			{
				name: "reference",
				description: "er en reference til en formel"
			}
		]
	},
	{
		name: "FORØGELSE",
		description: "Returnerer tal i en eksponentiel væksttendens svarende til kendte datapunkter.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er mængden af kendte y-værdier i forholdet y = b*m^x, en matrix eller et område med positive tal"
			},
			{
				name: "kendte_x'er",
				description: "er en mængde x-værdier, som muligvis er kendte, i forholdet y = b*m^x, en matrix eller et område med samme størrelse som Kendte_y'er"
			},
			{
				name: "nye_x'er",
				description: "er nye x-værdier, som FORØGELSE skal returnere tilsvarende y-værdier for"
			},
			{
				name: "konstant",
				description: "angiver en logisk værdi: Konstanten b beregnes normalt, hvis Konst = SAND; b sættes til lig 1, hvis Konst = FALSK eller udelades"
			}
		]
	},
	{
		name: "FORSKYDNING",
		description: "Returnerer en reference til et område, der er et givet antal rækker og kolonner fra en given reference.",
		arguments: [
			{
				name: "reference",
				description: "er den reference, som forskydningen, en cellereference eller et celleområde skal baseres på"
			},
			{
				name: "rækker",
				description: "er det antal rækker i opad- eller nedadgående retning, som den øverste venstre celle skal referere til"
			},
			{
				name: "kolonner",
				description: "er det antal kolonner til venstre eller højre, som resultatets øverste venstre celle skal referere til"
			},
			{
				name: "højde",
				description: "er den ønskede højde i antal rækker for den returnerede reference. Den samme højde som Reference, hvis den udelades"
			},
			{
				name: "bredde",
				description: "er den ønskede bredde i antal kolonner for den returnerede reference. Den samme bredde som Reference, hvis den udelades"
			}
		]
	},
	{
		name: "FORTEGN",
		description: "Returnerer et tals fortegn: 1, hvis tallet er positivt, nul, hvis tallet er nul og -1, hvis tallet er negativt.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal"
			}
		]
	},
	{
		name: "FRAKTIL",
		description: "Returnerer den k'te fraktil for værdier i et interval.",
		arguments: [
			{
				name: "vektor",
				description: "er den vektor eller det datainterval, som definerer den relative status"
			},
			{
				name: "k",
				description: "er en fraktilværdi i intervallet 0 til 1"
			}
		]
	},
	{
		name: "FRAKTIL.MEDTAG",
		description: "Returnerer den k'te fraktil for værdier i et interval, hvor k ligger i intervallet fra 0 til og med 1.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det datainterval, som definerer den relative status"
			},
			{
				name: "k",
				description: "er en fraktilværdi i intervallet fra 0 til og med 1"
			}
		]
	},
	{
		name: "FRAKTIL.UDELAD",
		description: "Returnerer den k'te fraktil for værdier i et interval, hvor k ligger i intervallet fra 0 til 1.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det datainterval, som definerer den relative status"
			},
			{
				name: "k",
				description: "er en fraktilværdi i intervallet fra 0 til og med 1"
			}
		]
	},
	{
		name: "FREKVENS",
		description: "Beregner hvor ofte en værdi forekommer indenfor et værdiområde og returnerer en lodret matrix af tal, der har ét element mere end Intervalmatrix.",
		arguments: [
			{
				name: "datavektor",
				description: "er en matrix eller en reference til et sæt af data, som du vil beregne hyppigheden af (blanktegn og tekst ignoreres)"
			},
			{
				name: "intervalvektor",
				description: "er en matrix af intervaller eller en reference til intervaller,  hvor værdierne fra datamatrix skal grupperes"
			}
		]
	},
	{
		name: "FTEST",
		description: "Returnerer resultatet af en F-test, dvs. den tosidige sandsynlighed for at varianserne til Matrix1 og Matrix2 ikke er tydeligt forskellige.",
		arguments: [
			{
				name: "matriks1",
				description: "er den første matrix eller det første dataområde. Det kan bestå af tal, navne, matrixer eller referencer, der indeholder tal (blanktegn ignoreres)"
			},
			{
				name: "matriks2",
				description: "er den anden matrix eller det andet dataområde. Det kan bestå af tal, navne, matrix eller referencer, der indeholder tal (blanktegn ignoreres)"
			}
		]
	},
	{
		name: "FV",
		description: "Returnerer fremtidig værdi af en investering på basis af periodiske, konstante ydelser og en konstant renteydelse.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder i investeringen"
			},
			{
				name: "ydelse",
				description: "er ydelsen i hver periode. Ydelsen kan ikke ændres, så længe investeringen løber"
			},
			{
				name: "nv",
				description: "er nutidsværdien eller den samlede værdi, som en række fremtidige ydelser er værd nu. Hvis den udelades, er Pv = 0"
			},
			{
				name: "type",
				description: "er en værdi, der angiver, hvornår ydelserne forfalder: ydelse i begyndelsen af perioden =1; ydelse i slutningen af perioden = 0 eller udeladt"
			}
		]
	},
	{
		name: "FVTABEL",
		description: "Returnerer den fremtidige værdi af en hovedstol efter at have anvendt en række sammensatte renter.",
		arguments: [
			{
				name: "hovedstol",
				description: "er nutidsværdien"
			},
			{
				name: "tabel",
				description: "er en matrix af rentesatser, der skal anvendes"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Returnerer gammafunktionsværdien.",
		arguments: [
			{
				name: "x",
				description: "er den værdi, du vil beregne gamma for"
			}
		]
	},
	{
		name: "GAMMA.FORDELING",
		description: "Returnerer gammafordelingen.",
		arguments: [
			{
				name: "x",
				description: "ier den værdi (et ikke-negativt tal), du vil evaluere distributionen for"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen (et positivt tal). Hvis beta = 1, returnerer GAMMA.FORDELING standardgammafordelingen"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer sandsynlighedsfunktionen"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Returnerer den inverse fordelingsfunktion for gammafordelingen: if p = GAMMA.FORDELING(x,...), then GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til gammafordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "beta",
				description: "er en parameter (et positivt tal) til fordelingen. Hvis beta = 1, returnerer GAMMA.INV den inverse standardgammafordeling"
			}
		]
	},
	{
		name: "GAMMAFORDELING",
		description: "Returnerer gammafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), du vil evaluere distributionen for"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen (et positivt tal). Hvis beta = 1, returnerer GAMMADIST standardgammafordelingen"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer sandsynlighedsfunktionen"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Returnerer den inverse fordelingsfunktion for gammafordelingen: if p = GAMMAFORDELING(x,...), then GAMMAFORDELING(p,...) = x.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til gammafordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "beta",
				description: "er en parameter (et positivt tal) til fordelingen. Hvis beta = 1, returnerer GAMMAFORDELING den inverse standardgammafordeling"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Returnerer den naturlige logaritme til gammafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et positivt tal), du vil beregne funktionen GAMMALN for"
			}
		]
	},
	{
		name: "GAMMALN.PRECISE",
		description: "Returnerer den naturlige logaritme til gammafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et positivt tal), du vil beregne funktionen GAMMALN.PRECISE for"
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
		name: "GENTAG",
		description: "Gentager tekst et givet antal gange. Brug REPT til at fylde en celle med et antal forekomster af en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst, der skal gentages"
			},
			{
				name: "antal_gange",
				description: "er et positivt tal, der specificerer det antal gange, tekst skal gentages"
			}
		]
	},
	{
		name: "GEOMIDDELVÆRDI",
		description: "Returnerer den geometriske middelværdi af en matrix eller et område med positive numeriske data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som middelværdien skal beregnes for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som middelværdien skal beregnes for"
			}
		]
	},
	{
		name: "GETPIVOTDATA",
		description: "Uddrager data, der er gemt i en pivottabel.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "datafelt",
				description: "er navnet på det datafelt, som dataene uddrages fra"
			},
			{
				name: "pivottabel",
				description: "er en reference til en celle eller et celleområde i pivottabellen med de data, du vil hente"
			},
			{
				name: "felt",
				description: "felt, der refereres til"
			},
			{
				name: "element",
				description: "feltelement, der refereres til"
			}
		]
	},
	{
		name: "GETRIN",
		description: "Undersøger, om et tal er større end en tærskelværdi.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der sammenlignes med trin"
			},
			{
				name: "trin",
				description: "er tærskelværdien"
			}
		]
	},
	{
		name: "GRADER",
		description: "Konverterer radianer til grader.",
		arguments: [
			{
				name: "vinkel",
				description: "er den vinkel i radianer, der skal konverteres"
			}
		]
	},
	{
		name: "H.YDELSE",
		description: "Returnerer afdragsdelen på ydelsen for en given investering baseret på konstante periodiske ydelser og en konstant rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "periode",
				description: "angiver perioden og skal være mellem 1 og nper"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder i en investering"
			},
			{
				name: "nv",
				description: "er nutidsværdien, dvs. den samlede værdi, som en række fremtidige ydelser er værd nu"
			},
			{
				name: "fv",
				description: "er den fremtidige værdi eller den kassebalance, der ønskes opnået, når den sidste ydelse er betalt"
			},
			{
				name: "type",
				description: "er en logisk værdi: ydelse i begyndelsen af perioden = 1; ydelse i slutningen af perioden = 0 eller udeladt"
			}
		]
	},
	{
		name: "HARMIDDELVÆRDI",
		description: "Returnerer den harmoniske middelværdi af et datasæt bestående af positive tal, dvs. det reciprokke tal til middelværdien af de reciprokke værdier.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som den harmoniske middelværdi skal beregnes for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som den harmoniske middelværdi skal beregnes for"
			}
		]
	},
	{
		name: "HELTAL",
		description: "Runder et tal ned til nærmeste heltal.",
		arguments: [
			{
				name: "tal",
				description: "er det reelle tal, du vil runde ned til et heltal"
			}
		]
	},
	{
		name: "HEX.TIL.BIN",
		description: "Konverterer et hexadecimaltal til et binært tal.",
		arguments: [
			{
				name: "tal",
				description: "er det hexadecimaltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "HEX.TIL.DEC",
		description: "Konverterer et hexadecimaltal til et decimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det hexadecimaltal, der skal konverteres"
			}
		]
	},
	{
		name: "HEX.TIL.OKT",
		description: "Konverterer et hexadecimaltal til et oktaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det hexadecimaltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "HØJRE",
		description: "Returnerer det angivne antal tegn fra slutningen af en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekststreng, der indeholder de tegn, der skal uddrages"
			},
			{
				name: "antal_tegn",
				description: "angiver, hvor mange tegn der skal uddrages. Sættes til 1, hvis feltet ikke udfyldes"
			}
		]
	},
	{
		name: "HVIS",
		description: "Undersøger, om et kriterium er opfyldt, og returnerer en værdi, hvis SAND, og en anden værdi, hvis FALSK.",
		arguments: [
			{
				name: "logisk_test",
				description: "er en vilkårlig værdi eller et vilkårligt udtryk, der kan evalueres som SAND eller FALSK"
			},
			{
				name: "værdi_hvis_sand",
				description: "er den værdi, der returneres, hvis logisk_test er SAND. Hvis intet angives, returneres SAND. Du kan indlejre op til syv HVIS-funktioner"
			},
			{
				name: "værdi_hvis_falsk",
				description: "er den værdi, der returneres, hvis logisk_test er FALSK. Hvis intet angives, returneres FALSK"
			}
		]
	},
	{
		name: "HVIS.FEJL",
		description: "Returnerer udtrykket værdi_hvis_fejl, hvis udtrykket er en fejl, og returnerer ellers værdien af selve udtrykket.",
		arguments: [
			{
				name: "værdi",
				description: "er en vilkårlig værdi et vilkårligt udtryk eller en vilkårlig reference"
			},
			{
				name: "værdi_hvis_fejl",
				description: "er en vilkårlig værdi et vilkårligt udtryk eller en vilkårlig reference"
			}
		]
	},
	{
		name: "HVISIT",
		description: "Returnerer den angivne værdi, hvis udtrykket evalueres til #I/T. Ellers returneres resultatet af udtrykket.",
		arguments: [
			{
				name: "værdi",
				description: "er en værdi, et udtryk eller en reference"
			},
			{
				name: "værdi_hvis_it",
				description: "er en værdi, et udtryk eller en reference"
			}
		]
	},
	{
		name: "HYPGEO.FORDELING",
		description: "Returnerer punktsandsynligheden i en hypergeometrisk fordeling.",
		arguments: [
			{
				name: "udfald_s",
				description: "er antal gunstige udfald i stikprøven"
			},
			{
				name: "størrelse",
				description: "er stikprøvestørrelsen"
			},
			{
				name: "population_s",
				description: "er antal gunstige udfald i populationen"
			},
			{
				name: "populationsstørrelse",
				description: "er populationsstørrelsen"
			},
			{
				name: "kumulativ",
				description: "er en logisk værd. Hvis SAND, returneres fordelingsfunktionen. Hvis FALSK, returneres sandsynlighedsfunktionen"
			}
		]
	},
	{
		name: "HYPGEOFORDELING",
		description: "Returnerer punktsandsynligheden i en hypergeometrisk fordeling.",
		arguments: [
			{
				name: "udfald_s",
				description: "er antal gunstige udfald i stikprøven"
			},
			{
				name: "størrelse",
				description: "er stikprøvestørrelsen"
			},
			{
				name: "population_s",
				description: "er antal gunstige i populationen"
			},
			{
				name: "populationsstørrelse",
				description: "er populationens størrelse"
			}
		]
	},
	{
		name: "HYPPIGST",
		description: "Returnerer den hyppigst forekommende værdi i en matrix eller et datainterval.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som du vil finde modus for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som du vil finde modus for"
			}
		]
	},
	{
		name: "HYPPIGST.ENKELT",
		description: "Returnerer den hyppigst forekommende værdi i en matrix eller et datainterval.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 ta, navne, matrixer eller referencer, der indeholder tal, som du vil finde modus for"
			},
			{
				name: "tal2",
				description: "er 1-255 ta, navne, matrixer eller referencer, der indeholder tal, som du vil finde modus for"
			}
		]
	},
	{
		name: "HYPPIGST.FLERE",
		description: "Returnerer en lodret matrix med de hyppigst forekommende værdier i en matrix eller et datainterval. Brug = TRANSPOSE(MODE.MULT(tal1,tal2,...)) til en vandret matrix.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som du vil finde modus for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som du vil finde modus for"
			}
		]
	},
	{
		name: "IA",
		description: "Returnerer det interne afkast for en række pengestrømme.",
		arguments: [
			{
				name: "værdier",
				description: "er en matrix eller en reference til celler, der indeholder tal, som det interne afkast skal beregnes for"
			},
			{
				name: "gæt",
				description: "er et tal, der skønnes at ligge tæt på resultatet af IA-beregningen. Sættes til 0,1 (10 procent), hvis feltet ikke udfyldes"
			}
		]
	},
	{
		name: "IDAG",
		description: "Returnerer dags dato formateret som en dato.",
		arguments: [
		]
	},
	{
		name: "IKKE",
		description: "Ændrer FALSK til SAND eller SAND til FALSK.",
		arguments: [
			{
				name: "logisk",
				description: "er en værdi eller et udtryk, der kan evalueres som SAND eller FALSK"
			}
		]
	},
	{
		name: "IKKE.TILGÆNGELIG",
		description: "Returnerer fejlværdien #I/T.",
		arguments: [
		]
	},
	{
		name: "IMAGABS",
		description: "Returnerer den absolutte værdi (modulus) af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket den absolutte værdi ønskes"
			}
		]
	},
	{
		name: "IMAGARGUMENT",
		description: "Returnerer argumentet q udtrykt i radianer.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket argumentet ønskes"
			}
		]
	},
	{
		name: "IMAGCOS",
		description: "Returnerer et komplekst tals cosinus.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket cosinus ønskes"
			}
		]
	},
	{
		name: "IMAGCOSH",
		description: "Returnerer den hyperbolske cosinus af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil finde den hyperbolske cosinus for"
			}
		]
	},
	{
		name: "IMAGCOT",
		description: "Returnerer cotangens af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil have cotangens af"
			}
		]
	},
	{
		name: "IMAGCSC",
		description: "Returnerer cosekanten af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil have cosekanten af"
			}
		]
	},
	{
		name: "IMAGCSCH",
		description: "Returnerer den hyperbolske cosekant af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil have den hyperbolske cosekant af"
			}
		]
	},
	{
		name: "IMAGDIV",
		description: "Returnerer kvotienten af to komplekse tal.",
		arguments: [
			{
				name: "ital1",
				description: "er den komplekse tæller eller dividend"
			},
			{
				name: "ital2",
				description: "er den komplekse nævner eller divisor"
			}
		]
	},
	{
		name: "IMAGEKSP",
		description: "Returnerer et komplekst tals eksponentialfunktion.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket eksponentialfunktionen ønskes"
			}
		]
	},
	{
		name: "IMAGINÆR",
		description: "Returnerer den imaginære koefficient af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket den imaginære koefficient ønskes"
			}
		]
	},
	{
		name: "IMAGKONJUGERE",
		description: "Returnerer den komplekst konjugerede af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket den konjugerede ønskes"
			}
		]
	},
	{
		name: "IMAGKVROD",
		description: "Returnerer kvadratroden af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket kvadratroden ønskes"
			}
		]
	},
	{
		name: "IMAGLN",
		description: "Returnerer et komplekst tals naturlige logaritme.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket den naturlige logaritme ønskes"
			}
		]
	},
	{
		name: "IMAGLOG10",
		description: "Returnerer et komplekst tals 10-tals logaritme.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket 10-tals logaritmen ønskes"
			}
		]
	},
	{
		name: "IMAGLOG2",
		description: "Returnerer et komplekst tals 2-tals logaritme.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket 2-tals logaritmen ønskes"
			}
		]
	},
	{
		name: "IMAGPOTENS",
		description: "Returnerer et komplekst tal opløftet i en heltalspotens.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, der ønskes opløftet i potens"
			},
			{
				name: "tal",
				description: "er potensen, som det komplekse tal skal opløftes i"
			}
		]
	},
	{
		name: "IMAGPRODUKT",
		description: "Returnerer produktet af 1 til 255 komplekse tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ital1",
				description: "Ital1, Ital2,... er 1 til 255 komplekse tal, der skal multipliceres."
			},
			{
				name: "ital2",
				description: "Ital1, Ital2,... er 1 til 255 komplekse tal, der skal multipliceres."
			}
		]
	},
	{
		name: "IMAGREELT",
		description: "Returnerer den reelle koefficient af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket den reelle koefficient ønskes"
			}
		]
	},
	{
		name: "IMAGSEC",
		description: "Returnerer sekanten af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil have sekanten af"
			}
		]
	},
	{
		name: "IMAGSECH",
		description: "Returnerer den hyperbolske sekant af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil have den hyperbolske sekant af"
			}
		]
	},
	{
		name: "IMAGSIN",
		description: "Returnerer et komplekst tals sinus.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, for hvilket sinus ønskes"
			}
		]
	},
	{
		name: "IMAGSINH",
		description: "Returnerer den hyperbolske sinus af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil finde den hyperbolske sinus for"
			}
		]
	},
	{
		name: "IMAGSUB",
		description: "Returnerer forskellen mellem 2 komplekse tal.",
		arguments: [
			{
				name: "ital1",
				description: "er det komplekse tal, hvorfra ital2 skal trækkes fra"
			},
			{
				name: "ital2",
				description: "er det komplekse tal, der skal trækkes fra ital1"
			}
		]
	},
	{
		name: "IMAGSUM",
		description: "Returnerer summen af komplekse tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "ital1",
				description: "er 1 til 255 komplekse tal, der skal tilføjes"
			},
			{
				name: "ital2",
				description: "er 1 til 255 komplekse tal, der skal tilføjes"
			}
		]
	},
	{
		name: "IMAGTAN",
		description: "Returnerer tangens af et komplekst tal.",
		arguments: [
			{
				name: "ital",
				description: "er et komplekst tal, du vil have tangens af"
			}
		]
	},
	{
		name: "INDEKS",
		description: "Returnerer en værdi fra eller reference til en celle ved skæringspunktet mellem en bestemt række og kolonne i et givet område.",
		arguments: [
			{
				name: "matrix",
				description: "er et celleområde eller en matrixkonstant."
			},
			{
				name: "række",
				description: "markerer den række i matrixen eller referencen, hvorfra der skal returneres en værdi. Hvis der ikke markeres en række, skal kolonnenummeret angives"
			},
			{
				name: "kolonne",
				description: "markerer den kolonne i matrixen eller referencen, hvorfra der skal returneres en værdi. Hvis der ikke markeres en kolonne, skal rækkenummeret angives"
			}
		]
	},
	{
		name: "INDIREKTE",
		description: "Returnerer den reference, der specificeres af en tekststreng.",
		arguments: [
			{
				name: "reference",
				description: "er en reference til en celle, som indeholder en reference af typen A1- eller R1C1, et navn defineret som en reference eller en tekstreference til en celle"
			},
			{
				name: "a1",
				description: "er en logisk værdi, der specificerer referencetypen i Ref_tekst: R1C1-type = FALSE; A1-type = TRUE eller udeladt"
			}
		]
	},
	{
		name: "INFO",
		description: "Returnerer oplysninger om det aktuelle operativmiljø.",
		arguments: [
			{
				name: "type",
				description: "er tekst, der angiver hvilken type oplysninger der skal returneres."
			}
		]
	},
	{
		name: "INTERN.RENTE",
		description: "Returnerer den interne rente for en pengestrømsplan.",
		arguments: [
			{
				name: "værdier",
				description: "er en serie pengestrømme, der stemmer overens med en betalingsplan med datoer"
			},
			{
				name: "datoer",
				description: "er en plan over betalingsdatoer, der stemmer overens med pengestrømsbetalingerne"
			},
			{
				name: "gæt",
				description: "er et tal, som, du gætter på, er tæt på resultatet af INTERN.RENTE"
			}
		]
	},
	{
		name: "ISO.LOFT",
		description: "Runder et tal op til det nærmeste heltal eller til nærmeste multiplum af betydning.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal afrundes"
			},
			{
				name: "betydning",
				description: "er det multiplum, der skal afrundes til"
			}
		]
	},
	{
		name: "ISOUGE.NR",
		description: "Returnerer tallet for ISO-ugenummeret for årstallet i en given dato.",
		arguments: [
			{
				name: "dato",
				description: "er den dato/klokkeslæt-kode, der bruges til beregning af dato og klokkeslæt i Spreadsheet"
			}
		]
	},
	{
		name: "ISPMT",
		description: "Returnerer den rente, der er betalt i en angivet investeringsperiode.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "periode",
				description: "er den periode, for hvilken renten ønskes beregnet"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder i en investering"
			},
			{
				name: "nv",
				description: "er den samlede værdi, som en række fremtidige ydelser er værd nu"
			}
		]
	},
	{
		name: "KODE",
		description: "Returnerer en numerisk kode fra computerens tegnsæt for det første tegn i en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst, hvor du vil have oplyst koden for det første tegn"
			}
		]
	},
	{
		name: "KODNINGSURL",
		description: "Returnerer en URL-kodet streng.",
		arguments: [
			{
				name: "tekst",
				description: "er en streng, der skal URL-kodes"
			}
		]
	},
	{
		name: "KOLONNE",
		description: "Returnerer kolonnenummeret på en reference.",
		arguments: [
			{
				name: "reference",
				description: "er den celle eller det celleområde, som du vil have oplyst kolonnenummeret for. Hvis det ikke udfyldes, benyttes den celle, der indeholder funktionen KOLONNE"
			}
		]
	},
	{
		name: "KOLONNER",
		description: "Returnerer antallet af kolonner i en matrix eller en reference.",
		arguments: [
			{
				name: "matrix",
				description: "er en matrix, en matrixformel eller en reference til et celleområde, hvor du vil have oplyst antallet af kolonner"
			}
		]
	},
	{
		name: "KOMBIN",
		description: "Returnerer antallet af kombinationer for et givet antal elementer.",
		arguments: [
			{
				name: "tal",
				description: "er det samlede antal elementer"
			},
			{
				name: "tal_valgt",
				description: "er antallet af elementer i hver kombination"
			}
		]
	},
	{
		name: "KOMBINA",
		description: "Returnerer antal kombinationer med gentagelser for et givet antal elementer.",
		arguments: [
			{
				name: "tal",
				description: "er det samlede antal elementer"
			},
			{
				name: "tal_valgt",
				description: "er antal elementer i hver kombination"
			}
		]
	},
	{
		name: "KOMPLEKS",
		description: "Konverterer reelle og imaginære koefficienter til komplekse tal.",
		arguments: [
			{
				name: "reel_koefficient",
				description: "er den reelle koefficient for det komplekse tal"
			},
			{
				name: "imag_koefficient",
				description: "er den imaginære koefficient for det komplekse tal"
			},
			{
				name: "suffiks",
				description: "er det imaginære elements suffiks for det komplekse tal"
			}
		]
	},
	{
		name: "KONFIDENS.NORM",
		description: "Returnerer konfidensintervallet for middelværdien i en population ved hjælp af en normalfordeling.",
		arguments: [
			{
				name: "alpha",
				description: "er det signifikansniveau, der bruges til at beregne konfidensniveauet og er et tal, der er større end 0 og mindre end 1"
			},
			{
				name: "standardafv",
				description: "er populationens standardafvigelse for dataområdet og antages at være kendt. Standardafv skal være større end 0"
			},
			{
				name: "størrelse",
				description: "er stikprøvens størrelse"
			}
		]
	},
	{
		name: "KONFIDENSINTERVAL",
		description: "Returnerer et konfidensinterval for middelværdien i en population.",
		arguments: [
			{
				name: "alpha",
				description: "er det signifikansniveau, der bruges til at beregne konfidensniveauet. Det er et tal større end 0 og mindre end 1"
			},
			{
				name: "standardafv",
				description: "er populationens standardafvigelse, der antages at være kendt. Standardafv skal være større end 0"
			},
			{
				name: "størrelse",
				description: "er stikprøvestørrelsen"
			}
		]
	},
	{
		name: "KONFIDENST",
		description: "Returnerer konfidensintervallet for middelværdien i en population ved hjælp af t-fordelingen for en student.",
		arguments: [
			{
				name: "alpha",
				description: "er det signifikansniveau, der bruges til at beregne konfidensniveauet og er et tal, der er større end 0 og mindre end 1"
			},
			{
				name: "standardafv",
				description: "er populationens standardafvigelse for dataområdet og antages at være kendt. Standardafv skal være større end 0"
			},
			{
				name: "størrelse",
				description: "er stikprøvens størrelse"
			}
		]
	},
	{
		name: "KONVERTÉR",
		description: "Konverterer et tal fra en måleenhed til en anden.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal konverteres fra"
			},
			{
				name: "fra_enhed",
				description: "er enheden for tal"
			},
			{
				name: "til_enhed",
				description: "er enheden for resultatet"
			}
		]
	},
	{
		name: "KORRELATION",
		description: "Returnerer korrelationskoefficienten mellem to datasæt.",
		arguments: [
			{
				name: "vektor1",
				description: "er et celleområde indeholdende værdier. Værdierne er tal, navne, vektorer eller referencer, der indeholder tal"
			},
			{
				name: "vektor2",
				description: "er et andet celleområde indeholdende værdier. Værdierne er tal, navne, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "KOVARIANS",
		description: "Beregner kovariansen, dvs. gennemsnittet af produktet af standardafvigelsen for hvert datapar i to datasæt.",
		arguments: [
			{
				name: "vektor1",
				description: "er det første celleområde indeholdende heltalsværdier, som kan være tal, vektorer eller referencer, der indeholder tal"
			},
			{
				name: "vektor2",
				description: "er det andet celleområde indeholdende heltalsværdier, som kan være tal, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "KOVARIANS.P",
		description: "Returnerer kovariansen i populationen, dvs. gennemsnittet af produktet af standardafvigelsen for hvert datapar i to datasæt.",
		arguments: [
			{
				name: "matrix1",
				description: "er det første celleområde indeholdende heltalsværdier, som kan være tal, vektorer eller referencer, der indeholder tal"
			},
			{
				name: "matrix2",
				description: "er det andet celleområde indeholdende heltalsværdier, som kan være tal, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "KOVARIANS.S",
		description: "Returnerer kovariansen i stikprøven, dvs. gennemsnittet af produktet af afvigelsen for hvert datapar i to datasæt.",
		arguments: [
			{
				name: "matrix1",
				description: "er det første celleområde indeholdende heltalsværdier, som kan være tal, vektorer eller referencer, der indeholder tal"
			},
			{
				name: "matrix2",
				description: "er det andet celleområde indeholdende heltalsværdier, som kan være tal, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "KR",
		description: "Konverterer et tal til tekst vha. valutaformat.",
		arguments: [
			{
				name: "tal",
				description: "er et tal, en reference til en celle, der indeholder et tal, eller en formel, der evalueres som et tal"
			},
			{
				name: "decimaler",
				description: "angiver antallet af cifre til højre for decimaltegnet. Tallet afrundes efter behov. Hvis udeladt er decimaler = 2"
			}
		]
	},
	{
		name: "KR.BRØK",
		description: "Konverterer en kronepris, udtrykt som et decimaltal, til en kronepris udtrykt som brøk.",
		arguments: [
			{
				name: "decimal_kr",
				description: "er et decimaltal"
			},
			{
				name: "brøkdel",
				description: "er heltallet, der skal anvendes som nævner i brøken"
			}
		]
	},
	{
		name: "KR.DECIMAL",
		description: "Konverterer en kronepris, udtrykt som brøk, til en kronepris udtrykt som decimaltal.",
		arguments: [
			{
				name: "brøkdel_kr",
				description: "er et tal, der er udtrykt som en brøk"
			},
			{
				name: "brøkdel",
				description: "er heltallet, der skal anvendes som nævner i brøken"
			}
		]
	},
	{
		name: "KRITBINOM",
		description: "Returnerer den mindste værdi, som det gælder for, at den kumulative binomiale fordeling er større end eller lig med en kriterieværdi.",
		arguments: [
			{
				name: "forsøg",
				description: "er antallet af Bernoulli-forsøg"
			},
			{
				name: "sandsynlighed_s",
				description: "er sandsynligheden for et gunstigt udfald for hvert forsøg. Det er et tal mellem 0 og 1"
			},
			{
				name: "alpha",
				description: "er kriterieværdien. Det er et tal mellem 0 og 1"
			}
		]
	},
	{
		name: "KUPONBETALINGER",
		description: "Returnerer antal kuponbetalinger mellem afregnings- og udløbsdatoen.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, udtrykt som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "hyppighed",
				description: "er antal kuponbetalinger pr. år"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "KUPONDAG.FORRIGE",
		description: "Returnerer den forrige kupondato før afregningsdatoen.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datottal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "hyppighed",
				description: "er antal kuponbetalinger pr. år"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "KUPONDAG.NÆSTE",
		description: "Returnerer den næste kupondato efter afregningsdatoen.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "hyppighed",
				description: "er antal kuponbetalinger pr. år"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "KUPONDAGE.SA",
		description: "Returnerer antal dage fra starten af kuponperioden til afregningsdatoen.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, udtrykt som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "hyppighed",
				description: "er antal kuponbetalinger pr. år"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "KURS.DISKONTO",
		description: "Returnerer kursen pr. kr 100 nominel værdi for et diskonteret værdipapir.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "diskonto",
				description: "er værdipapirets diskontosats"
			},
			{
				name: "indløsningskurs",
				description: "er værdipapirets indløsningskurs pr. kr. 100 i pålydende værdi"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "KVARTIL",
		description: "Returnerer kvartilen i et givet datasæt.",
		arguments: [
			{
				name: "vektor",
				description: "er den matrix eller det celleområde med numeriske værdier, som kvartilen skal beregnes for"
			},
			{
				name: "kvart",
				description: "er et tal, der kan have følgende værdier: 0 = minimumværdien, 1 = 1. kvartil, 2 = middelværdien, 3 = 3. kvartil, 4 = maksimumværdien"
			}
		]
	},
	{
		name: "KVARTIL.MEDTAG",
		description: "Returnerer kvartilen for et datasæt baseret på fraktilværdier fra 0..1 inklusive.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det celleområde af numeriske værdier, som du ønsker kvartilværdien for"
			},
			{
				name: "kvartil",
				description: "er et tal. minimumværdien = 0; første1. kvartil = 1; middelværdien = 2; 3. kvartil = 3; maksimumværdien = 4"
			}
		]
	},
	{
		name: "KVARTIL.UDELAD",
		description: "Returnerer kvartilen for et datasæt baseret på fraktilværdier fra 0..1 eksklusive.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det celleområde af numeriske værdier, som kvartilværdien skal beregnes for"
			},
			{
				name: "kvartil",
				description: "ier et tal: minimumværdien = 0; 1. kvartil = 1; middelværdien = 2; 3. kvartil = 3; maksimumværdien = 4"
			}
		]
	},
	{
		name: "KVOTIENT",
		description: "Returnerer heltalsdelen af en division.",
		arguments: [
			{
				name: "tæller",
				description: "er dividenden"
			},
			{
				name: "nævner",
				description: "er divisoren"
			}
		]
	},
	{
		name: "KVROD",
		description: "Returnerer kvadratroden af et tal.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, kvadratroden skal beregnes af"
			}
		]
	},
	{
		name: "KVRODPI",
		description: "Returnerer kvadratroden af (tal * pi).",
		arguments: [
			{
				name: "tal",
				description: "er det tal, der skal ganges med pi"
			}
		]
	},
	{
		name: "LA",
		description: "Returnerer den lineære afskrivning for et aktiv i en enkelt periode.",
		arguments: [
			{
				name: "købspris",
				description: "er aktivets kostpris"
			},
			{
				name: "restværdi",
				description: "er aktivets værdi ved afskrivningens afslutning"
			},
			{
				name: "levetid",
				description: "er antallet af afskrivningsperioder (aktivets levetid)"
			}
		]
	},
	{
		name: "LÆNGDE",
		description: "Returnerer antallet af tegn i en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst, som du vil finde længden på. Mellemrum tæller som tegn"
			}
		]
	},
	{
		name: "LIGE",
		description: "Runder positive tal op og negative tal ned til nærmeste lige heltal.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal afrundes"
			}
		]
	},
	{
		name: "LINK",
		description: "Opretter en genvej eller et jump, der åbner et dokument, der er lagret på harddisken, en netserver eller på internettet.",
		arguments: [
			{
				name: "linkplacering",
				description: "er den tekst, der leverer sti- eller filnavnet til det dokument, der skal åbnes, harddiskplacering, UNC-adresse eller URL-sti"
			},
			{
				name: "fuldt_navn",
				description: "er tekst eller et tal, der vises i cellen. Hvis udeladt, viser cellen Link_placeringsteksten"
			}
		]
	},
	{
		name: "LINREGR",
		description: "Returnerer en statistik, der beskriver en lineær tendens svarende til kendte datapunkter ved brug af de mindste kvadraters metode.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er mængden af kendte y-værdier i forholdet y = mx + b"
			},
			{
				name: "kendte_x'er",
				description: "er en mængde x-værdier, som muligvis er kendte, i forholdet y = mx + b"
			},
			{
				name: "konstant",
				description: "er en logisk værdi: konstanten b beregnes normalt, hvis Konst = SAND eller udeladt; b sættes til lig 0, hvis Konst = FALSK"
			},
			{
				name: "statistik",
				description: "er en logisk værdi: SAND = der returneres yderligere regressionsstatistik; FALSK eller udeladt = der returneres m-koefficienter og konstanten b"
			}
		]
	},
	{
		name: "LN",
		description: "Returnerer et tals naturlige logaritme.",
		arguments: [
			{
				name: "tal",
				description: "er det positive reelle tal, den naturlige logaritme skal beregnes for"
			}
		]
	},
	{
		name: "LOFT.MAT",
		description: "Runder et tal op til det nærmeste heltal eller til det nærmeste betydende multiplum.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, du vil afrunde"
			},
			{
				name: "betydning",
				description: "er det multiplum, du vil afrunde til"
			},
			{
				name: "tilstand",
				description: "hvis dette angives, og værdien ikke er nul, afrundes der væk fra nul"
			}
		]
	},
	{
		name: "LOFT.PRECISE",
		description: "Runder et tal op til nærmeste heltal eller til nærmeste multiplum af betydning.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal afrundes"
			},
			{
				name: "betydning",
				description: "er det multiplum, der skal afrundes til"
			}
		]
	},
	{
		name: "LOG",
		description: "Returnerer et tals logaritme på grundlag af et angivet grundtal.",
		arguments: [
			{
				name: "tal",
				description: "er det positive reelle tal, logaritmen skal beregnes for"
			},
			{
				name: "grundtal",
				description: "er logaritmens grundtal. Sættes til 10, hvis feltet ikke udfyldes"
			}
		]
	},
	{
		name: "LOG10",
		description: "Returnerer et tals titals logaritme.",
		arguments: [
			{
				name: "tal",
				description: "er det positive reelle tal, 10-tals logaritmen skal beregnes for"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Returnerer den inverse fordelingsfunktion for lognormalfordelingen til x, hvor In(x) er distribueret normalt med parametrene Middelværdi og Standardafv.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til lognormalfordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for ln(x)"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for ln(x)"
			}
		]
	},
	{
		name: "LOGNORM.FORDELING",
		description: "Returnerer fordelingsfunktionen for lognormalfordelingen af x, hvor In(x) normalt distribueres med parametrene Middelværdi og Standardafv.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et positivt tal), som funktionen skal evalueres for"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for ln(x)"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for ln(x)"
			},
			{
				name: "kumulativ",
				description: "er en logisk værd. Hvis SAND, returneres fordelingsfunktionen. Hvis FALSK, returneres sandsynlighedsfunktionen"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Returnerer den inverse fordelingsfunktion for lognormalfordelingen af x, hvor In(x) normalt distribueres med parametrene Middelværdi og Standardafv.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til lognormalfordelingen. Det er et tal mellem 0 og 1"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for ln(x)"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for ln(x)"
			}
		]
	},
	{
		name: "LOGNORMFORDELING",
		description: "Returnerer fordelingsfunktionen for lognormalfordelingen af x, hvor In(x) normalt distribueres med parametrene Middelværdi og Standardafv.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et positivt tal), som funktionen skal evalueres for"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for ln(x)"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for ln(x)"
			}
		]
	},
	{
		name: "LOGREGR",
		description: "Returnerer en statistik, der beskriver en eksponentialkurve svarende til kendte datapunkter.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er mængden af kendte y-værdier i forholdet y = b*m^x"
			},
			{
				name: "kendte_x'er",
				description: "er en mængde x-værdier, som muligvis er kendte, i forholdet y = b*m^x"
			},
			{
				name: "konstant",
				description: "er en logisk værdi: konstanten b beregnes normalt, hvis Konst = SAND eller udeladt; b sættes til 1, hvis Konst = FALSK"
			},
			{
				name: "statistik",
				description: "er en logisk værdi: SAND = der returneres yderligere regressionsstatistik; FALSK eller udeladt = der returneres m-koefficienter og konstanten b"
			}
		]
	},
	{
		name: "LOPSLAG",
		description: "Søger efter en værdi i den første kolonne i en tabel og returnerer en værdi i den samme række fra en anden kolonne, du har angivet. Tabellen skal som standard sorteres i stigende rækkefølge.",
		arguments: [
			{
				name: "opslagsværdi",
				description: "er den værdi, der skal findes i matrixens første kolonne. Det kan være en værdi, en reference eller en tekststreng"
			},
			{
				name: "tabelmatrix",
				description: "er en tabel med tekst, tal eller logiske værdier, hvor dataene findes. Tabelmatrix kan være en reference til et område eller et områdenavn"
			},
			{
				name: "kolonneindeks_nr",
				description: "er kolonnenummeret i tabelmatrixen, hvorfra den tilsvarende værdi skal returneres. Den første kolonne med værdier er kolonne 1"
			},
			{
				name: "lig_med",
				description: "er en logisk værdi, hvor SAND eller udeladt = find den nærmeste værdi i den første kolonne (sorteret i stigende rækkefølge), og FALSK = find en værdi, der er nøjagtigt lig med"
			}
		]
	},
	{
		name: "MAD",
		description: "Returnerer den gennemsnitlige absolutte afvigelse af datapunkter fra deres middelværdi. Argumenter kan være tal, navne, matrixer eller referencer, der indeholder tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 argumenter, som den gennemsnitlige absolutte afvigelse skal beregnes for"
			},
			{
				name: "tal2",
				description: "er 1-255 argumenter, som den gennemsnitlige absolutte afvigelse skal beregnes for"
			}
		]
	},
	{
		name: "MAFRUND",
		description: "Returnerer et tal afrundet til det ønskede multiplum.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal afrundes"
			},
			{
				name: "multiplum",
				description: "er det multiplum, hvortil tallet skal afrundes"
			}
		]
	},
	{
		name: "MAKS",
		description: "Returnerer den største værdi fra et datasæt. Ignorerer logiske værdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, tomme celler, logiske værdier eller tal i bogstavformat, som du vil finde den største værdi for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, tomme celler, logiske værdier eller tal i bogstavformat, som du vil finde den største værdi for"
			}
		]
	},
	{
		name: "MAKSV",
		description: "Returnerer den største værdi fra et værdisæt. Ignorerer ikke logiske værdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 tal, tomme celler, logiske værdier eller tal i bogstavformat, som du vil finde den maksimale værdi for"
			},
			{
				name: "værdi2",
				description: "er 1-255 tal, tomme celler, logiske værdier eller tal i bogstavformat, som du vil finde den maksimale værdi for"
			}
		]
	},
	{
		name: "MÅNED",
		description: "Returnerer måneden, et tal mellem 1 (januar) og 12 (december).",
		arguments: [
			{
				name: "serienr",
				description: "er et tal i den dato- og klokkeslætskode, der anvendes af Spreadsheet"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Returnerer determinanten for en matrix.",
		arguments: [
			{
				name: "matrix",
				description: "er en numerisk matrix med det samme antal rækker og kolonner, enten et celleområde eller en matrixkonstant"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Returnerer medianen, eller den midterste værdi, for det givne talsæt.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer indeholdende tal, som du vil finde medianen for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer indeholdende tal, som du vil finde medianen for"
			}
		]
	},
	{
		name: "MENHED",
		description: "Returnerer enhedsmatrixen for den angivne dimension.",
		arguments: [
			{
				name: "dimension",
				description: "er et heltal, der specificerer dimensionen af den enhedsmatrix, du vil returnere"
			}
		]
	},
	{
		name: "MIA",
		description: "Returnerer den interne forrentningprocent for en række periodiske pengestrømme, hvor både investeringsudgifter og renteindtægter ved geninvestering tages i betragtning.",
		arguments: [
			{
				name: "værdier",
				description: "er en matrix eller en reference til celler, der indeholder tal, som repræsenterer en serie af ydelser (negative) og indkomster (positive) i en given periode"
			},
			{
				name: "finansrente",
				description: "er den rente, du betaler på de beløb, der anvendes i pengestrømme"
			},
			{
				name: "investeringsrente",
				description: "er den rente, der fås på pengestrømme, efterhånden som de geninvesteres"
			}
		]
	},
	{
		name: "MIDDEL",
		description: "Returnerer middelværdien af argumenterne, som kan være tal, navne, matrixer eller referencer, der indeholder tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 numeriske argumenter, som du vil beregne middelværdien for"
			},
			{
				name: "tal2",
				description: "er 1-255 numeriske argumenter, som du vil beregne middelværdien for"
			}
		]
	},
	{
		name: "MIDDEL.HVIS",
		description: "Finder middelværdien af cellerne ud fra en given betingelse eller et givet kriterium.",
		arguments: [
			{
				name: "område",
				description: "er det celleområde, der skal evalueres"
			},
			{
				name: "kriterier",
				description: "er betingelsen eller kriteriet i form af et tal, et udtryk eller tekst, der definerer de celler, der skal bruges til at finde middelværdien"
			},
			{
				name: "middelområde",
				description: "er de faktiske celler, der skal bruges til at finde middelværdien. Hvis intet angives, bruges cellerne i området"
			}
		]
	},
	{
		name: "MIDDEL.HVISER",
		description: "Finder middelværdien af de celler, der er angivet med et sæt betingelser eller kriterier.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "middelområde",
				description: "er de faktiske celler, der skal bruges til at finde middelværdien"
			},
			{
				name: "kriterieområde",
				description: "er det celleområde, der skal evalueres i forhold til den konkrete betingelse"
			},
			{
				name: "kriterier",
				description: "er betingelsen eller kriteriet i form af et tal, et udtryk eller tekst, der definerer de celler, der skal bruges til at finde middelværdien"
			}
		]
	},
	{
		name: "MIDDELV",
		description: "Returnerer middelværdien af argumenterne, hvor tekst og FALSK evalueres som 0, og SAND evalueres som 1. Argumenter kan være tal, navne, matrixer eller referencer.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 argumenter, som du vil beregne middelværdien af"
			},
			{
				name: "værdi2",
				description: "er 1-255 argumenter, som du vil beregne middelværdien af"
			}
		]
	},
	{
		name: "MIDT",
		description: "Returnerer tegnene fra midten af en tekststreng ved angivelse af startposition og længde.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekststreng, du vil uddrage tegnene fra"
			},
			{
				name: "start_ved",
				description: "angiver positionen for det første tegn, der skal uddrages. Det første tegn i Tekst er 1"
			},
			{
				name: "antal_tegn",
				description: "angiver, hvor mange tegn der skal returneres fra Tekst"
			}
		]
	},
	{
		name: "MIN",
		description: "Returnerer det mindste tal fra et værdisæt. Ignorerer logiske værdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, tomme celler eller logiske værdier, som du vil finde den mindste værdi for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, tomme celler eller logiske værdier, som du vil finde den mindste værdi for"
			}
		]
	},
	{
		name: "MINDSTE",
		description: "Returnerer den k'te-mindste værdi i et datasæt. For eksempel det femtemindste tal.",
		arguments: [
			{
				name: "matrix",
				description: "er en matrix eller et interval af numeriske data, som den k'te-mindste værdi skal bestemmes for"
			},
			{
				name: "k",
				description: "er den position (regnet fra mindste værdi), hvorfra værdien skal returneres"
			}
		]
	},
	{
		name: "MINDSTE.FÆLLES.MULTIPLUM",
		description: "Returnerer det mindste fælles multiplum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1 til 255 værdier, hvis mindste fælles multiplum skal beregnes"
			},
			{
				name: "tal2",
				description: "er 1 til 255 værdier, hvis mindste fælles multiplum skal beregnes"
			}
		]
	},
	{
		name: "MINUT",
		description: "Returnerer minuttet, et tal mellem 0 og 59.",
		arguments: [
			{
				name: "serienr",
				description: "er et tal i den dato- og klokkeslætskode, der anvendes af Spreadsheet, eller tekst i klokkeslætsformat, f.eks. 16:48:00 eller 04:48:00 PM"
			}
		]
	},
	{
		name: "MINV",
		description: "Returnerer den mindste værdi fra et værdisæt. Logiske værdier og tekst ignoreres ikke.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 tal, tomme celler, logiske værdier eller tal i bogstavformat, som du vil finde den mindste værdi for"
			},
			{
				name: "værdi2",
				description: "er 1-255 tal, tomme celler, logiske værdier eller tal i bogstavformat, som du vil finde den mindste værdi for"
			}
		]
	},
	{
		name: "MINVERT",
		description: "Returnerer den inverse matrix for en matrix.",
		arguments: [
			{
				name: "matrix",
				description: "er en numerisk matrix med det samme antal rækker og kolonner, enten et celleområde eller en matrixkonstant"
			}
		]
	},
	{
		name: "MODTAGET.VED.UDLØB",
		description: "Returnerer beløbet modtaget ved udløbet af et værdipapir.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "investering",
				description: "er det beløb, der er investeret i værdipapiret"
			},
			{
				name: "diskonto",
				description: "er værdipapirets diskontosats"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "MPRODUKT",
		description: "Returnerer matrixproduktet af to matrixer, dvs. en matrix med samme antal rækker som matrix1 og samme antal kolonner som matrix2.",
		arguments: [
			{
				name: "matrix1",
				description: "er den første matrix af tal, der skal ganges, hvor antallet af kolonner skal være lig med antallet af rækker i Matrix 2"
			},
			{
				name: "matrix2",
				description: "er den første matrix af tal, der skal ganges, hvor antallet af kolonner skal være lig med antallet af rækker i Matrix 2"
			}
		]
	},
	{
		name: "MULTINOMIAL",
		description: "Returnerer polynomiet af en række tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1 til 255 værdier, hvis polynomium skal beregnes"
			},
			{
				name: "tal2",
				description: "er 1 til 255 værdier, hvis polynomium skal beregnes"
			}
		]
	},
	{
		name: "NEGBINOM.FORDELING",
		description: "Returnerer punktsandsynligheden for den negative binomialfordeling, dvs. sandsynligheden for, at der vil være tal_f mislykkede forsøg, før det lykkes i forsøg tal_s med sandsynligheden sandsynlighed_s for, at et forsøg lykkes.",
		arguments: [
			{
				name: "tal_f",
				description: "er antallet af mislykkede forsøg"
			},
			{
				name: "tal_s",
				description: "er det ønskede antal gunstige udfald"
			},
			{
				name: "sandsynlighed_s",
				description: "er sandsynligheden for et gunstigt udfald, dvs. et tal mellem 0 og 1"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer tæthedsfunktionen"
			}
		]
	},
	{
		name: "NEGBINOMFORDELING",
		description: "Returnerer punktsandsynligheden for den negative binomialfordeling, dvs. sandsynligheden for, at der vil være tal_f mislykkede forsøg før det lykkedes i forsøg tal_s med sandsynligheden sandsynlighed_s for at et forsøg lykkes.",
		arguments: [
			{
				name: "tal_f",
				description: "er antallet af mislykkede forsøg"
			},
			{
				name: "tal_s",
				description: "er det ønskede antal gunstige udfald"
			},
			{
				name: "sandsynlighed_s",
				description: "er sandsynligheden for et gunstigt udfald, dvs. et tal mellem 0 og 1"
			}
		]
	},
	{
		name: "NETTO.NUTIDSVÆRDI",
		description: "Returnerer nutidsværdien af en pengestrømsplan.",
		arguments: [
			{
				name: "rente",
				description: "er diskontosatsen, der gælder for pengestrømmene"
			},
			{
				name: "værdier",
				description: "er en serie af pengestrømme, der stemmer overens med en betalingsplan med datoer"
			},
			{
				name: "datoer",
				description: "er en plan over betalingsdatoer, der stemmer overens med pengestrømsbetalingerne"
			}
		]
	},
	{
		name: "NOMINEL",
		description: "Returnerer den årlige nominelle rente.",
		arguments: [
			{
				name: "effektiv_rente",
				description: "er den effektive rente"
			},
			{
				name: "nperår",
				description: "er antal sammensatte perioder pr. år"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Returnerer normalfordelingen for den angivne middelværdi og standardafvigelse.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til normalfordelingen, et tal større end eller lig med 0 og mindre end eller lig med 1"
			},
			{
				name: "middelværdi",
				description: "er fordelingens middelværdi"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen for fordelingen, et positivt tal"
			}
		]
	},
	{
		name: "NORMAL.FORDELING",
		description: "Returnerer normalfordelingen for den angivne middelværdi og standardafvigelse.",
		arguments: [
			{
				name: "x",
				description: "er den værdi, fordelingen skal beregnes for"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for fordelingen"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for fordelingen"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis SAND returneres fordelingsfunktionen. Hvis FALSK returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "NORMFORDELING",
		description: "Returnerer normalfordelingen for den angivne middelværdi og standardafvigelse.",
		arguments: [
			{
				name: "x",
				description: "er den værdi, fordelingen skal beregnes for"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for fordelingen"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for fordelingen"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis SAND returneres fordelingsfunktionen. Hvis FALSK returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Returnerer den inverse fordelingsfunktion for normalfordelingen for den angivne middelværdi og standardafvigelse.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til normalfordelingen. Det er et tal større end eller lig med 0 og mindre end eller lig med1"
			},
			{
				name: "middelværdi",
				description: "er fordelingens middelværdi"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for den stokastiske variabel"
			}
		]
	},
	{
		name: "NPER",
		description: "Returnerer antallet af perioder for en investering, baseret på konstante periodiske ydelser og en konstant rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "ydelse",
				description: "er ydelsen i hver periode. Ydelsen kan ikke ændres, så længe investeringen løber"
			},
			{
				name: "nv",
				description: "er nutidsværdien eller den samlede værdi, som en række fremtidige ydelser er værd nu"
			},
			{
				name: "fv",
				description: "er den fremtidige værdi eller den kassebalance, der ønskes opnået, når den sidste ydelse er betalt. Sættes til nul, hvis den udelades"
			},
			{
				name: "type",
				description: "er en logisk værdi: ydelse i begyndelsen af perioden = 1; ydelse i slutningen af perioden = 0 eller udeladt"
			}
		]
	},
	{
		name: "NU",
		description: "Returnerer den aktuelle dato og det aktuelle klokkeslæt formateret som dato og klokkeslæt.",
		arguments: [
		]
	},
	{
		name: "NUTIDSVÆRDI",
		description: "Returnerer den aktuelle nettoværdi af en investering på basis af en diskontosats og en serie fremtidige betalinger (negative værdier) og indkomst (positive værdier).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rente",
				description: "er diskonteringssatsen for en hel periode"
			},
			{
				name: "værdi1",
				description: "er 1-254 regelmæssige ydelser og indtægter i slutningen af hver periode"
			},
			{
				name: "værdi2",
				description: "er 1-254 regelmæssige ydelser og indtægter i slutningen af hver periode"
			}
		]
	},
	{
		name: "NV",
		description: "Returnerer nutidsværdien for en investering: det totale beløb, som en række fremtidige ydelser er værd nu.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder i en investering"
			},
			{
				name: "ydelse",
				description: "er den ydelse, der betales hver periode, og som ikke kan ændres i investeringens løbetid"
			},
			{
				name: "fv",
				description: "er den fremtidige værdi eller den kassebalance, der ønskes opnået, når den sidste ydelse er betalt"
			},
			{
				name: "type",
				description: "er en logisk værdi: ydelse i begyndelsen af perioden = 1; ydelse i slutningen af perioden = 0 eller udeladt"
			}
		]
	},
	{
		name: "OG",
		description: "Undersøger, om alle argumenter er SAND, og returnerer SAND, hvis alle argumenter er SAND.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "er 1-255 betingelser, du vil teste, som kan være enten SAND eller FALSK, og som kan være logiske værdier, matrixer eller referencer"
			},
			{
				name: "logisk2",
				description: "er 1-255 betingelser, du vil teste, som kan være enten SAND eller FALSK, og som kan være logiske værdier, matrixer eller referencer"
			}
		]
	},
	{
		name: "OKT.TIL.BIN",
		description: "Konverterer et oktaltal til et binært tal.",
		arguments: [
			{
				name: "tal",
				description: "er det oktaltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "OKT.TIL.DEC",
		description: "Konverterer et oktaltal til et decimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det oktaltal, der skal konverteres"
			}
		]
	},
	{
		name: "OKT.TIL.HEX",
		description: "Konverterer et oktaltal til et hexadecimaltal.",
		arguments: [
			{
				name: "tal",
				description: "er det oktaltal, der skal konverteres"
			},
			{
				name: "pladser",
				description: "er det antal tegn, der skal anvendes"
			}
		]
	},
	{
		name: "OMRÅDER",
		description: "Returnerer antallet af områder i en reference. Et område kan bestå af en enkelt eller af flere sammenhængende celler.",
		arguments: [
			{
				name: "reference",
				description: "er en reference til en celle eller en række af celler og kan referere til flere områder"
			}
		]
	},
	{
		name: "PÅLØBRENTE.UDLØB",
		description: "Returnerer den påløbne rente for et værdipapir med renteudbetaling ved udløb.",
		arguments: [
			{
				name: "udstedelsesdato",
				description: "er værdipapirets udstedelsesdato, angivet som et serielt datotal"
			},
			{
				name: "afregningsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "rente",
				description: "er værdipapirets årlige kuponrente"
			},
			{
				name: "nominel",
				description: "er værdipapirets nominelle værdi"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
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
		name: "PEARSON",
		description: "Returnerer Pearsons korrelationskoefficient, r.",
		arguments: [
			{
				name: "matrix1",
				description: "er en mængde uafhængige værdier"
			},
			{
				name: "matrix2",
				description: "er en mængde afhængige værdier"
			}
		]
	},
	{
		name: "PERMUT",
		description: "Returnerer antallet af permutationer for et givet antal af objekter, der kan  vælges fra det totale antal objekter.",
		arguments: [
			{
				name: "tal",
				description: "er det samlede antal objekter"
			},
			{
				name: "tal_valgt",
				description: "er antallet af objekter i hver permutation"
			}
		]
	},
	{
		name: "PERMUTATIONA",
		description: "Returnerer antal permutationer for et givet antal objekter (med gentagelser), der kan vælges ud fra det samlede antal objekter.",
		arguments: [
			{
				name: "tal",
				description: "er det samlede antal objekter"
			},
			{
				name: "tal_valgt",
				description: "er antal objekter i hver permutation"
			}
		]
	},
	{
		name: "PHI",
		description: "Returnerer værdien af tæthedsfunktionen for en standardnormalfordeling.",
		arguments: [
			{
				name: "x",
				description: "er det tal, du vil finde tætheden af standardnormalfordelingen for"
			}
		]
	},
	{
		name: "PI",
		description: "Returnerer værdien af pi  (3.14159265358979) med 15 decimalers nøjagtighed.",
		arguments: [
		]
	},
	{
		name: "PLADS",
		description: "Returnerer rangen for et tal i en liste med tal, dvs. tallets størrelse i forhold til de andre værdier i listen.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, du vil finde rangen for"
			},
			{
				name: "reference",
				description: "er en liste eller en reference til en liste af tal. Ikke-numeriske værdier ignoreres"
			},
			{
				name: "rækkefølge",
				description: "er et tal: rangen i listen, hvis den er sorteret faldende = 0 eller udeladt, rangen i listen, hvis den er sorteret stigende = en vilkårlig værdi forskellig fra nul"
			}
		]
	},
	{
		name: "PLADS.GNSN",
		description: "Returnerer rangen for et tal i en liste med tal. Dets størrelse i forhold til andre værdier på listen. Hvis mere end et tal har den samme rang, returneres den gennemsnitlige rang.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, du ønsker at finde rangen for"
			},
			{
				name: "reference",
				description: "er en matrix for eller en reference til en liste med tal. Ikke-numeriske værdier ignoreres"
			},
			{
				name: "rækkefølge",
				description: "er et tal. Rangen på listen er sorteret i faldende rækkefølge = 0 eller udeladt. Rangen på listen er sorteret i stigende rækkefølge = enhver anden værdi end nul"
			}
		]
	},
	{
		name: "PLADS.LIGE",
		description: "Returnerer rangen for et tal i en liste med tal. Dets størrelse i forhold til andre værdier på listen. Hvis mere end en værdi har den samme rang, returneres den øverste rang for dette værdisæt.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, du ønsker at finde rangen for"
			},
			{
				name: "reference",
				description: "er en matrix for eller en reference til en liste med tal. Ikke-numeriske værdier ignoreres"
			},
			{
				name: "rækkefølge",
				description: "er et tal. Rangen på listen er sorteret i faldende rækkefølge = 0 eller udeladt. Rangen på listen er sorteret i stigende rækkefølge = enhver anden værdi end nul"
			}
		]
	},
	{
		name: "POISSON",
		description: "Returnerer Poisson-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er antallet af hændelser"
			},
			{
				name: "middelværdi",
				description: "er den forventede numeriske værdi (et positivt tal)"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis kumulativ er SAND, returneres fordelingsfunktionen, og hvis FALSK, returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "POISSON.FORDELING",
		description: "Returnerer Poisson-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er antallet af hændelser"
			},
			{
				name: "middelværdi",
				description: "er den forventede numeriske værdi (et positivt tal)"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis kumulativ er SAND, returneres fordelingsfunktionen, og hvis FALSK, returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "POTENS",
		description: "Returnerer resultatet af et tal opløftet til en potens.",
		arguments: [
			{
				name: "tal",
				description: "er grundtallet, som kan være et vilkårligt reelt tal"
			},
			{
				name: "potens",
				description: "er den eksponent, som grundtallet skal opløftes til"
			}
		]
	},
	{
		name: "PROCENTPLADS",
		description: "Returnerer den procentuelle rang for en given værdi i datasættet.",
		arguments: [
			{
				name: "vektor",
				description: "er den vektor eller det numeriske datainterval, der definerer den relative status"
			},
			{
				name: "x",
				description: "er den værdi, hvis rang ønskes bestemt"
			},
			{
				name: "bet_cifre",
				description: "er en valgfri specifikation af betydende cifre ved beregning af procentvis rang. Sættes til 3 cifre, hvis det udelades"
			}
		]
	},
	{
		name: "PROCENTPLADS.MEDTAG",
		description: "Returnerer rangen for en værdi i et datasæt som en procentdel (fra 0 til og med 1) af datasættet.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det numeriske datainterval, der definerer den relative status"
			},
			{
				name: "x",
				description: "er den værdi, hvis rang ønskes bestemt"
			},
			{
				name: "signifikans",
				description: "er en valgfri specifikation af betydende cifre ved beregning af procentvis rang. Sættes til 3 cifre, hvis det udelades (0,xxx%)"
			}
		]
	},
	{
		name: "PROCENTPLADS.UDELAD",
		description: "Returnerer rangen for en værdi i et datasæt som en procentdel (fra 0 til 1) af datasættet.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det numeriske datainterval, der definerer den relative status"
			},
			{
				name: "x",
				description: "er den værdi, hvis rang ønskes bestemt"
			},
			{
				name: "signifikans",
				description: "er en valgfri specifikation af betydende cifre ved beregning af procentvis rang. Sættes til 3 cifre, hvis det udelades (0,xxx%)"
			}
		]
	},
	{
		name: "PRODUKT",
		description: "Multiplicerer de tal, der er givet som argumenter.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, logiske værdier eller tal i bogstavformat, der skal multipliceres med hinanden"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, logiske værdier eller tal i bogstavformat, der skal multipliceres med hinanden"
			}
		]
	},
	{
		name: "PROGNOSE",
		description: "Beregner eller forudsiger en fremtidig værdi baseret på lineær regression med brug af kendte værdier.",
		arguments: [
			{
				name: "x",
				description: "er den værdi for den forklarende variabel, som prognosen skal beregnes for. Det skal være en numerisk værdi"
			},
			{
				name: "kendte_y'er",
				description: "er observationer på den afhængige variabel"
			},
			{
				name: "kendte_x'er",
				description: "er observationer på den forklarende variabel i form af en matrix eller et numerisk dataområde. Variansen af Kendte_x'er må ikke være nul"
			}
		]
	},
	{
		name: "PVARIGHED",
		description: "Returnerer det påkrævede antal perioder, før en investering når den angivne værdi.",
		arguments: [
			{
				name: "rente",
				description: "er renten pr. periode."
			},
			{
				name: "nv",
				description: "er nutidsværdien af investeringen"
			},
			{
				name: "fv",
				description: "er den ønskede fremtidige værdi af investeringen"
			}
		]
	},
	{
		name: "R.YDELSE",
		description: "Returnerer rentedelen af en ydelse for en investering i en given periode, baseret på konstante periodiske ydelser og en konstant rente.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "periode",
				description: "er den periode, renten skal beregnes for. Den skal være mellem 1 og nper"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder i en investering"
			},
			{
				name: "nv",
				description: "er nutidsværdien eller den samlede værdi, som en række fremtidige ydelser er værd nu"
			},
			{
				name: "fv",
				description: "er den fremtidige værdi eller den kassebalance, der ønskes opnået, når den sidste ydelse er betalt. Sættes til Fv = 0, hvis intet angives"
			},
			{
				name: "type",
				description: "er en logisk værdi, der repræsenterer, hvornår ydelserne forfalder: 1 = i begyndelsen af perioden, 0 eller ikke udfyldt = i slutningen af perioden"
			}
		]
	},
	{
		name: "RADIANER",
		description: "Konverterer grader til radianer.",
		arguments: [
			{
				name: "vinkel",
				description: "er en vinkel i grader, som du vil konvertere"
			}
		]
	},
	{
		name: "RÆKKE",
		description: "Returnerer rækkenummeret for en reference.",
		arguments: [
			{
				name: "reference",
				description: "er den celle eller det celleområde, som du vil have oplyst rækkenummeret for. Hvis det udelades, returneres den celle, der indeholder funktionen RÆKKE"
			}
		]
	},
	{
		name: "RÆKKER",
		description: "Returnerer antallet af rækker i en reference eller en matrix.",
		arguments: [
			{
				name: "matrix",
				description: "er en matrix, en matrixformel eller en reference til et celleområde, hvor du vil have oplyst antallet af rækker"
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
		name: "RENS",
		description: "Fjerner alle tegn, der ikke kan udskrives, fra tekst.",
		arguments: [
			{
				name: "tekst",
				description: "er alle regnearksoplysninger, hvorfra tegn, der ikke kan udskrives, skal fjernes"
			}
		]
	},
	{
		name: "RENTE",
		description: "Returnerer renten i hver periode for et lån eller en investering. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR.",
		arguments: [
			{
				name: "nper",
				description: "er det samlede antal ydelsesperioder for et lån eller en investering"
			},
			{
				name: "ydelse",
				description: "er den ydelse, der betales hver periode. Ydelsen kan ikke ændres i lånets eller investeringens løbetid"
			},
			{
				name: "nv",
				description: "er nutidsværdien, dvs. den samlede værdi, som en række fremtidige ydelser er værd nu"
			},
			{
				name: "fv",
				description: "er den fremtidige værdi eller den kassebalance, der ønskes opnået, når den sidste ydelse er betalt. Hvis den udelades, sættes Fv = 0"
			},
			{
				name: "type",
				description: "er en logisk værdi: ydelse i begyndelsen af perioden = 1, ydelse i slutningen af perioden = 0 eller udeladt"
			},
			{
				name: "gæt",
				description: "er et skøn over rentefodens størrelse. Hvis feltet ikke udfyldes, sættes Gæt = 0,1 (10 procent)"
			}
		]
	},
	{
		name: "RENTEFOD",
		description: "Returnerer renten på et fuldt ud investeret værdipapir.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er værdipapirets afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er værdipapirets udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "investering",
				description: "er det beløb, der er investeret i værdipapiret"
			},
			{
				name: "indløsningskurs",
				description: "er det beløb, der modtages ved værdipapirets udløb"
			},
			{
				name: "datotype",
				description: "er den datotype, der skal anvendes"
			}
		]
	},
	{
		name: "REST",
		description: "Returnerer restværdien ved en division.",
		arguments: [
			{
				name: "tal",
				description: "er det tal, resten efter en division skal findes for"
			},
			{
				name: "divisor",
				description: "er det tal, som tal skal divideres med"
			}
		]
	},
	{
		name: "ROMERTAL",
		description: "Konverterer et arabertal til et romertal, som tekst.",
		arguments: [
			{
				name: "tal",
				description: "er det arabertal, der skal konverteres"
			},
			{
				name: "format",
				description: "er et tal, der angiver, den ønskede type romertal."
			}
		]
	},
	{
		name: "RRI",
		description: "Returnerer en ækvivalent rente for væksten i en investering.",
		arguments: [
			{
				name: "nper",
				description: "er antal perioder for investeringen"
			},
			{
				name: "nv",
				description: "er nutidsværdien af investeringen"
			},
			{
				name: "fv",
				description: "er fremtidsværdien af investeringen"
			}
		]
	},
	{
		name: "RTD",
		description: "Indhenter realtidsdata fra et program, der understøtter COM automatisering.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "er navnet på ProgID'en til et registreret COM-automatiseringstilføjelsesprogram. Omslut navnet med anførselstegn"
			},
			{
				name: "server",
				description: "er navnet på den server, hvor tilføjelsesprogrammet skal køres. Omslut navnet med anførselstegn. Hvis tilføjelsesprogrammet køres lokalt, skal du bruge en tom streng"
			},
			{
				name: "emne1",
				description: "er 1 til 28 parametre, som angiver oplysninger"
			},
			{
				name: "emne2",
				description: "er 1 til 28 parametre, som angiver oplysninger"
			}
		]
	},
	{
		name: "RUND.NED",
		description: "Runder et tal ned (mod nul).",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal, der skal nedrundes"
			},
			{
				name: "antal_cifre",
				description: "er det antal decimaler, tallet skal afrundes til. Hvis det er negativt, skal der afrundes til venstre for kommaet, hvis det udelades eller er nul, skal der afrundes til det nærmeste heltal"
			}
		]
	},
	{
		name: "RUND.OP",
		description: "Runder et tal op (væk fra nul).",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal, der skal rundes op"
			},
			{
				name: "antal_cifre",
				description: "er det antal decimaler, tallet skal afrundes til. Hvis det er negativt, skal der afrundes til venstre for kommaet, hvis det udelades eller er nul, skal der afrundes til det nærmeste heltal"
			}
		]
	},
	{
		name: "SAK",
		description: "Returnerer summen af datapunkternes kvadrerede afvigelser fra stikprøvens middelværdi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 argumenter, en matrix eller en reference til en matrix, som SAK skal beregnes på"
			},
			{
				name: "tal2",
				description: "er 1-255 argumenter, en matrix eller en reference til en matrix, som SAK skal beregnes på"
			}
		]
	},
	{
		name: "SAMMENKÆDNING",
		description: "Sammenkæder flere tekststrenge til én tekststreng.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "er 1-255 tekststrenge, der skal kædes sammen til én tekststreng. Det kan være tekststrenge, tal eller referencer til enkelte celler"
			},
			{
				name: "tekst2",
				description: "er 1-255 tekststrenge, der skal kædes sammen til én tekststreng. Det kan være tekststrenge, tal eller referencer til enkelte celler"
			}
		]
	},
	{
		name: "SAMMENLIGN",
		description: "Returnerer den relative placering af et element i en matrix, som svarer til en angivet værdi i en angivet rækkefølge.",
		arguments: [
			{
				name: "opslagsværdi",
				description: "er den værdi, der skal bruges til at finde den ønskede værdi i matrixen, et tal, tekst, en logisk værdi eller en reference"
			},
			{
				name: "opslagsmatrix",
				description: "er et sammenhængende celleområde, der indeholder mulige opslagsværdier, en matrix med værdier eller en reference til en matrix"
			},
			{
				name: "sammenligningstype",
				description: "er et af tallene 1, 0 eller -1, som angiver, hvilken værdi der skal returneres."
			}
		]
	},
	{
		name: "SAND",
		description: "Returnerer den logiske værdi SAND.",
		arguments: [
		]
	},
	{
		name: "SANDSYNLIGHED",
		description: "Returnerer sandsynligheden for at værdier i et interval er mellem to grænser eller lig med en nedre grænse.",
		arguments: [
			{
				name: "x_variationsområde",
				description: "er udfaldsrummet for x med tilknyttede punktsandsynligheder"
			},
			{
				name: "sandsynligheder",
				description: "punktsandsynligheder for de mulige udfald for x. Er værdier mellem 0 og 1 bortset fra 0"
			},
			{
				name: "nedre_grænse",
				description: "er den nedre grænse for den værdi, du vil finde sandsynligheden for"
			},
			{
				name: "øvre_grænse",
				description: "er en valgfri øvre grænse for værdien. Hvis den udelades, returnerer SANDSYNLIGHED sandsynligheden for at en x-værdi er lig med nedre_grænse"
			}
		]
	},
	{
		name: "SEC",
		description: "Returnerer sekanten af en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, du vil have sekanten af"
			}
		]
	},
	{
		name: "SECH",
		description: "Returnerer den hyperbolske sekant af en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, du vil have den hyperbolske sekant af"
			}
		]
	},
	{
		name: "SEKUND",
		description: "Returnerer sekundet, et tal mellem 0 og 59.",
		arguments: [
			{
				name: "serienr",
				description: "er et tal i den dato- og klokkeslætskode, der anvendes af Spreadsheet, eller tekst i klokkeslætsformat, f.eks. 16:48:23 eller 04:48:47 PM"
			}
		]
	},
	{
		name: "SERIESUM",
		description: "Returnerer summen af potensserie, baseret på formlen.",
		arguments: [
			{
				name: "x",
				description: "er inputværdien for potensserien"
			},
			{
				name: "n",
				description: "er den oprindelige potens, x skal opløftes til"
			},
			{
				name: "m",
				description: "er den trinværdi n skal øges med for hvert udtryk i serien"
			},
			{
				name: "koefficienter",
				description: "er et sæt koefficienter, som hver efterfølgende potens af x ganges med"
			}
		]
	},
	{
		name: "SIN",
		description: "Returnerer sinus til en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, som sinus skal beregnes til. Grader * PI()/180 = radianer"
			}
		]
	},
	{
		name: "SINH",
		description: "Returnerer den hyperbolske sinus til et tal.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal"
			}
		]
	},
	{
		name: "SKÆRING",
		description: "Returnerer skæringsværdien på y-aksen i en lineær regression ud fra kendte x- og y-værdier.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er observationer på den afhængige variabel, som kan være tal, navne, lister eller referencer, der indeholder tal"
			},
			{
				name: "kendte_x'er",
				description: "er observationer på den forklarende variabel, som kan være tal, navne, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "SKÆVHED",
		description: "Returnerer skævheden af en distribution, dvs. en karakteristik af graden af asymmetri for en distribution omkring dens middelværdi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som skævheden skal beregnes for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som skævheden skal beregnes for"
			}
		]
	},
	{
		name: "SKÆVHED.P",
		description: "Returnerer skævheden af en distribution baseret på en population: en karakteristik af graden af asymmetri for en distribution omkring dens middelværdi.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1 til 254 tal eller navne, matrixer eller referencer, der indeholde de tal, du vil finde populationens skævhed for"
			},
			{
				name: "tal2",
				description: "er 1 til 254 tal eller navne, matrixer eller referencer, der indeholde de tal, du vil finde populationens skævhed for"
			}
		]
	},
	{
		name: "SLÅ.OP",
		description: "Søger efter værdier i en række, en kolonne eller en matrix. Sikrer kompatibilitet med ældre versioner.",
		arguments: [
			{
				name: "opslagsværdi",
				description: "er en værdi, som SLÅ.OP søger efter i opslagsvektoren, og som kan være et tal, tekst, en logisk værdi eller et navn på eller en reference til en værdi"
			},
			{
				name: "opslagsvektor",
				description: "er et område, der kun indeholder én række eller én kolonne med tekst, tal eller logiske værdier anbragt i stigende rækkefølge"
			},
			{
				name: "resultatvektor",
				description: "er et område, der kun indeholder én række eller én kolonne i samme størrelse som opslagsvektoren"
			}
		]
	},
	{
		name: "SLUMP",
		description: "Returnerer et tilfældigt tal mellem 0 og 1, jævnt fordelt (ændres ved ny beregning).",
		arguments: [
		]
	},
	{
		name: "SLUMPMELLEM",
		description: "Returnerer et tilfældigt tal mellem de tal, der angives.",
		arguments: [
			{
				name: "mindst",
				description: "er det mindste heltal, der kan returneres"
			},
			{
				name: "størst",
				description: "er det største heltal, der kan returneres"
			}
		]
	},
	{
		name: "SLUT.PÅ.MÅNED",
		description: "Returnerer serienummeret på den sidste dag i måneden, før eller efter et specificeret antal måneder.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datotal, der repræsenterer startdatoen"
			},
			{
				name: "måneder",
				description: "er antal måneder, før eller efter startdatoen"
			}
		]
	},
	{
		name: "SMÅ.BOGSTAVER",
		description: "Konverterer tekst til små bogstaver.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst, der skal konverteres til små bogstaver. Tegn, som ikke er bogstaver, konverteres ikke"
			}
		]
	},
	{
		name: "SØG",
		description: "Returnerer et tal, der repræsenterer placeringen af et tegn eller en tekststreng i en anden tekststreng, læst fra venstre mod højre (skelner ikke mellem store og små bogstaver).",
		arguments: [
			{
				name: "find_tekst",
				description: "er den tekst, der skal findes. Du kan bruge jokertegnene ? og  *. Brug ~? og ~* til at finde tegnene ? og *"
			},
			{
				name: "i_tekst",
				description: "er den tekst, der skal søges i efter find_tekst"
			},
			{
				name: "start_ved",
				description: "er placeringen af det tegn i i_tekst (talt fra venstre), hvor søgningen skal begynde. Sættes til1, hvis det udelades"
			}
		]
	},
	{
		name: "STANDARD.NORM.FORDELING",
		description: "Returnerer fordelingsfunktionen for standardnormalfordelingen (har en middelværdi på nul og en standardafvigelse på en).",
		arguments: [
			{
				name: "z",
				description: "er den værdi, fordelingen skal beregnes for"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi, der, hvis kumulativ er SAND, returnerer fordelingsfunktionen, og som, hvis kumulativ er FALSK, returnerer tæthedsfunktionen"
			}
		]
	},
	{
		name: "STANDARD.NORM.INV",
		description: "Returnerer den inverse fordelingsfunktion for standardnormalfordelingen (den har en middelværdi på nul og en standardafvigelse på en).",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til normalfordelingen. Tallet er større end eller lig med 0 og mindre end eller lig med 1"
			}
		]
	},
	{
		name: "STANDARDISER",
		description: "Returnerer en standardiseret værdi fra en distribution, karakteriseret ved en middelværdi og en standardafvigelse.",
		arguments: [
			{
				name: "x",
				description: "er den værdi, der skal normaliseres"
			},
			{
				name: "middelværdi",
				description: "er middelværdien for den stokastiske variabel"
			},
			{
				name: "standardafv",
				description: "er standardafvigelsen (et positivt tal) for den stokastiske variabel"
			}
		]
	},
	{
		name: "STANDARDNORMFORDELING",
		description: "Returnerer fordelingsfunktionen for standardnormalfordelingen (har en middelværdi på nul og en standardafvigelse på en).",
		arguments: [
			{
				name: "z",
				description: "er den værdi, fordelingen skal beregnes for"
			}
		]
	},
	{
		name: "STANDARDNORMINV",
		description: "Returnerer den inverse fordelingsfunktion for standardnormalfordelingen (den har en middelværdi på nul og en standardafvigelse på en).",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er sandsynligheden, der knytter sig til normalfordelingen. Tallet er større end eller lig med 0 og mindre end eller lig med1"
			}
		]
	},
	{
		name: "STATSOBLIGATION",
		description: "Returnerer det obligationsækvivalente afkast for en statsobligation.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er statsobligationens afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er statsobligationens udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "diskonto",
				description: "er statsobligationens diskontosats"
			}
		]
	},
	{
		name: "STATSOBLIGATION.AFKAST",
		description: "Returnerer statsobligationens afkast.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er statsobligationens afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er statsobligationens udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "kurs",
				description: "er statsobligationens kurs pr. kr. 100 i pålydende værdi"
			}
		]
	},
	{
		name: "STATSOBLIGATION.KURS",
		description: "Returnerer kursen pr. kr. 100 nominel værdi for en statsobligation.",
		arguments: [
			{
				name: "afregningsdato",
				description: "er statsobligationens afregningsdato, angivet som et serielt datotal"
			},
			{
				name: "udløbsdato",
				description: "er statsobligationens udløbsdato, angivet som et serielt datotal"
			},
			{
				name: "diskonto",
				description: "er statsobligationens diskontosats"
			}
		]
	},
	{
		name: "STDAFV",
		description: "Beregner standardafvigelsen på basis af en stikprøve (ignorerer logiske værdier og tekst i stikprøven).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, der svarer til en stikprøve fra en population og kan være tal eller referencer, som indeholder tal"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, der svarer til en stikprøve fra en population og kan være tal eller referencer, som indeholder tal"
			}
		]
	},
	{
		name: "STDAFV.P",
		description: "Beregner standardafvigelsen baseret på hele populationen, der er givet som argumenter (ignorerer logiske værdier og tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1 til 255 tal, der svarer til en  population og kan være tal eller referencer, der indeholder tal"
			},
			{
				name: "tal2",
				description: "er 1 til 255 tal, der svarer til en  population og kan være tal eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "STDAFV.S",
		description: "Estimerer standardafvigelsen baseret på en stikprøve (ignorerer logiske værdier og tekst i stikprøven).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1 til 255 tal, der svarer til en stikprøve af en population og kan være tal eller referencer, der indeholder tal"
			},
			{
				name: "tal2",
				description: "er 1 til 255 tal, der svarer til en stikprøve af en population og kan være tal eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "STDAFVP",
		description: "Beregner standardafvigelsen på basis af en hel population givet som argumenter (ignorerer logiske værdier og tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, svarende til en hel population. Det kan være tal eller referencer, som indeholder tal"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, svarende til en hel population. Det kan være tal eller referencer, som indeholder tal"
			}
		]
	},
	{
		name: "STDAFVPV",
		description: "Beregner standardafvigelsen på basis af en hel population, herunder logiske værdier og tekst.  Tekst og den logiske værdi FALSK har værdien 0, og den logiske værdi SAND har værdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 værdier svarende til en hel population. Det kan være værdier, tal, navne, matrixer eller referencer, der indeholder værdier"
			},
			{
				name: "værdi2",
				description: "er 1-255 værdier svarende til en hel population. Det kan være værdier, tal, navne, matrixer eller referencer, der indeholder værdier"
			}
		]
	},
	{
		name: "STDAFVV",
		description: "Beregner standardafvigelsen på basis af en stikprøve, herunder logiske værdier og tekst. Tekst og den logiske værdi FALSK har værdien 0, og den logiske værdi SAND har værdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 værdier svarende til en stikprøve fra en population. Det kan være værdier, navne eller referencer til værdier"
			},
			{
				name: "værdi2",
				description: "er 1-255 værdier svarende til en stikprøve fra en population. Det kan være værdier, navne eller referencer til værdier"
			}
		]
	},
	{
		name: "STFYX",
		description: "Returnerer standardafvigelsen på de estimerede y-værdier i den simple linære regression.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er observationer på den afhængige variabel, som kan være tal, navne, lister eller referencer, der indeholder tal"
			},
			{
				name: "kendte_x'er",
				description: "er observationer på den forklarende variabel. Det kan være tal, navne, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "STIGNING",
		description: "Returnerer estimatet på hældningen fra en simpel lineær regression ud fra de givne datapunkter.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er observationer på den afhængige variabel, som kan være tal, navne, lister eller referencer, der indeholder tal"
			},
			{
				name: "kendte_x'er",
				description: "er observationer på den forklarende variabel. Det kan være tal, navne, vektorer eller referencer, der indeholder tal"
			}
		]
	},
	{
		name: "STORE.BOGSTAVER",
		description: "Konverterer tekst til store bogstaver.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst, der skal konverteres til store bogstaver, en reference eller en tekststreng"
			}
		]
	},
	{
		name: "STØRSTE",
		description: "Returnerer den k'te-største værdi i et datasæt. For eksempel det femtestørste tal.",
		arguments: [
			{
				name: "matrix",
				description: "er den matrix eller det datainterval, som den k'te-største værdi skal bestemmes for"
			},
			{
				name: "k",
				description: "er den position (regnet fra største værdi) i matrixen eller celleområdet, hvorfra værdien skal returneres"
			}
		]
	},
	{
		name: "STØRSTE.FÆLLES.DIVISOR",
		description: "Returnerer den største fælles divisor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1 til 255 værdier"
			},
			{
				name: "tal2",
				description: "er 1 til 255 værdier"
			}
		]
	},
	{
		name: "STORT.FORBOGSTAV",
		description: "Konverterer første bogstav i hvert ord til stort og resten af teksten til små bogstaver.",
		arguments: [
			{
				name: "tekst",
				description: "er tekst i anførselstegn, en formel, der returnerer tekst, eller en reference til en celle, der indeholder den tekst, der delvist skal skrives med stort"
			}
		]
	},
	{
		name: "SUBTOTAL",
		description: "Returnerer en subtotal på en liste eller i en database.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funktion",
				description: "er et tal fra 1-11, der angiver, hvilken sumfunktion der skal bruges til at beregne subtotalen."
			},
			{
				name: "reference1",
				description: "er 1 til 254 områder eller referencer, som subtotalen skal beregnes for"
			}
		]
	},
	{
		name: "SUM",
		description: "Lægger alle tal i et celleområde sammen.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, der skal lægges sammen. Logiske værdier og tekst ignoreres i cellerne, også hvis de indtastes som argumenter"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, der skal lægges sammen. Logiske værdier og tekst ignoreres i cellerne, også hvis de indtastes som argumenter"
			}
		]
	},
	{
		name: "SUM.HVIS",
		description: "Tilføjer de celler, der er specificeret af en given betingelse eller et givet kriterium.",
		arguments: [
			{
				name: "område",
				description: "er det celleområde, der skal evalueres"
			},
			{
				name: "kriterier",
				description: "er betingelsen eller kriteriet i form af et tal, et udtryk eller tekst, der angiver, hvilke celler der summeres"
			},
			{
				name: "sum_område",
				description: "er de celler, der skal summeres. Hvis feltet ikke udfyldes, benyttes cellerne i området"
			}
		]
	},
	{
		name: "SUM.HVISER",
		description: "Adderer de celler, der er angivet med et givet sæt betingelser eller kriterier.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "sumområde",
				description: "er de faktiske celler, der skal adderes."
			},
			{
				name: "kriterieområde",
				description: "er det celleområde, der skal evalueres i forhold til den konkrete betingelse"
			},
			{
				name: "kriterier",
				description: "er betingelsen eller kriteriet i form af et tal, et udtryk eller tekst, der definerer de celler, som skal  adderes"
			}
		]
	},
	{
		name: "SUMKV",
		description: "Returnerer summen af kvadrater for argumenterne. Argumenterne kan være tal, matrixer, navne eller referencer til celler, der indeholder tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, matrixer, navne eller referencer til matrixer, som summen af kvadrater skal beregnes for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, matrixer, navne eller referencer til matrixer, som summen af kvadrater skal beregnes for"
			}
		]
	},
	{
		name: "SUMPRODUKT",
		description: "Returnerer summen af produkterne af tilsvarende områder eller matrixer.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrix1",
				description: "er 2-255 matrixer, hvis komponenter ønskes ganget og derefter lagt sammen. Alle matrixer skal have de samme dimensioner"
			},
			{
				name: "matrix2",
				description: "er 2-255 matrixer, hvis komponenter ønskes ganget og derefter lagt sammen. Alle matrixer skal have de samme dimensioner"
			},
			{
				name: "matrix3",
				description: "er 2-255 matrixer, hvis komponenter ønskes ganget og derefter lagt sammen. Alle matrixer skal have de samme dimensioner"
			}
		]
	},
	{
		name: "SUMX2MY2",
		description: "Opsummerer forskellene mellem kvadraterne af to tilsvarende områder eller matrixer.",
		arguments: [
			{
				name: "matrix_x",
				description: "er den første matrix eller det første værdinterval. Det kan være et tal, et navn, en matrix eller en reference, der indeholder tal"
			},
			{
				name: "matrix_y",
				description: "er den anden matrix eller det andet værdiinterval. Det kan være et tal, et navn, en matrix eller en reference, der indeholder tal"
			}
		]
	},
	{
		name: "SUMX2PY2",
		description: "Returnerer summen af summen af kvadraterne af værdier i to tilsvarende områder eller matrixer.",
		arguments: [
			{
				name: "matrix_x",
				description: "er den første matrix eller det første værdinterval. Det kan være et tal, et navn, en matrix eller en reference, der indeholder tal"
			},
			{
				name: "matrix_y",
				description: "er den anden matrix eller det andet værdiinterval. Det kan være et tal, et navn, en matrix eller en reference, der indeholder tal"
			}
		]
	},
	{
		name: "SUMXMY2",
		description: "Opsummerer kvadraterne af forskellene i to tilsvarende områder eller matrixer.",
		arguments: [
			{
				name: "array_x",
				description: "er den første matrix eller det første værdiinterval. Det kan være et tal, et navn, en matrix eller en reference, der indeholder tal"
			},
			{
				name: "array_y",
				description: "er den anden matrix eller det andet værdiinterval. Det kan være et tal, et navn, en matrix eller en reference, der indeholder tal"
			}
		]
	},
	{
		name: "T",
		description: "Undersøger, om en værdi er tekst, og returnerer teksten, hvis dette er tilfældet, eller returnerer dobbelte anførselstegn (en tom streng), hvis det ikke er tilfældet.",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der skal testes"
			}
		]
	},
	{
		name: "T.FORDELING",
		description: "Returnerer den venstresidede fordelingsfunktion for Students t-fordeling.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske værdi, som fordelingen skal evalueres for"
			},
			{
				name: "frihedsgrader",
				description: "er et heltal, der angiver antallet af frihedsgrader, som kendetegner fordelingen"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis SAND returneres fordelingsfunktionen. Hvis FALSK returneres punktsandsynligheden"
			}
		]
	},
	{
		name: "T.FORDELING.2T",
		description: "Returnerer den tosidede fordelingsfunktion for Students t-fordeling.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske værdi, som fordelingen skal evalueres for"
			},
			{
				name: "frihedsgrader",
				description: "er et heltal, der angiver antallet af frihedsgrader, som kendetegner fordelingen"
			}
		]
	},
	{
		name: "T.FORDELING.RT",
		description: "Returnerer den højresidede fordelingsfunktion for Students t-fordeling.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske værdi, som fordelingen skal evalueres for"
			},
			{
				name: "frihedsgrader",
				description: "er et heltal, der angiver antallet af frihedsgrader, som kendetegner fordelingen"
			}
		]
	},
	{
		name: "T.INV",
		description: "Returnerer den venstresidede inverse fordelingsfunktion for Students t-fordeling.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er den sandsynlighed, der knytter sig til den tosidede t-fordeling. Det er et tal mellem 0 og 1"
			},
			{
				name: "frihedsgrader",
				description: "ier et positivt heltal, der angiver antallet af frihedsgrader, som kendetegner fordelingen"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Returnerer den tosidede inverse fordelingsfunktion for Students t-fordeling.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er den sandsynlighed, der knytter sig til den tosidede t-fordeling. Det er et tal mellem 0 og 1"
			},
			{
				name: "frihedsgrader",
				description: "er et positivt heltal, der angiver antallet af frihedsgrader, som kendetegner fordelingen"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Returnerer den sandsynlighed, der knytter sig til en Students t-test.",
		arguments: [
			{
				name: "matrix1",
				description: "er det første datasæt"
			},
			{
				name: "matrix2",
				description: "er det andet datasæt"
			},
			{
				name: "haler",
				description: "angiver om sandsynligheden skal bestemmes en- eller tosidet: 1 = ensidet; 2 = tosidet"
			},
			{
				name: "type",
				description: "er typen af t-test: 1 = parvis, 2 = dobbelt stikprøve med ens varians, 3 = dobbelt stikprøve med forskellig varians"
			}
		]
	},
	{
		name: "TÆL",
		description: "Tæller antallet af celler i et område, der indeholder tal.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255-argumenter, der kan indeholde eller referere til flere forskellige datatyper, men kun tal tælles"
			},
			{
				name: "værdi2",
				description: "er 1-255-argumenter, der kan indeholde eller referere til flere forskellige datatyper, men kun tal tælles"
			}
		]
	},
	{
		name: "TÆL.HVIS",
		description: "Tæller antallet af celler i et område, der svarer til de givne betingelser.",
		arguments: [
			{
				name: "område",
				description: "er det område, hvor du vil finde antallet af ikke-tomme celler"
			},
			{
				name: "kriterier",
				description: "er betingelsen i form af et tal, et udtryk eller tekst, der angiver, hvilke celler der tælles"
			}
		]
	},
	{
		name: "TÆL.HVISER",
		description: "Tæller antallet af celler i et givet sæt betingelser eller kriterier.",
		arguments: [
			{
				name: "kriterieområde",
				description: "er det celleområde, der skal evalueres i forhold til den konkrete betingelse"
			},
			{
				name: "kriterier",
				description: "er betingelsen i form af et tal, et udtryk eller tekst, der definerer de celler, som skal tælles"
			}
		]
	},
	{
		name: "TÆLV",
		description: "Tæller antallet af celler i et område der ikke er tomme.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255-argumenter, der repræsenterer de værdier og celler, du vil tælle. Værdier kan være alle typer af oplysninger"
			},
			{
				name: "værdi2",
				description: "er 1-255-argumenter, der repræsenterer de værdier og celler, du vil tælle. Værdier kan være alle typer af oplysninger"
			}
		]
	},
	{
		name: "TAL",
		description: "Konverterer en ikke-numerisk værdi til et tal. Datoer konverteres til serienumre, SAND til 1, og alt andet til 0 (nul).",
		arguments: [
			{
				name: "værdi",
				description: "er den værdi, der skal konverteres"
			}
		]
	},
	{
		name: "TALVÆRDI",
		description: "Konverterer tekst til tal ifølge landestandarden.",
		arguments: [
			{
				name: "tekst",
				description: "er den streng, som repræsenterer det tal, du vil konvertere"
			},
			{
				name: "decimaltegn",
				description: "er det tegn, der bruges som decimaltegn i strengen"
			},
			{
				name: "gruppeseparator",
				description: "er det tegn, der bruges som tusindtalsseparator i strengen"
			}
		]
	},
	{
		name: "TAN",
		description: "Returnerer tangens til en vinkel.",
		arguments: [
			{
				name: "tal",
				description: "er den vinkel i radianer, som tangens skal beregnes til. Grader * PI()/180 = radianer"
			}
		]
	},
	{
		name: "TANH",
		description: "Returnerer hyperbolsk tangens til et tal.",
		arguments: [
			{
				name: "tal",
				description: "er et vilkårligt reelt tal"
			}
		]
	},
	{
		name: "TEKST",
		description: "Konverterer en værdi til tekst i et specifikt talformat.",
		arguments: [
			{
				name: "værdi",
				description: "er et tal, en formel, der evalueres til en numerisk værdi, eller en reference til en celle, der indeholder en numerisk værdi"
			},
			{
				name: "format",
				description: "er et talformat i form af tekst fra feltet Kategori under fanen Tal i dialogboksen Formatér celler (ikke Standard)"
			}
		]
	},
	{
		name: "TENDENS",
		description: "Returnerer værdier i en lineær tendens svarende til kendte datapunkter med brug af de mindste kvadraters metode.",
		arguments: [
			{
				name: "kendte_y'er",
				description: "er et område eller en matrix med kendte y-værdier i forholdet y = mx + b"
			},
			{
				name: "kendte_x'er",
				description: "er et område eller en matrix med kendte x-værdier i forholdet y = mx + b, dvs. en matrix af samme størrelse som Kendte_y'er"
			},
			{
				name: "nye_x'er",
				description: "er et område eller en matrix med nye x-værdier, som TENDENS skal returnere tilsvarende y-værdier for"
			},
			{
				name: "konstant",
				description: "er en logisk værdi: konstanten b beregnes normalt, hvis Konst = SAND eller udeladt; b sættes til lig 0, hvis Konst = FALSK"
			}
		]
	},
	{
		name: "TFORDELING",
		description: "Returnerer t-fordelingen for Student.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske værdi, som distributionen skal evalueres for"
			},
			{
				name: "frihedsgrader",
				description: "er et heltal, der angiver det antal frihedsgrader, der karakteriserer distributionen"
			},
			{
				name: "haler",
				description: "angiver om sandsynligheden skal bestemmes en- eller tosidet: 1 = ensidet; 2 = tosidet"
			}
		]
	},
	{
		name: "TID",
		description: "Konverterer timer, minutter og sekunder angivet som tal i et Spreadsheet-serienummer, der er formateret med et klokkeslætsformat.",
		arguments: [
			{
				name: "time",
				description: "er et tal mellem 0 og 23, der repræsenterer timen"
			},
			{
				name: "minut",
				description: "er et tal mellem 0 og 59, der repræsenterer minuttet"
			},
			{
				name: "sekund",
				description: "er et tal mellem 0 og 59, der repræsenterer sekundet"
			}
		]
	},
	{
		name: "TIDSVÆRDI",
		description: "Konverterer et klokkeslæt i form af tekst til et Spreadsheet-serienummer, et tal fra 0 (12:00:00 AM) til 0.999988426 (11:59:59 PM). Formatér tallet med et klokkeslætsformat efter angivelse af formlen.",
		arguments: [
			{
				name: "tid",
				description: "er en tekststreng, der indeholder et klokkeslæt i et af de klokkeslætsformater, der findes i Spreadsheet (datooplysninger i strengen ignoreres)"
			}
		]
	},
	{
		name: "TIME",
		description: "Returnerer timen som et tal mellem 0 (24:00) og 23 (23:00).",
		arguments: [
			{
				name: "serienr",
				description: "er et tal i den dato- og klokkeslætskode, der anvendes af Spreadsheet, eller tekst i klokkeslætsformat, f.eks. 16:48:00 eller 04:48:00 PM"
			}
		]
	},
	{
		name: "TINV",
		description: "Returnerer den tosidede inverse fordelingsfunktion for Students t-fordeling.",
		arguments: [
			{
				name: "sandsynlighed",
				description: "er den sandsynlighed, der knytter sig til den tosidede t-fordeling. Det er et tal mellem 0 og 1"
			},
			{
				name: "frihedsgrader",
				description: "er et positivt heltal, der angiver antallet af frihedsgrader til at karakterisere distributionen"
			}
		]
	},
	{
		name: "TOPSTEJL",
		description: "Returnerer kurtosisværdien af et datasæt.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som kurtosisværdien skal beregnes for"
			},
			{
				name: "tal2",
				description: "er 1-255 tal, navne, matrixer eller referencer, der indeholder tal, som kurtosisværdien skal beregnes for"
			}
		]
	},
	{
		name: "TRANSPONER",
		description: "Konverterer et lodret celleområde til et vandret område eller omvendt.",
		arguments: [
			{
				name: "matrix",
				description: "er et celleområde i et regneark eller en matrix, som skal transponeres"
			}
		]
	},
	{
		name: "TRIMMIDDELVÆRDI",
		description: "Returnerer det trimmede gennemsnit for et datasæt.",
		arguments: [
			{
				name: "vektor",
				description: "er det interval eller den vektor, som det trimmede gennemsnit skal findes for"
			},
			{
				name: "procent",
				description: "er den procentvise andel af data, som skal ekskluderes fra toppen og bunden af datasættet inden beregning"
			}
		]
	},
	{
		name: "TTEST",
		description: "Returnerer den sandsynlighed, der knytter sig til en t-test.",
		arguments: [
			{
				name: "vektor1",
				description: "er det første datasæt"
			},
			{
				name: "vektor2",
				description: "er det andet datasæt"
			},
			{
				name: "haler",
				description: "angiver om sandsynligheden skal bestemmes en- eller tosidet: 1 = ensidet; 2 = tosidet"
			},
			{
				name: "type",
				description: "er typen af t-test: 1 = parvis, 2 = dobbelt stikprøve med ens varians, 3 = dobbelt stikprøve med forskellig varians"
			}
		]
	},
	{
		name: "UDSKIFT",
		description: "Erstatter gammel tekst med ny tekst i en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekst eller den reference til en celle, der indeholder den tekst, hvor der skal udskiftes tegn"
			},
			{
				name: "gammel_tekst",
				description: "er den eksisterende tekst, du vil erstatte. Hvis fordelingen af store og små bogstaver ikke er den samme i Gammel_tekst og tekst, vil UDSKIFT ikke erstatte teksten"
			},
			{
				name: "ny_tekst",
				description: "er den tekst, der skal indsættes i stedet for gammel_tekst"
			},
			{
				name: "forekomst",
				description: "angiver, hvilken forekomst af gammel_tekst der skal udskiftes. Hvis det ikke udfyldes, bliver alle forekomster af gammel_tekst udskiftet"
			}
		]
	},
	{
		name: "UGE.NR",
		description: "Konverterer ugenummeret i året.",
		arguments: [
			{
				name: "serienr",
				description: "er den dato-/tidskode, der bruges af Spreadsheet til dato- og tidsberegninger"
			},
			{
				name: "type",
				description: "er et tal (1 eller 2), som bestemmer returværdiens type"
			}
		]
	},
	{
		name: "UGEDAG",
		description: "Returnerer et tal mellem 1 og 7, som repræsenterer ugedagen i datoen.",
		arguments: [
			{
				name: "serienr",
				description: "er et tal, der repræsenterer en dato"
			},
			{
				name: "type",
				description: "er et tal: for søndag=1 til og med lørdag=7 bruges 1; for mandag=1 til og med søndag=7 bruges 2; for mandag=0 til og med søndag=6 bruges 3"
			}
		]
	},
	{
		name: "ULIGE",
		description: "Runder positive tal op og negative tal ned til nærmeste ulige heltal.",
		arguments: [
			{
				name: "tal",
				description: "er den værdi, der skal afrundes"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Returnerer det tal (tegnværdi), der svarer til det første tegn i teksten.",
		arguments: [
			{
				name: "tekst",
				description: "er det tegn, du vil have Unicode-værdien for"
			}
		]
	},
	{
		name: "VÆLG",
		description: "Vælger en værdi eller en handling fra en liste over værdier baseret på et indeksnummer.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indeksnr",
				description: "angiver, hvilket værdiargument der skal vælges. Indeksnr. skal enten være mellem 1 og 254, være en formel eller være en reference til et tal mellem 1 og 254"
			},
			{
				name: "værdi1",
				description: "er 1-254 tal, cellereferencer, definerede navne, formler, funktioner eller tekst, som VÆLG vælger fra"
			},
			{
				name: "værdi2",
				description: "er 1-254 tal, cellereferencer, definerede navne, formler, funktioner eller tekst, som VÆLG vælger fra"
			}
		]
	},
	{
		name: "VÆRDI",
		description: "Konverterer en tekststreng til et tal.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten i anførselstegn eller en reference til en celle, der indeholder den tekst, der skal konverteres"
			}
		]
	},
	{
		name: "VÆRDITYPE",
		description: "Returnerer et heltal, der repræsenterer datatypen for en værdi: tal = 1; tekst = 2; logisk værdi = 4; fejlværdi = 16; matrix = 64.",
		arguments: [
			{
				name: "værdi",
				description: "kan være enhver værdi"
			}
		]
	},
	{
		name: "VARIANS",
		description: "Beregner variansen baseret på en stikprøve (ignorerer logiske værdier og tekst i stikprøven).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 numeriske argumenter, svarende til en stikprøve fra populationen"
			},
			{
				name: "tal2",
				description: "er 1-255 numeriske argumenter, svarende til en stikprøve fra populationen"
			}
		]
	},
	{
		name: "VARIANS.P",
		description: "Beregner variansen baseret på hele populationen (ignorerer logiske værdier og tekst i populationen).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er mellem 1 og 255 numeriske argumenter, der svarer til en population"
			},
			{
				name: "tal2",
				description: "er mellem 1 og 255 numeriske argumenter, der svarer til en population"
			}
		]
	},
	{
		name: "VARIANS.S",
		description: "Beregner variansen baseret på en stikprøve (ignorerer logiske værdier og tekst i stikprøven).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er mellem 1 og 255 numeriske argumenter, der svarer til en stikprøve fra en population"
			},
			{
				name: "tal2",
				description: "er mellem 1 og 255 numeriske argumenter, der svarer til en stikprøve fra en population"
			}
		]
	},
	{
		name: "VARIANSP",
		description: "Beregner variansen baseret på en hel population (ignorerer logiske værdier og tekst i populationen).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tal1",
				description: "er 1-255 numeriske argumenter, svarende til en hel population"
			},
			{
				name: "tal2",
				description: "er 1-255 numeriske argumenter, svarende til en hel population"
			}
		]
	},
	{
		name: "VARIANSPV",
		description: "Beregner variansen baseret på en hel population,  herunder logiske værdier og tekst. Tekst og den logiske værdi FALSK har værdien 0, og den logiske værdi SAND har værdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 værdiargumenter svarende til en hel population"
			},
			{
				name: "værdi2",
				description: "er 1-255 værdiargumenter svarende til en hel population"
			}
		]
	},
	{
		name: "VARIANSV",
		description: "Beregner variansen baseret på en stikprøve, herunder logiske værdier og tekst. Tekst og den logiske værdi FALSK har værdien 0, og den logiske værdi SAND har værdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "værdi1",
				description: "er 1-255 værdiargumenter svarende til en stikprøve fra en population"
			},
			{
				name: "værdi2",
				description: "er 1-255 værdiargumenter svarende til en stikprøve fra en population"
			}
		]
	},
	{
		name: "VENSTRE",
		description: "Returnerer det angivne antal tegn fra begyndelsen af en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er den tekststreng, der indeholder de tegn, du vil uddrage"
			},
			{
				name: "antal_tegn",
				description: "angiver, hvor mange tegn VENSTRE skal uddrage. Sættes til 1, hvis feltet ikke udfyldes"
			}
		]
	},
	{
		name: "VOPSLAG",
		description: "Søger efter en bestemt værdi i øverste række af en tabel eller i en matrix og returnerer værdien i den samme kolonne fra en række, som du angiver.",
		arguments: [
			{
				name: "opslagsværdi",
				description: "angiver værdien i første række af tabellen og kan være en værdi, en reference eller en tekststreng"
			},
			{
				name: "tabelmatrix",
				description: "angiver en tabel med tekst, tal eller logiske værdier, hvor man kan slå data op. Tabelmatrixkan være en reference til et område eller områdenavn"
			},
			{
				name: "rækkeindeks",
				description: "angiver rækkenummeret i en tabel_matrix, hvorfra den tilsvarende værdi skal returneres. Den første række af værdier er i tabellens første række"
			},
			{
				name: "lig_med",
				description: "angiver en logisk værdi: find det nærmeste kriterium i den øverste række (sorteret i stigende rækkefølge) = SAND eller udelades; find er en nøjagtig kopi = FALSK"
			}
		]
	},
	{
		name: "VSA",
		description: "Returnerer afskrivningen på et aktiv i en specificeret periode, herunder delperioder, vha. dobbeltsaldometoden, eller en anden metode, du angiver.",
		arguments: [
			{
				name: "købspris",
				description: "er aktivets kostpris"
			},
			{
				name: "restværdi",
				description: "er aktivets værdi ved afskrivningens afslutning"
			},
			{
				name: "levetid",
				description: "er antallet af afskrivningsperioder (aktivets levetid)"
			},
			{
				name: "startperiode",
				description: "er startperioden, for hvilken afskrivningen skal beregnes, med de samme enheder som levetid"
			},
			{
				name: "slutperiode",
				description: "er slutperioden, for hvilken afskrivningen skal beregnes, med d e samme enheder som levetid"
			},
			{
				name: "faktor",
				description: "er satsen, med hvilken restbeløbet afskrives. Hvis intet anføres benyttes dobbeltsaldometoden"
			},
			{
				name: "ingen_skift",
				description: "FALSK eller udeladt = skift til lineær afskrivning, hvis afskrivningen er større end saldoen; SAND = undlad at skifte"
			}
		]
	},
	{
		name: "WEIBULL",
		description: "Returnerer Weibull-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), funktionen skal evalueres for"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "kumulativ",
				description: "er en logisk værdi. Hvis SAND, returneres fordelingsfunktionen. Hvis FALSK, returneres sandsynlighedsfunktionen"
			}
		]
	},
	{
		name: "WEIBULL.FORDELING",
		description: "Returnerer Weibull-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den værdi (et ikke-negativt tal), funktionen skal evalueres for"
			},
			{
				name: "alpha",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "beta",
				description: "er en parameter til fordelingen (et positivt tal)"
			},
			{
				name: "kumulativ",
				description: "Hvis SAND, returneres fordelingsfunktionen. Hvis FALSK, returneres sandsynlighedsfunktionen"
			}
		]
	},
	{
		name: "XELLER",
		description: "Returnerer et logisk 'Eksklusivt eller' for alle argumenterne.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "er 1 til 254 betingelser, du vil teste, og som kan være enten SAND eller FALSK og kan være logiske værdier, matrixer eller referencer"
			},
			{
				name: "logisk2",
				description: "er 1 til 254 betingelser, du vil teste, og som kan være enten SAND eller FALSK og kan være logiske værdier, matrixer eller referencer"
			}
		]
	},
	{
		name: "YDELSE",
		description: "Beregner ydelsen på et lån baseret på konstante ydelser og en konstant rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen i hver periode. Brug for eksempel 6%/4 om kvartårlige ydelser på 6% APR"
			},
			{
				name: "nper",
				description: "er det samlede antal ydelser på lånet"
			},
			{
				name: "nv",
				description: "er nutidsværdien, dvs. den samlede værdi, som en række fremtidige ydelser er værd nu"
			},
			{
				name: "fv",
				description: "er den fremtidige værdi eller den kassebalance, der ønskes opnået, når den sidste ydelse er betalt. Sættes til 0 (nul), hvis den udelades"
			},
			{
				name: "type",
				description: "er en logisk værdi: ydelse i begyndelsen af perioden = 1; ydelse i slutningen af perioden = 0 eller udeladt"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Returnerer den ensidige P-værdi for en z-test.",
		arguments: [
			{
				name: "matrix",
				description: "er matrixen eller dataområdet, som X skal testes i forhold til"
			},
			{
				name: "x",
				description: "er den værdi, der skal testes"
			},
			{
				name: "sigma",
				description: "er standardafvigelsen for populationen (kendt). Hvis den ikke angives, bruges standardafvigelsen for stikprøven"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Returnerer den tosidede P-værdi til en z-test.",
		arguments: [
			{
				name: "vektor",
				description: "er den matrix eller det dataområde, som X skal testes mod"
			},
			{
				name: "x",
				description: "er den værdi, der skal testes"
			},
			{
				name: "sigma",
				description: "er populationens standardafvigelse, der forudsættes kendt. Hvis den udelades, benyttes stikprøvens standardafvigelse"
			}
		]
	}
];