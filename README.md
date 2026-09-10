# nAnyWebApp 🌐

> Przekształcaj ulubione strony internetowe w dedykowane aplikacje na systemy **Windows** i **Android** z zaawansowanym wstrzykiwaniem **CSS** i **JavaScript**, dynamicznym menu oraz skrótami na pulpit i ekran główny!

Aplikacja zbudowana w technologii **.NET MAUI** z interfejsem użytkownika stworzonym w 100% w **C# Markup** (`CommunityToolkit.Maui.Markup`).

---

## ✨ Główne funkcjonalności

- 🎨 **Czysty C# Markup**: Interfejs użytkownika napisany w całości deklaratywnym kodem C# bez narzutu i ograniczeń XAML.
- 💉 **Zaawansowane wstrzykiwanie stylów i skryptów**:
  - **Globalne style CSS i kod JavaScript**: Aplikowane dla całej domeny po załadowaniu drzewa DOM.
  - **Reguły dla podstron (URL Fragment / Pattern)**: Możliwość zdefiniowania dedykowanych stylów i skryptów uruchamianych tylko wtedy, gdy adres URL zawiera określony fragment (np. `/watch` dla wideo na YouTube, `issues` na GitHubie, `/settings` itp.).
  - **Bezpieczne kodowanie UTF-8 Base64**: Eliminuje błędy związane ze znakami specjalnymi, polskimi znakami diakrytycznymi oraz cudzysłowami w kodzie CSS/JS.
  - **Gotowe szablony**: Wbudowane presety (m.in. uniwersalny ciemny motyw *Universal Dark*, automatyczne ukrywanie banerów cookies/RODO, tryb czytnika, odblokowywanie zaznaczania tekstu).
- 📑 **Dynamiczne menu boczne (Shell Flyout)**:
  - Każda dodana witryna automatycznie staje się osobną pozycją w menu bocznym (Flyout) z własną ikoną favicon i nazwą.
  - Szybkie przełączanie między witrynami jednym kliknięciem.
  - Dedykowany panel **„📑 Strony”** do zarządzania wszystkimi aplikacjami (dodawanie, edycja, usuwanie, podgląd).
  - Wygodny pasek narzędzi WebView z przyciskami: `☰ Menu`, `📑 Strony`, nawigacji `⬅️`/`➡️`/`🔄`, ponownego wstrzykiwania `⚡ Skrypty` oraz trybu pełnoekranowego `⛶ Pełny ekran`.
- 📌 **Skróty na Pulpicie (Windows) i Ekranie Głównym (Android)**:
  - **Windows**: Generowanie natywnych skrótów `.lnk` na Pulpicie użytkownika z argumentem uruchomieniowym `--webapp-id {id}`.
  - **Android**: Tworzenie przypiętych skrótów na ekranie głównym za pośrednictwem natywnego `ShortcutManager` (Android 8.0+ / API 26+).
  - **Deep-linking**: Uruchomienie aplikacji ze skrótu natychmiast przenosi użytkownika do wybranej strony internetowej.
- 💾 **Kopia zapasowa i synchronizacja JSON**:
  - Trwały zapis konfiguracji w pliku `nany_webapps.json` w bezpiecznym katalogu danych aplikacji.
  - Funkcja eksportu do schowka i importu konfiguracji w formacie JSON na innym urządzeniu.
  - Automatyczne wykrywanie ikon favicon na podstawie adresu URL witryny.

---

## 🏗️ Architektura projektu

```text
nAnyWebApp/
├── Models/
│   ├── WebApps.cs              # Model WebApp oraz UrlScriptRule (reguły dla fragmentów URL)
│   └── ScriptPresets.cs        # Gotowe szablony CSS i JavaScript
├── Services/
│   ├── IWebAppService.cs       # Interfejs CRUD i zarządzania bazą stron
│   ├── WebAppService.cs        # Zapis i odczyt JSON, pobieranie favicon, powiadomienia
│   ├── IScriptInjectorService.cs # Interfejs wstrzykiwania kodu
│   ├── ScriptInjectorService.cs # Bezpieczne wstrzykiwanie UTF-8 Base64
│   ├── IShortcutService.cs     # Interfejs tworzenia skrótów
│   └── DefaultShortcutService.cs # Domyślny fallback dla innych platform
├── ViewModels/
│   ├── BaseViewModel.cs        # Klasa bazowa MVVM
│   ├── MainViewModel.cs        # Zarządzanie listą, filtrowanie, usuwanie i uruchamianie
│   ├── WebAppEditViewModel.cs  # Formularz edycji, pobieranie favicon, reguły podstron
│   ├── nWebViewViewModel.cs    # Nawigacja w przeglądarce, obsługa skrótów, wstrzykiwanie
│   └── ustawieniaViewModel.cs  # Kopia zapasowa JSON i informacje
├── Views/ (100% C# Markup)
│   ├── MainPage.cs             # Lista stron z wyszukiwarką i kartami aplikacji
│   ├── WebAppEditPage.cs       # Edytor strony i podstron z podglądem
│   ├── nWebViewPage.cs         # Widok WebView z ukrywalnym paskiem i trybem pełnoekranowym
│   └── ustawieniaPage.cs       # Ustawienia aplikacji i kopia zapasowa
├── Platforms/
│   ├── Windows/
│   │   └── WindowsShortcutService.cs # Tworzenie skrótów .lnk na Pulpicie
│   └── Android/
│       ├── AndroidShortcutService.cs # Natywny ShortcutManager Android
│       └── MainActivity.cs           # Obsługa Intentu uruchomienia ze skrótu
├── AppShell.cs                 # Dynamiczne menu Shell Flyout
└── MauiProgram.cs              # Konfiguracja kontenera DI i toolkitów
```

---

## 🚀 Wymagania i uruchomienie

### Wymagania
- [.NET 9 SDK](https://dotnet.microsoft.com/download) lub nowszy
- Obciążenia .NET MAUI (`maui-windows`, `android`)
- Windows 10/11 (dla wersji Windows)
- Android SDK / urządzenie z Androidem 8.0+ (API 26+)

### Kompilacja i uruchomienie

#### Windows
```powershell
dotnet build -f net9.0-windows10.0.19041.0 nAnyWebApp/nAnyWebApp.csproj
dotnet run --project nAnyWebApp/nAnyWebApp.csproj -f net9.0-windows10.0.19041.0
```

#### Android
```powershell
dotnet build -f net9.0-android nAnyWebApp/nAnyWebApp.csproj
dotnet build -t:Run -f net9.0-android nAnyWebApp/nAnyWebApp.csproj
```

---

## 📝 Licencja

Projekt udostępniany na licencji MIT. Repozytorium: [https://github.com/nurbsn/nAnyWebApp](https://github.com/nurbsn/nAnyWebApp).
