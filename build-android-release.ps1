# Skrypt do generowania produkcyjnej paczki Android App Bundle (.aab) dla Google Play Console
param(
    [string]$KeystoreFile = "nanywebapp-upload.keystore",
    [string]$KeyAlias = "nanywebapp",
    [string]$Password = ""
)

$ErrorActionPreference = "Stop"

Write-Host "=== nAnyWebApp - Budowanie paczki Google Play (AAB) ===" -ForegroundColor Cyan

# Lokalizacja keytool (kompatybilna z roznymi wersjami JDK / PowerShell 5.1 i 7+)
$keytoolCandidates = @(
    "C:\Program Files\Android\openjdk\jdk-21.0.8\bin\keytool.exe",
    "C:\Program Files (x86)\Android\openjdk\jdk-21.0.8\bin\keytool.exe"
)

$keytoolPath = ""
foreach ($candidate in $keytoolCandidates) {
    if (Test-Path $candidate) {
        $keytoolPath = $candidate
        break
    }
}

if ([string]::IsNullOrWhiteSpace($keytoolPath)) {
    $cmd = Get-Command "keytool.exe" -ErrorAction SilentlyContinue
    if ($cmd) {
        $keytoolPath = $cmd.Source
    }
}

if (-not (Test-Path $KeystoreFile)) {
    Write-Host "`nNie znaleziono pliku klucza: $KeystoreFile" -ForegroundColor Yellow
    Write-Host "Generowanie nowego klucza podpisu (Upload Keystore)..." -ForegroundColor Green
    
    if ([string]::IsNullOrWhiteSpace($Password)) {
        $secPwd = Read-Host -Prompt "Podaj bezpieczne haslo dla nowego klucza podpisu" -AsSecureString
        $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secPwd)
        $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    }

    if ([string]::IsNullOrWhiteSpace($keytoolPath) -or (-not (Test-Path $keytoolPath))) {
        Write-Error "Nie znaleziono narzedzia keytool.exe. Upewnij sie, ze zainstalowano pakiet OpenJDK."
        exit 1
    }

    & $keytoolPath -genkeypair -v -keystore $KeystoreFile -alias $KeyAlias -keyalg RSA -keysize 2048 -validity 10000 `
        -storepass $Password -keypass $Password -dname "CN=nAnyWebApp, O=gfmm, C=EU"
        
    Write-Host "Utworzono klucz: $KeystoreFile (Alias: $KeyAlias)" -ForegroundColor Green
    Write-Host "UWAGA: Zachowaj ten plik i haslo w bezpiecznym miejscu! Bedzie potrzebny do kazdej kolejnej aktualizacji w Google Play." -ForegroundColor Red
}

if ([string]::IsNullOrWhiteSpace($Password)) {
    $secPwd = Read-Host -Prompt "Podaj haslo do magazynu kluczy $KeystoreFile" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secPwd)
    $Password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

Write-Host "`nKompilacja i publikacja do Android App Bundle (AAB)..." -ForegroundColor Cyan

dotnet publish nAnyWebApp/nAnyWebApp.csproj -f net9.0-android -c Release `
    -p:AndroidPackageFormat=aab `
    -p:AndroidKeyStore=true `
    -p:AndroidSigningKeyStore="..\$KeystoreFile" `
    -p:AndroidSigningKeyAlias=$KeyAlias `
    -p:AndroidSigningKeyPass=$Password `
    -p:AndroidSigningStorePass=$Password

$publishDir = "nAnyWebApp\bin\Release\net9.0-android\publish"
$outputAab = Get-ChildItem -Path $publishDir -Filter "*Signed.aab" -ErrorAction SilentlyContinue | Select-Object -First 1

if ($outputAab) {
    Write-Host "`nSUKCES! Gotowa paczka do wgrania do Google Play Console:" -ForegroundColor Green
    Write-Host $outputAab.FullName -ForegroundColor Yellow
} else {
    $fallbackAab = Get-ChildItem -Path "nAnyWebApp\bin\Release\net9.0-android" -Filter "*.aab" -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($fallbackAab) {
        Write-Host "`nPaczka wygenerowana w:" -ForegroundColor Green
        Write-Host $fallbackAab.FullName -ForegroundColor Yellow
    }
}
