namespace nAnyWebApp.Models;

public class ScriptPreset
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public static class ScriptPresets
{
    public static readonly ScriptPreset[] CssPresets =
    [
        new ScriptPreset
        {
            Name = "🌙 Ciemny motyw (Universal Dark)",
            Description = "Wymusza ciemny motyw na stronie poprzez odwrócenie kolorów i zachowanie barw obrazów.",
            Code = @"/* Wymuszony ciemny motyw */
html {
    filter: invert(0.92) hue-rotate(180deg) !important;
    background-color: #121212 !important;
}
img, video, iframe, canvas, svg {
    filter: invert(1) hue-rotate(180deg) !important;
}"
        },
        new ScriptPreset
        {
            Name = "🍪 Ukryj banery zgód i cookies",
            Description = "Ukrywa powszechne nakładki i paski z prośbą o akceptację ciasteczek / RODO.",
            Code = @"/* Ukrywanie banerów cookies */
[id*='cookie'], [class*='cookie'],
[id*='consent'], [class*='consent'],
[id*='notice-banner'], [class*='notice-banner'],
[id*='banner-cookie'], [class*='privacy-banner'] {
    display: none !important;
    visibility: hidden !important;
    opacity: 0 !important;
    pointer-events: none !important;
}"
        },
        new ScriptPreset
        {
            Name = "📖 Tryb czytnika (Czysty tekst)",
            Description = "Powiększa czcionkę i ogranicza szerokość tekstu dla wygody czytania.",
            Code = @"/* Tryb czytania */
body {
    max-width: 860px !important;
    margin: 0 auto !important;
    font-size: 18px !important;
    line-height: 1.6 !important;
    padding: 16px !important;
}"
        },
        new ScriptPreset
        {
            Name = "📱 Ukryj lepkie paski nawigacji (Sticky)",
            Description = "Usuwa przyklejone do góry i dołu paski, aby uzyskać więcej miejsca na ekranie.",
            Code = @"/* Usuwanie sticky header / footer */
header[style*='fixed'], div[style*='fixed'],
header[style*='sticky'], div[style*='sticky'] {
    position: static !important;
}"
        }
    ];

    public static readonly ScriptPreset[] JsPresets =
    [
        new ScriptPreset
        {
            Name = "🍪 Auto-akceptacja / ukrycie Cookies (JS)",
            Description = "Wyszukuje przyciski zgód (Zgadzam się / Accept / Agree) i automatycznie je klika.",
            Code = @"// Automatyczne klikanie przycisków akceptacji ciasteczek
(function() {
    setTimeout(function() {
        const keywords = ['zgadzam', 'akceptuj', 'accept', 'agree', 'allow all', 'accept all', 'rozumiem'];
        const buttons = Array.from(document.querySelectorAll('button, a, input[type=""button""], input[type=""submit""]'));
        for (const btn of buttons) {
            const text = (btn.innerText || btn.value || '').trim().toLowerCase();
            if (keywords.some(k => text.includes(k))) {
                console.log('[nAnyWebApp] Auto-clicking consent button:', btn);
                btn.click();
                break;
            }
        }
    }, 1500);
})();"
        },
        new ScriptPreset
        {
            Name = "🚫 Zezwalaj na zaznaczanie i kopiowanie tekstu",
            Description = "Odblokowuje możliwość zaznaczania tekstu i menu kontekstowe na zablokowanych stronach.",
            Code = @"// Odblokowanie zaznaczania i kopiowania
(function() {
    const enableSelect = () => {
        document.oncontextmenu = null;
        document.onselectstart = null;
        document.ondragstart = null;
        document.onmousedown = null;
        document.body.style.userSelect = 'auto';
        document.body.style.webkitUserSelect = 'auto';
    };
    enableSelect();
    window.addEventListener('load', enableSelect);
})();"
        },
        new ScriptPreset
        {
            Name = "⚡ Logowanie zdarzeń (Debug)",
            Description = "Wypisuje informację w konsoli o pomyślnym załadowaniu aplikacji w nAnyWebApp.",
            Code = @"// Konsola debugowania nAnyWebApp
console.log('%c [nAnyWebApp] Witaj w przekształconej aplikacji! ', 'background: #512BD4; color: #fff; font-size: 14px; border-radius: 4px; padding: 4px 8px;');
console.log('[nAnyWebApp] Adres URL:', window.location.href);
console.log('[nAnyWebApp] User Agent:', navigator.userAgent);"
        }
    ];
}
