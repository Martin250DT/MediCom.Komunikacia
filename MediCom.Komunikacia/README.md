# MediCom ChatGPT WPF

Jednoduchá desktopová aplikácia vo WPF (C# / .NET Framework 4.8), ktorá vizuálne pripomína WhatsApp Web a posiela správy do ChatGPT cez OpenAI API.

## Funkcie
- dvojpanelový layout podobný WhatsAppu,
- chat bubliny pre používateľa a asistenta,
- stavová lišta s informáciou o odosielaní,
- priame volanie OpenAI Chat Completions API,
- jednoduchá konfigurácia modelu a API kľúča v `App.config`.

## Spustenie
1. Otvor `MediCom.Komunikacia.sln` vo Visual Studiu na Windows.
2. Do `App.config` doplň hodnotu `OpenAiApiKey`.
3. Prípadne zmeň model v `OpenAiModel`.
4. Spusť projekt klávesom `F5`.

## Poznámka
Projekt cieli na .NET Framework 4.8, preto je určený pre build a spustenie vo Visual Studiu / MSBuild na Windows.
