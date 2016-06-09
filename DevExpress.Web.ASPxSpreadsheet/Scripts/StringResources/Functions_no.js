ASPxClientSpreadsheet.Functions = [
	{
		name: "ABS",
		description: "Returnerer absoluttverdien til et tall, et tall uten fortegn.",
		arguments: [
			{
				name: "tall",
				description: "er det reelle tallet du vil ha absoluttverdien til"
			}
		]
	},
	{
		name: "ACOT",
		description: "Returnerer arccotangens for et tall i radianer i området 0 til pi.",
		arguments: [
			{
				name: "tall",
				description: "er cotangens til vinkelen du ønsker"
			}
		]
	},
	{
		name: "ACOTH",
		description: "Returnerer invers hyperbolsk cotangens for et tall.",
		arguments: [
			{
				name: "tall",
				description: "er hyperbolsk cotangens til vinkelen du ønsker"
			}
		]
	},
	{
		name: "ADRESSE",
		description: "Lager en cellereferanse som tekst, når det angis rad- og kolonnenumre.",
		arguments: [
			{
				name: "rad_nr",
				description: "er radnummeret som skal brukes i cellereferansen: Rad_nummer = 1 for rad 1"
			},
			{
				name: "kolonne_nr",
				description: "er kolonnenummeret du vil bruke i cellereferansen, for eksempel 4 for kolonne D"
			},
			{
				name: "abs",
				description: "angir referansetypen: Absolutt = 1, absolutt rad/relativ kolonne = 2, relativ rad/absolutt kolonne = 3, relativ = 4"
			},
			{
				name: "a1",
				description: "er en logisk funksjon som angir referansestil: A1-stil = 1 eller SANN, R1C1-stil = 0 eller USANN"
			},
			{
				name: "regneark",
				description: "er tekst som angir navnet på regnearket som skal brukes som ekstern referanse"
			}
		]
	},
	{
		name: "AMORT",
		description: "Returnerer innbetaling på hovedstolen for en gitt investering basert på periodiske, konstante innbetalinger og en fast rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode. Bruk for eksempel 6%/4 for kvartalsvise betalinger med 6 % årlig rente"
			},
			{
				name: "periode",
				description: "angir perioden, og må ligge mellom 1 og antall_innbet"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalingsperioder i en investering"
			},
			{
				name: "nåverdi",
				description: "er nåverdien, eller det totale beløpet som en serie fremtidige innbetalinger er verdt i dag"
			},
			{
				name: "sluttverdi",
				description: "er fremtidig verdi, eller kontantbalansen du vil oppnå etter at siste innbetaling er foretatt"
			},
			{
				name: "type",
				description: "er en logisk verdi. Innbetaling ved begynnelsen av perioden = 1, innbetaling ved slutten av perioden = 0 eller utelatt"
			}
		]
	},
	{
		name: "ANTALL",
		description: "Teller antall celler i et område som inneholder tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 argumenter som kan inneholde eller referere til forskjellige datatyper, men bare tall telles"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 argumenter som kan inneholde eller referere til forskjellige datatyper, men bare tall telles"
			}
		]
	},
	{
		name: "ANTALL.ARK",
		description: "Returnerer antall ark i en referanse.",
		arguments: [
			{
				name: "referanse",
				description: "er en referanse som du vil vite antall ark i. Hvis argumentet utelates, returneres antall ark i arbeidsboken som inneholder funksjonen"
			}
		]
	},
	{
		name: "ANTALL.HVIS",
		description: "Teller antall celler som oppfyller det gitte vilkåret, i et område.",
		arguments: [
			{
				name: "område",
				description: "er området du vil telle antall utfylte celler i"
			},
			{
				name: "vilkår",
				description: "er vilkåret i form av et tall, uttrykk eller en tekst som definerer hvilke celler som telles"
			}
		]
	},
	{
		name: "ANTALL.HVIS.SETT",
		description: "Teller antall celler som angis av et gitt sett med vilkår eller kriterier.",
		arguments: [
			{
				name: "kriterieområde",
				description: "er celleområdet du vil evaluere for det spesifikke vilkåret"
			},
			{
				name: "kriterium",
				description: "er kriteriet i form av tall, uttrykk eller tekst som angir hvilke celler som skal telles"
			}
		]
	},
	{
		name: "ANTALLA",
		description: "Teller hvor mange celler i et intervall som ikke er tomme.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 argumenter som representerer verdiene og cellene du vil telle. Verdiene kan være av en hvilken som helst type"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 argumenter som representerer verdiene og cellene du vil telle. Verdiene kan være av en hvilken som helst type"
			}
		]
	},
	{
		name: "ÅR",
		description: "Returnerer årstallet, et heltall i intervallet 1900 - 9999.",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato eller et klokkeslett som brukes av Spreadsheet"
			}
		]
	},
	{
		name: "ARABISK",
		description: "Konverterer et romertall til et arabisk tall.",
		arguments: [
			{
				name: "tekst",
				description: "er romertallet du vil konvertere"
			}
		]
	},
	{
		name: "ARBEIDSDAG",
		description: "returnerer serienummeret for datoen før eller etter et angitt antall arbeidsdager.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datonummer som representerer startdatoen"
			},
			{
				name: "dager",
				description: "er antall dager som ikke er helge- eller helligdager før eller etter startdato"
			},
			{
				name: "ekstra_feriedager",
				description: "er en valgfri liste med én eller flere datoer som skal utelates fra arbeidskalenderen, for eksempel nasjonale helligdager og flytende helligdager"
			}
		]
	},
	{
		name: "ARBEIDSDAG.INTL",
		description: "Returnerer serienummeret for datoen før eller etter et angitt antall arbeidsdager med egendefinerte helgeparametere.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datonummer som representerer startdatoen"
			},
			{
				name: "sluttdato",
				description: "er antall ikke-helgedager og ikke-helligdager før eller etter startdatoen"
			},
			{
				name: "helg",
				description: "er et tall eller en streng som angir når det er helg"
			},
			{
				name: "helligdager",
				description: "er en valgfri matrise med ett eller flere seriedatonumre som du kan utelate fra arbeidskalenderen, for eksempel faste og flytende helligdager"
			}
		]
	},
	{
		name: "ARCCOS",
		description: "Returnerer arccosinus til et tall, i radianer i intervallet 0 til Pi. Arccosinus er vinkelen hvis cosinus = Tall.",
		arguments: [
			{
				name: "tall",
				description: "er cosinus til vinkelen du ønsker, og må ligge mellom -1 og 1"
			}
		]
	},
	{
		name: "ARCCOSH",
		description: "Returnerer den inverse hyperbolske cosinus til et tall.",
		arguments: [
			{
				name: "tall",
				description: "er et hvilket som helst reelt tall som er større enn eller lik 1"
			}
		]
	},
	{
		name: "ARCSIN",
		description: "Returnerer arcsinus til et tall i radianer, i området -Pi/2 til Pi/2.",
		arguments: [
			{
				name: "tall",
				description: "er sinus til vinkelen, og må være mellom -1 og 1"
			}
		]
	},
	{
		name: "ARCSINH",
		description: "Returnerer den inverse hyperbolske sinus til et tall.",
		arguments: [
			{
				name: "tall",
				description: "er et hvilket som helst reelt tall som er større enn eller lik 1"
			}
		]
	},
	{
		name: "ARCTAN",
		description: "Returnerer arctangens til et tall i radianer, i området -Pi/2 til Pi/2.",
		arguments: [
			{
				name: "tall",
				description: "er tangens til den vinkelen du ønsker"
			}
		]
	},
	{
		name: "ARCTAN2",
		description: "Returnerer arctangens til x- og y-koordinatene i radianer, i området fra -Pi til Pi, unntatt -Pi.",
		arguments: [
			{
				name: "x",
				description: "er x-koordinatet til punktet"
			},
			{
				name: "y",
				description: "er y-koordinatet til punktet"
			}
		]
	},
	{
		name: "ARCTANH",
		description: "Returnerer den inverse hyperbolske tangens til et tall.",
		arguments: [
			{
				name: "tall",
				description: "er ethvert reelt tall mellom -1 og 1 unntatt -1 og 1"
			}
		]
	},
	{
		name: "ÅRDEL",
		description: "Returnerer delen av året som representeres av antall hele dager mellom startdato og sluttdato.",
		arguments: [
			{
				name: "startdato",
				description: "er serienummeret for en dato som representerer startdatoen"
			},
			{
				name: "sluttdato",
				description: "er serienummeret som representerer sluttdatoen"
			},
			{
				name: "basis",
				description: "er typen datosystem som skal brukes"
			}
		]
	},
	{
		name: "ARK",
		description: "Returnerer arknummeret for arket det refereres til.",
		arguments: [
			{
				name: "verdi",
				description: "er navnet på et ark eller en referanse som du ønsker arknummeret til. Hvis argumentet utelates, returneres nummeret til arket som inneholder funksjonen"
			}
		]
	},
	{
		name: "ÅRSAVS",
		description: "Returnerer årsavskrivningen for et aktivum i en angitt periode.",
		arguments: [
			{
				name: "kostnad",
				description: "er den opprinnelige kostnaden til aktivumet"
			},
			{
				name: "restverdi",
				description: "er verdien ved slutten av avskrivningen"
			},
			{
				name: "levetid",
				description: "er antall perioder et aktivum blir avskrevet over (ofte kalt aktivumets økonomiske levetid)"
			},
			{
				name: "periode",
				description: "er perioden, og må oppgis i samme enhet som levetid"
			}
		]
	},
	{
		name: "AVDRAG",
		description: "Beregner innbetalinger for et lån basert på konstante innbetalinger og en fast rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode for lånet. Bruk for eksempel 6%/4 for kvartalsvise betalinger på 6 % årlig rente"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalinger for lånet"
			},
			{
				name: "nåverdi",
				description: "er nåverdien, eller det totale beløpet som en serie fremtidige innbetalinger er verdt i dag"
			},
			{
				name: "sluttverdi",
				description: "er fremtidig verdi, eller kontantbalansen du vil oppnå etter at siste innbetaling er foretatt. Settes til 0 (null) hvis argumentet utelates"
			},
			{
				name: "type",
				description: "er en logisk verdi. Innbetaling ved begynnelsen av perioden = 1, innbetaling ved slutten av perioden = 0 eller utelatt"
			}
		]
	},
	{
		name: "AVKAST.DISKONTERT",
		description: "Returnerer den årlige avkastningen for et diskontert verdipapir. For eksempel en statsobligasjon.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "pris",
				description: "er verdipapirets pris per pålydende kr 100"
			},
			{
				name: "innløsningsverdi",
				description: "er verdipapirets innløsningsverdi per pålydende kr 100"
			},
			{
				name: "basis",
				description: "angir typen datosystem som brukes"
			}
		]
	},
	{
		name: "AVKORT",
		description: "Avrunder et tall nedover ved å fjerne alle desimaler over et angitt antall (et positivt tall) eller runder et angitt antall sifre til venstre for kommaet ned til null (et negativt tall).",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil avkorte (avrunde nedover)"
			},
			{
				name: "antall_sifre",
				description: "er et tall som angir antallet desimalplasser som skal beholdes. Settes til 0 (null) hvis argumentet utelates"
			}
		]
	},
	{
		name: "AVRUND",
		description: "Runder av et tall til et angitt antall sifre.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil runde av"
			},
			{
				name: "antall_sifre",
				description: "er antall sifre du vil runde av tallet til. Et negativt tall runder av til venstre for desimalkommaet, null til nærmeste heltall"
			}
		]
	},
	{
		name: "AVRUND.GJELDENDE.MULTIPLUM",
		description: "Runder av et tall oppover til nærmeste multiplum av en faktor.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du vil runde av"
			},
			{
				name: "gjeldende_multiplum",
				description: "er multiplumet du vil runde av tall til et multiplum av"
			}
		]
	},
	{
		name: "AVRUND.GJELDENDE.MULTIPLUM.NED",
		description: "Runder av et tall nedover til nærmeste signifikante multiplum av en faktor.",
		arguments: [
			{
				name: "tall",
				description: "er den numeriske verdien du vil runde av"
			},
			{
				name: "gjeldende_multiplum",
				description: "er faktoren du vil runde av tall ned til et multiplum av. Tall og faktor må enten begge være positive eller begge være negative"
			}
		]
	},
	{
		name: "AVRUND.GJELDENDE.MULTIPLUM.NED.MATEMATISK",
		description: "Runder av et tall nedover til nærmeste heltall eller til nærmeste signifikante multiplum av en faktor.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du vil runde av"
			},
			{
				name: "gjeldende_multiplum",
				description: "er multiplumet du vil runde av tall til et multiplum av"
			},
			{
				name: "modus",
				description: "når dette argumentet er gitt og ikke null, avrunder denne funksjonen mot null"
			}
		]
	},
	{
		name: "AVRUND.GJELDENDE.MULTIPLUM.NED.PRESIS",
		description: "Runder av et tall nedover til nærmeste heltall eller til nærmeste signifikante multiplum av en faktor.",
		arguments: [
			{
				name: "tall",
				description: "er den numeriske verdien du vil avrunde"
			},
			{
				name: "gjeldende_multiplum",
				description: "er multiplumet du vil avrunde tall til et multiplum av"
			}
		]
	},
	{
		name: "AVRUND.GJELDENDE.MULTIPLUM.OPP.MATEMATISK",
		description: "Runder av et tall oppover til nærmeste heltall eller til nærmeste signifikante multiplum av en faktor.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du vil runde av"
			},
			{
				name: "gjeldende_multiplum",
				description: "er multiplumet du vil runde av tall til et multiplum av"
			},
			{
				name: "modus",
				description: "når dette argumentet er gitt og ikke null, avrunder denne funksjonen bort fra null"
			}
		]
	},
	{
		name: "AVRUND.GJELDENDE.MULTIPLUM.PRESIS",
		description: "Runder av et tall oppover til nærmeste heltall eller til nærmeste signifikante multiplum av en faktor.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du vil runde av"
			},
			{
				name: "gjeldende_multiplum",
				description: "er multiplumet du vil runde av tall til et multiplum av"
			}
		]
	},
	{
		name: "AVRUND.NED",
		description: "Runder av et tall nedover mot null.",
		arguments: [
			{
				name: "tall",
				description: "er et reelt tall du vil runde av nedover"
			},
			{
				name: "antall_sifre",
				description: "er antall sifre du vil runde av tallet til. En negativ verdi runder av til venstre for desimalkommaet, null eller utelatt til nærmeste heltall"
			}
		]
	},
	{
		name: "AVRUND.OPP",
		description: "Runder av et tall oppover, bort fra null.",
		arguments: [
			{
				name: "tall",
				description: "er et reelt tall du vil runde av oppover"
			},
			{
				name: "antall_sifre",
				description: "er antall sifre du vil runde av tallet til. En negativ verdi runder av til venstre for desimalkommaet, null eller utelatt til nærmeste heltall"
			}
		]
	},
	{
		name: "AVRUND.TIL.ODDETALL",
		description: "Runder av et positivt tall oppover og et negativt tall nedover til nærmeste heltall som er et oddetall.",
		arguments: [
			{
				name: "tall",
				description: "er verdien som skal rundes av"
			}
		]
	},
	{
		name: "AVRUND.TIL.PARTALL",
		description: "Runder av et positivt tall oppover og et negativt tall nedover til nærmeste heltall som er et partall.",
		arguments: [
			{
				name: "tall",
				description: "er verdien som skal rundes av"
			}
		]
	},
	{
		name: "AVVIK.KVADRERT",
		description: "Returnerer summen av datapunkters kvadrerte avvik fra utvalgsgjennomsnittet.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 argumenter, eller en matrise eller matrisereferanse, som du vil beregne med AVVIK.KVADRERT"
			},
			{
				name: "tall2",
				description: "er 1 til 255 argumenter, eller en matrise eller matrisereferanse, som du vil beregne med AVVIK.KVADRERT"
			}
		]
	},
	{
		name: "BAHTTEKST",
		description: "Konverterer et tall til tekst (baht).",
		arguments: [
			{
				name: "tall",
				description: "er et tall du vil konvertere"
			}
		]
	},
	{
		name: "BESSELI",
		description: "Returnerer Besselfunksjonen In(x).",
		arguments: [
			{
				name: "x",
				description: "er verdien funksjonen skal evalueres etter"
			},
			{
				name: "n",
				description: "er ordenen til Besselfunksjonen"
			}
		]
	},
	{
		name: "BESSELJ",
		description: "Returnerer Besselfunksjonen Jn(x).",
		arguments: [
			{
				name: "x",
				description: "er verdien du evaluerer funksjonen etter"
			},
			{
				name: "n",
				description: "er ordenen til Besselfunksjonen"
			}
		]
	},
	{
		name: "BESSELK",
		description: "Returnerer den modifiserte Besselfunksjonen Kn(x).",
		arguments: [
			{
				name: "x",
				description: "er verdien funksjonen skal evalueres etter"
			},
			{
				name: "n",
				description: "er ordenen til Besselfunksjonen"
			}
		]
	},
	{
		name: "BESSELY",
		description: "Returnerer Besselfunksjonen Yn(x).",
		arguments: [
			{
				name: "x",
				description: "er verdien funksjonen skal evalueres etter"
			},
			{
				name: "n",
				description: "er ordenen til Besselfunksjonen"
			}
		]
	},
	{
		name: "BETA.FORDELING",
		description: "Returnerer den kumulative betafordelingsfunksjonen for sannsynlig tetthet.",
		arguments: [
			{
				name: "x",
				description: "er en verdi mellom A og B du vil regne ut funksjonen for"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "A",
				description: "er en valgfri nedre grense for x-intervallet. Hvis argumentet utelates, settes A til 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grense for x-intervallet. Hvis argumentet utelates, settes B til 1"
			}
		]
	},
	{
		name: "BETA.FORDELING.N",
		description: "Returnerer betafunksjonen for sannsynlig fordeling.",
		arguments: [
			{
				name: "x",
				description: "er en verdi mellom A og B som funksjonen skal evalueres for"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi: Bruk SANN for kumulativ fordeling, bruk USANN for sannsynlig tetthet"
			},
			{
				name: "A",
				description: "er en valgfri nedre grense for x-intervallet. Hvis argumentet utelates, settes A lik 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grense for x-intervallet. Hvis argumentet utelates, settes B lik 1"
			}
		]
	},
	{
		name: "BETA.INV",
		description: "Returnerer den inverse av den kumulative betafunksjonen for sannsynlig tetthet (BETA.FORDELING.N).",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til betafordelingen"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "A",
				description: "er en valgfri nedre grense for x-intervallet. Hvis argumentet utelates, settes A lik 0"
			},
			{
				name: "B",
				description: " er en valgfri øvre grense for x-intervallet. Hvis argumentet utelates, settes B lik 1"
			}
		]
	},
	{
		name: "BINOM.FORDELING",
		description: "Returnerer punktsannsynlighet eller kumulativ sannsynlighet.",
		arguments: [
			{
				name: "tall_s",
				description: "er antallet vellykkede forsøk"
			},
			{
				name: "forsøk",
				description: "er antallet uavhengige forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for å lykkes i hvert forsøk"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For den kumulative fordelingsfunksjonen bruker du SANN. For punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "BINOM.FORDELING.N",
		description: "Returnerer den individuelle binomiske sannsynlighetsfordelingen.",
		arguments: [
			{
				name: "antall_s",
				description: "er antall vellykkede forsøk"
			},
			{
				name: "forsøk",
				description: "er antall uavhengige forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for å lykkes i hvert forsøk"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For den kumulative fordelingsfunksjonen bruker du SANN, for punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "BINOM.FORDELING.OMRÅDE",
		description: "Returnerer sannsynligheten for et forsøksresultat ved hjelp av en binomisk fordeling.",
		arguments: [
			{
				name: "forsøk",
				description: "er antallet uavhengige forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for å lykkes ved hvert forsøk"
			},
			{
				name: "antall_s",
				description: "er antallet forsøk med vellykket utfall"
			},
			{
				name: "antall_s2",
				description: "Hvis angitt returnerer denne funksjonen sannsynligheten for at antallet vellykkede forsøk skal ligge mellom antall_s og antall_s2"
			}
		]
	},
	{
		name: "BINOM.INV",
		description: "Returnerer den minste verdien der den kumulative binomiske fordelingen er større enn eller lik en vilkårsverdi.",
		arguments: [
			{
				name: "forsøk",
				description: "er antallet Bernoulli-forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for å lykkes i hvert forsøk, et tall fra og med 0 til og med 1"
			},
			{
				name: "alfa",
				description: "er vilkårsverdien, et tall fra og med 0 til og med 1"
			}
		]
	},
	{
		name: "BINTILDES",
		description: "Konverterer et binærtall til et heltall i 10-tallsystemet.",
		arguments: [
			{
				name: "tall",
				description: "er binærtallet du vil konvertere"
			}
		]
	},
	{
		name: "BINTILHEKS",
		description: "Konverterer et binærtall til et heksadesimalt tall.",
		arguments: [
			{
				name: "tall",
				description: "er binærtallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "BINTILOKT",
		description: "Konverterer et binærtall til et oktaltall.",
		arguments: [
			{
				name: "tall",
				description: "er binærtallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "BITEKSKLUSIVELLER",
		description: "Returnerer et bitvis 'Utelukkende eller' av to tall.",
		arguments: [
			{
				name: "tall1",
				description: "er desimalformen av det binære tallet du vil evaluere"
			},
			{
				name: "tall2",
				description: "er desimalformen av det binære tallet du vil evaluere"
			}
		]
	},
	{
		name: "BITELLER",
		description: "Returnerer et bitvis 'Eller' av to tall.",
		arguments: [
			{
				name: "tall1",
				description: "er desimalformen av det binære tallet du vil evaluere"
			},
			{
				name: "tall2",
				description: "er desimalformen av det binære tallet du vil evaluere"
			}
		]
	},
	{
		name: "BITHFORSKYV",
		description: "Returnerer et tall som forskyves mot høyre med størrelse_forskyvning biter.",
		arguments: [
			{
				name: "tall",
				description: "er desimalformen av det binære tallet du vil evaluere"
			},
			{
				name: "størrelse_forskyvning",
				description: "er antallet biter du vil forskyve tallet mot høyre med"
			}
		]
	},
	{
		name: "BITOG",
		description: "Returnerer et bitvis 'Og' av to tall.",
		arguments: [
			{
				name: "tall1",
				description: "er desimalformen av det binære tallet du vil evaluere"
			},
			{
				name: "tall2",
				description: "er desimalformen av det binære tallet du vil evaluere"
			}
		]
	},
	{
		name: "BITVFORSKYV",
		description: "Returnerer et tall som forskyves mot venstre med størrelse_forskyvning biter.",
		arguments: [
			{
				name: "tall",
				description: "er desimalformen av det binære tallet du vil evaluere"
			},
			{
				name: "størrelse_forskyvning",
				description: "er antallet biter du vil forskyve tallet mot venstre med"
			}
		]
	},
	{
		name: "BYTT.UT",
		description: "Erstatter eksisterende tekst med ny tekst i en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten eller referansen til en celle som inneholder teksten der du vil bytte ut tegn"
			},
			{
				name: "gammel_tekst",
				description: "er den eksisterende teksten du vil erstatte. Hvis gammel_tekst ikke er lik med hensyn til store og små bokstaver, vil BYTT.UT ikke erstatte teksten"
			},
			{
				name: "ny_tekst",
				description: "er teksten du vil erstatte gammel_tekst med"
			},
			{
				name: "forekomst_nr",
				description: "angir hvilken forekomst av gammel_tekst du vil erstatte. Hvis argumentet utelates, erstattes alle forekomster av gammel_tekst"
			}
		]
	},
	{
		name: "CELLE",
		description: "Returnerer informasjon om formateringen av, plasseringen til og innholdet i den første cellen, i henhold til arkets leserekkefølge, i en referanse.",
		arguments: [
			{
				name: "infotype",
				description: "er en tekstverdi som angir hvilken type celleinformasjon du ønsker."
			},
			{
				name: "ref",
				description: "er cellen du vil ha informasjon om"
			}
		]
	},
	{
		name: "COS",
		description: "Returnerer cosinus for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne cosinus til, i radianer"
			}
		]
	},
	{
		name: "COSH",
		description: "Returnerer den hyperbolske cosinus til et tall.",
		arguments: [
			{
				name: "tall",
				description: "er ethvert reelt tall"
			}
		]
	},
	{
		name: "COT",
		description: "Returnerer cotangens for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne cotangensen til, i radianer"
			}
		]
	},
	{
		name: "COTH",
		description: "Returnerer hyperbolsk cotangens for et tall.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne hyperbolsk cotangens til, i radianer"
			}
		]
	},
	{
		name: "CSC",
		description: "Returnerer cosekans for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne cosekans til, i radianer"
			}
		]
	},
	{
		name: "CSCH",
		description: "Returnerer hyperbolsk cosekans for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne hyperbolsk cosekans til, i radianer"
			}
		]
	},
	{
		name: "DAG",
		description: "Returnerer en dag i måneden, et tall fra 1 til 31.",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato eller et klokkeslett som brukes av Spreadsheet"
			}
		]
	},
	{
		name: "DAG.ETTER",
		description: "Returnerer serienummeret for datoen som er det viste antallet måneder før eller etter startdatoen.",
		arguments: [
			{
				name: "start_dato",
				description: "er et serienummer for datoen som representerer startdatoen"
			},
			{
				name: "måneder",
				description: "er antall måneder før eller etter start_dato"
			}
		]
	},
	{
		name: "DAGER",
		description: "Returnerer antallet dager mellom to datoer.",
		arguments: [
			{
				name: "sluttdato",
				description: "startdato og sluttdato er de to datoene du vil vite antallet dager mellom"
			},
			{
				name: "startdato",
				description: "startdato og sluttdato er de to datoene du vil vite antallet dager mellom"
			}
		]
	},
	{
		name: "DAGER360",
		description: "Beregner antall dager mellom to datoer basert på et år med 360 dager (12 måneder à 30 dager).",
		arguments: [
			{
				name: "startdato",
				description: "startdato og sluttdato er de to datoene du vil vite antall dager mellom"
			},
			{
				name: "sluttdato",
				description: "startdato og sluttdato er de to datoene du vil vite antall dager mellom"
			},
			{
				name: "metode",
				description: "er en logisk verdi som angir beregningsmetoden. U.S. (NASD) = USANN eller utelatt, Europeisk = SANN."
			}
		]
	},
	{
		name: "DANTALL",
		description: "Teller cellene som inneholder tall, i feltet (kolonnen) med poster i databasen som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen"
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DANTALLA",
		description: "Teller utfylte celler i feltet (kolonnen) med poster i databasen som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste med data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DATO",
		description: "Returnerer tallet som svarer til datoen i koden for dato og tid i Spreadsheet.",
		arguments: [
			{
				name: "år",
				description: "er et tall fra 1900 til 9999 i Spreadsheet for Windows, eller fra 1904 til 9999 i Spreadsheet for Macintosh"
			},
			{
				name: "måned",
				description: "er et tall fra 1 til 12 som representerer måneden i året"
			},
			{
				name: "dag",
				description: "er et tall fra 1 til 31 som representerer dagen i måneden"
			}
		]
	},
	{
		name: "DATODIFF",
		description: "",
		arguments: [
		]
	},
	{
		name: "DATOVERDI",
		description: "Konverterer en dato med tekstformat til et tall som representerer datoen i koden for dato og tid i Spreadsheet.",
		arguments: [
			{
				name: "dato_tekst",
				description: "er tekst som returnerer en dato i et av datoformatene i Spreadsheet, mellom 01.01.1900 (Windows) eller 01.01.1904 (Macintosh) og 31.12.9999"
			}
		]
	},
	{
		name: "DAVSKR",
		description: "Returnerer avskrivningen for et aktivum for en angitt periode, ved hjelp av fast degressiv avskrivning.",
		arguments: [
			{
				name: "kostnad",
				description: "er den opprinnelige kostnaden til aktivumet"
			},
			{
				name: "restverdi",
				description: "er verdien ved slutten av avskrivningen"
			},
			{
				name: "levetid",
				description: "er antall perioder et aktivum blir avskrevet over (ofte kalt aktivumets økonomiske levetid)"
			},
			{
				name: "periode",
				description: "er perioden du vil beregne avskrivningen for. Periode må angis med samme tidsenhet som Levetid"
			},
			{
				name: "måned",
				description: "er antall måneder i det første året. Hvis argumentet måneder er utelatt, blir det satt til 12"
			}
		]
	},
	{
		name: "DEGRAVS",
		description: "Returnerer avskrivningen for et aktivum for en gitt periode, ved hjelp av dobbel degressiv avskrivning eller en annen metode du selv angir.",
		arguments: [
			{
				name: "kostnad",
				description: "er den opprinnelige kostnaden til aktivumet"
			},
			{
				name: "restverdi",
				description: "er verdien ved slutten av avskrivningen"
			},
			{
				name: "levetid",
				description: "er antall perioder et aktivum blir avskrevet over (ofte kalt aktivumets økonomiske levetid)"
			},
			{
				name: "periode",
				description: "er perioden du vil beregne avskrivningen for. Periode må angis med samme tidsenhet som Levetid"
			},
			{
				name: "faktor",
				description: "er faktoren verdien avtar med. Hvis argumentet faktor utelates, blir det satt til 2 (dobbel degressiv avskrivning)"
			}
		]
	},
	{
		name: "DELSUM",
		description: "Returnerer en delsum fra en liste eller database.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "funksjon",
				description: "er et tall mellom 1 og 11 som angir hvilken sammendragsfunksjon som skal brukes i beregningen av delsummen."
			},
			{
				name: "ref1",
				description: "er 1 til 254 områder eller referanser du vil beregne delsummen for"
			}
		]
	},
	{
		name: "DELTA",
		description: "Undersøker om to tall er like.",
		arguments: [
			{
				name: "tall1",
				description: "er det første tallet"
			},
			{
				name: "tall2",
				description: "er det andre tallet"
			}
		]
	},
	{
		name: "DELTEKST",
		description: "Returnerer tegnene fra midten av en tekststreng, hvis posisjonen for det første tegnet og lengden er oppgitt.",
		arguments: [
			{
				name: "tekst",
				description: "er tekststrengen som inneholder tegnene du vil trekke ut"
			},
			{
				name: "startpos",
				description: "er posisjonen til det første tegnet du vil trekke ut. Det første tegnet i tekst er 1"
			},
			{
				name: "antall_tegn",
				description: "angir hvor mange tegn som skal returneres fra teksten"
			}
		]
	},
	{
		name: "DESIMAL",
		description: "Konverterer en tekstrepresentasjon av et tall i en gitt basis til et desimaltall.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil konvertere"
			},
			{
				name: "basis",
				description: "er basisen til tallet du konverterer"
			}
		]
	},
	{
		name: "DESTILBIN",
		description: "Konverterer et heltall i 10-tallsystemet til et binærtall.",
		arguments: [
			{
				name: "tall",
				description: "er heltallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "DESTILHEKS",
		description: "Konverterer et heltall i 10-tallsystemet til et heksadesimaltall.",
		arguments: [
			{
				name: "tall",
				description: "er heltallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "DESTILOKT",
		description: "Konverterer et heltall i 10-tallsystemet til et oktaltall.",
		arguments: [
			{
				name: "tall",
				description: "er heltallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "DGJENNOMSNITT",
		description: "Returnerer gjennomsnittet av verdiene i en kolonne i en liste eller database som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen"
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DHENT",
		description: "Trekker ut en post som oppfyller vilkårene du angir, fra en database.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen"
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DISKONTERT",
		description: "Returnerer diskonteringsraten for et verdipapir.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "pris",
				description: "er verdipapirets pris per pålydende kr 100"
			},
			{
				name: "innløsningsverdi",
				description: "er verdipapirets innløsningsverdi per pålydende kr 100"
			},
			{
				name: "basis",
				description: "angir typen datosystem som brukes"
			}
		]
	},
	{
		name: "DMAKS",
		description: "Returnerer det høyeste tallet i feltet (kolonnen) med poster i databasen som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen"
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DMIN",
		description: "Returnerer det laveste tallet i feltet (kolonnen) med poster i databasen som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DOBBELFAKT",
		description: "Returnerer et talls doble fakultet.",
		arguments: [
			{
				name: "tall",
				description: "er verdien som den doble fakultetsverdien skal returneres for"
			}
		]
	},
	{
		name: "DOLLARBR",
		description: "Konverterer en valutapris som et desimaltall til en valutapris uttrykt som en brøk.",
		arguments: [
			{
				name: "desimal_valuta",
				description: "er et desimaltall"
			},
			{
				name: "brøk",
				description: "er heltallet som skal brukes i brøkens nevner"
			}
		]
	},
	{
		name: "DOLLARDE",
		description: "Konverterer en valutapris uttrykt som en brøk til en valutapris uttrykt som et desimaltall.",
		arguments: [
			{
				name: "brøk_valuta",
				description: "er et tall uttrykt som en brøk"
			},
			{
				name: "brøk",
				description: "er heltallet som skal brukes i brøkens nevner"
			}
		]
	},
	{
		name: "DPRODUKT",
		description: "Multipliserer verdiene i et bestemt felt (kolonne) med poster i databasen som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DSTDAV",
		description: "Estimerer standardavviket på grunnlag av et utvalg i form av merkede databaseposter.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DSTDAVP",
		description: "Beregner standardavviket basert på at merkede databaseposter utgjør hele populasjonen.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DSUMMER",
		description: "Legger til tallene i feltet (kolonnen) med poster i databasen som oppfyller vilkårene du angir.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DVARIANS",
		description: "Estimerer variansen basert på at merkede databaseposter utgjør et utvalg.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "DVARIANSP",
		description: "Beregner variansen basert på at merkede databaseposter utgjør hele populasjonen.",
		arguments: [
			{
				name: "database",
				description: "er celleområdet som utgjør listen eller databasen. En database er en liste over data som hører sammen."
			},
			{
				name: "felt",
				description: "er enten etiketten til kolonnen i doble anførselstegn eller et tall som representerer kolonnens plass i listen"
			},
			{
				name: "vilkår",
				description: "er celleområdet som inneholder vilkårene du angir. Området inneholder en kolonneetikett og en celle under etiketten for et vilkår"
			}
		]
	},
	{
		name: "EFFEKTIV.RENTE",
		description: "Returnerer den effektive årlige renten.",
		arguments: [
			{
				name: "nominell_rente",
				description: "er den nominelle rentefoten"
			},
			{
				name: "perioder",
				description: "er antall sammensatte perioder per år"
			}
		]
	},
	{
		name: "EKSAKT",
		description: "Kontrollerer om to tekststrenger er helt like, og returnerer SANN eller USANN. EKSAKT skiller mellom små og store bokstaver.",
		arguments: [
			{
				name: "tekst1",
				description: "er den første tekststrengen"
			},
			{
				name: "tekst2",
				description: "er den andre tekststrengen"
			}
		]
	},
	{
		name: "EKSKLUSIVELLER",
		description: "Returnerer et 'Utelukkende eller' av alle argumenter.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "er 1 til 254 tilstander du vil teste, som hver kan være enten SANN eller USANN, og som kan være logiske verdier, matriser eller referanser"
			},
			{
				name: "logisk2",
				description: "er 1 til 254 tilstander du vil teste, som hver kan være enten SANN eller USANN, og som kan være logiske verdier, matriser eller referanser"
			}
		]
	},
	{
		name: "EKSP",
		description: "Returnerer e opphøyd i en potens du angir.",
		arguments: [
			{
				name: "tall",
				description: "er eksponenten som brukes på grunntallet e. Konstanten e = 2,71828182845904, grunntallet for den naturlige logaritmen"
			}
		]
	},
	{
		name: "EKSP.FORDELING",
		description: "Returnerer eksponentialfordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien av funksjonen, et ikke-negativt tall"
			},
			{
				name: "lambda",
				description: "er parameterverdien, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi som funksjonen skal returnere. For kumulativ fordeling bruker du SANN. For sannsynlighetstetthet bruker du USANN"
			}
		]
	},
	{
		name: "EKSP.FORDELING.N",
		description: "Returnerer eksponentialfordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien av funksjonen, et ikke-negativt tall"
			},
			{
				name: "lambda",
				description: "er parameterverdien, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi for hva funksjonen skal returnere: SANN gir den kumulative fordelingsfunksjonen, USANN gir funksjonen for sannsynlig tetthet"
			}
		]
	},
	{
		name: "ELLER",
		description: "Kontrollerer om noen av argumentene er lik SANN, og returnerer SANN eller USANN. Returnerer USANN bare hvis alle argumentene er lik USANN.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "er 1 til 255 tilstander du vil teste, og som hver kan være enten SANN eller USANN"
			},
			{
				name: "logisk2",
				description: "er 1 til 255 tilstander du vil teste, og som hver kan være enten SANN eller USANN"
			}
		]
	},
	{
		name: "ER.AVDRAG",
		description: "Returnerer renten som er betalt i løpet av en angitt investeringsperiode.",
		arguments: [
			{
				name: "rente",
				description: "rente per periode. Bruk for eksempel 6%/4 for kvartalsvise innbetalinger med 6 % årlig rente"
			},
			{
				name: "periode",
				description: "perioden du vil finne renten i"
			},
			{
				name: "antall_innbet",
				description: "antall innbetalingsperioder i en investering"
			},
			{
				name: "nåverdi",
				description: "nåverdien av en serie fremtidige innbetalinger"
			}
		]
	},
	{
		name: "ERF",
		description: "Kontrollerer om verdien er en annen feilverdi enn #I/T (#VERDI!, #REF!, #DIV/0!, #NUM!, #NAVN? eller #NULL!),  og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERFEIL",
		description: "Kontrollerer om verdien er en feilverdi (#I/T, #VERDI!, #REF!, #DIV/0!, #NUM!, #NAVN? eller #NULL!),og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERFORMEL",
		description: "Kontrollerer om en referanse er til en celle som inneholder en formel, og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "referanse",
				description: "er en referanse til cellen du vil teste. Referansen kan være en cellereferanse, en formel eller et navn som refererer til en celle"
			}
		]
	},
	{
		name: "ERIKKETEKST",
		description: "Kontrollerer om en verdi ikke er tekst (tomme celler er ikke tekst), og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste: en celle, en formel, eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERIT",
		description: "Kontrollerer om verdien er #I/T(ikke tilgjengelig) og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERLOGISK",
		description: "Kontrollerer om en verdi er en logisk verdi (SANN eller USANN), og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERODDE",
		description: "Returnerer SANN hvis tallet er et oddetall.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du ønsker å teste"
			}
		]
	},
	{
		name: "ERPARTALL",
		description: "Returnerer SANN hvis tallet er et partall.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du ønsker å teste"
			}
		]
	},
	{
		name: "ERREF",
		description: "Kontrollerer om verdien er en referanse, og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERSTATT",
		description: "Erstatter en del av en tekststreng med en annen tekststreng.",
		arguments: [
			{
				name: "gammel_tekst",
				description: "er tekst der du vil erstatte enkelte tegn"
			},
			{
				name: "startpos",
				description: "er posisjonen til tegnet i gammel_tekst som du vil erstatte med ny_tekst"
			},
			{
				name: "antall_tegn",
				description: "er antall tegn i gammel_tekst som du vil erstatte"
			},
			{
				name: "ny_tekst",
				description: "er teksten som skal erstatte tegn i gammel_tekst"
			}
		]
	},
	{
		name: "ERTALL",
		description: "Kontrollerer om en verdi er et tall, og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERTEKST",
		description: "Kontrollerer om en verdi er tekst, og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste. Verdien kan referere til en celle, en formel eller et navn som refererer til en celle, en formel eller en verdi"
			}
		]
	},
	{
		name: "ERTOM",
		description: "Kontrollerer om en referanse er til en tom celle, og returnerer SANN eller USANN.",
		arguments: [
			{
				name: "verdi",
				description: "er cellen eller et navn som refererer til cellen du vil teste"
			}
		]
	},
	{
		name: "F.FORDELING",
		description: "Returnerer (den venstresidige) F-sannsynlighetsfordelingen (spredningsgraden) for to datasett.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil evaluere funksjonen for, et ikke-negativt tall"
			},
			{
				name: "frihetsgrader1",
				description: "er tellerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "frihetsgrader2",
				description: "er nevnerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "kumulative",
				description: "er en logisk verdi som funksjonen skal returnere: SANN gir den kumulative fordelingsfunksjonen; USANN gir funksjonen for sannsynlig tetthet"
			}
		]
	},
	{
		name: "F.FORDELING.H",
		description: "Returnerer (den høyresidige) F-sannsynlighetsfordelingen (spredningsgraden) for to datasett.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil evaluere funksjonen for, et ikke-negativt tall"
			},
			{
				name: "frihetsgrader1",
				description: "er tellerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "frihetsgrader2",
				description: "er nevnerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "F.INV",
		description: "Returnerer den inverse av (den venstresidige) F-sannsynlighetsfordelingen. hvis p = F.FORDELING(x,...), sål F.INV(p,...) = x.",
		arguments: [
			{
				name: "x",
				description: "er en sannsynlighet knyttet til den kumulative F-fordelingen, et tall fra 0 til og med 1"
			},
			{
				name: "frihetsgrader1",
				description: "er tellerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "frihetsgrader2",
				description: "er nevnerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "F.INV.H",
		description: "Returnerer den inverse av (den høyresidige) F-sannsynlighetsfordelingen. hvis p = F.FORDELING.H(x,...), så F.INV.H(p,...) = x.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til den kumulative F-fordelingen, et tall fra 0 til og med 1"
			},
			{
				name: "frihetsgrader1",
				description: "er tellerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "frihetsgrader2",
				description: "er nevnerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "F.TEST",
		description: "Returnerer resultatet av en F-test, den tosidige sannsynligheten for at variansene i matrise1 og matrise2 ikke er signifikant forskjellige.",
		arguments: [
			{
				name: "matrise1",
				description: "er den første matrisen eller det første området med data og kan være tall eller navn, matriser eller referanser som inneholder tall (tomme ignoreres)"
			},
			{
				name: "matrise2",
				description: "er den andre matrisen eller det andre dataområdet og kan være tall eller navn, matriser eller referanser som inneholder tall (tomme ignoreres)"
			}
		]
	},
	{
		name: "FAKULTET",
		description: "Returnerer fakultet til et tall, med andre ord produktet av 1*2*3*...* Tall.",
		arguments: [
			{
				name: "tall",
				description: "er det ikke-negative tallet du vil finne fakultet til"
			}
		]
	},
	{
		name: "FASTSATT",
		description: "Runder av et tall til et angitt antall desimaler og konverter det til tekst med eller uten kommaer.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil runde av og konvertere til tekst"
			},
			{
				name: "desimaler",
				description: "er antall sifre til høyre for desimaltegnet. Hvis argumentet utelates, blir desimaler satt til 2"
			},
			{
				name: "ingen_tusenskille",
				description: "er en logisk verdi. Ikke vis kommaer i den returnerte teksten = SANN, vis kommaer i den returnerte teksten = USANN eller utelatt"
			}
		]
	},
	{
		name: "FEIL.TYPE",
		description: "Returnerer et tall som svarer til en feilverdi.",
		arguments: [
			{
				name: "feilverdi",
				description: "er feilverdien du vil finne identifikasjonsnummeret til, og kan være selve feilverdien eller en referanse til en celle som inneholder en feilverdi"
			}
		]
	},
	{
		name: "FEILF",
		description: "Returnerer feilfunksjonen.",
		arguments: [
			{
				name: "nedre_grense",
				description: "er den nedre grensen for å integrere FEILF"
			},
			{
				name: "øvre_grense",
				description: "er den øvre grensen for å integrere FEILF"
			}
		]
	},
	{
		name: "FEILF.PRESIS",
		description: "Returnerer feilfunksjonen.",
		arguments: [
			{
				name: "X",
				description: "er den nedre grensen for å integrere FEILF.PRESIS"
			}
		]
	},
	{
		name: "FEILFK",
		description: "Returnerer den komplementære feilfunksjonen.",
		arguments: [
			{
				name: "x",
				description: "er den nedre grensen for å integrere FEILF"
			}
		]
	},
	{
		name: "FEILFK.PRESIS",
		description: "Returnerer den komplementære feilfunksjonen.",
		arguments: [
			{
				name: "x",
				description: "er den nedre grensen for å integrere FEILF.PRESIS"
			}
		]
	},
	{
		name: "FFORDELING",
		description: "Returnerer (den høyre) F-fordelingen (spredningsgraden) for to datasett.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil regne ut funksjonen for, et ikke-negativt tall"
			},
			{
				name: "frihetsgrader1",
				description: "er tellerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "frihetsgrader2",
				description: "er nevnerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "FFORDELING.INVERS",
		description: "Returnerer den inverse av (den høyre) F-fordelingen. Hvis p = FFORDELING(x...), er FFORDELING.INVERS(p...) = x.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til den kumulative F-fordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "frihetsgrader1",
				description: "er tellerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "frihetsgrader2",
				description: "er nevnerens frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
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
		name: "FINN",
		description: "Returnerer nummeret for posisjonen for det første tegnet i en tekststreng inne i en annen tekststreng. FINN skiller mellom store og små bokstaver.",
		arguments: [
			{
				name: "finn",
				description: "er teksten du vil søke etter. Bruk doble anførselstegn (tom tekst) for å søke etter det første tegnet i innen_tekst. Du kan ikke bruke jokertegn"
			},
			{
				name: "innen_tekst",
				description: "er teksten som inneholder teksten du vil finne"
			},
			{
				name: "startpos",
				description: "angir tegnet du vil starte søket fra. Det første tegnet i innen_tekst er tegn nummer 1. Hvis den utelates, blir startpos satt til 1"
			}
		]
	},
	{
		name: "FINN.KOLONNE",
		description: "Søker etter en verdi i den øverste raden i en tabell eller matrise, og returnerer verdien i samme kolonne fra en rad du angir.",
		arguments: [
			{
				name: "søkeverdi",
				description: "er den verdien du vil søke etter i den første raden i tabellen, og kan være en verdi, en referanse eller en tekststreng"
			},
			{
				name: "matrise",
				description: "er en tabell med tekst, tall eller logiske verdier som du kan søke etter data i. Matrise kan være en referanse til et område eller et områdenavn"
			},
			{
				name: "radindeks",
				description: "er det radnummeret i matrise sammenligningsverdien skal returneres fra. Den første raden med verdier i tabellen er rad 1"
			},
			{
				name: "område",
				description: "er en logisk verdi. Hvis du vil finne den verdien som har størst samsvar i den øverste raden (sortert stigende) = SANN eller utelatt, hvis du vil finne en verdi som er eksakt lik = USANN"
			}
		]
	},
	{
		name: "FINN.RAD",
		description: "Søker etter en verdi i kolonnen lengst til venstre i en tabell, og returnerer en verdi i samme rad fra en kolonne du angir. Standardinnstilling er at tabellen må være sortert i stigende rekkefølge.",
		arguments: [
			{
				name: "søkeverdi",
				description: "er verdien du vil søke etter i den første kolonnen i tabellen, og kan være en verdi, en referanse eller en tekststreng"
			},
			{
				name: "matrise",
				description: "er en tabell med tekst, tall eller logiske verdier som data hentes fra. Tabell_matrise kan være en referanse til et område eller et områdenavn"
			},
			{
				name: "kolonneindeks",
				description: "er kolonnenummeret i tabell_matrise som den samsvarende verdien returneres fra. Den første kolonnen med verdier i tabellen er kolonne 1"
			},
			{
				name: "søkeområde",
				description: "er en logisk verdi. Hvis du vil finne verdien i den første kolonnen (sortert i stigende rekkefølge) som er mest lik søkeverdien = SANN eller utelatt. Hvis du vil finne en verdi som er helt lik søkeverdien = USANN"
			}
		]
	},
	{
		name: "FISHER",
		description: "Returnerer Fisher-transformasjonen.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil finne transformasjonen for, et tall mellom -1 og 1, unntatt -1 og 1"
			}
		]
	},
	{
		name: "FISHERINV",
		description: "Returnerer den inverse av Fisher-transformasjonen. Hvis y = FISHER(x), er FISHERINV(y) = x.",
		arguments: [
			{
				name: "y",
				description: "er verdien du vil utføre den inverse av transformasjonen på"
			}
		]
	},
	{
		name: "FORMELTEKST",
		description: "Returnerer en formel som en streng.",
		arguments: [
			{
				name: "referanse",
				description: "er en referanse til en formel"
			}
		]
	},
	{
		name: "FORSKYVNING",
		description: "Returnerer en referanse til et område som er et gitt antall rader og kolonner fra en gitt referanse.",
		arguments: [
			{
				name: "ref",
				description: "er referansen som skal brukes som utgangspunkt for forskyvningen, en referanse til en celle eller et sammenhengende celleområde"
			},
			{
				name: "rader",
				description: "er antall rader, opp eller ned, som du vil at cellen øverst til venstre i resultatet skal vise til"
			},
			{
				name: "kolonner",
				description: "er antall kolonner, til venstre eller høyre, som du vil at cellen øverst til venstre i resultatet skal vise til"
			},
			{
				name: "høyde",
				description: "er høyden, i antall rader, som du vil at resultatet skal ha. Får den samme høyden som referansen hvis argumentet utelates"
			},
			{
				name: "bredde",
				description: "er bredden, i antall kolonner, som du vil at resultatet skal ha. Får samme bredde som referansen hvis argumentet utelates"
			}
		]
	},
	{
		name: "FORTEGN",
		description: "Returnerer fortegnet for et tall: 1 hvis tallet er er positivt, 0 hvis tallet er null, og -1 hvis tallet er negativt.",
		arguments: [
			{
				name: "tall",
				description: "er ethvert reelt tall"
			}
		]
	},
	{
		name: "FREKVENS",
		description: "Beregner hvor ofte verdier forekommer i et område med verdier, og returnerer en loddrett matrise med tall med ett element mer enn intervallmatrise.",
		arguments: [
			{
				name: "datamatrise",
				description: "er en matrise eller en referanse til et sett verdier du vil beregne frekvensene i (tomme celler og tekst ignoreres)"
			},
			{
				name: "intervallmatrise",
				description: "er en matrise av eller referanse til intervaller du vil gruppere verdiene i datamatrisen i"
			}
		]
	},
	{
		name: "FTEST",
		description: "Returnerer resultatet av en F-test, den tosidige sannsynligheten for at variansene i matrise1 og matrise2 ikke er signifikant forskjellige.",
		arguments: [
			{
				name: "matrise1",
				description: "er den første matrisen eller det første området med data og kan være tall eller navn, matriser eller referanser som inneholder tall (tomme ignoreres)"
			},
			{
				name: "matrise2",
				description: "er den andre matrisen eller det andre området med data og kan være tall eller navn, matriser eller referanser som inneholder tall (tomme ignoreres)"
			}
		]
	},
	{
		name: "GAMMA",
		description: "Returnerer gammafunksjonsverdien.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil beregne gamma for"
			}
		]
	},
	{
		name: "GAMMA.FORDELING",
		description: "Returnerer gammafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien der du vil evaluere fordelingen, et ikke-negativt tall"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen, et positivt tall. Hvis beta = 1, returnerer GAMMA.FORDELING standard gammafordeling"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi: SANN returnerer den kumulative fordelingsfunksjonen, USANN eller utelatt returnerer punktsannsynlighet"
			}
		]
	},
	{
		name: "GAMMA.INV",
		description: "Returnerer den inverse av den kumulative gammafordelingen: hvis p = GAMMA.FORDELING(x,...), så GAMMA.INV(p,...) = x.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er sannsynligheten som er knyttet til gammafordelingen, et tall fra 0 til og med 1"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen, et positivt tall. Hvis beta = 1, returnerer GAMMA.INV den inverse av standard gammafordeling"
			}
		]
	},
	{
		name: "GAMMAFORDELING",
		description: "Returnerer gammafordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil beregne fordelingen for, et ikke-negativt tall"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen, et positivt tall. Hvis beta = 1, returnerer GAMMAFORDELING standard gammafordeling"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. SANN returnerer den kumulative fordelingsfunksjonen. USANN eller utelatt returnerer punktsannsynligheten"
			}
		]
	},
	{
		name: "GAMMAINV",
		description: "Returnerer den inverse av den kumulative gammafordelingen. Hvis p = GAMMAFORDELING(x...), er GAMMAINV(p,...) = x.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er sannsynligheten assosiert med gammafordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen, et positivt tall. Hvis beta = 1, returnerer funksjonen GAMMAINV den inverse av standard gammafordeling"
			}
		]
	},
	{
		name: "GAMMALN",
		description: "Returnerer den naturlige logaritmen til gammafunksjonen.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil beregne GAMMALN for, et positivt tall"
			}
		]
	},
	{
		name: "GAMMALN.PRESIS",
		description: "Returnerer den naturlige logaritmen til gammafunksjonen.",
		arguments: [
			{
				name: "x",
				description: "er verdien (et positivt tall) du vil beregne GAMMALN.PRESIS for"
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
		name: "GJENNOMSNITT",
		description: "Returnerer gjennomsnittet av argumentene, som kan være tall, navn, matriser eller referanser som inneholder tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 numeriske argumenter du vil finne gjennomsnittet av"
			},
			{
				name: "tall2",
				description: "er 1 til 255 numeriske argumenter du vil finne gjennomsnittet av"
			}
		]
	},
	{
		name: "GJENNOMSNITT.GEOMETRISK",
		description: "Returnerer den geometriske middelverdien for en matrise eller et område med positive numeriske data.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne gjennomsnittet av"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne gjennomsnittet av"
			}
		]
	},
	{
		name: "GJENNOMSNITT.HARMONISK",
		description: "Returnerer den harmoniske middelverdien for et datasett med positive tall, det vil si den resiproke verdien av den aritmetiske middelverdien av de resiproke verdiene.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne det harmoniske gjennomsnittet for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne det harmoniske gjennomsnittet for"
			}
		]
	},
	{
		name: "GJENNOMSNITT.HVIS.SETT",
		description: "Finner gjennomsnittet (aritmetisk middelverdi) for cellene som angis av et gitt sett med vilkår eller kriterier.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "gjennomsnitt_område",
				description: "er de faktiske cellene det skal finnes gjennomsnitt av"
			},
			{
				name: "kriterieområde",
				description: "er området med celler du vil evaluere i det spesifikke vilkåret"
			},
			{
				name: "kriterium",
				description: "er vilkåret eller kriteriet i form av tall, uttrykk eller tekst som definerer hvilke celler som skal brukes til å finne gjennomsnittet"
			}
		]
	},
	{
		name: "GJENNOMSNITTA",
		description: "Returnerer gjennomsnittet (aritmetisk middelverdi) av argumentene. Returnerer tekst og USANN i argumenter som 0 og SANN som 1. Argumentene kan være tall, navn, matriser eller referanser.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 argumenter du vil finne gjennomsnittet av"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 argumenter du vil finne gjennomsnittet av"
			}
		]
	},
	{
		name: "GJENNOMSNITTHVIS",
		description: "Finner gjennomsnittet (det aritmetiske gjennomsnittet) for cellene som angis av et gitt vilkår eller kriterium.",
		arguments: [
			{
				name: "område",
				description: "er området av celler du ønsker å få evaluert"
			},
			{
				name: "kriterium",
				description: "er vilkåret eller kriteriet i form av et tall, uttrykk eller tekst som definerer hvilke celler som vil brukes til å finne gjennomsnittet"
			},
			{
				name: "gjennomsnitt_område",
				description: "er de faktiske cellene som skal brukes til å finne gjennomsnittet. Hvis argumentet utelates, blir cellene i området brukt "
			}
		]
	},
	{
		name: "GJENNOMSNITTSAVVIK",
		description: "Returnerer datapunktenes gjennomsnittlige absoluttavvik fra middelverdien. Argumentene kan være tall eller navn, matriser eller referanser som inneholder tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 argumenter du vil finne de gjennomsnittlige absoluttavvikene for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 argumenter du vil finne de gjennomsnittlige absoluttavvikene for"
			}
		]
	},
	{
		name: "GJENTA",
		description: "Gjentar tekst et angitt antall ganger. Bruk GJENTA for å fylle en celle med et antall forekomster av en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten du vil gjenta"
			},
			{
				name: "antall_ganger",
				description: "er et positivt tall som angir hvor mange ganger teksten skal gjentas"
			}
		]
	},
	{
		name: "GRADER",
		description: "Konverterer radianer til grader.",
		arguments: [
			{
				name: "vinkel",
				description: "er vinkelen du vil konvertere, angitt i radianer"
			}
		]
	},
	{
		name: "GRENSE.BINOM",
		description: "Returnerer den minste verdien der den kumulative binomiske fordelingen er større enn eller lik en vilkårsverdi.",
		arguments: [
			{
				name: "forsøk",
				description: "er antallet Bernoulli-forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for å lykkes i hvert forsøk, et tall fra og med 0 til og med 1"
			},
			{
				name: "alfa",
				description: "er vilkårsverdien, et tall fra og med 0 til og med 1"
			}
		]
	},
	{
		name: "GRENSEVERDI",
		description: "Undersøker om et tall er større enn en terskelverdi.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du vil teste mot steg"
			},
			{
				name: "steg",
				description: "er terskelverdien"
			}
		]
	},
	{
		name: "GRUNNTALL",
		description: "Konverterer et tall til en tekstrepresentasjon med den gitte basisen.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil konvertere"
			},
			{
				name: "basis",
				description: "er den basisen som du vil konvertere tallet til"
			},
			{
				name: "minste_lengde",
				description: "er den minste lengden på den returnerte strengen. Hvis dette er utelatt, blir foranstilte nuller ikke lagt til"
			}
		]
	},
	{
		name: "HEKSTILBIN",
		description: "Konverterer et heksadesimalt tall til et binærtall.",
		arguments: [
			{
				name: "tall",
				description: "er det heksadesimale tallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "HEKSTILDES",
		description: "Konverterer et heksadesimalt tall til et heltall i 10-tallsystemet.",
		arguments: [
			{
				name: "tall",
				description: "er det heksadesimale tallet du vil konvertere"
			}
		]
	},
	{
		name: "HEKSTILOKT",
		description: "Konverterer et heksadesimaltall til et oktaltall.",
		arguments: [
			{
				name: "tall",
				description: "er det heksadesimale tallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "HELTALL",
		description: "Runder av et tall nedover til nærmeste heltall.",
		arguments: [
			{
				name: "tall",
				description: "er det reelle tallet du vil runde av nedover til et heltall"
			}
		]
	},
	{
		name: "HENTPIVOTDATA",
		description: "Trekker ut data som er lagret i en pivottabell.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "data_felt",
				description: "er navnet på datafeltet du vil trekke ut data fra"
			},
			{
				name: "pivottabell",
				description: "er en referanse til en celle eller et celleområde i pivottabellen som inneholder dataene du vil hente"
			},
			{
				name: "felt",
				description: "felt som det refereres til"
			},
			{
				name: "element",
				description: "feltelement det refereres til"
			}
		]
	},
	{
		name: "HØYRE",
		description: "Returnerer det angitte antall tegn fra slutten av en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er tekststrengen som inneholder tegnene du vil trekke ut"
			},
			{
				name: "antall_tegn",
				description: "angir hvor mange tegn du vil trekke ut. Settes til 1 hvis argumentet utelates"
			}
		]
	},
	{
		name: "HVIS",
		description: "Kontrollerer om vilkår er til stede, og returnerer en verdi hvis SANN, og en annen verdi hvis USANN.",
		arguments: [
			{
				name: "logisk_test",
				description: "er enhver verdi eller ethvert uttrykk som kan returnere SANN eller USANN"
			},
			{
				name: "sann",
				description: "er verdien som returneres hvis logisk_test er SANN. Hvis argumentet utelates, returneres SANN. Du kan neste opptil syv HVIS-funksjoner"
			},
			{
				name: "usann",
				description: "er verdien som returneres hvis logisk_test er USANN. Hvis argumentet utelates, returneres USANN"
			}
		]
	},
	{
		name: "HVIS.IT",
		description: "Returnerer verdien du angir hvis uttrykket løses til #I/T, og ellers returneres resultatet av uttrykket.",
		arguments: [
			{
				name: "verdi",
				description: "er enhver verdi eller et uttrykk eller en referanse"
			},
			{
				name: "verdi_hvis_it",
				description: "er enhver verdi eller et uttrykk eller en referanse"
			}
		]
	},
	{
		name: "HVISFEIL",
		description: "Returnerer verdi_hvis_feil hvis uttrykket er en feil, ellers returneres verdien til selve uttrykket.",
		arguments: [
			{
				name: "verdi",
				description: "en hvilken som helst verdi, referanse eller uttrykk"
			},
			{
				name: "verdi_hvis_feil",
				description: "en hvilken som helst verdi, referanse eller uttrykk"
			}
		]
	},
	{
		name: "HYPERKOBLING",
		description: "Lager en snarvei eller et hopp som åpner et dokument som er lagret på harddisken, på en server på nettverket eller på Internett.",
		arguments: [
			{
				name: "kobling",
				description: "er teksten som gir banen til og filnavnet på dokumentet som skal åpnes, en plassering på harddisken, en UNC-adresse eller en URL-bane"
			},
			{
				name: "egendefinert_navn",
				description: "er teksten eller tallet som vises i cellen. Hvis argumentet utelates, viser cellen koblingsteksten"
			}
		]
	},
	{
		name: "HYPGEOM.FORDELING",
		description: "Returnerer den hypergeometriske fordelingen.",
		arguments: [
			{
				name: "utvalg_s",
				description: "er antallet suksesser i utvalget"
			},
			{
				name: "utvalgsstørrelse",
				description: "er størrelsen på utvalget"
			},
			{
				name: "suksesser",
				description: "er antallet vellykkede forsøk i populasjonen"
			},
			{
				name: "populasjonsstørrelse",
				description: "er populasjonens størrelse"
			}
		]
	},
	{
		name: "HYPGEOM.FORDELING.N",
		description: "Returnerer den hypergeometriske fordelingen.",
		arguments: [
			{
				name: "utvalg_s",
				description: "er antallet suksesser i utvalget"
			},
			{
				name: "utvalgsstørrelse",
				description: "er størrelsen på utvalget"
			},
			{
				name: "suksesser",
				description: "er antallet vellykkede forsøk i populasjonen"
			},
			{
				name: "populasjonsstørrelse",
				description: "er populasjonens størrelse"
			},
			{
				name: "kumulative",
				description: "er en logisk verdi: Bruk SANN for kumulativ fordeling, bruk USANN for sannsynlig tetthet"
			}
		]
	},
	{
		name: "IDAG",
		description: "Returnerer gjeldende dato formatert som dato.",
		arguments: [
		]
	},
	{
		name: "IKKE",
		description: "Endrer USANN til SANN eller SANN til USANN.",
		arguments: [
			{
				name: "logisk",
				description: "er en verdi eller et uttrykk som kan beregnes til SANN eller USANN"
			}
		]
	},
	{
		name: "IMABS",
		description: "Returnerer absoluttverdien (modulus) til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne absoluttverdien til"
			}
		]
	},
	{
		name: "IMAGINÆR",
		description: "Returnerer den imaginære koeffisienten til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne den imaginære koeffisienten til"
			}
		]
	},
	{
		name: "IMARGUMENT",
		description: "Returnerer argumentet q, en vinkel uttrykt i radianer.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne argumentet til"
			}
		]
	},
	{
		name: "IMCOS",
		description: "Returnerer kosinusen til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne kosinusen til"
			}
		]
	},
	{
		name: "IMCOSH",
		description: "Returnerer hyperbolsk cosinus til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne hyperbolsk cosinus til"
			}
		]
	},
	{
		name: "IMCOT",
		description: "Returnerer cotangens til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne cotangens til"
			}
		]
	},
	{
		name: "IMCSC",
		description: "Returnerer cosekans til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne cosekans til"
			}
		]
	},
	{
		name: "IMCSCH",
		description: "Returnerer hyperbolsk cosekans til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne hyperbolsk cosekans til"
			}
		]
	},
	{
		name: "IMDIV",
		description: "Returnerer kvotienten av to komplekse tall.",
		arguments: [
			{
				name: "imtall1",
				description: "er den komplekse telleren eller dividenden"
			},
			{
				name: "imtall2",
				description: "er den komplekse nevneren eller divisoren"
			}
		]
	},
	{
		name: "IMEKSP",
		description: "Returnerer eksponenten til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne eksponenten til"
			}
		]
	},
	{
		name: "IMKONJUGERT",
		description: "Returnerer den komplekskonjugerte til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne den konjugerte til"
			}
		]
	},
	{
		name: "IMLN",
		description: "Returnerer den naturlige logaritmen til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne den naturlige logaritmen til"
			}
		]
	},
	{
		name: "IMLOG10",
		description: "Returnerer den briggske logaritmen (med grunntall 10) til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne den briggske logaritmen til"
			}
		]
	},
	{
		name: "IMLOG2",
		description: "Returnerer logaritmen med grunntall 2 til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne logaritmen med grunntall 2  til"
			}
		]
	},
	{
		name: "IMOPPHØY",
		description: "Returnerer et komplekst tall opphøyd i en heltallspotens.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du vil opphøye i en potens"
			},
			{
				name: "tall",
				description: "er potensen du vil opphøye det komplekse tallet i"
			}
		]
	},
	{
		name: "IMPRODUKT",
		description: "Returnerer produktet av 1 til 255 komplekse tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "imtall1",
				description: "imtall1,imtall2,... er fra 1 til 255 komplekse tall som skal multipliseres"
			},
			{
				name: "imtall2",
				description: "imtall1,imtall2,... er fra 1 til 255 komplekse tall som skal multipliseres"
			}
		]
	},
	{
		name: "IMREELL",
		description: "Returnerer den reelle koeffisienten til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne den reelle koeffisienten til"
			}
		]
	},
	{
		name: "IMROT",
		description: "Returnerer kvadratroten til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne kvadratroten til"
			}
		]
	},
	{
		name: "IMSEC",
		description: "Returnerer sekans til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne sekans til"
			}
		]
	},
	{
		name: "IMSECH",
		description: "Returnerer hyperbolsk sekans til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne hyperbolsk sekans til"
			}
		]
	},
	{
		name: "IMSIN",
		description: "Returnerer sinusen til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne sinusen til"
			}
		]
	},
	{
		name: "IMSINH",
		description: "Returnerer hyperbolsk sinus til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne hyperbolsk sinus til"
			}
		]
	},
	{
		name: "IMSUB",
		description: "Returnerer differansen mellom to komplekse tall.",
		arguments: [
			{
				name: "imtall1",
				description: "er det komplekse tallet imtall2 skal subtraheres fra"
			},
			{
				name: "imtall2",
				description: "er det komplekse tallet som skal subtraheres fra imtall1"
			}
		]
	},
	{
		name: "IMSUMMER",
		description: "Returnerer summen av to eller flere komplekse tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "imtall1",
				description: "er 1 til 255 komplekse tall som skal legges sammen"
			},
			{
				name: "imtall2",
				description: "er 1 til 255 komplekse tall som skal legges sammen"
			}
		]
	},
	{
		name: "IMTAN",
		description: "Returnerer tangens til et komplekst tall.",
		arguments: [
			{
				name: "imtall",
				description: "er det komplekse tallet du ønsker å finne tangens til"
			}
		]
	},
	{
		name: "INDEKS",
		description: "Returnerer en verdi eller referanse for cellen i skjæringspunktet av en bestemt rad eller kolonne, i et gitt celleområde.",
		arguments: [
			{
				name: "matrise",
				description: "er et celleområde eller en matrisekonstant."
			},
			{
				name: "rad_nr",
				description: "merker raden i matrisen eller referansen du vil returnere en verdi fra. Hvis argumentet utelates, må du bruke kolonne_nr"
			},
			{
				name: "kolonne_nr",
				description: "merker området i matrisen eller referansen du vil returnere en verdi fra. Hvis argumentet utelates, må du bruke rad_nr"
			}
		]
	},
	{
		name: "INDIREKTE",
		description: "Returnerer en referanse angitt av en tekststreng.",
		arguments: [
			{
				name: "ref",
				description: "er en referanse til en celle som inneholder en referanse i A1-stil eller R1C1-stil, et navn som er definert som en referanse, eller en referanse til en celle som en tekststreng"
			},
			{
				name: "a1",
				description: "er en logisk verdi som angir typen referanse i reftekst. R1C1-stil = USANN, A1-stil = SANN eller utelatt"
			}
		]
	},
	{
		name: "INFO",
		description: "Returnerer informasjon om det gjeldende operativmiljøet.",
		arguments: [
			{
				name: "type",
				description: "er tekst som angir hvilken type informasjon du vil returnere."
			}
		]
	},
	{
		name: "INVERS.BETA.FORDELING",
		description: "Returnerer den inverse til den kumulative betafordelingsfunksjonen for sannsynlig tetthet (BETA.FORDELING).",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet for betafordelingen"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen og må være større enn 0"
			},
			{
				name: "A",
				description: "er en valgfri nedre grense for x-intervallet. Hvis argumentet utelates, settes A til 0"
			},
			{
				name: "B",
				description: "er en valgfri øvre grense for x-intervallet. Hvis argumentet utelates, settes B til 1"
			}
		]
	},
	{
		name: "INVERS.KJI.FORDELING",
		description: "Returnerer den inverse av den høyre sannsynligheten til kjikvadratfordelingen.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til kjikvadratfordelingen, en verdi fra og med 0 til og med 1"
			},
			{
				name: "frihetsgrader",
				description: "er antallet frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "IR",
		description: "Returnerer internrenten for en serie kontantstrømmer.",
		arguments: [
			{
				name: "verdi",
				description: "er en matrise eller en referanse til celler som inneholder tall du vil beregne internrenten for"
			},
			{
				name: "antatt",
				description: "er et tall du antar er i nærheten av resultatet av funksjonen IR. Settes til 0,1 (10 prosent) hvis argumentet utelates"
			}
		]
	},
	{
		name: "ISO.AVRUND.GJELDENDE.MULTIPLUM",
		description: "Runder av et tall oppover til nærmeste heltall eller til nærmeste signifikante multiplum av en faktor.",
		arguments: [
			{
				name: "nummer",
				description: "er verdien du vil avrunde"
			},
			{
				name: "signifikans",
				description: "er det valgfrie multiplumet du vil avrunde tall til et multiplum av"
			}
		]
	},
	{
		name: "ISOUKENR",
		description: "Returnerer tallet for ISO-ukenummeret i året for en gitt dato.",
		arguments: [
			{
				name: "dato",
				description: "er dato-klokkeslett-koden som brukes av Spreadsheet til beregning av dato og klokkeslett"
			}
		]
	},
	{
		name: "IT",
		description: "Returnerer feilverdien #I/T (ikke tilgjengelig).",
		arguments: [
		]
	},
	{
		name: "KJEDE.SAMMEN",
		description: "Slår sammen flere tekststrenger til en tekststreng.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tekst1",
				description: "er 1 til 255 tekststrenger som skal slås sammen til en tekststreng, og kan være tekststrenger, tall eller referanser til enkelte celler"
			},
			{
				name: "tekst2",
				description: "er 1 til 255 tekststrenger som skal slås sammen til en tekststreng, og kan være tekststrenger, tall eller referanser til enkelte celler"
			}
		]
	},
	{
		name: "KJI.FORDELING",
		description: "Returnerer den høyre sannsynligheten til kjikvadratfordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil beregne fordelingen for, et ikke-negativt tall"
			},
			{
				name: "frihetsgrader",
				description: "er antallet frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "KJI.TEST",
		description: "Returnerer testen for uavhengighet: verdien fra kjikvadratfordelingen for observatoren og gjeldende frihetsgrader.",
		arguments: [
			{
				name: "faktisk",
				description: "er dataområdet som inneholder observasjonene som skal testes mot forventede verdier"
			},
			{
				name: "forventet",
				description: "er dataområdet som inneholder forholdet mellom produktet av rad- og kolonnesummer og hovedsummen"
			}
		]
	},
	{
		name: "KJIKVADRAT.FORDELING",
		description: "Returnerer venstre sannsynlighet for den kjikvadrerte fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien der du vil evaluere fordelingen, et ikke-negativt tall"
			},
			{
				name: "frihetsgrader",
				description: "er antall frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			},
			{
				name: "kumulative",
				description: "er en logisk verdi som funksjonen skal returnere: SANN gir den kumulative fordelingsfunksjonen; USANN gir funksjonen for sannsynlig tetthet"
			}
		]
	},
	{
		name: "KJIKVADRAT.FORDELING.H",
		description: "Returnerer den høyre sannsynligheten for den kjikvadrerte fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien der du vil evaluere fordelingen, et ikke-negativt tall"
			},
			{
				name: "frihetsgrader",
				description: "er antall frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "KJIKVADRAT.INV",
		description: "Returnerer den inverse av den venstre sannsynligheten for den kjikvadrerte fordelingen.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til den kjikvadrerte fordelingen, en verdi fra 0 til og med 1"
			},
			{
				name: "frihetsgrader",
				description: "er antall frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "KJIKVADRAT.INV.H",
		description: "Returnerer den inverse av den høyre sannsynligheten for den kjikvadrerte fordelingen.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til den kjikvadrerte fordelingen, en verdi fra 0 til og med 1"
			},
			{
				name: "frihetsgrader",
				description: "er antall frihetsgrader, et tall mellom 1 og 10^10, unntatt 10^10"
			}
		]
	},
	{
		name: "KJIKVADRAT.TEST",
		description: "Returnerer testen for uavhengighet: verdien fra den kjikvadrerte fordelingen for observatoren og gjeldende frihetsgrader.",
		arguments: [
			{
				name: "faktisk",
				description: "er dataområdet som inneholder observasjonene som skal testes mot forventede verdier"
			},
			{
				name: "forventet",
				description: "er dataområdet som inneholder forholdet mellom produktet av rad- og kolonnesummene og totalsummen"
			}
		]
	},
	{
		name: "KODE",
		description: "Returnerer en numerisk kode for det første tegnet i en tekststreng, i tegnsettet som din datamaskin bruker.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten der du vil finne koden for det første tegnet"
			}
		]
	},
	{
		name: "KOLONNE",
		description: "Returnerer kolonnenummeret for en referanse.",
		arguments: [
			{
				name: "ref",
				description: "er cellen eller celleområdet du vil vite kolonnenummeret til. Hvis dette utelates, brukes cellen som inneholder KOLONNE-funksjonen"
			}
		]
	},
	{
		name: "KOLONNER",
		description: "Returnerer antall kolonner i en matrise eller referanse.",
		arguments: [
			{
				name: "matrise",
				description: "er en matrise eller en matriseformel, eller en referanse til et celleområde, som du vil finne antall kolonner i"
			}
		]
	},
	{
		name: "KOMBINASJON",
		description: "Returnerer antall kombinasjoner for et gitt antall elementer.",
		arguments: [
			{
				name: "antall",
				description: "er totalt antall elementer"
			},
			{
				name: "valgt_antall",
				description: "er antall elementer i hver kombinasjon"
			}
		]
	},
	{
		name: "KOMBINASJONA",
		description: "Returnerer antallet kombinasjoner med repetisjoner for et gitt antall elementer.",
		arguments: [
			{
				name: "antall",
				description: "er totalt antall elementer"
			},
			{
				name: "valgt_antall",
				description: "er antallet elementer i hver kombinasjon"
			}
		]
	},
	{
		name: "KOMPLEKS",
		description: "Konverterer reelle og imaginære koeffisienter til et komplekst tall.",
		arguments: [
			{
				name: "reelt_tall",
				description: "er den reelle koeffisienten i det komplekse tallet"
			},
			{
				name: "imaginært_tall",
				description: "er den imaginære koeffisienten i det komplekse tallet"
			},
			{
				name: "suffiks",
				description: "er suffikset for den imaginære komponenten i det komplekse tallet"
			}
		]
	},
	{
		name: "KONFIDENS",
		description: "Returnerer konfidensintervallet til populasjonens gjennomsnitt ved å bruke en normalfordeling.",
		arguments: [
			{
				name: "alfa",
				description: "er signifikansnivået som brukes ved beregningen av konfidenskoeffisienten, et tall større enn 0 og mindre enn 1"
			},
			{
				name: "standardavvik",
				description: "er populasjonens standardavvik for dataområdet og antas å være kjent. Standardavvik må være større enn 0"
			},
			{
				name: "størrelse",
				description: "er størrelsen på utvalget"
			}
		]
	},
	{
		name: "KONFIDENS.NORM",
		description: "Returnerer konfidensintervallet til populasjonens middelverdi.",
		arguments: [
			{
				name: "alfa",
				description: "er signifikansnivået som brukes ved beregningen av konfidenskoeffisienten, et tall som er større enn 0 og mindre enn 1"
			},
			{
				name: "standardavvik",
				description: "er populasjonens standardavvik for dataområdet, og blir antatt å være kjent. Standardavvik må være større enn 0"
			},
			{
				name: "størrelse",
				description: "er størrelsen på utvalget"
			}
		]
	},
	{
		name: "KONFIDENS.T",
		description: "Returnerer konfidensintervallet til populasjonens middelverdi ved hjelp av en Student T-fordeling.",
		arguments: [
			{
				name: "alfa",
				description: "er signifikansnivået som brukes ved beregningen av konfidenskoeffisienten, et tall større enn 0 og mindre enn 1"
			},
			{
				name: "standardavvik",
				description: "er populasjonens standardavvik for dataområdet, og antas å være kjent. Standardavvik må være større enn 0"
			},
			{
				name: "størrelse",
				description: "er størrelsen på utvalget"
			}
		]
	},
	{
		name: "KONVERTER",
		description: "Konverterer et tall fra ett målesystem til et annet.",
		arguments: [
			{
				name: "tall",
				description: "er verdien i fra_enhet som skal konverteres"
			},
			{
				name: "fra_enhet",
				description: "er enheten for tall"
			},
			{
				name: "til_enhet",
				description: "er enheten for resultatet"
			}
		]
	},
	{
		name: "KORRELASJON",
		description: "Returnerer korrelasjonskoeffisienten mellom to datasett.",
		arguments: [
			{
				name: "matrise1",
				description: "er et celleområde med verdier. Verdiene må være tall eller navn, matriser eller referanser som inneholder tall"
			},
			{
				name: "matrise2",
				description: "er et annet celleområde med verdier. Verdiene må være tall, navn, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "KOVARIANS",
		description: "Returnerer kovariansen, gjennomsnittet av produktene av avvikene for hvert datapunktpar i to datasett.",
		arguments: [
			{
				name: "matrise1",
				description: "er det første celleområdet med heltall og må være tall, matriser eller referanser som inneholder tall"
			},
			{
				name: "matrise2",
				description: "er det andre celleområdet med heltall og må være tall, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "KOVARIANS.P",
		description: "Returnerer populasjonens kovarians, som er gjennomsnittet av produktene av avvikene for hvert datapunktpar i to datasett.",
		arguments: [
			{
				name: "matrise1",
				description: "er det første celleområdet med heltall, og må være tall, matriser eller referanser som inneholder tall"
			},
			{
				name: "matrise2",
				description: "er det andre celleområdet med heltall, og må være tall, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "KOVARIANS.S",
		description: "Returnerer utvalgets kovarians, som er gjennomsnittet av produktene av avvikene for hvert datapunktpar i to datasett.",
		arguments: [
			{
				name: "matrise1",
				description: "er det første celleområdet med heltall, og må være tall, matriser eller referanser som inneholder tall"
			},
			{
				name: "matrise2",
				description: "er det andre celleområdet med heltall, og må være tall, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "KURT",
		description: "Returnerer kurtosen til et datasett.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne kurtosen for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne kurtosen for"
			}
		]
	},
	{
		name: "KURVE",
		description: "Returnerer statistikk som beskriver en eksponentiell kurve som samsvarer med kjente datapunkter.",
		arguments: [
			{
				name: "kjente_y",
				description: "er et sett y-verdier du allerede kjenner i forholdet y = b*m^x"
			},
			{
				name: "kjente_x",
				description: "er et valgfritt sett med x-verdier som det kan hende du allerede kjenner i forholdet y = b*m^x"
			},
			{
				name: "konst",
				description: "er en logisk verdi. Konstanten b beregnes normalt hvis konst = SANN eller utelatt. B settes til 1 hvis konst = USANN"
			},
			{
				name: "statistikk",
				description: "er en logisk verdi. Returner flere regresjonsdata = SANN, returner m-koeffisienter og konstanten b = USANN eller utelatt"
			}
		]
	},
	{
		name: "KVARTIL",
		description: "Returnerer kvartilen for et datasett.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller celleområdet med numeriske verdier du vil finne kvartilverdien for"
			},
			{
				name: "kvartil",
				description: "er et tall. Minimumsverdi = 0, første kvartil = 1, medianverdi = 2, tredje kvartil = 3, maksimumsverdi = 4"
			}
		]
	},
	{
		name: "KVARTIL.EKS",
		description: "Returnerer kvartilen til et datasett basert på persentilverdier fra 0..1, eksklusive.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller celleområdet med numeriske verdier du vil finne kvartilverdien for"
			},
			{
				name: "kvart",
				description: "er et tall: minimumsverdi = 0; første kvartil = 1; medianverdi = 2; tredje kvartil = 3; maksimumsverdi = 4"
			}
		]
	},
	{
		name: "KVARTIL.INK",
		description: "Returnerer kvartilen til et datasett, basert på persentilverdier fra 0..1, inklusive.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller celleområdet med numeriske verdier du vil finne kvartilverdien for"
			},
			{
				name: "kvart",
				description: "er et tall: minimumsverdi = 0; første kvartil = 1; medianverdi = 2; tredje kvartil = 3; maksimumsverdi = 4"
			}
		]
	},
	{
		name: "KVOTIENT",
		description: "Returnerer heltallsdelen av en divisjon.",
		arguments: [
			{
				name: "teller",
				description: "er dividenden"
			},
			{
				name: "nevner",
				description: "er divisoren"
			}
		]
	},
	{
		name: "LENGDE",
		description: "Returnerer antall tegn i en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten du vil finne lengden på. Mellomrom telles som tegn"
			}
		]
	},
	{
		name: "LINAVS",
		description: "Returnerer den lineære verdiavskrivningen for et aktivum i en gitt periode.",
		arguments: [
			{
				name: "kostnad",
				description: "er den opprinnelige kostnaden til aktivumet"
			},
			{
				name: "restverdi",
				description: "er verdien ved slutten av avskrivningen"
			},
			{
				name: "levetid",
				description: "er antall perioder et aktivum blir avskrevet over (ofte kalt aktivumets økonomiske levetid)"
			}
		]
	},
	{
		name: "LN",
		description: "Returnerer den naturlige logaritmen for et tall.",
		arguments: [
			{
				name: "tall",
				description: "er det positive reelle tallet du vil finne den naturlige logaritmen til"
			}
		]
	},
	{
		name: "LOG",
		description: "Returnerer logaritmen til et tall med det grunntallet du angir.",
		arguments: [
			{
				name: "tall",
				description: "er det positive reelle tallet du vil finne logaritmen til"
			},
			{
				name: "grunntall",
				description: "er grunntallet i logaritmen. Settes til 10 hvis utelatt"
			}
		]
	},
	{
		name: "LOG10",
		description: "Returnerer logaritmen med grunntall 10 for et tall.",
		arguments: [
			{
				name: "tall",
				description: "er det positive reelle tallet du vil finne logaritmen med grunntall 10 til"
			}
		]
	},
	{
		name: "LOGINV",
		description: "Returnerer den inverse av den lognormale fordelingsfunksjonen, hvor In(x) har normalfordeling med parameterne median og standardavvik.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til den lognormale fordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "median",
				description: "er gjennomsnittet for ln(x)"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for ln(x), et positivt tall"
			}
		]
	},
	{
		name: "LOGNORM.FORDELING",
		description: "Returnerer den lognormale fordelingen av x, der ln(x) er normalfordelt med parameterne Gjennomsnitt og Standardavvik.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil evaluere funksjonen for, et positivt tall"
			},
			{
				name: "gjennomsnitt",
				description: "er gjennomsnittet til ln (x)"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for ln(x), et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi: Bruk SANN for kumulativ fordeling, bruk USANN for sannsynlig tetthet"
			}
		]
	},
	{
		name: "LOGNORM.INV",
		description: "Returnerer den inverse av den lognormale fordelingsfunksjonen, hvor In(x) har normalfordeling med parameterne middelverdi og standardavvik.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet knyttet til den lognormale fordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "middelverdi",
				description: "er middelverdien for ln(x)"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for ln(x), et positivt tall"
			}
		]
	},
	{
		name: "LOGNORMFORD",
		description: "Returnerer den kumulative lognormale fordelingen for x, hvor In(x) har normalfordeling med parameterne median og standardavvik.",
		arguments: [
			{
				name: "x",
				description: "er verdien funksjonen skal beregnes for, et positivt tall"
			},
			{
				name: "median",
				description: "er medianen for ln(x)"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for ln(x), et positivt tall"
			}
		]
	},
	{
		name: "MAKSA",
		description: "Returnerer den høyeste verdien i et verdisett. Ignorerer ikke logiske verdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne maksimumsverdien for"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne maksimumsverdien for"
			}
		]
	},
	{
		name: "MÅNED",
		description: "Returnerer måneden, et tall fra 1 (januar) til 12 (desember).",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato eller et klokkeslett som brukes av Spreadsheet"
			}
		]
	},
	{
		name: "MÅNEDSSLUTT",
		description: "Returnerer serienummeret for den siste dagen i måneden før eller etter et angitt antall måneder.",
		arguments: [
			{
				name: "start_dato",
				description: "er et serienummer for datoen som representerer startdatoen"
			},
			{
				name: "måneder",
				description: "er antall måneder før eller etter start_dato"
			}
		]
	},
	{
		name: "MDETERM",
		description: "Returnerer matrisedeterminanten til en matrise.",
		arguments: [
			{
				name: "matrise",
				description: "er en tallmatrise med likt antall rader og kolonner, enten et celleområde eller en matrisekonstant"
			}
		]
	},
	{
		name: "MEDIAN",
		description: "Returnerer medianen for settet av angitte verdier, altså den midterste verdien i rekken (eller gjennomsnittet av de to midterste) når verdiene er ordnet i stigende rekkefølge.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall du vil finne medianen for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall du vil finne medianen for"
			}
		]
	},
	{
		name: "MENHET",
		description: "Returnerer enhetsmatrisen for den angitte dimensjonen.",
		arguments: [
			{
				name: "dimensjon",
				description: "er et heltall som angir dimensjonen til enhetsmatrisen som du vil returnere"
			}
		]
	},
	{
		name: "MFM",
		description: "Returnerer minste felles multiplum.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 verdier du ønsker å finne minste felles multiplum for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 verdier du ønsker å finne minste felles multiplum for"
			}
		]
	},
	{
		name: "MIN",
		description: "Returnerer det laveste tallet i et verdisett. Ignorerer logiske verdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne minimumsverdien for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne minimumsverdien for"
			}
		]
	},
	{
		name: "MINA",
		description: "Returnerer den laveste verdien i et verdisett. Ignorerer ikke logiske verdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne minimumsverdien for"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne minimumsverdien for"
			}
		]
	},
	{
		name: "MINUTT",
		description: "Returnerer minuttet, et tall fra 0 til 59.",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato eller et klokkeslett som brukes av Spreadsheet, eller tekst i klokkeslettformat, for eksempel 16:48:00 eller 4:48:00 PM"
			}
		]
	},
	{
		name: "MINVERS",
		description: "Returnerer den inverse matrisen til matrisen som er lagret i en matrise.",
		arguments: [
			{
				name: "matrise",
				description: "er en tallmatrise med likt antall rader og kolonner, enten et celleområde eller en matrisekonstant"
			}
		]
	},
	{
		name: "MMULT",
		description: "Returnerer matriseproduktet av to matriser, en matrise med samme antall rader som matrise1 og kolonner som matrise2.",
		arguments: [
			{
				name: "matrise1",
				description: "er den første matrisen med tall som skal multipliseres. Må ha samme antall kolonner som matrise2 har rader"
			},
			{
				name: "matrise2",
				description: "er den første matrisen med tall som skal multipliseres. Må ha samme antall kolonner som matrise2 har rader"
			}
		]
	},
	{
		name: "MODIR",
		description: "Returnerer internrenten for en serie periodiske kontantstrømmer, og tar hensyn til både investeringskostnad og rente på reinvestering av kontanter.",
		arguments: [
			{
				name: "verdi",
				description: "er en matrise eller en referanse til celler som inneholder tall som representerer en serie betalinger (negative) og inntekter (positive) med regelmessige mellomrom"
			},
			{
				name: "kapital_rente",
				description: "er rentesatsen du betaler for penger som brukes i kontantstrømmene"
			},
			{
				name: "reinvesterings_rente",
				description: "er rentesatsen du mottar for kontantstrømmene når du reinvesterer dem"
			}
		]
	},
	{
		name: "MODUS",
		description: "Returnerer modus (modalverdien), eller den hyppigst forekommende verdien i en matrise eller et dataområde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall, som du vil finne modus for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall, som du vil finne modus for"
			}
		]
	},
	{
		name: "MODUS.MULT",
		description: "Returnerer en loddrett matrise av de hyppigste verdiene, eller de som gjentas oftest, i en matrise eller dataområde. For en vannrett matrise bruker du =TRANSPONER(MODUS.MULT(tall1,tall2,...)).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall du vil bruke modusen for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall du vil bruke modusen for"
			}
		]
	},
	{
		name: "MODUS.SNGL",
		description: "Returnerer den hyppigst forekommende, eller mest repeterte, verdien i en matrise eller et dataområde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne modus for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne modus for"
			}
		]
	},
	{
		name: "MOTTATT.AVKAST",
		description: "Returnerer summen som mottas ved forfallsdato for et fullinvestert verdipapir.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "investering",
				description: "er beløpet som er investert i verdipapiret"
			},
			{
				name: "diskonto",
				description: "er verdipapirets diskontosats"
			},
			{
				name: "basis",
				description: "angir typen datosystem som brukes"
			}
		]
	},
	{
		name: "MRUND",
		description: "Returnerer et tall avrundet til det ønskede multiplum.",
		arguments: [
			{
				name: "tall",
				description: "er verdien du vil runde av"
			},
			{
				name: "multiplum",
				description: "er det multiplum du vil runde av tallet til"
			}
		]
	},
	{
		name: "MULTINOMINELL",
		description: "Returnerer polynomet til et sett med tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 verdier du ønsker å finne polynomet til"
			},
			{
				name: "tall2",
				description: "er 1 til 255 verdier du ønsker å finne polynomet til"
			}
		]
	},
	{
		name: "N",
		description: "Konverterer verdier som ikke er tall, til tall, datoer til serienumre, SANN til 1 og alt annet til 0 (null).",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil konvertere"
			}
		]
	},
	{
		name: "N.MINST",
		description: "Returnerer den n-te laveste verdien i et datasett, for eksempel det femte laveste tallet.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller området med numeriske data du vil finne den n-te laveste verdien for"
			},
			{
				name: "n",
				description: "er posisjonen (fra den laveste verdien) i matrisen eller celleområdet som inneholder verdien som skal returneres"
			}
		]
	},
	{
		name: "N.STØRST",
		description: "Returnerer den n-te høyeste verdien i et datasett, for eksempel det femte høyeste tallet.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller området med numeriske data du vil finne den n-te høyeste verdien for"
			},
			{
				name: "n",
				description: "er posisjonen (fra den høyeste verdien) i matrisen eller celleområdet som inneholder verdien som skal returneres"
			}
		]
	},
	{
		name: "NÅ",
		description: "Returnerer gjeldende dato og klokkeslett formatert som dato og klokkeslett.",
		arguments: [
		]
	},
	{
		name: "NÅVERDI",
		description: "Returnerer nåverdien for en investering, det totale beløpet som en serie fremtidige innbetalinger er verdt i dag.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode. Bruk for eksempel 6 %/4 for kvartalsvise betalinger på 6 % årlig rente"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalinger i en annuitet"
			},
			{
				name: "betaling",
				description: "er innbetalingen som foretas i hver periode, og kan ikke endres i annuitetens løpetid"
			},
			{
				name: "sluttverdi",
				description: "er fremtidig verdi, eller kontantbalansen du vil oppnå etter at siste innbetaling er foretatt"
			},
			{
				name: "type",
				description: "er en logisk verdi. Innbetaling ved begynnelsen av perioden = 1, innbetaling ved slutten av perioden = 0 eller utelatt"
			}
		]
	},
	{
		name: "NEGBINOM.FORDELING",
		description: "Returnerer negativ binomisk fordeling, sjansen for at det vil bli tall_f fiaskoer før tall_s-te suksess, med sannsynlighet_s sjanse for suksess, hvor sannsynlighet_s er sannsynligheten for suksess.",
		arguments: [
			{
				name: "tall_f",
				description: "er antallet fiaskoer"
			},
			{
				name: "tall_s",
				description: "er terskeltallet for vellykkede forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for at forsøket lykkes, et tall mellom 0 og 1"
			}
		]
	},
	{
		name: "NEGBINOM.FORDELING.N",
		description: "Returnerer den negative binomiske fordelingen, sannsynligheten for at det vil være tall_f fiaskoer før tall_s-te suksess, der sannsynlighet_s er sannsynligheten for suksess.",
		arguments: [
			{
				name: "tall_f",
				description: "er antall mislykkede forsøk"
			},
			{
				name: "tall_s",
				description: "er terskeltallet for vellykkede forsøk"
			},
			{
				name: "sannsynlighet_s",
				description: "er sannsynligheten for et vellykket forsøket, et tall mellom 0 og 1"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi: Bruk SANN for kumulativ fordelingsfunksjon, bruk USANN for punktsannsynlighet"
			}
		]
	},
	{
		name: "NETT.ARBEIDSDAGER",
		description: "Returnerer antallet hele arbeidsdager mellom to datoer.",
		arguments: [
			{
				name: "startdato",
				description: "er et serielt datonummer som representerer startdatoen"
			},
			{
				name: "sluttdato",
				description: "er et serielt datonummer som representerer sluttdatoen"
			},
			{
				name: "helligdager",
				description: "er en valgfri liste med én eller flere datoer som skal utelates fra arbeidskalenderen, for eksempel nasjonale helligdager og flytende helligdager"
			}
		]
	},
	{
		name: "NETT.ARBEIDSDAGER.INTL",
		description: "Returnerer antall hele arbeidsdager mellom to datoer med egendefinerte helgeparametere.",
		arguments: [
			{
				name: "startdato",
				description: "er et seriedatonummer som representerer startdatoen"
			},
			{
				name: "sluttdato",
				description: "er et seriedatonummer som representerer sluttdatoen"
			},
			{
				name: "helg",
				description: "er et tall eller en streng som angir når helger forekommer"
			},
			{
				name: "helligdager",
				description: "er et valgfritt sett med ett eller flere seriedatonumre som du kan utelate fra arbeidskalenderen, for eksempel faste og flytende helligdager"
			}
		]
	},
	{
		name: "NNV",
		description: "Returnerer netto nåverdi for en investering basert på en rentesats og en serie fremtidige innbetalinger (negative verdier) og inntekter (positive verdier).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen over en periode"
			},
			{
				name: "verdi1",
				description: "er 1 til 254 argumenter som representerer utbetalingene og inntektene, med like mellomrom i tid. Alle inntreffer ved slutten av hver periode"
			},
			{
				name: "verdi2",
				description: "er 1 til 254 argumenter som representerer utbetalingene og inntektene, med like mellomrom i tid. Alle inntreffer ved slutten av hver periode"
			}
		]
	},
	{
		name: "NOMINELL",
		description: "Returnerer den årlige nominelle rentefoten.",
		arguments: [
			{
				name: "effektiv_rente",
				description: "er den effektive rentefoten"
			},
			{
				name: "perioder",
				description: "er antall sammensatte perioder per år"
			}
		]
	},
	{
		name: "NORM.FORDELING",
		description: "Returnerer normalfordelingen for angitt middelverdi og standardavvik.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil bruke fordelingen for"
			},
			{
				name: "median",
				description: "er den aritmetiske middelverdien for fordelingen"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for fordelingen, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi: Bruk SANN for den kumulative fordelingsfunksjonen, bruk USANN for funksjonen for sannsynlig tetthet"
			}
		]
	},
	{
		name: "NORM.INV",
		description: "Returnerer den inverse av den kumulative normalfordelingen for angitt gjennomsnitt og standardavvik.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet for normalfordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "median",
				description: "er gjennomsnittet (den aritmetiske middelverdien) av fordelingen"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for fordelingen, et positivt tall"
			}
		]
	},
	{
		name: "NORM.S.FORDELING",
		description: "Returnerer standard normalfordeling (har en middelverdi lik null og et standardavvik lik én).",
		arguments: [
			{
				name: "z",
				description: "er verdien du ønsker fordelingen for"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi som funksjonen skal returnere: SANN gir funksjonen for kumulativ fordeling; USANN gir funksjonen for sannsynlig tetthet"
			}
		]
	},
	{
		name: "NORM.S.INV",
		description: "Returnerer den inverse av standard kumulativ normalfordeling (har null som middelverdi og én som standardavvik).",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet som tilsvarer normalfordelingen, et tall fra og med 0 til og med 1"
			}
		]
	},
	{
		name: "NORMALFORDELING",
		description: "Returnerer den kumulative normalfordelingen for angitt median og standardavvik.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil finne fordelingen for"
			},
			{
				name: "median",
				description: "er medianen (den aritmetiske middelverdien) av fordelingen"
			},
			{
				name: "standardavvik",
				description: "er fordelingens standardavvik, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For kumulativ fordeling bruker du SANN. For punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "NORMALISER",
		description: "Returnerer en normalisert verdi fra en fordeling spesifisert ved gjennomsnitt og standardavvik.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil normalisere"
			},
			{
				name: "median",
				description: "er gjennomsnittet (den aritmetiske middelverdien) av fordelingen"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for fordelingen, et positivt tall"
			}
		]
	},
	{
		name: "NORMINV",
		description: "Returnerer den inverse av den kumulative normalfordelingen for angitt gjennomsnitt og standardavvik.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet for normalfordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "median",
				description: "er gjennomsnittet (den aritmetiske middelverdien) av fordelingen"
			},
			{
				name: "standardavvik",
				description: "er standardavviket for fordelingen, et positivt tall"
			}
		]
	},
	{
		name: "NORMSFORDELING",
		description: "Returnerer standard kumulativ normalfordeling (der gjennomsnittet er lik null og standardavviket er én).",
		arguments: [
			{
				name: "z",
				description: "er verdien du vil finne fordelingen for"
			}
		]
	},
	{
		name: "NORMSINV",
		description: "Returnerer den inverse av standard kumulativ normalfordeling (har et gjennomsnitt på null og standardavvik på én).",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er en sannsynlighet for normalfordelingen, et tall fra og med 0 til og med 1"
			}
		]
	},
	{
		name: "OBLIG.ANTALL",
		description: "Returnerer antall renteinnbetalinger mellom betalingsdato og forfallsdato.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "frekvens",
				description: "er antall renteinnbetalinger i året"
			},
			{
				name: "basis",
				description: "er typen datosystem som skal brukes"
			}
		]
	},
	{
		name: "OBLIG.DAG.FORRIGE",
		description: "Returnerer siste renteforfallsdato før betalingsdatoen.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "frekvens",
				description: "er antall renteinnbetalinger i året"
			},
			{
				name: "basis",
				description: "er typen datosystem som skal brukes"
			}
		]
	},
	{
		name: "OBLIG.DAGER.EF",
		description: "Returnerer neste rentedato etter betalingsdatoen.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "frekvens",
				description: "er antall renteinnbetalinger i året"
			},
			{
				name: "basis",
				description: "er typen datosystem som skal brukes"
			}
		]
	},
	{
		name: "OBLIG.DAGER.FF",
		description: "Returnerer antall dager fra begynnelsen av den rentebærende perioden til innløsningsdatoen.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "frekvens",
				description: "er antall renteinnbetalinger i året"
			},
			{
				name: "basis",
				description: "er typen datosystem som skal brukes"
			}
		]
	},
	{
		name: "OG",
		description: "Kontrollerer om alle argumenter er lik SANN, og returnerer SANN hvis alle argumentene er lik SANN.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "logisk1",
				description: "er 1 til 255 tilstander du vil teste, som hver kan være enten SANN eller USANN, og som kan være logiske verdier, matriser eller referanser"
			},
			{
				name: "logisk2",
				description: "er 1 til 255 tilstander du vil teste, som hver kan være enten SANN eller USANN, og som kan være logiske verdier, matriser eller referanser"
			}
		]
	},
	{
		name: "OKTTILBIN",
		description: "Konverterer et oktaltall til et binærtall.",
		arguments: [
			{
				name: "tall",
				description: "er oktaltallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "OKTTILDES",
		description: "Konverterer et oktaltall til et heltall i 10-tallsystemet.",
		arguments: [
			{
				name: "tall",
				description: "er oktaltallet du vil konvertere"
			}
		]
	},
	{
		name: "OKTTILHEKS",
		description: "Konverterer et oktaltall til et heksadesimalt tall.",
		arguments: [
			{
				name: "tall",
				description: "er oktaltallet du vil konvertere"
			},
			{
				name: "plasser",
				description: "er antallet tegn du skal bruke"
			}
		]
	},
	{
		name: "OMRÅDER",
		description: "Returnerer antall områder i en referanse. Området må bestå av sammenhengende celler eller en enkeltcelle.",
		arguments: [
			{
				name: "ref",
				description: "er en referanse til en celle eller et celleområde, og kan vise til flere områder"
			}
		]
	},
	{
		name: "OPPHØYD.I",
		description: "Returnerer resultatet av et tall opphøyd i en potens.",
		arguments: [
			{
				name: "tall",
				description: "er grunntallet, et reelt tall"
			},
			{
				name: "potens",
				description: "er eksponenten grunntallet blir opphøyd i"
			}
		]
	},
	{
		name: "PÅLØPT.FORFALLSRENTE",
		description: "Returnerer den påløpte renten for et verdipapir som betaler rente ved forfall.",
		arguments: [
			{
				name: "utstedt_dato",
				description: "er verdipapirets utstedelsesdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "rente",
				description: "er verdipapirets årlige rente"
			},
			{
				name: "pari",
				description: "er verdipapirets pariverdi"
			},
			{
				name: "basis",
				description: "er typen datosystem som skal brukes"
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
		description: "Returnerer produktmomentkorrelasjonskoeffisienten, Pearsons r.",
		arguments: [
			{
				name: "matrise1",
				description: "er et uavhengig verdisett"
			},
			{
				name: "matrise2",
				description: "er et avhengig verdisett"
			}
		]
	},
	{
		name: "PERIODER",
		description: "Returnerer antall perioder for en investering basert på periodiske, konstante innbetalinger og en fast rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode. Bruk for eksempel 6%/4 for kvartalsvise betalinger på 6 % årlig rente"
			},
			{
				name: "betaling",
				description: "er innbetalingen som foretas i hver periode, og kan ikke endres i innbetalinger løpetid"
			},
			{
				name: "nåverdi",
				description: "er nåverdien, eller det totale beløpet som en serie fremtidige innbetalinger er verdt i dag"
			},
			{
				name: "sluttverdi",
				description: "er fremtidig verdi, eller kontantbalansen du vil oppnå etter at siste innbetaling er foretatt. Hvis argumentet utelates, brukes null"
			},
			{
				name: "type",
				description: "er en logisk verdi. Innbetaling ved begynnelsen av perioden = 1, innbetaling ved slutten av perioden = 0 eller utelatt"
			}
		]
	},
	{
		name: "PERMUTASJONA",
		description: "Returnerer antallet permutasjoner for et gitt antall objekter (med repetisjoner) som kan velges fra det totale antallet objekter.",
		arguments: [
			{
				name: "antall",
				description: "er det totale antallet objekter"
			},
			{
				name: "valgt_antall",
				description: "er antall objekter i hver permutasjon"
			}
		]
	},
	{
		name: "PERMUTER",
		description: "Returnerer antall permutasjoner for et gitt antall objekter som kan velges fra det totale antall objekter.",
		arguments: [
			{
				name: "antall",
				description: "er det totale antall objekter"
			},
			{
				name: "valgt_antall",
				description: "er antall objekter i hver permutasjon"
			}
		]
	},
	{
		name: "PERSENTIL",
		description: "Returnerer det n-te persentilet av verdiene i et område.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet som definerer relativ fordeling"
			},
			{
				name: "n",
				description: "er persentilverdien som ligger i området fra og med 0 til og med 1"
			}
		]
	},
	{
		name: "PERSENTIL.EKS",
		description: "Returnerer den k-te persentilen av verdiene i et område, der k er i området 0..1, eksklusive.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet som definerer relativ fordeling"
			},
			{
				name: "k",
				description: "er persentilverdien som er mellom 0 og 1, inklusive"
			}
		]
	},
	{
		name: "PERSENTIL.INK",
		description: "Returnerer den k-te persentilen av verdiene i et område, der k er i området 0..1, inklusive.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet som definerer relativ fordeling"
			},
			{
				name: "k",
				description: "er persentilverdien som er mellom 0 og 1, inklusive"
			}
		]
	},
	{
		name: "PHI",
		description: "Returnerer verdien av tetthetsfunksjonen for en standard normalfordeling.",
		arguments: [
			{
				name: "x",
				description: "er tallet du vil finne tetthet for standard normalfordeling for"
			}
		]
	},
	{
		name: "PI",
		description: "Returnerer verdien av Pi, 3,14159265358979, med 15 desimalers nøyaktighet.",
		arguments: [
		]
	},
	{
		name: "POISSON",
		description: "Returnerer Poisson-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er antallet hendelser"
			},
			{
				name: "median",
				description: "er den forventede numeriske verdien, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For kumulativ Poisson-sannsynlighet bruker du SANN. For Poisson-punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "POISSON.FORDELING",
		description: "Returnerer Poisson-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er antallet hendelser"
			},
			{
				name: "median",
				description: "er den forventede numeriske verdien, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For kumulativ Poisson-sannsynlighet bruker du SANN, for Poisson-punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "PRIS.DISKONTERT",
		description: "Returnerer prisen per pålydende kr 100 for et diskontert verdipapir.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "diskonto",
				description: "er verdipapirets diskontosats"
			},
			{
				name: "innløsningsverdi",
				description: "er verdipapirets innløsningsverdi per pålydende kr 100"
			},
			{
				name: "basis",
				description: "angir typen datosystem som brukes"
			}
		]
	},
	{
		name: "PRODUKT",
		description: "Multipliserer alle tall som gis som argumenter.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall, logiske verdier eller tall i tekstformat du vil multiplisere"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall, logiske verdier eller tall i tekstformat du vil multiplisere"
			}
		]
	},
	{
		name: "PROGNOSE",
		description: "Beregner eller forutsier en fremtidig verdi langs en lineær trend på grunnlag av eksisterende verdier.",
		arguments: [
			{
				name: "x",
				description: "er datapunktet du vil forutsi en verdi for, og må være en numerisk verdi"
			},
			{
				name: "kjente_y",
				description: "er den avhengige matrisen eller det avhengige området med numeriske data"
			},
			{
				name: "kjente_x",
				description: "er den uavhengige matrisen eller det uavhengige området med numeriske data. Variansen av kjente_x kan ikke være null"
			}
		]
	},
	{
		name: "PROSENTDEL",
		description: "Returnerer rangeringen av en verdi i et datasett som en prosent av datasettet.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet med numeriske verdier som definerer relativ fordeling"
			},
			{
				name: "x",
				description: "er verdien du vil vite rangeringen for"
			},
			{
				name: "gjeldende_sifre",
				description: "er en valgfri verdi som identifiserer antallet signifikante sifre i den prosentdelen som returneres. Hvis argumentet utelates, brukes tre sifre (0,xxx %)"
			}
		]
	},
	{
		name: "PROSENTDEL.EKS",
		description: "Returnerer rangeringen av en verdi i et datasett som en prosentdel av datasettet som en prosentdel (0..1, eksklusive) av datasettet.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet med numeriske verdier som definerer relativ fordeling"
			},
			{
				name: "x",
				description: "er verdien du vil finne rangeringen for"
			},
			{
				name: "signifikans",
				description: "er en valgfri verdi som identifiserer antallet signifikante sifre for returnert prosentandel, tre sifre hvis utelatt (0,xxx %)"
			}
		]
	},
	{
		name: "PROSENTDEL.INK",
		description: "Returnerer rangeringen av en verdi i et datasett som en prosentdel av datasettet som en prosentdel (0..1, inklusive) av datasettet.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet med numeriske verdier som definerer relativ fordeling"
			},
			{
				name: "x",
				description: "er verdien du vil finne rangeringen for"
			},
			{
				name: "signifikans",
				description: "er en valgfri verdi som identifiserer antallet signifikante sifre for returnert prosentandel, tre sifre hvis utelatt (0,xxx %)"
			}
		]
	},
	{
		name: "PVARIGHET",
		description: "Returnerer antallet perioder som kreves av en investering for å nå en angitt verdi.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode."
			},
			{
				name: "nåverdi",
				description: "er nåverdien til investeringen"
			},
			{
				name: "sluttverdi",
				description: "er den ønskede fremtidige verdien av investeringen"
			}
		]
	},
	{
		name: "RAD",
		description: "Returnerer radnummeret for en referanse.",
		arguments: [
			{
				name: "ref",
				description: "er cellen eller celleområdet du vil vite radnummeret til. Hvis argumentet utelates, returneres cellen som inneholder RAD-funksjonen"
			}
		]
	},
	{
		name: "RADER",
		description: "Returnerer antall rader i en referanse eller en matrise.",
		arguments: [
			{
				name: "matrise",
				description: "er en matrise, en matriseformel eller en referanse til et celleområde du vil vite antall rader i"
			}
		]
	},
	{
		name: "RADIANER",
		description: "Konverterer grader til radianer.",
		arguments: [
			{
				name: "vinkel_i_grader",
				description: "er en vinkel, angitt i grader, som du vil konvertere"
			}
		]
	},
	{
		name: "RANG",
		description: "Returnerer rangeringen av et tall i en liste over tall. Tallet rangeres etter størrelsen i forhold til andre verdier i listen.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil finne rangeringen til"
			},
			{
				name: "ref",
				description: "er en matrise med, eller en referanse til, en liste over tall. Ikke-numeriske verdier ignoreres"
			},
			{
				name: "rekkefølge",
				description: "er et tall. Rangering i listen sortert synkende = 0 eller utelatt. Rangering i listen sortert stigende = en hvilken som helst annen verdi enn null"
			}
		]
	},
	{
		name: "RANG.EKV",
		description: "Returnerer rangeringen for et tall i en liste med tall: størrelsen i forhold til andre verdier i listen. Hvis mer enn én verdi har samme rangering, returneres den høyeste rangeringen for det verdisettet.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil finne rangeringen for"
			},
			{
				name: "ref",
				description: "er en matrise av, eller en referanse til, en liste med tall. Ikke-numeriske verdier ignoreres"
			},
			{
				name: "rekkefølge",
				description: "er et tall: rangert i listen som er sortert synkende = 0 eller utelatt, rangert i listen som er sortert stigende = en annen verdi enn null"
			}
		]
	},
	{
		name: "RANG.GJSN",
		description: "Returnerer rangeringen for et tall i en liste med tall: størrelsen i forhold til andre verdier i listen, hvis mer enn én verdi har samme rangering, returneres gjennomsnittlig rangering.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil finne rangeringen for"
			},
			{
				name: "ref",
				description: "er en matrise av, eller referanse til, en liste med tall. Ikke-numeriske verdier ignoreres"
			},
			{
				name: "rekkefølge",
				description: "er et tall: rangert i listen som er sortert synkende = 0 eller utelatt, rangert i listen som er sortert stigende = en annen verdi enn null"
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
		name: "RAVDRAG",
		description: "Returnerer betalte renter på en investering for en gitt periode, basert på periodiske, konstante innbetalinger og en fast rentesats.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode. Bruk for eksempel 6%/4 for kvartalsvise betalinger med 6 % årlig rente"
			},
			{
				name: "periode",
				description: "er perioden du vil finne renten for, og må være et tall mellom 1 og antall_innbet"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalingsperioder i en investering"
			},
			{
				name: "nåverdi",
				description: "er nåverdien, eller det totale beløpet som en serie fremtidige innbetalinger er verdt i dag"
			},
			{
				name: "sluttverdi",
				description: "er fremtidig verdi, eller kontantbalansen du vil oppnå etter at siste innbetaling er foretatt. Hvis argumentet utelates, blir sluttverdi satt til 0"
			},
			{
				name: "type",
				description: "er en logisk verdi som angir når betalingen forfaller. Slutten av perioden = 0 eller utelatt, begynnelsen av perioden = 1"
			}
		]
	},
	{
		name: "REALISERT.AVKASTNING",
		description: "Returnerer en ekvivalent rentesats for vekst for en investering.",
		arguments: [
			{
				name: "antall_innbet",
				description: "er antallet perioder for investeringen"
			},
			{
				name: "nåverdi",
				description: "er nåverdien til investeringen"
			},
			{
				name: "sluttverdi",
				description: "er den fremtidige verdien av investeringen"
			}
		]
	},
	{
		name: "RENSK",
		description: "Fjerner alle tegn som ikke kan skrives ut, fra teksten.",
		arguments: [
			{
				name: "tekst",
				description: "er all regnearkinformasjon der du vil fjerne tegn som ikke kan skrives ut"
			}
		]
	},
	{
		name: "RENTE",
		description: "Returnerer rentesatsen per periode for et lån eller en investering. Bruk for eksempel 6%/4 for kvartalsvise betalinger på 6 % årlig rente.",
		arguments: [
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalingsperioder for lånet eller investeringen"
			},
			{
				name: "betaling",
				description: "er innbetalingen som foretas i hver periode, og kan ikke endres i lånets eller investeringens løpetid"
			},
			{
				name: "nåverdi",
				description: "er nåverdien, eller det totale beløpet som en serie fremtidige innbetalinger er verdt i dag"
			},
			{
				name: "sluttverdi",
				description: "er fremtidig verdi, eller kontantbalansen du vil oppnå etter at siste innbetaling er foretatt. Hvis argumentet utelates, brukes sluttverdi = 0"
			},
			{
				name: "type",
				description: "er en logisk verdi. Innbetaling ved begynnelsen av perioden = 1, innbetaling ved slutten av perioden = 0 eller utelatt"
			},
			{
				name: "antatt",
				description: "er ditt anslag for hva rentesatsen vil være. Hvis argumentet utelates, settes anslag til 0,1 (10 prosent)"
			}
		]
	},
	{
		name: "RENTESATS",
		description: "Returnerer rentefoten av et fullfinansiert verdipapir.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er verdipapirets betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er verdipapirets forfallsdato uttrykt som et serienummer"
			},
			{
				name: "investering",
				description: "er beløpet som er investert i verdipapiret"
			},
			{
				name: "innløsningsverdi",
				description: "er beløpet som mottas ved forfallsdato"
			},
			{
				name: "basis",
				description: "angir typen datosystem som brukes"
			}
		]
	},
	{
		name: "REST",
		description: "Returnerer resten når et tall divideres med en divisor.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil finne resten for etter at divisjonen er utført"
			},
			{
				name: "divisor",
				description: "er det tallet som tall divideres med"
			}
		]
	},
	{
		name: "RETTLINJE",
		description: "Returnerer statistikk som beskriver en lineær trend som samsvarer med kjente datapunkter, ved å tilpasse en rett linje beregnet med minste kvadraters metode.",
		arguments: [
			{
				name: "kjente_y",
				description: "er et sett y-verdier du allerede kjenner, i forholdet y = mx + b"
			},
			{
				name: "kjente_x",
				description: "er et valgfritt sett med x-verdier det kan hende du allerede kjenner, i forholdet y = mx + b"
			},
			{
				name: "konst",
				description: "er en logisk verdi. Konstanten b beregnes normalt hvis konst = SANN eller utelatt, b settes til 0 hvis konst = USANN"
			},
			{
				name: "statistikk",
				description: "er en logisk verdi. Returner flere regresjonsdata = SANN, returner m-koeffisienter og konstanten b = USANN eller utelatt"
			}
		]
	},
	{
		name: "RKVADRAT",
		description: "Returnerer kvadratet av produktmomentkorrelasjonskoeffisienten (Pearsons r) gjennom de gitte datapunktene.",
		arguments: [
			{
				name: "kjente_y",
				description: "er en matrise eller et område som inneholder datapunkter, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			},
			{
				name: "kjente_x",
				description: "er en matrise eller et område som inneholder datapunkter, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "ROMERTALL",
		description: "Konverterer et arabertall til et romertall, som tekst.",
		arguments: [
			{
				name: "tall",
				description: "er det arabertallet du vil konvertere"
			},
			{
				name: "form",
				description: "er tallet som angir hvilken type romertall du vil bruke."
			}
		]
	},
	{
		name: "ROT",
		description: "Returnerer kvadratroten av et tall.",
		arguments: [
			{
				name: "tall",
				description: "er tallet du vil finne kvadratroten av"
			}
		]
	},
	{
		name: "ROTPI",
		description: "Returnerer kvadratroten av (tall * pi).",
		arguments: [
			{
				name: "tall",
				description: "er tallet som pi skal multipliseres med"
			}
		]
	},
	{
		name: "RTD",
		description: "Henter sanntidsdata fra et program som støtter COM-automatisering.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "progID",
				description: "er navnet på ProgID-en til et registrert COM-automatiseringstillegg. Legg ved navn i anførselstegn"
			},
			{
				name: "server",
				description: "er navnet på serveren som skal kjøre tillegget. Legg ved navn i anførselstegn. Bruk en tom streng hvis tillegget kjøres lokalt"
			},
			{
				name: "emne1",
				description: "er 1 til 38 parametere som spesifiserer en datadel"
			},
			{
				name: "emne2",
				description: "er 1 til 38 parametere som spesifiserer en datadel"
			}
		]
	},
	{
		name: "SAMLET.HOVEDSTOL",
		description: "Returnerer den kumulative hovedstolen på et lån mellom to perioder.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalinger"
			},
			{
				name: "nåverdi",
				description: "er nåværende verdi"
			},
			{
				name: "start_periode",
				description: "er den første perioden i beregningen"
			},
			{
				name: "slutt_periode",
				description: "er den siste perioden i beregningen"
			},
			{
				name: "type",
				description: "er en verdi som angir når betalingen forfaller"
			}
		]
	},
	{
		name: "SAMLET.RENTE",
		description: "Returnerer den kumulative renten på et lån mellom to perioder.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalinger"
			},
			{
				name: "nåverdi",
				description: "er nåværende verdi"
			},
			{
				name: "start_periode",
				description: "er den første perioden i beregningen"
			},
			{
				name: "slutt_periode",
				description: "er den siste perioden i beregningen"
			},
			{
				name: "type",
				description: "er en verdi som angir når betalingen forfaller"
			}
		]
	},
	{
		name: "SAMMENLIGNE",
		description: "Returnerer den relative posisjonen til et element i en matrise som tilsvarer en angitt verdi i en angitt rekkefølge.",
		arguments: [
			{
				name: "søkeverdi",
				description: "er verdien du bruker for å finne ønsket verdi i matrisen: et tall, tekst, en logisk verdi eller en referanse til en av disse"
			},
			{
				name: "søkematrise",
				description: "er et sammenhengende celleområde som inneholder mulige oppslagsverdier, en matrise med verdier eller en referanse til en matrise"
			},
			{
				name: "type",
				description: "er tallet 1, 0 eller -1, som bestemmer hvilken verdi som skal returneres."
			}
		]
	},
	{
		name: "SANN",
		description: "Returnerer den logiske verdien SANN.",
		arguments: [
		]
	},
	{
		name: "SANNSYNLIG",
		description: "Returnerer sannsynligheten for at en verdi ligger mellom to ytterpunkter i et område eller er lik et nedre ytterpunkt.",
		arguments: [
			{
				name: "x_område",
				description: "er området med numeriske verdier av x som det er knyttet sannsynligheter til"
			},
			{
				name: "utfallsområde",
				description: "er et et sett sannsynligheter knyttet til verdiene i x_område, verdier mellom 0 og 1, unntatt 0"
			},
			{
				name: "nedre_grense",
				description: "er den nedre grensen for verdien du vil finne en sannsynlighet for"
			},
			{
				name: "øvre_grense",
				description: "er den valgfrie øvre grensen for verdien. Hvis argumentet utelates, returnerer SANNSYNLIG sannsynligheten for at verdiene i x_område er lik nedre_grense"
			}
		]
	},
	{
		name: "SEC",
		description: "Returnerer sekans for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne sekans til, i radianer"
			}
		]
	},
	{
		name: "SECH",
		description: "Returnerer hyperbolsk sekans for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne hyperbolsk sekans til, i radianer"
			}
		]
	},
	{
		name: "SEKUND",
		description: "Returnerer sekundet, et tall fra 0 til 59.",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato eller et klokkeslett som brukes av Spreadsheet, eller tekst i klokkeslettformat, for eksempel 16:48:47 eller 4:48:47 PM"
			}
		]
	},
	{
		name: "SFF",
		description: "Returnerer den største felles divisor.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 verdier"
			},
			{
				name: "tall2",
				description: "er 1 til 255 verdier"
			}
		]
	},
	{
		name: "SIN",
		description: "Returnerer sinus for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne sinus til, i radianer. Grader * PI()/180 = radianer"
			}
		]
	},
	{
		name: "SINH",
		description: "Returnerer den hyperbolske sinus til et tall.",
		arguments: [
			{
				name: "tall",
				description: "er ethvert reelt tall"
			}
		]
	},
	{
		name: "SKJÆRINGSPUNKT",
		description: "Beregner punktet hvor en linje skjærer y-aksen ved å bruke en regresjonslinje for beste tilpasning som tegnes gjennom de kjente x- og y-verdiene.",
		arguments: [
			{
				name: "kjente_y",
				description: "er det avhengige settet observasjoner eller data, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			},
			{
				name: "kjente_x",
				description: "er det uavhengige settet observasjoner eller data, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "SKJEVFORDELING",
		description: "Returnerer skjevheten for en fordeling, et mål for en fordelings asymmetri i forhold til gjennomsnittet.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne skjevheten for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne skjevheten for"
			}
		]
	},
	{
		name: "SKJEVFORDELING.P",
		description: "Returnerer skjevheten for en fordeling basert på en populasjon, et mål for en fordelingsasymmetri i forhold til gjennomsnittet.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 254 tall eller navn, matriser eller referanser som inneholder tall som du vil finne populasjonsskjevheten for"
			},
			{
				name: "tall2",
				description: "er 1 til 254 tall eller navn, matriser eller referanser som inneholder tall som du vil finne populasjonsskjevheten for"
			}
		]
	},
	{
		name: "SLÅ.OPP",
		description: "Slår opp en verdi enten fra et enrads- eller enkolonnes-område eller fra en matrise. Angitt for bakoverkompatibilitet.",
		arguments: [
			{
				name: "søkeverdi",
				description: "er en verdi som funksjonen SLÅ.OPP søker etter i søkematrisen. Kan være et tall, tekst, en logisk verdi eller et navn eller en referanse til en verdi"
			},
			{
				name: "søkematrise",
				description: "er et celleområde som bare inneholder én rad eller én kolonne med tekst, tall eller logiske verdier, ordnet i stigende rekkefølge"
			},
			{
				name: "resultatvektor",
				description: "er et celleområde som bare inneholder én rad eller én kolonne, av samme størrelse som søkematrisen"
			}
		]
	},
	{
		name: "SLUTTVERDI",
		description: "Returnerer den fremtidige verdien av en investering basert på periodiske, konstante innbetalinger og en fast rente.",
		arguments: [
			{
				name: "rente",
				description: "er rentesatsen per periode. Bruk for eksempel 6%/4 for kvartalsvise betalinger på 6 % årlig rente"
			},
			{
				name: "antall_innbet",
				description: "er det totale antall innbetalingsperioder i en investering"
			},
			{
				name: "innbetaling",
				description: "er innbetalingen som foretas i hver periode, og kan ikke endres i investeringens løpetid"
			},
			{
				name: "nåverdi",
				description: "er nåverdien, eller det totale beløpet som en serie fremtidige innbetalinger er verdt i dag. Hvis argumentet utelates, blir nåverdi satt til 0"
			},
			{
				name: "type",
				description: "er en verdi som angir når innbetalingen forfaller. Innbetaling ved begynnelsen av perioden = 1, innbetaling ved slutten av perioden = 0 eller utelatt"
			}
		]
	},
	{
		name: "SMÅ",
		description: "Konverterer alle bokstaver i en tekststreng til små bokstaver.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten du vil konvertere til små bokstaver. Tegn i teksten som ikke er bokstaver, endres ikke"
			}
		]
	},
	{
		name: "SØK",
		description: "Returnerer tallet for tegnet hvor et bestemt tegn eller en tekststreng først blir funnet, lest fra venstre mot høyre (skiller ikke mellom små og store bokstaver).",
		arguments: [
			{
				name: "finn",
				description: "er teksten du vil søke etter. Du kan bruke jokertegnene ? og *. Bruk ~? og ~* når du vil finne tegnene ? og *"
			},
			{
				name: "innen_tekst",
				description: "er teksten der du vil søke etter argumentet finn"
			},
			{
				name: "startpos",
				description: "er nummeret til det tegnet i innen_tekst der du vil begynne søket, lest fra venstre mot høyre. Hvis argumentet utelates, brukes 1"
			}
		]
	},
	{
		name: "STANDARDFEIL",
		description: "Returnerer standardfeilen til den predikerte y-verdien for hver x i en regresjon.",
		arguments: [
			{
				name: "kjente_y",
				description: "er en matrise eller et område som inneholder avhengige datapunkter, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			},
			{
				name: "kjente_x",
				description: "er en matrise eller et område som inneholder uavhengige datapunkter, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "STDAV",
		description: "Beregner standardavvik basert på et utvalg (ignorerer logiske verdier og tekst i utvalget).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall som svarer til et utvalg fra en populasjon, og kan være tall eller referanser som inneholder tall"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall som svarer til et utvalg fra en populasjon, og kan være tall eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "STDAV.P",
		description: "Beregner standardavvik basert på hele populasjonen (ignorerer logiske verdier og tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall som utgjør en populasjon, og kan være tall eller referanser som inneholder tall"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall som utgjør en populasjon, og kan være tall eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "STDAV.S",
		description: "Beregner standardavvik basert på et utvalg (ignorerer logiske verdier og tekst i utvalget).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall som svarer til et utvalg fra en populasjon, og kan være tall eller referanser som inneholder tall"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall som svarer til et utvalg fra en populasjon, og kan være tall eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "STDAVP",
		description: "Beregner standardavvik basert på hele populasjonen (ignorerer logiske verdier og tekst).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall som tilsvarer en populasjon, og kan være tall eller referanser som inneholder tall"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall som tilsvarer en populasjon, og kan være tall eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "STDAVVIKA",
		description: "Estimerer standardavvik basert på et utvalg, inkludert logiske verdier og tekst. Tekst og den logiske verdien USANN har verdien 0, den logiske verdien SANN har verdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 verdier som utgjør et utvalg fra en populasjon og kan være verdier eller navn eller referanser til verdier"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 verdier som utgjør et utvalg fra en populasjon og kan være verdier eller navn eller referanser til verdier"
			}
		]
	},
	{
		name: "STDAVVIKPA",
		description: "Beregner standardavvik basert på at argumentene beskriver hele populasjonen, inkludert logiske verdier og tekst. Tekst og den logiske verdien USANN har verdien 0, den logiske verdien SANN har verdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 verdier som utgjør en populasjon, og kan være verdier eller navn, matriser eller referanser som inneholder verdier"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 verdier som utgjør en populasjon, og kan være verdier eller navn, matriser eller referanser som inneholder verdier"
			}
		]
	},
	{
		name: "STIGNINGSTALL",
		description: "Returnerer stigningstallet for den lineære regresjonslinjen gjennom de gitte datapunktene.",
		arguments: [
			{
				name: "kjente_y",
				description: "er en matrise eller et celleområde som inneholder numerisk avhengige datapunkter, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			},
			{
				name: "kjente_x",
				description: "er settet med uavhengige datapunkter, og kan være tall eller navn, matriser eller referanser som inneholder tall"
			}
		]
	},
	{
		name: "STOR.FORBOKSTAV",
		description: "Konverterer en tekststreng med hensyn til små og store bokstaver. Den første bokstaven i hvert ord får stor bokstav, og alle andre bokstaver konverterers til små bokstaver.",
		arguments: [
			{
				name: "tekst",
				description: "er tekst som står mellom anførselstegn, en formel som returnerer tekst, eller en referanse til en celle som inneholder tekst du vil skrive med store forbokstaver"
			}
		]
	},
	{
		name: "STORE",
		description: "Konverterer en tekststreng til store bokstaver.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten du vil konvertere til store bokstaver, en referanse eller en tekststreng"
			}
		]
	},
	{
		name: "STØRST",
		description: "Returnerer maksimumsverdien i et verdisett. Ignorerer logiske verdier og tekst.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne maksimumsverdien for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall, tomme celler, logiske verdier eller teksttall du vil finne maksimumsverdien for"
			}
		]
	},
	{
		name: "SUMMER",
		description: "Summerer alle tallene i et celleområde.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 argumenter som skal summeres. Logiske verdier og tekst ignoreres i celler, men tas med hvis de skrives inn som argumenter"
			},
			{
				name: "tall2",
				description: "er 1 til 255 argumenter som skal summeres. Logiske verdier og tekst ignoreres i celler, men tas med hvis de skrives inn som argumenter"
			}
		]
	},
	{
		name: "SUMMER.HVIS.SETT",
		description: "Legger sammen cellene som angis av et gitt sett med vilkår eller kriterier.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "summeringsområde",
				description: "er cellene som skal summeres"
			},
			{
				name: "kriterieområde",
				description: "er celleområdet du vil evaluere for det spesifikke vilkåret"
			},
			{
				name: "kriterium",
				description: "er kriteriet i form av tall, uttrykk eller tekst som definerer hvilke celler som skal summeres"
			}
		]
	},
	{
		name: "SUMMER.REKKE",
		description: "Returnerer summen av en geometrisk rekke, basert på formelen.",
		arguments: [
			{
				name: "x",
				description: "er innsettingsverdien til potensrekken"
			},
			{
				name: "n",
				description: "er den første eksponenten du vil opphøye x i"
			},
			{
				name: "m",
				description: "er verdien du vil øke n med for hvert ledd i rekken"
			},
			{
				name: "koeffisienter",
				description: "er et sett av koeffisienter som hver påfølgende potens av x skal multipliseres med"
			}
		]
	},
	{
		name: "SUMMERHVIS",
		description: "Summerer cellene som tilfredsstiller en gitt betingelse eller et gitt vilkår.",
		arguments: [
			{
				name: "område",
				description: "er celleområdet du vil beregne"
			},
			{
				name: "vilkår",
				description: "er betingelsen eller vilkåret i form av et tall, et uttrykk eller en tekst, som definerer hvilke celler som skal summeres"
			},
			{
				name: "summeringsområde",
				description: "er cellene som skal summeres. Hvis argumentet utelates, brukes cellene i området"
			}
		]
	},
	{
		name: "SUMMERKVADRAT",
		description: "Returnerer summen av de kvadrerte argumentene. Argumentene kan være tall eller matriser, navn eller referanser til celler som inneholder tall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne summen av kvadratene for"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tall eller navn, matriser eller referanser som inneholder tall som du vil finne summen av kvadratene for"
			}
		]
	},
	{
		name: "SUMMERPRODUKT",
		description: "Returnerer summen av produktene til tilsvarende områder eller matriser.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "matrise1",
				description: "er 2 til 255 matriser du vil multiplisere og så summere komponentene til. Alle matrisene må ha samme dimensjoner"
			},
			{
				name: "matrise2",
				description: "er 2 til 255 matriser du vil multiplisere og så summere komponentene til. Alle matrisene må ha samme dimensjoner"
			},
			{
				name: "matrise3",
				description: "er 2 til 255 matriser du vil multiplisere og så summere komponentene til. Alle matrisene må ha samme dimensjoner"
			}
		]
	},
	{
		name: "SUMMERX2MY2",
		description: "Summerer differansen mellom kvadratene av to tilsvarende områder eller matriser.",
		arguments: [
			{
				name: "matrise_x",
				description: "er det første området eller den første matrisen med tall, og kan være et tall eller et navn, en matrise eller en referanse som inneholder tall"
			},
			{
				name: "matrise_y",
				description: "er det andre området eller den andre matrisen med tall, og kan være et tall eller et navn, en matrise eller en referanse som inneholder tall"
			}
		]
	},
	{
		name: "SUMMERX2PY2",
		description: "Returnerer totalsummen av summene av kvadratene for tall i to tilsvarende områder eller matriser.",
		arguments: [
			{
				name: "matrise_x",
				description: "er det første området eller den første matrisen med tall, og kan være et tall eller et navn, en matrise eller en referanse som inneholder tall"
			},
			{
				name: "matrise_y",
				description: "er det andre området eller den andre matrisen med tall, og kan være et tall eller et navn, en matrise eller en referanse som inneholder tall"
			}
		]
	},
	{
		name: "SUMMERXMY2",
		description: "Summerer kvadratene av differansene mellom to tilsvarende områder eller matriser.",
		arguments: [
			{
				name: "matrise_x",
				description: "er det første området eller den første matrisen med verdier, og kan være et tall eller et navn, en matrise eller en referanse som inneholder tall"
			},
			{
				name: "matrise_y",
				description: "er det andre området eller den andre matrisen med verdier, og kan være et tall eller et navn, en matrise eller en referanse som inneholder tall"
			}
		]
	},
	{
		name: "SVPLAN",
		description: "Returnerer sluttverdien av en inngående hovedstol etter å ha brukt en serie med sammensatte rentesatser.",
		arguments: [
			{
				name: "hovedstol",
				description: "er nåverdien"
			},
			{
				name: "plan",
				description: "er en matrise av rentesatser som skal brukes"
			}
		]
	},
	{
		name: "T",
		description: "Kontrollerer om en verdi er tekst, og returnerer i så fall teksten, hvis ikke returnerer den doble anførselstegn (tom tekst).",
		arguments: [
			{
				name: "verdi",
				description: "er verdien du vil teste"
			}
		]
	},
	{
		name: "T.FORDELING",
		description: "Returnerer den venstre Student T-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske verdien du vil evaluere fordelingen for"
			},
			{
				name: "frihetsgrader",
				description: "er et heltall som angir antall frihetsgrader som kjennetegner fordelingen"
			},
			{
				name: "kumulative",
				description: "er en logisk verdi: Bruk SANN for kumulativ fordelingsfunksjon, bruk USANN for funksjon for sannsynlig tetthet"
			}
		]
	},
	{
		name: "T.FORDELING.2T",
		description: "Returnerer den tosidige Student T-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske verdien du vil evaluere fordelingen for"
			},
			{
				name: "frihetsgrader",
				description: "er et heltall som angir antall frihetsgrader som karakteriserer fordelingen"
			}
		]
	},
	{
		name: "T.FORDELING.H",
		description: "Returnerer den høyre Student T-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske verdien du vil evaluere fordelingen for"
			},
			{
				name: "frihetsgrader",
				description: "er et heltall som angir antall frihetsgrader som karakteriserer fordelingen"
			}
		]
	},
	{
		name: "T.INV",
		description: "Returnerer den venstre inverse av Student t-fordelingen.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er sannsynligheten som er knyttet til den tosidige Student t-fordelingen, et tall mellom 0 og 1, inklusive"
			},
			{
				name: "frihetsgrader",
				description: "er et positivt heltall som angir antallet frihetsgrader som karakteriserer fordelingen"
			}
		]
	},
	{
		name: "T.INV.2T",
		description: "Returnerer den tosidige inverse av Student T-fordelingen.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er sannsynligheten som er knyttet til den tosidige Student T-fordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "frihetsgrader",
				description: "er et positivt heltall som angir antall frihetsgrader som karakteriserer fordelingen"
			}
		]
	},
	{
		name: "T.TEST",
		description: "Returnerer sannsynligheten knyttet til en t-test.",
		arguments: [
			{
				name: "matrise1",
				description: "er det første datasettet"
			},
			{
				name: "matrise2",
				description: "er det andre datasettet"
			},
			{
				name: "sider",
				description: "angir hvor mange fordelingssider som skal returneres. Ensidig fordeling = 1, tosidig fordeling = 2"
			},
			{
				name: "type",
				description: "er typen T-test som skal utføres. Parvis = 1, to utvalg med lik varians (homoskedastisk) = 2, to utvalg med ulik varians = 3"
			}
		]
	},
	{
		name: "TALLVERDI",
		description: "Konverterer tekst til tall på en måte som er uavhengig av nasjonal innstilling.",
		arguments: [
			{
				name: "tekst",
				description: "er strengen som representerer tallet du vil konvertere"
			},
			{
				name: "desimalskilletegn",
				description: "er tegnet som brukes som desimalskilletegn i strengen"
			},
			{
				name: "gruppeskilletegn",
				description: "er tegnet som brukes som skilletegn for gruppe i en streng"
			}
		]
	},
	{
		name: "TAN",
		description: "Returnerer tangens for en vinkel.",
		arguments: [
			{
				name: "tall",
				description: "er vinkelen du vil finne tangens til, i radianer. Grader * PI()/180 = radianer"
			}
		]
	},
	{
		name: "TANH",
		description: "Returnerer den hyperbolske tangens til et tall.",
		arguments: [
			{
				name: "tall",
				description: "er ethvert reelt tall"
			}
		]
	},
	{
		name: "TBILLAVKASTNING",
		description: "Returnerer avkastningen for en statsobligasjon.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er statsobligasjonens betalingsdato uttrykt som et serienummer "
			},
			{
				name: "forfallsdato",
				description: "er statsobligasjonens forfallsdato uttrykt som et serienummer "
			},
			{
				name: "diskonto",
				description: "er statsobligasjonens diskontosats "
			}
		]
	},
	{
		name: "TBILLEKV",
		description: "Returnerer verdipapirekvivalenten til en statsobligasjon.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er statsobligasjonens betalingsdato uttrykt som et serienummer"
			},
			{
				name: "forfallsdato",
				description: "er statsobligasjonens forfallsdato uttrykt som et serienummer"
			},
			{
				name: "diskonto",
				description: "er statsobligasjonens diskontosats"
			}
		]
	},
	{
		name: "TBILLPRIS",
		description: "Returnerer prisen per pålydende kr 100 for en statsobligasjon.",
		arguments: [
			{
				name: "betalingsdato",
				description: "er statsobligasjonens betalingsdato uttrykt som et serienummer "
			},
			{
				name: "forfallsdato",
				description: "er statsobligasjonens forfallsdato uttrykt som et serienummer "
			},
			{
				name: "diskonto",
				description: "er statsobligasjonens diskontosats "
			}
		]
	},
	{
		name: "TEGNKODE",
		description: "Returnerer tegnet som svarer til kodenummeret fra tegnsettet på din datamaskin.",
		arguments: [
			{
				name: "tall",
				description: "er et tall mellom 1 og 255 som svarer til tegnet du ønsker"
			}
		]
	},
	{
		name: "TEKST",
		description: "Konverterer en verdi til tekst i et bestemt nummerformat.",
		arguments: [
			{
				name: "verdi",
				description: "et et tall, en formel som returnerer en numerisk verdi, eller en referanse til en celle som inneholder en numerisk verdi"
			},
			{
				name: "format",
				description: "er et tallformat i tekstform under Kategori i kategorien Tall i dialogboksen Formater celler (ikke Generelt)"
			}
		]
	},
	{
		name: "TELLBLANKE",
		description: "Teller antall tomme celler innenfor et område.",
		arguments: [
			{
				name: "område",
				description: "er området du vil telle tomme celler i"
			}
		]
	},
	{
		name: "TFORDELING",
		description: "Returnerer Student t-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er den numeriske verdien du vil regne ut fordelingen for"
			},
			{
				name: "frihetsgrader",
				description: "er et heltall som angir antallet frihetsgrader som karakteriserer fordelingen"
			},
			{
				name: "sider",
				description: "angir hvor mange fordelingssider som skal returneres. Ensidig fordeling = 1. Tosidig fordeling = 2"
			}
		]
	},
	{
		name: "TID",
		description: "Konverterer timer, minutter og sekunder som er gitt som tall, til et Spreadsheet-serienummer, formatert etter et klokkeslettsformat.",
		arguments: [
			{
				name: "time",
				description: "er et tall fra 0 til 23 som representerer timen"
			},
			{
				name: "minutt",
				description: "er et tall fra 0 til 59 som representerer minuttet"
			},
			{
				name: "sekund",
				description: "er et tall fra 0 til 59 som representerer sekundet"
			}
		]
	},
	{
		name: "TIDSVERDI",
		description: "Konverterer teksttid til et Spreadsheet-serienummer for et klokkeslett, et tall fra 0 (12:00:00 AM) til 0.999988426 (11:59:59 PM). Formaterer nummeret med et klokkeslettformat etter å ha innført formelen.",
		arguments: [
			{
				name: "tidstekst",
				description: "er en tekststreng som angir klokkeslettet i et av klokkeslettformatene i Spreadsheet (datoinformasjon i strengen ignoreres)"
			}
		]
	},
	{
		name: "TILFELDIG",
		description: "Returnerer et tilfeldig tall som er lik eller større enn 0 og mindre enn 1 (endres ved omberegning).",
		arguments: [
		]
	},
	{
		name: "TILFELDIGMELLOM",
		description: "Returnerer et tilfeldig tall mellom tallene du angir.",
		arguments: [
			{
				name: "bunn",
				description: "er det minste heltallet TILFELDIGMELLOM returnerer"
			},
			{
				name: "topp",
				description: "er det største heltallet TILFELDIGMELLOM returnerer"
			}
		]
	},
	{
		name: "TIME",
		description: "Returnerer time på dagen som et tall fra 0 (12:00 AM) til 23 (11:00 PM).",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato eller et klokkeslett som brukes av Spreadsheet, eller tekst i klokkeslettformat, for eksempel 16:48:00 eller 4:48:00 PM"
			}
		]
	},
	{
		name: "TINV",
		description: "Returnerer den inverse (tosidig) av Student t-fordelingen.",
		arguments: [
			{
				name: "sannsynlighet",
				description: "er sannsynligheten knyttet til den tosidige Student t-fordelingen, et tall fra og med 0 til og med 1"
			},
			{
				name: "frihetsgrader",
				description: "er et positivt heltall som indikerer antallet frihetsgrader som karakteriserer fordelingen"
			}
		]
	},
	{
		name: "TRANSPONER",
		description: "Konverterer et vertikalt celleområde til et horisontalt celleområde, eller omvendt.",
		arguments: [
			{
				name: "matrise",
				description: "er et celleområde i et regneark eller i en matrise der du vil bytte om rader og kolonner"
			}
		]
	},
	{
		name: "TREND",
		description: "Returnerer tall i en lineær trend som samsvarer med kjente datapunkter, ved hjelp av minste kvadraters metode.",
		arguments: [
			{
				name: "kjente_y",
				description: "er et område eller en matrise med y-verdier du allerede kjenner i forholdet y = mx + b"
			},
			{
				name: "kjente_x",
				description: "er et valgfritt sett med x-verdier det kan hende du allerede kjenner i forholdet y = mx + b, en matrise av samme størrelse som kjente_y"
			},
			{
				name: "nye_x",
				description: "er et område eller en matrise med nye x-verdier som du vil at funksjonen TREND skal returnere de tilsvarende y-verdiene for"
			},
			{
				name: "konst",
				description: "er en logisk verdi. Konstanten b beregnes normalt hvis konst = SANN eller utelatt, b settes til 0 hvis konst = USANN"
			}
		]
	},
	{
		name: "TRIMME",
		description: "Fjerner alle mellomrom fra tekst unntatt enkle mellomrom mellom ord.",
		arguments: [
			{
				name: "tekst",
				description: "er teksten du vil fjerne mellomrom fra"
			}
		]
	},
	{
		name: "TRIMMET.GJENNOMSNITT",
		description: "Returnerer det trimmede gjennomsnittet av et sett dataverdier.",
		arguments: [
			{
				name: "matrise",
				description: "er området eller matrisen som inneholder verdiene du vil trimme og beregne gjennomsnittet for"
			},
			{
				name: "prosent",
				description: "er andelen datapunkter som skal utelates fra toppen og bunnen av datasettet"
			}
		]
	},
	{
		name: "TTEST",
		description: "Returnerer sannsynligheten knyttet til en t-test.",
		arguments: [
			{
				name: "matrise1",
				description: "er det første datasettet"
			},
			{
				name: "matrise2",
				description: "er det andre datasettet"
			},
			{
				name: "sider",
				description: "angir hvor mange fordelingssider som skal returneres. Ensidig fordeling = 1, tosidig fordeling = 2"
			},
			{
				name: "type",
				description: "er den typen t-test som skal utføres. Partest = 1, to utvalg med lik varians (homoskedastisk) = 2, to utvalg med ulik varians = 3"
			}
		]
	},
	{
		name: "UKEDAG",
		description: "Returnerer et tall fra 1 til 7 som representerer ukedagen.",
		arguments: [
			{
				name: "serienummer",
				description: "er et tall som representerer en dato"
			},
			{
				name: "retur_type",
				description: "er et tall. For søndag = 1 til lørdag = 7 bruker du 1, for mandag = 1 til søndag = 7 bruker du 2, for mandag = 0 til søndag = 6 bruker du 3"
			}
		]
	},
	{
		name: "UKENR",
		description: "Returnerer ukenummeret i et år.",
		arguments: [
			{
				name: "tall",
				description: "er koden som brukes til å beregne dato og klokkeslett i Spreadsheet"
			},
			{
				name: "returtype",
				description: "er et tall (1 eller 2) som fastsetter typen returverdi"
			}
		]
	},
	{
		name: "UNICODE",
		description: "Returnerer tallet (kodepunktet) som tilsvarer det første tegnet i teksten.",
		arguments: [
			{
				name: "tekst",
				description: "er tegnet du vil finne Unicode-verdien for"
			}
		]
	},
	{
		name: "URL.KODE",
		description: "Returnerer en URL-kodet streng.",
		arguments: [
			{
				name: "text",
				description: "er en streng som skal URL-kodes"
			}
		]
	},
	{
		name: "USANN",
		description: "Returnerer den logiske verdien USANN.",
		arguments: [
		]
	},
	{
		name: "VALUTA",
		description: "Konverterer et tall til tekst i valutaformat.",
		arguments: [
			{
				name: "tall",
				description: "er et tall, en referanse til en celle som inneholder et tall, eller en formel som returnerer et tall"
			},
			{
				name: "desimaler",
				description: "er antall sifre til høyre for desimaltegnet. Tallet blir avrundet etter behov. Hvis argumentet utelates, settes Desimaler til 2"
			}
		]
	},
	{
		name: "VARIANS",
		description: "Beregner varians basert på et utvalg (ignorerer logiske verdier og tekst i utvalget).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tallargumenter som svarer til et utvalg fra en populasjon"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tallargumenter som svarer til et utvalg fra en populasjon"
			}
		]
	},
	{
		name: "VARIANS.P",
		description: "Beregner varians basert på hele populasjonen (ignorerer logiske verdier og tekst i populasjonen).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tallargumenter som svarer til en populasjon"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tallargumenter som svarer til en populasjon"
			}
		]
	},
	{
		name: "VARIANS.S",
		description: "Beregner varians basert på et utvalg (ignorerer logiske verdier og tekst i utvalget).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tallargumenter som tilsvarer et utvalg fra en populasjon"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tallargumenter som tilsvarer et utvalg fra en populasjon"
			}
		]
	},
	{
		name: "VARIANSA",
		description: "Anslår varians basert på et utvalg, inkludert logiske verdier og tekst. Tekst og den logiske verdien USANN har verdi 0, den logiske verdien SANN har verdi 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 verdiargumenter som tilsvarer et utvalg fra en populasjon"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 verdiargumenter som tilsvarer et utvalg fra en populasjon"
			}
		]
	},
	{
		name: "VARIANSP",
		description: "Beregner varians basert på hele populasjonen (ignorerer logiske verdier og tekst i utvalget).",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "tall1",
				description: "er 1 til 255 tallargumenter som tilsvarer en populasjon"
			},
			{
				name: "tall2",
				description: "er 1 til 255 tallargumenter som tilsvarer en populasjon"
			}
		]
	},
	{
		name: "VARIANSPA",
		description: "Beregner varians basert på hele populasjonen, inkludert logiske verdier og tekst. Tekst og den logiske verdien USANN har verdien 0, den logiske verdien SANN har verdien 1.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "verdi1",
				description: "er 1 til 255 verdiargumenter som utgjør en populasjon"
			},
			{
				name: "verdi2",
				description: "er 1 til 255 verdiargumenter som utgjør en populasjon"
			}
		]
	},
	{
		name: "VEKST",
		description: "Returnerer tall i en eksponentiell veksttrend som samsvarer med kjente datapunkter.",
		arguments: [
			{
				name: "kjente_y",
				description: "er et sett y-verdier du allerede kjenner i forholdet y = b*m^x, en matrise eller et intervall av positive tall"
			},
			{
				name: "kjente_x",
				description: "er et valgfritt sett med x-verdier som det kan hende du allerede kjenner, i forholdet y = b*m^x, en matrise eller et intervall av samme størrelse som kjente_y"
			},
			{
				name: "nye_x",
				description: "er nye x-verdier som du vil at funksjonen VEKST skal returnere de tilsvarende y-verdiene for"
			},
			{
				name: "konst",
				description: "er en logisk verdi. Konstanten b beregnes normalt hvis konst = SANN, b settes til 1 hvis konst = USANN eller utelatt"
			}
		]
	},
	{
		name: "VELG",
		description: "Velger en verdi eller en handling fra en liste med verdier, basert på et indekstall.",
		hasUnlimitedParametersCount: true,
		arguments: [
			{
				name: "indeks",
				description: "angir hvilket verdiargument som er valgt. Indeks må være mellom 1 og 254, eller en formel eller en referanse til et tall mellom 1 og 254."
			},
			{
				name: "verdi1",
				description: "er 1 til 254 tall, cellereferanser, definerte navn, formler, funksjoner eller tekstargumenter som funksjonen VELG velger fra"
			},
			{
				name: "verdi2",
				description: "er 1 til 254 tall, cellereferanser, definerte navn, formler, funksjoner eller tekstargumenter som funksjonen VELG velger fra"
			}
		]
	},
	{
		name: "VENSTRE",
		description: "Returnerer det angitte antall tegn fra begynnelsen av en tekststreng.",
		arguments: [
			{
				name: "tekst",
				description: "er tekststrengen som inneholder tegnene du vil trekke ut"
			},
			{
				name: "antall_tegn",
				description: "angir hvor mange tegn du vil at funksjonen VENSTRE skal trekke ut. Settes til 1 hvis argumentet utelates"
			}
		]
	},
	{
		name: "VERDI",
		description: "Konverterer en tekststreng som representerer et tall, til et tall.",
		arguments: [
			{
				name: "tekst",
				description: "er tekst som står mellom anførselstegn, eller en referanse til en celle som inneholder teksten du vil konvertere"
			}
		]
	},
	{
		name: "VERDIAVS",
		description: "Returnerer avskrivningen på et aktivum for en periode du angir, medregnet delperioder, ved hjelp av dobbel degressiv avskrivning eller en annen metode du angir.",
		arguments: [
			{
				name: "kostnad",
				description: "er den opprinnelige kostnaden til aktivumet"
			},
			{
				name: "restverdi",
				description: "er verdien ved slutten av avskrivningen"
			},
			{
				name: "levetid",
				description: "er antall perioder et aktivum blir avskrevet over (ofte kalt aktivumets økonomiske levetid)"
			},
			{
				name: "start_periode",
				description: "er startperioden du vil beregne avskrivningen fra, med samme enheter som levetid"
			},
			{
				name: "slutt_periode",
				description: "er sluttperioden du vil beregne avskrivningen til, med samme enheter som levetid"
			},
			{
				name: "faktor",
				description: "er satsen verdien avskrives med. Settes til 2 (dobbel degressiv avskrivning) hvis argumentet utelates"
			},
			{
				name: "skift",
				description: "bytt til lineær avskriving når avskrivingen er større enn verdiforringelsen = USANN eller utelatt, ikke bytt = SANN"
			}
		]
	},
	{
		name: "VERDITYPE",
		description: "Returnerer et heltall som representerer datatypen til verdien: tall = 1, tekst = 2, logisk verdi =4, feilverdi = 16, matrise = 64.",
		arguments: [
			{
				name: "verdi",
				description: "kan være en hvilken som helst verdi"
			}
		]
	},
	{
		name: "WEIBULL.DIST.N",
		description: "Returnerer Weibull-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil regne ut funksjonen for, et ikke-negativt tall"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For den kumulative fordelingsfunksjonen bruker du SANN, for punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "WEIBULL.FORDELING",
		description: "Returnerer Weibull-fordelingen.",
		arguments: [
			{
				name: "x",
				description: "er verdien du vil regne ut funksjonen for, et ikke-negativt tall"
			},
			{
				name: "alfa",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "beta",
				description: "er en parameter for fordelingen, et positivt tall"
			},
			{
				name: "kumulativ",
				description: "er en logisk verdi. For kumulativ fordeling bruker du SANN. For punktsannsynlighet bruker du USANN"
			}
		]
	},
	{
		name: "XIR",
		description: "Returnerer internrenten for en serie kontantstrømmer.",
		arguments: [
			{
				name: "verdier",
				description: "er en serie kontantstrømmer som tilsvarer en innbetalingsplan i datoer"
			},
			{
				name: "datoer",
				description: "er en plan over betalingsdatoer som tilsvarer kontantstrømbetalingene"
			},
			{
				name: "anslag",
				description: "er et tall du antar er i nærheten av resultatet av XIR"
			}
		]
	},
	{
		name: "XNNV",
		description: "Returnerer netto nåverdi av planlagte kontantstrømmer.",
		arguments: [
			{
				name: "rente",
				description: "er diskontosats for kontantstrømmene"
			},
			{
				name: "verdier",
				description: "er en serie kontanstrømmer som korresponderer med en betalingsplan i datoer"
			},
			{
				name: "datoer",
				description: "er en betalingsdatoplan som korresponderer med kontantstrømbetalingene"
			}
		]
	},
	{
		name: "Z.TEST",
		description: "Returnerer den ensidige P-verdien i en z-test.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet du vil teste X mot"
			},
			{
				name: "x",
				description: "er verdien som skal testes"
			},
			{
				name: "sigma",
				description: "er populasjonens (kjente) standardavvik. Hvis argumentet utelates, brukes standardavviket for utvalget"
			}
		]
	},
	{
		name: "ZTEST",
		description: "Returnerer den ensidige P-verdien for en z-test.",
		arguments: [
			{
				name: "matrise",
				description: "er matrisen eller dataområdet du vil teste X mot"
			},
			{
				name: "x",
				description: "er verdien som skal testes"
			},
			{
				name: "sigma",
				description: "er populasjonens (kjente) standardavvik. Hvis argumentet utelates, brukes standardavviket for utvalget"
			}
		]
	}
];